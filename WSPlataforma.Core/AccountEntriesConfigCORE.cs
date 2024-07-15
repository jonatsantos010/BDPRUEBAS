using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.DA;
using WSPlataforma.Entities.AccountEntriesConfigModel.BindingModel;
using WSPlataforma.Entities.AccountEntriesConfigModel.ViewModel;

namespace WSPlataforma.Core
{
    public class AccountEntriesConfigCORE
    {
        AccountEntriesConfigDA DataAccessQuery = new AccountEntriesConfigDA();

        // LISTAR ESTADOS DE ASIENTOS CONTABLES
        public Task<EstadosVM> ListarEstadosAsiento()
        {
            return this.DataAccessQuery.ListarEstadosAsiento();
        }

        // LISTAR MOVIMIENTO DE ASIENTOS CONTABLES
        public Task<MovimientoVM> ListarMovimientoAsiento(MovimientoFilter response)
        {
            return this.DataAccessQuery.ListarMovimientoAsiento(response);
        }
        
        // LISTAR MONTO ASOCIADO
        public Task<MontoAsociadoVM> ListarMontoAsociado(DetalleDinamicaFilter response)
        {
            return this.DataAccessQuery.ListarMontoAsociado(response);
        }

        // LISTAR DETALLE ASOCIADO
        public Task<MontoAsociadoVM> ListarDetalleAsociado(MovimientoDinamicaFilter response)
        {
            return this.DataAccessQuery.ListarDetalleAsociado(response);
        }

        // LISTAR TIPO DINAMICA
        public Task<TipoDinamicaVM> ListarTipoDinamica()
        {
            return this.DataAccessQuery.ListarTipoDinamica();
        }

        /******************ASIENTOS CONTABLES*******************************************/

        // LISTAR CONFIGURACION DE ASIENTOS CONTABLES
        public Task<AccountEntriesConfigVM> ListarConfiguracionAsientosContables(AccountEntriesConfigFilter response)
        {
            return this.DataAccessQuery.ListarConfiguracionAsientosContables(response);
        }

        // MODIFICAR CONFIGURACION DE ASIENTOS CONTABLES
        public Task<RespuestaVM> ModificarConfiguracionAsientosContables(AccountEntriesConfigFilterCrud response)
        {
            return this.DataAccessQuery.ModificarConfiguracionAsientosContables(response);
        }

        // AGREGAR CONFIGURACION DE ASIENTOS CONTABLES
        public Task<RespuestaVM> AgregarConfiguracionAsientosContables(AccountEntriesConfigFilterCrud response)
        {
            return this.DataAccessQuery.AgregarConfiguracionAsientosContables(response);
        }

        // ELIMINAR CONFIGURACION DE ASIENTOS CONTABLES
        public Task<RespuestaVM> EliminarConfiguracionAsientosContables(AccountEntriesConfigFilterCrud response)
        {
            return this.DataAccessQuery.EliminarConfiguracionAsientosContables(response);
        }

        /******************DINAMICAS CONTABLES***********************/
        // LISTAR DINAMICAS CONTABLES
        public Task<ListaDinamicaContablesVM> ListarDinamicasContables(ListaDinamicaContableFilter response)
        {
            return this.DataAccessQuery.ListarDinamicasContables(response);
        }

        // AGREGAR CONFIGURACION DE ASIENTOS CONTABLES
        public Task<RespuestaVM> AgregarDinamicasContables(MovimientoDinamicasFilter response)
        {
            return this.DataAccessQuery.AgregarDinamicasContables(response);
        }

        // AGREGAR CONFIGURACION DE ASIENTOS CONTABLES
        public Task<RespuestaVM> AgregarDetalleDinamicas(ListaDetalle[] response)
        {
            return this.DataAccessQuery.AgregarDetalleDinamicas(response);
        }

        // ELIMINAR DINAMICA ASIENTOS
        public Task<RespuestaVM> EliminarDinamicaAsientos(DetalleDinamicaFilter response)
        {
            return this.DataAccessQuery.EliminarDinamicaAsientos(response);
        }

        // MODIFICAR DINAMICA ASIENTOS 
        public Task<RespuestaVM> ModificarDinamicaAsientos(MovimientoDinamicasFilter response)
        {
            return this.DataAccessQuery.ModificarDinamicaAsientos(response);
        }


        /******************DETALLE DINAMICAS CONTABLES***********************/
        // LISTAR DINAMICAS CONTABLES
        public Task<ListaDetalleDinamicaVM> ListarDetalleDinamica(DetalleDinamicaFilter response)
        {
            return this.DataAccessQuery.ListarDetalleDinamica(response);
        }

        // ELIMINAR DINAMICA ASIENTOS
        public Task<RespuestaVM> EliminarDetalleDinamica(DetalleDinamicaFilter response)
        {
            return this.DataAccessQuery.EliminarDetalleDinamica(response);
        }
    }
}