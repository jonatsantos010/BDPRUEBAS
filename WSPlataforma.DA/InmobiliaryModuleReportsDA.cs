using System;
using System.IO;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Configuration;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using WSPlataforma.Util;
using WSPlataforma.Entities.InmobiliaryModuleReportsModel.ViewModel;
using WSPlataforma.Entities.InmobiliaryModuleReportsModel.BindingModel;
using SpreadsheetLight;
using DocumentFormat.OpenXml.Spreadsheet;

namespace WSPlataforma.DA
{
    public class InmobiliaryModuleReportsDA : ConnectionBase
    {
        private readonly string Package = "PKG_OI_REPORTS";
        private string pathReport = string.Empty;
        private string fileName = string.Empty;

        // REPORTE CONTROL BANCARIO
        public DataSet ProcessReportControlBancario(ModuleReportsFilter data)
        {

            DataSet dataReport = new DataSet();
            DataTable dataCab = new DataTable();

            int Pcode;
            string Pmessage;

            try
            {
                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleINMOB_OI"].ToString()))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = cn;
                        cmd.CommandText = string.Format("{0}.{1}", Package, "USP_OI_REPORT_BANK_CONTROL");
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new OracleParameter("P_NNUMORI", OracleDbType.Int16, data.P_NNUMORI, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NCODGRU", OracleDbType.Int16, data.P_NCODGRU, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int16, data.P_NBRANCH, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_FECHAINI", OracleDbType.Date, data.P_FECHAINI, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_FECHAFIN", OracleDbType.Date, data.P_FECHAFIN, ParameterDirection.Input));

                        OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                        OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                        cmd.Parameters.Add(P_NCODE);
                        cmd.Parameters.Add(P_SMESSAGE);
                        cmd.Parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
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
                        dataCab.TableName = "Control Bancario";
                        dataReport.Tables.Add(dataCab);
                        var cantCab = dataCab.Rows.Count;


                        //OBTIENE CONFIGURACION DE REPORTE
                        data.P_NREPORT = 3; //CONTROL BANCARIO
                        data.P_NTIPO = 1;   //DETALLE
                        DataTable configCBCO = this.GetDataFields(data);

                        List<ConfigFields> configurationCBCO = this.GetFieldsConfiguration(configCBCO, data);

                        pathReport = ELog.obtainConfig("reportOnlineInterfaceCBCO");
                        fileName = "RpteControlBancario.xlsx";
                        //ExportToExcel(dataCab, pathReport, fileName);

                        ExportToExcelCD(dataReport, configurationCBCO, configurationCBCO, pathReport, fileName);
                        cn.Close();

                    }
                }
            }
            catch (Exception ex)
            {
                LogControl.save("ProcessReportControlBancario", ex.ToString(), "3");
                throw;
            }
            return dataReport;
        }

        // NUEVO REPORTE OPERATIVO CAB/DET
        public DataSet ProcessReportOperativo(ModuleReportsFilter data)
        {

            DataSet dataReport = new DataSet();
            DataTable dataCab = new DataTable();
            DataTable dataDet = new DataTable();

            if (data.P_NNUMORI != 1)
            {
                data.P_NBRANCH = data.P_NNUMORI;
            }

            int Pcode;
            string Pmessage;

            int prod;
            if (data.P_NPRODUCT == "" || data.P_NPRODUCT == null)
            {
                prod = 0;
            }
            else
            {
                prod = Int32.Parse(data.P_NPRODUCT);
            }

            try
            {
                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleINMOB_OI"].ToString()))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = cn;
                        cmd.CommandText = string.Format("{0}.{1}", Package, "USP_OI_REPORT_OPERATIVE_CD");
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new OracleParameter("P_NNUMORI", OracleDbType.Int16, data.P_NNUMORI, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NCODGRU", OracleDbType.Int16, data.P_NCODGRU, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int16, data.P_NBRANCH, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_FECHAINI", OracleDbType.Date, data.P_FECHAINI, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_FECHAFIN", OracleDbType.Date, data.P_FECHAFIN, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int16, prod, ParameterDirection.Input));

                        OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                        OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                        cmd.Parameters.Add(P_NCODE);
                        cmd.Parameters.Add(P_SMESSAGE);
                        cmd.Parameters.Add(new OracleParameter("C_TABLE_CAB", OracleDbType.RefCursor, ParameterDirection.Output));
                        cmd.Parameters.Add(new OracleParameter("C_TABLE_DET", OracleDbType.RefCursor, ParameterDirection.Output));
                        cn.Open();


                        cmd.ExecuteNonQuery();

                        Pcode = Convert.ToInt32(P_NCODE.Value.ToString());
                        Pmessage = P_SMESSAGE.Value.ToString();

                        if (Pcode == 1)
                        {
                            throw new Exception(Pmessage);
                        }

                        //Obtenemos el cursor de detalle
                        OracleRefCursor cursorDet = (OracleRefCursor)cmd.Parameters["C_TABLE_DET"].Value;
                        dataDet.Load(cursorDet.GetDataReader());
                        dataDet.TableName = "DETALLE";
                        dataReport.Tables.Add(dataDet);
                        var cantDet = dataDet.Rows.Count;

                        //Obtenemos el cursor de resumen
                        OracleRefCursor cursorCab = (OracleRefCursor)cmd.Parameters["C_TABLE_CAB"].Value;
                        dataCab.Load(cursorCab.GetDataReader());
                        dataCab.TableName = "RESUMEN";
                        dataReport.Tables.Add(dataCab);
                        var cantCab = dataCab.Rows.Count;

                        //OBTIENE CONFIGURACION DE REPORTE
                        data.P_NREPORT = 2; //OPERATIVO
                        data.P_NTIPO = 1;   // RESUMEN
                        DataTable configRES = this.GetDataFields(data);

                        data.P_NTIPO = 2;   // RESUMEN
                        DataTable configDET = this.GetDataFields(data);

                        List<ConfigFields> configurationCAB = this.GetFieldsConfiguration(configRES, data);
                        List<ConfigFields> configurationDET = this.GetFieldsConfiguration(configDET, data);

                        pathReport = ELog.obtainConfig("reportOnlineInterfaceOperativo");
                        fileName = "RpteOperativo.xlsx";
                        ExportToExcelCD(dataReport, configurationCAB, configurationDET, pathReport, fileName);

                        cn.Close();

                    }
                }
            }
            catch (Exception ex)
            {
                LogControl.save("ProcessReportOperativo", ex.ToString(), "3");
                throw;
            }
            return dataReport;
        }

        // NUEVO REPORTE CONTABLE CAB/DET
        public DataSet ProcessReportContabilidad(ModuleReportsFilter data)
        {
            DataSet dataReport = new DataSet();
            DataTable dataCab = new DataTable();
            DataTable dataDet = new DataTable();

            int Pcode;
            string Pmessage;

            int prod;
            if (data.P_NPRODUCT == "" || data.P_NPRODUCT == null)
            {
                prod = 0;
            }
            else
            {
                prod = Int32.Parse(data.P_NPRODUCT);
            }

            if (data.P_NNUMORI != 1)
            {
                data.P_NBRANCH = data.P_NNUMORI;
            }

            try
            {
                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleINMOB_OI"].ToString()))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = cn;
                        cmd.CommandText = string.Format("{0}.{1}", Package, "USP_OI_REPORT_CONTABLE_CD");
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new OracleParameter("P_NNUMORI", OracleDbType.Int32, data.P_NNUMORI, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NCODGRU", OracleDbType.Int32, data.P_NCODGRU, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.P_NBRANCH, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_FECHAINI", OracleDbType.Date, data.P_FECHAINI, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_FECHAFIN", OracleDbType.Date, data.P_FECHAFIN, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, prod, ParameterDirection.Input));

                        OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                        OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                        cmd.Parameters.Add(P_NCODE);
                        cmd.Parameters.Add(P_SMESSAGE);
                        cmd.Parameters.Add(new OracleParameter("C_TABLE_CAB", OracleDbType.RefCursor, ParameterDirection.Output));
                        cmd.Parameters.Add(new OracleParameter("C_TABLE_DET", OracleDbType.RefCursor, ParameterDirection.Output));
                        cn.Open();


                        cmd.ExecuteNonQuery();

                        Pcode = Convert.ToInt32(P_NCODE.Value.ToString());
                        Pmessage = P_SMESSAGE.Value.ToString();

                        if (Pcode == 1)
                        {
                            throw new Exception(Pmessage);
                        }

                        //Obtenemos el cursor de Detalle
                        OracleRefCursor cursorDet = (OracleRefCursor)cmd.Parameters["C_TABLE_DET"].Value;
                        dataDet.Load(cursorDet.GetDataReader());
                        dataDet.TableName = "DETALLE";
                        dataReport.Tables.Add(dataDet);
                        var cantDet = dataDet.Rows.Count;

                        //Obtenemos el cursor de resumen
                        OracleRefCursor cursorCab = (OracleRefCursor)cmd.Parameters["C_TABLE_CAB"].Value;
                        dataCab.Load(cursorCab.GetDataReader());
                        dataCab.TableName = "RESUMEN";
                        dataReport.Tables.Add(dataCab);
                        var cantCab = dataCab.Rows.Count;

                        //OBTIENE CONFIGURACION DE REPORTE
                        data.P_NREPORT = 1; //CONTABLE
                        data.P_NTIPO = 1;   // RESUMEN
                        DataTable configRES = this.GetDataFields(data);

                        data.P_NTIPO = 2;   // RESUMEN
                        DataTable configDET = this.GetDataFields(data);

                        List<ConfigFields> configurationCAB = this.GetFieldsConfiguration(configRES, data);
                        List<ConfigFields> configurationDET = this.GetFieldsConfiguration(configDET, data);

                        pathReport = ELog.obtainConfig("reportOnlineInterfaceContable");
                        fileName = "RpteContable.xlsx";
                        ExportToExcelCD(dataReport, configurationCAB, configurationDET, pathReport, fileName);

                        cn.Close();

                    }
                }
            }
            catch (Exception ex)
            {
                LogControl.save("ProcessReportContabilidad", ex.ToString(), "3");
                throw;
            }
            return dataReport;
        }

        #region METODO GENERACION RPTE EXCEL
        // GENERA ARCHIVO EXCEL CON DATA DEL REPORTE (1 hoja)
        public void ExportToExcel(DataTable dt, string pathReport, string fileName)
        {
            try
            {
                int RowInital = 2;
                var contador = 1;
                int x = 1;
                var sl = new SLDocument();
                var Style = sl.CreateStyle();
                sl.RenameWorksheet(SLDocument.DefaultFirstSheetName, dt.TableName);
                //INSERTA TITULO
                CreateStyle(ref Style); // ASIGNA FORMATO DE TITULOS AL REPORTE EXCEL
                foreach (DataColumn Col in dt.Columns)
                {
                    SetValueWithStyle(ref sl, 1, contador, Col.ColumnName, Style);
                    contador++;
                }
                int rowTbl = 0;
                //INSERTA CUERPO
                foreach (DataRow row in dt.Rows)
                {
                    for (int i = 1; i <= dt.Columns.Count; i++)
                    {

                        sl.SetCellValue(RowInital, i, dt.Rows[rowTbl][i - 1].ToString());

                    }
                    RowInital++;
                    rowTbl++;
                }
                sl.AutoFitColumn(1, contador);

                if (!Directory.Exists(pathReport))
                    Directory.CreateDirectory(pathReport);
                sl.SaveAs(CombinePath(pathReport, fileName));
            }
            catch (Exception ex)
            {
                throw new Exception("ExportToExcel: \n" + ex.Message);
            }
        }


        public void ExportToExcelCD(DataSet ds, List<ConfigFields> configRES, List<ConfigFields> configDET, string pathReport, string fileName)
        {
            try
            {
                var sl = new SLDocument();
                var Style = sl.CreateStyle();
                ConfigFields item;

                for (int x = 0; x <= ds.Tables.Count - 1; x++)
                {
                    int RowInital = 2;
                    var contador = 1;
                    DataTable dt = new DataTable();

                    dt = ds.Tables[x];

                    sl.AddWorksheet(dt.TableName);

                    //INSERTA TITULO
                    CreateStyle(ref Style); // ASIGNA FORMATO DE TITULOS AL REPORTE EXCEL
                    foreach (DataColumn Col in dt.Columns)
                    {
                        SetValueWithStyle(ref sl, 1, contador, Col.ColumnName, Style);
                        contador++;
                    }
                    int rowTbl = 0;
                    //INSERTA CUERPO
                    foreach (DataRow row in dt.Rows)
                    {
                        for (int i = 1; i <= dt.Columns.Count; i++)
                        {
                            var columncurrent = (DataColumn)dt.Columns[i - 1];
                            if (x == 0)
                            {
                                item = configDET.Where(column => column.SCOLUMNAME == columncurrent.ColumnName).FirstOrDefault();
                            }
                            else
                            {
                                item = configRES.Where(column => column.SCOLUMNAME == columncurrent.ColumnName).FirstOrDefault();
                            }
                            if (item.SDATATYPE == "NUMBER" && dt.Rows[rowTbl][i - 1].ToString() != string.Empty)
                            {
                                sl.SetCellValue(RowInital, i, Convert.ToDecimal(dt.Rows[rowTbl][i - 1].ToString()));
                            }
                            else
                            {
                                if (item.SDATATYPE == "DATE" && dt.Rows[rowTbl][i - 1].ToString() != string.Empty)
                                {
                                    DateTime fecha = Convert.ToDateTime(dt.Rows[rowTbl][i - 1].ToString());
                                    sl.SetCellValue(RowInital, i, Convert.ToString(fecha.ToString("dd/MM/yyyy")));
                                }
                                else
                                {
                                    sl.SetCellValue(RowInital, i, dt.Rows[rowTbl][i - 1].ToString());
                                }
                            }
                        }
                        RowInital++;
                        rowTbl++;
                    }
                    sl.AutoFitColumn(1, contador);

                }

                sl.DeleteWorksheet(SLDocument.DefaultFirstSheetName);
                sl.SelectWorksheet(ds.Tables[0].TableName);

                if (!Directory.Exists(pathReport))
                    Directory.CreateDirectory(pathReport);
                sl.SaveAs(CombinePath(pathReport, fileName));
            }
            catch (Exception ex)
            {
                throw new Exception("ExportToExcel: \n" + ex.Message);
            }
        }

        //OBTIENE CONFIGURACION DE CAMPOS
        public DataTable GetDataFields(ModuleReportsFilter parameters)
        {
            DataTable dataFields = new DataTable();

            try
            {
                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleINMOB_OI"].ToString()))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = cn;

                        IDataReader reader = null;

                        cmd.CommandText = string.Format("{0}.{1}", Package, "USP_OI_GET_CONFIG_REPORTS");

                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("P_NNUMORI", OracleDbType.Int32).Value = parameters.P_NNUMORI;
                        cmd.Parameters.Add("P_NCODGRU", OracleDbType.Int32).Value = parameters.P_NCODGRU;
                        cmd.Parameters.Add("P_NBRANCH", OracleDbType.Int32).Value = parameters.P_NBRANCH;
                        cmd.Parameters.Add("P_NREPORT", OracleDbType.Int32).Value = parameters.P_NREPORT;   //  1: CONTABLE /2: OPERATIVO /3:CONTROL BANCARIO /4: PRELIMINAR ERROR / 5: TRANSFERENCIAS / 6: CHEQUES / 7: APROBACION PAGOS CAB / 8: APROBACION PAGOS DET
                        cmd.Parameters.Add("P_NTIPO", OracleDbType.Varchar2).Value = parameters.P_NTIPO;    //  1: RESUMEN  /2:DETALLE
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
                LogControl.save("GetDataFields", ex.ToString(), "3");
                throw;
            }
            return dataFields;
        }


        // ASIGNA FORMATO AL TITULO DEL ARCHIVO EXCEL
        private void SetValueWithStyle(ref SLDocument sl, int Row, int Column, string Value, SLStyle style)
        {
            sl.SetCellValue(Row, Column, Value);
            sl.SetCellStyle(Row, Column, style);
        }
        private void CreateStyle(ref SLStyle style)
        {
            style.SetVerticalAlignment(DocumentFormat.OpenXml.Spreadsheet.VerticalAlignmentValues.Center);
            style.SetHorizontalAlignment(DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Left);
            style.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, System.Drawing.Color.FromArgb(79, 129, 189), System.Drawing.Color.FromArgb(79, 129, 189));
            style.Font.FontSize = 12;
            style.Font.FontName = "Calibri";
            style.Font.FontColor = System.Drawing.Color.White;
            style.Font.Bold = true;
        }

        // FORMATEO DE LA RUTA Y NOMBRE DEL ARCHIVO
        private String CombinePath(string Path, string File)
        {
            return String.Format(@"{0}\{1}", Path, File);
        }
        public List<ConfigFields> GetFieldsConfiguration(DataTable Config, ModuleReportsFilter parameters)
        {
            List<ConfigFields> FieldConfiglist = new List<ConfigFields>();
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

        public DataSet ProcessReportCuentasPorCobrar(ModuleReportsFilter data)
        {

            DataSet dataReport = new DataSet();
            DataTable dataCab = new DataTable();


            int Pcode;
            string Pmessage;

            int prod;
            if (data.P_NPRODUCT == "" || data.P_NPRODUCT == null)
            {
                prod = 0;
            }
            else
            {
                prod = Int32.Parse(data.P_NPRODUCT);
            }

            try
            {
                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleINMOB_OI"].ToString()))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = cn;
                        cmd.CommandText = string.Format("{0}.{1}", ProcedureName.pkg_CtaXCobrar, "USP_OI_REPORT_CUENTAXCOBRAR");
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new OracleParameter("P_NNUMORI", OracleDbType.Int16, data.P_NNUMORI, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NCODGRU", OracleDbType.Int16, data.P_NCODGRU, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int16, data.P_NNUMORI, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_FECHAINI", OracleDbType.Date, data.P_FECHAINI, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_FECHAFIN", OracleDbType.Date, data.P_FECHAFIN, ParameterDirection.Input));

                        OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                        OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                        cmd.Parameters.Add(P_NCODE);
                        cmd.Parameters.Add(P_SMESSAGE);
                        cmd.Parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
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
                        dataCab.TableName = "CUENTAS POR COBRAR";
                        dataReport.Tables.Add(dataCab);
                        var cantCab = dataCab.Rows.Count;


                        //OBTIENE CONFIGURACION DE REPORTE RENTAS
                        data.P_NREPORT = 5; //cuentas por cobrar
                        data.P_NTIPO = 1;   //CABECERA
                        DataTable configCBCO = this.GetDataFields(data);

                        List<ConfigFields> configurationCtaXCobrar = this.GetFieldsConfiguration(configCBCO, data);

                        pathReport = ELog.obtainConfig("reportOnlineInterfaceOperativo");
                        fileName = "Reporte_CuentasXCobrar.xlsx";

                        ExportToExcelCD(dataReport, configurationCtaXCobrar, configurationCtaXCobrar, pathReport, fileName);
                        cn.Close();

                    }
                }
            }
            catch (Exception ex)
            {
                LogControl.save("ProcessReportOperativo", ex.ToString(), "3");
                throw;
            }
            return dataReport;
        }

    }
}