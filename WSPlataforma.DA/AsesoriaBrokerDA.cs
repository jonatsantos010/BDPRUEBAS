/*************************************************************************************************/
/*  NOMBRE              :   AsesoriaBrockerDA.CS                                                 */
/*  DESCRIPCION         :   Capa DataAccess - ejecuta procedimientos almacenados.                */
/*  AUTOR               :   MATERIAGRIS - MARCOS MATEO QUIROZ                                    */
/*  FECHA               :   03-05-2021                                                           */
/*  VERSION             :   1.0 - REQ-SCTR-004 - REPORTES ASESORÍA POR BROKER                    */
/*************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.Util;
using Oracle.DataAccess.Client;
using WSPlataforma.Entities.AsesoriaBrokerModel.ViewModel;
using WSPlataforma.Entities.AsesoriaBrokerModel.BindingModel;
using WSPlataforma.Entities.ReportModel.BindingModel;
using System.Data;


namespace WSPlataforma.DA
{
    public class AsesoriaBrokerDA : ConnectionBase
    {
        private readonly string Package = "PKG_SCTR_REPORT_BROKER"; //"PKG_SCTR_PRELIMINARY";

        //obtiene ramo
        public List<AsesoriaBrokerVM> GetBranch()
        {
            var parameters = new List<OracleParameter>();
            var suggests = new List<AsesoriaBrokerVM>();
            var procedure = string.Format("{0}.{1}", this.Package, "SPS_GET_BRANCHES");

            try
            {
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var suggest = new AsesoriaBrokerVM();
                        suggest.Id = reader["NID"] == DBNull.Value ? 0 : int.Parse(reader["NID"].ToString());
                        suggest.Description = reader["SDESCRIPTION"] == DBNull.Value ? string.Empty : reader["SDESCRIPTION"].ToString();

                        suggests.Add(suggest);
                    }
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("GetBranch", ex.ToString(), "3");
                throw;
            }

            return suggests;
        }

        //inserta tabla status de reporte
        public int InsertReportStatus(AsesoriaBrokerFilter data)
        {

            try
            {
                ResponseControl generic = new ResponseControl(Response.Ok);

                var invoices = new List<ReportAsesoriaBrokerBM>();
                var parameters = new List<OracleParameter>();
                var procedure = "SPS_INSERT_REPORT";
                int NSTATUS = 1; //insertar
                string fechaInicio = data.StartDate.ToString("dd/MM/yyyy");
                string fechaFin = data.EndDate.ToString("dd/MM/yyyy");

                parameters.Add(new OracleParameter("P_NIDREPORT", OracleDbType.Varchar2, data.IdReport, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NTYPE_REPORT", OracleDbType.Int32, data.NTYPE_REPORT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SUSERNAME", OracleDbType.Varchar2, data.UserName.ToUpper(), ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DINIREP", OracleDbType.Date, Convert.ToDateTime(fechaInicio), ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DFINREP", OracleDbType.Date, Convert.ToDateTime(fechaFin), ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.BranchId, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NSTATUSPROC", OracleDbType.Int32, NSTATUS, ParameterDirection.Input));

                //Ncode controla los errores
                var NCode = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);
                var SMessage = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                parameters.Add(NCode);
                parameters.Add(SMessage);
                NCode.Size = 4000;
                SMessage.Size = 4000;

                this.ExecuteByStoredProcedureVT(string.Format("{0}.{1}", this.Package, procedure), parameters);

                var PCode = Convert.ToInt32(NCode.Value.ToString());
                var PMessage = (SMessage.Value.ToString());

                if (PCode == 1)
                {
                    generic.Message = SMessage.Value.ToString();
                    generic.response = Response.Fail;
                }

                return PCode;// generic;
            }
            catch (Exception ex)
            {
                LogControl.save("InsertReportStatus", ex.ToString(), "3");
                throw;
            }
        }

        //obtiene lista del status de reportes
        public List<ReportAsesoriaBrokerBM> GetReportStatus(AsesoriaBrokerFilter data)
        {
            var validations = new List<ReportAsesoriaBrokerBM>();

            try
            {
                var parameters = new List<OracleParameter>();
                var procedure = "SPS_GET_REPORT_STATUS";

                string fch_inicio = data.StartDate.ToString("dd/MM/yyyy");
                string fch_fin = data.EndDate.ToString("dd/MM/yyyy");

                parameters.Add(new OracleParameter("P_NIDREPORT", OracleDbType.Varchar2, data.IdReport, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.BranchId, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DINIREP", OracleDbType.Date, Convert.ToDateTime(fch_inicio), ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DFINREP", OracleDbType.Date, Convert.ToDateTime(fch_fin), ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NTYPE_REPORT", OracleDbType.Int32, data.NTYPE_REPORT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(string.Format("{0}.{1}", this.Package, procedure), parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var validation = new ReportAsesoriaBrokerBM();

                        validation.NIDREPORT = reader["SIDREPORT"] == DBNull.Value ? string.Empty : reader["SIDREPORT"].ToString();
                        validation.SUSERNAME = reader["SUSERNAME"] == DBNull.Value ? string.Empty : reader["SUSERNAME"].ToString();
                        validation.DINIREP = reader["DINIREP"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["DINIREP"].ToString());
                        validation.DFINREP = reader["DFINREP"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["DFINREP"].ToString());
                        validation.NSTATUSPROC = reader["NSTATUSPROC"] == DBNull.Value ? 0 : int.Parse(reader["NSTATUSPROC"].ToString());
                        validation.NBRANCH = reader["NBRANCH"] == DBNull.Value ? 0 : int.Parse(reader["NBRANCH"].ToString());
                        validation.SBRANCH_NAME = reader["SBRANCH_NAME"] == DBNull.Value ? string.Empty : reader["SBRANCH_NAME"].ToString();

                        validations.Add(validation);

                    }
                }

                reader.Close();

            }
            catch (Exception ex)
            {
                LogControl.save("GetReportStatus", ex.ToString(), "3");
                throw;
            }
            return validations;
        }

    }
}
