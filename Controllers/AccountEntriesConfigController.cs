/*************************************************************************************************/
/*  NOMBRE              :   AccountEntriesConfigController.CS                                    */
/*  DESCRIPCION         :   Capa Controller                                                      */
/*  AUTOR               :   MATERIAGRIS - JOSUE CORONEL FLORES                                   */
/*  FECHA               :   15-09-2022                                                           */
/*  VERSION             :   1.0                                                                  */
/*************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WSPlataforma.Core;
using WSPlataforma.Entities.AccountEntriesConfigModel.BindingModel;

namespace WSPlataforma.Controllers
{
    [RoutePrefix("Api/AccountEntriesConfig")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]

    public class AccountEntriesConfigController : ApiController
    {
        #region Filters
        AccountEntriesConfigCORE AccountEntriesConfigCORE = new AccountEntriesConfigCORE();


        // LISTAR ESTADO DE ASIENTOS CONTABLES
        [Route("ListarEstadosAsiento")]
        [HttpGet]
        public IHttpActionResult ListarEstadosAsiento()
        {
            var result = this.AccountEntriesConfigCORE.ListarEstadosAsiento();
            return Ok(result);
        }

        // LISTAR MOVIMIENTO DE ASIENTOS CONTABLES
        [Route("ListarMovimientoAsiento")]
        [HttpPost]
        public IHttpActionResult ListarMovimientoAsiento(MovimientoFilter data)
        {
            var result = this.AccountEntriesConfigCORE.ListarMovimientoAsiento(data);
            return Ok(result);
        }

        // LISTAR MONTO ASOCIADO
        [Route("ListarMontoAsociado")]
        [HttpPost]
        public IHttpActionResult ListarMontoAsociado(DetalleDinamicaFilter data)
        {
            var result = this.AccountEntriesConfigCORE.ListarMontoAsociado(data);
            return Ok(result);
        }

        // LISTAR DETALLE ASOCIADO
        [Route("ListarDetalleAsociado")]
        [HttpPost]
        public IHttpActionResult ListarDetalleAsociado(MovimientoDinamicaFilter data)
        {
            var result = this.AccountEntriesConfigCORE.ListarDetalleAsociado(data);
            return Ok(result);
        }

        // LISTAR TIPO DINAMICA
        [Route("ListarTipoDinamica")]
        [HttpGet]
        public IHttpActionResult ListarTipoDinamica()
        {
            var result = this.AccountEntriesConfigCORE.ListarTipoDinamica();
            return Ok(result);
        }

        #endregion

        #region ASIENTOS CONTABLES

        // LISTAR CONFIGURACION DE ASIENTOS CONTABLES
        [Route("ListarConfiguracionAsientosContables")]
        [HttpPost]
        public IHttpActionResult ListarConfiguracionAsientosContables(AccountEntriesConfigFilter data)
        {
            var result = this.AccountEntriesConfigCORE.ListarConfiguracionAsientosContables(data);
            return Ok(result);
        }

        // MODIFICAR CONFIGURACIÓN DE ASIENTOS CONTABLES
        [Route("ModificarConfiguracionAsientosContables")]
        [HttpPost]
        public IHttpActionResult ModificarConfiguracionAsientosContables(AccountEntriesConfigFilterCrud data)
        {
            var result = this.AccountEntriesConfigCORE.ModificarConfiguracionAsientosContables(data);
            return Ok(result);
        }
        
        // AGREGAR CONFIGURACIÓN DE ASIENTOS CONTABLES
        [Route("AgregarConfiguracionAsientosContables")]
        [HttpPost]
        public IHttpActionResult AgregarConfiguracionAsientosContables(AccountEntriesConfigFilterCrud data)
        {
            var result = this.AccountEntriesConfigCORE.AgregarConfiguracionAsientosContables(data);
            return Ok(result);
        }

        // ELIMINAR CONFIGURACIÓN DE ASIENTOS CONTABLES
        [Route("EliminarConfiguracionAsientosContables")]
        [HttpPost]
        public IHttpActionResult EliminarConfiguracionAsientosContables(AccountEntriesConfigFilterCrud data)
        {
            var result = this.AccountEntriesConfigCORE.EliminarConfiguracionAsientosContables(data);
            return Ok(result);
        }

        #endregion

        #region DINAMICAS CONTABLES

        // LISTAR DINAMICAS CONTABLES
        [Route("ListarDinamicasContables")]
        [HttpPost]
        public IHttpActionResult ListarDinamicasContables(ListaDinamicaContableFilter data)
        {
            var result = this.AccountEntriesConfigCORE.ListarDinamicasContables(data);
            return Ok(result);
        }

        // AGREGAR DINAMICAS CONTABLES
        [Route("AgregarDinamicasContables")]
        [HttpPost]
        public IHttpActionResult AgregarDinamicasContables(MovimientoDinamicasFilter data)
        {
            var result = this.AccountEntriesConfigCORE.AgregarDinamicasContables(data);
            return Ok(result);
        }

        // ELIMINAR DINAMICA DE ASIENTOS
        [Route("EliminarDinamicaAsientos")]
        [HttpPost]
        public IHttpActionResult EliminarDinamicaAsientos(DetalleDinamicaFilter data)
        {
            var result = this.AccountEntriesConfigCORE.EliminarDinamicaAsientos(data);
            return Ok(result);
        }

        // MODIFICAR DINAMICA DE ASIENTOS
        [Route("ModificarDinamicaAsientos")]
        [HttpPost]
        public IHttpActionResult ModificarDinamicaAsientos(MovimientoDinamicasFilter data)
        {
            var result = this.AccountEntriesConfigCORE.ModificarDinamicaAsientos(data);
            return Ok(result);
        }
        #endregion

        #region DETALLE DINAMICAS CONTABLES

        // LISTAR DINAMICAS CONTABLES
        [Route("ListarDetalleDinamica")]
        [HttpPost]
        public IHttpActionResult ListarDetalleDinamica (DetalleDinamicaFilter data)
        {
            var result = this.AccountEntriesConfigCORE.ListarDetalleDinamica(data);
            return Ok(result);
        }

        // ELIMINAR DETALLE DINAMICA
        [Route("EliminarDetalleDinamica")]
        [HttpPost]
        public IHttpActionResult EliminarDetalleDinamica(DetalleDinamicaFilter data)
        {
            var result = this.AccountEntriesConfigCORE.EliminarDetalleDinamica(data);
            return Ok(result);
        }

        // AGREGAR DETALLE DINAMICAS
        [Route("AgregarDetalleDinamicas")]
        [HttpPost]
        public IHttpActionResult AgregarDetalleDinamicas(ListaDetalle[] data)
        {
            var result = this.AccountEntriesConfigCORE.AgregarDetalleDinamicas(data);
            return Ok(result);
        }

        #endregion
    }
}