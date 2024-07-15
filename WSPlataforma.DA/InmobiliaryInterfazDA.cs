using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Oracle.DataAccess.Client;
using System.Data;
using WSPlataforma.Util;
using WSPlataforma.Entities.InmobiliaryInterfazModel.ViewModel;
using WSPlataforma.Entities.InmobiliaryInterfazModel.BindingModel;

namespace WSPlataforma.DA
{
    public class InmobiliaryInterfazDA : ConnectionBase
    {
        private readonly string Package = "PKG_CONFIG_INTERFACE";

        public Task<InterfazVM> ListarOrigen()
        {
            var parameters = new List<OracleParameter>();
            InterfazVM entities = new InterfazVM();
            entities.combos = new List<InterfazVM.P_TABLE>();
            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_S_ORIGEN_LST");
            try
            {
                parameters.Add(new OracleParameter("P_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo_OI(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        InterfazVM.P_TABLE suggest = new InterfazVM.P_TABLE();
                        suggest.mvt_nnumori = reader["mvt_nnumori"] == DBNull.Value ? 0 : int.Parse(reader["mvt_nnumori"].ToString());
                        suggest.mvt_cdescri = reader["mvt_cdescri"] == DBNull.Value ? string.Empty : reader["mvt_cdescri"].ToString();
                        suggest.mvt_vestado = reader["mvt_vestado"] == DBNull.Value ? string.Empty : reader["mvt_vestado"].ToString();
                        suggest.mvt_destado = reader["desc_vestado"] == DBNull.Value ? string.Empty : reader["desc_vestado"].ToString();
                        entities.combos.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarOrigen", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<InterfazVM>(entities);
        }

        public Task<EstadosVM> ListarEstados()
        {
            var parameters = new List<OracleParameter>();
            EstadosVM entities = new EstadosVM();
            entities.combos = new List<EstadosVM.P_TABLE>();
            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_S_ESTADO_LST");
            try
            {
                parameters.Add(new OracleParameter("P_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo_OI(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        EstadosVM.P_TABLE suggest = new EstadosVM.P_TABLE();
                        suggest.mvt_vestado  = reader["MVT_VESTADO"] == DBNull.Value ? string.Empty : reader["MVT_VESTADO"].ToString();
                        suggest.mvt_destado = reader["DESC_VESTADO"] == DBNull.Value ? string.Empty : reader["DESC_VESTADO"].ToString();
                        entities.combos.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarEstados", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<EstadosVM>(entities);
        }

        public Task<TipoAsientoVM> ListarTipoAsiento()
        {
            var parameters = new List<OracleParameter>();
            TipoAsientoVM entities = new TipoAsientoVM();
            entities.combos = new List<TipoAsientoVM.P_TABLE>();
            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_S_TIPO_ASIENTO_LST");

            try
            {
                parameters.Add(new OracleParameter("P_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo_OI(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        TipoAsientoVM.P_TABLE suggest = new TipoAsientoVM.P_TABLE();
                        suggest.tipo_asiento = reader["tipo_asiento"] == DBNull.Value ? string.Empty : reader["tipo_asiento"].ToString();
                        suggest.descripcion = reader["descripcion"] == DBNull.Value ? string.Empty : reader["descripcion"].ToString();
                        suggest.noteexistsflag = reader["noteexistsflag"] == DBNull.Value ? string.Empty : reader["noteexistsflag"].ToString();
                        entities.combos.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarTipoAsiento", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<TipoAsientoVM>(entities);
        }

        public Task<RespuestaVM> ConsultarTipoAsiento(TipoAsientoFilter data)
        {
            var parameters = new List<OracleParameter>();
            RespuestaVM entities = new RespuestaVM();
            string asiento;
            asiento = data.mct_ctipreg.ToUpper();
            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_S_TIPO_ASIENTO_CONS");
            try
            {
                parameters.Add(new OracleParameter("P_SASIENTO", OracleDbType.NVarchar2, asiento, ParameterDirection.Input));
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureInmo_OI(procedure, parameters);
                entities.p_ncode = Convert.ToInt32(P_NCODE.Value.ToString());
                entities.p_smessage = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("ConsultarTipoAsiento", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<RespuestaVM>(entities);
        }

        public Task<ReportesAsociadosVM> ListarReportesAsociados()
        {
            var parameters = new List<OracleParameter>();
            ReportesAsociadosVM entities = new ReportesAsociadosVM();
            entities.combos = new List<ReportesAsociadosVM.P_TABLE>();
            var procedure = string.Format("{0}.{1}", "PKG_OI_CONFIG_CB_MODULE", "SP_OI_SEL_CB_MASTER");

            try
            {
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo_OI(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ReportesAsociadosVM.P_TABLE suggest = new ReportesAsociadosVM.P_TABLE();
                        suggest.id_cbco = reader["NIDCBCO"] == DBNull.Value ? string.Empty : reader["NIDCBCO"].ToString();
                        suggest.descripcion = reader["SDESCRIPT"] == DBNull.Value ? string.Empty : reader["SDESCRIPT"].ToString();
                        suggest.descripcion_corta = reader["SSHORT_DES"] == DBNull.Value ? string.Empty : reader["SSHORT_DES"].ToString();
                        suggest.estado = reader["NSTATUS"] == DBNull.Value ? string.Empty : reader["NSTATUS"].ToString();
                        suggest.estado_desc = reader["SSTATUS_DES"] == DBNull.Value ? string.Empty : reader["SSTATUS_DES"].ToString();
                        entities.combos.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarReportesAsociados", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<ReportesAsociadosVM>(entities);
        }

        public Task<ListarInterfazVM> ListarInterfaz(InterfazFilter data)
        {
            var parameters = new List<OracleParameter>();
            ListarInterfazVM entities = new ListarInterfazVM();
            entities.lista = new List<ListarInterfazVM.P_TABLE>();
            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_S_MOVTIP_LST");
            try
            {
                parameters.Add(new OracleParameter("P_NNUMORI", OracleDbType.Int16,data.mvt_nnumori, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo_OI(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ListarInterfazVM.P_TABLE suggest = new ListarInterfazVM.P_TABLE();
                        suggest.mvt_nnumori = reader["MVT_NNUMORI"] == DBNull.Value ? 0 : int.Parse(reader["MVT_NNUMORI"].ToString());
                        suggest.mvt_ncodgru = reader["MVT_NCODGRU"] == DBNull.Value ? 0 : int.Parse(reader["MVT_NCODGRU"].ToString());
                        suggest.mvt_cdescri = reader["MVT_CDESCRI"] == DBNull.Value ? string.Empty : reader["MVT_CDESCRI"].ToString();
                        suggest.mvt_vestado = reader["MVT_VESTADO"] == DBNull.Value ? string.Empty : reader["MVT_VESTADO"].ToString();
                        suggest.mvt_dfecreg = reader["MVT_DFECREG"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["MVT_DFECREG"].ToString());
                        suggest.mvt_cusucre = reader["MVT_CUSUCRE"] == DBNull.Value ? string.Empty : reader["MVT_CUSUCRE"].ToString();
                        suggest.mvt_cusumod = reader["MVT_CUSUMOD"] == DBNull.Value ? string.Empty : reader["MVT_CUSUMOD"].ToString();
                        suggest.mvt_dfecmod = reader["MVT_DFECMOD"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["MVT_DFECMOD"].ToString());
                        suggest.mvt_destado = reader["DESC_VESTADO"] == DBNull.Value ? string.Empty : reader["DESC_VESTADO"].ToString();
                        entities.lista.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarInterfaz", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<ListarInterfazVM>(entities);
        }

        public Task<RespuestaVM> AgregarInterfaz(InterfazFilterCrud data)
        {
            var parameters = new List<OracleParameter>();
            RespuestaVM entities = new RespuestaVM();
            string user_mod;
            if (data.mvt_cusucre.Length > 10)
            {
                user_mod = data.mvt_cusucre.ToString().Substring(1, 10);
            }
            else
            {
                user_mod = data.mvt_cusucre;
            }
            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_I_MOVTIP");
            try
            {
                parameters.Add(new OracleParameter("P_NNUMORI", OracleDbType.Int16, data.mvt_nnumori, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCODGRU", OracleDbType.Int16, data.mvt_ncodgru, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_CDESCRI", OracleDbType.NVarchar2, data.mvt_cdescri, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_VESTADO", OracleDbType.NVarchar2, data.mvt_vestado, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_CUSUMOD", OracleDbType.NVarchar2, user_mod, ParameterDirection.Input));
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureInmo_OI(procedure, parameters);
                entities.p_ncode = Convert.ToInt32(P_NCODE.Value.ToString());
                entities.p_smessage = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("AgregarInterfaz", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<RespuestaVM>(entities);
        }

        public Task<RespuestaVM> ModificarInterfaz(InterfazFilterCrud data)
        {
            var parameters = new List<OracleParameter>();
            RespuestaVM entities = new RespuestaVM();
            string user_mod;
            if (data.mvt_cusucre.Length > 10)
            {
                user_mod = data.mvt_cusucre.ToString().Substring(1, 10);
            }
            else
            {
                user_mod = data.mvt_cusucre;
            }
            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_U_MOVTIP");
            try
            {
                parameters.Add(new OracleParameter("P_NNUMORI", OracleDbType.Int16, data.mvt_nnumori, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCODGRU", OracleDbType.Int16, data.mvt_ncodgru, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_CDESCRI", OracleDbType.NVarchar2, data.mvt_cdescri, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_VESTADO", OracleDbType.NVarchar2, data.mvt_vestado, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_CUSUMOD", OracleDbType.NVarchar2, user_mod, ParameterDirection.Input));
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureInmo_OI(procedure, parameters);
                entities.p_ncode = Convert.ToInt32(P_NCODE.Value.ToString());
                entities.p_smessage = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("ModificarInterfaz", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<RespuestaVM>(entities);
        }

        public Task<RespuestaVM> EliminarInterfaz(InterfazFilterCrud data)
        {
            var parameters = new List<OracleParameter>();
            RespuestaVM entities = new RespuestaVM();
            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_D_MOVTIP");
            try
            {
                parameters.Add(new OracleParameter("P_NNUMORI", OracleDbType.Int16, data.mvt_nnumori, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCODGRU", OracleDbType.Int16, data.mvt_ncodgru, ParameterDirection.Input));
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureInmo_OI(procedure, parameters);
                entities.p_ncode = Convert.ToInt32(P_NCODE.Value.ToString());
                entities.p_smessage = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("EliminarInterfaz", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<RespuestaVM>(entities);
        }

        public Task<ListarMovimientosVM> ListarMovimientos(MovimientosFilter data)
        {
            var parameters = new List<OracleParameter>();
            ListarMovimientosVM entities = new ListarMovimientosVM();
            entities.lista = new List<ListarMovimientosVM.P_TABLE>();
            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_S_MOVCONT_LST");
            try
            {
                parameters.Add(new OracleParameter("P_NNUMORI", OracleDbType.Int16, data.mvt_nnumori, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCODGRU", OracleDbType.Int16, data.mvt_ncodgru, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo_OI(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ListarMovimientosVM.P_TABLE suggest = new ListarMovimientosVM.P_TABLE();
                        suggest.mct_nnumori = reader["MCT_NNUMORI"] == DBNull.Value ? 0 : int.Parse(reader["MCT_NNUMORI"].ToString());
                        suggest.mct_ncodgru = reader["MCT_NCODGRU"] == DBNull.Value ? 0 : int.Parse(reader["MCT_NCODGRU"].ToString());
                        suggest.mct_ncodcon = reader["MCT_NCODCON"] == DBNull.Value ? 0 : int.Parse(reader["MCT_NCODCON"].ToString());
                        suggest.mct_cdescri = reader["MCT_CDESCRI"] == DBNull.Value ? string.Empty : reader["MCT_CDESCRI"].ToString();
                        suggest.mct_ctipreg = reader["MCT_CTIPREG"] == DBNull.Value ? string.Empty : reader["MCT_CTIPREG"].ToString();
                        suggest.mct_vestado = reader["MCT_VESTADO"] == DBNull.Value ? string.Empty : reader["MCT_VESTADO"].ToString();
                        suggest.mct_dfecreg = reader["MCT_DFECREG"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["MCT_DFECREG"].ToString());
                        suggest.mct_cusucre = reader["MCT_CUSUCRE"] == DBNull.Value ? string.Empty : reader["MCT_CUSUCRE"].ToString();
                        suggest.mct_cusumod = reader["MCT_CUSUMOD"] == DBNull.Value ? string.Empty : reader["MCT_CUSUMOD"].ToString();
                        suggest.mct_dfecmod = reader["MCT_DFECMOD"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["MCT_DFECMOD"].ToString());
                        suggest.mct_destado = reader["DESC_VESTADO"] == DBNull.Value ? string.Empty : reader["DESC_VESTADO"].ToString();
                        suggest.mct_tiptreg = reader["DES_TIPTREG"] == DBNull.Value ? string.Empty : reader["DES_TIPTREG"].ToString();
                        suggest.mct_descbco = reader["NIDCBCO"] == DBNull.Value ? 0 : int.Parse(reader["NIDCBCO"].ToString());
                        suggest.mct_repasoc = reader["RPT_SDESCRIPT"] == DBNull.Value ? string.Empty : reader["RPT_SDESCRIPT"].ToString();
                        if (suggest.mct_descbco > 0)
                        {
                            suggest.mct_flagrep = true;
                        } else
                        {
                            suggest.mct_flagrep = false;
                        }
                        entities.lista.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarMovimientos", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<ListarMovimientosVM>(entities);
        }

        public Task<RespuestaVM> AgregarMovimientos(MovimientosFilterCrud data)
        {
            var parameters = new List<OracleParameter>();
            RespuestaVM entities = new RespuestaVM();
            string asiento;
            asiento = data.mct_ctipreg.ToUpper();
            string user_mod;
            if (data.mct_cusucre.Length > 10)
            {
                user_mod = data.mct_cusucre.ToString().Substring(1, 10);
            }
            else
            {
                user_mod = data.mct_cusucre;
            }
            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_I_MOVCONT");
            try
            {
                parameters.Add(new OracleParameter("P_NNUMORI", OracleDbType.Int16, data.mct_nnumori, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCODGRU", OracleDbType.Int16, data.mct_ncodgru, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCODCON", OracleDbType.Int16, data.mct_ncodcon, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_CDESCRI", OracleDbType.NVarchar2, data.mct_cdescri, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_CTIPREG", OracleDbType.NVarchar2, asiento, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_VESTADO", OracleDbType.NVarchar2, data.mct_vestado, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_CUSUMOD", OracleDbType.NVarchar2, user_mod, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NIDCBCO", OracleDbType.NVarchar2, data.mct_descbco, ParameterDirection.Input));
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureInmo_OI(procedure, parameters);
                entities.p_ncode = Convert.ToInt32(P_NCODE.Value.ToString());
                entities.p_smessage = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("AgregarMovimientos", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<RespuestaVM>(entities);
        }

        public Task<RespuestaVM> ModificarMovimientos(MovimientosFilterCrud data)
        {
            var parameters = new List<OracleParameter>();
            RespuestaVM entities = new RespuestaVM();
            string asiento;
            asiento = data.mct_ctipreg.ToUpper();
            string user_mod;
            if (data.mct_cusucre.Length > 10)
            {
                user_mod = data.mct_cusucre.ToString().Substring(1, 10);
            }
            else
            {
                user_mod = data.mct_cusucre;
            }
            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_U_MOVCONT");
            try
            {
                parameters.Add(new OracleParameter("P_NNUMORI", OracleDbType.Int16, data.mct_nnumori, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCODGRU", OracleDbType.Int16, data.mct_ncodgru, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCODCON", OracleDbType.Int16, data.mct_ncodcon, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_CDESCRI", OracleDbType.NVarchar2, data.mct_cdescri, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_CTIPREG", OracleDbType.NVarchar2, asiento, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_VESTADO", OracleDbType.NVarchar2, data.mct_vestado, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_CUSUMOD", OracleDbType.NVarchar2, user_mod, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NIDCBCO", OracleDbType.NVarchar2, data.mct_descbco, ParameterDirection.Input));
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureInmo_OI(procedure, parameters);
                entities.p_ncode = Convert.ToInt32(P_NCODE.Value.ToString());
                entities.p_smessage = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("ModificarMovimientos", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<RespuestaVM>(entities);
        }

        public Task<RespuestaVM> EliminarMovimientos(MovimientosFilterCrud data)
        {
            var parameters = new List<OracleParameter>();
            RespuestaVM entities = new RespuestaVM();
            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_D_MOVCONT");
            try
            {
                parameters.Add(new OracleParameter("P_NNUMORI", OracleDbType.Int16, data.mct_nnumori, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCODGRU", OracleDbType.Int16, data.mct_ncodgru, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCODCON", OracleDbType.Int16, data.mct_ncodcon, ParameterDirection.Input));
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureInmo_OI(procedure, parameters);
                entities.p_ncode = Convert.ToInt32(P_NCODE.Value.ToString());
                entities.p_smessage = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("EliminarMovimientos", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<RespuestaVM>(entities);
        }
    }
}