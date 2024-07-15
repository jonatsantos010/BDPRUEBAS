/*************************************************************************************************/
/*  NOMBRE              :   NoteCreditCore.CS                                               */
/*  DESCRIPCION         :   Capa CORE                                                            */
/*  AUTOR               :   MATERIAGRIS - PEDRO ANTICONA VALLE                                   */
/*  FECHA               :   23-12-2021                                                           */
/*  VERSION             :   1.0 - Generación de NC - PD                                          */
/*************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.DA;
using WSPlataforma.Entities.DevolucionesModel.ViewModel;

namespace WSPlataforma.Core
{
    public class DevolucionesCore
    {
       DevolucionesDA DataAccessQuery = new DevolucionesDA();
        #region Filters


        
        public Task<CombosBancos> ListarBancosCajas()
        {
            return this.DataAccessQuery.ListarBancosCajas();
        }

        public Task<CombosMotivoDev> ListarMotivoDev()
        {
            return this.DataAccessQuery.ListarMotivoDev();
        }
        public Task<CombosTipoDevoluciones> ListarTipoDevoluciones()
        {
            return this.DataAccessQuery.ListarTipoDevoluciones();
        }
        public Task<CombosTipoDocDevoluciones> ListarTipoDoc()
        {
            return this.DataAccessQuery.ListarTipoDoc();
        }
        public Task<CombosDevoluciones> ListarCombosDevoluciones()
        {
            return this.DataAccessQuery.ListarCombosDevoluciones();
        }
        public Task<ListarVisualizacion> VizualizarDevoluciones(VisualizarVM data)
        {
            return this.DataAccessQuery.VizualizarDevoluciones(data);
        }
        public Task<RespuestaExactus> EnviarExactus(EnviarExactusVM data)
        {
            return this.DataAccessQuery.EnviarExactus(data);
        }
        public Task<ComboSeleccionarBanco> SeleccionarBanco(BancoProtecta data)
        {
            return this.DataAccessQuery.SeleccionarBanco(data);
        }


        //--------------Reversion----------------//

        public Task<CombosMetodosBusqueda> ListarMetodosBusqueda()
        {
            return this.DataAccessQuery.ListarMetodosBusqueda();
        }

        public Task<ListarBusquedaReversion> ListarBusquedaCombo(ReversionVM data)
        {
            return this.DataAccessQuery.ListarBusquedaCombo(data);
        }

        public Task<RespuestaCorreo> EnviarCorreoReversion(CorreoEnviarReversionVM data)
        {
            return this.DataAccessQuery.EnviarCorreoReversion(data);
        }

        //------CORREO------------//

        public Task<ListarVisualizacionCorreo> BusquedaFechasCorreo(CorreoVM data)
        {
            return this.DataAccessQuery.BusquedaFechasCorreo(data);
        }
        public Task<ListarVisualizacionCorreoHora> BusquedaHoraFechasCorreo(CorreoVM data)
        {
            return this.DataAccessQuery.BusquedaHoraFechasCorreo(data);
        }

        public Task<RespuestaCorreo> EnviarCorreo(CorreoEnviarVM data)
        {
            return this.DataAccessQuery.EnviarCorreo(data);
        }


        





        #endregion

    }

}
