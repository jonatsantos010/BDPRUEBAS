using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.DA;
using WSPlataforma.Entities.CoverModel.BindingModel;
using WSPlataforma.Entities.CoverModel.ViewModel;
using WSPlataforma.Entities.ViewModel;

namespace WSPlataforma.Core
{
    public class CoverCORE
    {
        CoverDA DataAccessQuery = new CoverDA();

        public CoverVM GetCoverEspCorrelative(CoverBM coverBM)
        {
            return DataAccessQuery.GetCoverEspCorrelative(coverBM);
        }

        public List<CoverRateVM> GetCoverRateByCode(CoverRateBM coverRateBM)
        {
            return DataAccessQuery.GetCoverRateByCode(coverRateBM);
        }

        public ResponseVM InsertRateDetail(CoverRateBM coverRateBM)
        {
            return DataAccessQuery.InsertRateDetail(coverRateBM);
        }

        public List<CoverRateVM> GetCoverRateList(CoverRateBM coverRateBM)
        {
            return DataAccessQuery.GetCoverRateList(coverRateBM);
        }

        public List<CoverEspVM> GetCoverEspByCoverGen(CoverEspBM coverEspBM)
        {
            return DataAccessQuery.GetCoverEspByCoverGen(coverEspBM);
        }

        public List<CoverEspVM> GetCoverEspByCover(CoverEspBM coverEspBM)
        {
            return DataAccessQuery.GetCoverEspByCover(coverEspBM);
        }

        public CoverVM GetCoverEspCode(CoverBM coverBM)
        {
            return DataAccessQuery.GetCoverEspCode(coverBM);
        }

        public ResponseVM InsertModuleDetail(CoverBM coverBM)
        {
            return DataAccessQuery.InsertModuleDetail(coverBM);
        }

        public List<CoverVM> GetCoverList(CoverBM coverBM)
        {
            return DataAccessQuery.GetCoverList(coverBM);
        }
    }
}
