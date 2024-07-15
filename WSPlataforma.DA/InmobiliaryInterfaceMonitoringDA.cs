using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using Oracle.DataAccess.Client;
using WSPlataforma.Util;
using WSPlataforma.Entities.InmobiliaryInterfaceMonitoringModel.ViewModel;
using WSPlataforma.Entities.InmobiliaryInterfaceMonitoringModel.BindingModel;
using Oracle.DataAccess.Types;
using System.Configuration;
using WSPlataforma.Entities.InmobiliaryModuleReportsModel.BindingModel;

namespace WSPlataforma.DA
{
    public class InmobiliaryInterfaceMonitoringDA : ConnectionBase
    {
        private readonly string Package = "PKG_ONLINE_INTERFACE";

        // ESTADOS DE PROCESO
        public Task<EstadosVM> ListarEstados(ProcFilter data)
        {
            var parameters = new List<OracleParameter>();
            EstadosVM entities = new EstadosVM();
            entities.combos = new List<EstadosVM.P_TABLE>();
            var procedure = string.Format("{0}.{1}", this.Package, "SPS_OI_SEL_ESTADO_PROC");


            try
            {
                parameters.Add(new OracleParameter("P_VALOR1", OracleDbType.Int32, data.VALOR1, ParameterDirection.Input));

                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo_OI(procedure, parameters);
                
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        EstadosVM.P_TABLE suggest = new EstadosVM.P_TABLE();
                        suggest.NESTADO = reader["NESTADO"] == DBNull.Value ? string.Empty : reader["NESTADO"].ToString();
                        suggest.SESTADO_DES = reader["SESTADO_DES"] == DBNull.Value ? string.Empty : reader["SESTADO_DES"].ToString();
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

        // CABECERA DE INTERFACES
        public Task<InterfaceMonitoringVM> ListarCabeceraInterfaces(InterfaceMonitoringFilter data)
        {
            var parameters = new List<OracleParameter>();
            InterfaceMonitoringVM entities = new InterfaceMonitoringVM();
            entities.lista = new List<InterfaceMonitoringVM.P_TABLE>();
            string STATUS_CONV;
            if (data.NSTATUS_SEND == "null")
            {
                STATUS_CONV = null;
            }
            else
            {
                STATUS_CONV = data.NSTATUS_SEND;
            }
            var procedure = string.Format("{0}.{1}", this.Package, "SPS_OI_SEL_PROCESS_CAB");
            try
            {
                parameters.Add(new OracleParameter("P_NNUMORI", OracleDbType.Int16, data.NNUMORI, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCODGRU", OracleDbType.Int16, data.NCODGRU, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int16, data.NBRANCH, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NSTATUS_SEND", OracleDbType.Varchar2, STATUS_CONV, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DPROCESS_INI", OracleDbType.Date, data.DPROCESS_INI, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DPROCESS_FIN", OracleDbType.Date, data.DPROCESS_FIN, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NIDPROCESS", OracleDbType.Long, data.NIDPROCESS, ParameterDirection.Input));

                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                //var reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo_OI(procedure, parameters);
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo_OI(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        InterfaceMonitoringVM.P_TABLE suggest = new InterfaceMonitoringVM.P_TABLE();
                        suggest.NIDPROCESS = reader["NIDPROCESS"] == DBNull.Value ? 0 : int.Parse(reader["NIDPROCESS"].ToString());
                        suggest.NCODGRU = reader["NCODGRU"] == DBNull.Value ? 0 : int.Parse(reader["NCODGRU"].ToString());
                        suggest.SCODGRU_DES = reader["SCODGRU_DES"] == DBNull.Value ? string.Empty : reader["SCODGRU_DES"].ToString();
                        suggest.NBRANCH = reader["NBRANCH"] == DBNull.Value ? 0 : int.Parse(reader["NBRANCH"].ToString());
                        suggest.SBRANCH_DES = reader["SBRANCH_DES"] == DBNull.Value ? string.Empty : reader["SBRANCH_DES"].ToString();
                        suggest.DPROCESS_INI = reader["DPROCESS_INI"] == DBNull.Value ? string.Empty : Convert.ToDateTime(reader["DPROCESS_INI"]).ToString("dd/MM/yyyy HH:mm:ss");
                        suggest.DPROCESS_FIN = reader["DPROCESS_FIN"] == DBNull.Value ? string.Empty : Convert.ToDateTime(reader["DPROCESS_FIN"]).ToString("dd/MM/yyyy HH:mm:ss");
                        suggest.NSTATUS = reader["NSTATUS"] == DBNull.Value ? 0 : int.Parse(reader["NSTATUS"].ToString());
                        suggest.SSTATUS_DES = reader["SSTATUS_DES"] == DBNull.Value ? string.Empty : reader["SSTATUS_DES"].ToString();
                        suggest.NSTATUS_SEND = reader["NSTATUS_SEND"] == DBNull.Value ? 0 : int.Parse(reader["NSTATUS_SEND"].ToString());
                        suggest.SSTATUS_SEND = reader["SSTATUS_SEND"] == DBNull.Value ? string.Empty : reader["SSTATUS_SEND"].ToString();
                        suggest.SREPROC = reader["SREPROC"] == DBNull.Value ? string.Empty : reader["SREPROC"].ToString();
                        suggest.NSTATUS_ASI = reader["NSTATUS_ASI"] == DBNull.Value ? 0 : int.Parse(reader["NSTATUS_ASI"].ToString());
                        suggest.SSTATUS_ASI = reader["SSTATUS_ASI"] == DBNull.Value ? string.Empty : reader["SSTATUS_ASI"].ToString();
                        suggest.ID_JOB = reader["ID_JOB"] == DBNull.Value ? 0 : long.Parse(reader["ID_JOB"].ToString());
                        suggest.NSTATUS_JOB = reader["NSTATUS_JOB"] == DBNull.Value ? 0 : int.Parse(reader["NSTATUS_JOB"].ToString());
                        entities.lista.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarCabeceraInterfaces", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<InterfaceMonitoringVM>(entities);
        }
        public Task<InterfaceMonitoringReciboVM> ListarCabeceraInterfacesRecibo(InterfaceMonitoringReciboFilter data)
        {
            var parameters = new List<OracleParameter>();
            InterfaceMonitoringReciboVM entities = new InterfaceMonitoringReciboVM();
            entities.lista = new List<InterfaceMonitoringReciboVM.P_TABLE>();
            var procedure = string.Format("{0}.{1}", this.Package, "SPS_OI_SEL_PROCESS_CAB_REC");
            try
            {
                parameters.Add(new OracleParameter("P_NRECEIPT", OracleDbType.Long, data.NRECEIPT, ParameterDirection.Input));

                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                //var reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo_OI(procedure, parameters);
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo_OI(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        InterfaceMonitoringReciboVM.P_TABLE suggest = new InterfaceMonitoringReciboVM.P_TABLE();
                        suggest.NIDPROCESS = reader["NIDPROCESS"] == DBNull.Value ? 0 : int.Parse(reader["NIDPROCESS"].ToString());
                        suggest.NCODGRU = reader["NCODGRU"] == DBNull.Value ? 0 : int.Parse(reader["NCODGRU"].ToString());
                        suggest.SCODGRU_DES = reader["SCODGRU_DES"] == DBNull.Value ? string.Empty : reader["SCODGRU_DES"].ToString();
                        suggest.NBRANCH = reader["NBRANCH"] == DBNull.Value ? 0 : int.Parse(reader["NBRANCH"].ToString());
                        suggest.SBRANCH_DES = reader["SBRANCH_DES"] == DBNull.Value ? string.Empty : reader["SBRANCH_DES"].ToString();
                        suggest.DPROCESS_INI = reader["DPROCESS_INI"] == DBNull.Value ? string.Empty : Convert.ToDateTime(reader["DPROCESS_INI"]).ToString("dd/MM/yyyy HH:mm:ss");
                        suggest.DPROCESS_FIN = reader["DPROCESS_FIN"] == DBNull.Value ? string.Empty : Convert.ToDateTime(reader["DPROCESS_FIN"]).ToString("dd/MM/yyyy HH:mm:ss");
                        suggest.NSTATUS = reader["NSTATUS"] == DBNull.Value ? 0 : int.Parse(reader["NSTATUS"].ToString());
                        suggest.SSTATUS_DES = reader["SSTATUS_DES"] == DBNull.Value ? string.Empty : reader["SSTATUS_DES"].ToString();
                        suggest.NSTATUS_SEND = reader["NSTATUS_SEND"] == DBNull.Value ? 0 : int.Parse(reader["NSTATUS_SEND"].ToString());
                        suggest.SSTATUS_SEND = reader["SSTATUS_SEND"] == DBNull.Value ? string.Empty : reader["SSTATUS_SEND"].ToString();
                        suggest.SREPROC = reader["SREPROC"] == DBNull.Value ? string.Empty : reader["SREPROC"].ToString();
                        suggest.NSTATUS_ASI = reader["NSTATUS_ASI"] == DBNull.Value ? 0 : int.Parse(reader["NSTATUS_ASI"].ToString());
                        suggest.SSTATUS_ASI = reader["SSTATUS_ASI"] == DBNull.Value ? string.Empty : reader["SSTATUS_ASI"].ToString();
                        suggest.ID_JOB = reader["ID_JOB"] == DBNull.Value ? 0 : long.Parse(reader["ID_JOB"].ToString());
                        suggest.NSTATUS_JOB = reader["NSTATUS_JOB"] == DBNull.Value ? 0 : int.Parse(reader["NSTATUS_JOB"].ToString());
                        entities.lista.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarCabeceraInterfacesRecibo", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<InterfaceMonitoringReciboVM>(entities);
        }
      
        // DETALLE PRELIMINAR
        public Task<InterfaceMonitoringDetailVM> ListarDetalleInterfaces(InterfaceMonitoringDetailFilter data)
        {
            var parameters = new List<OracleParameter>();
            InterfaceMonitoringDetailVM entities = new InterfaceMonitoringDetailVM();
            entities.lista = new List<InterfaceMonitoringDetailVM.P_TABLE>();
            string STATUS_CONV;
            if (data.NSTATUS == "null")
            {
                STATUS_CONV = null;
            }
            else
            {
                STATUS_CONV = data.NSTATUS;
            }
            var procedure = string.Format("{0}.{1}", this.Package, "SPS_OI_SEL_PROCESS_DET");
            try
            {
                parameters.Add(new OracleParameter("P_NIDPROCESS", OracleDbType.Int16, data.NIDPROCESS, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NSTATUS", OracleDbType.Int16, STATUS_CONV, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NRECEIPT", OracleDbType.Long, data.NRECEIPT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NTIPO", OracleDbType.Int16, data.NTIPO, ParameterDirection.Input));

                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                //var reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo_OI(procedure, parameters);
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo_OI(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        InterfaceMonitoringDetailVM.P_TABLE suggest = new InterfaceMonitoringDetailVM.P_TABLE();
                        suggest.NIDPROCDET = reader["NIDPROCDET"] == DBNull.Value ? 0 : int.Parse(reader["NIDPROCDET"].ToString());
                        suggest.NPOLICY = reader["NPOLICY"] == DBNull.Value ? string.Empty : reader["NPOLICY"].ToString();
                        suggest.NRECEIPT = reader["NRECEIPT"] == DBNull.Value ? string.Empty : reader["NRECEIPT"].ToString();
                        suggest.NCURRENCY = reader["NCURRENCY"] == DBNull.Value ? 0 : int.Parse(reader["NCURRENCY"].ToString());
                        suggest.SCURRENCY_DES = reader["SCURRENCY_DES"] == DBNull.Value ? string.Empty : reader["SCURRENCY_DES"].ToString();
                        suggest.NPRODUCT = reader["NPRODUCT"] == DBNull.Value ? 0 : int.Parse(reader["NPRODUCT"].ToString());
                        suggest.PRODUCTO_DES = reader["PRODUCTO_DES"] == DBNull.Value ? string.Empty : reader["PRODUCTO_DES"].ToString();
                        suggest.NTYPE = reader["NTYPE"] == DBNull.Value ? 0 : int.Parse(reader["NTYPE"].ToString());
                        suggest.NTYPE_DES = reader["NTYPE_DES"] == DBNull.Value ? string.Empty : reader["NTYPE_DES"].ToString();
                        suggest.OPERACION = reader["OPERACION"] == DBNull.Value ? string.Empty : reader["OPERACION"].ToString();  // INI MMQ 14-03-2023 FIN
                        suggest.NSTATUS = reader["NSTATUS"] == DBNull.Value ? 0 : int.Parse(reader["NSTATUS"].ToString());
                        suggest.SDESCRIPT = reader["SDESCRIPT"] == DBNull.Value ? string.Empty : reader["SDESCRIPT"].ToString();
                        suggest.ERROR = reader["ERROR"] == DBNull.Value ? string.Empty : reader["ERROR"].ToString();
                        suggest.IS_SELECTED = false;
                        entities.lista.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarDetalleInterfaces", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<InterfaceMonitoringDetailVM>(entities);
        }
        public Task<InterfaceMonitoringDetailErrorVM> ListarErroresDetalle(InterfaceMonitoringDetailErrorFilter data)
        {
            var parameters = new List<OracleParameter>();
            InterfaceMonitoringDetailErrorVM entities = new InterfaceMonitoringDetailErrorVM();
            entities.lista = new List<InterfaceMonitoringDetailErrorVM.P_TABLE>();
            var procedure = string.Format("{0}.{1}", this.Package, "SPS_OI_SEL_PROCESS_DET_ERROR");
            try
            {
                parameters.Add(new OracleParameter("P_NIDPROCESS", OracleDbType.Long, data.NIDPROCESS, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NIDPROCDET", OracleDbType.Long, data.NIDPROCDET, ParameterDirection.Input));

                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                //var reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo_OI(procedure, parameters);
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo_OI(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        InterfaceMonitoringDetailErrorVM.P_TABLE suggest = new InterfaceMonitoringDetailErrorVM.P_TABLE();
                        suggest.NRECEIPT = reader["NRECEIPT"] == DBNull.Value ? string.Empty : reader["NRECEIPT"].ToString();
                        suggest.NTYPE = reader["NTYPE"] == DBNull.Value ? 0 : int.Parse(reader["NTYPE"].ToString());
                        suggest.NERRORNUM = reader["NERRORNUM"] == DBNull.Value ? string.Empty : reader["NERRORNUM"].ToString();
                        suggest.SMESSAGED = reader["SMESSAGED"] == DBNull.Value ? string.Empty : reader["SMESSAGED"].ToString();
                        suggest.SVALOR = reader["SVALOR"] == DBNull.Value ? string.Empty : reader["SVALOR"].ToString();
                        entities.lista.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarErroresDetalle", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<InterfaceMonitoringDetailErrorVM>(entities);
        }

        // DETALLE ASIENTOS
        public Task<InterfaceMonitoringAsientosContablesDetailVM> ListarDetalleInterfacesAsientosContables(InterfaceMonitoringAsientosContablesDetailFilter data)
        {
            var parameters = new List<OracleParameter>();
            InterfaceMonitoringAsientosContablesDetailVM entities = new InterfaceMonitoringAsientosContablesDetailVM();
            entities.lista = new List<InterfaceMonitoringAsientosContablesDetailVM.P_TABLE>();
            string STATUS_CONV;
            if (data.NSTATUS == "null")
            {
                STATUS_CONV = null;
            }
            else
            {
                STATUS_CONV = data.NSTATUS;
            }
            var procedure = string.Format("{0}.{1}", this.Package, "SPS_OI_SEL_PROCESS_ASI_CAB_INT");
            try
            {
                parameters.Add(new OracleParameter("P_NIDPROCESS", OracleDbType.Long, data.NIDPROCESS, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NSTATUS", OracleDbType.Long, STATUS_CONV, ParameterDirection.Input));

                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                //var reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo_OI(procedure, parameters);
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo_OI(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        InterfaceMonitoringAsientosContablesDetailVM.P_TABLE suggest = new InterfaceMonitoringAsientosContablesDetailVM.P_TABLE();
                        suggest.NNUMASI = reader["NNUMASI"] == DBNull.Value ? 0 : int.Parse(reader["NNUMASI"].ToString());
                        suggest.SCLAIM_MEMO = reader["SCLAIM_MEMO"] == DBNull.Value ? string.Empty : reader["SCLAIM_MEMO"].ToString();
                        suggest.NCODCON = reader["NCODCON"] == DBNull.Value ? 0 : int.Parse(reader["NCODCON"].ToString());
                        suggest.AST_CDESCRI = reader["AST_CDESCRI"] == DBNull.Value ? string.Empty : reader["AST_CDESCRI"].ToString();
                        suggest.MCT_CDESCRI = reader["MCT_CDESCRI"] == DBNull.Value ? string.Empty : reader["MCT_CDESCRI"].ToString();
                        suggest.NSTATUS = reader["NSTATUS"] == DBNull.Value ? 0 : int.Parse(reader["NSTATUS"].ToString());
                        suggest.ASIENTOS = reader["ASIENTOS"] == DBNull.Value ? string.Empty : reader["ASIENTOS"].ToString();
                        suggest.DFECCON = reader["DFECCON"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["DFECCON"].ToString());
                        suggest.DETALLE_ASIENTO = reader["DETALLE_ASIENTO"] == DBNull.Value ? string.Empty : reader["DETALLE_ASIENTO"].ToString();
                        suggest.DETALLE_ERROR = reader["DETALLE_ERROR"] == DBNull.Value ? string.Empty : reader["DETALLE_ERROR"].ToString();
                        suggest.IS_SELECTED = false;
                        entities.lista.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarDetalleInterfacesAsientosContables", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<InterfaceMonitoringAsientosContablesDetailVM>(entities);
        }
        public Task<InterfaceMonitoringAsientosContablesDetailAsientoVM> ListarDetalleInterfacesAsientosContablesAsiento(InterfaceMonitoringAsientosContablesDetailAsientoFilter data)
        {
            var parameters = new List<OracleParameter>();
            InterfaceMonitoringAsientosContablesDetailAsientoVM entities = new InterfaceMonitoringAsientosContablesDetailAsientoVM();
            entities.lista = new List<InterfaceMonitoringAsientosContablesDetailAsientoVM.P_TABLE>();
            var procedure = string.Format("{0}.{1}", this.Package, "SPS_OI_SEL_PROCESS_ASI_DET_INT");
            try
            {
                parameters.Add(new OracleParameter("P_NIDPROCESS", OracleDbType.Long, data.NIDPROCESS, ParameterDirection.Input)); // INI MMQ 23-01-2024 RENTAS VITALICIAS -- FIN
                parameters.Add(new OracleParameter("P_NNUMASI", OracleDbType.Long, data.NNUMASI, ParameterDirection.Input));

                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                //var reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo_OI(procedure, parameters);
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo_OI(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        InterfaceMonitoringAsientosContablesDetailAsientoVM.P_TABLE suggest = new InterfaceMonitoringAsientosContablesDetailAsientoVM.P_TABLE();
                        suggest.NNUMASI = reader["NNUMASI"] == DBNull.Value ? 0 : int.Parse(reader["NNUMASI"].ToString());
                        suggest.CCENCOS = reader["CCENCOS"] == DBNull.Value ? string.Empty : reader["CCENCOS"].ToString();
                        suggest.CCODCTA = reader["CCODCTA"] == DBNull.Value ? string.Empty : reader["CCODCTA"].ToString();
                        suggest.CREFERE = reader["CREFERE"] == DBNull.Value ? string.Empty : reader["CREFERE"].ToString();
                        suggest.NMONSOL = reader["NMONSOL"] == DBNull.Value ? string.Empty : reader["NMONSOL"].ToString();
                        suggest.NMONDOL = reader["NMONDOL"] == DBNull.Value ? string.Empty : reader["NMONDOL"].ToString();
                        suggest.VDEBCRE = reader["VDEBCRE"] == DBNull.Value ? string.Empty : reader["VDEBCRE"].ToString();
                        entities.lista.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarDetalleInterfacesAsientosContablesAsiento", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<InterfaceMonitoringAsientosContablesDetailAsientoVM>(entities);
        }
        public Task<InterfaceMonitoringAsientosContablesDetailErrorVM> ListarDetalleInterfacesAsientosContablesError(InterfaceMonitoringAsientosContablesDetailErrorFilter data)
        {
            var parameters = new List<OracleParameter>();
            InterfaceMonitoringAsientosContablesDetailErrorVM entities = new InterfaceMonitoringAsientosContablesDetailErrorVM();
            entities.lista = new List<InterfaceMonitoringAsientosContablesDetailErrorVM.P_TABLE>();
            var procedure = string.Format("{0}.{1}", this.Package, "SPS_OI_SEL_PROCESS_ERR_ASI_INT");
            try
            {
                parameters.Add(new OracleParameter("P_NIDPROCESS", OracleDbType.Long, data.NIDPROCESS, ParameterDirection.Input)); // INI MMQ 23-01-2024 RENTAS VITALICIAS -- FIN
                parameters.Add(new OracleParameter("P_NNUMASI", OracleDbType.Long, data.NNUMASI, ParameterDirection.Input));

                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                //var reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo_OI(procedure, parameters);
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo_OI(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        InterfaceMonitoringAsientosContablesDetailErrorVM.P_TABLE suggest = new InterfaceMonitoringAsientosContablesDetailErrorVM.P_TABLE();
                        suggest.NNUMASI = reader["NNUMASI"] == DBNull.Value ? 0 : int.Parse(reader["NNUMASI"].ToString());
                        suggest.NERRORNUM = reader["NERRORNUM"] == DBNull.Value ? 0 : int.Parse(reader["NERRORNUM"].ToString());
                        suggest.SMESSAGED = reader["SMESSAGED"] == DBNull.Value ? string.Empty : reader["SMESSAGED"].ToString();
                        suggest.SVALOR = reader["SVALOR"] == DBNull.Value ? string.Empty : reader["SVALOR"].ToString();
                        entities.lista.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarDetalleInterfacesAsientosContablesError", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<InterfaceMonitoringAsientosContablesDetailErrorVM>(entities);
        }

        // DETALLE EXACTUS
        public Task<InterfaceMonitoringExactusDetailVM> ListarDetalleInterfacesExactus(InterfaceMonitoringExactusDetailFilter data)
        {
            var parameters = new List<OracleParameter>();
            InterfaceMonitoringExactusDetailVM entities = new InterfaceMonitoringExactusDetailVM();
            entities.lista = new List<InterfaceMonitoringExactusDetailVM.P_TABLE>();
            string STATUS_CONV;
            if (data.NSTATUS == "null")
            {
                STATUS_CONV = null;
            }
            else
            {
                STATUS_CONV = data.NSTATUS;
            }
            var procedure = string.Format("{0}.{1}", this.Package, "SPS_OI_SEL_PROCESS_ASI_CAB");
            try
            {
                parameters.Add(new OracleParameter("P_NIDPROCESS", OracleDbType.Long, data.NIDPROCESS, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NSTATUS", OracleDbType.Long, STATUS_CONV, ParameterDirection.Input));

                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                //var reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo_OI(procedure, parameters);
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo_OI(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        InterfaceMonitoringExactusDetailVM.P_TABLE suggest = new InterfaceMonitoringExactusDetailVM.P_TABLE();
                        suggest.NNUMASI = reader["NNUMASI"] == DBNull.Value ? 0 : int.Parse(reader["NNUMASI"].ToString());
                        suggest.SCLAIM_MEMO = reader["SCLAIM_MEMO"] == DBNull.Value ? string.Empty : reader["SCLAIM_MEMO"].ToString();
                        suggest.NCODCON = reader["NCODCON"] == DBNull.Value ? 0 : int.Parse(reader["NCODCON"].ToString());
                        suggest.AST_CDESCRI = reader["AST_CDESCRI"] == DBNull.Value ? string.Empty : reader["AST_CDESCRI"].ToString();
                        suggest.MCT_CDESCRI = reader["MCT_CDESCRI"] == DBNull.Value ? string.Empty : reader["MCT_CDESCRI"].ToString();
                        suggest.NSTATUS_SEND = reader["NSTATUS_SEND"] == DBNull.Value ? 0 : int.Parse(reader["NSTATUS_SEND"].ToString());
                        suggest.EXACTUS = reader["EXACTUS"] == DBNull.Value ? string.Empty : reader["EXACTUS"].ToString();
                        suggest.CASIEXA = reader["CASIEXA"] == DBNull.Value ? string.Empty : reader["CASIEXA"].ToString();
                        suggest.DFECCON = reader["DFECCON"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["DFECCON"].ToString());
                        suggest.DETALLE_ASIENTO = reader["DETALLE_ASIENTO"] == DBNull.Value ? string.Empty : reader["DETALLE_ASIENTO"].ToString();
                        suggest.DETALLE_ERROR = reader["DETALLE_ERROR"] == DBNull.Value ? string.Empty : reader["DETALLE_ERROR"].ToString();
                        suggest.IS_SELECTED = false;
                        entities.lista.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarDetalleInterfacesExactus", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<InterfaceMonitoringExactusDetailVM>(entities);
        }
        public Task<InterfaceMonitoringExactusDetailAsientoVM> ListarDetalleInterfacesExactusAsiento(InterfaceMonitoringExactusDetailAsientoFilter data)
        {
            var parameters = new List<OracleParameter>();
            InterfaceMonitoringExactusDetailAsientoVM entities = new InterfaceMonitoringExactusDetailAsientoVM();
            entities.lista = new List<InterfaceMonitoringExactusDetailAsientoVM.P_TABLE>();
            var procedure = string.Format("{0}.{1}", this.Package, "SPS_OI_SEL_PROCESS_ASI_DET");
            try
            {
                parameters.Add(new OracleParameter("P_NIDPROCESS", OracleDbType.Long, data.NIDPROCESS, ParameterDirection.Input)); // INI MMQ 23-01-2024 RENTAS VITALICIAS -- FIN
                parameters.Add(new OracleParameter("P_NNUMASI", OracleDbType.Long, data.NNUMASI, ParameterDirection.Input));

                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                //var reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo_OI(procedure, parameters);
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo_OI(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        InterfaceMonitoringExactusDetailAsientoVM.P_TABLE suggest = new InterfaceMonitoringExactusDetailAsientoVM.P_TABLE();
                        suggest.NNUMASI = reader["NNUMASI"] == DBNull.Value ? 0 : int.Parse(reader["NNUMASI"].ToString());
                        suggest.CCENCOS = reader["CCENCOS"] == DBNull.Value ? string.Empty : reader["CCENCOS"].ToString();
                        suggest.CCODCTA = reader["CCODCTA"] == DBNull.Value ? string.Empty : reader["CCODCTA"].ToString();
                        suggest.CREFERE = reader["CREFERE"] == DBNull.Value ? string.Empty : reader["CREFERE"].ToString();
                        suggest.NMONSOL = reader["NMONSOL"] == DBNull.Value ? string.Empty : reader["NMONSOL"].ToString();
                        suggest.NMONDOL = reader["NMONDOL"] == DBNull.Value ? string.Empty : reader["NMONDOL"].ToString();
                        suggest.VDEBCRE = reader["VDEBCRE"] == DBNull.Value ? string.Empty : reader["VDEBCRE"].ToString();
                        entities.lista.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarDetalleInterfacesExactusAsiento", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<InterfaceMonitoringExactusDetailAsientoVM>(entities);
        }
        public Task<InterfaceMonitoringExactusDetailErrorVM> ListarDetalleInterfacesExactusError(InterfaceMonitoringExactusDetailErrorFilter data)
        {
            var parameters = new List<OracleParameter>();
            InterfaceMonitoringExactusDetailErrorVM entities = new InterfaceMonitoringExactusDetailErrorVM();
            entities.lista = new List<InterfaceMonitoringExactusDetailErrorVM.P_TABLE>();
            var procedure = string.Format("{0}.{1}", this.Package, "SPS_OI_SEL_PROCESS_ERR_ASI");
            try
            {
                parameters.Add(new OracleParameter("P_NIDPROCESS", OracleDbType.Long, data.NIDPROCESS, ParameterDirection.Input)); // INI MMQ 23-01-2024 RENTAS VITALICIAS -- FIN
                parameters.Add(new OracleParameter("P_NNUMASI", OracleDbType.Long, data.NNUMASI, ParameterDirection.Input));

                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                //var reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo_OI(procedure, parameters);
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo_OI(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        InterfaceMonitoringExactusDetailErrorVM.P_TABLE suggest = new InterfaceMonitoringExactusDetailErrorVM.P_TABLE();
                        suggest.NNUMASI = reader["NNUMASI"] == DBNull.Value ? 0 : int.Parse(reader["NNUMASI"].ToString());
                        suggest.NERRORNUM = reader["NERRORNUM"] == DBNull.Value ? 0 : int.Parse(reader["NERRORNUM"].ToString());
                        suggest.SMESSAGED = reader["SMESSAGED"] == DBNull.Value ? string.Empty : reader["SMESSAGED"].ToString();
                        suggest.SVALOR = reader["SVALOR"] == DBNull.Value ? string.Empty : reader["SVALOR"].ToString();
                        entities.lista.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarDetalleInterfacesExactusError", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<InterfaceMonitoringExactusDetailErrorVM>(entities);
        }
        
        // TIPO BUSQUEDA
        public Task<TipoBusquedaVM> ListarTipoBusqueda()
        {
            var parameters = new List<OracleParameter>();
            TipoBusquedaVM entities = new TipoBusquedaVM();
            entities.combos = new List<TipoBusquedaVM.P_TABLE>();
            var procedure = string.Format("{0}.{1}", this.Package, "SPS_OI_SEL_TIPO_BUSQUEDA");
            try
            {
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                //var reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo_OI(procedure, parameters);
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo_OI(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        TipoBusquedaVM.P_TABLE suggest = new TipoBusquedaVM.P_TABLE();
                        suggest.CODIGO = reader["CODIGO"] == DBNull.Value ? 0 : int.Parse(reader["CODIGO"].ToString());
                        suggest.DESCRIPCION = reader["DESCRIPCION"] == DBNull.Value ? string.Empty : reader["DESCRIPCION"].ToString();
                        entities.combos.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarTipoBusqueda", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<TipoBusquedaVM>(entities);
        }
        public Task<TipoBusquedaVM> ListarTipoBusquedaSI(TipoBusquedaFilter data)
        {
            var parameters = new List<OracleParameter>();
            TipoBusquedaVM entities = new TipoBusquedaVM();
            entities.combos = new List<TipoBusquedaVM.P_TABLE>();
            var procedure = string.Format("{0}.{1}", this.Package, "SPS_OI_SEL_TIPO_BUSQUEDA_SI");
            try
            {
                parameters.Add(new OracleParameter("P_FLG_TIP", OracleDbType.Int16, data.P_FLG_TIP, ParameterDirection.Input));
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo_OI(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        TipoBusquedaVM.P_TABLE suggest = new TipoBusquedaVM.P_TABLE();
                        suggest.CODIGO = reader["CODIGO"] == DBNull.Value ? 0 : int.Parse(reader["CODIGO"].ToString());
                        suggest.DESCRIPCION = reader["DESCRIPCION"] == DBNull.Value ? string.Empty : reader["DESCRIPCION"].ToString();
                        entities.combos.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarTipoBusquedaSI", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<TipoBusquedaVM>(entities);
        }

        // DETALLE OPERACIONES
        public Task<InterfacegDetailOperationVM> ListarDetalleOperacion(InterfaceMonitoringDetailOperacionFilter data)
        {
            var parameters = new List<OracleParameter>();
            InterfacegDetailOperationVM entities = new InterfacegDetailOperationVM();
            entities.lista = new List<InterfacegDetailOperationVM.P_TABLE>();
            var procedure = string.Format("{0}.{1}", this.Package, "SPS_OI_SEL_PROCESS_DET_ABONOS");
            try
            {
                parameters.Add(new OracleParameter("P_NCODGRU", OracleDbType.Long, data.NCODGRU, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NIDPROCDET", OracleDbType.Long, data.NIDPROCDET, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NMOVCONT", OracleDbType.Long, data.NMOVCONT, ParameterDirection.Input));

                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo_OI(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        InterfacegDetailOperationVM.P_TABLE suggest = new InterfacegDetailOperationVM.P_TABLE();
                        suggest.CNUMERO_OPERACION = reader["CNUMERO_OPERACION"] == DBNull.Value ? string.Empty : reader["CNUMERO_OPERACION"].ToString();
                        suggest.NID_BANCO = reader["NID_BANCO"] == DBNull.Value ? 0 : int.Parse(reader["NID_BANCO"].ToString());
                        suggest.CBANCO_DES = reader["CBANCO_DES"] == DBNull.Value ? string.Empty : reader["CBANCO_DES"].ToString();
                        suggest.DFECHA_DEPOSITO = reader["DFECHA_DEPOSITO"] == DBNull.Value ? string.Empty : Convert.ToDateTime(reader["DFECHA_DEPOSITO"]).ToString("dd/MM/yyyy");
                        suggest.NCURRENCY = reader["NCURRENCY"] == DBNull.Value ? 0 : int.Parse(reader["NCURRENCY"].ToString());
                        suggest.SCURRENCY_DES = reader["SCURRENCY_DES"] == DBNull.Value ? string.Empty : reader["SCURRENCY_DES"].ToString();
                        suggest.NMONTO_BRUTO_DEP = reader["NMONTO_BRUTO_DEP"] == DBNull.Value ? 0 : double.Parse(reader["NMONTO_BRUTO_DEP"].ToString());
                        suggest.CNUMERO_CUENTA_EQ = reader["CNUMERO_CUENTA_EQ"] == DBNull.Value ? string.Empty : reader["CNUMERO_CUENTA_EQ"].ToString();

                        entities.lista.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarDetalleOperacion", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<InterfacegDetailOperationVM>(entities);
        }

        // REPORTES PRELIMINARES
        public DataSet ListarDetalleInterfacesXLSX(InterfaceMonitoringDetailXLSXFilter data)
        {
            DataSet dataReport = new DataSet();
            DataTable dataCab = new DataTable();
            DataTable dataDet = new DataTable();
            int Pcode;
            string Pmessage;

            string STATUS_CONV;
            if (data.NSTATUS == "null")
            {
                STATUS_CONV = null;
            }
            else
            {
                STATUS_CONV = data.NSTATUS;
            }
            long RECIBO_CONV;
            if (data.NRECEIPT == null || data.NRECEIPT == "")
            {
                RECIBO_CONV = 0;
            }
            else
            {
                RECIBO_CONV = long.Parse(data.NRECEIPT);
            }

            try
            {
                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleINMOB_OI"].ToString()))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = cn;
                        cmd.CommandText = string.Format("{0}.{1}", Package, "SPS_OI_SEL_PROCESS_DET_XLS");
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("P_NIDPROCESS", OracleDbType.Int32).Value = data.NIDPROCESS;
                        cmd.Parameters.Add("P_NSTATUS", OracleDbType.Int32).Value = STATUS_CONV;
                        cmd.Parameters.Add("P_NRECEIPT", OracleDbType.Long).Value = RECIBO_CONV;
                        cmd.Parameters.Add("P_NTIPO", OracleDbType.Int32).Value = data.NTIPO;
                        cmd.Parameters.Add("C_TABLE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                        cn.Open();

                        cmd.ExecuteNonQuery();

                        //Obtenemos el cursor de Cabecera
                        OracleRefCursor cursorCab = (OracleRefCursor)cmd.Parameters["C_TABLE"].Value;
                        dataCab.Load(cursorCab.GetDataReader());
                        if (data.NCODGRU == 15 || data.NCODGRU == 16)
                        {
                            dataCab.TableName = "SINIESTRO";
                        }
                        else
                        {
                            dataCab.TableName = "RECIBO";
                        }
                        dataReport.Tables.Add(dataCab);
                        var cantCab = dataCab.Rows.Count;


                        ModuleReportsFilter dataRpteXLS = new ModuleReportsFilter();
                        InmobiliaryModuleReportsDA moduleDA = new InmobiliaryModuleReportsDA();

                        //OBTIENE CONFIGURACION DE REPORTE
                        dataRpteXLS.P_NNUMORI = 1;
                        dataRpteXLS.P_NREPORT = 4; //preliminar error 
                        dataRpteXLS.P_NTIPO = 1;   // RESUMEN
                        dataRpteXLS.P_NCODGRU = data.NCODGRU;
                        DataTable configRES = moduleDA.GetDataFields(dataRpteXLS);


                        List<ConfigFields> configurationCAB = moduleDA.GetFieldsConfiguration(configRES, dataRpteXLS);

                        string pathReport = ELog.obtainConfig("reportOnlinePrelError");
                        string fileName = "RptePreliminarERR - " + data.NIDPROCESS + ".xlsx";
                        moduleDA.ExportToExcelCD(dataReport, configurationCAB, configurationCAB, pathReport, fileName);

                        cn.Close();

                    }
                }
            }
            catch (Exception ex)
            {
                LogControl.save("ListarDetalleInterfacesXLSX", ex.ToString(), "3");
                throw;
            }

            return dataReport;
        }
        public DataSet ListarDetalleInterfacesXLSX_CB(InterfaceMonitoringDetailXLSXFilter_CB data)
        {
            DataSet dataReport = new DataSet();
            DataTable dataCab = new DataTable();
            DataTable dataDet = new DataTable();
            int Pcode;
            string Pmessage;

            string STATUS_CONV;
            if (data.NSTATUS == "null")
            {
                STATUS_CONV = null;
            }
            else
            {
                STATUS_CONV = data.NSTATUS;
            }
            long RECIBO_CONV;
            if (data.NRECEIPT == null || data.NRECEIPT == "")
            {
                RECIBO_CONV = 0;
            }
            else
            {
                RECIBO_CONV = long.Parse(data.NRECEIPT);
            }

            try
            {
                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleINMOB_OI"].ToString()))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = cn;
                        cmd.CommandText = string.Format("{0}.{1}", Package, "SPS_OI_SEL_PROCESS_DET_XLS_COB");
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("P_NIDPROCESS", OracleDbType.Int32).Value = data.NIDPROCESS;
                        cmd.Parameters.Add("P_NSTATUS", OracleDbType.Int32).Value = STATUS_CONV;
                        cmd.Parameters.Add("P_NRECEIPT", OracleDbType.Long).Value = RECIBO_CONV;
                        cmd.Parameters.Add("P_NTIPO", OracleDbType.Int32).Value = data.NTIPO;
                        cmd.Parameters.Add("C_TABLE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("C_TABLE_2", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                        var P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);
                        var P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                        P_NCODE.Size = 2000;
                        P_SMESSAGE.Size = 4000;
                        cmd.Parameters.Add(P_NCODE);
                        cmd.Parameters.Add(P_SMESSAGE);
                        cn.Open();

                        cmd.ExecuteNonQuery();

                        Pcode = Convert.ToInt32(P_NCODE.Value.ToString());
                        Pmessage = P_SMESSAGE.Value.ToString();

                        if (Pcode == 1)
                        {
                            throw new Exception(Pmessage);
                        }

                        //Obtenemos el cursor de Cabecera
                        OracleRefCursor cursorCab = (OracleRefCursor)cmd.Parameters["C_TABLE"].Value;
                        dataCab.Load(cursorCab.GetDataReader());
                        dataCab.TableName = "RECIBO";
                        dataReport.Tables.Add(dataCab);
                        var cantCab = dataCab.Rows.Count;

                        //Obtenemos el cursor de Detalle
                        OracleRefCursor cursorDet = (OracleRefCursor)cmd.Parameters["C_TABLE_2"].Value;
                        dataDet.Load(cursorDet.GetDataReader());
                        dataDet.TableName = "OPERACION";
                        dataReport.Tables.Add(dataDet);
                        var cantDet = dataDet.Rows.Count;

                        ModuleReportsFilter dataRpteXLS = new ModuleReportsFilter();
                        InmobiliaryModuleReportsDA moduleDA = new InmobiliaryModuleReportsDA();

                        //OBTIENE CONFIGURACION DE REPORTE
                        dataRpteXLS.P_NNUMORI = 1;
                        dataRpteXLS.P_NREPORT = 4; //preliminar error 
                        dataRpteXLS.P_NTIPO = 1;   // RECIBOS
                        DataTable configRES = moduleDA.GetDataFields(dataRpteXLS);

                        dataRpteXLS.P_NTIPO = 2;   // OPERACION
                        DataTable configDET = moduleDA.GetDataFields(dataRpteXLS);

                        List<ConfigFields> configurationCAB = moduleDA.GetFieldsConfiguration(configRES, dataRpteXLS);
                        List<ConfigFields> configurationDET = moduleDA.GetFieldsConfiguration(configDET, dataRpteXLS);

                        string pathReport = ELog.obtainConfig("reportOnlinePrelError");
                        string fileName = "RptePreliminarERR - " + data.NIDPROCESS + ".xlsx";
                        moduleDA.ExportToExcelCD(dataReport, configurationDET, configurationCAB, pathReport, fileName);

                        cn.Close();

                    }
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetDataReport", ex.ToString(), "3");
                throw;
            }
            return dataReport;
        }
    }
}