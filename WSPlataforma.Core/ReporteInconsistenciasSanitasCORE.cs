using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.DA;
using WSPlataforma.Entities.ReporteInconsistenciasSanitasModel.ViewModel;
using WSPlataforma.Entities.ReporteInconsistenciasSanitasModel.BindingModel;
using System.Data;

namespace WSPlataforma.Core
{
    public class ReporteInconsistenciasSanitasCORE
    {
        ReporteInconsistenciasSanitasDA DataAccesQuery = new ReporteInconsistenciasSanitasDA();

        public List<RamosVM> ListarRamos()
        {
            return this.DataAccesQuery.ListarRamos();
        }

        public DataSet generarReporte(ReporteInconsistenciasFilter data)
        {
            return this.DataAccesQuery.generarReporte(data);
        }
    }
}
