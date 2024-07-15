using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WSPlataforma.Core;
using WSPlataforma.Entities.ReporteNotaCreditoModel.BindingModel;

namespace WSPlataforma.Controllers
{
    [RoutePrefix("Api/ReporteRebill")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ReporteRebillController : ApiController
    {
        ReporteRebillCore ReporteRebillCore = new ReporteRebillCore();

        /************* LISTAR RAMO *************/
        [Route("listarRamo")]
        [HttpGet]
        public IHttpActionResult listarRamo()
        {
            var result = this.ReporteRebillCore.listarRamo();
            return Ok(result);
        }

        /************* LISTAR RAMO *************/
        [Route("permisoUser")]
        [HttpPost]
        public IHttpActionResult permisoUser(NuserCodeFilter data)
        {
            var result = this.ReporteRebillCore.permisoUser(data);
            return Ok(result);
        }

        /************* LISTAR RAMO *************/
        [Route("tipoDocumento")]
        [HttpGet]
        public IHttpActionResult tipoDocumento()
        {
            var result = this.ReporteRebillCore.tipoDocumento();
            return Ok(result);
        }

        /************* LISTAR PRODUCTO *************/
        [Route("listarProducto")]
        [HttpPost]
        public IHttpActionResult ListarProducto(ProductoFilter data)
        {
            var response = this.ReporteRebillCore.listarProducto(data);
            return Ok(response);
        }

        /************* LISTAR REFACTURACIONES *************/
        [Route("listarRefact")]
        [HttpPost]
        public IHttpActionResult listarRefact(ListarNotaCreditoFilter data)
        {
            var response = this.ReporteRebillCore.listarRefact(data);
            return Ok(response);
        }
    }
}