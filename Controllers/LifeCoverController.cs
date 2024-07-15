using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WSPlataforma.Core;
using WSPlataforma.Entities.CoverModel.BindingModel;
using WSPlataforma.Entities.CoverModel.ViewModel;

namespace WSPlataforma.Controllers
{
    [RoutePrefix("Api/LifeCoverManager")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class LifeCoverController : ApiController
    {
        LifeCoverCORE lifeCoverCore = new LifeCoverCORE();

        [Route("GetLifeCoverList")]
        [HttpPost]
        public List<CoverEspVM> GetCoverList(CoverEspBM coverBM)
        {
            return lifeCoverCore.GetCoverList(coverBM);
        }
    }
}
