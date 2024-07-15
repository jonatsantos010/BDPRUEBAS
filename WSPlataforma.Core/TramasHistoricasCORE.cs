using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.DA;
using WSPlataforma.Entities.TramasHistoricasModel.BindingModel;
using WSPlataforma.Entities.TramasHistoricasModel.ViewModel;
using WSPlataforma.Entities.ReportModel.BindingModel;

namespace WSPlataforma.Core
{
    public class TramasHistoricasCORE
    {
        TramasHistoricasDA DataAccessQuery = new TramasHistoricasDA();

        //
        public Task<ProductosVM> ListarProductos()
        {
            return this.DataAccessQuery.ListarProductos();
        }
        //
        public Task<EndososVM> ListarEndosos()
        {
            return this.DataAccessQuery.ListarEndosos();
        }
        //
        public Task<CabeceraVM> ListarCabeceras(CabeceraBM data)
        {
            return this.DataAccessQuery.ListarCabeceras(data);
        }
        //
        public List<TramasHistoricasCabeceraVM> InsertReportStatus(TramasHistoricasCabeceraBM data)
        {
            return this.DataAccessQuery.InsertReportStatus(data);
        }
    }
}