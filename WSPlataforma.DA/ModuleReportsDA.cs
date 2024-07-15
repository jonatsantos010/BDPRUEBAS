using System;
using System.IO;
using System.Web;
//using System.Web.Http;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Configuration;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using WSPlataforma.Util;
using WSPlataforma.Entities.ModuleReportsModel.ViewModel;
using WSPlataforma.Entities.ModuleReportsModel.BindingModel;
using WSPlataforma.Entities.ReportModel.BindingModel;
using SpreadsheetLight;
using DocumentFormat.OpenXml.Spreadsheet;

namespace WSPlataforma.DA
{
    public class ModuleReportsDA : ConnectionBase
    {
        private readonly string Package = "PKG_OI_REPORTS";
        private string pathReport = string.Empty;
        private string fileName = string.Empty;
        #region antiguo
        //// INI REPORTE CONTABLE
        //public List<ListModuleReportsContableResVM> GetReporteContableRes(ModuleReportsFilter data)
        //{
        //    try
        //    {
        //        using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleOnlineDB"].ToString()))
        //        {
        //            using (OracleCommand cmd = new OracleCommand())
        //            {
        //                cmd.Connection = cn;
        //                cmd.CommandText = string.Format("{0}.{1}", Package, "USP_OI_REPORT_CONTABLE");
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                var reports = new List<ListModuleReportsContableResVM>();

        //                cmd.Parameters.Add(new OracleParameter("P_NNUMORI", OracleDbType.Int16, data.P_NNUMORI, ParameterDirection.Input));
        //                cmd.Parameters.Add(new OracleParameter("P_NCODGRU", OracleDbType.Int16, data.P_NCODGRU, ParameterDirection.Input));
        //                cmd.Parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int16, data.P_NBRANCH, ParameterDirection.Input));
        //                cmd.Parameters.Add(new OracleParameter("P_FECHAINI", OracleDbType.Date, data.P_FECHAINI, ParameterDirection.Input));
        //                cmd.Parameters.Add(new OracleParameter("P_FECHAFIN", OracleDbType.Date, data.P_FECHAFIN, ParameterDirection.Input));

        //                cmd.Parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
        //                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
        //                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
        //                cmd.Parameters.Add(P_NCODE);
        //                cmd.Parameters.Add(P_SMESSAGE);
        //                cn.Open();
        //                var reader = (OracleDataReader)cmd.ExecuteReader();

        //                if (reader.HasRows)
        //                {
        //                    while (reader.Read())
        //                    {
        //                        var report = new ListModuleReportsContableResVM();
        //                        report.NTYPE = reader["NCODCON"] == DBNull.Value ? string.Empty : reader["NCODCON"].ToString();
        //                        report.CCENCOS = reader["CCENCOS"] == DBNull.Value ? string.Empty : reader["CCENCOS"].ToString();
        //                        report.CCODCTA = reader["CCODCTA"] == DBNull.Value ? string.Empty : reader["CCODCTA"].ToString();
        //                        report.DEBE_SOLES = reader["DEBE_SOLES"] == DBNull.Value ? 0 : decimal.Parse(reader["DEBE_SOLES"].ToString());
        //                        report.HABER_SOLES = reader["HABER_SOLES"] == DBNull.Value ? 0 : decimal.Parse(reader["HABER_SOLES"].ToString());
        //                        report.DEBE_DOLAR = reader["DEBE_DOLAR"] == DBNull.Value ? 0 : decimal.Parse(reader["DEBE_DOLAR"].ToString());
        //                        report.HABER_DOLAR = reader["HABER_DOLAR"] == DBNull.Value ? 0 : decimal.Parse(reader["HABER_DOLAR"].ToString());
        //                        reports.Add(report);
        //                    }
        //                }
        //                reader.Close();
        //                return reports;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ELog.save(this, ex);
        //        throw;
        //    }
        //}

        //public List<ListModuleReportsContableDetVM> GetReporteContableDet(ModuleReportsFilter data)
        //{
        //    try
        //    {
        //        using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleOnlineDB"].ToString()))
        //        {
        //            using (OracleCommand cmd = new OracleCommand())
        //            {
        //                cmd.Connection = cn;
        //                cmd.CommandText = string.Format("{0}.{1}", Package, "USP_OI_REPORT_CONTABLE_DET");
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                var reports = new List<ListModuleReportsContableDetVM>();

        //                cmd.Parameters.Add(new OracleParameter("P_NNUMORI", OracleDbType.Int16, data.P_NNUMORI, ParameterDirection.Input));
        //                cmd.Parameters.Add(new OracleParameter("P_NCODGRU", OracleDbType.Int16, data.P_NCODGRU, ParameterDirection.Input));
        //                cmd.Parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int16, data.P_NBRANCH, ParameterDirection.Input));
        //                cmd.Parameters.Add(new OracleParameter("P_FECHAINI", OracleDbType.Date, data.P_FECHAINI, ParameterDirection.Input));
        //                cmd.Parameters.Add(new OracleParameter("P_FECHAFIN", OracleDbType.Date, data.P_FECHAFIN, ParameterDirection.Input));

        //                cmd.Parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
        //                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
        //                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
        //                cmd.Parameters.Add(P_NCODE);
        //                cmd.Parameters.Add(P_SMESSAGE);
        //                cn.Open();
        //                var reader = (OracleDataReader)cmd.ExecuteReader();

        //                if (reader.HasRows)
        //                {
        //                    while (reader.Read())
        //                    {
        //                        var report = new ListModuleReportsContableDetVM();
        //                        report.NIDPROCESS = reader["NIDPROCESS"] == DBNull.Value ? 0 : int.Parse(reader["NIDPROCESS"].ToString());
        //                        report.DCOMPDATE = reader["DCOMPDATE"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["DCOMPDATE"].ToString());
        //                        report.DFECCON = reader["DFECCON"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["DFECCON"].ToString());
        //                        report.CASIEXA = reader["CASIEXA"] == DBNull.Value ? string.Empty : reader["CASIEXA"].ToString();
        //                        report.NNUMASI = reader["NNUMASI"] == DBNull.Value ? 0 : int.Parse(reader["NNUMASI"].ToString());

        //                        report.CNUMPAQ = reader["CNUMPAQ"] == DBNull.Value ? string.Empty : reader["CNUMPAQ"].ToString();
        //                        report.NDETASO = reader["NDETASO"] == DBNull.Value ? 0 : int.Parse(reader["NDETASO"].ToString());
        //                        report.NCODCON = reader["NCODCON"] == DBNull.Value ? 0 : int.Parse(reader["NCODCON"].ToString());
        //                        report.CCENCOS = reader["CCENCOS"] == DBNull.Value ? string.Empty : reader["CCENCOS"].ToString();
        //                        report.CCODCTA = reader["CCODCTA"] == DBNull.Value ? string.Empty : reader["CCODCTA"].ToString();

        //                        report.CREFERE = reader["CREFERE"] == DBNull.Value ? string.Empty : reader["CREFERE"].ToString();
        //                        report.DEBE_SOLES = reader["DEBE_SOLES"] == DBNull.Value ? 0: decimal.Parse(reader["DEBE_SOLES"].ToString());                                
        //                        report.HABER_SOLES = reader["HABER_SOLES"] == DBNull.Value ? 0: decimal.Parse(reader["HABER_SOLES"].ToString());
        //                        report.DEBE_DOLAR = reader["DEBE_DOLAR"] == DBNull.Value ? 0: decimal.Parse (reader["DEBE_DOLAR"].ToString());
        //                        report.HABER_DOLAR = reader["HABER_DOLAR"] == DBNull.Value ? 0: decimal.Parse(reader["HABER_DOLAR"].ToString());
        //                        reports.Add(report);
        //                    }
        //                }
        //                reader.Close();
        //                return reports;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ELog.save(this, ex);
        //        throw;
        //    }
        //}

        //public ResponseControl ProcessReportContable(ModuleReportsFilter data)
        //{
        //    ResponseControl generic = new ResponseControl(Response.Ok);
        //    try
        //    {
        //        var reports = this.GetReporteContableDet(data);
        //        var xlsbase64 = string.Empty;

        //        if (reports.Count > 0)
        //        {
        //            using (var xls = new SLDocument())
        //            {
        //                #region CONTABLE DETALLE
        //                xls.AddWorksheet("DETALLE");

        //                SLStyle rowHeaderStyle = new SLStyle();
        //                rowHeaderStyle.SetVerticalAlignment(DocumentFormat.OpenXml.Spreadsheet.VerticalAlignmentValues.Center);
        //                rowHeaderStyle.SetHorizontalAlignment(DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Left);
        //                rowHeaderStyle.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, System.Drawing.Color.FromArgb(79, 129, 189), System.Drawing.Color.FromArgb(79, 129, 189));
        //                rowHeaderStyle.Font.FontSize = 12;
        //                rowHeaderStyle.Font.FontName = "Calibri";
        //                rowHeaderStyle.Font.FontColor = System.Drawing.Color.White;
        //                rowHeaderStyle.Font.Bold = true;

        //                xls.SetRowStyle(1, rowHeaderStyle);
        //                xls.SetCellValue(1, 1, "CÓDIGO PROCESO");
        //                xls.SetCellValue(1, 2, "FECHA PROCESO");
        //                xls.SetCellValue(1, 3, "FECHA CONTABLE");
        //                xls.SetCellValue(1, 4, "ASIENTO EXACTUS");
        //                xls.SetCellValue(1, 5, "NÚMERO ASIENTO");

        //                xls.SetCellValue(1, 6, "PAQUETE");
        //                xls.SetCellValue(1, 7, "COR.");
        //                xls.SetCellValue(1, 8, "MOVIMIENTO");
        //                xls.SetCellValue(1, 9, "CENTRO DE COSTO");
        //                xls.SetCellValue(1, 10, "CUENTA");

        //                xls.SetCellValue(1, 11, "GLOSA");
        //                xls.SetCellValue(1, 12, "DEBE SOLES");
        //                xls.SetCellValue(1, 13, "HABER SOLES");
        //                xls.SetCellValue(1, 14, "DEBE DÓLAR");
        //                xls.SetCellValue(1, 15, "HABER DÓLAR");

        //                var rowIdx = 2;
        //                var colIdx = 1;
        //                foreach (var report in reports)
        //                {
        //                    string sDPROCESS; string sDCONTABLE;

        //                    try { sDPROCESS = report.DCOMPDATE == null ? "" : Convert.ToDateTime(report.DCOMPDATE).ToString("dd/MM/yyyy"); }
        //                    catch (Exception) { sDPROCESS = ""; }

        //                    try { sDCONTABLE = report.DFECCON == null ? "" : Convert.ToDateTime(report.DFECCON).ToString("dd/MM/yyyy"); }
        //                    catch (Exception) { sDCONTABLE = ""; }

        //                    xls.SetCellValue(rowIdx, colIdx++, report.NIDPROCESS);
        //                    xls.SetCellValue(rowIdx, colIdx++, sDPROCESS);
        //                    xls.SetCellValue(rowIdx, colIdx++, sDCONTABLE);
        //                    xls.SetCellValue(rowIdx, colIdx++, report.CASIEXA);
        //                    xls.SetCellValue(rowIdx, colIdx++, report.NNUMASI);

        //                    xls.SetCellValue(rowIdx, colIdx++, report.CNUMPAQ);
        //                    xls.SetCellValue(rowIdx, colIdx++, report.NDETASO);
        //                    xls.SetCellValue(rowIdx, colIdx++, report.NCODCON);
        //                    xls.SetCellValue(rowIdx, colIdx++, report.CCENCOS);
        //                    xls.SetCellValue(rowIdx, colIdx++, report.CCODCTA);

        //                    xls.SetCellValue(rowIdx, colIdx++, report.CREFERE);
        //                    xls.SetCellValue(rowIdx, colIdx++, report.DEBE_SOLES);
        //                    xls.SetCellValue(rowIdx, colIdx++, report.HABER_SOLES);
        //                    xls.SetCellValue(rowIdx, colIdx++, report.DEBE_DOLAR);
        //                    xls.SetCellValue(rowIdx, colIdx++, report.HABER_DOLAR);

        //                    rowIdx++;
        //                    colIdx = 1;
        //                }
        //                #endregion

        //                #region CONTABLE RESUMEN
        //                xls.AddWorksheet("RESUMEN");

        //                var reportsRes = this.GetReporteContableRes(data);

        //                SLStyle rowHeaderStyleRes = new SLStyle();
        //                rowHeaderStyleRes.SetVerticalAlignment(DocumentFormat.OpenXml.Spreadsheet.VerticalAlignmentValues.Center);
        //                rowHeaderStyleRes.SetHorizontalAlignment(DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Left);
        //                rowHeaderStyleRes.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, System.Drawing.Color.FromArgb(79, 129, 189), System.Drawing.Color.FromArgb(79, 129, 189));
        //                rowHeaderStyleRes.Font.FontSize = 12;
        //                rowHeaderStyleRes.Font.FontName = "Calibri";
        //                rowHeaderStyleRes.Font.FontColor = System.Drawing.Color.White;
        //                rowHeaderStyleRes.Font.Bold = true;

        //                xls.SetRowStyle(1, rowHeaderStyleRes);
        //                xls.SetCellValue(1, 1, "MOVIMIENTO");
        //                xls.SetCellValue(1, 2, "CENTRO DE COSTO");
        //                xls.SetCellValue(1, 3, "CUENTA");
        //                xls.SetCellValue(1, 4, "DEBE SOLES");
        //                xls.SetCellValue(1, 5, "HABER SOLES");
        //                xls.SetCellValue(1, 6, "DEBE DÓLAR");
        //                xls.SetCellValue(1, 7, "HABER DÓLAR");

        //                var rowIx = 2;
        //                var colIx = 1;

        //                foreach (var reportRes in reportsRes)
        //                {
        //                    xls.SetCellValue(rowIx, colIx++, reportRes.NTYPE);
        //                    xls.SetCellValue(rowIx, colIx++, reportRes.CCENCOS);
        //                    xls.SetCellValue(rowIx, colIx++, reportRes.CCODCTA);
        //                    xls.SetCellValue(rowIx, colIx++, reportRes.DEBE_SOLES);
        //                    xls.SetCellValue(rowIx, colIx++, reportRes.HABER_SOLES);
        //                    xls.SetCellValue(rowIx, colIx++, reportRes.DEBE_DOLAR);
        //                    xls.SetCellValue(rowIx, colIx++, reportRes.HABER_DOLAR);

        //                    rowIx++;
        //                    colIx = 1;
        //                }

        //                #endregion

        //                xls.DeleteWorksheet("Sheet1");
        //                xls.SelectWorksheet("DETALLE");

        //                generic.Data = "OK";
        //                pathReport = ELog.obtainConfig("reportOnlineInterfaceContable");
        //                if (!Directory.Exists(pathReport))
        //                    Directory.CreateDirectory(pathReport);

        //                xls.SaveAs(CombinePath(pathReport, $"RpteContable.xlsx"));
        //            }
        //        }
        //        else
        //        {
        //            generic.Data = "Fail";
        //        }
        //        return generic;
        //    }
        //    catch (Exception ex)
        //    {
        //        ELog.save(this, ex);
        //        throw new Exception(generic.Message.ToString());
        //    }
        //}
        //// FIN REPORTE CONTABLE

        //// INI REPORTE OPERACIONES
        //public List<ListModuleReportsOperacionesResVM> GetReporteOperacionesRes(ModuleReportsFilter data)
        //{
        //    try
        //    {
        //        using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleOnlineDB"].ToString()))
        //        {
        //            using (OracleCommand cmd = new OracleCommand())
        //            {
        //                cmd.Connection = cn;
        //                cmd.CommandText = string.Format("{0}.{1}", Package, "USP_OI_REPORT_OPERATIVE_RES");
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                var reports = new List<ListModuleReportsOperacionesResVM>();

        //                cmd.Parameters.Add(new OracleParameter("P_NNUMORI", OracleDbType.Int16, data.P_NNUMORI, ParameterDirection.Input));
        //                cmd.Parameters.Add(new OracleParameter("P_NCODGRU", OracleDbType.Int16, data.P_NCODGRU, ParameterDirection.Input));
        //                cmd.Parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int16, data.P_NBRANCH, ParameterDirection.Input));
        //                cmd.Parameters.Add(new OracleParameter("P_FECHAINI", OracleDbType.Date, data.P_FECHAINI, ParameterDirection.Input));
        //                cmd.Parameters.Add(new OracleParameter("P_FECHAFIN", OracleDbType.Date, data.P_FECHAFIN, ParameterDirection.Input));

        //                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
        //                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
        //                cmd.Parameters.Add(P_NCODE);
        //                cmd.Parameters.Add(P_SMESSAGE);
        //                cmd.Parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
        //                cn.Open();
        //                var reader = (OracleDataReader)cmd.ExecuteReader();

        //                if (reader.HasRows)
        //                {
        //                    while (reader.Read())
        //                    {
        //                        var report = new ListModuleReportsOperacionesResVM();
        //                        report.INTERFAZ = reader["INTERFAZ"] == DBNull.Value ? string.Empty : reader["INTERFAZ"].ToString();
        //                        report.TIPO_MOVIMIENTO = reader["TIPO_MOVIMIENTO"] == DBNull.Value ? string.Empty : reader["TIPO_MOVIMIENTO"].ToString();
        //                        report.MONEDA = reader["MONEDA"] == DBNull.Value ? string.Empty : reader["MONEDA"].ToString();
        //                        if (data.P_NCODGRU == 12 || data.P_NCODGRU == 13)
        //                        {
        //                            report.NPREMIUMN = reader["NPREMIUMN"] == DBNull.Value ? 0 : decimal.Parse(reader["NPREMIUMN"].ToString());
        //                            report.NIVA = reader["NIVA"] == DBNull.Value ? 0 : decimal.Parse(reader["NIVA"].ToString());
        //                        }
        //                        report.MONTO = reader["MONTO"] == DBNull.Value ? 0 : decimal.Parse(reader["MONTO"].ToString());
        //                        report.REGISTROS = reader["REGISTROS"] == DBNull.Value ? 0 : int.Parse(reader["REGISTROS"].ToString());
        //                        reports.Add(report);
        //                    }
        //                }
        //                reader.Close();
        //                return reports;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ELog.save(this, ex);
        //        throw;
        //    }
        //}

        //public List<ListModuleReportsOperacionesDetVM> GetReporteOperacionesDet(ModuleReportsFilter data)
        //{
        //    try
        //    {
        //        using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleOnlineDB"].ToString()))
        //        {
        //            using (OracleCommand cmd = new OracleCommand())
        //            {
        //                cmd.Connection = cn;
        //                cmd.CommandText = string.Format("{0}.{1}", Package, "USP_OI_REPORT_OPERATIVE");
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                var reports = new List<ListModuleReportsOperacionesDetVM>();

        //                cmd.Parameters.Add(new OracleParameter("P_NNUMORI", OracleDbType.Int16, data.P_NNUMORI, ParameterDirection.Input));
        //                cmd.Parameters.Add(new OracleParameter("P_NCODGRU", OracleDbType.Int16, data.P_NCODGRU, ParameterDirection.Input));
        //                cmd.Parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int16, data.P_NBRANCH, ParameterDirection.Input));
        //                cmd.Parameters.Add(new OracleParameter("P_FECHAINI", OracleDbType.Date, data.P_FECHAINI, ParameterDirection.Input));
        //                cmd.Parameters.Add(new OracleParameter("P_FECHAFIN", OracleDbType.Date, data.P_FECHAFIN, ParameterDirection.Input));

        //                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
        //                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
        //                cmd.Parameters.Add(P_NCODE);
        //                cmd.Parameters.Add(P_SMESSAGE);
        //                cmd.Parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
        //                cn.Open();
        //                var reader = (OracleDataReader)cmd.ExecuteReader();

        //                if (reader.HasRows)
        //                {
        //                    while (reader.Read())
        //                    {
        //                        var report = new ListModuleReportsOperacionesDetVM();
        //                        report.NMOVCONT = reader["NMOVCONT"] == DBNull.Value ? string.Empty : reader["NMOVCONT"].ToString();
        //                        report.POLIZA = reader["POLIZA"] == DBNull.Value ? string.Empty : reader["POLIZA"].ToString();
        //                        report.FECHA = reader["FECHA"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["FECHA"].ToString());
        //                        report.RAMO = reader["RAMO"] == DBNull.Value ? string.Empty : reader["RAMO"].ToString();
        //                        report.COBERTURA = reader["COBERTURA"] == DBNull.Value ? string.Empty : reader["COBERTURA"].ToString();
                                
        //                        report.TIP_DOC = reader["TIP_DOC"] == DBNull.Value ? string.Empty : reader["TIP_DOC"].ToString();
        //                        report.DOCUMENTO = reader["DOCUMENTO"] == DBNull.Value ? string.Empty : reader["DOCUMENTO"].ToString();
        //                        report.NOMBRE_COMPLETO = reader["NOMBRE_COMPLETO"] == DBNull.Value ? string.Empty : reader["NOMBRE_COMPLETO"].ToString();
        //                        report.TIPO = reader["TIPO"] == DBNull.Value ? string.Empty : reader["TIPO"].ToString();
        //                        report.NTIPCOM = reader["NTIPCOM"] == DBNull.Value ? string.Empty : reader["NTIPCOM"].ToString();
                                
        //                        report.COMPROBANTE_MONEDA = reader["COMPROBANTE_MONEDA"] == DBNull.Value ? string.Empty : reader["COMPROBANTE_MONEDA"].ToString();
        //                        report.MONEDA = reader["MONEDA"] == DBNull.Value ? string.Empty : reader["MONEDA"].ToString();
        //                        if (data.P_NCODGRU == 12 || data.P_NCODGRU == 13)
        //                        {
        //                            report.NPREMIUMN = reader["NPREMIUMN"] == DBNull.Value ? 0 : decimal.Parse(reader["NPREMIUMN"].ToString());
        //                            report.NIVA = reader["NIVA"] == DBNull.Value ? 0 : decimal.Parse(reader["NIVA"].ToString());
        //                        }
        //                        report.MONTO = reader["MONTO"] == DBNull.Value ? 0 : decimal.Parse(reader["MONTO"].ToString());
        //                        reports.Add(report);
        //                    }
        //                }
        //                reader.Close();
        //                return reports;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ELog.save(this, ex);
        //        throw;
        //    }
        //}

        //public ResponseControl ProcessReportOperaciones(ModuleReportsFilter data)
        //{
        //    ResponseControl generic = new ResponseControl(Response.Ok);
        //    try
        //    {
        //        var reports = this.GetReporteOperacionesDet(data);
        //        var xlsbase64 = string.Empty;

        //        if (reports.Count > 0)
        //        {
        //            using (var xls = new SLDocument())
        //            {
        //                #region OPERATIVO DETALLE
        //                xls.AddWorksheet("DETALLE");

        //                SLStyle rowHeaderStyle = new SLStyle();
        //                rowHeaderStyle.SetVerticalAlignment(DocumentFormat.OpenXml.Spreadsheet.VerticalAlignmentValues.Center);
        //                rowHeaderStyle.SetHorizontalAlignment(DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Left);
        //                rowHeaderStyle.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, System.Drawing.Color.FromArgb(79, 129, 189), System.Drawing.Color.FromArgb(79, 129, 189));
        //                rowHeaderStyle.Font.FontSize = 12;
        //                rowHeaderStyle.Font.FontName = "Calibri";
        //                rowHeaderStyle.Font.FontColor = System.Drawing.Color.White;
        //                rowHeaderStyle.Font.Bold = true;

        //                xls.SetRowStyle(1, rowHeaderStyle);
        //                xls.SetCellValue(1, 1, "COD. MOVIMIENTO");
        //                xls.SetCellValue(1, 2, "PÓLIZA");
        //                xls.SetCellValue(1, 3, "FECHA CONTABLE");
        //                xls.SetCellValue(1, 4, "RAMO");
        //                xls.SetCellValue(1, 5, "COBERTURA");

        //                xls.SetCellValue(1, 6, "TIPO DOC.");
        //                xls.SetCellValue(1, 7, "DOCUMENTO");
        //                xls.SetCellValue(1, 8, "NOMBRE COMPLETO");
        //                xls.SetCellValue(1, 9, "TIPO");
        //                xls.SetCellValue(1, 10, "TIPO COMPROBANTE");

        //                xls.SetCellValue(1, 11, "COMPROBANTE");
        //                xls.SetCellValue(1, 12, "MONEDA");
        //                if (data.P_NCODGRU == 12 || data.P_NCODGRU == 13)
        //                {
        //                    xls.SetCellValue(1, 13, "PRIMA NETA");
        //                    xls.SetCellValue(1, 14, "IGV");
        //                    xls.SetCellValue(1, 15, "PRIMA TOTAL");
        //                }
        //                else
        //                {
        //                    xls.SetCellValue(1, 13, "TOTAL");
        //                }

        //                var rowIdx = 2;
        //                var colIdx = 1;
        //                foreach (var report in reports)
        //                {
        //                    string sFECHA;

        //                    try { sFECHA = report.FECHA == null ? "" : Convert.ToDateTime(report.FECHA).ToString("dd/MM/yyyy"); }
        //                    catch (Exception) { sFECHA = ""; }

        //                    xls.SetCellValue(rowIdx, colIdx++, report.NMOVCONT);
        //                    xls.SetCellValue(rowIdx, colIdx++, report.POLIZA);
        //                    xls.SetCellValue(rowIdx, colIdx++, sFECHA);
        //                    xls.SetCellValue(rowIdx, colIdx++, report.RAMO);
        //                    xls.SetCellValue(rowIdx, colIdx++, report.COBERTURA);

        //                    xls.SetCellValue(rowIdx, colIdx++, report.TIP_DOC);
        //                    xls.SetCellValue(rowIdx, colIdx++, report.DOCUMENTO);
        //                    xls.SetCellValue(rowIdx, colIdx++, report.NOMBRE_COMPLETO);
        //                    xls.SetCellValue(rowIdx, colIdx++, report.TIPO);
        //                    xls.SetCellValue(rowIdx, colIdx++, report.NTIPCOM);

        //                    xls.SetCellValue(rowIdx, colIdx++, report.COMPROBANTE_MONEDA);
        //                    xls.SetCellValue(rowIdx, colIdx++, report.MONEDA);
        //                    if (data.P_NCODGRU == 12 || data.P_NCODGRU == 13)
        //                    {
        //                        xls.SetCellValue(rowIdx, colIdx++, report.NPREMIUMN);
        //                        xls.SetCellValue(rowIdx, colIdx++, report.NIVA);
        //                        xls.SetCellValue(rowIdx, colIdx++, report.MONTO);
        //                    }
        //                    else
        //                    {
        //                        xls.SetCellValue(rowIdx, colIdx++, report.MONTO);
        //                    }
        //                    rowIdx++;
        //                    colIdx = 1;
        //                }
        //                #endregion

        //                #region OPERATIVO RESUMEN
        //                xls.AddWorksheet("RESUMEN");
        //                var reportsRes = this.GetReporteOperacionesRes(data);

        //                SLStyle rowHeaderStyleRes = new SLStyle();
        //                rowHeaderStyleRes.SetVerticalAlignment(DocumentFormat.OpenXml.Spreadsheet.VerticalAlignmentValues.Center);
        //                rowHeaderStyleRes.SetHorizontalAlignment(DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Left);
        //                rowHeaderStyleRes.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, System.Drawing.Color.FromArgb(79, 129, 189), System.Drawing.Color.FromArgb(79, 129, 189));
        //                rowHeaderStyleRes.Font.FontSize = 12;
        //                rowHeaderStyleRes.Font.FontName = "Calibri";
        //                rowHeaderStyleRes.Font.FontColor = System.Drawing.Color.White;
        //                rowHeaderStyleRes.Font.Bold = true;

        //                xls.SetRowStyle(1, rowHeaderStyleRes);
        //                xls.SetCellValue(1, 1, "INTERFAZ");
        //                xls.SetCellValue(1, 2, "TIPO MOVIMIENTO");
        //                xls.SetCellValue(1, 3, "MONEDA");
        //                if (data.P_NCODGRU == 12 || data.P_NCODGRU == 13)
        //                {
        //                    xls.SetCellValue(1, 4, "PRIMA NETA");
        //                    xls.SetCellValue(1, 5, "IGV");
        //                    xls.SetCellValue(1, 6, "PRIMA TOTAL");
        //                    xls.SetCellValue(1, 7, "REGISTROS");
        //                }
        //                else
        //                {
        //                    xls.SetCellValue(1, 4, "TOTAL");
        //                    xls.SetCellValue(1, 5, "REGISTROS");

        //                }
                        

        //                var rowIx = 2;
        //                var colIx = 1;

        //                foreach (var reportRes in reportsRes)
        //                {
        //                    xls.SetCellValue(rowIx, colIx++, reportRes.INTERFAZ);
        //                    xls.SetCellValue(rowIx, colIx++, reportRes.TIPO_MOVIMIENTO);
        //                    xls.SetCellValue(rowIx, colIx++, reportRes.MONEDA);
        //                    if (data.P_NCODGRU == 12 || data.P_NCODGRU == 13)
        //                    {
        //                        xls.SetCellValue(rowIx, colIx++, reportRes.NPREMIUMN);
        //                        xls.SetCellValue(rowIx, colIx++, reportRes.NIVA);
        //                        xls.SetCellValue(rowIx, colIx++, reportRes.MONTO);
        //                        xls.SetCellValue(rowIx, colIx++, reportRes.REGISTROS);
        //                    }
        //                    else
        //                    {
        //                        xls.SetCellValue(rowIx, colIx++, reportRes.MONTO);
        //                        xls.SetCellValue(rowIx, colIx++, reportRes.REGISTROS);
        //                    }
                            
        //                    rowIx++;
        //                    colIx = 1;
        //                }

        //                #endregion

        //                xls.DeleteWorksheet("Sheet1");
        //                xls.SelectWorksheet("DETALLE");

        //                generic.Data = "OK";
        //                pathReport = ELog.obtainConfig("reportOnlineInterfaceOperativo");
        //                if (!Directory.Exists(pathReport))
        //                    Directory.CreateDirectory(pathReport);
        //                xls.SaveAs(CombinePath(pathReport, $"RpteOperativo.xlsx"));
        //            }
        //        }
        //        else
        //        {
        //            generic.Data = "Fail";
        //        }
        //        return generic;
        //    }
        //    catch (Exception ex)
        //    {
        //        ELog.save(this, ex);
        //        throw new Exception(generic.Message.ToString());
        //    }
        //}
        //// FIN REPORTE OPERACIONES
        #endregion antiguo

        // REPORTE CONTROL BANCARIO
        public DataSet ProcessReportControlBancario(ModuleReportsFilter data)
        {
            
            DataSet dataReport = new DataSet();
            DataTable dataCab = new DataTable();
            
            int Pcode;
            string Pmessage;
            
            try
            {
                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleOnlineDB"].ToString()))
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

            if(data.P_NNUMORI!=1)
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
                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleOnlineDB"].ToString()))
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
                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleOnlineDB"].ToString()))
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
        public void ExportToExcel( DataTable dt, string pathReport, string fileName)
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

                for (int x = 0; x <= ds.Tables.Count -1; x++)                
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
                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleOnlineDB"].ToString()))
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
                        cmd.Parameters.Add("P_NREPORT", OracleDbType.Int32).Value = parameters.P_NREPORT;   //  1: CONTABLE /2: OPERATIVO /3:CONTROL BANCARIO /4: PRELIMINAR ERROR / 5: TRANSFERENCIAS / 6: CHEQUES / 7: APROBACION PAGOS CAB / 8: APROBACION PAGOS DET/ 9: cuentas por cobrar
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

        // OBTIENE LOS PRODUCTOS DE VILP
        public Task<ListProductsVILPVM> ListarProductosVILP()
        {
            var parameters = new List<OracleParameter>();
            ListProductsVILPVM entities = new ListProductsVILPVM();
            entities.LISTA = new List<ListProductsVILPVM.P_TABLE>();
            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_GET_VILP_PRODUCTS");
            try
            {
                parameters.Add(new OracleParameter("P_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureOnlineDB(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ListProductsVILPVM.P_TABLE suggest = new ListProductsVILPVM.P_TABLE();
                        suggest.NVALOR = reader["NVALOR"] == DBNull.Value ? 0 : int.Parse(reader["NVALOR"].ToString());
                        suggest.SDESCRIPT = reader["SDESCRIPT"] == DBNull.Value ? string.Empty : reader["SDESCRIPT"].ToString();
                        suggest.SDESCRIPT_ABREV = reader["SDESCRIPT_ABREV"] == DBNull.Value ? string.Empty : reader["SDESCRIPT_ABREV"].ToString();
                        entities.LISTA.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Task.FromResult<ListProductsVILPVM>(entities);
        }

        #region REPORTES INTERFAZ ORDENES DE PAGO
        // REPORTE TRANSFERENCIAS BANCARIAS
        public DataSet ProcessReporTransferenciaOP(ModuleReportsFilter data)        
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
                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleOnlineDB"].ToString()))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = cn;
                        cmd.CommandText = string.Format("{0}.{1}", Package, "USP_OI_REPORT_OP_TRANSF");
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

                        //Obtenemos el cursor de resumen
                        OracleRefCursor cursorCab = (OracleRefCursor)cmd.Parameters["C_TABLE"].Value;
                        dataCab.Load(cursorCab.GetDataReader());
                        dataCab.TableName = "TRANSFERENCIA";
                        dataReport.Tables.Add(dataCab);
                        var cantCab = dataCab.Rows.Count;

                        //OBTIENE CONFIGURACION DE REPORTE
                        data.P_NREPORT = 5; //TRANSFERENCIAS
                        data.P_NTIPO = 1;   // RESUMEN
                        DataTable configRES = this.GetDataFields(data);

                        
                        List<ConfigFields> configurationCAB = this.GetFieldsConfiguration(configRES, data);                        

                        pathReport = ELog.obtainConfig("reportOnlineInterfaceOperativo");
                        fileName = "Reporte_Transferencias.xlsx";
                        ExportToExcelCD(dataReport, configurationCAB, configurationCAB, pathReport, fileName);

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


        // REPORTE CHEQUES
        public DataSet ProcessReportChequesOP(ModuleReportsFilter data)
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
                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleOnlineDB"].ToString()))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = cn;
                        cmd.CommandText = string.Format("{0}.{1}", Package, "USP_OI_REPORT_OP_CHEQUES");
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

                        //Obtenemos el cursor de resumen
                        OracleRefCursor cursorCab = (OracleRefCursor)cmd.Parameters["C_TABLE"].Value;
                        dataCab.Load(cursorCab.GetDataReader());
                        dataCab.TableName = "CHEQUES";
                        dataReport.Tables.Add(dataCab);
                        var cantCab = dataCab.Rows.Count;

                        //OBTIENE CONFIGURACION DE REPORTE
                        data.P_NREPORT = 6; // cheques
                        data.P_NTIPO = 1;   // RESUMEN
                        DataTable configRES = this.GetDataFields(data);


                        List<ConfigFields> configurationCAB = this.GetFieldsConfiguration(configRES, data);

                        pathReport = ELog.obtainConfig("reportOnlineInterfaceOperativo");
                        fileName = "Reporte_Cheques.xlsx";
                        ExportToExcelCD(dataReport, configurationCAB, configurationCAB, pathReport, fileName);

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

        #endregion


        #region REPORTES RENTAS VITALICIAS
        // REPORTE cuentas por cobrar rentas vitalicias
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
                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleOnlineDB"].ToString()))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = cn;
                        cmd.CommandText = string.Format("{0}.{1}", ProcedureName.pkg_CtaXCobrar , "USP_OI_REPORT_CUENTAXCOBRAR");
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
                        data.P_NREPORT = 9; //cuentas por cobrar
                        data.P_NTIPO = 1;   //CABECERA
                        DataTable configCBCO = this.GetDataFields(data);

                        List<ConfigFields> configurationCtaXCobrar = this.GetFieldsConfiguration(configCBCO, data);

                        pathReport = ELog.obtainConfig("reportOnlineInterfaceOperativo");
                        fileName = "Reporte_Transferencias.xlsx";

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
        #endregion
    }
}