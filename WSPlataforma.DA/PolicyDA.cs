using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WSPlataforma.Entities.PolicyModel.BindingModel;
using WSPlataforma.Entities.PolicyModel.ViewModel;
using WSPlataforma.Entities.QuotationModel.BindingModel;
using quotationVM = WSPlataforma.Entities.QuotationModel.ViewModel;
using WSPlataforma.Util;
using WSPlataforma.Util.PrintPolicyUtility;
using WSPlataforma.Entities.QuotationModel;
using System.Diagnostics;
using SpreadsheetLight;
using WSPlataforma.Entities.AjustedAmountsModel.BindingModel;
using WSPlataforma.Entities.AjustedAmountsModel.ViewModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using Newtonsoft.Json;
using DCPoliza = WSPlataforma.Entities.QuotationModel.ViewModel;
using System.Text.RegularExpressions;
using WSPlataforma.Entities.MovementsModel.ViewModel;
using DocumentFormat.OpenXml.EMMA;
using WSPlataforma.Entities.ReportModel.BindingModel;
using WSPlataforma.Entities.ValBrokerModel;
using WSPlataforma.Entities.EPSModel.ViewModel;
using Newtonsoft.Json.Linq;
using WSPlataforma.Entities.ProductModel.ViewModel;
using WSPlataforma.Entities.ComprobantesEPSModel.BindingModel;
using static WSPlataforma.DA.QuotationDA;
using static WSPlataforma.Entities.CoberturaModel.EntityCobertura;
using WSPlataforma.Entities.CoberturaModel;
using WSPlataforma.Entities.NidProcessRenewModel.ViewModel;

namespace WSPlataforma.DA
{
    public class PolicyDA : ConnectionBase
    {
        private SharedMethods sharedMethods = new SharedMethods();
        QuotationDA QuotationDA = new QuotationDA();

        public ResponseVM ValidatePolicyRenov(PolicyRenewBM policyRenew)
        {
            var response = new ResponseVM();

            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        OracleDataReader dr;

                        cmd.Connection = cn;
                        cmd.CommandText = ProcedureName.pkg_Poliza + ".VAL_POLICY_RENOV";
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, policyRenew.NBRANCH, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, policyRenew.NPRODUCT, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int64, policyRenew.NPOLICY, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, policyRenew.NID_COTIZACION, ParameterDirection.Input));
                        if (policyRenew.NTYPE_TRANSAC != 0)
                        {
                            cmd.Parameters.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Int32, policyRenew.NTYPE_TRANSAC, ParameterDirection.Input));
                        }

                        // OUTPUT
                        OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, response.P_NCODE, ParameterDirection.Output);
                        OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, response.P_SMESSAGE, ParameterDirection.Output);

                        cmd.Parameters.Add(P_NCODE);
                        cmd.Parameters.Add(P_SMESSAGE);

                        cn.Open();
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        response.P_NCODE = P_NCODE.Value.ToString();
                        response.P_SMESSAGE = P_SMESSAGE.Value.ToString();

                        dr.Close();
                    }
                    catch (Exception ex)
                    {
                        response.P_NCODE = "1";
                        response.P_SMESSAGE = ex.Message;
                        LogControl.save("ValidatePolicyRenov", ex.ToString(), "3");
                    }
                    finally
                    {
                        if (cn.State == ConnectionState.Open) cn.Close();
                    }
                }
            }

            //List<OracleParameter> parameter = new List<OracleParameter>();
            //string storedProcedureName = ProcedureName.pkg_Poliza + ".VAL_POLICY_RENOV";

            //try
            //{
            //    parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, policyRenew.NBRANCH, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, policyRenew.NPRODUCT, ParameterDirection.Input));//MARC
            //    parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int64, policyRenew.NPOLICY, ParameterDirection.Input));//MARC
            //    parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, policyRenew.NID_COTIZACION, ParameterDirection.Input));
            //    if (policyRenew.NTYPE_TRANSAC != 0)
            //        parameter.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Int32, policyRenew.NTYPE_TRANSAC, ParameterDirection.Input));

            //    //OUTPUT
            //    OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, response.P_NCODE, ParameterDirection.Output);
            //    OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, response.P_SMESSAGE, ParameterDirection.Output);

            //    parameter.Add(P_NCODE);
            //    parameter.Add(P_SMESSAGE);

            //    this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

            //    response.P_NCODE = P_NCODE.Value.ToString();
            //    response.P_SMESSAGE = P_SMESSAGE.Value.ToString();
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}

            return response;
        }

        public ResponseVM cancelMovement(PolicyMovementCancelBM data)
        {
            var response = new ResponseVM();

            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        OracleDataReader dr;

                        cmd.Connection = cn;
                        cmd.CommandText = ProcedureName.pkg_GeneraTransac + ".SP_PROCESO_REVERSO_RI";
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new OracleParameter("P_NIDHEADERPROC", OracleDbType.Double, data.NIDHEADERPROC, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_SCERTYPE", OracleDbType.Varchar2, data.SCERTYPE, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Double, data.NBRANCH, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Double, data.NPRODUCT, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NPOLICY", OracleDbType.Double, data.NPOLICY, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Double, data.NUSERCODE, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NFLAG_ANULA", OracleDbType.Double, data.NFLAG_AN_PROC, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NMOVEMENT", OracleDbType.Double, data.NMOVEMENT, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NTYPE_HIST", OracleDbType.Double, data.NTYPE_HIST, ParameterDirection.Input));

                        // OUTPUT
                        OracleParameter P_NPOLICY_OUT = new OracleParameter("P_NPOLICY_OUT", OracleDbType.Double, response.P_RESULT, ParameterDirection.Output);
                        OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Varchar2, 4000, response.P_NCODE, ParameterDirection.Output);
                        OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, response.P_SMESSAGE, ParameterDirection.Output);

                        cmd.Parameters.Add(P_NPOLICY_OUT);
                        cmd.Parameters.Add(P_NCODE);
                        cmd.Parameters.Add(P_SMESSAGE);

                        cmd.Parameters.Add(new OracleParameter("P_SKEY", OracleDbType.Double, null, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NMOVEMENT_PH", OracleDbType.Double, data.NMOVEMENT_PH, ParameterDirection.Input));

                        cn.Open();
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        response.P_RESULT = P_NPOLICY_OUT.Value.ToString();
                        response.P_NCODE = P_NCODE.Value.ToString();
                        response.P_SMESSAGE = response.P_NCODE == "0" ? "El reverso se realizó correctamente" : P_SMESSAGE.Value.ToString();

                        ELog.CloseConnection(dr);
                    }
                    catch (Exception ex)
                    {
                        response.P_NCODE = "1";
                        response.P_SMESSAGE = ex.Message;
                        LogControl.save("cancelMovement", ex.ToString(), "3");
                    }
                    finally
                    {
                        if (cn.State == ConnectionState.Open) cn.Close();
                    }
                }
            }

            /**/
            //List<OracleParameter> parameter = new List<OracleParameter>();
            //string storedProcedureName = ProcedureName.pkg_GeneraTransac + ".SP_PROCESO_REVERSO_RI";
            //try
            //{
            //    //INPUT
            //    parameter.Add(new OracleParameter("P_NIDHEADERPROC", OracleDbType.Double, data.NIDHEADERPROC, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_SCERTYPE", OracleDbType.Varchar2, data.SCERTYPE, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Double, data.NBRANCH, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Double, data.NPRODUCT, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Double, data.NPOLICY, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Double, data.NUSERCODE, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_NFLAG_ANULA", OracleDbType.Double, data.NFLAG_AN_PROC, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_NMOVEMENT", OracleDbType.Double, data.NMOVEMENT, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_NTYPE_HIST", OracleDbType.Double, data.NTYPE_HIST, ParameterDirection.Input));

            //    //OUTPUT
            //    OracleParameter P_NPOLICY_OUT = new OracleParameter("P_NPOLICY_OUT", OracleDbType.Double, result.P_RESULT, ParameterDirection.Output);
            //    OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Varchar2, result.P_NCODE, ParameterDirection.Output);
            //    OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, result.P_SMESSAGE, ParameterDirection.Output);

            //    P_NPOLICY_OUT.Size = 4000;
            //    P_NCODE.Size = 4000;
            //    P_SMESSAGE.Size = 4000;
            //    parameter.Add(P_NPOLICY_OUT);
            //    parameter.Add(P_NCODE);
            //    parameter.Add(P_SMESSAGE);

            //    parameter.Add(new OracleParameter("P_SKEY", OracleDbType.Double, null, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_NMOVEMENT_PH", OracleDbType.Double, data.NMOVEMENT_PH, ParameterDirection.Input));

            //    OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

            //    ELog.CloseConnection(odr);
            //    result.P_RESULT = P_NPOLICY_OUT.Value.ToString();
            //    result.P_NCODE = P_NCODE.Value.ToString();
            //    if (result.P_NCODE == "0")
            //    {
            //        result.P_SMESSAGE = "El reverso se realizó correctamente";
            //    }
            //    else
            //    {
            //        result.P_SMESSAGE = P_SMESSAGE.Value.ToString();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    ELog.save(this, ex.ToString());
            //    result.P_NCODE = "1";
            //    result.P_SMESSAGE = "Error en el servidor. " + ex.Message;
            //}

            return response;
        }

        public PolicyModuleVM GetValidateExistPolicy(PolicyModuleBM policyModuleBM)
        {
            var response = new PolicyModuleVM();

            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        OracleDataReader dr;

                        cmd.Connection = cn;
                        cmd.CommandText = ProcedureName.pkg_CargaMasiva + ".VAL_EXISTS_POLICY";
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Decimal, policyModuleBM.NBRANCH, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Decimal, policyModuleBM.NPRODUCT, ParameterDirection.Input)); // MARC
                        cmd.Parameters.Add(new OracleParameter("P_NPOLICY", OracleDbType.Decimal, policyModuleBM.NPOLICY, ParameterDirection.Input)); // MARC
                        cmd.Parameters.Add(new OracleParameter("P_DSTARTDATE", OracleDbType.Date, DateTime.Parse(policyModuleBM.DSTARTDATE).ToShortDateString(), ParameterDirection.Input)); // MARC

                        // OUTPUT
                        OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, response.SMESSAGE, ParameterDirection.Output);

                        cmd.Parameters.Add(P_SMESSAGE);

                        cn.Open();
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        response.SMESSAGE = P_SMESSAGE.Value.ToString();

                        ELog.CloseConnection(dr);
                    }
                    catch (Exception ex)
                    {
                        response.SMESSAGE = ex.Message;
                        LogControl.save("GetValidateExistPolicy", ex.ToString(), "3");
                    }
                    finally
                    {
                        if (cn.State == ConnectionState.Open) cn.Close();
                    }
                }
            }

            /**/
            //List<OracleParameter> parameter = new List<OracleParameter>();
            //string storedProcedureName = ProcedureName.pkg_CargaMasiva + ".VAL_EXISTS_POLICY";

            //try
            //{
            //    parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Decimal, policyModuleBM.NBRANCH, ParameterDirection.Input));
            //    parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Decimal, policyModuleBM.NPRODUCT, ParameterDirection.Input));//MARC
            //    parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Decimal, policyModuleBM.NPOLICY, ParameterDirection.Input));//MARC
            //    parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Date, DateTime.Parse(policyModuleBM.DSTARTDATE).ToShortDateString(), ParameterDirection.Input));//MARC

            //    //OUTPUT
            //    OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, response.SMESSAGE, ParameterDirection.Output);

            //    parameter.Add(P_SMESSAGE);

            //    this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

            //    response.SMESSAGE = P_SMESSAGE.Value.ToString();
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}

            return response;
        }



        public Entities.PolicyModel.ViewModel.GenericPackageVM GetMovementTypeList()
        {
            var response = new Entities.PolicyModel.ViewModel.GenericPackageVM();
            var listTransacction = new List<MovementTypeVM>();

            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        OracleDataReader dr;

                        cmd.Connection = cn;
                        cmd.CommandText = ProcedureName.pkg_Poliza + ".REA_TIP_TRANSACCION";
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;

                        cn.Open();
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                var item = new MovementTypeVM();
                                item.Id = dr["COD_TIPO_TRANSACCION"].ToString();
                                item.Name = dr["DES_TIPO_TRANSACCION"].ToString();
                                listTransacction.Add(item);
                            }
                        }

                        ELog.CloseConnection(dr);

                        response.GenericResponse = listTransacction;

                    }
                    catch (Exception ex)
                    {
                        LogControl.save("GetMovementTypeList", ex.ToString(), "3");
                    }
                    finally
                    {
                        if (cn.State == ConnectionState.Open) cn.Close();
                    }
                }
            }

            //List<OracleParameter> parameter = new List<OracleParameter>();
            //List<MovementTypeVM> list = new List<MovementTypeVM>();
            //string storedProcedureName = ProcedureName.pkg_Poliza + ".REA_TIP_TRANSACCION"; // [pending]
            //try
            //{
            //    //OUTPUT
            //    OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);

            //    parameter.Add(C_TABLE);

            //    OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);
            //    while (odr.Read())
            //    {
            //        MovementTypeVM item = new MovementTypeVM();
            //        item.Id = odr["COD_TIPO_TRANSACCION"].ToString();
            //        item.Name = odr["DES_TIPO_TRANSACCION"].ToString();

            //        list.Add(item);
            //    }
            //    ELog.CloseConnection(odr);

            //    response.GenericResponse = list;
            //}
            //catch (Exception ex)
            //{
            //    ELog.save(this, ex);
            //}

            return response;
        }

        public quotationVM.GenericResponseVM GetPolicyTrackingList(TrackingBM data)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            quotationVM.GenericResponseVM resultPackage = new quotationVM.GenericResponseVM();
            List<quotationVM.QuotationTrackingVM> list = new List<quotationVM.QuotationTrackingVM>();

            string storedProcedureName = ProcedureName.pkg_Poliza + ".REA_HIS_APROBACION";
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
                    quotationVM.QuotationTrackingVM item = new quotationVM.QuotationTrackingVM();
                    item.User = odr["USUARIO"].ToString();
                    item.Status = odr["COD_ESTADO"].ToString();
                    item.Comment = odr["COMENTARIO"].ToString();
                    //item.Reason = odr["MOTIVO"].ToString();
                    item.Profile = odr["PERFIL"].ToString();

                    if (odr["FECHA"].ToString() != null && odr["FECHA"].ToString().Trim() != "") item.EventDate = Convert.ToDateTime(odr["FECHA"].ToString());
                    else item.EventDate = null;

                    list.Add(item);
                }
                resultPackage.TotalRowNumber = Int32.Parse(totalRowNumber.Value.ToString());
                resultPackage.GenericResponse = list;
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                LogControl.save("GetPolicyTrackingList", ex.ToString(), "3");
            }

            return resultPackage;
        }

        public Entities.PolicyModel.ViewModel.GenericPackageVM GetProductTypeList()
        {
            Entities.PolicyModel.ViewModel.GenericPackageVM package = new Entities.PolicyModel.ViewModel.GenericPackageVM();
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<ProductTypeVM> list = new List<ProductTypeVM>();
            string storedProcedureName = ProcedureName.pkg_Cotizacion + ".REA_PRODUCT_SCTR";
            try
            {
                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);

                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);
                while (odr.Read())
                {
                    ProductTypeVM item = new ProductTypeVM();
                    item.Id = odr["COD_PRODUCT"].ToString();
                    item.Name = odr["DES_PRODUCT"].ToString();

                    list.Add(item);
                }
                package.TotalRowNumber = 2;
                package.GenericResponse = list;
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                LogControl.save("GetProductTypeList", ex.ToString(), "3");
            }

            return package;
        }
        public SalidadPolizaEmit transactionSave(PolicyTransactionSaveBM request, List<HttpPostedFile> files)
        {
            var response = new SalidadPolizaEmit();

            if (request.P_NPRODUCTO == "2")
            {
                request.P_SRUTA = String.Format(ELog.obtainConfig("pathPrincipal"), ELog.obtainConfig("pathPoliza") + request.P_NID_COTIZACION + "\\" + ELog.obtainConfig("pathAdjuntos") + "\\" + ELog.obtainConfig("movimiento" + request.P_NTYPE_TRANSAC) + "\\" + DateTime.Now.ToString("yyyyMMddhhmmss") + "\\");
            }
            else
            {
                request.P_SRUTA = ELog.obtainConfig("pathCotizacion") + request.P_NID_COTIZACION + "\\" + ELog.obtainConfig("movimiento") + "\\" + DateTime.Now.ToString("yyyyMMddhhmmss") + "\\";
            }

            try
            {
                var lTransaccion = ELog.obtainConfig("transaccionPoliza").Split(';');
                if (lTransaccion.Contains(request.P_NTYPE_TRANSAC.ToString()))
                {
                    if (request.P_NTYPE_TRANSAC == 3)
                    {
                        var trama = new ValidaTramaBM()
                        {
                            NID_COTIZACION = request.P_NID_COTIZACION,
                            NTYPE_TRANSAC = request.P_NTYPE_TRANSAC.ToString(),
                            NID_PROC = request.P_NID_PROC
                        };
                        new QuotationDA().insertHistTrama(trama);
                    }

                    response = transactionPolicy(request);
                }

                var lAnulacion = ELog.obtainConfig("anulacionPoliza").Split(';');
                if (lAnulacion.Contains(request.P_NTYPE_TRANSAC.ToString()))
                {

                    response = AnulMovementPolicy(request);

                    var insEPS = 0;

                    if (response.P_COD_ERR == 0)
                    {
                        if (request.P_NEPS == Convert.ToInt32(ELog.obtainConfig("grandiaKey")) && (request.P_NCOT_MIXTA == 1 || Convert.ToInt32(request.P_NPRODUCTO) == 2))
                        {
                            if (request.P_NTYPE_TRANSAC == 6 || request.P_NTYPE_TRANSAC == 7)
                            {
                                insEPS = AnulaEPS(request);
                            }
                        }

                    }
                }

                guardarArchivos(request.P_SRUTA, files);
            }
            catch (Exception ex)
            {
                LogControl.save("transactionSave", ex.ToString(), "3");
            }

            return response;
        }

        public int AnulaEPS(PolicyTransactionSaveBM request)
        {
            string codigoCip = GetCodigoCip(request.P_NID_COTIZACION.ToString());
            var epsTr = new dataTransaccion();
            var epsAnu = new dataTransaccionAnulacionMovimiento();
            var asincObject = new Response_EPS_Transaccion();
            var insEPS = 1;
            var URI_SendEmision_EPS = ELog.obtainConfig("urlEmisionEPS_SCTR");

            if (request.P_NTYPE_TRANSAC == 7) // POLIZA
            {
                epsTr.codigoCotizacion = request.P_NID_COTIZACION.ToString();
                epsTr.codigoContrato = request.P_NPOLICY_SALUD.ToString();
                epsTr.codigoProceso = request.P_NID_PROC.ToString();
                epsTr.fechaEfectoAseguradoRecibo = DateTime.ParseExact(request.P_DEFFECDATE, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                epsTr.fechaExpiracionAseguradoRecibo = DateTime.ParseExact(request.P_DEXPIRDAT, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                epsTr.fechaEfectoPoliza = DateTime.ParseExact(request.P_DSTARTDATE_POL, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                epsTr.fechaExpiracionPoliza = DateTime.ParseExact(request.P_DEXPIRDAT_POL, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                epsTr.asignacionActividadAltoRiesgo = (request.P_SDELIMITER == 1) ? true : false;
                epsTr.codigoMoneda = request.P_NCURRENCY.ToString();
                epsTr.codigoFrecuenciaPago = request.P_SCOLTIMRE.ToString();
                epsTr.codigoFrecuenciaRenovacion = request.P_NPAYFREQ.ToString();
                epsTr.codigoFormaPago = request.P_PAYPF.ToString();
                epsTr.asignacionFacturacionAnticipada = (request.P_SFLAG_FAC_ANT == 1) ? true : false;
                epsTr.asignacionRegulaMesVencido = (request.P_FACT_MES_VENCIDO == 1) ? true : false;
                epsTr.codigoTipoTransaccion = request.P_NTYPE_TRANSAC.ToString();
                epsTr.fechaTransaccion = DateTime.Now.ToString("yyyy-MM-dd");
                epsTr.codigoUsuarioRegistro = request.P_NUSERCODE.ToString();
                epsTr.primaMinimaAutorizada = decimal.Parse(request.P_NPREM_MINIMA);
                epsTr.primaComercial = decimal.Parse(request.P_NPREM_NETA);
                epsTr.igv = decimal.Parse(request.P_IGV);
                epsTr.derechoEmision = (decimal)request.P_NDE;
                epsTr.primaTotal = decimal.Parse(request.P_NPREM_BRU);
                epsTr.cipPagoEfectivo = codigoCip ?? "";
                epsTr.riesgos = null;
                epsTr.asegurados = null;

                new QuotationDA().InsertLog(Convert.ToInt64(request.P_NID_COTIZACION.ToString()), "02 - Se envia el JSON para anulación a la EPS", URI_SendEmision_EPS, JsonConvert.SerializeObject(epsTr), null);

                var json = Task.Run(async () =>
                {
                    var transaccion = "ANULACION";
                    var jsonTsk = await new PolicyDA().invocarServicio_EPS_TRA(Convert.ToInt32(request.P_NID_COTIZACION.ToString()), JsonConvert.SerializeObject(epsTr), transaccion);
                    asincObject = JsonConvert.DeserializeObject<Response_EPS_Transaccion>(jsonTsk);

                    bool isSuccess = asincObject == null ? false : asincObject.success;

                    if (!isSuccess)
                    {
                        string errorMessage = "Servicio EPS - SCTR - transacción: " + asincObject.message;
                        new QuotationDA().InsertLog(Convert.ToInt64(request.P_NID_COTIZACION.ToString()), "03 - Error en Servicio EPS - Transaccion de Poliza", URI_SendEmision_EPS, JsonConvert.SerializeObject(asincObject), null);
                        insEPS = (new PolicyDA().insertErrorEPS(request.P_NID_COTIZACION.ToString(), JsonConvert.SerializeObject(epsTr), request.P_NID_PROC_EPS, request.P_NID_PROC)).P_COD_ERR;
                    }
                    else
                    {
                        string mensaje = "SE REALIZO LA ANULACIÓN CORRECTAMENTE";
                        insEPS = new PolicyDA().insert_policy_EPS(long.Parse(request.P_NPOLICY_SALUD), asincObject.data.idContrato, request.P_NCOT_MIXTA == 1 ? request.P_NID_PROC_EPS : request.P_NID_PROC, mensaje);
                    }

                });
            }

            if (request.P_NTYPE_TRANSAC == 6)
            {
                epsAnu.codigoCotizacion = request.P_NID_COTIZACION.ToString();
                epsAnu.codigoContrato = request.P_NPOLICY_SALUD.ToString();
                epsAnu.codigoProceso = request.P_NID_PROC.ToString();
                epsAnu.codigoTipoTransaccion = request.P_NTYPE_TRANSAC.ToString();
                epsAnu.codigoUsuarioRegistro = request.P_NUSERCODE.ToString();

                new QuotationDA().InsertLog(Convert.ToInt64(request.P_NID_COTIZACION.ToString()), "02 - Se envia el JSON para anulación a la EPS", URI_SendEmision_EPS, JsonConvert.SerializeObject(epsAnu), null);

                var json = Task.Run(async () =>
                {
                    var transaccion = "REVERSO";
                    var jsonTsk = await new PolicyDA().invocarServicio_EPS_TRA(Convert.ToInt32(request.P_NID_COTIZACION.ToString()), JsonConvert.SerializeObject(epsAnu), transaccion);
                    asincObject = JsonConvert.DeserializeObject<Response_EPS_Transaccion>(jsonTsk);

                    bool isSuccess = asincObject == null ? false : asincObject.success;

                    if (!isSuccess)
                    {
                        string errorMessage = "Servicio EPS - SCTR - transacción: " + asincObject.message;
                        new QuotationDA().InsertLog(Convert.ToInt64(request.P_NID_COTIZACION.ToString()), "03 - Error en Servicio EPS - Transaccion de Poliza", URI_SendEmision_EPS, JsonConvert.SerializeObject(asincObject), null);
                        insEPS = (new PolicyDA().insertErrorEPS(request.P_NID_COTIZACION.ToString(), JsonConvert.SerializeObject(epsAnu), request.P_NID_PROC_EPS, request.P_NID_PROC)).P_COD_ERR;
                    }
                    else
                    {
                        string mensaje = "SE REALIZO LA ANULACIÓN CORRECTAMENTE";
                        insEPS = new PolicyDA().insert_policy_EPS(long.Parse(request.P_NPOLICY_SALUD), asincObject.data.idContrato, request.P_NCOT_MIXTA == 1 ? request.P_NID_PROC_EPS : request.P_NID_PROC, mensaje);
                    }

                });
            }

            return insEPS;
        }


        public ResponseVM valBilling(ValBillingBM data)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseVM result = new ResponseVM();
            string storedProcedureName = ProcedureName.pkg_ValidaReglas + ".SPS_VAL_GENERATE_FACT";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMOVEMENT", OracleDbType.Int32, data.P_NMOVEMENT, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Varchar2, result.P_NCODE, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, result.P_SMESSAGE, ParameterDirection.Output);

                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;
                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                result.P_NCODE = P_NCODE.Value.ToString();
                result.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                result.P_NCODE = "1";
                result.P_SMESSAGE = ex.ToString();
                LogControl.save("valBilling", ex.ToString(), "3");
            }

            return result;
        }

        public Entities.QuotationModel.ViewModel.QuotationResponseVM RenewMod(QuotationCabBM request)
        {
            var response = new Entities.QuotationModel.ViewModel.QuotationResponseVM();
            DbConnection DataConnection = ConnectionGet(enuTypeDataBase.OracleVTime);
            DbTransaction trx = null;
            DataConnection.Open();
            trx = DataConnection.BeginTransaction();

            var FlagList = new List<Entities.QuotationModel.ViewModel.QuotationResponseVM>();

            //ELog.saveJson("RenewMod", JsonConvert.SerializeObject(request));

            try
            {
                List<HttpPostedFile> files = new List<HttpPostedFile>();
                foreach (var item in HttpContext.Current.Request.Files)
                {
                    HttpPostedFile arch = HttpContext.Current.Request.Files[item.ToString()];
                    files.Add((HttpPostedFile)HttpContext.Current.Request.Files[item.ToString()]);
                }

                string folderPath = files.Count > 0 ? ELog.obtainConfig("pathCotizacion") + request.NumeroCotizacion + "\\" + ELog.obtainConfig("movimiento") + "\\" + DateTime.Now.ToString("yyyyMMddhhmmss") + "\\" : String.Empty;
                request.P_SRUTA = folderPath;
                response.P_COD_ERR = 0;

                // Actualizar cotizacion
                var lRamosAp = new string[] { ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch"),
                                                  ELog.obtainConfig("vidaLeyBranch") , ELog.obtainConfig("sctrBranch")};
                if (lRamosAp.Contains(request.P_NBRANCH.ToString()))
                {
                    if (request.P_STRAN == "RC")
                    {
                        response = QuotationDA.UpdateQuotationCab(request, DataConnection, trx);
                    }
                    else
                    {
                        response = QuotationDA.ApproveQuotation(request, DataConnection, trx);
                    }
                }

                if (request.QuotationDet != null)
                {
                    foreach (var item in request.QuotationDet)
                    {
                        if (response.P_COD_ERR == 0)
                        {
                            item.P_NID_COTIZACION = Convert.ToInt32(request.NumeroCotizacion);

                            // Inserción de cabecera para productos sin tabla de calculos - Revisar covid
                            var lRamosSC = new string[] { ELog.obtainConfig("vidaLeyBranch"), ELog.obtainConfig("sctrBranch") };
                            if (lRamosSC.Contains(request.P_NBRANCH.ToString()) && request.P_STRAN == "RC" && request.TrxCode == "EX")
                            {
                                response = QuotationDA.UpdateQuotationDetRC(request, item, DataConnection, trx);
                            }
                            else
                            {
                                if (lRamosSC.Contains(request.P_NBRANCH.ToString()))
                                {
                                    response = QuotationDA.InsertQuotationDet(request, item, DataConnection, trx);
                                    FlagList.Add(response);
                                }
                            }

                            // Inserción de cabecera para productos con tabla de calculos
                            var lRamosCC = new string[] { ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch") };
                            if (lRamosCC.Contains(request.P_NBRANCH.ToString()))
                            {
                                response = QuotationDA.InsertQuotationDetAP(request, item, DataConnection, trx);
                                FlagList.Add(response);
                            }
                        }
                    }
                }

                if (response.P_COD_ERR == 0 && request.P_NBRANCH == Convert.ToInt64(ELog.obtainConfig("vidaLeyBranch")) && request.TrxCode != "EX")
                {
                    response = QuotationDA.UpdateQuotationDetPremium(request, request.NumeroCotizacion, DataConnection, trx);
                }

                if (response.P_COD_ERR == 0 && request.P_NBRANCH == Convert.ToInt64(ELog.obtainConfig("sctrBranch")) && request.TrxCode != "EX")
                {
                    response = QuotationDA.UpdateQuotationDetPremium(request, request.NumeroCotizacion, DataConnection, trx);
                }

                if (request.TrxCode != "EX")
                {
                    if (response.P_COD_ERR == 0)
                    {
                        if (request.P_STRAN == "RC")
                        {
                            if (request.P_NBRANCH == Convert.ToInt64(ELog.obtainConfig("vidaLeyBranch"))
                            && request.NumeroCotizacion != null && request.NumeroCotizacion.Value != 0)
                            {
                                response = QuotationDA.UpdateDetalleModFinal(request, DataConnection, trx);
                                //response.P_NID_COTIZACION = NID_COTIZACION.ToString();
                            }

                            if (request.P_NBRANCH == Convert.ToInt64(ELog.obtainConfig("sctrBranch"))
                            && request.NumeroCotizacion != null && request.NumeroCotizacion.Value != 0)
                            {
                                response = QuotationDA.UpdateDetalleModFinal(request, DataConnection, trx);
                                //response.P_NID_COTIZACION = NID_COTIZACION.ToString();
                            }
                        }

                        if (request.QuotationCom != null)
                        {
                            var num = 0;
                            foreach (var item in request.QuotationCom)
                            {
                                if (response.P_COD_ERR == 0)
                                {
                                    item.P_NID_COTIZACION = Convert.ToInt32(request.NumeroCotizacion);
                                    item.P_NPRINCIPAL = num;
                                    response = QuotationDA.InsertQuotationCom(request, item, DataConnection, trx);
                                }
                                num++;
                            }
                        }
                    }
                }

                // Revisar covid no entra
                if (lRamosAp.Contains(request.P_NBRANCH.ToString()))
                {
                    if (response.P_COD_ERR == 0)
                    {
                        if (request.P_NBRANCH == Convert.ToInt32(ELog.obtainConfig("vidaLeyBranch")))
                        {
                            response = QuotationDA.UpdateCodQuotation(request.NumeroCotizacion.Value, request.CodigoProceso, DataConnection, trx);
                        }
                        else if (request.P_NBRANCH == Convert.ToInt32(ELog.obtainConfig("sctrBranch")))
                        {
                            response = QuotationDA.UpdateCodQuotation(request.NumeroCotizacion.Value, request.CodigoProceso, DataConnection, trx);
                        }
                        else
                        {
                            response = QuotationDA.UpdateCodQuotationPD(request.NumeroCotizacion.ToString(), request, DataConnection, trx);
                        }

                        try
                        {
                            var trama = new ValidaTramaBM()
                            {
                                NID_COTIZACION = request.NumeroCotizacion.Value,
                                NTYPE_TRANSAC = ELog.obtainConfig("trx" + request.TrxCode.ToUpper()), // request.TrxCode == "IN" ? "2" : request.TrxCode == "EX" ? "3" : "4",
                                NID_PROC = request.CodigoProceso
                            };
                            response = new QuotationDA().insertHistTrama(trama, DataConnection, trx);
                        }
                        catch (Exception ex)
                        {
                            response.P_COD_ERR = 1;
                            response.P_MESSAGE = ex.ToString();
                        }
                    }

                    if (response.P_COD_ERR == 0)
                    {
                        int _cambioPlan = 0;

                        if (request.P_NBRANCH == Convert.ToInt64(ELog.obtainConfig("vidaLeyBranch")))
                        {
                            if (request.PlanSeleccionado != null)
                            {
                                if (request.PlanSeleccionado.ToUpper() != request.PlanPropuesto.ToUpper())
                                {
                                    _cambioPlan = 1;
                                }
                            }
                        }

                        if (request.P_NBRANCH == Convert.ToInt64(ELog.obtainConfig("sctrBranch")))
                        {
                            if (request.PlanSeleccionado != null)
                            {
                                if (request.PlanSeleccionado.ToUpper() != request.PlanPropuesto.ToUpper())
                                {
                                    _cambioPlan = 1;
                                }
                            }
                        }

                        if (request.TrxCode != "EN")
                        {
                            // Validar Retroactividad
                            var lRetro = new string[] { ELog.obtainConfig("vidaLeyBranch"), ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch") };
                            if (lRetro.Contains(request.P_NBRANCH.ToString()))
                            {
                                response = QuotationDA.ValRetroCotizacion(request, request.NumeroCotizacion.Value, DataConnection, trx);
                            }


                            if (request.TrxCode == "RE" && request.P_NBRANCH == Convert.ToInt64(ELog.obtainConfig("sctrBranch")) && request.CodigoProceso != "")
                            {
                                response = QuotationDA.ValSCTRRetroactividad(request, request.NumeroCotizacion.Value, DataConnection, trx);

                                if (response.P_SAPROBADO != "N" && response.P_SAPROBADO != "R")
                                {
                                    response = QuotationDA.ValTransaccionSCTR(request.NumeroCotizacion.Value, request.P_NUSERCODE, request.CodigoProceso, DataConnection, trx);
                                }
                                else
                                {
                                    response = QuotationDA.EnvioBandejaTR(request, response, request.NumeroCotizacion.Value, DataConnection, trx);
                                }
                            }
                            else
                            {
                                response = QuotationDA.ValCotizacion(request.NumeroCotizacion.Value, request.P_NUSERCODE, 0, _cambioPlan, DataConnection, trx);
                            }


                        }
                        else
                        {
                            request.TipoEndoso = new string[] { ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch") }.Contains(request.P_NBRANCH.ToString()) ? 1 : 3;
                            response = AjusteEndoso(request, DataConnection, trx);
                            response = response.P_COD_ERR == 0 ? EndosoClose(request, DataConnection, trx) : response;
                        }
                    }

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

                }
            }
            catch (Exception ex)
            {
                response = new quotationVM.QuotationResponseVM();
                response.P_COD_ERR = 1;
                response.P_MESSAGE = ex.ToString();
                LogControl.save("RenewMod", ex.ToString(), "3");
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

        public Entities.QuotationModel.ViewModel.QuotationResponseVM AccPerEndoso(QuotationCabBM request)
        {
            var response = new Entities.QuotationModel.ViewModel.QuotationResponseVM();
            DbConnection DataConnection = ConnectionGet(enuTypeDataBase.OracleVTime);
            DbTransaction trx = null;
            DataConnection.Open();
            trx = DataConnection.BeginTransaction();
            var FlagList = new List<Entities.QuotationModel.ViewModel.QuotationResponseVM>();

            try
            {
                List<HttpPostedFile> files = new List<HttpPostedFile>();
                foreach (var item in HttpContext.Current.Request.Files)
                {
                    HttpPostedFile arch = HttpContext.Current.Request.Files[item.ToString()];
                    files.Add((HttpPostedFile)HttpContext.Current.Request.Files[item.ToString()]);
                }

                string folderPath = files.Count > 0 ? ELog.obtainConfig("pathCotizacion") + request.NumeroCotizacion + "\\" + ELog.obtainConfig("movimiento") + "\\" + DateTime.Now.ToString("yyyyMMddhhmmss") + "\\" : String.Empty;
                request.P_SRUTA = folderPath;

                //if (request.PolizaEditAsegurados == 1) /*Solo para cuando editan datos de los asegurados */
                //{
                //    response = QuotationDA.UpdateCustomerAP(request, DataConnection, trx);
                //}

                //if (request.PolizaEditAsegurados == 0) /* Cuando modifican los datos de la poliza */
                //{
                //    request.CodigoProceso = (request.P_NUSERCODE + ELog.generarCodigo() + DateTime.Now.ToString("yyyyMMddHHmmss")).PadLeft(30, '0');
                //    response = InsCargaMasSctr(request, DataConnection, trx);
                //    // y necesito que se llame al tarifario
                //    // despues de este hay que llamar a DEL_INSURED_COT INS_INSURED_COT

                //    if (response.P_COD_ERR == 0)
                //    {
                //        response = InsDetApEndooso(request, DataConnection, trx);
                //    }


                //    if (response.P_COD_ERR == 0)
                //    {
                //        response = QuotationDA.ApproveQuotationEndoso(request, DataConnection, trx);

                //        if (request.QuotationDet != null)
                //        {
                //            foreach (var item in request.QuotationDet)
                //            {
                //                if (response.P_COD_ERR == 0)
                //                {
                //                    item.P_NID_COTIZACION = Convert.ToInt32(request.NumeroCotizacion);
                //                    response = QuotationDA.InsertQuotationDetAP(request, item, DataConnection, trx);
                //                    FlagList.Add(response);
                //                }
                //            }
                //        }
                //    }
                //    /////* comentado porque se quito funcionalidad de endosar BK - 21/04/2021 */////
                //    /*if (response.P_COD_ERR == 0)
                //    {
                //        if (request.QuotationCom != null)
                //        {
                //            var num = 0;
                //            foreach (var item in request.QuotationCom)
                //            {
                //                if (response.P_COD_ERR == 0)
                //                {
                //                    item.P_NID_COTIZACION = Convert.ToInt32(request.NumeroCotizacion);
                //                    item.P_NPRINCIPAL = num;
                //                    response = QuotationDA.InsertQuotationComEndoso(request, item, DataConnection, trx);
                //                }
                //                num++;
                //            }

                //        }
                //    } */

                //    var lEndoso = new string[] { ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch") };
                //    if (lEndoso.Contains(request.P_NBRANCH.ToString()) && response.P_COD_ERR == 0)
                //    {
                //        if (response.P_COD_ERR == 0 && response.P_NID_COTIZACION != "0")
                //        {
                //            response = QuotationDA.UpdateCodQuotationPD(request.NumeroCotizacion.ToString(), request, DataConnection, trx);

                //            if (response.P_COD_ERR == 0)
                //            {
                //                var trama = new ValidaTramaBM()
                //                {
                //                    NID_COTIZACION = request.NumeroCotizacion.Value,
                //                    NTYPE_TRANSAC = request.TrxCode == "IN" ? "2" : request.TrxCode == "EN" ? "8" : "4",
                //                    NID_PROC = request.CodigoProceso
                //                };
                //                response = new QuotationDA().insertHistTrama(trama, DataConnection, trx);
                //            }
                //        }

                //        if (response.P_COD_ERR == 0)
                //        {
                //            response = QuotationDA.ValCotizacion(request.NumeroCotizacion.Value, request.P_NUSERCODE, 0, 0, DataConnection, trx);
                //            response.P_COD_ERR = response.P_NCODE;
                //            response.P_MESSAGE = response.P_SMESSAGE;
                //            response.P_NIDPROC = request.CodigoProceso;

                //            if (files.Count > 0)
                //            {
                //                if (!Directory.Exists(String.Format(ELog.obtainConfig("pathPrincipal"), folderPath)))
                //                {
                //                    Directory.CreateDirectory(String.Format(ELog.obtainConfig("pathPrincipal"), folderPath));
                //                }

                //                foreach (var file in files)
                //                {
                //                    file.SaveAs(String.Format(ELog.obtainConfig("pathPrincipal"), folderPath) + file.FileName);
                //                }
                //            }

                //        }
                //    }
                //}

                /////*comentado porque se quito funcionalidad de endosar BK - 21/04/2021 */////
                /* else
                {
                    if (response.P_COD_ERR == 0)
                    {
                        if (request.QuotationCom != null)
                        {
                            var num = 0;
                            foreach (var item in request.QuotationCom)
                            {
                                if (response.P_COD_ERR == 0)
                                {
                                    item.P_NID_COTIZACION = Convert.ToInt32(request.NumeroCotizacion);
                                    item.P_NPRINCIPAL = num;
                                    response = QuotationDA.InsertQuotationComEndoso(request, item, DataConnection, trx);
                                }
                                num++;
                            }

                            if (response.P_COD_ERR == 0)
                            {
                                response = QuotationDA.InsertHistorialComEndoso(request, DataConnection, trx);
                                response.P_COD_ERR = response.P_NERROR;
                                response.P_MESSAGE = response.P_SMENSAJE;
                            }
                        }
                    }
                } */
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = ex.ToString();
                LogControl.save("AccPerEndoso", ex.ToString(), "3");
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

        public SalidadPolizaEmit transactionPolicy(PolicyTransactionSaveBM data)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            SalidadPolizaEmit resultPackage = new SalidadPolizaEmit();
            string storedProcedureName = ProcedureName.pkg_Poliza + ".INS_JOB_SCTR";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                //parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Date, Convert.ToDateTime(data.P_DEFFECDATE), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Date, data.P_NTYPE_TRANSAC == 7 ? Convert.ToDateTime(data.P_DSTARTDATE_POL) : Convert.ToDateTime(data.P_DEFFECDATE), ParameterDirection.Input)); ;  // RI - LNSR
                parameter.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, Convert.ToDateTime(data.P_DEXPIRDAT), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, data.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Int32, data.P_STRAN.Trim() == "DE" || data.P_STRAN.Trim() == "Declaración" ? 11 : data.P_NTYPE_TRANSAC, ParameterDirection.Input)); // "Declaración" para recotizacion
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, data.P_NID_PROC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FACT_MES_VENCIDO", OracleDbType.Int32, data.P_FACT_MES_VENCIDO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SFLAG_FAC_ANT", OracleDbType.Char, data.P_SFLAG_FAC_ANT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOLTIMRE", OracleDbType.Char, data.P_SCOLTIMRE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPAYFREQ", OracleDbType.Int32, data.P_NPAYFREQ, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMOV_ANUL", OracleDbType.Int32, data.P_NMOV_ANUL, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NNULLCODE", OracleDbType.Int32, data.P_NNULLCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOMMENT", OracleDbType.Varchar2, data.P_SCOMMENT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SRUTA", OracleDbType.Varchar2, data.P_SRUTA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDPAYMENT", OracleDbType.Int32, data.P_NIDPAYMENT == null ? 0 : data.P_NIDPAYMENT, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, 9000, resultPackage.P_MESSAGE, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, resultPackage.P_COD_ERR, ParameterDirection.Output);
                OracleParameter P_NCONSTANCIA = new OracleParameter("P_NCONSTANCIA", OracleDbType.Int32, resultPackage.P_NCONSTANCIA, ParameterDirection.Output);

                parameter.Add(P_MESSAGE);
                parameter.Add(P_COD_ERR);
                parameter.Add(P_NCONSTANCIA);

                parameter.Add(new OracleParameter("P_DSTARTDATE_POL", OracleDbType.Date, data.P_DSTARTDATE_POL, ParameterDirection.Input));         //Add R.P.
                parameter.Add(new OracleParameter("P_DEXPIRDAT_POL", OracleDbType.Date, data.P_DEXPIRDAT_POL, ParameterDirection.Input));           //Add R.P.
                parameter.Add(new OracleParameter("P_NAMO_AFEC", OracleDbType.Decimal, data.P_NAMO_AFEC, ParameterDirection.Input));                  //RI
                parameter.Add(new OracleParameter("P_NIVA", OracleDbType.Decimal, data.P_NIVA, ParameterDirection.Input));                            //RI
                parameter.Add(new OracleParameter("P_NAMOUNT", OracleDbType.Decimal, data.P_NAMOUNT, ParameterDirection.Input));                      //RI
                parameter.Add(new OracleParameter("P_NDE", OracleDbType.Decimal, data.P_NDE, ParameterDirection.Input));                              //RI
                parameter.Add(new OracleParameter("P_NMOV_ANUL_PH", OracleDbType.Double, data.P_NMOVEMENT_PH, ParameterDirection.Input));             //RI - Reverso
                parameter.Add(new OracleParameter("P_DEVOLVPRI", OracleDbType.Int16, data.P_DEVOLVPRI == null ? 0 : data.P_DEVOLVPRI, ParameterDirection.Input)); // Add Devol Prim Cobrada - JTV 19092022
                parameter.Add(new OracleParameter("P_NCOT_MIXTA", OracleDbType.Int32, data.P_NCOT_MIXTA, ParameterDirection.Input)); // poliza mixta sctr
                parameter.Add(new OracleParameter("P_NCOT_NC", OracleDbType.Int32, data.P_NCOT_NC, ParameterDirection.Input)); //pry nc AVS 17/03/2023
                parameter.Add(new OracleParameter("P_SCOMMENT_FACT", OracleDbType.Varchar2, data.P_SCOMMENT_FACT, ParameterDirection.Input)); //AVS - PRY FACTURACION 23/08/2023

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                resultPackage.P_MESSAGE = P_MESSAGE.Value.ToString();
                resultPackage.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                resultPackage.P_NCONSTANCIA = P_NCONSTANCIA.Value.ToString() != "null" ? Convert.ToInt32(P_NCONSTANCIA.Value.ToString()) : 0;
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                resultPackage.P_COD_ERR = 1;
                resultPackage.P_MESSAGE = ex.ToString();
                LogControl.save("transactionPolicy", ex.ToString(), "3");
            }

            return resultPackage;
        }

        public SalidadPolizaEmit AnulMovementPolicy(PolicyTransactionSaveBM data)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            SalidadPolizaEmit resultPackage = new SalidadPolizaEmit();
            string storedProcedureName = ProcedureName.pkg_Poliza + ".INS_JOB_SCTR";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Date, DateTime.Now.ToShortDateString(), ParameterDirection.Input));  // RI - LNSR
                parameter.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, data.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Int32, data.P_NTYPE_TRANSAC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FACT_MES_VENCIDO", OracleDbType.Int32, 0, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SFLAG_FAC_ANT", OracleDbType.Char, "", ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOLTIMRE", OracleDbType.Char, "", ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPAYFREQ", OracleDbType.Int32, 0, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMOV_ANUL", OracleDbType.Int32, data.P_NMOV_ANUL, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NNULLCODE", OracleDbType.Int32, data.P_NBRANCH == "77" ? data.P_NNULLCODE : 1, ParameterDirection.Input)); // AGF 26122023 SAPSA
                parameter.Add(new OracleParameter("P_SCOMMENT", OracleDbType.Varchar2, data.P_SCOMMENT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SRUTA", OracleDbType.Varchar2, "", ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDPAYMENT", OracleDbType.Int32, data.P_NIDPAYMENT == null ? 0 : data.P_NIDPAYMENT, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, resultPackage.P_MESSAGE, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, resultPackage.P_COD_ERR, ParameterDirection.Output);
                OracleParameter P_NCONSTANCIA = new OracleParameter("P_NCONSTANCIA", OracleDbType.Int32, resultPackage.P_NCONSTANCIA, ParameterDirection.Output);

                P_MESSAGE.Size = 4000;
                parameter.Add(P_MESSAGE);
                parameter.Add(P_COD_ERR);
                parameter.Add(P_NCONSTANCIA);

                parameter.Add(new OracleParameter("P_DSTARTDATE_POL", OracleDbType.Date, data.P_DSTARTDATE_POL, ParameterDirection.Input));         // RI - LNSR
                parameter.Add(new OracleParameter("P_DEXPIRDAT_POL", OracleDbType.Date, data.P_DEXPIRDAT_POL, ParameterDirection.Input));           // RI - LNSR
                parameter.Add(new OracleParameter("P_NAMO_AFEC", OracleDbType.Decimal, data.P_NAMO_AFEC, ParameterDirection.Input));                  //RI
                parameter.Add(new OracleParameter("P_NIVA", OracleDbType.Decimal, data.P_NIVA, ParameterDirection.Input));                            //RI
                parameter.Add(new OracleParameter("P_NAMOUNT", OracleDbType.Decimal, data.P_NAMOUNT, ParameterDirection.Input));                      //RI
                parameter.Add(new OracleParameter("P_NDE", OracleDbType.Decimal, data.P_NDE, ParameterDirection.Input));                              //RI
                parameter.Add(new OracleParameter("P_NMOV_ANUL_PH", OracleDbType.Double, data.P_NMOVEMENT_PH, ParameterDirection.Input));             //RI - Reverso
                parameter.Add(new OracleParameter("P_DEVOLVPRI", OracleDbType.Int16, data.P_DEVOLVPRI == null ? 0 : data.P_DEVOLVPRI, ParameterDirection.Input)); // Add Devol Prim Cobrada - JTV 19092022
                parameter.Add(new OracleParameter("P_NCOT_MIXTA", OracleDbType.Int32, data.P_NCOT_MIXTA, ParameterDirection.Input)); // poliza mixta sctr
                parameter.Add(new OracleParameter("P_NCOT_NC", OracleDbType.Int32, data.P_NCOT_NC, ParameterDirection.Input)); //pry nc AVS 17/03/2023
                parameter.Add(new OracleParameter("P_SCOMMENT_FACT", OracleDbType.Varchar2, data.P_SCOMMENT_FACT, ParameterDirection.Input)); //AVS - PRY FACTURACION 23/08/2023

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                resultPackage.P_MESSAGE = P_MESSAGE.Value.ToString();
                resultPackage.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                resultPackage.P_NCONSTANCIA = Convert.ToInt32(P_NCONSTANCIA.Value.ToString());
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                resultPackage.P_COD_ERR = 1;
                resultPackage.P_MESSAGE = ex.ToString();
                LogControl.save("AnulMovementPolicy", ex.ToString(), "3");
            }

            return resultPackage;
        }

        public ErrorCode ReverHISDETSCTR(int P_NID_COTIZACION, int P_NMOVEMENT_PH, string P_NID_PROC)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            var response = new HisDetRenovSCTR();
            var errorCode = new ErrorCode();
            string storedProcedureName = ProcedureName.pkg_TecnicaSCTR + ".REVER_SCTR_HIS_DET";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, P_NID_COTIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMOVEMENT_PH", OracleDbType.Int32, P_NMOVEMENT_PH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, P_NID_PROC, ParameterDirection.Input));


                //OUTPUT
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, 9000, errorCode.P_MESSAGE, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, errorCode.P_COD_ERR, ParameterDirection.Output);

                parameter.Add(P_MESSAGE);
                parameter.Add(P_COD_ERR);


                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                errorCode.P_MESSAGE = P_MESSAGE.Value.ToString();
                errorCode.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                errorCode.P_COD_ERR = 1;
                errorCode.P_MESSAGE = ex.ToString();
                LogControl.save("ReverHISDETSCTR", ex.ToString(), "3");
            }

            return errorCode;
        }

        public ErrorCode Udp_SCTRHIS(SavePolicyEmit data)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            var errorCode = new ErrorCode();
            string storedProcedureName = ProcedureName.pkg_TecnicaSCTR + ".UDP_NIDPROCSCTR";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, data.P_NID_PROC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Varchar2, data.P_DEXPIRDAT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DSTARTDATE", OracleDbType.Varchar2, data.P_DSTARTDATE, ParameterDirection.Input));


                //OUTPUT
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, errorCode.P_COD_ERR, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, 9000, errorCode.P_MESSAGE, ParameterDirection.Output);

                parameter.Add(P_COD_ERR);
                parameter.Add(P_MESSAGE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);
                errorCode.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                errorCode.P_MESSAGE = P_MESSAGE.Value.ToString();

                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                errorCode.P_COD_ERR = 1;
                errorCode.P_MESSAGE = ex.ToString();
                LogControl.save("Udp_SCTRHIS", ex.ToString(), "3");
            }

            return errorCode;
        }

        public Entities.PolicyModel.ViewModel.GenericPackageVM GetInsuredPolicyList(InsuredPolicySearchBM data)
        {
            Entities.PolicyModel.ViewModel.GenericPackageVM package = new Entities.PolicyModel.ViewModel.GenericPackageVM();
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<InsuredPolicyVM> list = new List<InsuredPolicyVM>();
            string storedProcedureName = ProcedureName.pkg_ReportesSCTR + ".REA_REPORTE_ASEGURADOS";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Varchar2, data.PolicyNumber, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TIPO_PRODUCTO", OracleDbType.Varchar2, data.ProductType, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TIPO_MOVIMIENTO", OracleDbType.Varchar2, data.MovementType, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FECHA_DESDE", OracleDbType.Date, data.StartDate, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FECHA_HASTA", OracleDbType.Date, data.EndDate, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_MODO", OracleDbType.Varchar2, data.InsuredSearchMode, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TIPDOC", OracleDbType.Varchar2, data.InsuredDocumentType, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUMDOC", OracleDbType.Varchar2, data.InsuredDocumentNumber, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_APEPAT", OracleDbType.Varchar2, data.InsuredPaternalLastName, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_APEMAT", OracleDbType.Varchar2, data.InsuredMaternalLastName, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NAMES", OracleDbType.Varchar2, data.InsuredFirstName, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLIMITPERPAGE", OracleDbType.Int32, data.LimitPerpage, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPAGENUM", OracleDbType.Int32, data.PageNumber, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter NTOTALROWS = new OracleParameter("P_NTOTALROWS", OracleDbType.Int32, 4000, null, ParameterDirection.Output);

                parameter.Add(C_TABLE);
                parameter.Add(NTOTALROWS);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);
                while (odr.Read())
                {
                    InsuredPolicyVM item = new InsuredPolicyVM();
                    item.Product = odr["NOMBRE_PRODUCTO"].ToString();
                    item.Movement = odr["TIPO_MOVIMIENTO"].ToString();

                    if (odr["FEC_EFECTO"].ToString() != null && odr["FEC_EFECTO"].ToString().Trim() != "") item.StartDate = Convert.ToDateTime(odr["FEC_EFECTO"].ToString());
                    else item.StartDate = null;
                    if (odr["FEC_FIN"].ToString() != null && odr["FEC_FIN"].ToString().Trim() != "") item.EndDate = Convert.ToDateTime(odr["FEC_FIN"].ToString());
                    else item.EndDate = null;

                    item.PolicyNumber = odr["NRO_POLIZA"].ToString();

                    item.InsuredDocumentType = odr["TIPO_DOCUMENTO_ASEGURADO"].ToString();
                    item.InsuredDocumentNumber = odr["NRO_DOCUMENTO_ASEGURADO"].ToString();
                    item.InsuredFullName = odr["NOMBRE_ASEGURADO"].ToString();

                    item.ContractorDocumentType = odr["TIPO_DOCUMENTO_CONTRATANTE"].ToString();
                    item.ContractorDocumentNumber = odr["NRO_DOCUMENTO_CONTRATANTE"].ToString();
                    item.ContractorFullName = odr["NOMBRE_CONTRATANTE"].ToString();

                    item.BrokerDocumentType = odr["TIPO_DOCUMENTO_BROKER"].ToString();
                    item.BrokerDocumentNumber = odr["NRO_DOCUMENTO_BROKER"].ToString();
                    item.BrokerFullName = odr["NOMBRE_BROKER"].ToString();

                    item.ContractorLocation = odr["NOMBRE_SEDE"].ToString();

                    list.Add(item);
                }
                ELog.CloseConnection(odr);

                package.TotalRowNumber = Convert.ToInt32(NTOTALROWS.Value.ToString());
                package.GenericResponse = list;
            }
            catch (Exception ex)
            {
                LogControl.save("GetInsuredPolicyList", ex.ToString(), "3");
            }

            return package;
        }

        public Entities.PolicyModel.ViewModel.GenericPackageVM GetPolicyProofList(PolicyProofSearchBM data)
        {
            Entities.PolicyModel.ViewModel.GenericPackageVM package = new Entities.PolicyModel.ViewModel.GenericPackageVM();
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<PolicyProofVM> list = new List<PolicyProofVM>();
            string storedProcedureName = ProcedureName.pkg_ReportesSCTR + ".REA_REPORTE_CONSTANCIAS";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_CONSTANCIA", OracleDbType.Varchar2, data.ProofNumber, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TIPO_PRODUCTO", OracleDbType.Varchar2, data.ProductType, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TIPO_MOVIMIENTO", OracleDbType.Varchar2, data.MovementType, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FECHA_DESDE", OracleDbType.Date, data.StartDate, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FECHA_HASTA", OracleDbType.Date, data.EndDate, ParameterDirection.Input));

                parameter.Add(new OracleParameter("P_MODO", OracleDbType.Varchar2, data.InsuredSearchMode, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TIPDOC", OracleDbType.Varchar2, data.InsuredDocumentType, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUMDOC", OracleDbType.Varchar2, data.InsuredDocumentNumber, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_APEPAT", OracleDbType.Varchar2, data.InsuredPaternalLastName, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_APEMAT", OracleDbType.Varchar2, data.InsuredMaternalLastName, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NAMES", OracleDbType.Varchar2, data.InsuredFirstName, ParameterDirection.Input));

                parameter.Add(new OracleParameter("P_NLIMITPERPAGE", OracleDbType.Int32, data.LimitPerpage, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPAGENUM", OracleDbType.Int32, data.PageNumber, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter NTOTALROWS = new OracleParameter("P_NTOTALROWS", OracleDbType.Int32, 4000, null, ParameterDirection.Output);

                parameter.Add(C_TABLE);
                parameter.Add(NTOTALROWS);

                /* PREFILES -DGC - 30/04/2024 */
                //parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int64, data.usercode, ParameterDirection.Input));

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);
                while (odr.Read())
                {
                    PolicyProofVM item = new PolicyProofVM();
                    item.ProofNumber = odr["CONSTANCIA"].ToString();
                    item.Product = odr["NOMBRE_PRODUCTO"].ToString();
                    item.Movement = odr["TIPO_MOVIMIENTO"].ToString();

                    if (odr["FEC_EFECTO"].ToString() != null && odr["FEC_EFECTO"].ToString().Trim() != "") item.StartDate = Convert.ToDateTime(odr["FEC_EFECTO"].ToString());
                    else item.StartDate = null;
                    if (odr["FEC_FIN"].ToString() != null && odr["FEC_FIN"].ToString().Trim() != "") item.EndDate = Convert.ToDateTime(odr["FEC_FIN"].ToString());
                    else item.EndDate = null;
                    if (odr["FEC_OPERACION"].ToString() != null && odr["FEC_OPERACION"].ToString().Trim() != "") item.TransactionDate = Convert.ToDateTime(odr["FEC_OPERACION"].ToString());
                    else item.TransactionDate = null;

                    item.PolicyNumber = odr["NRO_POLIZA"].ToString();

                    item.InsuredDocumentType = odr["TIPO_DOCUMENTO_ASEGURADO"].ToString();
                    item.InsuredDocumentNumber = odr["NRO_DOCUMENTO_ASEGURADO"].ToString();
                    item.InsuredFullName = odr["NOMBRE_ASEGURADO"].ToString();

                    item.ContractorDocumentType = odr["TIPO_DOCUMENTO_CONTRATANTE"].ToString();
                    item.ContractorDocumentNumber = odr["NRO_DOCUMENTO_CONTRATANTE"].ToString();
                    item.ContractorFullName = odr["NOMBRE_CONTRATANTE"].ToString();

                    item.BrokerDocumentType = odr["TIPO_DOCUMENTO_BROKER"].ToString();
                    item.BrokerDocumentNumber = odr["NRO_DOCUMENTO_BROKER"].ToString();
                    item.BrokerFullName = odr["NOMBRE_BROKER"].ToString();

                    item.ContractorLocation = odr["NOMBRE_SEDE"].ToString();
                    string[] files = null;
                    var myRegex = new Regex(@"constancia"); //CRVQ Cambios usuario Petroperú 25102022
                    var myRegex2 = new Regex(@"inclusion"); //CRVQ Cambios usuario Petroperú 25102022
                    var myRegex3 = new Regex(@"exclusion"); //CRVQ Cambios usuario Petroperú 25102022

                    try
                    {
                        if (odr["MOVIMIENTO"].ToString() != ELog.obtainConfig("neteoKey"))
                        {
                            //CRVQ Cambios usuario Petroperú 25102022
                            files = Directory.GetFileSystemEntries(String.Format(ELog.obtainConfig("pathpdf") + ELog.obtainConfig("pathPoliza") + odr["NRO_COTIZACION"].ToString() + "\\" + ELog.obtainConfig("movimiento") + "\\" + ELog.obtainConfig("movimiento" + odr["MOVIMIENTO_ID"].ToString()) + "\\" + odr["MOVIMIENTO_ID"].ToString() + "\\", "*.*"));
                            files = files.Where(f => myRegex.IsMatch(f)).ToArray();
                        }
                        else
                        {
                            if (odr["COD_NETEO"].ToString() == ELog.obtainConfig("inclusionKey"))
                            {
                                files = Directory.GetFileSystemEntries(String.Format(ELog.obtainConfig("pathpdf") + ELog.obtainConfig("pathPoliza") + odr["NRO_COTIZACION"].ToString() + "\\" + ELog.obtainConfig("movimiento") + "\\" + ELog.obtainConfig("movimiento" + odr["MOVIMIENTO_ID"].ToString()) + "\\" + odr["MOVIMIENTO_ID"].ToString() + "\\", "*.*"));
                                files = files.Where(f => myRegex2.IsMatch(f)).ToArray();
                            }
                            else if (odr["COD_NETEO"].ToString() == ELog.obtainConfig("exclusionKey"))
                            {
                                files = Directory.GetFileSystemEntries(String.Format(ELog.obtainConfig("pathpdf") + ELog.obtainConfig("pathPoliza") + odr["NRO_COTIZACION"].ToString() + "\\" + ELog.obtainConfig("movimiento") + "\\" + ELog.obtainConfig("movimiento" + odr["MOVIMIENTO_ID"].ToString()) + "\\" + odr["MOVIMIENTO_ID"].ToString() + "\\", "*.*"));
                                files = files.Where(f => myRegex3.IsMatch(f)).ToArray();
                            }
                        }
                    }
                    catch (DirectoryNotFoundException ex)
                    {
                        files = null;
                        LogControl.save("GetPolicyProofList", ex.ToString(), "3");
                    }

                    item.FilePath = files != null && files.Length > 0 ? files[0] : String.Empty;
                    list.Add(item);
                }
                ELog.CloseConnection(odr);

                package.TotalRowNumber = Convert.ToInt32(NTOTALROWS.Value.ToString());
                package.GenericResponse = list;
            }
            catch (Exception ex)
            {
                LogControl.save("GetPolicyProofList", ex.ToString(), "3");
            }
            return package;
        }

        public List<PolicyTransactionAllVM> GetPolicyTransactionAllList(PolicyTransactionAllSearchBM data)
        {
            Entities.PolicyModel.ViewModel.GenericPackageVM package = new Entities.PolicyModel.ViewModel.GenericPackageVM();
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<PolicyTransactionAllVM> list = new List<PolicyTransactionAllVM>();
            string storedProcedureName = ProcedureName.pkg_ReportesPD + ".REA_POL_TRANSACCION";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NTYPE_HIST", OracleDbType.Double, data.NTYPE_HIST, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Double, data.NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Double, data.NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Double, data.NPOLICY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_DOC", OracleDbType.Double, data.NTYPE_DOC, ParameterDirection.Input));

                parameter.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, data.SIDDOC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SFIRSTNAME", OracleDbType.Varchar2, data.SFIRSTNAME, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLASTNAME", OracleDbType.Varchar2, data.SLASTNAME, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLASTNAME2", OracleDbType.Varchar2, data.SLASTNAME2, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLEGALNAME", OracleDbType.Varchar2, data.SLEGALNAME, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DINI", OracleDbType.Varchar2, data.DINI, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DFIN", OracleDbType.Varchar2, data.DFIN, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, data.NUSERCODE, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);

                parameter.Add(C_TABLE);

                //BROKER - LNSR
                parameter.Add(new OracleParameter("P_NTYPE_DOC_BR", OracleDbType.Double, data.NTYPE_DOC_BR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SIDDOC_BR", OracleDbType.Varchar2, data.SIDDOC_BR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SFIRSTNAME_BR", OracleDbType.Varchar2, data.SFIRSTNAME_BR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLASTNAME_BR", OracleDbType.Varchar2, data.SLASTNAME_BR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLASTNAME2_BR", OracleDbType.Varchar2, data.SLASTNAME2_BR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLEGALNAME_BR", OracleDbType.Varchar2, data.SLEGALNAME_BR, ParameterDirection.Input));

                //parameter.Add(NTOTALROWS);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);
                while (odr.Read())
                {
                    try
                    {
                        PolicyTransactionAllVM item = new PolicyTransactionAllVM();
                        item.SRAMO = odr["SRAMO"] == DBNull.Value ? "" : odr["SRAMO"].ToString();
                        item.SPRODUCTO = odr["SPRODUCTO"] == DBNull.Value ? "" : odr["SPRODUCTO"].ToString();
                        item.NPOLIZA = odr["NPOLIZA"] == DBNull.Value ? 0 : double.Parse(odr["NPOLIZA"].ToString());
                        item.SCOTIZACION = odr["SCOTIZACION"] == DBNull.Value ? "" : odr["SCOTIZACION"].ToString();  // MRC
                        item.SCONTRATANTE = odr["SCONTRATANTE"] == DBNull.Value ? "" : odr["SCONTRATANTE"].ToString();
                        item.NFLAG_TRAMA = Convert.ToInt32(odr["FLAG_TRAMA"] == DBNull.Value ? null : odr["FLAG_TRAMA"].ToString());
                        item.SBROKER = odr["SBROKER"] == DBNull.Value ? "" : odr["SBROKER"].ToString();

                        item.NTIP_RENOV = Convert.ToInt32(odr["NTIP_RENOV"] == DBNull.Value ? null : odr["NTIP_RENOV"].ToString());

                        item.NPAYFREQ = Convert.ToInt32(odr["NPAYFREQ"] == DBNull.Value ? null : odr["NPAYFREQ"].ToString());

                        item.DFECHA_EMISION = odr["DFECHA_EMISION"] == DBNull.Value ? "" : String.Format("{0:dd/MM/yyyy}", odr["DFECHA_EMISION"]);
                        item.DINICIO_VIGENCIA = odr["DINICIO_VIGENCIA"] == DBNull.Value ? "" : String.Format("{0:dd/MM/yyyy}", odr["DINICIO_VIGENCIA"]);
                        item.DFIN_VIGENCIA = odr["DFIN_VIGENCIA"] == DBNull.Value ? "" : String.Format("{0:dd/MM/yyyy}", odr["DFIN_VIGENCIA"]);


                        item.DINICIO_VIGENCIA_ASEG = odr["DINICIO_VIGENCIA_ASEG"] == DBNull.Value ? "" : String.Format("{0:dd/MM/yyyy}", odr["DINICIO_VIGENCIA_ASEG"]);
                        item.DFIN_VIGENCIA_ASEG = odr["DFIN_VIGENCIA_ASEG"] == DBNull.Value ? "" : String.Format("{0:dd/MM/yyyy}", odr["DFIN_VIGENCIA_ASEG"]);

                        item.NBRANCH = odr["NBRANCH"] == DBNull.Value ? 0 : double.Parse(odr["NBRANCH"].ToString());
                        item.NPRODUCT = odr["NPRODUCT"] == DBNull.Value ? 0 : double.Parse(odr["NPRODUCT"].ToString());
                        item.NID_COTIZACION = odr["NRO_COTIZACION"] == DBNull.Value ? 0 : int.Parse(odr["NRO_COTIZACION"].ToString()); //MRC
                        item.SDES_TYPE_PRODUCT = odr["SDES_TYPE_PRODUCT"] == DBNull.Value ? "" : odr["SDES_TYPE_PRODUCT"].ToString(); //ACCPER

                        /*if (odr["DFECHA_EMISION"].ToString() != null && odr["DFECHA_EMISION"].ToString().Trim() != "") item.DFECHA_EMISION = Convert.ToDateTime(odr["DFECHA_EMISION"].ToString());
                        else item.DFECHA_EMISION = null;

                        if (odr["DINICIO_VIGENCIA"].ToString() != null && odr["DINICIO_VIGENCIA"].ToString().Trim() != "") item.DINICIO_VIGENCIA = Convert.ToDateTime(odr["DINICIO_VIGENCIA"].ToString());
                        else item.DINICIO_VIGENCIA = null;

                        if (odr["DFIN_VIGENCIA"].ToString() != null && odr["DFIN_VIGENCIA"].ToString().Trim() != "") item.DFIN_VIGENCIA = Convert.ToDateTime(odr["DFIN_VIGENCIA"].ToString());
                        else item.DFIN_VIGENCIA = null;*/

                        item.NCLIENT_SEG = odr["NCLIENT_SEG"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NCLIENT_SEG"]);
                        item.SCLIENT_SEG = odr["SCLIENT_SEG"] == DBNull.Value ? string.Empty : odr["SCLIENT_SEG"].ToString();
                        item.SDESCRIPT_SEG = odr["SDESCRIPT_SEG"] == DBNull.Value ? string.Empty : odr["SDESCRIPT_SEG"].ToString();
                        item.NFLAG_EXTERNO = odr["NFLAG_EXTERNO"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NFLAG_EXTERNO"]);

                        list.Add(item);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                ELog.CloseConnection(odr);

                //package.TotalRowNumber = Convert.ToInt32(NTOTALROWS.Value.ToString());
                //package.GenericResponse = list;
            }
            catch (Exception ex)
            {
                LogControl.save("GetPolicyTransactionAllList", ex.ToString(), "3");
            }
            return list;
        }

        public string GetPolicyTransactionAllListExcel(PolicyTransactionAllSearchBM data)
        {
            List<PolicyTransactionAllVM> lista = this.GetPolicyTransactionAllList(data);
            string templatePath = string.Empty;
            if (lista[0].NFLAG_EXTERNO == 1)
            {
                templatePath = "D:/doc_templates/reportes/dev/Template_policy-ext.xlsx";
            }
            else
            {
                templatePath = "D:/doc_templates/reportes/dev/Template_policy.xlsx";
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
                    int i = 8;
                    int letra = 65;

                    sl.SetCellValue("B2", data.DINI);
                    sl.SetCellValue("B3", data.DFIN);

                    foreach (PolicyTransactionAllVM item in lista)
                    {
                        //string fecha = "";
                        //try
                        //{
                        //    fecha = item.ApprovalDate == null ? "" : Convert.ToDateTime(item.ApprovalDate).ToString("dd/MM/yyyy hh:mm");
                        //}
                        //catch (Exception)
                        //{
                        //    fecha = "";
                        //}
                        int c = 0;
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.SRAMO);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.SPRODUCTO);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.NPOLIZA);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.SCOTIZACION);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.SCONTRATANTE);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.SBROKER);
                        if (item.NFLAG_EXTERNO == 0)
                        {
                            sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.SDESCRIPT_SEG);
                        }
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.DFECHA_EMISION);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.DINICIO_VIGENCIA);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.DFIN_VIGENCIA);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.DINICIO_VIGENCIA_ASEG);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.DFIN_VIGENCIA_ASEG);
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
                plantilla = "";
                LogControl.save("GetPolicyTransactionAllListExcel", ex.ToString(), "3");
                throw ex;
            }
            return plantilla;
        }

        public List<PolicyMovAllVM> GetPolicyMovementsTransAllList(PolicyTransactionAllSearchBM data)
        {
            GenericTransVM package = new GenericTransVM();
            package.C_TABLE = new List<PolicyMovVM>();
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<PolicyMovAllVM> lista = new List<PolicyMovAllVM>();
            var lRutas = new string[] { ELog.obtainConfig("vidaLeyBranch"), ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch"), ELog.obtainConfig("vidaIndividualBranch"), ELog.obtainConfig("desgravamenBranch") };
            var rutaGen = string.Empty;
            string storedProcedureName = ProcedureName.pkg_ReportesPD + ".REA_POL_MOV";
            try
            {
                // INPUT
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Double, data.NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Double, data.NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Double, data.NPOLICY, ParameterDirection.Input));

                // OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, package.C_TABLE, ParameterDirection.Output);

                parameter.Add(C_TABLE);

                int i = 0;

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);
                while (odr.Read())
                {
                    PolicyMovAllVM item = new PolicyMovAllVM();
                    item.SCERTYPE = odr["SCERTYPE"] == DBNull.Value ? "" : odr["SCERTYPE"].ToString();
                    item.NBRANCH = odr["NBRANCH"] == DBNull.Value ? 0 : double.Parse(odr["NBRANCH"].ToString());
                    item.NPRODUCT = odr["NPRODUCT"] == DBNull.Value ? 0 : double.Parse(odr["NPRODUCT"].ToString());
                    item.NPOLICY = odr["NPOLICY"] == DBNull.Value ? 0 : double.Parse(odr["NPOLICY"].ToString());
                    item.NTYPE_HIST = odr["NTYPE_HIST"] == DBNull.Value ? 0 : double.Parse(odr["NTYPE_HIST"].ToString());
                    item.NTRANSAC = odr["NTRANSAC"] == DBNull.Value ? "" : odr["NTRANSAC"].ToString();
                    item.DES_NTYPE_HIST = odr["DES_NTYPE_HIST"] == DBNull.Value ? "" : odr["DES_NTYPE_HIST"].ToString();
                    item.DES_NTYPE_HIST_PDF = odr["DES_NTYPE_HIST_PDF"] == DBNull.Value ? "" : odr["DES_NTYPE_HIST_PDF"].ToString();
                    item.NUM_MOVIMIENTO = odr["NUM_MOVIMIENTO"] == DBNull.Value ? 0 : double.Parse(odr["NUM_MOVIMIENTO"].ToString());
                    item.DINI_VIGENCIA = odr["DINI_VIGENCIA"] == DBNull.Value ? null : DateTime.Parse(odr["DINI_VIGENCIA"].ToString()).ToShortDateString();
                    var DINI_VIGENCIA_RUTA = odr["NBRANCH"].ToString() == ELog.obtainConfig("vidaLeyBranch") ? DateTime.Parse(odr["DINI_VIGENCIA"].ToString()).ToString("d/MM/yyyy") : DateTime.Parse(odr["DINI_VIGENCIA"].ToString()).ToString("dd/MM/yyyy");
                    item.DFIN_VIGENCIA = odr["DFIN_VIGENCIA"] == DBNull.Value ? null : DateTime.Parse(odr["DFIN_VIGENCIA"].ToString()).ToShortDateString();
                    item.DFECHA_MOVIMIENTO = odr["DFECHA_MOVIMIENTO"] == DBNull.Value ? null : DateTime.Parse(odr["DFECHA_MOVIMIENTO"].ToString()).ToShortDateString();
                    item.SOBSERVACIONES = odr["SOBSERVACIONES"] == DBNull.Value ? "" : odr["SOBSERVACIONES"].ToString();
                    item.FLAG_OBSERVACIONES = 0; //AP
                    item.SUSUARIO = odr["SUSUARIO"] == DBNull.Value ? "" : odr["SUSUARIO"].ToString();
                    item.NIDHEADERPROC = odr["NIDHEADERPROC"] == DBNull.Value ? 0 : double.Parse(odr["NIDHEADERPROC"].ToString());
                    if (lRutas.Contains(odr["NBRANCH"].ToString()))
                    {
                        if (item.NTYPE_HIST == (double)PrintEnum.MovementType.EMISION_POLIZA)
                            rutaGen = ELog.obtainConfig("pathImpresion" + odr["NBRANCH"].ToString()) + data.NPOLICY + "\\" + item.NIDHEADERPROC + "\\" + PrintGenerateUtil.RemoveDiacritics(item.DES_NTYPE_HIST_PDF) + "\\";
                        else if (item.NTYPE_HIST == (double)PrintEnum.MovementType.EMISION_CERTIFICADO && item.NTRANSAC == "E")
                            rutaGen = ELog.obtainConfig("pathImpresion" + odr["NBRANCH"].ToString()) + data.NPOLICY + "\\" + item.NIDHEADERPROC + "\\" + PrintGenerateUtil.RemoveDiacritics(item.DES_NTYPE_HIST_PDF) + "\\";
                        else if (item.NTYPE_HIST == (double)PrintEnum.MovementType.RENOVACION)
                            rutaGen = ELog.obtainConfig("pathImpresion" + odr["NBRANCH"].ToString()) + data.NPOLICY + "\\" + item.NIDHEADERPROC + "\\" + PrintGenerateUtil.RemoveDiacritics(item.DES_NTYPE_HIST_PDF) + "\\" + DINI_VIGENCIA_RUTA.Replace("/", "") + "\\";
                        else if (item.NTYPE_HIST == (double)PrintEnum.MovementType.DECLARACION)
                            rutaGen = ELog.obtainConfig("pathImpresion" + odr["NBRANCH"].ToString()) + data.NPOLICY + "\\" + item.NIDHEADERPROC + "\\" + PrintGenerateUtil.RemoveDiacritics(item.DES_NTYPE_HIST_PDF) + "\\" + DINI_VIGENCIA_RUTA.Replace("/", "") + "\\";
                        else if (item.NTYPE_HIST == (double)PrintEnum.MovementType.INCLUSION_G && item.NTRANSAC == "I")
                            rutaGen = ELog.obtainConfig("pathImpresion" + odr["NBRANCH"].ToString()) + data.NPOLICY + "\\" + item.NIDHEADERPROC + "\\" + PrintGenerateUtil.RemoveDiacritics(item.DES_NTYPE_HIST_PDF) + "\\" + DINI_VIGENCIA_RUTA.Replace("/", "") + "\\";
                        else if (item.NTYPE_HIST == (double)PrintEnum.MovementType.EXCLUSION)
                            rutaGen = ELog.obtainConfig("pathImpresion" + odr["NBRANCH"].ToString()) + data.NPOLICY + "\\" + item.NIDHEADERPROC + "\\" + PrintGenerateUtil.RemoveDiacritics(item.DES_NTYPE_HIST_PDF) + "\\" + DINI_VIGENCIA_RUTA.Replace("/", "") + "\\";
                        else if (item.NTYPE_HIST == (double)PrintEnum.MovementType.ENDOSO)
                            rutaGen = ELog.obtainConfig("pathImpresion" + odr["NBRANCH"].ToString()) + data.NPOLICY + "\\" + item.NIDHEADERPROC + "\\" + PrintGenerateUtil.RemoveDiacritics(item.DES_NTYPE_HIST_PDF) + "\\" + DINI_VIGENCIA_RUTA.Replace("/", "") + "\\";
                        else if (item.NTYPE_HIST == (double)PrintEnum.MovementType.ANULACION)
                            rutaGen = ELog.obtainConfig("pathImpresion" + odr["NBRANCH"].ToString()) + data.NPOLICY + "\\" + item.NIDHEADERPROC + "\\" + PrintGenerateUtil.RemoveDiacritics(item.DES_NTYPE_HIST_PDF) + "\\" + DINI_VIGENCIA_RUTA.Replace("/", "") + "\\";
                    }
                    else
                    {
                        rutaGen = ELog.obtainConfig("pathImpresion") + data.NPOLICY + "\\" + item.NIDHEADERPROC + "\\" + PrintGenerateUtil.RemoveDiacritics(item.DES_NTYPE_HIST_PDF) + "\\";
                    }

                    item.SRUTA = rutaGen;
                    item.SRUTAS_GEN = sharedMethods.GetFilePathList(rutaGen);
                    item.NMOV_ANULADO = odr["NMOV_ANULADO"] == DBNull.Value ? 0 : double.Parse(odr["NMOV_ANULADO"].ToString());
                    item.FACTURAR = odr["FACTURAR"] == DBNull.Value ? 0 : double.Parse(odr["FACTURAR"].ToString());
                    item.RECIBO = odr["RECIBO"] == DBNull.Value ? "" : odr["RECIBO"].ToString();
                    item.NRO_COTIZACION = odr["NRO_COTIZACION"] == DBNull.Value ? 0 : double.Parse(odr["NRO_COTIZACION"].ToString());
                    item.NMOVEMENT_CT = odr["NMOVEMENT_CT"] == DBNull.Value ? 0 : double.Parse(odr["NMOVEMENT_CT"].ToString());
                    item.NMOVEMENT_PH = odr["NMOVEMENT_PH"] == DBNull.Value ? 0 : double.Parse(odr["NMOVEMENT_PH"].ToString());
                    item.NENV_PRINT = odr["NENV_PRINT"] == DBNull.Value ? 0 : int.Parse(odr["NENV_PRINT"].ToString());
                    item.NTYPE_TRANSAC = odr["NTYPE_TRANSAC"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NTYPE_TRANSAC"].ToString()); //ED

                    if (new[] { 1, 2, 4, 8, 11 }.Contains(item.NTYPE_TRANSAC) && i == 0)
                    {
                        item.NCHANGE_INSURED = 1;
                    };

                    lista.Add(item);
                    i++;
                }

                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("GetPolicyMovementsTransAllList", ex.ToString(), "3");
            }
            return lista;
        }

        // ED - ASEGURADO
        public InsuredPolicyResponse GetInsuredForTransactionPolicy(PolicyInsuredFilter _data)
        {
            var response = new InsuredPolicyResponse();
            response.inrureds = new List<InruredList>();

            Entities.PolicyModel.ViewModel.GenericPackageVM package = new Entities.PolicyModel.ViewModel.GenericPackageVM();
            List<OracleParameter> parameter = new List<OracleParameter>();

            string storedProcedureName = ProcedureName.pkg_Poliza + ".REA_INSURED_POLICY";

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, _data.NRO_COTIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_TRANSACTION", OracleDbType.Int32, _data.NTYPE_TRANSAC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("DFECHA_MOVIMIENTO", OracleDbType.Date, _data.DFECHA_MOVIMIENTO, ParameterDirection.Input));
                //OUTPU
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);
                parameter.Add(C_TABLE);
                parameter.Add(P_COD_ERR);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                response.validation = Convert.ToString(P_COD_ERR.Value.ToString());

                if (response.validation == "0")
                {
                    while (odr.Read())
                    {
                        InruredList item = new InruredList();
                        item.nombres = odr["INSURED_NAME"].ToString();
                        item.nroDocumento = odr["SIDDOC"].ToString();
                        item.fechaNacimiento = odr["DBIRTHDAT"].ToString();
                        item.sclient = odr["SCLIENT"].ToString();
                        item.tipoDocumento = Convert.ToInt32(odr["NIDDOC_TYPE"].ToString());
                        item.nidProc = odr["NID_PROC"].ToString();
                        response.inrureds.Add(item);
                    }
                }

            }
            catch (Exception ex)
            {
                LogControl.save("GetInsuredForTransactionPolicy", ex.ToString(), "3");
                response = new InsuredPolicyResponse();
            }

            return response;
        }


        public ResponseVM UpdateInsuredPolicy(UpdateInsuredPolicy request)
        {
            ResponseVM response = new ResponseVM();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.pkg_Poliza + ".UPD_INSURED_POLICY";

            DbConnection connection = ConnectionGet(enuTypeDataBase.OracleVTime);
            connection.Open();
            DbTransaction trx = connection.BeginTransaction();

            try
            {
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.nidProc, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, request.sclient, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCLIENT_NEW", OracleDbType.Varchar2, request.updateSclient, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, request.nroDoc, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDDOC_TYPE", OracleDbType.Int32, Convert.ToInt32(request.typeDoc), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Double, request.npolicy, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDHEADERPROC", OracleDbType.Int64, request.nidheaderproc, ParameterDirection.Input));

                OracleParameter P_COD_ERR = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                P_COD_ERR.Size = 9000;
                P_MESSAGE.Size = 9000;

                parameter.Add(P_COD_ERR);
                parameter.Add(P_MESSAGE);

                this.ExecuteByStoredProcedureVT_TRX(storeprocedure, parameter, connection, trx);

                response.P_NCODE = Convert.ToString(P_COD_ERR.Value.ToString());
                response.P_SMESSAGE = Convert.ToString(P_MESSAGE.Value.ToString());
            }
            catch (Exception ex)
            {
                response.P_NCODE = "1";
                response.P_SMESSAGE = ex.ToString();
                LogControl.save("UPD_INSURED_POLICY", ex.ToString(), "3");
            }
            finally
            {
                if (response.P_NCODE == "0")
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




        public List<MovementsModelVM> GetPolicyMovementsVCF(MovementsModelVM data)
        {

            var ruta = ELog.obtainConfig("pathMovementsVCF") + data.NPOLICY;
            string movement = "";
            var rutaGen = "";

            Entities.PolicyModel.ViewModel.GenericPackageVM package = new Entities.PolicyModel.ViewModel.GenericPackageVM(); ;
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<MovementsModelVM> lista = new List<MovementsModelVM>();
            string storedProcedureName = ProcedureName.pkg_MovementsVcfPD + ".SP_GET_LIST_MOVEMENTS_CREDITS";

            try
            {

                //INPUT
                parameter.Add(new OracleParameter("P_NIDUSER", OracleDbType.Int32, data.NIDUSER, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int32, data.NPOLICY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, data.NPRODUCT, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);


                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                while (odr.Read())
                {
                    MovementsModelVM item = new MovementsModelVM();
                    item.NIDHEADERPROC = Int32.Parse(odr["NIDHEADERPROC"].ToString());
                    item.DES_NTYPE_HIST = odr["SDESC_MOVEMENT"] == DBNull.Value ? "" : odr["SDESC_MOVEMENT"].ToString();
                    item.NUM_MOVIMIENTO = odr["NMOVEMENT"] == DBNull.Value ? 0 : Int32.Parse(odr["NMOVEMENT"].ToString());
                    item.DINI_VIGENCIA = DateTime.Parse(odr["DINI_VIGENCIA"].ToString()).ToShortDateString();
                    item.DFIN_VIGENCIA = DateTime.Parse(odr["DFIN_VIGENCIA"].ToString()).ToShortDateString();
                    item.DFECHA_MOVIMIENTO = DateTime.Parse(odr["DCREATE"].ToString()).ToShortDateString();
                    item.SOBSERVACIONES = odr["SOBSERVACIONES"] == DBNull.Value ? "" : odr["SOBSERVACIONES"].ToString();
                    item.SUSUARIO = odr["SUSER"].ToString();
                    item.NTYPEMOVECREDIT = odr["NTYPEMOVECREDIT"] == DBNull.Value ? -1 : Int32.Parse(odr["NTYPEMOVECREDIT"].ToString());
                    item.SSTATUS_POL = odr["SSTATUS_POL"].ToString();
                    if (item.NTYPEMOVECREDIT == 0)
                    {
                        movement = "aprobacion";

                    }
                    else if (item.NTYPEMOVECREDIT == 1)
                    {
                        movement = "rechazo";

                    }

                    rutaGen = ELog.obtainConfig("pathMovementsVCF") + data.NPOLICY + "\\" + PrintGenerateUtil.RemoveDiacritics(movement) + "\\" + Convert.ToDateTime(odr["DCREATE"].ToString()).ToString("yyyyMMddHHmmss") + "\\"; // mejora ruta 13062023

                    if (item.NTYPEMOVECREDIT == 0 || item.NTYPEMOVECREDIT == 1)
                    {

                        try
                        {
                            var exist_file = Directory.GetFileSystemEntries(string.Format(ELog.obtainConfig("pathImpresion71") + data.NPOLICY + "\\" + item.NIDHEADERPROC + "\\"));

                            if (exist_file.Length > 0)
                            {
                                item.SRUTA_CONSTANCIA_ENDOSO = ELog.obtainConfig("pathImpresion71") + data.NPOLICY + "\\" + item.NIDHEADERPROC + "\\";
                            }

                        }
                        catch
                        {
                            item.SRUTA_CONSTANCIA_ENDOSO = null;

                        }



                    }

                    else
                    {

                        item.SRUTA_CONSTANCIA_ENDOSO = null;
                    }


                    item.SRUTA = odr["SRUTA"].ToString() == "" ? null : rutaGen;
                    item.SRUTAS_GEN = item.SRUTA == null ? null : sharedMethods.GetFilePathList(rutaGen);
                    lista.Add(item);
                }
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("GetPolicyMovementsVCF", ex.ToString(), "3");
                lista.DefaultIfEmpty();

            }
            return lista;
        }

        public SalidadPolizaEmit SavePolicyMovementsVCF(MovementsModelVM data, List<HttpPostedFile> files)
        {
            var ruta = ELog.obtainConfig("pathMovementsVCF");
            string movement = "";
            //data.DCREATE = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            data.DCREATE = data.DCREATE.Replace(",", " ");

            if (data.NTYPEMOVECREDIT == 0)
            {
                movement = "aprobacion";

            }
            else if (data.NTYPEMOVECREDIT == 1)
            {
                movement = "rechazo";

            }

            var ruta_final = files.Count > 0 ? ruta + data.NPOLICY + "\\" + PrintGenerateUtil.RemoveDiacritics(movement) + "\\" + Convert.ToDateTime(data.DCREATE).ToString("yyyyMMddHHmmss") + "\\" : null;
            var url = ruta.Remove(0, 3) + data.NPOLICY + "\\" + PrintGenerateUtil.RemoveDiacritics(movement) + "\\" + Convert.ToDateTime(data.DCREATE).ToString("yyyyMMddHHmmss");


            if (files.Count > 0)
            {
                guardarArchivos(ruta_final, files);
            }

            List<OracleParameter> parameter = new List<OracleParameter>();
            string storedProcedureName = ProcedureName.pkg_MovementsVcfPD + ".SP_INS_LIST_MOVEMENTS_CREDITS";

            SalidadPolizaEmit response = new SalidadPolizaEmit();

            try
            {

                //INPUT
                parameter.Add(new OracleParameter("P_NIDUSER", OracleDbType.Int32, data.NIDUSER, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SUSER", OracleDbType.Varchar2, data.SUSUARIO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDHEADERPROC", OracleDbType.Int32, data.NIDHEADERPROC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DCREATE", OracleDbType.Date, Convert.ToDateTime(data.DCREATE), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DINI_VIGENCIA", OracleDbType.Date, Convert.ToDateTime(data.DINI_VIGENCIA), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DFIN_VIGENCIA", OracleDbType.Date, Convert.ToDateTime(data.DFIN_VIGENCIA), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SNOMB_ARCHIVO", OracleDbType.Varchar2, data.SNOMB_ARCHIVO ?? "", ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SRUTA", OracleDbType.Varchar2, data.NPOLICY == 0 || files.Count == 0 ? null : url, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int32, data.NPOLICY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, data.NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SOBSERVACIONES", OracleDbType.Varchar2, data.SOBSERVACIONES, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.NID_COTIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SDESC_MOVEMENT", OracleDbType.Varchar2, data.DES_NTYPE_HIST, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPEMOVECREDIT", OracleDbType.Int32, data.NTYPEMOVECREDIT, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, null, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, null, ParameterDirection.Output);

                P_MESSAGE.Size = 9000;

                parameter.Add(P_MESSAGE);
                parameter.Add(P_COD_ERR);


                this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                response.P_MESSAGE = Convert.ToString(P_MESSAGE.Value.ToString());
                response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());

            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = ex.ToString();
                LogControl.save("SavePolicyMovementsVCF", ex.ToString(), "3");
            }
            return response;
        }
        public SalidadPolizaEmit SendMailMovementsVCF(MovementsModelVM data)
        {

            List<OracleParameter> parameter = new List<OracleParameter>();
            string storedProcedureName = ProcedureName.pkg_MovementsEndosoVcfPD + ".SP_INS_TBL_MOVEMENT_ENDOSO_REQUEST";

            SalidadPolizaEmit response = new SalidadPolizaEmit();

            try
            {

                //INPUT
                parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int32, data.NPOLICY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.NID_COTIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDHEADERPROC", OracleDbType.Int32, data.NIDHEADERPROC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMOVEMENT", OracleDbType.Int32, data.NUM_MOVIMIENTO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPEMOVECREDIT", OracleDbType.Int32, data.NTYPEMOVECREDIT, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, null, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, null, ParameterDirection.Output);

                P_MESSAGE.Size = 9000;

                parameter.Add(P_MESSAGE);
                parameter.Add(P_COD_ERR);


                this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                response.P_MESSAGE = Convert.ToString(P_MESSAGE.Value.ToString());
                response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());

            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = ex.ToString();
                LogControl.save("SavePolicyMovementsVCF", ex.ToString(), "3");
            }
            return response;
        }
        public Entities.PolicyModel.ViewModel.GenericPackageVM GetPolicyTransactionList(PolicyTransactionSearchBM data)
        {
            Entities.PolicyModel.ViewModel.GenericPackageVM package = new Entities.PolicyModel.ViewModel.GenericPackageVM();
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<PolicyTransactionVM> list = new List<PolicyTransactionVM>();
            string storedProcedureName = ProcedureName.pkg_ReportesSCTR + ".rea_reporte_transacciones";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Varchar2, data.PolicyNumber, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TIPO_PRODUCTO", OracleDbType.Varchar2, data.ProductType, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TIPO_MOVIMIENTO", OracleDbType.Varchar2, data.MovementType, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FECHA_DESDE", OracleDbType.Date, data.StartDate, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FECHA_HASTA", OracleDbType.Date, data.EndDate, ParameterDirection.Input));

                parameter.Add(new OracleParameter("P_MODO", OracleDbType.Varchar2, data.ContractorSearchMode, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TIPDOC", OracleDbType.Varchar2, data.ContractorDocumentType, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUMDOC", OracleDbType.Varchar2, data.ContractorDocumentNumber, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_APEPAT", OracleDbType.Varchar2, data.ContractorPaternalLastName, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_APEMAT", OracleDbType.Varchar2, data.ContractorMaternalLastName, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NAMES", OracleDbType.Varchar2, data.ContractorFirstName, ParameterDirection.Input));

                parameter.Add(new OracleParameter("P_NLIMITPERPAGE", OracleDbType.Int32, data.LimitPerpage, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPAGENUM", OracleDbType.Int32, data.PageNumber, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter NTOTALROWS = new OracleParameter("P_NTOTALROWS", OracleDbType.Int32, 4000, null, ParameterDirection.Output);

                parameter.Add(C_TABLE);
                parameter.Add(NTOTALROWS);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);
                while (odr.Read())
                {
                    try
                    {
                        PolicyTransactionVM item = new PolicyTransactionVM();
                        item.PolicyNumber = odr["NRO_POLIZA"].ToString();
                        item.Product = odr["NOMBRE_PRODUCTO"].ToString();
                        item.Movement = odr["TIPO_MOVIMIENTO"].ToString();
                        item.OperationNumber = odr["NRO_OPERACION"].ToString();

                        item.ContractorDocumentType = odr["TIPO_DOCUMENTO_CONTRATANTE"].ToString();
                        item.ContractorDocumentNumber = odr["NRO_DOCUMENTO_CONTRATANTE"].ToString();
                        item.ContractorFullName = odr["NOMBRE_CONTRATANTE"].ToString();

                        item.BrokerDocumentType = odr["TIPO_DOCUMENTO_BROKER"].ToString();
                        item.BrokerDocumentNumber = odr["NRO_DOCUMENTO_BROKER"].ToString();
                        item.BrokerFullName = odr["NOMBRE_BROKER"].ToString();

                        item.ProformaNumber = odr["NRO_PROFORMA"].ToString();
                        item.NetPremium = odr["PRIMA_NETA"].ToString();

                        if (odr["FECHA_TRANSACCION"].ToString() != null && odr["FECHA_TRANSACCION"].ToString().Trim() != "") item.TransactionDate = Convert.ToDateTime(odr["FECHA_TRANSACCION"].ToString());
                        else item.TransactionDate = null;

                        list.Add(item);
                    }
                    catch (Exception ex)
                    {
                        ELog.CloseConnection(odr);
                        throw ex;
                    }
                }
                ELog.CloseConnection(odr);

                package.TotalRowNumber = Convert.ToInt32(NTOTALROWS.Value.ToString());
                package.GenericResponse = list;
            }
            catch (Exception ex)
            {
                LogControl.save("GetPolicyTransactionList", ex.ToString(), "3");
            }
            return package;
        }

        public Entities.PolicyModel.ViewModel.GenericPackageVM GetAccountTransactionList(AccountTransactionSearchBM data)
        {
            Entities.PolicyModel.ViewModel.GenericPackageVM package = new GenericPackageVM();
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<AccountTransactionVM> list = new List<AccountTransactionVM>();
            string storedProcedureName = ProcedureName.pkg_ClientePrima + ".REA_LIST_PREMIUM_MOV";
            try
            {
                parameter.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, data.ClientId, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_IDPRODUCT", OracleDbType.Varchar2, data.Product, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NSTATUS_PRE", OracleDbType.Varchar2, data.PaymentState, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FECHA_DESDE", OracleDbType.Date, data.StartDate, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FECHA_HASTA", OracleDbType.Date, data.EndDate, ParameterDirection.Input));

                parameter.Add(new OracleParameter("P_NLIMITPERPAGE", OracleDbType.Int32, data.LimitPerPage, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPAGENUM", OracleDbType.Int32, data.PageNumber, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter NTOTALROWS = new OracleParameter("P_NTOTALROWS", OracleDbType.Int32, 4000, null, ParameterDirection.Output);

                parameter.Add(C_TABLE);
                parameter.Add(NTOTALROWS);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);
                while (odr.Read())
                {
                    AccountTransactionVM item = new AccountTransactionVM();
                    item.Product = odr["NOMBRE_PRODUCTO"].ToString();
                    item.ContractNumber = odr["NRO_CONTRATO"].ToString();
                    item.ProformaCode = odr["NRO_PROFORMA"].ToString();
                    if (odr["FIN_PROFORMA"].ToString() != null && odr["FIN_PROFORMA"].ToString().Trim() != "") item.ExpirationDate = Convert.ToDateTime(odr["FIN_PROFORMA"].ToString());
                    else item.ExpirationDate = null;

                    item.PaymentState = odr["ESTADO_PAGO"].ToString();
                    item.LegalDocument = odr["DOC_LEGAL"].ToString();
                    if (odr["FECHA_PAGO"].ToString() != null && odr["FECHA_PAGO"].ToString().Trim() != "") item.PaymentDate = Convert.ToDateTime(odr["FECHA_PAGO"].ToString());
                    else item.PaymentDate = null;

                    Double amount = 0.00;
                    Double.TryParse(odr["PRIMA"].ToString(), out amount);
                    item.Amount = amount;

                    if (odr["INICIO_VIGENCIA"].ToString() != null && odr["INICIO_VIGENCIA"].ToString().Trim() != "") item.VigencyStartDate = Convert.ToDateTime(odr["INICIO_VIGENCIA"].ToString());
                    else item.VigencyStartDate = null;
                    if (odr["FIN_VIGENCIA"].ToString() != null && odr["FIN_VIGENCIA"].ToString().Trim() != "") item.VigencyEndDate = Convert.ToDateTime(odr["FIN_VIGENCIA"].ToString());
                    else item.VigencyEndDate = null;

                    list.Add(item);
                }
                ELog.CloseConnection(odr);

                package.GenericResponse = list;
                package.TotalRowNumber = Int32.Parse(NTOTALROWS.Value.ToString());
            }
            catch (Exception ex)
            {
                LogControl.save("GetAccountTransactionList", ex.ToString(), "3");
            }
            return package;
        }

        public List<TransaccionTypeVM> GetTransaccionList()
        {
            List<TransaccionTypeVM> contactTypeList = new List<TransaccionTypeVM>();
            List<OracleParameter> parameter = new List<OracleParameter>();

            try
            {
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, contactTypeList, ParameterDirection.Output);

                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(ProcedureName.pkg_Poliza + ".REA_TIP_TRANSACCION", parameter);
                while (odr.Read())
                {
                    TransaccionTypeVM item = new TransaccionTypeVM();
                    item.COD_TIPO_TRANSACCION = odr["COD_TIPO_TRANSACCION"].ToString();
                    item.DES_TIPO_TRANSACCION = odr["DES_TIPO_TRANSACCION"].ToString();

                    contactTypeList.Add(item);
                }

                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                LogControl.save("GetTransaccionList", ex.ToString(), "3");
            }

            return contactTypeList;
        }

        public List<TransactionAllTypeVM> GetTransactionAllList()
        {
            List<TransactionAllTypeVM> contactTypeList = new List<TransactionAllTypeVM>();
            List<OracleParameter> parameter = new List<OracleParameter>();

            try
            {
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, contactTypeList, ParameterDirection.Output);

                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(ProcedureName.pkg_ReportesPD + ".REA_TRANSACCION", parameter);
                while (odr.Read())
                {
                    TransactionAllTypeVM item = new TransactionAllTypeVM();
                    item.NCOD_TRANSAC = odr["NCOD_TRANSAC"] == DBNull.Value ? 0 : double.Parse(odr["NCOD_TRANSAC"].ToString());
                    item.SDES_TRANSAC = odr["SDES_TRANSAC"] == DBNull.Value ? "" : odr["SDES_TRANSAC"].ToString();

                    contactTypeList.Add(item);
                }

                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                LogControl.save("GetTransactionAllList", ex.ToString(), "3");
            }

            return contactTypeList;
        }

        public GenericTransVM GetPolicyTransList(TransPolicySearchBM data)
        {
            GenericTransVM package = new GenericTransVM();
            package.C_TABLE = new List<PolicyTransVM>();
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<PolicyTransVM> lista = new List<PolicyTransVM>();
            string storedProcedureName = ProcedureName.pkg_Poliza + ".REA_POLIZA";
            try
            {
                ////INPUT
                parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Varchar2, data.P_NPOLICY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Varchar2, data.P_NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Varchar2, data.P_NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FECHA_DESDE", OracleDbType.Date, Convert.ToDateTime(data.P_FECHA_DESDE), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FECHA_HASTA", OracleDbType.Date, Convert.ToDateTime(data.P_FECHA_HASTA), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Varchar2, data.P_NTYPE_TRANSAC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TIPO_DOC_CONT", OracleDbType.Varchar2, data.P_TIPO_DOC_CONT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUM_DOC_CONT", OracleDbType.Varchar2, data.P_NUM_DOC_CONT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_RAZON_SOCIAL_CONT", OracleDbType.Varchar2, data.P_RAZON_SOCIAL_CONT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_APE_PAT_CONT", OracleDbType.Varchar2, data.P_APE_PAT_CONT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_APE_MAT_CONT", OracleDbType.Varchar2, data.P_APE_MAT_CONT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NOMBRES_CONT", OracleDbType.Varchar2, data.P_NOMBRES_CONT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, data.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLIMITPERPAGE", OracleDbType.Varchar2, 9999, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPAGENUM", OracleDbType.Varchar2, 1, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCOMPANY_LNK", OracleDbType.Varchar2, data.CompanyLNK, ParameterDirection.Input)); // JDD

                ////OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, package.C_TABLE, ParameterDirection.Output);
                OracleParameter NTOTALROWS = new OracleParameter("P_NTOTALROWS", OracleDbType.Int64, package.P_NTOTALROWS, ParameterDirection.Output);

                parameter.Add(C_TABLE);
                parameter.Add(NTOTALROWS);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);
                while (odr.Read())
                {
                    PolicyTransVM item = new PolicyTransVM();
                    item.NOMBRE_PRODUCT = odr["NOMBRE_PRODUCT"].ToString();
                    item.NRO_COTIZACION = odr["NRO_COTIZACION"].ToString();
                    item.POLIZA = odr["POLIZA"].ToString();
                    item.DOCUMENTO = odr["DOCUMENTO"].ToString();
                    item.NOMBRE_CONTRATANTE = odr["NOMBRE_CONTRATANTE"].ToString();
                    item.SEDE = odr["SEDE"].ToString();
                    item.FECHA_EMISION = String.Format("{0:dd/MM/yyyy}", odr["FECHA_EMISION"]);
                    item.INICIO_VIGENCIA = String.Format("{0:dd/MM/yyyy}", odr["INICIO_VIGENCIA"]);
                    item.FIN_VIGENCIA = String.Format("{0:dd/MM/yyyy}", odr["FIN_VIGENCIA"]);
                    item.NRO_RENOVACIONES = odr["NRO_RENOVACIONES"].ToString();
                    item.SCLIENT_SEG = odr["SCLIENT_SEG"] == DBNull.Value ? string.Empty : odr["SCLIENT_SEG"].ToString();
                    item.SDESCRIPT_SEG = odr["SDESCRIPT_SEG"] == DBNull.Value ? string.Empty : odr["SDESCRIPT_SEG"].ToString();

                    lista.Add(item);
                }

                ELog.CloseConnection(odr);
                package.P_NTOTALROWS = Convert.ToInt64(NTOTALROWS.Value.ToString());
                package.C_TABLE = lista;
            }
            catch (Exception ex)
            {
                LogControl.save("GetPolicyTransList", ex.ToString(), "3");
            }
            return package;
        }

        public GenericTransVM GetPolicyMovementsTransList(TransPolicySearchBM data)
        {
            GenericTransVM package = new GenericTransVM();
            package.C_TABLE = new List<PolicyMovVM>();
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<PolicyMovVM> lista = new List<PolicyMovVM>();
            string storedProcedureName = ProcedureName.pkg_Poliza + ".REA_LIST_POLIZA_TRANSACCION";

            try
            {
                // INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int64, Convert.ToInt64(data.P_NID_COTIZACION), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLIMITPERPAGE", OracleDbType.Int64, 9999, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPAGENUM", OracleDbType.Int64, 1, ParameterDirection.Input));

                // OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, package.C_TABLE, ParameterDirection.Output);
                OracleParameter NTOTALROWS = new OracleParameter("P_NTOTALROWS", OracleDbType.Int64, package.P_NTOTALROWS, ParameterDirection.Output);

                parameter.Add(C_TABLE);
                parameter.Add(NTOTALROWS);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);
                while (odr.Read())
                {
                    PolicyMovVM item = new PolicyMovVM();
                    item.NRO = odr["NRO"] == DBNull.Value ? string.Empty : odr["NRO"].ToString();
                    item.TIPO_TRANSACCION = odr["TIPO_TRANSACCION"] == DBNull.Value ? string.Empty : odr["TIPO_TRANSACCION"].ToString();
                    item.INICIO_COBERTURA = String.Format("{0:dd/MM/yyyy}", odr["INICIO_COBERTURA"]);
                    item.FIN_COBERTURA = String.Format("{0:dd/MM/yyyy}", odr["FIN_COBERTURA"]);
                    item.FECHA_TRANSACCION = String.Format("{0:dd/MM/yyyy}", odr["FECHA_TRANSACCION"]);
                    item.USUARIO = odr["USUARIO"] == DBNull.Value ? string.Empty : odr["USUARIO"].ToString();
                    item.FACTURAR = odr["FACTURAR"] == DBNull.Value ? string.Empty : odr["FACTURAR"].ToString();
                    item.CONSTANCIA = odr["CONSTANCIA"] == DBNull.Value ? string.Empty : odr["CONSTANCIA"].ToString();
                    item.RUTA = odr["SRUTA"] == DBNull.Value ? string.Empty : odr["SRUTA"].ToString();
                    item.MOV_ANULADO = odr["MOV_ANULADO"] == DBNull.Value ? string.Empty : odr["MOV_ANULADO"].ToString();
                    item.RECIBO = odr["RECIBO"] == DBNull.Value ? string.Empty : odr["RECIBO"].ToString();
                    item.COD_TRANSAC = odr["COD_TRANSAC"] == DBNull.Value ? string.Empty : odr["COD_TRANSAC"].ToString();
                    item.MOT_ANULACION = odr["MOT_ANULACION"] == DBNull.Value ? string.Empty : odr["MOT_ANULACION"].ToString();
                    item.COMENTARIO = odr["COMENTARIO"] == DBNull.Value ? string.Empty : odr["COMENTARIO"].ToString();
                    item.NPOLICY = odr["NPOLICY"] == DBNull.Value ? 0 : long.Parse(odr["NPOLICY"].ToString());
                    item.NIDHEADERPROC = odr["NIDHEADERPROC"] == DBNull.Value ? 0 : int.Parse(odr["NIDHEADERPROC"].ToString());
                    item.NPRODUCT = odr["NPRODUCT"] == DBNull.Value ? 0 : int.Parse(odr["NPRODUCT"].ToString());
                    item.NTYPE_HIST = odr["NTYPE_HIST"] == DBNull.Value ? 0 : int.Parse(odr["NTYPE_HIST"].ToString());
                    item.DES_NTYPE_HIST_PDF = odr["DES_NTYPE_HIST_PDF"] == DBNull.Value ? "" : odr["DES_NTYPE_HIST_PDF"].ToString();
                    item.NCOT_MIXTA = odr["NCOT_MIXTA"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NCOT_MIXTA"]);
                    item.NPOLICY_PENSION = odr["NPOLICY_PENSION"] == DBNull.Value ? 0 : long.Parse(odr["NPOLICY_PENSION"].ToString());
                    item.NPOLICY_SALUD = odr["NPOLICY_SALUD"] == DBNull.Value ? 0 : long.Parse(odr["NPOLICY_SALUD"].ToString());
                    item.NPOLICY_SALUD = odr["NPOLICY_SALUD"] == DBNull.Value ? 0 : long.Parse(odr["NPOLICY_SALUD"].ToString());
                    item.NID_PROC = odr["NID_PROC"] == DBNull.Value ? string.Empty : odr["NID_PROC"].ToString();
                    item.NCOMPANY_LNK = odr["NCOMPANY_LNK"] == DBNull.Value ? 0 : int.Parse(odr["NCOMPANY_LNK"].ToString());
                    item.RUTAS = sharedMethods.GetFilePathList(item.RUTA);
                    item.NBRANCH = odr["RAMO"] == DBNull.Value ? 0 : int.Parse(odr["RAMO"].ToString());
                    item.NIDHEADERPROC_SCTR = odr["NIDHEADERPROC_SCTR"] == DBNull.Value ? 0 : int.Parse(odr["NIDHEADERPROC_SCTR"].ToString());

                    String RouteObt = null;
                    String ValRoute = null;
                    String RouteObt_Pension = null;
                    String RouteObt_Salud = null;
                    String ValRoute_Pension = null;
                    String ValRoute_Salud = null;
                    bool isDir = false;
                    bool isDir_Pension = false;
                    bool isDir_Salud = false;
                    var myRegex = new Regex(@"");

                    if (item.NIDHEADERPROC > 0)
                    {
                        item.INICIO_COBERTURA = String.Format("{0:dd/MM/yyyy}", odr["INICIO_COBERTURA"]);
                        if (item.COD_TRANSAC == "1")
                        {
                            if (item.NCOT_MIXTA != 1)
                            {
                                RouteObt = ELog.obtainConfig("pathCuadroPolicySctr") + item.NPOLICY + "\\" + item.NIDHEADERPROC + "\\" + PrintGenerateUtil.RemoveDiacritics(item.DES_NTYPE_HIST_PDF).ToString();
                                var path = new FileInfo(RouteObt);
                                ValRoute = path.ToString();
                                isDir = Directory.Exists(ValRoute);
                                if (isDir == true)
                                {
                                    item.RUTAS_GEN = Directory.GetFileSystemEntries(ELog.obtainConfig("pathCuadroPolicySctr") + item.NPOLICY + "\\" + item.NIDHEADERPROC + "\\" + PrintGenerateUtil.RemoveDiacritics(item.DES_NTYPE_HIST_PDF) + "\\", "*.*");
                                    item.RUTAS_GEN = item.RUTAS_GEN.Where(f => myRegex.IsMatch(f)).ToArray();
                                }
                            }
                            else
                            {
                                RouteObt_Pension = ELog.obtainConfig("pathCuadroPolicySctr") + item.NPOLICY_PENSION + "\\" + item.NIDHEADERPROC + "\\" + PrintGenerateUtil.RemoveDiacritics(item.DES_NTYPE_HIST_PDF).ToString();
                                RouteObt_Salud = ELog.obtainConfig("pathCuadroPolicySctr") + item.NPOLICY_SALUD + "\\" + item.NIDHEADERPROC + "\\" + PrintGenerateUtil.RemoveDiacritics(item.DES_NTYPE_HIST_PDF).ToString();
                                var path_pension = new FileInfo(RouteObt_Pension);
                                var path_salud = new FileInfo(RouteObt_Salud);
                                ValRoute_Pension = path_pension.ToString();
                                ValRoute_Salud = path_salud.ToString();
                                isDir_Pension = Directory.Exists(ValRoute_Pension);
                                isDir_Salud = Directory.Exists(ValRoute_Salud);
                                if (isDir_Pension == true)
                                {
                                    var rutas_pension = Directory.GetFileSystemEntries(ELog.obtainConfig("pathCuadroPolicySctr") + item.NPOLICY_PENSION + "\\" + item.NIDHEADERPROC + "\\" + PrintGenerateUtil.RemoveDiacritics(item.DES_NTYPE_HIST_PDF) + "\\", "*.*").ToArray();
                                    rutas_pension = rutas_pension.Where(f => myRegex.IsMatch(f)).ToArray();
                                    if (item.RUTAS_GEN != null)
                                    {
                                        item.RUTAS_GEN = item.RUTAS_GEN.Concat(rutas_pension).ToArray();
                                    }
                                    else
                                    {
                                        item.RUTAS_GEN = rutas_pension;
                                    }
                                }

                                if (isDir_Salud == true)
                                {
                                    var rutas_salud = Directory.GetFileSystemEntries(ELog.obtainConfig("pathCuadroPolicySctr") + item.NPOLICY_SALUD + "\\" + item.NIDHEADERPROC + "\\" + PrintGenerateUtil.RemoveDiacritics(item.DES_NTYPE_HIST_PDF) + "\\", "*.*").ToArray();
                                    rutas_salud = rutas_salud.Where(f => myRegex.IsMatch(f)).ToArray();
                                    if (item.RUTAS_GEN != null)
                                    {
                                        item.RUTAS_GEN = item.RUTAS_GEN.Concat(rutas_salud).ToArray();
                                    }
                                    else
                                    {
                                        item.RUTAS_GEN = rutas_salud;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (item.NCOT_MIXTA != 1)
                            {
                                RouteObt = ELog.obtainConfig("pathCuadroPolicySctr") + item.NPOLICY + "\\" + item.NIDHEADERPROC + "\\" + PrintGenerateUtil.RemoveDiacritics(item.DES_NTYPE_HIST_PDF) + "\\" + item.INICIO_COBERTURA.Replace("/", "").ToString();
                                var path = new FileInfo(RouteObt);
                                ValRoute = path.ToString();
                                isDir = Directory.Exists(ValRoute);
                                if (isDir == true)
                                {
                                    item.RUTAS_GEN = Directory.GetFileSystemEntries(ELog.obtainConfig("pathCuadroPolicySctr") + item.NPOLICY + "\\" + item.NIDHEADERPROC + "\\" + PrintGenerateUtil.RemoveDiacritics(item.DES_NTYPE_HIST_PDF) + "\\" + item.INICIO_COBERTURA.Replace("/", "") + "\\", "*.*");
                                    item.RUTAS_GEN = item.RUTAS_GEN.Where(f => myRegex.IsMatch(f)).ToArray();
                                }
                            }
                            else
                            {
                                RouteObt_Pension = ELog.obtainConfig("pathCuadroPolicySctr") + item.NPOLICY_PENSION + "\\" + item.NIDHEADERPROC + "\\" + PrintGenerateUtil.RemoveDiacritics(item.DES_NTYPE_HIST_PDF) + "\\" + item.INICIO_COBERTURA.Replace("/", "").ToString();
                                RouteObt_Salud = ELog.obtainConfig("pathCuadroPolicySctr") + item.NPOLICY_SALUD + "\\" + item.NIDHEADERPROC + "\\" + PrintGenerateUtil.RemoveDiacritics(item.DES_NTYPE_HIST_PDF) + "\\" + item.INICIO_COBERTURA.Replace("/", "").ToString();
                                var path_pension = new FileInfo(RouteObt_Pension);
                                var path_salud = new FileInfo(RouteObt_Salud);
                                ValRoute_Pension = path_pension.ToString();
                                ValRoute_Salud = path_salud.ToString();
                                isDir_Pension = Directory.Exists(ValRoute_Pension);
                                isDir_Salud = Directory.Exists(ValRoute_Salud);
                                if (isDir_Pension == true)
                                {
                                    var rutas_pension = Directory.GetFileSystemEntries(ELog.obtainConfig("pathCuadroPolicySctr") + item.NPOLICY_PENSION + "\\" + item.NIDHEADERPROC + "\\" + PrintGenerateUtil.RemoveDiacritics(item.DES_NTYPE_HIST_PDF) + "\\" + item.INICIO_COBERTURA.Replace("/", "") + "\\", "*.*").ToArray();
                                    rutas_pension = rutas_pension.Where(f => myRegex.IsMatch(f)).ToArray();
                                    if (item.RUTAS_GEN != null)
                                    {
                                        item.RUTAS_GEN = item.RUTAS_GEN.Concat(rutas_pension).ToArray();
                                    }
                                    else
                                    {
                                        item.RUTAS_GEN = rutas_pension;
                                    }
                                }

                                if (isDir_Salud == true)
                                {
                                    var rutas_salud = Directory.GetFileSystemEntries(ELog.obtainConfig("pathCuadroPolicySctr") + item.NPOLICY_SALUD + "\\" + item.NIDHEADERPROC + "\\" + PrintGenerateUtil.RemoveDiacritics(item.DES_NTYPE_HIST_PDF) + "\\" + item.INICIO_COBERTURA.Replace("/", "") + "\\", "*.*").ToArray();
                                    rutas_salud = rutas_salud.Where(f => myRegex.IsMatch(f)).ToArray();
                                    if (item.RUTAS_GEN != null)
                                    {
                                        item.RUTAS_GEN = item.RUTAS_GEN.Concat(rutas_salud).ToArray();
                                    }
                                    else
                                    {
                                        item.RUTAS_GEN = rutas_salud;
                                    }
                                }
                            }

                        }
                    }
                    else
                    {

                        if (item.NBRANCH == 77)
                        {
                            if (item.NCOT_MIXTA != 1) //solo pension
                            {
                                RouteObt = String.Format(ELog.obtainConfig("pathCuadroPolicySctr") + item.NPOLICY + "\\" + item.NIDHEADERPROC_SCTR + "\\" + PrintGenerateUtil.RemoveDiacritics(item.DES_NTYPE_HIST_PDF) + "\\" + item.INICIO_COBERTURA.Replace("/", "") + "\\", "*.*").ToString();
                                var path = new FileInfo(RouteObt);
                                ValRoute = path.ToString();
                                isDir = Directory.Exists(ValRoute);
                                if (isDir == true)
                                {
                                    item.RUTAS_GEN = Directory.GetFileSystemEntries(ELog.obtainConfig("pathCuadroPolicySctr") + item.NPOLICY + "\\" + item.NIDHEADERPROC_SCTR + "\\" + PrintGenerateUtil.RemoveDiacritics(item.DES_NTYPE_HIST_PDF) + "\\" + item.INICIO_COBERTURA.Replace("/", "") + "\\", "*.*");
                                    item.RUTAS_GEN = item.RUTAS_GEN.Where(f => myRegex.IsMatch(f)).ToArray();
                                }
                            }
                            else //ambos productos
                            {
                                RouteObt_Pension = ELog.obtainConfig("pathCuadroPolicySctr") + item.NPOLICY_PENSION + "\\" + item.NIDHEADERPROC_SCTR + "\\" + PrintGenerateUtil.RemoveDiacritics(item.DES_NTYPE_HIST_PDF) + "\\" + item.INICIO_COBERTURA.Replace("/", "").ToString();
                                RouteObt_Salud = ELog.obtainConfig("pathCuadroPolicySctr") + item.NPOLICY_SALUD + "\\" + item.NIDHEADERPROC_SCTR + "\\" + PrintGenerateUtil.RemoveDiacritics(item.DES_NTYPE_HIST_PDF) + "\\" + item.INICIO_COBERTURA.Replace("/", "").ToString();
                                var path_pension = new FileInfo(RouteObt_Pension);
                                var path_salud = new FileInfo(RouteObt_Salud);
                                ValRoute_Pension = path_pension.ToString();
                                ValRoute_Salud = path_salud.ToString();
                                isDir_Pension = Directory.Exists(ValRoute_Pension);
                                isDir_Salud = Directory.Exists(ValRoute_Salud);
                                if (isDir_Pension == true)
                                {
                                    var rutas_pension = Directory.GetFileSystemEntries(ELog.obtainConfig("pathCuadroPolicySctr") + item.NPOLICY_PENSION + "\\" + item.NIDHEADERPROC_SCTR + "\\" + PrintGenerateUtil.RemoveDiacritics(item.DES_NTYPE_HIST_PDF) + "\\" + item.INICIO_COBERTURA.Replace("/", "") + "\\", "*.*").ToArray();
                                    rutas_pension = rutas_pension.Where(f => myRegex.IsMatch(f)).ToArray();
                                    if (item.RUTAS_GEN != null)
                                    {
                                        item.RUTAS_GEN = item.RUTAS_GEN.Concat(rutas_pension).ToArray();
                                    }
                                    else
                                    {
                                        item.RUTAS_GEN = rutas_pension;
                                    }
                                }

                                if (isDir_Salud == true)
                                {
                                    var rutas_salud = Directory.GetFileSystemEntries(ELog.obtainConfig("pathCuadroPolicySctr") + item.NPOLICY_SALUD + "\\" + item.NIDHEADERPROC_SCTR + "\\" + PrintGenerateUtil.RemoveDiacritics(item.DES_NTYPE_HIST_PDF) + "\\" + item.INICIO_COBERTURA.Replace("/", "") + "\\", "*.*").ToArray();
                                    rutas_salud = rutas_salud.Where(f => myRegex.IsMatch(f)).ToArray();
                                    if (item.RUTAS_GEN != null)
                                    {
                                        item.RUTAS_GEN = item.RUTAS_GEN.Concat(rutas_salud).ToArray();
                                    }
                                    else
                                    {
                                        item.RUTAS_GEN = rutas_salud;
                                    }
                                }
                            }
                        }
                        else
                        {
                            RouteObt = String.Format(ELog.obtainConfig("pathPrincipal"), ELog.obtainConfig("pathPoliza")) + data.P_NID_COTIZACION + "\\" + ELog.obtainConfig("movimiento") + "\\" + ELog.obtainConfig("movimiento" + item.COD_TRANSAC) + "\\" + item.NRO.ToString();
                            var path = new FileInfo(RouteObt);
                            ValRoute = path.ToString();
                            isDir = Directory.Exists(ValRoute);
                            if (isDir == true)
                            {
                                item.RUTAS_GEN = Directory.GetFileSystemEntries(String.Format(ELog.obtainConfig("pathPrincipal"), ELog.obtainConfig("pathPoliza")) + data.P_NID_COTIZACION + "\\" + ELog.obtainConfig("movimiento") + "\\" + ELog.obtainConfig("movimiento" + item.COD_TRANSAC) + "\\" + item.NRO + "\\", "*.*");
                                item.RUTAS_GEN = item.RUTAS_GEN.Where(f => myRegex.IsMatch(f)).ToArray();
                            }
                        }

                    }

                    item.NMOVEMENT_PH = odr["NMOVEMENT_PH"] == DBNull.Value ? 0 : int.Parse(odr["NMOVEMENT_PH"].ToString());

                    lista.Add(item);

                }
                ELog.CloseConnection(odr);

                package.P_NTOTALROWS = Convert.ToInt64(NTOTALROWS.Value.ToString());
                package.C_TABLE = lista;
            }
            catch (Exception ex)
            {
                LogControl.save("GetPolicyMovementsTransList", ex.ToString(), "3");
            }
            return package;
        }


        public GenericPackageVM GetPaymentStateList()
        {
            GenericPackageVM package = new GenericPackageVM();
            List<OracleParameter> parameters = new List<OracleParameter>();
            List<PaymentStateVM> list = new List<PaymentStateVM>();
            string storedProcedureName = ProcedureName.pkg_ClientePrima + ".REA_LIST_STATUS_PRE";
            try
            {
                OracleParameter table = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameters.Add(table);
                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameters);

                while (odr.Read())
                {
                    PaymentStateVM item = new PaymentStateVM();
                    item.Id = odr["NSTATUS_PRE"].ToString();
                    item.Name = odr["SDESCRIPT"].ToString();

                    list.Add(item);
                }
                ELog.CloseConnection(odr);

                package.GenericResponse = list;
            }
            catch (Exception ex)
            {
                LogControl.save("GetPaymentStateList", ex.ToString(), "3");
            }
            return package;
        }

        public List<TipoRenovacion> GetTipoRenovacion(TypeRenBM request)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<TipoRenovacion> lista = new List<TipoRenovacion>();
            string[] tipRenovacionExcProd = ELog.obtainConfig("renovacionExcProd" + request.P_NPRODUCT).Split(';');
            string[] tipRenovacionExcEps = ELog.obtainConfig("renovacionExcEps" + request.P_NEPS).Split(';');
            string storedProcedureName = ProcedureName.pkg_Poliza + ".REA_TIP_RENOVACION";

            try
            {
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, request.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, request.P_NPRODUCT, ParameterDirection.Input));
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, lista, ParameterDirection.Output);
                parameter.Add(C_TABLE);
                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                while (odr.Read())
                {
                    TipoRenovacion tipoRenovacion = new TipoRenovacion();
                    tipoRenovacion.COD_TIPO_RENOVACION = Convert.ToInt32(odr["COD_TIPO_RENOVACION"]);
                    tipoRenovacion.DES_TIPO_RENOVACION = odr["DES_TIPO_RENOVACION"].ToString();
                    lista.Add(tipoRenovacion);
                }
                ELog.CloseConnection(odr);

                if (request.P_ENABLED == 1) // INI POLIZAS ESPECIALES RI JTV 11102022
                {
                    lista = lista.Where((n) => !(tipRenovacionExcProd.Contains(n.COD_TIPO_RENOVACION.ToString()))).ToList();
                } // FIN POLIZAS ESPECIALES RI JTV 11102022

                lista = lista.Where((n) => !(tipRenovacionExcEps.Contains(n.COD_TIPO_RENOVACION.ToString()))).ToList();
            }
            catch (Exception ex)
            {
                LogControl.save("GetTipoRenovacion", ex.ToString(), "3");
            }

            return lista;
        }

        public List<FrecuenciaPago> GetFrecuenciaPago(int codrenovacion, int producto)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<FrecuenciaPago> lista = new List<FrecuenciaPago>();
            string storedProcedureName = ProcedureName.pkg_Poliza + ".REA_TIP_FRECUENCIA";
            try
            {
                parameter.Add(new OracleParameter("P_TIP_RENOV", OracleDbType.Int32, codrenovacion, ParameterDirection.Input));
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, lista, ParameterDirection.Output);
                parameter.Add(C_TABLE);
                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);
                while (odr.Read())
                {
                    FrecuenciaPago frecuenciaPago = new FrecuenciaPago();
                    frecuenciaPago.COD_TIPO_FRECUENCIA = Convert.ToInt32(odr["COD_TIPO_FRECUENCIA"]);
                    frecuenciaPago.DES_TIPO_FRECUENCIA = odr["DES_TIPO_FRECUENCIA"].ToString();
                    lista.Add(frecuenciaPago);
                }
                ELog.CloseConnection(odr);

                string[] frecuenciaExcProd = ELog.obtainConfig("freqRenovaction" + producto).Split(';');
                if (producto != 0)
                {
                    lista = lista.Where((n) => !(frecuenciaExcProd.Contains(n.COD_TIPO_FRECUENCIA.ToString()))).ToList();
                }
                else
                {
                    if (codrenovacion != 6 && codrenovacion != 7)
                    {
                        lista = lista.Where((n) => !(frecuenciaExcProd.Contains(n.COD_TIPO_FRECUENCIA.ToString()))).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetFrecuenciaPago", ex.ToString(), "3");
            }

            return lista;
        }

        public List<FrecuenciaPago> GetFrecuenciaPagoTotal(int codrenovacion, int producto)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<FrecuenciaPago> lista = new List<FrecuenciaPago>();
            string storedProcedureName = ProcedureName.pkg_Poliza + ".REA_TIP_FRECUENCIA_TOTAL";
            try
            {
                parameter.Add(new OracleParameter("P_TIP_RENOV", OracleDbType.Int32, codrenovacion, ParameterDirection.Input));
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, lista, ParameterDirection.Output);
                parameter.Add(C_TABLE);
                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);
                while (odr.Read())
                {
                    FrecuenciaPago frecuenciaPago = new FrecuenciaPago();
                    frecuenciaPago.COD_TIPO_FRECUENCIA = Convert.ToInt32(odr["COD_TIPO_FRECUENCIA"]);
                    frecuenciaPago.DES_TIPO_FRECUENCIA = odr["DES_TIPO_FRECUENCIA"].ToString();
                    lista.Add(frecuenciaPago);
                }
                ELog.CloseConnection(odr);

                string[] frecuenciaExcProd = ELog.obtainConfig("freqRenovaction" + producto).Split(';');
                if (producto != 0)
                {
                    lista = lista.Where((n) => !(frecuenciaExcProd.Contains(n.COD_TIPO_FRECUENCIA.ToString()))).ToList();
                }
                else
                {
                    if (codrenovacion != 6 && codrenovacion != 7)
                    {
                        lista = lista.Where((n) => !(frecuenciaExcProd.Contains(n.COD_TIPO_FRECUENCIA.ToString()))).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetFrecuenciaPagoTotal", ex.ToString(), "3");
            }

            return lista;
        }

        public List<AnnulmentVM> GetAnnulment()
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<AnnulmentVM> lista = new List<AnnulmentVM>();
            string storedProcedureName = ProcedureName.pkg_Poliza + ".REA_ANULACION";
            try
            {
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, lista, ParameterDirection.Output);
                parameter.Add(C_TABLE);
                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);
                while (odr.Read())
                {
                    AnnulmentVM motivoAnulacion = new AnnulmentVM();
                    motivoAnulacion.COD_ANULACION = Convert.ToInt32(odr["COD_ANULACION"]);
                    motivoAnulacion.DES_ANULACION = odr["DES_ANULACION"].ToString();
                    lista.Add(motivoAnulacion);
                }
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                LogControl.save("GetAnnulment", ex.ToString(), "3");
            }
            return lista;
        }

        public List<TypeEndoso> GetTypeEndoso()
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<TypeEndoso> lista = new List<TypeEndoso>();
            string storedProcedureName = ProcedureName.pkg_Poliza + ".REA_TIPO_ENDOSO";
            try
            {
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, lista, ParameterDirection.Output);
                parameter.Add(C_TABLE);
                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);
                while (odr.Read())
                {
                    TypeEndoso typeEndoso = new TypeEndoso();
                    typeEndoso.TYPE_ENDOSO = Convert.ToInt32(odr["TYPE_ENDOSO"]);
                    typeEndoso.DES_TYPE_ENDOSO = odr["DES_TYPE_ENDOSO"].ToString();
                    lista.Add(typeEndoso);
                }
                odr.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("GetTypeEndoso", ex.ToString(), "3");
            }

            return lista;
        }

        public SalidadPolizaEmit SavePolizaEmitFile(SavePolicyEmit request, List<HttpPostedFile> files)
        {
            SalidadPolizaEmit response = null;
            var rutaFinal = String.Format(ELog.obtainConfig("pathPrincipal"), ELog.obtainConfig("pathPoliza") + request.P_NID_COTIZACION + "\\" + ELog.obtainConfig("pathAdjuntos") + "\\" + ELog.obtainConfig("movimiento1") + "\\");

            try
            {
                request.SRUTA = rutaFinal;
                response = SavePolizaEmit(request);

                guardarArchivos(rutaFinal, files);
            }
            catch (Exception ex)
            {
                LogControl.save("SavePolizaEmitFile", ex.ToString(), "3");
            }

            return response;
        }

        private void guardarArchivos(string ruta, List<HttpPostedFile> files)
        {
            if (files != null)
            {

                if (!Directory.Exists(ruta))
                {
                    Directory.CreateDirectory(ruta);
                }

                foreach (var item in files)
                {
                    item.SaveAs(ruta + item.FileName);
                }
            }
        }

        public SalidaPlanilla SaveUsingOracleBulkCopy(DataTable dt, string nroCotizacion)
        {
            SalidaPlanilla resultPackage = new SalidaPlanilla();
            string destTableName = ProcedureName.tbl_CargaPD;
            try
            {
                this.SaveUsingOracleBulkCopy(destTableName, dt);
                resultPackage.P_COD_ERR = 0;
                resultPackage.P_MESSAGE = ELog.obtainConfig("bulkyTrama");
            }
            catch (Exception ex)
            {
                resultPackage.P_COD_ERR = 1;
                resultPackage.P_MESSAGE = ex.Message + " " + nroCotizacion;
                LogControl.save("SaveUsingOracleBulkCopy", ex.ToString(), "3");
            }

            return resultPackage;
        }

        public SalidadPolizaEmit SavePolizaEmit(SavePolicyEmit policyEmit)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            SalidadPolizaEmit resultPackage = new SalidadPolizaEmit();
            string storedProcedureName = ProcedureName.pkg_Poliza + ".INS_EMITIR_POLIZA";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, policyEmit.P_NID_COTIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, policyEmit.P_NID_PROC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, policyEmit.P_NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, policyEmit.P_NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOLTIMRE", OracleDbType.Char, policyEmit.P_SCOLTIMRE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DSTARTDATE", OracleDbType.Varchar2, policyEmit.P_DSTARTDATE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Varchar2, policyEmit.P_DEXPIRDAT, ParameterDirection.Input));
                //parameter.Add(new OracleParameter("P_DSTARTDATE", OracleDbType.Date, Convert.ToDateTime(policyEmit.P_DSTARTDATE), ParameterDirection.Input));
                //parameter.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, Convert.ToDateTime(policyEmit.P_DEXPIRDAT), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPAYFREQ", OracleDbType.Int32, policyEmit.P_NPAYFREQ, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FACT_MES_VENCIDO", OracleDbType.Int32, policyEmit.P_FACT_MES_VENCIDO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SFLAG_FAC_ANT", OracleDbType.Char, policyEmit.P_SFLAG_FAC_ANT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPREM_NETA", OracleDbType.Decimal, policyEmit.P_NPREM_NETA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SRUTA", OracleDbType.Char, policyEmit.SRUTA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_IGV", OracleDbType.Decimal, policyEmit.P_IGV, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPREM_BRU", OracleDbType.Decimal, policyEmit.P_NPREM_BRU, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, policyEmit.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOMMENT", OracleDbType.Varchar2, policyEmit.P_SCOMMENT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDPAYMENT", OracleDbType.Int32, policyEmit.P_NIDPAYMENT == null ? 0 : policyEmit.P_NIDPAYMENT, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_NPOLICY = new OracleParameter("P_NPOLICY", OracleDbType.Int64, 4000, resultPackage.P_NPOLICY, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, 4000, resultPackage.P_MESSAGE, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, resultPackage.P_COD_ERR, ParameterDirection.Output);
                OracleParameter P_NCONSTANCIA = new OracleParameter("P_NCONSTANCIA", OracleDbType.Int32, 4000, resultPackage.P_NCONSTANCIA, ParameterDirection.Output);

                parameter.Add(P_NPOLICY);
                parameter.Add(P_MESSAGE);
                parameter.Add(P_COD_ERR);
                parameter.Add(P_NCONSTANCIA);

                parameter.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Int32, 1, ParameterDirection.Input));                  //RI
                parameter.Add(new OracleParameter("P_NAMO_AFEC", OracleDbType.Decimal, policyEmit.P_NAMO_AFEC, ParameterDirection.Input));                  //RI
                parameter.Add(new OracleParameter("P_NIVA", OracleDbType.Decimal, policyEmit.P_NIVA, ParameterDirection.Input));                            //RI
                parameter.Add(new OracleParameter("P_NAMOUNT", OracleDbType.Decimal, policyEmit.P_NAMOUNT, ParameterDirection.Input));                      //RI
                parameter.Add(new OracleParameter("P_NDE", OracleDbType.Decimal, policyEmit.P_NDE, ParameterDirection.Input));                              //RI
                parameter.Add(new OracleParameter("P_DSTARTDATE_ASE", OracleDbType.Date, policyEmit.P_DSTARTDATE_ASE, ParameterDirection.Input));           //RI
                parameter.Add(new OracleParameter("P_DEXPIRDAT_ASE", OracleDbType.Date, policyEmit.P_DEXPIRDAT_ASE, ParameterDirection.Input));             //RI
                //parameter.Add(new OracleParameter("P_STIPO_APP ", OracleDbType.Varchar2, policyEmit.P_STIPO_APP, ParameterDirection.Input));           //RI
                parameter.Add(new OracleParameter("P_NCOT_MIXTA", OracleDbType.Int32, policyEmit.P_NCOT_MIXTA, ParameterDirection.Input)); // poliza mixta sctr
                parameter.Add(new OracleParameter("P_NCOT_NC", OracleDbType.Int32, policyEmit.P_NCOT_NC, ParameterDirection.Input)); //pry nc AVS 17/03/2023
                parameter.Add(new OracleParameter("P_AMBOS_PRODUCTOS_SCTR", OracleDbType.Int32, policyEmit.P_AMBOS_PRODUCTOS_SCTR, ParameterDirection.Input)); // ED 07/07/2023 

                this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                resultPackage.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                resultPackage.P_MESSAGE = P_MESSAGE.Value.ToString();
                resultPackage.P_NPOLICY = resultPackage.P_COD_ERR == 0 ? Convert.ToInt64(P_NPOLICY.Value.ToString()) : 0;
                resultPackage.P_NCONSTANCIA = resultPackage.P_COD_ERR == 0 ? Convert.ToInt32(P_NCONSTANCIA.Value.ToString()) : 0;
            }
            catch (Exception ex)
            {
                resultPackage.P_COD_ERR = 1;
                resultPackage.P_MESSAGE = ex.ToString();
                LogControl.save("SavePolizaEmit", ex.ToString(), "3");
            }

            return resultPackage;
        }

        public SalidaPlanilla InsuredVal(InsuredValBM insuredVal)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            SalidaPlanilla resultPackage = new SalidaPlanilla();
            resultPackage.C_TABLE = new List<ErroresVM>();
            List<ErroresVM> lista = new List<ErroresVM>();
            string codError = String.Empty;
            string storedProcedureName = ProcedureName.sp_ValidaTramaSCTR;
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, insuredVal.P_NID_PROC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Int32, insuredVal.P_NTYPE_TRANSAC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, insuredVal.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NRETARIF", OracleDbType.Int32, insuredVal.P_NRETARIF, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Date, insuredVal.P_DEFFECDATE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int64, insuredVal.P_NID_COTIZACION, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, resultPackage.P_COD_ERR, ParameterDirection.Output);
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, resultPackage.C_TABLE, ParameterDirection.Output);

                P_COD_ERR.Size = 4000;

                parameter.Add(P_COD_ERR);
                parameter.Add(C_TABLE);


                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                while (odr.Read())
                {
                    var item = new ErroresVM();
                    item.REGISTRO = odr["REGISTRO"].ToString();
                    item.DESCRIPCION = odr["DESCRIPCION"].ToString();

                    lista.Add(item);
                }
                codError = P_COD_ERR.Value.ToString();
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                codError = "1";
                lista.Add(new ErroresVM()
                {
                    REGISTRO = "0",
                    DESCRIPCION = ex.ToString()
                });
                LogControl.save("InsuredVal", ex.ToString(), "3");
            }

            resultPackage.P_COD_ERR = Convert.ToInt32(codError);
            resultPackage.C_TABLE = lista;

            return resultPackage;
        }

        public async Task<GenericPackageVM> GetPolizaEmitCab(string nroCotizacion, string typeMovement, int userCode, int FlagNoCIP = 0)
        {
            GenericPackageVM genericPackage = new GenericPackageVM();
            List<OracleParameter> parameter = new List<OracleParameter>();
            var polizaEmit = new PolizaEmitCAB();

            string storedProcedureName = ProcedureName.pkg_Cotizacion + ".REA_COT_EMI_CAB";

            try
            {
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, nroCotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_MOVEMENT", OracleDbType.Varchar2, typeMovement, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, userCode, ParameterDirection.Input));

                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, polizaEmit, ParameterDirection.Output);
                parameter.Add(C_TABLE);
                parameter.Add(new OracleParameter("P_FLAGNOCIP", OracleDbType.Int32, FlagNoCIP, ParameterDirection.Input));
                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);
                if (odr.HasRows)
                {
                    odr.Read();
                    polizaEmit.MENSAJE = odr["MENSAJE"].ToString();
                    polizaEmit.COD_ERR = odr["COD_ERR"].ToString();

                    if (polizaEmit.COD_ERR != "1")
                    {
                        polizaEmit.NID_COTIZACION = nroCotizacion;
                        polizaEmit.EFECTO_COTIZACION = String.Format("{0:MM/dd/yyyy}", odr["EFECTO_COTIZACION"]);
                        polizaEmit.EXPIRACION_COTIZACION = String.Format("{0:MM/dd/yyyy}", odr["EXPIRACION_COTIZACION"]);
                        polizaEmit.TIPO_DOCUMENTO = odr["TIPO_DOCUMENTO"].ToString();
                        polizaEmit.TIPO_DES_DOCUMENTO = odr["TIPO_DES_DOCUMENTO"].ToString().Trim();
                        polizaEmit.NUM_DOCUMENTO = odr["NUM_DOCUMENTO"].ToString();
                        polizaEmit.SCLIENT = odr["SCLIENT"].ToString();
                        polizaEmit.COD_TIPO_PERSONA = odr["COD_TIPO_PERSONA"].ToString();
                        polizaEmit.DES_TIPO_PERSONA = odr["DES_TIPO_PERSONA"].ToString();
                        polizaEmit.NOMBRE_RAZON = odr["NOMBRE_RAZON"].ToString();
                        polizaEmit.COD_MONEDA = odr["COD_MONEDA"].ToString();
                        polizaEmit.DES_MONEDA = odr["DES_MONEDA"].ToString();
                        polizaEmit.DIRECCION = odr["DIRECCION"].ToString();
                        polizaEmit.CORREO = odr["CORREO"].ToString();
                        polizaEmit.COD_TIPO_SEDE = odr["COD_TIPO_SEDE"].ToString();
                        polizaEmit.DES_TIPO_SEDE = odr["DES_SEDE"].ToString();
                        polizaEmit.COD_ACT_ECONOMICA = odr["COD_ACT_ECONOMICA"].ToString();
                        polizaEmit.DES_ACT_ECONOMICA = odr["DES_ACT_ECONOMICA"].ToString();
                        polizaEmit.COD_DEPARTAMENTO = odr["COD_DEPARTAMENTO"].ToString();
                        polizaEmit.DES_DEPARTAMENTO = odr["DES_DEPARTAMENTO"].ToString();
                        polizaEmit.COD_PROVINCIA = odr["COD_PROVINCIA"].ToString();
                        polizaEmit.DES_PROVINCIA = odr["DES_PROVINCIA"].ToString();
                        polizaEmit.COD_DISTRITO = odr["COD_DISTRITO"].ToString();
                        polizaEmit.DES_DISTRITO = odr["DES_DISTRITO"].ToString();
                        polizaEmit.MINA = odr["MINA"].ToString();
                        polizaEmit.POLIZA_MATRIZ = odr["POLIZA_MATRIZ"].ToString();
                        polizaEmit.COD_TIPO_FACTURACION = odr["COD_TIPO_FACTURACION"].ToString(); //accper
                        polizaEmit.DES_TIPO_FACTURACION = odr["DES_TIPO_FACTURACION"].ToString(); //accper
                        polizaEmit.FACTURA_ANTICIPADA = odr["FACTURA_ANTICIPADA"].ToString();
                        polizaEmit.COBERTURA_ADICIONAL = odr["COBERTURA_ADICIONAL"].ToString();
                        polizaEmit.DELIMITACION = odr["DELIMITACION"].ToString();
                        polizaEmit.ACT_TECNICA = odr["ACT_TECNICA"].ToString();
                        polizaEmit.DES_ACT_TECNICA = odr["DES_ACT_TECNICA"].ToString();
                        polizaEmit.MIN_SALUD = odr["MIN_SALUD"].ToString();
                        polizaEmit.MIN_SALUD_PR = odr["MIN_SALUD_PR"].ToString();
                        polizaEmit.MIN_SALUD_AUT = odr["MIN_SALUD_AUT"].ToString();
                        polizaEmit.MIN_PENSION = odr["MIN_PENSION"].ToString();
                        polizaEmit.MIN_PENSION_PR = odr["MIN_PENSION_PR"].ToString();
                        polizaEmit.MIN_PENSION_AUT = odr["MIN_PENSION_AUT"].ToString();
                        polizaEmit.COD_SEDE = odr["ID_SEDE"].ToString();
                        polizaEmit.DES_SEDE = odr["DES_SEDE"].ToString();
                        polizaEmit.ESTADO_COT = odr["SSTATREGT"].ToString();
                        polizaEmit.NTEMPORALITY = odr["NTEMPORALITY"].ToString(); //accper
                        polizaEmit.NSCOPE = odr["NSCOPE"].ToString(); //accper
                        polizaEmit.NTYPE_LOCATION = odr["NTYPE_LOCATION"].ToString(); //accper
                        polizaEmit.NLOCATION = odr["NLOCATION"].ToString(); //accper
                        polizaEmit.NMODULEC_TARIFARIO = odr["NMODULEC_TARIFARIO"].ToString(); //accper
                        polizaEmit.SDESCRIPT_TARIFARIO = odr["SDESCRIPT_TARIFARIO"].ToString(); //accper
                        polizaEmit.NTYPE_TRANSAC = odr["NTYPE_TRANSAC"].ToString();
                        /* Reglas Tarifario */
                        polizaEmit.NFLAG_COVERS = !String.IsNullOrEmpty(odr["NFLAG_COVERS"].ToString()) ? odr["NFLAG_COVERS"].ToString() == "1" ? true : false : false;
                        polizaEmit.NFLAG_BENEFITS = true; // !String.IsNullOrEmpty(odr["NFLAG_BENEFITS"].ToString()) ? odr["NFLAG_BENEFITS"].ToString() == "1" ? true : false : false;
                        polizaEmit.NFLAG_ASSIST = true; //  !String.IsNullOrEmpty(odr["NFLAG_ASSIST"].ToString()) ? odr["NFLAG_ASSIST"].ToString() == "1" ? true : false : false;
                        polizaEmit.NFLAG_SINIEST = !String.IsNullOrEmpty(odr["NFLAG_SINIEST"].ToString()) ? odr["NFLAG_SINIEST"].ToString() == "1" ? true : false : false;
                        polizaEmit.NFLAG_ALCANC = !String.IsNullOrEmpty(odr["NFLAG_ALCANC"].ToString()) ? odr["NFLAG_ALCANC"].ToString() == "1" ? true : false : false;
                        polizaEmit.NFLAG_TEMPOR = !String.IsNullOrEmpty(odr["NFLAG_TEMPOR"].ToString()) ? odr["NFLAG_TEMPOR"].ToString() == "1" ? true : false : false;
                        polizaEmit.SID_TARIFARIO = odr["SID_TARIFARIO"].ToString();
                        polizaEmit.NVERSION_TARIFARIO = odr["NVERSION_TARIFARIO"].ToString();
                        polizaEmit.SNAME_TARIFARIO = odr["SNAME_TARIFARIO"].ToString();
                        polizaEmit.NCOT_TARIFARIO = odr["NCOT_TARIFARIO"].ToString();
                        polizaEmit.NMONTO_PLANILLA = odr["NMONTO_PLANILLA"] == DBNull.Value ? 0 : Convert.ToDecimal(odr["NMONTO_PLANILLA"].ToString());
                        polizaEmit.NTOT_ASEGURADOS = odr["NTOT_ASEGURADOS"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NTOT_ASEGURADOS"].ToString());
                        polizaEmit.SVALIDA_TRAMA = odr["SVALIDA_TRAMA"].ToString();
                        polizaEmit.STIPO_COTIZACION = odr["STIPO_COTIZACION"].ToString();
                        polizaEmit.NPENSIONES_RE = odr["NPENSIONES_RE"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NPENSIONES_RE"].ToString());
                        polizaEmit.NASIG_COLEGIO = odr["NASIG_COLEGIO"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NASIG_COLEGIO"].ToString());
                        polizaEmit.NTASA_CALCULADA = odr["NTASA_CALCULADA"].ToString();
                        polizaEmit.NTASA_PROP = odr["NTASA_PROP"].ToString();
                        polizaEmit.NTASA_AUTOR = odr["NTASA_AUTOR"].ToString();
                        /*VIDA LEY*/
                        // desgravament 
                        polizaEmit.SCLIENT_PROVIDER = odr["SCLIENT_PROVIDER"].ToString();
                        polizaEmit.NNUM_CREDIT = odr["NNUM_CREDIT"].ToString();
                        polizaEmit.NNUM_CUOTA = odr["NNUM_CUOTA"].ToString();
                        polizaEmit.DFEC_OTORGAMIENTO = String.Format("{0:MM/dd/yyyy}", odr["DFEC_OTORGAMIENTO"]);
                        polizaEmit.NCAPITAL = odr["NCAPITAL"].ToString();
                        /*VIDA LEY*/
                        polizaEmit.MIN_VIDALEY = odr["MIN_VIDALEY"].ToString();
                        polizaEmit.MIN_VIDALEY_PR = odr["MIN_VIDALEY_PR"].ToString();
                        polizaEmit.MIN_VIDALEY_AUT = odr["MIN_VIDALEY_AUT"].ToString();
                        polizaEmit.TIP_RENOV = odr["NTIP_RENOV"].ToString();
                        polizaEmit.DES_TIP_RENOV = odr["DES_TIP_RENOV"].ToString();
                        polizaEmit.NID_TYPE_PROFILE = odr["NTYPE_PROFILE"].ToString();
                        polizaEmit.SDES_TYPE_PROFILE = odr["DES_TYPE_PROFILE"].ToString();
                        polizaEmit.NID_TYPE_PRODUCT = odr["NTYPE_PRODUCT"].ToString();
                        polizaEmit.SDES_TYPE_PRODUCT = odr["DES_TYPE_PRODUCT"].ToString();
                        polizaEmit.NID_TYPE_MODALITY = odr["NTYPE_MODALITY"].ToString();
                        polizaEmit.DES_TYPE_MODALITY = odr["DES_TYPE_MODALITY"].ToString();
                        polizaEmit.NPOLICY = odr["NPOLICY"].ToString();
                        polizaEmit.FREQ_PAGO = odr["NPAYFREQ"].ToString();
                        polizaEmit.DES_FREQ_PAGO = odr["DES_FREQ_PAGO"].ToString();
                        polizaEmit.ACT_TEC_VL = odr["ACT_TECNICA_VL"].ToString();
                        polizaEmit.DES_ACT_TEC_VL = odr["DES_ACT_TECNICA_VL"].ToString();
                        polizaEmit.ACT_ECO_VL = odr["ACT_ECONOMICA_VL"].ToString();
                        polizaEmit.DES_ACT_ECO_VL = odr["DES_ACT_ECONOMICA_VL"].ToString();
                        polizaEmit.TIP_COMISS = odr["NTIP_NCOMISSION"].ToString();
                        polizaEmit.DES_TIP_COMISS = odr["SDES_NCOMISSION"].ToString();
                        polizaEmit.EFECTO_COTIZACION_VL = String.Format("{0:MM/dd/yyyy}", odr["EFECTO_COTIZACION"]);
                        polizaEmit.EXPIRACION_COTIZACION_VL = String.Format("{0:MM/dd/yyyy}", odr["EXPIRACION_COTIZACION"]);
                        polizaEmit.DSTARTDATE_ASEG_VL = String.Format("{0:MM/dd/yyyy}", odr["DSTARTDATE_ASEG_VL"]);
                        polizaEmit.EFECTO_ASEGURADOS = String.Format("{0:MM/dd/yyyy}", odr["EFECTO_ASEGURADOS"]);
                        polizaEmit.EXPIRACION_ASEGURADOS = String.Format("{0:MM/dd/yyyy}", odr["EXPIRACION_ASEGURADOS"]);

                        //Exclusion
                        polizaEmit.FECHA_EXCLUSION = String.Format("{0:MM/dd/yyyy}", odr["FECHA_EXCLUSION"]);

                        polizaEmit.NID_PROC = odr["NID_PROC"].ToString();
                        polizaEmit.FECHA_REGISTRO = String.Format("{0:MM/dd/yyyy}", odr["FECHA_REGISTRO"]);
                        polizaEmit.NBRANCH = odr["NBRANCH"] == DBNull.Value ? 0 : int.Parse(odr["NBRANCH"].ToString());
                        polizaEmit.NPRODUCT = odr["NPRODUCT"] == DBNull.Value ? 0 : int.Parse(odr["NPRODUCT"].ToString());
                        polizaEmit.NCOMISION_SAL_PR = odr["NCOMISION_SAL_PR"] == DBNull.Value ? 0 : double.Parse(odr["NCOMISION_SAL_PR"].ToString());

                        if (odr["EFECTO_COT"].ToString() != null && odr["EFECTO_COT"].ToString().Trim() != "") polizaEmit.FECHA = Convert.ToDateTime(odr["EFECTO_COT"].ToString());
                        else polizaEmit.FECHA = null;

                        polizaEmit.COMENTARIO = odr["COMENTARIO"].ToString();

                        /* Mapfre */
                        polizaEmit.NCOMPANY_LNK = odr["NCOMPANY_LNK"] == DBNull.Value ? null : odr["NCOMPANY_LNK"].ToString(); // JDD
                        polizaEmit.SDES_COMPANY_LNK = odr["SDES_COMPANY_LNK"] == DBNull.Value ? null : odr["SDES_COMPANY_LNK"].ToString(); // JDD
                        polizaEmit.SCOTIZA_LNK = odr["SCOTIZA_LNK"] == DBNull.Value ? null : odr["SCOTIZA_LNK"].ToString(); // JDD
                        polizaEmit.SPOLICY_MPE = odr["SPOLICY_MPE"] == DBNull.Value ? null : odr["SPOLICY_MPE"].ToString(); // JDD

                        polizaEmit.NIDPLAN = odr["NIDPLAN"] == DBNull.Value ? "" : odr["NIDPLAN"].ToString(); // COVID
                        polizaEmit.SDES_PLAN = odr["SDES_PLAN"] == DBNull.Value ? "" : odr["SDES_PLAN"].ToString(); // COVID

                        polizaEmit.NREM_EXC = odr["NREM_EXC"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NREM_EXC"].ToString()); // rq Exc EH
                        polizaEmit.NTYPE_END = odr["NTYPE_END"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NTYPE_END"].ToString()); // RQ MEJORAS ENDOSO EHH

                        polizaEmit.NID_TRAMITE = odr["NID_TRAMITE"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NID_TRAMITE"].ToString());
                        polizaEmit.SMAIL_EJECCOM = odr["SMAIL_EJECCOM"] == DBNull.Value ? "" : odr["SMAIL_EJECCOM"].ToString();
                        polizaEmit.SDEVOLPRI = odr["SDEVOLPRI"] == DBNull.Value ? 0 : Convert.ToInt32(odr["SDEVOLPRI"].ToString());
                        polizaEmit.NFLAG_PAY_DIRECTO = odr["NFLAG_PAY_DIRECTO"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NFLAG_PAY_DIRECTO"].ToString());
                        polizaEmit.DEFFECDATE = String.Format("{0:MM/dd/yyyy}", odr["DEFFECDATE"]); // RI - FechaEfecto

                        // tramite de estado vida ley
                        polizaEmit.SPOL_ESTADO = odr["SPOL_ESTADO"] == DBNull.Value ? 0 : Convert.ToInt32(odr["SPOL_ESTADO"].ToString());
                        polizaEmit.SPOL_MATRIZ = odr["SPOL_MATRIZ"] == DBNull.Value ? 0 : Convert.ToInt32(odr["SPOL_MATRIZ"].ToString());
                        polizaEmit.APROB_CLI = odr["APROB_CLI"] == DBNull.Value ? 0 : Convert.ToInt32(odr["APROB_CLI"].ToString());
                        var lRutas = new string[] { ELog.obtainConfig("vidaIndividualBranch") };
                        // POLIZA MIXTA  SCTR INI AVS INTERCONEXION SABSA 16/10/2023
                        polizaEmit.NCOT_MIXTA = odr["NCOT_MIXTA"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NCOT_MIXTA"].ToString());
                        polizaEmit.NCIP_STATUS_PEN = odr["NCIP_STATUS_PEN"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NCIP_STATUS_PEN"].ToString());
                        polizaEmit.SCIP_NUMBER_PEN = odr["SCIP_NUMBER_PEN"] == DBNull.Value ? 0 : Convert.ToInt64(odr["SCIP_NUMBER_PEN"].ToString());
                        polizaEmit.NCIP_STATUS_EPS = odr["NCIP_STATUS_EPS"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NCIP_STATUS_EPS"].ToString());
                        polizaEmit.SCIP_NUMBER_EPS = odr["SCIP_NUMBER_EPS"] == DBNull.Value ? 0 : Convert.ToInt64(odr["SCIP_NUMBER_EPS"].ToString());
                        polizaEmit.NID_PROC_SCTR = odr["NID_PROC_SCTR"] == DBNull.Value ? "" : odr["NID_PROC_SCTR"].ToString();
                        polizaEmit.NID_PROC_EPS = odr["NID_PROC_EPS"] == DBNull.Value ? "" : odr["NID_PROC_EPS"].ToString();
                        polizaEmit.NTYPE_PROCESO = odr["NTYPE_PROCESO"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NTYPE_PROCESO"].ToString());
                        polizaEmit.ESTADO_MSG = odr["ESTADO_MSG"] == DBNull.Value ? null : odr["ESTADO_MSG"].ToString(); //AVS - EPS

                        // POLIZA MIXTA  SCTR FIN 
                        if (lRutas.Contains(odr["NBRANCH"].ToString()))
                        {
                            // CONSULTA DPS PARA API
                            polizaEmit.OCCUPATION = odr["OCCUPATION"] == DBNull.Value ? "" : odr["OCCUPATION"].ToString(); // VCF
                            polizaEmit.ACTIVITY = odr["ACTIVITY"] == DBNull.Value ? "" : odr["ACTIVITY"].ToString(); // VCF V.2
                            polizaEmit.SID_DPS = odr["SID_DPS"].ToString();
                            polizaEmit.STOKEN_DPS = odr["STOKEN_DPS"].ToString();
                            polizaEmit.FECHA_NAC = odr["DBIRTHDAT"].ToString();
                            polizaEmit.EstadoDPS = odr["SSTATREGT_DPS"].ToString();
                            /*
                            List<QuotizacionCliBM> datosCLi = new List<QuotizacionCliBM>();
                            datosCLi = request.QuotationCli;
                            */

                            var jsonDPS = await invocarServicioConsultaDPS(polizaEmit.SID_DPS, polizaEmit.STOKEN_DPS);

                            //quotationVM.DpsToken responsedps = new quotationVM.DpsToken();
                            if (!string.IsNullOrEmpty(jsonDPS))
                            {
                                var responsedps = JsonConvert.DeserializeObject<quotationVM.DpsToken>(jsonDPS, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                                if (responsedps.success)
                                {
                                    try
                                    {
                                        if (responsedps.idEstado == "6" && polizaEmit.EstadoDPS != "6")
                                        {
                                            var datosdps = !string.IsNullOrEmpty(responsedps.dps) ? JsonConvert.DeserializeObject<quotationVM.AnswerDPS>(responsedps.dps, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }) : null;

                                            // AGF 25052023 Cambiando orden resppuestas DPS
                                            if (datosdps != null)
                                            {
                                                var position0 = datosdps?.answers[0];
                                                datosdps.answers[0] = datosdps.answers[1];
                                                datosdps.answers[0].question = "1";
                                                datosdps.answers[1] = position0;
                                                datosdps.answers[1].question = "2";

                                                var resAns = new quotationVM.SalidaErrorBaseVM();
                                                //Grabar DPS
                                                foreach (var answer in datosdps.answers)
                                                {
                                                    //quotationVM.DatosDPS insdps = new quotationVM.DatosDPS();
                                                    //insdps = item;
                                                    resAns = QuotationDA.regDPS(int.Parse(nroCotizacion), odr["NID_PROC"].ToString(), int.Parse(odr["NBRANCH"].ToString()), int.Parse(odr["NPRODUCT"].ToString()), userCode, answer);

                                                    if (resAns.P_COD_ERR == 0)
                                                    {
                                                        if (answer.items != null)
                                                        {
                                                            foreach (var itemdetail in answer.items)
                                                            {
                                                                quotationVM.DatosDPS insdpsdetail = new quotationVM.DatosDPS();
                                                                insdpsdetail = itemdetail;
                                                                insdpsdetail.questionFather = answer.question;
                                                                resAns = QuotationDA.regDPS(int.Parse(nroCotizacion), odr["NID_PROC"].ToString(), int.Parse(odr["NBRANCH"].ToString()), int.Parse(odr["NPRODUCT"].ToString()), userCode, insdpsdetail);
                                                            }
                                                        }
                                                    }
                                                }

                                                if (resAns.P_COD_ERR == 0)
                                                {
                                                    //Registro de datos Biometricos
                                                    QuotationDA.regBIO(int.Parse(nroCotizacion), responsedps.numeroDocumento, responsedps.nombreCliente, responsedps.algoritmo, responsedps.porcentajeSimilitud, responsedps.urlImagen, responsedps.fechaEvaluacion, responsedps.tipoValidacion, responsedps.codigoVerificacion);
                                                    //Actualizar Permisos Core
                                                    QuotationDA.UpdateDatosPer(polizaEmit.SCLIENT, datosdps.privacidad.ToString());
                                                    //Actualizar EstadoDPS
                                                    QuotationDA.UpdateDPS(Int32.Parse(nroCotizacion), userCode, polizaEmit.SID_DPS, polizaEmit.STOKEN_DPS, Int32.Parse(responsedps.idEstado));
                                                }
                                            }
                                        }
                                        polizaEmit.idEstadoDPS = responsedps.idEstado;
                                        polizaEmit.EstadoDPS = responsedps.estado;
                                    }
                                    catch (Exception ex)
                                    {
                                        LogControl.save("GetPolizaEmitCabDPS", ex.ToString(), "3");
                                    }
                                }
                            }

                        }


                        try
                        {
                            polizaEmit.RUTAS = sharedMethods.GetFilePathList(String.Format(ELog.obtainConfig("pathPrincipal"), odr["RUTA"].ToString()));
                        }
                        catch (ArgumentNullException)
                        {
                            polizaEmit.RUTAS = new List<string>();
                        }
                        catch (ArgumentException)
                        {
                            polizaEmit.RUTAS = new List<string>();
                        }
                        catch (System.IO.DirectoryNotFoundException)    //Si la ruta no fue encontrada, debemos enviar un mensaje de error
                        {
                            genericPackage.StatusCode = 0;
                            genericPackage.GenericResponse = polizaEmit;
                            polizaEmit.RUTAS = new List<string>();
                        }

                    }

                    genericPackage.StatusCode = 0;
                    genericPackage.SuccessMessageList = new List<string>(new string[] { "Operación exitosa." });
                    genericPackage.GenericResponse = polizaEmit;

                    ELog.CloseConnection(odr);
                }
            }
            catch (Exception ex)
            {
                genericPackage.StatusCode = 1;
                polizaEmit.MENSAJE = "Cotización no existe.";
                polizaEmit.COD_ERR = "1";
                genericPackage.GenericResponse = polizaEmit;
                LogControl.save("GetPolizaEmitCab", ex.ToString(), "3");
            }

            return genericPackage;
        }

        public async Task<string> invocarServicioConsultaDPS(string id, string token)
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
                    var tariffToken = JsonConvert.DeserializeObject<quotationVM.AuthTariffResult>(mToken);

                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tariffToken.token);
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                        var response = await client.GetAsync(ELog.obtainConfig("consultaDPS") + "/" + token);
                        sTariffResult = await response.Content.ReadAsStringAsync();
                        //sTariffResult = "";
                    }
                }
            }
            catch (Exception ex)
            {
                LogControl.save("invocarServicioConsultaDPS", ex.ToString(), "3");
            }

            return await Task.FromResult(sTariffResult);
        }

        public List<PolizaEmitComer> GetPolizaEmitComer(int nroCotizacion)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<PolizaEmitComer> lista = new List<PolizaEmitComer>();
            string storedProcedureName = ProcedureName.pkg_Cotizacion + ".REA_COT_EMI_COMER";
            try
            {
                parameter.Add(new OracleParameter("NRO_COTIZACION", OracleDbType.Int32, nroCotizacion, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, lista, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);
                while (odr.Read())
                {
                    PolizaEmitComer polizaEmitComer = new PolizaEmitComer();
                    polizaEmitComer.TYPE_DOC_COMER = Convert.ToInt32(odr["TYPE_DOC_COMER"]);
                    polizaEmitComer.DES_DOC_COMER = odr["DES_DOC_COMER"].ToString();
                    polizaEmitComer.DOC_COMER = odr["DOC_COMER"].ToString();
                    polizaEmitComer.COMERCIALIZADOR = odr["COMERCIALIZADOR"].ToString();
                    polizaEmitComer.COMISION_SALUD = odr["COMISION_SALUD"].ToString();
                    polizaEmitComer.COMISION_SALUD_PRO = odr["COMISION_SALUD_PRO"].ToString();
                    polizaEmitComer.COMISION_SALUD_AUT = odr["COMISION_SALUD_AUT"].ToString();
                    polizaEmitComer.COMISION_PENSION = odr["COMISION_PENSION"].ToString();
                    polizaEmitComer.COMISION_PENSION_PRO = odr["COMISION_PENSION_PRO"].ToString();
                    polizaEmitComer.COMISION_PENSION_AUT = odr["COMISION_PENSION_AUT"].ToString();
                    polizaEmitComer.PRINCIPAL = Convert.ToInt32(odr["PRINCIPAL"].ToString());
                    polizaEmitComer.TIPO_CANAL = odr["TIPO_CANAL"].ToString();
                    polizaEmitComer.CANAL = odr["CANAL_BR"].ToString();
                    polizaEmitComer.SCLIENT = odr["CLIENT_CAN_BR"].ToString();
                    polizaEmitComer.NSHARE = odr["NSHARE"].ToString();
                    polizaEmitComer.TYPE_INTERMEDIARY = odr["TYPE_INTERMEDIARY"] == DBNull.Value ? "" : odr["TYPE_INTERMEDIARY"].ToString();
                    polizaEmitComer.CODE_SBS = odr["CODE_SBS"] == DBNull.Value ? "" : odr["CODE_SBS"].ToString();
                    polizaEmitComer.NLOCAL = odr["NLOCAL"] == DBNull.Value ? "" : odr["NLOCAL"].ToString();
                    polizaEmitComer.NTIP_NCOMISSION = odr["NTIP_NCOMISSION"] == DBNull.Value ? "" : odr["NTIP_NCOMISSION"].ToString();
                    polizaEmitComer.NCORREDOR = odr["NCORREDOR"] == DBNull.Value ? "" : odr["NCORREDOR"].ToString();
                    polizaEmitComer.NINTERTYP = odr["NINTERTYP"] == DBNull.Value ? "" : odr["NINTERTYP"].ToString();
                    polizaEmitComer.NTYPE_PROCESO = odr["NTYPE_PROCESO"] == DBNull.Value ? "" : odr["NTYPE_PROCESO"].ToString();


                    lista.Add(polizaEmitComer);
                }
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                LogControl.save("GetPolizaEmitComer", ex.ToString(), "3");
            }

            return lista;
        }

        public List<PolizaEmitDet> GetPolizaEmitDet(int nroCotizacion, int? userCode)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<PolizaEmitDet> lista = new List<PolizaEmitDet>();
            string storedProcedureName = ProcedureName.pkg_Cotizacion + ".REA_COT_EMI_DET";
            try
            {
                parameter.Add(new OracleParameter("NRO_COTIZACION", OracleDbType.Int32, nroCotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE ", OracleDbType.Int32, userCode, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, lista, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                while (odr.Read())
                {
                    PolizaEmitDet polizaEmitDet = new PolizaEmitDet();
                    polizaEmitDet.ID_PRODUCTO = odr["ID_PRODUCTO"].ToString();
                    polizaEmitDet.NOMBRE_PRODUCT = odr["NOMBRE_PRODUCT"].ToString();
                    polizaEmitDet.TIP_RIESGO = odr["TIP_RIESGO"].ToString();
                    polizaEmitDet.DES_RIESGO = odr["DES_RIESGO"].ToString();
                    polizaEmitDet.NUM_TRABAJADORES = odr["NTOTAL_TRABAJADORES"].ToString();
                    polizaEmitDet.MONTO_PLANILLA = odr["MONTO_PLANILLA"].ToString();
                    polizaEmitDet.TASA_CALC = odr["TASA_CALC"].ToString();
                    polizaEmitDet.TASA_PRO = odr["TASA_PRO"].ToString();
                    polizaEmitDet.TASA = odr["TASA_AUTO"].ToString();
                    polizaEmitDet.PRIMA = odr["PRIMA"].ToString();
                    polizaEmitDet.FACTOR_IGV = odr["FACTOR_IGV"].ToString();
                    polizaEmitDet.PRIMA_MIN = odr["PRIMA_MIN"].ToString();
                    polizaEmitDet.PRIMA_MIN_PRO = odr["PRIMA_MIN_PRO"].ToString();
                    polizaEmitDet.PRIMA_MIN_AUT = odr["PRIMA_MIN_AUT"].ToString();
                    polizaEmitDet.TASA_RIESGO = odr["NRATE"].ToString();
                    polizaEmitDet.DESCUENTO = odr["NDISCOUNT"].ToString();
                    polizaEmitDet.VARIACION_TASA = odr["NACTIVITYVARIATION"].ToString();
                    polizaEmitDet.AUT_PRIMA = odr["AUT_PREMIUM"].ToString();
                    polizaEmitDet.PRIMA_END = odr["NPREMIUM_END"].ToString();
                    polizaEmitDet.NSUM_PREMIUMN = odr["NSUM_PREMIUMN"].ToString();
                    polizaEmitDet.NSUM_IGV = odr["NSUM_IGV"].ToString();
                    polizaEmitDet.NSUM_PREMIUM = odr["NSUM_PREMIUM"].ToString();
                    polizaEmitDet.NTASA_SEGURO = odr["NTASA_SEGURO"].ToString();
                    polizaEmitDet.NTASA_AHORRO = odr["NTASA_AHORRO"].ToString();
                    polizaEmitDet.NCUMULO_MAX = odr["NCUMULO_MAX"].ToString();
                    polizaEmitDet.SRANGO_EDAD = odr["SRANGO_EDAD"].ToString();
                    lista.Add(polizaEmitDet);
                }
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                LogControl.save("GetPolizaEmitDet", ex.ToString(), "3");
            }

            return lista;
        }

        public List<PolizaEmitDet> GetPolizaEmitDetTX(string processId, string typeMovement, int? userCode, int? vencido)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<PolizaEmitDet> detList = new List<PolizaEmitDet>();
            string storedProcedureName = ProcedureName.pkg_Cotizacion + ".REA_COT_EMI_DET_TR";

            try
            {
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, processId, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_TRANSAC ", OracleDbType.Varchar2, typeMovement, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE ", OracleDbType.Int32, userCode, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FLAG_VENCIDO ", OracleDbType.Int32, vencido, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, detList, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                while (odr.Read())
                {
                    PolizaEmitDet polizaEmitDet = new PolizaEmitDet();
                    polizaEmitDet.ID_PRODUCTO = odr["ID_PRODUCTO"].ToString();
                    polizaEmitDet.NOMBRE_PRODUCT = odr["NOMBRE_PRODUCT"].ToString();
                    polizaEmitDet.TIP_RIESGO = odr["TIP_RIESGO"].ToString();
                    polizaEmitDet.DES_RIESGO = odr["DES_RIESGO"].ToString();
                    polizaEmitDet.NUM_TRABAJADORES = odr["NTOTAL_TRABAJADORES"].ToString();
                    polizaEmitDet.MONTO_PLANILLA = odr["MONTO_PLANILLA"].ToString();
                    polizaEmitDet.TASA_CALC = odr["TASA_CALC"].ToString();
                    polizaEmitDet.TASA_PRO = odr["TASA_PRO"].ToString();
                    polizaEmitDet.TASA = odr["TASA_AUTO"].ToString();
                    polizaEmitDet.PRIMA = odr["PRIMA"].ToString();
                    polizaEmitDet.FACTOR_IGV = odr["FACTOR_IGV"].ToString();
                    polizaEmitDet.PRIMA_MIN = odr["PRIMA_MIN"].ToString();
                    polizaEmitDet.PRIMA_MIN_PRO = odr["PRIMA_MIN_PRO"].ToString();
                    polizaEmitDet.TASA_RIESGO = odr["NRATE"].ToString();
                    polizaEmitDet.DESCUENTO = odr["NDISCOUNT"].ToString();
                    polizaEmitDet.VARIACION_TASA = odr["NACTIVITYVARIATION"].ToString();
                    polizaEmitDet.AUT_PRIMA = odr["AUT_PREMIUM"].ToString();
                    polizaEmitDet.PRIMA_END = odr["NPREMIUM_END"].ToString();
                    polizaEmitDet.NSUM_PREMIUMN = odr["NSUM_PREMIUMN"].ToString();
                    polizaEmitDet.NSUM_PREMIUMC = odr["NSUM_PREMIUMC"].ToString();
                    polizaEmitDet.NSUM_IGV = odr["NSUM_IGV"].ToString();
                    polizaEmitDet.NSUM_PREMIUM = odr["NSUM_PREMIUM"].ToString();
                    polizaEmitDet.REMUNERACION_TOPE = odr["REMUNERACION_TOPE"].ToString();
                    polizaEmitDet.REMUN_TOPE_DESDE = odr["REMUN_TOPE_DESDE"].ToString();
                    polizaEmitDet.REMUN_TOPE_HASTA = odr["REMUN_TOPE_HASTA"].ToString();
                    polizaEmitDet.OPC_MES_VENCIDO = odr["OPC_MES_VENCIDO"].ToString();
                    detList.Add(polizaEmitDet);
                }
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                LogControl.save("GetPolizaEmitDetTX", ex.ToString(), "3");
            }

            return detList;
        }

        public List<PolicyDetVM> GetPolizaCot(int nroCotizacion, int nroTransac = 0)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<PolicyDetVM> result = new List<PolicyDetVM>();
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, nroCotizacion, ParameterDirection.Input));

                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, result, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                parameter.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Int32, nroTransac, ParameterDirection.Input));

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(ProcedureName.pkg_Cotizacion + ".REA_COT_POL", parameter);

                while (odr.Read())
                {
                    PolicyDetVM polizaDet = new PolicyDetVM();
                    polizaDet.TIP_RENOV = odr["TIP_RENOV"].ToString();
                    polizaDet.DESDE = odr["DESDE"].ToString();
                    polizaDet.HASTA = odr["HASTA"].ToString();
                    polizaDet.DESDE_ASEG = odr["DESDE_ASEG"].ToString();
                    polizaDet.HASTA_ASEG = odr["HASTA_ASEG"].ToString();
                    polizaDet.FREC_PAGO = odr["FREC_PAGO"].ToString();
                    polizaDet.FACT_MES_VENC = odr["FACT_MES_VENC"].ToString();
                    polizaDet.FACT_ANTI = odr["FACT_ANTI"].ToString();
                    polizaDet.POL_PEN = odr["POL_PEN"].ToString();
                    polizaDet.POL_SAL = odr["POL_SAL"].ToString();

                    result.Add(polizaDet);
                }
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                LogControl.save("GetPolizaCot", ex.ToString(), "3");
            }

            return result;
        }

        public List<TypeTrabajadorVM> GetListTypeTrabajador(Int64 request)
        {
            var sPackageName = ProcedureName.pkg_SedesCliente + ".SEL_LIST_TYPE_TRABAJ";
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<TypeTrabajadorVM> ListTypeTrabajador = new List<TypeTrabajadorVM>();

            try
            {
                //OUTPUT
                parameter.Add(new OracleParameter("P_COTIZACION", OracleDbType.Int64, request, ParameterDirection.Input));
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter))
                {
                    ListTypeTrabajador = dr.ReadRowsList<TypeTrabajadorVM>();
                    ELog.CloseConnection(dr);

                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetListTypeTrabajador", ex.ToString(), "3");
            }

            return ListTypeTrabajador;
        }

        public List<SedeVM> GetListSede(string Client)
        {
            var sPackageName = ProcedureName.pkg_SedesCliente + ".SEL_CLIENTLOCATIONLIST";
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<SedeVM> ListSede = new List<SedeVM>();

            try
            {
                parameter.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, Client, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLIMITPERPAGE", OracleDbType.Int64, 1000, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPAGENUM", OracleDbType.Int64, 1, ParameterDirection.Input));
                OracleParameter NTOTALROWS = new OracleParameter("NTOTALROWS", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);

                NTOTALROWS.Size = 4000;

                parameter.Add(C_TABLE);
                parameter.Add(NTOTALROWS);

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter))
                {
                    ListSede = dr.ReadRowsList<SedeVM>();
                    ELog.CloseConnection(dr);
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetListSede", ex.ToString(), "3");
            }

            return ListSede;
        }

        public SalidadPolizaEmit valTransactionPolicy(int nroCotizacion, int idTipo)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            SalidadPolizaEmit resultPackage = new SalidadPolizaEmit();
            string storedProcedureName = ProcedureName.pkg_Poliza + ".VAL_ANUL_POL";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, nroCotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_ID_TIPO_TRANS", OracleDbType.Int32, idTipo, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, resultPackage.P_MESSAGE, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, resultPackage.P_COD_ERR, ParameterDirection.Output);

                P_MESSAGE.Size = 4000;
                parameter.Add(P_MESSAGE);
                parameter.Add(P_COD_ERR);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                resultPackage.P_MESSAGE = P_MESSAGE.Value.ToString();
                resultPackage.P_COD_ERR = Int32.Parse(P_COD_ERR.Value.ToString());
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                //resultPackage.P_MESSAGE = ex.ToString();
                resultPackage.P_MESSAGE = "Hubo un error al validar. Favor de comunicarse con sistemas";
                resultPackage.P_COD_ERR = 1;
                LogControl.save("valTransactionPolicy", ex.ToString(), "3");
            }

            return resultPackage;
        }

        public DisplayProcessVM VisualizadorProc(DisplayProcessBM request)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            DisplayProcessVM result = new DisplayProcessVM();
            List<ProcessVM> listProcess = new List<ProcessVM>();
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, request.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Date, Convert.ToDateTime(request.P_DEFFECDATE), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLIMITPERPAGE", OracleDbType.Int32, 99999, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPAGENUM", OracleDbType.Int32, 1, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter NTOTALROWS = new OracleParameter("P_NTOTALROWS", OracleDbType.Int32, 4000, null, ParameterDirection.Output);

                parameter.Add(C_TABLE);
                parameter.Add(NTOTALROWS);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(ProcedureName.sp_VisualizadorProcesos, parameter);

                while (odr.Read())
                {
                    ProcessVM process = new ProcessVM();
                    process.NUM_COTIZACION = odr["NUM_COTIZACION"].ToString();
                    process.NUM_CONSTANCIA = odr["NUM_CONSTANCIA"].ToString();
                    process.TIPO_MOVIMIENTO = odr["TIPO_MOVIMIENTO"].ToString();
                    process.DES_PRODUCT = odr["DES_PRODUCT"].ToString();
                    process.NUM_POLIZA = odr["NUM_POLIZA"].ToString();
                    process.DES_CONTR = odr["DES_CONTR"].ToString();
                    process.DES_USUARIO = odr["DES_USUARIO"].ToString();
                    process.FECHA_HORA_ENVIO = odr["FECHA_HORA_ENVIO"].ToString();
                    process.FECHA_HORA_FIN = odr["FECHA_HORA_FIN"].ToString();
                    process.DES_ESTADO = odr["DES_ESTADO"].ToString();
                    process.SKEY = odr["SKEY"].ToString();
                    process.LINEA = odr["LINEA"].ToString();

                    listProcess.Add(process);
                }
                ELog.CloseConnection(odr);
                result.numRow = Convert.ToInt32(NTOTALROWS.Value.ToString());
                result.listProcess = listProcess;
            }
            catch (Exception ex)
            {
                LogControl.save("VisualizadorProc", ex.ToString(), "3");
            }

            return result;
        }

        public ResponseVM SavedPolicyTransac(SavedPolicyTXBM data)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseVM resultPackage = new ResponseVM();
            string storedProcedureName = ProcedureName.pkg_ValidaReglas + ".SPS_ENVIO_BANDEJA_TR";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, data.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Varchar2, data.P_NTYPE_TRANSAC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, data.P_NID_PROC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOMMENT", OracleDbType.Varchar2, data.P_SCOMMENT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SASIGNAR", OracleDbType.Varchar2, data.P_SASIGNAR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SAPROBADO", OracleDbType.Varchar2, data.P_SAPROBADO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Date, Convert.ToDateTime(data.P_DEFFECDATE), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, Convert.ToDateTime(data.P_DEXPIRDAT), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FACT_MES_VENCIDO", OracleDbType.Int32, data.P_FACT_MES_VENCIDO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SFLAG_FAC_ANT", OracleDbType.Char, data.P_SFLAG_FAC_ANT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOLTIMRE", OracleDbType.Char, data.P_SCOLTIMRE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPAYFREQ", OracleDbType.Int32, data.P_NPAYFREQ, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NAMO_AFEC", OracleDbType.Double, data.P_NAMO_AFEC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIVA", OracleDbType.Double, data.P_NIVA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NAMOUNT", OracleDbType.Double, data.P_NAMOUNT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SRUTA", OracleDbType.Varchar2, null, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, resultPackage.P_NCODE, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, resultPackage.P_SMESSAGE, ParameterDirection.Output);
                OracleParameter P_NCONSTANCIA = new OracleParameter("P_NCONSTANCIA", OracleDbType.Int32, resultPackage.P_NCONSTANCIA, ParameterDirection.Output);

                P_SMESSAGE.Size = 4000;
                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);
                parameter.Add(P_NCONSTANCIA);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                resultPackage.P_NCODE = P_NCODE.Value.ToString();
                resultPackage.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                resultPackage.P_NCONSTANCIA = P_NCONSTANCIA.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                resultPackage.P_NCODE = "1";
                resultPackage.P_SMESSAGE = ex.ToString();
                LogControl.save("SavedPolicyTransac", ex.ToString(), "3");
            }

            return resultPackage;
        }

        public List<AseguradoExcelVM> getListAseguradosExcel(string cotizacion, string poliza)
        {
            var sPackageName = ProcedureName.pkg_SedesCliente + ".SEL_LIST_ASEGURADOS_COTIZAC";
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<AseguradoExcelVM> ListAsegurados = new List<AseguradoExcelVM>();

            try
            {
                parameter.Add(new OracleParameter("P_COTIZACION", OracleDbType.Varchar2, cotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_POLICY", OracleDbType.Varchar2, cotizacion, ParameterDirection.Input));
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);

                parameter.Add(C_TABLE);

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter))
                {
                    ListAsegurados = dr.ReadRowsList<AseguradoExcelVM>();
                    ELog.CloseConnection(dr);
                }
            }
            catch (Exception ex)
            {
                LogControl.save("getListAseguradosExcel", ex.ToString(), "3");
            }

            return ListAsegurados;
        }

        public List<ConfigVM> getDataConfig(string stype)
        {
            var sPackageName = ProcedureName.pkg_Poliza + ".REA_CONFIG";
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<ConfigVM> configData = new List<ConfigVM>();

            try
            {
                parameter.Add(new OracleParameter("P_STYPE", OracleDbType.Varchar2, stype, ParameterDirection.Input));
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);

                parameter.Add(C_TABLE);

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter))
                {
                    configData = dr.ReadRowsList<ConfigVM>();
                }
            }
            catch (Exception ex)
            {
                LogControl.save("getDataConfig", ex.ToString(), "3");
            }

            return configData;
        }

        public async Task<SalidadPolizaEmit> insertPrePayment(PrePaymentBM data)
        {
            //data.pathFile = String.Format(ELog.obtainConfig("pathPrincipal"), ELog.obtainConfig("pathPoliza") + data.quotationNumber + "\\" + ELog.obtainConfig("pathAdjuntos") + "\\" + ELog.obtainConfig("movimiento" + data.typeTransaction) + "\\" + DateTime.Now.ToString("yyyyMMddhhmmss") + "\\");
            List<OracleParameter> parameter = new List<OracleParameter>();
            SalidadPolizaEmit resultPackage = new SalidadPolizaEmit();
            string storedProcedureName = ProcedureName.pkg_Poliza + ".INS_POLIZA_PREPAY";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, data.idProcess, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.quotationNumber, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Int32, data.typeTransaction, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SJSON_PRT", OracleDbType.Varchar2, data.jsonPrt, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SJSON_LNK", OracleDbType.Varchar2, data.jsonLnk, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, data.userCode, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, resultPackage.P_MESSAGE, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, resultPackage.P_COD_ERR, ParameterDirection.Output);

                P_MESSAGE.Size = 4000;
                parameter.Add(P_MESSAGE);
                parameter.Add(P_COD_ERR);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                resultPackage.P_MESSAGE = P_MESSAGE.Value.ToString();
                resultPackage.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                ELog.CloseConnection(odr);
                //guardarArchivos(data.pathFile, files);
            }
            catch (Exception ex)
            {
                resultPackage.P_COD_ERR = 1;
                resultPackage.P_MESSAGE = ex.ToString();
                LogControl.save("insertPrePayment", ex.ToString(), "3");
            }

            return await Task.FromResult(resultPackage);
        }
        public string ValidateRetroactivity(int cotizacion, int usercode, string efectdate, string transac)
        {
            QuotationDA quotationDA = new QuotationDA();
            QuotationCabBM quotationCab = new QuotationCabBM()
            {
                P_NBRANCH = Convert.ToInt32(ELog.obtainConfig("vidaLeyBranch")),
                P_NPRODUCT = Convert.ToInt32(ELog.obtainConfig("vidaLeyKey")),
                NumeroCotizacion = cotizacion,
                P_NUSERCODE = usercode,
                P_DSTARTDATE_ASE = efectdate,
                P_DSTARTDATE = efectdate,
                TrxCode = transac,
                RetOP = 2
            };
            WSPlataforma.Entities.QuotationModel.ViewModel.QuotationResponseVM quotationRes = new WSPlataforma.Entities.QuotationModel.ViewModel.QuotationResponseVM();
            quotationRes = quotationDA.ValRetroCotizacion(quotationCab, cotizacion);
            return quotationRes.P_SMESSAGE;
        }

        public Entities.QuotationModel.ViewModel.QuotationResponseVM ProcesarEndosoVL(QuotationCabBM request)
        {
            var response = new Entities.QuotationModel.ViewModel.QuotationResponseVM();
            DbConnection DataConnection = ConnectionGet(enuTypeDataBase.OracleVTime);
            DbTransaction trx = null;

            try
            {
                List<HttpPostedFile> files = new List<HttpPostedFile>();
                foreach (var item in HttpContext.Current.Request.Files)
                {
                    HttpPostedFile arch = HttpContext.Current.Request.Files[item.ToString()];
                    files.Add((HttpPostedFile)HttpContext.Current.Request.Files[item.ToString()]);
                }

                string folderPath = files.Count > 0 ? ELog.obtainConfig("pathCotizacion") + request.NumeroCotizacion + "\\" + ELog.obtainConfig("movimiento") + "\\" + DateTime.Now.ToString("yyyyMMddhhmmss") + "\\" : string.Empty;
                request.P_SRUTA = folderPath;

                DataConnection.Open();
                trx = DataConnection.BeginTransaction();

                response.P_COD_ERR = 0;

                if (request.recotizacion == 1)
                {
                    response = QuotationDA.UpdateQuotationCab(request, DataConnection, trx);
                }

                if (response.P_COD_ERR == 0)
                {
                    response = AjusteEndoso(request, DataConnection, trx);
                }

                int valor = 1; // GCAA 24062024

                // Homologar de forma correcta por favor
                if (request.QuotationDet != null)
                {
                    foreach (var item in request.QuotationDet)
                    {
                        response = response.P_COD_ERR == 0 ? EndosoDet(request, item, DataConnection, trx) : response;
                    }

                    foreach (var item in request.QuotationDet)
                    {
                        // GCAA 28122023
                        if (response.P_MESSAGE == "3")
                        {
                            response = response.P_COD_ERR == 0 ? EndosoClose(request, DataConnection, trx, item.P_RANGO, request.QuotationDet.Count(), valor) : response;
                        }
                        valor += 1;
                    }
                }

                if (response.P_MESSAGE != "3")
                {
                    response = response.P_COD_ERR == 0 ? EndosoClose(request, DataConnection, trx) : response;
                }

                response = response.P_COD_ERR == 0 ? QuotationDA.UpdateCodQuotation(Convert.ToInt32(request.NumeroCotizacion), request.CodigoProceso, DataConnection, trx) : response;
                response = response.P_COD_ERR == 0 ? QuotationDA.ValRetroCotizacion(request, request.NumeroCotizacion.Value, DataConnection, trx) : response;
                response = response.P_COD_ERR == 0 ? QuotationDA.ValCotizacion(request.NumeroCotizacion.Value, request.P_NUSERCODE, 0, 0, DataConnection, trx) : response;

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

            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = ex.ToString();
                LogControl.save("ProcesarEndosoVL", ex.ToString(), "3");
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

        public SalidadPolizaEmit insertarHistorial(PolicyTransactionSaveBM data)
        {
            var response = new SalidadPolizaEmit();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storedProcedureName = ProcedureName.pkg_Cotizacion + ".INS_COTIZA_HIS";
            try
            {
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_ESTADO", OracleDbType.Int32, data.flagTramite ? 10 : 20, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMOTIVO", OracleDbType.Int32, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, data.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOMMENT", OracleDbType.Varchar2, data.P_SCOMMENT, ParameterDirection.Input));

                parameter.Add(new OracleParameter("P_SRUTA", OracleDbType.Varchar2, data.P_SRUTA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, data.P_NPRODUCTO, ParameterDirection.Input));
                //parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.P_NBRANCH, ParameterDirection.Input));

                parameter.Add(new OracleParameter("P_NGLOSS_COT", OracleDbType.Int32, null, ParameterDirection.Input)); // Mejora SCTR
                parameter.Add(new OracleParameter("P_SGLOSS_COT", OracleDbType.Varchar2, null, ParameterDirection.Input)); // Mejora SCTR
                                                                                                                           //OUTPUT
                OracleParameter P_COD_ESTADO = new OracleParameter("P_COD_ESTADO", OracleDbType.Int32, null, ParameterDirection.Output);
                OracleParameter P_MENSAJE = new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, null, ParameterDirection.Output);

                P_MENSAJE.Size = 4000;
                parameter.Add(P_COD_ESTADO);
                parameter.Add(P_MENSAJE);
                //parameter.Add(P_DDATEAPROB);

                if (data.flagTramite)
                {
                    parameter.Add(new OracleParameter("P_DDATEAPROB", OracleDbType.Date, DateTime.Now.ToShortDateString(), ParameterDirection.Input)); // JDD Emerson
                    //OracleParameter P_DDATEAPROB = new OracleParameter("P_DDATEAPROB", OracleDbType.Date, DateTime.Now.ToShortDateString(), ParameterDirection.Output);
                }
                else
                {
                    parameter.Add(new OracleParameter("P_DDATEAPROB", OracleDbType.Date, DBNull.Value, ParameterDirection.Input)); // JDD Emerson
                    //OracleParameter P_DDATEAPROB = new OracleParameter("P_DDATEAPROB", OracleDbType.Date, DBNull.Value, ParameterDirection.Output);
                }



                this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                response.P_COD_ERR = Convert.ToInt32(P_COD_ESTADO.Value.ToString());
                response.P_MESSAGE = P_MENSAJE.Value.ToString();
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 3;
                response.P_MESSAGE = ex.ToString();
                LogControl.save("insertarHistorial", ex.ToString(), "3");
            }
            return response;
        }
        public Entities.QuotationModel.ViewModel.QuotationResponseVM EndosoDet(QuotationCabBM request, QuotationDetBM request2, DbConnection connection, DbTransaction trx)
        {
            var response = new Entities.QuotationModel.ViewModel.QuotationResponseVM();
            var sPackageName = ProcedureName.pkg_Cotizacion + ".INSUPD_ENDOSO_DET_VL";
            List<OracleParameter> parameter = new List<OracleParameter>();
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, request2.P_NID_COTIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Varchar2, request2.P_NMODULEC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPREMIUM_MENSUAL", OracleDbType.Decimal, request2.P_NPREMIUM_MENSUAL, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NSUM_PREMIUMN", OracleDbType.Decimal, request2.P_NSUM_PREMIUMN, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NSUM_IGV", OracleDbType.Decimal, request2.P_NSUM_IGV, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NSUM_PREMIUM", OracleDbType.Decimal, request2.P_NSUM_PREMIUM, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMONTO_PLANILLA", OracleDbType.Decimal, request2.P_NMONTO_PLANILLA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Date, request.P_DSTARTDATE_ASE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, request.P_NUSERCODE, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, null, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, null, ParameterDirection.Output);

                P_MESSAGE.Size = 9000;

                parameter.Add(P_MESSAGE);
                parameter.Add(P_COD_ERR);

                parameter.Add(new OracleParameter("P_NAMO_AFEC", OracleDbType.Decimal, request2.P_NAMO_AFEC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIVA", OracleDbType.Decimal, request2.P_NIVA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NAMOUNT", OracleDbType.Decimal, request2.P_NAMOUNT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NNUM_TRABAJADORES_END", OracleDbType.Decimal, request2.P_NTOTAL_TRABAJADORES, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_RANGO", OracleDbType.Varchar2, request2.P_RANGO, ParameterDirection.Input)); // gcaa 26122023

                this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, connection, trx);
                response.P_MESSAGE = P_MESSAGE.Value.ToString();
                response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = ex.ToString();
                LogControl.save("EndosoDet", ex.ToString(), "3");
            }
            return response;
        }
        public Entities.QuotationModel.ViewModel.QuotationResponseVM AjusteEndoso(QuotationCabBM request, DbConnection connection, DbTransaction trx)
        {
            var lRamosAp = new string[] { ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch") };
            var response = new Entities.QuotationModel.ViewModel.QuotationResponseVM();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = lRamosAp.Contains(request.P_NBRANCH.ToString()) ? ProcedureName.sp_AjusteEndosoAP : ProcedureName.sp_AjusteEndoso;

            try
            {
                response.P_COD_ERR = 0;
                response.P_MESSAGE = String.Empty;
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request.CodigoProceso, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, request.NumeroCotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Date, request.P_DSTARTDATE_ASE, ParameterDirection.Input));

                if (!lRamosAp.Contains(request.P_NBRANCH.ToString()))
                {
                    parameter.Add(new OracleParameter("P_NTYPE_ENDOSO", OracleDbType.Int32, request.TipoEndoso, ParameterDirection.Input)); // INI ENDOSO TECNICA JTV 11042023
                }

                this.ExecuteByStoredProcedureVT_TRX(storeprocedure, parameter, connection, trx);
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = ex.ToString();
                LogControl.save("AjusteEndoso", ex.ToString(), "3");
            }
            return response;
        }
        public Entities.QuotationModel.ViewModel.QuotationResponseVM EndosoClose(QuotationCabBM request, DbConnection connection, DbTransaction trx, string srango_edad = null, int total = 0, int valor = 0)
        {
            var response = new Entities.QuotationModel.ViewModel.QuotationResponseVM();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.pkg_Cotizacion + ".INSUPD_ENDOSO_CLOSE";

            try
            {
                // INPUT ENDOSO TECNICA JTV 23122022
                response.P_COD_ERR = 0;
                response.P_MESSAGE = string.Empty;

                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, request.NumeroCotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Date, request.P_DSTARTDATE_ASE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, request.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_END", OracleDbType.Int32, request.TipoEndoso, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOMMENT", OracleDbType.Varchar2, request.P_SCOMMENT, ParameterDirection.Input)); //recotizacion
                parameter.Add(new OracleParameter("P_SRUTA", OracleDbType.Varchar2, request.P_SRUTA, ParameterDirection.Input)); //recotizacion

                //OUTPUT INI ENDOSO TECNICA JTV 23122022
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, null, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, null, ParameterDirection.Output);

                P_MESSAGE.Size = 9000;

                parameter.Add(P_MESSAGE);
                parameter.Add(P_COD_ERR); // FIN ENDOSO TECNICA JTV 23122022

                parameter.Add(new OracleParameter("P_NFLAG_RECOT", OracleDbType.Int32, request.recotizacion, ParameterDirection.Input)); //recotizacion endoso tecnica JTV 12012023
                parameter.Add(new OracleParameter("P_DEXPIRDAT_POL", OracleDbType.Date, request.P_DEXPIRDAT, ParameterDirection.Input)); // ENDOSO TECNICA JTV 20042023
                parameter.Add(new OracleParameter("P_RANGO", OracleDbType.Varchar2, srango_edad, ParameterDirection.Input)); // GCAA 28122023
                parameter.Add(new OracleParameter("P_TOTAL", OracleDbType.Varchar2, total, ParameterDirection.Input)); // GCAA 28122023
                parameter.Add(new OracleParameter("P_VALOR", OracleDbType.Varchar2, valor, ParameterDirection.Input)); // GCAA 28122023



                this.ExecuteByStoredProcedureVT_TRX(storeprocedure, parameter, connection, trx);

                response.P_MESSAGE = P_MESSAGE.Value.ToString(); // INI ENDOSO TECNICA JTV 23122022
                response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString()); // FIN ENDOSO TECNICA JTV 23122022
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = ex.ToString();
                LogControl.save("EndosoClose", ex.ToString(), "3"); // ENDOSO TECNICA JTV 23122022
            }
            return response;
        }

        public List<TypePolicy> getTypePolicy(string p_branch)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<TypePolicy> list = new List<TypePolicy>();

            string storedProcedureName = ProcedureName.pkg_ReaDataAP + ".REA_TYPE_PRODUCT";
            try
            {
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Varchar2, p_branch, ParameterDirection.Input));
                OracleParameter table = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(table);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                while (odr.Read())
                {
                    TypePolicy item = new TypePolicy();
                    item.id = Convert.ToInt32(odr["NTYPE_PRODUCT"].ToString());
                    item.descripcion = odr["SDESCRIPT"].ToString();

                    list.Add(item);

                }
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("getTypePolicy", ex.ToString(), "3");
            }
            return list;
        }

        public Entities.QuotationModel.ViewModel.QuotationResponseVM InsCargaMasSctr(QuotationCabBM data, DbConnection connection, DbTransaction trx)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            Entities.QuotationModel.ViewModel.QuotationResponseVM resultPackage = new Entities.QuotationModel.ViewModel.QuotationResponseVM();
            string storedProcedureName = ProcedureName.pkg_ValidadorGenPD + ".SP_INS_CARGA_MAS_SCTR";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NCERTYPE", OracleDbType.Varchar2, "2", ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.P_NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, data.P_NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Decimal, data.NumeroPoliza, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DSTARTDATE", OracleDbType.Date, Convert.ToDateTime(data.P_DSTARTDATE_ASE), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Varchar2, data.NumeroCotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, data.CodigoProceso, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_NERROR = new OracleParameter("P_NERROR", OracleDbType.Decimal, resultPackage.P_NERROR, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, resultPackage.P_SMESSAGE, ParameterDirection.Output);

                P_SMESSAGE.Size = 4000;
                parameter.Add(P_NERROR);
                parameter.Add(P_SMESSAGE);

                this.ExecuteByStoredProcedureVT_TRX(storedProcedureName, parameter, connection, trx);

                resultPackage.P_MESSAGE = P_SMESSAGE.Value.ToString();
                resultPackage.P_COD_ERR = Convert.ToInt32(P_NERROR.Value.ToString());
            }
            catch (Exception ex)
            {
                resultPackage.P_COD_ERR = 1;
                resultPackage.P_MESSAGE = ex.ToString();
                LogControl.save("InsCargaMasSctr", ex.ToString(), "3");
            }

            return resultPackage;
        }

        public Entities.QuotationModel.ViewModel.QuotationResponseVM InsDetApEndooso(QuotationCabBM data, DbConnection connection, DbTransaction trx)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            Entities.QuotationModel.ViewModel.QuotationResponseVM response = new Entities.QuotationModel.ViewModel.QuotationResponseVM();
            string storedProcedureName = ProcedureName.pkg_ValidadorGenPD + ".SP_INS_DET_AP_ENDOSO";
            try
            {
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, data.CodigoProceso, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_PRODUCT", OracleDbType.Int32, Convert.ToInt32(data.P_NTYPE_PRODUCT), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, data.planId, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_PROFILE", OracleDbType.Int32, Convert.ToInt32(data.P_NTYPE_PROFILE), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMODALITY", OracleDbType.Int32, Convert.ToInt32(data.P_NTYPE_MODALITY), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_EMPLOYEE", OracleDbType.Varchar2, "1", ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCERTYPE", OracleDbType.Char, "2", ParameterDirection.Input)); // jaime dice q va en 2
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.P_NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTIP_RENOV", OracleDbType.Int32, data.P_NTIP_RENOV, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPAYFREQ", OracleDbType.Int32, data.P_NPAYFREQ, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOLINVOT", OracleDbType.Int32, Convert.ToInt32(data.P_NTIPO_FACTURACION), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCIUU", OracleDbType.Int32, Convert.ToInt32(data.P_SCOD_ACTIVITY_TEC), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCURRENCY", OracleDbType.Int32, data.P_NCURRENCY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE_POL", OracleDbType.Varchar2, data.P_DSTARTDATE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEXPIRDAT_POL", OracleDbType.Varchar2, data.P_DEXPIRDAT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE_ASE", OracleDbType.Varchar2, data.P_DSTARTDATE_ASE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEXPIRDAT_ASE", OracleDbType.Varchar2, data.P_DEXPIRDAT_ASE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STYPE_TRANSAC", OracleDbType.Int32, 1, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, data.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, data.P_NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPREMIUM_PROP", OracleDbType.Varchar2, null, ParameterDirection.Input)); // hay que enviar la prima 
                parameter.Add(new OracleParameter("P_NFLAG_REC", OracleDbType.Int32, 1, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NFLAG_SA", OracleDbType.Int32, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.NumeroCotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Int32, 8, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SAPLICACION", OracleDbType.Varchar2, data.codAplicacion, ParameterDirection.Input));

                OracleParameter P_COD_ERR = new OracleParameter("P_ERROR", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                P_COD_ERR.Size = 9000;
                P_MESSAGE.Size = 9000;

                parameter.Add(P_COD_ERR);
                parameter.Add(P_MESSAGE);

                this.ExecuteByStoredProcedureVT_TRX(storedProcedureName, parameter, connection, trx);

                response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                response.P_MESSAGE = P_MESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = ex.ToString();
                LogControl.save("InsDetApEndooso", ex.ToString(), "3");
            }

            return response;
        }

        /// <summary>
        /// Importes Ajustados - SCTR - Reconocimiento de Ingresos - LNSR
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public SalidaAjustedAmounts AjustedAmounts(AjustedAmountsBM data)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            SalidaAjustedAmounts result = new SalidaAjustedAmounts();
            string storedProcedureName = ProcedureName.pkg_ValidadorGenPD + ".SP_GET_IMPORTES_AJUSTADOS_STRING";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.P_NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, data.P_NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NAMO_AFEC_INI", OracleDbType.Decimal, data.P_NAMO_AFEC_INI, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_NAMO_AFEC = new OracleParameter("P_NAMO_AFEC", OracleDbType.Varchar2, result.P_NAMO_AFEC, ParameterDirection.Output);
                OracleParameter P_NIVA = new OracleParameter("P_NIVA", OracleDbType.Varchar2, result.P_NIVA, ParameterDirection.Output);
                OracleParameter P_NAMOUNT = new OracleParameter("P_NAMOUNT", OracleDbType.Varchar2, result.P_NAMOUNT, ParameterDirection.Output);
                OracleParameter P_NDE = new OracleParameter("P_NDE", OracleDbType.Varchar2, result.P_NDE, ParameterDirection.Output);

                P_NAMO_AFEC.Size = 100;
                P_NIVA.Size = 100;
                P_NAMOUNT.Size = 100;
                P_NDE.Size = 100;

                parameter.Add(P_NAMO_AFEC);
                parameter.Add(P_NIVA);
                parameter.Add(P_NAMOUNT);
                parameter.Add(P_NDE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                result.P_NAMO_AFEC = P_NAMO_AFEC.Value.ToString() != null ? P_NAMO_AFEC.Value.ToString() : "0";
                result.P_NIVA = P_NIVA.Value.ToString() != null ? P_NIVA.Value.ToString() : "0";
                result.P_NAMOUNT = P_NAMOUNT.Value.ToString() != null ? P_NAMOUNT.Value.ToString() : "0";
                result.P_NDE = P_NDE.Value.ToString() != null ? P_NDE.Value.ToString() : "0";
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                result.P_COD_ERR = 1;
                result.P_MESSAGE = "Error en el servidor. " + ex.Message;
                LogControl.save("AjustedAmounts", ex.ToString(), "3");
            }

            return result;
        }

        #region codigo-comentado
        //public Entities.QuotationModel.ViewModel.QuotationResponseVM processPremiumDemand(PremiumDemandVM data)
        //{
        //    var response = new Entities.QuotationModel.ViewModel.QuotationResponseVM();

        //    #region Timer por 5 segundos
        //    var temp = new Stopwatch();
        //    temp.Start();
        //    while (temp.Elapsed < TimeSpan.FromSeconds(Double.Parse("60"))) { }
        //    temp.Stop();
        //    #endregion

        //    var resPremium = dataPremium(data);

        //    if (resPremium.Count > 0)
        //    {
        //        DbConnection DataConnection = ConnectionGet(enuTypeDataBase.OracleVTime);
        //        DbTransaction trx = null;

        //        try
        //        {
        //            DataConnection.Open();
        //            trx = DataConnection.BeginTransaction();
        //            response = loadPremium(resPremium[0], DataConnection, trx);
        //        }
        //        catch (Exception ex)
        //        {
        //            response.P_NCODE = 1;
        //            response.P_SMESSAGE = ex.ToString();
        //            ELog.save(this, ex.ToString());
        //        }
        //        finally
        //        {
        //            if (response.P_NCODE == 0)
        //            {
        //                trx.Commit();
        //            }
        //            else
        //            {
        //                if (trx != null) trx.Rollback();
        //            }

        //            trx.Dispose();
        //            ELog.CloseConnection(DataConnection);
        //        }
        //    }

        //    return response;
        //}

        //private quotationVM.QuotationResponseVM loadPremium(DataPremiumVM data, DbConnection connection, DbTransaction trx)
        //{
        //    var response = new quotationVM.QuotationResponseVM();
        //    List<OracleParameter> parameter = new List<OracleParameter>();
        //    string storeprocedure = ProcedureName.sp_cargaPremium;

        //    try
        //    {
        //        parameter.Add(new OracleParameter("pfecha", OracleDbType.Date, data.DDATE_PAYMENT, ParameterDirection.Input));
        //        parameter.Add(new OracleParameter("psbilltype", OracleDbType.Varchar2, data.SBILLTYPE, ParameterDirection.Input));
        //        parameter.Add(new OracleParameter("pninsur_area", OracleDbType.Int32, data.NINSUR_AREA, ParameterDirection.Input));
        //        parameter.Add(new OracleParameter("pnbillnum", OracleDbType.Int32, data.NBILLNUM, ParameterDirection.Input));
        //        parameter.Add(new OracleParameter("ptipocomp", OracleDbType.Varchar2, data.STYPE_PROFF, ParameterDirection.Input));

        //        this.ExecuteByStoredProcedureVT_TRX(storeprocedure, parameter, connection, trx);

        //        response.P_COD_ERR = 0;
        //        response.P_MESSAGE = null;
        //    }
        //    catch (Exception ex)
        //    {
        //        response.P_COD_ERR = 1;
        //        response.P_MESSAGE = ex.ToString();
        //        ELog.save(this, ex);
        //    }

        //    return response;
        //}

        //public List<DataPremiumVM> dataPremium(PremiumDemandVM data)
        //{
        //    List<OracleParameter> parameter = new List<OracleParameter>();
        //    List<DataPremiumVM> list = new List<DataPremiumVM>();

        //    string storedProcedureName = ProcedureName.sp_LeerPremium;
        //    try
        //    {
        //        parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, data.snid_proc, ParameterDirection.Input));
        //        OracleParameter table = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
        //        parameter.Add(table);

        //        OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

        //        while (odr.Read())
        //        {
        //            DataPremiumVM item = new DataPremiumVM();
        //            item.SBILLTYPE = odr["SBILLTYPE"].ToString();
        //            item.NINSUR_AREA = odr["NINSUR_AREA"].ToString();
        //            item.NBILLNUM = odr["NBILLNUM"].ToString();
        //            item.STYPE_PROFF = odr["STYPE_PROFF"].ToString();
        //            item.DDATE_PAYMENT = odr["DDATE_PAYMENT"].ToString();
        //            list.Add(item);
        //        }

        //        ELog.CloseConnection(odr);
        //    }
        //    catch (Exception ex)
        //    {
        //        ELog.save(this, ex.ToString());
        //    }
        //    return list;
        //}
        #endregion

        public List<ListaTipoRenovacion> GetListaTipoRenovacion()
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<ListaTipoRenovacion> lista = new List<ListaTipoRenovacion>();
            string[] tipRenovacion = { "3" };
            string storedProcedureName = ProcedureName.pkg_Generales + ".SP_LIST_TYPE_RENOVATION";
            try
            {
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, lista, ParameterDirection.Output);
                parameter.Add(C_TABLE);
                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);
                while (odr.Read())
                {
                    ListaTipoRenovacion listaTipoRenovacion = new ListaTipoRenovacion();
                    listaTipoRenovacion.ID = Convert.ToInt32(odr["STYPE_RENOVATION"]);
                    /*listaTipoRenovacion.SDESCRIPT = odr["SDESCRIPT"].ToString();*/
                    listaTipoRenovacion.DESCRIPCION = odr["SDESCRIPT_TYPE_RENOVATION"].ToString();
                    lista.Add(listaTipoRenovacion);
                }
                ELog.CloseConnection(odr);

                lista = lista.Where((n) => !(tipRenovacion.Contains(n.ID.ToString()))).ToList();
            }
            catch (Exception ex)
            {
                LogControl.save("GetListaTipoRenovacion", ex.ToString(), "3");
            }
            return lista;
        }
        //R.P.
        public SalidadPolizaEmit UpdateNLocalBroker(WSPlataforma.Entities.TransactModel.DepBroker BrkList, int nusercode)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseVM resultPackage = new ResponseVM();
            string storedProcedureName = ProcedureName.pkg_Cotizacion + ".UPD_BROKER_NLOCAL";
            var response = new SalidadPolizaEmit();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, BrkList.P_NID_COTIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCLIENT_COMER", OracleDbType.Varchar2, BrkList.P_SCLIENT_COMER, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLOCAL", OracleDbType.Int32, BrkList.P_NLOCAL, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, nusercode, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_COD_ERR = new OracleParameter("P_ERROR", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                P_COD_ERR.Size = 9000;
                P_MESSAGE.Size = 9000;

                parameter.Add(P_COD_ERR);
                parameter.Add(P_MESSAGE);


                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                response.P_MESSAGE = P_MESSAGE.Value.ToString();
                response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = "Error en el servidor. " + ex.Message;
                LogControl.save("UpdateNLocalBroker", ex.ToString(), "3");
            }

            return response;
        }

        public List<PolicyReceipt> GetNumRecibo(SavePolicyEmit response, string typerecibo)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<PolicyReceipt> result = new List<PolicyReceipt>();
            string storedProcedureName = ProcedureName.pkg_ReportesPD + ".GET_NUM_RECIBO";

            try
            {
                parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int32, response.P_NPOLICY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, response.P_NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, response.P_NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPERECEIPT", OracleDbType.Varchar2, typerecibo, ParameterDirection.Input));

                OracleParameter table = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(table);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                while (odr.Read())
                {
                    PolicyReceipt item = new PolicyReceipt();
                    item.nBranch = Convert.ToInt32(odr["NBRANCH"].ToString());
                    item.nProduct = Convert.ToInt32(odr["NPRODUCT"].ToString());
                    item.nReceipt = Convert.ToInt64(odr["NRECEIPT"].ToString());
                    item.dEffecDate = Convert.ToDateTime(odr["DEFFECDATE"].ToString()).ToString("dd/MM/yyyy");
                    item.dIssueDat = Convert.ToDateTime(odr["DISSUEDAT"].ToString()).ToString("dd/MM/yyyy");
                    item.nPremium = Convert.ToDecimal(odr["NPREMIUM"].ToString());

                    result.Add(item);

                }
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("GetNumRecibo", ex.ToString(), "3");
            }

            return result;
        }

        public DCPoliza.ResponseHeader getCotHeader(DCPoliza.RequestCuadroPolizaBM data)
        {
            var response = new DCPoliza.ResponseHeader();

            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        OracleDataReader dr;

                        cmd.Connection = cn;
                        cmd.CommandText = ProcedureName.sp_GetCodHeader;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int64, data.p_poliza, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_STRANSAC", OracleDbType.Varchar2, data.p_transaccion.Substring(0, 1), ParameterDirection.Input));

                        var P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, ParameterDirection.Output);
                        var P_MESSAGE = new OracleParameter("P_SERROR", OracleDbType.Varchar2, ParameterDirection.Output);
                        var P_NIDHEADER = new OracleParameter("P_NIDHEADER", OracleDbType.Int32, ParameterDirection.Output);

                        P_MESSAGE.Size = 4000;

                        cmd.Parameters.Add(P_COD_ERR);
                        cmd.Parameters.Add(P_MESSAGE);
                        cmd.Parameters.Add(P_NIDHEADER);

                        cn.Open();
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        response.cod_error = Convert.ToInt32(P_COD_ERR.Value.ToString());
                        response.mensaje = P_MESSAGE.Value.ToString();
                        response.cod_header = Convert.ToInt32(P_NIDHEADER.Value.ToString());

                        dr.Close();
                    }
                    catch (Exception ex)
                    {
                        response.cod_error = 1;
                        response.mensaje = ex.Message;
                        LogControl.save("getCotHeader", ex.ToString(), "3");
                    }
                    finally
                    {
                        if (cn.State == ConnectionState.Open) cn.Close();
                    }
                }
            }

            return response;
        }

        public Entities.QuotationModel.ViewModel.QuotationResponseVM ProcesarTramaEstado(QuotationCabBM request)
        {
            var response = new Entities.QuotationModel.ViewModel.QuotationResponseVM();
            DbConnection DataConnection = ConnectionGet(enuTypeDataBase.OracleVTime);
            DbTransaction trx = null;
            DataConnection.Open();
            trx = DataConnection.BeginTransaction();

            var FlagList = new List<Entities.QuotationModel.ViewModel.QuotationResponseVM>();

            try
            {
                List<HttpPostedFile> files = new List<HttpPostedFile>();
                foreach (var item in HttpContext.Current.Request.Files)
                {
                    HttpPostedFile arch = HttpContext.Current.Request.Files[item.ToString()];
                    files.Add((HttpPostedFile)HttpContext.Current.Request.Files[item.ToString()]);
                }

                string folderPath = files.Count > 0 ? ELog.obtainConfig("pathCotizacion") + request.NumeroCotizacion + "\\" + ELog.obtainConfig("movimiento") + "\\" + DateTime.Now.ToString("yyyyMMddhhmmss") + "\\" : String.Empty;
                request.P_SRUTA = folderPath;
                response.P_COD_ERR = 0;

                QuotationCabBM requestEC = new QuotationCabBM()
                {
                    NumeroCotizacion = request.NumeroCotizacion,
                    P_ESTADO = 5,
                    P_NUSERCODE = request.P_NUSERCODE,
                    P_SCOMMENT = request.P_SCOMMENT,
                    TrxCode = "EC",
                    P_DSTARTDATE = request.P_DSTARTDATE,
                    P_DEXPIRDAT = request.P_DEXPIRDAT,
                    P_NTIP_RENOV = Convert.ToInt32(request.P_NTIP_RENOV),
                    P_DSTARTDATE_ASE = request.P_DSTARTDATE_ASE,
                    P_DEXPIRDAT_ASE = request.P_DEXPIRDAT_ASE,
                    P_NPAYFREQ = request.P_NPAYFREQ,
                    P_SRUTA = request.P_SRUTA,
                    planId = request.planId
                };

                response = QuotationDA.ApproveQuotation(requestEC, DataConnection, trx); // inserta mov EC en cotiza_his

                if (request.QuotationDet != null)
                {
                    foreach (var item in request.QuotationDet)
                    {

                        item.P_NID_COTIZACION = Convert.ToInt32(request.NumeroCotizacion);
                        var lRamosSC = new string[] { ELog.obtainConfig("vidaLeyBranch") };

                        if (lRamosSC.Contains(request.P_NBRANCH.ToString()))
                        {
                            response = QuotationDA.InsertQuotationDet(request, item, DataConnection, trx);
                            FlagList.Add(response);
                        }

                    }
                }

                if (response.P_COD_ERR == 0 && request.P_NBRANCH == Convert.ToInt64(ELog.obtainConfig("vidaLeyBranch")))
                {
                    response = QuotationDA.UpdateQuotationDetPremium(request, request.NumeroCotizacion, DataConnection, trx);
                }

                if (response.P_COD_ERR == 0)
                {
                    if (request.P_NBRANCH == Convert.ToInt32(ELog.obtainConfig("vidaLeyBranch")))
                    {
                        response = QuotationDA.UpdateCodQuotation(request.NumeroCotizacion.Value, request.CodigoProceso, DataConnection, trx);
                    }
                    try
                    {
                        ValidaTramaBM trama = new ValidaTramaBM()
                        {
                            NID_COTIZACION = request.NumeroCotizacion.Value,
                            NTYPE_TRANSAC = "14",
                            NID_PROC = request.CodigoProceso
                        };
                        new QuotationDA().insertHistTrama(trama, DataConnection, trx);

                    }
                    catch (Exception ex)
                    {
                        response.P_COD_ERR = 1;
                        response.P_MESSAGE = ex.ToString();
                    }

                    var lRetro = new string[] { ELog.obtainConfig("vidaLeyBranch"), ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch") };
                    if (lRetro.Contains(request.P_NBRANCH.ToString()))
                    {
                        response = QuotationDA.ValRetroCotizacion(request, request.NumeroCotizacion.Value, DataConnection, trx);
                    }

                    response = QuotationDA.ValCotizacion(request.NumeroCotizacion.Value, request.P_NUSERCODE, 0, 0, DataConnection, trx);

                }

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

            }
            catch (Exception ex)
            {
                response = new quotationVM.QuotationResponseVM();
                response.P_COD_ERR = 1;
                response.P_MESSAGE = ex.ToString();
                LogControl.save("ProcesarTramaEstado", ex.ToString(), "3");
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

        public SalidadPolizaEmit regBIO(int nidcotizacion, string nrodoc, string nrocli, string algoritmo, string porcsimil, string urlImagen, string fechaeval, string tipoval, string codigoverif)
        {
            SalidadPolizaEmit response = new SalidadPolizaEmit();
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
                parameter.Add(new OracleParameter("P_PORC_SIMIL", OracleDbType.Double, porcsimil, ParameterDirection.Input));
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


        public SalidadPolizaEmit UpdateDatosPer(string sclient, string sproteg, DbConnection connection = null, DbTransaction trx = null)
        {
            var sPackageName = ProcedureName.pkg_ReaDataAP + ".UPD_SPROTEG_DATOS_IND";
            List<OracleParameter> parameter = new List<OracleParameter>();
            SalidadPolizaEmit result = new SalidadPolizaEmit();

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

        public ErrorCode InsertEmicionDetV2(dataQuotation data, int norder)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            var errorCode = new ErrorCode();
            string storedProcedureName = ProcedureName.pkg_TecnicaSCTR + ".INSERTEMIDET";

            try
            {
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Varchar2, data.NID_COTIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Varchar2, data.NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Varchar2, data.NMODULEC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NNUM_TRABAJADORES", OracleDbType.Varchar2, data.NNUM_TRABAJADORES, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMONTO_PLANILLA", OracleDbType.Int64, data.NMONTO_PLANILLA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTASA_CALCULADA", OracleDbType.Double, data.NTASA_CALCULADA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTASA_PROP", OracleDbType.Double, data.NTASA_PROP, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTASA_AUTOR", OracleDbType.Double, data.NTASA_AUTOR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPREMIUM_MIN", OracleDbType.Varchar2, data.NPREMIUM_MIN, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPREMIUM_MIN_PR", OracleDbType.Varchar2, data.NPREMIUM_MIN_PR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPREMIUM_MIN_AU", OracleDbType.Varchar2, data.NPREMIUM_MIN_AU, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPREMIUM_END", OracleDbType.Varchar2, data.NPREMIUM_END, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NSUM_PREMIUMN", OracleDbType.Varchar2, data.NSUM_PREMIUMN, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NSUM_IGV", OracleDbType.Double, data.NSUM_IGV, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NSUM_PREMIUM", OracleDbType.Double, data.NSUM_PREMIUM, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int64, data.NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NRATE", OracleDbType.Double, data.NRATE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NDISCOUNT", OracleDbType.Varchar2, data.NDISCOUNT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NACTIVITYVARIATION", OracleDbType.Varchar2, data.NACTIVITYVARIATION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SSTATREGT", OracleDbType.Varchar2, data.SSTATREGT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMODULEC_FINAL", OracleDbType.Varchar2, data.NMODULEC_FINAL, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NAMO_AFEC", OracleDbType.Varchar2, data.NAMO_AFEC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIVA", OracleDbType.Varchar2, data.NIVA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NAMOUNT", OracleDbType.Varchar2, data.NAMOUNT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NDE", OracleDbType.Varchar2, data.NDE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FRECUENCIA_PAGO", OracleDbType.Int64, data.FRECUENCIA_PAGO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_ORDER", OracleDbType.Int64, norder, ParameterDirection.Input));

                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, errorCode.P_COD_ERR, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, 9000, errorCode.P_MESSAGE, ParameterDirection.Output);

                P_COD_ERR.Size = 9000;
                P_MESSAGE.Size = 9000;

                parameter.Add(P_COD_ERR);
                parameter.Add(P_MESSAGE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                errorCode.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                errorCode.P_MESSAGE = Convert.ToString(P_MESSAGE.Value.ToString());
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                errorCode.P_COD_ERR = 1;
                errorCode.P_MESSAGE = ex.ToString();
                LogControl.save("InsertEmicionDetV2", ex.ToString(), "3");
            }

            return errorCode;
        }

        public int GetOrderDet(string cotizacion)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            var errorCode = new ErrorCode();
            int norder = 0;
            string storedProcedureName = ProcedureName.pkg_TecnicaSCTR + ".GET_ORDER_DET";

            try
            {
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Varchar2, cotizacion, ParameterDirection.Input));

                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, errorCode.P_COD_ERR, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, 9000, errorCode.P_MESSAGE, ParameterDirection.Output);
                OracleParameter P_ORDER = new OracleParameter("P_ORDER", OracleDbType.Int32, errorCode.P_ORDER, ParameterDirection.Output);

                P_COD_ERR.Size = 9000;
                P_MESSAGE.Size = 9000;
                P_ORDER.Size = 9000;

                parameter.Add(P_COD_ERR);
                parameter.Add(P_MESSAGE);
                parameter.Add(P_ORDER);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                errorCode.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                errorCode.P_MESSAGE = Convert.ToString(P_MESSAGE.Value.ToString());
                norder = Convert.ToInt32(P_ORDER.Value.ToString());

                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                errorCode.P_COD_ERR = 1;
                errorCode.P_MESSAGE = ex.ToString();
                LogControl.save("InsertEmicionDetV2", ex.ToString(), "3");
            }

            return norder;
        }

        public ErrorCode DeleteDataSCTR(delSCTR data)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            var errorCode = new ErrorCode();
            string storedProcedureName = ProcedureName.pkg_TecnicaSCTR + ".DELETE_ERRORSCTR";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, errorCode.P_COD_ERR, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, 9000, errorCode.P_MESSAGE, ParameterDirection.Output);

                parameter.Add(P_COD_ERR);
                parameter.Add(P_MESSAGE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);
                errorCode.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                errorCode.P_MESSAGE = P_MESSAGE.Value.ToString();

                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                errorCode.P_COD_ERR = 1;
                errorCode.P_MESSAGE = ex.ToString();
                LogControl.save("DeleteDataSCTR", ex.ToString(), "3");
            }

            return errorCode;
        }
        public MonthsSCTRReturn GetMonthsSCTR(monthsSCTR data)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            var months = new MonthsSCTRReturn();
            string storedProcedureName = ProcedureName.pkg_Poliza + ".PD_MONTHS_SCTR";
            try
            {

                //INPUT
                parameter.Add(new OracleParameter("P_DSTARTDATE", OracleDbType.Varchar2, data.date, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Varchar2, data.dateFn, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPAYFREQ", OracleDbType.Int32, data.npayfreq, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_RESULT = new OracleParameter("P_RESULT", OracleDbType.Varchar2, 4000, months.P_RESULT, ParameterDirection.Output);
                parameter.Add(P_RESULT);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);
                months.P_RESULT = P_RESULT.Value.ToString();

                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("GetMonthsSCTR", ex.ToString(), "3");
            }

            return months;
        }

        public CommissionVLVM getCommissionVL(CommissionVLBM data)
        {
            CommissionVLVM commissionVLVM = new CommissionVLVM();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storedProcedureName = ProcedureName.sp_LeerComisionTramite;
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.NID_COTIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, data.NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_TRAMITE", OracleDbType.Int32, data.NID_TRAMITE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Int32, data.NTYPE_TRANSAC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTIP_NCOMMISSION", OracleDbType.Int32, data.NTIP_NCOMMISSION, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_COMMISIONVL = new OracleParameter("P_SCOMMISSION_VL", OracleDbType.Varchar2, 4000, commissionVLVM.commisionVL, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, 4000, commissionVLVM.P_MESSAGE, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, commissionVLVM.P_COD_ERR, ParameterDirection.Output);
                parameter.Add(P_COMMISIONVL);
                parameter.Add(P_MESSAGE);
                parameter.Add(P_COD_ERR);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);
                commissionVLVM.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                commissionVLVM.P_MESSAGE = P_MESSAGE.Value.ToString();
                commissionVLVM.commisionVL = P_COMMISIONVL.Value.ToString();

                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                commissionVLVM.P_COD_ERR = 1;
                commissionVLVM.P_MESSAGE = "Erro al obtener la comision";
                LogControl.save("getCommissionVL", ex.ToString(), "3");
            }

            return commissionVLVM;
        }

        public ValBrokerModel valBrokerVCF(ValBrokerModel data)
        {

            List<OracleParameter> parameter = new List<OracleParameter>();
            ValBrokerModel response = new ValBrokerModel();
            string storedProcedureName = "INSUDB.VALIDATE_BROKER_VCF";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, data.SCLIENT, ParameterDirection.Input));
                //parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, policyEmit.P_NID_COTIZACION, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, 4000, response.P_SMESSAGE, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, response.COD_ERROR, ParameterDirection.Output);

                parameter.Add(P_MESSAGE);
                parameter.Add(P_COD_ERR);


                this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                response.COD_ERROR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                response.P_SMESSAGE = P_MESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                response.COD_ERROR = 1;
                response.P_SMESSAGE = ex.ToString();
                LogControl.save("SavePolizaEmit", ex.ToString(), "3");
            }

            return response;

        }

        public PolizaEmitCAB getExpirAseg(string nroCotizacion)
        {

            GenericPackageVM genericPackage = new GenericPackageVM();
            List<OracleParameter> parameter = new List<OracleParameter>();
            var response = new PolizaEmitCAB();

            string storedProcedureName = "INSUDB.GETEXPIRASEG";

            try
            {
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Varchar2, nroCotizacion, ParameterDirection.Input));
                OracleParameter P_EXPIRACION_ASEGURADOS = new OracleParameter("P_EXPIRACION_ASEGURADOS", OracleDbType.Varchar2, 4000, response.EXPIRACION_ASEGURADOS, ParameterDirection.Output);

                parameter.Add(P_EXPIRACION_ASEGURADOS);
                this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                response.EXPIRACION_ASEGURADOS = P_EXPIRACION_ASEGURADOS.Value.ToString();
            }
            catch (Exception ex)
            {
                response.EXPIRACION_ASEGURADOS = "";
                LogControl.save("SavePolizaEmit", ex.ToString(), "3");
            }

            return response;
        }

        public ValPolicy GetValFact(ValPolicy data)
        {

            ValPolicy response = new ValPolicy();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storedProcedureName = ProcedureName.pkg_ValidaReglas + ".SPS_VAL_FACT";

            try
            {
                parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Varchar2, data.P_NPOLICY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Varchar2, data.P_NBRANCH, ParameterDirection.Input));

                OracleParameter P_VAL = new OracleParameter("P_VAL", OracleDbType.Int32, response.P_VAL, ParameterDirection.Output);

                parameter.Add(P_VAL);
                this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                response.P_VAL = Convert.ToInt32(P_VAL.Value.ToString());
            }
            catch (Exception ex)
            {
                LogControl.save("GetValFact", ex.ToString(), "3");
            }

            return response;
        }

        public ResponseVM relanzarDocumento(RelanzarDocumentoVM data)
        {
            var response = new ResponseVM();

            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        OracleDataReader dr;

                        cmd.Connection = cn;
                        cmd.CommandText = ProcedureName.sp_RelanzarDocumentos;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int64, data.npolicy, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.nbranch, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, data.nproduct, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NIDHEADERPROC", OracleDbType.Int64, data.nidheaderproc, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, data.suser, ParameterDirection.Input));

                        // OUTPUT
                        OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, response.P_NCODE, ParameterDirection.Output);
                        OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, response.P_SMESSAGE, ParameterDirection.Output);

                        cmd.Parameters.Add(P_NCODE);
                        cmd.Parameters.Add(P_SMESSAGE);

                        cn.Open();
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        response.P_NCODE = P_NCODE.Value.ToString();
                        response.P_SMESSAGE = P_SMESSAGE.Value.ToString();

                        ELog.CloseConnection(dr);
                    }
                    catch (Exception ex)
                    {
                        response.P_NCODE = "1";
                        response.P_SMESSAGE = ex.Message;
                        LogControl.save("relanzarDocumento", ex.ToString(), "3");
                    }
                    finally
                    {
                        if (cn.State == ConnectionState.Open) cn.Close();
                    }
                }

            }

            return response;
        }

        public async Task<string> invocarServicio_EPS_TRA(int cotizacion, string json, string transaccion) //AVS - PRY INTERCONEXION SABSA 20/07/2023
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
                    //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    var responseToken = await client.PostAsync(ELog.obtainConfig("urlTokenEPS_SCTR"), content);
                    mToken = await responseToken.Content.ReadAsStringAsync();

                    if (string.IsNullOrEmpty(mToken))
                    {
                        LogControl.save("invocarServicio_EPS - API EMI", mToken, "3");
                        new QuotationDA().InsertLog(cotizacion, "02 - ERROR TOKEN EPS - " + transaccion, ELog.obtainConfig("urlTokenEPS_SCTR"), responseToken + Environment.NewLine + Environment.NewLine + "No se genero Token del servicio EPS", null);
                        LogControl.save("generarTransaccionEPS", "{\"cotizacion\": \"" + cotizacion.ToString() + ", \"mensaje\":  \"02 - Error en Generacion de Token - Transaccion de Poliza\", \"url\": " +
                        ELog.obtainConfig("urlTokenEPS_SCTR") + ", \"json\": \"No se genero Token del servicio EPS\" }", "2", "EPS");
                        throw new Exception(ELog.obtainConfig("errorInvocarServicio_EPS"));
                    }

                    var QuotEPSToken = JsonConvert.DeserializeObject<quotationVM.AuthEPSResult>(mToken);

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", QuotEPSToken.token);
                    var response = await client.PostAsync(ELog.obtainConfig("urlEmisionEPS_SCTR"), new StringContent(json, Encoding.UTF8, "application/json"));
                    sQuoteResult = await response.Content.ReadAsStringAsync();
                    var responseObject = JsonConvert.DeserializeObject<dynamic>(sQuoteResult);
                    bool isSuccess = responseObject == null ? false : responseObject.success;

                    if (isSuccess)
                    {
                        new QuotationDA().InsertLog(cotizacion, "02 - RESPUESTAS EPS - " + transaccion, ELog.obtainConfig("urlEmisionEPS_SCTR"), sQuoteResult, null);
                        LogControl.save("generarTransaccionEPS", "{\"cotizacion\": \"" + cotizacion.ToString() + ", \"mensaje\":  \"02 - Se recibe respuesta de EPS - Transaccion de Poliza\", \"url\": " +
                        ELog.obtainConfig("urlTokenEPS_SCTR") + ", \"json\":" + sQuoteResult + " }", "2", "EPS");

                    }
                    else
                    {
                        string errorMessage = responseObject.message;
                        LogControl.save("invocarServicio_EPS - API EMI", errorMessage, "3");
                        throw new Exception(errorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                LogControl.save("invocarServicio_EPS - API EMI", ex.ToString(), "3");
                new QuotationDA().InsertLog(cotizacion, "02 - ERROR SERVICIO EPS - " + transaccion, ELog.obtainConfig("urlEmisionEPS_SCTR"),
                json + Environment.NewLine + ex.ToString(), null);
            }

            return await Task.FromResult(sQuoteResult);
        }

        public string GetCodigoCip(string cod_cotizacion)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storedProcedureName = ProcedureName.pkg_EPS + ".SP_GET_COD_CIP";
            string soperationNumber = null;

            try
            {
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, cod_cotizacion, ParameterDirection.Input));

                OracleParameter table = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(table);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                while (odr.Read())
                {
                    soperationNumber = odr["SOPERATIONNUMBER"].ToString();
                }
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("GetCodigoCip", ex.ToString(), "3");
            }

            return soperationNumber;
        }

        public List<riesgos> GetRiesgos(string cod_cotizacion)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<riesgos> resultPackage = new List<riesgos>();

            string storedProcedureName = ProcedureName.pkg_EPS + ".SP_GET_RIESGOS";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Varchar2, cod_cotizacion, ParameterDirection.Input));

                OracleParameter table = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(table);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                while (odr.Read())
                {
                    riesgos item = new riesgos();
                    item.codigoProducto = odr["NPRODUCT"] == DBNull.Value ? "" : odr["NPRODUCT"].ToString();
                    item.codigoPlan = odr["NMODULEC"] == DBNull.Value ? "" : odr["NMODULEC"].ToString();
                    item.codigoCategoria = odr["NMODULEC"] == DBNull.Value ? "" : odr["NMODULEC"].ToString();
                    item.cantidadTrabajador = odr["NNUM_TRABAJADORES"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NNUM_TRABAJADORES"].ToString());
                    item.planillaTotal = odr["NSUM_PREMIUM"] == DBNull.Value ? 0 : Convert.ToDecimal(odr["NSUM_PREMIUM"].ToString());
                    item.tasaAutorizada = odr["NTASA_AUTOR"] == DBNull.Value ? 0 : Convert.ToDecimal(odr["NTASA_AUTOR"].ToString());
                    resultPackage.Add(item);
                }
                resultPackage = resultPackage.Where(item => item.cantidadTrabajador != 0).ToList();

                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                LogControl.save("GetRiesgos", ex.ToString(), "3");
            }

            return resultPackage;
        }

        public List<asegurados> GetAsegurados(string nid_proc)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<asegurados> resultPackage = new List<asegurados>();

            string storedProcedureName = ProcedureName.pkg_EPS + ".SP_GET_ASEGURADOS";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_PROC_EPS", OracleDbType.Varchar2, nid_proc, ParameterDirection.Input));

                OracleParameter table = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(table);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                while (odr.Read())
                {
                    asegurados item = new asegurados();
                    item.nombres = odr["SFIRSTNAME"] == DBNull.Value ? "" : odr["SFIRSTNAME"].ToString();
                    item.apellidoPaterno = odr["SLASTNAME"] == DBNull.Value ? "" : odr["SLASTNAME"].ToString();
                    item.apellidoMaterno = odr["SLASTNAME2"] == DBNull.Value ? "" : odr["SLASTNAME2"].ToString();
                    item.codigoPlan = odr["NMODULEC"] == DBNull.Value ? "" : odr["NMODULEC"].ToString();
                    item.codigoTipoDocumento = odr["NIDDOC_TYPE"] == DBNull.Value ? "" : odr["NIDDOC_TYPE"].ToString();
                    item.numeroDocumento = odr["SIDDOC"] == DBNull.Value ? "" : odr["SIDDOC"].ToString();
                    item.fechaNacimiento = odr["DBIRTHDAT"] == DBNull.Value ? "" : DateTime.Parse(odr["DBIRTHDAT"].ToString()).ToString("yyyy-MM-dd");
                    item.remuneracion = odr["NREMUNERACION"] == DBNull.Value ? 0 : Convert.ToDecimal(odr["NREMUNERACION"]);
                    item.codigoUnicoCliente = odr["SCLIENT"] == DBNull.Value ? "" : odr["SCLIENT"].ToString();
                    item.primaCobrada = odr["NPREMIUMN"] == DBNull.Value ? 0 : Convert.ToDecimal(odr["NPREMIUMN"]);
                    item.igvCobrado = odr["NIGV"] == DBNull.Value ? 0 : Convert.ToDecimal(odr["NIGV"]);
                    item.derechoEmisionCobrado = odr["NDE"] == DBNull.Value ? 0 : Convert.ToDecimal(odr["NDE"]);
                    item.primaBrutaCobrada = odr["NPREMIUM"] == DBNull.Value ? 0 : Convert.ToDecimal(odr["NPREMIUM"]);
                    item.codigoGenero = odr["SSEXCLIEN"] == DBNull.Value ? "" : odr["SSEXCLIEN"].ToString();
                    item.correoElectronico = odr["SE_MAIL"] == DBNull.Value ? "" : odr["SE_MAIL"].ToString();
                    item.codigoTipoTelefono = odr["NPHONE_TYPE"] == DBNull.Value ? "" : odr["NPHONE_TYPE"].ToString();
                    item.telefono = odr["SPHONE"] == DBNull.Value ? "" : odr["SPHONE"].ToString();
                    resultPackage.Add(item);
                }
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                LogControl.save("GetAsegurados", ex.ToString(), "3");
            }

            return resultPackage;
        }


        public ErrorCode Udp_quotation_EPS(instPay_SCTR data)
        {
            var respons = new ErrorCode();
            var sPackageName = ProcedureName.pkg_EPS + ".SP_UDP_QUOTATION_EPS";
            List<OracleParameter> parameters = new List<OracleParameter>();
            try
            {
                parameters.Add(new OracleParameter("P_NID_PROC_EPS", OracleDbType.Varchar2, data.P_NID_PROC_EPS, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NID_PROC_SCTR", OracleDbType.Varchar2, data.P_NID_PROC_SCTR, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Varchar2, data.P_NID_COTIZACION, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Varchar2, data.P_NTYPE_TRANSAC, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_MENSAJE_TR", OracleDbType.Varchar2, data.P_MENSAJE_TR, ParameterDirection.Input));

                // OUTPUT
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, 500, respons.P_MESSAGE, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, respons.P_COD_ERR, ParameterDirection.Output);
                parameters.Add(P_MESSAGE);
                parameters.Add(P_COD_ERR);

                this.ExecuteByStoredProcedureVT(sPackageName, parameters);

                respons.P_COD_ERR = 0;
                respons.P_MESSAGE = "Se insertó correctamente el registro";
            }
            catch (Exception ex)
            {
                respons.P_COD_ERR = 1;
                respons.P_MESSAGE = ex.ToString();
                LogControl.save("InsertQuotation_EPS", ex.ToString(), "3");
            }

            return respons;
        }

        public poliza_eps_sctr GetNpolicy_EPS(List<SavePolicyEmit> requestProtecta)
        {
            var response = new poliza_eps_sctr();
            var sPackageName = ProcedureName.pkg_EPS + ".SP_GEN_NPOLICY_EPS";
            List<OracleParameter> parameters = new List<OracleParameter>();

            try
            {

                parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, requestProtecta[0].P_NID_COTIZACION, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, requestProtecta[0].P_NBRANCH, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, requestProtecta[0].P_NPRODUCT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCOT_MIXTA", OracleDbType.Int32, requestProtecta[0].P_NCOT_MIXTA, ParameterDirection.Input));

                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int64, response.P_NCODE, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, 500, response.P_MESSAGE, ParameterDirection.Output);
                OracleParameter P_NPOLICY = new OracleParameter("P_NPOLICY", OracleDbType.Int64, response.P_NPOLICY, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_MESSAGE);
                parameters.Add(P_NPOLICY);

                this.ExecuteByStoredProcedureVT(sPackageName, parameters);

                response.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                response.P_MESSAGE = P_MESSAGE.Value.ToString();
                response.P_NPOLICY = Convert.ToInt64(P_NPOLICY.Value.ToString());
            }
            catch (Exception ex)
            {
                response.P_NCODE = 1;
                response.P_MESSAGE = ex.ToString();
                response.P_NPOLICY = 0;
                LogControl.save("GetNpolicy_EPS", ex.ToString(), "3");
            }

            return response;
        }

        public int insert_policy_EPS(long poliza_sctr, long poliza_eps, string nid_proc_eps, string mensaje)
        {
            var sPackageName = ProcedureName.pkg_EPS + ".SP_UDP_POLIZA_EPS";
            int result = 0;
            List<OracleParameter> parameters = new List<OracleParameter>();

            try
            {
                parameters.Add(new OracleParameter("P_NPOLICY_SCTR", OracleDbType.Int64, poliza_sctr, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPOLICY_EPS", OracleDbType.Int64, poliza_eps, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NID_PROC_EPS", OracleDbType.Varchar2, nid_proc_eps, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, mensaje, ParameterDirection.Input));

                OracleParameter P_COD_ERR = new OracleParameter("P_NCODE", OracleDbType.Int64, result, ParameterDirection.Output);

                P_COD_ERR.Size = 200;

                parameters.Add(P_COD_ERR);

                this.ExecuteByStoredProcedureVT(sPackageName, parameters);

                result = Convert.ToInt32(P_COD_ERR.Value.ToString());
            }
            catch (Exception ex)
            {
                result = 1;
                LogControl.save("insert_policy_EPS", ex.ToString(), "3");

                parameters.Clear();
            }
            return result;
        }

        public int Udp_cip_EPS(string id_sctr, string id_eps, string mensaje)
        {
            var sPackageName = ProcedureName.pkg_EPS + ".SP_UDP_CIP_EPS";
            int result = 0;
            List<OracleParameter> parameters = new List<OracleParameter>();

            try
            {
                parameters.Add(new OracleParameter("P_ID_SCTR", OracleDbType.Varchar2, id_sctr, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_ID_EPS", OracleDbType.Varchar2, id_eps, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, mensaje, ParameterDirection.Input));

                OracleParameter P_COD_ERR = new OracleParameter("P_NCODE", OracleDbType.Int64, result, ParameterDirection.Output);

                P_COD_ERR.Size = 200;

                parameters.Add(P_COD_ERR);

                this.ExecuteByStoredProcedureVT(sPackageName, parameters);

                result = Convert.ToInt32(P_COD_ERR.Value.ToString());
            }
            catch (Exception ex)
            {
                result = 1;
                LogControl.save("Udp_cip_EPS", ex.ToString(), "3");
            }
            return result;
        }

        public poliza_eps_sctr Ins_Documentos_EPS(Ins_Documentacion_EPS insDocEPSList)
        {
            var sPackageName = ProcedureName.pkg_EPS + ".SP_INS_DOCUMENTO_EPS";
            var response = new poliza_eps_sctr();
            List<OracleParameter> parameters = new List<OracleParameter>();

            try
            {
                parameters.Add(new OracleParameter("P_NID_PROC_EPS", OracleDbType.Varchar2, insDocEPSList.P_NID_PROC_EPS, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int64, insDocEPSList.P_NBRANCH, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int64, insDocEPSList.P_NPRODUCT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int64, insDocEPSList.P_NID_COTIZACION, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int64, insDocEPSList.P_NPOLICY, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_ID_DOCUMENTO", OracleDbType.Int64, insDocEPSList.P_ID_DOCUMENTO, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DES_DOCUMENTO", OracleDbType.Varchar2, insDocEPSList.P_DES_DOCUMENTO, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NSTATE", OracleDbType.Int64, insDocEPSList.P_NSTATE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_MENSAJE_IMP", OracleDbType.Varchar2, insDocEPSList.P_MENSAJE_IMP, ParameterDirection.Input));

                OracleParameter P_COD_ERR = new OracleParameter("P_NCODE", OracleDbType.Int64, response.P_NCODE, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, response.P_MESSAGE, ParameterDirection.Output);

                P_COD_ERR.Size = 200;
                P_MESSAGE.Size = 4000;

                parameters.Add(P_COD_ERR);
                parameters.Add(P_MESSAGE);

                this.ExecuteByStoredProcedureVT(sPackageName, parameters);

                response.P_NCODE = Convert.ToInt32(P_COD_ERR.Value.ToString());
                response.P_MESSAGE = P_MESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                response.P_NCODE = 1;
                response.P_MESSAGE = ex.ToString();
                LogControl.save("Ins_Documentos_EPS", ex.ToString(), "3");
            }
            return response;
        }

        public poliza_eps_sctr Ins_AseguradosEPS(CargaAsegEPS request_ins_aseg, string tipo_transaccion)
        {
            var sPackageName = ProcedureName.pkg_EPS + ".SP_TRAS_CARGA_MAS_EPS";
            var response = new poliza_eps_sctr();
            List<OracleParameter> parameters = new List<OracleParameter>();

            try
            {
                parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, request_ins_aseg.P_NID_COTIZACION, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, request_ins_aseg.P_NID_PROC, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, request_ins_aseg.P_NUSERCODE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPAYFREQ", OracleDbType.Int32, request_ins_aseg.P_NPAYFREQ, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, request_ins_aseg.P_NBRANCH, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, request_ins_aseg.P_NPRODUCTO, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DSTARTDATE", OracleDbType.Varchar2, request_ins_aseg.P_DSTARTDATE_POL, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Varchar2, request_ins_aseg.P_DEXPIRDAT_POL, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_FACT_MES_VENCIDO", OracleDbType.Varchar2, 0, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Varchar2, tipo_transaccion, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DSTARTDATE_ASE", OracleDbType.Varchar2, request_ins_aseg.P_DSTARTDATE_ASE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DEXPIRDAT_ASE", OracleDbType.Varchar2, request_ins_aseg.P_DEXPIRDAT_ASE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NID_PROC_EPS", OracleDbType.Varchar2, request_ins_aseg.P_NID_PROC_EPS, ParameterDirection.Input));

                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, response.P_NCODE, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, response.P_MESSAGE, ParameterDirection.Output);

                P_COD_ERR.Size = 200;
                P_MESSAGE.Size = 4000;

                parameters.Add(P_COD_ERR);
                parameters.Add(P_MESSAGE);

                this.ExecuteByStoredProcedureVT(sPackageName, parameters);

                response.P_NCODE = Convert.ToInt32(P_COD_ERR.Value.ToString());
                response.P_MESSAGE = P_MESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                response.P_NCODE = 1;
                response.P_MESSAGE = ex.ToString();
                LogControl.save("Ins_Documentos_EPS", ex.ToString(), "3");
            }
            return response;
        }

        public async Task<ValAgencySCTR> ValidateAgencySCTR(ValidateAgencySCTR request)
        {
            var response = new ValAgencySCTR();
            var sPackageName = ProcedureName.pkg_EPS + ".SP_VALIDATE_AGENCY";
            List<OracleParameter> parameters = new List<OracleParameter>();
            try
            {
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, request.nbranch, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int64, request.ncotizacion, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCTO", OracleDbType.Int32, request.nproduct, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCOTMIXTA", OracleDbType.Int32, request.nflagMixto, ParameterDirection.Input));

                // OUTPUT
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, response.P_COD_ERR, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, 500, response.P_MESSAGE, ParameterDirection.Output);
                OracleParameter P_COD_RUC = new OracleParameter("P_COD_RUC", OracleDbType.Varchar2, 500, response.P_COD_RUC, ParameterDirection.Output);
                OracleParameter P_NOM_RUC = new OracleParameter("P_NOM_RUC", OracleDbType.Varchar2, 500, response.P_NOM_RUC, ParameterDirection.Output);
                parameters.Add(P_COD_ERR);
                parameters.Add(P_MESSAGE);
                parameters.Add(P_COD_RUC);
                parameters.Add(P_NOM_RUC);

                this.ExecuteByStoredProcedureVT(sPackageName, parameters);

                response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                response.P_MESSAGE = P_MESSAGE.Value.ToString();
                response.P_COD_RUC = P_COD_RUC.Value.ToString();
                response.P_NOM_RUC = P_NOM_RUC.Value.ToString();
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = ex.ToString();
                LogControl.save("val_agency_sctr", ex.ToString(), "3");
            }

            return await Task.FromResult(response);
        }

        public ErrorCode GetPayMethodsTypeValidate(string riesgo, string prima, string typeMovement)
        {
            var response = new ErrorCode();
            var sPackageName = ProcedureName.pkg_EPS + ".SP_VALIDATE_METHODS_PAYMENT";
            List<OracleParameter> parameters = new List<OracleParameter>();
            var primaDecimal = Convert.ToDecimal(prima);
            try
            {

                parameters.Add(new OracleParameter("P_NPRIMA", OracleDbType.Decimal, primaDecimal, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DES_RIESGO", OracleDbType.Varchar2, riesgo, ParameterDirection.Input));

                // OUTPUT
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                OracleParameter P_PAY_TYPE = new OracleParameter("P_PAY_TYPE", OracleDbType.Int32, ParameterDirection.Output);

                P_COD_ERR.Size = 200;
                P_MESSAGE.Size = 4000;
                P_PAY_TYPE.Size = 200;

                parameters.Add(P_COD_ERR);
                parameters.Add(P_MESSAGE);
                parameters.Add(P_PAY_TYPE);

                parameters.Add(new OracleParameter("P_NTYPE_MOVEMENT", OracleDbType.Int32, typeMovement, ParameterDirection.Input));

                this.ExecuteByStoredProcedureVT(sPackageName, parameters);

                response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                response.P_MESSAGE = P_MESSAGE.Value.ToString();
                response.P_ORDER = Convert.ToInt32(P_PAY_TYPE.Value.ToString());
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = ex.ToString();
                LogControl.save("GetPayMethodsTypeValidate", ex.ToString(), "3");
            }

            return response;
        }

        // ACTUALIZAR ESTADO PAGADO EPS - DGC - 14/11/2023
        public Task<RespuestaVM> ActualizarEstadoPagadoEPS(ActualizarEstadoPagadoEPSFilter data)
        {
            var parameters = new List<OracleParameter>();
            RespuestaVM entities = new RespuestaVM();
            var procedure = "PKG_PV_TRAT_EPS.SP_ACT_ESTADO_PAY_EPS";
            try
            {
                parameters.Add(new OracleParameter("NID_PROC", OracleDbType.NVarchar2, data.NID_PROC, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SBILLTYPE", OracleDbType.NVarchar2, data.SBILLTYPE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NINSUR_AREA", OracleDbType.Int64, data.NINSUR_AREA, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NBILLNUM", OracleDbType.Int64, data.NBILLNUM, ParameterDirection.Input));
                parameters.Add(new OracleParameter("DFECPAGO", OracleDbType.Date, data.DFECPAGO, ParameterDirection.Input));
                OracleParameter NCODE = new OracleParameter("NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter MESSAGE = new OracleParameter("MESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(NCODE);
                parameters.Add(MESSAGE);
                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);
                entities.NCODE = Convert.ToInt32(NCODE.Value.ToString());
                entities.MESSAGE = MESSAGE.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Task.FromResult<RespuestaVM>(entities);
        }

        public ErrorCode UdpJsonCIP(string sjson, int cotizacion, string nid_proc)
        {
            var response = new ErrorCode();
            var sPackageName = ProcedureName.pkg_EPS + ".SP_UDP_CIP_CLOB";
            List<OracleParameter> parameters = new List<OracleParameter>();
            try
            {

                parameters.Add(new OracleParameter("P_SJSON_CIP", OracleDbType.Clob, sjson, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, cotizacion, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NID_PROC_EPS", OracleDbType.Varchar2, nid_proc, ParameterDirection.Input));

                // OUTPUT
                OracleParameter P_COD_ERR = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                P_COD_ERR.Size = 200;
                P_MESSAGE.Size = 4000;

                parameters.Add(P_COD_ERR);
                parameters.Add(P_MESSAGE);


                this.ExecuteByStoredProcedureVT(sPackageName, parameters);

                response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                response.P_MESSAGE = P_MESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = ex.ToString();
                LogControl.save("UdpJsonCIP", ex.ToString(), "3");
            }

            return response;
        }

        public string GetPolicyTransListExcel(TransPolicySearchBM data)
        {
            var resTrans = this.GetPolicyTransList(data);
            var lista = (List<PolicyTransVM>)resTrans.C_TABLE;

            string templatePath = "D:/doc_templates/reportes/dev/Template_consulta_polizas.xlsx";


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
                    int i = 8;
                    int letra = 65;

                    sl.SetCellValue("B2", data.P_FECHA_DESDE);
                    sl.SetCellValue("B3", data.P_FECHA_HASTA);

                    foreach (var item in lista)
                    {
                        int c = 0;
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.NOMBRE_PRODUCT);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.POLIZA);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.NRO_COTIZACION);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.DOCUMENTO);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.NOMBRE_CONTRATANTE);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.NOMBRE_BROKER);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.FECHA_EMISION);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.INICIO_VIGENCIA);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.FIN_VIGENCIA);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.NRO_RENOVACIONES);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.SDESCRIPT_SEG);
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
                plantilla = "";
                LogControl.save("GetPolicyTransactionAllListExcel", ex.ToString(), "3");
                throw ex;
            }
            return plantilla;
        }

        public ErrorCode insertErrorEPS(string cotizacion, string json, string nid_proc_epc, string nid_proc_sctr)
        {
            var response = new ErrorCode();
            var sPackageName = ProcedureName.pkg_EPS + ".SP_UDP_JSON_RELANZAR_EPS";
            List<OracleParameter> parameters = new List<OracleParameter>();
            try
            {

                parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, cotizacion, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SJSON", OracleDbType.Clob, json, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NID_PROC_EPS", OracleDbType.Varchar2, nid_proc_epc, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NID_PROC_SCTR", OracleDbType.Varchar2, nid_proc_sctr, ParameterDirection.Input));

                // OUTPUT
                OracleParameter P_COD_ERR = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                P_COD_ERR.Size = 200;
                P_MESSAGE.Size = 4000;

                parameters.Add(P_COD_ERR);
                parameters.Add(P_MESSAGE);


                this.ExecuteByStoredProcedureVT(sPackageName, parameters);

                response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                response.P_MESSAGE = P_MESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = ex.ToString();
                LogControl.save("insertErrorEPS", ex.ToString(), "3");
            }

            return response;
        }

        public async Task<ErrorCode> GenerarCalculosSCTR(string nid_proc, string nidproc_eps, int cotizacion, int type_mov, string date, string dateFn, int npayfreq, int codUser)
        {
            var response = new ErrorCode();
            var sPackageName = ProcedureName.pkg_EPS + ".SP_UPD_GENERAR_CALCULOS";
            List<OracleParameter> parameters = new List<OracleParameter>();
            try
            {
                parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, cotizacion, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NID_PROC_SCTR", OracleDbType.Varchar2, nid_proc, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NID_PROC_EPS", OracleDbType.Varchar2, nidproc_eps, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Int32, type_mov, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DATE", OracleDbType.Varchar2, date, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DATE_FIN", OracleDbType.Varchar2, dateFn, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPAYFREQ", OracleDbType.Int32, npayfreq, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Varchar2, codUser, ParameterDirection.Input));

                // OUTPUT
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                P_COD_ERR.Size = 200;
                P_MESSAGE.Size = 4000;

                parameters.Add(P_COD_ERR);
                parameters.Add(P_MESSAGE);

                this.ExecuteByStoredProcedureVT(sPackageName, parameters);

                response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                response.P_MESSAGE = P_MESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = ex.ToString();
                LogControl.save("GenerarCalculosSCTR", ex.ToString(), "3");
            }


            return await Task.FromResult(response); ;
        }

        public SalidadPolizaEmit saveToSendComprobantesEps(SavePolicyEmit policyEmit)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            SalidadPolizaEmit resultPackage = new SalidadPolizaEmit();
            string storedProcedureName = ProcedureName.pkg_EPS + ".SP_INS_TBL_PD_EPS_COMPROBANTES";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, policyEmit.P_NID_COTIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, policyEmit.P_NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, policyEmit.P_NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Char, policyEmit.P_NPOLICY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Int32, policyEmit.P_NTYPE_TRANSAC == null ? 1 : policyEmit.P_NTYPE_TRANSAC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Char, policyEmit.P_NID_PROC, ParameterDirection.Input));
                //OUTPUT
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, resultPackage.P_COD_ERR, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, 4000, resultPackage.P_MESSAGE, ParameterDirection.Output);

                parameter.Add(P_COD_ERR);
                parameter.Add(P_MESSAGE);

                this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                resultPackage.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                resultPackage.P_MESSAGE = P_MESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                resultPackage.P_COD_ERR = 1;
                resultPackage.P_MESSAGE = ex.ToString();
                LogControl.save("GenerarCalculosSCTR", ex.ToString(), "3");
            }

            return resultPackage;
        }


        public ComprobantesEpsBM getTypeComprobante(ComprobantesEpsBM item)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            ComprobantesEpsBM resultPackage = new ComprobantesEpsBM();
            string storedProcedureName = ProcedureName.pkg_EPS + ".SP_INS_TBL_PD_EPS_COMPROBANTES";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, item.P_NID_COTIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Char, item.P_NID_PROC, ParameterDirection.Input));
                //OUTPUT
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, resultPackage.P_COD_ERR, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, 4000, resultPackage.P_MESSAGE, ParameterDirection.Output);

                parameter.Add(P_COD_ERR);
                parameter.Add(P_MESSAGE);

                this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                resultPackage.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                resultPackage.P_MESSAGE = P_MESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                resultPackage.P_COD_ERR = 1;
                resultPackage.P_MESSAGE = ex.ToString();
                LogControl.save("GenerarCalculosSCTR", ex.ToString(), "3");
            }

            return item;
        }

        public async Task<ComprobantesEpsBM> GetTipoComprobante(ComprobantesEpsBM item)
        { // Obteniendo el Tipo de Comprobatne para el Movimiento de una Póliza

            List<OracleParameter> parameter = new List<OracleParameter>();
            ComprobantesEpsBM response = new ComprobantesEpsBM();
            //string storedProcedureName = ProcedureName.pkg_EPS + ".SP_GET_TIP_COMPROBANTE_TRANSACCION"; POR EL MOMENTO SOLO SE LLAMARA AL PROCEDURE DESPUES AL PAQUETE CON EL PROCEDURE
            string storedProcedureName = "SP_GET_TIP_COMPROBANTE_TRANSACCION";

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, item.P_NID_PROC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int64, item.P_NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int64, item.P_NBRANCH, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_SBILLTYPE = new OracleParameter("P_SBILLTYPE", OracleDbType.Varchar2, 100, response.SBILLTYPE, ParameterDirection.Output);
                OracleParameter P_NINSUR_AREA = new OracleParameter("P_NINSUR_AREA", OracleDbType.Int64, response.NINSUR_AREA, ParameterDirection.Output);
                OracleParameter P_NRECEIPT = new OracleParameter("P_NRECEIPT", OracleDbType.Int64, response.NRECEIPT, ParameterDirection.Output);

                parameter.Add(P_SBILLTYPE);
                parameter.Add(P_NINSUR_AREA);
                parameter.Add(P_NRECEIPT);

                this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                response = item;

                response.SBILLTYPE = P_SBILLTYPE.Value.ToString();
                response.NINSUR_AREA = Int32.Parse(P_NINSUR_AREA.Value.ToString());
                response.NRECEIPT = (int)long.Parse(P_NRECEIPT.Value.ToString());
                response.P_COD_ERR = 0;
            }
            catch (Exception ex)
            {
                item.P_COD_ERR = 1;
                item.P_MESSAGE = ex.ToString();
                LogControl.save("GenerarCalculosSCTR", ex.ToString(), "3");
            }

            return item;
        }


        public ComprobantesEpsBM GetReceiptRefernce(ComprobantesEpsBM new_item)
        {

            ComprobantesEpsBM response = new ComprobantesEpsBM();
            List<OracleParameter> parameter = new List<OracleParameter>();
            //string storedProcedureName = ProcedureName.pkg_EPS + ".SP_GET_TIP_COMPROBANTE_TRANSACCION"; POR EL MOMENTO SOLO SE LLAMARA AL PROCEDURE DESPUES AL PAQUETE CON EL PROCEDURE
            string storedProcedureName = "SP_GET_RECIBO_REFERENCIAL_EPS";

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, new_item.P_NID_PROC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, new_item.P_NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, new_item.P_NBRANCH, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_NRECEIPT_REF = new OracleParameter("P_NRECEIPT_REF", OracleDbType.Varchar2, null, ParameterDirection.Output);
                OracleParameter P_SBILLTYPE_REF = new OracleParameter("P_SBILLTYPE_REF", OracleDbType.Int32, null, ParameterDirection.Output);
                OracleParameter P_NINSUR_AREA_REF = new OracleParameter("P_NINSUR_AREA_REF", OracleDbType.Int32, null, ParameterDirection.Output);
                //OracleParameter P_NBILLNUM_REF = new OracleParameter("P_NBILLNUM_REF", OracleDbType.Int32, null, ParameterDirection.Output);


                parameter.Add(P_NRECEIPT_REF);
                parameter.Add(P_SBILLTYPE_REF);
                parameter.Add(P_NINSUR_AREA_REF);
                //parameter.Add(P_NBILLNUM_REF);

                this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                response.NRECEIPT_NC = Int32.Parse(P_NRECEIPT_REF.Value.ToString());
                response.SBILLTYPE_NC = P_SBILLTYPE_REF.Value.ToString();
                response.NINSUR_AREA_NC = Int32.Parse(P_NINSUR_AREA_REF.Value.ToString());
                //response.nb = 0;
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = ex.ToString();
                LogControl.save("GenerarCalculosSCTR", ex.ToString(), "3");
            }

            return response;
        }

        public async Task<quotationVM.AuthEPSResult> getTokenEPS(ComprobantesEpsBM comprobanteEps)
        {

            // Empezando llamdo de Servicio de la EPS.
            quotationVM.AuthEPSResult sQuoteResult = new quotationVM.AuthEPSResult();
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
                    var response = await client.PostAsync(ELog.obtainConfig("urlTokenEPS_SCTR"), content);
                    mToken = await response.Content.ReadAsStringAsync();
                }

                if (string.IsNullOrEmpty(mToken))
                {
                    LogControl.save("invocarServicio_EPS - API EMI", mToken, "3");
                    sQuoteResult.success = "false";
                    await UpdStatusSendComprobantesEPS(comprobanteEps, 5); // AQUI LLAMAR AL QUE ACTUALIZAR Y LO COLOCAMOS EN ESTADO PENDIENTE 3;

                }
                else
                {
                    sQuoteResult = JsonConvert.DeserializeObject<quotationVM.AuthEPSResult>(mToken);


                    if (sQuoteResult.success == "true")
                    {
                        if (comprobanteEps.SBILLTYPE == "5") // NECESITA REFACTORIZAR CODIGO
                        {
                            var format_object = new
                            {
                                codigoProcesoTransaccion = comprobanteEps.P_NID_PROC,
                                TipoComprobanteEmitir = comprobanteEps.SBILLTYPE,
                                SerieComprobanteEmitir = comprobanteEps.NINSUR_AREA.ToString(),
                                NumeroComprobanteEmitir = comprobanteEps.NRECEIPT.ToString()
                            };


                            jsonCredentials = JsonConvert.SerializeObject(format_object);
                        }

                        else // 7 Nota de Credito Con Factura Asociada Asociado
                        {
                            var format_object = new
                            {
                                codigoProcesoTransaccion = comprobanteEps.P_NID_PROC,
                                TipoComprobanteEmitir = comprobanteEps.SBILLTYPE,
                                SerieComprobanteEmitir = comprobanteEps.NINSUR_AREA,
                                NumeroComprobanteEmitir = comprobanteEps.NRECEIPT,
                                TipoComprobanteNC = comprobanteEps.SBILLTYPE_NC,
                                SerieComprobanteNC = comprobanteEps.NINSUR_AREA_NC,
                                NumeroComprobanteNC = comprobanteEps.NRECEIPT_NC
                            };

                            jsonCredentials = JsonConvert.SerializeObject(format_object);
                        }

                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sQuoteResult.token);
                            //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                            client.Timeout = TimeSpan.FromSeconds(Convert.ToInt32(ELog.obtainConfig("timeout")));
                            var response = await client.PostAsync(ELog.obtainConfig("urlComprobanteEPS_SCTR"), new StringContent(jsonCredentials, Encoding.UTF8, "application/json"));
                            var sQuoteResult2 = await response.Content.ReadAsStringAsync();
                            var responseObject = JsonConvert.DeserializeObject<dynamic>(sQuoteResult2);
                            bool isSuccess = responseObject.success;

                            if (isSuccess)
                            {
                                await UpdStatusSendComprobantesEPS(comprobanteEps, 1); // AQUI LLAMAR AL QUE ACTUALIZAR Y LO COLOCAMOS EN ESTADO PENDIENTE 3;
                            }
                            else
                            {
                                string errorMessage = responseObject.message;
                                LogControl.save("invocarServicio_EPS - API Certificados", errorMessage, "3");
                                await UpdStatusSendComprobantesEPS(comprobanteEps, 5); // AQUI LLAMAR AL QUE ACTUALIZAR Y LO COLOCAMOS EN ESTADO PENDIENTE 3;

                            }

                        }
                    }
                    else
                    {
                        // No se obtubo el Token
                        await UpdStatusSendComprobantesEPS(comprobanteEps, 5); // AQUI LLAMAR AL QUE ACTUALIZAR Y LO COLOCAMOS EN ESTADO PENDIENTE 3;
                    }
                }




            }
            catch (Exception ex)
            {
                LogControl.save("invocarServicio_EPS - API Login", ex.ToString(), "3");
            }
            return sQuoteResult;
        }

        public async Task<ErrorCode> sendComprobantEPSService(quotationVM.AuthEPSResult token, ComprobantesEpsBM comprobanteEps)
        {

            ErrorCode status_error = new ErrorCode();

            var jsonCredentials = "";
            if (comprobanteEps.P_NTYPE_TRANSAC == "5") // NECESITA REFACTORIZAR CODIGO
            {
                var format_object = new
                {
                    codigoProcesoTransaccion = comprobanteEps.P_NID_PROC,
                    TipoComprobanteEmitir = comprobanteEps.SBILLTYPE,
                    SerieComprobanteEmitir = comprobanteEps.NINSUR_AREA,
                    NumeroComprobanteEmitir = comprobanteEps.NRECEIPT
                };


                jsonCredentials = JsonConvert.SerializeObject(format_object);
            }

            else
            {  // 7 Nota de Credito Con Factura Asociada Asociado

                var format_object = new
                {
                    codigoProcesoTransaccion = comprobanteEps.P_NID_PROC,
                    TipoComprobanteEmitir = comprobanteEps.SBILLTYPE,
                    SerieComprobanteEmitir = comprobanteEps.NINSUR_AREA,
                    NumeroComprobanteEmitir = comprobanteEps.NRECEIPT,
                    TipoComprobanteNC = comprobanteEps.SBILLTYPE_NC,
                    SerieComprobanteNC = comprobanteEps.NINSUR_AREA_NC,
                    NumeroComprobanteNC = comprobanteEps.NRECEIPT_NC
                };

                jsonCredentials = JsonConvert.SerializeObject(format_object);
            }
            // Comprobantes

            //var content = new StringContent(jsonCredentials, Encoding.UTF8, "application/json");
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.token);
                    //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    client.Timeout = TimeSpan.FromSeconds(Convert.ToInt32(ELog.obtainConfig("timeout")));
                    var response = await client.PostAsync(ELog.obtainConfig("urlComprobanteEPS_SCTR"), new StringContent(jsonCredentials, Encoding.UTF8, "application/json"));
                    var sQuoteResult = await response.Content.ReadAsStringAsync();
                    var responseObject = JsonConvert.DeserializeObject<dynamic>(sQuoteResult);
                    bool isSuccess = responseObject.success;

                    if (isSuccess) { }
                    else
                    {
                        string errorMessage = responseObject.message;
                        LogControl.save("invocarServicio_EPS - API COMPROBANTE", errorMessage, "3");
                        throw new Exception(errorMessage);
                    }

                }
            }

            catch (Exception ex)
            {
                LogControl.save("invocarServicio_EPS - API EMI", ex.ToString(), "3");
            }

            return status_error;
        }


        public async Task UpdStatusSendComprobantesEPS(ComprobantesEpsBM item, int nstate)
        {

            ComprobantesEpsBM response = new ComprobantesEpsBM();
            List<OracleParameter> parameter = new List<OracleParameter>();
            //string storedProcedureName = ProcedureName.pkg_EPS + ".SP_UPD_STATUS_SEND_COMPROBANTES_EPS"; POR EL MOMENTO SOLO SE LLAMARA AL PROCEDURE DESPUES AL PAQUETE CON EL PROCEDURE
            string storedProcedureName = "SP_UPD_STATUS_SEND_COMPROBANTES_EPS";

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NSTATE", OracleDbType.Int32, nstate, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int64, item.P_NPOLICY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Int32, item.NID_PROC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, item.P_NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, item.P_NBRANCH, ParameterDirection.Input));

                this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = ex.ToString();
                LogControl.save("UpdStatusSendComprobantesEPS", ex.ToString(), "3");
            }

        }

        public async Task<ErrorCode> SendComprobantesEps(List<ComprobantesEpsBM> list_comprobantes)
        {
            ErrorCode error = new ErrorCode();

            var formatter_request = new { };

            foreach (var item in list_comprobantes)
            {
                // Agregando el tipo De Comprobante, Si es  Recibo o Nota de Credito
                // Llamar al Procedure que se encarga de carga el Tipo de Comprobante y si obtiene valor lo que va hacer es actualizar el ESTADO A TOMADO, ya que se entiende que se Genero el Comprobante

                try
                {

                    await UpdStatusSendComprobantesEPS(item, 3); // AQUI LLAMAR AL QUE ACTUALIZAR Y LO COLOCAMOS EN ESTADO PENDIENTE 3;

                    ComprobantesEpsBM new_item = await GetTipoComprobante(item); // El new Item Tiene los valores que son traidos del Procedure

                    if (new_item.P_COD_ERR == 0)
                    {
                        // 5 Factura - 7 Nota de Credito Factura, Si es Tipo 7 Nota de Credito, tiene que buscar La Factura Asociada
                        if (new_item.SBILLTYPE == "7")
                        {
                            var reference_responce = GetReceiptRefernce(new_item);
                            new_item = reference_responce;
                        }


                        var json = Task.Run(async () =>
                        {
                            quotationVM.AuthEPSResult token_res = await getTokenEPS(new_item);
                            if (token_res.success != "true")
                            {
                                // Token Invalido O sucedio Un error
                            }
                            else
                            {
                                //var response_eps_service = await sendComprobantEPSService(token_res, new_item);
                            }
                        });




                        /*
                        var credentials = new
                        {
                            usuario = ELog.obtainConfig("EPSUser"),
                            clave = ELog.obtainConfig("EPSoPwd")
                        };
                        */
                        /*
                        var jsonCredentials = JsonConvert.SerializeObject(credentials);
                        var content = new StringContent(jsonCredentials, Encoding.UTF8, "application/json");
                        var mToken = "";
                        */
                        /*
                        using (HttpClient client = new HttpClient())
                        {
                            var response = await client.PostAsync(ELog.obtainConfig("urlTokenEPS_SCTR"), content);
                            mToken = await response.Content.ReadAsStringAsync();
                            if (!string.IsNullOrEmpty(mToken))
                            {
                                LogControl.save("SendComprobantesEps", mToken, "3");
                            }
                        }
                        */

                        /*
                        var QuotEPSToken = JsonConvert.DeserializeObject<quotationVM.AuthEPSResult>(mToken);
                        */
                        //if (QuotEPSToken.success == "succes") { }
                        /*
                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", QuotEPSToken.token);
                            client.Timeout = TimeSpan.FromSeconds(Convert.ToInt32(ELog.obtainConfig("timeout")));
                            string esta_es_la_data_para_enviar_como_json = "";
                            var json = JsonConvert.SerializeObject(esta_es_la_data_para_enviar_como_json); // ACA SE VA CONVERTIR A JSON LOS VALORES QUE TENEMOS QUE ENVIARLE AL POSTMAN
                            var response = await client.PostAsync(ELog.obtainConfig("urlEmisionEPS_SCTR"), new StringContent(json, Encoding.UTF8, "application/json"));
                            var sQuoteResult = await response.Content.ReadAsStringAsync();
                            var responseObject = JsonConvert.DeserializeObject<dynamic>(sQuoteResult);
                            bool isSuccess = responseObject.success;
                        */
                        /*
                        if (isSuccess)
                        {
                            new QuotationDA().InsertLog(cotizacion, "02 - Se recibe respuesta de EPS - Transaccion de Poliza", ELog.obtainConfig("urlEmisionEPS_SCTR"), sQuoteResult, null);
                            LogControl.save("generarTransaccionEPS", "{\"cotizacion\": \"" + cotizacion.ToString() + ", \"mensaje\":  \"02 - Se recibe respuesta de EPS - Transaccion de Poliza\", \"url\": " + ELog.obtainConfig("urlTokenEPS_SCTR") + ", \"json\":" + sQuoteResult + " }", "2", "EPS");
                        }
                        else
                        {
                            string errorMessage = responseObject.message;
                            LogControl.save("invocarServicio_EPS - API EMI", errorMessage, "3");
                            throw new Exception(errorMessage);
                        }
                        */
                    }
                    //}
                }
                catch (Exception ex)
                {
                    LogControl.save("insertHistTrama", ex.ToString(), "3");
                    await UpdStatusSendComprobantesEPS(item, 5);
                }
            }
            return error;
        }

        public SalidadPolizaEmit UPD_TRX_CARGA_DEFINITIVA(Ins_Documentacion_EPS request)
        {
            var sPackageName = ProcedureName.pkg_EPS + ".UPD_TRX_CARGA_DEFINITIVA_EPS";
            var response = new SalidadPolizaEmit();
            List<OracleParameter> parameters = new List<OracleParameter>();

            try
            {
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int64, request.P_NBRANCH, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int64, request.P_NPRODUCT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int64, request.P_NID_COTIZACION, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int64, request.P_NPOLICY, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Int64, request.P_NTYPE_TRANSAC, ParameterDirection.Input));

                OracleParameter P_COD_ERR = new OracleParameter("P_NCODE", OracleDbType.Int64, response.P_COD_ERR, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, response.P_MESSAGE, ParameterDirection.Output);

                P_COD_ERR.Size = 200;
                P_MESSAGE.Size = 4000;

                parameters.Add(P_COD_ERR);
                parameters.Add(P_MESSAGE);

                this.ExecuteByStoredProcedureVT(sPackageName, parameters);

                response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                response.P_MESSAGE = P_MESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = ex.ToString();
                LogControl.save("UPD_TRX_CARGA_DEFINITIVA", ex.ToString(), "3");
            }

            return response;

        }

        public List<dataTecnicaTrans> getDataTecnica(int cotizacion)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<dataTecnicaTrans> resultPackage = new List<dataTecnicaTrans>();

            string storedProcedureName = ProcedureName.pkg_EPS + ".SP_GET_RENOV_TECNICA";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, cotizacion, ParameterDirection.Input));

                OracleParameter table = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(table);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                while (odr.Read())
                {
                    dataTecnicaTrans item = new dataTecnicaTrans();
                    item.NPOLICY = odr["NPOLICY"] == DBNull.Value ? "" : odr["NPOLICY"].ToString();
                    item.NCURRENCY = odr["NCURRENCY"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NCURRENCY"]);
                    item.NPREMIUM_MIN_AU = odr["NPREMIUM_MIN_AU"] == DBNull.Value ? 0 : Convert.ToDouble(odr["NPREMIUM_MIN_AU"]);
                    item.NSUM_PREMIUMN = odr["NSUM_PREMIUMN"] == DBNull.Value ? 0 : Convert.ToDouble(odr["NSUM_PREMIUMN"]);
                    item.NSUM_IGV = odr["NSUM_IGV"] == DBNull.Value ? 0 : Convert.ToDouble(odr["NSUM_IGV"]);
                    item.NDE = odr["NDE"] == DBNull.Value ? 0 : Convert.ToDouble(odr["NDE"]);
                    item.NSUM_PREMIUM = odr["NSUM_PREMIUM"] == DBNull.Value ? 0 : Convert.ToDouble(odr["NSUM_PREMIUM"]);
                    item.NID_PROC = odr["NID_PROC"] == DBNull.Value ? "" : odr["NID_PROC"].ToString();
                    item.SDELIMITER = odr["SDELIMITER"] == DBNull.Value ? 0 : Convert.ToInt32(odr["SDELIMITER"]);
                    item.FACTURA_ANT = odr["FACTURA_ANT"] == DBNull.Value ? 0 : Convert.ToInt32(odr["FACTURA_ANT"]);
                    item.REGULA = odr["REGULA"] == DBNull.Value ? 0 : Convert.ToInt32(odr["REGULA"]);
                    resultPackage.Add(item);
                }
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                LogControl.save("GetAsegurados", ex.ToString(), "3");
            }

            return resultPackage;
        }

        public string GetFechaPago(int cod_cotizacion)
        {
            var fechaPago = "";
            var sPackageName = ProcedureName.pkg_EPS + ".SP_GET_FECHA_PAGO_DIAS_CREDITO";
            List<OracleParameter> parameters = new List<OracleParameter>();
            try
            {
                parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, cod_cotizacion, ParameterDirection.Input));

                // OUTPUT
                OracleParameter P_FECHA_PAGO = new OracleParameter("P_FECHA_PAGO", OracleDbType.Varchar2, fechaPago, ParameterDirection.Output);
                P_FECHA_PAGO.Size = 200;
                parameters.Add(P_FECHA_PAGO);

                this.ExecuteByStoredProcedureVT(sPackageName, parameters);

                DateTime fecha = DateTime.Parse(P_FECHA_PAGO.Value.ToString());
                fechaPago = fecha.ToString("dd/MM/yyyy");
            }
            catch (Exception ex)
            {
                LogControl.save("GetFechaPago", ex.ToString(), "3");
            }

            return fechaPago;
        }

        public SalidadPolizaEmit updateQuotationCol(SavePolicyEmit request, SalidadPolizaEmit request2)
        {
            var sPackageName = ProcedureName.pkg_Cotizacion + ".UPD_COTIZACION_COL_PD";
            List<OracleParameter> parameter = new List<OracleParameter>();

            var response = new SalidadPolizaEmit();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, request.P_NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, request.P_NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, request.P_NID_COTIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int32, request2.P_POL_AP, ParameterDirection.Input));


                //OUTPUT
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, response.P_MESSAGE, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_NCOD_ERR", OracleDbType.Int32, response.P_COD_ERR, ParameterDirection.Output);

                P_MESSAGE.Size = 9000;

                parameter.Add(P_MESSAGE);
                parameter.Add(P_COD_ERR);

                this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                response.P_MESSAGE = P_MESSAGE.Value.ToString();
                response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = ex.ToString();
                LogControl.save("InsertQuotationDetAP", ex.ToString(), "3");
            }

            return response;
        }

        public List<QuotizacionColBM> GetPolizaColegio(int nroCotizacion)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<QuotizacionColBM> lista = new List<QuotizacionColBM>();
            string storedProcedureName = ProcedureName.pkg_Cotizacion + ".REA_COT_EMI_COLE";

            try
            {
                parameter.Add(new OracleParameter("NRO_COTIZACION", OracleDbType.Int32, nroCotizacion, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, lista, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);
                while (odr.Read())
                {
                    var polizaEmitCole = new QuotizacionColBM();
                    polizaEmitCole.sclient = odr["SCLIENT"] == DBNull.Value ? "" : odr["SCLIENT"].ToString();
                    polizaEmitCole.tipDoc = odr["NIDDOC_TYPE"] == DBNull.Value ? "" : odr["NIDDOC_TYPE"].ToString();
                    polizaEmitCole.document = odr["SNUM_DOCUMENT"] == DBNull.Value ? "" : odr["SNUM_DOCUMENT"].ToString();
                    polizaEmitCole.legalName = odr["SLEGALNAME"] == DBNull.Value ? "" : odr["SLEGALNAME"].ToString();
                    polizaEmitCole.nid_cotizacion = nroCotizacion;
                    lista.Add(polizaEmitCole);
                }
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                LogControl.save("GetPolizaColegio", ex.ToString(), "3");
            }

            return lista;
        }

        // INI GCAA 22012024
        public string getDatos_Procesar_Cobertura(int cotizacion)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storedProcedureName = "GETDATOS_PROCESAR_COBERTURA";
            try
            {
                List<listaDatosConsultaPesos> ListFinal = new List<listaDatosConsultaPesos>();

                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int64, cotizacion, ParameterDirection.Input));
                OracleParameter table = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(table);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                while (odr.Read())
                {
                    var item = new listaDatosConsultaPesos();
                    item.NBRANCH = Convert.ToInt32(odr["NBRANCH"].ToString());
                    item.NPRODUCT = Convert.ToInt32(odr["NPRODUCT"].ToString());
                    item.NMODULEC = Convert.ToInt32(odr["NMODULEC_FINAL"].ToString());
                    item.DEFFECDATE = Convert.ToDateTime(odr["DEFFECDATE"].ToString());
                    item.SDESCRIPT = odr["SDESCRIPT"].ToString();
                    ListFinal.Add(item);
                }
                ELog.CloseConnection(odr);

                foreach (var ite in ListFinal)
                {
                    getListaCoberturas(ite.NBRANCH, ite.NPRODUCT, ite.NMODULEC, ite.DEFFECDATE, cotizacion, ite.SDESCRIPT);
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GETDATOS_PROCESAR_COBERTURA", ex.ToString(), "3");
            }

            return "";
        }



        // FIN GCAA 22012024


        // INI GCAA 18012024
        public List<ListaCobertura> getListaCoberturas(int P_NBRANCH, int P_NPRODUCT, int P_NMODULEC, DateTime P_DEFFECDATE, int cotizacion, string CATEGORIA)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<ListaCobertura> resultPackage = new List<ListaCobertura>();
            EntityCobertura respuesta = new EntityCobertura();

            string storedProcedureName = "pkg_pd_genera_transaccion.SP_LIST_COVER_";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int64, P_NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int64, P_NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int64, P_NMODULEC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Date, P_DEFFECDATE, ParameterDirection.Input));

                OracleParameter table = new OracleParameter("P_C_COVER", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(table);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                string coverageCode = "";
                ListaCobertura item = new ListaCobertura();

                item.coverageCodeList = new List<string>();

                while (odr.Read())
                {
                    coverageCode = odr["NCOVER"].ToString();
                    item.coverageCodeList.Add(coverageCode);
                }

                ELog.CloseConnection(odr);

                resultPackage.Add(item);


                LogControl.save("getListaCoberturas-GCAA-1", JsonConvert.SerializeObject(resultPackage.ToList()), "2");


                if (resultPackage.ToList().Count > 0)
                {
                    // se arma el json de coberturas para consultar al devmente
                    string jsonx = JsonConvert.SerializeObject(resultPackage.ToList());
                    // se quita los []
                    string resultado = jsonx.Substring(1, jsonx.Length - 2);
                    // se obtiene el resultado de cobertura devueltas por devmente

                    LogControl.save("getListaCoberturas-GCAA-2", JsonConvert.SerializeObject(resultado), "2");
                    respuesta = new WebApiDevmente().ConsultarCobertura(resultado);

                    // enviamos cobertura por cobertura para actualizar
                    foreach (var ite in respuesta.data)
                    {
                        new PolicyDA().Actualizar_proceso_cobertura(
                            cotizacion,
                            Convert.ToInt32(ite.coverageCode),
                            ite.weight,
                            P_NMODULEC,
                            Convert.ToInt32(ite.isPrincipal),
                            CATEGORIA,
                            "PD",
                            "");
                    }

                }
            }
            catch (Exception ex)
            {
                LogControl.save("getListaCoberturas", ex.ToString(), "3");
            }

            return resultPackage.ToList();
        }
        // FIN GCAA 18012024

        // INI GCAA 18012024
        public string Actualizar_proceso_cobertura(int P_NID_COTIZACION, int P_NCOVER, double P_NRATECOVE, int P_NMODULEC, int P_NMAIN, string P_SDESCRIPTION, string P_SORIGEN, string P_SRANGO_EDAD)
        {
            var response = new ErrorCode();
            var sPackageName = "PKG_PD_GENERA_TRANSACCION.SP_SET_NCOVER_BY_MODULE";
            List<OracleParameter> parameters = new List<OracleParameter>();
            try
            {
                parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, P_NID_COTIZACION, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCOVER", OracleDbType.Int32, P_NCOVER, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NRATECOVE", OracleDbType.Double, P_NRATECOVE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, P_NMODULEC, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NMAIN", OracleDbType.Double, P_NMAIN, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SDESCRIPTION", OracleDbType.Varchar2, P_SDESCRIPTION, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SORIGEN", OracleDbType.Varchar2, P_SORIGEN, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SRANGO_EDAD", OracleDbType.Varchar2, P_SRANGO_EDAD, ParameterDirection.Input));
                //// OUTPUT
                //OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, ParameterDirection.Output);
                //OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                //P_COD_ERR.Size = 200;
                //P_MESSAGE.Size = 4000;

                //parameters.Add(P_COD_ERR);
                //parameters.Add(P_MESSAGE);

                this.ExecuteByStoredProcedureVT(sPackageName, parameters);

                //response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                // response.P_MESSAGE = ""; // P_MESSAGE.Value.ToString();

                LogControl.save("Actualizar_proceso_cobertura-GCAA-4", "", "2");

            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = ex.ToString();
                LogControl.save("Actualizar_proceso_cobertura-GCAA-4", ex.ToString(), "3");
            }
            return "";
        }
        // FIN GCAA 18012024

        public calcExec GetCalcExecSCTR(string nidproc, int ramo, int? producto, int? nrocotizacion, string npolicy, int varCalcExec, string fecha_exclusion)
        {
            var sPackageName = ProcedureName.pkg_EPS + ".SP_GET_PRIMA_EXEC";
            calcExec result = new calcExec();

            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();

                parameters.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, nidproc, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, ramo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, producto, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, nrocotizacion, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPOLICY", OracleDbType.Varchar2, npolicy, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_TIPO_CALC_EXC", OracleDbType.Int32, varCalcExec, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DFECANULA", OracleDbType.Varchar2, fecha_exclusion, ParameterDirection.Input));

                OracleParameter P_COD_ERR = new OracleParameter("P_ERROR", OracleDbType.Int32, result.error, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, result.mensaje, ParameterDirection.Output);
                OracleParameter table = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                P_COD_ERR.Size = 200;
                P_MESSAGE.Size = 4000;

                parameters.Add(P_COD_ERR);
                parameters.Add(P_MESSAGE);
                parameters.Add(table);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameters);

                while (odr.Read())
                {
                    result.npremiumn = odr["NPREMIUMN"] == DBNull.Value ? 0 : Convert.ToDouble(odr["NPREMIUMN"]);
                    result.npremiumn_ori = odr["NPREMIUMN_ORI"] == DBNull.Value ? 0 : Convert.ToDouble(odr["NPREMIUMN_ORI"]);
                    result.nigv = odr["NIGV"] == DBNull.Value ? 0 : Convert.ToDouble(odr["NIGV"]);
                    result.npremium = odr["NPREMIUM"] == DBNull.Value ? 0 : Convert.ToDouble(odr["NPREMIUM"]);
                    result.nproduct = odr["NPRODUCT"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NPRODUCT"]);
                }

                // Obtener valores de P_ERROR y P_MESSAGE
                result.error = Convert.ToInt32(P_COD_ERR.Value.ToString());
                result.mensaje = P_MESSAGE.Value.ToString();

                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                result.error = 1;
                result.mensaje = ex.ToString();
                LogControl.save("GetCalcExecSCTR", ex.ToString(), "3");
            }

            return result;
        }

        public Entities.QuotationModel.ViewModel.QuotationResponseVM ValidateProcessInsured(QuotationCabBM request)
        {
            var response = new Entities.QuotationModel.ViewModel.QuotationResponseVM();
            DbConnection DataConnection = ConnectionGet(enuTypeDataBase.OracleVTime);
            DbTransaction trx = null;
            DataConnection.Open();
            trx = DataConnection.BeginTransaction();

            var FlagList = new List<Entities.QuotationModel.ViewModel.QuotationResponseVM>();

            try
            {

                response = QuotationDA.UpdateCodQuotationPD(request.NumeroCotizacion.ToString(), request, DataConnection, trx);
                response = QuotationDA.ValCotizacion(request.NumeroCotizacion.Value, request.P_NUSERCODE, 0, 0, DataConnection, trx, 1);

            }
            catch (Exception ex)
            {
                response = new quotationVM.QuotationResponseVM();
                response.P_COD_ERR = 1;
                response.P_MESSAGE = ex.ToString();
                LogControl.save("RenewMod", ex.ToString(), "3");
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

        public PolicyValidateQuotationTransacVM PolicyValidateQuotationTransac(PolicyValidateQuotationTransacBM data)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            PolicyValidateQuotationTransacVM result = new PolicyValidateQuotationTransacVM();
            string storedProcedureName = ProcedureName.pkg_Cotizacion + ".VAL_ANUL_COT";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                //parameter.Add(new OracleParameter("P_ID_TIPO_TRANS", OracleDbType.Int32, idTipo, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_SMESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_SRPTA = new OracleParameter("P_SRPTA", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);

                //P_SMESSAGE.Size = 4000;
                parameter.Add(P_SMESSAGE);
                parameter.Add(P_NCODE);
                parameter.Add(P_SRPTA);


                // OracleParameter P_MESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                result.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                result.P_NCODE = Int32.Parse(P_NCODE.Value.ToString());
                result.P_SRPTA = P_SRPTA.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                //resultPackage.P_MESSAGE = ex.ToString();
                result.P_SMESSAGE = "Hubo un error al validar. Favor de comunicarse con sistemas";
                result.P_NCODE = 1;
                LogControl.save("valTransactionPolicy", ex.ToString(), "3");
            }

            return result;
        }

        public int GetDataFlagPM(string nidproc)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            int result = 0;
            string storedProcedureName = ProcedureName.pkg_ValidadorGenPD + ".SP_GET_FLAG_PM";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, nidproc, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_FLAG = new OracleParameter("P_FLAG", OracleDbType.Int32, ParameterDirection.Output);

                parameter.Add(P_FLAG);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                result = Int32.Parse(P_FLAG.Value.ToString());
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                result = 0;
            }

            return result;
        }

        public int Udp_link_kushki(string id_sctr, string id_eps, string id_link_sctr, string id_link_eps, string id_tipo_pago_sctr, string id_tipo_pago_eps, string mensaje)
        {
            var sPackageName = ProcedureName.pkg_EPS + ".SP_UDP_LINK_KUSHKI";
            int result = 0;
            List<OracleParameter> parameters = new List<OracleParameter>();

            try
            {
                parameters.Add(new OracleParameter("P_ID_SCTR", OracleDbType.Varchar2, id_sctr, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_ID_EPS", OracleDbType.Varchar2, id_eps, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SLINK_PEN", OracleDbType.Varchar2, id_link_sctr, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SLINK_SAL", OracleDbType.Varchar2, id_link_eps, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_STIPO_PAGO_PEN", OracleDbType.Varchar2, id_tipo_pago_sctr, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_STIPO_PAGO_SAL", OracleDbType.Varchar2, id_tipo_pago_eps, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, mensaje, ParameterDirection.Input));

                OracleParameter P_COD_ERR = new OracleParameter("P_NCODE", OracleDbType.Int64, result, ParameterDirection.Output);

                P_COD_ERR.Size = 200;

                parameters.Add(P_COD_ERR);

                this.ExecuteByStoredProcedureVT(sPackageName, parameters);

                result = Convert.ToInt32(P_COD_ERR.Value.ToString());
            }
            catch (Exception ex)
            {
                result = 1;
                LogControl.save("SP_UDP_LINK_KUSHKI", ex.ToString(), "3");
            }
            return result;
        }

        public PagoKushki getPagoKushki(int nroCotizacion_pen, string nidProc_pen, int nroCotizacion_sal, string nidProc_sal, string stype_transac)
        {
            var sPackageName = ProcedureName.pkg_EPS + ".SP_SCTR_GET_PAGO_KUSHKI";
            List<OracleParameter> parameters = new List<OracleParameter>();
            PagoKushki result = new PagoKushki();

            try
            {
                //INPUT
                parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, nroCotizacion_pen, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NID_PROC_SCTR", OracleDbType.Varchar2, nidProc_pen, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NID_COTIZACION_EPS", OracleDbType.Int32, nroCotizacion_sal, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NID_PROC_EPS", OracleDbType.Varchar2, nidProc_sal, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Varchar2, stype_transac, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);

                parameters.Add(C_TABLE);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameters);

                while (odr.Read())
                {
                    result.P_STIPO_PAGO_PEN = odr["P_STIPO_PAGO_PEN"] == DBNull.Value ? "" : odr["P_STIPO_PAGO_PEN"].ToString();
                    result.P_SLINK_PEN = odr["P_SLINK_PEN"] == DBNull.Value ? "" : odr["P_SLINK_PEN"].ToString();
                    result.P_STIPO_PAGO_SAL = odr["P_STIPO_PAGO_SAL"] == DBNull.Value ? "" : odr["P_STIPO_PAGO_SAL"].ToString();
                    result.P_SLINK_SAL = odr["P_SLINK_SAL"] == DBNull.Value ? "" : odr["P_SLINK_SAL"].ToString();
                }

                result.P_NCODE = P_NCODE.Value.ToString();
                result.P_SMESSAGE = P_SMESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                result.P_NCODE = "1";
                result.P_SMESSAGE = ex.Message;
            }

            return result;
        }


        public PagoKushki getCodPagoKushki(int ramo, int idPayment_pen, int idPayment_sal)
        {
            var sPackageName = ProcedureName.pkg_EPS + ".SP_SCTR_GET_CODPAGO_KUSHKI";
            List<OracleParameter> parameters = new List<OracleParameter>();
            PagoKushki result = new PagoKushki();

            try
            {
                //INPUT
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, ramo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NID_PAYMENT_PEN", OracleDbType.Int32, idPayment_pen, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NID_PAYMENT_SAL", OracleDbType.Int32, idPayment_sal, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);

                parameters.Add(C_TABLE);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameters);

                while (odr.Read())
                {
                    result.P_SCOD_PAGO_PEN = odr["P_SCOD_PAGO_PEN"] == DBNull.Value ? "" : odr["P_SCOD_PAGO_PEN"].ToString();
                    result.P_SCOD_PAGO_SAL = odr["P_SCOD_PAGO_SAL"] == DBNull.Value ? "" : odr["P_SCOD_PAGO_SAL"].ToString();
                }

                result.P_NCODE = P_NCODE.Value.ToString();
                result.P_SMESSAGE = P_SMESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                result.P_NCODE = "1";
                result.P_SMESSAGE = ex.Message;
            }

            return result;
        }

        public NidProcessRenewVM GetProcessTrama(int nroCotizacion) {

            List<OracleParameter> parameter = new List<OracleParameter>();
            NidProcessRenewVM result = new NidProcessRenewVM();
            string storedProcedureName = ProcedureName.pkg_Poliza + ".GET_PROC_RENEW";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, nroCotizacion, ParameterDirection.Input));
                //OUTPUT
                OracleParameter P_NID_PROC = new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                OracleParameter P_NID_PROC_EPS = new OracleParameter("P_NID_PROC_EPS", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
             
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);

                parameter.Add(P_NID_PROC);
                parameter.Add(P_NID_PROC_EPS);
                parameter.Add(P_NCODE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                result.NID_PROC = P_NID_PROC.Value.ToString();
                result.NID_PROC_EPS = P_NID_PROC_EPS.Value.ToString();
                result.NCODE = Int32.Parse(P_NCODE.Value.ToString());

                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                result.NID_PROC = "null";
                result.NID_PROC_EPS = "null";
                result.NCODE = 1;
                LogControl.save("GetProcessTrama", ex.ToString(), "3");
            }

            return result;
        }

        public ErrorCode PayDirectMethods(int nroCotizacion, string typeMovement)
        {
            var response = new ErrorCode();
            var sPackageName = ProcedureName.pkg_EPS + ".SP_VALIDATE_PAGO_DIRECTO_SCTR";
            List<OracleParameter> parameters = new List<OracleParameter>();
            try
            {

                parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, nroCotizacion, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NTRANSAC", OracleDbType.Varchar2, typeMovement, ParameterDirection.Input));

                // OUTPUT
                OracleParameter L_NFLAG = new OracleParameter("L_NFLAG", OracleDbType.Int32, ParameterDirection.Output);

                parameters.Add(L_NFLAG);

                this.ExecuteByStoredProcedureVT(sPackageName, parameters);

                response.P_ORDER = Convert.ToInt32(L_NFLAG.Value.ToString());
            }
            catch (Exception ex)
            {
                //response.P_COD_ERR = 1;
                response.P_COD_ERR = 0;
                response.P_MESSAGE = ex.ToString();
                LogControl.save("GetPayDirectMethods", ex.ToString(), "3");
            }

            return response;
        }

        public TypePayVM GetTipoPagoSCTR(int nroCotizacion)
        {
            var response = new TypePayVM();
            var sPackageName = ProcedureName.pkg_EPS + ".SP_TIPO_PAGO_SCTR";
            List<OracleParameter> parameters = new List<OracleParameter>();
            try
            {
                parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, nroCotizacion, ParameterDirection.Input));

                // OUTPUT
                OracleParameter P_NTIPOPAGO = new OracleParameter("P_NTIPOPAGO", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_NCANTDIAS = new OracleParameter("P_NCANTDIAS", OracleDbType.Int32, ParameterDirection.Output);

                parameters.Add(P_NTIPOPAGO);
                parameters.Add(P_NCANTDIAS);

                this.ExecuteByStoredProcedureVT(sPackageName, parameters);

                response.NTIPOPAGO = Convert.ToInt32(P_NTIPOPAGO.Value.ToString());
                response.CANTIDAD_DIAS = Convert.ToInt32(P_NCANTDIAS.Value.ToString());
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 0;
                response.P_MESSAGE = ex.ToString();
                LogControl.save("GetTipoPagoSCTR", ex.ToString(), "3");
            }

            return response;
        }
    }
}