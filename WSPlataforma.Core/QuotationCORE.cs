using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Web;
using WSPlataforma.DA;
using WSPlataforma.Entities.Cliente360.BindingModel;
using WSPlataforma.Entities.Cliente360.ViewModel;
using WSPlataforma.Entities.Graphql;
using WSPlataforma.Entities.HistoryCreditServiceModel.ViewModel;
using WSPlataforma.Entities.PolicyModel.ViewModel;
using WSPlataforma.Entities.QuotationModel.BindingModel;
using WSPlataforma.Entities.QuotationModel.BindingModel.QuotationModification;
using WSPlataforma.Entities.QuotationModel.ViewModel;
using WSPlataforma.Entities.TechnicalTariffModel.BindingModel;
using WSPlataforma.Entities.TechnicalTariffModel.ViewModel;
using WSPlataforma.Entities.EPSModel.ViewModel;
using WSPlataforma.Util;
using static WSPlataforma.Entities.Graphql.CotizadorGraph;
using Rule = WSPlataforma.Entities.Graphql.Rule;

namespace WSPlataforma.Core
{
    public class QuotationCORE
    {
        QuotationDA quotationDA = new QuotationDA();
        public SalidaInfoAuthBaseVM GetInfoQuotationAuth(InfoAuthBM infoAuth)
        {
            return quotationDA.GetInfoQuotationAuth(infoAuth);
        }

        public string GetProcessCode(int numeroCotizacion)
        {
            return quotationDA.GetProcessCode(numeroCotizacion);
        }

        public QuotationResponseVM ValidarReglasPago(ValidarPagoBM request)
        {
            return quotationDA.ValCotizacion(request.ncotizacion, request.nusercode);
        }
        public SalidaTramaBaseVM readInfoTramaDetailWeb(InfoClientBM infoClient, ref SalidaTramaBaseVM obj)
        {
            return quotationDA.readInfoTramaDetailWeb(infoClient, ref obj);
        }

        public SalidaTramaBaseVM readInfoTramaWeb(InfoClientBM infoClient, ref SalidaTramaBaseVM obj)
        {
            return quotationDA.readInfoTramaWeb(infoClient, ref obj);
        }

        public SalidaErrorBaseVM ValidateTramaWeb(InfoClientBM infoClient)
        {
            return quotationDA.ValidateTramaWeb(infoClient);
        }

        public List<BandejaVM> GetBandejaList(BandejaBM bandejaBM)
        {
            return quotationDA.GetBandejaList(bandejaBM);
        }

        public GenericResponseVM ChangeStatusVL(StatusChangeBM data, List<HttpPostedFile> fileList)
        {
            return quotationDA.ChangeStatusVL(data, fileList, null);
        }

        public CreditHistoryVM GetConfigCreditHistory(ConsultVM consultVM)
        {
            return quotationDA.GetConfigCreditHistory(consultVM);
        }

        public QuotationResponseVM UpdateCodQuotation(ValidaTramaBM request)
        {
            return quotationDA.UpdateCodQuotation(request);
        }

        public SalidaTramaBaseVM readInfoTramaDetail(ValidaTramaBM validaTramaBM, ref SalidaTramaBaseVM obj)
        {
            return quotationDA.readInfoTramaDetail(validaTramaBM, ref obj);
        }

        public SalidaTramaBaseVM readInfoTrama(ValidaTramaBM validaTramaBM, ref SalidaTramaBaseVM obj)
        {
            return quotationDA.readInfoTrama(validaTramaBM, ref obj);
        }

        public SalidaErrorBaseVM ValidateTramaVL_ECOM(ValidaTramaBM validaTramaBM)
        {
            return quotationDA.ValidateTramaVL_ECOM(validaTramaBM);
        }

        public SalidaErrorBaseVM ValidateTramaVL(ValidaTramaBM validaTramaBM)
        {
            return quotationDA.ValidateTramaVL(validaTramaBM);
        }

        public void deleteProcess(string idProcess)
        {
            quotationDA.deleteProcess(idProcess);
        }

        public SalidaErrorBaseVM ValidateTramaCovid(ValidaTramaBM validaTramaBM)
        {
            return quotationDA.ValidateTramaCovid(validaTramaBM);
        }

        public SalidaErrorBaseVM ValidateTramaAccidentesPersonales(ValidaTramaBM validaTramaBM, validaTramaVM objValida)
        {
            return quotationDA.ValidateTramaAccidentesPersonales(validaTramaBM, objValida);
        }

        public int ValCotizacionReglas(validaTramaVM request)
        {
            return quotationDA.ValCotizacionReglas(request);
        }

        public List<int> ValCantAseguradosPoliza(RecalculoTramaVM request)
        {
            return quotationDA.ValCantAseguradoPoliza(request);
        }

        public EmitPolVM UpdCotizacionDet(ProcesaTramaBM request)
        {
            return quotationDA.UpdCotizacionDet(request);
        }

        public SalidaTramaBaseVM InsCotizaTrama(validaTramaVM data, int flagTecnica = 0)
        {
            return quotationDA.InsertCotizaTrama(data, flagTecnica);
        }

        public SalidaTramaBaseVM InsertInfoMatriz(validaTramaVM data)
        {
            return quotationDA.InsertInfoMatriz(data);
        }

        public SalidaErrorBaseVM InsCotizaTramaPol(validaTramaVM data)
        {
            return quotationDA.InsertCotizaTramaPol(data);
        }

        public SalidaTramaBaseVM InsReCotizaTrama(validaTramaVM request)
        {
            return quotationDA.InsertReCotizaTrama(request);
        }

        //public EmitPolVM InsCoberturaDet(DatosPolizaVM request)
        //{
        //    return quotationDA.InsCoberturaDet(request);
        //}

        public SalidaTramaBaseVM ReaListCotiza(string codproceso, string tiporenov, string codProducto, string codRamo)
        {
            return quotationDA.ReaListCotiza(codproceso, tiporenov, codProducto, codRamo);
        }

        public SalidaPremiumVM ReaListDetCotiza(string idproc, string tipoplan, string codProducto, string codRamo, string flagCot = "2")
        {
            return quotationDA.ReaListDetCotiza(idproc, tipoplan, codProducto, codRamo, flagCot);
        }

        public SalidaTramaBaseVM ReaListDetPremiumAut(string nrocotizacion)
        {
            return quotationDA.ReaListDetPremiumAut(nrocotizacion);
        }

        public SalidaTramaBaseVM SaveUsingOracleBulkCopy(DataTable dt)
        {
            return quotationDA.SaveUsingOracleBulkCopy(dt);
        }
        public SalidaTramaBaseVM InsertAseguradosBulk(List<aseguradoMas> asegurados, int primary)
        {
            return quotationDA.InsertAseguradosBulk(asegurados, primary);
        }

        public GenericResponseVM GetTrackingList(TrackingBM data)
        {
            return quotationDA.GetTrackingList(data);
        }

        public string tramitePendiente(QuotationTrackingVM info, int condicionado)
        {
            return quotationDA.tramitePendiente(info, condicionado);
        }

        public GenericResponseVM ChangeStatus(StatusChangeBM data, List<HttpPostedFile> fileList)
        {
            return quotationDA.ChangeStatus(data, fileList, null);
        }

        public GenericResponseVM ChangeStatusRenovate(StatusChangeBM data, List<HttpPostedFile> fileList)
        {
            return quotationDA.ChangeStatusRenovate(data, fileList, null);
        }

        public GenericResponseVM GetQuotationList(QuotationSearchBM dataToSearch)
        {
            return quotationDA.GetQuotationList(dataToSearch);
        }

        public GenericResponseVM GetPolicyList(QuotationSearchBM dataToSearch)
        {
            return quotationDA.GetPolicyList(dataToSearch);
        }

        public List<StatusVM> GetStatusList(string certype, string codProduct)
        {
            return quotationDA.GetStatusList(certype, codProduct);
        }

        public List<ReasonVM> GetReasonList(Int32 statusCode, string branch)
        {
            return quotationDA.GetReasonList(statusCode, branch);
        }

        public async Task<QuotationResponseVM> InsertQuotation(QuotationCabBM request, List<HttpPostedFile> files, dataQuotation_EPS request_EPS)
        {
            return await quotationDA.InsertQuotation(request, files, request_EPS);
        }

        public async Task<QuotationResponseVM> ValidateQuotationEstadoMatriz(QuotationCabBM request, int NID_COTIZACION, int usercode)
        {
            return await quotationDA.ValidateQuotationEstadoMatriz(request, NID_COTIZACION, usercode);
        }

        public async Task<string> invocarServicioDPS(string json)
        {
            return await quotationDA.invocarServicioDPS(json);
        }

        public QuotationResponseVM UpdateDPS(int nidcotizacion, int user, string iddps, string tokendps, int estadodps)
        {
            return quotationDA.UpdateDPS(nidcotizacion, user, iddps, tokendps, estadodps);
        }

        public DpsToken FindDPS(int nidcotizacion, int branch, int nproduct)
        {
            return quotationDA.FindDPS(nidcotizacion, branch, nproduct);
        }

        public string LimpiarTrama(string idproc)
        {
            return quotationDA.LimpiarTrama(idproc);
        }

        public string LimpiarTramaDes(string idproc)
        {
            return quotationDA.LimpiarTramaDes(idproc);
        }

        public QuotationResponseVM UpdateReQuotation(InfoPropBM request)
        {
            return quotationDA.UpdateReQuotation(request);
        }

        public string GetProcessCodePD(string nrocotizacion)
        {
            return quotationDA.GetProcessCodePD(nrocotizacion);
        }

        public EmitPolVM ObtenerSkey(string nrocotizacion, string idproc)
        {
            return quotationDA.ObtenerSkey(nrocotizacion, idproc);
        }

        public SalidadPolizaEmit EmitPolicyPol(string cotizacion, string skey)
        {
            return quotationDA.EmitPolicyPol(cotizacion, skey);
        }

        public EmitPolVM TRAS_CARGA_ASE(ProcesaTramaBM request)
        {
            return quotationDA.TRAS_CARGA_ASE(request);
        }

        public EmitPolVM Update_header(string idproc)
        {
            return quotationDA.UpdateHeader(idproc);
        }
        public EmitPolVM UpdateInsuredPremium_Matriz(ProcesaTramaBM request, int flagTecnica = 0)
        {
            return quotationDA.UpdateInsuredPremium_Matriz(request, flagTecnica);
        }

        public int ValidardataTrama(string idproc)
        {
            return quotationDA.ValidarDataTrama(idproc);
        }

        public EmitPolVM INS_JOB_CLOSE_SCTR(ProcesaTramaBM request)
        {
            return quotationDA.INS_JOB_CLOSE_SCTR(request);
        }

        public BrokerResponseVM SearchBroker(BrokerSearchBM data)
        {
            return quotationDA.SearchBroker(data);
        }

        public QuotationResponseVM ApproveQuotation(QuotationUpdateBM data)
        {
            return quotationDA.ApproveQuotation(data);
        }

        public QuotationResponseVM ModifyQuotation(QuotationModification data, List<HttpPostedFile> statusChangeFileList)
        {
            return quotationDA.ModifyQuotation(data, statusChangeFileList);
        }

        public string equivalentMunicipality(Int64 municipality)
        {
            return quotationDA.equivalentMunicipality(municipality);
        }

        public string GetIgv(QuotationIGVBM data)
        {
            return quotationDA.GetIgv(data);
        }
        public async Task<TariffVM> GetTariff(ProtectaBM data)
        {
            return await quotationDA.getTariffProtecta(data);
        }
        public async Task<ResponseCVM> GesCliente360(ConsultaCBM data, string asegurado)
        {
            return await quotationDA.GesCliente360(data, asegurado);
        }

        public List<ComisionVM> ComisionGet(int nroCotizacion)
        {
            return quotationDA.ComisionGet(nroCotizacion);
        }

        public List<GlossVM> GlossGet()
        {
            return quotationDA.GlossGet();
        }
        public void insertHistTrama(ValidaTramaBM trama)
        {
            quotationDA.insertHistTrama(trama);
        }

        public List<PlanVM> GetPlansList(PlanBM data)
        {
            return quotationDA.GetPlansList(data);
        }

        public SalidaTramaBaseVM getDetail(validaTramaVM objValida, ref SalidaTramaBaseVM obj)
        {
            return quotationDA.getDetail(objValida, ref obj);
        }

        public SalidaTramaBaseVM getDetailAmount(validaTramaVM objValida, ref SalidaTramaBaseVM obj)
        {
            return quotationDA.getDetailAmount(objValida, ref obj);
        }

        public List<PlanVM> GetPlans()
        {
            return quotationDA.getPlans();
        }

        //marcos silverio
        public GenericResponseVM UpdateStatusQuotationVidaLey(UpdateStatusQuotationBM data, DataTable dtCover)
        {
            return quotationDA.UpdateStatusQuotationVidaLey(data, dtCover);
        }
        public QuotationRequestDM ReadInfoQuotationDM(int nroCotizacion, int tipoEndoso)
        {
            return quotationDA.ReadInfoQuotationDM(nroCotizacion, tipoEndoso);
        }
        public List<QuotationCoverVM> GetQuotationCoverByNumQuotation(long nroCotizacion)
        {
            return quotationDA.GetQuotationCoverByNumQuotation(nroCotizacion);
        }
        public string GetCodeuserByUsername(string username)
        {
            return quotationDA.GetCodeuserByUsername(username);
        }
        public GenericResponseVM DeleteQuotationCover(int idCotizacion)
        {
            return quotationDA.DeleteQuotationCover(idCotizacion);
        }
        public QuotationByPolicyVM GetQuotationByPolicy(int npolicy)
        {
            return quotationDA.GetQuotationByPolicy(npolicy);
        }

        public QuotationResponseVM ValidateRetroactivity(QuotationCabBM request)
        {
            return quotationDA.ValRetroCotizacion(request, Convert.ToInt32(request.NumeroCotizacion));
        }
        public QuotationResponseVM UpdateFechaEfectoAsegurado(QuotationCabBM request)
        {
            return quotationDA.UpdateFechaEfectoAsegurado(request);
        }

        public string GetExcelQuotationList(QuotationSearchBM dataToSearch)
        {
            return quotationDA.GetExcelQuotationList(dataToSearch);
        }
        public SalidaErrorBaseVM ValidateTramaEndosoVL(ValidaTramaBM validaTramaBM)
        {
            return quotationDA.ValidateTramaEndosoVL(validaTramaBM);
        }
        public SalidaTramaBaseVM readInfoTramaEndoso(ValidaTramaBM validaTramaBM, ref SalidaTramaBaseVM obj)
        {
            return quotationDA.readInfoTramaEndoso(validaTramaBM, ref obj);
        }

        // INI ENDOSO TECNICA JTV 22022023
        /*public SalidaTramaBaseVM readInfoTramaDetailEndoso(ValidaTramaBM validaTramaBM, ref SalidaTramaBaseVM obj)
        {
            return quotationDA.readInfoTramaDetailEndoso(validaTramaBM, ref obj);
        }*/
        // FIN ENDOSO TECNICA JTV 22022023

        public List<PlanVM> GetPlansListAcc(PlanBM data)
        {
            return quotationDA.GetPlansListAcc(data);
        }

        public List<TipoPlanVM> GetTipoPlan(PlanBM data)
        {
            return quotationDA.GetTipoPlan(data);
        }

        public List<ModalidadVm> GetModalidad(PlanBM data)
        {
            return quotationDA.GetModalidad(data);
        }

        public List<CoberturasVM> GetCoberturas(CoberturaBM data)
        {
            return quotationDA.GetCoberturas(data);
        }

        public List<CoberturasVM> GetCoberturasGenDev(CoberturaBM data)
        {
            return quotationDA.GetCoberturasGenDev(data);
        }

        public ReponsePrimaPlanVM CalculoPrimaEC(validaTramaVM data)
        {
            return quotationDA.CalculoPrimaEC(data);
        }

        //public List<PrimaPlanVM> GetPrimaPlan(PrimaPlanBM data)
        //{
        //    return quotationDA.GetPrimaPlan(data);
        //}

        public ComisionVM GetListComision()
        {
            return quotationDA.GetListComision();
        }

        public SalidaErrorBaseVM regDetallePlanCot(validaTramaVM data, int condicion = 0, int tecnica = 0)
        {
            return quotationDA.regDetallePlanCot(data, condicion, tecnica);
        }

        public SalidaErrorBaseVM UpdCoberturasProp(string covergen, string sumaProp, string codProc, int flag)
        {
            return quotationDA.UpdCoberturasProp(covergen, sumaProp, codProc, flag);
        }

        public PremiumBM GetPremiumBM()
        {
            return quotationDA.GetPremiumMin();
        }

        public FechasRenoVM ObtenerFechasRenov(string nrocotizacion, string typetransac, string codproducto)
        {
            return quotationDA.ObtenerFechasRenovacion(nrocotizacion, typetransac, codproducto);
        }
        public GenericResponseVM getFechaFin(string fecha, int freq)
        {
            return quotationDA.getFechaFin(fecha, freq);
        }
        public void InsertLog(long cotizacion, string proceso, string url, string parametros, string resultados)
        {
            quotationDA.InsertLog(cotizacion, proceso, url, parametros, resultados);
        }

        public ResponseBenefit ListBenefits(BenefitBM data)
        {
            return quotationDA.ListBenefits(data);
        }

        public ResponseBenefit ListSurcharges(BenefitBM data)
        {
            return quotationDA.ListSurcharges(data);
        }

        public ResponseAssists ListAssists(AssistsBM data)
        {
            return quotationDA.ListAssists(data);
        }

        public ResponseBenefit ListAdditionalServices(BenefitBM data)
        {
            return quotationDA.ListAdditionalServices(data);
        }

        public GenericResponseVM UpdateStatusCotPD(CoverBM data, DataTable dtCover)
        {
            return quotationDA.UpdateStatusCotPD(data, dtCover);
        }
        public GenericResponseVM UpdateStatusCotPD(InsertAssitsBM data, DataTable dtCover)
        {
            return quotationDA.UpdateStatusCotPD(data, dtCover);
        }
        public GenericResponseVM UpdateStatusCotPD(InsertBenefitBM data, DataTable dtCover)
        {
            return quotationDA.UpdateStatusCotPD(data, dtCover);
        }

        public EmitPolVM transferDataPM(string idproc)
        {
            return quotationDA.transferDataPM(idproc);
        }

        public async Task<ResponseGraph> SendDataQuotationGraphql(string nroQuotation, string trxCode, int polizaMatriz, string rutafiles)
        {
            return await new GraphqlDA().SendDataQuotationGraphql(nroQuotation, trxCode, polizaMatriz, rutafiles);
        }

        public async Task<ResponseGraph> SendDataTarifarioGraphql(validaTramaVM dataCotizacion)
        {
            return await new GraphqlDA().SendDataTarifarioGraphql(dataCotizacion);
        }

        //public SalidaErrorBaseVM regDetallePlanCot(validaTramaVM data, Entities.PolicyModel.ViewModel.GenericPackageVM dataQuotation)
        //{
        //    return quotationDA.regDetallePlanCot(data, dataQuotation);
        //}

        public SalidaTramaBaseVM insertCoverRequerid(validaTramaVM objValida, List<Coverage> cover)
        {
            return quotationDA.insertCoverRequerid(objValida, cover);
        }

        public SalidaErrorBaseVM insertAssistRequeridDes(validaTramaVM objValida, AssistanceDes assistance)
        {
            return quotationDA.insertAssistRequeridDes(objValida, assistance);
        }
        public SalidaErrorBaseVM insertCoverRequeridDes(validaTramaVM objValida, CoverageDes cover)
        {
            return quotationDA.insertCoverRequeridDes(objValida, cover);
        }

        public SalidaErrorBaseVM insertBenefitRequeridDes(validaTramaVM objValida, BenefitDes beneficios)
        {
            return quotationDA.insertBenefitRequeridDes(objValida, beneficios);
        }

        public SalidaTramaBaseVM updatePrimaCotizador(updateCotizador data)
        {
            return quotationDA.updatePrimaCotizador(data);
        }

        public SalidaErrorBaseVM InsCargaMasEspecial(validaTramaVM request)
        {
            return quotationDA.InsCargaMasEspecial(request);
        }
        public SalidaErrorBaseVM InsCargaMasEspecialBeneficiarios(validaTramaVM request, DatosBenefactors beneficiarios)
        {
            return quotationDA.InsCargaMasEspecialBeneficiarios(request, beneficiarios);
        }

        public SalidaTramaBaseVM DeleteInsuredCot(validaTramaVM data)
        {
            return quotationDA.DeleteInsuredCot(data);
        }

        public SalidaTramaBaseVM InsertInsuredCot(validaTramaVM data)
        {
            return quotationDA.InsertInsuredCot(data);
        }

        public validaTramaVM getInfoQuotationTransac(validaTramaVM request)
        {
            return quotationDA.getInfoQuotationTransac(request);
        }

        //public async Task<ResponseGraph> getSegmentsGraph(string nbranch)
        //{
        //    return await new GraphqlDA().getSegmentsGraph(nbranch);
        //}

        public async Task<ResponseGraph> getSegmentGraph(string segmentId, string nbranch)
        {
            return await new GraphqlDA().getSegmentGraph(segmentId, nbranch);
        }

        public SalidaTramaBaseVM InsertReajustes(validaTramaVM objValida, ReadjustmentFactor data)
        {
            return quotationDA.InsertReajustes(objValida, data);
        }

        public SalidaTramaBaseVM InsertRecargos(validaTramaVM objValida, List<SurchargeTariff> data)
        {
            return quotationDA.InsertRecargos(objValida, data);
        }

        public SalidaTramaBaseVM InsertExclusiones(validaTramaVM objValida, List<ExclusionTariff> data)
        {
            return quotationDA.InsertExclusiones(objValida, data);
        }

        public ConsultaCBM equivalenteTipoDocumento(ConsultaCBM item)
        {
            return quotationDA.equivalenteTipoDocumento(item);
        }

        public List<StatusVM> GetStatusListRQ(string certype, string codProduct)
        {
            return quotationDA.GetStatusListRQ(certype, codProduct);
        }

        public SalidaErrorBaseVM regLogCliente360(ConsultaCBM item, ResponseCVM response360)
        {
            return quotationDA.regLogCliente360(item, response360);
        }

        public List<insuredGraph> GetTramaAsegurado(string codProceso)
        {
            return quotationDA.GetTramaAsegurado(codProceso);
        }

        public GenericResponseVM AgrupacionAPVG(ValidaTramaBM request)
        {
            return quotationDA.AgrupacionAPVG(request);
        }

        public string getProfileRepeat(string nbranch, string codPerfil)
        {
            return new GraphqlDA().getProfileRepeat(nbranch, codPerfil);
        }

        public bool validarAforo(string ramo, string perfil, string stransac)
        {
            return new GraphqlDA().validarAforo(ramo, perfil, stransac);
        }

        public string codPerfil(string perfil)
        {
            return new GraphqlDA().codPerfil(perfil);
        }

        public int validarConsultaAsegurado(ConsultaCBM item, ConsultaCBM request)
        {
            return quotationDA.validarConsultaAsegurado(item, request);
        }

        public int validarConsultaAseguradoPD(ConsultaCBM item, ConsultaCBM request)
        {
            return quotationDA.validarConsultaAseguradoPD(item, request);
        }

        public PremiumFixVM TotalPremiumFix(PremiumFixBM data)
        {
            return quotationDA.TotalPremiumFix(data);
        }

        public PremiumFixVM TotalPremiumCred(PremiumFixBM data) //AVS PRY NC
        {
            return quotationDA.TotalPremiumCred(data);
        }

        public CipValidateVM validarCipExistente(CipValidateBM data)
        {
            return quotationDA.validarCipExistente(data);
        }

        public void UpdCargaTramaEndoso(ValidaTramaBM validaTramaBM)
        {
            quotationDA.UpdCargaTramaEndoso(validaTramaBM);
        }
        public async Task<ResponseGraph> getTariffGraph(TariffGraph data)
        {
            return await new GraphqlDA().getTariffGraph(data);
        }
        public async Task<ResponseGraph> getMiniTariffGraph(TariffGraph data, validaTramaVM request = null)
        {
            return await new GraphqlDA().getMiniTariffGraph(data, request);
        }
        public async Task<ResponseGraphDes> getMiniTariffGraphDes(TariffGraph data)
        {
            return await new GraphqlDA().getMiniTariffGraphDes(data);
        }

        public async Task<RulesLogic> getRulesLogic(List<Rule> data)
        {
            return await new GraphqlDA().getRulesLogic(data);
        }

        public async Task<RulesLogic> getRulesLogicDes(List<Rule> data)
        {
            return await new GraphqlDA().getRulesLogicDes(data);
        }

        public async Task<bool> valMiniTariffGraph(TariffGraph data)
        {
            return await new GraphqlDA().valMiniTariffGraph(data);
        }

        public int cantidadMeses(int renovationType, int nbranch = 0)
        {
            return new GraphqlDA().cantidadMeses(renovationType, nbranch);
        }

        public int cantidadMesesDes(int renovationType)
        {
            return new GraphqlDA().cantidadMesesDes(renovationType);
        }

        public SalidaTramaBaseVM insertRules(List<Rule> data, validaTramaVM info)
        {
            return quotationDA.insertRules(data, info);
        }
        public SalidaTramaBaseVM insertRulesDes(List<Rule> data, validaTramaVM info)
        {
            return quotationDA.insertRulesDes(data, info);
        }
        public SalidaTramaBaseVM insertRulesDes(Rule data, validaTramaVM info)
        {
            return quotationDA.insertRulesDes(data, info);
        }

        public SalidaErrorBaseVM insertSubItemTrx(validaTramaVM objValida, ItemGeneric subItem)
        {
            return quotationDA.insertSubItem(objValida, subItem);
        }

        public SalidaTramaBaseVM insertSubItemPD(validaTramaVM objValida, List<Coverage> coberturas)
        {
            return quotationDA.insertSubItemPD(objValida, coberturas);
        }

        public PremiumFixVM ReverseMovementsIncomplete(PremiumFixBM data)
        {
            return quotationDA.ReverseMovementsIncomplete(data);
        }

        public AuthDerivationVM getAuthDerivation(validaTramaVM data)
        {
            return quotationDA.getAuthDerivation(data);
        }

        public validaTramaVM valMinMaxAseguradosPolMatriz(validaTramaVM data)
        {
            return quotationDA.valMinMaxAseguradosPolMatriz(data);
        }

        public SalidaErrorBaseVM valSumasASeguradas(validaTramaVM data)
        {
            return quotationDA.valSumasASeguradas(data);
        }
        //INI MEJORAS VALIDACION VL 
        public SalidaErrorBaseVM updateAsegReniec(ConsultaCBM item, EListClient client, validaTramaVM objValida = null)
        {
            return quotationDA.updateAsegReniec(item, client, objValida);
        }
        //FIN MEJORAS VALIDACION VL 
        public async Task<ResponseGraph> getObtenerPrimaPD(validaTramaVM dataCotizacion)
        {
            return await quotationDA.getObtenerPrimaPD(dataCotizacion);
        }

        public ResponseCVM GesClienteReniecTbl(ConsultaCBM request)
        {
            return quotationDA.GesClienteReniecTbl(request);
        }

        public QuotationResponseVM GetFlagPremiumMin(QuotationCabBM data)
        {
            return quotationDA.GetFlagPremiumMin(data);
        }

        // Desgravament 
        public SalidaTramaBaseVM InsGenBenefactors(validaTramaVM request)
        {
            return quotationDA.InsGenBenefactors(request);
        }
        public SalidaTramaBaseVM DeleteBenefactors(string codProceso, string rol)
        {
            return quotationDA.DeleteBenefactors(codProceso, rol);
        }

        public List<BeneficiarVM> GetBeneficiarsList(BeneficiarBM data)
        {
            return quotationDA.GetBeneficiarsList(data);
        }

        public NumCredit UpdateNumCredit(NumCreditUPD request)
        {
            return quotationDA.UpdateNumCredit(request);
        }

        public SalidaErrorBaseVM regDPS(int nidcotizacion, string codproceso, int branch, int codprod, int usuario, Entities.QuotationModel.ViewModel.DatosDPS datosdps)
        {
            return quotationDA.regDPS(nidcotizacion, codproceso, branch, codprod, usuario, datosdps);
        }

        public List<DpsVM> getDPS(int numcotizacion, int nbranch, int nproduct)
        {
            return quotationDA.getDPS(numcotizacion, nbranch, nproduct);
        }

        public long getMaxTrama(string param, string codRamo)
        {
            return quotationDA.getMaxTrama(param, codRamo);
        }

        public long getMaxReg(string param)
        {
            return quotationDA.getMaxReg(param);
        }

        public SalidaTramaBaseVM InsertDataEC(ValidaTramaEcommerce data)
        {
            return quotationDA.InsertDataEC(data);
        }

        public List<coberturasDetail> getCoverEcommerce(ValidaTramaEcommerce objValida, int cod_plan)
        {
            return quotationDA.getCoverEcommerce(objValida, cod_plan);
        }
        public EdadValidarVM GetEdadesValidar(CoberturaBM rangoEdades)
        {
            return quotationDA.GetEdadesValidar(rangoEdades);
        }

        public SobrevivenciaVM GETSOBREVIXCOVER(string nidcotizacion, string nbranch)
        {
            return quotationDA.GETSOBREVIXCOVER(nidcotizacion, nbranch);
        }

        public bool ValEstadoCotizacion(string nroCotizacion)
        {
            return quotationDA.ValEstadoCotizacion(nroCotizacion);
        }
        // GCAA 30012024
        public bool ValTransaccionCotizacion(string nroCotizacion, int opcion)
        {
            return quotationDA.ValTransaccionCotizacion(nroCotizacion, opcion);
        }

        public PrimaProrrateo GetPrimaProrrateo(int ncotizacion)
        {
            return quotationDA.GetPrimaProrrateo(ncotizacion);
        }
        //ARJG 19/09/2022
        public ValidGesClie ValidarGestClie(string idproc)
        {
            return quotationDA.ValidarGestClie(idproc);
        }

        //ARJG 20/09/2022
        public SalidaErrorBaseVM ConsultaAseguradoApi(string idproc)
        {
            return quotationDA.ConsultaAseguradoApi(idproc);
        }

        public async Task<QuotationResponseVM> updateCotizacionClienteEstado(QuotationCabBM request)
        {
            return await quotationDA.updateCotizacionClienteEstado(request);
        }

        public EmitPolVM finalizarCotizacionEstadoVL(ProcesaTramaBM request)
        {
            return quotationDA.finalizarCotizacionEstadoVL(request);
        }

        public SalidaErrorBaseVM updateCargaExclusion(ValidaTramaBM validaTramaBM)
        {
            return quotationDA.updateCargaExclusion(validaTramaBM);
        }

        public async Task<ResponseGraph> confirmQuote(ResponseUpdateVM data)
        {
            return await new GraphqlDA().confirmQuote(data);
        }

        public int getFactor(string nbranch, string nproduct, string ncover, string sclient)
        {
            return quotationDA.getFactor(nbranch, nproduct, ncover, sclient);
        }

        public SalidaTramaBaseVM InsertInsuredCotRentEst(validaTramaVM data)
        {
            return quotationDA.InsertInsuredCotRentEst(data);
        }

        public SalidaTramaBaseVM getTextCoverDetail(validaTramaVM data)
        {
            return quotationDA.getTextCoverDetail(data);
        }

        public validateBrokerVL validateBroker(validateBrokerVL data)
        {
            return quotationDA.validateBroker(data);
        }
        public DeleteIgv DeleteIgv(validaTramaVM data)
        {
            return quotationDA.DeleteIgv(data);
        }

        public GenericResponseEPS UpdateCotizacionState(SendStateEPS data)
        {
            return quotationDA.UpdateCotizacionState(data);
        }

        public GenericResponseVM NullQuotationCIP(QuotationCabBM data)
        {
            return quotationDA.NullQuotationCIP(data);
        }

        public rea_Broker GetBrokerAgenciadoSCTR(string P_SCLIENT, int P_TIPO)
        {
            return quotationDA.GetBrokerAgenciadoSCTR(P_SCLIENT, P_TIPO);
        }

        public string equivalentINEI(Int64 municipality)
        {
            return quotationDA.equivalentINEI(municipality);
        }

        public rmv_ACT obtRMV(string P_DATE)
        {
            return quotationDA.obtRMV(P_DATE);
        }

        public double GetIGV(validaTramaVM data)
        {
            return quotationDA.GetIGV(data);
        }

        public List<listComisionesTecnica> GetComisionTecnica(string P_SCLIENT)
        {
            return quotationDA.GetComisionTecnica(P_SCLIENT);
        }

        public List<listtasastecnica> GetTasastecnica(string P_SCLIENT)
        {
            return quotationDA.GetTasastecnica(P_SCLIENT);
        }

        public ClienteEstado GetEstadoClienteNuevo(string P_SCLIENT)
        {
            return quotationDA.GetEstadoClienteNuevo(P_SCLIENT);
        }

        public async Task<SJSON_CIP> getSJSON_CIP(CipRelanzamiento data)
        {
            return await quotationDA.getSJSON_CIP(data);
        }

        public async Task<SJSON_CIP> getSJSON_LINK(LinkRelanzamiento data)
        {
            return await quotationDA.getSJSON_LINK(data);
        }

        public List<CouponList> getCuponesExclusion(string nid_proc)
        {
            return quotationDA.getCuponesExclusion(nid_proc);
        }

        public void saveQuotationEps(QuotationResponseVM data, QuotationCabBM cotizacion, dataQuotation_EPS request_EPS)
        {
            var returnEPS = quotationDA.saveQuotationEps(data, cotizacion, request_EPS);
        }

        public List<CotizadorDetalleVM> detailQuotationInfo(List<insuredGraph> listAsegurados, validaTramaVM objValida)
        {
            return quotationDA.detailQuotationInfo(listAsegurados, objValida);
        }

        public infoQuotationPreviewVM GetInfoQuotationPreview(infoQuotationPreviewBM request)
        {
            return quotationDA.GetInfoQuotationPreview(request);
        }

        public SalidaTramaBaseVM updateCoverageRE(studentCoverages aseg, InfoCover cover, validaTramaVM objValida)
        {
            return quotationDA.updateCoverageRE(aseg, cover, objValida);
        }

        public void GenerateInfoAmount(double igv, string trx, ref validaTramaVM objValida)
        {
            quotationDA.GenerateInfoAmount(igv, trx, ref objValida);
        }

        public SalidaErrorBaseVM regDetallePlanValidacion(validaTramaVM data, string origen)
        {
            return quotationDA.regDetallePlanValidacion(data, origen);
        }

        public EmitPolVM InsCotizaHisTecnica(int nroCotizacion)
        {
            return quotationDA.InsCotizaHisTecnica(nroCotizacion);
        }

        public string getQuotationType(validaTramaVM objValida)
        {
            return quotationDA.getQuotationType(objValida);
        }
    }
}