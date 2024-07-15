using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.DataAccess.Client;
using System.Data;
using WSPlataforma.Util;
using WSPlataforma.Entities.AccountEntriesConfigModel.ViewModel;
using WSPlataforma.Entities.AccountEntriesConfigModel.BindingModel;


namespace WSPlataforma.DA
{
    public class AccountEntriesConfigDA : ConnectionBase
    {

        private readonly string Package = "PKG_CONFIG_ASIENTO";


        // LISTA DE ESTADO DE LOS ASIENTOS
        public Task<EstadosVM> ListarEstadosAsiento()
        {
            var parameters = new List<OracleParameter>();
            EstadosVM entities = new EstadosVM();
            entities.combos = new List<EstadosVM.P_TABLE>();
            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_T_NESTADO_ASI");
            try
            {
                parameters.Add(new OracleParameter("P_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureOnlineDB(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        EstadosVM.P_TABLE suggest = new EstadosVM.P_TABLE();
                        suggest.NCODIGO = reader["NCODIGO"] == DBNull.Value ? string.Empty : reader["NCODIGO"].ToString();
                        suggest.SSTATE = reader["SSTATE"] == DBNull.Value ? string.Empty : reader["SSTATE"].ToString();
                        entities.combos.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarEstadosAsiento", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<EstadosVM>(entities);
        }

        // LISTA DE MOVIMIENTOS DE LOS ASIENTOS
        public Task<MovimientoVM> ListarMovimientoAsiento(MovimientoFilter data)
        {
            var parameters = new List<OracleParameter>();

            MovimientoVM entities = new MovimientoVM();
            entities.lista = new List<MovimientoVM.P_TABLE>();

            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_T_NMOV_ASI");

            try
            {
                parameters.Add(new OracleParameter("P_NNUMORI", OracleDbType.Int16, data.NNUMORI, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCODGRU", OracleDbType.Int16, data.NCODGRU, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureOnlineDB(procedure, parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        MovimientoVM.P_TABLE suggest = new MovimientoVM.P_TABLE();
                        suggest.NNUMORI = reader["MCT_NNUMORI"] == DBNull.Value ? 0 : int.Parse(reader["MCT_NNUMORI"].ToString());
                        suggest.NCODGRU = reader["MCT_NCODGRU"] == DBNull.Value ? 0 : int.Parse(reader["MCT_NCODGRU"].ToString());
                        suggest.NCODCON = reader["MCT_NCODCON"] == DBNull.Value ? 0 : int.Parse(reader["MCT_NCODCON"].ToString());
                        suggest.CDESCRI = reader["MCT_CDESCRI"] == DBNull.Value ? string.Empty : reader["MCT_CDESCRI"].ToString();
                        suggest.VESTADO = reader["MCT_VESTADO"] == DBNull.Value ? 0 : int.Parse(reader["MCT_VESTADO"].ToString());

                        entities.lista.Add(suggest);
                    }
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarMovimientoAsiento", ex.ToString(), "3");
                throw;
            }

            return Task.FromResult<MovimientoVM>(entities);
        }

        // LISTA DE MONTO ASOCIADO
        public Task<MontoAsociadoVM> ListarMontoAsociado(DetalleDinamicaFilter data)
        {
            var parameters = new List<OracleParameter>();

            MontoAsociadoVM entities = new MontoAsociadoVM();
            entities.lista = new List<MontoAsociadoVM.P_TABLE>();

            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_S_MONASO_LST");

            try
            {
                parameters.Add(new OracleParameter("P_NNUMORI", OracleDbType.Int16, data.NNUMORI, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCODGRU", OracleDbType.Int16, data.NCODGRU, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCONCEPTO", OracleDbType.Int16, data.NCODCON, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureOnlineDB(procedure, parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        MontoAsociadoVM.P_TABLE suggest = new MontoAsociadoVM.P_TABLE();
                        suggest.AGD_NCODEST = reader["AGD_NCODEST"] == DBNull.Value ? 0 : int.Parse(reader["AGD_NCODEST"].ToString());
                        suggest.EST_CDESCRI = reader["EST_CDESCRI"] == DBNull.Value ? string.Empty : reader["EST_CDESCRI"].ToString();

                        entities.lista.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarMontoAsociado", ex.ToString(), "3");
                throw;
            }

            return Task.FromResult<MontoAsociadoVM>(entities);
        }
        // LISTA DE DETALLE ASOCIADO
        public Task<MontoAsociadoVM> ListarDetalleAsociado(MovimientoDinamicaFilter data)
        {
            var parameters = new List<OracleParameter>();

            MontoAsociadoVM entities = new MontoAsociadoVM();
            entities.lista = new List<MontoAsociadoVM.P_TABLE>();

            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_S_DETASO_LST");

            try
            {
                parameters.Add(new OracleParameter("P_NNUMORI", OracleDbType.Int16, data.NNUMORI, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCODGRU", OracleDbType.Int16, data.NCODGRU, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCONCEPTO", OracleDbType.Int16, data.NCODCON, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureOnlineDB(procedure, parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        MontoAsociadoVM.P_TABLE suggest = new MontoAsociadoVM.P_TABLE();
                        suggest.AGD_NCODEST = reader["AGD_NCODEST"] == DBNull.Value ? 0 : int.Parse(reader["AGD_NCODEST"].ToString());
                        suggest.EST_CDESCRI = reader["EST_CDESCRI"] == DBNull.Value ? string.Empty : reader["EST_CDESCRI"].ToString();

                        entities.lista.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarDetalleAsociado", ex.ToString(), "3");
                throw;
            }

            return Task.FromResult<MontoAsociadoVM>(entities);
        }

        // LISTA DE TIPO DINAMICA
        public Task<TipoDinamicaVM> ListarTipoDinamica()
        {
            var parameters = new List<OracleParameter>();
            TipoDinamicaVM entities = new TipoDinamicaVM();
            entities.combos = new List<TipoDinamicaVM.P_TABLE>();
            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_S_DEBCRE_LST");
            try
            {
                parameters.Add(new OracleParameter("P_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureOnlineDB(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        TipoDinamicaVM.P_TABLE suggest = new TipoDinamicaVM.P_TABLE();
                        suggest.CVALDES = reader["CVALDES"] == DBNull.Value ? string.Empty : reader["CVALDES"].ToString();
                        suggest.CDESCRI = reader["CDESCRI"] == DBNull.Value ? string.Empty : reader["CDESCRI"].ToString();
                        entities.combos.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarTipoDinamica", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<TipoDinamicaVM>(entities);
        }

        /****************ASIENTOS CONTABLES*********************/

        // MOSTRAR LISTADO CONFIGURACIÓN DE ASIENTOS CONTABLES
        public Task<AccountEntriesConfigVM> ListarConfiguracionAsientosContables(AccountEntriesConfigFilter data)
        {
            var parameters = new List<OracleParameter>();

            AccountEntriesConfigVM entities = new AccountEntriesConfigVM();
            entities.lista = new List<AccountEntriesConfigVM.P_TABLE>();

            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_S_ASITIPOT_LST");

            try
            {
                parameters.Add(new OracleParameter("P_NNUMORI", OracleDbType.Int16, data.AST_NNUMORI, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCODGRU", OracleDbType.Int16, data.AST_NCODGRU, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureOnlineDB(procedure, parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        AccountEntriesConfigVM.P_TABLE suggest = new AccountEntriesConfigVM.P_TABLE();
                        suggest.AST_NNUMORI = reader["AST_NNUMORI"] == DBNull.Value ? 0 : int.Parse(reader["AST_NNUMORI"].ToString());
                        suggest.AST_NCODGRU = reader["AST_NCODGRU"] == DBNull.Value ? 0 : int.Parse(reader["AST_NCODGRU"].ToString());
                        suggest.AST_NNUMASI = reader["AST_NNUMASI"] == DBNull.Value ? 0 : int.Parse(reader["AST_NNUMASI"].ToString());
                        suggest.AST_NCODCON = reader["AST_NCODCON"] == DBNull.Value ? 0 : int.Parse(reader["AST_NCODCON"].ToString());
                        suggest.AST_CDESCRI = reader["AST_CDESCRI"] == DBNull.Value ? string.Empty : reader["AST_CDESCRI"].ToString();
                        suggest.AST_VESTADO = reader["AST_VESTADO"] == DBNull.Value ? string.Empty : reader["AST_VESTADO"].ToString();
                        suggest.AST_CUSUCRE = reader["AST_CUSUCRE"] == DBNull.Value ? string.Empty : reader["AST_CUSUCRE"].ToString();
                        suggest.AST_DFECREG = reader["AST_DFECREG"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["AST_DFECREG"].ToString());
                        suggest.AST_CUSUMOD = reader["AST_CUSUMOD"] == DBNull.Value ? string.Empty : reader["AST_CUSUMOD"].ToString();
                        suggest.AST_DFECMOD = reader["AST_DFECMOD"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["AST_DFECMOD"].ToString());
                        suggest.MCT_CDESCRI = reader["MCT_CDESCRI"] == DBNull.Value ? string.Empty : reader["MCT_CDESCRI"].ToString();
                        suggest.DESC_VESTADO = reader["DESC_VESTADO"] == DBNull.Value ? string.Empty : reader["DESC_VESTADO"].ToString();

                        entities.lista.Add(suggest);
                    }
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarConfiguracionAsientosContables", ex.ToString(), "3");
                throw;
            }

            return Task.FromResult<AccountEntriesConfigVM>(entities);
        }

        // AGREGAR CONFIGURACIÓN DE ASIENTOS CONTABLES
        public Task<RespuestaVM> AgregarConfiguracionAsientosContables(AccountEntriesConfigFilterCrud data)
        {
            var parameters = new List<OracleParameter>();
            RespuestaVM entities = new RespuestaVM();
            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_I_ASITIPOT");
            try
            {
                parameters.Add(new OracleParameter("P_NNUMORI", OracleDbType.Int16, data.NNUMORI, ParameterDirection.Input)); // NUMERO DE ORIGEN
                parameters.Add(new OracleParameter("P_NCODGRU", OracleDbType.Int16, data.NCODGRU, ParameterDirection.Input)); // NUMERO DE INTERFACE
                parameters.Add(new OracleParameter("P_NCODCON", OracleDbType.Int16, data.NCODCON, ParameterDirection.Input)); // NUMERO DE MOVIMIENTO
                parameters.Add(new OracleParameter("P_CDESCRI", OracleDbType.NVarchar2, data.CDESCRI, ParameterDirection.Input)); // DESCRIPCION
                parameters.Add(new OracleParameter("P_VESTADO", OracleDbType.Int16, data.VESTADO, ParameterDirection.Input)); // ESTADO
                parameters.Add(new OracleParameter("P_CUSUMOD", OracleDbType.NVarchar2, data.CUSUMOD, ParameterDirection.Input)); // DESCRIPCION MOVIMIENTO

                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureOnlineDB(procedure, parameters);

                //this.ExecuteByStoredProcedureVT(string.Format("{0}.{1}", ProcedureName.pkg_PreliminarSCTR, procedure), parameters);

                entities.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                entities.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("AgregarConfiguracionAsientosContables", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<RespuestaVM>(entities);
        }

        // MODIFICAR CONFIGURACIÓN DE ASIENTOS CONTABLES
        public Task<RespuestaVM> ModificarConfiguracionAsientosContables(AccountEntriesConfigFilterCrud data)
        {
            var parameters = new List<OracleParameter>();
            RespuestaVM entities = new RespuestaVM();
            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_U_ASITIPOT");
            try
            {
                parameters.Add(new OracleParameter("P_NNUMORI", OracleDbType.Int16, data.NNUMORI, ParameterDirection.Input)); // NUMERO DE ORIGEN
                parameters.Add(new OracleParameter("P_NCODGRU", OracleDbType.Int16, data.NCODGRU, ParameterDirection.Input)); // NUMERO DE INTERFACE
                parameters.Add(new OracleParameter("P_NNUMASI", OracleDbType.Int16, data.NNUMASI, ParameterDirection.Input)); // NUMERO DE ASIENTO
                parameters.Add(new OracleParameter("P_NCODCON", OracleDbType.Int16, data.NCODCON, ParameterDirection.Input)); // NUMERO DE MOVIMIENTO
                parameters.Add(new OracleParameter("P_CDESCRI", OracleDbType.NVarchar2, data.CDESCRI, ParameterDirection.Input)); // DESCRIPCION DEL ASIENTO
                parameters.Add(new OracleParameter("P_VESTADO", OracleDbType.Int16, data.VESTADO, ParameterDirection.Input)); // ESTADO
                parameters.Add(new OracleParameter("P_CUSUMOD", OracleDbType.NVarchar2, data.CUSUMOD, ParameterDirection.Input)); // DESCRIPCION MOVIMIENTO

                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureOnlineDB(procedure, parameters);
                entities.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                entities.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("ModificarConfiguracionAsientosContables", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<RespuestaVM>(entities);
        }

        // ELIMINAR CONFIGURACIÓN DE ASIENTOS CONTABLES
        public Task<RespuestaVM> EliminarConfiguracionAsientosContables(AccountEntriesConfigFilterCrud data)
        {
            var parameters = new List<OracleParameter>();
            RespuestaVM entities = new RespuestaVM();
            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_D_ASITIPOT");
            try
            {
                parameters.Add(new OracleParameter("P_NNUMORI", OracleDbType.Int16, data.NNUMORI, ParameterDirection.Input)); // NUMERO DE ORIGEN
                parameters.Add(new OracleParameter("P_NCODGRU", OracleDbType.Int16, data.NCODGRU, ParameterDirection.Input)); // NUMERO DE INTERFACE
                parameters.Add(new OracleParameter("P_NNUMASI", OracleDbType.Int16, data.NNUMASI, ParameterDirection.Input)); // NUMERO DE ASIENTO
                parameters.Add(new OracleParameter("P_NCODCON", OracleDbType.Int16, data.NCODCON, ParameterDirection.Input)); // NUMERO DE MOVIMIENTO
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureOnlineDB(procedure, parameters);
                entities.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                entities.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("EliminarConfiguracionAsientosContables", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<RespuestaVM>(entities);
        }

        /*****************DINAMICAS CONTABLES********************/
        // LISTAR DINAMICA CONTABLE 
        public Task<ListaDinamicaContablesVM> ListarDinamicasContables(ListaDinamicaContableFilter data)
        {
            var parameters = new List<OracleParameter>();

            ListaDinamicaContablesVM entities = new ListaDinamicaContablesVM();
            entities.lista = new List<ListaDinamicaContablesVM.P_TABLE>();

            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_S_DINAMICAS_LST");

            try
            {
                parameters.Add(new OracleParameter("P_NNUMORI", OracleDbType.Int16, data.AST_NNUMORI, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCODGRU", OracleDbType.Int16, data.AST_NCODGRU, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCONCEPTO", OracleDbType.Int16, data.AST_NCODCON, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NNUMASI", OracleDbType.Int16, data.AST_NNUMASI, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureOnlineDB(procedure, parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ListaDinamicaContablesVM.P_TABLE suggest = new ListaDinamicaContablesVM.P_TABLE();
                        suggest.DIN_NNUMORI = reader["DIN_NNUMORI"] == DBNull.Value ? 0 : int.Parse(reader["DIN_NNUMORI"].ToString());
                        suggest.DIN_NCODGRU = reader["DIN_NCODGRU"] == DBNull.Value ? 0 : int.Parse(reader["DIN_NCODGRU"].ToString());
                        suggest.DIN_NCONCEPTO = reader["DIN_NCONCEPTO"] == DBNull.Value ? 0 : int.Parse(reader["DIN_NCONCEPTO"].ToString());
                        suggest.DIN_NNUMASI = reader["DIN_NNUMASI"] == DBNull.Value ? 0 : int.Parse(reader["DIN_NNUMASI"].ToString());
                        suggest.DIN_NDETASO = reader["DIN_NDETASO"] == DBNull.Value ? 0 : int.Parse(reader["DIN_NDETASO"].ToString());
                        suggest.DIN_CDESCRI = reader["DIN_CDESCRI"] == DBNull.Value ? string.Empty : reader["DIN_CDESCRI"].ToString();
                        suggest.DIN_CNUMCTA = reader["DIN_CNUMCTA"] == DBNull.Value ? string.Empty : reader["DIN_CNUMCTA"].ToString();
                        suggest.DIN_CCENCOS = reader["DIN_CCENCOS"] == DBNull.Value ? string.Empty : reader["DIN_CCENCOS"].ToString();
                        suggest.DIN_VDEBCRE = reader["DIN_VDEBCRE"] == DBNull.Value ? string.Empty : reader["DIN_VDEBCRE"].ToString();
                        suggest.DIN_NMONASO = reader["DIN_NMONASO"] == DBNull.Value ? 0 : int.Parse(reader["DIN_NMONASO"].ToString());
                        suggest.DIN_DFECREG = reader["DIN_DFECREG"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["DIN_DFECREG"].ToString());
                        suggest.DIN_CUSUCRE = reader["DIN_CUSUCRE"] == DBNull.Value ? string.Empty : reader["DIN_CUSUCRE"].ToString();
                        suggest.DIN_CUSUMOD = reader["DIN_CUSUMOD"] == DBNull.Value ? string.Empty : reader["DIN_CUSUMOD"].ToString();
                        suggest.DIN_DFECMOD = reader["DIN_DFECMOD"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["DIN_DFECMOD"].ToString());
                        suggest.DESC_VDEBCRE = reader["DESC_VDEBCRE"] == DBNull.Value ? string.Empty : reader["DESC_VDEBCRE"].ToString();
                        suggest.FLG_CCENCOS = reader["FLG_CCENCOS"] == DBNull.Value ? 0 : int.Parse(reader["FLG_CCENCOS"].ToString());
                        suggest.EST_CDESCRI = reader["EST_CDESCRI"] == DBNull.Value ? string.Empty : reader["EST_CDESCRI"].ToString();

                        entities.lista.Add(suggest);
                    }
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarDinamicasContables", ex.ToString(), "3");
                throw;
            }

            return Task.FromResult<ListaDinamicaContablesVM>(entities);
        }

        // AGREGAR DINAMICA CONTABLE 
        public Task<RespuestaVM> AgregarDinamicasContables(MovimientoDinamicasFilter data)
        {
            var parameters = new List<OracleParameter>();
            RespuestaVM entities = new RespuestaVM();
            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_I_DINAMICAS");
            try
            {
                parameters.Add(new OracleParameter("P_NNUMORI", OracleDbType.Int16, data.NNUMORI, ParameterDirection.Input)); // NUMERO DE ORIGEN
                parameters.Add(new OracleParameter("P_NCODGRU", OracleDbType.Int16, data.NCODGRU, ParameterDirection.Input)); // NUMERO DE ORIGEN
                parameters.Add(new OracleParameter("P_NCONCEPTO", OracleDbType.Int16, data.NCODCON, ParameterDirection.Input)); // NUMERO DE ORIGEN
                parameters.Add(new OracleParameter("P_NNUMASI", OracleDbType.Int16, data.NNUMASI, ParameterDirection.Input)); // NUMERO DE ORIGEN
                parameters.Add(new OracleParameter("P_NDETASO", OracleDbType.Int16, data.NDETASO, ParameterDirection.Input)); // NUMERO DE ORIGEN
                parameters.Add(new OracleParameter("P_CDESCRI", OracleDbType.NVarchar2, data.CDESCRI, ParameterDirection.Input)); // DESCRIPCION
                parameters.Add(new OracleParameter("P_CNUMCTA", OracleDbType.NVarchar2, data.CNUMCTA, ParameterDirection.Input)); // DESCRIPCION
                parameters.Add(new OracleParameter("P_FLGCCOS", OracleDbType.Int16, data.FLGCCOS, ParameterDirection.Input)); // NUMERO DE ORIGEN
                parameters.Add(new OracleParameter("P_VDEBCRE", OracleDbType.NVarchar2, data.VDEBCRE, ParameterDirection.Input)); // DESCRIPCION
                parameters.Add(new OracleParameter("P_NMONASO", OracleDbType.Int16, data.NMONASO, ParameterDirection.Input)); // NUMERO DE ORIGEN
                parameters.Add(new OracleParameter("P_CUSUMOD", OracleDbType.NVarchar2, data.CUSUMOD, ParameterDirection.Input)); // DESCRIPCION

                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureOnlineDB(procedure, parameters);

                //this.ExecuteByStoredProcedureVT(string.Format("{0}.{1}", ProcedureName.pkg_PreliminarSCTR, procedure), parameters);

                entities.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                entities.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                LogControl.save("AgregarDinamicasContables", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<RespuestaVM>(entities);
        }

        // ELIMINAR DINAMICA DE ASIENTOS 
        public Task<RespuestaVM> EliminarDinamicaAsientos(DetalleDinamicaFilter data)
        {
            var parameters = new List<OracleParameter>();
            RespuestaVM entities = new RespuestaVM();
            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_D_DINAMICAS");
            try
            {
                parameters.Add(new OracleParameter("P_NNUMORI", OracleDbType.Int16, data.NNUMORI, ParameterDirection.Input)); // NUMERO DE ORIGEN
                parameters.Add(new OracleParameter("P_NCODGRU", OracleDbType.Int16, data.NCODGRU, ParameterDirection.Input)); // NUMERO DE INTERFACE
                parameters.Add(new OracleParameter("P_NCONCEPTO", OracleDbType.Int16, data.NCODCON, ParameterDirection.Input)); // NUMERO DE MOVIMIENTO
                parameters.Add(new OracleParameter("P_NNUMASI", OracleDbType.Int16, data.NNUMASI, ParameterDirection.Input)); // NUMERO DE ASIENTO
                parameters.Add(new OracleParameter("P_NDETASO", OracleDbType.Int16, data.NDETASO, ParameterDirection.Input)); // NUMERO DE ASIENTO

                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureOnlineDB(procedure, parameters);
                entities.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                entities.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("EliminarDinamicaAsientos", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<RespuestaVM>(entities);
        }

        // MODIFICAR DINAMICA ASIENTOS 
        public Task<RespuestaVM> ModificarDinamicaAsientos(MovimientoDinamicasFilter data)
        {
            var parameters = new List<OracleParameter>();
            RespuestaVM entities = new RespuestaVM();
            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_U_DINAMICAS");
            try
            {
                parameters.Add(new OracleParameter("P_NNUMORI", OracleDbType.Int16, data.NNUMORI, ParameterDirection.Input)); // NUMERO DE ORIGEN
                parameters.Add(new OracleParameter("P_NCODGRU", OracleDbType.Int16, data.NCODGRU, ParameterDirection.Input)); // NUMERO DE ORIGEN
                parameters.Add(new OracleParameter("P_NCONCEPTO", OracleDbType.Int16, data.NCODCON, ParameterDirection.Input)); // NUMERO DE ORIGEN
                parameters.Add(new OracleParameter("P_NNUMASI", OracleDbType.Int16, data.NNUMASI, ParameterDirection.Input)); // NUMERO DE ORIGEN
                parameters.Add(new OracleParameter("P_NDETASO", OracleDbType.Int16, data.NDETASO, ParameterDirection.Input)); // NUMERO DE ORIGEN
                parameters.Add(new OracleParameter("P_CDESCRI", OracleDbType.NVarchar2, data.CDESCRI, ParameterDirection.Input)); // DESCRIPCION
                parameters.Add(new OracleParameter("P_CNUMCTA", OracleDbType.NVarchar2, data.CNUMCTA, ParameterDirection.Input)); // DESCRIPCION
                parameters.Add(new OracleParameter("P_FLGCCOS", OracleDbType.Int16, data.FLGCCOS, ParameterDirection.Input)); // NUMERO DE ORIGEN
                parameters.Add(new OracleParameter("P_VDEBCRE", OracleDbType.NVarchar2, data.VDEBCRE, ParameterDirection.Input)); // DESCRIPCION
                parameters.Add(new OracleParameter("P_NMONASO", OracleDbType.Int16, data.NMONASO, ParameterDirection.Input)); // NUMERO DE ORIGEN
                parameters.Add(new OracleParameter("P_CUSUMOD", OracleDbType.NVarchar2, data.CUSUMOD, ParameterDirection.Input)); // DESCRIPCION

                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureOnlineDB(procedure, parameters);
                entities.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                entities.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("ModificarDinamicaAsientos", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<RespuestaVM>(entities);
        }
        /*****************DETALLE DINAMICAS CONTABLES********************/

        // LISTA DE DETALLE DINAMICAS
        public Task<ListaDetalleDinamicaVM> ListarDetalleDinamica(DetalleDinamicaFilter data)
        {
            var parameters = new List<OracleParameter>();

            ListaDetalleDinamicaVM entities = new ListaDetalleDinamicaVM();
            entities.lista = new List<ListaDetalleDinamicaVM.P_TABLE>();

            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_S_DINVALOR_LS");

            try
            {
                parameters.Add(new OracleParameter("P_NNUMORI", OracleDbType.Int16, data.NNUMORI, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCODGRU", OracleDbType.Int16, data.NCODGRU, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCONCEPTO", OracleDbType.Int16, data.NCODCON, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NNUMASI", OracleDbType.Int16, data.NNUMASI, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NDETASO", OracleDbType.Int16, data.NDETASO, ParameterDirection.Input));

                parameters.Add(new OracleParameter("P_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureOnlineDB(procedure, parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ListaDetalleDinamicaVM.P_TABLE suggest = new ListaDetalleDinamicaVM.P_TABLE();
                        suggest.DNV_NNUMORI = reader["DNV_NNUMORI"] == DBNull.Value ? 0 : int.Parse(reader["DNV_NNUMORI"].ToString());
                        suggest.DNV_NCODGRU = reader["DNV_NCODGRU"] == DBNull.Value ? 0 : int.Parse(reader["DNV_NCODGRU"].ToString());
                        suggest.DNV_NCONCEPTO = reader["DNV_NCONCEPTO"] == DBNull.Value ? 0 : int.Parse(reader["DNV_NCONCEPTO"].ToString());
                        suggest.DNV_NNUMASI = reader["DNV_NNUMASI"] == DBNull.Value ? 0 : int.Parse(reader["DNV_NNUMASI"].ToString());
                        suggest.DNV_NDETASO = reader["DNV_NDETASO"] == DBNull.Value ? 0 : int.Parse(reader["DNV_NDETASO"].ToString());
                        suggest.DNV_CVALOR = reader["DNV_CVALOR"] == DBNull.Value ? string.Empty : reader["DNV_CVALOR"].ToString();
                        suggest.DNV_COBSERV = reader["DNV_COBSERV"] == DBNull.Value ? string.Empty : reader["DNV_COBSERV"].ToString();
                        suggest.DNV_NCODEST = reader["DNV_NCODEST"] == DBNull.Value ? 0 : int.Parse(reader["DNV_NCODEST"].ToString());
                        suggest.DNV_DFECREG = reader["DNV_DFECREG"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["DNV_DFECREG"].ToString());
                        suggest.DNV_CUSUCRE = reader["DNV_CUSUCRE"] == DBNull.Value ? string.Empty : reader["DNV_CUSUCRE"].ToString();
                        suggest.EST_CDESCRI = reader["EST_CDESCRI"] == DBNull.Value ? string.Empty : reader["EST_CDESCRI"].ToString();

                        entities.lista.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarDetalleDinamica", ex.ToString(), "3");
                throw;
            }

            return Task.FromResult<ListaDetalleDinamicaVM>(entities);
        }


        // ELIMINAR DETALLE DINAMICA DE ASIENTOS 
        public Task<RespuestaVM> EliminarDetalleDinamica(DetalleDinamicaFilter data)
        {
            var parameters = new List<OracleParameter>();
            RespuestaVM entities = new RespuestaVM();
            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_D_DINVALOR");
            try
            {
                parameters.Add(new OracleParameter("P_NNUMORI", OracleDbType.Int16, data.NNUMORI, ParameterDirection.Input)); // NUMERO DE ORIGEN
                parameters.Add(new OracleParameter("P_NCODGRU", OracleDbType.Int16, data.NCODGRU, ParameterDirection.Input)); // NUMERO DE INTERFACE
                parameters.Add(new OracleParameter("P_NCONCEPTO", OracleDbType.Int16, data.NCODCON, ParameterDirection.Input)); // NUMERO DE MOVIMIENTO
                parameters.Add(new OracleParameter("P_NNUMASI", OracleDbType.Int16, data.NNUMASI, ParameterDirection.Input)); // NUMERO DE ASIENTO
                parameters.Add(new OracleParameter("P_NDETASO", OracleDbType.Int16, data.NDETASO, ParameterDirection.Input)); // NUMERO DE ASIENTO

                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureOnlineDB(procedure, parameters);
                entities.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                entities.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("EliminarDetalleDinamica", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<RespuestaVM>(entities);
        }

        // AGREGAR DETALLE DINAMICA 
        public Task<RespuestaVM> AgregarDetalleDinamicas(ListaDetalle[] data)
        {
            var sPackageName = string.Format("{0}.{1}", this.Package, "USP_OI_I_DINVALOR");
            int rows = 0;
            OracleDataReader dr = null;
            List<OracleParameter> parameter = new List<OracleParameter>();

            List<ListaDetalle> ListReciDev = new List<ListaDetalle>();
            RespuestaVM Respuesta = new RespuestaVM();

            try
            {

                while (rows < data.Length)
                {
                    parameter.Add(new OracleParameter("P_NNUMORI", OracleDbType.Int16, data[rows].NNUMORI, ParameterDirection.Input)); // NUMERO DE ORIGEN
                    parameter.Add(new OracleParameter("P_NCODGRU", OracleDbType.Int16, data[rows].NCODGRU, ParameterDirection.Input)); // NUMERO DE ORIGEN
                    parameter.Add(new OracleParameter("P_NCONCEPTO", OracleDbType.Int16, data[rows].NCODCON, ParameterDirection.Input)); // NUMERO DE ORIGEN
                    parameter.Add(new OracleParameter("P_NNUMASI", OracleDbType.Int16, data[rows].NNUMASI, ParameterDirection.Input)); // NUMERO DE ORIGEN
                    parameter.Add(new OracleParameter("P_NDETASO", OracleDbType.Int16, data[rows].NDETASO, ParameterDirection.Input)); // NUMERO DE ORIGEN
                    parameter.Add(new OracleParameter("P_CVALOR", OracleDbType.NVarchar2, data[rows].CVALOR, ParameterDirection.Input)); // DESCRIPCION
                    parameter.Add(new OracleParameter("P_NCODEST", OracleDbType.Int16, data[rows].NCODEST, ParameterDirection.Input)); // NUMERO DE ORIGEN
                    parameter.Add(new OracleParameter("P_CUSUMOD", OracleDbType.NVarchar2, data[rows].CUSUMOD, ParameterDirection.Input)); // DESCRIPCION

                    OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                    OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                    parameter.Add(P_NCODE);
                    parameter.Add(P_SMESSAGE);

                    //using (dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter))

                    using (dr = (OracleDataReader)this.ExecuteByStoredProcedureOnlineDB(sPackageName, parameter)) ;
                    Respuesta.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                    Respuesta.P_SMESSAGE = P_SMESSAGE.Value.ToString();

                    rows++;
                    parameter.Clear();

                    if (data.Length == rows)
                    {
                        return Task.FromResult<RespuestaVM>(Respuesta);
                    }

                }
                Respuesta.P_NCODE = 1;

                Respuesta.P_SMESSAGE = "NO HAY DATOS SELECCIONADOS";
                ELog.CloseConnection(dr);

            }
            catch (Exception ex)
            {
                LogControl.save("AgregarDetalleDinamicas", ex.ToString(), "3");
            }
            return Task.FromResult<RespuestaVM>(Respuesta);

            /*
            var parameters = new List<OracleParameter>();
            RespuestaVM entities = new RespuestaVM();
            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_I_DINVALOR");
            try
            {
                parameters.Add(new OracleParameter("P_NNUMORI", OracleDbType.Int16, data.NNUMORI, ParameterDirection.Input)); // NUMERO DE ORIGEN
                parameters.Add(new OracleParameter("P_NCODGRU", OracleDbType.Int16, data.NCODGRU, ParameterDirection.Input)); // NUMERO DE ORIGEN
                parameters.Add(new OracleParameter("P_NCONCEPTO", OracleDbType.Int16, data.NCODCON, ParameterDirection.Input)); // NUMERO DE ORIGEN
                parameters.Add(new OracleParameter("P_NNUMASI", OracleDbType.Int16, data.NNUMASI, ParameterDirection.Input)); // NUMERO DE ORIGEN
                parameters.Add(new OracleParameter("P_NDETASO", OracleDbType.Int16, data.NDETASO, ParameterDirection.Input)); // NUMERO DE ORIGEN
                parameters.Add(new OracleParameter("P_CVALOR", OracleDbType.NVarchar2, data.CVALOR, ParameterDirection.Input)); // DESCRIPCION
                parameters.Add(new OracleParameter("P_NCODEST", OracleDbType.Int16, data.NCODEST, ParameterDirection.Input)); // NUMERO DE ORIGEN
                parameters.Add(new OracleParameter("P_CUSUMOD", OracleDbType.NVarchar2, data.CUSUMOD, ParameterDirection.Input)); // DESCRIPCION

                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureOnlineDB(procedure, parameters);

                //this.ExecuteByStoredProcedureVT(string.Format("{0}.{1}", ProcedureName.pkg_PreliminarSCTR, procedure), parameters);

                entities.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                entities.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                ELog.save(this, ex);
                throw;
            }
            return Task.FromResult<RespuestaVM>(entities);
            */
        }

    }
}
