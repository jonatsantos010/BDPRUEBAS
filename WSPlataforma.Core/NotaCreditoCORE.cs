using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.DA;
using WSPlataforma.Entities.NotasCreditoModel.BindingModel;
using WSPlataforma.Entities.NotasCreditoModel.ViewModel;

namespace WSPlataforma.Core
{
    public class NotaCreditoCORE
    {
        NotaCreditoDA DataAccessQuery = new NotaCreditoDA();
        public List<ReporteNotaCreditoVM> GetBranch()
        {
            return this.DataAccessQuery.GetBranch();
        }

        public List<ReporteNotaCreditoBM> InsertReportStatus(ReporteNotaCreditoFilter responseControl)
        {
            var rpta = this.DataAccessQuery.InsertReportStatus(responseControl);

            var rpteCierre = this.DataAccessQuery.GetReportStatus(responseControl);
            return rpteCierre;
        }

        public List<ReporteNotaCreditoBM> GetReportStatus(ReporteNotaCreditoFilter data)
        {
            return this.DataAccessQuery.GetReportStatus(data);
        }
    }
}
