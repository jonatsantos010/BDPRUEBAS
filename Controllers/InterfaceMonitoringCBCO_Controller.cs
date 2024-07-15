using System;
using System.IO;
using WSPlataforma.Util;
using System.Web.Http;
using System.Web.Http.Cors;
using WSPlataforma.Core;
using WSPlataforma.Entities.InterfaceMonitoringModel.BindingModel;
using WSPlataforma.Entities.ReportModel.BindingModel;

namespace WSPlataforma.Controllers
{
    [RoutePrefix("Api/InterfaceMonitoringCBCO")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]

    public class InterfaceMonitoringCBCO_Controller : ApiController
    {
        InterfaceMonitoringCBCO_CORE InterfaceMonitoringCBCO_CORE = new InterfaceMonitoringCBCO_CORE();

        // ESTADOS DE PROCESO
        [Route("ListarEstados")]
        [HttpGet]
        public IHttpActionResult ListarEstados()
        {
            var result = this.InterfaceMonitoringCBCO_CORE.ListarEstados();
            return Ok(result);
        }
        
        // CABECERA DE INTERFACES
        [Route("ListarCabeceraInterfacesCBCO")]
        [HttpPost]
        public IHttpActionResult ListarCabeceraInterfacesCBCO(InterfaceMonitoringFilter data)
        {
            var result = this.InterfaceMonitoringCBCO_CORE.ListarCabeceraInterfaces_CBCO(data);
            return Ok(result);
        }

        // CABECERA POR RECIBO (BUSQUEDA)
        [Route("ListarCabeceraInterfacesCBCORecibo")]
        [HttpPost]
        public IHttpActionResult ListarCabeceraInterfacesCBCORecibo(InterfaceMonitoringReciboFilter data)
        {
            var result = this.InterfaceMonitoringCBCO_CORE.ListarCabeceraInterfacesCBCORecibo(data);
            return Ok(result);
        }

        // DETALLE PRELIMINAR
        [Route("ListarDetalleInterfacesCBCO")]
        [HttpPost]
        public IHttpActionResult ListarDetalleInterfaces(InterfaceMonitoringDetailFilter data)
        {
            var result = this.InterfaceMonitoringCBCO_CORE.ListarDetalleInterfaces(data);
            return Ok(result);
        }

        [Route("ListarDetalleOperacion")]
        [HttpPost]
        public IHttpActionResult ListarDetalleOperacion(InterfaceMonitoringDetailOperacionFilter data)
        {
            var result = this.InterfaceMonitoringCBCO_CORE.ListarDetalleOperacion(data);
            return Ok(result);
        }

        [Route("ListarDetalleInterfacesXLSX_CBCO")]
        [HttpPost]
        public IHttpActionResult ListarDetalleInterfacesXLSX_CBCO(InterfaceMonitoringDetailXLSXFilter_CB data)
        {
            ResponseControl Rpt = new ResponseControl(Response.Ok);
            try
            {
                var response = this.InterfaceMonitoringCBCO_CORE.GetDataReport(data);
                if (response.Tables[0].Rows.Count > 0 || response.Tables[1].Rows.Count > 0)
                {
                    string directory = ELog.obtainConfig("reportOnlinePrelError");

                    if (Directory.Exists(directory) == false)
                        Directory.CreateDirectory(directory);

                    string file = "Reporte_Detalle_PreliminarCB - " + data.NIDPROCESS + ".xlsx";
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
        
        // DETALLE ASIENTOS CONTABLES
        [Route("ListarDetalleInterfacesAsientosContables")]
        [HttpPost]
        public IHttpActionResult ListarDetalleInterfacesAsientosContables(InterfaceMonitoringAsientosContablesDetailFilter data)
        {
            var result = this.InterfaceMonitoringCBCO_CORE.ListarDetalleInterfacesAsientosContables(data);
            return Ok(result);
        }
        [Route("ListarDetalleInterfacesAsientosContablesAsiento")]
        [HttpPost]
        public IHttpActionResult ListarDetalleInterfacesAsientosContablesAsiento(InterfaceMonitoringAsientosContablesDetailAsiento_CB_Filter data)
        {
            var result = this.InterfaceMonitoringCBCO_CORE.ListarDetalleInterfacesAsientosContablesAsiento(data);
            return Ok(result);
        }
        [Route("ListarDetalleInterfacesAsientosContablesError")]
        [HttpPost]
        public IHttpActionResult ListarDetalleInterfacesAsientosContablesError(InterfaceMonitoringAsientosContablesDetailError_CB_Filter data)
        {
            var result = this.InterfaceMonitoringCBCO_CORE.ListarDetalleInterfacesAsientosContablesError(data);
            return Ok(result);
        }
        
        // DETALLE EXACTUS
        [Route("ListarDetalleInterfacesExactus")]
        [HttpPost]
        public IHttpActionResult ListarDetalleInterfacesExactus(InterfaceMonitoringExactusDetailFilter data)
        {
            var result = this.InterfaceMonitoringCBCO_CORE.ListarDetalleInterfacesExactus(data);
            return Ok(result);
        }

        [Route("ListarDetalleInterfacesExactusAsiento")]
        [HttpPost]
        public IHttpActionResult ListarDetalleInterfacesExactusAsiento(InterfaceMonitoringExactusDetailAsiento_CB_Filter data)
        {
            var result = this.InterfaceMonitoringCBCO_CORE.ListarDetalleInterfacesExactusAsiento(data);
            return Ok(result);
        }

        [Route("ListarDetalleInterfacesExactusError")]
        [HttpPost]
        public IHttpActionResult ListarDetalleInterfacesExactusError(InterfaceMonitoringExactusDetailError_CB_Filter data)
        {
            var result = this.InterfaceMonitoringCBCO_CORE.ListarDetalleInterfacesExactusError(data);
            return Ok(result);
        }
        
        // ÓRDENES DE PAGO
        [Route("ListarDetalleInterfacesAsientosContablesOP")]
        [HttpPost]
        public IHttpActionResult ListarDetalleInterfacesAsientosContablesOP(InterfaceMonitoringAsientosContablesDetailFilter data)
        {
            var result = this.InterfaceMonitoringCBCO_CORE.ListarDetalleInterfacesAsientosContablesOP(data);
            return Ok(result);
        }
        [Route("ListarDetalleInterfacesAsientosContablesAsientoOP")]
        [HttpPost]
        public IHttpActionResult ListarDetalleInterfacesAsientosContablesAsientoOP(InterfaceMonitoringAsientosContablesDetailAsiento_CB_Filter data)
        {
            var result = this.InterfaceMonitoringCBCO_CORE.ListarDetalleInterfacesAsientosContablesAsientoOP(data);
            return Ok(result);
        }
        [Route("ListarDetalleInterfacesExactusOP")]
        [HttpPost]
        public IHttpActionResult ListarDetalleInterfacesExactusOP(InterfaceMonitoringExactusDetailFilter data)
        {
            var result = this.InterfaceMonitoringCBCO_CORE.ListarDetalleInterfacesExactusOP(data);
            return Ok(result);
        }
        [Route("ListarDetalleInterfacesExactusAsientoOP")]
        [HttpPost]
        public IHttpActionResult ListarDetalleInterfacesExactusAsientoOP(InterfaceMonitoringExactusDetailAsiento_CB_Filter data)
        {
            var result = this.InterfaceMonitoringCBCO_CORE.ListarDetalleInterfacesExactusAsientoOP(data);
            return Ok(result);
        }
        // RENTAS
        [Route("ListarOrigenRentas")]
        [HttpGet]
        public IHttpActionResult ListarOrigenRentas()
        {
            var result = this.InterfaceMonitoringCBCO_CORE.ListarOrigenRentas();
            return Ok(result);
        }
        [Route("ListarPagoSiniestro")]
        [HttpPost]
        public IHttpActionResult ListarPagoSiniestro(ListarPagoSiniestroBM data)
        {
            var result = this.InterfaceMonitoringCBCO_CORE.ListarPagoSiniestro(data);
            return Ok(result);
        }
        [Route("ListarAprobacionesRentas")]
        [HttpPost]
        public IHttpActionResult ListarAprobacionesRentas(RentasAprobacionesBM data)
        {
            var result = this.InterfaceMonitoringCBCO_CORE.ListarAprobacionesRentas(data);
            return Ok(result);
        }
        [Route("AprobarPagoList")]
        [HttpPost]
        public IHttpActionResult AprobarPagoList(RentasAprobarPagoListBM data)
        {
            var result = this.InterfaceMonitoringCBCO_CORE.AprobarPagoList(data);
            return Ok(result);
        }
    }
}