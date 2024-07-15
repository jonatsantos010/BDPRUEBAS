using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WSPlataforma.Core;
using WSPlataforma.Entities.ModuleCostCenterModel.BindingModel;

namespace WSPlataforma.Controllers
{
    [RoutePrefix("Api/ModuleCostCenter")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]

    public class ModuleCostCenterController : ApiController
    {
        #region Filters
        ModuleCostCenterCORE ModuleCostCenterCORE = new ModuleCostCenterCORE();

        // LISTAR RAMOS
        [Route("ListarRamos")]
        [HttpGet]
        public IHttpActionResult ListarRamos()
        {
            var result = this.ModuleCostCenterCORE.ListarRamos();
            return Ok(result);
        }

        // LISTAR RAMOS TODOS
        [Route("listarRamosTodos")]
        [HttpGet]
        public IHttpActionResult listarRamosTodos()
        {
            var result = this.ModuleCostCenterCORE.listarRamosTodos();
            return Ok(result);
        }

        // LISTAR PRODUCTOS
        [Route("ListarProductos")]
        [HttpPost]
        public IHttpActionResult ListarProductos (ListarProductosFilter data)
        {
            var result = this.ModuleCostCenterCORE.ListarProductos(data);
            return Ok(result);
        }

        // LISTAR TODOS PRODUCTOS
        [Route("ListarProductosTodos")]
        [HttpPost]
        public IHttpActionResult ListarProductosTodos(ListarProductosFilter data)
        {
            var result = this.ModuleCostCenterCORE.ListarProductosTodos(data);
            return Ok(result);
        }

        // LISTAR CENTRO DE COSTOS
        [Route("ListarCentroCostos")]
        [HttpPost]
        public IHttpActionResult ListarCentroCostos(ListarCentroCostosFilter data)
        {
            var result = this.ModuleCostCenterCORE.ListarCentroCostos(data);
            return Ok(result);
        }

        // VALIDAR CENTRO DE COSTOS
        [Route("ValidarCentroCostos")]
        [HttpPost]
        public IHttpActionResult ValidarCentroCostos(AgregarCentroCostoFilter data)
        {
            var result = this.ModuleCostCenterCORE.ValidarCentroCostos(data);
            return Ok(result);
        }

        // BUSCAR AGREGAR CENTRO DE COSTOS
        [Route("BuscarAgregarCentroCostos")]
        [HttpPost]
        public IHttpActionResult BuscarAgregarCentroCostos(AgregarCentroCostoFilter data)
        {
            var result = this.ModuleCostCenterCORE.BuscarAgregarCentroCostos(data);
            return Ok(result);
        }

        // AGREGAR CENTRO DE COSTOS
        [Route("AgregarCentroCostos")]
        [HttpPost]
        public IHttpActionResult AgregarCentroCostos(AgregarCentroCostoFilter data)
        {
            var result = this.ModuleCostCenterCORE.AgregarCentroCostos(data);
            return Ok(result);
        }

        // LISTAR POLIZA CENTRO DE COSTOS

        [Route("ListarPoliza")]
        [HttpPost]
        public IHttpActionResult ListarPoliza(AsignarCentroCostoFilter data)
        {
            var result = this.ModuleCostCenterCORE.ListarPoliza(data);
            return Ok(result);
        }

        // REGISTRAR POLIZA AL CENTRO DE COSTOS
        [Route("RegistrarPoliza")]
        [HttpPost]
        public IHttpActionResult RegistrarPoliza(AsignarCentroCostoFilter data)
        {
            var result = this.ModuleCostCenterCORE.RegistrarPoliza(data);
            return Ok(result);
        }

        // REGISTRAR ASIGNACION POLIZA CON CENTRO DE COSTOS
        [Route("RegistrarAsignacion")]
        [HttpPost]
        public IHttpActionResult RegistrarAsignacion(AsignarCentroCostoFilter data)
        {
            var result = this.ModuleCostCenterCORE.RegistrarAsignacion(data);
            return Ok(result);
        }
        #endregion

    }
}