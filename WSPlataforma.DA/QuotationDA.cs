using Newtonsoft.Json;
using Oracle.DataAccess.Client;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WSPlataforma.Entities.Cliente360.BindingModel;
using WSPlataforma.Entities.Cliente360.ViewModel;
using WSPlataforma.Entities.Graphql;
using WSPlataforma.Entities.HistoryCreditServiceModel.ViewModel;
using WSPlataforma.Entities.PolicyModel.BindingModel;
using WSPlataforma.Entities.QuotationModel.BindingModel;
using WSPlataforma.Entities.QuotationModel.BindingModel.QuotationModification;
using WSPlataforma.Entities.QuotationModel.ViewModel;
using WSPlataforma.Entities.TechnicalTariffModel.BindingModel;
using WSPlataforma.Entities.TransactModel;
using WSPlataforma.Util;
using WSPlataforma.Entities.EPSModel.ViewModel;
using WSPlataforma.Entities.ReportModel.BindingModel;
using WSPlataforma.Entities.PrintPolicyModel.ViewModel.Particular_conditions;
using System.Text.RegularExpressions;

namespace WSPlataforma.DA
{
    public class QuotationDA : ConnectionBase
    {
        WebServiceUtil WebServiceUtil = new WebServiceUtil();
        private SharedMethods sharedMethods = new SharedMethods();
        TransactDA transactDA = new TransactDA();

        public QuotationResponseVM ApproveQuotation(QuotationCabBM request, DbConnection connection, DbTransaction trx)
        {
            var sPackageName = ProcedureName.pkg_Cotizacion + ".UPD_COTIZACION";
            List<OracleParameter> parameter = new List<OracleParameter>();
            QuotationResponseVM result = new QuotationResponseVM();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, request.NumeroCotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_ESTADO", OracleDbType.Int32, request.P_ESTADO != null ? request.P_ESTADO : 2, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, request.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMOTIVO", OracleDbType.Int32, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOMMENT", OracleDbType.Varchar2, request.P_SCOMMENT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STRANSAC", OracleDbType.Varchar2, request.TrxCode, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DSTARTDATE", OracleDbType.Date, Convert.ToDateTime(request.P_DSTARTDATE), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, request.P_DEXPIRDAT != null ? Convert.ToDateTime(request.P_DEXPIRDAT) : Convert.ToDateTime(request.P_DSTARTDATE), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTIP_RENOV", OracleDbType.Int32, request.P_NTIP_RENOV == 0 ? null : request.P_NTIP_RENOV, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DSTARTDATE_ASE", OracleDbType.Date, request.P_DSTARTDATE_ASE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEXPIRDAT_ASE", OracleDbType.Date, request.P_DEXPIRDAT_ASE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPAYFREQ", OracleDbType.Int32, request.P_NPAYFREQ == 0 ? null : request.P_NPAYFREQ, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SRUTA", OracleDbType.Varchar2, request.P_SRUTA, ParameterDirection.Input));

                parameter.Add(new OracleParameter("P_NIDPLAN", OracleDbType.Int32, request.planId, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMODULEC_TARIFARIO", OracleDbType.Int32, request.P_NMODULEC_TARIFARIO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SDESCRIPT_TARIFARIO", OracleDbType.Varchar2, request.P_SDESCRIPT_TARIFARIO, ParameterDirection.Input));
                //OUTPUT
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, result.P_MESSAGE, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, result.P_COD_ERR, ParameterDirection.Output);

                P_MESSAGE.Size = 6000;

                parameter.Add(P_MESSAGE);
                parameter.Add(P_COD_ERR);

                // mejoras CIIU VL 
                parameter.Add(new OracleParameter("P_SCOD_ACTIVITY_TEC", OracleDbType.Varchar2, request.P_SCOD_ACTIVITY_TEC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOD_CIUU", OracleDbType.Varchar2, request.P_SCOD_CIUU, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NFLAG_UPD", OracleDbType.Int32, request.P_NFLAG_UPD, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_COMMISSION", OracleDbType.Int32, request.P_NTIP_NCOMISSION, ParameterDirection.Input)); // COMISIONES VL JTV 26042023
                parameter.Add(new OracleParameter("P_NSCOPE", OracleDbType.Int32, request.P_NSCOPE, ParameterDirection.Input)); //AVS - RENTAS
                parameter.Add(new OracleParameter("P_NTEMPORALITY", OracleDbType.Int32, request.P_NTEMPORALITY, ParameterDirection.Input)); //AVS - RENTAS
                parameter.Add(new OracleParameter("P_NFACTURA_ANTICIPADA", OracleDbType.Int32, request.P_NFACTURA_ANTICIPADA, ParameterDirection.Input)); // JDD - NO RENTAS

                this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, connection, trx);
                result.P_MESSAGE = P_MESSAGE.Value.ToString();
                result.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
            }
            catch (Exception ex)
            {
                result.P_MESSAGE = ex.ToString();
                result.P_COD_ERR = 1;
                LogControl.save("ApproveQuotation", ex.ToString(), "3");
            }

            return result;
        }

        public QuotationResponseVM ApproveQuotationEndoso(QuotationCabBM request, DbConnection connection, DbTransaction trx)
        {
            var sPackageName = ProcedureName.pkg_Cotizacion + ".UPD_COTIZACION_ENDOSO_PD";
            List<OracleParameter> parameter = new List<OracleParameter>();
            QuotationResponseVM result = new QuotationResponseVM();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, request.NumeroCotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, request.P_NPRODUCT, ParameterDirection.Input)); // nuevo 
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, request.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Date, Convert.ToDateTime(request.P_DSTARTDATE_ASE), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTIP_RENOV", OracleDbType.Int32, request.P_NTIP_RENOV == 0 ? null : request.P_NTIP_RENOV, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DSTARTDATE", OracleDbType.Date, Convert.ToDateTime(request.P_DSTARTDATE), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, request.P_DEXPIRDAT != null ? Convert.ToDateTime(request.P_DEXPIRDAT) : Convert.ToDateTime(request.P_DSTARTDATE), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPAYFREQ", OracleDbType.Int32, request.P_NPAYFREQ == 0 ? null : request.P_NPAYFREQ, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DSTARTDATE_ASE", OracleDbType.Date, Convert.ToDateTime(request.P_DSTARTDATE_ASE), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEXPIRDAT_ASE", OracleDbType.Date, Convert.ToDateTime(request.P_DEXPIRDAT_ASE), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_ESTADO", OracleDbType.Int32, request.P_ESTADO != null ? request.P_ESTADO : 2, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMOTIVO", OracleDbType.Int32, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOMMENT", OracleDbType.Varchar2, request.P_SCOMMENT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SRUTA", OracleDbType.Varchar2, request.P_SRUTA, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, result.P_MESSAGE, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, result.P_COD_ERR, ParameterDirection.Output);

                P_MESSAGE.Size = 6000;

                parameter.Add(P_MESSAGE);
                parameter.Add(P_COD_ERR);

                this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, connection, trx);
                result.P_MESSAGE = P_MESSAGE.Value.ToString();
                result.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
            }
            catch (Exception ex)
            {
                result.P_COD_ERR = 1;
                result.P_MESSAGE = ex.ToString();
                LogControl.save("ApproveQuotationEndoso", ex.ToString(), "3");
            }

            return result;
        }

        public GenericResponseVM NullQuotationCIP(QuotationCabBM request)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            GenericResponseVM resultPackage = new GenericResponseVM();
            List<QuotationResponseVM> list = new List<QuotationResponseVM>();

            string storedProcedureName = ProcedureName.pkg_pv_otros_procesos + ".SP_ANULA_CIP";

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, request.NumeroCotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SOPERATIONNUMBER", OracleDbType.Int32, request.NumeroOperacion, ParameterDirection.Input));
                //OUTPUT
                var P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, ParameterDirection.Output);
                parameter.Add(P_COD_ERR);
                var P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameter.Add(P_MESSAGE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                resultPackage.ErrorCode = Convert.ToInt32(P_COD_ERR.Value.ToString()); //int.Parse(P_COD_ERR.ToString());
                resultPackage.MessageError = P_MESSAGE.Value.ToString();

                resultPackage.StatusCode = 0;
                resultPackage.GenericResponse = list;
                ELog.CloseConnection(odr);

                return resultPackage;
            }

            catch (Exception ex)
            {
                resultPackage.MessageError = ex.ToString();
                LogControl.save("GetQuotationList", ex.ToString(), "3");
                return resultPackage;
            }
        }

        public SalidaInfoAuthBaseVM GetInfoQuotationAuth(InfoAuthBM infoAuth)
        {
            SalidaInfoAuthBaseVM response = new SalidaInfoAuthBaseVM();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.sp_DetalleMontos;

            try
            {
                List<CategoryRateBM> listCategoryRate = new List<CategoryRateBM>();
                foreach (var item1 in infoAuth.AuthorizedList)
                {
                    listCategoryRate.Add(new CategoryRateBM() { NMODULEC = item1.CategoryCode, SDESCRIPT = item1.CategoryAuthorized, P_NTASA_AUT = item1.RateAuthorized });
                }
                foreach (var item2 in infoAuth.ProposalList)
                {
                    listCategoryRate.Add(new CategoryRateBM() { NMODULEC = item2.CategoryCode, SDESCRIPT = item2.CategoryProposal, P_NTASA_PR = item2.RateProposal });
                }
                string json = JsonConvert.SerializeObject(listCategoryRate, Formatting.None).ToString();
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, infoAuth.QuotationNumber, ParameterDirection.Input));
                /*parameter.Add(new OracleParameter("P_NTASA_AUT_EMP", OracleDbType.Double, infoAuth.RateAuthorizedEmp, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTASA_AUT_OBR", OracleDbType.Double, infoAuth.RateAuthorizedObr, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTASA_AUT_OAR", OracleDbType.Double, infoAuth.RateAuthorizedOar, ParameterDirection.Input));*/
                parameter.Add(new OracleParameter("P_NCOMISION_AUT", OracleDbType.Double, infoAuth.AuthorizedCommission, ParameterDirection.Input));
                /*parameter.Add(new OracleParameter("P_NTASA_PR_EMP", OracleDbType.Double, infoAuth.RateProposedEmp, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTASA_PR_OBR", OracleDbType.Double, infoAuth.RateProposedObr, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTASA_PR_OAR", OracleDbType.Double, infoAuth.RateProposedOar, ParameterDirection.Input));*/
                parameter.Add(new OracleParameter("P_NCOMISION_PR", OracleDbType.Double, infoAuth.ProposedCommission, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_JTASAS", OracleDbType.Varchar2, json, ParameterDirection.Input));

                //OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, ParameterDirection.Output);
                string[] arrayCursor = { "C_CATEGORIA", "C_COMISSION_PR_AUT", "C_CALCULADA", "C_CALCULADA_DET", "C_PROPUESTA", "C_PROPUESTA_DET", "C_AUTORIZADA", "C_AUTORIZADA_DET" };

                OracleParameter C_CATEGORIA = new OracleParameter(arrayCursor[0], OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter C_COMISSION_PR_AUT = new OracleParameter(arrayCursor[1], OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter C_CALCULADA = new OracleParameter(arrayCursor[2], OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter C_CALCULADA_DET = new OracleParameter(arrayCursor[3], OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter C_PROPUESTA = new OracleParameter(arrayCursor[4], OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter C_PROPUESTA_DET = new OracleParameter(arrayCursor[5], OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter C_AUTORIZADA = new OracleParameter(arrayCursor[6], OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter C_AUTORIZADA_DET = new OracleParameter(arrayCursor[7], OracleDbType.RefCursor, ParameterDirection.Output);

                parameter.Add(C_CATEGORIA);
                parameter.Add(C_COMISSION_PR_AUT);
                parameter.Add(C_CALCULADA);
                parameter.Add(C_CALCULADA_DET);
                parameter.Add(C_PROPUESTA);
                parameter.Add(C_PROPUESTA_DET);
                parameter.Add(C_AUTORIZADA);
                parameter.Add(C_AUTORIZADA_DET);

                OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVTCursores(storeprocedure, arrayCursor, parameter);

                //if (dr.HasRows)
                //{
                while (dr.Read())
                {
                    CategoryQuotationVM item = new CategoryQuotationVM();
                    item.SCATEGORIA = dr["CATEGORIA"] == DBNull.Value ? "" : dr["CATEGORIA"].ToString();
                    item.SRANGO_EDAD = dr["SRANGO_EDAD"] == DBNull.Value ? "" : dr["SRANGO_EDAD"].ToString(); // GCAA 22112023
                    item.NCOUNT = dr["TOTAL_TRABAJADORES"] == DBNull.Value ? 0 : int.Parse(dr["TOTAL_TRABAJADORES"].ToString());
                    item.NTOTAL_PLANILLA = dr["MONTO_PLANILLA"] == DBNull.Value ? 0.0 : double.Parse(dr["MONTO_PLANILLA"].ToString());
                    response.CategoryList.Add(item);
                }
                dr.NextResult();

                while (dr.Read())
                {
                    CommisionQuotationVM item = new CommisionQuotationVM();
                    item.MinPremium = dr["PRIMA_MINIMA_ANUAL"] == DBNull.Value ? 0 : double.Parse(dr["PRIMA_MINIMA_ANUAL"].ToString());
                    item.Commission = dr["COMISION"] == DBNull.Value ? "" : dr["COMISION"].ToString();
                    item.Plan = dr["PLAN"] == DBNull.Value ? "" : dr["PLAN"].ToString();
                    item.CommissionProposed = dr["COMISION_PROPUESTA"] == DBNull.Value ? "" : dr["COMISION_PROPUESTA"].ToString();
                    item.CommissionAuthorized = dr["COMISION_AUTORIZADA"] == DBNull.Value ? "" : dr["COMISION_AUTORIZADA"].ToString();
                    response.CommissionList.Add(item);
                }

                dr.NextResult();

                while (dr.Read()) // PRIMA MENSUAL
                {
                    CalculateQuotationVM item = new CalculateQuotationVM();
                    item.CategoryCalculate = dr["CATEGORIA"] == DBNull.Value ? "" : dr["CATEGORIA"].ToString();
                    item.SRANGO_EDAD = dr["SRANGO_EDAD"] == DBNull.Value ? "" : dr["SRANGO_EDAD"].ToString();// GCAA 22112023
                    item.RateCalculate = dr["NTASA_CALCULADA"] == DBNull.Value ? 0.0 : double.Parse(dr["NTASA_CALCULADA"].ToString());
                    item.PremiumCalculate = dr["NPREMIUM_CALCULADA"] == DBNull.Value ? 0.0 : double.Parse(dr["NPREMIUM_CALCULADA"].ToString());
                    response.CalculateList.Add(item);
                }

                dr.NextResult();

                while (dr.Read())
                {
                    CalculateDetaillQuotationVM item = new CalculateDetaillQuotationVM();
                    item.DescriptionCalculate = dr["SDESCRIPCION"] == DBNull.Value ? "" : dr["SDESCRIPCION"].ToString();
                    item.AmountCalculate = dr["NMONTO_CALCULADA"] == DBNull.Value ? 0.0 : double.Parse(dr["NMONTO_CALCULADA"].ToString());
                    response.CalculateDetailList.Add(item);
                }

                dr.NextResult();

                while (dr.Read()) // PRIMA PROPUESTA
                {
                    ProposalQuotationVM item = new ProposalQuotationVM();
                    item.CategoryProposal = dr["CATEGORIA"] == DBNull.Value ? "" : dr["CATEGORIA"].ToString();
                    item.SRANGO_EDAD = dr["SRANGO_EDAD"] == DBNull.Value ? "" : dr["SRANGO_EDAD"].ToString();// GCAA 22112023
                    item.RateProposal = dr["NTASA_PROPUESTA"] == DBNull.Value ? 0.0 : double.Parse(dr["NTASA_PROPUESTA"].ToString());
                    item.PremiumProposal = dr["NPREMIUM_PROPUESTA"] == DBNull.Value ? 0.0 : double.Parse(dr["NPREMIUM_PROPUESTA"].ToString());
                    item.CategoryCode = dr["NMODULEC"] == DBNull.Value ? 0 : int.Parse(dr["NMODULEC"].ToString());
                    response.ProposalList.Add(item);
                }

                dr.NextResult();

                while (dr.Read())
                {
                    ProposalDetailQuotationVM item = new ProposalDetailQuotationVM();
                    item.DescriptionProposal = dr["SDESCRIPCION"] == DBNull.Value ? "" : dr["SDESCRIPCION"].ToString();
                    item.AmountProposal = dr["NMONTO_CALCULADA"] == DBNull.Value ? 0.0 : double.Parse(dr["NMONTO_CALCULADA"].ToString());
                    response.ProposalDetailList.Add(item);
                }

                dr.NextResult();

                while (dr.Read()) // PRIMA AUTORIZADA
                {
                    AuthorizedQuotationVM item = new AuthorizedQuotationVM();
                    item.CategoryAuthorized = dr["CATEGORIA"] == DBNull.Value ? "" : dr["CATEGORIA"].ToString();
                    item.SRANGO_EDAD = dr["SRANGO_EDAD"] == DBNull.Value ? "" : dr["SRANGO_EDAD"].ToString();
                    item.RateAuthorized = dr["NTASA_AUTORIZADA"] == DBNull.Value ? 0.0 : double.Parse(dr["NTASA_AUTORIZADA"].ToString());
                    item.PremiumAuthorized = dr["NPREMIUM_AUTORIZADA"] == DBNull.Value ? 0.0 : double.Parse(dr["NPREMIUM_AUTORIZADA"].ToString());
                    item.CategoryCode = dr["NMODULEC"] == DBNull.Value ? 0 : int.Parse(dr["NMODULEC"].ToString());
                    response.AuthorizedList.Add(item);
                }

                dr.NextResult();

                while (dr.Read())
                {
                    AuthorizedDetailQuotationVM item = new AuthorizedDetailQuotationVM();
                    item.DescriptionAuthorized = dr["SDESCRIPCION"] == DBNull.Value ? "" : dr["SDESCRIPCION"].ToString();
                    item.AmountAuthorized = dr["NMONTO_CALCULADA"] == DBNull.Value ? 0.0 : double.Parse(dr["NMONTO_CALCULADA"].ToString());
                    response.AuthorizedDetailList.Add(item);
                }
                //}

                ELog.CloseConnection(dr);
            }
            catch (Exception ex)
            {
                LogControl.save("GetInfoQuotationAuth", ex.ToString(), "3");
            }

            return response;
        }

        public int validarConsultaAsegurado(ConsultaCBM item, ConsultaCBM request)
        {
            int flagConsulta = 0;

            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        OracleDataReader dr;

                        cmd.Connection = cn;
                        cmd.CommandText = ProcedureName.sp_LeerClienteReniec;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new OracleParameter("P_NTYPE_DOCUMENT", OracleDbType.Int32, request.P_NIDDOC_TYPE, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, item.P_SIDDOC, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_SAPEPAT", OracleDbType.Varchar2, item.P_SLASTNAME, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_SAPEMAT", OracleDbType.Varchar2, item.P_SLASTNAME2, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_SNAMES", OracleDbType.Varchar2, item.P_SFIRSTNAME, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NSEX", OracleDbType.Varchar2, item.P_SSEXCLIEN, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_DBIRTHDAT", OracleDbType.Date, Convert.ToDateTime(item.P_DBIRTHDAT), ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_SMODULEC", OracleDbType.Varchar2, item.P_SMODULEC, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NRENTAMOUNT", OracleDbType.Varchar2, item.P_NRENTAMOUNT, ParameterDirection.Input));

                        var P_COD_ERR = new OracleParameter("P_NINDICADOR", OracleDbType.Int32, ParameterDirection.Output);

                        //P_COD_ERR.Size = 200;

                        cmd.Parameters.Add(P_COD_ERR);

                        cn.Open();
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        flagConsulta = Convert.ToInt32(P_COD_ERR.Value.ToString());

                        ELog.CloseConnection(dr);
                    }
                    catch (Exception ex)
                    {
                        flagConsulta = 0;
                        LogControl.save("validarConsultaAsegurado", "json: " + JsonConvert.SerializeObject(request) + " - json2: " + JsonConvert.SerializeObject(item), "2");
                        LogControl.save("validarConsultaAsegurado", ex.ToString(), "3");
                    }
                    finally
                    {
                        if (cn.State == ConnectionState.Open) cn.Close();
                    }
                }
            }


            //int flagConsulta = 0;
            //var sPackageName = ProcedureName.sp_LeerClienteReniec;
            //List<OracleParameter> parameter = new List<OracleParameter>();
            //try
            //{
            //    //INPUT
            //    parameter.Add(new OracleParameter("P_NTYPE_DOCUMENT", OracleDbType.Int32, request.P_NIDDOC_TYPE, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, item.P_SIDDOC, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_SAPEPAT", OracleDbType.Varchar2, item.P_SLASTNAME, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_SAPEMAT", OracleDbType.Varchar2, item.P_SLASTNAME2, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_SNAMES", OracleDbType.Varchar2, item.P_SFIRSTNAME, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_NSEX", OracleDbType.Varchar2, item.P_SSEXCLIEN, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_DBIRTHDAT", OracleDbType.Date, Convert.ToDateTime(item.P_DBIRTHDAT), ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_SMODULEC", OracleDbType.Varchar2, item.P_SMODULEC, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_NRENTAMOUNT", OracleDbType.Varchar2, item.P_NRENTAMOUNT, ParameterDirection.Input));
            //    // OUTPUT
            //    OracleParameter P_COD_ERR = new OracleParameter("P_NINDICADOR", OracleDbType.Int32, flagConsulta, ParameterDirection.Output);
            //    parameter.Add(P_COD_ERR);

            //    this.ExecuteByStoredProcedureVT(sPackageName, parameter);
            //    flagConsulta = Convert.ToInt32(P_COD_ERR.Value.ToString());
            //}
            //catch (Exception ex)
            //{
            //    flagConsulta = 0;
            //    ELog.save(this, ex.ToString());
            //}

            return flagConsulta;
        }

        public int validarConsultaAseguradoPD(ConsultaCBM item, ConsultaCBM request)
        {
            int flagConsulta = 0;

            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        OracleDataReader dr;

                        cmd.Connection = cn;
                        cmd.CommandText = ProcedureName.sp_LeerClienteClient;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new OracleParameter("P_NTYPE_DOCUMENT", OracleDbType.Int32, request.P_NIDDOC_TYPE, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, item.P_SIDDOC, ParameterDirection.Input));

                        var P_COD_ERR = new OracleParameter("P_NINDICADOR", OracleDbType.Int32, ParameterDirection.Output);

                        //P_COD_ERR.Size = 200;

                        cmd.Parameters.Add(P_COD_ERR);

                        cn.Open();
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        flagConsulta = Convert.ToInt32(P_COD_ERR.Value.ToString());

                        ELog.CloseConnection(dr);
                    }
                    catch (Exception ex)
                    {
                        flagConsulta = 0;
                        LogControl.save("validarConsultaAsegurado", "json: " + JsonConvert.SerializeObject(request) + " - json2: " + JsonConvert.SerializeObject(item), "2");
                        LogControl.save("validarConsultaAsegurado", ex.ToString(), "3");
                    }
                    finally
                    {
                        if (cn.State == ConnectionState.Open) cn.Close();
                    }
                }
            }

            return flagConsulta;
        }

        public ConsultaCBM equivalenteTipoDocumento(ConsultaCBM item)
        {
            var documentosList = new SharedDA().GetDocumentTypeList("");
            var documento = documentosList.Where(x => x.Name.ToUpper() == item.P_NIDDOC_TYPE.ToUpper()).Select(c => c.Id).ToList();

            return new ConsultaCBM()
            {
                P_NIDDOC_TYPE = documento.Count > 0 ? documento[0].ToString() : "",
                P_SIDDOC = item.P_SIDDOC,
                P_NUSERCODE = item.P_NUSERCODE,
                P_TipOper = item.P_TipOper
            };
        }

        public SalidaErrorBaseVM regLogCliente360(ConsultaCBM item, ResponseCVM response360)
        {
            var response = new SalidaErrorBaseVM();
            var sPackageName = ProcedureName.sp_InsertarLog360;
            List<OracleParameter> parameter = new List<OracleParameter>();
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_DESDOC_TYPE", OracleDbType.Varchar2, item.P_NIDDOC_TYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, item.P_SIDDOC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCLIENAME", OracleDbType.Varchar2, item.P_SLEGALNAME, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DESSEXCLIEN", OracleDbType.Varchar2, item.P_SSEXCLIEN, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DESCIVILSTA", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DBIRTHDAT", OracleDbType.Varchar2, item.P_DBIRTHDAT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SBLOCKADE", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SBLOCKLAFT", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SISCLIENT_IND", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NSTATREGT", OracleDbType.Int32, response360.P_NCODE, ParameterDirection.Input));

                this.ExecuteByStoredProcedureVT(sPackageName, parameter);

                response.P_COD_ERR = 0;
                response.P_MESSAGE = "Se insertó correctamente el registro";
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = ex.ToString();
                LogControl.save("regLogCliente360", ex.ToString(), "3");
            }

            return response;
        }

        public ResponseCVM GesClienteReniecTbl(ConsultaCBM data)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            var result = new ResponseCVM();
            result.EListClient = new List<EListClient>();

            string storedProcedureName = ProcedureName.sp_ReaClientReniec;
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NUMERODNI", OracleDbType.Varchar2, data.P_SIDDOC, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                if (odr.HasRows)
                {

                    while (odr.Read())
                    {
                        var item = new EListClient();
                        item.P_NIDDOC_TYPE = data.P_NIDDOC_TYPE;
                        item.P_SIDDOC = odr["NUMERODNI"] == DBNull.Value ? string.Empty : odr["NUMERODNI"].ToString();
                        item.P_DIG_VERIFICACION = odr["DIGITOVERIFICACION"] == DBNull.Value ? string.Empty : odr["DIGITOVERIFICACION"].ToString();
                        item.P_SFIRSTNAME = odr["NOMBRES"] == DBNull.Value ? string.Empty : odr["NOMBRES"].ToString();
                        item.P_SLASTNAME = odr["APELLIDOPATERNO"] == DBNull.Value ? string.Empty : odr["APELLIDOPATERNO"].ToString();
                        item.P_SLASTNAME2 = odr["APELLIDOMATERNO"] == DBNull.Value ? string.Empty : odr["APELLIDOMATERNO"].ToString();
                        item.P_APELLIDO_CASADA = odr["APELLIDOCASADA"] == DBNull.Value ? string.Empty : odr["APELLIDOCASADA"].ToString();
                        item.P_SSEXCLIEN = odr["SEXO"] == DBNull.Value ? string.Empty : odr["SEXO"].ToString();
                        item.P_DBIRTHDAT = odr["FECHANACIMIENTO"] == DBNull.Value ? string.Empty : DateTime.ParseExact(odr["FECHANACIMIENTO"].ToString(), "yyyyMMdd", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");
                        item.P_ORIGEN_DATA = "CLIENT_RENIEC";
                        result.EListClient.Add(item);
                    }
                }

                result.P_NCODE = result.EListClient.Count > 0 ? "0" : "1";
                result.P_SMESSAGE = result.EListClient.Count > 0 ? "El nro de dni existe en client_reniec" : "No existen registros con el nro de dni";

                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                result.P_NCODE = "1";
                result.P_SMESSAGE = ex.ToString();
            }

            return result;
        }

        public SalidaTramaBaseVM updateCoverageRE(studentCoverages aseg, InfoCover cover, validaTramaVM objValida)
        {
            var response = new SalidaTramaBaseVM();

            List<OracleParameter> parameter = new List<OracleParameter>();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, objValida.codProceso, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_DOCUMENT", OracleDbType.Int32, aseg.documentType, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, aseg.documentNumber, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCOVERGEN", OracleDbType.Int32, cover.id, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCAPITAL", OracleDbType.Double, cover.capital, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int64, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                P_SMESSAGE.Size = 9000;

                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);

                this.ExecuteByStoredProcedureVT(ProcedureName.pkg_ValidadorGenPD + ".SP_UPD_ASEGURADO_RE", parameter);
                response.P_COD_ERR = P_NCODE.Value.ToString();
                response.P_MESSAGE = P_SMESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = "1";
                response.P_MESSAGE = ex.ToString();
                LogControl.save("updateCoverageRE", ex.ToString(), "3");
            }

            return response;
        }

        public string getQuotationType(validaTramaVM objValida)
        {
            var response = string.Empty;

            List<OracleParameter> parameter = new List<OracleParameter>();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Varchar2, objValida.nroCotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, objValida.codProcesoOld, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_TYPE_COTIZACION = new OracleParameter("P_TYPE_COTIZACION", OracleDbType.Varchar2, ParameterDirection.Output);

                P_TYPE_COTIZACION.Size = 9000;

                parameter.Add(P_TYPE_COTIZACION);

                this.ExecuteByStoredProcedureVT("SP_GET_TIPO_COTIZACION", parameter);
                response = P_TYPE_COTIZACION.Value.ToString();
            }
            catch (Exception ex)
            {
                response = "PRIME";
                LogControl.save("getQuotationType", ex.ToString(), "3");
            }

            return response;
        }

        public void GenerateInfoAmount(double igv, string trx, ref validaTramaVM data)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_STIPO_COTIZACION", OracleDbType.Varchar2, trx != "trx" ? data.tipoCotizacion : "TRX", ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPLANILLA", OracleDbType.Double, data.montoPlanilla, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NRATE", OracleDbType.Double, data.ntasa_tariff, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRIMA_NETA_CIGV", OracleDbType.Double, trx != "trx" ? data.asegPremiumTotal : data.premium, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRIMA_TOTAL_CIGV", OracleDbType.Double, trx != "trx" ? data.premiumTotal : 0, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTASA_NIGV", OracleDbType.Double, igv, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCANTIDAD_ASEG", OracleDbType.Int64, data.CantidadTrabajadores, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STIPO_FACTURACION", OracleDbType.Int64, 2 /*data.datosPoliza.codTipoFacturacion*/, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_NPRIMA_NETA_UNITARIA = new OracleParameter("P_NPRIMA_NETA_UNITARIA", OracleDbType.Double, ParameterDirection.Output);
                OracleParameter P_NIGV_NETA_UNITARIA = new OracleParameter("P_NIGV_NETA_UNITARIA", OracleDbType.Double, ParameterDirection.Output);
                OracleParameter P_NIGV_NETA_UNIT_TOTAL = new OracleParameter("P_NIGV_NETA_UNIT_TOTAL", OracleDbType.Double, ParameterDirection.Output);
                OracleParameter P_NPRIMA = new OracleParameter("P_NPRIMA", OracleDbType.Double, ParameterDirection.Output);
                OracleParameter P_NIGV = new OracleParameter("P_NIGV", OracleDbType.Double, ParameterDirection.Output);
                OracleParameter P_NPRIMA_TOTAL = new OracleParameter("P_NPRIMA_TOTAL", OracleDbType.Double, ParameterDirection.Output);

                parameter.Add(P_NPRIMA_NETA_UNITARIA);
                parameter.Add(P_NIGV_NETA_UNITARIA);
                parameter.Add(P_NIGV_NETA_UNIT_TOTAL);
                parameter.Add(P_NPRIMA);
                parameter.Add(P_NIGV);
                parameter.Add(P_NPRIMA_TOTAL);

                this.ExecuteByStoredProcedureVT(ProcedureName.pkg_GeneraTransac + ".SP_DISGREGA_MONTOS", parameter);
                data.premium = Convert.ToDouble(P_NPRIMA.Value.ToString().Replace(",", "."));
                data.igv = Convert.ToDouble(P_NIGV.Value.ToString().Replace(",", "."));
                data.premiumTotal = Convert.ToDouble(P_NPRIMA_TOTAL.Value.ToString().Replace(",", "."));
                data.asegPremium = Convert.ToDouble(P_NPRIMA_NETA_UNITARIA.Value.ToString().Replace(",", "."));
                data.asegIgv = Convert.ToDouble(P_NIGV_NETA_UNITARIA.Value.ToString().Replace(",", "."));
                data.asegPremiumTotal = Convert.ToDouble(P_NIGV_NETA_UNIT_TOTAL.Value.ToString().Replace(",", "."));
            }
            catch (Exception ex)
            {
                LogControl.save("updateCoverageRE", ex.ToString(), "3");
            }

        }

        public SalidaTramaBaseVM InsertInsuredCot(validaTramaVM data)
        {
            var response = new SalidaTramaBaseVM();

            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        OracleDataReader dr;

                        var flagAccess = new string[] { ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch") }.Contains(data.datosPoliza.branch.ToString());

                        cmd.Connection = cn;
                        cmd.CommandText = ProcedureName.pkg_ReaDataAP + ".INS_INSURED_COT";
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Varchar2, data.datosPoliza.branch.ToString(), ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Varchar2, data.datosPoliza.codTipoProducto.ToString(), ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, data.codProceso, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NIDDOC_TYPE", OracleDbType.Varchar2, null, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, null, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NTIP_RENOV", OracleDbType.Int32, data.datosPoliza.codTipoRenovacion, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NPAYFREQ", OracleDbType.Int32, data.datosPoliza.codTipoFrecuenciaPago, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NPREMIUMN", OracleDbType.Double, data.premium, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NPREMIUMN_ANU", OracleDbType.Int64, null, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NCANT_INSURED", OracleDbType.Int64, null, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int64, null, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int64, data.codUsuario, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NTYPE_BILLING", OracleDbType.Int32, data.datosPoliza.codTipoFacturacion, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NIGV", OracleDbType.Double, flagAccess ? data.igv : 0, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NPREMIUMT", OracleDbType.Double, flagAccess ? data.premiumTotal : 0, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NPREMIUMN_ASEG", OracleDbType.Double, flagAccess ? data.asegPremium : 0, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NIGV_ASEG", OracleDbType.Double, flagAccess ? data.asegIgv : 0, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NPREMIUMT_ASEG", OracleDbType.Double, flagAccess ? data.asegPremiumTotal : 0, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NFLAG_REC", OracleDbType.Int32, flagAccess ? data.flagCalcular : 0, ParameterDirection.Input)); //AVS - RENTAS

                        var P_COD_ERR = new OracleParameter("P_ERROR", OracleDbType.Int32, ParameterDirection.Output);
                        var P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                        P_MESSAGE.Size = 4000;

                        cmd.Parameters.Add(P_COD_ERR);
                        cmd.Parameters.Add(P_MESSAGE);

                        cn.Open();
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        response.P_COD_ERR = P_COD_ERR.Value.ToString();
                        response.P_MESSAGE = P_MESSAGE.Value.ToString();

                        ELog.CloseConnection(dr);
                    }
                    catch (Exception ex)
                    {
                        response.P_COD_ERR = "1";
                        response.P_MESSAGE = ex.ToString();
                        LogControl.save("InsertInsuredCot", ex.ToString(), "3");
                    }
                    finally
                    {
                        if (cn.State == ConnectionState.Open) cn.Close();
                    }
                }
            }

            return response;
        }

        public SalidaTramaBaseVM InsertInsuredCotRentEst(validaTramaVM data)
        {
            var response = new SalidaTramaBaseVM();

            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        OracleDataReader dr;

                        cmd.Connection = cn;
                        cmd.CommandText = ProcedureName.sp_GenPremiumEstVG;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, data.codProceso, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.datosPoliza.branch.ToString(), ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, data.datosPoliza.codTipoProducto.ToString(), ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NPAYFREQ", OracleDbType.Int32, data.datosPoliza.codTipoFrecuenciaPago, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, data.datosContratante.codContratante, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NTECNICA", OracleDbType.Int32, data.ntecnica, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int64, data.nroCotizacion, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NTYPE_BILLING", OracleDbType.Int32, data.datosPoliza.codTipoFacturacion, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_STYPE_TRANSAC", OracleDbType.Varchar2, data.datosPoliza.trxCode, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int64, data.codUsuario, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_DEFFECDATE_ASE", OracleDbType.Varchar2, data.datosPoliza.InicioVigAsegurado, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_DEXPIRDAT_POL", OracleDbType.Varchar2, data.datosPoliza.FinVigPoliza, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NTASA", OracleDbType.Decimal, data.ntasa_tariff, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NTYPE_COT", OracleDbType.Varchar2, data.tipoCotizacion, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NIGV", OracleDbType.Decimal, data.igvPD, ParameterDirection.Input));

                        var P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, ParameterDirection.Output);
                        var P_MESSAGE = new OracleParameter("P_SERROR", OracleDbType.Varchar2, ParameterDirection.Output);
                        //var P_NPRIMA = new OracleParameter("P_NPRIMA", OracleDbType.Double, ParameterDirection.Output);

                        P_MESSAGE.Size = 4000;

                        cmd.Parameters.Add(P_COD_ERR);
                        cmd.Parameters.Add(P_MESSAGE);
                        //cmd.Parameters.Add(P_NPRIMA);

                        cn.Open();
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        response.P_COD_ERR = P_COD_ERR.Value.ToString();
                        response.P_MESSAGE = P_MESSAGE.Value.ToString();
                        //response.PRIMA = Convert.ToDouble(P_NPRIMA.Value.ToString());

                        ELog.CloseConnection(dr);
                    }
                    catch (Exception ex)
                    {
                        response.P_COD_ERR = "1";
                        response.P_MESSAGE = ex.ToString();
                        LogControl.save("InsertInsuredCotRentEst", ex.ToString(), "3");
                    }
                    finally
                    {
                        if (cn.State == ConnectionState.Open) cn.Close();
                    }
                }
            }

            return response;
        }

        public List<coberturasDetail> getCoverEcommerce(ValidaTramaEcommerce data, int cod_plan)
        {
            var response = new List<coberturasDetail>();

            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        OracleDataReader dr;

                        cmd.Connection = cn;
                        cmd.CommandText = ProcedureName.sp_GetCoverEC;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.cod_ramo, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, data.cod_producto, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NCURRENCY", OracleDbType.Int32, data.cod_moneda, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, cod_plan, ParameterDirection.Input));

                        cmd.Parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;

                        cn.Open();
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                var item = new coberturasDetail();
                                item.num = dr["NUM_COVER"] == DBNull.Value ? 0 : Convert.ToInt32(dr["NUM_COVER"].ToString());
                                item.cod_cobertura = dr["COD_COVER"] == DBNull.Value ? 0 : Convert.ToInt32(dr["COD_COVER"].ToString());
                                item.des_cobertura = dr["DES_COVER"] == DBNull.Value ? "" : dr["DES_COVER"].ToString();
                                item.obligatorio = dr["FLAG"] == DBNull.Value ? false : String.IsNullOrEmpty(dr["FLAG"].ToString()) ? false : dr["FLAG"].ToString() == "1" ? true : false;
                                response.Add(item);
                            }
                        }

                        ELog.CloseConnection(dr);
                    }
                    catch (Exception ex)
                    {
                        LogControl.save("getCoverEcommerce", ex.ToString(), "3");
                    }
                    finally
                    {
                        if (cn.State == ConnectionState.Open) cn.Close();
                    }
                }
            }

            return response;
        }

        public SalidaTramaBaseVM DeleteInsuredCot(validaTramaVM data)
        {
            var response = new SalidaTramaBaseVM();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.pkg_ReaDataAP + ".DEL_INSURED_COT";

            try
            {
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, data.codProceso, ParameterDirection.Input));

                OracleParameter P_COD_ERR = new OracleParameter("P_ERROR", OracleDbType.Int64, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                P_COD_ERR.Size = 9000;
                P_MESSAGE.Size = 9000;

                parameter.Add(P_COD_ERR);
                parameter.Add(P_MESSAGE);

                this.ExecuteByStoredProcedureVT(storeprocedure, parameter);

                response.P_COD_ERR = P_COD_ERR.Value.ToString();
                response.P_MESSAGE = P_MESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = "1";
                response.P_MESSAGE = ex.ToString();
                LogControl.save("DeleteInsuredCot", ex.ToString(), "3");
            }

            return response;
        }

        public SalidaErrorBaseVM InsCargaMasEspecial(validaTramaVM data)
        {
            SalidaErrorBaseVM response = new SalidaErrorBaseVM();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.pkg_ValidadorGenPD + ".SP_INS_CARGA_MAS_SCTR_ESPECIAL";

            try
            {
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Varchar2, data.nroCotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, data.codProceso, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Varchar2, data.datosPoliza.branch, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SFIRSTNAME", OracleDbType.Varchar2, data.datosContratante.nombre, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLASTNAME", OracleDbType.Varchar2, data.datosContratante.apePaterno, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLASTNAME2", OracleDbType.Varchar2, data.datosContratante.apeMaterno, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int64, 1, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NNATIONALITY", OracleDbType.Int64, !string.IsNullOrEmpty(data.datosContratante.nacionalidad) ? data.datosContratante.nacionalidad : "1", ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDDOC_TYPE", OracleDbType.Int64, data.datosContratante.codDocumento, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SIDDOC", OracleDbType.Int64, data.datosContratante.documento, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DBIRTHDAT", OracleDbType.Varchar2, Convert.ToDateTime(data.datosContratante.fechaNacimiento).ToString("dd/MM/yyyy"), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NREMUNERACION", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDCLIENTLOCATION", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCOD_NETEO", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Varchar2, data.codUsuario, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SSTATREGT", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOMMENT", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_REG", OracleDbType.Varchar2, "1", ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SSEXCLIEN", OracleDbType.Varchar2, data.datosContratante.sexo, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NACTION", OracleDbType.Int64, 0, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SROLE", OracleDbType.Int64, 2, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCIVILSTA", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SSTREET", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SPROVINCE", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLOCAL", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SMUNICIPALITY", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SE_MAIL", OracleDbType.Varchar2, data.datosContratante.email, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SPHONE_TYPE", OracleDbType.Varchar2, data.datosContratante.desTelefono, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SPHONE", OracleDbType.Varchar2, data.datosContratante.telefono, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDDOC_TYPE_BENEFICIARY", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SIDDOC_BENEFICIARY", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TYPE_PLAN", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_PERCEN_PARTICIPATION", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SRELATION", OracleDbType.Varchar2, null, ParameterDirection.Input));

                OracleParameter P_COD_ERR = new OracleParameter("P_NERROR", OracleDbType.Int64, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                P_COD_ERR.Size = 9000;
                P_MESSAGE.Size = 9000;

                parameter.Add(P_COD_ERR);
                parameter.Add(P_MESSAGE);

                this.ExecuteByStoredProcedureVT(storeprocedure, parameter);

                response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                response.P_MESSAGE = Convert.ToString(P_MESSAGE.Value.ToString());

            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = ex.ToString();
                LogControl.save("InsCargaMasEspecial", ex.ToString(), "3");
            }

            return response;
        }
        public SalidaErrorBaseVM InsCargaMasEspecialBeneficiarios(validaTramaVM data, DatosBenefactors beneficiarios)
        {
            SalidaErrorBaseVM response = new SalidaErrorBaseVM();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.pkg_ValidadorGenPD + ".SP_INS_CARGA_MAS_SCTR_ESPECIAL";

            try
            {
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Varchar2, data.nroCotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, data.codProceso, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Varchar2, data.datosPoliza.branch, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SFIRSTNAME", OracleDbType.Varchar2, beneficiarios.nombre, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLASTNAME", OracleDbType.Varchar2, beneficiarios.apePaterno, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLASTNAME2", OracleDbType.Varchar2, beneficiarios.apeMaterno, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int64, 1, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NNATIONALITY", OracleDbType.Int64, beneficiarios.nacionalidad, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDDOC_TYPE", OracleDbType.Int64, data.datosContratante.codDocumento, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, data.datosContratante.documento, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DBIRTHDAT", OracleDbType.Varchar2, beneficiarios.fechaNacimiento, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NREMUNERACION", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDCLIENTLOCATION", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCOD_NETEO", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Varchar2, data.codUsuario, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SSTATREGT", OracleDbType.Varchar2, "1", ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOMMENT", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_REG", OracleDbType.Varchar2, "1", ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SSEXCLIEN", OracleDbType.Varchar2, beneficiarios.sexo, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NACTION", OracleDbType.Int64, 0, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SROLE", OracleDbType.Int64, 16, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCIVILSTA", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SSTREET", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SPROVINCE", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLOCAL", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SMUNICIPALITY", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SE_MAIL", OracleDbType.Varchar2, beneficiarios.email, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SPHONE_TYPE", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SPHONE", OracleDbType.Varchar2, beneficiarios.telefono, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDDOC_TYPE_BENEFICIARY", OracleDbType.Varchar2, beneficiarios.codDocumento, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SIDDOC_BENEFICIARY", OracleDbType.Varchar2, beneficiarios.documento, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TYPE_PLAN", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_PERCEN_PARTICIPATION", OracleDbType.Varchar2, beneficiarios.porcentaParticipacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SRELATION", OracleDbType.Varchar2, beneficiarios.relacion, ParameterDirection.Input));

                OracleParameter P_COD_ERR = new OracleParameter("P_NERROR", OracleDbType.Int64, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                P_COD_ERR.Size = 9000;
                P_MESSAGE.Size = 9000;

                parameter.Add(P_COD_ERR);
                parameter.Add(P_MESSAGE);

                this.ExecuteByStoredProcedureVT(storeprocedure, parameter);

                response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                response.P_MESSAGE = Convert.ToString(P_MESSAGE.Value.ToString());

            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = ex.ToString();
                LogControl.save("InsCargaMasEspecialBeneficiarios", ex.ToString(), "3");
            }

            return response;
        }

        public SalidaTramaBaseVM insertCoverRequerid(validaTramaVM request, List<Coverage> coberturas)
        {
            var response = new SalidaTramaBaseVM();

            try
            {
                var dataTable = generateDataTableCover(request, coberturas);
                response = this.SaveUsingOracleBulkCopy(dataTable, ProcedureName.tbl_CotizacionCoverPD);
            }
            catch (Exception ex)
            {
                LogControl.save("insertCoverRequerid", ex.ToString(), "3");
            }

            return response;
        }

        private DataTable generateDataTableCover(validaTramaVM request, List<Coverage> coberturas)
        {
            DataTable dt = new DataTable();

            try
            {
                dt.Columns.Add("NIDPROD");
                dt.Columns.Add("NID_COTIZACION", typeof(int));
                dt.Columns.Add("NBRANCH", typeof(int));
                dt.Columns.Add("NPRODUCT", typeof(int));
                dt.Columns.Add("NCURRENCY", typeof(int));
                dt.Columns.Add("NTYPE_PRODUCT", typeof(int));
                dt.Columns.Add("NTYPE_PROFILE", typeof(int));
                dt.Columns.Add("NMODULEC", typeof(int));
                dt.Columns.Add("STYPE_TRANSAC");
                dt.Columns.Add("NCOVERGEN", typeof(int));
                dt.Columns.Add("SDESCRIPT");
                dt.Columns.Add("NCAPITAL", typeof(decimal));
                dt.Columns.Add("NCAPITAL_PRO", typeof(decimal));
                dt.Columns.Add("NCAPITAL_MAX", typeof(decimal));
                dt.Columns.Add("NCAPITAL_MIN", typeof(decimal));
                dt.Columns.Add("NCAPITAL_AUT", typeof(decimal));
                dt.Columns.Add("SCOVERUSE", typeof(char));
                dt.Columns.Add("NPERCCOVERED", typeof(decimal));
                dt.Columns.Add("NAGEMINPER", typeof(int));
                dt.Columns.Add("NAGEMAXPER", typeof(int));
                dt.Columns.Add("NAGEMINING", typeof(int));
                dt.Columns.Add("NAGEMAXING", typeof(int));
                dt.Columns.Add("NLIMIT", typeof(decimal));
                dt.Columns.Add("NHOUR", typeof(int));
                dt.Columns.Add("DEFFECDATE", typeof(DateTime));
                dt.Columns.Add("DNULLDATE", typeof(DateTime));
                dt.Columns.Add("DCOMPDATE", typeof(DateTime));
                dt.Columns.Add("NUSERCODE", typeof(int));
                dt.Columns.Add("NFACTOR", typeof(int));
                dt.Columns.Add("SCLIENT", typeof(char));
                dt.Columns.Add("ID_COVER");
                dt.Columns.Add("DES_COVER");
                dt.Columns.Add("PENSION_BASE", typeof(decimal));
                dt.Columns.Add("PENSION_MAX", typeof(decimal));
                dt.Columns.Add("PENSION_MIN", typeof(decimal));
                dt.Columns.Add("PENSION_PROP", typeof(decimal));
                dt.Columns.Add("PORCEN_BASE", typeof(decimal));
                dt.Columns.Add("PORCEN_MAX", typeof(decimal));
                dt.Columns.Add("PORCEN_MIN", typeof(decimal));
                dt.Columns.Add("PORCEN_PROP", typeof(decimal));
                dt.Columns.Add("COPAYMENT", typeof(decimal));
                dt.Columns.Add("MAXACCUMULATION", typeof(decimal));
                dt.Columns.Add("LACKPERIOD", typeof(int));
                dt.Columns.Add("DEDUCTIBLE", typeof(decimal));
                dt.Columns.Add("SCOMMENT");
                dt.Columns.Add("NID_REG");
                dt.Columns.Add("SCLIENT_BENEFICIARY", typeof(char));

                foreach (var cobertura in coberturas)
                {
                    DataRow dr = dt.NewRow();
                    dr["NIDPROD"] = request.codProceso;
                    dr["NID_COTIZACION"] = 0;
                    dr["NBRANCH"] = request.datosPoliza.branch;
                    dr["NPRODUCT"] = request.datosPoliza.codTipoProducto;
                    dr["NCURRENCY"] = request.datosPoliza.codMon;
                    dr["NTYPE_PRODUCT"] = request.datosPoliza.codTipoNegocio;
                    dr["NTYPE_PROFILE"] = request.datosPoliza.codTipoPerfil;
                    dr["NMODULEC"] = ELog.obtainConfig("planMaestro" + request.datosPoliza.branch);
                    dr["STYPE_TRANSAC"] = request.datosPoliza.trxCode;
                    dr["NCOVERGEN"] = cobertura.id;
                    dr["SDESCRIPT"] = cobertura.description;
                    dr["NCAPITAL"] = cobertura.capital.@base;
                    dr["NCAPITAL_PRO"] = cobertura.capital.@base;
                    dr["NCAPITAL_MAX"] = cobertura.capital.max;
                    dr["NCAPITAL_MIN"] = cobertura.capital.min;
                    dr["NCAPITAL_AUT"] = 0;
                    dr["SCOVERUSE"] = Convert.ToBoolean(cobertura.required) ? "1" : "0";
                    dr["NPERCCOVERED"] = cobertura.capitalCovered;
                    dr["NAGEMINPER"] = cobertura.stayAge.min;
                    dr["NAGEMAXPER"] = cobertura.stayAge.max;
                    dr["NAGEMINING"] = cobertura.entryAge.min;
                    dr["NAGEMAXING"] = cobertura.entryAge.max;
                    dr["NLIMIT"] = cobertura.limit;
                    dr["NHOUR"] = cobertura.hours;
                    dr["DEFFECDATE"] = Convert.ToDateTime(request.datosPoliza.InicioVigPoliza);
                    dr["DNULLDATE"] = DBNull.Value;
                    dr["DCOMPDATE"] = DateTime.Now;
                    dr["NUSERCODE"] = request.codUsuario;
                    dr["NFACTOR"] = 0;
                    dr["SCLIENT"] = DBNull.Value;
                    dr["ID_COVER"] = cobertura.capital.@type;
                    dr["DES_COVER"] = cobertura.capital.@type == "FIXED_PENSION_QUANTITY" ? "CANT. PENSIONES FIJA" :
                                        cobertura.capital.@type == "MAXIMUM_PENSION_QUANTITY" ? "CANT. PENSION MAXIMA" :
                                        cobertura.capital.@type == "FIXED_INSURED_SUM" ? "SUMA ASEGURADA" :
                                        cobertura.capital.@type == "FIXED_INSURED_SUM_PERCENTAGE " ? "%" :
                                        "SUMA ASEGURADA";
                    dr["PENSION_BASE"] = cobertura.capital.@type == "FIXED_PENSION_QUANTITY" ? cobertura.capital.@base : cobertura.capital.@type == "MAXIMUM_PENSION_QUANTITY" ? cobertura.capital.@base : 0;
                    dr["PENSION_MAX"] = cobertura.capital.@type == "FIXED_PENSION_QUANTITY" ? cobertura.capital.max : cobertura.capital.@type == "MAXIMUM_PENSION_QUANTITY" ? cobertura.capital.max : 0;
                    dr["PENSION_MIN"] = cobertura.capital.@type == "FIXED_PENSION_QUANTITY" ? cobertura.capital.min : cobertura.capital.@type == "MAXIMUM_PENSION_QUANTITY" ? cobertura.capital.min : 0;
                    dr["PENSION_PROP"] = cobertura.capital.@type == "FIXED_PENSION_QUANTITY" ? cobertura.capital.@base : cobertura.capital.@type == "MAXIMUM_PENSION_QUANTITY" ? cobertura.capital.@base : 0;
                    dr["PORCEN_BASE"] = cobertura.capital.@type == "FIXED_INSURED_SUM_PERCENTAGE" ? cobertura.capitalCovered : 0;
                    dr["PORCEN_MAX"] = cobertura.capital.@type == "FIXED_INSURED_SUM_PERCENTAGE" ? cobertura.capitalCovered : 0;
                    dr["PORCEN_MIN"] = cobertura.capital.@type == "FIXED_INSURED_SUM_PERCENTAGE" ? cobertura.capitalCovered : 0;
                    dr["PORCEN_PROP"] = cobertura.capital.@type == "FIXED_INSURED_SUM_PERCENTAGE" ? cobertura.capitalCovered : 0;
                    dr["COPAYMENT"] = cobertura.copayment;
                    dr["MAXACCUMULATION"] = cobertura.maxAccumulation;
                    dr["LACKPERIOD"] = cobertura.lackPeriod;
                    dr["DEDUCTIBLE"] = cobertura.deductible;
                    dr["SCOMMENT"] = cobertura.comment;
                    dr["NID_REG"] = DBNull.Value;
                    dr["SCLIENT_BENEFICIARY"] = DBNull.Value;

                    dt.Rows.Add(dr);
                }
            }
            catch (Exception ex)
            {
                LogControl.save("generateDataTableCover", ex.ToString(), "3");
            }

            return dt;
        }

        public int getFactor(string nbranch, string nproduct, string ncover, string sclient, int ntecnica = 0)
        {
            int nvalue = 0;
            int ntype = 0;

            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        OracleDataReader dr;

                        cmd.Connection = cn;
                        cmd.CommandText = ProcedureName.pkg_ReaDataAP + ".GET_FACTOR_COVER_PD";
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, nbranch, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, nproduct, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NCOVER", OracleDbType.Int32, ncover, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NTECNICA", OracleDbType.Int32, ntecnica, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, sclient, ParameterDirection.Input));

                        var P_NVALUE = new OracleParameter("P_NVALUEA", OracleDbType.Int32, ParameterDirection.Output);
                        var P_NTYPE = new OracleParameter("P_NVALUEB", OracleDbType.Int32, ParameterDirection.Output);

                        //P_COD_ERR.Size = 200;

                        cmd.Parameters.Add(P_NVALUE);
                        cmd.Parameters.Add(P_NTYPE);

                        cn.Open();
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        nvalue = Convert.ToInt32(P_NVALUE.Value.ToString());
                        ntype = Convert.ToInt32(P_NTYPE.Value.ToString());

                        ELog.CloseConnection(dr);
                    }
                    catch (Exception ex)
                    {
                        nvalue = 0;
                        LogControl.save("getFactor", ex.ToString(), "3");
                    }
                    finally
                    {
                        if (cn.State == ConnectionState.Open) cn.Close();
                    }
                }
            }

            return nvalue;
        }

        public SalidaTramaBaseVM getTextCoverDetail(validaTramaVM data)
        {
            var response = new SalidaTramaBaseVM();

            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        OracleDataReader dr;

                        cmd.Connection = cn;
                        cmd.CommandText = ProcedureName.pkg_ReaDataAP + ".GET_TEXT_COVER_DETAIL";
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.datosPoliza.branch, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, data.datosPoliza.codTipoProducto, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, data.codProceso, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NTECNICA", OracleDbType.Int32, data.ntecnica, ParameterDirection.Input));

                        var P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);

                        //P_COD_ERR.Size = 200;

                        cmd.Parameters.Add(P_NCODE);

                        cn.Open();
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        response.P_COD_ERR = P_NCODE.Value.ToString();

                        ELog.CloseConnection(dr);
                    }
                    catch (Exception ex)
                    {
                        response.P_COD_ERR = "1";
                        LogControl.save("getTextCoverDetail", ex.ToString(), "3");
                    }
                    finally
                    {
                        if (cn.State == ConnectionState.Open) cn.Close();
                    }
                }
            }

            return response;
        }

        public SalidaErrorBaseVM insertCoverRequeridDes(validaTramaVM request, CoverageDes cobertura)
        {
            var response = new SalidaErrorBaseVM();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.pkg_ReaDataAP + ".INS_COVER_COT";

            DbConnection DataConnection = ConnectionGet(enuTypeDataBase.OracleVTime);
            DataConnection.Open();
            DbTransaction trx = DataConnection.BeginTransaction();

            try
            {
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.codProceso, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, request.datosPoliza.branch, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, request.datosPoliza.codTipoProducto, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCURRENCY", OracleDbType.Int32, request.datosPoliza.codMon, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_PRODUCT", OracleDbType.Int32, request.datosPoliza.codTipoNegocio, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_PROFILE", OracleDbType.Int32, request.datosPoliza.codTipoPerfil, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, ELog.obtainConfig("planMaestro" + request.datosPoliza.branch), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STYPE_TRANSAC", OracleDbType.Varchar2, request.datosPoliza.trxCode, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCOVERGEN", OracleDbType.Int64, cobertura.code, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCAPITAL", OracleDbType.Double, cobertura.capital.@base, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCAPITAL_PRO", OracleDbType.Double, cobertura.capital.@base, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCAPITAL_MAX", OracleDbType.Double, cobertura.capital.max, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCAPITAL_MIN", OracleDbType.Double, cobertura.capital.min, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCAPITAL_AUT", OracleDbType.Double, 0, ParameterDirection.Input));

                if (request.datosPoliza.branch.ToString() == ELog.obtainConfig("vidaIndividualBranch"))
                {
                    parameter.Add(new OracleParameter("P_SCOVERUSE", OracleDbType.Varchar2, Convert.ToBoolean(cobertura.optional) ? "0" : "1", ParameterDirection.Input));
                }
                else
                {
                    parameter.Add(new OracleParameter("P_SCOVERUSE", OracleDbType.Varchar2, Convert.ToBoolean(cobertura.required) ? "1" : "0", ParameterDirection.Input));
                }

                parameter.Add(new OracleParameter("P_NPERCCOVERED", OracleDbType.Double, cobertura.capitalCovered, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NAGEMINPER", OracleDbType.Int32, cobertura.stayAge.min, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NAGEMAXPER", OracleDbType.Int32, cobertura.stayAge.max, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NAGEMINING", OracleDbType.Int32, cobertura.entryAge.min, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NAGEMAXING", OracleDbType.Int32, cobertura.entryAge.max, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Date, request.datosPoliza.InicioVigPoliza, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int64, request.codUsuario, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLIMIT", OracleDbType.Double, cobertura.limit, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NHOUR", OracleDbType.Int64, cobertura.hours, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SDESCRIPT", OracleDbType.Varchar2, cobertura.description, ParameterDirection.Input));

                OracleParameter P_COD_ERR = new OracleParameter("P_ERROR", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                P_COD_ERR.Size = 9000;
                P_MESSAGE.Size = 9000;

                parameter.Add(P_COD_ERR);
                parameter.Add(P_MESSAGE);

                this.ExecuteByStoredProcedureVT_TRX(storeprocedure, parameter, DataConnection, trx);

                response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                response.P_MESSAGE = Convert.ToString(P_MESSAGE.Value.ToString());
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = ex.ToString();
                LogControl.save("insertCoverRequeridDes", ex.ToString(), "3");
            }
            finally
            {
                if (response.P_COD_ERR == 0)
                {
                    trx.Commit();
                }
                else
                {
                    if (trx.Connection != null) trx.Rollback();
                }

                if (trx.Connection != null) trx.Dispose();
                ELog.CloseConnection(DataConnection);
            }

            return response;
        }

        public SalidaErrorBaseVM insertAssistRequeridDes(validaTramaVM request, AssistanceDes asistencia)
        {
            var response = new SalidaErrorBaseVM();
            List<OracleParameter> parameter = new List<OracleParameter>();
            //string storedProcedureName = ProcedureName.pkg_ReaDataAP + ".INS_COVER_INTEGRA_EC";
            string storeprocedure = ProcedureName.pkg_ReaDataAP + ".INS_ASSISTANCE_COT";

            DbConnection DataConnection = ConnectionGet(enuTypeDataBase.OracleVTime);
            DataConnection.Open();
            DbTransaction trx = DataConnection.BeginTransaction();

            try
            {
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.codProceso, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, request.datosPoliza.branch, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, request.datosPoliza.codTipoProducto, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, ELog.obtainConfig("planMaestro" + request.datosPoliza.branch), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPROVIDER", OracleDbType.Varchar2, asistencia.provider.code, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCOD_ASSISTANCE", OracleDbType.Varchar2, asistencia.code, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SDESC_ASSISTANCE", OracleDbType.Varchar2, asistencia.description, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPERCENT", OracleDbType.Double, 0, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NAMOUNT", OracleDbType.Double, 0, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Date, request.datosPoliza.InicioVigPoliza, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, request.datosPoliza.FinVigPoliza, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int64, request.codUsuario, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_URL", OracleDbType.Varchar2, asistencia.document, ParameterDirection.Input));

                OracleParameter P_COD_ERR = new OracleParameter("P_ERROR", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                P_COD_ERR.Size = 9000;
                P_MESSAGE.Size = 9000;

                parameter.Add(P_COD_ERR);
                parameter.Add(P_MESSAGE);


                parameter.Add(new OracleParameter("P_SASSISTUSE", OracleDbType.Varchar2, Convert.ToBoolean(asistencia.optional) ? "0" : "1", ParameterDirection.Input));

                this.ExecuteByStoredProcedureVT_TRX(storeprocedure, parameter, DataConnection, trx);

                response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                response.P_MESSAGE = Convert.ToString(P_MESSAGE.Value.ToString());
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = ex.ToString();
                LogControl.save("insertAssistRequeridDes", ex.ToString(), "3");
            }
            finally
            {
                if (response.P_COD_ERR == 0)
                {
                    trx.Commit();
                }
                else
                {
                    if (trx.Connection != null) trx.Rollback();
                }

                if (trx.Connection != null) trx.Dispose();
                ELog.CloseConnection(DataConnection);
            }

            return response;
        }

        public SalidaErrorBaseVM insertBenefitRequeridDes(validaTramaVM request, BenefitDes beneficios)
        {
            var response = new SalidaErrorBaseVM();
            List<OracleParameter> parameter = new List<OracleParameter>();
            //string storedProcedureName = ProcedureName.pkg_ReaDataAP + ".INS_COVER_INTEGRA_EC";
            string storeprocedure = ProcedureName.pkg_ReaDataAP + ".INS_BENEFIT_COT";

            DbConnection DataConnection = ConnectionGet(enuTypeDataBase.OracleVTime);
            DataConnection.Open();
            DbTransaction trx = DataConnection.BeginTransaction();

            try
            {
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.codProceso, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, request.datosPoliza.branch, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, request.datosPoliza.codTipoProducto, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, ELog.obtainConfig("planMaestro" + request.datosPoliza.branch), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCOD_BENEFIT", OracleDbType.Int64, beneficios.code, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SDESC_BENEFIT", OracleDbType.Varchar2, beneficios.description, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPERCENT", OracleDbType.Double, 0, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NAMOUNT", OracleDbType.Double, 0, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Date, request.datosPoliza.InicioVigPoliza, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, request.datosPoliza.FinVigPoliza, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int64, request.codUsuario, ParameterDirection.Input));

                OracleParameter P_COD_ERR = new OracleParameter("P_ERROR", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                P_COD_ERR.Size = 9000;
                P_MESSAGE.Size = 9000;

                parameter.Add(P_COD_ERR);
                parameter.Add(P_MESSAGE);

                parameter.Add(new OracleParameter("P_SBENEFITUSE", OracleDbType.Varchar2, Convert.ToBoolean(beneficios.optional) ? "0" : "1", ParameterDirection.Input));

                this.ExecuteByStoredProcedureVT_TRX(storeprocedure, parameter, DataConnection, trx);

                response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                response.P_MESSAGE = Convert.ToString(P_MESSAGE.Value.ToString());
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = ex.ToString();
                LogControl.save("insertBenefitRequeridDes", ex.ToString(), "3");
            }
            finally
            {
                if (response.P_COD_ERR == 0)
                {
                    trx.Commit();
                }
                else
                {
                    if (trx.Connection != null) trx.Rollback();
                }

                if (trx.Connection != null) trx.Dispose();
                ELog.CloseConnection(DataConnection);
            }

            return response;
        }

        public SalidaErrorBaseVM insertSurchargeRequeridDes(validaTramaVM request, SurchargeDes recargos)
        {
            var response = new SalidaErrorBaseVM();
            List<OracleParameter> parameter = new List<OracleParameter>();
            //string storedProcedureName = ProcedureName.pkg_ReaDataAP + ".INS_COVER_INTEGRA_EC";
            string storeprocedure = ProcedureName.pkg_ReaDataAP + ".INS_BENEFIT_COT";

            DbConnection DataConnection = ConnectionGet(enuTypeDataBase.OracleVTime);
            DataConnection.Open();
            DbTransaction trx = DataConnection.BeginTransaction();

            try
            {
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.codProceso, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, request.datosPoliza.branch, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, request.datosPoliza.codTipoProducto, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, ELog.obtainConfig("planMaestro" + request.datosPoliza.branch), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCOD_SURCHARGE", OracleDbType.Int64, recargos.code, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SDESC_SURCHARGE", OracleDbType.Varchar2, recargos.description, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPERCENT", OracleDbType.Double, 0, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NAMOUNT", OracleDbType.Double, 0, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Date, request.datosPoliza.InicioVigPoliza, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, request.datosPoliza.FinVigPoliza, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int64, request.codUsuario, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NSTATREGT", OracleDbType.Int32, Convert.ToBoolean(recargos.optional) ? "0" : "1", ParameterDirection.Input));

                OracleParameter P_COD_ERR = new OracleParameter("P_ERROR", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                P_COD_ERR.Size = 9000;
                P_MESSAGE.Size = 9000;

                parameter.Add(P_COD_ERR);
                parameter.Add(P_MESSAGE);


                parameter.Add(new OracleParameter("P_SSURCHARGEUSE", OracleDbType.Varchar2, Convert.ToBoolean(recargos.optional) ? "0" : "1", ParameterDirection.Input));

                this.ExecuteByStoredProcedureVT_TRX(storeprocedure, parameter, DataConnection, trx);

                response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                response.P_MESSAGE = Convert.ToString(P_MESSAGE.Value.ToString());
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = ex.ToString();
                LogControl.save("insertSurchargeRequeridDes", ex.ToString(), "3");
            }
            finally
            {
                if (response.P_COD_ERR == 0)
                {
                    trx.Commit();
                }
                else
                {
                    if (trx.Connection != null) trx.Rollback();
                }

                if (trx.Connection != null) trx.Dispose();
                ELog.CloseConnection(DataConnection);
            }

            return response;
        }

        public ReponsePrimaPlanVM CalculoPrimaEC(validaTramaVM data)

        {
            string storedProcedureName = ProcedureName.pkg_ReaDataAP + ".CAL_PREMIUM_COVER";
            List<OracleParameter> parameter = new List<OracleParameter>();
            ReponsePrimaPlanVM result = new ReponsePrimaPlanVM();
            SalidaErrorBaseVM response = new SalidaErrorBaseVM();


            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NIDPROD", OracleDbType.Varchar2, data.codProceso, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.datosPoliza.branch, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_PRODUCT", OracleDbType.Int32, data.datosPoliza.codTipoNegocio, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, data.datosPoliza.codTipoProducto, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, data.datosPoliza.codTipoPlan, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCURRENCY", OracleDbType.Int32, data.datosPoliza.codMon, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STYPE_TRANSAC", OracleDbType.Varchar2, data.datosPoliza.typeTransac, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMODALITY", OracleDbType.Int32, data.datosPoliza.codTipoModalidad, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCIUU", OracleDbType.Varchar2, data.datosPoliza.CodActividadRealizar, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_EMPLOYEE", OracleDbType.Int32, "1", ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Varchar2, data.datosPoliza.InicioVigPoliza, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, data.codUsuario, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTIP_RENOV", OracleDbType.Int32, data.datosPoliza.codTipoRenovacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPAYFREQ", OracleDbType.Int32, data.datosPoliza.codTipoFrecuenciaPago, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NNUM_TRABAJADORES", OracleDbType.Varchar2, data.CantidadTrabajadores, ParameterDirection.Input));
                //


                //OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Varchar2, result.codError, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, result.sMessage, ParameterDirection.Output);
                OracleParameter P_PREMIUMN = new OracleParameter("P_PREMIUMN", OracleDbType.Varchar2, result.sPremium, ParameterDirection.Output);
                P_SMESSAGE.Size = 9000;
                P_NCODE.Size = 9000;
                P_PREMIUMN.Size = 9000;
                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);
                parameter.Add(P_PREMIUMN);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                result.codError = P_NCODE.Value.ToString();
                result.sMessage = P_SMESSAGE.Value.ToString();
                result.sPremium = P_PREMIUMN.Value.ToString();

                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                result.codError = "1";
                result.sMessage = ex.ToString();
            }


            return result;


        }

        public ResponseAssists ListAssists(AssistsBM data)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseAssists result = new ResponseAssists();
            result.ListAssists = new List<AssistsVM>();

            string storedProcedureName = ProcedureName.pkg_ReaDataAP + ".REA_ASSISTANCE_ALL";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.codBranch, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, data.codProduct, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, ELog.obtainConfig("planMaestro" + data.codBranch), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_PROC ", OracleDbType.Varchar2, data.IdProc, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        AssistsVM item = new AssistsVM();

                        item.codAssist = odr["NCOD_ASSISTANCE"] == DBNull.Value ? string.Empty : odr["NCOD_ASSISTANCE"].ToString();
                        item.desAssist = odr["SDES_ASSISTANCE"] == DBNull.Value ? string.Empty : odr["SDES_ASSISTANCE"].ToString();
                        result.ListAssists.Add(item);
                    }
                }

                ELog.CloseConnection(odr);
                result.codError = "0";
                result.message = result.ListAssists.Count > 0 ? "Se ejecuto correctamente" : "No existen asistencias configuradas";
            }
            catch (Exception ex)
            {
                result.codError = "1";
                result.message = ex.ToString();
            }

            return result;
        }

        public ResponseBenefit ListBenefits(BenefitBM data)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseBenefit result = new ResponseBenefit();
            result.ListBenefits = new List<BenefitVM>();

            string storedProcedureName = ProcedureName.pkg_ReaDataAP + ".REA_BENEFITS_ALL";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.codBranch, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, data.codProduct, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, ELog.obtainConfig("planMaestro" + data.codBranch), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_PROC ", OracleDbType.Varchar2, data.IdProc, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        BenefitVM item = new BenefitVM();

                        item.codBenefit = odr["NCOD_BENEFIT"] == DBNull.Value ? string.Empty : odr["NCOD_BENEFIT"].ToString();
                        item.desBenefit = odr["SDES_BENEFIT"] == DBNull.Value ? string.Empty : odr["SDES_BENEFIT"].ToString();
                        item.sMark = odr["SMARK"] == DBNull.Value ? string.Empty : odr["SMARK"].ToString();

                        result.ListBenefits.Add(item);
                    }
                }

                ELog.CloseConnection(odr);
                result.codError = "0";
                result.message = result.ListBenefits.Count > 0 ? "Se ejecuto correctamente" : "No existen beneficios configurados";
            }
            catch (Exception ex)
            {
                result.codError = "1";
                result.message = ex.ToString();
            }

            return result;
        }

        public ResponseBenefit ListSurcharges(BenefitBM data)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseBenefit result = new ResponseBenefit();
            result.ListSurcharges = new List<SurchargeVM>();

            string storedProcedureName = ProcedureName.pkg_ReaDataAP + ".REA_SURCHARGES_ALL";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.codBranch, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, data.codProduct, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, ELog.obtainConfig("planMaestro" + data.codBranch), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_PROC ", OracleDbType.Varchar2, data.IdProc, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        SurchargeVM item = new SurchargeVM();

                        item.codSurcharge = odr["NCOD_SURCHARGE"] == DBNull.Value ? string.Empty : odr["NCOD_SURCHARGE"].ToString();
                        item.desSurcharge = odr["SDESC_SURCHARGE"] == DBNull.Value ? string.Empty : odr["SDESC_SURCHARGE"].ToString();
                        item.sMark = odr["SMARK"] == DBNull.Value ? string.Empty : odr["SMARK"].ToString();
                        item.value = odr["NPERCENT"] == DBNull.Value ? 0 : Convert.ToDouble(odr["NPERCENT"].ToString());
                        result.ListSurcharges.Add(item);
                    }
                }

                ELog.CloseConnection(odr);
                result.codError = "0";
                result.message = result.ListBenefits.Count > 0 ? "Se ejecuto correctamente" : "No existen beneficios configurados";
            }
            catch (Exception ex)
            {
                result.codError = "1";
                result.message = ex.ToString();
            }

            return result;
        }

        public ResponseBenefit ListAdditionalServices(BenefitBM data)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseBenefit result = new ResponseBenefit();
            result.ListBenefits = new List<BenefitVM>();

            string storedProcedureName = ProcedureName.pkg_ReaDataAP + ".REA_ADDITIONAL_SERVICES_ALL";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.codBranch, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, data.codProduct, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, ELog.obtainConfig("planMaestro" + data.codBranch), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_PROC ", OracleDbType.Varchar2, data.IdProc, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        AdditionalServiceVM item = new AdditionalServiceVM();
                        item.codServAdicionales = odr["NCOD_ADDITIONAL_SERVICES"] == DBNull.Value ? string.Empty : odr["NCOD_ADDITIONAL_SERVICES"].ToString();
                        item.desServAdicionales = odr["SDESC_ADDITIONAL_SERVICES"] == DBNull.Value ? string.Empty : odr["SDESC_ADDITIONAL_SERVICES"].ToString();
                        item.amount = odr["NHOUR"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NHOUR"].ToString());
                        item.sMark = odr["SMARK"] == DBNull.Value ? string.Empty : odr["SMARK"].ToString();

                        result.ListAdditionalService.Add(item);
                    }
                }

                ELog.CloseConnection(odr);
                result.codError = "0";
                result.message = result.ListBenefits.Count > 0 ? "Se ejecuto correctamente" : "No existen servicios adicionales configurados";
            }
            catch (Exception ex)
            {
                result.codError = "1";
                result.message = ex.ToString();
            }

            return result;
        }

        public List<PlanVM> GetPlansList(PlanBM data)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<PlanVM> planList = new List<PlanVM>();

            string storedProcedureName = ProcedureName.pkg_Cotizacion + ".REA_LIST_PLAN";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.P_NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, data.P_NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTIP_RENOV", OracleDbType.Int32, data.P_NTIP_RENOV, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCURRENCY", OracleDbType.Int32, data.P_NCURRENCY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDPLAN", OracleDbType.Int32, data.P_NIDPLAN, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        PlanVM item = new PlanVM();
                        item.NIDPLAN = odr["NIDPLAN"] == DBNull.Value ? string.Empty : odr["NIDPLAN"].ToString();
                        item.SDESCRIPT = odr["SDESCRIPT"] == DBNull.Value ? string.Empty : odr["SDESCRIPT"].ToString();
                        item.NPREMIUMN = odr["NPREMIUMN"] == DBNull.Value ? string.Empty : odr["NPREMIUMN"].ToString();
                        item.NDE = odr["NDE"] == DBNull.Value ? string.Empty : odr["NDE"].ToString();
                        item.NIGV = odr["NIGV"] == DBNull.Value ? string.Empty : odr["NIGV"].ToString();
                        item.NPREMIUM = odr["NPREMIUM"] == DBNull.Value ? string.Empty : odr["NPREMIUM"].ToString();
                        item.NCOMMI_RATE = odr["NCOMMI_RATE"] == DBNull.Value ? string.Empty : odr["NCOMMI_RATE"].ToString();
                        planList.Add(item);
                    }
                }

                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("GetPlansList", ex.ToString(), "3");
            }

            return planList;
        }

        public List<PlanVM> GetPlansListAcc(PlanBM data)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<PlanVM> planList = new List<PlanVM>();

            string storedProcedureName = ProcedureName.pkg_ReaDataAP + ".REA_TYPE_PROFILE";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.P_NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_PRODUCT", OracleDbType.Int32, data.P_NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Varchar2, data.DEFFECDATE, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        PlanVM item = new PlanVM();
                        item.NIDPLAN = odr["NTYPE_PROFILE"] == DBNull.Value ? string.Empty : odr["NTYPE_PROFILE"].ToString();
                        item.SDESCRIPT = odr["SDESCRIPT"] == DBNull.Value ? string.Empty : odr["SDESCRIPT"].ToString().Trim();
                        planList.Add(item);
                    }
                }

                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("GetPlansListAcc", ex.ToString(), "3");
            }

            return planList;
        }


        public List<TipoPlanVM> GetTipoPlan(PlanBM data)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<TipoPlanVM> lista = new List<TipoPlanVM>();

            string storedProcedureName = ProcedureName.pkg_ReaDataAP + ".REA_MODULEC";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Varchar2, data.P_NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_PRODUCT", OracleDbType.Varchar2, data.P_NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_PROFILE", OracleDbType.Varchar2, data.TYPE_PROFILE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Varchar2, data.DEFFECDATE, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        TipoPlanVM item = new TipoPlanVM();
                        item.ID_PLAN = Convert.ToInt32(odr["NMODULEC"] == DBNull.Value ? string.Empty : odr["NMODULEC"].ToString());
                        item.TIPO_PLAN = odr["SDESCRIPT"] == DBNull.Value ? string.Empty : odr["SDESCRIPT"].ToString();
                        lista.Add(item);
                    }
                }

                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("GetTipoPlan", ex.ToString(), "3");
            }

            return lista;
        }
        public QuotationResponseVM TRAS_CARGA_ASE_ENDOSO(QuotationCabBM request)
        {
            var sPackageName = ProcedureName.pkg_Poliza + ".TRAS_CARGA_ASE";
            List<OracleParameter> parameter = new List<OracleParameter>();
            QuotationResponseVM result = new QuotationResponseVM();


            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, request.NumeroCotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.CodigoProceso, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, request.P_NUSERCODE, ParameterDirection.Input));
                // SOLO RI
                parameter.Add(new OracleParameter("P_DSTARTDATE", OracleDbType.Date, Convert.ToDateTime(request.P_DSTARTDATE_ASE), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, Convert.ToDateTime(request.P_DEXPIRDAT_ASE), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Int32, 2, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPAYFREQ", OracleDbType.Int32, request.P_NPAYFREQ, ParameterDirection.Input));

                this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                result.P_COD_ERR = 0;
                result.P_MESSAGE = "Se realizo correctamente el traslado de asegurados";
            }
            catch (Exception ex)
            {
                result.P_COD_ERR = 1;
                result.P_MESSAGE = ex.ToString();
                LogControl.save("TRAS_CARGA_ASE_ENDOSO", ex.ToString(), "3");
            }

            return result;

        }

        public QuotationResponseVM UpdateCustomerAP(QuotationCabBM request, DbConnection dataConnection, DbTransaction trx)
        {
            var response = new QuotationResponseVM();
            response = TRAS_CARGA_ASE_ENDOSO(request);

            if (response.P_COD_ERR == 0)
            {
                var sPackageName = ProcedureName.pkg_GeneraTransac + ".SP_INS_UPD_CUSTOMER";

                List<OracleParameter> parameter = new List<OracleParameter>();
                QuotationResponseVM result = new QuotationResponseVM();

                try
                {
                    //INPUT
                    parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Decimal, request.NumeroCotizacion, ParameterDirection.Input)); ;
                    parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.CodigoProceso, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_SCERTYPE", OracleDbType.Varchar2, "2", ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, request.P_NBRANCH, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, request.P_NPRODUCT, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Decimal, request.NumeroPoliza, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_SKEY", OracleDbType.Varchar2, "", ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Decimal, request.P_NUSERCODE, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Varchar2, "8", ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_NCURRENCY", OracleDbType.Int32, request.P_NCURRENCY, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_DSTARTDATE", OracleDbType.Date, Convert.ToDateTime(request.P_DSTARTDATE_ASE), ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, Convert.ToDateTime(request.P_DEXPIRDAT_ASE), ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_NFLAG", OracleDbType.Decimal, 1, ParameterDirection.Input));

                    //OUTPUT
                    OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Decimal, ParameterDirection.Output);
                    OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                    P_NCODE.Size = 9000;
                    P_SMESSAGE.Size = 9000;
                    parameter.Add(P_NCODE);
                    parameter.Add(P_SMESSAGE);

                    this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, dataConnection, trx);
                    result.P_COD_ERR = Convert.ToInt32(P_NCODE.Value.ToString());
                    result.P_MESSAGE = P_SMESSAGE.Value.ToString();

                }
                catch (Exception ex)
                {
                    result.P_COD_ERR = 1;
                    result.P_MESSAGE = ex.ToString();
                    LogControl.save("UpdateCustomerAP", ex.ToString(), "3");
                }

                return result;
            }
            else
            {
                return response;
            }
        }

        public List<ModalidadVm> GetModalidad(PlanBM data)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<ModalidadVm> lista = new List<ModalidadVm>();

            string storedProcedureName = ProcedureName.pkg_ReaDataAP + ".REA_TYPE_MODALITY";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.P_NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_PRODUCT", OracleDbType.Int32, data.P_NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_PROFILE", OracleDbType.Int32, data.TYPE_PROFILE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Varchar2, data.DEFFECDATE, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        ModalidadVm item = new ModalidadVm();
                        item.ID = Convert.ToInt32(odr["NMODALITY"] == DBNull.Value ? string.Empty : odr["NMODALITY"].ToString());
                        item.DESCRIPCION = odr["SDESCRIPT"] == DBNull.Value ? string.Empty : odr["SDESCRIPT"].ToString();
                        lista.Add(item);
                    }
                }

                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("GetModalidad", ex.ToString(), "3");
            }

            return lista;
        }

        public List<CoberturasVM> GetCoberturas(CoberturaBM data)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<CoberturasVM> lista = new List<CoberturasVM>();

            string storedProcedureName = ProcedureName.pkg_ReaDataAP + ".REA_COVER_COT";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, data.IdProc, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.codBranch, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_PRODUCT", OracleDbType.Int32, data.tipoProducto, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_PROFILE", OracleDbType.Int32, data.codPerfil, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, ELog.obtainConfig("planMaestro" + data.codBranch), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCURRENCY", OracleDbType.Varchar2, data.monedaId, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STYPE_TRANSAC", OracleDbType.Varchar2, data.tipoTransac, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMODALITY", OracleDbType.Varchar2, data.tipoModalidad, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCIUU", OracleDbType.Varchar2, data.ciiuId, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_EMPLOYEE", OracleDbType.Varchar2, data.tipoEmpleado, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Varchar2, data.fechaEfecto, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);
                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        var item = new CoberturasVM();
                        item.codCobertura = odr["NCOVERGEN"] == DBNull.Value ? string.Empty : odr["NCOVERGEN"].ToString();
                        item.descripcion = odr["SDESCRIPT"] == DBNull.Value ? string.Empty : odr["SDESCRIPT"].ToString();
                        item.capital_prop = odr["NCAPITAL_PRO"] == DBNull.Value ? 0 : Convert.ToDouble(odr["NCAPITAL_PRO"].ToString());
                        item.capital_aut = odr["NCAPITAL_AUT"] == DBNull.Value ? 0 : Convert.ToDouble(odr["NCAPITAL_AUT"].ToString());
                        item.def = odr["DEF"] == DBNull.Value ? string.Empty : odr["DEF"].ToString();
                        item.nfactor = odr["NFACTOR"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NFACTOR"].ToString());
                        item.pension_prop = odr["PENSION_PROP"] == DBNull.Value ? 0 : Convert.ToInt32(odr["PENSION_PROP"].ToString());
                        item.porcen_prop = odr["PORCEN_PROP"] == DBNull.Value ? 0 : Convert.ToInt32(odr["PORCEN_PROP"].ToString());
                        item.id_cover = odr["ID_COVER"] == DBNull.Value ? string.Empty : odr["ID_COVER"].ToString();
                        lista.Add(item);
                    }
                }

                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("GetCoberturas", ex.ToString(), "3");
            }

            return lista;
        }

        public List<CoberturasVM> GetCoberturasGenDev(CoberturaBM data)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<CoberturasVM> lista = new List<CoberturasVM>();

            string storedProcedureName = ProcedureName.pkg_ReaDataAP + ".REA_COVER_DES_DEV";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.nbranch, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_PRODUCT", OracleDbType.Int32, data.tipoProducto, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, ELog.obtainConfig("planMaestro" + data.nbranch), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCURRENCY", OracleDbType.Varchar2, 1, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);
                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        var item = new CoberturasVM();
                        item.codCobertura = odr["NCOVERGEN"] == DBNull.Value ? string.Empty : odr["NCOVERGEN"].ToString();
                        item.descripcion = odr["SDESCRIPT"] == DBNull.Value ? string.Empty : odr["SDESCRIPT"].ToString();
                        item.capital_prop = odr["NCACALFIX"] == DBNull.Value ? 0 : Convert.ToDouble(odr["NCACALFIX"].ToString());
                        item.capital_aut = odr["NCACALFIX"] == DBNull.Value ? 0 : Convert.ToDouble(odr["NCACALFIX"].ToString());
                        item.capital_max = odr["NCAPMAXIM"] == DBNull.Value ? 0 : Convert.ToDouble(odr["NCAPMAXIM"].ToString());
                        item.capital_min = odr["NCAPMINIM"] == DBNull.Value ? 0 : Convert.ToDouble(odr["NCAPMINIM"].ToString());
                        item.cobertura_pri = odr["Principal?"] == DBNull.Value ? "0" : odr["Principal?"].ToString() == "S" ? "1" : "0";
                        lista.Add(item);
                    }
                }

                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("GetCoberturasGenDev", ex.ToString(), "3");
            }

            return lista;
        }


        //public List<PrimaPlanVM> GetPrimaPlan(PrimaPlanBM data)
        //{
        //    List<OracleParameter> parameter = new List<OracleParameter>();
        //    List<PrimaPlanVM> lista = new List<PrimaPlanVM>();


        //    string storedProcedureName = ProcedureName.pkg_ReaDataAP + ".CAL_PREMIUM_COVER_MODULEC_ALL";
        //    try
        //    {
        //        //INPUT
        //        parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.codBranch, ParameterDirection.Input));
        //        parameter.Add(new OracleParameter("P_NTYPE_PRODUCT", OracleDbType.Int32, data.codTypePolicy, ParameterDirection.Input));
        //        parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, data.codProduct, ParameterDirection.Input));
        //        parameter.Add(new OracleParameter("P_NCURRENCY", OracleDbType.Int32, data.codCurrency, ParameterDirection.Input));
        //        parameter.Add(new OracleParameter("P_STYPE_TRANSAC", OracleDbType.Varchar2, data.stransac, ParameterDirection.Input));
        //        parameter.Add(new OracleParameter("P_NMODALITY", OracleDbType.Int32, data.codTypeMod, ParameterDirection.Input));
        //        parameter.Add(new OracleParameter("P_NCIUU", OracleDbType.Varchar2, data.codActivity, ParameterDirection.Input));
        //        parameter.Add(new OracleParameter("P_NTYPE_EMPLOYEE", OracleDbType.Int32, "1", ParameterDirection.Input));
        //        parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Varchar2, data.policyInitDate, ParameterDirection.Input));
        //        parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, data.codUser, ParameterDirection.Input));
        //        parameter.Add(new OracleParameter("P_NFLAG", OracleDbType.Varchar2, "9", ParameterDirection.Input));
        //        parameter.Add(new OracleParameter("P_NTIP_RENOV", OracleDbType.Int32, data.codTypeRenov, ParameterDirection.Input));
        //        parameter.Add(new OracleParameter("P_NPAYFREQ", OracleDbType.Int32, data.codFreqPay, ParameterDirection.Input));
        //        parameter.Add(new OracleParameter("P_NNUM_TRABAJADORES", OracleDbType.Varchar2, data.workers, ParameterDirection.Input));
        //        parameter.Add(new OracleParameter("P_NTYPE_PROFILE", OracleDbType.Int32, data.codProfile, ParameterDirection.Input));

        //        //OUTPUT
        //        OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Varchar2, ParameterDirection.Output);
        //        OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
        //        OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
        //        P_SMESSAGE.Size = 9000;
        //        P_NCODE.Size = 9000;
        //        parameter.Add(P_NCODE);
        //        parameter.Add(P_SMESSAGE);
        //        parameter.Add(C_TABLE);


        //        OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

        //        if (odr.HasRows)
        //        {
        //            while (odr.Read())
        //            {
        //                PrimaPlanVM item = new PrimaPlanVM();
        //                item.codProceso = (data.codUser + ELog.generarCodigo() + DateTime.Now.ToString("yyyyMMddHHmmss")).PadLeft(30, '0');
        //                item.codPlan = odr["NMODULEC"] == DBNull.Value ? string.Empty : odr["NMODULEC"].ToString();
        //                item.desPlan = odr["SDESCRIPT"] == DBNull.Value ? string.Empty : odr["SDESCRIPT"].ToString().Trim();

        //                lista.Add(item);
        //            }
        //        }
        //        odr.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        ELog.save(this, ex.ToString());
        //    }

        //    return lista;
        //}

        public ComisionVM GetListComision()
        {
            var sPackageName = ProcedureName.sp_LeerComision;
            List<OracleParameter> parameter = new List<OracleParameter>();
            ComisionVM response = new ComisionVM();

            try
            {
                OracleParameter P_NCOMISION = new OracleParameter("P_NCOMISION", OracleDbType.Varchar2, response.nroComision, ParameterDirection.Output);

                P_NCOMISION.Size = 9000;
                parameter.Add(P_NCOMISION);
                this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                response.nroComision = P_NCOMISION.Value.ToString();

            }
            catch (Exception ex)
            {
                response.nroComision = "0";
                LogControl.save("GetListComision", ex.ToString(), "3");
            }

            return response;
        }

        public QuotationResponseVM UpdateQuotationCab(QuotationCabBM request, DbConnection connection, DbTransaction trx)
        {
            var sPackageName = ProcedureName.pkg_Cotizacion + ".UPD_COTIZACION_CAB_REC";

            List<OracleParameter> parameter = new List<OracleParameter>();
            QuotationResponseVM result = new QuotationResponseVM();

            try
            {
                // Vida Ley
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, request.NumeroCotizacion == 0 ? null : request.NumeroCotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DSTARTDATE", OracleDbType.Date, Convert.ToDateTime(request.P_DSTARTDATE), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, request.P_DEXPIRDAT != null ? Convert.ToDateTime(request.P_DEXPIRDAT) : Convert.ToDateTime(request.P_DSTARTDATE), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTIP_RENOV", OracleDbType.Int32, request.P_NTIP_RENOV == 0 ? null : request.P_NTIP_RENOV, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPAYFREQ", OracleDbType.Int32, request.P_NPAYFREQ == 0 ? null : request.P_NPAYFREQ, ParameterDirection.Input));

                parameter.Add(new OracleParameter("P_NTIP_NCOMISSION", OracleDbType.Int32, request.P_NTIP_NCOMISSION == 0 ? null : request.P_NTIP_NCOMISSION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOD_ACTIVITY_TEC", OracleDbType.Varchar2, request.P_SCOD_ACTIVITY_TEC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOD_CIUU", OracleDbType.Varchar2, request.P_SCOD_CIUU, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, request.P_NUSERCODE == 0 ? 0 : request.P_NUSERCODE, ParameterDirection.Input));

                parameter.Add(new OracleParameter("P_DSTARTDATE_ASE", OracleDbType.Date, Convert.ToDateTime(request.P_DSTARTDATE_ASE), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEXPIRDAT_ASE", OracleDbType.Date, Convert.ToDateTime(request.P_DEXPIRDAT_ASE), ParameterDirection.Input));

                parameter.Add(new OracleParameter("P_NREM_EXC", OracleDbType.Double, request.P_NREM_EXC, ParameterDirection.Input)); //rq Exc EH

                parameter.Add(new OracleParameter("P_NIDPLAN", OracleDbType.Double, request.planId, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, result.P_MESSAGE, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, result.P_COD_ERR, ParameterDirection.Output);

                P_MESSAGE.Size = 9000;
                P_COD_ERR.Size = 9000;

                parameter.Add(P_MESSAGE);
                parameter.Add(P_COD_ERR);

                parameter.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Int32, request.P_SPOL_ESTADO == 1 ? "1" : new string[] { "IN", "RE", "DE", "EX", "EN" }.Contains(request.TrxCode ?? "") ? "1" : "0", ParameterDirection.Input)); // ENDOSO TECNICA JTV 12012023
                parameter.Add(new OracleParameter("P_SCOMMENT", OracleDbType.Varchar2, request.P_SCOMMENT, ParameterDirection.Input)); //recotizacion
                parameter.Add(new OracleParameter("P_SRUTA", OracleDbType.Varchar2, request.P_SRUTA, ParameterDirection.Input)); //recotizacion
                parameter.Add(new OracleParameter("P_NACT_MINA", OracleDbType.Int32, request.P_NACT_MINA, ParameterDirection.Input)); //recotizacion
                parameter.Add(new OracleParameter("P_SPOL_ESTADO", OracleDbType.Int32, request.P_SPOL_ESTADO, ParameterDirection.Input)); //  Tramite de estado VL
                parameter.Add(new OracleParameter("P_NTYPE_ENDOSO", OracleDbType.Int32, request.TipoEndoso, ParameterDirection.Input)); // -- ENDOSO TECNICA JTV 02022023

                this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, connection, trx);

                result.P_MESSAGE = P_MESSAGE.Value.ToString();
                result.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
            }
            catch (Exception ex)
            {
                result.P_COD_ERR = 1;
                result.P_MESSAGE = ex.ToString();
            }

            return result;
        }

        public QuotationResponseVM UpdateReQuotation(InfoPropBM request)
        {
            var sPackageName = ProcedureName.pkg_Cotizacion + ".INSUPD_RECOT_INCL_VL";

            List<OracleParameter> parameter = new List<OracleParameter>();
            QuotationResponseVM result = new QuotationResponseVM();
            DbConnection DataConnection = ConnectionGet(enuTypeDataBase.OracleVTime);
            DbTransaction trx = null;
            try
            {
                List<CategoryRateBM> listCategoryRate = new List<CategoryRateBM>();
                foreach (var item2 in request.ProposalList)
                {
                    listCategoryRate.Add(new CategoryRateBM() { NMODULEC = item2.CategoryCode, SDESCRIPT = item2.CategoryProposal, P_NTASA_PR = item2.RateProposal });
                }
                string json = JsonConvert.SerializeObject(listCategoryRate, Formatting.None).ToString();
                DataConnection.Open();
                trx = DataConnection.BeginTransaction();

                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, request.QuotationNumber, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_JTASAS", OracleDbType.Varchar2, json, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Double, request.UserCode, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, request.Message, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, result.P_MESSAGE, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, result.P_COD_ERR, ParameterDirection.Output);

                P_MESSAGE.Size = 9000;
                P_COD_ERR.Size = 9000;

                parameter.Add(P_MESSAGE);
                parameter.Add(P_COD_ERR);

                this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, DataConnection, trx);

                result.P_MESSAGE = P_MESSAGE.Value.ToString();
                result.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
            }
            catch (Exception ex)
            {
                result.P_COD_ERR = 1;
                result.P_MESSAGE = ex.ToString();
            }
            finally
            {

                if (result.P_COD_ERR == 0)
                {
                    trx.Commit();
                }
                else
                {
                    if (trx.Connection != null) trx.Rollback();
                }
                if (trx.Connection != null) trx.Dispose();
                ELog.CloseConnection(DataConnection);
            }
            return result;
        }

        public string GetProcessCode(int numeroCotizacion)
        {
            var sPackageName = ProcedureName.pkg_Cotizacion + ".REA_COD_PROCESO_VALIDA";
            string result = "0";
            List<OracleParameter> parameters = new List<OracleParameter>();
            try
            {
                parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, numeroCotizacion, ParameterDirection.Input));
                OracleParameter P_NID_PROC = new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, result, ParameterDirection.Output);

                P_NID_PROC.Size = 100;

                parameters.Add(P_NID_PROC);
                this.ExecuteByStoredProcedureVT(sPackageName, parameters);
                result = P_NID_PROC.Value.ToString();
            }
            catch (Exception ex)
            {
                LogControl.save("GetProcessCode", ex.ToString(), "3");
            }
            return result;
        }

        public SalidaTramaBaseVM readInfoTramaDetailWeb(InfoClientBM infoClient, ref SalidaTramaBaseVM obj)
        {
            //SalidaTramaBaseVM response = new SalidaTramaBaseVM();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.sp_DetalleMontosCotWeb;

            try
            {
                parameter.Add(new OracleParameter("P_FLAG_COT", OracleDbType.Varchar2, infoClient.FlagCotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUM_TRABAJADORES_OBR", OracleDbType.Varchar2, infoClient.NumTrabajadoresObr, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUM_TRABAJADORES_EMP", OracleDbType.Varchar2, infoClient.NumTrabajadoresEmp, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_MONTO_PLANILLA_OBR", OracleDbType.Varchar2, infoClient.MontoPlanillaObr, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_MONTO_PLANILLA_EMP", OracleDbType.Varchar2, infoClient.MontoPlanillaEmp, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Double, infoClient.TipoTransaccion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_COMMISSION", OracleDbType.Double, infoClient.TipoComision, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Double, infoClient.NumeroCotizacion, ParameterDirection.Input));

                //OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, ParameterDirection.Output);
                string[] arrayCursor = { "C_AMOUNT_PRIMA", "C_AMOUNT_DET_TOTAL" };

                OracleParameter C_AMOUNT_PRIMA = new OracleParameter(arrayCursor[0], OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter C_AMOUNT_DET_TOTAL = new OracleParameter(arrayCursor[1], OracleDbType.RefCursor, ParameterDirection.Output);

                parameter.Add(C_AMOUNT_PRIMA);
                parameter.Add(C_AMOUNT_DET_TOTAL);

                OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storeprocedure, arrayCursor, parameter);

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        AmountPremiumQuotationVM item = new AmountPremiumQuotationVM();
                        item.SCATEGORIA = dr["SCATEGORIA"] == DBNull.Value ? "" : dr["SCATEGORIA"].ToString();
                        item.NTASA = dr["NTASA"] == DBNull.Value ? 0.0 : double.Parse(dr["NTASA"].ToString());
                        item.NPREMIUMN_MEN = dr["NPREMIUMN_MEN"] == DBNull.Value ? 0.0 : double.Parse(dr["NPREMIUMN_MEN"].ToString());
                        item.NPREMIUMN_BIM = dr["NPREMIUMN_BIM"] == DBNull.Value ? 0.0 : double.Parse(dr["NPREMIUMN_BIM"].ToString());
                        item.NPREMIUMN_TRI = dr["NPREMIUMN_TRI"] == DBNull.Value ? 0.0 : double.Parse(dr["NPREMIUMN_TRI"].ToString());
                        item.NPREMIUMN_SEM = dr["NPREMIUMN_SEM"] == DBNull.Value ? 0.0 : double.Parse(dr["NPREMIUMN_SEM"].ToString());
                        item.NPREMIUMN_ANU = dr["NPREMIUMN_ANU"] == DBNull.Value ? 0.0 : double.Parse(dr["NPREMIUMN_ANU"].ToString());
                        obj.amountPremiumList.Add(item);
                    }
                    dr.NextResult();
                    while (dr.Read())
                    {
                        AmountDetailTotalQuotationVM item = new AmountDetailTotalQuotationVM();
                        item.SDESCRIPCION = dr["SDESCRIPCION"] == DBNull.Value ? "" : dr["SDESCRIPCION"].ToString();
                        item.NAMOUNT_MEN = dr["NAMOUNT_MEN"] == DBNull.Value ? 0.0 : double.Parse(dr["NAMOUNT_MEN"].ToString());
                        item.NAMOUNT_BIM = dr["NAMOUNT_BIM"] == DBNull.Value ? 0.0 : double.Parse(dr["NAMOUNT_BIM"].ToString());
                        item.NAMOUNT_TRI = dr["NAMOUNT_TRI"] == DBNull.Value ? 0.0 : double.Parse(dr["NAMOUNT_TRI"].ToString());
                        item.NAMOUNT_SEM = dr["NAMOUNT_SEM"] == DBNull.Value ? 0.0 : double.Parse(dr["NAMOUNT_SEM"].ToString());
                        item.NAMOUNT_ANU = dr["NAMOUNT_ANU"] == DBNull.Value ? 0.0 : double.Parse(dr["NAMOUNT_ANU"].ToString());
                        obj.amountDetailTotalList.Add(item);
                    }
                }

                ELog.CloseConnection(dr);
            }
            catch (Exception ex)
            {
                LogControl.save("readInfoTramaDetailWeb", ex.ToString(), "3");
            }

            return obj;
        }

        public SalidaTramaBaseVM readInfoTramaWeb(InfoClientBM infoClient, ref SalidaTramaBaseVM obj)
        {
            //SalidaTramaBaseVM response = new SalidaTramaBaseVM();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.sp_DetalleCotizacionWeb;

            OracleDataReader dr = null;
            try
            {
                parameter.Add(new OracleParameter("P_NUM_TRABAJADORES_OBR", OracleDbType.Varchar2, infoClient.NumTrabajadoresObr, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUM_TRABAJADORES_EMP", OracleDbType.Varchar2, infoClient.NumTrabajadoresEmp, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_MONTO_PLANILLA_OBR", OracleDbType.Varchar2, infoClient.MontoPlanillaObr, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_MONTO_PLANILLA_EMP", OracleDbType.Varchar2, infoClient.MontoPlanillaEmp, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Double, infoClient.TipoTransaccion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_COMMISSION", OracleDbType.Double, infoClient.TipoComision, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Double, infoClient.NumeroCotizacion, ParameterDirection.Input));

                string[] arrayCursor = { "C_CATEGORIA", "C_TASA_X_PLAN", "C_DET_PLAN" };
                OracleParameter C_CATEGORIA = new OracleParameter(arrayCursor[0], OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter C_TASA_X_PLAN = new OracleParameter(arrayCursor[1], OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter C_DET_PLAN = new OracleParameter(arrayCursor[2], OracleDbType.RefCursor, ParameterDirection.Output);

                parameter.Add(C_CATEGORIA);
                parameter.Add(C_TASA_X_PLAN);
                parameter.Add(C_DET_PLAN);

                dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storeprocedure, arrayCursor, parameter);

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        CategoryQuotationVM item = new CategoryQuotationVM();
                        item.SCATEGORIA = dr["SCATEGORIA"] == DBNull.Value ? "" : dr["SCATEGORIA"].ToString();
                        item.NCOUNT = dr["NCOUNT"] == DBNull.Value ? 0 : int.Parse(dr["NCOUNT"].ToString());
                        item.NTOTAL_PLANILLA = dr["NTOTAL_PLANILLA"] == DBNull.Value ? 0.0 : double.Parse(dr["NTOTAL_PLANILLA"].ToString());
                        item.NTASA = dr["NTASA"] == DBNull.Value ? 0.0 : double.Parse(dr["NTASA"].ToString());
                        obj.categoryList.Add(item);
                    }

                    dr.NextResult();

                    while (dr.Read())
                    {
                        RateXPlanQuotationVM item = new RateXPlanQuotationVM();
                        item.NTASA = dr["NTASA"] == DBNull.Value ? 0.0 : double.Parse(dr["NTASA"].ToString());
                        obj.rateByPlanList.Add(item);
                    }
                    dr.NextResult();

                    while (dr.Read())
                    {
                        DetailPlanQuotationVM item = new DetailPlanQuotationVM();
                        item.PRIMA_MINIMA = dr["PRIMA_MINIMA"] == DBNull.Value ? 0.0 : double.Parse(dr["PRIMA_MINIMA"].ToString());
                        item.DET_PLAN = dr["DET_PLAN"] == DBNull.Value ? "" : dr["DET_PLAN"].ToString();
                        obj.detailPlanList.Add(item);
                    }
                }

                ELog.CloseConnection(dr);

            }
            catch (OracleException ex)
            {
                ELog.CloseConnection(dr);
                throw ex;
            }

            return obj;
        }

        public List<BandejaVM> GetBandejaList(BandejaBM bandejaBM)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<BandejaVM> BandejaList = new List<BandejaVM>();
            string storedProcedureName = ProcedureName.pkg_Cotizacion + ".REA_BANDEJA";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, bandejaBM.NID_COTIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, bandejaBM.NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, bandejaBM.NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FECHA_DESDE", OracleDbType.Date, bandejaBM.DDESDE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FECHA_HASTA", OracleDbType.Date, bandejaBM.DHASTA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SSTATREGT", OracleDbType.Varchar2, bandejaBM.SSTATREGT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, bandejaBM.NUSERCODE, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        BandejaVM item = new BandejaVM();
                        item.NUM_COTIZACION = odr["NUM_COTIZACION"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NUM_COTIZACION"].ToString());
                        item.NOMBRE_PRODUCT = odr["NOMBRE_PRODUCT"] == DBNull.Value ? string.Empty : odr["NOMBRE_PRODUCT"].ToString();
                        item.ID_PRODUCTO = odr["ID_PRODUCTO"] == DBNull.Value ? 0 : Convert.ToInt32(odr["ID_PRODUCTO"].ToString());
                        item.NOMBRE_CONTRATANTE = odr["NOMBRE_CONTRATANTE"] == DBNull.Value ? string.Empty : odr["NOMBRE_CONTRATANTE"].ToString();
                        item.CLIENT_BROKER = odr["CLIENT_BROKER"] == DBNull.Value ? string.Empty : odr["CLIENT_BROKER"].ToString();
                        item.PRIMA_MINIMA = odr["PRIMA_MINIMA"] == DBNull.Value ? 0 : double.Parse(odr["PRIMA_MINIMA"].ToString());
                        item.NOM_TOTAL_TRAB = odr["NOM_TOTAL_TRAB"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NOM_TOTAL_TRAB"].ToString());
                        item.PLANILLA = odr["PLANILLA"] == DBNull.Value ? 0 : double.Parse(odr["PLANILLA"].ToString());
                        item.TASA = odr["TASA"] == DBNull.Value ? "" : odr["TASA"].ToString();
                        item.COMISION = odr["COMISION"] == DBNull.Value ? 0 : double.Parse(odr["COMISION"].ToString());
                        item.DES_ESTADO = odr["DES_ESTADO"] == DBNull.Value ? string.Empty : odr["DES_ESTADO"].ToString();
                        item.FECHA_APROBACION = odr["FECHA_APROBACION"] == DBNull.Value ? "" : odr["FECHA_APROBACION"].ToString();
                        item.MODO = odr["MODO"] == DBNull.Value ? string.Empty : odr["MODO"].ToString();
                        item.POLIZA = odr["poliza"] == DBNull.Value ? 0 : Convert.ToInt64(odr["poliza"]);
                        item.STRANSAC = odr["stransac"] == DBNull.Value ? string.Empty : odr["stransac"].ToString();
                        item.NCLIENT_SEG = odr["NCLIENT_SEG"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NCLIENT_SEG"]);
                        item.SCLIENT_SEG = odr["SCLIENT_SEG"] == DBNull.Value ? string.Empty : odr["SCLIENT_SEG"].ToString();
                        item.NTIEMPO_TOTAL_SLA = odr["NTIEMPO_TOTAL_SLA"] == DBNull.Value ? "" : odr["NTIEMPO_TOTAL_SLA"].ToString();
                        item.SDESCRIPT_SEG = odr["SDESCRIPT_SEG"] == DBNull.Value ? string.Empty : odr["SDESCRIPT_SEG"].ToString();
                        BandejaList.Add(item);
                    }
                }
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                LogControl.save("GetBandejaList", ex.ToString(), "3");
            }

            return BandejaList;
        }

        public GenericResponseVM ChangeStatusVL(StatusChangeBM data, List<HttpPostedFile> fileList, OracleTransaction externalTransaction)
        {
            GenericResponseVM resultPackage = new GenericResponseVM();
            resultPackage.ErrorMessageList = new List<string>();
            resultPackage.SuccessMessageList = new List<string>();
            var connectionString = ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString();

            string folderPath = ELog.obtainConfig("pathCotizacion") + data.QuotationNumber + "\\" + ELog.obtainConfig("movimiento") + "\\" + DateTime.Now.ToString("yyyyMMddhhmmss") + "\\";
            try
            {
                using (var connection = new OracleConnection(connectionString))
                {
                    connection.Open();
                    var transaction = externalTransaction;

                    if (transaction == null) transaction = connection.BeginTransaction();
                    /*string storeActualiza = ProcedureName.pkg_Cotizacion + ".UPD_COTIZACION";   //Nombre de stored procedure para insertar "tasa autorizada"

                    DbCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = storeActualiza;
                    command.Transaction = transaction;

                    command.Parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.QuotationNumber, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("P_ESTADO", OracleDbType.Varchar2, data.Status, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Varchar2, data.User, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("P_NMOTIVO", OracleDbType.Int32, null, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("P_SCOMMENT", OracleDbType.Varchar2, data.Comment, ParameterDirection.Input));

                    command.Parameters.Add(new OracleParameter("P_COD_ESTADO", OracleDbType.Int32, 4000, null, ParameterDirection.Output));
                    command.Parameters.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output));

                    command.ExecuteNonQuery();

                    if (Convert.ToInt32(command.Parameters["P_COD_ESTADO"].Value.ToString()) != 0)
                    {
                        resultPackage.StatusCode = 3;
                        transaction.Rollback();
                        connection.Close();
                        connection.Dispose();
                        return resultPackage;
                    }
                    else
                    {*/

                    // insertarHistorial
                    string storeActualizaRA = ProcedureName.pkg_Cotizacion + ".UPD_COTIZACION_RIESGO_ALTO";   //Nombre de stored procedure para insertar "tasa autorizada"

                    DbCommand commandRA = connection.CreateCommand();
                    commandRA.CommandType = CommandType.StoredProcedure;
                    commandRA.CommandText = storeActualizaRA;
                    commandRA.Transaction = transaction;

                    commandRA.Parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.QuotationNumber, ParameterDirection.Input));

                    if (data.Flag == 1)
                        commandRA.Parameters.Add(new OracleParameter("P_ESTADO", OracleDbType.Int32, 0, ParameterDirection.Input));
                    else
                        commandRA.Parameters.Add(new OracleParameter("P_ESTADO", OracleDbType.Int32, data.Status, ParameterDirection.Input));

                    if (data.Reason != 0) commandRA.Parameters.Add(new OracleParameter("P_NMOTIVO", OracleDbType.Int32, data.Reason, ParameterDirection.Input));
                    else commandRA.Parameters.Add(new OracleParameter("P_NMOTIVO", OracleDbType.Int32, null, ParameterDirection.Input));

                    commandRA.Parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, data.User, ParameterDirection.Input));
                    commandRA.Parameters.Add(new OracleParameter("P_SCOMMENT", OracleDbType.Varchar2, data.Comment, ParameterDirection.Input));
                    commandRA.Parameters.Add(new OracleParameter("P_SRUTA", OracleDbType.Varchar2, folderPath, ParameterDirection.Input));

                    commandRA.Parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, data.Product, ParameterDirection.Input));
                    commandRA.Parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.Nbranch, ParameterDirection.Input));
                    commandRA.Parameters.Add(new OracleParameter("P_NFLAG", OracleDbType.Int32, data.Flag, ParameterDirection.Input));

                    commandRA.Parameters.Add(new OracleParameter("P_COD_ESTADO", OracleDbType.Int32, 4000, null, ParameterDirection.Output));
                    commandRA.Parameters.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output));

                    commandRA.Parameters.Add(new OracleParameter("P_FLAGPAGO_CIP", OracleDbType.Int32, data.FlagCIP, ParameterDirection.Input));
                    commandRA.Parameters.Add(new OracleParameter("P_SCOMMENT2", OracleDbType.Varchar2, data.Comment2, ParameterDirection.Input));

                    commandRA.ExecuteNonQuery();

                    if (Convert.ToInt32(commandRA.Parameters["P_COD_ESTADO"].Value.ToString()) != 0)
                    {
                        resultPackage.StatusCode = 1;
                        resultPackage.MessageList = new List<string>();
                        resultPackage.MessageList.Add(commandRA.Parameters["P_MENSAJE"].Value.ToString());
                        transaction.Rollback();
                        transaction.Dispose();
                        connection.Close();
                        connection.Dispose();
                        return resultPackage;
                    }
                    else
                    {
                        /*
                        string subProcedureName = ProcedureName.pkg_Cotizacion + ".UPD_COTIZACION_DET";   //Nombre de stored procedure para insertar "tasa autorizada"

                        foreach (AuthorizedRateBM subData in data.saludAuthorizedRateList)
                        {
                            DbCommand subCommand = connection.CreateCommand();
                            subCommand.CommandType = CommandType.StoredProcedure;
                            subCommand.CommandText = subProcedureName;
                            subCommand.Transaction = transaction;

                            subCommand.Parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.QuotationNumber, ParameterDirection.Input));
                            subCommand.Parameters.Add(new OracleParameter("P_ESTADO", OracleDbType.Int32, data.Status, ParameterDirection.Input));
                            subCommand.Parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, subData.ProductId, ParameterDirection.Input));
                            subCommand.Parameters.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, subData.RiskTypeId, ParameterDirection.Input));
                            subCommand.Parameters.Add(new OracleParameter("P_NTASA_AUTO", OracleDbType.Double, subData.AuthorizedRate, ParameterDirection.Input));
                            subCommand.Parameters.Add(new OracleParameter("P_NPREMIUM_MEN_AUT", OracleDbType.Double, subData.AuthorizedPremium, ParameterDirection.Input));
                            subCommand.Parameters.Add(new OracleParameter("P_MIN_PREMIUM_AUT", OracleDbType.Double, subData.AuthorizedMinimunPremium, ParameterDirection.Input));
                            subCommand.Parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, data.User, ParameterDirection.Input));

                            subCommand.Parameters.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output));
                            subCommand.Parameters.Add(new OracleParameter("P_COD_ESTADO", OracleDbType.Int32, 4000, null, ParameterDirection.Output));

                            subCommand.Parameters.Add(new OracleParameter("P_NPREMIUM_TOT", OracleDbType.Double, null, ParameterDirection.Input));
                            subCommand.Parameters.Add(new OracleParameter("P_NTASA_TOT", OracleDbType.Double, null, ParameterDirection.Input));

                            subCommand.ExecuteNonQuery();
                            if (Convert.ToInt32(subCommand.Parameters["P_COD_ESTADO"].Value.ToString()) != 0)
                            {
                                resultPackage.StatusCode = 3;
                                transaction.Rollback();
                                connection.Close();
                                connection.Dispose();
                                return resultPackage;
                            }
                        }*/

                        var lBroker = new string[] { ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch"),
                                                     ELog.obtainConfig("vidaIndividualBranch"), ELog.obtainConfig("desgravamenBranch") };
                        if (lBroker.Contains(data.Nbranch.ToString()))
                        {
                            resultPackage = updateBroker(transaction, connection, data);

                            if (resultPackage.ErrorCode != 0)
                            {
                                transaction.Rollback();
                                transaction.Dispose();
                                connection.Close();
                                connection.Dispose();
                            }
                        }
                    }

                    if (fileList != null && fileList.Count > 0)
                    {
                        if (!Directory.Exists(String.Format(ELog.obtainConfig("pathPrincipal"), folderPath)))
                        {
                            Directory.CreateDirectory(String.Format(ELog.obtainConfig("pathPrincipal"), folderPath));
                        }

                        foreach (var file in fileList)
                        {
                            file.SaveAs(String.Format(ELog.obtainConfig("pathPrincipal"), folderPath) + file.FileName);
                        }
                    }
                    resultPackage.StatusCode = 0;

                    if (externalTransaction == null)
                    {
                        transaction.Commit();
                        transaction.Dispose();
                        connection.Close();
                        connection.Dispose();
                    }

                    return resultPackage;
                }
            }
            catch (Exception ex)
            {
                LogControl.save("ChangeStatusVL", ex.ToString(), "3");
                resultPackage.StatusCode = 3;
                resultPackage.MessageError = ex.ToString();
                return resultPackage;
            }
        }

        private GenericResponseVM updateBroker(OracleTransaction transaction, OracleConnection connection, StatusChangeBM data)
        {
            var result = new GenericResponseVM();

            try
            {
                if (data != null)
                {
                    foreach (BrokerBM broker in data.brokerList)
                    {
                        foreach (BrokerProductBM product in broker.ProductList)
                        {
                            DbCommand subCommand = connection.CreateCommand();
                            subCommand.CommandType = CommandType.StoredProcedure;
                            subCommand.CommandText = ProcedureName.pkg_Cotizacion + ".UPD_COTIZACION_COMER";
                            subCommand.Transaction = transaction;

                            subCommand.Parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.QuotationNumber, ParameterDirection.Input));
                            subCommand.Parameters.Add(new OracleParameter("P_CHANNEL", OracleDbType.Int32, broker.Id, ParameterDirection.Input));
                            subCommand.Parameters.Add(new OracleParameter("P_PRODUCT", OracleDbType.Int32, product.Product, ParameterDirection.Input));
                            subCommand.Parameters.Add(new OracleParameter("P_COMISION_AUT", OracleDbType.Double, product.AuthorizedCommission, ParameterDirection.Input));
                            subCommand.Parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, data.User, ParameterDirection.Input));

                            subCommand.Parameters.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output));
                            subCommand.Parameters.Add(new OracleParameter("P_COD_ESTADO", OracleDbType.Int32, 4000, null, ParameterDirection.Output));

                            subCommand.ExecuteNonQuery();

                            result.ErrorCode = Convert.ToInt32(subCommand.Parameters["P_COD_ESTADO"].Value.ToString());

                            if (result.ErrorCode != 0)
                            {
                                result.StatusCode = 3;
                                result.MessageError = subCommand.Parameters["P_MENSAJE"].Value.ToString();
                            }
                        }
                    }
                }
                else
                {
                    result.StatusCode = 3;
                    result.MessageError = "No se envió la informacion necesaria";
                }
            }
            catch (Exception ex)
            {
                result.StatusCode = 3;
                result.MessageError = ex.ToString();
            }

            return result;
        }

        public CreditHistoryVM GetConfigCreditHistory(ConsultVM consultVM)
        {
            var sPackageName = ProcedureName.pkg_Cotizacion + ".REA_POINTS_CLIENT";

            List<OracleParameter> parameter = new List<OracleParameter>();
            CreditHistoryVM result = new CreditHistoryVM();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, consultVM.sclient, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCORE_CLIENT", OracleDbType.Double, double.Parse(consultVM.score), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, consultVM.usercode, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_RISKTYPE = new OracleParameter("P_NTYPERISK", OracleDbType.Int32, result.nriskType, ParameterDirection.Output);
                OracleParameter P_SDESCRIPT = new OracleParameter("P_SDESCRIPT", OracleDbType.Varchar2, result.sdescript, ParameterDirection.Output);
                OracleParameter P_FLAG = new OracleParameter("P_NFLAG_CRE", OracleDbType.Int32, result.nflag, ParameterDirection.Output);

                P_RISKTYPE.Size = 9000;
                P_SDESCRIPT.Size = 9000;
                P_FLAG.Size = 2000;

                parameter.Add(P_RISKTYPE);
                parameter.Add(P_SDESCRIPT);
                parameter.Add(P_FLAG);

                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, consultVM.nbranch, ParameterDirection.Input)); //AVS - INTERCONEXION SABSA 02/11/2023

                this.ExecuteByStoredProcedureVT(sPackageName, parameter);

                result.nriskType = int.Parse(P_RISKTYPE.Value.ToString());
                result.sdescript = P_SDESCRIPT.Value.ToString();
                result.nflag = int.Parse(P_FLAG.Value.ToString());
            }
            catch (Exception ex)
            {
                LogControl.save("GetConfigCreditHistory", ex.ToString(), "3");
            }
            return result;
        }

        public QuotationResponseVM UpdateCodQuotation(ValidaTramaBM request)
        {
            var sPackageName = ProcedureName.pkg_Cotizacion + ".UPD_COTIZACION_NID";

            List<OracleParameter> parameter = new List<OracleParameter>();
            QuotationResponseVM result = new QuotationResponseVM();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, request.NID_COTIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.NID_PROC, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, result.P_MESSAGE, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, result.P_COD_ERR, ParameterDirection.Output);

                P_MESSAGE.Size = 9000;
                P_COD_ERR.Size = 9000;

                parameter.Add(P_MESSAGE);
                parameter.Add(P_COD_ERR);

                this.ExecuteByStoredProcedureVT(sPackageName, parameter);

                result.P_MESSAGE = P_MESSAGE.Value.ToString();
                result.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
            }
            catch (Exception ex)
            {
                result.P_COD_ERR = 1;
                result.P_MESSAGE = ex.ToString();
            }
            return result;
        }

        public QuotationResponseVM UpdateCodQuotation(int codigoCotizacion, string codigoProceso, DbConnection connection = null, DbTransaction trx = null)
        {
            var sPackageName = ProcedureName.pkg_Cotizacion + ".UPD_COTIZACION_NID";

            List<OracleParameter> parameter = new List<OracleParameter>();
            QuotationResponseVM result = new QuotationResponseVM();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, codigoCotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, codigoProceso, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, result.P_MESSAGE, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, result.P_COD_ERR, ParameterDirection.Output);

                P_MESSAGE.Size = 9000;
                P_COD_ERR.Size = 9000;

                parameter.Add(P_MESSAGE);
                parameter.Add(P_COD_ERR);

                if (connection == null)
                {
                    this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                    result.P_MESSAGE = P_MESSAGE.Value.ToString();
                    result.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                }
                else
                {
                    this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, connection, trx);
                    result.P_MESSAGE = P_MESSAGE.Value.ToString();
                    result.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                }
            }
            catch (Exception ex)
            {
                result.P_COD_ERR = 1;
                result.P_MESSAGE = ex.ToString();
            }
            return result;
        }

        public QuotationResponseVM UpdateCodQuotationPD(string cotizacion, QuotationCabBM request, DbConnection connection, DbTransaction trx)
        {
            var sPackageName = ProcedureName.pkg_Cotizacion + ".UPD_COTIZACION_NID_PD";

            List<OracleParameter> parameter = new List<OracleParameter>();
            QuotationResponseVM result = new QuotationResponseVM();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, cotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.CodigoProceso, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Varchar2, request.P_NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Varchar2, request.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NACT_MINA", OracleDbType.Varchar2, request.P_NACT_MINA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOD_ACTIVITY_TEC", OracleDbType.Varchar2, request.P_SCOD_ACTIVITY_TEC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPOLIZA_MATRIZ", OracleDbType.Varchar2, request.P_NPOLIZA_MATRIZ, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCOMISION_SAL_PR", OracleDbType.Varchar2, request.P_NCOMISION_SAL_PR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Varchar2, request.P_NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_PRODUCT", OracleDbType.Varchar2, request.P_NTYPE_PRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_PROFILE", OracleDbType.Varchar2, request.P_NTYPE_PROFILE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDPLAN", OracleDbType.Varchar2, request.planId, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Varchar2, request.P_DSTARTDATE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCAPITAL_PRO", OracleDbType.Varchar2, request.P_NCAPITAL_PRO, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, result.P_MESSAGE, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, result.P_COD_ERR, ParameterDirection.Output);

                P_MESSAGE.Size = 9000;
                P_COD_ERR.Size = 9000;

                parameter.Add(P_MESSAGE);
                parameter.Add(P_COD_ERR);

                this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, connection, trx);

                result.P_MESSAGE = P_MESSAGE.Value.ToString();
                result.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
            }
            catch (Exception ex)
            {
                result.P_COD_ERR = 1;
                result.P_MESSAGE = ex.ToString();
            }
            return result;
        }

        //public SalidaTramaBaseVM readInfoTramaDetail(ValidaTramaBM validaTramaBM, ref SalidaTramaBaseVM obj)
        //{
        //    //SalidaTramaBaseVM response = new SalidaTramaBaseVM();
        //    List<OracleParameter> parameter = new List<OracleParameter>();
        //    string storeprocedure = ProcedureName.sp_DetalleMontosCot;

        //    try
        //    {
        //        string json = JsonConvert.SerializeObject(validaTramaBM.P_JTASAS, Formatting.None).ToString();
        //        parameter.Add(new OracleParameter("P_FLAG_COT", OracleDbType.Varchar2, validaTramaBM.SFLAGCOT, ParameterDirection.Input));
        //        parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, validaTramaBM.NID_PROC, ParameterDirection.Input));
        //        parameter.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Varchar2, validaTramaBM.NTYPE_TRANSAC, ParameterDirection.Input));
        //        parameter.Add(new OracleParameter("P_NTYPE_COMMISSION", OracleDbType.Int32, validaTramaBM.NTYPE_COMMISSION, ParameterDirection.Input));
        //        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, validaTramaBM.NID_COTIZACION, ParameterDirection.Input));

        //        parameter.Add(new OracleParameter("P_FLAG_PR", OracleDbType.Int32, validaTramaBM.P_FLAG_PR, ParameterDirection.Input));
        //        parameter.Add(new OracleParameter("P_COMISSION_PR", OracleDbType.Double, validaTramaBM.P_COMISSION_PR, ParameterDirection.Input));
        //        /*parameter.Add(new OracleParameter("P_TASA_OB", OracleDbType.Double, validaTramaBM.P_TASA_OB, ParameterDirection.Input));
        //        parameter.Add(new OracleParameter("P_TASA_EM", OracleDbType.Double, validaTramaBM.P_TASA_EM, ParameterDirection.Input));
        //        parameter.Add(new OracleParameter("P_TASA_OAR", OracleDbType.Double, validaTramaBM.P_TASA_OAR, ParameterDirection.Input));*/
        //        parameter.Add(new OracleParameter("P_JTASAS", OracleDbType.Varchar2, json, ParameterDirection.Input));
        //        parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Date, validaTramaBM.DEFFECDATE, ParameterDirection.Input));

        //        //OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, ParameterDirection.Output);
        //        string[] arrayCursor = { "C_AMOUNT_PRIMA", "C_AMOUNT_DET_TOTAL" };

        //        OracleParameter C_AMOUNT_PRIMA = new OracleParameter(arrayCursor[0], OracleDbType.RefCursor, ParameterDirection.Output);
        //        OracleParameter C_AMOUNT_DET_TOTAL = new OracleParameter(arrayCursor[1], OracleDbType.RefCursor, ParameterDirection.Output);

        //        parameter.Add(C_AMOUNT_PRIMA);
        //        parameter.Add(C_AMOUNT_DET_TOTAL);

        //        parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, validaTramaBM.P_NBRANCH, ParameterDirection.Input));
        //        parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, validaTramaBM.P_NPRODUCT, ParameterDirection.Input));

        //        OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storeprocedure, arrayCursor, parameter);

        //        if (dr.HasRows)
        //        {
        //            while (dr.Read())
        //            {
        //                AmountPremiumQuotationVM item = new AmountPremiumQuotationVM();
        //                item.SCATEGORIA = dr["SCATEGORIA"] == DBNull.Value ? "" : dr["SCATEGORIA"].ToString();
        //                item.NTASA = dr["NTASA"] == DBNull.Value ? 0.0 : double.Parse(dr["NTASA"].ToString());
        //                item.NPREMIUMN_MEN = dr["NPREMIUMN_MEN"] == DBNull.Value ? 0.0 : double.Parse(dr["NPREMIUMN_MEN"].ToString());
        //                item.NPREMIUMN_BIM = dr["NPREMIUMN_BIM"] == DBNull.Value ? 0.0 : double.Parse(dr["NPREMIUMN_BIM"].ToString());
        //                item.NPREMIUMN_TRI = dr["NPREMIUMN_TRI"] == DBNull.Value ? 0.0 : double.Parse(dr["NPREMIUMN_TRI"].ToString());
        //                item.NPREMIUMN_SEM = dr["NPREMIUMN_SEM"] == DBNull.Value ? 0.0 : double.Parse(dr["NPREMIUMN_SEM"].ToString());
        //                item.NPREMIUMN_ANU = dr["NPREMIUMN_ANU"] == DBNull.Value ? 0.0 : double.Parse(dr["NPREMIUMN_ANU"].ToString());
        //                obj.amountPremiumList.Add(item);
        //            }
        //            dr.NextResult();
        //            while (dr.Read())
        //            {
        //                AmountDetailTotalQuotationVM item = new AmountDetailTotalQuotationVM();
        //                item.SDESCRIPCION = dr["SDESCRIPCION"] == DBNull.Value ? "" : dr["SDESCRIPCION"].ToString();
        //                item.NAMOUNT_MEN = dr["NAMOUNT_MEN"] == DBNull.Value ? 0.0 : double.Parse(dr["NAMOUNT_MEN"].ToString());
        //                item.NAMOUNT_BIM = dr["NAMOUNT_BIM"] == DBNull.Value ? 0.0 : double.Parse(dr["NAMOUNT_BIM"].ToString());
        //                item.NAMOUNT_TRI = dr["NAMOUNT_TRI"] == DBNull.Value ? 0.0 : double.Parse(dr["NAMOUNT_TRI"].ToString());
        //                item.NAMOUNT_SEM = dr["NAMOUNT_SEM"] == DBNull.Value ? 0.0 : double.Parse(dr["NAMOUNT_SEM"].ToString());
        //                item.NAMOUNT_ANU = dr["NAMOUNT_ANU"] == DBNull.Value ? 0.0 : double.Parse(dr["NAMOUNT_ANU"].ToString());
        //                obj.amountDetailTotalList.Add(item);
        //            }
        //        }

        //        ELog.CloseConnection(dr);

        //    }
        //    catch (Exception ex)
        //    {
        //        ELog.save(this, ex.ToString());
        //    }

        //    return obj;
        //}

        public SalidaTramaBaseVM readInfoTramaDetail(ValidaTramaBM validaTramaBM, ref SalidaTramaBaseVM obj)
        {
            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        OracleDataReader dr;

                        cmd.Connection = cn;
                        cmd.CommandText = ProcedureName.sp_DetalleMontosCot;
                        cmd.CommandType = CommandType.StoredProcedure;

                        string json = JsonConvert.SerializeObject(validaTramaBM.P_JTASAS, Formatting.None).ToString();
                        cmd.Parameters.Add(new OracleParameter("P_FLAG_COT", OracleDbType.Varchar2, validaTramaBM.SFLAGCOT, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, validaTramaBM.NID_PROC, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Varchar2, validaTramaBM.NTYPE_TRANSAC, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NTYPE_COMMISSION", OracleDbType.Int32, validaTramaBM.NTYPE_COMMISSION, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NTARIFA_COMMISSION", OracleDbType.Int32, validaTramaBM.NTARIFA_COMMISSION, ParameterDirection.Input)); //NTARIFA_COMMISSION
                        cmd.Parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, validaTramaBM.NID_COTIZACION, ParameterDirection.Input));

                        cmd.Parameters.Add(new OracleParameter("P_FLAG_PR", OracleDbType.Int32, validaTramaBM.P_FLAG_PR, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_COMISSION_PR", OracleDbType.Double, validaTramaBM.P_COMISSION_PR, ParameterDirection.Input));
                        /*parameter.Add(new OracleParameter("P_TASA_OB", OracleDbType.Double, validaTramaBM.P_TASA_OB, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_TASA_EM", OracleDbType.Double, validaTramaBM.P_TASA_EM, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_TASA_OAR", OracleDbType.Double, validaTramaBM.P_TASA_OAR, ParameterDirection.Input));*/
                        cmd.Parameters.Add(new OracleParameter("P_JTASAS", OracleDbType.Varchar2, json == "null" ? null : json, ParameterDirection.Input));
                        //cmd.Parameters.Add(new OracleParameter("P_JTASAS", OracleDbType.Varchar2,   null , ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Date, validaTramaBM.DEFFECDATE, ParameterDirection.Input));

                        cmd.Parameters.Add(new OracleParameter("C_AMOUNT_PRIMA", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(new OracleParameter("C_AMOUNT_DET_TOTAL", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;

                        cmd.Parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, validaTramaBM.P_NBRANCH, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, validaTramaBM.P_NPRODUCT, ParameterDirection.Input));

                        cn.Open();
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        if (dr.HasRows)
                        {
                            string SCATE = "";
                            while (dr.Read())
                            {
                                var item = new AmountPremiumQuotationVM();

                                //GCAA
                                item.SCATEGORIA = dr["SCATEGORIA"] == DBNull.Value ? "" : dr["SCATEGORIA"].ToString();
                                //item.SCATEGORIA_view = (dr["SCATEGORIA"].ToString() == SCATE) ? "" : dr["SCATEGORIA"].ToString();//GCAA 28/09/2023
                                item.SRANGO_EDAD = dr["SEDADES"] == DBNull.Value ? "" : dr["SEDADES"].ToString();  //GCAA 28/09/2023
                                item.NTASA = dr["NTASA"] == DBNull.Value ? 0.0 : double.Parse(dr["NTASA"].ToString());
                                item.NPREMIUMN_MEN = dr["NPREMIUMN_MEN"] == DBNull.Value ? 0.0 : double.Parse(dr["NPREMIUMN_MEN"].ToString());
                                item.NPREMIUMN_BIM = dr["NPREMIUMN_BIM"] == DBNull.Value ? 0.0 : double.Parse(dr["NPREMIUMN_BIM"].ToString());
                                item.NPREMIUMN_TRI = dr["NPREMIUMN_TRI"] == DBNull.Value ? 0.0 : double.Parse(dr["NPREMIUMN_TRI"].ToString());
                                item.NPREMIUMN_SEM = dr["NPREMIUMN_SEM"] == DBNull.Value ? 0.0 : double.Parse(dr["NPREMIUMN_SEM"].ToString());
                                item.NPREMIUMN_ANU = dr["NPREMIUMN_ANU"] == DBNull.Value ? 0.0 : double.Parse(dr["NPREMIUMN_ANU"].ToString());
                                //SCATE = dr["SCATEGORIA"] == DBNull.Value ? "" : dr["SCATEGORIA"].ToString();
                                obj.amountPremiumList.Add(item);
                            }

                            dr.NextResult();

                            while (dr.Read())
                            {
                                var item = new AmountDetailTotalQuotationVM();
                                item.SDESCRIPCION = dr["SDESCRIPCION"] == DBNull.Value ? "" : dr["SDESCRIPCION"].ToString();
                                item.NAMOUNT_MEN = dr["NAMOUNT_MEN"] == DBNull.Value ? 0.0 : double.Parse(dr["NAMOUNT_MEN"].ToString());
                                item.NAMOUNT_BIM = dr["NAMOUNT_BIM"] == DBNull.Value ? 0.0 : double.Parse(dr["NAMOUNT_BIM"].ToString());
                                item.NAMOUNT_TRI = dr["NAMOUNT_TRI"] == DBNull.Value ? 0.0 : double.Parse(dr["NAMOUNT_TRI"].ToString());
                                item.NAMOUNT_SEM = dr["NAMOUNT_SEM"] == DBNull.Value ? 0.0 : double.Parse(dr["NAMOUNT_SEM"].ToString());
                                item.NAMOUNT_ANU = dr["NAMOUNT_ANU"] == DBNull.Value ? 0.0 : double.Parse(dr["NAMOUNT_ANU"].ToString());
                                obj.amountDetailTotalList.Add(item);
                            }
                        }

                        dr.Close();
                    }
                    catch (Exception ex)
                    {
                        LogControl.save("readInfoTramaDetail", ex.ToString(), "3");
                    }
                    finally
                    {
                        if (cn.State == ConnectionState.Open) cn.Close();
                    }
                }
            }

            return obj;
        }

        public async Task<ResponseCVM> GesCliente360(ConsultaCBM data, string asegurado)
        {
            var response = new ResponseCVM();

            try
            {
                string urlServicio = String.Format(ELog.obtainConfig("cliente360"), ELog.obtainConfig("consultarCliente" + asegurado));
                var json = await WebServiceUtil.invocarServicio(JsonConvert.SerializeObject(data), urlServicio);
                response = JsonConvert.DeserializeObject<ResponseCVM>(json, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            catch (Exception ex)
            {
                LogControl.save("GesCliente360", ex.ToString(), "3");
            }

            return await Task.FromResult(response);
        }


        public QuotationResponseVM insertHistTrama(ValidaTramaBM trama, DbConnection connection = null, DbTransaction trx = null)// modificado para transaccion
        {
            var sPackageName = ProcedureName.pkg_Cotizacion + ".INSUPD_COTIZA_HIS_TRAMA";

            List<OracleParameter> parameter = new List<OracleParameter>();
            QuotationResponseVM result = new QuotationResponseVM();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, trama.NID_COTIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Int32, trama.NTYPE_TRANSAC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, trama.NID_PROC, ParameterDirection.Input));

                if (connection == null)
                {
                    this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                }
                else
                {
                    this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, connection, trx);
                    result.P_MESSAGE = String.Empty;
                    result.P_COD_ERR = 0;
                }
            }
            catch (Exception ex)
            {
                result.P_COD_ERR = 1;
                result.P_MESSAGE = ex.ToString();
                LogControl.save("insertHistTrama", ex.ToString(), "3");
            }
            return result;
        }

        //public SalidaTramaBaseVM readInfoTrama(ValidaTramaBM validaTramaBM, ref SalidaTramaBaseVM obj)
        //{
        //    //SalidaTramaBaseVM response = new SalidaTramaBaseVM();
        //    List<OracleParameter> parameter = new List<OracleParameter>();
        //    string storeprocedure = ProcedureName.sp_DetalleCotizacion;

        //    OracleDataReader dr = null;
        //    try
        //    {
        //        string json = JsonConvert.SerializeObject(validaTramaBM.P_JTASAS, Formatting.None).ToString();
        //        parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, validaTramaBM.NID_PROC, ParameterDirection.Input));
        //        parameter.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Varchar2, validaTramaBM.NTYPE_TRANSAC, ParameterDirection.Input));
        //        parameter.Add(new OracleParameter("P_NTYPE_COMMISSION", OracleDbType.Int32, validaTramaBM.NTYPE_COMMISSION, ParameterDirection.Input));
        //        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, validaTramaBM.NID_COTIZACION, ParameterDirection.Input));
        //        parameter.Add(new OracleParameter("P_NPAYFREQ", OracleDbType.Int32, validaTramaBM.NPAYFREQ, ParameterDirection.Input));

        //        parameter.Add(new OracleParameter("P_FLAG_PR", OracleDbType.Int32, validaTramaBM.P_FLAG_PR, ParameterDirection.Input));
        //        parameter.Add(new OracleParameter("P_COMISSION_PR", OracleDbType.Double, validaTramaBM.P_COMISSION_PR, ParameterDirection.Input));
        //        //parameter.Add(new OracleParameter("P_TASA_OB", OracleDbType.Double, validaTramaBM.P_TASA_OB, ParameterDirection.Input));
        //        //parameter.Add(new OracleParameter("P_TASA_EM", OracleDbType.Double, validaTramaBM.P_TASA_EM, ParameterDirection.Input));
        //        parameter.Add(new OracleParameter("P_JTASAS", OracleDbType.Varchar2, json, ParameterDirection.Input));

        //        string[] arrayCursor = { "C_CATEGORIA", "C_TASA_X_PLAN", "C_DET_PLAN" };
        //        OracleParameter C_CATEGORIA = new OracleParameter(arrayCursor[0], OracleDbType.RefCursor, ParameterDirection.Output);
        //        OracleParameter C_TASA_X_PLAN = new OracleParameter(arrayCursor[1], OracleDbType.RefCursor, ParameterDirection.Output);
        //        OracleParameter C_DET_PLAN = new OracleParameter(arrayCursor[2], OracleDbType.RefCursor, ParameterDirection.Output);

        //        parameter.Add(C_CATEGORIA);
        //        parameter.Add(C_TASA_X_PLAN);
        //        parameter.Add(C_DET_PLAN);

        //        dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storeprocedure, arrayCursor, parameter);

        //        if (dr.HasRows)
        //        {
        //            while (dr.Read())
        //            {
        //                CategoryQuotationVM item = new CategoryQuotationVM();
        //                item.SCATEGORIA = dr["SCATEGORIA"] == DBNull.Value ? "" : dr["SCATEGORIA"].ToString();
        //                item.NCOUNT = dr["NCOUNT"] == DBNull.Value ? 0 : int.Parse(dr["NCOUNT"].ToString());
        //                item.NTOTAL_PLANILLA = dr["NTOTAL_PLANILLA"] == DBNull.Value ? 0.0 : double.Parse(dr["NTOTAL_PLANILLA"].ToString());
        //                item.NTASA = dr["NTASA"] == DBNull.Value ? 0.0 : double.Parse(dr["NTASA"].ToString());
        //                item.ProposalRate = dr["NTASA_PR"] == DBNull.Value ? 0.0 : double.Parse(dr["NTASA_PR"].ToString());
        //                item.ModuleCode = dr["NMODULEC"] == DBNull.Value ? 0 : int.Parse(dr["NMODULEC"].ToString());
        //                obj.categoryList.Add(item);
        //            }

        //            dr.NextResult();

        //            while (dr.Read())
        //            {
        //                RateXPlanQuotationVM item = new RateXPlanQuotationVM();
        //                item.NTASA = dr["NTASA"] == DBNull.Value ? 0.0 : double.Parse(dr["NTASA"].ToString());
        //                obj.rateByPlanList.Add(item);
        //            }
        //            dr.NextResult();

        //            while (dr.Read())
        //            {
        //                DetailPlanQuotationVM item = new DetailPlanQuotationVM();
        //                item.PRIMA_MINIMA = dr["PRIMA_MINIMA"] == DBNull.Value ? 0.0 : double.Parse(dr["PRIMA_MINIMA"].ToString());
        //                item.DET_PLAN = dr["DET_PLAN"] == DBNull.Value ? "" : dr["DET_PLAN"].ToString();
        //                obj.detailPlanList.Add(item);
        //            }
        //        }
        //        ELog.CloseConnection(dr);

        //    }
        //    catch (OracleException ex)
        //    {
        //        ELog.save(this, ex.ToString());
        //        ELog.CloseConnection(dr);
        //        throw ex;
        //    }

        //    return obj;
        //}

        public SalidaTramaBaseVM readInfoTrama(ValidaTramaBM validaTramaBM, ref SalidaTramaBaseVM obj)
        {
            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        OracleDataReader dr;

                        cmd.Connection = cn;
                        cmd.CommandText = ProcedureName.sp_DetalleCotizacion;
                        cmd.CommandType = CommandType.StoredProcedure;

                        string json = JsonConvert.SerializeObject(validaTramaBM.P_JTASAS, Formatting.None).ToString();
                        cmd.Parameters.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, validaTramaBM.NID_PROC, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Varchar2, validaTramaBM.NTYPE_TRANSAC, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NTYPE_COMMISSION", OracleDbType.Int32, validaTramaBM.NTYPE_COMMISSION, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NTARIFA_COMMISSION", OracleDbType.Int32, validaTramaBM.NTARIFA_COMMISSION, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, validaTramaBM.NID_COTIZACION, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NPAYFREQ", OracleDbType.Int32, validaTramaBM.NPAYFREQ, ParameterDirection.Input));

                        cmd.Parameters.Add(new OracleParameter("P_FLAG_PR", OracleDbType.Int32, validaTramaBM.P_FLAG_PR, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_COMISSION_PR", OracleDbType.Double, validaTramaBM.P_COMISSION_PR, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_JTASAS", OracleDbType.Varchar2, json == "null" ? null : json, ParameterDirection.Input));
                        //cmd.Parameters.Add(new OracleParameter("P_JTASAS", OracleDbType.Varchar2,  null  , ParameterDirection.Input));

                        cmd.Parameters.Add(new OracleParameter("C_CATEGORIA", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(new OracleParameter("C_TASA_X_PLAN", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(new OracleParameter("C_DET_PLAN", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;

                        cn.Open();
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        if (dr.HasRows)
                        {
                            string SCATE = "";
                            while (dr.Read())
                            {
                                var item = new CategoryQuotationVM();
                                // GCAA
                                item.SCATEGORIA = dr["SCATEGORIA"] == DBNull.Value ? "" : dr["SCATEGORIA"].ToString();
                                item.SCATEGORIA_view = (dr["SCATEGORIA"].ToString() == SCATE) ? "" : dr["SCATEGORIA"].ToString();//GCAA 28/09/2023
                                item.NCOUNT = dr["NCOUNT"] == DBNull.Value ? 0 : int.Parse(dr["NCOUNT"].ToString());
                                item.SRANGO_EDAD = dr["SEDADES"] == DBNull.Value ? "" : dr["SEDADES"].ToString();//GCAA 28/09/2023
                                item.NTOTAL_PLANILLA = dr["NTOTAL_PLANILLA"] == DBNull.Value ? 0.0 : double.Parse(dr["NTOTAL_PLANILLA"].ToString());
                                item.NTASA = dr["NTASA"] == DBNull.Value ? 0.0 : double.Parse(dr["NTASA"].ToString());
                                item.ProposalRate = dr["NTASA_PR"] == DBNull.Value ? 0.0 : double.Parse(dr["NTASA_PR"].ToString());
                                item.ModuleCode = dr["NMODULEC"] == DBNull.Value ? 0 : int.Parse(dr["NMODULEC"].ToString());
                                SCATE = dr["SCATEGORIA"] == DBNull.Value ? "" : dr["SCATEGORIA"].ToString();
                                obj.categoryList.Add(item);
                            }

                            dr.NextResult();

                            while (dr.Read())
                            {
                                var item = new RateXPlanQuotationVM();
                                item.NTASA = dr["NTASA"] == DBNull.Value ? 0.0 : double.Parse(dr["NTASA"].ToString());
                                obj.rateByPlanList.Add(item);
                            }

                            dr.NextResult();

                            while (dr.Read())
                            {
                                var item = new DetailPlanQuotationVM();
                                item.PRIMA_MINIMA = dr["PRIMA_MINIMA"] == DBNull.Value ? 0.0 : double.Parse(dr["PRIMA_MINIMA"].ToString());
                                item.DET_PLAN = dr["DET_PLAN"] == DBNull.Value ? "" : dr["DET_PLAN"].ToString();
                                item.COD_PLAN = dr["COD_PLAN"] == DBNull.Value ? 0 : Convert.ToInt32(dr["COD_PLAN"].ToString());
                                obj.detailPlanList.Add(item);
                            }

                        }

                        ELog.CloseConnection(dr);
                    }
                    catch (Exception ex)
                    {
                        LogControl.save("readInfoTrama", ex.ToString(), "3");
                    }
                    finally
                    {
                        if (cn.State == ConnectionState.Open) cn.Close();
                    }
                }
            }

            return obj;
        }

        public SalidaErrorBaseVM ValidateTramaWeb(InfoClientBM infoClient)
        {
            SalidaErrorBaseVM response = new SalidaErrorBaseVM();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.sp_ValidaTramaWebVL;

            try
            {
                parameter.Add(new OracleParameter("P_NUM_TRABAJADORES_OBR", OracleDbType.Varchar2, infoClient.NumTrabajadoresObr, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUM_TRABAJADORES_EMP", OracleDbType.Varchar2, infoClient.NumTrabajadoresEmp, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_MONTO_PLANILLA_OBR", OracleDbType.Varchar2, infoClient.MontoPlanillaObr, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_MONTO_PLANILLA_EMP", OracleDbType.Varchar2, infoClient.MontoPlanillaEmp, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_COMMISSION", OracleDbType.Double, infoClient.TipoComision, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTIP_RENOV", OracleDbType.Double, infoClient.TipoRenovacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPAYFREQ", OracleDbType.Double, infoClient.FrecuenciaPago, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Double, infoClient.UserCode, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Varchar2, infoClient.FechaEfecto, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOD_ACTIVITY_TEC", OracleDbType.Varchar2, infoClient.CodigoActividad, ParameterDirection.Input));

                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);

                P_COD_ERR.Size = 9000;
                P_MESSAGE.Size = 9000;

                parameter.Add(P_COD_ERR);
                parameter.Add(P_MESSAGE);
                //parameter.Add(C_TABLE);

                OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storeprocedure, parameter);

                /*if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        ErroresVM item = new ErroresVM();

                        item.REGISTRO = dr["REGISTRO"] == DBNull.Value ? "" : dr["REGISTRO"].ToString();
                        item.DESCRIPCION = dr["DESCRIPCION"] == DBNull.Value ? "" : dr["DESCRIPCION"].ToString();

                        response.errorList.Add(item);
                    }
                }*/

                response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                response.P_MESSAGE = P_MESSAGE.Value.ToString();
                ELog.CloseConnection(dr);

            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = ex.ToString();
                LogControl.save("ValidateTramaWeb", ex.ToString(), "3");
            }

            return response;
        }

        public SalidaErrorBaseVM ValidateTramaVL_ECOM(ValidaTramaBM validaTramaBM)
        {
            SalidaErrorBaseVM response = new SalidaErrorBaseVM();

            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        OracleDataReader dr;

                        cmd.Connection = cn;
                        cmd.CommandText = ProcedureName.sp_ValidaTramaEcommerceVL;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, validaTramaBM.NID_PROC, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Varchar2, validaTramaBM.NTYPE_TRANSAC, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NTYPE_COMMISSION", OracleDbType.Int32, validaTramaBM.NTYPE_COMMISSION, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NTIP_RENOV", OracleDbType.Int32, validaTramaBM.NTIP_RENOV, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NPAYFREQ", OracleDbType.Int32, validaTramaBM.NPAYFREQ, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, validaTramaBM.NUSERCODE, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Date, Convert.ToDateTime(validaTramaBM.DEFFECDATE), ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, validaTramaBM.NID_COTIZACION, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_SCOD_ACTIVITY_TEC", OracleDbType.Varchar2, validaTramaBM.P_NTECHNICAL, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NACT_MINA", OracleDbType.Int32, validaTramaBM.NFLAG_MINA, ParameterDirection.Input));

                        var P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, ParameterDirection.Output);
                        var P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                        //var C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);

                        P_MESSAGE.Size = 4000;

                        cmd.Parameters.Add(P_COD_ERR);
                        cmd.Parameters.Add(P_MESSAGE);
                        //cmd.Parameters.Add(C_TABLE);
                        cmd.Parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;

                        cn.Open();
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                var item = new ErroresVM();
                                item.REGISTRO = dr["REGISTRO"] == DBNull.Value ? "" : dr["REGISTRO"].ToString();
                                item.DESCRIPCION = dr["DESCRIPCION"] == DBNull.Value ? "" : dr["DESCRIPCION"].ToString();
                                item.COD_ERROR = dr["COD_ERROR"] == DBNull.Value ? 0 : int.Parse(dr["COD_ERROR"].ToString());
                                response.errorList.Add(item);
                            }
                        }

                        response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                        response.P_MESSAGE = P_MESSAGE.Value.ToString();
                        ELog.CloseConnection(dr);

                    }
                    catch (Exception ex)
                    {
                        response.P_COD_ERR = 1;
                        response.P_MESSAGE = ex.ToString();
                        LogControl.save("ValidateTramaVL_ECOM", ex.ToString(), "3");
                    }
                    finally
                    {
                        if (cn.State == ConnectionState.Open) cn.Close();
                    }
                }
            }

            return response;
        }

        public List<ErroresVM> ValidateTrama(ValidaTramaBM validaTramaBM)
        {
            List<ErroresVM> response = new List<ErroresVM>();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.sp_ValidaTramaVL;

            try
            {
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, validaTramaBM.NID_PROC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Varchar2, validaTramaBM.NTYPE_TRANSAC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_COMMISSION", OracleDbType.Int32, validaTramaBM.NTYPE_COMMISSION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTIP_RENOV", OracleDbType.Int32, validaTramaBM.NTIP_RENOV, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPAYFREQ", OracleDbType.Int32, validaTramaBM.NPAYFREQ, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, validaTramaBM.NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Date, validaTramaBM.DEFFECDATE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, validaTramaBM.NID_COTIZACION, ParameterDirection.Input));

                //OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);

                //parameter.Add(P_COD_ERR);
                parameter.Add(C_TABLE);
                /* parameter.Add(new OracleParameter("P_COD_ERR", OracleDbType.Int32, ParameterDirection.Output));
                 parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));*/
                /*OracleParameter C_CURSOR = new OracleParameter("C_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_CURSOR);*/


                OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storeprocedure, parameter);

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        ErroresVM item = new ErroresVM();

                        item.REGISTRO = dr["REGISTRO"] == DBNull.Value ? "" : dr["REGISTRO"].ToString();
                        item.DESCRIPCION = dr["DESCRIPCION"] == DBNull.Value ? "" : dr["DESCRIPCION"].ToString();

                        response.Add(item);
                    }
                }
                //response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                ELog.CloseConnection(dr);
            }
            catch (Exception ex)
            {
                LogControl.save("ValidateTrama", ex.ToString(), "3");
            }

            return response;
        }

        public SalidaErrorBaseVM ValidateTramaCot(ValidaTramaBM validaTramaBM)
        {
            SalidaErrorBaseVM response = new SalidaErrorBaseVM();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.sp_ValidaTramaVL;

            try
            {
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, validaTramaBM.NID_PROC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Varchar2, validaTramaBM.NTYPE_TRANSAC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_COMMISSION", OracleDbType.Int32, validaTramaBM.NTYPE_COMMISSION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTIP_RENOV", OracleDbType.Int32, validaTramaBM.NTIP_RENOV, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPAYFREQ", OracleDbType.Int32, validaTramaBM.NPAYFREQ, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, validaTramaBM.NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Date, Convert.ToDateTime(validaTramaBM.DEFFECDATE), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, validaTramaBM.NID_COTIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOD_ACTIVITY_TEC", OracleDbType.Varchar2, validaTramaBM.P_NTECHNICAL, ParameterDirection.Input));

                parameter.Add(new OracleParameter("P_FLAG_PR", OracleDbType.Int32, validaTramaBM.P_FLAG_PR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_COMISSION_PR", OracleDbType.Double, validaTramaBM.P_COMISSION_PR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TASA_OB", OracleDbType.Double, validaTramaBM.P_TASA_OB, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TASA_EM", OracleDbType.Double, validaTramaBM.P_TASA_EM, ParameterDirection.Input));

                parameter.Add(new OracleParameter("P_FLAG_CALC_EXC", OracleDbType.Double, validaTramaBM.P_FLAG_EXC, ParameterDirection.Input));

                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                OracleParameter P_FLAG_EXC = new OracleParameter("P_FLAG_EXC", OracleDbType.Varchar2, ParameterDirection.Output);
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);

                P_COD_ERR.Size = 9000;
                P_MESSAGE.Size = 9000;

                parameter.Add(P_COD_ERR);
                parameter.Add(P_MESSAGE);
                parameter.Add(C_TABLE);

                OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storeprocedure, parameter);

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        ErroresVM item = new ErroresVM();

                        item.REGISTRO = dr["REGISTRO"] == DBNull.Value ? "" : dr["REGISTRO"].ToString();
                        item.DESCRIPCION = dr["DESCRIPCION"] == DBNull.Value ? "" : dr["DESCRIPCION"].ToString();

                        response.errorList.Add(item);
                    }
                }

                response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                response.P_MESSAGE = P_MESSAGE.Value.ToString();
                response.P_FLAG_EXC = Convert.ToInt32(P_FLAG_EXC.Value);
                ELog.CloseConnection(dr);

            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = ex.ToString();
                LogControl.save("ValidateTramaCot", ex.ToString(), "3");
            }

            return response;
        }

        public SalidaErrorBaseVM ValidateTramaVL(ValidaTramaBM validaTramaBM)
        {
            SalidaErrorBaseVM response = new SalidaErrorBaseVM();
            List<OracleParameter> parameter = new List<OracleParameter>();
            
            string storeprocedure = validaTramaBM.NTYPE_TRANSAC != "3" ? ProcedureName.sp_ValidaTramaVL : ProcedureName.pkg_ValidadorGenPD + ".SP_VALIDA_TRAMA";

            try
            {
                if (validaTramaBM.NTYPE_TRANSAC == "3") // INI EXC PRIMA COBRADA JTV 04042023
                {
                    parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, validaTramaBM.P_NBRANCH, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, validaTramaBM.P_NPRODUCT, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int64, validaTramaBM.P_NPOLICY, ParameterDirection.Input));
                } // FIN EXC PRIMA COBRADA JTV 04042023

                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, validaTramaBM.NID_PROC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Varchar2, validaTramaBM.NTYPE_TRANSAC, ParameterDirection.Input));

                if (validaTramaBM.NTYPE_TRANSAC != "3")
                {
                    parameter.Add(new OracleParameter("P_NTYPE_COMMISSION", OracleDbType.Int32, validaTramaBM.NTYPE_COMMISSION, ParameterDirection.Input));
                }

                parameter.Add(new OracleParameter("P_NTIP_RENOV", OracleDbType.Int32, validaTramaBM.NTIP_RENOV, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPAYFREQ", OracleDbType.Int32, validaTramaBM.NPAYFREQ, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, validaTramaBM.NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Date, Convert.ToDateTime(validaTramaBM.DEFFECDATE), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, validaTramaBM.NID_COTIZACION, ParameterDirection.Input));

                if (validaTramaBM.NTYPE_TRANSAC != "3")
                {
                    parameter.Add(new OracleParameter("P_SCOD_ACTIVITY_TEC", OracleDbType.Varchar2, validaTramaBM.P_NTECHNICAL, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_FLAG_PR", OracleDbType.Int32, validaTramaBM.P_FLAG_PR, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_COMISSION_PR", OracleDbType.Double, validaTramaBM.P_COMISSION_PR, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_TASA_OB", OracleDbType.Double, validaTramaBM.P_TASA_OB, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_TASA_EM", OracleDbType.Double, validaTramaBM.P_TASA_EM, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_NREM_EXC", OracleDbType.Double, validaTramaBM.P_NREM_EXC, ParameterDirection.Input)); //rq Exc EH
                }

                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                OracleParameter P_FLAG_EXC = new OracleParameter("P_FLAG_EXC", OracleDbType.Varchar2, ParameterDirection.Output);
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);

                P_COD_ERR.Size = 9000;
                P_MESSAGE.Size = 9000;
                P_FLAG_EXC.Size = 9000;

                parameter.Add(P_COD_ERR);
                parameter.Add(P_MESSAGE);

                if (validaTramaBM.NTYPE_TRANSAC != "3")
                {
                    parameter.Add(P_FLAG_EXC);
                }
                parameter.Add(C_TABLE);

                OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storeprocedure, parameter);

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        ErroresVM item = new ErroresVM();

                        item.REGISTRO = dr["REGISTRO"] == DBNull.Value ? "" : dr["REGISTRO"].ToString();
                        item.DESCRIPCION = dr["DESCRIPCION"] == DBNull.Value ? "" : dr["DESCRIPCION"].ToString();
                        response.errorList.Add(item);
                    }
                }

                response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                response.P_MESSAGE = P_MESSAGE.Value.ToString();

                if (validaTramaBM.NTYPE_TRANSAC != "3")
                {
                    response.P_FLAG_EXC = Convert.ToInt32(P_FLAG_EXC.Value.ToString());
                }

                ELog.CloseConnection(dr);
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = ex.ToString();
                LogControl.save("ValidateTramaVL", ex.ToString(), "3");
            }

            return response;
        }

        public SalidaErrorBaseVM ValidateTramaCovid(ValidaTramaBM validaTramaBM)
        {
            SalidaErrorBaseVM response = new SalidaErrorBaseVM();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.sp_ValidaTramaCovid;

            try
            {
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, validaTramaBM.NID_PROC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Varchar2, validaTramaBM.NTYPE_TRANSAC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, validaTramaBM.NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Date, Convert.ToDateTime(validaTramaBM.DEFFECDATE), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, validaTramaBM.NID_COTIZACION, ParameterDirection.Input));

                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);

                P_COD_ERR.Size = 9000;

                parameter.Add(P_COD_ERR);
                parameter.Add(C_TABLE);

                OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storeprocedure, parameter);

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        ErroresVM item = new ErroresVM();
                        item.REGISTRO = dr["REGISTRO"] == DBNull.Value ? "" : dr["REGISTRO"].ToString();
                        item.DESCRIPCION = dr["DESCRIPCION"] == DBNull.Value ? "" : dr["DESCRIPCION"].ToString();
                        response.errorList.Add(item);
                    }
                }

                response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                ELog.CloseConnection(dr);
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                LogControl.save("ValidateTramaCovid", ex.ToString(), "3");
            }

            return response;
        }

        public SalidaErrorBaseVM ValidateTramaAccidentesPersonales(ValidaTramaBM validaTramaBM, validaTramaVM objValida)
        {
            SalidaErrorBaseVM response = new SalidaErrorBaseVM();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.pkg_ValidadorGenPD + ".SP_VALIDADOR_PRODUCTOS";

            bool flagReMod = objValida.datosPoliza.trxCode == "RE" && Convert.ToBoolean(objValida.datosPoliza.modoEditar);
            bool flagDesg = ELog.obtainConfig("desgravamenBranch") == objValida.datosPoliza.branch.ToString();

            try
            {
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, validaTramaBM.NID_PROC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Varchar2, validaTramaBM.NTYPE_TRANSAC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, validaTramaBM.NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Date, Convert.ToDateTime(validaTramaBM.DEFFECDATE), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, flagReMod ? 0 : validaTramaBM.NID_COTIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, objValida.datosPoliza.codTipoProducto, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, objValida.datosPoliza.branch, ParameterDirection.Input));

                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                P_COD_ERR.Size = 9000;
                P_MESSAGE.Size = 9000;

                parameter.Add(P_COD_ERR);
                parameter.Add(P_MESSAGE);
                parameter.Add(C_TABLE);

                parameter.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, objValida.datosPoliza.numDocumento, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TASA_SEGURO", OracleDbType.Varchar2, flagDesg ? objValida.datosPoliza.tasa_seguro : 0, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TASA_AHORRO", OracleDbType.Varchar2, flagDesg ? objValida.datosPoliza.tasa_ahorro : 0, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_CUMULO_ASEG", OracleDbType.Varchar2, flagDesg ? objValida.datosPoliza.cumulo_asegurado : 0, ParameterDirection.Input));

                //parameter.Add(new OracleParameter("P_NTYPE_PRODUCT", OracleDbType.Varchar2, objValida.datosPoliza.codTipoProducto, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_PROFILE", OracleDbType.Varchar2, objValida.datosPoliza.codTipoPerfil, ParameterDirection.Input));

                OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storeprocedure, parameter);

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        ErroresVM item = new ErroresVM();
                        item.REGISTRO = dr["REGISTRO"] == DBNull.Value ? "" : dr["REGISTRO"].ToString();
                        item.DESCRIPCION = dr["DESCRIPCION"] == DBNull.Value ? "" : dr["DESCRIPCION"].ToString();
                        if (ELog.obtainConfig("desgravamenBranch") != objValida.datosPoliza.branch.ToString())
                        {
                            item.COD_ERROR = dr["TIP_ERROR"] == DBNull.Value ? 0 : Convert.ToInt32(dr["TIP_ERROR"].ToString());
                        }

                        response.errorList.Add(item);
                    }
                }

                response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                response.P_MESSAGE = P_MESSAGE.Value.ToString();
                ELog.CloseConnection(dr);
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = ex.ToString();
                response.errorList.Add(new ErroresVM
                {
                    REGISTRO = "0",
                    DESCRIPCION = ex.ToString()
                });
            }

            return response;
        }

        public SalidaTramaBaseVM InsertInfoMatriz(validaTramaVM data)
        {
            var response = new SalidaTramaBaseVM();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.pkg_ReaDataAP + ".GEN_PREPARE_DATA_PROC_LAST";

            try
            {
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, data.codProceso, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STYPE_TRANSAC", OracleDbType.Varchar2, data.datosPoliza.trxCode, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Varchar2, data.datosPoliza.InicioVigPoliza, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Varchar2, data.datosPoliza.FinVigPoliza, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, data.codUsuario, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.nroCotizacion, ParameterDirection.Input));

                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                P_SMESSAGE.Size = 9000;

                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);

                this.ExecuteByStoredProcedureVT(storeprocedure, parameter);

                response.P_COD_ERR = P_NCODE.Value.ToString();
                response.P_MESSAGE = P_SMESSAGE.Value.ToString();

            }
            catch (Exception ex)
            {
                response.P_COD_ERR = "1";
                response.P_MESSAGE = ex.ToString();
                LogControl.save("IntInfoMatriz", ex.ToString(), "3");
            }

            return response;
        }

        public SalidaTramaBaseVM InsCotizaTrama(validaTramaVM data, int flagTecnica, DbConnection connection, DbTransaction trx)
        {
            var response = new SalidaTramaBaseVM();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = data.flagCot == "1" ? ProcedureName.pkg_ValidadorGenPD + ".SP_INS_DET_AP_PM" : ProcedureName.pkg_ValidadorGenPD + ".SP_INS_DET_AP";

            try
            {
                if (Convert.ToBoolean(data.PolizaMatriz) == true && data.flagCot == "1")
                {
                    data.flagpremium = 0;
                }
            }
            catch (Exception ex)
            {
                data.flagpremium = 0;
            }

            var flagAccess = new string[] { ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch") }.Contains(data.datosPoliza.branch.ToString());
            var flagAccessVI = new string[] { ELog.obtainConfig("vidaIndividualBranch") }.Contains(data.datosPoliza.branch.ToString());

            try
            {
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, data.codProceso, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_PRODUCT", OracleDbType.Int32, data.datosPoliza.codTipoNegocio, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, ELog.obtainConfig("planMaestro" + data.datosPoliza.branch) /*data.datosPoliza.codTipoPlan*/, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_PROFILE", OracleDbType.Int32, data.datosPoliza.codTipoPerfil, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMODALITY", OracleDbType.Int32, data.datosPoliza.codTipoModalidad, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_EMPLOYEE", OracleDbType.Varchar2, data.datosPoliza.type_employee, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.datosPoliza.branch, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTIP_RENOV", OracleDbType.Int32, data.datosPoliza.codTipoRenovacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPAYFREQ", OracleDbType.Int32, data.datosPoliza.codTipoFrecuenciaPago, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOLINVOT", OracleDbType.Int32, data.datosPoliza.codTipoFacturacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCIUU", OracleDbType.Int32, data.datosPoliza.CodActividadRealizar, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCURRENCY", OracleDbType.Int32, data.datosPoliza.codMon, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE_POL", OracleDbType.Varchar2, data.datosPoliza.InicioVigPoliza, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEXPIRDAT_POL", OracleDbType.Varchar2, data.datosPoliza.FinVigPoliza, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE_ASE", OracleDbType.Varchar2, data.datosPoliza.InicioVigAsegurado, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEXPIRDAT_ASE", OracleDbType.Varchar2, data.datosPoliza.FinVigAsegurado, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STYPE_TRANSAC", OracleDbType.Varchar2, data.datosPoliza.trxCode, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, data.codUsuario, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, data.datosPoliza.codTipoProducto, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPREMIUM_PROP", OracleDbType.Double, data.premium, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NFLAG_REC", OracleDbType.Int32, data.flagCalcular, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NFLAG_SA", OracleDbType.Int32, data.flagpremium, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.nroCotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Int32, data.type_mov == null ? null : data.type_mov == "0" ? "1" : data.type_mov, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SAPLICACION", OracleDbType.Varchar2, data.codAplicacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTIPO_FACTURACION", OracleDbType.Int32, data.datosPoliza.codTipoFacturacion, ParameterDirection.Input));

                OracleParameter P_COD_ERR = new OracleParameter("P_ERROR", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                P_COD_ERR.Size = 9000;
                P_MESSAGE.Size = 9000;

                parameter.Add(P_COD_ERR);
                parameter.Add(P_MESSAGE);

                parameter.Add(new OracleParameter("P_PRORRATDATE", OracleDbType.Varchar2, flagAccessVI ? data.datosPoliza.fechaProrrateo : null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NROCUOTAS", OracleDbType.Int32, flagAccessVI ? Convert.ToInt32(data.datosPoliza.nroCuotas) : 0, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_OCCUPATION", OracleDbType.Int32, flagAccessVI ? Convert.ToInt32(data.datosPoliza.occupation) : 0, ParameterDirection.Input));     //R.P. V.C.F. V.2
                parameter.Add(new OracleParameter("P_ACTIVITY", OracleDbType.Int32, flagAccessVI ? Convert.ToInt32(data.datosPoliza.activity) : 0, ParameterDirection.Input));         //R.P. V.C.F. V.2

                parameter.Add(new OracleParameter("P_NIGV", OracleDbType.Double, flagAccess ? data.igv : 0, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPREMIUMT", OracleDbType.Double, flagAccess ? data.premiumTotal : 0, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPREMIUMN_ASEG", OracleDbType.Double, flagAccess ? data.asegPremium : 0, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIGV_ASEG", OracleDbType.Double, flagAccess ? data.asegIgv : 0, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPREMIUMT_ASEG", OracleDbType.Double, flagAccess ? data.asegPremiumTotal : 0, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NFLAG_TECNICA", OracleDbType.Int32, flagTecnica, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTASA", OracleDbType.Decimal, data.ntasa_tariff, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMONTO_PLA", OracleDbType.Decimal, data.montoPlanilla, ParameterDirection.Input));

                this.ExecuteByStoredProcedureVT_TRX(storeprocedure, parameter, connection, trx);

                response.P_COD_ERR = P_COD_ERR.Value.ToString();
                response.P_MESSAGE = P_MESSAGE.Value.ToString();

            }
            catch (Exception ex)
            {
                response.P_COD_ERR = "1";
                response.P_MESSAGE = ex.ToString();
                LogControl.save("InsCotizaTrama", ex.ToString(), "3");
            }

            return response;
        }

        public SalidaTramaBaseVM InsCotizaTramaExc(validaTramaVM data, DbConnection connection, DbTransaction trx)
        {
            var response = new SalidaTramaBaseVM();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.pkg_ValidadorGenPD + ".SP_INS_DET_AP_EXC";

            try
            {
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, data.codProceso, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_PRODUCT", OracleDbType.Int32, data.datosPoliza.codTipoNegocio, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, ELog.obtainConfig("planMaestro" + data.datosPoliza.branch), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_PROFILE", OracleDbType.Int32, data.datosPoliza.codTipoPerfil, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMODALITY", OracleDbType.Int32, data.datosPoliza.codTipoModalidad, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_EMPLOYEE", OracleDbType.Varchar2, data.datosPoliza.type_employee, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.datosPoliza.branch, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTIP_RENOV", OracleDbType.Int32, data.datosPoliza.codTipoRenovacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPAYFREQ", OracleDbType.Int32, data.datosPoliza.codTipoFrecuenciaPago, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOLINVOT", OracleDbType.Int32, data.datosPoliza.codTipoFacturacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCIUU", OracleDbType.Int32, data.datosPoliza.CodActividadRealizar, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCURRENCY", OracleDbType.Int32, data.datosPoliza.codMon, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE_POL", OracleDbType.Varchar2, data.datosPoliza.InicioVigPoliza, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEXPIRDAT_POL", OracleDbType.Varchar2, data.datosPoliza.FinVigPoliza, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE_CER", OracleDbType.Varchar2, data.datosPoliza.InicioVigAsegurado, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEXPIRDAT_CER", OracleDbType.Varchar2, data.datosPoliza.FinVigAsegurado, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STYPE_TRANSAC", OracleDbType.Varchar2, data.datosPoliza.trxCode, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, data.codUsuario, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, data.codProducto, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPREMIUM_PROP", OracleDbType.Varchar2, data.premium, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NFLAG_REC", OracleDbType.Int32, data.flagCalcular, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NFLAG_SA", OracleDbType.Int32, data.flagpremium, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.nroCotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Int32, data.type_mov == null ? null : data.type_mov == "0" ? "1" : data.type_mov, ParameterDirection.Input));

                parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int64, data.nroPoliza, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TIPO_CALC_EXC", OracleDbType.Int32, data.tipoExclusion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DFECANULA", OracleDbType.Date, Convert.ToDateTime(data.fechaExclusion), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NFLAG_PRIMA_COBRADA", OracleDbType.Int32, data.devolucionPrima, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTIPO_FACTURACION", OracleDbType.Int32, data.datosPoliza.codTipoFacturacion, ParameterDirection.Input));

                OracleParameter P_COD_ERR = new OracleParameter("P_ERROR", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                P_COD_ERR.Size = 9000;
                P_MESSAGE.Size = 9000;

                parameter.Add(P_COD_ERR);
                parameter.Add(P_MESSAGE);

                this.ExecuteByStoredProcedureVT_TRX(storeprocedure, parameter, connection, trx);

                response.P_COD_ERR = P_COD_ERR.Value.ToString();
                response.P_MESSAGE = P_MESSAGE.Value.ToString();

            }
            catch (Exception ex)
            {
                response.P_COD_ERR = "1";
                response.P_MESSAGE = ex.ToString();
                LogControl.save("InsCotizaTramaExc", ex.ToString(), "3");
            }

            return response;
        }

        public SalidaTramaBaseVM InsCotizaTramaEndoso(validaTramaVM data, DbConnection connection, DbTransaction trx)
        {
            var response = new SalidaTramaBaseVM();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.pkg_ValidadorGenPD + ".SP_INS_DET_AP_ENDOSO";

            try
            {
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, data.codProceso, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_PRODUCT", OracleDbType.Int32, data.datosPoliza.codTipoNegocio, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, ELog.obtainConfig("planMaestro" + data.datosPoliza.branch), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_PROFILE", OracleDbType.Int32, data.datosPoliza.codTipoPerfil, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMODALITY", OracleDbType.Int32, data.datosPoliza.codTipoModalidad, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_EMPLOYEE", OracleDbType.Varchar2, data.datosPoliza.type_employee, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCERTYPE", OracleDbType.Char, "2", ParameterDirection.Input)); // jaime dice q va en 2
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.datosPoliza.branch, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTIP_RENOV", OracleDbType.Int32, data.datosPoliza.codTipoRenovacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPAYFREQ", OracleDbType.Int32, data.datosPoliza.codTipoFrecuenciaPago, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOLINVOT", OracleDbType.Int32, data.datosPoliza.codTipoFacturacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCIUU", OracleDbType.Int32, data.datosPoliza.CodActividadRealizar, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCURRENCY", OracleDbType.Int32, data.datosPoliza.codMon, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE_POL", OracleDbType.Varchar2, data.datosPoliza.InicioVigPoliza, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEXPIRDAT_POL", OracleDbType.Varchar2, data.datosPoliza.FinVigPoliza, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE_ASE", OracleDbType.Varchar2, data.datosPoliza.InicioVigAsegurado, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEXPIRDAT_ASE", OracleDbType.Varchar2, data.datosPoliza.FinVigAsegurado, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STYPE_TRANSAC", OracleDbType.Varchar2, data.datosPoliza.trxCode, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, data.codUsuario, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, data.codProducto, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPREMIUM_PROP", OracleDbType.Varchar2, data.premium, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NFLAG_REC", OracleDbType.Int32, data.flagCalcular, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NFLAG_SA", OracleDbType.Int32, data.flagpremium, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.nroCotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Int32, data.type_mov == null ? null : data.type_mov == "0" ? "1" : data.type_mov, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SAPLICACION", OracleDbType.Varchar2, data.codAplicacion, ParameterDirection.Input));
                //parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int64, data.nroPoliza, ParameterDirection.Input));
                //parameter.Add(new OracleParameter("P_TIPO_CALC_EXC", OracleDbType.Int32, data.tipoExclusion, ParameterDirection.Input));
                //parameter.Add(new OracleParameter("P_DFECANULA", OracleDbType.Date, Convert.ToDateTime(data.fechaExclusion), ParameterDirection.Input));
                //parameter.Add(new OracleParameter("P_NFLAG_PRIMA_COBRADA", OracleDbType.Int32, data.devolucionPrima, ParameterDirection.Input));


                OracleParameter P_COD_ERR = new OracleParameter("P_ERROR", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                P_COD_ERR.Size = 9000;
                P_MESSAGE.Size = 9000;

                parameter.Add(P_COD_ERR);
                parameter.Add(P_MESSAGE);


                this.ExecuteByStoredProcedureVT_TRX(storeprocedure, parameter, connection, trx);

                response.P_COD_ERR = P_COD_ERR.Value.ToString();
                response.P_MESSAGE = P_MESSAGE.Value.ToString();

            }
            catch (Exception ex)
            {
                response.P_COD_ERR = "1";
                response.P_MESSAGE = ex.ToString();
                LogControl.save("InsCotizaTramaEndoso", ex.ToString(), "3");
            }

            return response;
        }
        public SalidaErrorBaseVM InsCotizaTramaPol(validaTramaVM data, DbConnection connection, DbTransaction trx)
        {
            SalidaErrorBaseVM response = new SalidaErrorBaseVM();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.pkg_ValidadorGenPD + ".SP_INS_DET_POL_M";

            try
            {
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, data.codProceso, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_PRODUCT", OracleDbType.Int32, data.datosPoliza.codTipoNegocio, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, data.datosPoliza.codTipoPlan, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_PROFILE", OracleDbType.Int32, data.datosPoliza.codTipoPerfil, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMODALITY", OracleDbType.Int32, data.datosPoliza.codTipoModalidad, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_EMPLOYEE", OracleDbType.Varchar2, data.datosPoliza.type_employee, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.datosPoliza.branch, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTIP_RENOV", OracleDbType.Int32, data.datosPoliza.codTipoRenovacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPAYFREQ", OracleDbType.Int32, data.datosPoliza.codTipoFrecuenciaPago, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOLINVOT", OracleDbType.Int32, data.datosPoliza.codTipoFacturacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCIUU", OracleDbType.Int32, data.datosPoliza.CodActividadRealizar, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCURRENCY", OracleDbType.Int32, data.datosPoliza.codMon, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE_POL", OracleDbType.Varchar2, data.datosPoliza.InicioVigPoliza, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEXPIRDAT_POL", OracleDbType.Varchar2, data.datosPoliza.FinVigPoliza, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE_ASE", OracleDbType.Varchar2, data.datosPoliza.InicioVigAsegurado, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEXPIRDAT_ASE", OracleDbType.Varchar2, data.datosPoliza.FinVigAsegurado, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STYPE_TRANSAC", OracleDbType.Varchar2, data.datosPoliza.trxCode, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, data.codUsuario, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, data.codProducto, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPREMIUM_PROP", OracleDbType.Varchar2, data.premium, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.nroCotizacion, ParameterDirection.Input));

                OracleParameter P_COD_ERR = new OracleParameter("P_ERROR", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                P_COD_ERR.Size = 9000;
                P_MESSAGE.Size = 9000;

                parameter.Add(P_COD_ERR);
                parameter.Add(P_MESSAGE);


                this.ExecuteByStoredProcedureVT_TRX(storeprocedure, parameter, connection, trx);

                response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                response.P_MESSAGE = Convert.ToString(P_MESSAGE.Value.ToString());

            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = ex.ToString();
                LogControl.save("InsCotizaTramaPol", ex.ToString(), "3");
            }

            return response;
        }

        public SalidaTramaBaseVM InsReCotizaTrama(validaTramaVM data, DbConnection connection, DbTransaction trx)
        {
            var response = new SalidaTramaBaseVM();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.pkg_ValidadorGenPD + ".SP_INS_CAL_COT";

            var flagPM = generateFlagPM(data);


            try
            {
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, data.codProceso, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_PRODUCT", OracleDbType.Int32, Convert.ToInt32(data.datosPoliza.codTipoNegocio), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, Convert.ToInt32(ELog.obtainConfig("planMaestro" + data.datosPoliza.branch)), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_PROFILE", OracleDbType.Int32, Convert.ToInt32(data.datosPoliza.codTipoPerfil), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMODALITY", OracleDbType.Int32, Convert.ToInt32(data.datosPoliza.codTipoModalidad), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_EMPLOYEE", OracleDbType.Varchar2, data.datosPoliza.type_employee, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, Convert.ToInt32(data.datosPoliza.branch), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTIP_RENOV", OracleDbType.Int32, Convert.ToInt32(data.datosPoliza.codTipoRenovacion), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPAYFREQ", OracleDbType.Int32, Convert.ToInt32(data.datosPoliza.codTipoFrecuenciaPago), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOLINVOT", OracleDbType.Int32, Convert.ToInt32(data.datosPoliza.codTipoFacturacion), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCIUU", OracleDbType.Int32, Convert.ToInt32(data.datosPoliza.CodActividadRealizar), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCURRENCY", OracleDbType.Int32, Convert.ToInt32(data.datosPoliza.codMon), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE_POL", OracleDbType.Varchar2, data.datosPoliza.InicioVigPoliza, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEXPIRDAT_POL", OracleDbType.Varchar2, data.datosPoliza.FinVigPoliza, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE_ASE", OracleDbType.Varchar2, data.datosPoliza.InicioVigAsegurado, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEXPIRDAT_ASE", OracleDbType.Varchar2, data.datosPoliza.FinVigAsegurado, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STYPE_TRANSAC", OracleDbType.Varchar2, data.datosPoliza.trxCode, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, Convert.ToInt32(data.codUsuario), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPREMIUM", OracleDbType.Double, Convert.ToDouble(data.premium), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMON_PLA", OracleDbType.Decimal, data.montoPlanilla, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCAN_TRAB", OracleDbType.Int64, Convert.ToInt64(data.CantidadTrabajadores), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, Convert.ToInt32(data.datosPoliza.codTipoProducto), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NFLAG", OracleDbType.Int32, 1, ParameterDirection.Input)); //0 Trama 1 Sin trama
                parameter.Add(new OracleParameter("P_NFLAG_SA", OracleDbType.Int32, Convert.ToInt32(data.flag), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, Convert.ToInt32(data.datosPoliza.nid_cotizacion), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Int32, Convert.ToInt32(data.type_mov), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SAPLICACION", OracleDbType.Varchar2, data.codAplicacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPREMIUM_PROP", OracleDbType.Double, Convert.ToDouble(data.premium), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPREMIUMN_ANU", OracleDbType.Double, 0, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPOLIZA_MATRIZ", OracleDbType.Int32, flagPM, ParameterDirection.Input)); // 0 aforo o 1 polizamatriz
                                                                                                                              // tarifa mensual/anul / te devuelve el tarifario
                                                                                                                              // poliza matriz                                                                                                                                          PARA

                parameter.Add(new OracleParameter("P_NTIPO_FACTURACION", OracleDbType.Int32, Convert.ToInt32(data.datosPoliza.codTipoFacturacion), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPREMIUMN_ASEG", OracleDbType.Double, Convert.ToDouble(data.asegPremium), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIGV_ASEG", OracleDbType.Double, Convert.ToDouble(data.asegIgv), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPREMIUMT_ASEG", OracleDbType.Double, Convert.ToDouble(data.asegPremiumTotal), ParameterDirection.Input));
                OracleParameter P_COD_ERR = new OracleParameter("P_ERROR", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                // OracleParameter P_FLAG = new OracleParameter("P_FLAG_PRI", OracleDbType.Int32, ParameterDirection.Output);

                P_COD_ERR.Size = 9000;
                P_MESSAGE.Size = 9000;

                parameter.Add(P_COD_ERR);
                parameter.Add(P_MESSAGE);

                //if (ELog.obtainConfig("vidaIndividualBranch") == data.datosPoliza.branch.ToString())
                //{
                parameter.Add(new OracleParameter("P_PRORRATDATE", OracleDbType.Varchar2, data.datosPoliza.fechaProrrateo, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_OCCUPATION", OracleDbType.Int32, Convert.ToInt32(data.datosPoliza.occupation), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NROCUOTAS", OracleDbType.Int32, Convert.ToInt32(data.datosPoliza.nroCuotas), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_ACTIVITY", OracleDbType.Int32, Convert.ToInt32(data.datosPoliza.activity), ParameterDirection.Input));         //R.P. V.C.F. V.2
                //}
                parameter.Add(new OracleParameter("P_NTASA", OracleDbType.Decimal, data.ntasa_tariff, ParameterDirection.Input));

                this.ExecuteByStoredProcedureVT_TRX(storeprocedure, parameter, connection, trx);

                response.P_COD_ERR = P_COD_ERR.Value.ToString();
                response.P_MESSAGE = Convert.ToString(P_MESSAGE.Value.ToString());
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = "1";
                response.P_MESSAGE = ex.ToString();
                LogControl.save("InsReCotizaTrama", ex.ToString(), "3");
            }

            return response;
        }

        private int generateFlagPM(validaTramaVM data)
        {
            var flagPM = 0;

            try
            {
                flagPM = Convert.ToBoolean(data.PolizaMatriz) ? 1 : 0;
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(data.PolizaMatriz))
                {
                    flagPM = new string[] { "0", "1" }.Contains((string)data.PolizaMatriz) ? Convert.ToInt32(data.PolizaMatriz) : 0;
                }
                else
                {
                    flagPM = 0;
                }
            }

            return flagPM;
        }



        public SalidaErrorBaseVM eliminarDPS(validaTramaVM request, DbConnection connection, DbTransaction trx)
        {
            SalidaErrorBaseVM response = new SalidaErrorBaseVM();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.pkg_ReaDataAP + ".DEL_DPS_COT";

            try
            {
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.codProceso, ParameterDirection.Input));

                OracleParameter P_COD_ERR = new OracleParameter("P_ERROR", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                P_COD_ERR.Size = 9000;
                P_MESSAGE.Size = 9000;

                parameter.Add(P_COD_ERR);
                parameter.Add(P_MESSAGE);

                this.ExecuteByStoredProcedureVT_TRX(storeprocedure, parameter, connection, trx);

                response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                response.P_MESSAGE = Convert.ToString(P_MESSAGE.Value.ToString());
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = ex.ToString();
                LogControl.save("eliminarDPS", ex.ToString(), "3");
            }

            return response;
        }

        public SalidaErrorBaseVM eliminarCoberturasCotTrx(validaTramaVM request, OracleConnection connection, OracleTransaction trx)
        {
            SalidaErrorBaseVM response = new SalidaErrorBaseVM();

            using (OracleCommand cmd = new OracleCommand())
            {
                try
                {
                    OracleDataReader dr;

                    cmd.Connection = connection;
                    cmd.CommandText = ProcedureName.pkg_ReaDataAP + ".DEL_COVER_COT";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Transaction = trx;

                    cmd.Parameters.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.codProceso, ParameterDirection.Input));

                    var P_COD_ERR = new OracleParameter("P_ERROR", OracleDbType.Int32, ParameterDirection.Output);
                    var P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                    P_MESSAGE.Size = 9000;

                    cmd.Parameters.Add(P_COD_ERR);
                    cmd.Parameters.Add(P_MESSAGE);

                    dr = cmd.ExecuteReader();

                    response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                    response.P_MESSAGE = P_MESSAGE.Value.ToString();
                }
                catch (Exception ex)
                {
                    response.P_COD_ERR = 1;
                    response.P_MESSAGE = ex.ToString();
                    LogControl.save("eliminarCoberturasCotTrx", ex.ToString(), "3");
                }
            }


            //List<OracleParameter> parameter = new List<OracleParameter>();
            //string storeprocedure = ProcedureName.pkg_ReaDataAP + ".DEL_COVER_COT";

            //try
            //{
            //    parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.codProceso, ParameterDirection.Input));

            //    OracleParameter P_COD_ERR = new OracleParameter("P_ERROR", OracleDbType.Int32, ParameterDirection.Output);
            //    OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

            //    P_COD_ERR.Size = 9000;
            //    P_MESSAGE.Size = 9000;

            //    parameter.Add(P_COD_ERR);
            //    parameter.Add(P_MESSAGE);

            //    this.ExecuteByStoredProcedureVT_TRX(storeprocedure, parameter, connection, trx);

            //    response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
            //    response.P_MESSAGE = Convert.ToString(P_MESSAGE.Value.ToString());
            //}
            //catch (Exception ex)
            //{
            //    response.P_COD_ERR = 1;
            //    response.P_MESSAGE = ex.ToString();
            //    ELog.save(this, ex);
            //}

            return response;
        }

        public SalidaErrorBaseVM eliminarAsistenciasCotTrx(validaTramaVM request, OracleConnection connection, OracleTransaction trx)
        {
            SalidaErrorBaseVM response = new SalidaErrorBaseVM();

            using (OracleCommand cmd = new OracleCommand())
            {
                try
                {
                    OracleDataReader dr;

                    cmd.Connection = connection;
                    cmd.CommandText = ProcedureName.pkg_ReaDataAP + ".DEL_ASSISTANCE_COT";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Transaction = trx;

                    cmd.Parameters.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.codProceso, ParameterDirection.Input));

                    var P_COD_ERR = new OracleParameter("P_ERROR", OracleDbType.Int32, ParameterDirection.Output);
                    var P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                    P_MESSAGE.Size = 9000;

                    cmd.Parameters.Add(P_COD_ERR);
                    cmd.Parameters.Add(P_MESSAGE);

                    dr = cmd.ExecuteReader();

                    response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                    response.P_MESSAGE = P_MESSAGE.Value.ToString();
                }
                catch (Exception ex)
                {
                    response.P_COD_ERR = 1;
                    response.P_MESSAGE = ex.ToString();
                    LogControl.save("eliminarAsistenciasCotTrx", ex.ToString(), "3");
                }
            }



            //List<OracleParameter> parameter = new List<OracleParameter>();
            //string storeprocedure = ProcedureName.pkg_ReaDataAP + ".DEL_ASSISTANCE_COT";

            //try
            //{
            //    parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.codProceso, ParameterDirection.Input));

            //    OracleParameter P_COD_ERR = new OracleParameter("P_ERROR", OracleDbType.Int32, ParameterDirection.Output);
            //    OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

            //    P_COD_ERR.Size = 9000;
            //    P_MESSAGE.Size = 9000;

            //    parameter.Add(P_COD_ERR);
            //    parameter.Add(P_MESSAGE);

            //    this.ExecuteByStoredProcedureVT_TRX(storeprocedure, parameter, connection, trx);

            //    response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
            //    response.P_MESSAGE = Convert.ToString(P_MESSAGE.Value.ToString());
            //}
            //catch (Exception ex)
            //{
            //    response.P_COD_ERR = 1;
            //    response.P_MESSAGE = ex.ToString();
            //    ELog.save(this, ex);
            //}

            return response;
        }

        public SalidaErrorBaseVM eliminarBeneficiosCotTrx(validaTramaVM request, OracleConnection connection, OracleTransaction trx)
        {
            SalidaErrorBaseVM response = new SalidaErrorBaseVM();

            using (OracleCommand cmd = new OracleCommand())
            {
                try
                {
                    OracleDataReader dr;

                    cmd.Connection = connection;
                    cmd.CommandText = ProcedureName.pkg_ReaDataAP + ".DEL_BENEFIT_COT";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Transaction = trx;

                    cmd.Parameters.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.codProceso, ParameterDirection.Input));

                    var P_COD_ERR = new OracleParameter("P_ERROR", OracleDbType.Int32, ParameterDirection.Output);
                    var P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                    P_MESSAGE.Size = 9000;

                    cmd.Parameters.Add(P_COD_ERR);
                    cmd.Parameters.Add(P_MESSAGE);

                    dr = cmd.ExecuteReader();

                    response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                    response.P_MESSAGE = P_MESSAGE.Value.ToString();
                }
                catch (Exception ex)
                {
                    response.P_COD_ERR = 1;
                    response.P_MESSAGE = ex.ToString();
                    LogControl.save("eliminarBeneficiosCotTrx", ex.ToString(), "3");
                }
            }

            //List<OracleParameter> parameter = new List<OracleParameter>();
            //string storeprocedure = ProcedureName.pkg_ReaDataAP + ".DEL_BENEFIT_COT";

            //try
            //{
            //    parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.codProceso, ParameterDirection.Input));

            //    OracleParameter P_COD_ERR = new OracleParameter("P_ERROR", OracleDbType.Int32, ParameterDirection.Output);
            //    OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

            //    P_COD_ERR.Size = 9000;
            //    P_MESSAGE.Size = 9000;

            //    parameter.Add(P_COD_ERR);
            //    parameter.Add(P_MESSAGE);

            //    this.ExecuteByStoredProcedureVT_TRX(storeprocedure, parameter, connection, trx);

            //    response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
            //    response.P_MESSAGE = Convert.ToString(P_MESSAGE.Value.ToString());
            //}
            //catch (Exception ex)
            //{
            //    response.P_COD_ERR = 1;
            //    response.P_MESSAGE = ex.ToString();
            //    ELog.save(this, ex);
            //}

            return response;
        }

        public SalidaErrorBaseVM eliminarRecargosCotTrx(validaTramaVM request, OracleConnection connection, OracleTransaction trx)
        {
            SalidaErrorBaseVM response = new SalidaErrorBaseVM();

            using (OracleCommand cmd = new OracleCommand())
            {
                try
                {
                    OracleDataReader dr;

                    cmd.Connection = connection;
                    cmd.CommandText = ProcedureName.pkg_ReaDataAP + ".DEL_SURCHARGE_COT";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Transaction = trx;

                    cmd.Parameters.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.codProceso, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_NCONDITION", OracleDbType.Int32, request.condicion, ParameterDirection.Input));

                    var P_COD_ERR = new OracleParameter("P_ERROR", OracleDbType.Int32, ParameterDirection.Output);
                    var P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                    P_MESSAGE.Size = 9000;

                    cmd.Parameters.Add(P_COD_ERR);
                    cmd.Parameters.Add(P_MESSAGE);

                    dr = cmd.ExecuteReader();

                    response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                    response.P_MESSAGE = P_MESSAGE.Value.ToString();
                }
                catch (Exception ex)
                {
                    response.P_COD_ERR = 1;
                    response.P_MESSAGE = ex.ToString();
                    LogControl.save("eliminarRecargosCotTrx", ex.ToString(), "3");
                }
            }


            //List<OracleParameter> parameter = new List<OracleParameter>();
            //string storeprocedure = ProcedureName.pkg_ReaDataAP + ".DEL_SURCHARGE_COT";

            //try
            //{
            //    parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.codProceso, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_NCONDITION", OracleDbType.Int32, request.condicion, ParameterDirection.Input));

            //    OracleParameter P_COD_ERR = new OracleParameter("P_ERROR", OracleDbType.Int32, ParameterDirection.Output);
            //    OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

            //    P_COD_ERR.Size = 9000;
            //    P_MESSAGE.Size = 9000;

            //    parameter.Add(P_COD_ERR);
            //    parameter.Add(P_MESSAGE);

            //    this.ExecuteByStoredProcedureVT_TRX(storeprocedure, parameter, connection, trx);

            //    response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
            //    response.P_MESSAGE = Convert.ToString(P_MESSAGE.Value.ToString());
            //}
            //catch (Exception ex)
            //{
            //    response.P_COD_ERR = 1;
            //    response.P_MESSAGE = ex.ToString();
            //    ELog.save(this, ex);
            //}

            return response;
        }

        public SalidaErrorBaseVM eliminarServiciosAdicionalesCotTrx(validaTramaVM request, OracleConnection connection, OracleTransaction trx)
        {
            SalidaErrorBaseVM response = new SalidaErrorBaseVM();

            using (OracleCommand cmd = new OracleCommand())
            {
                try
                {
                    OracleDataReader dr;

                    cmd.Connection = connection;
                    cmd.CommandText = ProcedureName.pkg_ReaDataAP + ".DEL_ADDITIONAL_SERVICES_COT";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Transaction = trx;

                    cmd.Parameters.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.codProceso, ParameterDirection.Input));

                    var P_COD_ERR = new OracleParameter("P_ERROR", OracleDbType.Int32, ParameterDirection.Output);
                    var P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                    P_MESSAGE.Size = 9000;

                    cmd.Parameters.Add(P_COD_ERR);
                    cmd.Parameters.Add(P_MESSAGE);

                    dr = cmd.ExecuteReader();

                    response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                    response.P_MESSAGE = P_MESSAGE.Value.ToString();
                }
                catch (Exception ex)
                {
                    response.P_COD_ERR = 1;
                    response.P_MESSAGE = ex.ToString();
                    LogControl.save("eliminarServiciosAdicionalesCotTrx", ex.ToString(), "3");
                }
            }

            return response;
        }


        public SalidaErrorBaseVM eliminarExclusionsCotTrx(validaTramaVM request, OracleConnection connection, OracleTransaction trx)
        {
            SalidaErrorBaseVM response = new SalidaErrorBaseVM();

            using (OracleCommand cmd = new OracleCommand())
            {
                try
                {
                    OracleDataReader dr;

                    cmd.Connection = connection;
                    cmd.CommandText = ProcedureName.pkg_ReaDataAP + ".DEL_EXCLUSION_COT";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Transaction = trx;

                    cmd.Parameters.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.codProceso, ParameterDirection.Input));

                    var P_COD_ERR = new OracleParameter("P_ERROR", OracleDbType.Int32, ParameterDirection.Output);
                    var P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                    P_MESSAGE.Size = 9000;

                    cmd.Parameters.Add(P_COD_ERR);
                    cmd.Parameters.Add(P_MESSAGE);

                    dr = cmd.ExecuteReader();

                    response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                    response.P_MESSAGE = P_MESSAGE.Value.ToString();
                }
                catch (Exception ex)
                {
                    response.P_COD_ERR = 1;
                    response.P_MESSAGE = ex.ToString();
                    LogControl.save("eliminarExclusionsCotTrx", ex.ToString(), "3");
                }
            }


            //List<OracleParameter> parameter = new List<OracleParameter>();
            //string storeprocedure = ProcedureName.pkg_ReaDataAP + ".DEL_EXCLUSION_COT";

            //try
            //{
            //    parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.codProceso, ParameterDirection.Input));

            //    OracleParameter P_COD_ERR = new OracleParameter("P_ERROR", OracleDbType.Int32, ParameterDirection.Output);
            //    OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

            //    P_COD_ERR.Size = 9000;
            //    P_MESSAGE.Size = 9000;

            //    parameter.Add(P_COD_ERR);
            //    parameter.Add(P_MESSAGE);

            //    this.ExecuteByStoredProcedureVT_TRX(storeprocedure, parameter, connection, trx);

            //    response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
            //    response.P_MESSAGE = Convert.ToString(P_MESSAGE.Value.ToString());
            //}
            //catch (Exception ex)
            //{
            //    response.P_COD_ERR = 1;
            //    response.P_MESSAGE = ex.ToString();
            //    ELog.save(this, ex);
            //}

            return response;
        }

        public SalidaErrorBaseVM eliminarReajustesCotTrx(validaTramaVM request, OracleConnection connection, OracleTransaction trx)
        {
            SalidaErrorBaseVM response = new SalidaErrorBaseVM();

            using (OracleCommand cmd = new OracleCommand())
            {
                try
                {
                    OracleDataReader dr;

                    cmd.Connection = connection;
                    cmd.CommandText = ProcedureName.pkg_ReaDataAP + ".DEL_ADJUSTMENT_FACTOR_COT";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Transaction = trx;

                    cmd.Parameters.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.codProceso, ParameterDirection.Input));

                    var P_COD_ERR = new OracleParameter("P_ERROR", OracleDbType.Int32, ParameterDirection.Output);
                    var P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                    P_MESSAGE.Size = 9000;

                    cmd.Parameters.Add(P_COD_ERR);
                    cmd.Parameters.Add(P_MESSAGE);

                    dr = cmd.ExecuteReader();

                    response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                    response.P_MESSAGE = P_MESSAGE.Value.ToString();
                }
                catch (Exception ex)
                {
                    response.P_COD_ERR = 1;
                    response.P_MESSAGE = ex.ToString();
                    LogControl.save("eliminarReajustesCotTrx", ex.ToString(), "3");
                }
            }

            //List<OracleParameter> parameter = new List<OracleParameter>();
            //string storeprocedure = ProcedureName.pkg_ReaDataAP + ".DEL_ADJUSTMENT_FACTOR_COT";

            //try
            //{
            //    parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.codProceso, ParameterDirection.Input));

            //    OracleParameter P_COD_ERR = new OracleParameter("P_ERROR", OracleDbType.Int32, ParameterDirection.Output);
            //    OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

            //    P_COD_ERR.Size = 9000;
            //    P_MESSAGE.Size = 9000;

            //    parameter.Add(P_COD_ERR);
            //    parameter.Add(P_MESSAGE);

            //    this.ExecuteByStoredProcedureVT_TRX(storeprocedure, parameter, connection, trx);

            //    response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
            //    response.P_MESSAGE = Convert.ToString(P_MESSAGE.Value.ToString());
            //}
            //catch (Exception ex)
            //{
            //    response.P_COD_ERR = 1;
            //    response.P_MESSAGE = ex.ToString();
            //    ELog.save(this, ex);
            //}

            return response;
        }

        public SalidaErrorBaseVM regCoberturasCotTrx(validaTramaVM request, Entities.QuotationModel.BindingModel.coberturaPropuesta cobertura, OracleConnection connection, OracleTransaction trx)
        {
            var response = new SalidaErrorBaseVM();

            //bool edadBase = new string[] { ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch") }.Contains(request.datosPoliza.branch.ToString()) ? true : false;

            using (OracleCommand cmd = new OracleCommand())
            {
                try
                {
                    OracleDataReader dr;

                    cmd.Connection = connection;
                    cmd.CommandText = ProcedureName.pkg_ReaDataAP + ".INS_COVER_COT";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Transaction = trx;

                    cmd.Parameters.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.codProceso, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, request.datosPoliza.branch, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, request.datosPoliza.codTipoProducto, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_NCURRENCY", OracleDbType.Int32, request.datosPoliza.codMon, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_NTYPE_PRODUCT", OracleDbType.Int32, request.datosPoliza.codTipoNegocio, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_NTYPE_PROFILE", OracleDbType.Int32, request.datosPoliza.codTipoPerfil, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, ELog.obtainConfig("planMaestro" + request.datosPoliza.branch), ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_STYPE_TRANSAC", OracleDbType.Varchar2, request.datosPoliza.trxCode, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_NCOVERGEN", OracleDbType.Int64, cobertura.codCobertura, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_NCAPITAL", OracleDbType.Double, cobertura.suma_asegurada, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_NCAPITAL_PRO", OracleDbType.Double, cobertura.sumaPropuesta, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_NCAPITAL_MAX", OracleDbType.Double, cobertura.capital_max, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_NCAPITAL_MIN", OracleDbType.Double, cobertura.capital_min, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_NCAPITAL_AUT", OracleDbType.Double, cobertura.sumaPropuesta /*cobertura.capital_aut*/, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_SCOVERUSE", OracleDbType.Varchar2, cobertura.cobertura_pri, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_NPERCCOVERED", OracleDbType.Double, cobertura.capitalCovered, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_NAGEMINPER", OracleDbType.Int32, cobertura.stayAge == null ? 0 : cobertura.stayAge.min, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_NAGEMAXPER", OracleDbType.Int32, cobertura.stayAge == null ? 0 : cobertura.stayAge.max, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_NAGEMINING", OracleDbType.Int32, cobertura.entryAge == null ? 0 : cobertura.entryAge.min, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_NAGEMAXING", OracleDbType.Int32, cobertura.entryAge == null ? 0 : cobertura.entryAge.max, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Date, request.datosPoliza.InicioVigPoliza, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int64, request.codUsuario, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_NLIMIT", OracleDbType.Double, cobertura.limit, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_NHOUR", OracleDbType.Int64, cobertura.hours, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_SDESCRIPT", OracleDbType.Varchar2, cobertura.descripcion, ParameterDirection.Input));

                    var P_COD_ERR = new OracleParameter("P_ERROR", OracleDbType.Int32, ParameterDirection.Output);
                    var P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                    P_MESSAGE.Size = 9000;

                    cmd.Parameters.Add(P_COD_ERR);
                    cmd.Parameters.Add(P_MESSAGE);

                    cmd.Parameters.Add(new OracleParameter("P_NFACTOR", OracleDbType.Varchar2, cobertura.nfactor, ParameterDirection.Input));

                    //AVS - RENTAS
                    var xd = new string[] { "FIXED_PENSION_QUANTITY", "MAXIMUM_PENSION_QUANTITY" }.Contains(cobertura.id_cover) ? cobertura.sumaPropuesta : cobertura.pension_prop;
                    cmd.Parameters.Add(new OracleParameter("P_ID_COVER", OracleDbType.Varchar2, cobertura.id_cover, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_DES_COVER", OracleDbType.Varchar2, cobertura.sdes_cover, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_PENSION_BASE", OracleDbType.Double, cobertura.pension_base, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_PENSION_MAX", OracleDbType.Double, cobertura.pension_max, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_PENSION_MIN", OracleDbType.Double, cobertura.pension_min, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_PENSION_PROP", OracleDbType.Double, new string[] { "FIXED_PENSION_QUANTITY", "MAXIMUM_PENSION_QUANTITY" }.Contains(cobertura.id_cover) ? cobertura.sumaPropuesta : cobertura.pension_prop, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_PORCEN_BASE", OracleDbType.Double, cobertura.porcen_base, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_PORCEN_MAX", OracleDbType.Double, cobertura.porcen_max, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_PORCEN_MIN", OracleDbType.Double, cobertura.porcen_min, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_PORCEN_PROP", OracleDbType.Double, cobertura.porcen_prop, ParameterDirection.Input));

                    cmd.Parameters.Add(new OracleParameter("P_LACK_PERIOD", OracleDbType.Int32, cobertura.lackPeriod, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_DEDUCTIBLE", OracleDbType.Double, cobertura.deductible, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_COPAYMENT", OracleDbType.Double, cobertura.copayment, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_MAX_ACCUMULATION", OracleDbType.Double, cobertura.maxAccumulation, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_SCOMMENT", OracleDbType.Varchar2, cobertura.comment, ParameterDirection.Input));
                    ////////////////////////////////////////////////////

                    dr = cmd.ExecuteReader();

                    response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                    response.P_MESSAGE = P_MESSAGE.Value.ToString();
                }
                catch (Exception ex)
                {
                    response.P_COD_ERR = 1;
                    response.P_MESSAGE = "Hubo un error al insertar las coberturas"; // ex.ToString();
                    LogControl.save("regCoberturasCotTrx", ex.ToString(), "3");
                }
            }

            return response;
        }

        public SalidaErrorBaseVM regAsistenciasCotTrx(validaTramaVM request, Entities.QuotationModel.ViewModel.asistenciaPropuesta asistencia, OracleConnection connection, OracleTransaction trx)
        {
            SalidaErrorBaseVM response = new SalidaErrorBaseVM();

            using (OracleCommand cmd = new OracleCommand())
            {
                try
                {
                    OracleDataReader dr;

                    cmd.Connection = connection;
                    cmd.CommandText = ProcedureName.pkg_ReaDataAP + ".INS_ASSISTANCE_COT";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Transaction = trx;

                    cmd.Parameters.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.codProceso, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, request.datosPoliza.branch, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, request.datosPoliza.codTipoProducto, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, ELog.obtainConfig("planMaestro" + request.datosPoliza.branch), ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_NPROVIDER", OracleDbType.Varchar2, asistencia.provider.code, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_NCOD_ASSISTANCE", OracleDbType.Varchar2, asistencia.codAsistencia, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_SDESC_ASSISTANCE", OracleDbType.Varchar2, asistencia.desAssist, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_NPERCENT", OracleDbType.Double, 0, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_NAMOUNT", OracleDbType.Double, 0, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Date, request.datosPoliza.InicioVigPoliza, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, request.datosPoliza.FinVigPoliza, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int64, request.codUsuario, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_URL", OracleDbType.Varchar2, asistencia.document, ParameterDirection.Input));

                    var P_COD_ERR = new OracleParameter("P_ERROR", OracleDbType.Int32, ParameterDirection.Output);
                    var P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                    P_MESSAGE.Size = 9000;

                    cmd.Parameters.Add(P_COD_ERR);
                    cmd.Parameters.Add(P_MESSAGE);

                    cmd.Parameters.Add(new OracleParameter("P_SASSISTUSE", OracleDbType.Varchar2, asistencia.asistencia_pri, ParameterDirection.Input));


                    dr = cmd.ExecuteReader();

                    response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                    response.P_MESSAGE = P_MESSAGE.Value.ToString();
                }
                catch (Exception ex)
                {
                    response.P_COD_ERR = 1;
                    response.P_MESSAGE = "Hubo un error al insertar las asistencias"; // ex.ToString();
                    LogControl.save("regAsistenciasCotTrx", ex.ToString(), "3");
                }
            }



            //List<OracleParameter> parameter = new List<OracleParameter>();
            //string storeprocedure = ProcedureName.pkg_ReaDataAP + ".INS_ASSISTANCE_COT";

            //try
            //{
            //    parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.codProceso, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, request.datosPoliza.branch, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, request.datosPoliza.codTipoProducto, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, ELog.obtainConfig("planMaestro" + request.datosPoliza.branch), ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_NPROVIDER", OracleDbType.Varchar2, asistencia.provider.code, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_NCOD_ASSISTANCE", OracleDbType.Int64, asistencia.codAsistencia, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_SDESC_ASSISTANCE", OracleDbType.Varchar2, asistencia.desAssist, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_NPERCENT", OracleDbType.Double, 0, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_NAMOUNT", OracleDbType.Double, 0, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Date, request.datosPoliza.InicioVigPoliza, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, request.datosPoliza.FinVigPoliza, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int64, request.codUsuario, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_URL", OracleDbType.Varchar2, asistencia.document, ParameterDirection.Input));


            //    OracleParameter P_COD_ERR = new OracleParameter("P_ERROR", OracleDbType.Int32, ParameterDirection.Output);
            //    OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

            //    P_COD_ERR.Size = 9000;
            //    P_MESSAGE.Size = 9000;

            //    parameter.Add(P_COD_ERR);
            //    parameter.Add(P_MESSAGE);

            //    this.ExecuteByStoredProcedureVT_TRX(storeprocedure, parameter, connection, trx);

            //    response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
            //    response.P_MESSAGE = Convert.ToString(P_MESSAGE.Value.ToString());
            //}
            //catch (Exception ex)
            //{
            //    response.P_COD_ERR = 1;
            //    response.P_MESSAGE = "Hubo un error al insertar las asistencias."; // ex.ToString();
            //    ELog.save(this, ex);
            //}

            return response;
        }

        public SalidaErrorBaseVM regBeneficiosCotTrx(validaTramaVM request, Entities.QuotationModel.ViewModel.beneficioPropuesto beneficio, OracleConnection connection, OracleTransaction trx)
        {
            SalidaErrorBaseVM response = new SalidaErrorBaseVM();

            using (OracleCommand cmd = new OracleCommand())
            {
                try
                {
                    OracleDataReader dr;

                    cmd.Connection = connection;
                    cmd.CommandText = ProcedureName.pkg_ReaDataAP + ".INS_BENEFIT_COT";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Transaction = trx;

                    cmd.Parameters.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.codProceso, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, request.datosPoliza.branch, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, request.datosPoliza.codTipoProducto, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, ELog.obtainConfig("planMaestro" + request.datosPoliza.branch), ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_NCOD_BENEFIT", OracleDbType.Varchar2, beneficio.codBeneficio, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_SDESC_BENEFIT", OracleDbType.Varchar2, beneficio.desBenefit, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_NPERCENT", OracleDbType.Int64, 0, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_NAMOUNT", OracleDbType.Int64, 0, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Date, request.datosPoliza.InicioVigPoliza, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, request.datosPoliza.FinVigPoliza, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int64, request.codUsuario, ParameterDirection.Input));

                    var P_COD_ERR = new OracleParameter("P_ERROR", OracleDbType.Int32, ParameterDirection.Output);
                    var P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                    P_MESSAGE.Size = 9000;

                    cmd.Parameters.Add(P_COD_ERR);
                    cmd.Parameters.Add(P_MESSAGE);

                    cmd.Parameters.Add(new OracleParameter("P_SBENEFITUSE", OracleDbType.Varchar2, beneficio.beneficio_pri, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_EXEC_BENEFIT", OracleDbType.Int32, beneficio.exc_beneficio, ParameterDirection.Input)); //AVS - RENTAS
                    cmd.Parameters.Add(new OracleParameter("P_STUDENTRENTBENEFIT", OracleDbType.Varchar2, beneficio.studentRentBenefit, ParameterDirection.Input)); //AVS - RENTAS

                    dr = cmd.ExecuteReader();

                    response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                    response.P_MESSAGE = P_MESSAGE.Value.ToString();
                }
                catch (Exception ex)
                {
                    response.P_COD_ERR = 1;
                    response.P_MESSAGE = "Hubo un error al insertar los beneficios"; // ex.ToString();
                    LogControl.save("regBeneficiosCotTrx", ex.ToString(), "3");
                }
            }


            //List<OracleParameter> parameter = new List<OracleParameter>();
            //string storeprocedure = ProcedureName.pkg_ReaDataAP + ".INS_BENEFIT_COT";

            //try
            //{
            //    parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.codProceso, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, request.datosPoliza.branch, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, request.datosPoliza.codTipoProducto, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, ELog.obtainConfig("planMaestro" + request.datosPoliza.branch), ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_NCOD_BENEFIT", OracleDbType.Int64, beneficio.codBeneficio, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_SDESC_BENEFIT", OracleDbType.Varchar2, beneficio.desBenefit, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_NPERCENT", OracleDbType.Int64, 0, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_NAMOUNT", OracleDbType.Int64, 0, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Date, request.datosPoliza.InicioVigPoliza, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, request.datosPoliza.FinVigPoliza, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int64, request.codUsuario, ParameterDirection.Input));

            //    OracleParameter P_COD_ERR = new OracleParameter("P_ERROR", OracleDbType.Int32, ParameterDirection.Output);
            //    OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

            //    P_COD_ERR.Size = 9000;
            //    P_MESSAGE.Size = 9000;

            //    parameter.Add(P_COD_ERR);
            //    parameter.Add(P_MESSAGE);

            //    this.ExecuteByStoredProcedureVT_TRX(storeprocedure, parameter, connection, trx);

            //    response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
            //    response.P_MESSAGE = Convert.ToString(P_MESSAGE.Value.ToString());
            //}
            //catch (Exception ex)
            //{
            //    response.P_COD_ERR = 1;
            //    response.P_MESSAGE = "Hubo un error al insertar los beneficios"; // ex.ToString();
            //    ELog.save(this, ex);
            //}

            return response;
        }

        public SalidaErrorBaseVM regServiciosAdicionalesCotTrx(validaTramaVM request, Entities.QuotationModel.ViewModel.servicioAdicionalPropuesto servicioAdicional, OracleConnection connection, OracleTransaction trx)
        {
            SalidaErrorBaseVM response = new SalidaErrorBaseVM();

            using (OracleCommand cmd = new OracleCommand())
            {
                try
                {
                    OracleDataReader dr;

                    cmd.Connection = connection;
                    cmd.CommandText = ProcedureName.pkg_ReaDataAP + ".INS_ADDITIONAL_SERVICES_COT";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Transaction = trx;

                    cmd.Parameters.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.codProceso, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, request.datosPoliza.branch, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, request.datosPoliza.codTipoProducto, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, ELog.obtainConfig("planMaestro" + request.datosPoliza.branch), ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_NCOD_ADDITIONAL_SERVICES", OracleDbType.Varchar2, servicioAdicional.codServAdicionales, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_SDESC_ADDITIONAL_SERVICES", OracleDbType.Varchar2, servicioAdicional.desServAdicionales, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_NPERCENT", OracleDbType.Int64, 0, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_NHOUR", OracleDbType.Int64, servicioAdicional.amount, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Date, request.datosPoliza.InicioVigPoliza, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, request.datosPoliza.FinVigPoliza, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int64, request.codUsuario, ParameterDirection.Input));

                    var P_COD_ERR = new OracleParameter("P_ERROR", OracleDbType.Int32, ParameterDirection.Output);
                    var P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                    P_MESSAGE.Size = 9000;

                    cmd.Parameters.Add(P_COD_ERR);
                    cmd.Parameters.Add(P_MESSAGE);

                    dr = cmd.ExecuteReader();

                    response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                    response.P_MESSAGE = P_MESSAGE.Value.ToString();
                }
                catch (Exception ex)
                {
                    response.P_COD_ERR = 1;
                    response.P_MESSAGE = "Hubo un error al insertar los servicios adicionales"; // ex.ToString();
                    LogControl.save("regServiciosAdicionalesCotTrx", ex.ToString(), "3");
                }
            }

            return response;
        }


        public SalidaErrorBaseVM UpdateCoberturasProp(string covergen, string sumaProp, string codProc, int flag, DbConnection connection, DbTransaction trx)
        {
            SalidaErrorBaseVM response = new SalidaErrorBaseVM();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.pkg_ValidadorGenPD + ".UPD_COV_CAP_PROP";

            try
            {
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, codProc, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCOVERGEN", OracleDbType.Int32, covergen, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_CAPITAL_PROP", OracleDbType.Double, sumaProp, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FLAG", OracleDbType.Int32, flag, ParameterDirection.Input));


                OracleParameter P_COD_ERR = new OracleParameter("P_ERROR", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                P_COD_ERR.Size = 9000;
                P_MESSAGE.Size = 9000;

                parameter.Add(P_COD_ERR);
                parameter.Add(P_MESSAGE);

                this.ExecuteByStoredProcedureVT_TRX(storeprocedure, parameter, connection, trx);

                response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                response.P_MESSAGE = Convert.ToString(P_MESSAGE.Value.ToString());
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = ex.ToString();
                LogControl.save("UpdateCoberturasProp", ex.ToString(), "3");
            }

            return response;
        }

        public SalidaTramaBaseVM ReaListCotiza(string codproceso, string tiporenov, string codProducto, string codRamo)
        {
            SalidaTramaBaseVM response = new SalidaTramaBaseVM();
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<AmountPremiumQuotationVM> lista = new List<AmountPremiumQuotationVM>();
            string storeprocedure = ProcedureName.pkg_ValidadorGenPD + ".SP_REA_COTIZADOR_AP";

            try
            {
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, codproceso, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTIP_RENOV", OracleDbType.Int32, tiporenov, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, codProducto, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, codRamo, ParameterDirection.Input));

                string[] cursores = { "C_AMOUNT_TOTAL", "C_TABLE" };
                OracleParameter P_COD_ERR = new OracleParameter("P_ERROR", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                OracleParameter C_AMOUNT_TOTAL = new OracleParameter(cursores[0], OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter C_TABLE = new OracleParameter(cursores[1], OracleDbType.RefCursor, ParameterDirection.Output);

                P_COD_ERR.Size = 9000;
                P_MESSAGE.Size = 9000;

                parameter.Add(P_COD_ERR);
                parameter.Add(P_MESSAGE);

                parameter.Add(C_AMOUNT_TOTAL);
                parameter.Add(C_TABLE);

                OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storeprocedure, parameter);

                if (dr.HasRows)
                {

                    while (dr.Read())
                    {
                        AmountPremiumQuotationVM item = new AmountPremiumQuotationVM();
                        item.NCATEGORIA = Convert.ToInt32(dr["orden"] == DBNull.Value ? "" : dr["orden"].ToString());
                        item.SCATEGORIA = dr["categoria"] == DBNull.Value ? "" : dr["categoria"].ToString();
                        item.DES_TASA = dr["tasa"] == DBNull.Value ? "" : dr["tasa"].ToString();
                        item.NPREMIUMN_MEN = Convert.ToDouble(dr["npremiumn_men"] == DBNull.Value ? "0" : dr["npremiumn_men"].ToString());
                        item.NPREMIUMN_BIM = Convert.ToDouble(dr["npremiumn_bim"] == DBNull.Value ? "0" : dr["npremiumn_bim"].ToString());
                        item.NPREMIUMN_TRI = Convert.ToDouble(dr["npremiumn_tri"] == DBNull.Value ? "0" : dr["npremiumn_tri"].ToString());
                        item.NPREMIUMN_SEM = Convert.ToDouble(dr["npremiumn_sem"] == DBNull.Value ? "0" : dr["npremiumn_sem"].ToString());
                        item.NPREMIUMN_ANU = Convert.ToDouble(dr["npremiumn_anu"] == DBNull.Value ? "0" : dr["npremiumn_anu"].ToString());
                        item.NPREMIUMN_ESP = Convert.ToDouble(dr["npremiumn_esp"] == DBNull.Value ? "0" : dr["npremiumn_esp"].ToString());
                        lista.Add(item);
                    }

                    dr.NextResult();

                    while (dr.Read())
                    {
                        response.SUM_ASEGURADA = Convert.ToDouble(dr["SUMA_ASEGURADA"] == DBNull.Value ? "0" : dr["SUMA_ASEGURADA"].ToString());
                        response.TOT_ASEGURADOS = dr["TOT_ASEGURADOS"] == DBNull.Value ? 0 : Convert.ToInt32(dr["TOT_ASEGURADOS"].ToString());
                        response.PRIMA = Convert.ToDouble(dr["PREMIUM"] == DBNull.Value ? "0" : dr["PREMIUM"].ToString());
                    }
                }
                response.amountPremiumList = lista;
                response.P_COD_ERR = Convert.ToString(P_COD_ERR.Value.ToString());
                response.P_MESSAGE = Convert.ToString(P_MESSAGE.Value.ToString());

                ELog.CloseConnection(dr);
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = "1";
                response.P_MESSAGE = ex.ToString();
                LogControl.save("ReaListCotiza", ex.ToString(), "3");
            }

            return response;
        }

        public SalidaPremiumVM ReaListDetCotiza(string idproc, string tipoplan, string codProducto, string codRamo, string flagCot)
        {
            var response = new SalidaPremiumVM();

            string storeprocedure = flagCot == "1" ? ProcedureName.pkg_ValidadorGenPD + ".SP_REA_COT_DET_AP_PM" : ProcedureName.pkg_ValidadorGenPD + ".SP_REA_COT_DET_AP";

            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        OracleDataReader dr;

                        cmd.Connection = cn;
                        cmd.CommandText = storeprocedure;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, idproc, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_SPLAN", OracleDbType.Varchar2, tipoplan, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, codProducto, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, codRamo, ParameterDirection.Input));

                        var P_COD_ERR = new OracleParameter("P_ERROR", OracleDbType.Int32, ParameterDirection.Output);
                        var P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                        P_MESSAGE.Size = 4000;

                        cmd.Parameters.Add(P_COD_ERR);
                        cmd.Parameters.Add(P_MESSAGE);

                        cmd.Parameters.Add(new OracleParameter("C_TABLE_ANU", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(new OracleParameter("C_TABLE_PROP", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(new OracleParameter("C_TABLE_AUT", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(new OracleParameter("C_TABLE_PARAM", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;


                        cn.Open();
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                response.TOT_ASEGURADOS = dr["TOT_ASEGURADOS"] == DBNull.Value ? "" : dr["TOT_ASEGURADOS"].ToString();
                                response.NMONTO_PLANILLA = Convert.ToDouble(dr["NMONTO_PLANILLA"] == DBNull.Value ? "0" : dr["NMONTO_PLANILLA"].ToString());
                                response.NTASA = Convert.ToDouble(dr["NTASA"] == DBNull.Value ? "0" : dr["NTASA"].ToString());
                                response.PRIMA = Convert.ToDouble(dr["PREMIUM"] == DBNull.Value ? "0" : dr["PREMIUM"].ToString());
                                response.PRIMA_ASEG = Convert.ToDouble(dr["NPREMIUMN_ASEG"] == DBNull.Value ? "0" : dr["NPREMIUMN_ASEG"].ToString());
                                response.IGV_ASEG = Convert.ToDouble(dr["NIGV_ASEG"] == DBNull.Value ? "0" : dr["NIGV_ASEG"].ToString());
                                response.PRIMAT_ASEG = Convert.ToDouble(dr["NPREMIUM_ASEG"] == DBNull.Value ? "0" : dr["NPREMIUM_ASEG"].ToString());
                            }

                            dr.NextResult();

                            while (dr.Read())
                            {
                                AmountPremiumList item = new AmountPremiumList();
                                item.NCATEGORIA = Convert.ToInt32(dr["orden"] == DBNull.Value ? "0" : dr["orden"].ToString());
                                item.CAPITAL = Convert.ToDouble(dr["capital"] == DBNull.Value ? "0" : dr["capital"].ToString());
                                item.DES_TASA = dr["tasa"] == DBNull.Value ? "" : dr["tasa"].ToString();
                                item.NPREMIUMN_ANU = Convert.ToDouble(dr["npremiumn_anu"] == DBNull.Value ? "0" : dr["npremiumn_anu"].ToString());
                                response.amountPremiumAnu.Add(item);
                            }

                            dr.NextResult();

                            while (dr.Read())
                            {
                                AmountPremiumList item2 = new AmountPremiumList();
                                item2.NCATEGORIA = Convert.ToInt32(dr["orden"] == DBNull.Value ? "0" : dr["orden"].ToString());
                                item2.CAPITAL = Convert.ToDouble(dr["capital"] == DBNull.Value ? "0" : dr["capital"].ToString());
                                item2.DES_TASA = dr["tasa"] == DBNull.Value ? "" : dr["tasa"].ToString();
                                item2.NPREMIUMN_ANU = Convert.ToDouble(dr["npremiumn_anu"] == DBNull.Value ? "0" : dr["npremiumn_anu"].ToString());
                                response.amountPremiumProp.Add(item2);
                            }

                            dr.NextResult();

                            while (dr.Read())
                            {
                                AmountPremiumList item3 = new AmountPremiumList();
                                item3.NCATEGORIA = Convert.ToInt32(dr["orden"] == DBNull.Value ? "0" : dr["orden"].ToString());
                                item3.CAPITAL = Convert.ToDouble(dr["capital"] == DBNull.Value ? "0" : dr["capital"].ToString());
                                item3.DES_TASA = dr["tasa"] == DBNull.Value ? "" : dr["tasa"].ToString();
                                item3.NPREMIUMN_ANU = Convert.ToDouble(dr["npremiumn_anu"] == DBNull.Value ? "0" : dr["npremiumn_anu"].ToString());
                                response.amountPremiumAut.Add(item3);
                            }
                        }

                        ELog.CloseConnection(dr);

                        response.P_COD_ERR = Convert.ToString(P_COD_ERR.Value.ToString());
                        response.P_MESSAGE = Convert.ToString(P_MESSAGE.Value.ToString());
                    }
                    catch (Exception ex)
                    {
                        response.P_COD_ERR = "1";
                        response.P_MESSAGE = ex.ToString();
                        LogControl.save("ReaListDetCotiza", ex.ToString(), "3");
                    }
                    finally
                    {
                        if (cn.State == ConnectionState.Open) cn.Close();
                    }
                }
            }

            return response;
        }

        public SalidaTramaBaseVM ReaListDetPremiumAut(string idproc)
        {
            var response = new SalidaTramaBaseVM();

            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        OracleDataReader dr;

                        cmd.Connection = cn;
                        cmd.CommandText = ProcedureName.pkg_ValidadorGenPD + ".SP_REA_COT_PREMIUM_AUT";
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new OracleParameter("P_NID_PROD", OracleDbType.Varchar2, idproc, ParameterDirection.Input));

                        var P_COD_ERR = new OracleParameter("P_ERROR", OracleDbType.Int32, ParameterDirection.Output);
                        var P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                        P_MESSAGE.Size = 4000;

                        cmd.Parameters.Add(P_COD_ERR);
                        cmd.Parameters.Add(P_MESSAGE);

                        cmd.Parameters.Add(new OracleParameter("C_TABLE_AUT", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;

                        cn.Open();
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                AmountPremiumList item = new AmountPremiumList();
                                item.NCATEGORIA = Convert.ToInt32(dr["orden"] == DBNull.Value ? "" : dr["orden"].ToString());
                                item.CAPITAL = Convert.ToDouble(dr["capital"] == DBNull.Value ? "" : dr["capital"].ToString());
                                item.DES_TASA = dr["tasa"] == DBNull.Value ? "" : dr["tasa"].ToString();
                                item.NPREMIUMN_ANU = Convert.ToDouble(dr["npremiumn_anu"] == DBNull.Value ? "" : dr["npremiumn_anu"].ToString());
                                response.amountPremiumListAut.Add(item);
                            }

                            dr.NextResult();

                            while (dr.Read())
                            {
                                response.TOT_ASEGURADOS = dr["TOT_ASEGURADOS"] == DBNull.Value ? 0 : Convert.ToInt32(dr["TOT_ASEGURADOS"].ToString());
                                response.PRIMA = Convert.ToDouble(dr["PREMIUM"] == DBNull.Value ? "" : dr["PREMIUM"].ToString());
                            }
                        }

                        ELog.CloseConnection(dr);

                        response.P_COD_ERR = Convert.ToString(P_COD_ERR.Value.ToString());
                        response.P_MESSAGE = Convert.ToString(P_MESSAGE.Value.ToString());
                    }
                    catch (Exception ex)
                    {
                        response.P_COD_ERR = "1";
                        response.P_MESSAGE = ex.ToString();
                        LogControl.save("ReaListDetPremiumAut", ex.ToString(), "3");
                    }
                    finally
                    {
                        if (cn.State == ConnectionState.Open) cn.Close();
                    }
                }
            }

            return response;
        }

        public SalidaTramaBaseVM SaveUsingOracleBulkCopy(DataTable dt)
        {
            var result = new SalidaTramaBaseVM() { P_COD_ERR = "0" };
            var tblNameList = new string[] { ProcedureName.tbl_CargaPD_TMP, ProcedureName.tbl_CargaPD_Original };

            try
            {
                if (dt.Rows.Count > 0)
                {
                    foreach (var tblName in tblNameList)
                    {
                        if (result.P_COD_ERR == "0")
                        {
                            result = this.SaveUsingOracleBulkCopy(tblName, dt);
                        }
                    }

                    result = result.P_COD_ERR == "0" ? moveTablePD(dt.Rows[0]["NID_PROC"].ToString(), 0) : result; // Se traslada de temporal a la tabla correcta
                    result = result.P_COD_ERR == "0" ? moveTablePD(dt.Rows[0]["NID_PROC"].ToString(), 1) : result; // Se elimina data de la tabla temporal
                }
                else
                {
                    result.P_COD_ERR = "1";
                    result.P_MESSAGE = "La trama adjuntada tiene el formato incorrecto.";
                }

            }
            catch (Exception ex)
            {
                result.P_COD_ERR = "1";
                result.P_MESSAGE = ex.ToString();
                LogControl.save("SaveUsingOracleBulkCopy", ex.ToString(), "3");
            }

            return result;
        }

        public SalidaTramaBaseVM moveTablePD(string codProcess, int process)
        {
            var result = new SalidaTramaBaseVM() { P_COD_ERR = "0" };

            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        OracleDataReader dr;

                        cmd.Connection = cn;
                        cmd.CommandText = process == 0 ? ProcedureName.sp_InsCargaSctr : ProcedureName.sp_DelCargaSctrTMP;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, codProcess, ParameterDirection.Input));

                        var P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);
                        var P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);

                        cmd.Parameters.Add(P_NCODE);
                        cmd.Parameters.Add(P_SMESSAGE);

                        cn.Open();
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        result.P_COD_ERR = P_NCODE.Value.ToString();
                        result.P_MESSAGE = P_SMESSAGE.Value.ToString();

                        ELog.CloseConnection(dr);
                    }
                    catch (Exception ex)
                    {
                        result.P_COD_ERR = "1";
                        result.P_MESSAGE = ex.ToString();
                        LogControl.save("moveTablePD", ex.ToString(), "3");
                    }
                    finally
                    {
                        if (cn.State == ConnectionState.Open) cn.Close();
                    }
                }
            }

            return result;
        }

        public GenericResponseVM GetQuotationList(QuotationSearchBM dataToSearch)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            GenericResponseVM resultPackage = new GenericResponseVM();
            List<QuotationVM> list = new List<QuotationVM>();

            string storedProcedureName = ProcedureName.pkg_Cotizacion + ".REA_COTIZA";

            string palabaConTildes = "áéíóúñ";

            string palabaSinTildes = Regex.Replace(palabaConTildes.Normalize(NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", "");

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Varchar2, dataToSearch.QuotationNumber, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Varchar2, dataToSearch.ProductType, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FECHA_DESDE", OracleDbType.Date, dataToSearch.StartDate, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FECHA_HASTA", OracleDbType.Date, dataToSearch.EndDate, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SSTATREGT", OracleDbType.Varchar2, dataToSearch.Status, ParameterDirection.Input));

                parameter.Add(new OracleParameter("P_TIPO_BUS_CONT", OracleDbType.Varchar2, dataToSearch.ContractorSearchMode, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TIPO_DOC_CONT", OracleDbType.Varchar2, dataToSearch.ContractorDocumentType, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUM_DOC_CONT", OracleDbType.Varchar2, dataToSearch.ContractorDocumentNumber, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_RAZON_SOCIAL_CONT", OracleDbType.Varchar2, dataToSearch.ContractorLegalName, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_APE_PAT_CONT", OracleDbType.Varchar2, dataToSearch.ContractorPaternalLastName, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_APE_MAT_CONT", OracleDbType.Varchar2, dataToSearch.ContractorMaternalLastName, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NOMBRES_CONT", OracleDbType.Varchar2, dataToSearch.ContractorFirstName, ParameterDirection.Input));

                parameter.Add(new OracleParameter("P_TIPO_BUS_BR", OracleDbType.Varchar2, dataToSearch.BrokerSearchMode, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TIPO_DOC_BR", OracleDbType.Varchar2, dataToSearch.BrokerDocumentType, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUM_DOC_BR", OracleDbType.Varchar2, dataToSearch.BrokerDocumentNumber, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_RAZON_SOCIAL_BR", OracleDbType.Varchar2, dataToSearch.BrokerLegalName, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_APE_PAT_BR", OracleDbType.Varchar2, dataToSearch.BrokerPaternalLastName, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_APE_MAT_BR", OracleDbType.Varchar2, dataToSearch.BrokerMaternalLastName, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NOMBRES_BR", OracleDbType.Varchar2, dataToSearch.BrokerFirstName, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Varchar2, dataToSearch.User, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLIMITPERPAGE", OracleDbType.Int32, dataToSearch.LimitPerPage, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPAGENUM", OracleDbType.Int32, dataToSearch.PageNumber, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCOMPANY_LNK", OracleDbType.Int32, dataToSearch.CompanyLNK, ParameterDirection.Input)); // JDD
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, dataToSearch.Nbranch, ParameterDirection.Input)); // JDD
                OracleParameter table = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(table);
                OracleParameter totalRowNumber = new OracleParameter("P_NTOTALROWS", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameter.Add(totalRowNumber);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                while (odr.Read())
                {
                    QuotationVM item = new QuotationVM();
                    item.ProductId = odr["ID_PRODUCTO"] == DBNull.Value ? "" : odr["ID_PRODUCTO"].ToString();
                    item.ProductName = odr["NOMBRE_PRODUCT"] == DBNull.Value ? "" : odr["NOMBRE_PRODUCT"].ToString();
                    item.QuotationNumber = odr["NUM_COTIZACION"] == DBNull.Value ? "" : odr["NUM_COTIZACION"].ToString();
                    item.ContractorFullName = odr["NOMBRE_CONTRATANTE"] == DBNull.Value ? "" : odr["NOMBRE_CONTRATANTE"].ToString();
                    item.MinimalPremium = odr["PRIMA_MINIMA"] == DBNull.Value ? "" : odr["PRIMA_MINIMA"].ToString();
                    item.WorkersCount = odr["NOM_TOTAL_TRAB"] == DBNull.Value ? "" : odr["NOM_TOTAL_TRAB"].ToString();
                    item.Payroll = odr["PLANILLA"] == DBNull.Value ? "" : odr["PLANILLA"].ToString();
                    item.Rate = odr["TASA"] == DBNull.Value ? "" : odr["TASA"].ToString();
                    item.BrokerFullName = odr["CLIENT_BROKER"] == DBNull.Value ? "" : odr["CLIENT_BROKER"].ToString();
                    item.Bounty = odr["COMISION"] == DBNull.Value ? "" : odr["COMISION"].ToString();
                    item.Creator = odr["CREADOR"] == DBNull.Value ? "" : odr["CREADOR"].ToString();
                    item.ruta = odr["RUTA_COT"] == DBNull.Value ? "" : odr["RUTA_COT"].ToString();
                    item.PerfilCreator = odr["PERFIL_CREADOR"] == DBNull.Value ? "" : odr["PERFIL_CREADOR"].ToString();
                    item.PolicyNumber = odr["POLIZA"] == DBNull.Value ? "" : odr["POLIZA"].ToString();
                    item.ID_ESTADO = odr["ID_ESTADO"] == DBNull.Value ? 0 : Convert.ToInt32(odr["ID_ESTADO"]);
                    item.Status = odr["DES_ESTADO"] == DBNull.Value ? "" : odr["DES_ESTADO"].ToString();
                    item.ApprovalDate = odr["FECHA_APROBACION"] != null && odr["FECHA_APROBACION"].ToString().Trim() != "" ? Convert.ToDateTime(odr["FECHA_APROBACION"].ToString()) : (DateTime?)null;
                    item.LINEA = odr["LINEA"] == DBNull.Value ? 0 : Convert.ToInt32(odr["LINEA"]);
                    item.Mode = odr["MODO"] == DBNull.Value ? "" : odr["MODO"].ToString();
                    item.TramiteProcess = odr["TRAMITE_PROCESS"] == DBNull.Value ? 0 : Convert.ToInt32(odr["TRAMITE_PROCESS"].ToString()); // tramite ehh
                    item.TypeTransac = odr["STRANSAC"] == DBNull.Value ? "" : Regex.Replace(odr["STRANSAC"].ToString().Normalize(NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", "");
                    item.NCOMPANY_LNK = odr["NCOMPANY_LNK"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NCOMPANY_LNK"]);
                    item.SDES_COMPANY_LNK = odr["SDES_COMPANY_LNK"] == DBNull.Value ? "" : odr["SDES_COMPANY_LNK"].ToString();
                    item.SCOTIZA_LNK = odr["SCOTIZA_LNK"] == DBNull.Value ? "" : odr["SCOTIZA_LNK"].ToString();
                    item.codPlan = odr["NIDPLAN"] == DBNull.Value ? "" : odr["NIDPLAN"].ToString(); // Covid
                    item.desPlan = odr["SDES_PLAN"] == DBNull.Value ? "" : odr["SDES_PLAN"].ToString(); // Covid
                    item.Rutas_archivo = !String.IsNullOrEmpty(odr["RUTA_COT"].ToString()) ? sharedMethods.GetFilePathList(odr["RUTA_COT"].ToString()) : new List<string>();
                    item.PRIMA_TOTAL = odr["PRIMA_TOTAL"] == DBNull.Value ? 0 : Convert.ToDouble(odr["PRIMA_TOTAL"]);
                    item.NCLIENT_SEG = odr["NCLIENT_SEG"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NCLIENT_SEG"]);
                    item.SCLIENT_SEG = odr["SCLIENT_SEG"] == DBNull.Value ? string.Empty : odr["SCLIENT_SEG"].ToString();
                    item.SDESCRIPT_SEG = odr["SDESCRIPT_SEG"] == DBNull.Value ? string.Empty : odr["SDESCRIPT_SEG"].ToString();
                    item.NTIEMPO_TOTAL_SLA = odr["NTIEMPO_TOTAL_SLA"] == DBNull.Value ? "" : odr["NTIEMPO_TOTAL_SLA"].ToString();
                    item.NFLAG_EXTERNO = odr["NFLAG_EXTERNO"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NFLAG_EXTERNO"]);
                    item.SCIP_ID = odr["SCIP_ID"] == DBNull.Value ? string.Empty : odr["SCIP_ID"].ToString(); // JRIOS 
                    item.NCIP_STATUS = odr["NCIP_STATUS"] == DBNull.Value ? string.Empty : odr["NCIP_STATUS"].ToString(); // JRIOS 
                    item.NCOT_MIXTA = odr["NCOT_MIXTA"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NCOT_MIXTA"]);
                    item.NCIP_STATUS_PEN = odr["NCIP_STATUS_PEN"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NCIP_STATUS_PEN"]);
                    item.SCIP_NUMBER_PEN = odr["SCIP_NUMBER_PEN"] == DBNull.Value ? 0 : Convert.ToInt64(odr["SCIP_NUMBER_PEN"]);
                    item.NCIP_STATUS_EPS = odr["NCIP_STATUS_EPS"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NCIP_STATUS_EPS"]);
                    item.SCIP_NUMBER_EPS = odr["SCIP_NUMBER_EPS"] == DBNull.Value ? 0 : Convert.ToInt64(odr["SCIP_NUMBER_EPS"]);
                    item.NPOLICY = odr["NPOLICY"] == DBNull.Value ? string.Empty : odr["NPOLICY"].ToString();
                    list.Add(item);
                }
                resultPackage.StatusCode = 0;
                resultPackage.TotalRowNumber = Int32.Parse(totalRowNumber.Value.ToString());
                resultPackage.GenericResponse = list;
                ELog.CloseConnection(odr);

                return resultPackage;
            }
            catch (Exception ex)
            {
                resultPackage.MessageError = ex.ToString();
                LogControl.save("GetQuotationList", ex.ToString(), "3");
                return resultPackage;
            }
        }

        public List<ComisionVM> ComisionGet(int nroCotizacion)
        {
            var sPackageJobs = ProcedureName.pkg_Cotizacion + ".REA_LIST_COMMISSION";

            List<OracleParameter> parameter = new List<OracleParameter>();
            List<ComisionVM> response = new List<ComisionVM>();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, nroCotizacion, ParameterDirection.Input));
                //output
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageJobs, "C_TABLE", parameter);

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        ComisionVM item = new ComisionVM();
                        item.idComision = dr["NTYPE_COMMISSION"] == DBNull.Value ? 0 : int.Parse(dr["NTYPE_COMMISSION"].ToString());
                        item.nroComision = dr["NCOMMISSION"] == DBNull.Value ? String.Empty : dr["NCOMMISSION"].ToString();
                        item.porcentaje = dr["NCOMMISSION_PERCENTAGE"] == DBNull.Value ? 0 : double.Parse(dr["NCOMMISSION_PERCENTAGE"].ToString());
                        response.Add(item);
                    }
                }

                ELog.CloseConnection(dr);
            }
            catch (Exception ex)
            {
                LogControl.save("ComisionGet", ex.ToString(), "3");
            }

            return response;
        }

        public List<GlossVM> GlossGet()
        {
            var sPackageJobs = ProcedureName.pkg_Cotizacion + ".REA_LIST_GLOSS";

            List<OracleParameter> parameter = new List<OracleParameter>();
            List<GlossVM> response = new List<GlossVM>();

            try
            {
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageJobs, "C_TABLE", parameter);

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        GlossVM item = new GlossVM();
                        item.idGloss = dr["NGLOSS_COT"].ToString();
                        item.desGloss = dr["SGLOSS_COT"].ToString();
                        response.Add(item);
                    }
                }

                ELog.CloseConnection(dr);
            }
            catch (Exception ex)
            {
                LogControl.save("GlossGet", ex.ToString(), "3");
            }

            return response;
        }

        public GenericResponseVM GetPolicyList(QuotationSearchBM dataToSearch)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            GenericResponseVM resultPackage = new GenericResponseVM();
            List<QuotationVM> list = new List<QuotationVM>();

            string storedProcedureName = ProcedureName.pkg_Poliza + ".REA_POLIZA_EVALUAR";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Varchar2, dataToSearch.PolicyNumber, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Varchar2, dataToSearch.ProductType, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Varchar2, dataToSearch.Nbranch, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FECHA_DESDE", OracleDbType.Date, dataToSearch.StartDate, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FECHA_HASTA", OracleDbType.Date, dataToSearch.EndDate, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SSTATREGT", OracleDbType.Varchar2, dataToSearch.Status, ParameterDirection.Input));

                parameter.Add(new OracleParameter("P_TIPO_BUS_CONT", OracleDbType.Varchar2, dataToSearch.ContractorSearchMode, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TIPO_DOC_CONT", OracleDbType.Varchar2, dataToSearch.ContractorDocumentType, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUM_DOC_CONT", OracleDbType.Varchar2, dataToSearch.ContractorDocumentNumber, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_RAZON_SOCIAL_CONT", OracleDbType.Varchar2, dataToSearch.ContractorLegalName, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_APE_PAT_CONT", OracleDbType.Varchar2, dataToSearch.ContractorPaternalLastName, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_APE_MAT_CONT", OracleDbType.Varchar2, dataToSearch.ContractorMaternalLastName, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NOMBRES_CONT", OracleDbType.Varchar2, dataToSearch.ContractorFirstName, ParameterDirection.Input));

                parameter.Add(new OracleParameter("P_TIPO_BUS_BR", OracleDbType.Varchar2, dataToSearch.BrokerSearchMode, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TIPO_DOC_BR", OracleDbType.Varchar2, dataToSearch.BrokerDocumentType, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUM_DOC_BR", OracleDbType.Varchar2, dataToSearch.BrokerDocumentNumber, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_RAZON_SOCIAL_BR", OracleDbType.Varchar2, dataToSearch.BrokerLegalName, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_APE_PAT_BR", OracleDbType.Varchar2, dataToSearch.BrokerPaternalLastName, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_APE_MAT_BR", OracleDbType.Varchar2, dataToSearch.BrokerMaternalLastName, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NOMBRES_BR", OracleDbType.Varchar2, dataToSearch.BrokerFirstName, ParameterDirection.Input));

                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Varchar2, dataToSearch.User, ParameterDirection.Input));

                parameter.Add(new OracleParameter("P_NLIMITPERPAGE", OracleDbType.Int32, dataToSearch.LimitPerPage, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPAGENUM", OracleDbType.Int32, dataToSearch.PageNumber, ParameterDirection.Input));

                OracleParameter table = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(table);
                OracleParameter totalRowNumber = new OracleParameter("P_NTOTALROWS", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameter.Add(totalRowNumber);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                while (odr.Read())
                {
                    QuotationVM item = new QuotationVM();
                    item.QuotationNumber = odr["NUM_COTIZACION"].ToString();
                    item.PolicyNumber = odr["POLIZA"].ToString();
                    item.NroProcess = odr["NID_PROC"].ToString();
                    item.TypeTransac = odr["NTYPE_TRANSAC"].ToString();
                    item.ProductName = odr["NOMBRE_PRODUCT"].ToString();

                    item.ContractorFullName = odr["NOMBRE_CONTRATANTE"].ToString();
                    item.BrokerFullName = odr["CLIENT_BROKER"].ToString();

                    item.MinimalPremium = odr["PRIMA_MINIMA"].ToString();
                    item.WorkersCount = odr["NOM_TOTAL_TRAB"].ToString();
                    item.Payroll = odr["PLANILLA"].ToString();
                    item.Rate = odr["TASA"].ToString();

                    item.Bounty = odr["COMISION"].ToString();
                    item.Status = odr["DES_ESTADO"].ToString();

                    item.Status = odr["DES_ESTADO"].ToString();
                    item.Mode = odr["MODO"].ToString();

                    item.ApprovalDate = odr["FECHA_APROBACION"] != null && odr["FECHA_APROBACION"].ToString().Trim() != "" ? Convert.ToDateTime(odr["FECHA_APROBACION"].ToString()) : (DateTime?)null;
                    list.Add(item);

                }
                resultPackage.StatusCode = 0;
                resultPackage.TotalRowNumber = Int32.Parse(totalRowNumber.Value.ToString());
                resultPackage.GenericResponse = list;
                ELog.CloseConnection(odr);

                return resultPackage;
            }
            catch (Exception ex)
            {
                LogControl.save("GetPolicyList", ex.ToString(), "3");
                return resultPackage;
            }
        }

        public GenericResponseVM ChangeStatus(StatusChangeBM data, List<HttpPostedFile> fileList, OracleTransaction externalTransaction)
        {
            GenericResponseVM resultPackage = new GenericResponseVM();
            resultPackage.ErrorMessageList = new List<string>();
            resultPackage.SuccessMessageList = new List<string>();

            var connectionString = ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString();
            string folderPath = ELog.obtainConfig("pathCotizacion") + data.QuotationNumber + "\\" + ELog.obtainConfig("movimiento") + "\\" + DateTime.Now.ToString("yyyyMMddhhmmss") + "\\";

            using (var connection = new OracleConnection(connectionString))
            {
                connection.Open();
                var transaction = connection.BeginTransaction();

                try
                {

                    if (data.RenovateStatus != 1) // AGF 25032024 SCTR retorno de tecnica en una renovacio
                    {
                        resultPackage = insertarHistorial(connection, transaction, data, fileList, folderPath);
                    }
                    else
                    {
                        resultPackage = insertarHistorial(connection, transaction, data, fileList, folderPath);
                        actualizarCotizacion(connection, transaction, data, fileList, folderPath); // AGF SCTR CAMBIO DE ESTADO A APROBADO CUANDO ES UNA RENOVACION Y PROVIENE DE TECNICA
                    }

                    if (resultPackage.StatusCode == 0)
                    {
                        foreach (var subData in data.pensionAuthorizedRateList)
                        {
                            if (resultPackage.StatusCode == 0)
                            {
                                resultPackage = insertarTasasAutorizadas(connection, transaction, data, subData);
                            }
                        }
                    }

                    if (resultPackage.StatusCode == 0 && !Convert.ToBoolean(data.flag))
                    {
                        foreach (var subData in data.saludAuthorizedRateList)
                        {
                            if (resultPackage.StatusCode == 0)
                            {
                                resultPackage = insertarTasasAutorizadas(connection, transaction, data, subData);
                            }
                        }
                    }

                    if (resultPackage.StatusCode == 0)
                    {
                        foreach (BrokerBM broker in data.brokerList)
                        {
                            foreach (BrokerProductBM product in broker.ProductList)
                            {
                                if (resultPackage.StatusCode == 0)
                                {
                                    resultPackage = insertarBrokers(connection, transaction, data, broker, product);
                                }
                            }
                        }
                    }

                    if (fileList.Count > 0 && resultPackage.StatusCode == 0)
                    {
                        if (!Directory.Exists(String.Format(ELog.obtainConfig("pathPrincipal"), folderPath)))
                        {
                            Directory.CreateDirectory(String.Format(ELog.obtainConfig("pathPrincipal"), folderPath));
                        }

                        foreach (var file in fileList)
                        {
                            file.SaveAs(String.Format(ELog.obtainConfig("pathPrincipal"), folderPath) + file.FileName);
                        }
                    }

                    if (resultPackage.StatusCode == 0)
                    {
                        transaction.Commit();
                        transaction.Dispose();
                        connection.Close();
                        connection.Dispose();
                    }
                    else
                    {
                        transaction.Rollback();
                        transaction.Dispose();
                        connection.Close();
                        connection.Dispose();
                    }

                }
                catch (Exception ex)
                {
                    resultPackage.StatusCode = 3;
                    transaction.Rollback();
                    transaction.Dispose();
                    connection.Close();
                    connection.Dispose();
                    LogControl.save("ChangeStatus", ex.ToString(), "3");
                }

            }

            return resultPackage;

        }

        public GenericResponseVM ChangeStatusRenovate(StatusChangeBM data, List<HttpPostedFile> fileList, OracleTransaction externalTransaction)
        {
            GenericResponseVM resultPackage = new GenericResponseVM();
            resultPackage.ErrorMessageList = new List<string>();
            resultPackage.SuccessMessageList = new List<string>();

            //GenericResponseVM resp;

            var connectionString = ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString();
            string folderPath = ELog.obtainConfig("pathCotizacion") + data.QuotationNumber + "\\" + ELog.obtainConfig("movimiento") + "\\" + DateTime.Now.ToString("yyyyMMddhhmmss") + "\\";

            using (var connection = new OracleConnection(connectionString))
            {
                connection.Open();
                var transaction = connection.BeginTransaction();

                try
                {
                    resultPackage = insertarHistorial(connection, transaction, data, fileList, folderPath);
                    actualizarCotizacion(connection, transaction, data, fileList, folderPath);

                    if (resultPackage.StatusCode == 0)
                    {
                        transaction.Commit();
                        transaction.Dispose();
                        connection.Close();
                        connection.Dispose();

                    }
                    else
                    {
                        transaction.Rollback();
                        transaction.Dispose();
                        connection.Close();
                        connection.Dispose();
                    }

                }
                catch (Exception ex)
                {
                    resultPackage.StatusCode = 3;
                    transaction.Rollback();
                    transaction.Dispose();
                    connection.Close();
                    connection.Dispose();
                    LogControl.save("ChangeStatus", ex.ToString(), "3");
                }

            }

            return resultPackage;

        }

        private GenericResponseVM insertarBrokers(OracleConnection connection, OracleTransaction transaction, StatusChangeBM data, BrokerBM broker, BrokerProductBM product)
        {
            var response = new GenericResponseVM();

            try
            {
                DbCommand subCommand = connection.CreateCommand();
                subCommand.CommandType = CommandType.StoredProcedure;
                subCommand.CommandText = ProcedureName.pkg_Cotizacion + ".UPD_COTIZACION_COMER"; //
                subCommand.Transaction = transaction;

                subCommand.Parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.QuotationNumber, ParameterDirection.Input));
                subCommand.Parameters.Add(new OracleParameter("P_CHANNEL", OracleDbType.Int32, broker.Id, ParameterDirection.Input));
                subCommand.Parameters.Add(new OracleParameter("P_PRODUCT", OracleDbType.Int32, product.Product, ParameterDirection.Input));
                subCommand.Parameters.Add(new OracleParameter("P_COMISION_AUT", OracleDbType.Decimal, product.AuthorizedCommission, ParameterDirection.Input));
                subCommand.Parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, data.User, ParameterDirection.Input));
                subCommand.Parameters.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output));
                subCommand.Parameters.Add(new OracleParameter("P_COD_ESTADO", OracleDbType.Int32, 4000, null, ParameterDirection.Output));

                subCommand.ExecuteNonQuery();

                response.StatusCode = Convert.ToInt32(subCommand.Parameters["P_COD_ESTADO"].Value.ToString());
                response.ErrorMessageList = new List<string>();
                response.ErrorMessageList.Add(subCommand.Parameters["P_MENSAJE"].Value.ToString());
            }
            catch (Exception ex)
            {
                response.StatusCode = 3;
                LogControl.save("insertarBrokers", ex.ToString(), "3");
            }

            return response;
        }

        private GenericResponseVM insertarTasasAutorizadas(OracleConnection connection, OracleTransaction transaction, StatusChangeBM data, AuthorizedRateBM subData)
        {
            var response = new GenericResponseVM();

            try
            {
                DbCommand subCommand = connection.CreateCommand();
                subCommand.CommandType = CommandType.StoredProcedure;
                subCommand.CommandText = ProcedureName.pkg_Cotizacion + ".UPD_COTIZACION_DET"; //Nombre de stored procedure para insertar "tasa autorizada"
                subCommand.Transaction = transaction;

                subCommand.Parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.QuotationNumber, ParameterDirection.Input));
                subCommand.Parameters.Add(new OracleParameter("P_ESTADO", OracleDbType.Int32, data.Status, ParameterDirection.Input));
                subCommand.Parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, subData.ProductId, ParameterDirection.Input));
                subCommand.Parameters.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, subData.RiskTypeId, ParameterDirection.Input));
                subCommand.Parameters.Add(new OracleParameter("P_NTASA_AUTO", OracleDbType.Double, subData.AuthorizedRate, ParameterDirection.Input));
                subCommand.Parameters.Add(new OracleParameter("P_NPREMIUM_MEN_AUT", OracleDbType.Double, subData.AuthorizedPremium, ParameterDirection.Input));
                subCommand.Parameters.Add(new OracleParameter("P_MIN_PREMIUM_AUT", OracleDbType.Double, subData.AuthorizedMinimunPremium, ParameterDirection.Input));
                subCommand.Parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, data.User, ParameterDirection.Input));

                subCommand.Parameters.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output));
                subCommand.Parameters.Add(new OracleParameter("P_COD_ESTADO", OracleDbType.Int32, 4000, null, ParameterDirection.Output));

                subCommand.Parameters.Add(new OracleParameter("P_NPREMIUM_TOT", OracleDbType.Double, null, ParameterDirection.Input));
                subCommand.Parameters.Add(new OracleParameter("P_NTASA_TOT", OracleDbType.Double, null, ParameterDirection.Input));

                subCommand.ExecuteNonQuery();

                response.StatusCode = Convert.ToInt32(subCommand.Parameters["P_COD_ESTADO"].Value.ToString());
                response.ErrorMessageList = new List<string>();
                response.ErrorMessageList.Add(subCommand.Parameters["P_MENSAJE"].Value.ToString());
            }
            catch (Exception ex)
            {
                response.StatusCode = 3;
                LogControl.save("insertarTasasAutorizadas", ex.ToString(), "3");
            }

            return response;
        }

        public GenericResponseVM insertarHistorial(OracleConnection connection, OracleTransaction transaction, StatusChangeBM data, List<HttpPostedFile> fileList, string folderPath)
        {
            var response = new GenericResponseVM();
            try
            {
                DbCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = ProcedureName.pkg_Cotizacion + ".INS_COTIZA_HIS";
                command.Transaction = transaction;

                command.Parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.QuotationNumber, ParameterDirection.Input));
                command.Parameters.Add(new OracleParameter("P_ESTADO", OracleDbType.Int32, data.Status, ParameterDirection.Input));
                if (data.Reason != 0) command.Parameters.Add(new OracleParameter("P_NMOTIVO", OracleDbType.Int32, data.Reason, ParameterDirection.Input));
                else command.Parameters.Add(new OracleParameter("P_NMOTIVO", OracleDbType.Int32, null, ParameterDirection.Input));
                command.Parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, data.User, ParameterDirection.Input));
                command.Parameters.Add(new OracleParameter("P_SCOMMENT", OracleDbType.Varchar2, data.Comment, ParameterDirection.Input));

                if (fileList.Count > 0) command.Parameters.Add(new OracleParameter("P_SRUTA", OracleDbType.Varchar2, folderPath, ParameterDirection.Input));
                else command.Parameters.Add(new OracleParameter("P_SRUTA", OracleDbType.Varchar2, null, ParameterDirection.Input));
                command.Parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, data.Product, ParameterDirection.Input));

                command.Parameters.Add(new OracleParameter("P_NGLOSS_COT", OracleDbType.Int32, data.Gloss, ParameterDirection.Input)); // Mejora SCTR
                command.Parameters.Add(new OracleParameter("P_SGLOSS_COT", OracleDbType.Varchar2, data.GlossComment, ParameterDirection.Input)); // Mejora SCTR

                command.Parameters.Add(new OracleParameter("P_COD_ESTADO", OracleDbType.Int32, 4000, null, ParameterDirection.Output));
                command.Parameters.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output));
                command.Parameters.Add(new OracleParameter("P_DDATEAPROB", OracleDbType.Date, DBNull.Value, ParameterDirection.Input)); // Mejora VIDA LEY

                command.ExecuteNonQuery();

                response.StatusCode = Convert.ToInt32(command.Parameters["P_COD_ESTADO"].Value.ToString());
                response.ErrorMessageList = new List<string>();
                response.ErrorMessageList.Add(command.Parameters["P_MENSAJE"].Value.ToString());
            }
            catch (Exception ex)
            {
                response.StatusCode = 3;
                LogControl.save("insertarHistorial", ex.ToString(), "3");
            }

            return response;
        }

        public GenericResponseVM actualizarCotizacion(OracleConnection connection, OracleTransaction transaction, StatusChangeBM data, List<HttpPostedFile> fileList, string folderPath)
        {
            var response = new GenericResponseVM();

            try
            {
                DbCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = ProcedureName.pkg_Cotizacion + ".UPD_COTIZA_HIS_REN";
                command.Transaction = transaction;

                //input
                command.Parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.QuotationNumber, ParameterDirection.Input));

                //ouput
                command.Parameters.Add(new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output));
                command.Parameters.Add(new OracleParameter("P_NCOD_ERR", OracleDbType.Int32, 4000, null, ParameterDirection.Output));

                command.ExecuteNonQuery();

                response.StatusCode = Convert.ToInt32(command.Parameters["P_NCOD_ERR"].Value.ToString());
                response.ErrorMessageList = new List<string>();
                response.ErrorMessageList.Add(command.Parameters["P_MESSAGE"].Value.ToString());
            }
            catch (Exception ex)
            {
                response.StatusCode = 3;
                LogControl.save("actualizarCotizacion", ex.ToString(), "3");
            }

            return response;
        }

        public GenericResponseVM AddStatusChange(StatusChangeBM data, List<HttpPostedFile> fileList, OracleTransaction transaction, DbConnection connection)
        {
            GenericResponseVM resultPackage = new GenericResponseVM();
            resultPackage.ErrorMessageList = new List<string>();
            resultPackage.SuccessMessageList = new List<string>();

            try
            {
                string storedProcedureName = ProcedureName.pkg_Cotizacion + ".INS_COTIZA_HIS";
                var connectionString = ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString();


                string folderPath = ELog.obtainConfig("pathCotizacion") + data.QuotationNumber + "\\" + ELog.obtainConfig("movimiento") + "\\" + DateTime.Now.ToString("yyyyMMddhhmmss") + "\\";

                DbCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = storedProcedureName;
                command.Transaction = transaction;

                command.Parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.QuotationNumber, ParameterDirection.Input));
                command.Parameters.Add(new OracleParameter("P_ESTADO", OracleDbType.Int32, data.Status, ParameterDirection.Input));
                if (data.Reason != 0) command.Parameters.Add(new OracleParameter("P_NMOTIVO", OracleDbType.Int32, data.Reason, ParameterDirection.Input));
                else command.Parameters.Add(new OracleParameter("P_NMOTIVO", OracleDbType.Int32, null, ParameterDirection.Input));
                command.Parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, data.User, ParameterDirection.Input));
                command.Parameters.Add(new OracleParameter("P_SCOMMENT", OracleDbType.Varchar2, data.Comment, ParameterDirection.Input));

                if (fileList.Count > 0) command.Parameters.Add(new OracleParameter("P_SRUTA", OracleDbType.Varchar2, folderPath, ParameterDirection.Input));
                else command.Parameters.Add(new OracleParameter("P_SRUTA", OracleDbType.Varchar2, null, ParameterDirection.Input));

                command.Parameters.Add(new OracleParameter("P_COD_ESTADO", OracleDbType.Int32, 4000, null, ParameterDirection.Output));
                command.Parameters.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output));
                command.Parameters.Add(new OracleParameter("P_DDATEAPROB", OracleDbType.Date, DBNull.Value, ParameterDirection.Input)); // Mejora VIDA LEY

                command.ExecuteNonQuery();
                resultPackage.StatusCode = Convert.ToInt32(command.Parameters["P_COD_ESTADO"].Value.ToString());
                if (resultPackage.StatusCode == 0)
                {
                    if (fileList.Count > 0)
                    {
                        if (!Directory.Exists(String.Format(ELog.obtainConfig("pathPrincipal"), folderPath)))
                        {
                            Directory.CreateDirectory(String.Format(ELog.obtainConfig("pathPrincipal"), folderPath));
                        }

                        foreach (var file in fileList)
                        {
                            file.SaveAs(String.Format(ELog.obtainConfig("pathPrincipal"), folderPath) + file.FileName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                resultPackage.StatusCode = 1;
                LogControl.save("AddStatusChange", ex.ToString(), "3");
            }

            return resultPackage;

        }
        public List<StatusVM> GetStatusList(string certype, string codProduct)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            string[] statusList = ELog.obtainConfig("statusList" + certype + codProduct).Split(';');
            List<StatusVM> list = new List<StatusVM>();

            string storedProcedureName = ProcedureName.pkg_Cotizacion + ".REA_EST_COTIZA";
            try
            {
                parameter.Add(new OracleParameter("P_SCERTYPE", OracleDbType.Varchar2, certype, ParameterDirection.Input));
                OracleParameter table = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(table);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                while (odr.Read())
                {
                    StatusVM item = new StatusVM();
                    item.Id = odr["COD_ESTADO"].ToString();
                    item.Name = odr["DES_ESTADO"].ToString();

                    list.Add(item);

                }
                ELog.CloseConnection(odr);

                list = list.Where((x) => statusList.Contains(x.Id.ToString())).ToList();


            }
            catch (Exception ex)
            {
                LogControl.save("GetStatusList", ex.ToString(), "3");
            }
            return list;
        }

        public List<ReasonVM> GetReasonList(Int32 statusCode, string branch)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<ReasonVM> list = new List<ReasonVM>();

            string storedProcedureName = ProcedureName.pkg_Cotizacion + ".REA_MOTI_COTIZA";
            try
            {
                parameter.Add(new OracleParameter("P_SSTATREGT_COT", OracleDbType.Int32, statusCode, ParameterDirection.Input));
                OracleParameter table = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(table);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                while (odr.Read())
                {
                    ReasonVM item = new ReasonVM();
                    item.Id = odr["COD_MOTIVO"].ToString();
                    item.Name = odr["DES_MOTIVO"].ToString();
                    list.Add(item);
                }
                ELog.CloseConnection(odr);

                var lPermitidos = new string[] { ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch") };
                if (lPermitidos.Contains(branch.ToString()))
                {
                    list = statusCode == 11 ? list.Where((x) => (new string[] { "4" }).Contains(x.Id.ToString())).ToList() :
                        list = list.Where((x) => (new string[] { "1", "4" }).Contains(x.Id.ToString())).ToList();
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetReasonList", ex.ToString(), "3");
            }
            return list;
        }

        public GenericResponseVM GetTrackingList(TrackingBM data)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            GenericResponseVM resultPackage = new GenericResponseVM();
            List<QuotationTrackingVM> list = new List<QuotationTrackingVM>();

            string storedProcedureName = ProcedureName.pkg_Cotizacion + ".REA_COTIZA_HIS";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Varchar2, data.QuotationNumber, ParameterDirection.Input));

                parameter.Add(new OracleParameter("P_NLIMITPERPAGE", OracleDbType.Int32, data.LimitPerPage, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPAGENUM", OracleDbType.Int32, data.PageNumber, ParameterDirection.Input));

                OracleParameter table = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(table);
                OracleParameter totalRowNumber = new OracleParameter("P_NTOTALROWS", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameter.Add(totalRowNumber);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                while (odr.Read())
                {
                    QuotationTrackingVM item = new QuotationTrackingVM();
                    item.quotation = data.QuotationNumber;
                    item.User = odr["USUARIO"].ToString();
                    item.Status = odr["ESTADO"].ToString();
                    item.Comment = odr["SCOMMENT"].ToString();
                    item.Reason = odr["MOTIVO"].ToString();
                    item.Profile = odr["PERFIL"].ToString();
                    item.codProceso = (odr["COD_PROCESO"] == null ? string.Empty : odr["COD_PROCESO"].ToString());
                    item.linea = (odr["LINEA"] == null ? string.Empty : odr["LINEA"].ToString());
                    item.desMov = (odr["DES_MOVIMIENTO"] == null ? string.Empty : odr["DES_MOVIMIENTO"].ToString());
                    item.nbranch = odr["NBRANCH"].ToString();
                    item.reverse = 0;
                    if (odr["FECHA_ESTADO"].ToString() != null && odr["FECHA_ESTADO"].ToString().Trim() != "") item.EventDate = Convert.ToDateTime(odr["FECHA_ESTADO"].ToString());
                    else item.EventDate = null;
                    item.sstatregt = odr["SSTATREGT"].ToString();

                    if (odr["SRUTA"] != null && odr["SRUTA"].ToString().Length > 0)
                    {
                        try
                        {
                            item.FilePathList = sharedMethods.GetFilePathList(String.Format(ELog.obtainConfig("pathPrincipal"), odr["SRUTA"].ToString()));
                        }
                        catch (ArgumentNullException)
                        {
                            item.FilePathList = new List<string>();
                        }
                        catch (ArgumentException)
                        {
                            item.FilePathList = new List<string>();
                        }
                        catch (System.IO.DirectoryNotFoundException)    //Si la ruta no fue encontrada, debemos enviar un mensaje de error
                        {
                            item.FilePathList = new List<string>();
                        }
                    }
                    else
                    {
                        item.FilePathList = new List<string>();
                    }

                    item.Vigencia = odr["SVIGENCIA_MOV"] == DBNull.Value ? string.Empty : odr["SVIGENCIA_MOV"].ToString();

                    list.Add(item);
                }
                resultPackage.TotalRowNumber = Int32.Parse(totalRowNumber.Value.ToString());

                resultPackage.GenericResponse = list;
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("GetTrackingList", ex.ToString(), "3");
            }

            return resultPackage;
        }

        public string tramitePendiente(QuotationTrackingVM info, int condicionado)
        {
            var response = string.Empty;
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.pkg_Tramites + ".REA_PENDIENTE_VOUCHER";

            try
            {
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int64, info.quotation, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SSTATREGT", OracleDbType.Int32, info.sstatregt, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCONDICIONADO", OracleDbType.Int32, condicionado, ParameterDirection.Input));

                OracleParameter P_SDATA = new OracleParameter("P_SDATA", OracleDbType.Varchar2, ParameterDirection.Output);
                P_SDATA.Size = 9000;
                parameter.Add(P_SDATA);

                this.ExecuteByStoredProcedureVT(storeprocedure, parameter);
                response = P_SDATA.Value.ToString();
            }
            catch (Exception ex)
            {
                response = "0";
                LogControl.save("tramitePendiente", ex.ToString(), "3");
            }

            return response;
        }

        public SalidaTramaBaseVM InsertReCotizaTrama(validaTramaVM request)
        {
            var response = new SalidaTramaBaseVM();
            DbConnection DataConnection = ConnectionGet(enuTypeDataBase.OracleVTime);
            DbTransaction trx = null;

            try
            {
                DataConnection.Open();
                trx = DataConnection.BeginTransaction();

                response = InsReCotizaTrama(request, DataConnection, trx);
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = "1";
                response.P_MESSAGE = ex.ToString();
                LogControl.save("InsertReCotizaTrama", ex.ToString(), "3");
            }
            finally
            {
                if (response.P_COD_ERR == "0")
                {
                    trx.Commit();
                }
                else
                {
                    if (trx.Connection != null) trx.Rollback();
                }

                if (trx.Connection != null) trx.Dispose();
                ELog.CloseConnection(DataConnection);
            }
            return response;
        }

        public SalidaTramaBaseVM InsertCotizaTrama(validaTramaVM data, int flagTecnica)
        {
            var response = new SalidaTramaBaseVM();

            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
            {
                cn.Open();

                using (var tran = cn.BeginTransaction())
                {
                    try
                    {
                        if (data.type_mov == "8") // Endoso
                        {
                            response = InsCotizaTramaEndoso(data, cn, tran);
                        }
                        else if (data.type_mov == "3") // Exclusion
                        {
                            response = InsCotizaTramaExc(data, cn, tran);
                        }
                        else // Otros
                        {
                            response = InsCotizaTrama(data, flagTecnica, cn, tran);
                        }

                        // tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        response.P_COD_ERR = "1";
                        response.P_MESSAGE = ex.ToString();
                        LogControl.save("InsertCotizaTrama", ex.ToString(), "3");
                    }
                    finally
                    {
                        if (response.P_COD_ERR == "0")
                        {
                            tran.Commit();
                        }
                        else
                        {
                            tran.Rollback();
                        }

                        if (cn.State == ConnectionState.Open) cn.Close();
                    }
                }
            }

            return response;
        }

        public SalidaErrorBaseVM InsertCotizaTramaPol(validaTramaVM data)
        {
            SalidaErrorBaseVM response = new SalidaErrorBaseVM();
            DbConnection DataConnection = ConnectionGet(enuTypeDataBase.OracleVTime);
            DbTransaction trx = null;
            DataConnection.Close();

            try
            {
                DataConnection.Open();
                trx = DataConnection.BeginTransaction();
                response = InsCotizaTramaPol(data, DataConnection, trx);
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = ex.ToString();
                LogControl.save("InsertCotizaTramaPol", ex.ToString(), "3");
            }
            finally
            {
                if (response.P_COD_ERR == 0)
                {
                    trx.Commit();
                }
                else
                {
                    if (trx.Connection != null) trx.Rollback();
                }

                if (trx.Connection != null) trx.Dispose();
                ELog.CloseConnection(DataConnection);
            }

            return response;
        }

        public SalidaErrorBaseVM regDetallePlanCot(validaTramaVM data, int condicion, int tecnica)
        {
            SalidaErrorBaseVM response = new SalidaErrorBaseVM();

            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
            {
                cn.Open();

                using (var tran = cn.BeginTransaction())
                {
                    try
                    {
                        data.condicion = condicion;
                        response = eliminarCoberturasCotTrx(data, cn, tran);
                        response = response.P_COD_ERR == 0 ? eliminarAsistenciasCotTrx(data, cn, tran) : response;
                        response = response.P_COD_ERR == 0 ? eliminarBeneficiosCotTrx(data, cn, tran) : response;
                        //response = response.P_COD_ERR == 0 ? eliminarReajustesCotTrx(data, DataConnection, trx) : response;
                        response = response.P_COD_ERR == 0 ? eliminarRecargosCotTrx(data, cn, tran) : response;
                        response = response.P_COD_ERR == 0 ? eliminarServiciosAdicionalesCotTrx(data, cn, tran) : response; //ED
                        response = response.P_COD_ERR == 0 && tecnica == 1 ? eliminarExclusionsCotTrx(data, cn, tran) : response;


                        if (response.P_COD_ERR == 0)
                        {
                            //int flagcov = data.flagCalcular == 1 ? 2 : 1;
                            if (data.lcoberturas != null)
                            {
                                foreach (var cobertura in data.lcoberturas)
                                {
                                    //cobertura.nfactor = cobertura.nfactor > 0 ? cobertura.nfactor : data.datosPoliza.branch == 72 && data.datosPoliza.codTipoProducto == "3" ? getFactor(data.datosPoliza.branch.ToString(), data.datosPoliza.codTipoProducto, cobertura.codCobertura, data.datosContratante.codContratante, data.ntecnica) : 0;
                                    // valRentaEstudiantil(generic.NBRANCH.ToString(), generic.NPRODUCT.ToString()) ? quotationCORE.getFactor(generic.NBRANCH.ToString(), generic.NPRODUCT.ToString(), request.module) : 0
                                    //cobertura.nfactor = cobertura.nfactor > 0 ? cobertura.nfactor : data.nfactor;
                                    response = response.P_COD_ERR == 0 ? regCoberturasCotTrx(data, cobertura, cn, tran) : response;
                                    if (cobertura.items != null && cobertura.items.Count > 0)
                                    {
                                        foreach (var subItem in cobertura.items)
                                        {
                                            subItem.codeCover = cobertura.codCobertura;
                                            response = response = response.P_COD_ERR == 0 ? insertSubItemTrx(data, subItem, cn, tran) : response;
                                        }
                                    }
                                }
                            }
                        }

                        if (response.P_COD_ERR == 0)
                        {
                            if (data.lasistencias != null)
                            {
                                foreach (var asistencia in data.lasistencias)
                                {
                                    response = response.P_COD_ERR == 0 ? regAsistenciasCotTrx(data, asistencia, cn, tran) : response;
                                }
                            }
                        }

                        if (response.P_COD_ERR == 0)
                        {
                            if (data.lbeneficios != null)
                            {
                                foreach (var beneficio in data.lbeneficios)
                                {
                                    response = response.P_COD_ERR == 0 ? regBeneficiosCotTrx(data, beneficio, cn, tran) : response;
                                }
                            }
                        }

                        //ED RENTA
                        if (response.P_COD_ERR == 0)
                        {
                            if (data.lservAdicionales != null)
                            {
                                foreach (var servAdicionales in data.lservAdicionales)
                                {
                                    response = response.P_COD_ERR == 0 ? regServiciosAdicionalesCotTrx(data, servAdicionales, cn, tran) : response;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        response.P_COD_ERR = 1;
                        response.P_MESSAGE = ex.ToString();
                        LogControl.save("regDetallePlanCot", ex.ToString(), "3");
                    }
                    finally
                    {
                        if (response.P_COD_ERR == 0)
                        {
                            tran.Commit();
                        }
                        else
                        {
                            tran.Rollback();
                        }

                        if (cn.State == ConnectionState.Open) cn.Close();
                    }
                }
            }

            return response;
        }

        public SalidaErrorBaseVM UpdCoberturasProp(string covergen, string sumaProp, string codProc, int flag)
        {
            SalidaErrorBaseVM response = new SalidaErrorBaseVM();

            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
            {
                cn.Open();

                using (var tran = cn.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        response = UpdateCoberturasProp(covergen, sumaProp, codProc, flag, cn, tran);
                    }
                    catch (Exception ex)
                    {
                        response.P_COD_ERR = 1;
                        response.P_MESSAGE = ex.ToString();
                        LogControl.save("UpdCoberturasProp", ex.ToString(), "3");
                    }
                    finally
                    {
                        if (response.P_COD_ERR == 0)
                        {
                            tran.Commit();
                        }
                        else
                        {
                            tran.Rollback();
                        }

                        if (cn.State == ConnectionState.Open) cn.Close();
                    }
                }
            }
            //DbConnection DataConnection = ConnectionGet(enuTypeDataBase.OracleVTime);
            //DbTransaction trx = null;
            //DataConnection.Close();

            //try
            //{
            //    DataConnection.Open();
            //    trx = DataConnection.BeginTransaction();
            //    response = UpdateCoberturasProp(covergen, sumaProp, codProc, flag, DataConnection, trx);
            //}
            //catch (Exception ex)
            //{
            //    response.P_COD_ERR = 1;
            //    response.P_MESSAGE = ex.ToString();
            //    ELog.save(this, ex.ToString());
            //}
            //finally
            //{
            //    if (response.P_COD_ERR == 0)
            //    {
            //        trx.Commit();
            //    }
            //    else
            //    {
            //        if (trx.Connection != null) trx.Rollback();
            //    }

            //    if (trx.Connection != null) trx.Dispose();
            //    ELog.CloseConnection(DataConnection);
            //}
            return response;
        }

        public PremiumBM GetPremiumMin()
        {
            PremiumBM response = new PremiumBM();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.pkg_ValidadorGenPD + ".SP_REA_PREMIUM_MIN";

            try
            {
                OracleParameter P_NPREMIUM = new OracleParameter("P_NPREMIUM", OracleDbType.Varchar2, ParameterDirection.Output);
                P_NPREMIUM.Size = 9000;
                parameter.Add(P_NPREMIUM);

                this.ExecuteByStoredProcedureVT(storeprocedure, parameter);
                response.premium = Convert.ToInt32(P_NPREMIUM.Value.ToString());
            }
            catch (Exception ex)
            {
                response.premium = 0;
                LogControl.save("GetPremiumMin", ex.ToString(), "3");
            }

            return response;
        }

        public async Task<QuotationResponseVM> InsertQuotation(QuotationCabBM request, List<HttpPostedFile> files, dataQuotation_EPS request_EPS)
        {
            var NID_COTIZACION = 0;
            long NID_TRAMITE = 0;
            QuotationResponseVM response = null;
            DbConnection DataConnection = ConnectionGet(enuTypeDataBase.OracleVTime);
            DbTransaction trx = null;

            request.P_SRUTA = request.FlagCotEstado == 2 ? ELog.obtainConfig("pathCotizacion") + request.NumeroCotizacion + "\\" + ELog.obtainConfig("movimiento") + "\\" + DateTime.Now.ToString("yyyyMMddhhmmss") + "\\" : ELog.obtainConfig("pathCotizacion");

            try
            {
                DataConnection.Open();
                trx = DataConnection.BeginTransaction();

                // Inserción de cabecera para productos sin tabla de calculos - Revisar covid
                var lRamosSC = new string[] { ELog.obtainConfig("vidaLeyBranch"), ELog.obtainConfig("sctrBranch") };

                // Inserción de cabecera para productos con tabla de calculos
                var lRamosCC = new string[] { ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch"),
                    ELog.obtainConfig("vidaIndividualBranch"), ELog.obtainConfig("desgravamenBranch") };

                if (request.NumeroCotizacion != null && request.NumeroCotizacion.Value != 0)
                {
                    response = UpdateQuotationCab(request, DataConnection, trx);
                    NID_COTIZACION = request.NumeroCotizacion.Value;
                }
                else
                {
                    // Sin Tabla de calculos
                    if (lRamosSC.Contains(request.P_NBRANCH.ToString()))
                    {
                        response = InsertQuotationCab(request, DataConnection, trx);
                    }

                    // Con tabla de calculos 
                    if (lRamosCC.Contains(request.P_NBRANCH.ToString()))
                    {
                        response = InsertQuotationCabAP(request, DataConnection, trx);
                    }

                    NID_COTIZACION = response.P_NID_COTIZACION != "null" ? Convert.ToInt32(response.P_NID_COTIZACION) : 0;
                }

                if (ELog.obtainConfig("vidaIndividualBranch") == request.P_NBRANCH.ToString())
                {
                    if (request.P_SAPLICACION != "EC")
                    {
                        //Enviar Información DPS - Correo
                        List<QuotizacionCliBM> datosCLi = new List<QuotizacionCliBM>();
                        datosCLi = request.QuotationCli;
                        datosCLi[0].externalId = response.P_NID_COTIZACION;
                        var jsonDPS = await invocarServicioDPS(JsonConvert.SerializeObject(datosCLi[0]));

                        DpsToken responsedps = new DpsToken();

                        responsedps = JsonConvert.DeserializeObject<DpsToken>(jsonDPS, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        if (responsedps.success)
                        {
                            request.P_SID_DPS = responsedps.id;
                            request.P_STOKEN_DPS = responsedps.key;
                        }

                        QuotationResponseVM var = null;
                        var = UpdateDPS(NID_COTIZACION, datosCLi[0].idUsuario, responsedps.id, responsedps.key, 1, DataConnection, trx);

                        //Enviar Información DPS
                    }
                }

                if (response.P_COD_ERR == 0 && request.TrxCode != "EX")
                {
                    if (request.P_NBRANCH == Convert.ToInt64(ELog.obtainConfig("vidaLeyBranch")))
                    {
                        if (request.QuotationDet != null)
                        {
                            // INI GCAA 23012024
                            var agruparxCategoria = request.QuotationDet
                                                    .GroupBy(x => x.P_NMODULEC)
                                                    .Select(grupo => new
                                                    {
                                                        P_NMODULEC = grupo.Key,
                                                        TotalTrabajadores = grupo.Sum(item => item.P_NTOTAL_TRABAJADORES)
                                                    })
                                                    .ToList();

                            foreach (var grupo in agruparxCategoria)
                            {
                                if (grupo.TotalTrabajadores > 0)
                                {
                                    foreach (var item in request.QuotationDet.Where(z => z.P_NMODULEC == grupo.P_NMODULEC))
                                    {
                                        if (response.P_COD_ERR == 0)
                                        {
                                            item.P_NID_COTIZACION = NID_COTIZACION;

                                            // Productos Sin tabla Calculos
                                            if (lRamosSC.Contains(request.P_NBRANCH.ToString()))
                                            {
                                                response = InsertQuotationDet(request, item, DataConnection, trx);
                                                response.P_NID_COTIZACION = NID_COTIZACION.ToString();
                                            }

                                            // Productos Con tabla Calculos
                                            if (lRamosCC.Contains(request.P_NBRANCH.ToString()))
                                            {
                                                response = InsertQuotationDetAP(request, item, DataConnection, trx);
                                                response.P_NID_COTIZACION = NID_COTIZACION.ToString();
                                            }
                                        }

                                    }
                                }
                            }
                            // FIN GCAA 23012024
                        }
                    }
                    else
                    {
                        if (request.QuotationDet != null)
                        {
                            foreach (var item in request.QuotationDet)
                            {
                                if (response.P_COD_ERR == 0)
                                {
                                    item.P_NID_COTIZACION = NID_COTIZACION;

                                    // Productos Sin tabla Calculos
                                    if (lRamosSC.Contains(request.P_NBRANCH.ToString()))
                                    {
                                        response = InsertQuotationDet(request, item, DataConnection, trx);
                                        response.P_NID_COTIZACION = NID_COTIZACION.ToString();
                                    }

                                    // Productos Con tabla Calculos
                                    if (lRamosCC.Contains(request.P_NBRANCH.ToString()))
                                    {
                                        response = InsertQuotationDetAP(request, item, DataConnection, trx);
                                        response.P_NID_COTIZACION = NID_COTIZACION.ToString();
                                    }
                                }

                            }
                        }
                    }
                }

                if (response.P_COD_ERR == 0 && request.P_NBRANCH == Convert.ToInt64(ELog.obtainConfig("vidaLeyBranch")) && request.TrxCode != "EX" && request.FlagCotEstado != 1)
                {
                    response = UpdateQuotationDetPremium(request, NID_COTIZACION, DataConnection, trx);
                }

                if (response.P_COD_ERR == 0 && request.TrxCode != "EX")
                {
                    if (request.QuotationCom != null)
                    {
                        if (request.QuotationCom.Count == 0)
                        {
                            if (request.P_NBRANCH == Convert.ToInt64(ELog.obtainConfig("vidaLeyBranch"))
                            && request.NumeroCotizacion != null && request.NumeroCotizacion.Value != 0 && request.FlagCotEstado != 1)
                            {
                                response = UpdateDetalleModFinal(request, DataConnection, trx);
                                response.P_NID_COTIZACION = NID_COTIZACION.ToString();
                            }
                        }
                        var num = 0;

                        foreach (var item in request.QuotationCom)
                        {
                            if (response.P_COD_ERR == 0)
                            {
                                item.P_NPRINCIPAL = num;
                                item.P_NID_COTIZACION = NID_COTIZACION;
                                response = InsertQuotationCom(request, item, DataConnection, trx);
                                response.P_NID_COTIZACION = NID_COTIZACION.ToString();
                            }
                            num++;
                        }
                    }
                }

                if (response.P_COD_ERR == 0 && request.TrxCode != "EX")
                {
                    if (request.QuotationCol != null)
                    {
                        foreach (var item in request.QuotationCol)
                        {
                            item.nid_cotizacion = NID_COTIZACION;
                            response = InsertQuotationCol(request, item, DataConnection, trx);
                            response.P_NID_COTIZACION = NID_COTIZACION.ToString();
                        }
                    }
                }

                if (response.P_COD_ERR == 0 && response.P_NID_COTIZACION != "0")
                {
                    // Productos Sin tabla Calculos
                    if (lRamosSC.Contains(request.P_NBRANCH.ToString()) && request.FlagCotEstado != 1)
                    {
                        response = UpdateCodQuotation(NID_COTIZACION, request.CodigoProceso, DataConnection, trx);
                        response.P_NID_COTIZACION = NID_COTIZACION.ToString();
                    }

                    // Productos Con tabla Calculos
                    if (lRamosCC.Contains(request.P_NBRANCH.ToString()))
                    {
                        if (request.P_NBRANCH == Convert.ToInt64(ELog.obtainConfig("desgravamenBranch")))
                        {
                            response.P_NID_COTIZACION = NID_COTIZACION.ToString();
                        }
                        else
                        {
                            response = UpdateCodQuotationPD(response.P_NID_COTIZACION, request, DataConnection, trx);
                            response.P_NID_COTIZACION = NID_COTIZACION.ToString();
                        }

                    }
                }

                if (response.P_COD_ERR == 0 && response.P_NID_COTIZACION != "0")
                {
                    var rutaFinal = String.Format(ELog.obtainConfig("pathPrincipal"), ELog.obtainConfig("pathCotizacion") + response.P_NID_COTIZACION + "\\" + ELog.obtainConfig("cotizacionKey") + "\\");

                    if (files != null && files.Count > 0)
                    {
                        if (!Directory.Exists(rutaFinal))
                        {
                            Directory.CreateDirectory(rutaFinal);
                        }

                        foreach (var item in files)
                        {
                            item.SaveAs(rutaFinal + item.FileName);
                        }
                    }

                    //desgravament
                    if (request.P_NBRANCH.ToString() == ELog.obtainConfig("vidaIndividualBranch") && request.P_SAPLICACION == "EC")
                    {
                        var nroCotizacion = response.P_NID_COTIZACION;
                        response = UpdateCargaEC(nroCotizacion, request, DataConnection, trx);
                        response = UpdateStatusCotizacion(nroCotizacion, request, DataConnection, trx);
                        response = UpdateCotProrrateo(nroCotizacion, request, DataConnection, trx);
                    }

                    if (request.P_NBRANCH.ToString() == ELog.obtainConfig("vidaIndividualBranch") && request.P_SAPLICACION != "EC")
                    {
                        request.SMAIL_EJECCOM = request.QuotationCli[0].correo;
                        response = InsertTransact(request, NID_COTIZACION, request.NumeroCotizacion != null && request.NumeroCotizacion.Value != 0, DataConnection, trx);
                        NID_TRAMITE = response.P_NID_TRAMITE;
                        response.P_NID_COTIZACION = NID_COTIZACION.ToString();
                    }

                    if (request.flagComerExclu == 0) //RQ - Perfil Nuevo - Comercial Exclusivo
                    {
                        /*generar trámite vl*/
                        if (request.P_NBRANCH.ToString() == ELog.obtainConfig("vidaLeyBranch"))
                        {
                            var folderPath = ELog.obtainConfig("pathTramite") + response.P_NID_COTIZACION + "\\" + ELog.obtainConfig("movimiento") + "\\" + DateTime.Now.ToString("yyyyMMddhhmmss") + "\\";
                            folderPath = request.FlagCotEstado == 2 ? request.P_SRUTA : folderPath;

                            if (request.FlagCotEstado == 1 || request.FlagCotEstado == 2)
                            {
                                request.P_SRUTA = folderPath;
                            }
                            response = InsertTransact(request, NID_COTIZACION, request.NumeroCotizacion != null && request.NumeroCotizacion.Value != 0, DataConnection, trx);
                            NID_TRAMITE = response.P_NID_TRAMITE;
                            response.P_NID_COTIZACION = NID_COTIZACION.ToString();

                            if ((request.FlagCotEstado == 1 && response.P_COD_ERR == 0) || request.FlagCotEstado == 2) // Para la insercion de adjuntos en el tramite de emision -VL tramite de estado. 
                            {


                                if (files != null && files.Count > 0)
                                {

                                    if (!Directory.Exists(String.Format(ELog.obtainConfig("pathPrincipal"), folderPath)))
                                    {
                                        Directory.CreateDirectory(String.Format(ELog.obtainConfig("pathPrincipal"), folderPath));
                                    }

                                    foreach (var file in files)
                                    {
                                        file.SaveAs(String.Format(ELog.obtainConfig("pathPrincipal"), folderPath) + file.FileName);
                                    }
                                }
                            }
                        }
                    }

                    // Validacion de retroactividad
                    var lRetro = new string[] { ELog.obtainConfig("vidaLeyBranch"), ELog.obtainConfig("sctrBranch"), ELog.obtainConfig("accidentesBranch"),
                                                 ELog.obtainConfig("vidaGrupoBranch") };
                    if (lRetro.Contains(request.P_NBRANCH.ToString()))
                    {
                        if (request.FlagCotEstado != 1)
                        {
                            response = ValRetroCotizacion(request, NID_COTIZACION, DataConnection, trx);
                        }

                        if (response.P_COD_ERR == 1)
                        {
                            response.P_NID_COTIZACION = "";
                        }
                        else
                        {
                            response.P_NID_COTIZACION = NID_COTIZACION.ToString();
                        }

                    }

                    if (response.P_COD_ERR == 0 && response.P_NID_COTIZACION != "0")
                    {
                        // Desgravament                    
                        if (request.P_SAPLICACION != "EC")
                        {
                            if (lRetro.Contains(request.P_NBRANCH.ToString()))
                            {
                                if (request.FlagCotEstado != 1)
                                {
                                    response = ValCotizacion(NID_COTIZACION, request.P_NUSERCODE, request.P_NPENDIENTE, request.P_NBRANCH == Convert.ToInt64(ELog.obtainConfig("vidaLeyBranch")) ? request.P_NCAMBIOPLAN : 0, DataConnection, trx);
                                }

                            }
                            else if (request.P_NBRANCH.ToString() == ELog.obtainConfig("vidaIndividualBranch"))
                            {
                                response.P_SAPROBADO = "S";
                            }
                            else if (request.P_NBRANCH.ToString() == ELog.obtainConfig("desgravamenBranch"))
                            {
                                //response.P_SAPROBADO = "S";
                                response = ValCotizacion(NID_COTIZACION, request.P_NUSERCODE, request.P_NPENDIENTE, request.P_NBRANCH == Convert.ToInt64(ELog.obtainConfig("vidaLeyBranch")) ? request.P_NCAMBIOPLAN : 0, DataConnection, trx);

                            };
                        }

                        // Desgravament
                        response.P_NID_COTIZACION = NID_COTIZACION.ToString();
                        response.P_NIDPROC = request.CodigoProceso;
                        response.P_NID_TRAMITE = NID_TRAMITE;
                    }
                    else
                    {
                        if (trx.Connection != null)
                        {
                            trx.Rollback();
                            trx.Dispose();
                        }
                        DataConnection.Close();
                        DataConnection.Dispose();
                    }

                    trx.Commit();
                    trx.Dispose();
                    DataConnection.Close();
                }
                else
                {
                    if (trx.Connection != null)
                    {
                        trx.Rollback();
                        trx.Dispose();
                    }
                    DataConnection.Close();
                    DataConnection.Dispose();

                }
            }
            catch (Exception ex)
            {
                response.P_NCODE = 1;
                response.P_SMESSAGE = ex.ToString();
                response.P_COD_ERR = 1;
                response.P_MESSAGE = ex.ToString();
                if (trx.Connection != null)
                {
                    trx.Rollback();
                    trx.Dispose();
                }
                DataConnection.Close();
                DataConnection.Dispose();
                LogControl.save("InsertQuotation", ex.ToString(), "3");
            }

            return await Task.FromResult(response);
        }

        public async Task<QuotationResponseVM> ValidateQuotationEstadoMatriz(QuotationCabBM request, int NID_COTIZACION, int usercode)
        {
            QuotationResponseVM response = new QuotationResponseVM();

            request.P_SRUTA = ELog.obtainConfig("pathCotizacion");
            List<HttpPostedFile> files = new List<HttpPostedFile>();
            foreach (var item in HttpContext.Current.Request.Files)
            {
                HttpPostedFile arch = HttpContext.Current.Request.Files[item.ToString()];
                files.Add((HttpPostedFile)HttpContext.Current.Request.Files[item.ToString()]);
            }

            string folderPath = files.Count > 0 ? ELog.obtainConfig("pathCotizacion") + request.NumeroCotizacion + "\\" + ELog.obtainConfig("movimiento") + "\\" + DateTime.Now.ToString("yyyyMMddhhmmss") + "\\" : String.Empty;
            request.P_SRUTA = folderPath;
            response.P_COD_ERR = 0;

            DbConnection DataConnection = ConnectionGet(enuTypeDataBase.OracleVTime);
            DbTransaction trx = null;
            try
            {
                var nmotivo = 10;
                var smessage = "Emisión Póliza Matriz " + (string.IsNullOrEmpty(request.P_SCOMMENT) ? "" : "/ " + request.P_SCOMMENT);
                DataConnection.Open();
                trx = DataConnection.BeginTransaction();
                // this.InsCotizaHisEstadoMatriz(request, smessage, DataConnection, trx, nmotivo,0);

                var resValRetroCotizacion = ValRetroCotizacion(request, NID_COTIZACION, DataConnection, trx);
                var resValCotizacion = ValCotizacion(NID_COTIZACION, request.P_NUSERCODE, request.P_NPENDIENTE, request.P_NBRANCH == Convert.ToInt64(ELog.obtainConfig("vidaLeyBranch")) ? request.P_NCAMBIOPLAN : 0, DataConnection, trx);


                if (resValRetroCotizacion.P_SAPROBADO == "A")
                {
                    nmotivo = 11;
                    smessage = smessage + " - ";
                }

                this.InsCotizaHisEstadoMatriz(request, smessage, DataConnection, trx, nmotivo);

                if (files.Count > 0 && response.P_COD_ERR == 0)
                {
                    if (!Directory.Exists(String.Format(ELog.obtainConfig("pathPrincipal"), folderPath)))
                    {
                        Directory.CreateDirectory(String.Format(ELog.obtainConfig("pathPrincipal"), folderPath));
                    }

                    foreach (var file in files)
                    {
                        file.SaveAs(String.Format(ELog.obtainConfig("pathPrincipal"), folderPath) + file.FileName);
                    }
                }


                //P_NMOTIVO
                trx.Commit();
                trx.Dispose();
                DataConnection.Close();

                response.P_NCODE = 0;
                response.P_SMESSAGE = "OK";
            }
            catch (Exception ex)
            {
                response.P_NCODE = 1;
                response.P_SMESSAGE = ex.ToString();
                LogControl.save("ValidateQuotationEstadoMatriz", ex.ToString(), "3");
                trx.Rollback();
                trx.Dispose();
            }

            return await Task.FromResult(response);
        }

        public async Task<string> invocarServicioDPS(string json)
        {
            var sTariffResult = string.Empty;

            try
            {
                var mToken = "";
                using (HttpClient client = new HttpClient())
                {
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    var response = await client.GetAsync(ELog.obtainConfig("authDPS"));
                    mToken = await response.Content.ReadAsStringAsync();
                }

                if (string.IsNullOrEmpty(mToken))
                {
                    throw new Exception(ELog.obtainConfig("errorTarifario"));
                }
                else
                {
                    var tariffToken = JsonConvert.DeserializeObject<AuthTariffResult>(mToken);

                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tariffToken.token);
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                        // Enviando correo de la Dps
                        var response = await client.PostAsync(ELog.obtainConfig("registroDPS"),
                                                              new StringContent(json, Encoding.UTF8, "application/json"));

                        sTariffResult = await response.Content.ReadAsStringAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                LogControl.save("invocarServicioDPS", ex.ToString(), "3");
            }

            return await Task.FromResult(sTariffResult);
        }

        public QuotationResponseVM UpdateDPS(int nidcotizacion, int user, string iddps, string tokendps, int estadodps, DbConnection connection = null, DbTransaction trx = null)
        {
            var sPackageName = ProcedureName.pkg_Cotizacion + ".SP_UPD_COTIZACION_CAB_DPS";
            List<OracleParameter> parameter = new List<OracleParameter>();
            QuotationResponseVM result = new QuotationResponseVM();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, nidcotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SID_DPS", OracleDbType.Varchar2, iddps.ToString(), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STOKEN_DPS", OracleDbType.Varchar2, tokendps.ToString(), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SSTATREG_DPS", OracleDbType.Int32, estadodps, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, result.P_COD_ERR, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, result.P_MESSAGE, ParameterDirection.Output);


                P_MESSAGE.Size = 9000;

                parameter.Add(P_COD_ERR);
                parameter.Add(P_MESSAGE);

                if (connection == null)
                {
                    this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                    result.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                    result.P_MESSAGE = P_MESSAGE.Value.ToString();
                }
                else
                {
                    this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, connection, trx);
                    result.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                    result.P_MESSAGE = P_MESSAGE.Value.ToString();
                }
            }
            catch (Exception ex)
            {
                result.P_COD_ERR = 1;
                result.P_MESSAGE = ex.ToString();
                LogControl.save("UpdateDPS", ex.ToString(), "3");
            }

            return result;
        }

        public DpsToken FindDPS(int nidcotizacion, int branch, int nproduct, DbConnection connection = null, DbTransaction trx = null)
        {
            var sPackageName = ProcedureName.pkg_Cotizacion + ".SP_GET_DATA_TOKEN";
            List<OracleParameter> parameter = new List<OracleParameter>();
            DpsToken result = new DpsToken();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, branch, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Varchar2, nproduct, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Varchar2, nidcotizacion, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_SID_DPS = new OracleParameter("P_SID_DPS", OracleDbType.Varchar2, result.id, ParameterDirection.Output);
                OracleParameter P_STOKEN_DPS = new OracleParameter("P_STOKEN_DPS", OracleDbType.Varchar2, result.key, ParameterDirection.Output);

                P_SID_DPS.Size = 9000;
                P_STOKEN_DPS.Size = 9000;

                parameter.Add(P_SID_DPS);
                parameter.Add(P_STOKEN_DPS);

                if (connection == null)
                {
                    this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                    result.id = P_SID_DPS.Value.ToString();
                    result.key = P_STOKEN_DPS.Value.ToString();
                }
                else
                {
                    this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, connection, trx);
                    result.id = P_SID_DPS.Value.ToString();
                    result.key = P_STOKEN_DPS.Value.ToString();
                }
            }
            catch (Exception ex)
            {
                result.id = "1";
                result.key = ex.ToString();
                LogControl.save("FindDPS", ex.ToString(), "3");
            }

            return result;
        }

        public QuotationResponseVM UpdateDatosPer(string sclient, string sproteg, DbConnection connection = null, DbTransaction trx = null)
        {
            var sPackageName = ProcedureName.pkg_ReaDataAP + ".UPD_SPROTEG_DATOS_IND";
            List<OracleParameter> parameter = new List<OracleParameter>();
            QuotationResponseVM result = new QuotationResponseVM();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, sclient, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SPROTEG_DATOS_IND", OracleDbType.Varchar2, sproteg, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_COD_ERR = new OracleParameter("P_NERROR", OracleDbType.Int32, result.P_COD_ERR, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_SMENSAJE", OracleDbType.Varchar2, result.P_MESSAGE, ParameterDirection.Output);


                P_MESSAGE.Size = 9000;

                parameter.Add(P_COD_ERR);
                parameter.Add(P_MESSAGE);

                if (connection == null)
                {
                    this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                    result.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                    result.P_MESSAGE = P_MESSAGE.Value.ToString();
                }
                else
                {
                    this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, connection, trx);
                    result.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                    result.P_MESSAGE = P_MESSAGE.Value.ToString();
                }
            }
            catch (Exception ex)
            {
                result.P_COD_ERR = 1;
                result.P_MESSAGE = ex.ToString();
                LogControl.save("UpdateDatosPer", ex.ToString(), "3");
            }

            return result;
        }

        public QuotationResponseVM ValCotizacion(int cotizacion, int usercode, int pendiente = 0, int cambioPlan = 0, DbConnection connection = null, DbTransaction trx = null, int flagCert = 0)
        {
            var sPackageName = ProcedureName.pkg_ValidaReglas + ".SPS_APROB_AUTOM_COTIZA";
            List<OracleParameter> parameter = new List<OracleParameter>();
            QuotationResponseVM result = new QuotationResponseVM();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, cotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, usercode, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPENDIENTE", OracleDbType.Int32, pendiente, ParameterDirection.Input)); // JDD


                //OUTPUT
                OracleParameter P_SAPROBADO = new OracleParameter("P_SAPROBADO", OracleDbType.Varchar2, result.P_SAPROBADO, ParameterDirection.Output);
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, result.P_NCODE, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, result.P_SMESSAGE, ParameterDirection.Output);

                P_SAPROBADO.Size = 9000;
                P_SMESSAGE.Size = 9000;

                parameter.Add(P_SAPROBADO);
                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);
                parameter.Add(new OracleParameter("P_NCAMBIOPLAN", OracleDbType.Int32, cambioPlan, ParameterDirection.Input)); // MSR
                parameter.Add(new OracleParameter("P_NFLAG_CERT", OracleDbType.Int32, flagCert, ParameterDirection.Input)); // MSR

                if (connection == null)
                {
                    this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                    result.P_SAPROBADO = P_SAPROBADO.Value.ToString();
                    result.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                    result.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                }
                else
                {
                    this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, connection, trx);
                    result.P_SAPROBADO = P_SAPROBADO.Value.ToString();
                    result.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                    result.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                }
            }
            catch (Exception ex)
            {
                result.P_NCODE = 1;
                result.P_SMESSAGE = ex.ToString();
                LogControl.save("ValCotizacion", ex.ToString(), "3");
            }

            return result;
        }

        public QuotationResponseVM ValTransaccionSCTR(int cotizacion, int usercode, string nidproc, DbConnection connection = null, DbTransaction trx = null)
        {
            var sPackageName = ProcedureName.pkg_ValidaReglas + ".SPS_VAL_REGLAS_SCTR";
            List<OracleParameter> parameter = new List<OracleParameter>();
            QuotationResponseVM result = new QuotationResponseVM();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, cotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, usercode, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, nidproc, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_SASIGNAR = new OracleParameter("P_SASIGNAR", OracleDbType.Varchar2, result.P_SASIGNAR, ParameterDirection.Output);
                OracleParameter P_SAPROBADO = new OracleParameter("P_SAPROBADO", OracleDbType.Varchar2, result.P_SAPROBADO, ParameterDirection.Output);
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, result.P_NCODE, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, result.P_SMESSAGE, ParameterDirection.Output);

                P_SASIGNAR.Size = 9000;
                P_SAPROBADO.Size = 9000;
                P_SMESSAGE.Size = 9000;

                parameter.Add(P_SASIGNAR);
                parameter.Add(P_SAPROBADO);
                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);

                if (connection == null)
                {
                    this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                    result.P_SASIGNAR = P_SASIGNAR.Value.ToString().Trim();
                    result.P_SAPROBADO = P_SAPROBADO.Value.ToString().Trim();
                    result.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                    result.P_SMESSAGE = P_SMESSAGE.Value.ToString().Trim();
                }
                else
                {
                    this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, connection, trx);
                    result.P_SASIGNAR = P_SASIGNAR.Value.ToString().Trim();
                    result.P_SAPROBADO = P_SAPROBADO.Value.ToString().Trim();
                    result.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                    result.P_SMESSAGE = P_SMESSAGE.Value.ToString().Trim();
                }
            }
            catch (Exception ex)
            {
                result.P_NCODE = 1;
                result.P_COD_ERR = 1;
                result.P_SMESSAGE = ex.ToString().Trim();
                LogControl.save("ValTransaccionSCTR", ex.ToString(), "3");
            }

            return result;
        }

        public QuotationResponseVM InsCotizaHisEstadoMatriz(QuotationCabBM request, string smessage, DbConnection connection = null, DbTransaction trx = null, int nmotivo = 10, int flag = 1)
        {
            var sPackageName = ProcedureName.pkg_Cotizacion + ".SPS_INS_COTIZA_HIS";
            List<OracleParameter> parameter = new List<OracleParameter>();
            QuotationResponseVM result = new QuotationResponseVM();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, request.NumeroCotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, request.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMOTIVO", OracleDbType.Int32, nmotivo, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOMMENT", OracleDbType.Varchar2, smessage, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SRUTA", OracleDbType.Varchar2, request.P_SRUTA, ParameterDirection.Input));


                //OUTPUT
                OracleParameter P_SAPROBADO = new OracleParameter("P_SAPROBADO", OracleDbType.Varchar2, result.P_SAPROBADO, ParameterDirection.Output);
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, result.P_NCODE, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, result.P_SMESSAGE, ParameterDirection.Output);

                P_SAPROBADO.Size = 9000;
                P_SMESSAGE.Size = 9000;

                parameter.Add(P_SAPROBADO);
                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);
                parameter.Add(new OracleParameter("P_NFLAG", OracleDbType.Int32, flag, ParameterDirection.Input));
                if (connection == null)
                {
                    this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                    result.P_SAPROBADO = P_SAPROBADO.Value.ToString();
                    result.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                    result.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                }
                else
                {
                    this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, connection, trx);
                    result.P_SAPROBADO = P_SAPROBADO.Value.ToString();
                    result.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                    result.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                }
            }
            catch (Exception ex)
            {
                result.P_NCODE = 1;
                result.P_SMESSAGE = ex.ToString();
                LogControl.save("InsCotizaHisEstadoMatriz", ex.ToString(), "3");
            }

            return result;
        }
        public int ValCotizacionReglas(validaTramaVM request)
        {
            var sPackageName = ProcedureName.pkg_Cotizacion + ".SP_VALIDA_REGLAS_PM_PD";
            List<OracleParameter> parameter = new List<OracleParameter>();
            QuotationResponseVM result = new QuotationResponseVM();
            int estadocot = 0;

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, request.nroCotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, request.codUsuario, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, request.codProducto, ParameterDirection.Input)); // JDD
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, request.codRamo, ParameterDirection.Input)); // JDD

                //OUTPUT
                OracleParameter P_SAPROBADO = new OracleParameter("P_SSTATREGT", OracleDbType.Varchar2, estadocot, ParameterDirection.Output);

                P_SAPROBADO.Size = 9000;
                parameter.Add(P_SAPROBADO);

                this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                estadocot = Convert.ToInt32(P_SAPROBADO.Value.ToString());
            }
            catch (Exception ex)
            {
                estadocot = 0;
                LogControl.save("ValCotizacionReglas", ex.ToString(), "3");
            }

            return estadocot;
        }

        public List<int> ValCantAseguradoPoliza(RecalculoTramaVM request)
        {
            List<int> lista = new List<int>();
            var sPackageName = ProcedureName.pkg_ValidadorGenPD + ".SP_VAL_ASEG_POL";
            List<OracleParameter> parameter = new List<OracleParameter>();
            QuotationResponseVM result = new QuotationResponseVM();
            int ncode = 0;
            int cant_aseg = 0;

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_CANT_ASEG", OracleDbType.Int64, request.CantidadTrabajadores, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_CODE", OracleDbType.Int32, ncode, ParameterDirection.Output);
                OracleParameter P_NCANT_ASEG = new OracleParameter("P_NCANT_ASEG_DET", OracleDbType.Int32, cant_aseg, ParameterDirection.Output);

                P_NCODE.Size = 9000;
                P_NCANT_ASEG.Size = 9000;

                parameter.Add(P_NCODE);
                parameter.Add(P_NCANT_ASEG);

                this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                lista.Add(Convert.ToInt32(P_NCODE.Value.ToString()));
                lista.Add(Convert.ToInt32(P_NCANT_ASEG.Value.ToString()));
            }
            catch (Exception ex)
            {
                LogControl.save("ValCantAseguradoPoliza", ex.ToString(), "3");
            }

            return lista;
        }

        public string GetProcessCodePD(string nroCotizacion)
        {
            var sPackageName = ProcedureName.sp_LeerCodProceso;
            string result = String.Empty;
            List<OracleParameter> parameters = new List<OracleParameter>();

            try
            {
                //INPUT
                parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int64, nroCotizacion, ParameterDirection.Input));
                OracleParameter P_NID_PROC = new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, 100, result, ParameterDirection.Output);

                parameters.Add(P_NID_PROC);
                this.ExecuteByStoredProcedureVT(sPackageName, parameters);
                result = P_NID_PROC.Value.ToString();

            }
            catch (Exception ex)
            {
                result = String.Empty;
                LogControl.save("GetProcessCodePD", ex.ToString(), "3");
            }

            return result;
        }

        public EmitPolVM ObtenerSkey(string nrocotizacion, string idproc)
        {
            var sPackageName = ProcedureName.sp_LeerKeyPM;
            List<OracleParameter> parameter = new List<OracleParameter>();
            EmitPolVM response = new EmitPolVM();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("PID_PROC", OracleDbType.Varchar2, idproc, ParameterDirection.Input));
                parameter.Add(new OracleParameter("PID_COTIZACION", OracleDbType.Int32, nrocotizacion, ParameterDirection.Input));

                OracleParameter P_SKEY = new OracleParameter("PSKEY", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(P_SKEY);
                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter);

                while (odr.Read())
                {
                    response.skey = odr["SKEY"].ToString();
                }
                response.cod_error = 0;

                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                response.cod_error = 1;
                response.message = ex.ToString();
                response.skey = string.Empty;
                LogControl.save("ObtenerSkey", ex.ToString(), "3");
            }

            return response;
        }

        public Entities.PolicyModel.ViewModel.SalidadPolizaEmit EmitPolicyPol(string cotizacion, string key)
        {
            var sPackageName = ProcedureName.sp_ProcesarEmisionPM;
            List<OracleParameter> parameter = new List<OracleParameter>();
            Entities.PolicyModel.ViewModel.SalidadPolizaEmit response = new Entities.PolicyModel.ViewModel.SalidadPolizaEmit();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_SKEY", OracleDbType.Varchar2, key, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, cotizacion, ParameterDirection.Input));

                this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                response.P_COD_ERR = 0;
                response.P_MESSAGE = "Se a realizado correctamente la emisión de la póliza matriz";
            }
            catch (Exception ex)
            {
                response.P_MESSAGE = ex.ToString();
                response.P_COD_ERR = 1;
                LogControl.save("EmitPolicyPol", ex.ToString(), "3");
            }

            return response;
        }

        //public EmitPolVM InsCoberturaDet(DatosPolizaVM data)
        //{
        //    var sPackageName = ProcedureName.pkg_ReaDataAP + ".CAL_PREMIUM_COVER_ALL";
        //    List<OracleParameter> parameter = new List<OracleParameter>();
        //    EmitPolVM response = new EmitPolVM();

        //    try
        //    {
        //        //INPUT
        //        parameter.Add(new OracleParameter("P_NIDPROD", OracleDbType.Varchar2, data.codProceso, ParameterDirection.Input));
        //        parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.branch, ParameterDirection.Input));
        //        parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, data.codproducto, ParameterDirection.Input));
        //        parameter.Add(new OracleParameter("P_NTYPE_PRODUCT", OracleDbType.Int32, data.codTipoNegocio, ParameterDirection.Input));
        //        parameter.Add(new OracleParameter("P_NTYPE_PROFILE", OracleDbType.Int32, data.codTipoPerfil, ParameterDirection.Input));
        //        parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, data.codTipoPlan, ParameterDirection.Input));
        //        parameter.Add(new OracleParameter("P_NCURRENCY", OracleDbType.Int32, data.codMon, ParameterDirection.Input));
        //        parameter.Add(new OracleParameter("P_STYPE_TRANSAC", OracleDbType.Int32, data.typeTransac, ParameterDirection.Input));
        //        parameter.Add(new OracleParameter("P_NMODALITY", OracleDbType.Int32, data.codTipoModalidad, ParameterDirection.Input));
        //        parameter.Add(new OracleParameter("P_NCIUU", OracleDbType.Int32, data.CodActividadRealizar, ParameterDirection.Input));
        //        parameter.Add(new OracleParameter("P_NTYPE_EMPLOYEE", OracleDbType.Varchar2, data.type_employee, ParameterDirection.Input));
        //        parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Varchar2, data.deffecdate, ParameterDirection.Input));
        //        parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, data.codusuario, ParameterDirection.Input));
        //        parameter.Add(new OracleParameter("P_NFLAG", OracleDbType.Int32, data.flag, ParameterDirection.Input));

        //        OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Varchar2, response.cod_error, ParameterDirection.Output);
        //        OracleParameter P_NMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, response.message, ParameterDirection.Output);

        //        P_NCODE.Size = 9000;
        //        P_NMESSAGE.Size = 9000;

        //        parameter.Add(P_NCODE);
        //        parameter.Add(P_NMESSAGE);

        //        this.ExecuteByStoredProcedureVT(sPackageName, parameter);
        //        response.cod_error = Convert.ToInt32(P_NCODE.Value.ToString());
        //        response.message = P_NMESSAGE.Value.ToString();
        //    }
        //    catch (Exception ex)
        //    {
        //        response.cod_error = 1;
        //        response.message = ex.ToString();
        //        ELog.save(this, ex.ToString());
        //    }

        //    return response;
        //}

        public string LimpiarTrama(string idproc)
        {
            var sPackageName = ProcedureName.pkg_ValidadorGenPD + ".LIMPIAR_TRAMA";
            List<OracleParameter> parameter = new List<OracleParameter>();
            string codError = "0";

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, idproc, ParameterDirection.Input));
                this.ExecuteByStoredProcedureVT(sPackageName, parameter);
            }
            catch (Exception ex)
            {
                codError = "1";
                LogControl.save("LimpiarTrama", ex.ToString(), "3");
            }

            return codError;
        }

        public string LimpiarTramaDes(string idproc)
        {
            var sPackageName = ProcedureName.pkg_ValidadorGenPD + ".LIMPIAR_TRAMA_DES";
            List<OracleParameter> parameter = new List<OracleParameter>();
            string codError = "0";

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, idproc, ParameterDirection.Input));
                this.ExecuteByStoredProcedureVT(sPackageName, parameter);
            }
            catch (Exception ex)
            {
                codError = "1";
                LogControl.save("LimpiarTramaDes", ex.ToString(), "3");
            }

            return codError;
        }

        public EmitPolVM UpdCotizacionDet(ProcesaTramaBM request)
        {
            var sPackageName = ProcedureName.pkg_Cotizacion + ".UPD_COTIZACION_DET_MATRIZ";
            List<OracleParameter> parameter = new List<OracleParameter>();
            var result = new EmitPolVM();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.idproc, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, request.nrocotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, request.nbranch, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, request.nproducto, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTASA_CALCULADA", OracleDbType.Decimal, Decimal.Parse(request.rate_cal), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTASA_PROP", OracleDbType.Decimal, Decimal.Parse(request.rate_prop), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPREMIUM_MIN", OracleDbType.Decimal, request.npremium_min, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPREMIUM_MIN_PR", OracleDbType.Decimal, request.npremium_min_pro, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPREMIUM_END", OracleDbType.Decimal, request.npremium_end, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, request.usercode, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NRATE", OracleDbType.Decimal, Decimal.Parse(request.nrate), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NDISCOUNT", OracleDbType.Decimal, request.ndiscount, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NACTIVITYVARIACTION", OracleDbType.Decimal, request.nvariaction, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FLAG", OracleDbType.Int32, request.nflag, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SID_TARIFARIO", OracleDbType.Varchar2, request.sid_tarifario, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NVERSION_TARIFARIO", OracleDbType.Int32, string.IsNullOrEmpty(request.nversion_tarifario) ? "0" : request.nversion_tarifario, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SNAME_TARIFARIO", OracleDbType.Varchar2, request.sname_tarifario, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, 9000, result.message, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, result.cod_error, ParameterDirection.Output);
                OracleParameter P_FLAG_AUT = new OracleParameter("P_FLAG_AUT", OracleDbType.Int32, result.flag_aut, ParameterDirection.Output);

                parameter.Add(P_MESSAGE);
                parameter.Add(P_COD_ERR);
                parameter.Add(P_FLAG_AUT);

                parameter.Add(new OracleParameter("P_NAMO_AFEC", OracleDbType.Double, request.namo_afec, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIVA", OracleDbType.Double, request.niva, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NAMOUNT", OracleDbType.Double, request.namount, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NDE", OracleDbType.Double, request.nde, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Int32, request.typeTransact, ParameterDirection.Input));

                //ARJG
                if (ELog.obtainConfig("desgravamenBranch") == request.nbranch.ToString())
                {
                    parameter.Add(new OracleParameter("P_TASA_SEGURO", OracleDbType.Varchar2, request.ntasa_seguro, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_TASA_AHORRO", OracleDbType.Varchar2, request.ntasa_ahorro, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_CUMULO_ASEG", OracleDbType.Varchar2, request.ncumulo_max, ParameterDirection.Input));
                }

                this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                result.message = P_MESSAGE.Value.ToString();
                result.cod_error = Convert.ToInt32(P_COD_ERR.Value.ToString());
                result.flag_aut = Convert.ToInt32(P_FLAG_AUT.Value.ToString());
            }
            catch (Exception ex)
            {
                result.message = ex.ToString();
                result.cod_error = 1;
                LogControl.save("UpdCotizacionDet", ex.ToString(), "3");
            }

            return result;
        }

        public QuotationResponseVM ValRetroCotizacion(QuotationCabBM request, int cotizacion = 0, DbConnection connection = null, DbTransaction trx = null)
        {
            var sPackageName = ProcedureName.pkg_ValidaReglas + ".SPS_APROB_RETRO_COTIZA";
            List<OracleParameter> parameter = new List<OracleParameter>();
            QuotationResponseVM result = new QuotationResponseVM();
            var flagRetroGob = new string[] { ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch"), ELog.obtainConfig("vidaLeyBranch") }.Contains(request.P_NBRANCH.ToString());
            if (!flagRetroGob) request.P_SISCLIENT_GBD = "0";

            try
            {
                //INPUT
                string trans = request.TrxCode = request.TrxCode ?? "EM";
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, request.P_NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, request.P_NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_OPERACION", OracleDbType.Int32, request.RetOP, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, cotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, request.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Date, (new string[] { "DE", "IN", "RE", "EX", "EN" }).Contains(trans) ? request.P_DSTARTDATE_ASE : request.P_DSTARTDATE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DCREATE", OracleDbType.Date, DateTime.Today.ToString("dd/MM/yyyy"), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STYPE_TRANSAC", OracleDbType.Varchar2, trans, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SISCLIENT_GBD", OracleDbType.Int32, Convert.ToInt32(request.P_SISCLIENT_GBD), ParameterDirection.Input)); //P_SCLIENT
                parameter.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, request.P_SCLIENT, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_SAPROBADO = new OracleParameter("P_SAPROBADO", OracleDbType.Varchar2, result.P_SAPROBADO, ParameterDirection.Output);
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, result.P_NCODE, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, result.P_SMESSAGE, ParameterDirection.Output);

                P_SAPROBADO.Size = 9000;
                P_SMESSAGE.Size = 9000;

                parameter.Add(P_SAPROBADO);
                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);

                parameter.Add(new OracleParameter("P_NMOD_FECHA", OracleDbType.Int32, request.FlagCambioFecha, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_END", OracleDbType.Int32, request.TipoEndoso, ParameterDirection.Input)); // ENDOSO TECNICA JTV 20022023

                if (connection == null)
                {
                    this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                    result.P_SAPROBADO = P_SAPROBADO.Value.ToString();
                    result.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                    result.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                }
                else
                {
                    this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, connection, trx);
                    result.P_SAPROBADO = P_SAPROBADO.Value.ToString();
                    result.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                    result.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                }

            }
            catch (Exception ex)
            {
                result.P_NCODE = 4;
                result.P_COD_ERR = 1;
                result.P_MESSAGE = "Error al consultar retroactividad - verifique la configuracion registrada para la transaccion.";
                result.P_SMESSAGE = "Error al consultar retroactividad - verifique la configuracion registrada para la transaccion.";
                LogControl.save("ValRetroCotizacion", ex.ToString(), "3");
            }

            return result;
        }

        public EmitPolVM TRAS_CARGA_ASE(ProcesaTramaBM request)
        {
            var sPackageName = ProcedureName.pkg_Poliza + ".TRAS_CARGA_ASE";
            List<OracleParameter> parameter = new List<OracleParameter>();
            QuotationResponseVM result = new QuotationResponseVM();
            EmitPolVM response = new EmitPolVM();
            string L_NTYPE_TRANSAC = "2";
            var ramos = new string[] { ELog.obtainConfig("vidaLeyBranch") };
            int flagEmisionCertificadosVL = ramos.Contains(request.nbranch) ? 1 : 0;

            LogControl.save("TRAS_CARGA_ASE", JsonConvert.SerializeObject(request) + "flagEmisionCertificadosVL:" + flagEmisionCertificadosVL.ToString(), "2");

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, request.nrocotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.idproc, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, request.usercode, ParameterDirection.Input));
                // SOLO RI

                parameter.Add(new OracleParameter("P_DSTARTDATE", OracleDbType.Date, Convert.ToDateTime(request.fechaini_aseg), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, Convert.ToDateTime(request.fechafin_aseg), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FACT_MES_VENCIDO", OracleDbType.Int32, 0, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Varchar2, L_NTYPE_TRANSAC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPAYFREQ", OracleDbType.Int32, request.idfrecuencia_pago, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DSTARTDATE_ASE", OracleDbType.Date, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEXPIRDAT_ASE", OracleDbType.Date, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NDEVOLVPRI", OracleDbType.Int32, 0, ParameterDirection.Input));  // Comentar en produccion? revisar si existe este parametro en bd de produccion
                parameter.Add(new OracleParameter("P_FLAG_EC", OracleDbType.Int32, flagEmisionCertificadosVL, ParameterDirection.Input));

                this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                response.cod_error = 0;
                response.message = "Se realizo correctamente el traslado de asegurados";
            }
            catch (Exception ex)
            {
                response.message = ex.ToString();
                response.cod_error = 1;
                LogControl.save("TRAS_CARGA_ASE", ex.ToString(), "3");
            }

            return response;
        }

        public EmitPolVM UpdateHeader(string idproc)
        {
            var response = new EmitPolVM();
            var sPackageName = ProcedureName.pkg_ValidadorGenPD + ".UPDATE_HEADER_POL";
            List<OracleParameter> parameter = new List<OracleParameter>();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, idproc, ParameterDirection.Input));
                this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                response.cod_error = 0;
                response.message = "Se realizó correctamente la actualización de la cabecera";
            }
            catch (Exception ex)
            {
                response.cod_error = 1;
                response.message = ex.ToString();
                LogControl.save("UpdateHeader", ex.ToString(), "3");
            }

            return response;
        }

        public EmitPolVM UpdateInsuredPremium_Matriz(ProcesaTramaBM request, int flagTecnica)
        {
            var response = new EmitPolVM();
            var sPackageName = ProcedureName.pkg_ValidadorGenPD + ".UPDATE_INSURED_PREMIUM_MATRIZ";
            List<OracleParameter> parameter = new List<OracleParameter>();
            try
            {

                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, request.nrocotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.idproc, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, request.usercode, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTECNICA", OracleDbType.Int32, flagTecnica, ParameterDirection.Input)); //AVS - RENTAS

                this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                response.cod_error = 0;
                response.message = "Se Asocio la cotización correctamente";
            }
            catch (Exception ex)
            {
                response.cod_error = 1;
                response.message = ex.ToString();
                LogControl.save("UpdateInsuredPremium_Matriz", ex.ToString(), "3");
            }

            return response;
        }


        public EmitPolVM INS_JOB_CLOSE_SCTR(ProcesaTramaBM request)
        {
            var sPackageName = ProcedureName.pkg_Poliza + ".INS_JOB_CLOSE_SCTR";
            List<OracleParameter> parameter = new List<OracleParameter>();
            EmitPolVM response = new EmitPolVM();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, request.nrocotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Date, request.fechaini_poliza, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, request.fechafin_poliza, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, request.usercode, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Varchar2, request.typeTransact, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.idproc, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SRECEIPT_IND", OracleDbType.Varchar2, request.p_sreceipt_ind, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SFLAG_FAC_ANT", OracleDbType.Int32, request.p_sflag_fac_ant, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOLTIMRE", OracleDbType.Varchar2, request.coltimre, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPAYFREQ", OracleDbType.Int32, request.idfrecuencia_pago, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMOV_ANUL", OracleDbType.Int32, request.mov_anul, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NNULLCODE", OracleDbType.Int32, 0, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOMMENT", OracleDbType.Varchar2, "", ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SRUTA", OracleDbType.Varchar2, request.ruta, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDPAYMENT", OracleDbType.Int32, request.idpayment, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_SMESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, 9000, response.message, ParameterDirection.Output);
                OracleParameter P_NCOD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, 9000, response.cod_error, ParameterDirection.Output);
                OracleParameter P_NCONSTANCIA = new OracleParameter("P_NCONSTANCIA", OracleDbType.Int32, 9000, response.constancia, ParameterDirection.Output);

                parameter.Add(P_SMESSAGE);
                parameter.Add(P_NCOD_ERR);
                parameter.Add(P_NCONSTANCIA);
                // SOLO RI
                parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int64, request.policy, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DSTARTDATE_POL", OracleDbType.Date, Convert.ToDateTime(request.fechaini_poliza), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEXPIRDAT_POL", OracleDbType.Date, Convert.ToDateTime(request.fechafin_poliza), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NAMO_AFEC", OracleDbType.Double, request.namo_afec, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIVA", OracleDbType.Double, request.niva, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NAMOUNT", OracleDbType.Double, request.namount, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NDE", OracleDbType.Double, request.nde, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DSTARTDATE_ASE", OracleDbType.Date, Convert.ToDateTime(request.fechaini_aseg), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEXPIRDAT_ASE", OracleDbType.Date, Convert.ToDateTime(request.fechafin_aseg), ParameterDirection.Input));



                this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                response.message = P_SMESSAGE.Value.ToString();
                response.cod_error = Convert.ToInt32(P_NCOD_ERR.Value.ToString());
                response.constancia = Convert.ToInt32(P_NCONSTANCIA.Value.ToString());
            }
            catch (Exception ex)
            {
                response.message = ex.ToString();
                response.cod_error = 1;
                LogControl.save("INS_JOB_CLOSE_SCTR", ex.ToString(), "3");
            }

            return response;
        }

        /// <summary>
        /// Proceso de recotización
        /// </summary>
        /// <param name="quotationData">Datos de cotización</param>
        /// <param name="fileList">Archivos de cambio de estado de cotización</param>
        /// <returns></returns>
        public QuotationResponseVM ModifyQuotation(QuotationModification quotationData, List<HttpPostedFile> statusChangeFileList)
        {
            QuotationResponseVM resultPackage = new QuotationResponseVM();
            var connectionString = ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString();
            using (var connection = new OracleConnection(connectionString))
            {
                connection.Open();
                bool shouldContinue = true;
                var transaction = connection.BeginTransaction();
                try
                {

                    for (int i = 0; i < quotationData.RiskList.Count; i++)
                    {
                        Insertion insertion = InsertQuotationRisk(quotationData.RiskList[i], quotationData, transaction, connection);
                        if (insertion.StatusCode != 0)
                        {
                            shouldContinue = false;
                            resultPackage.P_MESSAGE = insertion.ErrorMessageList != null && insertion.ErrorMessageList.Count > 0 ? insertion.ErrorMessageList.ElementAt(0) : "Ha ocurrido un error inesperado en la inserción de los riesgos.";
                        }
                    }
                    if (shouldContinue)
                    {
                        for (int i = 0; i < quotationData.BrokerList.Count; i++)
                        {
                            Insertion insertion = InsertQuotationBroker(quotationData.BrokerList[i], quotationData, transaction, connection);
                            if (insertion.StatusCode != 0)
                            {
                                shouldContinue = false;
                                resultPackage.P_MESSAGE = insertion.ErrorMessageList != null && insertion.ErrorMessageList.Count > 0 ? insertion.ErrorMessageList.ElementAt(0) : "Ha ocurrido un error inesperado en la inserción del broker.";
                            }
                        }
                    }

                    if (shouldContinue)
                    {
                        DbCommand subCommand = connection.CreateCommand();
                        subCommand.CommandType = CommandType.StoredProcedure;
                        subCommand.CommandText = ProcedureName.pkg_Cotizacion + ".UPD_COTIZACION_CLOSE";
                        subCommand.Transaction = transaction;

                        subCommand.Parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, quotationData.Number, ParameterDirection.Input));
                        subCommand.Parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, quotationData.User, ParameterDirection.Input));

                        subCommand.ExecuteNonQuery();
                    }

                    if (shouldContinue)
                    {
                        if (AddStatusChange(quotationData.StatusChangeData, statusChangeFileList, transaction, connection).StatusCode != 0) shouldContinue = false;
                    }

                    if (shouldContinue)
                    {
                        transaction.Commit();
                        transaction.Dispose();
                        connection.Close();
                        connection.Dispose();

                        QuotationResponseVM response = ValCotizacion(Int32.Parse(quotationData.Number), Int32.Parse(quotationData.User));
                        response.P_NID_COTIZACION = quotationData.Number;

                        return response;
                    }
                    else
                    {
                        resultPackage.P_COD_ERR = 1;
                        transaction.Rollback();
                        transaction.Dispose();
                        connection.Close();
                        connection.Dispose();
                        return resultPackage;
                    }
                }
                catch (Exception ex)
                {

                    transaction.Rollback();
                    connection.Close();
                    connection.Dispose();
                    LogControl.save("ModifyQuotation", ex.ToString(), "3");
                    return resultPackage;
                }

            }
        }

        private Insertion InsertQuotationRisk(QuotationRisk riskDetail, QuotationModification quotationData, OracleTransaction transaction, DbConnection connection)
        {
            Insertion result = new Insertion();

            try
            {
                string storedProcedureName = ProcedureName.pkg_Cotizacion + ".INS_COTIZACION_DET";
                List<OracleParameter> parameter = new List<OracleParameter>();
                DbCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.Connection = connection;
                command.CommandText = storedProcedureName;
                command.Transaction = transaction;

                command.Parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, quotationData.Number, ParameterDirection.Input));
                command.Parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, quotationData.Branch, ParameterDirection.Input));
                command.Parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, riskDetail.ProductTypeId, ParameterDirection.Input));
                command.Parameters.Add(new OracleParameter("P_NMODULEC", OracleDbType.Varchar2, riskDetail.RiskTypeId, ParameterDirection.Input));
                command.Parameters.Add(new OracleParameter("P_NTOTAL_TRABAJADORES", OracleDbType.Int64, riskDetail.WorkersCount, ParameterDirection.Input));
                command.Parameters.Add(new OracleParameter("P_NMONTO_PLANILLA", OracleDbType.Decimal, riskDetail.PayrollAmount, ParameterDirection.Input));
                command.Parameters.Add(new OracleParameter("P_NTASA_CALCULADA", OracleDbType.Decimal, riskDetail.CalculatedRate, ParameterDirection.Input));
                command.Parameters.Add(new OracleParameter("P_NTASA_PROP", OracleDbType.Decimal, riskDetail.ProposedRate, ParameterDirection.Input));
                command.Parameters.Add(new OracleParameter("P_NPREMIUM_MENSUAL", OracleDbType.Decimal, riskDetail.PremimunPerRisk, ParameterDirection.Input));
                command.Parameters.Add(new OracleParameter("P_NPREMIUM_MIN", OracleDbType.Decimal, riskDetail.MinimunPremium, ParameterDirection.Input));
                command.Parameters.Add(new OracleParameter("P_NPREMIUM_MIN_PR", OracleDbType.Decimal, riskDetail.ProposedMinimunPremium, ParameterDirection.Input));
                command.Parameters.Add(new OracleParameter("P_NPREMIUM_END", OracleDbType.Decimal, riskDetail.EndorsmentPremium, ParameterDirection.Input));
                command.Parameters.Add(new OracleParameter("P_NSUM_PREMIUMN", OracleDbType.Decimal, riskDetail.NetPremium, ParameterDirection.Input));
                command.Parameters.Add(new OracleParameter("P_NSUM_IGV", OracleDbType.Decimal, riskDetail.PremiumIGV, ParameterDirection.Input));
                command.Parameters.Add(new OracleParameter("P_NSUM_PREMIUM", OracleDbType.Decimal, riskDetail.GrossPremium, ParameterDirection.Input));
                command.Parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, quotationData.User, ParameterDirection.Input));
                command.Parameters.Add(new OracleParameter("P_NRATE", OracleDbType.Decimal, riskDetail.RiskRate, ParameterDirection.Input));
                command.Parameters.Add(new OracleParameter("P_NDISCOUNT", OracleDbType.Decimal, Double.Parse(riskDetail.Discount), ParameterDirection.Input));
                command.Parameters.Add(new OracleParameter("P_NACTIVITYVARIACTION", OracleDbType.Decimal, Double.Parse(riskDetail.Variation), ParameterDirection.Input));
                command.Parameters.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, "", ParameterDirection.Input));
                command.Parameters.Add(new OracleParameter("P_FLAG", OracleDbType.Int32, riskDetail.TariffFlag, ParameterDirection.Input));

                command.Parameters.Add(new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, 9000, null, ParameterDirection.Output));
                command.Parameters.Add(new OracleParameter("P_COD_ERR", OracleDbType.Int32, 4000, null, ParameterDirection.Output));
                command.Parameters.Add(new OracleParameter("P_FLAG_AUT", OracleDbType.Int32, 4000, null, ParameterDirection.Output));

                command.ExecuteNonQuery();
                result.StatusCode = Convert.ToInt32(command.Parameters["P_COD_ERR"].Value.ToString());
                result.ErrorMessageList = sharedMethods.StringToList(command.Parameters["P_MESSAGE"].Value.ToString());
            }
            catch (Exception ex)
            {
                result.StatusCode = 1;
                LogControl.save("InsertQuotationRisk", ex.ToString(), "3");
            }

            return result;
        }
        private Insertion InsertQuotationBroker(QuotationBroker broker, QuotationModification quotationData, OracleTransaction transaction, DbConnection connection)
        {
            Insertion result = new Insertion();

            try
            {
                string storedProcedureName = ProcedureName.pkg_Cotizacion + ".INS_COTIZACION_COMER";
                List<OracleParameter> parameter = new List<OracleParameter>();
                DbCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.Connection = connection;
                command.CommandText = storedProcedureName;
                command.Transaction = transaction;

                command.Parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, quotationData.Number, ParameterDirection.Input));
                command.Parameters.Add(new OracleParameter("P_NIDTYPECHANNEL", OracleDbType.Int32, broker.ChannelTypeId, ParameterDirection.Input));
                command.Parameters.Add(new OracleParameter("P_NINTERMED", OracleDbType.Int32, broker.ChannelId, ParameterDirection.Input));
                command.Parameters.Add(new OracleParameter("P_SCLIENT_COMER", OracleDbType.Varchar2, broker.ClientId, ParameterDirection.Input));
                command.Parameters.Add(new OracleParameter("P_NCOMISION_SAL", OracleDbType.Decimal, broker.HealthCommission, ParameterDirection.Input));
                command.Parameters.Add(new OracleParameter("P_NCOMISION_SAL_PR", OracleDbType.Decimal, broker.HealthProposedCommission, ParameterDirection.Input));
                command.Parameters.Add(new OracleParameter("P_NCOMISION_PEN", OracleDbType.Decimal, broker.PensionCommission, ParameterDirection.Input));
                command.Parameters.Add(new OracleParameter("P_NCOMISION_PEN_PR", OracleDbType.Decimal, broker.PensionProposedCommission, ParameterDirection.Input));
                command.Parameters.Add(new OracleParameter("P_NPRINCIPAL", OracleDbType.Int32, broker.IsPrincipal == true ? 1 : 0, ParameterDirection.Input));
                command.Parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, quotationData.User, ParameterDirection.Input));
                command.Parameters.Add(new OracleParameter("P_NSHARE", OracleDbType.Int32, broker.SharedCommission, ParameterDirection.Input));

                command.Parameters.Add(new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, 9000, null, ParameterDirection.Output));
                command.Parameters.Add(new OracleParameter("P_COD_ERR", OracleDbType.Int32, 4000, null, ParameterDirection.Output));

                command.ExecuteNonQuery();
                result.StatusCode = Convert.ToInt32(command.Parameters["P_COD_ERR"].Value.ToString());
                result.ErrorMessageList = sharedMethods.StringToList(command.Parameters["P_MESSAGE"].Value.ToString());
            }
            catch (Exception ex)
            {
                result.StatusCode = 1;
                LogControl.save("InsertQuotationBroker", ex.ToString(), "3");
            }

            return result;
        }
        public QuotationResponseVM InsertQuotationCab(QuotationCabBM request, DbConnection connection, DbTransaction trx)
        {
            var sPackageName = ProcedureName.pkg_Cotizacion + ".INS_COTIZACION_CAB";

            List<OracleParameter> parameter = new List<OracleParameter>();
            QuotationResponseVM result = new QuotationResponseVM();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_SCLIENT", OracleDbType.Char, request.P_SCLIENT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCURRENCY", OracleDbType.Int32, request.P_NCURRENCY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, request.P_NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DSTARTDATE", OracleDbType.Date, Convert.ToDateTime(request.P_DSTARTDATE), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, request.P_DEXPIRDAT != null ? Convert.ToDateTime(request.P_DEXPIRDAT) : Convert.ToDateTime(request.P_DSTARTDATE), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDCLIENTLOCATION", OracleDbType.Int64, request.P_NIDCLIENTLOCATION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOMMENT", OracleDbType.Varchar2, request.P_SCOMMENT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SRUTA", OracleDbType.Varchar2, request.P_SRUTA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, request.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NACT_MINA", OracleDbType.Int32, request.P_NACT_MINA, ParameterDirection.Input));
                // Vida Ley
                parameter.Add(new OracleParameter("P_NTIP_RENOV", OracleDbType.Int32, request.P_NTIP_RENOV == 0 ? null : request.P_NTIP_RENOV, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPAYFREQ", OracleDbType.Int32, request.P_NPAYFREQ == 0 ? null : request.P_NPAYFREQ, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOD_ACTIVITY_TEC", OracleDbType.Varchar2, request.P_SCOD_ACTIVITY_TEC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOD_CIUU", OracleDbType.Varchar2, request.P_SCOD_CIUU, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTIP_NCOMISSION", OracleDbType.Int32, request.P_NTIP_NCOMISSION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, request.P_NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCOMISION_SAL_PR", OracleDbType.Int32, request.P_NCOMISION_SAL_PR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DSTARTDATE_ASE", OracleDbType.Date, request.P_DSTARTDATE_ASE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEXPIRDAT_ASE", OracleDbType.Date, request.P_DEXPIRDAT_ASE, ParameterDirection.Input));
                // Mapfre
                parameter.Add(new OracleParameter("P_NCOMPANY_LNK", OracleDbType.Int32, request.P_NEPS, ParameterDirection.Input)); // JDD
                parameter.Add(new OracleParameter("P_SCOTIZA_LNK", OracleDbType.Varchar2, request.P_QUOTATIONNUMBER_EPS, ParameterDirection.Input)); // JDD
                parameter.Add(new OracleParameter("P_NIDPLAN", OracleDbType.Int32, request.planId, ParameterDirection.Input)); // COVID
                parameter.Add(new OracleParameter("P_NCOBERTURA_ADICIONAL", OracleDbType.Int32, request.P_NCOBERTURA_ADICIONAL, ParameterDirection.Input)); // COVID
                //OUTPUT
                OracleParameter P_NID_COTIZACION = new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, result.P_NID_COTIZACION, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, result.P_MESSAGE, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, result.P_COD_ERR, ParameterDirection.Output);

                P_NID_COTIZACION.Size = 9000;
                P_MESSAGE.Size = 9000;
                P_COD_ERR.Size = 9000;

                parameter.Add(P_NID_COTIZACION);
                parameter.Add(P_MESSAGE);
                parameter.Add(P_COD_ERR);

                parameter.Add(new OracleParameter("P_NREM_EXC", OracleDbType.Int32, request.P_NREM_EXC, ParameterDirection.Input)); // rq Exc EH
                parameter.Add(new OracleParameter("P_SPOL_ESTADO", OracleDbType.Int32, request.P_SPOL_ESTADO, ParameterDirection.Input)); //  Tramite de estado VL
                parameter.Add(new OracleParameter("P_SPOL_MATRIZ", OracleDbType.Int32, request.P_SPOL_MATRIZ, ParameterDirection.Input)); // Tramite de estado VL
                parameter.Add(new OracleParameter("P_NCOT_MIXTA", OracleDbType.Int32, request.P_NCOT_MIXTA, ParameterDirection.Input)); // AVS

                this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, connection, trx);
                result.P_NID_COTIZACION = P_NID_COTIZACION.Value.ToString();
                result.P_MESSAGE = P_MESSAGE.Value.ToString();
                result.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
            }
            catch (Exception ex)
            {
                result.P_MESSAGE = ex.ToString();
                result.P_COD_ERR = 1;
                LogControl.save("InsertQuotationCab", ex.ToString(), "3");
            }

            return result;
        }

        public QuotationResponseVM InsertQuotationCabAP(QuotationCabBM request, DbConnection connection, DbTransaction trx)
        {
            var sPackageName = ProcedureName.pkg_Cotizacion + ".INS_COTIZACION_CAB_PD";

            List<OracleParameter> parameter = new List<OracleParameter>();
            QuotationResponseVM result = new QuotationResponseVM();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_SCLIENT", OracleDbType.Char, request.P_SCLIENT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCURRENCY", OracleDbType.Int32, request.P_NCURRENCY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, request.P_NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DSTARTDATE", OracleDbType.Date, Convert.ToDateTime(request.P_DSTARTDATE), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, request.P_DEXPIRDAT != null ? Convert.ToDateTime(request.P_DEXPIRDAT) : Convert.ToDateTime(request.P_DSTARTDATE), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDCLIENTLOCATION", OracleDbType.Int64, request.P_NIDCLIENTLOCATION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOMMENT", OracleDbType.Varchar2, request.P_SCOMMENT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SRUTA", OracleDbType.Varchar2, request.P_SRUTA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, request.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NACT_MINA", OracleDbType.Int32, request.P_NACT_MINA, ParameterDirection.Input));
                // Vida Ley
                parameter.Add(new OracleParameter("P_NTIP_RENOV", OracleDbType.Int32, request.P_NTIP_RENOV == 0 ? null : request.P_NTIP_RENOV, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPAYFREQ", OracleDbType.Int32, request.P_NPAYFREQ == 0 ? null : request.P_NPAYFREQ, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOD_ACTIVITY_TEC", OracleDbType.Varchar2, request.P_SCOD_ACTIVITY_TEC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOD_CIUU", OracleDbType.Varchar2, request.P_SCOD_CIUU, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTIP_NCOMISSION", OracleDbType.Int32, request.P_NTIP_NCOMISSION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, request.P_NPRODUCT, ParameterDirection.Input));
                /*parameter.Add(new OracleParameter("P_NCOMISION_SAL_PR", OracleDbType.Int32, request.P_NCOMISION_SAL_PR, ParameterDirection.Input));*/ // Revisar en homologacion
                parameter.Add(new OracleParameter("P_DSTARTDATE_ASE", OracleDbType.Date, request.P_DSTARTDATE_ASE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEXPIRDAT_ASE", OracleDbType.Date, request.P_DEXPIRDAT_ASE, ParameterDirection.Input));
                // Mapfre
                parameter.Add(new OracleParameter("P_NCOMPANY_LNK", OracleDbType.Int32, request.P_NEPS, ParameterDirection.Input)); // JDD
                parameter.Add(new OracleParameter("P_SCOTIZA_LNK", OracleDbType.Varchar2, request.P_QUOTATIONNUMBER_EPS, ParameterDirection.Input)); // JDD
                parameter.Add(new OracleParameter("P_NIDPLAN", OracleDbType.Int32, request.planId, ParameterDirection.Input)); // COVID
                // parameter.Add(new OracleParameter("P_NIDPLAN", OracleDbType.Int32, request.P_NIDPLAN, ParameterDirection.Input)); // No hay en produccion
                parameter.Add(new OracleParameter("P_NPOLIZA_MATRIZ", OracleDbType.Int32, request.P_NPOLIZA_MATRIZ, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NFACTURA_ANTICIPADA", OracleDbType.Int32, request.P_NFACTURA_ANTICIPADA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCOBERTURA_ADICIONAL", OracleDbType.Int32, request.P_NCOBERTURA_ADICIONAL, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_PROFILE", OracleDbType.Int32, request.P_NTYPE_PROFILE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_PRODUCT", OracleDbType.Int32, request.P_NTYPE_PRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_MODALITY", OracleDbType.Int32, request.P_NTYPE_MODALITY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTIPO_FACTURACION", OracleDbType.Int32, request.P_NTIPO_FACTURACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.CodigoProceso, ParameterDirection.Input));

                // Agregar los campos nuevos del tarifario
                parameter.Add(new OracleParameter("P_NDERIVA_TECNICA", OracleDbType.Int32, request.P_NDERIVA_TECNICA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTEMPORALITY", OracleDbType.Int32, request.P_NTEMPORALITY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NSCOPE", OracleDbType.Int32, request.P_NSCOPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_LOCATION", OracleDbType.Int32, request.P_NTYPE_LOCATION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLOCATION", OracleDbType.Int32, request.P_NLOCATION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMODULEC_TARIFARIO", OracleDbType.Int32, request.P_NMODULEC_TARIFARIO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SDESCRIPT_TARIFARIO", OracleDbType.Varchar2, request.P_SDESCRIPT_TARIFARIO, ParameterDirection.Input));

                /* SOLO DESA - DESGRAVAMEN */
                parameter.Add(new OracleParameter("P_SCLIENT_PROVIDER", OracleDbType.Varchar2, request.P_SCLIENT_PROVIDER, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NNUM_CREDIT", OracleDbType.Int64, request.P_NNUM_CREDIT, ParameterDirection.Input)); // AGF 23052023 Cambio tipo variable
                parameter.Add(new OracleParameter("P_NNUM_CUOTA", OracleDbType.Int32, request.P_NNUM_CUOTA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DFEC_OTORGAMIENTO", OracleDbType.Date, request.P_DFEC_OTORGAMIENTO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCAPITAL", OracleDbType.Int32, request.P_NCAPITAL, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SAPLICACION", OracleDbType.Varchar2, request.P_SAPLICACION, ParameterDirection.Input));
                // nuevo valor para tipo de renovacion
                parameter.Add(new OracleParameter("P_STIMEREN ", OracleDbType.Varchar2, request.P_STIMEREN, ParameterDirection.Input));
                // Datos de DPS
                //if (request.P_NBRANCH.ToString() == ELog.obtainConfig("vidaIndividualBranch"))
                //{
                parameter.Add(new OracleParameter("P_SID_DPS", OracleDbType.Varchar2, request.P_SID_DPS, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STOKEN_DPS", OracleDbType.Varchar2, request.P_STOKEN_DPS, ParameterDirection.Input));
                //}

                //OUTPUT
                OracleParameter P_NID_COTIZACION = new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, 9000, result.P_NID_COTIZACION, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, 9000, result.P_MESSAGE, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, 9000, result.P_COD_ERR, ParameterDirection.Output);

                parameter.Add(P_NID_COTIZACION);
                parameter.Add(P_MESSAGE);
                parameter.Add(P_COD_ERR);

                parameter.Add(new OracleParameter("P_OCCUPATION", OracleDbType.Int32, Convert.ToInt32(request.P_OCCUPATION), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_ACTIVITY", OracleDbType.Int32, Convert.ToInt32(request.P_ACTIVITY), ParameterDirection.Input));

                parameter.Add(new OracleParameter("P_NPENSIONES_RE", OracleDbType.Int32, Convert.ToInt32(request.P_NPENSIONES_RE), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NASIG_COLEGIO", OracleDbType.Int32, Convert.ToInt32(request.P_NASIG_COLEGIO), ParameterDirection.Input));

                this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, connection, trx);
                result.P_NID_COTIZACION = P_NID_COTIZACION.Value.ToString();
                result.P_MESSAGE = P_MESSAGE.Value.ToString();
                result.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
            }
            catch (Exception ex)
            {
                result.P_MESSAGE = ex.ToString();
                result.P_COD_ERR = 1;
                LogControl.save("InsertQuotationCabAP", ex.ToString(), "3");
            }

            return result;
        }

        public QuotationResponseVM InsertQuotationDet(QuotationCabBM request, QuotationDetBM request2, DbConnection connection, DbTransaction trx)
        {
            var sPackageName = ProcedureName.pkg_Cotizacion + ".INS_COTIZACION_DET";
            List<OracleParameter> parameter = new List<OracleParameter>();
            QuotationResponseVM result = new QuotationResponseVM();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, request2.P_NID_COTIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, request.P_NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, request2.P_NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Varchar2, request2.P_NMODULEC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTOTAL_TRABAJADORES", OracleDbType.Int64, request2.P_NTOTAL_TRABAJADORES, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMONTO_PLANILLA", OracleDbType.Decimal, request2.P_NMONTO_PLANILLA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTASA_CALCULADA", OracleDbType.Decimal, request2.P_NTASA_CALCULADA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTASA_PROP", OracleDbType.Decimal, request2.P_NTASA_PROP, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPREMIUM_MENSUAL", OracleDbType.Decimal, request2.P_NPREMIUM_MENSUAL, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPREMIUM_MIN", OracleDbType.Decimal, request2.P_NPREMIUM_MIN, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPREMIUM_MIN_PR", OracleDbType.Decimal, request2.P_NPREMIUM_MIN_PR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPREMIUM_END", OracleDbType.Decimal, request2.P_NPREMIUM_END, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NSUM_PREMIUMN", OracleDbType.Decimal, request2.P_NSUM_PREMIUMN, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NSUM_IGV", OracleDbType.Decimal, request2.P_NSUM_IGV, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NSUM_PREMIUM", OracleDbType.Decimal, request2.P_NSUM_PREMIUM, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, request.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NRATE", OracleDbType.Decimal, request2.P_NRATE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NDISCOUNT", OracleDbType.Decimal, request2.P_NDISCOUNT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NACTIVITYVARIACTION", OracleDbType.Decimal, request2.P_NACTIVITYVARIATION, ParameterDirection.Input));
                //parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.CodigoProceso, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Date, request.P_DSTARTDATE_ASE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FLAG", OracleDbType.Int32, request2.P_FLAG, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, result.P_MESSAGE, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, result.P_COD_ERR, ParameterDirection.Output);
                OracleParameter P_FLAG_AUT = new OracleParameter("P_FLAG_AUT", OracleDbType.Int32, result.P_FLAG_AUT, ParameterDirection.Output);

                P_MESSAGE.Size = 9000;

                parameter.Add(P_MESSAGE);
                parameter.Add(P_COD_ERR);
                parameter.Add(P_FLAG_AUT);

                parameter.Add(new OracleParameter("P_NAMO_AFEC", OracleDbType.Decimal, request2.P_NAMO_AFEC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIVA", OracleDbType.Decimal, request2.P_NIVA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NAMOUNT", OracleDbType.Decimal, request2.P_NAMOUNT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NDE", OracleDbType.Decimal, request2.P_NDE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_RANGO", OracleDbType.Varchar2, request2.P_RANGO, ParameterDirection.Input));
                //RI
                //parameter.Add(new OracleParameter("P_STIPO_APP ", OracleDbType.Varchar2, request.P_STIPO_APP, ParameterDirection.Input));           //RI
                //parameter.Add(new OracleParameter("P_RANGO", OracleDbType.Varchar2, "", ParameterDirection.Input));

                this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, connection, trx);
                result.P_MESSAGE = P_MESSAGE.Value.ToString();
                result.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                result.P_FLAG_AUT = Convert.ToInt32(P_FLAG_AUT.Value.ToString());
            }
            catch (Exception ex)
            {
                result.P_MESSAGE = ex.ToString();
                result.P_COD_ERR = 1;
                LogControl.save("InsertQuotationDet", ex.ToString(), "3");
            }

            return result;
        }

        public QuotationResponseVM InsertQuotationDetAP(QuotationCabBM request, QuotationDetBM request2, DbConnection connection, DbTransaction trx)
        {
            var sPackageName = ProcedureName.pkg_Cotizacion + ".INS_COTIZACION_DET_PD";
            List<OracleParameter> parameter = new List<OracleParameter>();
            QuotationResponseVM result = new QuotationResponseVM();
            var primaTasa = request2.P_TIPO_COT == "RATE" ? "1" : "0"; // TASA CALCULADA

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.CodigoProceso, ParameterDirection.Input)); // Revisar orden en producció
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, request2.P_NID_COTIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, request.P_NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, request.P_NPRODUCT, ParameterDirection.Input));
                /*parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Varchar2, request2.P_NMODULEC, ParameterDirection.Input));*/ // Revisar en homologacion con prod
                parameter.Add(new OracleParameter("P_NTOTAL_TRABAJADORES", OracleDbType.Int64, request2.P_NTOTAL_TRABAJADORES, ParameterDirection.Input));
                /*parameter.Add(new OracleParameter("P_NMONTO_PLANILLA", OracleDbType.Decimal, request2.P_NMONTO_PLANILLA, ParameterDirection.Input));*/ // Revisar en homologacion con prod
                parameter.Add(new OracleParameter("P_NTASA_CALCULADA", OracleDbType.Decimal, request2.P_NTASA_CALCULADA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTASA_PROP", OracleDbType.Decimal, request2.P_NTASA_PROP, ParameterDirection.Input));
                /*parameter.Add(new OracleParameter("P_NPREMIUM_MENSUAL", OracleDbType.Decimal, request2.P_NPREMIUM_MENSUAL, ParameterDirection.Input));*/ // Revisar en homologacion con prod
                parameter.Add(new OracleParameter("P_NPREMIUM_MIN", OracleDbType.Decimal, request2.P_NPREMIUM_MIN, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPREMIUM_MIN_PR", OracleDbType.Decimal, request2.P_NPREMIUM_MIN_PR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPREMIUM_END", OracleDbType.Decimal, request2.P_NPREMIUM_END, ParameterDirection.Input));
                /*parameter.Add(new OracleParameter("P_NSUM_PREMIUMN", OracleDbType.Decimal, request2.P_NSUM_PREMIUMN, ParameterDirection.Input)); // Revisar en homologacion con prod
                parameter.Add(new OracleParameter("P_NSUM_IGV", OracleDbType.Decimal, request2.P_NSUM_IGV, ParameterDirection.Input)); // Revisar en homologacion con prod
                parameter.Add(new OracleParameter("P_NSUM_PREMIUM", OracleDbType.Decimal, request2.P_NSUM_PREMIUM, ParameterDirection.Input));*/
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, request.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NRATE", OracleDbType.Decimal, request2.P_NRATE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NDISCOUNT", OracleDbType.Decimal, request2.P_NDISCOUNT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NACTIVITYVARIACTION", OracleDbType.Decimal, request2.P_NACTIVITYVARIATION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Date, request.P_DSTARTDATE_ASE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FLAG", OracleDbType.Int32, request2.P_FLAG, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPOLIZA_MATRIZ", OracleDbType.Int32, request.P_NPOLIZA_MATRIZ, ParameterDirection.Input));

                parameter.Add(new OracleParameter("P_SID_TARIFARIO", OracleDbType.Varchar2, request2.P_SID_TARIFARIO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NVERSION_TARIFARIO", OracleDbType.Int32, string.IsNullOrEmpty(request2.P_NVERSION_TARIFARIO) ? "0" : request2.P_NVERSION_TARIFARIO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SNAME_TARIFARIO", OracleDbType.Varchar2, request2.P_SNAME_TARIFARIO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCOT_TARIFARIO", OracleDbType.Varchar2, request2.P_NCOT_TARIFARIO, ParameterDirection.Input));

                /* SOLO DESA - RI */
                parameter.Add(new OracleParameter("P_NAMO_AFEC", OracleDbType.Double, request2.P_NAMO_AFEC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIVA", OracleDbType.Double, request2.P_NIVA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NAMOUNT", OracleDbType.Double, request2.P_NAMOUNT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NDE", OracleDbType.Double, request2.P_NDE, ParameterDirection.Input));
                /* RI DESA */

                //ED RENTA
                parameter.Add(new OracleParameter("P_MAX_PENSION", OracleDbType.Double, Convert.ToDouble(request2.P_NMONTO_PLANILLA), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TOT_ASEGURADOS", OracleDbType.Int32, Convert.ToInt32(request2.P_NTOTAL_TRABAJADORES), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_VALIDA_TRAMA", OracleDbType.Varchar2, request2.P_VALIDA_TRAMA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_PRIMA_TASA", OracleDbType.Varchar2, primaTasa, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TIPO_COTIZACION", OracleDbType.Varchar2, request2.P_TIPO_COT, ParameterDirection.Input));


                //OUTPUT
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, result.P_MESSAGE, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, result.P_COD_ERR, ParameterDirection.Output);
                OracleParameter P_FLAG_AUT = new OracleParameter("P_FLAG_AUT", OracleDbType.Int32, result.P_FLAG_AUT, ParameterDirection.Output);

                P_MESSAGE.Size = 9000;

                parameter.Add(P_MESSAGE);
                parameter.Add(P_COD_ERR);
                parameter.Add(P_FLAG_AUT);

                if (ELog.obtainConfig("desgravamenBranch") == request.P_NBRANCH.ToString())
                {
                    parameter.Add(new OracleParameter("P_TASA_SEGURO", OracleDbType.Decimal, Decimal.Parse(request.P_TASA_SEGURO), ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_TASA_AHORRO", OracleDbType.Decimal, Decimal.Parse(request.P_TASA_AHORRO), ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_CUMULO_MAX", OracleDbType.Decimal, Decimal.Parse(request.P_CUMULO_MAX), ParameterDirection.Input));
                }

                this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, connection, trx);
                result.P_MESSAGE = P_MESSAGE.Value.ToString();
                result.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                result.P_FLAG_AUT = Convert.ToInt32(P_FLAG_AUT.Value.ToString());
            }
            catch (Exception ex)
            {
                result.P_COD_ERR = 1;
                result.P_MESSAGE = ex.ToString();
                LogControl.save("InsertQuotationDetAP", ex.ToString(), "3");
            }

            return result;
        }

        public QuotationResponseVM InsertQuotationCom(QuotationCabBM request, QuotizacionComBM request2, DbConnection connection, DbTransaction trx)
        {
            var sPackageName = ProcedureName.pkg_Cotizacion + ".INS_COTIZACION_COMER";
            List<OracleParameter> parameter = new List<OracleParameter>();
            QuotationResponseVM result = new QuotationResponseVM();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, request2.P_NID_COTIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDTYPECHANNEL", OracleDbType.Int32, request2.P_NIDTYPECHANNEL, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NINTERMED", OracleDbType.Int32, request2.P_NINTERMED, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCLIENT_COMER", OracleDbType.Varchar2, request2.P_SCLIENT_COMER, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCOMISION_SAL", OracleDbType.Decimal, request2.P_NCOMISION_SAL, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCOMISION_SAL_PR", OracleDbType.Decimal, request2.P_NCOMISION_SAL_PR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCOMISION_PEN", OracleDbType.Decimal, request2.P_NCOMISION_PEN, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCOMISION_PEN_PR", OracleDbType.Decimal, request2.P_NCOMISION_PEN_PR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRINCIPAL", OracleDbType.Int32, request2.P_NPRINCIPAL, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, request.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NSHARE", OracleDbType.Int32, request2.P_NSHARE, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, result.P_MESSAGE, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Varchar2, result.P_COD_ERR, ParameterDirection.Output);

                P_MESSAGE.Size = 9000;
                P_COD_ERR.Size = 9000;

                parameter.Add(P_MESSAGE);
                parameter.Add(P_COD_ERR);

                parameter.Add(new OracleParameter("P_NLOCAL", OracleDbType.Int32, request2.P_NLOCAL, ParameterDirection.Input)); //Robert Pariasca
                //INI  BACKLOG 111 VL 
                parameter.Add(new OracleParameter("P_DEFFECDATE_TRANSAC", OracleDbType.Date, request.P_DSTARTDATE_ASE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.CodigoProceso, ParameterDirection.Input));
                //FIN  BACKLOG 111 VL 

                parameter.Add(new OracleParameter("P_NTIP_NCOMISSION", OracleDbType.Int32, request.P_NTIP_NCOMISSION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FLAG_COMISSION_PEN", OracleDbType.Int32, request.flagComisionPension, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FLAG_COMISSION_SAL", OracleDbType.Int32, request.flagComisionSalud, ParameterDirection.Input));

                this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, connection, trx);
                result.P_MESSAGE = P_MESSAGE.Value.ToString();
                result.P_COD_ERR = P_COD_ERR.Value.ToString() == "null" ? 0 : Convert.ToInt32(P_COD_ERR.Value.ToString());
            }
            catch (Exception ex)
            {
                result.P_MESSAGE = ex.ToString();
                result.P_COD_ERR = 1;
                LogControl.save("InsertQuotationCom", ex.ToString(), "3");
            }

            return result;
        }

        public QuotationResponseVM InsertQuotationComEndoso(QuotationCabBM request, QuotizacionComBM request2, DbConnection connection, DbTransaction trx)
        {
            var sPackageName = ProcedureName.pkg_Cotizacion + ".INS_COTIZACION_COMER_PD";
            List<OracleParameter> parameter = new List<OracleParameter>();
            QuotationResponseVM result = new QuotationResponseVM();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, request2.P_NID_COTIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDTYPECHANNEL", OracleDbType.Int32, request2.P_NIDTYPECHANNEL, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NINTERMED", OracleDbType.Int32, request2.P_NINTERMED, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCLIENT_COMER", OracleDbType.Varchar2, request2.P_SCLIENT_COMER, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCOMISION_SAL", OracleDbType.Decimal, request2.P_NCOMISION_SAL, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCOMISION_SAL_PR", OracleDbType.Decimal, request2.P_NCOMISION_SAL_PR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCOMISION_PEN", OracleDbType.Decimal, request2.P_NCOMISION_PEN, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCOMISION_PEN_PR", OracleDbType.Decimal, request2.P_NCOMISION_PEN_PR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRINCIPAL", OracleDbType.Int32, request2.P_NPRINCIPAL, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, request.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NSHARE", OracleDbType.Int32, request2.P_NSHARE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, request.P_NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SEDIT", OracleDbType.Varchar2, request2.P_SEDIT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Varchar2, request.P_NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Varchar2, request.NumeroPoliza, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, result.P_MESSAGE, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Varchar2, result.P_COD_ERR, ParameterDirection.Output);

                P_MESSAGE.Size = 9000;
                P_COD_ERR.Size = 9000;

                parameter.Add(P_MESSAGE);
                parameter.Add(P_COD_ERR);

                this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, connection, trx);
                result.P_MESSAGE = P_MESSAGE.Value.ToString();
                result.P_COD_ERR = P_COD_ERR.Value.ToString() == "null" ? 0 : Convert.ToInt32(P_COD_ERR.Value.ToString());
            }
            catch (Exception ex)
            {
                result.P_COD_ERR = 1;
                result.P_MESSAGE = ex.ToString();
                LogControl.save("InsertQuotationComEndoso", ex.ToString(), "3");
            }

            return result;
        }

        public QuotationResponseVM InsertQuotationCol(QuotationCabBM request, QuotizacionColBM request2, DbConnection connection, DbTransaction trx)
        {
            var sPackageName = ProcedureName.pkg_Cotizacion + ".INS_COTIZACION_COL_PD";
            List<OracleParameter> parameter = new List<OracleParameter>();
            QuotationResponseVM result = new QuotationResponseVM();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, request.P_NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, request.P_NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, request.P_SCLIENT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCLIENT_C", OracleDbType.Varchar2, request2.sclient, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSER", OracleDbType.Int32, request.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, request2.nid_cotizacion, ParameterDirection.Input));


                //OUTPUT
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, result.P_MESSAGE, ParameterDirection.Output);
                OracleParameter P_NCOD_ERR = new OracleParameter("P_NCOD_ERR", OracleDbType.Int32, result.P_COD_ERR, ParameterDirection.Output);

                P_MESSAGE.Size = 9000;

                parameter.Add(P_MESSAGE);
                parameter.Add(P_NCOD_ERR);

                this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, connection, trx);
                result.P_MESSAGE = P_MESSAGE.Value.ToString();
                result.P_COD_ERR = Convert.ToInt32(P_NCOD_ERR.Value.ToString());
            }
            catch (Exception ex)
            {
                result.P_COD_ERR = 1;
                result.P_MESSAGE = ex.ToString();
                LogControl.save("InsertQuotationCol", ex.ToString(), "3");
            }

            return result;
        }

        public QuotationResponseVM InsertHistorialComEndoso(QuotationCabBM request, DbConnection connection, DbTransaction trx)
        {
            var sPackageName = ProcedureName.pkg_GeneraTransac + ".SP_INS_POLICY_HIS";
            List<OracleParameter> parameter = new List<OracleParameter>();
            QuotationResponseVM result = new QuotationResponseVM();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_SCERTYPE", OracleDbType.Varchar2, "2", ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, request.P_NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, request.P_NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Decimal, request.NumeroPoliza, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCERTIF", OracleDbType.Decimal, 0, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCURRENCY", OracleDbType.Int32, request.P_NCURRENCY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DSTARTDATE", OracleDbType.Date, request.P_DSTARTDATE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, request.P_DEXPIRDAT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Varchar2, "8", ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, request.P_NUSERCODE, ParameterDirection.Input));


                //OUTPUT
                OracleParameter P_NMOVEMENT_HIS = new OracleParameter("P_NMOVEMENT_HIS ", OracleDbType.Decimal, result.P_NMOVEMENT_HIS, ParameterDirection.Output);
                OracleParameter P_NERROR = new OracleParameter("P_COD_ERR", OracleDbType.Decimal, result.P_NERROR, ParameterDirection.Output);
                OracleParameter P_SMENSAJE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, result.P_SMENSAJE, ParameterDirection.Output);


                P_SMENSAJE.Size = 9000;
                P_NERROR.Size = 9000;

                parameter.Add(P_NMOVEMENT_HIS);
                parameter.Add(P_NERROR);
                parameter.Add(P_SMENSAJE);

                this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, connection, trx);
                result.P_NMOVEMENT_HIS = P_NMOVEMENT_HIS.Value.ToString() == "null" ? 0 : Convert.ToInt32(P_NMOVEMENT_HIS.Value.ToString());
                result.P_NERROR = P_NERROR.Value.ToString() == "null" ? 0 : Convert.ToInt32(P_NERROR.Value.ToString());
                result.P_SMENSAJE = P_SMENSAJE.Value.ToString();
            }
            catch (Exception ex)
            {
                result.P_NERROR = 1;
                result.P_SMENSAJE = ex.ToString();
                LogControl.save("InsertHistorialComEndoso", ex.ToString(), "3");
            }

            return result;
        }

        public BrokerResponseVM SearchBroker(BrokerSearchBM data)
        {
            BrokerResponseVM result = new BrokerResponseVM();
            List<BrokerItemVM> contactTypeList = new List<BrokerItemVM>();

            try
            {
                List<OracleParameter> parameter = new List<OracleParameter>();
                parameter.Add(new OracleParameter("P_NTIPO_BUSQUEDA", OracleDbType.Int64, data.P_NTIPO_BUSQUEDA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTIPO_DOC", OracleDbType.Int64, data.P_NTIPO_DOC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NNUM_DOC", OracleDbType.Varchar2, data.P_NNUM_DOC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SNOMBRE", OracleDbType.Varchar2, data.P_SNOMBRE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SAP_PATERNO", OracleDbType.Varchar2, data.P_SAP_PATERNO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SAP_MATERNO", OracleDbType.Varchar2, data.P_SAP_MATERNO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SNOMBRE_LEGAL", OracleDbType.Varchar2, data.P_SNOMBRE_LEGAL, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_IS_AGENCY", OracleDbType.Varchar2, data.P_IS_AGENCY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCODSBS", OracleDbType.Varchar2, data.P_SCODSBS, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);

                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(ProcedureName.pkg_Cotizacion + ".REA_BROKERS", parameter);
                while (odr.Read())
                {
                    BrokerItemVM item = new BrokerItemVM();
                    item.COD_CANAL = odr["COD_CANAL"].ToString();
                    item.CODCANAL = odr["CODCANAL"].ToString();
                    item.RAZON_SOCIAL = odr["RAZON_SOCIAL"].ToString();
                    item.SCLIENT = odr["SCLIENT"].ToString();
                    item.NTYPECHANNEL = odr["NTYPECHANNEL"].ToString();
                    item.NTIPDOC = odr["NTIPDOC"].ToString();
                    item.NNUMDOC = odr["NNUMDOC"].ToString();
                    item.NCORREDOR = odr["NCORREDOR"].ToString();

                    item.STYPCLIENTDOC = odr["STYPCLIENTDOC"] == DBNull.Value ? "" : odr["STYPCLIENTDOC"].ToString();
                    item.SCLINUMDOCU = odr["SCLINUMDOCU"] == DBNull.Value ? "" : odr["SCLINUMDOCU"].ToString();
                    item.TYPE_INTERMEDIARY = odr["TYPE_INTERMEDIARY"] == DBNull.Value ? "" : odr["TYPE_INTERMEDIARY"].ToString();
                    item.CODE_SBS = odr["CODE_SBS"] == DBNull.Value ? "" : odr["CODE_SBS"].ToString();
                    item.NINTERTYP = odr["NINTERTYP"] == DBNull.Value ? "" : odr["NINTERTYP"].ToString();

                    item.SFIRSTNAME = odr["SFIRSTNAME"] == DBNull.Value ? "" : odr["SFIRSTNAME"].ToString();
                    item.SLASTNAME = odr["SLASTNAME"] == DBNull.Value ? "" : odr["SLASTNAME"].ToString();
                    item.SLASTNAME2 = odr["SLASTNAME2"] == DBNull.Value ? "" : odr["SLASTNAME2"].ToString();
                    item.SSTREET = odr["SSTREET"] == DBNull.Value ? "" : odr["SSTREET"].ToString();
                    item.SPHONE1 = odr["SPHONE1"] == DBNull.Value ? "" : odr["SPHONE1"].ToString();
                    item.SEMAILCLI = odr["SEMAILCLI"] == DBNull.Value ? "" : odr["SEMAILCLI"].ToString();
                    item.NINTERMED = odr["NINTERMED"] == DBNull.Value ? "" : odr["NINTERMED"].ToString();
                    item.SCLIENAME = odr["SCLIENAME"] == DBNull.Value ? "" : odr["SCLIENAME"].ToString();
                    item.NMUNICIPALITY = odr["NMUNICIPALITY"] == DBNull.Value ? "" : odr["NMUNICIPALITY"].ToString();

                    contactTypeList.Add(item);
                }
                ELog.CloseConnection(odr);

                result.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                result.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                result.listBroker = contactTypeList;
            }
            catch (Exception ex)
            {
                result.P_NCODE = 1;
                result.P_SMESSAGE = ex.ToString();
                LogControl.save("SearchBroker", ex.ToString(), "3");
            }

            return result;
        }

        public QuotationResponseVM ApproveQuotation(QuotationUpdateBM request)
        {
            var sPackageName = ProcedureName.pkg_Cotizacion + ".UPD_COTIZACION";
            List<OracleParameter> parameter = new List<OracleParameter>();
            QuotationResponseVM result = new QuotationResponseVM();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, request.P_NID_COTIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_ESTADO", OracleDbType.Int32, request.P_ESTADO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, request.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMOTIVO", OracleDbType.Int32, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOMMENT", OracleDbType.Varchar2, request.P_SCOMMENT, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, result.P_MESSAGE, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, result.P_COD_ERR, ParameterDirection.Output);

                P_MESSAGE.Size = 6000;

                parameter.Add(P_MESSAGE);
                parameter.Add(P_COD_ERR);

                this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                result.P_MESSAGE = P_MESSAGE.Value.ToString();
                result.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
            }
            catch (Exception ex)
            {
                result.P_COD_ERR = 1;
                result.P_SMESSAGE = ex.ToString();
                LogControl.save("ApproveQuotation", ex.ToString(), "3");
            }

            return result;
        }

        public string equivalentMunicipality(Int64 municipality)
        {
            var sPackageName = ProcedureName.pkg_Cotizacion + ".OBT_EQUIMUNICIPALITYINEI";
            string result = "";
            List<OracleParameter> parameters = new List<OracleParameter>();
            try
            {
                parameters.Add(new OracleParameter("P_NMUNICIPALITY", OracleDbType.Int64, municipality, ParameterDirection.Input));

                OracleParameter P_COD_UBI_CLI = new OracleParameter("P_COD_UBI_CLI", OracleDbType.Varchar2, result, ParameterDirection.Output);

                P_COD_UBI_CLI.Size = 200;

                parameters.Add(P_COD_UBI_CLI);
                this.ExecuteByStoredProcedureVT(sPackageName, parameters);
                result = P_COD_UBI_CLI.Value.ToString();
            }
            catch (Exception ex)
            {
                result = null;
                LogControl.save("equivalentMunicipality", ex.ToString(), "3");
            }
            return result;
        }

        public string GetIgv(QuotationIGVBM data)
        {
            var sPackageName = ProcedureName.pkg_Cotizacion + ".REA_RECARGO";

            string result = "";
            List<OracleParameter> parameters = new List<OracleParameter>();
            try
            {
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int64, data.P_NBRANCH, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int64, data.P_NPRODUCT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_TIPO_REC", OracleDbType.Varchar2, data.P_TIPO_REC, ParameterDirection.Input));

                OracleParameter P_RECARGO = new OracleParameter("P_RECARGO", OracleDbType.Varchar2, result, ParameterDirection.Output);

                P_RECARGO.Size = 200;

                parameters.Add(P_RECARGO);
                this.ExecuteByStoredProcedureVT(sPackageName, parameters);
                result = P_RECARGO.Value.ToString();
            }
            catch (Exception ex)
            {
                result = null;
                LogControl.save("GetIgv", ex.ToString(), "3");
            }
            return result;
        }

        public async Task<TariffVM> getTariffProtecta(ProtectaBM data)
        {
            // var response = new TariffVM();
            var response = new TariffVM();

            try
            {
                var json = await invocarServicio(JsonConvert.SerializeObject(data));
                //    response = JsonConvert.DeserializeObject<TariffVM>(json, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                LogControl.save("getTariffProtecta - Ini", JsonConvert.SerializeObject(data), "2");

                response = JsonConvert.DeserializeObject<TariffVM>(json, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                LogControl.save("getTariffProtecta - Fin", json.ToString(), "2");

            }
            catch (Exception ex)
            {
                //response.codError = ELog.obtainConfig("codigo_NO");
                //response.mensaje = ex.Message;
                LogControl.save("getTariffProtecta", ex.ToString(), "3");
            }

            //var responseWS = response.Count > 0 ? response[0] : new TariffVM();
            var responseWS = response != null ? response : new TariffVM();
            return await Task.FromResult(responseWS);
        }

        public async Task<string> invocarServicio_EPS(int cotizacion, string json) //AVS - PRY INTERCONEXION SABSA 20/07/2023
        {
            var sQuoteResult = string.Empty;
            try
            {
                var mToken = "";
                var credentials = new
                {
                    usuario = ELog.obtainConfig("EPSUser"),
                    clave = ELog.obtainConfig("EPSoPwd")
                };

                var jsonCredentials = JsonConvert.SerializeObject(credentials);
                var content = new StringContent(jsonCredentials, Encoding.UTF8, "application/json");

                using (HttpClient client = new HttpClient())
                {
                    //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12; --ACTIVAR EN CASO SER NECESARIO
                    var responseToken = await client.PostAsync(ELog.obtainConfig("urlTokenEPS_SCTR"), content);
                    mToken = await responseToken.Content.ReadAsStringAsync();

                    if (string.IsNullOrEmpty(mToken))
                    {
                        InsertLog(cotizacion, "02 - ERROR EN TOKEN EPS - COTIZACION", ELog.obtainConfig("urlTokenEPS_SCTR"), responseToken + Environment.NewLine + Environment.NewLine + "No se genero Token del servicio EPS", null);
                        throw new Exception(ELog.obtainConfig("errorInvocarServicio_EPS"));
                    }

                    var QuotEPSToken = JsonConvert.DeserializeObject<AuthEPSResult>(mToken);

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", QuotEPSToken.token);
                    var response = await client.PostAsync(ELog.obtainConfig("urlCotizacionEPS_SCTR"), new StringContent(json, Encoding.UTF8, "application/json"));
                    sQuoteResult = await response.Content.ReadAsStringAsync();
                    var responseObject = JsonConvert.DeserializeObject<dynamic>(sQuoteResult);
                    bool isSuccess = responseObject.success;

                    if (isSuccess)
                    {
                        InsertLog(cotizacion, "02 - RESPUESTAS EPS - COTIZACION", ELog.obtainConfig("urlCotizacionEPS_SCTR"), sQuoteResult, null);
                    }
                    else
                    {
                        string errorMessage = responseObject.message;
                        throw new Exception(errorMessage);
                    }
                }

            }
            catch (Exception ex)
            {
                LogControl.save("invocarServicio_EPS", ex.ToString(), "3");
                new QuotationDA().InsertLog(cotizacion, "02 - ERROR SERVICIO EPS - COTIZACION", ELog.obtainConfig("urlCotizacionEPS_SCTR"), json + Environment.NewLine + Environment.NewLine + ex.ToString(), null);
            }

            return await Task.FromResult(sQuoteResult);
        }

        /**  Obtener Tarifas **/
        public async Task<string> invocarServicio(string json)
        {
            var sTariffResult = string.Empty;

            try
            {
                var mToken = "";
                using (HttpClient client = new HttpClient())
                {
                    var content = new FormUrlEncodedContent(new[]
                    {
                       new KeyValuePair<string, string>("client_id", "soat-api"),
                       new KeyValuePair<string, string>("grant_type", "password"),
                       new KeyValuePair<string, string>("username", ELog.obtainConfig("tarifarioUser")),
                       new KeyValuePair<string, string>("password", ELog.obtainConfig("tarifarioPwd")),
                    });
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    var response = await client.PostAsync(ELog.obtainConfig("tarifarioToken"), content);
                    mToken = await response.Content.ReadAsStringAsync();
                }

                if (string.IsNullOrEmpty(mToken))
                {
                    throw new Exception(ELog.obtainConfig("errorTarifario"));
                }
                else
                {
                    var tariffToken = JsonConvert.DeserializeObject<AuthTariffResult>(mToken);

                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tariffToken.access_token);
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                        var response = await client.PostAsync(ELog.obtainConfig("tarifario"),
                                                              new StringContent(json, Encoding.UTF8, "application/json"));

                        sTariffResult = await response.Content.ReadAsStringAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                LogControl.save("invocarServicio", ex.ToString(), "3");
            }

            return await Task.FromResult(sTariffResult);
        }

        public FechasRenoVM ObtenerFechasRenovacion(string nrocotizacion, string typetransac, string codproducto)
        {
            var sPackageName = ProcedureName.pkg_Cotizacion + ".OBTENER_FECHAS_RENOV";
            List<OracleParameter> parameter = new List<OracleParameter>();
            FechasRenoVM model = new FechasRenoVM();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, nrocotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, codproducto, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Int32, typetransac, ParameterDirection.Input));

                OracleParameter P_SKEY = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);

                parameter.Add(P_SKEY);
                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter);

                while (odr.Read())
                {
                    model.fechaini_pol = String.Format("{0:MM/dd/yyyy}", odr["FECHAINI_POL"]);
                    model.fechafin_pol = String.Format("{0:MM/dd/yyyy}", odr["FECHAFIN_POL"]);
                    model.fechaini_aseg = String.Format("{0:MM/dd/yyyy}", odr["FECHAINI_ASEG"]);
                    model.fechafin_aseg = String.Format("{0:MM/dd/yyyy}", odr["FECHAFIN_ASEG"]);
                    model.titulo = odr["MENSAJE"].ToString();
                    model.indicador_mov = Convert.ToBoolean(odr["INDICADOR"].ToString());
                }

                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                LogControl.save("ObtenerFechasRenovacion", ex.ToString(), "3");
            }

            return model;
        }


        public int ValidarDataTrama(string codProc)
        {
            SalidaErrorBaseVM response = new SalidaErrorBaseVM();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.pkg_ValidadorGenPD + ".SP_VALIDA_EXISTE_TRAMA";
            int contador = 0;

            try
            {
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, codProc, ParameterDirection.Input));
                OracleParameter P_COUNT = new OracleParameter("P_NCONTADOR", OracleDbType.Int32, ParameterDirection.Output);
                P_COUNT.Size = 9000;

                parameter.Add(P_COUNT);
                this.ExecuteByStoredProcedureVT(storeprocedure, parameter);
                contador = Convert.ToInt32(P_COUNT.Value.ToString());
            }
            catch (Exception ex)
            {
                contador = 0;
                LogControl.save("ValidarDataTrama", ex.ToString(), "3");
            }

            return contador;
        }

        public SalidaTramaBaseVM getDetail(validaTramaVM objValida, ref SalidaTramaBaseVM obj)
        {
            //SalidaTramaBaseVM response = new SalidaTramaBaseVM();

            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.pkg_ValidadorGenPD + ".SP_DET_COTIZADOR";

            OracleDataReader dr = null;
            try
            {
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, objValida.codProceso, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int64, objValida.codProducto, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int64, objValida.nroCotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int64, objValida.nroPoliza, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FEC_ANULACION", OracleDbType.Date, Convert.ToDateTime(objValida.fechaEfecto), ParameterDirection.Input)); //RQ EXCEDENTE EHH
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, objValida.codUsuario, ParameterDirection.Input)); //RQ EXCEDENTE EHH

                string[] arrayCursor = { "C_CATEGORIA", "C_DET_PLAN" };
                OracleParameter C_CATEGORIA = new OracleParameter(arrayCursor[0], OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter C_DET_PLAN = new OracleParameter(arrayCursor[1], OracleDbType.RefCursor, ParameterDirection.Output);

                parameter.Add(C_CATEGORIA);
                parameter.Add(C_DET_PLAN);

                dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storeprocedure, arrayCursor, parameter);

                if (dr.HasRows)
                {
                    string SCATE = "";

                    while (dr.Read())
                    {
                        CategoryQuotationVM item = new CategoryQuotationVM();
                        item.SCATEGORIA_view = (dr["SCATEGORIA"].ToString() == SCATE) ? "" : dr["SCATEGORIA"].ToString();
                        item.SCATEGORIA = dr["SCATEGORIA"] == DBNull.Value ? "" : dr["SCATEGORIA"].ToString();
                        item.SRANGO_EDAD = dr["SEDADES"] == DBNull.Value ? "" : dr["SEDADES"].ToString();
                        item.NCOUNT = dr["NCOUNT"] == DBNull.Value ? 0 : int.Parse(dr["NCOUNT"].ToString());
                        item.NTOTAL_PLANILLA = dr["NTOTAL_PLANILLA"] == DBNull.Value ? 0.0 : double.Parse(dr["NTOTAL_PLANILLA"].ToString());
                        item.NTASA = dr["NTASA"] == DBNull.Value ? 0.0 : double.Parse(dr["NTASA"].ToString());
                        SCATE = dr["SCATEGORIA"] == DBNull.Value ? "" : dr["SCATEGORIA"].ToString();
                        obj.categoryList.Add(item);
                    }

                    dr.NextResult();

                    while (dr.Read())
                    {
                        DetailPlanQuotationVM item = new DetailPlanQuotationVM();
                        item.DET_PLAN = dr["DET_PLAN"] == DBNull.Value ? "" : dr["DET_PLAN"].ToString();
                        obj.detailPlanList.Add(item);
                    }
                }
            }
            catch (OracleException ex)
            {
                LogControl.save("getDetail", ex.ToString(), "3");
            }
            finally
            {
                ELog.CloseConnection(dr);
            }

            return obj;
        }

        public SalidaTramaBaseVM getDetailAmount(validaTramaVM objValida, ref SalidaTramaBaseVM obj)
        {
            //SalidaTramaBaseVM response = new SalidaTramaBaseVM();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.pkg_ValidadorGenPD + ".SP_DET_AMOUNT_COTIZADOR";

            try
            {
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, objValida.codProceso, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Varchar2, objValida.type_mov, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int64, objValida.nroCotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TASA_OB", OracleDbType.Double, 0, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TASA_EM", OracleDbType.Double, 0, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int64, objValida.nroPoliza, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int64, objValida.codProducto, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, Convert.ToDateTime(objValida.fechaExpiracion), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FEC_ANULACION", OracleDbType.Date, Convert.ToDateTime(objValida.fechaEfecto), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NFLAG_PR_DEV", OracleDbType.Int32, objValida.excludeType, ParameterDirection.Input));

                string[] arrayCursor = { "C_AMOUNT_PRIMA", "C_AMOUNT_DET_TOTAL" };
                OracleParameter C_AMOUNT_PRIMA = new OracleParameter(arrayCursor[0], OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter C_AMOUNT_DET_TOTAL = new OracleParameter(arrayCursor[1], OracleDbType.RefCursor, ParameterDirection.Output);

                parameter.Add(C_AMOUNT_PRIMA);
                parameter.Add(C_AMOUNT_DET_TOTAL);

                OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storeprocedure, arrayCursor, parameter);

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        AmountPremiumQuotationVM item = new AmountPremiumQuotationVM();
                        item.SCATEGORIA = dr["SCATEGORIA"] == DBNull.Value ? "" : dr["SCATEGORIA"].ToString();
                        item.SRANGO_EDAD = dr["SEDADES"] == DBNull.Value ? "" : dr["SEDADES"].ToString();
                        item.NTASA = dr["NTASA"] == DBNull.Value ? 0.0 : double.Parse(dr["NTASA"].ToString());
                        item.NPREMIUMN_TOT = dr["NPREMIUMN_TOT"] == DBNull.Value ? 0.0 : double.Parse(dr["NPREMIUMN_TOT"].ToString());
                        obj.amountPremiumList.Add(item);
                    }
                    dr.NextResult();
                    while (dr.Read())
                    {
                        AmountDetailTotalQuotationVM item = new AmountDetailTotalQuotationVM();
                        item.NORDER = dr["NORDER"] == DBNull.Value ? "0" : dr["NORDER"].ToString();
                        item.SDESCRIPCION = dr["SDESCRIPCION"] == DBNull.Value ? "" : dr["SDESCRIPCION"].ToString();
                        item.NAMOUNT_MEN = dr["NAMOUNT_MEN"] == DBNull.Value ? 0.0 : double.Parse(dr["NAMOUNT_MEN"].ToString());
                        item.NAMOUNT_TOT = dr["NAMOUNT_TOT"] == DBNull.Value ? 0.0 : double.Parse(dr["NAMOUNT_TOT"].ToString());
                        obj.amountDetailTotalList.Add(item);
                    }
                }

                ELog.CloseConnection(dr);
            }
            catch (Exception ex)
            {
                LogControl.save("getDetailAmount", ex.ToString(), "3");
            }

            return obj;
        }

        public List<PlanVM> getPlans()
        {
            var sPackageJobs = ProcedureName.pkg_Cotizacion + ".REA_LIST_PLAN_VL";
            var response = new List<PlanVM>();
            var parameter = new List<OracleParameter>();

            try
            {
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageJobs, "C_TABLE", parameter);

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        PlanVM item = new PlanVM
                        {
                            //COD_PLAN = dr["COD_PLAN"] == DBNull.Value ? 0 : int.Parse(dr["COD_PLAN"].ToString()),
                            //DES_PLAN = dr["DES_PLAN"] == DBNull.Value ? String.Empty : dr["DES_PLAN"].ToString()
                        };
                        response.Add(item);
                    }

                }
                ELog.CloseConnection(dr);

            }
            catch (Exception ex)
            {
                LogControl.save("getPlans", ex.ToString(), "3");
            }
            return response;
        }

        //marcos silverio
        public GenericResponseVM UpdateStatusQuotationVidaLey(UpdateStatusQuotationBM request, DataTable dtCover)
        {
            var response = new GenericResponseVM();
            response.ErrorMessageList = new List<string>();

            SalidaTramaBaseVM resultBulkInsert = new SalidaTramaBaseVM();
            if (dtCover != null)
            {
                resultBulkInsert = this.SaveUsingOracleBulkCopy(dtCover, "TBL_PD_COTIZACION_COVER");

                if (resultBulkInsert.P_COD_ERR == "1")
                {
                    response.ErrorCode = Convert.ToInt32(resultBulkInsert.P_COD_ERR);
                    response.ErrorMessageList.Add(resultBulkInsert.P_MESSAGE);

                    return response;
                }

                LogControl.save("UpdateStatusQuotationVidaLey", "Paso 2: Se registraron coberturas en TBL_PD_COTIZACION_COVER.", "1");
            }

            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
            {
                cn.Open();

                using (var tran = cn.BeginTransaction())
                {
                    try
                    {
                        response = InsCotizacionHis(request, cn, tran);



                        if (response.ErrorCode == 0)
                        {

                            // GCAA 16042024
                            var resu = false;
                            resu = this.UpdateEstadoCabecera(cn, tran, request.NroCotizacion.ToString(), request, ref response);

                            // 1: prima
                            // 2: retroactividad
                            // 3: prima y retroactividad
                            // 4: prima en poliza matriz
                            // 5: retroactividad + prima en poliza matriz
                            if (request.TipoProceso != "1" && request.TipoProceso != "4")
                            {
                                //Retroactividad
                                var valid_result = this.UpdateStatusQuotation(cn, tran, request);
                                response.Transaction = valid_result.Transaction;
                                response.GenericFlag = valid_result.GenericFlag;
                                response.ErrorCode = valid_result.ErrorCode;
                                response.ErrorMessageList.AddRange(valid_result.ErrorMessageList);
                            }

                            if (request.TipoProceso != "2" && request.TipoProceso != "4" && request.TipoProceso != "5")
                            {
                                //Cotizacion
                                if (dtCover != null && response.ErrorCode == 0)
                                {
                                    var valid_result = this.ValidResponseUpdateStatusQuotation(cn, tran, request.NroCotizacion.ToString());
                                    response.ErrorCode = valid_result.ErrorCode;
                                    response.ErrorMessageList.AddRange(valid_result.ErrorMessageList);

                                    LogControl.save("UpdateStatusQuotationVidaLey", "Paso 4: Se ejecutó " + ProcedureName.pkg_Cotizacion + ".VAL_COTIZACION_COVER", "1");
                                }

                                if (response.ErrorCode == 0 && request.Estado != ELog.obtainConfig("RejectedStatusQuotationVL").ToString())
                                {
                                    //var success = this.InsertDetailPremiunEndoso(cn, tran, request.NroCotizacion.ToString(), request, ref response);

                                    var success = false;

                                    // GCAA POR RANGO DE EDAD
                                    if (request.opcion == "1" || request.opcion == "3")
                                    {
                                        // DEJA EN NULO LAS TASAS QUE TIENE ACTUALMENTE EN LA BD
                                        success = this.DeleteOldRangoEdad(cn, tran, request.NroCotizacion.ToString(), request, ref response);

                                        if (request.opcion == "1")
                                        {
                                            if (request.tasasPlanillasPrimasRangoEdadExcesoTopeRemuneraciones.Count == 0)
                                            {
                                            // inserta los rangos de edad
                                            success = this.InsertNewRangoEdad(cn, tran, request.NroCotizacion.ToString(), request, ref response);
                                            }

                                            if (request.tasasPlanillasPrimasRangoEdadExcesoTopeRemuneraciones.Count > 0)
                                            {
                                            // procesa informacion para registrar en el detalle de la cotizacion
                                            success = this.InsertNewRangoEdadExcedente(cn, tran, request.NroCotizacion.ToString(), request, ref response);
                                        }
                                        }
                                        else
                                        {
                                            // inserta los rangos de edad
                                            success = this.InsertNewRangoEdadxCategoria(cn, tran, request.NroCotizacion.ToString(), request, ref response);
                                            //EXCEDENTE
                                            //success = this.InsertNewRangoEdadxCategoriaExcedente(cn, tran, request.NroCotizacion.ToString(), request, ref response);
                                            if (request.tasasPlanillasPrimasCodigoCategoriaExcesoTopeRemuneraciones.Count > 0)
                                            {
                                                success = this.InsertNewRangoEdadxCategoriaExcedente(cn, tran, request.NroCotizacion.ToString(), request, ref response);
                                            }
                                        }

                                        // PROCESA DETALLE DE LA COTIZACION
                                        success = this.InsertDetalleCotizador(cn, tran, request.NroCotizacion.ToString(), request, ref response);

                                        //if (request.opcion == "3")
                                        //{
                                        //    // procesa informacion para registrar en el detalle de la cotizacion despues de registrar los datos de los NO RMA
                                        //    success = this.InsertNewRangoEdadxCategoriaExcedente(cn, tran, request.NroCotizacion.ToString(), request, ref response);
                                        //}
                                    }

                                    // GCAA POR CATEGORIA 
                                    if (request.opcion == "2")// || request.opcion == "4")
                                    {
                                        // DEJA EN NULO LAS TASAS QUE TIENE ACTUALMENTE EN LA BD
                                        success = this.DeleteOldRangoEdad(cn, tran, request.NroCotizacion.ToString(), request, ref response);

                                        // inserta los rangos de edad
                                        success = this.InsertNewxCategoria(cn, tran, request.NroCotizacion.ToString(), request, ref response);

                                        success = this.InsertNewxCategoriaPonderada(cn, tran, request.NroCotizacion.ToString(), request, ref response);

                                        // procesa informacion para registrar en el detalle de la cotizacion
                                        success = this.InsertNewxCategoriaExcedente(cn, tran, request.NroCotizacion.ToString(), request, ref response);

                                        // PROCESA DETALLE DE LA COTIZACION
                                        success = this.InsertDetalleCotizador_CAT(cn, tran, request.NroCotizacion.ToString(), request, ref response);


                                    }





                                    // GCAA POR CATEGORIA 
                                    if (request.opcion == "4")
                                    {
                                        success = this.DeleteOldRangoEdad(cn, tran, request.NroCotizacion.ToString(), request, ref response);
                                        success = this.InsertNewxCategoriaMatriz(cn, tran, request.NroCotizacion.ToString(), request, ref response);
                                        success = this.InsertDetalleCotizador_CAT_MATRIZ(cn, tran, request.NroCotizacion.ToString(), request, ref response);
                                        //success = this.InsertDetailPremiunEndoso(cn, tran, request.NroCotizacion.ToString(), request, ref response);
                                    }


                                    //if (response.ErrorCode == 0 && success)
                                    //{
                                    //    success = this.InsertDetailPremiun(cn, tran, request.NroCotizacion.ToString(), request, ref response);
                                    //    LogControl.save("UpdateStatusQuotationVidaLey", "Paso 5: Se ejecutó " + ProcedureName.pkg_Cotizacion + ".UPD_COTIZACION_DET", "1");
                                    //}
                                    if (response.ErrorCode == 0 && success)
                                    {
                                        //if (request.opcion == "4")
                                        //{
                                        //    success = this.InsertDetailPremiun(cn, tran, request.NroCotizacion.ToString(), request, ref response);
                                        //    LogControl.save("UpdateStatusQuotationVidaLey", "Paso 5: Se ejecutó " + ProcedureName.pkg_Cotizacion + ".UPD_COTIZACION_DET", "1");

                                        //}

                                        if (request.opcion == "3")
                                        {
                                            success = this.InsertDetailPremiunRangoEdadxCategoria(cn, tran, request.NroCotizacion.ToString(), request, ref response);
                                        }
                                        LogControl.save("UpdateStatusQuotationVidaLey", "Paso 5: Se ejecutó " + ProcedureName.pkg_Cotizacion + ".UPD_COTIZACION_DET", "1");

                                    }

                                    response.ErrorCode = success ? response.ErrorCode : 1;
                                }

                            }

                            if (request.Estado != ELog.obtainConfig("RejectedStatusQuotationVL").ToString())
                            {
                                var brokers = new PolicyDA().GetPolizaEmitComer(Convert.ToInt32(request.NroCotizacion));
                                var validateBrokerData = new validateBrokerVL
                                {
                                    P_SCLIENT = brokers[0].SCLIENT
                                };

                                var flagBrokers = new QuotationDA().validateBroker(validateBrokerData);


                                if (brokers[0].CANAL != "2015000002" && flagBrokers.P_FLAG_BROKER != 1) //DIRECTO - AVS - COMISIONES
                                {
                                    #region creacion de objeto broker
                                    var broker = new Entities.QuotationModel.BindingModel.BrokerBM[]
                                    {
                                        new Entities.QuotationModel.BindingModel.BrokerBM()
                                        {
                                            Id = brokers[0].CANAL,
                                            ProductList = new BrokerProductBM[]
                                            {
                                                new BrokerProductBM()
                                                {
                                                    Product = "1",
                                                    AuthorizedCommission = request.Comision
                                                }
                                            }
                                        }
                                    };
                                    #endregion

                                    var data = new StatusChangeBM()
                                    {
                                        QuotationNumber = request.NroCotizacion,
                                        Status = Convert.ToInt32(request.Estado),
                                        User = request.UsuarioAprobador,
                                        brokerList = broker, //request.Comision > 0 ? broker : new Entities.QuotationModel.BindingModel.BrokerBM[] { },
                                    };

                                    response = updateBroker(tran, cn, data);

                                }
                            }

                            if (request.TipoProceso != "1" && request.TipoProceso != "2" && request.TipoProceso != "3")
                            {
                                //Cotizacion PME
                                if (dtCover != null && response.ErrorCode == 0)
                                {
                                    var valid_result = this.ValidResponseUpdateStatusQuotation(cn, tran, request.NroCotizacion.ToString());
                                    //response.ErrorCode = valid_result.ErrorCode;
                                    //response.ErrorMessageList.AddRange(valid_result.ErrorMessageList);

                                    LogControl.save("UpdateStatusQuotationVidaLey", "Paso 4: Se ejecutó " + ProcedureName.pkg_Cotizacion + ".VAL_COTIZACION_COVER", "1");
                                }

                                if (response.ErrorCode == 0 && request.Estado != ELog.obtainConfig("RejectedStatusQuotationVL").ToString())
                                {
                                    var success = false;

                                    // GCAA POR RANGO DE EDAD
                                    if (request.opcion == "1" || request.opcion == "3")
                                    {
                                        success = this.DeleteOldRangoEdad(cn, tran, request.NroCotizacion.ToString(), request, ref response);
                                        if (request.opcion == "1")
                                        {
                                            if (request.tasasPlanillasPrimasRangoEdadExcesoTopeRemuneraciones.Count == 0)
                                            {
                                                // inserta los rangos de edad
                                            success = this.InsertNewRangoEdad(cn, tran, request.NroCotizacion.ToString(), request, ref response);
                                            }

                                            if (request.tasasPlanillasPrimasRangoEdadExcesoTopeRemuneraciones.Count > 0)
                                            {
                                                // procesa informacion para registrar en el detalle de la cotizacion
                                            success = this.InsertNewRangoEdadExcedente(cn, tran, request.NroCotizacion.ToString(), request, ref response);
                                        }
                                        }
                                        else
                                        {
                                            success = this.InsertNewRangoEdadxCategoria(cn, tran, request.NroCotizacion.ToString(), request, ref response);
                                            //success = this.InsertNewRangoEdadxCategoriaExcedente(cn, tran, request.NroCotizacion.ToString(), request, ref response);

                                            if (request.tasasPlanillasPrimasCodigoCategoriaExcesoTopeRemuneraciones.Count > 0)
                                            {
                                                success = this.InsertNewRangoEdadxCategoriaExcedente(cn, tran, request.NroCotizacion.ToString(), request, ref response);
                                            }
                                        }
                                        success = this.InsertDetalleCotizador(cn, tran, request.NroCotizacion.ToString(), request, ref response);
                                    }

                                    // GCAA POR CATEGORIA 
                                    if (request.opcion == "2")// || request.opcion == "4")
                                    {
                                        success = this.DeleteOldRangoEdad(cn, tran, request.NroCotizacion.ToString(), request, ref response);
                                        success = this.InsertNewxCategoria(cn, tran, request.NroCotizacion.ToString(), request, ref response);
                                        success = this.InsertNewxCategoriaExcedente(cn, tran, request.NroCotizacion.ToString(), request, ref response);
                                        success = this.InsertDetalleCotizador_CAT(cn, tran, request.NroCotizacion.ToString(), request, ref response);
                                    }




                                    // GCAA POR CATEGORIA 
                                    if (request.opcion == "4")
                                    {
                                        success = this.DeleteOldRangoEdad(cn, tran, request.NroCotizacion.ToString(), request, ref response);
                                        success = this.InsertNewxCategoriaMatriz(cn, tran, request.NroCotizacion.ToString(), request, ref response);
                                        success = this.InsertDetalleCotizador_CAT_MATRIZ(cn, tran, request.NroCotizacion.ToString(), request, ref response);
                                        //success = this.InsertDetailPremiunEndoso(cn, tran, request.NroCotizacion.ToString(), request, ref response);
                                    }




                                    if (response.ErrorCode == 0 && success)
                                    {
                                        if (request.opcion == "4")
                                        {
                                            success = this.InsertDetailPremiun(cn, tran, request.NroCotizacion.ToString(), request, ref response);
                                        }

                                        if (request.opcion == "3")
                                        {
                                            success = this.InsertDetailPremiunRangoEdadxCategoria(cn, tran, request.NroCotizacion.ToString(), request, ref response);
                                        }
                                        LogControl.save("UpdateStatusQuotationVidaLey", "Paso 5: Se ejecutó " + ProcedureName.pkg_Cotizacion + ".UPD_COTIZACION_DET", "1");

                                    }

                                    response.ErrorCode = success ? response.ErrorCode : 1;
                                }

                            }

                            //if (response.ErrorCode == 0 && request.Estado != ELog.obtainConfig("RejectedStatusQuotationVL").ToString() && request.TipoProceso != "4" && request.TipoProceso != "5")
                            //{
                            //    response = this.PD_REM_EXC_AUT(cn, tran, request);
                            //}




                        }
                        else
                        {
                            response.ErrorMessageList.Add(response.Message);
                        }
                        // tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        response.ErrorMessageList.Add(ex.ToString());
                        response.ErrorCode = 3;
                        LogControl.save("UpdateStatusQuotationVidaLey", ex.ToString(), "3");
                    }
                    finally
                    {
                        if (response.ErrorCode == 0)
                        {
                            tran.Commit();
                            LogControl.save("UpdateStatusQuotationVidaLey", "Paso 6: trx.Commit()", "1");
                            if (request.Estado == ELog.obtainConfig("RejectedStatusQuotationVL").ToString())
                            {
                                RechazarTramiteProcess(request);
                            }
                        }
                        else
                        {
                            tran.Rollback();
                        }

                        if (cn.State == ConnectionState.Open) cn.Close();
                    }
                }
            }

            return response;
        }

        public GenericResponseVM InsCotizacionHis(UpdateStatusQuotationBM data, DbConnection connection, DbTransaction trx)
        {
            var response = new GenericResponseVM();

            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.pkg_Cotizacion + ".INS_COTIZA_HIS";

            try
            {
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.NroCotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_ESTADO", OracleDbType.Varchar2, data.Estado, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMOTIVO", OracleDbType.Int32, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, Convert.ToInt32(data.UsuarioAprobador), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOMMENT", OracleDbType.Varchar2, data.Comentario, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SRUTA", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NGLOSS_COT", OracleDbType.Int32, null, ParameterDirection.Input)); // Mejora SCTR
                parameter.Add(new OracleParameter("P_SGLOSS_COT", OracleDbType.Varchar2, null, ParameterDirection.Input)); // Mejora SCTR

                OracleParameter P_COD_ESTADO = new OracleParameter("P_COD_ESTADO", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_MENSAJE = new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, ParameterDirection.Output);

                P_MENSAJE.Size = 4000;

                parameter.Add(P_COD_ESTADO);
                parameter.Add(P_MENSAJE);

                parameter.Add(new OracleParameter("P_DDATEAPROB", OracleDbType.Date, data.FechaAprobacion, ParameterDirection.Input)); // Mejora VIDA LEY
                parameter.Add(new OracleParameter("L_NTOPE_REM_EXC_AUT", OracleDbType.Decimal, data.RemMaxExcedente, ParameterDirection.Input));
                parameter.Add(new OracleParameter("L_NTOPE_AGE_EXC_AUT", OracleDbType.Decimal, data.EdadMaxExcedente, ParameterDirection.Input));

                this.ExecuteByStoredProcedureVT_TRX(storeprocedure, parameter, connection, trx);

                response.ErrorCode = Convert.ToInt32(P_COD_ESTADO.Value.ToString());
                response.Message = P_MENSAJE.Value.ToString();

                LogControl.save("InsCotizacionHis", "Paso 3: Se ejecutó " + ProcedureName.pkg_Cotizacion + ".INS_COTIZA_HIS", "2");
            }
            catch (Exception ex)
            {
                response.ErrorCode = 1;
                response.Message = ex.ToString();
                LogControl.save("InsCotizacionHis", JsonConvert.SerializeObject(data), "2");
                LogControl.save("InsCotizacionHis", ex.ToString(), "3");
            }

            return response;




            //try
            //{
            //    DbCommand command = connection.CreateCommand();
            //    command.CommandType = CommandType.StoredProcedure;
            //    command.CommandText = ProcedureName.pkg_Cotizacion + ".INS_COTIZA_HIS";
            //    command.Transaction = trx;

            //    command.Parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.NroCotizacion, ParameterDirection.Input));
            //    command.Parameters.Add(new OracleParameter("P_ESTADO", OracleDbType.Varchar2, data.Estado, ParameterDirection.Input));
            //    command.Parameters.Add(new OracleParameter("P_NMOTIVO", OracleDbType.Int32, null, ParameterDirection.Input));
            //    command.Parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, Convert.ToInt32(data.UsuarioAprobador), ParameterDirection.Input));
            //    command.Parameters.Add(new OracleParameter("P_SCOMMENT", OracleDbType.Varchar2, data.Comentario, ParameterDirection.Input));
            //    command.Parameters.Add(new OracleParameter("P_SRUTA", OracleDbType.Varchar2, null, ParameterDirection.Input));
            //    command.Parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, null, ParameterDirection.Input));
            //    command.Parameters.Add(new OracleParameter("P_NGLOSS_COT", OracleDbType.Int32, null, ParameterDirection.Input)); // Mejora SCTR
            //    command.Parameters.Add(new OracleParameter("P_SGLOSS_COT", OracleDbType.Varchar2, null, ParameterDirection.Input)); // Mejora SCTR

            //    command.Parameters.Add(new OracleParameter("P_COD_ESTADO", OracleDbType.Int32, 4000, null, ParameterDirection.Output));
            //    command.Parameters.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output));

            //    command.Parameters.Add(new OracleParameter("P_DDATEAPROB", OracleDbType.Date, data.FechaAprobacion, ParameterDirection.Input)); // Mejora VIDA LEY

            //    command.Parameters.Add(new OracleParameter("L_NTOPE_REM_EXC_AUT", OracleDbType.Decimal, data.RemMaxExcedente, ParameterDirection.Input));
            //    command.Parameters.Add(new OracleParameter("L_NTOPE_AGE_EXC_AUT", OracleDbType.Decimal, data.EdadMaxExcedente, ParameterDirection.Input));

            //    command.ExecuteNonQuery();

            //    response.ErrorCode = Convert.ToInt32(command.Parameters["P_COD_ESTADO"].Value.ToString());
            //    if (response.ErrorCode != 0)
            //        response.ErrorMessageList.Add(command.Parameters["P_MENSAJE"].Value.ToString());

            //    ELogWS.save("Paso 3: Se ejecutó " + ProcedureName.pkg_Cotizacion + ".INS_COTIZA_HIS");

            //    if (data.TipoProceso != "1")
            //    {
            //        var valid_result = this.UpdateStatusQuotation(connection, trx, data);
            //        response.Transaction = valid_result.Transaction;
            //        response.GenericFlag = valid_result.GenericFlag;
            //        response.ErrorCode = valid_result.ErrorCode;
            //        response.ErrorMessageList.AddRange(valid_result.ErrorMessageList);
            //    }

            //    if (data.TipoProceso != "2")
            //    {
            //        if (dtCover != null && response.ErrorCode == 0)
            //        {
            //            var valid_result = this.ValidResponseUpdateStatusQuotation(connection, trx, data.NroCotizacion.ToString());
            //            response.ErrorCode = valid_result.ErrorCode;
            //            response.ErrorMessageList.AddRange(valid_result.ErrorMessageList);

            //            ELogWS.save("Paso 4: Se ejecutó " + ProcedureName.pkg_Cotizacion + ".VAL_COTIZACION_COVER");
            //        }

            //        if (response.ErrorCode == 0 && data.Estado != ELog.obtainConfig("RejectedStatusQuotationVL").ToString())
            //        {
            //            this.InsertDetailPremiunEndoso(connection, trx, data.NroCotizacion.ToString(), data, ref response);
            //            if (response.ErrorCode == 0)
            //            {
            //                this.InsertDetailPremiun(connection, trx, data.NroCotizacion.ToString(), data, ref response);
            //            }
            //        }

            //        ELogWS.save("Paso 5: Se ejecutó " + ProcedureName.pkg_Cotizacion + ".UPD_COTIZACION_DET");
            //    }
            //    this.PD_REM_EXC_AUT(connection, trx, data);

            //    //if (response.ErrorCode == 0)
            //    //{
            //    //    trx.Commit();
            //    //    trx.Dispose();
            //    //    ELogWS.save("Paso 6: trx.Commit()");
            //    //}

            //    //ELog.CloseConnection(connection);
            //}
            //catch (Exception ex)
            //{
            //    response.ErrorMessageList.Add(ex.ToString());
            //    response.ErrorCode = 3;
            //    ELogWS.save(ex.ToString());
            //}
            //finally
            //{
            //    if (response.ErrorCode == 0)
            //    {
            //        trx.Commit();
            //        ELogWS.save("Paso 6: trx.Commit()");
            //        if (data.Estado == ELog.obtainConfig("RejectedStatusQuotationVL").ToString())
            //        {
            //            RechazarTramiteProcess(data);
            //        }
            //    }
            //    else
            //    {
            //        if (trx.Connection != null) trx.Rollback();
            //    }

            //    if (trx.Connection != null) trx.Dispose();
            //    ELog.CloseConnection(connection);
            //    //connection.Close();
            //}
        }

        public SalidaTramaBaseVM SaveUsingOracleBulkCopy(DataTable dt, string destTableName)
        {
            SalidaTramaBaseVM resultPackage = new SalidaTramaBaseVM();
            resultPackage.errorList = new List<ErroresVM>();

            try
            {
                resultPackage = this.SaveUsingOracleBulkCopy(destTableName, dt);
            }
            catch (Exception ex)
            {
                resultPackage.P_COD_ERR = "1";
                resultPackage.P_MESSAGE = ex.ToString();
                LogControl.save("SaveUsingOracleBulkCopy", ex.ToString(), "3");
            }

            return resultPackage;
        }

        private GenericResponseVM ValidResponseUpdateStatusQuotation(DbConnection connection, DbTransaction trx, string quotationNumber)
        {
            var response = new GenericResponseVM();
            response.ErrorMessageList = new List<string>();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.pkg_Cotizacion + ".VAL_COTIZACION_COVER";

            try
            {

                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, quotationNumber, ParameterDirection.Input));

                OracleParameter P_MENSAJE = new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, ParameterDirection.Output);

                P_MENSAJE.Size = 4000;

                parameter.Add(P_MENSAJE);
                parameter.Add(P_COD_ERR);

                this.ExecuteByStoredProcedureVT_TRX(storeprocedure, parameter, connection, trx);

                response.ErrorCode = Convert.ToInt32(P_COD_ERR.Value.ToString());
                response.ErrorMessageList.Add(P_MENSAJE.Value.ToString());

                LogControl.save("ValidResponseUpdateStatusQuotation", "Paso X: Se ejecutó " + ProcedureName.pkg_Cotizacion + ".VAL_COTIZACION_COVER", "2");
            }
            catch (Exception ex)
            {
                response.ErrorCode = 3;
                response.ErrorMessageList.Add(ex.Message);
                LogControl.save("ValidResponseUpdateStatusQuotation", JsonConvert.SerializeObject(quotationNumber), "2");
                LogControl.save("ValidResponseUpdateStatusQuotation", ex.ToString(), "3");
            }

            return response;

            ////GenericResponseVM response
            //var response = new GenericResponseVM();
            //response.ErrorMessageList = new List<string>();
            //try
            //{
            //    DbCommand command = connection.CreateCommand();
            //    command.CommandType = CommandType.StoredProcedure;
            //    command.CommandText = ProcedureName.pkg_Cotizacion + ".VAL_COTIZACION_COVER";
            //    command.Transaction = transaction;

            //    command.Parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, quotationNumber, ParameterDirection.Input));
            //    command.Parameters.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output));
            //    command.Parameters.Add(new OracleParameter("P_COD_ERR", OracleDbType.Int32, 4000, null, ParameterDirection.Output));

            //    command.ExecuteNonQuery();

            //    response.ErrorCode = Convert.ToInt32(command.Parameters["P_COD_ERR"].Value.ToString());
            //    response.ErrorMessageList.Add(command.Parameters["P_MENSAJE"].Value.ToString());
            //}
            //catch (Exception ex)
            //{
            //    response.ErrorCode = 3;
            //    response.ErrorMessageList.Add(ex.Message);
            //    ELogWS.save(ex.ToString());
            //}

            //return response;
        }

        private bool InsertDetailPremiun(DbConnection connection, DbTransaction trx, string quotationNumber, UpdateStatusQuotationBM data, ref GenericResponseVM response)
        {
            bool success = true;
            string storeprocedure = ProcedureName.pkg_Cotizacion + ".UPD_COTIZACION_DET";

            for (int i = 0; i < data.PrimaPorCategoriaList.Count(); i++)
            {
                List<OracleParameter> parameter = new List<OracleParameter>();

                if (success)
                {
                    try
                    {
                        RemuneracionMaximaItem rem = new RemuneracionMaximaItem();
                        var codigoCategoria = data.TasasPorCategoriaList.OrderBy(o => o.CodigoCategoria).ToList()[i].CodigoCategoria;
                        var tasa = data.TasasPorCategoriaList.OrderBy(o => o.CodigoCategoria).ToList()[i];
                        var prima = data.PrimaPorCategoriaList.OrderBy(o => o.CodigoCategoria).ToList()[i];
                        if (data.RemMaxPorCategoriaList.Count > 0)
                        {
                            rem = data.RemMaxPorCategoriaList.OrderBy(o => o.CodigoCategoria).ToList()[i];
                        }

                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.NroCotizacion, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_ESTADO", OracleDbType.Int32, 0, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, 117, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, Convert.ToInt32(codigoCategoria), ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NTASA_AUTO", OracleDbType.Double, tasa.TasaNeta, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NPREMIUM_MEN_AUT", OracleDbType.Double, prima.PrimaNeta, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_MIN_PREMIUM_AUT", OracleDbType.Double, 0, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, Convert.ToInt32(data.UsuarioAprobador), ParameterDirection.Input));

                        OracleParameter P_MENSAJE = new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, ParameterDirection.Output);
                        OracleParameter P_COD_ESTADO = new OracleParameter("P_COD_ESTADO", OracleDbType.Int32, ParameterDirection.Output);

                        P_MENSAJE.Size = 4000;

                        parameter.Add(P_MENSAJE);
                        parameter.Add(P_COD_ESTADO);

                        parameter.Add(new OracleParameter("P_NPREMIUM_TOT", OracleDbType.Double, data.PrimaNeta, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NTASA_TOT", OracleDbType.Double, data.TasaNeta, ParameterDirection.Input));

                        parameter.Add(new OracleParameter("P_NTOPE_REM", OracleDbType.Double, data.RemMaxExcedente, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NFLAG_TYP_RATE", OracleDbType.Int32, data.TipoTasa, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NCACALMUL_EXC", OracleDbType.Double, rem.RemuneracionMaxima, ParameterDirection.Input));

                        this.ExecuteByStoredProcedureVT_TRX(storeprocedure, parameter, connection, trx);

                        response.ErrorCode = Convert.ToInt32(P_COD_ESTADO.Value.ToString());

                        if (response.ErrorCode != 0)
                        {
                            response.ErrorCode = 3;
                            response.ErrorMessageList.Add(P_MENSAJE.Value.ToString());
                            success = false;
                        }

                        LogControl.save("InsertDetailPremiun", "Paso X: Se ejecutó " + ProcedureName.pkg_Cotizacion + ".UPD_COTIZACION_DET", "2");
                    }
                    catch (Exception ex)
                    {
                        response.ErrorCode = 3;
                        response.ErrorMessageList.Add(ex.Message);
                        success = false;
                        LogControl.save("InsertDetailPremiun", JsonConvert.SerializeObject(data), "2");
                        LogControl.save("InsertDetailPremiun", ex.ToString(), "3");
                    }
                }

                //DbCommand subCommand = connection.CreateCommand();
                //subCommand.CommandType = CommandType.StoredProcedure;
                //subCommand.CommandText = subProcedureName;
                //subCommand.Transaction = transaction;

                //var codigoCategoria = data.TasasPorCategoriaList.OrderBy(o => o.CodigoCategoria).ToList()[i].CodigoCategoria;
                //var tasa = data.TasasPorCategoriaList.OrderBy(o => o.CodigoCategoria).ToList()[i];
                //var prima = data.PrimaPorCategoriaList.OrderBy(o => o.CodigoCategoria).ToList()[i];
                //var rem = data.RemMaxPorCategoriaList.OrderBy(o => o.CodigoCategoria).ToList()[i];

                //subCommand.Parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.NroCotizacion, ParameterDirection.Input));
                //subCommand.Parameters.Add(new OracleParameter("P_ESTADO", OracleDbType.Int32, 0, ParameterDirection.Input));
                //subCommand.Parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, 117, ParameterDirection.Input));
                //subCommand.Parameters.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, Convert.ToInt32(codigoCategoria), ParameterDirection.Input));
                //subCommand.Parameters.Add(new OracleParameter("P_NTASA_AUTO", OracleDbType.Double, tasa.TasaNeta, ParameterDirection.Input));
                //subCommand.Parameters.Add(new OracleParameter("P_NPREMIUM_MEN_AUT", OracleDbType.Double, prima.PrimaNeta, ParameterDirection.Input));
                //subCommand.Parameters.Add(new OracleParameter("P_MIN_PREMIUM_AUT", OracleDbType.Double, 0, ParameterDirection.Input));
                //subCommand.Parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, Convert.ToInt32(data.UsuarioAprobador), ParameterDirection.Input));

                //subCommand.Parameters.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output));
                //subCommand.Parameters.Add(new OracleParameter("P_COD_ESTADO", OracleDbType.Int32, 4000, null, ParameterDirection.Output));

                //subCommand.Parameters.Add(new OracleParameter("P_NPREMIUM_TOT", OracleDbType.Double, data.PrimaNeta, ParameterDirection.Input));
                //subCommand.Parameters.Add(new OracleParameter("P_NTASA_TOT", OracleDbType.Double, data.TasaNeta, ParameterDirection.Input));

                //subCommand.Parameters.Add(new OracleParameter("P_NTOPE_REM", OracleDbType.Double, data.RemMaxExcedente, ParameterDirection.Input));
                //subCommand.Parameters.Add(new OracleParameter("P_NFLAG_TYP_RATE", OracleDbType.Varchar2, data.TipoTasa, ParameterDirection.Input));
                //subCommand.Parameters.Add(new OracleParameter("P_NCACALMUL_EXC", OracleDbType.Double, rem.RemuneracionMaxima, ParameterDirection.Input));

                //subCommand.ExecuteNonQuery();

                //if (Convert.ToInt32(subCommand.Parameters["P_COD_ESTADO"].Value.ToString()) != 0)
                //{
                //    response.ErrorCode = 3;
                //    response.ErrorMessageList.Add(subCommand.Parameters["P_MENSAJE"].Value.ToString());

                //    success = false;
                //    transaction.Rollback();
                //    ELog.CloseConnection(connection);
                //    return success;
                //}
            }

            return success;
        }

        private bool InsertDetailPremiunEndoso(DbConnection connection, DbTransaction trx, string quotationNumber, UpdateStatusQuotationBM data, ref GenericResponseVM response)
        {
            bool success = true;
            string storeprocedure = ProcedureName.pkg_Cotizacion + ".INS_COTIZACION_DET_CLONAR";

            for (int i = 0; i < data.PrimaPorCategoriaList.Count(); i++)
            {
                List<OracleParameter> parameter = new List<OracleParameter>();

                if (success)
                {
                    try
                    {
                        RemuneracionMaximaItem rem = new RemuneracionMaximaItem();
                        var codigoCategoria = data.TasasPorCategoriaList.OrderBy(o => o.CodigoCategoria).ToList()[i].CodigoCategoria;
                        var tasa = data.TasasPorCategoriaList.OrderBy(o => o.CodigoCategoria).ToList()[i];
                        var prima = data.PrimaPorCategoriaList.OrderBy(o => o.CodigoCategoria).ToList()[i];
                        if (data.RemMaxPorCategoriaList.Count > 0)
                        {
                            rem = data.RemMaxPorCategoriaList.OrderBy(o => o.CodigoCategoria).ToList()[i];
                        }

                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.NroCotizacion, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, Convert.ToInt32(codigoCategoria), ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NMONTO_PLANILLA", OracleDbType.Double, /*rem.RemuneracionMaxima*/0, ParameterDirection.Input)); // ENDOSO TECNICA JTV 11042023
                        parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, Convert.ToInt32(data.UsuarioAprobador), ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NTASA_AUTO", OracleDbType.Double, tasa.TasaNeta, ParameterDirection.Input));

                        OracleParameter P_MENSAJE = new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, ParameterDirection.Output);
                        OracleParameter P_COD_ESTADO = new OracleParameter("P_COD_ESTADO", OracleDbType.Int32, ParameterDirection.Output);

                        P_MENSAJE.Size = 4000;

                        parameter.Add(P_MENSAJE);
                        parameter.Add(P_COD_ESTADO);

                        this.ExecuteByStoredProcedureVT_TRX(storeprocedure, parameter, connection, trx);

                        response.ErrorCode = Convert.ToInt32(P_COD_ESTADO.Value.ToString());

                        if (response.ErrorCode != 0)
                        {
                            response.ErrorCode = 3;
                            response.ErrorMessageList.Add(P_MENSAJE.Value.ToString());
                            success = false;
                        }

                        LogControl.save("InsertDetailPremiunEndoso", "Paso X: Se ejecutó " + ProcedureName.pkg_Cotizacion + ".INS_COTIZACION_DET_CLONAR", "2");
                    }
                    catch (Exception ex)
                    {
                        response.ErrorCode = 3;
                        response.ErrorMessageList.Add(ex.Message);
                        success = false;
                        LogControl.save("InsertDetailPremiunEndoso", JsonConvert.SerializeObject(data), "2");
                        LogControl.save("InsertDetailPremiunEndoso", ex.ToString(), "3");
                    }
                }
            }

            return success;

            //    DbCommand subCommand = connection.CreateCommand();
            //    subCommand.CommandType = CommandType.StoredProcedure;
            //    subCommand.CommandText = subProcedureName;
            //    subCommand.Transaction = transaction;

            //    var codigoCategoria = data.TasasPorCategoriaList.OrderBy(o => o.CodigoCategoria).ToList()[i].CodigoCategoria;
            //    var tasa = data.TasasPorCategoriaList.OrderBy(o => o.CodigoCategoria).ToList()[i];
            //    var prima = data.PrimaPorCategoriaList.OrderBy(o => o.CodigoCategoria).ToList()[i];
            //    var rem = data.RemMaxPorCategoriaList.OrderBy(o => o.CodigoCategoria).ToList()[i];

            //    subCommand.Parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.NroCotizacion, ParameterDirection.Input));
            //    subCommand.Parameters.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, Convert.ToInt32(codigoCategoria), ParameterDirection.Input));
            //    subCommand.Parameters.Add(new OracleParameter("P_NMONTO_PLANILLA", OracleDbType.Double, rem.RemuneracionMaxima, ParameterDirection.Input));
            //    subCommand.Parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, Convert.ToInt32(data.UsuarioAprobador), ParameterDirection.Input));
            //    subCommand.Parameters.Add(new OracleParameter("P_NTASA_AUTO", OracleDbType.Double, tasa.TasaNeta, ParameterDirection.Input));

            //    subCommand.Parameters.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output));
            //    subCommand.Parameters.Add(new OracleParameter("P_COD_ESTADO", OracleDbType.Int32, 4000, null, ParameterDirection.Output));

            //    subCommand.ExecuteNonQuery();

            //    if (Convert.ToInt32(subCommand.Parameters["P_COD_ESTADO"].Value.ToString()) != 0)
            //    {
            //        response.ErrorCode = 3;
            //        response.ErrorMessageList.Add(subCommand.Parameters["P_MENSAJE"].Value.ToString());

            //        success = false;

            //        transaction.Rollback();

            //        ELog.CloseConnection(connection);
            //        return success;
            //    }
            //}
            //return success;
        }

        #region 24012024 GCAA PROCESAR NUEVOS TIPOS DEL COTIZADOR
        // GCAA 31012024
        private bool InsertDetailPremiunRangoEdadxCategoria(DbConnection connection, DbTransaction trx, string quotationNumber, UpdateStatusQuotationBM data, ref GenericResponseVM response)
        {
            bool success = true;
            string storeprocedure = "UPD_COTIZACION_DET_RANGO";
            double primaNeta = 0;
            double tasa = 0;

            for (int i = 0; i < data.tasasPlanillasPrimasCodigoCategoriaExcesoTopeRemuneraciones.Count(); i++)
            {
                List<OracleParameter> parameter = new List<OracleParameter>();

                if (success)
                {
                    try
                    {
                        if (data.tasasPlanillasPrimasCodigoCategoriaExcesoTopeRemuneraciones[i].descripcion == "Tasa Comercial Mensual")
                        {
                            RemuneracionMaximaItem rem = new RemuneracionMaximaItem();

                            if (data.RemMaxPorCategoriaList.Count > 0)
                            {
                                rem = data.RemMaxPorCategoriaList.OrderBy(o => o.CodigoCategoria).ToList()[i];
                            }

                            if (data.tasasPlanillasPrimasCodigoCategoriaExcesoTopeRemuneraciones[i + 1].valor != 0)
                            {
                                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.NroCotizacion, ParameterDirection.Input));
                                parameter.Add(new OracleParameter("P_ESTADO", OracleDbType.Int32, 0, ParameterDirection.Input));
                                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, 117, ParameterDirection.Input));
                                parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, Convert.ToInt32(data.tasasPlanillasPrimasCodigoCategoriaExcesoTopeRemuneraciones[i].codigoCategoria), ParameterDirection.Input));
                                parameter.Add(new OracleParameter("P_NTASA_AUTO", OracleDbType.Double, data.tasasPlanillasPrimasCodigoCategoriaExcesoTopeRemuneraciones[i].valor, ParameterDirection.Input));
                                parameter.Add(new OracleParameter("P_NPREMIUM_MEN_AUT", OracleDbType.Double, data.tasasPlanillasPrimasCodigoCategoriaExcesoTopeRemuneraciones[i + 1].valor, ParameterDirection.Input));
                                parameter.Add(new OracleParameter("P_MIN_PREMIUM_AUT", OracleDbType.Double, 0, ParameterDirection.Input));
                                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, Convert.ToInt32(data.UsuarioAprobador), ParameterDirection.Input));


                                OracleParameter P_MENSAJE = new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, ParameterDirection.Output);
                                OracleParameter P_COD_ESTADO = new OracleParameter("P_COD_ESTADO", OracleDbType.Int32, ParameterDirection.Output);

                                P_MENSAJE.Size = 4000;

                                parameter.Add(P_MENSAJE);
                                parameter.Add(P_COD_ESTADO);

                                parameter.Add(new OracleParameter("P_NPREMIUM_TOT", OracleDbType.Double, data.PrimaNeta, ParameterDirection.Input));
                                parameter.Add(new OracleParameter("P_NTASA_TOT", OracleDbType.Double, data.TasaNeta, ParameterDirection.Input));

                                parameter.Add(new OracleParameter("P_NTOPE_REM", OracleDbType.Double, data.RemMaxExcedente, ParameterDirection.Input));
                                parameter.Add(new OracleParameter("P_NFLAG_TYP_RATE", OracleDbType.Int32, data.TipoTasa, ParameterDirection.Input));
                                parameter.Add(new OracleParameter("P_NCACALMUL_EXC", OracleDbType.Double, data.tasasPlanillasPrimasCodigoCategoriaExcesoTopeRemuneraciones[i - 2].valor, ParameterDirection.Input));
                                parameter.Add(new OracleParameter("P_RANGO_EDAD", OracleDbType.Varchar2, data.tasasPlanillasPrimasCodigoCategoriaExcesoTopeRemuneraciones[i].rango_edad, ParameterDirection.Input));

                                this.ExecuteByStoredProcedureVT_TRX(storeprocedure, parameter, connection, trx);

                                response.ErrorCode = Convert.ToInt32(P_COD_ESTADO.Value.ToString());

                                if (response.ErrorCode != 0)
                                {
                                    response.ErrorCode = 3;
                                    response.ErrorMessageList.Add(P_MENSAJE.Value.ToString());
                                    success = false;
                                }
                            }
                        }
                        LogControl.save("InsertDetailPremiunRangoEdadxCategoria", "Paso X: Se ejecutó " + ProcedureName.pkg_Cotizacion + ".UPD_COTIZACION_DET_RANGO", "2");
                    }
                    catch (Exception ex)
                    {
                        response.ErrorCode = 3;
                        response.ErrorMessageList.Add(ex.Message);
                        success = false;
                        LogControl.save("InsertDetailPremiunRangoEdadxCategoria", JsonConvert.SerializeObject(data), "2");
                        LogControl.save("InsertDetailPremiunRangoEdadxCategoria", ex.ToString(), "3");
                    }
                }
            }

            return success;
        }

        // GCAA 24012024
        private bool InsertDetalleCotizador(DbConnection connection, DbTransaction trx, string quotationNumber, UpdateStatusQuotationBM data, ref GenericResponseVM response)
        {
            bool success = true;
            string storeprocedure = "PD_PROCESA_DET_COTIZACION_BK";

            List<OracleParameter> parameter = new List<OracleParameter>();

            if (success)
            {
                try
                {
                    parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.NroCotizacion, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_TIPO", OracleDbType.Int32, data.opcion, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_NUSERCODE_P", OracleDbType.Int32, Convert.ToInt32(data.UsuarioAprobador), ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_TOPE", OracleDbType.Double, Convert.ToDouble(data.RemMaxExcedente), ParameterDirection.Input));


                    OracleParameter P_MENSAJE = new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, ParameterDirection.Output);
                    OracleParameter P_COD_ESTADO = new OracleParameter("P_COD_ESTADO", OracleDbType.Int32, ParameterDirection.Output);

                    P_MENSAJE.Size = 4000;

                    parameter.Add(P_MENSAJE);
                    parameter.Add(P_COD_ESTADO);

                    this.ExecuteByStoredProcedureVT_TRX(storeprocedure, parameter, connection, trx);

                    response.ErrorCode = Convert.ToInt32(P_COD_ESTADO.Value.ToString());

                    if (response.ErrorCode != 0)
                    {
                        response.ErrorCode = 3;
                        response.ErrorMessageList.Add(P_MENSAJE.Value.ToString());
                        success = false;
                    }

                    LogControl.save("InsertDetalleCotizador", "Paso X: Se ejecutó " + ProcedureName.pkg_Cotizacion + ".PD_PROCESA_DET_COTIZACION", "2");
                }
                catch (Exception ex)
                {
                    response.ErrorCode = 3;
                    response.ErrorMessageList.Add(ex.Message);
                    success = false;
                    LogControl.save("InsertDetalleCotizador", JsonConvert.SerializeObject(data), "2");
                    LogControl.save("InsertDetalleCotizador", ex.ToString(), "3");
                }
            }


            return success;

        }

        // GCAA 24012024
        private bool DeleteOldRangoEdad(DbConnection connection, DbTransaction trx, string quotationNumber, UpdateStatusQuotationBM data, ref GenericResponseVM response)
        {
            bool success = true;
            string storeprocedure = "PD_PROCESA_NEW_RANGO";

            List<OracleParameter> parameter = new List<OracleParameter>();

            if (success)
            {
                try
                {
                    parameter.Add(new OracleParameter("P_TIPO", OracleDbType.Int32, 1, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.NroCotizacion, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_RANGO", OracleDbType.Varchar2, "", ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, 1, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_NTASA", OracleDbType.Int32, 0, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("N_ORDER", OracleDbType.Double, 0, ParameterDirection.Input));

                    OracleParameter P_COD_ESTADO = new OracleParameter("P_COD_ESTADO", OracleDbType.Int32, ParameterDirection.Output);
                    OracleParameter P_MENSAJE = new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, ParameterDirection.Output);
                    OracleParameter P_ORDER = new OracleParameter("P_ORDER", OracleDbType.Int32, ParameterDirection.Output);

                    P_MENSAJE.Size = 4000;

                    parameter.Add(P_MENSAJE);
                    parameter.Add(P_COD_ESTADO);
                    parameter.Add(P_ORDER);

                    this.ExecuteByStoredProcedureVT_TRX(storeprocedure, parameter, connection, trx);

                    response.ErrorCode = Convert.ToInt32(P_COD_ESTADO.Value.ToString());
                    response.Order = Convert.ToInt32(P_ORDER.Value.ToString());

                    if (response.ErrorCode != 0)
                    {
                        response.ErrorCode = 3;
                        response.ErrorMessageList.Add(P_MENSAJE.Value.ToString());
                        success = false;
                    }

                    LogControl.save("DeleteOldRangoEdad", "Paso X: Se ejecutó " + ProcedureName.pkg_Cotizacion + ".PD_PROCESA_NEW_RANGO", "2");
                }
                catch (Exception ex)
                {
                    response.ErrorCode = 3;
                    response.ErrorMessageList.Add(ex.Message);
                    success = false;
                    LogControl.save("DeleteOldRangoEdad", JsonConvert.SerializeObject(data), "2");
                    LogControl.save("DeleteOldRangoEdad", ex.ToString(), "3");
                }
            }
            return success;
        }


        public bool update_table_peso(int quotationNumber)
        {
            bool success = true;
            string storeprocedure = "PD_PROCESA_NEW_RANGO";

            List<OracleParameter> parameter = new List<OracleParameter>();
            var response = new WSPlataforma.Entities.PolicyModel.ViewModel.ErrorCode();
            if (success)
            {
                try
                {
                    parameter.Add(new OracleParameter("P_TIPO", OracleDbType.Int32, 4, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, quotationNumber, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_RANGO", OracleDbType.Varchar2, "", ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, 0, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_NTASA", OracleDbType.Int32, 0, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("N_ORDER", OracleDbType.Double, 0, ParameterDirection.Input));

                    OracleParameter P_COD_ESTADO = new OracleParameter("P_COD_ESTADO", OracleDbType.Int32, ParameterDirection.Output);
                    OracleParameter P_MENSAJE = new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, ParameterDirection.Output);
                    OracleParameter P_ORDER = new OracleParameter("P_ORDER", OracleDbType.Int32, ParameterDirection.Output);

                    P_MENSAJE.Size = 4000;

                    parameter.Add(P_MENSAJE);
                    parameter.Add(P_COD_ESTADO);
                    parameter.Add(P_ORDER);

                    this.ExecuteByStoredProcedureVT(storeprocedure, parameter);

                    response.P_COD_ERR = Convert.ToInt32(P_COD_ESTADO.Value.ToString());
                    // response.Order = Convert.ToInt32(P_ORDER.Value.ToString());

                    if (response.P_COD_ERR != 0)
                    {
                        response.P_COD_ERR = 3;
                        response.P_MESSAGE = P_MENSAJE.Value.ToString();
                        success = false;
                    }

                    LogControl.save("update_table_peso", "Paso X: Se ejecutó " + ProcedureName.pkg_Cotizacion + ".PD_PROCESA_NEW_RANGO", "4");
                }
                catch (Exception ex)
                {
                    response.P_COD_ERR = 1;
                    response.P_MESSAGE = ex.Message.ToString();
                    success = false;
                    LogControl.save("update_table_peso", JsonConvert.SerializeObject(ex.Message.ToString()), "2");
                    LogControl.save("update_table_peso", ex.ToString(), "3");
                }
            }
            return success;
        }


        // GCAA 24012024
        private bool InsertNewRangoEdad(DbConnection connection, DbTransaction trx, string quotationNumber, UpdateStatusQuotationBM data, ref GenericResponseVM response)
        {
            bool success = true;
            string storeprocedure = "PD_PROCESA_NEW_RANGO";

            for (int i = 0; i < data.tasasPlanillasPrimasRangoEdadHastaTopeRemuneraciones.Count(); i++)
            {
                if (data.tasasPlanillasPrimasRangoEdadHastaTopeRemuneraciones[i].descripcion == "Tasa Comercial Mensual")
                {
                    List<OracleParameter> parameter = new List<OracleParameter>();

                    if (success)
                    {
                        try
                        {
                            parameter.Add(new OracleParameter("P_TIPO", OracleDbType.Int32, 2, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.NroCotizacion, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_RANGO", OracleDbType.Varchar2, data.tasasPlanillasPrimasRangoEdadHastaTopeRemuneraciones[i].rango_edad, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, 1, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NTASA", OracleDbType.Double, data.tasasPlanillasPrimasRangoEdadHastaTopeRemuneraciones[i].valor, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("N_ORDER", OracleDbType.Double, 0, ParameterDirection.Input));

                            OracleParameter P_COD_ESTADO = new OracleParameter("P_COD_ESTADO", OracleDbType.Int32, ParameterDirection.Output);
                            OracleParameter P_MENSAJE = new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, ParameterDirection.Output);
                            OracleParameter P_ORDER = new OracleParameter("P_ORDER", OracleDbType.Int32, ParameterDirection.Output);


                            P_MENSAJE.Size = 4000;

                            parameter.Add(P_MENSAJE);
                            parameter.Add(P_COD_ESTADO);
                            parameter.Add(P_ORDER);

                            this.ExecuteByStoredProcedureVT_TRX(storeprocedure, parameter, connection, trx);

                            response.ErrorCode = Convert.ToInt32(P_COD_ESTADO.Value.ToString());
                            response.Order = Convert.ToInt32(P_ORDER.Value.ToString());


                            if (response.ErrorCode != 0)
                            {
                                response.ErrorCode = 3;
                                response.ErrorMessageList.Add(P_MENSAJE.Value.ToString());
                                success = false;
                            }

                            LogControl.save("InsertNewRangoEdad", "Paso X: Se ejecutó " + ProcedureName.pkg_Cotizacion + ".PD_PROCESA_NEW_RANGO", "2");
                        }
                        catch (Exception ex)
                        {
                            response.ErrorCode = 3;
                            response.ErrorMessageList.Add(ex.Message);
                            success = false;
                            LogControl.save("InsertNewRangoEdad", JsonConvert.SerializeObject(data), "2");
                            LogControl.save("InsertNewRangoEdad", ex.ToString(), "3");
                        }
                    }
                }
            }

            return success;

        }

        // GCAA 24012024
        private bool InsertNewRangoEdadExcedente(DbConnection connection, DbTransaction trx, string quotationNumber, UpdateStatusQuotationBM data, ref GenericResponseVM response)
        {
            bool success = true;
            string storeprocedure = "PD_PROCESA_NEW_RANGO";

            for (int i = 0; i < data.tasasPlanillasPrimasRangoEdadExcesoTopeRemuneraciones.Count(); i++)
            {
                if (data.tasasPlanillasPrimasRangoEdadExcesoTopeRemuneraciones[i].descripcion == "Tasa Comercial Mensual")
                {
                    List<OracleParameter> parameter = new List<OracleParameter>();

                    if (success)
                    {
                        try
                        {
                            parameter.Add(new OracleParameter("P_TIPO", OracleDbType.Int32, 2, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.NroCotizacion, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_RANGO", OracleDbType.Varchar2, data.tasasPlanillasPrimasRangoEdadExcesoTopeRemuneraciones[i].rango_edad, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, 1, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NTASA", OracleDbType.Double, data.tasasPlanillasPrimasRangoEdadExcesoTopeRemuneraciones[i].valor, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("N_ORDER", OracleDbType.Double, 0, ParameterDirection.Input));


                            OracleParameter P_COD_ESTADO = new OracleParameter("P_COD_ESTADO", OracleDbType.Int32, ParameterDirection.Output);
                            OracleParameter P_MENSAJE = new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, ParameterDirection.Output);
                            OracleParameter P_ORDER = new OracleParameter("P_ORDER", OracleDbType.Int32, ParameterDirection.Output);


                            P_MENSAJE.Size = 4000;

                            parameter.Add(P_MENSAJE);
                            parameter.Add(P_COD_ESTADO);
                            parameter.Add(P_ORDER);

                            this.ExecuteByStoredProcedureVT_TRX(storeprocedure, parameter, connection, trx);

                            response.ErrorCode = Convert.ToInt32(P_COD_ESTADO.Value.ToString());
                            response.Order = Convert.ToInt32(P_ORDER.Value.ToString());

                            if (response.ErrorCode != 0)
                            {
                                response.ErrorCode = 3;
                                response.ErrorMessageList.Add(P_MENSAJE.Value.ToString());
                                success = false;
                            }

                            LogControl.save("InsertNewRangoEdadExcedente", "Paso X: Se ejecutó " + ProcedureName.pkg_Cotizacion + ".PD_PROCESA_NEW_RANGO", "2");
                        }
                        catch (Exception ex)
                        {
                            response.ErrorCode = 3;
                            response.ErrorMessageList.Add(ex.Message);
                            success = false;
                            LogControl.save("InsertNewRangoEdadExcedente", JsonConvert.SerializeObject(data), "2");
                            LogControl.save("InsertNewRangoEdadExcedente", ex.ToString(), "3");
                        }
                    }
                }
            }

            return success;

        }

        // GCAA 24012024
        private bool InsertNewRangoEdadxCategoria(DbConnection connection, DbTransaction trx, string quotationNumber, UpdateStatusQuotationBM data, ref GenericResponseVM response)
        {
            bool success = true;
            string storeprocedure = "PD_PROCESA_NEW_RANGO";

            for (int i = 0; i < data.tasasPlanillasPrimasCodigoCategoriaHastaTopeRemuneraciones.Count(); i++)
            {
                if (data.tasasPlanillasPrimasCodigoCategoriaHastaTopeRemuneraciones[i].descripcion == "Tasa Comercial Mensual")
                {
                    List<OracleParameter> parameter = new List<OracleParameter>();

                    if (success)
                    {
                        try
                        {
                            parameter.Add(new OracleParameter("P_TIPO", OracleDbType.Int32, 3, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.NroCotizacion, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_RANGO", OracleDbType.Varchar2, data.tasasPlanillasPrimasCodigoCategoriaHastaTopeRemuneraciones[i].rango_edad, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, data.tasasPlanillasPrimasCodigoCategoriaHastaTopeRemuneraciones[i].codigoCategoria, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NTASA", OracleDbType.Double, data.tasasPlanillasPrimasCodigoCategoriaHastaTopeRemuneraciones[i].valor, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("N_ORDER", OracleDbType.Double, 0, ParameterDirection.Input));



                            OracleParameter P_COD_ESTADO = new OracleParameter("P_COD_ESTADO", OracleDbType.Int32, ParameterDirection.Output);
                            OracleParameter P_MENSAJE = new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, ParameterDirection.Output);
                            OracleParameter P_ORDER = new OracleParameter("P_ORDER", OracleDbType.Int32, ParameterDirection.Output);

                            P_MENSAJE.Size = 4000;

                            parameter.Add(P_MENSAJE);
                            parameter.Add(P_COD_ESTADO);
                            parameter.Add(P_ORDER);


                            this.ExecuteByStoredProcedureVT_TRX(storeprocedure, parameter, connection, trx);

                            response.ErrorCode = Convert.ToInt32(P_COD_ESTADO.Value.ToString());
                            response.Order = Convert.ToInt32(P_ORDER.Value.ToString());

                            if (response.ErrorCode != 0)
                            {
                                response.ErrorCode = 3;
                                response.ErrorMessageList.Add(P_MENSAJE.Value.ToString());
                                success = false;
                            }

                            LogControl.save("InsertNewRangoEdadxCategoria", "Paso X: Se ejecutó " + ProcedureName.pkg_Cotizacion + ".PD_PROCESA_NEW_RANGO", "2");
                        }
                        catch (Exception ex)
                        {
                            response.ErrorCode = 3;
                            response.ErrorMessageList.Add(ex.Message);
                            success = false;
                            LogControl.save("InsertNewRangoEdadxCategoria", JsonConvert.SerializeObject(data), "2");
                            LogControl.save("InsertNewRangoEdadxCategoria", ex.ToString(), "3");
                        }
                    }
                }
            }

            return success;

        }

        // GCAA 24012024
        private bool InsertNewRangoEdadxCategoriaExcedente(DbConnection connection, DbTransaction trx, string quotationNumber, UpdateStatusQuotationBM data, ref GenericResponseVM response)
        {
            bool success = true;
            string storeprocedure = "PD_PROCESA_NEW_RANGO";

            for (int i = 0; i < data.tasasPlanillasPrimasCodigoCategoriaExcesoTopeRemuneraciones.Count(); i++)
            {
                if (data.tasasPlanillasPrimasCodigoCategoriaExcesoTopeRemuneraciones[i].descripcion == "Tasa Comercial Mensual")
                {
                    List<OracleParameter> parameter = new List<OracleParameter>();

                    if (success)
                    {
                        try
                        {
                            parameter.Add(new OracleParameter("P_TIPO", OracleDbType.Int32, 3, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.NroCotizacion, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_RANGO", OracleDbType.Varchar2, data.tasasPlanillasPrimasCodigoCategoriaExcesoTopeRemuneraciones[i].rango_edad, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, data.tasasPlanillasPrimasCodigoCategoriaExcesoTopeRemuneraciones[i].codigoCategoria, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NTASA", OracleDbType.Double, data.tasasPlanillasPrimasCodigoCategoriaExcesoTopeRemuneraciones[i].valor, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("N_ORDER", OracleDbType.Double, response.Order, ParameterDirection.Input));



                            OracleParameter P_COD_ESTADO = new OracleParameter("P_COD_ESTADO", OracleDbType.Int32, ParameterDirection.Output);
                            OracleParameter P_MENSAJE = new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, ParameterDirection.Output);
                            OracleParameter P_ORDER = new OracleParameter("P_ORDER", OracleDbType.Int32, ParameterDirection.Output);

                            P_MENSAJE.Size = 4000;

                            parameter.Add(P_MENSAJE);
                            parameter.Add(P_COD_ESTADO);
                            parameter.Add(P_ORDER);

                            this.ExecuteByStoredProcedureVT_TRX(storeprocedure, parameter, connection, trx);

                            response.ErrorCode = Convert.ToInt32(P_COD_ESTADO.Value.ToString());
                            response.Order = Convert.ToInt32(P_ORDER.Value.ToString());

                            if (response.ErrorCode != 0)
                            {
                                response.ErrorCode = 3;
                                response.ErrorMessageList.Add(P_MENSAJE.Value.ToString());
                                success = false;
                            }

                            LogControl.save("InsertNewRangoEdadxCategoriaExcedente", "Paso X: Se ejecutó " + ProcedureName.pkg_Cotizacion + ".PD_PROCESA_NEW_RANGO", "2");
                        }
                        catch (Exception ex)
                        {
                            response.ErrorCode = 3;
                            response.ErrorMessageList.Add(ex.Message);
                            success = false;
                            LogControl.save("InsertNewRangoEdadxCategoriaExcedente", JsonConvert.SerializeObject(data), "2");
                            LogControl.save("InsertNewRangoEdadxCategoriaExcedente", ex.ToString(), "3");
                        }
                    }
                }
            }

            return success;

        }

        // GCAA 24012024
        private bool InsertNewxCategoria(DbConnection connection, DbTransaction trx, string quotationNumber, UpdateStatusQuotationBM data, ref GenericResponseVM response)
        {
            bool success = true;
            string storeprocedure = "PD_PROCESA_NEW_RANGO";

            for (int i = 0; i < data.tasasPlanillasPrimasCategoriaTrabajadorHastaTopeRemuneraciones.Count(); i++)
            {
                if (data.tasasPlanillasPrimasCategoriaTrabajadorHastaTopeRemuneraciones[i].descripcion == "Tasa Comercial Mensual")
                {
                    List<OracleParameter> parameter = new List<OracleParameter>();

                    if (success)
                    {
                        try
                        {
                            parameter.Add(new OracleParameter("P_TIPO", OracleDbType.Int32, 3, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.NroCotizacion, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_RANGO", OracleDbType.Varchar2, "", ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, data.tasasPlanillasPrimasCategoriaTrabajadorHastaTopeRemuneraciones[i].codigoCategoria, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NTASA", OracleDbType.Double, data.tasasPlanillasPrimasCategoriaTrabajadorHastaTopeRemuneraciones[i].valor, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("N_ORDER", OracleDbType.Double, 0, ParameterDirection.Input));

                            OracleParameter P_COD_ESTADO = new OracleParameter("P_COD_ESTADO", OracleDbType.Int32, ParameterDirection.Output);
                            OracleParameter P_MENSAJE = new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, ParameterDirection.Output);
                            OracleParameter P_ORDER = new OracleParameter("P_ORDER", OracleDbType.Int32, ParameterDirection.Output);


                            P_MENSAJE.Size = 4000;

                            parameter.Add(P_MENSAJE);
                            parameter.Add(P_COD_ESTADO);
                            parameter.Add(P_ORDER);


                            this.ExecuteByStoredProcedureVT_TRX(storeprocedure, parameter, connection, trx);

                            response.ErrorCode = Convert.ToInt32(P_COD_ESTADO.Value.ToString());
                            response.Order = Convert.ToInt32(P_ORDER.Value.ToString());



                            if (response.ErrorCode != 0)
                            {
                                response.ErrorCode = 3;
                                response.ErrorMessageList.Add(P_MENSAJE.Value.ToString());
                                success = false;
                            }

                            LogControl.save("InsertNewxCategoria", "Paso X: Se ejecutó " + ProcedureName.pkg_Cotizacion + ".PD_PROCESA_NEW_RANGO", "2");
                        }
                        catch (Exception ex)
                        {
                            response.ErrorCode = 3;
                            response.ErrorMessageList.Add(ex.Message);
                            success = false;
                            LogControl.save("InsertNewxCategoria", JsonConvert.SerializeObject(data), "2");
                            LogControl.save("InsertNewxCategoria", ex.ToString(), "3");
                        }
                    }
                }
            }

            return success;

        }
        private bool InsertNewxCategoriaPonderada(DbConnection connection, DbTransaction trx, string quotationNumber, UpdateStatusQuotationBM data, ref GenericResponseVM response)
        {
            bool success = true;
            string storeprocedure = "PD_PROCESA_NEW_RANGO";

            for (int i = 0; i < data.TasasPorCategoriaList.Count(); i++)
            {
                
                List<OracleParameter> parameter = new List<OracleParameter>();

                if (success)
                {
                    try
                    {
                        parameter.Add(new OracleParameter("P_TIPO", OracleDbType.Int32, 3, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.NroCotizacion, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_RANGO", OracleDbType.Varchar2, "", ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, data.TasasPorCategoriaList[i].CodigoCategoria, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NTASA", OracleDbType.Double, data.TasasPorCategoriaList[i].TasaNeta, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("N_ORDER", OracleDbType.Double, 0, ParameterDirection.Input));

                        OracleParameter P_COD_ESTADO = new OracleParameter("P_COD_ESTADO", OracleDbType.Int32, ParameterDirection.Output);
                        OracleParameter P_MENSAJE = new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, ParameterDirection.Output);
                        OracleParameter P_ORDER = new OracleParameter("P_ORDER", OracleDbType.Int32, ParameterDirection.Output);


                        P_MENSAJE.Size = 4000;

                        parameter.Add(P_MENSAJE);
                        parameter.Add(P_COD_ESTADO);
                        parameter.Add(P_ORDER);


                        this.ExecuteByStoredProcedureVT_TRX(storeprocedure, parameter, connection, trx);

                        response.ErrorCode = Convert.ToInt32(P_COD_ESTADO.Value.ToString());
                        response.Order = Convert.ToInt32(P_ORDER.Value.ToString());



                        if (response.ErrorCode != 0)
                        {
                            response.ErrorCode = 3;
                            response.ErrorMessageList.Add(P_MENSAJE.Value.ToString());
                            success = false;
                        }

                        LogControl.save("InsertNewxCategoria", "Paso X: Se ejecutó " + ProcedureName.pkg_Cotizacion + ".PD_PROCESA_NEW_RANGO", "2");
                    }
                    catch (Exception ex)
                    {
                        response.ErrorCode = 3;
                        response.ErrorMessageList.Add(ex.Message);
                        success = false;
                        LogControl.save("InsertNewxCategoria", JsonConvert.SerializeObject(data), "2");
                        LogControl.save("InsertNewxCategoria", ex.ToString(), "3");
                    }
                }
                
            }

            return success;

        }
        // GCAA 24012024
        private bool InsertNewxCategoriaMatriz(DbConnection connection, DbTransaction trx, string quotationNumber, UpdateStatusQuotationBM data, ref GenericResponseVM response)
        {
            bool success = true;
            string storeprocedure = "PD_PROCESA_NEW_RANGO";

            //for (int i = 0; i < data.TasasPorCategoriaList.Count(); i++)
            // {
                List<OracleParameter> parameter = new List<OracleParameter>();

                if (success)
                {
                    try
                    {
                    parameter.Add(new OracleParameter("P_TIPO", OracleDbType.Int32, 5, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.NroCotizacion, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_RANGO", OracleDbType.Varchar2, "", ParameterDirection.Input));
                    //  parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, data.TasasPorCategoriaList[i].CodigoCategoria, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, 1, ParameterDirection.Input));
                    //parameter.Add(new OracleParameter("P_NTASA", OracleDbType.Double, data.TasasPorCategoriaList[i].TasaNeta, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_NTASA", OracleDbType.Double, data.TasasPorCategoriaList[0].TasaNeta, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("N_ORDER", OracleDbType.Double, response.Order, ParameterDirection.Input));

                        OracleParameter P_COD_ESTADO = new OracleParameter("P_COD_ESTADO", OracleDbType.Int32, ParameterDirection.Output);
                        OracleParameter P_MENSAJE = new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, ParameterDirection.Output);
                        OracleParameter P_ORDER = new OracleParameter("P_ORDER", OracleDbType.Int32, ParameterDirection.Output);

                        P_MENSAJE.Size = 4000;

                        parameter.Add(P_MENSAJE);
                        parameter.Add(P_COD_ESTADO);
                        parameter.Add(P_ORDER);


                        this.ExecuteByStoredProcedureVT_TRX(storeprocedure, parameter, connection, trx);

                        response.ErrorCode = Convert.ToInt32(P_COD_ESTADO.Value.ToString());
                        response.Order = Convert.ToInt32(P_ORDER.Value.ToString());

                        if (response.ErrorCode != 0)
                        {
                            response.ErrorCode = 3;
                            response.ErrorMessageList.Add(P_MENSAJE.Value.ToString());
                            success = false;
                        }

                        LogControl.save("InsertNewxCategoriaMatriz", "Paso X: Se ejecutó " + ProcedureName.pkg_Cotizacion + ".PD_PROCESA_NEW_RANGO", "2");
                    }
                    catch (Exception ex)
                    {
                        response.ErrorCode = 3;
                        response.ErrorMessageList.Add(ex.Message);
                        success = false;
                        LogControl.save("InsertNewxCategoriaMatriz", JsonConvert.SerializeObject(data), "2");
                        LogControl.save("InsertNewxCategoriaMatriz", ex.ToString(), "3");
                    }
                }

            // }

            return success;

        }

        // GCAA 24012024
        private bool InsertNewxCategoriaExcedente(DbConnection connection, DbTransaction trx, string quotationNumber, UpdateStatusQuotationBM data, ref GenericResponseVM response)
        {
            bool success = true;
            string storeprocedure = "PD_PROCESA_NEW_RANGO";

            for (int i = 0; i < data.tasasPlanillasPrimasCategoriaTrabajadorExcesoTopeRemuneraciones.Count(); i++)
            {
                if (data.tasasPlanillasPrimasCategoriaTrabajadorExcesoTopeRemuneraciones[i].descripcion == "Tasa Comercial Mensual")
                {
                    List<OracleParameter> parameter = new List<OracleParameter>();

                    if (success)
                    {
                        try
                        {
                            parameter.Add(new OracleParameter("P_TIPO", OracleDbType.Int32, 3, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.NroCotizacion, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_RANGO", OracleDbType.Varchar2, "", ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, data.tasasPlanillasPrimasCategoriaTrabajadorExcesoTopeRemuneraciones[i].codigoCategoria, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NTASA", OracleDbType.Double, data.tasasPlanillasPrimasCategoriaTrabajadorExcesoTopeRemuneraciones[i].valor, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("N_ORDER", OracleDbType.Double, response.Order, ParameterDirection.Input));

                            OracleParameter P_COD_ESTADO = new OracleParameter("P_COD_ESTADO", OracleDbType.Int32, ParameterDirection.Output);
                            OracleParameter P_MENSAJE = new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, ParameterDirection.Output);
                            OracleParameter P_ORDER = new OracleParameter("P_ORDER", OracleDbType.Int32, ParameterDirection.Output);

                            P_MENSAJE.Size = 4000;

                            parameter.Add(P_MENSAJE);
                            parameter.Add(P_COD_ESTADO);
                            parameter.Add(P_ORDER);

                            this.ExecuteByStoredProcedureVT_TRX(storeprocedure, parameter, connection, trx);

                            response.ErrorCode = Convert.ToInt32(P_COD_ESTADO.Value.ToString());
                            response.Order = Convert.ToInt32(P_ORDER.Value.ToString());


                            if (response.ErrorCode != 0)
                            {
                                response.ErrorCode = 3;
                                response.ErrorMessageList.Add(P_MENSAJE.Value.ToString());
                                success = false;
                            }

                            LogControl.save("InsertNewxCategoriaExcedente", "Paso X: Se ejecutó " + ProcedureName.pkg_Cotizacion + ".PD_PROCESA_NEW_RANGO", "2");
                        }
                        catch (Exception ex)
                        {
                            response.ErrorCode = 3;
                            response.ErrorMessageList.Add(ex.Message);
                            success = false;
                            LogControl.save("InsertNewxCategoriaExcedente", JsonConvert.SerializeObject(data), "2");
                            LogControl.save("InsertNewxCategoriaExcedente", ex.ToString(), "3");
                        }
                    }
                }
            }

            return success;

        }

        // GCAA 24012024
        private bool InsertDetalleCotizador_CAT(DbConnection connection, DbTransaction trx, string quotationNumber, UpdateStatusQuotationBM data, ref GenericResponseVM response)
        {
            bool success = true;
            string storeprocedure = "PD_PROCESA_DET_COTIZACION_CAT";

            List<OracleParameter> parameter = new List<OracleParameter>();

            if (success)
            {
                try
                {
                    parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.NroCotizacion, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_TIPO", OracleDbType.Int32, data.opcion, ParameterDirection.Input));
                    OracleParameter P_MENSAJE = new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, ParameterDirection.Output);
                    OracleParameter P_COD_ESTADO = new OracleParameter("P_COD_ESTADO", OracleDbType.Int32, ParameterDirection.Output);

                    P_MENSAJE.Size = 4000;

                    parameter.Add(P_MENSAJE);
                    parameter.Add(P_COD_ESTADO);

                    this.ExecuteByStoredProcedureVT_TRX(storeprocedure, parameter, connection, trx);

                    response.ErrorCode = Convert.ToInt32(P_COD_ESTADO.Value.ToString());

                    if (response.ErrorCode != 0)
                    {
                        response.ErrorCode = 3;
                        response.ErrorMessageList.Add(P_MENSAJE.Value.ToString());
                        success = false;
                    }

                    LogControl.save("InsertDetalleCotizador_CAT", "Paso X: Se ejecutó " + ProcedureName.pkg_Cotizacion + ".PD_PROCESA_DET_COTIZACION_CAT", "2");
                }
                catch (Exception ex)
                {
                    response.ErrorCode = 3;
                    response.ErrorMessageList.Add(ex.Message);
                    success = false;
                    LogControl.save("InsertDetalleCotizador_CAT", JsonConvert.SerializeObject(data), "2");
                    LogControl.save("InsertDetalleCotizador_CAT", ex.ToString(), "3");
                }
            }


            return success;

        }

        private bool InsertDetalleCotizador_CAT_MATRIZ(DbConnection connection, DbTransaction trx, string quotationNumber, UpdateStatusQuotationBM data, ref GenericResponseVM response)
        {
            bool success = true;
            string storeprocedure = "PD_PROCESA_DET_COTIZACION_CAT_MAT";


            List<OracleParameter> parameter = new List<OracleParameter>();

            if (success)
            {
                try
                {
                    parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.NroCotizacion, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_TIPO", OracleDbType.Int32, data.opcion, ParameterDirection.Input));

                    OracleParameter P_MENSAJE = new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, ParameterDirection.Output);
                    OracleParameter P_COD_ESTADO = new OracleParameter("P_COD_ESTADO", OracleDbType.Int32, ParameterDirection.Output);

                    P_MENSAJE.Size = 4000;

                    parameter.Add(P_MENSAJE);
                    parameter.Add(P_COD_ESTADO);

                    this.ExecuteByStoredProcedureVT_TRX(storeprocedure, parameter, connection, trx);

                    response.ErrorCode = Convert.ToInt32(P_COD_ESTADO.Value.ToString());

                    if (response.ErrorCode != 0)
                    {
                        response.ErrorCode = 3;
                        response.ErrorMessageList.Add(P_MENSAJE.Value.ToString());
                        success = false;
                    }

                    LogControl.save("InsertDetalleCotizador_CAT_MATRIZ", "Paso X: Se ejecutó " + ProcedureName.pkg_Cotizacion + ".PD_PROCESA_DET_COTIZACION_CAT_MAT", "2");
                }
                catch (Exception ex)
                {
                    response.ErrorCode = 3;
                    response.ErrorMessageList.Add(ex.Message);
                    success = false;
                    LogControl.save("InsertDetalleCotizador_CAT_MATRIZ", JsonConvert.SerializeObject(data), "2");
                    LogControl.save("InsertDetalleCotizador_CAT_MATRIZ", ex.ToString(), "3");
                }
            }


            return success;

        }

        // GCAA 15042024
        private bool UpdateEstadoCabecera(DbConnection connection, DbTransaction trx, string quotationNumber, UpdateStatusQuotationBM data, ref GenericResponseVM response)
        {
            bool success = true;
            string storeprocedure = "PD_UPDATE_ESTADO_COTIZADOR";

            List<OracleParameter> parameter = new List<OracleParameter>();

            if (success)
            {
                try
                {
                    parameter.Add(new OracleParameter("P_TIPO", OracleDbType.Int32, data.opcion, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.NroCotizacion, ParameterDirection.Input));

                    OracleParameter P_COD_ESTADO = new OracleParameter("P_COD_ESTADO", OracleDbType.Int32, ParameterDirection.Output);
                    OracleParameter P_MENSAJE = new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, ParameterDirection.Output);
                    P_MENSAJE.Size = 4000;

                    parameter.Add(P_MENSAJE);
                    parameter.Add(P_COD_ESTADO);

                    this.ExecuteByStoredProcedureVT_TRX(storeprocedure, parameter, connection, trx);

                    response.ErrorCode = Convert.ToInt32(P_COD_ESTADO.Value.ToString());

                    if (response.ErrorCode != 0)
                    {
                        response.ErrorCode = 3;
                        response.ErrorMessageList.Add(P_MENSAJE.Value.ToString());
                        success = false;
                    }

                    LogControl.save("UpdateEstadoCabecera", "Paso X: Se ejecutó " + ProcedureName.pkg_Cotizacion + ".PD_UPDATE_ESTADO_COTIZADOR", "2");
                }
                catch (Exception ex)
                {
                    response.ErrorCode = 3;
                    response.ErrorMessageList.Add(ex.Message);
                    success = false;
                    LogControl.save("UpdateEstadoCabecera", JsonConvert.SerializeObject(data), "2");
                    LogControl.save("UpdateEstadoCabecera", ex.ToString(), "3");
                }
            }
            return success;
        }

        #endregion

        public QuotationRequestDM ReadInfoQuotationDM(int nroCotizacion, int tipoEndoso)
        {
            QuotationRequestDM quotationRequestDM = new QuotationRequestDM();

            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.sp_LeerData;

            OracleDataReader dr = null;
            try
            {
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, nroCotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPENDOSO", OracleDbType.Int32, tipoEndoso, ParameterDirection.Input)); // JDD Endosos 20220930
                string[] arrayCursor = { "C_TABLE_COT", "C_TABLE_PLL_ACT", "C_TABLE_TASA_PR", "C_TABLE_TASA_PREVIEW" }; // JDD Endosos 20220930
                OracleParameter C_CAB = new OracleParameter(arrayCursor[0], OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter C_PLANILLA = new OracleParameter(arrayCursor[1], OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter C_TASAS = new OracleParameter(arrayCursor[2], OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter C_TASAS_PRE = new OracleParameter(arrayCursor[3], OracleDbType.RefCursor, ParameterDirection.Output); // JDD Endosos 20220930
                parameter.Add(C_CAB);
                parameter.Add(C_PLANILLA);
                parameter.Add(C_TASAS);
                parameter.Add(C_TASAS_PRE); // JDD Endosos 20220930

                dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storeprocedure, arrayCursor, parameter);

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        QuotationRequestDMHeader header = new QuotationRequestDMHeader();
                        header.NUM_COTIZACION = dr["NUM_COTIZACION"] == DBNull.Value ? 0 : Convert.ToInt32(dr["NUM_COTIZACION"].ToString());
                        //header.TAZA_PROPUESTA = dr["TASA_PROPUESTA"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["TASA_PROPUESTA"].ToString());
                        header.COD_BROKER = dr["COD_BROKER"] == DBNull.Value ? 0 : Convert.ToInt32(dr["COD_BROKER"].ToString());
                        header.DES_BROKER = dr["DES_BROKER"] == DBNull.Value ? "" : Convert.ToString(dr["DES_BROKER"].ToString());
                        header.COD_CONTRATANTE = dr["COD_CONTRATANTE"] == DBNull.Value ? "" : dr["COD_CONTRATANTE"].ToString();
                        header.DES_CONTRATANTE = dr["DES_CONTRATANTE"] == DBNull.Value ? "" : Convert.ToString(dr["DES_CONTRATANTE"].ToString());
                        header.COD_ACT_TEC = dr["COD_ACT_TEC"] == DBNull.Value ? "" : (dr["COD_ACT_TEC"].ToString());
                        header.DES_ACT_TEC = dr["DES_ACT_TEC"] == DBNull.Value ? "" : Convert.ToString(dr["DES_ACT_TEC"].ToString());
                        header.COD_ACT_TEC_ASOC = dr["COD_ACT_TEC_ASOC"] == DBNull.Value ? "" : (dr["COD_ACT_TEC_ASOC"].ToString());
                        header.DES_ACT_TEC_ASOC = dr["DES_ACT_TEC_ASOC"] == DBNull.Value ? "" : Convert.ToString(dr["DES_ACT_TEC_ASOC"].ToString());
                        header.INI_VIG = dr["INI_VIG"] == DBNull.Value ? new DateTime() : Convert.ToDateTime(dr["INI_VIG"].ToString());
                        header.FIN_VIG = dr["FIN_VIG"] == DBNull.Value ? new DateTime() : Convert.ToDateTime(dr["FIN_VIG"].ToString());
                        header.COD_PLAN = dr["COD_PLAN"] == DBNull.Value ? 0 : Convert.ToInt32(dr["COD_PLAN"].ToString());
                        header.COD_USUARIO = dr["COD_USUARIO"] == DBNull.Value ? 0 : Convert.ToInt32(dr["COD_USUARIO"].ToString());
                        header.FEC_ENVIO = dr["FEC_ENVIO"] == DBNull.Value ? new DateTime() : Convert.ToDateTime(dr["FEC_ENVIO"].ToString());
                        header.COMENTARIO = dr["COMENTARIO"] == DBNull.Value ? "" : Convert.ToString(dr["COMENTARIO"].ToString());
                        header.TIPO_COT = dr["TIPO_COT"] == DBNull.Value ? 0 : Convert.ToInt32(dr["TIPO_COT"].ToString());
                        header.PROC_TYPE = Convert.ToString(dr["PROC_TYPE"]);
                        header.DAY_ALLOWED = dr["DAY_ALLOWED"] == DBNull.Value ? null : dr["DAY_ALLOWED"];
                        header.TOPE_REM_EXC = dr["TOPE_REM_EXC"] == DBNull.Value ? 0 : Convert.ToDouble(dr["TOPE_REM_EXC"].ToString());
                        header.TOPE_REM_AGE = dr["TOPE_REM_AGE"] == DBNull.Value ? 0 : Convert.ToInt32(dr["TOPE_REM_AGE"].ToString());
                        header.DES_TIPO_COT = dr["DES_TIPO_COT"] == DBNull.Value ? "" : Convert.ToString(dr["DES_TIPO_COT"].ToString());
                        header.MES_FREQ_PAGO = dr["MES_FREQ_PAGO"] == DBNull.Value ? 0 : Convert.ToInt32(dr["MES_FREQ_PAGO"].ToString());
                        header.PRI_MIN = dr["PRI_MIN"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PRI_MIN"].ToString());
                        header.NUM_POLICY = dr["NUM_POLICY"] == DBNull.Value ? "" : dr["NUM_POLICY"].ToString();
                        header.PRI_PREV = dr["PRI_PREV"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PRI_PREV"].ToString());
                        header.TIPO_ENDOSO = dr["TIPO_ENDOSO"] == DBNull.Value ? "" : dr["TIPO_ENDOSO"].ToString();
                        header.NUM_TRAMITE = dr["NUM_TRAMITE"] == DBNull.Value ? 0 : Convert.ToInt32(dr["NUM_TRAMITE"].ToString());
                        header.SEGMENTO = dr["NCLIENT_SEG"] == DBNull.Value ? "" : dr["NCLIENT_SEG"].ToString();
                        header.SLA = dr["NTIEMPO_TOTAL_SLA"] == DBNull.Value ? "" : dr["NTIEMPO_TOTAL_SLA"].ToString();
                        header.RATE_TYPE = dr["RATE_TYPE"] == DBNull.Value ? 0 : Convert.ToInt32(dr["RATE_TYPE"].ToString()); //Tipo de tasa VL
                        header.PAYROLLAMOUNT = dr["PAYROLLAMOUNT"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PAYROLLAMOUNT"].ToString()); //EAER - Gestion Tramite
                        header.COMISION = dr["COMISION"] == DBNull.Value ? 0 : Convert.ToDouble(dr["COMISION"].ToString());
                        header.FRECUENCY_PRE = dr["FRECUENCY_PRE"] == DBNull.Value ? 0 : Convert.ToInt32(dr["FRECUENCY_PRE"].ToString()); // JDD Endosos 20220930
                        header.PREMIUM_MN_ENDOSO = dr["PREMIUM_MN_ENDOSO"] == DBNull.Value ? 0 : Convert.ToDouble(dr["PREMIUM_MN_ENDOSO"].ToString()); // JDD Endosos 20220930
                        quotationRequestDM.Header = header;
                    }

                    dr.NextResult();

                    while (dr.Read())
                    {
                        QuotationRequestDMDetail det = new QuotationRequestDMDetail();
                        //det.ID_REG = dr["ID_REG"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ID_REG"].ToString());
                        det.NOMBRE = dr["NOMBRE"] == DBNull.Value ? "" : Convert.ToString(dr["NOMBRE"].ToString());
                        det.APE_PAT = dr["APE_PAT"] == DBNull.Value ? "" : Convert.ToString(dr["APE_PAT"].ToString());
                        det.APE_NAT = dr["APE_NAT"] == DBNull.Value ? "" : Convert.ToString(dr["APE_NAT"].ToString());
                        det.TIPO_DOC = dr["TIPO_DOC"] == DBNull.Value ? "" : Convert.ToString(dr["TIPO_DOC"].ToString());
                        det.NUM_DOCUMENTO = dr["NUM_DOCUMENTO"] == DBNull.Value ? "" : Convert.ToString(dr["NUM_DOCUMENTO"].ToString());
                        det.FEC_NACIMIENTO = dr["FEC_NACIMIENTO"] == DBNull.Value ? new DateTime() : Convert.ToDateTime(dr["FEC_NACIMIENTO"].ToString());
                        det.GENERO = dr["GENERO"] == DBNull.Value ? "" : Convert.ToString(dr["GENERO"].ToString());
                        det.REMUNERACION = dr["REMUNERACION"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["REMUNERACION"].ToString());
                        det.TIPO_TRABAJADOR = dr["TIPO_TRABAJADOR"] == DBNull.Value ? 0 : Convert.ToInt32(dr["TIPO_TRABAJADOR"].ToString());
                        det.DES_TRABAJADOR = dr["DES_TRABAJADOR"] == DBNull.Value ? "" : Convert.ToString(dr["DES_TRABAJADOR"].ToString());
                        det.SEDE = dr["SEDE"] == DBNull.Value ? "" : Convert.ToString(dr["SEDE"].ToString());
                        det.PAIS_NACIMIENTO = dr["PAIS_NACIMIENTO"] == DBNull.Value ? "" : Convert.ToString(dr["PAIS_NACIMIENTO"].ToString());
                        det.STATUS = dr["STATUS"] == DBNull.Value ? "" : Convert.ToString(dr["STATUS"].ToString());
                        det.RANGO_EDAD = dr["RANGO_EDAD"] == DBNull.Value ? "" : Convert.ToString(dr["RANGO_EDAD"].ToString()); // GCAA 27/10/2023
                        quotationRequestDM.Detail.Add(det);
                    }

                    dr.NextResult();

                    while (dr.Read())
                    {
                        QuotationRequestDMDetailTasas dtTasa = new QuotationRequestDMDetailTasas();
                        dtTasa.DES_MODULEC = dr["DES_MODULEC"] == DBNull.Value ? "" : Convert.ToString(dr["DES_MODULEC"].ToString());
                        dtTasa.TASA_PROPUESTA = dr["TASA_PROPUESTA"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["TASA_PROPUESTA"].ToString());
                        dtTasa.RANGO_EDAD = dr["RANGO_EDAD"] == DBNull.Value ? "" : Convert.ToString(dr["RANGO_EDAD"].ToString()); // GCAA 27/10/2023
                        dtTasa.NTYPE_PROCESO = dr["TIPO_PROCESO"] == DBNull.Value ? 0 : Convert.ToInt32(dr["TIPO_PROCESO"].ToString()); // GCAA 20052024
                        dtTasa.CATEGORIA_ID = dr["CATEGORIAID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["CATEGORIAID"].ToString());// GCAA 20052024
                        quotationRequestDM.DetailTasas.Add(dtTasa);
                    }

                    dr.NextResult(); // JDD Endosos 20220930

                    while (dr.Read()) // JDD Endosos 20220930
                    {
                        QuotationRequestDMDetailTasasPreview dtTasaPreview = new QuotationRequestDMDetailTasasPreview();
                        dtTasaPreview.CATEGORY = dr["COD_CATEGORY"] == DBNull.Value ? String.Empty : Convert.ToString(dr["COD_CATEGORY"].ToString());
                        dtTasaPreview.TOP_BONUS = dr["TOP_BONUS"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["TOP_BONUS"].ToString());
                        dtTasaPreview.TOP_RATE = dr["TOP_RATE"] == DBNull.Value ? 0 : Convert.ToDouble(dr["TOP_RATE"].ToString());
                        dtTasaPreview.EXTRA_BONUS = dr["EXTRA_BONUS"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["EXTRA_BONUS"].ToString());
                        dtTasaPreview.TOP_REM = dr["TOP_REM"] == DBNull.Value ? 0 : Convert.ToDouble(dr["TOP_REM"].ToString());
                        dtTasaPreview.EXTRA_RATE = dr["EXTRA_RATE"] == DBNull.Value ? 0 : Convert.ToDouble(dr["EXTRA_RATE"].ToString());
                        dtTasaPreview.EXTRA_REM = dr["EXTRA_REM"] == DBNull.Value ? 0 : Convert.ToDouble(dr["EXTRA_REM"].ToString());

                        quotationRequestDM.DetailTasasPreview.Add(dtTasaPreview);
                    }
                }
            }
            catch (Exception ex)
            {
                LogControl.save("ReadInfoQuotationDM", ex.ToString(), "3");
            }
            finally
            {
                ELog.CloseConnection(dr);
            }

            return quotationRequestDM;
        }

        public List<QuotationCoverVM> GetQuotationCoverByNumQuotation(long nroCotizacion)
        {
            List<QuotationCoverVM> covers = new List<QuotationCoverVM>();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.sp_InfoCotCover;

            OracleDataReader dr = null;
            try
            {
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, nroCotizacion, ParameterDirection.Input));
                string[] arrayCursor = { "C_TABLE" };
                OracleParameter C_TABLE = new OracleParameter(arrayCursor[0], OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storeprocedure, arrayCursor, parameter);

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        QuotationCoverVM coverVM = new QuotationCoverVM();
                        coverVM.COD_COVER = dr["COD_COVER"] == DBNull.Value ? 0 : Convert.ToInt32(dr["COD_COVER"].ToString());
                        coverVM.DES_COVER = dr["DES_COVER"] == DBNull.Value ? "" : Convert.ToString(dr["DES_COVER"].ToString());
                        coverVM.RMA = dr["RMA"] == DBNull.Value ? "0" : Convert.ToString(dr["RMA"].ToString());
                        coverVM.CAPITAL_MAX = dr["CAPITAL_MAX"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["CAPITAL_MAX"].ToString());

                        covers.Add(coverVM);
                    }
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetQuotationCoverByNumQuotation", ex.ToString(), "3");
            }
            finally
            {
                ELog.CloseConnection(dr);
            }

            return covers;
        }

        public GenericResponseVM DeleteQuotationCover(int idCotizacion)
        {
            var response = new GenericResponseVM();

            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        OracleDataReader dr;

                        cmd.Connection = cn;
                        cmd.CommandText = ProcedureName.pkg_Cotizacion + ".DEL_COTIZACION_COVER";
                        cmd.CommandType = CommandType.StoredProcedure;


                        cmd.Parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, idCotizacion, ParameterDirection.Input));

                        var P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                        var P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, ParameterDirection.Output);

                        P_MESSAGE.Size = 200;

                        cmd.Parameters.Add(P_MESSAGE);
                        cmd.Parameters.Add(P_COD_ERR);

                        cn.Open();
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        response.MessageError = P_MESSAGE.Value.ToString();
                        response.ErrorCode = 0; // Convert.ToInt32(P_COD_ERR.Value.ToString());

                        dr.Close();
                    }
                    catch (Exception ex)
                    {
                        response.MessageError = ex.ToString();
                        response.ErrorCode = 1;
                        LogControl.save("DeleteQuotationCover", "Paso 1: Error => " + Environment.NewLine + ex.ToString(), "3");
                    }
                    finally
                    {
                        if (cn.State == ConnectionState.Open) cn.Close();
                    }
                }
            }

            //List<OracleParameter> parameter = new List<OracleParameter>();
            //GenericResponseVM response = new GenericResponseVM();
            //DbConnection connection = ConnectionGet(enuTypeDataBase.OracleVTime);
            //try
            //{
            //    connection.Open();

            //    DbCommand command = connection.CreateCommand();
            //    command.CommandType = CommandType.StoredProcedure;
            //    command.CommandText = ProcedureName.pkg_Cotizacion + ".DEL_COTIZACION_COVER";

            //    //INPUT
            //    command.Parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, idCotizacion, ParameterDirection.Input));
            //    //OUTPUT
            //    command.Parameters.Add(new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, null, ParameterDirection.Output));
            //    command.Parameters.Add(new OracleParameter("P_COD_ERR", OracleDbType.Int32, null, ParameterDirection.Output));

            //    command.ExecuteNonQuery();

            //    response.MessageError = command.Parameters["P_MESSAGE"].Value.ToString();
            //    //response.ErrorCode = Convert.ToInt32(command.Parameters["P_COD_ERR"].Value.ToString());
            //    ELog.CloseConnection(connection);
            //    command.Dispose();
            //}
            //catch (Exception ex)
            //{
            //    ELog.CloseConnection(connection);
            //    response.MessageError = ex.ToString();
            //    ELogWS.save("Paso 1: Error => " + Environment.NewLine + ex.ToString());

            //}
            return response;
        }

        public string GetCodeuserByUsername(string username)
        {
            var usercode = string.Empty;

            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        OracleDataReader dr;

                        cmd.Connection = cn;
                        cmd.CommandText = ProcedureName.sp_ValidaUsuario;
                        cmd.CommandType = CommandType.StoredProcedure;


                        cmd.Parameters.Add(new OracleParameter("P_SUSERNAME", OracleDbType.Varchar2, username, ParameterDirection.Input));

                        var P_SUSERCODE = new OracleParameter("P_SUSERCODE", OracleDbType.Varchar2, null, ParameterDirection.Output);
                        P_SUSERCODE.Size = 100;

                        cmd.Parameters.Add(P_SUSERCODE);

                        cn.Open();
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        usercode = P_SUSERCODE.Value.ToString();

                        dr.Close();
                    }
                    catch (Exception ex)
                    {
                        usercode = string.Empty;
                        LogControl.save("GetCodeuserByUsername", "Paso 1: Error => " + Environment.NewLine + ex.ToString(), "3");
                    }
                    finally
                    {
                        if (cn.State == ConnectionState.Open) cn.Close();
                    }
                }
            }

            //var storedProcedure = ProcedureName.sp_ValidaUsuario;
            //var usercode = string.Empty;
            //List<OracleParameter> parameter = new List<OracleParameter>();

            //try
            //{
            //    //INPUT
            //    parameter.Add(new OracleParameter("P_SUSERNAME", OracleDbType.Varchar2, username, ParameterDirection.Input));

            //    //OUTPUT
            //    OracleParameter P_SUSERCODE = new OracleParameter("P_SUSERCODE", OracleDbType.Varchar2, null, ParameterDirection.Output);
            //    P_SUSERCODE.Size = 20;

            //    parameter.Add(P_SUSERCODE);

            //    this.ExecuteByStoredProcedureVT(storedProcedure, parameter);
            //    usercode = P_SUSERCODE.Value.ToString();
            //}
            //catch (Exception ex)
            //{
            //    ELog.save(this, ex);
            //}

            return usercode;
        }

        public QuotationByPolicyVM GetQuotationByPolicy(int npolicy)
        {
            QuotationByPolicyVM response = new QuotationByPolicyVM();

            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.sp_InfoCotPoliza;

            OracleDataReader dr = null;

            try
            {
                parameter.Add(new OracleParameter("P_NID_POLICY", OracleDbType.Int32, npolicy, ParameterDirection.Input));
                string[] arrayCursor = { "C_TABLE_COT", "C_TABLE_COT_DET", "C_TABLE_COT_COMER" };
                OracleParameter C_CAB = new OracleParameter(arrayCursor[0], OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter C_DET = new OracleParameter(arrayCursor[1], OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter C_COMER = new OracleParameter(arrayCursor[2], OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_CAB);
                parameter.Add(C_DET);
                parameter.Add(C_COMER);

                dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storeprocedure, arrayCursor, parameter);

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        QuotationByPolicyHeaderVM quotationByPolicyHeaderVM = new QuotationByPolicyHeaderVM();
                        quotationByPolicyHeaderVM.P_SCLIENT = dr["P_SCLIENT"] == DBNull.Value ? string.Empty : dr["P_SCLIENT"].ToString();
                        quotationByPolicyHeaderVM.P_NCURRENCY = dr["P_NCURRENCY"] == DBNull.Value ? string.Empty : dr["P_NCURRENCY"].ToString();
                        quotationByPolicyHeaderVM.P_NBRANCH = dr["P_NBRANCH"] == DBNull.Value ? string.Empty : dr["P_NBRANCH"].ToString();
                        quotationByPolicyHeaderVM.P_DSTARTDATE = dr["P_DSTARTDATE"] == DBNull.Value ? string.Empty : dr["P_DSTARTDATE"].ToString();
                        quotationByPolicyHeaderVM.P_DEXPIRDAT = dr["P_DEXPIRDAT"] == DBNull.Value ? string.Empty : dr["P_DEXPIRDAT"].ToString();
                        quotationByPolicyHeaderVM.P_NIDCLIENTLOCATION = dr["P_NIDCLIENTLOCATION"] == DBNull.Value ? string.Empty : dr["P_NIDCLIENTLOCATION"].ToString();
                        quotationByPolicyHeaderVM.P_SCOMMENT = dr["P_SCOMMENT"] == DBNull.Value ? string.Empty : dr["P_SCOMMENT"].ToString();
                        quotationByPolicyHeaderVM.P_SRUTA = dr["P_SRUTA"] == DBNull.Value ? string.Empty : dr["P_SRUTA"].ToString();
                        quotationByPolicyHeaderVM.P_NUSERCODE = dr["P_NUSERCODE"] == DBNull.Value ? string.Empty : dr["P_NUSERCODE"].ToString();
                        quotationByPolicyHeaderVM.P_NACT_MINA = dr["P_NACT_MINA"] == DBNull.Value ? string.Empty : dr["P_NACT_MINA"].ToString();
                        quotationByPolicyHeaderVM.P_NTIP_RENOV = dr["P_NTIP_RENOV"] == DBNull.Value ? string.Empty : dr["P_NTIP_RENOV"].ToString();
                        quotationByPolicyHeaderVM.P_NPAYFREQ = dr["P_NPAYFREQ"] == DBNull.Value ? string.Empty : dr["P_NPAYFREQ"].ToString();
                        quotationByPolicyHeaderVM.P_SCOD_ACTIVITY_TEC = dr["P_SCOD_ACTIVITY_TEC"] == DBNull.Value ? string.Empty : dr["P_SCOD_ACTIVITY_TEC"].ToString();
                        quotationByPolicyHeaderVM.P_SCOD_CIUU = dr["P_SCOD_CIUU"] == DBNull.Value ? string.Empty : dr["P_SCOD_CIUU"].ToString();
                        quotationByPolicyHeaderVM.P_NTIP_NCOMISSION = dr["P_NTIP_NCOMISSION"] == DBNull.Value ? string.Empty : dr["P_NTIP_NCOMISSION"].ToString();
                        quotationByPolicyHeaderVM.P_NPRODUCT = dr["P_NPRODUCT"] == DBNull.Value ? string.Empty : dr["P_NPRODUCT"].ToString();
                        quotationByPolicyHeaderVM.P_NCOMISION_SAL_PR = dr["P_NCOMISION_SAL_PR"] == DBNull.Value ? string.Empty : dr["P_NCOMISION_SAL_PR"].ToString();
                        quotationByPolicyHeaderVM.P_DSTARTDATE_ASE = dr["P_DSTARTDATE_ASE"] == DBNull.Value ? string.Empty : dr["P_DSTARTDATE_ASE"].ToString();
                        quotationByPolicyHeaderVM.P_DEXPIRDAT_ASE = dr["P_DEXPIRDAT_ASE"] == DBNull.Value ? string.Empty : dr["P_DEXPIRDAT_ASE"].ToString();
                        quotationByPolicyHeaderVM.P_NCOMPANY_LNK = dr["P_NCOMPANY_LNK"] == DBNull.Value ? string.Empty : dr["P_NCOMPANY_LNK"].ToString();
                        quotationByPolicyHeaderVM.P_SCOTIZA_LNK = dr["P_SCOTIZA_LNK"] == DBNull.Value ? string.Empty : dr["P_SCOTIZA_LNK"].ToString();
                        quotationByPolicyHeaderVM.P_NIDPLAN = dr["P_NIDPLAN"] == DBNull.Value ? string.Empty : dr["P_NIDPLAN"].ToString();

                        response.Header = quotationByPolicyHeaderVM;
                    }

                    dr.NextResult();

                    while (dr.Read())
                    {
                        QuotationByPolicyDetailVM quotationByPolicyDetailVM = new QuotationByPolicyDetailVM();
                        quotationByPolicyDetailVM.P_NPRODUCT = dr["P_NPRODUCT"] == DBNull.Value ? string.Empty : dr["P_NPRODUCT"].ToString();
                        quotationByPolicyDetailVM.P_NMODULEC = dr["P_NMODULEC"] == DBNull.Value ? string.Empty : dr["P_NMODULEC"].ToString();
                        quotationByPolicyDetailVM.P_NTOTAL_TRABAJADORES = dr["P_NTOTAL_TRABAJADORES"] == DBNull.Value ? string.Empty : dr["P_NTOTAL_TRABAJADORES"].ToString();
                        quotationByPolicyDetailVM.P_NMONTO_PLANILLA = dr["P_NMONTO_PLANILLA"] == DBNull.Value ? string.Empty : dr["P_NMONTO_PLANILLA"].ToString();
                        quotationByPolicyDetailVM.P_NTASA_CALCULADA = dr["P_NTASA_CALCULADA"] == DBNull.Value ? string.Empty : dr["P_NTASA_CALCULADA"].ToString();
                        quotationByPolicyDetailVM.P_NTASA_PROP = dr["P_NTASA_PROP"] == DBNull.Value ? string.Empty : dr["P_NTASA_PROP"].ToString();
                        quotationByPolicyDetailVM.P_NPREMIUM_MENSUAL = dr["P_NPREMIUM_MENSUAL"] == DBNull.Value ? string.Empty : dr["P_NPREMIUM_MENSUAL"].ToString();
                        quotationByPolicyDetailVM.P_NPREMIUM_MIN = dr["P_NPREMIUM_MIN"] == DBNull.Value ? string.Empty : dr["P_NPREMIUM_MIN"].ToString();
                        quotationByPolicyDetailVM.P_NPREMIUM_MIN_PR = dr["P_NPREMIUM_MIN_PR"] == DBNull.Value ? string.Empty : dr["P_NPREMIUM_MIN_PR"].ToString();
                        quotationByPolicyDetailVM.P_NPREMIUM_END = dr["P_NPREMIUM_END"] == DBNull.Value ? string.Empty : dr["P_NPREMIUM_END"].ToString();
                        quotationByPolicyDetailVM.P_NSUM_PREMIUMN = dr["P_NSUM_PREMIUMN"] == DBNull.Value ? string.Empty : dr["P_NSUM_PREMIUMN"].ToString();
                        quotationByPolicyDetailVM.P_NSUM_IGV = dr["P_NSUM_IGV"] == DBNull.Value ? string.Empty : dr["P_NSUM_IGV"].ToString();
                        quotationByPolicyDetailVM.P_NSUM_PREMIUM = dr["P_NSUM_PREMIUM"] == DBNull.Value ? string.Empty : dr["P_NSUM_PREMIUM"].ToString();
                        quotationByPolicyDetailVM.P_NUSERCODE = dr["P_NUSERCODE"] == DBNull.Value ? string.Empty : dr["P_NUSERCODE"].ToString();
                        quotationByPolicyDetailVM.P_NRATE = dr["P_NRATE"] == DBNull.Value ? string.Empty : dr["P_NRATE"].ToString();
                        quotationByPolicyDetailVM.P_NDISCOUNT = dr["P_NDISCOUNT"] == DBNull.Value ? string.Empty : dr["P_NDISCOUNT"].ToString();
                        quotationByPolicyDetailVM.P_NACTIVITYVARIACTION = dr["P_NACTIVITYVARIACTION"] == DBNull.Value ? string.Empty : dr["P_NACTIVITYVARIACTION"].ToString();
                        quotationByPolicyDetailVM.P_DEFFECDATE = dr["P_DEFFECDATE"] == DBNull.Value ? string.Empty : dr["P_DEFFECDATE"].ToString();
                        quotationByPolicyDetailVM.P_FLAG = dr["P_FLAG"] == DBNull.Value ? string.Empty : dr["P_FLAG"].ToString();

                        response.Detail.Add(quotationByPolicyDetailVM);
                    }
                    dr.NextResult();

                    while (dr.Read())
                    {
                        QuotationByPolicyComerVM quotationByPolicyComerVM = new QuotationByPolicyComerVM();
                        quotationByPolicyComerVM.P_NID_COTIZACION = dr["P_NID_COTIZACION"] == DBNull.Value ? string.Empty : dr["P_NID_COTIZACION"].ToString();
                        quotationByPolicyComerVM.P_NIDTYPECHANNEL = dr["P_NIDTYPECHANNEL"] == DBNull.Value ? string.Empty : dr["P_NIDTYPECHANNEL"].ToString();
                        quotationByPolicyComerVM.P_NINTERMED = dr["P_NINTERMED"] == DBNull.Value ? string.Empty : dr["P_NINTERMED"].ToString();
                        quotationByPolicyComerVM.P_SCLIENT_COMER = dr["P_SCLIENT_COMER"] == DBNull.Value ? string.Empty : dr["P_SCLIENT_COMER"].ToString();
                        quotationByPolicyComerVM.P_NCOMISION_SAL = dr["P_NCOMISION_SAL"] == DBNull.Value ? string.Empty : dr["P_NCOMISION_SAL"].ToString();
                        quotationByPolicyComerVM.P_NCOMISION_SAL_PR = dr["P_NCOMISION_SAL_PR"] == DBNull.Value ? string.Empty : dr["P_NCOMISION_SAL_PR"].ToString();
                        quotationByPolicyComerVM.P_NCOMISION_PEN = dr["P_NCOMISION_PEN"] == DBNull.Value ? string.Empty : dr["P_NCOMISION_PEN"].ToString();
                        quotationByPolicyComerVM.P_NCOMISION_PEN_PR = dr["P_NCOMISION_PEN_PR"] == DBNull.Value ? string.Empty : dr["P_NCOMISION_PEN_PR"].ToString();
                        quotationByPolicyComerVM.P_NPRINCIPAL = dr["P_NPRINCIPAL"] == DBNull.Value ? string.Empty : dr["P_NPRINCIPAL"].ToString();
                        quotationByPolicyComerVM.P_NUSERCODE = dr["P_NUSERCODE"] == DBNull.Value ? string.Empty : dr["P_NUSERCODE"].ToString();
                        quotationByPolicyComerVM.P_NSHARE = dr["P_NSHARE"] == DBNull.Value ? string.Empty : dr["P_NSHARE"].ToString();

                        response.DetailComer.Add(quotationByPolicyComerVM);
                    }
                }
                ELog.CloseConnection(dr);
            }
            catch (Exception ex)
            {
                ELog.CloseConnection(dr);
                LogControl.save("GetQuotationByPolicy", ex.ToString(), "3");
            }

            return response;
        }

        private GenericResponseVM UpdateStatusQuotation(DbConnection connection, DbTransaction trx, UpdateStatusQuotationBM quotation)
        {
            var response = new GenericResponseVM();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.pkg_Cotizacion + ".UPD_COTIZA_HIS";

            try
            {
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, quotation.NroCotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DDATERETRO", OracleDbType.Date, Convert.ToDateTime(quotation.FechaRetroactividad), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TIPO_PROC", OracleDbType.Int32, quotation.TipoProceso, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, quotation.UsuarioAprobador, ParameterDirection.Input));

                OracleParameter P_FLAG_DERIVA = new OracleParameter("P_FLAG_DERIVA", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_STRANSAC = new OracleParameter("P_STRANSAC", OracleDbType.Varchar2, ParameterDirection.Output);
                OracleParameter P_COD_ESTADO = new OracleParameter("P_COD_ESTADO", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_MENSAJE = new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, ParameterDirection.Output);

                P_STRANSAC.Size = 4000;
                P_MENSAJE.Size = 4000;

                parameter.Add(P_FLAG_DERIVA);
                parameter.Add(P_STRANSAC);
                parameter.Add(P_COD_ESTADO);
                parameter.Add(P_MENSAJE);

                this.ExecuteByStoredProcedureVT_TRX(storeprocedure, parameter, connection, trx);

                response.ErrorCode = Convert.ToInt32(P_COD_ESTADO.Value.ToString());
                response.ErrorMessageList.Add(P_MENSAJE.Value.ToString());
                response.GenericFlag = Convert.ToInt32(P_FLAG_DERIVA.Value.ToString());
                response.Transaction = Convert.ToString(P_STRANSAC.Value.ToString());

                LogControl.save("UpdateStatusQuotation", "Paso X: Se ejecutó " + ProcedureName.pkg_Cotizacion + ".UPD_COTIZA_HIS", "2");
            }
            catch (Exception ex)
            {
                response.ErrorCode = 3;
                response.ErrorMessageList.Add(ex.Message);
                LogControl.save("UpdateStatusQuotation", JsonConvert.SerializeObject(quotation), "2");
                LogControl.save("UpdateStatusQuotation", ex.ToString(), "3");
            }

            return response;

            //GenericResponseVM response
            //var response = new GenericResponseVM();
            //response.ErrorMessageList = new List<string>();
            //try
            //{
            //    DbCommand command = connection.CreateCommand();
            //    command.CommandType = CommandType.StoredProcedure;
            //    command.CommandText = ProcedureName.pkg_Cotizacion + ".UPD_COTIZA_HIS";
            //    command.Transaction = transaction;

            //    command.Parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, quotation.NroCotizacion, ParameterDirection.Input));
            //    command.Parameters.Add(new OracleParameter("P_DDATERETRO", OracleDbType.Date, Convert.ToDateTime(quotation.FechaRetroactividad), ParameterDirection.Input));
            //    command.Parameters.Add(new OracleParameter("P_TIPO_PROC", OracleDbType.Int32, quotation.TipoProceso, ParameterDirection.Input));
            //    command.Parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, quotation.UsuarioAprobador, ParameterDirection.Input));

            //    command.Parameters.Add(new OracleParameter("P_FLAG_DERIVA", OracleDbType.Int32, 4000, null, ParameterDirection.Output));
            //    command.Parameters.Add(new OracleParameter("P_STRANSAC", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output));
            //    command.Parameters.Add(new OracleParameter("P_COD_ESTADO", OracleDbType.Int32, 4000, null, ParameterDirection.Output));
            //    command.Parameters.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output));
            //    command.ExecuteNonQuery();

            //    response.ErrorCode = Convert.ToInt32(command.Parameters["P_COD_ESTADO"].Value.ToString());
            //    response.ErrorMessageList.Add(command.Parameters["P_MENSAJE"].Value.ToString());
            //    response.GenericFlag = Convert.ToInt32(command.Parameters["P_FLAG_DERIVA"].Value.ToString());
            //    response.Transaction = Convert.ToString(command.Parameters["P_STRANSAC"].Value.ToString());
            //}
            //catch (Exception ex)
            //{
            //    response.ErrorCode = 3;
            //    response.ErrorMessageList.Add(ex.Message);

            //    ELogWS.save(ex.ToString());
            //}

            //return response;
        }
        // end - marcos silverio

        /*Excel*/
        public string GetExcelQuotationList(QuotationSearchBM data)
        {
            GenericResponseVM response = this.GetQuotationList(data);
            List<QuotationVM> lista = (List<QuotationVM>)response.GenericResponse;
            string templatePath = string.Empty;
            if (lista[0].NFLAG_EXTERNO == 1)
            {
                templatePath = "D:/doc_templates/reportes/dev/Template_request_status-ext.xlsx";
            }
            else
            {
                templatePath = "D:/doc_templates/reportes/dev/Template_request_status.xlsx";
            }
            string plantilla = "";
            try
            {
                MemoryStream ms = new MemoryStream();
                using (FileStream fs = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
                {
                    fs.CopyTo(ms);
                }
                using (SLDocument sl = new SLDocument(ms))
                {
                    int i = 6;
                    int letra = 65;

                    sl.SetCellValue("B1", data.StartDate?.ToString("dd/MM/yyyy"));
                    sl.SetCellValue("B2", data.EndDate?.ToString("dd/MM/yyyy"));

                    foreach (QuotationVM item in lista)
                    {
                        string fecha = "";
                        try
                        {
                            fecha = item.ApprovalDate == null ? "" : Convert.ToDateTime(item.ApprovalDate).ToString("dd/MM/yyyy hh:mm");
                        }
                        catch (Exception)
                        {
                            fecha = "";
                        }
                        int c = 0;
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.QuotationNumber);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.PolicyNumber);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.ProductName);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.ContractorFullName);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.MinimalPremium);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.WorkersCount);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.Payroll);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.BrokerFullName);
                        if (item.NFLAG_EXTERNO == 0)
                        {
                            sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.SDESCRIPT_SEG);
                            sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.NTIEMPO_TOTAL_SLA);
                        }
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.TypeTransac);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.Status);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, fecha);
                        i++;
                    }
                    using (MemoryStream ms2 = new MemoryStream())
                    {
                        sl.SaveAs(ms2);
                        plantilla = Convert.ToBase64String(ms2.ToArray(), 0, ms2.ToArray().Length);
                    }
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetExcelQuotationList", ex.ToString(), "3");
                plantilla = "";
                throw ex;
            }
            return plantilla;
        }
        public QuotationResponseVM UpdateFechaEfectoAsegurado(QuotationCabBM request)
        {
            var sPackageName = ProcedureName.pkg_Cotizacion + ".UPD_EFFECDATE_ASE";
            List<OracleParameter> parameter = new List<OracleParameter>();
            QuotationResponseVM result = new QuotationResponseVM();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, request.P_NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, request.P_NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STYPE_TRANSAC", OracleDbType.Varchar2, request.TrxCode, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, request.NumeroCotizacion, ParameterDirection.Input));
                //parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, request.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Date, request.P_DSTARTDATE_ASE, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, result.P_NCODE, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, result.P_SMESSAGE, ParameterDirection.Output);

                P_SMESSAGE.Size = 9000;

                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);

                this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                result.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                result.P_SMESSAGE = P_SMESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                result.P_NCODE = 4;
                result.P_SMESSAGE = ex.ToString();
                LogControl.save("UpdateFechaEfectoAsegurado", ex.ToString(), "3");
            }
            return result;
        }

        public SalidaErrorBaseVM ValidateTramaEndosoVL(ValidaTramaBM validaTramaBM)
        {
            SalidaErrorBaseVM response = new SalidaErrorBaseVM();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.pkg_ValidadorGenPD + ".SP_VAL_TRAMA_COT_VL_ENDOSO";
            string SCERTYPE = "2";
            string STRANSAC = "EM";
            //string storeprocedure = ProcedureName.sp_ValidaTramaEndosoVL;

            try
            {
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, validaTramaBM.NID_PROC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Varchar2, validaTramaBM.NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, validaTramaBM.NID_COTIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTIPO_ENDOSO", OracleDbType.Int32, validaTramaBM.TYPE_ENDOSO, ParameterDirection.Input));//tipo endoso
                parameter.Add(new OracleParameter("P_SCERTYPE", OracleDbType.Varchar2, SCERTYPE, ParameterDirection.Input)); // preguntar por el origen del valor
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, validaTramaBM.P_NBRANCH, ParameterDirection.Input)); //valida llave config // o nbranch - preguntar
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, validaTramaBM.P_NPRODUCT, ParameterDirection.Input));  //valida llave config
                parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Varchar2, validaTramaBM.P_NPOLICY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCURRENCY", OracleDbType.Int32, validaTramaBM.P_NCURRENCY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STRANSAC", OracleDbType.Varchar2, STRANSAC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Date, validaTramaBM.DEFFECDATE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NREM_EXC", OracleDbType.Double, validaTramaBM.P_NREM_EXC, ParameterDirection.Input));

                //parameter.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Varchar2, validaTramaBM.NTYPE_TRANSAC, ParameterDirection.Input));                

                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                string[] arrayCursor = { "C_TABLE", "C_TABLE_ASEG" };
                OracleParameter C_TABLE = new OracleParameter(arrayCursor[0], OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter C_TABLE_ASEG = new OracleParameter(arrayCursor[1], OracleDbType.RefCursor, ParameterDirection.Output);

                P_COD_ERR.Size = 9000;
                P_MESSAGE.Size = 9000;

                parameter.Add(P_COD_ERR);
                parameter.Add(P_MESSAGE);
                parameter.Add(C_TABLE);
                parameter.Add(C_TABLE_ASEG);

                OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVTCursores(storeprocedure, arrayCursor, parameter);

                try
                {
                    while (dr.Read())
                    {
                        ErroresVM item = new ErroresVM();
                        item.REGISTRO = dr["REGISTRO"] == DBNull.Value ? "" : dr["REGISTRO"].ToString();
                        item.DESCRIPCION = dr["DESCRIPCION"] == DBNull.Value ? "" : dr["DESCRIPCION"].ToString();
                        response.errorList.Add(item);
                    }
                }
                catch (Exception) { }

                dr.NextResult();

                try
                {
                    while (dr.Read())
                    {
                        ChangesVM item = new ChangesVM();
                        item.NID_COTIZACION = dr["NID_COTIZACION"] == DBNull.Value ? "" : dr["NID_COTIZACION"].ToString();
                        item.NID_PROC = dr["NID_PROC"] == DBNull.Value ? "" : dr["NID_PROC"].ToString();
                        item.NIDDOC_TYPE = dr["NIDDOC_TYPE"] == DBNull.Value ? "" : dr["NIDDOC_TYPE"].ToString();
                        item.SIDDOC = dr["SIDDOC"] == DBNull.Value ? "" : dr["SIDDOC"].ToString();
                        item.SFIRSTNAME = dr["SFIRSTNAME"] == DBNull.Value ? "" : dr["SFIRSTNAME"].ToString();
                        item.SLASTNAME = dr["SLASTNAME"] == DBNull.Value ? "" : dr["SLASTNAME"].ToString();
                        item.SLASTNAME2 = dr["SLASTNAME2"] == DBNull.Value ? "" : dr["SLASTNAME2"].ToString();
                        item.SSEXCLIEN = dr["SSEXCLIEN"] == DBNull.Value ? "" : dr["SSEXCLIEN"].ToString();
                        item.SE_MAIL = dr["SE_MAIL"] == DBNull.Value ? "" : dr["SE_MAIL"].ToString();
                        item.SPHONE = dr["SPHONE"] == DBNull.Value ? "" : dr["SPHONE"].ToString();
                        item.DBIRTHDAT = dr["DBIRTHDAT"] == DBNull.Value ? "" : dr["DBIRTHDAT"].ToString();
                        item.NREMUNERACION = dr["NREMUNERACION"] == DBNull.Value ? "" : dr["NREMUNERACION"].ToString();
                        item.SFIRSTNAME_NEW = dr["SFIRSTNAME_NEW"] == DBNull.Value ? "" : dr["SFIRSTNAME_NEW"].ToString();
                        item.SLASTNAME_NEW = dr["SLASTNAME_NEW"] == DBNull.Value ? "" : dr["SLASTNAME_NEW"].ToString();
                        item.SLASTNAME2_NEW = dr["SLASTNAME2_NEW"] == DBNull.Value ? "" : dr["SLASTNAME2_NEW"].ToString();
                        item.SSEXCLIEN_NEW = dr["SSEXCLIEN_NEW"] == DBNull.Value ? "" : dr["SSEXCLIEN_NEW"].ToString();
                        item.SE_MAIL_NEW = dr["SE_MAIL_NEW"] == DBNull.Value ? "" : dr["SE_MAIL_NEW"].ToString();
                        item.SPHONE_NEW = dr["SPHONE_NEW"] == DBNull.Value ? "" : dr["SPHONE_NEW"].ToString();
                        item.DBIRTHDAT_NEW = dr["DBIRTHDAT_NEW"] == DBNull.Value ? "" : dr["DBIRTHDAT_NEW"].ToString();
                        item.NREMUNERACION_NEW = dr["NREMUNERACION_NEW"] == DBNull.Value ? "" : dr["NREMUNERACION_NEW"].ToString();
                        response.changeList.Add(item);
                    }
                }
                catch (Exception) { }

                response.P_FLAG_EXC = validaTramaBM.P_NREM_EXC; // ENDOSO TECNICA JTV 18042023

                response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                response.P_MESSAGE = P_MESSAGE.Value.ToString();
                ELog.CloseConnection(dr);
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = ex.ToString();
                LogControl.save("ValidateTramaEndosoVL", ex.ToString(), "3");
            }

            return response;
        }

        /// <summary>
        /// <para>v1. calcula las diferencias de planillas para endosos de remuneracion</para>
        /// <para>v2. [23/02/2023] calcula las planillas, las primas por categoria, la prima comercial y actualiza las diferencias en la tabla tbl_pd_carga_mas_sctr</para>
        /// </summary>
        /// <param name="validaTramaBM">Objetos con los datos asociados a la transaccion</param>
        /// <param name="obj">Salida de cursores</param>
        /// <returns></returns>
        public SalidaTramaBaseVM readInfoTramaEndoso(ValidaTramaBM validaTramaBM, ref SalidaTramaBaseVM obj) // INI ENDOSO TECNICA JTV 22022023
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.sp_DetalleCotizacionEndoso;
            OracleDataReader dr = null;

            try
            {
                string json = JsonConvert.SerializeObject(validaTramaBM.P_JTASAS, Formatting.None).ToString();

                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, validaTramaBM.NID_PROC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, validaTramaBM.NID_COTIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, validaTramaBM.P_NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, validaTramaBM.P_NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Decimal, validaTramaBM.P_NPOLICY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Date, validaTramaBM.DEFFECDATE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, validaTramaBM.P_DEXPIRDAT, ParameterDirection.Input));

                string[] arrayCursor = { "C_CATEGORIA", "C_DET_PLAN", "C_AMOUNT_PRIMA", "C_AMOUNT_DET_TOTAL" };
                OracleParameter C_CATEGORIA = new OracleParameter(arrayCursor[0], OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter C_DET_PLAN = new OracleParameter(arrayCursor[1], OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter C_AMOUNT_PRIMA = new OracleParameter(arrayCursor[2], OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter C_AMOUNT_DET_TOTAL = new OracleParameter(arrayCursor[3], OracleDbType.RefCursor, ParameterDirection.Output);

                parameter.Add(C_CATEGORIA);
                parameter.Add(C_DET_PLAN);
                parameter.Add(C_AMOUNT_PRIMA);
                parameter.Add(C_AMOUNT_DET_TOTAL);

                OracleParameter P_NERROR = new OracleParameter("P_NERROR", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                P_NERROR.Size = 3;
                P_MESSAGE.Size = 9000;

                parameter.Add(P_NERROR);
                parameter.Add(P_MESSAGE);

                dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storeprocedure, arrayCursor, parameter);

                obj.P_COD_ERR = P_NERROR.Value.ToString(); // ENDOSOS TECNICA JTV 23022023
                obj.P_MESSAGE = P_MESSAGE.Value.ToString(); // ENDOSOS TECNICA JTV 23022023

                if (obj.P_COD_ERR == "0") // ENDOSOS TECNICA JTV 23022023
                {
                    /*if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            CategoryQuotationVM item = new CategoryQuotationVM();
                            item.SCATEGORIA = dr["CATEGORIA"] == DBNull.Value ? "" : dr["CATEGORIA"].ToString();
                            item.NCOUNT = dr["NCOUNT"] == DBNull.Value ? 0 : int.Parse(dr["NCOUNT"].ToString());
                            item.NTOTAL_PLANILLA = dr["TOTAL_PLANILLA"] == DBNull.Value ? 0.0 : double.Parse(dr["TOTAL_PLANILLA"].ToString());
                            obj.categoryList.Add(item);
                        }

                        dr.NextResult();

                        while (dr.Read())
                        {
                            DetailPlanQuotationVM item = new DetailPlanQuotationVM();
                            item.COD_PLAN = dr["COD_PLAN"] == DBNull.Value ? 0 : int.Parse(dr["COD_PLAN"].ToString());
                            item.DET_PLAN = dr["DET_PLAN"] == DBNull.Value ? "" : dr["DET_PLAN"].ToString();
                            obj.detailPlanList.Add(item);
                        }

                        dr.NextResult();

                        while (dr.Read())
                        {
                            AmountPremiumQuotationVM item = new AmountPremiumQuotationVM();
                            item.SCATEGORIA = dr["CATEGORIA"] == DBNull.Value ? "" : dr["CATEGORIA"].ToString();
                            item.NTASA = dr["TASA"] == DBNull.Value ? 0.0 : double.Parse(dr["TASA"].ToString());
                            item.NPREMIUMN_TOT = dr["PRIMA"] == DBNull.Value ? 0.0 : double.Parse(dr["PRIMA"].ToString());
                            obj.amountPremiumList.Add(item);
                        }

                        dr.NextResult();

                        while (dr.Read())
                        {
                            AmountDetailTotalQuotationVM item = new AmountDetailTotalQuotationVM();
                            item.NORDER = dr["NORDER"] == DBNull.Value ? "" : dr["NORDER"].ToString();
                            item.SDESCRIPCION = dr["SDESCRIPCION"] == DBNull.Value ? "" : dr["SDESCRIPCION"].ToString();
                            item.NAMOUNT_TOT = dr["NAMOUNT"] == DBNull.Value ? 0.0 : double.Parse(dr["NAMOUNT"].ToString());
                            obj.amountDetailTotalList.Add(item);
                        }// FIN ENDOSOS TECNICA JTV 22022023
                    }*/

                    if (dr.HasRows)
                    {
                        string SCATE = "";
                        for (int f = 0; f < 4; f++)
                        {
                            while (dr.Read())
                            {
                                switch (f)
                                {
                                    case 0:

                                        CategoryQuotationVM item1 = new CategoryQuotationVM();
                                        item1.SCATEGORIA = dr["CATEGORIA"] == DBNull.Value ? "" : dr["CATEGORIA"].ToString();
                                        item1.SCATEGORIA_view = (dr["CATEGORIA"].ToString() == SCATE) ? "" : dr["CATEGORIA"].ToString(); // GCAA 22122023
                                        item1.SRANGO_EDAD = dr["SEDADES"] == DBNull.Value ? "" : dr["SEDADES"].ToString();
                                        item1.NCOUNT = dr["NCOUNT"] == DBNull.Value ? 0 : int.Parse(dr["NCOUNT"].ToString());
                                        item1.NTOTAL_PLANILLA = dr["TOTAL_PLANILLA"] == DBNull.Value ? 0.0 : double.Parse(dr["TOTAL_PLANILLA"].ToString());
                                        SCATE = dr["CATEGORIA"] == DBNull.Value ? "" : dr["CATEGORIA"].ToString();
                                        obj.categoryList.Add(item1);
                                        break;
                                    case 1:
                                        DetailPlanQuotationVM item2 = new DetailPlanQuotationVM();
                                        item2.COD_PLAN = dr["COD_PLAN"] == DBNull.Value ? 0 : int.Parse(dr["COD_PLAN"].ToString());
                                        item2.DET_PLAN = dr["DET_PLAN"] == DBNull.Value ? "" : dr["DET_PLAN"].ToString();
                                        obj.detailPlanList.Add(item2);

                                        break;
                                    case 2:
                                        AmountPremiumQuotationVM item3 = new AmountPremiumQuotationVM();
                                        item3.SCATEGORIA = dr["CATEGORIA"] == DBNull.Value ? "" : dr["CATEGORIA"].ToString();
                                        item3.SRANGO_EDAD = dr["SEDADES"] == DBNull.Value ? "" : dr["SEDADES"].ToString();
                                        item3.NTASA = dr["TASA"] == DBNull.Value ? 0.0 : double.Parse(dr["TASA"].ToString());
                                        item3.NPREMIUMN_TOT = dr["PRIMA"] == DBNull.Value ? 0.0 : double.Parse(dr["PRIMA"].ToString());
                                        obj.amountPremiumList.Add(item3);

                                        break;
                                    case 3:
                                        AmountDetailTotalQuotationVM item4 = new AmountDetailTotalQuotationVM();
                                        item4.NORDER = dr["NORDER"] == DBNull.Value ? "" : dr["NORDER"].ToString();
                                        item4.SDESCRIPCION = dr["SDESCRIPCION"] == DBNull.Value ? "" : dr["SDESCRIPCION"].ToString();
                                        item4.NAMOUNT_TOT = dr["NAMOUNT"] == DBNull.Value ? 0.0 : double.Parse(dr["NAMOUNT"].ToString());
                                        obj.amountDetailTotalList.Add(item4);

                                        break;
                                }
                            }

                            dr.NextResult();
                        }
                    }

                } // ENDOSOS TECNICA JTV 23022023
            }
            catch (OracleException ex)
            {
                obj.P_COD_ERR = "1"; // ENDOSOS TECNICA JTV 23022023
                obj.P_MESSAGE = ex.ToString(); // ENDOSOS TECNICA JTV 23022023
                LogControl.save("readInfoTramaEndoso", ex.ToString(), "3");
            }
            finally
            {
                ELog.CloseConnection(dr);
            }

            return obj;
        }

        /*public SalidaTramaBaseVM readInfoTramaDetailEndoso(ValidaTramaBM validaTramaBM, ref SalidaTramaBaseVM obj) // INI ENDOSO TECNICA JTV 22022023
        {
            //SalidaTramaBaseVM response = new SalidaTramaBaseVM();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.sp_DetalleMontosCotEndosos;

            try
            {
                //string json = JsonConvert.SerializeObject(validaTramaBM.P_JTASAS, Formatting.None).ToString();
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, validaTramaBM.NID_PROC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, validaTramaBM.NID_COTIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DATE_TRANSAC", OracleDbType.Date, new DateTime(), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Date, validaTramaBM.DEFFECDATE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, validaTramaBM.P_DEXPIRDAT, ParameterDirection.Input));

                //OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, ParameterDirection.Output);
                string[] arrayCursor = { "C_AMOUNT_PRIMA", "C_AMOUNT_DET_TOTAL" };

                OracleParameter C_AMOUNT_PRIMA = new OracleParameter(arrayCursor[0], OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter C_AMOUNT_DET_TOTAL = new OracleParameter(arrayCursor[1], OracleDbType.RefCursor, ParameterDirection.Output);

                parameter.Add(C_AMOUNT_PRIMA);
                parameter.Add(C_AMOUNT_DET_TOTAL);

                parameter.Add(new OracleParameter("P_NTYPE_END", OracleDbType.Int32, validaTramaBM.TYPE_ENDOSO, ParameterDirection.Input));

                OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storeprocedure, arrayCursor, parameter);

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        AmountPremiumQuotationVM item = new AmountPremiumQuotationVM();
                        item.SCATEGORIA = dr["CATEGORIA"] == DBNull.Value ? "" : dr["CATEGORIA"].ToString();
                        item.NTASA = dr["TASA"] == DBNull.Value ? 0.0 : double.Parse(dr["TASA"].ToString());
                        item.NPREMIUMN_TOT = dr["PRIMA"] == DBNull.Value ? 0.0 : double.Parse(dr["PRIMA"].ToString());
                        obj.amountPremiumList.Add(item);
                    }
                    dr.NextResult();
                    while (dr.Read())
                    {
                        AmountDetailTotalQuotationVM item = new AmountDetailTotalQuotationVM();
                        item.NORDER = dr["NORDER"] == DBNull.Value ? "" : dr["NORDER"].ToString();
                        item.SDESCRIPCION = dr["SDESCRIPCION"] == DBNull.Value ? "" : dr["SDESCRIPCION"].ToString();
                        item.NAMOUNT_TOT = dr["NAMOUNT"] == DBNull.Value ? 0.0 : double.Parse(dr["NAMOUNT"].ToString());
                        obj.amountDetailTotalList.Add(item);
                    }
                }
                ELog.CloseConnection(dr);

            }
            catch (Exception ex)
            {
                ELog.save(this, ex.ToString());
            }

            return obj;
        }*/ // FIN ENDOSO TECNICA JTV 22022023

        private GenericResponseVM PD_REM_EXC_AUT(DbConnection connection, DbTransaction trx, UpdateStatusQuotationBM quotation)
        {
            //GenericResponseVM response
            var response = new GenericResponseVM();
            //response.ErrorMessageList = new List<string>();
            var storeprocedure = ProcedureName.sp_RemuneracionExclusion;
            List<OracleParameter> parameter = new List<OracleParameter>();

            try
            {

                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, quotation.NroCotizacion, ParameterDirection.Input));

                OracleParameter P_MENSAJE = new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, ParameterDirection.Output);

                P_MENSAJE.Size = 4000;

                this.ExecuteByStoredProcedureVT_TRX(storeprocedure, parameter, connection, trx);

                response.ErrorCode = 0;

                LogControl.save("PD_REM_EXC_AUT", "Paso X: Se ejecutó " + ProcedureName.sp_RemuneracionExclusion, "1");
            }
            catch (Exception ex)
            {
                response.ErrorCode = 3;
                response.ErrorMessageList.Add(ex.Message);
                LogControl.save("PD_REM_EXC_AUT", ex.ToString(), "3");
            }

            //try
            //{
            //    DbCommand command = connection.CreateCommand();
            //    command.CommandType = CommandType.StoredProcedure;
            //    command.CommandText = ProcedureName.sp_RemuneracionExclusion;
            //    command.Transaction = transaction;

            //    command.Parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, quotation.NroCotizacion, ParameterDirection.Input));
            //    command.ExecuteNonQuery();
            //}
            //catch (Exception ex)
            //{
            //    response.ErrorCode = 3;
            //    response.ErrorMessageList.Add(ex.Message);

            //    ELogWS.save(ex.ToString());
            //}

            return response;
        }
        public GenericResponseVM getFechaFin(string fecha, int freq)
        {
            var response = new GenericResponseVM();

            if (!string.IsNullOrEmpty(fecha) && fecha != "NaN/NaN/NaN" && fecha != "null")
            {
                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        try
                        {
                            OracleDataReader dr;

                            cmd.Connection = cn;
                            cmd.CommandText = ProcedureName.sp_GenerarFechaFin;
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Date, Convert.ToDateTime(fecha), ParameterDirection.Input));
                            cmd.Parameters.Add(new OracleParameter("NPAYFREQ", OracleDbType.Int32, freq, ParameterDirection.Input));

                            var P_DEXPIRDAT = new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, ParameterDirection.Output);

                            //P_SOPERATIONNUMBER.Size = 200;

                            cmd.Parameters.Add(P_DEXPIRDAT);

                            cn.Open();
                            dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                            var fechaFin = P_DEXPIRDAT.Value.ToString() != "null" ? P_DEXPIRDAT.Value.ToString() : String.Empty;
                            response.FechaExp = !String.IsNullOrEmpty(fechaFin) ? Convert.ToDateTime(fechaFin).ToString("MM/dd/yyyy") : String.Empty;

                            dr.Close();
                        }
                        catch (Exception ex)
                        {
                            response.FechaExp = string.Empty;
                            LogControl.save("getFechaFin", ex.ToString() + Environment.NewLine + "fecha: " + fecha + " - freq: " + freq, "3");
                        }
                        finally
                        {
                            if (cn.State == ConnectionState.Open) cn.Close();
                        }
                    }
                }
            }

            //var sPackageName = ProcedureName.sp_GenerarFechaFin;
            //List<OracleParameter> parameter = new List<OracleParameter>();
            //DateTime fechaFin = new DateTime();
            //try
            //{
            //    if (!string.IsNullOrEmpty(fecha) && fecha != "NaN/NaN/NaN")
            //    {
            //        //INPUT
            //        parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Date, Convert.ToDateTime(fecha), ParameterDirection.Input));
            //        parameter.Add(new OracleParameter("NPAYFREQ", OracleDbType.Int32, freq, ParameterDirection.Input));

            //        //OUTPUT
            //        OracleParameter P_DEXPIRDAT = new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, null, ParameterDirection.Output);

            //        parameter.Add(P_DEXPIRDAT);
            //        this.ExecuteByStoredProcedureVT(sPackageName, parameter);
            //        fechaFin = Convert.ToDateTime(P_DEXPIRDAT.Value.ToString());

            //        response.FechaExp = fechaFin.ToString("MM/dd/yyyy");
            //    }
            //    else
            //    {
            //        response.FechaExp = DateTime.Now.ToString("MM/dd/yyyy");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    response.FechaExp = string.Empty;
            //}
            return response;
        }

        public void InsertLog(long cotizacion, string proceso, string url, string parametros, string resultados)
        {
            var sPackageName = ProcedureName.sp_InsertarLog;
            List<OracleParameter> parameter = new List<OracleParameter>();
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int64, cotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOD_PROCESS", OracleDbType.Varchar2, proceso, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SURL", OracleDbType.Varchar2, url, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SJSON", OracleDbType.Clob, parametros, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SRESULT", OracleDbType.Varchar2, resultados, ParameterDirection.Input));

                this.ExecuteByStoredProcedureVT(sPackageName, parameter);
            }
            catch (Exception ex)
            {
                LogControl.save("InsertLog", ex.ToString(), "3");
            }
        }
        // JDLC INSERT COBERTURA
        public GenericResponseVM UpdateStatusCotPD(CoverBM data, DataTable dtCover)
        {
            GenericResponseVM response = new GenericResponseVM();
            response.ErrorMessageList = new List<string>();

            SalidaTramaBaseVM resultBulkInsert = new SalidaTramaBaseVM();
            if (dtCover != null)
            {
                resultBulkInsert = this.SaveUsingOracleBulkCopy(dtCover, ProcedureName.tbl_CotizacionCover);
                if (resultBulkInsert.P_COD_ERR == "1")
                {
                    response.ErrorCode = Convert.ToInt32(resultBulkInsert.P_COD_ERR);
                    response.ErrorMessageList.Add(resultBulkInsert.P_MESSAGE);

                    return response;
                }

                LogControl.save("UpdateStatusCotPD", "Paso 2: Se registraron coberturas en TBL_PD_COTIZACION_COVER.", "1");
            }

            return response;
        }

        // JDLC INSERT ASISTENCIAS
        public GenericResponseVM UpdateStatusCotPD(InsertAssitsBM data, DataTable dtCover)
        {
            GenericResponseVM response = new GenericResponseVM();
            response.ErrorMessageList = new List<string>();

            SalidaTramaBaseVM resultBulkInsert = new SalidaTramaBaseVM();
            if (dtCover != null)
            {
                resultBulkInsert = this.SaveUsingOracleBulkCopy(dtCover, ProcedureName.tbl_CotizacionAsistencias);
                if (resultBulkInsert.P_COD_ERR == "1")
                {
                    response.ErrorCode = Convert.ToInt32(resultBulkInsert.P_COD_ERR);
                    response.ErrorMessageList.Add(resultBulkInsert.P_MESSAGE);

                    return response;
                }

                LogControl.save("UpdateStatusCotPD", "Paso 2: Se registraron coberturas en TBL_PD_COTIZACION_COVER.", "1");
            }

            return response;
        }

        // JDLC INSERT BENEFICIOS
        public GenericResponseVM UpdateStatusCotPD(InsertBenefitBM data, DataTable dtCover)
        {
            GenericResponseVM response = new GenericResponseVM();
            response.ErrorMessageList = new List<string>();

            SalidaTramaBaseVM resultBulkInsert = new SalidaTramaBaseVM();
            if (dtCover != null)
            {
                resultBulkInsert = this.SaveUsingOracleBulkCopy(dtCover, ProcedureName.tbl_CotizacionBeneficios);
                if (resultBulkInsert.P_COD_ERR == "1")
                {
                    response.ErrorCode = Convert.ToInt32(resultBulkInsert.P_COD_ERR);
                    response.ErrorMessageList.Add(resultBulkInsert.P_MESSAGE);

                    return response;
                }

                LogControl.save("UpdateStatusCotPD", "Paso 2: Se registraron coberturas en TBL_PD_COTIZACION_COVER.", "1");
            }

            return response;
        }

        public EmitPolVM transferDataPM(string idproc)
        {
            var sPackageName = ProcedureName.pkg_ValidadorGenPD + ".SP_GEN_DAT_PM_AP";

            List<OracleParameter> parameter = new List<OracleParameter>();
            var result = new EmitPolVM();
            DbConnection DataConnection = ConnectionGet(enuTypeDataBase.OracleVTime);
            DbTransaction trx = null;
            try
            {
                DataConnection.Open();
                trx = DataConnection.BeginTransaction();

                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, idproc, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_COD_ERR = new OracleParameter("P_ERROR", OracleDbType.Int32, result.cod_error, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, result.message, ParameterDirection.Output);

                P_COD_ERR.Size = 9000;
                P_MESSAGE.Size = 9000;

                parameter.Add(P_COD_ERR);
                parameter.Add(P_MESSAGE);

                this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, DataConnection, trx);

                result.cod_error = Convert.ToInt32(P_COD_ERR.Value.ToString());
                result.message = P_MESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                result.cod_error = 1;
                result.message = ex.ToString();
            }
            finally
            {
                if (result.cod_error == 0)
                {
                    trx.Commit();
                }
                else
                {
                    if (trx.Connection != null) trx.Rollback();
                }
                if (trx.Connection != null) trx.Dispose();
                ELog.CloseConnection(DataConnection);
            }

            return result;
        }

        // Registrar Detalle Cotizador
        //public SalidaErrorBaseVM regDetallePlanCot(validaTramaVM dataDetalle, Entities.PolicyModel.ViewModel.GenericPackageVM dataQuotation)
        //{
        //    SalidaErrorBaseVM response = new SalidaErrorBaseVM();
        //    DbConnection DataConnection = ConnectionGet(enuTypeDataBase.OracleVTime);
        //    DbTransaction trx = null;
        //    if (DataConnection.State == ConnectionState.Open)
        //    {
        //        DataConnection.Close();
        //    }

        //    //var generic = (Entities.PolicyModel.ViewModel.PolizaEmitCAB)dataQuotation.GenericResponse;
        //    //var dataDetalle = new validaTramaVM();

        //    try
        //    {
        //        DataConnection.Open();
        //        trx = DataConnection.BeginTransaction();                

        //        response = eliminarCoberturasCotTrx(dataDetalle, DataConnection, trx);
        //        response = response.P_COD_ERR == 0 ? eliminarAsistenciasCotTrx(dataDetalle, DataConnection, trx) : response;
        //        response = response.P_COD_ERR == 0 ? eliminarBeneficiosCotTrx(dataDetalle, DataConnection, trx) : response;
        //        response = response.P_COD_ERR == 0 ? eliminarReajustesCotTrx(dataDetalle, DataConnection, trx) : response;
        //        response = response.P_COD_ERR == 0 ? eliminarRecargosCotTrx(dataDetalle, DataConnection, trx) : response;

        //        if (response.P_COD_ERR == 0)
        //        {
        //            //int flagcov = data.flagCalcular == 1 ? 2 : 1;
        //            if (data.coberturasList != null)
        //            {
        //                foreach (var cobertura in data.coberturasList)
        //                {
        //                    response = response.P_COD_ERR == 0 ? regCoberturasCotTrx(dataDetalle, cobertura, DataConnection, trx) : response;
        //                }
        //            }
        //        }

        //        if (response.P_COD_ERR == 0)
        //        {
        //            if (data.asistenciasList != null)
        //            {
        //                foreach (var asistencia in data.asistenciasList)
        //                {
        //                    response = response.P_COD_ERR == 0 ? regAsistenciasCotTrx(dataDetalle, asistencia, DataConnection, trx) : response;
        //                }
        //            }
        //        }

        //        if (response.P_COD_ERR == 0)
        //        {
        //            if (data.beneficiosList != null)
        //            {
        //                foreach (var beneficio in data.beneficiosList)
        //                {
        //                    response = response.P_COD_ERR == 0 ? regBeneficiosCotTrx(dataDetalle, beneficio, DataConnection, trx) : response;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.P_COD_ERR = 1;
        //        response.P_MESSAGE = ex.ToString();
        //        ELog.save(this, ex.ToString());
        //    }
        //    finally
        //    {
        //        if (response.P_COD_ERR == 0)
        //        {
        //            trx.Commit();
        //        }
        //        else
        //        {
        //            if (trx.Connection !=null) trx.Rollback();
        //        }

        //        trx.Dispose();
        //        DataConnection.Close();

        //        if (response.P_COD_ERR == 0)
        //        {
        //            response = genericInsertData(dataDetalle, data);
        //        }
        //    }
        //    return response;
        //}

        public SalidaTramaBaseVM updatePrimaCotizador(updateCotizador data)
        {
            var sPackageName = ProcedureName.pkg_Cotizacion + ".UPD_COTIZACION_DET_PD";

            List<OracleParameter> parameter = new List<OracleParameter>();
            var response = new SalidaTramaBaseVM();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, data.codProceso, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int64, data.nroCotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.codRamo, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, data.codProducto, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, data.codUsuario, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_ESTADO", OracleDbType.Int32, data.codEstado, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPOLIZA_MATRIZ", OracleDbType.Int32, data.polMatriz, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Date, data.fechaEfecto, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NRATE", OracleDbType.Double, data.rate, ParameterDirection.Input)); //AVS - RENTAS
                parameter.Add(new OracleParameter("P_SDETAIL_ID", OracleDbType.Varchar2, data.detailId, ParameterDirection.Input)); //AVS - RENTAS

                //OUTPUT
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, response.P_MESSAGE, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, response.P_COD_ERR, ParameterDirection.Output);

                P_MESSAGE.Size = 9000;
                P_COD_ERR.Size = 2000;

                parameter.Add(P_MESSAGE);
                parameter.Add(P_COD_ERR);

                this.ExecuteByStoredProcedureVT(sPackageName, parameter);

                response.P_COD_ERR = P_COD_ERR.Value.ToString();
                response.P_MESSAGE = P_MESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                response.P_MESSAGE = ex.ToString();
                response.P_COD_ERR = "1";
                LogControl.save("updatePrimaCotizador", ex.ToString(), "3");
            }

            return response;
        }

        public validaTramaVM getInfoQuotationTransac(validaTramaVM request)
        {
            var response = request;
            response.lcoberturas = new List<coberturaPropuesta>();
            response.lbeneficios = new List<beneficioPropuesto>();
            response.lasistencias = new List<asistenciaPropuesta>();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.pkg_ReaDataAP + ".REA_ALL_AP";

            OracleDataReader dr = null;
            try
            {
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, request.datosPoliza.branch, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, request.datosPoliza.codTipoProducto, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, ELog.obtainConfig("planMaestro" + request.datosPoliza.branch), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int64, request.datosPoliza.nid_cotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_PRODUCT", OracleDbType.Int32, request.datosPoliza.codTipoNegocio, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_PROFILE", OracleDbType.Int32, request.datosPoliza.codTipoPerfil, ParameterDirection.Input));
                string[] arrayCursor = { "C_CURSOR1", "C_CURSOR2", "C_CURSOR3", "C_CURSOR4", "C_CURSOR5" };
                OracleParameter C_CURSOR1 = new OracleParameter(arrayCursor[0], OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter C_CURSOR2 = new OracleParameter(arrayCursor[1], OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter C_CURSOR3 = new OracleParameter(arrayCursor[2], OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter C_CURSOR4 = new OracleParameter(arrayCursor[3], OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter C_CURSOR5 = new OracleParameter(arrayCursor[3], OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_CURSOR1);
                parameter.Add(C_CURSOR2);
                parameter.Add(C_CURSOR3);
                parameter.Add(C_CURSOR4);
                parameter.Add(C_CURSOR5);

                dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storeprocedure, arrayCursor, parameter);

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        if (dr["NCOVERGEN"] != DBNull.Value)
                        {
                            var item = new coberturaPropuesta();
                            item.codCobertura = dr["NCOVERGEN"].ToString();
                            item.sumaPropuesta = Convert.ToDouble(dr["NCAPITAL"].ToString());
                            response.lcoberturas.Add(item);
                        }
                    }

                    dr.NextResult();

                    while (dr.Read())
                    {
                        if (dr["NCOD_BENEFIT"] != DBNull.Value)
                        {
                            var item = new beneficioPropuesto();
                            item.codBeneficio = dr["NCOD_BENEFIT"].ToString();
                            response.lbeneficios.Add(item);
                        }
                    }

                    dr.NextResult();

                    while (dr.Read())
                    {
                        if (dr["NCOD_ASSISTANCE"] != DBNull.Value)
                        {
                            var item = new asistenciaPropuesta();
                            item.codAsistencia = dr["NCOD_ASSISTANCE"].ToString();
                            response.lasistencias.Add(item);
                        }
                    }

                    dr.NextResult();

                    while (dr.Read())
                    {
                        if (dr["NCOD_SURCHARGE"] != DBNull.Value)
                        {
                            var item = new recargoPropuesto();
                            item.codRecargo = dr["NCOD_SURCHARGE"].ToString();
                            response.lrecargos.Add(item);
                        }
                    }

                    dr.NextResult();

                    while (dr.Read())
                    {
                        if (dr["NCOD_ADDITIONAL_SERVICES"] != DBNull.Value)
                        {
                            var item = new servicioAdicionalPropuesto();
                            item.codServAdicionales = dr["NCOD_ADDITIONAL_SERVICES"].ToString();
                            item.amount = Convert.ToDouble(dr["NHOUR"].ToString());
                            response.lservAdicionales.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogControl.save("getInfoQuotationTransac", ex.ToString(), "3");
            }
            finally
            {
                ELog.CloseConnection(dr);

                //if (dr != null)
                //    dr.Close();
            }

            return response;
        }

        public SalidaTramaBaseVM InsertReajustes(validaTramaVM data, ReadjustmentFactor readjustment)
        {
            var response = new SalidaTramaBaseVM();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.pkg_ReaDataAP + ".INS_ADJUSTMENT_FACTOR_COT";

            try
            {
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, data.codProceso, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.datosPoliza.branch, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, data.datosPoliza.codTipoProducto, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, data.datosPoliza.codTipoPlan, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCOD_FACTOR", OracleDbType.Int32, readjustment.code, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SDESC_FACTOR", OracleDbType.Varchar2, readjustment.description, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPERCENT", OracleDbType.Double, readjustment.rate, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NAMOUNT", OracleDbType.Double, readjustment.value, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Varchar2, data.datosPoliza.InicioVigPoliza, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Varchar2, data.datosPoliza.FinVigPoliza, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int64, data.codUsuario, ParameterDirection.Input));

                OracleParameter P_COD_ERR = new OracleParameter("P_ERROR", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                P_COD_ERR.Size = 9000;
                P_MESSAGE.Size = 9000;

                parameter.Add(P_COD_ERR);
                parameter.Add(P_MESSAGE);

                this.ExecuteByStoredProcedureVT(storeprocedure, parameter);

                response.P_COD_ERR = P_COD_ERR.Value.ToString();
                response.P_MESSAGE = P_MESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = "1";
                response.P_MESSAGE = ex.ToString();
                LogControl.save("InsertReajustes", ex.ToString(), "3");
            }

            return response;
        }

        public SalidaTramaBaseVM InsertRecargos(validaTramaVM data, List<SurchargeTariff> surcharges)
        {
            var response = new SalidaTramaBaseVM();

            if (surcharges != null && surcharges.Count > 0)
            {
                try
                {
                    var dataTable = generateDataTableSurcharges(surcharges, data);
                    response = this.SaveUsingOracleBulkCopy(dataTable, ProcedureName.tbl_CotizacionRecargos);
                }
                catch (Exception ex)
                {
                    response.P_COD_ERR = "1";
                    response.P_MESSAGE = ex.ToString();
                    LogControl.save("InsertRecargos", ex.ToString(), "3");
                }
            }
            else
            {
                response.P_COD_ERR = "0";
            }


            //using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
            //{
            //    using (OracleCommand cmd = new OracleCommand())
            //    {
            //        try
            //        {
            //            OracleDataReader dr;

            //            cmd.Connection = cn;
            //            cmd.CommandText = ProcedureName.pkg_ReaDataAP + ".INS_SURCHARGE_COT";
            //            cmd.CommandType = CommandType.StoredProcedure;

            //            cmd.Parameters.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, data.codProceso, ParameterDirection.Input));
            //            cmd.Parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.datosPoliza.branch, ParameterDirection.Input));
            //            cmd.Parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, data.datosPoliza.codTipoProducto, ParameterDirection.Input));
            //            cmd.Parameters.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, ELog.obtainConfig("planMaestro" + data.datosPoliza.branch), ParameterDirection.Input));
            //            cmd.Parameters.Add(new OracleParameter("P_NCOD_SURCHARGE", OracleDbType.Varchar2, surcharge.code, ParameterDirection.Input));
            //            cmd.Parameters.Add(new OracleParameter("P_SDESC_SURCHARGE", OracleDbType.Varchar2, surcharge.description, ParameterDirection.Input));
            //            cmd.Parameters.Add(new OracleParameter("P_NPERCENT", OracleDbType.Double, surcharge.value.amount, ParameterDirection.Input));
            //            cmd.Parameters.Add(new OracleParameter("P_NAMOUNT", OracleDbType.Double, surcharge.value.amount, ParameterDirection.Input));
            //            cmd.Parameters.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Varchar2, data.datosPoliza.InicioVigPoliza, ParameterDirection.Input));
            //            cmd.Parameters.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Varchar2, data.datosPoliza.FinVigPoliza, ParameterDirection.Input));
            //            cmd.Parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int64, data.codUsuario, ParameterDirection.Input));
            //            cmd.Parameters.Add(new OracleParameter("P_NSTATREGT", OracleDbType.Int32, Convert.ToBoolean(surcharge.optional) ? 1 : 0, ParameterDirection.Input));

            //            var P_COD_ERR = new OracleParameter("P_ERROR", OracleDbType.Int32, ParameterDirection.Output);
            //            var P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

            //            P_MESSAGE.Size = 4000;

            //            cmd.Parameters.Add(P_COD_ERR);
            //            cmd.Parameters.Add(P_MESSAGE);

            //            cmd.Parameters.Add(new OracleParameter("P_SSURCHARGEUSE", OracleDbType.Int32, Convert.ToBoolean(surcharge.optional) ? 1 : 0, ParameterDirection.Input));

            //            cn.Open();
            //            dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            //            response.P_COD_ERR = P_COD_ERR.Value.ToString();
            //            response.P_MESSAGE = P_MESSAGE.Value.ToString();

            //            dr.Close();
            //        }
            //        catch (Exception ex)
            //        {
            //            response.P_COD_ERR = "1";
            //            response.P_MESSAGE = ex.ToString();
            //            ELog.save(this, ex.ToString());
            //        }
            //        finally
            //        {
            //            if (cn.State == ConnectionState.Open) cn.Close();
            //        }
            //    }
            //}

            return response;
        }

        private DataTable generateDataTableSurcharges(List<SurchargeTariff> surcharges, validaTramaVM data)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("NID_COTIZACION", typeof(int));
            dt.Columns.Add("NID_PROC");
            dt.Columns.Add("NBRANCH", typeof(int));
            dt.Columns.Add("NPRODUCT", typeof(int));
            dt.Columns.Add("NMODULEC", typeof(int));
            dt.Columns.Add("NCOD_SURCHARGE");
            dt.Columns.Add("SDESC_SURCHARGE");
            dt.Columns.Add("NPERCENT", typeof(decimal));
            dt.Columns.Add("NAMOUNT", typeof(decimal));
            dt.Columns.Add("DEFFECDATE", typeof(DateTime));
            dt.Columns.Add("DEXPIRDAT", typeof(DateTime));
            dt.Columns.Add("DNULLDATE", typeof(DateTime));
            dt.Columns.Add("DCOMPDATE", typeof(DateTime));
            dt.Columns.Add("NUSERCODE", typeof(int));
            dt.Columns.Add("NSTATREGT", typeof(int));
            dt.Columns.Add("SSURCHARGEUSE");
            dt.Columns.Add("NIGV", typeof(int));

            foreach (var surcharge in surcharges)
            {
                DataRow dr = dt.NewRow();
                dr["NID_COTIZACION"] = 0;
                dr["NID_PROC"] = data.codProceso;
                dr["NBRANCH"] = data.datosPoliza.branch;
                dr["NPRODUCT"] = data.datosPoliza.codTipoProducto;
                dr["NMODULEC"] = ELog.obtainConfig("planMaestro" + data.datosPoliza.branch);
                dr["NCOD_SURCHARGE"] = surcharge.id;
                dr["SDESC_SURCHARGE"] = surcharge.description;
                dr["NPERCENT"] = surcharge.value.amount;
                dr["NAMOUNT"] = surcharge.value.amount;
                dr["DEFFECDATE"] = Convert.ToDateTime(data.datosPoliza.InicioVigPoliza);
                dr["DEXPIRDAT"] = Convert.ToDateTime(data.datosPoliza.FinVigPoliza);
                dr["DNULLDATE"] = DBNull.Value;
                dr["DCOMPDATE"] = DateTime.Now;
                dr["NUSERCODE"] = data.codUsuario;
                dr["NSTATREGT"] = Convert.ToBoolean(surcharge.optional) ? 1 : 0;
                dr["SSURCHARGEUSE"] = Convert.ToBoolean(surcharge.optional) ? 1 : 0;
                dr["NIGV"] = Convert.ToBoolean(surcharge.igv) ? 1 : 0;

                dt.Rows.Add(dr);
            }

            return dt;
        }

        public SalidaTramaBaseVM InsertExclusiones(validaTramaVM data, List<ExclusionTariff> exclusions)
        {
            var response = new SalidaTramaBaseVM();

            try
            {
                var dataTable = generateDataTableExclusions(exclusions, data);
                response = this.SaveUsingOracleBulkCopy(dataTable, ProcedureName.tbl_CotizacionExclusiones);
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = "1";
                response.P_MESSAGE = ex.ToString();
                LogControl.save("InsertExclusiones", ex.ToString(), "3");
            }

            //using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
            //{
            //    using (OracleCommand cmd = new OracleCommand())
            //    {
            //        try
            //        {
            //            OracleDataReader dr;

            //            cmd.Connection = cn;
            //            cmd.CommandText = ProcedureName.pkg_ReaDataAP + ".INS_EXCLUSION_COT";
            //            cmd.CommandType = CommandType.StoredProcedure;

            //            cmd.Parameters.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, data.codProceso, ParameterDirection.Input));
            //            cmd.Parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.datosPoliza.branch, ParameterDirection.Input));
            //            cmd.Parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, data.datosPoliza.codTipoProducto, ParameterDirection.Input));
            //            cmd.Parameters.Add(new OracleParameter("P_NCOD_EXCLUSION", OracleDbType.Varchar2, exclusion.exclusion.code, ParameterDirection.Input));
            //            cmd.Parameters.Add(new OracleParameter("P_SDESC_EXCLUSION", OracleDbType.Varchar2, exclusion.exclusion.description, ParameterDirection.Input));
            //            cmd.Parameters.Add(new OracleParameter("P_STYPE", OracleDbType.Varchar2, exclusion.exclusion.type, ParameterDirection.Input));
            //            cmd.Parameters.Add(new OracleParameter("P_SVERSION", OracleDbType.Varchar2, exclusion.exclusion._id, ParameterDirection.Input));
            //            cmd.Parameters.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Varchar2, data.datosPoliza.InicioVigPoliza, ParameterDirection.Input));
            //            cmd.Parameters.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Varchar2, data.datosPoliza.FinVigPoliza, ParameterDirection.Input));
            //            cmd.Parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int64, data.codUsuario, ParameterDirection.Input));
            //            cmd.Parameters.Add(new OracleParameter("P_NSTATREGT", OracleDbType.Int32, exclusion.status == "ENABLED" ? 1 : 0, ParameterDirection.Input));

            //            var P_COD_ERR = new OracleParameter("P_ERROR", OracleDbType.Int32, ParameterDirection.Output);
            //            var P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

            //            P_MESSAGE.Size = 4000;

            //            cmd.Parameters.Add(P_COD_ERR);
            //            cmd.Parameters.Add(P_MESSAGE);

            //            cn.Open();
            //            dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            //            response.P_COD_ERR = P_COD_ERR.Value.ToString();
            //            response.P_MESSAGE = P_MESSAGE.Value.ToString();

            //            dr.Close();
            //        }
            //        catch (Exception ex)
            //        {
            //            response.P_COD_ERR = "1";
            //            response.P_MESSAGE = ex.ToString();
            //            ELog.save(this, ex.ToString());
            //        }
            //        finally
            //        {
            //            if (cn.State == ConnectionState.Open) cn.Close();
            //        }
            //    }
            //} 

            return response;
        }

        private DataTable generateDataTableExclusions(List<ExclusionTariff> exclusions, validaTramaVM request)
        {
            DataTable dt = new DataTable();

            try
            {
                dt.Columns.Add("NID_COTIZACION", typeof(int));
                dt.Columns.Add("NID_PROC");
                dt.Columns.Add("NBRANCH", typeof(int));
                dt.Columns.Add("NPRODUCT", typeof(int));
                dt.Columns.Add("NCOD_EXCLUSION");
                dt.Columns.Add("SDESC_EXCLUSION");
                dt.Columns.Add("STYPE");
                dt.Columns.Add("SVERSION");
                dt.Columns.Add("DEFFECDATE", typeof(DateTime));
                dt.Columns.Add("DEXPIRDAT", typeof(DateTime));
                dt.Columns.Add("DNULLDATE", typeof(DateTime));
                dt.Columns.Add("DCOMPDATE", typeof(DateTime));
                dt.Columns.Add("NUSERCODE", typeof(int));
                dt.Columns.Add("NSTATREGT", typeof(int));

                foreach (var exclusion in exclusions)
                {
                    DataRow dr = dt.NewRow();
                    dr["NID_COTIZACION"] = 0;
                    dr["NID_PROC"] = request.codProceso;
                    dr["NBRANCH"] = request.datosPoliza.branch;
                    dr["NPRODUCT"] = request.datosPoliza.codTipoProducto;
                    dr["NCOD_EXCLUSION"] = exclusion.exclusion.code;
                    dr["SDESC_EXCLUSION"] = exclusion.exclusion.description;
                    dr["STYPE"] = exclusion.exclusion.type;
                    dr["SVERSION"] = exclusion.exclusion._id;
                    dr["DEFFECDATE"] = Convert.ToDateTime(request.datosPoliza.InicioVigPoliza);
                    dr["DEXPIRDAT"] = Convert.ToDateTime(request.datosPoliza.FinVigPoliza);
                    dr["DNULLDATE"] = DBNull.Value;
                    dr["DCOMPDATE"] = DateTime.Now;
                    dr["NUSERCODE"] = request.codUsuario;
                    dr["NSTATREGT"] = exclusion.status == "ENABLED" ? 1 : 0;

                    dt.Rows.Add(dr);
                }
            }
            catch (Exception ex)
            {
                LogControl.save("generateDataTableExclusions", ex.ToString(), "3");
            }

            return dt;
        }

        public QuotationResponseVM UpdateDetalleModFinal(QuotationCabBM request, DbConnection connection, DbTransaction trx)
        {
            var sPackageName = ProcedureName.pkg_Cotizacion + ".UPD_COTIZACION_DET_MOD_FINAL";
            List<OracleParameter> parameter = new List<OracleParameter>();
            QuotationResponseVM result = new QuotationResponseVM();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, request.NumeroCotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, request.P_NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, request.P_NPRODUCT, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, result.P_MESSAGE, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, result.P_COD_ERR, ParameterDirection.Output);

                P_MESSAGE.Size = 9000;

                parameter.Add(P_MESSAGE);
                parameter.Add(P_COD_ERR);

                this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, connection, trx);
                result.P_MESSAGE = P_MESSAGE.Value.ToString();
                result.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
            }
            catch (Exception ex)
            {
                LogControl.save("UpdateDetalleModFinal", ex.ToString(), "3");
            }
            return result;
        }

        public List<StatusVM> GetStatusListRQ(string certype, string codProduct)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            string[] statusList = ELog.obtainConfig("statusList1" + certype + codProduct).Split(';');
            List<StatusVM> list = new List<StatusVM>();

            string storedProcedureName = ProcedureName.pkg_Cotizacion + ".REA_EST_COTIZA";
            try
            {
                parameter.Add(new OracleParameter("P_SCERTYPE", OracleDbType.Varchar2, certype, ParameterDirection.Input));
                OracleParameter table = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(table);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                while (odr.Read())
                {
                    StatusVM item = new StatusVM();
                    item.Id = odr["COD_ESTADO"].ToString();
                    item.Name = odr["DES_ESTADO"].ToString();

                    list.Add(item);

                }
                ELog.CloseConnection(odr);

                list = list.Where((x) => statusList.Contains(x.Id.ToString())).ToList();
            }
            catch (Exception ex)
            {
                LogControl.save("GetStatusListRQ", ex.ToString(), "3");
            }
            return list;
        }

        public void deleteProcess(string idProcess)
        {
            var sPackageName = ProcedureName.sp_EliminarProcessPayment;
            List<OracleParameter> parameter = new List<OracleParameter>();
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, idProcess, ParameterDirection.Input));

                this.ExecuteByStoredProcedureVT(sPackageName, parameter);
            }
            catch (Exception ex)
            {
                LogControl.save("deleteProcess", ex.ToString(), "3");
            }
        }

        public List<insuredGraph> GetTramaAsegurado(string codProceso)
        {
            var aseguradosList = new List<insuredGraph>();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.pkg_ValidadorGenPD + ".SP_TRAMA_SEND";

            try
            {

                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, codProceso, ParameterDirection.Input));
                OracleParameter table = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(table);

                OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storeprocedure, parameter);
                while (dr.Read())
                {
                    var det = new insuredGraph();
                    det.role = dr["ROLE"] == DBNull.Value ? "" : Convert.ToString(dr["ROLE"].ToString());
                    det.name = dr["NAME"] == DBNull.Value ? "" : Convert.ToString(dr["NAME"].ToString());
                    det.surname1 = dr["SURNAME1"] == DBNull.Value ? "" : Convert.ToString(dr["SURNAME1"].ToString());
                    det.surname2 = dr["SURNAME2"] == DBNull.Value ? "" : Convert.ToString(dr["SURNAME2"].ToString());
                    det.documentType = dr["DOCUMENTTYPE"] == DBNull.Value ? "" : Convert.ToString(dr["DOCUMENTTYPE"].ToString());
                    det.documentNumber = dr["DOCUMENTNUMBER"] == DBNull.Value ? "" : Convert.ToString(dr["DOCUMENTNUMBER"].ToString());
                    det.birthDate = dr["BIRTHDATE"] == DBNull.Value ? new DateTime().ToString("yyyy-MM-ddThh:mm:ss.fffZ") : Convert.ToDateTime(dr["BIRTHDATE"].ToString()).ToString("yyyy-MM-ddThh:mm:ss.fffZ");
                    det.gender = dr["GENDER"] == DBNull.Value ? "" : Convert.ToString(dr["GENDER"].ToString());
                    det.salary = dr["SALARY"] == DBNull.Value ? 0 : Convert.ToDouble(dr["SALARY"].ToString());
                    det.workerType = new WorkerType()
                    {
                        code_core = dr["CODE_CORE"] == DBNull.Value ? "0" : Convert.ToString(dr["CODE_CORE"].ToString()),
                        description = dr["DESCRIPTION"] == DBNull.Value ? "" : Convert.ToString(dr["DESCRIPTION"].ToString())

                    };
                    det.status = dr["STATUS"] == DBNull.Value ? "" : Convert.ToString(dr["STATUS"].ToString());
                    det.countryOfBirth = dr["COUNTRYOFBITH"] == DBNull.Value ? "Peru" : Convert.ToString(dr["COUNTRYOFBITH"].ToString());
                    det.documentType2 = dr["DOCUMENTTYPE2"] == DBNull.Value ? "" : Convert.ToString(dr["DOCUMENTTYPE2"].ToString());
                    det.documentNumber2 = dr["DOCUMENTNUMBER2"] == DBNull.Value ? "" : Convert.ToString(dr["DOCUMENTNUMBER2"].ToString());
                    det.relation = dr["RELATION"] == DBNull.Value ? "" : Convert.ToString(dr["RELATION"].ToString());
                    det.entryGrade = dr["ENTRYGRADE"] == DBNull.Value ? "" : Convert.ToString(dr["ENTRYGRADE"].ToString());

                    aseguradosList.Add(det);
                }
                ELog.CloseConnection(dr);
            }
            catch (Exception ex)
            {
                LogControl.save("GetTramaAsegurado", ex.ToString(), "3");
            }

            return aseguradosList;
        }

        public GenericResponseVM AgrupacionAPVG(ValidaTramaBM request)
        {
            var response = new GenericResponseVM();
            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        OracleDataReader dr;

                        cmd.Connection = cn;
                        cmd.CommandText = ProcedureName.sp_ValTramaApVg;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.NID_PROC, ParameterDirection.Input));
                        var P_ERROR = new OracleParameter("P_COD_ERR", OracleDbType.Varchar2, ParameterDirection.Output);
                        var P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                        P_ERROR.Size = 200;
                        P_MESSAGE.Size = 4000;

                        cmd.Parameters.Add(P_ERROR);
                        cmd.Parameters.Add(P_MESSAGE);

                        cn.Open();
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        response.StatusCode = Convert.ToInt32(P_ERROR.Value.ToString());
                        response.Message = P_MESSAGE.Value.ToString();

                        ELog.CloseConnection(dr);
                    }
                    catch (Exception ex)
                    {
                        LogControl.save("Error Inserccion en MAS_SCTR AP/VG", ex.ToString(), "3");
                        response.MessageError = "Error en el servidor: " + ex.Message;
                        response.StatusCode = 1;
                    }
                    finally
                    {
                        if (cn.State == ConnectionState.Open) cn.Close();
                    }
                }
            }
            return response;
        }

        public PremiumFixVM TotalPremiumFix(PremiumFixBM request)
        {
            var response = new PremiumFixVM();
            var sPackageName = ProcedureName.sp_FixPrimaCotiza;
            List<OracleParameter> parameter = new List<OracleParameter>();
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, request.quotation, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Varchar2, request.codBranch, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPREMIUM_IN", OracleDbType.Double, request.premium, ParameterDirection.Input));
                // OUTPUT
                OracleParameter P_NPREMIUM_OUT = new OracleParameter("P_NPREMIUM_OUT", OracleDbType.Varchar2, 200, response.premium, ParameterDirection.Output);
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, response.codError, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 200, response.desError, ParameterDirection.Output);

                parameter.Add(P_NPREMIUM_OUT);
                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);

                this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                response.codError = Convert.ToInt32(P_NCODE.Value.ToString());
                response.desError = P_SMESSAGE.Value.ToString();
                response.premium = Convert.ToDouble(P_NPREMIUM_OUT.Value.ToString());
            }
            catch (Exception ex)
            {
                response = new PremiumFixVM()
                {
                    codError = 1,
                    desError = ex.ToString(),
                    premium = 0
                };
                LogControl.save("TotalPremiumFix", ex.ToString(), "3");
            }

            return response;
        }

        public PremiumFixVM TotalPremiumCred(PremiumFixBM request) //AVS PRY NC
        {
            var response = new PremiumFixVM();
            var sPackageName = ProcedureName.pkg_paymentNC + ".SP_OBT_PREMIUNNC";
            List<OracleParameter> parameter = new List<OracleParameter>();
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, request.quotation, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Varchar2, request.codBranch, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPREMIUM_IN", OracleDbType.Double, request.premium, ParameterDirection.Input));
                // OUTPUT
                OracleParameter P_NPREMIUM_OUT = new OracleParameter("P_NPREMIUM_OUT", OracleDbType.Varchar2, 200, response.premium, ParameterDirection.Output);
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, response.codError, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 200, response.desError, ParameterDirection.Output);

                parameter.Add(P_NPREMIUM_OUT);
                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);

                this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                response.codError = Convert.ToInt32(P_NCODE.Value.ToString());
                response.desError = P_SMESSAGE.Value.ToString();
                response.premium = Convert.ToDouble(P_NPREMIUM_OUT.Value.ToString());
            }
            catch (Exception ex)
            {
                response = new PremiumFixVM()
                {
                    codError = 1,
                    desError = ex.ToString(),
                    premium = 0
                };
                LogControl.save("TotalPremiumCred", ex.ToString(), "3");
            }

            return response;
        }


        public CipValidateVM validarCipExistente(CipValidateBM request)
        {
            var response = new CipValidateVM();

            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        OracleDataReader dr;

                        cmd.Connection = cn;
                        cmd.CommandText = ProcedureName.pkg_payment + ".SP_VALIDATE_CIP";
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.nidproc, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, request.quotation, ParameterDirection.Input));

                        var P_SOPERATIONNUMBER = new OracleParameter("P_SOPERATIONNUMBER", OracleDbType.Varchar2, ParameterDirection.Output);
                        var P_NCIP_STATUS = new OracleParameter("P_NCIP_STATUS", OracleDbType.Int32, ParameterDirection.Output);
                        var P_FLAG_CIP = new OracleParameter("P_FLAG_CIP", OracleDbType.Int32, ParameterDirection.Output);

                        P_SOPERATIONNUMBER.Size = 200;

                        cmd.Parameters.Add(P_SOPERATIONNUMBER);
                        cmd.Parameters.Add(P_NCIP_STATUS);
                        cmd.Parameters.Add(P_FLAG_CIP);

                        cn.Open();
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        response.numberOperation = P_SOPERATIONNUMBER.Value.ToString();
                        response.statusCip = Convert.ToInt32(P_NCIP_STATUS.Value.ToString());
                        response.flagCip = Convert.ToInt32(P_FLAG_CIP.Value.ToString());

                        ELog.CloseConnection(dr);
                    }
                    catch (Exception ex)
                    {
                        LogControl.save("validarCipExistente-CIP", ex.ToString() + " - json: " + JsonConvert.SerializeObject(request), "3");
                    }
                    finally
                    {
                        if (cn.State == ConnectionState.Open) cn.Close();
                        LogControl.save("validarCipExistente-CIP", JsonConvert.SerializeObject(response, Formatting.None), "2");
                    }
                }
            }



            //var sPackageName = ProcedureName.pkg_payment + ".SP_VALIDATE_CIP";
            //List<OracleParameter> parameter = new List<OracleParameter>();
            //CipValidateVM response = new CipValidateVM();

            //try
            //{
            //    //INPUT
            //    parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.nidproc, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, request.quotation, ParameterDirection.Input));
            //    //output
            //    OracleParameter P_SOPERATIONNUMBER = new OracleParameter("P_SOPERATIONNUMBER", OracleDbType.Varchar2, 200, response.numberOperation, ParameterDirection.Output);
            //    OracleParameter P_NCIP_STATUS = new OracleParameter("P_NCIP_STATUS", OracleDbType.Int32, response.statusCip, ParameterDirection.Output);
            //    OracleParameter P_FLAG_CIP = new OracleParameter("P_FLAG_CIP", OracleDbType.Int32, response.flagCip, ParameterDirection.Output);

            //    parameter.Add(P_SOPERATIONNUMBER);
            //    parameter.Add(P_NCIP_STATUS);
            //    parameter.Add(P_FLAG_CIP);

            //    this.ExecuteByStoredProcedureVT(sPackageName, parameter);
            //    response.numberOperation = P_SOPERATIONNUMBER.Value.ToString();
            //    response.statusCip = Convert.ToInt32(P_NCIP_STATUS.Value.ToString());
            //    response.flagCip = Convert.ToInt32(P_FLAG_CIP.Value.ToString());
            //}
            //catch (Exception ex)
            //{
            //    ELog.save(this, ex.ToString());
            //}

            return response;
        }

        public void UpdCargaTramaEndoso(ValidaTramaBM validaTramaBM)
        {
            SalidaErrorBaseVM response = new SalidaErrorBaseVM();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.sp_updRemTramaEndoso;

            try
            {
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, validaTramaBM.NID_COTIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, validaTramaBM.NID_PROC, ParameterDirection.Input));

                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                P_COD_ERR.Size = 9000;
                P_MESSAGE.Size = 9000;

                parameter.Add(P_COD_ERR);
                parameter.Add(P_MESSAGE);

                this.ExecuteByStoredProcedureVT(storeprocedure, parameter);
            }
            catch (Exception ex)
            {
                LogControl.save("UpdCargaTramaEndoso", ex.ToString(), "3");
            }
        }

        public QuotationResponseVM InsertTransact(QuotationCabBM request, int cotizacion, bool update = false, DbConnection connection = null, DbTransaction trx = null)
        {
            QuotationResponseVM result = new QuotationResponseVM();
            RequestTransact requestTransact = new RequestTransact();
            TransactResponse transactResponse = new TransactResponse();
            try
            {
                requestTransact.P_NID_COTIZACION = cotizacion;
                requestTransact.P_NPRODUCT = request.P_NPRODUCT;
                requestTransact.P_NBRANCH = request.P_NBRANCH;
                requestTransact.P_STRANSAC = request.TrxCode;
                requestTransact.P_NUSERCODE = request.P_NUSERCODE;
                requestTransact.P_SMAIL_EJECCOM = request.SMAIL_EJECCOM;
                requestTransact.P_SRUTA = request.FlagCotEstado == 1 || request.FlagCotEstado == 2 ? request.P_SRUTA : "";
                requestTransact.P_SCOMMENT = request.P_SCOMMENT;
                requestTransact.P_SDEVOLPRI = 0;
                requestTransact.P_NTIP_RENOV = Convert.ToInt32(request.P_NTIP_RENOV);
                requestTransact.P_NPAYFREQ = Convert.ToInt32(request.P_NPAYFREQ);
                requestTransact.P_NTYPENDOSO = 0;
                requestTransact.P_NIDPLAN = request.planId;
                requestTransact.P_NINTERMED = 0;
                requestTransact.P_DFEC_REG = request.P_DSTARTDATE;
                requestTransact.P_SPOL_MATRIZ = request.P_SPOL_MATRIZ.ToString();
                requestTransact.P_SPOL_ESTADO = request.P_SPOL_ESTADO;
                requestTransact.P_APROB_CLI = request.P_APROB_CLI;


                if (update)
                {
                    requestTransact.P_NID_TRAMITE = request.P_NID_TRAMITE;
                    transactResponse = transactDA.UpdateTransact(requestTransact, connection, trx);

                    result.P_NID_COTIZACION = cotizacion.ToString();
                    result.P_MESSAGE = transactResponse.P_MESSAGE.ToString();
                    result.P_COD_ERR = Convert.ToInt32(transactResponse.P_COD_ERR);
                    result.P_NID_TRAMITE = request.P_NID_TRAMITE;
                }
                else
                {
                    transactResponse = transactDA.InsertTransact(requestTransact, connection, trx);

                    result.P_NID_COTIZACION = cotizacion.ToString();
                    result.P_MESSAGE = transactResponse.P_MESSAGE.ToString();
                    result.P_COD_ERR = Convert.ToInt32(transactResponse.P_COD_ERR);
                    result.P_NID_TRAMITE = Convert.ToInt32(transactResponse.P_NID_TRAMITE);

                    if (result.P_NID_TRAMITE == 0)
                    {
                        result.P_COD_ERR = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                result.P_MESSAGE = ex.Message;
                result.P_COD_ERR = 1;
            }

            return result;
        }

        public GenericResponseVM RechazarTramiteProcess(UpdateStatusQuotationBM request)
        {
            var response = new GenericResponseVM();

            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        OracleDataReader dr;

                        cmd.Connection = cn;
                        cmd.CommandText = ProcedureName.pkg_Tramites + ".REVERSE_COTIZA_PEND";
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, request.NroCotizacion, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, 1, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, 73, ParameterDirection.Input));

                        cn.Open();
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        dr.Close();

                        response.ErrorCode = 0;
                        LogControl.save("RechazarTramiteProcess", "Paso X: Se ejecutó " + ProcedureName.pkg_Tramites + ".REVERSE_COTIZA_PEND", "1");
                    }
                    catch (Exception ex)
                    {
                        LogControl.save("RechazarTramiteProcess", ex.ToString(), "3");
                        response.MessageError = "Error en el servidor: " + ex.Message;
                        response.ErrorCode = 1;
                    }
                    finally
                    {
                        if (cn.State == ConnectionState.Open) cn.Close();
                    }
                }
            }

            return response;


            //List<OracleParameter> parameter = new List<OracleParameter>();
            //GenericResponseVM result = new GenericResponseVM();

            //try
            //{
            //    //INPUT
            //    parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, request.NroCotizacion, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, 1, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, 73, ParameterDirection.Input));

            //    this.ExecuteByStoredProcedureVT(sPackageName, parameter);

            //}
            //catch (Exception ex)
            //{
            //    result.MessageError = "Error en el servidor. " + ex.Message;
            //    result.ErrorCode = 1;
            //}
            //return result;
        }

        public PremiumFixVM ReverseMovementsIncomplete(PremiumFixBM request)
        {
            var response = new PremiumFixVM();
            var sPackageName = ProcedureName.pkg_Cotizacion + ".REVERSE_MOVEMENTS";
            List<OracleParameter> parameter = new List<OracleParameter>();
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, request.quotation, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, request.codBranch, ParameterDirection.Input));
                // OUTPUT
                OracleParameter P_SMESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, 200, response.desError, ParameterDirection.Output);
                OracleParameter P_NCODE = new OracleParameter("P_COD_ERR", OracleDbType.Int32, response.codError, ParameterDirection.Output);

                parameter.Add(P_SMESSAGE);
                parameter.Add(P_NCODE);

                this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                response.codError = Convert.ToInt32(P_NCODE.Value.ToString());
                response.desError = P_SMESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                response = new PremiumFixVM()
                {
                    codError = 1,
                    desError = ex.ToString(),
                };
                LogControl.save("ReverseMovementsIncomplete", ex.ToString(), "3");
            }

            return response;
        }

        public SalidaTramaBaseVM insertRules(List<Entities.Graphql.Rule> request, validaTramaVM info)
        {
            var response = new SalidaTramaBaseVM();
            try
            {
                var dataTable = generateDataTableRules(request, info);
                response = this.SaveUsingOracleBulkCopy(dataTable, ProcedureName.tbl_CotizacionReglas);
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = "1";
                response.P_MESSAGE = ex.ToString();
                LogControl.save("insertRules", ex.ToString(), "3");
            }

            return response;

            //using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
            //{
            //    using (OracleCommand cmd = new OracleCommand())
            //    {
            //        try
            //        {
            //            OracleDataReader dr;

            //            cmd.Connection = cn;
            //            cmd.CommandText = ProcedureName.pkg_ReaDataAP + ".INS_RULES_COT";
            //            cmd.CommandType = CommandType.StoredProcedure;

            //            cmd.Parameters.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, info.codProceso, ParameterDirection.Input));
            //            cmd.Parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int64, (info.datosPoliza.trxCode == "RE" && Convert.ToBoolean(info.datosPoliza.modoEditar)) ? 0 : info.nroCotizacion, ParameterDirection.Input));
            //            cmd.Parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, info.datosPoliza.branch, ParameterDirection.Input));
            //            cmd.Parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, info.datosPoliza.codTipoProducto, ParameterDirection.Input));
            //            cmd.Parameters.Add(new OracleParameter("P_STYPE_TRANSAC", OracleDbType.Varchar2, info.datosPoliza.trxCode, ParameterDirection.Input));
            //            cmd.Parameters.Add(new OracleParameter("P_NRULE", OracleDbType.Varchar2, request.id, ParameterDirection.Input));
            //            cmd.Parameters.Add(new OracleParameter("P_SIDRULE", OracleDbType.Varchar2, request.definition._id, ParameterDirection.Input));
            //            cmd.Parameters.Add(new OracleParameter("P_NCODE", OracleDbType.Varchar2, request.definition.code, ParameterDirection.Input));
            //            cmd.Parameters.Add(new OracleParameter("P_NORDER", OracleDbType.Int32, request.definition.order, ParameterDirection.Input));
            //            cmd.Parameters.Add(new OracleParameter("P_STYPE", OracleDbType.Varchar2, request.definition.type, ParameterDirection.Input));
            //            cmd.Parameters.Add(new OracleParameter("P_SDESCRIPT", OracleDbType.Varchar2, request.definition.description, ParameterDirection.Input));
            //            cmd.Parameters.Add(new OracleParameter("P_NRANGEMIN", OracleDbType.Int32, request.range != null ? request.range.min : 0, ParameterDirection.Input));
            //            cmd.Parameters.Add(new OracleParameter("P_NRANGEMAX", OracleDbType.Int32, request.range != null ? request.range.max : 0, ParameterDirection.Input));
            //            cmd.Parameters.Add(new OracleParameter("P_SRANGETYPE", OracleDbType.Varchar2, request.range != null ? request.range.type : null, ParameterDirection.Input));
            //            cmd.Parameters.Add(new OracleParameter("P_SCONDITION", OracleDbType.Varchar2, request.condition, ParameterDirection.Input));
            //            cmd.Parameters.Add(new OracleParameter("P_SVALUES", OracleDbType.Varchar2, request.values != null ? string.Join(",", request.values) : null, ParameterDirection.Input));
            //            cmd.Parameters.Add(new OracleParameter("P_NINCLUSION", OracleDbType.Int32, Convert.ToBoolean(request.inclusion) ? 1 : 0, ParameterDirection.Input));
            //            cmd.Parameters.Add(new OracleParameter("P_NVALIDATE", OracleDbType.Int32, request.active ? 1 : 0, ParameterDirection.Input));

            //            var P_ERROR = new OracleParameter("P_ERROR", OracleDbType.Varchar2, ParameterDirection.Output);
            //            var P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

            //            P_ERROR.Size = 200;
            //            P_MESSAGE.Size = 4000;

            //            cmd.Parameters.Add(P_ERROR);
            //            cmd.Parameters.Add(P_MESSAGE);

            //            cn.Open();
            //            dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            //            response.P_MESSAGE = P_MESSAGE.Value.ToString();
            //            response.P_COD_ERR = P_ERROR.Value.ToString();

            //            dr.Close();
            //        }
            //        catch (Exception ex)
            //        {
            //            ELog.save(this, ex.ToString() + " - request:" + JsonConvert.SerializeObject(request) + " - info: " + JsonConvert.SerializeObject(info));
            //        }
            //        finally
            //        {
            //            if (cn.State == ConnectionState.Open) cn.Close();
            //        }
            //    }
            //}

            //return response;
        }

        public DataTable generateDataTableRules(List<Entities.Graphql.Rule> request, validaTramaVM info)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("NID_PROC");
            dt.Columns.Add("NID_COTIZACION", typeof(int));
            dt.Columns.Add("NBRANCH", typeof(int));
            dt.Columns.Add("NPRODUCT", typeof(int));
            dt.Columns.Add("STYPE_TRANSAC");
            dt.Columns.Add("NRULE");
            dt.Columns.Add("SIDRULE");
            dt.Columns.Add("NCODE");
            dt.Columns.Add("NORDER", typeof(int));
            dt.Columns.Add("STYPE");
            dt.Columns.Add("SDESCRIPT");
            dt.Columns.Add("NRANGEMIN", typeof(int));
            dt.Columns.Add("NRANGEMAX", typeof(int));
            dt.Columns.Add("SRANGETYPE");
            dt.Columns.Add("SCONDITION");
            dt.Columns.Add("SVALUES");
            dt.Columns.Add("NINCLUSION", typeof(int));
            dt.Columns.Add("NVALIDATE", typeof(int));
            dt.Columns.Add("DCOMPDATE", typeof(DateTime));
            dt.Columns.Add("DNULLDATE");

            foreach (var rule in request)
            {
                DataRow dr = dt.NewRow();
                dr["NID_PROC"] = info.codProceso;
                dr["NID_COTIZACION"] = info.nroCotizacion == null ? 0 : (info.datosPoliza.trxCode == "RE" && Convert.ToBoolean(info.datosPoliza.modoEditar)) ? 0 : info.nroCotizacion;
                dr["NBRANCH"] = info.datosPoliza.branch;
                dr["NPRODUCT"] = info.datosPoliza.codTipoProducto;
                dr["STYPE_TRANSAC"] = info.datosPoliza.trxCode;
                dr["NRULE"] = rule.id;
                dr["SIDRULE"] = rule.definition._id;
                dr["NCODE"] = rule.definition.code;
                dr["NORDER"] = rule.definition.order;
                dr["STYPE"] = rule.definition.type.Trim().ToUpper();
                dr["SDESCRIPT"] = rule.definition.description.Trim().ToUpper();
                dr["NRANGEMIN"] = rule.range != null ? rule.range.min : 0;
                dr["NRANGEMAX"] = rule.range != null ? rule.range.max : 0;
                dr["SRANGETYPE"] = rule.range != null ? rule.range.type : null;
                dr["SCONDITION"] = rule.condition;
                dr["SVALUES"] = rule.values != null ? string.Join(",", rule.values) : null;
                dr["NINCLUSION"] = Convert.ToBoolean(rule.inclusion) ? 1 : 0;
                dr["NVALIDATE"] = rule.status == "ACTIVE" ? rule.definition.type == "GENERAL_LOGIC" ? rule.active ? 1 : 0 : 1 : 0;
                dr["DCOMPDATE"] = DateTime.Now;
                dr["DNULLDATE"] = null;

                dt.Rows.Add(dr);
            }

            return dt;
        }

        public SalidaTramaBaseVM insertRulesDes(List<Entities.Graphql.Rule> request, validaTramaVM info)
        {
            var response = new SalidaTramaBaseVM();
            try
            {
                var dataTable = generateDataTableRulesDes(request, info);
                response = this.SaveUsingOracleBulkCopy(dataTable, ProcedureName.tbl_CotizacionReglas);
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = "1";
                response.P_MESSAGE = ex.ToString();
                LogControl.save("insertRulesDes", ex.ToString(), "3");
            }

            return response;
        }

        public DataTable generateDataTableRulesDes(List<Entities.Graphql.Rule> request, validaTramaVM info)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("NID_PROC");
            dt.Columns.Add("NID_COTIZACION", typeof(int));
            dt.Columns.Add("NBRANCH", typeof(int));
            dt.Columns.Add("NPRODUCT", typeof(int));
            dt.Columns.Add("STYPE_TRANSAC");
            dt.Columns.Add("NRULE");
            dt.Columns.Add("SIDRULE");
            dt.Columns.Add("NCODE");
            dt.Columns.Add("NORDER", typeof(int));
            dt.Columns.Add("STYPE");
            dt.Columns.Add("SDESCRIPT");
            dt.Columns.Add("NRANGEMIN", typeof(int));
            dt.Columns.Add("NRANGEMAX", typeof(int));
            dt.Columns.Add("SRANGETYPE");
            dt.Columns.Add("SCONDITION");
            dt.Columns.Add("SVALUES");
            dt.Columns.Add("NINCLUSION", typeof(int));
            dt.Columns.Add("NVALIDATE", typeof(int));
            dt.Columns.Add("DCOMPDATE", typeof(DateTime));
            dt.Columns.Add("DNULLDATE");

            foreach (var rule in request)
            {
                DataRow dr = dt.NewRow();
                dr["NID_PROC"] = info.codProceso;
                dr["NID_COTIZACION"] = (info.datosPoliza.trxCode == "RE" && Convert.ToBoolean(info.datosPoliza.modoEditar)) ? 0 : info.nroCotizacion;
                dr["NBRANCH"] = info.datosPoliza.branch;
                dr["NPRODUCT"] = info.datosPoliza.codTipoProducto;
                dr["STYPE_TRANSAC"] = info.datosPoliza.trxCode;
                dr["NRULE"] = rule.id;
                dr["SIDRULE"] = rule.definition._id;
                dr["NCODE"] = rule.definition.code;
                dr["NORDER"] = rule.definition.order;
                dr["STYPE"] = rule.definition.type.Trim().ToUpper();
                dr["SDESCRIPT"] = rule.definition.description.Trim().ToUpper();
                dr["NRANGEMIN"] = rule.content.numericInterval != null ? rule.content.numericInterval.interval.min : 0;
                dr["NRANGEMAX"] = rule.content.numericInterval != null ? rule.content.numericInterval.interval.max : 0;
                dr["SRANGETYPE"] = rule.content.numericInterval != null ? rule.content.numericInterval.interval.type : null;
                dr["SCONDITION"] = rule.content.conditionalValue != null ? rule.content.conditionalValue.condition : null;
                dr["SVALUES"] = rule.content.conditionalValue != null ? string.Join(",", rule.content.conditionalValue.value) : null;
                dr["NINCLUSION"] = Convert.ToBoolean(rule.inclusion) ? 1 : 0;
                dr["NVALIDATE"] = rule.active ? 1 : 0;
                dr["DCOMPDATE"] = DateTime.Now;
                dr["DNULLDATE"] = null;

                dt.Rows.Add(dr);
            }

            return dt;
        }

        public SalidaTramaBaseVM insertRulesDes(Entities.Graphql.Rule request, validaTramaVM info)
        {
            var response = new SalidaTramaBaseVM();

            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        OracleDataReader dr;

                        cmd.Connection = cn;
                        cmd.CommandText = ProcedureName.pkg_ReaDataAP + ".INS_RULES_COT";
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, info.codProceso, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int64, info.nroCotizacion, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, info.datosPoliza.branch, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, info.datosPoliza.codTipoProducto, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_STYPE_TRANSAC", OracleDbType.Varchar2, info.datosPoliza.trxCode, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NRULE", OracleDbType.Varchar2, request.id, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_SIDRULE", OracleDbType.Varchar2, request.definition._id, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NCODE", OracleDbType.Varchar2, request.definition.code, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NORDER", OracleDbType.Int32, request.definition.order, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_STYPE", OracleDbType.Varchar2, request.definition.type, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_SDESCRIPT", OracleDbType.Varchar2, request.definition.description, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NRANGEMIN", OracleDbType.Int32, request.content.numericInterval != null ? request.content.numericInterval.interval.min : 0, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NRANGEMAX", OracleDbType.Int32, request.content.numericInterval != null ? request.content.numericInterval.interval.max : 0, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_SRANGETYPE", OracleDbType.Varchar2, request.content.numericInterval != null ? request.content.numericInterval.interval.type : null, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_SCONDITION", OracleDbType.Varchar2, request.content.conditionalValue != null ? request.content.conditionalValue.condition : null, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_SVALUES", OracleDbType.Varchar2, request.content.conditionalValue != null ? string.Join(",", request.content.conditionalValue.value) : null, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NINCLUSION", OracleDbType.Int32, Convert.ToBoolean(request.inclusion) ? 1 : 0, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NVALIDATE", OracleDbType.Int32, request.active ? 1 : 0, ParameterDirection.Input));

                        var P_ERROR = new OracleParameter("P_ERROR", OracleDbType.Varchar2, ParameterDirection.Output);
                        var P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                        P_ERROR.Size = 200;
                        P_MESSAGE.Size = 4000;

                        cmd.Parameters.Add(P_ERROR);
                        cmd.Parameters.Add(P_MESSAGE);

                        cn.Open();
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        response.P_MESSAGE = P_MESSAGE.Value.ToString();
                        response.P_COD_ERR = P_ERROR.Value.ToString();

                        ELog.CloseConnection(dr);

                    }
                    catch (Exception ex)
                    {
                        LogControl.save("insertRulesDes", ex.ToString() + " - request:" + JsonConvert.SerializeObject(request) + " - info: " + JsonConvert.SerializeObject(info), "3");
                    }
                    finally
                    {
                        if (cn.State == ConnectionState.Open) cn.Close();
                    }
                }
            }

            //var sPackageName = ProcedureName.pkg_ReaDataAP + ".INS_RULES_COT";
            //List<OracleParameter> parameter = new List<OracleParameter>();

            //try
            //{
            //    //INPUT
            //    parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, info.codProceso, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int64, info.nroCotizacion, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, info.datosPoliza.branch, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, info.datosPoliza.codTipoProducto, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_STYPE_TRANSAC", OracleDbType.Varchar2, info.datosPoliza.trxCode, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_NRULE", OracleDbType.Int32, request.id, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_SIDRULE", OracleDbType.Varchar2, request.definition._id, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_NCODE", OracleDbType.Int32, request.definition.code, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_NORDER", OracleDbType.Int32, request.definition.order, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_STYPE", OracleDbType.Varchar2, request.definition.type, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_SDESCRIPT", OracleDbType.Varchar2, request.definition.description, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_NRANGEMIN", OracleDbType.Int32, request.content.numericInterval != null ? request.content.numericInterval.interval.min : 0, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_NRANGEMAX", OracleDbType.Int32, request.content.numericInterval != null ? request.content.numericInterval.interval.max : 0, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_SRANGETYPE", OracleDbType.Varchar2, request.content.numericInterval != null ? request.content.numericInterval.interval.type : null, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_SCONDITION", OracleDbType.Varchar2, request.content.conditionalOptions != null ? request.content.conditionalOptions.condition : null, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_SVALUES", OracleDbType.Varchar2, request.content.conditionalOptions != null ? string.Join(",", request.content.conditionalOptions.options) : null, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_NINCLUSION", OracleDbType.Int32, Convert.ToBoolean(request.inclusion) ? 1 : 0, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_NVALIDATE", OracleDbType.Int32, request.active ? 1 : 0, ParameterDirection.Input));

            //    //OUTPUT
            //    OracleParameter P_ERROR = new OracleParameter("P_ERROR", OracleDbType.Varchar2, 9000, response.P_COD_ERR, ParameterDirection.Output);
            //    OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, 900, response.P_MESSAGE, ParameterDirection.Output);

            //    parameter.Add(P_ERROR);
            //    parameter.Add(P_MESSAGE);

            //    this.ExecuteByStoredProcedureVT(sPackageName, parameter);
            //    response.P_MESSAGE = P_MESSAGE.Value.ToString();
            //    response.P_COD_ERR = P_ERROR.Value.ToString();
            //}
            //catch (Exception ex)
            //{
            //    ELog.save(this, ex.ToString());
            //}
            return response;
        }

        public SalidaErrorBaseVM insertSubItem(validaTramaVM data, ItemGeneric subItem)
        {
            SalidaErrorBaseVM response = new SalidaErrorBaseVM();

            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
            {
                cn.Open();

                using (var tran = cn.BeginTransaction())
                {
                    try
                    {
                        response = insertSubItemTrx(data, subItem, cn, tran);
                    }
                    catch (Exception ex)
                    {
                        response.P_COD_ERR = 1;
                        response.P_MESSAGE = ex.ToString();
                        LogControl.save("insertSubItem", ex.ToString(), "3");
                    }
                    finally
                    {
                        if (response.P_COD_ERR == 0)
                        {
                            tran.Commit();
                        }
                        else
                        {
                            tran.Rollback();
                        }

                        if (cn.State == ConnectionState.Open) cn.Close();
                    }
                }
            }

            return response;
        }

        public SalidaErrorBaseVM insertSubItemTrx(validaTramaVM request, ItemGeneric subItem, OracleConnection connection, OracleTransaction trx)
        {
            var response = new SalidaErrorBaseVM();

            using (OracleCommand cmd = new OracleCommand())
            {
                try
                {
                    OracleDataReader dr;

                    cmd.Connection = connection;
                    cmd.CommandText = ProcedureName.pkg_ReaDataAP + ".INS_COVER_EXT_COT";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Transaction = trx;

                    //cmd.Parameters.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.codProceso, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.codProceso, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, request.datosPoliza.branch, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, request.datosPoliza.codTipoProducto, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_NCOVERGEN", OracleDbType.Int64, subItem.codeCover, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_NID", OracleDbType.Int64, subItem.id, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_SDESCRIPT", OracleDbType.Varchar2, subItem.description, ParameterDirection.Input));
                    cmd.Parameters.Add(new OracleParameter("P_NCAPITALCOVERED", OracleDbType.Double, subItem.capitalCovered, ParameterDirection.Input));

                    var P_COD_ERR = new OracleParameter("P_ERROR", OracleDbType.Int32, ParameterDirection.Output);
                    var P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                    P_MESSAGE.Size = 9000;

                    cmd.Parameters.Add(P_COD_ERR);
                    cmd.Parameters.Add(P_MESSAGE);

                    dr = cmd.ExecuteReader();

                    response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                    response.P_MESSAGE = P_MESSAGE.Value.ToString();
                }
                catch (Exception ex)
                {
                    response.P_COD_ERR = 1;
                    response.P_MESSAGE = "Hubo un error al insertar los sub items de las coberturas"; // ex.ToString();
                    LogControl.save("insertSubItemTrx", ex.ToString(), "3");
                }
            }

            return response;
        }

        public SalidaTramaBaseVM insertSubItemPD(validaTramaVM request, List<Coverage> coberturas)
        {
            var response = new SalidaTramaBaseVM();

            try
            {
                var dataTable = generateDataTableCoverSub(request, coberturas);
                response = this.SaveUsingOracleBulkCopy(dataTable, ProcedureName.tbl_CotizacionCoverSubPD);
            }
            catch (Exception ex)
            {
                LogControl.save("insertSubItemPD", ex.ToString(), "3");
            }

            return response;
        }

        private DataTable generateDataTableCoverSub(validaTramaVM request, List<Coverage> coberturas)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("NID_PROC");
            dt.Columns.Add("NID_COTIZACION", typeof(int));
            dt.Columns.Add("NBRANCH", typeof(int));
            dt.Columns.Add("NPRODUCT", typeof(int));
            dt.Columns.Add("NCOVERGEN", typeof(int));
            dt.Columns.Add("NID", typeof(int));
            dt.Columns.Add("SDESCRIPT");
            dt.Columns.Add("NCAPITALCOVERED", typeof(decimal));
            dt.Columns.Add("DCOMPDATE", typeof(DateTime));

            foreach (var cobertura in coberturas)
            {
                foreach (var subItem in cobertura.items)
                {
                    DataRow dr = dt.NewRow();
                    dr["NID_PROC"] = request.codProceso;
                    dr["NID_COTIZACION"] = 0;
                    dr["NBRANCH"] = request.datosPoliza.branch;
                    dr["NPRODUCT"] = request.datosPoliza.codTipoProducto;
                    dr["NCOVERGEN"] = cobertura.id;
                    dr["NID"] = subItem.id;
                    dr["SDESCRIPT"] = subItem.description;
                    dr["NCAPITALCOVERED"] = subItem.capitalCovered;
                    dr["DCOMPDATE"] = DateTime.Now;

                    dt.Rows.Add(dr);
                }
            }

            return dt;
        }

        public SalidaTramaBaseVM InsGenBenefactors(validaTramaVM data)
        {
            string storedProcedureName = ProcedureName.pkg_ValidadorGenPD + ".SP_GEN_BENEFICIARIO";
            List<OracleParameter> parameter = new List<OracleParameter>();
            SalidaTramaBaseVM result = new SalidaTramaBaseVM();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_PROD", OracleDbType.Varchar2, data.codProceso, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, 1, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_PROFILE", OracleDbType.Int32, data.datosPoliza.codTipoPerfil, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, data.codUsuario, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, data.datosPoliza.codTipoProducto, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NFLAG_REC", OracleDbType.Int32, 0, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.nroCotizacion, ParameterDirection.Input));


                //OUTPUT
                OracleParameter P_ERROR = new OracleParameter("P_ERROR", OracleDbType.Varchar2, result.P_COD_ERR, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, result.P_MESSAGE, ParameterDirection.Output);
                P_ERROR.Size = 9000;
                P_MESSAGE.Size = 9000;
                parameter.Add(P_ERROR);
                parameter.Add(P_MESSAGE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                result.P_COD_ERR = P_ERROR.Value.ToString();
                result.P_MESSAGE = P_MESSAGE.Value.ToString();

            }
            catch (Exception ex)
            {
                result.P_COD_ERR = "1";
                result.P_MESSAGE = ex.ToString();
            }

            return result;
        }

        public SalidaTramaBaseVM DeleteBenefactors(string codProceso, string rol)

        {
            string storedProcedureName = ProcedureName.pkg_ValidadorGenPD + ".SP_DEL_CARGA_MAS_SCTR";
            List<OracleParameter> parameter = new List<OracleParameter>();
            SalidaTramaBaseVM result = new SalidaTramaBaseVM();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, codProceso, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SROLE", OracleDbType.Varchar2, rol, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_ERROR = new OracleParameter("P_ERROR", OracleDbType.Varchar2, result.P_COD_ERR, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, result.P_MESSAGE, ParameterDirection.Output);
                P_ERROR.Size = 9000;
                P_MESSAGE.Size = 9000;
                parameter.Add(P_ERROR);
                parameter.Add(P_MESSAGE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                result.P_COD_ERR = P_ERROR.Value.ToString();
                result.P_MESSAGE = P_MESSAGE.Value.ToString();

            }
            catch (Exception ex)
            {
                result.P_COD_ERR = "1";
                result.P_MESSAGE = ex.ToString();
            }

            return result;

        }
        public QuotationResponseVM UpdateStatusCotizacion(string cotizacion, QuotationCabBM request, DbConnection connection, DbTransaction trx)
        {
            var sPackageName = ProcedureName.pkg_Cotizacion + ".SP_GEN_MOV_COTIZACION_EST";

            List<OracleParameter> parameter = new List<OracleParameter>();
            QuotationResponseVM result = new QuotationResponseVM();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, cotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SSTATREGT", OracleDbType.Varchar2, "2", ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NSTATE_DOC", OracleDbType.Int32, 0, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DSTARTDATE", OracleDbType.Varchar2, request.P_DSTARTDATE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Varchar2, request.P_NUSERCODE, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, result.P_COD_ERR, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, result.P_MESSAGE, ParameterDirection.Output);

                P_COD_ERR.Size = 9000;
                P_MESSAGE.Size = 9000;

                parameter.Add(P_COD_ERR);
                parameter.Add(P_MESSAGE);


                this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, connection, trx);

                result.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                result.P_MESSAGE = P_MESSAGE.Value.ToString();

            }
            catch (Exception ex)
            {
                result.P_COD_ERR = 1;
                result.P_MESSAGE = ex.ToString();
            }
            return result;
        }
        public QuotationResponseVM UpdateCargaEC(string cotizacion, QuotationCabBM request, DbConnection connection, DbTransaction trx)
        {
            var sPackageName = ProcedureName.pkg_Cotizacion + ".SP_UPD_CARGA_MAS_SCTR_EC";

            List<OracleParameter> parameter = new List<OracleParameter>();
            QuotationResponseVM result = new QuotationResponseVM();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.CodigoProceso, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, cotizacion, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, result.P_COD_ERR, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, result.P_MESSAGE, ParameterDirection.Output);

                P_COD_ERR.Size = 9000;
                P_MESSAGE.Size = 9000;

                parameter.Add(P_COD_ERR);
                parameter.Add(P_MESSAGE);

                this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, connection, trx);

                result.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                result.P_MESSAGE = P_MESSAGE.Value.ToString();

            }
            catch (Exception ex)
            {
                result.P_COD_ERR = 1;
                result.P_MESSAGE = ex.ToString();
            }
            return result;
        }

        public QuotationResponseVM UpdateCotProrrateo(string cotizacion, QuotationCabBM request, DbConnection connection, DbTransaction trx)
        {
            var sPackageName = ProcedureName.pkg_Cotizacion + ".SP_UPD_TABLA_PRORRATEO";

            List<OracleParameter> parameter = new List<OracleParameter>();
            QuotationResponseVM result = new QuotationResponseVM();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.CodigoProceso, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, cotizacion, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, result.P_COD_ERR, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, result.P_MESSAGE, ParameterDirection.Output);

                P_COD_ERR.Size = 9000;
                P_MESSAGE.Size = 9000;

                parameter.Add(P_COD_ERR);
                parameter.Add(P_MESSAGE);


                this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, connection, trx);

                result.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                result.P_MESSAGE = P_MESSAGE.Value.ToString();

            }
            catch (Exception ex)
            {
                result.P_COD_ERR = 1;
                result.P_MESSAGE = ex.ToString();
            }
            return result;
        }

        public List<BeneficiarVM> GetBeneficiarsList(BeneficiarBM data)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<BeneficiarVM> beneficiarList = new List<BeneficiarVM>();

            string storedProcedureName = ProcedureName.pkg_Cotizacion + ".SP_LIST_BENEFICIAR";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, data.P_NID_PROC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.P_NBRANCH, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        BeneficiarVM item = new BeneficiarVM();
                        item.NID_PROC = odr["NID_PROC"] == DBNull.Value ? string.Empty : odr["NID_PROC"].ToString();
                        item.NID_COTIZACION = odr["NID_COTIZACION"] == DBNull.Value ? string.Empty : odr["NID_COTIZACION"].ToString();
                        item.SFIRSTNAME = odr["SFIRSTNAME"] == DBNull.Value ? string.Empty : odr["SFIRSTNAME"].ToString();
                        item.SLASTNAME = odr["SLASTNAME"] == DBNull.Value ? string.Empty : odr["SLASTNAME"].ToString();
                        item.SLASTNAME2 = odr["SLASTNAME2"] == DBNull.Value ? string.Empty : odr["SLASTNAME2"].ToString();
                        item.NMODULEC = odr["NMODULEC"] == DBNull.Value ? string.Empty : odr["NMODULEC"].ToString();
                        item.NNATIONALITY = odr["NNATIONALITY"] == DBNull.Value ? string.Empty : odr["NNATIONALITY"].ToString();
                        item.NIDDOC_TYPE = odr["NIDDOC_TYPE"] == DBNull.Value ? string.Empty : odr["NIDDOC_TYPE"].ToString();
                        item.SIDDOC = odr["SIDDOC"] == DBNull.Value ? string.Empty : odr["SIDDOC"].ToString();
                        item.DBIRTHDAT = odr["DBIRTHDAT"] == DBNull.Value ? string.Empty : odr["DBIRTHDAT"].ToString();
                        item.SSEXCLIEN = odr["SSEXCLIEN"] == DBNull.Value ? string.Empty : odr["SSEXCLIEN"].ToString();
                        item.SE_MAIL = odr["SE_MAIL"] == DBNull.Value ? string.Empty : odr["SE_MAIL"].ToString();
                        item.NPHONE_TYPE = odr["NPHONE_TYPE"] == DBNull.Value ? string.Empty : odr["NPHONE_TYPE"].ToString();
                        item.SPHONE = odr["SPHONE"] == DBNull.Value ? string.Empty : odr["SPHONE"].ToString();
                        item.PERCEN_PARTICIPATION = odr["PERCEN_PARTICIPATION"] == DBNull.Value ? string.Empty : odr["PERCEN_PARTICIPATION"].ToString();
                        item.NRELATION = odr["NRELATION"] == DBNull.Value ? string.Empty : odr["NRELATION"].ToString();
                        item.SCLIENT = odr["SCLIENT"] == DBNull.Value ? string.Empty : odr["SCLIENT"].ToString();
                        item.NIDPLAN = odr["NIDPLAN"] == DBNull.Value ? string.Empty : odr["NIDPLAN"].ToString();
                        item.NID_REG = odr["NID_REG"] == DBNull.Value ? string.Empty : odr["NID_REG"].ToString();
                        item.NCERTIF = odr["NCERTIF"] == DBNull.Value ? string.Empty : odr["NCERTIF"].ToString();
                        item.NIDDOC_TYPE_PADRE = odr["NIDDOC_TYPE_PADRE"] == DBNull.Value ? string.Empty : odr["NIDDOC_TYPE_PADRE"].ToString();
                        item.SIDDOC_PADRE = odr["SIDDOC_PADRE"] == DBNull.Value ? string.Empty : odr["SIDDOC_PADRE"].ToString();
                        beneficiarList.Add(item);
                    }
                }

                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("GetBeneficiarsList", ex.ToString(), "3");
            }

            return beneficiarList;
        }

        public NumCredit UpdateNumCredit(NumCreditUPD request)
        {
            string sPackageName = ProcedureName.pkg_Cotizacion + ".SP_UPD_COTIZACION_CAB_NUMCRED";

            List<OracleParameter> parameter = new List<OracleParameter>();
            NumCredit result = new NumCredit();

            DbConnection DataConnection = ConnectionGet(enuTypeDataBase.OracleVTime);
            DataConnection.Open();
            DbTransaction trx = DataConnection.BeginTransaction();


            try
            {

                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, request.P_NID_COTIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NNUM_CREDIT", OracleDbType.Int32, request.P_NNUM_CREDIT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, request.P_NUSERCODE, ParameterDirection.Input));


                //OUTPUT
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, result.P_COD_ERR, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, result.P_MESSAGE, ParameterDirection.Output);

                P_COD_ERR.Size = 9000;
                P_MESSAGE.Size = 9000;

                parameter.Add(P_COD_ERR);
                parameter.Add(P_MESSAGE);


                this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, DataConnection, trx);

                result.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                result.P_MESSAGE = P_MESSAGE.Value.ToString();

                if (result.P_COD_ERR == 0)
                {
                    trx.Commit();
                    trx.Dispose();
                    DataConnection.Close();
                }
            }

            catch (Exception ex)
            {
                result.P_COD_ERR = 1;
                result.P_MESSAGE = ex.ToString();
                trx.Dispose();
                DataConnection.Close();
            }

            return result;
        }
        public SalidaErrorBaseVM regDPS(int nidcotizacion, string codproceso, int branch, int codprod, int usuario, Entities.QuotationModel.ViewModel.DatosDPS datosdps)
        {
            SalidaErrorBaseVM response = new SalidaErrorBaseVM();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.pkg_ReaDataAP + ".INS_DPS_COT";

            DbConnection connection = ConnectionGet(enuTypeDataBase.OracleVTime);
            connection.Open();
            DbTransaction trx = connection.BeginTransaction();

            try
            {
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, nidcotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, codproceso, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, branch, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, codprod, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NQUESTION", OracleDbType.Double, datosdps.question, ParameterDirection.Input)); // AGF 14042023
                parameter.Add(new OracleParameter("P_SDESC_QUESTION", OracleDbType.Varchar2, "", ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SVALUE", OracleDbType.Varchar2, datosdps.value, ParameterDirection.Input));
                if (datosdps.detail != null)
                {
                    parameter.Add(new OracleParameter("P_SDESC_ANSWER", OracleDbType.Varchar2, datosdps.detail.value, ParameterDirection.Input));
                }
                else
                {
                    parameter.Add(new OracleParameter("P_SDESC_ANSWER", OracleDbType.Varchar2, "", ParameterDirection.Input));
                }
                if (datosdps.questionFather != null)
                {
                    parameter.Add(new OracleParameter("P_NQUESTION_FATHER", OracleDbType.Varchar2, datosdps.questionFather, ParameterDirection.Input));
                }
                else
                {
                    parameter.Add(new OracleParameter("P_NQUESTION_FATHER", OracleDbType.Varchar2, "", ParameterDirection.Input));
                }
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int64, usuario, ParameterDirection.Input));

                OracleParameter P_COD_ERR = new OracleParameter("P_ERROR", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                P_COD_ERR.Size = 9000;
                P_MESSAGE.Size = 9000;

                parameter.Add(P_COD_ERR);
                parameter.Add(P_MESSAGE);

                this.ExecuteByStoredProcedureVT_TRX(storeprocedure, parameter, connection, trx);

                response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                response.P_MESSAGE = Convert.ToString(P_MESSAGE.Value.ToString());
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = ex.ToString();
                LogControl.save("regDPS", ex.ToString(), "3");
            }
            finally
            {
                if (response.P_COD_ERR == 0)
                {
                    trx.Commit();
                }
                else
                {
                    if (trx.Connection != null) trx.Rollback();
                }

                if (trx.Connection != null) trx.Dispose();
                ELog.CloseConnection(connection);
            }
            return response;
        }

        public SalidaErrorBaseVM regDPSRecalculo(validaTramaVM request, Entities.QuotationModel.BindingModel.DatosDPS datosdps, DbConnection connection, DbTransaction trx)
        {
            SalidaErrorBaseVM response = new SalidaErrorBaseVM();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.pkg_ReaDataAP + ".INS_DPS_COT";

            try
            {
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, 0, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.codProceso, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, request.datosPoliza.branch, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, request.datosPoliza.codTipoProducto, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NQUESTION", OracleDbType.Double, datosdps.question, ParameterDirection.Input)); ; // AGF 14042023
                parameter.Add(new OracleParameter("P_SDESC_QUESTION", OracleDbType.Varchar2, "", ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SVALUE", OracleDbType.Varchar2, datosdps.value, ParameterDirection.Input));
                if (datosdps.detail != null)
                {
                    parameter.Add(new OracleParameter("P_SDESC_ANSWER", OracleDbType.Varchar2, datosdps.detail.value, ParameterDirection.Input));
                }
                else
                {
                    parameter.Add(new OracleParameter("P_SDESC_ANSWER", OracleDbType.Varchar2, "", ParameterDirection.Input));
                }
                if (datosdps.questionFather != null)
                {
                    parameter.Add(new OracleParameter("P_NQUESTION_FATHER", OracleDbType.Varchar2, datosdps.questionFather, ParameterDirection.Input));
                }
                else
                {
                    parameter.Add(new OracleParameter("P_NQUESTION_FATHER", OracleDbType.Varchar2, "", ParameterDirection.Input));
                }
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int64, request.codUsuario, ParameterDirection.Input));

                OracleParameter P_COD_ERR = new OracleParameter("P_ERROR", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                P_COD_ERR.Size = 9000;
                P_MESSAGE.Size = 9000;

                parameter.Add(P_COD_ERR);
                parameter.Add(P_MESSAGE);

                this.ExecuteByStoredProcedureVT_TRX(storeprocedure, parameter, connection, trx);

                response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                response.P_MESSAGE = Convert.ToString(P_MESSAGE.Value.ToString());
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = ex.ToString();
                LogControl.save("regDPSRecalculo", ex.ToString(), "3");
            }

            return response;
        }

        public List<DpsVM> getDPS(int numcotizacion, int nbranch, int nproduct)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<DpsVM> dpsList = new List<DpsVM>();

            string storedProcedureName = ProcedureName.pkg_ReaDataAP + ".REA_DPS_ALL";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, nbranch, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, nproduct, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, numcotizacion, ParameterDirection.Input)); // Verificar para usar ncotizacion

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        DpsVM item = new DpsVM();

                        item.NQUESTION = odr["NQUESTION"] == DBNull.Value ? string.Empty : odr["NQUESTION"].ToString();
                        item.SVALUE = odr["SVALUE"] == DBNull.Value ? string.Empty : odr["SVALUE"].ToString();
                        item.SDESC_ANSWER = odr["SDESC_ANSWER"] == DBNull.Value ? string.Empty : odr["SDESC_ANSWER"].ToString();
                        item.NQUESTION_FATHER = odr["SVALUE"] == DBNull.Value ? string.Empty : odr["NQUESTION_FATHER"].ToString();

                        dpsList.Add(item);
                    }
                }

                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("getDPS", ex.ToString(), "3");
            }

            return dpsList;
        }
        public AuthDerivationVM getAuthDerivation(validaTramaVM request)
        {
            var response = new AuthDerivationVM();
            var sPackageName = ProcedureName.pkg_ValidadorGenPD + ".SP_GET_AUTH_RULE";
            List<OracleParameter> parameter = new List<OracleParameter>();
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.codProceso, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int64, (request.datosPoliza.trxCode == "RE" && Convert.ToBoolean(request.datosPoliza.modoEditar)) ? 0 : request.nroCotizacion, ParameterDirection.Input));
                // OUTPUT
                OracleParameter P_NAUTH = new OracleParameter("P_NAUTH", OracleDbType.Int32, response.codAuth, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, 500, response.desAuth, ParameterDirection.Output);

                parameter.Add(P_NAUTH);
                parameter.Add(P_SMESSAGE);

                this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                response.codAuth = Convert.ToInt32(P_NAUTH.Value.ToString());
                response.desAuth = P_SMESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                response = new AuthDerivationVM()
                {
                    codAuth = 0,
                    desAuth = ex.ToString(),
                };
                LogControl.save("getAuthDerivation", ex.ToString(), "3");
            }

            return response;
        }

        public validaTramaVM valMinMaxAseguradosPolMatriz(validaTramaVM request)
        {
            //var response = new validaTramaVM();
            var sPackageName = ProcedureName.pkg_ValidadorGenPD + ".SP_MIN_MAX_ASEGURADOS";
            List<OracleParameter> parameter = new List<OracleParameter>();
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NTRAB", OracleDbType.Int64, request.CantidadTrabajadores, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.codProceso, ParameterDirection.Input));
                // OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, request.codError, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, 500, request.desError, ParameterDirection.Output);

                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);

                this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                request.codError = Convert.ToInt32(P_NCODE.Value.ToString());
                request.desError = P_SMESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                request = new validaTramaVM()
                {
                    codError = 1,
                    desError = ex.ToString(),
                };
                LogControl.save("valMinMaxAseguradosPolMatriz", ex.ToString(), "3");
            }

            return request;
        }

        public SalidaErrorBaseVM valSumasASeguradas(validaTramaVM request)
        {
            var response = new SalidaErrorBaseVM();
            var sPackageName = ProcedureName.pkg_ValidadorGenPD + ".SP_VAL_SUMAS_ASEGURADAS";
            List<OracleParameter> parameter = new List<OracleParameter>();
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.codProceso, ParameterDirection.Input));
                // OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, response.P_COD_ERR, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, 500, response.P_MESSAGE, ParameterDirection.Output);

                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);

                this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                request.codError = Convert.ToInt32(P_NCODE.Value.ToString());
                request.desError = P_SMESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                response = new SalidaErrorBaseVM()
                {
                    P_COD_ERR = 1,
                    P_MESSAGE = ex.ToString(),
                };
                LogControl.save("valSumasASeguradas", ex.ToString(), "3");
            }

            return response;
        }

        //INI MEJORAS VALIDACION VL 
        public SalidaErrorBaseVM updateAsegReniec(ConsultaCBM item, EListClient client, validaTramaVM objValida = null)
        {
            var response = new SalidaErrorBaseVM();
            var sPackageName = ProcedureName.sp_UpdateReniec;
            List<OracleParameter> parameter = new List<OracleParameter>();
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_DESDOC_TYPE", OracleDbType.Varchar2, item.P_NIDDOC_TYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, item.P_SIDDOC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SFIRSTNAME", OracleDbType.Varchar2, client.P_SFIRSTNAME, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLASTNAME", OracleDbType.Varchar2, client.P_SLASTNAME, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLASTNAME2", OracleDbType.Varchar2, client.P_SLASTNAME2, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SSEXCLIEN", OracleDbType.Varchar2, client.P_SSEXCLIEN, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DBIRTHDAT", OracleDbType.Varchar2, client.P_DBIRTHDAT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, item.P_NID_PROC, ParameterDirection.Input));

                // OUTPUT
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, 500, response.P_MESSAGE, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, response.P_COD_ERR, ParameterDirection.Output);

                parameter.Add(P_MESSAGE);
                parameter.Add(P_COD_ERR);
                parameter.Add(new OracleParameter("P_SAPE_CASADA", OracleDbType.Varchar2, client.P_APELLIDO_CASADA, ParameterDirection.Input)); //Apellido de casada
                parameter.Add(new OracleParameter("P_SLASTNAME2CONCAT", OracleDbType.Varchar2, client.P_SLASTNAME2CONCAT, ParameterDirection.Input)); //Apellido de casada  concatenado con apellido materno.
                                                                                                                                                      //INI MEJORAS VALIDACION VL 
                if (objValida != null)
                {
                    parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, objValida.codRamo, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, objValida.codProducto, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, objValida.codUsuario, ParameterDirection.Input));
                }
                //FIN MEJORAS VALIDACION VL 
                this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                response.P_MESSAGE = P_MESSAGE.Value.ToString();

            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = ex.ToString();
                LogControl.save("updateAsegReniec", ex.ToString(), "3");
            }

            return response;
        }

        public async Task<ResponseGraph> getObtenerPrimaPD(validaTramaVM dataCotizacion)
        {

            var response = new ResponseGraph();
            var sPackageName = ProcedureName.pkg_ValidadorGenPD + ".SP_GET_OBTENER_PRIMA_PD";

            response.data = new GenericResponse()
            {
                searchPremium = new SearchPremium()
                {
                    commercialPremium = 0,
                    netRate = 0 //AVS - RENTAS
                }
            };

            List<OracleParameter> parameter = new List<OracleParameter>();
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, dataCotizacion.codProceso, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_PROC_ANT", OracleDbType.Varchar2, dataCotizacion.datosPoliza.proceso_anterior, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, dataCotizacion.datosPoliza.branch, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int64, (dataCotizacion.datosPoliza.trxCode == "RE" && Convert.ToBoolean(dataCotizacion.datosPoliza.modoEditar)) ? 0 : dataCotizacion.nroCotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NWORKERS", OracleDbType.Int64, dataCotizacion.CantidadTrabajadores, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NFREQUENCY", OracleDbType.Int64, dataCotizacion.datosPoliza.codTipoFrecuenciaPago, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STRANSAC", OracleDbType.Varchar2, dataCotizacion.datosPoliza.trxCode, ParameterDirection.Input));

                // OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, response.codError, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 500, response.message, ParameterDirection.Output);
                OracleParameter P_NPREMIUN = new OracleParameter("P_NPREMIUN", OracleDbType.Varchar2, 500, response.data.searchPremium.commercialPremium, ParameterDirection.Output);
                OracleParameter P_NTASA = new OracleParameter("P_NTASA", OracleDbType.Varchar2, 500, response.data.searchPremium.riskRate, ParameterDirection.Output);

                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);
                parameter.Add(P_NPREMIUN);
                parameter.Add(P_NTASA);

                this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                response.codError = Convert.ToInt32(P_NCODE.Value.ToString());
                response.message = P_SMESSAGE.Value.ToString();
                response.data.searchPremium.commercialPremium = Convert.ToDouble(P_NPREMIUN.Value.ToString());
                response.data.searchPremium.netRate = Convert.ToDouble(P_NTASA.Value.ToString()) / 100;

            }
            catch (Exception ex)
            {
                response.codError = 1;
                response.message = ex.ToString();
                response.data.searchPremium.commercialPremium = 0;
                LogControl.save("getObtenerPrimaPD", ex.ToString(), "3");
            }

            return await Task.FromResult(response);
        }

        public QuotationResponseVM GetFlagPremiumMin(QuotationCabBM request)
        {
            var response = new QuotationResponseVM();
            var sPackageName = ProcedureName.pkg_Cotizacion + ".SP_GET_CLIENTE_NOAPLICA_PM";
            List<OracleParameter> parameter = new List<OracleParameter>();
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, request.P_NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, request.P_SCLIENT, ParameterDirection.Input));
                // OUTPUT
                OracleParameter P_NFLAG_APLICA_PM = new OracleParameter("P_NFLAG_APLICA_PM", OracleDbType.Int32, response.P_NCODE, ParameterDirection.Output);

                parameter.Add(P_NFLAG_APLICA_PM);

                this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                response.P_NCODE = Convert.ToInt32(P_NFLAG_APLICA_PM.Value.ToString());
            }
            catch (Exception ex)
            {
                response = new QuotationResponseVM()
                {
                    P_NCODE = 0,
                };
                LogControl.save("GetFlagPremiumMin", ex.ToString(), "3");
            }

            return response;
        }

        public long getMaxTrama(string param, string codRamo)
        {
            long response = -1;
            //var sPackageName = ProcedureName.sp_ReaAseguradoTrama;
            //List<OracleParameter> parameter = new List<OracleParameter>();
            //try
            //{
            //    //INPUT
            //    parameter.Add(new OracleParameter("P_STYPE", OracleDbType.Varchar2, param, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Varchar2, codRamo, ParameterDirection.Input));
            //    // OUTPUT
            //    OracleParameter P_NASEG_MAX = new OracleParameter("P_NASEG_MAX", OracleDbType.Int64, response, ParameterDirection.Output);

            //    parameter.Add(P_NASEG_MAX);

            //    this.ExecuteByStoredProcedureVT(sPackageName, parameter);
            //    response = Convert.ToInt64(P_NASEG_MAX.Value.ToString());
            //}
            //catch (Exception ex)
            //{
            //    response = -1;
            //    ELog.save(this, ex.ToString());
            //}


            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        OracleDataReader dr;

                        cmd.Connection = cn;
                        cmd.CommandText = ProcedureName.sp_ReaAseguradoTrama;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new OracleParameter("P_STYPE", OracleDbType.Varchar2, param, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Varchar2, codRamo, ParameterDirection.Input));

                        var P_NASEG_MAX = new OracleParameter("P_NASEG_MAX", OracleDbType.Int64, ParameterDirection.Output);
                        cmd.Parameters.Add(P_NASEG_MAX);

                        cn.Open();

                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        response = Convert.ToInt64(P_NASEG_MAX.Value.ToString());

                        ELog.CloseConnection(dr);

                    }
                    catch (Exception ex)
                    {
                        response = -1;
                        LogControl.save("getMaxTrama", ex.ToString(), "3");
                    }
                    finally
                    {
                        if (cn.State == ConnectionState.Open) cn.Close();
                    }
                }
            }

            return response;
        }

        //arjg consulta de registros en tabla configuracion
        public long getMaxReg(string param)
        {
            long response = -1;
            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        OracleDataReader dr;

                        cmd.Connection = cn;
                        cmd.CommandText = ProcedureName.sp_Configuraciondcd;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, param, ParameterDirection.Input));
                        var P_NCONTADOR = new OracleParameter("P_NCONTADOR", OracleDbType.Int64, ParameterDirection.Output);
                        cmd.Parameters.Add(P_NCONTADOR);
                        cn.Open();
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        response = Convert.ToInt64(P_NCONTADOR.Value.ToString());
                        ELog.CloseConnection(dr);
                    }
                    catch (Exception ex)
                    {
                        response = -1;
                        LogControl.save("getMaxReg", ex.ToString(), "3");
                    }
                    finally
                    {
                        if (cn.State == ConnectionState.Open) cn.Close();
                    }
                }
            }

            return response;
        }

        public QuotationResponseVM UpdateQuotationDetRC(QuotationCabBM request, QuotationDetBM request2, DbConnection connection, DbTransaction trx)
        {
            var sPackageName = ProcedureName.pkg_Cotizacion + ".UPD_COTIZACION_DET_RC";
            List<OracleParameter> parameter = new List<OracleParameter>();
            QuotationResponseVM result = new QuotationResponseVM();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, request2.P_NID_COTIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, request.P_NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, request2.P_NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Varchar2, request2.P_NMODULEC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTOTAL_TRABAJADORES", OracleDbType.Int64, request2.P_NTOTAL_TRABAJADORES, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMONTO_PLANILLA", OracleDbType.Decimal, request2.P_NMONTO_PLANILLA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTASA_CALCULADA", OracleDbType.Decimal, request2.P_NTASA_CALCULADA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTASA_PROP", OracleDbType.Decimal, request2.P_NTASA_PROP, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPREMIUM_MENSUAL", OracleDbType.Decimal, request2.P_NPREMIUM_MENSUAL, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPREMIUM_MIN", OracleDbType.Decimal, request2.P_NPREMIUM_MIN, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPREMIUM_MIN_PR", OracleDbType.Decimal, request2.P_NPREMIUM_MIN_PR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPREMIUM_END", OracleDbType.Decimal, request2.P_NPREMIUM_END, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NSUM_PREMIUMN", OracleDbType.Decimal, request2.P_NSUM_PREMIUMN, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NSUM_IGV", OracleDbType.Decimal, request2.P_NSUM_IGV, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NSUM_PREMIUM", OracleDbType.Decimal, request2.P_NSUM_PREMIUM, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, request.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NRATE", OracleDbType.Decimal, request2.P_NRATE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NDISCOUNT", OracleDbType.Decimal, request2.P_NDISCOUNT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NACTIVITYVARIACTION", OracleDbType.Decimal, request2.P_NACTIVITYVARIATION, ParameterDirection.Input));
                //parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.CodigoProceso, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Date, request.P_DSTARTDATE_ASE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FLAG", OracleDbType.Int32, request2.P_FLAG, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, result.P_MESSAGE, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, result.P_COD_ERR, ParameterDirection.Output);
                OracleParameter P_FLAG_AUT = new OracleParameter("P_FLAG_AUT", OracleDbType.Int32, result.P_FLAG_AUT, ParameterDirection.Output);

                P_MESSAGE.Size = 9000;

                parameter.Add(P_MESSAGE);
                parameter.Add(P_COD_ERR);
                parameter.Add(P_FLAG_AUT);

                parameter.Add(new OracleParameter("P_NAMO_AFEC", OracleDbType.Decimal, request2.P_NAMO_AFEC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIVA", OracleDbType.Decimal, request2.P_NIVA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NAMOUNT", OracleDbType.Decimal, request2.P_NAMOUNT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NDE", OracleDbType.Decimal, request2.P_NDE, ParameterDirection.Input));                        //RI
                //parameter.Add(new OracleParameter("P_STIPO_APP ", OracleDbType.Varchar2, request.P_STIPO_APP, ParameterDirection.Input));           //RI

                this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, connection, trx);
                result.P_MESSAGE = P_MESSAGE.Value.ToString();
                result.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                result.P_FLAG_AUT = Convert.ToInt32(P_FLAG_AUT.Value.ToString());
            }
            catch (Exception ex)
            {
                result.P_MESSAGE = ex.ToString();
                result.P_COD_ERR = 1;
                LogControl.save("UpdateQuotationDetRC", ex.ToString(), "3");
            }

            return result;
        }

        public QuotationResponseVM UpdateQuotationDetPremium(QuotationCabBM request, int? NID_COTIZACION, DbConnection connection, DbTransaction trx)
        {
            var sPackageName = ProcedureName.pkg_Cotizacion + ".UPD_COTIZACION_DET_PRIMA";
            List<OracleParameter> parameter = new List<OracleParameter>();
            QuotationResponseVM result = new QuotationResponseVM();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, NID_COTIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, request.P_NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, request.P_NPRODUCT, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, result.P_MESSAGE, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, result.P_COD_ERR, ParameterDirection.Output);

                P_MESSAGE.Size = 9000;

                parameter.Add(P_MESSAGE);
                parameter.Add(P_COD_ERR);

                this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, connection, trx);
                result.P_MESSAGE = P_MESSAGE.Value.ToString();
                result.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
            }
            catch (Exception ex)
            {
                result.P_MESSAGE = ex.ToString();
                result.P_COD_ERR = 1;
                LogControl.save("UpdateQuotationDetPremium", ex.ToString(), "3");
            }

            return result;
        }

        public SalidaTramaBaseVM InsertDataEC(ValidaTramaEcommerce data)
        {
            var response = new SalidaTramaBaseVM();
            //List<OracleParameter> parameter = new List<OracleParameter>();
            //string storeprocedure = "PD_INS_DATA_TRAMA_EC";

            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        OracleDataReader dr;

                        cmd.Connection = cn;
                        cmd.CommandText = ProcedureName.sp_InsertDataEcommerce;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, data.cod_proceso, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NID_PROC_OLD", OracleDbType.Varchar2, data.cod_proceso_old, ParameterDirection.Input));

                        var P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, ParameterDirection.Output);
                        var P_MESSAGE = new OracleParameter("P_SERROR", OracleDbType.Varchar2, ParameterDirection.Output);

                        P_MESSAGE.Size = 4000;

                        cmd.Parameters.Add(P_COD_ERR);
                        cmd.Parameters.Add(P_MESSAGE);

                        cn.Open();
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        response.P_COD_ERR = P_COD_ERR.Value.ToString();
                        response.P_MESSAGE = P_MESSAGE.Value.ToString();

                        ELog.CloseConnection(dr);
                    }
                    catch (Exception ex)
                    {
                        response.P_COD_ERR = "1";
                        response.P_MESSAGE = ex.Message;
                        LogControl.save("InsertDataEC", ex.ToString(), "3");
                    }
                    finally
                    {
                        if (cn.State == ConnectionState.Open) cn.Close();
                    }
                }
            }

            return response;
        }
        public EdadValidarVM GetEdadesValidar(CoberturaBM rangoEdades)
        {

            List<OracleParameter> parameter = new List<OracleParameter>();
            EdadValidarVM edades = new EdadValidarVM();

            string storedProcedureName = ProcedureName.pkg_ReaDataAP + ".REA_EDADES_MINMAX";

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, rangoEdades.codBranch, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, rangoEdades.tipoProducto, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {

                        edades.nbranch = odr["NBRANCH"] == DBNull.Value ? string.Empty : odr["NBRANCH"].ToString();
                        edades.nproduct = odr["NPRODUCT"] == DBNull.Value ? string.Empty : odr["NPRODUCT"].ToString();
                        edades.deffecdate = odr["DEFFECDATE"] == DBNull.Value ? string.Empty : odr["DEFFECDATE"].ToString();
                        edades.edadmin_ingreso = odr["NSUAGEMIN"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NSUAGEMIN"].ToString());
                        edades.edadmax_ingreso = odr["NSUAGEMAX"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NSUAGEMAX"].ToString());
                        edades.edadmax_permanen = odr["NREAGEMAX"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NREAGEMAX"].ToString());

                    }
                }

                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("GetEdadesValidar", ex.ToString(), "3");
            }

            return edades;

        }

        public SobrevivenciaVM GETSOBREVIXCOVER(string nidcotizacion, string nbranch)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            SobrevivenciaVM sobrevivencia = new SobrevivenciaVM();

            string storedProcedureName = ProcedureName.pkg_Cotizacion + ".GETSOBREVIXCOVER";

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, Int32.Parse(nidcotizacion), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, Int32.Parse(nbranch), ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_SOBREVIVENCIA = new OracleParameter("P_SOBREVIVENCIA", OracleDbType.Decimal, ParameterDirection.Output);
                OracleParameter P_NAGEMINING = new OracleParameter("P_NAGEMINING", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_NAGEMAXING = new OracleParameter("P_NAGEMAXING", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_NAGEMAXPER = new OracleParameter("P_NAGEMAXPER", OracleDbType.Int32, ParameterDirection.Output);

                parameter.Add(P_SOBREVIVENCIA);
                parameter.Add(P_NAGEMINING);
                parameter.Add(P_NAGEMAXING);
                parameter.Add(P_NAGEMAXPER);

                this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                string sobrevivenciastr = P_SOBREVIVENCIA.Value.ToString().Replace(',', '.');

                sobrevivencia.sobrevivencia = Decimal.Parse(sobrevivenciastr);
                sobrevivencia.edadmin_ingreso = Int32.Parse(P_NAGEMINING.Value.ToString());
                sobrevivencia.edadmax_ingreso = Int32.Parse(P_NAGEMAXING.Value.ToString());
                sobrevivencia.edadmax_permanen = Int32.Parse(P_NAGEMAXPER.Value.ToString());

                //OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                //result.P_MESSAGE = P_MESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                LogControl.save("GETSOBREVIXCOVER", ex.ToString(), "3");
            }

            return sobrevivencia;
        }

        public bool ValEstadoCotizacion(string nroCotizacion)
        {
            bool valido = false;

            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        OracleDataReader dr;

                        cmd.Connection = cn;
                        cmd.CommandText = ProcedureName.sp_ValEstadoCotizacion;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, nroCotizacion, ParameterDirection.Input));

                        var P_NSTATE = new OracleParameter("P_NSTATE", OracleDbType.Int32, ParameterDirection.Output);


                        cmd.Parameters.Add(P_NSTATE);

                        cn.Open();
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        valido = Convert.ToInt32(P_NSTATE.Value.ToString()) == 1 ? true : false;

                        ELog.CloseConnection(dr);
                    }
                    catch (Exception ex)
                    {
                        valido = false;
                        LogControl.save("ValEstadoCotizacion", "Paso 1: Error => " + Environment.NewLine + ex.ToString(), "3");
                    }
                    finally
                    {
                        if (cn.State == ConnectionState.Open) cn.Close();
                    }
                }
            }

            //valido = true;

            return valido;
        }

        // GCAA 30012024
        public bool ValTransaccionCotizacion(string nroCotizacion, int opcion)
        {
            bool valido = false;

            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        OracleDataReader dr;

                        cmd.Connection = cn;
                        cmd.CommandText = "SP_VAL_TIPO_COT";
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, nroCotizacion, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NTYPE_PROCESO", OracleDbType.Int32, opcion, ParameterDirection.Input));
                        var P_NSTATE = new OracleParameter("P_NSTATE", OracleDbType.Int32, ParameterDirection.Output);


                        cmd.Parameters.Add(P_NSTATE);

                        cn.Open();
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        valido = Convert.ToInt32(P_NSTATE.Value.ToString()) == 1 ? true : false;

                        dr.Close();
                    }
                    catch (Exception ex)
                    {
                        valido = false;
                        LogControl.save("ValEstadoCotizacion", "Paso 1: Error => " + Environment.NewLine + ex.ToString(), "3");
                    }
                    finally
                    {
                        if (cn.State == ConnectionState.Open) cn.Close();
                    }
                }
            }

            return valido;
        }

        public PrimaProrrateo GetPrimaProrrateo(int ncotizacion)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            PrimaProrrateo primas = new PrimaProrrateo();

            string storedProcedureName = ProcedureName.pkg_Cotizacion + ".GET_PRIMAS_PRORRATEADAS";

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, ncotizacion, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);
                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        var item = new PrimaProrrateo();
                        item.NPREMIUMN = odr["NPREMIUMN"] == DBNull.Value ? 0 : decimal.Parse(odr["NPREMIUMN"].ToString());
                        item.NPREMIUM = odr["NPREMIUM"] == DBNull.Value ? 0 : decimal.Parse(odr["NPREMIUM"].ToString());
                        item.NIGV = odr["NIGV"] == DBNull.Value ? 0 : decimal.Parse(odr["NIGV"].ToString());
                        item.NDE = odr["NDE"] == DBNull.Value ? 0 : decimal.Parse(odr["NDE"].ToString());
                        primas = item;
                    }
                }

                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("GetPrimaProrrateo", ex.ToString(), "3");
            }

            return primas;

        }

        //public QuotationResponseVM UpdateQuotationDetPremium(QuotationCabBM request, int? NID_COTIZACION, DbConnection connection, DbTransaction trx)
        //{
        //    var sPackageName = ProcedureName.pkg_Cotizacion + ".UPD_COTIZACION_DET_PRIMA";
        //    List<OracleParameter> parameter = new List<OracleParameter>();
        //    QuotationResponseVM result = new QuotationResponseVM();

        //    try
        //    {
        //        //INPUT
        //        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, NID_COTIZACION, ParameterDirection.Input));
        //        parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, request.P_NBRANCH, ParameterDirection.Input));
        //        parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, request.P_NPRODUCT, ParameterDirection.Input));

        //        //OUTPUT
        //        OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, result.P_MESSAGE, ParameterDirection.Output);
        //        OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, result.P_COD_ERR, ParameterDirection.Output);

        //        P_MESSAGE.Size = 9000;

        //        parameter.Add(P_MESSAGE);
        //        parameter.Add(P_COD_ERR);

        //        this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, connection, trx);
        //        result.P_MESSAGE = P_MESSAGE.Value.ToString();
        //        result.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
        //    }
        //    catch (Exception ex)
        //    {
        //        result.P_MESSAGE = ex.ToString();
        //        result.P_COD_ERR = 1;
        //        ELog.save(this, ex.ToString());
        //    }

        //    return result;
        //}

        public SalidaErrorBaseVM regBIO(int nidcotizacion, string nrodoc, string nrocli, string algoritmo, string porcsimil, string urlImagen, string fechaeval, string tipoval, string codigoverif)
        {
            SalidaErrorBaseVM response = new SalidaErrorBaseVM();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.pkg_ReaDataAP + ".INS_BIOMETRICO_COT";

            DbConnection connection = ConnectionGet(enuTypeDataBase.OracleVTime);
            connection.Open();
            DbTransaction trx = connection.BeginTransaction();

            try
            {
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, nidcotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUM_DOC", OracleDbType.Varchar2, nrodoc, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NOM_CLI", OracleDbType.Varchar2, nrocli, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_ALGORITMO", OracleDbType.Varchar2, algoritmo, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_PORC_SIMIL", OracleDbType.Double, Convert.ToDouble(porcsimil), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_URL_IMG", OracleDbType.Varchar2, urlImagen, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FECHA_EVAL", OracleDbType.Varchar2, fechaeval, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TIPO_VAL", OracleDbType.Int32, tipoval, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_COD_VER", OracleDbType.Varchar2, codigoverif, ParameterDirection.Input));

                OracleParameter P_COD_ERR = new OracleParameter("P_ERROR", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                P_COD_ERR.Size = 9000;
                P_MESSAGE.Size = 9000;

                parameter.Add(P_COD_ERR);
                parameter.Add(P_MESSAGE);

                this.ExecuteByStoredProcedureVT_TRX(storeprocedure, parameter, connection, trx);

                response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                response.P_MESSAGE = Convert.ToString(P_MESSAGE.Value.ToString());
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = ex.ToString();
                LogControl.save("regBIO", ex.ToString(), "3");
            }
            finally
            {
                if (response.P_COD_ERR == 0)
                {
                    trx.Commit();
                }
                else
                {
                    if (trx.Connection != null) trx.Rollback();
                }

                if (trx.Connection != null) trx.Dispose();
                ELog.CloseConnection(connection);
            }
            return response;
        }


        //VALIDACION DE LA API GESTIONAR CLIENTE  ARJG 16/09/2022

        public class Cliente
        {
            public string P_NCODE { get; set; }
            public string P_SMESSAGE { get; set; }
            public List<EListClient> EListClient { get; set; }
        }

        public class DatosGestClie
        {
            public string P_CodAplicacion { get; set; }
            public string P_TipOper { get; set; }
            public string P_NUSERCODE { get; set; }
            public string P_NIDDOC_TYPE { get; set; }
            public string P_SIDDOC { get; set; }
        }

        public ValidGesClieError insertPdvalError(string NID_PROC, string NUM_REGISTRO, string SDES_ERR)
        {
            var respuestaconf = new ValidGesClieError();
            List<OracleParameter> parameterins = new List<OracleParameter>();
            string storeprocedure = ProcedureName.sp_InsValError;
            OracleDataReader odri = null;

            try
            {
                parameterins.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, NID_PROC, ParameterDirection.Input));
                parameterins.Add(new OracleParameter("P_NUM_REGISTRO", OracleDbType.Int32, NUM_REGISTRO, ParameterDirection.Input));
                parameterins.Add(new OracleParameter("P_SDES_ERR", OracleDbType.Varchar2, SDES_ERR, ParameterDirection.Input));
                odri = (OracleDataReader)this.ExecuteByStoredProcedureVT(storeprocedure, parameterins);
                respuestaconf.P_COD_ERR = 0;
                respuestaconf.P_MESSAGE = "Se agrego registro correctamente";
            }
            catch (Exception ex)
            {
                respuestaconf.P_COD_ERR = 1;
                respuestaconf.P_MESSAGE = ex.ToString();
            }
            return respuestaconf;
        }

        //CONSUMIR API DE GESTIONAR CLIENTE ARJG 19/09/2022
        public ValidGesClie ValidarGestClie(string idproc)
        {
            List<QuotationApiGestClieVM> gestclie = new List<QuotationApiGestClieVM>();
            var response = new ValidGesClie();
            const int tam = 1000;
            string[,] MatrizApi = new string[tam, 3];
            int filas = 1;
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.sp_ValApigc;
            OracleDataReader dr = null;

            string DATA = "";

            try
            {
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, idproc, ParameterDirection.Input));
                string[] arrayCursor = { "C_TABLE" };
                OracleParameter C_TABLE = new OracleParameter(arrayCursor[0], OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);
                dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storeprocedure, arrayCursor, parameter);
                if (dr.HasRows)
                {

                    while (dr.Read())
                    {
                        DATA = "";
                        QuotationApiGestClieVM GestClieVM = new QuotationApiGestClieVM();
                        GestClieVM.NUSERCODE = dr["NUSERCODE"] == DBNull.Value ? "0" : Convert.ToString(dr["NUSERCODE"].ToString());
                        GestClieVM.NIDDOC_TYPE = dr["NIDDOC_TYPE"] == DBNull.Value ? "0" : Convert.ToString(dr["NIDDOC_TYPE"].ToString());
                        GestClieVM.SIDDOC = dr["SIDDOC"] == DBNull.Value ? "0" : Convert.ToString(dr["SIDDOC"].ToString());
                        GestClieVM.SFIRSTNAME = dr["SFIRSTNAME"] == DBNull.Value ? "0" : Convert.ToString(dr["SFIRSTNAME"].ToString());
                        GestClieVM.SLASTNAME = dr["SLASTNAME"] == DBNull.Value ? "0" : Convert.ToString(dr["SLASTNAME"].ToString());
                        GestClieVM.SLASTNAME2 = dr["SLASTNAME2"] == DBNull.Value ? "0" : Convert.ToString(dr["SLASTNAME2"].ToString());
                        GestClieVM.DBIRTHDAT = dr["DBIRTHDAT"] == DBNull.Value ? "0" : Convert.ToString(dr["DBIRTHDAT"].ToString());
                        GestClieVM.SSEXCLIEN = dr["SSEXCLIEN"] == DBNull.Value ? "0" : Convert.ToString(dr["SSEXCLIEN"].ToString());
                        GestClieVM.NID_REG = dr["NID_REG"] == DBNull.Value ? "0" : Convert.ToString(dr["NID_REG"].ToString());
                        gestclie.Add(GestClieVM);

                        //ENVIANDO DATOS A LA API 
                        DatosGestClie data = new DatosGestClie
                        {
                            P_CodAplicacion = "GESTORCLIENTE",
                            P_TipOper = "CON",
                            P_NUSERCODE = "1",
                            P_NIDDOC_TYPE = GestClieVM.NIDDOC_TYPE.ToString().Trim(),
                            P_SIDDOC = GestClieVM.SIDDOC.ToString().Trim()
                        };
                        DATA = JsonConvert.SerializeObject(data);
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://10.10.1.56/WSGestorFoto/Api/Cliente/GestionarCliente");
                        request.Method = "POST";
                        request.ContentType = "application/json";
                        request.ContentLength = DATA.Length;
                        StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
                        requestWriter.Write(DATA);
                        requestWriter.Flush();
                        requestWriter.Close();

                        try
                        {
                            WebResponse webResponse = request.GetResponse();
                            Stream webStream = webResponse.GetResponseStream();
                            StreamReader responseReader = new StreamReader(webStream);
                            string respuesta = responseReader.ReadToEnd();
                            var cliente = JsonConvert.DeserializeObject<Cliente>(respuesta);
                            if (cliente.P_NCODE == "1" || cliente.P_NCODE == "3")
                            {
                                response.cod_error = 0;
                                response.message = cliente.P_SMESSAGE;
                                //guardando en la matrizapi 
                                MatrizApi[filas, 1] = GestClieVM.NID_REG.ToString().Trim();
                                MatrizApi[filas, 2] = "Error en la trama: Doc. Ident. " + GestClieVM.SIDDOC.ToString().Trim() + ", " + cliente.P_SMESSAGE.Trim();
                                filas++;
                            }
                            else
                            {
                                string P_SNAME = cliente.EListClient[0].P_SFIRSTNAME.ToUpper().ToString().Trim();
                                string P_SLAST = cliente.EListClient[0].P_SLASTNAME.ToUpper().ToString().Trim();
                                string P_SLAST2 = cliente.EListClient[0].P_SLASTNAME2.ToUpper().ToString().Trim();
                                string P_BIRTHDAY = cliente.EListClient[0].P_DBIRTHDAT.ToString().Trim();
                                string P_SSEX = cliente.EListClient[0].P_SSEXCLIEN.ToString().Trim();
                                string P_CLIENTE_URL = P_SNAME + ' ' + P_SLAST + ' ' + P_SLAST2;
                                string P_CLIENTE_FILE = GestClieVM.SFIRSTNAME.ToUpper().ToString().Trim() + ' ' + GestClieVM.SLASTNAME.ToUpper().ToString().Trim() + ' ' + GestClieVM.SLASTNAME2.ToUpper().ToString().Trim();

                                //guardando en la matrizapi 
                                if (P_CLIENTE_URL != P_CLIENTE_FILE)
                                {
                                    MatrizApi[filas, 1] = GestClieVM.NID_REG.ToString().Trim();
                                    MatrizApi[filas, 2] = "Error en la trama: Doc. Ident. " + GestClieVM.SIDDOC.ToString().Trim() + " de " + P_CLIENTE_FILE + ", con registro: " + P_CLIENTE_URL;
                                    filas++;
                                }
                                if (GestClieVM.DBIRTHDAT.ToString().Trim() != P_BIRTHDAY)
                                {
                                    MatrizApi[filas, 1] = GestClieVM.NID_REG.ToString().Trim();
                                    MatrizApi[filas, 2] = "Error en la trama: Doc. Ident. " + GestClieVM.SIDDOC.ToString().Trim() + " de Fec. Nac. " + GestClieVM.DBIRTHDAT.ToString().Trim() + ", con registro: " + P_BIRTHDAY;
                                    filas++;
                                }


                            }

                            responseReader.Dispose();
                            responseReader.Close();
                        }
                        catch (Exception ex)
                        {
                            response.cod_error = 1;
                            response.message = ex.ToString();
                            LogControl.save("ValidarGestClie", ex.ToString(), "3");
                        }

                    }
                }
                //insercion de registros de errores 
                for (int x = 1; x < filas; x++)
                {
                    insertPdvalError(idproc, MatrizApi[x, 1].ToString(), MatrizApi[x, 2].ToString().Trim());
                }
                response.cod_error = 0;
                response.message = "Se realizó correctamente la consulta a la API GestionarCliente";
            }
            catch (Exception ex)
            {
                response.cod_error = 1;
                response.message = ex.ToString();
                LogControl.save("ValidarGestClie", ex.ToString(), "3");
            }
            return response;
        }

        public SalidaErrorBaseVM ConsultaAseguradoApi(string idproc)
        {
            SalidaErrorBaseVM response = new SalidaErrorBaseVM();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.sp_ConAsegApi;
            OracleDataReader dr = null;
            try
            {
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, idproc, ParameterDirection.Input));
                string[] arrayCursor = { "C_TABLE" };
                OracleParameter C_TABLE = new OracleParameter(arrayCursor[0], OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);
                dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storeprocedure, arrayCursor, parameter);
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        ErroresVM item = new ErroresVM();
                        item.REGISTRO = dr["REGISTRO"].ToString().Trim();
                        item.DESCRIPCION = dr["DESCRIPCION"].ToString().Trim();
                        response.errorList.Add(item);
                    }
                }
                response.P_COD_ERR = 0;
                response.P_MESSAGE = "Consulta Realizada Correctamente";
                ELog.CloseConnection(dr);
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = ex.ToString();
                response.errorList.Add(new ErroresVM
                {
                    REGISTRO = "0",
                    DESCRIPCION = ex.ToString()
                });
            }
            return response;
        }

        public SalidaTramaBaseVM InsertAseguradosBulk(List<aseguradoMas> asegurados, int primary)
        {
            var response = new SalidaTramaBaseVM();

            foreach (var item in asegurados)
            {
                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        try
                        {
                            OracleDataReader dr;

                            cmd.Connection = cn;
                            cmd.CommandText = primary == 0 ? "SP_INSERT_TBL_PD_CARGA_MAS_SCTR" : "SP_INSERT_TBL_PD_CARGA_MAS_SCTR_TRAMA";
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Varchar2, item.nid_cotizacion, ParameterDirection.Input));
                            cmd.Parameters.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, item.nid_proc, ParameterDirection.Input));
                            cmd.Parameters.Add(new OracleParameter("P_SFIRSTNAME", OracleDbType.Varchar2, item.sfirstname, ParameterDirection.Input));
                            cmd.Parameters.Add(new OracleParameter("P_SLASTNAME", OracleDbType.Varchar2, item.slastname, ParameterDirection.Input));
                            cmd.Parameters.Add(new OracleParameter("P_SLASTNAME2", OracleDbType.Varchar2, item.slastname2, ParameterDirection.Input));
                            cmd.Parameters.Add(new OracleParameter("P_NMODULEC", OracleDbType.Varchar2, item.nmodulec, ParameterDirection.Input));
                            cmd.Parameters.Add(new OracleParameter("P_NNATIONALITY", OracleDbType.Varchar2, item.nnationality, ParameterDirection.Input));
                            cmd.Parameters.Add(new OracleParameter("P_NIDDOC_TYPE", OracleDbType.Varchar2, item.niiddoc_type, ParameterDirection.Input));
                            cmd.Parameters.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, item.siddoc, ParameterDirection.Input));
                            cmd.Parameters.Add(new OracleParameter("P_DBIRTHDAT", OracleDbType.Varchar2, item.dbirthdat, ParameterDirection.Input));
                            cmd.Parameters.Add(new OracleParameter("P_NREMUNERACION", OracleDbType.Varchar2, item.nremuneracion, ParameterDirection.Input));
                            cmd.Parameters.Add(new OracleParameter("P_NIDCLIENTLOCATION", OracleDbType.Varchar2, item.nidclientlocation, ParameterDirection.Input));
                            cmd.Parameters.Add(new OracleParameter("P_NCOD_NETEO", OracleDbType.Varchar2, item.ncod_neteo, ParameterDirection.Input));
                            cmd.Parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Varchar2, item.nusercode, ParameterDirection.Input));
                            cmd.Parameters.Add(new OracleParameter("P_SSTATREGT", OracleDbType.Varchar2, item.sstatregt, ParameterDirection.Input));
                            cmd.Parameters.Add(new OracleParameter("P_SCOMMENT", OracleDbType.Varchar2, item.scomment, ParameterDirection.Input));
                            cmd.Parameters.Add(new OracleParameter("P_NID_REG", OracleDbType.Varchar2, item.nid_reg, ParameterDirection.Input));
                            cmd.Parameters.Add(new OracleParameter("P_SSEXCLIEN", OracleDbType.Varchar2, item.ssexclien, ParameterDirection.Input));
                            cmd.Parameters.Add(new OracleParameter("P_NACTION", OracleDbType.Varchar2, item.naction, ParameterDirection.Input));
                            cmd.Parameters.Add(new OracleParameter("P_SROLE", OracleDbType.Varchar2, item.srole, ParameterDirection.Input));
                            cmd.Parameters.Add(new OracleParameter("P_SCIVILSTA", OracleDbType.Varchar2, item.scivilsta, ParameterDirection.Input));
                            cmd.Parameters.Add(new OracleParameter("P_SSTREET", OracleDbType.Varchar2, item.sstreet, ParameterDirection.Input));
                            cmd.Parameters.Add(new OracleParameter("P_SPROVINCE", OracleDbType.Varchar2, item.sprovince, ParameterDirection.Input));
                            cmd.Parameters.Add(new OracleParameter("P_SLOCAL", OracleDbType.Varchar2, item.slocal, ParameterDirection.Input));
                            cmd.Parameters.Add(new OracleParameter("P_SMUNICIPALITY", OracleDbType.Varchar2, item.smunicipality, ParameterDirection.Input));
                            cmd.Parameters.Add(new OracleParameter("P_SE_MAIL", OracleDbType.Varchar2, item.se_mail, ParameterDirection.Input));
                            cmd.Parameters.Add(new OracleParameter("P_SPHONE_TYPE", OracleDbType.Varchar2, item.sphone_type, ParameterDirection.Input));
                            cmd.Parameters.Add(new OracleParameter("P_SPHONE", OracleDbType.Varchar2, item.sphone, ParameterDirection.Input));
                            cmd.Parameters.Add(new OracleParameter("P_NIDDOC_TYPE_BENEFICIARY", OracleDbType.Varchar2, item.niddoc_type_beneficiary, ParameterDirection.Input));
                            cmd.Parameters.Add(new OracleParameter("P_SIDDOC_BENEFICIARY", OracleDbType.Varchar2, item.siddoc_beneficiary, ParameterDirection.Input));
                            cmd.Parameters.Add(new OracleParameter("P_TYPE_PLAN", OracleDbType.Varchar2, item.type_plan, ParameterDirection.Input));
                            cmd.Parameters.Add(new OracleParameter("P_PERCEN_PARTICIPATION", OracleDbType.Varchar2, item.percen_participation, ParameterDirection.Input));
                            cmd.Parameters.Add(new OracleParameter("P_SRELATION", OracleDbType.Varchar2, item.srelation, ParameterDirection.Input));
                            cmd.Parameters.Add(new OracleParameter("P_SAPE_CASADA", OracleDbType.Varchar2, null, ParameterDirection.Input));
                            cmd.Parameters.Add(new OracleParameter("P_SLASTNAME2CONCAT", OracleDbType.Varchar2, null, ParameterDirection.Input));
                            cmd.Parameters.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, null, ParameterDirection.Input));
                            cmd.Parameters.Add(new OracleParameter("P_NCERTIF_CLI", OracleDbType.Varchar2, null, ParameterDirection.Input));
                            cmd.Parameters.Add(new OracleParameter("P_SCREDITNUM", OracleDbType.Varchar2, null, ParameterDirection.Input));
                            cmd.Parameters.Add(new OracleParameter("P_DINIT_CRE", OracleDbType.Varchar2, null, ParameterDirection.Input));
                            cmd.Parameters.Add(new OracleParameter("P_DEND_CRE", OracleDbType.Varchar2, null, ParameterDirection.Input));
                            cmd.Parameters.Add(new OracleParameter("P_NAMOUNT_CRE", OracleDbType.Varchar2, null, ParameterDirection.Input));
                            cmd.Parameters.Add(new OracleParameter("P_NAMOUNT_ACT", OracleDbType.Varchar2, null, ParameterDirection.Input));
                            cmd.Parameters.Add(new OracleParameter("P_NQ_QUOT", OracleDbType.Varchar2, null, ParameterDirection.Input));
                            cmd.Parameters.Add(new OracleParameter("P_NTYPPREMIUM", OracleDbType.Varchar2, null, ParameterDirection.Input));
                            cmd.Parameters.Add(new OracleParameter("P_NPREMIUMN", OracleDbType.Varchar2, null, ParameterDirection.Input));
                            cmd.Parameters.Add(new OracleParameter("P_NIGV", OracleDbType.Varchar2, null, ParameterDirection.Input));
                            cmd.Parameters.Add(new OracleParameter("P_NDE", OracleDbType.Varchar2, null, ParameterDirection.Input));
                            cmd.Parameters.Add(new OracleParameter("P_NPREMIUM", OracleDbType.Varchar2, null, ParameterDirection.Input));
                            cmd.Parameters.Add(new OracleParameter("P_NIDDOC_TYPE_CONT", OracleDbType.Varchar2, item.niddoc_type_cont, ParameterDirection.Input));
                            cmd.Parameters.Add(new OracleParameter("P_SIDDOC_CONT", OracleDbType.Varchar2, item.siddoc_cont, ParameterDirection.Input));
                            cmd.Parameters.Add(new OracleParameter("P_NGRADO", OracleDbType.Varchar2, item.ngrado, ParameterDirection.Input));

                            var P_COD_ERR = new OracleParameter("P_ES_ERROR", OracleDbType.Int32, ParameterDirection.Output);
                            var P_MESSAGE = new OracleParameter("P_ERROR_MSG", OracleDbType.Varchar2, ParameterDirection.Output);
                            P_MESSAGE.Size = 4000;

                            cmd.Parameters.Add(P_COD_ERR);
                            cmd.Parameters.Add(P_MESSAGE);

                            cn.Open();
                            dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                            dr.Close();

                            response.P_COD_ERR = Convert.ToString(P_COD_ERR.Value.ToString());
                            response.P_MESSAGE = Convert.ToString(P_MESSAGE.Value.ToString());
                        }
                        catch (Exception ex)
                        {
                            response.P_COD_ERR = "1";
                            response.P_MESSAGE = ex.ToString();
                            LogControl.save("InsertAseguradosBulk", ex.ToString(), "3");
                        }
                        finally
                        {
                            if (cn.State == ConnectionState.Open) cn.Close();
                        }
                    }
                }
            }

            return response;
        }

        public async Task<QuotationResponseVM> updateCotizacionClienteEstado(QuotationCabBM request)
        {
            QuotationResponseVM response = null;
            DbConnection DataConnection = ConnectionGet(enuTypeDataBase.OracleVTime);
            DbTransaction trx = null;
            var NID_COTIZACION = request.NumeroCotizacion.Value;
            try
            {
                var count = new PolicyDA().GetPolizaEmitDet(NID_COTIZACION, request.P_NUSERCODE).Count(); // si existe data actualiza sino inserta

                DataConnection.Open();
                trx = DataConnection.BeginTransaction();

                if (request.NumeroCotizacion != null && request.NumeroCotizacion.Value != 0)
                {
                    response = UpdateQuotationCab(request, DataConnection, trx);

                    response.P_NID_COTIZACION = NID_COTIZACION.ToString();


                    if (response.P_COD_ERR == 0)
                    {
                        if (request.QuotationDet != null)
                        {
                            foreach (var item in request.QuotationDet)
                            {

                                if (request.P_SPOL_MATRIZ == 1 && response.P_COD_ERR == 0)
                                {
                                    request.P_DSTARTDATE_ASE = null; // el sp lo sacara de la cabecera de cotizacion
                                    if (count > 0)
                                    {
                                        item.P_NID_COTIZACION = NID_COTIZACION;

                                        response = UpdateQuotationDetRC(request, item, DataConnection, trx);
                                    }

                                    if (count == 0)
                                    {
                                        item.P_NID_COTIZACION = NID_COTIZACION;
                                        response = InsertQuotationDet(request, item, DataConnection, trx);

                                    }
                                }
                            }
                        }
                    }


                }

                if (response.P_COD_ERR == 0 && response.P_NID_COTIZACION != "0")
                {
                    trx.Commit();
                    trx.Dispose();
                    DataConnection.Close();
                }
                else
                {
                    if (trx.Connection != null)
                    {
                        trx.Rollback();
                        trx.Dispose();
                    }
                    DataConnection.Close();
                    DataConnection.Dispose();
                }

            }
            catch (Exception ex)
            {
                response.P_NCODE = 1;
                response.P_SMESSAGE = ex.ToString();
                if (trx.Connection != null) trx.Rollback();
                if (trx.Connection != null) trx.Dispose();
                DataConnection.Close();
                DataConnection.Dispose();
                LogControl.save("updateCotizacionClienteEstado", ex.ToString(), "3");
            }
            return await Task.FromResult(response);
        }

        public EmitPolVM finalizarCotizacionEstadoVL(ProcesaTramaBM request)
        {
            var sPackageName = ProcedureName.pkg_Cotizacion + ".SP_COTIZACON_ESTADO_FINALIZAR";

            List<OracleParameter> parameter = new List<OracleParameter>();
            var result = new EmitPolVM();
            DbConnection DataConnection = ConnectionGet(enuTypeDataBase.OracleVTime);
            DbTransaction trx = null;
            try
            {
                DataConnection.Open();
                trx = DataConnection.BeginTransaction();

                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, Convert.ToInt32(request.nrocotizacion), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, Convert.ToInt32(request.nbranch), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, Convert.ToInt32(request.nproducto), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.idproc, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, Convert.ToInt32(request.usercode), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DSTARTDATE", OracleDbType.Date, Convert.ToDateTime(request.fechaini_poliza), ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, result.message, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, result.cod_error, ParameterDirection.Output);

                P_COD_ERR.Size = 9000;
                P_MESSAGE.Size = 9000;
                parameter.Add(P_MESSAGE);
                parameter.Add(P_COD_ERR);


                this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, DataConnection, trx);

                result.cod_error = Convert.ToInt32(P_COD_ERR.Value.ToString());
                result.message = P_MESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                result.cod_error = 1;
                result.message = ex.ToString();
            }
            finally
            {
                if (result.cod_error == 0)
                {
                    trx.Commit();
                }
                else
                {
                    if (trx.Connection != null) trx.Rollback();
                }
                if (trx.Connection != null) trx.Dispose();
                ELog.CloseConnection(DataConnection);
            }

            return result;
        }

        public SalidaErrorBaseVM updateCargaExclusion(ValidaTramaBM validaTramaBM)
        {
            var sPackageName = ProcedureName.sp_UpdateExlusion;
            var response = new SalidaErrorBaseVM();
            List<OracleParameter> parameters = new List<OracleParameter>();

            DbConnection connection = ConnectionGet(enuTypeDataBase.OracleVTime);
            DbTransaction trx = null;
            connection.Open();
            trx = connection.BeginTransaction();

            try
            {

                parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int64, validaTramaBM.NID_COTIZACION, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, validaTramaBM.NID_PROC, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Varchar2, validaTramaBM.NTYPE_TRANSAC, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Varchar2, validaTramaBM.P_NBRANCH, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Varchar2, validaTramaBM.P_NPRODUCT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Date, Convert.ToDateTime(validaTramaBM.DEFFECDATE), ParameterDirection.Input));

                // OUTPUT
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, 500, response.P_MESSAGE, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, response.P_COD_ERR, ParameterDirection.Output);
                parameters.Add(P_MESSAGE);
                parameters.Add(P_COD_ERR);

                this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameters, connection, trx);
                response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                response.P_MESSAGE = P_MESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = ex.ToString();
                LogControl.save("updateCargaExclusion", ex.ToString(), "3");
            }
            finally
            {
                if (response.P_COD_ERR == 0)
                {
                    trx.Commit();
                }
                else
                {
                    if (trx.Connection != null) trx.Rollback();
                }
                if (trx.Connection != null) trx.Dispose();
                ELog.CloseConnection(connection);
            }
            return response;
        }

        public validateBrokerVL validateBroker(validateBrokerVL data)
        {
            var sPackageName = ProcedureName.pkg_Cotizacion + ".REA_VALIDACION_BROKER";
            var response = new validateBrokerVL();
            List<OracleParameter> parameters = new List<OracleParameter>();

            DbConnection connection = ConnectionGet(enuTypeDataBase.OracleVTime);
            DbTransaction trx = null;
            connection.Open();
            trx = connection.BeginTransaction();

            try
            {

                parameters.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, data.P_SCLIENT, ParameterDirection.Input));

                // OUTPUT
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, 500, response.P_MESSAGE, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, response.P_COD_ERR, ParameterDirection.Output);
                OracleParameter P_FLAG_BROKER = new OracleParameter("P_COD_ERR", OracleDbType.Int32, response.P_FLAG_BROKER, ParameterDirection.Output);
                parameters.Add(P_MESSAGE);
                parameters.Add(P_COD_ERR);
                parameters.Add(P_FLAG_BROKER);

                this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameters, connection, trx);

                response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                response.P_MESSAGE = P_MESSAGE.Value.ToString();
                response.P_FLAG_BROKER = Convert.ToInt32(P_FLAG_BROKER.Value.ToString());

                ELog.CloseConnection(connection);
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = ex.ToString();
            }
            return response;
        }

        public DeleteIgv DeleteIgv(validaTramaVM data)
        {
            var response = new DeleteIgv();

            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        OracleDataReader dr;

                        cmd.Connection = cn;
                        cmd.CommandText = ProcedureName.pkg_ReaDataAP + ".SP_GET_PREMIUM_SIN_IGV";
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Varchar2, data.datosPoliza.branch.ToString(), ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Varchar2, data.datosPoliza.codTipoProducto.ToString(), ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NTYPE_PRODUCT", OracleDbType.Int32, data.datosPoliza.codTipoNegocio, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NTYPE_PROFILE", OracleDbType.Int32, data.datosPoliza.codTipoPerfil, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NTIPO_FACTURACION", OracleDbType.Int32, data.datosPoliza.codTipoFacturacion, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Int32, data.type_mov == null ? null : data.type_mov == "0" ? "1" : data.type_mov, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NPREMIUMN", OracleDbType.Double, data.premium, ParameterDirection.Input));

                        var P_NPREMIUMN = new OracleParameter("P_NPREMIUMN", OracleDbType.Decimal, ParameterDirection.Output);
                        var P_NIGV = new OracleParameter("P_NIGV", OracleDbType.Decimal, ParameterDirection.Output);
                        var P_NDE = new OracleParameter("P_NDE", OracleDbType.Decimal, ParameterDirection.Output);
                        var P_NPREMIUM = new OracleParameter("P_NPREMIUM", OracleDbType.Decimal, ParameterDirection.Output);
                        var P_NERROR = new OracleParameter("P_NERROR", OracleDbType.Int32, ParameterDirection.Output);


                        cmd.Parameters.Add(P_NPREMIUMN);
                        cmd.Parameters.Add(P_NIGV);
                        cmd.Parameters.Add(P_NDE);
                        cmd.Parameters.Add(P_NPREMIUM);
                        cmd.Parameters.Add(P_NERROR);

                        cn.Open();
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        response.ncod_error = Convert.ToInt32(P_NERROR.Value.ToString());

                        if (response.ncod_error == 0)
                        {
                            response.npremiumn = Convert.ToDouble(P_NPREMIUMN.Value.ToString().Replace(",", "."));
                            response.nigv = Convert.ToDouble(P_NIGV.Value.ToString().Replace(",", "."));
                            response.nde = Convert.ToDouble(P_NDE.Value.ToString().Replace(",", "."));
                            response.npremium = Convert.ToDouble(P_NPREMIUM.Value.ToString().Replace(",", "."));
                        }
                        else
                        {
                            response.smenssage = "Hubo un error al quitar el igv a la prima del tarifario";
                            LogControl.save("DeleteIgv", response.smenssage, "3");

                        }

                        ELog.CloseConnection(dr);
                    }
                    catch (Exception ex)
                    {
                        response.ncod_error = 1;
                        response.smenssage = ex.ToString();
                        LogControl.save("DeleteIgv", ex.ToString(), "3");
                    }
                    finally
                    {
                        if (cn.State == ConnectionState.Open) cn.Close();
                    }
                }
            }

            return response;
        }

        public GenericResponseEPS UpdateCotizacionState(SendStateEPS cotizacion) //AVS - INTERCONEXION SABSA 18/10/2023 
        {
            var response = new GenericResponseEPS();
            List<OracleParameter> parameters = new List<OracleParameter>();
            string storeprocedure = ProcedureName.pkg_EPS + ".SP_UPD_SSTATREGT_COTIZACION";
            OracleDataReader odri = null;

            try
            {
                parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int64, cotizacion.COTIZACION, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, cotizacion.RAMO, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SSTATREGT", OracleDbType.Int32, cotizacion.ESTADO, ParameterDirection.Input));


                // OUTPUT
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, 500, response.StatusCode, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, response.Message, ParameterDirection.Output);
                parameters.Add(P_MESSAGE);
                parameters.Add(P_COD_ERR);

                odri = (OracleDataReader)this.ExecuteByStoredProcedureVT(storeprocedure, parameters);

                response.StatusCode = Convert.ToInt32(P_COD_ERR.Value.ToString());
                response.Message = P_MESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                response.StatusCode = 1;
                response.Message = ex.ToString();
            }
            return response;
        }

        public double GetIGV(validaTramaVM data)
        {
            double igvConfig = 0;

            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        OracleDataReader dr;

                        cmd.Connection = cn;
                        cmd.CommandText = ProcedureName.sp_GetIgv;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Varchar2, data.datosPoliza.branch.ToString(), ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Varchar2, data.datosPoliza.codTipoProducto.ToString(), ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_STYPE_TRANSAC", OracleDbType.Varchar2, data.datosPoliza.trxCode, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NTYPE_PRODUCT", OracleDbType.Int32, data.datosPoliza.codTipoNegocio, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NTYPE_PROFILE", OracleDbType.Int32, data.datosPoliza.codTipoPerfil, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NTIPO_FACTURACION", OracleDbType.Int32, data.datosPoliza.codTipoFacturacion, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, data.codUsuario, ParameterDirection.Input));

                        var P_NIGV = new OracleParameter("P_NIGV", OracleDbType.Double, ParameterDirection.Output);

                        //P_MESSAGE.Size = 4000;

                        cmd.Parameters.Add(P_NIGV);
                        cn.Open();
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        igvConfig = Convert.ToDouble(P_NIGV.Value.ToString());

                        ELog.CloseConnection(dr);
                    }
                    catch (Exception ex)
                    {
                        igvConfig = 0;
                        LogControl.save("GetIGV", ex.ToString(), "3");
                    }
                    finally
                    {
                        if (cn.State == ConnectionState.Open) cn.Close();
                    }
                }
            }

            return igvConfig;
        }

        public double GetIGVGraph(genericTechnicalGraph data, string nproduct, string profile)
        {
            double igvConfig = 0;

            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        OracleDataReader dr;

                        cmd.Connection = cn;
                        cmd.CommandText = ProcedureName.sp_GetIgv;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Varchar2, data.nbranch.ToString(), ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Varchar2, nproduct, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_STYPE_TRANSAC", OracleDbType.Varchar2, data.stransac, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NTYPE_PRODUCT", OracleDbType.Int32, data.dataCotizacion.policyType, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NTYPE_PROFILE", OracleDbType.Int32, profile, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NTIPO_FACTURACION", OracleDbType.Int32, data.dataCotizacion.billingType, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, data.dataCotizacion.userCode, ParameterDirection.Input));

                        var P_NIGV = new OracleParameter("P_NIGV", OracleDbType.Double, ParameterDirection.Output);

                        //P_MESSAGE.Size = 4000;

                        cmd.Parameters.Add(P_NIGV);
                        cn.Open();
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        igvConfig = Convert.ToDouble(P_NIGV.Value.ToString());

                        ELog.CloseConnection(dr);
                    }
                    catch (Exception ex)
                    {
                        igvConfig = 0;
                        LogControl.save("GetIGV", ex.ToString(), "3");
                    }
                    finally
                    {
                        if (cn.State == ConnectionState.Open) cn.Close();
                    }
                }
            }

            return igvConfig;
        }

        public rea_Broker GetBrokerAgenciadoSCTR(string P_SCLIENT, int P_TIPO)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storedProcedureName = ProcedureName.pkg_Cotizacion + ".REA_GET_BROKER_SCTR";
            rea_Broker response = new rea_Broker();
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, P_SCLIENT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, P_TIPO, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);
                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        var item = new rea_Broker();
                        item.NTYPECHANNEL = odr["NTYPECHANNEL"] == DBNull.Value ? string.Empty : odr["NTYPECHANNEL"].ToString();
                        item.COD_CANAL = odr["COD_CANAL"] == DBNull.Value ? string.Empty : odr["COD_CANAL"].ToString();
                        item.NNUMDOC = odr["NNUMDOC"] == DBNull.Value ? string.Empty : odr["NNUMDOC"].ToString();
                        item.SCLIENAME = odr["SCLIENAME"] == DBNull.Value ? string.Empty : odr["SCLIENAME"].ToString();
                        item.SCLIENT = odr["SCLIENT"] == DBNull.Value ? string.Empty : odr["SCLIENT"].ToString();
                        item.NINTERTYP = odr["NINTERTYP"] == DBNull.Value ? string.Empty : odr["NINTERTYP"].ToString();
                        item.NTIPDOC = odr["NTIPDOC"] == DBNull.Value ? string.Empty : odr["NTIPDOC"].ToString();
                        item.SFIRSTNAME = odr["SFIRSTNAME"] == DBNull.Value ? string.Empty : odr["SFIRSTNAME"].ToString();
                        item.SLASTNAME = odr["SLASTNAME"] == DBNull.Value ? string.Empty : odr["SLASTNAME"].ToString();
                        item.SLASTNAME2 = odr["SLASTNAME2"] == DBNull.Value ? string.Empty : odr["SLASTNAME2"].ToString();

                        item.SSTREET = odr["SSTREET"] == DBNull.Value ? string.Empty : odr["SSTREET"].ToString();
                        item.SPHONE1 = odr["SPHONE1"] == DBNull.Value ? string.Empty : odr["SPHONE1"].ToString();
                        item.SEMAILCLI = odr["SEMAILCLI"] == DBNull.Value ? string.Empty : odr["SEMAILCLI"].ToString();
                        item.NINTERMED = odr["NINTERMED"] == DBNull.Value ? string.Empty : odr["NINTERMED"].ToString();
                        item.NMUNICIPALITY = odr["NMUNICIPALITY"] == DBNull.Value ? string.Empty : odr["NMUNICIPALITY"].ToString();

                        response = item;
                    }
                }

                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("GetBrokerAgenciadoSCTR", ex.ToString(), "3");
            }

            return response;
        }

        public string equivalentINEI(Int64 municipality)
        {
            var sPackageName = ProcedureName.pkg_Cotizacion + ".OBT_EQUIMUNICIPALITYINEI_SCTR";
            string result = "";
            List<OracleParameter> parameters = new List<OracleParameter>();
            try
            {
                parameters.Add(new OracleParameter("P_NMUNICIPALITY", OracleDbType.Int64, municipality, ParameterDirection.Input));

                OracleParameter P_COD_UBI_CLI = new OracleParameter("P_COD_INEI", OracleDbType.Varchar2, result, ParameterDirection.Output);

                P_COD_UBI_CLI.Size = 200;

                parameters.Add(P_COD_UBI_CLI);
                this.ExecuteByStoredProcedureVT(sPackageName, parameters);
                result = P_COD_UBI_CLI.Value.ToString();
            }
            catch (Exception ex)
            {
                result = null;
                LogControl.save("equivalentINEI", ex.ToString(), "3");
            }
            return result;
        }

        public rmv_ACT obtRMV(string P_DATE)
        {
            rmv_ACT response = new rmv_ACT();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storedProcedureName = ProcedureName.pkg_Cotizacion + ".OBT_RMV";

            try
            {
                parameter.Add(new OracleParameter("P_DATE", OracleDbType.Varchar2, P_DATE, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);
                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        var item = new rmv_ACT();
                        item.P_RMV = odr["RMV"] != DBNull.Value ? Convert.ToDouble(odr["RMV"]) : 0.0;
                        item.P_RMV_MITAD = odr["RMV_MITAD"] != DBNull.Value ? Convert.ToDouble(odr["RMV_MITAD"]) : 0.0;
                        response = item;
                    }

                }

                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("obtRMV", ex.ToString(), "3");
            }
            return response;
        }

        public string insert_quotation_EPS(int cotizacion, int idCotizacion, string mensaje, string sjsonCOT, int idEstaddo)
        {
            var sPackageName = ProcedureName.pkg_EPS + ".SP_INS_QUOTATION_EPS";
            string result = "";
            List<OracleParameter> parameters = new List<OracleParameter>();

            try
            {
                parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int64, cotizacion, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NID_COTIZACION_EPS", OracleDbType.Int64, idCotizacion, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_MENSAJE_TR", OracleDbType.Varchar2, mensaje, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SJSON_COT", OracleDbType.Varchar2, sjsonCOT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_ESTADO_REL", OracleDbType.Int64, idEstaddo, ParameterDirection.Input));

                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, result, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Varchar2, result, ParameterDirection.Output);

                P_MESSAGE.Size = 200;
                P_COD_ERR.Size = 200;

                parameters.Add(P_MESSAGE);
                parameters.Add(P_COD_ERR);

                this.ExecuteByStoredProcedureVT(sPackageName, parameters);

                result = P_MESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                result = null;
                LogControl.save("insert_quotation_EPS", ex.ToString(), "3");
            }
            return result;
        }

        public List<listComisionesTecnica> GetComisionTecnica(string P_SCLIENT)
        {
            List<listComisionesTecnica> response = new List<listComisionesTecnica>();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storedProcedureName = ProcedureName.pkg_EPS + ".SP_GET_COMISSION";

            try
            {
                parameter.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, P_SCLIENT, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);
                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        var item = new listComisionesTecnica();
                        item.NPRODUCT = odr["NPRODUCT"] != DBNull.Value ? Convert.ToInt32(odr["NPRODUCT"]) : 0;
                        item.NCOMISION_AUT = odr["NCOMISION_AUT"] != DBNull.Value ? Convert.ToDecimal(odr["NCOMISION_AUT"]) : 0;
                        response.Add(item);
                    }

                }

                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("GetComisionTecnica", ex.ToString(), "3");
            }
            return response;
        }

        public List<listtasastecnica> GetTasastecnica(string P_SCLIENT)
        {
            List<listtasastecnica> response = new List<listtasastecnica>();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storedProcedureName = ProcedureName.pkg_EPS + ".SP_GET_TASAS";

            try
            {
                parameter.Add(new OracleParameter("P_DATE", OracleDbType.Varchar2, P_SCLIENT, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);
                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        var item = new listtasastecnica();
                        item.NMODULEC = odr["NMODULEC"] != DBNull.Value ? Convert.ToInt32(odr["NMODULEC"]) : 0;
                        item.NTASA_AUTOR = odr["NTASA_AUTOR"] != DBNull.Value ? Convert.ToDecimal(odr["NTASA_AUTOR"]) : 0;
                        item.NPRODUCT = odr["NPRODUCT"] != DBNull.Value ? Convert.ToInt32(odr["NPRODUCT"]) : 0;
                        response.Add(item);
                    }

                }

                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("GetTasastecnica", ex.ToString(), "3");
            }
            return response;
        }

        public ClienteEstado GetEstadoClienteNuevo(string P_SCLIENT)
        {
            ClienteEstado result = new ClienteEstado();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storedProcedureName = ProcedureName.pkg_EPS + ".SP_GET_CLIENTES_NUEVOS";

            try
            {
                parameter.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, P_SCLIENT, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_ESTADO = new OracleParameter("P_ESTADO", OracleDbType.Int32, result.P_ESTADO, ParameterDirection.Output);
                P_ESTADO.Size = 200;

                parameter.Add(P_ESTADO);

                this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                result.P_ESTADO = Convert.ToInt32(P_ESTADO.Value.ToString());
            }
            catch (Exception ex)
            {
                LogControl.save("GetTasastecnica", ex.ToString(), "3");
            }

            return result;
        }

        public async Task<SJSON_CIP> getSJSON_CIP(CipRelanzamiento data)
        {
            SJSON_CIP response = new SJSON_CIP();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storedProcedureName = ProcedureName.pkg_EPS + ".SP_ACT_RELANZAMIENTO_CIP";

            try
            {
                parameter.Add(new OracleParameter("P_NCIP", OracleDbType.Varchar2, data.codeCip, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FLAG", OracleDbType.Int32, data.flagProducto, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_MIXTA", OracleDbType.Int32, data.flagNcotMixta, ParameterDirection.Input));


                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);
                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        var item = new SJSON_CIP();
                        item.sjson = odr["SJSON_CIP"] != DBNull.Value ? odr["SJSON_CIP"].ToString() : "";
                        response.sjson = item.sjson;
                    }
                }

                ELog.CloseConnection(odr);
                response.ncode = response.sjson != null ? 0 : 1;
                response.mensaje = "Se obtuvo la información.";
            }
            catch (Exception ex)
            {
                LogControl.save("getSJSON_CIP", ex.ToString(), "3");
            }
            return await Task.FromResult(response);
        }

        public async Task<SJSON_CIP> getSJSON_LINK(LinkRelanzamiento data)
        {
            SJSON_CIP response = new SJSON_CIP();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storedProcedureName = ProcedureName.pkg_EPS + ".SP_ACT_RELANZAMIENTO_LINK";

            try
            {
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, data.P_NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCIP", OracleDbType.Varchar2, data.P_SCIP, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STIPO_PAGO", OracleDbType.Varchar2, data.P_STIPO_PAGO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMIXTA", OracleDbType.Int32, data.P_NMIXTA, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);
                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        var item = new SJSON_CIP();
                        item.sjson = odr["SJSON_CIP"] != DBNull.Value ? odr["SJSON_CIP"].ToString() : "";
                        response.sjson = item.sjson;
                    }
                }

                ELog.CloseConnection(odr);
                response.ncode = response.sjson != null ? 0 : 1;
                response.mensaje = "Se obtuvo la información.";
            }
            catch (Exception ex)
            {
                LogControl.save("getSJSON_LINK", ex.ToString(), "3");
            }
            return await Task.FromResult(response);
        }

        public async Task<nstatesEPS> getNstatesEPS(int cotizacion)
        {
            nstatesEPS response = new nstatesEPS();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storedProcedureName = ProcedureName.pkg_EPS + ".SP_GET_ESTADOS_CABECERA_EPS";

            try
            {
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, cotizacion, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);
                //OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT_TRX(storedProcedureName, parameter, connection, trx);


                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        var item = new nstatesEPS();
                        item.nstateEPS = odr["N_STATE_EPS"] != DBNull.Value ? Convert.ToInt32(odr["N_STATE_EPS"]) : 0;
                        item.nstateSCTR = odr["N_STATE_SCTR"] != DBNull.Value ? Convert.ToInt32(odr["N_STATE_SCTR"]) : 0;
                        response = item;
                    }
                }

                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                LogControl.save("getNstatesEPS", ex.ToString(), "3");
            }
            return await Task.FromResult(response);
        }
        public async Task saveQuotationEps(QuotationResponseVM response, QuotationCabBM cotizacion, dataQuotation_EPS request_EPS)
        {

            var resp = Task.Run(async () =>
            {
                var URI_SendQuotation_EPS = ELog.obtainConfig("urlCotizacionEPS_SCTR");
                int nuevaCotizacion = (int)Convert.ToInt64(response.P_NID_COTIZACION);
                var estadosEPS = await new QuotationDA().getNstatesEPS(Convert.ToInt32(response.P_NID_COTIZACION));
                response.P_NSTATE_EPS = estadosEPS.nstateEPS;
                response.P_NSTATE_SCTR = estadosEPS.nstateSCTR;
                response.P_NCOT_MIXTA = cotizacion.P_NCOT_MIXTA;

                request_EPS.codigoCotizacion = response.P_NID_COTIZACION;
                request_EPS.fechaRegistro = DateTime.Now.ToString("yyyy-MM-dd");
                request_EPS.codigoEstadoCotizacion = response.P_SAPROBADO == "S" ? "9" : "1";

                new QuotationDA().InsertLog(Convert.ToInt64(response.P_NID_COTIZACION), "01 - SE ENVIA JSON A EPS - COTIZACION", URI_SendQuotation_EPS, JsonConvert.SerializeObject(request_EPS), null);

                var json = await new QuotationDA().invocarServicio_EPS(nuevaCotizacion, JsonConvert.SerializeObject(request_EPS));

                var responseObject = JsonConvert.DeserializeObject<dynamic>(json);
                bool isSuccess = responseObject == null ? false : responseObject.success == null ? false : responseObject.success;
                int idCotizacion = isSuccess == false ? 0 : responseObject.data.idCotizacion;
                //string mensaje = isSuccess == false ? "SE CAYO SERVICIO DE LA EPS" : "SE CREO LA COTIZACION EPS";
                string mensaje = "SE CREO LA COTIZACION SCTR";
                int idEstado = idCotizacion == 0 ? 0 : 1;
                var insEPS = new QuotationDA().insert_quotation_EPS(int.Parse(response.P_NID_COTIZACION), idCotizacion, mensaje, JsonConvert.SerializeObject(request_EPS), idEstado);
            });
        }

        public QuotationResponseVM ValSCTRRetroactividad(QuotationCabBM request, int cotizacion = 0, DbConnection connection = null, DbTransaction trx = null)
        {
            var sPackageName = ProcedureName.pkg_ValidaReglas + ".SPS_VAL_RETROACTIVIDAD";
            List<OracleParameter> parameter = new List<OracleParameter>();
            QuotationResponseVM result = new QuotationResponseVM();

            try
            {

                //INPUT
                string trans = request.TrxCode = request.TrxCode ?? "EM";
                int idtrans = request.TrxCode == "RE" ? 4 : 4;

                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, cotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_MOVEMENT", OracleDbType.Int32, idtrans, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Date, (new string[] { "DE", "IN", "RE", "EX", "EN" }).Contains(trans) ? request.P_DSTARTDATE_ASE : request.P_DSTARTDATE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_MES_ADEL_O_VENCI", OracleDbType.Varchar2, null, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_SASIGNAR = new OracleParameter("P_SASIGNAR", OracleDbType.Varchar2, result.P_SASIGNAR, ParameterDirection.Output);
                OracleParameter P_SAPROBADO = new OracleParameter("P_SAPROBADO", OracleDbType.Varchar2, result.P_SAPROBADO, ParameterDirection.Output);
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, result.P_NCODE, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, result.P_SMESSAGE, ParameterDirection.Output);

                P_SASIGNAR.Size = 9000;
                P_SAPROBADO.Size = 9000;
                P_SMESSAGE.Size = 9000;

                parameter.Add(P_SASIGNAR);
                parameter.Add(P_SAPROBADO);
                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);

                if (connection == null)
                {
                    this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                    result.P_SASIGNAR = P_SASIGNAR.Value.ToString().Trim();
                    result.P_SAPROBADO = P_SAPROBADO.Value.ToString().Trim();
                    result.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                    result.P_SMESSAGE = P_SMESSAGE.Value.ToString().Trim();
                }
                else
                {
                    this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, connection, trx);
                    result.P_SASIGNAR = P_SASIGNAR.Value.ToString().Trim();
                    result.P_SAPROBADO = P_SAPROBADO.Value.ToString().Trim();
                    result.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                    result.P_SMESSAGE = P_SMESSAGE.Value.ToString().Trim();
                }

            }
            catch (Exception ex)
            {
                result.P_NCODE = 4;
                result.P_COD_ERR = 1;
                result.P_MESSAGE = "Error al consultar retroactividad - verifique la configuracion registrada para la transaccion.";
                result.P_SMESSAGE = "Error al consultar retroactividad - verifique la configuracion registrada para la transaccion.";
                LogControl.save("ValSCTRRetroactividad", ex.ToString(), "3");
            }

            return result;
        }

        public QuotationResponseVM EnvioBandejaTR(QuotationCabBM request, QuotationResponseVM data, int cotizacion = 0, DbConnection connection = null, DbTransaction trx = null)
        {
            var sPackageName = ProcedureName.pkg_ValidaReglas + ".SPS_ENVIO_BANDEJA_TR";
            List<OracleParameter> parameter = new List<OracleParameter>();
            QuotationResponseVM result = new QuotationResponseVM();

            try
            {

                //INPUT
                string trans = request.TrxCode = request.TrxCode ?? "EM";
                int idtrans = request.TrxCode == "RE" ? 4 : 4;

                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, cotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, request.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Int32, idtrans, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.CodigoProceso, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOMMENT", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SASIGNAR", OracleDbType.Varchar2, data.P_SASIGNAR.Trim(), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SAPROBADO", OracleDbType.Varchar2, data.P_SAPROBADO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FACT_MES_VENCIDO", OracleDbType.Int32, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SFLAG_FAC_ANT", OracleDbType.Int32, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOLTIMRE", OracleDbType.Int32, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPAYFREQ", OracleDbType.Int32, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NAMO_AFEC", OracleDbType.Int32, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIVA", OracleDbType.Int32, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NAMOUNT", OracleDbType.Int32, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SRUTA", OracleDbType.Varchar2, null, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, result.P_NCODE, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, result.P_SMESSAGE, ParameterDirection.Output);
                OracleParameter P_NCONSTANCIA = new OracleParameter("P_NCONSTANCIA", OracleDbType.Int32, result.P_NCONSTANCIA, ParameterDirection.Output);

                P_SMESSAGE.Size = 9000;

                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);
                parameter.Add(P_NCONSTANCIA);

                if (connection == null)
                {
                    this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                    result.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                    result.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                    result.P_NCONSTANCIA = Convert.ToInt32(P_NCODE.Value.ToString());
                    result.P_SAPROBADO = result.P_NCODE == 2 ? "N" : "S";
                }
                else
                {
                    this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, connection, trx);
                    result.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                    result.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                    result.P_NCONSTANCIA = Convert.ToInt32(P_NCODE.Value.ToString());
                    result.P_SAPROBADO = result.P_NCODE == 2 ? "N" : "S";
                }

            }
            catch (Exception ex)
            {
                result.P_NCODE = 4;
                result.P_COD_ERR = 1;
                result.P_MESSAGE = "Error al consultar retroactividad - verifique la configuracion registrada para la transaccion.";
                result.P_SMESSAGE = "Error al consultar retroactividad - verifique la configuracion registrada para la transaccion.";
                LogControl.save("ValRetroCotizacion", ex.ToString(), "3");
            }

            return result;
        }

        public List<CotizadorDetalleVM> detailQuotationInfo(List<insuredGraph> listAsegurados, validaTramaVM objValida)
        {
            var response = new List<CotizadorDetalleVM>();

            LogControl.save(objValida.codProceso, "detailQuotationInfo objValida: " + JsonConvert.SerializeObject(objValida, Formatting.None), "2", objValida.codAplicacion);
            LogControl.save(objValida.codProceso, "detailQuotationInfo listAsegurados: " + JsonConvert.SerializeObject(listAsegurados, Formatting.None), "2", objValida.codAplicacion);

            objValida.igvPD = objValida.igvPD + 1.0;

            if (Generic.valRentaEstudiantilVG(objValida.datosPoliza.branch.ToString(), objValida.datosPoliza.codTipoProducto, objValida.datosPoliza.codTipoPerfil) &&
                (listAsegurados != null && listAsegurados.Count != 0)) //AVS - RENTAS
            {

                response = getDetailQuotationInfo(objValida, 1);

                //var filterList = listAsegurados.Where(x => x.role == "Beneficiario").ToList().GroupBy(x => x.salary).ToList();

                //foreach (var aseg in filterList)
                //{
                //    var item = new CotizadorDetalleVM();
                //    item.MONT_PLANILLA = aseg.Key.ToString();
                //    item.PRIMA = objValida.tipoCotizacion == "RATE" ? Math.Round(((Math.Round((aseg.Key * (objValida.ntasa_tariff / 100)), 2)) * objValida.igvPD), 2).ToString() : Math.Round((objValida.asegPremium * objValida.igvPD), 2).ToString(); // objValida.premium.ToString()
                //    item.PRIMA_UNIT = objValida.tipoCotizacion == "RATE" ? Math.Round((aseg.Key * (objValida.ntasa_tariff / 100)), 2).ToString() : objValida.asegPremium.ToString();
                //    item.TASA = objValida.ntasa_tariff.ToString();
                //    item.TOT_ASEGURADOS = aseg.Count().ToString();
                //    response.Add(item);
                //}
            }
            else
            {
                if (listAsegurados != null && listAsegurados.Count > 0)
                {
                    response = getDetailQuotationInfo(objValida, 1);
                }
                else
                {
                    response = getDetailQuotationInfo(objValida, 0);
                }

                //objValida.asegPremium = !(new string[] { "EX", "EN" }).Contains(objValida.datosPoliza.trxCode) ? objValida.asegPremium : objValida.premium / objValida.CantidadTrabajadores;
                //response.Add(new CotizadorDetalleVM
                //{
                //    MONT_PLANILLA = objValida.montoPlanilla.ToString(),
                //    PRIMA = Generic.valRentaEstudiantil(objValida.datosPoliza.branch.ToString(), objValida.datosPoliza.codTipoProducto, objValida.datosPoliza.codTipoPerfil) ? (objValida.asegPremiumTotal).ToString() : objValida.premium.ToString(),
                //    PRIMA_UNIT = objValida.asegPremium.ToString(),
                //    TASA = objValida.ntasa_tariff.ToString(),
                //    TOT_ASEGURADOS = objValida.CantidadTrabajadores.ToString()
                //});
            }

            LogControl.save(objValida.codProceso, "detailQuotationInfo Fin: " + JsonConvert.SerializeObject(response, Formatting.None), "2", objValida.codAplicacion);

            return response;
        }

        public List<CotizadorDetalleVM> getDetailQuotationInfo(validaTramaVM objValida, int trama)
        {
            var response = new List<CotizadorDetalleVM>();

            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.sp_detailQuotation;

            try
            {
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, objValida.codProceso, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, objValida.datosPoliza.branch, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, objValida.datosPoliza.codTipoProducto, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTRAMA", OracleDbType.Int32, trama, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int64, objValida.nroCotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("C_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));

                OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storeprocedure, parameter);

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        var item = new CotizadorDetalleVM();
                        item.MONT_PLANILLA = dr["NPENSION"].ToString();
                        item.PRIMA = Generic.valRentaEstudiantil(objValida.datosPoliza.branch.ToString(), objValida.datosPoliza.codTipoProducto, objValida.datosPoliza.codTipoPerfil) ? dr["NPREMIUMT_ASEG"].ToString() : dr["NPREMIUMN"].ToString();
                        item.PRIMA_UNIT = dr["NPREMIUMN_ASEG"].ToString();
                        item.TASA = dr["NTASA"].ToString();
                        item.TOT_ASEGURADOS = dr["NCANT_TRAB"].ToString();
                        response.Add(item);
                    }
                }

                ELog.CloseConnection(dr);
            }
            catch (Exception ex)
            {
                LogControl.save("getInfoQuotationTransac", ex.ToString(), "3");
            }

            return response;
        }

        public infoQuotationPreviewVM GetInfoQuotationPreview(infoQuotationPreviewBM request)
        {
            var response = new infoQuotationPreviewVM();

            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.pkg_ReaDataAP + ".REA_ALL_AP_PREVIEW";

            OracleDataReader dr = null;
            try
            {
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, request.nbranch, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, ELog.obtainConfig("planMaestro" + request.nbranch), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int64, request.nid_cotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.nid_process, ParameterDirection.Input));
                string[] arrayCursor = { "C_CURSOR1", "C_CURSOR2", "C_CURSOR3", "C_CURSOR4", "C_CURSOR5" };
                OracleParameter C_CURSOR1 = new OracleParameter(arrayCursor[0], OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter C_CURSOR2 = new OracleParameter(arrayCursor[1], OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter C_CURSOR3 = new OracleParameter(arrayCursor[2], OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter C_CURSOR4 = new OracleParameter(arrayCursor[3], OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter C_CURSOR5 = new OracleParameter(arrayCursor[4], OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_CURSOR1);
                parameter.Add(C_CURSOR2);
                parameter.Add(C_CURSOR3);
                parameter.Add(C_CURSOR4);
                parameter.Add(C_CURSOR5);

                dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storeprocedure, arrayCursor, parameter);

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        if (dr["NCOVERGEN"] != DBNull.Value)
                        {
                            var item = new coberturaPropuesta();
                            item.codCobertura = dr["NCOVERGEN"].ToString();
                            item.sumaPropuesta = Convert.ToDouble(dr["NCAPITAL"].ToString());
                            response.lcoberturas.Add(item);
                        }
                    }

                    dr.NextResult();

                    while (dr.Read())
                    {
                        if (dr["NCOD_BENEFIT"] != DBNull.Value)
                        {
                            var item = new beneficioPropuesto();
                            item.codBeneficio = dr["NCOD_BENEFIT"].ToString();
                            response.lbeneficios.Add(item);
                        }
                    }

                    dr.NextResult();

                    while (dr.Read())
                    {
                        if (dr["NCOD_ASSISTANCE"] != DBNull.Value)
                        {
                            var item = new asistenciaPropuesta();
                            item.codAsistencia = dr["NCOD_ASSISTANCE"].ToString();
                            response.lasistencias.Add(item);
                        }
                    }

                    dr.NextResult();

                    while (dr.Read())
                    {
                        if (dr["NCOD_SURCHARGE"] != DBNull.Value)
                        {
                            var item = new recargoPropuesto();
                            item.codRecargo = dr["NCOD_SURCHARGE"].ToString();
                            response.lrecargos.Add(item);
                        }
                    }

                    dr.NextResult();

                    while (dr.Read())
                    {
                        if (dr["NCOD_ADDITIONAL_SERVICES"] != DBNull.Value)
                        {
                            var item = new servicioAdicionalPropuesto();
                            item.codServAdicionales = dr["NCOD_ADDITIONAL_SERVICES"].ToString();
                            item.nhoras = dr["NHOUR"] == DBNull.Value ? 0 : Convert.ToDouble(dr["NHOUR"].ToString());
                            response.lservAdicionales.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogControl.save("getInfoQuotationTransac", ex.ToString(), "3");
            }
            finally
            {
                ELog.CloseConnection(dr);
            }

            return response;
        }

        public string GetRangoEdad(string fecha)
        {
            var rango = "";
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storedProcedureName = ProcedureName.pkg_GeneraTransac + ".SP_SET_RANGO_EDAD_ASEG";

            try
            {
                parameter.Add(new OracleParameter("P_DBIRTHDAT", OracleDbType.Varchar2, fecha, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_SRANGO_EDAD = new OracleParameter("P_SRANGO_EDAD", OracleDbType.Varchar2, rango, ParameterDirection.Output);
                P_SRANGO_EDAD.Size = 400;

                parameter.Add(P_SRANGO_EDAD);

                this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                rango = P_SRANGO_EDAD.Value.ToString();
            }
            catch (Exception ex)
            {
                LogControl.save("GetRangoEdad", ex.ToString(), "3");
            }

            return rango;
        }
        // GCAA 06022024 - CUPONERAS
        public List<CouponList> getCuponesExclusion(string nid_proc)
        {
            List<CouponList> cupones = new List<CouponList>();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = "PD_ACC_PER_GET_CUPONES";

            OracleDataReader dr = null;
            try
            {
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.NVarchar2, nid_proc, ParameterDirection.Input));
                string[] arrayCursor = { "C_RESPUESTA" };
                OracleParameter C_TABLE = new OracleParameter(arrayCursor[0], OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storeprocedure, arrayCursor, parameter);

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        CouponList _item = new CouponList();
                        _item.NCUPONERA = 0;
                        _item.NCOUPON = dr["NCOUPON"] == DBNull.Value ? 0 : Convert.ToInt32(dr["NCOUPON"].ToString());
                        _item.NRECEIPT = dr["NRECEIPT"] == DBNull.Value ? 0 : Convert.ToInt64(dr["NRECEIPT"].ToString());
                        _item.DEFFECDATE = dr["DEFFECDATE"] == DBNull.Value ? "" : Convert.ToString(Convert.ToDateTime(dr["DEFFECDATE"]).ToString("dd/MM/yyyy").ToString());
                        _item.DEXPIRDAT = dr["DEXPIRDAT"] == DBNull.Value ? "0" : Convert.ToString(Convert.ToDateTime(dr["DEXPIRDAT"]).ToString("dd/MM/yyyy").ToString());
                        _item.DPAYDATE = dr["DPAYDATE"] == DBNull.Value ? "0" : Convert.ToString(Convert.ToDateTime(dr["DPAYDATE"]).ToString("dd/MM/yyyy").ToString());
                        _item.NPREMIUM = dr["NPREMIUM"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["NPREMIUM"].ToString());

                        cupones.Add(_item);
                    }
                }
            }
            catch (Exception ex)
            {
                LogControl.save("getCuponesExclusion", ex.ToString(), "3");
            }
            finally
            {
                ELog.CloseConnection(dr);
            }

            return cupones;
        }

        public SalidaErrorBaseVM regDetallePlanValidacion(validaTramaVM data, string origen)
        {
            SalidaErrorBaseVM response = new SalidaErrorBaseVM();

            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
            {
                cn.Open();

                using (var tran = cn.BeginTransaction())
                {
                    try
                    {
                        if (origen == "coberturas")
                        {
                            // response = eliminarCoberturasCotTrx(data, cn, tran);

                            if (data.lcoberturas != null)
                            {
                                foreach (var cobertura in data.lcoberturas)
                                {
                                    response = response.P_COD_ERR == 0 ? regCoberturasCotTrx(data, cobertura, cn, tran) : response;
                                    if (cobertura.items != null && cobertura.items.Count > 0)
                                    {
                                        foreach (var subItem in cobertura.items)
                                        {
                                            subItem.codeCover = cobertura.codCobertura;
                                            response = response.P_COD_ERR == 0 ? insertSubItemTrx(data, subItem, cn, tran) : response;
                                        }
                                    }
                                }
                            }
                        }

                        //if (origen == "asistencias")
                        //{
                        //    response = response.P_COD_ERR == 0 ? eliminarAsistenciasCotTrx(data, cn, tran) : response;

                        //    if (data.lasistencias != null)
                        //    {
                        //        foreach (var asistencia in data.lasistencias)
                        //        {
                        //            response = response.P_COD_ERR == 0 ? regAsistenciasCotTrx(data, asistencia, cn, tran) : response;
                        //        }
                        //    }

                        //}

                        //if (origen == "beneficios")
                        //{
                        //    response = response.P_COD_ERR == 0 ? eliminarBeneficiosCotTrx(data, cn, tran) : response;

                        //    if (data.lbeneficios != null)
                        //    {
                        //        foreach (var beneficio in data.lbeneficios)
                        //        {
                        //            response = response.P_COD_ERR == 0 ? regBeneficiosCotTrx(data, beneficio, cn, tran) : response;
                        //        }
                        //    }
                        //}

                        //if (origen == "adicionales")
                        //{
                        //    response = response.P_COD_ERR == 0 ? eliminarServiciosAdicionalesCotTrx(data, cn, tran) : response; //ED

                        //    if (data.lservAdicionales != null)
                        //    {
                        //        foreach (var servAdicionales in data.lservAdicionales)
                        //        {
                        //            response = response.P_COD_ERR == 0 ? regServiciosAdicionalesCotTrx(data, servAdicionales, cn, tran) : response;
                        //        }
                        //    }
                        //}

                        //if (origen == "recargos")
                        //{
                        //    response = response.P_COD_ERR == 0 ? eliminarRecargosCotTrx(data, cn, tran) : response;

                        //    if (data.lbeneficios != null)
                        //    {
                        //        foreach (var beneficio in data.lbeneficios)
                        //        {
                        //            response = response.P_COD_ERR == 0 ? regBeneficiosCotTrx(data, beneficio, cn, tran) : response;
                        //        }
                        //    }

                        //}


                        //response = response.P_COD_ERR == 0 ? eliminarRecargosCotTrx(data, cn, tran) : response;
                        //response = response.P_COD_ERR == 0 ? eliminarServiciosAdicionalesCotTrx(data, cn, tran) : response; //ED
                    }
                    catch (Exception ex)
                    {
                        response.P_COD_ERR = 1;
                        response.P_MESSAGE = ex.ToString();
                        LogControl.save("regDetallePlanCot", ex.ToString(), "3");
                    }
                    finally
                    {
                        if (response.P_COD_ERR == 0)
                        {
                            tran.Commit();
                        }
                        else
                        {
                            tran.Rollback();
                        }

                        if (cn.State == ConnectionState.Open) cn.Close();
                    }


                }
            }

            return response;
        }

        public EmitPolVM InsCotizaHisTecnica(int nroCotizacion)
        {
            var response = new EmitPolVM();
            var sPackageName = ProcedureName.pkg_ValidadorGenPD + ".SP_INS_TECNICA_HIS";
            List<OracleParameter> parameter = new List<OracleParameter>();
            try
            {
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, nroCotizacion, ParameterDirection.Input));

                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, response.cod_error, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, response.message, ParameterDirection.Output);

                P_SMESSAGE.Size = 9000;

                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);

                this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                response.cod_error = Convert.ToInt32(P_NCODE.Value.ToString());
                response.message = P_SMESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                response.cod_error = 1;
                response.message = ex.ToString();
                LogControl.save("InsCotizaHisTecnica", ex.ToString(), "3");
            }

            return response;
        }

        public bool PD_PROCESA_RANGO_X_ASOCIAR(int quotationNumber)
        {
            var response = new EmitPolVM();
            bool success = true;
            string storeprocedure = "PD_PROCESA_RANGO_X_ASOCIAR";

            List<OracleParameter> parameter = new List<OracleParameter>();

            if (success)
            {
                try
                {
                    parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, quotationNumber, ParameterDirection.Input));

                    OracleParameter P_MENSAJE = new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, ParameterDirection.Output);
                    OracleParameter P_COD_ESTADO = new OracleParameter("P_COD_ESTADO", OracleDbType.Int32, ParameterDirection.Output);

                    P_MENSAJE.Size = 4000;

                    parameter.Add(P_MENSAJE);
                    parameter.Add(P_COD_ESTADO);

                    this.ExecuteByStoredProcedureVT(storeprocedure, parameter);

                    response.cod_error = Convert.ToInt32(P_COD_ESTADO.Value.ToString());

                    if (response.cod_error != 0)
                    {
                        response.cod_error = 3;
                        response.message = P_MENSAJE.Value.ToString();
                        success = false;
                    }

                    LogControl.save("PD_PROCESA_RANGO_X_ASOCIAR", "Paso X: Se ejecutó " + "PD_PROCESA_RANGO_X_ASOCIAR", "1");
                }
                catch (Exception ex)
                {
                    response.cod_error = 3;
                    response.message = ex.Message;
                    success = false;
                    LogControl.save("PD_PROCESA_RANGO_X_ASOCIAR", ex.ToString(), "3");
                }
            }


            return success;

        }
    }
}