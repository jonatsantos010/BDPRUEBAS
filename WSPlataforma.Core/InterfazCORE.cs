using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WSPlataforma.DA;
using WSPlataforma.Entities.InterfazModel.BindingModel;
using WSPlataforma.Entities.InterfazModel.ViewModel;

namespace WSPlataforma.Core
{
    public class InterfazCORE
    {
        InterfazDA DataAccessQuery = new InterfazDA();

        public Task<InterfazVM> ListarOrigen()
        {
            return this.DataAccessQuery.ListarOrigen();
        }

        public Task<EstadosVM> ListarEstados()
        {
            return this.DataAccessQuery.ListarEstados();
        }

        public Task<TipoAsientoVM> ListarTipoAsiento()
        {
            return this.DataAccessQuery.ListarTipoAsiento();
        }

        public Task<RespuestaVM> ConsultarTipoAsiento(TipoAsientoFilter data)
        {
            return this.DataAccessQuery.ConsultarTipoAsiento(data);
        }

        public Task<ReportesAsociadosVM> ListarReportesAsociados()
        {
            return this.DataAccessQuery.ListarReportesAsociados();
        }

        public Task<ListarInterfazVM> ListarInterfaz(InterfazFilter response)
        {
            return this.DataAccessQuery.ListarInterfaz(response);
        }

        public Task<RespuestaVM> AgregarInterfaz(InterfazFilterCrud response)
        {
            return this.DataAccessQuery.AgregarInterfaz(response);
        }

        public Task<RespuestaVM> ModificarInterfaz(InterfazFilterCrud response)
        {
            return this.DataAccessQuery.ModificarInterfaz(response);
        }

        public Task<RespuestaVM> EliminarInterfaz(InterfazFilterCrud response)
        {
            return this.DataAccessQuery.EliminarInterfaz(response);
        }

        public Task<ListarMovimientosVM> ListarMovimientos(MovimientosFilter response)
        {
            return this.DataAccessQuery.ListarMovimientos(response);
        }

        public Task<RespuestaVM> AgregarMovimientos(MovimientosFilterCrud response)
        {
            return this.DataAccessQuery.AgregarMovimientos(response);
        }

        public Task<RespuestaVM> ModificarMovimientos(MovimientosFilterCrud response)
        {
            return this.DataAccessQuery.ModificarMovimientos(response);
        }

        public Task<RespuestaVM> EliminarMovimientos(MovimientosFilterCrud response)
        {
            return this.DataAccessQuery.EliminarMovimientos(response);
        }
    }
}