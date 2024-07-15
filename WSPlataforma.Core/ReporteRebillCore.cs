/*************************************************************************************************/
/*  NOMBRE              :   ReporteRebillCore.cs                                                 */
/*  DESCRIPCION         :   Capa CORE - Reporte Refacturación                                    */
/*  AUTOR               :   MATERIAGRIS - Francisco Aquiño Ramirez                               */
/*  FECHA               :   08-11-2023                                                           */
/*  VERSION             :   1.0                                                                  */
/*************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.Entities.ReporteNotaCreditoModel.ViewModel;
using WSPlataforma.Entities.ReporteNotaCreditoModel.BindingModel;
using WSPlataforma.Entities.RebillModel;
using WSPlataforma.DA;

namespace WSPlataforma.Core
{
    public class ReporteRebillCore
    {
        ReporteRebillDA ReporteRebill = new ReporteRebillDA();

        public List<BranchVM> listarRamo()
        {
            return this.ReporteRebill.listarRamo();
        }
        public int permisoUser(NuserCodeFilter data)
        {
            return this.ReporteRebill.permisoUser(data);
        }

        public List<ProductVM> tipoDocumento()
        {
            return this.ReporteRebill.tipoDocumento();
        }

        public List<ProductVM> listarProducto(ProductoFilter data)
        {
            return ReporteRebill.listarProducto(data);
        }
        public List<ListarRefactVM> listarRefact(ListarNotaCreditoFilter data)
        {
            return ReporteRebill.listarRefact(data);
        }
    }
}
