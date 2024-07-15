/*************************************************************************************************/
/*  NOMBRE              :   AsesoriaBrockerCORE.CS                                               */
/*  DESCRIPCION         :   Capa CORE                                                            */
/*  AUTOR               :   MATERIAGRIS - MARCOS MATEO QUIROZ                                    */
/*  FECHA               :   03-05-2021                                                           */
/*  VERSION             :   1.0 - REQ-SCTR-004 - REPORTES ASESORÍA POR BROKER                    */
/*************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.Entities.AsesoriaBrokerModel.BindingModel;
using WSPlataforma.Entities.AsesoriaBrokerModel.ViewModel;
using WSPlataforma.Entities.ReportModel.BindingModel;
using WSPlataforma.DA;

namespace WSPlataforma.Core
{
    public class AsesoriaBrokerCORE
    {
        AsesoriaBrokerDA DataAccessQuery = new AsesoriaBrokerDA();
        public List<AsesoriaBrokerVM> GetBranch()
        {
            return this.DataAccessQuery.GetBranch();
        }

        public List<ReportAsesoriaBrokerBM> InsertReportStatus(AsesoriaBrokerFilter responseControl)
        {
            var rpta = this.DataAccessQuery.InsertReportStatus(responseControl);

            var rpteBoker = this.GetReportStatus(responseControl);
            return rpteBoker;
        }

        public List<ReportAsesoriaBrokerBM> GetReportStatus(AsesoriaBrokerFilter data)
        {
            return this.DataAccessQuery.GetReportStatus(data);
        }

    }
}
