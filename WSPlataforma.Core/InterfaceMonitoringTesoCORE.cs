using System.Data;
using System.Threading.Tasks;
using WSPlataforma.DA;
using WSPlataforma.Entities.InterfaceMonitoringTesoModel.BindingModel;
using WSPlataforma.Entities.InterfaceMonitoringTesoModel.ViewModel;

namespace WSPlataforma.Core
{
    public class InterfaceMonitoringTesoCORE
    {
        InterfaceMonitoringTesoDA DataAccessQuery = new InterfaceMonitoringTesoDA();

        // TESORERÍA
        public Task<BancosTesoreríaVM> ListarBancosTesoreria()
        {
            return this.DataAccessQuery.ListarBancosTesoreria();
        }
        public Task<BuscarPorVM> ListarBuscarPor()
        {
            return this.DataAccessQuery.ListarBuscarPor();
        }
        public Task<EstadosTesoreríaVM> ListarEstadosTesoreria(EstadosTesoreríaFilter data)
        {
            return this.DataAccessQuery.ListarEstadosTesoreria(data);
        }
        public Task<AprobacionesVM> ListarAprobaciones(AprobacionesFilter data)
        {
            return this.DataAccessQuery.ListarAprobaciones(data);
        }
        public Task<AprobacionesVM> ListarAprobacionesDoc(AprobacionesDocFilter data)
        {
            return this.DataAccessQuery.ListarAprobacionesDoc(data);
        }
        public Task<AprobacionesDetalleVM> ListarAprobacionesDetalle(AprobacionesDetalleFilter data)
        {
            return this.DataAccessQuery.ListarAprobacionesDetalle(data);
        }
        public Task<AprobacionesDetalleVM> ListarAprobacionesDetalleDoc(AprobacionesDetalleDocFilter data)
        {
            return this.DataAccessQuery.ListarAprobacionesDetalleDoc(data);
        }
        public Task<ResponseVM> AgregarDetalleObservacion(AgregarDetalleObservacionFilter data)
        {
            return this.DataAccessQuery.AgregarDetalleObservacion(data);
        }
        public Task<DetalleObservacionVM> ListarDetalleObservacion(ListarDetalleObservacionFilter data)
        {
            return this.DataAccessQuery.ListarDetalleObservacion(data);
        }
        public Task<ResponseVM> AprobarProcesoList(AprobarProcesoListFilter data)
        {
            return this.DataAccessQuery.AprobarProcesoList(data);
        }

        public DataSet ListarBankTesoreriaCAB_XLS(InterfaceMonitoringTesoFilter data)
        {
            return this.DataAccessQuery.ListarBankTesoreriaCAB_XLS(data);
        }

        public DataSet ListarBankTesoreriaDET_XLS(InterfaceMonitoringDetTesoFilter data)
        {
            return this.DataAccessQuery.ListarBankTesoreriaDET_XLS(data);
        }

        public Task<ResponseVM> ResolverObservacionList(ResolverObservacionListFilter data)
        {
            return this.DataAccessQuery.ResolverObservacionList(data);
        }

        public Task<HorarioVM> GetHorarioInterfaz(HorarioBM data)
        {
            return this.DataAccessQuery.GetHorarioInterfaz(data);
        }
    }
}