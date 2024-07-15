using ExcelDataReader;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WSPlataforma.Core;
using WSPlataforma.Entities.PolicyModel.BindingModel;
using WSPlataforma.Entities.PolicyModel.ViewModel;
using System.Text;
using WSPlataforma.Entities.ViewModel;
using WSPlataforma.Entities.QuotationModel.ViewModel;
using WSPlataforma.Entities.QuotationModel.BindingModel;
using VMpolicy = WSPlataforma.Entities.PolicyModel.ViewModel;
using System.Configuration;
using WSPlataforma.Entities.MapfreIntegrationModel.BindingModel;
using WSPlataforma.Entities.EPSModel.ViewModel;
using System.Threading.Tasks;
using WSPlataforma.Util;
using WSPlataforma.Entities.MapfreIntegrationModel.ViewModel;
using GenericPackageVM = WSPlataforma.Entities.PolicyModel.ViewModel.GenericPackageVM;
using WSPlataforma.Entities.CobranzasModel;
using WSPlataforma.ApiServiceClient;
using WSPlataforma.Entities.Graphql;
using SharedVM = WSPlataforma.Entities.SharedModel.ViewModel;
using WSPlataforma.Entities.AjustedAmountsModel.BindingModel;
using WSPlataforma.Entities.AjustedAmountsModel.ViewModel;
using System.Net;
using System.Globalization;
using System.Threading;
using WSPlataforma.Mappings;
using WSPlataforma.Entities.MovementsModel.ViewModel;
using WSPlataforma.Entities.ValBrokerModel;
using WSPlataforma.DA;
using WSPlataforma.Entities.SharedModel.BindingModel;
using WSPlataforma.Entities.ComprobantesEPSModel.BindingModel;
using static WSPlataforma.Entities.CoberturaModel.EntityCobertura;
using WSPlataforma.Entities.CoberturaModel;
using WSPlataforma.Entities.NidProcessRenewModel.ViewModel;

namespace WSPlataforma.Controllers
{
    [RoutePrefix("Api/PolicyManager")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PolicyController : ApiController
    {

        PolicyCORE policyCORE = new PolicyCORE();
        MapfreIntegrationController mapfreController = new MapfreIntegrationController();
        SharedMethods sharedMethods = new SharedMethods();
        PolicyDA policyDA = new PolicyDA();
        QuotationDA QuotationDA = new QuotationDA();

        [Route("ValidatePolicyRenov")]
        [HttpPost]
        public VMpolicy.ResponseVM cancelMovement(PolicyRenewBM policyRenew)
        {
            VMpolicy.ResponseVM response = new VMpolicy.ResponseVM();
            try
            {
                response = policyCORE.ValidatePolicyRenov(policyRenew);
            }
            catch (Exception ex)
            {
                response.P_NCODE = "1";
                response.P_SMESSAGE = ex.Message;
            }

            return response;
        }

        [Route("CancelMovement")]
        [HttpPost]
        public VMpolicy.ResponseVM cancelMovement(PolicyMovementCancelBM data)
        {
            VMpolicy.ResponseVM response = new VMpolicy.ResponseVM();
            try
            {
                LogControl.save("CancelMovement-REVERSO_VL", JsonConvert.SerializeObject(data, Formatting.None), "2");
                response = policyCORE.cancelMovement(data);
            }
            catch (Exception ex)
            {
                response.P_NCODE = "1";
                response.P_SMESSAGE = ex.Message;
                LogControl.save("CancelMovement-REVERSO_VL", ex.ToString(), "3");
            }

            return response;
        }
        [Route("GetValidateExistPolicy")]
        [HttpPost]
        public PolicyModuleVM GetValidateExistPolicy(PolicyModuleBM policyModuleBM)
        {
            return policyCORE.GetValidateExistPolicy(policyModuleBM);
        }

        [Route("GetMovementTypeList")]
        [HttpGet]
        public Entities.PolicyModel.ViewModel.GenericPackageVM GetMovementTypeList()
        {
            return policyCORE.GetMovementTypeList();
        }
        [Route("GetPolicyTrackingList")]
        [HttpPost]
        public GenericResponseVM GetPolicyTrackingList(TrackingBM data)
        {
            return policyCORE.GetPolicyTrackingList(data);
        }
        [Route("GetProductTypeList")]
        [HttpGet]
        public Entities.PolicyModel.ViewModel.GenericPackageVM GetProductTypeList()
        {
            return policyCORE.GetProductTypeList();
        }
        [Route("GetInsuredPolicyList")]
        [HttpPost]
        public Entities.PolicyModel.ViewModel.GenericPackageVM GetInsuredPolicyList(InsuredPolicySearchBM data)
        {
            return policyCORE.GetInsuredPolicyList(data);
        }
        [Route("GetTransactionAllList")]
        [HttpGet]
        public List<TransactionAllTypeVM> GetTransactionAllList()
        {
            return policyCORE.GetTransactionAllList();
        }
        [Route("GetTransaccionList")]
        [HttpGet]
        public List<TransaccionTypeVM> GetTransaccionList()
        {
            return policyCORE.GetTransaccionList();
        }
        [Route("GetPolicyTransList")]
        [HttpPost]
        public GenericTransVM GetPolicyTransList(TransPolicySearchBM data)
        {
            return policyCORE.GetPolicyTransList(data);
        }
        [Route("GetPolicyMovementsTransList")]
        [HttpPost]
        public GenericTransVM GetPolicyMovementsTransList(TransPolicySearchBM data)
        {
            return policyCORE.GetPolicyMovementsTransList(data);
        }
        [Route("GetPolicyProofList")]
        [HttpPost]
        public Entities.PolicyModel.ViewModel.GenericPackageVM GetPolicyProofList(PolicyProofSearchBM data)
        {
            return policyCORE.GetPolicyProofList(data);
        }
        [Route("GetPolicyMovementsTransAllList")]
        [HttpPost]
        public List<PolicyMovAllVM> GetPolicyMovementsTransAllList(PolicyTransactionAllSearchBM data)
        {
            return policyCORE.GetPolicyMovementsTransAllList(data);
        }

        [Route("GetInsuredForTransactionPolicy")]
        [HttpPost]
        public InsuredPolicyResponse GetInsuredForTransactionPolicy(PolicyInsuredFilter _data)
        {
            return policyCORE.GetInsuredForTransactionPolicy(_data);
        }

        [Route("UpdateInsuredPolicy")]
        [HttpPost]
        public Entities.PolicyModel.ViewModel.ResponseVM UpdateInsuredPolicy(UpdateInsuredPolicy _data)
        {

            var response = policyCORE.UpdateInsuredPolicy(_data);
            if (response.P_NCODE == "0")
            {
                var rq = new RelanzarDocumentoVM();
                rq.npolicy = (long)_data.npolicy;
                rq.nbranch = _data.nbranch;
                rq.nproduct = _data.nproduct;
                rq.nidheaderproc = _data.nidheaderproc;
                rq.suser = _data.suser;

                DeleteFiles(_data.sruta);
                var rpta = relanzarDocumento(rq);
                response.P_NCODE = rpta.P_NCODE;
                if (response.P_NCODE == "1")
                {
                    response.P_SMESSAGE = "EL ASEGURADO FUE ACTUALIZADO, PERO HUBO UN ERROR EN EL RELANZAMIENTO DE DOCUMENTOS";
                }
            }

            return response;
        }

        [Route("GetPolicyMovementsVCF")]
        [HttpPost]
        public List<MovementsModelVM> GetPolicyMovementsVCF()
        {
            var data = JsonConvert.DeserializeObject<MovementsModelVM>(HttpContext.Current.Request.Params.Get("objeto"), new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
            return policyCORE.GetPolicyMovementsVCF(data);
        }

        [Route("SavePolicyMovementsVCF")]
        [HttpPost]
        public SalidadPolizaEmit SavePolicyMovementsVCF()
        {

            var data = JsonConvert.DeserializeObject<MovementsModelVM>(HttpContext.Current.Request.Params.Get("objeto"), new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
            List<HttpPostedFile> files = new List<HttpPostedFile>();

            foreach (var item in HttpContext.Current.Request.Files)
            {
                HttpPostedFile arch = HttpContext.Current.Request.Files[item.ToString()];
                files.Add((HttpPostedFile)HttpContext.Current.Request.Files[item.ToString()]);
            }
            return policyCORE.SavePolicyMovementsVCF(data, files);
        }

        [Route("SendMailMovementsVCF")]
        [HttpPost]
        public SalidadPolizaEmit SendMailMovementsVCF()
        {

            var data = JsonConvert.DeserializeObject<MovementsModelVM>(HttpContext.Current.Request.Params.Get("objeto"), new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

            return policyCORE.SendMailMovementsVCF(data);
        }

        [Route("GetDocuments")]
        [HttpGet]
        public List<string> getDocuments(string sruta)
        {
            return sharedMethods.GetFilePathList(sruta);
        }

        [Route("GetPolicyTransactionAllList")]
        [HttpPost]
        public List<PolicyTransactionAllVM> GetPolicyTransactionAllList(PolicyTransactionAllSearchBM data)
        {
            return policyCORE.GetPolicyTransactionAllList(data);
        }

        /// <summary>
        /// Reporte de Poliza - Excel
        /// AP: Luis Sánchez Rojas - MG
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Route("GetPolicyTransactionAllListExcel")]
        [HttpPost]
        public string GetPolicyTransactionAllListExcel(PolicyTransactionAllSearchBM data)
        {
            return policyCORE.GetPolicyTransactionAllListExcel(data);
        }

        [Route("GetPolicyTransactionList")]
        [HttpPost]
        public Entities.PolicyModel.ViewModel.GenericPackageVM GetPolicyTransactionList(PolicyTransactionSearchBM data)
        {
            return policyCORE.GetPolicyTransactionList(data);
        }

        [Route("GetAccountTransactionList")]
        [HttpPost]
        public Entities.PolicyModel.ViewModel.GenericPackageVM GetAccountTransactionList(AccountTransactionSearchBM data)
        {
            return policyCORE.GetAccountTransactionList(data);
        }
        [Route("GetPaymentStateList")]
        [HttpGet]
        public Entities.PolicyModel.ViewModel.GenericPackageVM GetPaymentStateList()
        {
            return policyCORE.GetPaymentStateList();
        }
        [Route("PolizaEmitCab")]
        [HttpGet]
        public async Task<Entities.PolicyModel.ViewModel.GenericPackageVM> GetPolizaEmitCab(string nroCotizacion, string typeMovement, int userCode, int trama = 0, int FlagNoCIP = 0, int flagTecnica = 0)
        {
            var response = new GenericPackageVM();
            response.GenericResponse = new PolizaEmitCAB();
            SalidaPremiumVM response2 = new SalidaPremiumVM();
            response = await policyCORE.GetPolizaEmitCab(nroCotizacion, typeMovement, userCode, FlagNoCIP);
            var generic = (PolizaEmitCAB)response.GenericResponse;

            if (new string[] { ELog.obtainConfig("desgravamenBranch") }.Contains(generic.NBRANCH.ToString()))
            {
                response.idproc = new QuotationCORE().GetProcessCodePD(nroCotizacion);
                if (!String.IsNullOrEmpty(response.idproc))
                {
                    response2 = new QuotationCORE().ReaListDetCotiza(response.idproc, "", generic.NPRODUCT.ToString(), generic.NBRANCH.ToString());
                    response.amountPremiumListAnu = response2.amountPremiumAnu;
                    response.amountPremiumListProp = response2.amountPremiumProp;
                    response.amountPremiumListAut = response2.amountPremiumAut;
                    response.total_asegurados = response2.TOT_ASEGURADOS;
                    response.prima = response2.PRIMA;
                }
            }

            var lAutorizado = new string[] { ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch"), ELog.obtainConfig("vidaIndividualBranch") };
            if (lAutorizado.Contains(generic.NBRANCH.ToString()))
            {
                response.idproc = new QuotationCORE().GetProcessCodePD(nroCotizacion);

                if (!String.IsNullOrEmpty(response.idproc))
                {
                    var ramoRentas = new string[] { ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch") }; //AVS - RENTAS
                    var prodRentas = new string[] { "3", "7" }; //AVS - RENTAS

                    if (ramoRentas.Contains(generic.NBRANCH.ToString()) && prodRentas.Contains(generic.NPRODUCT.ToString())) //AVS - RENTAS
                    {
                        var flagDataPM = policyDA.GetDataFlagPM(response.idproc);
                        response2 = new QuotationCORE().ReaListDetCotiza(response.idproc, "", generic.NPRODUCT.ToString(), generic.NBRANCH.ToString(), flagDataPM.ToString());
                    }
                    else
                    {
                        response2 = new QuotationCORE().ReaListDetCotiza(response.idproc, "", generic.NPRODUCT.ToString(), generic.NBRANCH.ToString());
                    }

                    var objValida = generateValidaCab(generic, response2, response);

                    if (ELog.obtainConfig("accidentesBranch") == generic.NBRANCH.ToString())
                    {
                        response.cotizadorDetalleList = new QuotationCORE().detailQuotationInfo(null, objValida);
                    }
                    else
                    {
                        var aseguradoList = new QuotationCORE().GetTramaAsegurado(response.idproc);
                        if (generic.SVALIDA_TRAMA == "1" || aseguradoList.Count > 0)
                        {
                            response.cotizadorDetalleList = new QuotationCORE().detailQuotationInfo(aseguradoList, objValida);
                        }
                        else
                        {
                            response.cotizadorDetalleList = new QuotationCORE().detailQuotationInfo(null, objValida);
                        }
                    }

                    response.amountPremiumListAnu = response2.amountPremiumAnu;
                    response.amountPremiumListProp = response2.amountPremiumProp;
                    response.amountPremiumListAut = response2.amountPremiumAut;
                    response.tasa = response2.NTASA;
                    response.planilla = response2.NMONTO_PLANILLA;
                    response.total_asegurados = response2.TOT_ASEGURADOS;
                    response.prima = response2.PRIMA;
                }
            }
            return response;
        }

        private validaTramaVM generateValidaCab(PolizaEmitCAB generic, SalidaPremiumVM response2, GenericPackageVM response)
        {
            var objValida = new validaTramaVM();

            objValida.premium = response2.PRIMA;
            objValida.CantidadTrabajadores = Convert.ToInt64(response2.TOT_ASEGURADOS);
            objValida.asegPremium = response2.PRIMA_ASEG; //  Convert.ToDouble(decimal.Round(Convert.ToDecimal(objValida.premium / objValida.CantidadTrabajadores), 2));
            objValida.asegIgv = response2.IGV_ASEG;
            objValida.asegPremiumTotal = response2.PRIMAT_ASEG;
            objValida.ntasa_tariff = Convert.ToDouble(response2.NTASA);
            objValida.montoPlanilla = Convert.ToDouble(response2.NMONTO_PLANILLA);
            objValida.flagTrama = generic.SVALIDA_TRAMA;
            objValida.codUsuario = "0";
            objValida.tipoCotizacion = generic.STIPO_COTIZACION;
            objValida.codProceso = response.idproc;
            objValida.nroCotizacion = Convert.ToInt64(generic.NID_COTIZACION);
            objValida.datosPoliza = new DatosPoliza()
            {
                branch = generic.NBRANCH,
                codTipoProducto = generic.NPRODUCT.ToString(),
                codTipoPerfil = generic.NID_TYPE_PROFILE,
                codTipoNegocio = generic.NID_TYPE_PRODUCT,
                codTipoFacturacion = generic.COD_TIPO_FACTURACION,
            };
            objValida.igvPD = new QuotationCORE().GetIGV(objValida) / 100;

            //new QuotationCORE().GenerateInfoAmount(objValida.igvPD, "trx", ref objValida);

            return objValida;
        }

        [Route("CargadoBeneficiariosEC")]
        [HttpPost]
        public async Task<ResponseBaseVM> CargadoBeneficiariosEC(DatosValidarBeneficiarios request)
        {
            var response = new ResponseBaseVM();
            var responseCab = new GenericPackageVM();
            response.P_COD_ERR = 0;
            response.P_SMESSAGE = "Cargado Exitoso";
            try
            {
                //var responseCot = new SalidaTramaBaseVM() { P_COD_ERR = "0" };

                responseCab.GenericResponse = new PolizaEmitCAB();
                responseCab = await policyCORE.GetPolizaEmitCab(request.nroCotizacion.ToString(), request.typeMovement.ToString(), request.userCode);
                var generic = (PolizaEmitCAB)responseCab.GenericResponse;
                var response_error = new SalidaErrorBaseVM();

                var requestTrama = new validaTramaVM();

                requestTrama = new validaTramaVM()
                {
                    codProceso = generic.NID_PROC,
                    codProducto = generic.NPRODUCT.ToString(),
                    codUsuario = request.userCode.ToString(),
                    flagpremium = 1,
                    flag = "1",
                    type_mov = "1",
                    nroCotizacion = request.nroCotizacion,
                    codCanal = "2015000002",
                    CantidadTrabajadores = 1,
                    premium = request.premium,
                    datosPoliza = new Entities.QuotationModel.BindingModel.DatosPoliza()
                    {
                        branch = generic.NBRANCH,
                        codTipoNegocio = generic.NID_TYPE_PRODUCT,
                        codTipoPerfil = generic.NID_TYPE_PROFILE,
                        codMon = generic.COD_MONEDA,
                        typeTransac = generic.NTYPE_TRANSAC,
                        InicioVigPoliza = generic.EFECTO_COTIZACION,
                        FinVigPoliza = generic.EXPIRACION_COTIZACION,
                        InicioVigAsegurado = generic.EFECTO_ASEGURADOS,
                        FinVigAsegurado = generic.EXPIRACION_ASEGURADOS,
                        codTipoFrecuenciaPago = generic.FREQ_PAGO,
                        codTipoFacturacion = generic.COD_TIPO_FACTURACION,
                        codTipoModalidad = generic.NID_TYPE_MODALITY,
                        CodActividadRealizar = generic.COD_ACT_ECONOMICA,
                        CodCiiu = generic.ACT_TECNICA,
                        codTipoProducto = generic.NPRODUCT.ToString(),
                        codTipoRenovacion = generic.TIP_RENOV,
                        nid_cotizacion = request.nroCotizacion
                        //segmentoId = responsePlan.codSegmento
                    }
                    ,
                    datosContratante = new DatosContractor()
                    {
                        codDocumento = generic.TIPO_DOCUMENTO,
                        documento = generic.NUM_DOCUMENTO
                    }
                };


                if (request.datosBeneficiarios != null)
                {
                    foreach (var beneficiarios in request.datosBeneficiarios)
                    {
                        response_error = new QuotationCORE().InsCargaMasEspecialBeneficiarios(requestTrama, beneficiarios);

                        if (response_error.P_COD_ERR == 1)
                        {
                            response.P_COD_ERR = 1;
                            response.P_SMESSAGE = "Error al cargar beneficiarios";
                            return response;
                        }
                    }
                }
            }
            catch
            {
                response.P_COD_ERR = 1;
                response.P_SMESSAGE = "Error al cargar beneficiarios";
            }


            return response;
        }


        [Route("PolizaEmitComer")]
        [HttpGet]
        public List<PolizaEmitComer> GetPolizaEmitComer(int nroCotizacion)
        {
            return policyCORE.GetPolizaEmitComer(nroCotizacion);
        }
        [Route("PolizaEmitDet")]
        [HttpGet]
        public List<PolizaEmitDet> GetPolizaEmitDet(int nroCotizacion, int? userCode)
        {
            return policyCORE.GetPolizaEmitDet(nroCotizacion, userCode);
        }
        [Route("PolizaEmitDetTX")]
        [HttpGet]
        public List<PolizaEmitDet> GetPolizaEmitDetTX(string processId, string typeMovement, int? userCode, int? vencido)
        {
            return policyCORE.GetPolizaEmitDetTX(processId, typeMovement, userCode, vencido);
        }
        [Route("TipoRenovacion")]
        [HttpPost]
        public List<TipoRenovacion> GetTipoRenovacion(TypeRenBM request)
        {
            return policyCORE.GetTipoRenovacion(request);
        }
        [Route("FrecuenciaPago")]
        [HttpGet]
        public List<FrecuenciaPago> GetFrecuenciaPago(int codrenovacion, int producto = 0)
        {
            return policyCORE.GetFrecuenciaPago(codrenovacion, producto);
        }
        [Route("MotivoAnulacion")]
        [HttpGet]
        public List<AnnulmentVM> GetAnnulment()
        {
            return policyCORE.GetAnnulment();
        }
        [Route("TypeEndoso")]
        [HttpGet]
        public List<TypeEndoso> GetTypeEndoso()
        {
            return policyCORE.GetTypeEndoso();
        }
        [Route("PolizaCot")]
        [HttpGet]
        public List<PolicyDetVM> GetPolizaCot(int nroCotizacion, int nroTransac = 0)
        {
            return policyCORE.GetPolizaCot(nroCotizacion, nroTransac);
        }
        [Route("savedAsegurados")]
        [HttpPost]
        public async Task<SalidaPlanilla> savedAsegurados()
        {
            var response = new SalidaPlanilla();
            var objValida = new validaPlanillaBM();

            try
            {
                HttpPostedFile upload = HttpContext.Current.Request.Files["dataFile"];
                objValida = JsonConvert.DeserializeObject<validaPlanillaBM>(HttpContext.Current.Request.Params.Get("objValida"));


                if (upload != null && upload.ContentLength > 0)
                {
                    Stream stream = upload.InputStream;
                    IExcelDataReader reader = null;

                    if (upload.FileName.EndsWith(".xls"))
                    {
                        reader = ExcelReaderFactory.CreateBinaryReader(stream);
                    }
                    else if (upload.FileName.EndsWith(".xlsx"))
                    {
                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    }

                    if (reader != null)
                    {
                        DataSet result = reader.AsDataSet();
                        reader.Close();

                        if (result.Tables[0].Rows.Count > 1)
                        {
                            response = await tramaProtecta(result.Tables[0], objValida);

                            var flagSaludEPS = (objValida.codRamo == Convert.ToInt32(ELog.obtainConfig("sctrBranch")) && objValida.nroProduct == 2) ? 1 : 0;

                            /*
                            if (objValida.eps == ELog.obtainConfig("grandiaKey") && (objValida.codMixta == 1 || flagSaludEPS == 1)) //AVS - INTERCONEXION SABSA 20/09/2023
                            {
                                //NID PROC DE SCTR - SALUD PAGOEFECTIVO
                                var v_proceso_EPS = (objValida.codUser + GenerarCodigo() + DateTime.Now.ToString("yyyyMMddHHmmss") + 2).PadLeft(30, '0');

                                var data = new instPay_SCTR();
                                data.P_NID_PROC_SCTR = objValida.codMixta == 1 ? response.P_NID_PROC : "";
                                data.P_NID_PROC_EPS = objValida.codMixta == 1 ? v_proceso_EPS : response.P_NID_PROC;
                                data.P_NID_COTIZACION = objValida.nroCotizacion;
                                data.P_NTYPE_TRANSAC = objValida.type_mov;
                                data.P_MENSAJE_TR = "TRAMA ACTUALIZADA";
                                data.P_NPOLICY = objValida.nroPolizaSalud;


                                var response_EPS = new PolicyDA().Udp_quotation_EPS(data);

                                if (response_EPS.P_COD_ERR == 0)
                                {
                                    response.P_NID_PROC_EPS = v_proceso_EPS;
                                }

                                var cargaSCTR = await policyDA.GenerarCalculosSCTR(response.P_NID_PROC, response.P_NID_PROC_EPS, Convert.ToInt32(objValida.nroCotizacion), Convert.ToInt32(objValida.type_mov), objValida.date, objValida.dateFn, Convert.ToInt32(objValida.tipRenovacion), Convert.ToInt32(objValida.codUser));

                            }

                            if (objValida.eps == ELog.obtainConfig("mapfreKey") && (objValida.nroCotizacionEPS != null || objValida.nroPolizaEPS != null))
                            {
                                response = await tramaMapfre(result.Tables[0], objValida, response);
                            }*/

                            var v_proceso_EPS = (objValida.codUser + GenerarCodigo() + DateTime.Now.ToString("yyyyMMddHHmmss") + 2).PadLeft(30, '0');

                            var data = new instPay_SCTR();
                            data.P_NID_PROC_SCTR = (objValida.codMixta == 1) ? response.P_NID_PROC : (flagSaludEPS == 1) ? "" : response.P_NID_PROC;
                            data.P_NID_PROC_EPS = (objValida.codMixta == 1) ? v_proceso_EPS : (flagSaludEPS == 1) ? response.P_NID_PROC : "";
                            data.P_NID_COTIZACION = objValida.nroCotizacion;
                            data.P_NTYPE_TRANSAC = objValida.type_mov;
                            data.P_MENSAJE_TR = "TRAMA ACTUALIZADA";
                            data.P_NPOLICY = objValida.nroPolizaSalud;


                            var response_EPS = new PolicyDA().Udp_quotation_EPS(data);

                            if (response_EPS.P_COD_ERR == 0)
                            {
                                response.P_NID_PROC_EPS = data.P_NID_PROC_EPS;
                            }

                            var cargaSCTR = await policyDA.GenerarCalculosSCTR(response.P_NID_PROC, response.P_NID_PROC_EPS, Convert.ToInt32(objValida.nroCotizacion), Convert.ToInt32(objValida.type_mov), objValida.date, objValida.dateFn, Convert.ToInt32(objValida.tipRenovacion), Convert.ToInt32(objValida.codUser));

                            if (objValida.eps == ELog.obtainConfig("mapfreKey") && (objValida.nroCotizacionEPS != null || objValida.nroPolizaEPS != null))
                            {
                                response = await tramaMapfre(result.Tables[0], objValida, response);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                //response.P_MESSAGE = ex.Message + " " + objValida.nroCotizacion;
                response.P_MESSAGE = "Hubo un error al validar los valores de la trama. Favor de comunicarse con sistemas";
                LogControl.save("savedAsegurados", ex.ToString(), "3");
            }

            return response;
        }

        public async Task<SalidaPlanilla> tramaProtecta(DataTable dataTable, validaPlanillaBM objValida)
        {
            var response = new SalidaPlanilla();

            var v_proceso = (objValida.codUser + GenerarCodigo() + DateTime.Now.ToString("yyyyMMddHHmmss")).PadLeft(30, '0');

            Int32 count = 1;
            Int32 rows = Convert.ToInt32(ELog.obtainConfig("rows120"));

            DataTable dt = new DataTable();
            dt.Columns.Add("NID_COTIZACION");
            dt.Columns.Add("NID_PROC");
            dt.Columns.Add("SFIRSTNAME");
            dt.Columns.Add("SLASTNAME");
            dt.Columns.Add("SLASTNAME2");
            dt.Columns.Add("NMODULEC");
            dt.Columns.Add("NNATIONALITY");
            dt.Columns.Add("NIDDOC_TYPE");
            dt.Columns.Add("SIDDOC");
            dt.Columns.Add("DBIRTHDAT");
            dt.Columns.Add("NREMUNERACION");
            dt.Columns.Add("NIDCLIENTLOCATION");
            dt.Columns.Add("NCOD_NETEO");
            dt.Columns.Add("NUSERCODE");
            dt.Columns.Add("SSTATREGT");
            dt.Columns.Add("SCOMMENT");
            dt.Columns.Add("NID_REG");
            dt.Columns.Add("SSEXCLIEN");
            dt.Columns.Add("NACTION", typeof(int));
            dt.Columns.Add("SROLE");
            dt.Columns.Add("SCIVILSTA");
            dt.Columns.Add("SSTREET");
            dt.Columns.Add("SPROVINCE");
            dt.Columns.Add("SLOCAL");
            dt.Columns.Add("SMUNICIPALITY");
            dt.Columns.Add("SE_MAIL");
            dt.Columns.Add("SPHONE_TYPE");
            dt.Columns.Add("SPHONE");
            dt.Columns.Add("NIDDOC_TYPE_BENEFICIARY");
            dt.Columns.Add("SIDDOC_BENEFICIARY");

            while (rows < dataTable.Rows.Count)
            {
                if (dataTable.Rows[rows][5].ToString().Trim() != "" &&
                    dataTable.Rows[rows][3].ToString().Trim() != "" &&
                    dataTable.Rows[rows][4].ToString().Trim() != "" &&
                    dataTable.Rows[rows][1].ToString().Trim() != "" &&
                    dataTable.Rows[rows][2].ToString().Trim() != ""
                    )
                {
                DataRow dr = dt.NewRow();
                dr["NID_COTIZACION"] = objValida.nroCotizacion.Trim();
                dr["NID_PROC"] = v_proceso;
                dr["SFIRSTNAME"] = dataTable.Rows[rows][5].ToString().Trim() == "" ? null : dataTable.Rows[rows][5].ToString().Trim();
                dr["SLASTNAME"] = dataTable.Rows[rows][3].ToString().Trim() == "" ? null : dataTable.Rows[rows][3].ToString().Trim();
                dr["SLASTNAME2"] = dataTable.Rows[rows][4].ToString().Trim() == "" ? null : dataTable.Rows[rows][4].ToString().Trim();
                dr["NMODULEC"] = dataTable.Rows[rows][8].ToString().Trim() == "" ? "" : dataTable.Rows[rows][8].ToString().Trim().ToUpper() == "RIESGO MEDIO" ? "Flat" : dataTable.Rows[rows][8].ToString().Trim();
                dr["NNATIONALITY"] = dataTable.Rows[rows][14].ToString().Trim() == "" ? "" : dataTable.Rows[rows][14].ToString().Trim();
                dr["NIDDOC_TYPE"] = dataTable.Rows[rows][1].ToString().Trim() == "" ? "" : dataTable.Rows[rows][1].ToString().Trim();
                //dr["SIDDOC"] = dataTable.Rows[rows][2].ToString().Trim() == "" ? null : dataTable.Rows[rows][2].ToString().Trim();
                dr["SIDDOC"] = dataTable.Rows[rows][2].ToString().Trim() == "" ? null : dataTable.Rows[rows][1].ToString().Trim().ToUpper() == "DNI" && dataTable.Rows[rows][2].ToString().Trim().Length == 7 ? dataTable.Rows[rows][2].ToString().Trim().PadLeft(8, '0') : dataTable.Rows[rows][2].ToString().Trim();
                var fecha = dataTable.Rows[rows][7].ToString().Trim().Split(' ').ToList();
                var fnacimiento = fecha.Count() == 1 ? fecha[0] : Convert.ToDateTime(dataTable.Rows[rows][7].ToString()).ToString("dd/MM/yyyy");
                dr["DBIRTHDAT"] = fecha[0] == String.Empty ? null : fnacimiento;
                dr["NREMUNERACION"] = dataTable.Rows[rows][9].ToString().Trim() == "" ? "0" : dataTable.Rows[rows][9].ToString().Trim();
                dr["SE_MAIL"] = dataTable.Rows[rows][10].ToString().Trim() == "" ? null : dataTable.Rows[rows][10].ToString().Trim();
                dr["SPHONE_TYPE"] = dataTable.Rows[rows][11].ToString().Trim() == "" ? null : "Celular";
                dr["SPHONE"] = dataTable.Rows[rows][11].ToString().Trim() == "" ? null : dataTable.Rows[rows][11].ToString().Trim();
                dr["NIDCLIENTLOCATION"] = dataTable.Rows[rows][12].ToString().Trim() == "" ? "" : dataTable.Rows[rows][12].ToString().Trim();
                dr["NCOD_NETEO"] = dataTable.Rows[rows][13].ToString().Trim() == String.Empty ? null : dataTable.Rows[rows][13].ToString().Trim().Length > 5 ? dataTable.Rows[rows][13].ToString().Trim().Substring(0, 5) : dataTable.Rows[rows][13].ToString().Trim();
                dr["NUSERCODE"] = objValida.codUser.Trim();
                dr["SSTATREGT"] = "1";
                dr["SCOMMENT"] = null;
                dr["NID_REG"] = count.ToString();
                dr["SSEXCLIEN"] = dataTable.Rows[rows][6].ToString().Trim() == "" ? null : dataTable.Rows[rows][6].ToString().Trim();
                dr["NACTION"] = DBNull.Value;
                dr["NIDDOC_TYPE_BENEFICIARY"] = null;
                dr["SIDDOC_BENEFICIARY"] = null;
                rows++;
                count++;
                dt.Rows.Add(dr);
            }
                else
                {
                    break;
                }
            }

            var responseTrama = new QuotationCORE().SaveUsingOracleBulkCopy(dt);

            response = new SalidaPlanilla()
            {
                P_COD_ERR = Convert.ToInt32(responseTrama.P_COD_ERR),
                P_MESSAGE = responseTrama.P_MESSAGE
            };

            if (response.P_COD_ERR == 0)
            {
                InsuredValBM insuredVal = new InsuredValBM();
                insuredVal.P_NID_PROC = v_proceso;
                insuredVal.P_NUSERCODE = Convert.ToInt32(objValida.codUser);
                insuredVal.P_NTYPE_TRANSAC = objValida.type_mov;
                insuredVal.P_NRETARIF = Convert.ToInt32(objValida.retarif);
                insuredVal.P_DEFFECDATE = Convert.ToDateTime(objValida.date);
                insuredVal.P_NID_COTIZACION = Convert.ToInt64(objValida.nroCotizacion);
                //response = policyCORE.InsuredVal(insuredVal);

                #region Guardado de asegurados - Cliente360
                var response360 = await new QuotationController().consultaAsegurados360(dt);

                //if (response360 != null)
                //{
                if (response360.baseError.errorList.Count > 0)
                {
                    foreach (var item in response360.baseError.errorList)
                    {
                        var error = new VMpolicy.ErroresVM()
                        {
                            REGISTRO = item.REGISTRO,
                            DESCRIPCION = item.DESCRIPCION
                        };

                        response.C_TABLE.Add(error);
                    }
                }

                response = policyCORE.InsuredVal(insuredVal);
                //}
                #endregion

                response.P_NID_PROC = v_proceso;
                response.P_COD_ERR = response.C_TABLE.Count > 0 ? 0 : response.P_COD_ERR;
            }  // Error en VAL_TRAMA_COT

            //response.P_NID_PROC = v_proceso;
            //response.P_COD_ERR = 0;
            //response.C_TABLE = new List<VMpolicy.ErroresVM>();

            return await Task.FromResult(response);
        }

        public async Task<SalidaPlanilla> tramaMapfre(DataTable dataTable, validaPlanillaBM objValida, SalidaPlanilla responseProtecta)
        {
            ValidarAseguradosBM objMapfre = new ValidarAseguradosBM();
            objMapfre.numCotizacion = objValida.nroCotizacionEPS;
            objMapfre.cabecera = new CabeceraBM();
            objMapfre.cabecera.keyService = "validarAsegurado";

            objMapfre.listaAsegurado = new List<AseguradoBM>();

            int rows = Convert.ToInt32(ELog.obtainConfig("rows120"));
            while (rows < dataTable.Rows.Count)
            {
                var dr = new AseguradoBM();
                dr.numRiesgoEnlace = 1;
                dr.nomCategoriaEnlace = ELog.obtainConfig("nomCategoriaMP");
                dr.tipDocum = dataTable.Rows[rows][1].ToString().Trim() == String.Empty ? null : await mapfreController.EquivalenciaMapfreWS(dataTable.Rows[rows][1].ToString().Trim(), "tipDocumento", "tableKey"); // Llama equivalencia
                dr.codDocum = dataTable.Rows[rows][2].ToString().Trim() == String.Empty ? null : dataTable.Rows[rows][2].ToString().Trim();
                dr.nombre = dataTable.Rows[rows][5].ToString().Trim() == String.Empty ? null : dataTable.Rows[rows][5].ToString().Trim();
                dr.apePaterno = dataTable.Rows[rows][3].ToString().Trim() == String.Empty ? null : dataTable.Rows[rows][3].ToString().Trim();
                dr.apeMaterno = dataTable.Rows[rows][4].ToString().Trim() == String.Empty ? null : dataTable.Rows[rows][4].ToString().Trim();
                string[] fecha = dataTable.Rows[rows][7].ToString().Trim().Split(' ');
                dr.fecNacimiento = fecha[0] == String.Empty ? null : fecha[0];
                double salarioMensual = dataTable.Rows[rows][9].ToString().Trim() == String.Empty ? 0 : Convert.ToDouble(dataTable.Rows[rows][9].ToString().Trim());
                dr.impSalario = salarioMensual * Convert.ToInt64(ELog.obtainConfig("renovacion" + objValida.tipRenovacion));
                rows++;
                objMapfre.listaAsegurado.Add(dr);
            }

            objMapfre.trama = new TramaBM();
            objMapfre.trama.mcaValidaNroAseg = ELog.obtainConfig("mcaValidar");
            objMapfre.trama.numAsegurados = (rows - 1).ToString();
            objMapfre.trama.mcaExoneraSueldoMin = objValida.nroCotizacionEPS != null ? ELog.obtainConfig("mcaExoneraSueldoC") : ELog.obtainConfig("mcaExoneraSueldoP");
            objMapfre.trama.mcaValidaSueldoTope = objValida.nroCotizacionEPS != null ? ELog.obtainConfig("mcaSueldoTopeC") : ELog.obtainConfig("mcaSueldoTopeP");
            objMapfre.contratante = new ContratanteBM();
            objMapfre.contratante.tipDocum = await mapfreController.EquivalenciaMapfreWS(objValida.tipDocumento, "tipDocumento", "tableKey"); // Llama equivalencia
            objMapfre.contratante.codDocum = objValida.nroDocumento;

            //if (objValida.nroPolizaEPS != null)
            //{
            objMapfre.poliza = new PolizaBM();
            objMapfre.poliza.numPolizaEnlace = objValida.nroPolizaEPS;
            objMapfre.poliza.fecEfecSpto = objValida.date;
            objMapfre.poliza.fecVctoSpto = objValida.dateFn;
            //}

            var resMapfre = await mapfreController.ValAseguradoMapfre(objMapfre);

            if (resMapfre.codError != "0" && resMapfre.codError != "2")
            {
                if (resMapfre.asegurado != null && resMapfre.asegurado.Count > 0)
                {
                    if (responseProtecta.C_TABLE == null)
                    {
                        responseProtecta.C_TABLE = new List<VMpolicy.ErroresVM>();
                    }

                    foreach (var item in resMapfre.asegurado)
                    {
                        var error = new VMpolicy.ErroresVM();
                        error.REGISTRO = item.numFilia.ToString();
                        error.DESCRIPCION = item.columna + " " + item.descripcion;
                        responseProtecta.C_TABLE.Add(error);
                    }
                }
            }

            responseProtecta.P_COD_ERR = Convert.ToInt32(resMapfre.codError);
            responseProtecta.P_MESSAGE = resMapfre.descError;
            responseProtecta.P_NID_PROC_EPS = responseProtecta.C_TABLE.Count == 0 ? resMapfre.nroMovimiento : null; //prod
            //responseProtecta.P_NID_PROC_EPS = responseProtecta.C_TABLE.Count == 0 ? (objValida.codUser + DateTime.Now.ToString("yyyyMMddHHmmss")).PadLeft(30, '0') : null;

            return responseProtecta;
        }

        [Route("TransactionPolicy")]
        [HttpPost]
        public async Task<SalidadPolizaEmit> TransactionPolicy()
        {
            var response = new SalidadPolizaEmit() { P_COD_ERR = 0 };
            var response2 = new SalidadPolizaEmit();    //R.P.

            try
            {
                List<HttpPostedFile> adjuntoList = null;
                if (HttpContext.Current.Request.Files.Count > 0)
                {
                    adjuntoList = new List<HttpPostedFile>(HttpContext.Current.Request.Files.GetMultiple("adjuntos"));
                }

                LogControl.save("TransactionPolicy", HttpContext.Current.Request.Params.Get("transaccionProtecta"), "2");
                // Objeto Protecta
                var requestProtecta = HttpContext.Current.Request.Params.Get("transaccionProtecta") != null ? JsonConvert.DeserializeObject<PolicyTransactionSaveBM>(HttpContext.Current.Request.Params.Get("transaccionProtecta")) : null;

                LogControl.save("TransactionPolicy", JsonConvert.SerializeObject(requestProtecta), "2");

                // Objeto Mapfre
                var requestMapfre = HttpContext.Current.Request.Params.Get("transaccionMapfre") != null ? JsonConvert.DeserializeObject<DeclararBM>(HttpContext.Current.Request.Params.Get("transaccionMapfre")) : null;

                // Transacciones Mapfre
                if (requestMapfre != null)
                {
                    response = await generarTransaccionMapfre(requestMapfre, requestProtecta);
                }

                // Objeto Broker R.P.
                var requestBroker = HttpContext.Current.Request.Params.Get("transaccionBroker") != null ? JsonConvert.DeserializeObject<WSPlataforma.Entities.TransactModel.RequestTransact>(HttpContext.Current.Request.Params.Get("transaccionBroker")) : null;

                if (requestBroker != null)
                {
                    foreach (var item in requestBroker.P_DAT_BROKER)
                    {
                        response2 = policyCORE.UpdateNLocalBroker(item, requestProtecta.P_NUSERCODE);
                    }
                }

                var requestNotaCredito = HttpContext.Current.Request.Params.Get("notaCredito") != null && HttpContext.Current.Request.Params.Get("notaCredito") != "null" ? JsonConvert.DeserializeObject<List<NotaCreditoModel>>(HttpContext.Current.Request.Params.Get("notaCredito")) : null;
                response = response.P_COD_ERR == 0 ? insertNotaCreditoTemp(Convert.ToInt32(requestProtecta.P_NBRANCH), requestNotaCredito, response) : response;

                if (requestProtecta.P_NEPS == Convert.ToInt32(ELog.obtainConfig("grandiaKey")) && requestProtecta.P_NTYPE_TRANSAC != 7 && requestProtecta.P_NTYPE_TRANSAC != 6 && (requestProtecta.P_NCOT_MIXTA == 1 || Convert.ToInt32(requestProtecta.P_NPRODUCTO) == 2)) /*AVS - INTERCONEXION SABSA */
                {
                    LogControl.save("generarTransaccionEPS", "Entro en Transaccion " + requestProtecta.P_NID_COTIZACION.ToString(), "1");

                    response = await generarTransaccionEPS(requestProtecta);
                }

                // Transacciones Protecta
                if (response.P_COD_ERR == 0)
                {
                    #region Eliminar registro de process payment
                    if (requestProtecta.P_SPAGO_ELEGIDO == "directo")
                    {
                        new QuotationCORE().deleteProcess(requestProtecta.P_NID_PROC);
                    }
                    #endregion

                    var lRetro = new string[] { ELog.obtainConfig("vidaLeyBranch"), ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch") };

                    /*if (requestProtecta.P_NTYPE_TRANSAC != 8 && lRetro.Contains(requestProtecta.P_NBRANCH))
                    {
                        response = await ValidateRetroactivity(requestProtecta.P_NID_COTIZACION, requestProtecta.P_NUSERCODE, requestProtecta.P_DSTARTDATE_ASE, requestProtecta.P_STRAN, requestProtecta.FlagCambioFecha, requestProtecta.P_NBRANCH, requestProtecta.P_NPRODUCTO);
                    }*/

                    if (requestProtecta.P_NTYPE_TRANSAC == 8 && requestProtecta.P_NBRANCH != ELog.obtainConfig("sctrBranch"))
                    {
                        response = policyCORE.insertarHistorial(requestProtecta);
                    }

                    if (response.P_COD_ERR == 0)
                    {
                        response = policyCORE.transactionSave(requestProtecta, adjuntoList);
                        LogControl.save("TransactionPolicy", JsonConvert.SerializeObject(response), "2");
                    }
                }

            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                //response.P_MESSAGE = ex.ToString();
                response.P_MESSAGE = "Hubo un error al generar los recibos. Favor de comunicarse con sistemas";
                LogControl.save("TransactionPolicy", ex.ToString(), "3");
            }

            return response;
        }

        [Route("ReverHISDETSCTR")]
        [HttpPost]
        public ErrorCode ReverHISDETSCTR(int P_NID_COTIZACION, int P_NMOVEMENT_PH, string P_NID_PROC)
        {
            return policyCORE.ReverHISDETSCTR(P_NID_COTIZACION, P_NMOVEMENT_PH, P_NID_PROC);
        }

        private async Task<SalidadPolizaEmit> generarTransaccionMapfre(DeclararBM requestMapfre, PolicyTransactionSaveBM requestProtecta)
        {
            var response = new SalidadPolizaEmit();

            try
            {
                var responseMapfre = await mapfreController.DeclararMapfre(requestMapfre);

                if (responseMapfre.codError == ELog.obtainConfig("codigo_OK"))
                {
                    var transaccionMapfre = new DeclararMpBM();
                    transaccionMapfre.nproduct = ELog.obtainConfig("saludKey");
                    transaccionMapfre.spolicy_mpe = requestMapfre.poliza.numPolizaEnlace;
                    transaccionMapfre.sconstancia_mpe = responseMapfre.numConstancia;
                    transaccionMapfre.stipmovimiento = ELog.obtainConfig(String.Format(ELog.obtainConfig("equiTransaccionKey"), requestMapfre.tipoMovimiento));
                    transaccionMapfre.snumrecibo_mpe = responseMapfre.recibosSalud != null ? responseMapfre.recibosSalud.recibo.Count > 0 ? responseMapfre.recibosSalud.recibo[0].numRecibo : null : null;
                    transaccionMapfre.nimprecibo_mpe = responseMapfre.recibosSalud != null ? responseMapfre.recibosSalud.recibo.Count > 0 ? responseMapfre.recibosSalud.recibo[0].impRecibo != null ? Convert.ToDecimal(responseMapfre.recibosSalud.recibo[0].impRecibo) : 0 : 0 : 0;
                    transaccionMapfre.sefecrecibo_mpe = responseMapfre.recibosSalud != null ? responseMapfre.recibosSalud.recibo.Count > 0 ? responseMapfre.recibosSalud.recibo[0].fecEfecRecibo : String.Empty : String.Empty;
                    transaccionMapfre.svctorecibo_mpe = responseMapfre.recibosSalud != null ? responseMapfre.recibosSalud.recibo.Count > 0 ? responseMapfre.recibosSalud.recibo[0].fecVctoRecibo : String.Empty : String.Empty;
                    transaccionMapfre.ncuotarec_mpe = responseMapfre.recibosSalud != null ? responseMapfre.recibosSalud.recibo.Count > 0 ? responseMapfre.recibosSalud.recibo[0].numCuota : String.Empty : String.Empty;
                    transaccionMapfre.nusercode = requestProtecta.P_NUSERCODE.ToString();

                    var declararMapfre = await mapfreController.DeclararPolizaMapfre(transaccionMapfre);

                    response.P_COD_ERR = declararMapfre.codError == "0" ? Convert.ToInt32(declararMapfre.codError) : 1;
                    response.P_MESSAGE = declararMapfre.mensaje;

                    if (requestProtecta.P_NID_PENSION == ELog.obtainConfig("codigo_OK"))
                    {
                        response.P_NCONSTANCIA = Convert.ToInt64(responseMapfre.numConstancia);
                    }
                }
                else
                {
                    response.P_COD_ERR = Convert.ToInt32(ELog.obtainConfig("codigo_NO"));
                    response.P_MESSAGE = "Error servicio Mapfre";
                }
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = Convert.ToInt32(ELog.obtainConfig("codigo_NO"));
                response.P_MESSAGE = ex.Message;
                LogControl.save("generarTransaccionMapfre", ex.ToString(), "2");
            }

            return response;
        }

        public async Task<SalidadPolizaEmit> generarTransaccionEPS(PolicyTransactionSaveBM requestProtecta) /*AVS - INTERCONEXION SABSA*/
        {
            var response = new SalidadPolizaEmit();
            var responseObject = new Response_EPS_Transaccion();
            var tipo_transaccion = requestProtecta.P_NTYPE_TRANSAC;
            var asincObject = new Response_EPS_Transaccion();
            var transaccion = String.Empty;

            if (tipo_transaccion == 2)
            {
                transaccion = "INCLUSION";
            }
            else if (tipo_transaccion == 3)
            {
                transaccion = "EXCLUSION";
            }
            else if (tipo_transaccion == 4)
            {
                transaccion = "RENOVACION";
            }
            else if (tipo_transaccion == 11)
            {
                transaccion = "DECLARACION";
            }
            else if (tipo_transaccion == 9)
            {
                transaccion = "GEN. FACTURACION";
            }
            else if (tipo_transaccion == 8)
            {
                transaccion = "ENDOSO";
            }

            var request_ins_aseg = new CargaAsegEPS
            {
                P_NID_COTIZACION = requestProtecta.P_NID_COTIZACION,
                P_NID_PROC = requestProtecta.P_NID_PROC,
                P_NUSERCODE = requestProtecta.P_NUSERCODE,
                P_NPAYFREQ = requestProtecta.P_NPAYFREQ,
                P_NBRANCH = Convert.ToInt32(requestProtecta.P_NBRANCH),
                P_NPRODUCTO = Convert.ToInt32(requestProtecta.P_NPRODUCTO),
                P_DSTARTDATE_POL = requestProtecta.P_DSTARTDATE_POL,
                P_DEXPIRDAT_POL = requestProtecta.P_DEXPIRDAT_POL,
                P_DSTARTDATE_ASE = requestProtecta.P_DEFFECDATE,
                P_DEXPIRDAT_ASE = requestProtecta.P_DEXPIRDAT,
                P_NID_PROC_EPS = requestProtecta.P_NID_PROC_EPS
            };

            try
            {
                if (requestProtecta != null)
                {
                    string cotizacion = requestProtecta.P_NID_COTIZACION.ToString();
                    string codigoCip = policyDA.GetCodigoCip(cotizacion);
                    string fechaPago = policyDA.GetFechaPago(Convert.ToInt32(cotizacion));
                    List<riesgos> listaRiesgos = requestProtecta.P_NTYPE_TRANSAC != 6 && requestProtecta.P_NTYPE_TRANSAC != 7 ? policyDA.GetRiesgos(cotizacion) : null;
                    List<asegurados> listaAsegurados = requestProtecta.P_NTYPE_TRANSAC != 6 && requestProtecta.P_NTYPE_TRANSAC != 7 ? policyDA.GetAsegurados(requestProtecta.P_NID_PROC) : null;

                    var epsTr = new dataTransaccion();
                    epsTr.codigoCotizacion = cotizacion;
                    epsTr.codigoProceso = requestProtecta.P_NID_PROC.ToString();
                    epsTr.codigoContrato = requestProtecta.P_NPOLICY_SALUD.ToString();
                    epsTr.fechaVencimientoPago = fechaPago == "" ? null : DateTime.ParseExact(fechaPago, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                    epsTr.fechaEfectoAseguradoRecibo = DateTime.ParseExact(requestProtecta.P_DEFFECDATE, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                    epsTr.fechaExpiracionAseguradoRecibo = DateTime.ParseExact(requestProtecta.P_DEXPIRDAT, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                    epsTr.fechaEfectoPoliza = DateTime.ParseExact(requestProtecta.P_DSTARTDATE_POL, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                    epsTr.fechaExpiracionPoliza = DateTime.ParseExact(requestProtecta.P_DEXPIRDAT_POL, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                    epsTr.asignacionActividadAltoRiesgo = (requestProtecta.P_SDELIMITER == 1) ? true : false;
                    epsTr.codigoMoneda = requestProtecta.P_NCURRENCY.ToString();
                    epsTr.codigoFrecuenciaPago = requestProtecta.P_SCOLTIMRE.ToString();
                    epsTr.codigoFrecuenciaRenovacion = requestProtecta.P_NPAYFREQ.ToString();
                    epsTr.codigoFormaPago = requestProtecta.P_PAYPF.ToString(); //AVS - INTERCONEXION SABSA REVISAR CODIGO DE PAGO

                    LogControl.save("generarPolizaEPS", JsonConvert.SerializeObject(epsTr), "2");

                    //AGF SABSA 27042024 Integracion KUSHKI Cash o transferencia
                    if (epsTr.codigoFormaPago == "3") { 
                        epsTr.codigoFormaPago = "1";
                    }
                    epsTr.asignacionFacturacionAnticipada = (requestProtecta.P_SFLAG_FAC_ANT == 1) ? true : false;
                    epsTr.asignacionRegulaMesVencido = (requestProtecta.P_FACT_MES_VENCIDO == 1) ? true : false;
                    epsTr.codigoTipoTransaccion = tipo_transaccion.ToString();
                    epsTr.fechaTransaccion = DateTime.Now.ToString("yyyy-MM-dd");
                    epsTr.codigoUsuarioRegistro = requestProtecta.P_NUSERCODE.ToString();
                    epsTr.primaMinimaAutorizada = decimal.Parse(requestProtecta.P_NPREM_MINIMA);
                    epsTr.primaComercial = decimal.Parse(requestProtecta.P_NPREM_NETA);
                    epsTr.igv = decimal.Parse(requestProtecta.P_IGV);
                    epsTr.derechoEmision = (decimal)requestProtecta.P_NDE;
                    epsTr.primaTotal = decimal.Parse(requestProtecta.P_NPREM_BRU);
                    epsTr.cipPagoEfectivo = codigoCip ?? "";
                    epsTr.riesgos = listaRiesgos;
                    epsTr.asegurados = listaAsegurados;

                    var URI_SendEmision_EPS = ELog.obtainConfig("urlEmisionEPS_SCTR");

                    new QuotationDA().InsertLog(Convert.ToInt64(requestProtecta.P_NID_COTIZACION.ToString()), "01 - SE ENVIA JSON EPS - " + transaccion, URI_SendEmision_EPS, JsonConvert.SerializeObject(epsTr), null);

                    var json = Task.Run(async () =>
                    {
                        var jsonTsk = await new PolicyDA().invocarServicio_EPS_TRA(Convert.ToInt32(cotizacion), JsonConvert.SerializeObject(epsTr), transaccion);

                        asincObject = JsonConvert.DeserializeObject<Response_EPS_Transaccion>(jsonTsk);

                        bool isSuccess = asincObject == null ? false : asincObject.success;

                        if (!isSuccess)
                        {
                            string errorMessage = "Servicio EPS - SCTR - transacción: " + asincObject.message;
                            new QuotationDA().InsertLog(Convert.ToInt64(requestProtecta.P_NID_COTIZACION.ToString()), "03 - ERROR SERVICIO EPS - " + transaccion, URI_SendEmision_EPS, JsonConvert.SerializeObject(asincObject), null);
                            new PolicyDA().insertErrorEPS(cotizacion, JsonConvert.SerializeObject(epsTr), requestProtecta.P_NID_PROC_EPS, requestProtecta.P_NID_PROC);
                        }
                        else
                        {
                            string mensaje = "SE REALIZO LA TRANSACCIÓN CORRECTAMENTE";
                            var insEPS = new PolicyDA().insert_policy_EPS(long.Parse(requestProtecta.P_NPOLICY_SALUD), asincObject.data.idContrato, requestProtecta.P_NCOT_MIXTA == 1 ? requestProtecta.P_NID_PROC_EPS : requestProtecta.P_NID_PROC, mensaje);
                            var insDocEPSList = new List<Ins_Documentacion_EPS>();

                            foreach (var data in asincObject.data.documentos)
                            {
                                var insDocEPS = new Ins_Documentacion_EPS
                                {
                                    P_NID_PROC_EPS = requestProtecta.P_NID_PROC,
                                    P_NBRANCH = Convert.ToInt32(requestProtecta.P_NBRANCH),
                                    P_NPRODUCT = Convert.ToInt32(requestProtecta.P_NPRODUCTO),
                                    P_NID_COTIZACION = Convert.ToInt32(cotizacion.ToString()),
                                    P_NPOLICY = long.Parse(requestProtecta.P_NPOLICY_SALUD),
                                    P_ID_DOCUMENTO = data.idDocumento,
                                    P_DES_DOCUMENTO = data.nombreDocumento,
                                    P_NSTATE = 1,
                                    P_MENSAJE_IMP = "TRABAJO INSERTADO",
                                    P_NTYPE_TRANSAC = requestProtecta.P_NTYPE_TRANSAC
                                };

                                insDocEPSList.Add(insDocEPS);
                            }

                            foreach (var docEPS in insDocEPSList)
                            {
                                var result = policyDA.Ins_Documentos_EPS(docEPS);
                            }

                            policyDA.UPD_TRX_CARGA_DEFINITIVA(insDocEPSList[0]);
                        }

                    });

                    new PolicyDA().insertErrorEPS(cotizacion, JsonConvert.SerializeObject(epsTr), requestProtecta.P_NID_PROC_EPS, requestProtecta.P_NID_PROC);
                }
            }
            catch (Exception ex)
            {
                LogControl.save("generarPolizaEPS", ex.ToString(), "3");
            }

            return response;
        }

        [Route("valBilling")]
        [HttpPost]
        public VMpolicy.ResponseVM valBilling(ValBillingBM data)
        {
            VMpolicy.ResponseVM response = new VMpolicy.ResponseVM();
            try
            {
                response = policyCORE.valBilling(data);
            }
            catch (Exception ex)
            {
                response.P_NCODE = "1";
                response.P_SMESSAGE = ex.Message;
            }

            return response;
        }

        [Route("RenewMod")]
        [HttpPost]
        public async Task<QuotationResponseVM> RenewMod(QuotationCabBM data)
        {
            var response = new QuotationResponseVM();

            var lAccess = new string[] { ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch"), ELog.obtainConfig("sctrBranch") };

            if (data.TrxCode == "EN")
            {
                if (lAccess.Contains(data.P_NBRANCH.ToString()))
                {
                    response = policyCORE.RenewMod(data);
                    //response = policyCORE.AccPerEndoso(data);
                }
                else
                {
                    if (data.P_NBRANCH == Convert.ToInt32(ELog.obtainConfig("vidaLeyBranch")))
                    {
                        response = policyCORE.ProcesarEndosoVL(data);
                        if (response.P_SAPROBADO == "A")
                        {
                            var success = await SendNotificationToDevmente(data, data.P_SRUTA);
                            if (!success)
                            {
                                response.P_SMESSAGE = response.P_SMESSAGE
                                    + Environment.NewLine
                                    + "Hubo un error en el envió de la cotización a técnica. Favor de comunicarse con sistemas.";
                            }
                        }
                    }
                }
            }
            else
            {
                response = policyCORE.RenewMod(data);
                if (data.TrxCode == "EX" && ELog.obtainConfig("vidaLeyBranch") == data.P_NBRANCH.ToString()) // Revisar si solo es VL
                {
                    if (response.P_COD_ERR == 0)
                    {
                        TransactCORE transactCORE = new TransactCORE();
                        transactCORE.UpdateDevolvPri(data.NumeroCotizacion.Value, data.P_DEVOLVPRI);
                    }
                }

                // Validar retroactividad
                var lRamosR = new string[] { ELog.obtainConfig("vidaLeyBranch"), ELog.obtainConfig("accidentesBranch"),
                                                 ELog.obtainConfig("vidaGrupoBranch")};

                if (response.P_SAPROBADO == "A" && lRamosR.Contains(data.P_NBRANCH.ToString()))
                {
                    var success = await SendNotificationToDevmente(data);
                    if (!success)
                    {
                        response.P_SMESSAGE = response.P_SMESSAGE
                            + Environment.NewLine
                            + "Hubo un error en el envió de la cotización a técnica. Favor de comunicarse con sistemas.";
                    }
                }
            }

            return response;
        }

        [Route("RenewModFiles")]
        [HttpPost]
        public async Task<QuotationResponseVM> RenewModFiles()
        {
            var response = new QuotationResponseVM();
            var data = JsonConvert.DeserializeObject<QuotationCabBM>(HttpContext.Current.Request.Params.Get("objeto"), new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
            var lAccess = new string[] { ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch"), ELog.obtainConfig("sctrBranch") };

            List<HttpPostedFile> files = new List<HttpPostedFile>();
            foreach (var item in HttpContext.Current.Request.Files)
            {
                HttpPostedFile arch = HttpContext.Current.Request.Files[item.ToString()];
                files.Add((HttpPostedFile)HttpContext.Current.Request.Files[item.ToString()]);
            }

            if (files.Count > 0)
            {
                string folderPath = files.Count > 0 ? ELog.obtainConfig("pathCotizacion") + data.NumeroCotizacion + "\\" + ELog.obtainConfig("movimiento") + "\\" + DateTime.Now.ToString("yyyyMMddhhmmss") + "\\" : String.Empty;
                response = sharedMethods.ValidatePath<QuotationResponseVM>(response, folderPath, files);
                if (response.P_COD_ERR == 1)
                {
                    return response;
                }
            }

            if (data.TrxCode == "EN")
            {
                if (lAccess.Contains(data.P_NBRANCH.ToString()))
                {
                    response = policyCORE.RenewMod(data);
                    //response = policyCORE.AccPerEndoso(data);
                }
                else
                {
                    // Endosos tecnica JTV 10012023
                    if (data.P_STRAN == "RC")
                        data.recotizacion = 1;
                    // Endosos tecnica JTV 10012023

                    if (data.P_NBRANCH == Convert.ToInt32(ELog.obtainConfig("vidaLeyBranch")))
                    {
                        response = policyCORE.ProcesarEndosoVL(data);
                        if (response.P_SAPROBADO == "A")
                        {
                            var success = await SendNotificationToDevmente(data, data.P_SRUTA);
                            if (!success)
                            {
                                response.P_SMESSAGE = response.P_SMESSAGE
                                    + Environment.NewLine
                                    + "Hubo un error en el envió de la cotización a técnica. Favor de comunicarse con sistemas.";
                            }
                        }
                    }
                }
            }
            else
            {
                response = policyCORE.RenewMod(data);
                if (data.TrxCode == "EX" && ELog.obtainConfig("vidaLeyBranch") == data.P_NBRANCH.ToString())
                {
                    if (response.P_COD_ERR == 0)
                    {
                        TransactCORE transactCORE = new TransactCORE();
                        transactCORE.UpdateDevolvPri(data.NumeroCotizacion.Value, data.P_DEVOLVPRI);
                    }
                }

                // Validar retroactividad
                var lRamosR = new string[] { ELog.obtainConfig("vidaLeyBranch"), ELog.obtainConfig("accidentesBranch"),
                                                 ELog.obtainConfig("vidaGrupoBranch"), ELog.obtainConfig("sctrBranch") };

                if (response.P_SAPROBADO == "A" && lRamosR.Contains(data.P_NBRANCH.ToString()))
                {
                    var success = await SendNotificationToDevmente(data, data.P_SRUTA);
                    if (!success)
                    {
                        response.P_SMESSAGE = response.P_SMESSAGE
                            + Environment.NewLine
                            + "Hubo un error en el envió de la cotización a técnica. Favor de comunicarse con sistemas.";
                    }
                }
            }

            if (data.P_NBRANCH.ToString() == "73")
            {
                new QuotationDA().PD_PROCESA_RANGO_X_ASOCIAR(Convert.ToInt32(data.NumeroCotizacion.Value.ToString()));
            }

            return response;
        }

        private async Task<bool> SendNotificationToDevmente(QuotationCabBM cotizacion, string ruta = "")
        {
            var success = false;
            string trx_code = cotizacion.IsDeclare ? "DECLARACION" : cotizacion.TrxCode;

            // Envio de notificacion al cotizador
            var lRamosN = new string[] { ELog.obtainConfig("vidaLeyBranch") };

            // Envio de notificacion al cotizador Graphql
            var lRamosGraphql = new string[] { ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch") };

            if (lRamosN.Contains(cotizacion.P_NBRANCH.ToString()))
            {
                var responseVM = await new IntegrateToDevmente().Execute(trx_code, cotizacion.NumeroCotizacion, ruta, cotizacion.TipoEndoso); // JDD Endosos 20220930
                success = responseVM.ErrorCode == 0 ? true : false;
            }

            // Enviar notificacion
            if (lRamosGraphql.Contains(cotizacion.P_NBRANCH.ToString()))
            {
                var responseGraphql = await new QuotationCORE().SendDataQuotationGraphql(cotizacion.NumeroCotizacion.ToString(), cotizacion.TrxCode, 0, ruta);
                success = responseGraphql.codError == 0 ? true : false;
            }

            return success;
        }

        [Route("downloadExcel")]
        [HttpPost]
        public ResponseAccountStatusVM DownloadExcels(GetTrama data)
        {

            var response = new PolicyCORE().ExecuteExcelDemo(data);

            return response;

        }

        [Route("insertDETPF")]
        [HttpPost]
        public Task<ErrorCode> InsertDetPF() /*AVS - INTERCONEXION SABSA*/
        {
            var errorCode = new ErrorCode();
            var objetoDetParam = HttpContext.Current.Request.Params.Get("objetoDet");
            var requestSCTR = !string.IsNullOrEmpty(objetoDetParam) && objetoDetParam != "null" ? JsonConvert.DeserializeObject<List<dataQuotation>>(objetoDetParam) : null;

            try
            {
                if (requestSCTR != null)
                {
                    //int cotizacion = int.Parse(requestSCTR[0].NID_COTIZACION);
                    var norder = policyDA.GetOrderDet(requestSCTR[0].NID_COTIZACION);

                    foreach (var policyDetSCTR in requestSCTR)
                    {
                        errorCode = policyDA.InsertEmicionDetV2(policyDetSCTR, norder);
                    }

                    if (errorCode.P_COD_ERR == 1)
                    {
                        throw new Exception("Fallo en InsertEmicionDetV2");
                    }
                }
            }
            catch (Exception ex)
            {
                errorCode.P_COD_ERR = 1;
                errorCode.P_MESSAGE = "Hubo un error al insertar la informacion del detalle de la transacción. Favor de comunicarse con sistemas";
                LogControl.save("insertDetPF", ex.ToString(), "3");
            }

            return Task.FromResult(errorCode);
        }

        [Route("ValidateAgencySCTR")]
        [HttpPost]
        public async Task<ValAgencySCTR> ValidateAgencySCTR(ValidateAgencySCTR request)
        {
            return await policyCORE.ValidateAgencySCTR(request);
        }

        [Route("SavedPolicyEmit")]
        [HttpPost]
        public async Task<SalidadPolizaEmit> SavePolizaEmit()
        {
            var response = new SalidadPolizaEmit() { P_COD_ERR = 0 };

            Int64 poliza1 = 0;
            Int64 poliza2 = 0;
            int nidpaymentVC = 0;

            try
            {
                List<HttpPostedFile> adjuntoList = null;
                if (HttpContext.Current.Request.Files.Count > 0)
                {
                    adjuntoList = new List<HttpPostedFile>(HttpContext.Current.Request.Files.GetMultiple("adjuntos"));
                }

                // Objeto Emisión Protecta
                LogControl.save("SavedPolicyEmit", HttpContext.Current.Request.Params.Get("objeto"), "2");
                var requestProtecta = HttpContext.Current.Request.Params.Get("objeto") != null && HttpContext.Current.Request.Params.Get("objeto") != "null" ? JsonConvert.DeserializeObject<List<SavePolicyEmit>>(HttpContext.Current.Request.Params.Get("objeto")) : null;
                LogControl.save("SavedPolicyEmit", JsonConvert.SerializeObject(requestProtecta), "2");

                //var dpsDatosEC = HttpContext.Current.Request.Params.Get("datosDPS") != null && HttpContext.Current.Request.Params.Get("datosDPS") != "null" ? JsonConvert.DeserializeObject<WSPlataforma.Entities.QuotationModel.ViewModel.DpsTokenEC>(HttpContext.Current.Request.Params.Get("datosDPS")) : null;

                // Objeto Emision Mapfre
                var requestMapfre = HttpContext.Current.Request.Params.Get("emisionMapfre") != null && HttpContext.Current.Request.Params.Get("emisionMapfre") != "null" ? JsonConvert.DeserializeObject<EmitirBM>(HttpContext.Current.Request.Params.Get("emisionMapfre")) : null;
                if (requestMapfre != null)
                {
                    response = await generarPolizaMapfre(requestMapfre, requestProtecta);
                    poliza2 = response.P_COD_ERR == 0 ? response.P_POL_COMPANY : 0;
                }

                var requestNotaCredito = HttpContext.Current.Request.Params.Get("notaCredito") != null && HttpContext.Current.Request.Params.Get("notaCredito") != "null" ? JsonConvert.DeserializeObject<List<NotaCreditoModel>>(HttpContext.Current.Request.Params.Get("notaCredito")) : null;
                response = response.P_COD_ERR == 0 ? insertNotaCreditoTemp(requestProtecta[0].P_NBRANCH, requestNotaCredito, response) : response;

                //AVS - INTERCONEXION - VALIDACION DE AGENCIAMIENTO DE NUEVAS REGLAS SCTR - GENERACION DE POLIZA DE LA EPS
                response = await agenciamientoReglas(requestProtecta, response);

                if (response.P_COD_ERR == 0)
                {
                    // Emision Protecta
                    foreach (var policyEmit in requestProtecta)
                    {
                        #region Eliminar registro de process payment
                        if (policyEmit.P_SPAGO_ELEGIDO == "directo") // poliza matriz debe borrar esta llegando null
                        {
                            new QuotationCORE().deleteProcess(policyEmit.P_NID_PROC);
                        }
                        #endregion

                        if (response.P_COD_ERR == 0)
                        {
                            response = await validarRetroactividad(policyEmit);

                            response = response.P_COD_ERR == 0 ? registrarDPS(policyEmit) : response;

                            policyEmit.P_AMBOS_PRODUCTOS_SCTR = valuePolizaMixta(requestProtecta);

                            policyEmit.P_NCOT_NC = requestNotaCredito != null ? 1 : 0;

                            nidpaymentVC = Convert.ToInt32(policyEmit.P_NIDPAYMENT); // AGF 25042023
                            policyEmit.P_NIDPAYMENT = null;

                            // GCAA 22012024
                            if (policyEmit.P_NBRANCH == Convert.ToInt32(ELog.obtainConfig("vidaLeyBranch")))
                            {
                                new QuotationDA().update_table_peso(Convert.ToInt32(policyEmit.P_NID_COTIZACION));

                                new PolicyDA().getDatos_Procesar_Cobertura(policyEmit.P_NID_COTIZACION);
                            }

                            response = response.P_COD_ERR == 0 ? policyCORE.SavePolizaEmit(policyEmit, adjuntoList) : response;
                            LogControl.save("SavedPolicyEmit", JsonConvert.SerializeObject(response), "2");

                            var res2 = updateCodQuotation(policyEmit.P_NBRANCH, policyEmit.P_NID_COTIZACION, policyEmit.P_NID_PROC); // AVS - INTERCONEXION SABSA

                            response = response.P_COD_ERR == 0 ? historialSCTR(policyEmit, response, requestMapfre, ref poliza1, ref poliza2) : response;

                            response = response.P_COD_ERR == 0 ? insertHistTrama(policyEmit, response) : response;

                            //if (policyEmit.P_NEPS == Convert.ToInt32(ELog.obtainConfig("grandiaKey")) && (policyEmit.P_NPRODUCT == 2 || policyEmit.P_NCOT_MIXTA == 1) && response.P_COD_ERR == 0) // Solo para EPS
                            //{
                            //    policyEmit.P_NPOLICY = poliza2;
                            //    if (policyEmit.P_SFLAG_FAC_ANT == 1 && policyEmit.P_NPRODUCT == 2)
                            //    {
                            //        var res3 = policyDA.saveToSendComprobantesEps(policyEmit); // AGF - SABSA ENVIO DE RECIBOS A LA EPS
                            //    }
                            //}


                        }
                    }

                    if (response.P_COD_ERR == 0)
                    {
                        response.P_POL_PENSION = poliza1;
                        response.P_POL_SALUD = poliza2;
                        response.P_POL_VLEY = poliza1;
                        response.P_POL_AP = poliza1;
                        response.P_NPOLICY_ = poliza1.ToString();

                        getNumRecibo(requestProtecta[0], response, nidpaymentVC);

                        updateSchoolRE(requestProtecta[0], response);
                    }
                }
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = "Hubo un error al generar la póliza. Favor de comunicarse con sistemas";

                LogControl.save("SavedPolicyEmit", ex.ToString(), "3");
            }

            return response;
        }


        //private EntityCobertura ListaCobertura(int P_NBRANCH, int P_NPRODUCT, int P_NMODULEC, string P_DEFFECDATE, int P_COTIZACION, string P_CATEGORIA)
        //{
        //    EntityCobertura respuesta = new EntityCobertura();

        //    List<ListaCobertura> _item = new List<ListaCobertura>();
        //    _item = new PolicyDA().getListaCoberturas(P_NBRANCH, P_NPRODUCT, P_NMODULEC, P_DEFFECDATE);

        //    // se arma el json de coberturas para consultar al devmente
        //    string jsonx = JsonConvert.SerializeObject(_item.ToList());
        //    // se quita los []
        //    string resultado = jsonx.Substring(1, jsonx.Length - 2);

        //    // se obtiene el resultado de cobertura devueltas por devmente
        //    respuesta = new WebApiDevmente().ConsultarCobertura(resultado);

        //    // enviamos cobertura por cobertura para actualizar
        //    foreach (var ite in respuesta.data)
        //    {
        //        new PolicyDA().Actualizar_proceso_cobertura(
        //            P_COTIZACION,
        //            Convert.ToInt32(ite.coverageCode),
        //            ite.weight,
        //            1,
        //            Convert.ToInt32(ite.isPrincipal),
        //            P_CATEGORIA);
        //    }
        //    return respuesta;
        //}



        private QuotationResponseVM updateCodQuotation(int nbranch, int codigoCotizacion, string codigoProceso)
        {
            var response = new QuotationResponseVM() { P_NCODE = 0 };

            if (nbranch == Convert.ToInt64(ELog.obtainConfig("sctrBranch"))) //AVS - INTERCONEXION SABSA
            {
                response = QuotationDA.UpdateCodQuotation(codigoCotizacion, codigoProceso, null, null);
            }
            return response;
        }

        private void updateSchoolRE(SavePolicyEmit data, SalidadPolizaEmit response)
        {
            if (Generic.valRentaEstudiantil(data.P_NBRANCH.ToString(), data.P_NPRODUCT.ToString(), "3"))
            {
                policyCORE.updateQuotationCol(data, response);
            }
        }

        private SalidadPolizaEmit insertNotaCreditoTemp(int branch, List<NotaCreditoModel> requestNotaCredito, SalidadPolizaEmit response)
        {
            var ncList = new string[] { ELog.obtainConfig("vidaLeyBranch"), ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch") };

            if (ncList.Contains(branch.ToString()))
            {
                if (requestNotaCredito != null)
                {
                    response = new SharedCORE().insertNCTemp(requestNotaCredito);
                }
            }

            return response;
        }

        private SalidadPolizaEmit historialSCTR(SavePolicyEmit policyEmit, SalidadPolizaEmit response, EmitirBM requestMapfre, ref long poliza1, ref long poliza2)
        {
            if (policyEmit.P_NBRANCH == Convert.ToInt64(ELog.obtainConfig("sctrBranch")))
            {
                policyDA.Udp_SCTRHIS(policyEmit);

                poliza1 = policyEmit.P_NPRODUCT == 1 ? response.P_NPOLICY : poliza1;
                poliza2 = policyEmit.P_NPRODUCT == 2 ? requestMapfre != null ? poliza2 : response.P_NPOLICY : poliza2;
            }
            else
            {
                poliza1 = response.P_NPOLICY;
            }

            return response;
        }

        public int valuePolizaMixta(List<SavePolicyEmit> data)
        {
            int value = 0;

            // INI 07/07/2023 SCTR - AMBOS PRODUCTOS //AVS - INTERCONEXION SABSA
            if (/*data.Any(d => d.P_NPRODUCT == 1) && data[0].P_NBRANCH == Convert.ToInt32(ELog.obtainConfig("sctrBranch")) && data.Count() == 2 &&*/ data[0].P_NCOT_MIXTA == 1 && data[0].P_NEPS == Convert.ToInt32(ELog.obtainConfig("grandiaKey")))
            {
                //value = 1
                value = data[0].P_NCOT_MIXTA;
            }
            //FIN 07/07/2023

            return value;
        }

        public SalidadPolizaEmit registrarDPS(SavePolicyEmit data)
        {
            var response = new SalidadPolizaEmit() { P_COD_ERR = 0 };

            if (data.datosDPS != null)
            {
                //Registro de datos Biometricos
                policyCORE.regBIO(data.P_NID_COTIZACION, data.datosDPS.P_NRODOCUMENTO, data.datosDPS.P_NOMCLIENTE, data.datosDPS.P_ALGORITMO, data.datosDPS.P_PORCSIMILITUD, data.datosDPS.P_URLIMAGEN, data.datosDPS.P_FECHAEVAL, data.datosDPS.P_TIPOVAL, data.datosDPS.P_CODIGOVER);
                //Actualizar Permisos Core
                policyCORE.UpdateDatosPer(data.datosDPS.P_CODCONTRATANTE, data.datosDPS.P_PRIVACIDAD.ToString());
            }

            return response;
        }

        public async Task<SalidadPolizaEmit> validarRetroactividad(SavePolicyEmit data)
        {
            var response = new SalidadPolizaEmit() { P_COD_ERR = 0 };

            // Retroactividad
            var lRetro = new string[] { ELog.obtainConfig("vidaLeyBranch"), ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch") };
            if (lRetro.Contains(data.P_NBRANCH.ToString()))
            {
                response = await ValidateRetroactivity(data.P_NID_COTIZACION, data.P_NUSERCODE, data.P_DSTARTDATE, "EM", data.FlagCambioFecha, data.P_NBRANCH.ToString(), data.P_NPRODUCT.ToString());
            }

            return response;
        }

        public async Task<SalidadPolizaEmit> agenciamientoReglas(List<SavePolicyEmit> data, SalidadPolizaEmit response)
        {

            if (data[0].P_NBRANCH == Convert.ToInt64(ELog.obtainConfig("sctrBranch")))
            {
                //Objeto de detalle - SCTR - AVS PRY INTERCONEXION SABSA 17/07/2023
                var objetoDetParam = HttpContext.Current.Request.Params.Get("objetoDet");
                var requestSCTR = !string.IsNullOrEmpty(objetoDetParam) && objetoDetParam != "null" ? JsonConvert.DeserializeObject<List<dataQuotation>>(objetoDetParam) : null;

                if (requestSCTR != null)
                {
                    var norder = policyDA.GetOrderDet(requestSCTR[0].NID_COTIZACION);

                    foreach (var policyDetSCTR in requestSCTR)
                    {
                        var errorCode = policyDA.InsertEmicionDetV2(policyDetSCTR, norder);

                        if (errorCode.P_COD_ERR == 1)
                        {
                            response.P_COD_ERR = 1;
                            response.P_MESSAGE = "Fallo en InsertEmicionDetV2";
                            break;
                        }
                    }
                }

                if (response.P_COD_ERR == 0)
                {
                    if (data[0].P_NEPS == Convert.ToInt32(ELog.obtainConfig("grandiaKey")) && (data[0].P_NCOT_MIXTA == 1 || Convert.ToInt32(data[0].P_NPRODUCT) == 2))
                    {
                        LogControl.save("generarPolizaEPS", "INGRESO EN GENERACION DE POLIZA" + data[0].P_NID_COTIZACION.ToString(), "1");
                        response = await generarPolizaEPS(requestSCTR, data);
                    }
                }
            }

            return response;
        }

        public void getNumRecibo(SavePolicyEmit policyEmit, SalidadPolizaEmit response, int nidpaymentVC)
        {
            //var policyEmit = requestProtecta[0];
            if (policyEmit.P_NBRANCH.ToString() == ELog.obtainConfig("vidaIndividualBranch"))
            {
                policyEmit.P_NPOLICY = response.P_NPOLICY;

                var receipts = new List<PolicyReceipt>();

                do
                {
                    receipts = policyCORE.GetNumRecibo(policyEmit, "P");
                }
                while (!receipts.Any());

                var receiptsfinal = new List<PolicyReceipt>();
                receiptsfinal.Add(receipts[0]);

                policyEmit.P_NIDPAYMENT = nidpaymentVC;
                response.receipts = receiptsfinal;

                SendProcessVDPVC(policyEmit, response, receipts[0].nReceipt.ToString());
            }
        }

        public SalidadPolizaEmit insertHistTrama(SavePolicyEmit policyEmit, SalidadPolizaEmit response)
        {
            // Historial
            var lHistorial = new string[] { ELog.obtainConfig("vidaLeyBranch"), ELog.obtainConfig("accidentesBranch"),
                                            ELog.obtainConfig("vidaGrupoBranch"),  ELog.obtainConfig("vidaIndividualBranch"),
                                            ELog.obtainConfig("desgravamenBranch")}; //AVS PRY INTERCONEXION SABSA 17/07/2023

            if (lHistorial.Contains(policyEmit.P_NBRANCH.ToString()))
            {
                var trama = new ValidaTramaBM()
                {
                    NID_COTIZACION = policyEmit.P_NID_COTIZACION,
                    NTYPE_TRANSAC = "1",
                    NID_PROC = policyEmit.P_NID_PROC
                };
                new QuotationCORE().insertHistTrama(trama);
            }

            return response;
        }

        private async Task<SalidadPolizaEmit> generarPolizaMapfre(EmitirBM requestMapfre, List<SavePolicyEmit> requestProtecta)
        {
            var response = new SalidadPolizaEmit();
            try
            {
                var responseMapfre = await mapfreController.EmitirMapfre(requestMapfre);
                if (responseMapfre.codError == ELog.obtainConfig("codigo_OK") && responseMapfre.poliza.mcaProvisionalSalud != ELog.obtainConfig("mProvicional"))
                {
                    var polizaMapfreIns = new PolizaMpBM();
                    polizaMapfreIns.nid_cotizacion = requestProtecta[0].P_NID_COTIZACION.ToString();
                    polizaMapfreIns.nproduct = ELog.obtainConfig("saludKey");
                    polizaMapfreIns.ncompany_lnk = ELog.obtainConfig("mapfreKey");
                    polizaMapfreIns.scotiza_lnk = requestMapfre.numCotizacion;
                    polizaMapfreIns.ncodcia_mpe = ELog.obtainConfig("codCia");
                    polizaMapfreIns.spolicy_mpe = responseMapfre.poliza.numPolizaEnlace;
                    polizaMapfreIns.nspto_mpe = responseMapfre.poliza.numSptoEnlace;
                    polizaMapfreIns.napli_mpe = responseMapfre.poliza.numApliEnlace;
                    polizaMapfreIns.nsptoapli_mpe = responseMapfre.poliza.numSptoApliEnlace;
                    polizaMapfreIns.sefecspto_mpe = responseMapfre.poliza.fecEfecSpto;
                    polizaMapfreIns.svctospto_mpe = responseMapfre.poliza.fecVctoSpto;
                    polizaMapfreIns.sconstancia_mpe = responseMapfre.numConstancia;
                    polizaMapfreIns.ncodagt_mpe = responseMapfre.poliza.codAgt != null ? responseMapfre.poliza.codAgt.ToString() : "0";
                    polizaMapfreIns.nfraccpago_mpe = responseMapfre.poliza.fraccionamiento.codFraccPago != null ? responseMapfre.poliza.fraccionamiento.codFraccPago.ToString() : "0";
                    polizaMapfreIns.sfraccpago_mpe = responseMapfre.poliza.fraccionamiento.nomFraccPago;
                    polizaMapfreIns.nfracuotas_mpe = responseMapfre.poliza.fraccionamiento.numCuotas != null ? responseMapfre.poliza.fraccionamiento.numCuotas.ToString() : "0";
                    polizaMapfreIns.ncodmon_mpe = responseMapfre.poliza.moneda.codMon != null ? responseMapfre.poliza.moneda.codMon.ToString() : "1";
                    polizaMapfreIns.snommon_mpe = responseMapfre.poliza.moneda.nomMon;
                    polizaMapfreIns.nriesgo_mpe = responseMapfre.riesgoSCTR != null && responseMapfre.riesgoSCTR.Count > 0 ? responseMapfre.riesgoSCTR[0].numRiesgoSalud : null;
                    polizaMapfreIns.scategoria_mpe = responseMapfre.riesgoSCTR != null && responseMapfre.riesgoSCTR.Count > 0 ? responseMapfre.riesgoSCTR[0].nomCategoriaSalud : null;
                    polizaMapfreIns.ntasa_mpe = responseMapfre.riesgoSCTR != null && responseMapfre.riesgoSCTR.Count > 0 ? responseMapfre.riesgoSCTR[0].tasaSalud != null ? responseMapfre.riesgoSCTR[0].tasaSalud : null : null;
                    polizaMapfreIns.snumrecibo_mpe = responseMapfre.recibosSalud != null ? responseMapfre.recibosSalud.recibo.Count > 0 ? responseMapfre.recibosSalud.recibo[0].numRecibo : null : null;
                    polizaMapfreIns.nimprecibo_mpe = responseMapfre.recibosSalud != null ? responseMapfre.recibosSalud.recibo.Count > 0 ? responseMapfre.recibosSalud.recibo[0].impRecibo != null ? responseMapfre.recibosSalud.recibo[0].impRecibo : null : null : null;
                    polizaMapfreIns.sefecrecibo_mpe = responseMapfre.recibosSalud != null ? responseMapfre.recibosSalud.recibo.Count > 0 ? responseMapfre.recibosSalud.recibo[0].fecEfecRecibo : null : null;
                    polizaMapfreIns.svctorecibo_mpe = responseMapfre.recibosSalud != null ? responseMapfre.recibosSalud.recibo.Count > 0 ? responseMapfre.recibosSalud.recibo[0].fecVctoRecibo : null : null;
                    polizaMapfreIns.nusercode = ELog.obtainConfig("codUsuarioConfig");
                    polizaMapfreIns.smesadlntdo_mpe = responseMapfre.poliza.mcaPolizaMesAdelantado != null ? responseMapfre.poliza.mcaPolizaMesAdelantado : "N";

                    var polizaMapfre = await mapfreController.InsertaPolizaMapfre(polizaMapfreIns);
                    response.P_POL_COMPANY = Convert.ToInt64(responseMapfre.poliza.numPolizaEnlace);
                    response.P_COD_ERR = Convert.ToInt32(polizaMapfre.codError);
                    response.P_MESSAGE = polizaMapfre.mensaje;
                }
                else
                {
                    response.P_COD_ERR = Convert.ToInt32(ELog.obtainConfig("codigo_NO"));
                    response.P_MESSAGE = responseMapfre.descError;
                }
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = Convert.ToInt32(ELog.obtainConfig("codigo_NO"));
                response.P_MESSAGE = ex.Message;
                LogControl.save("generarPolizaMapfre", ex.ToString(), "3");
            }

            return response;
        }

        private async Task<SalidadPolizaEmit> generarPolizaEPS(List<dataQuotation> requestSctr, List<SavePolicyEmit> requestProtecta) /*AVS - INTERCONEXION SABSA*/
        {
            var response = new SalidadPolizaEmit();
            var poliza = new poliza_eps_sctr();
            var responseObject = new Response_EPS_Transaccion();
            var asincObject = new Response_EPS_Transaccion();
            poliza = policyDA.GetNpolicy_EPS(requestProtecta);
            var policyEmitEPS = requestProtecta.FirstOrDefault(p => p.P_NPRODUCT == 2);
            var transaccion = "EMISION";

            var request_ins_aseg = new CargaAsegEPS
            {
                P_NID_COTIZACION = policyEmitEPS.P_NID_COTIZACION,
                P_NID_PROC = policyEmitEPS.P_NID_PROC,
                P_NUSERCODE = policyEmitEPS.P_NUSERCODE,
                P_NPAYFREQ = policyEmitEPS.P_NPAYFREQ,
                P_NBRANCH = policyEmitEPS.P_NBRANCH,
                P_NPRODUCTO = policyEmitEPS.P_NPRODUCT,
                P_DSTARTDATE_POL = policyEmitEPS.P_DSTARTDATE,
                P_DEXPIRDAT_POL = policyEmitEPS.P_DEXPIRDAT,
                P_DSTARTDATE_ASE = policyEmitEPS.P_DSTARTDATE_ASE,
                P_DEXPIRDAT_ASE = policyEmitEPS.P_DEXPIRDAT_ASE,
                P_NID_PROC_EPS = policyEmitEPS.P_NID_PROC_EPS
            };

            try
            {
                if (policyEmitEPS != null)
                {
                    string cotizacion = policyEmitEPS.P_NID_COTIZACION.ToString();
                    string codigoCip = policyDA.GetCodigoCip(cotizacion);
                    string fechaPago = policyDA.GetFechaPago(Convert.ToInt32(cotizacion));
                    List<riesgos> listaRiesgos = policyDA.GetRiesgos(cotizacion);
                    List<asegurados> listaAsegurados = policyDA.GetAsegurados(policyEmitEPS.P_NID_PROC);

                    var epsTr = new dataTransaccion();
                    epsTr.codigoCotizacion = cotizacion;
                    epsTr.codigoProceso = policyEmitEPS.P_NID_PROC.ToString();
                    epsTr.codigoContrato = poliza.P_NPOLICY.ToString();
                    epsTr.fechaVencimientoPago = fechaPago == "" ? null : DateTime.ParseExact(fechaPago, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                    epsTr.fechaEfectoAseguradoRecibo = DateTime.ParseExact(policyEmitEPS.P_DSTARTDATE_ASE, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                    epsTr.fechaExpiracionAseguradoRecibo = DateTime.ParseExact(policyEmitEPS.P_DEXPIRDAT_ASE, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                    epsTr.fechaEfectoPoliza = DateTime.ParseExact(policyEmitEPS.P_DSTARTDATE, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                    epsTr.fechaExpiracionPoliza = DateTime.ParseExact(policyEmitEPS.P_DEXPIRDAT, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                    epsTr.asignacionActividadAltoRiesgo = (policyEmitEPS.P_SDELIMITER == 1) ? true : false;
                    epsTr.codigoMoneda = policyEmitEPS.P_NCURRENCY.ToString();
                    epsTr.codigoFrecuenciaPago = policyEmitEPS.P_SCOLTIMRE.ToString();
                    epsTr.codigoFrecuenciaRenovacion = policyEmitEPS.P_NPAYFREQ.ToString();
                    epsTr.codigoFormaPago = policyEmitEPS.P_PAYPF.ToString(); //AVS - INTERCONEXION SABSA REVISAR CODIGO DE PAGO
                    epsTr.asignacionFacturacionAnticipada = (policyEmitEPS.P_SFLAG_FAC_ANT == 1) ? true : false;
                    epsTr.asignacionRegulaMesVencido = (policyEmitEPS.P_FACT_MES_VENCIDO == 1) ? true : false;
                    epsTr.codigoTipoTransaccion = 1.ToString();
                    epsTr.fechaTransaccion = DateTime.Now.ToString("yyyy-MM-dd");
                    epsTr.codigoUsuarioRegistro = policyEmitEPS.P_NUSERCODE.ToString();
                    epsTr.primaMinimaAutorizada = decimal.Parse(policyEmitEPS.P_NPREM_MIN_EPS);
                    epsTr.primaComercial = policyEmitEPS.P_NPREM_NETA;
                    epsTr.igv = policyEmitEPS.P_IGV;
                    epsTr.derechoEmision = (decimal)policyEmitEPS.P_NDE;
                    epsTr.primaTotal = policyEmitEPS.P_NPREM_BRU;
                    epsTr.cipPagoEfectivo = codigoCip ?? "";
                    epsTr.riesgos = listaRiesgos;
                    epsTr.asegurados = listaAsegurados;

                    LogControl.save("generarPolizaEPS", JsonConvert.SerializeObject(epsTr), "2");

                    //REVISAR AQUI SI CUANDO SE PAGE DIRECTO QUE VALOR LE ESTA LLEGANDO
                    if (epsTr.codigoFormaPago == "3") {
                        epsTr.codigoFormaPago = "1";
                    }
                    var URI_SendEmision_EPS = ELog.obtainConfig("urlEmisionEPS_SCTR");

                    new QuotationDA().InsertLog(Convert.ToInt64(policyEmitEPS.P_NID_COTIZACION.ToString()), "01 - SE ENVIA JSON A EPS - " + transaccion, URI_SendEmision_EPS, JsonConvert.SerializeObject(epsTr), null);

                    var json = Task.Run(async () =>
                    {
                        var jsonTsk = await new PolicyDA().invocarServicio_EPS_TRA(Convert.ToInt32(cotizacion), JsonConvert.SerializeObject(epsTr), transaccion);

                        asincObject = JsonConvert.DeserializeObject<Response_EPS_Transaccion>(jsonTsk);

                        bool isSuccess = asincObject == null ? false : asincObject.success;

                        if (!isSuccess)
                        {
                            string errorMessage = "Servicio EPS - SCTR - Emision: " + asincObject.message;
                            new QuotationDA().InsertLog(Convert.ToInt64(policyEmitEPS.P_NID_COTIZACION.ToString()), "03 - ERROR SERVICIO EPS - " + transaccion, URI_SendEmision_EPS, JsonConvert.SerializeObject(asincObject), null);
                            new PolicyDA().insertErrorEPS(cotizacion, JsonConvert.SerializeObject(epsTr), policyEmitEPS.P_NID_PROC_EPS, policyEmitEPS.P_NID_PROC);
                        }
                        else
                        {
                            LogControl.save("generarPolizaEPS", "ENTRO PARA POLIZAS" + requestProtecta[0].P_NID_COTIZACION.ToString(), "1");
                            string mensaje = "SE REALIZO LA EMISIÓN CORRECTAMENTE";
                            var insEPS = new PolicyDA().insert_policy_EPS(poliza.P_NPOLICY, asincObject.data.idContrato, policyEmitEPS.P_NCOT_MIXTA == 1 ? policyEmitEPS.P_NID_PROC_EPS : policyEmitEPS.P_NID_PROC, mensaje);
                            var insDocEPSList = new List<Ins_Documentacion_EPS>();

                            foreach (var data in asincObject.data.documentos)
                            {
                                var insDocEPS = new Ins_Documentacion_EPS
                                {
                                    P_NID_PROC_EPS = policyEmitEPS.P_NID_PROC,
                                    P_NBRANCH = policyEmitEPS.P_NBRANCH,
                                    P_NPRODUCT = policyEmitEPS.P_NPRODUCT,
                                    P_NID_COTIZACION = Convert.ToInt32(cotizacion.ToString()),
                                    P_NPOLICY = poliza.P_NPOLICY,
                                    P_ID_DOCUMENTO = data.idDocumento,
                                    P_DES_DOCUMENTO = data.nombreDocumento,
                                    P_NSTATE = 1,
                                    P_MENSAJE_IMP = "TRABAJO INSERTADO",
                                    P_NTYPE_TRANSAC = 1
                                };

                                insDocEPSList.Add(insDocEPS);
                            }

                            foreach (var docEPS in insDocEPSList)
                            {
                                var result = policyDA.Ins_Documentos_EPS(docEPS);
                            }

                            policyDA.UPD_TRX_CARGA_DEFINITIVA(insDocEPSList[0]);
                        }

                    });

                    new PolicyDA().insertErrorEPS(cotizacion, JsonConvert.SerializeObject(epsTr), policyEmitEPS.P_NID_PROC_EPS, policyEmitEPS.P_NID_PROC);

                }
            }
            catch (Exception ex)
            {
                LogControl.save("generarPolizaEPS", ex.ToString(), "3");
            }

            return response;
        }

        private void SendProcessVDP(SavePolicyEmit request, SalidadPolizaEmit response)
        {
            ProcessRequest processRequest = new ProcessRequest();

            string sInput = string.Empty;

            string sUrl = ELog.obtainConfig("urlProcessVDP");

            sUrl = String.Format(sUrl, request.P_NIDPAYMENT, request.P_NPOLICY);

            request.P_NPOLICY = response.P_NPOLICY;

            var receipts = new List<PolicyReceipt>();
            receipts = policyCORE.GetNumRecibo(request, "E");

            processRequest.recibos = new List<Recibo>();
            foreach (var obj in receipts)
            {
                Recibo recibo = new Recibo();
                recibo.numeroRecibo = obj.nReceipt;
                processRequest.recibos.Add(recibo);

            }

            var json = JsonConvert.SerializeObject(processRequest);
            var requestPrintATP = (HttpWebRequest)WebRequest.Create(sUrl);
            requestPrintATP.Method = "PUT";
            requestPrintATP.ContentType = "application/json";
            requestPrintATP.Accept = "application/json";
            using (var streamWriter = new StreamWriter(requestPrintATP.GetRequestStream()))
            {
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }
            sInput = string.Format("{0} /{1}", sUrl, json);
            try
            {
                using (WebResponse respPrint = requestPrintATP.GetResponse())
                {
                    using (Stream strReader = respPrint.GetResponseStream())
                    {
                        //if (strReader == null) return;
                        if (strReader != null)
                        {
                            using (StreamReader objReader = new StreamReader(strReader))
                            {
                                string responseBody = objReader.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogControl.save("SendProcessVDP", ex.ToString(), "2");
                //throw ex;
            }
        }

        private void SendProcessVDPVC(SavePolicyEmit request, SalidadPolizaEmit response, string recibo1)
        {
            ProcessRequest processRequest = new ProcessRequest();

            string sInput = string.Empty;
            LogControl.save("SendProcessVDPVC", JsonConvert.SerializeObject(response), "2");
            string sUrl = ELog.obtainConfig("urlProcessVDP");

            sUrl = String.Format(sUrl, request.P_NIDPAYMENT, request.P_NPOLICY);

            request.P_NPOLICY = response.P_NPOLICY;

            var receipts = new List<PolicyReceipt>();
            receipts = policyCORE.GetNumRecibo(request, "P");
            LogControl.save("SendProcessVDPVCRecibos", JsonConvert.SerializeObject(receipts), "2");

            processRequest.recibos = new List<Recibo>();
            foreach (var obj in receipts)
            {
                Recibo recibo = new Recibo();
                if (obj.nReceipt.ToString() != recibo1)
                {
                    recibo.numeroRecibo = obj.nReceipt;
                    processRequest.recibos.Add(recibo);
                }

            }
            LogControl.save("SendProcessVDPVC", JsonConvert.SerializeObject(request), "2");
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12; // AGF 25042023
            var json = JsonConvert.SerializeObject(processRequest);
            LogControl.save("SendProcessVDPVCRecibos", JsonConvert.SerializeObject(json), "2");
            var requestPrintATP = (HttpWebRequest)WebRequest.Create(sUrl);
            requestPrintATP.Method = "PUT";
            requestPrintATP.ContentType = "application/json";
            requestPrintATP.Accept = "application/json";
            using (var streamWriter = new StreamWriter(requestPrintATP.GetRequestStream()))
            {
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }
            sInput = string.Format("{0} /{1}", sUrl, json);
            try
            {
                using (WebResponse respPrint = requestPrintATP.GetResponse())
                {
                    using (Stream strReader = respPrint.GetResponseStream())
                    {
                        //if (strReader == null) return;
                        if (strReader != null)
                        {
                            using (StreamReader objReader = new StreamReader(strReader))
                            {
                                string responseBody = objReader.ReadToEnd();
                                LogControl.save("SendProcessVDPVC", JsonConvert.SerializeObject(responseBody), "2");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogControl.save("SendProcessVDPVC", ex.ToString(), "3");
                //throw ex;
            }
        }

        [Route("valTransactionPolicy")]
        [HttpGet]
        public SalidadPolizaEmit valTransactionPolicy(int nroCotizacion, int idTipo = 0)
        {
            return policyCORE.valTransactionPolicy(nroCotizacion, idTipo);
        }

        [Route("GetVisualizadorProc")]
        [HttpPost]
        public DisplayProcessVM VisualizadorProc(DisplayProcessBM request)
        {
            return policyCORE.VisualizadorProc(request);
        }

        [Route("SavedPolicyTransac")]
        [HttpPost]
        public VMpolicy.ResponseVM SavedPolicyTransac(SavedPolicyTXBM data)
        {
            if (data.P_NBRANCH == Convert.ToInt32(ELog.obtainConfig("sctrBranch")) && data.P_NEPS == Convert.ToInt32(ELog.obtainConfig("grandiaKey")) && data.P_NTYPE_TRANSAC == "4" && (data.P_NCOT_MIXTA == 1 || data.P_NPRODUCT == 2))
            {
                List<dataTecnicaTrans> cursor = policyDA.getDataTecnica(data.P_NID_COTIZACION);

                var request = new PolicyTransactionSaveBM();
                request.P_NID_COTIZACION = data.P_NID_COTIZACION;
                request.P_NID_PROC = data.P_NID_PROC;
                request.P_NUSERCODE = data.P_NUSERCODE;
                request.P_NPAYFREQ = Convert.ToInt32(data.P_NPAYFREQ);
                request.P_NBRANCH = ELog.obtainConfig("sctrBranch");
                request.P_NPRODUCTO = "2";
                request.P_DSTARTDATE_POL = data.P_DEFFECDATE;
                request.P_DEXPIRDAT_POL = data.P_DEXPIRDAT;
                request.P_DSTARTDATE_ASE = data.P_DEFFECDATE;
                request.P_DEXPIRDAT = data.P_DEXPIRDAT;
                request.P_NID_PROC_EPS = cursor[0].NID_PROC;
                request.P_NTYPE_TRANSAC = Convert.ToInt32(data.P_NTYPE_TRANSAC);
                request.P_NPOLICY_SALUD = cursor[0].NPOLICY.ToString();
                request.P_DEFFECDATE = data.P_DEFFECDATE;
                request.P_SDELIMITER = cursor[0].SDELIMITER;
                request.P_NCURRENCY = cursor[0].NCURRENCY;
                request.P_SCOLTIMRE = data.P_SCOLTIMRE;
                request.P_PAYPF = 2;
                request.P_SFLAG_FAC_ANT = cursor[0].FACTURA_ANT;
                request.P_FACT_MES_VENCIDO = Convert.ToChar(cursor[0].REGULA);
                request.P_NPREM_MINIMA = cursor[0].NPREMIUM_MIN_AU.ToString();
                request.P_NPREM_NETA = cursor[0].NSUM_PREMIUMN.ToString();
                request.P_IGV = cursor[0].NSUM_IGV.ToString();
                request.P_NDE = Convert.ToDecimal(cursor[0].NDE);
                request.P_NPREM_BRU = cursor[0].NSUM_PREMIUM.ToString();

                LogControl.save("generarTransaccionEPS", "Entro en Transaccion aprobacion tecnica" + request.P_NID_COTIZACION.ToString(), "1");

                var response = generarTransaccionEPS(request);
            }

            return policyCORE.SavedPolicyTransac(data);
        }

        public string GenerarCodigo()
        {
            int longitud = 8;
            const string alfabeto = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder token = new StringBuilder();
            Random rnd = new Random();

            for (int i = 0; i < longitud; i++)
            {
                int indice = rnd.Next(alfabeto.Length);
                token.Append(alfabeto[indice]);
            }
            return token.ToString();
        }

        [Route("getDataConfig")]
        [HttpGet]
        public List<ConfigVM> getDataConfig(string stype)
        {
            return policyCORE.getDataConfig(stype);
        }

        [Route("insertPrePayment")]
        [HttpPost]
        public async Task<SalidadPolizaEmit> insertPrePayment(PrePaymentBM data)
        {
            var response = new SalidadPolizaEmit();

            try
            {
                LogControl.save("insertPrePayment-CIP", JsonConvert.SerializeObject(data, Formatting.None), "2"); //Para guardar la data enviada desde el front
                response = await policyCORE.insertPrePayment(data);
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = Convert.ToInt32(ELog.obtainConfig("codigo_NO"));
                response.P_MESSAGE = ex.Message;
                LogControl.save("insertPrePayment", ex.ToString(), "3");
            }

            return response;
        }

        private string GetSortableDateTime(DateTime dt)
        {
            var dateStr = String.Format("{0:s}", dt);
            return dateStr;
        }

        async public Task<SalidadPolizaEmit> ValidateRetroactivity(int cotizacion, int usercode, string startdate, string transac, int flagCambioFecha, string ramo, string producto)
        {

            var quotationCab = new QuotationCabBM()
            {
                P_NBRANCH = Convert.ToInt32(ramo),
                P_NPRODUCT = Convert.ToInt32(producto),
                RetOP = 2,
                NumeroCotizacion = cotizacion,
                P_NUSERCODE = usercode,
                P_DSTARTDATE = startdate,
                P_DSTARTDATE_ASE = startdate,
                TrxCode = transac,
                FlagCambioFecha = flagCambioFecha
            };
            var responseRetro = new QuotationCORE().ValidateRetroactivity(quotationCab);
            var result = new SalidadPolizaEmit()
            {
                P_COD_ERR = responseRetro.P_NCODE,
                P_MESSAGE = responseRetro.P_SMESSAGE
            };

            if (transac == "IN" || transac == "EM")
            {
                // Actualiza fecha
                new QuotationCORE().UpdateFechaEfectoAsegurado(quotationCab);
            }

            if (result.P_COD_ERR == 4)
            {
                result.P_MESSAGE = result.P_MESSAGE + " Se procede con la derivación al área técnica.";
                var success = await SendNotificationToDevmente(quotationCab);
                if (!success)
                {
                    result.P_MESSAGE = result.P_MESSAGE
                        + Environment.NewLine
                        + "Hubo un error en el envió de la cotización a técnica. Favor de comunicarse con sistemas.";
                }
            }
            return result;
        }

        [Route("getTypePolicy")]
        [HttpGet]
        public List<TypePolicy> getTypePolicy(string pbranch)
        {
            return policyCORE.getTypePolicy(pbranch);
        }

        [Route("getTypePolicyPD")]
        [HttpPost]
        public List<TypePolicy> getTypePolicyPD(TypePolicyVM data)
        {
            var response = policyCORE.getTypePolicy(data.codRamo);

            if (data.ntype_doc == 1)
            {
                if (data.sdocument.Substring(0, 2) == "20")
                {
                    response = response.Where(x => x.id == 2).ToList();
                }
            }
            else
            {
                response = response.Where(x => x.id == 1).ToList();
            }

            return response;
        }

        [Route("GetTipoRenovacionGraph")]
        [HttpPost]
        public async Task<List<TipoRenovacion>> GetTipoRenovacionGraph(TariffGraph data)
        {
            var response = new List<TipoRenovacion>();

            try
            {
                //if (await new QuotationCORE().valMiniTariffGraph(data))
                //{
                var dataRenov = new TypeRenBM()
                {
                    P_NPRODUCT = data.nproduct_channel,
                    P_NUSERCODE = data.nusercode,
                    P_TYPE_TRANSAC = data.ntype_transac,
                    P_NEPS = data.neps
                };

                var renovacionList = policyCORE.GetTipoRenovacion(dataRenov);

                if (new string[] { ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch") }.Contains(data.nbranch))
                {
                    response = await getRenovacionApVg(renovacionList, data);
                }
                else if (new string[] { ELog.obtainConfig("vidaIndividualBranch") }.Contains(data.nbranch))
                {
                    response = await getRenovacionDes(renovacionList, data);
                }
                else
                {
                    response = renovacionList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return response;
        }

        public async Task<List<TipoRenovacion>> getRenovacionDes(List<TipoRenovacion> renovacionList, TariffGraph data)
        {
            var response = new List<TipoRenovacion>();
            data.profile = new QuotationCORE().getProfileRepeat(data.nbranch, data.profile);

            // Traer todos los segmentos
            var miniTariff = await new QuotationCORE().getMiniTariffGraphDes(data);

            if (miniTariff.codError == 0)
            {
                if (miniTariff.data.miniTariffMatrix != null &&
                    miniTariff.data.miniTariffMatrix.segment != null &&
                    miniTariff.data.miniTariffMatrix.segment.rules != null)
                {
                    if (miniTariff.data.miniTariffMatrix.segment.rules.Count > 0)
                    {
                        //var arrTariff = miniTariff.data.miniTariffMatrix.segment.rules.SingleOrDefault(x => x.definition.code == "11").content.conditionalOptions.condition.ToArray();
                        var arrTariff = new List<string>();
                        foreach (var item in miniTariff.data.miniTariffMatrix.segment.rules)
                        {
                            if (item.definition.code == "11")
                            {
                                arrTariff.Add(item.content.conditionalOptions.condition);
                            }
                        }

                        if (arrTariff != null)
                        {
                            var arrMeses = ELog.obtainConfig("cantidadMesesDes").Split(';').Select(x => x.Split(',')).ToArray();
                            var arrHabilitado = arrMeses.Where(x => arrTariff.Contains(x[1])).ToArray();
                            /*
                            foreach (var item in arrHabilitado)
                            {
                                response.Add(renovacionList.SingleOrDefault(x => x.COD_TIPO_RENOVACION.ToString() == item[0]));
                            }
                            */

                            response.Add(renovacionList.SingleOrDefault(x => x.COD_TIPO_RENOVACION.ToString() == "6"));

                            response = response.Where(param => param != null).ToList();
                        }

                    }
                }
            }

            return response;
        }

        public async Task<List<TipoRenovacion>> getRenovacionApVg(List<TipoRenovacion> renovacionList, TariffGraph data)
        {
            var response = new List<TipoRenovacion>();
            data.profile = new QuotationCORE().getProfileRepeat(data.nbranch, data.profile);

            // Traer todos los segmentos
            var miniTariff = await new QuotationCORE().getMiniTariffGraph(data);

            if (miniTariff.codError == 0)
            {
                if (miniTariff.data.miniTariffMatrix != null &&
                    miniTariff.data.miniTariffMatrix.segment != null &&
                    miniTariff.data.miniTariffMatrix.segment.rules != null)
                {
                    if (miniTariff.data.miniTariffMatrix.segment.rules.Count > 0)
                    {

                        var arrTariff = miniTariff.data.miniTariffMatrix.segment.rules.SingleOrDefault(x => x.definition.code == "11" && x.definition.type == "GENERAL_FREQUENCY");

                        if (arrTariff != null)
                        {
                            var arrMeses = ELog.obtainConfig("cantidadMeses").Split(';').Select(x => x.Split(',')).ToArray();
                            var arrHabilitado = arrMeses.Where(x => arrTariff.values.ToArray().Contains(x[1])).ToArray();

                            foreach (var item in arrHabilitado)
                            {
                                response.Add(renovacionList.SingleOrDefault(x => x.COD_TIPO_RENOVACION.ToString() == item[0]));
                            }

                            response = response.Where(param => param != null).ToList();
                        }

                    }
                }
            }

            if (response.Count == 0)
            {
                response = renovacionList;
            }

            return response;
        }

        [Route("GetFrecuenciaPagoGraph")]
        [HttpPost]
        public async Task<List<FrecuenciaPago>> GetFrecuenciaPagoGraph(TariffGraph data)
        {
            var response = new List<FrecuenciaPago>();

            try
            {
                //if (await new QuotationCORE().valMiniTariffGraph(data))
                //{
                var frequencyList = new List<FrecuenciaPago>();

                if (ELog.obtainConfig("vidaIndividualBranch") == data.nbranch)
                {
                    frequencyList = policyCORE.GetFrecuenciaPagoTotal(data.renovationType, data.nproduct_channel);
                }
                else
                {
                    frequencyList = policyCORE.GetFrecuenciaPago(data.renovationType, data.nproduct_channel);
                }

                if (new string[] { ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch") }.Contains(data.nbranch))
                {
                    response = await getFrecuenciaApVg(frequencyList, data);
                }

                if (new string[] { ELog.obtainConfig("vidaIndividualBranch") }.Contains(data.nbranch)) // Colocar llave de desgravamen
                {
                    response = await getFrecuenciaDes(frequencyList, data);
                }

                if (new string[] { ELog.obtainConfig("desgravamenBranch") }.Contains(data.nbranch))
                {
                    response = frequencyList;
                }
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return response;
        }

        public async Task<List<FrecuenciaPago>> getFrecuenciaApVg(List<FrecuenciaPago> frequencyList, TariffGraph data)
        {
            var response = new List<FrecuenciaPago>();

            try
            {
                data.profile = new QuotationCORE().getProfileRepeat(data.nbranch, data.profile);

                // Traer todos los segmentos
                var miniTariff = await new QuotationCORE().getMiniTariffGraph(data);

                if (miniTariff.codError == 0)
                {
                    if (miniTariff.data.miniTariffMatrix != null &&
                        miniTariff.data.miniTariffMatrix.segment != null &&
                        miniTariff.data.miniTariffMatrix.segment.rules != null)
                    {
                        if (miniTariff.data.miniTariffMatrix.segment.rules.Count > 0)
                        {
                            var cantMeses = new QuotationCORE().cantidadMeses(data.renovationType, Int32.Parse(data.nbranch)).ToString();
                            var arrTariff = miniTariff.data.miniTariffMatrix.segment.rules.SingleOrDefault(x => new string[] { "12", "28" }.Contains(x.definition.code) && x.definition.type == "GENERAL_PAYMENT_FREQUENCY" && x.condition == cantMeses);

                            if (arrTariff != null)
                            {
                                var arrMeses = ELog.obtainConfig("cantidadMeses").Split(';').Select(x => x.Split(',')).ToArray();
                                var arrHabilitado = arrMeses.Where(x => arrTariff.values.ToArray().Contains(x[1])).ToArray();

                                foreach (var item in arrHabilitado)
                                {
                                    response.Add(frequencyList.FirstOrDefault(x => x.COD_TIPO_FRECUENCIA.ToString() == item[0]));
                                }

                                response = response.Where(param => param != null).ToList();
                            }
                        }
                    }
                }

                if (response.Count == 0)
                {
                    response = frequencyList;
                }
            }
            catch (Exception ex)
            {
                response = new List<FrecuenciaPago>();
            }

            return response;
        }

        public async Task<List<FrecuenciaPago>> getFrecuenciaDes(List<FrecuenciaPago> frequencyList, TariffGraph data)
        {
            var response = new List<FrecuenciaPago>();

            data.profile = new QuotationCORE().getProfileRepeat(data.nbranch, data.profile);

            // Traer todos los segmentos
            var miniTariff = await new QuotationCORE().getMiniTariffGraphDes(data);

            if (miniTariff.codError == 0)
            {
                if (miniTariff.data.miniTariffMatrix != null &&
                    miniTariff.data.miniTariffMatrix.segment != null &&
                    miniTariff.data.miniTariffMatrix.segment.rules != null)
                {
                    if (miniTariff.data.miniTariffMatrix.segment.rules.Count > 0)
                    {

                        //var cantMeses = new QuotationCORE().cantidadMeses(data.renovationType).ToString();
                        var cantMeses = new QuotationCORE().cantidadMesesDes(data.renovationType).ToString();
                        //var arrTariff = miniTariff.data.miniTariffMatrix.segment.rules.SingleOrDefault(x => x.id == "12" && x.condition == cantMeses).content.conditionalOptions.options.ToArray();
                        var arrTariff = new List<string>();
                        foreach (var item in miniTariff.data.miniTariffMatrix.segment.rules)
                        {
                            if (item.definition.code == "11" && item.content.conditionalOptions.condition == cantMeses)
                            {
                                //arrTariff.Add(item.content.conditionalOptions.condition);
                                arrTariff = item.content.conditionalOptions.options;
                            }
                        }
                        if (arrTariff != null)
                        {
                            var arrMeses = ELog.obtainConfig("cantidadMesesDes").Split(';').Select(x => x.Split(',')).ToArray();
                            var arrHabilitado = arrMeses.Where(x => arrTariff.Contains(x[0])).ToArray();

                            foreach (var item in arrHabilitado)
                            {
                                response.Add(frequencyList.SingleOrDefault(x => (x.COD_TIPO_FRECUENCIA.ToString() == item[1] && x.COD_TIPO_FRECUENCIA.ToString() != "6"))); //Cambiar a futuro en la llamada de frencuencias
                            }

                            response = response.Where(param => param != null).ToList();
                        }
                    }
                }
            }

            return response;
        }


        [Route("GetTechnicalTariffList")]
        [HttpPost]
        public async Task<List<SharedVM.TechnicalActivityVM>> GetTechnicalActivityList(TariffGraph data)
        {
            var activityList = new SharedCORE().GetTechnicalActivityList(data.nproduct_channel);

            try
            {
                //if (await new QuotationCORE().valMiniTariffGraph(data))
                //{
                if (new string[] { ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch") }.Contains(data.nbranch))
                {
                    activityList = await getActividadesApVg(activityList, data);
                }

                //if (new string[] { ELog.obtainConfig("") }.Contains(data.nbranch)) // Colocar llave de desgravamen
                //{

                //}
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return await Task.FromResult(activityList);
        }

        public async Task<List<SharedVM.TechnicalActivityVM>> getActividadesApVg(List<SharedVM.TechnicalActivityVM> activityList, TariffGraph data)
        {
            data.profile = new QuotationCORE().getProfileRepeat(data.nbranch, data.profile);

            // Traer todos los segmentos
            var miniTariff = await new QuotationCORE().getMiniTariffGraph(data);

            if (miniTariff.codError == 0)
            {
                if (miniTariff.data.miniTariffMatrix != null &&
                    miniTariff.data.miniTariffMatrix.segment != null &&
                    miniTariff.data.miniTariffMatrix.segment.rules != null)
                {
                    if (miniTariff.data.miniTariffMatrix.segment.rules.Count > 0)
                    {
                        var exclusionObj = miniTariff.data.miniTariffMatrix.segment.rules.SingleOrDefault(x => x.definition.code == "13" && x.definition.type == "GENERAL_INCLUSION");

                        if (exclusionObj != null)
                        {
                            if (!Convert.ToBoolean(exclusionObj.inclusion))
                            {
                                if (exclusionObj.values != null)
                                {
                                    activityList = activityList.Where((n) => !(exclusionObj.values.ToArray().Contains(n.Id.ToString()))).ToList();

                                }
                            }
                        }
                    }
                }
            }

            return activityList;
        }

        [Route("ListaTipoRenovacion")]
        [HttpGet]
        public List<ListaTipoRenovacion> GetListaTipoRenovacion()
        {
            return policyCORE.GetListaTipoRenovacion();
        }

        [Route("AjustedAmounts")]
        [HttpPost]
        public SalidaAjustedAmounts AjustedAmounts(AjustedAmountsBM data)
        {
            return policyCORE.AjustedAmounts(data);
        }

        [Route("getReferenceURL")]
        [HttpGet]
        public Task<ResponseGraph> getReferenceURL(string nbranch, string codUrl, string origen)
        {
            return new PolicyCORE().getReferenceURL(nbranch, codUrl, origen);
        }

        /// <summary>
        /// Servicio para descargar el cuadro de póliza en formato base64
        /// </summary>
        [Route("DownloadCuadroPoliza")]
        [HttpPost]
        public ResponseCuadroPolizaVM DownloadCuadroPoliza(RequestCuadroPolizaBM request)
        {
            var response = new ResponseCuadroPolizaVM() { cod_error = 0 };

            try
            {

                CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
                TextInfo textInfo = cultureInfo.TextInfo;

                request.p_transaccion = textInfo.ToTitleCase(request.p_transaccion);

                var resHeader = new PolicyCORE().getCotHeader(request);

                if (resHeader.cod_error == 0)
                {
                    string path = ELog.obtainConfig("pathImpresion" + request.p_ramo) + request.p_poliza + "\\" + resHeader.cod_header + "\\" + request.p_transaccion + "\\";

                    if (Directory.Exists(path))
                    {
                        string[] files = System.IO.Directory.GetFiles(path, "Cuadro*.pdf");

                        if (files.Length > 0)
                        {
                            Byte[] bytes = File.ReadAllBytes(files[0]);
                            response.documento = Convert.ToBase64String(bytes);
                            response.mensaje = "Se encontró el cuadro de póliza correctamente";
                        }
                        else
                        {
                            response.cod_error = 1;
                            response.mensaje = "No se ha generado el cuadro de póliza de la póliza enviada";
                        }
                    }
                    else
                    {
                        response.cod_error = 1;
                        response.mensaje = "No se ha encontrado la ruta de la póliza enviada";
                    }
                }
                else
                {
                    response.cod_error = resHeader.cod_error;
                    response.mensaje = resHeader.mensaje;
                }

            }
            catch (Exception ex)
            {
                response.cod_error = 1;
                response.mensaje = "Hubo un error al buscar el cuadro de póliza de la póliza enviada";
            }

            return response;
        }

        [Route("ProcesarTramaEstado")]
        [HttpPost]
        public async Task<QuotationResponseVM> ProcesarTramaEstado(QuotationCabBM data)
        {

            var objeto = JsonConvert.SerializeObject(data);

            var response = new QuotationResponseVM();

            if (data.P_NBRANCH == Convert.ToInt32(ELog.obtainConfig("vidaLeyBranch")))
            {
                response = policyCORE.ProcesarTramaEstado(data);
                if (response.P_SAPROBADO == "A")
                {
                    data.TrxCode = "EM";
                    var success = await SendNotificationToDevmente(data, data.P_SRUTA);
                    if (!success)
                    {
                        response.P_SMESSAGE = response.P_SMESSAGE
                            + Environment.NewLine
                            + "Hubo un error en el envió de la cotización a técnica. Favor de comunicarse con sistemas.";
                    }
                }
            }
            return response;
        }



        [Route("ProcesarTramaEstadoFiles")]
        [HttpPost]
        public async Task<QuotationResponseVM> ProcesarTramaEstadoFiles()
        {
            var response = new QuotationResponseVM();
            var data = JsonConvert.DeserializeObject<QuotationCabBM>(HttpContext.Current.Request.Params.Get("objEstado"), new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
            List<HttpPostedFile> files = new List<HttpPostedFile>();
            foreach (var item in HttpContext.Current.Request.Files)
            {
                HttpPostedFile arch = HttpContext.Current.Request.Files[item.ToString()];
                files.Add((HttpPostedFile)HttpContext.Current.Request.Files[item.ToString()]);
            }

            if (files.Count > 0)
            {
                string folderPath = files.Count > 0 ? ELog.obtainConfig("pathCotizacion") + data.NumeroCotizacion + "\\" + ELog.obtainConfig("movimiento") + "\\" + DateTime.Now.ToString("yyyyMMddhhmmss") + "\\" : String.Empty;
                response = sharedMethods.ValidatePath<QuotationResponseVM>(response, folderPath, files);
                if (response.P_COD_ERR == 1)
                {
                    return response;
                }
            }
            if (data.P_NBRANCH == Convert.ToInt32(ELog.obtainConfig("vidaLeyBranch")))
            {
                response = policyCORE.ProcesarTramaEstado(data);
                if (response.P_SAPROBADO == "A")
                {
                    data.TrxCode = "EM";
                    var success = await SendNotificationToDevmente(data, data.P_SRUTA);
                    if (!success)
                    {
                        response.P_SMESSAGE = response.P_SMESSAGE
                            + Environment.NewLine
                            + "Hubo un error en el envió de la cotización a técnica. Favor de comunicarse con sistemas.";
                    }
                }
            }
            return response;
        }

        [Route("EmitirCertificadoEstado")]
        public SalidadPolizaEmit EmitirCertificadoEstado()
        {
            var response = new SalidadPolizaEmit();
            var requestProtecta = HttpContext.Current.Request.Params.Get("objetoCE") != null && HttpContext.Current.Request.Params.Get("objetoCE") != "null" ? JsonConvert.DeserializeObject<List<SavePolicyEmit>>(HttpContext.Current.Request.Params.Get("objetoCE")) : null;

            LogControl.save("EmitirCertificadoEstado", JsonConvert.SerializeObject(requestProtecta), "2");
            try
            {
                foreach (var policyEmit in requestProtecta)
                {

                    var requestCertificado = new SavePolicyEmitMapper(policyEmit).Map();
                    var responseEmit = new QuotationCORE().finalizarCotizacionEstadoVL(requestCertificado);
                    responseEmit = responseEmit.cod_error == 0 ? new QuotationCORE().TRAS_CARGA_ASE(requestCertificado) : responseEmit;
                    responseEmit = responseEmit.cod_error == 0 ? new QuotationCORE().INS_JOB_CLOSE_SCTR(requestCertificado) : responseEmit;
                    response.P_COD_ERR = responseEmit.cod_error;
                    response.P_MESSAGE = responseEmit.message;
                    response.P_NPOLICY = (long)requestCertificado.policy;
                    response.P_POL_VLEY = (long)requestCertificado.policy;

                }


            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = ex.ToString();
                response.P_MESSAGE = "Hubo un error al generar la póliza. Favor de comunicarse con sistemas";
                LogControl.save("EmitirCertificadoEstado", ex.ToString(), "3");
            }

            return response;

        }

        [Route("Msj_TecnicsSCTR")]
        [HttpPost]
        public ErrorCode Msj_TecnicsSCTR()
        {
            var data = JsonConvert.DeserializeObject<HisDetRenovSCTR>(HttpContext.Current.Request.Params.Get("objeto"));
            return policyCORE.Msj_TecnicsSCTR(data);
        }

        /*
        [Route("StatementInfo")]
        [HttpPost]
        public async Task<PolicyStatementResponseVM> StatementInfo(PolicyStatementVM data)
        {
            return await policyCORE.StatementInfo(data);
        }
        */

        [Route("DeleteDataSCTR")]
        [HttpPost]
        public ErrorCode DeleteDataSCTR()
        {
            var data = JsonConvert.DeserializeObject<delSCTR>(HttpContext.Current.Request.Params.Get("DeleteSCTR"));
            return policyCORE.DeleteDataSCTR(data);
        }

        [Route("GetMonthsSCTR")]
        [HttpPost]
        public MonthsSCTRReturn GetMonthsSCTR(monthsSCTR data)
        {
            return policyCORE.GetMonthsSCTR(data);
        }

        [Route("GenerarCIPTransaccionesPD")]
        [HttpPost]
        public async Task<CIPGeneradoResponse> GenerarCIPTransaccionesPD()
        {
            CIPGeneradoResponse response = new CIPGeneradoResponse();
            CIPGeneradoBM data = new CIPGeneradoBM();
            string json = ""; //AVS - INTERCONEXION SABSA 19/09/2023
            string url = ""; //AVS - INTERCONEXION SABSA 19/09/2023
            try
            {

                SalidadPolizaEmit responseInsertPrepay = new SalidadPolizaEmit();
                List<HttpPostedFile> fileList = new List<HttpPostedFile>();
                foreach (var fileName in HttpContext.Current.Request.Files)
                {
                    fileList.Add((HttpPostedFile)HttpContext.Current.Request.Files[fileName.ToString()]);
                }

                string jsonData = HttpContext.Current.Request.Params.Get("dataTransaccionesPD"); //AVS - INTERCONEXION SABSA 
                data = JsonConvert.DeserializeObject<CIPGeneradoBM>(HttpContext.Current.Request.Params.Get("dataTransaccionesPD"));
                LogControl.save("GenerarCIPTransaccionesPD", "DATA: " + JsonConvert.SerializeObject(data), "2");
                WebServiceUtil webServiceUtil = new WebServiceUtil();

                if (Convert.ToInt32(data.dataCIPBM.ramo) == Convert.ToInt32(ELog.obtainConfig("sctrBranch"))) //AVS - INTERCONEXION SABSA 
                {
                    url = ELog.obtainConfig("PagoGeneraCIP_Multiple");

                    var lista_cliente = new cliente_eps
                    {
                        nombres = data.dataCIPBM.nombres,
                        apellidos = data.dataCIPBM.Apellidos,
                        idTipoDocumento = data.dataCIPBM.tipoDocumento,
                        numeroDocumento = data.dataCIPBM.numeroDocumento,
                        correo = data.dataCIPBM.correo,
                        telefono = data.dataCIPBM.telefono
                    };

                    var lista_detalle = new List<detalle_eps>();
                    if (data.dataCIPBM.eps == Convert.ToInt32(ELog.obtainConfig("grandiaKey")))
                    {
                        if (data.dataCIPBM.mixta == 1)
                        {
                            lista_detalle.Add(new detalle_eps
                            {
                                idProducto = data.dataCIPBM.producto,
                                monto = data.dataCIPBM.monto_pension,
                                externalId = data.dataCIPBM.ExternalId
                            });

                            lista_detalle.Add(new detalle_eps
                            {
                                idProducto = data.dataCIPBM.producto_EPS,
                                monto = data.dataCIPBM.monto_salud,
                                externalId = data.dataCIPBM.ExternalId_EPS
                            });
                        }
                        else
                        {
                            if (data.dataCIPBM.flagProducto == 1)
                            {
                                lista_detalle.Add(new detalle_eps
                                {
                                    idProducto = data.dataCIPBM.producto,
                                    monto = data.dataCIPBM.monto_pension,
                                    externalId = data.dataCIPBM.ExternalId
                                });
                            }
                            else if (data.dataCIPBM.flagProducto == 2)
                            {
                                lista_detalle.Add(new detalle_eps
                                {
                                    idProducto = data.dataCIPBM.producto_EPS,
                                    monto = data.dataCIPBM.monto_salud,
                                    externalId = data.dataCIPBM.ExternalId_EPS
                                });
                            }
                            else if (data.dataCIPBM.flagProducto == 0)
                            {
                                if (data.dataCIPBM.producto == "1")
                                {
                                    lista_detalle.Add(new detalle_eps
                                    {
                                        idProducto = data.dataCIPBM.producto,
                                        monto = data.dataCIPBM.monto_pension != null ? data.dataCIPBM.monto_pension : data.dataCIPBM.monto,
                                        externalId = data.dataCIPBM.ExternalId
                                    });
                                }
                                else if (data.dataCIPBM.producto == "2")
                                {
                                    lista_detalle.Add(new detalle_eps
                                    {
                                        idProducto = data.dataCIPBM.producto_EPS,
                                        monto = data.dataCIPBM.monto_salud != null ? data.dataCIPBM.monto_salud : data.dataCIPBM.monto,
                                        externalId = data.dataCIPBM.ExternalId
                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        lista_detalle.Add(new detalle_eps
                        {
                            idProducto = data.dataCIPBM.producto,
                            monto = data.dataCIPBM.monto,
                            externalId = data.dataCIPBM.ExternalId
                        });
                    }

                    var cip_eps = new CIPRequest_Multiple();
                    cip_eps.tipoSolicitud = data.dataCIPBM.tipoSolicitud;
                    cip_eps.tipoPago = "1";
                    cip_eps.cliente = lista_cliente;
                    cip_eps.codigoCanal = data.dataCIPBM.codigoCanal;
                    cip_eps.idUsuario = data.dataCIPBM.codUser.ToString();
                    cip_eps.idMoneda = data.dataCIPBM.Moneda;
                    cip_eps.conceptoPago = data.dataCIPBM.conceptoPago;
                    cip_eps.idRamo = data.dataCIPBM.ramo;
                    cip_eps.montoTotal = data.dataCIPBM.monto;
                    cip_eps.detalle = lista_detalle;

                    json = await webServiceUtil.invocarServicio(JsonConvert.SerializeObject(cip_eps), url, data.token, servicesCIP: true);
                }
                else
                {
                    url = ELog.obtainConfig("PagoGeneraCIP");

                    json = await webServiceUtil.invocarServicio(JsonConvert.SerializeObject(data.dataCIPBM), url, data.token, servicesCIP: true);
                }

                if (!string.IsNullOrEmpty(json))
                {

                    if (Convert.ToInt32(data.dataCIPBM.ramo) == Convert.ToInt32(ELog.obtainConfig("sctrBranch")))  //AVS - INTERCONEXION SABSA 
                    {
                        response = JsonConvert.DeserializeObject<CIPGeneradoResponse>(json, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        if (data.dataCIPBM.eps == Convert.ToInt32(ELog.obtainConfig("grandiaKey")))
                        {
                            var udp_cip = new PolicyDA().Udp_cip_EPS((data.dataCIPBM.flagProducto == 1) ? data.dataCIPBM.ExternalId : ((data.dataCIPBM.flagProducto != 1 && data.dataCIPBM.mixta == 1) ? data.dataCIPBM.ExternalId : ""),
                                     (data.dataCIPBM.flagProducto == 2) ? data.dataCIPBM.ExternalId_EPS : ((data.dataCIPBM.flagProducto != 2 && data.dataCIPBM.mixta == 1) ? data.dataCIPBM.ExternalId_EPS : ""),
                                      "COD. CIP GENERADOS");
                        }
                        LogControl.save("GenerarCIPTransaccionesPD_Multiple", JsonConvert.SerializeObject(response.data), "2");
                    }
                    else
                    {
                        response.cipResponse = JsonConvert.DeserializeObject<CIPResponse>(json, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        LogControl.save("GenerarCIPTransaccionesPD", JsonConvert.SerializeObject(response.cipResponse), "2");
                    }

                    bool validacion = response.cipResponse != null ? response.cipResponse.exito : response.data.success; //AVS - INTERCONEXION SABSA

                    if (validacion)
                    {
                        List<PrePaymentBM> prepayment_multiple = new List<PrePaymentBM>(); //AVS - INTERCONEXION SABSA
                        PrePaymentBM prepayment = new PrePaymentBM();

                        if (Convert.ToInt32(data.dataCIPBM.ramo) == Convert.ToInt32(ELog.obtainConfig("sctrBranch"))) //AVS - INTERCONEXION SABSA
                        {
                            if (data.dataCIPBM.eps == Convert.ToInt32(ELog.obtainConfig("grandiaKey")))
                            {
                                if (data.dataCIPBM.mixta == 1)
                                {
                                    PrePaymentBM prepayment1 = new PrePaymentBM();
                                    prepayment1.idProcess = data.idProcess;
                                    prepayment1.quotationNumber = data.quotationNumber;
                                    prepayment1.typeTransaction = data.typeTransaction;
                                    prepayment1.userCode = data.userCode;
                                    prepayment_multiple.Add(prepayment1);

                                    PrePaymentBM prepayment2 = new PrePaymentBM();
                                    prepayment2.idProcess = data.idProcess_EPS;
                                    prepayment2.quotationNumber = data.quotationNumber;
                                    prepayment2.typeTransaction = data.typeTransaction;
                                    prepayment2.userCode = data.userCode;
                                    prepayment_multiple.Add(prepayment2);
                                }
                                else
                                {
                                    PrePaymentBM prepayment2 = new PrePaymentBM();
                                    prepayment2.idProcess = data.idProcess;
                                    prepayment2.quotationNumber = data.quotationNumber;
                                    prepayment2.typeTransaction = data.typeTransaction;
                                    prepayment2.userCode = data.userCode;
                                    prepayment_multiple.Add(prepayment2);
                                }
                            }
                            else
                            {
                                PrePaymentBM prepayment2 = new PrePaymentBM();
                                prepayment2.idProcess = data.idProcess;
                                prepayment2.quotationNumber = data.quotationNumber;
                                prepayment2.typeTransaction = data.typeTransaction;
                                prepayment2.userCode = data.userCode;
                                prepayment_multiple.Add(prepayment2);
                            }

                        }
                        else
                        {
                            prepayment.idProcess = data.idProcess;
                            prepayment.quotationNumber = data.quotationNumber;
                            prepayment.typeTransaction = data.typeTransaction;
                            prepayment.userCode = data.userCode;
                        }

                        if (data.actualizarCotizacion != null)
                        {
                            var statuschanges = new QuotationCORE().ChangeStatusVL(data.actualizarCotizacion, fileList);
                            LogControl.save("GenerarCIPTransaccionesPD", "statuschanges : " + JsonConvert.SerializeObject(statuschanges), "2");
                            #region Eliminar registro de process payment
                            if (statuschanges.ErrorCode == 0)
                            {
                                if (new string[] { "voucher", "directo" }.Contains(data.actualizarCotizacion.pagoElegido))
                                {
                                    var idProcess = new QuotationCORE().GetProcessCodePD(data.actualizarCotizacion.QuotationNumber);
                                    new QuotationCORE().deleteProcess(idProcess);
                                }
                            }
                            #endregion
                            if (statuschanges.StatusCode == 0)
                            {
                                var responsecab = GetPolizaEmitCab(data.quotationNumber.ToString(), "1", data.userCode, FlagNoCIP: 1).Result;
                                var genericResponse = (PolizaEmitCAB)responsecab.GenericResponse;
                                if (genericResponse.COD_ERR == "0")
                                {
                                    prepayment = dataPrepayment(prepayment, data.savedPolicyList, genericResponse);
                                    prepayment.idProcess = genericResponse.NID_PROC;
                                    responseInsertPrepay = await policyCORE.insertPrePayment(prepayment);
                                    if (responseInsertPrepay.P_COD_ERR == 0)
                                    {
                                        response.valid = true;
                                        response.approve = true;
                                    }
                                    else
                                    {
                                        response.valid = true;
                                        response.approve = false;
                                        response.P_MESSAGE = "Hubo un problema al Generar el CIP. Favor de comunicarse con sistemas.";
                                        LogControl.save("GenerarCIPTransaccionesPD", "insertPrePayment 1 | NID_PROC:  " + prepayment.idProcess + "  | " + JsonConvert.SerializeObject(responseInsertPrepay, Formatting.None), "2");
                                    }
                                }
                                else
                                {
                                    response.approve = false;
                                    response.P_COD_ERR = Convert.ToInt32(genericResponse.COD_ERR);
                                    response.P_MESSAGE = "Problemas con la obtención de los datos de la cotización. Favor de comunicarse con sistemas.";
                                    LogControl.save("GenerarCIPTransaccionesPD", "GetPolizaEmitCab  | NID_PROC:  " + prepayment.idProcess + "  | " + JsonConvert.SerializeObject(responsecab, Formatting.None), "2");
                                }
                            }
                            else
                            {
                                response.approve = false;
                                response.P_COD_ERR = statuschanges.StatusCode;
                                response.P_MESSAGE = "Problemas con la Actualización de la cotización. Favor de comunicarse con sistemas.";
                                LogControl.save("GenerarCIPTransaccionesPD", "ChangeStatusVL  | NID_PROC:  " + prepayment.idProcess + "  | " + JsonConvert.SerializeObject(statuschanges, Formatting.None), "2");
                            }
                        }
                        else
                        {
                            if (Convert.ToInt32(data.dataCIPBM.ramo) == Convert.ToInt32(ELog.obtainConfig("sctrBranch"))) //AVS - INTERCONEXION SABSA
                            {
                                PrePaymentBM prepaymentItem = prepayment_multiple[0];
                                prepaymentItem = dataPrepayment(prepaymentItem, data.savedPolicyList);
                                responseInsertPrepay = await policyCORE.insertPrePayment(prepaymentItem);
                            }
                            else
                            {
                                prepayment = dataPrepayment(prepayment, data.savedPolicyList);
                                responseInsertPrepay = await policyCORE.insertPrePayment(prepayment);
                            }

                            if (responseInsertPrepay.P_COD_ERR == 0)
                            {
                                var flagJson = policyDA.UdpJsonCIP(jsonData, data.quotationNumber, data.dataCIPBM.ExternalId);
                                response.valid = true;
                                response.approve = true;
                            }
                            else
                            {
                                response.valid = true;
                                response.approve = false;
                                response.P_COD_ERR = responseInsertPrepay.P_COD_ERR;
                                response.P_MESSAGE = "Hubo un problema al Generar el CIP. Favor de comunicarse con sistemas.";
                                LogControl.save("GenerarCIPTransaccionesPD", "insertPrePayment 2 | NID_PROC:  " + prepayment.idProcess + "  | " + JsonConvert.SerializeObject(responseInsertPrepay, Formatting.None), "2");
                            }
                        }
                    }
                    else
                    {
                        response.P_COD_ERR = 1;
                        response.P_MESSAGE = "Problemas con la generación de CIP. Favor de comunicarse con sistemas.";
                    }
                }
                return response;
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = "Hubo un problema con la generación de CIP. Favor de comunicarse con sistemas.";
                LogControl.save("GenerarCIPTransaccionesPD", ex.ToString() + " |  NID_PROC:  " + JsonConvert.SerializeObject(data, Formatting.None), "3");
                return response;
            }

        }

        [Route("GenerarCIPKushkiTransaccionesPD")]
        [HttpPost]
        public async Task<CIPGeneradoKushkiResponse> GenerarCIPKushkiTransaccionesPD()
        {
            CIPGeneradoKushkiResponse response = new CIPGeneradoKushkiResponse();
            CIPGeneradoBM data = new CIPGeneradoBM();
            string json = ""; 
            string url = "";

            try
            {
                SalidadPolizaEmit responseInsertPrepay = new SalidadPolizaEmit();
                List<HttpPostedFile> fileList = new List<HttpPostedFile>();
                foreach (var fileName in HttpContext.Current.Request.Files)
                {
                    fileList.Add((HttpPostedFile)HttpContext.Current.Request.Files[fileName.ToString()]);
                }

                string jsonData = HttpContext.Current.Request.Params.Get("dataTransaccionesPD");
                data = JsonConvert.DeserializeObject<CIPGeneradoBM>(HttpContext.Current.Request.Params.Get("dataTransaccionesPD"));
                LogControl.save("GenerarCIPKushkiTransaccionesPD", "DATA: " + JsonConvert.SerializeObject(data), "2");
                WebServiceUtil webServiceUtil = new WebServiceUtil();

                url = ELog.obtainConfig("PagoGeneraCIP_Kushki");

                var lista_cliente = new cliente_kushki
                {
                    nombres = data.dataCIPBM.nombres,
                    apellidoPaterno = data.dataCIPBM.Apellidos,
                    idTipoDocumento = data.dataCIPBM.tipoDocumento,
                    numeroDocumento = data.dataCIPBM.numeroDocumento,
                    correo = data.dataCIPBM.correo,
                    telefono = data.dataCIPBM.telefono
                };

                var lista_detalle = new List<detalle_kushki>();

                if (data.dataCIPBM.mixta == 1)
                {
                    lista_detalle.Add(new detalle_kushki
                    {
                        idProducto = data.dataCIPBM.producto,
                        monto = data.dataCIPBM.monto_pension,
                        correo = data.dataCIPBM.correo,
                        externalId = data.dataCIPBM.ExternalId
                    });

                    lista_detalle.Add(new detalle_kushki
                    {
                        idProducto = data.dataCIPBM.producto_EPS,
                        monto = data.dataCIPBM.monto_salud,
                        correo = data.dataCIPBM.correo,
                        externalId = data.dataCIPBM.ExternalId_EPS
                    });
                }
                else
                {
                    if (data.dataCIPBM.flagProducto == 1)
                    {
                        lista_detalle.Add(new detalle_kushki
                        {
                            idProducto = data.dataCIPBM.producto,
                            monto = data.dataCIPBM.monto_pension,
                            correo = data.dataCIPBM.correo,
                            externalId = data.dataCIPBM.ExternalId
                        });
                    }
                    else if (data.dataCIPBM.flagProducto == 2)
                    {
                        lista_detalle.Add(new detalle_kushki
                        {
                            idProducto = data.dataCIPBM.producto_EPS,
                            monto = data.dataCIPBM.monto_salud,
                            correo = data.dataCIPBM.correo,
                            externalId = data.dataCIPBM.ExternalId_EPS
                        });
                    }
                    else if (data.dataCIPBM.flagProducto == 0)
                    {
                        if (data.dataCIPBM.producto == "1")
                        {
                            lista_detalle.Add(new detalle_kushki
                            {
                                idProducto = data.dataCIPBM.producto,
                                monto = data.dataCIPBM.monto_pension != null ? data.dataCIPBM.monto_pension : data.dataCIPBM.monto,
                                correo = data.dataCIPBM.correo,
                                externalId = data.dataCIPBM.ExternalId
                            });
                        }
                        else if (data.dataCIPBM.producto == "2")
                        {
                            lista_detalle.Add(new detalle_kushki
                            {
                                idProducto = data.dataCIPBM.producto_EPS,
                                monto = data.dataCIPBM.monto_salud != null ? data.dataCIPBM.monto_salud : data.dataCIPBM.monto,
                                correo = data.dataCIPBM.correo,
                                externalId = data.dataCIPBM.ExternalId
                            });
                        }
                    }                    
                }

                var cip_ = new CIPRequest_Kushki();
                cip_.tipoSolicitud = data.dataCIPBM.tipoSolicitud;
                cip_.tipoPago = data.dataCIPBM.tipoPago;
                cip_.cliente = lista_cliente;
                cip_.codigoCanal = data.dataCIPBM.codigoCanal;
                cip_.idUsuario = data.dataCIPBM.codUser.ToString();
                cip_.idMoneda = data.dataCIPBM.Moneda;
                cip_.idRamo = data.dataCIPBM.ramo;
                cip_.montoTotal = data.dataCIPBM.monto;
                cip_.detalle = lista_detalle;

                LogControl.save("GenerarCIPKushkiTransaccionesPD3", data.quotationNumber.ToString(), "2");
                LogControl.save("GenerarCIPKushkiTransaccionesPD3", JsonConvert.SerializeObject(cip_), "2");
                LogControl.save("GenerarCIPKushkiTransaccionesPD3", data.quotationNumber.ToString(), "2");
                json = await webServiceUtil.invocarServicioKushki(JsonConvert.SerializeObject(cip_), url, data.token,null,null, servicesCIP: true, data.quotationNumber);

                if (!string.IsNullOrEmpty(json))
                {
                    response = JsonConvert.DeserializeObject<CIPGeneradoKushkiResponse>(json, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    /*var udp_cip = new PolicyDA().Udp_cip_EPS((data.dataCIPBM.flagProducto == 1) ? data.dataCIPBM.ExternalId : ((data.dataCIPBM.flagProducto != 1 && data.dataCIPBM.mixta == 1) ? data.dataCIPBM.ExternalId : ""),
                                      (data.dataCIPBM.flagProducto == 2) ? data.dataCIPBM.ExternalId_EPS : ((data.dataCIPBM.flagProducto != 2 && data.dataCIPBM.mixta == 1) ? data.dataCIPBM.ExternalId_EPS : ""), "COD. CIP GENERADOS");*/
                    
                    var udp_cip = new PolicyDA().Udp_link_kushki((data.dataCIPBM.mixta == 1 && data.dataCIPBM.producto == "1") ? data.dataCIPBM.ExternalId : (data.dataCIPBM.mixta == 0 && data.dataCIPBM.producto == "1") ? data.dataCIPBM.ExternalId : "",
                                                                 (data.dataCIPBM.mixta == 1 && data.dataCIPBM.producto_EPS == "2") ? data.dataCIPBM.ExternalId_EPS : (data.dataCIPBM.mixta == 0 && data.dataCIPBM.producto_EPS == "2") ? data.dataCIPBM.ExternalId_EPS : "",
                                                                 (data.dataCIPBM.mixta == 1 && data.dataCIPBM.producto == "1") ? response.data.cupones[0].url : (data.dataCIPBM.mixta == 0 && data.dataCIPBM.producto == "1") ? response.data.cupones[0].url : "",
                                                                 (data.dataCIPBM.mixta == 1 && data.dataCIPBM.producto_EPS == "2") ? response.data.cupones[1].url : (data.dataCIPBM.mixta == 0 && data.dataCIPBM.producto_EPS == "2") ? response.data.cupones[0].url : "",
                                                                 (data.dataCIPBM.mixta == 1 && data.dataCIPBM.producto == "1") ? data.dataCIPBM.tipoPago : (data.dataCIPBM.mixta == 0 && data.dataCIPBM.producto == "1") ? data.dataCIPBM.tipoPago : "",
                                                                 (data.dataCIPBM.mixta == 1 && data.dataCIPBM.producto_EPS == "2") ? data.dataCIPBM.tipoPago : (data.dataCIPBM.mixta == 0 && data.dataCIPBM.producto_EPS == "2") ? data.dataCIPBM.tipoPago : "",
                                                                 "COD. LINK KUSHKI GENERADOS");

                    LogControl.save("GenerarCIPKushkiTransaccionesPD3", data.quotationNumber.ToString(), "2");
                    LogControl.save("GenerarCIPKushkiTransaccionesPD3", JsonConvert.SerializeObject(response.data), "2");
                    LogControl.save("GenerarCIPKushkiTransaccionesPD3", data.quotationNumber.ToString(), "2");

                    bool validacion = response.data.success;

                    if (validacion)
                    {
                        List<PrePaymentBM> prepayment_multiple = new List<PrePaymentBM>();
                        PrePaymentBM prepayment = new PrePaymentBM();

                        if (data.dataCIPBM.mixta == 1)
                        {
                            PrePaymentBM prepayment1 = new PrePaymentBM();
                            prepayment1.idProcess = data.idProcess;
                            prepayment1.quotationNumber = data.quotationNumber;
                            prepayment1.typeTransaction = data.typeTransaction;
                            prepayment1.userCode = data.userCode;
                            prepayment_multiple.Add(prepayment1);

                            PrePaymentBM prepayment2 = new PrePaymentBM();
                            prepayment2.idProcess = data.idProcess_EPS;
                            prepayment2.quotationNumber = data.quotationNumber;
                            prepayment2.typeTransaction = data.typeTransaction;
                            prepayment2.userCode = data.userCode;
                            prepayment_multiple.Add(prepayment2);
                        }
                        else
                        {
                            PrePaymentBM prepayment2 = new PrePaymentBM();
                            prepayment2.idProcess = data.idProcess;
                            prepayment2.quotationNumber = data.quotationNumber;
                            prepayment2.typeTransaction = data.typeTransaction;
                            prepayment2.userCode = data.userCode;
                            prepayment_multiple.Add(prepayment2);
                        }

                        PrePaymentBM prepaymentItem = prepayment_multiple[0];
                        prepaymentItem = dataPrepayment(prepaymentItem, data.savedPolicyList);
                        responseInsertPrepay = await policyCORE.insertPrePayment(prepaymentItem);

                        if (responseInsertPrepay.P_COD_ERR == 0)
                        {
                            var flagJson = policyDA.UdpJsonCIP(jsonData, data.quotationNumber, data.dataCIPBM.ExternalId);
                            response.valid = true;
                            response.approve = true;
                        }
                        else
                        {
                            response.valid = true;
                            response.approve = false;
                            response.P_COD_ERR = responseInsertPrepay.P_COD_ERR;
                            response.P_MESSAGE = "Hubo un problema al Generar el CIP. Favor de comunicarse con sistemas.";
                            LogControl.save("GenerarCIPKushkiTransaccionesPD", "insertPrePayment 2" + JsonConvert.SerializeObject(responseInsertPrepay, Formatting.None), "2");
                        }
                    }
                    else
                    {
                        response.P_COD_ERR = 1;
                        response.P_MESSAGE = "Problemas con la generación de CIP. Favor de comunicarse con sistemas.";
                    }
                }
                else {
                    LogControl.save("GenerarCIPKushkiTransaccionesPD3", data.quotationNumber.ToString(), "2");
                    LogControl.save("GenerarCIPKushkiTransaccionesPD3", json, "2");
                    LogControl.save("GenerarCIPKushkiTransaccionesPD3", data.quotationNumber.ToString(), "2");
                }
                return response;
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = "Hubo un problema con la generación de CIP. Favor de comunicarse con sistemas.";
                LogControl.save("GenerarCIPKushkiTransaccionesPD", ex.ToString() + " |  NID_PROC:  " + JsonConvert.SerializeObject(data, Formatting.None), "3");
                return response;
            }
        }

        public PrePaymentBM dataPrepayment(PrePaymentBM request, string savedPolicyList, PolizaEmitCAB responsecab = null)
        {

            if (request.typeTransaction == 1)
            {
                var saveEmit = JsonConvert.DeserializeObject<List<SavePolicyEmitDataBM>>(savedPolicyList, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                if (responsecab != null)
                {
                    saveEmit.ForEach(x =>
                    {
                        x.P_NID_PROC = responsecab.NID_PROC;
                        x.P_SCOLTIMRE = responsecab.TIP_RENOV;
                        x.P_NPAYFREQ = Convert.ToInt32(responsecab.FREQ_PAGO);
                        x.P_NIDPAYMENT = 0;
                    });
                    request.jsonPrt = JsonConvert.SerializeObject(saveEmit, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                }
                else
                {
                    saveEmit.ForEach(x =>
                    {
                        x.P_NIDPAYMENT = 0;
                        x.P_PAYPF = 1; // AGF  05052024 SCTR KUSHKI 
                    });
                    request.jsonPrt = JsonConvert.SerializeObject(saveEmit, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                }
                return request;

            }
            else
            {
                var saveEmit = JsonConvert.DeserializeObject<PolicyTransactionDataCIPBM>(savedPolicyList, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                if (responsecab != null)
                {
                    saveEmit.P_NIDPAYMENT = 0;
                    saveEmit.P_NID_PROC = responsecab.NID_PROC;
                    saveEmit.P_SCOLTIMRE = responsecab.TIP_RENOV;
                    saveEmit.P_NPAYFREQ = Convert.ToInt32(responsecab.FREQ_PAGO);
                    request.jsonPrt = JsonConvert.SerializeObject(saveEmit, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                }
                else
                {
                    saveEmit.P_NIDPAYMENT = 0;
                    //saveEmit.P_SFLAG_FAC_ANT = Convert.ToInt32(saveEmit.P_SFLAG_FAC_ANT);
                    request.jsonPrt = JsonConvert.SerializeObject(saveEmit, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                }
                return request;

            }
        }

        [Route("downloadExcelVL")]
        [HttpPost]
        public ResponseAccountStatusVM GetPlantillaVidaLey(GetTrama data)
        {

            var response = new PolicyCORE().DownloadExcelPlantillaVidaLey(data);

            return response;

        }

        //AGF 03042023
        [Route("downloadExcelVCF")]
        [HttpPost]
        public ResponseAccountStatusVM GetPlantillaVCF(GetTrama data)
        {
            var response = new PolicyCORE().DownloadExcelPlantillaVCF(data);
            return response;
        }

        //INI COMISION VL KT
        [Route("getCommissionVL")]
        [HttpPost]
        public CommissionVLVM getCommissionVL(CommissionVLBM data)
        {
            return policyCORE.getCommissionVL(data);
        }
        // FIN COMISION VL KT

        [Route("valBrokerVCF")]
        [HttpPost]
        public async Task<ValBrokerModel> ValBrokerVCF(ValBrokerModel data)
        {
            return policyCORE.valBrokerVCF(data);
        }

        [Route("GetExpirAseg")]
        [HttpGet]
        public async Task<PolizaEmitCAB> GetExpirAseg(string nroCotizacion)
        {
            PolizaEmitCAB response = new PolizaEmitCAB();
            response = new PolicyCORE().GetExpirAseg(nroCotizacion);

            return response;
        }

        [Route("GetValFact")]
        [HttpPost]
        public ValPolicy GetValFact(ValPolicy data)
        {
            return policyCORE.GetValFact(data);
        }

        [Route("relanzarDocumento")]
        [HttpPost]
        public VMpolicy.ResponseVM relanzarDocumento(RelanzarDocumentoVM data)
        {
            var response = new VMpolicy.ResponseVM();
            try
            {
                DeleteFiles(data.sruta);
                response = policyCORE.relanzarDocumento(data);
            }
            catch (Exception ex)
            {
                response.P_NCODE = "1";
                response.P_SMESSAGE = ex.Message;
            }

            return response;
        }

        public void DeleteFiles(string sruta)
        {
            try
            {
                Directory.Delete(sruta, true);
            }
            catch (Exception ex)
            {

            }
        }

        [Route("PayMethodsTypeValidate")]
        [HttpGet]
        public ErrorCode GetPayMethodsTypeValidate(string riesgo, string prima, string typeMovement = "1") /*AVS - INTERCONEXION SABSA */
        {
            return policyCORE.GetPayMethodsTypeValidate(riesgo, prima, typeMovement);
        }

        // ACTUALIZAR ESTADO PAGADO EPS - DGC - 14/11/2023
        [Route("ActualizarEstadoPagadoEPS")]
        [HttpPost]
        public IHttpActionResult ActualizarEstadoPagadoEPS(ActualizarEstadoPagadoEPSFilter data)
        {
            var result = this.policyCORE.ActualizarEstadoPagadoEPS(data);
            return Ok(result);
        }

        [Route("GetPolicyTransListExcel")]
        [HttpPost]
        public string GetPolicyTransListExcel(TransPolicySearchBM data)
        {
            return policyCORE.GetPolicyTransListExcel(data);
        }
        [Route("SendComprobantesEps")]
        [HttpPost]
        public async Task<IHttpActionResult> SendComprobantesEps(List<ComprobantesEpsBM> list_comprobantes)
        {
            List<ComprobantesEpsBM> list_comprobantes2 = new List<ComprobantesEpsBM>();

            //var let1 = new ComprobantesEpsBM()
            //{
            //    P_NID_COTIZACION = 58362,
            //    P_NBRANCH = 77,
            //    P_NPRODUCT = 2,
            //    P_NPOLICY = 5000000597,
            //    P_NSTATE = 0,
            //    P_NID_PROC = "00002586x8DXVBmM20231221210647",
            //    P_NTYPE_TRANSAC = "1"
            //};
            /*
            var let2 = new ComprobantesEpsBM()
            {
                P_NID_COTIZACION = 58364,
                P_NBRANCH = 77,
                P_NPRODUCT = 2,
                P_NPOLICY = 5000000598,
                P_NSTATE = 0,
                P_NID_PROC = "00002586EABeST8C20231221211318",
                P_NTYPE_TRANSAC = "1"
            };

            var let3 = new ComprobantesEpsBM()
            {
                P_NID_COTIZACION = 58350,
                P_NBRANCH = 77,
                P_NPRODUCT = 2,
                P_NPOLICY = 5000000593,
                P_NSTATE = 0,
                P_NID_PROC = "000025865VeI7Oxw20231221190421",
                P_NTYPE_TRANSAC = "1"
            };
            */
            //list_comprobantes2.Add(let1);
            //list_comprobantes2.Add(let2);
            //list_comprobantes2.Add(let3);

            ErrorCode result = await policyCORE.SendComprobantesEps(list_comprobantes);
            if (result.P_COD_ERR == 0)
            {
            }

            return Ok(result);
        }

        [Route("GetCalcExecSCTR")]
        [HttpPost]
        public calcExec GetCalcExecSCTR(string nidproc, int ramo, int? producto, int? nrocotizacion, string npolicy, int varCalcExec, string fecha_exclusion)
        {
            return policyCORE.GetCalcExecSCTR(nidproc, ramo, producto, nrocotizacion, npolicy, varCalcExec, fecha_exclusion);
        }

        [Route("GetPolizaColegio")]
        [HttpGet]
        public List<QuotizacionColBM> GetPolizaColegio(int nroCotizacion)
        {
            return policyCORE.GetPolizaColegio(nroCotizacion);
        }

        [Route("PolicyValidateQuotationTransac")]
        [HttpPost]
        public PolicyValidateQuotationTransacVM PolicyValidateQuotationTransac(PolicyValidateQuotationTransacBM data)
        {
            return policyCORE.PolicyValidateQuotationTransac(data);
        }

        //[Route("GetCommentsCotiVIGP")]
        //[HttpPost]
        //public QuotationStateVIGPVM GetCommentsCotiVIGP(QuotationStateVIGPBM data)
        //{
        //    return quotationCORE.GetCommentsCotiVIGP(data);
        //}


        [Route("RecacularCoberturas")]
        [HttpPost]
        public string RecacularCoberturas(int P_NID_COTIZACION)
        {
            new PolicyDA().getDatos_Procesar_Cobertura(P_NID_COTIZACION);
            return "";
        }

        [Route("ValidateProcessInsured")]
        [HttpPost]
        public async Task<QuotationResponseVM> ValidateProcessInsured(QuotationCabBM data)
        {
            var response = new QuotationResponseVM();

            var lAccess = new string[] { ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch"), ELog.obtainConfig("sctrBranch") };

            response = new PolicyCORE().ValidateProcessInsured(data);


            // Validar retroactividad
            var lRamosR = new string[] { ELog.obtainConfig("accidentesBranch"),
                                                 ELog.obtainConfig("vidaGrupoBranch")};

            if (response.P_SAPROBADO == "A" && lRamosR.Contains(data.P_NBRANCH.ToString()))
            {
                var success = await SendNotificationToDevmente(data);
                if (!success)
                {
                    response.P_SMESSAGE = response.P_SMESSAGE
                        + Environment.NewLine
                        + "Hubo un error en el envió de la cotización a técnica. Favor de comunicarse con sistemas.";
                }
            }


            return response;
        }

        [Route("getPagoKushki")]
        [HttpGet]
        public PagoKushki getPagoKushki(int nroCotizacion_pen, string nidProc_pen, int nroCotizacion_sal, string nidProc_sal, string stype_transac)
        {
            return policyCORE.getPagoKushki(nroCotizacion_pen, nidProc_pen, nroCotizacion_sal, nidProc_sal, stype_transac);
        }

        [Route("getCodPagoKushki")]
        [HttpGet]
        public PagoKushki getCodPagoKushki(int ramo, int idPayment_pen, int idPayment_sal)
        {
            return policyCORE.getCodPagoKushki(ramo, idPayment_pen, idPayment_sal);
        }

        [Route("GetProcessTrama")]
        [HttpGet]
        public NidProcessRenewVM GetProcessTrama(int nroCotizacion)
        {
            return policyCORE.GetProcessTrama(nroCotizacion);
        }

        [Route("PayDirectMethods")]
        [HttpGet]
        public ErrorCode PayDirectMethods(int nroCotizacion, string typeMovement)
        {
            return policyCORE.PayDirectMethods(nroCotizacion, typeMovement);
        }

        [Route("GetTipoPagoSCTR")]
        [HttpGet]
        public TypePayVM GetTipoPagoSCTR(int nroCotizacion)
        {
            return policyCORE.GetTipoPagoSCTR(nroCotizacion);
        }

    }
}