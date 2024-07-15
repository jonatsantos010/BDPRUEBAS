using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.DA;
using WSPlataforma.Entities.AccountStateModel.BindingModel;
using WSPlataforma.Entities.AccountStateModel.ViewModel;

namespace WSPlataforma.Core
{
    public class AccountStateCORE
    {
        private AccountStateDA accountStateDA = new AccountStateDA();

        public List<QualificationVM> GetQualificationTypeList()
        {
            return accountStateDA.GetQualificationTypeList();
        }

        public GenericPackageVM GetCreditEvaluationList(CreditEvaluationSearchBM filter)
        {
            return accountStateDA.GetCreditEvaluationList(filter);
        }

        public GenericPackageVM EvaluateClient(CreditEvaluationBM data)
        {
            return accountStateDA.EvaluateClient(data);
        }
        public GenericPackageVM EnableClientMovement(ClientEnablementBM data)
        {
            return accountStateDA.EnableClientMovement(data);
        }

        public ContractorStateVM GetContractorState(string clientId)
        {
            return accountStateDA.GetContractorState(clientId);
        }
    }
}
