using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Configuration;
using Oracle.DataAccess.Client;
using WSPlataforma.Util;
using WSPlataforma.Entities.TramasHistoricasModel.ViewModel;
using WSPlataforma.Entities.TramasHistoricasModel.BindingModel;
using WSPlataforma.Entities.ReportModel.BindingModel;
using SpreadsheetLight;

namespace WSPlataforma.DA
{
    public class TramasHistoricasDA : ConnectionBase
    {
        private readonly string Package = "PKG_REPORTE_ASEGURADOS_HISTORICO_VS_EMITIDO_SCTR";
        private string pathReport = string.Empty;
        //
        public Task<ProductosVM> ListarProductos()
        {
            var parameters = new List<OracleParameter>();
            ProductosVM entities = new ProductosVM();
            entities.lista = new List<ProductosVM.C_TABLE>();
            var procedure = string.Format("{0}.{1}", this.Package, "SPS_LIST_PRODUCTOS_SCTR");
            try
            {
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ProductosVM.C_TABLE suggest = new ProductosVM.C_TABLE();
                        suggest.NPRODUCT = reader["NPRODUCT"] == DBNull.Value ? 0 : int.Parse(reader["NPRODUCT"].ToString());
                        suggest.SDESCRIPT = reader["SDESCRIPT"] == DBNull.Value ? string.Empty : reader["SDESCRIPT"].ToString();
                        entities.lista.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarProductos", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<ProductosVM>(entities);
        }
        //
        public Task<EndososVM> ListarEndosos()
        {
            var parameters = new List<OracleParameter>();
            EndososVM entities = new EndososVM();
            entities.lista = new List<EndososVM.C_TABLE>();
            var procedure = string.Format("{0}.{1}", this.Package, "SPS_LIST_ENDOSOS_SCTR");
            try
            {
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        EndososVM.C_TABLE suggest = new EndososVM.C_TABLE();
                        suggest.NTYPE_MOVEMENT = reader["NTYPE_MOVEMENT"] == DBNull.Value ? 0 : int.Parse(reader["NTYPE_MOVEMENT"].ToString());
                        suggest.SDESCRIPT = reader["SDESCRIPT"] == DBNull.Value ? string.Empty : reader["SDESCRIPT"].ToString();
                        entities.lista.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarEndosos", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<EndososVM>(entities);
        }
        //
        public Task<CabeceraVM> ListarCabeceras(CabeceraBM data)
        {
            var parameters = new List<OracleParameter>();
            CabeceraVM entities = new CabeceraVM();
            entities.lista = new List<CabeceraVM.C_TABLE>();
            var procedure = string.Format("{0}.{1}", this.Package, "SPS_LIST_CABECERA_SCTR");
            string fechaInicio = data.P_DSTARTDATE.ToString("yyyy-MM-dd");
            string fechaFin = data.P_DEXPIRDAT.ToString("yyyy-MM-dd");
            try
            {
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int64, data.P_NPRODUCT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_STYPE_ENDOSO", OracleDbType.Int64, data.P_STYPE_ENDOSO, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DSTARTDATE", OracleDbType.Date, Convert.ToDateTime(fechaInicio), ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, Convert.ToDateTime(fechaFin), ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int64, data.P_NUSERCODE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        CabeceraVM.C_TABLE suggest = new CabeceraVM.C_TABLE();
                        suggest.PROD = reader["PROD"] == DBNull.Value ? string.Empty : reader["PROD"].ToString();
                        suggest.ENDO = reader["ENDO"] == DBNull.Value ? string.Empty : reader["ENDO"].ToString();
                        suggest.INI_VIG = reader["INI_VIG"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["INI_VIG"].ToString());
                        suggest.FIN_VIG = reader["FIN_VIG"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["FIN_VIG"].ToString());
                        suggest.CANT_ASEG = reader["CANT_ASEG"] == DBNull.Value ? 0 : int.Parse(reader["CANT_ASEG"].ToString());
                        suggest.ASEG_REG = reader["ASEG_REG"] == DBNull.Value ? 0 : int.Parse(reader["ASEG_REG"].ToString());
                        suggest.ASEG_NOREG = reader["ASEG_NOREG"] == DBNull.Value ? 0 : int.Parse(reader["ASEG_NOREG"].ToString());
                        suggest.ESTADO = reader["ESTADO"] == DBNull.Value ? 0 : int.Parse(reader["ESTADO"].ToString());
                        suggest.NID_CAB = reader["NID_CAB"] == DBNull.Value ? 0 : int.Parse(reader["NID_CAB"].ToString());
                        entities.lista.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarCabeceras", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<CabeceraVM>(entities);
        }
        //
        public List<TramasHistoricasCabeceraVM> InsertReportStatus(TramasHistoricasCabeceraBM data)
        {
            var entities = new List<TramasHistoricasCabeceraVM>();
            var parameters = new List<OracleParameter>();
            TramasHistoricasCabeceraVM entity = new TramasHistoricasCabeceraVM();
            string fecInicio = data.P_DSTARTDATE.ToString("dd/MM/yyyy");
            string fecFin = data.P_DEXPIRDAT.ToString("dd/MM/yyyy");
            var procedure = string.Format("{0}.{1}", this.Package, "SPS_INS_RPTE_ASEG_HIST_SCTR_CAB");
            try
            {
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int64, data.P_NPRODUCT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_STYPE_ENDOSO", OracleDbType.Int64, data.P_STYPE_ENDOSO, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DSTARTDATE", OracleDbType.NVarchar2, fecInicio, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.NVarchar2, fecFin, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SCLINUMDOCU", OracleDbType.NVarchar2, data.P_SCLINUMDOCU, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPOLICY", OracleDbType.NVarchar2, data.P_NPOLICY, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NRECEIPT", OracleDbType.NVarchar2, data.P_NRECEIPT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SCOMPROBANTE", OracleDbType.NVarchar2, data.P_SCOMPROBANTE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int64, data.P_NUSERCODE, ParameterDirection.Input));

                OracleParameter P_NID_CAB = new OracleParameter("P_NID_CAB", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_NERROR = new OracleParameter("P_NERROR", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMENSAJE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NID_CAB);
                parameters.Add(P_NERROR);
                parameters.Add(P_SMENSAJE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);
                entity.P_NID_CAB = Convert.ToInt32(P_NID_CAB.Value.ToString());
                entity.P_NERROR = Convert.ToInt32(P_NERROR.Value.ToString());
                entity.P_SMENSAJE = P_SMENSAJE.Value.ToString();
                entities.Add(entity);
            }
            catch (Exception ex)
            {
                LogControl.save("InsertReportStatus", ex.ToString(), "3");
                throw;
            }
            return entities;
        }
    }
}