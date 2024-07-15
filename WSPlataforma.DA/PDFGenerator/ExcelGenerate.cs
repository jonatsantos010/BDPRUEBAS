
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;
using WSPlataforma.Entities.PolicyModel.ViewModel;
using WSPlataforma.Entities.SharedModel.ViewModel;
using System.IO;
using WSPlataforma.Util;
using WSPlataforma.Entities.CobranzasModel;
using WSPlataforma.DA.PrintPolicyPDF;
using WSPlataforma.Entities.PrintPolicyModel.ViewModel;
using SpreadsheetLight;
using DocumentFormat.OpenXml;
using Oracle.DataAccess.Client;
using System.Data;
using DocumentFormat.OpenXml.Spreadsheet;
using WSPlataforma.Entities.QuotationModel.BindingModel;

namespace WSPlataforma.DA
{
    public class ExcelGenerate : ConnectionBase
    {

        PolicyDA policyDA = new PolicyDA();
        SharedDA sharedDA = new SharedDA();
        public string[] Cabecera = new string[] { "Nombres*", "ApPaterno*", "ApMaterno*", "Sexo", "TipoTrabajador*", "PaisNacimiento", "TipoIdent*", "NumIdent*", "FecNacimiento*", "Remuneracion*", "Correo", "Celular", "Sede*", "TipoMovimiento*" };
        public List<DocumentTypeVM> documentos = null;
        public List<SedeVM> Sedes = null;
        public List<NationalityVM> paises = new List<NationalityVM>();
        public List<TypeTrabajadorVM> typeTrabajadors = null;
        private string StrDocumentos;
        private string StrSede;
        private string StrPais;
        private string StrTtrabajador;
        private string StrMov = "I,E";
        private string Sexo = "M,F";
        public Int32 NumFilas = Int32.Parse(ELog.obtainConfig("excelFilas"));

        private void GetListEntities(string Client, string Cotizacion)
        {

            documentos = sharedDA.GetDocumentTypeList("0");
            StrDocumentos = ConvertToString(documentos.Select(obj => obj.Name).ToList());
            Sedes = policyDA.GetListSede(Client);
            StrSede = ConvertToString(Sedes.Select(obj => obj.SDESCRIPTION).ToList());
            paises = sharedDA.GetNationalityList();
            StrPais = ConvertToString(paises.Select(obj => obj.SDESCRIPT).Take(25).ToList());
            typeTrabajadors = policyDA.GetListTypeTrabajador(Convert.ToInt64(Cotizacion));
            StrTtrabajador = ConvertToString(typeTrabajadors.Select(obj => obj.SDESCRIPT).ToList());
        }

        public ResponseAccountStatusVM GetTramaMovimiento(ParamsMovimientoVL param)
        {
            ResponseAccountStatusVM response = new ResponseAccountStatusVM();
            List<PrintPathsVM> ListPath = new PrintPolicyDA().GetPathsPrint(param.documentCode);
            PrintPathsVM path = ListPath[0];
            List<MovimientoTramaVL> listMovimiento = GetDataMovimiento(param);

            string pathNameFileTemplate = path.SRUTA_TEMPLATE + path.SNAME_TEMPLATE + ".xlsx";
            try
            {
                MemoryStream ms = new MemoryStream();
                using (FileStream fs = new FileStream(pathNameFileTemplate, FileMode.Open, FileAccess.Read))
                {
                    fs.CopyTo(ms);
                }

                using (SLDocument sl = new SLDocument(ms))
                {
                    var cab = listMovimiento[0];
                    sl.SetCellValue("C6", cab.Contratante);
                    sl.SetCellValue("C7", cab.Ruc);
                    sl.SetCellValue("C8", cab.Moneda);

                    int indicador = 13;

                    if (param.documentCode == Convert.ToInt32(ELog.obtainConfig("condicionadoTramaAP")))
                    {
                        foreach (MovimientoTramaVL item in listMovimiento)
                        {
                            sl.SetCellValue("B" + indicador, item.Rol);
                            sl.SetCellValue("C" + indicador, item.Rol.ToUpper() == "ASEGURADO" ? item.TipoDocumento : item.TipoDocumentoBeneficiario);
                            sl.SetCellValue("D" + indicador, item.Rol.ToUpper() == "ASEGURADO" ? item.NumeroDocumento : item.NumeroDocumentoBeneficiario);
                            sl.SetCellValue("E" + indicador, item.ApellidoPaterno);
                            sl.SetCellValue("F" + indicador, item.ApellidoMaterno);
                            sl.SetCellValue("G" + indicador, item.Nombre);
                            sl.SetCellValue("H" + indicador, item.Sexo);
                            sl.SetCellValue("I" + indicador, !string.IsNullOrEmpty(item.FechaNac) ? Convert.ToDateTime(item.FechaNac).ToShortDateString() : "");
                            sl.SetCellValue("J" + indicador, item.Sueldo);
                            sl.SetCellValue("K" + indicador, item.Email);
                            sl.SetCellValue("L" + indicador, item.Celular);
                            sl.SetCellValue("M" + indicador, item.Rol.ToUpper() != "ASEGURADO" ? item.TipoDocumento : "");
                            sl.SetCellValue("N" + indicador, item.Rol.ToUpper() != "ASEGURADO" ? item.NumeroDocumento : "");
                            sl.SetCellValue("O" + indicador, item.Porcentaje_participacion);
                            sl.SetCellValue("P" + indicador, item.Relacion);
                            sl.SetCellValue("Q" + indicador, item.Pais);
                            sl.SetCellValue("R" + indicador, item.Grado); //AVS - RENTAS
                            indicador++;
                        }
                    }
                    else
                    {
                        foreach (MovimientoTramaVL item in listMovimiento)
                        {
                            // sLRstType = new SLRstType();
                            //  sl.SetCellValue("",)
                            /*
                        SLStyle style = sl.CreateStyle();
                        style.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                        style.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                        style.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                        style.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                        style.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);
                            style.Alignment.Horizontal = HorizontalAlignmentValues.Center;

                            style.SetVerticalAlignment(VerticalAlignmentValues.Center);
                            */ //comentado en reqinq por k o m --EH

                            sl.SetCellValue("B" + indicador, item.TipoDocumento);
                            sl.SetCellValue("C" + indicador, item.NumeroDocumento);

                            //sl.SetCellStyle("C" + indicador, style); // comentado en reqinc por k o m --EH

                            sl.SetCellValue("D" + indicador, item.ApellidoPaterno);
                            sl.SetCellValue("E" + indicador, item.ApellidoMaterno);
                            sl.SetCellValue("F" + indicador, item.Nombre);
                            sl.SetCellValue("G" + indicador, item.Sexo);
                            if (item.FechaNac != "")
                            {
                                DateTime? date = new DateTime?();
                                date = Convert.ToDateTime(item.FechaNac);
                                sl.SetCellValue("H" + indicador, date.Value);
                            }
                            else
                            {
                                sl.SetCellValue("H" + indicador, "");
                            }
                            sl.SetCellValue("I" + indicador, item.TipoTrabajador);
                            sl.SetCellValue("J" + indicador, item.Sueldo);
                            sl.SetCellValue("K" + indicador, item.Email);
                            sl.SetCellValue("L" + indicador, item.Celular);
                            sl.SetCellValue("M" + indicador, item.Sede);
                            sl.SetCellValue("N" + indicador, item.TipoMovimiento);
                            sl.SetCellValue("O" + indicador, item.Pais);
                            indicador++;
                        }
                    }

                    using (MemoryStream stre = new MemoryStream())
                    {
                        sl.SaveAs(stre);
                        var inputAsString = Convert.ToBase64String(stre.ToArray(), 0, stre.ToArray().Length);
                        response.path = inputAsString;
                    }
                }
            }
            catch (Exception ex)
            {
                response.message = ex.ToString();
                response.path = string.Empty;
                response.indEECC = 1;
            }

            return response;

        }

        /*
        protected void SetValueWithStyle(ref SLDocument sl, string cell, string Value, SLStyle style)
        {
            sl.SetCellValue(cell, Value);
            sl.SetCellStyle(cell, style);
        }
        */ //Quitado en reqinc por k o m -- EH

        private List<MovimientoTramaVL> GetDataMovimiento(ParamsMovimientoVL param)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<MovimientoTramaVL> listEntity = new List<MovimientoTramaVL>();
            string storedProcedureName = ProcedureName.sp_DescargaTrama;
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, param.idCotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMOVEMENT", OracleDbType.Int32, param.idMovimiento, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, param.codProcess, ParameterDirection.Input));

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        MovimientoTramaVL item = new MovimientoTramaVL();
                        item.Contratante = odr["CONTRATANTE"] == DBNull.Value ? string.Empty : odr["CONTRATANTE"].ToString();
                        item.Ruc = odr["RUC"] == DBNull.Value ? string.Empty : odr["RUC"].ToString();
                        item.Moneda = odr["MONEDA"] == DBNull.Value ? string.Empty : odr["MONEDA"].ToString();
                        item.TipoDocumento = odr["TIPO_DE_DOCUMENTO"] == DBNull.Value ? string.Empty : odr["TIPO_DE_DOCUMENTO"].ToString();
                        item.NumeroDocumento = odr["NUM_DOCUMENTO"] == DBNull.Value ? string.Empty : odr["NUM_DOCUMENTO"].ToString();
                        item.ApellidoPaterno = odr["APELLIDO_PATERNO"] == DBNull.Value ? string.Empty : odr["APELLIDO_PATERNO"].ToString();
                        item.ApellidoMaterno = odr["APELLIDO_MATERNO"] == DBNull.Value ? string.Empty : odr["APELLIDO_MATERNO"].ToString();
                        item.Nombre = odr["NOMBRES"] == DBNull.Value ? string.Empty : odr["NOMBRES"].ToString();
                        item.Sexo = odr["SEXO"] == DBNull.Value ? string.Empty : odr["SEXO"].ToString();
                        item.FechaNac = odr["FECHA_NAC"] == DBNull.Value ? string.Empty : odr["FECHA_NAC"].ToString();
                        item.TipoTrabajador = odr["TIPO_TRABAJADOR"] == DBNull.Value ? string.Empty : odr["TIPO_TRABAJADOR"].ToString();
                        item.Sueldo = odr["SUELDO_BRUTO"] == DBNull.Value ? string.Empty : odr["SUELDO_BRUTO"].ToString();
                        item.Sede = odr["SEDE"] == DBNull.Value ? string.Empty : odr["SEDE"].ToString();
                        item.TipoMovimiento = odr["TIPO_DE_MOVIMIENTO"] == DBNull.Value ? string.Empty : odr["TIPO_DE_MOVIMIENTO"].ToString();
                        item.Pais = odr["PAIS_NACIMIENTO"] == DBNull.Value ? string.Empty : odr["PAIS_NACIMIENTO"].ToString();
                        item.Email = odr["EMAIL"] == DBNull.Value ? string.Empty : odr["EMAIL"].ToString();
                        item.Celular = odr["CELULAR"] == DBNull.Value ? string.Empty : odr["CELULAR"].ToString();
                        item.TipoDocumentoBeneficiario = odr["TIPO_DOCUMENTO_BEN"] == DBNull.Value ? string.Empty : odr["TIPO_DOCUMENTO_BEN"].ToString();
                        item.NumeroDocumentoBeneficiario = odr["NUM_DOCUMENTO_BEN"] == DBNull.Value ? string.Empty : odr["NUM_DOCUMENTO_BEN"].ToString();
                        item.TipoPlan = odr["TIPO_PLAN"] == DBNull.Value ? string.Empty : odr["TIPO_PLAN"].ToString();
                        item.Porcentaje_participacion = odr["PORCENTAJE_PAR"] == DBNull.Value ? string.Empty : odr["PORCENTAJE_PAR"].ToString();
                        item.Rol = odr["ROL"] == DBNull.Value ? string.Empty : odr["ROL"].ToString();
                        item.Relacion = odr["RELACION"] == DBNull.Value ? string.Empty : odr["RELACION"].ToString();
                        item.Grado = odr["GRADO"] == DBNull.Value ? string.Empty : odr["GRADO"].ToString(); //AVS - RENTAS
                        listEntity.Add(item);
                    }
                }
                odr.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("GetDataMovimiento", ex.ToString(), "3");
            }

            return listEntity;
        }


        private string ConvertToString<T>(List<T> list)
        {
            return string.Join(",", list);
        }


        public ResponseAccountStatusVM ExecuteExcelDemo(GetTrama data)
        {
            ResponseAccountStatusVM response = new ResponseAccountStatusVM();
            List<PrintPathsVM> ListPath = new PrintPolicyDA().GetPathsPrint(28);
            PrintPathsVM path = ListPath[0];
            GetListEntities(data.sclient, data.cotizacion);

            string pathNameFileTemplate = path.SRUTA_TEMPLATE + "01_trama_Sctr.xlsx";
            try
            {
                MemoryStream ms = new MemoryStream();
                using (FileStream fs = new FileStream(pathNameFileTemplate, FileMode.Open, FileAccess.Read))
                {
                    fs.CopyTo(ms);
                }

                using (SLDocument sl = new SLDocument(ms))
                {
                    sl.SetCellValue("C6", data.contratante);
                    sl.SetCellValue("C7", data.ruc);
                    sl.SetCellValue("C8", data.moneda);
                    int indicador = 13;

                    for (var i = 0; i <= 5; i++)
                    {
                        sl.SetCellValue("B" + indicador, String.Empty);
                        sl.SetCellValue("C" + indicador, String.Empty);
                        sl.SetCellValue("D" + indicador, String.Empty);
                        sl.SetCellValue("E" + indicador, String.Empty);
                        sl.SetCellValue("F" + indicador, String.Empty);
                        sl.SetCellValue("G" + indicador, String.Empty);
                        sl.SetCellValue("H" + indicador, String.Empty);
                        sl.SetCellValue("I" + indicador, String.Empty);
                        sl.SetCellValue("J" + indicador, String.Empty);
                        sl.SetCellValue("K" + indicador, String.Empty);
                        sl.SetCellValue("L" + indicador, String.Empty);
                        sl.SetCellValue("M" + indicador, String.Empty);
                        sl.SetCellValue("N" + indicador, String.Empty);
                        sl.SetCellValue("O" + indicador, String.Empty);
                        indicador++;
                    }

                    using (MemoryStream stre = new MemoryStream())
                    {
                        sl.SaveAs(stre);
                        var inputAsString = Convert.ToBase64String(stre.ToArray(), 0, stre.ToArray().Length);
                        response.path = inputAsString;
                    }
                }
            }
            catch (Exception ex)
            {
                response.message = ex.Message;
                response.path = string.Empty;
                response.indEECC = 1;
            }

            return response;

            //Entities.PolicyModel.ViewModel.GenericPackageVM response;

            //GetListEntities(client, Cotizacion);

            //response = new Entities.PolicyModel.ViewModel.GenericPackageVM { GenericResponse = OpenExcelWorkBook(client, Cotizacion, operacion, movimiento, poliza) };

            //return response;
        }

        private string OpenExcelWorkBook(string client, string cotizacion, int operacion, int movimiento, string poliza)
        {
            string rutaExcel = String.Format(ELog.obtainConfig("rutaExcel"), client);
            //string Ruta = ELog.obtainConfig("rutaExcel") + "ExelEjempl-" + client + ".xlsx";


            Excel.Application app = new Excel.Application();
            try
            {
                app.Visible = false;
                var rutaSave = String.Format(ELog.obtainConfig("pathExcel"), rutaExcel);

                Excel.Workbook xWb = null;
                Excel.Worksheet xlWorkSheet;
                xWb = app.Workbooks.Add(Type.Missing);
                xlWorkSheet = (Excel.Worksheet)xWb.Sheets[1];
                GenerateData(xlWorkSheet, operacion, cotizacion, movimiento, poliza);
                if (File.Exists(rutaSave))
                {
                    File.Delete(rutaSave);
                }
                xWb.SaveAs(rutaSave);
                xWb.Close();
                app.Quit();
            }
            catch (Exception ex)
            {
                //ELog.save(this, ex);
                app.Quit();
            }
            return rutaExcel;
        }
        private string obtenerConsecutivo(string actual)
        {
            string letraActual = actual.Substring(0, 1).ToString();
            string letraSiguiente = ((char)((int)letraActual.ToCharArray()[0] + 1)).ToString();
            return string.Format("{0}{1}", letraSiguiente, actual.Substring(1, actual.Length - 1));
        }

        private void GenerateData(Excel.Worksheet oSheet, int operacions, string cotizacion, int movimiento, string poliza)
        {
            try
            {
                string Cell = "A1";
                for (int i = 0; i <= Cabecera.Length - 1; i++)
                {
                    oSheet.Range[Cell].Font.Bold = true;
                    oSheet.Range[Cell].Value = this.Cabecera[i];
                    Cell = obtenerConsecutivo(Cell);
                }

                switch (operacions)
                {
                    case 1:
                        for (int i = 2; i < NumFilas + 2; i++)
                        {
                            oSheet.Range["D" + i.ToString()].Validation.Add(Excel.XlDVType.xlValidateList, Type.Missing,
                           Excel.XlFormatConditionOperator.xlBetween, Sexo);
                            if (typeTrabajadors.Count > 0)
                            {
                                oSheet.Range["E" + i.ToString()].Validation.Add(Excel.XlDVType.xlValidateList, Type.Missing,
                                Excel.XlFormatConditionOperator.xlBetween, StrTtrabajador);
                            }

                            oSheet.Range["F" + i.ToString()].Validation.Add(Excel.XlDVType.xlValidateList, Type.Missing,
                            Excel.XlFormatConditionOperator.xlBetween, StrPais);

                            oSheet.Range["G" + i.ToString()].Validation.Add(Excel.XlDVType.xlValidateList, Type.Missing,
                            Excel.XlFormatConditionOperator.xlBetween, StrDocumentos);
                            if (Sedes.Count > 0)
                            {
                                oSheet.Range["M" + i.ToString()].Validation.Add(Excel.XlDVType.xlValidateList, Type.Missing,
                                Excel.XlFormatConditionOperator.xlBetween, StrSede);
                            }
                            oSheet.Range["N" + i.ToString()].Validation.Add(Excel.XlDVType.xlValidateList, Type.Missing,
                            Excel.XlFormatConditionOperator.xlBetween, StrMov);
                        }
                        break;
                    case 0:
                        var _DobjExcel = policyDA.getListAseguradosExcel(cotizacion, poliza);
                        if (_DobjExcel != null)
                        {
                            for (int i = 0; i < _DobjExcel.Count - 1; i++)
                            {
                                oSheet.Range["A" + (i + 2).ToString()].Value = (_DobjExcel[i].SFIRSTNAME == null ? "" : _DobjExcel[i].SFIRSTNAME.Trim());
                                oSheet.Range["B" + (i + 2).ToString()].Value = (_DobjExcel[i].SLASTNAME == null ? "" : _DobjExcel[i].SLASTNAME.Trim());
                                oSheet.Range["C" + (i + 2).ToString()].Value = (_DobjExcel[i].SLASTNAME2 == null ? "" : _DobjExcel[i].SLASTNAME2.Trim());
                                oSheet.Range["D" + (i + 2).ToString()].Value = (_DobjExcel[i].SEXO == null ? "" : _DobjExcel[i].SEXO.Trim());
                                oSheet.Range["E" + (i + 2).ToString()].Value = (_DobjExcel[i].SDESCRIPT_TRAB == null ? "" : _DobjExcel[i].SDESCRIPT_TRAB.Trim());
                                oSheet.Range["F" + (i + 2).ToString()].Value = (_DobjExcel[i].SDESCRIPT_NAC == null ? "" : _DobjExcel[i].SDESCRIPT_NAC.Trim());
                                oSheet.Range["G" + (i + 2).ToString()].Value = (_DobjExcel[i].SDESCRIPT_DOC == null ? "" : _DobjExcel[i].SDESCRIPT_DOC.Trim());
                                oSheet.Range["H" + (i + 2).ToString()].Value = (_DobjExcel[i].SIDDOC == null ? "" : _DobjExcel[i].SIDDOC.Trim());
                                oSheet.Range["I" + (i + 2).ToString()].Value = (_DobjExcel[i].DBIRTHDAT == null ? "" : _DobjExcel[i].DBIRTHDAT.ToString("dd/MM/yyyy"));
                                oSheet.Range["J" + (i + 2).ToString()].Value = (_DobjExcel[i].NREMUNERACION == 0 ? "" : _DobjExcel[i].NREMUNERACION.ToString("N2"));
                                oSheet.Range["K" + (i + 2).ToString()].Value = (_DobjExcel[i].CORREO == null ? "" : _DobjExcel[i].CORREO.Trim());
                                oSheet.Range["L" + (i + 2).ToString()].Value = (_DobjExcel[i].CELULAR == null ? "" : _DobjExcel[i].CELULAR.Trim());
                                oSheet.Range["M" + (i + 2).ToString()].Value = (_DobjExcel[i].SDESCRIPTION == null ? "" : _DobjExcel[i].SDESCRIPTION.Trim());
                                if (movimiento == 5)
                                {
                                    oSheet.Range["N" + (i + 2).ToString()].Value = (i % 2) == 0 ? "E" : "I";
                                }
                            }
                        }
                        break;

                }

                oSheet.Columns.AutoFit();
            }
            catch (Exception ex)
            {
                //ELog.save(this, ex);
            }
        }

        public ResponseAccountStatusVM GetTramaMovimientoSCTR(ParamsMovimientoVL param)
        {
            ResponseAccountStatusVM response = new ResponseAccountStatusVM();
            List<PrintPathsVM> ListPath = new PrintPolicyDA().GetPathsPrint(param.documentCode);
            PrintPathsVM path = ListPath[0];
            List<MovimientoTramaVL> listMovimiento = GetDataMovimientoSCTR(param);

            string pathNameFileTemplate = path.SRUTA_TEMPLATE + path.SNAME_TEMPLATE + ".xlsx";
            try
            {
                MemoryStream ms = new MemoryStream();
                using (FileStream fs = new FileStream(pathNameFileTemplate, FileMode.Open, FileAccess.Read))
                {
                    fs.CopyTo(ms);
                }

                using (SLDocument sl = new SLDocument(ms))
                {
                    var cab = listMovimiento[0];
                    sl.SetCellValue("C6", cab.Contratante);
                    sl.SetCellValue("C7", cab.Ruc);
                    sl.SetCellValue("C8", cab.Moneda);

                    int indicador = 13;

                    if (param.documentCode == Convert.ToInt32(ELog.obtainConfig("condicionadoTramaAP")))
                    {
                        foreach (MovimientoTramaVL item in listMovimiento)
                        {
                            sl.SetCellValue("B" + indicador, item.Rol);
                            sl.SetCellValue("C" + indicador, item.Rol.ToUpper() == "ASEGURADO" ? item.TipoDocumento : item.TipoDocumentoBeneficiario);
                            sl.SetCellValue("D" + indicador, item.Rol.ToUpper() == "ASEGURADO" ? item.NumeroDocumento : item.NumeroDocumentoBeneficiario);
                            sl.SetCellValue("E" + indicador, item.ApellidoPaterno);
                            sl.SetCellValue("F" + indicador, item.ApellidoMaterno);
                            sl.SetCellValue("G" + indicador, item.Nombre);
                            sl.SetCellValue("H" + indicador, item.Sexo);
                            sl.SetCellValue("I" + indicador, !string.IsNullOrEmpty(item.FechaNac) ? Convert.ToDateTime(item.FechaNac).ToShortDateString() : "");
                            sl.SetCellValue("J" + indicador, item.Sueldo);
                            sl.SetCellValue("K" + indicador, item.Email);
                            sl.SetCellValue("L" + indicador, item.Celular);
                            sl.SetCellValue("M" + indicador, item.Rol.ToUpper() != "ASEGURADO" ? item.TipoDocumento : "");
                            sl.SetCellValue("N" + indicador, item.Rol.ToUpper() != "ASEGURADO" ? item.NumeroDocumento : "");
                            sl.SetCellValue("O" + indicador, item.Porcentaje_participacion);
                            sl.SetCellValue("P" + indicador, item.Relacion);
                            sl.SetCellValue("Q" + indicador, item.Pais);
                            indicador++;
                        }
                    }
                    else
                    {
                        foreach (MovimientoTramaVL item in listMovimiento)
                        {

                            sl.SetCellValue("B" + indicador, item.TipoDocumento);
                            sl.SetCellValue("C" + indicador, item.NumeroDocumento);

                            sl.SetCellValue("D" + indicador, item.ApellidoPaterno);
                            sl.SetCellValue("E" + indicador, item.ApellidoMaterno);
                            sl.SetCellValue("F" + indicador, item.Nombre);
                            sl.SetCellValue("G" + indicador, item.Sexo);
                            if (item.FechaNac != "")
                            {
                                DateTime? date = new DateTime?();
                                date = Convert.ToDateTime(item.FechaNac);
                                sl.SetCellValue("H" + indicador, date.Value);
                            }
                            else
                            {
                                sl.SetCellValue("H" + indicador, "");
                            }
                            sl.SetCellValue("I" + indicador, item.TipoTrabajador);
                            sl.SetCellValue("J" + indicador, item.Sueldo);
                            sl.SetCellValue("K" + indicador, item.Email);
                            sl.SetCellValue("L" + indicador, item.Celular);
                            sl.SetCellValue("M" + indicador, item.Sede);
                            sl.SetCellValue("N" + indicador, item.TipoMovimiento);
                            sl.SetCellValue("O" + indicador, item.Pais);
                            indicador++;
                        }
                    }

                    using (MemoryStream stre = new MemoryStream())
                    {
                        sl.SaveAs(stre);
                        var inputAsString = Convert.ToBase64String(stre.ToArray(), 0, stre.ToArray().Length);
                        response.path = inputAsString;
                    }
                }
            }
            catch (Exception ex)
            {
                response.message = ex.ToString();
                response.path = string.Empty;
                response.indEECC = 1;
            }

            return response;

        }

        private List<MovimientoTramaVL> GetDataMovimientoSCTR(ParamsMovimientoVL param)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<MovimientoTramaVL> listEntity = new List<MovimientoTramaVL>();
            string storedProcedureName = ProcedureName.sp_DescargaTramaSCTR;
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, param.idCotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMOVEMENT", OracleDbType.Int32, param.idMovimiento, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, param.codProcess, ParameterDirection.Input));

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        MovimientoTramaVL item = new MovimientoTramaVL();
                        item.Contratante = odr["CONTRATANTE"] == DBNull.Value ? string.Empty : odr["CONTRATANTE"].ToString();
                        item.Ruc = odr["RUC"] == DBNull.Value ? string.Empty : odr["RUC"].ToString();
                        item.Moneda = odr["MONEDA"] == DBNull.Value ? string.Empty : odr["MONEDA"].ToString();
                        item.TipoDocumento = odr["TIPO_DE_DOCUMENTO"] == DBNull.Value ? string.Empty : odr["TIPO_DE_DOCUMENTO"].ToString();
                        item.NumeroDocumento = odr["NUM_DOCUMENTO"] == DBNull.Value ? string.Empty : odr["NUM_DOCUMENTO"].ToString();
                        item.ApellidoPaterno = odr["APELLIDO_PATERNO"] == DBNull.Value ? string.Empty : odr["APELLIDO_PATERNO"].ToString();
                        item.ApellidoMaterno = odr["APELLIDO_MATERNO"] == DBNull.Value ? string.Empty : odr["APELLIDO_MATERNO"].ToString();
                        item.Nombre = odr["NOMBRES"] == DBNull.Value ? string.Empty : odr["NOMBRES"].ToString();
                        item.Sexo = odr["SEXO"] == DBNull.Value ? string.Empty : odr["SEXO"].ToString();
                        item.FechaNac = odr["FECHA_NAC"] == DBNull.Value ? string.Empty : odr["FECHA_NAC"].ToString();
                        item.TipoTrabajador = odr["TIPO_TRABAJADOR"] == DBNull.Value ? string.Empty : odr["TIPO_TRABAJADOR"].ToString();
                        item.Sueldo = odr["SUELDO_BRUTO"] == DBNull.Value ? string.Empty : odr["SUELDO_BRUTO"].ToString();
                        item.Sede = odr["SEDE"] == DBNull.Value ? string.Empty : odr["SEDE"].ToString();
                        item.TipoMovimiento = odr["TIPO_DE_MOVIMIENTO"] == DBNull.Value ? string.Empty : odr["TIPO_DE_MOVIMIENTO"].ToString();
                        item.Pais = odr["PAIS_NACIMIENTO"] == DBNull.Value ? string.Empty : odr["PAIS_NACIMIENTO"].ToString();
                        item.Email = odr["EMAIL"] == DBNull.Value ? string.Empty : odr["EMAIL"].ToString();
                        item.Celular = odr["CELULAR"] == DBNull.Value ? string.Empty : odr["CELULAR"].ToString();
                        item.TipoDocumentoBeneficiario = odr["TIPO_DOCUMENTO_BEN"] == DBNull.Value ? string.Empty : odr["TIPO_DOCUMENTO_BEN"].ToString();
                        item.NumeroDocumentoBeneficiario = odr["NUM_DOCUMENTO_BEN"] == DBNull.Value ? string.Empty : odr["NUM_DOCUMENTO_BEN"].ToString();
                        item.TipoPlan = odr["TIPO_PLAN"] == DBNull.Value ? string.Empty : odr["TIPO_PLAN"].ToString();
                        item.Porcentaje_participacion = odr["PORCENTAJE_PAR"] == DBNull.Value ? string.Empty : odr["PORCENTAJE_PAR"].ToString();
                        item.Rol = odr["ROL"] == DBNull.Value ? string.Empty : odr["ROL"].ToString();
                        item.Relacion = odr["RELACION"] == DBNull.Value ? string.Empty : odr["RELACION"].ToString();

                        listEntity.Add(item);
                    }
                }
                odr.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("GetDataMovimientoSCTR", ex.ToString(), "3");
            }

            return listEntity;
        }

        public ResponseAccountStatusVM GetTramaEndoso(ParamsMovimientoVL param)
        {
            ResponseAccountStatusVM response = new ResponseAccountStatusVM();
            List<PrintPathsVM> ListPath = new PrintPolicyDA().GetPathsPrint(param.documentCode);
            PrintPathsVM path = ListPath[0];
            List<MovimientoTramaVL> listMovimiento = GetDataEndoso(param);

            string pathNameFileTemplate = path.SRUTA_TEMPLATE + "01_trama_Vida_Ley_endoso" + ".xlsx";
            try
            {
                MemoryStream ms = new MemoryStream();
                using (FileStream fs = new FileStream(pathNameFileTemplate, FileMode.Open, FileAccess.Read))
                {
                    fs.CopyTo(ms);
                }
                using (SLDocument sl = new SLDocument(ms))
                {
                    var cab = listMovimiento[0];
                    sl.SetCellValue("C6", cab.TipoEndosoDes);

                    int indicador = 10;

                    foreach (MovimientoTramaVL item in listMovimiento)
                    {
                        sl.SetCellValue("B" + indicador, item.TipoDocumento);
                        sl.SetCellValue("C" + indicador, item.NumeroDocumento);
                        sl.SetCellValue("D" + indicador, item.ApellidoPaterno);
                        sl.SetCellValue("E" + indicador, item.ApellidoMaterno);
                        sl.SetCellValue("F" + indicador, item.Nombre);
                        sl.SetCellValue("G" + indicador, item.Sexo);
                        sl.SetCellValue("H" + indicador, item.Email);
                        sl.SetCellValue("I" + indicador, item.Celular);
                        sl.SetCellValue("J" + indicador, item.Sueldo);
                        if (item.FechaNac != "")
                        {
                            DateTime? date = new DateTime?();
                            date = Convert.ToDateTime(item.FechaNac);
                            sl.SetCellValue("K" + indicador, date.Value);
                        }
                        else
                        {
                            sl.SetCellValue("K" + indicador, "");
                        }
                        sl.SetCellValue("L" + indicador, item.ApellidoPaternoN);
                        sl.SetCellValue("M" + indicador, item.ApellidoMaternoN);
                        sl.SetCellValue("N" + indicador, item.NombreN);
                        sl.SetCellValue("O" + indicador, item.SexoN);
                        sl.SetCellValue("P" + indicador, item.EmailN);
                        sl.SetCellValue("Q" + indicador, item.CelularN);
                        sl.SetCellValue("R" + indicador, item.SueldoN);
                        if (item.FechaNacN != "")
                        {
                            DateTime? date = new DateTime?();
                            date = Convert.ToDateTime(item.FechaNacN);
                            sl.SetCellValue("S" + indicador, date.Value);
                        }
                        else
                        {
                            sl.SetCellValue("S" + indicador, "");
                        }
                        indicador++;
                    }

                    using (MemoryStream stre = new MemoryStream())
                    {
                        sl.SaveAs(stre);
                        var inputAsString = Convert.ToBase64String(stre.ToArray(), 0, stre.ToArray().Length);
                        response.path = inputAsString;
                    }
                }
            }
            catch (Exception ex)
            {
                response.message = ex.Message;
                response.path = string.Empty;
                response.indEECC = 1;
            }

            return response;

        }

        private List<MovimientoTramaVL> GetDataEndoso(ParamsMovimientoVL param)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<MovimientoTramaVL> listEntity = new List<MovimientoTramaVL>();
            string storedProcedureName = "PKG_PD_VALIDACION_GEN.SP_GET_CARGA_TRAMA_ENDOSO";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, param.codProcess, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, param.idCotizacion, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        MovimientoTramaVL item = new MovimientoTramaVL();
                        /*item.Contratante = odr["CONTRATANTE"] == DBNull.Value ? string.Empty : odr["CONTRATANTE"].ToString();
                        item.Ruc = odr["RUC"] == DBNull.Value ? string.Empty : odr["RUC"].ToString();
                        item.Moneda = odr["MONEDA"] == DBNull.Value ? string.Empty : odr["MONEDA"].ToString();*/
                        item.TipoDocumento = odr["NIDDOC_TYPE"] == DBNull.Value ? string.Empty : odr["NIDDOC_TYPE"].ToString();
                        item.NumeroDocumento = odr["SIDDOC"] == DBNull.Value ? string.Empty : odr["SIDDOC"].ToString();
                        item.ApellidoPaterno = odr["SLASTNAME"] == DBNull.Value ? string.Empty : odr["SLASTNAME"].ToString();
                        item.ApellidoMaterno = odr["SLASTNAME2"] == DBNull.Value ? string.Empty : odr["SLASTNAME2"].ToString();
                        item.Nombre = odr["SFIRSTNAME"] == DBNull.Value ? string.Empty : odr["SFIRSTNAME"].ToString();
                        item.Sexo = odr["SSEXCLIEN"] == DBNull.Value ? string.Empty : odr["SSEXCLIEN"].ToString();
                        item.Email = odr["SE_MAIL"] == DBNull.Value ? string.Empty : odr["SE_MAIL"].ToString();
                        item.Celular = odr["SPHONE"] == DBNull.Value ? string.Empty : odr["SPHONE"].ToString();
                        item.Sueldo = odr["NREMUNERACION"] == DBNull.Value ? string.Empty : odr["NREMUNERACION"].ToString();
                        item.FechaNac = odr["DBIRTHDAT"] == DBNull.Value ? string.Empty : odr["DBIRTHDAT"].ToString();
                        /**/
                        item.ApellidoPaternoN = odr["SLASTNAME_NEW"] == DBNull.Value ? string.Empty : odr["SLASTNAME_NEW"].ToString();
                        item.ApellidoMaternoN = odr["SLASTNAME2_NEW"] == DBNull.Value ? string.Empty : odr["SLASTNAME2_NEW"].ToString();
                        item.NombreN = odr["SFIRSTNAME_NEW"] == DBNull.Value ? string.Empty : odr["SFIRSTNAME_NEW"].ToString();
                        item.SexoN = odr["SSEXCLIEN_NEW"] == DBNull.Value ? string.Empty : odr["SSEXCLIEN_NEW"].ToString();
                        item.EmailN = odr["SE_MAIL_NEW"] == DBNull.Value ? string.Empty : odr["SE_MAIL_NEW"].ToString();
                        item.CelularN = odr["SPHONE_NEW"] == DBNull.Value ? string.Empty : odr["SPHONE_NEW"].ToString();
                        item.SueldoN = odr["NREMUNERACION_NEW"] == DBNull.Value ? string.Empty : odr["NREMUNERACION_NEW"].ToString();
                        item.FechaNacN = odr["DBIRTHDAT_NEW"] == DBNull.Value ? string.Empty : odr["DBIRTHDAT_NEW"].ToString();

                        item.TipoEndoso = odr["NTYPE_END"] == DBNull.Value ? string.Empty : odr["NTYPE_END"].ToString();
                        item.TipoEndosoDes = odr["STYPE_ENDOSO"] == DBNull.Value ? string.Empty : odr["STYPE_ENDOSO"].ToString();

                        listEntity.Add(item);
                    }
                }
                odr.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("GetDataEndoso", ex.ToString(), "3");
            }
            return listEntity;
        }

        private List<MovimientoTramaVL> GetDataCover(ParamsMovimientoVL param)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<MovimientoTramaVL> listEntity = new List<MovimientoTramaVL>();
            string storedProcedureName = "PKG_PV_CONDICIONADOS.QUOTATION_COVER_GEN";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, param.idCotizacion, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        MovimientoTramaVL item = new MovimientoTramaVL();
                        item.nCobertura = odr["NCOVER"] == DBNull.Value ? string.Empty : odr["NCOVER"].ToString();
                        item.Cobertura = odr["SDESCRIPT_COVER_PR"] == DBNull.Value ? string.Empty : odr["SDESCRIPT_COVER_PR"].ToString();

                        listEntity.Add(item);
                    }
                }
                odr.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("GetDataCover", ex.ToString(), "3");
            }
            return listEntity;
        }

        private List<MovimientoTramaVL> GetDataCoverCo(ParamsMovimientoVL param)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<MovimientoTramaVL> listEntity = new List<MovimientoTramaVL>();
            string storedProcedureName = "PKG_PV_CONDICIONADOS.QUOTATION_COVER_CO_GEN";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, param.idCotizacion, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                try
                {
                    if (odr.HasRows)
                    {
                        while (odr.Read())
                        {
                            MovimientoTramaVL item = new MovimientoTramaVL();
                            item.nCobertura = odr["NCOVER"] == DBNull.Value ? string.Empty : odr["NCOVER"].ToString();
                            item.Cobertura = odr["SDESCRIPT_COVER_CO"] == DBNull.Value ? string.Empty : odr["SDESCRIPT_COVER_CO"].ToString();

                            listEntity.Add(item);
                        }
                    }
                }
                catch (Exception)
                {
                    odr.Close();
                }
                odr.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("GetDataCoverCo", ex.ToString(), "3");
            }
            return listEntity;
        }

        public ResponseAccountStatusVM GetTramaCover(ParamsMovimientoVL param)
        {
            ResponseAccountStatusVM response = new ResponseAccountStatusVM();
            List<PrintPathsVM> ListPath = new PrintPolicyDA().GetPathsPrint(param.documentCode);
            PrintPathsVM path = ListPath[0];
            List<MovimientoTramaVL> listCover = GetDataCover(param);
            List<MovimientoTramaVL> listCover2 = GetDataCoverCo(param);

            foreach (var item in listCover2)
            {
                listCover.Add(item);
            }

            string pathNameFileTemplate = path.SRUTA_TEMPLATE + "01_trama_cover" + ".xlsx";
            try
            {
                MemoryStream ms = new MemoryStream();
                using (FileStream fs = new FileStream(pathNameFileTemplate, FileMode.Open, FileAccess.Read))
                {
                    fs.CopyTo(ms);
                }
                using (SLDocument sl = new SLDocument(ms))
                {
                    int indicador = 8;

                    foreach (MovimientoTramaVL item in listCover)
                    {
                        sl.SetCellValue("B" + indicador, item.nCobertura);
                        sl.SetCellValue("C" + indicador, item.Cobertura.ToString().Trim());
                        indicador++;
                    }

                    using (MemoryStream stre = new MemoryStream())
                    {
                        sl.SaveAs(stre);
                        var inputAsString = Convert.ToBase64String(stre.ToArray(), 0, stre.ToArray().Length);
                        response.path = inputAsString;
                    }
                }
            }
            catch (Exception ex)
            {
                response.message = ex.Message;
                response.path = string.Empty;
                response.indEECC = 1;
            }

            return response;

        }


        public ResponseAccountStatusVM GetPlantillaVidaLey(GetTrama data)
        {
            ResponseAccountStatusVM response = new ResponseAccountStatusVM();
            string pathNameFileTemplate = ELog.obtainConfig("modeloVidaLey") + "TRAMA_VL.xlsm";
            try
            {
                MemoryStream ms = new MemoryStream();
                using (FileStream fs = new FileStream(pathNameFileTemplate, FileMode.Open, FileAccess.Read))
                {
                    fs.CopyTo(ms);
                }
                using (SLDocument sl = new SLDocument(ms))
                {
                    //sl.SetCellValue("A1", "Prueba");
                    using (MemoryStream stre = new MemoryStream())
                    {
                        sl.SaveAs(stre);
                        var inputAsString = Convert.ToBase64String(stre.ToArray(), 0, stre.ToArray().Length);
                        response.path = inputAsString;
                    }
                }
            }
            catch (Exception ex)
            {
                response.message = ex.Message;
                response.path = string.Empty;
                response.indEECC = 1;
            }
            return response;
        }

        // AGF 12052023 Beneficiarios VCF
        public ResponseAccountStatusVM GetPlantillaVCF(GetTrama data)
        {
            ResponseAccountStatusVM response = new ResponseAccountStatusVM();
            string pathNameFileTemplate = ELog.obtainConfig("rutaExcelVCF") + "01_TramaBeneficiarios_VCF.xlsm";
            //string pathNameFileTemplate = "\\doc_templates\\cobranzas\\qa\\01_TramaBeneficiarios_VCF.xlsm";

            try
            {
                MemoryStream ms = new MemoryStream();
                using (FileStream fs = new FileStream(pathNameFileTemplate, FileMode.Open, FileAccess.Read))
                {
                    fs.CopyTo(ms);
                }
                using (SLDocument sl = new SLDocument(ms))
                {
                    using (MemoryStream stre = new MemoryStream())
                    {
                        sl.SaveAs(stre);
                        var inputAsString = Convert.ToBase64String(stre.ToArray(), 0, stre.ToArray().Length);
                        response.path = inputAsString;
                    }
                }
            }
            catch (Exception ex)
            {
                response.message = ex.Message;
                response.path = string.Empty;
                response.indEECC = 1;
            }
            return response;
        }

    }
    public enum operation
    {
        Emision = 0,
        Cotizacion = 1
    }

}
