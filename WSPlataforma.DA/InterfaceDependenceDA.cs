using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Oracle.DataAccess.Client;
using WSPlataforma.Util;
using WSPlataforma.Entities.InterfaceDependenceModel.ViewModel;
using WSPlataforma.Entities.InterfaceDependenceModel.BindingModel;

namespace WSPlataforma.DA
{
    public class InterfaceDependenceDA : ConnectionBase
    {
        private readonly string Package = "PKG_ONLINE_INTERFACE";
        private string msgValida;

        // LISTAR DEPENDENCIA DE INTERFACES
        public Task<InterfaceDependenceVM> ListarDependenciasInterfaces(InterfaceDependenceFilter data)
        {
            var parameters = new List<OracleParameter>();
            InterfaceDependenceVM entities = new InterfaceDependenceVM();
            entities.lista = new List<InterfaceDependenceVM.P_TABLE>();
            var procedure = string.Format("{0}.{1}", this.Package, "SPS_OI_SEL_INTERFACE_DEPENDENCE");
            try
            {
                parameters.Add(new OracleParameter("P_NVALOR", OracleDbType.Int16, data.NBRANCH, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NORIGEN", OracleDbType.Int16, data.NNUMORI, ParameterDirection.Input));
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureOnlineDB(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        InterfaceDependenceVM.P_TABLE suggest = new InterfaceDependenceVM.P_TABLE();
                        suggest.NBRANCH = reader["NBRANCH"] == DBNull.Value ? 0 : int.Parse(reader["NBRANCH"].ToString());
                        suggest.SDESCRIPT = reader["SDESCRIPT"] == DBNull.Value ? string.Empty : reader["SDESCRIPT"].ToString();
                        suggest.NCODGRU = reader["NCODGRU"] == DBNull.Value ? 0 : int.Parse(reader["NCODGRU"].ToString());
                        suggest.MVT_CDESCRI_1 = reader["MVT_CDESCRI_1"] == DBNull.Value ? string.Empty : reader["MVT_CDESCRI_1"].ToString();
                        suggest.NCODGRUDEP = reader["NCODGRUDEP"] == DBNull.Value ? 0 : int.Parse(reader["NCODGRUDEP"].ToString());
                        suggest.MVT_CDESCRI_2 = reader["MVT_CDESCRI_2"] == DBNull.Value ? string.Empty : reader["MVT_CDESCRI_2"].ToString();
                        suggest.NNUMORI = reader["NNUMORI"] == DBNull.Value ? 0 : int.Parse(reader["NNUMORI"].ToString());
                        suggest.NASINCRONO = reader["NASINCRONO"] == DBNull.Value ? 0 : int.Parse(reader["NASINCRONO"].ToString());
                        suggest.NSTATUS = reader["NSTATUS"] == DBNull.Value ? 0 : int.Parse(reader["NSTATUS"].ToString());
                        suggest.NREPIT = reader["NREPIT"] == DBNull.Value ? 0 : int.Parse(reader["NREPIT"].ToString());
                        suggest.NTESPERA = reader["NTESPERA"] == DBNull.Value ? 0 : int.Parse(reader["NTESPERA"].ToString());
                        entities.lista.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarDependenciasInterfaces", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<InterfaceDependenceVM>(entities);
        }

        // AGREGAR DEPENDENCIA DE INTERFACES
        public Task<ResponseInterfaceDependenceVM> AgregarDependenciasInterfaces(NewInterfaceDependenceFilter data)
        {
            var parameters = new List<OracleParameter>();
            ResponseInterfaceDependenceVM entities = new ResponseInterfaceDependenceVM();

            string user_modIN;
            if (data.CUSUMOD.Length > 10)
            {
                user_modIN = data.CUSUMOD.ToString().Substring(1, 10);
            }
            else
            {
                user_modIN = data.CUSUMOD;
            }

            int asincIN;
            if (data.NASINCRONO == true)
            {
                asincIN = 1;
            }
            else
            {
                asincIN = 0;
            }

            var procedure = string.Format("{0}.{1}", this.Package, "SPS_OI_NEW_INTERFACE_DEPENDENCE");
            try
            {
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int16, data.NBRANCH, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NNUMORI", OracleDbType.Int16, data.NNUMORI, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCODGRU", OracleDbType.Int16, data.NCODGRU, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCODGRUDEP", OracleDbType.Int16, data.NCODGRUDEP, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NASINCRONO", OracleDbType.Int16, asincIN, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NSTATUS", OracleDbType.Int16, data.NSTATUS, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_CUSUMOD", OracleDbType.NVarchar2, user_modIN, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NREPIT", OracleDbType.Int16, data.NREPIT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NTESPERA", OracleDbType.Int16, data.NTESPERA, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_FLAG", OracleDbType.Int16, data.FLAG, ParameterDirection.Input));
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
                LogControl.save("AgregarDependenciasInterfaces", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<ResponseInterfaceDependenceVM>(entities);
        }

        // ELIMINAR DEPENDENCIA DE INTERFACES
        public Task<ResponseInterfaceDependenceVM> EliminarDependenciasInterfaces(DeleteInterfaceDependenceFilter data)
        {
            var parameters = new List<OracleParameter>();
            ResponseInterfaceDependenceVM entities = new ResponseInterfaceDependenceVM();
            var procedure = string.Format("{0}.{1}", this.Package, "SPS_OI_DEL_INTERFACE_DEPENDENCE");
            try
            {
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int16, data.NBRANCH, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NNUMORI", OracleDbType.Int16, data.NNUMORI, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCODGRU", OracleDbType.Int16, data.NCODGRU, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCODGRUDEP", OracleDbType.Int16, data.NCODGRUDEP, ParameterDirection.Input));

                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureOnlineDB(procedure, parameters);
                entities.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                entities.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);

                // ELIMINAR PRIORIDADES DE INTERFACES
                DeleteInterfacePriorityFilter dataDelete = new DeleteInterfacePriorityFilter();
                dataDelete.NBRANCH = data.NBRANCH;
                dataDelete.NNUMORI = data.NNUMORI;
                try
                {
                    EliminarPrioridadesInterfaces(dataDelete);
                }
                catch (Exception ex)
                {
                    LogControl.save("EliminarDependenciasInterfaces", ex.ToString(), "3");
                    throw;
                }

            }
            catch (Exception ex)
            {
                LogControl.save("EliminarDependenciasInterfaces", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<ResponseInterfaceDependenceVM>(entities);
        }

        // LISTAR PRIORIDADES DE INTERFACES
        public Task<InterfacePriorityVM> ListarPrioridadesInterfaces(InterfacePriorityFilter data)
        {
            var parameters = new List<OracleParameter>();
            InterfacePriorityVM entities = new InterfacePriorityVM();
            entities.lista = new List<InterfacePriorityVM.P_TABLE>();
            var procedure = string.Format("{0}.{1}", this.Package, "SPS_OI_SEL_INTERFACE_PRIORITY");
            try
            {
                parameters.Add(new OracleParameter("P_NVALOR", OracleDbType.Int16, data.NBRANCH, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NORIGEN", OracleDbType.Int16, data.NNUMORI, ParameterDirection.Input));
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureOnlineDB(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        InterfacePriorityVM.P_TABLE suggest = new InterfacePriorityVM.P_TABLE();
                        suggest.NBRANCH = reader["NBRANCH"] == DBNull.Value ? 0 : int.Parse(reader["NBRANCH"].ToString());
                        suggest.SDESCRIPT = reader["SDESCRIPT"] == DBNull.Value ? string.Empty : reader["SDESCRIPT"].ToString();
                        suggest.NCODGRU = reader["NCODGRU"] == DBNull.Value ? 0 : int.Parse(reader["NCODGRU"].ToString());
                        suggest.MVT_CDESCRI_1 = reader["MVT_CDESCRI_1"] == DBNull.Value ? string.Empty : reader["MVT_CDESCRI_1"].ToString();
                        suggest.NCODGRUDEP = reader["NCODGRUDEP"] == DBNull.Value ? 0 : int.Parse(reader["NCODGRUDEP"].ToString());
                        suggest.MVT_CDESCRI_2 = reader["MVT_CDESCRI_2"] == DBNull.Value ? string.Empty : reader["MVT_CDESCRI_2"].ToString();
                        suggest.NNUMORI = reader["NNUMORI"] == DBNull.Value ? 0 : int.Parse(reader["NNUMORI"].ToString());
                        suggest.NORDENP = reader["NORDENP"] == DBNull.Value ? 0 : int.Parse(reader["NORDENP"].ToString());
                        entities.lista.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarPrioridadesInterfaces", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<InterfacePriorityVM>(entities);
        }

        // AGREGAR PRIORIDADES DE INTERFACES
        public Task<ResponseInterfacePriorityVM> AgregarPrioridadesInterfaces(NewInterfacePriorityFilter data)
        {
            var parameters = new List<OracleParameter>();
            ResponseInterfacePriorityVM entities = new ResponseInterfacePriorityVM();

            var procedure = string.Format("{0}.{1}", this.Package, "SPS_OI_NEW_INTERFACE_PRIORITY");
            try
            {
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int16, data.NBRANCH, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NNUMORI", OracleDbType.Int16, data.NNUMORI, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCODGRU", OracleDbType.Int16, data.NCODGRU, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCODGRUDEP", OracleDbType.Int16, data.NCODGRUDEP, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NORDENP", OracleDbType.Int16, data.NORDENP, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_CUSUCRE", OracleDbType.NVarchar2, data.CUSUCRE, ParameterDirection.Input));
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureOnlineDB(procedure, parameters);
                entities.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                entities.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                data.NCODE = entities.P_NCODE;
                data.MSGVAL = entities.P_SMESSAGE;
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("AgregarPrioridadesInterfaces", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<ResponseInterfacePriorityVM>(entities);
        }

        // ELIMINAR PRIORIDADES DE INTERFACES
        public Task<ResponseInterfacePriorityVM> EliminarPrioridadesInterfaces(DeleteInterfacePriorityFilter data)
        {
            var parameters = new List<OracleParameter>();
            ResponseInterfacePriorityVM entities = new ResponseInterfacePriorityVM();
            var procedure = string.Format("{0}.{1}", this.Package, "SPS_OI_DEL_INTERFACE_PRIORITY");
            try
            {
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int16, data.NBRANCH, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NNUMORI", OracleDbType.Int16, data.NNUMORI, ParameterDirection.Input));

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
                LogControl.save("EliminarPrioridadesInterfaces", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<ResponseInterfacePriorityVM>(entities);
        }

        // ELIMINAR E INSERTAR NUEVAS PRIORIDADES
        public Task<ResponseInterfacePriorityVM> EliminarAgregarPrioridadesInterfaces(NewPrioritiesFilter data)
        {
            var parameters = new List<OracleParameter>();
            ResponseInterfacePriorityVM entities = new ResponseInterfacePriorityVM();

            try
            {
                // ELIMINAR PRIORIDADES DE INTERFACES
                DeleteInterfacePriorityFilter dataDelete = new DeleteInterfacePriorityFilter();
                dataDelete.NBRANCH = data.NBRANCH;
                dataDelete.NNUMORI = data.NNUMORI;
                try
                {
                    EliminarPrioridadesInterfaces(dataDelete);
                }
                catch (Exception ex)
                {
                    LogControl.save("EliminarAgregarPrioridadesInterfaces", ex.ToString(), "3");
                    throw;
                }
                string MESSAGE_ERR;
                // AGREGAR PRIORIDADES DE INTERFACES
                NewInterfacePriorityFilter dataNew = new NewInterfacePriorityFilter();
                int canPri = data.PRIORITIES.Count();
                for (int i = 0; i < canPri; i++)
                {
                    dataNew.NBRANCH = data.PRIORITIES[i].NBRANCH;
                    dataNew.NNUMORI = data.PRIORITIES[i].NNUMORI;
                    dataNew.NCODGRU = data.PRIORITIES[i].NCODGRU;
                    dataNew.NCODGRUDEP = data.PRIORITIES[i].NCODGRUDEP;
                    dataNew.NORDENP = i + 1;
                    dataNew.CUSUCRE = data.PRIORITIES[i].CUSUCRE;
                    try
                    {
                        msgValida = string.Empty;
                        AgregarPrioridadesInterfaces(dataNew);
                        entities.P_NCODE = dataNew.NCODE;
                        entities.P_SMESSAGE = dataNew.MSGVAL;
                    }
                    catch (Exception ex)
                    {
                        LogControl.save("EliminarAgregarPrioridadesInterfaces", ex.ToString(), "3");
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                LogControl.save("EliminarAgregarPrioridadesInterfaces", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<ResponseInterfacePriorityVM>(entities);
        }

        // LISTAR DEPENDENCIA DE INTERFACES FILTRADAS
        public Task<InterfaceDependenceVM> ListarDependenciasInterfacesFiltradas(InterfaceDependenceFilter data)
        {
            var parameters = new List<OracleParameter>();
            InterfaceDependenceVM entities = new InterfaceDependenceVM();
            entities.lista = new List<InterfaceDependenceVM.P_TABLE>();
            var procedure = string.Format("{0}.{1}", this.Package, "SPS_OI_SEL_INTERFACE_DEPENDENCE_FILTER");
            try
            {
                parameters.Add(new OracleParameter("P_NVALOR", OracleDbType.Int16, data.NBRANCH, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NORIGEN", OracleDbType.Int16, data.NNUMORI, ParameterDirection.Input));
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureOnlineDB(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        InterfaceDependenceVM.P_TABLE suggest = new InterfaceDependenceVM.P_TABLE();
                        suggest.NBRANCH = reader["NBRANCH"] == DBNull.Value ? 0 : int.Parse(reader["NBRANCH"].ToString());
                        suggest.SDESCRIPT = reader["SDESCRIPT"] == DBNull.Value ? string.Empty : reader["SDESCRIPT"].ToString();
                        suggest.NCODGRU = reader["NCODGRU"] == DBNull.Value ? 0 : int.Parse(reader["NCODGRU"].ToString());
                        suggest.MVT_CDESCRI_1 = reader["MVT_CDESCRI_1"] == DBNull.Value ? string.Empty : reader["MVT_CDESCRI_1"].ToString();
                        suggest.NCODGRUDEP = reader["NCODGRUDEP"] == DBNull.Value ? 0 : int.Parse(reader["NCODGRUDEP"].ToString());
                        suggest.MVT_CDESCRI_2 = reader["MVT_CDESCRI_2"] == DBNull.Value ? string.Empty : reader["MVT_CDESCRI_2"].ToString();
                        suggest.NNUMORI = reader["NNUMORI"] == DBNull.Value ? 0 : int.Parse(reader["NNUMORI"].ToString());
                        suggest.NASINCRONO = reader["NASINCRONO"] == DBNull.Value ? 0 : int.Parse(reader["NASINCRONO"].ToString());
                        suggest.NSTATUS = reader["NSTATUS"] == DBNull.Value ? 0 : int.Parse(reader["NSTATUS"].ToString());
                        suggest.NREPIT = reader["NREPIT"] == DBNull.Value ? 0 : int.Parse(reader["NREPIT"].ToString());
                        suggest.NTESPERA = reader["NTESPERA"] == DBNull.Value ? 0 : int.Parse(reader["NTESPERA"].ToString());
                        entities.lista.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarDependenciasInterfacesFiltradas", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<InterfaceDependenceVM>(entities);
        }
    }
}