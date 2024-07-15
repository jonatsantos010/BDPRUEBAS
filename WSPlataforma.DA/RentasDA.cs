using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using System.Linq;
using Oracle.DataAccess.Client;
using WSPlataforma.Util;
using WSPlataforma.Entities.RentasModel.ViewModel;
using WSPlataforma.Entities.RentasModel.BindingModel;
using Oracle.DataAccess.Types;
using System.Configuration;
using WSPlataforma.Entities.ModuleReportsModel.BindingModel;
using Newtonsoft.Json;

namespace WSPlataforma.DA
{
    public class RentasDA : ConnectionBase
    {
        ReprocessDA reprocc = new ReprocessDA();
        WebServiceUtil WebServiceUtil = new WebServiceUtil();

        private readonly string Package = "PKG_OI_RENTAS";

        public Task<RentasAprobacionesResVM> ListarAprobacionesRentasRes(RentasAprobacionesResBM data)
        {
            var parameters = new List<OracleParameter>();
            RentasAprobacionesResVM entities = new RentasAprobacionesResVM();
            entities.P_LIST = new List<RentasAprobacionesResVM.P_TABLE>();
            var procedure = string.Format("{0}.{1}", "PKG_OI_RENTAS", "USP_OI_S_OP_APROB_LST_RES");
            try
            {
                parameters.Add(new OracleParameter("P_NNUMORI", OracleDbType.Int32, data.P_NNUMORI, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NTYPEOP", OracleDbType.Int32, data.P_NTYPEOP, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DFECINI", OracleDbType.Date, data.P_DFECINI, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DFECFIN", OracleDbType.Date, data.P_DFECFIN, ParameterDirection.Input));

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
                        RentasAprobacionesResVM.P_TABLE suggest = new RentasAprobacionesResVM.P_TABLE();
                        suggest.NNUMORI = reader["NNUMORI"] == DBNull.Value ? string.Empty : reader["NNUMORI"].ToString();
                        suggest.NTYPEPAGO = reader["NTYPEPAGO"] == DBNull.Value ? string.Empty : reader["NTYPEPAGO"].ToString();
                        suggest.DES_TYPEPAGO = reader["DES_TYPEPAGO"] == DBNull.Value ? string.Empty : reader["DES_TYPEPAGO"].ToString();
                        suggest.SCOD_AFP = reader["SCOD_AFP"] == DBNull.Value ? string.Empty : reader["SCOD_AFP"].ToString();
                        suggest.DES_AFP = reader["DES_AFP"] == DBNull.Value ? string.Empty : reader["DES_AFP"].ToString();
                        suggest.SCOD_BANCO = reader["SCOD_BANCO"] == DBNull.Value ? string.Empty : reader["SCOD_BANCO"].ToString();
                        suggest.DES_BANK = reader["DES_BANK"] == DBNull.Value ? string.Empty : reader["DES_BANK"].ToString();
                        suggest.SCOD_VIAPAGO = reader["SCOD_VIAPAGO"] == DBNull.Value ? string.Empty : reader["SCOD_VIAPAGO"].ToString();
                        suggest.DES_VIAPAGO = reader["DES_VIAPAGO"] == DBNull.Value ? string.Empty : reader["DES_VIAPAGO"].ToString();
                        suggest.SCOD_MONEDA = reader["SCOD_MONEDA"] == DBNull.Value ? string.Empty : reader["SCOD_MONEDA"].ToString();
                        suggest.DES_MONEDA = reader["DES_MONEDA"] == DBNull.Value ? string.Empty : reader["DES_MONEDA"].ToString();
                        suggest.NMTO_PENSION = reader["NMTO_PENSION"] == DBNull.Value ? string.Empty : reader["NMTO_PENSION"].ToString();
                        suggest.NMTO_LIQPAGAR = reader["NMTO_LIQPAGAR"] == DBNull.Value ? string.Empty : reader["NMTO_LIQPAGAR"].ToString();
                        suggest.NMTO_LIQSALUD = reader["NMTO_LIQSALUD"] == DBNull.Value ? string.Empty : reader["NMTO_LIQSALUD"].ToString();
                        suggest.NMTO_LIQRJUD = reader["NMTO_LIQRJUD"] == DBNull.Value ? string.Empty : reader["NMTO_LIQRJUD"].ToString();
                        suggest.CANT_REG = reader["CANT_REG"] == DBNull.Value ? string.Empty : reader["CANT_REG"].ToString();
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
                LogControl.save("ListarAprobacionesRentasRes", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<RentasAprobacionesResVM>(entities);
        }
        public Task<RentasAprobacionesDetVM> ListarAprobacionesRentasDet(RentasAprobacionesDetBM data)
        {
            var parameters = new List<OracleParameter>();
            RentasAprobacionesDetVM entities = new RentasAprobacionesDetVM();
            entities.P_LIST = new List<RentasAprobacionesDetVM.P_TABLE>();
            var procedure = string.Format("{0}.{1}", "PKG_OI_RENTAS", "USP_OI_S_OP_APROB_LST_RES_DET");
            try
            {
                parameters.Add(new OracleParameter("P_NNUMORI", OracleDbType.Int32, data.P_NNUMORI, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NTYPEOP", OracleDbType.Int32, data.P_NTYPEOP, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SCOD_AFP", OracleDbType.NVarchar2, data.P_SCOD_AFP, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SCOD_BANCO", OracleDbType.NVarchar2, data.P_SCOD_BANCO, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SCOD_VIAPAGO", OracleDbType.NVarchar2, data.P_SCOD_VIAPAGO, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SCOD_MONEDA", OracleDbType.NVarchar2, data.P_SCOD_MONEDA, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DFECINI", OracleDbType.Date, data.P_DFECINI, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DFECFIN", OracleDbType.Date, data.P_DFECFIN, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SUSR_APROB", OracleDbType.NVarchar2, data.P_SUSR_APROB, ParameterDirection.Input));

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
                        RentasAprobacionesDetVM.P_TABLE suggest = new RentasAprobacionesDetVM.P_TABLE();
                        suggest.NIDCHEQUE = reader["NIDCHEQUE"] == DBNull.Value ? string.Empty : reader["NIDCHEQUE"].ToString();
                        suggest.DES_NUMORI = reader["DES_NUMORI"] == DBNull.Value ? string.Empty : reader["DES_NUMORI"].ToString();
                        suggest.SNUM_POLIZA = reader["SNUM_POLIZA"] == DBNull.Value ? string.Empty : reader["SNUM_POLIZA"].ToString();
                        suggest.DES_TYPE_IDENBEN = reader["DES_TYPE_IDENBEN"] == DBNull.Value ? string.Empty : reader["DES_TYPE_IDENBEN"].ToString();
                        suggest.SNUM_IDENBEN = reader["SNUM_IDENBEN"] == DBNull.Value ? string.Empty : reader["SNUM_IDENBEN"].ToString();
                        suggest.SNOM_BENEFICIA = reader["SNOM_BENEFICIA"] == DBNull.Value ? string.Empty : reader["SNOM_BENEFICIA"].ToString();
                        suggest.SNOM_RECEPTOR = reader["SNOM_RECEPTOR"] == DBNull.Value ? string.Empty : reader["SNOM_RECEPTOR"].ToString();
                        suggest.DES_TYPEPAGO = reader["DES_TYPEPAGO"] == DBNull.Value ? string.Empty : reader["DES_TYPEPAGO"].ToString();
                        suggest.DFEC_PAGO = reader["DFEC_PAGO"] == DBNull.Value ? string.Empty : Convert.ToDateTime(reader["DFEC_PAGO"]).ToString("dd/MM/yyyy");
                        suggest.DES_MONEDA = reader["DES_MONEDA"] == DBNull.Value ? string.Empty : reader["DES_MONEDA"].ToString();
                        suggest.NMTO_PENSION = reader["NMTO_PENSION"] == DBNull.Value ? string.Empty : reader["NMTO_PENSION"].ToString();
                        entities.P_LIST.Add(suggest);
                    }
                }
                reader.Close();

                entities.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                entities.P_SMESSAGE = P_SMESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarAprobacionesRentasDet", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<RentasAprobacionesDetVM>(entities);
        }
        public async Task<RentasAprobacionesDetVM> Aprobar(RentasAprobacionesDetBM data)
        {
            var parameters = new List<OracleParameter>();
            RentasAprobacionesDetVM entities = new RentasAprobacionesDetVM();
            entities.P_LIST = new List<RentasAprobacionesDetVM.P_TABLE>();

            string FCH_INI = data.P_DFECINI.ToString("dd/MM/yyyy");
            string FCH_FIN = data.P_DFECFIN.ToString("dd/MM/yyyy");

            var procedure = string.Format("{0}.{1}", "PKG_OI_RENTAS", "USP_OI_I_OP_APROB_RESUMEN");
            try
            {
                parameters.Add(new OracleParameter("P_SKEY", OracleDbType.Varchar2, data.P_SKEY, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NNUMORI", OracleDbType.Int32, data.P_NNUMORI, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NTYPEOP", OracleDbType.Int32, data.P_NTYPEOP, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SCOD_AFP", OracleDbType.Varchar2, data.P_SCOD_AFP, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SCOD_BANCO", OracleDbType.Varchar2, data.P_SCOD_BANCO, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SCOD_VIAPAGO", OracleDbType.Varchar2, data.P_SCOD_VIAPAGO, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SCOD_MONEDA", OracleDbType.Varchar2, data.P_SCOD_MONEDA, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DFECINI", OracleDbType.Varchar2, FCH_INI, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DFECFIN", OracleDbType.Varchar2, FCH_FIN, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SUSR_APROB", OracleDbType.Varchar2, data.P_SUSR_APROB, ParameterDirection.Input));
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureRentas(procedure, parameters);
                entities.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                entities.P_SMESSAGE = P_SMESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                LogControl.save("Aprobar", ex.ToString(), "3");
                reprocc.InsertLogerror(0, "AprobarResumen Lin(137)", "Aprobacion OP-Rentas: " + ex.Message);
                throw;
            }
            return await Task.FromResult<RentasAprobacionesDetVM>(entities);
        }
        public async Task<ResponseVM> AprobarPagos(RentasAprobarPagosBM data)
        {
            ResponseVM entities = new ResponseVM();
            //List<string> cap = new List<string>();
            try
            {
                RentasAprobacionesDetVM entitiesDet = new RentasAprobacionesDetVM();
                string SKEY = data.P_LIST[0].P_SUSR_APROB.ToUpper() + DateTime.Now.ToString("yyyyMMddhhmmssff");

                int canReg = data.P_LIST.Count;
                for (int i = 0; i < canReg; i++)
                {
                    RentasAprobacionesDetBM dataDet = new RentasAprobacionesDetBM();

                    dataDet = data.P_LIST[i];
                    try
                    {
                        dataDet.P_SKEY = SKEY;
                        entitiesDet = await (Aprobar(dataDet));
                    }
                    catch (Exception ex)
                    {
                        LogControl.save("AprobarResumen", ex.ToString(), "3");
                        reprocc.InsertLogerror(0, "AprobarPagos Lin(198)", "Aprobacion OP-Rentas: " + ex.Message);
                        throw;
                    }
                }

                try
                {
                    AprobarDetalle(SKEY);
                }
                catch (Exception ex)
                {
                    LogControl.save("AprobarDetalle", ex.ToString(), "3");
                    reprocc.InsertLogerror(0, "AprobarPagos Lin(210)", "Aprobacion OP-Rentas: " + ex.Message);
                    throw;
                }

                //if (cap.Count > 0)
                //{
                //    string temp = string.Concat(cap);
                //    entities.P_NCODE = 1;
                //    entities.P_SMESSAGE = "Hubo errores en los siguientes pagos de pensión: " + temp;
                //}
                //else
                //{
                entities.P_NCODE = 0;
                entities.P_SMESSAGE = "Se aprobaron los pagos exitosamente.";
                //}
            }
            catch (Exception ex)
            {
                LogControl.save("AprobarPagoList", ex.ToString(), "3");
                reprocc.InsertLogerror(0, "AprobarPagos Lin(229)", "Aprobacion OP-Rentas: " + ex.Message);
                throw;
            }
            return await Task.FromResult<ResponseVM>(entities);
        }

        public RentasAprobacionesDetVM AprobarDetalle(string SKEY)
        {
            var parameters = new List<OracleParameter>();
            RentasAprobacionesDetVM entities = new RentasAprobacionesDetVM();
            entities.P_LIST = new List<RentasAprobacionesDetVM.P_TABLE>();
            var procedure = string.Format("{0}.{1}", "PKG_OI_RENTAS", "USP_OI_I_OP_APROB_DETALLE");
            try
            {
                parameters.Add(new OracleParameter("P_SKEY", OracleDbType.Varchar2, SKEY, ParameterDirection.Input));
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureRentas(procedure, parameters);
                entities.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                entities.P_SMESSAGE = P_SMESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                LogControl.save("Aprobar", ex.ToString(), "3");
                reprocc.InsertLogerror(0, "AprobarDetalle Lin(255)", "Aprobacion OP-Rentas: " + ex.Message);
                throw;
            }
            return entities;
        }
        public DataSet GetDataReportRentasRes(ReportRentasResBM data)
        {
            DataSet dataReport = new DataSet();
            DataTable dataCab = new DataTable();
            int Pcode;
            string Pmessage;

            try
            {
                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleRENTAS"].ToString()))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = cn;
                        cmd.CommandText = string.Format("{0}.{1}", Package, "USP_OI_S_OP_APROB_RES_XLS");
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("P_NNUMORI", OracleDbType.Int32).Value = data.P_NNUMORI;
                        cmd.Parameters.Add("P_NTYPEOP", OracleDbType.Int32).Value = data.P_NTYPEOP;
                        cmd.Parameters.Add("P_DFECINI", OracleDbType.Date).Value = data.P_DFECINI;
                        cmd.Parameters.Add("P_DFECFIN", OracleDbType.Date).Value = data.P_DFECFIN;
                        cmd.Parameters.Add("P_TABLE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
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

                        if (Pcode == 1) { throw new Exception(Pmessage); }

                        OracleRefCursor cursorCab = (OracleRefCursor)cmd.Parameters["P_TABLE"].Value;
                        dataCab.Load(cursorCab.GetDataReader());
                        dataCab.TableName = "REPORTE";
                        dataReport.Tables.Add(dataCab);
                        var cantCab = dataCab.Rows.Count;

                        ModuleReportsRentasFilter dataRpteXLS = new ModuleReportsRentasFilter();
                        ModuleReportsDA moduleDA = new ModuleReportsDA();

                        // OBTIENE CONFIGURACIÓN DE REPORTE
                        dataRpteXLS.P_NNUMORI = data.P_NNUMORI;
                        dataRpteXLS.P_NTYPEOP = data.P_NTYPEOP;
                        if (dataRpteXLS.P_NNUMORI == 2)
                        {
                            if (dataRpteXLS.P_NTYPEOP == 7)
                            {
                                dataRpteXLS.P_NREPORT = 1;
                            }
                            else
                            {
                                dataRpteXLS.P_NTYPEOP = 0;
                                dataRpteXLS.P_NREPORT = 2;
                            }
                        }
                        else
                        {
                            dataRpteXLS.P_NNUMORI = 0;
                            dataRpteXLS.P_NTYPEOP = 0;
                            dataRpteXLS.P_NREPORT = 2;
                        }
                        dataRpteXLS.P_NTIPO = 1; // RENTAS RESUMEN

                        DataTable configRES = GetDataFieldsRentas(dataRpteXLS);
                        List<ConfigFields> configurationCAB = GetFieldsConfigurationRentas(configRES, dataRpteXLS);

                        string pathReport = ELog.obtainConfig("reportRentasRes");
                        string fileName = "Reporte_Rentas_Resumen.xlsx";

                        moduleDA.ExportToExcelCD(dataReport, configurationCAB, configurationCAB, pathReport, fileName);
                        cn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetDataReportRentasRes", ex.ToString(), "3");
                throw;
            }
            return dataReport;
        }

        public DataSet GetDataReportRentasDet(ReportRentasDetBM data)
        {
            DataSet dataReport = new DataSet();
            DataTable dataCab = new DataTable();
            int Pcode;
            string Pmessage;

            try
            {
                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleRENTAS"].ToString()))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = cn;
                        cmd.CommandText = string.Format("{0}.{1}", Package, "USP_OI_S_OP_APROB_RES_DET_XLS");
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("P_NNUMORI", OracleDbType.Int32).Value = data.P_NNUMORI;
                        cmd.Parameters.Add("P_NTYPEOP", OracleDbType.Int32).Value = data.P_NTYPEOP;
                        cmd.Parameters.Add("P_SCOD_AFP", OracleDbType.NVarchar2).Value = data.P_SCOD_AFP;
                        cmd.Parameters.Add("P_SCOD_BANCO", OracleDbType.NVarchar2).Value = data.P_SCOD_BANCO;
                        cmd.Parameters.Add("P_SCOD_VIAPAGO", OracleDbType.NVarchar2).Value = data.P_SCOD_VIAPAGO;
                        cmd.Parameters.Add("P_SCOD_MONEDA", OracleDbType.NVarchar2).Value = data.P_SCOD_MONEDA;
                        cmd.Parameters.Add("P_DFECINI", OracleDbType.Date).Value = data.P_DFECINI;
                        cmd.Parameters.Add("P_DFECFIN", OracleDbType.Date).Value = data.P_DFECFIN;
                        cmd.Parameters.Add("P_SUSR_APROB", OracleDbType.NVarchar2).Value = data.P_SUSR_APROB;
                        cmd.Parameters.Add("P_TABLE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
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

                        if (Pcode == 1) { throw new Exception(Pmessage); }

                        OracleRefCursor cursorCab = (OracleRefCursor)cmd.Parameters["P_TABLE"].Value;
                        dataCab.Load(cursorCab.GetDataReader());
                        dataCab.TableName = "REPORTE";
                        dataReport.Tables.Add(dataCab);
                        var cantCab = dataCab.Rows.Count;

                        ModuleReportsRentasFilter dataRpteXLS = new ModuleReportsRentasFilter();
                        ModuleReportsDA moduleDA = new ModuleReportsDA();

                        // OBTIENE CONFIGURACIÓN DE REPORTE
                        dataRpteXLS.P_NNUMORI = data.P_NNUMORI;
                        dataRpteXLS.P_NTYPEOP = data.P_NTYPEOP;
                        if (dataRpteXLS.P_NNUMORI == 2)
                        {
                            if (dataRpteXLS.P_NTYPEOP == 7)
                            {
                                dataRpteXLS.P_NREPORT = 1;
                            }
                            else
                            {
                                dataRpteXLS.P_NTYPEOP = 0;
                                dataRpteXLS.P_NREPORT = 2;
                            }
                        }
                        else
                        {
                            dataRpteXLS.P_NNUMORI = 0;
                            dataRpteXLS.P_NTYPEOP = 0;
                            dataRpteXLS.P_NREPORT = 2;
                        }
                        dataRpteXLS.P_NTIPO = 2; // RENTAS DETALLE

                        DataTable configRES = GetDataFieldsRentas(dataRpteXLS);
                        List<ConfigFields> configurationCAB = GetFieldsConfigurationRentas(configRES, dataRpteXLS);

                        string pathReport = ELog.obtainConfig("reportRentasDet");
                        string fileName = "Reporte_Rentas_Detalle.xlsx";

                        moduleDA.ExportToExcelCD(dataReport, configurationCAB, configurationCAB, pathReport, fileName);
                        cn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetDataReportRentasDet", ex.ToString(), "3");
                throw;
            }
            return dataReport;
        }

        // OBTIENE CONFIGURACIÓN DE CAMPOS PARA ENCABEZADOS DE REPORTES XLS (RENTAS)
        #region genera reporte excel
        public DataTable GetDataFieldsRentas(ModuleReportsRentasFilter parameters)
        {
            DataTable dataFields = new DataTable();

            try
            {
                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleRENTAS"].ToString()))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = cn;
                        IDataReader reader = null;

                        cmd.CommandText = string.Format("{0}.{1}", "PKG_OI_RENTAS", "USP_OI_GET_CONFIG_REPORTS");
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("P_NNUMORI", OracleDbType.Int32).Value = parameters.P_NNUMORI;
                        cmd.Parameters.Add("P_NTYPEOP", OracleDbType.Int32).Value = parameters.P_NTYPEOP;
                        cmd.Parameters.Add("P_NREPORT", OracleDbType.Int32).Value = parameters.P_NREPORT;
                        cmd.Parameters.Add("P_NTIPO", OracleDbType.Int32).Value = parameters.P_NTIPO;
                        cmd.Parameters.Add("C_TABLE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                        var P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);
                        var P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                        P_NCODE.Size = 2000;
                        P_SMESSAGE.Size = 4000;
                        cmd.Parameters.Add(P_NCODE);
                        cmd.Parameters.Add(P_SMESSAGE);

                        cn.Open();
                        reader = cmd.ExecuteReader();

                        if (reader != null)
                        {
                            dataFields.Load(reader);
                            var cant = dataFields.Rows.Count;
                        }
                        cn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetDataFieldsRentas", ex.ToString(), "3");
                throw;
            }
            return dataFields;
        }

        public List<ConfigFields> GetFieldsConfigurationRentas(DataTable Config, ModuleReportsRentasFilter parameters)
        {
            List<ConfigFields> FieldConfiglist = new List<ConfigFields>();
            ModuleReportsDA moduleDA = new ModuleReportsDA();
            FieldConfiglist = CommonMethod.ConvertToList<ConfigFields>(Config);
            return FieldConfiglist;
        }

        public static class CommonMethod
        {
            public static List<T> ConvertToList<T>(DataTable dt)
            {
                var columnNames = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName.ToLower()).ToList();
                var properties = typeof(T).GetProperties();
                return dt.AsEnumerable().Select(row =>
                {
                    var objT = Activator.CreateInstance<T>();
                    foreach (var pro in properties)
                    {
                        if (columnNames.Contains(pro.Name.ToLower()))
                        {
                            try
                            {
                                pro.SetValue(objT, row[pro.Name]);
                            }
                            catch (Exception ex) { }
                        }
                    }
                    return objT;
                }).ToList();
            }
        }
        #endregion

        //DATA ACCESS QUERY PARA LOS TICKETS
        public RentasFilterTicketsResponseVM PD_REA_CLIENT_TICKETS(RentasFilterTicketsBM data)
        {
            var response = new RentasFilterTicketsResponseVM();
            var sPackageName = ProcedureName.pkg_rentas + ".SP_PD_REA_CLIENT_TICKETS";
            List<OracleParameter> parameters = new List<OracleParameter>();
            List<RentasTicketsVM> list = new List<RentasTicketsVM>();

            try
            {
                parameters.Add(new OracleParameter("P_DATEINI", OracleDbType.Date, data.P_DATEINI, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DATEEND", OracleDbType.Date, data.P_DATEEND, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.P_NBRANCH, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, data.P_NPRODUCT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int32, data.P_NPOLICY, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NMOTIVO", OracleDbType.Int32, data.P_NMOTIVO, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SSUBMOTIVO", OracleDbType.Int32, data.P_SSUBMOTIVO, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SCODE_JIRA", OracleDbType.Varchar2, data.P_SCODE_JIRA, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SCUSPP", OracleDbType.Varchar2, data.P_SCUSPP, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NSTATE", OracleDbType.Int32, data.P_NSTATE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, data.P_SCLIENT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, data.P_NUSERCODE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NUSERCODE_RE", OracleDbType.Int32, data.P_NUSERCODE_RE, ParameterDirection.Input));


                // OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, ParameterDirection.Output);
                OracleParameter C_TICKETS = new OracleParameter("C_TICKETS", OracleDbType.RefCursor, ParameterDirection.Output);

                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                parameters.Add(C_TICKETS);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameters);
                while (odr.Read())
                {
                    RentasTicketsVM item = new RentasTicketsVM();
                    item.SCODE = odr["SCODE"].ToString();
                    item.SCODE_JIRA = odr["SCODE_JIRA"].ToString();
                    item.SRAMO = odr["SRAMO"].ToString();
                    item.SPRODUCT = odr["SPRODUCT"].ToString();
                    item.SCODE_JIRA = odr["SCODE_JIRA"].ToString();
                    item.POLIZA = Convert.ToInt32(odr["POLIZA"].ToString());
                    item.MOTIVO_DES = odr["MOTIVO_DES"].ToString();
                    item.SUBMOTIVO_DES = odr["SUBMOTIVO_DES"].ToString();
                    item.CONTRATANTE = odr["CONTRATANTE"].ToString();
                    item.ASEGURADO = odr["ASEGURADO"].ToString();
                    item.FECHA_REGISTRO = Convert.ToDateTime(odr["FECHA_REGISTRO"].ToString());
                    item.FECHA_RECEPCION = Convert.ToDateTime(odr["FECHA_RECEPCION"].ToString());
                    item.USUARIO_REGISTRO = odr["USUARIO_REGISTRO"].ToString();
                    item.SLA = odr["SLA"].ToString();
                    item.ESTADO = odr["ESTADO"].ToString();
                    item.MONEDA = odr["MONEDA"] != DBNull.Value ? Convert.ToInt32(odr["MONEDA"]) : (int?)null;
                    item.IMP_DEVOLUCION = odr["IMP_DEVOLUCION"].ToString();
                    item.NTYPE_SYSTEM = Convert.ToInt32(odr["NTYPE_SYSTEM"].ToString());
                    item.NRAMO = Convert.ToInt32(odr["NRAMO"].ToString());
                    item.NPRODUCT = Convert.ToInt32(odr["NPRODUCT"].ToString());
                    item.NMOTIV = Convert.ToInt32(odr["NMOTIV"].ToString());
                    item.NSUBMOTIV = Convert.ToInt32(odr["NSUBMOTIV"].ToString());
                    item.NTICKET = Convert.ToInt32(odr["NTICKET"].ToString());
                    list.Add(item);
                }
                response.GenericResponse = list;
                response.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                response.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                response.P_NCODE = 1;
                response.P_SMESSAGE = ex.ToString();
                LogControl.save("PD_REA_CLIENT_TICKETS", ex.ToString(), "3");
            }

            return response;
        }
        //DATA ACCESS QUERY PARA LISTAR UN TICKET 
        public RentasFilterTicketResponseVM PD_REA_CLIENT_TICKET(RentasFilterTicketBM data)
        {
            var response = new RentasFilterTicketResponseVM();
            var sPackageName = ProcedureName.pkg_rentas + ".SP_PD_REA_CLIENT_TICKET";
            List<OracleParameter> parameters = new List<OracleParameter>();
            List<RentasTicketVM> list = new List<RentasTicketVM>();
            List<RentasADJUNTOSVM> list2 = new List<RentasADJUNTOSVM>();
            List<RentasADJUNTOSVM> list3 = new List<RentasADJUNTOSVM>();
            List<RentasTicketBeneficiaryVM> list4 = new List<RentasTicketBeneficiaryVM>();
            List<RentasHistoryVM> list5 = new List<RentasHistoryVM>();
            List<RentasHistoryStatusVM> list6 = new List<RentasHistoryStatusVM>();

            try
            {
                parameters.Add(new OracleParameter("P_SCODE", OracleDbType.Varchar2, data.P_SCODE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, data.P_NUSERCODE, ParameterDirection.Input));

                // OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, ParameterDirection.Output);
                OracleParameter C_DET_TICKET = new OracleParameter("C_DET_TICKET", OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter C_ADJUNTOS_REGIS = new OracleParameter("C_ADJUNTOS_REGIS", OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter C_ADJUNTOS_ENVI = new OracleParameter("C_ADJUNTOS_ENVI", OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter C_BENEFICIARIES = new OracleParameter("C_BENEFICIARIES", OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter C_HISTORY_POL = new OracleParameter("C_HISTORY_POL", OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter C_HISTORY_STATUS = new OracleParameter("C_HISTORY_STATUS", OracleDbType.RefCursor, ParameterDirection.Output);

                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                parameters.Add(C_DET_TICKET);
                parameters.Add(C_ADJUNTOS_REGIS);
                parameters.Add(C_ADJUNTOS_ENVI);
                parameters.Add(C_BENEFICIARIES);
                parameters.Add(C_HISTORY_POL);
                parameters.Add(C_HISTORY_STATUS);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameters);
                while (odr.Read())
                {
                    RentasTicketVM item = new RentasTicketVM();
                    item.SCODE_JIRA = odr["SCODE_JIRA"].ToString();
                    item.USUARIO_REGISTRO = odr["USUARIO_REGISTRO"].ToString();
                    item.ESTADO = odr["ESTADO"].ToString();
                    item.CANAL = odr["CANAL"].ToString();
                    item.FECHA_RECEPCION = odr["FECHA_RECEPCION"].ToString();
                    item.VIA_RECEPCION = odr["VIA_RECEPCION"].ToString();
                    item.ATENCION_SLA = odr["ATENCION_SLA"].ToString();
                    item.FORM_CALCULO = odr["FORM_CALCULO"].ToString();
                    item.MOTIVO = odr["MOTIVO"].ToString();
                    item.DET_UBIGEO = odr["DET_UBIGEO"].ToString();
                    item.COD_RECLAMO = odr["COD_RECLAMO"].ToString();
                    item.CANAL_OPERACION = odr["CANAL_OPERACION"].ToString();
                    item.SUBMOTIVO = odr["SUBMOTIVO"].ToString();
                    item.IMP_DEVOLUCION = odr["IMP_DEVOLUCION"].ToString();
                    item.TIPO_TRAMITE = odr["TIPO_TRAMITE"].ToString();
                    item.TIPO_PENSION_TCK = odr["TIPO_PENSION_TCK"].ToString();
                    item.TIPO_PENSION = odr["TIPO_PENSION"].ToString();
                    item.TIPO_PRESTACION = odr["TIPO_PRESTACION"].ToString();
                    item.CANAL_INGRESO = odr["CANAL_INGRESO"].ToString();
                    item.DESCRIPCION = odr["DESCRIPCION"].ToString();
                    item.NTYPE_SYSTEM = Convert.ToInt32(odr["NTYPE_SYSTEM"].ToString());
                    item.STYPE_SYSTEM = odr["STYPE_SYSTEM"].ToString();
                    item.DOCUMENTO_CONT = odr["DOCUMENTO_CONT"].ToString();
                    item.NOMBRE_CONT = odr["NOMBRE_CONT"].ToString();
                    item.CORREO_ELECTRONICO_CONT = odr["CORREO_ELECTRONICO_CONT"].ToString();
                    item.NROSOL_CONTR = odr["NROSOL_CONTR"].ToString();
                    item.POLIZA_CONTR = odr["POLIZA_CONTR"].ToString();
                    item.CONTRATANTE = odr["CONTRATANTE"].ToString();
                    item.ASESEGURADO = odr["ASESEGURADO"].ToString();
                    item.NROIDENTI_ASE = odr["NROIDENTI_ASE"].ToString();
                    item.RAMO = odr["RAMO"].ToString();
                    item.PRODUCTO = odr["PRODUCTO"].ToString();
                    item.POLIZA = odr["POLIZA"].ToString();
                    item.ESTADO_POLIZA = odr["ESTADO_POLIZA"].ToString();
                    item.FRECUENCIA_PAGO = odr["FRECUENCIA_PAGO"].ToString();
                    item.PRIMA = odr["PRIMA"].ToString();
                    item.PERIODO_VIGENCIA = odr["PERIODO_VIGENCIA"].ToString();
                    item.NRO_ENDOSO = odr["NRO_ENDOSO"].ToString();
                    item.TIPO_PENSION = odr["TIPO_PENSION"].ToString();
                    item.TIPO_RENTA = odr["TIPO_RENTA"].ToString();
                    item.MODALIDAD = odr["MODALIDAD"].ToString();
                    item.NROS_BENEFECIER = odr["NROS_BENEFECIER"].ToString();
                    item.AFP = odr["AFP"].ToString();
                    item.TASA_CTO_REASEGURO = odr["TASA_CTO_REASEGURO"].ToString();
                    item.TATA_CTO_EQUIVALENTE = odr["TATA_CTO_EQUIVALENTE"].ToString();
                    item.MONTO_PRIMA = odr["MONTO_PRIMA"].ToString();
                    item.MESES_DIFERIDOS = odr["MESES_DIFERIDOS"].ToString();
                    item.MESES_GARANTIZADOS = odr["MESES_GARANTIZADOS"].ToString();
                    item.SOLICITUD_DEV = odr["SOLICITUD_DEV"].ToString();
                    item.TASA_VENTA = odr["TASA_VENTA"].ToString();
                    item.TASA_PERIODO_GARAN = odr["TASA_PERIODO_GARAN"].ToString();
                    item.MONTO_PENSION = odr["MONTO_PENSION"].ToString();
                    item.PRIMA = odr["ASESOR"].ToString();
                    item.MONEDA = odr["MONEDA"].ToString();
                    item.MONTO = odr["MONTO"].ToString();
                    item.PORC_DEVOLUCION = odr["PORC_DEVOLUCION"].ToString();
                    item.INI_VIGENCIA = odr["INI_VIGENCIA"].ToString();
                    item.FIN_VIGENCIA = odr["FIN_VIGENCIA"].ToString();
                    item.FIN_FINABONO = odr["FIN_FINABONO"].ToString();
                    item.FCH_DEVENGUE = odr["FCH_DEVENGUE"].ToString();
                    item.DIFERIMIENTO = odr["DIFERIMIENTO"].ToString();
                    item.ANO_TMP = odr["ANO_TMP"].ToString();
                    item.ANO_GARANTIZA = odr["ANO_GARANTIZA"].ToString();
                    item.PENSION = odr["PENSION"].ToString();
                    item.PROC_SEGURO_VIDA = odr["PROC_SEGURO_VIDA"].ToString();
                    item.PROC_SEGURO_ACC = odr["PROC_SEGURO_ACC"].ToString();
                    item.PROC_AJUSTE_CAN = odr["PROC_AJUSTE_CAN"].ToString();
                    item.GASTO_SEPELIO = odr["GASTO_SEPELIO"].ToString();
                    item.GRATIFICACION = odr["GRATIFICACION"].ToString();
                    item.POLIZA_MENOR_EDAD = odr["POLIZA_MENOR_EDAD"].ToString();
                    item.FCH_RESPONSABLE = odr["FCH_RESPONSABLE"].ToString();
                    item.ABSO_RESPONSABLE = odr["ABSO_RESPONSABLE"].ToString();
                    item.RESP_FINAL_RESPONSABLE = odr["RESP_FINAL_RESPONSABLE"].ToString();
                    item.RESP_SOLU_RESPONSABLE = odr["RESP_SOLU_RESPONSABLE"].ToString();
                    item.ATENTIDO_TICKET = odr["ATENTIDO_TICKET"].ToString();
                    item.FCH_RESPUESTA_TICKET = odr["FCH_RESPUESTA_TICKET"].ToString();
                    item.DIAS_ATENCION_TICKET = odr["DIAS_ATENCION_TICKET"].ToString();
                    item.SLINK_JIRA = odr["SLINK_JIRA"].ToString();
                    item.NTICKET = odr["NTICKET"].ToString();
                    item.NMOTIVO = Convert.ToInt32(odr["NMOTIVO"].ToString());
                    item.NSUBMOTIVO = Convert.ToInt32(odr["NSUBMOTIVO"].ToString());
                    list.Add(item);
                }
                response.C_TICKET = list;
                odr.NextResult();

                while (odr.Read())
                {
                    RentasADJUNTOSVM item2 = new RentasADJUNTOSVM();
                    item2.NID = Convert.ToInt32(odr["NID"].ToString());
                    item2.NTICKET = Convert.ToInt32(odr["NTICKET"].ToString());
                    item2.SCODE = odr["SCODE"].ToString();
                    item2.SNAME = odr["SNAME"].ToString();
                    item2.SSIZE = odr["SSIZE"].ToString();
                    item2.SPATH = odr["SPATH"].ToString();
                    item2.SPATH_GD = odr["SPATH_GD"].ToString();
                    item2.NTYPE = Convert.ToInt32(odr["NTYPE"].ToString());
                    list2.Add(item2);

                }
                response.C_ADJUNTOS_REGIS = list2;
                odr.NextResult();

                while (odr.Read())
                {
                    RentasADJUNTOSVM item3 = new RentasADJUNTOSVM();
                    item3.NID = Convert.ToInt32(odr["NID"].ToString()); ;
                    item3.NTICKET = Convert.ToInt32(odr["NTICKET"].ToString());
                    item3.SCODE = odr["SCODE"].ToString();
                    item3.SNAME = odr["SNAME"].ToString();
                    item3.SSIZE = odr["SSIZE"].ToString();
                    item3.SPATH = odr["SPATH"].ToString();
                    item3.SPATH_GD = odr["SPATH_GD"].ToString();
                    item3.NTYPE = Convert.ToInt32(odr["NTYPE"].ToString());
                    list3.Add(item3);

                }
                response.C_ADJUNTOS_ENVI = list3;
                odr.NextResult();

                while (odr.Read())
                {
                    RentasTicketBeneficiaryVM item4 = new RentasTicketBeneficiaryVM();
                    item4.NOMBRES = odr["NOMBRES"].ToString();
                    item4.NRO_IDENTIDAD = odr["NRO_IDENTIDAD"].ToString();
                    item4.PARENTESCO = odr["PARENTESCO"].ToString();
                    item4.PROC_PENSION = odr["PROC_PENSION"].ToString();
                    list4.Add(item4);

                }
                response.C_BENEFICIARIES = list4;

                odr.NextResult();

                while (odr.Read())
                {
                    RentasHistoryVM item5 = new RentasHistoryVM();
                    item5.INDICE = odr["LINEA"].ToString();
                    item5.FECHA = odr["FECHA_ESTADO"].ToString();
                    item5.USUARIO = odr["USUARIO"].ToString();
                    item5.ESTADO = odr["ESTADO"].ToString();
                    list5.Add(item5);

                }
                response.C_HISTORY_POL = list5;

                odr.NextResult();

                while (odr.Read())
                {
                    RentasHistoryStatusVM item6 = new RentasHistoryStatusVM();
                    item6.NTRANSAC = odr["NTRANSAC"].ToString();
                    item6.ESTADO = odr["ESTADO"].ToString();
                    item6.FECHA_REGISTO = odr["FECHA_REGISTO"].ToString();
                    item6.USUARIO = odr["USUARIO"].ToString();
                    item6.STYPECOMMENT = odr["STYPECOMMENT"].ToString();
                    item6.SCOMMETS = odr["SCOMMETS"].ToString();
                    item6.INTER_DAYS = odr["INTER_DAYS"].ToString();
                    list6.Add(item6);
                }
                response.C_HISTORY_STATUS = list6;



                response.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                response.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                response.P_NCODE = 1;
                response.P_SMESSAGE = ex.ToString();
                LogControl.save("PD_REA_CLIENT_TICKET", ex.ToString(), "3");
            }

            return response;
        }
        //DATA ACCESS QUERY PARA LISTAR LOS PRODUCTOS
        public RentasListProductcsResponseVM PD_GET_PRODUCTOS()
        {
            var response = new RentasListProductcsResponseVM();
            var sPackageName = ProcedureName.pkg_rentas + ".SP_PD_GET_PRODUCTOS";
            List<OracleParameter> parameters = new List<OracleParameter>();
            List<RentasProductcsResponseVM> list = new List<RentasProductcsResponseVM>();
            try
            {
                // OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, ParameterDirection.Output);
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);

                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                parameters.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameters);
                while (odr.Read())
                {
                    RentasProductcsResponseVM item = new RentasProductcsResponseVM();
                    item.NBRANCH = Convert.ToInt32(odr["NBRANCH"].ToString());
                    item.NPRODUCT = Convert.ToInt32(odr["NPRODUCT"].ToString());
                    item.SPRODUCT = odr["SPRODUCT"].ToString();
                    list.Add(item);
                }
                response.C_TABLE = list;
                response.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                response.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                response.P_NCODE = 1;
                response.P_SMESSAGE = ex.ToString();
                LogControl.save("PD_GET_PRODUCTOS", ex.ToString(), "3");
            }

            return response;
        }
        //DATA ACCESS QUERY PARA LISTAR LOS MOTIVOS
        public RentasListProductcsResponseVM PD_GET_MOTIVOS()
        {
            var response = new RentasListProductcsResponseVM();
            var sPackageName = ProcedureName.pkg_rentas + ".SD_PD_GET_MOTIVOS";
            List<OracleParameter> parameters = new List<OracleParameter>();
            List<RentasMotivosResponseVM> list = new List<RentasMotivosResponseVM>();
            try
            {
                // OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, ParameterDirection.Output);
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);

                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                parameters.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameters);
                while (odr.Read())
                {
                    RentasMotivosResponseVM item = new RentasMotivosResponseVM();
                    item.NCODE = Convert.ToInt32(odr["NCODE"].ToString());
                    item.SDESCRIPT = odr["SDESCRIPT"].ToString();
                    list.Add(item);
                }
                response.C_TABLE = list;
                response.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                response.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                response.P_NCODE = 1;
                response.P_SMESSAGE = ex.ToString();
                LogControl.save("PD_GET_MOTIVOS", ex.ToString(), "3");
            }

            return response;
        }
        //DATA ACCESS QUERY PARA LISTAR LOS SUBMOTIVOS
        public RentasListSubMotivosResponseVM PD_GET_SUBMOTIVOS(RentasSubMotivoscsBM data)
        {
            var response = new RentasListSubMotivosResponseVM();
            var sPackageName = ProcedureName.pkg_rentas + ".SP_PD_GET_SUBMOTIVOS";
            List<OracleParameter> parameters = new List<OracleParameter>();
            List<RentasSubMotivosResponseVM> list = new List<RentasSubMotivosResponseVM>();
            try
            {
                parameters.Add(new OracleParameter("P_NMOTIVO", OracleDbType.Varchar2, data.P_NMOTIVO, ParameterDirection.Input));


                // OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, ParameterDirection.Output);
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);

                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                parameters.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameters);
                while (odr.Read())
                {
                    RentasSubMotivosResponseVM item = new RentasSubMotivosResponseVM();
                    item.NCODE = Convert.ToInt32(odr["NCODE"].ToString());
                    item.SDESCRIPT = odr["SDESCRIPT"].ToString();
                    list.Add(item);
                }
                response.C_TABLE = list;
                response.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                response.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                response.P_NCODE = 1;
                response.P_SMESSAGE = ex.ToString();
                LogControl.save("PD_GET_SUBMOTIVOS", ex.ToString(), "3");
            }

            return response;
        }
        //DATA ACCESS QUERY PARA ASIGNAR EJECUTIVO 
        public RentasASSIGNEXECResponseVM PD_UPD_ASSIGNEXEC(RentasASSIGNEXECBM data)
        {
            var response = new RentasASSIGNEXECResponseVM();
            var sPackageName = ProcedureName.pkg_rentas + ".SP_PD_UPD_ASSIGNEXEC";
            List<OracleParameter> parameters = new List<OracleParameter>();
            try
            {
                if (data.LIST_SCODE_JIRA != null)
                {
                    foreach (var item in data.LIST_SCODE_JIRA)
                    {
                        parameters.Clear();

                        parameters.Add(new OracleParameter("P_SCODE_JIRA", OracleDbType.Varchar2, item, ParameterDirection.Input));
                        parameters.Add(new OracleParameter("P_NUSERCODE_RE", OracleDbType.Int32, data.P_NUSERCODE_RE, ParameterDirection.Input));
                        parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, data.P_NUSERCODE, ParameterDirection.Input));

                        // OUTPUT
                        OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, ParameterDirection.Output);
                        OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, string.Empty, ParameterDirection.Output);

                        P_SMESSAGE.Size = 4000;
                        parameters.Add(P_NCODE);
                        parameters.Add(P_SMESSAGE);

                        OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameters);
                        response.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                        response.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                        ELog.CloseConnection(odr);
                    }
                }
                else
                {
                    parameters.Add(new OracleParameter("P_SCODE_JIRA", OracleDbType.Varchar2, data.P_SCODE_JIRA, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_NUSERCODE_RE", OracleDbType.Int32, data.P_NUSERCODE_RE, ParameterDirection.Input));

                    // OUTPUT
                    OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, ParameterDirection.Output);
                    OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, string.Empty, ParameterDirection.Output);

                    P_SMESSAGE.Size = 4000;
                    parameters.Add(P_NCODE);
                    parameters.Add(P_SMESSAGE);

                    OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameters);
                    response.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                    response.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                    response.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                    response.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                    ELog.CloseConnection(odr);
                }
            }
            catch (Exception ex)
            {
                response.P_NCODE = 1;
                response.P_SMESSAGE = ex.ToString();
                LogControl.save("PD_UPD_ASSIGNEXEC", ex.ToString(), "3");
            }


            return response;
        }
        //DATA ACCESS QUERY PARA LISTAR LOS CLIENTES
        public RentasListSubMotivosResponseVM PD_REA_CLIENTS(RentasClientsBM data)
        {
            var response = new RentasListSubMotivosResponseVM();
            var sPackageName = ProcedureName.pkg_rentas + ".SP_PD_REA_CLIENTS";
            List<OracleParameter> parameters = new List<OracleParameter>();
            List<RentasClientsVM> list = new List<RentasClientsVM>();
            try
            {
                parameters.Add(new OracleParameter("P_NTIPO_BUSQUEDA", OracleDbType.Int32, data.P_NTIPO_BUSQUEDA, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NTIPO_DOC", OracleDbType.Int32, data.P_NTIPO_DOC, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NNUM_DOC", OracleDbType.Varchar2, data.P_NNUM_DOC, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SNOMBRE", OracleDbType.Varchar2, data.P_SNOMBRE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SAP_PATERNO", OracleDbType.Varchar2, data.P_SAP_PATERNO, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SAP_MATERNO", OracleDbType.Varchar2, data.P_SAP_MATERNO, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SNOMBRE_LEGAL", OracleDbType.Varchar2, data.P_SNOMBRE_LEGAL, ParameterDirection.Input));


                // OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, ParameterDirection.Output);
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);

                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                parameters.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameters);
                while (odr.Read())
                {
                    RentasClientsVM item = new RentasClientsVM();
                    item.SCLIENT = odr["SCLIENT"].ToString();
                    item.NTYPCLIENTDOC = Convert.ToInt32(odr["NTYPCLIENTDOC"].ToString());
                    item.STYPCLIENTDOC = odr["STYPCLIENTDOC"].ToString();
                    item.SCLINUMDOCU = odr["SCLINUMDOCU"].ToString();
                    item.SCLIENAME = odr["SCLIENAME"].ToString();
                    list.Add(item);
                }
                response.C_TABLE = list;
                response.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                response.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                response.P_NCODE = 1;
                response.P_SMESSAGE = ex.ToString();
                LogControl.save("PD_REA_CLIENTS", ex.ToString(), "3");
            }

            return response;
        }
        //DATA ACCESS QUERY PARA LISTAR LOS ESTADOS
        public RentasListEstadosResponseVM PD_GET_ESTADOS()
        {
            var response = new RentasListEstadosResponseVM();
            var sPackageName = ProcedureName.pkg_rentas + ".SP_PD_GET_ESTADOS";
            List<OracleParameter> parameters = new List<OracleParameter>();
            List<RentasEstadoResponseVM> list = new List<RentasEstadoResponseVM>();
            try
            {
                // OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, ParameterDirection.Output);
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);

                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                parameters.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameters);
                while (odr.Read())
                {
                    RentasEstadoResponseVM item = new RentasEstadoResponseVM();
                    item.NCODE = Convert.ToInt32(odr["NCODE"].ToString());
                    item.SDESCRIPT = odr["SDESCRIPT"].ToString();
                    list.Add(item);
                }
                response.C_TABLE = list;
                response.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                response.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                response.P_NCODE = 1;
                response.P_SMESSAGE = ex.ToString();
                LogControl.save("PD_GET_ESTADOS", ex.ToString(), "3");
            }

            return response;
        }
        //DATA ACCESS QUERY PARA LISTAR LOS EJECUTIVOS
        public RentasListEstadosResponseVM PD_GET_EJECUTIVOS(RentaseEjecutivosBM data)
        {
            var response = new RentasListEstadosResponseVM();
            var sPackageName = ProcedureName.pkg_rentas + ".SP_PD_GET_EJECUTIVOS";
            List<OracleParameter> parameters = new List<OracleParameter>();
            List<RentasEstadoResponseVM> list = new List<RentasEstadoResponseVM>();
            try
            {

                parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, data.P_NUSERCODE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NIDPROFILE", OracleDbType.Int32, data.P_NIDPROFILE, ParameterDirection.Input));

                // OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, ParameterDirection.Output);
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);

                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                parameters.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameters);
                while (odr.Read())
                {
                    RentasEstadoResponseVM item = new RentasEstadoResponseVM();
                    item.NCODE = Convert.ToInt32(odr["NCODE"].ToString());
                    item.SDESCRIPT = odr["SDESCRIPT"].ToString();
                    list.Add(item);
                }
                response.C_TABLE = list;
                response.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                response.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                response.P_NCODE = 1;
                response.P_SMESSAGE = ex.ToString();
                LogControl.save("PD_GET_EJECUTIVOS", ex.ToString(), "3");
            }

            return response;
        }
        //DATA ACCESS QUERY PARA LISTAR LOS TIPOS DE DOCUMENTOS
        public RentasListEstadosResponseVM PD_GET_TYPE_DOCUMENT()
        {
            var response = new RentasListEstadosResponseVM();
            var sPackageName = ProcedureName.pkg_rentas + ".SP_PD_GET_TYPE_DOCUMENT";
            List<OracleParameter> parameters = new List<OracleParameter>();
            List<RentasEstadoResponseVM> list = new List<RentasEstadoResponseVM>();
            try
            {
                // OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, ParameterDirection.Output);
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);

                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                parameters.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameters);
                while (odr.Read())
                {
                    RentasEstadoResponseVM item = new RentasEstadoResponseVM();
                    item.NCODE = Convert.ToInt32(odr["NCODE"].ToString());
                    item.SDESCRIPT = odr["SDESCRIPT"].ToString();
                    list.Add(item);
                }
                response.C_TABLE = list;
                response.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                response.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                response.P_NCODE = 1;
                response.P_SMESSAGE = ex.ToString();
                LogControl.save("PD_GET_TYPE_DOCUMENT", ex.ToString(), "3");
            }

            return response;
        }
        //DATA ACCESS QUERY PARA LISTAR EL TIPO DE PERSONA
        public RentasListEstadosResponseVM PD_GET_TYPE_PERSON()
        {
            var response = new RentasListEstadosResponseVM();
            var sPackageName = ProcedureName.pkg_rentas + ".SP_PD_GET_TYPE_PERSON";
            List<OracleParameter> parameters = new List<OracleParameter>();
            List<RentasEstadoResponseVM> list = new List<RentasEstadoResponseVM>();
            try
            {
                // OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, ParameterDirection.Output);
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);

                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                parameters.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameters);
                while (odr.Read())
                {
                    RentasEstadoResponseVM item = new RentasEstadoResponseVM();
                    item.NCODE = Convert.ToInt32(odr["NCODE"].ToString());
                    item.SDESCRIPT = odr["SDESCRIPT"].ToString();
                    list.Add(item);
                }
                response.C_TABLE = list;
                response.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                response.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                response.P_NCODE = 1;
                response.P_SMESSAGE = ex.ToString();
                LogControl.save("PD_GET_TYPE_PERSON", ex.ToString(), "3");
            }

            return response;
        }
        //DATA ACCESS QUERY PARA VALIDAD LA CANTIDAD DE CARACTERES DE LOS DOCUMENTOS
        public RentasValidaResponseVM PD_VALFORMATVALUES(RentasValidaBM data)
        {
            var response = new RentasValidaResponseVM();
            var sPackageName = ProcedureName.pkg_rentas + ".SP_PD_VALFORMATVALUES";
            List<OracleParameter> parameters = new List<OracleParameter>();
            try
            {
                parameters.Add(new OracleParameter("P_NTYPCLIENTDOC", OracleDbType.Int32, data.P_NTYPCLIENTDOC, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SCLINUMDOCU", OracleDbType.Varchar2, data.P_SCLINUMDOCU, ParameterDirection.Input));

                // OUTPUT
                OracleParameter P_SVALIDA = new OracleParameter("P_SVALIDA", OracleDbType.Int32, 4000, ParameterDirection.Output);

                parameters.Add(P_SVALIDA);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameters);
                response.P_SVALIDA = Convert.ToInt32(P_SVALIDA.Value.ToString());

                ELog.CloseConnection(odr);


            }
            catch (Exception ex)
            {
                LogControl.save("PD_VALFORMATVALUES", ex.ToString(), "3");
            }

            return response;
        }
        //DATA ACCESS QUERY PARA OBTENER LA CANTIDAD DE DIAS A RETROCEDER
        public RentasFilterDayResponseVM PD_GET_FILTERDAY_START()
        {
            var response = new RentasFilterDayResponseVM();
            var sPackageName = ProcedureName.pkg_rentas + ".SP_PD_GET_FILTERDAY_START";
            List<OracleParameter> parameters = new List<OracleParameter>();
            List<RentasDayResponseVM> list = new List<RentasDayResponseVM>();

            try
            {
                // OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, ParameterDirection.Output);
                OracleParameter C_DAYSTART = new OracleParameter("C_DAYSTART ", OracleDbType.RefCursor, ParameterDirection.Output);

                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                parameters.Add(C_DAYSTART);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameters);
                while (odr.Read())
                {
                    RentasDayResponseVM item = new RentasDayResponseVM();
                    item.NDAYS = Convert.ToInt32(odr["NDAYS"].ToString());
                    list.Add(item);
                }

                response.NDAYS = list[0].NDAYS;
                response.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                response.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);


            }
            catch (Exception ex)
            {
                response.P_NCODE = 1;
                response.P_SMESSAGE = ex.ToString();
                LogControl.save("PD_GET_FILTERDAY_START", ex.ToString(), "3");
            }

            return response;
        }
        //DATA ACCESS QUERY PARA RECUPERAR EL PRODUCTO
        public RentasProductCanalResponseVM PD_GET_NPRODUCTCANAL()
        {
            var response = new RentasProductCanalResponseVM();
            var sPackageName = ProcedureName.pkg_rentas + ".SP_PD_GET_NPRODUCTCANAL";
            List<OracleParameter> parameters = new List<OracleParameter>();
            List<RentasProductCanalVM> list = new List<RentasProductCanalVM>();

            try
            {
                // OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, ParameterDirection.Output);
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);

                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                parameters.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameters);
                while (odr.Read())
                {
                    RentasProductCanalVM item = new RentasProductCanalVM();
                    item.NPRODUCT = Convert.ToInt32(odr["NPRODUCT"].ToString());
                    list.Add(item);
                }
                int NPRODUCT = list[0].NPRODUCT;

                response.NPRODUCT = NPRODUCT;
                response.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                response.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);


            }
            catch (Exception ex)
            {
                response.P_NCODE = 1;
                response.P_SMESSAGE = ex.ToString();
                LogControl.save("PD_GET_NPRODUCTCANAL", ex.ToString(), "3");
            }

            return response;
        }
        //DATA ACCESS QUERY PARA LISTAR LAS ACCIONES PERMITIDAS PARA EL PERFIL
        public RentasListActionsResponseVM PD_REA_LIST_ACTIONS(RentasListActionsBM data)
        {
            var response = new RentasListActionsResponseVM();
            var sPackageName = ProcedureName.pkg_rentas + ".SP_PD_REA_LIST_ACTIONS";
            List<OracleParameter> parameters = new List<OracleParameter>();
            List<RentasActionResponseVM> list = new List<RentasActionResponseVM>();

            try
            {
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, data.P_NPRODUCT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NIDPROFILE", OracleDbType.Int32, data.P_NIDPROFILE, ParameterDirection.Input));

                // OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, ParameterDirection.Output);
                OracleParameter C_TABLE = new OracleParameter("C_TABLE  ", OracleDbType.RefCursor, ParameterDirection.Output);

                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                parameters.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameters);
                while (odr.Read())
                {
                    RentasActionResponseVM item = new RentasActionResponseVM();
                    item.FECHA_INICIO = odr["FECHA_INICIO"].ToString();
                    item.FECHA_FIN = odr["FECHA_FIN"].ToString();
                    item.PRODUCTO = odr["PRODUCTO"].ToString();
                    item.NRO_POLIZA = odr["NRO_POLIZA"].ToString();
                    item.MOTIVOS = odr["MOTIVOS"].ToString();
                    item.SUBMOTIVOS = odr["SUBMOTIVOS"].ToString();
                    item.NRO_TICKET = odr["NRO_TICKET"].ToString();
                    item.CUSPP = odr["CUSPP"].ToString();
                    item.ESTADO = odr["ESTADO"].ToString();
                    item.CLIENTE = odr["CLIENTE"].ToString();
                    item.TIPO_DOC_CLIENT = odr["TIPO_DOC_CLIENT"].ToString();
                    item.NRO_DOC_CLIENT = odr["NRO_DOC_CLIENT"].ToString();
                    item.BUSCAR_DOC_CLIENT = odr["BUSCAR_DOC_CLIENT"].ToString();
                    item.FBUSCAR_DOC_CLIENT = odr["FBUSCAR_DOC_CLIENT"].ToString();
                    item.SELECCIONAR_CLIENT = odr["SELECCIONAR_CLIENT"].ToString();
                    item.TIPO_PER_CLIENT = odr["TIPO_PER_CLIENT"].ToString();
                    item.NOMBRE_CLIENT = odr["NOMBRE_CLIENT"].ToString();
                    item.APELLIDOP_CLIENT = odr["APELLIDOP_CLIENT"].ToString();
                    item.APELLIDOM_CLIENT = odr["APELLIDOM_CLIENT"].ToString();
                    item.RAZON_SOCIAL_CLIENT = odr["RAZON_SOCIAL_CLIENT"].ToString();
                    item.EDITAR_LISTADO = odr["EDITAR_LISTADO"].ToString();
                    item.ASIGNAR_EJECUTIVO = odr["ASIGNAR_EJECUTIVO"].ToString();
                    item.ACCIONES = odr["ACCIONES"].ToString();
                    item.VER_DETALLE = odr["VER_DETALLE"].ToString();
                    item.ASIGNAR = odr["ASIGNAR"].ToString();
                    item.CALCULAR = odr["CALCULAR"].ToString();
                    item.CONFIRMAR = odr["CONFIRMAR"].ToString();
                    item.RECHAZAR = odr["RECHAZAR"].ToString();
                    item.DERIVAR = odr["DERIVAR"].ToString();
                    item.OBSERVAR = odr["OBSERVAR"].ToString();
                    item.APROBAR = odr["APROBAR"].ToString();
                    item.CANCELAR = odr["CANCELAR"].ToString();
                    item.BUSCAR = odr["BUSCAR"].ToString();
                    item.REFRESCAR = odr["REFRESCAR"].ToString();
                    item.BUSCAR_POR_CLIENT = odr["BUSCAR_POR_CLIENT"].ToString();
                    item.CANCELAR_CLIENT = odr["CANCELAR_CLIENT"].ToString();
                    item.CHECKBOX = odr["CHECKBOX"].ToString();
                    item.VER_JIRA = odr["VER_JIRA"].ToString();
                    item.REFRESCAR_DETALLE = odr["REFRESCAR_DETALLE"].ToString();
                    item.USUARIO_RESPONSABLE = odr["USUARIO_RESPONSABLE"].ToString();
                    item.ADJUNTAR_ARCHIVO = odr["ADJUNTAR_ARCHIVO"].ToString();
                    item.EDITAR_DESCRIPCION = odr["EDITAR_DESCRIPCION"].ToString();
                    item.EDITAR_DESCRIPCION_OK = odr["EDITAR_DESCRIPCION_OK"].ToString();
                    item.EDITAR_DESCRIPCION_X = odr["EDITAR_DESCRIPCION_X"].ToString();
                    item.DESCRIPCION_DET = odr["DESCRIPCION_DET"].ToString();
                    item.EDITAR_MOT = odr["EDITAR_MOT"].ToString();
                    item.EDITAR_MOT_OK = odr["EDITAR_MOT_OK"].ToString();
                    item.EDITAR_MOT_X = odr["EDITAR_MOT_X"].ToString();
                    item.MOTIVO_DET = odr["MOTIVO_DET"].ToString();
                    item.SUBMOTIVO_DET = odr["SUBMOTIVO_DET"].ToString();
                    item.ELIMINAR_ADJ = odr["ELIMINAR_ADJ"].ToString();
                    item.DESCARJAR_ADJ = odr["DESCARJAR_ADJ"].ToString();
                    item.SUBIR_ADJ = odr["SUBIR_ADJ"].ToString();
                    item.CONFIRMAR_DATOS_C = odr["CONFIRMAR_DATOS_C"].ToString();
                    item.CONFIRMAR_DATOS_B = odr["CONFIRMAR_DATOS_B"].ToString();
                    item.CONFIRMAR_DATOS_H = odr["CONFIRMAR_DATOS_H"].ToString();
                    item.EDITAR_FECHA_PAGO = odr["EDITAR_FECHA_PAGO"].ToString();
                    item.EDITAR_FECHA_PAGO_OK = odr["EDITAR_FECHA_PAGO_OK"].ToString();
                    item.EDITAR_FECHA_PAGO_X = odr["EDITAR_FECHA_PAGO_X"].ToString();
                    item.FECHA_PAGO_DET = odr["FECHA_PAGO_DET"].ToString();
                    list.Add(item);
                }
                response.C_TABLE = list;
                response.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                response.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                response.P_NCODE = 1;
                response.P_SMESSAGE = ex.ToString();
                LogControl.save("PD_REA_LIST_ACTIONS", ex.ToString(), "3");
            }

            return response;
        }
        //DATA ACCESS QUERY PARA RECUPERAR EL ID DE PERFIL
        public RentasNidProfileResponseVM PD_GET_NIDPROFILE(RentasNidProfileBM data)
        {
            var response = new RentasNidProfileResponseVM();
            var sPackageName = ProcedureName.pkg_rentas + ".SP_PD_GET_NIDPROFILE";
            List<OracleParameter> parameters = new List<OracleParameter>();
            List<RentasNidProfileVM> list = new List<RentasNidProfileVM>();

            try
            {
                parameters.Add(new OracleParameter("P_NIDUSER", OracleDbType.Int32, data.P_NIDUSER, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, data.P_NPRODUCT, ParameterDirection.Input));

                // OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, ParameterDirection.Output);
                OracleParameter C_TABLE = new OracleParameter("C_TABLE  ", OracleDbType.RefCursor, ParameterDirection.Output);

                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                parameters.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameters);
                while (odr.Read())
                {
                    RentasNidProfileVM item = new RentasNidProfileVM();
                    item.NIDPROFILE = Convert.ToInt32(odr["NIDPROFILE"].ToString());
                    list.Add(item);
                }
                response.C_TABLE = list;
                response.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                response.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                response.P_NCODE = 1;
                response.P_SMESSAGE = ex.ToString();
                LogControl.save("PD_GET_NIDPROFILE", ex.ToString(), "3");
            }

            return response;
        }
        //DATA ACCESS QUERY PARA RECUPERAR LAS ACCIONES DE LOS TICKETS
        public RentasListActionsTikectResponseVM PD_REA_LIST_ACTIONS_TICKET(RentasListActionsTikectBM data)
        {
            var response = new RentasListActionsTikectResponseVM();
            var sPackageName = ProcedureName.pkg_rentas + ".SP_PD_REA_LIST_ACTIONS_TICKET";
            List<OracleParameter> parameters = new List<OracleParameter>();
            List<RentasListActionsTikectVM> list = new List<RentasListActionsTikectVM>();

            try
            {
                parameters.Add(new OracleParameter("P_SCODE", OracleDbType.Varchar2, data.P_SCODE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, data.P_NUSERCODE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, data.P_NPRODUCT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NIDPROFILE", OracleDbType.Int32, data.P_NIDPROFILE, ParameterDirection.Input));

                // OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, ParameterDirection.Output);
                OracleParameter C_TABLE = new OracleParameter("C_TABLE  ", OracleDbType.RefCursor, ParameterDirection.Output);

                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                parameters.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameters);
                while (odr.Read())
                {
                    RentasListActionsTikectVM item = new RentasListActionsTikectVM();
                    item.VER_DETALLE = odr["VER_DETALLE"].ToString();
                    item.ASIGNAR = odr["ASIGNAR"].ToString();
                    item.CALCULAR = odr["CALCULAR"].ToString();
                    item.CONFIRMAR = odr["CONFIRMAR"].ToString();
                    item.RECHAZAR = odr["RECHAZAR"].ToString();
                    item.DERIVAR = odr["DERIVAR"].ToString();
                    item.OBSERVAR = odr["OBSERVAR"].ToString();
                    item.APROBAR = odr["APROBAR"].ToString();
                    item.CANCELAR = odr["CANCELAR"].ToString();
                    list.Add(item);
                }
                response.C_TABLE = list;
                response.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                response.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                response.P_NCODE = 1;
                response.P_SMESSAGE = ex.ToString();
                LogControl.save("PD_REA_LIST_ACTIONS_TICKET", ex.ToString(), "3");
            }

            return response;
        }
        //DATA ACCESS QUERY PARA ACTUALIZAR EL ESTADO DE TICKET
        public RentasStatusTicketTikectResponseVM PD_UPD_STATUS_TICKET(RentasStatusTicketTikectBM data)
        {
            var response = new RentasStatusTicketTikectResponseVM();
            var sPackageName = ProcedureName.pkg_rentas + ".SP_PD_UPD_STATUS_TICKET";
            List<OracleParameter> parameters = new List<OracleParameter>();

            try
            {
                parameters.Add(new OracleParameter("P_SCODE_JIRA", OracleDbType.Varchar2, data.P_SCODE_JIRA, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, data.P_NUSERCODE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NIDPROFILE", OracleDbType.Int32, data.P_NIDPROFILE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SNAME_ACT", OracleDbType.Varchar2, data.P_SNAME_ACT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NTYPECOMMENT", OracleDbType.Varchar2, data.P_NTYPECOMMENT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SCOMMETS", OracleDbType.Varchar2, data.P_SCOMMETS, ParameterDirection.Input));

                // OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, string.Empty, ParameterDirection.Output);

                P_SMESSAGE.Size = 4000;
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameters);

                response.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                response.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                response.P_NCODE = 1;
                response.P_SMESSAGE = ex.Message;
                LogControl.save("PD_UPD_STATUS_TICKET", ex.ToString(), "3");
            }

            return response;
        }
        public async Task<RentasGetPolicyDataResponseVM> GET_POLICY_DATA(RentasGetPolicyDataBM data)
        {

            var response = new RentasGetPolicyDataResponseVM();

            try
            {

                string urlServicio = String.Format(data.NTYPE_SYSTEM == 1 ? ELog.obtainConfig("BackOffice") : ELog.obtainConfig("Seacsa"));
                var json = await WebServiceUtil.GetServicioRentas(JsonConvert.SerializeObject(data), urlServicio, "token");
                response = JsonConvert.DeserializeObject<RentasGetPolicyDataResponseVM>(json, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            catch (Exception ex)
            {
                LogControl.save("GET_POLICY_DATA", ex.ToString(), "3");
            }

            return await Task.FromResult(response);
        }

        public async Task<RentasGetCalculationAmountResponseVM> GET_CALCULATION_AMOUNT(RentasGetCalculationAmountBM data)
        {

            var response = new RentasGetCalculationAmountResponseVM();

            try
            {
                //string urlServicio = String.Format(data.NTYPE_SYSTEM == 1 ? ELog.obtainConfig("BackOffice") : ELog.obtainConfig("SEACSA"));
                string urlServicio = String.Format(data.NTYPE_SYSTEM == 1 ? ELog.obtainConfig("BackOffice") : ELog.obtainConfig("SEACSA"));
                var json = await WebServiceUtil.GetServicioRentas(JsonConvert.SerializeObject(data), urlServicio + "?results=4&page=1&pageSize=4", "token");
                response = JsonConvert.DeserializeObject<RentasGetCalculationAmountResponseVM>(json, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            catch (Exception ex)
            {
                LogControl.save("GET_CALCULATION_AMOUNT", ex.ToString(), "3");
            }

            return await Task.FromResult(response);
        }
        public async Task<RentasGetCalculationAmountResponseVM> GET_CALCULATION_AMOUNT_DUMMY(RentasGetCalculationAmountBM data)
        {
            var response = new RentasGetCalculationAmountResponseVM();
            var sPackageName = ProcedureName.pkg_rentas + ".SP_PD_GET_CALCULATION_AMOUNT";
            List<OracleParameter> parameters = new List<OracleParameter>();
            List<RentasNidProfileVM> list = new List<RentasNidProfileVM>();

            try
            {
                parameters.Add(new OracleParameter("P_NRAMO", OracleDbType.Int32, data.P_NRAMO, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, data.P_NPRODUCT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPOLIZA", OracleDbType.Int32, data.P_NPOLIZA, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NTIPO_DOC", OracleDbType.Int32, data.P_NTIPO_DOC, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NNUM_DOC", OracleDbType.Int32, data.P_NNUM_DOC, ParameterDirection.Input));


                // OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, ParameterDirection.Output);
                OracleParameter C_TABLE = new OracleParameter("C_TABLE  ", OracleDbType.RefCursor, ParameterDirection.Output);

                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                parameters.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameters);
                while (odr.Read())
                {
                    RentasNidProfileVM item = new RentasNidProfileVM();
                    response.IMPORTE = Convert.ToInt32(odr["IMPORTE"].ToString());
                    response.MONEDA = Convert.ToInt32(odr["MONEDA"].ToString());
                }
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("PD_GET_NIDPROFILE", ex.ToString(), "3");
            }

            return response;
        }
        //DATA ACCESS QUERY PARA ACTUALIZAR LA MONEDA Y EL IMPORTE DEL TICKET
        public RentasGetCalculationAmountVM PD_UPD_AMOUNT_TICKET(RentasUpdAmountTicketBM data)
        {
            var response = new RentasGetCalculationAmountVM();
            var sPackageName = ProcedureName.pkg_rentas + ".SP_PD_UPD_AMOUNT_TICKET";
            List<OracleParameter> parameters = new List<OracleParameter>();

            try
            {
                parameters.Add(new OracleParameter("P_NTICKET", OracleDbType.Int32, data.P_NTICKET, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_IMPORTE", OracleDbType.Int32, data.P_IMPORTE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_MONEDA", OracleDbType.Int32, data.P_MONEDA, ParameterDirection.Input));


                // OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, ParameterDirection.Output);

                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameters);

                response.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                response.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                response.P_NCODE = 1;
                response.P_SMESSAGE = ex.ToString();
                LogControl.save("PD_UPD_STATUS_TICKET", ex.ToString(), "3");
            }

            return response;
        }
        public RentasListAdjResponseVM PD_LIST_ADJ(RentasListAdjBM data)
        {
            var response = new RentasListAdjResponseVM();
            var sPackageName = ProcedureName.pkg_rentas + ".SP_PD_LIST_ADJ";
            List<OracleParameter> parameters = new List<OracleParameter>();
            List<RentasListAdjVM> list = new List<RentasListAdjVM>();

            try
            {
                parameters.Add(new OracleParameter("P_NTICKET", OracleDbType.Int32, data.P_NTICKET, ParameterDirection.Input));

                // OUTPUT

                OracleParameter P_CURSOR = new OracleParameter("P_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, ParameterDirection.Output);

                parameters.Add(P_CURSOR);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameters);
                while (odr.Read())
                {
                    RentasListAdjVM item = new RentasListAdjVM();
                    item.NID = odr["NID"].ToString();
                    item.SNAME = odr["SNAME"].ToString();
                    item.SSIZE = odr["SSIZE"].ToString();
                    item.SPATH = odr["SPATH"].ToString();
                    item.SPATH_GD = odr["SPATH_GD"].ToString();
                    item.NTYPEATTACHMENT = odr["NTYPEATTACHMENT"] != DBNull.Value ? Convert.ToInt32(odr["NTYPEATTACHMENT"]) : (int?)null;
                    list.Add(item);
                }
                response.P_CURSOR = list;
                response.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                response.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                response.P_NCODE = 1;
                response.P_SMESSAGE = ex.ToString();
                LogControl.save("PD_LIST_ADJ", ex.ToString(), "3");
            }

            return response;
        }
        public RentasUpdAttachmentVM PD_UPD_ATTACHMENT(RentasUpdAttachmentBM data)
        {
            var response = new RentasUpdAttachmentVM();
            var sPackageName = ProcedureName.pkg_rentas + ".SP_PD_UPD_ATTACHMENT";
            List<OracleParameter> parameters = new List<OracleParameter>();

            try
            {
                parameters.Add(new OracleParameter("P_NTICKET", OracleDbType.Int32, data.P_NTICKET, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NTYPEATTACHMENT", OracleDbType.Int32, data.P_NTYPEATTACHMENT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NID", OracleDbType.Int32, data.P_NID, ParameterDirection.Input));

                // OUTPUT

                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, ParameterDirection.Output);

                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameters);
                response.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                response.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                response.P_NCODE = 1;
                response.P_SMESSAGE = ex.ToString();
                LogControl.save("PD_LIST_ADJ", ex.ToString(), "3");
            }

            return response;
        }
        public RentasInsDataEmailResponseVM PD_INS_DATA_EMAIL(RentasInsDataEmailBM data)
        {
            var response = new RentasInsDataEmailResponseVM();
            var sPackageName = ProcedureName.pkg_rentas + ".SP_PD_INS_DATA_EMAIL";
            List<OracleParameter> parameters = new List<OracleParameter>();

            try
            {
                parameters.Add(new OracleParameter("P_NTICKET", OracleDbType.Int32, data.P_NTICKET, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SEMAIL_MESSAGE", OracleDbType.Varchar2, data.P_SEMAIL_MESSAGE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SEMAIL_SUBJECT", OracleDbType.Varchar2, data.P_SEMAIL_SUBJECT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SMASK_EMAIL", OracleDbType.Varchar2, data.P_SMASK_EMAIL, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SRECIPIENT_EMAIL", OracleDbType.Varchar2, data.P_SRECIPIENT_EMAIL, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.P_NBRANCH, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, data.P_NPRODUCT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NMOTIV", OracleDbType.Int32, data.P_NMOTIV, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NSUBMOTIV", OracleDbType.Int32, data.P_NSUBMOTIV, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int32, data.P_NPOLICY, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, data.P_NUSERCODE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCOMMUNICATION_TYPE", OracleDbType.Int32, data.P_NCOMMUNICATION_TYPE, ParameterDirection.Input));

                // OUTPUT

                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, ParameterDirection.Output);

                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameters);
                response.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                response.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                response.P_NCODE = 1;
                response.P_SMESSAGE = ex.ToString();
                LogControl.save("PD_LIST_ADJ", ex.ToString(), "3");
            }

            return response;
        }

        public string PD_GET_ROUTE_FILE()
        {
            var sPackageName = ProcedureName.pkg_rentas + ".SP_PD_GET_ROUTE_FILE";
            List<OracleParameter> parameters = new List<OracleParameter>();
            string firstSRoute = string.Empty;

            try
            {
                // OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameters.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameters);
                if (odr.Read()) // Solo lee la primera fila
                {
                    firstSRoute = odr["SROUTE"].ToString();
                }
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("PD_GET_ROUTE_FILE", ex.ToString(), "3");
            }

            return firstSRoute;
        }

        public RentasInsDataEmailResponseVM PD_INS_TBL_TICK_ADJUNT(RentasInsTickAdjuntBM data)
        {
            var response = new RentasInsDataEmailResponseVM();
            var sPackageName = ProcedureName.pkg_rentas + ".SP_PD_INS_TBL_TICK_ADJUNT";
            List<OracleParameter> parameters = new List<OracleParameter>();

            try
            {
                parameters.Add(new OracleParameter("P_SCODE", OracleDbType.Varchar2, data.P_SCODE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NTICKET", OracleDbType.Int32, data.P_NTICKET, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SNAME", OracleDbType.Varchar2, data.P_SNAME, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SSIZE", OracleDbType.Varchar2, data.P_SSIZE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SPATH", OracleDbType.Varchar2, data.P_SPATH, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, data.P_NUSERCODE, ParameterDirection.Input));
                // OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, string.Empty, ParameterDirection.Output);

                P_SMESSAGE.Size = 4000;
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameters);
                response.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                response.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                response.P_NCODE = 1;
                response.P_SMESSAGE = ex.ToString();
                LogControl.save("PD_LIST_ADJ", ex.ToString(), "3");
            }

            return response;
        }

        public RentasInsDataEmailResponseVM PD_DEL_TBL_TICK_ADJUNT(RentasDelTickAdjuntBM data)
        {
            var response = new RentasInsDataEmailResponseVM();
            var sPackageName = ProcedureName.pkg_rentas + ".SP_PD_DEL_TBL_TICK_ADJUNT";
            List<OracleParameter> parameters = new List<OracleParameter>();

            try
            {
                parameters.Add(new OracleParameter("P_NID", OracleDbType.Int32, data.P_NID, ParameterDirection.Input));

                // OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, string.Empty, ParameterDirection.Output);

                P_SMESSAGE.Size = 4000;
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameters);
                response.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                response.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                response.P_NCODE = 1;
                response.P_SMESSAGE = ex.ToString();
                LogControl.save("PD_LIST_ADJ", ex.ToString(), "3");
            }

            return response;
        }
        public RentasUpdTicketDescriptResponseVM PD_UPD_TICKET_DESCRIPT(RentasUpdTicketDescriptBM data)
        {
            var response = new RentasUpdTicketDescriptResponseVM();
            var sPackageName = ProcedureName.pkg_rentas + ".SP_PD_UPD_TICKET_DESCRIPT";
            List<OracleParameter> parameters = new List<OracleParameter>();

            try
            {
                parameters.Add(new OracleParameter("P_NTICKET", OracleDbType.Int32, data.P_NTICKET, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SDESCRIPT", OracleDbType.Varchar2, data.P_SDESCRIPT, ParameterDirection.Input));

                // OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, string.Empty, ParameterDirection.Output);

                P_SMESSAGE.Size = 4000;
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameters);
                response.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                response.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                response.P_NCODE = 1;
                response.P_SMESSAGE = ex.ToString();
                LogControl.save("PD_LIST_ADJ", ex.ToString(), "3");
            }

            return response;
        }
        public RentasUpdTicketNmotivResponseVM PD_UPD_TICKET_NMOTIV(RentasUpdTicketNmotivBM data)
        {
            var response = new RentasUpdTicketNmotivResponseVM();
            var sPackageName = ProcedureName.pkg_rentas + ".SP_PD_UPD_TICKET_NMOTIV";
            List<OracleParameter> parameters = new List<OracleParameter>();

            try
            {
                parameters.Add(new OracleParameter("P_NTICKET", OracleDbType.Int32, data.P_NTICKET, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NMOTIV", OracleDbType.Int32, data.P_NMOTIV, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NSUBMOTIV", OracleDbType.Int32, data.P_NSUBMOTIV, ParameterDirection.Input));

                // OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, string.Empty, ParameterDirection.Output);

                P_SMESSAGE.Size = 4000;
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameters);
                response.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                response.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                response.P_NCODE = 1;
                response.P_SMESSAGE = ex.ToString();
                LogControl.save("PD_LIST_ADJ", ex.ToString(), "3");
            }

            return response;
        }
        public RentasGetUserResponsibleResponseVM PD_GET_USER_RESPONSIBLE(RentasGetUserResponsibleBM data)
        {
            var response = new RentasGetUserResponsibleResponseVM();
            var sPackageName = ProcedureName.pkg_rentas + ".SP_PD_GET_USER_RESPONSIBLE";
            List<OracleParameter> parameters = new List<OracleParameter>();
            List<RentasGetUserResponsibleVM> list = new List<RentasGetUserResponsibleVM>();

            try
            {
                parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, data.P_NUSERCODE, ParameterDirection.Input));

                // OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, ParameterDirection.Output);
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);

                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                parameters.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameters);
                while (odr.Read())
                {
                    RentasGetUserResponsibleVM item = new RentasGetUserResponsibleVM();
                    item.NCODE = Convert.ToInt32(odr["NCODE"].ToString());
                    item.SDESCRIPT = odr["SDESCRIPT"].ToString();
                    list.Add(item);
                }
                response.C_TABLE = list;
                response.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                response.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                response.P_NCODE = 1;
                response.P_SMESSAGE = ex.ToString();
                LogControl.save("PD_GET_USER_RESPONSIBLE", ex.ToString(), "3");
            }

            return response;
        }
        public RentasGetValpopupResponseVM PD_GET_VALPOPUP(RentasGetValpopupBM data)
        {
            var response = new RentasGetValpopupResponseVM();
            var sPackageName = ProcedureName.pkg_rentas + ".SP_PD_GET_VALPOPUP";
            List<OracleParameter> parameters = new List<OracleParameter>();
            List<RentasGetValpopupVM> list = new List<RentasGetValpopupVM>();

            try
            {
                parameters.Add(new OracleParameter("P_NTICKET", OracleDbType.Int32, data.P_NTICKET, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SNAME_ACT", OracleDbType.Varchar2, data.P_SNAME_ACT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NIDPROFILE", OracleDbType.Int32, data.P_NIDPROFILE, ParameterDirection.Input));

                // OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, ParameterDirection.Output);
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);

                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                parameters.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameters);
                while (odr.Read())
                {
                    RentasGetValpopupVM item = new RentasGetValpopupVM();
                    item.NPOP_UP = Convert.ToInt32(odr["NPOP_UP"].ToString());
                    list.Add(item);
                }
                response.C_TABLE = list;
                response.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                response.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                response.P_NCODE = 1;
                response.P_SMESSAGE = ex.ToString();
                LogControl.save("PD_GET_VALPOPUP", ex.ToString(), "3");
            }

            return response;
        }

        public RentasGenericResponseVM PD_GET_TYPECOMMENT()
        {
            var response = new RentasGenericResponseVM();
            var sPackageName = ProcedureName.pkg_rentas + ".SP_PD_GET_TYPECOMMENT";
            List<OracleParameter> parameters = new List<OracleParameter>();
            List<RentasGetTypeCommentResponseVM> list = new List<RentasGetTypeCommentResponseVM>();

            try
            {
                // OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, ParameterDirection.Output);
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);

                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                parameters.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameters);
                while (odr.Read())
                {
                    RentasGetTypeCommentResponseVM item = new RentasGetTypeCommentResponseVM();
                    item.NCODE = Convert.ToInt32(odr["NCODE"].ToString());
                    item.SDESCRIPT = odr["SDESCRIPT"].ToString();
                    list.Add(item);
                }
                response.C_TABLE = list;
                response.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                response.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                response.P_NCODE = 1;
                response.P_SMESSAGE = ex.ToString();
                LogControl.save("PD_GET_TYPECOMMENT", ex.ToString(), "3");
            }

            return response;
        }
        public RentasGenericResponseVM PD_GET_DESTINATION()
        {
            var response = new RentasGenericResponseVM();
            var sPackageName = ProcedureName.pkg_rentas + ".SP_PD_GET_DESTINATION";
            List<OracleParameter> parameters = new List<OracleParameter>();
            List<RentasGetDestinationResponseVM> list = new List<RentasGetDestinationResponseVM>();

            try
            {
                // OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, ParameterDirection.Output);
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);

                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                parameters.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameters);
                while (odr.Read())
                {
                    RentasGetDestinationResponseVM item = new RentasGetDestinationResponseVM();
                    item.NCODE = Convert.ToInt32(odr["NCODE"].ToString());
                    item.SDESCRIPT = odr["SDESCRIPT"].ToString();
                    list.Add(item);
                }
                response.C_TABLE = list;
                response.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                response.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                response.P_NCODE = 1;
                response.P_SMESSAGE = ex.ToString();
                LogControl.save("PD_GET_DESTINATION", ex.ToString(), "3");
            }

            return response;
        }

        public RentasGenericResponseVM PD_GET_EMAIL_DESTINATION(RentasGetEmailDestinationBM data)
        {
            var response = new RentasGenericResponseVM();
            var sPackageName = ProcedureName.pkg_rentas + ".SP_PD_GET_EMAIL_DESTINATION";
            List<OracleParameter> parameters = new List<OracleParameter>();
            List<RentasGetEmailDestinationVM> list = new List<RentasGetEmailDestinationVM>();

            try
            {
                parameters.Add(new OracleParameter("P_STYPE_DEST", OracleDbType.Varchar2, data.P_STYPE_DEST, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NTICKET", OracleDbType.Int32, data.P_NTICKET, ParameterDirection.Input));



                // OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, string.Empty, ParameterDirection.Output);
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);

                P_SMESSAGE.Size = 4000;
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                parameters.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameters);
                while (odr.Read())
                {
                    RentasGetEmailDestinationVM item = new RentasGetEmailDestinationVM();
                    item.SEMAILS = odr["SEMAILS"].ToString();
                    list.Add(item);
                }
                response.C_TABLE = list;
                response.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                response.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                response.P_NCODE = 1;
                response.P_SMESSAGE = ex.ToString();
                LogControl.save("PD_GET_DESTINATION", ex.ToString(), "3");
            }

            return response;
        }
        public RentasGenericResponseVM PD_GET_EMAIL_USER(RentasGetEmailUserBM data)
        {
            var response = new RentasGenericResponseVM();
            var sPackageName = ProcedureName.pkg_rentas + ".SP_PD_GET_EMAIL_USER";
            List<OracleParameter> parameters = new List<OracleParameter>();
            List<RentasGetEmailUserVM> list = new List<RentasGetEmailUserVM>();

            try
            {
                parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, data.P_NUSERCODE, ParameterDirection.Input));



                // OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, string.Empty, ParameterDirection.Output);
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);

                P_SMESSAGE.Size = 4000;
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                parameters.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameters);
                while (odr.Read())
                {
                    RentasGetEmailUserVM item = new RentasGetEmailUserVM();
                    item.SEMAIL = odr["SEMAIL"].ToString();
                    list.Add(item);
                }
                response.C_TABLE = list;
                response.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                response.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                response.P_NCODE = 1;
                response.P_SMESSAGE = ex.ToString();
                LogControl.save("PD_GET_DESTINATION", ex.ToString(), "3");
            }

            return response;
        }

        public RentasGenericResponseVM PD_GET_CONF_FILE()
        {
            var response = new RentasGenericResponseVM();
            var sPackageName = ProcedureName.pkg_rentas + ".SP_PD_GET_CONF_FILE";
            List<OracleParameter> parameters = new List<OracleParameter>();
            List<RentasGetConfFileVM> list = new List<RentasGetConfFileVM>();

            try
            {
                // OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, string.Empty, ParameterDirection.Output);
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);

                P_SMESSAGE.Size = 4000;
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                parameters.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameters);
                while (odr.Read())
                {
                    RentasGetConfFileVM item = new RentasGetConfFileVM();
                    item.FILE_CANT_TK = odr["FILE_CANT_TK"].ToString();
                    item.FILE_SIZE = odr["FILE_SIZE"].ToString();
                    item.FILE_FORMATS = odr["FILE_FORMATS"].ToString();
                    list.Add(item);
                }
                response.C_TABLE = list;
                response.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                response.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                response.P_NCODE = 1;
                response.P_SMESSAGE = ex.ToString();
                LogControl.save("PD_GET_DESTINATION", ex.ToString(), "3");
            }

            return response;
        }

        public RentasGenericResponseVM PD_GET_MESSAGE(RentasGetMessage data)
        {
            var response = new RentasGenericResponseVM();
            var sPackageName = ProcedureName.pkg_rentas + ".SP_PD_GET_MESSAGE";
            List<OracleParameter> parameters = new List<OracleParameter>();
            List<RentasGetMessageVM> list = new List<RentasGetMessageVM>();

            try
            {
                parameters.Add(new OracleParameter("P_NERRORNUM", OracleDbType.Int32, data.P_NERRORNUM, ParameterDirection.Input));

                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);


                parameters.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameters);
                while (odr.Read())
                {
                    RentasGetMessageVM item = new RentasGetMessageVM();
                    item.SMESSAGE = odr["SMESSAGE"].ToString();
                    list.Add(item);
                }
                response.C_TABLE = list;
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                response.P_NCODE = 1;
                response.P_SMESSAGE = ex.ToString();
                LogControl.save("PD_GET_MESSAGE", ex.ToString(), "3");
            }

            return response;
        }
    }
}


