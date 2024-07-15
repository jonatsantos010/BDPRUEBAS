using System.Collections.Generic;
using WSPlataforma.DA;
using WSPlataforma.Entities.DemandProcessModel.BindingModel;
using WSPlataforma.Entities.DemandProcessModel.ViewModel;
using WSPlataforma.Entities.ViewModel;

namespace WSPlataforma.Core
{
    public class DemandProcessCORE
    {
        DemandProcessDA DataAccessQuery = new DemandProcessDA();

        public ResponseVM SendBillsMassive()
        {
            return DataAccessQuery.SendBillsMassive();
        }
        public ResponseVM UpdateDemandState(DemandProcessBM demandProcessBM)
        {
            return DataAccessQuery.UpdateDemandState(demandProcessBM);
        }
        public SunatResponseVM ExecuteServiceSunat(SunatRequestBM sunatSendBM)
        {
            return DataAccessQuery.ExecuteServiceSunat(sunatSendBM);
        }
        public List<DemandProcessVM> GetDemandProcessList(DemandProcessBM demandProcessBM)
        {
            return DataAccessQuery.GetDemandProcessList(demandProcessBM);
        }
    }
}
