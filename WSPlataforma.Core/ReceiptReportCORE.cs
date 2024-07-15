using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.DA;
using WSPlataforma.Entities.ReceiptReportModel.BindingModel;
using WSPlataforma.Entities.ReceiptReportModel.ViewModel;

namespace WSPlataforma.Core
{
    public class ReceiptReportCORE
    {
        ReceiptReportDA DataAccessQuery = new ReceiptReportDA();

        // RAMOS
        public Task<RamosVM> ListarRamos()
        {
            return this.DataAccessQuery.ListarRamos();
        }

        // EXPORTAR DATA RECIBOS
        public Task<ExportDataReceiptVM> ExportarDataRecibos(ExportDataReceiptBM data)
        {
            return this.DataAccessQuery.ExportarDataRecibos(data);
        }

        // GENERAR REPORTE RECIBOS SCTR
        public int GenerarReporteRecibosSCTR(ReceiptReportBM data)
        {
            return this.DataAccessQuery.GenerarReporteRecibosSCTR(data);
        }

        // LISTAR REPORTES CREADOS RECIBO
        public Task<ListCreatedReportsReceiptVM> ListCreatedReportsReceipt(ListCreatedReportsReceiptBM data)
        {
            return this.DataAccessQuery.ListCreatedReportsReceipt(data);
        }

        // LISTAR CABECERA REPORTES RECIBO
        public Task<ListHeaderReportsReceiptVM> ListHeaderReportsReceipt(ListHeaderReportsReceiptBM data)
        {
            return this.DataAccessQuery.ListHeaderReportsReceipt(data);
        }
    }
}