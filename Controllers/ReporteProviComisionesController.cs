using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.IO;
using WSPlataforma.Core;
using WSPlataforma.Entities.ReporteProviComisionesModel.BindingModel;
using WSPlataforma.Entities.ReportModel.BindingModel;
using WSPlataforma.Entities.PrintPolicyModel.ATP;
using WSPlataforma.Entities.ReportModel.ViewModel;
using System.Diagnostics;
using WSPlataforma.Util;

namespace WSPlataforma.Controllers
{
    [RoutePrefix("Api/ProviComisiones")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ReporteProviComisionesController : ApiController
    {
        #region Filters
        ReporteProviComisionesCORE reportProviComisionesCORE = new ReporteProviComisionesCORE();
        [Route("GetBranch")]
        [HttpGet]
        public IHttpActionResult GetBranches()
        {
            var result = this.reportProviComisionesCORE.GetBranch();
            return Ok(result);
        }
        #endregion

        #region Reporte de Provision Comisiones

        [Route("ProcessReport")]
        [HttpPost]
        public IHttpActionResult InsertReportStatus(ReporteProviComisionesFilter data)
        {
            string codigoProceso = string.Empty;
            string fecStart = data.StartDate.ToString("ddMMyyyy");
            string fecEnd = data.EndDate.ToString("ddMMyyyy");
            codigoProceso = (data.UserName).ToUpper() + DateTime.Now.ToString("yyyyMMddhhmmssff")+ fecStart +fecEnd;
            data.IdReport = codigoProceso.Trim();

            var result = this.reportProviComisionesCORE.InsertReportStatus(data);

            long rpta = result.LongCount();
            //ReporteProviComisionesBM test = new ReporteProviComisiones();
            ReportProcess rpoProc = new ReportProcess();

            rpoProc.P_NCODE = int.Parse(rpta.ToString());
            rpoProc.P_SMESSAGE = "Error";

            //inicio proceso para generar el reporte Provision de comisiones
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
        public IHttpActionResult GetStatusReport(ReporteProviComisionesFilter data)

        {
            var result = this.reportProviComisionesCORE.GetReportStatus(data);
            return Ok(result);
        }

        //Servicio para Obtener Archivo(Excel)
        [Route("getFileReporteProviComisiones")]
        [HttpPost]
        public IHttpActionResult GetFileProviComisionesReport(ReporteProviComisionesFilter data)
        {
            ResponseControl Rpt = new ResponseControl(Response.Ok);
            try
            {
                var ProviComisionesNotas = "-N";
                var ProviComisionesPrimas = "-P";

                if (data.IdReport != null || data.IdReport != "")
                {
                    //Construimos la ruta para obtener el archivo de cabecera
                    string directory = ELog.obtainConfig("reportPathRPTSoat");
                    string directoryReport = data.IdReport + "\\";
                    string file = data.IdReport.ToUpper() + ProviComisionesNotas + ".xlsx";
                    string route = directory + "\\" + directoryReport + file;
                    if (File.Exists(route))
                    {
                        //Convertimos el archivo en Base64
                        Rpt.Data = Convert.ToBase64String(File.ReadAllBytes(route));
                    }

                    //Construimos la ruta para obtener el archivo de detalle
                    string directory2 = ELog.obtainConfig("reportPathRPTSoat");
                    string directoryReport2 = data.IdReport + "\\";
                    string file2 = data.IdReport.ToUpper() + ProviComisionesPrimas + ".xlsx";
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



        /*reporte comisiones de desgravamen con devolución*/
        [Route("ReportProviComisionesById")]
        [HttpPost]
        public ResponseReportATPVM ReportProviComisionesById(RequestReportProviComisionesById data)
        {
            return this.reportProviComisionesCORE.ReportProviComisionesById(data);
        }







        #endregion

    }
}