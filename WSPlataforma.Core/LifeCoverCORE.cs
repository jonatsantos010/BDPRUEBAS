using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.DA;
using WSPlataforma.Entities.CoverModel.BindingModel;
using WSPlataforma.Entities.CoverModel.ViewModel;

namespace WSPlataforma.Core
{
    public class LifeCoverCORE
    {
        LifeCoverDA DataAccessQuery = new LifeCoverDA();

        public List<CoverEspVM> GetCoverList(CoverEspBM coverBM)
        {
            return DataAccessQuery.GetCoverList(coverBM);
        }
    }
}
