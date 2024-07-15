using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.IO;
using WSPlataforma.Core;
using WSPlataforma.Entities.ReporteSoatModel.BindingModel;
using WSPlataforma.Entities.ReportModel.BindingModel;
using System.Diagnostics;
using WSPlataforma.Util;

namespace WSPlataforma.Controllers
{
    [RoutePrefix("Api/ReporteSoat")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ReporteSoatController : ApiController
    {
        #region Filters
        ReporteSoatCORE reportSoatCORE = new ReporteSoatCORE();
        [Route("GetBranch")]
        [HttpGet]
        public IHttpActionResult GetBranches()
        {
            var result = this.reportSoatCORE.GetBranch();
            return Ok(result);
        }
        #endregion

        #region Reporte de Soat Produccion

        [Route("ProcessReport")]
        [HttpPost]
        public IHttpActionResult InsertReportStatus(ReporteSoatFilter data)
        {
            string codigoProceso = string.Empty;
            codigoProceso = (data.UserName).ToUpper() + DateTime.Now.ToString("yyyyMMddhhmmssff");
            data.IdReport = codigoProceso.Trim();

            var result = this.reportSoatCORE.InsertReportStatus(data);

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

                    string arguments = string.Format(@"{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}", codigoProceso,
                        data.BranchId, fecInicio, fecFin, 0, 0, 0, 0, 0, 104);
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
        public IHttpActionResult GetStatusReport(ReporteSoatFilter data)

        {
            var result = this.reportSoatCORE.GetReportStatus(data);
            return Ok(result);
        }

        //Servicio para Obtener Archivo(Excel)
        [Route("getFileReporteSoat")]
        [HttpPost]
        public IHttpActionResult GetFilePremiumReport(ReporteSoatFilter data)
        {
            ResponseControl Rpt = new ResponseControl(Response.Ok);
            try
            {
                var soatNotas = "-N";
                var soatPrimas = "-P";

                if (data.IdReport != null || data.IdReport != "")
                {
                    //Construimos la ruta para obtener el archivo de cabecera
                    string directory = ELog.obtainConfig("reportPathRPTSoat");
                    string directoryReport = data.IdReport + "\\";
                    string file = data.IdReport.ToUpper() + soatNotas + ".xlsx";
                    string route = directory + "\\" + directoryReport + file;
                    if (File.Exists(route))
                    {
                        //Convertimos el archivo en Base64
                        Rpt.Data = Convert.ToBase64String(File.ReadAllBytes(route));
                    }

                    //Construimos la ruta para obtener el archivo de detalle
                    string directory2 = ELog.obtainConfig("reportPathRPTSoat");
                    string directoryReport2 = data.IdReport + "\\";
                    string file2 = data.IdReport.ToUpper() + soatPrimas + ".xlsx";
                    string route2 = directory2 + "\\" + directoryReport2 + file2;
                    if (File.Exists(route2))
                    {
                        //Convertimos el archivo en Base64
                        Rpt.Data2 = Convert.ToBase64String(File.ReadAllBytes(route2));
                    }
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