using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Cors;
using WSPlataforma.Core;
using WSPlataforma.Entities.InterfazModel.BindingModel;

namespace WSPlataforma.Controllers
{
    [RoutePrefix("Api/Interfaz")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]

    public class InterfazController : ApiController
    {
        InterfazCORE InterfazCORE = new InterfazCORE();

        [Route("ListarOrigen")]
        [HttpGet]
        public IHttpActionResult ListarOrigen()
        {
            var result = this.InterfazCORE.ListarOrigen();
            return Ok(result);
        }

        [Route("ListarEstados")]
        [HttpGet]
        public IHttpActionResult ListarEstados()
        {
            var result = this.InterfazCORE.ListarEstados();
            return Ok(result);
        }

        [Route("ListarTipoAsiento")]
        [HttpGet]
        public IHttpActionResult ListarTipoAsiento()
        {
            var result = this.InterfazCORE.ListarTipoAsiento();
            return Ok(result);
        }

        [Route("ConsultarTipoAsiento")]
        [HttpPost]
        public IHttpActionResult ConsultarTipoAsiento(TipoAsientoFilter data)
        {
            var result = this.InterfazCORE.ConsultarTipoAsiento(data);
            return Ok(result);
        }

        [Route("ListarReportesAsociados")]
        [HttpGet]
        public IHttpActionResult ListarReportesAsociados()
        {
            var result = this.InterfazCORE.ListarReportesAsociados();
            return Ok(result);
        }

        [Route("ListarInterfaz")]
        [HttpPost]
        public IHttpActionResult ListarInterfaz(InterfazFilter data)
        {
            var result = this.InterfazCORE.ListarInterfaz(data);
            return Ok(result);
        }

        [Route("AgregarInterfaz")]
        [HttpPost]
        public IHttpActionResult AgregarInterfaz(InterfazFilterCrud data)
        {
            var result = this.InterfazCORE.AgregarInterfaz(data);
            return Ok(result);
        }

        [Route("ModificarInterfaz")]
        [HttpPost]
        public IHttpActionResult ModificarInterfaz(InterfazFilterCrud data)
        {
            var result = this.InterfazCORE.ModificarInterfaz(data);
            return Ok(result);
        }

        [Route("EliminarInterfaz")]
        [HttpPost]
        public IHttpActionResult EliminarInterfaz(InterfazFilterCrud data)
        {
            var result = this.InterfazCORE.EliminarInterfaz(data);
            return Ok(result);
        }

        [Route("ListarMovimientos")]
        [HttpPost]
        public IHttpActionResult ListarMovimientos(MovimientosFilter data)
        {
            var result = this.InterfazCORE.ListarMovimientos(data);
            return Ok(result);
        }

        [Route("AgregarMovimientos")]
        [HttpPost]
        public IHttpActionResult AgregarMovimientos(MovimientosFilterCrud data)
        {
            var result = this.InterfazCORE.AgregarMovimientos(data);
            return Ok(result);
        }

        [Route("ModificarMovimientos")]
        [HttpPost]
        public IHttpActionResult ModificarMovimientos(MovimientosFilterCrud data)
        {
            var result = this.InterfazCORE.ModificarMovimientos(data);
            return Ok(result);
        }

        [Route("EliminarMovimientos")]
        [HttpPost]
        public IHttpActionResult EliminarMovimientos(MovimientosFilterCrud data)
        {
            var result = this.InterfazCORE.EliminarMovimientos(data);
            return Ok(result);
        }
    }
}