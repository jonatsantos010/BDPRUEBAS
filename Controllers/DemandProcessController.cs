using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Cors;
using WSPlataforma.Core;
using WSPlataforma.Entities.DemandProcessModel.BindingModel;
using WSPlataforma.Entities.DemandProcessModel.ViewModel;
using WSPlataforma.Entities.ViewModel;

namespace WSPlataforma.Controllers
{
    [RoutePrefix("Api/DemandProcessManager")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class DemandProcessController : ApiController
    {
        DemandProcessCORE processCORE = new DemandProcessCORE();

        [Route("SendBillsMassive")]
        [HttpGet]
        public ResponseVM SendBillsMassive()
        {
            return processCORE.SendBillsMassive();
        }

        [Route("UpdateDemandState")]
        [HttpPost]
        public ResponseVM UpdateDemandState(DemandProcessBM demandProcessBM)
        {
            return processCORE.UpdateDemandState(demandProcessBM);
        }

        [Route("ExecuteServiceSunat")]
        [HttpPost]
        public SunatResponseVM ExecuteServiceSunat(SunatRequestBM sunatSendBM)
        {
            return processCORE.ExecuteServiceSunat(sunatSendBM);
        }

        [Route("GetDemandProcessList")]
        [HttpPost]
        public List<DemandProcessVM> GetDemandProcessList(DemandProcessBM demandProcessBM)
        {
            return processCORE.GetDemandProcessList(demandProcessBM);
        }
    }
}
