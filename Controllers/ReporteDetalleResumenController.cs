using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.IO;
using WSPlataforma.Core;
using WSPlataforma.Entities.ReporteDetalleResumenModel.BindingModel;
using WSPlataforma.Entities.ReportModel.BindingModel;
using System.Diagnostics;
using WSPlataforma.Util;

namespace WSPlataforma.Controllers
{
    [RoutePrefix("Api/ReporteDetalleResumen")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ReporteDetalleResumenController : ApiController
    {
        #region Filters
        ReporteDetalleResumenCORE reportDetalleResumenCORE = new ReporteDetalleResumenCORE();
        [Route("GetBranch")]
        [HttpGet]
        public IHttpActionResult GetBranches(string SREPORT)
        {
            var result = this.reportDetalleResumenCORE.GetBranch(SREPORT);
            return Ok(result);
        }
        #endregion

        #region Reporte de Soat Produccion

        [Route("ProcessReport")]
        [HttpPost]
        public IHttpActionResult InsertReportStatus(ReporteDetalleResumenFilter data)
        {
            string codigoProceso = string.Empty;
            codigoProceso = (data.UserName).ToUpper() + DateTime.Now.ToString("yyyyMMddhhmmssff");
            data.IdReport = codigoProceso.Trim();

            var result = this.reportDetalleResumenCORE.InsertReportStatus(data);

            long rpta = result.LongCount();

            ReportProcess rpoProc = new ReportProcess();

            rpoProc.P_NCODE = int.Parse(rpta.ToString());
            rpoProc.P_SMESSAGE = "Error";

            //inicio proceso para generar el reporte preliminar detalle producción SOAT
            try
            {
                if (rpoProc.P_NCODE > 0)
                {
                    //NUEVOS ARGUMENTOS PARA VALIDACION PRIMAS COMISIONES
                    string fecInicio = data.StartDate.ToString("dd/MM/yyyy");
                    string fecFin = data.EndDate.ToString("dd/MM/yyyy");

                    string arguments = string.Format(@"{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}", codigoProceso,
                        data.BranchId, fecInicio, fecFin, 1, 1, 1, 1, 1, data.estados, data.SREPORT);
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
        public IHttpActionResult GetStatusReport(ReporteDetalleResumenFilter data)

        {
            var result = this.reportDetalleResumenCORE.GetReportStatus(data);
            return Ok(result);
        }

        //Servicio para Obtener Archivo(Excel)
        [Route("getFileReporteDetalleResumen")]
        [HttpPost]
        public IHttpActionResult GetFilePremiumReport(ReporteDetalleResumenFilter data)
        {
            ResponseControl Rpt = new ResponseControl(Response.Ok);
            try
            {
                /*
                var DetalleResumenNotas = "-N";
                var DetalleResumenPrimas = "-P";
                */
                if (data.IdReport != null || data.IdReport != "")
                {
                    //Construimos la ruta para obtener el archivo de cabecera
                    string directory = ELog.obtainConfig("reportPathRPTDetalleResumen") + "\\" + data.SREPORT + "\\";
                    string directoryReport = data.IdReport.ToUpper() + "\\";
                    string file = data.IdReport.ToUpper() + /*DetalleResumenNotas +*/ ".xlsx";
                    string route = directory + "\\" + directoryReport + file;

                    LogControl.save("GetFilePremiumReport", "Reporte Comisiones "+ route, "3");

                    if (File.Exists(route))
                    {
                        //Convertimos el archivo en Base64
                        Rpt.Data = Convert.ToBase64String(File.ReadAllBytes(route));
                    }

                    //Construimos la ruta para obtener el archivo de detalle
                    /*
                     * string directory2 = ELog.obtainConfig("reportPathRPTDetalleResumen") + data.SREPORT + "\"";
                    string directoryReport2 = data.IdReport + "\\";
                    string file2 = data.IdReport.ToUpper() + DetalleResumenPrimas + ".xlsx";
                    string route2 = directory2 + "\\" + directoryReport2 + file2;
                    if (File.Exists(route2))
                    {
                        //Convertimos el archivo en Base64
                        Rpt.Data2 = Convert.ToBase64String(File.ReadAllBytes(route2));
                    }*/
                }
                else
                {
                    Rpt.Data = null;
                    Rpt.Data2 = null;
                    Rpt.response = Response.Fail;
                }
            }
            catch (Exception ex)
            {
                Rpt.response = Response.Fail;
                Rpt.Data = ex.Message;
                Rpt.Data2 = ex.Message;

            }
            // var response = GetFilePath(id);
            return Ok(Rpt);
        }









        #endregion

    }
}