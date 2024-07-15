using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WSPlataforma.Core;
using WSPlataforma.Entities.InmobiliaryInterfaceDependenceModel.BindingModel;

namespace WSPlataforma.Controllers
{
    [RoutePrefix("Api/InmoInterfaceDependence")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]

    public class InmobiliaryInterfaceDependenceController : ApiController
    {
        #region Filters
        InmobiliaryInterfaceDependenceCORE InterfaceDependenceCORE = new InmobiliaryInterfaceDependenceCORE();

        // LISTAR DEPENDENCIA DE INTERFACES
        [Route("ListarDependenciasInterfaces")]
        [HttpPost]
        public IHttpActionResult ListarDependenciasInterfaces(InterfaceDependenceFilter data)
        {
            var result = this.InterfaceDependenceCORE.ListarDependenciasInterfaces(data);
            return Ok(result);
        }

        // AGREGAR DEPENDENCIA DE INTERFACES
        [Route("AgregarDependenciasInterfaces")]
        [HttpPost]
        public IHttpActionResult AgregarDependenciasInterfaces(NewInterfaceDependenceFilter data)
        {
            var result = this.InterfaceDependenceCORE.AgregarDependenciasInterfaces(data);
            return Ok(result);
        }

        // ELIMINAR DEPENDENCIA DE INTERFACES
        [Route("EliminarDependenciasInterfaces")]
        [HttpPost]
        public IHttpActionResult EliminarDependenciasInterfaces(DeleteInterfaceDependenceFilter data)
        {
            var result = this.InterfaceDependenceCORE.EliminarDependenciasInterfaces(data);
            return Ok(result);
        }

        // LISTAR PRIORIDADES DE INTERFACES
        [Route("ListarPrioridadesInterfaces")]
        [HttpPost]
        public IHttpActionResult ListarPrioridadesInterfaces(InterfacePriorityFilter data)
        {
            var result = this.InterfaceDependenceCORE.ListarPrioridadesInterfaces(data);
            return Ok(result);
        }

        // ELIMINAR E INSERTAR NUEVAS PRIORIDADES
        [Route("EliminarAgregarPrioridadesInterfaces")]
        [HttpPost]
        public IHttpActionResult EliminarAgregarPrioridadesInterfaces(NewPrioritiesFilter data)
        {
            var result = this.InterfaceDependenceCORE.EliminarAgregarPrioridadesInterfaces(data);
            return Ok(result);
        }

        // LISTAR DEPENDENCIA DE INTERFACES FILTRADAS
        [Route("ListarDependenciasInterfacesFiltradas")]
        [HttpPost]
        public IHttpActionResult ListarDependenciasInterfacesFiltradas(InterfaceDependenceFilter data)
        {
            var result = this.InterfaceDependenceCORE.ListarDependenciasInterfacesFiltradas(data);
            return Ok(result);
        }
        #endregion
    }
}