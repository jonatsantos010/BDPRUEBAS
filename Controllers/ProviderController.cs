using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WSPlataforma.Core;
using WSPlataforma.Entities.ProviderModel.BindingModel;
using WSPlataforma.Entities.ProviderModel.ViewModel;

namespace WSPlataforma.Controllers
{
    [RoutePrefix("Api/ProviderManager")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ProviderController : ApiController
    {
        ProviderCORE providerCORE = new ProviderCORE();
        [Route("getProviderList")]
        [HttpPost]
        public ResponseProvider getProviderList(ProviderBM request)
        {
            return providerCORE.getProviderList(request);
        }

        [Route("updProvider")]
        [HttpPost]
        public ResponseProvider updProvider(ProviderUpdBM request)
        {
            return providerCORE.updProvider(request);
        }

        [Route("deltProvider")]
        [HttpPost]
        public ResponseProvider deltProvider(ProviderDelBM request)
        {
            return providerCORE.deltProvider(request);
        }

        [Route("insProvider")]
        [HttpPost]
        public ResponseProvider insProvider(ProviderInsBM request)
        {
            return providerCORE.insProvider(request);
        }

        [Route("getTypeProviderList")]
        [HttpGet]
        public List<TypeProvider> getTypeProviderList(int cod_tabla)
        {
            return providerCORE.getTypeProviderList(cod_tabla);
        }

        [Route("getTypeVoucherList")]
        [HttpGet]
        public List<TypeVoucher> getTypeVoucherList(int cod_voucher)
        {
            return providerCORE.getTypeVoucherList(cod_voucher);
        }

        [Route("getProviderListByQuotation")]
        [HttpPost]
        public ResponseProvider getProviderListByQuotation(ProviderQuotationBM request)
        {
            return providerCORE.getProviderListByQuotation(request);
        }

    }
}