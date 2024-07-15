using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WSPlataforma.Entities.ReportModel.ViewModel;
using WSPlataforma.Entities.ReportModel.BindingModel;
using WSPlataforma.Util;
using SpreadsheetLight;

namespace WSPlataforma.DA
{
    public class BillReportDA : ConnectionBase
    {
        public List<PayStateVM> ObtenerEstadosDePago()
        {
            List<OracleParameter> parameters = new List<OracleParameter>();
            List<PayStateVM> lista = new List<PayStateVM>();
            string storedProcedureName = ProcedureName.pkg_ReportesProd + ".REA_FORMA_PAGO";

            try
            {
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameters);
                while (odr.Read())
                {
                    PayStateVM item = new PayStateVM();
                    item.COD_FORMA = Convert.ToInt16(odr["COD_FORMA"].ToString());
                    item.DES_FORMA = odr["DES_FORMA"].ToString();
                    lista.Add(item);
                }
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lista;
        }

        public List<BillStateVM> ObtenerEstadosFactura()
        {
            List<OracleParameter> parameters = new List<OracleParameter>();
            List<BillStateVM> lista = new List<BillStateVM>();

            string storedProcedureName = ProcedureName.pkg_ReportesProd + ".REA_ESTADO_FACTURA";

            try
            {
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameters);
                while (odr.Read())
                {
                    BillStateVM item = new BillStateVM();
                    item.COD_ESTADO = Convert.ToInt16(odr["COD_ESTADO"].ToString());
                    item.DES_ESTADO = odr["DES_ESTADO"].ToString();
                    lista.Add(item);
                }
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lista;
        }

        public Dictionary<string, object> ObtenerReporteDeFecturas(BillReportFiltersBM data)
        {
            Dictionary<string, object> map = new Dictionary<string, object>();
            List<OracleParameter> parameters = new List<OracleParameter>();
            string storedProcedureName = ProcedureName.sp_ReporteFactura;
            try
            {
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int64, data.nBranch, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int64, data.nProduct, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int64, data.nPolicy, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DEFFECDATE_INICIO", OracleDbType.Date, Convert.ToDateTime(data.startDate), ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DEFFECDATE_FIN", OracleDbType.Date, Convert.ToDateTime(data.endDate), ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NBILLSTAT", OracleDbType.Int64, data.nState, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPAY_TYPE", OracleDbType.Int64, data.nPayType, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, data.sDoc, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NIDDOC_TYPE", OracleDbType.Int64, data.nIdDocType, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SCLIENAME", OracleDbType.Varchar2, data.SLEGALNAME, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SFIRSTNAME", OracleDbType.Varchar2, data.SFIRSTNAME, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SLASTNAME", OracleDbType.Varchar2, data.SLASTNAME, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SLASTNAME2", OracleDbType.Varchar2, data.SLASTNAME2, ParameterDirection.Input));

                parameters.Add(new OracleParameter("P_SFIRSTNAME_BR", OracleDbType.Varchar2, data.SFIRSTNAME_BR, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SLASTNAME_BR", OracleDbType.Varchar2, data.SLASTNAME_BR, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SLASTNAME2_BR", OracleDbType.Varchar2, data.SLASTNAME2_BR, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SLEGALNAME_BR", OracleDbType.Varchar2, data.SLEGALNAME_BR, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SIDDOC_BR", OracleDbType.Varchar2, data.sDoc_BR, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NIDDOC_TYPE_BR", OracleDbType.Int64, data.nIdDocType_BR, ParameterDirection.Input));

                var P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int64, ParameterDirection.Output);
                parameters.Add(P_COD_ERR);
                var P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                P_MESSAGE.Size = 2048;
                parameters.Add(P_MESSAGE);
                var C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameters.Add(C_TABLE);

                List<BillReportVM> lista = new List<BillReportVM>();

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameters);
                map["codErr"] = Convert.ToInt64(P_COD_ERR.Value.ToString());
                map["message"] = P_MESSAGE.Value.ToString();


                if ((long)map["codErr"] == 0)
                {
                    while (odr.Read())
                    {
                        BillReportVM item = new BillReportVM();
                        item.RAMO = odr["RAMO"];
                        item.PRODUCTO = odr["PRODUCTO"];
                        item.TIPO_DOC = odr["TIPO_DOC"];
                        item.RAZON_SOCIAL = odr["RAZON_SOCIAL"];
                        item.TIPO_MOV = odr["TIPO_MOV"];
                        item.NRO_DOC_CONTR = odr["NRO_DOC_CONTR"];
                        item.NRO_POLIZA = odr["NRO_POLIZA"];
                        item.NRO_RECIBO = odr["NRO_RECIBO"];
                        item.INICIO_DE_VIGENCIA = odr["INICIO_DE_VIGENCIA"];
                        item.FIN_DE_VIGENCIA = odr["FIN_DE_VIGENCIA"];
                        item.NRO_DOC_FACTURA = odr["NRO_DOC_FACTURA"];
                        item.TIPO_COMPROBANTE = odr["TIPO_COMPROBANTE"] == DBNull.Value ? "-" : odr["TIPO_COMPROBANTE"];
                        item.SERIE_DOC = odr["SERIE_DOC"] == DBNull.Value ? "-" : odr["SERIE_DOC"];
                        item.NRO_DOC_FACTURA = odr["NRO_DOC_FACTURA"] == DBNull.Value ? "-" : odr["NRO_DOC_FACTURA"];
                        item.FECHA_EMISION = odr["FECHA_EMISION"] == DBNull.Value ? "-" : odr["FECHA_EMISION"];
                        item.FECHA_DE_PAGO = odr["FECHA_DE_PAGO"] == DBNull.Value ? "-" : odr["FECHA_DE_PAGO"];
                        item.ESTADO = odr["ESTADO"] == DBNull.Value ? "-" : odr["ESTADO"];
                        item.PRIMA_TOTAL = odr["PRIMA_TOTAL"] == DBNull.Value ? "-" : String.Format("{0:#,0.00}", odr["PRIMA_TOTAL"]);
                        item.MONEDA = odr["MONEDA"] == DBNull.Value ? "-" : odr["MONEDA"];
                        item.TIPO_DE_PAGO = odr["TIPO_DE_PAGO"] == DBNull.Value ? "-" : odr["TIPO_DE_PAGO"];
                        item.DIAS_DE_CREDITO = odr["DIAS_DE_CREDITO"] == DBNull.Value ? "-" : odr["DIAS_DE_CREDITO"];
                        item.INTERMEDIARIO = odr["INTERMEDIARIO"] == DBNull.Value ? "-" : odr["INTERMEDIARIO"];
                        item.NOMUSER = odr["NOMUSER"] == DBNull.Value ? "-" : odr["NOMUSER"];
                        lista.Add(item);
                    }
                }
                ELog.CloseConnection(odr);
                map["lista"] = lista;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return map;
        }

        public string ObtenerPlantilla(BillReportFiltersBM data)
        {
            Dictionary<string, object> mapa = this.ObtenerReporteDeFecturas(data);
            List<BillReportVM> lista = data.searchButtonPressed ? (List<BillReportVM>)mapa["lista"] : new List<BillReportVM>();
            string templatePath = "D:/doc_templates/reportes/dev/MiPlantilla.xlsx";
            string plantilla = "";
            try
            {
                MemoryStream ms = new MemoryStream();
                using (FileStream fs = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
                {
                    fs.CopyTo(ms);
                }
                using (SLDocument sl = new SLDocument(ms))
                {
                    int i = 7;
                    int letra = 65;

                    sl.SetCellValue("B1", data.startDate);
                    sl.SetCellValue("B2", data.endDate);

                    foreach (BillReportVM item in lista)
                    {
                        int c = 0;
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.RAMO);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.PRODUCTO);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.TIPO_DOC);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.NRO_DOC_CONTR);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.RAZON_SOCIAL);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.NRO_POLIZA);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.TIPO_MOV);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.NRO_RECIBO);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.INICIO_DE_VIGENCIA);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.FIN_DE_VIGENCIA);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.INTERMEDIARIO);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.TIPO_COMPROBANTE);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.SERIE_DOC);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.NRO_DOC_FACTURA);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.FECHA_EMISION);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.FECHA_DE_PAGO);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.ESTADO);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.MONEDA);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.PRIMA_TOTAL);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.TIPO_DE_PAGO);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.DIAS_DE_CREDITO);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.NOMUSER);
                        i++;
                    }
                    using (MemoryStream ms2 = new MemoryStream())
                    {
                        sl.SaveAs(ms2);
                        plantilla = Convert.ToBase64String(ms2.ToArray(), 0, ms2.ToArray().Length);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return plantilla;
        }
    }
}
