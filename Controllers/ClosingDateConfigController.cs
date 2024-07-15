using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WSPlataforma.Core;
using WSPlataforma.Entities.ClosingDateConfigModel.BindingModel;

namespace WSPlataforma.Controllers {

    [RoutePrefix("Api/ClosingDateConfig")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]

    public class ClosingDateConfigController : ApiController {

        #region Filters
        ClosingDateConfigCORE ClosingDateConfigCORE = new ClosingDateConfigCORE();

        // LISTAR CONFIGURACIONES
        [Route("ListarConfiguraciones")]
        [HttpPost]
        public IHttpActionResult ListarConfiguraciones(ClosingDateConfigFilter data)
        {
            var result = this.ClosingDateConfigCORE.ListarConfiguraciones(data);
            return Ok(result);
        }

        // AGREGAR CONFIGURACIÓN
        [Route("AgregarConfiguracion")]
        [HttpPost]
        public IHttpActionResult AgregarConfiguracion(ClosingDateConfigFilterCrud data)
        {
            var result = this.ClosingDateConfigCORE.AgregarConfiguracion(data);
            return Ok(result);
        }

        // MODIFICAR CONFIGURACIÓN
        [Route("ModificarConfiguracion")]
        [HttpPost]
        public IHttpActionResult ModificarConfiguracion(ClosingDateConfigFilterCrud data)
        {
            var result = this.ClosingDateConfigCORE.ModificarConfiguracion(data);
            return Ok(result);
        }

        // ELIMINAR CONFIGURACIÓN
        [Route("EliminarConfiguracion")]
        [HttpPost]
        public IHttpActionResult EliminarConfiguracion(ClosingDateConfigFilterCrud data)
        {
            var result = this.ClosingDateConfigCORE.EliminarConfiguracion(data);
            return Ok(result);
        }

        // ESTADOS
        [Route("ListarEstados")]
        [HttpGet]
        public IHttpActionResult ListarEstados()
        {
            var result = this.ClosingDateConfigCORE.ListarEstados();
            return Ok(result);
        }

        // RAMOS
        [Route("ListarRamos")]
        [HttpPost]
        public IHttpActionResult ListarRamos(RamosFilter data)
        {
            var result = this.ClosingDateConfigCORE.ListarRamos(data);
            return Ok(result);
        }

        // MESES
        [Route("ListarMeses")]
        [HttpGet]
        public IHttpActionResult ListarMeses()
        {
            var result = this.ClosingDateConfigCORE.ListarMeses();
            return Ok(result);
        }
        #endregion
    }
}