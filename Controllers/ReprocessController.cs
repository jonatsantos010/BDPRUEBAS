using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Diagnostics;
using WSPlataforma.Util;
using System.Web.Http;
using System.Web.Http.Cors;
using WSPlataforma.Core;
using WSPlataforma.Entities.ReprocessModel.BindingModel;

namespace WSPlataforma.Controllers
{
    [RoutePrefix("Api/Reprocess")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ReprocessController : ApiController
    {
        #region 
        ReprocessCORE ReprocessCORE = new ReprocessCORE();

        [Route("InsertarReproceso")]
        [HttpPost]
        public IHttpActionResult InsertarReproceso(ReprocessFilter data)
        {
            var result = this.ReprocessCORE.InsertarReproceso(data);
            return Ok(result);
        }

        [Route("InsertarReprocesoAsi")]
        [HttpPost]
        public IHttpActionResult InsertarReprocesoAsi(ReprocessFilter data)
        {
            var result = this.ReprocessCORE.InsertarReprocesoAsi(data);
            return Ok(result);
        }

        [Route("InsertarReprocesoPlanilla")]
        [HttpPost]
        public IHttpActionResult InsertarReprocesoPlanilla(ReprocessFilter data)
        {
            var result = this.ReprocessCORE.InsertarReprocesoPlanilla(data);
            return Ok(result);
        }
        //CONTROL BANCARIO
        // REPOCROCESO ASIENTO
        [Route("InsertarReprocesoAsi_CBCO")]
        [HttpPost]
        public IHttpActionResult InsertarReprocesoAsi_CBCO(ReprocessFilter data)
        {
            var result = this.ReprocessCORE.InsertarReprocesoAsi_CBCO(data);
            return Ok(result);
        }
        //REPOCESO EXACTUS
        #endregion
    }
}