using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WSPlataforma.Core;
using WSPlataforma.Entities.ReceiptReportModel.BindingModel;

namespace WSPlataforma.Controllers
{
    [RoutePrefix("Api/ReceiptReport")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]

    public class ReceiptReportController : ApiController
    {
        ReceiptReportCORE ReceiptReportCORE = new ReceiptReportCORE();

        // RAMOS
        [Route("ListarRamos")]
        [HttpGet]
        public IHttpActionResult ListarRamos()
        {
            var result = this.ReceiptReportCORE.ListarRamos();
            return Ok(result);
        }

        // EXPORTAR DATA RECIBOS
        [Route("ExportarDataRecibos")]
        [HttpPost]
        public IHttpActionResult ExportarDataRecibos(ExportDataReceiptBM data)
        {
            var result = this.ReceiptReportCORE.ExportarDataRecibos(data);
            return Ok(result);
        }

        // GENERAR REPORTE RECIBOS SCTR
        [Route("GenerarReporteRecibosSCTR")]
        [HttpPost]
        public IHttpActionResult GenerarReporteRecibosSCTR(ReceiptReportBM data)
        {
            var result = this.ReceiptReportCORE.GenerarReporteRecibosSCTR(data);
            return Ok(result);
        }

        // LISTAR REPORTES CREADOS RECIBO
        [Route("ListCreatedReportsReceipt")]
        [HttpPost]
        public IHttpActionResult ListCreatedReportsReceipt(ListCreatedReportsReceiptBM data)
        {
            var result = this.ReceiptReportCORE.ListCreatedReportsReceipt(data);
            return Ok(result);
        }

        // LISTAR CABECERA REPORTES RECIBO
        [Route("ListHeaderReportsReceipt")]
        [HttpPost]
        public IHttpActionResult ListHeaderReportsReceipt(ListHeaderReportsReceiptBM data)
        {
            var result = this.ReceiptReportCORE.ListHeaderReportsReceipt(data);
            return Ok(result);
        }
    }
}