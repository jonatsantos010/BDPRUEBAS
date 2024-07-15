using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.DA;
using WSPlataforma.Entities.ModuleCostCenterModel.BindingModel;
using WSPlataforma.Entities.ModuleCostCenterModel.ViewModel;

namespace WSPlataforma.Core
{
    public class ModuleCostCenterCORE
    {
        ModuleCostCenterDA DataAccessQuery = new ModuleCostCenterDA();

        // LISTAR RAMOS
        public Task<ListarRamosVM> ListarRamos()
        {
            return this.DataAccessQuery.ListarRamos();
        }

        // LISTAR RAMOS TODOS
        public Task<ListarRamosVM> listarRamosTodos()
        {
            return this.DataAccessQuery.listarRamosTodos();
        }

        // LISTAR PRODUCTOS
        public Task<ListarProductosVM> ListarProductos (ListarProductosFilter response)
        {
            return this.DataAccessQuery.ListarProductos(response);
        }

        // LISTAR TODOS PRODUCTOS
        public Task<ListarProductosVM> ListarProductosTodos(ListarProductosFilter response)
        {
            return this.DataAccessQuery.ListarProductosTodos(response);
        }

        // LISTAR CENTRO DE COSTOS
        public Task<ListarCentroCostosVM> ListarCentroCostos(ListarCentroCostosFilter response)
        {
            return this.DataAccessQuery.ListarCentroCostos(response);
        }

        // VALIDAR CENTRO DE COSTOS
        public Task<RespuestaVM> ValidarCentroCostos(AgregarCentroCostoFilter response)
        {
            return this.DataAccessQuery.ValidarCentroCostos(response);
        }

        // BUSCAR AGREGAR CENTRO DE COSTOS
        public Task<RespuestaVM> BuscarAgregarCentroCostos(AgregarCentroCostoFilter response)
        {
            return this.DataAccessQuery.BuscarAgregarCentroCostos(response);
        }

        //  AGREGAR CENTRO DE COSTOS
        public Task<RespuestaVM> AgregarCentroCostos(AgregarCentroCostoFilter response)
        {
            return this.DataAccessQuery.AgregarCentroCostos(response);
        }

        // LISTAR POLIZA CENTRO DE COSTOS
        public Task<AsignarCentroCostoVM> ListarPoliza(AsignarCentroCostoFilter response)
        {
            return this.DataAccessQuery.ListarPoliza(response);
        }

        // REGISTRAR POLIZA AL CENTRO DE COSTOS
        public Task<RespuestaVM> RegistrarPoliza(AsignarCentroCostoFilter response)
        {
            return this.DataAccessQuery.RegistrarPoliza(response);
        }
        // REGISTRAR ASIGNACION POLIZA CON CENTRO DE COSTOS
        public Task<RespuestaVM> RegistrarAsignacion(AsignarCentroCostoFilter response)
        {
            return this.DataAccessQuery.RegistrarPoliza(response);
        }
    }
}