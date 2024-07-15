
using System.Collections.Generic;

using System;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.IO;
using WSPlataforma.Core;
using WSPlataforma.Entities.CtaXcobrarModel.BindingModel;
using System.Diagnostics;
using WSPlataforma.Util;
using WSPlataforma.Entities.ReportModel.BindingModel;

namespace WSPlataforma.Controllers
{
    [RoutePrefix("Api/CtasXcobrar")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CtaXcobrarController : ApiController
    {
        #region Filters
        CtaXcobrarCORE ReportClossingCORE = new CtaXcobrarCORE();
        [Route("GetBranch")]
        [HttpGet]
        public IHttpActionResult GetBranches()
        {
            var result = this.ReportClossingCORE.GetBranch();
            return Ok(result);
        }
        #endregion
        #region Reporte de Cierre Produccion

        [Route("ProcessReport")]
        [HttpPost]
        public IHttpActionResult InsertReportStatus(CtaXcobrarFilter data)
        {
            string codigoProceso = string.Empty;
            codigoProceso = (data.UserName).ToUpper() + DateTime.Now.ToString("yyyyMMddhhmmssff");
            data.IdReport = codigoProceso.Trim();

            var result = this.ReportClossingCORE.InsertReportStatus(data);

            long rpta = result.LongCount();

            ReportProcess rpoProc = new ReportProcess();

            rpoProc.P_NCODE = int.Parse(rpta.ToString());
            rpoProc.P_SMESSAGE = "Error";

            //inicio proceso para generar el reporte Asesoria Broker
            try
            {
                if (rpoProc.P_NCODE > 0)
                {


                    string arguments = string.Format(@"{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}", codigoProceso,
                        data.BranchId, 0, 0, 0, 0, 0, 0, 0, 103);
                    using (var process = new Process
                    {
                        StartInfo = new ProcessStartInfo { FileName = ELog.obtainConfig("exePath"), Arguments = arguments }
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
        public IHttpActionResult GetStatusReport(CtaXcobrarFilter data)

        {
            var result = this.ReportClossingCORE.GetReportStatus(data);
            return Ok(result);
        }


        //Servicio para Obtener Archivo(Excel)
        [Route("getFileReporteCierre")]
        [HttpPost]
        public IHttpActionResult getFileReporteCierre(CtaXcobrarFilter data)
        {
            ResponseControl Rpt = new ResponseControl(Response.Ok);
            try
            {
                if (data.IdReport != null || data.IdReport != "")
                {
                    //Construimos la ruta para obtener el archivo excel
                    string directory = ELog.obtainConfig("reportPathCtaXcobrar");
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
