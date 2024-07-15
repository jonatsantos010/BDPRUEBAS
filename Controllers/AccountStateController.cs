using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WSPlataforma.Core;
using WSPlataforma.Entities.AccountStateModel.BindingModel;
using WSPlataforma.Entities.AccountStateModel.ViewModel;

namespace WSPlataforma.Controllers
{
    [RoutePrefix("Api/AccountStateManager")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AccountStateController : ApiController
    {
        private AccountStateCORE accountStateCORE = new AccountStateCORE();

        [Route("GetQualificationTypeList")]
        [HttpGet]
        public List<QualificationVM> GetQualificationTypeList()
        {
            return accountStateCORE.GetQualificationTypeList();
        }

        [Route("GetCreditEvaluationList")]
        [HttpPost]
        public GenericPackageVM GetCreditEvaluationList(CreditEvaluationSearchBM filter)
        {
            return accountStateCORE.GetCreditEvaluationList(filter);
        }

        [Route("EvaluateClient")]
        [HttpPost]
        public GenericPackageVM EvaluateClient(CreditEvaluationBM data)
        {
            return accountStateCORE.EvaluateClient(data);
        }

        [Route("EnableClientMovement")]
        [HttpPost]
        public GenericPackageVM EnableClientMovement(ClientEnablementBM data)
        {
            return accountStateCORE.EnableClientMovement(data);
        }

        [Route("GetContractorState")]
        [HttpGet]
        public ContractorStateVM GetContractorState(string clientId)
        {
            return accountStateCORE.GetContractorState(clientId);
        }
    }
}