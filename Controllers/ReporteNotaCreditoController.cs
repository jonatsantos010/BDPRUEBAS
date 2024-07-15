/*************************************************************************************************/
/*  NOMBRE              :   ReporteNotaCredito.CS                                                */
/*  DESCRIPCION         :   Capa Controller - Reporte Nota de Credito                                              */
/*  AUTOR               :   MATERIAGRIS - JOSUE CORONEL FLORES                                   */
/*  FECHA               :   08-11-2022                                                           */
/*  VERSION             :   1.0                                                                  */
/*************************************************************************************************/

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WSPlataforma.Entities.PreliminaryModel.BindingModel;
using WSPlataforma.Entities.ReportModel.BindingModel;
using WSPlataforma.Core;
using WSPlataforma.Entities.ReporteNotaCreditoModel.ViewModel;
using WSPlataforma.Entities.ReporteNotaCreditoModel.BindingModel;
using System.Web.Http.Cors;
using WSPlataforma.Util;

namespace WSPlataforma.Controllers
{
    [RoutePrefix("Api/ReporteNotaCredito")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ReporteNotaCreditoController : ApiController
    {
        ReporteNotaCreditoCore ReporteNotaCreditoCore = new ReporteNotaCreditoCore();

        #region Filters
        /************* LISTAR RAMO *************/
        [Route("ListarRamo")]
        [HttpGet]
        public IHttpActionResult ListarRamo()
        {
            var result = this.ReporteNotaCreditoCore.ListarRamo();
            return Ok(result);
        }

        /************* LISTAR RAMO 999 MASIVO *************/
        [Route("GetProductsListByBranch")]
        [HttpPost]
        public IHttpActionResult GetProductsListByBranch(ProductoFilter data)
        {
            var result = this.ReporteNotaCreditoCore.GetProductsListByBranch(data);
            return Ok(result);
        }

        /************* LISTAR PRODUCTO *************/
        [Route("ListarProducto")]
        [HttpPost]
        public IHttpActionResult ListarProducto(ProductoFilter data)
        {
            var response = this.ReporteNotaCreditoCore.ListarProducto(data);
            return Ok(response);
        }

        /************* LISTAR TIPO DE BUSQUEDA *************/
        [Route("ListarTipoConsulta")]
        [HttpGet]
        public IHttpActionResult ListarTipoConsulta()
        {
            var result = this.ReporteNotaCreditoCore.ListarTipoConsulta();
            return Ok(result);
        }
        #endregion

        #region Listado
        /************* LISTAR NOTA DE CREDITO *************/
        [Route("ListarNotaCredito")]
        [HttpPost]
        public IHttpActionResult ListarNotaCredito(ListarNotaCreditoFilter data)
        {
            var response = this.ReporteNotaCreditoCore.ListarNotaCredito(data);
            return Ok(response);
        }
        #endregion 
    }
}