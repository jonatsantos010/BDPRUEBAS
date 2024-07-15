using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Http;
using System.Web.Http.Cors;
using WSPlataforma.Util;
using WSPlataforma.Core;
using WSPlataforma.Entities.InmobiliaryModuleReportsModel.BindingModel;
using WSPlataforma.Entities.ReportModel.BindingModel;

namespace WSPlataforma.Controllers
{
    [RoutePrefix("Api/InmoModuleReports")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]

    public class InmobiliaryModuleReportsController : ApiController
    {

        #region Filters

        InmobiliaryModuleReportsCORE ModuleReportsCORE = new InmobiliaryModuleReportsCORE();

        // GENERA REPORTE CONTABLE
        [Route("ProcessReportContable")]
        [HttpPost]
        public IHttpActionResult ProcessReportContable(ModuleReportsFilter data)
        {
            ResponseControl Rpt = new ResponseControl(Response.Ok);
            try
            {
                var response = ModuleReportsCORE.ProcessReportContable_CD(data);
                if (response.Tables[0].Rows.Count > 0 || response.Tables[1].Rows.Count > 0)
                {
                    //Construimos la ruta para obtener el rpte excel contable
                    string directory = ELog.obtainConfig("reportOnlineInterfaceContable");

                    if (Directory.Exists(directory) == false)
                        Directory.CreateDirectory(directory);

                    string file = "RpteContable" + ".xlsx";
                    string route = directory + "\\" + file;

                    if (File.Exists(route))
                    {
                        //Convertimos el archivo en Base64
                        Rpt.Data = Convert.ToBase64String(File.ReadAllBytes(route));
                        File.Delete(route);
                    }
                    else
                    {
                        Rpt.Data = null;
                        Rpt.response = Response.Fail;
                    }
                }
                else
                {
                    Rpt.response = Response.Fail;
                    Rpt.Data = "No se obtuvo información en las fechas solicitadas para el reporte Contable.";
                }
            }
            catch (Exception ex)
            {
                Rpt.response = Response.Fail;
                Rpt.Data = ex.Message;

            }
            return Ok(Rpt);
        }

        // GENERA REPORTE OPERATIVO
        [Route("ProcessReportOperaciones")]
        [HttpPost]
        public IHttpActionResult ProcessReportOperaciones(ModuleReportsFilter data)
        {
            ResponseControl Rpt = new ResponseControl(Response.Ok);
            try
            {
                var response = ModuleReportsCORE.ProcessReportOperaciones_CD(data);
                if (response.Tables[0].Rows.Count > 0 || response.Tables[1].Rows.Count > 0)
                {
                    //Construimos la ruta para obtener el rpte excel operativo
                    string directory = ELog.obtainConfig("reportOnlineInterfaceOperativo");

                    if (Directory.Exists(directory) == false)
                        Directory.CreateDirectory(directory);

                    string file = "RpteOperativo" + ".xlsx";
                    string route = directory + "\\" + file;

                    if (File.Exists(route))
                    {
                        //Convertimos el archivo en Base64
                        Rpt.Data = Convert.ToBase64String(File.ReadAllBytes(route));
                        File.Delete(route);
                    }
                    else
                    {
                        Rpt.Data = null;
                        Rpt.response = Response.Fail;
                    }
                }
                else
                {
                    Rpt.response = Response.Fail;
                    Rpt.Data = "No se obtuvo información en las fechas solicitadas para el reporte Operativo.";
                }
            }
            catch (Exception ex)
            {
                Rpt.response = Response.Fail;
                Rpt.Data = ex.Message;

            }
            return Ok(Rpt);
        }

        // GENERA REPORTE CONTROL BANCARIO
        [Route("ProcessReportControlBancario")]
        [HttpPost]
        public IHttpActionResult ProcessReportControlBancario(ModuleReportsFilter data)
        {
            ResponseControl Rpt = new ResponseControl(Response.Ok);
            try
            {
                var response = ModuleReportsCORE.ProcessReportControlBancario(data);
                if (response.Tables[0].Rows.Count > 0)
                {
                    //Construimos la ruta para obtener el rpte excel contable
                    string directory = ELog.obtainConfig("reportOnlineInterfaceCBCO");

                    if (Directory.Exists(directory) == false)
                        Directory.CreateDirectory(directory);

                    string file = "RpteControlBancario" + ".xlsx";
                    string route = directory + "\\" + file;

                    if (File.Exists(route))
                    {
                        //Convertimos el archivo en Base64
                        Rpt.Data = Convert.ToBase64String(File.ReadAllBytes(route));
                        File.Delete(route);
                    }
                    else
                    {
                        Rpt.Data = null;
                        Rpt.response = Response.Fail;
                    }
                }
                else
                {
                    Rpt.response = Response.Fail;
                    Rpt.Data = "No se obtuvo información en las fechas solicitadas para el reporte de Control Bancario.";
                }
            }
            catch (Exception ex)
            {
                Rpt.response = Response.Fail;
                Rpt.Data = ex.Message;

            }
            return Ok(Rpt);
        }

        // GENERA REPORTE CUENTAS POR COBRAR INMOBILIARIA
        [Route("ProcessReportCuentasPorCobrar")]
        [HttpPost]
        public IHttpActionResult ProcessReportCuentasPorCobrar(ModuleReportsFilter data)
        {
            ResponseControl Rpt = new ResponseControl(Response.Ok);
            try
            {
                var response = ModuleReportsCORE.ProcessReportCuentasPorCobrar(data);
                if (response.Tables[0].Rows.Count > 0)
                {
                    //Construimos la ruta para obtener el rpte excel operativo
                    string directory = ELog.obtainConfig("reportOnlineInterfaceOperativo");

                    if (Directory.Exists(directory) == false)
                        Directory.CreateDirectory(directory);

                    string file = "Reporte_CuentasXCobrar" + ".xlsx";
                    string route = directory + "\\" + file;

                    if (File.Exists(route))
                    {
                        //Convertimos el archivo en Base64
                        Rpt.Data = Convert.ToBase64String(File.ReadAllBytes(route));
                        File.Delete(route);
                    }
                    else
                    {
                        Rpt.Data = null;
                        Rpt.response = Response.Fail;
                    }
                }
                else
                {
                    Rpt.response = Response.Fail;
                    Rpt.Data = "No se obtuvo información en las fechas solicitadas para el reporte de trasferencias bancarias.";
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