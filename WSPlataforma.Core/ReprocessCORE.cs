using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.DA;
using WSPlataforma.Entities.ReprocessModel.BindingModel;
using WSPlataforma.Entities.ReprocessModel.ViewModel;

namespace WSPlataforma.Core
{
    public class ReprocessCORE
    {
        ReprocessDA DataAccessQuery = new ReprocessDA();

        public Task<ReprocessVM> InsertarReproceso(ReprocessFilter response)
        {
            return this.DataAccessQuery.InsertarReproceso(response);
        }

        public Task<ReprocessVM> InsertarReprocesoAsi(ReprocessFilter response)
        {
            return this.DataAccessQuery.InsertarReprocesoAsi(response);
        }

        public Task<ReprocessVM> InsertarReprocesoPlanilla(ReprocessFilter response)
        {
            return this.DataAccessQuery.InsertarReprocesoPlanilla(response);
        }
        // CONTROL BANCARIO
        public Task<ReprocessVM> InsertarReprocesoAsi_CBCO(ReprocessFilter response)
        {
            return this.DataAccessQuery.InsertarReprocesoAsi_CBCO(response);
        }
    }
}