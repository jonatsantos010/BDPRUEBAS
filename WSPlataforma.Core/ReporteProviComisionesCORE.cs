using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.DA;
using WSPlataforma.Entities.ReporteProviComisionesModel.BindingModel;
using WSPlataforma.Entities.ReporteProviComisionesModel.ViewModel;
using WSPlataforma.Entities.PrintPolicyModel.ATP;
using WSPlataforma.Entities.ReportModel.ViewModel;

namespace WSPlataforma.Core
{
    public class ReporteProviComisionesCORE
    {
        ReporteProviComisionesDA DataAccessQuery = new ReporteProviComisionesDA();
        public List<ReporteProviComisionesVM> GetBranch()
        {
            return this.DataAccessQuery.GetBranch();
        }


        public List<ReporteProviComisionesBM> InsertReportStatus(ReporteProviComisionesFilter responseControl)
        {
            var rpta = this.DataAccessQuery.InsertReportStatus(responseControl);

            var rpteProviComisiones = this.DataAccessQuery.GetReportStatus(responseControl);
            return rpteProviComisiones;

        }

        public List<ReporteProviComisionesBM> GetReportStatus(ReporteProviComisionesFilter data)
        {

            return this.DataAccessQuery.GetReportStatus(data);

        }
        public ResponseReportATPVM ReportProviComisionesById(RequestReportProviComisionesById data)
        {

            return this.DataAccessQuery.ReportProviComisionesById(data);

        }
    }
}
