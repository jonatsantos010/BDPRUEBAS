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
    [RoutePrefix("Api/Rebill")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class RebillController : ApiController
    {
        RebillCore rebillCore = new RebillCore();

        /************* LISTAR RAMO *************/
        [Route("ListarRamo")]
        [HttpGet]
        public IHttpActionResult ListarRamo()
        {
            var result = this.rebillCore.ListarRamo();
            return Ok(result);
        }

        /************* LISTAR RAMO *************/
        [Route("ListarPerfil")]
        [HttpGet]
        public IHttpActionResult ListarPerfil()
        {
            var result = this.rebillCore.ListarPerfil();
            return Ok(result);
        }

        /************* LISTAR PRODUCTO *************/
        [Route("ListarProducto")]
        [HttpPost]
        public IHttpActionResult ListarProducto(ProductoFilter data)
        {
            var response = this.rebillCore.ListarProducto(data);
            return Ok(response);
        }

        /************* LISTAR PERFILES Y DETALLES *************/
        [Route("listPerPerfil")]
        [HttpPost]
        public IHttpActionResult listPerPerfil(PerfilFilter data)
        {
            var response = this.rebillCore.listPerPerfil(data);
            return Ok(response);
        }

        /************* INSERTAR PERFILES Y DETALLES *************/
        [Route("guardar")]
        [HttpPost]
        public IHttpActionResult guardar(PerfilFilter data)
        {
            var response = this.rebillCore.guardar(data);
            return Ok(response);
        }

        /************* LISTAR PRODUCTO *************/
        [Route("permisoPerfil")]
        [HttpPost]
        public IHttpActionResult permisoPerfil(NuserCodeFilter data)
        {
            var response = this.rebillCore.permisoPerfil(data);
            return Ok(response);
        }

    }
}