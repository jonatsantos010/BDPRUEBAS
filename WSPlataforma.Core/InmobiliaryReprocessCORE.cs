using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.DA;
using WSPlataforma.Entities.InmobiliaryReprocessModel.BindingModel;
using WSPlataforma.Entities.InmobiliaryReprocessModel.ViewModel;

namespace WSPlataforma.Core
{
    public class InmobiliaryReprocessCORE
    {
        InmobiliaryReprocessDA DataAccessQuery = new InmobiliaryReprocessDA();

        public Task<ReprocessVM> InsertarReproceso(ReprocessFilter response)
        {
            return this.DataAccessQuery.InsertarReproceso(response);
        }

        public Task<ReprocessVM> InsertarReprocesoAsi(ReprocessFilter response)
        {
            return this.DataAccessQuery.InsertarReprocesoAsi(response);
        }
       
        // CONTROL BANCARIO
        public Task<ReprocessVM> InsertarReprocesoAsi_CBCO(ReprocessFilter response)
        {
            return this.DataAccessQuery.InsertarReprocesoAsi_CBCO(response);
        }
    }
}