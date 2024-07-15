using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WSPlataforma.Core;
using WSPlataforma.Entities.ScheduleModel.BindingModel;

namespace WSPlataforma.Controllers
{
    [RoutePrefix("Api/Schedule")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]

    public class ScheduleController : ApiController
    {
        #region Filters
        ScheduleCORE ScheduleCORE = new ScheduleCORE();

        // LISTAR HORARIOS
        [Route("ListarHorarios")]
        [HttpGet]
        public IHttpActionResult ListarHorarios()
        {
            var result = this.ScheduleCORE.ListarHorarios();
            return Ok(result);
        }

        // MODIFICAR HORARIOS
        [Route("ModificarHorarios")]
        [HttpPost]
        public IHttpActionResult ModificarHorarios(ScheduleFilter data)
        {
            var result = this.ScheduleCORE.ModificarHorarios(data);
            return Ok(result);
        }

        // REGISTRAR HORARIOS
        [Route("RegistrarHorarios")]
        [HttpPost]
        public IHttpActionResult RegistrarHorarios(ScheduleFilter data)
        {
            var result = this.ScheduleCORE.RegistrarHorarios(data);
            return Ok(result);
        }

        // INICIAR SERVICIO
        [Route("IniciarServicio")]
        [HttpPost]
        public IHttpActionResult IniciarServicio(ServicioFilter data)
        {
            var result = this.ScheduleCORE.IniciarServicio(data);
            return Ok(result);
        }

        // DETENER SERVICIO
        [Route("DetenerServicio")]
        [HttpPost]
        public IHttpActionResult DetenerServicio(ServicioFilter data)
        {
            var result = this.ScheduleCORE.DetenerServicio(data);
            return Ok(result);
        }

        // ESTADO SERVICIO
        [Route("EstadoServicio")]
        [HttpPost]
        public IHttpActionResult EstadoServicio(ServicioFilter data)
        {
            var result = this.ScheduleCORE.EstadoServicio(data);
            return Ok(result);
        }

        // LISTAR SERVICIOS
        [Route("ListarServicios")]
        [HttpGet]
        public IHttpActionResult ListarServicios()
        {
            var result = this.ScheduleCORE.ListarServicios();
            return Ok(result);
        }
        #endregion
    }
}