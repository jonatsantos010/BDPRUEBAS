using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.DA;
using WSPlataforma.Entities.InmobiliaryInterfaceDependenceModel.BindingModel;
using WSPlataforma.Entities.InmobiliaryInterfaceDependenceModel.ViewModel;

namespace WSPlataforma.Core
{
    public class InmobiliaryInterfaceDependenceCORE
    {
        InmobiliaryInterfaceDependenceDA DataAccessQuery = new InmobiliaryInterfaceDependenceDA();

        // LISTAR DEPENDENCIA DE INTERFACES
        public Task<InterfaceDependenceVM> ListarDependenciasInterfaces(InterfaceDependenceFilter response)
        {
            return this.DataAccessQuery.ListarDependenciasInterfaces(response);
        }

        // AGREGAR DEPENDENCIA DE INTERFACES
        public Task<ResponseInterfaceDependenceVM> AgregarDependenciasInterfaces(NewInterfaceDependenceFilter response)
        {
            return this.DataAccessQuery.AgregarDependenciasInterfaces(response);
        }

        // ELIMINAR DEPENDENCIA DE INTERFACES
        public Task<ResponseInterfaceDependenceVM> EliminarDependenciasInterfaces(DeleteInterfaceDependenceFilter response)
        {
            return this.DataAccessQuery.EliminarDependenciasInterfaces(response);
        }

        // LISTAR PRIORIDADES DE INTERFACES
        public Task<InterfacePriorityVM> ListarPrioridadesInterfaces(InterfacePriorityFilter response)
        {
            return this.DataAccessQuery.ListarPrioridadesInterfaces(response);
        }

        // ELIMINAR E INSERTAR NUEVAS PRIORIDADES
        public Task<ResponseInterfacePriorityVM> EliminarAgregarPrioridadesInterfaces(NewPrioritiesFilter response)
        {
            return this.DataAccessQuery.EliminarAgregarPrioridadesInterfaces(response);
        }

        // LISTAR DEPENDENCIA DE INTERFACES FILTRADAS
        public Task<InterfaceDependenceVM> ListarDependenciasInterfacesFiltradas(InterfaceDependenceFilter response)
        {
            return this.DataAccessQuery.ListarDependenciasInterfacesFiltradas(response);
        }
    }
}