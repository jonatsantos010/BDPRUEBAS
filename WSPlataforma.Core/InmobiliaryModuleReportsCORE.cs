using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.DA;
using WSPlataforma.Entities.InmobiliaryModuleReportsModel.BindingModel;

namespace WSPlataforma.Core
{
    public class InmobiliaryModuleReportsCORE
    {
        InmobiliaryModuleReportsDA DataAccessQuery = new InmobiliaryModuleReportsDA();

        public DataSet ProcessReportControlBancario(ModuleReportsFilter data)
        {
            return this.DataAccessQuery.ProcessReportControlBancario(data);
        }
        //operativo cab/det
        public DataSet ProcessReportOperaciones_CD(ModuleReportsFilter data)
        {
            return this.DataAccessQuery.ProcessReportOperativo(data);
        }
        //contable cab/det
        public DataSet ProcessReportContable_CD(ModuleReportsFilter data)
        {
            return this.DataAccessQuery.ProcessReportContabilidad(data);
        }

        public DataSet ProcessReportCuentasPorCobrar(ModuleReportsFilter data)
        {
            return this.DataAccessQuery.ProcessReportCuentasPorCobrar(data);
        }
    }
}