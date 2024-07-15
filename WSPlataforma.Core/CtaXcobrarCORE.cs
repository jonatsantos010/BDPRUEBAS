using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.DA;
using WSPlataforma.Entities.CtaXcobrarModel.BindingModel;
using WSPlataforma.Entities.CtaXcobrarModel.ViewModel;
using WSPlataforma.Entities.ReporteCierreModel.BindingModel;

namespace WSPlataforma.Core
{
    public class CtaXcobrarCORE
    {
        CtaXcobrarDA DataAccessQuery = new CtaXcobrarDA();
        public List<CuentaXcobrarVM> GetBranch()
        {
            return this.DataAccessQuery.GetBranch();
        }

        public List<CtaXcobrarBM> InsertReportStatus(CtaXcobrarFilter responseControl)
        {
            var rpta = this.DataAccessQuery.InsertReportStatus(responseControl);

            var rptaCtaXcobrar = this.DataAccessQuery.GetReportStatus(responseControl);
            return rptaCtaXcobrar;

        }

        public List<CtaXcobrarBM> GetReportStatus(CtaXcobrarFilter data)
        {

            return this.DataAccessQuery.GetReportStatus(data);

        }
    }
}
