using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.DA;
using WSPlataforma.Entities.ProductModel.BindingModel;
using WSPlataforma.Entities.ProductModel.ViewModel;

namespace WSPlataforma.Core
{
    public class ProductCore
    {
        ProductDA ProductDA = new ProductDA();
        public List<ProductUserVM> ProductByUser(ProductByUserBM request)
        {
            return ProductDA.ProductByUser(request);
        }
        public List<EpsVM> EpsKuntur(ProductByUserBM request)
        {
            return ProductDA.EpsKuntur(request);
        }
    }
}
