using System;
using System.IO;
using WSPlataforma.Util;
using System.Web.Http;
using System.Web.Http.Cors;
using WSPlataforma.Core;
using WSPlataforma.Entities.InterfaceMonitoringTesoModel.BindingModel;
using WSPlataforma.Entities.ReportModel.BindingModel;

namespace WSPlataforma.Controllers
{
    [RoutePrefix("Api/InterfaceMonitoringTeso")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]

    public class InterfaceMonitoringTesoController : ApiController
    {
        InterfaceMonitoringTesoCORE InterfaceMonitoringTesoCORE = new InterfaceMonitoringTesoCORE();

        [Route("ListarBankTesoreriaCAB_XLS")]
        [HttpPost]
        public IHttpActionResult ListarBankTesoreriaCAB_XLS(InterfaceMonitoringTesoFilter data)
        {
            ResponseControl Rpt = new ResponseControl(Response.Ok);
            try
            {
                var response = this.InterfaceMonitoringTesoCORE.ListarBankTesoreriaCAB_XLS(data);                
                if (response.Tables[0].Rows.Count > 0)
                {
                    string directory = ELog.obtainConfig("reportOnlinePrelError");

                    if (Directory.Exists(directory) == false)
                        Directory.CreateDirectory(directory);

                    string file = "RptePagoBancoCAB.xlsx";
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

        [Route("ListarBankTesoreriaDET_XLS")]
        [HttpPost]
        public IHttpActionResult ListarBankTesoreriaDET_XLS(InterfaceMonitoringDetTesoFilter data)
        {
            ResponseControl Rpt = new ResponseControl(Response.Ok);
            try
            {
                var response = this.InterfaceMonitoringTesoCORE.ListarBankTesoreriaDET_XLS(data);
                if (response.Tables[0].Rows.Count > 0 )
                {
                    string directory = ELog.obtainConfig("reportOnlinePrelError");

                    if (Directory.Exists(directory) == false)
                        Directory.CreateDirectory(directory);

                    string file = "RptePagoBancoDET.xlsx";
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



        // TESORERÍA
        [Route("ListarBancosTesoreria")]
        [HttpGet]
        public IHttpActionResult ListarBancosTesoreria()
        {
            var result = this.InterfaceMonitoringTesoCORE.ListarBancosTesoreria();
            return Ok(result);
        }
        [Route("ListarBuscarPor")]
        [HttpGet]
        public IHttpActionResult ListarBuscarPor()
        {
            var result = this.InterfaceMonitoringTesoCORE.ListarBuscarPor();
            return Ok(result);
        }
        [Route("ListarEstadosTesoreria")]
        [HttpPost]
        public IHttpActionResult ListarEstadosTesoreria(EstadosTesoreríaFilter data)
        {
            var result = this.InterfaceMonitoringTesoCORE.ListarEstadosTesoreria(data);
            return Ok(result);
        }
        [Route("ListarAprobaciones")]
        [HttpPost]
        public IHttpActionResult ListarAprobaciones(AprobacionesFilter data)
        {
            var result = this.InterfaceMonitoringTesoCORE.ListarAprobaciones(data);
            return Ok(result);
        }
        [Route("ListarAprobacionesDoc")]
        [HttpPost]
        public IHttpActionResult ListarAprobacionesDoc(AprobacionesDocFilter data)
        {
            var result = this.InterfaceMonitoringTesoCORE.ListarAprobacionesDoc(data);
            return Ok(result);
        }
        [Route("ListarAprobacionesDetalle")]
        [HttpPost]
        public IHttpActionResult ListarAprobacionesDetalle(AprobacionesDetalleFilter data)
        {
            var result = this.InterfaceMonitoringTesoCORE.ListarAprobacionesDetalle(data);
            return Ok(result);
        }
        [Route("ListarAprobacionesDetalleDoc")]
        [HttpPost]
        public IHttpActionResult ListarAprobacionesDetalleDoc(AprobacionesDetalleDocFilter data)
        {
            var result = this.InterfaceMonitoringTesoCORE.ListarAprobacionesDetalleDoc(data);
            return Ok(result);
        }
        [Route("AgregarDetalleObservacion")]
        [HttpPost]
        public IHttpActionResult AgregarDetalleObservacion(AgregarDetalleObservacionFilter data)
        {
            var result = this.InterfaceMonitoringTesoCORE.AgregarDetalleObservacion(data);
            return Ok(result);
        }
        [Route("ListarDetalleObservacion")]
        [HttpPost]
        public IHttpActionResult ListarDetalleObservacion(ListarDetalleObservacionFilter data)
        {
            var result = this.InterfaceMonitoringTesoCORE.ListarDetalleObservacion(data);
            return Ok(result);
        }
        [Route("AprobarProcesoList")]
        [HttpPost]
        public IHttpActionResult AprobarProcesoList(AprobarProcesoListFilter data)
        {
            var result = this.InterfaceMonitoringTesoCORE.AprobarProcesoList(data);
            return Ok(result);
        }
        [Route("ResolverObservacionList")]
        [HttpPost]
        public IHttpActionResult ResolverObservacionList(ResolverObservacionListFilter data)
        {
            var result = this.InterfaceMonitoringTesoCORE.ResolverObservacionList(data);
            return Ok(result);
        }
        [Route("GetHorarioInterfaz")]
        [HttpPost]
        public IHttpActionResult GetHorarioInterfaz(HorarioBM data)
        {
            var result = this.InterfaceMonitoringTesoCORE.GetHorarioInterfaz(data);
            return Ok(result);
        }
    }
}