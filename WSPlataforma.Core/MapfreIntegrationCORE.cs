using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.DA;
using WSPlataforma.Entities.MapfreIntegrationModel;
using WSPlataforma.Entities.MapfreIntegrationModel.BindingModel;
using WSPlataforma.Entities.MapfreIntegrationModel.ViewModel;

namespace WSPlataforma.Core
{
    public class MapfreIntegrationCORE
    {
        MapfreIntegrationDA mapdreDA = new MapfreIntegrationDA();

        public async Task<UpdateRequestVM> UpdateRequest(UpdateRequestBM data)
        {
            return await mapdreDA.UpdateRequest(data);
        }

        public async Task<CancelPolicyVM> CancelPolicy(CancelPolicyBM data)
        {
            return await mapdreDA.CancelPolicy(data);
        }

        public async Task<CotizarVM> GesCotizacionMapfre(CotizarBM data)
        {
            return await mapdreDA.GesCotizacionMapfre(data);
        }

        public async Task<ValidarAseguradosVM> ValAseguradoMapfre(ValidarAseguradosBM data)
        {
            return await mapdreDA.ValAseguradoMapfre(data);
        }

        public async Task<DeclararVM> DeclararMapfre(DeclararBM data)
        {
            return await mapdreDA.DeclararMapfre(data);
        }

        public async Task<EmitirVM> EmitirMapfre(EmitirBM data)
        {
            return await mapdreDA.EmitirMapfre(data);
        }

        public async Task<CetifDocumGenVM> GesDocumentoMapfre(CetifDocumGenBM data)
        {
            return await mapdreDA.GesDocumentoMapfre(data);
        }

        public async Task<EquivalenteVM> EquivalenciaMapfre(EquivalenteBM data)
        {
            return await mapdreDA.EquivalenciaMapfre(data);
        }

        public async Task<PolizaMpVM> InsertaPolizaMapfre(PolizaMpBM data)
        {
            return await mapdreDA.InsertaPolizaMapfre(data);
        }

        public async Task<DeclararMpVM> DeclararPolizaMapfre(DeclararMpBM data)
        {
            return await mapdreDA.DeclararPolizaMapfre(data);
        }
    }
}
