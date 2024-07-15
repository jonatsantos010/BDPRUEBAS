using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WSPlataforma.Core;
using WSPlataforma.Entities.TransactModel;
using WSPlataforma.Util;

namespace WSPlataforma.Controllers
{
    [RoutePrefix("Api/TransactManager")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class TransactController : ApiController
    {
        TransactCORE transactCORE = new TransactCORE();
        SharedMethods sharedMethods = new SharedMethods();

        [Route("InsUpdTransact")]
        [HttpPost]
        public TransactResponse InsUpdTransact()
        {
            RequestTransact request = new RequestTransact();
            List<HttpPostedFile> files = new List<HttpPostedFile>();
            List<HttpPostedFile> filesCarta = new List<HttpPostedFile>();
            TransactResponse response = new TransactResponse();
            TransactResponse response2 = new TransactResponse();
            try
            {
                foreach (var item in HttpContext.Current.Request.Files)
                {
                    if (item.ToString().IndexOf("carta_agenciamiento_") > -1)
                    {
                        filesCarta.Add((HttpPostedFile)HttpContext.Current.Request.Files[item.ToString()]);
                    }
                    else
                    {
                        HttpPostedFile arch = HttpContext.Current.Request.Files[item.ToString()];
                        files.Add((HttpPostedFile)HttpContext.Current.Request.Files[item.ToString()]);
                    }
                }
                request = JsonConvert.DeserializeObject<RequestTransact>(HttpContext.Current.Request.Params.Get("objeto"));

                string folderPath = "";
                string folderPathCarta = "";
                if (files.Count > 0)
                {
                    folderPath = ELog.obtainConfig("pathTramite") + request.P_NID_COTIZACION + "\\" + ELog.obtainConfig("movimiento") + "\\" + DateTime.Now.ToString("yyyyMMddhhmmss") + "\\";

                    response = sharedMethods.ValidatePath<TransactResponse>(response, folderPath, files);
                    if (response.P_COD_ERR == 1)
                        return response;

                }
                if (filesCarta.Count > 0)
                {
                    folderPathCarta = ELog.obtainConfig("pathTramite") + request.P_NID_COTIZACION + "\\" + ELog.obtainConfig("movimiento") + "\\broker\\" + DateTime.Now.ToString("yyyyMMddhhmmss") + "\\";
                    response = sharedMethods.ValidatePath<TransactResponse>(response, folderPathCarta, filesCarta);
                    if (response.P_COD_ERR == 1)
                        return response;
                }
                request.P_SRUTA = folderPath;
                request.P_SLETTER_AGENCY = folderPathCarta;

                if (request.P_NID_TRAMITE != 0)
                {
                    response = transactCORE.UpdateTransact(request);
                }
                else
                {
                    response = transactCORE.InsertTransact(request);
                }

                if (response.P_COD_ERR == 0)    //R.P.
                {
                    if (request.P_STRANSAC == "RE" || request.P_STRANSAC == "EM")
                    {
                        foreach (var dep in request.P_DAT_BROKER)
                        {
                            dep.P_NID_TRAMITE = response.P_NID_TRAMITE;
                            response2 = transactCORE.InsertDepBrkTramite(dep);
                        }
                    }
                }

                if (response.P_COD_ERR == 0)
                {
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
                    if (filesCarta.Count > 0 && response.P_COD_ERR == 0)
                    {

                        if (!Directory.Exists(String.Format(ELog.obtainConfig("pathPrincipal"), folderPathCarta)))
                        {
                            Directory.CreateDirectory(String.Format(ELog.obtainConfig("pathPrincipal"), folderPathCarta));
                        }

                        foreach (var file in filesCarta)
                        {
                            try
                            {
                                file.SaveAs(String.Format(ELog.obtainConfig("pathPrincipal"), folderPathCarta) + file.FileName.ToString());
                            }
                            catch (Exception)
                            {
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                //response.P_MESSAGE = "Error en el servidor. - " + ex.Message;
                response.P_MESSAGE = "Error en el servidor.";
                LogControl.save("InsUpdTransact", ex.ToString(), "3");
            }
            return response;
        }

        [Route("GetUsersTransact")]
        [HttpPost]
        public List<UserTransact> GetUsersTransact(RequestTransact request)
        {
            return transactCORE.GetUsersTransact(request);
        }

        [Route("AsignarTransact")]
        [HttpPost]
        public TransactResponse AsignarTransact()
        {
            RequestTransact request = new RequestTransact();
            List<HttpPostedFile> files = new List<HttpPostedFile>();
            TransactResponse response = new TransactResponse();
            try
            {
                foreach (var item in HttpContext.Current.Request.Files)
                {
                    HttpPostedFile arch = HttpContext.Current.Request.Files[item.ToString()];
                    files.Add((HttpPostedFile)HttpContext.Current.Request.Files[item.ToString()]);
                }
                request = JsonConvert.DeserializeObject<RequestTransact>(HttpContext.Current.Request.Params.Get("objeto"));

                string folderPath = "";
                if (files.Count > 0)
                {
                    folderPath = ELog.obtainConfig("pathTramite") + request.P_NID_COTIZACION + "\\" + ELog.obtainConfig("movimiento") + "\\" + DateTime.Now.ToString("yyyyMMddhhmmss") + "\\";

                    response = sharedMethods.ValidatePath<TransactResponse>(response, folderPath, files);
                    if (response.P_COD_ERR == 1)
                        return response;
                }
                request.P_SRUTA = folderPath;

                response = transactCORE.AsignarTransact(request);

                if (response.P_COD_ERR == 0)
                {
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
                response.P_COD_ERR = 1;
                //response.P_MESSAGE = "Error en el servidor. - " + ex.Message;
                response.P_MESSAGE = "Error en el servidor.";
                LogControl.save("AsignarTransact", ex.ToString(), "3");
            }
            return response;
        }

        [Route("GetHistTransact")]
        [HttpPost]
        public List<TransactModel> GetHistTransact(RequestTransact request)
        {
            return transactCORE.GetHistTransact(request);
        }

        [Route("UpdateTransactEmail")]
        [HttpPost]
        public TransactResponse UpdateTransactEmail()
        {
            RequestTransact request = new RequestTransact();
            TransactResponse response = new TransactResponse();
            try
            {

                request = JsonConvert.DeserializeObject<RequestTransact>(HttpContext.Current.Request.Params.Get("objeto"));
                response = transactCORE.InsertDerivarTransact(request);

            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = "Error en el servidor. - " + ex.Message;
                LogControl.save("UpdateTransactEmail", ex.ToString(), "3");
            }
            return response;

        }

        [Route("InsertDerivarTransact")]
        [HttpPost]
        public TransactResponse InsertDerivarTransact()
        {
            RequestTransact request = new RequestTransact();
            List<HttpPostedFile> files = new List<HttpPostedFile>();
            TransactResponse response = new TransactResponse();
            try
            {
                foreach (var item in HttpContext.Current.Request.Files)
                {
                    HttpPostedFile arch = HttpContext.Current.Request.Files[item.ToString()];
                    files.Add((HttpPostedFile)HttpContext.Current.Request.Files[item.ToString()]);
                }

                LogControl.save("InsertDerivarTransact", HttpContext.Current.Request.Params.Get("objeto"), "2");
                request = JsonConvert.DeserializeObject<RequestTransact>(HttpContext.Current.Request.Params.Get("objeto"));

                string folderPath = "";
                if (files.Count > 0)
                {
                    folderPath = ELog.obtainConfig("pathTramite") + request.P_NID_COTIZACION + "\\" + ELog.obtainConfig("movimiento") + "\\" + DateTime.Now.ToString("yyyyMMddhhmmss") + "\\";
                    response = sharedMethods.ValidatePath<TransactResponse>(response, folderPath, files);
                    if (response.P_COD_ERR == 1)
                        return response;

                }
                request.P_SRUTA = folderPath;

                if (request.P_NID_TRAMITE == 0)
                {
                    if (new string[] { ELog.obtainConfig("vidaIndividualBranch") }.Contains(request.P_NBRANCH.ToString()))
                    {

                        TransactSearchRequest dataToSearch = new TransactSearchRequest();
                        GenericResponseTransact datobuscado = new GenericResponseTransact();
                        List<TransactVM> generic = new List<TransactVM>();

                        dataToSearch.User = request.P_NUSERCODE.ToString();
                        dataToSearch.Nbranch = request.P_NBRANCH.ToString();
                        //dataToSearch.StartDate = DateTime.Now;
                        dataToSearch.EndDate = DateTime.Now;
                        dataToSearch.QuotationNumber = request.P_NID_COTIZACION.ToString();
                        dataToSearch.TypeSearch = 1;

                        datobuscado = transactCORE.GetTransactList(dataToSearch);
                        generic = (List<TransactVM>)datobuscado.GenericResponse;
                        // (new System.Collections.Generic.Mscorlib_CollectionDebugView<WSPlataforma.Entities.TransactModel.TransactVM>(datobuscado.GenericResponse).Items[0]).TransactNumber

                        foreach (var item in generic)
                        {
                            request.P_NID_TRAMITE = item.TransactNumber;
                        }

                        // datonidtramite = ((datobuscado.GenericResponse).Items[0]).TransactNumber;

                        /*
                        

                        if (datobuscado != null)
                        {
                            foreach (var item in datobuscado.GenericResponse)
                            {

                            }

                        }
                        */
                    }
                }

                response = transactCORE.InsertDerivarTransact(request);

                if (response.P_COD_ERR == 0)
                {
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
                response.P_COD_ERR = 1;
                //response.P_MESSAGE = "Error en el servidor. - " + ex.Message;
                response.P_MESSAGE = "Error en el servidor.";
                LogControl.save("InsertDerivarTransact", ex.ToString(), "3");
            }
            return response;
        }

        [Route("GetStatusListTransact")]
        [HttpPost]
        public List<StatusTransact> GetStatusListTransact()
        {
            return transactCORE.GetStatusListTransact();
        }

        [Route("GetTransactList")]
        [HttpPost]
        public GenericResponseTransact GetTransactList(TransactSearchRequest dataToSearch)
        {
            return transactCORE.GetTransactList(dataToSearch);
        }

        [Route("GetExcelTransactList")]
        [HttpPost]
        public string GetExcelTransactList(TransactSearchRequest dataToSearch)
        {
            return transactCORE.GetExcelTransactList(dataToSearch);
        }

        [Route("GetInfoTransact")]
        [HttpPost]
        public TransactModel GetInfoTransact(RequestTransact request)
        {
            return transactCORE.GetInfoTransact(request);
        }

        [Route("InsertHistTransact")]
        [HttpPost]
        public TransactResponse InsertHistTransact()
        {
            RequestTransact request = new RequestTransact();
            List<HttpPostedFile> files = new List<HttpPostedFile>();
            TransactResponse response = new TransactResponse();
            try
            {
                foreach (var item in HttpContext.Current.Request.Files)
                {
                    HttpPostedFile arch = HttpContext.Current.Request.Files[item.ToString()];
                    files.Add((HttpPostedFile)HttpContext.Current.Request.Files[item.ToString()]);
                }
                request = JsonConvert.DeserializeObject<RequestTransact>(HttpContext.Current.Request.Params.Get("objeto"));

                string folderPath = "";
                if (files.Count > 0)
                {
                    folderPath = ELog.obtainConfig("pathTramite") + request.P_NID_COTIZACION + "\\" + ELog.obtainConfig("movimiento") + "\\" + DateTime.Now.ToString("yyyyMMddhhmmss") + "\\";
                    response = sharedMethods.ValidatePath<TransactResponse>(response, folderPath, files);
                    if (response.P_COD_ERR == 1)
                        return response;
                }
                request.P_SRUTA = folderPath;

                response = transactCORE.InsertHistTransact(request);

                if (response.P_COD_ERR == 0)
                {
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
                response.P_COD_ERR = 1;
                //response.P_MESSAGE = "Error en el servidor. - " + ex.Message;
                response.P_MESSAGE = "Error en el servidor.";
                LogControl.save("InsertHistTransact", ex.ToString(), "3");
            }
            return response;
        }

        [Route("GetInfoLastTransact")]
        [HttpPost]
        public TransactModel GetInfoLastTransact(RequestTransact request)
        {
            return transactCORE.GetInfoLastTransact(request);
        }

        [Route("UpdateBroker")]
        [HttpPost]
        public TransactResponse UpdateBroker()
        {
            RequestTransact request = new RequestTransact();
            List<HttpPostedFile> files = new List<HttpPostedFile>();
            TransactResponse response = new TransactResponse();
            try
            {
                foreach (var item in HttpContext.Current.Request.Files)
                {
                    HttpPostedFile arch = HttpContext.Current.Request.Files[item.ToString()];
                    files.Add((HttpPostedFile)HttpContext.Current.Request.Files[item.ToString()]);
                }

                LogControl.save("UpdateBroker", HttpContext.Current.Request.Params.Get("objeto"), "2");
                request = JsonConvert.DeserializeObject<RequestTransact>(HttpContext.Current.Request.Params.Get("objeto"));

                string folderPath = "";
                if (files.Count > 0)
                {
                    folderPath = ELog.obtainConfig("pathTramite") + request.P_NID_COTIZACION + "\\" + ELog.obtainConfig("movimiento") + "\\" + DateTime.Now.ToString("yyyyMMddhhmmss") + "\\";
                    response = sharedMethods.ValidatePath<TransactResponse>(response, folderPath, files);
                    if (response.P_COD_ERR == 1)
                        return response;
                }
                request.P_SRUTA = folderPath;

                response = transactCORE.UpdateBroker(request);

                if (response.P_COD_ERR == 0)
                {
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
                response.P_COD_ERR = 1;
                //response.P_MESSAGE = "Error en el servidor. - " + ex.Message;
                response.P_MESSAGE = "Error en el servidor.";
                LogControl.save("UpdateBroker", ex.ToString(), "3");
            }
            return response;
        }

        [Route("ValidateAccess")]
        [HttpPost]
        public TransactResponse ValidateAccess(RequestTransact request)
        {
            return transactCORE.ValidateAccess(request);
        }

        [Route("ValidateAccessDes")]
        [HttpPost]
        public TransactResponse ValidateAccessDes(RequestTransact request)
        {
            return transactCORE.ValidateAccessDes(request);
        }

        [Route("GetVigenciaAnterior")]
        [HttpPost]
        public GenericResponseTransact GetVigenciaAnterior(RequestTransact request)
        {
            return transactCORE.GetVigenciaAnterior(request);
        }

        [Route("FinalizarTramite")]
        [HttpPost]
        public TransactResponse FinalizarTramite(RequestTransact request)
        {
            return transactCORE.FinalizarTramite(request);
        }

        [Route("AnularTramite")]
        [HttpPost]
        public TransactResponse AnularTramite()
        {
            RequestTransact request = new RequestTransact();
            List<HttpPostedFile> files = new List<HttpPostedFile>();
            TransactResponse response = new TransactResponse();
            try
            {
                foreach (var item in HttpContext.Current.Request.Files)
                {
                    HttpPostedFile arch = HttpContext.Current.Request.Files[item.ToString()];
                    files.Add((HttpPostedFile)HttpContext.Current.Request.Files[item.ToString()]);
                }
                LogControl.save("AnularTramite", HttpContext.Current.Request.Params.Get("objeto"), "2");
                request = JsonConvert.DeserializeObject<RequestTransact>(HttpContext.Current.Request.Params.Get("objeto"));

                string folderPath = "";
                if (files.Count > 0)
                {
                    folderPath = ELog.obtainConfig("pathTramite") + request.P_NID_COTIZACION + "\\" + ELog.obtainConfig("movimiento") + "\\" + DateTime.Now.ToString("yyyyMMddhhmmss") + "\\";
                    response = sharedMethods.ValidatePath<TransactResponse>(response, folderPath, files);
                    if (response.P_COD_ERR == 1)
                        return response;
                }
                request.P_SRUTA = folderPath;

                response = transactCORE.AnularTramite(request);

                if (response.P_COD_ERR == 0)
                {
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
                response.P_COD_ERR = 1;
                //response.P_MESSAGE = "Error en el servidor. - " + ex.Message;
                response.P_MESSAGE = "Error en el servidor.";
                LogControl.save("AnularTramite", ex.ToString(), "3");
            }
            return response;
        }

        [Route("PerfilTramiteOpe")]
        [HttpPost]
        public TransactResponse PerfilTramiteOpe(RequestTransact request)
        {
            return transactCORE.PerfilTramiteOpe(request);
        }

        [Route("PerfilComercialEx")]
        [HttpPost]
        public TransactResponse PerfilComercialEx(RequestTransact request)
        {
            return transactCORE.PerfilComercialEx(request);
        }

        [Route("UpdateMail")]
        [HttpPost]
        public TransactResponse UpdateMail(RequestTransact request)
        {
            return transactCORE.UpdateMail(request);
        }

        [Route("InsReingresarTransact")]
        [HttpPost]
        public TransactResponse InsReingresarTransact()
        {
            RequestTransact request = new RequestTransact();
            List<HttpPostedFile> files = new List<HttpPostedFile>();
            List<HttpPostedFile> filesCarta = new List<HttpPostedFile>();
            TransactResponse response = new TransactResponse();
            TransactResponse response2 = new TransactResponse();

            try
            {
                foreach (var item in HttpContext.Current.Request.Files)
                {
                    if (item.ToString().IndexOf("carta_agenciamiento_") > -1)
                    {
                        filesCarta.Add((HttpPostedFile)HttpContext.Current.Request.Files[item.ToString()]);
                    }
                    else
                    {
                        HttpPostedFile arch = HttpContext.Current.Request.Files[item.ToString()];
                        files.Add((HttpPostedFile)HttpContext.Current.Request.Files[item.ToString()]);
                    }
                }
                LogControl.save("InsReingresarTransact", HttpContext.Current.Request.Params.Get("objeto"), "2");
                request = JsonConvert.DeserializeObject<RequestTransact>(HttpContext.Current.Request.Params.Get("objeto"));

                string folderPath = "";
                string folderPathCarta = "";
                if (files.Count > 0)
                {
                    folderPath = ELog.obtainConfig("pathTramite") + request.P_NID_COTIZACION + "\\" + ELog.obtainConfig("movimiento") + "\\" + DateTime.Now.ToString("yyyyMMddhhmmss") + "\\";
                    response = sharedMethods.ValidatePath<TransactResponse>(response, folderPath, files);
                    if (response.P_COD_ERR == 1)
                        return response;
                }
                if (filesCarta.Count > 0)
                {
                    folderPathCarta = ELog.obtainConfig("pathTramite") + request.P_NID_COTIZACION + "\\" + ELog.obtainConfig("movimiento") + "\\broker\\" + DateTime.Now.ToString("yyyyMMddhhmmss") + "\\";
                    response = sharedMethods.ValidatePath<TransactResponse>(response, folderPath, files);
                    if (response.P_COD_ERR == 1)
                        return response;
                }
                request.P_SRUTA = folderPath;
                request.P_SLETTER_AGENCY = folderPathCarta;

                response = transactCORE.InsReingresarTransact(request);
                //R.P.
                if (response.P_COD_ERR == 0)
                {
                    if (request.P_STRANSAC == "RE" || request.P_STRANSAC == "EM")
                    {
                        foreach (var dep in request.P_DAT_BROKER)
                        {
                            dep.P_NID_TRAMITE = response.P_NID_TRAMITE;
                            response2 = transactCORE.InsertDepBrkTramite(dep);
                        }
                    }
                }
                //R.P.
                if (response.P_COD_ERR == 0)
                {
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
                    if (filesCarta.Count > 0 && response.P_COD_ERR == 0)
                    {

                        if (!Directory.Exists(String.Format(ELog.obtainConfig("pathPrincipal"), folderPathCarta)))
                        {
                            Directory.CreateDirectory(String.Format(ELog.obtainConfig("pathPrincipal"), folderPathCarta));
                        }

                        foreach (var file in filesCarta)
                        {
                            try
                            {
                                file.SaveAs(String.Format(ELog.obtainConfig("pathPrincipal"), folderPathCarta) + file.FileName.ToString());
                            }
                            catch (Exception)
                            {
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                //response.P_MESSAGE = "Error en el servidor. - " + ex.Message;
                response.P_MESSAGE = "Error en el servidor.";
                LogControl.save("InsReingresarTransact", ex.ToString(), "3");
            }
            return response;
        }

        [Route("GetBrokerObl")]
        [HttpPost]
        public List<BrokerObl> GetBrokerObl(int nbranch)
        {
            return transactCORE.GetBrokerObl(nbranch);
        }

        [Route("MotivoRechazoTransact")]
        [HttpGet]
        public List<MotivoRechazoVM> GetMotivoRechazoTransact()
        {
            return transactCORE.GetMotivoRechazoTransact();
        }

        [Route("ValidarRechazoEjecutivo")]
        [HttpPost]
        public TransactResponse ValidarRechazoEjecutivo(ValidateEjecutivoTransac data)
        {
            var response = new TransactResponse();

            try
            {
                response = transactCORE.ValidarRechazoEjecutivo(data);
            }
            catch (Exception ex)
            {
                LogControl.save("ValidarRechazoEjecutivo", ex.ToString(), "3");
            }

            return response;
        }
    }
}
