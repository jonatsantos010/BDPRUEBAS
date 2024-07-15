using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.DA;
using WSPlataforma.Entities.ProviderModel.BindingModel;
using WSPlataforma.Entities.ProviderModel.ViewModel;
using WSPlataforma.Util;
using static WSPlataforma.Entities.Graphql.CotizadorGraph;


namespace WSPlataforma.Core
{
    public class ProviderCORE
    {
        ProviderDA  providerDA = new ProviderDA();
        public ResponseProvider getProviderList(ProviderBM request)
        {
            return providerDA.getProviderList(request);
        }

        public ResponseProvider updProvider(ProviderUpdBM request)
        {
            return providerDA.updProvider(request);
        }

        public ResponseProvider deltProvider(ProviderDelBM request)
        {
            return providerDA.deltProvider(request);
        }

        public ResponseProvider insProvider(ProviderInsBM request)
        {
            return providerDA.insProvider(request);
        }

        public List<TypeProvider> getTypeProviderList(int cod_tabla)
        {
            return providerDA.getTypeProviderList(cod_tabla);
        }

        public List<TypeVoucher> getTypeVoucherList(int cod_voucher)
        {
            return providerDA.getTypeVoucherList(cod_voucher);
        }

        public ResponseProvider getProviderListByQuotation(ProviderQuotationBM request)
        {
            return providerDA.getProviderListByQuotation(request);
        }

    }
}
