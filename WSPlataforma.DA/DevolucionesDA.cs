/*************************************************************************************************/
/*  NOMBRE              :   Devoluciones                                               */
/*  DESCRIPCION         :   Capa DataAccess                                                           */
/*  AUTOR               :   MATERIAGRIS - FRANCISCO AQUIÑO                                     */
/*  FECHA               :   22-12-2021                                                           */
/*  VERSION             :   1.0 - Devoluciones de NC - PD                 */
/*************************************************************************************************/
using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.Entities.DevolucionesModel.ViewModel;

using WSPlataforma.Util;

namespace WSPlataforma.DA
{
    public class DevolucionesDA : ConnectionBase
    {
        #region Filters

        // List<PremiumVM> ListPremium = new List<PremiumVM>();

        public Task<CombosBancos> ListarBancosCajas()
        {
            var parameters = new List<OracleParameter>();

            CombosBancos entities = new CombosBancos();
            entities.Combos = new List<CombosBancos.Contenido>();

            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_Devoluciones, "SPS_POST_ENTIDAD");

            try
            {
                parameters.Add(new OracleParameter("TIP_TRANSBANK", OracleDbType.Int64, 1, ParameterDirection.Input));
                var P_SMESSAGE = new OracleParameter("P_MENSAJE", OracleDbType.NVarchar2, ParameterDirection.Output)
                {
                    Size = 2000
                };
                parameters.Add(P_SMESSAGE);
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {


                        CombosBancos.Contenido conte = new CombosBancos.Contenido();

                        conte.id_tipo_entidad = reader["ID_TIPO_ENTIDAD"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ID_TIPO_ENTIDAD"].ToString());
                        conte.id_entidad = reader["ID_ENTIDAD"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ID_ENTIDAD"].ToString());
                        conte.vc_nombre = (reader["VC_NOMBRE"] != null ? reader["VC_NOMBRE"].ToString() : string.Empty);



                        entities.Combos.Add(conte);
                    }

                    entities.Mensaje = P_SMESSAGE.Value.ToString();
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarBancosCajas", ex.ToString(), "3");
                throw;
            }

            return Task.FromResult<CombosBancos>(entities);
        }

        public Task<CombosMotivoDev> ListarMotivoDev()
        {
            var parameters = new List<OracleParameter>();

            CombosMotivoDev entities = new CombosMotivoDev();
            entities.Combos = new List<CombosMotivoDev.Contenido>();

            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_Devoluciones, "SPS_LIST_MOTIV_DEV");

            try
            {
                var P_SMESSAGE = new OracleParameter("P_MENSAJE", OracleDbType.NVarchar2, ParameterDirection.Output)
                {
                    Size = 2000
                };
                parameters.Add(P_SMESSAGE);
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {


                        CombosMotivoDev.Contenido conte = new CombosMotivoDev.Contenido();
                        conte.nvalue = reader["NVALUE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["NVALUE"].ToString());
                        conte.svalue = (reader["SVALUE"] != null ? reader["SVALUE"].ToString() : string.Empty);

                        entities.Combos.Add(conte);
                    }

                    entities.Mensaje = P_SMESSAGE.Value.ToString();
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarMotivoDev", ex.ToString(), "3");
                throw;
            }

            return Task.FromResult<CombosMotivoDev>(entities);
        }
        public Task<CombosTipoDevoluciones> ListarTipoDevoluciones()
        {
            var parameters = new List<OracleParameter>();

            CombosTipoDevoluciones entities = new CombosTipoDevoluciones();
            entities.Combos = new List<CombosTipoDevoluciones.Contenido>();

            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_Devoluciones, "SPS_LIST_TIPDEV");

            try
            {
                var P_SMESSAGE = new OracleParameter("P_MENSAJE", OracleDbType.NVarchar2, ParameterDirection.Output)
                {
                    Size = 2000
                };
                parameters.Add(P_SMESSAGE);
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {


                        CombosTipoDevoluciones.Contenido conte = new CombosTipoDevoluciones.Contenido();
                        conte.nvalue = reader["NVALUE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["NVALUE"].ToString());
                        conte.svalue = (reader["SVALUE"] != null ? reader["SVALUE"].ToString() : string.Empty);

                        entities.Combos.Add(conte);
                    }

                    entities.Mensaje = P_SMESSAGE.Value.ToString();
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarTipoDevoluciones", ex.ToString(), "3");
                throw;
            }

            return Task.FromResult<CombosTipoDevoluciones>(entities);
        }
        public Task<CombosTipoDocDevoluciones> ListarTipoDoc()
        {
            var parameters = new List<OracleParameter>();

            CombosTipoDocDevoluciones entities = new CombosTipoDocDevoluciones();
            entities.Combos = new List<CombosTipoDocDevoluciones.Contenido>();

            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_Devoluciones, "SPS_LIST_TIPO_DOC");

            try
            {
                var P_SMESSAGE = new OracleParameter("P_MENSAJE", OracleDbType.NVarchar2, ParameterDirection.Output)
                {
                    Size = 2000
                };
                parameters.Add(P_SMESSAGE);
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {


                        CombosTipoDocDevoluciones.Contenido conte = new CombosTipoDocDevoluciones.Contenido();
                        conte.ntipdoc = reader["NTIPDOC"] == DBNull.Value ? 0 : Convert.ToInt32(reader["NTIPDOC"].ToString());
                        conte.des_tip_doc = (reader["DES_TIP_DOC"] != null ? reader["DES_TIP_DOC"].ToString() : string.Empty);

                        entities.Combos.Add(conte);
                    }

                    entities.Mensaje = P_SMESSAGE.Value.ToString();
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarTipoDoc", ex.ToString(), "3");
                throw;
            }

            return Task.FromResult<CombosTipoDocDevoluciones>(entities);
        }
        public Task<CombosDevoluciones> ListarCombosDevoluciones()
        {
            var parameters = new List<OracleParameter>();

            CombosDevoluciones entities = new CombosDevoluciones();
            entities.Combos = new List<CombosDevoluciones.Contenido>();

            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_Devoluciones, "SPS_LIST_COMBO_DEV");

            try
            {
                var P_SMESSAGE = new OracleParameter("P_MENSAJE", OracleDbType.NVarchar2, ParameterDirection.Output)
                {
                    Size = 2000
                };
                parameters.Add(P_SMESSAGE);
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {


                        CombosDevoluciones.Contenido conte = new CombosDevoluciones.Contenido();
                        conte.ntipdev = reader["NVALUE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["NVALUE"].ToString());
                        conte.des_tipdev = (reader["SVALUE"] != null ? reader["SVALUE"].ToString() : string.Empty);


                        entities.Combos.Add(conte);
                    }

                    entities.Mensaje = P_SMESSAGE.Value.ToString();
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarCombosDevoluciones", ex.ToString(), "3");
                throw;
            }

            return Task.FromResult<CombosDevoluciones>(entities);
        }
        public Task<ListarVisualizacion> VizualizarDevoluciones(VisualizarVM data)
        {
            var parameters = new List<OracleParameter>();

            ListarVisualizacion entities = new ListarVisualizacion();
            entities.Lista = new List<ListarVisualizacion.listas>();

            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_Devoluciones_tesoreria, "SPS_POST_DEV_VISUALIZAR");

            try
            {
                //INPUT
                parameters.Add(new OracleParameter("P_NABONO_DEV", OracleDbType.Int16, data.nabonodev, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_TIPO_DEV", OracleDbType.Int16, data.ntipodev, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_FECHA_INI", OracleDbType.Date, data.dfechaini, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_FECHA_FIN", OracleDbType.Date, data.dfechafin, ParameterDirection.Input));
                //OUTPUT
                var P_ESTADO = new OracleParameter("P_ESTADO", OracleDbType.Int64, ParameterDirection.Output);
                var P_MENSAGE = new OracleParameter("P_MENSAGE", OracleDbType.NVarchar2, ParameterDirection.Output)
                {
                    Size = 2000
                };
                parameters.Add(P_ESTADO);
                parameters.Add(P_MENSAGE);
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));


                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);


                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ListarVisualizacion.listas conte = new ListarVisualizacion.listas();

                        conte.NIDDEV = reader["NIDDEV"] == DBNull.Value ? 0 : Convert.ToInt32(reader["NIDDEV"].ToString());
                        conte.DFECHADEV = reader["DFECHADEV"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["DFECHADEV"].ToString());
                        conte.SUSUARIO = (reader["SUSUARIO"] != null ? reader["SUSUARIO"].ToString() : string.Empty);
                        conte.NRO_CERTIFICADO = (reader["NRO_CERTIFICADO"] != null ? reader["NRO_CERTIFICADO"].ToString() : string.Empty);
                        conte.NRO_TRANSACCION = (reader["NRO_TRANSACCION"] != null ? reader["NRO_TRANSACCION"].ToString() : string.Empty);
                        conte.MOTIVO_DEVOLUCION = (reader["MOTIVO_DEVOLUCION"] != null ? reader["MOTIVO_DEVOLUCION"].ToString() : string.Empty);
                        conte.SPRODUCTO = (reader["SPRODUCTO"] != null ? reader["SPRODUCTO"].ToString() : string.Empty);
                        conte.SCLIENTE = (reader["SCLIENTE"] != null ? reader["SCLIENTE"].ToString() : string.Empty);
                        conte.FACTURA = (reader["FACTURA"] != null ? reader["FACTURA"].ToString() : string.Empty);
                        conte.NOTA_CREDITO = (reader["NOTA_CREDITO"] != null ? reader["NOTA_CREDITO"].ToString() : string.Empty);
                        conte.S_MONEDA = (reader["S_MONEDA"] != null ? reader["S_MONEDA"].ToString() : string.Empty);

                        entities.Lista.Add(conte);
                    }

                    entities.Estado = Int32.Parse(P_ESTADO.Value.ToString());
                    entities.Mensaje = P_MENSAGE.Value.ToString();
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("VizualizarDevoluciones", ex.ToString(), "3");
                throw;
            }

            return Task.FromResult<ListarVisualizacion>(entities);
        }

        public Task<ComboSeleccionarBanco> SeleccionarBanco(BancoProtecta data)
        {
            var parameters = new List<OracleParameter>();

            ComboSeleccionarBanco entities = new ComboSeleccionarBanco();
            entities.Combos = new List<ComboSeleccionarBanco.Contenido>();

            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_Devoluciones_tesoreria, "SPS_LIST_CTA");

            try
            {
                parameters.Add(new OracleParameter("P_BANCO", OracleDbType.Int16, data.bancoProtecta, ParameterDirection.Input));
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {


                        ComboSeleccionarBanco.Contenido conte = new ComboSeleccionarBanco.Contenido();
                        conte.ID_CTA = reader["ID_CTA"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ID_CTA"].ToString());
                        conte.CTA_OUT = (reader["CTA_OUT"] != null ? reader["CTA_OUT"].ToString() : string.Empty);

                        entities.Combos.Add(conte);
                    }

                    //entities.Mensaje = P_SMESSAGE.Value.ToString();
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("SeleccionarBanco", ex.ToString(), "3");
                throw;
            }

            return Task.FromResult<ComboSeleccionarBanco>(entities);
        }
        public Task<RespuestaExactus> EnviarExactus(EnviarExactusVM data)
        {
            var parameters = new List<OracleParameter>();

            RespuestaExactus entities = new RespuestaExactus();
            entities.Lista = new List<RespuestaExactus.listas>();

            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_Devoluciones_tesoreria, "SPS_POST_ENVIAR_EXACTUS");

            try
            {
                //INPUT
                parameters.Add(new OracleParameter("P_NIDDEV", OracleDbType.Int16, data.P_NIDDEV, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NIDBANCOPROTECTA", OracleDbType.NVarchar2, data.P_NIDBANCOPROTECTA, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SNUMBACOPROTECTA", OracleDbType.NVarchar2, data.P_SNUMBACOPROTECTA, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NSTE", OracleDbType.NVarchar2, data.P_NSTE, ParameterDirection.Input));
                //OUTPUT
                var P_ESTADO = new OracleParameter("P_ESTADO", OracleDbType.Int64, ParameterDirection.Output);
                var P_MENSAGE = new OracleParameter("P_MENSAGE", OracleDbType.NVarchar2, ParameterDirection.Output)
                {
                    Size = 2000
                };
                parameters.Add(P_MENSAGE);
                parameters.Add(P_ESTADO);
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {

                        RespuestaExactus.listas conte = new RespuestaExactus.listas();

                        conte.SCUENTAORIGEN = (reader["SCUENTAORIGEN"] != null ? reader["SCUENTAORIGEN"].ToString() : string.Empty);
                        conte.NORIGEN = reader["NORIGEN"] == DBNull.Value ? 0 : Convert.ToInt32(reader["NORIGEN"].ToString());
                        conte.STIPORIGEN = (reader["STIPORIGEN"] != null ? reader["STIPORIGEN"].ToString() : string.Empty);
                        conte.SCUENTADESTINO = (reader["SCUENTADESTINO"] != null ? reader["SCUENTADESTINO"].ToString() : string.Empty);
                        conte.NNUMDESTINO = reader["NNUMDESTINO"] == DBNull.Value ? 0 : Convert.ToInt32(reader["NNUMDESTINO"].ToString());
                        conte.STIPODESTINO = (reader["STIPODESTINO"] != null ? reader["STIPODESTINO"].ToString() : string.Empty);
                        conte.NSUBTIPODESTINO = reader["NSUBTIPODESTINO"] == DBNull.Value ? 0 : Convert.ToInt32(reader["NSUBTIPODESTINO"].ToString());
                        conte.NMONTOORIGEN = (reader["NMONTOORIGEN"] != null ? reader["NMONTOORIGEN"].ToString() : string.Empty);
                        conte.NMONTODESTINO = (reader["NMONTODESTINO"] != null ? reader["NMONTODESTINO"].ToString() : string.Empty);
                        //conte.NMONTODESTINO = reader["NMONTODESTINO"] == DBNull.Value ? 0 : Convert.ToInt32(reader["NMONTODESTINO"].ToString());
                        conte.SCONCEPTO = (reader["SCONCEPTO"] != null ? reader["SCONCEPTO"].ToString() : string.Empty);
                        conte.SENTIDADFINAN = (reader["SENTIDADFINAN"] != null ? reader["SENTIDADFINAN"].ToString() : string.Empty);
                        conte.SMONEDA = (reader["SMONEDA"] != null ? reader["SMONEDA"].ToString() : string.Empty);
                        conte.SBENEFICIARIO = (reader["SBENEFICIARIO"] != null ? reader["SBENEFICIARIO"].ToString() : string.Empty);
                        conte.SCONTRIBUYENTE = (reader["SCONTRIBUYENTE"] != null ? reader["SCONTRIBUYENTE"].ToString() : string.Empty);
                        conte.SCENTROCOSTO = (reader["SCENTROCOSTO"] != null ? reader["SCENTROCOSTO"].ToString() : string.Empty);
                        conte.SCUENTACONTABLE = (reader["SCUENTACONTABLE"] != null ? reader["SCUENTACONTABLE"].ToString() : string.Empty);
                        conte.SDATO1 = (reader["SDATO1"] != null ? reader["SDATO1"].ToString() : string.Empty);
                        conte.SDATO2 = (reader["SDATO2"] != null ? reader["SDATO2"].ToString() : string.Empty);
                        conte.SDATO3 = (reader["SDATO3"] != null ? reader["SDATO3"].ToString() : string.Empty);
                        conte.DFECHAHOY = DateTime.Now.ToString("dd-MM-yyyy");

                        entities.Lista.Add(conte);
                    }

                    //entities.Mensaje = P_SMESSAGE.Value.ToString();
                }

                reader.Close();

                // OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);
                //entities.Estado = Int32.Parse(P_ESTADO.Value.ToString());
                //entities.Mensaje = P_MENSAGE.Value.ToString();
                //ELog.CloseConnection(odr);


            }
            catch (Exception ex)
            {
                LogControl.save("EnviarExactus", ex.ToString(), "3");
                throw;
            }

            return Task.FromResult<RespuestaExactus>(entities);
        }

        //--------------Reversion----------------//

        public Task<CombosMetodosBusqueda> ListarMetodosBusqueda()
        {
            var parameters = new List<OracleParameter>();

            CombosMetodosBusqueda entities = new CombosMetodosBusqueda();
            entities.Combos = new List<CombosMetodosBusqueda.Contenido>();

            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_Devoluciones_tesoreria, "SPS_LIST_COMBO");

            try
            {
                var P_SMESSAGE = new OracleParameter("P_MENSAJE", OracleDbType.NVarchar2, ParameterDirection.Output)
                {
                    Size = 2000
                };
                parameters.Add(P_SMESSAGE);
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {


                        CombosMetodosBusqueda.Contenido conte = new CombosMetodosBusqueda.Contenido();
                        conte.NTIPDOC = reader["NTIPDOC"] == DBNull.Value ? 0 : Convert.ToInt32(reader["NTIPDOC"].ToString());
                        conte.DES_TIP_DOC = (reader["DES_TIP_DOC"] != null ? reader["DES_TIP_DOC"].ToString() : string.Empty);


                        entities.Combos.Add(conte);
                    }

                    entities.Mensaje = P_SMESSAGE.Value.ToString();
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarMetodosBusqueda", ex.ToString(), "3");
                throw;
            }

            return Task.FromResult<CombosMetodosBusqueda>(entities);
        }

        public Task<ListarBusquedaReversion> ListarBusquedaCombo(ReversionVM data)
        {
            var parameters = new List<OracleParameter>();

            ListarBusquedaReversion entities = new ListarBusquedaReversion();
            entities.Lista = new List<ListarBusquedaReversion.listas>();

            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_Devoluciones_tesoreria, "SPS_LIST_COMBO_BUSQUEDA");

            string fechaInicio = data.fechaInicio?.ToString("dd/MM/yyyy");
            string fechaFin = data.fechaFin?.ToString("dd/MM/yyyy");

            try
            {
                //INPUT
                parameters.Add(new OracleParameter("P_FECHA_INI", OracleDbType.NVarchar2, fechaInicio, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_FECHA_FIN", OracleDbType.NVarchar2, fechaFin, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DOCUMENT", OracleDbType.NVarchar2, data.sdocumento, ParameterDirection.Input));
                //OUTPUT
                //var P_ESTADO = new OracleParameter("P_ESTADO", OracleDbType.Int64, ParameterDirection.Output);
                var P_MENSAGE = new OracleParameter("P_MENSAGE", OracleDbType.NVarchar2, ParameterDirection.Output)
                {
                    Size = 2000
                };
                parameters.Add(P_MENSAGE);
                //parameters.Add(P_ESTADO);
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));


                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ListarBusquedaReversion.listas conte = new ListarBusquedaReversion.listas();

                        conte.niddev = (reader["NIDDEV"] != null ? reader["NIDDEV"].ToString() : string.Empty);
                        conte.scuentacliente = (reader["SCUENTACLIENTE"] != null ? reader["SCUENTACLIENTE"].ToString() : string.Empty);
                        conte.scliename = (reader["SCLIENAME"] != null ? reader["SCLIENAME"].ToString() : string.Empty);
                        conte.scorreocli = (reader["SCORREOCLI"] != null ? reader["SCORREOCLI"].ToString() : string.Empty);
                        conte.dfechadev = (reader["DFECHADEV"] != null ? reader["DFECHADEV"].ToString() : string.Empty);
                        conte.stransferencia = (reader["STRANSFERENCIA"] != null ? reader["STRANSFERENCIA"].ToString() : string.Empty);


                        entities.Lista.Add(conte);
                    }

                    //entities.Estado = Int32.Parse(P_ESTADO.Value.ToString());
                    entities.Mensaje = P_MENSAGE.Value.ToString();
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarBusquedaCombo", ex.ToString(), "3");
                throw;
            }

            return Task.FromResult<ListarBusquedaReversion>(entities);
        }

        ///---------------------CORREO-------------------------------//

        public Task<ListarVisualizacionCorreo> BusquedaFechasCorreo(CorreoVM data)
        {
            var parameters = new List<OracleParameter>();

            ListarVisualizacionCorreo entities = new ListarVisualizacionCorreo();
            entities.Lista = new List<ListarVisualizacionCorreo.listas>();

            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_Devoluciones_tesoreria, "SP_POST_VALIDAR_PAGOS_BANCO");

            try
            {
                //INPUT
                parameters.Add(new OracleParameter("P_FECHA_INI", OracleDbType.Date, data.fechaInicio, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_FECHA_FIN", OracleDbType.Date, data.fechaFin, ParameterDirection.Input));
                //OUTPUT
                var P_ESTADO = new OracleParameter("P_ESTADO", OracleDbType.Int64, ParameterDirection.Output);
                var P_MENSAGE = new OracleParameter("P_MENSAGE", OracleDbType.NVarchar2, ParameterDirection.Output)
                {
                    Size = 2000
                };
                parameters.Add(P_MENSAGE);
                parameters.Add(P_ESTADO);
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ListarVisualizacionCorreo.listas conte = new ListarVisualizacionCorreo.listas();

                        conte.dfmovimientoDesc = (reader["DFMOVIMIENTO"] != null ? reader["DFMOVIMIENTO"].ToString() : string.Empty);

                        //reader["DFMOVIMIENTO"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["DFMOVIMIENTO"].ToString());
                        conte.dfmovimientoValue = (reader["DFMOVIMIENTO"] != null ? reader["DFMOVIMIENTO"].ToString() : string.Empty);
                        //reader["DFMOVIMIENTO"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["DFMOVIMIENTO"].ToString());

                        entities.Lista.Add(conte);
                    }

                    entities.Estado = Int32.Parse(P_ESTADO.Value.ToString());
                    entities.Mensaje = P_MENSAGE.Value.ToString();
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("BusquedaFechasCorreo", ex.ToString(), "3");
                throw;
            }

            return Task.FromResult<ListarVisualizacionCorreo>(entities);
        }

        public Task<ListarVisualizacionCorreoHora> BusquedaHoraFechasCorreo(CorreoVM data)
        {
            var parameters = new List<OracleParameter>();

            ListarVisualizacionCorreoHora entities = new ListarVisualizacionCorreoHora();
            entities.Lista = new List<ListarVisualizacionCorreoHora.listas>();

            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_Devoluciones_tesoreria, "SP_POST_NOTIFICACION_DEV");

            try
            {
                //INPUT
                parameters.Add(new OracleParameter("P_FECHA_INI", OracleDbType.Date, data.fechaInicio, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_FECHA_FIN", OracleDbType.Date, data.fechaFin, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_FECHA_VALOR", OracleDbType.NVarchar2, data.dfmovimiento, ParameterDirection.Input));
                //OUTPUT
                var P_ESTADO = new OracleParameter("P_ESTADO", OracleDbType.Int64, ParameterDirection.Output);
                var P_MENSAGE = new OracleParameter("P_MENSAGE", OracleDbType.NVarchar2, ParameterDirection.Output)
                {
                    Size = 2000
                };
                parameters.Add(P_MENSAGE);
                parameters.Add(P_ESTADO);
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ListarVisualizacionCorreoHora.listas conte = new ListarVisualizacionCorreoHora.listas();
                        conte.NIDDEV = reader["NIDDEV"] == DBNull.Value ? 0 : Convert.ToInt32(reader["NIDDEV"].ToString());
                        conte.SCUENTACLIENTE = (reader["SCUENTACLIENTE"] != null ? reader["SCUENTACLIENTE"].ToString() : string.Empty);
                        conte.SCLIENAME = (reader["SCLIENAME"] != null ? reader["SCLIENAME"].ToString() : string.Empty);
                        conte.SCORREOCLI = (reader["SCORREOCLI"] != null ? reader["SCORREOCLI"].ToString() : string.Empty);
                        conte.SNUMERO_OPERACION = (reader["SNUMERO_OPERACION"] != null ? reader["SNUMERO_OPERACION"].ToString() : string.Empty);
                        conte.STRANSFERENCIA = (reader["STRANSFERENCIA"] != null ? reader["STRANSFERENCIA"].ToString() : string.Empty);
                        conte.SDETALLE_CORREO = (reader["SDETALLE_CORREO"] != null ? reader["SDETALLE_CORREO"].ToString() : string.Empty);


                        entities.Lista.Add(conte);
                    }

                    entities.Estado = Int32.Parse(P_ESTADO.Value.ToString());
                    entities.Mensaje = P_MENSAGE.Value.ToString();
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("BusquedaHoraFechasCorreo", ex.ToString(), "3");
                throw;
            }

            return Task.FromResult<ListarVisualizacionCorreoHora>(entities);
        }

        //BusquedaHoraFechasCorreo

        public Task<RespuestaCorreo> EnviarCorreoReversion(CorreoEnviarReversionVM data)
        {
            var parameters = new List<OracleParameter>();


            RespuestaCorreo entities = new RespuestaCorreo();

            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_Devoluciones_tesoreria, "SPS_INICIAR_REVERSION");

            try
            {
                //INPUT
                parameters.Add(new OracleParameter("P_NIDDEV", OracleDbType.NVarchar2, data.p_niddev, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SDETALLE_CORREO_RCLIENTE", OracleDbType.NVarchar2, data.p_sdetalle_correo_rcliente, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SDETALLE_CORREO_RCOBRANZA", OracleDbType.NVarchar2, data.p_sdetalle_correo_rcobranza, ParameterDirection.Input));
                //OUTPUT
                //var P_ESTADO = new OracleParameter("P_ESTADO", OracleDbType.Int64, ParameterDirection.Output);
                var P_MENSAGE = new OracleParameter("P_MENSAGE", OracleDbType.NVarchar2, ParameterDirection.Output)
                {
                    Size = 2000
                };
                parameters.Add(P_MENSAGE);
                //parameters.Add(P_ESTADO);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);
                //entities.Estado = Int32.Parse(P_ESTADO.Value.ToString());
                entities.Mensaje = P_MENSAGE.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("EnviarCorreoReversion", ex.ToString(), "3");
                throw;
            }

            return Task.FromResult<RespuestaCorreo>(entities);
        }

        //RespuestaCorreo

        public Task<RespuestaCorreo> EnviarCorreo(CorreoEnviarVM data)
        {
            var parameters = new List<OracleParameter>();
            RespuestaCorreo entities = new RespuestaCorreo();

            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_Devoluciones_tesoreria, "SPS_POST_ENVIAR_CORREO");

            try
            {
                //INPUT
                parameters.Add(new OracleParameter("P_NIDDEV", OracleDbType.Int64, data.P_NIDDEV, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SDETALLE_CORREO", OracleDbType.NVarchar2, data.P_SDETALLE_CORREO, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_STRANSFERENCIA", OracleDbType.NVarchar2, data.P_STRANSFERENCIA, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SNUMERO_OPERACION", OracleDbType.NVarchar2, data.P_SNUMERO_OPERACION, ParameterDirection.Input));
                //OUTPUT
                var P_ESTADO = new OracleParameter("P_ESTADO", OracleDbType.Int64, ParameterDirection.Output);
                var P_MENSAGE = new OracleParameter("P_MENSAGE", OracleDbType.NVarchar2, ParameterDirection.Output)
                {
                    Size = 2000
                };
                parameters.Add(P_MENSAGE);
                parameters.Add(P_ESTADO);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);
                entities.Estado = Int32.Parse(P_ESTADO.Value.ToString());
                entities.Mensaje = P_MENSAGE.Value.ToString();
                ELog.CloseConnection(odr);


            }
            catch (Exception ex)
            {
                LogControl.save("EnviarCorreo", ex.ToString(), "3");
                throw;
            }

            return Task.FromResult<RespuestaCorreo>(entities);
        }

        #endregion
    }
}