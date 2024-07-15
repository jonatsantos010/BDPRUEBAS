using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WSPlataforma.DA;
using WSPlataforma.Entities.CobranzasModel;
using WSPlataforma.Entities.PolicyModel.BindingModel;
using WSPlataforma.Entities.PolicyModel.ViewModel;
using WSPlataforma.Entities.QuotationModel.BindingModel;
using WSPlataforma.Entities.QuotationModel.ViewModel;
using WSPlataforma.Entities.AjustedAmountsModel.BindingModel;
using WSPlataforma.Entities.AjustedAmountsModel.ViewModel;
using WSPlataforma.Entities.Graphql;
using WSPlataforma.Entities.MovementsModel.ViewModel;
using WSPlataforma.Entities.ValBrokerModel;
using WSPlataforma.Entities.ComprobantesEPSModel.BindingModel;
using WSPlataforma.Entities.NidProcessRenewModel.ViewModel;

namespace WSPlataforma.Core
{
    public class PolicyCORE
    {
        PolicyDA policyDA = new PolicyDA();
        CombiSCTRDA sctrDA = new CombiSCTRDA();

        public ResponseVM ValidatePolicyRenov(PolicyRenewBM policyRenew)
        {
            return policyDA.ValidatePolicyRenov(policyRenew);
        }

        public ResponseVM cancelMovement(PolicyMovementCancelBM data)
        {
            return policyDA.cancelMovement(data);
        }

        public PolicyModuleVM GetValidateExistPolicy(PolicyModuleBM policyModuleBM)
        {
            return policyDA.GetValidateExistPolicy(policyModuleBM);
        }

        public Entities.PolicyModel.ViewModel.GenericPackageVM GetMovementTypeList()
        {
            return policyDA.GetMovementTypeList();
        }

        public GenericResponseVM GetPolicyTrackingList(TrackingBM data)
        {
            return policyDA.GetPolicyTrackingList(data);
        }

        public Entities.PolicyModel.ViewModel.GenericPackageVM GetProductTypeList()
        {
            return policyDA.GetProductTypeList();
        }

        public Entities.PolicyModel.ViewModel.GenericPackageVM GetInsuredPolicyList(InsuredPolicySearchBM data)
        {
            return policyDA.GetInsuredPolicyList(data);
        }

        public List<TransaccionTypeVM> GetTransaccionList()
        {
            return policyDA.GetTransaccionList();
        }

        public List<TransactionAllTypeVM> GetTransactionAllList()
        {
            return policyDA.GetTransactionAllList();
        }

        public GenericTransVM GetPolicyTransList(TransPolicySearchBM data)
        {
            return policyDA.GetPolicyTransList(data);
        }

        public GenericTransVM GetPolicyMovementsTransList(TransPolicySearchBM data)
        {
            return policyDA.GetPolicyMovementsTransList(data);
        }

        public Entities.PolicyModel.ViewModel.GenericPackageVM GetPolicyProofList(PolicyProofSearchBM data)
        {
            return policyDA.GetPolicyProofList(data);
        }

        public Entities.PolicyModel.ViewModel.GenericPackageVM GetPolicyTransactionList(PolicyTransactionSearchBM data)
        {
            return policyDA.GetPolicyTransactionList(data);
        }

        public List<PolicyTransactionAllVM> GetPolicyTransactionAllList(PolicyTransactionAllSearchBM data)
        {
            return policyDA.GetPolicyTransactionAllList(data);
        }

        public string GetPolicyTransactionAllListExcel(PolicyTransactionAllSearchBM data)
        {
            return policyDA.GetPolicyTransactionAllListExcel(data);
        }

        public List<PolicyMovAllVM> GetPolicyMovementsTransAllList(PolicyTransactionAllSearchBM data)
        {
            return policyDA.GetPolicyMovementsTransAllList(data);
        }

        // ED - ASEGURADO
        public InsuredPolicyResponse GetInsuredForTransactionPolicy(PolicyInsuredFilter _data)
        {
            return policyDA.GetInsuredForTransactionPolicy(_data);
        }

        public ResponseVM UpdateInsuredPolicy(UpdateInsuredPolicy request)
        {
            return policyDA.UpdateInsuredPolicy(request);
        }

        public List<MovementsModelVM> GetPolicyMovementsVCF(MovementsModelVM data)
        {
            return policyDA.GetPolicyMovementsVCF(data);
        }

        public SalidadPolizaEmit SavePolicyMovementsVCF(MovementsModelVM data, List<HttpPostedFile> files)
        {
            return policyDA.SavePolicyMovementsVCF(data, files);
        }

        public SalidadPolizaEmit SendMailMovementsVCF(MovementsModelVM data)
        {
            return policyDA.SendMailMovementsVCF(data);
        }

        public Entities.PolicyModel.ViewModel.GenericPackageVM GetAccountTransactionList(AccountTransactionSearchBM data)
        {
            return policyDA.GetAccountTransactionList(data);
        }

        public Entities.PolicyModel.ViewModel.GenericPackageVM GetPaymentStateList()
        {
            return policyDA.GetPaymentStateList();
        }

        public async Task<Entities.PolicyModel.ViewModel.GenericPackageVM> GetPolizaEmitCab(string nroCotizacion, string typeMovement, int userCode, int FlagNoCIP = 0)
        {
            return await policyDA.GetPolizaEmitCab(nroCotizacion, typeMovement, userCode, FlagNoCIP);
        }

        public List<PolizaEmitComer> GetPolizaEmitComer(int nroCotizacion)
        {
            return policyDA.GetPolizaEmitComer(nroCotizacion);
        }

        public List<PolizaEmitDet> GetPolizaEmitDet(int nroCotizacion, int? userCode)
        {
            return policyDA.GetPolizaEmitDet(nroCotizacion, userCode);
        }

        public List<PolizaEmitDet> GetPolizaEmitDetTX(string processId, string typeMovement, int? userCode, int? vencido)
        {
            return policyDA.GetPolizaEmitDetTX(processId, typeMovement, userCode, vencido);
        }

        public List<TipoRenovacion> GetTipoRenovacion(TypeRenBM request)
        {
            return policyDA.GetTipoRenovacion(request);
        }

        public List<FrecuenciaPago> GetFrecuenciaPago(int codrenovacion, int producto)
        {
            return policyDA.GetFrecuenciaPago(codrenovacion, producto);
        }

        public List<FrecuenciaPago> GetFrecuenciaPagoTotal(int codrenovacion, int producto)
        {
            return policyDA.GetFrecuenciaPagoTotal(codrenovacion, producto);
        }

        public List<AnnulmentVM> GetAnnulment()
        {
            return policyDA.GetAnnulment();
        }

        public List<TypeEndoso> GetTypeEndoso()
        {
            return policyDA.GetTypeEndoso();
        }

        public SalidadPolizaEmit SavePolizaEmit(SavePolicyEmit policyEmit, List<HttpPostedFile> files)
        {
            return policyDA.SavePolizaEmitFile(policyEmit, files);
        }

        public SalidaPlanilla InsuredVal(Entities.PolicyModel.BindingModel.InsuredValBM insuredVal)
        {
            return policyDA.InsuredVal(insuredVal);
        }

        public List<PolicyDetVM> GetPolizaCot(int nroCotizacion, int nroTransac = 0)
        {
            return policyDA.GetPolizaCot(nroCotizacion, nroTransac);
        }

        public ResponseAccountStatusVM ExecuteExcelDemo(GetTrama data)
        {
            return new ExcelGenerate().ExecuteExcelDemo(data);
        }

        public SalidadPolizaEmit transactionSave(PolicyTransactionSaveBM request, List<HttpPostedFile> files)
        {
            return policyDA.transactionSave(request, files);
        }

        public QuotationResponseVM RenewMod(QuotationCabBM request)
        {
            return policyDA.RenewMod(request);
        }

        public QuotationResponseVM AccPerEndoso(QuotationCabBM request)
        {
            return policyDA.AccPerEndoso(request);
        }

        public SalidadPolizaEmit valTransactionPolicy(int nroCotizacion, int idTipo)
        {
            return policyDA.valTransactionPolicy(nroCotizacion, idTipo);
        }

        public DisplayProcessVM VisualizadorProc(DisplayProcessBM request)
        {
            return policyDA.VisualizadorProc(request);
        }

        public ErrorCode ReverHISDETSCTR(int P_NID_COTIZACION, int P_NMOVEMENT_PH, string P_NID_PROC)
        {
            return policyDA.ReverHISDETSCTR(P_NID_COTIZACION, P_NMOVEMENT_PH, P_NID_PROC);
        }

        public ResponseVM SavedPolicyTransac(SavedPolicyTXBM data)
        {
            return policyDA.SavedPolicyTransac(data);
        }

        public ResponseVM valBilling(ValBillingBM data)
        {
            return policyDA.valBilling(data);
        }

        public List<ConfigVM> getDataConfig(string stype)
        {
            return policyDA.getDataConfig(stype);
        }

        public async Task<SalidadPolizaEmit> insertPrePayment(PrePaymentBM request)
        {
            return await policyDA.insertPrePayment(request);
        }

        public string ValidateRetroactivity(int cotizacion, int usercode, string efectdate, string transac)
        {
            return policyDA.ValidateRetroactivity(cotizacion, usercode, efectdate, transac);
        }

        public QuotationResponseVM ProcesarEndosoVL(QuotationCabBM request)
        {
            return policyDA.ProcesarEndosoVL(request);
        }

        public SalidadPolizaEmit insertarHistorial(PolicyTransactionSaveBM request)
        {
            return policyDA.insertarHistorial(request);
        }

        public List<TypePolicy> getTypePolicy(string pbranch)
        {
            return policyDA.getTypePolicy(pbranch);
        }

        public List<ListaTipoRenovacion> GetListaTipoRenovacion()
        {
            return policyDA.GetListaTipoRenovacion();
        }

        public async Task<string> invocarServicioConsultaDPS(string id, string token)
        {
            return await policyDA.invocarServicioConsultaDPS(id, token);
        }

        public SalidaAjustedAmounts AjustedAmounts(AjustedAmountsBM data)
        {
            return policyDA.AjustedAmounts(data);
        }

        public SalidadPolizaEmit UpdateNLocalBroker(WSPlataforma.Entities.TransactModel.DepBroker BrkList, int nusercode)
        {
            return policyDA.UpdateNLocalBroker(BrkList, nusercode);
        }

        public Task<ResponseGraph> getReferenceURL(string nbranch, string codUrl, string origen)
        {
            return new GraphqlDA().getReferenceURL(nbranch, codUrl, origen);
        }

        public List<PolicyReceipt> GetNumRecibo(SavePolicyEmit response, string typerecibo)
        {
            return policyDA.GetNumRecibo(response, typerecibo);
        }

        public ResponseHeader getCotHeader(RequestCuadroPolizaBM request)
        {
            return policyDA.getCotHeader(request);
        }

        public SalidadPolizaEmit regBIO(int nidcotizacion, string nrodoc, string nrocli, string algoritmo, string porcsimil, string urlImagen, string fechaeval, string tipoval, string codigoverif)
        {
            return policyDA.regBIO(nidcotizacion, nrodoc, nrocli, algoritmo, porcsimil, urlImagen, fechaeval, tipoval, codigoverif);
        }

        public SalidadPolizaEmit UpdateDatosPer(string sclient, string sproteg)
        {
            return policyDA.UpdateDatosPer(sclient, sproteg);
        }

        public QuotationResponseVM ProcesarTramaEstado(QuotationCabBM request)
        {
            return policyDA.ProcesarTramaEstado(request);
        }

        public ErrorCode Msj_TecnicsSCTR(HisDetRenovSCTR data)
        {
            return sctrDA.Msj_TecnicsSCTR(data);
        }

        public ErrorCode DeleteDataSCTR(delSCTR data)
        {
            return policyDA.DeleteDataSCTR(data);
        }
        public MonthsSCTRReturn GetMonthsSCTR(monthsSCTR data)
        {
            return policyDA.GetMonthsSCTR(data);
        }

        public ResponseAccountStatusVM DownloadExcelPlantillaVidaLey(GetTrama data)
        {
            return new ExcelGenerate().GetPlantillaVidaLey(data);
        }

        // AGF 12052023 Beneficiarios VCF
        public ResponseAccountStatusVM DownloadExcelPlantillaVCF(GetTrama data)
        {
            return new ExcelGenerate().GetPlantillaVCF(data);
        }

        public CommissionVLVM getCommissionVL(CommissionVLBM data)
        {
            return policyDA.getCommissionVL(data);
        }

        public ValBrokerModel valBrokerVCF(ValBrokerModel data)
        {
            return policyDA.valBrokerVCF(data);
        }
        public PolizaEmitCAB GetExpirAseg(string nroCotizacion)
        {

            return policyDA.getExpirAseg(nroCotizacion);
        }

        public ValPolicy GetValFact(ValPolicy data)
        {
            return policyDA.GetValFact(data);
        }

        public ResponseVM relanzarDocumento(RelanzarDocumentoVM data)
        {
            return policyDA.relanzarDocumento(data);
        }

        public ErrorCode GetPayMethodsTypeValidate(string riesgo, string prima, string typeMovement)
        {
            return policyDA.GetPayMethodsTypeValidate(riesgo, prima, typeMovement);
        }

        // ACTUALIZAR ESTADO PAGADO EPS - DGC - 14/11/2023
        public Task<RespuestaVM> ActualizarEstadoPagadoEPS(ActualizarEstadoPagadoEPSFilter data)
        {
            return policyDA.ActualizarEstadoPagadoEPS(data);
        }

        public string GetPolicyTransListExcel(TransPolicySearchBM data)
        {
            return policyDA.GetPolicyTransListExcel(data);
        }

        public async Task<ValAgencySCTR> ValidateAgencySCTR(ValidateAgencySCTR request)
        {
            return await policyDA.ValidateAgencySCTR(request);
        }

        public async Task<ErrorCode> SendComprobantesEps(List<ComprobantesEpsBM> list_comprobantes)
        {
            return await policyDA.SendComprobantesEps(list_comprobantes);
        }

        public calcExec GetCalcExecSCTR(string nidproc, int ramo, int? producto, int? nrocotizacion, string npolicy, int varCalcExec, string fecha_exclusion)
        {
            return policyDA.GetCalcExecSCTR(nidproc, ramo, producto, nrocotizacion, npolicy, varCalcExec, fecha_exclusion);
        }

        public SalidadPolizaEmit updateQuotationCol(SavePolicyEmit request, SalidadPolizaEmit request2)
        {
            return policyDA.updateQuotationCol(request, request2);
        }

        public List<QuotizacionColBM> GetPolizaColegio(int nroCotizacion)
        {
            return policyDA.GetPolizaColegio(nroCotizacion);
        }

        public QuotationResponseVM ValidateProcessInsured(QuotationCabBM data)
        {
            return policyDA.ValidateProcessInsured(data);
        }

        public PolicyValidateQuotationTransacVM PolicyValidateQuotationTransac(PolicyValidateQuotationTransacBM data)
        {
            return policyDA.PolicyValidateQuotationTransac(data);
        }

        public PagoKushki getPagoKushki(int nroCotizacion_pen, string nidProc_pen, int nroCotizacion_sal, string nidProc_sal, string stype_transac)
        {
            return policyDA.getPagoKushki(nroCotizacion_pen, nidProc_pen, nroCotizacion_sal, nidProc_sal, stype_transac);
        }

        public PagoKushki getCodPagoKushki(int ramo, int idPayment_pen, int idPayment_sal)
        {
            return policyDA.getCodPagoKushki(ramo, idPayment_pen, idPayment_sal);
        }

        public NidProcessRenewVM GetProcessTrama(int nroCotizacion)
        {
            return policyDA.GetProcessTrama(nroCotizacion);
        }
        public ErrorCode PayDirectMethods(int nroCotizacion, string typeMovement)
        {
            return policyDA.PayDirectMethods(nroCotizacion, typeMovement);
        }

        public TypePayVM GetTipoPagoSCTR(int nroCotizacion)
        {
            return policyDA.GetTipoPagoSCTR(nroCotizacion);
        }
    }
}