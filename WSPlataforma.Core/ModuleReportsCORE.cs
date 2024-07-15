using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.DA;
using WSPlataforma.Entities.ModuleReportsModel.BindingModel;
using WSPlataforma.Entities.ModuleReportsModel.ViewModel;
using WSPlataforma.Entities.ReportModel.BindingModel;

namespace WSPlataforma.Core
{
    public class ModuleReportsCORE
    {
        ModuleReportsDA DataAccessQuery = new ModuleReportsDA();
        #region ANTIGUO
        //public ResponseControl ProcessReportContable(ModuleReportsFilter data)
        //{
        //    return this.DataAccessQuery.ProcessReportContable(data);
        //}

        //public ResponseControl ProcessReportOperaciones(ModuleReportsFilter data)
        //{
        //    return this.DataAccessQuery.ProcessReportOperaciones(data);
        //}
        #endregion 
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
        // OBTIENE LOS PRODUCTOS DE VILP
        public Task<ListProductsVILPVM> ListarProductosVILP()
        {
            return this.DataAccessQuery.ListarProductosVILP();
        }
        // obtiene reporte transferencias
        public DataSet ProcessReporTransferenciaOP(ModuleReportsFilter data)
        {
            return this.DataAccessQuery.ProcessReporTransferenciaOP(data);
        }

        // obtiene reporte cheques
        public DataSet ProcessReportChequesOP(ModuleReportsFilter data)
        {
            return this.DataAccessQuery.ProcessReportChequesOP(data);
        }


        // INI MMQ 23-01-2024
        // obtiene reporte CUentas por cobrar rentas vitalicias
        public DataSet ProcessReportCuentasPorCobrar(ModuleReportsFilter data)
        {
            return this.DataAccessQuery.ProcessReportCuentasPorCobrar(data);
        }
        // FIN MMQ 23-01-2024

    }
}