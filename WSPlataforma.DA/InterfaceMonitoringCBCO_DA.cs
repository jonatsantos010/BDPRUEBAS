using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using Oracle.DataAccess.Client;
using WSPlataforma.Util;
using WSPlataforma.Entities.InterfaceMonitoringModel.ViewModel;
using WSPlataforma.Entities.InterfaceMonitoringModel.BindingModel;
using Oracle.DataAccess.Types;
using System.Configuration;
using WSPlataforma.Entities.ModuleReportsModel.BindingModel;


namespace WSPlataforma.DA
{
    public class InterfaceMonitoringCBCO_DA : ConnectionBase
    {
        private readonly string Package = "PKG_OI_CB_VIEWS";

        // ESTADOS DE PROCESO
        public Task<EstadosVM> ListarEstados()
        {
            var parameters = new List<OracleParameter>();
            EstadosVM entities = new EstadosVM();
            entities.combos = new List<EstadosVM.P_TABLE>();
            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_CB_SEL_ESTADO_PROC");
            try
            {
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

        // CABECERA CONTROL BANCARIO
        public Task<InterfaceMonitoring_CB_VM> ListarCabeceraInterfaces_CBCO(InterfaceMonitoringFilter data)
        {
            var parameters = new List<OracleParameter>();
            InterfaceMonitoring_CB_VM entities = new InterfaceMonitoring_CB_VM();
            entities.lista = new List<InterfaceMonitoring_CB_VM.P_TABLE>();
            string STATUS_CONV;
            if (data.NSTATUS_SEND == "null")
            {
                STATUS_CONV = null;
            }
            else
            {
                STATUS_CONV = data.NSTATUS_SEND;
            }
            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_CB_SEL_PROCESS_CAB");
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

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureOnlineDB(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        InterfaceMonitoring_CB_VM.P_TABLE suggest = new InterfaceMonitoring_CB_VM.P_TABLE();
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
                        suggest.NSTATUS_CBCO = reader["NSTATUS_CBCO"] == DBNull.Value ? 0 : int.Parse(reader["NSTATUS_CBCO"].ToString());
                        suggest.SSTATUS_CBCO = reader["SSTATUS_CBCO"] == DBNull.Value ? string.Empty : reader["SSTATUS_CBCO"].ToString();
                        entities.lista.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarCabeceraInterfaces_CBCO", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<InterfaceMonitoring_CB_VM>(entities);
        }

        // CABECERA POR RECIBO (BUSQUEDA)
        public Task<InterfaceMonitoring_CB_VM> ListarCabeceraInterfacesCBCORecibo(InterfaceMonitoringReciboFilter data)
        {
            var parameters = new List<OracleParameter>();
            InterfaceMonitoring_CB_VM entities = new InterfaceMonitoring_CB_VM();
            entities.lista = new List<InterfaceMonitoring_CB_VM.P_TABLE>();
            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_CB_SEL_PROCESS_CAB_REC"); 
            try
            {
                parameters.Add(new OracleParameter("P_NRECEIPT", OracleDbType.Long, data.NRECEIPT, ParameterDirection.Input));

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
                        InterfaceMonitoring_CB_VM.P_TABLE suggest = new InterfaceMonitoring_CB_VM.P_TABLE();
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
                        suggest.NSTATUS_CBCO = reader["NSTATUS_CBCO"] == DBNull.Value ? 0 : int.Parse(reader["NSTATUS_CBCO"].ToString());
                        suggest.SSTATUS_CBCO = reader["SSTATUS_CBCO"] == DBNull.Value ? string.Empty : reader["SSTATUS_CBCO"].ToString();
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
            return Task.FromResult<InterfaceMonitoring_CB_VM>(entities);
        }

        // DETALLE PRELIMINAR
        public Task<InterfaceMonitoringDetail_CB_VM> ListarDetalleInterfaces(InterfaceMonitoringDetailFilter data)
        {
            var parameters = new List<OracleParameter>();
            InterfaceMonitoringDetail_CB_VM entities = new InterfaceMonitoringDetail_CB_VM();
            entities.lista = new List<InterfaceMonitoringDetail_CB_VM.P_TABLE>();
            string STATUS_CONV;
            if (data.NSTATUS == "null")
            {
                STATUS_CONV = null;
            }
            else
            {
                STATUS_CONV = data.NSTATUS;
            }
            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_CB_SEL_PROCESS_DET");
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

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureOnlineDB(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        InterfaceMonitoringDetail_CB_VM.P_TABLE suggest = new InterfaceMonitoringDetail_CB_VM.P_TABLE();
                        suggest.NIDPROCDET = reader["NIDPROCDET"] == DBNull.Value ? 0 : int.Parse(reader["NIDPROCDET"].ToString());
                        suggest.NPOLICY = reader["NPOLICY"] == DBNull.Value ? string.Empty : reader["NPOLICY"].ToString();
                        suggest.NRECEIPT = reader["NRECEIPT"] == DBNull.Value ? string.Empty : reader["NRECEIPT"].ToString();
                        suggest.NPRODUCT = reader["NPRODUCT"] == DBNull.Value ? 0 : int.Parse(reader["NPRODUCT"].ToString());
                        suggest.PRODUCTO_DES = reader["PRODUCTO_DES"] == DBNull.Value ? string.Empty : reader["PRODUCTO_DES"].ToString();
                        suggest.NMOVCONT = reader["NMOVCONT"] == DBNull.Value ? 0 : int.Parse(reader["NMOVCONT"].ToString());
                        suggest.NTYPE_DES = reader["NTYPE_DES"] == DBNull.Value ? string.Empty : reader["NTYPE_DES"].ToString();
                        suggest.OPERACION = reader["OPERACION"] == DBNull.Value ? string.Empty : reader["OPERACION"].ToString();
                        suggest.DFECOPE = reader["DFECOPE"] == DBNull.Value ? string.Empty : Convert.ToDateTime(reader["DFECOPE"]).ToString("dd/MM/yyyy HH:mm:ss");
                        suggest.CMONEDA_DES = reader["CMONEDA_DES"] == DBNull.Value ? string.Empty : reader["CMONEDA_DES"].ToString();
                        suggest.NMONOPE = reader["NMONOPE"] == DBNull.Value ? 0 : double.Parse(reader["NMONOPE"].ToString());
                        suggest.CBANCO_DES = reader["CBANCO_DES"] == DBNull.Value ? string.Empty : reader["CBANCO_DES"].ToString();
                        suggest.CCTABCO = reader["CCTABCO"] == DBNull.Value ? string.Empty : reader["CCTABCO"].ToString();
                        suggest.NSTATUS = reader["NSTATUS"] == DBNull.Value ? 0 : int.Parse(reader["NSTATUS"].ToString());
                        suggest.SDESCRIPT = reader["SDESCRIPT"] == DBNull.Value ? string.Empty : reader["SDESCRIPT"].ToString();
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
            return Task.FromResult<InterfaceMonitoringDetail_CB_VM>(entities);
        }
        public Task<InterfacegDetailOperationVM> ListarDetalleOperacion(InterfaceMonitoringDetailOperacionFilter data)
        {
            var parameters = new List<OracleParameter>();
            InterfacegDetailOperationVM entities = new InterfacegDetailOperationVM();
            entities.lista = new List<InterfacegDetailOperationVM.P_TABLE>();
            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_CB_SEL_PROCESS_DET_ABONOS");
            try
            {
                parameters.Add(new OracleParameter("P_NIDPROCDET", OracleDbType.Long, data.NIDPROCDET, ParameterDirection.Input));

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
        public DataSet GetDataReport(InterfaceMonitoringDetailXLSXFilter_CB data)
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
                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleOnlineDB"].ToString()))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = cn;
                        cmd.CommandText = string.Format("{0}.{1}", Package, "USP_OI_CB_SEL_PROCESS_DET_XLS");
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("P_NIDPROCESS", OracleDbType.Int32).Value = data.NIDPROCESS;
                        cmd.Parameters.Add("P_NSTATUS", OracleDbType.Int32).Value = STATUS_CONV;
                        cmd.Parameters.Add("P_NRECEIPT", OracleDbType.Long).Value = RECIBO_CONV;
                        cmd.Parameters.Add("P_NTIPO", OracleDbType.Int32).Value = data.NTIPO;
                        cmd.Parameters.Add("C_TABLE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("C_TABLE_2", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                        var P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);
                        var P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                        P_NCODE.Size = 4000;
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
                        ModuleReportsDA moduleDA = new ModuleReportsDA();

                        //OBTIENE CONFIGURACION DE REPORTE
                        dataRpteXLS.P_NNUMORI = 1;
                        dataRpteXLS.P_NREPORT = 4; //preliminar error 
                        dataRpteXLS.P_NTIPO = 1;   // RESUMEN
                        DataTable configRES = moduleDA.GetDataFields(dataRpteXLS);

                        dataRpteXLS.P_NTIPO = 2;   // DETALLE
                        DataTable configDET = moduleDA.GetDataFields(dataRpteXLS);

                        List<ConfigFields> configurationCAB = moduleDA.GetFieldsConfiguration(configRES, dataRpteXLS);
                        List<ConfigFields> configurationDET = moduleDA.GetFieldsConfiguration(configDET, dataRpteXLS);

                        string pathReport = ELog.obtainConfig("reportOnlinePrelError");
                        string fileName = "Reporte_Detalle_PreliminarCB - " + data.NIDPROCESS + ".xlsx";
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

        // DETALLE ASIENTOS
        public Task<InterfaceMonitoringAsientosContablesDetail_CB_VM> ListarDetalleInterfacesAsientosContables(InterfaceMonitoringAsientosContablesDetailFilter data)
        {
            var parameters = new List<OracleParameter>();
            InterfaceMonitoringAsientosContablesDetail_CB_VM entities = new InterfaceMonitoringAsientosContablesDetail_CB_VM();
            entities.lista = new List<InterfaceMonitoringAsientosContablesDetail_CB_VM.P_TABLE>();
            string STATUS_CONV;
            if (data.NSTATUS == "null")
            {
                STATUS_CONV = null;
            }
            else
            {
                STATUS_CONV = data.NSTATUS;
            }
            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_CB_SEL_PROCESS_ASI_CAB_INT");
            try
            {
                parameters.Add(new OracleParameter("P_NIDPROCESS", OracleDbType.Long, data.NIDPROCESS, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NSTATUS", OracleDbType.Long, STATUS_CONV, ParameterDirection.Input));

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
                        InterfaceMonitoringAsientosContablesDetail_CB_VM.P_TABLE suggest = new InterfaceMonitoringAsientosContablesDetail_CB_VM.P_TABLE();
                        suggest.NIDMOVBCO = reader["NIDMOVBCO"] == DBNull.Value ? 0 : int.Parse(reader["NIDMOVBCO"].ToString());
                        suggest.CCTABCO = reader["CCTABCO"] == DBNull.Value ? string.Empty : reader["CCTABCO"].ToString();
                        suggest.CDOCTYPE = reader["CDOCTYPE"] == DBNull.Value ? string.Empty : reader["CDOCTYPE"].ToString();
                        suggest.DFECCON = reader["DFECCON"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["DFECCON"].ToString());
                        suggest.SCONCEPTO = reader["SCONCEPTO"] == DBNull.Value ? string.Empty : reader["SCONCEPTO"].ToString();
                        suggest.SCURRENCY_DES = reader["SCURRENCY_DES"] == DBNull.Value ? string.Empty : reader["SCURRENCY_DES"].ToString();
                        suggest.NMONTO = reader["NMONTO"] == DBNull.Value ? 0 : double.Parse(reader["NMONTO"].ToString());
                        suggest.NSTATUS = reader["NSTATUS"] == DBNull.Value ? 0 : int.Parse(reader["NSTATUS"].ToString());
                        suggest.ASIENTOS = reader["ASIENTOS"] == DBNull.Value ? string.Empty : reader["ASIENTOS"].ToString();
                        suggest.CNUMEXA = reader["CNUMEXA"] == DBNull.Value ? string.Empty : reader["CNUMEXA"].ToString();
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
            return Task.FromResult<InterfaceMonitoringAsientosContablesDetail_CB_VM>(entities);
        }
        public Task<InterfaceMonitoringAsientosContablesDetailAsiento_CB_VM> ListarDetalleInterfacesAsientosContablesAsiento(InterfaceMonitoringAsientosContablesDetailAsiento_CB_Filter data)
        {
            var parameters = new List<OracleParameter>();
            InterfaceMonitoringAsientosContablesDetailAsiento_CB_VM entities = new InterfaceMonitoringAsientosContablesDetailAsiento_CB_VM();
            entities.lista = new List<InterfaceMonitoringAsientosContablesDetailAsiento_CB_VM.P_TABLE>();
            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_CB_SEL_PROCESS_ASI_DET_INT");
            try
            {
                parameters.Add(new OracleParameter("P_NIDCBTMOVBCO", OracleDbType.Long, data.NIDCBTMOVBCO, ParameterDirection.Input));

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
                        InterfaceMonitoringAsientosContablesDetailAsiento_CB_VM.P_TABLE suggest = new InterfaceMonitoringAsientosContablesDetailAsiento_CB_VM.P_TABLE();
                        suggest.NIDMOVBCO = reader["NIDMOVBCO"] == DBNull.Value ? 0 : int.Parse(reader["NIDMOVBCO"].ToString());
                        suggest.NOPERACION = reader["NOPERACION"] == DBNull.Value ? string.Empty : reader["NOPERACION"].ToString();
                        suggest.CREFERE = reader["CREFERE"] == DBNull.Value ? string.Empty : reader["CREFERE"].ToString();
                        suggest.CCENCOS = reader["CCENCOS"] == DBNull.Value ? string.Empty : reader["CCENCOS"].ToString();
                        suggest.CCODCTA = reader["CCODCTA"] == DBNull.Value ? string.Empty : reader["CCODCTA"].ToString();
                        suggest.NMONTO = reader["NMONTO"] == DBNull.Value ? string.Empty : reader["NMONTO"].ToString();
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
            return Task.FromResult<InterfaceMonitoringAsientosContablesDetailAsiento_CB_VM>(entities);
        }
        public Task<InterfaceMonitoringAsientosContablesDetailError_CB_VM> ListarDetalleInterfacesAsientosContablesError(InterfaceMonitoringAsientosContablesDetailError_CB_Filter data)
        {
            var parameters = new List<OracleParameter>();
            InterfaceMonitoringAsientosContablesDetailError_CB_VM entities = new InterfaceMonitoringAsientosContablesDetailError_CB_VM();
            entities.lista = new List<InterfaceMonitoringAsientosContablesDetailError_CB_VM.P_TABLE>();
            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_CB_SEL_PROCESS_ASI_ERR_INT");
            try
            {
                parameters.Add(new OracleParameter("P_NIDPROCESS", OracleDbType.Long, data.NIDPROCESS, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NIDCBTMOVBCO", OracleDbType.Long, data.NIDCBTMOVBCO, ParameterDirection.Input));

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
                        InterfaceMonitoringAsientosContablesDetailError_CB_VM.P_TABLE suggest = new InterfaceMonitoringAsientosContablesDetailError_CB_VM.P_TABLE();
                        suggest.NIDMOVBCO = reader["NIDMOVBCO"] == DBNull.Value ? 0 : int.Parse(reader["NIDMOVBCO"].ToString());
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
            return Task.FromResult<InterfaceMonitoringAsientosContablesDetailError_CB_VM>(entities);
        }

        // DETALLE EXACTUS
        public Task<InterfaceMonitoringExactusDetail_CB_VM> ListarDetalleInterfacesExactus(InterfaceMonitoringExactusDetailFilter data)
        {
            var parameters = new List<OracleParameter>();
            InterfaceMonitoringExactusDetail_CB_VM entities = new InterfaceMonitoringExactusDetail_CB_VM();
            entities.lista = new List<InterfaceMonitoringExactusDetail_CB_VM.P_TABLE>();
            string STATUS_CONV;
            if (data.NSTATUS == "null")
            {
                STATUS_CONV = null;
            }
            else
            {
                STATUS_CONV = data.NSTATUS;
            }
            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_CB_SEL_PROCESS_ASI_CAB");
            try
            {
                parameters.Add(new OracleParameter("P_NIDPROCESS", OracleDbType.Long, data.NIDPROCESS, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NSTATUS_SEND", OracleDbType.Long, STATUS_CONV, ParameterDirection.Input));

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
                        InterfaceMonitoringExactusDetail_CB_VM.P_TABLE suggest = new InterfaceMonitoringExactusDetail_CB_VM.P_TABLE();
                        suggest.NIDMOVBCO = reader["NIDMOVBCO"] == DBNull.Value ? 0 : int.Parse(reader["NIDMOVBCO"].ToString());
                        suggest.CCTABCO = reader["CCTABCO"] == DBNull.Value ? string.Empty : reader["CCTABCO"].ToString();
                        suggest.CDOCTYPE = reader["CDOCTYPE"] == DBNull.Value ? string.Empty : reader["CDOCTYPE"].ToString();
                        suggest.DFECCON = reader["DFECCON"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["DFECCON"].ToString());
                        suggest.SCONCEPTO = reader["SCONCEPTO"] == DBNull.Value ? string.Empty : reader["SCONCEPTO"].ToString();
                        suggest.SCURRENCY_DES = reader["SCURRENCY_DES"] == DBNull.Value ? string.Empty : reader["SCURRENCY_DES"].ToString();
                        suggest.NMONTO = reader["NMONTO"] == DBNull.Value ? 0 : double.Parse(reader["NMONTO"].ToString());
                        suggest.NSTATUS_SEND = reader["NSTATUS_SEND"] == DBNull.Value ? 0 : int.Parse(reader["NSTATUS_SEND"].ToString());
                        suggest.EXACTUS = reader["EXACTUS"] == DBNull.Value ? string.Empty : reader["EXACTUS"].ToString();
                        suggest.CNUMEXA = reader["CNUMEXA"] == DBNull.Value ? string.Empty : reader["CNUMEXA"].ToString();
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
            return Task.FromResult<InterfaceMonitoringExactusDetail_CB_VM>(entities);
        }
        public Task<InterfaceMonitoringExactusDetailAsiento_CB_VM> ListarDetalleInterfacesExactusAsiento(InterfaceMonitoringExactusDetailAsiento_CB_Filter data)
        {
            var parameters = new List<OracleParameter>();
            InterfaceMonitoringExactusDetailAsiento_CB_VM entities = new InterfaceMonitoringExactusDetailAsiento_CB_VM();
            entities.lista = new List<InterfaceMonitoringExactusDetailAsiento_CB_VM.P_TABLE>();
            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_CB_SEL_PROCESS_ASI_DET");
            try
            {
                parameters.Add(new OracleParameter("P_NIDCBTMOVBCO", OracleDbType.Long, data.NIDCBTMOVBCO, ParameterDirection.Input));

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
                        InterfaceMonitoringExactusDetailAsiento_CB_VM.P_TABLE suggest = new InterfaceMonitoringExactusDetailAsiento_CB_VM.P_TABLE();
                        suggest.NIDMOVBCO = reader["NIDMOVBCO"] == DBNull.Value ? 0 : int.Parse(reader["NIDMOVBCO"].ToString());
                        suggest.SFUENTE = reader["SFUENTE"] == DBNull.Value ? string.Empty : reader["SFUENTE"].ToString();
                        suggest.CREFERE = reader["CREFERE"] == DBNull.Value ? string.Empty : reader["CREFERE"].ToString();
                        suggest.CCENCOS = reader["CCENCOS"] == DBNull.Value ? string.Empty : reader["CCENCOS"].ToString();
                        suggest.CCODCTA = reader["CCODCTA"] == DBNull.Value ? string.Empty : reader["CCODCTA"].ToString();
                        suggest.NMONTO = reader["NMONTO"] == DBNull.Value ? string.Empty : reader["NMONTO"].ToString();
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
            return Task.FromResult<InterfaceMonitoringExactusDetailAsiento_CB_VM>(entities);
        }
        public Task<InterfaceMonitoringExactusDetailError_CB_VM> ListarDetalleInterfacesExactusError(InterfaceMonitoringExactusDetailError_CB_Filter data)
        {
            var parameters = new List<OracleParameter>();
            InterfaceMonitoringExactusDetailError_CB_VM entities = new InterfaceMonitoringExactusDetailError_CB_VM();
            entities.lista = new List<InterfaceMonitoringExactusDetailError_CB_VM.P_TABLE>();
            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_CB_SEL_PROCESS_ASI_ERR");
            try
            {
                parameters.Add(new OracleParameter("P_NIDPROCESS", OracleDbType.Long, data.NIDPROCESS, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NIDCBTMOVBCO", OracleDbType.Long, data.NIDCBTMOVBCO, ParameterDirection.Input));

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
                        InterfaceMonitoringExactusDetailError_CB_VM.P_TABLE suggest = new InterfaceMonitoringExactusDetailError_CB_VM.P_TABLE();
                        suggest.NIDMOVBCO = reader["NIDMOVBCO"] == DBNull.Value ? 0 : int.Parse(reader["NIDMOVBCO"].ToString());
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
            return Task.FromResult<InterfaceMonitoringExactusDetailError_CB_VM>(entities);
        }

        // ÓRDENES DE PAGO
        public Task<InterfaceMonitoringAsientosContablesDetail_CB_VM_OP> ListarDetalleInterfacesAsientosContablesOP(InterfaceMonitoringAsientosContablesDetailFilter data)
        {
            var parameters = new List<OracleParameter>();
            InterfaceMonitoringAsientosContablesDetail_CB_VM_OP entities = new InterfaceMonitoringAsientosContablesDetail_CB_VM_OP();
            entities.lista = new List<InterfaceMonitoringAsientosContablesDetail_CB_VM_OP.P_TABLE>();
            string STATUS_CONV;
            if (data.NSTATUS == "null")
            {
                STATUS_CONV = null;
            }
            else
            {
                STATUS_CONV = data.NSTATUS;
            }
            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_CB_OP_SEL_PROCESS_ASI_CAB_INT");
            try
            {
                parameters.Add(new OracleParameter("P_NIDPROCESS", OracleDbType.Long, data.NIDPROCESS, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NSTATUS", OracleDbType.Long, STATUS_CONV, ParameterDirection.Input));

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
                        InterfaceMonitoringAsientosContablesDetail_CB_VM_OP.P_TABLE suggest = new InterfaceMonitoringAsientosContablesDetail_CB_VM_OP.P_TABLE();
                        suggest.NIDCBTMOVBCO = reader["NIDCBTMOVBCO"] == DBNull.Value ? string.Empty : reader["NIDCBTMOVBCO"].ToString();
                        suggest.SCLAIM_MEMO = reader["SCLAIM_MEMO"] == DBNull.Value ? string.Empty : reader["SCLAIM_MEMO"].ToString();
                        suggest.NMOVCONT = reader["NMOVCONT"] == DBNull.Value ? string.Empty : reader["NMOVCONT"].ToString();
                        suggest.MCT_CDESCRI = reader["MCT_CDESCRI"] == DBNull.Value ? string.Empty : reader["MCT_CDESCRI"].ToString();
                        suggest.DFECCON = reader["DFECCON"] == DBNull.Value ? string.Empty : reader["DFECCON"].ToString();
                        suggest.DETALLE_ASIENTO = reader["DETALLE_ASIENTO"] == DBNull.Value ? string.Empty : reader["DETALLE_ASIENTO"].ToString();
                        suggest.NSTATUS_ASI = reader["NSTATUS_ASI"] == DBNull.Value ? 0 : int.Parse(reader["NSTATUS_ASI"].ToString());
                        suggest.ASIENTOS = reader["ASIENTOS"] == DBNull.Value ? string.Empty : reader["ASIENTOS"].ToString();
                        suggest.DETALLE_ERROR = reader["DETALLE_ERROR"] == DBNull.Value ? string.Empty : reader["DETALLE_ERROR"].ToString();
                        suggest.IS_SELECTED = false;
                        entities.lista.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarDetalleInterfacesAsientosContablesOP", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<InterfaceMonitoringAsientosContablesDetail_CB_VM_OP>(entities);
        }
        public Task<InterfaceMonitoringAsientosContablesDetailAsiento_CB_VM_OP> ListarDetalleInterfacesAsientosContablesAsientoOP(InterfaceMonitoringAsientosContablesDetailAsiento_CB_Filter data)
        {
            var parameters = new List<OracleParameter>();
            InterfaceMonitoringAsientosContablesDetailAsiento_CB_VM_OP entities = new InterfaceMonitoringAsientosContablesDetailAsiento_CB_VM_OP();
            entities.lista = new List<InterfaceMonitoringAsientosContablesDetailAsiento_CB_VM_OP.P_TABLE>();
            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_CB_OP_SEL_PROCESS_ASI_DET_INT");
            try
            {
                parameters.Add(new OracleParameter("P_NIDPROCESS", OracleDbType.Long, data.NIDPROCESS, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NIDCBTMOVBCO", OracleDbType.Long, data.NIDCBTMOVBCO, ParameterDirection.Input));

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
                        InterfaceMonitoringAsientosContablesDetailAsiento_CB_VM_OP.P_TABLE suggest = new InterfaceMonitoringAsientosContablesDetailAsiento_CB_VM_OP.P_TABLE();
                        suggest.NIDCBTMOVBCO = reader["NIDCBTMOVBCO"] == DBNull.Value ? 0 : int.Parse(reader["NIDCBTMOVBCO"].ToString());
                        suggest.CCENCOS = reader["CCENCOS"] == DBNull.Value ? string.Empty : reader["CCENCOS"].ToString();
                        suggest.CCODCTA = reader["CCODCTA"] == DBNull.Value ? string.Empty : reader["CCODCTA"].ToString();
                        suggest.NCURRENCY = reader["CCENCOS"] == DBNull.Value ? string.Empty : reader["NCURRENCY"].ToString();
                        suggest.SCURRENCY_DES = reader["CCODCTA"] == DBNull.Value ? string.Empty : reader["SCURRENCY_DES"].ToString();
                        suggest.NMONTO = reader["NMONTO"] == DBNull.Value ? string.Empty : reader["NMONTO"].ToString();
                        entities.lista.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarDetalleInterfacesAsientosContablesAsientoOP", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<InterfaceMonitoringAsientosContablesDetailAsiento_CB_VM_OP>(entities);
        }
        public Task<InterfaceMonitoringExactusDetail_CB_VM_OP> ListarDetalleInterfacesExactusOP(InterfaceMonitoringExactusDetailFilter data)
        {
            var parameters = new List<OracleParameter>();
            InterfaceMonitoringExactusDetail_CB_VM_OP entities = new InterfaceMonitoringExactusDetail_CB_VM_OP();
            entities.lista = new List<InterfaceMonitoringExactusDetail_CB_VM_OP.P_TABLE>();
            string STATUS_CONV;
            if (data.NSTATUS == "null")
            {
                STATUS_CONV = null;
            }
            else
            {
                STATUS_CONV = data.NSTATUS;
            }
            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_CB_OP_SEL_PROCESS_ASI_CAB");
            try
            {
                parameters.Add(new OracleParameter("P_NIDPROCESS", OracleDbType.Long, data.NIDPROCESS, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NSTATUS_SEND", OracleDbType.Long, STATUS_CONV, ParameterDirection.Input));

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
                        InterfaceMonitoringExactusDetail_CB_VM_OP.P_TABLE suggest = new InterfaceMonitoringExactusDetail_CB_VM_OP.P_TABLE();
                        suggest.NIDCBTMOVBCO = reader["NIDCBTMOVBCO"] == DBNull.Value ? 0 : int.Parse(reader["NIDCBTMOVBCO"].ToString());
                        suggest.SCLAIM_MEMO = reader["SCLAIM_MEMO"] == DBNull.Value ? string.Empty : reader["SCLAIM_MEMO"].ToString();
                        suggest.NMOVCONT = reader["NMOVCONT"] == DBNull.Value ? string.Empty : reader["NMOVCONT"].ToString();
                        suggest.MCT_CDESCRI = reader["MCT_CDESCRI"] == DBNull.Value ? string.Empty : reader["MCT_CDESCRI"].ToString();
                        suggest.DFECCON = reader["DFECCON"] == DBNull.Value ? string.Empty : reader["DFECCON"].ToString();
                        suggest.DETALLE_ASIENTO = reader["DETALLE_ASIENTO"] == DBNull.Value ? string.Empty : reader["DETALLE_ASIENTO"].ToString();
                        suggest.CNUMEXA = reader["CNUMEXA"] == DBNull.Value ? string.Empty : reader["CNUMEXA"].ToString();
                        suggest.NSTATUS_SEND = reader["NSTATUS_SEND"] == DBNull.Value ? string.Empty : reader["NSTATUS_SEND"].ToString();
                        suggest.ASIENTOS = reader["ASIENTOS"] == DBNull.Value ? string.Empty : reader["ASIENTOS"].ToString();
                        suggest.DETALLE_ERROR = reader["DETALLE_ERROR"] == DBNull.Value ? string.Empty : reader["DETALLE_ERROR"].ToString();
                        suggest.IS_SELECTED = false;
                        entities.lista.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarDetalleInterfacesExactusOP", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<InterfaceMonitoringExactusDetail_CB_VM_OP>(entities);
        }
        public Task<InterfaceMonitoringExactusDetailAsiento_CB_VM_OP> ListarDetalleInterfacesExactusAsientoOP(InterfaceMonitoringExactusDetailAsiento_CB_Filter data)
        {
            var parameters = new List<OracleParameter>();
            InterfaceMonitoringExactusDetailAsiento_CB_VM_OP entities = new InterfaceMonitoringExactusDetailAsiento_CB_VM_OP();
            entities.lista = new List<InterfaceMonitoringExactusDetailAsiento_CB_VM_OP.P_TABLE>();
            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_CB_OP_SEL_PROCESS_ASI_DET");
            try
            {
                parameters.Add(new OracleParameter("P_NIDPROCESS", OracleDbType.Long, data.NIDPROCESS, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NIDCBTMOVBCO", OracleDbType.Long, data.NIDCBTMOVBCO, ParameterDirection.Input));

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
                        InterfaceMonitoringExactusDetailAsiento_CB_VM_OP.P_TABLE suggest = new InterfaceMonitoringExactusDetailAsiento_CB_VM_OP.P_TABLE();
                        suggest.NIDCBTMOVBCO = reader["NIDCBTMOVBCO"] == DBNull.Value ? 0 : int.Parse(reader["NIDCBTMOVBCO"].ToString());
                        suggest.CCENCOS = reader["CCENCOS"] == DBNull.Value ? string.Empty : reader["CCENCOS"].ToString();
                        suggest.CCODCTA = reader["CCODCTA"] == DBNull.Value ? string.Empty : reader["CCODCTA"].ToString();
                        suggest.NCURRENCY = reader["NCURRENCY"] == DBNull.Value ? string.Empty : reader["NCURRENCY"].ToString();
                        suggest.SCURRENCY_DES = reader["SCURRENCY_DES"] == DBNull.Value ? string.Empty : reader["SCURRENCY_DES"].ToString();
                        suggest.NMONTO = reader["NMONTO"] == DBNull.Value ? string.Empty : reader["NMONTO"].ToString();
                        entities.lista.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarDetalleInterfacesExactusAsientoOP", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<InterfaceMonitoringExactusDetailAsiento_CB_VM_OP>(entities);
        }

        //RENTAS
        public Task<RentasOrigenVM> ListarOrigenRentas()
        {
            var parameters = new List<OracleParameter>();
            RentasOrigenVM entities = new RentasOrigenVM();
            entities.P_LIST = new List<RentasOrigenVM.P_TABLE>();
            var procedure = string.Format("{0}.{1}", "PKG_OI_RENTAS", "USP_OI_S_ORIGEN_LST");
            try
            {
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureRentas(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        RentasOrigenVM.P_TABLE suggest = new RentasOrigenVM.P_TABLE();
                        suggest.COD_MAE = reader["COD_MAE"] == DBNull.Value ? 0 : int.Parse(reader["COD_MAE"].ToString());
                        suggest.DESC_MAE = reader["DESC_MAE"] == DBNull.Value ? string.Empty : reader["DESC_MAE"].ToString();
                        entities.P_LIST.Add(suggest);
                    }
                }
                reader.Close();

                entities.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                entities.P_SMESSAGE = P_SMESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarOrigenRentas", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<RentasOrigenVM>(entities);
        }

        public Task<RentasPagoSiniestroVM> ListarPagoSiniestro(ListarPagoSiniestroBM data)
        {
            var parameters = new List<OracleParameter>();
            RentasPagoSiniestroVM entities = new RentasPagoSiniestroVM();
            entities.P_LIST = new List<RentasPagoSiniestroVM.P_TABLE>();
            var procedure = string.Format("{0}.{1}", "PKG_OI_RENTAS", "USP_OI_S_PAGOSIN_LST");
            try
            {
                parameters.Add(new OracleParameter("P_NNUMORI", OracleDbType.Int32, data.P_NNUMORI, ParameterDirection.Input));
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureRentas(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        RentasPagoSiniestroVM.P_TABLE suggest = new RentasPagoSiniestroVM.P_TABLE();
                        suggest.COD_MAE = reader["COD_MAE"] == DBNull.Value ? 0 : int.Parse(reader["COD_MAE"].ToString());
                        suggest.DESC_MAE = reader["DESC_MAE"] == DBNull.Value ? string.Empty : reader["DESC_MAE"].ToString();
                        entities.P_LIST.Add(suggest);
                    }
                }
                reader.Close();

                entities.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                entities.P_SMESSAGE = P_SMESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarPagoSiniestro", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<RentasPagoSiniestroVM>(entities);
        }

        public Task<RentasAprobacionesVM> ListarAprobacionesRentas(RentasAprobacionesBM data)
        {
            var parameters = new List<OracleParameter>();
            RentasAprobacionesVM entities = new RentasAprobacionesVM();
            entities.P_LIST = new List<RentasAprobacionesVM.P_TABLE>();
            var procedure = string.Format("{0}.{1}", "PKG_OI_RENTAS", "USP_OI_S_OP_APROB_LST");
            try
            {
                parameters.Add(new OracleParameter("P_NNUMORI", OracleDbType.Int32, data.P_NNUMORI, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NTYPEOP", OracleDbType.Int32, data.P_NTYPEOP, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DFECINI", OracleDbType.Date, data.P_DFECINI, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DFECFIN", OracleDbType.Date, data.P_DFECFIN, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SPOLIZA", OracleDbType.NVarchar2, data.P_SPOLIZA, ParameterDirection.Input));

                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureRentas(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        RentasAprobacionesVM.P_TABLE suggest = new RentasAprobacionesVM.P_TABLE();
                        suggest.NIDCHEQUE = reader["NIDCHEQUE"] == DBNull.Value ? string.Empty : reader["NIDCHEQUE"].ToString();
                        suggest.DES_NUMORI = reader["DES_NUMORI"] == DBNull.Value ? string.Empty : reader["DES_NUMORI"].ToString();
                        suggest.SNUM_POLIZA = reader["SNUM_POLIZA"] == DBNull.Value ? string.Empty : reader["SNUM_POLIZA"].ToString();
                        suggest.DES_TYPE_IDENBEN = reader["DES_TYPE_IDENBEN"] == DBNull.Value ? string.Empty : reader["DES_TYPE_IDENBEN"].ToString();
                        suggest.SNUM_IDENBEN = reader["SNUM_IDENBEN"] == DBNull.Value ? string.Empty : reader["SNUM_IDENBEN"].ToString();
                        suggest.SNOM_ASEGURADO = reader["SNOM_ASEGURADO"] == DBNull.Value ? string.Empty : reader["SNOM_ASEGURADO"].ToString();
                        suggest.DES_TYPEPAGO = reader["DES_TYPEPAGO"] == DBNull.Value ? string.Empty : reader["DES_TYPEPAGO"].ToString();
                        //suggest.DFEC_PAGO = reader["DFEC_PAGO"] == DBNull.Value ? string.Empty : reader["DFEC_PAGO"].ToString();
                        suggest.DFEC_PAGO = reader["DFEC_PAGO"] == DBNull.Value ? string.Empty : Convert.ToDateTime(reader["DFEC_PAGO"]).ToString("dd/MM/yyyy");
                        suggest.DES_MONEDA = reader["DES_MONEDA"] == DBNull.Value ? string.Empty : reader["DES_MONEDA"].ToString();
                        suggest.NMTO_PENSION = reader["NMTO_PENSION"] == DBNull.Value ? string.Empty : reader["NMTO_PENSION"].ToString();
                        suggest.SNOM_RECEPTOR = reader["SNOM_RECEPTOR"] == DBNull.Value ? string.Empty : reader["SNOM_RECEPTOR"].ToString();
                        suggest.SNOM_BENEFICIA = reader["SNOM_BENEFICIA"] == DBNull.Value ? string.Empty : reader["SNOM_BENEFICIA"].ToString();
                        suggest.IS_SELECTED = false;
                        entities.P_LIST.Add(suggest);
                    }
                }
                reader.Close();

                entities.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                entities.P_SMESSAGE = P_SMESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarAprobacionesRentas", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<RentasAprobacionesVM>(entities);
        }
        public ResponseVM AprobarPago(RentasAprobarPagoBM data)
        {
            var parameters = new List<OracleParameter>();
            ResponseVM entities = new ResponseVM();
            var procedure = string.Format("{0}.{1}", "PKG_OI_RENTAS", "USP_OI_U_OP_APROB_STA");
            try
            {
                parameters.Add(new OracleParameter("P_NIDCHEQUE", OracleDbType.Long, data.P_NIDCHEQUE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SUSR_APROB", OracleDbType.NVarchar2, data.P_SUSR_APROB, ParameterDirection.Input));
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureRentas(procedure, parameters);
                entities.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                entities.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("AprobarPago", ex.ToString(), "3");
                throw;
            }
            return entities;
        }

        public Task<ResponseVM> AprobarPagoList(RentasAprobarPagoListBM data)
        {
            ResponseVM entities = new ResponseVM();
            List<string> cap = new List<string>();
            try
            {
                RentasAprobarPagoBM dataDet = new RentasAprobarPagoBM();
                int canReg = data.P_LIST.Count;
                for (int i = 0; i < canReg; i++)
                {
                    dataDet = data.P_LIST[i];
                    try
                    {
                        ResponseVM entitiesDet = new ResponseVM();
                        entitiesDet = AprobarPago(dataDet);
                        if (entitiesDet.P_NCODE == 1)
                        {
                            cap.Add(dataDet.P_NIDCHEQUE.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        LogControl.save("AprobarPago", ex.ToString(), "3");
                        throw;
                    }
                }
                if (cap.Count > 0)
                {
                    string temp = string.Concat(cap);
                    entities.P_NCODE = 1;
                    entities.P_SMESSAGE = "Hubo errores en los siguientes pagos de pensión: " + temp;
                }
                else
                {
                    entities.P_NCODE = 0;
                    entities.P_SMESSAGE = "Se aprobaron los pagos exitosamente.";
                }
            }
            catch (Exception ex)
            {
                LogControl.save("AprobarPagoList", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<ResponseVM>(entities);
        }
    }
}