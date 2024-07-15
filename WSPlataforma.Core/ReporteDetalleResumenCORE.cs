using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.DA;
using WSPlataforma.Entities.ReporteDetalleResumenModel.BindingModel;
using WSPlataforma.Entities.ReporteDetalleResumenModel.ViewModel;

namespace WSPlataforma.Core
{
    public class ReporteDetalleResumenCORE
    {
        ReporteDetalleResumenDA DataAccessQuery = new ReporteDetalleResumenDA();
        public List<ReporteDetalleResumenVM> GetBranch(string SREPORT)
        {
            return this.DataAccessQuery.GetBranch(SREPORT);
        }


        public List<ReporteDetalleResumenBM> InsertReportStatus(ReporteDetalleResumenFilter responseControl)
        {
            var rpta = this.DataAccessQuery.InsertReportStatus(responseControl);

            var rpteDetalleResumen = this.DataAccessQuery.GetReportStatus(responseControl);
            return rpteDetalleResumen;

        }

        public List<ReporteDetalleResumenBM> GetReportStatus(ReporteDetalleResumenFilter data)
        {

            return this.DataAccessQuery.GetReportStatus(data);

        }
    }
}
