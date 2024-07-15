using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.DA;
using WSPlataforma.Entities.PrintPolicyModel.ATP;
using WSPlataforma.Entities.ReportModel.BindingModel;
using WSPlataforma.Entities.ReportModel.ViewModel;
using WSPlataforma.Entities.VDPModel.BindingModel;

namespace WSPlataforma.Core
{
    public class ReportCORE
    {
        ReportDA reportDA = new ReportDA();
        BillReportDA billReportDA = new BillReportDA();

        public async Task<List<ReportSalesChannelVM>> SaleChannelReport(ReportSalesChannelBM data)
        {
            return await reportDA.SaleChannelReport(data);
        }

        public async Task<List<ReportSalesClientVM>> SaleClientReport(ReportSalesClientBM data)
        {
            return await reportDA.SaleClientReport(data);
        }

        public async Task<List<ReportSalesEnterpriseVM>> SaleEnterpriseReport(ReportSalesEnterpriseBM data)
        {
            return await reportDA.SaleEnterpriseReport(data);
        }

        public async Task<List<ReportCommissionEnterpriseVM>> CommissionEnterpriseReport(ReportSalesEnterpriseBM data)
        {
            return await reportDA.CommissionEnterpriseReport(data);
        }

        public async Task<List<ReportCommissionChannelVM>> CommissionChannelReport(ReportCommissionChannelBM data)
        {
            return await reportDA.CommissionChannelReport(data);
        }

        public async Task<List<ReportSaleRecordVM>> SaleRecordReport(ReportSaleRecordBM data)
        {
            return await reportDA.SaleRecordReport(data);
        }

        //Ramo
        public List<ReportBranProc> BranchPremiumReport(string ptipo)
        {
            return reportDA.BranchPremiumReport(ptipo);
        }

        //Producto
        public List<ReportProdProc> ProductPremiumReport(string ptipo, int nbranch)
        {
            return reportDA.ProductPremiumReport(ptipo, nbranch);
        }

        //Estado
        public List<ReportStatusProc> StatusPremiumReport()
        {
            return reportDA.StatusPremiumReport();
        }

        //Lista Reporte 
        public List<ReportListProc> GetListPremiumReport(ReportListProc data)
        {
            return reportDA.GetListPremiumReport(data);
        }

        //Procesar
        public ResponseControl InsertProcessPremiumReport(ReportProcess responseControl)
        {
            return reportDA.InsertProcessPremiumReport(responseControl);
        }

        //Tipo de Comprobante
        public List<ReportBillTypeProf> BillTypeRequestProforma()
        {
            return reportDA.BillTypeRequestProforma();
        }

        //Número de Serie
        public List<ReportSerieProf> SerieNumberRequestProforma()
        {
            return reportDA.SerieNumberRequestProforma();
        }

        //Tipo de Documento
        public List<ReportTypeDocProf> DocumentTypeRequestProforma()
        {
            return reportDA.DocumentTypeRequestProforma();
        }

        //Lista Proformas 
        public ResponseReportProf GetListRequestProforma(ReportProfBM data)
        {
            return reportDA.GetListRequestProforma(data);
        }

        //Lista Asegurados 
        public ResponseReportInsu GetListRequestInsured(ReportInsuBM data)
        {
            return reportDA.GetListRequestInsured(data);
        }

        //Lista Cruce de Interfaces
        public ResponseReportInter GetInterfacesCrossing(ReportInterfBM data)
        {
            return reportDA.GetInterfacesCrossing(data);
        }

        public List<PayStateVM> ObtenerEstadosDePago()
        {
            return billReportDA.ObtenerEstadosDePago();
        }

        public List<BillStateVM> ObtenerEstadosFactura()
        {
            return billReportDA.ObtenerEstadosFactura();
        }

        public Dictionary<string, object> ObtenerReporteDeFecturas(BillReportFiltersBM data)
        {
            return billReportDA.ObtenerReporteDeFecturas(data);
        }

        public string ObtenerPlantillaReporteFactura(BillReportFiltersBM data)
        {
            return billReportDA.ObtenerPlantilla(data);
        }
        public Dictionary<string, object> ObtenerReporteDeCotizaciones(QuotationReportFiltersBM data)
        {
            return reportDA.ObtenerReporteDeCotizaciones(data);
        }

        public string ObtenerPlantillaReporteCotizacion(QuotationReportFiltersBM data)
        {
            return reportDA.ObtenerPlantillaCotizacion(data);
        }

        public List<ChannelTypeVM> GetChannelTypeAllList()
        {
            return reportDA.GetChannelTypeAllList();
        }

        public List<DocumentTypeQRVM> DocumentTypeQuotationReport()
        {
            return reportDA.DocumentTypeQuotationReport();
        }
        public List<RequestAllTypeVM> GetRequestAllList()
        {
            return reportDA.GetRequestAllList();
        }

        public Dictionary<string, object> ObtenerReporteDeTramites(ProcedureReportFiltersBM data)
        {
            return reportDA.ObtenerReporteDeTramites(data);
        }

        public string ObtenerPlantillaReporteTramite(ProcedureReportFiltersBM data)
        {
            return reportDA.ObtenerPlantillaReporteTramite(data);
        }

        public List<UsersReportPRVM> ObtenerUsersList(string productId, string branch)
        {
            return reportDA.ObtenerUsersList(productId, branch);
        }

        public List<ProcedureStatus> GetProcedureStatusList()
        {
            return reportDA.GetProcedureStatusList();
        }
        public ResponseReportATPVM ReportOperationVDP(RequestReportOperationVDP data)
        {
            return reportDA.ReportOperationVDP(data);
        }
        public ResponseReportATPVM ReportTecVDP(RequestReportTecVDP data)
        {
            return reportDA.ReportTecVDP(data);
        }
        public ResponseReportATPVM ReportControlDaily(RequestReportControlDaily data)
        {
            return reportDA.ReportControlDaily(data);
        }
        public ResponseReportATPVM ReportControlMonth(RequestReportControlMonth data)
        {
            return reportDA.ReportControlMonth(data);
        }
        public ResponseReportATPVM ReportControlYear(RequestReportControlYear data)
        {
            return reportDA.ReportControlYear(data);
        }
        public ResponseReportATPVM ReportRegistryPolicies(RequestReportRegistryPolicies data)
        {
            return reportDA.ReportRegistryPolicies(data);
        }
        public ResponseReportATPVM ReportRegistryPoliciesDetail(RequestReportRegistryPoliciesDetail data)
        {
            return reportDA.ReportRegistryPoliciesDetail(data);
        }
        public ResponseReportATPVM ReportRegistryReserve(RequestReportRegistryReserve data)
        {
            return reportDA.ReportRegistryReserve(data);
        }
        public ResponseReportATPVM ReportClaimATP(RequestReportClaimATP data)
        {
            return reportDA.ReportClaimATP(data);
        }
        public ResponseReportATPVM ReportPersistVDP(RequestReportPersistVDP data)
        {
            return reportDA.ReportPersistVDP(data);
        }
        //Reporte Tecnico Desgravamen con Devolucion
        public ResponseReportATPVM ReportTecnic(RequestReportTecnic data)
        {
            return reportDA.ReportTecnic(data);
        }

        //Reporte Operacion Desgravamen con Devolucion
        public ResponseReportATPVM ReportOperac(RequestReportOperac data)
        {
            return reportDA.ReportOperac(data);
        }

        //Reporte Facturacion Desgravamen con Devolucion
        public ResponseReportATPVM ReportFacturacion(RequestReportFacturacion data)
        {
            return reportDA.ReportFacturacion(data);
        }

        //Reporte Convenios Desgravamen con Devolucion
        public ResponseReportATPVM ReportConvenios(RequestReportConvenios data)
        {
            return reportDA.ReportConvenios(data);
        }

        //Reporte Comisiones Desgravamen con Devolucion
        public ResponseReportATPVM ReportComisiones(RequestReportComisiones data)
        {
            return reportDA.ReportComisiones(data);
        }

        //Reporte Comisiones Desgravamen con Devolucion
        public ResponseReportATPVM ReportProviComisiones(RequestReportProviComisiones data)
        {
            return reportDA.ReportProviComisiones(data);
        }

        //Reporte Cuentasxcobrar Desgravamen con Devolucion
        public ResponseReportATPVM ReportCuentasxcobrar(RequestReportCuentasxcobrar data)
        {
            return reportDA.ReportCuentasxcobrar(data);
        }


    }
}
