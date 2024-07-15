using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.Util;
using Oracle.DataAccess.Client;
using WSPlataforma.Entities.ReporteInconsistenciasSanitasModel.ViewModel;
using WSPlataforma.Entities.ReporteInconsistenciasSanitasModel.BindingModel;
using System.Data;
using System.Configuration;
using Oracle.DataAccess.Types;

namespace WSPlataforma.DA
{
    public class ReporteInconsistenciasSanitasDA : ConnectionBase
    {

        private readonly string Package = "PKG_SCTR_REPORTE_INCONSISTENCIAS_SANITAS";

        public List<RamosVM> ListarRamos()
        {
            var parameters = new List<OracleParameter>();
            var suggests = new List<RamosVM>();
            var procedure = string.Format("{0}.{1}", this.Package, "SPS_SCTR_LISTA_RAMOS");

            try
            {
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var suggest = new RamosVM();
                        suggest.NBRANCH = reader["NBRANCH"] == DBNull.Value ? 0 : int.Parse(reader["NBRANCH"].ToString());
                        suggest.SDESCRIPT = reader["SDESCRIPT"] == DBNull.Value ? string.Empty : reader["SDESCRIPT"].ToString();

                        suggests.Add(suggest);
                    }
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                ELog.save(this, ex);
                throw;
            }

            return suggests;
        }

        public DataSet generarReporte(ReporteInconsistenciasFilter data)
        {
            DataSet dataReport = new DataSet();
            DataTable dataAse = new DataTable();
            DataTable dataDif = new DataTable();
            DataTable dataDup = new DataTable();

            try
            {
                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = cn;
                        cmd.CommandText = string.Format("{0}.{1}", Package, "SPS_SCTR_GENERA_REPORTE");
                        cmd.CommandType = CommandType.StoredProcedure;

                        string fi = data.DSTARTDATE.ToString("dd/MM/yyyy");
                        string ff = data.DEXPIRDAT.ToString("dd/MM/yyyy");

                        cmd.Parameters.Add(new OracleParameter("P_RAMO", OracleDbType.Int32, data.NBRANCH, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_FECHA_INI", OracleDbType.Date, fi, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_FECHA_FIN", OracleDbType.Date, ff, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("C_TABLE_ASE", OracleDbType.RefCursor, ParameterDirection.Output));
                        cmd.Parameters.Add(new OracleParameter("C_TABLE_DIF", OracleDbType.RefCursor, ParameterDirection.Output));
                        cmd.Parameters.Add(new OracleParameter("C_TABLE_DUP", OracleDbType.RefCursor, ParameterDirection.Output));

                        cn.Open();

                        cmd.ExecuteNonQuery();

                        //Obtenemos primer cursor
                        OracleRefCursor cursorAse = (OracleRefCursor)cmd.Parameters["C_TABLE_ASE"].Value;
                        dataAse.Load(cursorAse.GetDataReader());
                        dataAse.TableName = "ASEGURADOS";
                        dataReport.Tables.Add(dataAse);

                        //Obtenemos segundo cursor
                        OracleRefCursor cursorDif = (OracleRefCursor)cmd.Parameters["C_TABLE_DIF"].Value;
                        dataDif.Load(cursorDif.GetDataReader());
                        dataDif.TableName = "DIFERENCIAS";
                        dataReport.Tables.Add(dataDif);

                        //Obtenemos tercer cursor
                        OracleRefCursor cursorDup = (OracleRefCursor)cmd.Parameters["C_TABLE_DUP"].Value;
                        dataDup.Load(cursorDup.GetDataReader());
                        dataDup.TableName = "DUPLICADOS";
                        dataReport.Tables.Add(dataDup);

                        cn.Close();
                    }
                }
            }
            catch (Exception e)
            {
                ELog.save(this, e);
                throw;
            }

            return dataReport;

        }
    }
}
