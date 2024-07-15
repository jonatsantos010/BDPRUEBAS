using ExcelDataReader;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.DynamicData;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Windows.Forms;
using WSPlataforma.ApiServiceClient;
using WSPlataforma.Core;
using WSPlataforma.Entities.Cliente360.BindingModel;
using WSPlataforma.Entities.Cliente360.ViewModel;
using WSPlataforma.Entities.Graphql;
using WSPlataforma.Entities.MapfreIntegrationModel.BindingModel;
using WSPlataforma.Entities.MapfreIntegrationModel.ViewModel;
using WSPlataforma.Entities.PolicyModel.BindingModel;
using WSPlataforma.Entities.QuotationModel.BindingModel;
using WSPlataforma.Entities.QuotationModel.BindingModel.QuotationModification;
using WSPlataforma.Entities.QuotationModel.ViewModel;
using WSPlataforma.Entities.TechnicalTariffModel.BindingModel;
using WSPlataforma.Entities.TechnicalTariffModel.ViewModel;
using WSPlataforma.Entities.EPSModel.ViewModel;
using WSPlataforma.Util;
using System.Text.RegularExpressions;
using static WSPlataforma.Entities.Graphql.CotizadorGraph;
using System.Threading;
using System.Globalization;
using WSPlataforma.DA;
using static WSPlataforma.Entities.CoberturaModel.EntityCobertura;
using WSPlataforma.Entities.CoberturaModel;
using Newtonsoft.Json.Linq;

namespace WSPlataforma.Controllers
{
    [RoutePrefix("Api/QuotationManager")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class QuotationController : ApiController
    {
        QuotationCORE quotationCORE = new QuotationCORE();
        PolicyCORE policyCORE = new PolicyCORE();
        SharedMethods sharedMethods = new SharedMethods();
        QuotationDA quotationDA = new QuotationDA();
        MapfreIntegrationController mapfreController = new MapfreIntegrationController();

        [Route("GetInfoQuotationAuth")]
        [HttpPost]
        public SalidaInfoAuthBaseVM GetInfoQuotationAuth(InfoAuthBM infoAuth)
        {
            return quotationCORE.GetInfoQuotationAuth(infoAuth);
        }

        [Route("GetProcessCode")]
        [HttpGet]
        public string GetProcessCode(int numeroCotizacion)
        {
            return quotationCORE.GetProcessCode(numeroCotizacion);
        }

        [Route("GetBandejaList")]
        [HttpPost]
        public List<BandejaVM> GetBandejaList(BandejaBM bandejaBM)
        {
            return quotationCORE.GetBandejaList(bandejaBM);
        }

        [Route("ChangeStatusVL")]
        [HttpPost]
        public GenericResponseVM ChangeStatusVL()
        {
            try
            {
                List<HttpPostedFile> fileList = new List<HttpPostedFile>();
                foreach (var fileName in HttpContext.Current.Request.Files)
                {
                    fileList.Add((HttpPostedFile)HttpContext.Current.Request.Files[fileName.ToString()]);
                }

                var data = JsonConvert.DeserializeObject<StatusChangeBM>(HttpContext.Current.Request.Params.Get("statusChangeData"));
                LogControl.save("ChangeStatusVL", JsonConvert.SerializeObject(data, Formatting.None), "2");
                GenericResponseVM resp = quotationCORE.ChangeStatusVL(data, fileList);

                #region Eliminar registro de process payment
                if (resp.ErrorCode == 0)
                {
                    if (new string[] { "voucher", "directo" }.Contains(data.pagoElegido))
                    {
                        data.idProcess = new QuotationCORE().GetProcessCodePD(data.QuotationNumber);
                        quotationCORE.deleteProcess(data.idProcess);
                    }
                }
                #endregion

                return resp;
            }
            catch (Exception ex)
            {
                GenericResponseVM resp = new GenericResponseVM();
                resp.StatusCode = 1;
                resp.ErrorMessageList = new List<string>(new string[] { ex.Message });
                LogControl.save("ChangeStatusVL", ex.ToString(), "3");

                return resp;
            }
        }

        [Route("UpdateCodQuotation")]
        [HttpPost]
        public QuotationResponseVM UpdateCodQuotation(ValidaTramaBM request)
        {
            return quotationCORE.UpdateCodQuotation(request);
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

        [Route("GetTrackingList")]
        [HttpPost]
        public GenericResponseVM GetTrackingList(TrackingBM data)
        {
            var dataHist = quotationCORE.GetTrackingList(data);
            var listaHist = (List<QuotationTrackingVM>)dataHist.GenericResponse;

            if (listaHist.Count > 0)
            {
                listaHist = validarReverse(listaHist, listaHist[0].nbranch);
                dataHist.GenericResponse = listaHist;
            }

            return dataHist;
        }

        public List<QuotationTrackingVM> validarReverse(List<QuotationTrackingVM> list, string nbranch)
        {
            try
            {
                if (new string[] { ELog.obtainConfig("vidaLeyBranch") }.Contains(nbranch))
                {
                    var reverse = list.SingleOrDefault(x => x.Status == "EMITIDO");
                    if (reverse != null && list[0].Status != "EMITIDO")
                    {
                        var statusList = new string[] { "CREADO", "APROBADO", "POR DECLARAR", "POR RENOVAR", "POR INCLUIR",
                        "POR ENDOSAR", "POR EXCLUIR", "RECHAZADO", "NO PROCEDE", "AP. POR TÉCNICA" };

                        if (statusList.Contains(list[0].Status))
                        {
                            var tramite = quotationCORE.tramitePendiente(list[0], 1);
                            if (Convert.ToInt64(tramite) == 0) // Valida que no haya tramites pendientes
                            {
                                if (list[0].Status == "APROBADO") // Evalua si está en la bandeja de operaciones
                                {
                                    var modo = quotationCORE.tramitePendiente(list[0], 2);
                                    var operacionesList = new string[] { "RENOVAR", "INCLUIR", "DECLARAR", "ENDOSAR", "EXCLUIR" };

                                    if (!operacionesList.Contains(modo) && list[0].FilePathList.Count == 0)
                                    {
                                        list[0].reverse = 1;
                                    }
                                }
                                else
                                {
                                    list[0].reverse = 1;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogControl.save("validarReverse", ex.ToString(), "3");
            }

            return list;
        }

        [Route("ChangeStatus")]
        [HttpPost]
        public GenericResponseVM ChangeStatus()
        {
            try
            {
                List<HttpPostedFile> fileList = new List<HttpPostedFile>();
                foreach (var fileName in HttpContext.Current.Request.Files)
                {
                    fileList.Add((HttpPostedFile)HttpContext.Current.Request.Files[fileName.ToString()]);
                }

                var agencyData = JsonConvert.DeserializeObject<StatusChangeBM>(HttpContext.Current.Request.Params.Get("statusChangeData"));
                GenericResponseVM resp = quotationCORE.ChangeStatus(agencyData, fileList);

                return resp;
            }
            catch (Exception ex)
            {
                GenericResponseVM resp = new GenericResponseVM();
                resp.StatusCode = 3;
                resp.MessageList = new List<string>(new string[] { ex.Message });
                return resp;
            }
        }

        [Route("GetQuotationList")]
        [HttpPost]
        public GenericResponseVM GetQuotationList(QuotationSearchBM dataToSearch)
        {
            return quotationCORE.GetQuotationList(dataToSearch);
        }

        [Route("GetPolicyList")]
        [HttpPost]
        public GenericResponseVM GetPolicyList(QuotationSearchBM dataToSearch)
        {
            return quotationCORE.GetPolicyList(dataToSearch);
        }

        //Controlador cotizacion para validar pendiente por tecnica
        [Route("GetQuotationValidateFromTecnica")]
        //[HttpPost]

        [Route("ResendDPS")]
        [HttpPost]
        public async Task<string> ResendDPS(QuotizacionCliBM datoscli)
        {

            DpsToken responsedps = new DpsToken();
            DpsToken responsedpsUpd = new DpsToken();

            var respuesta = "";

            responsedps = quotationCORE.FindDPS(Int32.Parse(datoscli.externalId), datoscli.idRamo, datoscli.idProducto);

            var jsonDPSConsulta = await policyCORE.invocarServicioConsultaDPS(responsedps.id, responsedps.key);

            responsedps = JsonConvert.DeserializeObject<DpsToken>(jsonDPSConsulta, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            if (responsedps.success)
            {
                if (responsedps.idEstado != "4" && responsedps.idEstado != "5" && responsedps.idEstado != "6")
                {

                    var jsonDPS = await quotationCORE.invocarServicioDPS(JsonConvert.SerializeObject(datoscli));

                    responsedpsUpd = JsonConvert.DeserializeObject<DpsToken>(jsonDPS, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    if (responsedpsUpd.success)
                    {
                        quotationCORE.UpdateDPS(Int32.Parse(datoscli.externalId), datoscli.idUsuario, responsedpsUpd.id, responsedpsUpd.key, 1);

                        respuesta = "Se ha enviado correctamente el correo";

                    }

                }
                else
                {
                    respuesta = "No puede reenviar este documento, por favor volver a consultar";
                }
            }


            return respuesta;
        }
        [Route("envioTecnicaPolizaMatriz")]
        [HttpPost]
        public async Task<QuotationResponseVM> envioTecnicaPolizaMatriz()
        {
            var response = new QuotationResponseVM();
            var responseSendNotificartionDevmente = new GenericResponseVM();
            EnvioPolizaMatriz envioPolizaMatriz = new EnvioPolizaMatriz();
            try
            {

                var quotationEnvioMariz = JsonConvert.DeserializeObject<QuotationCabBM>(HttpContext.Current.Request.Params.Get("quotationEnvioMariz"));
                envioPolizaMatriz = JsonConvert.DeserializeObject<EnvioPolizaMatriz>(HttpContext.Current.Request.Params.Get("objetoEnvioMariz"));


                List<HttpPostedFile> files = new List<HttpPostedFile>();
                foreach (var item in HttpContext.Current.Request.Files)
                {
                    HttpPostedFile arch = HttpContext.Current.Request.Files[item.ToString()];
                    files.Add((HttpPostedFile)HttpContext.Current.Request.Files[item.ToString()]);
                }


                //var rutaFinalEval = ELog.obtainConfig("pathCotizacion") + response.P_NID_COTIZACION + "\\" + ELog.obtainConfig("cotizacionKey") + "\\";
                //response = sharedMethods.ValidatePath<QuotationResponseVM>(response, rutaFinalEval, files);
                //if (response.P_COD_ERR == 1)
                //    return response;
                string folderPath = files.Count > 0 ? ELog.obtainConfig("pathCotizacion") + quotationEnvioMariz.NumeroCotizacion + "\\" + ELog.obtainConfig("movimiento") + "\\" + DateTime.Now.ToString("yyyyMMddhhmmss") + "\\" : String.Empty;
                quotationEnvioMariz.P_SRUTA = folderPath;
                response.P_COD_ERR = 0;


                var resultValidate = await quotationCORE.ValidateQuotationEstadoMatriz(quotationEnvioMariz, envioPolizaMatriz.P_NID_COTIZACION, envioPolizaMatriz.P_NUSERCODE);


                //if (envioPolizaMatriz.ReevaluarTecnica)
                //{
                //    var request = new PolicyTransactionSaveBM()
                //    {
                //        P_NID_COTIZACION = envioPolizaMatriz.P_NID_COTIZACION,
                //        P_NUSERCODE = envioPolizaMatriz.P_NUSERCODE,
                //        P_SCOMMENT = "Reevaluar Técnica",
                //        P_SRUTA = "",
                //        P_NPRODUCTO = "1",
                //        flagTramite = true
                //    };
                //    var result = policyCORE.insertarHistorial(request);
                //}

                if (resultValidate.P_NCODE == 0)
                {

                    responseSendNotificartionDevmente = await SendDataQuotationToDevmenteTramitesMatriz(envioPolizaMatriz.P_NID_COTIZACION, 0, envioPolizaMatriz.TipoTransaccion, revaluarTecnica: envioPolizaMatriz.ReevaluarTecnica, comentarioReevaluar: quotationEnvioMariz.P_SCOMMENT, ruta: quotationEnvioMariz.P_SRUTA);

                    if (responseSendNotificartionDevmente.ErrorCode == 1)
                    {
                        response.P_SMESSAGE = response.P_SMESSAGE + Environment.NewLine + responseSendNotificartionDevmente.MessageError;
                    }
                    else
                    {
                        response.P_SAPROBADO = "S";
                        response.P_COD_ERR = 0;
                        response.P_MESSAGE = envioPolizaMatriz.ReevaluarTecnica ? "Se envió al área técnica para reevaluar el tramite Nro. " + quotationEnvioMariz.P_NID_TRAMITE : "Para emitir la póliza, la cotización necesita aprobación del área técnica";
                    }
                }
                else
                {
                    response.P_COD_ERR = 1;
                    response.P_MESSAGE = resultValidate.P_MESSAGE;
                    LogControl.save("envioTecnicaPolizaMatriz", JsonConvert.SerializeObject(response, Formatting.None), "2");
                }
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = "Hubo un error al generar la cotización. Favor de comunicarse con sistemas";
                LogControl.save("envioTecnicaPolizaMatriz", ex.ToString(), "3");
            }

            return response;
        }

        [Route("InsertQuotation")]
        [HttpPost]
        public async Task<QuotationResponseVM> InsertQuotation()
        {
            var response = new QuotationResponseVM();
            var cotizacion = new QuotationCabBM();

            try
            {
                List<HttpPostedFile> files = new List<HttpPostedFile>();
                foreach (var item in HttpContext.Current.Request.Files)
                {
                    HttpPostedFile arch = HttpContext.Current.Request.Files[item.ToString()];
                    files.Add((HttpPostedFile)HttpContext.Current.Request.Files[item.ToString()]);
                }

                LogControl.save("InsertQuotation", HttpContext.Current.Request.Params.Get("objeto"), "2");

                // Deserializar objetos
                cotizacion = JsonConvert.DeserializeObject<QuotationCabBM>(HttpContext.Current.Request.Params.Get("objeto"));
                var request_EPS = HttpContext.Current.Request.Params.Get("objeto_EPS") != null ? JsonConvert.DeserializeObject<dataQuotation_EPS>(HttpContext.Current.Request.Params.Get("objeto_EPS")) : null;

                LogControl.save(cotizacion.CodigoProceso, "InsertQuotation Ini: " + JsonConvert.SerializeObject(cotizacion, Formatting.None), "2");

                cotizacion = changeInfoEcommerce(cotizacion);

                LogControl.save(cotizacion.CodigoProceso, "InsertQuotation Ecommerce: " + JsonConvert.SerializeObject(cotizacion, Formatting.None), "2");

                // No se pa que sirve response.P_NID_COTIZACION nunca tiene valor ¿?
                //var rutaFinalEval = ELog.obtainConfig("pathCotizacion") + response.P_NID_COTIZACION + "\\" + ELog.obtainConfig("cotizacionKey") + "\\";
                //response = sharedMethods.ValidatePath<QuotationResponseVM>(response, rutaFinalEval, files);
                //if (response.P_COD_ERR == 1)
                //    return response;


                //if (files.Count > 0 && cotizacion.FlagCotEstado == 2)
                //{
                //    var folderPath = ELog.obtainConfig("pathCotizacion") + response.P_NID_COTIZACION + "\\" + ELog.obtainConfig("movimiento") + "\\" + DateTime.Now.ToString("yyyyMMddhhmmss") + "\\";

                //    response = sharedMethods.ValidatePath<QuotationResponseVM>(response, folderPath, files);
                //    if (response.P_COD_ERR == 1)
                //        return response;

                //}


                //if (files.Count > 0 && cotizacion.FlagCotEstado == 1)
                //{
                //    var folderPath = ELog.obtainConfig("pathTramite") + response.P_NID_COTIZACION + "\\" + ELog.obtainConfig("movimiento") + "\\" + DateTime.Now.ToString("yyyyMMddhhmmss") + "\\";

                //    response = sharedMethods.ValidatePath<QuotationResponseVM>(response, folderPath, files);
                //    if (response.P_COD_ERR == 1)
                //        return response;
                //}

                response = insertObjDes(ref cotizacion, response);
                var validaPlanSeleccionado = objCambioPlan(HttpContext.Current.Request.Params.Get("objetoValidaCambioPlan"));
                bool flagIsNull = HttpContext.Current.Request.Params.Get("objetoValidaCambioPlan") == null ? true : false;

                response = response.P_COD_ERR == 0 ? gestionarMapfre(ref cotizacion, 1, ref response) : response;

                response = response.P_COD_ERR == 0 ? infoChangePlan(flagIsNull, validaPlanSeleccionado, ref cotizacion) : response;

                response = response.P_COD_ERR == 0 ? cotizacion.TrxCode == "EN" ? procesarEndosos(cotizacion) : await procesarCotizacion(cotizacion, files, request_EPS) : response;

                response = response.P_COD_ERR == 0 ? gestionarMapfre(ref cotizacion, 2, ref response) : response;

                response = response.P_COD_ERR == 0 ? insertHistorial(cotizacion, ref response) : response;

                response = response.P_COD_ERR == 0 && response.P_SAPROBADO == "A" ? await derivacionTecnica(cotizacion, validaPlanSeleccionado, response) : response;

                response = response.P_COD_ERR == 0 ? saveQuotationEps(response, cotizacion, request_EPS) : response; // AGF 24/12/2023 Cotizacion SABSA (TODO ESTE CODIGO DE SABASA LO CREAMOS DE FORMA ASINCRONA)

            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = ex.ToString();
                LogControl.save("InsertQuotation", ex.ToString(), "3");
            }

            if (response.P_COD_ERR != 1)
            {
                response.P_MESSAGE = response.P_MESSAGE == "null" ? string.Empty : response.P_MESSAGE;
            }

            LogControl.save(cotizacion.CodigoProceso, "InsertQuotation Fin: " + JsonConvert.SerializeObject(response, Formatting.None), "2");

            return response;
        }

        private QuotationCabBM changeInfoEcommerce(QuotationCabBM data)
        {
            var lRamos = new string[] { ELog.obtainConfig("vidaLeyBranch") };

            if (lRamos.Contains(data.P_NBRANCH.ToString()) && data.P_NUSERCODE == 3822)
            {
                var cantMeses = new QuotationCORE().cantidadMeses(Convert.ToInt32(data.P_NTIP_RENOV), data.P_NBRANCH);

                foreach (var item in data.QuotationDet)
                {
                    item.P_NSUM_PREMIUMN = item.P_NSUM_PREMIUMN / cantMeses;
                    item.P_NSUM_IGV = item.P_NSUM_IGV / cantMeses;
                    item.P_NSUM_PREMIUM = item.P_NSUM_PREMIUM / cantMeses;
                }
            }

            return data;
        }

        private QuotationResponseVM saveQuotationEps(QuotationResponseVM response, QuotationCabBM cotizacion, dataQuotation_EPS request_EPS)
        {
            /*if (cotizacion.P_NEPS == Convert.ToInt32(ELog.obtainConfig("grandiaKey")) && cotizacion.P_NBRANCH == Convert.ToInt32(ELog.obtainConfig("sctrBranch")) && (cotizacion.P_NCOT_MIXTA == 1 || cotizacion.P_NPRODUCT == 2)) //AVS - PRY INTERCONEXION SABSA 20 / 07 / 2023
            {
                quotationCORE.saveQuotationEps(response, cotizacion, request_EPS);
            }*/

            if (cotizacion.P_NBRANCH == Convert.ToInt32(ELog.obtainConfig("sctrBranch"))) //AVS - PRY INTERCONEXION SABSA 20 / 07 / 2023
            {
                quotationCORE.saveQuotationEps(response, cotizacion, request_EPS);
            }

            return response;
        }

        private async Task<QuotationResponseVM> derivacionTecnica(QuotationCabBM cotizacion, ValidaCambioPlanBM validaPlanSeleccionado, QuotationResponseVM response)
        {
            // Envio de notificacion al cotizador
            var lRamosN = new string[] { ELog.obtainConfig("vidaLeyBranch") };

            // Envio de notificacion al cotizador Graphql
            var lRamosGraphql = new string[] { ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch") };


            // Enviar notificacion
            if (lRamosN.Contains(cotizacion.P_NBRANCH.ToString()))
            {
                var responseSendNotificartionDevmente = new GenericResponseVM();

                if (cotizacion.P_SPOL_ESTADO == 1 && cotizacion.P_SPOL_MATRIZ == 0 && cotizacion.TrxCode == "EM")
                {
                    responseSendNotificartionDevmente = await SendDataQuotationToDevmente(response.P_NID_COTIZACION, 0, validaPlanSeleccionado.TipoTransaccion);

                }
                else if (cotizacion.P_SPOL_ESTADO == 1 && cotizacion.P_SPOL_MATRIZ == 1 && cotizacion.TrxCode == "EM")
                {
                    responseSendNotificartionDevmente = await SendDataQuotationToDevmenteTramitesMatriz(Convert.ToInt32(response.P_NID_COTIZACION), 0, validaPlanSeleccionado.TipoTransaccion);
                }
                else
                {
                    responseSendNotificartionDevmente = await SendDataQuotationToDevmente(response.P_NID_COTIZACION, cotizacion.NumeroCotizacion, validaPlanSeleccionado.TipoTransaccion);
                }
                if (responseSendNotificartionDevmente.ErrorCode == 1)
                {
                    response.P_SMESSAGE = response.P_SMESSAGE + Environment.NewLine + responseSendNotificartionDevmente.MessageError;
                }
            }

            // Enviar notificacion
            if (lRamosGraphql.Contains(cotizacion.P_NBRANCH.ToString()))
            {
                var responseGraphql = await quotationCORE.SendDataQuotationGraphql(response.P_NID_COTIZACION, cotizacion.TrxCode, cotizacion.P_NPOLIZA_MATRIZ, string.Empty);

                if (responseGraphql.codError == 1)
                {
                    response.P_SMESSAGE = response.P_SMESSAGE + Environment.NewLine + responseGraphql.message;
                }
            }

            return response;
        }

        private QuotationResponseVM insertHistorial(QuotationCabBM cotizacion, ref QuotationResponseVM response)
        {
            // Inserción en historial - Revisarcovid
            var lRamosH = new string[] { ELog.obtainConfig("vidaLeyBranch"), ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch"), ELog.obtainConfig("vidaIndividualBranch"), ELog.obtainConfig("desgravamenBranch") };

            // Insertar historial
            if (lRamosH.Contains(cotizacion.P_NBRANCH.ToString()))
            {
                if (cotizacion.P_NPOLIZA_MATRIZ != 1 && cotizacion.FlagCotEstado != 1)
                {
                    string typetransc = cotizacion.P_SPOL_ESTADO == 1 && cotizacion.TrxCode == "EM" ? "0" : cotizacion.NumeroCotizacion != 0 && cotizacion.NumeroCotizacion != null ? "10" : "0";

                    var trama = new ValidaTramaBM()
                    {
                        NID_COTIZACION = Int32.Parse(response.P_NID_COTIZACION),
                        NTYPE_TRANSAC = typetransc,
                        NID_PROC = cotizacion.CodigoProceso
                    };

                    quotationCORE.insertHistTrama(trama);
                }
            }

            return response;
        }

        private async Task<QuotationResponseVM> procesarCotizacion(QuotationCabBM cotizacion, List<HttpPostedFile> files, dataQuotation_EPS request_EPS)
        {
            var response = new QuotationResponseVM();

            response = await quotationCORE.InsertQuotation(cotizacion, files, request_EPS);

            return response;
        }

        private QuotationResponseVM procesarEndosos(QuotationCabBM cotizacion)
        {
            var response = new QuotationResponseVM();

            if (cotizacion.P_NBRANCH == Convert.ToInt64(ELog.obtainConfig("vidaLeyBranch")))
            {

                response = policyCORE.ProcesarEndosoVL(cotizacion);
                response.P_NID_COTIZACION = cotizacion.NumeroCotizacion.ToString();
            }

            return response;
        }

        private QuotationResponseVM infoChangePlan(bool flagIsNull, ValidaCambioPlanBM validaPlanSeleccionado, ref QuotationCabBM cotizacion)
        {
            var response = new QuotationResponseVM();

            if (!flagIsNull)
            {
                PlanBM data = new PlanBM
                {
                    P_NBRANCH = cotizacion.P_NBRANCH,
                    P_NPRODUCT = cotizacion.P_NPRODUCT,
                    P_NTIP_RENOV = 0,
                    P_NCURRENCY = cotizacion.P_NCURRENCY,
                    P_NIDPLAN = 0
                };

                var planes = this.GetPlanList(data);
                var miPlan = planes.FirstOrDefault(m => m.SDESCRIPT.ToUpper().Trim() == validaPlanSeleccionado.PlanSeleccionado.Trim().ToUpper());

                cotizacion.planId = miPlan != null ? Convert.ToInt32(miPlan.NIDPLAN) : 0;

                //try
                //{
                //    cotizacion.planId = Convert.ToInt32(miPlan.NIDPLAN);
                //}
                //catch (Exception)
                //{
                //    cotizacion.planId = 0;
                //}

            }

            if (validaPlanSeleccionado.PlanPropuesto != validaPlanSeleccionado.PlanSeleccionado)
            {
                cotizacion.P_NCAMBIOPLAN = 1;
            }

            return response;
        }


        private QuotationResponseVM gestionarMapfre(ref QuotationCabBM cotizacion, int typeProcess, ref QuotationResponseVM response)
        {
            //var response = new QuotationResponseVM();

            if (cotizacion.cotizacionMapfre != null)
            {
                if (typeProcess == 1) // Insertar cotizacion en Mapfre
                {
                    var resMapfre = mapfreController.GesCotizacionMapfre(cotizacion.cotizacionMapfre);

                    response.P_COD_ERR = Int32.Parse(resMapfre.Result.codError);
                    response.P_MESSAGE = resMapfre.Result.descError;

                    if (response.P_COD_ERR == 0)
                    {
                        cotizacion.cotizacionMapfre.cotizacion = new CotizacionBM();
                        cotizacion.cotizacionMapfre.cotizacion.numCotizacion = resMapfre.Result.cotizacion.numCotizacion;
                        cotizacion.P_QUOTATIONNUMBER_EPS = resMapfre.Result.cotizacion.numCotizacion;
                    }
                }

                if (typeProcess == 2) // Actualizar cotizacion mapfre
                {
                    if (response.P_COD_ERR == 0 && response.P_SAPROBADO != "S")
                    {
                        var listSalud = cotizacion.QuotationDet.Where(x => x.P_NPRODUCT == Convert.ToInt32(ELog.obtainConfig("saludKey")) &&
                        x.P_NBRANCH == Convert.ToInt32(ELog.obtainConfig("sctrBranch"))).ToList();

                        if (listSalud.Count > 0)
                        {
                            decimal? tasaPropuesta = listSalud[0].P_NTASA_PROP;

                            if (tasaPropuesta != null)
                            {
                                cotizacion.cotizacionMapfre.cabecera.keyService = "ajustarTasa";
                                cotizacion.cotizacionMapfre.cotizacion.dscMotivoAjuste = String.Format(ELog.obtainConfig("dscMotivoAjuste"), tasaPropuesta);
                                var responseAjuste = mapfreController.GesCotizacionMapfre(cotizacion.cotizacionMapfre);
                            }
                        }
                    }
                }

            }

            return response;
        }

        private ValidaCambioPlanBM objCambioPlan(string data)
        {
            var objPlan = new ValidaCambioPlanBM();

            if (HttpContext.Current.Request.Params.Get("objetoValidaCambioPlan") == null)
            {
                objPlan = new ValidaCambioPlanBM
                {
                    PlanPropuesto = "",
                    PlanSeleccionado = "",
                    TipoTransaccion = "EMISION"
                };
            }
            else
            {
                objPlan = JsonConvert.DeserializeObject<ValidaCambioPlanBM>(HttpContext.Current.Request.Params.Get("objetoValidaCambioPlan"));
            }

            return objPlan;
        }

        private QuotationResponseVM insertObjDes(ref QuotationCabBM cotizacion, QuotationResponseVM response)
        {
            if (cotizacion.P_NBRANCH.ToString() == ELog.obtainConfig("vidaIndividualBranch"))
            {
                LogControl.save("InsertQuotation", HttpContext.Current.Request.Params.Get("objDes"), "2");
                var precotiza = JsonConvert.DeserializeObject<validaTramaVM>(HttpContext.Current.Request.Params.Get("objDes"));
                precotiza.codProceso = cotizacion.CodigoProceso; // AGF  24052023 Guardando el cod proces de trama para VCF

                if (precotiza != null)
                {
                    var calCotiza = CalcularPrima(precotiza);

                    response.P_COD_ERR = Int32.Parse(calCotiza.Result.P_COD_ERR);
                    response.P_MESSAGE = calCotiza.Result.P_MESSAGE;

                    if (response.P_COD_ERR == 0)
                    {
                        cotizacion.CodigoProceso = calCotiza.Result.NIDPROC;
                        cotizacion.QuotationCli[0].externalId = calCotiza.Result.NIDPROC;

                        cotizacion.QuotationDet[0].P_NPREMIUM_MIN = Convert.ToDecimal(calCotiza.Result.PRIMA);
                        cotizacion.QuotationDet[0].P_NPREMIUM_MIN_PR = Convert.ToDecimal(calCotiza.Result.PRIMA);
                        cotizacion.QuotationDet[0].P_NAMO_AFEC = Convert.ToDecimal(calCotiza.Result.PRIMA);
                        cotizacion.QuotationDet[0].P_NAMOUNT = Convert.ToDecimal(calCotiza.Result.PRIMA);
                        cotizacion.QuotationCom[0].P_NCOMISION_SAL = Convert.ToDecimal(calCotiza.Result.COMISION_BROKER);
                        cotizacion.QuotationCom[0].P_NCOMISION_SAL_PR = Convert.ToDecimal(calCotiza.Result.COMISION_BROKER);
                    }
                }
            }

            return response;
        }

        //private async Task<QuotationResponseVM> insertObjDessss(ref QuotationCabBM cotizacion, QuotationResponseVM response)
        //{

        //}

        private async Task<GenericResponseVM> SendDataQuotationToDevmente(string P_NID_COTIZACION_Generado, int? NumeroCotizacion_existente, string tipoTransaccion, int evaluar = 0)
        {
            GenericResponseVM responseVM = new GenericResponseVM();

            try
            {
                //Invocar servicio de técnica
                var _tipo_cotizacion = "EMISION";
                if (NumeroCotizacion_existente != null && NumeroCotizacion_existente.Value != 0)
                {
                    _tipo_cotizacion = "RECOTIZACION";
                }
                if (NumeroCotizacion_existente != null && NumeroCotizacion_existente.Value != 0 && tipoTransaccion == "Declaración")
                {
                    _tipo_cotizacion = "DECLARACION";
                }
                if (NumeroCotizacion_existente != null && NumeroCotizacion_existente.Value != 0 && tipoTransaccion == "INCLUSION")
                {
                    _tipo_cotizacion = "INCLUSION";
                }
                IntegrateToDevmente integrateToDevmente = new IntegrateToDevmente();
                responseVM = await integrateToDevmente.Execute(evaluar == 0 ? _tipo_cotizacion : tipoTransaccion, Convert.ToInt32(P_NID_COTIZACION_Generado));
                //var data_request = quotationCORE.ReadInfoQuotationDM(Convert.ToInt32(P_NID_COTIZACION_Generado));

                //if (data_request.Header.NUM_COTIZACION > 0)
                //{
                //    SendNotificationTecnicaRequest requestNotification = new SendNotificationTecnicaRequest();
                //    requestNotification.detail = new Detail()
                //    {
                //        payroll = new List<Payroll>()
                //    };
                //    requestNotification.header = new Header
                //    {
                //        number = P_NID_COTIZACION_Generado,
                //        brokerCode = data_request.Header.COD_BROKER.ToString(),
                //        brokerName = data_request.Header.DES_BROKER,
                //        contractorCode = data_request.Header.COD_CONTRATANTE.ToString(),
                //        contractorName = data_request.Header.DES_CONTRATANTE,
                //        associatedEconomyActivityCode = data_request.Header.COD_ACT_TEC_ASOC.ToString(),
                //        associatedEconomyActivityName = data_request.Header.DES_ACT_TEC_ASOC,
                //        economicActivityCode = data_request.Header.COD_ACT_TEC.ToString(),
                //        economicActivityName = data_request.Header.DES_ACT_TEC.ToString(),
                //        endValidity = this.GetSortableDateTime(data_request.Header.FIN_VIG),
                //        initialValidity = this.GetSortableDateTime(data_request.Header.INI_VIG),
                //        plan = data_request.Header.COD_PLAN.ToString(),
                //        user = data_request.Header.COD_USUARIO.ToString(),
                //        quotationType = _tipo_cotizacion, // "EMISION",
                //        quotationRequestComment = data_request.Header.COMENTARIO,
                //        proposedComercialRate = new List<ProposedComercialRate>(),
                //        quotationRequestDate = this.GetSortableDateTime(DateTime.Now.Date),
                //        quotationSendedDate = this.GetSortableDateTime(DateTime.Now.Date)
                //    };

                //    foreach (var item in data_request.DetailTasas)
                //    {
                //        requestNotification.header.proposedComercialRate.Add(new ProposedComercialRate { description = item.DES_MODULEC, rate = (double)item.TASA_PROPUESTA });
                //    }

                //    foreach (var item in data_request.Detail)
                //    {
                //        requestNotification.detail.payroll.Add(new Payroll
                //        {
                //            birthDate = this.GetSortableDateTime(item.FEC_NACIMIENTO),
                //            documentNumber = item.NUM_DOCUMENTO,
                //            documentType = item.TIPO_DOC,
                //            gender = item.GENERO,
                //            id = item.ID_REG,
                //            name = item.NOMBRE,
                //            surname1 = item.APE_PAT,
                //            surname2 = item.APE_NAT,
                //            salary = item.REMUNERACION,
                //            workerType = new WorkerType
                //            {
                //                code_core = item.TIPO_TRABAJADOR.ToString(),
                //                description = item.DES_TRABAJADOR
                //            },
                //            status = item.STATUS,
                //            countryOfBirth = item.PAIS_NACIMIENTO
                //        });
                //    }

                //    WebServiceUtil webServiceUtil = new WebServiceUtil();
                //    var URI_SendNotification = "https://lwlt-quotator-api.staging.devmente.com/quotation-request/request/" + _tipo_cotizacion + "/" + requestNotification.header.number;
                //    var response_SendNotification = await webServiceUtil.CallApiService(JsonConvert.SerializeObject(requestNotification), URI_SendNotification);

                //    ELog.save(URI_SendNotification + Environment.NewLine + Environment.NewLine +
                //        JsonConvert.SerializeObject(requestNotification, Formatting.None) + Environment.NewLine + Environment.NewLine +
                //        "Respuesta => " + response_SendNotification.Body);

                //    if (response_SendNotification.Status != HttpStatusCode.OK)
                //    {
                //        ELog.save("Error: caida de integración con devmente. " 
                //            + Environment.NewLine 
                //            + "No se pudo establecer conexión con URL " + URI_SendNotification);

                //        responseVM.ErrorCode = 1;
                //        responseVM.MessageError = "Hubo un error en el envió de la cotización a técnica. Favor de comunicarse con sistemas.";

                //        return responseVM;
                //    }

                //    foreach (var item in HttpContext.Current.Request.Files)
                //    {
                //        HttpPostedFile arch = HttpContext.Current.Request.Files[item.ToString()];

                //        var objGetUrlUploadFilesRequest = new GetUrlUploadFilesRequest
                //        {
                //            number = data_request.Header.NUM_COTIZACION.ToString(),
                //            comment = data_request.Header.COMENTARIO,
                //            extension = arch.FileName.Split('.').Last(),
                //            quotationType = _tipo_cotizacion, //"EMISION",
                //            name = Path.GetFileNameWithoutExtension(arch.FileName)
                //        };
                //        var URI_GetUrlUploadFiles = "https://lwlt-quotator-api.staging.devmente.com/quotation-request/attachment/create";
                //        var Response_GetUrlUploadFiles = await webServiceUtil.CallApiService(JsonConvert.SerializeObject(objGetUrlUploadFilesRequest), URI_GetUrlUploadFiles);

                //        if (Response_GetUrlUploadFiles.Status == HttpStatusCode.OK)
                //        {
                //            var _resp_obj_UrlUpload = JsonConvert.DeserializeObject<GetUrlUploadFilesResponse>(Response_GetUrlUploadFiles.Body);
                //            if (_resp_obj_UrlUpload.code == 200 && !string.IsNullOrWhiteSpace(_resp_obj_UrlUpload.data))
                //            {
                //                await webServiceUtil.invocarServicioWithFiles(string.Empty, _resp_obj_UrlUpload.data, arch);
                //            }
                //            else
                //            {
                //                responseVM.ErrorCode = 1;
                //                responseVM.MessageError = "Hubo un error en el envió de la cotización a técnica. Favor de comunicarse con sistemas.";

                //                return responseVM;
                //            }
                //        }
                //        else
                //        {
                //            ELog.save("Error: caida de integración con devmente. " + Environment.NewLine + "No se pudo establecer conexión con URL " + URI_GetUrlUploadFiles);

                //            responseVM.ErrorCode = 1;
                //            responseVM.MessageError = "Hubo un error en el envió de la cotización a técnica. Favor de comunicarse con sistemas.";

                //            return responseVM;
                //        }
                //    }
                //}

            }
            catch (Exception ex)
            {
                LogControl.save("SendDataQuotationToDevmente", ex.ToString(), "3");
                responseVM.ErrorCode = 1;
                responseVM.MessageError = "Hubo un error en el envió de la cotización a técnica. Favor de comunicarse con sistemas.";
            }

            return responseVM;
        }
        private async Task<GenericResponseVM> SendDataQuotationToDevmenteTramitesMatriz(int P_NID_COTIZACION_Generado, int? NumeroCotizacion_existente, string tipoTransaccion, int evaluar = 0, bool revaluarTecnica = false, string comentarioReevaluar = "", string ruta = "")
        {
            GenericResponseVM responseVM = new GenericResponseVM();

            try
            {
                //Invocar servicio de técnica
                var _tipo_cotizacion = "EMISION";
                if (NumeroCotizacion_existente != null && NumeroCotizacion_existente.Value != 0)
                {
                    _tipo_cotizacion = "RECOTIZACION";
                }
                if (NumeroCotizacion_existente != null && NumeroCotizacion_existente.Value != 0 && tipoTransaccion == "Declaración")
                {
                    _tipo_cotizacion = "DECLARACION";
                }
                if (NumeroCotizacion_existente != null && NumeroCotizacion_existente.Value != 0 && tipoTransaccion == "INCLUSION")
                {
                    _tipo_cotizacion = "INCLUSION";
                }
                IntegrateToDevmente integrateToDevmente = new IntegrateToDevmente();
                responseVM = await integrateToDevmente.ExecuteTramitesMatriz(evaluar == 0 ? _tipo_cotizacion : tipoTransaccion, P_NID_COTIZACION_Generado, ruta, revaluarTecnica: revaluarTecnica, comentarioReevaluar: comentarioReevaluar);

            }
            catch (Exception ex)
            {
                LogControl.save("SendDataQuotationToDevmenteTramitesMatriz", ex.ToString(), "3");
                responseVM.ErrorCode = 1;
                responseVM.MessageError = "Hubo un error en el envió de la cotización a técnica. Favor de comunicarse con sistemas.";
            }

            return responseVM;
        }

        private string GetSortableDateTime(DateTime dt)
        {
            var dateStr = String.Format("{0:s}", dt);
            return dateStr;
        }

        private double GetMilisecondsOfDate(DateTime dt)
        {
            var milisegundos = DateTime
                                .UtcNow
                                .Subtract(new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0, DateTimeKind.Utc))
                                .TotalMilliseconds;
            return milisegundos;
        }

        [Route("ValidarReglasPago")]
        [HttpPost]
        public async Task<QuotationResponseVM> ValidarReglasPago(ValidarPagoBM request)
        {
            var response = new QuotationResponseVM();
            response = quotationCORE.ValidarReglasPago(request);
            return response;
        }

        [Route("ReQuotationVL")]
        [HttpPost]
        public async Task<QuotationResponseVM> ReQuotationVL(InfoPropBM request)
        {
            var response = new QuotationResponseVM();
            var responseSendNotificartionDevmente = new GenericResponseVM();
            try
            {
                //var cotizacion = JsonConvert.DeserializeObject<InfoAuthBM>(HttpContext.Current.Request.Params.Get("objeto"));
                if (request.Recotizar == 1)
                {
                    response = quotationCORE.UpdateReQuotation(request);
                }
                if (request.Retroactividad == 1)
                {
                    QuotationCabBM quotationCab = new QuotationCabBM()
                    {
                        P_NBRANCH = Convert.ToInt32(ELog.obtainConfig("vidaLeyBranch")),
                        P_NPRODUCT = Convert.ToInt32(ELog.obtainConfig("vidaLeyKey")),
                        NumeroCotizacion = request.QuotationNumber,
                        P_NUSERCODE = Convert.ToInt32(request.UserCode),
                        P_DSTARTDATE_ASE = request.DateEfect,
                        P_DSTARTDATE = request.DateEfect,
                        TrxCode = "IN",
                        RetOP = 2,
                        FlagCambioFecha = request.FlagCambioFecha
                    };
                    response = quotationCORE.ValidateRetroactivity(quotationCab);
                    if (response.P_NCODE == 4)
                    {
                        response.P_MESSAGE = "Se procede con la derivación al área técnica.";
                        //udpate fecha
                        quotationCORE.UpdateFechaEfectoAsegurado(quotationCab);
                    }
                }
                //--- Aquí solo se RECOTIZA una Inclusión
                responseSendNotificartionDevmente = await SendDataQuotationToDevmente(request.QuotationNumber.ToString(), (int?)request.QuotationNumber, "RECOTIZACION");
                if (responseSendNotificartionDevmente.ErrorCode == 1)
                {
                    response.P_MESSAGE = response.P_MESSAGE + Environment.NewLine + responseSendNotificartionDevmente.MessageError;
                }
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = ex.Message;
                LogControl.save("ReQuotationVL", ex.ToString(), "3");
            }
            return response;
        }

        [Route("SearchBroker")]
        [HttpPost]
        public BrokerResponseVM SearchBroker(BrokerSearchBM data)
        {
            return quotationCORE.SearchBroker(data);
        }

        [Route("GetStatusList")]
        [HttpGet]
        public List<StatusVM> GetStatusList(string certype, string codProduct)
        {
            return quotationCORE.GetStatusList(certype, codProduct);
        }

        [Route("GetReasonList")]
        [HttpGet]
        public List<ReasonVM> GetReasonList(Int32 statusCode, string branch)
        {
            return quotationCORE.GetReasonList(statusCode, branch);
        }

        [Route("ApproveQuotation")]
        [HttpPost]
        public QuotationResponseVM ApproveQuotation(QuotationUpdateBM data)
        {
            return quotationCORE.ApproveQuotation(data);
        }

        [Route("ModifyQuotation")]
        [HttpPost]
        public QuotationResponseVM ModifyQuotation()
        {
            var response = new QuotationResponseVM();
            try
            {
                List<HttpPostedFile> files = new List<HttpPostedFile>();
                foreach (var item in HttpContext.Current.Request.Files)
                {
                    HttpPostedFile arch = HttpContext.Current.Request.Files[item.ToString()];
                    files.Add((HttpPostedFile)HttpContext.Current.Request.Files[item.ToString()]);
                }

                var objeto = JsonConvert.DeserializeObject<QuotationModification>(HttpContext.Current.Request.Params.Get("quotationModification"));
                if (files.Count > 0)
                {
                    var folderPath = ELog.obtainConfig("pathCotizacion") + objeto.StatusChangeData.QuotationNumber + "\\" + ELog.obtainConfig("movimiento") + "\\" + DateTime.Now.ToString("yyyyMMddhhmmss") + "\\";
                    response = sharedMethods.ValidatePath<QuotationResponseVM>(response, folderPath, files);
                    if (response.P_COD_ERR == 1)
                        return response;
                }
                response = quotationCORE.ModifyQuotation(objeto, files);
            }
            catch (Exception ex)
            {
                LogControl.save("ModifyQuotation", ex.ToString(), "3");
            }
            return response;
        }
        [Route("EquivalentMunicipality")]
        [HttpGet]
        public String equivalentMunicipality(Int64 municipality)
        {
            return quotationCORE.equivalentMunicipality(municipality);
        }

        [Route("GetIgv")]
        [HttpPost]
        public string GetIgv(QuotationIGVBM data)
        {
            return quotationCORE.GetIgv(data);
        }

        [Route("GetTariff")]
        [HttpPost]
        public async Task<TariffVM> GetTariff(TariffBM data)
        {
            TariffVM tariff = new TariffVM();

            if (data.protectaTariff != null)
            {
                data.protectaTariff.queryDate = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss.fffZ");

                if (data.protectaTariff.workers > 0)
                {
                    tariff = await quotationCORE.GetTariff(data.protectaTariff);
                }
                else
                {
                    //var json = "{\"fields\":[{\"id\":\"2lpYdVqA1uOwiUSW8gfelD\",\"tariff\":\"tarifario pruebas 18-12-2023\",\"field\":\"SALUD\",\"fieldEquivalenceCore\":\"2\",\"areaGroup\":\"Nacional\",\"activityGroup\":\"\",\"enterprise\":[{\"size\":\"(<) Menor a 50\",\"minimumPremium\":110.0,\"minimumPremiumEndoso\":0.0,\"netRate\":[{\"id\":\"4cUZhWEeLjDXp9hvcuI2VT\",\"description\":\"Flat\",\"rate\":0.000},{\"id\":\"dv2VgPdjqxbTwCQEV5zT3\",\"description\":\"Administrativo\",\"rate\":0.000},{\"id\":\"15kAG8g1oiUNR1tfxOElzb\",\"description\":\"Medio\",\"rate\":0.000},{\"id\":\"6d1hyHQdyFF70q0YR5NeE6\",\"description\":\"Obrero\",\"rate\":0.000}],\"riskRate\":[{\"id\":\"4cUZhWEeLjDXp9hvcuI2VT\",\"description\":\"Flat\",\"rate\":0.000},{\"id\":\"dv2VgPdjqxbTwCQEV5zT3\",\"description\":\"Administrativo\",\"rate\":0.000},{\"id\":\"15kAG8g1oiUNR1tfxOElzb\",\"description\":\"Medio\",\"rate\":0.000},{\"id\":\"6d1hyHQdyFF70q0YR5NeE6\",\"description\":\"Obrero\",\"rate\":0.000}]}],\"channels\":[{\"roleType\":\"BROKER\",\"roleDescription\":\"\",\"roleId\":\"2016000019\"}],\"channelDistributions\":[{\"roleId\":\"2016000019\",\"distribution\":\"100\"}],\"activityVariation\":0.0,\"commission\":\"10\"},{\"id\":\"6HflpsVpymokCdaJ1caoVT\",\"tariff\":\"tarifario pruebas 18-12-2023\",\"field\":\"PENSIÓN\",\"fieldEquivalenceCore\":\"1\",\"areaGroup\":\"Nacional\",\"activityGroup\":\"\",\"enterprise\":[{\"size\":\"(<) Menor a 50\",\"minimumPremium\":0.0,\"minimumPremiumEndoso\":0.0,\"netRate\":[{\"id\":\"4cUZhWEeLjDXp9hvcuI2VT\",\"description\":\"Flat\",\"rate\":0.000},{\"id\":\"dv2VgPdjqxbTwCQEV5zT3\",\"description\":\"Administrativo\",\"rate\":0.000},{\"id\":\"15kAG8g1oiUNR1tfxOElzb\",\"description\":\"Medio\",\"rate\":0.000},{\"id\":\"6d1hyHQdyFF70q0YR5NeE6\",\"description\":\"Obrero\",\"rate\":0.000}],\"riskRate\":[{\"id\":\"4cUZhWEeLjDXp9hvcuI2VT\",\"description\":\"Flat\",\"rate\":0.000},{\"id\":\"dv2VgPdjqxbTwCQEV5zT3\",\"description\":\"Administrativo\",\"rate\":0.000},{\"id\":\"15kAG8g1oiUNR1tfxOElzb\",\"description\":\"Medio\",\"rate\":0.000},{\"id\":\"6d1hyHQdyFF70q0YR5NeE6\",\"description\":\"Obrero\",\"rate\":0.000}]}],\"channels\":[{\"roleType\":\"BROKER\",\"roleDescription\":\"\",\"roleId\":\"2016000019\"}],\"channelDistributions\":[{\"roleId\":\"2016000019\",\"distribution\":\"100\"}],\"activityVariation\":2.0,\"commission\":\"0\"}]}";
                    var json = "{\"fields\":[{\"id\":\"2IStEWOB5z7OD7rTem0n2v\",\"tariff\":\"Tarifario demo\",\"field\":\"SALUD\",\"fieldEquivalenceCore\":\"2\",\"areaGroup\":\"Nacional\",\"activityGroup\":\"\",\"enterprise\":[{\"size\":\"(<) Menor a 50\",\"minimumPremium\":0,\"minimumPremiumEndoso\":0,\"netRate\":[{\"id\":\"2rJ3BaQk95sPqFFmvrHWY8\",\"description\":\"Flat\",\"rate\":0},{\"id\":\"5eMzsThBQcCP7rDcML7TWr\",\"description\":\"Administrativo\",\"rate\":0},{\"id\":\"2rJ3BaQk95sPqFFmvrHWY8\",\"description\":\"Medio\",\"rate\":0},{\"id\":\"4BWUfq1nbFbHwLvFor4Dla\",\"description\":\"Obrero\",\"rate\":0}],\"riskRate\":[{\"id\":\"2rJ3BaQk95sPqFFmvrHWY8\",\"description\":\"Flat\",\"rate\":0},{\"id\":\"5eMzsThBQcCP7rDcML7TWr\",\"description\":\"Administrativo\",\"rate\":0},{\"id\":\"2rJ3BaQk95sPqFFmvrHWY8\",\"description\":\"Medio\",\"rate\":0},{\"id\":\"4BWUfq1nbFbHwLvFor4Dla\",\"description\":\"Obrero\",\"rate\":0}]}],\"channels\":[{\"roleType\":\"BROKER\",\"roleDescription\":\"\",\"roleId\":\"2016000019\"}],\"channelDistributions\":[{\"roleId\":\"2016000019\",\"distribution\":\"100\"}],\"activityVariation\":0,\"commission\":\"0\"},{\"id\":\"XovRN5HicoLoA9RMaPat6\",\"tariff\":\"Tarifario demo\",\"field\":\"PENSIÓN\",\"fieldEquivalenceCore\":\"1\",\"areaGroup\":\"Nacional\",\"activityGroup\":\"\",\"enterprise\":[{\"size\":\"(<) Menor a 50\",\"minimumPremium\":0,\"minimumPremiumEndoso\":0,\"netRate\":[{\"id\":\"2rJ3BaQk95sPqFFmvrHWY8\",\"description\":\"Flat\",\"rate\":0},{\"id\":\"5eMzsThBQcCP7rDcML7TWr\",\"description\":\"Administrativo\",\"rate\":0},{\"id\":\"2rJ3BaQk95sPqFFmvrHWY8\",\"description\":\"Medio\",\"rate\":0},{\"id\":\"4BWUfq1nbFbHwLvFor4Dla\",\"description\":\"Obrero\",\"rate\":0}],\"riskRate\":[{\"id\":\"5eMzsThBQcCP7rDcML7TWr\",\"description\":\"Flat\",\"rate\":0},{\"id\":\"5eMzsThBQcCP7rDcML7TWr\",\"description\":\"Administrativo\",\"rate\":0},{\"id\":\"5eMzsThBQcCP7rDcML7TWr\",\"description\":\"Medio\",\"rate\":0},{\"id\":\"4BWUfq1nbFbHwLvFor4Dla\",\"description\":\"Obrero\",\"rate\":0}]}],\"channels\":[{\"roleType\":\"BROKER\",\"roleDescription\":\"\",\"roleId\":\"2016000019\"}],\"channelDistributions\":[{\"roleId\":\"2016000019\",\"distribution\":\"100\"}],\"activityVariation\":0,\"commission\":\"0\"}]}";


                    tariff = JsonConvert.DeserializeObject<TariffVM>(json, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                }

                foreach (var item in tariff.fields)
                {
                    if (item.fieldEquivalenceCore == ELog.obtainConfig("saludKey") ||
                            item.fieldEquivalenceCore == ELog.obtainConfig("saludOldKey"))
                    {
                        item.fieldEquivalenceCore = ELog.obtainConfig("saludKey");
                    }

                    if (item.fieldEquivalenceCore == ELog.obtainConfig("pensionKey") ||
                            item.fieldEquivalenceCore == ELog.obtainConfig("pensionOldKey"))
                    {
                        item.fieldEquivalenceCore = ELog.obtainConfig("pensionKey");
                    }

                    item.branch = ELog.obtainConfig("sctrBranch");
                }
            }

            if (data.mapfreTariff != null)
            {
                var responseMapfre = new CotizarVM();
                if (data.mapfreTariff.cabecera.tariff == true)
                {
                    data.mapfreTariff.riesgoSCTR[0].impPlanillaSalud = data.mapfreTariff.riesgoSCTR[0].impPlanillaSalud * Convert.ToInt32(ELog.obtainConfig("renovacionM" + data.mapfreTariff.poliza.codFormaDeclaracion));
                    responseMapfre = await mapfreController.GesCotizacionMapfre(data.mapfreTariff);
                }
                else
                {
                    responseMapfre = generateTasasMapfre(responseMapfre);
                }


                if (responseMapfre.codError == "0")
                {
                    if (responseMapfre.tasas != null && responseMapfre.tasas.Count > 0)
                    {
                        tariff = await procesarMapfre(tariff, responseMapfre, data.mapfreTariff);
                    }
                    else
                    {
                        responseMapfre = generateTasasMapfre(responseMapfre);
                        tariff = await procesarMapfre(tariff, responseMapfre, data.mapfreTariff);
                    }
                }
                else
                {
                    tariff.fields = null;
                    //responseMapfre = generateTasasMapfre(responseMapfre);
                    //tariff = await procesarMapfre(tariff, responseMapfre);
                }

            }

            return tariff;
        }

        public CotizarVM generateTasasMapfre(CotizarVM responseMapfre)
        {
            responseMapfre.cotizacion = new CotizacionVM();
            responseMapfre.cotizacion.conceptosDesgloseSalud = new ConceptosDesgloseVM();
            responseMapfre.cotizacion.conceptosDesgloseSalud.impPneta = 0;
            responseMapfre.cotizacion.conceptosDesgloseSalud.impInteres = 0;
            responseMapfre.cotizacion.conceptosDesgloseSalud.impRecargos = 0;
            responseMapfre.cotizacion.conceptosDesgloseSalud.impPnetaBoni = 0;
            responseMapfre.cotizacion.conceptosDesgloseSalud.impPrimaTotal = 0;
            responseMapfre.cotizacion.conceptosDesgloseSalud.impImptos = 0;
            responseMapfre.cotizacion.conceptosDesgloseSalud.impBoni = 0;
            responseMapfre.cotizacion.conceptosDesgloseSalud.impImptosInteres = 0;
            responseMapfre.cotizacion.numCotizacion = "0";
            responseMapfre.tasas = new List<TasaVM>();
            var tasas = new TasaVM();
            tasas.tasa = 0;
            responseMapfre.tasas.Add(tasas);
            responseMapfre.codError = "0";

            return responseMapfre;
        }

        public async Task<TariffVM> procesarMapfre(TariffVM dataProtecta, CotizarVM dataMapfre, CotizarBM mapfreTariff)
        {
            if (dataProtecta.fields != null)
            {
                if (dataProtecta.fields.Count > 0)
                {
                    foreach (var item in dataProtecta.fields)
                    {
                        if (item.fieldEquivalenceCore == ELog.obtainConfig("saludKey") ||
                            item.fieldEquivalenceCore == ELog.obtainConfig("saludOldKey"))
                        {
                            if (item.enterprise[0].netRate != null)
                            {
                                foreach (var rate in item.enterprise[0].netRate)
                                {
                                    foreach (var tasaMP in dataMapfre.tasas)
                                    {
                                        rate.rate = tasaMP.tasa / 100;
                                        rate.premiumMonth = calculateAmmount(dataMapfre.cotizacion.conceptosDesgloseSalud.impPneta, mapfreTariff); // dataMapfre.cotizacion.conceptosDesgloseSalud.impPneta;
                                    }
                                }
                            }

                            if (item.enterprise[0].riskRate != null)
                            {
                                foreach (var rate in item.enterprise[0].riskRate)
                                {
                                    foreach (var tasaMP in dataMapfre.tasas)
                                    {
                                        rate.rate = tasaMP.tasa / 100;
                                    }
                                }
                            }

                            item.discount = "0";
                            item.activityVariation = 0;
                            item.commission = "0";

                            item.channels = null;
                            item.channelDistributions = null;
                            item.enterprise[0].minimumPremium = Convert.ToDouble(ELog.obtainConfig("primaMinimaMapfre"));
                            item.enterprise[0].minimumPremiumEndoso = 0;
                            item.enterprise[0].impPneta = calculateAmmount(dataMapfre.cotizacion.conceptosDesgloseSalud.impPneta, mapfreTariff);
                            item.enterprise[0].impInteres = calculateAmmount(dataMapfre.cotizacion.conceptosDesgloseSalud.impInteres, mapfreTariff);  //dataMapfre.cotizacion.conceptosDesgloseSalud.impInteres;
                            item.enterprise[0].impRecargos = calculateAmmount(dataMapfre.cotizacion.conceptosDesgloseSalud.impRecargos, mapfreTariff); // dataMapfre.cotizacion.conceptosDesgloseSalud.impRecargos;
                            item.enterprise[0].impPnetaBoni = calculateAmmount(dataMapfre.cotizacion.conceptosDesgloseSalud.impPnetaBoni, mapfreTariff);  // dataMapfre.cotizacion.conceptosDesgloseSalud.impPnetaBoni;
                            item.enterprise[0].impPrimaTotal = calculateAmmount(dataMapfre.cotizacion.conceptosDesgloseSalud.impPrimaTotal, mapfreTariff);  // dataMapfre.cotizacion.conceptosDesgloseSalud.impPrimaTotal;
                            item.enterprise[0].impImptos = calculateAmmount(dataMapfre.cotizacion.conceptosDesgloseSalud.impImptos, mapfreTariff);  // dataMapfre.cotizacion.conceptosDesgloseSalud.impImptos;
                            item.enterprise[0].impBoni = calculateAmmount(dataMapfre.cotizacion.conceptosDesgloseSalud.impBoni, mapfreTariff);  // dataMapfre.cotizacion.conceptosDesgloseSalud.impBoni;
                            item.enterprise[0].impImptosInteres = calculateAmmount(dataMapfre.cotizacion.conceptosDesgloseSalud.impImptosInteres, mapfreTariff);  //dataMapfre.cotizacion.conceptosDesgloseSalud.impImptosInteres;
                            item.cotizacion = dataMapfre.cotizacion.numCotizacion;
                        }
                    }
                }
            }
            else
            {
                dataProtecta.fields = new List<ProductCotVM>();
                var productoSalud = new ProductCotVM();
                productoSalud.id = String.Empty;
                productoSalud.tariff = String.Empty;
                productoSalud.field = "SALUD";
                productoSalud.fieldEquivalenceCore = ELog.obtainConfig("saludKey"); // ELog.obtainConfig("saludKey");
                productoSalud.areaGroup = String.Empty;
                productoSalud.activityGroup = String.Empty;
                productoSalud.enterprise = new List<EnterpriseVM>();
                var itemEnterprise = new EnterpriseVM();
                itemEnterprise.size = String.Empty;
                itemEnterprise.minimumPremium = Convert.ToDouble(ELog.obtainConfig("primaMinimaMapfre"));
                itemEnterprise.minimumPremiumEndoso = 0;

                itemEnterprise.netRate = new List<NetRateVM>();
                foreach (var tasaMP in dataMapfre.tasas)
                {
                    var netRate = new NetRateVM();
                    netRate.id = ELog.obtainConfig("flatKey");
                    netRate.description = ELog.obtainConfig("nomCategoriaMP");
                    netRate.rate = tasaMP.tasa / 100;
                    netRate.premiumMonth = calculateAmmount(dataMapfre.cotizacion.conceptosDesgloseSalud.impPneta, mapfreTariff); // dataMapfre.cotizacion.conceptosDesgloseSalud.impPneta;
                    itemEnterprise.netRate.Add(netRate);
                }

                itemEnterprise.riskRate = new List<RiskPremiumVM>();
                foreach (var tasaMP in dataMapfre.tasas)
                {
                    var riskRate = new RiskPremiumVM();
                    riskRate.id = ELog.obtainConfig("flatKey");
                    riskRate.description = ELog.obtainConfig("nomCategoriaMP");
                    riskRate.rate = tasaMP.tasa / 100;
                    itemEnterprise.riskRate.Add(riskRate);
                }

                itemEnterprise.impPneta = calculateAmmount(dataMapfre.cotizacion.conceptosDesgloseSalud.impPneta, mapfreTariff); // dataMapfre.cotizacion.conceptosDesgloseSalud.impPneta;
                itemEnterprise.impInteres = calculateAmmount(dataMapfre.cotizacion.conceptosDesgloseSalud.impInteres, mapfreTariff); //  dataMapfre.cotizacion.conceptosDesgloseSalud.impInteres;
                itemEnterprise.impRecargos = calculateAmmount(dataMapfre.cotizacion.conceptosDesgloseSalud.impRecargos, mapfreTariff); //  dataMapfre.cotizacion.conceptosDesgloseSalud.impRecargos;
                itemEnterprise.impPnetaBoni = calculateAmmount(dataMapfre.cotizacion.conceptosDesgloseSalud.impPnetaBoni, mapfreTariff); // dataMapfre.cotizacion.conceptosDesgloseSalud.impPnetaBoni;
                itemEnterprise.impPrimaTotal = calculateAmmount(dataMapfre.cotizacion.conceptosDesgloseSalud.impPrimaTotal, mapfreTariff); // dataMapfre.cotizacion.conceptosDesgloseSalud.impPrimaTotal;
                itemEnterprise.impImptos = calculateAmmount(dataMapfre.cotizacion.conceptosDesgloseSalud.impImptos, mapfreTariff); // dataMapfre.cotizacion.conceptosDesgloseSalud.impPrimaTotal;
                itemEnterprise.impBoni = calculateAmmount(dataMapfre.cotizacion.conceptosDesgloseSalud.impBoni, mapfreTariff); // dataMapfre.cotizacion.conceptosDesgloseSalud.impBoni;
                itemEnterprise.impImptosInteres = calculateAmmount(dataMapfre.cotizacion.conceptosDesgloseSalud.impImptosInteres, mapfreTariff); // dataMapfre.cotizacion.conceptosDesgloseSalud.impImptosInteres;
                productoSalud.enterprise.Add(itemEnterprise);

                productoSalud.channels = null;
                productoSalud.channelDistributions = null;
                productoSalud.discount = "0";
                productoSalud.cotizacion = dataMapfre.cotizacion.numCotizacion;
                productoSalud.activityVariation = 0;
                productoSalud.commission = "0";

                dataProtecta.fields.Add(productoSalud);
            }

            return await Task.FromResult(dataProtecta);
        }

        public double calculateAmmount(double ammount, CotizarBM mapfreTariff)
        {
            return ammount / Convert.ToDouble(ELog.obtainConfig("renovacionM" + mapfreTariff.poliza.codFormaDeclaracion));
        }

        [Route("ComisionGet")]
        [HttpGet]
        public List<ComisionVM> ComisionGet(int nroCotizacion)
        {
            return quotationCORE.ComisionGet(nroCotizacion);
        }

        [Route("GlossGet")]
        [HttpGet]
        public List<GlossVM> GlossGet()
        {
            return quotationCORE.GlossGet();
        }

        [Route("GesCliente360")]
        [HttpPost]
        public async Task<ResponseCVM> GesCliente360(ConsultaCBM data, string asegurado = "")
        {
            var response = new ResponseCVM();
            data.P_CodAplicacion = ELog.obtainConfig("codAppCliente");
            response = await quotationCORE.GesCliente360(data, asegurado);

            if (response.EListClient != null && response.EListClient.Count == 1 && data.validaMapfre != null)
            {
                if (!String.IsNullOrEmpty(response.EListClient[0].P_SCLIENT))
                {
                    var mensajeMapfre = String.Empty;
                    mensajeMapfre = response.EListClient[0].EListAddresClient.Count == 0 ? ELog.obtainConfig("msjDireccion") : mensajeMapfre;
                    mensajeMapfre = response.EListClient[0].EListPhoneClient.Count == 0 ? mensajeMapfre + ELog.obtainConfig("msjTelefono") : mensajeMapfre;
                    mensajeMapfre = response.EListClient[0].P_NPERSON_TYP == ELog.obtainConfig("personaJuridica") && response.EListClient[0].EListContactClient.Count == 0 ? mensajeMapfre + ELog.obtainConfig("msjContacto") : mensajeMapfre;
                    //mensajeMapfre = response.EListClient[0].P_NPERSON_TYP == ELog.obtainConfig("personaJuridica") && response.EListClient[0].EListCIIUClient.Count == 0 ? mensajeMapfre + ELog.obtainConfig("msjCiiu") : mensajeMapfre;

                    if (String.IsNullOrEmpty(mensajeMapfre))
                    {
                        var responseMapfre = await mapfreController.GesCotizacionMapfre(data.validaMapfre);
                        var listAprobado = ELog.obtainConfig("aprobadoMapfre").Split(';').ToList();

                        //if (responseMapfre.codError != ELog.obtainConfig("codigo_NO"))
                        //{
                        for (int i = 0; i < listAprobado.Count; i++)
                        {
                            if (responseMapfre.codError == listAprobado[i])
                            {
                                response.P_NCODE = ELog.obtainConfig("codigo_OK");
                                response.P_SMESSAGE = String.Empty;
                                response.P_NPENDIENTE = responseMapfre.codError == ELog.obtainConfig("codigo_OK") ? ELog.obtainConfig("codigo_OK") : ELog.obtainConfig("pendienteActivo");
                                break;
                            }
                            else
                            {
                                response.P_NCODE = ELog.obtainConfig("codigo_NO");
                                response.P_SMESSAGE = responseMapfre.descError;
                            }
                        }
                        //}
                        //else
                        //{
                        //    response.P_NCODE = ELog.obtainConfig("codigo_NO");
                        //    response.P_SMESSAGE = responseMapfre.descError;
                        //}
                    }
                    else
                    {
                        response.P_SMESSAGE = String.Format(ELog.obtainConfig("msjCliente360"), mensajeMapfre.Remove(mensajeMapfre.Length - 2));
                        response.P_NCODE = ELog.obtainConfig("codigo_NO");
                    }
                }
            }
            else
            {
                response.P_NPENDIENTE = ELog.obtainConfig("codigo_OK");
            }
            return response;
        }

        /// <summary>
        /// Servicio para ecommerce
        /// </summary>
        /// <returns></returns>
        [Route("validarTrama")]
        [HttpPost]
        public async Task<SalidaTramaBaseVM> validarTramaVLEcom()
        {
            SalidaTramaBaseVM response = new SalidaTramaBaseVM();
            HttpPostedFile upload = HttpContext.Current.Request.Files["dataFile"];
            string codUser = HttpContext.Current.Request.Params.Get("codUser").ToString();
            string effecDate = HttpContext.Current.Request.Params.Get("effecDate").ToString();
            int comission = Convert.ToInt32(HttpContext.Current.Request.Params.Get("comission").ToString());
            int tiporenov = Convert.ToInt32(HttpContext.Current.Request.Params.Get("tiporenov").ToString());
            int payfreq = Convert.ToInt32(HttpContext.Current.Request.Params.Get("payfreq").ToString());
            var topeLey = Convert.ToInt64(ELog.obtainConfig("planillaMax"));
            var planillaMax = Convert.ToInt64(ELog.obtainConfig("planillaMax"));
            string codProducto = HttpContext.Current.Request.Params.Get("productoId").ToString();
            string flagCot = HttpContext.Current.Request.Params.Get("flagCot").ToString();
            string codActivity = HttpContext.Current.Request.Params.Get("codActivity").ToString();
            string v_proceso = string.Empty;

            try
            {
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

                        v_proceso = (codUser + GenerarCodigo() + DateTime.Now.ToString("yyyyMMddHHmmss")).PadLeft(30, '0');

                        if (result.Tables[0].Rows.Count > 12)
                        {
                            var request = new ValidaTramaEcommerce()
                            {
                                cod_proceso = v_proceso,
                                tip_comision = comission,
                                tip_renovacion = tiporenov,
                                freq_pago = payfreq,
                                tip_transac = "1",
                                cod_usuario = int.Parse(codUser),
                                fec_efecto = effecDate,
                                cod_actividad = codActivity,
                                flag_cotizacion = flagCot,
                                max_planilla = planillaMax,
                                tope_ley = topeLey,
                                cod_producto = codProducto,
                                fec_actual = DateTime.Now
                            };

                            // Se guarda los datos del excel y se valida con el cliente 360
                            response = await procesoEcommerce(result.Tables[0], request);

                            // Se crea objeto para validar la trama
                            var validaTramaBM = generarObjTrama(request);

                            // Se valida la trama y nos trae el detalle
                            //response = validarTramaEcommerce(request, validaTramaBM, response);

                        }
                        else
                        {
                            response.P_COD_ERR = "1";
                            response.P_MESSAGE = "La cantidad mínima de asegurados debe ser de 1.";
                        }
                    }
                    else
                    {
                        var errorItem = new ErroresVM();
                        errorItem.REGISTRO = "0";
                        errorItem.DESCRIPCION = "La cantidad mínima de asegurados debe ser de 1.";
                        response.baseError.errorList.Add(errorItem);
                    }
                    response.NIDPROC = v_proceso;
                }
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = "1";
                response.P_MESSAGE = ex.Message;

                LogControl.save("validarTrama", ex.ToString(), "3", "EC");
            }

            return await Task.FromResult(response);
        }

        public async Task<SalidaTramaBaseVM> procesoEcommerce(DataTable dataTable, ValidaTramaEcommerce request)
        {
            var response = new SalidaTramaBaseVM();
            var tramaList = new List<tramaIngresoBM>();
            //INI MEJORAS VALIDACION VL 
            var objValida = new validaTramaVM();
            //FIN MEJORAS VALIDACION VL 
            try
            {
                Int32 count = 1;
                Int32 rows = 12;

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
                    //var dr = new tramaIngresoBM();
                    DataRow dr = dt.NewRow();
                    dr["NID_COTIZACION"] = null;
                    dr["NID_PROC"] = request.cod_proceso;
                    dr["SFIRSTNAME"] = dataTable.Rows[rows][5].ToString().Trim() == "" ? null : dataTable.Rows[rows][5].ToString().Trim();
                    dr["SLASTNAME"] = dataTable.Rows[rows][3].ToString().Trim() == "" ? null : dataTable.Rows[rows][3].ToString().Trim();
                    dr["SLASTNAME2"] = dataTable.Rows[rows][4].ToString().Trim() == "" ? null : dataTable.Rows[rows][4].ToString().Trim();
                    dr["NMODULEC"] = dataTable.Rows[rows][8].ToString().Trim() == "" ? "" : dataTable.Rows[rows][8].ToString().Trim();
                    dr["NNATIONALITY"] = dataTable.Rows[rows][14].ToString().Trim() == "" ? "" : dataTable.Rows[rows][14].ToString().Trim();
                    dr["NIDDOC_TYPE"] = dataTable.Rows[rows][1].ToString().Trim() == "" ? "" : dataTable.Rows[rows][1].ToString().Trim();
                    //dr["SIDDOC"] = result.Tables[0].Rows[rows][2].ToString().Trim() == "" ? null : dataTable.Rows[rows][2].ToString().Trim();
                    dr["SIDDOC"] = dataTable.Rows[rows][2].ToString().Trim() == "" ? null : dataTable.Rows[rows][1].ToString().Trim().ToUpper() == "DNI" && dataTable.Rows[rows][2].ToString().Trim().Length == 7 ? dataTable.Rows[rows][2].ToString().Trim().PadLeft(8, '0') : dataTable.Rows[rows][2].ToString().Trim();
                    string fecha = IsDate(dataTable.Rows[rows][7].ToString().Trim()) ? Convert.ToDateTime(dataTable.Rows[rows][7].ToString().Trim()).ToShortDateString() : dataTable.Rows[rows][7].ToString().Trim();
                    dr["DBIRTHDAT"] = dataTable.Rows[rows][7].ToString().Trim() == "" ? request.fec_actual.ToShortDateString() : fecha;
                    dr["NREMUNERACION"] = dataTable.Rows[rows][9].ToString().Trim() == "" ? "" : dataTable.Rows[rows][9].ToString().Trim();
                    dr["SE_MAIL"] = dataTable.Rows[rows][10].ToString().Trim() == "" ? null : dataTable.Rows[rows][10].ToString().Trim();
                    dr["SPHONE_TYPE"] = dataTable.Rows[rows][11].ToString().Trim() == "" ? null : "Celular";
                    dr["SPHONE"] = dataTable.Rows[rows][11].ToString().Trim() == "" ? null : dataTable.Rows[rows][11].ToString().Trim();
                    dr["NIDCLIENTLOCATION"] = dataTable.Rows[rows][12].ToString().Trim() == "" ? "" : dataTable.Rows[rows][12].ToString().Trim();
                    dr["NCOD_NETEO"] = dataTable.Rows[rows][13].ToString().Trim() == "" ? "" : dataTable.Rows[rows][13].ToString().Trim().Length > 5 ? dataTable.Rows[rows][13].ToString().Trim().Substring(0, 5) : dataTable.Rows[rows][13].ToString().Trim();
                    dr["NUSERCODE"] = request.cod_usuario.ToString().Trim();
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

                var maxTrama = new QuotationCORE().getMaxTrama("NASEG_MAX_TRAMA", ELog.obtainConfig("vidaLeyBranch"));

                if (maxTrama == -1 || count <= maxTrama)
                {
                    response = quotationCORE.SaveUsingOracleBulkCopy(dt);

                    if (response.P_COD_ERR == "0")
                    {
                        #region Guardado de asegurados - Cliente360
                        //INI MEJORAS VALIDACION VL 
                        objValida.codProducto = request.cod_producto;
                        objValida.codRamo = request.cod_ramo;
                        objValida.codUsuario = request.cod_usuario.ToString();
                        response = request.cod_ramo == ELog.obtainConfig("vidaLeyBranch") ? await consultaAsegurados360(dt, objValida: objValida) : await consultaAsegurados360(dt);
                        //FIN MEJORAS VALIDACION VL 
                        #endregion
                    }
                }
                else
                {
                    response.P_COD_ERR = "1";
                    response.P_MESSAGE = "La cantidad de asegurados enviada superar el máximo configurado -> " + maxTrama;
                }
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = "1";
                response.P_MESSAGE = ex.Message;
                LogControl.save("procesoEcommerce", ex.ToString(), "3", "EC");
            }

            return response;
        }

        private SalidaTramaBaseVM validarTramaEcommerce(ValidaTramaEcommerce request, ValidaTramaBM validaTramaBM, SalidaTramaEcommerce res)
        {
            var response = new SalidaTramaBaseVM() { P_COD_ERR = "0", P_MESSAGE = "Se valido correctamente la trama" };
            var tramaList = new List<tramaIngresoBM>();

            response.baseError = quotationCORE.ValidateTramaVL_ECOM(validaTramaBM);

            if (response.baseError.errorList.Count == 0)
            {
                var dr = new tramaIngresoBM();
                quotationCORE.readInfoTrama(validaTramaBM, ref response);
                quotationCORE.readInfoTramaDetail(validaTramaBM, ref response);

                int cantEmpleados = 0;
                int cantObreros = 0;

                float? sumEmpleados = 0;
                float? sumObreros = 0;

                cantEmpleados = tramaList.Where(x => x.codTipoTrabajador == 1).Count();
                cantObreros = tramaList.Where(x => x.codTipoTrabajador == 2).Count();

                float? totalPlanilla = 0;

                //var numReg = 1;

                if (totalPlanilla > request.max_planilla)
                {
                    totalPlanilla = request.max_planilla;
                }

                sumEmpleados = tramaList.Where(x => x.codTipoTrabajador == 1).Sum(x => x.sueldoBruto) > request.max_planilla ? request.max_planilla : tramaList.Where(x => x.codTipoTrabajador == 1).Sum(x => x.sueldoBruto);
                sumObreros = tramaList.Where(x => x.codTipoTrabajador == 2).Sum(x => x.sueldoBruto) > request.max_planilla ? request.max_planilla : tramaList.Where(x => x.codTipoTrabajador == 2).Sum(x => x.sueldoBruto);

                /*if (response.errorList.Count == 0)
                {*/
                response.P_COD_ERR = "0";
                response.P_MESSAGE = "";

                TasasVM item1 = new TasasVM();
                item1.TIP_RIESGO = "1";
                item1.DES_RIESGO = "Empleados";
                item1.NUM_TRABAJADORES = cantEmpleados.ToString();
                item1.MONTO_PLANILLA = sumEmpleados.ToString();
                item1.ID_PRODUCTO = request.cod_producto;
                response.planillaList.Add(item1);

                TasasVM item2 = new TasasVM();
                item2.TIP_RIESGO = "2";
                item2.DES_RIESGO = "Obreros";
                item2.NUM_TRABAJADORES = cantObreros.ToString();
                item2.MONTO_PLANILLA = sumObreros.ToString();
                item2.ID_PRODUCTO = request.cod_producto;
                response.planillaList.Add(item2);

                TasasVM item3 = new TasasVM();
                item3.TIP_RIESGO = "3";
                item3.DES_RIESGO = "Flat";
                item3.NUM_TRABAJADORES = tramaList.Count.ToString();
                item3.MONTO_PLANILLA = totalPlanilla.ToString();
                item3.ID_PRODUCTO = request.cod_producto;
                response.planillaList.Add(item3);
            }

            return response;
        }

        private ValidaTramaBM generarObjTrama(ValidaTramaEcommerce request)
        {
            var validaTramaBM = new ValidaTramaBM()
            {
                NID_PROC = request.cod_proceso,
                NTYPE_COMMISSION = request.tip_comision,
                NTIP_RENOV = request.tip_renovacion,
                NPAYFREQ = request.freq_pago,
                NTYPE_TRANSAC = "1",
                NUSERCODE = request.cod_usuario,
                DEFFECDATE = request.fec_efecto,
                P_NTECHNICAL = request.cod_actividad,
                SFLAGCOT = request.flag_cotizacion,
                P_NBRANCH = request.cod_ramo,
                P_NPRODUCT = request.cod_producto,
                NFLAG_MINA = request.flag_mina

            };

            return validaTramaBM;
        }

        [Route("validarTramaVL")]
        [HttpPost]
        public async Task<SalidaTramaBaseVM> validarTramaVL()
        {
            var response = new SalidaTramaBaseVM();
            response.errorList = new List<ErroresVM>();
            var objValida = new validaTramaVM();

            try
            {
                HttpPostedFile upload = HttpContext.Current.Request.Files["dataFile"];
                objValida = JsonConvert.DeserializeObject<validaTramaVM>(HttpContext.Current.Request.Params.Get("objValida"));

                objValida.planillaMax = Convert.ToInt64(ELog.obtainConfig("planillaMax"));
                objValida.topeLey = Convert.ToInt64(ELog.obtainConfig("topeLey"));
                var validaTramaBM = generarValidaItem(objValida);
                objValida.tipoCotizacion = String.IsNullOrEmpty(objValida.tipoCotizacion) ? "PRIME" : objValida.tipoCotizacion; //GCAA 20022024
                objValida.codAplicacion = String.IsNullOrEmpty(objValida.codAplicacion) ? "PD" : objValida.codAplicacion;
                objValida.fechaActual = DateTime.Now;

                objValida.codProcesoOld = objValida.flagCot == "1" ? objValida.codProceso : null;

                if (upload != null && upload.ContentLength > 0)
                {
                    if (objValida.codAplicacion == "PD") // Plataforma Digital
                    {
                        if (objValida.codRamo == ELog.obtainConfig("vidaLeyBranch")) // mejora - homologacion - RI
                        {
                            if (objValida.flagPolizaEmision != 1 && objValida.flagCot != "1")
                            {
                                objValida.codProceso = (objValida.codUsuario + GenerarCodigo() + DateTime.Now.ToString("yyyyMMddHHmmss")).PadLeft(30, '0');
                            }
                        }
                        else
                        {
                            if (objValida.flagPolizaEmision != 1)
                            {
                                if (objValida.codRamo != ELog.obtainConfig("vidaIndividualBranch"))
                                {
                                    objValida.codProceso = (objValida.codUsuario + GenerarCodigo() + DateTime.Now.ToString("yyyyMMddHHmmss")).PadLeft(30, '0');
                                }
                                else
                                {
                                    if (objValida.codProceso == null)
                                    {
                                        objValida.codProceso = (objValida.codUsuario + GenerarCodigo() + DateTime.Now.ToString("yyyyMMddHHmmss")).PadLeft(30, '0');
                                    }
                                }
                            }
                        }
                    }

                    validaTramaBM.NID_PROC = objValida.codProceso;
                    Stream stream = upload.InputStream;
                    IExcelDataReader reader = null;

                    if (upload.FileName.EndsWith(".xls"))
                    {
                        reader = ExcelReaderFactory.CreateBinaryReader(stream);
                    }
                    else if (upload.FileName.EndsWith(".xlsx") || upload.FileName.EndsWith(".xlsm"))
                    {
                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    }

                    if (reader != null)
                    {
                        DataSet result = reader.AsDataSet();
                        reader.Close();

                        if (result.Tables[0].Rows.Count > Convert.ToInt32(ELog.obtainConfig("rows" + objValida.codProducto + objValida.codRamo)))
                        {
                            var maxTrama = new QuotationCORE().getMaxTrama("NASEG_MAX_TRAMA", objValida.codRamo);

                            if (objValida.codRamo == ELog.obtainConfig("vidaLeyBranch"))
                            {
                                response = await tramaVidaLey(result.Tables[0], upload, objValida, validaTramaBM, maxTrama);
                            }

                            if (objValida.codRamo == ELog.obtainConfig("accidentesBranch"))
                            {
                                response = await tramaAccPersonales(result.Tables[0], upload, objValida, validaTramaBM, maxTrama);
                            }

                            if (objValida.codRamo == ELog.obtainConfig("vidaGrupoBranch"))
                            { // validar covid
                                response = await tramaAccPersonales(result.Tables[0], upload, objValida, validaTramaBM, maxTrama);
                                // response = await tramaCovid(result.Tables[0], upload, objValida, validaTramaBM);
                            }

                            if (objValida.codRamo == ELog.obtainConfig("vidaIndividualBranch"))
                            {
                                response = quotationCORE.DeleteBenefactors(objValida.codProceso, "ASEGURADO");
                                response = quotationCORE.DeleteBenefactors(objValida.codProceso, "BENEFICIARIO");

                                if (response.P_COD_ERR == "0")
                                {
                                    response = await tramaBeneficiarios(result.Tables[0], upload, objValida, validaTramaBM);
                                    if (response.P_COD_ERR == "0")
                                    {
                                        if (objValida.codProceso != null)
                                        {
                                            response = quotationCORE.InsGenBenefactors(objValida);
                                        }
                                    }
                                }
                            }

                            if (objValida.codRamo == ELog.obtainConfig("desgravamenBranch"))
                            {
                                //ARJG 28112023
                                objValida.sclientdcd = objValida.datosContratante.codContratante;
                                var numregdcd = new QuotationCORE().getMaxReg(objValida.sclientdcd);
                                if (numregdcd == 1)
                                {
                                    objValida.nconfigdcd = 1;
                                }
                                else
                                {
                                    objValida.nconfigdcd = 0;
                                }
                                //---
                                response = await tramaDesgravamen(result.Tables[0], upload, objValida, validaTramaBM, maxTrama);
                            }
                        }
                        else
                        {
                            response.P_COD_ERR = "1";
                            response.P_MESSAGE = "La cantidad mínima de asegurados debe ser de 1.";
                        }
                    }
                }
                else //Recotizacion
                {
                    if (objValida.codProducto == ELog.obtainConfig("vidaLeyKey") && (String.IsNullOrEmpty(objValida.codRamo) ? objValida.nbranch : objValida.codRamo) == ELog.obtainConfig("vidaLeyBranch"))
                    {
                        if (objValida.type_mov == "8")
                        {
                            validaTramaBM.SFLAGCOT = objValida.flagCot;

                            //quotationCORE.UpdCargaTramaEndoso(validaTramaBM); // ENDOSO TECNICA JTV 242022023
                            quotationCORE.readInfoTramaEndoso(validaTramaBM, ref response);

                            //quotationCORE.readInfoTramaDetailEndoso(validaTramaBM, ref response); // ENDOSO TECNICA JTV 22022023
                        }
                        else if (objValida.type_mov == "3")
                        {
                            quotationCORE.getDetail(objValida, ref response);
                            quotationCORE.getDetailAmount(objValida, ref response);
                            response.P_COD_ERR = "0";
                        }
                        else
                        {
                            quotationCORE.readInfoTrama(validaTramaBM, ref response);
                            validaTramaBM.SFLAGCOT = objValida.flagCot;
                            quotationCORE.readInfoTramaDetail(validaTramaBM, ref response);
                            response.P_COD_ERR = "0";
                        }
                    }
                }

                response.NIDPROC = objValida.codProceso;

                if (!new string[] { "0", "3" }.Contains(response.P_COD_ERR)) // AGF 12052023 Beneficiarios VCF
                {
                    // INI AGF 12052023 Beneficiarios VCF
                    if (response.P_COD_ERR == "2" && objValida.codRamo == ELog.obtainConfig("vidaIndividualBranch"))
                    {
                        LogControl.save("validarTramaVL", JsonConvert.SerializeObject(response, Formatting.None), "2");
                        response.P_MESSAGE = "La trama no puede tener asegurados";
                    }
                    // FIN AGF 12052023 Beneficiarios VCF

                    response.P_MESSAGE = response.P_MESSAGE + " Hubo un error al validar los valores de la trama. Favor de comunicarse con sistemas.";
                    LogControl.save("validarTramaVL", JsonConvert.SerializeObject(response, Formatting.None), "2");
                }
                else
                {
                    response.P_COD_ERR = response.P_COD_ERR == "3" ? "1" : "0";
                }
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = "1";
                response.P_MESSAGE = "Hubo un error al validar los valores de la trama. Favor de comunicarse con sistemas";
                LogControl.save("validarTramaVL", ex.ToString(), "3");
            }

            return response;
        }

        // Servicio que procesa la trama adjuntada a la poliza matriz
        [Route("ProcesarTrama")]
        [HttpPost]
        public EmitPolVM ProcesarTrama(ProcesaTramaBM trama)
        {
            var response = new EmitPolVM();

            if (quotationCORE.ValidardataTrama(trama.idproc) > 0)
            {
                response = quotationCORE.transferDataPM(trama.idproc);
                response = response.cod_error == 0 ? quotationCORE.UpdCotizacionDet(trama) : response;
                response = response.cod_error == 0 ? quotationCORE.TRAS_CARGA_ASE(trama) : response;
                response = response.cod_error == 0 ? quotationCORE.INS_JOB_CLOSE_SCTR(trama) : response;
                response = response.cod_error == 0 ? quotationCORE.Update_header(trama.idproc) : response;
                response = response.cod_error == 0 ? quotationCORE.UpdateInsuredPremium_Matriz(trama) : response;



                //if (response.cod_error == 0)
                //{
                //    response = quotationCORE.INS_JOB_CLOSE_SCTR(trama);
                //    quotationCORE.Update_header(trama.idproc);
                //}
                //else
                //{
                //    response.cod_error = 1;
                //    response.message = "Error";
                //}
            }
            else
            {
                response.cod_error = 1;
                response.message = "No se adjunto la trama";
            }


            return response;
        }

        private ValidaTramaBM generarValidaItem(validaTramaVM objValida)
        {
            ValidaTramaBM validaTramaBM = new ValidaTramaBM();
            validaTramaBM.NID_PROC = objValida.codProceso;
            validaTramaBM.NTYPE_COMMISSION = objValida.comision != null ? objValida.comision : 0;
            validaTramaBM.NTARIFA_COMMISSION = objValida.comision_porcentaje; // TASA X COMISION JRIOS
            validaTramaBM.NTIP_RENOV = objValida.tipoRenovacion;
            validaTramaBM.NPAYFREQ = objValida.freqPago;
            validaTramaBM.NTYPE_TRANSAC = objValida.type_mov ?? "1";
            validaTramaBM.NUSERCODE = int.Parse(objValida.codUsuario);
            validaTramaBM.DEFFECDATE = objValida.fechaEfecto;
            validaTramaBM.P_NTECHNICAL = objValida.codActividad; // MRC
            validaTramaBM.P_FLAG_PR = objValida.flagComisionPro; // MRC
            validaTramaBM.P_COMISSION_PR = objValida.comisionPro != null ? Convert.ToDouble(objValida.comisionPro) : 0; // MRC
            validaTramaBM.P_JTASAS = new List<CategoryRateBM>();

            var categoriasFiltradas = objValida.categoryList != null ? objValida.categoryList.Where(item => item.NTOTAL_PLANILLA > 0) : null; // GCAA 10012024

            if (objValida.categoryList != null)
            {
                foreach (var item in categoriasFiltradas)
                {
                    validaTramaBM.P_JTASAS.Add(new CategoryRateBM()
                    {
                        SDESCRIPT = item.SCATEGORIA,
                        P_TASA = item.ProposalRate,
                        NMODULEC = item.ModuleCode,
                        NCOUNT = item.NCOUNT, // GCAA 09012024
                        NTOTAL_PLANILLA = item.NTOTAL_PLANILLA, // GCAA 09012024
                        SRANGO_EDAD = item.SRANGO_EDAD // GCAA 09012024
                    });
                }
                /*
                validaTramaBM.P_TASA_OB = objValida.categoryList.Where(w => w.SCATEGORIA == "OBRERO").Select(s => s.ProposalRate).FirstOrDefault(); // cotizacion  objValida.tasaObreroPro != null ? Convert.ToDouble(objValida.tasaObreroPro) : 0; // MRC
                validaTramaBM.P_TASA_EM = objValida.categoryList.Where(w => w.SCATEGORIA == "EMPLEADO").Select(s => s.ProposalRate).FirstOrDefault(); //cotizacion objValida.tasaEmpleadoPro != null ? Convert.ToDouble(objValida.tasaEmpleadoPro) : 0; // MRC
                validaTramaBM.P_TASA_OAR = objValida.categoryList.Where(w => w.SCATEGORIA == "OBRERO ALTO RIESGO").Select(s => s.ProposalRate).FirstOrDefault();
                */
            }
            validaTramaBM.NID_COTIZACION = Convert.ToInt32(objValida.nroCotizacion); // MRC
            validaTramaBM.P_NPRODUCT = objValida.codProducto;
            validaTramaBM.P_NREM_EXC = Convert.ToInt32(objValida.remExc);
            validaTramaBM.P_DEXPIRDAT = objValida.fechaExpiracion;
            //endoso
            validaTramaBM.P_NCURRENCY = objValida.nCurrency;
            validaTramaBM.TYPE_ENDOSO = objValida.TYPE_ENDOSO;
            validaTramaBM.P_NBRANCH = !string.IsNullOrEmpty(objValida.codRamo) ? objValida.codRamo : objValida.nbranch; // ENDOSO TECNICA JTV 10042023
            validaTramaBM.P_NPOLICY = objValida.nroPoliza;
            return validaTramaBM;
        }

        public async Task<ResponseGraph> getMiniTariffGraph(validaTramaVM request)
        {
            var data = new TariffGraph()
            {
                idTariff = request.datosPoliza.idTariff,
                versionTariff = request.datosPoliza.versionTariff,
                nbranch = request.datosPoliza.branch.ToString(),
                currency = request.datosPoliza.codMon,
                channel = request.codCanal,
                billingType = request.datosPoliza.codTipoFacturacion,
                policyType = request.datosPoliza.codTipoNegocio,
                collocationType = request.datosPoliza.codTipoModalidad,
                profile = request.datosPoliza.codTipoPerfil
            };

            data.profile = new QuotationCORE().getProfileRepeat(data.nbranch, data.profile);

            var response = await new QuotationCORE().getMiniTariffGraph(data, request);

            return response;
        }

        public async Task<ResponseGraphDes> getMiniTariffGraphDes(validaTramaVM request)
        {
            var data = new TariffGraph()
            {
                nbranch = request.datosPoliza.branch.ToString(),
                currency = request.datosPoliza.codMon,
                channel = request.codCanal,
                billingType = request.datosPoliza.codTipoFacturacion.ToString(),
                policyType = request.datosPoliza.codTipoNegocio,
                renewalType = request.datosPoliza.renewalType,
                creditType = request.datosPoliza.creditType
            };

            data.profile = new QuotationCORE().getProfileRepeat(data.nbranch, data.profile);

            var response = await new QuotationCORE().getMiniTariffGraphDes(data);

            return response;
        }

        public async Task<validaTramaVM> insertRules(validaTramaVM request)
        {
            //var response = new SalidaTramaBaseVM();
            try
            {
                var data = new TariffGraph();
                if (new string[] { ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch") }.Contains(request.datosPoliza.branch.ToString()))
                {
                    data = new TariffGraph()
                    {
                        nbranch = request.datosPoliza.branch.ToString(),
                        currency = request.datosPoliza.codMon,
                        channel = request.codCanal,
                        billingType = request.datosPoliza.codTipoFacturacion,
                        policyType = request.datosPoliza.codTipoNegocio,
                        collocationType = request.datosPoliza.codTipoModalidad,
                        profile = request.datosPoliza.codTipoPerfil
                    };

                    data.profile = new QuotationCORE().getProfileRepeat(data.nbranch, data.profile);

                    var miniTariff = await new QuotationCORE().getMiniTariffGraph(data);
                    if (miniTariff.codError == 0)
                    {
                        if (miniTariff.data.miniTariffMatrix != null &&
                            miniTariff.data.miniTariffMatrix.segment != null &&
                            miniTariff.data.miniTariffMatrix.segment.rules != null)
                        {
                            if (miniTariff.data.miniTariffMatrix.segment.rules.Count > 0)
                            {
                                //foreach (var rule in miniTariff.data.miniTariffMatrix.segment.rules)
                                //{

                                var response_error = new SalidaTramaBaseVM();
                                response_error = quotationCORE.insertRules(miniTariff.data.miniTariffMatrix.segment.rules, request);
                                //}
                            }

                            request.codError = 0;
                            request.desError = miniTariff.message;
                        }
                    }
                    else
                    {
                        request.codError = 1;
                        request.desError = miniTariff.message;
                    }

                }
                else if (new string[] { ELog.obtainConfig("vidaIndividualBranch") }.Contains(request.datosPoliza.branch.ToString()))
                {
                    data = new TariffGraph()
                    {
                        nbranch = request.datosPoliza.branch.ToString(),
                        currency = request.datosPoliza.codMon,
                        channel = request.codCanal,
                        billingType = request.datosPoliza.codTipoFacturacion,
                        policyType = request.datosPoliza.codTipoNegocio,
                        renewalType = request.datosPoliza.renewalType,
                        creditType = request.datosPoliza.creditType
                    };

                    var miniTariff = await new QuotationCORE().getMiniTariffGraphDes(data);
                    if (miniTariff.codError == 0)
                    {
                        if (miniTariff.data.miniTariffMatrix != null &&
                            miniTariff.data.miniTariffMatrix.segment != null &&
                            miniTariff.data.miniTariffMatrix.segment.rules != null)
                        {
                            foreach (var rule in miniTariff.data.miniTariffMatrix.segment.rules)
                            {

                                var response_error = new SalidaTramaBaseVM();

                                response_error = quotationCORE.insertRulesDes(rule, request);

                            }

                            //request.range = miniTariff.data.miniTariffMatrix.segment.rules.SingleOrDefault(x => x.id == "10").content.numericInterval.interval;
                            request.codError = 0;
                            request.desError = miniTariff.message;
                        }
                    }
                    else
                    {
                        request.codError = 1;
                        request.desError = miniTariff.message;
                    }

                }

            }
            catch (Exception ex)
            {
                request.codError = 1;
                request.desError = ex.ToString();
                LogControl.save("insertRules", ex.ToString(), "3");
            }

            return await Task.FromResult(request);
        }

        //Servicio para calcular prima cuando se da check en poliza matriz
        //[Route("CalcularPrima")]
        //[HttpPost]
        //public async Task<SalidaTramaBaseVM> CalcularPrima(validaTramaVM request)
        //{
        //    var response = new SalidaTramaBaseVM();
        //    var resAuth = new AuthDerivationVM()
        //    {
        //        desAuth = "Se realizó los cálculos correctamente."
        //    };

        //    try
        //    {
        //        if (request != null)
        //        {
        //            string branch = request.datosPoliza.branch.ToString();
        //            request.codAplicacion = request.codAplicacion ?? "PD";
        //            request.codProceso = request.codAplicacion != "EC" ? (request.codUsuario + GenerarCodigo() + DateTime.Now.ToString("yyyyMMddHHmmss")).PadLeft(30, '0') : request.codProceso;

        //            if (new string[] { ELog.obtainConfig("desgravamenBranch") }.Contains(request.datosPoliza.branch.ToString()))
        //            {
        //                request.codError = 0;
        //            }
        //            else
        //            {
        //                request = await insertRules(request);
        //            }


        //            if (new string[] { ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch") }.Contains(request.datosPoliza.branch.ToString()))
        //            {
        //                request = request.codError == 0 ? quotationCORE.valMinMaxAseguradosPolMatriz(request) : request;
        //            }

        //            if (request.codError == 0)
        //            {
        //                //response.validacion_asegurados = request.CantidadTrabajadores < request.range.min || request.CantidadTrabajadores > request.range.max ? 1 : 0;
        //                double comisionBroker = 0;
        //                double comisionInter = 0;

        //                // Validacion de tipo de documento 
        //                var listError = validarDocumentoPerfil(request.datosPoliza.tipoDocumento, request.datosPoliza.numDocumento, request.datosPoliza.codTipoNegocio, request.datosPoliza.codTipoProducto);

        //                // validacion de asegurados
        //                //if ((response.validacion_asegurados == 0 && listError.Count == 0) || request.codAplicacion == "EC")
        //                if (listError.Count == 0 || request.codAplicacion == "EC")
        //                {
        //                    request.flag = "1";
        //                    request.flagpremium = 1;

        //                    if (new string[] { ELog.obtainConfig("desgravamenBranch") }.Contains(request.datosPoliza.branch.ToString()))
        //                    {
        //                        //datos para cobertura (esperar detalle de William para coberturas) desgravamen devolucion
        //                        request.lcoberturas = insertCoverRequeridDesDev(request);
        //                    }
        //                    else
        //                    {
        //                        if (request.codAplicacion != "EC")
        //                        {
        //                            request.lcoberturas = await insertCoverRequerid(request);
        //                        }

        //                        if (new string[] { ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch") }.Contains(request.datosPoliza.branch.ToString()))
        //                        {
        //                            if (!quotationCORE.validarAforo(request.datosPoliza.branch.ToString(), request.datosPoliza.codTipoPerfil, request.datosPoliza.trxCode))
        //                            {
        //                                request.ruta = generarAseguradosJson(request.codProceso);
        //                            }
        //                        }
        //                    }

        //                    if (new string[] { ELog.obtainConfig("vidaIndividualBranch") }.Contains(request.datosPoliza.branch.ToString()))
        //                    {
        //                        request.lasistencias = await insertAssitsRequirid(request);
        //                        request.lbeneficios = await insertBenefitRequirid(request);
        //                    }


        //                    // Acá se ha validado todo correctamente - Llamamos a Tarifario AP
        //                    //var responseTarifario = new ResponseGraph() { codError = 0 };

        //                    var responseTarifario = new ResponseGraph();

        //                    if (new string[] { ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch") }.Contains(request.datosPoliza.branch.ToString()))
        //                    {
        //                        responseTarifario = await new QuotationCORE().SendDataTarifarioGraphql(request);

        //                    }
        //                    else if (new string[] { ELog.obtainConfig("vidaIndividualBranch") }.Contains(request.datosPoliza.branch.ToString()))
        //                    {

        //                        //AQUI LLAMAR A TARIFARIO DE NUEVO CON DPS 
        //                        //CAMBIO V2


        //                        List<Entities.QuotationModel.BindingModel.DatosDPS> datosDPS = new List<Entities.QuotationModel.BindingModel.DatosDPS>();

        //                        var valDPS = new Entities.QuotationModel.BindingModel.DatosDPS()
        //                        {
        //                            question = "1",
        //                            type = "NUMBER",
        //                            value = "75"
        //                        };

        //                        datosDPS.Add(valDPS);

        //                        var valDPS2 = new Entities.QuotationModel.BindingModel.DatosDPS()
        //                        {
        //                            question = "2",
        //                            type = "NUMBER",
        //                            value = "1.75"
        //                        };

        //                        datosDPS.Add(valDPS2);

        //                        request.datosDPS = datosDPS;

        //                        responseTarifario = await new QuotationCORE().SendDataTarifarioGraphql(request);

        //                        if (responseTarifario.codError == 0)
        //                        {
        //                            if (responseTarifario.data.searchPremium.commercialPremium == 0)
        //                            {
        //                                responseTarifario.codError = 1;
        //                                responseTarifario.message = "NO VALIDO PARA ASEGURAR";
        //                            }
        //                            else
        //                            {
        //                                responseTarifario.data.searchPremium.commercialPremium = 0;
        //                            }
        //                        }


        //                        //responseTarifario.codError = 0;
        //                        //AQUI LLAMAR A TARIFARIO DE NUEVO CON DPS 
        //                        //CAMBIO V2
        //                    }
        //                    else
        //                    {
        //                        //Calculo de prima para desgravamen devolucion (Consultar si es generico)
        //                        responseTarifario.codError = 0;
        //                    }

        //                    if (responseTarifario.codError == 0)
        //                    {
        //                        request.premium = responseTarifario.data != null ? responseTarifario.data.searchPremium.commercialPremium : 0.0;

        //                        #region Simular prima
        //                        //if (request.premium == 0)
        //                        //{
        //                        //    var array = new int[] { 0, 12, 6, 3, 2, 1, 1 };
        //                        //    request.premium = (100 + ((13 * request.lcoberturas.Count) * 0.75314) + (request.lasistencias.Count * 10) + (request.lbeneficios.Count * 10)) * array[Convert.ToInt32(request.datosPoliza.codTipoFrecuenciaPago)];
        //                        //}
        //                        #endregion

        //                        if (request.premium > 0 || branch == ELog.obtainConfig("vidaIndividualBranch"))
        //                        {
        //                            var response_error = new SalidaErrorBaseVM() { P_COD_ERR = 0 };

        //                            if (new string[] { ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch") }.Contains(request.datosPoliza.branch.ToString()))
        //                            {
        //                                // si tipo de perfil es igual AFORO entra
        //                                if (quotationCORE.validarAforo(request.datosPoliza.branch.ToString(), request.datosPoliza.codTipoPerfil, request.datosPoliza.trxCode))
        //                                {
        //                                    response_error = quotationCORE.InsCargaMasEspecial(request);
        //                                }
        //                            }
        //                            else if (branch == ELog.obtainConfig("vidaIndividualBranch"))
        //                            {
        //                                response_error = quotationCORE.InsCargaMasEspecial(request); // PREGUNTAR POR ESTA TABLA
        //                            }

        //                            // metodo para insertar en cabecera y detalle de cotizacion y realizar calculos
        //                            if (response_error.P_COD_ERR == 0)
        //                            {
        //                                response = await genericProcessAP(request, response, 2);
        //                                comisionBroker = response.P_COD_ERR == "0" ? response.COMISION_BROKER : 0;
        //                                comisionInter = response.P_COD_ERR == "0" ? response.COMISION_INTER : 0;

        //                                ////response = InsertReajustes(request, responseTarifario.data.searchPremium.readjustmentFactors);
        //                                //response = response.P_COD_ERR == "0" ? await InsertRecargos(request) : response;
        //                                //comisionBroker = response.P_COD_ERR == "0" ? response.COMISION_BROKER : 0;
        //                                //response = response.P_COD_ERR == "0" ? await InsertExclusiones(request) : response;
        //                                //response = response.P_COD_ERR == "0" ? quotationCORE.InsReCotizaTrama(request) : response;

        //                                if (response.P_COD_ERR == "0")
        //                                {
        //                                    // desgravamen
        //                                    if (branch == ELog.obtainConfig("vidaIndividualBranch"))
        //                                    {
        //                                        //response = quotationCORE.InsGenBenefactors(request);
        //                                    }
        //                                    // metodo para obtener las listas de primas que se pintaran en la pantalla de cotizacion
        //                                    response = quotationCORE.ReaListCotiza(request.codProceso, request.datosPoliza.codTipoRenovacion, request.datosPoliza.codTipoProducto, request.datosPoliza.branch.ToString());
        //                                    response.TOT_ASEGURADOS = Convert.ToInt32(request.CantidadTrabajadores);
        //                                    response.IND_PRIMA_MINIMA = response_error.flag;
        //                                    response.COMISION_BROKER = comisionBroker;
        //                                    response.COMISION_INTER = comisionInter;
        //                                    response.P_MESSAGE = resAuth.desAuth;
        //                                }
        //                                else
        //                                {
        //                                    response.P_COD_ERR = "1";
        //                                    response.P_MESSAGE = "Hubo un inconveniente al momento de realizar los cálculos.";
        //                                }
        //                            }
        //                            else
        //                            {
        //                                response.P_COD_ERR = "1";
        //                                response.P_MESSAGE = "Hubo un inconveniente al simular asegurado.";
        //                            }
        //                        }
        //                        else
        //                        {
        //                            if (new string[] { ELog.obtainConfig("desgravamenBranch") }.Contains(request.datosPoliza.branch.ToString()))
        //                            {
        //                                resAuth.codAuth = 1;
        //                            }
        //                            else
        //                            {
        //                                resAuth = quotationCORE.getAuthDerivation(request); // Validación de datos de asegurados.
        //                            }

        //                            if (resAuth.codAuth == 1)
        //                            {
        //                                response = await genericProcessAP(request, response, 2);
        //                                if (new string[] { ELog.obtainConfig("desgravamenBranch") }.Contains(request.datosPoliza.branch.ToString()))
        //                                {
        //                                    response = quotationCORE.ReaListCotiza(request.codProceso, request.datosPoliza.codTipoRenovacion, request.datosPoliza.codTipoProducto, request.datosPoliza.branch.ToString());      //NO MUESTRA PARA POLIZA MATRIZ - DESGRAVAMEN DEVOLUCION
        //                                    resAuth.codAuth = 1;
        //                                }
        //                                else
        //                                {
        //                                    resAuth = quotationCORE.getAuthDerivation(request); // Validación de datos de asegurados.
        //                                }
        //                                comisionBroker = response.P_COD_ERR == "0" ? response.COMISION_BROKER : 0;
        //                                comisionInter = response.P_COD_ERR == "0" ? response.COMISION_INTER : 0;
        //                            }
        //                            else
        //                            {
        //                                response.P_COD_ERR = "1";
        //                                response.P_MESSAGE = resAuth.desAuth;
        //                                ELog.save(this, resAuth.desAuth);
        //                            }
        //                            //response.P_COD_ERR = "1";
        //                            //response.P_MESSAGE = "No hay primas configuradas para los datos enviados";
        //                        }
        //                    }
        //                    else
        //                    {
        //                        response.P_COD_ERR = responseTarifario.codError.ToString();
        //                        response.P_MESSAGE = responseTarifario.message;
        //                        ELog.save(this, responseTarifario.message);
        //                    }

        //                    response.NIDPROC = request.codProceso;
        //                }
        //                else
        //                {
        //                    response.P_COD_ERR = "1";

        //                    // Validación de asegurados (mínima)
        //                    //if (response.validacion_asegurados != 0)
        //                    //{
        //                    //    response.P_MESSAGE += "La cantidad de asegurados ingresada es menor / mayor a la cantidad configurada. Cantidad mínima: " + request.range.min + " / Cantidad máxima: " + request.range.max + ". <br />";
        //                    //}

        //                    // Validacion de tipo documento (según perfil)
        //                    if (listError.Count > 0)
        //                    {
        //                        foreach (var item in listError)
        //                        {
        //                            response.P_MESSAGE += item.DESCRIPCION + "<br />";
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                response.P_COD_ERR = "1";
        //                response.P_MESSAGE = request.desError;
        //                ELog.save(this, request.desError);
        //            }

        //        }
        //        else
        //        {
        //            response.P_COD_ERR = "1";
        //            response.P_MESSAGE = "No se ha procesado la solicitud por falta de datos";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.P_COD_ERR = "1";
        //        //response.P_MESSAGE = ex.Message;
        //        response.P_MESSAGE = "Error al calcular la prima.";
        //        ELog.save(this, ex.ToString());
        //    }
        //    return await Task.FromResult(response);
        //}

        //Servicio para calcular prima cuando se da check en poliza matriz
        [Route("CalcularPrima")]
        [HttpPost]
        public async Task<SalidaTramaBaseVM> CalcularPrima(validaTramaVM request)
        {
            var response = new SalidaTramaBaseVM();

            try
            {
                if (request != null)
                {
                    request.codAplicacion = request.codAplicacion ?? "PD";

                    if ((request.codRamo != ELog.obtainConfig("vidaIndividualBranch")) || (request.codRamo == ELog.obtainConfig("vidaIndividualBranch") && request.flagSubirTrama == false))
                    {  //AGF 25052023 Val cod proceso para VCF

                        request.codProceso = request.codAplicacion != "EC" ? (request.codUsuario + GenerarCodigo() + DateTime.Now.ToString("yyyyMMddHHmmss")).PadLeft(30, '0') : request.codProceso;
                    }

                    if (new string[] { ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch") }.Contains(request.datosPoliza.branch.ToString()))
                    {
                        response = await CalcularPrimaApVg(request);
                    }

                    else if (new string[] { ELog.obtainConfig("vidaIndividualBranch") }.Contains(request.datosPoliza.branch.ToString()))
                    {
                        response = quotationCORE.DeleteBenefactors(request.codProceso, "ASEGURADO"); //AGF 24052023 guardar beneficiarios
                        response = await CalcularPrimaVC(request);
                    }

                    else if (new string[] { ELog.obtainConfig("desgravamenBranch") }.Contains(request.datosPoliza.branch.ToString()))
                    {
                        response = await CalcularPrimaVD(request);
                    }
                }
                else
                {
                    response.P_COD_ERR = "1";
                    response.P_MESSAGE = "No se ha procesado la solicitud por falta de datos";
                }
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = "1";
                response.P_MESSAGE = "Error al calcular la prima.";
                LogControl.save("CalcularPrima", ex.ToString(), "3");
            }

            return await Task.FromResult(response);
        }

        private async Task<SalidaTramaBaseVM> CalcularPrimaVD(validaTramaVM request)
        {

            var response = new SalidaTramaBaseVM();
            var resAuth = new AuthDerivationVM()
            {
                desAuth = "Se realizó los cálculos correctamente."
            };

            //response.validacion_asegurados = request.CantidadTrabajadores < request.range.min || request.CantidadTrabajadores > request.range.max ? 1 : 0;
            double comisionBroker = 0;
            double comisionInter = 0;

            // Validacion de tipo de documento 
            var listError = validarDocumentoPerfil(request.datosPoliza.tipoDocumento, request.datosPoliza.numDocumento, request.datosPoliza.codTipoNegocio, request.datosPoliza.codTipoProducto);

            // validacion de asegurados
            //if ((response.validacion_asegurados == 0 && listError.Count == 0) || request.codAplicacion == "EC")
            if (listError.Count == 0 || request.codAplicacion == "EC")
            {
                request.flag = "1";
                request.flagpremium = 1;

                //datos para cobertura (esperar detalle de William para coberturas) desgravamen devolucion
                request.lcoberturas = insertCoverRequeridDesDev(request);

                // Acá se ha validado todo correctamente - Llamamos a Tarifario AP
                request.premium = 0.0;

                #region Simular prima
                //if (request.premium == 0)
                //{
                //    var array = new int[] { 0, 12, 6, 3, 2, 1, 1 };
                //    request.premium = (100 + ((13 * request.lcoberturas.Count) * 0.75314) + (request.lasistencias.Count * 10) + (request.lbeneficios.Count * 10)) * array[Convert.ToInt32(request.datosPoliza.codTipoFrecuenciaPago)];
                //}
                #endregion

                resAuth.codAuth = 1;
                response = await genericProcessPD(request, response, 2);
                response = quotationCORE.ReaListCotiza(request.codProceso, request.datosPoliza.codTipoRenovacion, request.datosPoliza.codTipoProducto, request.datosPoliza.branch.ToString());      //NO MUESTRA PARA POLIZA MATRIZ - DESGRAVAMEN DEVOLUCION

                comisionBroker = response.P_COD_ERR == "0" ? response.COMISION_BROKER : 0;
                comisionInter = response.P_COD_ERR == "0" ? response.COMISION_INTER : 0;

                response.NIDPROC = request.codProceso;
            }
            else
            {
                response.P_COD_ERR = "1";

                // Validacion de tipo documento (según perfil)
                if (listError.Count > 0)
                {
                    foreach (var item in listError)
                    {
                        response.P_MESSAGE += item.DESCRIPCION + "<br />";
                    }
                }
            }

            return response;
        }

        private async Task<SalidaTramaBaseVM> CalcularPrimaVC(validaTramaVM request)
        {
            var response = new SalidaTramaBaseVM();
            var resAuth = new AuthDerivationVM()
            {
                desAuth = "Se realizó los cálculos correctamente."
            };

            var miniTariff = await getMiniTariffGraphDes(request);

            if (miniTariff.codError == 0)
            {
                if (miniTariff.data.miniTariffMatrix != null &&
                    miniTariff.data.miniTariffMatrix.segment != null &&
                    miniTariff.data.miniTariffMatrix.segment.modules != null)
                {

                    //AGF 12042023 - enviando prima por regla modificada
                    // (Mensual = 12, Bimestral = 6, Trimestral = 4, Semestral = 2 Anual = 1 )
                    var months_freq = 0;

                    if (request.datosPoliza.codTipoFrecuenciaPago == "5")
                    {
                        months_freq = 12;

                    }
                    else if (request.datosPoliza.codTipoFrecuenciaPago == "4")
                    {
                        months_freq = 6;
                    }
                    else if (request.datosPoliza.codTipoFrecuenciaPago == "3")
                    {
                        months_freq = 4;
                    }
                    else if (request.datosPoliza.codTipoFrecuenciaPago == "2")
                    {
                        months_freq = 2;
                    }
                    else if (request.datosPoliza.codTipoFrecuenciaPago == "1")
                    {
                        months_freq = 1;
                    }

                    if (miniTariff.data.miniTariffMatrix.segment.modules.Count() > 0)
                    {

                        foreach (var rule in miniTariff.data.miniTariffMatrix.segment.rules)
                        {

                            if (rule.content.conditionalValue?.value != null)
                            {

                                var cal_svalue = Decimal.Parse(rule.content.conditionalValue.value) / months_freq;
                                var parse_cal_svalue = cal_svalue.ToString("0.##");
                                rule.content.conditionalValue.value = parse_cal_svalue;

                            }

                        }
                        response = quotationCORE.insertRulesDes(miniTariff.data.miniTariffMatrix.segment.rules, request);

                        if (response.P_COD_ERR == "0")
                        {
                            //response.validacion_asegurados = request.CantidadTrabajadores < request.range.min || request.CantidadTrabajadores > request.range.max ? 1 : 0;
                            double comisionBroker = 0;
                            double comisionInter = 0;

                            // Validacion de tipo de documento 
                            var listError = validarDocumentoPerfil(request.datosPoliza.tipoDocumento, request.datosPoliza.numDocumento, request.datosPoliza.codTipoNegocio, request.datosPoliza.codTipoProducto);

                            // validacion de asegurados
                            //if ((response.validacion_asegurados == 0 && listError.Count == 0) || request.codAplicacion == "EC")
                            if (listError.Count == 0 || request.codAplicacion == "EC")
                            {
                                request.flag = "1";
                                request.flagpremium = 1;


                                if (request.codAplicacion != "EC")
                                {
                                    request.lcoberturas = await insertCoverRequerid(request);
                                }

                                request.lasistencias = await insertAssitsRequirid(request);
                                request.lbeneficios = await insertBenefitRequirid(request);

                                // Acá se ha validado todo correctamente - Llamamos a Tarifario AP
                                //var responseTarifario = new ResponseGraph() { codError = 0 };

                                //AQUI LLAMAR A TARIFARIO DE NUEVO CON DPS 
                                //CAMBIO V2
                                List<Entities.QuotationModel.BindingModel.DatosDPS> datosDPS = new List<Entities.QuotationModel.BindingModel.DatosDPS>();

                                var valDPS = new Entities.QuotationModel.BindingModel.DatosDPS()
                                {
                                    question = "1",
                                    type = "NUMBER",
                                    value = "75"
                                };

                                datosDPS.Add(valDPS);

                                var valDPS2 = new Entities.QuotationModel.BindingModel.DatosDPS()
                                {
                                    question = "2",
                                    type = "NUMBER",
                                    value = "1.75"
                                };

                                datosDPS.Add(valDPS2);

                                request.datosDPS = datosDPS;

                                var responseTarifario = await new QuotationCORE().SendDataTarifarioGraphql(request);

                                if (responseTarifario.codError == 0)
                                {
                                    if (responseTarifario.data.searchPremium.commercialPremium == 0)
                                    {
                                        responseTarifario.codError = 1;
                                        responseTarifario.message = "NO VALIDO PARA ASEGURAR";
                                    }
                                    else
                                    {
                                        responseTarifario.data.searchPremium.commercialPremium = 0;
                                    }
                                }

                                if (responseTarifario.codError == 0)
                                {
                                    request.premium = responseTarifario.data != null ? responseTarifario.data.searchPremium.commercialPremium : 0.0;

                                    #region Simular prima
                                    //if (request.premium == 0)
                                    //{
                                    //    var array = new int[] { 0, 12, 6, 3, 2, 1, 1 };
                                    //    request.premium = (100 + ((13 * request.lcoberturas.Count) * 0.75314) + (request.lasistencias.Count * 10) + (request.lbeneficios.Count * 10)) * array[Convert.ToInt32(request.datosPoliza.codTipoFrecuenciaPago)];
                                    //}
                                    #endregion

                                    //var response_error = new SalidaErrorBaseVM() { P_COD_ERR = 0 };
                                    var response_error = quotationCORE.InsCargaMasEspecial(request); // PREGUNTAR POR ESTA TABLA

                                    // metodo para insertar en cabecera y detalle de cotizacion y realizar calculos
                                    if (response_error.P_COD_ERR == 0)
                                    {
                                        response = await genericProcessPD(request, response, 2);
                                        comisionBroker = response.P_COD_ERR == "0" ? response.COMISION_BROKER : 0;
                                        comisionInter = response.P_COD_ERR == "0" ? response.COMISION_INTER : 0;

                                        if (response.P_COD_ERR == "0")
                                        {
                                            // desgravamen
                                            //response = quotationCORE.InsGenBenefactors(request);

                                            // metodo para obtener las listas de primas que se pintaran en la pantalla de cotizacion
                                            response = quotationCORE.ReaListCotiza(request.codProceso, request.datosPoliza.codTipoRenovacion, request.datosPoliza.codTipoProducto, request.datosPoliza.branch.ToString());
                                            response.TOT_ASEGURADOS = Convert.ToInt32(request.CantidadTrabajadores);
                                            response.IND_PRIMA_MINIMA = response_error.flag;
                                            response.COMISION_BROKER = comisionBroker;
                                            response.COMISION_INTER = comisionInter;
                                            response.P_MESSAGE = resAuth.desAuth;
                                        }
                                        else
                                        {
                                            response.P_COD_ERR = "1";
                                            response.P_MESSAGE = "Hubo un inconveniente al momento de realizar los cálculos.";
                                        }
                                    }
                                    else
                                    {
                                        response.P_COD_ERR = "1";
                                        response.P_MESSAGE = "Hubo un inconveniente al simular asegurado.";
                                    }
                                }
                                else
                                {
                                    response.P_COD_ERR = responseTarifario.codError.ToString();
                                    response.P_MESSAGE = responseTarifario.message;
                                    LogControl.save("CalcularPrimaVC", JsonConvert.SerializeObject(response, Formatting.None), "2");
                                }

                                response.NIDPROC = request.codProceso;
                            }
                            else
                            {
                                response.P_COD_ERR = "1";

                                // Validacion de tipo documento (según perfil)
                                if (listError.Count > 0)
                                {
                                    foreach (var item in listError)
                                    {
                                        response.P_MESSAGE += item.DESCRIPCION + "<br />";
                                    }
                                }
                            }
                        }
                        else
                        {
                            response.P_COD_ERR = "1";
                            response.P_MESSAGE = request.desError;
                            LogControl.save("CalcularPrimaVC", JsonConvert.SerializeObject(response, Formatting.None), "2");
                        }
                    }
                }
            }

            return response;
        }

        private async Task<SalidaTramaBaseVM> CalcularPrimaApVg(validaTramaVM request)
        {
            var response = new SalidaTramaBaseVM() { P_COD_ERR = "0" };
            var resAuth = new AuthDerivationVM()
            {
                desAuth = "Se realizó los cálculos correctamente."
            };

            LogControl.save(request.codProceso, "CalcularPrimaApVg Ini: " + JsonConvert.SerializeObject(request, Formatting.None), "2", request.codAplicacion);

            if (validarTrx(request))
            {
                var miniTariff = await getMiniTariffGraph(request);

                if (miniTariff.codError == 0)
                {
                    if (miniTariff.data.miniTariffMatrix != null &&
                        miniTariff.data.miniTariffMatrix.segment != null &&
                        miniTariff.data.miniTariffMatrix.segment.modules != null)
                    {
                        if (miniTariff.data.miniTariffMatrix.segment.modules.Count() > 0)
                        {
                            insertRulesTrama(miniTariff.data.miniTariffMatrix.segment.rules, request);

                            request = request.codError == 0 && request.datosPoliza.trxCode == "EM" ? quotationCORE.valMinMaxAseguradosPolMatriz(request) : request;

                            if (request.codError == 0)
                            {
                                // Validacion de tipo de documento 
                                var listError = processValidateInsured(request, null, ref response);

                                // validacion de asegurados
                                if (listError.Count == 0 || request.codAplicacion == "EC")
                                {
                                    request.flag = "1";
                                    //request.flagpremium = 1;

                                    if (request.codAplicacion != "EC")
                                    {
                                        insertCoverTrama(miniTariff.data.miniTariffMatrix.segment.modules, request.datosPoliza.codTipoPlan, ref request);
                                    }

                                    var aseguradoList = new QuotationCORE().GetTramaAsegurado(request.codProceso);

                                    LogControl.save(request.codProceso, "CalcularPrimaApVg: " + JsonConvert.SerializeObject(aseguradoList, Formatting.None), "2", request.codAplicacion);

                                    getPremiumTariff(request.CantidadTrabajadores, aseguradoList, ref request, ref response);

                                    response = response.P_COD_ERR == "0" ? processPremiumPD(request, miniTariff, "calcular", ref response, ref resAuth) : response;

                                    getInfoListPremium(request, resAuth, ref response);

                                    response.cotizadorDetalleList = new QuotationCORE().detailQuotationInfo(null, request);
                                    response.P_COD_ERR = response.P_COD_ERR == "0" && (new string[] { "IN", "DE", "RE" }).Contains(request.datosPoliza.trxCode) ? resAuth.codAuth == 1 ? "2" : response.P_COD_ERR : response.P_COD_ERR;
                                    response.P_QUESTION = response.P_COD_ERR == "2" ? "La transacción va a ser derivada al área técnica por límite de edad, ¿Desea continuar?" : null;
                                    response.NIDPROC = request.codProceso;
                                }
                                else
                                {
                                    response.P_COD_ERR = "1";
                                    // Validacion de tipo documento (según perfil)
                                    if (listError.Count > 0)
                                    {
                                        foreach (var item in listError)
                                        {
                                            response.P_MESSAGE += item.DESCRIPCION + "<br />";
                                        }
                                    }
                                }
                            }
                            else
                            {
                                response.P_COD_ERR = "1";
                                response.P_MESSAGE = request.desError;
                                LogControl.save("CalcularPrimaApVg", JsonConvert.SerializeObject(response, Formatting.None), "2");
                            }
                        }
                    }
                }
            }
            else
            {
                // Acá deberia haber un sp que traiga las coberturas / beneficios / asistencias
                // Para llamar al tarifario con esa data
                // Si es poliza matriz la data guardada
                // Si es alguna transac deberia  se la data de la ultima transac
                getInfoQuotationProcess(ref request);
                //objValida = new QuotationCORE().getInfoQuotationTransac(objValida);

                getPremiumTariff(request.CantidadTrabajadores, null, ref request, ref response);

                response = response.P_COD_ERR == "0" ? processPremiumPD(request, null, "calcular", ref response, ref resAuth) : response;

                getInfoListPremium(request, resAuth, ref response);

                response.cotizadorDetalleList = new QuotationCORE().detailQuotationInfo(null, request);
                response.P_COD_ERR = response.P_COD_ERR == "0" && (new string[] { "IN", "DE", "RE" }).Contains(request.datosPoliza.trxCode) ? resAuth.codAuth == 1 ? "2" : response.P_COD_ERR : response.P_COD_ERR;
                response.P_QUESTION = response.P_COD_ERR == "2" ? "La transacción va a ser derivada al área técnica por límite de edad, ¿Desea continuar?" : null;
                response.NIDPROC = request.codProceso;
            }

            LogControl.save(request.codProceso, "CalcularPrimaApVg Fin: " + JsonConvert.SerializeObject(response, Formatting.None), "2", request.codAplicacion);

            return response;
        }

        [Route("GuardarCover")]
        [HttpPost]
        public async Task<SalidaTramaBaseVM> GuardarCover()
        {
            var response = new SalidaTramaBaseVM() { P_COD_ERR = "0" };

            var request = JsonConvert.DeserializeObject<validaTramaVM>(HttpContext.Current.Request.Params.Get("dataCalcular"));

            try
            {
                var response_error = new SalidaErrorBaseVM();

                response_error = quotationCORE.regDetallePlanCot(request);

                if (response_error.P_COD_ERR == 0)
                {
                    response.P_COD_ERR = "0";
                    //response.P_MESSAGE = ex.Message;
                    response.P_MESSAGE = "Exito";
                    response.NIDPROC = request.codProceso;
                }
                else
                {
                    response.P_COD_ERR = "1";
                    //response.P_MESSAGE = ex.Message;
                    response.P_MESSAGE = "Hubo un error al guardar coberturas. Favor de comunicarse con sistemas";
                    response.NIDPROC = request.codProceso;
                }
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = "1";
                response.P_MESSAGE = "Hubo un error al guardar coberturas. Favor de comunicarse con sistemas";
                response.NIDPROC = request.codProceso;
                LogControl.save("GuardarCover", ex.ToString(), "3");
            }

            return response;
        }


        //Servicio para recalcular prima cuando se da en procesar dentro de coberturas - no poliza matriz
        //[Route("ReCalcularPrima")]
        //[HttpPost]
        //public async Task<SalidaTramaBaseVM> ReCalcularPrima()
        //{
        //    var response = new SalidaTramaBaseVM() { P_COD_ERR = "0" };
        //    var resAuth = new AuthDerivationVM()
        //    {
        //        codAuth = 3,
        //        desAuth = "Se realizó el recalculo correctamente."
        //    };
        //    //var upload = HttpContext.Current.Request.Files["fileASeg"];
        //    var request = JsonConvert.DeserializeObject<validaTramaVM>(HttpContext.Current.Request.Params.Get("dataCalcular"));
        //    //double comisionBroker = 0;
        //    //long cantTrabajadores = 0;

        //    try
        //    {
        //        if (request != null)
        //        {
        //            //request.type_mov = "1";
        //            string branch = request.datosPoliza.branch.ToString();
        //            request.codAplicacion = request.codAplicacion ?? "PD";
        //            request.flagpremium = request.flagCalcular == 1 ? 2 : 0;

        //            var response_error = new SalidaErrorBaseVM();

        //            if (request.codAplicacion != "EC" && request.lcoberturas.Count() == 0 && request.lasistencias.Count() == 0 && new string[] { ELog.obtainConfig("vidaIndividualBranch") }.Contains(branch))
        //            {
        //                request.lcoberturas = await insertCoverRequeridRecal(request);
        //                request.lasistencias = await insertAssitsRequiridRecal(request);
        //                request.lbeneficios = await insertBenefitRequiridRecal(request);
        //                //request.lrecargos = await insertSurchargeRequiridRecal(request);
        //            }
        //            else if (request.lcoberturas.Count() > 0)
        //            {
        //                response_error = quotationCORE.regDetallePlanCot(request); // Guarda Coberturas, Asistencias y Beneficios
        //            }
        //            else
        //            {
        //                response_error.P_COD_ERR = 0;
        //            }

        //            response_error = response_error.P_COD_ERR == 0 ? quotationCORE.valSumasASeguradas(request) : response_error;
        //            if (response_error.P_COD_ERR == 0)
        //            {
        //                // Verifica que haya trama de aegurados
        //                //if (upload != null && upload.ContentLength > 0)
        //                //{
        //                //    request.ruta = String.Format(ELog.obtainConfig("pathPrincipal"), DateTime.Now.ToString("yyyyMMddHHmmss").PadLeft(14, 'C')) + "\\";

        //                //    if (!Directory.Exists(request.ruta))
        //                //    {
        //                //        Directory.CreateDirectory(request.ruta);
        //                //    }
        //                //    upload.SaveAs(request.ruta + upload.FileName);
        //                //}

        //                // Acá se ha validado todo correctamente - Llamamos a Tarifario AP
        //                //var responseTarifario = new ResponseGraph() { codError = 0 };
        //                //cantTrabajadores = request.CantidadTrabajadores;
        //                //request.CantidadTrabajadores = request.datosPoliza.poliza_matriz ? request.CantidadTrabajadores : request.datosPoliza.branch == Convert.ToInt32(ELog.obtainConfig("accidentesBranch")) && request.datosPoliza.codTipoPerfil == "6" ? request.CantidadTrabajadores : 0;
        //                if (new string[] { ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch") }.Contains(request.datosPoliza.branch.ToString()))
        //                {
        //                    if (!quotationCORE.validarAforo(request.datosPoliza.branch.ToString(), request.datosPoliza.codTipoPerfil, request.datosPoliza.trxCode))
        //                    {
        //                        request.ruta = generarAseguradosJson(request.codProceso);
        //                    }
        //                }

        //                //ANTES DE AQUI LLENAR DPS

        //                if (new string[] { ELog.obtainConfig("vidaIndividualBranch") }.Contains(branch))
        //                {

        //                    List<Entities.QuotationModel.BindingModel.DatosDPS> datosDPS = new List<Entities.QuotationModel.BindingModel.DatosDPS>();

        //                    List<DpsVM> listaDPS = new List<DpsVM>();

        //                    listaDPS = GetDPS(Convert.ToInt32(request.nroCotizacion), Convert.ToInt32(request.codRamo), Convert.ToInt32(request.codProducto));

        //                    foreach (var item in listaDPS)
        //                    {

        //                        var typequestion = "";
        //                        var typequestionDetail = "";

        //                        if (item.NQUESTION_FATHER == "0")
        //                        {
        //                            if (item.NQUESTION == "1" || item.NQUESTION == "2")
        //                            {
        //                                typequestion = "NUMBER";
        //                            }
        //                            else if (item.NQUESTION == "3" || item.NQUESTION == "4")
        //                            {
        //                                typequestion = "SELECTION";
        //                                typequestionDetail = "NUMBER";
        //                            }
        //                            else if (item.NQUESTION == "6")
        //                            {
        //                                typequestion = "SELECTION";
        //                                typequestionDetail = "TEXT";
        //                            }
        //                            else if (item.NQUESTION == "8")
        //                            {
        //                                typequestion = "TEXT";
        //                            }
        //                            else
        //                            {
        //                                typequestion = "SELECTION";
        //                            }
        //                        }
        //                        else
        //                        {
        //                            if (item.NQUESTION == "8")
        //                            {
        //                                typequestion = "TEXT";
        //                            }
        //                            else
        //                            {
        //                                typequestion = "SELECTION";
        //                                typequestionDetail = "TEXT";
        //                            }

        //                        }

        //                        var valDPS = new Entities.QuotationModel.BindingModel.DatosDPS()
        //                        {
        //                            question = item.NQUESTION_FATHER != "0" && item.NQUESTION_FATHER != "" ? item.NQUESTION_FATHER : item.NQUESTION,
        //                            type = typequestion,
        //                            value = item.SVALUE,
        //                            questionFather = item.NQUESTION_FATHER != "0" ? item.NQUESTION : "0",
        //                            detail = item.SVALUE != "NO" && item.SVALUE != "SI" ? null : new Entities.QuotationModel.BindingModel.AnswerDetailInput() { type = typequestionDetail, value = item.SDESC_ANSWER },
        //                            items = (item.NQUESTION == "5" || item.NQUESTION == "7") && item.NQUESTION_FATHER == "" ? new List<Entities.QuotationModel.BindingModel.DatosDPS>() : null
        //                        };

        //                        if (item.NQUESTION_FATHER == "0" || item.SVALUE == "")
        //                        {
        //                            datosDPS.Add(valDPS);
        //                        }
        //                        else
        //                        {
        //                            if (item.NQUESTION == "5")
        //                            {
        //                                datosDPS[4].items.Add(valDPS);
        //                            }
        //                            else if (item.NQUESTION == "7")
        //                            {
        //                                datosDPS[6].items.Add(valDPS);
        //                            }

        //                        }

        //                    }

        //                    request.datosDPS = datosDPS;
        //                }


        //                //ANTES DE AQUI LLENAR DPS

        //                var responseTarifario = await new QuotationCORE().SendDataTarifarioGraphql(request);

        //                if (responseTarifario.codError == 0)
        //                {
        //                    // Coloco el valor de la prima que nos brinda el tarifario
        //                    request.premium = responseTarifario.data != null ? responseTarifario.data.searchPremium.commercialPremium : 0.0;

        //                    #region Simular prima
        //                    /* if (ELog.obtainConfig("simularPrimas") == "1")
        //                    {
        //                        if (request.premium == 0)
        //                        {
        //                            var array = new int[] { 0, 12, 6, 3, 2, 1, 1 };
        //                            request.premium = (10 + ((11 * request.lcoberturas.Count) * 0.75314) + (request.lasistencias.Count * 5) + (request.lbeneficios.Count * 5)) * array[Convert.ToInt32(request.datosPoliza.codTipoFrecuenciaPago)];
        //                        }

        //                    } */
        //                    #endregion

        //                    if (!request.datosPoliza.poliza_matriz)
        //                    {
        //                        if (request.premium > 0 || branch == ELog.obtainConfig("vidaIndividualBranch"))
        //                        {
        //                            //if (new string[] { "EM" }.Contains(request.datosPoliza.trxCode))
        //                            //{
        //                            //    response = InsertReajustes(request, responseTarifario.data.searchPremium.readjustmentFactors);
        //                            //}
        //                            response = await genericProcessAP(request, response, 1);
        //                            comisionBroker = response.P_COD_ERR == "0" ? response.COMISION_BROKER : 0;

        //                            if (branch == ELog.obtainConfig("vidaIndividualBranch"))
        //                            {
        //                                var dataPrima = new updateCotizador()
        //                                {
        //                                    codProceso = request.codProceso,
        //                                    nroCotizacion = request.datosPoliza.nid_cotizacion.ToString(),
        //                                    codRamo = Convert.ToInt32(request.codRamo),
        //                                    codProducto = Convert.ToInt32(request.codProducto),
        //                                    codUsuario = request.codUsuario,
        //                                    codEstado = 23, //Estado Pendiente DPS
        //                                    polMatriz = "0",
        //                                    fechaEfecto = Convert.ToDateTime(request.datosPoliza.InicioVigPoliza).ToString("dd/MM/yyyy")
        //                                };

        //                                response = response.P_COD_ERR == "0" ? quotationCORE.updatePrimaCotizador(dataPrima) : response;
        //                            }



        //                            //response = response.P_COD_ERR == "0" ? await InsertRecargos(request, request.lrecargos) : response;
        //                            //comisionBroker = response.P_COD_ERR == "0" ? response.COMISION_BROKER : 0;
        //                            //response = response.P_COD_ERR == "0" ? quotationCORE.DelelteInsuredCot(request) : response; // Metododo que hace delete a la trama si existe (TBL_PD_LOAD_INSURED_PREMIUM)
        //                            //response = response.P_COD_ERR == "0" ? quotationCORE.InsertInsuredCot(request) : response; // Metodo que inserta valor de prima por asegurado (TBL_PD_LOAD_INSURED_PREMIUM )
        //                            //response = response.P_COD_ERR == "0" ? quotationCORE.InsCotizaTrama(request) : response;
        //                            //response = quotationCORE.InsCotizaTrama(request);
        //                        }
        //                        else
        //                        {
        //                            resAuth = quotationCORE.getAuthDerivation(request);

        //                            if (new int[] { 1, 2 }.Contains(resAuth.codAuth))
        //                            {
        //                                response = await genericProcessAP(request, response, 1);
        //                                comisionBroker = response.P_COD_ERR == "0" ? response.COMISION_BROKER : 0;
        //                                //if (objValida.datosPoliza.trxCode == "EM")
        //                                //{
        //                                //    response = await InsertRecargos(objValida);
        //                                //    comisionBroker = response.P_COD_ERR == "0" ? response.COMISION_BROKER : 0;
        //                                //    response = response.P_COD_ERR == "0" ? await InsertExclusiones(objValida) : response;
        //                                //}

        //                                //response = response.P_COD_ERR == "0" ? quotationCORE.DelelteInsuredCot(objValida) : response; // Metododo que hace delete a la trama si existe (TBL_PD_LOAD_INSURED_PREMIUM)
        //                                //response = response.P_COD_ERR == "0" ? quotationCORE.InsertInsuredCot(objValida) : response; // Metodo que inserta valor de prima por asegurado (TBL_PD_LOAD_INSURED_PREMIUM )
        //                                //response = response.P_COD_ERR == "0" ? quotationCORE.InsCotizaTrama(objValida) : response;
        //                            }
        //                            else
        //                            {
        //                                response.P_COD_ERR = "1";
        //                            }
        //                            //response.P_COD_ERR = "1";
        //                            //response.P_MESSAGE = "No hay primas configuradas para los datos enviados";
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (request.premium > 0)
        //                        {
        //                            request.flag = "0";
        //                            //if (new string[] { "EM" }.Contains(request.datosPoliza.trxCode))
        //                            //{
        //                            //    response = InsertReajustes(request, responseTarifario.data.searchPremium.readjustmentFactors);
        //                            //}
        //                            response = await genericProcessAP(request, response, 1);
        //                            comisionBroker = response.P_COD_ERR == "0" ? response.COMISION_BROKER : 0;

        //                            //response = response.P_COD_ERR == "0" ? await InsertRecargos(request, request.lrecargos) : response;
        //                            //comisionBroker = response.P_COD_ERR == "0" ? response.COMISION_BROKER : 0;
        //                            ////response = quotationCORE.DelelteInsuredCot(request); // Metododo que hace delete a la trama si existe (TBL_PD_LOAD_INSURED_PREMIUM)
        //                            ////response = response.P_COD_ERR == "0" ? quotationCORE.InsertInsuredCot(request) : response; // Metodo que inserta valor de prima por asegurado (TBL_PD_LOAD_INSURED_PREMIUM )
        //                            //response = response.P_COD_ERR == "0" ? quotationCORE.InsReCotizaTrama(request) : response;
        //                            //response = quotationCORE.InsReCotizaTrama(request);
        //                        }
        //                        else
        //                        {
        //                            resAuth = quotationCORE.getAuthDerivation(request);

        //                            if (new int[] { 1, 2 }.Contains(resAuth.codAuth))
        //                            {
        //                                response = await genericProcessAP(request, response, 1);
        //                                comisionBroker = response.P_COD_ERR == "0" ? response.COMISION_BROKER : 0;
        //                                //if (objValida.datosPoliza.trxCode == "EM")
        //                                //{
        //                                //    response = await InsertRecargos(objValida);
        //                                //    comisionBroker = response.P_COD_ERR == "0" ? response.COMISION_BROKER : 0;
        //                                //    response = response.P_COD_ERR == "0" ? await InsertExclusiones(objValida) : response;
        //                                //}

        //                                //response = response.P_COD_ERR == "0" ? quotationCORE.DelelteInsuredCot(objValida) : response; // Metododo que hace delete a la trama si existe (TBL_PD_LOAD_INSURED_PREMIUM)
        //                                //response = response.P_COD_ERR == "0" ? quotationCORE.InsertInsuredCot(objValida) : response; // Metodo que inserta valor de prima por asegurado (TBL_PD_LOAD_INSURED_PREMIUM )
        //                                //response = response.P_COD_ERR == "0" ? quotationCORE.InsCotizaTrama(objValida) : response;
        //                            }
        //                            else
        //                            {
        //                                response.P_COD_ERR = "1";
        //                            }

        //                            //response.P_COD_ERR = "1";
        //                            //response.P_MESSAGE = "No hay primas configuradas para los datos enviados";
        //                        }
        //                    }

        //                    if (!request.datosPoliza.poliza_matriz)
        //                    {
        //                        if (response.P_COD_ERR == "0")
        //                        {
        //                            // flag 1 cuando se autoriza suma desde detalle de cotizacion
        //                            if (request.flagCalcular == 1)
        //                            {
        //                                response = quotationCORE.ReaListDetPremiumAut(request.codProceso.ToString());
        //                                response.COMISION_BROKER = comisionBroker;
        //                                response.P_MESSAGE = resAuth.desAuth;
        //                            }
        //                            else
        //                            {
        //                                response = quotationCORE.ReaListCotiza(request.codProceso, request.datosPoliza.codTipoRenovacion, request.datosPoliza.codTipoProducto, request.datosPoliza.branch.ToString()); ;
        //                                response.COMISION_BROKER = comisionBroker;
        //                                response.P_MESSAGE = resAuth.desAuth;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            response.P_COD_ERR = "1";
        //                            response.P_MESSAGE = new int[] { 0, 1, 2 }.Contains(resAuth.codAuth) ? resAuth.desAuth : "Hubo un inconveniente al momento de realizar los cálculos.";
        //                            //response.P_MESSAGE = resAuth.codAuth == 1 ? resAuth.desAuth : response.P_MESSAGE;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (response.P_COD_ERR == "0")
        //                        {
        //                            response = quotationCORE.ReaListCotiza(request.codProceso, request.datosPoliza.codTipoRenovacion, request.datosPoliza.codTipoProducto, request.datosPoliza.branch.ToString());
        //                            response.IND_PRIMA_MINIMA = response_error.flag;
        //                            response.COMISION_BROKER = comisionBroker;
        //                            response.P_MESSAGE = resAuth.desAuth;
        //                        }
        //                        else
        //                        {
        //                            response.P_COD_ERR = "1";
        //                            response.P_MESSAGE = new int[] { 0, 1, 2 }.Contains(resAuth.codAuth) ? resAuth.desAuth : "Hubo un inconveniente al momento de realizar los cálculos.";
        //                            //response.P_COD_ERR = "1";
        //                            //response.P_MESSAGE = "Hubo un inconveniente al momento de realizar los cálculos.";
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    response.P_COD_ERR = responseTarifario.codError.ToString();
        //                    response.P_MESSAGE = responseTarifario.message;
        //                }
        //            }
        //            else
        //            {
        //                response.P_COD_ERR = response_error.P_COD_ERR.ToString();
        //                response.P_MESSAGE = response_error.P_MESSAGE;
        //            }

        //            response.NIDPROC = request.codProceso;
        //            response.TOT_ASEGURADOS = response.TOT_ASEGURADOS == 0 ? Convert.ToInt32(request.CantidadTrabajadores) : response.TOT_ASEGURADOS;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.P_COD_ERR = "1";
        //        //response.P_MESSAGE = ex.Message;
        //        response.P_MESSAGE = "Hubo un error al generar la prima. Favor de comunicarse con sistemas";
        //        response.NIDPROC = request.codProceso;
        //    }
        //    return await Task.FromResult(response);
        //}


        //Servicio para recalcular prima cuando se da en procesar dentro de coberturas - no poliza matriz
        [Route("ReCalcularPrima")]
        [HttpPost]
        public async Task<SalidaTramaBaseVM> ReCalcularPrima()
        {
            var response = new SalidaTramaBaseVM() { P_COD_ERR = "0" };
            var resAuth = new AuthDerivationVM()
            {
                codAuth = 3,
                desAuth = "Se realizó el recalculo correctamente."
            };

            string jsonData = HttpContext.Current.Request.Params.Get("dataCalcular"); //AVS - RENTAS

            JObject jsonObject = JObject.Parse(jsonData);
            jsonObject["montoPlanilla"] = jsonObject["montoPlanilla"] == null || string.IsNullOrEmpty(jsonObject["montoPlanilla"].ToString()) ? 0 : jsonObject["montoPlanilla"];

            //var request = JsonConvert.DeserializeObject<validaTramaVM>(HttpContext.Current.Request.Params.Get("dataCalcular"));
            var request = JsonConvert.DeserializeObject<validaTramaVM>(jsonObject.ToString());

            LogControl.save(request.codProceso, "ReCalcularPrima Ini: " + HttpContext.Current.Request.Params.Get("dataCalcular"), "2", request.codAplicacion);

            try
            {
                if (request != null)
                {
                    //request.type_mov = "1";
                    string branch = request.datosPoliza.branch.ToString();
                    request.codAplicacion = request.codAplicacion ?? "PD";
                    request.flagpremium = request.flagCalcular == 1 ? 2 : 0;

                    if (new string[] { ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch") }.Contains(request.datosPoliza.branch.ToString()))
                    {
                        // request.nfactor = valRentaEstudiantil(request.datosPoliza.branch.ToString(), request.datosPoliza.codTipoProducto) ? quotationCORE.getFactor(request.datosPoliza.branch.ToString(), request.datosPoliza.codTipoProducto, request.datosPoliza.codTipoPlan) : 0;
                        response = await ReCalcularPrimaApVg(request);
                    }

                    else if (new string[] { ELog.obtainConfig("vidaIndividualBranch") }.Contains(request.datosPoliza.branch.ToString()))
                    {
                        response = await ReCalcularPrimaVC(request);
                    }

                    //if (new string[] { ELog.obtainConfig("desgravamenBranch") }.Contains(request.datosPoliza.branch.ToString()))
                    //{
                    //    response = await ReCalcularPrimaVD(request);
                    //}
                }
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = "1";
                response.P_MESSAGE = "Hubo un error al generar la prima. Favor de comunicarse con sistemas";
                response.NIDPROC = request.codProceso;
                LogControl.save("ReCalcularPrima", ex.ToString(), "3");
            }

            LogControl.save(request.codProceso, "ReCalcularPrima Fin: " + JsonConvert.SerializeObject(response, Formatting.None), "2", request.codAplicacion);

            return await Task.FromResult(response);
        }

        private async Task<SalidaTramaBaseVM> ReCalcularPrimaVC(validaTramaVM request)
        {
            var response = new SalidaTramaBaseVM() { P_COD_ERR = "0" };
            var resAuth = new AuthDerivationVM()
            {
                codAuth = 3,
                desAuth = "Se realizó el recalculo correctamente."
            };
            double comisionBroker = 0;
            double comisionInter = 0;

            var response_error = new SalidaErrorBaseVM() { P_COD_ERR = 0 };

            if (request.codAplicacion != "EC" && request.lcoberturas.Count() == 0 && request.lasistencias.Count() == 0)
            {
                request.lcoberturas = await insertCoverRequeridRecal(request);
                request.lasistencias = await insertAssitsRequiridRecal(request);
                request.lbeneficios = await insertBenefitRequiridRecal(request);
                //request.lrecargos = await insertSurchargeRequiridRecal(request);
            }
            else
            {
                response_error = quotationCORE.regDetallePlanCot(request); // Guarda Coberturas, Asistencias y Beneficios
            }

            response_error = response_error.P_COD_ERR == 0 ? quotationCORE.valSumasASeguradas(request) : response_error;
            if (response_error.P_COD_ERR == 0)
            {
                // Verifica que haya trama de aegurados
                //if (upload != null && upload.ContentLength > 0)
                //{
                //    request.ruta = String.Format(ELog.obtainConfig("pathPrincipal"), DateTime.Now.ToString("yyyyMMddHHmmss").PadLeft(14, 'C')) + "\\";

                //    if (!Directory.Exists(request.ruta))
                //    {
                //        Directory.CreateDirectory(request.ruta);
                //    }
                //    upload.SaveAs(request.ruta + upload.FileName);
                //}

                // Acá se ha validado todo correctamente - Llamamos a Tarifario AP
                //var responseTarifario = new ResponseGraph() { codError = 0 };
                //cantTrabajadores = request.CantidadTrabajadores;
                //request.CantidadTrabajadores = request.datosPoliza.poliza_matriz ? request.CantidadTrabajadores : request.datosPoliza.branch == Convert.ToInt32(ELog.obtainConfig("accidentesBranch")) && request.datosPoliza.codTipoPerfil == "6" ? request.CantidadTrabajadores : 0;


                //ANTES DE AQUI LLENAR DPS
                var datosDPS = new List<Entities.QuotationModel.BindingModel.DatosDPS>();
                var listaDPS = GetDPS(Convert.ToInt32(request.nroCotizacion), Convert.ToInt32(request.codRamo), Convert.ToInt32(request.codProducto));

                if (listaDPS != null && listaDPS.Count > 0)
                {
                    // AGF 14042023 
                    listaDPS[5].NQUESTION = "5.1";
                    listaDPS[5].NQUESTION_FATHER = "5";

                    listaDPS[6].NQUESTION = "5.2";
                    listaDPS[6].NQUESTION_FATHER = "5";

                    listaDPS[7].NQUESTION = "5.3";
                    listaDPS[7].NQUESTION_FATHER = "5";

                    foreach (var item in listaDPS)
                    {
                        var typequestion = "";
                        var typequestionDetail = "";

                        if (item.NQUESTION_FATHER == "" || item.NQUESTION_FATHER == "0")
                        {
                            if (item.NQUESTION == "1" || item.NQUESTION == "2")
                            {
                                typequestion = "NUMBER";
                            }
                            else if (item.NQUESTION == "3")
                            {
                                typequestion = "SELECTION";
                                typequestionDetail = "NUMBER";
                            }

                            else if (item.NQUESTION == "4")
                            {
                                typequestion = "SELECTION";
                                typequestionDetail = "SELECTION";
                            }

                            else if (item.NQUESTION == "6")
                            {
                                typequestion = "SELECTION";
                                typequestionDetail = "TEXT";
                            }
                            else if (item.NQUESTION == "8")
                            {
                                typequestion = "SELECTION";
                            }
                            else
                            {
                                typequestion = "SELECTION";
                            }
                        }
                        else
                        {
                            if (item.NQUESTION == "8")
                            {
                                typequestion = "SELECTION";
                            }
                            else if (item.NQUESTION == "5")
                            {

                                typequestion = "TEXT";
                            }
                            else
                            {
                                typequestion = "SELECTION";
                                typequestionDetail = "TEXT";
                            }

                        }

                        var valDPS = new Entities.QuotationModel.BindingModel.DatosDPS()
                        {
                            question = item.NQUESTION,
                            type = typequestion,
                            value = item.SVALUE,
                            questionFather = item.NQUESTION_FATHER != "0" ? item.NQUESTION_FATHER : null,
                            //detail = item.SVALUE != "NO" && item.SVALUE != "SI" ? null : new Entities.QuotationModel.BindingModel.AnswerDetailInput() { type = typequestionDetail, value = item.SDESC_ANSWER },
                            detail = new Entities.QuotationModel.BindingModel.AnswerDetailInput() { type = typequestionDetail, value = item.SDESC_ANSWER },
                            items = null
                        };

                        datosDPS.Add(valDPS);

                        /*
                        if (item.NQUESTION_FATHER == "" || item.NQUESTION_FATHER == "0" || item.SVALUE == "")
                        {
                            datosDPS.Add(valDPS);
                        }
                        else
                        {

                            if (item.NQUESTION == "7")
                            {
                                //datosDPS[6].items.Add(valDPS);
                            }
                        }
                        */

                    }

                    /*
                    foreach (var item in listaDPS)
                    {
                        var typequestion = "SELECTION";
                        var typequestionDetail = "TEXT";

                        var valDPS2 = new Entities.QuotationModel.BindingModel.DatosDPS()
                        {

                            question = item.NQUESTION_FATHER != "0" && item.NQUESTION_FATHER != "" ? item.NQUESTION_FATHER : item.NQUESTION,
                            type = typequestion,
                            value = item.SVALUE,
                            questionFather = item.NQUESTION_FATHER != "0" ? item.NQUESTION : "0",
                            detail = new Entities.QuotationModel.BindingModel.AnswerDetailInput() { type = typequestionDetail, value = item.SDESC_ANSWER },
                            items = (item.NQUESTION == "5" || item.NQUESTION == "7") && item.NQUESTION_FATHER == "" ? new List<Entities.QuotationModel.BindingModel.DatosDPS>() : null
                        };

                        if (item.NQUESTION_FATHER == "5")
                        {
                            datosDPS[4].items.Add(valDPS2);
                        }

                    }
                    */

                    request.datosDPS = datosDPS;

                    //ANTES DE AQUI LLENAR DPS
                    var responseTarifario = await new QuotationCORE().SendDataTarifarioGraphql(request);

                    if (responseTarifario.codError == 0)
                    {
                        // Coloco el valor de la prima que nos brinda el tarifario
                        request.premium = responseTarifario.data != null ? responseTarifario.data.searchPremium.commercialPremium : 0.0;

                        #region Simular prima
                        /* if (ELog.obtainConfig("simularPrimas") == "1")
                        {
                        if (request.premium == 0)
                        {
                            var array = new int[] { 0, 12, 6, 3, 2, 1, 1 };
                            request.premium = (10 + ((11 * request.lcoberturas.Count) * 0.75314) + (request.lasistencias.Count * 5) + (request.lbeneficios.Count * 5)) * array[Convert.ToInt32(request.datosPoliza.codTipoFrecuenciaPago)];
                        }

                     } */
                        #endregion

                        if (!request.datosPoliza.poliza_matriz)
                        {

                            //if (new string[] { "EM" }.Contains(request.datosPoliza.trxCode))
                            //{
                            //    response = InsertReajustes(request, responseTarifario.data.searchPremium.readjustmentFactors);
                            //}
                            // Llamar al miniTariff
                            response = await genericProcessPD(request, response, 1);
                            comisionBroker = response.P_COD_ERR == "0" ? response.COMISION_BROKER : 0;
                            comisionInter = response.P_COD_ERR == "0" ? response.COMISION_INTER : 0;

                            var dataPrima = new updateCotizador()
                            {
                                codProceso = request.codProceso,
                                nroCotizacion = request.datosPoliza.nid_cotizacion.ToString(),
                                codRamo = Convert.ToInt32(request.codRamo),
                                codProducto = Convert.ToInt32(request.codProducto),
                                codUsuario = request.codUsuario,
                                codEstado = 23, //Estado Pendiente DPS
                                polMatriz = "0",
                                fechaEfecto = Convert.ToDateTime(request.datosPoliza.InicioVigPoliza).ToString("dd/MM/yyyy")
                            };

                            response = response.P_COD_ERR == "0" ? quotationCORE.updatePrimaCotizador(dataPrima) : response;

                        }
                        else
                        {
                            if (request.premium > 0)
                            {
                                request.flag = "0";
                                //if (new string[] { "EM" }.Contains(request.datosPoliza.trxCode))
                                //{
                                //    response = InsertReajustes(request, responseTarifario.data.searchPremium.readjustmentFactors);
                                //}
                                response = await genericProcessPD(request, response, 1);
                                comisionBroker = response.P_COD_ERR == "0" ? response.COMISION_BROKER : 0;
                                comisionInter = response.P_COD_ERR == "0" ? response.COMISION_INTER : 0;
                            }
                            else
                            {
                                resAuth = quotationCORE.getAuthDerivation(request);

                                if (new int[] { 1, 2 }.Contains(resAuth.codAuth))
                                {
                                    response = await genericProcessPD(request, response, 1);
                                    comisionBroker = response.P_COD_ERR == "0" ? response.COMISION_BROKER : 0;
                                    comisionInter = response.P_COD_ERR == "0" ? response.COMISION_INTER : 0;
                                    //if (objValida.datosPoliza.trxCode == "EM")
                                    //{
                                    //    response = await InsertRecargos(objValida);
                                    //    comisionBroker = response.P_COD_ERR == "0" ? response.COMISION_BROKER : 0;
                                    //    response = response.P_COD_ERR == "0" ? await InsertExclusiones(objValida) : response;
                                    //}

                                    //response = response.P_COD_ERR == "0" ? quotationCORE.DelelteInsuredCot(objValida) : response; // Metododo que hace delete a la trama si existe (TBL_PD_LOAD_INSURED_PREMIUM)
                                    //response = response.P_COD_ERR == "0" ? quotationCORE.InsertInsuredCot(objValida) : response; // Metodo que inserta valor de prima por asegurado (TBL_PD_LOAD_INSURED_PREMIUM )
                                    //response = response.P_COD_ERR == "0" ? quotationCORE.InsCotizaTrama(objValida) : response;
                                }
                                else
                                {
                                    response.P_COD_ERR = "3";
                                }

                                //response.P_COD_ERR = "1";
                                //response.P_MESSAGE = "No hay primas configuradas para los datos enviados";
                            }
                        }

                        if (!request.datosPoliza.poliza_matriz)
                        {
                            if (response.P_COD_ERR == "0")
                            {
                                // flag 1 cuando se autoriza suma desde detalle de cotizacion
                                if (request.flagCalcular == 1)
                                {
                                    response = quotationCORE.ReaListDetPremiumAut(request.codProceso.ToString());
                                    response.COMISION_BROKER = comisionBroker;
                                    response.COMISION_INTER = comisionInter;
                                    response.P_MESSAGE = resAuth.desAuth;
                                }
                                else
                                {
                                    response = quotationCORE.ReaListCotiza(request.codProceso, request.datosPoliza.codTipoRenovacion, request.datosPoliza.codTipoProducto, request.datosPoliza.branch.ToString()); ;
                                    response.COMISION_BROKER = comisionBroker;
                                    response.COMISION_INTER = comisionInter;
                                    response.P_MESSAGE = resAuth.desAuth;
                                }
                            }
                            else
                            {
                                response.P_COD_ERR = "1";
                                response.P_MESSAGE = new int[] { 0, 1, 2 }.Contains(resAuth.codAuth) ? resAuth.desAuth : "Hubo un inconveniente al momento de realizar los cálculos.";
                                //response.P_MESSAGE = resAuth.codAuth == 1 ? resAuth.desAuth : response.P_MESSAGE;
                            }
                        }
                        else
                        {
                            if (response.P_COD_ERR == "0")
                            {
                                response = quotationCORE.ReaListCotiza(request.codProceso, request.datosPoliza.codTipoRenovacion, request.datosPoliza.codTipoProducto, request.datosPoliza.branch.ToString());
                                response.IND_PRIMA_MINIMA = response_error.flag;
                                response.COMISION_BROKER = comisionBroker;
                                response.COMISION_INTER = comisionInter;
                                response.P_MESSAGE = resAuth.desAuth;
                            }
                            else
                            {
                                response.P_COD_ERR = "1";
                                response.P_MESSAGE = new int[] { 0, 1, 2 }.Contains(resAuth.codAuth) ? resAuth.desAuth : "Hubo un inconveniente al momento de realizar los cálculos.";
                                //response.P_COD_ERR = "1";
                                //response.P_MESSAGE = "Hubo un inconveniente al momento de realizar los cálculos.";
                            }
                        }
                    }
                    else
                    {
                        response.P_COD_ERR = responseTarifario.codError.ToString();
                        response.P_MESSAGE = responseTarifario.message;
                    }
                }
                else
                {
                    response.P_COD_ERR = "1";
                    response.P_MESSAGE = "Hubo un inconveniente al momento de realizar los cálculos.";
                }
            }
            else
            {
                response.P_COD_ERR = response_error.P_COD_ERR.ToString();
                response.P_MESSAGE = response_error.P_MESSAGE;
            }

            response.NIDPROC = request.codProceso;
            response.TOT_ASEGURADOS = response.TOT_ASEGURADOS == 0 ? Convert.ToInt32(request.CantidadTrabajadores) : response.TOT_ASEGURADOS;

            return response;
        }

        private async Task<SalidaTramaBaseVM> ReCalcularPrimaApVg(validaTramaVM request)
        {

            var response = new SalidaTramaBaseVM() { P_COD_ERR = "0" };
            var resAuth = new AuthDerivationVM()
            {
                codAuth = 3,
                desAuth = "Se realizó el recalculo correctamente."
            };

            LogControl.save(request.codProceso, "ReCalcularPrimaApVg Ini: " + JsonConvert.SerializeObject(request, Formatting.None), "2", request.codAplicacion);

            if (request.lcoberturas.Count() > 0)
            {
                var response_error = quotationCORE.regDetallePlanCot(request); // Guarda Coberturas, Asistencias y Beneficios -- ED Servicios Adicionales

                response_error = response_error.P_COD_ERR == 0 ? quotationCORE.valSumasASeguradas(request) : response_error;

                if (response_error.P_COD_ERR == 0)
                {

                    //request.ruta = generarAseguradosJson(request);

                    var miniTariff = await getMiniTariffGraph(request);

                    if (miniTariff.codError == 0)
                    {
                        if (miniTariff.data.miniTariffMatrix != null &&
                            miniTariff.data.miniTariffMatrix.segment != null &&
                            miniTariff.data.miniTariffMatrix.segment.modules != null)
                        {
                            if (miniTariff.data.miniTariffMatrix.segment.modules.Count() > 0)
                            {
                                var aseguradoList = new QuotationCORE().GetTramaAsegurado(request.codProceso);

                                LogControl.save(request.codProceso, "ReCalcularPrimaApVg aseguradoList: " + JsonConvert.SerializeObject(aseguradoList, Formatting.None), "2", request.codAplicacion);

                                getPremiumTariff(request.CantidadTrabajadores, aseguradoList, ref request, ref response);
                                response = response.P_COD_ERR == "0" ? processPremiumPD(request, miniTariff, "recalcular", ref response, ref resAuth) : response;

                                if (response.P_COD_ERR == "0")
                                {
                                    getInfoListPremium(request, resAuth, ref response);

                                    response.cotizadorDetalleList = new QuotationCORE().detailQuotationInfo(aseguradoList, request); // modificar
                                    response.P_COD_ERR = response.P_COD_ERR == "0" && (new string[] { "IN", "DE", "RE" }).Contains(request.datosPoliza.trxCode) ? resAuth.codAuth == 1 ? "2" : response.P_COD_ERR : response.P_COD_ERR;
                                    response.P_QUESTION = response.P_COD_ERR == "2" ? "La transacción va a ser derivada al área técnica por límite de edad, ¿Desea continuar?" : null;
                                }
                                else
                                {
                                    response.P_MESSAGE = new int[] { 0, 1, 2 }.Contains(resAuth.codAuth) ? resAuth.desAuth : response.P_MESSAGE;
                                }
                            }
                        }
                    }


                }
                else
                {
                    response.P_COD_ERR = response_error.P_COD_ERR.ToString();
                    response.P_MESSAGE = response_error.P_MESSAGE;
                }

                response.NIDPROC = request.codProceso;
                response.TOT_ASEGURADOS = response.TOT_ASEGURADOS == 0 ? Convert.ToInt32(request.CantidadTrabajadores) : response.TOT_ASEGURADOS;
                response.MONT_PLANILLA = response.MONT_PLANILLA == 0 ? Convert.ToInt32(request.montoPlanilla) : response.MONT_PLANILLA;
                response.P_MENSAJE_TECNICA = resAuth.codAuth == 2 ? 1 : 0; //AVS - RENTAS
            }

            LogControl.save(request.codProceso, "ReCalcularPrimaApVg Fin: " + JsonConvert.SerializeObject(request, Formatting.None), "2", request.codAplicacion);

            return response;
        }


        //Servicio para recalcular prima cuando se da en procesar dentro de coberturas en check poliza matriz
        //[Route("ReCalcularPrimaPol")]
        //[HttpPost]
        //public Task<SalidaTramaBaseVM> CalcularPrimaPol(RecalculoTramaVM request)
        //{
        //    SalidaTramaBaseVM response = new SalidaTramaBaseVM();
        //    SalidaErrorBaseVM response_error = new SalidaErrorBaseVM();
        //    int flag = 0;

        //    try
        //    {
        //        if (request != null)
        //        {
        //            foreach (var item in request.lcoberturas)
        //            {
        //                flag = 1;
        //                response_error = quotationCORE.UpdCoberturasProp(item.codCobertura, item.sumaPropuesta, request.codProceso, flag);
        //            }

        //            if (response_error.P_COD_ERR == 0)
        //            {
        //                //request.type_mov = "1";
        //                request.flag = "0";
        //                response_error = quotationCORE.InsReCotizaTrama(request);
        //            }



        //            if (response_error.P_COD_ERR == 0)
        //            {

        //                response = quotationCORE.ReaListCotiza(request.codProceso, request.datosPoliza.codTipoRenovacion, request.datosPoliza.codTipoProducto, request.datosPoliza.branch.ToString());
        //                response.NIDPROC = request.codProceso;
        //                response.TOT_ASEGURADOS = request.CantidadTrabajadores.ToString();
        //                response.IND_PRIMA_MINIMA = response_error.flag;

        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.P_COD_ERR = "1";
        //        response.P_MESSAGE = ex.Message;
        //    }
        //    return Task.FromResult(response);
        //}

        //[Route("InsCoberturaDet")]
        //[HttpPost]
        //public Task<EmitPolVM> InsCoberturaDet(DatosPolizaVM request)
        //{
        //    EmitPolVM response = new EmitPolVM();

        //    try
        //    {
        //        if (request != null)
        //        {
        //            request.codProceso = (request.codusuario + GenerarCodigo() + DateTime.Now.ToString("yyyyMMddHHmmss")).PadLeft(30, '0');
        //            response = quotationCORE.InsCoberturaDet(request);
        //            response.idproc = request.codProceso;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        response.cod_error = 1;
        //        response.message = ex.Message;
        //    }
        //    return Task.FromResult(response);
        //}

        private async Task<SalidaTramaBaseVM> tramaDesgravamen(DataTable dataTable, HttpPostedFile upload, validaTramaVM objValida, ValidaTramaBM validaTramaBM, long maxTrama)
        {
            LogControl.save("tramaDesgravamen", JsonConvert.SerializeObject(objValida, Formatting.None), "2");
            var response = new SalidaTramaBaseVM();
            var listaspremium = new SalidaPremiumVM();
            var resAuth = new AuthDerivationVM()
            {
                codAuth = 3,
                desAuth = "Se validó correctamente la trama"
            };
            //Cantidad de Asegurados---PARA PROBAR
            maxTrama = 1000000;
            //Cantidad de Asegurados---PARA PROBAR

            try
            {
                var tramaList = new List<tramaIngresoBM>();

                Int32 count = 1;
                Int32 rows = Convert.ToInt32(ELog.obtainConfig("rows" + objValida.codRamo));
                double comisionBroker = 0;
                Int64 cantidadTrabajadores = 0;
                double primatotal = 0;
                DataTable dt = new DataTable();
                //dt.Columns.Add("NPOLICY");
                //ARJG
                double ncpremium = 0;
                String sncapital = "";
                String scontratante = "";
                //---

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
                dt.Columns.Add("NCIVILSTA");
                dt.Columns.Add("SSTREET");
                dt.Columns.Add("SPROVINCE");
                dt.Columns.Add("SLOCAL");
                dt.Columns.Add("NMUNUCIPALITY");
                dt.Columns.Add("SE_MAIL");
                dt.Columns.Add("NPHONE_TYPE");
                dt.Columns.Add("SPHONE");
                dt.Columns.Add("NIDDOC_TYPE_BENEFICIARY");
                dt.Columns.Add("SIDDOC_BENEFICIARY");
                dt.Columns.Add("TYPE_PLAN");
                dt.Columns.Add("PERCEN_PARTICIPATION");
                dt.Columns.Add("SRELATION");
                dt.Columns.Add("SAPE_CASADA");
                dt.Columns.Add("SLASTNAME2CONCAT");
                dt.Columns.Add("SCLIENT");
                dt.Columns.Add("NCERTIF_CLI");
                //dt.Columns.Add("NROLE");

                //dt.Columns.Add("SSTATUSVAR");
                //dt.Columns.Add("NPAYFREQ");
                //dt.Columns.Add("SPOPULATION");
                //dt.Columns.Add("SBUILD");
                //dt.Columns.Add("SDEPARTMENT");
                //dt.Columns.Add("SREFERENCE");

                dt.Columns.Add("SCREDITNUM");
                dt.Columns.Add("DINIT_CRE");
                dt.Columns.Add("DEND_CRE");
                //dt.Columns.Add("NCURREN_CRE");
                dt.Columns.Add("NAMOUNT_CRE");
                dt.Columns.Add("NAMOUNT_ACT");
                //dt.Columns.Add("NCALCAPITAL");
                //dt.Columns.Add("NCAPITAL");
                dt.Columns.Add("NQ_QUOT");
                //dt.Columns.Add("NMODULEC");
                //dt.Columns.Add("NRATEDESG");
                dt.Columns.Add("NTYPPREMIUM");
                dt.Columns.Add("NPREMIUMN");
                dt.Columns.Add("NIGV");
                dt.Columns.Add("NDE");
                dt.Columns.Add("NPREMIUM");
                //dt.Columns.Add("DSTARTDATE");
                //dt.Columns.Add("DEXPIRDAT");
                //dt.Columns.Add("NNUMAGE");
                //dt.Columns.Add("NNUMPRO");
                //dt.Columns.Add("DPROCESS");
                //dt.Columns.Add("STERMINAL");
                //dt.Columns.Add("NIDCLIENTLOCATION");
                //dt.Columns.Add("NCOD_NETEO");



                while (rows < dataTable.Rows.Count)
                {
                    DataRow dr = dt.NewRow();

                    //arjg 28112023
                    if (objValida.nconfigdcd == 0)
                    {
                        sncapital = dataTable.Rows[rows][32].ToString().Trim();
                        if (sncapital == "1")
                        {
                            ncpremium = Double.Parse(dataTable.Rows[rows][25].ToString().Trim()) * Double.Parse(dataTable.Rows[rows][31].ToString().Trim());
                        }
                        if (sncapital == "2")
                        {
                            ncpremium = Double.Parse(dataTable.Rows[rows][26].ToString().Trim()) * Double.Parse(dataTable.Rows[rows][31].ToString().Trim());
                        }
                        scontratante = "";
                    }
                    else
                    {
                        scontratante = objValida.sclientdcd;
                        ncpremium = Double.Parse(dataTable.Rows[rows][33].ToString().Trim());
                    }
                    //---
                    dr["NID_COTIZACION"] = objValida.nroCotizacion == 0 ? null : objValida.nroCotizacion;
                    dr["NID_PROC"] = objValida.codProceso;
                    dr["SFIRSTNAME"] = dataTable.Rows[rows][8].ToString().Trim() == "" ? null : dataTable.Rows[rows][8].ToString().Trim();
                    dr["SLASTNAME"] = dataTable.Rows[rows][6].ToString().Trim() == "" ? null : dataTable.Rows[rows][6].ToString().Trim();
                    dr["SLASTNAME2"] = dataTable.Rows[rows][7].ToString().Trim() == "" ? null : dataTable.Rows[rows][7].ToString().Trim();
                    dr["NMODULEC"] = ELog.obtainConfig("riesgoDefaultAP");
                    dr["NNATIONALITY"] = "";
                    dr["NIDDOC_TYPE"] = dataTable.Rows[rows][3].ToString().Trim() == "" ? null : dataTable.Rows[rows][3].ToString().Trim();
                    dr["SIDDOC"] = getDatoTramaDesDev(dataTable.Rows[rows], "documento", "asegurado");//dataTable.Rows[rows][4].ToString().Trim() == "" ? null : dataTable.Rows[rows][4].ToString().Trim();
                    dr["DBIRTHDAT"] = dataTable.Rows[rows][9].ToString().Trim() == "" ? null : Generic.ValidFormatDate(dataTable.Rows[rows][9].ToString().Trim())?.ToString("dd/MM/yyyy");
                    dr["NREMUNERACION"] = "";
                    dr["NIDCLIENTLOCATION"] = null;
                    dr["NCOD_NETEO"] = null;
                    dr["NUSERCODE"] = objValida.codUsuario;
                    dr["SSTATREGT"] = null;
                    dr["SCOMMENT"] = null;
                    dr["NID_REG"] = count;
                    dr["SSEXCLIEN"] = dataTable.Rows[rows][10].ToString().Trim() == "" ? null : dataTable.Rows[rows][10].ToString().Trim();
                    dr["NACTION"] = DBNull.Value;
                    dr["SROLE"] = dataTable.Rows[rows][5].ToString().Trim() != "2" ? "" : "Asegurado";//"Asegurado";                                                                                                                                                         
                    dr["NCIVILSTA"] = dataTable.Rows[rows][11].ToString().Trim() == "" ? null : dataTable.Rows[rows][11].ToString().Trim();
                    dr["SSTREET"] = dataTable.Rows[rows][13].ToString().Trim() == "" ? null : dataTable.Rows[rows][13].ToString().Trim();
                    dr["SPROVINCE"] = dataTable.Rows[rows][17].ToString().Trim() == "" ? null : dataTable.Rows[rows][17].ToString().Trim().Length == 5 ? dataTable.Rows[rows][17].ToString().Trim().Substring(1, 2) : dataTable.Rows[rows][17].ToString().Trim().Substring(2, 2);
                    dr["SLOCAL"] = dataTable.Rows[rows][17].ToString().Trim() == "" ? null : dataTable.Rows[rows][17].ToString().Trim().Length == 5 ? dataTable.Rows[rows][17].ToString().Trim().Substring(1) : dataTable.Rows[rows][17].ToString().Trim().Substring(2);
                    dr["NMUNUCIPALITY"] = dataTable.Rows[rows][17].ToString().Trim() == "" ? null : dataTable.Rows[rows][17].ToString().Trim();
                    dr["SE_MAIL"] = dataTable.Rows[rows][38].ToString().Trim() == "" ? null : dataTable.Rows[rows][38].ToString().Trim();
                    dr["NPHONE_TYPE"] = dataTable.Rows[rows][19].ToString().Trim() == "" ? null : dataTable.Rows[rows][19].ToString().Trim();
                    dr["SPHONE"] = dataTable.Rows[rows][20].ToString().Trim() == "" ? null : dataTable.Rows[rows][20].ToString().Trim();

                    dr["NIDDOC_TYPE_BENEFICIARY"] = "";
                    dr["SIDDOC_BENEFICIARY"] = "";
                    dr["TYPE_PLAN"] = null;
                    dr["PERCEN_PARTICIPATION"] = "";
                    dr["SRELATION"] = "";

                    dr["SAPE_CASADA"] = null;
                    dr["SLASTNAME2CONCAT"] = null;
                    dr["SCLIENT"] = scontratante;

                    dr["NCERTIF_CLI"] = dataTable.Rows[rows][1].ToString().Trim() == "" ? null : dataTable.Rows[rows][1].ToString().Trim();
                    //dr["NROLE"] = dataTable.Rows[rows][5].ToString().Trim() == "" ? null : dataTable.Rows[rows][5].ToString().Trim();                                                       //Confirmar si NMODULEC

                    //dr["SSTATUSVAR"] = dataTable.Rows[rows][2].ToString().Trim() == "" ? null : dataTable.Rows[rows][2].ToString().Trim();


                    //dr["NPAYFREQ"] = dataTable.Rows[rows][12].ToString().Trim() == "" ? null : dataTable.Rows[rows][12].ToString().Trim();

                    //dr["SPOPULATION"] = dataTable.Rows[rows][14].ToString().Trim() == "" ? null : dataTable.Rows[rows][14].ToString().Trim();
                    //dr["SBUILD"] = dataTable.Rows[rows][15].ToString().Trim() == "" ? null : dataTable.Rows[rows][15].ToString().Trim();
                    //dr["SDEPARTMENT"] = dataTable.Rows[rows][16].ToString().Trim() == "" ? null : dataTable.Rows[rows][16].ToString().Trim();


                    //dr["SREFERENCE"] = dataTable.Rows[rows][18].ToString().Trim() == "" ? null : dataTable.Rows[rows][18].ToString().Trim();

                    dr["SCREDITNUM"] = dataTable.Rows[rows][21].ToString().Trim() == "" ? null : dataTable.Rows[rows][21].ToString().Trim();
                    dr["DINIT_CRE"] = dataTable.Rows[rows][22].ToString().Trim() == "" ? null : Generic.ValidFormatDate(dataTable.Rows[rows][22].ToString().Trim())?.ToString("dd/MM/yyyy");
                    dr["DEND_CRE"] = dataTable.Rows[rows][23].ToString().Trim() == "" ? null : Generic.ValidFormatDate(dataTable.Rows[rows][23].ToString().Trim())?.ToString("dd/MM/yyyy");
                    //dr["NCURREN_CRE"] = dataTable.Rows[rows][24].ToString().Trim() == "" ? null : dataTable.Rows[rows][24].ToString().Trim();
                    dr["NAMOUNT_CRE"] = dataTable.Rows[rows][25].ToString().Trim() == "" ? null : dataTable.Rows[rows][25].ToString().Trim();
                    dr["NAMOUNT_ACT"] = dataTable.Rows[rows][26].ToString().Trim() == "" ? null : dataTable.Rows[rows][26].ToString().Trim();
                    //dr["NCALCAPITAL"] = dataTable.Rows[rows][27].ToString().Trim() == "" ? null : dataTable.Rows[rows][27].ToString().Trim();
                    //dr["NCAPITAL"] = dataTable.Rows[rows][28].ToString().Trim() == "" ? null : Convert.ToDouble(dataTable.Rows[rows][28].ToString().Trim()).ToString("0.00");
                    dr["NQ_QUOT"] = dataTable.Rows[rows][29].ToString().Trim() == "" ? null : dataTable.Rows[rows][29].ToString().Trim();
                    //dr["NMODULEC"] = dataTable.Rows[rows][30].ToString().Trim() == "" ? null : dataTable.Rows[rows][30].ToString().Trim();
                    //dr["NRATEDESG"] = dataTable.Rows[rows][31].ToString().Trim() == "" ? null : Convert.ToDouble(dataTable.Rows[rows][31].ToString().Trim()).ToString("0.000000");
                    dr["NTYPPREMIUM"] = dataTable.Rows[rows][32].ToString().Trim() == "" ? null : dataTable.Rows[rows][32].ToString().Trim();
                    dr["NPREMIUMN"] = null;
                    dr["NIGV"] = null;
                    dr["NDE"] = null;
                    dr["NPREMIUM"] = ncpremium.ToString().Trim();
                    //dr["NPREMIUM"] = dataTable.Rows[rows][33].ToString().Trim() == "" ? null : dataTable.Rows[rows][33].ToString().Trim();
                    //dr["DSTARTDATE"] = dataTable.Rows[rows][34].ToString().Trim() == "" ? null : Convert.ToDateTime(dataTable.Rows[rows][34].ToString()).ToString("dd/MM/yyyy");
                    //dr["DEXPIRDAT"] = dataTable.Rows[rows][35].ToString().Trim() == "" ? null : Convert.ToDateTime(dataTable.Rows[rows][35].ToString()).ToString("dd/MM/yyyy");
                    //dr["NNUMAGE"] = dataTable.Rows[rows][36].ToString().Trim() == "" ? null : dataTable.Rows[rows][36].ToString().Trim();
                    //dr["NNUMPRO"] = dataTable.Rows[rows][37].ToString().Trim() == "" ? null : dataTable.Rows[rows][37].ToString().Trim();
                    //dr["DPROCESS"] = DateTime.Now.ToString("dd/MM/yyyy");
                    //dr["STERMINAL"] = "0";
                    //dr["NIDCLIENTLOCATION"] = null;
                    //dr["NCOD_NETEO"] = null;
                    //cantidadTrabajadores = dataTable.Rows[rows][1].ToString().Trim() == "Asegurado" ? cantidadTrabajadores + 1 : cantidadTrabajadores;


                    //primatotal = primatotal + Double.Parse(dataTable.Rows[rows][33].ToString().Trim() == "" ? "0" : dataTable.Rows[rows][33].ToString().Trim());
                    primatotal = primatotal + Double.Parse(ncpremium.ToString().Trim());
                    var prueba = dataTable.Rows[rows][31].ToString().Trim();
                    objValida.p_ntasa_calculada_desdev = Math.Round(Double.Parse(dataTable.Rows[rows][31].ToString().Trim() == "" ? "0" : dataTable.Rows[rows][31].ToString().Trim()) * 1000, 6);

                    count++;
                    rows++;
                    dt.Rows.Add(dr);
                }

                if (maxTrama == -1 || count <= maxTrama)
                {
                    /*
                    //flag cot 1 cuando ingresa trama desde poliza matriz  -- flag cot 2 cuando ingresa trama desde el proceso de renovacion
                    if ((new string[] { "1", "2" }).Contains(objValida.flagCot) || objValida.codAplicacion == "EC")
                    {
                        
                    }
                    */
                    //quotationCORE.LimpiarTramaDes(objValida.codProceso);
                    //response = quotationCORE.SaveUsingOracleBulkCopyDesDev(dt);


                    response = quotationCORE.SaveUsingOracleBulkCopy(dt);

                    if (response.P_COD_ERR == "0")
                    {
                        objValida.premium = primatotal;

                        if (objValida.premium > 0)
                        {

                            if (objValida.datosPoliza.trxCode == "EM")
                            //    || (objValida.datosPoliza.trxCode == "RE" && Convert.ToBoolean(objValida.datosPoliza.modoEditar)))
                            {

                                if (objValida.flagCot == "1")
                                {
                                    var trama = new ValidaTramaBM()
                                    {
                                        NID_COTIZACION = Convert.ToInt32(objValida.nroCotizacion),
                                        NTYPE_TRANSAC = "1",
                                        NID_PROC = objValida.codProceso
                                    };
                                    quotationCORE.insertHistTrama(trama);
                                }

                                if (String.IsNullOrEmpty(objValida.codProcesoOld))
                                {
                                    if (objValida.datosPoliza.trxCode == "EM" ||
                                            (objValida.datosPoliza.trxCode == "RE" && Convert.ToBoolean(objValida.datosPoliza.modoEditar)))
                                    {
                                        objValida.lcoberturas = insertCoverRequeridDesDev(objValida);
                                    }
                                    else
                                    {
                                        // Acá deberia haber un sp que traiga las coberturas / beneficios / asistencias
                                        // Para llamar al tarifario con esa data
                                        // Si es poliza matriz la data guardada
                                        // Si es alguna transac deberia  se la data de la ultima transac
                                        objValida = new QuotationCORE().getInfoQuotationTransac(objValida);
                                    }
                                }
                                else
                                {
                                    // Acá deberia haber un sp que traiga las coberturas / beneficios / asistencias
                                    // Para llamar al tarifario con esa data
                                    // Si es poliza matriz la data guardada
                                    // Si es alguna transac deberia  se la data de la ultima transac
                                    objValida = new QuotationCORE().getInfoQuotationTransac(objValida);
                                    var responseReplica = new QuotationCORE().InsertInfoMatriz(objValida);
                                }


                            }

                            response.baseError = quotationCORE.ValidateTramaAccidentesPersonales(validaTramaBM, objValida);

                            if (response.baseError.errorList.Count == 0 && response.baseError.P_COD_ERR == 0 && response.P_COD_ERR == "0")
                            {
                                response = await genericProcessPD(objValida, response);
                                comisionBroker = response.P_COD_ERR == "0" ? response.COMISION_BROKER : 0;
                            }
                            else
                            {
                                response.P_COD_ERR = "1";
                                response.P_MESSAGE = !String.IsNullOrEmpty(response.baseError.P_MESSAGE) ? response.baseError.P_MESSAGE : "Error en el proceso";
                            }



                        }
                        else
                        {
                            response.P_COD_ERR = "1";
                            response.P_MESSAGE = "La trama no se pudo guardar correctamente.";
                        }

                        /*if (response.baseError.errorList.Count == 0 && response.baseError.P_COD_ERR == 0 && response.P_COD_ERR == "0")
                        {
                        */


                        //if (objValida.datosPoliza.trxCode == "EM")
                        //{
                        //    //response = InsertReajustes(objValida, responseTarifario.data.searchPremium.readjustmentFactors);
                        //    //response = response.P_COD_ERR == "0" ? await InsertRecargos(objValida) : response;
                        //    response = await InsertRecargos(objValida);
                        //    comisionBroker = response.P_COD_ERR == "0" ? response.COMISION_BROKER : 0;
                        //    response = response.P_COD_ERR == "0" ? await InsertExclusiones(objValida) : response;
                        //}

                        //response = response.P_COD_ERR == "0" ? quotationCORE.DelelteInsuredCot(objValida) : response; // Metododo que hace delete a la trama si existe (TBL_PD_LOAD_INSURED_PREMIUM)
                        //response = response.P_COD_ERR == "0" ? quotationCORE.InsertInsuredCot(objValida) : response; // Metodo que inserta valor de prima por asegurado (TBL_PD_LOAD_INSURED_PREMIUM )
                        //response = response.P_COD_ERR == "0" ? quotationCORE.InsCotizaTrama(objValida) : response;
                    }
                    /*
                    else
                    {
                        resAuth = quotationCORE.getAuthDerivation(objValida);

                        if (resAuth.codAuth == 1)
                        {
                            response = await genericProcessAP(objValida, response);
                            comisionBroker = response.P_COD_ERR == "0" ? response.COMISION_BROKER : 0;
                            response.P_COD_ERR = objValida.codAplicacion == "EC" ? "1" : response.P_COD_ERR;
                        }
                        else
                        {
                            response.P_COD_ERR = "1";
                        }
                    }
                    */

                    if (response.P_COD_ERR == "0")
                    {
                        //flag cot 0 cuando se ingresa asegurados desde una poliza normal
                        if (objValida.flagCot == "0")
                        {
                            response = quotationCORE.ReaListCotiza(objValida.codProceso, objValida.datosPoliza.codTipoRenovacion, objValida.datosPoliza.codTipoProducto, objValida.codRamo);
                            response.COMISION_BROKER = comisionBroker;
                            response.P_MESSAGE = resAuth.desAuth;
                        }
                        else
                        {
                            listaspremium = quotationCORE.ReaListDetCotiza(objValida.codProceso, "", objValida.codProducto, objValida.codRamo, objValida.flagCot);
                            response.amountPremiumListAnu = listaspremium.amountPremiumAnu;
                            response.amountPremiumListProp = listaspremium.amountPremiumProp;
                            response.amountPremiumListAut = listaspremium.amountPremiumAut;
                            response.TOT_ASEGURADOS = Convert.ToInt32(listaspremium.TOT_ASEGURADOS);
                            response.PRIMA = listaspremium.PRIMA;
                            response.COMISION_BROKER = comisionBroker;
                            response.P_MESSAGE = resAuth.desAuth;
                        }
                        response.p_ntasa_calculada_desdev = objValida.p_ntasa_calculada_desdev;
                        response.P_COD_ERR = response.P_COD_ERR == "0" && (new string[] { "IN", "DE", "RE" }).Contains(objValida.datosPoliza.trxCode) ? resAuth.codAuth == 1 ? "2" : response.P_COD_ERR : response.P_COD_ERR;
                        response.P_QUESTION = response.P_COD_ERR == "2" ? "La transacción va a ser derivada al área técnica por límite de edad, ¿Desea continuar?" : null;
                    }
                    else
                    {
                        response.P_MESSAGE = new int[] { 0, 1, 2 }.Contains(resAuth.codAuth) ? resAuth.desAuth : response.P_MESSAGE;
                    }

                }
                //}

            }
            catch (Exception ex)
            {
                response.P_COD_ERR = "1";
                response.P_MESSAGE = ex.Message;
                LogControl.save("tramaDesgravamen", ex.ToString(), "3");
            }

            return await Task.FromResult(response);
        }

        private async Task<SalidaTramaBaseVM> tramaAccPersonales(DataTable dataTable, HttpPostedFile upload, validaTramaVM objValida, ValidaTramaBM validaTramaBM, long maxTrama)
        {
            LogControl.save(objValida.codProceso, "tramaAccPersonales Ini: " + JsonConvert.SerializeObject(objValida, Formatting.None), "2", objValida.codAplicacion);
            objValida.datosPoliza.codUbigeo = !string.IsNullOrEmpty(objValida.datosPoliza.codUbigeo) ? objValida.datosPoliza.codUbigeo : "14";
            //objValida.tipoCotizacion = validarTrx(objValida) ? objValida.tipoCotizacion : "PRIME";
            objValida.datosPoliza.proceso_anterior = (objValida.datosPoliza.trxCode == "EM" && objValida.flagCot == "1") || objValida.flagProcesartrama == 1 ? new QuotationCORE().GetProcessCodePD(objValida.nroCotizacion.ToString()) : objValida.datosPoliza.proceso_anterior;

            var response = new SalidaTramaBaseVM();
            var listaspremium = new SalidaPremiumVM();
            var resAuth = new AuthDerivationVM()
            {
                codAuth = 3,
                desAuth = "Se validó correctamente la trama"
            };

            try
            {
                Int32 count = 0;
                Int64 cantidadTrabajadores = 0;

                var dt = prepareDataTableAPyVG(dataTable, objValida, ref cantidadTrabajadores, ref count);

                if (maxTrama == -1 || count <= maxTrama)
                {
                    // Limpiar información de trama Ecommerce
                    limpiarTrama(objValida);

                    // Guardar asegurados || Método BulkCopy || Iterando
                    response = saveAseguradosPD(dt, objValida, count, response);

                    if (response.P_COD_ERR == "0")
                    {

                        objValida.tipoCotizacion = !string.IsNullOrEmpty(objValida.nroCotizacion.ToString()) ? objValida.nroCotizacion > 0 ? new QuotationCORE().getQuotationType(objValida) : objValida.tipoCotizacion : objValida.tipoCotizacion;

                        if (validarTrx(objValida))
                        {
                            var miniTariff = await getMiniTariffGraph(objValida);

                            if ((miniTariff.data.miniTariffMatrix != null &&
                                miniTariff.data.miniTariffMatrix.segment != null &&
                                miniTariff.data.miniTariffMatrix.segment.modules != null))
                            {
                                if (miniTariff.data.miniTariffMatrix.segment.modules.Count() > 0)
                                {
                                    // Inserta Reglas
                                    insertRulesTrama(miniTariff.data.miniTariffMatrix.segment.rules, objValida);

                                    // Solo ingresa cuando viene de PD
                                    if (objValida.codAplicacion != "EC")
                                    {
                                        if (String.IsNullOrEmpty(objValida.codProcesoOld)) // Proceso Poliza normal
                                        {
                                            insertCoverTrama(miniTariff.data.miniTariffMatrix.segment.modules, objValida.datosPoliza.codTipoPlan, ref objValida);
                                        }
                                        else // Proceso Poliza Matriz
                                        {
                                            getInfoQuotationProcess(ref objValida);
                                        }
                                    }

                                    // Proceso de validaciones
                                    processValidateInsured(objValida, validaTramaBM, ref response);

                                    #region Guardado de asegurados - Cliente360 (variante)
                                    response = response.baseError.errorList.Count == 0 ? await consultaAsegurados(dt, objValida) : response;
                                    #endregion

                                    var aseguradoList = new QuotationCORE().GetTramaAsegurado(objValida.codProceso);

                                    LogControl.save(objValida.codProceso, "GetTramaAsegurado: " + JsonConvert.SerializeObject(aseguradoList, Formatting.None), "2", objValida.codAplicacion);

                                    AgroupTramaAPVG(validaTramaBM, objValida, ref response);

                                    // Poliza Matriz || Inserta TBL_PD_CARGA_TRAMA || Update TBL_PD_CARGA_TRAMA
                                    insertHistorialTramaPM(objValida);

                                    if (response.P_COD_ERR == "0")
                                    {
                                        getPremiumTariff(cantidadTrabajadores, aseguradoList, ref objValida, ref response);
                                    }

                                    response = response.P_COD_ERR == "0" ? processPremiumPD(objValida, miniTariff, "trama", ref response, ref resAuth) : response;

                                    if (response.P_COD_ERR == "0")
                                    {
                                        getInfoListPremium(objValida, resAuth, ref response);

                                        response.cotizadorDetalleList = new QuotationCORE().detailQuotationInfo(aseguradoList, objValida);
                                        response.P_COD_ERR = response.P_COD_ERR == "0" && (new string[] { "IN", "DE", "RE" }).Contains(objValida.datosPoliza.trxCode) ? resAuth.codAuth == 1 ? "2" : response.P_COD_ERR : response.P_COD_ERR;
                                        response.P_QUESTION = response.P_COD_ERR == "2" ? "La transacción va a ser derivada al área técnica por límite de edad, ¿Desea continuar?" : null;
                                    }
                                    else
                                    {
                                        response.P_MESSAGE = new int[] { 0, 1, 2 }.Contains(resAuth.codAuth) ? resAuth.desAuth : response.P_MESSAGE;
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Acá deberia haber un sp que traiga las coberturas / beneficios / asistencias
                            // Para llamar al tarifario con esa data
                            // Si es poliza matriz la data guardada
                            // Si es alguna transac deberia  se la data de la ultima transac
                            getInfoQuotationProcess(ref objValida);
                            //objValida = new QuotationCORE().getInfoQuotationTransac(objValida);

                            // Proceso de validaciones
                            processValidateInsured(objValida, validaTramaBM, ref response);

                            #region Guardado de asegurados - Cliente360 (variante)
                            response = response.baseError.errorList.Count == 0 ? await consultaAsegurados(dt, objValida) : response;
                            #endregion

                            var aseguradoList = !(new string[] { "EX", "EN" }).Contains(objValida.datosPoliza.trxCode) ? new QuotationCORE().GetTramaAsegurado(objValida.codProceso) : null;

                            AgroupTramaAPVG(validaTramaBM, objValida, ref response);

                            // Poliza Matriz || Inserta TBL_PD_CARGA_TRAMA || Update TBL_PD_CARGA_TRAMA
                            insertHistorialTramaPM(objValida);

                            getPremiumTariff(cantidadTrabajadores, aseguradoList, ref objValida, ref response);

                            response = response.P_COD_ERR == "0" ? processPremiumPD(objValida, null, "trama", ref response, ref resAuth) : response;

                            if (response.P_COD_ERR == "0")
                            {
                                getInfoListPremium(objValida, resAuth, ref response);
                                objValida.premium = !(new string[] { "EX", "EN" }).Contains(objValida.datosPoliza.trxCode) ? objValida.premium : response.PRIMA;
                                response.cotizadorDetalleList = new QuotationCORE().detailQuotationInfo(aseguradoList, objValida);
                                response.P_COD_ERR = response.P_COD_ERR == "0" && (new string[] { "IN", "DE", "RE" }).Contains(objValida.datosPoliza.trxCode) ? resAuth.codAuth == 1 ? "2" : response.P_COD_ERR : response.P_COD_ERR;
                                response.P_QUESTION = response.P_COD_ERR == "2" ? "La transacción va a ser derivada al área técnica por límite de edad, ¿Desea continuar?" : null;
                            }
                            else
                            {
                                response.P_MESSAGE = new int[] { 0, 1, 2 }.Contains(resAuth.codAuth) ? resAuth.desAuth : response.P_MESSAGE;
                            }
                        }
                    }
                    else
                    {
                        response.P_COD_ERR = "1";
                        response.P_MESSAGE = "La trama no se pudo guardar correctamente.";
                    }
                }
                else
                {
                    response.P_COD_ERR = "1";
                    response.P_MESSAGE = "La cantidad de asegurados enviada superar el máximo configurado -> " + maxTrama;
                }

            }
            catch (Exception ex)
            {
                response.P_COD_ERR = "1";
                response.P_MESSAGE = ex.Message;
                LogControl.save("tramaAccPersonales", ex.ToString(), "3");
            }

            LogControl.save(objValida.codProceso, "tramaAccPersonales Fin: " + JsonConvert.SerializeObject(response, Formatting.None), "2", objValida.codAplicacion);

            return await Task.FromResult(response);
        }

        private List<CotizadorDetalleVM> detailQuotationInfo(List<insuredGraph> listAsegurados, validaTramaVM objValida)
        {
            var response = new List<CotizadorDetalleVM>();

            if (Generic.valRentaEstudiantilVG(objValida.datosPoliza.branch.ToString(), objValida.datosPoliza.codTipoProducto, objValida.datosPoliza.codTipoPerfil) &&
                listAsegurados != null)
            {
                var filterList = listAsegurados.Where(x => x.role == "Beneficiario").ToList().GroupBy(x => x.salary).ToList();

                foreach (var aseg in filterList)
                {
                    response.Add(new CotizadorDetalleVM
                    {
                        MONT_PLANILLA = aseg.Key.ToString(),
                        PRIMA = objValida.premium.ToString(),
                        PRIMA_UNIT = objValida.asegPremium.ToString(),
                        TASA = objValida.ntasa_tariff.ToString(),
                        TOT_ASEGURADOS = aseg.Count().ToString()
                    });
                }
            }
            else
            {
                response.Add(new CotizadorDetalleVM
                {
                    MONT_PLANILLA = objValida.montoPlanilla.ToString(),
                    PRIMA = objValida.premium.ToString(),
                    PRIMA_UNIT = objValida.asegPremium.ToString(),
                    TASA = objValida.ntasa_tariff.ToString(),
                    TOT_ASEGURADOS = objValida.CantidadTrabajadores.ToString()
                });
            }

            return response;
        }


        private void AgroupTramaAPVG(ValidaTramaBM validaTramaBM, validaTramaVM objValida, ref SalidaTramaBaseVM response)
        {
            if ((response.baseError.errorList.Count == 0) && Generic.valRentaEstudiantilVG(objValida.datosPoliza.branch.ToString(), objValida.datosPoliza.codTipoProducto, objValida.datosPoliza.codTipoPerfil))
            {
                var responseAPGVG = new QuotationCORE().AgrupacionAPVG(validaTramaBM);
                response.P_COD_ERR = responseAPGVG.StatusCode == 0 ? "0" : "1";
            }
        }

        private void getInfoListPremium(validaTramaVM objValida, AuthDerivationVM resAuth, ref SalidaTramaBaseVM response)
        {
            var comisionBroker = response.COMISION_BROKER;
            var comisionInter = response.COMISION_INTER;

            LogControl.save(objValida.codProceso, "getInfoListPremium Ini: " + JsonConvert.SerializeObject(objValida, Formatting.None), "2", objValida.codAplicacion);

            // Proceso de cotizacion
            if (objValida.flagCot == "0")
            {
                response = quotationCORE.ReaListCotiza(objValida.codProceso, objValida.datosPoliza.codTipoRenovacion, objValida.datosPoliza.codTipoProducto, objValida.codRamo);
            }
            else
            {
                //var listaTemp = quotationCORE.ReaListCotiza(objValida.codProceso, objValida.datosPoliza.codTipoRenovacion, objValida.datosPoliza.codTipoProducto, objValida.codRamo);
                var listaspremium = quotationCORE.ReaListDetCotiza(objValida.codProceso, "", objValida.codProducto, objValida.codRamo, objValida.flagCot);
                response.amountPremiumListAnu = listaspremium.amountPremiumAnu;
                response.amountPremiumListProp = listaspremium.amountPremiumProp;
                response.amountPremiumListAut = listaspremium.amountPremiumAut;
                //response.amountPremiumList = listaTemp.amountPremiumList;
                //response.TOT_ASEGURADOS = Convert.ToInt32(listaspremium.TOT_ASEGURADOS);
                response.PRIMA = listaspremium.PRIMA;
                response.PRIMA_ASEG = listaspremium.PRIMA_ASEG;
                response.IGV_ASEG = listaspremium.IGV_ASEG;
                response.PRIMAT_ASEG = listaspremium.PRIMAT_ASEG;
            }

            response.P_MESSAGE = resAuth.desAuth;
            response.TOT_ASEGURADOS = Convert.ToInt32(objValida.CantidadTrabajadores);
            response.MONT_PLANILLA = objValida.montoPlanilla;
            response.COMISION_BROKER = comisionBroker;
            response.COMISION_INTER = comisionInter;

            LogControl.save(objValida.codProceso, "getInfoListPremium Fin: " + JsonConvert.SerializeObject(response, Formatting.None), "2", objValida.codAplicacion);
        }

        private SalidaTramaBaseVM processPremiumPD(validaTramaVM objValida, ResponseGraph miniTariff, string metodo, ref SalidaTramaBaseVM response, ref AuthDerivationVM resAuth)
        {
            LogControl.save(objValida.codProceso, "processPremiumPD Ini: " + metodo + " || " + JsonConvert.SerializeObject(objValida, Formatting.None), "2", objValida.codAplicacion);

            if (!(new string[] { "EX", "EN" }).Contains(objValida.datosPoliza.trxCode))
            {
                if ((objValida.tipoCotizacion == "PRIME" && objValida.premium > 0) || (objValida.tipoCotizacion == "RATE" && objValida.ntasa_tariff > 0))
                {

                    if (!Generic.validarAforo(objValida.datosPoliza.branch.ToString(), objValida.datosPoliza.codTipoPerfil))
                    {
                        if (validarTrx(objValida))
                        {
                            resAuth = quotationCORE.getAuthDerivation(objValida);
                        }
                        else
                        {
                            resAuth = new AuthDerivationVM() { codAuth = 0, desAuth = "Se validó correctamente la trama" };
                        }

                        objValida.premium = new int[] { 1, 2 }.Contains(resAuth.codAuth) ? 0 : objValida.premium;
                        objValida.ntasa_tariff = new int[] { 1, 2 }.Contains(resAuth.codAuth) ? 0 : objValida.ntasa_tariff;

                        objValida.igv = resAuth.codAuth == 0 ? objValida.igv : 0;
                        objValida.premiumTotal = resAuth.codAuth == 0 ? objValida.premiumTotal : 0;
                        objValida.asegPremium = resAuth.codAuth == 0 ? objValida.asegPremium : 0;
                        objValida.asegIgv = resAuth.codAuth == 0 ? objValida.asegIgv : 0;
                        objValida.asegPremiumTotal = resAuth.codAuth == 0 ? objValida.asegPremiumTotal : 0;
                        resAuth = resAuth.codAuth == 0
                                  ? new AuthDerivationVM() { codAuth = 3, desAuth = "Se validó correctamente la trama" }
                                  : resAuth;
                        response = genericProcessPD(objValida, response, 0, miniTariff, metodo).Result;

                        //if (metodo == "calcular")
                        //{
                        //    if (Generic.valRentaEstudiantil(objValida.datosPoliza.branch.ToString(), objValida.datosPoliza.codTipoProducto, objValida.datosPoliza.codTipoPerfil))
                        //    {
                        //        if ((objValida.tipoCotizacion == "PRIME" && objValida.premium == 0) || (objValida.tipoCotizacion == "RATE" && objValida.ntasa_tariff == 0))
                        //        {
                        //            resAuth = quotationCORE.getAuthDerivation(objValida); // Validación de datos de asegurados.
                        //        }
                        //    }
                        //    else
                        //    {
                        //        resAuth = quotationCORE.getAuthDerivation(objValida); // Validación de datos de asegurados.
                        //    }
                        //}

                    }
                    else
                    {
                        var resAforo = new SalidaErrorBaseVM() { P_COD_ERR = 0 };

                        // si tipo de perfil es igual AFORO entra
                        if (quotationCORE.validarAforo(objValida.datosPoliza.branch.ToString(), objValida.datosPoliza.codTipoPerfil, objValida.datosPoliza.trxCode))
                        {
                            resAforo = quotationCORE.InsCargaMasEspecial(objValida);
                        }

                        if (resAforo.P_COD_ERR == 0)
                        {
                            response = genericProcessPD(objValida, response, 0, miniTariff, metodo).Result;

                            if (metodo == "calcular")
                            {
                                resAuth = quotationCORE.getAuthDerivation(objValida); // Validación de datos de asegurados.
                            }
                        }
                    }
                }
                else
                {
                    resAuth = quotationCORE.getAuthDerivation(objValida);

                    if (metodo != "recalcular")
                    {
                        if (resAuth.codAuth == 1)
                        {
                            response = genericProcessPD(objValida, response, 0, miniTariff, metodo).Result;

                            if (metodo == "calcular")
                            {
                                resAuth = quotationCORE.getAuthDerivation(objValida); // Validación de datos de asegurados.
                            }
                            response.P_COD_ERR = objValida.codAplicacion == "EC" ? "1" : response.P_COD_ERR;
                        }
                        else
                        {
                            response.P_COD_ERR = "3";
                            response.P_MESSAGE = metodo == "calcular" ? resAuth.desAuth : response.P_MESSAGE;
                        }
                    }
                    else
                    {
                        if (new int[] { 1, 2 }.Contains(resAuth.codAuth))
                        {
                            response = genericProcessPD(objValida, response, 0, miniTariff, metodo).Result;
                        }
                        else
                        {
                            response.P_COD_ERR = "3";
                            response.P_MESSAGE = metodo == "calcular" ? resAuth.desAuth : response.P_MESSAGE;
                        }
                    }

                }
            }

            LogControl.save(objValida.codProceso, "processPremiumPD Fin response: " + metodo + " || " + JsonConvert.SerializeObject(response, Formatting.None), "2", objValida.codAplicacion);
            LogControl.save(objValida.codProceso, "processPremiumPD Fin resAuth: " + metodo + " || " + JsonConvert.SerializeObject(resAuth, Formatting.None), "2", objValida.codAplicacion);

            return response;
        }

        private void getPremiumTariff(long cantidadTrabajadores, List<insuredGraph> aseguradoList, ref validaTramaVM objValida, ref SalidaTramaBaseVM response)
        {
            if (response.baseError.errorList.Count == 0 && response.baseError.P_COD_ERR == 0 && response.P_COD_ERR == "0")
            {
                response.NIDPROC = objValida.codProceso;
                objValida.flagpremium = objValida.datosPoliza.trxCode == "RE" && Convert.ToBoolean(objValida.datosPoliza.modoEditar) ? 0 : 1;

                // Traer data de Trama
                objValida.ruta = generarAseguradosJson(objValida, aseguradoList);

                if (!(new string[] { "EX", "EN" }).Contains(objValida.datosPoliza.trxCode))
                {
                    // Acá se ha validado todo correctamente - Llamamos a Tarifario AP
                    objValida.CantidadTrabajadores = cantidadTrabajadores;
                    var responseTarifario = getObtenerPrimaTarifario(objValida); // buscar prima

                    if (responseTarifario.codError == 0)
                    {
                        objValida.premium = objValida.tipoCotizacion == "PRIME" ? responseTarifario.data != null ? responseTarifario.data.searchPremium.commercialPremium : 0.0 : 0.0;
                        objValida.ntasa_tariff = objValida.tipoCotizacion == "RATE" ? responseTarifario.data != null ? Convert.ToDouble(decimal.Round(Convert.ToDecimal(responseTarifario.data.searchPremium.netRate * 100), 2)) : 0.0 : 0.0;
                        objValida.url_student = responseTarifario.data != null ? responseTarifario.data.searchPremium.studentCoverages : null;

                        //objValida.premium = Generic.valRentaEstudiantilVG(objValida.datosPoliza.branch.ToString(), objValida.datosPoliza.codTipoProducto, objValida.datosPoliza.codTipoPerfil) ? 1 : responseTarifario.data != null ? responseTarifario.data.searchPremium.commercialPremium : 0.0;

                        #region Simular prima
                        if (ELog.obtainConfig("simularPrimas") == "1")
                        {
                            //if (objValida.premium == 0)
                            //{
                            //var array = new int[] { 0, 12, 6, 3, 2, 1, 1 };
                            //objValida.premium = Convert.ToDouble(decimal.Round(Convert.ToDecimal((10 + ((11 * objValida.lcoberturas.Count) * 0.75314) + (objValida.lasistencias.Count * 5) + (objValida.lbeneficios.Count * 5)) * array[Convert.ToInt32(objValida.datosPoliza.codTipoFrecuenciaPago)]), 2));

                            // objValida.ntasa_tariff = objValida.tipoCotizacion == "RATE" && objValida.ntasa_tariff == 0 ? Convert.ToDouble(decimal.Round(Convert.ToDecimal(((double)(objValida.lcoberturas.Count + objValida.lasistencias.Count + objValida.lbeneficios.Count + objValida.lrecargos.Count + objValida.lservAdicionales.Count) / 100) * 5.55), 2)) : objValida.ntasa_tariff;


                            //responseTarifario.data = null;
                            //}
                        }
                        #endregion

                        if (validarTrx(objValida))
                        {
                            generateValuesPremium(responseTarifario.data, ref objValida);
                        }
                        else
                        {
                            generateValuesPremium(null, ref objValida);
                        }
                    }
                    else
                    {
                        response.P_COD_ERR = responseTarifario.codError.ToString();
                        response.P_MESSAGE = responseTarifario.message;

                        foreach (var item in responseTarifario.errors)
                        {
                            response.baseError.errorList.Add(new ErroresVM
                            {
                                REGISTRO = "0",
                                COD_ERROR = 10,
                                DESCRIPCION = item.message + " " + item.locations
                            });
                        }
                    }
                }
                else
                {
                    response = quotationCORE.InsCotizaTrama(objValida, 0);
                    objValida.CantidadTrabajadores = cantidadTrabajadores;
                }
            }
            else
            {
                response.P_COD_ERR = "1";
                response.P_MESSAGE = !String.IsNullOrEmpty(response.baseError.P_MESSAGE) ? response.baseError.P_MESSAGE : "Error en el proceso";
                response.FLAG_ERROR = response.baseError.errorList.Where(x => x.COD_ERROR == 1).ToList().Count > 0 ? 2 : 1;

            }

        }

        private void generateValuesPremium(GenericResponse dataTariff, ref validaTramaVM objValida)
        {
            LogControl.save(objValida.codProceso, "generateValuesPremium Ini: " + JsonConvert.SerializeObject(objValida, Formatting.None), "2", objValida.codAplicacion);

            if ((objValida.tipoCotizacion == "PRIME" && objValida.premium > 0) || (objValida.tipoCotizacion == "RATE" && objValida.ntasa_tariff > 0))
            {
                objValida.igvPD = new QuotationCORE().GetIGV(objValida) / 100;

                if (objValida.tipoCotizacion == "RATE")
                {
                    if (objValida.flagTrama == "0")
                    {
                        new QuotationCORE().GenerateInfoAmount(objValida.igvPD, null, ref objValida);
                        //pkg_GeneraTransac
                        //objValida.premium = (objValida.montoPlanilla * (objValida.ntasa_tariff * 100)) * objValida.CantidadTrabajadores;
                        //objValida.igv = Convert.ToDouble(decimal.Round(Convert.ToDecimal(objValida.premium * igv), 2));
                        //objValida.premiumTotal = objValida.premium + objValida.igv;
                        //objValida.asegPremium = Convert.ToDouble(decimal.Round(Convert.ToDecimal(objValida.premium / objValida.CantidadTrabajadores), 2));
                        //objValida.asegIgv = Convert.ToDouble(decimal.Round(Convert.ToDecimal(objValida.asegPremium * igv), 2));
                        //objValida.asegPremiumTotal = Convert.ToDouble(decimal.Round(Convert.ToDecimal(objValida.asegPremium + objValida.asegIgv), 2));
                    }
                    else
                    {
                        //if (!validarTrx(objValida))
                        //{
                        new QuotationCORE().GenerateInfoAmount(objValida.igvPD, null, ref objValida);

                        //objValida.igv = Convert.ToDouble(decimal.Round(Convert.ToDecimal(objValida.premium * igv), 2));
                        //objValida.premiumTotal = objValida.premium + objValida.igv;
                        //objValida.asegPremium = Convert.ToDouble(decimal.Round(Convert.ToDecimal(objValida.premium / objValida.CantidadTrabajadores), 2));
                        objValida.asegIgv = -1;
                        //objValida.asegPremiumTotal = Convert.ToDouble(decimal.Round(Convert.ToDecimal(objValida.asegPremium + objValida.asegIgv), 2));
                        //}
                    }
                }
                else
                {
                    if (dataTariff == null)
                    {
                        if (objValida.asegPremiumTotal == 0)
                        {
                            var codProcess = new QuotationCORE().GetProcessCodePD(objValida.nroCotizacion.ToString());
                            var resList = new QuotationCORE().ReaListDetCotiza(codProcess, "", objValida.datosPoliza.codTipoProducto, objValida.datosPoliza.branch.ToString());
                            objValida.asegPremium = resList.PRIMA_ASEG;
                            objValida.asegIgv = resList.IGV_ASEG;
                            objValida.asegPremiumTotal = resList.PRIMAT_ASEG;
                        }

                        new QuotationCORE().GenerateInfoAmount(objValida.igvPD, null, ref objValida);
                        //objValida.igv = Convert.ToDouble(decimal.Round(Convert.ToDecimal(objValida.premium * igv), 2));
                        //objValida.premiumTotal = objValida.premium + objValida.igv;
                        //objValida.asegPremium = Convert.ToDouble(decimal.Round(Convert.ToDecimal(objValida.premium / objValida.CantidadTrabajadores), 2));
                        //objValida.asegIgv = Convert.ToDouble(decimal.Round(Convert.ToDecimal(objValida.asegPremium * igv), 2));
                        //objValida.asegPremiumTotal = Convert.ToDouble(decimal.Round(Convert.ToDecimal(objValida.asegPremium + objValida.asegIgv), 2));
                    }
                    else
                    {
                        //if(Generic.valRentaEstudiantilVG(objValida.datosPoliza.branch.ToString(), objValida.datosPoliza.codTipoProducto, objValida.datosPoliza.codTipoPerfil))
                        //{
                        //    objValida.premiumTotal = dataTariff != null ? dataTariff.searchPremium.totalPremium : 0.0;
                        //    objValida.premium = dataTariff != null ? objValida.premiumTotal / (igv + 1) : 0.0;
                        //    objValida.igv = dataTariff != null ? objValida.premiumTotal - objValida.premium : 0.0;
                        //    objValida.asegPremium = dataTariff != null ? objValida.premium / objValida.CantidadTrabajadores : 0.0;
                        //    objValida.asegIgv = dataTariff != null ? objValida.igv / objValida.CantidadTrabajadores : 0.0;
                        //    objValida.asegPremiumTotal = dataTariff != null ? objValida.premiumTotal / objValida.CantidadTrabajadores : 0.0;
                        //}
                        //else
                        //{

                        objValida.asegPremiumTotal = dataTariff != null ? dataTariff.searchPremium.unitCommercialPremiumWithIgv : 0.0;
                        objValida.premiumTotal = dataTariff != null ? dataTariff.searchPremium.totalPremium : 0.0;
                        new QuotationCORE().GenerateInfoAmount(objValida.igvPD, null, ref objValida);
                        //objValida.igv = dataTariff != null ? dataTariff.searchPremium.totalValueWithIgv : 0.0;
                        //objValida.premiumTotal = dataTariff != null ? dataTariff.searchPremium.totalPremium : 0.0;
                        //objValida.asegPremium = dataTariff != null ? dataTariff.searchPremium.unitCommercialPremium : 0.0;
                        //objValida.asegIgv = dataTariff != null ? dataTariff.searchPremium.igvUnitValue : 0.0;
                        //objValida.asegPremiumTotal = dataTariff != null ? dataTariff.searchPremium.unitCommercialPremiumWithIgv : 0.0;
                        //}
                    }
                }
            }

            LogControl.save(objValida.codProceso, "generateValuesPremium Fin: " + JsonConvert.SerializeObject(objValida, Formatting.None), "2", objValida.codAplicacion);
        }

        private ResponseGraph getObtenerPrimaTarifario(validaTramaVM objValida)
        {
            var responseTarifario = new ResponseGraph();
            LogControl.save(objValida.codProceso, "getObtenerPrimaTarifario Ini: " + JsonConvert.SerializeObject(objValida, Formatting.None), "2", objValida.codAplicacion);
            if (validarTrx(objValida))
            {
                responseTarifario = new QuotationCORE().SendDataTarifarioGraphql(objValida).Result;
            }
            else
            {
                responseTarifario = new QuotationCORE().getObtenerPrimaPD(objValida).Result;
            }
            LogControl.save(objValida.codProceso, "getObtenerPrimaTarifario Fin: " + JsonConvert.SerializeObject(responseTarifario, Formatting.None), "2", objValida.codAplicacion);

            return responseTarifario;
        }

        private List<ErroresVM> processValidateInsured(validaTramaVM objValida, ValidaTramaBM validaTramaBM, ref SalidaTramaBaseVM response)
        {
            var errorList = new List<ErroresVM>();
            if (validaTramaBM != null)
            {
                // Validamos la trama  (Para enviar al servicio de tarifario todo correcto)
                LogControl.save(objValida.codProceso, "processValidateInsured Ini: " + JsonConvert.SerializeObject(objValida, Formatting.None), "2", objValida.codAplicacion);
                response.baseError = quotationCORE.ValidateTramaAccidentesPersonales(validaTramaBM, objValida);
                // Validacion de tipo de documento 
                response.baseError.errorList.AddRange(validarDocumentoPerfil(objValida.datosPoliza.tipoDocumento, objValida.datosPoliza.numDocumento, objValida.datosPoliza.codTipoNegocio, objValida.datosPoliza.codTipoProducto));
                LogControl.save(objValida.codProceso, "processValidateInsured Fin: " + JsonConvert.SerializeObject(response.baseError, Formatting.None), "2", objValida.codAplicacion);
                //response.P_COD_ERR = response.baseError.errorList.Count == 0 ? "0" : "1";
            }
            else
            {
                errorList = validarDocumentoPerfil(objValida.datosPoliza.tipoDocumento, objValida.datosPoliza.numDocumento, objValida.datosPoliza.codTipoNegocio, objValida.datosPoliza.codTipoProducto);
                response.P_COD_ERR = errorList.Count == 0 ? "0" : "1";
            }

            return errorList;


        }

        private void getInfoQuotationProcess(ref validaTramaVM objValida)
        {
            // Acá deberia haber un sp que traiga las coberturas / beneficios / asistencias
            // Para llamar al tarifario con esa data
            // Si es poliza matriz la data guardada
            // Si es alguna transac deberia  se la data de la ultima transac
            objValida = new QuotationCORE().getInfoQuotationTransac(objValida);
            LogControl.save(objValida.codProceso, "getInfoQuotationProcess: " + JsonConvert.SerializeObject(objValida, Formatting.None), "2", objValida.codAplicacion);

            if ((objValida.flagCot == "1" && objValida.datosPoliza.trxCode != "RE") ||
                (objValida.flagCot == "2" && !Generic.validarAforo(objValida.datosPoliza.branch.ToString(), objValida.datosPoliza.codTipoPerfil)) ||
               (objValida.PolizaMatriz && !Generic.validarAforo(objValida.datosPoliza.branch.ToString(), objValida.datosPoliza.codTipoPerfil) && objValida.datosPoliza.trxCode == "RE")) // GCAA 22032024
            {
                LogControl.save(objValida.codProceso, "InsertInfoMatriz: " + JsonConvert.SerializeObject(objValida, Formatting.None), "2", objValida.codAplicacion);
                var res = new QuotationCORE().InsertInfoMatriz(objValida);
            }
        }

        private void insertCoverTrama(List<Module> modules, string moduleId, ref validaTramaVM objValida)
        {
            LogControl.save(objValida.codProceso, "insertCoverTrama Ini: " + JsonConvert.SerializeObject(modules, Formatting.None), "2", objValida.codAplicacion);
            var itemPlan = modules.FirstOrDefault(x => x.id == moduleId);

            if (itemPlan != null)
            {
                //var listCover = itemPlan.coverages.Where(x => Convert.ToBoolean(x.required) == true && x.status == "ENABLED").ToList();

                //foreach (var cover in listCover)
                //{
                //    var coverItem = new coberturaPropuesta()
                //    {
                //        codCobertura = cover.id,
                //        sumaPropuesta = cover.capital.@base
                //    };

                //    objValida.lcoberturas.Add(coverItem);
                //}

                //var responseCover = quotationCORE.insertCoverRequerid(objValida, listCover);

                //var listSubItem = listCover.Where(x => x.items != null && x.items.Count > 0).ToList();
                //var responseSubCover = quotationCORE.insertSubItemPD(objValida, listSubItem);



                var listCover = itemPlan.coverages.Where(x => Convert.ToBoolean(x.required) == true && x.status == "ENABLED").ToList();

                if (listCover != null)
                {
                    foreach (var cover in listCover)
                    {
                        var cobertura = new coberturaPropuesta();
                        cobertura.codCobertura = cover.id;
                        cobertura.descripcion = cover.description;
                        cobertura.suma_asegurada = cover.capital.@base;
                        cobertura.sumaPropuesta = cover.capital.@base;
                        cobertura.capital_max = cover.capital.max;
                        cobertura.capital_min = cover.capital.min;
                        cobertura.capital_aut = cobertura.sumaPropuesta;
                        cobertura.capital = cover.capital.@base;
                        cobertura.cobertura_pri = Convert.ToBoolean(cover.required) ? "1" : "0";

                        //cobertura.cobertura_adi = "0";
                        cobertura.entryAge = cover.entryAge;
                        cobertura.stayAge = cover.stayAge;
                        cobertura.hours = cover.hours;
                        cobertura.capitalCovered = cover.capitalCovered;
                        cobertura.limit = cover.limit;
                        cobertura.items = cover.items;

                        //PRUEBAS AVS - RENTAS
                        cobertura.id_cover = cover.capital.@type;
                        cobertura.sdes_cover = cover.capital.@type == "FIXED_PENSION_QUANTITY" ? "CANT. PENSIONES FIJA" :
                                               cover.capital.@type == "MAXIMUM_PENSION_QUANTITY" ? "PENSIÓN MÁXIMA" :
                                               cover.capital.@type == "FIXED_INSURED_SUM" ? "MONTO FIJO" :
                                               cover.capital.@type == "FIXED_INSURED_SUM_PERCENTAGE" ? "VALOR PORCENTUAL" :
                                               "MONTO FIJO";

                        cobertura.pension_base = new string[] { "FIXED_PENSION_QUANTITY", "MAXIMUM_PENSION_QUANTITY" }.Contains(cover.capital.@type) ? cover.capital.@base : 0;
                        cobertura.pension_max = new string[] { "FIXED_PENSION_QUANTITY", "MAXIMUM_PENSION_QUANTITY" }.Contains(cover.capital.@type) ? cover.capital.max : 0;
                        cobertura.pension_min = new string[] { "FIXED_PENSION_QUANTITY", "MAXIMUM_PENSION_QUANTITY" }.Contains(cover.capital.@type) ? cover.capital.min : 0;
                        cobertura.pension_prop = new string[] { "FIXED_PENSION_QUANTITY", "MAXIMUM_PENSION_QUANTITY" }.Contains(cover.capital.@type) ? cover.capital.@base : 0;

                        cobertura.porcen_base = cover.capital.@type == "FIXED_INSURED_SUM_PERCENTAGE" ? cover.capitalCovered : 0;
                        cobertura.porcen_max = cover.capital.@type == "FIXED_INSURED_SUM_PERCENTAGE" ? cover.capitalCovered : 0;
                        cobertura.porcen_min = cover.capital.@type == "FIXED_INSURED_SUM_PERCENTAGE" ? cover.capitalCovered : 0;
                        cobertura.porcen_prop = cover.capital.@type == "FIXED_INSURED_SUM_PERCENTAGE" ? cover.capitalCovered : 0;

                        cobertura.lackPeriod = cover.lackPeriod;
                        cobertura.deductible = cover.deductible;
                        cobertura.copayment = cover.copayment;
                        cobertura.maxAccumulation = cover.maxAccumulation;
                        cobertura.comment = cover.comment;

                        //var coverFind = coversCot.FirstOrDefault(x => x.codCobertura == cobertura.codCobertura);
                        //cobertura.cobertura_adi = coverFind != null ? "1" : "0";
                        //cobertura.capital_prop = coverFind != null ? coverFind.capital_prop == 0 ? cobertura.suma_asegurada : coverFind.capital_prop : cobertura.suma_asegurada; // cover.capital.@base;
                        //cobertura.capital_aut = coverFind != null ? coverFind.capital_aut : 0;

                        //cobertura.porcen_prop = coverFind != null ? coverFind.porcen_prop : cobertura.porcen_prop;
                        //cobertura.pension_prop = coverFind != null ? coverFind.pension_prop : cobertura.pension_prop;

                        objValida.lcoberturas.Add(cobertura);

                    }

                    var res = quotationCORE.regDetallePlanValidacion(objValida, "coberturas");


                    //foreach (var cover in listCover)
                    //{


                    //    //var coverItem = new coberturaPropuesta()
                    //    //{
                    //    //    codCobertura = cover.id,
                    //    //    sumaPropuesta = cover.capital.@base
                    //    //};

                    //    objValida.lcoberturas.Add(coverItem);

                    //    var res = quotationCORE.regDetallePlanValidacion(objValida, "coberturas");

                    //}
                }

            }

            LogControl.save(objValida.codProceso, "insertCoverTrama Fin: " + JsonConvert.SerializeObject(objValida, Formatting.None), "2", objValida.codAplicacion);
        }

        private void insertRulesTrama(List<Entities.Graphql.Rule> rules, validaTramaVM objValida)
        {
            LogControl.save(objValida.codProceso, "insertRulesTrama Ini: " + JsonConvert.SerializeObject(rules, Formatting.None), "2", objValida.codAplicacion);
            quotationCORE.insertRules(rules, objValida);
            LogControl.save(objValida.codProceso, "insertRulesTrama Fin: " + JsonConvert.SerializeObject(objValida, Formatting.None), "2", objValida.codAplicacion);
        }

        private void insertHistorialTramaPM(validaTramaVM objValida)
        {
            if (objValida.flagCot == "1" && (objValida.datosPoliza.trxCode == "EM" || objValida.datosPoliza.trxCode == "RE"))
            {
                var trama = new ValidaTramaBM()
                {
                    NID_COTIZACION = Convert.ToInt32(objValida.nroCotizacion),
                    NTYPE_TRANSAC = objValida.datosPoliza.trxCode == "EM" ? "1" : "4",
                    NID_PROC = objValida.codProceso
                };
                quotationCORE.insertHistTrama(trama);
            }
        }

        private SalidaTramaBaseVM saveAseguradosPD(DataTable dt, validaTramaVM objValida, int count, SalidaTramaBaseVM response)
        {
            LogControl.save(objValida.codProceso, "saveAseguradosPD Ini: " + JsonConvert.SerializeObject(objValida, Formatting.None), "2", objValida.codAplicacion);

            if (count > 15)
            {
                response = quotationCORE.SaveUsingOracleBulkCopy(dt);
            }
            else
            {
                var listAsegurados = mappingAsegurados(dt);
                response = quotationCORE.InsertAseguradosBulk(listAsegurados, 0);
                response = response.P_COD_ERR == "0" ? quotationCORE.InsertAseguradosBulk(listAsegurados, 1) : response;
            }

            LogControl.save(objValida.codProceso, "saveAseguradosPD Fin: " + JsonConvert.SerializeObject(objValida, Formatting.None), "2", objValida.codAplicacion);

            return response;
        }

        private void limpiarTrama(validaTramaVM objValida)
        {
            //flag cot 1 cuando ingresa trama desde poliza matriz  -- flag cot 2 cuando ingresa trama desde el proceso de renovacion
            if ((new string[] { "1", "2" }).Contains(objValida.flagCot) || objValida.codAplicacion == "EC")
            {
                quotationCORE.LimpiarTrama(objValida.codProceso);
            }
        }

        private DataTable prepareDataTableAPyVG(DataTable dataTable, validaTramaVM objValida, ref long cantidadTrabajadores, ref int countTrama)
        {
            Int32 rowsExcel = Convert.ToInt32(ELog.obtainConfig("rows" + objValida.codRamo));

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
            dt.Columns.Add("TYPE_PLAN");
            dt.Columns.Add("PERCEN_PARTICIPATION");
            dt.Columns.Add("SRELATION");
            dt.Columns.Add("SAPE_CASADA");
            dt.Columns.Add("SLASTNAME2CONCAT");
            dt.Columns.Add("SCLIENT");
            dt.Columns.Add("NCERTIF_CLI");
            dt.Columns.Add("SCREDITNUM");
            dt.Columns.Add("DINIT_CRE");
            dt.Columns.Add("DEND_CRE");
            dt.Columns.Add("NAMOUNT_CRE");
            dt.Columns.Add("NAMOUNT_ACT");
            dt.Columns.Add("NQ_QUOT");
            dt.Columns.Add("NTYPPREMIUM");
            dt.Columns.Add("NPREMIUMN");
            dt.Columns.Add("NIGV");
            dt.Columns.Add("NDE");
            dt.Columns.Add("NPREMIUM");
            dt.Columns.Add("NIDDOC_TYPE_CONT");
            dt.Columns.Add("SIDDOC_CONT");
            dt.Columns.Add("NPHONE_TYPE");
            dt.Columns.Add("SRANGO_EDAD");
            dt.Columns.Add("NGRADO");

            while (rowsExcel < dataTable.Rows.Count)
            {
                if (dataTable.Rows[rowsExcel][6].ToString().Trim() != "" &&
                   dataTable.Rows[rowsExcel][4].ToString().Trim() != "" &&
                   dataTable.Rows[rowsExcel][5].ToString().Trim() != "" &&
                   dataTable.Rows[rowsExcel][2].ToString().Trim() != "" &&
                   dataTable.Rows[rowsExcel][3].ToString().Trim() != ""
                   )
                {
                    countTrama++;
                    DataRow dr = dt.NewRow();
                    dr["NID_COTIZACION"] = objValida.nroCotizacion == 0 ? null : objValida.nroCotizacion;
                    dr["NID_PROC"] = objValida.codProceso;
                    dr["SFIRSTNAME"] = dataTable.Rows[rowsExcel][6].ToString().Trim() == "" ? null : dataTable.Rows[rowsExcel][6].ToString().Trim();
                    dr["SLASTNAME"] = dataTable.Rows[rowsExcel][4].ToString().Trim() == "" ? null : dataTable.Rows[rowsExcel][4].ToString().Trim();
                    dr["SLASTNAME2"] = dataTable.Rows[rowsExcel][5].ToString().Trim() == "" ? null : dataTable.Rows[rowsExcel][5].ToString().Trim();
                    dr["NMODULEC"] = ELog.obtainConfig("riesgoDefaultAP");
                    dr["NNATIONALITY"] = dataTable.Rows[rowsExcel][16].ToString().Trim() == "" ? null : dataTable.Rows[rowsExcel][16].ToString().Trim();
                    dr["NIDDOC_TYPE"] = getDatoTrama(dataTable.Rows[rowsExcel], "tipo_documento", "asegurado");
                    dr["SIDDOC"] = getDatoTrama(dataTable.Rows[rowsExcel], "documento", "asegurado");
                    var fechav = dataTable.Rows[rowsExcel][8].ToString().Trim().Split(' ').ToList();
                    var fnacimientov = fechav.Count() == 1 ? fechav[0] : Convert.ToDateTime(dataTable.Rows[rowsExcel][8].ToString()).ToString("dd/MM/yyyy");
                    string fecha = IsDate(dataTable.Rows[rowsExcel][8].ToString().Trim()) ? fnacimientov.PadLeft(10, '0') : dataTable.Rows[rowsExcel][8].ToString().Trim();
                    dr["DBIRTHDAT"] = dataTable.Rows[rowsExcel][8].ToString().Trim() == "" ? null : fecha;
                    dr["NREMUNERACION"] = dataTable.Rows[rowsExcel][9].ToString().Trim() == "" ? null : dataTable.Rows[rowsExcel][9].ToString().Trim().Replace(",", "");
                    dr["NIDCLIENTLOCATION"] = null;
                    dr["NCOD_NETEO"] = null;
                    dr["NUSERCODE"] = objValida.codUsuario.Trim();
                    dr["SSTATREGT"] = null;
                    dr["SCOMMENT"] = null;
                    dr["NID_REG"] = countTrama.ToString();
                    dr["SSEXCLIEN"] = dataTable.Rows[rowsExcel][7].ToString().Trim() == "" ? null : dataTable.Rows[rowsExcel][7].ToString().Trim();
                    dr["NACTION"] = DBNull.Value;
                    dr["SROLE"] = dataTable.Rows[rowsExcel][1].ToString().Trim() == "" ? null : dataTable.Rows[rowsExcel][1].ToString().Trim();
                    dr["SCIVILSTA"] = null;
                    dr["SSTREET"] = null;
                    dr["SPROVINCE"] = null;
                    dr["SLOCAL"] = null;
                    dr["SMUNICIPALITY"] = null;
                    dr["SE_MAIL"] = dataTable.Rows[rowsExcel][10].ToString().Trim() == "" ? null : dataTable.Rows[rowsExcel][10].ToString().Trim();
                    dr["SPHONE_TYPE"] = dataTable.Rows[rowsExcel][11].ToString().Trim() == "" ? null : "Celular";
                    dr["SPHONE"] = dataTable.Rows[rowsExcel][11].ToString().Trim() == "" ? null : dataTable.Rows[rowsExcel][11].ToString().Trim();
                    dr["NIDDOC_TYPE_BENEFICIARY"] = getDatoTrama(dataTable.Rows[rowsExcel], "tipo_documento", "beneficiario");
                    dr["SIDDOC_BENEFICIARY"] = getDatoTrama(dataTable.Rows[rowsExcel], "documento", "beneficiario");
                    dr["TYPE_PLAN"] = null;
                    dr["PERCEN_PARTICIPATION"] = dataTable.Rows[rowsExcel][14].ToString().Trim() == "" ? null : dataTable.Rows[rowsExcel][14].ToString().Trim();
                    dr["SRELATION"] = dataTable.Rows[rowsExcel][15].ToString().Trim() == "" ? null : dataTable.Rows[rowsExcel][15].ToString().Trim();
                    dr["NIDDOC_TYPE_CONT"] = objValida.datosContratante.desCodDocumento;
                    dr["SIDDOC_CONT"] = objValida.datosContratante.documento;
                    dr["NGRADO"] = Generic.valRentaEstudiantil(objValida.datosPoliza.branch.ToString(), objValida.datosPoliza.codTipoProducto, objValida.datosPoliza.codTipoPerfil) ? String.IsNullOrEmpty(dataTable.Rows[rowsExcel][17].ToString().Trim()) ? null : dataTable.Rows[rowsExcel][17].ToString().Trim() : string.Empty;
                    var role = Generic.valRentaEstudiantilVG(objValida.datosPoliza.branch.ToString(), objValida.datosPoliza.codTipoProducto, objValida.datosPoliza.codTipoPerfil) ? "Beneficiario" : "Asegurado";
                    cantidadTrabajadores = dataTable.Rows[rowsExcel][1].ToString().Trim() == role ? cantidadTrabajadores + 1 : cantidadTrabajadores;
                    rowsExcel++;
                    dt.Rows.Add(dr);
                }
                else
                {
                    break;
                }

            }

            return dt;
        }

        private List<aseguradoMas> mappingAsegurados(DataTable dt)
        {
            var response = new List<aseguradoMas>();

            response = (from DataRow dr in dt.Rows
                        select new aseguradoMas()
                        {
                            nid_cotizacion = dr["NID_COTIZACION"].ToString(),
                            nid_proc = dr["NID_PROC"].ToString(),
                            sfirstname = dr["SFIRSTNAME"].ToString(),
                            slastname = dr["SLASTNAME"].ToString(),
                            slastname2 = dr["SLASTNAME2"].ToString(),
                            nmodulec = dr["NMODULEC"].ToString(),
                            nnationality = dr["NNATIONALITY"].ToString(),
                            niiddoc_type = dr["NIDDOC_TYPE"].ToString(),
                            siddoc = dr["SIDDOC"].ToString(),
                            dbirthdat = dr["DBIRTHDAT"].ToString(),
                            nremuneracion = dr["NREMUNERACION"].ToString(),
                            nidclientlocation = dr["NIDCLIENTLOCATION"].ToString(),
                            ncod_neteo = dr["NCOD_NETEO"].ToString(),
                            nusercode = dr["NUSERCODE"].ToString(),
                            sstatregt = dr["SSTATREGT"].ToString(),
                            scomment = dr["NID_COTIZACION"].ToString(),
                            nid_reg = dr["NID_REG"].ToString(),
                            ssexclien = dr["SSEXCLIEN"].ToString(),
                            naction = null,
                            srole = dr["SROLE"].ToString(),
                            scivilsta = null,
                            sstreet = null,
                            sprovince = null,
                            slocal = null,
                            smunicipality = null,
                            se_mail = dr["SE_MAIL"].ToString(),
                            sphone_type = dr["SPHONE_TYPE"].ToString(),
                            sphone = dr["SPHONE"].ToString(),
                            niddoc_type_beneficiary = dr["NIDDOC_TYPE_BENEFICIARY"].ToString(),
                            siddoc_beneficiary = dr["SIDDOC_BENEFICIARY"].ToString(),
                            type_plan = null,
                            percen_participation = dr["PERCEN_PARTICIPATION"].ToString(),
                            srelation = dr["SRELATION"].ToString(),
                            niddoc_type_cont = dr["NIDDOC_TYPE_CONT"].ToString(),
                            siddoc_cont = dr["SIDDOC_CONT"].ToString(),
                            ngrado = dr["NGRADO"].ToString()
                        }).ToList();

            return response;
        }

        private bool validarTrx(validaTramaVM objValida)
        {
            bool flag = false;

            if ((objValida.datosPoliza.trxCode == "EM" && objValida.flagCot == "0") || ((objValida.datosPoliza.trxCode == "RE" && Convert.ToBoolean(objValida.datosPoliza.modoEditar)) && objValida.flagProcesartrama == 0))
            {
                flag = true;
            }

            return flag;
        }

        //public bool valRentaEstudiantil(string nbranch, string nproduct, string perfil = null)
        //{
        //    bool flag = false;

        //    if (ELog.obtainConfig("vidaGrupoBranch") == nbranch)
        //    {
        //        if (new string[] { ELog.obtainConfig("rentaEstIndiv") }.Contains(nproduct) &&
        //            new string[] { ELog.obtainConfig("perfilEstIndiv"), ELog.obtainConfig("perfilEstGrupal") }.Contains(perfil))
        //        {
        //            flag = true;
        //        }
        //    }


        //    if (ELog.obtainConfig("accidentesBranch") == nbranch)
        //    {
        //        if (new string[] { ELog.obtainConfig("rentaEstIndiv"), ELog.obtainConfig("rentaEstGrupal") }.Contains(nproduct))
        //        {
        //            flag = true;
        //        }
        //    }


        //    return flag;
        //}

        /*public async Task<SalidaTramaBaseVM> genericProcessAP(validaTramaVM objValida, SalidaTramaBaseVM response, int condicion = 0)
        {
            double comisionBroker = 0;
            double comisionInter = 0;

            if (!objValida.datosPoliza.poliza_matriz || objValida.datosPoliza.trxCode != "EM")
            {
                if (objValida.datosPoliza.trxCode == "EM" || (objValida.datosPoliza.trxCode == "RE" && Convert.ToBoolean(objValida.datosPoliza.modoEditar)) ||
                    condicion == 1)
                {
                    if (new string[] { ELog.obtainConfig("vidaIndividualBranch") }.Contains(objValida.datosPoliza.branch.ToString()))
                    {
                        //response.P_COD_ERR = "0";
                        response = new int[] { 0, 2 }.Contains(condicion) ? await InsertRecargos(objValida) : await InsertRecargos(objValida, objValida.lrecargos);
                        comisionBroker = response.P_COD_ERR == "0" ? response.COMISION_BROKER : 0;
                        comisionInter = response.P_COD_ERR == "0" ? response.COMISION_INTER : 0;
                        response = response.P_COD_ERR == "0" ? new int[] { 0, 2 }.Contains(condicion) ? await InsertExclusiones(objValida) : response : response;
                    }
                    else if (new string[] { ELog.obtainConfig("desgravamenBranch") }.Contains(objValida.datosPoliza.branch.ToString()))
                    {
                        response.P_COD_ERR = "0";
                    }
                    else
                    {
                        //response = InsertReajustes(objValida, responseTarifario.data.searchPremium.readjustmentFactors);
                        //response = response.P_COD_ERR == "0" ? await InsertRecargos(objValida) : response;
                        response = new int[] { 0, 2 }.Contains(condicion) ? await InsertRecargos(objValida) : await InsertRecargos(objValida, objValida.lrecargos);
                        comisionBroker = response.P_COD_ERR == "0" ? response.COMISION_BROKER : 0;
                        comisionInter = response.P_COD_ERR == "0" ? response.COMISION_INTER : 0;
                        response = response.P_COD_ERR == "0" ? new int[] { 0, 2 }.Contains(condicion) ? await InsertExclusiones(objValida) : response : response;
                    }

                }

                if (condicion != 2)
                {
                    response = response.P_COD_ERR == "0" ? quotationCORE.DelelteInsuredCot(objValida) : response; // Metododo que hace delete a la trama si existe (TBL_PD_LOAD_INSURED_PREMIUM)
                    response = response.P_COD_ERR == "0" ? quotationCORE.InsertInsuredCot(objValida) : response; // Metodo que inserta valor de prima por asegurado (TBL_PD_LOAD_INSURED_PREMIUM )
                    response = response.P_COD_ERR == "0" ? quotationCORE.InsCotizaTrama(objValida) : response;
                    response.COMISION_BROKER = comisionBroker;
                    response.COMISION_INTER = comisionInter;
                }
                else
                {
                    response = response.P_COD_ERR == "0" ? quotationCORE.InsReCotizaTrama(objValida) : response;
                    response.COMISION_BROKER = comisionBroker;
                    response.COMISION_INTER = comisionInter;
                }

            }
            else
            {
                response = response.P_COD_ERR == "0" ? await InsertRecargos(objValida, objValida.lrecargos) : response;
                comisionBroker = response.P_COD_ERR == "0" ? response.COMISION_BROKER : 0;
                comisionInter = response.P_COD_ERR == "0" ? response.COMISION_INTER : 0;
                //response = quotationCORE.DelelteInsuredCot(request); // Metododo que hace delete a la trama si existe (TBL_PD_LOAD_INSURED_PREMIUM)
                //response = response.P_COD_ERR == "0" ? quotationCORE.InsertInsuredCot(request) : response; // Metodo que inserta valor de prima por asegurado (TBL_PD_LOAD_INSURED_PREMIUM )
                response = response.P_COD_ERR == "0" ? quotationCORE.InsReCotizaTrama(objValida) : response;
            }


            return response;

        }*/

        public async Task<SalidaTramaBaseVM> genericProcessPD(validaTramaVM objValida, SalidaTramaBaseVM response, int condicion = 0, ResponseGraph miniTariff = null, string metodo = "trama")
        {
            // condicion = 0 validacion trama // condicion = 1 recalcular prima // condicion = 2 calcular prima

            if (new string[] { ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch") }.Contains(objValida.datosPoliza.branch.ToString()))
            {
                response = await procesoGenericoApVg(objValida, response, miniTariff, metodo);
            }

            else if (new string[] { ELog.obtainConfig("vidaIndividualBranch") }.Contains(objValida.datosPoliza.branch.ToString()))
            {
                response = await procesoGenericoVC(objValida, response, condicion);
            }

            else if (new string[] { ELog.obtainConfig("desgravamenBranch") }.Contains(objValida.datosPoliza.branch.ToString()))
            {
                response = await procesoGenericoVD(objValida, response, condicion);
            }

            return response;
        }

        private async Task<SalidaTramaBaseVM> procesoGenericoVD(validaTramaVM objValida, SalidaTramaBaseVM response, int condicion)
        {
            double comisionBroker = 0;
            double comisionInter = 0;

            if (!objValida.datosPoliza.poliza_matriz || objValida.datosPoliza.trxCode != "EM")
            {

                if (condicion != 2)
                {
                    response = quotationCORE.DeleteInsuredCot(objValida); // Metododo que hace delete a la trama si existe (TBL_PD_LOAD_INSURED_PREMIUM)
                    response = response.P_COD_ERR == "0" ? quotationCORE.InsertInsuredCot(objValida) : response; // Metodo que inserta valor de prima por asegurado (TBL_PD_LOAD_INSURED_PREMIUM )
                    response = response.P_COD_ERR == "0" ? quotationCORE.InsCotizaTrama(objValida, 0) : response;
                    response.COMISION_BROKER = comisionBroker;
                    response.COMISION_INTER = comisionInter;
                }
                else
                {
                    response.P_COD_ERR = "0"; //ARJG - POLIZA MATRIZ DCD
                    response = response.P_COD_ERR == "0" ? quotationCORE.InsReCotizaTrama(objValida) : response;
                    response.COMISION_BROKER = comisionBroker;
                    response.COMISION_INTER = comisionInter;
                }
            }
            else
            {
                response = response.P_COD_ERR == "0" ? await InsertRecargos(objValida, objValida.lrecargos) : response;
                comisionBroker = response.P_COD_ERR == "0" ? response.COMISION_BROKER : 0;
                comisionInter = response.P_COD_ERR == "0" ? response.COMISION_INTER : 0;
                //response = quotationCORE.DelelteInsuredCot(request); // Metododo que hace delete a la trama si existe (TBL_PD_LOAD_INSURED_PREMIUM)
                //response = response.P_COD_ERR == "0" ? quotationCORE.InsertInsuredCot(request) : response; // Metodo que inserta valor de prima por asegurado (TBL_PD_LOAD_INSURED_PREMIUM )
                response = response.P_COD_ERR == "0" ? quotationCORE.InsReCotizaTrama(objValida) : response;
            }

            return response;
        }

        private async Task<SalidaTramaBaseVM> procesoGenericoVC(validaTramaVM objValida, SalidaTramaBaseVM response, int condicion)
        {
            double comisionBroker = 0;
            double comisionInter = 0;

            if (!objValida.datosPoliza.poliza_matriz || objValida.datosPoliza.trxCode != "EM")
            {
                if (objValida.datosPoliza.trxCode == "EM" || (objValida.datosPoliza.trxCode == "RE" && Convert.ToBoolean(objValida.datosPoliza.modoEditar)) ||
                    condicion == 1)
                {
                    response = new int[] { 0, 2 }.Contains(condicion) ? await InsertRecargos(objValida) : await InsertRecargos(objValida, objValida.lrecargos);
                    comisionBroker = response.P_COD_ERR == "0" ? response.COMISION_BROKER : 0;
                    comisionInter = response.P_COD_ERR == "0" ? response.COMISION_INTER : 0;
                    response = new int[] { 0, 2 }.Contains(condicion) ? await InsertExclusiones(objValida) : response;
                }

                if (condicion != 2)
                {
                    response = response.P_COD_ERR == "0" ? quotationCORE.DeleteInsuredCot(objValida) : response; // Metododo que hace delete a la trama si existe (TBL_PD_LOAD_INSURED_PREMIUM)
                    response = response.P_COD_ERR == "0" ? quotationCORE.InsertInsuredCot(objValida) : response; // Metodo que inserta valor de prima por asegurado (TBL_PD_LOAD_INSURED_PREMIUM )
                    response = response.P_COD_ERR == "0" ? quotationCORE.InsCotizaTrama(objValida, 0) : response;
                    response.COMISION_BROKER = comisionBroker;
                    response.COMISION_INTER = comisionInter;
                }
                else
                {
                    response = response.P_COD_ERR == "0" ? quotationCORE.InsReCotizaTrama(objValida) : response;
                    response.COMISION_BROKER = comisionBroker;
                    response.COMISION_INTER = comisionInter;
                }
            }
            else
            {
                response = response.P_COD_ERR == "0" ? await InsertRecargos(objValida, objValida.lrecargos) : response;
                comisionBroker = response.P_COD_ERR == "0" ? response.COMISION_BROKER : 0;
                comisionInter = response.P_COD_ERR == "0" ? response.COMISION_INTER : 0;
                //response = quotationCORE.DelelteInsuredCot(request); // Metododo que hace delete a la trama si existe (TBL_PD_LOAD_INSURED_PREMIUM)
                //response = response.P_COD_ERR == "0" ? quotationCORE.InsertInsuredCot(request) : response; // Metodo que inserta valor de prima por asegurado (TBL_PD_LOAD_INSURED_PREMIUM )
                response = response.P_COD_ERR == "0" ? quotationCORE.InsReCotizaTrama(objValida) : response;
            }

            return response;
        }

        private async Task<SalidaTramaBaseVM> procesoGenericoApVg(validaTramaVM objValida, SalidaTramaBaseVM response, ResponseGraph miniTariff, string metodo)
        {
            double comisionBroker = 0;
            double comisionInter = 0;

            objValida.flagTrama = objValida.codAplicacion == "EC" ? "1" : objValida.flagTrama;

            LogControl.save(objValida.codProceso, "procesoGenericoApVg objValida: " + metodo + " || " + JsonConvert.SerializeObject(objValida, Formatting.None), "2", objValida.codAplicacion);
            LogControl.save(objValida.codProceso, "procesoGenericoApVg miniTariff: " + metodo + " || " + JsonConvert.SerializeObject(miniTariff, Formatting.None), "2", objValida.codAplicacion);

            //if (!objValida.datosPoliza.poliza_matriz || objValida.datosPoliza.trxCode != "EM")
            if (!objValida.datosPoliza.poliza_matriz || objValida.datosPoliza.trxCode != "EM")
            {
                if (validarTrx(objValida) || metodo == "recalcular")
                {
                    if (new string[] { "trama", "calcular" }.Contains(metodo))
                    {
                        if (miniTariff != null)
                        {
                            var listSurcharge = miniTariff.data.miniTariffMatrix.segment.surcharges.Where(x => Convert.ToBoolean(x.optional) == false && x.status == "ENABLED").ToList();
                            comisionBroker = listSurcharge.FirstOrDefault(x => x.id == "2") != null ? listSurcharge.FirstOrDefault(x => x.id == "2").value.amount : 0;
                            comisionInter = listSurcharge.FirstOrDefault(x => x.id == "3") != null ? listSurcharge.FirstOrDefault(x => x.id == "3").value.amount : 0;
                            quotationCORE.InsertRecargos(objValida, listSurcharge);
                            quotationCORE.InsertExclusiones(objValida, miniTariff.data.miniTariffMatrix.segment.exclusionLinks);
                        }
                    }
                    else
                    {
                        response = await InsertRecargos(objValida, objValida.lrecargos);
                    }
                }

                if (new string[] { "trama", "recalcular" }.Contains(metodo) && objValida.flagTrama == "1")
                {
                    response = response.P_COD_ERR == "0" ? quotationCORE.DeleteInsuredCot(objValida) : response; // Metododo que hace delete a la trama si existe (TBL_PD_LOAD_INSURED_PREMIUM)
                    response = response.P_COD_ERR == "0" ? objValida.tipoCotizacion == "RATE" ? quotationCORE.InsertInsuredCotRentEst(objValida) : quotationCORE.InsertInsuredCot(objValida) : response; // Metodo que inserta valor de prima por asegurado (TBL_PD_LOAD_INSURED_PREMIUM )
                    response = response.P_COD_ERR == "0" ? !string.IsNullOrEmpty(objValida.url_student) ? updateBeneficiariosRE(objValida) : response : response;
                    response = response.P_COD_ERR == "0" ? quotationCORE.InsCotizaTrama(objValida, 0) : response;
                }
                else
                {
                    // generateValuesPremium(null, ref objValida);
                    response = response.P_COD_ERR == "0" ? quotationCORE.InsReCotizaTrama(objValida) : response;
                }

                response.COMISION_BROKER = comisionBroker;
                response.COMISION_INTER = comisionInter;
            }
            else
            {
                response = response.P_COD_ERR == "0" ? await InsertRecargos(objValida, objValida.lrecargos) : response;
                response = response.P_COD_ERR == "0" ? quotationCORE.InsReCotizaTrama(objValida) : response;
            }

            LogControl.save(objValida.codProceso, "procesoGenericoApVg Fin: " + metodo + " || " + JsonConvert.SerializeObject(response, Formatting.None), "2", objValida.codAplicacion);

            return response;
        }

        private SalidaTramaBaseVM updateBeneficiariosRE(validaTramaVM objValida)
        {
            var response = new SalidaTramaBaseVM() { P_COD_ERR = "0" };

            if (Generic.valRentaEstudiantilVG(objValida.datosPoliza.branch.ToString(), objValida.datosPoliza.codTipoProducto, objValida.datosPoliza.codTipoPerfil))
            {
                var asegurados = readJsonAseg(objValida);

                foreach (var aseg in asegurados)
                {
                    foreach (var cover in aseg.coverages)
                    {
                        response = new QuotationCORE().updateCoverageRE(aseg, cover, objValida);
                    }
                }
            }

            return response;
        }

        private List<studentCoverages> readJsonAseg(validaTramaVM objValida)
        {
            var response = new List<studentCoverages>();

            var listOrigen = new string[] { "Tarifario", "Cotizador" };

            foreach (var item in listOrigen)
            {
                var resReference = new GraphqlDA().getReferenceURL(objValida.datosPoliza.branch.ToString(), objValida.url_student, item).Result;

                if (resReference.data != null)
                {
                    if (!String.IsNullOrEmpty(resReference.data.referenceURL))
                    {
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(resReference.data.referenceURL);
                        using (HttpWebResponse data = (HttpWebResponse)request.GetResponse())
                        using (Stream stream = data.GetResponseStream())
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            var json = reader.ReadToEnd();
                            response = JsonConvert.DeserializeObject<List<studentCoverages>>(json);
                        }

                        if (response.Count > 0)
                        {
                            break;
                        }
                    }

                }
            }

            return response;
        }

        private string generarAseguradosJson(validaTramaVM objValida, List<insuredGraph> aseguradoList)
        {
            string path = null;

            if (!quotationCORE.validarAforo(objValida.datosPoliza.branch.ToString(), objValida.datosPoliza.codTipoPerfil, objValida.datosPoliza.trxCode))
            {
                //var role = Generic.valRentaEstudiantilVG(objValida.datosPoliza.branch.ToString(), objValida.datosPoliza.codTipoProducto, objValida.datosPoliza.codTipoPerfil) ? "Beneficiario" : "Asegurado";
                //var asegList = aseguradoList.Where(x => x.role == role).ToList();

                if (aseguradoList != null && aseguradoList.Count() > 0)
                {

                    path = String.Format(ELog.obtainConfig("pathPrincipal"), objValida.codProceso.PadLeft(31, 'A')) + "\\";

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    System.IO.File.WriteAllText(path + "asegurados.json", JsonConvert.SerializeObject(aseguradoList));
                }
            }

            return path;
        }

        //public bool validarAforo(string ramo, string perfil)
        //{
        //    var perfilAforo = ELog.obtainConfig("aforo" + ramo) ?? "6";
        //    var flag = perfilAforo == perfil ? true : false;
        //    return flag;
        //}

        public string getDatoTrama(System.Data.DataRow datoTrama, string tipoDato, string tipoRol)
        {
            string response = null;

            if (!string.IsNullOrEmpty(datoTrama[1].ToString()))
            {
                if (datoTrama[1].ToString().Trim().ToUpper() == "ASEGURADO")
                {
                    if (tipoRol == "asegurado")
                    {
                        if (tipoDato == "tipo_documento")
                        {
                            response = datoTrama[2].ToString().Trim() == "" ? "" : datoTrama[2].ToString().Trim().ToUpper();
                        }

                        if (tipoDato == "documento")
                        {
                            response = datoTrama[3].ToString().Trim() == "" ? null : datoTrama[2].ToString().Trim() == "DNI" ? datoTrama[3].ToString().Trim().Length == 7 ? datoTrama[3].ToString().Trim().PadLeft(8, '0') : datoTrama[3].ToString().Trim() : datoTrama[3].ToString().Trim();
                        }
                    }
                    else
                    {
                        if (tipoDato == "tipo_documento")
                        {
                            response = datoTrama[12].ToString().Trim() == "" ? "" : datoTrama[12].ToString().Trim().ToUpper();
                        }

                        if (tipoDato == "documento")
                        {
                            response = datoTrama[13].ToString().Trim() == "" ? null : datoTrama[12].ToString().Trim().ToUpper() == "DNI" ? datoTrama[13].ToString().Trim().Length == 7 ? datoTrama[13].ToString().Trim().PadLeft(8, '0') : datoTrama[13].ToString().Trim() : datoTrama[13].ToString().Trim();
                        }
                    }
                }
                else
                {
                    if (tipoRol == "asegurado")
                    {
                        if (tipoDato == "tipo_documento")
                        {
                            response = datoTrama[12].ToString().Trim() == "" ? "" : datoTrama[12].ToString().Trim().ToUpper();
                        }

                        if (tipoDato == "documento")
                        {
                            response = datoTrama[13].ToString().Trim() == "" ? null : datoTrama[12].ToString().Trim().ToUpper() == "DNI" ? datoTrama[13].ToString().Trim().Length == 7 ? datoTrama[13].ToString().Trim().PadLeft(8, '0') : datoTrama[13].ToString().Trim() : datoTrama[13].ToString().Trim();
                        }
                    }
                    else
                    {
                        if (tipoDato == "tipo_documento")
                        {
                            response = datoTrama[2].ToString().Trim() == "" ? "" : datoTrama[2].ToString().Trim().ToUpper();
                        }

                        if (tipoDato == "documento")
                        {
                            response = datoTrama[3].ToString().Trim() == "" ? null : datoTrama[2].ToString().Trim().ToUpper() == "DNI" ? datoTrama[3].ToString().Trim().Length == 7 ? datoTrama[3].ToString().Trim().PadLeft(8, '0') : datoTrama[3].ToString().Trim().ToUpper() : datoTrama[3].ToString().Trim();
                        }
                    }
                }
            }
            else
            {
                if (tipoDato == "tipo_documento")
                {
                    response = datoTrama[2].ToString().Trim() == "" ? "" : datoTrama[2].ToString().Trim().ToUpper();
                }

                if (tipoDato == "documento")
                {
                    response = datoTrama[3].ToString().Trim() == "" ? null : datoTrama[2].ToString().Trim().ToUpper() == "DNI" ? datoTrama[3].ToString().Trim().Length == 7 ? datoTrama[3].ToString().Trim().PadLeft(8, '0') : datoTrama[3].ToString().Trim() : datoTrama[3].ToString().Trim();
                }
            }

            return response;
        }

        public string getDatoTramaDesDev(System.Data.DataRow datoTrama, string tipoDato, string tipoRol)
        {
            string response = null;

            if (tipoDato == "documento")
            {
                response = datoTrama[4].ToString().Trim() == "" ? null : datoTrama[3].ToString().Trim() == "2" ? datoTrama[4].ToString().Trim().Length == 7 ? datoTrama[4].ToString().Trim().PadLeft(8, '0') : datoTrama[4].ToString().Trim() : datoTrama[4].ToString().Trim();
            }

            return response;
        }
        //INI MEJORAS VALIDACION VL 
        public async Task<SalidaTramaBaseVM> consultaAsegurados360(DataTable dt, ValidaTramaBM validaTramaBM = null, validaTramaVM objValida = null)
        {
            var response = new SalidaTramaBaseVM();
            var listAsegurados = (from DataRow dr in dt.Rows
                                  select new ConsultaCBM()
                                  {
                                      P_SFIRSTNAME = dr["SFIRSTNAME"].ToString().Trim().ToUpper(),
                                      P_SLASTNAME = dr["SLASTNAME"].ToString().Trim().ToUpper(),
                                      P_SLASTNAME2 = dr["SLASTNAME2"].ToString().Trim().ToUpper(),
                                      P_NIDDOC_TYPE = String.IsNullOrEmpty(dr["NIDDOC_TYPE_BENEFICIARY"].ToString()) ? dr["NIDDOC_TYPE"].ToString() : dr["NIDDOC_TYPE_BENEFICIARY"].ToString(),
                                      P_SIDDOC = String.IsNullOrEmpty(dr["SIDDOC_BENEFICIARY"].ToString()) ? !String.IsNullOrEmpty(dr["SIDDOC"].ToString()) ? dr["SIDDOC"].ToString().Trim().PadLeft(8, '0') : null : dr["SIDDOC_BENEFICIARY"].ToString().Trim().PadLeft(8, '0'),
                                      P_NUSERCODE = dr["NUSERCODE"].ToString(),
                                      P_DBIRTHDAT = dr["DBIRTHDAT"].ToString(),
                                      P_SSEXCLIEN = dr["SSEXCLIEN"].ToString().Trim().ToUpper() == "F" ? "1" : dr["SSEXCLIEN"].ToString().Trim().ToUpper() == "M" ? "2" : "3",
                                      P_SSEXCLIEN_TRAMA = dr["SSEXCLIEN"].ToString().Trim().ToUpper(),
                                      P_SE_MAIL = dr["SE_MAIL"].ToString(),
                                      P_SPHONE = dr["SPHONE"].ToString(),
                                      P_SLEGALNAME = (dr["SLASTNAME"].ToString().Trim().ToUpper() + " " + dr["SLASTNAME2"].ToString().Trim().ToUpper()).Trim() + ", " + dr["SFIRSTNAME"].ToString().Trim().ToUpper(),
                                      P_TipOper = "CON",
                                      P_SMODULEC = dr["NMODULEC"].ToString(),
                                      //P_NRENTAMOUNT = String.IsNullOrEmpty(dr["NREMUNERACION"].ToString()) ? 0 : Convert.ToDouble(dr["NREMUNERACION"].ToString()), -- error de formato JTV
                                      P_NID_PROC = dr["NID_PROC"].ToString()
                                  }).ToList();

            var index = 1;
            if (validaTramaBM == null)
            {

                foreach (var item in listAsegurados)
                {
                    if (!string.IsNullOrEmpty(item.P_NIDDOC_TYPE) && !string.IsNullOrEmpty(item.P_SIDDOC))
                    {
                        var request = quotationCORE.equivalenteTipoDocumento(item);

                        var flagConsulta = quotationCORE.validarConsultaAsegurado(item, request);

                        if (flagConsulta == 1)
                        {
                            var response360 = await GesCliente360(request, "1");
                            //INI MEJORAS VALIDACION VL 
                            response = gestionarInfo(response360, item, index, objValida);
                            //FIN MEJORAS VALIDACION VL 
                        }
                        else
                        {
                            if (item.P_NIDDOC_TYPE.ToUpper() == "DNI")
                            {
                                var response360 = quotationCORE.GesClienteReniecTbl(request);
                                //INI MEJORAS VALIDACION VL 
                                response = gestionarInfo(response360, item, index, objValida);
                                //FIN MEJORAS VALIDACION VL 
                            }
                        }

                        index++;
                    }
                }
            }
            else
            {

                var respExclusion = quotationCORE.updateCargaExclusion(validaTramaBM);
                if (respExclusion.P_COD_ERR == 1)
                {
                    response.baseError.errorList.Add(new ErroresVM() { COD_ERROR = 0, REGISTRO = index.ToString(), DESCRIPCION = respExclusion.P_MESSAGE });
                }
            }

            response.baseError.P_COD_ERR = 0;
            response.P_COD_ERR = "0";

            return response;
        }
        public async Task<SalidaTramaBaseVM> consultaAsegurados(DataTable dt, validaTramaVM objValida)
        {
            var response = new SalidaTramaBaseVM();
            var listAsegurados = (from DataRow dr in dt.Rows
                                  select new ConsultaCBM()
                                  {
                                      P_SFIRSTNAME = dr["SFIRSTNAME"].ToString().Trim().ToUpper(),
                                      P_SLASTNAME = dr["SLASTNAME"].ToString().Trim().ToUpper(),
                                      P_SLASTNAME2 = dr["SLASTNAME2"].ToString().Trim().ToUpper(),
                                      P_NIDDOC_TYPE = String.IsNullOrEmpty(dr["NIDDOC_TYPE_BENEFICIARY"].ToString()) ? dr["NIDDOC_TYPE"].ToString() : dr["NIDDOC_TYPE_BENEFICIARY"].ToString(),
                                      P_SIDDOC = String.IsNullOrEmpty(dr["SIDDOC_BENEFICIARY"].ToString()) ? !String.IsNullOrEmpty(dr["SIDDOC"].ToString()) ? dr["SIDDOC"].ToString().Trim().PadLeft(8, '0') : null : dr["SIDDOC_BENEFICIARY"].ToString().Trim().PadLeft(8, '0'),
                                      P_NUSERCODE = dr["NUSERCODE"].ToString(),
                                      P_DBIRTHDAT = dr["DBIRTHDAT"].ToString(),
                                      P_SSEXCLIEN = dr["SSEXCLIEN"].ToString().Trim().ToUpper() == "F" ? "1" : dr["SSEXCLIEN"].ToString().Trim().ToUpper() == "M" ? "2" : "3",
                                      P_SSEXCLIEN_TRAMA = dr["SSEXCLIEN"].ToString().Trim().ToUpper(),
                                      P_SE_MAIL = dr["SE_MAIL"].ToString(),
                                      P_SPHONE = dr["SPHONE"].ToString(),
                                      P_SLEGALNAME = (dr["SLASTNAME"].ToString().Trim().ToUpper() + " " + dr["SLASTNAME2"].ToString().Trim().ToUpper()).Trim() + ", " + dr["SFIRSTNAME"].ToString().Trim().ToUpper(),
                                      P_TipOper = "CON",
                                      P_SMODULEC = dr["NMODULEC"].ToString(),
                                      //P_NRENTAMOUNT = String.IsNullOrEmpty(dr["NREMUNERACION"].ToString()) ? 0 : Convert.ToDouble(dr["NREMUNERACION"].ToString()), -- error de formato JTV
                                      P_NID_PROC = dr["NID_PROC"].ToString()
                                  }).ToList();

            var index = 1;


            foreach (var item in listAsegurados)
            {
                if (!string.IsNullOrEmpty(item.P_NIDDOC_TYPE) && !string.IsNullOrEmpty(item.P_SIDDOC))
                {
                    var request = quotationCORE.equivalenteTipoDocumento(item);

                    var flagConsulta = quotationCORE.validarConsultaAseguradoPD(item, request);

                    if (flagConsulta == 1)
                    {
                        var response360 = await GesCliente360(request);

                        if (response360.EListClient.Count > 0)
                        {
                            var responseVal = gestionarInfoError(response360, item, index, objValida);

                            if (responseVal != null)
                            {
                                response.baseError.errorList.Add(responseVal);
                            }
                        }
                    }
                    else if (flagConsulta == 0)
                    {
                        if (item.P_NIDDOC_TYPE.ToUpper() == "DNI")
                        {
                            var response360 = quotationCORE.GesClienteReniecTbl(request);

                            if (response360.EListClient.Count > 0)
                            {
                                var responseVal = gestionarInfoError(response360, item, index, objValida);

                                if (responseVal != null)
                                {
                                    response.baseError.errorList.Add(responseVal);
                                }
                            }
                        }
                    }

                    index++;
                }
            }

            response.baseError.P_COD_ERR = 0;
            response.P_COD_ERR = "0";

            return response;
        }

        //INI MEJORAS VALIDACION VL 
        public SalidaTramaBaseVM gestionarInfo(ResponseCVM response360, ConsultaCBM item, int index, validaTramaVM objValida = null)
        {
            var response = new SalidaTramaBaseVM();

            if (response360.P_NCODE == "0")
            {
                var cliente = response360.EListClient.Count > 0 ? response360.EListClient[0] : null;

                if (cliente != null)
                {
                    //if (!String.IsNullOrEmpty(item.P_SFIRSTNAME))
                    //{
                    //    if (item.P_SFIRSTNAME != cliente.P_SFIRSTNAME)
                    //    {
                    //        response.baseError.errorList.Add(gestionErrores360(index, "NOMBRE", item.P_SFIRSTNAME, cliente.P_SFIRSTNAME));
                    //    }
                    //}

                    //if (!String.IsNullOrEmpty(item.P_SLASTNAME))
                    //{
                    //    if (item.P_SLASTNAME != cliente.P_SLASTNAME)
                    //    {
                    //        response.baseError.errorList.Add(gestionErrores360(index, "APELLIDO PATERNO", item.P_SLASTNAME, cliente.P_SLASTNAME));
                    //    }
                    //}

                    //if (!String.IsNullOrEmpty(item.P_SLASTNAME2))
                    //{
                    //    if (item.P_SLASTNAME2 != cliente.P_SLASTNAME2)
                    //    {
                    //        response.baseError.errorList.Add(gestionErrores360(index, "APELLIDO MATERNO", item.P_SLASTNAME2, cliente.P_SLASTNAME2));
                    //    }
                    //}

                    if (String.IsNullOrEmpty(cliente.P_DBIRTHDAT))
                    {
                        cliente.P_DBIRTHDAT = item.P_DBIRTHDAT;
                        //if (item.P_DBIRTHDAT != cliente.P_DBIRTHDAT)
                        //{
                        //        response.baseError.errorList.Add(gestionErrores360(index, "FECHA DE NACIMIENTO", item.P_DBIRTHDAT, cliente.P_DBIRTHDAT));
                        //}
                    }

                    if (!String.IsNullOrEmpty(cliente.P_SSEXCLIEN))
                    {
                        cliente.P_SSEXCLIEN = ELog.obtainConfig("genero" + cliente.P_SSEXCLIEN);
                        //if (item.P_SSEXCLIEN != cliente.P_SSEXCLIEN)
                        //{
                        //        response.baseError.errorList.Add(gestionErrores360(index, "GÉNERO", ELog.obtainConfig("genero" + item.P_SSEXCLIEN), ELog.obtainConfig("genero" + cliente.P_SSEXCLIEN)));
                        //}
                    }
                    else
                    {
                        cliente.P_SSEXCLIEN = item.P_SSEXCLIEN_TRAMA;
                    }

                    if (!String.IsNullOrEmpty(cliente.P_APELLIDO_CASADA))
                    {
                        cliente.P_SLASTNAME2CONCAT = cliente.P_SLASTNAME2 + " " + cliente.P_APELLIDO_CASADA;
                    }
                    //INI MEJORAS VALIDACION VL 
                    var resUpdate = quotationCORE.updateAsegReniec(item, cliente, objValida);
                    //FIN MEJORAS VALIDACION VL 
                    if (resUpdate.P_COD_ERR == 1)
                    {
                        response.baseError.errorList.Add(new ErroresVM() { COD_ERROR = 0, REGISTRO = index.ToString(), DESCRIPCION = resUpdate.P_MESSAGE });
                    }
                }
                else
                {
                    var res360 = quotationCORE.regLogCliente360(item, response360);
                    if (res360.P_COD_ERR == 1)
                    {
                        response.baseError.errorList.Add(new ErroresVM() { COD_ERROR = 0, REGISTRO = index.ToString(), DESCRIPCION = res360.P_MESSAGE });
                    }
                }
            }
            else
            {
                var res360 = quotationCORE.regLogCliente360(item, response360);
                if (res360.P_COD_ERR == 1)
                {
                    response.baseError.errorList.Add(new ErroresVM() { COD_ERROR = 0, REGISTRO = index.ToString(), DESCRIPCION = res360.P_MESSAGE });
                }
            }

            return response;
        }

        public ErroresVM gestionarInfoError(ResponseCVM response360, ConsultaCBM item, int index, validaTramaVM objValida)
        {
            //var branchList = new string[] { ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch") };
            //bool branchFlag = objValida != null ? branchList.Contains(objValida.datosPoliza.branch.ToString()) ? true : false : false;
            var response = new ErroresVM();

            if (response360.P_NCODE == "0")
            {
                var cliente = response360.EListClient.Count > 0 ? response360.EListClient[0] : null;

                if (cliente != null)
                {
                    if (!String.IsNullOrEmpty(cliente.P_DBIRTHDAT))
                    {
                        if (item.P_DBIRTHDAT != cliente.P_DBIRTHDAT)
                        {
                            response = gestionErrores360(index, "FECHA DE NACIMIENTO", item.P_DBIRTHDAT, cliente.P_DBIRTHDAT, item.P_SIDDOC);
                        }
                        else
                        {
                            response = null;
                        }
                    }
                    else
                    {
                        response = null;
                    }

                }
            }

            return response;
        }

        public ErroresVM gestionErrores360(Int32 index, string etiqueta, string campoTrama, string campo360, string documento = null)
        {
            if (!string.IsNullOrEmpty(documento))
            {
                return new ErroresVM()
                {
                    COD_ERROR = 0,
                    REGISTRO = index.ToString(),
                    DESCRIPCION = "El campo " + etiqueta + " no son iguales para el documento " + documento + ", el dato enviado en la trama es " +
                                campoTrama + " y el campo que tenemos registrado es " + campo360
                };
            }
            else
            {
                return new ErroresVM()
                {
                    COD_ERROR = 0,
                    REGISTRO = index.ToString(),
                    DESCRIPCION = "El campo " + etiqueta + " no son iguales, el dato enviado en la trama es " +
                                campoTrama + " y el campo que tenemos registrado es " + campo360
                };
            }

        }

        public SalidaTramaBaseVM InsertReajustes(validaTramaVM objValida, List<ReadjustmentFactor> data)
        {
            var response = new SalidaTramaBaseVM() { P_COD_ERR = "0" };
            if (data != null)
            {
                foreach (var item in data)
                {
                    response = quotationCORE.InsertReajustes(objValida, item);
                }
            }
            return response;
        }

        public async Task<SalidaTramaBaseVM> InsertExclusiones(validaTramaVM objValida, List<ExclusionTariff> data = null)
        {
            var response = new SalidaTramaBaseVM() { P_COD_ERR = "0" };

            if (data == null)
            {
                var requestPlan = new TariffGraph();
                if (new string[] { ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch") }.Contains(objValida.datosPoliza.branch.ToString()))
                {
                    requestPlan = new TariffGraph()
                    {
                        nbranch = objValida.datosPoliza.branch.ToString(),
                        currency = objValida.datosPoliza.codMon,
                        channel = objValida.codCanal,
                        billingType = objValida.datosPoliza.codTipoFacturacion.ToString(),
                        policyType = objValida.datosPoliza.codTipoNegocio,
                        collocationType = objValida.datosPoliza.codTipoModalidad.ToString(),
                        profile = objValida.datosPoliza.codTipoPerfil
                    };

                    requestPlan.profile = quotationCORE.getProfileRepeat(requestPlan.nbranch, requestPlan.profile);

                    var miniTariff = await quotationCORE.getMiniTariffGraph(requestPlan);

                    if (miniTariff.codError == 0)
                    {
                        if (miniTariff.data.miniTariffMatrix != null &&
                           miniTariff.data.miniTariffMatrix.segment != null &&
                           miniTariff.data.miniTariffMatrix.segment.exclusionLinks != null)
                        {
                            if (miniTariff.data.miniTariffMatrix.segment.exclusionLinks.Count() > 0)
                            {
                                //foreach (var item in miniTariff.data.miniTariffMatrix.segment.exclusionLinks)
                                //{
                                response = quotationCORE.InsertExclusiones(objValida, miniTariff.data.miniTariffMatrix.segment.exclusionLinks);
                                //}
                            }
                        }
                    }
                }
                else if (new string[] { ELog.obtainConfig("vidaIndividualBranch") }.Contains(objValida.datosPoliza.branch.ToString()))
                {
                    requestPlan = new TariffGraph()
                    {
                        nbranch = objValida.datosPoliza.branch.ToString(),
                        currency = objValida.datosPoliza.codMon,
                        channel = objValida.codCanal,
                        billingType = objValida.datosPoliza.codTipoFacturacion.ToString(),
                        policyType = objValida.datosPoliza.codTipoNegocio,
                        renewalType = objValida.datosPoliza.renewalType,
                        creditType = objValida.datosPoliza.creditType
                    };

                    requestPlan.profile = quotationCORE.getProfileRepeat(requestPlan.nbranch, requestPlan.profile);

                    var miniTariff = await quotationCORE.getMiniTariffGraphDes(requestPlan);

                    if (miniTariff.codError == 0)
                    {
                        if (miniTariff.data.miniTariffMatrix != null &&
                           miniTariff.data.miniTariffMatrix.segment != null &&
                           miniTariff.data.miniTariffMatrix.segment.exclusionLinks != null)
                        {
                            if (miniTariff.data.miniTariffMatrix.segment.exclusionLinks.Count() > 0)
                            {
                                //foreach (var item in miniTariff.data.miniTariffMatrix.segment.exclusionLinks)
                                //{
                                response = quotationCORE.InsertExclusiones(objValida, miniTariff.data.miniTariffMatrix.segment.exclusionLinks);
                                //}
                            }
                        }
                    }

                }
            }
            else
            {
                //foreach (var item in data)
                //{
                response = quotationCORE.InsertExclusiones(objValida, data);
                //}
            }

            return response;
        }

        public async Task<SalidaTramaBaseVM> InsertRecargos(validaTramaVM objValida, List<recargoPropuesto> data = null, int option = 0)
        {
            var response = new SalidaTramaBaseVM() { P_COD_ERR = "0", COMISION_BROKER = 0 };
            double commisionBroker = 0;
            double commisionInter = 0;
            if (data == null)
            {

                if (new string[] { ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch") }.Contains(objValida.datosPoliza.branch.ToString()))
                {
                    var requestPlan = new TariffGraph()
                    {
                        nbranch = objValida.datosPoliza.branch.ToString(),
                        currency = objValida.datosPoliza.codMon,
                        channel = objValida.codCanal,
                        billingType = objValida.datosPoliza.codTipoFacturacion.ToString(),
                        policyType = objValida.datosPoliza.codTipoNegocio,
                        collocationType = objValida.datosPoliza.codTipoModalidad.ToString(),
                        profile = objValida.datosPoliza.codTipoPerfil
                    };

                    requestPlan.profile = quotationCORE.getProfileRepeat(requestPlan.nbranch, requestPlan.profile);

                    // Traer toda la info del segmento
                    var miniTariff = await quotationCORE.getMiniTariffGraph(requestPlan);

                    if (miniTariff.codError == 0)
                    {
                        if (miniTariff.data.miniTariffMatrix != null &&
                           miniTariff.data.miniTariffMatrix.segment != null)
                        {
                            if (miniTariff.data.miniTariffMatrix.segment.surcharges.Count() > 0)
                            {
                                var listSurcharge = miniTariff.data.miniTariffMatrix.segment.surcharges.Where(x => Convert.ToBoolean(x.optional) == false && x.status == "ENABLED").ToList();
                                var itemBroker = listSurcharge.FirstOrDefault(x => x.id == "2"); // Código de recargo Comisión de broker / corredor
                                var itemInter = listSurcharge.FirstOrDefault(x => x.id == "3"); // Código de recargo Comisión de intermediario / comercializador
                                response = quotationCORE.InsertRecargos(objValida, listSurcharge);
                                response.COMISION_BROKER = itemBroker != null ? itemBroker.value.amount : 0;
                                response.COMISION_INTER = itemInter != null ? itemInter.value.amount : 0;
                            }
                        }
                    }
                }
                else if (new string[] { ELog.obtainConfig("vidaIndividualBranch") }.Contains(objValida.datosPoliza.branch.ToString()))
                {
                    var requestPlan = new TariffGraph()
                    {
                        nbranch = objValida.datosPoliza.branch.ToString(),
                        currency = objValida.datosPoliza.codMon,
                        channel = objValida.codCanal,
                        billingType = objValida.datosPoliza.codTipoFacturacion,
                        policyType = objValida.datosPoliza.codTipoNegocio,
                        renewalType = objValida.datosPoliza.renewalType,
                        creditType = objValida.datosPoliza.creditType
                    };

                    //requestPlan.profile = quotationCORE.getProfileRepeat(requestPlan.nbranch, requestPlan.profile);

                    // Traer toda la info del segmento
                    var miniTariff = await quotationCORE.getMiniTariffGraphDes(requestPlan);

                    if (miniTariff.codError == 0)
                    {
                        if (miniTariff.data.miniTariffMatrix != null &&
                           miniTariff.data.miniTariffMatrix.segment != null)
                        {
                            if (miniTariff.data.miniTariffMatrix.segment.surcharges.Count() > 0)
                            {
                                var listSurcharge = miniTariff.data.miniTariffMatrix.segment.surcharges.Where(x => Convert.ToBoolean(x.optional) == false && x.status == "ENABLED").ToList();


                                response = quotationCORE.InsertRecargos(objValida, listSurcharge);

                                foreach (var item in listSurcharge)
                                {

                                    //if (item.description.ToLower().Contains("corredor")) // Código de recargo Comisión de corredor
                                    if (item.code == "2") // Código de recargo Comisión de broker / corredor
                                    {
                                        commisionBroker = item.value.amount;
                                    }

                                    if (item.code == "3") // Código de recargo Comisión de intermediario
                                    {
                                        commisionInter = item.value.amount;
                                    }
                                }
                                response.COMISION_BROKER = commisionBroker;
                                response.COMISION_INTER = commisionInter;
                            }
                        }
                    }
                }

            }
            else
            {
                var listSurcharge = new List<SurchargeTariff>();
                foreach (var item in data)
                {
                    var itemR = new SurchargeTariff()
                    {
                        code = item.codRecargo,
                        id = item.codRecargo,
                        description = item.desRecargo,
                        value = new Value()
                        {
                            amount = item.amount
                        },
                        optional = option == 0 ? true : Convert.ToBoolean(item.esBeneficio)
                    };

                    listSurcharge.Add(itemR);
                }

                response = quotationCORE.InsertRecargos(objValida, listSurcharge);
            }

            return response;
        }

        private List<ErroresVM> validarDocumentoPerfil(string tipDocumento, string numDocumento, string tipProducto, string tipPerfil)
        {
            var listErr = new List<ErroresVM>();

            if (String.IsNullOrEmpty(tipDocumento) || String.IsNullOrEmpty(numDocumento))
            {
                listErr.Add(new ErroresVM()
                {
                    COD_ERROR = 1,
                    REGISTRO = "0",
                    DESCRIPCION = "Los datos del contratante estan incompletos."
                });
            }
            else
            {
                if (tipProducto == "1") // Individual
                {
                    if ((new string[] { "1", "2", "3", "4" }).Contains(tipPerfil)) // Individual o Familiar
                    {
                        if (tipDocumento == "1" && numDocumento.Substring(0, 2) == "20")
                        {
                            //listErr.Add(new ErroresVM()
                            //{
                            //    COD_ERROR = 1,
                            //    REGISTRO = "0",
                            //    DESCRIPCION = "Para el tipo de perfil enviado, el contratante es incorrecto."
                            //});
                        }
                    }
                }
                else
                {
                    if ((new string[] { "5", "6", "7", "8" }).Contains(tipPerfil))
                    { // Empresas, aforo, viajes
                      //if (tipDocumento == "1")
                      //{
                      //    if (numDocumento.Substring(0, 2) != "20")
                      //    {
                      //        listErr.Add(new ErroresVM()
                      //        {
                      //            COD_ERROR = 1,
                      //            REGISTRO = "0",
                      //            DESCRIPCION = "Para el tipo de perfil enviado, el contratante deberá ser una persona jurídica."
                      //        });
                      //    }
                      //}
                      //else
                        if (tipDocumento != "1")
                        {
                            listErr.Add(new ErroresVM()
                            {
                                COD_ERROR = 1,
                                REGISTRO = "0",
                                DESCRIPCION = "Para el tipo de perfil enviado, el contratante es incorrecto."
                            });
                        }
                    }
                }
            }

            return listErr;
        }

        private async Task<SalidaTramaBaseVM> tramaVidaLey(DataTable dataTable, HttpPostedFile upload, validaTramaVM objValida, ValidaTramaBM validaTramaBM, long maxTrama)
        {
            LogControl.save("tramaVidaLey", JsonConvert.SerializeObject(objValida, Formatting.None), "2");

            var response = new SalidaTramaBaseVM();
            //var flagExc = 0;
            try
            {
                var tramaList = new List<tramaIngresoBM>();
                Regex regex = new Regex("\\s+");

                Int32 count = 1;
                Int32 rows = Convert.ToInt32(ELog.obtainConfig("rows" + objValida.codProducto + objValida.codRamo));
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
                dt.Columns.Add("TYPE_PLAN");
                dt.Columns.Add("PERCEN_PARTICIPATION");
                dt.Columns.Add("SRELATION");
                dt.Columns.Add("SAPE_CASADA");
                dt.Columns.Add("SLASTNAME2CONCAT");
                dt.Columns.Add("SCLIENT");
                dt.Columns.Add("NCERTIF_CLI");
                dt.Columns.Add("SCREDITNUM");
                dt.Columns.Add("DINIT_CRE");
                dt.Columns.Add("DEND_CRE");
                dt.Columns.Add("NAMOUNT_CRE");
                dt.Columns.Add("NAMOUNT_ACT");
                dt.Columns.Add("NQ_QUOT");
                dt.Columns.Add("NTYPPREMIUM");
                dt.Columns.Add("NPREMIUMN");
                dt.Columns.Add("NIGV");
                dt.Columns.Add("NDE");
                dt.Columns.Add("NPREMIUM");
                dt.Columns.Add("NIDDOC_TYPE_CONT");
                dt.Columns.Add("SIDDOC_CONT");
                dt.Columns.Add("NPHONE_TYPE");
                dt.Columns.Add("SRANGO_EDAD");

                while (rows < dataTable.Rows.Count)
                {
                    if (dataTable.Rows[rows][5].ToString().Trim() != "" &&
                        dataTable.Rows[rows][3].ToString().Trim() != "" &&
                        //dataTable.Rows[rows][4].ToString().Trim() != "" &&
                        dataTable.Rows[rows][1].ToString().Trim() != "" &&
                        dataTable.Rows[rows][2].ToString().Trim() != ""
                        )
                    {
                        DataRow dr = dt.NewRow();
                        dr["NID_COTIZACION"] = null;
                        dr["NID_PROC"] = objValida.codProceso;
                        dr["SFIRSTNAME"] = dataTable.Rows[rows][5].ToString().Trim() == "" ? null : regex.Replace(dataTable.Rows[rows][5].ToString().Trim(), " ");
                        dr["SLASTNAME"] = dataTable.Rows[rows][3].ToString().Trim() == "" ? null : regex.Replace(dataTable.Rows[rows][3].ToString().Trim(), " ");
                        dr["SLASTNAME2"] = dataTable.Rows[rows][4].ToString().Trim() == "" ? null : regex.Replace(dataTable.Rows[rows][4].ToString().Trim(), " ");
                        dr["NMODULEC"] = dataTable.Rows[rows][8].ToString().Trim().ToUpper() == "" ? "" : dataTable.Rows[rows][8].ToString().Trim().ToUpper();
                        dr["NNATIONALITY"] = dataTable.Rows[rows][14].ToString().Trim() == "" ? "" : dataTable.Rows[rows][14].ToString().Trim();
                        dr["NIDDOC_TYPE"] = dataTable.Rows[rows][1].ToString().Trim() == "" ? "" : dataTable.Rows[rows][1].ToString().Trim();
                        // dr["SIDDOC"] = dataTable.Rows[rows][2].ToString().Trim() == "" ? null : dataTable.Rows[rows][2].ToString().Trim();
                        dr["SIDDOC"] = dataTable.Rows[rows][2].ToString().Trim() == "" ? null : dataTable.Rows[rows][1].ToString().Trim().ToUpper() == "DNI" && dataTable.Rows[rows][2].ToString().Trim().Length == 7 ? dataTable.Rows[rows][2].ToString().Trim().PadLeft(8, '0') : dataTable.Rows[rows][2].ToString().Trim();
                        var fechav = dataTable.Rows[rows][7].ToString().Trim().Split(' ').ToList();
                        var fnacimientov = fechav.Count() == 1 ? fechav[0] : Convert.ToDateTime(dataTable.Rows[rows][7].ToString()).ToString("dd/MM/yyyy");
                        string fecha = IsDate(dataTable.Rows[rows][7].ToString().Trim()) ? fnacimientov : dataTable.Rows[rows][7].ToString().Trim();
                        dr["DBIRTHDAT"] = dataTable.Rows[rows][7].ToString().Trim() == "" ? objValida.fechaActual?.ToShortDateString() : fecha;
                        dr["NREMUNERACION"] = dataTable.Rows[rows][9].ToString().Trim() == "" ? "0" : dataTable.Rows[rows][9].ToString().Trim();
                        dr["SE_MAIL"] = dataTable.Rows[rows][10].ToString().Trim() == "" ? null : dataTable.Rows[rows][10].ToString().Trim();
                        dr["SPHONE_TYPE"] = dataTable.Rows[rows][11].ToString().Trim() == "" ? null : "Celular";
                        dr["SPHONE"] = dataTable.Rows[rows][11].ToString().Trim() == "" ? null : dataTable.Rows[rows][11].ToString().Trim();
                        dr["NIDCLIENTLOCATION"] = dataTable.Rows[rows][12].ToString().Trim() == "" ? "" : dataTable.Rows[rows][12].ToString().Trim();
                        dr["NCOD_NETEO"] = dataTable.Rows[rows][13].ToString().Trim() == "" ? "" : dataTable.Rows[rows][13].ToString().Trim();
                        dr["NUSERCODE"] = objValida.codUsuario.Trim();
                        dr["SSTATREGT"] = "1";
                        dr["SCOMMENT"] = null;
                        dr["NID_REG"] = count.ToString();
                        dr["SSEXCLIEN"] = dataTable.Rows[rows][6].ToString().Trim() == "" ? null : dataTable.Rows[rows][6].ToString().Trim();
                        dr["NACTION"] = objValida.flagPolizaEmision.HasValue ? (object)objValida.flagPolizaEmision.Value : DBNull.Value;
                        dr["NIDDOC_TYPE_BENEFICIARY"] = null;
                        dr["SIDDOC_BENEFICIARY"] = null;
                        dr["SRANGO_EDAD"] = quotationDA.GetRangoEdad(dataTable.Rows[rows][7].ToString().Trim() == "" ? objValida.fechaActual?.ToShortDateString() : fecha);
                        rows++;
                        count++;
                        dt.Rows.Add(dr);
                    }
                    else
                    {
                        break;
                    }
                }

                if (maxTrama == -1 || count <= maxTrama)
                {
                    response = quotationCORE.SaveUsingOracleBulkCopy(dt); /// AQUI GUARDA DIRECTO EN LA TABLA LA TRAMA GCAA

                    if (response.P_COD_ERR == "0")
                    {
                        //if (validaTramaBM.NTYPE_TRANSAC == "8")
                        //{
                        //    response.baseError = quotationCORE.ValidateTramaEndosoVL(validaTramaBM);
                        //}
                        //else
                        //{
                        //    response.baseError = quotationCORE.ValidateTramaVL(validaTramaBM);
                        //}

                        //response.P_COD_ERR = response.baseError.P_COD_ERR.ToString();
                        //response.P_MESSAGE = response.baseError.P_MESSAGE;
                        //flagExc = response.baseError.P_FLAG_EXC;

                        //INI MEJORAS VALIDACION VL 
                        #region Guardado de asegurados - Cliente360
                        response = validaTramaBM.NTYPE_TRANSAC == "3" ? await consultaAsegurados360(dt, validaTramaBM) : await consultaAsegurados360(dt, objValida: objValida);
                        response.baseError = validaTramaBM.NTYPE_TRANSAC == "8" ? quotationCORE.ValidateTramaEndosoVL(validaTramaBM) : quotationCORE.ValidateTramaVL(validaTramaBM);
                        //FIN MEJORAS VALIDACION VL 


                        #endregion

                        response.P_COD_ERR = response.baseError.P_COD_ERR.ToString();
                        response.P_MESSAGE = response.baseError.P_MESSAGE;
                        //flagExc = response.baseError.P_FLAG_EXC;

                        response.P_FLAG_EXC = response.baseError.P_FLAG_EXC; //recuperando flag de excedente

                        if (objValida.type_mov == "3")
                        {
                            if (response.baseError.errorList.Count == 0 && response.baseError.P_COD_ERR == 0)
                            {
                                objValida.codProceso = objValida.codProceso;
                                quotationCORE.getDetail(objValida, ref response);
                                quotationCORE.getDetailAmount(objValida, ref response);
                            }
                        }
                        else
                        {
                            if (response.baseError.errorList.Count == 0 && response.baseError.P_COD_ERR == 0)
                            {
                                var dr = new tramaIngresoBM();

                                if (validaTramaBM.NTYPE_TRANSAC == "8")
                                {
                                    quotationCORE.readInfoTramaEndoso(validaTramaBM, ref response);
                                    validaTramaBM.SFLAGCOT = objValida.flagCot;
                                    //quotationCORE.readInfoTramaDetailEndoso(validaTramaBM, ref response); // ENDOSO TECNICA JTV 22022023
                                }
                                else
                                {
                                    quotationCORE.readInfoTrama(validaTramaBM, ref response);
                                    validaTramaBM.SFLAGCOT = objValida.flagCot;
                                    quotationCORE.readInfoTramaDetail(validaTramaBM, ref response);
                                    response.P_COD_ERR = "0"; // ENDOSO TECNICA JTV 23022023
                                }

                                if (response.P_COD_ERR == "0") // ENDOSO TECNICA JTV 23022023
                                {
                                    foreach (var item in response.categoryList)
                                    {
                                        item.ProposalRate = 0;
                                    }

                                    int cantEmpleados = 0;
                                    int cantObreros = 0;

                                    float? sumEmpleados = 0;
                                    float? sumObreros = 0;

                                    cantEmpleados = tramaList.Where(x => x.codTipoTrabajador == 1).Count();
                                    cantObreros = tramaList.Where(x => x.codTipoTrabajador == 2).Count();

                                    float? totalPlanilla = 0;

                                    //var numReg = 1;

                                    if (totalPlanilla > objValida.planillaMax)
                                    {
                                        totalPlanilla = objValida.planillaMax;
                                    }

                                    sumEmpleados = tramaList.Where(x => x.codTipoTrabajador == 1).Sum(x => x.sueldoBruto) > objValida.planillaMax ? objValida.planillaMax : tramaList.Where(x => x.codTipoTrabajador == 1).Sum(x => x.sueldoBruto);
                                    sumObreros = tramaList.Where(x => x.codTipoTrabajador == 2).Sum(x => x.sueldoBruto) > objValida.planillaMax ? objValida.planillaMax : tramaList.Where(x => x.codTipoTrabajador == 2).Sum(x => x.sueldoBruto);

                                    TasasVM item1 = new TasasVM();
                                    item1.TIP_RIESGO = "1";
                                    item1.DES_RIESGO = "Empleados";
                                    item1.NUM_TRABAJADORES = cantEmpleados.ToString();
                                    item1.MONTO_PLANILLA = sumEmpleados.ToString();
                                    item1.ID_PRODUCTO = objValida.codProducto;
                                    response.planillaList.Add(item1);

                                    TasasVM item2 = new TasasVM();
                                    item2.TIP_RIESGO = "2";
                                    item2.DES_RIESGO = "Obreros";
                                    item2.NUM_TRABAJADORES = cantObreros.ToString();
                                    item2.MONTO_PLANILLA = sumObreros.ToString();
                                    item2.ID_PRODUCTO = objValida.codProducto;
                                    response.planillaList.Add(item2);

                                    TasasVM item3 = new TasasVM();
                                    item3.TIP_RIESGO = "3";
                                    item3.DES_RIESGO = "Flat";
                                    item3.NUM_TRABAJADORES = tramaList.Count.ToString();
                                    item3.MONTO_PLANILLA = totalPlanilla.ToString();
                                    item3.ID_PRODUCTO = objValida.codProducto;
                                    response.planillaList.Add(item3);
                                }
                                else // INI ENDOSO TECNICA JTV 23022023
                                {
                                    LogControl.save("tramaVidaLey", JsonConvert.SerializeObject(response, Formatting.None), "2");
                                    response.P_MESSAGE = "Hubo un error al validar los valores de la trama. Favor de comunicarse con sistemas";
                                } // FIN ENDOSO TECNICA JTV 23022023
                            }
                        }

                        response.NIDPROC = objValida.codProceso;
                    }
                    else
                    {
                        LogControl.save("tramaVidaLey", JsonConvert.SerializeObject(response, Formatting.None), "2");
                        response.P_MESSAGE = "Error en trama. Favor de comunicarse con sistemas";
                    }
                }
                else
                {
                    response.P_COD_ERR = "1";
                    response.P_MESSAGE = "La cantidad de asegurados enviada superar a máximo configurado -> " + maxTrama;
                }

            }
            catch (Exception ex)
            {
                response.P_COD_ERR = "1";
                response.P_MESSAGE = ex.Message;
                LogControl.save("tramaVidaLey", ex.ToString(), "3");
            }

            return await Task.FromResult(response);
        }

        private Task<SalidaTramaBaseVM> tramaCovid(DataTable dataTable, HttpPostedFile upload, validaTramaVM objValida, ValidaTramaBM validaTramaBM)
        {
            var response = new SalidaTramaBaseVM();
            try
            {
                var tramaAsegurado = new List<AseguradosVM>();

                Int32 count = 1;
                Int32 rows = Convert.ToInt32(ELog.obtainConfig("rows" + objValida.codProducto + objValida.codRamo));

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

                while (rows < dataTable.Rows.Count)
                {
                    var itemAsegurado = new AseguradosVM();
                    DataRow dr = dt.NewRow();
                    dr["NID_COTIZACION"] = null;
                    dr["NID_PROC"] = objValida.codProceso;
                    dr["SFIRSTNAME"] = dataTable.Rows[rows][6].ToString().Trim() == "" ? null : dataTable.Rows[rows][6].ToString().Trim();
                    dr["SLASTNAME"] = dataTable.Rows[rows][4].ToString().Trim() == "" ? null : dataTable.Rows[rows][4].ToString().Trim();
                    dr["SLASTNAME2"] = dataTable.Rows[rows][5].ToString().Trim() == "" ? null : dataTable.Rows[rows][5].ToString().Trim();
                    dr["NMODULEC"] = dataTable.Rows[rows][17].ToString().Trim() == "" ? "" : dataTable.Rows[rows][17].ToString().Trim();
                    itemAsegurado.DES_RIESGO = dataTable.Rows[rows][17].ToString().Trim() == "" ? "" : dataTable.Rows[rows][17].ToString().Trim();
                    itemAsegurado.DES_ROL = dataTable.Rows[rows][3].ToString().Trim() == "" ? null : dataTable.Rows[rows][3].ToString().Trim();
                    dr["NNATIONALITY"] = null;
                    dr["NIDDOC_TYPE"] = dataTable.Rows[rows][1].ToString().Trim() == "" ? "" : dataTable.Rows[rows][1].ToString().Trim();
                    dr["SIDDOC"] = dataTable.Rows[rows][2].ToString().Trim() == "" ? null : dataTable.Rows[rows][2].ToString().Trim();
                    var fechav = dataTable.Rows[rows][7].ToString().Trim().Split(' ').ToList();
                    var fnacimientov = fechav.Count() == 1 ? fechav[0] : Convert.ToDateTime(dataTable.Rows[rows][7].ToString()).ToString("dd/MM/yyyy");
                    string fecha = IsDate(dataTable.Rows[rows][7].ToString().Trim()) ? fnacimientov : dataTable.Rows[rows][7].ToString().Trim();
                    dr["DBIRTHDAT"] = dataTable.Rows[rows][7].ToString().Trim() == "" ? objValida.fechaActual?.ToShortDateString() : fecha;
                    dr["NREMUNERACION"] = null;
                    dr["NIDCLIENTLOCATION"] = null;
                    dr["NCOD_NETEO"] = null;
                    dr["NUSERCODE"] = objValida.codUsuario.Trim();
                    dr["SSTATREGT"] = "1";
                    dr["SCOMMENT"] = null;
                    dr["NID_REG"] = count.ToString();
                    dr["SSEXCLIEN"] = dataTable.Rows[rows][8].ToString().Trim() == "" ? null : dataTable.Rows[rows][8].ToString().Trim();
                    dr["NACTION"] = DBNull.Value; // objValida.flagPolizaEmision.HasValue ? (object)objValida.flagPolizaEmision.Value : DBNull.Value;
                    dr["SROLE"] = dataTable.Rows[rows][3].ToString().Trim() == "" ? null : dataTable.Rows[rows][3].ToString().Trim();
                    dr["SCIVILSTA"] = dataTable.Rows[rows][9].ToString().Trim() == "" ? null : dataTable.Rows[rows][9].ToString().Trim();
                    dr["SSTREET"] = dataTable.Rows[rows][10].ToString().Trim() == "" ? null : dataTable.Rows[rows][10].ToString().Trim();
                    dr["SPROVINCE"] = dataTable.Rows[rows][13].ToString().Trim() == "" ? null : dataTable.Rows[rows][13].ToString().Trim();
                    dr["SLOCAL"] = dataTable.Rows[rows][12].ToString().Trim() == "" ? null : dataTable.Rows[rows][12].ToString().Trim();
                    dr["SMUNICIPALITY"] = dataTable.Rows[rows][11].ToString().Trim() == "" ? null : dataTable.Rows[rows][11].ToString().Trim();
                    dr["SE_MAIL"] = dataTable.Rows[rows][16].ToString().Trim() == "" ? null : dataTable.Rows[rows][16].ToString().Trim();
                    dr["SPHONE_TYPE"] = dataTable.Rows[rows][14].ToString().Trim() == "" ? null : dataTable.Rows[rows][14].ToString().Trim();
                    dr["SPHONE"] = dataTable.Rows[rows][15].ToString().Trim() == "" ? null : dataTable.Rows[rows][15].ToString().Trim();
                    rows++;
                    count++;
                    dt.Rows.Add(dr);
                    tramaAsegurado.Add(itemAsegurado);
                }

                response = quotationCORE.SaveUsingOracleBulkCopy(dt);
                response.baseError = quotationCORE.ValidateTramaCovid(validaTramaBM);

                if (response.P_COD_ERR == "0" && response.baseError.errorList.Count == 0)
                {
                    var groups = tramaAsegurado.GroupBy(u => u.DES_RIESGO).ToList();

                    response.rateInfoList = new List<RateInfoVM>();
                    response.planInfoList = new List<PlanInfoVM>();

                    foreach (var item in groups)
                    {
                        //var item2 = item.Where(x => x.DES_ROL == "Titular").ToList();

                        var rateItem = new RateInfoVM();
                        rateItem.TIP_RIESGO = item.Key;
                        rateItem.DES_RIESGO = item.Key;
                        rateItem.NUM_TRABAJADORES = item.Count();
                        rateItem.TASA_CALC = Convert.ToDouble(objValida.premiumPlan);
                        rateItem.TASA_PRO = objValida.premiumPlanPro == null ? "" : objValida.premiumPlanPro;
                        rateItem.MONTO_PLANILLA = rateItem.NUM_TRABAJADORES * rateItem.TASA_CALC;
                        rateItem.MONTO_PLANILLA_PRO = objValida.premiumPlanPro == null ? "" : (rateItem.NUM_TRABAJADORES * Convert.ToDouble(rateItem.TASA_PRO)).ToString();
                        response.rateInfoList.Add(rateItem);

                        var planItem = new PlanInfoVM();
                        planItem.TIP_RIESGO = item.Key;
                        planItem.DES_RIESGO = item.Key;
                        planItem.NUM_TRABAJADORES = item.Count();
                        planItem.TASA_CALC = Convert.ToDouble(objValida.premiumPlan);
                        planItem.TASA_PRO = objValida.premiumPlanPro == null ? "" : objValida.premiumPlanPro;
                        planItem.NSUM_PREMIUM = rateItem.MONTO_PLANILLA;
                        planItem.NSUM_PREMIUM_PRO = objValida.premiumPlanPro == null ? "" : rateItem.MONTO_PLANILLA_PRO;
                        response.planInfoList.Add(planItem);
                    }

                    response.NIDPROC = objValida.codProceso;
                }
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = "1";
                response.P_MESSAGE = ex.Message;
            }

            return Task.FromResult(response);
        }

        [Route("GetInfoClient")]
        [HttpPost]
        public SalidaTramaBaseVM GetInfoClient(InfoClientBM infoClient)
        {
            SalidaTramaBaseVM response = new SalidaTramaBaseVM();

            try
            {
                response.baseError = quotationCORE.ValidateTramaWeb(infoClient);

                if (response.baseError.errorList.Count == 0)
                {
                    quotationCORE.readInfoTramaWeb(infoClient, ref response);

                    quotationCORE.readInfoTramaDetailWeb(infoClient, ref response);
                }
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = "1";
                response.P_MESSAGE = ex.Message;
                LogControl.save("GetInfoClient", ex.ToString(), "3");
            }

            return response;
        }

        public bool IsDate(string inputDate)
        {
            bool isDate = true;
            string[] formats = { "dd/mm/yyyy", "dd/mm/yyyy HH:mm:ss" };
            //DateTime date;
            try
            {

                //DateTime dateValue;
                Convert.ToDateTime(inputDate).ToShortDateString();
            }
            catch
            {
                isDate = false;
            }
            return isDate;
        }

        //Marcos Silverio
        [Route("GetPlan")]
        [HttpPost]
        public List<PlanVM> GetPlanList(PlanBM data)
        {
            var response = quotationCORE.GetPlansList(data);
            return response;
        }

        [Route("UpdateStatusQuotationVidaLey")]
        [HttpPost]
        public IHttpActionResult UpdateStatusQuotationVL([FromBody] Model.Request.UpdateStatusQuotationRequest request)
        {

            try
            {
                LogControl.save("UpdateStatusQuotationVidaLey", "Step 0: Inicio del metodo", "1");
                LogControl.save("UpdateStatusQuotationVidaLey", JsonConvert.SerializeObject(request, Formatting.None), "2");

                if (request != null)
                {
                    var errors = new List<string>();

                    if (ModelState.IsValid)
                    {
                        LogControl.save("UpdateStatusQuotationVidaLey", Request.RequestUri.AbsoluteUri, "1");
                        LogControl.save("UpdateStatusQuotationVidaLey", "Paso 0.5: Validamos estado de la cotizacion.", "1");

                        if (quotationCORE.ValEstadoCotizacion(request.NroCotizacion))
                        {

                            // GCAA 30012024
                            if (!quotationCORE.ValTransaccionCotizacion(request.NroCotizacion, Convert.ToInt32(request.opcion)))
                            {
                                quotationCORE.InsertLog(Convert.ToInt32(request.NroCotizacion), "Aprobación Devmente", "Api/QuotationManager/UpdateStatusQuotationVidaLey", JsonConvert.SerializeObject(request, Formatting.None), "El Tipo no puede ser cambiado, debido a la transaccion");
                                errors.Add("El Tipo de no puede ser cambiando, debido a la transaccion.");
                                return Ok(new
                                {
                                    success = false,
                                    quotationNumber = request.NroCotizacion,
                                    errors
                                });
                            }
                            else
                            {
                                LogControl.save("UpdateStatusQuotationVidaLey", "Paso 1: Eliminando coberturas que se quedaron en la solicitud anterior.", "1");
                                var resultDetelete = quotationCORE.DeleteQuotationCover(Convert.ToInt32(request.NroCotizacion));
                                LogControl.save("UpdateStatusQuotationVidaLey", "Paso 1: Resultado => " + resultDetelete.MessageError, "1");

                                if (resultDetelete.ErrorCode == 0)
                                {
                                    string userCode = quotationCORE.GetCodeuserByUsername(request.UsuarioAprobador);

                                    if (!string.IsNullOrEmpty(userCode))
                                    {
                                        ThreadStart starter = delegate { UpdateStatusVLAsync(request, userCode); };
                                        Thread threads = new Thread(starter);
                                        threads.Start();

                                        quotationCORE.InsertLog(Convert.ToInt32(request.NroCotizacion), "Aprobación Devmente", "Api/QuotationManager/UpdateStatusQuotationVidaLey", JsonConvert.SerializeObject(request, Formatting.None), "Proceso ha sido recibido");

                                        return Ok(new
                                        {
                                            success = true,
                                            quotationNumber = request.NroCotizacion,
                                            errors = new List<string>(),
                                            errorCode = 0,
                                            messageError = string.Empty
                                        });

                                    }
                                    else
                                    {
                                        quotationCORE.InsertLog(Convert.ToInt32(request.NroCotizacion), "Aprobación Devmente", "Api/QuotationManager/UpdateStatusQuotationVidaLey", JsonConvert.SerializeObject(request, Formatting.None), "No se encontró usuario relacionado en la base de datos.");
                                        errors.Add("No se encontró usuario relacionado en la base de datos.");
                                        return Ok(new
                                        {
                                            success = false,
                                            quotationNumber = request.NroCotizacion,
                                            errors
                                        });
                                    }
                                }
                                else
                                {
                                    errors.Add("No se pudo eliminar las coberturas correctamente.");
                                    return Ok(new
                                    {
                                        success = false,
                                        quotationNumber = request.NroCotizacion,
                                        errors
                                    });
                                }
                            }
                        }
                        else
                        {
                            errors.Add("La cotización ya se encuentra aprobada / rechazada.");
                            return Ok(new
                            {
                                success = true,
                                quotationNumber = request.NroCotizacion,
                                errors
                            });
                        }
                    }
                    else
                    {
                        LogControl.save("UpdateStatusQuotationVidaLey", "Step Error: => " + JsonConvert.SerializeObject(request, Formatting.None) + " || Error Model=> " + JsonConvert.SerializeObject(ModelState.Values, Formatting.None), "3");

                        return Ok(new
                        {
                            success = false,
                            quotationNumber = request.NroCotizacion,
                            errors = ModelState.Values.Select(s => s.Errors.Select(x => x.ErrorMessage).FirstOrDefault())
                        });
                    }
                }
                else
                {
                    throw new ArgumentNullException(nameof(request));
                }
            }
            catch (Exception ex)
            {
                LogControl.save("UpdateStatusQuotationVidaLey", "Error exception: " + ex.ToString(), "1");
                quotationCORE.InsertLog(Convert.ToInt32(request.NroCotizacion), "Aprobación Devmente",
                    "Api/QuotationManager/UpdateStatusQuotationVidaLey", JsonConvert.SerializeObject(request, Formatting.None),
                    "Error exception: " + ex.ToString());

                return Ok(new
                {
                    success = false,
                    messageError = "Error en el servidor."
                });
            }
        }

        public void UpdateStatusVLAsync(Model.Request.UpdateStatusQuotationRequest request, string userCode)
        {
            Task.Run(async () =>
            {
                Task<GenericResponseVM> TBool = updateStatusVL(request, userCode);
            }).GetAwaiter().GetResult();

        }


        public async Task<GenericResponseVM> updateStatusVL(Model.Request.UpdateStatusQuotationRequest request, string userCode)
        {
            var resp = new GenericResponseVM();
            IntegrateToDevmente integrateToDevmente = new IntegrateToDevmente();

            try
            {
                // ARMADO DE OBJETO DEL COTIZADOR - GCAA
                var updateStatusQuotationBM = new UpdateStatusQuotationBM();
                updateStatusQuotationBM.Comentario = request.Comentario;
                updateStatusQuotationBM.Estado = request.Estado;
                updateStatusQuotationBM.FechaAprobacion = DateTime.ParseExact(request.FechaAprobacionStr, "dd/MM/yyyy", null);
                updateStatusQuotationBM.NroCotizacion = request.NroCotizacion;
                updateStatusQuotationBM.PrimaNeta = request.PrimaNeta;
                updateStatusQuotationBM.TasaNeta = request.TasaNeta;
                updateStatusQuotationBM.UsuarioAprobador = userCode;
                updateStatusQuotationBM.PrimaPorCategoriaList = new List<PrimaNetaItem>();
                updateStatusQuotationBM.TasasPorCategoriaList = new List<TasaNetaItem>();
                updateStatusQuotationBM.TipoProceso = request.TipoProceso;
                updateStatusQuotationBM.FechaRetroactividad = request.FechaRetroactividad;
                updateStatusQuotationBM.RemMaxPorCategoriaList = new List<RemuneracionMaximaItem>();
                updateStatusQuotationBM.RemMaxExcedente = request.RemMaxExcedente;
                updateStatusQuotationBM.EdadMaxExcedente = request.EdadMaxExcedente;
                updateStatusQuotationBM.TipoTasa = request.TipoTasa;
                updateStatusQuotationBM.Comision = request.Comision;
                updateStatusQuotationBM.opcion = request.opcion; // GCAA 24012024

                // GCAA 25012024 - PARA EL TIPO 1
                updateStatusQuotationBM.tasasPlanillasPrimasRangoEdad = new List<TasasPlanillasPrimasRangoEdadItem>(); // GCAA 24012024
                updateStatusQuotationBM.tasasPlanillasPrimasRangoEdadHastaTopeRemuneraciones = new List<TasasPlanillasPrimasRangoEdadHastaTopeRemuneracioneItem>(); // GCAA 24012024
                updateStatusQuotationBM.tasasPlanillasPrimasRangoEdadExcesoTopeRemuneraciones = new List<TasasPlanillasPrimasRangoEdadExcesoTopeRemuneracioneItem>(); // GCAA 24012024

                // GCAA 25012024 - PARA EL TIPO 2
                updateStatusQuotationBM.tasasPlanillasPrimasCategoriaTrabajador = new List<TasasPlanillasPrimasCategoriaTrabajadorItem>(); // GCAA 24012024
                updateStatusQuotationBM.tasasPlanillasPrimasCategoriaTrabajadorHastaTopeRemuneraciones = new List<TasasPlanillasPrimasCategoriaTrabajadorHastaTopeRemuneracioneItem>(); // GCAA 24012024
                updateStatusQuotationBM.tasasPlanillasPrimasCategoriaTrabajadorExcesoTopeRemuneraciones = new List<TasasPlanillasPrimasCategoriaTrabajadorExcesoTopeRemuneracioneItem>(); // GCAA 24012024

                // GCAA 25012024 - PARA EL TIPO 3
                updateStatusQuotationBM.tasasPlanillasPrimasRangoEdadCodigoCategoria = new List<TasasPlanillasPrimasRangoEdadCodigoCategoriaItem>(); // GCAA 24012024
                updateStatusQuotationBM.tasasPlanillasPrimasCodigoCategoriaHastaTopeRemuneraciones = new List<TasasPlanillasPrimasCodigoCategoriaHastaTopeRemuneracioneItem>(); // GCAA 24012024
                updateStatusQuotationBM.tasasPlanillasPrimasCodigoCategoriaExcesoTopeRemuneraciones = new List<TasasPlanillasPrimasCodigoCategoriaExcesoTopeRemuneracioneItem>(); // GCAA 24012024


                #region TIPO 4
                if (request.TasasPorCategoriaListHastaTope != null)
                {
                    foreach (var item in request.TasasPorCategoriaListHastaTope)
                    {
                        updateStatusQuotationBM.TasasPorCategoriaList.Add(new TasaNetaItem { CodigoCategoria = item.CodigoCategoria, TasaNeta = item.TasaNeta });
                    }
                }

                if (request.TasasPorCategoriaListExcesoTope != null)
                {
                    foreach (var item in request.TasasPorCategoriaListExcesoTope)
                    {
                        updateStatusQuotationBM.TasasPorCategoriaList.Add(new TasaNetaItem { CodigoCategoria = item.CodigoCategoria, TasaNeta = item.TasaNeta });
                    }
                }

                if (request.PrimaPorCategoriaListHastaTope != null)
                {
                    foreach (var item in request.PrimaPorCategoriaListHastaTope)
                    {
                        updateStatusQuotationBM.PrimaPorCategoriaList.Add(new PrimaNetaItem { CodigoCategoria = item.CodigoCategoria, PrimaNeta = item.PrimaNeta });
                    }
                }

                if (request.PrimaPorCategoriaListExcesoTope != null)
                {
                    foreach (var item in request.PrimaPorCategoriaListExcesoTope)
                    {
                        updateStatusQuotationBM.PrimaPorCategoriaList.Add(new PrimaNetaItem { CodigoCategoria = item.CodigoCategoria, PrimaNeta = item.PrimaNeta });
                    }
                }

                if (request.RemPorCategoriaListHastaTope != null)
                {
                    foreach (var item in request.RemPorCategoriaListHastaTope)
                    {
                        updateStatusQuotationBM.RemMaxPorCategoriaList.Add(new RemuneracionMaximaItem { CodigoCategoria = item.CodigoCategoria, RemuneracionMaxima = item.Remuneracion });
                    }
                }

                if (request.RemPorCategoriaListExcesoTope != null)
                {
                    foreach (var item in request.RemPorCategoriaListExcesoTope)
                    {
                        updateStatusQuotationBM.RemMaxPorCategoriaList.Add(new RemuneracionMaximaItem { CodigoCategoria = item.CodigoCategoria, RemuneracionMaxima = item.Remuneracion });
                    }
                }

                var dtCovers = this.GetDataTableCoverApproved(request, Convert.ToInt32(updateStatusQuotationBM.UsuarioAprobador));
                #endregion

                //if (request.opcion == "1" || request.opcion == "3")
                //{
                //    var rangotipo1 = request.tasasPlanillasPrimasRangoEdad
                //           .Select(item => item.rango_edad)
                //           .Distinct();

                //    var rangotipo3 = request.tasasPlanillasPrimasRangoEdadCodigoCategoria
                //               .Select(item => item.rango_edad)
                //               .Distinct();
                //}

                new QuotationDA().update_table_peso(Convert.ToInt32(request.NroCotizacion));
                #region TIPO 1
                if (request.opcion == "1")
                {

                    //if (request.tasasPlanillasPrimasRangoEdad != null)
                    //{
                    //    foreach (var item in request.tasasPlanillasPrimasRangoEdad)
                    //    {
                    //        List<CoberturasTasasComercialeItem> coberturas = new List<CoberturasTasasComercialeItem>();
                    //        TasasPlanillasPrimasRangoEdadItem obj = new TasasPlanillasPrimasRangoEdadItem();
                    //        if (item.coberturasTasasComerciales != null)
                    //        {
                    //            foreach (var ite in item.coberturasTasasComerciales.ToList())
                    //            {
                    //                CoberturasTasasComercialeItem cobe = new CoberturasTasasComercialeItem();
                    //                cobe.idCobertura = ite.idCobertura;
                    //                cobe.tasaComercial = ite.tasaComercial;
                    //                coberturas.Add(cobe);                                  
                    //                // SE ENVIA TIPO EN ELA DESCRIPCION PARA QUE EL SP TENGA OTRO COMPORTAMIENTO
                    //                new PolicyDA().Actualizar_proceso_cobertura(Convert.ToInt32(request.NroCotizacion), Convert.ToInt32(ite.idCobertura),ite.tasaComercial, 0,0,"TIPO 1","COT 1",item.rango_edad);                                    
                    //            }
                    //        }

                    //        obj.rango_edad = item.rango_edad;
                    //        obj.descripcion = item.descripcion;
                    //        obj.valor = item.valor;
                    //        obj.coberturasTasasComerciales = coberturas ?? null;

                    //        updateStatusQuotationBM.tasasPlanillasPrimasRangoEdad.Add(obj);
                    //    }
                    //}

                    if (request.tasasPlanillasPrimasRangoEdadHastaTopeRemuneraciones != null)
                    {
                        foreach (var item in request.tasasPlanillasPrimasRangoEdadHastaTopeRemuneraciones)
                        {
                            List<CoberturasTasasComercialeItem> coberturas = new List<CoberturasTasasComercialeItem>();
                            TasasPlanillasPrimasRangoEdadHastaTopeRemuneracioneItem obj = new TasasPlanillasPrimasRangoEdadHastaTopeRemuneracioneItem();
                            if (item.coberturasTasasComerciales != null)
                            {
                                foreach (var ite in item.coberturasTasasComerciales.ToList())
                                {
                                    CoberturasTasasComercialeItem cobe = new CoberturasTasasComercialeItem();
                                    cobe.idCobertura = ite.idCobertura;
                                    cobe.tasaComercial = ite.tasaComercial;
                                    coberturas.Add(cobe);
                                    // SE ENVIA TIPO EN ELA DESCRIPCION PARA QUE EL SP TENGA OTRO COMPORTAMIENTO
                                    new PolicyDA().Actualizar_proceso_cobertura(Convert.ToInt32(request.NroCotizacion), Convert.ToInt32(ite.idCobertura), ite.tasaComercial, 0, 0, "TIPO1", "COT", item.rango_edad);
                                }
                            }

                            obj.rango_edad = item.rango_edad;
                            obj.descripcion = item.descripcion;
                            obj.valor = item.valor;
                            obj.coberturasTasasComerciales = coberturas ?? null;
                            updateStatusQuotationBM.tasasPlanillasPrimasRangoEdadHastaTopeRemuneraciones.Add(obj);
                        }
                    }

                    if (request.tasasPlanillasPrimasRangoEdadExcesoTopeRemuneraciones != null)
                    {
                        foreach (var item in request.tasasPlanillasPrimasRangoEdadExcesoTopeRemuneraciones)
                        {
                            List<CoberturasTasasComercialeItem> coberturas = new List<CoberturasTasasComercialeItem>();
                            TasasPlanillasPrimasRangoEdadExcesoTopeRemuneracioneItem obj = new TasasPlanillasPrimasRangoEdadExcesoTopeRemuneracioneItem();
                            if (item.coberturasTasasComerciales != null)
                            {
                                foreach (var ite in item.coberturasTasasComerciales.ToList())
                                {
                                    CoberturasTasasComercialeItem cobe = new CoberturasTasasComercialeItem();
                                    cobe.idCobertura = ite.idCobertura;
                                    cobe.tasaComercial = ite.tasaComercial;
                                    coberturas.Add(cobe);
                                    // SE ENVIA TIPO EN ELA DESCRIPCION PARA QUE EL SP TENGA OTRO COMPORTAMIENTO
                                    new PolicyDA().Actualizar_proceso_cobertura(Convert.ToInt32(request.NroCotizacion), Convert.ToInt32(ite.idCobertura), ite.tasaComercial, 0, 0, "TIPO1", "COT", item.rango_edad);
                                }
                            }
                            obj.rango_edad = item.rango_edad;
                            obj.descripcion = item.descripcion;
                            obj.valor = item.valor;
                            obj.coberturasTasasComerciales = coberturas ?? null;
                            updateStatusQuotationBM.tasasPlanillasPrimasRangoEdadExcesoTopeRemuneraciones.Add(obj);
                        }
                    }
                }
                #endregion

                #region TIPO 2
                if (request.opcion == "2")
                {
                    //if (request.tasasPlanillasPrimasCategoriaTrabajador != null)
                    //{
                    //    foreach (var item in request.tasasPlanillasPrimasCategoriaTrabajador)
                    //    {
                    //        List<CoberturasTasasComercialeItem> coberturas = new List<CoberturasTasasComercialeItem>();
                    //        TasasPlanillasPrimasCategoriaTrabajadorItem obj = new TasasPlanillasPrimasCategoriaTrabajadorItem();

                    //        if (item.coberturasTasasComerciales != null)
                    //        {
                    //            foreach (var ite in item.coberturasTasasComerciales.ToList())
                    //            {
                    //                CoberturasTasasComercialeItem cobe = new CoberturasTasasComercialeItem();
                    //                cobe.idCobertura = ite.idCobertura;
                    //                cobe.tasaComercial = ite.tasaComercial;
                    //                coberturas.Add(cobe);

                    //                new PolicyDA().Actualizar_proceso_cobertura(Convert.ToInt32(request.NroCotizacion), Convert.ToInt32(ite.idCobertura), ite.tasaComercial, item.codigoCategoria, 0, item.descripcion, "COT","");
                    //            }
                    //        }

                    //        obj.codigoCategoria = item.codigoCategoria.ToString();
                    //        obj.descripcion = item.descripcion;
                    //        obj.valor = item.valor;
                    //        obj.coberturasTasasComerciales = coberturas ?? null;
                    //        updateStatusQuotationBM.tasasPlanillasPrimasCategoriaTrabajador.Add(obj);
                    //    }
                    //}

                    if (request.tasasPlanillasPrimasCategoriaTrabajadorHastaTopeRemuneraciones != null)
                    {
                        foreach (var item in request.tasasPlanillasPrimasCategoriaTrabajadorHastaTopeRemuneraciones)
                        {
                            List<CoberturasTasasComercialeItem> coberturas = new List<CoberturasTasasComercialeItem>();
                            TasasPlanillasPrimasCategoriaTrabajadorHastaTopeRemuneracioneItem obj = new TasasPlanillasPrimasCategoriaTrabajadorHastaTopeRemuneracioneItem();

                            if (item.coberturasTasasComerciales != null)
                            {
                                foreach (var ite in item.coberturasTasasComerciales.ToList())
                                {
                                    CoberturasTasasComercialeItem cobe = new CoberturasTasasComercialeItem();
                                    cobe.idCobertura = ite.idCobertura;
                                    cobe.tasaComercial = ite.tasaComercial;
                                    coberturas.Add(cobe);
                                    new PolicyDA().Actualizar_proceso_cobertura(Convert.ToInt32(request.NroCotizacion), Convert.ToInt32(ite.idCobertura), ite.tasaComercial, item.codigoCategoria, 0, item.descripcion, "COT", "");
                                }
                            }

                            obj.codigoCategoria = item.codigoCategoria.ToString();
                            obj.descripcion = item.descripcion;
                            obj.valor = item.valor;
                            obj.coberturasTasasComerciales = coberturas ?? null;
                            updateStatusQuotationBM.tasasPlanillasPrimasCategoriaTrabajadorHastaTopeRemuneraciones.Add(obj);
                        }
                    }

                    if (request.tasasPlanillasPrimasCategoriaTrabajadorExcesoTopeRemuneraciones != null)
                    {
                        foreach (var item in request.tasasPlanillasPrimasCategoriaTrabajadorExcesoTopeRemuneraciones)
                        {
                            List<CoberturasTasasComercialeItem> coberturas = new List<CoberturasTasasComercialeItem>();
                            TasasPlanillasPrimasCategoriaTrabajadorExcesoTopeRemuneracioneItem obj = new TasasPlanillasPrimasCategoriaTrabajadorExcesoTopeRemuneracioneItem();

                            if (item.coberturasTasasComerciales != null)
                            {
                                foreach (var ite in item.coberturasTasasComerciales.ToList())
                                {
                                    CoberturasTasasComercialeItem cobe = new CoberturasTasasComercialeItem();
                                    cobe.idCobertura = ite.idCobertura;
                                    cobe.tasaComercial = ite.tasaComercial;
                                    coberturas.Add(cobe);
                                    new PolicyDA().Actualizar_proceso_cobertura(Convert.ToInt32(request.NroCotizacion), Convert.ToInt32(ite.idCobertura), ite.tasaComercial, item.codigoCategoria, 0, item.descripcion, "COT", "");
                                }
                            }

                            obj.codigoCategoria = item.codigoCategoria.ToString();
                            obj.descripcion = item.descripcion;
                            obj.valor = item.valor;
                            obj.coberturasTasasComerciales = coberturas ?? null;
                            updateStatusQuotationBM.tasasPlanillasPrimasCategoriaTrabajadorExcesoTopeRemuneraciones.Add(obj);
                        }
                    }
                }

                #endregion

                #region TIPO 3
                if (request.opcion == "3")
                {
                    //if (request.tasasPlanillasPrimasRangoEdadCodigoCategoria != null)
                    //{
                    //    foreach (var item in request.tasasPlanillasPrimasRangoEdadCodigoCategoria)
                    //    {
                    //        List<CoberturasTasasComercialeItem> coberturas = new List<CoberturasTasasComercialeItem>();
                    //        TasasPlanillasPrimasRangoEdadCodigoCategoriaItem obj = new TasasPlanillasPrimasRangoEdadCodigoCategoriaItem();

                    //        if (item.coberturasTasasComerciales != null)
                    //        {
                    //            foreach (var ite in item.coberturasTasasComerciales.ToList())
                    //            {
                    //                CoberturasTasasComercialeItem cobe = new CoberturasTasasComercialeItem();
                    //                cobe.idCobertura = ite.idCobertura;
                    //                cobe.tasaComercial = ite.tasaComercial;
                    //                coberturas.Add(cobe);
                    //                new PolicyDA().Actualizar_proceso_cobertura(Convert.ToInt32(request.NroCotizacion), Convert.ToInt32(ite.idCobertura), ite.tasaComercial, item.codigoCategoria, 0, item.descripcion, "COT", item.rango_edad);
                    //            }
                    //        }

                    //        obj.codigoCategoria = item.codigoCategoria;
                    //        obj.rango_edad = item.rango_edad;
                    //        obj.descripcion = item.descripcion;
                    //        obj.valor = item.valor;
                    //        obj.coberturasTasasComerciales = coberturas ??  null;    
                    //        updateStatusQuotationBM.tasasPlanillasPrimasRangoEdadCodigoCategoria.Add(obj);
                    //    }
                    //}

                    if (request.tasasPlanillasPrimasCodigoCategoriaHastaTopeRemuneraciones != null)
                    {
                        foreach (var item in request.tasasPlanillasPrimasCodigoCategoriaHastaTopeRemuneraciones)
                        {
                            List<CoberturasTasasComercialeItem> coberturas = new List<CoberturasTasasComercialeItem>();
                            TasasPlanillasPrimasCodigoCategoriaHastaTopeRemuneracioneItem obj = new TasasPlanillasPrimasCodigoCategoriaHastaTopeRemuneracioneItem();

                            if (item.coberturasTasasComerciales != null)
                            {
                                foreach (var ite in item.coberturasTasasComerciales.ToList())
                                {
                                    CoberturasTasasComercialeItem cobe = new CoberturasTasasComercialeItem();
                                    cobe.idCobertura = ite.idCobertura;
                                    cobe.tasaComercial = ite.tasaComercial;
                                    coberturas.Add(cobe);
                                    new PolicyDA().Actualizar_proceso_cobertura(Convert.ToInt32(request.NroCotizacion), Convert.ToInt32(ite.idCobertura), ite.tasaComercial, item.codigoCategoria, 0, item.descripcion, "COT", item.rango_edad);
                                }
                            }

                            obj.codigoCategoria = item.codigoCategoria.ToString();
                            obj.rango_edad = item.rango_edad;
                            obj.descripcion = item.descripcion;
                            obj.valor = item.valor;
                            obj.coberturasTasasComerciales = coberturas ?? null;
                            updateStatusQuotationBM.tasasPlanillasPrimasCodigoCategoriaHastaTopeRemuneraciones.Add(obj);
                        }
                    }

                    if (request.tasasPlanillasPrimasCodigoCategoriaExcesoTopeRemuneraciones != null)
                    {
                        foreach (var item in request.tasasPlanillasPrimasCodigoCategoriaExcesoTopeRemuneraciones)
                        {
                            List<CoberturasTasasComercialeItem> coberturas = new List<CoberturasTasasComercialeItem>();
                            TasasPlanillasPrimasCodigoCategoriaExcesoTopeRemuneracioneItem obj = new TasasPlanillasPrimasCodigoCategoriaExcesoTopeRemuneracioneItem();

                            if (item.coberturasTasasComerciales != null)
                            {
                                foreach (var ite in item.coberturasTasasComerciales.ToList())
                                {
                                    CoberturasTasasComercialeItem cobe = new CoberturasTasasComercialeItem();
                                    cobe.idCobertura = ite.idCobertura;
                                    cobe.tasaComercial = ite.tasaComercial;
                                    coberturas.Add(cobe);
                                    new PolicyDA().Actualizar_proceso_cobertura(Convert.ToInt32(request.NroCotizacion), Convert.ToInt32(ite.idCobertura), ite.tasaComercial, item.codigoCategoria, 0, item.descripcion, "COT", item.rango_edad);
                                }
                            }

                            obj.codigoCategoria = item.codigoCategoria.ToString();
                            obj.rango_edad = item.rango_edad;
                            obj.descripcion = item.descripcion;
                            obj.valor = item.valor;
                            obj.coberturasTasasComerciales = coberturas ?? null;
                            updateStatusQuotationBM.tasasPlanillasPrimasCodigoCategoriaExcesoTopeRemuneraciones.Add(obj);
                        }
                    }
                }

                #endregion


                resp = quotationCORE.UpdateStatusQuotationVidaLey(updateStatusQuotationBM, dtCovers);
                resp.ErrorMessageList = resp.ErrorMessageList.Where(m => m != "null").ToList();

                if (resp.GenericFlag == 1)
                {
                    await integrateToDevmente.Execute(resp.Transaction, Convert.ToInt32(request.NroCotizacion));
                }

                LogControl.save("UpdateStatusQuotationVidaLey", "Error exception: Paso Final: Respuesta => " + Environment.NewLine + JsonConvert.SerializeObject(resp, Formatting.None), "1");

                quotationCORE.InsertLog(Convert.ToInt32(request.NroCotizacion), "Aprobación Devmente",
                    "Api/QuotationManager/UpdateStatusQuotationVidaLey", JsonConvert.SerializeObject(request, Formatting.None),
                    resp.ErrorCode == 0 ? "El proceso se completó correctamente" : "Error: " + String.Join("|", resp.ErrorMessageList));


                await integrateToDevmente.ExecuteUpdate(request, resp.ErrorCode);


            }
            catch (Exception ex)
            {
                LogControl.save("UpdateStatusQuotationVidaLey", "Error: Respuesta => " + Environment.NewLine + ex.ToString(), "1");
                quotationCORE.InsertLog(Convert.ToInt32(request.NroCotizacion), "Aprobación Devmente",
                    "Api/QuotationManager/UpdateStatusQuotationVidaLey", JsonConvert.SerializeObject(request, Formatting.None),
                    "Error: " + ex.Message);
            }

            return await Task.FromResult(resp);
        }

        private DataTable GetDataTableCoverApproved(Model.Request.UpdateStatusQuotationRequest request, int usercode)
        {
            DataTable dt = new DataTable();

            if (request.Coberturas != null || request.Coberturas.Any())
            {
                if (request.Coberturas.Count > 0)
                {
                    dt.Columns.Add("NID_COTIZACION", System.Type.GetType("System.Int32"));
                    dt.Columns.Add("NMOVEMENT", System.Type.GetType("System.Int32"));
                    dt.Columns.Add("NCODPLAN", System.Type.GetType("System.Int32"));
                    dt.Columns.Add("NCOVER", System.Type.GetType("System.Int32"));
                    dt.Columns.Add("NCAPITAL", System.Type.GetType("System.Decimal"));
                    dt.Columns.Add("NCAPMAXIM", System.Type.GetType("System.Decimal"));
                    dt.Columns.Add("NAGEMININSM", System.Type.GetType("System.Decimal"));
                    dt.Columns.Add("NAGEMAXINSM", System.Type.GetType("System.Decimal"));
                    dt.Columns.Add("NAGEMAXPERM", System.Type.GetType("System.Decimal"));

                    dt.Columns.Add("DEFFECDATE", System.Type.GetType("System.DateTime"));
                    dt.Columns.Add("DEXPIRDAT", System.Type.GetType("System.DateTime"));
                    dt.Columns.Add("DNULLDATE", System.Type.GetType("System.DateTime"));
                    dt.Columns.Add("DCOMPDATE", System.Type.GetType("System.DateTime"));
                    dt.Columns.Add("NUSERCODE", System.Type.GetType("System.Int32"));

                    dt.Columns.Add("NSTATUS_COVER", System.Type.GetType("System.Int32"));

                    foreach (var item in request.Coberturas)
                    {
                        dt.Rows.Add(request.NroCotizacion,
                                    null, //item.NroOrden,
                                    item.CodigoPlan,
                                    item.CodigoCobertura,
                                    item.SumaAsegurada,
                                    item.TopeSumaAsegurada,
                                    item.TopeMinimoEdad,
                                    item.TopeMaximoEdad,
                                    item.TopeMaximoEdadPermanencia,
                                    request.FechaAprobacion,
                                    null,
                                    null,
                                    null,
                                    usercode, //request.UsuarioAprobador,
                                    0);
                    }
                }
                else
                {
                    dt = null;
                }
            }
            else
            {
                dt = null;
            }

            return dt;
        }

        [Route("GetPlansList")]
        [HttpPost]
        public List<PlanVM> GetPlansList(PlanBM data)
        {
            return quotationCORE.GetPlansList(data);
        }

        [Route("GetQuotationCoverByNumQuotation")]
        [HttpGet]
        public List<QuotationCoverVM> GetQuotationCoverByNumQuotation(string numCotizacion)
        {
            var data = quotationCORE.GetQuotationCoverByNumQuotation(int.Parse(numCotizacion));
            return data;
        }

        [Route("ValidateRetroactivity")]
        [HttpPost]
        public async Task<QuotationResponseVM> ValidateRetroactivity()
        {
            var response = new QuotationResponseVM();

            try
            {
                LogControl.save("ValidateRetroactivity", HttpContext.Current.Request.Params.Get("objeto"), "2");
                var cotizacion = JsonConvert.DeserializeObject<QuotationCabBM>(HttpContext.Current.Request.Params.Get("objeto"));
                response = quotationCORE.ValidateRetroactivity(cotizacion);

                if (response.P_SAPROBADO == "A" && cotizacion.RetOP == 2)
                {
                    // Envio de notificacion al cotizador
                    var lRamosN = new string[] { ELog.obtainConfig("vidaLeyBranch") };
                    // Envio de notificacion al cotizador Graphql
                    var lRamosGraphql = new string[] { ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch") };

                    // Enviar notificacion
                    if (lRamosN.Contains(cotizacion.P_NBRANCH.ToString()))
                    {
                        var responseSendNotificartionDevmente = await SendDataQuotationToDevmente(cotizacion.NumeroCotizacion.ToString(), cotizacion.NumeroCotizacion, cotizacion.TrxCode, 1);
                        if (responseSendNotificartionDevmente.ErrorCode == 1)
                        {
                            response.P_SMESSAGE = response.P_SMESSAGE + Environment.NewLine + responseSendNotificartionDevmente.MessageError;
                        }
                    }

                    // Enviar notificacion
                    if (lRamosGraphql.Contains(cotizacion.P_NBRANCH.ToString()))
                    {
                        var responseGraphql = await quotationCORE.SendDataQuotationGraphql(cotizacion.NumeroCotizacion.ToString(), cotizacion.TrxCode, 0, null);
                        if (responseGraphql.codError == 1)
                        {
                            response.P_SMESSAGE = response.P_SMESSAGE + Environment.NewLine + responseGraphql.message;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = ex.ToString();
            }
            return response;
        }

        [Route("GetPlansListAcc")]
        [HttpPost]
        public List<PlanVM> GetPlansListAcc(PlanBM data)
        {
            return quotationCORE.GetPlansListAcc(data);
        }

        [Route("GetExcelQuotationList")]
        [HttpPost]
        public string GetExcelQuotationList(QuotationSearchBM dataToSearch)
        {
            return quotationCORE.GetExcelQuotationList(dataToSearch);
        }
        [Route("GetFechaFin")]
        [HttpGet]
        public GenericResponseVM getFechaFin(string fecha, int freq)
        {
            return quotationCORE.getFechaFin(fecha, freq);
        }

        [Route("ReSendQuotation")]
        [HttpPost]
        public async Task<GenericResponseVM> ReSendQuotation(QuotationCabBM data)
        {
            IntegrateToDevmente integrateToDevmente = new IntegrateToDevmente();
            GenericResponseVM response = new GenericResponseVM();
            response = await integrateToDevmente.Execute(data.TrxCode, data.NumeroCotizacion);
            return response;
        }

        [Route("GetTipoTarifario")]
        [HttpPost]
        public async Task<List<TipoTarifarioVM>> GetTipoTarifario(TariffGraph data)
        {
            var response = new List<TipoTarifarioVM>();

            var profilebase = data.profile;
            data.profile = quotationCORE.getProfileRepeat(data.nbranch, data.profile);

            if (new string[] { ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch") }.Contains(data.nbranch.ToString()))
            {
                var tariff = await quotationCORE.getTariffGraph(data);

                if (tariff.codError == 0)
                {
                    if (tariff.data.tariffQuery != null)
                    {
                        if (data.application != "EC")
                        {
                            if (tariff.data.tariffQuery.Count > 0)
                            {
                                var lista = tariff.data.tariffQuery.OrderBy(x => x.id);

                                foreach (var item in lista)
                                {
                                    var tarifario = new TipoTarifarioVM();
                                    tarifario.idTariff = item.id;
                                    tarifario.desTariff = item.description;
                                    tarifario.versionTariff = item.version;
                                    tarifario.validarTrama = !Generic.validarAforo(data.nbranch, profilebase) ? item.dataFrame ? "1" : "0" : "1";
                                    tarifario.tipoCotizacion = Generic.valRentaEstudiantilVG(data.nbranch, profilebase, profilebase) ? item.dataFrame ? item.quotationType : "RATE" : "PRIME";
                                    response.Add(tarifario);
                                }
                            }

                        }
                        else
                        {
                            if (tariff.data.tariffQuery.Count > 0)
                            {
                                var tariffPrime = Generic.valRentaEstudiantil(data.nbranch, profilebase, profilebase) ? tariff.data.tariffQuery.FirstOrDefault(x => x.quotationType == "PRIME" && x.dataFrame == true) : tariff.data.tariffQuery[0];

                                if (tariffPrime != null)
                                {
                                    var tarifario = new TipoTarifarioVM();
                                    tarifario.idTariff = tariffPrime.id;
                                    tarifario.desTariff = tariffPrime.description;
                                    tarifario.versionTariff = tariffPrime.version;
                                    tarifario.validarTrama = "1";
                                    tarifario.tipoCotizacion = "PRIME";
                                    response.Add(tarifario);
                                }
                            }
                        }
                    }
                }
            }


            return await Task.FromResult(response);
        }


        [Route("GetTypePlan")]
        [HttpPost]
        public async Task<ResponsePlan> GetTipoPlan(TariffGraph data)
        {
            var response = new ResponsePlan();

            int faults = 0;
            int age_contractor = 0;
            double age_fault_max = 0;
            double age_fault_min = 0;
            DateTime fechaActual = DateTime.Now;

            try
            {
                data.profile = quotationCORE.getProfileRepeat(data.nbranch, data.profile);

                if (new string[] { ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch") }.Contains(data.nbranch.ToString()))
                {
                    var miniTariff = await quotationCORE.getMiniTariffGraph(data);

                    if (miniTariff.codError == 0)
                    {
                        if (miniTariff.data.miniTariffMatrix != null &&
                            miniTariff.data.miniTariffMatrix.segment != null &&
                            miniTariff.data.miniTariffMatrix.segment.modules != null)
                        {
                            if (miniTariff.data.miniTariffMatrix.segment.modules.Count > 0)
                            {
                                var modules = data.npensiones == "1" ? miniTariff.data.miniTariffMatrix.segment.modules.Where(x => x.planType == "PENSION_QUANTITY").ToList() : miniTariff.data.miniTariffMatrix.segment.modules;

                                foreach (var item in modules)
                                {
                                    var plan = new TipoPlanVM();
                                    plan.ID_PLAN = Convert.ToInt32(item.id);
                                    plan.TIPO_PLAN = item.description;
                                    response.planList.Add(plan);
                                }
                            }

                            if (miniTariff.data.miniTariffMatrix.segment.rules != null)
                            {
                                response.rulesList = await quotationCORE.getRulesLogic(miniTariff.data.miniTariffMatrix.segment.rules);
                            }
                        }
                    }
                }
                else if (new string[] { ELog.obtainConfig("vidaIndividualBranch") }.Contains(data.nbranch.ToString()))
                {
                    var miniTariff = await quotationCORE.getMiniTariffGraphDes(data);

                    if (miniTariff.codError == 0)
                    {
                        if (miniTariff.data.miniTariffMatrix != null &&
                            miniTariff.data.miniTariffMatrix.segment != null &&
                            miniTariff.data.miniTariffMatrix.segment.modules != null)
                        {
                            if (miniTariff.data.miniTariffMatrix.segment.modules.Count > 0)
                            {
                                foreach (var item in miniTariff.data.miniTariffMatrix.segment.modules)
                                {
                                    var plan = new TipoPlanVM();
                                    plan.ID_PLAN = Convert.ToInt32(item.id);
                                    plan.TIPO_PLAN = item.description;
                                    response.planList.Add(plan);
                                }
                            }

                            if (miniTariff.data.miniTariffMatrix.segment.rules != null)
                            {
                                response.rulesList = await quotationCORE.getRulesLogicDes(miniTariff.data.miniTariffMatrix.segment.rules);
                            }
                            response.id_tarifario = miniTariff.data.miniTariffMatrix.id;
                            response.version_tarifario = miniTariff.data.miniTariffMatrix.version;
                            response.name_tarifario = miniTariff.data.miniTariffMatrix.description;

                            if (data.birthday_contractor != "")
                            {

                                DateTime fechaNacimiento = DateTime.Parse(data.birthday_contractor);

                                age_contractor = fechaActual.Year - fechaNacimiento.Year;

                                if (fechaActual.Month < fechaNacimiento.Month || (fechaActual.Month == fechaNacimiento.Month && fechaActual.Day > fechaNacimiento.Day))
                                {
                                    age_contractor--;
                                }

                                foreach (var coverage in miniTariff.data.miniTariffMatrix.segment.modules[0].coverages)
                                {
                                    var age_min = coverage.entryAge.min;
                                    var age_max = coverage.entryAge.max;


                                    if ((age_contractor < age_min) || (age_contractor > age_max))
                                    {

                                        age_fault_min = age_min;
                                        age_fault_max = age_max;
                                        faults++;
                                    }

                                }

                                if (faults > 0)
                                {
                                    response.codError = "1";
                                    response.sMessage = "El asegurado no cumple con la edad mínima/máxima permitida (de " + age_fault_min + " a " + age_fault_max + ").";
                                    return response;
                                }
                            }

                        }
                    }
                }

                response.codError = response.planList.Count > 0 ? "0" : "1";
                response.sMessage = response.planList.Count > 0 ? "Se proceso todo correctamente" : "No existen planes para esta combinación";

            }
            catch (Exception ex)
            {
                response.codError = "1";
                response.sMessage = "Hubo un error al consultar los planes: " + ex.Message;
                response.planList = null;
                LogControl.save("GetTypePlan", ex.ToString(), "3");
            }

            return response;
        }

        [Route("GetModalidad")]
        [HttpPost]
        public List<ModalidadVm> GetModalidad(PlanBM data)
        {
            return quotationCORE.GetModalidad(data);
        }

        [Route("GetCoberturas")]
        [HttpPost]
        public async Task<List<CoberturasVM>> GetCoberturas(CoberturaBM data)
        {
            var response = new List<CoberturasVM>();

            var requestPlan = new TariffGraph();

            if (new string[] { ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch") }.Contains(data.codBranch.ToString()))
            {
                requestPlan = new TariffGraph()
                {
                    idTariff = data.idTariff,
                    versionTariff = data.versionTariff,
                    nbranch = data.codBranch.ToString(),
                    currency = data.monedaId.ToString(),
                    channel = data.channel,
                    billingType = data.billingType,
                    policyType = data.tipoProducto.ToString(),
                    collocationType = data.tipoModalidad.ToString(),
                    profile = data.codPerfil.ToString(),
                };
                requestPlan.profile = quotationCORE.getProfileRepeat(requestPlan.nbranch, requestPlan.profile);

                var coversCot = quotationCORE.GetCoberturas(data);

                if (!data.visualizar)
                {
                    var miniTariff = await quotationCORE.getMiniTariffGraph(requestPlan);

                    if (miniTariff.codError == 0)
                    {
                        if (miniTariff.data.miniTariffMatrix != null &&
                            miniTariff.data.miniTariffMatrix.segment != null &&
                            miniTariff.data.miniTariffMatrix.segment.modules != null)
                        {
                            if (miniTariff.data.miniTariffMatrix.segment.modules.Count() > 0)
                            {
                                var itemPlan = miniTariff.data.miniTariffMatrix.segment.modules.Where(x => x.id == data.planId).ToList();

                                // int valFactor = valRentaEstudiantil(data.codBranch.ToString(), data.codproducto) ? quotationCORE.getFactor(data.codBranch.ToString(), data.codproducto, data.planId) : 0;

                                foreach (var item in itemPlan)
                                {
                                    //if (item.id == data.planId)
                                    //{
                                    var listCover = item.coverages.Where(x => x.status == "ENABLED").ToList();

                                    int reg = 1;

                                    string JSON = JsonConvert.SerializeObject(listCover);

                                    foreach (var cover in listCover)
                                    {
                                        var cobertura = new CoberturasVM();
                                        cobertura.ncover = reg++;
                                        cobertura.codCobertura = cover.id;
                                        cobertura.descripcion = cover.description;
                                        cobertura.suma_asegurada = cover.capital.@base;
                                        cobertura.capital_max = cover.capital.max;
                                        cobertura.capital_min = cover.capital.min;
                                        cobertura.capital = cover.capital.@base;
                                        cobertura.cobertura_pri = Convert.ToBoolean(cover.required) ? "1" : "0";

                                        //cobertura.cobertura_adi = "0";
                                        cobertura.entryAge = cover.entryAge;
                                        cobertura.stayAge = cover.stayAge;
                                        cobertura.hours = cover.hours;
                                        cobertura.capitalCovered = cover.capitalCovered;
                                        cobertura.limit = cover.limit;
                                        cobertura.message = string.Empty;
                                        cobertura.items = cover.items;

                                        //PRUEBAS AVS - RENTAS
                                        cobertura.id_cover = cover.capital.@type;
                                        cobertura.sdes_cover = cover.capital.@type == "FIXED_PENSION_QUANTITY" ? "CANT. PENSIONES FIJA" :
                                                               cover.capital.@type == "MAXIMUM_PENSION_QUANTITY" ? "PENSIÓN MÁXIMA" :
                                                               cover.capital.@type == "FIXED_INSURED_SUM" ? "MONTO FIJO" :
                                                               cover.capital.@type == "FIXED_INSURED_SUM_PERCENTAGE" ? "VALOR PORCENTUAL" :
                                                               "MONTO FIJO";

                                        cobertura.pension_base = new string[] { "FIXED_PENSION_QUANTITY", "MAXIMUM_PENSION_QUANTITY" }.Contains(cover.capital.@type) ? cover.capital.@base : 0;
                                        cobertura.pension_max = new string[] { "FIXED_PENSION_QUANTITY", "MAXIMUM_PENSION_QUANTITY" }.Contains(cover.capital.@type) ? cover.capital.max : 0;
                                        cobertura.pension_min = new string[] { "FIXED_PENSION_QUANTITY", "MAXIMUM_PENSION_QUANTITY" }.Contains(cover.capital.@type) ? cover.capital.min : 0;
                                        cobertura.pension_prop = new string[] { "FIXED_PENSION_QUANTITY", "MAXIMUM_PENSION_QUANTITY" }.Contains(cover.capital.@type) ? cover.capital.@base : 0;

                                        cobertura.porcen_base = cover.capital.@type == "FIXED_INSURED_SUM_PERCENTAGE" ? cover.capitalCovered : 0;
                                        cobertura.porcen_max = cover.capital.@type == "FIXED_INSURED_SUM_PERCENTAGE" ? cover.capitalCovered : 0;
                                        cobertura.porcen_min = cover.capital.@type == "FIXED_INSURED_SUM_PERCENTAGE" ? cover.capitalCovered : 0;
                                        cobertura.porcen_prop = cover.capital.@type == "FIXED_INSURED_SUM_PERCENTAGE" ? cover.capitalCovered : 0;

                                        cobertura.lackPeriod = cover.lackPeriod;
                                        cobertura.deductible = cover.deductible;
                                        cobertura.copayment = cover.copayment;
                                        cobertura.maxAccumulation = cover.maxAccumulation;
                                        cobertura.comment = cover.comment;

                                        var coverFind = coversCot.FirstOrDefault(x => x.codCobertura == cobertura.codCobertura);
                                        cobertura.cobertura_adi = coverFind != null ? "1" : "0";
                                        cobertura.capital_prop = coverFind != null ? coverFind.capital_prop == 0 ? cobertura.suma_asegurada : coverFind.capital_prop : cobertura.suma_asegurada; // cover.capital.@base;
                                        cobertura.capital_aut = coverFind != null ? coverFind.capital_prop == 0 ? cobertura.capital_aut : coverFind.capital_prop : 0;

                                        cobertura.porcen_prop = coverFind != null ? coverFind.porcen_prop : cobertura.porcen_prop;
                                        cobertura.pension_prop = coverFind != null ? coverFind.pension_prop : cobertura.pension_prop;

                                        response.Add(cobertura);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (coversCot.Count > 0)
                    {
                        int reg = 1;

                        foreach (var cover in coversCot)
                        {
                            var cobertura = new CoberturasVM();
                            cobertura = cover;
                            cobertura.ncover = reg++;
                            cobertura.cobertura_adi = "1";
                            cobertura.sdes_cover = cobertura.id_cover == "FIXED_PENSION_QUANTITY" ? "CANT. PENSIONES FIJA" :
                                                   cobertura.id_cover == "MAXIMUM_PENSION_QUANTITY" ? "PENSIÓN MÁXIMA" :
                                                   cobertura.id_cover == "FIXED_INSURED_SUM" ? "MONTO FIJO" :
                                                   cobertura.id_cover == "FIXED_INSURED_SUM_PERCENTAGE" ? "VALOR PORCENTUAL" :
                                                   "MONTO FIJO";
                            response.Add(cobertura);
                        }
                    }
                }
            }
            else if (new string[] { ELog.obtainConfig("vidaIndividualBranch") }.Contains(data.codBranch.ToString()))
            {
                requestPlan = new TariffGraph()
                {
                    nbranch = data.codBranch.ToString(),
                    currency = data.monedaId.ToString(),
                    channel = data.channel,
                    billingType = data.billingType,
                    policyType = data.tipoProducto.ToString(),
                    renewalType = data.renewalType,
                    creditType = data.creditType
                };

                var coversCot = quotationCORE.GetCoberturas(data);

                var miniTariff = await quotationCORE.getMiniTariffGraphDes(requestPlan);

                if (miniTariff.codError == 0)
                {
                    if (miniTariff.data.miniTariffMatrix != null &&
                        miniTariff.data.miniTariffMatrix.segment != null &&
                        miniTariff.data.miniTariffMatrix.segment.modules != null)
                    {
                        if (miniTariff.data.miniTariffMatrix.segment.modules.Count() > 0)
                        {
                            var itemPlan = miniTariff.data.miniTariffMatrix.segment.modules.Where(x => x.id == data.planId).ToList();

                            foreach (var item in itemPlan)
                            {
                                //if (item.id == data.planId)
                                //{
                                var listCover = item.coverages.Where(x => x.status == "ENABLED").ToList();

                                int reg = 1;
                                foreach (var cover in listCover)
                                {
                                    var cobertura = new CoberturasVM();
                                    cobertura.ncover = reg++;
                                    cobertura.codCobertura = cover.id;
                                    cobertura.descripcion = cover.description;
                                    cobertura.suma_asegurada = cover.capital.min;
                                    cobertura.capital_max = cover.capital.max;
                                    cobertura.capital_min = cover.capital.min;
                                    //cobertura.capital_prop = cover.capital.@base;
                                    //cobertura.capital_aut = 0;

                                    //cobertura.capital = cover.capital.@base;
                                    //cobertura.cobertura_pri = Convert.ToBoolean(cover.required) ? "1" : "0";

                                    //cobertura.cobertura_adi = "0";
                                    cobertura.entryAge = cover.entryAge;
                                    cobertura.stayAge = cover.stayAge;
                                    cobertura.hours = cover.hours;
                                    cobertura.capitalCovered = cover.capitalCovered;
                                    cobertura.limit = cover.limit;
                                    cobertura.message = string.Empty;
                                    cobertura.items = cover.items;

                                    double monto = cover.capital.min;

                                    var coverFind = coversCot.FirstOrDefault(x => x.codCobertura == cobertura.codCobertura);

                                    if (coverFind != null)
                                    {
                                        if (Convert.ToBoolean(!cover.optional))
                                        {
                                            if (coverFind.def == "1")
                                            {
                                                //monto = objValida.datosPoliza.capitalCredito;
                                                monto = coverFind.capital == 0 ? coverFind.capital_prop : coverFind.capital;
                                            }
                                            else
                                            {
                                                monto = cover.capital.min;
                                            }
                                            cobertura.def = coverFind.def;
                                        }
                                    }
                                    else
                                    {
                                        monto = cover.capital.min;
                                    }

                                    cobertura.cobertura_adi = coverFind != null ? "1" : "0";
                                    cobertura.capital_prop = coverFind != null ? coverFind.capital_prop == 0 ? cobertura.suma_asegurada : coverFind.capital_prop : cover.capital.@base;
                                    cobertura.capital_aut = coverFind != null ? coverFind.capital_aut : 0;
                                    //Cambio R.P. Coberturas al actualizar
                                    cobertura.suma_asegurada = monto;   //cobertura.capital_prop;
                                                                        //Cambio R.P. Coberturas al actualizar
                                    cobertura.capital = monto;
                                    cobertura.cobertura_pri = Convert.ToBoolean(cover.optional) ? "0" : "1";

                                    response.Add(cobertura);
                                }
                                //break;
                                //}
                            }
                        }
                    }
                }

            }
            else if (new string[] { ELog.obtainConfig("desgravamenBranch") }.Contains(data.codBranch.ToString()))
            {

                var coversCotDes = quotationCORE.GetCoberturas(data);


                //Desgravamen-Devolucion

                var CoverBM = new CoberturaBM()
                {
                    nbranch = data.codBranch.ToString(),
                    tipoProducto = Int32.Parse(data.codproducto.ToString()),
                    currency = data.monedaId.ToString()
                };

                var coversdesdev = quotationCORE.GetCoberturasGenDev(CoverBM);
                int reg = 1;
                foreach (var coverCot in coversdesdev)
                {

                    var coversave = new CoberturasVM();

                    coversave.ncover = Int32.Parse(coverCot.codCobertura.ToString());
                    coversave.ncover = reg++;
                    coversave.codCobertura = coverCot.codCobertura.ToString();
                    coversave.descripcion = coverCot.descripcion.Trim();
                    coversave.cobertura_pri = coverCot.cobertura_pri;
                    coversave.capital = coverCot.capital_prop;
                    coversave.capitalCovered = 0;
                    coversave.hours = 24;
                    coversave.cobertura_adi = "0";

                    foreach (var coverCotdet in coversCotDes)
                    {
                        if (coversave.codCobertura == coverCotdet.codCobertura)
                        {
                            coversave.cobertura_adi = "1";
                            break;
                        }
                    }

                    response.Add(coversave);
                }

                //return response;
                //Desgravamen-Devolucion
            }

            return response;
        }

        [Route("GetListComision")]
        [HttpPost]
        public ComisionVM GetListComision()
        {
            return quotationCORE.GetListComision();
        }

        [Route("GetPremiumMin")]
        [HttpGet]
        public PremiumBM GetPremiumMin()
        {
            return quotationCORE.GetPremiumBM();
        }

        [Route("GetFechasRenovacion")]
        [HttpGet]
        public FechasRenoVM GetFechasRenovacion(string nrocotizacion, string typetransac, string codproducto)
        {
            return quotationCORE.ObtenerFechasRenov(nrocotizacion, typetransac, codproducto);
        }

        [Route("UploadVoucher")]
        [HttpPost]
        public async Task<GenericResponseVM> UploadVoucher()
        {
            var nrocotizacion = "";
            if (!Request.Content.IsMimeMultipartContent())
            {
                var response = new GenericResponseVM();
                response.StatusCode = 1;
                response.Message = "El requerimiento no contiene un contenido válido.";
                return response;
            }

            nrocotizacion = HttpContext.Current.Request.Params.Get("keycotizacion");
            String filePath = String.Format(ELog.obtainConfig("pathPrincipal"), ELog.obtainConfig("pathCotizacion") + nrocotizacion + "\\" + ELog.obtainConfig("cotizacionKey") + "\\");  //Directorio raiz de proyecto

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            try
            {
                var provider = new MultipartMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);
                foreach (var file in provider.Contents)
                {
                    var dataStream = await file.ReadAsStreamAsync();
                    using (var fileStream = File.Create(filePath + "voucher"))
                    {
                        dataStream.Seek(0, SeekOrigin.Begin);
                        dataStream.CopyTo(fileStream);
                    }
                    var response = new GenericResponseVM();
                    response.StatusCode = 0;
                    response.Message = "Archivo subido exitosamente.";
                    response.GenericResponse = filePath;
                    response.archivos = Directory.GetFiles(filePath, "*", SearchOption.TopDirectoryOnly);
                    return response;

                }
                var otherresponse = new GenericResponseVM();
                otherresponse.StatusCode = 1;
                otherresponse.Message = "Archivo no puedo ser subido.";
                return otherresponse;
            }
            catch (Exception ex)
            {
                var otherresponse = new GenericResponseVM();
                otherresponse.StatusCode = 1;
                otherresponse.Message = ex.Message;
                LogControl.save("UploadVoucher", ex.ToString(), "3");
                return otherresponse;
            }
        }

        [Route("ListSurcharges")]
        [HttpPost]
        public async Task<ResponseBenefit> ListSurcharges(BenefitBM data)
        {
            var response = new ResponseBenefit() { codError = "0" };
            var requestPlan = new TariffGraph();

            if (new string[] { ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch") }.Contains(data.codBranch.ToString()))
            {
                requestPlan = new TariffGraph()
                {
                    idTariff = data.idTariff,
                    versionTariff = data.versionTariff,
                    nbranch = data.codBranch.ToString(),
                    currency = data.currency,
                    channel = data.channel,
                    billingType = data.billingType,
                    policyType = data.policyType,
                    collocationType = data.collocationType,
                    profile = data.profile
                };

                requestPlan.profile = quotationCORE.getProfileRepeat(requestPlan.nbranch, requestPlan.profile);

                // Recargos de la cotizacion
                var surchargesCot = quotationCORE.ListSurcharges(data);

                if (!data.visualizar)
                {
                    // Traer toda la info del segmento
                    var miniTariff = await quotationCORE.getMiniTariffGraph(requestPlan);

                    if (miniTariff.codError == 0)
                    {
                        if (miniTariff.data.miniTariffMatrix != null &&
                           miniTariff.data.miniTariffMatrix.segment != null &&
                           miniTariff.data.miniTariffMatrix.segment.modules != null)
                        {
                            if (miniTariff.data.miniTariffMatrix.segment.surcharges.Count() > 0)
                            {
                                var list = miniTariff.data.miniTariffMatrix.segment.surcharges.Where(x => Convert.ToBoolean(x.optional) == true && x.status == "ENABLED").ToList();

                                int num = 0;
                                foreach (var item in list)
                                {
                                    num = num + 1;
                                    var recargo = new SurchargeVM()
                                    {
                                        order = num,
                                        codSurcharge = item.id,
                                        desSurcharge = item.description,
                                        value = item.value.amount,
                                        //sMark = "0"
                                        sMark = surchargesCot.ListSurcharges.FirstOrDefault(x => x.codSurcharge == item.id) != null ? "1" : "0"
                                    };

                                    //foreach (var surchargeCot in surchargesCot.ListSurcharges)
                                    //{
                                    //    if (surchargeCot.codSurcharge == item.id)
                                    //    {
                                    //        recargo.sMark = "1";
                                    //        break;
                                    //    }
                                    //}

                                    response.ListSurcharges.Add(recargo);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (surchargesCot.ListSurcharges.Count > 0)
                    {
                        var num = 1;
                        foreach (var item in surchargesCot.ListSurcharges)
                        {
                            var recargo = new SurchargeVM();
                            recargo = item;
                            recargo.order = num++;
                            response.ListSurcharges.Add(recargo);
                        }
                    }
                }
            }
            else if (new string[] { ELog.obtainConfig("vidaIndividualBranch") }.Contains(data.codBranch.ToString()))
            {
                requestPlan = new TariffGraph()
                {
                    nbranch = data.codBranch.ToString(),
                    currency = data.monedaId.ToString(),
                    channel = data.channel,
                    billingType = data.billingType,
                    policyType = data.policyType,
                    renewalType = data.renewalType,
                    creditType = data.creditType
                };

                // Recargos de la cotizacion
                var surchargesCot = quotationCORE.ListSurcharges(data);

                // Traer toda la info del segmento
                var miniTariff = await quotationCORE.getMiniTariffGraphDes(requestPlan);

                if (miniTariff.codError == 0)
                {
                    if (miniTariff.data.miniTariffMatrix != null &&
                       miniTariff.data.miniTariffMatrix.segment != null &&
                       miniTariff.data.miniTariffMatrix.segment.modules != null)
                    {
                        if (miniTariff.data.miniTariffMatrix.segment.surcharges.Count() > 0)
                        {
                            var list = miniTariff.data.miniTariffMatrix.segment.surcharges.Where(x => Convert.ToBoolean(x.optional) == true && x.status == "ENABLED").ToList();

                            int num = 0;
                            foreach (var item in list)
                            {
                                num = num + 1;
                                var recargo = new SurchargeVM()
                                {
                                    order = num,
                                    codSurcharge = item.id,
                                    desSurcharge = item.description,
                                    value = item.value.amount,
                                    //sMark = "0"
                                    sMark = surchargesCot.ListSurcharges.FirstOrDefault(x => x.codSurcharge == item.id) != null ? "1" : "0"
                                };

                                //foreach (var surchargeCot in surchargesCot.ListSurcharges)
                                //{
                                //    if (surchargeCot.codSurcharge == item.id)
                                //    {
                                //        recargo.sMark = "1";
                                //        break;
                                //    }
                                //}

                                response.ListSurcharges.Add(recargo);
                            }
                        }
                    }
                }
            }

            return response;
        }

        [Route("ListBenefits")]
        [HttpPost]
        public async Task<ResponseBenefit> ListBenefits(BenefitBM data)
        {
            var response = new ResponseBenefit() { codError = "0" };
            var requestPlan = new TariffGraph();

            if (new string[] { ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch") }.Contains(data.codBranch.ToString()))
            {
                requestPlan = new TariffGraph()
                {
                    idTariff = data.idTariff,
                    versionTariff = data.versionTariff,
                    nbranch = data.codBranch.ToString(),
                    currency = data.currency,
                    channel = data.channel,
                    billingType = data.billingType,
                    policyType = data.policyType,
                    collocationType = data.collocationType,
                    profile = data.profile
                };

                requestPlan.profile = quotationCORE.getProfileRepeat(requestPlan.nbranch, requestPlan.profile);

                var benefitCot = quotationCORE.ListBenefits(data);

                if (!data.visualizar)
                {
                    var miniTariff = await quotationCORE.getMiniTariffGraph(requestPlan);
                    if (miniTariff.codError == 0)
                    {
                        if (miniTariff.data.miniTariffMatrix != null &&
                           miniTariff.data.miniTariffMatrix.segment != null &&
                           miniTariff.data.miniTariffMatrix.segment.modules != null)
                        {
                            if (miniTariff.data.miniTariffMatrix.segment.modules.Count() > 0)
                            {
                                var itemPlan = miniTariff.data.miniTariffMatrix.segment.modules.Where(x => x.id == data.planId).ToList();

                                foreach (var item in itemPlan)
                                {

                                    var listBenefits = item.benefits.Where(x => x.status == "ENABLED").ToList();

                                    string jsonListBenefits = JsonConvert.SerializeObject(listBenefits); //PARA PRUEBAS


                                    var num = 0;
                                    string benef = "0";
                                    foreach (var benefit in listBenefits)
                                    {
                                        num = num + 1;
                                        var beneficio = new BenefitVM();
                                        beneficio.num = num;
                                        beneficio.codBenefit = benefit.id;
                                        beneficio.desBenefit = benefit.description;
                                        beneficio.sMark = benefitCot.ListBenefits.FirstOrDefault(x => x.codBenefit == benefit.id) != null ? "1" : "0";
                                        beneficio.exc_beneficio = benefit.exclusiveStudentRent;
                                        beneficio.studentRentBenefit = benefit.studentRentBenefit; //AVS - RENTAS

                                        if (data.codBranch.ToString() == ELog.obtainConfig("vidaGrupoBranch"))
                                        {
                                            benef = data.varExec?.FirstOrDefault(cobertura => cobertura.codCobertura == "1001")?.capital_prop.ToString() ?? "0"; //AVS - RENTAS

                                            if (beneficio.exc_beneficio && beneficio.desBenefit.Contains("(X)"))
                                            {
                                                beneficio.desBenefit = beneficio.desBenefit.Replace("(X)", "(" + benef + ")");
                                            }
                                        }

                                        response.ListBenefits.Add(beneficio);
                                    }
                                }
                            }
                        }

                    }
                }
                else
                {
                    if (benefitCot.ListBenefits.Count > 0)
                    {
                        var num = 1;
                        foreach (var benefit in benefitCot.ListBenefits)
                        {
                            var beneficio = new BenefitVM();
                            beneficio = benefit;
                            beneficio.num = num++;
                            beneficio.sMark = "1";
                            response.ListBenefits.Add(beneficio);
                        }
                    }

                }
            }
            else if (new string[] { ELog.obtainConfig("vidaIndividualBranch") }.Contains(data.codBranch.ToString()))
            {
                requestPlan = new TariffGraph()
                {
                    nbranch = data.codBranch.ToString(),
                    currency = data.monedaId.ToString(),
                    channel = data.channel,
                    billingType = data.billingType,
                    policyType = data.policyType,
                    renewalType = data.renewalType,
                    creditType = data.creditType
                };

                var benefitCot = quotationCORE.ListBenefits(data);

                var miniTariff = await quotationCORE.getMiniTariffGraphDes(requestPlan);
                if (miniTariff.codError == 0)
                {
                    if (miniTariff.data.miniTariffMatrix != null &&
                       miniTariff.data.miniTariffMatrix.segment != null &&
                       miniTariff.data.miniTariffMatrix.segment.modules != null)
                    {
                        if (miniTariff.data.miniTariffMatrix.segment.modules.Count() > 0)
                        {
                            var itemPlan = miniTariff.data.miniTariffMatrix.segment.modules.Where(x => x.id == data.planId).ToList();

                            foreach (var item in itemPlan)
                            {
                                //if (item.id == data.planId)
                                //{
                                var listBenefits = item.benefits.Where(x => x.status == "ENABLED").ToList();

                                var num = 0;
                                foreach (var benefit in listBenefits)
                                {
                                    num = num + 1;
                                    var beneficio = new BenefitVM();
                                    beneficio.num = num;
                                    beneficio.codBenefit = benefit.id;
                                    beneficio.desBenefit = benefit.description;
                                    //beneficio.sMark = "0";
                                    beneficio.sMark = benefitCot.ListBenefits.FirstOrDefault(x => x.codBenefit == benefit.id) != null ? "1" : "0";
                                    beneficio.benefit_pri = Convert.ToBoolean(benefit.optional) ? "0" : "1";

                                    //foreach (var beneficioCot in benefitCot.ListBenefits)
                                    //{
                                    //    if (benefit.code == beneficioCot.codBenefit)
                                    //    {
                                    //        beneficio.sMark = "1";
                                    //        break;
                                    //    }
                                    //}

                                    response.ListBenefits.Add(beneficio);
                                }
                                //break;
                                //}
                            }
                        }
                        /*
                    {
                        foreach (var item in miniTariff.data.miniTariffMatrix.segment.modules)
                        {
                            if (item.id == data.planId)
                            {
                                foreach (var benefit in item.benefits)
                                {
                                    var beneficio = new BenefitVM();
                                    beneficio.codBenefit = benefit.id;
                                    beneficio.desBenefit = benefit.description;
                                    beneficio.sMark = "0";
                                    beneficio.benefit_pri = Convert.ToBoolean(benefit.optional) ? "0" : "1";

                                    foreach (var beneficioCot in benefitCot.ListBenefits)
                                    {
                                        if (benefit.id == beneficioCot.codBenefit)
                                        {
                                            beneficio.sMark = "1";
                                            break;
                                        }
                                    }

                                    response.ListBenefits.Add(beneficio);
                                }
                                break;
                            }
                        }
                    }
                    */
                    }

                }
            }

            return response;
        }

        [Route("ListAssists")]
        [HttpPost]
        public async Task<ResponseAssists> ListAssists(AssistsBM data)
        {
            var response = new ResponseAssists() { codError = "0" };

            var requestPlan = new TariffGraph();

            if (new string[] { ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch") }.Contains(data.codBranch.ToString()))
            {
                requestPlan = new TariffGraph()
                {
                    idTariff = data.idTariff,
                    versionTariff = data.versionTariff,
                    nbranch = data.codBranch.ToString(),
                    currency = data.currency,
                    channel = data.channel,
                    billingType = data.billingType,
                    policyType = data.policyType,
                    collocationType = data.collocationType,
                    profile = data.profile
                };
                requestPlan.profile = quotationCORE.getProfileRepeat(requestPlan.nbranch, requestPlan.profile);

                var assistCot = quotationCORE.ListAssists(data);

                if (!data.visualizar)
                {
                    var miniTariff = await quotationCORE.getMiniTariffGraph(requestPlan);

                    if (miniTariff.codError == 0)
                    {
                        if (miniTariff.data.miniTariffMatrix != null &&
                            miniTariff.data.miniTariffMatrix.segment != null &&
                            miniTariff.data.miniTariffMatrix.segment.modules != null)
                        {
                            if (miniTariff.data.miniTariffMatrix.segment.modules.Count() > 0)
                            {
                                var itemPlan = miniTariff.data.miniTariffMatrix.segment.modules.Where(x => x.id == data.planId).ToList();

                                foreach (var item in itemPlan)
                                {
                                    item.assistances = item.assistances != null ? item.assistances : new List<Assistance>();

                                    var num = 0;
                                    foreach (var assist in item.assistances)
                                    {
                                        num = num + 1;
                                        var asistencia = new AssistsVM();
                                        asistencia.num = num;
                                        asistencia.codAssist = assist.id;
                                        asistencia.desAssist = assist.description;
                                        //asistencia.sMark = "0";
                                        asistencia.provider = assist.provider;
                                        asistencia.value = assist.value;
                                        asistencia.document = assist.document;
                                        asistencia.sMark = assistCot.ListAssists.FirstOrDefault(x => x.codAssist == assist.id) != null ? "1" : "0";
                                        response.ListAssists.Add(asistencia);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (assistCot.ListAssists.Count > 0)
                    {
                        var num = 1;
                        foreach (var assist in assistCot.ListAssists)
                        {
                            var asistencia = new AssistsVM();
                            asistencia = assist;
                            asistencia.num = num++;
                            asistencia.sMark = "1";
                            response.ListAssists.Add(asistencia);
                        }
                    }
                }

            }
            else if (new string[] { ELog.obtainConfig("vidaIndividualBranch") }.Contains(data.codBranch.ToString()))
            {
                requestPlan = new TariffGraph()
                {
                    nbranch = data.codBranch.ToString(),
                    currency = data.monedaId.ToString(),
                    channel = data.channel,
                    billingType = data.billingType,
                    policyType = data.policyType,
                    renewalType = data.renewalType,
                    creditType = data.creditType
                };

                var assistCot = quotationCORE.ListAssists(data);
                var miniTariff = await quotationCORE.getMiniTariffGraphDes(requestPlan);

                if (miniTariff.codError == 0)
                {
                    if (miniTariff.data.miniTariffMatrix != null &&
                       miniTariff.data.miniTariffMatrix.segment != null &&
                       miniTariff.data.miniTariffMatrix.segment.modules != null)
                    {
                        if (miniTariff.data.miniTariffMatrix.segment.modules.Count() > 0)
                        {
                            var itemPlan = miniTariff.data.miniTariffMatrix.segment.modules.Where(x => x.id == data.planId).ToList();

                            foreach (var item in itemPlan)
                            {
                                //if (item.id == data.planId)
                                //{
                                var listAssistances = item.assistances.Where(x => x.status == "ENABLED").ToList();

                                var num = 0;
                                foreach (var assist in listAssistances)
                                {
                                    var asistencia = new AssistsVM();
                                    asistencia.codAssist = assist.id;
                                    asistencia.desAssist = assist.description;
                                    asistencia.assist_pri = Convert.ToBoolean(assist.optional) ? "0" : "1";
                                    asistencia.sMark = "0";
                                    asistencia.provider = new Provider()
                                    {
                                        code = assist.provider.code,
                                        description = assist.provider.description
                                    };
                                    asistencia.document = assist.document;

                                    foreach (var asistenciaCot in assistCot.ListAssists)
                                    {
                                        if (assist.id == asistenciaCot.codAssist)
                                        {
                                            asistencia.sMark = "1";
                                            break;
                                        }
                                    }
                                    response.ListAssists.Add(asistencia);
                                }
                                //break;
                                //}
                            }
                        }
                    }
                }

            }

            return response;
        }

        [Route("ListAdditionalServices")]
        [HttpPost]
        public async Task<ResponseBenefit> ListAdditionalServices(BenefitBM data)
        {
            var listServiciosAdicionales = new ResponseBenefit() { codError = "0" };
            var response = new ResponseBenefit() { codError = "0" };
            var requestPlan = new TariffGraph();

            if (new string[] { ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch") }.Contains(data.codBranch.ToString()))
            {
                requestPlan = new TariffGraph()
                {
                    idTariff = data.idTariff,
                    versionTariff = data.versionTariff,
                    nbranch = data.codBranch.ToString(),
                    currency = data.currency,
                    channel = data.channel,
                    billingType = data.billingType,
                    policyType = data.policyType,
                    collocationType = data.collocationType,
                    profile = data.profile
                };
                requestPlan.profile = quotationCORE.getProfileRepeat(requestPlan.nbranch, requestPlan.profile);

                var servicesCot = quotationCORE.ListAdditionalServices(data);

                if (!data.visualizar)
                {
                    var miniTariff = await quotationCORE.getMiniTariffGraph(requestPlan);

                    if (miniTariff.codError == 0)
                    {
                        if (miniTariff.data.miniTariffMatrix != null &&
                            miniTariff.data.miniTariffMatrix.segment != null &&
                            miniTariff.data.miniTariffMatrix.segment.modules != null)
                        {
                            if (miniTariff.data.miniTariffMatrix.segment.modules.Count() > 0)
                            {
                                var itemPlan = miniTariff.data.miniTariffMatrix.segment.modules.Where(x => x.id == data.planId).ToList();

                                foreach (var item in itemPlan)
                                {
                                    item.additionalServices = item.additionalServices != null ? item.additionalServices : new List<AdditionalService>();
                                    var num = 0;
                                    foreach (var service in item.additionalServices)
                                    {
                                        num = num + 1;
                                        var serviceFind = servicesCot.ListAdditionalService.FirstOrDefault(x => x.codServAdicionales == service.id);
                                        var adservices = new AdditionalServiceVM();
                                        adservices.order = num;
                                        adservices.codServAdicionales = service.id;
                                        adservices.desServAdicionales = service.description;
                                        adservices.sMark = serviceFind != null ? "1" : "0";
                                        adservices.amount = serviceFind != null ? serviceFind.amount : 1;
                                        response.ListAdditionalService.Add(adservices);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    var num = 1;
                    foreach (var service in servicesCot.ListAdditionalService)
                    {
                        var adservices = new AdditionalServiceVM();
                        adservices = service;
                        adservices.order = num;
                        response.ListAdditionalService.Add(adservices);
                    }
                }

            }

            return response;
        }

        [Route("GetPrimaPlan")]
        [HttpPost]
        public async Task<ResponseEcommerceVM> GetPrimaPlan(PrimaPlanBM data)
        {
            LogControl.save("GetPrimaPlan", "Se inicio el servicio GetPrimaPlan-Ecommerce", "1", "EC");
            LogControl.save("GetPrimaPlan", JsonConvert.SerializeObject(data, Formatting.None), "2", "EC");

            var response = new ResponseEcommerceVM();
            var requestTariff = new TariffGraph();
            requestTariff.billingType = data.codBilling;
            requestTariff.channel = data.codChannel;
            requestTariff.application = "EC";
            requestTariff.collocationType = data.codTypeMod;
            requestTariff.nbranch = data.codBranch.ToString();
            requestTariff.policyType = data.codTypePolicy;
            requestTariff.profile = data.codProfile;
            requestTariff.currency = data.codCurrency;

            var res = await GetTipoTarifario(requestTariff);

            if (res.Count() > 0)
            {

                var objValida = new validaTramaVM()
                {
                    codProducto = data.codProduct,
                    codUsuario = data.codUser,
                    flagpremium = 1,
                    type_mov = "1",
                    nroCotizacion = null,
                    codCanal = data.codChannel,
                    CantidadTrabajadores = Convert.ToInt64(data.workers),
                    datosPoliza = new Entities.QuotationModel.BindingModel.DatosPoliza()
                    {
                        branch = Convert.ToInt32(data.codBranch),
                        codTipoNegocio = data.codTypePolicy,
                        codTipoPerfil = data.codProfile,
                        //codTipoPlan = plan.codPlan,
                        codMon = data.codCurrency,
                        typeTransac = data.stransac,
                        InicioVigPoliza = data.policyInitDate,
                        FinVigPoliza = data.policyEndDate,
                        codTipoFrecuenciaPago = data.codFreqPay,
                        codTipoFacturacion = data.codBilling,
                        codTipoModalidad = data.codTypeMod,
                        CodActividadRealizar = data.codActivity,
                        CodCiiu = data.codActivity,
                        codTipoProducto = data.codProduct,
                        temporalidad = Convert.ToInt32(data.temporality),
                        codAlcance = Convert.ToInt32(data.codScope),
                        tipoUbigeo = Convert.ToInt32(data.typeLocation),
                        codUbigeo = data.location,
                        trxCode = data.stransac,
                        idTariff = res[0].idTariff,
                        versionTariff = res[0].versionTariff.ToString()
                    },
                    datosContratante = new DatosContractor()
                    {
                        codContratante = data.codContractor,
                        desContratante = data.desContractor
                    }
                };

                var miniTariff = await getMiniTariffGraph(objValida);

                if (miniTariff.codError == 0)
                {
                    if (miniTariff.data.miniTariffMatrix != null &&
                        miniTariff.data.miniTariffMatrix.segment != null &&
                        miniTariff.data.miniTariffMatrix.segment.modules != null)
                    {
                        if (miniTariff.data.miniTariffMatrix.segment.modules.Count > 0)
                        {
                            var modules = data.npensiones == "1" ? miniTariff.data.miniTariffMatrix.segment.modules.Where(x => x.planType == "PENSION_QUANTITY").ToList() : miniTariff.data.miniTariffMatrix.segment.modules;

                            response.codError = "0";
                            response.sMessage = "Se proceso todo correctamente";
                            response.idTariff = res[0].idTariff;
                            response.versionTariff = res[0].versionTariff;
                            response.nameTariff = res[0].desTariff;

                            int num = 1;

                            foreach (var item in modules)
                            {
                                var plan = new PrimaPlanVM();
                                plan.codProceso = (num + data.codUser + ELog.generarCodigo() + DateTime.Now.ToString("yyyyMMddHHmmss")).PadLeft(30, '0');
                                plan.codPlan = item.id;
                                plan.desPlan = item.description;

                                objValida.codProceso = plan.codProceso;
                                objValida.datosPoliza.codTipoPlan = plan.codPlan;

                                //var itemPlan = miniTariff.data.miniTariffMatrix.segment.modules.FirstOrDefault(x => x.id == item.id);

                                //if (itemPlan != null)
                                //{
                                objValida.lcoberturas = new List<coberturaPropuesta>();

                                var listCover = item.coverages.Where(x => Convert.ToBoolean(x.required) == true && x.status == "ENABLED").ToList();

                                foreach (var cover in listCover)
                                {
                                    var coverItem = new coberturaPropuesta()
                                    {
                                        codCobertura = cover.id,
                                        sumaPropuesta = cover.capital.@base
                                    };

                                    objValida.lcoberturas.Add(coverItem);
                                }

                                var responseCover = quotationCORE.insertCoverRequerid(objValida, listCover);

                                var listSubItem = listCover.Where(x => x.items != null && x.items.Count > 0).ToList();
                                var responseSubCover = quotationCORE.insertSubItemPD(objValida, listSubItem);

                                var responseTarifario = await new QuotationCORE().SendDataTarifarioGraphql(objValida);
                                plan.prima = responseTarifario.codError == 0 ? responseTarifario.data != null ? responseTarifario.data.searchPremium.commercialPremium.ToString() : "0" : "0";
                                plan.igv = responseTarifario.codError == 0 ? responseTarifario.data != null ? responseTarifario.data.searchPremium.totalValueWithIgv.ToString() : "0" : "0";
                                plan.primaTotal = responseTarifario.codError == 0 ? responseTarifario.data != null ? responseTarifario.data.searchPremium.totalPremium.ToString() : "0" : "0";
                                plan.asegPrima = responseTarifario.codError == 0 ? responseTarifario.data != null ? responseTarifario.data.searchPremium.unitCommercialPremium.ToString() : "0" : "0";
                                plan.asegIgv = responseTarifario.codError == 0 ? responseTarifario.data != null ? responseTarifario.data.searchPremium.igvUnitValue.ToString() : "0" : "0";
                                plan.asegPrimaTotal = responseTarifario.codError == 0 ? responseTarifario.data != null ? responseTarifario.data.searchPremium.unitCommercialPremiumWithIgv.ToString() : "0" : "0";



                                num++;
                                response.planList.Add(plan);
                            }
                        }
                        else
                        {
                            response.codError = "1";
                            response.sMessage = "No existen planes para esta combinación";
                        }
                    }
                    else
                    {
                        response.codError = "1";
                        response.sMessage = "No existen planes para esta combinación";
                    }
                }
                else
                {
                    response.codError = "1";
                    response.sMessage = "No existen planes para esta combinación";
                }
            }
            else
            {
                response.codError = "1";
                response.sMessage = "No existen tarifarios para esta combinación";
            }

            LogControl.save("GetPrimaPlan", "Se termina el servicio GetPrimaPlan-Ecommerce", "1", "EC");

            return response;
        }

        public List<coberturaPropuesta> insertCoverRequeridDesDev(validaTramaVM objValida)
        {
            var response = new List<coberturaPropuesta>();
            var lisrCover = new List<Coverage>();

            var cobBM = new CoberturaBM()
            {
                nbranch = objValida.datosPoliza.branch.ToString(),
                tipoProducto = Int32.Parse(objValida.codProducto),
                currency = objValida.datosPoliza.codMon,
                segmentoId = "1"
            };

            var coversCot = quotationCORE.GetCoberturasGenDev(cobBM);

            foreach (var coverCot in coversCot)
            {

                var coversave = new Coverage
                {
                    code = coverCot.codCobertura,
                    id = coverCot.codCobertura,
                    required = coverCot.cobertura_pri == "1" ? true : false,
                    capital = new CoverageValue
                    {
                        @base = coverCot.capital_prop,
                        min = coverCot.capital_min,
                        max = coverCot.capital_max
                    },
                    entryAge = new CoverageValue
                    {
                        @base = Double.Parse(objValida.datosPoliza.edminingreso.ToString()),
                        min = Double.Parse(objValida.datosPoliza.edminingreso.ToString()),
                        max = Double.Parse(objValida.datosPoliza.edmaxingreso.ToString())
                    },
                    stayAge = new CoverageValue
                    {
                        @base = Double.Parse(objValida.datosPoliza.edminpermanencia.ToString()),
                        min = Double.Parse(objValida.datosPoliza.edminpermanencia.ToString()),
                        max = Double.Parse(objValida.datosPoliza.edmaxpermamencia.ToString())
                    },
                    capitalCovered = 0,
                    limit = coverCot.codCobertura == "1154" ? objValida.datosPoliza.sobrevivencia : 0,
                    hours = 24,
                    description = coverCot.descripcion.Trim()
                };

                if (Convert.ToBoolean(coversave.required) == true)
                {
                    lisrCover.Add(coversave);

                    var coverItem = new coberturaPropuesta()
                    {
                        codCobertura = coversave.code,
                        sumaPropuesta = coversave.capital.@base
                    };

                    response.Add(coverItem);
                }

            }

            var responseCover = quotationCORE.insertCoverRequerid(objValida, lisrCover);

            return response;

        }

        public async Task<List<coberturaPropuesta>> insertCoverRequerid(validaTramaVM objValida)
        {
            var response = new List<coberturaPropuesta>();

            var requestPlan = new TariffGraph();
            if (new string[] { ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch") }.Contains(objValida.datosPoliza.branch.ToString()))
            {
                requestPlan = new TariffGraph()
                {
                    nbranch = objValida.datosPoliza.branch.ToString(),
                    currency = objValida.datosPoliza.codMon,
                    channel = objValida.codCanal,
                    billingType = objValida.datosPoliza.codTipoFacturacion,
                    policyType = objValida.datosPoliza.codTipoNegocio,
                    collocationType = objValida.datosPoliza.codTipoModalidad,
                    profile = objValida.datosPoliza.codTipoPerfil
                };
                requestPlan.profile = quotationCORE.getProfileRepeat(requestPlan.nbranch, requestPlan.profile);

                var miniTariff = await quotationCORE.getMiniTariffGraph(requestPlan);

                if (miniTariff.codError == 0)
                {
                    if (miniTariff.data.miniTariffMatrix != null &&
                        miniTariff.data.miniTariffMatrix.segment != null &&
                        miniTariff.data.miniTariffMatrix.segment.modules != null)
                    {
                        if (miniTariff.data.miniTariffMatrix.segment.modules.Count() > 0)
                        {
                            var itemPlan = miniTariff.data.miniTariffMatrix.segment.modules.Where(x => x.id == objValida.datosPoliza.codTipoPlan).ToList();

                            foreach (var item in itemPlan)
                            {
                                var listCover = item.coverages.Where(x => Convert.ToBoolean(x.required) == true && x.status == "ENABLED").ToList();

                                var responseCover = quotationCORE.insertCoverRequerid(objValida, listCover);
                                var listSubItem = listCover.Where(x => x.items != null && x.items.Count > 0).ToList();
                                var responseSubCover = quotationCORE.insertSubItemPD(objValida, listSubItem);

                                foreach (var cover in listCover)
                                {
                                    var coverItem = new coberturaPropuesta()
                                    {
                                        codCobertura = cover.id,
                                        sumaPropuesta = cover.capital.@base
                                    };

                                    response.Add(coverItem);
                                }
                            }
                        }
                    }
                }
            }
            else if (new string[] { ELog.obtainConfig("vidaIndividualBranch") }.Contains(objValida.datosPoliza.branch.ToString()))
            {
                requestPlan = new TariffGraph()
                {
                    nbranch = objValida.datosPoliza.branch.ToString(),
                    currency = objValida.datosPoliza.codMon,
                    channel = objValida.codCanal,
                    billingType = objValida.datosPoliza.codTipoFacturacion,
                    policyType = objValida.datosPoliza.codTipoNegocio,
                    renewalType = objValida.datosPoliza.renewalType,
                    creditType = objValida.datosPoliza.creditType
                };

                var miniTariff = await quotationCORE.getMiniTariffGraphDes(requestPlan);

                if (miniTariff.codError == 0)
                {
                    if (miniTariff.data.miniTariffMatrix != null &&
                        miniTariff.data.miniTariffMatrix.segment != null &&
                        miniTariff.data.miniTariffMatrix.segment.modules != null)
                    {
                        if (miniTariff.data.miniTariffMatrix.segment.modules.Count() > 0)
                        {
                            var itemPlan = miniTariff.data.miniTariffMatrix.segment.modules.Where(x => x.id == objValida.datosPoliza.codTipoPlan).ToList();

                            foreach (var item in itemPlan)
                            {
                                var listCover = item.coverages.Where(x => Convert.ToBoolean(x.optional) == false && x.status == "ENABLED").ToList();

                                foreach (var cover in listCover)
                                {
                                    cover.capital.@base = objValida.datosPoliza.capitalCredito;
                                    var responseCover = quotationCORE.insertCoverRequeridDes(objValida, cover);
                                    if (cover.items != null && cover.items.Count > 0)
                                    {
                                        foreach (var subItem in cover.items)
                                        {
                                            subItem.codeCover = cover.id;
                                            var responseSubCover = quotationCORE.insertSubItemTrx(objValida, subItem);
                                        }
                                    }
                                    var coverItem = new coberturaPropuesta()
                                    {
                                        codCobertura = cover.code,
                                        sumaPropuesta = cover.capital.@base
                                    };

                                    response.Add(coverItem);
                                }
                            }
                        }
                    }
                }
            }
            return response;
        }

        public async Task<List<asistenciaPropuesta>> insertAssitsRequirid(validaTramaVM objValida)
        {
            var response = new List<asistenciaPropuesta>();

            var requestPlan = new TariffGraph();
            if (new string[] { ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch") }.Contains(objValida.datosPoliza.branch.ToString()))
            {

            }
            else if (new string[] { ELog.obtainConfig("vidaIndividualBranch") }.Contains(objValida.datosPoliza.branch.ToString()))
            {
                requestPlan = new TariffGraph()
                {
                    nbranch = objValida.datosPoliza.branch.ToString(),
                    currency = objValida.datosPoliza.codMon,
                    channel = objValida.codCanal,
                    billingType = objValida.datosPoliza.codTipoFacturacion,
                    policyType = objValida.datosPoliza.codTipoNegocio,
                    renewalType = objValida.datosPoliza.renewalType,
                    creditType = objValida.datosPoliza.creditType
                };

                var miniTariff = await quotationCORE.getMiniTariffGraphDes(requestPlan);

                if (miniTariff.codError == 0)
                {
                    if (miniTariff.data.miniTariffMatrix != null &&
                        miniTariff.data.miniTariffMatrix.segment != null &&
                        miniTariff.data.miniTariffMatrix.segment.modules != null)
                    {
                        if (miniTariff.data.miniTariffMatrix.segment.modules.Count() > 0)
                        {
                            var itemPlan = miniTariff.data.miniTariffMatrix.segment.modules.Where(x => x.id == objValida.datosPoliza.codTipoPlan).ToList();

                            foreach (var item in itemPlan)
                            {
                                //if (item.id == data.planId)
                                //{
                                var listAssistances = item.assistances.Where(x => x.status == "ENABLED").ToList();

                                var num = 0;
                                foreach (var assistances in listAssistances)
                                {
                                    if (Convert.ToBoolean(!assistances.optional))
                                    {
                                        // Insertar Coberturas obligatorias
                                        var responseCover = quotationCORE.insertAssistRequeridDes(objValida, assistances);

                                        var assistancesItem = new asistenciaPropuesta()
                                        {
                                            codAsistencia = assistances.code,
                                            desAssist = assistances.description
                                        };

                                        response.Add(assistancesItem);
                                    }
                                }
                                //break;
                                //}
                            }
                        }
                    }
                }
            }
            return response;
        }

        public async Task<List<beneficioPropuesto>> insertBenefitRequirid(validaTramaVM objValida)
        {
            var response = new List<beneficioPropuesto>();

            var requestPlan = new TariffGraph();
            if (new string[] { ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch") }.Contains(objValida.datosPoliza.branch.ToString()))
            {

            }
            else if (new string[] { ELog.obtainConfig("vidaIndividualBranch") }.Contains(objValida.datosPoliza.branch.ToString()))
            {
                requestPlan = new TariffGraph()
                {
                    nbranch = objValida.datosPoliza.branch.ToString(),
                    currency = objValida.datosPoliza.codMon,
                    channel = objValida.codCanal,
                    billingType = objValida.datosPoliza.codTipoFacturacion,
                    policyType = objValida.datosPoliza.codTipoNegocio,
                    renewalType = objValida.datosPoliza.renewalType,
                    creditType = objValida.datosPoliza.creditType
                };

                var miniTariff = await quotationCORE.getMiniTariffGraphDes(requestPlan);

                if (miniTariff.codError == 0)
                {
                    if (miniTariff.data.miniTariffMatrix != null &&
                        miniTariff.data.miniTariffMatrix.segment != null &&
                        miniTariff.data.miniTariffMatrix.segment.modules != null)
                    {
                        if (miniTariff.data.miniTariffMatrix.segment.modules.Count() > 0)
                        {
                            var itemPlan = miniTariff.data.miniTariffMatrix.segment.modules.Where(x => x.id == objValida.datosPoliza.codTipoPlan).ToList();

                            foreach (var item in itemPlan)
                            {
                                //if (item.id == data.planId)
                                //{
                                var listBenefits = item.benefits.Where(x => x.status == "ENABLED").ToList();

                                var num = 0;
                                foreach (var benefit in listBenefits)
                                {
                                    if (Convert.ToBoolean(!benefit.optional))
                                    {
                                        // Insertar Coberturas obligatorias
                                        var responseCover = quotationCORE.insertBenefitRequeridDes(objValida, benefit);

                                        var benefitsItem = new beneficioPropuesto()
                                        {
                                            codBeneficio = benefit.id,
                                            desBenefit = benefit.description
                                        };

                                        response.Add(benefitsItem);
                                    }
                                }
                                //break;
                                //}
                            }
                        }
                    }
                }
            }
            return response;
        }

        public async Task<List<coberturaPropuesta>> insertCoverRequeridRecal(validaTramaVM objValida)
        {
            var response = new List<coberturaPropuesta>();

            var data = new CoberturaBM()
            {
                IdProc = objValida.codProceso,
                codBranch = Int32.Parse(objValida.codRamo),
                tipoProducto = 1,    //Int32.Parse(objValida.datosPoliza.codTipoProducto),
                codPerfil = Int32.Parse(objValida.datosPoliza.codTipoPerfil),
                monedaId = Int32.Parse(objValida.datosPoliza.codMon),
                tipoTransac = objValida.datosPoliza.typeTransac,
                tipoModalidad = 0,
                ciiuId = 0,
                tipoEmpleado = objValida.datosPoliza.type_employee,
                fechaEfecto = objValida.datosPoliza.InicioVigPoliza

            };

            var coversCot = quotationCORE.GetCoberturas(data);

            var requestPlan = new TariffGraph();

            requestPlan = new TariffGraph()
            {
                nbranch = objValida.datosPoliza.branch.ToString(),
                currency = objValida.datosPoliza.codMon,
                channel = objValida.codCanal,
                billingType = objValida.datosPoliza.codTipoFacturacion,
                policyType = objValida.datosPoliza.codTipoNegocio,
                renewalType = objValida.datosPoliza.renewalType,
                creditType = objValida.datosPoliza.creditType
            };

            requestPlan.profile = quotationCORE.getProfileRepeat(requestPlan.nbranch, requestPlan.profile);

            var miniTariff = await quotationCORE.getMiniTariffGraphDes(requestPlan);

            if (miniTariff.codError == 0)
            {
                if (miniTariff.data.miniTariffMatrix != null &&
                    miniTariff.data.miniTariffMatrix.segment != null &&
                    miniTariff.data.miniTariffMatrix.segment.modules != null)
                {
                    if (miniTariff.data.miniTariffMatrix.segment.modules.Count() > 0)
                    {
                        foreach (var item in miniTariff.data.miniTariffMatrix.segment.modules)
                        {
                            if (item.id == objValida.datosPoliza.codTipoPlan)
                            {
                                foreach (var cover in item.coverages)
                                {
                                    if (Convert.ToBoolean(!cover.optional))
                                    {
                                        double monto = cover.capital.min;
                                        foreach (var coverCot in coversCot)
                                        {
                                            if (cover.code == coverCot.codCobertura)
                                            {
                                                monto = objValida.datosPoliza.capitalCredito;
                                                /*
                                                if (coverCot.def == "1")
                                                {
                                                    monto = objValida.datosPoliza.capitalCredito;
                                                }
                                                else
                                                {
                                                    monto = cover.capital.min;
                                                }
                                                */
                                                break;
                                            }
                                        }

                                        var coverItem = new coberturaPropuesta()
                                        {
                                            codCobertura = cover.code,
                                            sumaPropuesta = monto
                                        };

                                        response.Add(coverItem);
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
            }

            return response;
        }

        public async Task<List<asistenciaPropuesta>> insertAssitsRequiridRecal(validaTramaVM objValida)
        {
            var response = new List<asistenciaPropuesta>();

            var requestPlan = new TariffGraph();

            if (new string[] { ELog.obtainConfig("vidaIndividualBranch") }.Contains(objValida.datosPoliza.branch.ToString()))
            {
                requestPlan = new TariffGraph()
                {
                    nbranch = objValida.datosPoliza.branch.ToString(),
                    currency = objValida.datosPoliza.codMon,
                    channel = objValida.codCanal,
                    billingType = objValida.datosPoliza.codTipoFacturacion,
                    policyType = objValida.datosPoliza.codTipoNegocio,
                    renewalType = objValida.datosPoliza.renewalType,
                    creditType = objValida.datosPoliza.creditType
                };

                var miniTariff = await quotationCORE.getMiniTariffGraphDes(requestPlan);

                if (miniTariff.codError == 0)
                {
                    if (miniTariff.data.miniTariffMatrix != null &&
                        miniTariff.data.miniTariffMatrix.segment != null &&
                        miniTariff.data.miniTariffMatrix.segment.modules != null)
                    {
                        if (miniTariff.data.miniTariffMatrix.segment.modules.Count() > 0)
                        {
                            foreach (var item in miniTariff.data.miniTariffMatrix.segment.modules)
                            {
                                if (item.id == objValida.datosPoliza.codTipoPlan)
                                {
                                    foreach (var assistances in item.assistances)
                                    {
                                        if (Convert.ToBoolean(!assistances.optional))
                                        {
                                            // Insertar Asistencias obligatorias
                                            var assistancesItem = new asistenciaPropuesta()
                                            {
                                                codAsistencia = assistances.code,
                                                desAssist = assistances.description
                                            };

                                            response.Add(assistancesItem);
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return response;
        }

        public async Task<List<beneficioPropuesto>> insertBenefitRequiridRecal(validaTramaVM objValida)
        {
            var response = new List<beneficioPropuesto>();

            var requestPlan = new TariffGraph();

            if (new string[] { ELog.obtainConfig("vidaIndividualBranch") }.Contains(objValida.datosPoliza.branch.ToString()))
            {
                requestPlan = new TariffGraph()
                {
                    nbranch = objValida.datosPoliza.branch.ToString(),
                    currency = objValida.datosPoliza.codMon,
                    channel = objValida.codCanal,
                    billingType = objValida.datosPoliza.codTipoFacturacion,
                    policyType = objValida.datosPoliza.codTipoNegocio,
                    renewalType = objValida.datosPoliza.renewalType,
                    creditType = objValida.datosPoliza.creditType
                };

                var miniTariff = await quotationCORE.getMiniTariffGraphDes(requestPlan);

                if (miniTariff.codError == 0)
                {
                    if (miniTariff.data.miniTariffMatrix != null &&
                        miniTariff.data.miniTariffMatrix.segment != null &&
                        miniTariff.data.miniTariffMatrix.segment.modules != null)
                    {
                        if (miniTariff.data.miniTariffMatrix.segment.modules.Count() > 0)
                        {
                            foreach (var item in miniTariff.data.miniTariffMatrix.segment.modules)
                            {
                                if (item.id == objValida.datosPoliza.codTipoPlan)
                                {
                                    foreach (var benefits in item.benefits)
                                    {
                                        if (Convert.ToBoolean(!benefits.optional))
                                        {
                                            // Insertar Beneficios obligatorios
                                            var benefitsItem = new beneficioPropuesto()
                                            {
                                                codBeneficio = benefits.code,
                                                desBenefit = benefits.description
                                            };

                                            response.Add(benefitsItem);
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return response;
        }

        public async Task<List<recargoPropuesto>> insertSurchargeRequiridRecal(validaTramaVM objValida)
        {
            var response = new List<recargoPropuesto>();

            var requestPlan = new TariffGraph();

            if (new string[] { ELog.obtainConfig("vidaIndividualBranch") }.Contains(objValida.datosPoliza.branch.ToString()))
            {
                requestPlan = new TariffGraph()
                {
                    nbranch = objValida.datosPoliza.branch.ToString(),
                    currency = objValida.datosPoliza.codMon,
                    channel = objValida.codCanal,
                    billingType = objValida.datosPoliza.codTipoFacturacion,
                    policyType = objValida.datosPoliza.codTipoNegocio,
                    renewalType = objValida.datosPoliza.renewalType,
                    creditType = objValida.datosPoliza.creditType
                };

                var miniTariff = await quotationCORE.getMiniTariffGraphDes(requestPlan);

                if (miniTariff.codError == 0)
                {
                    if (miniTariff.data.miniTariffMatrix != null &&
                        miniTariff.data.miniTariffMatrix.segment != null)
                    {
                        if (miniTariff.data.miniTariffMatrix.segment.surcharges.Count() > 0)
                        {
                            var listSurcharge = miniTariff.data.miniTariffMatrix.segment.surcharges.Where(x => Convert.ToBoolean(x.optional) == false && x.status == "ENABLED").ToList();

                            foreach (var item in listSurcharge)
                            {
                                if (Convert.ToBoolean(!item.optional))
                                {
                                    // Insertar Beneficios obligatorios
                                    var surchareItem = new recargoPropuesto()
                                    {
                                        codRecargo = item.id,
                                        desRecargo = item.description
                                    };

                                    response.Add(surchareItem);
                                }
                                if (item.code == "2") // Código de recargo Comisión de broker / corredor
                                {
                                    objValida.datosPoliza.commissionBroker = item.value.amount;
                                }

                            }
                        }
                    }
                }
            }
            return response;
        }

        [Route("CalculoPrimaEC")]
        [HttpPost]
        public async Task<ReponsePrimaPlanVM> CalculoPrimaEC(validaTramaVM data)
        {
            LogControl.save("CalculoPrimaEC", JsonConvert.SerializeObject(data, Formatting.None), "2", "EC");
            var response = new ReponsePrimaPlanVM();

            data = await ordenarDataCoberturas(data);

            var responseDet = quotationCORE.regDetallePlanCot(data);

            data.codAplicacion = "EC";
            data.tipoCotizacion = "PRIME";
            //data.datosPoliza.idTariff = data.datosPoliza.id_tarifario;
            //data.datosPoliza.versionTariff = data.datosPoliza.version_tarifario;

            if (responseDet.P_COD_ERR == 0)
            {
                // Acá se ha validado todo correctamente - Llamamos a Tarifario AP
                var responseTarifario = new ResponseGraph() { codError = 0 };

                // Genera documento
                if (!quotationCORE.validarAforo(data.datosPoliza.branch.ToString(), data.datosPoliza.codTipoPerfil, data.datosPoliza.trxCode))
                {
                    var aseguradoList = new QuotationCORE().GetTramaAsegurado(data.codProceso);
                    //var aseguradoList = new QuotationCORE().GetTramaAseguradoPD(data.codProceso);

                    data.ruta = generarAseguradosJson(data, aseguradoList);
                    LogControl.save("CalculoPrimaEC", JsonConvert.SerializeObject(data, Formatting.None), "2", "EC");
                }

                responseTarifario = await new QuotationCORE().SendDataTarifarioGraphql(data);

                if (responseTarifario.codError == 0)
                {
                    response.codError = responseTarifario.codError.ToString();
                    response.sMessage = responseTarifario.message == null ? "Se ha consumido correctamente el tarifario." : responseTarifario.message;
                    //response.sPremium = responseTarifario.data != null ? responseTarifario.data.searchPremium.commercialPremium.ToString() : "0";

                    data.premium = responseTarifario.data != null ? responseTarifario.data.searchPremium.commercialPremium : 0.0;
                    data.igv = responseTarifario.data != null ? responseTarifario.data.searchPremium.totalValueWithIgv : 0.0;
                    data.premiumTotal = responseTarifario.data != null ? responseTarifario.data.searchPremium.totalPremium : 0.0;
                    data.asegPremium = responseTarifario.data != null ? responseTarifario.data.searchPremium.unitCommercialPremium : 0.0;
                    data.asegIgv = responseTarifario.data != null ? responseTarifario.data.searchPremium.igvUnitValue : 0.0;
                    data.asegPremiumTotal = responseTarifario.data != null ? responseTarifario.data.searchPremium.unitCommercialPremiumWithIgv : 0.0;

                    //var resIGV = new QuotationCORE().DeleteIgv(data);
                    //premium = resIGV.npremiumn;

                    #region Simular prima
                    //if (request.premium == 0)
                    //{
                    //    var array = new int[] { 0, 12, 6, 3, 2, 1, 1 };
                    //    request.premium = (100 + ((13 * request.lcoberturas.Count) * 0.75314) + (request.lasistencias.Count * 10) + (request.lbeneficios.Count * 10)) * array[Convert.ToInt32(request.datosPoliza.codTipoFrecuenciaPago)];
                    //}
                    #endregion

                    //resAuth = request.premium > 0 ? resAuth : quotationCORE.getAuthDerivation(request); // Validación de datos de asegurados.

                    if (data.premium > 0)
                    {
                        var response_error = new SalidaErrorBaseVM() { P_COD_ERR = 0 };

                        // si tipo de perfil es igual AFORO entra
                        if (quotationCORE.validarAforo(data.datosPoliza.branch.ToString(), data.datosPoliza.codTipoPerfil, data.datosPoliza.trxCode))
                        {
                            response_error = quotationCORE.InsCargaMasEspecial(data);
                        }

                        if (response_error.P_COD_ERR == 0)
                        {
                            var resIn = new SalidaTramaBaseVM();
                            //data.premium = premium;
                            resIn = await genericProcessPD(data, resIn, 1, null, "recalcular");

                            if (response.codError == "0")
                            {
                                //if (!Generic.valRentaEstudiantilVG(data.datosPoliza.branch.ToString(), data.datosPoliza.codTipoProducto, data.datosPoliza.codTipoPerfil))
                                //{
                                //    response.sPremium = responseTarifario.data != null ? responseTarifario.data.searchPremium.commercialPremium.ToString() : "0";
                                //    //var resIGV2 = new QuotationCORE().DeleteIgv(data);
                                //    //response.sPremium = resIGV2.npremiumn.ToString();
                                //}
                                //else
                                //{
                                var res = quotationCORE.ReaListCotiza(data.codProceso, data.datosPoliza.codTipoRenovacion, data.datosPoliza.codTipoProducto, data.datosPoliza.branch.ToString());
                                LogControl.save("CalculoPrimaEC", JsonConvert.SerializeObject(res, Formatting.None), "2", "EC");
                                response.sPremium = res.PRIMA.ToString();
                                //}
                            }
                            else
                            {
                                response.codError = "1";
                                response.sMessage = "Hubo un inconveniente al momento de realizar los cálculos.";
                                response.sPremium = "0";
                            }
                        }
                        else
                        {
                            response.codError = "1";
                            response.sMessage = "Hubo un error al calcular la prima";
                            response.sPremium = "0";
                            LogControl.save("CalculoPrimaEC", JsonConvert.SerializeObject(response, Formatting.None), "2", "EC");
                        }
                    }
                    else
                    {
                        response.codError = "1";
                        //response.sMessage = "Hubo un error al calcular la prima";
                        response.sPremium = "0";
                        LogControl.save("CalculoPrimaEC", JsonConvert.SerializeObject(response, Formatting.None), "2", "EC");
                    }
                }
                else
                {
                    response.codError = responseTarifario.codError.ToString();
                    response.sMessage = responseTarifario.message;
                    response.sPremium = "0";
                }

                //response = quotationCORE.CalculoPrimaEC(data);
            }
            else
            {
                response.codError = "1";
                response.sMessage = responseDet.P_MESSAGE;
            }

            return response;
        }

        [Route("GetCoberturasEC")]
        [HttpPost]
        public async Task<List<CoberturasVM>> GetCoberturasEC(PrimaPlanBM data)
        {
            LogControl.save("GetCoberturasEC", JsonConvert.SerializeObject(data, Formatting.None), "2", "EC");

            var request = new CoberturaBM()
            {
                IdProc = null,
                codBranch = Convert.ToInt32(data.codBranch),
                codPerfil = Convert.ToInt32(data.codPerfil),
                planId = data.codPlan,
                monedaId = Convert.ToInt32(data.codCurrency),
                tipoTransac = "1",
                tipoModalidad = Convert.ToInt32(data.codTypeMod),
                ciiuId = 0,
                tipoEmpleado = null,
                fechaEfecto = data.dateEffect,
                channel = data.channel,
                billingType = data.billingType,
                policyType = data.codTypePolicy,
                tipoProducto = Convert.ToInt32(data.codTypePolicy),
                collocationType = data.codTypeMod,
                profile = data.codPerfil.ToString(),
                idTariff = data.idTariff,
                versionTariff = data.versionTariff
            };

            var result = await GetCoberturas(request);

            return result;
        }

        // Receipt cotizador tecnica
        [Route("UpdateStatusQuotationPD")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateStatusQuotationPD(UpdateCotRequestBM request)
        {
            var response = new ResponseUpdateVM();
            try
            {
                //ELogWS.save("Step 0: Inicio del metodo");
                //var sd = JsonConvert.SerializeObject(request, Formatting.Indented);
                LogControl.save(request.requestId, "UpdateStatusQuotationPD Ini: " + JsonConvert.SerializeObject(request, Formatting.None), "2");

                if (request != null)
                {
                    // Guarda log en tbl
                    quotationCORE.InsertLog(Convert.ToInt64(request.quotationNumber), "01 - Recepción de info de Devmente", "UpdateStatusQuotationPD", JsonConvert.SerializeObject(request), null);

                    //if (ModelState.IsValid)
                    //{
                    if (quotationCORE.ValEstadoCotizacion(request.quotationNumber))
                    {
                        //ELogWS.save(Request.RequestUri.AbsoluteUri);
                        // Trae codigo de usuario a partir de la descripcion de este
                        request.userCode = string.IsNullOrEmpty(request.userCode) ? new QuotationCORE().GetCodeuserByUsername(request.userName) : request.userCode;

                        if (!String.IsNullOrWhiteSpace(request.userCode))
                        {
                            if (new int[] { 13, 11 }.Contains(request.stateRequest))
                            {
                                ThreadStart starter = delegate { UpdateStatusPDAsync(request, request.userCode); };
                                Thread threads = new Thread(starter);
                                threads.Start();

                                response = new ResponseUpdateVM()
                                {
                                    state = true,
                                    nroCotizacion = request.quotationNumber,
                                    codError = "0",
                                    errors = new List<string>()
                                        {
                                            "Se realizó todo correctamente"
                                        }
                                };
                            }
                            else
                            {
                                response = new ResponseUpdateVM()
                                {
                                    state = false,
                                    nroCotizacion = request.quotationNumber,
                                    errors = new List<string>()
                                    {
                                        "El valor del stateRequest enviado no se encuentra entre los permitidos {11, 13}. El valor enviado fue: " + request.stateRequest
                                    }
                                };
                            }
                        }
                        else
                        {
                            quotationCORE.InsertLog(Convert.ToInt32(request.quotationNumber), "02 - Validación: No se encontró usuario", "UpdateStatusQuotationPD", JsonConvert.SerializeObject(request, Formatting.None), "No se encontró usuario relacionado en la base de datos.");
                            response = new ResponseUpdateVM()
                            {
                                state = false,
                                nroCotizacion = request.quotationNumber,
                                errors = new List<string>()
                                {
                                    "No se encontró usuario relacionado en la base de datos."
                                }
                            };
                        }
                    }
                    else
                    {
                        response = new ResponseUpdateVM()
                        {
                            state = true,
                            nroCotizacion = request.quotationNumber,
                            codError = "0",
                            errors = new List<string>()
                                {
                                    "La cotización ya se encuentra aprobada / rechazada."
                                }
                        };
                    }
                    //}
                    //else
                    //{
                    //    response = new ResponseUpdateVM()
                    //    {
                    //        state = false,
                    //        nroCotizacion = request.quotationNumber,
                    //        errors = new List<string>()
                    //        {
                    //            "Error de ModalState "
                    //        }.Concat(ModelState.Values.Select(s => s.Errors.Select(x => x.ErrorMessage).FirstOrDefault())).ToList()
                    //    };
                    //}
                    quotationCORE.InsertLog(Convert.ToInt64(request.quotationNumber), "99 - Se realiza envio a devmente de forma async", "UpdateStatusQuotationPD", JsonConvert.SerializeObject(request), JsonConvert.SerializeObject(response));
                }
                else
                {
                    response = new ResponseUpdateVM()
                    {
                        state = false,
                        nroCotizacion = String.Empty,
                        errors = new List<string>()
                        {
                            "No se encontró algún dato ",
                            new ArgumentNullException(nameof(request)).ToString()
                        }
                    };
                }

                // Guarda log en tbl
            }
            catch (Exception ex)
            {
                LogControl.save("UpdateStatusQuotationPD", ex.ToString(), "3");
                response = new ResponseUpdateVM()
                {
                    state = false,
                    nroCotizacion = request.quotationNumber,
                    errors = new List<string>()
                    {
                       ex.ToString()
                    }
                };

                quotationCORE.InsertLog(Convert.ToInt64(request.quotationNumber), "99 - Error en actualizacion (Cotizador 2)", "UpdateStatusQuotationPD", ex.ToString(), JsonConvert.SerializeObject(response));
            }

            return await Task.FromResult(Ok(response));
        }

        public void UpdateStatusPDAsync(UpdateCotRequestBM request, string userCode)
        {
            Task.Run(async () =>
            {
                Task<ResponseUpdateVM> TBool = updateStatusPD(request, userCode);
            }).GetAwaiter().GetResult();
        }

        private async Task<ResponseUpdateVM> updateStatusPD(UpdateCotRequestBM request, string userCode)
        {
            var response = new ResponseUpdateVM();
            var dataDetalle = new validaTramaVM();

            try
            {
                LogControl.save(request.requestId, "UpdateStatusQuotationPD Parte 2 -> request: " + JsonConvert.SerializeObject(request, Formatting.None), "2");

                // Crea objeto para ejecutar sp que actualiza cotizacion
                // Trae datos de la cotizacion
                var tranTecnica = request.type == "RENOVATION" ? "4" : request.type == "INCLUSION" ? "2" : "1";
                var dataQuotation = await new PolicyController().GetPolizaEmitCab(request.quotationNumber, tranTecnica, Convert.ToInt32(request.userCode), 0, 0, quotationCORE.ValidardataTrama(request.requestId) > 0 ? 1 : 0);
                var brokers = new PolicyController().GetPolizaEmitComer(Convert.ToInt32(request.quotationNumber));

                // Eliminar Coberturas, Asistencias y Beneficios por codigo de proceso
                // Insertar Coberturas,Asistencias y Beneficios por nroCotizacion
                var generic = (Entities.PolicyModel.ViewModel.PolizaEmitCAB)dataQuotation.GenericResponse;
                dataDetalle = new validaTramaVM()
                {
                    codProceso = !string.IsNullOrEmpty(request.requestId) ? request.requestId : dataQuotation.idproc,
                    type_mov = generic.NTYPE_TRANSAC, // Traer transac pendiente
                    codUsuario = request.userCode,
                    flagCalcular = 0,
                    premium = request.commercialPremium > 0 ? request.commercialPremium : dataQuotation.amountPremiumListAnu[3].NPREMIUMN_ANU,
                    igv = request.totalValueWithIgv,
                    premiumTotal = request.totalPremium,
                    asegPremium = request.unitCommercialPremium,
                    asegIgv = request.igvUnitValue,
                    asegPremiumTotal = request.unitCommercialPremiumWithIgv, //Math.Round(request.unitCommercialPremium, 2) + Math.Round(request.igvUnitValue, 2), // este parametro aun no llega segun correo de Cinthya 
                    nroCotizacion = Convert.ToInt64(request.quotationNumber),
                    flagCot = "0",
                    flagpremium = 0,
                    flag = generic.POLIZA_MATRIZ == "1" ? "0" : null,
                    PolizaMatriz = generic.POLIZA_MATRIZ == "1" ? generic.POLIZA_MATRIZ : "false",
                    CantidadTrabajadores = Convert.ToInt64(dataQuotation.total_asegurados),
                    codAplicacion = "PD",
                    ntecnica = 1,
                    tipoCotizacion = generic.STIPO_COTIZACION, // == request.quotationType ? request.quotationType : generic.STIPO_COTIZACION,
                                                               // flagTrama: request.dataFrame,
                    detailId = request.detailId,
                    ntasa_tariff = request.rate > 0 ? Convert.ToDouble(decimal.Round(Convert.ToDecimal(request.rate * 100), 2)) : Convert.ToDouble(generic.NTASA_AUTOR), //AVS - RENTAS
                    montoPlanilla = Convert.ToInt64(generic.NMONTO_PLANILLA),
                    flagTrama = quotationCORE.ValidardataTrama(request.requestId) > 0 ? "1" : "0", //generic.SVALIDA_TRAMA,
                    flagTramaOrigen = generic.SVALIDA_TRAMA,
                    datosContratante = new DatosContractor()
                    {
                        codContratante = generic.SCLIENT
                    },
                    datosPoliza = new Entities.QuotationModel.BindingModel.DatosPoliza()
                    {
                        branch = generic.NBRANCH,
                        codTipoProducto = generic.NPRODUCT.ToString(),
                        codMon = request.currency,
                        codTipoRenovacion = generic.TIP_RENOV,
                        codTipoFrecuenciaPago = generic.FREQ_PAGO,
                        codTipoNegocio = generic.NID_TYPE_PRODUCT, // request.policyType,// generic.NID_TYPE_PRODUCT,
                        codTipoPerfil = generic.NID_TYPE_PROFILE,
                        codTipoModalidad = generic.NID_TYPE_MODALITY, // request.colocationType, // generic.NID_TYPE_MODALITY,
                        codTipoPlan = request.module,
                        codTipoFacturacion = generic.COD_TIPO_FACTURACION, //request.billingType, // generic.COD_TIPO_FACTURACION,
                        CodActividadRealizar = request.activity, // generic.ACT_TEC_VL,
                        InicioVigPoliza = Convert.ToDateTime(request.policyInitDate).ToString("dd/MM/yyyy"),
                        FinVigPoliza = Convert.ToDateTime(request.policyEndDate).ToString("dd/MM/yyyy"),
                        InicioVigAsegurado = Convert.ToDateTime(request.insuredInitDate).ToString("dd/MM/yyyy"),
                        FinVigAsegurado = Convert.ToDateTime(request.insuredEndDate).ToString("dd/MM/yyyy"),
                        typeTransac = generic.NTYPE_TRANSAC,
                        trxCode = request.type.Substring(0, 2),
                        idTariff = request.tariffId,
                        versionTariff = request.tariffVersion.ToString(),
                        type_employee = "0", //AVS - RENTAS
                        nid_cotizacion = (int)Convert.ToInt64(request.quotationNumber),
                    },
                    lcoberturas = request.coberturasList,
                    lbeneficios = request.beneficiosList,
                    lasistencias = request.asistenciasList,
                    lservAdicionales = request.servAdicionalesList
                };

                dataDetalle.igvPD = new QuotationCORE().GetIGV(dataDetalle) / 100;

                //var resIGV = new QuotationCORE().DeleteIgv(dataDetalle);
                //dataDetalle.premium = resIGV.npremiumn;

                LogControl.save(request.requestId, "UpdateStatusQuotationPD Parte 3 -> dataDetalle: " + JsonConvert.SerializeObject(dataDetalle, Formatting.None), "2");

                quotationCORE.InsertLog(Convert.ToInt64(request.quotationNumber), "02 - Creación de objeto dataDetalle", "UpdateStatusQuotationPD", JsonConvert.SerializeObject(dataDetalle), null);

                var response_error = new SalidaErrorBaseVM() { P_COD_ERR = 0 };

                if ((request.coberturasList != null && request.coberturasList.Count > 0) ||
                    (request.asistenciasList != null && request.asistenciasList.Count > 0) ||
                    (request.beneficiosList != null && request.beneficiosList.Count > 0) ||
                    (request.recargosList != null && request.recargosList.Count > 0) ||
                    (request.exclusionList != null && request.exclusionList.Count > 0) ||
                    (request.servAdicionalesList != null && request.servAdicionalesList.Count > 0)) //AVS - RENTAS
                {
                    dataDetalle = await ordenarDataCoberturas(dataDetalle);

                    response_error = new QuotationCORE().regDetallePlanCot(dataDetalle, 1, request.coberturasList != null && request.exclusionList.Count > 0 ? 1 : 0);
                    quotationCORE.InsertLog(Convert.ToInt64(request.quotationNumber), "03 - Se elimina y registra info de la cotización", "UpdateStatusQuotationPD", JsonConvert.SerializeObject(response_error), null);

                    if (response_error.P_COD_ERR == 0)
                    {
                        var list = new List<recargoPropuesto>();
                        foreach (var item in request.recargosList)
                        {
                            var itemR = new recargoPropuesto()
                            {
                                codRecargo = item.code,
                                desRecargo = item.description,
                                amount = item.rate,
                                esBeneficio = Convert.ToBoolean(item.optional)
                            };
                            list.Add(itemR);
                        }

                        var responseRecargo = await InsertRecargos(dataDetalle, list, 1);
                        response_error = new SalidaErrorBaseVM()
                        {
                            P_COD_ERR = Convert.ToInt32(responseRecargo.P_COD_ERR),
                            P_MESSAGE = responseRecargo.P_MESSAGE
                        };
                    }

                    if (response_error.P_COD_ERR == 0 && (request.exclusionList != null && request.exclusionList.Count > 0))
                    {
                        var responseExclusion = await InsertExclusiones(dataDetalle, request.exclusionList);
                        response_error = new SalidaErrorBaseVM()
                        {
                            P_COD_ERR = Convert.ToInt32(responseExclusion.P_COD_ERR),
                            P_MESSAGE = responseExclusion.P_MESSAGE
                        };
                    }
                }

                double comision = 0;
                // Ejecutan sp de actualizacion y luego evalua si hay error
                if (response_error.P_COD_ERR == 0)
                {
                    var itemBroker = request.recargosList.FirstOrDefault(x => x.code == "2");
                    var itemInter = request.recargosList.FirstOrDefault(x => x.code == "3");

                    comision = brokers[0].NINTERTYP == "1" ? 0 : brokers[0].NINTERTYP == "3" ? itemBroker != null ? itemBroker.rate : 0 : itemInter != null ? itemInter.rate : 0;

                    #region creacion de objeto broker
                    var broker = new Entities.QuotationModel.BindingModel.BrokerBM[] {
                                        new Entities.QuotationModel.BindingModel.BrokerBM()
                                        {
                                            Id = brokers[0].CANAL,
                                            ProductList = new BrokerProductBM[]
                                            {
                                                new BrokerProductBM()
                                                {
                                                    Product = generic.NPRODUCT.ToString(),
                                                    AuthorizedCommission = comision
                                                }
                                            }
                                        }
                                    };
                    #endregion

                    var data = new StatusChangeBM()
                    {
                        QuotationNumber = request.quotationNumber,
                        Status = request.stateRequest,
                        Reason = request.stateRequest == 13 ? 5 : 0,
                        Comment = request.comments,
                        User = request.userCode,
                        Product = generic.NPRODUCT,
                        Nbranch = generic.NBRANCH,
                        path = string.Empty,
                        Gloss = null,
                        GlossComment = string.Empty,
                        saludAuthorizedRateList = null,
                        pensionAuthorizedRateList = null,
                        brokerList = comision > 0 ? broker : new Entities.QuotationModel.BindingModel.BrokerBM[] { },
                        Flag = 0,
                        Comment2 = request.comments2
                    };

                    var responseHist = new QuotationCORE().ChangeStatusVL(data, null);
                    // response es resp
                    // en este objeto debe enviarse la nueva prima enviada por devmente 
                    if (responseHist.ErrorCode == 0)
                    {
                        var responseCot = new SalidaTramaBaseVM() { P_COD_ERR = "0" };

                        if (request.coberturasList != null && request.coberturasList.Count > 0)
                        {
                            if ((request.quotationType == "PRIME" && request.commercialPremium > 0) || (request.quotationType == "RATE" && request.rate > 0))
                            {
                                var flagTecnica = 1; //AVS - RENTAS
                                var flagTecnicaACT = 2; //AVS - RENTAS
                                var requestPT = new ProcesaTramaBM();
                                generateValuesPremium(null, ref dataDetalle);

                                if ((dataDetalle.tipoCotizacion == "PRIME" && dataDetalle.premium > 0) || (dataDetalle.tipoCotizacion == "RATE" && dataDetalle.ntasa_tariff > 0))
                                {
                                    if (generic.POLIZA_MATRIZ == "1" && new string[] { "1", "4" }.Contains(dataDetalle.type_mov) && quotationCORE.ValidardataTrama(dataDetalle.codProceso) == 0)
                                    {
                                        responseCot = responseCot.P_COD_ERR == "0" ? quotationCORE.InsReCotizaTrama(dataDetalle) : responseCot;
                                    }
                                    else
                                    {
                                        if (dataDetalle.flagTrama == "1" && dataDetalle.flagTramaOrigen == "0")
                                        {
                                            requestPT.idproc = dataDetalle.codProceso;
                                            requestPT.nrocotizacion = generic.NID_COTIZACION;
                                            requestPT.p_sreceipt_ind = "0";
                                            requestPT.p_sflag_fac_ant = generic.FACTURA_ANTICIPADA;
                                            requestPT.idpayment = 0;
                                            requestPT.ruta = string.Empty;
                                            requestPT.coltimre = dataDetalle.datosPoliza.codTipoRenovacion;
                                            requestPT.mov_anul = "0";
                                            requestPT.fechaini_poliza = dataDetalle.datosPoliza.InicioVigPoliza;
                                            requestPT.fechafin_poliza = dataDetalle.datosPoliza.FinVigPoliza;
                                            requestPT.fechaini_aseg = dataDetalle.datosPoliza.InicioVigAsegurado;
                                            requestPT.fechafin_aseg = dataDetalle.datosPoliza.FinVigAsegurado;
                                            requestPT.usercode = dataDetalle.codUsuario;
                                            requestPT.typeTransact = "14";
                                            requestPT.idfrecuencia_pago = dataDetalle.datosPoliza.codTipoFrecuenciaPago;
                                            requestPT.nbranch = generic.NBRANCH.ToString();
                                            requestPT.nproducto = generic.NPRODUCT.ToString();
                                            requestPT.rate_cal = "0";
                                            requestPT.rate_prop = "0";
                                            requestPT.npremium_min = "0";
                                            requestPT.npremium_min_pro = "0";
                                            requestPT.npremium_end = "0";
                                            requestPT.nrate = "0";
                                            requestPT.ndiscount = "0";
                                            requestPT.nvariaction = "0";
                                            requestPT.nflag = "0";
                                            requestPT.namo_afec = dataDetalle.premium;
                                            requestPT.niva = dataDetalle.igv;
                                            requestPT.namount = dataDetalle.premiumTotal;
                                            requestPT.nde = 0;
                                            requestPT.sid_tarifario = dataDetalle.datosPoliza.idTariff;
                                            requestPT.nversion_tarifario = dataDetalle.datosPoliza.versionTariff;
                                            requestPT.policy = Convert.ToDouble(generic.NPOLICY);

                                            dataDetalle.flagCot = "2";

                                            var valCot = quotationDA.ValCotizacion(Convert.ToInt32(generic.NID_COTIZACION), Convert.ToInt32(dataDetalle.codUsuario), 0, 0, null, null);
                                            var resProcesaTrama = quotationCORE.transferDataPM(requestPT.idproc);
                                            resProcesaTrama = resProcesaTrama.cod_error == 0 ? quotationCORE.UpdCotizacionDet(requestPT) : resProcesaTrama;
                                            responseCot = responseCot.P_COD_ERR == "0" ? quotationCORE.DeleteInsuredCot(dataDetalle) : responseCot; // Metododo que hace delete a la trama si existe (TBL_PD_LOAD_INSURED_PREMIUM)
                                            responseCot = responseCot.P_COD_ERR == "0" ? dataDetalle.tipoCotizacion == "RATE" ? quotationCORE.InsertInsuredCotRentEst(dataDetalle) : quotationCORE.InsertInsuredCot(dataDetalle) : responseCot; // Metodo que inserta valor de prima por asegurado (TBL_PD_LOAD_INSURED_PREMIUM )
                                            responseCot = responseCot.P_COD_ERR == "0" ? !string.IsNullOrEmpty(dataDetalle.url_student) ? updateBeneficiariosRE(dataDetalle) : responseCot : responseCot;
                                            responseCot = responseCot.P_COD_ERR == "0" ? quotationCORE.InsCotizaTrama(dataDetalle, flagTecnicaACT) : responseCot;

                                            var dataPrimaAct = new updateCotizador()
                                            {
                                                codProceso = dataQuotation.idproc,
                                                nroCotizacion = request.quotationNumber,
                                                codRamo = generic.NBRANCH,
                                                codProducto = generic.NPRODUCT,
                                                codUsuario = request.userCode,
                                                codEstado = request.stateRequest,
                                                polMatriz = generic.POLIZA_MATRIZ,
                                                fechaEfecto = Convert.ToDateTime(request.policyInitDate).ToString("dd/MM/yyyy"),
                                                rate = dataDetalle.ntasa_tariff,
                                                detailId = request.detailId
                                            };

                                            responseCot = responseCot.P_COD_ERR == "0" ? quotationCORE.updatePrimaCotizador(dataPrimaAct) : responseCot;

                                            var resProcesaTransac = quotationCORE.TRAS_CARGA_ASE(requestPT);
                                            resProcesaTransac = resProcesaTransac.cod_error == 0 ? quotationCORE.INS_JOB_CLOSE_SCTR(requestPT) : resProcesaTransac;
                                            resProcesaTransac = resProcesaTransac.cod_error == 0 ? quotationCORE.Update_header(requestPT.idproc) : resProcesaTransac;
                                            resProcesaTransac = resProcesaTransac.cod_error == 0 ? quotationCORE.UpdateInsuredPremium_Matriz(requestPT, flagTecnica) : resProcesaTransac;
                                        }
                                        else
                                        {
                                            if (dataDetalle.tipoCotizacion == "RATE" && (dataDetalle.flagTramaOrigen == "0" && dataDetalle.flagTrama == "0"))
                                            {
                                                responseCot = responseCot.P_COD_ERR == "0" ? quotationCORE.InsReCotizaTrama(dataDetalle) : responseCot;
                                            }
                                            else
                                            {
                                                responseCot = responseCot.P_COD_ERR == "0" ? quotationCORE.DeleteInsuredCot(dataDetalle) : responseCot; // Metododo que hace delete a la trama si existe (TBL_PD_LOAD_INSURED_PREMIUM)
                                                responseCot = responseCot.P_COD_ERR == "0" ? dataDetalle.tipoCotizacion == "RATE" ? quotationCORE.InsertInsuredCotRentEst(dataDetalle) : quotationCORE.InsertInsuredCot(dataDetalle) : responseCot; // Metodo que inserta valor de prima por asegurado (TBL_PD_LOAD_INSURED_PREMIUM )
                                                responseCot = responseCot.P_COD_ERR == "0" ? !string.IsNullOrEmpty(dataDetalle.url_student) ? updateBeneficiariosRE(dataDetalle) : responseCot : responseCot;
                                                responseCot = responseCot.P_COD_ERR == "0" ? quotationCORE.InsCotizaTrama(dataDetalle, flagTecnica) : responseCot;
                                            }
                                        }
                                    }

                                    quotationCORE.InsertLog(Convert.ToInt64(request.quotationNumber), "04 - Se actualiza la prima por asegurados", "UpdateStatusQuotationPD", JsonConvert.SerializeObject(responseCot), null);
                                }

                                var dataPrima = new updateCotizador()
                                {
                                    codProceso = dataQuotation.idproc,
                                    nroCotizacion = request.quotationNumber,
                                    codRamo = generic.NBRANCH,
                                    codProducto = generic.NPRODUCT,
                                    codUsuario = request.userCode,
                                    codEstado = request.stateRequest,
                                    polMatriz = generic.POLIZA_MATRIZ,
                                    fechaEfecto = Convert.ToDateTime(request.policyInitDate).ToString("dd/MM/yyyy"),
                                    rate = dataDetalle.ntasa_tariff,
                                    detailId = request.detailId
                                };

                                LogControl.save(request.requestId, "UpdateStatusQuotationPD Parte 4 -> Creacion de objeto dataPrima: " + JsonConvert.SerializeObject(dataPrima, Formatting.None), "2");

                                responseCot = responseCot.P_COD_ERR == "0" ? quotationCORE.updatePrimaCotizador(dataPrima) : responseCot;
                                quotationCORE.InsertLog(Convert.ToInt64(request.quotationNumber), "05 - Se actualiza la prima general", "UpdateStatusQuotationPD", JsonConvert.SerializeObject(responseCot), null);

                                response = new ResponseUpdateVM()
                                {
                                    state = responseCot.P_COD_ERR == "0" ? true : false,
                                    nroCotizacion = request.quotationNumber,
                                    branch = dataDetalle.datosPoliza.branch.ToString(),
                                    id = request.id,
                                    stateRequest = responseCot.P_COD_ERR == "0" ? request.stateRequest == 13 ? "APPROVED" : "REJECTED" : "PENDING",
                                    codError = responseCot.P_COD_ERR,
                                    errors = new List<string>()
                                        {
                                             responseCot.P_COD_ERR == "0" ? "Se realizó todo correctamente" : responseCot.P_MESSAGE
                                        }
                                };

                                if (dataDetalle.flagTrama == "1" && dataDetalle.flagTramaOrigen == "0" && requestPT.typeTransact == "14") //AVS - RENTAS
                                {
                                    var datatecnica = quotationCORE.InsCotizaHisTecnica(Convert.ToInt32(request.quotationNumber));
                                }
                            }
                            else
                            {

                                response = new ResponseUpdateVM()
                                {
                                    state = false,
                                    nroCotizacion = request.quotationNumber,
                                    errors = new List<string>()
                                            {
                                                request.quotationType == "RATE" ? "Es obligatorio enviar la tasa para una cotización de tipo : RATE" : "Es obligatorio enviar la prima para una cotización de tipo : PRIME"
                                            }
                                };

                            }
                        }
                        else
                        {
                            var dataPrima = new updateCotizador()
                            {
                                codProceso = dataQuotation.idproc,
                                nroCotizacion = request.quotationNumber,
                                codRamo = generic.NBRANCH,
                                codProducto = generic.NPRODUCT,
                                codUsuario = request.userCode,
                                codEstado = request.stateRequest,
                                polMatriz = generic.POLIZA_MATRIZ,
                                fechaEfecto = Convert.ToDateTime(request.policyInitDate).ToString("dd/MM/yyyy"),
                                rate = dataDetalle.ntasa_tariff,
                                detailId = request.detailId
                            };
                            responseCot = responseCot.P_COD_ERR == "0" ? quotationCORE.updatePrimaCotizador(dataPrima) : responseCot;
                            quotationCORE.InsertLog(Convert.ToInt64(request.quotationNumber), "05 - Se actualiza la prima general", "UpdateStatusQuotationPD", JsonConvert.SerializeObject(responseCot), null);

                            response = new ResponseUpdateVM()
                            {
                                state = responseCot.P_COD_ERR == "0" ? true : false,
                                nroCotizacion = request.quotationNumber,
                                branch = dataDetalle.datosPoliza.branch.ToString(),
                                id = request.id,
                                stateRequest = responseCot.P_COD_ERR == "0" ? request.stateRequest == 13 ? "APPROVED" : "REJECTED" : "PENDING",
                                codError = responseCot.P_COD_ERR,
                                errors = new List<string>()
                                        {
                                             responseCot.P_COD_ERR == "0" ? "Se realizó todo correctamente" : responseCot.P_MESSAGE
                                        }
                            };
                        }
                    }
                    else
                    {
                        response = new ResponseUpdateVM()
                        {
                            state = false,
                            nroCotizacion = request.quotationNumber,
                            codError = responseHist.ErrorCode.ToString(),
                            branch = dataDetalle.datosPoliza.branch.ToString(),
                            id = request.id,
                            stateRequest = "PENDING",
                            errors = new List<string>()
                                        {
                                            "Hubo un error al cambiar el estado de la transacción."
                                        }
                        };
                    }
                }
                else
                {
                    response = new ResponseUpdateVM()
                    {
                        state = false,
                        nroCotizacion = request.quotationNumber,
                        branch = dataDetalle.datosPoliza.branch.ToString(),
                        id = request.id,
                        stateRequest = "PENDING",
                        codError = response_error.P_COD_ERR.ToString(),
                        errors = new List<string>()
                                    {
                                        response_error.P_MESSAGE
                                    }
                    };
                }

                // Implementacion de servicios de Devmente
                await quotationCORE.confirmQuote(response);
            }
            catch (Exception ex)
            {
                LogControl.save("updateStatusPD", ex.ToString(), "3");

                // Implementacion de servicios de Devmente
                response = new ResponseUpdateVM()
                {
                    state = false,
                    nroCotizacion = request.quotationNumber,
                    branch = dataDetalle.datosPoliza.branch.ToString(),
                    id = request.id,
                    stateRequest = "PENDING",
                    codError = "1",
                    errors = new List<string>()
                                {
                                    ex.ToString()
                                }
                };

                await quotationCORE.confirmQuote(response);
            }

            return await Task.FromResult(response);
        }

        public async Task<validaTramaVM> ordenarDataCoberturas(validaTramaVM data)
        {

            foreach (var item in data.lcoberturas)
            {
                item.pension_base = new string[] { "FIXED_PENSION_QUANTITY", "MAXIMUM_PENSION_QUANTITY" }.Contains(item.id_cover) ? item.sumaPropuesta == 0 ? item.capital_aut == 0 ? item.suma_asegurada : item.capital_aut : item.sumaPropuesta : 0;
                item.pension_max = new string[] { "FIXED_PENSION_QUANTITY", "MAXIMUM_PENSION_QUANTITY" }.Contains(item.id_cover) ? item.capital_max : 0;
                item.pension_min = new string[] { "FIXED_PENSION_QUANTITY", "MAXIMUM_PENSION_QUANTITY" }.Contains(item.id_cover) ? item.capital_min : 0;
                item.pension_prop = new string[] { "FIXED_PENSION_QUANTITY", "MAXIMUM_PENSION_QUANTITY" }.Contains(item.id_cover) ? item.sumaPropuesta : 0;

                item.porcen_base = item.id_cover == "FIXED_INSURED_SUM_PERCENTAGE" ? item.capitalCovered : 0;
                item.porcen_max = item.id_cover == "FIXED_INSURED_SUM_PERCENTAGE" ? item.capitalCovered : 0;
                item.porcen_min = item.id_cover == "FIXED_INSURED_SUM_PERCENTAGE" ? item.capitalCovered : 0;
                item.porcen_prop = item.id_cover == "FIXED_INSURED_SUM_PERCENTAGE" ? item.capitalCovered : 0;

                item.sdes_cover = item.id_cover == "FIXED_PENSION_QUANTITY" ? "CANT. PENSIONES FIJA" :
                                  item.id_cover == "MAXIMUM_PENSION_QUANTITY" ? "PENSIÓN MÁXIMA" :
                                  item.id_cover == "FIXED_INSURED_SUM" ? "MONTO FIJO" :
                                  item.id_cover == "FIXED_INSURED_SUM_PERCENTAGE" ? "VALOR PORCENTUAL" : "MONTO FIJO";
            }

            return data;
        }

        private async Task<SalidaTramaBaseVM> tramaBeneficiarios(DataTable dataTable, HttpPostedFile upload, validaTramaVM objValida, ValidaTramaBM validaTramaBM)
        {
            LogControl.save("tramaBeneficiarios", JsonConvert.SerializeObject(objValida, Formatting.None), "2");
            var response = new SalidaTramaBaseVM();

            try
            {
                var tramaList = new List<tramaIngresoBM>();
                Int32 count = 1;
                Int32 rows = Convert.ToInt32(ELog.obtainConfig("rows" + objValida.codRamo));
                Int64 cantidadTrabajadores = 0;
                Int64 cantAsegurados = 0;  // AGF 12052023 Beneficiarios VCF

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
                dt.Columns.Add("TYPE_PLAN");
                dt.Columns.Add("PERCEN_PARTICIPATION");
                dt.Columns.Add("SRELATION");


                // INI AGF 12052023 Beneficiarios VCF

                if (ELog.obtainConfig("vidaIndividualBranch") == objValida.codRamo)
                {

                    var nationality_contrac = objValida.datosContratante.nacionalidad == "1" ? "Peru" : "Peru";
                    var document_contrac = objValida.datosContratante.codDocumento == "2" ? "DNI" : "DNI";
                    var des_telefono_contract = objValida.datosContratante.desTelefono;
                    var telefono_contract = objValida.datosContratante.telefono;
                    var sex_contract = objValida.datosContratante.sexo == "1" ? "F" : objValida.datosContratante.sexo == "2" ? "M" : "I";



                    if (dataTable.Rows[rows][1].ToString().Trim().ToUpper() == "BENEFICIARIO")
                    {
                        DataRow dr = dt.NewRow();
                        dr["NID_COTIZACION"] = objValida.nroCotizacion == 0 ? null : objValida.nroCotizacion;
                        dr["NID_PROC"] = objValida.codProceso;
                        dr["SFIRSTNAME"] = objValida.datosContratante.nombre;
                        dr["SLASTNAME"] = objValida.datosContratante.apePaterno;
                        dr["SLASTNAME2"] = objValida.datosContratante.apeMaterno;
                        dr["NMODULEC"] = ELog.obtainConfig("riesgoDefaultAP");
                        dr["NNATIONALITY"] = nationality_contrac;
                        dr["NIDDOC_TYPE"] = document_contrac;
                        dr["SIDDOC"] = objValida.datosContratante.documento;
                        dr["DBIRTHDAT"] = objValida.datosContratante.fechaNacimiento.ToString().Trim();
                        dr["NREMUNERACION"] = null;
                        dr["NIDCLIENTLOCATION"] = null;
                        dr["NCOD_NETEO"] = null;
                        dr["NUSERCODE"] = objValida.codUsuario.Trim();
                        dr["SSTATREGT"] = null;
                        dr["SCOMMENT"] = null;
                        dr["NID_REG"] = count.ToString();
                        dr["SSEXCLIEN"] = sex_contract;
                        dr["NACTION"] = DBNull.Value;
                        dr["SROLE"] = "Asegurado";
                        dr["SCIVILSTA"] = null;
                        dr["SSTREET"] = null;
                        dr["SPROVINCE"] = null;
                        dr["SLOCAL"] = null;
                        dr["SMUNICIPALITY"] = null;
                        dr["SE_MAIL"] = objValida.datosContratante.email;
                        dr["SPHONE_TYPE"] = des_telefono_contract;
                        dr["SPHONE"] = telefono_contract;
                        dr["NIDDOC_TYPE_BENEFICIARY"] = null;
                        dr["SIDDOC_BENEFICIARY"] = null;
                        dr["TYPE_PLAN"] = null;
                        dr["PERCEN_PARTICIPATION"] = null;
                        dr["SRELATION"] = null;


                        count++;
                        cantidadTrabajadores++;

                        dt.Rows.Add(dr);
                    }
                }

                // FIN AGF 12052023 Beneficiarios VCF
                while (rows < dataTable.Rows.Count)
                {
                    DataRow dr = dt.NewRow();
                    dr["NID_COTIZACION"] = objValida.nroCotizacion == 0 ? null : objValida.nroCotizacion;
                    dr["NID_PROC"] = objValida.codProceso;
                    dr["SFIRSTNAME"] = dataTable.Rows[rows][6].ToString().Trim() == "" ? null : dataTable.Rows[rows][6].ToString().Trim();
                    dr["SLASTNAME"] = dataTable.Rows[rows][4].ToString().Trim() == "" ? null : dataTable.Rows[rows][4].ToString().Trim();
                    dr["SLASTNAME2"] = dataTable.Rows[rows][5].ToString().Trim() == "" ? null : dataTable.Rows[rows][5].ToString().Trim();
                    dr["NMODULEC"] = ELog.obtainConfig("riesgoDefaultAP");
                    dr["NNATIONALITY"] = dataTable.Rows[rows][16].ToString().Trim() == "" ? null : dataTable.Rows[rows][16].ToString().Trim();
                    dr["NIDDOC_TYPE"] = getDatoTrama(dataTable.Rows[rows], "tipo_documento", "asegurado");
                    dr["SIDDOC"] = getDatoTrama(dataTable.Rows[rows], "documento", "asegurado");
                    var fechav = dataTable.Rows[rows][8].ToString().Trim().Split(' ').ToList();
                    var fnacimientov = fechav.Count() == 1 ? fechav[0] : Convert.ToDateTime(dataTable.Rows[rows][8].ToString()).ToString("dd/MM/yyyy");
                    string fecha = IsDate(dataTable.Rows[rows][8].ToString().Trim()) ? fnacimientov : dataTable.Rows[rows][8].ToString().Trim();
                    dr["DBIRTHDAT"] = dataTable.Rows[rows][8].ToString().Trim() == "" ? null : fecha;
                    dr["NREMUNERACION"] = dataTable.Rows[rows][9].ToString().Trim() == "" ? null : dataTable.Rows[rows][9].ToString().Trim().Replace(",", "");
                    dr["NIDCLIENTLOCATION"] = null;
                    dr["NCOD_NETEO"] = null;
                    dr["NUSERCODE"] = objValida.codUsuario.Trim();
                    dr["SSTATREGT"] = null;
                    dr["SCOMMENT"] = null;
                    dr["NID_REG"] = count.ToString();
                    dr["SSEXCLIEN"] = dataTable.Rows[rows][7].ToString().Trim() == "" ? null : dataTable.Rows[rows][7].ToString().Trim();
                    dr["NACTION"] = DBNull.Value;
                    dr["SROLE"] = dataTable.Rows[rows][1].ToString().Trim() == "" ? null : dataTable.Rows[rows][1].ToString().Trim();
                    dr["SCIVILSTA"] = null;
                    dr["SSTREET"] = null;
                    dr["SPROVINCE"] = null;
                    dr["SLOCAL"] = null;
                    dr["SMUNICIPALITY"] = null;
                    dr["SE_MAIL"] = dataTable.Rows[rows][10].ToString().Trim() == "" ? null : dataTable.Rows[rows][10].ToString().Trim();
                    dr["SPHONE_TYPE"] = dataTable.Rows[rows][11].ToString().Trim() == "" ? null : "Celular";
                    dr["SPHONE"] = dataTable.Rows[rows][11].ToString().Trim() == "" ? null : dataTable.Rows[rows][11].ToString().Trim();
                    dr["NIDDOC_TYPE_BENEFICIARY"] = getDatoTrama(dataTable.Rows[rows], "tipo_documento", "beneficiario");
                    dr["SIDDOC_BENEFICIARY"] = getDatoTrama(dataTable.Rows[rows], "documento", "beneficiario");
                    dr["TYPE_PLAN"] = null;
                    dr["PERCEN_PARTICIPATION"] = dataTable.Rows[rows][14].ToString().Trim() == "" ? null : dataTable.Rows[rows][14].ToString().Trim();
                    dr["SRELATION"] = dataTable.Rows[rows][15].ToString().Trim() == "" ? null : dataTable.Rows[rows][15].ToString().Trim();

                    cantidadTrabajadores = dataTable.Rows[rows][1].ToString().Trim() == "Asegurado" ? cantidadTrabajadores + 1 : cantidadTrabajadores;
                    cantAsegurados = dataTable.Rows[rows][1].ToString().Trim() == "Asegurado" ? cantAsegurados + 1 : cantAsegurados; // AGF 12052023 Beneficiarios VCF
                    count++;
                    rows++;
                    dt.Rows.Add(dr);
                }

                response = quotationCORE.SaveUsingOracleBulkCopy(dt);

                if (response.P_COD_ERR == "0")
                {

                    if (cantidadTrabajadores == 0)
                    {
                        response.P_COD_ERR = "1";
                        response.P_MESSAGE = "La trama no tiene asegurados";
                    }
                    else if (cantidadTrabajadores > 1)
                    {
                        response.P_COD_ERR = "1";
                        response.P_MESSAGE = "La trama solo puede tener un asegurado";
                    }

                    // INI AGF 22052023 cambio validacion Beneficiarios VCF
                    else if (cantAsegurados > 0)
                    {
                        if (ELog.obtainConfig("vidaIndividualBranch") == objValida.codRamo)
                        {
                            response.P_COD_ERR = "2";
                            response.P_MESSAGE = "La trama no puede tener asegurados";
                        }
                    }
                    // FIN AGF 22052023 cambio validacion Beneficiarios VCF

                    else
                    {
                        // SP_VALIDADOR_PRODUCTOS
                        response.baseError = quotationCORE.ValidateTramaAccidentesPersonales(validaTramaBM, objValida);
                        response.P_COD_ERR = response.baseError.errorList.Count == 0 ? "0" : "1";
                        response.P_MESSAGE = response.baseError.errorList.Count == 0 ? response.P_MESSAGE : "Hay errores en la trama";
                    }
                }

            }
            catch (Exception ex)
            {
                response.P_COD_ERR = "1";
                response.P_MESSAGE = ex.Message;
                LogControl.save("tramaBeneficiarios", ex.ToString(), "3");
            }

            return await Task.FromResult(response);
        }

        [Route("GetStatusListRQ")]
        [HttpGet]
        public List<StatusVM> GetStatusListRQ(string certype, string codProduct)
        {
            return quotationCORE.GetStatusListRQ(certype, codProduct);
        }


        [Route("GetBeneficiar")]
        [HttpPost]
        public List<BeneficiarVM> GetBeneficiarList(BeneficiarBM data)
        {
            var response = quotationCORE.GetBeneficiarsList(data);
            return response;
        }


        [Route("UpdateNumCredit")]
        [HttpPost]
        public NumCredit UpdateNumCredit(NumCreditUPD request)
        {
            return quotationCORE.UpdateNumCredit(request);
        }

        [Route("GetDPS")]
        [HttpPost]
        public List<DpsVM> GetDPS(int numcotizacion, int nbranch, int nproduct)
        {
            var response = quotationCORE.getDPS(numcotizacion, nbranch, nproduct);
            return response;
        }

        [Route("TotalPremiumFix")]
        [HttpPost]
        public PremiumFixVM TotalPremiumFix(PremiumFixBM data)
        {
            return quotationCORE.TotalPremiumFix(data);
        }
        [Route("TotalPremiumCred")]
        [HttpPost]
        public PremiumFixVM TotalPremiumCred(PremiumFixBM data)
        {
            return quotationCORE.TotalPremiumCred(data);
        }
        [Route("validarCipExistente")]
        [HttpPost]
        public CipValidateVM validarCipExistente(CipValidateBM data)
        {
            LogControl.save("validarCipExistente-CIP", JsonConvert.SerializeObject(data, Formatting.None), "2");
            return quotationCORE.validarCipExistente(data);

        }
        [Route("ReverseMovementsIncomplete")]
        [HttpPost]
        public PremiumFixVM ReverseMovementsIncomplete(PremiumFixBM data)
        {
            return quotationCORE.ReverseMovementsIncomplete(data);
        }

        [Route("GetFlagPremiumMin")]
        [HttpPost]
        public QuotationResponseVM GetFlagPremiumMin(QuotationCabBM data)
        {
            return quotationCORE.GetFlagPremiumMin(data);
        }

        // desgravament
        public async Task<SalidaTramaBaseVM> InsDataCalculoEC(validaTramaVM request)
        {
            var response = new SalidaTramaBaseVM();
            var response_error = new SalidaErrorBaseVM();

            try
            {
                if (request != null)
                {

                    string branch = request.datosPoliza.branch.ToString();

                    // condicional para ramo 71
                    if (branch == ELog.obtainConfig("vidaIndividualBranch") && request.codAplicacion == "EC")
                    {
                        request.codProceso = (request.codUsuario + GenerarCodigo() + DateTime.Now.ToString("yyyyMMddHHmmss")).PadLeft(30, '0');
                        //request.nroCotizacion = long.Parse(request.codProceso);
                        request.flag = "1";
                        //Datos de Prima
                        //request.premium = 80;
                        //Datos de Prima
                        request.flagpremium = 1;


                        //response_error = quotationCORE.regDetallePlanCot(request);

                        response_error = response_error.P_COD_ERR == 0 ? quotationCORE.InsCargaMasEspecial(request) : response_error;

                        //regla prima minima

                        var arrMeses = ELog.obtainConfig("cantidadMesesDes").Split(';').Select(x => x.Split(',')).ToArray();
                        var arrHabilitado = arrMeses.Where(x => x[0].ToString() == request.datosPoliza.codTipoFrecuenciaPago.ToString()).ToArray();

                        var scondition = arrHabilitado[0][1].ToString();

                        validaTramaVM info = new validaTramaVM()
                        {
                            codProceso = request.codProceso,
                            datosPoliza = new DatosPoliza()
                            {
                                branch = Convert.ToInt32(branch),
                                codTipoProducto = request.codProducto,
                                trxCode = "EM"
                            }
                        };
                        WSPlataforma.Entities.Graphql.Rule rule = new WSPlataforma.Entities.Graphql.Rule()
                        {
                            id = "1",
                            definition = new Definition()
                            {
                                _id = "PF1",
                                code = "PF1",
                                order = 1,
                                type = "MIN_PREMIUM_FREQUENCIES",
                                description = "PRIMA MINIMA - FRECUENCIA DE PAGO",

                            },
                            content = new RuleContent()
                            {
                                numericInterval = new NumericIntervalRuleContent()
                                {
                                    interval = new Range()
                                    {
                                        min = -1,
                                        max = -1,
                                        type = "CLOSED"
                                    }
                                },
                                conditionalValue = new ConditionalValueRuleContent()
                                {
                                    condition = scondition,
                                    value = request.datosPoliza.prima_minima_tar.ToString()
                                }
                            }
                        };
                        var response_errorEC = new SalidaTramaBaseVM();

                        response_errorEC = new QuotationCORE().insertRulesDes(rule, info);
                        //regla prima minima

                        response.P_COD_ERR = response_error.P_COD_ERR == 0 ? "0" : "1";

                        response = response.P_COD_ERR == "0" ? quotationCORE.InsReCotizaTrama(request) : response;


                        response.P_COD_ERR = "0";
                        response.P_MESSAGE = "";
                        response.NIDPROC = request.codProceso;
                    }
                    else
                    {
                        response.P_COD_ERR = "1";
                        response.P_MESSAGE = "Fallo paso 1";
                    }
                }
                else
                {
                    response.P_COD_ERR = "1";
                    response.P_MESSAGE = "No se ha procesado la solicitud por falta de datos";
                }
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = "1";
                response.P_MESSAGE = ex.Message;
            }
            return await Task.FromResult(response);
        }

        [Route("InsertQuotationEC")]
        [HttpPost]
        public async Task<ResponseEmitVM> InsertQuotationEC(validaTramaVM request)
        {
            var responseResult = new ResponseEmitVM();
            var response = new SalidaTramaBaseVM();
            var files = new List<HttpPostedFile>();
            var responseInsert = new QuotationResponseVM();
            var response_error = new SalidaErrorBaseVM();

            LogControl.save("InsertQuotationEC", JsonConvert.SerializeObject(request, Formatting.None), "2");

            try
            {
                if (request != null)
                {
                    response = await InsDataCalculoEC(request);

                    if (response.P_COD_ERR == "0")
                    {
                        var resquestQuotation = new QuotationCabBM()
                        {

                            CodigoProceso = response.NIDPROC,
                            TrxCode = request.datosPoliza.trxCode,
                            planId = 3,
                            P_NBRANCH = request.datosPoliza.branch,
                            P_SCLIENT = request.datosContratante.codContratante,
                            P_NCURRENCY = Int32.Parse(request.datosPoliza.codMon),
                            P_NIDCLIENTLOCATION = 1,
                            P_DSTARTDATE = request.datosPoliza.InicioVigPoliza,
                            P_DEXPIRDAT = request.datosPoliza.FinVigPoliza,
                            P_DSTARTDATE_ASE = request.datosPoliza.InicioVigAsegurado,
                            P_DEXPIRDAT_ASE = request.datosPoliza.FinVigAsegurado,
                            P_NTIP_RENOV = Int32.Parse(request.datosPoliza.codTipoRenovacion),
                            P_NPAYFREQ = Int32.Parse(request.datosPoliza.codTipoFrecuenciaPago),
                            P_NPRODUCT = Int32.Parse(request.datosPoliza.codTipoProducto),
                            P_NFACTURA_ANTICIPADA = 1,
                            P_NTYPE_PROFILE = request.datosPoliza.codTipoPerfil,
                            P_NTYPE_PRODUCT = request.datosPoliza.codTipoNegocio,
                            P_NTIPO_FACTURACION = request.datosPoliza.codTipoFacturacion,
                            P_NPENDIENTE = 0,
                            P_NSCOPE = 1,
                            P_SCOMMENT = null,
                            P_SRUTA = "",
                            P_NUSERCODE = Int32.Parse(request.codUsuario),
                            P_NMODULEC_TARIFARIO = Int32.Parse(request.datosPoliza.codTipoPlan),
                            P_SDESCRIPT_TARIFARIO = request.datosPoliza.desTipoPlan,
                            P_SCLIENT_PROVIDER = request.datosPoliza.codClienteEndosatario,
                            P_NNUM_CREDIT = request.datosPoliza.nroCredito,
                            P_NNUM_CUOTA = request.datosPoliza.nroCuotas,
                            P_DFEC_OTORGAMIENTO = request.datosPoliza.fechaOtorgamiento,
                            P_NCAPITAL = request.datosPoliza.capitalCredito,
                            P_NPOLIZA_MATRIZ = 0,
                            P_SAPLICACION = request.codAplicacion,
                            P_STIMEREN = request.datosPoliza.tipoRenovacionPoliza,
                            P_ACTIVITY = request.datosPoliza.CodActividadRealizar
                        };

                        response_error = quotationCORE.regDetallePlanCot(request);

                        resquestQuotation.QuotationDet = request.QuotationDet;
                        resquestQuotation.QuotationCom = request.QuotationCom;

                        LogControl.save("InsertQuotationEC", JsonConvert.SerializeObject(resquestQuotation, Formatting.None), "2");

                        responseInsert = await quotationCORE.InsertQuotation(resquestQuotation, files, null);

                        LogControl.save("InsertQuotationEC", JsonConvert.SerializeObject(responseInsert, Formatting.None), "2");


                        var dataPrima = new updateCotizador()
                        {
                            codProceso = request.codProceso,
                            nroCotizacion = responseInsert.P_NID_COTIZACION,
                            codRamo = Convert.ToInt32(request.codRamo),
                            codProducto = Convert.ToInt32(request.codProducto),
                            codUsuario = request.codUsuario,
                            codEstado = 2, //Estado Creado
                            polMatriz = "0",
                            fechaEfecto = Convert.ToDateTime(request.datosPoliza.InicioVigPoliza).ToString("dd/MM/yyyy")
                        };

                        response = response.P_COD_ERR == "0" && response_error.P_COD_ERR == 0 ? quotationCORE.updatePrimaCotizador(dataPrima) : response;

                        PrimaProrrateo prorr = new PrimaProrrateo();

                        prorr = GetPrimaProrrateo(Int32.Parse(responseInsert.P_NID_COTIZACION));

                        if (responseInsert.P_COD_ERR == 0)  //&& response.P_COD_ERR == "0"
                        {
                            responseResult.P_COD_ERR = 0;
                            responseResult.P_NIDPROC = responseInsert.P_NIDPROC;
                            responseResult.P_NID_COTIZACION = responseInsert.P_NID_COTIZACION;
                            responseResult.P_SMESSAGE = "Cotización  creada exitosamente";
                            responseResult.P_SAPROBADO = responseInsert.P_SAPROBADO;
                            responseResult.P_PRIMA_PRORR = Double.Parse(prorr.NPREMIUM.ToString());

                        }
                        else
                        {
                            responseResult.P_COD_ERR = 1;
                            responseResult.P_SMESSAGE = "Ha ocurrido al insertar la Cotización";
                        }
                    }
                    else
                    {
                        responseResult.P_COD_ERR = 1;
                        responseResult.P_SMESSAGE = "Ha ocurrido al registrar los datos";
                    }
                }
                else
                {
                    responseResult.P_COD_ERR = 1;
                    responseResult.P_SMESSAGE = "Debe ingresar los datos solicitados";
                }

            }

            catch (Exception ex)
            {
                responseResult.P_COD_ERR = 1;
                responseResult.P_SMESSAGE = ex.ToString();
                LogControl.save("InsertQuotationEC", ex.ToString(), "3");
            }

            return responseResult;
        }

        /// <summary>
        /// Servicio para descarga documento Slip en formato base64
        /// </summary>
        [Route("DownloadSlip")]
        [HttpPost]
        public ResponseSlipVM DownloadSlip(RequestSlipBM request)
        {
            var response = new ResponseSlipVM() { cod_error = 0 };

            try
            {
                string path = ELog.obtainConfig("pathSlip" + request.p_ramo) + request.p_cotizacion + "\\";

                if (Directory.Exists(path))
                {
                    string[] files = System.IO.Directory.GetFiles(path, "Cotizacion*.pdf");

                    if (files.Length > 0)
                    {
                        Byte[] bytes = File.ReadAllBytes(files[0]);
                        response.documento = Convert.ToBase64String(bytes);
                        response.mensaje = "Se encontró el slip correctamente";
                    }
                    else
                    {
                        response.cod_error = 1;
                        response.mensaje = "No se ha generado el slip de la cotización enviada";
                    }
                }
                else
                {
                    response.cod_error = 1;
                    response.mensaje = "No se ha encontrado la ruta de la cotización enviada";
                }
            }
            catch (Exception ex)
            {
                response.cod_error = 1;
                response.mensaje = "Hubo un error al buscar el slip de la cotización";
                LogControl.save("DownloadSlip", ex.ToString(), "3");
            }

            return response;
        }

        [Route("validaTramaEcommerceVL")]
        [HttpPost]
        public async Task<SalidaTramaEcommerce> validaTramaEcommerceVL()
        {
            var response = new SalidaTramaEcommerce() { cod_error = 0, mensaje = "Se validó correctamente la trama" };

            try
            {
                HttpPostedFile upload = HttpContext.Current.Request.Files["dataFile"];
                LogControl.save("validaTramaEcommerceVL", HttpContext.Current.Request.Params.Get("objValida"), "2");
                var objValida = JsonConvert.DeserializeObject<ValidaTramaEcommerce>(HttpContext.Current.Request.Params.Get("objValida"));

                objValida.max_planilla = Convert.ToInt64(ELog.obtainConfig("planillaMax"));
                objValida.tope_ley = Convert.ToInt64(ELog.obtainConfig("topeLey"));
                objValida.fec_actual = DateTime.Now;

                if (upload != null && upload.ContentLength > 0)
                {
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

                            if (result.Tables[0].Rows.Count > 12)
                            {
                                var listRenov = new int[] { 5, 4, 3, 2, 1 }.ToList();
                                var v_proceso_old = string.Empty;

                                foreach (var item in listRenov)
                                {
                                    var v_proceso = (item.ToString() + objValida.cod_usuario + ELog.generarCodigo() + DateTime.Now.ToString("yyyyMMddHHmmss")).PadLeft(30, '0');
                                    v_proceso_old = item == 5 ? v_proceso : v_proceso_old;

                                    objValida.cod_proceso = v_proceso;
                                    objValida.cod_proceso_old = v_proceso_old;
                                    objValida.tip_renovacion = item;
                                    objValida.freq_pago = item;

                                    //if (response.errorList.Count == 0)
                                    //{
                                    response = await validarRenovacionEC(objValida, result, response, item);
                                    //}
                                    //else
                                    //{
                                    //    var dataCot = new SalidaTramaBaseVM() { };
                                    //    response.detalleTrama.Add(generateDetalle(dataCot, objValida, response));
                                    //}
                                }

                                var obj = response.detalleTrama.FirstOrDefault(x => x.detallePlanList.Count > 0);

                                if (obj != null)
                                {
                                    response.detallePlan = new detailPlanRoot();
                                    response.detallePlan.cod_plan = obj.detallePlanList[0].cod_plan;
                                    response.detallePlan.des_plan = obj.detallePlanList[0].des_plan;
                                    response.detallePlan.coberturaList = quotationCORE.getCoverEcommerce(objValida, response.detallePlan.cod_plan);
                                }
                            }
                        }
                        else
                        {
                            response.cod_error = 1;
                            response.mensaje = "La cantidad mínima de asegurados debe ser de 1.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.cod_error = 1;
                response.mensaje = "Hubo un error al procesar la trama";
                LogControl.save("validaTramaEcommerceVL", ex.ToString(), "3");
            }

            return response;
        }

        private async Task<SalidaTramaEcommerce> validarRenovacionEC(ValidaTramaEcommerce objValida, DataSet result, SalidaTramaEcommerce response, int item)
        {
            var dataCot = new SalidaTramaBaseVM() { };

            // Se guarda los datos del excel y se valida con el cliente 360
            if (item == 5)
            {
                dataCot = await procesoEcommerce(result.Tables[0], objValida);
            }
            else
            {
                dataCot = quotationCORE.InsertDataEC(objValida);
            }

            // Se crea objeto para validar la trama
            var validaTramaBM = generarObjTrama(objValida);

            // Se valida la trama y nos trae el detalle
            dataCot = dataCot.P_COD_ERR == "0" ? validarTramaEcommerce(objValida, validaTramaBM, response) : dataCot;

            if (dataCot.P_COD_ERR == "0")
            {
                response.detalleTrama.Add(generateDetalle(dataCot, objValida, response));

                if (dataCot.baseError.errorList.Count > 0)
                {
                    var errors = dataCot.baseError.errorList.Where(x => x.COD_ERROR == 0).ToList();

                    if (errors.Count > 0)
                    {
                        response.errorList = new List<error>();

                        foreach (var error in errors)
                        {
                            var itemError = new error()
                            {
                                cod_error = error.COD_ERROR,
                                num_registro = error.REGISTRO,
                                des_error = error.DESCRIPCION
                            };

                            response.errorList.Add(itemError);
                        }
                    }
                }
            }
            else
            {
                response.cod_error = 1;
                response.mensaje = dataCot.P_MESSAGE;
            }

            return response;
        }

        private detTrama generateDetalle(SalidaTramaBaseVM dataCot, ValidaTramaEcommerce obj, SalidaTramaEcommerce res)
        {
            var response = new detTrama();

            var request = new TypeRenBM()
            {
                P_NEPS = 1,
                P_NPRODUCT = 3,
                P_NUSERCODE = obj.cod_usuario
            };

            var renovacion = policyCORE.GetTipoRenovacion(request);

            if (renovacion.Count > 0)
            {
                response.cod_renovacion = obj.tip_renovacion;
                response.des_renovacion = renovacion.FirstOrDefault(x => x.COD_TIPO_RENOVACION == obj.tip_renovacion).DES_TIPO_RENOVACION;
                response.cod_proceso = obj.cod_proceso;

                if (dataCot.baseError.errorList.Count > 0)
                {
                    var errors = dataCot.baseError.errorList.Where(x => x.COD_ERROR == 1).ToList();

                    if (errors.Count > 0)
                    {
                        foreach (var error in errors)
                        {
                            var item = new error()
                            {
                                cod_error = error.COD_ERROR,
                                num_registro = error.REGISTRO,
                                des_error = error.DESCRIPCION
                            };

                            response.errorList.Add(item);
                        }
                    }
                }
                else
                {
                    //if (res.detalleTrama.Count > 1 && res.detalleTrama.Count < 4)
                    //{
                    //    response.errorList = res.detalleTrama[0].errorList;
                    //}

                    foreach (var categoria in dataCot.categoryList)
                    {
                        if (categoria.ModuleCode != 3)   // GCAA 24112023
                        {
                            if (categoria.NTOTAL_PLANILLA > 0) // GCAA 21112023
                            {
                                var amount = dataCot.amountPremiumList.FirstOrDefault(x => x.SCATEGORIA == categoria.SCATEGORIA);
                                var nameOfProperty = ELog.obtainConfig("categoriaPrima" + obj.tip_renovacion);
                                var propertyInfo = amount.GetType().GetProperty(nameOfProperty);

                                var item = new category()
                                {
                                    cod_categoria = categoria.ModuleCode,
                                    des_categoria = categoria.SCATEGORIA,
                                    rango_edad = categoria.SRANGO_EDAD, // GCAA 14112023 CAMBIO TRAMA PARA DEVMENTE
                                    cant_trabajadores = categoria.NCOUNT,
                                    planilla = categoria.NTOTAL_PLANILLA,
                                    tasa = categoria.NTASA,
                                    prima = Convert.ToDouble(propertyInfo.GetValue(amount, null))
                                };

                                response.categoriaList.Add(item);
                            }
                            else
                            {
                                var _ite = new category()
                                {
                                    cod_categoria = categoria.ModuleCode,
                                    des_categoria = categoria.SCATEGORIA,
                                    rango_edad = categoria.SRANGO_EDAD,// GCAA 14112023 CAMBIO TRAMA PARA DEVMENTE
                                    cant_trabajadores = 0,
                                    planilla = 0,
                                    tasa = categoria.NTASA,
                                    prima = 0
                                };
                                response.categoriaList.Add(_ite);
                            }
                        }
                    }


                    foreach (var plan in dataCot.detailPlanList)
                    {
                        var item = new detailPlan()
                        {
                            cod_plan = plan.COD_PLAN,
                            des_plan = plan.DET_PLAN,
                            prima_minima = plan.PRIMA_MINIMA
                        };

                        response.detallePlanList.Add(item);
                    }

                    foreach (var det in dataCot.amountDetailTotalList)
                    {
                        var nameOfProperty = ELog.obtainConfig("monto" + obj.tip_renovacion);
                        var propertyInfo = det.GetType().GetProperty(nameOfProperty);

                        var item = new amountDetailTotalList()
                        {
                            des_glosa = det.SDESCRIPCION,
                            prima = Convert.ToDouble(propertyInfo.GetValue(det, null))
                        };

                        response.detalleMontosList.Add(item);
                    }
                }
            }
            else
            {

            }

            return response;
        }

        [Route("GetEdadesValidar")]
        [HttpPost]
        public EdadValidarVM GetEdadesValidar(CoberturaBM rangoEdades)
        {
            return quotationCORE.GetEdadesValidar(rangoEdades);
        }


        [Route("GetSobrevivencia")]
        [HttpPost]
        public SobrevivenciaVM GETSOBREVIXCOVER(string nidcotizacion, string nbranch)
        {
            return quotationCORE.GETSOBREVIXCOVER(nidcotizacion, nbranch);
        }

        [Route("UpdateCotizacionClienteEstado")]
        [HttpPost]
        public async Task<QuotationResponseVM> updateCotizacionClienteEstado()
        {
            var response = new QuotationResponseVM();
            try
            {
                List<HttpPostedFile> files = new List<HttpPostedFile>();
                foreach (var item in HttpContext.Current.Request.Files)
                {
                    HttpPostedFile arch = HttpContext.Current.Request.Files[item.ToString()];
                    files.Add((HttpPostedFile)HttpContext.Current.Request.Files[item.ToString()]);
                }

                LogControl.save("updateCotizacionClienteEstado", HttpContext.Current.Request.Params.Get("objetoEstado"), "2");
                var cotizacion = JsonConvert.DeserializeObject<QuotationCabBM>(HttpContext.Current.Request.Params.Get("objetoEstado"));

                //var rutaFinalEval = ELog.obtainConfig("pathCotizacion") + response.P_NID_COTIZACION + "\\" + ELog.obtainConfig("cotizacionKey") + "\\";
                //response = sharedMethods.ValidatePath<QuotationResponseVM>(response, rutaFinalEval, files);
                //if (response.P_COD_ERR == 1)
                //    return response;

                response = await quotationCORE.updateCotizacionClienteEstado(cotizacion);

            }

            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = "Hubo un error al generar la cotización. Favor de comunicarse con sistemas";
                LogControl.save("updateCotizacionClienteEstado", ex.ToString(), "3");
            }

            if (response.P_COD_ERR == 1)
            {
                LogControl.save("updateCotizacionClienteEstado", JsonConvert.SerializeObject(response, Formatting.None), "3");
                response.P_MESSAGE = "Hubo un error al generar la cotización. Favor de comunicarse con sistemas";

            }
            else
            {
                response.P_MESSAGE = response.P_MESSAGE == "null" ? string.Empty : response.P_MESSAGE;
            }

            return response;
        }
        [Route("FinalizarCotizaEstadoVL")]
        [HttpPost]
        public EmitPolVM FinalizarCotizacionEstadoVL(ProcesaTramaBM request)
        {
            var response = new EmitPolVM();
            response = quotationCORE.finalizarCotizacionEstadoVL(request);
            return response;
        }
        [Route("GetPrimaProrrateo")]
        [HttpGet]
        public PrimaProrrateo GetPrimaProrrateo(int ncotizacion)
        {
            return quotationCORE.GetPrimaProrrateo(ncotizacion);
        }

        [Route("ReSendQuotationMatrix")]
        [HttpPost]
        public async Task<GenericResponseVM> ReSendQuotationMatrix(ResendPolizaMatriz data)
        {
            IntegrateToDevmente integrateToDevmente = new IntegrateToDevmente();
            GenericResponseVM response = new GenericResponseVM();

            response = await integrateToDevmente.ExecuteTramitesMatriz(data.TrxCode, data.NumeroCotizacion, revaluarTecnica: data.ReevaluarTecnica, comentarioReevaluar: data.comentarioReevaluar);
            return response;
        }

        [Route("validateBroker")]
        [HttpPost]
        public validateBrokerVL validateBroker(validateBrokerVL data)
        {
            return quotationCORE.validateBroker(data);
        }

        [Route("envioEstadoEPS")]
        [HttpPost]
        public GenericResponseEPS envioEstadoEPS(SendStateEPS data)
        {
            return quotationCORE.UpdateCotizacionState(data);
        }

        [Route("NullQuotationCIP")]
        [HttpPost]
        public GenericResponseVM NullQuotationCIP(QuotationCabBM data)
        {
            return quotationCORE.NullQuotationCIP(data);
        }

        [Route("GetBrokerAgenciadoSCTR")]
        [HttpPost]
        public rea_Broker GetBrokerAgenciadoSCTR(string P_SCLIENT, int P_TIPO)
        {
            return quotationCORE.GetBrokerAgenciadoSCTR(P_SCLIENT, P_TIPO);
        }

        [Route("equivalentINEI")]
        [HttpGet]
        public String equivalentINEI(Int64 municipality)
        {
            return quotationCORE.equivalentINEI(municipality);
        }

        [Route("obtRMV")]
        [HttpPost]
        public rmv_ACT obtRMV(string P_DATE)
        {
            return quotationCORE.obtRMV(P_DATE);
        }

        [Route("GetComisionTecnica")]
        [HttpPost]
        public List<listComisionesTecnica> GetComisionTecnica(string P_SCLIENT)
        {
            return quotationCORE.GetComisionTecnica(P_SCLIENT);
        }

        [Route("GetTasastecnica")]
        [HttpPost]
        public List<listtasastecnica> GetTasastecnica(string P_SCLIENT)
        {
            return quotationCORE.GetTasastecnica(P_SCLIENT);
        }

        [Route("GetEstadoClienteNuevo")]
        [HttpPost]
        public ClienteEstado GetEstadoClienteNuevo(string P_SCLIENT)
        {
            return quotationCORE.GetEstadoClienteNuevo(P_SCLIENT);
        }

        [Route("RelanzarCip")]
        [HttpPost]
        public async Task<SJSON_CIP> GetEstadoClienteNuevo(CipRelanzamiento data)
        {
            CIPGeneradoBM cip = new CIPGeneradoBM();
            var formData = new MultipartFormDataContent();
            SJSON_CIP sjsonCip = null;

            if (data.flagKushki == 1)
            {
                var data_ = new LinkRelanzamiento()
                {
                    P_NPRODUCT = data.producto_link,
                    P_SCIP = data.codeCip,
                    P_STIPO_PAGO = data.stipo_pago,
                    P_NMIXTA = data.flagNcotMixta
                };

                sjsonCip = await quotationCORE.getSJSON_LINK(data_);
            }
            else
            {
                sjsonCip = await quotationCORE.getSJSON_CIP(data);
            }

            SJSON_CIP respuesta = new SJSON_CIP();

            if (sjsonCip.ncode == 0)
            {
                cip = JsonConvert.DeserializeObject<CIPGeneradoBM>(sjsonCip.sjson);
                cip.token = data.token;

                if (data.flagNcotMixta == 1)
                {
                    if (data.mixta == 1) //CIP GRUPAL PENSION - SALUD
                    {
                        cip.dataCIPBM.mixta = 1;
                        cip.dataCIPBM.flagProducto = 0;
                    }
                    else
                    {
                        if (data.nproducto == 1) //CIP PENSION
                        {
                            cip.dataCIPBM.mixta = 0;
                            cip.dataCIPBM.flagProducto = 1;
                        }
                        else if (data.nproducto == 2) //CIP SALUD
                        {
                            cip.dataCIPBM.mixta = 0;
                            cip.dataCIPBM.flagProducto = 2;
                        }
                    }
                }
                else
                {
                    cip.dataCIPBM.mixta = 0;
                    cip.dataCIPBM.flagProducto = 0;
                }


                string cipJson = JsonConvert.SerializeObject(cip);
                formData.Add(new StringContent(cipJson), "dataTransaccionesPD");
                var client = new HttpClient();
                var response = new HttpResponseMessage();

                if (data.flagKushki == 1)
                {
                    //response = await client.PostAsync("http://localhost:30897/Api/PolicyManager/GenerarCIPKushkiTransaccionesPD", formData);//Local
                    response = await client.PostAsync("https://serviciosqa.protectasecurity.pe/WSKuntur/Api/PolicyManager/GenerarCIPKushkiTransaccionesPD", formData); //QA
                }
                else
                {
                    //var response = await client.PostAsync("http://localhost:30897/Api/PolicyManager/GenerarCIPTransaccionesPD", formData); //LOCAL
                    response = await client.PostAsync("https://serviciosqa.protectasecurity.pe/WSKuntur/Api/PolicyManager/GenerarCIPTransaccionesPD", formData); //QA   
                }

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();

                    if (data.mixta == 1)
                    {
                        respuesta.ncode = 0;
                        respuesta.mensaje = "Los nuevos códigos han sido generados con éxito. Se procederá a actualizar la plataforma.";
                    }
                    else
                    {
                        respuesta.ncode = 0;
                        respuesta.mensaje = "El nuevo código ha sido generado con éxito. Se procederá a actualizar la plataforma.";
                    }
                }
            }
            else
            {
                respuesta.ncode = 1;
                respuesta.mensaje = "Se encontró un problema al momento de generar el nuevo código. Por favor, comuníquese con sistemas.";
            }

            return respuesta;
        }

        // GCAA 06022024 - CUPONERAS
        [Route("getCuponesExclusion")]
        [HttpGet]
        public List<CouponList> getCuponesExclusion(string nid_proc)
        {
            return quotationCORE.getCuponesExclusion(nid_proc);
        }

        [Route("GetInfoQuotationPreview")]
        [HttpPost]
        public infoQuotationPreviewVM GetInfoQuotationPreview(infoQuotationPreviewBM request)
        {
            return quotationCORE.GetInfoQuotationPreview(request);
        }
    }

}