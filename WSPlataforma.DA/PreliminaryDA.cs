using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using Oracle.DataAccess.Client;
using WSPlataforma.Entities.PreliminaryModel.BindingModel;
using WSPlataforma.Entities.PreliminaryModel.ViewModel;
using WSPlataforma.Entities.ReportModel.BindingModel;
using WSPlataforma.Entities.ReportModel.ViewModel;
using WSPlataforma.Util;
using SpreadsheetLight;
using System.IO;

namespace WSPlataforma.DA
{
    public class PreliminaryDA : ConnectionBase
    {
        #region Filters

        public List<SuggestVM> GetBranches()
        {
            var parameters = new List<OracleParameter>();
            var suggests = new List<SuggestVM>();
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_PreliminarSCTR, "SPS_GET_BRANCHES");

            try
            {
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var suggest = new SuggestVM();
                        suggest.Id = reader["NID"] == DBNull.Value ? 0 : int.Parse(reader["NID"].ToString());
                        suggest.Description = reader["SDESCRIPTION"] == DBNull.Value ? string.Empty : reader["SDESCRIPTION"].ToString();

                        suggests.Add(suggest);
                    }
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("GetBranches", ex.ToString(), "3");
                throw;
            }

            return suggests;
        }

        public List<SuggestVM> GetTypeProcess()
        {
            var parameters = new List<OracleParameter>();
            var suggests = new List<SuggestVM>();
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_PreliminarSCTR, "SPS_GET_PROCESO");

            try
            {
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var suggest = new SuggestVM();
                        suggest.Id = reader["NVALUE"] == DBNull.Value ? 0 : int.Parse(reader["NVALUE"].ToString());
                        suggest.Description = reader["SDESCRIPTION"] == DBNull.Value ? string.Empty : reader["SDESCRIPTION"].ToString();

                        suggests.Add(suggest);
                    }
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("GetTypeProcess", ex.ToString(), "3");
                throw;
            }

            return suggests;
        }

        public List<SuggestVMDate> GetBranchPeriod(PreliminaryFilter data)
        {
            var parameters = new List<OracleParameter>();
            var suggests = new List<SuggestVMDate>();
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_PreliminarSCTR, "SPS_GET_PERIODO");

            try
            {
                parameters.Add(new OracleParameter("P_NBRANCH_ID", OracleDbType.Int32, data.BranchId, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NREPORT_TYPE", OracleDbType.Int32, data.ReportId, ParameterDirection.Input));
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var suggest = new SuggestVMDate();
                        suggest.IdPeriodo = reader["L_IDPERIODO"] == DBNull.Value ? 0 : int.Parse(reader["L_IDPERIODO"].ToString());
                        suggest.fchIniPeriodo = reader["L_PERFCHINI"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["L_PERFCHINI"].ToString());
                        suggest.fchFinPeriodo = reader["L_PERFCHFIN"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["L_PERFCHFIN"].ToString());
                        suggest.periodoMes = reader["L_PERMES"] == DBNull.Value ? 0 : int.Parse(reader["L_PERMES"].ToString());
                        suggest.periodoAnio = reader["L_PERANIO"] == DBNull.Value ? 0 : int.Parse(reader["L_PERANIO"].ToString());

                        suggests.Add(suggest);
                    }
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("GetBranchPeriod", ex.ToString(), "3");
                throw;
            }

            return suggests;
        }

        public List<SuggestVM> GetReports()
        {
            var parameters = new List<OracleParameter>();
            var suggests = new List<SuggestVM>();
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_PreliminarSCTR, "SPS_GET_PRELIMINARYTYPE");

            try
            {
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var suggest = new SuggestVM();
                        suggest.Id = reader["NID"] == DBNull.Value ? 0 : int.Parse(reader["NID"].ToString());
                        suggest.Description = reader["SDESCRIPTION"] == DBNull.Value ? string.Empty : reader["SDESCRIPTION"].ToString();

                        suggests.Add(suggest);
                    }
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("GetReports", ex.ToString(), "3");
                throw;
            }

            return suggests;
        }

        #endregion

        #region Validations

        //genera los datos de la cabecera del proceso generaro (prelominar / definitivo)
        public int GetValidateHeadId(PreliminaryFilter data)
        {
            try
            {
                //INSERTA REGISTRO EN PRELIMINAR_CABECERA QUE REPRESENTA POR PRIMAS, COBRANZA O COMISION Y DEVUELVE ID CABECERA
                var parameters = new List<OracleParameter>();
                var procedure = ProcedureName.sp_InsertValidacion;
                string fechaInicio = data.StartDate.ToString("dd/MM/yyyy");
                string fechaFin = data.EndDate.ToString("dd/MM/yyyy");
                int prod;
                if (data.ProductN == "" || data.ProductN == null)
                {
                    prod = 0;
                }
                else
                {
                    prod = Int32.Parse(data.ProductN);
                }
                parameters.Add(new OracleParameter("P_NBRANCH_ID", OracleDbType.Int32, data.BranchId, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, prod, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_USERNAME", OracleDbType.Varchar2, data.UserName, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DDAT_PROCESS_INI", OracleDbType.Date, fechaInicio, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DDAT_PROCESS_FIN", OracleDbType.Date, fechaFin, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NIDPERIODO", OracleDbType.Int32, data.IdPeriodo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPERIODOMES", OracleDbType.Int32, data.PeriodoMes, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPERIODONAIO", OracleDbType.Int32, data.PeriodoAnio, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NTYPE_PROCESS", OracleDbType.Int32, data.TypeProcess, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NTYPE_REPORT", OracleDbType.Int32, data.ReportId, ParameterDirection.Input));

                OracleParameter validationId = new OracleParameter("P_VALIDATION", OracleDbType.Int32, ParameterDirection.Output);
                parameters.Add(validationId);

                this.ExecuteByStoredProcedureVT(string.Format("{0}.{1}", ProcedureName.pkg_PreliminarSCTR, procedure), parameters);

                return int.Parse(validationId.Value.ToString());
            }
            catch (Exception ex)
            {
                LogControl.save("GetValidateHeadId", ex.ToString(), "3");
                throw;
            }
        }

        public void UpdateStateValidation(int validationId)
        {
            try
            {
                var procedure = string.Format("{0}.{1}", ProcedureName.pkg_PreliminarSCTR, "SPS_UPDVALIDATION_ERROR");
                var parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("P_VALIDATION_ID", OracleDbType.Int32, validationId, ParameterDirection.Input));

                this.ExecuteByStoredProcedureVT(procedure, parameters);

            }
            catch (Exception ex)
            {
                LogControl.save("UpdateStateValidation", ex.ToString(), "3");
                throw;
            }
        }
        //obtiene los datos de la cabecera del proceso generaro (prelominar / definitivo)
        public List<PreliminaryValidationVM> GetValidationById(int validationId)
        {
            var validations = new List<PreliminaryValidationVM>();

            try
            {
                var parameters = new List<OracleParameter>();
                var procedure = ProcedureName.sp_LeerValidacion;

                parameters.Add(new OracleParameter("P_NVALIDATION", OracleDbType.Int32, validationId, ParameterDirection.Input));
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(string.Format("{0}.{1}", ProcedureName.pkg_PreliminarSCTR, procedure), parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var validation = new PreliminaryValidationVM();

                        validation.PreliminaryId = reader["NPRELCAB_ID"] == DBNull.Value ? 0 : int.Parse(reader["NPRELCAB_ID"].ToString());
                        validation.BranchId = reader["NBRANCH_ID"] == DBNull.Value ? 0 : int.Parse(reader["NBRANCH_ID"].ToString());
                        validation.BranchName = reader["SBRANCH_NAME"] == DBNull.Value ? string.Empty : reader["SBRANCH_NAME"].ToString();
                        validation.StateId = reader["NSTATE_ID"] == DBNull.Value ? 0 : int.Parse(reader["NSTATE_ID"].ToString());
                        validation.StateName = reader["SSTATE_NAME"] == DBNull.Value ? string.Empty : reader["SSTATE_NAME"].ToString();
                        validation.ReportTypeId = reader["NREPORT_TYPE_ID"] == DBNull.Value ? 0 : int.Parse(reader["NREPORT_TYPE_ID"].ToString());
                        validation.ReportTypeName = reader["SREPORT_TYPE"] == DBNull.Value ? string.Empty : reader["SREPORT_TYPE"].ToString();
                        validation.UserName = reader["SUSER_NAME"] == DBNull.Value ? string.Empty : reader["SUSER_NAME"].ToString();
                        validation.StartDate = reader["DSTART_DATE"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["DSTART_DATE"].ToString());
                        validation.EndDate = reader["DEND_DATE"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["DEND_DATE"].ToString());
                        validation.Type_Process = reader["NTYPE_PROCESS"] == DBNull.Value ? 0 : int.Parse(reader["NTYPE_PROCESS"].ToString());

                        validations.Add(validation);

                    }
                }

                reader.Close();

            }
            catch (Exception ex)
            {
                LogControl.save("GetValidationById", ex.ToString(), "3");
                throw;
            }
            return validations;
        }

        //Procesar - Insertar MONITOREO
        public ResponseControl InsertProcessPremiumReport(ReportProcess data)
        {
            DateTime fechaInicio = Convert.ToDateTime(data.fecInicio);
            DateTime fechaFin = Convert.ToDateTime(data.fecFin);
            string FINICIO = fechaInicio.ToString("dd/MM/yyyy");
            string FFIN = fechaFin.ToString("dd/MM/yyyy");

            int prod;
            if (data.productN == "" || data.productN == null)
            {
                prod = 0;
            }
            else
            {
                prod = Int32.Parse(data.productN);
            }
            ResponseControl generic = new ResponseControl(Response.Ok);

            using (OracleConnection cn = new OracleConnection(System.Configuration.ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = cn;
                    cmd.CommandText = string.Format("{0}.{1}", ProcedureName.pkg_PreliminarSCTR, "SPS_INSERT_MONITORCAB");
                    cmd.CommandType = CommandType.StoredProcedure;
                    try
                    {
                        cmd.Parameters.Add(new OracleParameter("P_IDMONITOR", OracleDbType.Varchar2, data.idProceso, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_SUSERNAME", OracleDbType.Varchar2, data.desUsuario.ToUpper(), ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_DINIREP", OracleDbType.Date, FINICIO, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_DFINREP", OracleDbType.Date, FFIN, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.codRamo, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_STIPO", OracleDbType.Varchar2, data.codTipo, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NPRELCAB_ID", OracleDbType.Varchar2, data.codProducto, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, prod, ParameterDirection.Input));
                        //Ncode controla los errores
                        var NCode = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);
                        var SMessage = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                        NCode.Size = 4000;
                        SMessage.Size = 4000;
                        cmd.Parameters.Add(NCode);
                        cmd.Parameters.Add(SMessage);
                        cn.Open();

                        cmd.ExecuteNonQuery();
                        generic.Data = data.idProceso;
                        var PCode = Convert.ToInt32(NCode.Value.ToString());
                        var PMessage = (SMessage.Value.ToString());

                        if (PCode == 1)
                        {
                            generic.Message = SMessage.Value.ToString();
                            generic.response = Response.Fail;
                        }
                        cmd.Dispose();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(generic.Message.ToString());
                    }
                }
            }
            return generic;
        }
        //obtiene registro de monitoreo (status reporte preliminar/definitivo)
        public List<SuggestVMMonitor> GetMonitorProcess(PreliminaryFilter data)
        {
            var parameters = new List<OracleParameter>();
            var suggests = new List<SuggestVMMonitor>();
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_PreliminarSCTR, "SPS_GET_MONITORPROCESS");

            try
            {
                string fechaInicio = data.StartDate.ToString("dd/MM/yyyy");
                string fechaFin = data.EndDate.ToString("dd/MM/yyyy");

                int prod;
                if (data.ProductN == "" || data.ProductN == null)
                {
                    prod = 0;
                }
                else
                {
                    prod = Int32.Parse(data.ProductN);
                }
                parameters.Add(new OracleParameter("P_NBRANCH_ID", OracleDbType.Int32, data.BranchId, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DINIREP", OracleDbType.Date, Convert.ToDateTime(fechaInicio), ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DFINREP", OracleDbType.Date, Convert.ToDateTime(fechaFin), ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NREPORT_TYPE_ID", OracleDbType.Int32, data.ReportId, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, prod, ParameterDirection.Input));

                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var suggest = new SuggestVMMonitor();

                        suggest.IDMONITOR = reader["IDMONITOR"] == DBNull.Value ? string.Empty : reader["IDMONITOR"].ToString();
                        suggest.NPRELCAB_ID = reader["NPRELCAB_ID"] == DBNull.Value ? 0 : int.Parse(reader["NPRELCAB_ID"].ToString());
                        suggest.SBRANCH_NAME = reader["SBRANCH_NAME"] == DBNull.Value ? string.Empty : reader["SBRANCH_NAME"].ToString();
                        suggest.SREPORT_TYPE = reader["SREPORT_TYPE"] == DBNull.Value ? string.Empty : reader["SREPORT_TYPE"].ToString();
                        suggest.DSTART_DATE = reader["DINIREP"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["DINIREP"].ToString());
                        suggest.DEND_DATE = reader["DFINREP"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["DFINREP"].ToString());
                        suggest.SUSERNAME = reader["SUSERNAME"] == DBNull.Value ? string.Empty : reader["SUSERNAME"].ToString();
                        suggest.STIPO = reader["STIPO"] == DBNull.Value ? string.Empty : reader["STIPO"].ToString();
                        suggest.NSTATUSPROC = reader["NSTATUSPROC"] == DBNull.Value ? 0 : int.Parse(reader["NSTATUSPROC"].ToString());
                        suggest.DCOMPDATE_CO = reader["DCOMPDATE"] == DBNull.Value ? string.Empty : Convert.ToDateTime(reader["DCOMPDATE"]).ToString("dd/MM/yyyy HH:mm:ss");
                        suggest.DCOMPDATE = reader["DCOMPDATE"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["DCOMPDATE"].ToString());
                        suggests.Add(suggest);
                    }
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("GetMonitorProcess", ex.ToString(), "3");
                throw;
            }

            return suggests;
        }
        #endregion
    }
}
