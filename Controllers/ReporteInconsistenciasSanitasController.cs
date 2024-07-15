using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WSPlataforma.Core;
using WSPlataforma.Entities.ReporteInconsistenciasSanitasModel.BindingModel;

namespace WSPlataforma.Controllers
{
    [RoutePrefix("Api/ReporteInconsistenciasSanitas")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]

    public class ReporteInconsistenciasSanitasController : ApiController
    {
        ReporteInconsistenciasSanitasCORE InconsistenciasSanitasCORE = new ReporteInconsistenciasSanitasCORE();

        [Route("ListarRamos")]
        [HttpGet]
        public IHttpActionResult ListarRamos()
        {
            var result = this.InconsistenciasSanitasCORE.ListarRamos();
            return Ok(result);
        }

        [Route("GenerarReporte")]
        [HttpPost]
        public IHttpActionResult GenerarReporte(ReporteInconsistenciasFilter data)
        {
            var result = this.InconsistenciasSanitasCORE.generarReporte(data);
            return Ok(result);
        }
    }
}