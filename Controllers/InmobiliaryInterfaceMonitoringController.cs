using System;
using System.IO;
using WSPlataforma.Util;
using System.Web.Http;
using System.Web.Http.Cors;
using WSPlataforma.Core;
using WSPlataforma.Entities.InmobiliaryInterfaceMonitoringModel.BindingModel;
using WSPlataforma.Entities.ReportModel.BindingModel;
namespace WSPlataforma.Controllers
{    

    [RoutePrefix("Api/InmoInterfaceMonitoring")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]

    public class InmobiliaryInterfaceMonitoringController : ApiController
    {
        InmobiliaryInterfaceMonitoringCORE InterfaceMonitoringCORE = new InmobiliaryInterfaceMonitoringCORE();

        // ESTADOS DE PROCESO
       //[Route("ListarEstados")]
       //[HttpGet]
       //public IHttpActionResult ListarEstados(ProcFilter data)
       //{
       //    var result = this.InterfaceMonitoringCORE.ListarEstados(data);
       //    return Ok(result);
       //}

        [Route("ListarEstados")]
        [HttpPost]
        public IHttpActionResult ListarEstados(ProcFilter data)
        {
            var result = this.InterfaceMonitoringCORE.ListarEstados(data);
            return Ok(result);
        }

        // CABECERA DE INTERFACES
        [Route("ListarCabeceraInterfaces")]
        [HttpPost]
        public IHttpActionResult ListarCabeceraInterfaces(InterfaceMonitoringFilter data)
        {
            var result = this.InterfaceMonitoringCORE.ListarCabeceraInterfaces(data);
            return Ok(result);
        }
        [Route("ListarCabeceraInterfacesRecibo")]
        [HttpPost]
        public IHttpActionResult ListarCabeceraInterfacesRecibo(InterfaceMonitoringReciboFilter data)
        {
            var result = this.InterfaceMonitoringCORE.ListarCabeceraInterfacesRecibo(data);
            return Ok(result);
        }

        // DETALLE PRELIMINAR
        [Route("ListarDetalleInterfaces")]
        [HttpPost]
        public IHttpActionResult ListarDetalleInterfaces(InterfaceMonitoringDetailFilter data)
        {
            var result = this.InterfaceMonitoringCORE.ListarDetalleInterfaces(data);
            return Ok(result);
        }
        [Route("ListarErroresDetalle")]
        [HttpPost]
        public IHttpActionResult ListarErroresDetalle(InterfaceMonitoringDetailErrorFilter data)
        {
            var result = this.InterfaceMonitoringCORE.ListarErroresDetalle(data);
            return Ok(result);
        }

        // DETALLE ASIENTOS
        [Route("ListarDetalleInterfacesAsientosContables")]
        [HttpPost]
        public IHttpActionResult ListarDetalleInterfacesAsientosContables(InterfaceMonitoringAsientosContablesDetailFilter data)
        {
            var result = this.InterfaceMonitoringCORE.ListarDetalleInterfacesAsientosContables(data);
            return Ok(result);
        }
        [Route("ListarDetalleInterfacesAsientosContablesAsiento")]
        [HttpPost]
        public IHttpActionResult ListarDetalleInterfacesAsientosContablesAsiento(InterfaceMonitoringAsientosContablesDetailAsientoFilter data)
        {
            var result = this.InterfaceMonitoringCORE.ListarDetalleInterfacesAsientosContablesAsiento(data);
            return Ok(result);
        }
        [Route("ListarDetalleInterfacesAsientosContablesError")]
        [HttpPost]
        public IHttpActionResult ListarDetalleInterfacesAsientosContablesError(InterfaceMonitoringAsientosContablesDetailErrorFilter data)
        {
            var result = this.InterfaceMonitoringCORE.ListarDetalleInterfacesAsientosContablesError(data);
            return Ok(result);
        }

        // DETALLE EXACTUS
        [Route("ListarDetalleInterfacesExactus")]
        [HttpPost]
        public IHttpActionResult ListarDetalleInterfacesExactus(InterfaceMonitoringExactusDetailFilter data)
        {
            var result = this.InterfaceMonitoringCORE.ListarDetalleInterfacesExactus(data);
            return Ok(result);
        }
        [Route("ListarDetalleInterfacesExactusAsiento")]
        [HttpPost]
        public IHttpActionResult ListarDetalleInterfacesExactusAsiento(InterfaceMonitoringExactusDetailAsientoFilter data)
        {
            var result = this.InterfaceMonitoringCORE.ListarDetalleInterfacesExactusAsiento(data);
            return Ok(result);
        }
        [Route("ListarDetalleInterfacesExactusError")]
        [HttpPost]
        public IHttpActionResult ListarDetalleInterfacesExactusError(InterfaceMonitoringExactusDetailErrorFilter data)
        {
            var result = this.InterfaceMonitoringCORE.ListarDetalleInterfacesExactusError(data);
            return Ok(result);
        }

        // TIPO BUSQUEDA
        [Route("ListarTipoBusqueda")]
        [HttpGet]
        public IHttpActionResult ListarTipoBusqueda()
        {
            var result = this.InterfaceMonitoringCORE.ListarTipoBusqueda();
            return Ok(result);
        }
        [Route("ListarTipoBusquedaSI")]
        [HttpPost]
        public IHttpActionResult ListarTipoBusquedaSI(TipoBusquedaFilter data)
        {
            var result = this.InterfaceMonitoringCORE.ListarTipoBusquedaSI(data);
            return Ok(result);
        }

        // DETALLE OPERACIONES
        [Route("ListarDetalleOperacion")]
        [HttpPost]
        public IHttpActionResult ListarDetalleOperacion(InterfaceMonitoringDetailOperacionFilter data)
        {
            var result = this.InterfaceMonitoringCORE.ListarDetalleOperacion(data);
            return Ok(result);
        }

        // REPORTES PRELIMINARES
        [Route("ListarDetalleInterfacesXLSX")]
        [HttpPost]
        public IHttpActionResult ListarDetalleInterfacesXLSX(InterfaceMonitoringDetailXLSXFilter data)
        {
            ResponseControl Rpt = new ResponseControl(Response.Ok);
            try
            {
                var response = this.InterfaceMonitoringCORE.ListarDetalleInterfacesXLSX(data);
                if (response.Tables[0].Rows.Count > 0 || response.Tables[1].Rows.Count > 0)
                {
                    string directory = ELog.obtainConfig("reportOnlinePrelError");

                    if (Directory.Exists(directory) == false)
                        Directory.CreateDirectory(directory);

                    string file = "RptePreliminarERR - " + data.NIDPROCESS + ".xlsx";
                    string route = directory + "\\" + file;

                    if (File.Exists(route))
                    {
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
        [Route("ListarDetalleInterfacesXLSX_CB")]
        [HttpPost]
        public IHttpActionResult ListarDetalleInterfacesXLSX_CB(InterfaceMonitoringDetailXLSXFilter_CB data)
        {
            ResponseControl Rpt = new ResponseControl(Response.Ok);
            try
            {
                var response = this.InterfaceMonitoringCORE.ListarDetalleInterfacesXLSX_CB(data);
                if (response.Tables[0].Rows.Count > 0 || response.Tables[1].Rows.Count > 0)
                {
                    string directory = ELog.obtainConfig("reportOnlinePrelError");

                    if (Directory.Exists(directory) == false)
                        Directory.CreateDirectory(directory);

                    string file = "RptePreliminarERR - " + data.NIDPROCESS + ".xlsx";
                    string route = directory + "\\" + file;

                    if (File.Exists(route))
                    {
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
    }
}