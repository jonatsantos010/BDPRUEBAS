using System.Data;
using System.Threading.Tasks;
using WSPlataforma.DA;
using WSPlataforma.Entities.InmobiliaryInterfaceMonitoringModel.BindingModel;
using WSPlataforma.Entities.InmobiliaryInterfaceMonitoringModel.ViewModel;

namespace WSPlataforma.Core
{
    public class InmobiliaryInterfaceMonitoringCORE
    {
        InmobiliaryInterfaceMonitoringDA DataAccessQuery = new InmobiliaryInterfaceMonitoringDA();

        // ESTADOS DE PROCESO
        public Task<EstadosVM> ListarEstados(ProcFilter data)
        {
            return this.DataAccessQuery.ListarEstados(data);
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