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
using WSPlataforma.Entities.ViewModel;

namespace WSPlataforma.Controllers
{
    [RoutePrefix("Api/CoverManager")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CoverController : ApiController
    {
        CoverCORE coverCore = new CoverCORE();
        CoverGenCORE coverGenCore = new CoverGenCORE();

        [Route("GetCoverEspCorrelative")]
        [HttpPost]
        public CoverVM GetCoverEspCorrelative(CoverBM coverBM)
        {
            return coverCore.GetCoverEspCorrelative(coverBM);
        }

        [Route("GetCoverGenByCode")]
        [HttpPost]
        public List<CoverGenVM> GetCoverGenByCode(CoverGenBM coverGenBM)
        {
            return coverGenCore.GetCoverGenByCode(coverGenBM);
        }

        [Route("GetCoverRateByCode")]
        [HttpPost]
        public List<CoverRateVM> GetCoverRateByCode(CoverRateBM coverRateBM)
        {
            return coverCore.GetCoverRateByCode(coverRateBM);
        }

        [Route("InsertRateDetail")]
        [HttpPost]
        public ResponseVM InsertRateDetail(CoverRateBM coverRateBM)
        {
            return coverCore.InsertRateDetail(coverRateBM);
        }

        [Route("GetCoverRateList")]
        [HttpPost]
        public List<CoverRateVM> GetCoverRateList(CoverRateBM coverRateBM)
        {
            return coverCore.GetCoverRateList(coverRateBM);
        }

        [Route("GetCoverEspByCoverGen")]
        [HttpPost]
        public List<CoverEspVM> GetCoverEspByCoverGen(CoverEspBM coverBM)
        {
            return coverCore.GetCoverEspByCoverGen(coverBM);
        }

        [Route("GetCoverEspByCover")]
        [HttpPost]
        public List<CoverEspVM> GetCoverEspByCover(CoverEspBM coverBM)
        {
            return coverCore.GetCoverEspByCover(coverBM);
        }

        [Route("GetCoverEspCode")]
        [HttpPost]
        public CoverVM GetCoverEspCode(CoverBM coverBM)
        {
            return coverCore.GetCoverEspCode(coverBM);
        }

        [Route("UpdateStateCoverGen")]
        [HttpPost]
        public ResponseVM UpdateStateCoverGen(CoverGenBM coverGenBM)
        {
            return coverGenCore.UpdateStateCoverGen(coverGenBM);
        }

        [Route("GetCoverGenList")]
        [HttpPost]
        public List<CoverGenVM> GetCoverGenList(CoverGenBM coverGenBM)
        {
            return coverGenCore.GetCoverGenList(coverGenBM);
        }

        [Route("InsertCoverGeneric")]
        [HttpPost]
        public ResponseVM InsertCoverGeneric(CoverGenBM coverGenBM)
        {
            return coverGenCore.InsertCoverGeneric(coverGenBM);
        }

        [Route("InsertModuleDetail")]
        [HttpPost]
        public ResponseVM InsertModuleDetail(CoverBM coverBM)
        {
            return coverCore.InsertModuleDetail(coverBM);
        }

        [Route("GetCoverList")]
        [HttpPost]
        public List<CoverVM> GetCoverList(CoverBM coverBM)
        {
            return coverCore.GetCoverList(coverBM);
        }
    }
}
