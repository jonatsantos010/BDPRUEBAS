using System.Data;
using System.Threading.Tasks;
using WSPlataforma.DA;
using WSPlataforma.Entities.InterfaceMonitoringModel.BindingModel;
using WSPlataforma.Entities.InterfaceMonitoringModel.ViewModel;

namespace WSPlataforma.Core
{
    public class InterfaceMonitoringCBCO_CORE
    {
        InterfaceMonitoringCBCO_DA DataAccessQuery = new InterfaceMonitoringCBCO_DA();

        // ESTADOS DE PROCESO
        public Task<EstadosVM> ListarEstados()
        {
            return this.DataAccessQuery.ListarEstados();
        }

        // CABECERA DE INTERFACES
        public Task<InterfaceMonitoring_CB_VM> ListarCabeceraInterfaces_CBCO(InterfaceMonitoringFilter response)
        {
            return this.DataAccessQuery.ListarCabeceraInterfaces_CBCO(response);
        }

        // CABECERA POR RECIBO (BUSQUEDA)
        public Task<InterfaceMonitoring_CB_VM> ListarCabeceraInterfacesCBCORecibo(InterfaceMonitoringReciboFilter response)
        {
            return this.DataAccessQuery.ListarCabeceraInterfacesCBCORecibo(response);
        }

        // DETALLE PRELIMINAR
        public Task<InterfaceMonitoringDetail_CB_VM> ListarDetalleInterfaces(InterfaceMonitoringDetailFilter response)
        {
            return this.DataAccessQuery.ListarDetalleInterfaces(response);
        }
        public Task<InterfacegDetailOperationVM> ListarDetalleOperacion(InterfaceMonitoringDetailOperacionFilter response)
        {
            return this.DataAccessQuery.ListarDetalleOperacion(response);
        }
        public DataSet GetDataReport(InterfaceMonitoringDetailXLSXFilter_CB data)
        {
            return this.DataAccessQuery.GetDataReport(data);
        }

        // DETALLE ASIENTOS
        public Task<InterfaceMonitoringAsientosContablesDetail_CB_VM> ListarDetalleInterfacesAsientosContables(InterfaceMonitoringAsientosContablesDetailFilter response)
        {
            return this.DataAccessQuery.ListarDetalleInterfacesAsientosContables(response);
        }
        public Task<InterfaceMonitoringAsientosContablesDetailAsiento_CB_VM> ListarDetalleInterfacesAsientosContablesAsiento(InterfaceMonitoringAsientosContablesDetailAsiento_CB_Filter response)
        {
            return this.DataAccessQuery.ListarDetalleInterfacesAsientosContablesAsiento(response);
        }
        public Task<InterfaceMonitoringAsientosContablesDetailError_CB_VM> ListarDetalleInterfacesAsientosContablesError(InterfaceMonitoringAsientosContablesDetailError_CB_Filter response)
        {
            return this.DataAccessQuery.ListarDetalleInterfacesAsientosContablesError(response);
        }

        // DETALLE EXACTUS
        public Task<InterfaceMonitoringExactusDetail_CB_VM> ListarDetalleInterfacesExactus(InterfaceMonitoringExactusDetailFilter response)
        {
            return this.DataAccessQuery.ListarDetalleInterfacesExactus(response);
        }
        public Task<InterfaceMonitoringExactusDetailAsiento_CB_VM> ListarDetalleInterfacesExactusAsiento(InterfaceMonitoringExactusDetailAsiento_CB_Filter response)
        {
            return this.DataAccessQuery.ListarDetalleInterfacesExactusAsiento(response);
        }
        public Task<InterfaceMonitoringExactusDetailError_CB_VM> ListarDetalleInterfacesExactusError(InterfaceMonitoringExactusDetailError_CB_Filter response)
        {
            return this.DataAccessQuery.ListarDetalleInterfacesExactusError(response);
        }

        // ÓRDENES DE PAGO
        public Task<InterfaceMonitoringAsientosContablesDetail_CB_VM_OP> ListarDetalleInterfacesAsientosContablesOP(InterfaceMonitoringAsientosContablesDetailFilter response)
        {
            return this.DataAccessQuery.ListarDetalleInterfacesAsientosContablesOP(response);
        }
        public Task<InterfaceMonitoringAsientosContablesDetailAsiento_CB_VM_OP> ListarDetalleInterfacesAsientosContablesAsientoOP(InterfaceMonitoringAsientosContablesDetailAsiento_CB_Filter response)
        {
            return this.DataAccessQuery.ListarDetalleInterfacesAsientosContablesAsientoOP(response);
        }
        public Task<InterfaceMonitoringExactusDetail_CB_VM_OP> ListarDetalleInterfacesExactusOP(InterfaceMonitoringExactusDetailFilter response)
        {
            return this.DataAccessQuery.ListarDetalleInterfacesExactusOP(response);
        }
        public Task<InterfaceMonitoringExactusDetailAsiento_CB_VM_OP> ListarDetalleInterfacesExactusAsientoOP(InterfaceMonitoringExactusDetailAsiento_CB_Filter response)
        {
            return this.DataAccessQuery.ListarDetalleInterfacesExactusAsientoOP(response);
        }
        // RENTAS
        public Task<RentasOrigenVM> ListarOrigenRentas()
        {
            return this.DataAccessQuery.ListarOrigenRentas();
        }
        public Task<RentasPagoSiniestroVM> ListarPagoSiniestro(ListarPagoSiniestroBM data)
        {
            return this.DataAccessQuery.ListarPagoSiniestro(data);
        }
        public Task<RentasAprobacionesVM> ListarAprobacionesRentas(RentasAprobacionesBM data)
        {
            return this.DataAccessQuery.ListarAprobacionesRentas(data);
        }
        public Task<ResponseVM> AprobarPagoList(RentasAprobarPagoListBM data)
        {
            return this.DataAccessQuery.AprobarPagoList(data);
        }
    }
}