using System.Web.Http;
using System.Web.Http.Cors;
using WSPlataforma.Core;
using WSPlataforma.Entities.PreliminaryModel.BindingModel;
using WSPlataforma.Entities.ReportModel.BindingModel;
using System;
using System.IO;
using WSPlataforma.Entities.ReportModel.ViewModel;
using System.Diagnostics;
using WSPlataforma.Util;
using System.Threading;

namespace WSPlataforma.Controllers
{
    [RoutePrefix("Api/Preliminary")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PreliminaryController : ApiController
    {
        PreliminaryCore PreliminaryCore = new PreliminaryCore();

        #region Filters

        [Route("Branches")]
        [HttpGet]
        public IHttpActionResult GetBranches()
        {
            var result = this.PreliminaryCore.GetBranches();
            return Ok(result);
        }

        [Route("BranchPeriod")]
        [HttpGet]
        public IHttpActionResult GetBranchPeriod(int branchId, int ReportId)
        {

            PreliminaryFilter data = new PreliminaryFilter();
            data.BranchId = branchId;
            data.ReportId = ReportId;
            var result = this.PreliminaryCore.GetBranchPeriod(data);
            return Ok(result);
        }

        [Route("TypeProcess")]
        [HttpGet]
        public IHttpActionResult GetTypeProcess()
        {
            var result = this.PreliminaryCore.GetTypeProcess();
            return Ok(result);
        }

        [Route("Reports")]
        [HttpGet]
        public IHttpActionResult GetReports()
        {
            var result = this.PreliminaryCore.GetReports();
            return Ok(result);
        }

        #endregion

        #region Validations

        [Route("GenerateReports")]
        [HttpPost]
        public IHttpActionResult GetPreliminaryValidation(PreliminaryFilter data)
        {
            var result = this.PreliminaryCore.GetPreliminaryValidation(data);

            int cantResult = result.Count - 1;
            for (int i = 0; i <= cantResult; i++)
            {

                //PREPARAMOS PARAMETROS PARA INVOCAR EL SERVICIO

                int P_NBRANCH_ID = data.BranchId;
                System.DateTime P_DDAT_PROCESS_INI = data.StartDate;
                System.DateTime P_DDAT_PROCESS_FIN = data.EndDate;
                int P_NTYPE_PROCESS = data.TypeProcess;
                int P_NPRELCAB_ID = result[i].PreliminaryId;
                int P_NVALIDATION = data.IdPeriodo;

                int prod;
                if (data.ProductN == "" || data.ProductN == null)
                {
                    prod = 0;
                }
                else
                {
                    prod = Int32.Parse(data.ProductN);
                }

                //IdAutogenerado concatenado con codigo de usuario y fecha actual
                ReportProcess r = new ReportProcess();
                var codigoProceso = string.Empty;
                var branchDes = string.Empty;
                int typePreliminary = result[i].ReportTypeId; //1: primas y comisiones - 2: cobranzas
                r.desUsuario = data.UserName;
                r.desRamo = result[0].BranchName.Trim();
                codigoProceso = (r.desUsuario).ToUpper() + DateTime.Now.ToString("yyyyMMddhhmmssff");

                r.idProceso = codigoProceso;
                r.fecInicio = P_DDAT_PROCESS_INI.ToString("dd/MM/yyyy");
                r.fecFin = P_DDAT_PROCESS_FIN.ToString("dd/MM/yyyy");
                r.codRamo = P_NBRANCH_ID;
                r.codTipo = P_NTYPE_PROCESS.ToString();
                r.codProducto = P_NPRELCAB_ID;
                r.productN = prod.ToString();
                branchDes = r.desRamo.Replace(" ", string.Empty).ToUpper();

                //nombre de usuario fecha con milesimas                
                var response = PreliminaryCore.InsertProcessPremiumReport(r);
                var pMessage = response.Message;

                //Respuesta
                try
                {
                    if (response.response == 0)
                    {
                        //NUEVOS ARGUMENTOS PARA VALIDACION PRIMAS COMISIONES
                        string arguments = string.Format(@"{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}", codigoProceso,
                            P_NBRANCH_ID, r.fecInicio, r.fecFin, P_NTYPE_PROCESS, P_NPRELCAB_ID, P_NVALIDATION, typePreliminary, prod, 999);
                        using (var process = new Process
                        {
                            StartInfo = new ProcessStartInfo { FileName = ELog.obtainConfig("exePath"), Arguments = arguments }
                        })
                        {
                            process.Start();
                        }
                        response.Data = codigoProceso.ToString();
                        response.response = Response.Ok;
                    }
                    else
                    {
                        response.Data = string.Empty;
                        response.Message = pMessage;
                        response.response = Response.Fail;
                    }
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return Ok(pMessage);
                }
            }
            return Ok(result);
        }


        //INI MMQ 14-04-2021 REQ-CSTR-002
        [Route("GetMonitorProcess")]
        [HttpPost]
        public IHttpActionResult GetMonitorProcess(PreliminaryFilter data)

        {
            var result = this.PreliminaryCore.GetMonitorProcess(data);
            return Ok(result);
        }

        //Servicio para Obtener Archivo(Excel)
        [Route("getFilePreliminaryReport")]
        [HttpPost]
        public IHttpActionResult GetFilePreliminaymReport(PreliminaryFilter data)
        {
            ResponseControl Rpt = new ResponseControl(Response.Ok);
            try
            {
                if (data.IDMONITOR != null || data.IDMONITOR != "")
                {
                    //Construimos la ruta para obtener el archivo excel
                    string directory = ELog.obtainConfig("reportPathPreliminay");
                    string directoryReport = data.IDMONITOR + "\\";
                    string file = data.IDMONITOR.ToUpper() + ".xlsx";
                    string route = directory + "\\" + directoryReport + file;
                    if (File.Exists(route))
                    {
                        //Convertimos el archivo en Base64
                        Rpt.Data = Convert.ToBase64String(File.ReadAllBytes(route));
                    }
                }
                else
                {
                    Rpt.Data = null;
                    Rpt.response = Response.Fail;
                }
            }
            catch (Exception ex)
            {
                Rpt.response = Response.Fail;
                Rpt.Data = ex.Message;

            }
            return Ok(Rpt);
        }
        //FIN MMQ 14-04-2021 REQ-CSTR-002
        #endregion

    }
}
