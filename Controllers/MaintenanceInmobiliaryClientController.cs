
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http.Cors;
using WSPlataforma.Core;
using WSPlataforma.Entities.PolicyModel.BindingModel;
using System.Web.Http;
using WSPlataforma.Entities.MaintenanceInmoClientModel.ViewModel;
using WSPlataforma.Entities.MaintenanceInmoClientModel.BindingModel;
using WSPlataforma.Entities.ReportModel.BindingModel;
using System.IO;
using Newtonsoft.Json;
using WSPlataforma.Util;
using ExcelDataReader;
using System.Data;

namespace WSPlataforma.Controllers
{
    [RoutePrefix("Api/MaintenanceInmobiliary")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]

    #pragma warning disable CS1591

    public class MaintenanceInmobiliaryClientController : ApiController
    {
        MaintenanceInmobiliaryClientCORE mantClientCORE = new MaintenanceInmobiliaryClientCORE();

        [Route("GetTypeDocumentsList")]
        [HttpGet]
        public List<DocumentBM> GetTipeDocumentsList()
        {
            return mantClientCORE.GetTypeDocumentList();
        }

        [Route("GetOptionList")]
        [HttpGet]
        public List<OptionBM> GetTypeOptionList()
        {
            return mantClientCORE.GetTypeOptionList();
        }

        [Route("GetListBuscarPor")]
        [HttpGet]
        public List<OptionBM> GetListBuscarPor()
        {
            return mantClientCORE.GetListBuscarPor();
        }

        [Route("GetListTipFacturacion")]
        [HttpPost]
        public List<OptionBM> GetListTipFacturacion(ContratantesVM data)
        {
            return mantClientCORE.GetListTipFacturacion(data);
        }

        [Route("GetContratantesList")]
        [HttpPost]
        public List<ContratantesBM> GetContratantesList(ContratantesVM data)
        {
            return mantClientCORE.GetContratantesList(data);
        }

        [Route("GetConsultaClientesList")]
        [HttpPost]
        public List<ConsultaClienteBM> GetConsultaClientesList(ConsultaClienteVM data)
        {
            var response = mantClientCORE.GetConsultaClientesList(data);
            return response;
        }

        // CIERRE
        [Route("ListarDocumentosCierre")]
        [HttpGet]
        public IHttpActionResult ListarDocumentosCierre()
        {
            var result = mantClientCORE.ListarDocumentosCierre();
            return Ok(result);
        }
        [Route("ListarPorVencerCierre")]
        [HttpGet]
        public IHttpActionResult ListarPorVencerCierre()
        {
            var result = mantClientCORE.ListarPorVencerCierre();
            return Ok(result);
        }
        [Route("ListarEstadosBillsCierre")]
        [HttpGet]
        public IHttpActionResult ListarEstadosBillsCierre()
        {
            var result = mantClientCORE.ListarEstadosBillsCierre();
            return Ok(result);
        }
        [Route("GetInmobiliairyCierreReport")]
        [HttpPost]
        public IHttpActionResult GetInmobiliairyCierreReport(InmobiliairyCierreBM data)
        {
            var result = mantClientCORE.GetInmobiliairyCierreReport(data);
            return Ok(result);
        }
        [Route("GetDataReportInmobiliariasControlSeguimiento")]
        [HttpPost]
        public IHttpActionResult GetDataReportInmobiliariasControlSeguimiento(InmobiliairyCierreBM data)
        {
            ResponseControl Rpt = new ResponseControl(Response.Ok);
            try
            {
                var response = this.mantClientCORE.GetDataReportInmobiliariasControlSeguimiento(data);
                if (response.Tables[0].Rows.Count > 0 || response.Tables[1].Rows.Count > 0)
                {
                    string directory = ELog.obtainConfig("reportInmobControlSeg");

                    if (Directory.Exists(directory) == false)
                        Directory.CreateDirectory(directory);

                    string file = "Reporte_Seguimiento_Control_Inmobiliarias.xlsx";
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
                    Rpt.Data = "No se obtuvo información en las fechas solicitadas para el reporte de control y seguimiento.";
                }
            }
            catch (Exception ex)
            {
                Rpt.response = Response.Fail;
                Rpt.Data = ex.Message;

            }
            return Ok(Rpt);
        }
        [Route("ListarTipoServicioCierre")]
        [HttpGet]
        public IHttpActionResult ListarTipoServicioCierre()
        {
            var result = mantClientCORE.ListarTipoServicioCierre();
            return Ok(result);
        }
        [Route("GetInmobiliairyReporteCierre")]
        [HttpPost]
        public IHttpActionResult GetInmobiliairyReporteCierre(InmobiliairyReportCierreBM data)
        {
            var result = mantClientCORE.GetInmobiliairyReporteCierre(data);
            return Ok(result);
        }
        [Route("GetDataInmobiliairyReporteCierre")]
        [HttpPost]
        public IHttpActionResult GetDataInmobiliairyReporteCierre(InmobiliairyReportCierreBM data)
        {
            ResponseControl Rpt = new ResponseControl(Response.Ok);
            try
            {
                var response = this.mantClientCORE.GetDataInmobiliairyReporteCierre(data);
                if (response.Tables[0].Rows.Count > 0 || response.Tables[1].Rows.Count > 0)
                {
                    string directory = ELog.obtainConfig("reportInmobCierre");

                    if (Directory.Exists(directory) == false)
                        Directory.CreateDirectory(directory);

                    string file = "Reporte_Cierre_Inmobiliarias.xlsx";
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
                    Rpt.Data = "No se obtuvo información en las fechas solicitadas para el reporte de cierre.";
                }
            }
            catch (Exception ex)
            {
                Rpt.response = Response.Fail;
                Rpt.Data = ex.Message;

            }
            return Ok(Rpt);
        }

        // DGC

        // GÉNERO
        [Route("ListarGeneroInmobiliaria")]
        [HttpGet]
        public IHttpActionResult ListarGeneroInmobiliaria()
        {
            var result = mantClientCORE.ListarGeneroInmobiliaria();
            return Ok(result);
        }

        // NACIONALIDAD
        [Route("ListarNacionalidadInmobiliaria")]
        [HttpGet]
        public IHttpActionResult ListarNacionalidadInmobiliaria()
        {
            var result = mantClientCORE.ListarNacionalidadInmobiliaria();
            return Ok(result);
        }

        // ESTADO CIVIL
        [Route("ListarEstadoCivilInmobiliaria")]
        [HttpGet]
        public IHttpActionResult ListarEstadoCivilInmobiliaria()
        {
            var result = mantClientCORE.ListarEstadoCivilInmobiliaria();
            return Ok(result);
        }

        // TIPO DE DOCUMENTO
        [Route("ListarTipoDocumentoInmobiliaria")]
        [HttpGet]
        public IHttpActionResult ListarTipoDocumentoInmobiliaria()
        {
            var result = mantClientCORE.ListarTipoDocumentoInmobiliaria();
            return Ok(result);
        }

        // DETALLE CLIENTE
        [Route("GetClientDetInmobiliaria")]
        [HttpPost]
        public IHttpActionResult GetClientDetInmobiliaria(GetClientDetBM data)
        {
            var result = mantClientCORE.GetClientDetInmobiliaria(data);
            return Ok(result);
        }

        // TIPO VIA
        [Route("GetTipoVias")]
        [HttpGet]
        public IHttpActionResult GetTipoVias()
        {
            var result = mantClientCORE.GetTipoVias();
            return Ok(result);
        }

        // DEPARTAMENTOS
        [Route("GetDepartamentos")]
        [HttpGet]
        public IHttpActionResult GetDepartamentos()
        {
            var result = mantClientCORE.GetDepartamentos();
            return Ok(result);
        }

        // PROVINCIA
        [Route("GetProvincias")]
        [HttpGet]
        public IHttpActionResult GetProvincias(int P_NPROVINCE)
        {
            var result = mantClientCORE.GetProvincias(P_NPROVINCE);
            return Ok(result);
        }

        // DISTRITO
        [Route("GetDistrito")]
        [HttpGet]
        public IHttpActionResult GetDistrito(int P_NLOCAL)
        {
            var result = mantClientCORE.GetDistrito(P_NLOCAL);
            return Ok(result);
        }

        // DETALLE DIRECCION
        [Route("GetDetalleDireccion")]
        [HttpGet]
        public IHttpActionResult GetDetalleDireccion(string P_SCLIENT)
        {
            var result = mantClientCORE.GetDetalleDireccion(P_SCLIENT);
            return Ok(result);
        }

        //VALIDACION FACT
        [Route("GetExistsBills")]
        [HttpGet]
        public IHttpActionResult GetExistsBills(int P_NBRANCH, int P_NPRODUCT, int P_NPOLICY)
        {
            var result = mantClientCORE.GetExistsBills(P_NBRANCH, P_NPRODUCT, P_NPOLICY);
            return Ok(result);
        }

        // INSERTAR CLIENTE
        [Route("InsertarClienteInmobiliaria")]
        [HttpPost]
        public IHttpActionResult InsertarClienteInmobiliaria(InsClientBM data)
        {
            var result = mantClientCORE.InsertarClienteInmobiliaria(data);
            return Ok(result);
        }

        // ACTUALIZAR CLIENTE
        [Route("ActualizarClienteInmobiliaria")]
        [HttpPost]
        public IHttpActionResult ActualizarClienteInmobiliaria(UpdClientBM data)
        {
            var result = mantClientCORE.ActualizarClienteInmobiliaria(data);
            return Ok(result);
        }

        // OBTENER PAGOS
        [Route("PayDateInmob")]
        [HttpPost]
        public IHttpActionResult PayDateInmob(PayDateBM data)
        {
            var result = mantClientCORE.PayDateInmob(data);
            return Ok(result);
        }
    }
}