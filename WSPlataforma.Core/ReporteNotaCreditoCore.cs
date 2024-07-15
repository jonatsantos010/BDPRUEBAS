/*************************************************************************************************/
/*  NOMBRE              :   ReporteNotaCreditoCore.CS                                            */
/*  DESCRIPCION         :   Capa CORE - Reporte Nota de Credito                                  */
/*  AUTOR               :   MATERIAGRIS - JOSUE CORONEL FLORES                                   */
/*  FECHA               :   08-11-2022                                                           */
/*  VERSION             :   1.0                                                                  */
/*************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.DA;
using WSPlataforma.Entities.ReporteNotaCreditoModel.ViewModel;
using WSPlataforma.Entities.ReporteNotaCreditoModel.BindingModel;

namespace WSPlataforma.Core
{
    public class ReporteNotaCreditoCore
    {
        ReporteNotaCreditoDA DataAccessQuery = new ReporteNotaCreditoDA();

        #region Filters

        public List<BranchVM> ListarRamo()
        {
            return this.DataAccessQuery.ListarRamo();
        }

        public List<ProductVM> GetProductsListByBranch(ProductoFilter data)
        {
            return DataAccessQuery.GetProductsListByBranch(data);
        }
        public List<ProductVM> ListarProducto(ProductoFilter data)
        {
            return DataAccessQuery.ListarProducto(data);
        }
        public List<TipoConsultaVM> ListarTipoConsulta()
        {
            return this.DataAccessQuery.ListarTipoConsulta();
        }
        #endregion

        #region Listado
        public List<ListarNotaCreditoVM> ListarNotaCredito(ListarNotaCreditoFilter data)
        {
            return DataAccessQuery.ListarNotaCredito(data);
        }
        #endregion
    }

}
