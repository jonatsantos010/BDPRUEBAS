using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Oracle.DataAccess.Client;
using WSPlataforma.Util;
using WSPlataforma.Entities.ReceiptReportModel.ViewModel;
using WSPlataforma.Entities.ReceiptReportModel.BindingModel;

namespace WSPlataforma.DA
{
    public class ReceiptReportDA : ConnectionBase
    {
        private readonly string Package = "PKG_REPORTE_RECIBOS";

        // RAMOS
        public Task<RamosVM> ListarRamos()
        {
            var parameters = new List<OracleParameter>();
            RamosVM entities = new RamosVM();
            entities.lista = new List<RamosVM.C_TABLE>();
            var procedure = string.Format("{0}.{1}", this.Package, "REA_LIST_BRANCH");
            try
            {
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        RamosVM.C_TABLE suggest = new RamosVM.C_TABLE();
                        suggest.NBRANCH = reader["NBRANCH"] == DBNull.Value ? 0 : int.Parse(reader["NBRANCH"].ToString());
                        suggest.SDESCRIPT = reader["SDESCRIPT"] == DBNull.Value ? string.Empty : reader["SDESCRIPT"].ToString();
                        entities.lista.Add(suggest);
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

        // EXPORTAR DATA RECIBOS
        public Task<ExportDataReceiptVM> ExportarDataRecibos(ExportDataReceiptBM data)
        {
            var parameters = new List<OracleParameter>();
            ExportDataReceiptVM entities = new ExportDataReceiptVM();
            entities.lista = new List<ExportDataReceiptVM.C_TABLE>();
            var procedure = string.Format("{0}.{1}", this.Package, "SPS_EXPORTAR_DATA_RECEIPTS");
            try
            {
                parameters.Add(new OracleParameter("P_NIDHEADER", OracleDbType.Int32, data.P_NIDHEADER, ParameterDirection.Input));
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ExportDataReceiptVM.C_TABLE suggest = new ExportDataReceiptVM.C_TABLE();
                        suggest.PRODUCTO = reader["PRODUCTO"] == DBNull.Value ? string.Empty : reader["PRODUCTO"].ToString();
                        suggest.TIPO = reader["TIPO"] == DBNull.Value ? string.Empty : reader["TIPO"].ToString();
                        suggest.MONEDA = reader["MONEDA"] == DBNull.Value ? string.Empty : reader["MONEDA"].ToString();
                        suggest.CODIGO_CONT = reader["CODIGO_CONT"] == DBNull.Value ? string.Empty : reader["CODIGO_CONT"].ToString();
                        suggest.RUC = reader["RUC"] == DBNull.Value ? string.Empty : reader["RUC"].ToString();
                        suggest.RECIBO = reader["RECIBO"] == DBNull.Value ? string.Empty : reader["RECIBO"].ToString();
                        suggest.NOMBRE_CONT = reader["NOMBRE_CONT"] == DBNull.Value ? string.Empty : reader["NOMBRE_CONT"].ToString();
                        suggest.POLIZA = reader["POLIZA"] == DBNull.Value ? string.Empty : reader["POLIZA"].ToString();
                        suggest.CERTIFICADO = reader["CERTIFICADO"] == DBNull.Value ? 0 : int.Parse(reader["CERTIFICADO"].ToString());
                        suggest.INICIO_POL = reader["INICIO_POL"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["INICIO_POL"].ToString());
                        suggest.FIN_POL = reader["FIN_POL"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["FIN_POL"].ToString());
                        suggest.INICIO_RECIBO = reader["INICIO_RECIBO"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["INICIO_RECIBO"].ToString());
                        suggest.FIN_RECIBO = reader["FIN_RECIBO"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["FIN_RECIBO"].ToString());
                        suggest.COMPROBANTE = reader["COMPROBANTE"] == DBNull.Value ? string.Empty : reader["COMPROBANTE"].ToString();
                        suggest.FEC_COMP = reader["FEC_COMP"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["FEC_COMP"].ToString());
                        suggest.GRACIA = reader["GRACIA"] == DBNull.Value ? 0 : int.Parse(reader["GRACIA"].ToString());
                        suggest.VENC_COMP = reader["VENC_COMP"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["VENC_COMP"].ToString());
                        suggest.PROV_COMP = reader["PROV_COMP"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["PROV_COMP"].ToString());
                        suggest.EST_RECIBO = reader["EST_RECIBO"] == DBNull.Value ? string.Empty : reader["EST_RECIBO"].ToString();
                        suggest.FEC_EST_RECIBO = reader["FEC_EST_RECIBO"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["FEC_EST_RECIBO"].ToString());
                        suggest.NETO = reader["NETO"] == DBNull.Value ? 0 : decimal.Parse(reader["NETO"].ToString());
                        suggest.DER_EMIS = reader["DER_EMIS"] == DBNull.Value ? 0 : int.Parse(reader["DER_EMIS"].ToString());
                        suggest.IGV = reader["IGV"] == DBNull.Value ? 0 : decimal.Parse(reader["IGV"].ToString());
                        suggest.TOTAL = reader["TOTAL"] == DBNull.Value ? 0 : decimal.Parse(reader["TOTAL"].ToString());
                        suggest.PORC_COMISION = reader["PORC_COMISION"] == DBNull.Value ? 0 : int.Parse(reader["PORC_COMISION"].ToString());
                        suggest.MONTO_COMISION = reader["MONTO_COMISION"] == DBNull.Value ? 0 : decimal.Parse(reader["MONTO_COMISION"].ToString());
                        suggest.FEC_COMISION = reader["FEC_COMISION"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["FEC_COMISION"].ToString());
                        suggest.EST_SUNAT = reader["EST_SUNAT"] == DBNull.Value ? string.Empty : reader["EST_SUNAT"].ToString();
                        suggest.INT_CONV = reader["INT_CONV"] == DBNull.Value ? string.Empty : reader["INT_CONV"].ToString();
                        suggest.CANAL = reader["CANAL"] == DBNull.Value ? string.Empty : reader["CANAL"].ToString();
                        entities.lista.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ExportarDataRecibos", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<ExportDataReceiptVM>(entities);
        }

        // GENERAR REPORTE RECIBOS SCTR
        public int GenerarReporteRecibosSCTR(ReceiptReportBM data)
        {
            var parameters = new List<OracleParameter>();
            ReceiptReportVM entities = new ReceiptReportVM();
            var procedure = string.Format("{0}.{1}", this.Package, "SPS_GENERAR_REPORTE_RECIBOS_SCTR");
            string fechaInicio = data.P_FEINI.ToString("dd/MM/yyyy");
            string fechaFin = data.P_FEFIN.ToString("dd/MM/yyyy");
            string nPolicy = "";
            string nContratante = "";

            if (data.P_NPOLICY == ""){
                nPolicy = "0";
            }
            if (data.P_NDOCUMENT_CONT == "")
            {
                nContratante = null;
            }


            try
            {
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int16, data.P_NBRANCH, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_FEINI", OracleDbType.NVarchar2, fechaInicio, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_FEFIN", OracleDbType.NVarchar2, fechaFin, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPOLICY", OracleDbType.NVarchar2, nPolicy, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NDOCUMENT_CONT", OracleDbType.NVarchar2, nContratante, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int16, data.P_NUSERCODE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_USER_REG", OracleDbType.NVarchar2, data.P_USER_REG, ParameterDirection.Input));

                OracleParameter P_NIDHEADER = new OracleParameter("P_NIDHEADER", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NIDHEADER);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);
                entities.P_NIDHEADER = Convert.ToInt32(P_NIDHEADER.Value.ToString());
                entities.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                entities.P_SMESSAGE = P_SMESSAGE.Value.ToString();

                return int.Parse(P_NIDHEADER.Value.ToString());

            }
            catch (Exception ex)
            {
                LogControl.save("GenerarReporteRecibosSCTR", ex.ToString(), "3");
                throw;
            }
        }

        // LISTAR REPORTES CREADOS RECIBO
        public Task<ListCreatedReportsReceiptVM> ListCreatedReportsReceipt(ListCreatedReportsReceiptBM data)
        {
            var parameters = new List<OracleParameter>();
            ListCreatedReportsReceiptVM entities = new ListCreatedReportsReceiptVM();
            entities.lista = new List<ListCreatedReportsReceiptVM.C_TABLE>();
            var procedure = string.Format("{0}.{1}", this.Package, "SPS_LIST_CREATED_REPORTS_RECEIPTS");
            try
            {
                parameters.Add(new OracleParameter("P_NIDHEADER", OracleDbType.Int32, data.P_NIDHEADER, ParameterDirection.Input));
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ListCreatedReportsReceiptVM.C_TABLE suggest = new ListCreatedReportsReceiptVM.C_TABLE();
                        suggest.NIDHEADER = reader["NIDHEADER"] == DBNull.Value ? 0 : int.Parse(reader["NIDHEADER"].ToString());
                        suggest.SDESCRIPT = reader["SDESCRIPT"] == DBNull.Value ? string.Empty : reader["SDESCRIPT"].ToString();
                        suggest.NBRANCH = reader["NBRANCH"] == DBNull.Value ? 0 : int.Parse(reader["NBRANCH"].ToString());
                        suggest.SBRANCH = reader["SBRANCH"] == DBNull.Value ? string.Empty : reader["SBRANCH"].ToString();
                        suggest.NPOLICY = reader["NPOLICY"] == DBNull.Value ? string.Empty : reader["NPOLICY"].ToString();
                        suggest.NDOCUMENT_CONT = reader["NDOCUMENT_CONT"] == DBNull.Value ? string.Empty : reader["NDOCUMENT_CONT"].ToString();
                        suggest.SUSERNAME = reader["SUSERNAME"] == DBNull.Value ? string.Empty : reader["SUSERNAME"].ToString();
                        suggest.DREGINIPROD = reader["DREGINIPROD"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["DREGINIPROD"].ToString());
                        suggest.DREGFINPROD = reader["DREGFINPROD"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["DREGFINPROD"].ToString());
                        suggest.NSTATUS = reader["NSTATUS"] == DBNull.Value ? 0 : int.Parse(reader["NSTATUS"].ToString());
                        suggest.SDESCRIPTION = reader["SDESCRIPTION"] == DBNull.Value ? string.Empty : reader["SDESCRIPTION"].ToString();
                        suggest.DCOMPDATE = reader["DCOMPDATE"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["DCOMPDATE"].ToString());
                        entities.lista.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListCreatedReportsReceipt", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<ListCreatedReportsReceiptVM>(entities);
        }

        // LISTAR CABECERA REPORTES RECIBO
        public Task<ListHeaderReportsReceiptVM> ListHeaderReportsReceipt(ListHeaderReportsReceiptBM data)
        {
            var parameters = new List<OracleParameter>();
            ListHeaderReportsReceiptVM entities = new ListHeaderReportsReceiptVM();
            entities.lista = new List<ListHeaderReportsReceiptVM.C_TABLE>();
            var procedure = string.Format("{0}.{1}", this.Package, "SPS_LIST_HEADER_REPORTS_RECEIPTS");
            try
            {
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int16, data.P_NBRANCH, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_FEINI", OracleDbType.Date, data.P_FEINI, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_FEFIN", OracleDbType.Date, data.P_FEFIN, ParameterDirection.Input));
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ListHeaderReportsReceiptVM.C_TABLE suggest = new ListHeaderReportsReceiptVM.C_TABLE();
                        suggest.NSTATUS = reader["NSTATUS"] == DBNull.Value ? 0 : int.Parse(reader["NSTATUS"].ToString());
                        suggest.SDESCRIPTION = reader["SDESCRIPTION"] == DBNull.Value ? string.Empty : reader["SDESCRIPTION"].ToString();
                        suggest.NIDHEADER = reader["NIDHEADER"] == DBNull.Value ? 0 : int.Parse(reader["NIDHEADER"].ToString());
                        suggest.SDESCRIPT = reader["SDESCRIPT"] == DBNull.Value ? string.Empty : reader["SDESCRIPT"].ToString();
                        suggest.NBRANCH = reader["NBRANCH"] == DBNull.Value ? 0 : int.Parse(reader["NBRANCH"].ToString());
                        suggest.SBRANCH = reader["SBRANCH"] == DBNull.Value ? string.Empty : reader["SBRANCH"].ToString();
                        suggest.SUSERNAME = reader["SUSERNAME"] == DBNull.Value ? string.Empty : reader["SUSERNAME"].ToString();
                        suggest.DREGINIPROD = reader["DREGINIPROD"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["DREGINIPROD"].ToString());
                        suggest.DREGFINPROD = reader["DREGFINPROD"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["DREGFINPROD"].ToString());
                        suggest.DCOMPDATE = reader["DCOMPDATE"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["DCOMPDATE"].ToString());
                        entities.lista.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListHeaderReportsReceipt", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<ListHeaderReportsReceiptVM>(entities);
        }
    }
}