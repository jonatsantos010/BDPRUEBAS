
/*************************************************************************************************/
/*  NOMBRE              :   class RptCierreCORE.CS                                               */
/*  DESCRIPCION         :   Capa CORE                                                            */
/*  AUTOR               :   MATERIAGRIS - MARCOS MATEO QUIROZ                                    */
/*  FECHA               :   09-06-2021                                                           */
/*  VERSION             :   1.0 - REQ-SCTR-005 - REPORTES CIERRE PRODUCCION                      */
/*************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.DA;
using WSPlataforma.Entities.ReporteCierreModel.BindingModel;
using WSPlataforma.Entities.ReporteCierreModel.ViewModel;

namespace WSPlataforma.Core
{
    public class ReporteCierreCORE
    {
        ReporteCierreDA DataAccessQuery = new ReporteCierreDA();
        public List<ReporteCierreVM> GetBranch()
        {
            return this.DataAccessQuery.GetBranch();
        }


        public List<ReporteCierreBM> InsertReportStatus(ReporteCierreFilter responseControl)
        {
            var rpta = this.DataAccessQuery.InsertReportStatus(responseControl);

            var rpteCierre = this.DataAccessQuery.GetReportStatus(responseControl);
            return rpteCierre;

        }

        public List<ReporteCierreBM> GetReportStatus(ReporteCierreFilter data)
        {

            return this.DataAccessQuery.GetReportStatus(data);

        }
    }
}
