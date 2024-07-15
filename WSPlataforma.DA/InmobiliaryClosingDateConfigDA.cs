using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Oracle.DataAccess.Client;
using WSPlataforma.Util;
using WSPlataforma.Entities.InmobiliaryClosingDateConfigModel.ViewModel;
using WSPlataforma.Entities.InmobiliaryClosingDateConfigModel.BindingModel;

namespace WSPlataforma.DA
{

    public class InmobiliaryClosingDateConfigDA : ConnectionBase
    {

        private readonly string Package = "PKG_CONFIG_CIERRE";

        // ESTADOS
        public Task<EstadosVM> ListarEstados()
        {
            var parameters = new List<OracleParameter>();
            EstadosVM entities = new EstadosVM();
            entities.combos = new List<EstadosVM.P_TABLE>();
            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_T_NESTADO");
            try
            {
                parameters.Add(new OracleParameter("P_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo_OI(procedure, parameters);
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
                LogControl.save("ListarEstados", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<EstadosVM>(entities);
        }

        // RAMOS
        public Task<RamosVM> ListarRamos(RamosFilter data)
        {
            var parameters = new List<OracleParameter>();

            RamosVM entities = new RamosVM();
            entities.combos = new List<RamosVM.P_TABLE>();

            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_T_NRAMOS");
            try
            {
                parameters.Add(new OracleParameter("P_NNUMORI", OracleDbType.Int16, data.NNUMORI, ParameterDirection.Input)); // ORIGEN
                parameters.Add(new OracleParameter("P_NFLAG", OracleDbType.Int16, data.FLAG, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo_OI(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        RamosVM.P_TABLE suggest = new RamosVM.P_TABLE();
                        suggest.NBRANCH = reader["NBRANCH"] == DBNull.Value ? string.Empty : reader["NBRANCH"].ToString();
                        suggest.SBRANCH_DESC = reader["SBRANCH_DESC"] == DBNull.Value ? string.Empty : reader["SBRANCH_DESC"].ToString();
                        entities.combos.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarRamos", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<RamosVM>(entities);
        }

        // MESES
        public Task<MesesVM> ListarMeses()
        {
            var parameters = new List<OracleParameter>();
            MesesVM entities = new MesesVM();
            entities.combos = new List<MesesVM.P_TABLE>();
            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_T_NMESES");
            try
            {
                parameters.Add(new OracleParameter("P_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo_OI(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        MesesVM.P_TABLE suggest = new MesesVM.P_TABLE();
                        suggest.NROMES = reader["NROMES"] == DBNull.Value ? string.Empty : reader["NROMES"].ToString();
                        suggest.MES = reader["MES"] == DBNull.Value ? string.Empty : reader["MES"].ToString();
                        entities.combos.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarMeses", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<MesesVM>(entities);
        }

        // LISTAR CONFIGURACIONES
        public Task<ClosingDateConfigVM> ListarConfiguraciones(ClosingDateConfigFilter data)
        {
            var parameters = new List<OracleParameter>();
            ClosingDateConfigVM entities = new ClosingDateConfigVM();
            entities.lista = new List<ClosingDateConfigVM.P_TABLE>();
            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_S_CONFIGCIERRE");
            try
            {
                parameters.Add(new OracleParameter("P_NNUMORI", OracleDbType.Int16, data.NNUMORI, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NMES", OracleDbType.Int16, data.NMES, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NANIO", OracleDbType.Int16, data.NANIO, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NESTADO", OracleDbType.Int16, data.NESTADO, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int16, data.NBRANCH, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCODGRU", OracleDbType.Int16, data.NCODGRU, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo_OI(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ClosingDateConfigVM.P_TABLE suggest = new ClosingDateConfigVM.P_TABLE();
                        suggest.NIDCCIERRE = reader["NIDCCIERRE"] == DBNull.Value ? 0 : int.Parse(reader["NIDCCIERRE"].ToString()); // CÓDIGO AUTOGENERADO
                        suggest.NNUMORI = reader["NNUMORI"] == DBNull.Value ? 0 : int.Parse(reader["NNUMORI"].ToString()); // ORIGEN
                        suggest.NCODGRU = reader["NCODGRU"] == DBNull.Value ? 0 : int.Parse(reader["NCODGRU"].ToString()); // INTERFAZ
                        suggest.NMES = reader["NMES"] == DBNull.Value ? 0 : int.Parse(reader["NMES"].ToString()); // MES
                        suggest.NBRANCH = reader["NBRANCH"] == DBNull.Value ? 0 : int.Parse(reader["NBRANCH"].ToString()); // RAMO
                        suggest.NESTADO = reader["NESTADO"] == DBNull.Value ? 0 : int.Parse(reader["NESTADO"].ToString()); // ESTADO
                        suggest.NANIO = reader["NANIO"] == DBNull.Value ? string.Empty : reader["NANIO"].ToString(); // AÑO

                        //suggest.DFECINI      = reader["DFECINI"]      == DBNull.Value ? string.Empty : Convert.ToDateTime(reader["DFECINI"]).ToString("dd/MM/yyyy"); // FECHA DE INICIO
                        //suggest.DFECFIN      = reader["DFECFIN"]      == DBNull.Value ? string.Empty : Convert.ToDateTime(reader["DFECFIN"]).ToString("dd/MM/yyyy"); // FECHA DE FIN
                        //suggest.DFECCIERRE   = reader["DFECCIERRE"]   == DBNull.Value ? string.Empty : Convert.ToDateTime(reader["DFECCIERRE"]).ToString("dd/MM/yyyy"); // FECHA DE CIERRE
                        suggest.DFECINI = reader["DFECINI"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["DFECINI"].ToString()); // FECHA DE INICIO
                        suggest.DFECFIN = reader["DFECFIN"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["DFECFIN"].ToString()); // FECHA DE FIN
                        suggest.DFECCIERRE = reader["DFECCIERRE"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["DFECCIERRE"].ToString()); // FECHA DE CIERRE

                        suggest.SCUSUCRE = reader["SCUSUCRE"] == DBNull.Value ? string.Empty : reader["SCUSUCRE"].ToString(); // USUARIO CREADOR
                        suggest.SCUSUMOD = reader["SCUSUMOD"] == DBNull.Value ? string.Empty : reader["SCUSUMOD"].ToString(); // USUARIO MODIFICADOR
                        suggest.DFECREG = reader["DFECREG"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["DFECREG"].ToString()); // FECHA DE REGISTRO
                        suggest.DFECMOD = reader["DFECMOD"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["DFECMOD"].ToString()); // FECHA DE MODIFICACÓN

                        suggest.DESC_NNUMORI = reader["DESC_NNUMORI"] == DBNull.Value ? string.Empty : reader["DESC_NNUMORI"].ToString(); // DESCRIPCIÓN ORIGEN
                        suggest.DESC_NCODGRU = reader["DESC_NCODGRU"] == DBNull.Value ? string.Empty : reader["DESC_NCODGRU"].ToString(); // DESCRIPCIÓN INTERFAZ
                        suggest.DESC_NMES = reader["DESC_NMES"] == DBNull.Value ? string.Empty : reader["DESC_NMES"].ToString(); // DESCRIPCIÓN MES
                        suggest.DESC_NESTADO = reader["DESC_NESTADO"] == DBNull.Value ? string.Empty : reader["DESC_NESTADO"].ToString(); // DESCRIPCIÓN ESTADO
                        suggest.DESC_NBRANCH = reader["DESC_NBRANCH"] == DBNull.Value ? string.Empty : reader["DESC_NBRANCH"].ToString(); // DESCRIPCIÓN RAMO
                        entities.lista.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarConfiguraciones", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<ClosingDateConfigVM>(entities);
        }

        // AGREGAR CONFIGURACIÓN
        public Task<RespuestaVM> AgregarConfiguracion(ClosingDateConfigFilterCrud data)
        {
            var parameters = new List<OracleParameter>();
            RespuestaVM entities = new RespuestaVM();
            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_I_CONFIGCIERRE");
            try
            {

                parameters.Add(new OracleParameter("P_NNUMORI", OracleDbType.Int16, data.NNUMORI, ParameterDirection.Input)); // ORIGEN
                parameters.Add(new OracleParameter("P_NCODGRU", OracleDbType.Int16, data.NCODGRU, ParameterDirection.Input)); // INTERFAZ
                parameters.Add(new OracleParameter("P_NMES", OracleDbType.Int16, data.NMES, ParameterDirection.Input)); // MES
                parameters.Add(new OracleParameter("P_NANIO", OracleDbType.Int16, data.NANIO, ParameterDirection.Input)); // AÑO
                parameters.Add(new OracleParameter("P_DFECINI", OracleDbType.Date, data.DFECINI, ParameterDirection.Input)); // FECHA DE INICIO
                parameters.Add(new OracleParameter("P_DFECFIN", OracleDbType.Date, data.DFECFIN, ParameterDirection.Input)); // FECHA DE FIN
                parameters.Add(new OracleParameter("P_DFECCIERRE", OracleDbType.Date, data.DFECCIERRE, ParameterDirection.Input)); // FECHA DE CIERRE

                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int16, data.NBRANCH, ParameterDirection.Input)); // RAMO
                parameters.Add(new OracleParameter("P_NESTADO", OracleDbType.Int16, data.NESTADO, ParameterDirection.Input)); // ESTADO
                parameters.Add(new OracleParameter("P_SCUSUMOD", OracleDbType.NVarchar2, data.SCUSUMOD, ParameterDirection.Input)); // USUARIO MODIFICADOR

                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureInmo_OI(procedure, parameters);
                entities.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                entities.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                LogControl.save("AgregarConfiguracion", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<RespuestaVM>(entities);
        }

        // MODIFICAR CONFIGURACIÓN
        public Task<RespuestaVM> ModificarConfiguracion(ClosingDateConfigFilterCrud data)
        {
            var parameters = new List<OracleParameter>();
            RespuestaVM entities = new RespuestaVM();
            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_U_CONFIGCIERRE");
            try
            {
                parameters.Add(new OracleParameter("P_NIDCCIERRE", OracleDbType.Int16, data.NIDCCIERRE, ParameterDirection.Input)); // CÓDIGO AUTOGENERADO
                parameters.Add(new OracleParameter("P_NNUMORI", OracleDbType.Int16, data.NNUMORI, ParameterDirection.Input)); // ORIGEN
                parameters.Add(new OracleParameter("P_NCODGRU", OracleDbType.Int16, data.NCODGRU, ParameterDirection.Input)); // INTERFAZ
                parameters.Add(new OracleParameter("P_NMES", OracleDbType.Int16, data.NMES, ParameterDirection.Input)); // MES
                parameters.Add(new OracleParameter("P_NANIO", OracleDbType.Int16, data.NANIO, ParameterDirection.Input)); // AÑO
                parameters.Add(new OracleParameter("P_DFECINI", OracleDbType.Date, data.DFECINI, ParameterDirection.Input)); // FECHA DE INICIO
                parameters.Add(new OracleParameter("P_DFECFIN", OracleDbType.Date, data.DFECFIN, ParameterDirection.Input)); // FECHA DE FIN
                parameters.Add(new OracleParameter("P_DFECCIERRE", OracleDbType.Date, data.DFECCIERRE, ParameterDirection.Input)); // FECHA DE CIERRE
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int16, data.NBRANCH, ParameterDirection.Input)); // RAMO
                parameters.Add(new OracleParameter("P_NESTADO", OracleDbType.Int16, data.NESTADO, ParameterDirection.Input)); // ESTADO
                parameters.Add(new OracleParameter("P_SCUSUMOD", OracleDbType.NVarchar2, data.SCUSUMOD, ParameterDirection.Input)); // USUARIO MODIFICADOR

                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureInmo_OI(procedure, parameters);
                entities.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                entities.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                LogControl.save("ModificarConfiguracion", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<RespuestaVM>(entities);
        }

        // ELIMINAR CONFIGURACIÓN
        public Task<RespuestaVM> EliminarConfiguracion(ClosingDateConfigFilterCrud data)
        {
            var parameters = new List<OracleParameter>();
            RespuestaVM entities = new RespuestaVM();
            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_D_CONFIGCIERRE");
            try
            {
                parameters.Add(new OracleParameter("P_NIDCCIERRE", OracleDbType.Int16, data.NIDCCIERRE, ParameterDirection.Input)); // CÓDIGO AUTOGENERADO
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureInmo_OI(procedure, parameters);
                entities.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                entities.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                LogControl.save("EliminarConfiguracion", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<RespuestaVM>(entities);
        }
    }
}