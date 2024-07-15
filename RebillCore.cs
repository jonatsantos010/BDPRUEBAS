/*************************************************************************************************/
/*  NOMBRE              :   Refacturacaión.CS                                            */
/*  DESCRIPCION         :   Capa CORE - Reporte Nota de Credito                                  */
/*  AUTOR               :   MATERIAGRIS - FRANCISCO AQUIÑO                                  */
/*  FECHA               :   08-11-2023                                                           */
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
	public class RebillCore
	{
		Refacturacion DataAccessQuery = new Refacturacion();

		public List<BranchVM> ListarRamo()
		{
			return this.DataAccessQuery.ListarRamo();
		}
	}
}
