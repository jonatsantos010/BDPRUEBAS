using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Http;
using System.Web.Http.Cors;
using WSPlataforma.Core;
using WSPlataforma.Entities.NotasCreditoModel.BindingModel;
using WSPlataforma.Entities.ReportModel.BindingModel;
using System.Diagnostics;
using WSPlataforma.Util;

namespace WSPlataforma.Controllers
{
    [RoutePrefix("Api/NotasCredito")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class NotaCreditoController : ApiController
    {
        #region Filters
        NotaCreditoCORE ReportNotaCreditoCORE = new NotaCreditoCORE();
        [Route("GetBranch")]
        [HttpGet]
        public IHttpActionResult GetBranches()
        {
            var result = this.ReportNotaCreditoCORE.GetBranch();
            return Ok(result);
        }
        #endregion

        #region Report Notas de Credito

        [Route("ProcessReport")]
        [HttpPost]
        public IHttpActionResult InsertReportStatus(ReporteNotaCreditoFilter data)
        {
            string codigoProceso = string.Empty;
            codigoProceso = (data.UserName).ToUpper() + DateTime.Now.ToString("yyyyMMddhhmmssff");
            data.IdReport = codigoProceso.Trim();

            var result = this.ReportNotaCreditoCORE.InsertReportStatus(data);

            long rpta = result.LongCount();

            ReportProcess rpoProc = new ReportProcess();

            rpoProc.P_NCODE = int.Parse(rpta.ToString());
            rpoProc.P_SMESSAGE = "Error";

            //inicio proceso para generar el reporte Asesoria Broker
            try
            {
                if (rpoProc.P_NCODE > 0)
                {
                    //NUEVOS ARGUMENTOS PARA VALIDACION PRIMAS COMISIONES
                    string fecInicio = data.StartDate.ToString("dd/MM/yyyy");
                    string fecFin = data.EndDate.ToString("dd/MM/yyyy");

                    string arguments = string.Format(@"{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}", codigoProceso,
                        data.BranchId, fecInicio, fecFin, 0, 0, 0, 0, 0, 102);
                    using (var process = new Process
                    {
                        StartInfo =
                    new ProcessStartInfo { FileName = ELog.obtainConfig("exePath"), Arguments = arguments }
                    })
                    {
                        process.Start();
                    }

                }
                else
                {
                    return Ok(rpoProc.P_SMESSAGE);
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            if (rpoProc.P_NCODE > 0)
            {
                return Ok(result);
            }
            else
            {
                return Ok(rpoProc.P_SMESSAGE);
            }
        }


        //Servicio para obtener status de reportes
        [Route("GetStatusReport")]
        [HttpPost]
        public IHttpActionResult GetStatusReport(ReporteNotaCreditoFilter data)

        {
            var result = this.ReportNotaCreditoCORE.GetReportStatus(data);
            return Ok(result);
        }


        //Servicio para Obtener Archivo(Excel)
        [Route("GetReportBrokerXLS")]
        [HttpPost]
        public IHttpActionResult getFileNotasCreditoReport(ReporteNotaCreditoFilter data)
        {
            ResponseControl Rpt = new ResponseControl(Response.Ok);
            try
            {
                if (data.IdReport != null || data.IdReport != "")
                {
                    //Construimos la ruta para obtener el archivo excel
                    string directory = ELog.obtainConfig("reportPathCreditNote");
                    string directoryReport = data.IdReport + "\\";
                    string file = data.IdReport.ToUpper() + ".xlsx";
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

        #endregion
    }
}