using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using WSPlataforma.Entities.DocQuotationModel.BindingModel;
using WSPlataforma.Entities.DocQuotationModel.ViewModel;
using WSPlataforma.Entities.PrintPolicyModel;
using WSPlataforma.Entities.PrintPolicyModel.ViewModel;
using WSPlataforma.Util;
using System.Linq;
using WSPlataforma.Util.PrintPolicyUtility;

namespace WSPlataforma.DA.PrintPolicyPDF.Quotation
{
    public class DocQuotationDA : ConnectionBase
    {
        public ResponsePrintVM QuotationPDF(GenerateQuotationBM generateQuotationBM)
        {
            var response = new ResponsePrintVM();
            try
            {
                List<PrintPathsVM> printPathsVM = new PrintPolicyDA().GetPathsPrint(generateQuotationBM.NCOD_CONDICIONADO);
                //generateQuotationBM.NSTATE_DOC = PrintEnum.State.SIN_INICIAR;
                generateQuotationBM.NSTATE_DOC = PrintEnum.State.EN_PROCESO;
                UpdateStateQuotation(generateQuotationBM);
                response = new GenerateDocumentsDA().QutationPlanBasicPDF(generateQuotationBM, printPathsVM[0]);
                //var path = response.PATH_PDF;

                //if (response.NCODE == 0)
                //{
                //}
                //else
                //{
                //response.NCODE = 1;
                generateQuotationBM.SLOGERROR = response.SMESSAGE;
                generateQuotationBM.NSTATE_DOC = response.NCODE == 0 ? PrintEnum.State.CORRECTO : PrintEnum.State.ERROR;
                //generateQuotationBM.NSTATE_DOC = PrintEnum.State.ERROR;
                //UpdateStateQuotation(generateQuotationBM);
                //}

                UpdateStateQuotation(generateQuotationBM);

                //response.PATH_PDF = path;
                //return response;
            }
            catch (Exception ex)
            {
                response.NCODE = 1;
                generateQuotationBM.SLOGERROR = "QuotationPDF: " + ex.Message + " - " + "Nro Cotizacion: " + generateQuotationBM.NID_COTIZACION;
                generateQuotationBM.NSTATE_DOC = PrintEnum.State.ERROR;
                UpdateStateQuotation(generateQuotationBM);
                //ELog.save(this, ex);
                //return response;
            }

            return response;
        }

        public ResponsePrintVM UpdateStateQuotation(GenerateQuotationBM generateQuotationBM)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponsePrintVM response = new ResponsePrintVM();
            string storedProcedureName = ProcedureName.pkg_CargaMasivaPD + ".UPD_STATE_QUOTATION";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Long, generateQuotationBM.NID_COTIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NSTATE_DOC", OracleDbType.Int32, generateQuotationBM.NSTATE_DOC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLOGERROR", OracleDbType.Varchar2, generateQuotationBM.SLOGERROR, ParameterDirection.Input));
                //OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, response.NCODE, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, response.SMESSAGE, ParameterDirection.Output);

                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;
                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);

                this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                response.NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                response.SMESSAGE = P_SMESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                LogControl.save("UpdateStateQuotation", ex.ToString(), "3");
            }

            return response;
        }

        public string GetStateProcess(long NID_COTIZACION)
        {
            var sPackageName = ProcedureName.pkg_CargaMasivaPD + ".VAL_DOC_QUOTATION_STATE";
            string result = "";

            List<OracleParameter> parameters = new List<OracleParameter>();
            try
            {
                parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Long, NID_COTIZACION, ParameterDirection.Input));

                OracleParameter P_SFLAG = new OracleParameter("P_NSATE_DOC", OracleDbType.Int32, result, ParameterDirection.Output);

                P_SFLAG.Size = 200;

                parameters.Add(P_SFLAG);
                this.ExecuteByStoredProcedureVT(sPackageName, parameters);
                result = P_SFLAG.Value.ToString();
            }
            catch (Exception ex)
            {
                LogControl.save("GetStateProcess", ex.ToString(), "3");
            }
            return result;
        }

        public List<JobQuotationVM> GetJobsQuotation()
        {
            var sPackageJobs = ProcedureName.pkg_CargaMasivaPD + ".REA_JOB_QUOTATION";

            List<OracleParameter> parameter = new List<OracleParameter>();
            List<JobQuotationVM> response = new List<JobQuotationVM>();

            try
            {
                parameter.Add(new OracleParameter("C_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageJobs, "C_CURSOR", parameter))
                {
                    response = dr.ReadRowsList<JobQuotationVM>();
                    ELog.CloseConnection(dr);
                }
                return response;
            }
            catch (Exception ex)
            {
                LogControl.save("GetJobsQuotation", ex.ToString(), "3");
                return response = null;
            }
        }

        public List<ModuleQuotationVM> GetModulesQuotation(FormatQuotationBM formatQuotationBM)
        {
            var sPackageJobs = ProcedureName.pkg_CargaMasivaPD + ".REA_MODULE_QUOTATION";

            List<OracleParameter> parameter = new List<OracleParameter>();
            List<ModuleQuotationVM> response = new List<ModuleQuotationVM>();

            try
            {
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, formatQuotationBM.NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Long, formatQuotationBM.NID_COTIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("C_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));
                /*OracleParameter C_CURSOR = new OracleParameter("C_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_CURSOR);*/

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageJobs, "C_CURSOR", parameter))
                {
                    response = dr.ReadRowsList<ModuleQuotationVM>();
                    ELog.CloseConnection(dr);
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetModulesQuotation", ex.ToString(), "3");
                //ELog.save(this, ex);
            }

            return response;
        }

        public List<FormatQuotationVM> GetFormatsQuotation(FormatQuotationBM formatQuotationBM)
        {
            var sPackageJobs = ProcedureName.pkg_CargaMasivaPD + ".REA_FORMATOS_QUOTATION";

            List<OracleParameter> parameter = new List<OracleParameter>();
            List<FormatQuotationVM> response = new List<FormatQuotationVM>();

            try
            {
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, formatQuotationBM.NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Long, formatQuotationBM.NID_COTIZACION, ParameterDirection.Input));
                //parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, formatQuotationBM.NMODULEC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("C_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));
                /*OracleParameter C_CURSOR = new OracleParameter("C_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_CURSOR);*/

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageJobs, "C_CURSOR", parameter))
                {
                    response = dr.ReadRowsList<FormatQuotationVM>();
                    ELog.CloseConnection(dr);
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetFormatsQuotation", ex.ToString(), "3");
                //ELog.save(this, ex);
            }

            return response;
        }

        public List<ProcedureQuotationVM> GetProceduresQuotationPrint(ProcedureQuotationBM procedureQuotationBM)
        {
            var sPackageJobs = ProcedureName.pkg_CargaMasivaPD + ".REA_PROCEDURES_QUOTATION";

            List<OracleParameter> parameter = new List<OracleParameter>();
            List<ProcedureQuotationVM> response = new List<ProcedureQuotationVM>();

            try
            {
                parameter.Add(new OracleParameter("P_NCOD_CONDICIONADO", OracleDbType.Double, procedureQuotationBM.NCOD_CONDICIONADO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("C_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));
                /*OracleParameter C_CURSOR = new OracleParameter("C_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_CURSOR);*/

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageJobs, "C_CURSOR", parameter))
                {
                    response = dr.ReadRowsList<ProcedureQuotationVM>();
                    ELog.CloseConnection(dr);
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetProceduresQuotationPrint", ex.ToString(), "3");
                //ELog.save(this, ex);
            }

            return response;
        }
    }
}