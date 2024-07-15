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
    public class CoverGenCORE
    {
        CoverGenDA DataAccessQuery = new CoverGenDA();

        public List<CoverGenVM> GetCoverGenByCode(CoverGenBM coverGenBM)
        {
            return DataAccessQuery.GetCoverGenByCode(coverGenBM);
        }

        public ResponseVM UpdateStateCoverGen(CoverGenBM coverGenBM)
        {
            return DataAccessQuery.UpdateStateCoverGen(coverGenBM);
        }

        public List<CoverGenVM> GetCoverGenList(CoverGenBM coverGenBM)
        {
            return DataAccessQuery.GetCoverGenList(coverGenBM);
        }

        public ResponseVM InsertCoverGeneric(CoverGenBM coverGenBM)
        {
            return DataAccessQuery.InsertCoverGeneric(coverGenBM);
        }
    }
}
