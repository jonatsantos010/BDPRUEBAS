using System;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Collections.Generic;
using WSPlataforma.Core;
using WSPlataforma.Entities.ProductModel.BindingModel;
using WSPlataforma.Entities.ProductModel.ViewModel;
using WSPlataforma.Entities.LoadMassiveModel;

namespace WSPlataforma.Controllers
{
    [RoutePrefix("Api/Product")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ProductController : ApiController
    {
        [Route("ProductsByUser")]
        [HttpPost]
        public IHttpActionResult ProductsByUser(ProductByUserBM request)
        {
            ProductCore ProductCore = new ProductCore();
            List<ProductUserVM> response = new List<ProductUserVM>();

            try
            {
                response = ProductCore.ProductByUser(request);
            }
            catch (Exception ex)
            {
                response = new List<ProductUserVM>();
            }

            return Ok(response);
        }

        [Route("EpsKuntur")]
        [HttpPost]
        public IHttpActionResult EpsKuntur(ProductByUserBM request)
        {
            ProductCore ProductCore = new ProductCore();
            List<EpsVM> response = new List<EpsVM>();

            try
            {
                response = ProductCore.EpsKuntur(request);
            }
            catch (Exception ex)
            {
                response = new List<EpsVM>();
            }

            return Ok(response);
        }

        [Route("DataSctr")]
        [HttpPost]
        public IHttpActionResult DataSctr(ProductByUserBM request)
        {
            var response = new DataSctrVM();

            try
            {
                response.productUserList = new ProductCore().ProductByUser(request);
                response.epsList = new ProductCore().EpsKuntur(request);
                response.productList = new SharedCORE().GetProducts(String.Empty, String.Empty, String.Empty);
            }
            catch (Exception ex)
            {
                response.mensaje = ex.ToString();
            }

            return Ok(response);
        }
    }
}