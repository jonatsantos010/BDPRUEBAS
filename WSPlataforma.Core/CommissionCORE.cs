using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.DA;
using WSPlataforma.Entities.CommissionModel.BindingModel;

namespace WSPlataforma.Core
{
    public class CommissionCORE
    {
        CommissionDA commissionDA = new CommissionDA();
        public List<BrokerCommissionBM> GetCommisionBroker(CommissionTransactionAllSearchBM search)
        {
            return commissionDA.GetCommisionBroker(search);
        }
        public List<IntermedaryBM> getListIntermedary(CommissionTransactionAllSearchBM search)
        {
            return commissionDA.getListIntermedary(search);
        }
        public GenericResponse UpdCommissionBR(CommissionBR commission)
        {
            return commissionDA.UpdCommissionBR(commission);
        }

        public List<CommissionBR> GetLastCommission(CommissionTransactionAllSearchBM search)
        {
            return commissionDA.GetLastCommission(search);
        }

        public GenericResponse InsCommissionVDP(CommissionBR commission)
        {
            return commissionDA.InsCommissionVDP(commission);
        }

        public GenericResponse UpdCommissionVDP(CommissionBR commission)
        {
            return commissionDA.UpdCommissionVDP(commission);
        }

        public ValPerfilVDP ValPerfilVDP(int nid_user)
        {
            return commissionDA.ValPerfilVDP(nid_user);
        }
    }
}
