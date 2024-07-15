using System.Data;
using System.Threading.Tasks;
using WSPlataforma.DA;
using WSPlataforma.Entities.InterfaceMonitoringModel.BindingModel;
using WSPlataforma.Entities.InterfaceMonitoringModel.ViewModel;

namespace WSPlataforma.Core
{
    public class InterfaceMonitoringCORE
    {
        InterfaceMonitoringDA DataAccessQuery = new InterfaceMonitoringDA();

        // ESTADOS DE PROCESO
        public Task<EstadosVM> ListarEstados()
        {
            return this.DataAccessQuery.ListarEstados();
        }

        // CABECERA DE INTERFACES
        public Task<InterfaceMonitoringVM> ListarCabeceraInterfaces(InterfaceMonitoringFilter response)
        {
            return this.DataAccessQuery.ListarCabeceraInterfaces(response);
        }
        public Task<InterfaceMonitoringReciboVM> ListarCabeceraInterfacesRecibo(InterfaceMonitoringReciboFilter response)
        {
            return this.DataAccessQuery.ListarCabeceraInterfacesRecibo(response);
        }
        public Task<InterfaceMonitoringSiniestrosVM> ListarCabeceraInterfacesSiniestros(InterfaceMonitoringSiniestrosFilter response)
        {
            return this.DataAccessQuery.ListarCabeceraInterfacesSiniestros(response);
        }
        public Task<InterfaceMonitoringPolizaVM> ListarCabeceraInterfacesPoliza(InterfaceMonitoringPolizaFilter response)
        {
            return this.DataAccessQuery.ListarCabeceraInterfacesPoliza(response);
        }

        // DETALLE PRELIMINAR
        public Task<InterfaceMonitoringDetailVM> ListarDetalleInterfaces(InterfaceMonitoringDetailFilter response)
        {
            return this.DataAccessQuery.ListarDetalleInterfaces(response);
        }
        public Task<InterfaceMonitoringDetailErrorVM> ListarErroresDetalle(InterfaceMonitoringDetailErrorFilter response)
        {
            return this.DataAccessQuery.ListarErroresDetalle(response);
        }

        // DETALLE ASIENTOS
        public Task<InterfaceMonitoringAsientosContablesDetailVM> ListarDetalleInterfacesAsientosContables(InterfaceMonitoringAsientosContablesDetailFilter response)
        {
            return this.DataAccessQuery.ListarDetalleInterfacesAsientosContables(response);
        }
        public Task<InterfaceMonitoringAsientosContablesDetailAsientoVM> ListarDetalleInterfacesAsientosContablesAsiento(InterfaceMonitoringAsientosContablesDetailAsientoFilter response)
        {
            return this.DataAccessQuery.ListarDetalleInterfacesAsientosContablesAsiento(response);
        }
        public Task<InterfaceMonitoringAsientosContablesDetailErrorVM> ListarDetalleInterfacesAsientosContablesError(InterfaceMonitoringAsientosContablesDetailErrorFilter response)
        {
            return this.DataAccessQuery.ListarDetalleInterfacesAsientosContablesError(response);
        }

        // DETALLE EXACTUS
        public Task<InterfaceMonitoringExactusDetailVM> ListarDetalleInterfacesExactus(InterfaceMonitoringExactusDetailFilter response)
        {
            return this.DataAccessQuery.ListarDetalleInterfacesExactus(response);
        }
        public Task<InterfaceMonitoringExactusDetailAsientoVM> ListarDetalleInterfacesExactusAsiento(InterfaceMonitoringExactusDetailAsientoFilter response)
        {
            return this.DataAccessQuery.ListarDetalleInterfacesExactusAsiento(response);
        }
        public Task<InterfaceMonitoringExactusDetailErrorVM> ListarDetalleInterfacesExactusError(InterfaceMonitoringExactusDetailErrorFilter response)
        {
            return this.DataAccessQuery.ListarDetalleInterfacesExactusError(response);
        }

        // DETALLE PLANILLA
        public Task<InterfaceMonitoringPlanillaMasivosVM> ListarDetalleErroresPlanillaMasivos(InterfaceMonitoringPlanillaMasivosFilter response)
        {
            return this.DataAccessQuery.ListarDetalleErroresPlanillaMasivos(response);
        }

        // TIPO BUSQUEDA
        public Task<TipoBusquedaVM> ListarTipoBusqueda()
        {
            return this.DataAccessQuery.ListarTipoBusqueda();
        }
        public Task<TipoBusquedaVM> ListarTipoBusquedaSI(TipoBusquedaFilter data)
        {
            return this.DataAccessQuery.ListarTipoBusquedaSI(data);
        }

        // DETALLE OPERACIONES
        public Task<InterfacegDetailOperationVM> ListarDetalleOperacion(InterfaceMonitoringDetailOperacionFilter response)
        {
            return this.DataAccessQuery.ListarDetalleOperacion(response);
        }

        // REPORTES PRELIMINARES
        public DataSet ListarDetalleInterfacesXLSX(InterfaceMonitoringDetailXLSXFilter response)
        {
            return this.DataAccessQuery.ListarDetalleInterfacesXLSX(response);
        }
        public DataSet ListarDetalleInterfacesXLSX_CB(InterfaceMonitoringDetailXLSXFilter_CB data)
        {
            return this.DataAccessQuery.ListarDetalleInterfacesXLSX_CB(data);
        }       
    }
}