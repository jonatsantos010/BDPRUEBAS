using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using Oracle.DataAccess.Client;
using WSPlataforma.Util;
using WSPlataforma.Entities.InterfaceMonitoringTesoModel.ViewModel;
using WSPlataforma.Entities.InterfaceMonitoringTesoModel.BindingModel;
using Oracle.DataAccess.Types;
using System.Configuration;
using WSPlataforma.Entities.ModuleReportsModel.BindingModel;
using System.Diagnostics;

namespace WSPlataforma.DA
{
    public class InterfaceMonitoringTesoDA : ConnectionBase
    {
        private readonly string Package = "PKG_OI_TESORERIA";

        // INI MMQ 06-11-2023 NEW
        public DataSet ListarBankTesoreriaCAB_XLS(InterfaceMonitoringTesoFilter data)
        {
            DataSet dataReport = new DataSet();
            DataTable dataCab = new DataTable();
            DataTable dataDet = new DataTable();
            int Pcode;
            string Pmessage;

            string STATUS_CONV;           
            string BANK_CONV;
            if (data.P_NSTATUS == "null")
            {
                STATUS_CONV = null;
            }
            else
            {
                STATUS_CONV = data.P_NSTATUS;
            }
            if (data.P_NBANK_CODE == "null")
            {
                BANK_CONV = null;
            }
            else
            {
                BANK_CONV = data.P_NBANK_CODE;
            }


            try
            {
                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleOnlineDB"].ToString()))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = cn;
                        cmd.CommandText = string.Format("{0}.{1}", Package, "USP_OI_GET_LIST_PAYBNK_CAB_XLS");
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("P_NNUMORI", OracleDbType.Int32).Value = data.P_NNUMORI;
                        cmd.Parameters.Add("P_NCODGRU", OracleDbType.Int32).Value = data.P_NCODGRU;
                        cmd.Parameters.Add("P_NBANK_CODE", OracleDbType.NVarchar2).Value = BANK_CONV;
                        cmd.Parameters.Add("P_NSTATUS", OracleDbType.NVarchar2).Value = STATUS_CONV;
                        cmd.Parameters.Add("P_DFECINI", OracleDbType.Date).Value = data.P_DFECINI;
                        cmd.Parameters.Add("P_DFECFIN", OracleDbType.Date).Value = data.P_DFECFIN;
                        cmd.Parameters.Add("P_NTIPO", OracleDbType.Int32).Value = data.P_NTIPO;
                        cmd.Parameters.Add("P_SVALOR", OracleDbType.NVarchar2).Value = data.P_SVALOR;
                        cmd.Parameters.Add("P_CTABLE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                        
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
                        OracleRefCursor cursorCab = (OracleRefCursor)cmd.Parameters["P_CTABLE"].Value;
                        dataCab.Load(cursorCab.GetDataReader());
                        dataCab.TableName = "APROBACION PAGOS";
                        dataReport.Tables.Add(dataCab);
                        var cantCab = dataCab.Rows.Count;                        

                        ModuleReportsFilter dataRpteXLS = new ModuleReportsFilter();
                        ModuleReportsDA moduleDA = new ModuleReportsDA();

                        //OBTIENE CONFIGURACION DE REPORTE
                        dataRpteXLS.P_NNUMORI = data.P_NNUMORI;
                        dataRpteXLS.P_NCODGRU = data.P_NCODGRU;
                        dataRpteXLS.P_NREPORT = 7; // CABECERA APROBACION ENVIO PAGO BANCO
                        dataRpteXLS.P_NTIPO = 1;   // RESUMEN
                        DataTable configRES = moduleDA.GetDataFields(dataRpteXLS);
                       
                        List<ConfigFields> configurationCAB = moduleDA.GetFieldsConfiguration(configRES, dataRpteXLS);                        

                        string pathReport = ELog.obtainConfig("reportOnlinePrelError");
                        string fileName = "RptePagoBancoCAB.xlsx";
                        moduleDA.ExportToExcelCD(dataReport, configurationCAB, configurationCAB, pathReport, fileName);

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

        public DataSet ListarBankTesoreriaDET_XLS(InterfaceMonitoringDetTesoFilter data)
        {
            DataSet dataReport = new DataSet();
            DataTable dataCab = new DataTable();
            DataTable dataDet = new DataTable();
            int Pcode;
            string Pmessage;

            string STATUS_CONV;
           
            if (data.P_NSTATUS == "null")
            {
                STATUS_CONV = null;
            }
            else
            {
                STATUS_CONV = data.P_NSTATUS;
            }
           
            try
            {
                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleOnlineDB"].ToString()))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = cn;
                        cmd.CommandText = string.Format("{0}.{1}", Package, "USP_OI_GET_LIST_PAYBNK_DET_XLS");
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("P_NIDPAYBNK", OracleDbType.Int32).Value = data.P_NIDPAYBNK;
                        cmd.Parameters.Add("P_NSTATUS", OracleDbType.NVarchar2).Value = STATUS_CONV;
                        cmd.Parameters.Add("P_STIPO", OracleDbType.Int32).Value = data.P_NTIPO;
                        cmd.Parameters.Add("P_SVALOR", OracleDbType.NVarchar2).Value = data.P_SVALOR;
                        cmd.Parameters.Add("P_CTABLE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

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
                        OracleRefCursor cursorCab = (OracleRefCursor)cmd.Parameters["P_CTABLE"].Value;
                        dataCab.Load(cursorCab.GetDataReader());
                        dataCab.TableName = "APROBACION DETALLE PAGOS";
                        dataReport.Tables.Add(dataCab);
                        var cantCab = dataCab.Rows.Count;

                        ModuleReportsFilter dataRpteXLS = new ModuleReportsFilter();
                        ModuleReportsDA moduleDA = new ModuleReportsDA();

                        //OBTIENE CONFIGURACION DE REPORTE
                        dataRpteXLS.P_NNUMORI = 1;
                        dataRpteXLS.P_NCODGRU = 16;
                        dataRpteXLS.P_NREPORT = 8; // DETALLE APROBACION ENVIO PAGO BANCO
                        dataRpteXLS.P_NTIPO = 1;   // RESUMEN
                        DataTable configRES = moduleDA.GetDataFields(dataRpteXLS);

                        List<ConfigFields> configurationCAB = moduleDA.GetFieldsConfiguration(configRES, dataRpteXLS);

                        string pathReport = ELog.obtainConfig("reportOnlinePrelError");
                        string fileName = "RptePagoBancoDET.xlsx";
                        moduleDA.ExportToExcelCD(dataReport, configurationCAB, configurationCAB, pathReport, fileName);

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
        // FIN MMQ 06-11-2023 NEW


        // TESORERÍA
        public Task<BancosTesoreríaVM> ListarBancosTesoreria()
        {
            var parameters = new List<OracleParameter>();
            BancosTesoreríaVM entities = new BancosTesoreríaVM();
            entities.combos = new List<BancosTesoreríaVM.P_TABLE>();
            var procedure = "PKG_OI_TESORERIA.USP_OI_GET_LIST_BNK_ORI";
            try
            {
                parameters.Add(new OracleParameter("P_CTABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureOnlineDB(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        BancosTesoreríaVM.P_TABLE suggest = new BancosTesoreríaVM.P_TABLE();
                        suggest.NBANK_CODE = reader["NBANK_CODE"] == DBNull.Value ? string.Empty : reader["NBANK_CODE"].ToString();
                        suggest.SBANK_DES = reader["SBANK_DES"] == DBNull.Value ? string.Empty : reader["SBANK_DES"].ToString();
                        entities.combos.Add(suggest);
                    }
                }
                entities.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                entities.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarBancosTesoreria", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<BancosTesoreríaVM>(entities);
        }
        public Task<BuscarPorVM> ListarBuscarPor()
        {
            var parameters = new List<OracleParameter>();
            BuscarPorVM entities = new BuscarPorVM();
            entities.combos = new List<BuscarPorVM.P_TABLE>();
            var procedure = "PKG_OI_TESORERIA.USP_OI_GET_LIST_DOC_IDEM";
            try
            {
                parameters.Add(new OracleParameter("P_CTABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureOnlineDB(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        BuscarPorVM.P_TABLE suggest = new BuscarPorVM.P_TABLE();
                        suggest.NCODE = reader["NCODE"] == DBNull.Value ? 0 : int.Parse(reader["NCODE"].ToString());
                        suggest.BUSCAR_POR = reader["BUSCAR_POR"] == DBNull.Value ? string.Empty : reader["BUSCAR_POR"].ToString();
                        entities.combos.Add(suggest);
                    }
                }
                entities.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                entities.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarBuscarPor", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<BuscarPorVM>(entities);
        }
        public Task<EstadosTesoreríaVM> ListarEstadosTesoreria(EstadosTesoreríaFilter data)
        {
            var parameters = new List<OracleParameter>();
            EstadosTesoreríaVM entities = new EstadosTesoreríaVM();
            entities.combos = new List<EstadosTesoreríaVM.P_TABLE>();
            var procedure = "PKG_OI_TESORERIA.USP_OI_GET_LIST_EST_BCO";
            try
            {
                parameters.Add(new OracleParameter("P_TIPO", OracleDbType.Int16, data.P_TIPO, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_CTABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureOnlineDB(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        EstadosTesoreríaVM.P_TABLE suggest = new EstadosTesoreríaVM.P_TABLE();
                        suggest.NESTADO = reader["NESTADO"] == DBNull.Value ? 0 : int.Parse(reader["NESTADO"].ToString());
                        suggest.SESTADO_DES = reader["SESTADO_DES"] == DBNull.Value ? string.Empty : reader["SESTADO_DES"].ToString();
                        entities.combos.Add(suggest);
                    }
                }
                entities.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                entities.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarEstadosTesoreria", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<EstadosTesoreríaVM>(entities);
        }
        public Task<AprobacionesVM> ListarAprobaciones(AprobacionesFilter data)
        {
            var parameters = new List<OracleParameter>();
            AprobacionesVM entities = new AprobacionesVM();
            entities.lista = new List<AprobacionesVM.P_TABLE>();
            string STATUS_CONV;
            string BANK_CONV;
            if (data.P_NSTATUS == "null")
            {
                STATUS_CONV = null;
            }
            else
            {
                STATUS_CONV = data.P_NSTATUS;
            }
            if (data.P_NBANK_CODE == "null")
            {
                BANK_CONV = null;
            }
            else
            {
                BANK_CONV = data.P_NBANK_CODE;
            }
            var procedure = "PKG_OI_TESORERIA.USP_OI_GET_LIST_PAYBNK_CAB";
            try
            {
                parameters.Add(new OracleParameter("P_NNUMORI", OracleDbType.Int16, data.P_NNUMORI, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCODGRU", OracleDbType.Int16, data.P_NCODGRU, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NBANK_CODE", OracleDbType.NVarchar2, BANK_CONV, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NSTATUS", OracleDbType.NVarchar2, STATUS_CONV, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DFECINI", OracleDbType.Date, data.P_DFECINI, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DFECFIN", OracleDbType.Date, data.P_DFECFIN, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_CTABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureOnlineDB(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        AprobacionesVM.P_TABLE suggest = new AprobacionesVM.P_TABLE();
                        suggest.NIDPAYBNK = reader["NIDPAYBNK"] == DBNull.Value ? 0 : long.Parse(reader["NIDPAYBNK"].ToString());
                        suggest.NIDPROCESS = reader["NIDPROCESS"] == DBNull.Value ? 0 : long.Parse(reader["NIDPROCESS"].ToString());
                        suggest.NNUMORI = reader["NNUMORI"] == DBNull.Value ? 0 : int.Parse(reader["NNUMORI"].ToString());
                        suggest.SORIGEN_DES = reader["SORIGEN_DES"] == DBNull.Value ? string.Empty : reader["SORIGEN_DES"].ToString();
                        suggest.NCODGRU = reader["NCODGRU"] == DBNull.Value ? 0 : int.Parse(reader["NCODGRU"].ToString());
                        suggest.NBRANCH = reader["NBRANCH"] == DBNull.Value ? 0 : int.Parse(reader["NBRANCH"].ToString());
                        suggest.DFEC_INT = reader["DFEC_INT"] == DBNull.Value ? string.Empty : Convert.ToDateTime(reader["DFEC_INT"]).ToString("dd/MM/yyyy HH:mm:ss");
                        suggest.NCURRENCY = reader["NCURRENCY"] == DBNull.Value ? 0 : int.Parse(reader["NCURRENCY"].ToString());
                        suggest.SCURRENCY_DES = reader["SCURRENCY_DES"] == DBNull.Value ? string.Empty : reader["SCURRENCY_DES"].ToString();
                        suggest.CBANK_ORI = reader["CBANK_ORI"] == DBNull.Value ? 0 : int.Parse(reader["CBANK_ORI"].ToString());
                        suggest.SBANK_ORI_DES = reader["SBANK_ORI_DES"] == DBNull.Value ? string.Empty : reader["SBANK_ORI_DES"].ToString();
                        suggest.CTRANSAC = reader["CTRANSAC"] == DBNull.Value ? string.Empty : reader["CTRANSAC"].ToString();
                        suggest.CNUMEXA = reader["CNUMEXA"] == DBNull.Value ? string.Empty : reader["CNUMEXA"].ToString();
                        suggest.NAMOUNT = reader["NAMOUNT"] == DBNull.Value ? 0 : decimal.Parse(reader["NAMOUNT"].ToString());
                        suggest.NSTATUS = reader["NSTATUS"] == DBNull.Value ? 0 : int.Parse(reader["NSTATUS"].ToString());
                        suggest.SSTATUS_DES = reader["SSTATUS_DES"] == DBNull.Value ? string.Empty : reader["SSTATUS_DES"].ToString();
                        suggest.CUSERCODE = reader["CUSERCODE"] == DBNull.Value ? string.Empty : reader["CUSERCODE"].ToString();
                        suggest.DCOMPDATE = reader["DCOMPDATE"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["DCOMPDATE"].ToString());
                        entities.lista.Add(suggest);
                    }
                }
                entities.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                entities.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarAprobaciones", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<AprobacionesVM>(entities);
        }
        public Task<AprobacionesVM> ListarAprobacionesDoc(AprobacionesDocFilter data)
        {
            var parameters = new List<OracleParameter>();
            AprobacionesVM entities = new AprobacionesVM();
            entities.lista = new List<AprobacionesVM.P_TABLE>();
            var procedure = "PKG_OI_TESORERIA.USP_OI_GET_LIST_PAYBNK_CAB_IND";
            try
            {
                parameters.Add(new OracleParameter("P_NTIPO", OracleDbType.Int16, data.P_NTIPO, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SVALOR", OracleDbType.NVarchar2, data.P_SVALOR, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_CTABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureOnlineDB(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        AprobacionesVM.P_TABLE suggest = new AprobacionesVM.P_TABLE();
                        suggest.NIDPAYBNK = reader["NIDPAYBNK"] == DBNull.Value ? 0 : long.Parse(reader["NIDPAYBNK"].ToString());
                        suggest.NIDPROCESS = reader["NIDPROCESS"] == DBNull.Value ? 0 : long.Parse(reader["NIDPROCESS"].ToString());
                        suggest.NNUMORI = reader["NNUMORI"] == DBNull.Value ? 0 : int.Parse(reader["NNUMORI"].ToString());
                        suggest.SORIGEN_DES = reader["SORIGEN_DES"] == DBNull.Value ? string.Empty : reader["SORIGEN_DES"].ToString();
                        suggest.NCODGRU = reader["NCODGRU"] == DBNull.Value ? 0 : int.Parse(reader["NCODGRU"].ToString());
                        suggest.NBRANCH = reader["NBRANCH"] == DBNull.Value ? 0 : int.Parse(reader["NBRANCH"].ToString());
                        suggest.DFEC_INT = reader["DFEC_INT"] == DBNull.Value ? string.Empty : Convert.ToDateTime(reader["DFEC_INT"]).ToString("dd/MM/yyyy HH:mm:ss");
                        suggest.NCURRENCY = reader["NCURRENCY"] == DBNull.Value ? 0 : int.Parse(reader["NCURRENCY"].ToString());
                        suggest.SCURRENCY_DES = reader["SCURRENCY_DES"] == DBNull.Value ? string.Empty : reader["SCURRENCY_DES"].ToString();
                        suggest.CBANK_ORI = reader["CBANK_ORI"] == DBNull.Value ? 0 : int.Parse(reader["CBANK_ORI"].ToString());
                        suggest.SBANK_ORI_DES = reader["SBANK_ORI_DES"] == DBNull.Value ? string.Empty : reader["SBANK_ORI_DES"].ToString();
                        suggest.CTRANSAC = reader["CTRANSAC"] == DBNull.Value ? string.Empty : reader["CTRANSAC"].ToString();
                        suggest.CNUMEXA = reader["CNUMEXA"] == DBNull.Value ? string.Empty : reader["CNUMEXA"].ToString();
                        suggest.NAMOUNT = reader["NAMOUNT"] == DBNull.Value ? 0 : decimal.Parse(reader["NAMOUNT"].ToString());
                        suggest.NSTATUS = reader["NSTATUS"] == DBNull.Value ? 0 : int.Parse(reader["NSTATUS"].ToString());
                        suggest.SSTATUS_DES = reader["SSTATUS_DES"] == DBNull.Value ? string.Empty : reader["SSTATUS_DES"].ToString();
                        suggest.CUSERCODE = reader["CUSERCODE"] == DBNull.Value ? string.Empty : reader["CUSERCODE"].ToString();
                        suggest.DCOMPDATE = reader["DCOMPDATE"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["DCOMPDATE"].ToString());
                        entities.lista.Add(suggest);
                    }
                }
                entities.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                entities.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarAprobacionesDoc", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<AprobacionesVM>(entities);
        }
        public Task<AprobacionesDetalleVM> ListarAprobacionesDetalle(AprobacionesDetalleFilter data)
        {
            var parameters = new List<OracleParameter>();
            AprobacionesDetalleVM entities = new AprobacionesDetalleVM();
            entities.lista = new List<AprobacionesDetalleVM.P_TABLE>();
            string STATUS_CONV;
            if (data.P_NSTATUS == "null")
            {
                STATUS_CONV = null;
            }
            else
            {
                STATUS_CONV = data.P_NSTATUS;
            }
            var procedure = "PKG_OI_TESORERIA.USP_OI_GET_LIST_PAYBNK_DET";
            try
            {
                parameters.Add(new OracleParameter("P_NIDPAYBNK", OracleDbType.Long, data.P_NIDPAYBNK, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NSTATUS", OracleDbType.NVarchar2, STATUS_CONV, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_CTABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureOnlineDB(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        AprobacionesDetalleVM.P_TABLE suggest = new AprobacionesDetalleVM.P_TABLE();
                        suggest.NIDPAYBNKD = reader["NIDPAYBNKD"] == DBNull.Value ? 0 : long.Parse(reader["NIDPAYBNKD"].ToString());
                        suggest.SBEN_DOC_DESC = reader["SBEN_DOC_DESC"] == DBNull.Value ? string.Empty : reader["SBEN_DOC_DESC"].ToString();
                        suggest.SBEN_DOC_NUM = reader["SBEN_DOC_NUM"] == DBNull.Value ? string.Empty : reader["SBEN_DOC_NUM"].ToString();
                        suggest.SBEN_NOM_CLI = reader["SBEN_NOM_CLI"] == DBNull.Value ? string.Empty : reader["SBEN_NOM_CLI"].ToString();
                        suggest.NAMOUNT = reader["NAMOUNT"] == DBNull.Value ? 0 : decimal.Parse(reader["NAMOUNT"].ToString());
                        suggest.CNUMEXA = reader["CNUMEXA"] == DBNull.Value ? string.Empty : reader["CNUMEXA"].ToString();
                        suggest.DFEC_APROB = reader["DFEC_APROB"] == DBNull.Value ? string.Empty : Convert.ToDateTime(reader["DFEC_APROB"]).ToString("dd/MM/yyyy HH:mm:ss");
                        suggest.DFEC_OPER = reader["DFEC_OPER"] == DBNull.Value ? string.Empty : Convert.ToDateTime(reader["DFEC_OPER"]).ToString("dd/MM/yyyy");
                        suggest.NSTATUS = reader["NSTATUS"] == DBNull.Value ? 0 : int.Parse(reader["NSTATUS"].ToString());
                        suggest.SSTATUS_DES = reader["SSTATUS_DES"] == DBNull.Value ? string.Empty : reader["SSTATUS_DES"].ToString();
                        suggest.OBSERVACION = reader["OBSERVACION"] == DBNull.Value ? string.Empty : reader["OBSERVACION"].ToString();
                        suggest.CID_RE_OPEN = reader["CID_RE_OPEN"] == DBNull.Value ? string.Empty : reader["CID_RE_OPEN"].ToString();
                        suggest.CIDOPERBNK = reader["CIDOPERBNK"] == DBNull.Value ? string.Empty : reader["CIDOPERBNK"].ToString();
                        suggest.IS_SELECTED = false;
                        entities.lista.Add(suggest);
                    }
                }
                entities.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                entities.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarAprobacionesDetalle", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<AprobacionesDetalleVM>(entities);
        }
        public Task<AprobacionesDetalleVM> ListarAprobacionesDetalleDoc(AprobacionesDetalleDocFilter data)
        {
            var parameters = new List<OracleParameter>();
            AprobacionesDetalleVM entities = new AprobacionesDetalleVM();
            entities.lista = new List<AprobacionesDetalleVM.P_TABLE>();
            var procedure = "PKG_OI_TESORERIA.USP_OI_GET_LIST_PAYBNK_DET_IND";
            try
            {
                parameters.Add(new OracleParameter("P_NIDPAYBNK", OracleDbType.Long, data.P_NIDPAYBNK, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NTIPO", OracleDbType.Int16, data.P_NTIPO, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SVALOR", OracleDbType.NVarchar2, data.P_SVALOR, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_CTABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureOnlineDB(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        AprobacionesDetalleVM.P_TABLE suggest = new AprobacionesDetalleVM.P_TABLE();
                        suggest.NIDPAYBNKD = reader["NIDPAYBNKD"] == DBNull.Value ? 0 : long.Parse(reader["NIDPAYBNKD"].ToString());
                        suggest.SBEN_DOC_DESC = reader["SBEN_DOC_DESC"] == DBNull.Value ? string.Empty : reader["SBEN_DOC_DESC"].ToString();
                        suggest.SBEN_DOC_NUM = reader["SBEN_DOC_NUM"] == DBNull.Value ? string.Empty : reader["SBEN_DOC_NUM"].ToString();
                        suggest.SBEN_NOM_CLI = reader["SBEN_NOM_CLI"] == DBNull.Value ? string.Empty : reader["SBEN_NOM_CLI"].ToString();
                        suggest.NAMOUNT = reader["NAMOUNT"] == DBNull.Value ? 0 : decimal.Parse(reader["NAMOUNT"].ToString());
                        suggest.CNUMEXA = reader["CNUMEXA"] == DBNull.Value ? string.Empty : reader["CNUMEXA"].ToString();
                        suggest.DFEC_APROB = reader["DFEC_APROB"] == DBNull.Value ? string.Empty : Convert.ToDateTime(reader["DFEC_APROB"]).ToString("dd/MM/yyyy HH:mm:ss");
                        suggest.DFEC_OPER = reader["DFEC_OPER"] == DBNull.Value ? string.Empty : Convert.ToDateTime(reader["DFEC_OPER"]).ToString("dd/MM/yyyy");
                        suggest.NSTATUS = reader["NSTATUS"] == DBNull.Value ? 0 : int.Parse(reader["NSTATUS"].ToString());
                        suggest.SSTATUS_DES = reader["SSTATUS_DES"] == DBNull.Value ? string.Empty : reader["SSTATUS_DES"].ToString();
                        suggest.OBSERVACION = reader["OBSERVACION"] == DBNull.Value ? string.Empty : reader["OBSERVACION"].ToString();
                        suggest.CID_RE_OPEN = reader["CID_RE_OPEN"] == DBNull.Value ? string.Empty : reader["CID_RE_OPEN"].ToString();
                        suggest.CIDOPERBNK = reader["CIDOPERBNK"] == DBNull.Value ? string.Empty : reader["CIDOPERBNK"].ToString();
                        suggest.IS_SELECTED = false;
                        entities.lista.Add(suggest);
                    }
                }
                entities.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                entities.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarAprobacionesDetalleDoc", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<AprobacionesDetalleVM>(entities);
        }
        public Task<ResponseVM> AgregarDetalleObservacion(AgregarDetalleObservacionFilter data)
        {
            var parameters = new List<OracleParameter>();
            ResponseVM entities = new ResponseVM();
            DateTime? dateFormat = null;

            if (data.P_DFEC_APROBR == null || data.P_DFEC_APROBR == "" || data.P_DFEC_APROBR == "null")
            {
                dateFormat = null;
            }
            else
            {
                dateFormat = Convert.ToDateTime(data.P_DFEC_APROBR);
            }

            var procedure = "PKG_OI_TESORERIA.USP_OI_SET_INS_BNK_OBS";
            try
            {
                parameters.Add(new OracleParameter("P_NIDPAYBNKD", OracleDbType.Int64, data.P_NIDPAYBNKD, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NIDPAYBNK", OracleDbType.Int64, data.P_NIDPAYBNK, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NIDPROCESS", OracleDbType.Int64, data.P_NIDPROCESS, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SDES_OBSERV", OracleDbType.NVarchar2, data.P_SDES_OBSERV, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NSTATUS", OracleDbType.Int64, data.P_NSTATUS, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_CID_RE_OPEN", OracleDbType.NVarchar2, data.P_CID_RE_OPEN, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DFEC_APROBR", OracleDbType.Date, dateFormat, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_CIDOPERBNKR", OracleDbType.NVarchar2, data.P_CIDOPERBNKR, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_CUSERCODE", OracleDbType.NVarchar2, data.P_CUSERCODE, ParameterDirection.Input));

                OracleParameter P_NIDPAYBNKO = new OracleParameter("P_NIDPAYBNKO", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NIDPAYBNKO);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureOnlineDB(procedure, parameters);
                entities.P_NIDPAYBNKO = Convert.ToInt32(P_NIDPAYBNKO.Value.ToString());
                entities.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                entities.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);

                //SI NO HAY ERROR EN AGREGAR OBS, ENVÍA CORREO AL USUARIO DE SINIESTROS
                if (entities.P_NCODE == 0)
                {
                    try
                    {
                        enviaCorreoOBS(entities.P_NIDPAYBNKO, 3, 3);
                    }
                    catch (Exception ex)
                    {
                        LogControl.save("AgregarDetalleObservacion", ex.ToString(), "3");
                        throw;
                    }
                }

            }
            catch (Exception ex)
            {
                LogControl.save("AgregarDetalleObservacion", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<ResponseVM>(entities);
        }
        public Task<DetalleObservacionVM> ListarDetalleObservacion(ListarDetalleObservacionFilter data)
        {
            var parameters = new List<OracleParameter>();
            DetalleObservacionVM entities = new DetalleObservacionVM();
            entities.lista = new List<DetalleObservacionVM.P_TABLE>();
            var procedure = "PKG_OI_TESORERIA.USP_OI_GET_LIST_BNK_OBS";
            try
            {
                parameters.Add(new OracleParameter("P_NIDPAYBNK", OracleDbType.Long, data.P_NIDPAYBNKD, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_CTABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureOnlineDB(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        DetalleObservacionVM.P_TABLE suggest = new DetalleObservacionVM.P_TABLE();
                        suggest.NIDPAYBNKO = reader["NIDPAYBNKO"] == DBNull.Value ? 0 : long.Parse(reader["NIDPAYBNKO"].ToString());
                        suggest.NIDPAYBNKD = reader["NIDPAYBNKD"] == DBNull.Value ? 0 : long.Parse(reader["NIDPAYBNKD"].ToString());
                        suggest.NIDPAYBNK = reader["NIDPAYBNK"] == DBNull.Value ? 0 : long.Parse(reader["NIDPAYBNK"].ToString());
                        suggest.NIDPROCESS = reader["NIDPROCESS"] == DBNull.Value ? 0 : long.Parse(reader["NIDPROCESS"].ToString());
                        suggest.SDES_OBSERV = reader["SDES_OBSERV"] == DBNull.Value ? string.Empty : reader["SDES_OBSERV"].ToString();
                        suggest.DFEC_OBSERV = reader["DFEC_OBSERV"] == DBNull.Value ? string.Empty : Convert.ToDateTime(reader["DFEC_OBSERV"]).ToString("dd/MM/yyyy HH:mm:ss");
                        suggest.DFEC_RESULT = reader["DFEC_RESULT"] == DBNull.Value ? string.Empty : Convert.ToDateTime(reader["DFEC_RESULT"]).ToString("dd/MM/yyyy");
                        suggest.NSTA_RESULT = reader["NSTA_RESULT"] == DBNull.Value ? 0 : int.Parse(reader["NSTA_RESULT"].ToString());
                        suggest.CUSERCODE = reader["CUSERCODE"] == DBNull.Value ? string.Empty : reader["CUSERCODE"].ToString();
                        suggest.IS_SELECTED = suggest.NSTA_RESULT == 1 || suggest.NSTA_RESULT == 4 ? true : false;
                        entities.lista.Add(suggest);
                    }
                }
                entities.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                entities.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarDetalleObservacion", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<DetalleObservacionVM>(entities);
        }
        public ResponseVM AprobarProceso(AprobarProcesoFilter data)
        {
            var parameters = new List<OracleParameter>();
            ResponseVM entities = new ResponseVM();
            var procedure = "PKG_OI_TESORERIA.USP_OI_SET_APPROVED_PB";
            try
            {
                parameters.Add(new OracleParameter("P_NIDPAYBNK", OracleDbType.Long, data.P_NIDPAYBNK, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NIDPAYBND", OracleDbType.Long, data.P_NIDPAYBNKD, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_CIDOPERBNK", OracleDbType.NVarchar2, data.P_CIDOPERBNK, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DFEC_OPER", OracleDbType.Date, data.P_DFEC_OPER, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_CUSERCODE", OracleDbType.NVarchar2, data.P_CUSERCODE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCOMMIT", OracleDbType.Int64, data.P_NCOMMIT, ParameterDirection.Input));
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
                LogControl.save("AprobarProceso", ex.ToString(), "3");
                throw;
            }
            return entities;
        }

        public Task<ResponseVM> AprobarProcesoList(AprobarProcesoListFilter data)
        {
            ResponseVM entities = new ResponseVM();
            List<string> cap = new List<string>();
            try
            {
                AprobarProcesoFilter dataDet = new AprobarProcesoFilter();
                int canReg = data.APROBADOS.Count;
                for (int i = 0; i < canReg; i++)
                {
                    dataDet = data.APROBADOS[i];
                    try
                    {
                        ResponseVM entitiesDet = new ResponseVM();
                        entitiesDet = AprobarProceso(dataDet);
                        if (entitiesDet.P_NCODE == 1)
                        {
                            cap.Add(dataDet.P_NIDPAYBNKD.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        LogControl.save("AprobarProceso", ex.ToString(), "3");
                        throw;
                    }
                }
                if (cap.Count > 0)
                {
                    string procList = string.Concat(cap);
                    entities.P_NCODE = 1;
                    entities.P_SMESSAGE = "Hubo errores en los siguientes procesos: " + procList;
                }
                else
                {
                    entities.P_NCODE = 0;
                    entities.P_SMESSAGE = "Se aprobaron los procesos exitosamente.";
                }
            }
            catch (Exception ex)
            {
                LogControl.save("AprobarProcesoList", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<ResponseVM>(entities);
        }
        public ResponseVM ResolverObservacion(ResolverObservacionFilter data)
        {
            var parameters = new List<OracleParameter>();
            ResponseVM entities = new ResponseVM();
            //var procedure = "PKG_OI_TESORERIA.USP_OI_SET_RESOLVED_OBS_PB";
            var procedure = "PKG_OI_TESORERIA.USP_OI_SET_RESOLVED_OBS_OP";
            try
            {
                parameters.Add(new OracleParameter("P_NIDPAYBNKO", OracleDbType.Long, data.P_NIDPAYBNKO, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DFEC_APROB", OracleDbType.Date, data.P_DFEC_APROB, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_CUSERCODE", OracleDbType.NVarchar2, data.P_CUSERCODE, ParameterDirection.Input));
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
                LogControl.save("ResolverObservacion", ex.ToString(), "3");
                throw;
            }
            return entities;
        }

        public Task<ResponseVM> ResolverObservacionList(ResolverObservacionListFilter data)
        {
            ResponseVM entitiesDet = new ResponseVM();
            ResponseVM entities = new ResponseVM();
            List<string> cap = new List<string>();
            string msgErr = string.Empty;
            try
            {
                ResolverObservacionFilter dataDet = new ResolverObservacionFilter();
                int canReg = data.OBSERVACIONES.Count;
                for (int i = 0; i < canReg; i++)
                {
                    dataDet = data.OBSERVACIONES[i];
                    try
                    {
                        
                        entitiesDet = ResolverObservacion(dataDet);
                        if (entitiesDet.P_NCODE == 1)
                        {
                            cap.Add(dataDet.P_NIDPAYBNKO.ToString());
                            msgErr = entitiesDet.P_SMESSAGE;

                        }
                        if (entitiesDet.P_NCODE == 0)
                        {
                            try
                            {
                                enviaCorreoOBS(Convert.ToInt32(dataDet.P_NIDPAYBNKO.ToString()), 4, 4);
                            }
                            catch (Exception ex)
                            {
                                LogControl.save("ResolverObservacion", ex.ToString(), "3");
                                throw;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogControl.save("ResolverObservacion", ex.ToString(), "3");
                        throw;
                    }
                }
                if (cap.Count > 0)
                {
                    string procList = string.Concat(cap);
                    entities.P_NCODE = 1;
                    entities.P_SMESSAGE = "Error: " + msgErr + "-" + procList;
                }
                else
                {
                    entities.P_NCODE = 0;
                    entities.P_SMESSAGE = "Se resolvieron las observaciones exitosamente.";
                }
            }
            catch (Exception ex)
            {
                LogControl.save("ResolverObservacionList", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<ResponseVM>(entities);
        }
        public void enviaCorreoOBS(int VALOR1, int VALOR2, int VALOR3)
        {            
            //NUEVOS ARGUMENTOS PARA VALIDACION PRIMAS COMISIONES
            string arguments = string.Format(@"{0}|{1}|{2}", VALOR1, VALOR2, VALOR3);
            using
            (
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo { FileName = ELog.obtainConfig("exePathRepro"), Arguments = arguments }
                }
            )
            {
                process.Start();
            }            
        }

        public Task<HorarioVM> GetHorarioInterfaz(HorarioBM data)
        {
            var parameters = new List<OracleParameter>();
            HorarioVM entities = new HorarioVM();
            var procedure = "PKG_OI_TESORERIA.USP_OI_GET_HORARIO";
            try
            {
                parameters.Add(new OracleParameter("P_NNUMORI", OracleDbType.Int32, data.P_NNUMORI, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCODGRU", OracleDbType.Int32, data.P_NCODGRU, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.P_NBRANCH, ParameterDirection.Input));
                OracleParameter P_NHORARIO = new OracleParameter("P_NHORARIO", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NHORARIO);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureOnlineDB(procedure, parameters);
                entities.P_NHORARIO = P_NHORARIO.Value.ToString();
                entities.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                entities.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("GetHorarioInterfaz", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<HorarioVM>(entities);
        }
    }
}
