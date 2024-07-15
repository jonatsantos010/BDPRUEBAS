using System.Data;
using System.Threading.Tasks;
using WSPlataforma.DA;
using WSPlataforma.Entities.RentasModel.BindingModel;
using WSPlataforma.Entities.RentasModel.ViewModel;

namespace WSPlataforma.Core
{
    public class RentasCORE
    {
        RentasDA DataAccessQuery = new RentasDA();

        public Task<RentasAprobacionesResVM> ListarAprobacionesRentasRes(RentasAprobacionesResBM data)
        {
            return this.DataAccessQuery.ListarAprobacionesRentasRes(data);
        }
        public Task<RentasAprobacionesDetVM> ListarAprobacionesRentasDet(RentasAprobacionesDetBM data)
        {
            return this.DataAccessQuery.ListarAprobacionesRentasDet(data);
        }
        public Task<ResponseVM> AprobarPagos(RentasAprobarPagosBM data)
        {
            return this.DataAccessQuery.AprobarPagos(data);
        }
        public DataSet GetDataReportRentasRes(ReportRentasResBM data)
        {
            return this.DataAccessQuery.GetDataReportRentasRes(data);
        }
        public DataSet GetDataReportRentasDet(ReportRentasDetBM data)
        {
            return this.DataAccessQuery.GetDataReportRentasDet(data);
        }
        public RentasFilterTicketsResponseVM PD_REA_CLIENT_TICKETS(RentasFilterTicketsBM data)
        {
            return this.DataAccessQuery.PD_REA_CLIENT_TICKETS(data);
        }
        public RentasFilterTicketResponseVM PD_REA_CLIENT_TICKET(RentasFilterTicketBM data)
        {
            return this.DataAccessQuery.PD_REA_CLIENT_TICKET(data);
        }
        public RentasListProductcsResponseVM PD_GET_PRODUCTOS()
        {
            return this.DataAccessQuery.PD_GET_PRODUCTOS();
        }
        public RentasListProductcsResponseVM PD_GET_MOTIVOS()
        {
            return this.DataAccessQuery.PD_GET_MOTIVOS();
        }
        public RentasListSubMotivosResponseVM PD_GET_SUBMOTIVOS(RentasSubMotivoscsBM data)
        {
            return this.DataAccessQuery.PD_GET_SUBMOTIVOS(data);
        }
        public RentasASSIGNEXECResponseVM PD_UPD_ASSIGNEXEC(RentasASSIGNEXECBM data)
        {
            return this.DataAccessQuery.PD_UPD_ASSIGNEXEC(data);
        }
        public RentasListSubMotivosResponseVM PD_REA_CLIENTS(RentasClientsBM data)
        {
            return this.DataAccessQuery.PD_REA_CLIENTS(data);
        }
        public RentasListEstadosResponseVM PD_GET_ESTADOS()
        {
            return this.DataAccessQuery.PD_GET_ESTADOS();
        }
        public RentasListEstadosResponseVM PD_GET_EJECUTIVOS(RentaseEjecutivosBM data)
        {
            return this.DataAccessQuery.PD_GET_EJECUTIVOS(data);
        }
        public RentasListEstadosResponseVM PD_GET_TYPE_DOCUMENT()
        {
            return this.DataAccessQuery.PD_GET_TYPE_DOCUMENT();
        }
        public RentasListEstadosResponseVM PD_GET_TYPE_PERSON()
        {
            return this.DataAccessQuery.PD_GET_TYPE_PERSON();
        }
        public RentasValidaResponseVM PD_VALFORMATVALUES(RentasValidaBM data)
        {
            return this.DataAccessQuery.PD_VALFORMATVALUES(data);
        }
        public RentasFilterDayResponseVM PD_GET_FILTERDAY_START()
        {
            return this.DataAccessQuery.PD_GET_FILTERDAY_START();
        }
        public RentasProductCanalResponseVM PD_GET_NPRODUCTCANAL()
        {
            return this.DataAccessQuery.PD_GET_NPRODUCTCANAL();
        }
        public RentasListActionsResponseVM PD_REA_LIST_ACTIONS(RentasListActionsBM data)
        {
            return this.DataAccessQuery.PD_REA_LIST_ACTIONS(data);
        }
        public RentasNidProfileResponseVM PD_GET_NIDPROFILE(RentasNidProfileBM data)
        {
            return this.DataAccessQuery.PD_GET_NIDPROFILE(data);
        }
        public RentasListActionsTikectResponseVM PD_REA_LIST_ACTIONS_TICKET(RentasListActionsTikectBM data)
        {
            return this.DataAccessQuery.PD_REA_LIST_ACTIONS_TICKET(data);
        }
        public RentasStatusTicketTikectResponseVM PD_UPD_STATUS_TICKET(RentasStatusTicketTikectBM data)
        {
            return this.DataAccessQuery.PD_UPD_STATUS_TICKET(data);
        }
        public async Task<RentasGetPolicyDataResponseVM> GET_POLICY_DATA(RentasGetPolicyDataBM data)
        {
            return await this.DataAccessQuery.GET_POLICY_DATA(data);
        }
        public async Task<RentasGetCalculationAmountResponseVM> GET_CALCULATION_AMOUNT(RentasGetCalculationAmountBM data)
        {
            return await this.DataAccessQuery.GET_CALCULATION_AMOUNT(data);
        }
        public async Task<RentasGetCalculationAmountResponseVM> GET_CALCULATION_AMOUNT_DUMMY(RentasGetCalculationAmountBM data)
        {
            return await this.DataAccessQuery.GET_CALCULATION_AMOUNT_DUMMY(data);
        }
        public RentasGetCalculationAmountVM PD_UPD_AMOUNT_TICKET(RentasUpdAmountTicketBM data)
        {
            return this.DataAccessQuery.PD_UPD_AMOUNT_TICKET(data);
        }
        public RentasListAdjResponseVM PD_LIST_ADJ(RentasListAdjBM data)
        {
            return this.DataAccessQuery.PD_LIST_ADJ(data);
        }
        public RentasUpdAttachmentVM PD_UPD_ATTACHMENT(RentasUpdAttachmentBM data)
        {
            return this.DataAccessQuery.PD_UPD_ATTACHMENT(data);
        }
        public RentasInsDataEmailResponseVM PD_INS_DATA_EMAIL(RentasInsDataEmailBM data)
        {
            return this.DataAccessQuery.PD_INS_DATA_EMAIL(data);
        }
        public string PD_GET_ROUTE_FILE()
        {
            return this.DataAccessQuery.PD_GET_ROUTE_FILE();
        }
        public RentasInsDataEmailResponseVM PD_INS_TBL_TICK_ADJUNT(RentasInsTickAdjuntBM data)
        {
            return this.DataAccessQuery.PD_INS_TBL_TICK_ADJUNT(data);
        }
        public RentasInsDataEmailResponseVM PD_DEL_TBL_TICK_ADJUNT(RentasDelTickAdjuntBM data)
        {
            return this.DataAccessQuery.PD_DEL_TBL_TICK_ADJUNT(data);
        }
        public RentasUpdTicketDescriptResponseVM PD_UPD_TICKET_DESCRIPT(RentasUpdTicketDescriptBM data)
        {
            return this.DataAccessQuery.PD_UPD_TICKET_DESCRIPT(data);
        }
        public RentasUpdTicketNmotivResponseVM PD_UPD_TICKET_NMOTIV(RentasUpdTicketNmotivBM data)
        {
            return this.DataAccessQuery.PD_UPD_TICKET_NMOTIV(data);
        }
        public RentasGetUserResponsibleResponseVM PD_GET_USER_RESPONSIBLE(RentasGetUserResponsibleBM data)
        {
            return this.DataAccessQuery.PD_GET_USER_RESPONSIBLE(data);
        }
        public RentasGetValpopupResponseVM PD_GET_VALPOPUP(RentasGetValpopupBM data)
        {
            return this.DataAccessQuery.PD_GET_VALPOPUP(data);
        }
        public RentasGenericResponseVM PD_GET_TYPECOMMENT()
        {
            return this.DataAccessQuery.PD_GET_TYPECOMMENT();
        }
        public RentasGenericResponseVM PD_GET_DESTINATION()
        {
            return this.DataAccessQuery.PD_GET_DESTINATION();
        }
        public RentasGenericResponseVM PD_GET_EMAIL_DESTINATION(RentasGetEmailDestinationBM data)
        {
            return this.DataAccessQuery.PD_GET_EMAIL_DESTINATION(data);
        }
        public RentasGenericResponseVM PD_GET_EMAIL_USER(RentasGetEmailUserBM data)
        {
            return this.DataAccessQuery.PD_GET_EMAIL_USER(data);
        }
        public RentasGenericResponseVM PD_GET_CONF_FILE()
        {
            return this.DataAccessQuery.PD_GET_CONF_FILE();
        }
        public RentasGenericResponseVM PD_GET_MESSAGE(RentasGetMessage data )
        {
            return this.DataAccessQuery.PD_GET_MESSAGE(data);
        }
    }
}
