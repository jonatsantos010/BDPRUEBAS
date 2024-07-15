using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.DA;
using WSPlataforma.Entities.InmobiliaryClosingDateConfigModel.BindingModel;
using WSPlataforma.Entities.InmobiliaryClosingDateConfigModel.ViewModel;

namespace WSPlataforma.Core
{
    public class InmobiliaryClosingDateConfigCORE
    {
        InmobiliaryClosingDateConfigDA DataAccessQuery = new InmobiliaryClosingDateConfigDA();

        public Task<ClosingDateConfigVM> ListarConfiguraciones(ClosingDateConfigFilter response)
        {
            return this.DataAccessQuery.ListarConfiguraciones(response);
        }

        public Task<RespuestaVM> AgregarConfiguracion(ClosingDateConfigFilterCrud response)
        {
            return this.DataAccessQuery.AgregarConfiguracion(response);
        }

        public Task<RespuestaVM> ModificarConfiguracion(ClosingDateConfigFilterCrud response)
        {
            return this.DataAccessQuery.ModificarConfiguracion(response);
        }

        public Task<RespuestaVM> EliminarConfiguracion(ClosingDateConfigFilterCrud response)
        {
            return this.DataAccessQuery.EliminarConfiguracion(response);
        }

        public Task<EstadosVM> ListarEstados()
        {
            return this.DataAccessQuery.ListarEstados();
        }

        public Task<RamosVM> ListarRamos(RamosFilter response)
        {
            return this.DataAccessQuery.ListarRamos(response);
        }

        public Task<MesesVM> ListarMeses()
        {
            return this.DataAccessQuery.ListarMeses();
        }
    }
}