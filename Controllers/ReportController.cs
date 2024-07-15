using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WSPlataforma.Core;
using WSPlataforma.Entities.ReportModel.BindingModel;
using WSPlataforma.Entities.ReportModel.ViewModel;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using System.Diagnostics;
using System.Text;
using WSPlataforma.Util;
using WSPlataforma.Entities.PrintPolicyModel.ATP;
using WSPlataforma.Entities.VDPModel.BindingModel;

namespace WSPlataforma.Controllers
{
    [RoutePrefix("Api/ReportKuntur")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ReportController : ApiController
    {
        ReportCORE reportCORE = new ReportCORE();

        [Route("saleChannelReport")]
        [HttpPost]
        public async Task<List<ReportSalesChannelVM>> SaleChannelReport(ReportSalesChannelBM data)
        {
            return await reportCORE.SaleChannelReport(data);
        }

        [Route("saleClientReport")]
        [HttpPost]
        public async Task<List<ReportSalesClientVM>> SaleClientReport(ReportSalesClientBM data)
        {
            return await reportCORE.SaleClientReport(data);
        }

        [Route("saleEnterpriseReport")]
        [HttpPost]
        public async Task<List<ReportSalesEnterpriseVM>> SaleEnterpriseReport(ReportSalesEnterpriseBM data)
        {
            return await reportCORE.SaleEnterpriseReport(data);
        }

        [Route("commissionEnterpriseReport")]
        [HttpPost]
        public async Task<List<ReportCommissionEnterpriseVM>> CommissionEnterpriseReport(ReportSalesEnterpriseBM data)
        {
            return await reportCORE.CommissionEnterpriseReport(data);
        }

        [Route("commissionChannelReport")]
        [HttpPost]
        public async Task<List<ReportCommissionChannelVM>> CommissionChannelReport(ReportCommissionChannelBM data)
        {
            return await reportCORE.CommissionChannelReport(data);
        }

        [Route("saleRecordReport")]
        [HttpPost]
        public async Task<List<ReportSaleRecordVM>> SaleRecordReport(ReportSaleRecordBM data)
        {
            return await reportCORE.SaleRecordReport(data);
        }

        //INI JF
        //Servicio para Obtener Ramos
        [Route("branchPremiumReport")]
        [HttpGet]
        public IHttpActionResult BranchPremiumReport(string P_TIPO)
        {
            return Ok(reportCORE.BranchPremiumReport(P_TIPO));
        }
        //INI JF
        //Servicio para Obtener Ramos
        [Route("branchProviComisionReport")]
        [HttpGet]
        public IHttpActionResult branchProviComisionReport(string P_TIPO)
        {
            return Ok(reportCORE.BranchPremiumReport(P_TIPO));
        }

        //Servicio para Obtener Productos
        [Route("productPremiumReport")]
        [HttpGet]
        public IHttpActionResult ProductPremiumReport(string P_TIPO, int P_NBRANCH)
        {
            var response = reportCORE.ProductPremiumReport(P_TIPO, P_NBRANCH);
            return Ok(response);
        }

        //Servicio para Obtener Estados
        [Route("statusPremiumReport")]
        [HttpGet]
        public IHttpActionResult StatusPremiumReport()
        {
            return Ok(reportCORE.StatusPremiumReport());
        }

        //Servicio para ListarReportes
        [Route("listPremiumReport")]
        [HttpPost]
        public IHttpActionResult GetListPremiumReport(ReportListProc data)
        {
            var response = reportCORE.GetListPremiumReport(data);
            return Ok(response);
        }

        //Servicio para Obtener Archivo(Excel)
        [Route("getFilePremiumReport")]
        [HttpPost]
        public IHttpActionResult GetFilePremiumReport(ReportListProc data)
        {
            ResponseControl Rpt = new ResponseControl(Response.Ok);
            try
            {
                var headerName = "-CABECERA";
                var detailName = "-DETALLE";

                if (data.id != null)
                {
                    //Construimos la ruta para obtener el archivo de cabecera
                    string directory = ELog.obtainConfig("reportPath");
                    string directoryReport = data.id + "\\";
                    string file = data.id + headerName + ".xlsx";
                    string route = directory + directoryReport + file;
                    if (File.Exists(route))
                    {
                        //Convertimos el archivo en Base64
                        Rpt.Data = Convert.ToBase64String(File.ReadAllBytes(route));
                    }

                    //Construimos la ruta para obtener el archivo de detalle
                    string directory2 = ELog.obtainConfig("reportPath");
                    string directoryReport2 = data.id + "\\";
                    string file2 = data.id + detailName + ".xlsx";
                    string route2 = directory2 + directoryReport2 + file2;
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

        //Servicio para Procesar Reportes
        [Route("processPremiumReport")]
        [HttpPost]
        public IHttpActionResult InsertProcessPremiumReport(ReportProcess r)
        {
            //IdAutogenerado concatenado con codigo de usuario y fecha actual
            var codigoProceso = string.Empty;
            var branchDes = string.Empty;
            codigoProceso = (r.desUsuario).ToUpper() + DateTime.Now.ToString("yyyyMMddhhmmssff");
            r.idProceso = codigoProceso;
            branchDes = r.desRamo.Replace(" ", string.Empty).ToUpper();

            //nombre de usuario fecha con milesimas       
            var response = reportCORE.InsertProcessPremiumReport(r);
            var pMessage = response.Message;
            //ResponseControl Rpt = new ResponseControl(Response.Ok);
            //Respuesta
            try
            {
                if (response.response == 0)
                {
                    string arguments = string.Format(@"{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}", codigoProceso, r.codTipo, r.codPerfil, r.codRamo, branchDes, r.codProducto,
                        Convert.ToDateTime(r.fecInicio).ToString("yyyyMMdd"), Convert.ToDateTime(r.fecFin).ToString("yyyyMMdd"), r.codUsuario, 200, r.typeReport, r.idProcesoOr);
                    using (var process = new Process
                    {
                        StartInfo =
                    new ProcessStartInfo { FileName = ELog.obtainConfig("exePath"), Arguments = arguments }
                    })
                    {
                        process.Start();
                    }
                    response.Data = codigoProceso.ToString();
                    response.response = Response.Ok;
                }
                else
                {
                    response.Data = string.Empty;
                    response.Message = pMessage;
                    response.response = Response.Fail;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            if (response.response == 0)
            {
                return Ok(response);
            }
            else
            {
                return Ok(pMessage);
            }
        }

        //Servicio para Obtener Tipo de Comprobante
        [Route("billTypeRequestProforma")]
        [HttpGet]
        public IHttpActionResult BillTypeRequestProforma()
        {
            return Ok(reportCORE.BillTypeRequestProforma());
        }

        //Servicio para Obtener Serie
        [Route("serieNumberRequestProforma")]
        [HttpGet]
        public IHttpActionResult SerieNumberRequestProforma()
        {
            return Ok(reportCORE.SerieNumberRequestProforma());
        }

        //Servicio para Obtener Tipo de Documento
        [Route("documentTypeRequestProforma")]
        [HttpGet]
        public IHttpActionResult DocumentTypeRequestProforma()
        {
            return Ok(reportCORE.DocumentTypeRequestProforma());
        }

        //Servicio para Listar Proformas
        [Route("listRequestProforma")]
        [HttpPost]
        public IHttpActionResult GetListRequestProforma(ReportProfBM data)
        {
            var response = reportCORE.GetListRequestProforma(data);
            return Ok(response);
        }

        //Servicio para Listar Asegurados
        [Route("listDetailRequestInsured")]
        [HttpPost]
        public IHttpActionResult GetListRequestInsured(ReportInsuBM data)
        {
            var response = reportCORE.GetListRequestInsured(data);
            return Ok(response);
        }

        //Servicio para Listar Cruce de Interfaces
        [Route("interfacesCrossing")]
        [HttpPost]
        public IHttpActionResult GetInterfacesCrossing(ReportInterfBM data)
        {
            var response = reportCORE.GetInterfacesCrossing(data);
            return Ok(response);
        }

        //FIN JF

        [Route("listPayState")]
        [HttpGet]
        public IHttpActionResult ListPayState()
        {
            return Ok(reportCORE.ObtenerEstadosDePago());
        }

        [Route("listBillState")]
        [HttpGet]
        public IHttpActionResult ListBillState()
        {
            return Ok(reportCORE.ObtenerEstadosFactura());
        }

        [Route("listBillsReceipts")]
        [HttpPost]
        public IHttpActionResult ObtenerReporteDeFecturas(BillReportFiltersBM data)
        {
            return Ok(reportCORE.ObtenerReporteDeFecturas(data));
        }

        [Route("billReportTemplate")]
        [HttpPost]
        public IHttpActionResult ObtenerPlantillaReporteFactura(BillReportFiltersBM data)
        {
            return Ok(reportCORE.ObtenerPlantillaReporteFactura(data));
        }

        [Route("listQuotationReport")]
        [HttpPost]
        public IHttpActionResult ObtenerReporteDeCotizaciones(QuotationReportFiltersBM data)
        {
            return Ok(reportCORE.ObtenerReporteDeCotizaciones(data));
        }

        [Route("quotationReportTemplate")]
        [HttpPost]
        public IHttpActionResult ObtenerPlantillaReporteCotizacion(QuotationReportFiltersBM data)
        {
            return Ok(reportCORE.ObtenerPlantillaReporteCotizacion(data));
        }

        [Route("GetChannelTypeAllList")]
        [HttpGet]
        public List<ChannelTypeVM> GetChannelTypeAllList()
        {
            return reportCORE.GetChannelTypeAllList();
        }

        //Servicio para Obtener Tipo de Documento
        [Route("documentTypeQuotationReport")]
        [HttpGet]
        public IHttpActionResult DocumentTypeQuotationReport()
        {
            return Ok(reportCORE.DocumentTypeQuotationReport());
        }

        //Servicio para Obtener Tipo de Documento
        [Route("GetRequestAllList")]
        [HttpGet]
        public IHttpActionResult GetRequestAllList()
        {
            return Ok(reportCORE.GetRequestAllList());
        }

        /// <summary>
        /// Metodo para la consulta de reporte de tramites
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Route("listProcedureReport")]
        [HttpPost]
        public IHttpActionResult ObtenerReporteDeTramites(ProcedureReportFiltersBM data)
        {
            return Ok(reportCORE.ObtenerReporteDeTramites(data));
        }

        /// <summary>
        /// Metodo para la consulta de reporte de tramte en Excel
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Route("procedureReportTemplate")]
        [HttpPost]
        public IHttpActionResult ObtenerPlantillaReporteTramite(ProcedureReportFiltersBM data)
        {
            return Ok(reportCORE.ObtenerPlantillaReporteTramite(data));
        }

        [Route("getUsersList")]
        [HttpGet]
        public IHttpActionResult ObtenerUsersList(string productId, string branch)
        {
            return Ok(reportCORE.ObtenerUsersList(productId, branch));
        }

        [Route("GetProcedureStatusList")]
        [HttpGet]
        public IHttpActionResult GetProcedureStatusList()
        {
            return Ok(reportCORE.GetProcedureStatusList());
        }

        [Route("ReportOperationVDP")]
        [HttpPost]
        public ResponseReportATPVM ReportOperationVDP(RequestReportOperationVDP data)
        {
            return reportCORE.ReportOperationVDP(data);
        }

        [Route("ReportTecVDP")]
        [HttpPost]
        public ResponseReportATPVM ReportTecVDP(RequestReportTecVDP data)
        {
            return reportCORE.ReportTecVDP(data);
        }

        [Route("ReportControlDaily")]
        [HttpPost]
        public ResponseReportATPVM ReportControlDaily(RequestReportControlDaily data)
        {
            return reportCORE.ReportControlDaily(data);
        }

        [Route("ReportControlMonth")]
        [HttpPost]
        public ResponseReportATPVM ReportControlMonth(RequestReportControlMonth data)
        {
            return reportCORE.ReportControlMonth(data);
        }
        [Route("ReportControlYear")]
        [HttpPost]
        public ResponseReportATPVM ReportControlYear(RequestReportControlYear data)
        {
            return reportCORE.ReportControlYear(data);
        }
        [Route("ReportRegistryPolicies")]
        [HttpPost]
        public ResponseReportATPVM ReportRegistryPolicies(RequestReportRegistryPolicies data)
        {
            return reportCORE.ReportRegistryPolicies(data);
        }
        [Route("ReportRegistryPoliciesDetail")]
        [HttpPost]
        public ResponseReportATPVM ReportRegistryPoliciesDetail(RequestReportRegistryPoliciesDetail data)
        {
            return reportCORE.ReportRegistryPoliciesDetail(data);
        }
        [Route("ReportRegistryReserve")]
        [HttpPost]
        public ResponseReportATPVM ReportRegistryReserve(RequestReportRegistryReserve data)
        {
            return reportCORE.ReportRegistryReserve(data);
        }
        /*
        [Route("ReportClaimATP")]
        [HttpPost]
        public ResponseReportATPVM ReportClaimATP(RequestReportClaimATP data)
        {
            return reportCORE.ReportClaimATP(data);
        }
        */
        [Route("ReportPersistVDP")]
        [HttpPost]
        public ResponseReportATPVM ReportPersistVDP(RequestReportPersistVDP data)
        {
            return reportCORE.ReportPersistVDP(data);
        }

        /*reporte tecnico de desgravamen con devolución*/
        [Route("ReportTecnic")]
        [HttpPost]
        public ResponseReportATPVM ReportTecnic(RequestReportTecnic data)
        {
            return reportCORE.ReportTecnic(data);
        }
        /*reporte operaciones  de desgravamen con devolución*/
        [Route("ReportOperac")]
        [HttpPost]
        public ResponseReportATPVM ReportOperac(RequestReportOperac data)
        {
            return reportCORE.ReportOperac(data);
        }


        /*reporte facturacion de desgravamen con devolución*/
        [Route("ReportFacturacion")]
        [HttpPost]
        public ResponseReportATPVM ReportFacturacion(RequestReportFacturacion data)
        {
            return reportCORE.ReportFacturacion(data);
        }


        /*reporte convenios de desgravamen con devolución*/
        [Route("ReportConvenios")]
        [HttpPost]
        public ResponseReportATPVM ReportConvenios(RequestReportConvenios data)
        {
            return reportCORE.ReportConvenios(data);
        }


        /*reporte comisiones de desgravamen con devolución*/
        [Route("ReportComisiones")]
        [HttpPost]
        public ResponseReportATPVM ReportComisiones(RequestReportComisiones data)
        {
            return reportCORE.ReportComisiones(data);
        }


        /*reporte comisiones de desgravamen con devolución*/
        [Route("ReportProviComisiones")]
        [HttpPost]
        public ResponseReportATPVM ReportProviComisiones(RequestReportProviComisiones data)
        {
            return reportCORE.ReportProviComisiones(data);
        }


        /*reporte cuentasxcobrar de desgravamen con devolución*/
        [Route("ReportCuentasxcobrar")]
        [HttpPost]
        public ResponseReportATPVM ReportCuentasxcobrar(RequestReportCuentasxcobrar data)
        {
            return reportCORE.ReportCuentasxcobrar(data);
        }



    }
}
