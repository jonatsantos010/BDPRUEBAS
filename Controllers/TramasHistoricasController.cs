using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.IO;
using WSPlataforma.Core;
using WSPlataforma.Entities.TramasHistoricasModel.BindingModel;
using WSPlataforma.Entities.ReportModel.BindingModel;
using System.Diagnostics;
using WSPlataforma.Util;

namespace WSPlataforma.Controllers
{
    [RoutePrefix("Api/TramasHistoricas")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]

    public class TramasHistoricasController : ApiController
    {
        TramasHistoricasCORE TramasHistoricasCORE = new TramasHistoricasCORE();
        //
        [Route("ListarProductos")]
        [HttpGet]
        public IHttpActionResult ListarProductos()
        {
            var result = this.TramasHistoricasCORE.ListarProductos();
            return Ok(result);
        }
        //
        [Route("ListarEndosos")]
        [HttpGet]
        public IHttpActionResult ListarEndosos()
        {
            var result = this.TramasHistoricasCORE.ListarEndosos();
            return Ok(result);
        }
        //
        [Route("ListarCabeceras")]
        [HttpPost]
        public IHttpActionResult ListarCabeceras(CabeceraBM data)
        {
            var result = this.TramasHistoricasCORE.ListarCabeceras(data);
            return Ok(result);
        }
        //
        [Route("InsertReportStatus")]
        [HttpPost]
        public IHttpActionResult InsertReportStatus(TramasHistoricasCabeceraBM data)
        {
            string codigoProceso = string.Empty;
            codigoProceso = (data.USER_NAME).ToUpper() + DateTime.Now.ToString("yyyyMMddhhmmssff");
            data.ID_REPORT = codigoProceso.Trim();

            var result = this.TramasHistoricasCORE.InsertReportStatus(data);
            int idCab = result[0].P_NID_CAB;
            long rpta = result.LongCount();

            ReportProcess rpoProc = new ReportProcess();
            rpoProc.P_NCODE = int.Parse(rpta.ToString());
            rpoProc.P_SMESSAGE = "Error";

            try
            {
                if (rpoProc.P_NCODE > 0)
                {
                    string fecInicio = data.P_DSTARTDATE.ToString("dd/MM/yyyy");
                    string fecFin = data.P_DEXPIRDAT.ToString("dd/MM/yyyy");

                    string arguments = string.Format(@"{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}", codigoProceso, data.P_NPRODUCT, fecInicio, fecFin, idCab, 0, 0, 0, 0, 105);
                    using
                    (
                        var process = new Process
                        {
                            StartInfo = new ProcessStartInfo { FileName = ELog.obtainConfig("exePath"), Arguments = arguments }
                        }
                    )
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
        //
        [Route("GetReporteTramasHistoricas")]
        [HttpPost]
        public IHttpActionResult GetReporteTramasHistoricas(TramasHistoricasDetalleBM data)
        {
            ResponseControl Rpt = new ResponseControl(Response.Ok);
            try
            {
                if (data.P_NID_CAB != 0)
                {
                    string directory = ELog.obtainConfig("reportPathTramasH");
                    string directoryReport = data.P_NID_CAB + "\\";
                    string file = data.P_NID_CAB + ".xlsx";
                    string route = directory + "\\" + directoryReport + file;
                    if (File.Exists(route))
                    {
                        Rpt.Data = Convert.ToBase64String(File.ReadAllBytes(route));
                    }
                    string directory2 = ELog.obtainConfig("reportPathTramasH");
                    string directoryReport2 = data.P_NID_CAB + "\\";
                    string file2 = data.P_NID_CAB + ".xlsx";
                    string route2 = directory2 + "\\" + directoryReport2 + file2;
                    if (File.Exists(route2))
                    {
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
            return Ok(Rpt);
        }
    }
}