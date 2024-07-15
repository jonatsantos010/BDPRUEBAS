using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.DA;
using WSPlataforma.Entities.ReporteSoatModel.BindingModel;
using WSPlataforma.Entities.ReporteSoatModel.ViewModel;

namespace WSPlataforma.Core
{
    public class ReporteSoatCORE
    {
        ReporteSoatDA DataAccessQuery = new ReporteSoatDA();
        public List<ReporteSoatVM> GetBranch()
        {
            return this.DataAccessQuery.GetBranch();
        }


        public List<ReporteDetalleResumenBM> InsertReportStatus(ReporteSoatFilter responseControl)
        {
            var rpta = this.DataAccessQuery.InsertReportStatus(responseControl);

            var rpteSoat = this.DataAccessQuery.GetReportStatus(responseControl);
            return rpteSoat;

        }

        public List<ReporteDetalleResumenBM> GetReportStatus(ReporteSoatFilter data)
        {

            return this.DataAccessQuery.GetReportStatus(data);

        }
    }
}
