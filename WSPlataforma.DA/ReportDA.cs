using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.Entities.PrintPolicyModel.ATP;
using WSPlataforma.Entities.ReportModel.BindingModel;
using WSPlataforma.Entities.ReportModel.ViewModel;
using WSPlataforma.Util;
using SpreadsheetLight;
using WSPlataforma.Entities.VDPModel.BindingModel;
using RestSharp.Extensions;
using System.Globalization;

namespace WSPlataforma.DA
{
    public class ReportDA : ConnectionBase
    {
        public async Task<List<ReportSalesChannelVM>> SaleChannelReport(ReportSalesChannelBM data)
        {
            List<OracleParameter> parameters = new List<OracleParameter>();
            List<ReportSalesChannelVM> listSale = new List<ReportSalesChannelVM>();

            try
            {
                parameters.Add(new OracleParameter("P_PROVINCE", OracleDbType.Int64, data.NPROVINCE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NLOCAL", OracleDbType.Int64, data.NLOCAL, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NMUNICIPALITY", OracleDbType.Int64, data.NMUNICIPALITY, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NSTATUS", OracleDbType.Int64, data.NSTATUS, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_PREMIUM_INI", OracleDbType.Decimal, data.PREMIUM_INI, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_PREMIUM_FIN", OracleDbType.Decimal, data.PREMIUM_FIN, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DISSUEDAT_INI", OracleDbType.Date, Convert.ToDateTime(data.DISSUEDAT_INI), ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DISSUEDAT_FIN", OracleDbType.Date, Convert.ToDateTime(data.DISSUEDAT_FIN), ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int64, data.NPOLICY, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SCHANNEL_BO", OracleDbType.Varchar2, data.SCHANNEL_BO, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SSALEPOINT_BO", OracleDbType.Varchar2, data.SSALEPOINT_BO, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPAGE", OracleDbType.Int64, data.NPAGE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NRECORD_PAGE", OracleDbType.Int64, data.NRECORD_PAGE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SSALEPOINTLOG", OracleDbType.Varchar2, data.SSALEPOINTLOG, ParameterDirection.Input));
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

            }
            catch (Exception ex)
            {
                //throw;
                listSale = new List<ReportSalesChannelVM>();
                LogControl.save("SaleChannelReport", ex.ToString(), "3");
            }

            return await Task.FromResult(listSale);
        }

        public async Task<List<ReportSalesClientVM>> SaleClientReport(ReportSalesClientBM data)
        {
            List<OracleParameter> parameters = new List<OracleParameter>();
            List<ReportSalesClientVM> listSale = new List<ReportSalesClientVM>();

            try
            {
                parameters.Add(new OracleParameter("P_NESTADO", OracleDbType.Int64, data.P_NESTADO, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NTIPO_TRANSAC", OracleDbType.Int64, data.P_NTIPO_TRANSAC, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPOLIZA", OracleDbType.Int64, data.P_NPOLIZA, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int64, data.P_NPRODUCT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_PRIMA_DESDE", OracleDbType.Decimal, data.P_PRIMA_DESDE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_PRIMA_HASTA", OracleDbType.Decimal, data.P_PRIMA_HASTA, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_TRANS_DESDE", OracleDbType.Date, Convert.ToDateTime(data.P_TRANS_DESDE), ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_TRANS_HASTA", OracleDbType.Date, Convert.ToDateTime(data.P_TRANS_HASTA), ParameterDirection.Input));

                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameters.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(ProcedureName.pkg_ReportesSCTR + ".REA_REPORTE_CLIENT_FINAL", parameters);

                while (odr.Read())
                {
                    ReportSalesClientVM itemSale = new ReportSalesClientVM();
                    itemSale.NUM_COTIZACION = odr["NUM_COTIZACION"].ToString();
                    itemSale.COD_PRODUCTO = odr["COD_PRODUCTO"].ToString();
                    itemSale.DES_PRODUCTO = odr["DES_PRODUCTO"].ToString();
                    itemSale.POLIZA = odr["POLIZA"].ToString();
                    itemSale.TIPO_MOVIMIENTO = odr["TIPO_MOVIMIENTO"].ToString();
                    itemSale.CONSTANCIA = odr["CONSTANCIA"].ToString();
                    itemSale.CANT_ASEGURADOS = odr["CANT_ASEGURADOS"].ToString();
                    itemSale.SCLIENT = odr["SCLIENT"].ToString();
                    itemSale.PRIMA_NETA = odr["PRIMA_NETA"].ToString();
                    itemSale.IGV = odr["IGV"].ToString();
                    itemSale.PRIMA_TOTAL = odr["PRIMA_TOTAL"].ToString();
                    itemSale.FECHA_ENVIO = String.Format("{0:dd/MM/yyyy}", odr["FECHA_ENVIO"]);
                    itemSale.FECHA_EFECTO = String.Format("{0:dd/MM/yyyy}", odr["FECHA_EFECTO"]);
                    itemSale.FECHA_EXPIRACION = String.Format("{0:dd/MM/yyyy}", odr["FECHA_EXPIRACION"]);
                    itemSale.PROFORMA = odr["PROFORMA"].ToString();

                    listSale.Add(itemSale);
                }

                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                listSale = new List<ReportSalesClientVM>();
                LogControl.save("SaleClientReport", ex.ToString(), "3");
            }

            return await Task.FromResult(listSale);
        }

        public async Task<List<ReportSalesEnterpriseVM>> SaleEnterpriseReport(ReportSalesEnterpriseBM data)
        {
            List<OracleParameter> parameters = new List<OracleParameter>();
            List<ReportSalesEnterpriseVM> listSale = new List<ReportSalesEnterpriseVM>();

            try
            {
            }
            catch (Exception ex)
            {
                //throw;
                listSale = new List<ReportSalesEnterpriseVM>();
                LogControl.save("SaleEnterpriseReport", ex.ToString(), "3");
            }

            return await Task.FromResult(listSale);
        }

        public async Task<List<ReportCommissionEnterpriseVM>> CommissionEnterpriseReport(ReportSalesEnterpriseBM data)
        {
            List<OracleParameter> parameters = new List<OracleParameter>();
            List<ReportCommissionEnterpriseVM> listCommission = new List<ReportCommissionEnterpriseVM>();

            try
            {
                parameters.Add(new OracleParameter("P_NESTADO", OracleDbType.Int64, data.P_NESTADO, ParameterDirection.Input));
                //parameters.Add(new OracleParameter("P_NTIPO_TRANSAC", OracleDbType.Int64, data.P_NTIPO_TRANSAC, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPOLIZA", OracleDbType.Int64, data.P_NPOLIZA, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int64, data.P_NPRODUCT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_PRIMA_DESDE", OracleDbType.Decimal, data.P_PRIMA_DESDE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_PRIMA_HASTA", OracleDbType.Decimal, data.P_PRIMA_HASTA, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_TRANS_DESDE", OracleDbType.Date, Convert.ToDateTime(data.P_TRANS_DESDE), ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_TRANS_HASTA", OracleDbType.Date, Convert.ToDateTime(data.P_TRANS_HASTA), ParameterDirection.Input));

                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameters.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(ProcedureName.pkg_ReportesSCTR + ".REA_REPORTE_COMISION", parameters);

                while (odr.Read())
                {
                    ReportCommissionEnterpriseVM itemCommission = new ReportCommissionEnterpriseVM();
                    itemCommission.NUM_COTIZACION = odr["NUM_COTIZACION"].ToString();
                    itemCommission.COD_PRODUCTO = odr["COD_PRODUCTO"].ToString();
                    itemCommission.DES_PRODUCTO = odr["DES_PRODUCTO"].ToString();
                    itemCommission.POLIZA = odr["POLIZA"].ToString();
                    itemCommission.CANT_ASEGURADOS = odr["CANT_ASEGURADOS"].ToString();
                    itemCommission.CODIGO_INTERMEDIARIO = odr["CODIGO_INTERMEDIARIO"].ToString();
                    itemCommission.TIPO_COMERCIALIZADOR = odr["TIPO_COMERCIALIZADOR"].ToString();
                    itemCommission.COMERCIALIZADOR = odr["COMERCIALIZADOR"].ToString();
                    itemCommission.PORCENTAJE_COMISION = odr["PORCENTAJE_COMISION"].ToString();
                    itemCommission.PRIMA_EMISION = odr["PRIMA_EMISION"].ToString();
                    itemCommission.ESTADO_PAGO = odr["ESTADO_PAGO"].ToString();

                    listCommission.Add(itemCommission);
                }

                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                listCommission = new List<ReportCommissionEnterpriseVM>();
                LogControl.save("CommissionEnterpriseReport", ex.ToString(), "3");
            }

            return await Task.FromResult(listCommission);

        }

        public async Task<List<ReportCommissionChannelVM>> CommissionChannelReport(ReportCommissionChannelBM data)
        {
            List<OracleParameter> parameters = new List<OracleParameter>();
            List<ReportCommissionChannelVM> listSale = new List<ReportCommissionChannelVM>();

            try
            {
                //parameters.Add(new OracleParameter("P_PROVINCE", OracleDbType.Int64, data.NPROVINCE, ParameterDirection.Input));
                //parameters.Add(new OracleParameter("P_NLOCAL", OracleDbType.Int64, data.NLOCAL, ParameterDirection.Input));
                //parameters.Add(new OracleParameter("P_NMUNICIPALITY", OracleDbType.Int64, data.NMUNICIPALITY, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NSTATUS", OracleDbType.Int64, data.NSTATUS, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_PREMIUM_INI", OracleDbType.Decimal, data.PREMIUM_INI, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_PREMIUM_FIN", OracleDbType.Decimal, data.PREMIUM_FIN, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DISSUEDAT_INI", OracleDbType.Date, Convert.ToDateTime(data.DISSUEDAT_INI), ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DISSUEDAT_FIN", OracleDbType.Date, Convert.ToDateTime(data.DISSUEDAT_FIN), ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int64, data.NPOLICY, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPAGE", OracleDbType.Int64, data.NPAGE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NRECORD_PAGE", OracleDbType.Int64, data.NRECORD_PAGE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
            }
            catch (Exception ex)
            {
                //throw;
                listSale = new List<ReportCommissionChannelVM>();
                LogControl.save("CommissionChannelReport", ex.ToString(), "3");
            }

            return await Task.FromResult(listSale);

        }

        public async Task<List<ReportSaleRecordVM>> SaleRecordReport(ReportSaleRecordBM data)
        {
            List<OracleParameter> parameters = new List<OracleParameter>();
            List<ReportSaleRecordVM> listSale = new List<ReportSaleRecordVM>();

            try
            {
                parameters.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int64, 20, data.NPOLICY, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SSTATUS_POL", OracleDbType.Varchar2, data.SSTATUS_POL, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCODEMODE", OracleDbType.Decimal, data.SSALEMODE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DISSUEDAT_INI", OracleDbType.Date, Convert.ToDateTime(data.DSTARTDATE), ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DREGDATEINI", OracleDbType.Date, Convert.ToDateTime(data.DEXPIRDAT), ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SCHANNEL_BO", OracleDbType.Varchar2, data.P_SCHANNEL_BO, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SSALEPOINT_BO", OracleDbType.Varchar2, data.P_SSALEPOINT_BO, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPAGE", OracleDbType.Int64, data.NPAGE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NRECORD_PAGE", OracleDbType.Int64, data.NRECORD_PAGE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
            }
            catch (Exception ex)
            {
                //throw;
                listSale = new List<ReportSaleRecordVM>();
                LogControl.save("SaleRecordReport", ex.ToString(), "3");
            }

            return await Task.FromResult(listSale);

        }

        //INI JF

        //Ramo
        public List<ReportBranProc> BranchPremiumReport(string P_TIPO)

        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<ReportBranProc> listBranch = new List<ReportBranProc>();

            string storedProcedureName = ProcedureName.pkg_TableroControl + ".SPS_LIST_BRANCH_REP";

            try
            {
                //Parámetros de Entrada
                parameter.Add(new OracleParameter("P_TIPO", OracleDbType.Char, P_TIPO, ParameterDirection.Input));
                //Parámetros de Salida
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);
                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        ReportBranProc item = new ReportBranProc();
                        item.NBRANCH = odr["NBRANCH"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NBRANCH"].ToString());
                        item.SDESCRIPT = odr["SDESCRIPT"] == DBNull.Value ? string.Empty : odr["SDESCRIPT"].ToString();
                        listBranch.Add(item);
                    }
                }

                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                LogControl.save("BranchPremiumReport", ex.ToString(), "3");
            }

            return listBranch;
        }

        //Producto
        public List<ReportProdProc> ProductPremiumReport(string P_TIPO, int P_NBRANCH)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<ReportProdProc> listProducts = new List<ReportProdProc>();

            string storedProcedureName = ProcedureName.pkg_TableroControl + ".SPS_LIST_PRODUCT_REP";

            try
            {
                //Parámetros de Entrada
                parameter.Add(new OracleParameter("P_TIPO", OracleDbType.Char, P_TIPO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, P_NBRANCH, ParameterDirection.Input));
                //Parámetros de Salida
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);

                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);
                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        ReportProdProc item = new ReportProdProc();
                        item.NPRODUCT = odr["NPRODUCT"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NPRODUCT"].ToString());
                        item.SDESCRIPT = odr["SDESCRIPT"] == DBNull.Value ? string.Empty : odr["SDESCRIPT"].ToString();
                        listProducts.Add(item);
                    }
                }

                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return listProducts;

        }

        //Estado
        public List<ReportStatusProc> StatusPremiumReport()
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<ReportStatusProc> listStatus = new List<ReportStatusProc>();

            string storedProcedureName = ProcedureName.pkg_TableroControl + ".SPS_LIST_STATUS_PROC";

            try
            {
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);
                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        ReportStatusProc item = new ReportStatusProc();
                        item.NUSERCODE = odr["NUSERCODE"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NUSERCODE"].ToString());
                        item.NSTATUSPROC = odr["NSTATUSPROC"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NSTATUSPROC"].ToString());
                        item.SDESCRIPTION = odr["SDESCRIPTION"] == DBNull.Value ? string.Empty : odr["SDESCRIPTION"].ToString();
                        item.DCOMPDATE = String.Format("{0:dd/MM/yyyy}", odr["DCOMPDATE"]);
                        listStatus.Add(item);
                    }
                }

                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return listStatus;

        }

        //Listar Reportes
        public List<ReportListProc> GetListPremiumReport(ReportListProc data)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<ReportListProc> ListReport = new List<ReportListProc>();

            string storedProcedureName = ProcedureName.pkg_TableroControl + ".SPS_LIST_REPORTES";

            try
            {
                //Parámetros de entrada
                parameter.Add(new OracleParameter("P_DCOMPDATE_INI", OracleDbType.Varchar2, data.fecInicio, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DCOMPDATE_FIN", OracleDbType.Varchar2, data.fecFin, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, data.codProducto, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NSTATUSPROC", OracleDbType.Int32, data.codEstado, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.codRamo, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_IDPROCESS", OracleDbType.Varchar2, data.codReporte, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TI_BUSQUEDA", OracleDbType.Varchar2, data.codTipoBus, ParameterDirection.Input));


                //Parámetros de Salida
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);

                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;

                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        //Parámetros del C_TABLE
                        ReportListProc itemProc = new ReportListProc();
                        itemProc.id = odr["ID"] == DBNull.Value ? string.Empty : odr["ID"].ToString();
                        itemProc.codUsuario = Int32.Parse(odr["NUSERCODE"] == DBNull.Value ? string.Empty : odr["NUSERCODE"].ToString());
                        itemProc.desUsuario = odr["DESUSUARIO"] == DBNull.Value ? string.Empty : odr["DESUSUARIO"].ToString();
                        itemProc.fecInicio = odr["DINIREP"] == DBNull.Value ? string.Empty : odr["DINIREP"].ToString();
                        itemProc.fecFin = odr["DFINREP"] == DBNull.Value ? string.Empty : odr["DFINREP"].ToString();
                        itemProc.codProducto = Int32.Parse(odr["NPRODUCT"] == DBNull.Value ? string.Empty : odr["NPRODUCT"].ToString());
                        itemProc.desProducto = odr["DESPRODUCTO"] == DBNull.Value ? string.Empty : odr["DESPRODUCTO"].ToString();
                        itemProc.codEstado = Int32.Parse(odr["NSTATUSPROC"] == DBNull.Value ? string.Empty : odr["NSTATUSPROC"].ToString());
                        itemProc.desEstado = odr["DESESTADO"] == DBNull.Value ? string.Empty : odr["DESESTADO"].ToString();
                        itemProc.fecha = odr["DCOMPDATE"] == DBNull.Value ? string.Empty : odr["DCOMPDATE"].ToString();
                        itemProc.desRamo = odr["DESRAMO"] == DBNull.Value ? string.Empty : odr["DESRAMO"].ToString();
                        itemProc.codTipo = odr["TIPODESCARGA"] == DBNull.Value ? string.Empty : odr["TIPODESCARGA"].ToString();

                        ListReport.Add(itemProc);
                    }
                }

                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ListReport;
        }

        //Procesar - Insertar
        public ResponseControl InsertProcessPremiumReport(ReportProcess data)
        {
            ReportProcess Rpt = (ReportProcess)data;

            if (Rpt.codRamo != 998)
            {
                Rpt.idProcesoOr = null;
            }

            ResponseControl generic = new ResponseControl(Response.Ok);

            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = cn;
                    cmd.CommandText = string.Format("{0}.{1}", ProcedureName.pkg_TableroControl, "SPS_INS_CAB_REPORT_PROCESS");
                    cmd.CommandType = CommandType.StoredProcedure;
                    try
                    {
                        cmd.Parameters.Add("P_ID", OracleDbType.Varchar2, Rpt.idProceso, ParameterDirection.Input);
                        cmd.Parameters.Add("P_TIPO", OracleDbType.Varchar2, Rpt.codTipo, ParameterDirection.Input);
                        cmd.Parameters.Add("P_NUSERCODE", OracleDbType.Int32, Rpt.codUsuario, ParameterDirection.Input);
                        cmd.Parameters.Add("P_DINIREP", OracleDbType.Date, Convert.ToDateTime(Rpt.fecInicio), ParameterDirection.Input);
                        cmd.Parameters.Add("P_DFINREP", OracleDbType.Date, Convert.ToDateTime(Rpt.fecFin), ParameterDirection.Input);
                        cmd.Parameters.Add("P_NPRODUCT", OracleDbType.Int32, Rpt.codProducto, ParameterDirection.Input);
                        cmd.Parameters.Add("P_NBRANCH", OracleDbType.Int32, Rpt.codRamo, ParameterDirection.Input);
                        cmd.Parameters.Add("P_ID_PROCESS_OR", OracleDbType.Varchar2, Rpt.idProcesoOr, ParameterDirection.Input);
                        //Ncode controla los errores
                        var NCode = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);
                        var SMessage = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                        NCode.Size = 4000;
                        SMessage.Size = 4000;
                        cmd.Parameters.Add(NCode);
                        cmd.Parameters.Add(SMessage);
                        cn.Open();

                        cmd.ExecuteNonQuery();
                        generic.Data = Rpt.idProceso;
                        var PCode = Convert.ToInt32(NCode.Value.ToString());
                        var PMessage = (SMessage.Value.ToString());

                        if (PCode == 1)
                        {
                            generic.Message = SMessage.Value.ToString();
                            generic.response = Response.Fail;
                        }

                        ELog.CloseConnection(cn);
                        cmd.Dispose();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(generic.Message.ToString());
                    }
                }
            }
            return generic;
        }

        //Tipo de Comprobante
        public List<ReportBillTypeProf> BillTypeRequestProforma()
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<ReportBillTypeProf> listBillType = new List<ReportBillTypeProf>();

            string storedProcedureName = ProcedureName.pkg_TableroControlConsultas + ".SPS_SEL_TIPO_COMPROBANTE";

            try
            {
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);
                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        ReportBillTypeProf item = new ReportBillTypeProf();
                        item.SBILLTYPE = odr["SBILLTYPE"] == DBNull.Value ? 0 : Convert.ToInt32(odr["SBILLTYPE"].ToString());
                        item.SDESCRIPT = odr["SDESCRIPT"] == DBNull.Value ? string.Empty : odr["SDESCRIPT"].ToString();
                        listBillType.Add(item);
                    }
                }

                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return listBillType;

        }

        //Numero de Serie
        public List<ReportSerieProf> SerieNumberRequestProforma()
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<ReportSerieProf> listSeries = new List<ReportSerieProf>();

            string storedProcedureName = ProcedureName.pkg_TableroControlConsultas + ".SPS_SEL_SERIE";

            try
            {
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);
                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        ReportSerieProf item = new ReportSerieProf();
                        item.SERIE_COMPROBANTE = odr["SERIE_COMPROBANTE"] == DBNull.Value ? string.Empty : odr["SERIE_COMPROBANTE"].ToString();
                        listSeries.Add(item);
                    }
                }

                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return listSeries;

        }

        //Tipo de Documento
        public List<ReportTypeDocProf> DocumentTypeRequestProforma()
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<ReportTypeDocProf> listDocumentType = new List<ReportTypeDocProf>();

            string storedProcedureName = ProcedureName.pkg_TableroControlConsultas + ".SPS_SEL_TIPO_DOCUMENTO";

            try
            {
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);
                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        ReportTypeDocProf item = new ReportTypeDocProf();
                        item.NIDDOC_TYPE = odr["NIDDOC_TYPE"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NIDDOC_TYPE"].ToString());
                        item.SDESCRIPT = odr["SDESCRIPT"] == DBNull.Value ? string.Empty : odr["SDESCRIPT"].ToString();
                        listDocumentType.Add(item);
                    }
                }

                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return listDocumentType;

        }

        //Listar Proformas
        public ResponseReportProf GetListRequestProforma(ReportProfBM data)
        {
            ResponseReportProf result = new ResponseReportProf();
            List<ReportListProf> profList = new List<ReportListProf>();

            string storedProcedureName = ProcedureName.pkg_TableroControlConsultas + ".SPS_LIST_PROFORMAS";

            try
            {
                List<OracleParameter> parameter = new List<OracleParameter>();
                //Parámetros de entrada
                parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int32, data.idPolicy, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDDOC_TYPE", OracleDbType.Int32, data.idDocType, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, data.idDocument, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DBILLDATE_INI", OracleDbType.Varchar2, data.startDate, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DBILLDATE_FIN", OracleDbType.Varchar2, data.endDate, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SBILLTYPE", OracleDbType.Char, data.idBillType, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_MINSUAREA", OracleDbType.Int32, data.idSerieNumber, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBILLNUM", OracleDbType.Varchar2, data.idBill, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TI_BUSQUEDA", OracleDbType.Varchar2, data.searchType, ParameterDirection.Input));
                //Parámetros de Salida
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);

                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;

                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);
                parameter.Add(C_TABLE);

                /* PREFILES -DGC - 30/04/2024 */
                //parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Varchar2, data.usercode, ParameterDirection.Input));

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        //Parámetros del C_TABLE
                        ReportListProf itemProf = new ReportListProf();
                        itemProf.billType = odr["TIPO_COMPROBANTE"] == null ? string.Empty : odr["TIPO_COMPROBANTE"].ToString();
                        itemProf.serieNumber = odr["SERIE_COMPROBANTE"] == null ? string.Empty : odr["SERIE_COMPROBANTE"].ToString();
                        itemProf.billNumber = odr["NRO_COMPROBANTE"] == null ? string.Empty : odr["NRO_COMPROBANTE"].ToString();
                        itemProf.proforma = odr["PROFORMA"] == null ? string.Empty : odr["PROFORMA"].ToString();
                        itemProf.requestType = odr["TIPO_SOLICITUD"] == null ? string.Empty : odr["TIPO_SOLICITUD"].ToString();
                        itemProf.purePremium = Decimal.Parse(odr["PRIMA_NETA"] == null ? string.Empty : odr["PRIMA_NETA"].ToString());
                        itemProf.igv = Decimal.Parse(odr["IGV"] == null ? string.Empty : odr["IGV"].ToString());
                        itemProf.rightIssue = Decimal.Parse(odr["DERECHO_EMISION"] == null ? string.Empty : odr["DERECHO_EMISION"].ToString());
                        itemProf.grossPremium = Decimal.Parse(odr["PRIMA_BRUTA"] == null ? string.Empty : odr["PRIMA_BRUTA"].ToString());
                        itemProf.billDate = odr["FECHA_FACTURA"] == null ? string.Empty : odr["FECHA_FACTURA"].ToString();
                        itemProf.scertype = odr["SCERTYPE"] == null ? string.Empty : odr["SCERTYPE"].ToString();
                        itemProf.branch = Int32.Parse(odr["RAMO"] == null ? string.Empty : odr["RAMO"].ToString());
                        itemProf.product = Int32.Parse(odr["PRODUCTO"] == null ? string.Empty : odr["PRODUCTO"].ToString());
                        itemProf.policy = Int32.Parse(odr["POLIZA"] == null ? string.Empty : odr["POLIZA"].ToString());
                        itemProf.startReceipt = odr["FEC_EFECTO"] == null ? string.Empty : odr["FEC_EFECTO"].ToString();
                        itemProf.endReceipt = odr["FEC_EXPIR"] == null ? string.Empty : odr["FEC_EXPIR"].ToString();
                        itemProf.docType = odr["TIPO_DOC_CONT"] == null ? string.Empty : odr["TIPO_DOC_CONT"].ToString();
                        itemProf.docNumber = odr["NRO_DOC_CONT"] == null ? string.Empty : odr["NRO_DOC_CONT"].ToString();
                        itemProf.clientName = odr["NOMBRE_CONT"] == null ? string.Empty : odr["NOMBRE_CONT"].ToString();
                        itemProf.ciu = odr["CIU"] == null ? string.Empty : odr["CIU"].ToString();
                        itemProf.activity = odr["ACTIVIDAD"] == null ? string.Empty : odr["ACTIVIDAD"].ToString();
                        profList.Add(itemProf);
                    }
                }
                ELog.CloseConnection(odr);

                result.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                result.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                result.listProf = profList;

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        //Listar Asegurados
        public ResponseReportInsu GetListRequestInsured(ReportInsuBM data)
        {
            ResponseReportInsu result = new ResponseReportInsu();
            List<ReportListInsu> insuredList = new List<ReportListInsu>();

            string storedProcedureName = ProcedureName.pkg_TableroControlConsultas + ".SPS_LIST_ASEGURADOS";

            try
            {
                List<OracleParameter> parameter = new List<OracleParameter>();
                //Parámetros de entrada
                parameter.Add(new OracleParameter("P_SCERTYPE", OracleDbType.Varchar2, data.scertype, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.branch, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, data.product, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int32, data.policy, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SPROFORMA", OracleDbType.Varchar2, data.proforma, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TIP_COMPROBANTE", OracleDbType.Varchar2, data.billType, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NRO_COMPROBANTE", OracleDbType.Varchar2, data.billNumber, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SERIE_COMPROBANTE", OracleDbType.Varchar2, data.serieNumber, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRIMA_NETA", OracleDbType.Decimal, data.purePremium, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NDERECHO_EMISION", OracleDbType.Decimal, data.rightIssue, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIGV", OracleDbType.Decimal, data.igv, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TIP_SOLICITUD", OracleDbType.Varchar2, data.requestType, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FECHA_FACTURA", OracleDbType.Varchar2, data.billDate, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FECHA_INI_RECIBO", OracleDbType.Varchar2, data.startReceipt, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FECHA_FIN_RECIBO", OracleDbType.Varchar2, data.endReceipt, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TIPO_DOC_CONT", OracleDbType.Varchar2, data.docType, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUM_DOC_CONT", OracleDbType.Varchar2, data.docNumber, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_CONTRATANTE", OracleDbType.Varchar2, data.clientName, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_CIU", OracleDbType.Varchar2, data.ciu, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_ACTIVIDAD", OracleDbType.Varchar2, data.activity, ParameterDirection.Input));

                //Parámetros de Salida
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);

                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;

                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        //Parámetros del C_TABLE
                        ReportListInsu itemInsured = new ReportListInsu();
                        itemInsured.policy = Int32.Parse(odr["POLIZA"] == null ? string.Empty : odr["POLIZA"].ToString());
                        itemInsured.proforma = odr["PROFORMA"] == null ? string.Empty : odr["PROFORMA"].ToString();
                        itemInsured.certificatNumber = Int32.Parse(odr["NRO_CERTIFICADO"] == null ? string.Empty : odr["NRO_CERTIFICADO"].ToString());
                        itemInsured.startDateValidity = odr["INICIO_VIGENCIA_CERTIFICADO"] == null ? string.Empty : odr["INICIO_VIGENCIA_CERTIFICADO"].ToString();
                        itemInsured.endDateValidity = odr["FIN_VIGENCIA_CERTIFICADO"] == null ? string.Empty : odr["FIN_VIGENCIA_CERTIFICADO"].ToString();
                        itemInsured.statusCertificat = odr["ESTADO_CERTIFICADO"] == null ? string.Empty : odr["ESTADO_CERTIFICADO"].ToString();
                        itemInsured.documentType = odr["TIPO_DOCUMENTO_ASEGURADO"] == null ? string.Empty : odr["TIPO_DOCUMENTO_ASEGURADO"].ToString();
                        itemInsured.IdDocument = odr["NUMERO_DOCUMENTO_ASEGURADO"] == null ? string.Empty : odr["NUMERO_DOCUMENTO_ASEGURADO"].ToString();
                        itemInsured.insured = odr["ASEGURADO"] == null ? string.Empty : odr["ASEGURADO"].ToString();
                        itemInsured.payroll = odr["PLANILLA"] == null ? string.Empty : odr["PLANILLA"].ToString();
                        itemInsured.purePremium = Decimal.Parse(odr["PRIMA_NETA"] == null ? string.Empty : (odr["PRIMA_NETA"].ToString()));
                        itemInsured.rightIssue = Decimal.Parse(odr["DE_EMISION"] == null ? string.Empty : (odr["DE_EMISION"].ToString()));
                        itemInsured.grossPremium = Decimal.Parse(odr["PRIMA_BRUTA"] == null ? string.Empty : (odr["PRIMA_BRUTA"].ToString()));
                        itemInsured.kindRisk = odr["TIPO_RIESGO"] == null ? string.Empty : odr["TIPO_RIESGO"].ToString();
                        itemInsured.rateRisk = Decimal.Parse(odr["TASA_RIESGO"] == null ? string.Empty : (odr["TASA_RIESGO"].ToString()));
                        itemInsured.igv = Decimal.Parse(odr["IGV"] == null ? string.Empty : (odr["IGV"].ToString()));
                        itemInsured.billType = odr["TIPO_COMPROBANTE"] == null ? string.Empty : odr["TIPO_COMPROBANTE"].ToString();
                        itemInsured.billNumber = odr["NRO_COMPROBANTE"] == null ? string.Empty : odr["NRO_COMPROBANTE"].ToString();
                        itemInsured.serieNumber = odr["SERIE"] == null ? string.Empty : odr["SERIE"].ToString();
                        itemInsured.requestType = odr["TIPO_SOLICITUD"] == null ? string.Empty : odr["TIPO_SOLICITUD"].ToString();
                        itemInsured.billDate = odr["FECHA_FACTURA"] == null ? string.Empty : odr["FECHA_FACTURA"].ToString();
                        itemInsured.startReceipt = odr["FECHA_INI_RECIBO"] == null ? string.Empty : odr["FECHA_INI_RECIBO"].ToString();
                        itemInsured.endReceipt = odr["FECHA_FIN_RECIBO"] == null ? string.Empty : odr["FECHA_FIN_RECIBO"].ToString();
                        itemInsured.nullDate = odr["FECHA_ANULACION"] == null ? string.Empty : odr["FECHA_ANULACION"].ToString();
                        itemInsured.docType = odr["TIPO_DOC_CONT"] == null ? string.Empty : odr["TIPO_DOC_CONT"].ToString();
                        itemInsured.docNumber = odr["NUM_DOC_CONT"] == null ? string.Empty : odr["NUM_DOC_CONT"].ToString();
                        itemInsured.clientName = odr["CONTRATANTE"] == null ? string.Empty : odr["CONTRATANTE"].ToString();
                        itemInsured.ciu = odr["CIU"] == null ? string.Empty : odr["CIU"].ToString();
                        itemInsured.activity = odr["ACTIVIDAD"] == null ? string.Empty : odr["ACTIVIDAD"].ToString();


                        insuredList.Add(itemInsured);
                    }
                }
                ELog.CloseConnection(odr);

                result.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                result.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                result.listInsu = insuredList;

            }
            catch (Exception ex)
            {
                if (result.P_SMESSAGE == null || result.listInsu == null)
                {
                    result.P_SMESSAGE = "Lamentamos el inconveniente. Porfavor contacte a soporte de TI";
                }
            }

            return result;
        }

        //Listar Cruce de Interfaces
        public ResponseReportInter GetInterfacesCrossing(ReportInterfBM data)
        {
            ResponseReportInter result = new ResponseReportInter();

            var ValType = data.searchType;

            if (data.searchType == "0" || data.searchType == "4" || data.searchType == "5" || data.searchType == "6" || data.searchType == "7"
                || data.searchType == "8" || data.searchType == "9" || data.searchType == "14")
            {
                data.searchType = "0";

                List<ReportInterfComm> interfaceListComm = new List<ReportInterfComm>();

                string storedProcedureName = ProcedureName.pkg_TableroControlConsultas + ".SPS_LIST_CRUCE_INTERFACE";

                try
                {
                    List<OracleParameter> parameter = new List<OracleParameter>();
                    //Parámetros de entrada
                    parameter.Add(new OracleParameter("P_DINTERFACEDATE_INI", OracleDbType.Varchar2, data.startDate, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_DINTERFACEDATE_FIN", OracleDbType.Varchar2, data.endDate, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_STI_BUSQUEDA", OracleDbType.Varchar2, data.searchType, ParameterDirection.Input));
                    //Parámetros de Salida
                    OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);
                    OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                    OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);

                    P_NCODE.Size = 4000;
                    P_SMESSAGE.Size = 4000;
                    parameter.Add(P_NCODE);
                    parameter.Add(P_SMESSAGE);
                    parameter.Add(C_TABLE);

                    OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                    if (odr.HasRows)
                    {
                        while (odr.Read())
                        {
                            //Parámetros del C_TABLE
                            ReportInterfComm itemInterfaceComm = new ReportInterfComm();
                            itemInterfaceComm.preliminary = odr["PRELIMINAR"] == null ? string.Empty : odr["PRELIMINAR"].ToString();
                            itemInterfaceComm.receipt = odr["RECIBO"] == null ? string.Empty : odr["RECIBO"].ToString();
                            itemInterfaceComm.branch = odr["RAMO"] == null ? string.Empty : odr["RAMO"].ToString();
                            itemInterfaceComm.styp_comm = odr["DESCRIP_TIP_COMISION"] == null ? string.Empty : odr["DESCRIP_TIP_COMISION"].ToString();
                            itemInterfaceComm.product = odr["PRODUCTO"] == null ? string.Empty : odr["PRODUCTO"].ToString();
                            itemInterfaceComm.branch_led = odr["RAMO_CONTABLE"] == null ? string.Empty : odr["RAMO_CONTABLE"].ToString();
                            itemInterfaceComm.currency = odr["MONEDA"] == null ? string.Empty : odr["MONEDA"].ToString();
                            itemInterfaceComm.total_com = Decimal.Parse(odr["TOTAL_COMISION"] == null ? string.Empty : odr["TOTAL_COMISION"].ToString());
                            itemInterfaceComm.deffecdate = odr["FECHA_REGISTRO"] == null ? string.Empty : odr["FECHA_REGISTRO"].ToString();
                            itemInterfaceComm.intermed = odr["INTERMEDIARIO"] == null ? string.Empty : odr["INTERMEDIARIO"].ToString();
                            itemInterfaceComm.policy = odr["POLIZA"] == null ? string.Empty : odr["POLIZA"].ToString();
                            itemInterfaceComm.intertyp = odr["TIP_INTERMEDIARIO"] == null ? string.Empty : odr["TIP_INTERMEDIARIO"].ToString();
                            itemInterfaceComm.commnew = Decimal.Parse(odr["NCOMMNEW"] == null ? string.Empty : odr["NCOMMNEW"].ToString());
                            itemInterfaceComm.billtype = odr["TIP_COMPROBANTE"] == null ? string.Empty : odr["TIP_COMPROBANTE"].ToString();
                            itemInterfaceComm.insur_area = odr["SERIE_COMPROBANTE"] == null ? string.Empty : odr["SERIE_COMPROBANTE"].ToString();
                            itemInterfaceComm.billnum = odr["NRO_COMPROBANTE"] == null ? string.Empty : odr["NRO_COMPROBANTE"].ToString();
                            interfaceListComm.Add(itemInterfaceComm);
                        }
                    }
                    ELog.CloseConnection(odr);

                    result.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                    result.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                    result.listInterComm = interfaceListComm;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                ValType = (data.searchType == "0") ? data.searchType = ValType : data.searchType;

            }

            if (data.searchType == "1" || data.searchType == "4" || data.searchType == "7" || data.searchType == "8" || data.searchType == "9"
                || data.searchType == "10" || data.searchType == "11" || data.searchType == "12")
            {
                data.searchType = "1";

                List<ReportInterAdvisory> interfaceListAdv = new List<ReportInterAdvisory>();

                string storedProcedureName = ProcedureName.pkg_TableroControlConsultas + ".SPS_LIST_CRUCE_INTERFACE";

                try
                {
                    List<OracleParameter> parameter = new List<OracleParameter>();
                    //Parámetros de entrada
                    parameter.Add(new OracleParameter("P_DINTERFACEDATE_INI", OracleDbType.Varchar2, data.startDate, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_DINTERFACEDATE_FIN", OracleDbType.Varchar2, data.endDate, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_STI_BUSQUEDA", OracleDbType.Varchar2, data.searchType, ParameterDirection.Input));
                    //Parámetros de Salida
                    OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);
                    OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                    OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);

                    P_NCODE.Size = 4000;
                    P_SMESSAGE.Size = 4000;
                    parameter.Add(P_NCODE);
                    parameter.Add(P_SMESSAGE);
                    parameter.Add(C_TABLE);

                    OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                    if (odr.HasRows)
                    {
                        while (odr.Read())
                        {
                            //Parámetros del C_TABLE
                            ReportInterAdvisory itemInterfaceAdv = new ReportInterAdvisory();
                            itemInterfaceAdv.preliminary = odr["PRELIMINAR"] == null ? string.Empty : odr["PRELIMINAR"].ToString();
                            itemInterfaceAdv.branch = odr["RAMO"] == null ? string.Empty : odr["RAMO"].ToString();
                            itemInterfaceAdv.product = odr["PRODUCTO"] == null ? string.Empty : odr["PRODUCTO"].ToString();
                            itemInterfaceAdv.policy = odr["POLIZA"] == null ? string.Empty : odr["POLIZA"].ToString();
                            itemInterfaceAdv.branchLed = odr["RAMO_CONTABLE"] == null ? string.Empty : odr["RAMO_CONTABLE"].ToString();
                            itemInterfaceAdv.currency = odr["MONEDA"] == null ? string.Empty : odr["MONEDA"].ToString();
                            itemInterfaceAdv.totalComm = Decimal.Parse(odr["NTOTAL_COM"] == null ? string.Empty : odr["NTOTAL_COM"].ToString());
                            itemInterfaceAdv.deffecDate = odr["FECHA_EFECTO"] == null ? string.Empty : odr["FECHA_EFECTO"].ToString();
                            itemInterfaceAdv.ntype = odr["NTYPE"] == null ? string.Empty : odr["NTYPE"].ToString();
                            itemInterfaceAdv.commType = odr["TIPO_COMISION"] == null ? string.Empty : odr["TIPO_COMISION"].ToString();
                            itemInterfaceAdv.intermed = odr["INTERMEDIARIO"] == null ? string.Empty : odr["INTERMEDIARIO"].ToString();
                            itemInterfaceAdv.intertyp = odr["TIP_INTERMEDIARIO"] == null ? string.Empty : odr["TIP_INTERMEDIARIO"].ToString();
                            itemInterfaceAdv.bankCode = odr["NBANK_CODE"] == null ? string.Empty : odr["NBANK_CODE"].ToString();
                            itemInterfaceAdv.billType = odr["TIP_COMPROBANTE"] == null ? string.Empty : odr["TIP_COMPROBANTE"].ToString();
                            itemInterfaceAdv.insurArea = odr["SERIE_COMPROBANTE"] == null ? string.Empty : odr["SERIE_COMPROBANTE"].ToString();
                            itemInterfaceAdv.billNum = odr["NRO_COMPROBANTE"] == null ? string.Empty : odr["NRO_COMPROBANTE"].ToString();
                            itemInterfaceAdv.operBank = odr["NOPERACION_BANCO"] == null ? string.Empty : odr["NOPERACION_BANCO"].ToString();
                            itemInterfaceAdv.type = odr["TIPO"] == null ? string.Empty : odr["TIPO"].ToString();
                            interfaceListAdv.Add(itemInterfaceAdv);
                        }
                    }
                    ELog.CloseConnection(odr);

                    result.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                    result.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                    result.listInterAdv = interfaceListAdv;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                ValType = (data.searchType == "1") ? data.searchType = ValType : data.searchType;

            }

            if (data.searchType == "2" || data.searchType == "5" || data.searchType == "7" || data.searchType == "9" || data.searchType == "10"
                || data.searchType == "12" || data.searchType == "13" || data.searchType == "14")
            {
                data.searchType = "2";

                List<ReportInterColl> interfaceListColl = new List<ReportInterColl>();

                string storedProcedureName = ProcedureName.pkg_TableroControlConsultas + ".SPS_LIST_CRUCE_INTERFACE";

                try
                {
                    List<OracleParameter> parameter = new List<OracleParameter>();
                    //Parámetros de entrada
                    parameter.Add(new OracleParameter("P_DINTERFACEDATE_INI", OracleDbType.Varchar2, data.startDate, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_DINTERFACEDATE_FIN", OracleDbType.Varchar2, data.endDate, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_STI_BUSQUEDA", OracleDbType.Varchar2, data.searchType, ParameterDirection.Input));
                    //Parámetros de Salida
                    OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);
                    OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                    OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);

                    P_NCODE.Size = 4000;
                    P_SMESSAGE.Size = 4000;
                    parameter.Add(P_NCODE);
                    parameter.Add(P_SMESSAGE);
                    parameter.Add(C_TABLE);

                    OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                    if (odr.HasRows)
                    {
                        while (odr.Read())
                        {
                            //Parámetros del C_TABLE
                            ReportInterColl itemInterfaceColl = new ReportInterColl();
                            itemInterfaceColl.preliminary = odr["PRELIMINAR"] == null ? string.Empty : odr["PRELIMINAR"].ToString();
                            itemInterfaceColl.auxiliarAccount = odr["CODIGO_AUXILIAR"] == null ? string.Empty : odr["CODIGO_AUXILIAR"].ToString();
                            itemInterfaceColl.transaction = odr["NUMERO_TRANSACCION"] == null ? string.Empty : odr["NUMERO_TRANSACCION"].ToString();
                            itemInterfaceColl.type = odr["TIPO_MOVIMIENTO"] == null ? string.Empty : odr["TIPO_MOVIMIENTO"].ToString();
                            itemInterfaceColl.receipt = odr["RECIBO"] == null ? string.Empty : odr["RECIBO"].ToString();
                            itemInterfaceColl.billNum = odr["NUMERO_FACTURA"] == null ? string.Empty : odr["NUMERO_FACTURA"].ToString();
                            itemInterfaceColl.branch = odr["RAMO"] == null ? string.Empty : odr["RAMO"].ToString();
                            itemInterfaceColl.scertype = odr["TIPO_REGISTROS"] == null ? string.Empty : odr["TIPO_REGISTROS"].ToString();
                            itemInterfaceColl.product = odr["PRODUCTO"] == null ? string.Empty : odr["PRODUCTO"].ToString();
                            itemInterfaceColl.policy = odr["POLIZA"] == null ? string.Empty : odr["POLIZA"].ToString();
                            itemInterfaceColl.stype = odr["DESCRIP_TIP_MOVIMIENTO"] == null ? string.Empty : odr["DESCRIP_TIP_MOVIMIENTO"].ToString();
                            itemInterfaceColl.serie = odr["SERIE_COMPROBANTE"] == null ? string.Empty : odr["SERIE_COMPROBANTE"].ToString();
                            itemInterfaceColl.cardNumber = odr["VOUCHER"] == null ? string.Empty : odr["VOUCHER"].ToString();
                            itemInterfaceColl.cardType = odr["CODIGO_TIPO_PAGO"] == null ? string.Empty : odr["CODIGO_TIPO_PAGO"].ToString();
                            itemInterfaceColl.codeBank = odr["CODIGO_BANCO"] == null ? string.Empty : odr["CODIGO_BANCO"].ToString();
                            itemInterfaceColl.fecCont = odr["FECHA_CONTABILIZACION"] == null ? string.Empty : odr["FECHA_CONTABILIZACION"].ToString();
                            itemInterfaceColl.sclient = odr["COD_CLIENTE"] == null ? string.Empty : odr["COD_CLIENTE"].ToString();
                            itemInterfaceColl.currency = odr["MONEDA"] == null ? string.Empty : odr["MONEDA"].ToString();
                            itemInterfaceColl.branchLed = odr["RAMO_CONTABLE"] == null ? string.Empty : odr["RAMO_CONTABLE"].ToString();
                            itemInterfaceColl.operBank = odr["NOPERACION_BANCO"] == null ? string.Empty : odr["NOPERACION_BANCO"].ToString();
                            itemInterfaceColl.billType = odr["TIP_COMPROBANTE"] == null ? string.Empty : odr["TIP_COMPROBANTE"].ToString();
                            itemInterfaceColl.newBill = odr["NUMERO_FACTURA_GENERADA"] == null ? string.Empty : odr["NUMERO_FACTURA_GENERADA"].ToString();
                            itemInterfaceColl.premium = odr["PRIMA_NETA"] == null ? string.Empty : odr["PRIMA_NETA"].ToString();
                            itemInterfaceColl.de = odr["NDE"] == null ? string.Empty : odr["NDE"].ToString();
                            itemInterfaceColl.tax = odr["NTAX"] == null ? string.Empty : odr["NTAX"].ToString();
                            itemInterfaceColl.totalAmmount = Decimal.Parse(odr["MONTO_TOTAL_FACTURA"] == null ? string.Empty : odr["MONTO_TOTAL_FACTURA"].ToString());
                            itemInterfaceColl.newComm = Decimal.Parse(odr["NPREMIUMNC"] == null ? string.Empty : odr["NPREMIUMNC"].ToString());
                            itemInterfaceColl.billNumber = odr["NRO_COMPROBANTE"] == null ? string.Empty : odr["NRO_COMPROBANTE"].ToString();
                            interfaceListColl.Add(itemInterfaceColl);
                        }
                    }
                    ELog.CloseConnection(odr);
                    result.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                    result.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                    result.listInterColl = interfaceListColl;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                ValType = (data.searchType == "2") ? data.searchType = ValType : data.searchType;

            }

            if (data.searchType == "3" || data.searchType == "6" || data.searchType == "8" || data.searchType == "9" || data.searchType == "11"
                || data.searchType == "12" || data.searchType == "13" || data.searchType == "14")
            {
                data.searchType = "3";

                List<ReportInterfPrem> interfaceListPrem = new List<ReportInterfPrem>();

                string storedProcedureName = ProcedureName.pkg_TableroControlConsultas + ".SPS_LIST_CRUCE_INTERFACE";

                try
                {
                    List<OracleParameter> parameter = new List<OracleParameter>();
                    //Parámetros de entrada
                    parameter.Add(new OracleParameter("P_DINTERFACEDATE_INI", OracleDbType.Varchar2, data.startDate, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_DINTERFACEDATE_FIN", OracleDbType.Varchar2, data.endDate, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_STI_BUSQUEDA", OracleDbType.Varchar2, data.searchType, ParameterDirection.Input));
                    //Parámetros de Salida
                    OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);
                    OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                    OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);

                    P_NCODE.Size = 4000;
                    P_SMESSAGE.Size = 4000;
                    parameter.Add(P_NCODE);
                    parameter.Add(P_SMESSAGE);
                    parameter.Add(C_TABLE);

                    OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                    if (odr.HasRows)
                    {
                        while (odr.Read())
                        {
                            ReportInterfPrem itemInterfacePrem = new ReportInterfPrem();
                            itemInterfacePrem.preliminary = odr["PRELIMINAR"] == null ? string.Empty : odr["PRELIMINAR"].ToString();
                            itemInterfacePrem.receipt = odr["RECIBO"] == null ? string.Empty : odr["RECIBO"].ToString();
                            itemInterfacePrem.branch = odr["RAMO"] == null ? string.Empty : odr["RAMO"].ToString();
                            itemInterfacePrem.scertype = odr["TIPO_REGISTROS"] == null ? string.Empty : odr["TIPO_REGISTROS"].ToString();
                            itemInterfacePrem.product = odr["PRODUCTO"] == null ? string.Empty : odr["PRODUCTO"].ToString();
                            itemInterfacePrem.policy = odr["POLIZA"] == null ? string.Empty : odr["POLIZA"].ToString();
                            itemInterfacePrem.type = odr["TIPO_MOVIMIENTO"] == null ? string.Empty : odr["TIPO_MOVIMIENTO"].ToString();
                            itemInterfacePrem.serie = odr["SERIE_COMPROBANTE"] == null ? string.Empty : odr["SERIE_COMPROBANTE"].ToString();
                            itemInterfacePrem.cardNumber = odr["VOUCHER"] == null ? string.Empty : odr["VOUCHER"].ToString();
                            itemInterfacePrem.cardType = odr["CODIGO_TIPO_PAGO"] == null ? string.Empty : odr["CODIGO_TIPO_PAGO"].ToString();
                            itemInterfacePrem.codeBank = odr["CODIGO_BANCO"] == null ? string.Empty : odr["CODIGO_BANCO"].ToString();
                            itemInterfacePrem.fecCont = odr["FECHA_CONTABILIZACION"] == null ? string.Empty : odr["FECHA_CONTABILIZACION"].ToString();
                            itemInterfacePrem.sclient = odr["CLIENTE"] == null ? string.Empty : odr["CLIENTE"].ToString();
                            itemInterfacePrem.currency = odr["MONEDA"] == null ? string.Empty : odr["MONEDA"].ToString();
                            itemInterfacePrem.branchLed = odr["RAMO_CONTABLE"] == null ? string.Empty : odr["RAMO_CONTABLE"].ToString();
                            itemInterfacePrem.operBank = odr["NOPERACION_BANCO"] == null ? string.Empty : odr["NOPERACION_BANCO"].ToString();
                            itemInterfacePrem.billType = odr["TIP_COMPROBANTE"] == null ? string.Empty : odr["TIP_COMPROBANTE"].ToString();
                            itemInterfacePrem.newBill = odr["NUMERO_FACTURA_GENERADA"] == null ? string.Empty : odr["NUMERO_FACTURA_GENERADA"].ToString();
                            itemInterfacePrem.premium = Decimal.Parse(odr["PRIMA_NETA"] == null ? string.Empty : odr["PRIMA_NETA"].ToString());
                            itemInterfacePrem.commAmmount = Decimal.Parse(odr["MONTO_COMISION"] == null ? string.Empty : odr["MONTO_COMISION"].ToString());
                            itemInterfacePrem.billAmmount = Decimal.Parse(odr["MONTO_FACTURA"] == null ? string.Empty : odr["MONTO_FACTURA"].ToString());
                            itemInterfacePrem.totalAmmount = Decimal.Parse(odr["MONTO_TOTAL_FACTURA"] == null ? string.Empty : odr["MONTO_TOTAL_FACTURA"].ToString());
                            itemInterfacePrem.newPremium = odr["NPREMIUMNC"] == null ? string.Empty : odr["NPREMIUMNC"].ToString();
                            itemInterfacePrem.billNumber = odr["NRO_COMPROBANTE"] == null ? string.Empty : odr["NRO_COMPROBANTE"].ToString();
                            interfaceListPrem.Add(itemInterfacePrem);
                        }
                    }
                    ELog.CloseConnection(odr);

                    result.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                    result.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                    result.listInterPrem = interfaceListPrem;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                ValType = (data.searchType == "3") ? data.searchType = ValType : data.searchType;
            }

            return result;
        }

        //Obtiene registros para reportes de Cotizacion - LS
        public Dictionary<string, object> ObtenerReporteDeCotizaciones(QuotationReportFiltersBM data)
        {
            Dictionary<string, object> map = new Dictionary<string, object>();
            List<OracleParameter> parameters = new List<OracleParameter>();
            string storedProcedureName = ProcedureName.sp_ReporteCotizacion;
            try
            {
                #region COMPLETO COMENT
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int64, data.nBranch, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int64, data.nProduct, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NQUOTATION", OracleDbType.Int64, data.nQuotation, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int64, data.nPolicy, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NSTATE", OracleDbType.Int64, data.nState, ParameterDirection.Input));

                parameters.Add(new OracleParameter("P_NTRX_PEND", OracleDbType.Int64, data.trxPendiente, ParameterDirection.Input));

                parameters.Add(new OracleParameter("P_NTYPE_CANAL", OracleDbType.Int64, data.typeCanal, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NIDDOC_TYPE", OracleDbType.Int64, data.nIdDocType, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, data.sDoc, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SCLIENAME", OracleDbType.Varchar2, data.SLEGALNAME, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SFIRSTNAME", OracleDbType.Varchar2, data.SFIRSTNAME, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SLASTNAME", OracleDbType.Varchar2, data.SLASTNAME, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SLASTNAME2", OracleDbType.Varchar2, data.SLASTNAME2, ParameterDirection.Input));

                parameters.Add(new OracleParameter("P_NIDDOC_TYPE_BR", OracleDbType.Int64, data.nIdDocType_BR, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SIDDOC_BR", OracleDbType.Varchar2, data.sDoc_BR, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SLEGALNAME_BR", OracleDbType.Varchar2, data.SLEGALNAME_BR, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SFIRSTNAME_BR", OracleDbType.Varchar2, data.SFIRSTNAME_BR, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SLASTNAME_BR", OracleDbType.Varchar2, data.SLASTNAME_BR, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SLASTNAME2_BR", OracleDbType.Varchar2, data.SLASTNAME2_BR, ParameterDirection.Input));

                parameters.Add(new OracleParameter("P_NIDDOC_TYPE_US", OracleDbType.Int64, data.nIdDocType_US, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SIDDOC_US", OracleDbType.Varchar2, data.sDoc_US, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SLEGALNAME_US", OracleDbType.Varchar2, data.SLEGALNAME_US, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SFIRSTNAME_US", OracleDbType.Varchar2, data.SFIRSTNAME_US, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SLASTNAME_US", OracleDbType.Varchar2, data.SLASTNAME_US, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SLASTNAME2_US", OracleDbType.Varchar2, data.SLASTNAME2_US, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_FLAG_DATE", OracleDbType.Int64, data.flagDate, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DEFFECDATE_INICIO", OracleDbType.Date, data.startDate, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DEFFECDATE_FIN", OracleDbType.Date, data.endDate, ParameterDirection.Input));

                var P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int64, ParameterDirection.Output);
                parameters.Add(P_COD_ERR);
                var P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                P_MESSAGE.Size = 2048;
                parameters.Add(P_MESSAGE);
                #endregion
                var C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameters.Add(C_TABLE);

                List<QuotationReportVM> lista = new List<QuotationReportVM>();

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameters);
                map["codErr"] = Convert.ToInt64(P_COD_ERR.Value.ToString());
                map["message"] = P_MESSAGE.Value.ToString();


                if ((long)map["codErr"] == 0)
                {
                    //if (odr.HasRows)
                    //{
                    while (odr.Read())
                    {
                        #region COMPLETO
                        QuotationReportVM item = new QuotationReportVM();
                        item.RAMO = odr["RAMO"];
                        item.PRODUCTO = odr["PRODUCTO"];
                        item.COTIZACION = odr["COTIZACION"];
                        item.FECHA_COTIZACION = odr["FECHA_COTIZACION"] == DBNull.Value ? "" : odr["FECHA_COTIZACION"];
                        item.FECHA_APROBACION = odr["FECHA_APROBACION"] == DBNull.Value ? "" : odr["FECHA_APROBACION"];
                        item.TRX_PENDIENTE = odr["TRX_PENDIENTE"] == DBNull.Value ? "" : odr["TRX_PENDIENTE"];
                        item.ESTADO = odr["ESTADO"] == DBNull.Value ? "" : odr["ESTADO"];
                        item.POLIZA = odr["POLIZA"] == DBNull.Value ? "" : odr["POLIZA"];
                        item.FECHA_EMISION = odr["FECHA_EMSION_POLIZA"] == DBNull.Value ? "" : odr["FECHA_EMSION_POLIZA"];
                        item.TIPO_DOCUMENTO_CO = odr["TIPO_DOCUMENTO_CO"] == DBNull.Value ? "" : odr["TIPO_DOCUMENTO_CO"];
                        item.DOC_CONTRATANTE = odr["DOC_CONTRATANTE"] == DBNull.Value ? "" : odr["DOC_CONTRATANTE"];
                        item.CONTRATANTE = odr["CONTRATANTE"] == DBNull.Value ? "" : odr["CONTRATANTE"];
                        item.TIPO_CANAL = odr["CATEGORIA_CANAL"] == DBNull.Value ? "" : odr["CATEGORIA_CANAL"];
                        item.TIPO_DOCUMENTO_BR = odr["TIPO_DOCUMENTO_BR"] == DBNull.Value ? "" : odr["TIPO_DOCUMENTO_BR"];
                        item.DOC_BROKER = odr["DOC_BROKER"] == DBNull.Value ? "" : odr["DOC_BROKER"];
                        item.BROKER = odr["BROKER"] == DBNull.Value ? "" : odr["BROKER"];
                        item.PRIMA_MINIMA = odr["PRIMA_MINIMA"] == DBNull.Value ? "0.00" : odr["PRIMA_MINIMA"];
                        item.PLANILLA = odr["PLANILLA"] == DBNull.Value ? "0.00" : odr["PLANILLA"];
                        item.PRIMA_NETA = odr["PRIMA_NETA"] == DBNull.Value ? "0.00" : odr["PRIMA_NETA"];
                        item.USUARIO = odr["USUARIO"] == DBNull.Value ? "" : odr["USUARIO"];
                        item.PROCEDENCIA = odr["PROCEDENCIA"] == DBNull.Value ? "" : odr["PROCEDENCIA"];
                        item.TIPO_OPERACION = odr["TIPO_OPERACION"] == DBNull.Value ? "" : odr["TIPO_OPERACION"];
                        lista.Add(item);
                        #endregion
                    }
                    //}
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

        //Obtiene excel para reporte de cotizaciones - LS
        public string ObtenerPlantillaCotizacion(QuotationReportFiltersBM data)
        {
            Dictionary<string, object> mapa = this.ObtenerReporteDeCotizaciones(data);
            List<QuotationReportVM> lista = (List<QuotationReportVM>)mapa["lista"];
            string templatePath = "D:/doc_templates/reportes/dev/MiPlantillaCotizacion.xlsx";
            string plantilla = "";
            try
            {
                MemoryStream ms = new MemoryStream();
                //ms.Flush();
                //ms.Position = 0; // or ms.Seek(0, SeekOrigin.Begin);
                using (FileStream fs = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
                {
                    fs.CopyTo(ms);
                }
                using (SLDocument sl = new SLDocument(ms))
                {
                    int i = 7;
                    int letra = 65;

                    sl.SetCellValue("B1", lista[0].TIPO_OPERACION);
                    sl.SetCellValue("B2", data.startDate.ToShortDateString());
                    sl.SetCellValue("B3", data.endDate.ToShortDateString());

                    foreach (QuotationReportVM item in lista)
                    {
                        int c = 0;

                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.RAMO);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.PRODUCTO);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.COTIZACION);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.FECHA_COTIZACION);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.FECHA_APROBACION);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.TRX_PENDIENTE);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.ESTADO);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.POLIZA);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.FECHA_EMISION);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.TIPO_DOCUMENTO_CO);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.DOC_CONTRATANTE);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.CONTRATANTE);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.TIPO_CANAL);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.TIPO_DOCUMENTO_BR);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.DOC_BROKER);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.BROKER);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.PRIMA_MINIMA);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.PLANILLA);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.PRIMA_NETA);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.USUARIO);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.PROCEDENCIA);
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

        //Obtiene el tipo de canal - LS
        public List<ChannelTypeVM> GetChannelTypeAllList()
        {
            List<ChannelTypeVM> contactTypeList = new List<ChannelTypeVM>();
            List<OracleParameter> parameter = new List<OracleParameter>();

            try
            {
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, contactTypeList, ParameterDirection.Output);

                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(ProcedureName.pkg_ReportesPD + ".REA_CANAL", parameter);
                while (odr.Read())
                {
                    ChannelTypeVM item = new ChannelTypeVM();
                    item.NID_TYPE_CHANNEL = odr["NID_TYPE_CHANNEL"] == DBNull.Value ? 0 : double.Parse(odr["NID_TYPE_CHANNEL"].ToString());
                    item.SDES_TYPE_CHANNEL = odr["SDES_TYPE_CHANNEL"] == DBNull.Value ? "" : odr["SDES_TYPE_CHANNEL"].ToString();

                    contactTypeList.Add(item);
                }

                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                LogControl.save("GetChannelTypeAllList", ex.ToString(), "3");
            }

            return contactTypeList;
        }

        //Obtiene el Tipo de Documento - LS
        public List<DocumentTypeQRVM> DocumentTypeQuotationReport()
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<DocumentTypeQRVM> listDocumentType = new List<DocumentTypeQRVM>();

            string storedProcedureName = ProcedureName.pkg_ReportesPD + ".REA_TIPO_DOCUMENTO";

            try
            {
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);
                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        DocumentTypeQRVM item = new DocumentTypeQRVM();
                        item.NIDDOC_TYPE = odr["NIDDOC_TYPE"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NIDDOC_TYPE"].ToString());
                        item.SDESCRIPT = odr["SDESCRIPT"] == DBNull.Value ? string.Empty : odr["SDESCRIPT"].ToString();
                        listDocumentType.Add(item);
                    }
                }

                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return listDocumentType;

        }

        public List<RequestAllTypeVM> GetRequestAllList()
        {
            List<RequestAllTypeVM> contactTypeList = new List<RequestAllTypeVM>();
            List<OracleParameter> parameter = new List<OracleParameter>();

            try
            {
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, contactTypeList, ParameterDirection.Output);

                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(ProcedureName.pkg_ReporteIndicadores + ".READ_TIPO_SOLICITUD", parameter);
                while (odr.Read())
                {
                    RequestAllTypeVM item = new RequestAllTypeVM();
                    item.NTYPE_MOVEMENT = odr["NTYPE_MOVEMENT"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NTYPE_MOVEMENT"].ToString());
                    item.SDESCRIPT = odr["SDESCRIPT"] == DBNull.Value ? string.Empty : odr["SDESCRIPT"].ToString();

                    contactTypeList.Add(item);
                }

                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                LogControl.save("GetRequestAllList", ex.ToString(), "3");
            }

            return contactTypeList;
        }

        //Obtiene registros para reportes de Tramites - LS
        public Dictionary<string, object> ObtenerReporteDeTramites(ProcedureReportFiltersBM data)
        {
            Dictionary<string, object> map = new Dictionary<string, object>();
            List<OracleParameter> parameters = new List<OracleParameter>();
            string storedProcedureName = ProcedureName.pkg_ReporteIndicadores + ".SP_REA_TRAMITE_REPORT";
            try
            {
                #region COMPLETO COMENT
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int64, data.nProduct, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int64, data.nBranch, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_TYPE_REPORT", OracleDbType.Int64, data.nTypeReport, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_FECHA_DESDE", OracleDbType.Date, data.startDate.ToShortDateString(), ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_FECHA_HASTA", OracleDbType.Date, data.endDate.ToShortDateString(), ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NHOUR_CORTE", OracleDbType.Int64, data.nCut, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NID_TRAMITE", OracleDbType.Int64, data.nProcedure, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int64, data.nQuotation, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_POLICY", OracleDbType.Int64, data.nPolicy, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NSTATUS_TRA", OracleDbType.Int64, data.nState, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SISCLIENT_GBD", OracleDbType.Int64, data.nTypeBill, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_STRANSAC", OracleDbType.Int64, data.nTypeRequest, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int64, data.nUserCode, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NFECHA_OPERA", OracleDbType.Int64, data.flagDate, ParameterDirection.Input));

                //var P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int64, ParameterDirection.Output);
                //parameters.Add(P_COD_ERR);
                //var P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                //P_MESSAGE.Size = 2048;
                //parameters.Add(P_MESSAGE);
                #endregion
                var C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameters.Add(C_TABLE);
                var C_TABLE_PEN = new OracleParameter("C_TABLE_PEN", OracleDbType.RefCursor, ParameterDirection.Output);
                parameters.Add(C_TABLE_PEN);
                var C_TABLE_PRO = new OracleParameter("C_TABLE_PRO", OracleDbType.RefCursor, ParameterDirection.Output);
                parameters.Add(C_TABLE_PRO);
                var C_TABLE_TIP = new OracleParameter("C_TABLE_TIP", OracleDbType.RefCursor, ParameterDirection.Output);
                parameters.Add(C_TABLE_TIP);

                List<ReportProcedureVM> lista = new List<ReportProcedureVM>();
                List<ReportProcedureProcessVM> listaPend = new List<ReportProcedureProcessVM>();
                List<ReportProcedureProductivityVM> listaProd = new List<ReportProcedureProductivityVM>();
                List<ReportProcedureTypeVM> listaType = new List<ReportProcedureTypeVM>();

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameters);
                //map["codErr"] = Convert.ToInt64(P_COD_ERR.Value.ToString());
                //map["message"] = P_MESSAGE.Value.ToString();


                //if ((long)map["codErr"] == 0)
                //{
                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        #region COMPLETO
                        ReportProcedureVM item = new ReportProcedureVM();
                        item.NPRODUCT = odr["NPRODUCT"];
                        item.NBRANCH = odr["NBRANCH"];
                        item.NID_TRAMITE = odr["NID_TRAMITE"] == DBNull.Value ? "" : odr["NID_TRAMITE"];
                        item.NID_COTIZACION = odr["NID_COTIZACION"] == DBNull.Value ? "" : odr["NID_COTIZACION"];
                        item.POLICY = odr["POLICY"] == DBNull.Value ? "" : odr["POLICY"];
                        item.DFECHA_CREATE = odr["DFECHA_CREATE"] == DBNull.Value ? "" : odr["DFECHA_CREATE"];
                        item.DHORA_CREATE = odr["DHORA_CREATE"] == DBNull.Value ? "" : odr["DHORA_CREATE"];
                        item.SUUSERCODE_OPERA = odr["SUUSERCODE_OPERA"] == DBNull.Value ? "" : odr["SUUSERCODE_OPERA"];
                        item.STRANSAC_TYPE = odr["STRANSAC_TYPE"] == DBNull.Value ? "" : odr["STRANSAC_TYPE"];
                        item.SSTATUS_TRA = odr["SSTATUS_TRA"] == DBNull.Value ? "" : odr["SSTATUS_TRA"];
                        item.NSTATUS_TRA_HOMOLOG = odr["NSTATUS_TRA_HOMOLOG"] == DBNull.Value ? "" : odr["NSTATUS_TRA_HOMOLOG"];
                        item.SSTATUS_TRA_HOMOLOG = odr["SSTATUS_TRA_HOMOLOG"] == DBNull.Value ? "" : odr["SSTATUS_TRA_HOMOLOG"];
                        item.TYPE_ACCOUNT = odr["TYPE_ACCOUNT"] == DBNull.Value ? "" : odr["TYPE_ACCOUNT"];
                        item.SMOTIVO_RECHAZO = odr["SMOTIVO_RECHAZO"] == DBNull.Value ? "" : odr["SMOTIVO_RECHAZO"];
                        item.SCOMENTARIO_OPERA = odr["SCOMENTARIO_OPERA"] == DBNull.Value ? "" : odr["SCOMENTARIO_OPERA"];
                        item.DFECHA_ASSIGNED_OPERA = odr["DFECHA_ASSIGNED_OPERA"] == DBNull.Value ? "" : String.Format("{0:dd/MM/yyyy hh:mm:ss tt}", odr["DFECHA_ASSIGNED_OPERA"]);
                        item.DFECHA_ASSIGNED_OPERA_HORA = odr["DFECHA_ASSIGNED_OPERA_HORA"] == DBNull.Value ? "" : String.Format("{0:dd/MM/yyyy hh:mm:ss tt}", odr["DFECHA_ASSIGNED_OPERA_HORA"]);
                        item.DFECHA_DER_TECNICA = odr["DFECHA_DER_TECNICA"] == DBNull.Value ? "" : String.Format("{0:dd/MM/yyyy hh:mm:ss tt}", odr["DFECHA_DER_TECNICA"]);
                        item.DFECHA_DER_TECNICA_HOUR = odr["DFECHA_DER_TECNICA_HOUR"] == DBNull.Value ? "" : String.Format("{0:dd/MM/yyyy hh:mm:ss tt}", odr["DFECHA_DER_TECNICA_HOUR"]);
                        item.DFECHA_RES_TECNICA = odr["DFECHA_RES_TECNICA"] == DBNull.Value ? "" : String.Format("{0:dd/MM/yyyy hh:mm:ss tt}", odr["DFECHA_RES_TECNICA"]);
                        item.DFECHA_RES_TECNICA_HOUR = odr["DFECHA_RES_TECNICA_HOUR"] == DBNull.Value ? "" : String.Format("{0:dd/MM/yyyy hh:mm:ss tt}", odr["DFECHA_RES_TECNICA_HOUR"]);
                        item.NTIME_ATTENDED_TECNICA = odr["NTIME_ATTENDED_TECNICA"] == DBNull.Value ? "" : odr["NTIME_ATTENDED_TECNICA"];
                        item.NTIME_MIN_ATTEND_TECNICA = odr["NTIME_MIN_ATTEND_TECNICA"] == DBNull.Value ? "" : odr["NTIME_MIN_ATTEND_TECNICA"];
                        //item.DFECHA_DER_EJECCOM = odr["DFECHA_DER_EJECCOM"] == DBNull.Value ? "" : String.Format("{0:dd/MM/yyyy hh:mm:ss tt}", odr["DFECHA_DER_EJECCOM"]);
                        //item.DFECHA_DER_EJECCOM_HOUR = odr["DFECHA_DER_EJECCOM_HOUR"] == DBNull.Value ? "" : String.Format("{0:dd/MM/yy hh:mm:ss tt}", odr["DFECHA_DER_EJECCOM_HOUR"]);
                        item.DFECHA_RES_EJECCOM = odr["DFECHA_RES_EJECCOM"] == DBNull.Value ? "" : String.Format("{0:dd/MM/yyyy hh:mm:ss tt}", odr["DFECHA_RES_EJECCOM"]);
                        item.DFECHA_RES_EJECCOM_HOUR = odr["DFECHA_RES_EJECCOM_HOUR"] == DBNull.Value ? "" : String.Format("{0:dd/MM/yyyy hh:mm:ss tt}", odr["DFECHA_RES_EJECCOM_HOUR"]);
                        item.DFECHA_ATTENDED_OPERA = odr["DFECHA_ATTENDED_OPERA"] == DBNull.Value ? "" : String.Format("{0:dd/MM/yyyy hh:mm:ss tt}", odr["DFECHA_ATTENDED_OPERA"]);
                        item.DFECHA_ATTENDED_OPERA_HOUR = odr["DFECHA_ATTENDED_OPERA_HOUR"] == DBNull.Value ? "" : String.Format("{0:dd/MM/yyyy hh:mm:ss tt}", odr["DFECHA_ATTENDED_OPERA_HOUR"]);
                        item.NTIME_ATTENDED = odr["NTIME_ATTENDED"] == DBNull.Value ? "" : odr["NTIME_ATTENDED"];
                        item.NTIME_MIN_ATTENDED = odr["NTIME_MIN_ATTENDED"] == DBNull.Value ? "" : odr["NTIME_MIN_ATTENDED"];
                        item.NTIME_MIN_ATTENDED_OPERA_TECNICA = odr["NTIME_MIN_ATTENDED_OPERA_TECNICA"] == DBNull.Value ? "" : odr["NTIME_MIN_ATTENDED_OPERA_TECNICA"];
                        item.SUUSERCODE_COMMER = odr["SUUSERCODE_COMMER"] == DBNull.Value ? "" : odr["SUUSERCODE_COMMER"];
                        lista.Add(item);
                        #endregion
                    }

                    odr.NextResult();

                    while (odr.Read())
                    {
                        #region COMPLETO
                        ReportProcedureProcessVM item = new ReportProcedureProcessVM();
                        item.SUUSERCODE_OPERA = odr["SUUSERCODE_OPERA"] == DBNull.Value ? "" : odr["SUUSERCODE_OPERA"];
                        item.NFROM0TO2 = odr["NFROM0TO2"] == DBNull.Value ? "" : odr["NFROM0TO2"];
                        item.NFROM3TO5 = odr["NFROM3TO5"] == DBNull.Value ? "" : odr["NFROM3TO5"];
                        item.NFRO6TO10 = odr["NFRO6TO10"] == DBNull.Value ? "" : odr["NFRO6TO10"];
                        item.NFROM11TOMAS = odr["NFROM11TOMAS"] == DBNull.Value ? "" : odr["NFROM11TOMAS"];
                        listaPend.Add(item);
                        #endregion
                    }

                    odr.NextResult();

                    while (odr.Read())
                    {
                        #region COMPLETO
                        ReportProcedureProductivityVM item = new ReportProcedureProductivityVM();
                        item.SUUSERCODE_OPERA = odr["SUUSERCODE_OPERA"] == DBNull.Value ? "" : odr["SUUSERCODE_OPERA"];
                        item.NCOUNT_TRAMITE = odr["NCOUNT_TRAMITE"] == DBNull.Value ? "" : odr["NCOUNT_TRAMITE"];
                        item.NPORCENTAJE = odr["NPORCENTAJE"] == DBNull.Value ? "" : odr["NPORCENTAJE"];
                        listaProd.Add(item);
                        #endregion
                    }

                    odr.NextResult();

                    while (odr.Read())
                    {
                        #region COMPLETO
                        ReportProcedureTypeVM item = new ReportProcedureTypeVM();
                        item.SUUSERCODE_OPERA = odr["SUUSERCODE_OPERA"] == DBNull.Value ? "" : odr["SUUSERCODE_OPERA"];
                        item.DFECHA_CREATE = odr["DFECHA_CREATE"] == DBNull.Value ? "" : odr["DFECHA_CREATE"];
                        item.NINCLUSION = odr["NINCLUSION"] == DBNull.Value ? "" : odr["NINCLUSION"];
                        item.NRENOVACION = odr["NRENOVACION"] == DBNull.Value ? "" : odr["NRENOVACION"];
                        item.NDECLARACION = odr["NDECLARACION"] == DBNull.Value ? "" : odr["NDECLARACION"];
                        item.NEXCLUSION = odr["NEXCLUSION"] == DBNull.Value ? "" : odr["NEXCLUSION"];
                        item.NENDOSO = odr["NENDOSO"] == DBNull.Value ? "" : odr["NENDOSO"];
                        item.NBROKER = odr["NBROKER"] == DBNull.Value ? "" : odr["NBROKER"];
                        item.NTOTAL_TRAMITE = odr["NTOTAL_TRAMITE"] == DBNull.Value ? "" : odr["NTOTAL_TRAMITE"];
                        listaType.Add(item);
                        #endregion
                    }
                }

                //}
                ELog.CloseConnection(odr);

                if (data.nTypeReport == 1)
                {
                    map["listaPend"] = listaPend;
                }
                if (data.nTypeReport == 2)
                {
                    map["listaProd"] = listaProd;
                }
                if (data.nTypeReport == 4)
                {
                    map["listaType"] = listaType;
                }
                map["lista"] = lista;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return map;
        }

        //Obtiene excel para reporte de tramites - LS
        public string ObtenerPlantillaReporteTramite(ProcedureReportFiltersBM data)
        {
            Dictionary<string, object> mapa = this.ObtenerReporteDeTramites(data);
            List<ReportProcedureVM> lista = new List<ReportProcedureVM>();
            List<ReportProcedureProcessVM> listaPend = new List<ReportProcedureProcessVM>();
            List<ReportProcedureProductivityVM> listaProd = new List<ReportProcedureProductivityVM>();
            List<ReportProcedureTypeVM> listaType = new List<ReportProcedureTypeVM>();

            string templatePath = "";
            string templatebase = ELog.obtainConfig("pathSLA");
            string corte = "";
            if (data.nCut == 1) { corte = "09:00 AM"; }
            if (data.nCut == 2) { corte = "12:00 PM"; }
            if (data.nCut == 3) { corte = "06:00 PM"; }

            switch (data.nTypeReport)
            {
                case 1:
                    listaPend = (List<ReportProcedureProcessVM>)mapa["listaPend"];
                    templatePath = templatebase + "Template_Tramites_Pendientes.xlsx";
                    break;
                case 2:
                    listaProd = (List<ReportProcedureProductivityVM>)mapa["listaProd"];
                    templatePath = templatebase + "Template_Tramites_Productividad.xlsx";
                    break;
                case 3:
                    lista = (List<ReportProcedureVM>)mapa["lista"];
                    templatePath = templatebase + "Template_Tramites_TiempoTotal.xlsx";
                    break;
                case 4:
                    listaType = (List<ReportProcedureTypeVM>)mapa["listaType"];
                    templatePath = templatebase + "Template_Tramites_Tipos.xlsx";
                    break;
            }

            string plantilla = "";

            try
            {
                MemoryStream ms = new MemoryStream();
                //ms.Flush();
                //ms.Position = 0; // or ms.Seek(0, SeekOrigin.Begin);
                using (FileStream fs = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
                {
                    fs.CopyTo(ms);
                }
                using (SLDocument sl = new SLDocument(ms))
                {
                    int i = 0;
                    int letra = 65;

                    //Tramite Pendiente
                    if (data.nTypeReport == 1)
                    {
                        i = 6;
                        sl.SetCellValue("B1", "TRÁMITES PENDIENTES");
                        sl.SetCellValue("B2", "POR HORA");
                        sl.SetCellValue("C2", corte);
                        sl.SetCellValue("B3", data.startDate.ToShortDateString());
                        sl.SetCellValue("C3", data.endDate.ToShortDateString());

                        foreach (ReportProcedureProcessVM item in listaPend)
                        {
                            int c = 1;

                            sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.SUUSERCODE_OPERA);
                            sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.NFROM0TO2);
                            sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.NFROM3TO5);
                            sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.NFRO6TO10);
                            sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.NFROM11TOMAS);
                            i++;
                        }
                    }

                    //Productividad
                    if (data.nTypeReport == 2)
                    {
                        i = 7;
                        sl.SetCellValue("B1", DateTime.Now.ToShortDateString());
                        sl.SetCellValue("B2", "PRODUCTIVIDAD");
                        sl.SetCellValue("B3", data.startDate.ToShortDateString());
                        sl.SetCellValue("C3", data.endDate.ToShortDateString());

                        foreach (ReportProcedureProductivityVM item in listaProd)
                        {
                            int c = 0;

                            sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.SUUSERCODE_OPERA);
                            sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.NCOUNT_TRAMITE);
                            sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.NPORCENTAJE);
                            i++;
                        }
                    }

                    //Tiempo Total
                    if (data.nTypeReport == 3)
                    {
                        i = 7;
                        sl.SetCellValue("B1", DateTime.Now.ToShortDateString());
                        sl.SetCellValue("B2", "TIEMPO TOTAL DE ATENCIÓN");
                        sl.SetCellValue("B3", data.startDate.ToShortDateString());
                        sl.SetCellValue("C3", data.endDate.ToShortDateString());

                        foreach (ReportProcedureVM item in lista)
                        {
                            int c = 0;

                            sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.NID_COTIZACION);
                            sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.POLICY);
                            sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.NID_TRAMITE);
                            sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.STRANSAC_TYPE);
                            sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.TYPE_ACCOUNT);
                            sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.SSTATUS_TRA);
                            sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.SSTATUS_TRA_HOMOLOG); //descripción homologada
                            sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.SCOMENTARIO_OPERA); // comentario de operaciones
                            sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.SUUSERCODE_COMMER);
                            sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.DFECHA_CREATE);
                            sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.DHORA_CREATE);
                            sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.DFECHA_ASSIGNED_OPERA); // fecha asing de operaciones
                            sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.DFECHA_ASSIGNED_OPERA_HORA); // hora aignacion de operaciones
                            sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.SUUSERCODE_OPERA);
                            sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.SMOTIVO_RECHAZO); // motivo de rechazo
                            sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.DFECHA_DER_TECNICA);
                            sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.DFECHA_DER_TECNICA_HOUR);
                            sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.DFECHA_RES_TECNICA);
                            sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.DFECHA_RES_TECNICA_HOUR);
                            sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.NTIME_ATTENDED_TECNICA);
                            sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.NTIME_MIN_ATTEND_TECNICA);
                            sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.DFECHA_ATTENDED_OPERA);
                            sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.DFECHA_ATTENDED_OPERA_HOUR);
                            //sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.DFECHA_DER_EJECCOM);
                            //sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.DFECHA_DER_EJECCOM_HOUR);
                            sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.DFECHA_RES_EJECCOM);
                            sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.DFECHA_RES_EJECCOM_HOUR);
                            sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.NTIME_ATTENDED);
                            if ((letra + c) == 91)
                            {
                                c = 0;
                                sl.SetCellValue((char)(letra) + Char.ToString((char)(letra + c++)) + i, item.NTIME_MIN_ATTENDED);
                                sl.SetCellValue((char)(letra) + Char.ToString((char)(letra + c++)) + i, item.NTIME_MIN_ATTENDED_OPERA_TECNICA);
                            }
                            // sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.NTIME_MIN_ATTENDED);
                            //sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.NTIME_MIN_ATTENDED_OPERA_TECNICA);

                            i++;
                        }
                    }

                    //Tipo Tramite
                    if (data.nTypeReport == 4)
                    {
                        i = 6;
                        sl.SetCellValue("B1", DateTime.Now.ToShortDateString());
                        sl.SetCellValue("B2", "TIPOS DE TRÁMITE");

                        foreach (ReportProcedureTypeVM item in listaType)
                        {
                            int c = 1;

                            sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.SUUSERCODE_OPERA);
                            sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.DFECHA_CREATE);
                            sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.NINCLUSION);
                            sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.NRENOVACION);
                            sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.NDECLARACION);
                            sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.NEXCLUSION);
                            sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.NENDOSO);
                            sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.NBROKER);
                            sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.NTOTAL_TRAMITE);
                            i++;
                        }
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

        public List<UsersReportPRVM> ObtenerUsersList(string productId, string branch)
        {
            List<OracleParameter> parameters = new List<OracleParameter>();
            List<UsersReportPRVM> listUsers = new List<UsersReportPRVM>();

            string storedProcedureName = ProcedureName.pkg_ReporteIndicadores + ".READ_USERS_OPE";

            try
            {

                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int64, branch, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int64, productId, ParameterDirection.Input));

                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameters.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameters);
                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        UsersReportPRVM item = new UsersReportPRVM();
                        item.NIDUSER = odr["NIDUSER"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NIDUSER"].ToString());
                        item.SNAMES = odr["SNAMES"] == DBNull.Value ? string.Empty : odr["SNAMES"].ToString();
                        listUsers.Add(item);
                    }
                }

                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return listUsers;

        }

        public List<ProcedureStatus> GetProcedureStatusList()
        {
            List<ProcedureStatus> response = new List<ProcedureStatus>();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.pkg_Tramites + ".READ_TRAMITE_ESTADO";
            OracleDataReader dr = null;
            try
            {
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);

                parameter.Add(C_TABLE);

                dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storeprocedure, parameter);

                foreach (var obj in dr.ReadRowsList<ProcedureStatus>())
                {
                    response.Add(obj);
                }
                ELog.CloseConnection(dr);
            }
            catch (Exception ex)
            {
                response = new List<ProcedureStatus>();
                ELog.CloseConnection(dr);
            }
            return response;
        }

        public ResponseReportATPVM ReportOperationVDP(RequestReportOperationVDP request)
        {
            ResponseReportATPVM response = new ResponseReportATPVM();
            //string nameStore = ProcedureName.pkg_ATP + ".SP_ATP_REPORT_DATA";
            string nameStore = "INSUDB.SP_REPORT_OPE_VDP";
            List<OracleParameter> oracleParameterList = new List<OracleParameter>();
            ReportOperationVDP dataReport = new ReportOperationVDP();
            List<ReportOperationModelVDP> reportVig = new List<ReportOperationModelVDP>();
            List<ReportOperationModelVDP> reportAnnul = new List<ReportOperationModelVDP>();
            try
            {
                if (request.dStart_Date.HasValue)
                    oracleParameterList.Add(new OracleParameter("P_DSTARTDATE", OracleDbType.Date, (object)request.dStart_Date, ParameterDirection.Input));
                else
                    oracleParameterList.Add(new OracleParameter("P_DSTARTDATE", OracleDbType.Date, (object)DBNull.Value, ParameterDirection.Input));

                if (request.dExpir_Dat.HasValue)
                    oracleParameterList.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, (object)request.dExpir_Dat, ParameterDirection.Input));
                else
                    oracleParameterList.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, (object)DBNull.Value, ParameterDirection.Input));

                string[] nameCursor = new string[2] { "C_POLICY", "C_POLICY_ANNUL" };
                oracleParameterList.Add(new OracleParameter(nameCursor[0], OracleDbType.RefCursor, ParameterDirection.Output));
                oracleParameterList.Add(new OracleParameter(nameCursor[1], OracleDbType.RefCursor, ParameterDirection.Output));

                OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(nameStore, nameCursor, oracleParameterList);

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        ReportOperationModelVDP obj = new ReportOperationModelVDP();
                        obj.POLIZA = dr["POLIZA"] == DBNull.Value ? 0 : Convert.ToInt64(dr["POLIZA"].ToString());
                        obj.NOMBRES = dr["NOMBRES"] == DBNull.Value ? string.Empty : dr["NOMBRES"].ToString().Trim();
                        obj.TIPO_DOCUMENTO = dr["TIPO_DOCUMENTO"] == DBNull.Value ? string.Empty : dr["TIPO_DOCUMENTO"].ToString().Trim();
                        obj.NRO_DOCUMENTO = dr["NRO_DOCUMENTO"] == DBNull.Value ? string.Empty : dr["NRO_DOCUMENTO"].ToString().Trim();
                        obj.CORREO_ELECTRONICO = dr["CORREO_ELECTRONICO"] == DBNull.Value ? string.Empty : dr["CORREO_ELECTRONICO"].ToString().Trim();
                        obj.CELULAR = dr["CELULAR"] == DBNull.Value ? string.Empty : dr["CELULAR"].ToString().Trim();
                        obj.DIRECCION = dr["DIRECCION"] == DBNull.Value ? string.Empty : dr["DIRECCION"].ToString().Trim();
                        obj.DISTRITO = dr["DISTRITO"] == DBNull.Value ? string.Empty : dr["DISTRITO"].ToString().Trim();
                        obj.PROVINCIA = dr["PROVINCIA"] == DBNull.Value ? string.Empty : dr["PROVINCIA"].ToString().Trim();
                        obj.DEPARTAMENTO = dr["DEPARTAMENTO"] == DBNull.Value ? string.Empty : dr["DEPARTAMENTO"].ToString().Trim();
                        obj.NACIMIENTO = dr["NACIMIENTO"] == DBNull.Value ? string.Empty : dr["NACIMIENTO"].ToString();
                        obj.SUMA_ASEGURADA_COVER1 = dr["SUMA_ASEGURADA_COVER1"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["SUMA_ASEGURADA_COVER1"].ToString());
                        obj.SUMA_ASEGURADA_COVER2 = dr["SUMA_ASEGURADA_COVER2"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["SUMA_ASEGURADA_COVER2"].ToString());
                        obj.PRIMERA_PRIMA = dr["PRIMERA_PRIMA"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PRIMERA_PRIMA"].ToString());
                        obj.PRIMA_MENSUAL = dr["PRIMA_MENSUAL"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PRIMA_MENSUAL"].ToString());
                        obj.PRIMERA_PRIMA_ANUAL = dr["PRIMERA_PRIMA_ANUAL"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PRIMERA_PRIMA_ANUAL"].ToString());
                        obj.MONTO_PRIMAS = dr["MONTO_PRIMAS"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["MONTO_PRIMAS"].ToString());
                        obj.ABONO = dr["ABONO"] == DBNull.Value ? string.Empty : dr["ABONO"].ToString();
                        obj.FECHA_INICIO_VIGENCIA = dr["FECHA_INICIO"] == DBNull.Value ? string.Empty : dr["FECHA_INICIO"].ToString();
                        obj.FECHA_EMISION = dr["EMISION"] == DBNull.Value ? string.Empty : dr["EMISION"].ToString();
                        obj.TEMPORALIDAD = dr["TEMPORALIDAD"] == DBNull.Value ? 0 : Convert.ToInt32(dr["TEMPORALIDAD"].ToString());
                        obj.MONEDA = dr["MONEDA"] == DBNull.Value ? string.Empty : dr["MONEDA"].ToString().Trim();
                        obj.POR_DEVOLUCION = dr["POR_DEVOLUCION"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["POR_DEVOLUCION"].ToString());
                        obj.COD_SEXO = dr["COD_SEXO"] == DBNull.Value ? string.Empty : dr["COD_SEXO"].ToString();
                        obj.CUOTAS_VENCIDAS = dr["CUOTAS_VENCIDAS"] == DBNull.Value ? 0 : Convert.ToInt32(dr["CUOTAS_VENCIDAS"].ToString());
                        obj.CUOTAS_PAGADAS = dr["CUOTAS_PAGADAS"] == DBNull.Value ? 0 : Convert.ToInt32(dr["CUOTAS_PAGADAS"].ToString());
                        obj.ESTADO_POLIZA = dr["ESTADO_POLIZA"] == DBNull.Value ? string.Empty : dr["ESTADO_POLIZA"].ToString();
                        obj.FECHA_FIN_VIGENCIA = dr["FECHA_FIN_VIGENCIA"] == DBNull.Value ? string.Empty : dr["FECHA_FIN_VIGENCIA"].ToString();
                        obj.FECHA_FALLECIMIENTO = dr["FECHA_FALLECIMIENTO"] == DBNull.Value ? string.Empty : dr["FECHA_FALLECIMIENTO"].ToString();
                        obj.NRO_RECIBO = dr["NRO_RECIBO"] == DBNull.Value ? 0 : Convert.ToInt64(dr["NRO_RECIBO"].ToString());
                        obj.NRO_BOLETA = dr["NRO_BOLETA"] == DBNull.Value ? string.Empty : dr["NRO_BOLETA"].ToString();
                        obj.ESTADO_BOLETA = dr["ESTADO_BOLETA"] == DBNull.Value ? string.Empty : dr["ESTADO_BOLETA"].ToString();
                        obj.FECHA_BOLETA = dr["FECHA_BOLETA"] == DBNull.Value ? string.Empty : dr["FECHA_BOLETA"].ToString();
                        obj.MONTO_DEVOLUCION = dr["MONTO_DEVOLUCION"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["MONTO_DEVOLUCION"].ToString());
                        obj.JEFE = dr["JEFE"] == DBNull.Value ? string.Empty : dr["JEFE"].ToString();
                        obj.SUPERVISOR = dr["SUPERVISOR"] == DBNull.Value ? string.Empty : dr["SUPERVISOR"].ToString();
                        obj.DOC_ASESOR_VENTA = dr["DOC_ASESOR_VENTA"] == DBNull.Value ? string.Empty : dr["DOC_ASESOR_VENTA"].ToString();
                        obj.ASESOR_VENTA = dr["ASESOR_VENTA"] == DBNull.Value ? string.Empty : dr["ASESOR_VENTA"].ToString();
                        obj.DOC_ASESOR_MANT = dr["DOC_ASESOR_MANT"] == DBNull.Value ? string.Empty : dr["DOC_ASESOR_MANT"].ToString();
                        obj.ASESOR_MANT = dr["ASESOR_MANT"] == DBNull.Value ? string.Empty : dr["ASESOR_MANT"].ToString();
                        obj.COMISION = dr["COMISION"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["COMISION"].ToString());
                        obj.COMISION_PAGADA = dr["COMISION_PAGADA"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["COMISION_PAGADA"].ToString());
                        obj.MONTO_FACTURADO = dr["MONTO_FACTURADO"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["MONTO_FACTURADO"].ToString());
                        obj.MONTO_NO_DEVENGADO = dr["MONTO_NO_DEVENGADO"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["MONTO_NO_DEVENGADO"].ToString());
                        obj.ANUALIDAD = dr["ANUALIDAD"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ANUALIDAD"].ToString());
                        obj.NUMERO_Cuotas_Devengadas = dr["NUMERO_Cuotas_Devengadas"] == DBNull.Value ? 0 : Convert.ToInt32(dr["NUMERO_Cuotas_Devengadas"].ToString());
                        obj.NUMERO_MESES_NO_DEVENGADAS = dr["NUMERO_MESES_NO_DEVENGADAS"] == DBNull.Value ? 0 : Convert.ToInt32(dr["NUMERO_MESES_NO_DEVENGADAS"].ToString());
                        reportVig.Add(obj);
                    }

                    dr.NextResult();
                    while (dr.Read())
                    {
                        ReportOperationModelVDP obj = new ReportOperationModelVDP();
                        obj.POLIZA = dr["POLIZA"] == DBNull.Value ? 0 : Convert.ToInt64(dr["POLIZA"].ToString());
                        obj.NOMBRES = dr["NOMBRES"] == DBNull.Value ? string.Empty : dr["NOMBRES"].ToString().Trim();
                        obj.TIPO_DOCUMENTO = dr["TIPO_DOCUMENTO"] == DBNull.Value ? string.Empty : dr["TIPO_DOCUMENTO"].ToString().Trim();
                        obj.NRO_DOCUMENTO = dr["NRO_DOCUMENTO"] == DBNull.Value ? string.Empty : dr["NRO_DOCUMENTO"].ToString().Trim();
                        obj.CORREO_ELECTRONICO = dr["CORREO_ELECTRONICO"] == DBNull.Value ? string.Empty : dr["CORREO_ELECTRONICO"].ToString().Trim();
                        obj.CELULAR = dr["CELULAR"] == DBNull.Value ? string.Empty : dr["CELULAR"].ToString().Trim();
                        obj.DIRECCION = dr["DIRECCION"] == DBNull.Value ? string.Empty : dr["DIRECCION"].ToString().Trim();
                        obj.DISTRITO = dr["DISTRITO"] == DBNull.Value ? string.Empty : dr["DISTRITO"].ToString().Trim();
                        obj.PROVINCIA = dr["PROVINCIA"] == DBNull.Value ? string.Empty : dr["PROVINCIA"].ToString().Trim();
                        obj.DEPARTAMENTO = dr["DEPARTAMENTO"] == DBNull.Value ? string.Empty : dr["DEPARTAMENTO"].ToString().Trim();
                        obj.NACIMIENTO = dr["NACIMIENTO"] == DBNull.Value ? string.Empty : dr["NACIMIENTO"].ToString();
                        obj.SUMA_ASEGURADA_COVER1 = dr["SUMA_ASEGURADA_COVER1"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["SUMA_ASEGURADA_COVER1"].ToString());
                        obj.SUMA_ASEGURADA_COVER2 = dr["SUMA_ASEGURADA_COVER2"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["SUMA_ASEGURADA_COVER2"].ToString());
                        obj.PRIMERA_PRIMA = dr["PRIMERA_PRIMA"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PRIMERA_PRIMA"].ToString());
                        obj.PRIMA_MENSUAL = dr["PRIMA_MENSUAL"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PRIMA_MENSUAL"].ToString());
                        obj.PRIMERA_PRIMA_ANUAL = dr["PRIMERA_PRIMA_ANUAL"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PRIMERA_PRIMA_ANUAL"].ToString());
                        obj.MONTO_PRIMAS = dr["MONTO_PRIMAS"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["MONTO_PRIMAS"].ToString());
                        obj.ABONO = dr["ABONO"] == DBNull.Value ? string.Empty : dr["ABONO"].ToString();
                        obj.FECHA_INICIO_VIGENCIA = dr["FECHA_INICIO"] == DBNull.Value ? string.Empty : dr["FECHA_INICIO"].ToString();
                        obj.FECHA_EMISION = dr["EMISION"] == DBNull.Value ? string.Empty : dr["EMISION"].ToString();
                        obj.TEMPORALIDAD = dr["TEMPORALIDAD"] == DBNull.Value ? 0 : Convert.ToInt32(dr["TEMPORALIDAD"].ToString());
                        obj.MONEDA = dr["MONEDA"] == DBNull.Value ? string.Empty : dr["MONEDA"].ToString().Trim();
                        obj.POR_DEVOLUCION = dr["POR_DEVOLUCION"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["POR_DEVOLUCION"].ToString());
                        obj.COD_SEXO = dr["COD_SEXO"] == DBNull.Value ? string.Empty : dr["COD_SEXO"].ToString();
                        obj.CUOTAS_VENCIDAS = dr["CUOTAS_VENCIDAS"] == DBNull.Value ? 0 : Convert.ToInt32(dr["CUOTAS_VENCIDAS"].ToString());
                        obj.CUOTAS_PAGADAS = dr["CUOTAS_PAGADAS"] == DBNull.Value ? 0 : Convert.ToInt32(dr["CUOTAS_PAGADAS"].ToString());
                        obj.ESTADO_POLIZA = dr["ESTADO_POLIZA"] == DBNull.Value ? string.Empty : dr["ESTADO_POLIZA"].ToString();
                        obj.FECHA_FIN_VIGENCIA = dr["FECHA_FIN_VIGENCIA"] == DBNull.Value ? string.Empty : dr["FECHA_FIN_VIGENCIA"].ToString();
                        obj.FECHA_FALLECIMIENTO = dr["FECHA_FALLECIMIENTO"] == DBNull.Value ? string.Empty : dr["FECHA_FALLECIMIENTO"].ToString();
                        obj.NRO_RECIBO = dr["NRO_RECIBO"] == DBNull.Value ? 0 : Convert.ToInt64(dr["NRO_RECIBO"].ToString());
                        obj.NRO_BOLETA = dr["NRO_BOLETA"] == DBNull.Value ? string.Empty : dr["NRO_BOLETA"].ToString();
                        obj.ESTADO_BOLETA = dr["ESTADO_BOLETA"] == DBNull.Value ? string.Empty : dr["ESTADO_BOLETA"].ToString();
                        obj.FECHA_BOLETA = dr["FECHA_BOLETA"] == DBNull.Value ? string.Empty : dr["FECHA_BOLETA"].ToString();
                        obj.MONTO_DEVOLUCION = dr["MONTO_DEVOLUCION"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["MONTO_DEVOLUCION"].ToString());
                        obj.JEFE = dr["JEFE"] == DBNull.Value ? string.Empty : dr["JEFE"].ToString();
                        obj.SUPERVISOR = dr["SUPERVISOR"] == DBNull.Value ? string.Empty : dr["SUPERVISOR"].ToString();
                        obj.DOC_ASESOR_VENTA = dr["DOC_ASESOR_VENTA"] == DBNull.Value ? string.Empty : dr["DOC_ASESOR_VENTA"].ToString();
                        obj.ASESOR_VENTA = dr["ASESOR_VENTA"] == DBNull.Value ? string.Empty : dr["ASESOR_VENTA"].ToString();
                        obj.DOC_ASESOR_MANT = dr["DOC_ASESOR_MANT"] == DBNull.Value ? string.Empty : dr["DOC_ASESOR_MANT"].ToString();
                        obj.ASESOR_MANT = dr["ASESOR_MANT"] == DBNull.Value ? string.Empty : dr["ASESOR_MANT"].ToString();
                        obj.COMISION = dr["COMISION"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["COMISION"].ToString());
                        obj.COMISION_PAGADA = dr["COMISION_PAGADA"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["COMISION_PAGADA"].ToString());
                        obj.MONTO_FACTURADO = dr["MONTO_FACTURADO"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["MONTO_FACTURADO"].ToString());
                        obj.MONTO_NO_DEVENGADO = dr["MONTO_NO_DEVENGADO"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["MONTO_NO_DEVENGADO"].ToString());
                        obj.ANUALIDAD = dr["ANUALIDAD"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ANUALIDAD"].ToString());
                        obj.FECHA_ANULACION = dr["FECHA_ANULACION"] == DBNull.Value ? string.Empty : dr["FECHA_ANULACION"].ToString();
                        obj.EXTORNO_PRIMAS = dr["EXTORNO_PRIMAS"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["EXTORNO_PRIMAS"].ToString());
                        obj.EXTORNO_ANUAL = dr["EXTORNO_ANUAL"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["EXTORNO_ANUAL"].ToString());
                        obj.MONEDA_EXTORNO = dr["MONEDA_EXTORNO"] == DBNull.Value ? string.Empty : dr["MONEDA_EXTORNO"].ToString();
                        obj.EXTORNO_COMISION = dr["EXTORNO_COMISION"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["EXTORNO_COMISION"].ToString());
                        obj.NOTA_CREDITO = dr["NOTA_CREDITO"] == DBNull.Value ? string.Empty : dr["NOTA_CREDITO"].ToString();
                        obj.FECHA_NOTA_CREDITO = dr["FECHA_NOTA_CREDITO"] == DBNull.Value ? string.Empty : dr["FECHA_NOTA_CREDITO"].ToString();
                        obj.BOLETA_ASOCIADA_NC = dr["BOLETA_ASOCIADA_NC"] == DBNull.Value ? string.Empty : dr["BOLETA_ASOCIADA_NC"].ToString();
                        obj.CONVENIO_NEGATIVO = dr["CONVENIO_NEGATIVO"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["CONVENIO_NEGATIVO"].ToString());
                        obj.ANULACIÓN_CONVENIO_NEGATIVO = dr["ANULACIÓN_CONVENIO_NEGATIVO"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["ANULACIÓN_CONVENIO_NEGATIVO"].ToString());
                        reportAnnul.Add(obj);
                    }
                }

                ELog.CloseConnection(dr);

                dataReport.reportVig = reportVig;
                dataReport.reportAnnul = reportAnnul;

                response.TotalRowNumber = reportVig.Count + reportAnnul.Count;
                response.GenericResponse = dataReport;

            }
            catch (Exception ex)
            {
                LogControl.save("ReportOperationVDP", ex.ToString(), "3");
            }
            return response;
        }

        //........................INICIO REPORTE TECNICO VDP..................................
        public ResponseReportATPVM ReportTecVDP(RequestReportTecVDP request)
        {
            ResponseReportATPVM response = new ResponseReportATPVM();
            string nameStore = "INSUDB.SP_REPORT_TEC_VDP";
            List<OracleParameter> oracleParameterList = new List<OracleParameter>();
            List<ReportTecVDP> dataReport = new List<ReportTecVDP>();
            try
            {
                if (request.dStart_Date.HasValue)
                    oracleParameterList.Add(new OracleParameter("P_DSTARTDATE", OracleDbType.Date, (object)request.dStart_Date, ParameterDirection.Input));
                else
                    oracleParameterList.Add(new OracleParameter("P_DSTARTDATE", OracleDbType.Date, (object)DBNull.Value, ParameterDirection.Input));

                if (request.dExpir_Dat.HasValue)
                    oracleParameterList.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, (object)request.dExpir_Dat, ParameterDirection.Input));
                else
                    oracleParameterList.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, (object)DBNull.Value, ParameterDirection.Input));

                string[] nameCursor = new string[1] { "C_POLICY" };
                oracleParameterList.Add(new OracleParameter(nameCursor[0], OracleDbType.RefCursor, ParameterDirection.Output));

                OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(nameStore, nameCursor, oracleParameterList);

                if (dr.HasRows)
                {
                    while (dr.Read())

                    {
                        ReportTecVDP obj = new ReportTecVDP();
                        obj.POLIZA = dr["POLIZA"] == DBNull.Value ? 0 : Convert.ToInt64(dr["POLIZA"].ToString());
                        obj.NOMBRES = dr["NOMBRES"] == DBNull.Value ? string.Empty : dr["NOMBRES"].ToString().Trim();
                        obj.TIPO_DOCUMENTO = dr["TIPO_DOCUMENTO"] == DBNull.Value ? string.Empty : dr["TIPO_DOCUMENTO"].ToString().Trim();
                        obj.NRO_DOCUMENTO = dr["NRO_DOCUMENTO"] == DBNull.Value ? string.Empty : dr["NRO_DOCUMENTO"].ToString().Trim();
                        obj.NACIMIENTO = dr["NACIMIENTO"] == DBNull.Value ? string.Empty : dr["NACIMIENTO"].ToString();
                        obj.SUMA_ASEGURADA_COVER1 = dr["SUMA_ASEGURADA_COVER1"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["SUMA_ASEGURADA_COVER1"].ToString());
                        obj.SUMA_ASEGURADA_COVER2 = dr["SUMA_ASEGURADA_COVER2"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["SUMA_ASEGURADA_COVER2"].ToString());
                        obj.PRIMA_MENSUAL = dr["PRIMA_MENSUAL"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PRIMA_MENSUAL"].ToString());
                        obj.PRIMA_ANUAL = dr["PRIMA_ANUAL"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PRIMA_ANUAL"].ToString());
                        obj.PRIMERA_PRIMA = dr["PRIMERA_PRIMA"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PRIMERA_PRIMA"].ToString());
                        obj.ABONO = dr["ABONO"] == DBNull.Value ? string.Empty : dr["ABONO"].ToString();
                        obj.FECHA_INICIO_VIGENCIA = dr["FECHA_INICIO"] == DBNull.Value ? string.Empty : dr["FECHA_INICIO"].ToString();
                        obj.FECHA_EMISION = dr["EMISION"] == DBNull.Value ? string.Empty : dr["EMISION"].ToString();
                        obj.TEMPORALIDAD = dr["TEMPORALIDAD"] == DBNull.Value ? 0 : Convert.ToInt32(dr["TEMPORALIDAD"].ToString());
                        obj.MONEDA = dr["MONEDA"] == DBNull.Value ? string.Empty : dr["MONEDA"].ToString().Trim();
                        obj.POR_DEVOLUCION = dr["POR_DEVOLUCION"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["POR_DEVOLUCION"].ToString());
                        obj.COTIZACION = dr["COTIZACION"] == DBNull.Value ? string.Empty : dr["COTIZACION"].ToString();
                        obj.COD_SEXO = dr["COD_SEXO"] == DBNull.Value ? string.Empty : dr["COD_SEXO"].ToString();
                        obj.CUOTAS_VENCIDAS = dr["CUOTAS_VENCIDAS"] == DBNull.Value ? 0 : Convert.ToInt32(dr["CUOTAS_VENCIDAS"].ToString());
                        obj.VIGENTE = dr["VIGENTE"] == DBNull.Value ? string.Empty : dr["VIGENTE"].ToString();

                        dataReport.Add(obj);
                    }
                }
                ELog.CloseConnection(dr);
                response.TotalRowNumber = dataReport.Count;
                response.GenericResponse = dataReport;
            }
            catch (Exception ex)
            {
                LogControl.save("ReportTecVDP", ex.ToString(), "3");
            }
            return response;
        }

        //........................FIN REPORTE TECNICO VDP..................................

        //...................INICIO REPORTE CONTROL COMERCIAL DIARIO.......................
        public ResponseReportATPVM ReportControlDaily(RequestReportControlDaily request)
        {
            ResponseReportATPVM response = new ResponseReportATPVM();
            string nameStore = "INSUDB.RESUMEN_DAILY_POLICY";
            List<OracleParameter> oracleParameterList = new List<OracleParameter>();

            ReportControlDailys dataReport = new ReportControlDailys();
            List<ReportControlDaily> reportDaily = new List<ReportControlDaily>();
            List<ReportControlDaily> reportDailyTotal = new List<ReportControlDaily>();


            try
            {
                if (request.dStart_Date.HasValue)
                    oracleParameterList.Add(new OracleParameter("P_DSTARTDATE", OracleDbType.Date, (object)request.dStart_Date, ParameterDirection.Input));
                else
                    oracleParameterList.Add(new OracleParameter("P_DSTARTDATE", OracleDbType.Date, (object)DBNull.Value, ParameterDirection.Input));

                if (request.dExpir_Dat.HasValue)
                    oracleParameterList.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, (object)request.dExpir_Dat, ParameterDirection.Input));
                else
                    oracleParameterList.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, (object)DBNull.Value, ParameterDirection.Input));

                string[] nameCursor = new string[2] { "P_RESUMEN", "P_RESUMEN_TOTAL" };
                oracleParameterList.Add(new OracleParameter(nameCursor[0], OracleDbType.RefCursor, ParameterDirection.Output));

                oracleParameterList.Add(new OracleParameter(nameCursor[1], OracleDbType.RefCursor, ParameterDirection.Output));

                OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(nameStore, nameCursor, oracleParameterList);

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        ReportControlDaily obj = new ReportControlDaily();
                        obj.PERIODO = dr["PERIODO"] == DBNull.Value ? string.Empty : dr["PERIODO"].ToString();
                        obj.CANTIDAD_POLIZAS = dr["CANTIDAD_POLIZAS"] == DBNull.Value ? 0 : Convert.ToInt32(dr["CANTIDAD_POLIZAS"].ToString());
                        obj.PRIMA_ANUAL_SOLES = dr["PRIMA_ANUAL_SOLES"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PRIMA_ANUAL_SOLES"].ToString());
                        obj.PRIMA_ANUAL_DOLARES = dr["PRIMA_ANUAL_DOLARES"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PRIMA_ANUAL_DOLARES"].ToString());
                        obj.PRIMA_ANUAL_EQUIVALENTE_SOLES = dr["PRIMA_ANUAL_EQUIVALENTE_SOLES"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PRIMA_ANUAL_EQUIVALENTE_SOLES"].ToString());
                        obj.PRIMA_TOTAL_ANUAL_SOLES = dr["PRIMA_TOTAL_ANUAL_SOLES"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PRIMA_TOTAL_ANUAL_SOLES"].ToString());
                        reportDaily.Add(obj);
                    }

                    dr.NextResult();
                    while (dr.Read())
                    {
                        ReportControlDaily obj = new ReportControlDaily();
                        obj.CANTIDAD_POLIZAS_TOTAL = dr["CANTIDAD_POLIZAS_TOTAL"] == DBNull.Value ? 0 : Convert.ToInt32(dr["CANTIDAD_POLIZAS_TOTAL"].ToString());
                        obj.PRIMA_ANUAL_SOLES_TOTAL = dr["PRIMA_ANUAL_SOLES_TOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PRIMA_ANUAL_SOLES_TOTAL"].ToString());
                        obj.PRIMA_ANUAL_DOLARES_TOTAL = dr["PRIMA_ANUAL_DOLARES_TOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PRIMA_ANUAL_DOLARES_TOTAL"].ToString());
                        obj.PRIMA_ANUAL_EQUIVALENTE_SOLES_TOTAL = dr["PRIMA_ANUAL_EQUIVALENTE_SOLES_TOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PRIMA_ANUAL_EQUIVALENTE_SOLES_TOTAL"].ToString());
                        obj.PRIMA_TOTAL_ANUAL_SOLES_TOTAL = dr["PRIMA_TOTAL_ANUAL_SOLES_TOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PRIMA_TOTAL_ANUAL_SOLES_TOTAL"].ToString());
                        reportDailyTotal.Add(obj);
                    }
                }
                ELog.CloseConnection(dr);
                dataReport.reportDaily = reportDaily;
                dataReport.reportDailyTotal = reportDailyTotal;

                response.TotalRowNumber = reportDaily.Count + reportDailyTotal.Count;
                response.GenericResponse = dataReport;
            }
            catch (Exception ex)
            {
                LogControl.save("ReportControlDaily", ex.ToString(), "3");
            }
            return response;
        }

        //.....................FIN REPORTE CONTROL COMERCIAL DIARIO........................

        //...................INICIO REPORTE CONTROL COMERCIAL MENSUAL......................
        public ResponseReportATPVM ReportControlMonth(RequestReportControlMonth request)
        {
            ResponseReportATPVM response = new ResponseReportATPVM();
            string nameStore = "INSUDB.RESUMEN_MONTH_POLICY";
            List<OracleParameter> oracleParameterList = new List<OracleParameter>();

            ReportControlMonths dataReport = new ReportControlMonths();
            List<ReportControlMonth> reportMonth = new List<ReportControlMonth>();
            List<ReportControlMonth> reportMonthTotal = new List<ReportControlMonth>();
            try
            {
                if (request.dStart_Date.HasValue)
                    oracleParameterList.Add(new OracleParameter("P_DSTARTDATE", OracleDbType.Date, (object)request.dStart_Date, ParameterDirection.Input));
                else
                    oracleParameterList.Add(new OracleParameter("P_DSTARTDATE", OracleDbType.Date, (object)DBNull.Value, ParameterDirection.Input));

                if (request.dExpir_Dat.HasValue)
                    oracleParameterList.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, (object)request.dExpir_Dat, ParameterDirection.Input));
                else
                    oracleParameterList.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, (object)DBNull.Value, ParameterDirection.Input));

                string[] nameCursor = new string[2] { "P_RESUMEN", "P_RESUMEN_TOTAL" };
                oracleParameterList.Add(new OracleParameter(nameCursor[0], OracleDbType.RefCursor, ParameterDirection.Output));

                oracleParameterList.Add(new OracleParameter(nameCursor[1], OracleDbType.RefCursor, ParameterDirection.Output));

                OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(nameStore, nameCursor, oracleParameterList);

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        ReportControlMonth obj = new ReportControlMonth();
                        obj.PERIODO = dr["PERIODO"] == DBNull.Value ? string.Empty : dr["PERIODO"].ToString();
                        obj.CANTIDAD_POLIZAS = dr["CANTIDAD_POLIZAS"] == DBNull.Value ? 0 : Convert.ToInt32(dr["CANTIDAD_POLIZAS"].ToString());
                        obj.PRIMA_ANUAL_SOLES = dr["PRIMA_ANUAL_SOLES"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PRIMA_ANUAL_SOLES"].ToString());
                        obj.PRIMA_ANUAL_DOLARES = dr["PRIMA_ANUAL_DOLARES"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PRIMA_ANUAL_DOLARES"].ToString());
                        obj.PRIMA_ANUAL_EQUIVALENTE_SOLES = dr["PRIMA_ANUAL_EQUIVALENTE_SOLES"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PRIMA_ANUAL_EQUIVALENTE_SOLES"].ToString());
                        obj.PRIMA_TOTAL_ANUAL_SOLES = dr["PRIMA_TOTAL_ANUAL_SOLES"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PRIMA_TOTAL_ANUAL_SOLES"].ToString());
                        reportMonth.Add(obj);
                    }

                    dr.NextResult();
                    while (dr.Read())
                    {
                        ReportControlMonth obj = new ReportControlMonth();
                        obj.CANTIDAD_POLIZAS_TOTAL = dr["CANTIDAD_POLIZAS_TOTAL"] == DBNull.Value ? 0 : Convert.ToInt32(dr["CANTIDAD_POLIZAS_TOTAL"].ToString());
                        obj.PRIMA_ANUAL_SOLES_TOTAL = dr["PRIMA_ANUAL_SOLES_TOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PRIMA_ANUAL_SOLES_TOTAL"].ToString());
                        obj.PRIMA_ANUAL_DOLARES_TOTAL = dr["PRIMA_ANUAL_DOLARES_TOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PRIMA_ANUAL_DOLARES_TOTAL"].ToString());
                        obj.PRIMA_ANUAL_EQUIVALENTE_SOLES_TOTAL = dr["PRIMA_ANUAL_EQUIVALENTE_SOLES_TOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PRIMA_ANUAL_EQUIVALENTE_SOLES_TOTAL"].ToString());
                        obj.PRIMA_TOTAL_ANUAL_SOLES_TOTAL = dr["PRIMA_TOTAL_ANUAL_SOLES_TOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PRIMA_TOTAL_ANUAL_SOLES_TOTAL"].ToString());
                        reportMonthTotal.Add(obj);
                    }
                }
                ELog.CloseConnection(dr);
                dataReport.reportMonth = reportMonth;
                dataReport.reportMonthTotal = reportMonthTotal;

                response.TotalRowNumber = reportMonth.Count + reportMonthTotal.Count;
                response.GenericResponse = dataReport;
            }
            catch (Exception ex)
            {
                LogControl.save("ReportControlMonth", ex.ToString(), "3");
            }
            return response;
        }
        //.....................FIN REPORTE CONTROL COMERCIAL MENSUAL........................

        //...................INICIO REPORTE CONTROL COMERCIAL ANUAL......................
        public ResponseReportATPVM ReportControlYear(RequestReportControlYear request)
        {
            ResponseReportATPVM response = new ResponseReportATPVM();
            string nameStore = "INSUDB.RESUMEN_YEAR_POLICY";
            List<OracleParameter> oracleParameterList = new List<OracleParameter>();

            ReportControlYears dataReport = new ReportControlYears();
            List<ReportControlYear> reportYear = new List<ReportControlYear>();
            List<ReportControlYear> reportYearTotal = new List<ReportControlYear>();
            try
            {
                if (request.dStart_Date.HasValue)
                    oracleParameterList.Add(new OracleParameter("P_DSTARTDATE", OracleDbType.Date, (object)request.dStart_Date, ParameterDirection.Input));
                else
                    oracleParameterList.Add(new OracleParameter("P_DSTARTDATE", OracleDbType.Date, (object)DBNull.Value, ParameterDirection.Input));

                if (request.dExpir_Dat.HasValue)
                    oracleParameterList.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, (object)request.dExpir_Dat, ParameterDirection.Input));
                else
                    oracleParameterList.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, (object)DBNull.Value, ParameterDirection.Input));

                string[] nameCursor = new string[2] { "P_RESUMEN", "P_RESUMEN_TOTAL" };
                oracleParameterList.Add(new OracleParameter(nameCursor[0], OracleDbType.RefCursor, ParameterDirection.Output));

                oracleParameterList.Add(new OracleParameter(nameCursor[1], OracleDbType.RefCursor, ParameterDirection.Output));

                OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(nameStore, nameCursor, oracleParameterList);

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        ReportControlYear obj = new ReportControlYear();
                        obj.PERIODO = dr["PERIODO"] == DBNull.Value ? string.Empty : dr["PERIODO"].ToString();
                        obj.CANTIDAD_POLIZAS = dr["CANTIDAD_POLIZAS"] == DBNull.Value ? 0 : Convert.ToInt32(dr["CANTIDAD_POLIZAS"].ToString());
                        obj.PRIMA_ANUAL_SOLES = dr["PRIMA_ANUAL_SOLES"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PRIMA_ANUAL_SOLES"].ToString());
                        obj.PRIMA_ANUAL_DOLARES = dr["PRIMA_ANUAL_DOLARES"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PRIMA_ANUAL_DOLARES"].ToString());
                        obj.PRIMA_ANUAL_EQUIVALENTE_SOLES = dr["PRIMA_ANUAL_EQUIVALENTE_SOLES"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PRIMA_ANUAL_EQUIVALENTE_SOLES"].ToString());
                        obj.PRIMA_TOTAL_ANUAL_SOLES = dr["PRIMA_TOTAL_ANUAL_SOLES"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PRIMA_TOTAL_ANUAL_SOLES"].ToString());
                        reportYear.Add(obj);
                    }

                    dr.NextResult();
                    while (dr.Read())
                    {
                        ReportControlYear obj = new ReportControlYear();
                        obj.CANTIDAD_POLIZAS_TOTAL = dr["CANTIDAD_POLIZAS_TOTAL"] == DBNull.Value ? 0 : Convert.ToInt32(dr["CANTIDAD_POLIZAS_TOTAL"].ToString());
                        obj.PRIMA_ANUAL_SOLES_TOTAL = dr["PRIMA_ANUAL_SOLES_TOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PRIMA_ANUAL_SOLES_TOTAL"].ToString());
                        obj.PRIMA_ANUAL_DOLARES_TOTAL = dr["PRIMA_ANUAL_DOLARES_TOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PRIMA_ANUAL_DOLARES_TOTAL"].ToString());
                        obj.PRIMA_ANUAL_EQUIVALENTE_SOLES_TOTAL = dr["PRIMA_ANUAL_EQUIVALENTE_SOLES_TOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PRIMA_ANUAL_EQUIVALENTE_SOLES_TOTAL"].ToString());
                        obj.PRIMA_TOTAL_ANUAL_SOLES_TOTAL = dr["PRIMA_TOTAL_ANUAL_SOLES_TOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PRIMA_TOTAL_ANUAL_SOLES_TOTAL"].ToString());
                        reportYearTotal.Add(obj);
                    }
                }
                ELog.CloseConnection(dr);
                dataReport.reportYear = reportYear;
                dataReport.reportYearTotal = reportYearTotal;

                response.TotalRowNumber = reportYear.Count + reportYearTotal.Count;
                response.GenericResponse = dataReport;
            }
            catch (Exception ex)
            {
                LogControl.save("ReportControlYear", ex.ToString(), "3");
            }
            return response;
        }
        //.....................FIN REPORTE CONTROL COMERCIAL ANUAL.........................

        //...................INICIO REPORTE REPORTE REGISTRO POLIZAS...................... 
        public ResponseReportATPVM ReportRegistryPolicies(RequestReportRegistryPolicies request)
        {
            ResponseReportATPVM response = new ResponseReportATPVM();
            string nameStore = "INSUDB.REPORT_REGISTRY_POLICIES_SP";
            List<OracleParameter> oracleParameterList = new List<OracleParameter>();
            List<ReportRegistryPolicies> dataReport = new List<ReportRegistryPolicies>();
            try
            {
                if (request.dStart_Date.HasValue)
                    oracleParameterList.Add(new OracleParameter("P_DSTARTDATE", OracleDbType.Date, (object)request.dStart_Date, ParameterDirection.Input));
                else
                    oracleParameterList.Add(new OracleParameter("P_DSTARTDATE", OracleDbType.Date, (object)DBNull.Value, ParameterDirection.Input));

                if (request.dExpir_Dat.HasValue)
                    oracleParameterList.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, (object)request.dExpir_Dat, ParameterDirection.Input));
                else
                    oracleParameterList.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, (object)DBNull.Value, ParameterDirection.Input));

                string[] nameCursor = new string[1] { "C_POLICY" };
                oracleParameterList.Add(new OracleParameter(nameCursor[0], OracleDbType.RefCursor, ParameterDirection.Output));

                OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(nameStore, nameCursor, oracleParameterList);

                if (dr.HasRows)
                {
                    while (dr.Read())

                    {
                        ReportRegistryPolicies obj = new ReportRegistryPolicies();
                        obj.AÑO_VENTA = dr["AÑO_VENTA"] == DBNull.Value ? string.Empty : dr["AÑO_VENTA"].ToString();
                        obj.MES_VENTA = dr["MES_VENTA"] == DBNull.Value ? string.Empty : dr["MES_VENTA"].ToString();
                        obj.NRO_POLICY = dr["NRO_POLICY"] == DBNull.Value ? 0 : Convert.ToInt64(dr["NRO_POLICY"].ToString());
                        obj.ESTADO_POLIZA = dr["ESTADO_POLIZA"] == DBNull.Value ? string.Empty : dr["ESTADO_POLIZA"].ToString();
                        obj.ASESOR_MANT = dr["ASESOR_MANT"] == DBNull.Value ? string.Empty : dr["ASESOR_MANT"].ToString();
                        obj.DNI_JEFE = dr["DNI_JEFE"] == DBNull.Value ? string.Empty : dr["DNI_JEFE"].ToString();
                        obj.JEFE = dr["JEFE"] == DBNull.Value ? string.Empty : dr["JEFE"].ToString();
                        obj.SUPERVISOR = dr["SUPERVISOR"] == DBNull.Value ? string.Empty : dr["SUPERVISOR"].ToString();
                        obj.DNI_SUPERVISOR = dr["DNI_SUPERVISOR"] == DBNull.Value ? string.Empty : dr["DNI_SUPERVISOR"].ToString();
                        obj.ASESOR_VENTA = dr["ASESOR_VENTA"] == DBNull.Value ? string.Empty : dr["ASESOR_VENTA"].ToString();
                        obj.DOC_ASESOR_VENTA = dr["DOC_ASESOR_VENTA"] == DBNull.Value ? string.Empty : dr["DOC_ASESOR_VENTA"].ToString();
                        obj.NOM_CONTRATANTE = dr["NOM_CONTRATANTE"] == DBNull.Value ? string.Empty : dr["NOM_CONTRATANTE"].ToString();
                        obj.PEP = dr["PEP"] == DBNull.Value ? string.Empty : dr["PEP"].ToString();
                        obj.DNI_CONTRATANTE = dr["DNI_CONTRATANTE"] == DBNull.Value ? string.Empty : dr["DNI_CONTRATANTE"].ToString();
                        obj.FECHA_NACIMIENTO_CONTRATANTE = dr["FECHA_NACIMIENTO_CONTRATANTE"] == DBNull.Value ? string.Empty : dr["FECHA_NACIMIENTO_CONTRATANTE"].ToString();
                        obj.SEXO_CONTRATANTE = dr["SEXO_CONTRATANTE"] == DBNull.Value ? string.Empty : dr["SEXO_CONTRATANTE"].ToString();
                        obj.NOM_ASEGRUADO = dr["NOM_ASEGRUADO"] == DBNull.Value ? string.Empty : dr["NOM_ASEGRUADO"].ToString();
                        obj.PEP2 = dr["PEP2"] == DBNull.Value ? string.Empty : dr["PEP2"].ToString();
                        obj.DNI_ASEGURADO = dr["DNI_ASEGURADO"] == DBNull.Value ? string.Empty : dr["DNI_ASEGURADO"].ToString();
                        obj.FECHA_NACIMIENTO_ASEG = dr["FECHA_NACIMIENTO_ASEG"] == DBNull.Value ? string.Empty : dr["FECHA_NACIMIENTO_ASEG"].ToString();
                        obj.SEXO_ASEGU = dr["SEXO_ASEGU"] == DBNull.Value ? string.Empty : dr["SEXO_ASEGU"].ToString();
                        obj.EDAD_AÑO_FIRMA = dr["EDAD_AÑO_FIRMA"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["EDAD_AÑO_FIRMA"].ToString());
                        obj.MONEDA = dr["MONEDA"] == DBNull.Value ? string.Empty : dr["MONEDA"].ToString();
                        obj.CUOTA_INICIAL = dr["CUOTA_INICIAL"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["CUOTA_INICIAL"].ToString());
                        obj.PRIMA_ANUAL_SOLES = dr["PRIMA_ANUAL_SOLES"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PRIMA_ANUAL_SOLES"].ToString());
                        obj.PRIMA_ANUAL_DOLARES = dr["PRIMA_ANUAL_DOLARES"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PRIMA_ANUAL_DOLARES"].ToString());
                        obj.EQUIVALENTE_SOLES = dr["EQUIVALENTE_SOLES"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["EQUIVALENTE_SOLES"].ToString());
                        obj.MODALIDAD = dr["MODALIDAD"] == DBNull.Value ? 0 : Convert.ToInt32(dr["MODALIDAD"].ToString());
                        obj.PORCENTAJE = dr["PORCENTAJE"] == DBNull.Value ? string.Empty : dr["PORCENTAJE"].ToString();
                        obj.INICIO_VIGENCIA = dr["INICIO_VIGENCIA"] == DBNull.Value ? string.Empty : dr["INICIO_VIGENCIA"].ToString();
                        obj.FECHA_EMISION = dr["FECHA_EMISION"] == DBNull.Value ? string.Empty : dr["FECHA_EMISION"].ToString();
                        obj.FECHA1DEPOS = dr["FECHA1DEPOS"] == DBNull.Value ? string.Empty : dr["FECHA1DEPOS"].ToString();
                        obj.FECHA2DEPOS = dr["FECHA2DEPOS"] == DBNull.Value ? string.Empty : dr["FECHA2DEPOS"].ToString();
                        obj.FECHA_COMISION_1 = dr["FECHA_COMISION_1"] == DBNull.Value ? string.Empty : dr["FECHA_COMISION_1"].ToString();
                        obj.FECHA3DEPOS = dr["FECHA3DEPOS"] == DBNull.Value ? string.Empty : dr["FECHA3DEPOS"].ToString();
                        obj.FECHA_COMISION_2 = dr["FECHA_COMISION_2"] == DBNull.Value ? string.Empty : dr["FECHA_COMISION_2"].ToString();
                        obj.CUOTAS_PAGADAS = dr["CUOTAS_PAGADAS"] == DBNull.Value ? 0 : Convert.ToInt32(dr["CUOTAS_PAGADAS"].ToString());
                        obj.NTOTAL_CUOTAS = dr["NTOTAL_CUOTAS"] == DBNull.Value ? 0 : Convert.ToInt32(dr["NTOTAL_CUOTAS"].ToString());
                        obj.PERIDO1 = dr["PERIDO1"] == DBNull.Value ? string.Empty : dr["PERIDO1"].ToString();
                        obj.FECHAPAGO1 = dr["FECHAPAGO1"] == DBNull.Value ? string.Empty : dr["FECHAPAGO1"].ToString();
                        obj.PERIDO2 = dr["PERIDO2"] == DBNull.Value ? string.Empty : dr["PERIDO2"].ToString();
                        obj.FECHAPAGO2 = dr["FECHAPAGO2"] == DBNull.Value ? string.Empty : dr["FECHAPAGO2"].ToString();
                        obj.PERIDO3 = dr["PERIDO3"] == DBNull.Value ? string.Empty : dr["PERIDO3"].ToString();
                        obj.FECHAPAGO3 = dr["FECHAPAGO3"] == DBNull.Value ? string.Empty : dr["FECHAPAGO3"].ToString();
                        obj.PERIDO4 = dr["PERIDO4"] == DBNull.Value ? string.Empty : dr["PERIDO4"].ToString();
                        obj.FECHAPAGO4 = dr["FECHAPAGO4"] == DBNull.Value ? string.Empty : dr["FECHAPAGO4"].ToString();
                        obj.MOTIVO_ANULACIÓN = dr["MOTIVO_ANULACIÓN"] == DBNull.Value ? string.Empty : dr["MOTIVO_ANULACIÓN"].ToString();
                        obj.FECHA_ANULACIÓN = dr["FECHA_ANULACIÓN"] == DBNull.Value ? string.Empty : dr["FECHA_ANULACIÓN"].ToString();


                        dataReport.Add(obj);
                    }
                }
                ELog.CloseConnection(dr);
                response.TotalRowNumber = dataReport.Count;
                response.GenericResponse = dataReport;
            }
            catch (Exception ex)
            {
                LogControl.save("ReportRegistryPolicies", ex.ToString(), "3");
            }
            return response;
        }
        //.....................FIN REPORTE REPORTE REGISTRO POLIZAS.........................

        //...................INICIO REPORTE REPORTE REGISTRO POLIZAS DETALLE................
        public ResponseReportATPVM ReportRegistryPoliciesDetail(RequestReportRegistryPoliciesDetail request)
        {
            ResponseReportATPVM response = new ResponseReportATPVM();
            string nameStore = "INSUDB.REPORT_DETAIL_REGISTRY_POLICIES_SP";
            List<OracleParameter> oracleParameterList = new List<OracleParameter>();

            ReportRegistryPoliciesDetails dataReport = new ReportRegistryPoliciesDetails();


            List<ReportRegistryPoliciesDetail> ReportPoliciesDetail = new List<ReportRegistryPoliciesDetail>();
            List<ReportRegistryPoliciesDetail> ReportPoliciesDetailTotal = new List<ReportRegistryPoliciesDetail>();
            try
            {
                if (request.nPolicy.HasValue)
                    oracleParameterList.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int64, (object)request.nPolicy, ParameterDirection.Input));
                else
                    oracleParameterList.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int64, (object)DBNull.Value, ParameterDirection.Input));

                string[] nameCursor = new string[2] { "P_REPORT_DETAIL", "P_REPORT_DETAIL_CUOT" };
                oracleParameterList.Add(new OracleParameter(nameCursor[0], OracleDbType.RefCursor, ParameterDirection.Output));

                oracleParameterList.Add(new OracleParameter(nameCursor[1], OracleDbType.RefCursor, ParameterDirection.Output));

                OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(nameStore, nameCursor, oracleParameterList);

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        ReportRegistryPoliciesDetail obj = new ReportRegistryPoliciesDetail();
                        obj.ANO_VENTA = dr["ANO_VENTA"] == DBNull.Value ? 0 : Convert.ToInt64(dr["ANO_VENTA"].ToString());
                        obj.MES_VENTA = dr["MES_VENTA"] == DBNull.Value ? string.Empty : dr["MES_VENTA"].ToString();
                        obj.NRO_POLICY = dr["NRO_POLICY"] == DBNull.Value ? 0 : Convert.ToInt64(dr["NRO_POLICY"].ToString());
                        obj.ESTADO_POLIZA = dr["ESTADO_POLIZA"] == DBNull.Value ? string.Empty : dr["ESTADO_POLIZA"].ToString();
                        obj.ASESOR_MANT = dr["ASESOR_MANT"] == DBNull.Value ? string.Empty : dr["ASESOR_MANT"].ToString();
                        obj.DNI_JEFE = dr["DNI_JEFE"] == DBNull.Value ? string.Empty : dr["DNI_JEFE"].ToString();
                        obj.JEFE = dr["JEFE"] == DBNull.Value ? string.Empty : dr["JEFE"].ToString();
                        obj.SUPERVISOR = dr["SUPERVISOR"] == DBNull.Value ? string.Empty : dr["SUPERVISOR"].ToString();
                        obj.DNI_SUPERVISOR = dr["DNI_SUPERVISOR"] == DBNull.Value ? string.Empty : dr["DNI_SUPERVISOR"].ToString();
                        obj.ASESOR_VENTA = dr["ASESOR_VENTA"] == DBNull.Value ? string.Empty : dr["ASESOR_VENTA"].ToString();
                        obj.DOC_ASESOR_VENTA = dr["DOC_ASESOR_VENTA"] == DBNull.Value ? string.Empty : dr["DOC_ASESOR_VENTA"].ToString();
                        obj.NOM_CONTRATANTE = dr["NOM_CONTRATANTE"] == DBNull.Value ? string.Empty : dr["NOM_CONTRATANTE"].ToString();
                        obj.PEP = dr["PEP"] == DBNull.Value ? string.Empty : dr["PEP"].ToString();
                        obj.DNI_CONTRATANTE = dr["DNI_CONTRATANTE"] == DBNull.Value ? string.Empty : dr["DNI_CONTRATANTE"].ToString();
                        obj.FECHA_NACIMIENTO_CONTRATANTE = dr["FECHA_NACIMIENTO_CONTRATANTE"] == DBNull.Value ? string.Empty : dr["FECHA_NACIMIENTO_CONTRATANTE"].ToString();
                        obj.SEXO_CONTRATANTE = dr["SEXO_CONTRATANTE"] == DBNull.Value ? string.Empty : dr["SEXO_CONTRATANTE"].ToString();
                        obj.NOM_ASEGRUADO = dr["NOM_ASEGRUADO"] == DBNull.Value ? string.Empty : dr["NOM_ASEGRUADO"].ToString();
                        obj.PEP2 = dr["PEP2"] == DBNull.Value ? string.Empty : dr["PEP2"].ToString();
                        obj.DNI_ASEGURADO = dr["DNI_ASEGURADO"] == DBNull.Value ? string.Empty : dr["DNI_ASEGURADO"].ToString();
                        obj.FECHA_NACIMIENTO_ASEG = dr["FECHA_NACIMIENTO_ASEG"] == DBNull.Value ? string.Empty : dr["FECHA_NACIMIENTO_ASEG"].ToString();
                        obj.SEXO_ASEGU = dr["SEXO_ASEGU"] == DBNull.Value ? string.Empty : dr["SEXO_ASEGU"].ToString();
                        obj.EDAD_AÑO_FIRMA = dr["EDAD_AÑO_FIRMA"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["EDAD_AÑO_FIRMA"].ToString());
                        obj.MONEDA = dr["MONEDA"] == DBNull.Value ? string.Empty : dr["MONEDA"].ToString();
                        obj.CUOTA_INICIAL = dr["CUOTA_INICIAL"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["CUOTA_INICIAL"].ToString());
                        obj.PRIMA_MENSUAL = dr["PRIMA_MENSUAL"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PRIMA_MENSUAL"].ToString());
                        obj.PRIMA_ANUAL_SOLES = dr["PRIMA_ANUAL_SOLES"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PRIMA_ANUAL_SOLES"].ToString());
                        obj.PRIMA_ANUAL_DOLARES = dr["PRIMA_ANUAL_DOLARES"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PRIMA_ANUAL_DOLARES"].ToString());
                        obj.EQUIVALENTE_SOLES = dr["EQUIVALENTE_SOLES"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["EQUIVALENTE_SOLES"].ToString());
                        obj.MODALIDAD = dr["MODALIDAD"] == DBNull.Value ? 0 : Convert.ToInt32(dr["MODALIDAD"].ToString());
                        obj.PORCENTAJE = dr["PORCENTAJE"] == DBNull.Value ? string.Empty : dr["PORCENTAJE"].ToString();
                        obj.INICIO_VIGENCIA = dr["INICIO_VIGENCIA"] == DBNull.Value ? string.Empty : dr["INICIO_VIGENCIA"].ToString();
                        obj.FECHA_EMISION = dr["FECHA_EMISION"] == DBNull.Value ? string.Empty : dr["FECHA_EMISION"].ToString();
                        ReportPoliciesDetail.Add(obj);
                    }

                    dr.NextResult();
                    while (dr.Read())
                    {
                        ReportRegistryPoliciesDetail obj = new ReportRegistryPoliciesDetail();
                        obj.NRO_CUOTA = dr["NRO_CUOTA"] == DBNull.Value ? 0 : Convert.ToInt32(dr["NRO_CUOTA"].ToString());
                        obj.PERIODO = dr["PERIODO"] == DBNull.Value ? string.Empty : dr["PERIODO"].ToString();
                        obj.FECHA_DEPOSITO = dr["FECHA_DEPOSITO"] == DBNull.Value ? string.Empty : dr["FECHA_DEPOSITO"].ToString();
                        ReportPoliciesDetailTotal.Add(obj);
                    }
                }
                ELog.CloseConnection(dr);
                dataReport.ReportPoliciesDetail = ReportPoliciesDetail;
                dataReport.ReportPoliciesDetailTotal = ReportPoliciesDetailTotal;

                response.TotalRowNumber = ReportPoliciesDetail.Count + ReportPoliciesDetailTotal.Count;
                response.GenericResponse = dataReport;
            }
            catch (Exception ex)
            {
                LogControl.save("ReportRegistryPoliciesDetail", ex.ToString(), "3");
            }
            return response;
        }
        //.....................FIN REPORTE REPORTE REGISTRO POLIZAS DETALLE.........................
        //...................INICIO REPORTE DE RESERVA Y REASEGUROS......................  
        public ResponseReportATPVM ReportRegistryReserve(RequestReportRegistryReserve request)
        {
            ResponseReportATPVM response = new ResponseReportATPVM();
            string nameStore = "INSUDB.REPORT_RERSERVES_AND_REINSURANCE_SP";
            List<OracleParameter> oracleParameterList = new List<OracleParameter>();
            List<ReportRegistryReserve> dataReport = new List<ReportRegistryReserve>();
            try
            {
                if (request.dStart_Date.HasValue)
                    oracleParameterList.Add(new OracleParameter("P_DSTARTDATE", OracleDbType.Date, (object)request.dStart_Date, ParameterDirection.Input));
                else
                    oracleParameterList.Add(new OracleParameter("P_DSTARTDATE", OracleDbType.Date, (object)DBNull.Value, ParameterDirection.Input));

                if (request.dExpir_Dat.HasValue)
                    oracleParameterList.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, (object)request.dExpir_Dat, ParameterDirection.Input));
                else
                    oracleParameterList.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, (object)DBNull.Value, ParameterDirection.Input));

                string[] nameCursor = new string[1] { "P_REPORT" };
                oracleParameterList.Add(new OracleParameter(nameCursor[0], OracleDbType.RefCursor, ParameterDirection.Output));

                OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(nameStore, nameCursor, oracleParameterList);

                if (dr.HasRows)
                {
                    while (dr.Read())

                    {
                        ReportRegistryReserve obj = new ReportRegistryReserve();
                        obj.NRO_POLICY = dr["NRO_POLICY"] == DBNull.Value ? 0 : Convert.ToInt64(dr["NRO_POLICY"].ToString());
                        obj.NOM_ASEGRUADO = dr["NOM_ASEGRUADO"] == DBNull.Value ? string.Empty : dr["NOM_ASEGRUADO"].ToString();
                        obj.OCUP_ASEGRUADO = dr["OCUP_ASEGRUADO"] == DBNull.Value ? string.Empty : dr["OCUP_ASEGRUADO"].ToString();
                        obj.MONEDA = dr["MONEDA"] == DBNull.Value ? string.Empty : dr["MONEDA"].ToString();
                        obj.ABONO = dr["ABONO"] == DBNull.Value ? string.Empty : dr["ABONO"].ToString();
                        obj.NACIMIENTO = dr["NACIMIENTO"] == DBNull.Value ? string.Empty : dr["NACIMIENTO"].ToString();
                        obj.SEXO = dr["SEXO"] == DBNull.Value ? string.Empty : dr["SEXO"].ToString();
                        obj.TASA = dr["TASA"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["TASA"].ToString());
                        obj.TEMPORALIDAD = dr["TEMPORALIDAD"] == DBNull.Value ? 0 : Convert.ToInt32(dr["TEMPORALIDAD"].ToString());
                        obj.DEVOLUCION = dr["DEVOLUCION"] == DBNull.Value ? string.Empty : dr["DEVOLUCION"].ToString();
                        obj.SUMA_ASEGURADA = dr["SUMA_ASEGURADA"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["SUMA_ASEGURADA"].ToString());
                        obj.PRIMA_MENSUAL = dr["PRIMA_MENSUAL"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PRIMA_MENSUAL"].ToString());
                        obj.INI_VIGENCIA_RECIBO = dr["INI_VIGENCIA_RECIBO"] == DBNull.Value ? string.Empty : dr["INI_VIGENCIA_RECIBO"].ToString();
                        obj.FIN_VIGENCIA_RECIBO = dr["FIN_VIGENCIA_RECIBO"] == DBNull.Value ? string.Empty : dr["FIN_VIGENCIA_RECIBO"].ToString();
                        obj.INI_VIGENCIA_POLIZA = dr["INI_VIGENCIA_POLIZA"] == DBNull.Value ? string.Empty : dr["INI_VIGENCIA_POLIZA"].ToString();
                        dataReport.Add(obj);
                    }
                }
                ELog.CloseConnection(dr);
                response.TotalRowNumber = dataReport.Count;
                response.GenericResponse = dataReport;
            }
            catch (Exception ex)
            {
                LogControl.save("ReportRegistryReserve", ex.ToString(), "3");
            }
            return response;
        }
        //.....................FIN REPORTE DE RESERVA Y REASEGUROS.........................

        public ResponseReportATPVM ReportClaimATP(RequestReportClaimATP request)
        {
            ResponseReportATPVM response = new ResponseReportATPVM();
            //string nameStore = "INSUDB.PKG_ATP.SP_ATP_REPORT_DATA";
            string nameStore = "INSUDB.SP_ATP_REPORT_CLAIM";
            List<OracleParameter> oracleParameterList = new List<OracleParameter>();
            List<ReportClaimATP> dataReport = new List<ReportClaimATP>();
            try
            {
                if (!string.IsNullOrEmpty(request.sDocument))
                    oracleParameterList.Add(new OracleParameter("P_NRODOC", OracleDbType.Varchar2, (object)request.sDocument, ParameterDirection.Input));
                else
                    oracleParameterList.Add(new OracleParameter("P_NRODOC", OracleDbType.Varchar2, (object)DBNull.Value, ParameterDirection.Input));

                if (request.nPolicy.HasValue)
                    oracleParameterList.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int64, (object)request.nPolicy, ParameterDirection.Input));
                else
                    oracleParameterList.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int64, (object)DBNull.Value, ParameterDirection.Input));

                if (!string.IsNullOrEmpty(request.sNombres))
                    oracleParameterList.Add(new OracleParameter("P_NOMBRES", OracleDbType.Varchar2, (object)request.sNombres, ParameterDirection.Input));
                else
                    oracleParameterList.Add(new OracleParameter("P_NOMBRES", OracleDbType.Varchar2, (object)DBNull.Value, ParameterDirection.Input));

                string[] nameCursor = new string[1] { "C_POLICY" };
                oracleParameterList.Add(new OracleParameter(nameCursor[0], OracleDbType.RefCursor, ParameterDirection.Output));

                OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(nameStore, nameCursor, oracleParameterList);

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        ReportClaimATP obj = new ReportClaimATP();
                        obj.DOC_CONTRATANTE = dr["DOC_CONTRATANTE"] == DBNull.Value ? string.Empty : dr["DOC_CONTRATANTE"].ToString().Trim();
                        obj.CONTRATANTE = dr["CONTRATANTE"] == DBNull.Value ? string.Empty : dr["CONTRATANTE"].ToString().Trim();
                        obj.POLIZA = dr["POLIZA"] == DBNull.Value ? 0 : Convert.ToInt64(dr["POLIZA"].ToString());
                        obj.PRODUCTO = dr["PRODUCTO"] == DBNull.Value ? string.Empty : dr["PRODUCTO"].ToString().Trim();
                        obj.PERIODICIDAD = dr["PERIODICIDAD"] == DBNull.Value ? 0 : Convert.ToInt32(dr["PERIODICIDAD"].ToString());
                        obj.INICIO_VIGENCIA = dr["INICIO_VIGENCIA"] == DBNull.Value ? string.Empty : dr["INICIO_VIGENCIA"].ToString().Trim();
                        obj.FIN_VIGENCIA = dr["FIN_VIGENCIA"] == DBNull.Value ? string.Empty : dr["FIN_VIGENCIA"].ToString().Trim();
                        obj.SUMA_ASEGURADA_COV1 = dr["SUMA_ASEGURADA_COV1"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["SUMA_ASEGURADA_COV1"].ToString());
                        obj.SUMA_ASEGURADA_COV2 = dr["SUMA_ASEGURADA_COV2"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["SUMA_ASEGURADA_COV2"].ToString());
                        obj.MONEDA = dr["MONEDA"] == DBNull.Value ? string.Empty : dr["MONEDA"].ToString().Trim();
                        obj.PERIODO_GRACIA = dr["PERIODO_GRACIA"] == DBNull.Value ? 0 : Convert.ToInt32(dr["PERIODO_GRACIA"].ToString());
                        obj.BENEFICIARIOS = dr["BENEFICIARIOS"] == DBNull.Value ? string.Empty : dr["BENEFICIARIOS"].ToString().Trim();
                        obj.ESTADO_COBRO = dr["ESTADO_COBRO"] == DBNull.Value ? string.Empty : dr["ESTADO_COBRO"].ToString().Trim();
                        obj.FECHA_ESTADO = dr["FECHA_ESTADO"] == DBNull.Value ? string.Empty : dr["FECHA_ESTADO"].ToString();
                        obj.NRO_RECIBO = dr["NRO_RECIBO"] == DBNull.Value ? 0 : Convert.ToInt64(dr["NRO_RECIBO"].ToString());
                        obj.NRO_COMPROBANTE = dr["NRO_COMPROBANTE"] == DBNull.Value ? 0 : Convert.ToInt64(dr["NRO_COMPROBANTE"].ToString());
                        obj.PRIMA_MENSUAL = dr["PRIMA_MENSUAL"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PRIMA_MENSUAL"].ToString());
                        obj.ESTADO_POLIZA = dr["ESTADO_POLIZA"] == DBNull.Value ? string.Empty : dr["ESTADO_POLIZA"].ToString().Trim();
                        obj.FECHA_ESTADO_POLIZA = dr["FECHA_ESTADO_POLIZA"] == DBNull.Value ? string.Empty : dr["FECHA_ESTADO_POLIZA"].ToString();
                        obj.FECHA_EMISION_SUSP = dr["FECHA_EMISION_SUSP"] == DBNull.Value ? string.Empty : dr["FECHA_EMISION_SUSP"].ToString();
                        obj.FECHA_ENVIO_SUSP = dr["FECHA_ENVIO_SUSP"] == DBNull.Value ? string.Empty : dr["FECHA_ENVIO_SUSP"].ToString();
                        obj.CORREO = dr["CORREO"] == DBNull.Value ? string.Empty : dr["CORREO"].ToString().Trim();
                        dataReport.Add(obj);
                    }
                }

                ELog.CloseConnection(dr);
                response.TotalRowNumber = dataReport.Count;
                response.GenericResponse = dataReport;
            }
            catch (Exception ex)
            {
                LogControl.save("ReportClaimATP", ex.ToString(), "3");
            }
            return response;
        }

        public ResponseReportATPVM ReportPersistVDP(RequestReportPersistVDP request)
        {
            ResponseReportATPVM response = new ResponseReportATPVM();
            string nameStore = "INSUDB.SP_VDP_REPORT_PERSIST";
            List<OracleParameter> oracleParameterList = new List<OracleParameter>();
            List<ReportPersistVDP> dataReport = new List<ReportPersistVDP>();
            try
            {
                if (request.dProcess_Date.HasValue)
                    oracleParameterList.Add(new OracleParameter("P_DPROCESSDATE", OracleDbType.Date, (object)request.dProcess_Date, ParameterDirection.Input));
                else
                    oracleParameterList.Add(new OracleParameter("P_DPROCESSDATE", OracleDbType.Date, (object)DBNull.Value, ParameterDirection.Input));

                string[] nameCursor = new string[1] { "C_POLICY" };
                oracleParameterList.Add(new OracleParameter(nameCursor[0], OracleDbType.RefCursor, ParameterDirection.Output));

                OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(nameStore, nameCursor, oracleParameterList);

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        ReportPersistVDP obj = new ReportPersistVDP();
                        obj.DOC_ASESOR = dr["DOC_ASESOR"] == DBNull.Value ? string.Empty : dr["DOC_ASESOR"].ToString().Trim();
                        obj.ASESOR = dr["ASESOR"] == DBNull.Value ? string.Empty : dr["ASESOR"].ToString().Trim();
                        obj.PERS_ASESOR_3 = dr["PERS_ASESOR_3"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PERS_ASESOR_3"].ToString());
                        obj.DOC_SUPER = dr["DOC_SUPER"] == DBNull.Value ? string.Empty : dr["DOC_SUPER"].ToString().Trim();
                        obj.SUPER = dr["SUPER"] == DBNull.Value ? string.Empty : dr["SUPER"].ToString().Trim();
                        obj.PERS_SUPER_3 = dr["PERS_SUPER_3"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PERS_SUPER_3"].ToString());
                        obj.DOC_JEFE = dr["DOC_JEFE"] == DBNull.Value ? string.Empty : dr["DOC_JEFE"].ToString().Trim();
                        obj.JEFE = dr["JEFE"] == DBNull.Value ? string.Empty : dr["JEFE"].ToString().Trim();
                        obj.PERS_JEFE_3 = dr["PERS_JEFE_3"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PERS_JEFE_3"].ToString());
                        dataReport.Add(obj);
                    }
                }

                ELog.CloseConnection(dr);
                response.TotalRowNumber = dataReport.Count;
                response.GenericResponse = dataReport;
            }
            catch (Exception ex)
            {
                LogControl.save("ReportPersistVDP", ex.ToString(), "3");
            }
            return response;
        }

        //REPORTE TECNICO DESGRAVAMEN CON DEVOLUCION
        public ResponseReportATPVM ReportTecnic(RequestReportTecnic request)
        {
            ResponseReportATPVM response = new ResponseReportATPVM();
            string nameStore = "INSUDB.SP_REPORT_TEC_DESGRAVAMEN_DEVOLUCION";
            List<OracleParameter> oracleParameterList = new List<OracleParameter>();
            List<ReportTecnic> dataReport = new List<ReportTecnic>();
            try
            {
                if (request.dStart_Date.HasValue)
                    oracleParameterList.Add(new OracleParameter("P_DSTARTDATE", OracleDbType.Date, (object)request.dStart_Date, ParameterDirection.Input));
                else
                    oracleParameterList.Add(new OracleParameter("P_DSTARTDATE", OracleDbType.Date, (object)DBNull.Value, ParameterDirection.Input));

                if (request.dExpir_Dat.HasValue)
                    oracleParameterList.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, (object)request.dExpir_Dat, ParameterDirection.Input));
                else
                    oracleParameterList.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, (object)DBNull.Value, ParameterDirection.Input));

                string[] nameCursor = new string[1] { "C_POLICY" };
                oracleParameterList.Add(new OracleParameter(nameCursor[0], OracleDbType.RefCursor, ParameterDirection.Output));

                OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(nameStore, nameCursor, oracleParameterList);

                if (dr.HasRows)
                {
                    while (dr.Read())

                    {
                        ReportTecnic obj = new ReportTecnic();
                        obj.POLIZA = dr["POLIZA"] == DBNull.Value ? 0 : Convert.ToInt64(dr["POLIZA"].ToString());
                        obj.CERTIFICADO = dr["CERTIFICADO"] == DBNull.Value ? 0 : Convert.ToInt64(dr["CERTIFICADO"].ToString());
                        obj.NOMBRES = dr["NOMBRES"] == DBNull.Value ? string.Empty : dr["NOMBRES"].ToString().Trim();
                        obj.TIPO_DOC = dr["TIPO_DOC"] == DBNull.Value ? string.Empty : dr["TIPO_DOC"].ToString().Trim();
                        obj.NRO_DOC = dr["NRO_DOC"] == DBNull.Value ? string.Empty : dr["NRO_DOC"].ToString().Trim();
                        obj.ASEGURADO = dr["ASEGURADO"] == DBNull.Value ? string.Empty : dr["ASEGURADO"].ToString().Trim();
                        obj.TIPO_DOCUMENTO = dr["TIPO_DOCUMENTO"] == DBNull.Value ? string.Empty : dr["TIPO_DOCUMENTO"].ToString().Trim();
                        obj.NRO_DOCUMENTO = dr["NRO_DOCUMENTO"] == DBNull.Value ? string.Empty : dr["NRO_DOCUMENTO"].ToString().Trim();
                        obj.NACIMIENTO = dr["NACIMIENTO"] == DBNull.Value ? string.Empty : dr["NACIMIENTO"].ToString();
                        obj.SUMA_ASEGURADA_COVER1 = dr["SUMA_ASEGURADA_COVER1"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["SUMA_ASEGURADA_COVER1"].ToString());
                        obj.SUMA_ASEGURADA_COVER2 = dr["SUMA_ASEGURADA_COVER2"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["SUMA_ASEGURADA_COVER2"].ToString());
                        obj.PRIMA_MENSUAL = dr["PRIMA_MENSUAL"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PRIMA_MENSUAL"].ToString());
                        obj.PRIMA_ANUAL = dr["PRIMA_ANUAL"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PRIMA_ANUAL"].ToString());
                        obj.PRIMERA_PRIMA = dr["PRIMERA_PRIMA"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PRIMERA_PRIMA"].ToString());
                        obj.ABONO = dr["ABONO"] == DBNull.Value ? string.Empty : dr["ABONO"].ToString();
                        obj.FECHA_INICIO_VIGENCIA = dr["FECHA_INICIO"] == DBNull.Value ? string.Empty : dr["FECHA_INICIO"].ToString();
                        obj.FECHA_EMISION = dr["EMISION"] == DBNull.Value ? string.Empty : dr["EMISION"].ToString();
                        obj.TEMPORALIDAD = dr["TEMPORALIDAD"] == DBNull.Value ? 0 : Convert.ToInt32(dr["TEMPORALIDAD"].ToString());
                        obj.MONEDA = dr["MONEDA"] == DBNull.Value ? string.Empty : dr["MONEDA"].ToString().Trim();
                        obj.POR_DEVOLUCION = dr["POR_DEVOLUCION"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["POR_DEVOLUCION"].ToString());
                        obj.COTIZACION = dr["COTIZACION"] == DBNull.Value ? string.Empty : dr["COTIZACION"].ToString();
                        obj.COD_SEXO = dr["COD_SEXO"] == DBNull.Value ? string.Empty : dr["COD_SEXO"].ToString();
                        obj.CUOTAS_VENCIDAS = dr["CUOTAS_VENCIDAS"] == DBNull.Value ? 0 : Convert.ToInt32(dr["CUOTAS_VENCIDAS"].ToString());
                        obj.VIGENTE = dr["VIGENTE"] == DBNull.Value ? string.Empty : dr["VIGENTE"].ToString();

                        dataReport.Add(obj);
                    }
                }
                ELog.CloseConnection(dr);
                response.TotalRowNumber = dataReport.Count;
                response.GenericResponse = dataReport;
            }
            catch (Exception ex)
            {
                LogControl.save("ReportTecnic", ex.ToString(), "3");
            }
            return response;
        }

        //REPORTE OPERATIVO DESGRAVAMEN CON DEVOLUCION
        public ResponseReportATPVM ReportOperac(RequestReportOperac request)
        {
            ResponseReportATPVM response = new ResponseReportATPVM();
            string nameStore = "INSUDB.SP_REPORT_OPE_DESGRAVAMEN_DEVOL";
            List<OracleParameter> oracleParameterList = new List<OracleParameter>();
            ReportOperacModel dataReport = new ReportOperacModel();

            List<ReportOperac> reportVig = new List<ReportOperac>();
            List<ReportOperac> reportAnnul = new List<ReportOperac>();

            try
            {
                if (request.dStart_Date.HasValue)
                    oracleParameterList.Add(new OracleParameter("P_DSTARTDATE", OracleDbType.Date, (object)request.dStart_Date, ParameterDirection.Input));
                else
                    oracleParameterList.Add(new OracleParameter("P_DSTARTDATE", OracleDbType.Date, (object)DBNull.Value, ParameterDirection.Input));

                if (request.dExpir_Dat.HasValue)
                    oracleParameterList.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, (object)request.dExpir_Dat, ParameterDirection.Input));
                else
                    oracleParameterList.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, (object)DBNull.Value, ParameterDirection.Input));

                string[] nameCursor = new string[2] { "C_POLICY", "C_POLICY_ANNUL" };
                oracleParameterList.Add(new OracleParameter(nameCursor[0], OracleDbType.RefCursor, ParameterDirection.Output));
                oracleParameterList.Add(new OracleParameter(nameCursor[1], OracleDbType.RefCursor, ParameterDirection.Output));

                OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(nameStore, nameCursor, oracleParameterList);

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        ReportOperac obj = new ReportOperac();
                        obj.POLIZA = dr["POLIZA"] == DBNull.Value ? 0 : Convert.ToInt64(dr["POLIZA"].ToString());
                        obj.CERTIFICADO = dr["CERTIFICADO"] == DBNull.Value ? 0 : Convert.ToInt64(dr["CERTIFICADO"].ToString());
                        obj.NOMBRES = dr["NOMBRES"] == DBNull.Value ? string.Empty : dr["NOMBRES"].ToString().Trim();
                        obj.TIPO_DOC = dr["TIPO_DOC"] == DBNull.Value ? string.Empty : dr["TIPO_DOC"].ToString().Trim();
                        obj.NRO_DOC = dr["NRO_DOC"] == DBNull.Value ? string.Empty : dr["NRO_DOC"].ToString().Trim();
                        obj.ASEGURADO = dr["ASEGURADO"] == DBNull.Value ? string.Empty : dr["ASEGURADO"].ToString().Trim();
                        obj.TIPO_DOCUMENTO = dr["TIPO_DOCUMENTO"] == DBNull.Value ? string.Empty : dr["TIPO_DOCUMENTO"].ToString().Trim();
                        obj.NRO_DOCUMENTO = dr["NRO_DOCUMENTO"] == DBNull.Value ? string.Empty : dr["NRO_DOCUMENTO"].ToString().Trim();
                        obj.CORREO_ELECTRONICO = dr["CORREO_ELECTRONICO"] == DBNull.Value ? string.Empty : dr["CORREO_ELECTRONICO"].ToString().Trim();
                        obj.CELULAR = dr["CELULAR"] == DBNull.Value ? string.Empty : dr["CELULAR"].ToString().Trim();
                        obj.DIRECCION = dr["DIRECCION"] == DBNull.Value ? string.Empty : dr["DIRECCION"].ToString().Trim();
                        obj.DISTRITO = dr["DISTRITO"] == DBNull.Value ? string.Empty : dr["DISTRITO"].ToString().Trim();
                        obj.PROVINCIA = dr["PROVINCIA"] == DBNull.Value ? string.Empty : dr["PROVINCIA"].ToString().Trim();
                        obj.DEPARTAMENTO = dr["DEPARTAMENTO"] == DBNull.Value ? string.Empty : dr["DEPARTAMENTO"].ToString().Trim();
                        obj.NACIMIENTO = dr["NACIMIENTO"] == DBNull.Value ? string.Empty : dr["NACIMIENTO"].ToString();
                        obj.SUMA_ASEGURADA_COVER1 = dr["SUMA_ASEGURADA_COVER1"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["SUMA_ASEGURADA_COVER1"].ToString());
                        obj.SUMA_ASEGURADA_COVER2 = dr["SUMA_ASEGURADA_COVER2"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["SUMA_ASEGURADA_COVER2"].ToString());
                        obj.PRIMERA_PRIMA = dr["PRIMERA_PRIMA"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PRIMERA_PRIMA"].ToString());
                        obj.PRIMA_MENSUAL = dr["PRIMA_MENSUAL"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PRIMA_MENSUAL"].ToString());
                        obj.PRIMERA_PRIMA_ANUAL = dr["PRIMERA_PRIMA_ANUAL"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PRIMERA_PRIMA_ANUAL"].ToString());
                        obj.MONTO_PRIMAS = dr["MONTO_PRIMAS"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["MONTO_PRIMAS"].ToString());
                        obj.ABONO = dr["ABONO"] == DBNull.Value ? string.Empty : dr["ABONO"].ToString();
                        obj.FECHA_INICIO_VIGENCIA = dr["FECHA_INICIO"] == DBNull.Value ? string.Empty : dr["FECHA_INICIO"].ToString();
                        obj.FECHA_EMISION = dr["EMISION"] == DBNull.Value ? string.Empty : dr["EMISION"].ToString();
                        obj.TEMPORALIDAD = dr["TEMPORALIDAD"] == DBNull.Value ? 0 : Convert.ToInt32(dr["TEMPORALIDAD"].ToString());
                        obj.MONEDA = dr["MONEDA"] == DBNull.Value ? string.Empty : dr["MONEDA"].ToString().Trim();
                        obj.POR_DEVOLUCION = dr["POR_DEVOLUCION"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["POR_DEVOLUCION"].ToString());
                        obj.COD_SEXO = dr["COD_SEXO"] == DBNull.Value ? string.Empty : dr["COD_SEXO"].ToString();
                        obj.CUOTAS_VENCIDAS = dr["CUOTAS_VENCIDAS"] == DBNull.Value ? 0 : Convert.ToInt32(dr["CUOTAS_VENCIDAS"].ToString());
                        obj.CUOTAS_PAGADAS = dr["CUOTAS_PAGADAS"] == DBNull.Value ? 0 : Convert.ToInt32(dr["CUOTAS_PAGADAS"].ToString());
                        obj.ESTADO_POLIZA = dr["ESTADO_POLIZA"] == DBNull.Value ? string.Empty : dr["ESTADO_POLIZA"].ToString();
                        obj.FECHA_FIN_VIGENCIA = dr["FECHA_FIN_VIGENCIA"] == DBNull.Value ? string.Empty : dr["FECHA_FIN_VIGENCIA"].ToString();
                        obj.FECHA_FALLECIMIENTO = dr["FECHA_FALLECIMIENTO"] == DBNull.Value ? string.Empty : dr["FECHA_FALLECIMIENTO"].ToString();
                        obj.NRO_RECIBO = dr["NRO_RECIBO"] == DBNull.Value ? 0 : Convert.ToInt64(dr["NRO_RECIBO"].ToString());
                        obj.NRO_BOLETA = dr["NRO_BOLETA"] == DBNull.Value ? string.Empty : dr["NRO_BOLETA"].ToString();
                        obj.ESTADO_BOLETA = dr["ESTADO_BOLETA"] == DBNull.Value ? string.Empty : dr["ESTADO_BOLETA"].ToString();
                        obj.FECHA_BOLETA = dr["FECHA_BOLETA"] == DBNull.Value ? string.Empty : dr["FECHA_BOLETA"].ToString();
                        obj.MONTO_DEVOLUCION = dr["MONTO_DEVOLUCION"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["MONTO_DEVOLUCION"].ToString());
                        obj.JEFE = dr["JEFE"] == DBNull.Value ? string.Empty : dr["JEFE"].ToString();
                        obj.SUPERVISOR = dr["SUPERVISOR"] == DBNull.Value ? string.Empty : dr["SUPERVISOR"].ToString();
                        obj.DOC_ASESOR_VENTA = dr["DOC_ASESOR_VENTA"] == DBNull.Value ? string.Empty : dr["DOC_ASESOR_VENTA"].ToString();
                        obj.ASESOR_VENTA = dr["ASESOR_VENTA"] == DBNull.Value ? string.Empty : dr["ASESOR_VENTA"].ToString();
                        obj.DOC_ASESOR_MANT = dr["DOC_ASESOR_MANT"] == DBNull.Value ? string.Empty : dr["DOC_ASESOR_MANT"].ToString();
                        obj.ASESOR_MANT = dr["ASESOR_MANT"] == DBNull.Value ? string.Empty : dr["ASESOR_MANT"].ToString();
                        obj.COMISION = dr["COMISION"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["COMISION"].ToString());
                        obj.COMISION_PAGADA = dr["COMISION_PAGADA"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["COMISION_PAGADA"].ToString());
                        obj.MONTO_FACTURADO = dr["MONTO_FACTURADO"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["MONTO_FACTURADO"].ToString());
                        obj.MONTO_NO_DEVENGADO = dr["MONTO_NO_DEVENGADO"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["MONTO_NO_DEVENGADO"].ToString());
                        obj.ANUALIDAD = dr["ANUALIDAD"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ANUALIDAD"].ToString());
                        obj.NUMERO_Cuotas_Devengadas = dr["NUMERO_Cuotas_Devengadas"] == DBNull.Value ? 0 : Convert.ToInt32(dr["NUMERO_Cuotas_Devengadas"].ToString());
                        obj.NUMERO_MESES_NO_DEVENGADAS = dr["NUMERO_MESES_NO_DEVENGADAS"] == DBNull.Value ? 0 : Convert.ToInt32(dr["NUMERO_MESES_NO_DEVENGADAS"].ToString());
                        reportVig.Add(obj);
                    }

                    dr.NextResult();
                    while (dr.Read())
                    {
                        ReportOperac obj = new ReportOperac();
                        obj.POLIZA = dr["POLIZA"] == DBNull.Value ? 0 : Convert.ToInt64(dr["POLIZA"].ToString());
                        obj.CERTIFICADO = dr["CERTIFICADO"] == DBNull.Value ? 0 : Convert.ToInt64(dr["CERTIFICADO"].ToString());
                        obj.NOMBRES = dr["NOMBRES"] == DBNull.Value ? string.Empty : dr["NOMBRES"].ToString().Trim();
                        obj.TIPO_DOC = dr["TIPO_DOC"] == DBNull.Value ? string.Empty : dr["TIPO_DOC"].ToString().Trim();
                        obj.NRO_DOC = dr["NRO_DOC"] == DBNull.Value ? string.Empty : dr["NRO_DOC"].ToString().Trim();
                        obj.ASEGURADO = dr["ASEGURADO"] == DBNull.Value ? string.Empty : dr["ASEGURADO"].ToString().Trim();
                        obj.TIPO_DOCUMENTO = dr["TIPO_DOCUMENTO"] == DBNull.Value ? string.Empty : dr["TIPO_DOCUMENTO"].ToString().Trim();
                        obj.NRO_DOCUMENTO = dr["NRO_DOCUMENTO"] == DBNull.Value ? string.Empty : dr["NRO_DOCUMENTO"].ToString().Trim();
                        obj.CORREO_ELECTRONICO = dr["CORREO_ELECTRONICO"] == DBNull.Value ? string.Empty : dr["CORREO_ELECTRONICO"].ToString().Trim();
                        obj.CELULAR = dr["CELULAR"] == DBNull.Value ? string.Empty : dr["CELULAR"].ToString().Trim();
                        obj.DIRECCION = dr["DIRECCION"] == DBNull.Value ? string.Empty : dr["DIRECCION"].ToString().Trim();
                        obj.DISTRITO = dr["DISTRITO"] == DBNull.Value ? string.Empty : dr["DISTRITO"].ToString().Trim();
                        obj.PROVINCIA = dr["PROVINCIA"] == DBNull.Value ? string.Empty : dr["PROVINCIA"].ToString().Trim();
                        obj.DEPARTAMENTO = dr["DEPARTAMENTO"] == DBNull.Value ? string.Empty : dr["DEPARTAMENTO"].ToString().Trim();
                        obj.NACIMIENTO = dr["NACIMIENTO"] == DBNull.Value ? string.Empty : dr["NACIMIENTO"].ToString();
                        obj.SUMA_ASEGURADA_COVER1 = dr["SUMA_ASEGURADA_COVER1"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["SUMA_ASEGURADA_COVER1"].ToString());
                        obj.SUMA_ASEGURADA_COVER2 = dr["SUMA_ASEGURADA_COVER2"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["SUMA_ASEGURADA_COVER2"].ToString());
                        obj.PRIMERA_PRIMA = dr["PRIMERA_PRIMA"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PRIMERA_PRIMA"].ToString());
                        obj.PRIMA_MENSUAL = dr["PRIMA_MENSUAL"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PRIMA_MENSUAL"].ToString());
                        obj.PRIMERA_PRIMA_ANUAL = dr["PRIMERA_PRIMA_ANUAL"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PRIMERA_PRIMA_ANUAL"].ToString());
                        obj.MONTO_PRIMAS = dr["MONTO_PRIMAS"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["MONTO_PRIMAS"].ToString());
                        obj.ABONO = dr["ABONO"] == DBNull.Value ? string.Empty : dr["ABONO"].ToString();
                        obj.FECHA_INICIO_VIGENCIA = dr["FECHA_INICIO"] == DBNull.Value ? string.Empty : dr["FECHA_INICIO"].ToString();
                        obj.FECHA_EMISION = dr["EMISION"] == DBNull.Value ? string.Empty : dr["EMISION"].ToString();
                        obj.TEMPORALIDAD = dr["TEMPORALIDAD"] == DBNull.Value ? 0 : Convert.ToInt32(dr["TEMPORALIDAD"].ToString());
                        obj.MONEDA = dr["MONEDA"] == DBNull.Value ? string.Empty : dr["MONEDA"].ToString().Trim();
                        obj.POR_DEVOLUCION = dr["POR_DEVOLUCION"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["POR_DEVOLUCION"].ToString());
                        obj.COD_SEXO = dr["COD_SEXO"] == DBNull.Value ? string.Empty : dr["COD_SEXO"].ToString();
                        obj.CUOTAS_VENCIDAS = dr["CUOTAS_VENCIDAS"] == DBNull.Value ? 0 : Convert.ToInt32(dr["CUOTAS_VENCIDAS"].ToString());
                        obj.CUOTAS_PAGADAS = dr["CUOTAS_PAGADAS"] == DBNull.Value ? 0 : Convert.ToInt32(dr["CUOTAS_PAGADAS"].ToString());
                        obj.ESTADO_POLIZA = dr["ESTADO_POLIZA"] == DBNull.Value ? string.Empty : dr["ESTADO_POLIZA"].ToString();
                        obj.FECHA_FIN_VIGENCIA = dr["FECHA_FIN_VIGENCIA"] == DBNull.Value ? string.Empty : dr["FECHA_FIN_VIGENCIA"].ToString();
                        obj.FECHA_FALLECIMIENTO = dr["FECHA_FALLECIMIENTO"] == DBNull.Value ? string.Empty : dr["FECHA_FALLECIMIENTO"].ToString();
                        obj.NRO_RECIBO = dr["NRO_RECIBO"] == DBNull.Value ? 0 : Convert.ToInt64(dr["NRO_RECIBO"].ToString());
                        obj.NRO_BOLETA = dr["NRO_BOLETA"] == DBNull.Value ? string.Empty : dr["NRO_BOLETA"].ToString();
                        obj.ESTADO_BOLETA = dr["ESTADO_BOLETA"] == DBNull.Value ? string.Empty : dr["ESTADO_BOLETA"].ToString();
                        obj.FECHA_BOLETA = dr["FECHA_BOLETA"] == DBNull.Value ? string.Empty : dr["FECHA_BOLETA"].ToString();
                        obj.MONTO_DEVOLUCION = dr["MONTO_DEVOLUCION"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["MONTO_DEVOLUCION"].ToString());
                        obj.JEFE = dr["JEFE"] == DBNull.Value ? string.Empty : dr["JEFE"].ToString();
                        obj.SUPERVISOR = dr["SUPERVISOR"] == DBNull.Value ? string.Empty : dr["SUPERVISOR"].ToString();
                        obj.DOC_ASESOR_VENTA = dr["DOC_ASESOR_VENTA"] == DBNull.Value ? string.Empty : dr["DOC_ASESOR_VENTA"].ToString();
                        obj.ASESOR_VENTA = dr["ASESOR_VENTA"] == DBNull.Value ? string.Empty : dr["ASESOR_VENTA"].ToString();
                        obj.DOC_ASESOR_MANT = dr["DOC_ASESOR_MANT"] == DBNull.Value ? string.Empty : dr["DOC_ASESOR_MANT"].ToString();
                        obj.ASESOR_MANT = dr["ASESOR_MANT"] == DBNull.Value ? string.Empty : dr["ASESOR_MANT"].ToString();
                        obj.COMISION = dr["COMISION"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["COMISION"].ToString());
                        obj.COMISION_PAGADA = dr["COMISION_PAGADA"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["COMISION_PAGADA"].ToString());
                        obj.MONTO_FACTURADO = dr["MONTO_FACTURADO"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["MONTO_FACTURADO"].ToString());
                        obj.MONTO_NO_DEVENGADO = dr["MONTO_NO_DEVENGADO"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["MONTO_NO_DEVENGADO"].ToString());
                        obj.ANUALIDAD = dr["ANUALIDAD"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ANUALIDAD"].ToString());
                        obj.FECHA_ANULACION = dr["FECHA_ANULACION"] == DBNull.Value ? string.Empty : dr["FECHA_ANULACION"].ToString();
                        obj.EXTORNO_PRIMAS = dr["EXTORNO_PRIMAS"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["EXTORNO_PRIMAS"].ToString());
                        obj.EXTORNO_ANUAL = dr["EXTORNO_ANUAL"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["EXTORNO_ANUAL"].ToString());
                        obj.MONEDA_EXTORNO = dr["MONEDA_EXTORNO"] == DBNull.Value ? string.Empty : dr["MONEDA_EXTORNO"].ToString();
                        obj.EXTORNO_COMISION = dr["EXTORNO_COMISION"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["EXTORNO_COMISION"].ToString());
                        obj.NOTA_CREDITO = dr["NOTA_CREDITO"] == DBNull.Value ? string.Empty : dr["NOTA_CREDITO"].ToString();
                        obj.FECHA_NOTA_CREDITO = dr["FECHA_NOTA_CREDITO"] == DBNull.Value ? string.Empty : dr["FECHA_NOTA_CREDITO"].ToString();
                        obj.BOLETA_ASOCIADA_NC = dr["BOLETA_ASOCIADA_NC"] == DBNull.Value ? string.Empty : dr["BOLETA_ASOCIADA_NC"].ToString();
                        obj.CONVENIO_NEGATIVO = dr["CONVENIO_NEGATIVO"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["CONVENIO_NEGATIVO"].ToString());
                        obj.ANULACIÓN_CONVENIO_NEGATIVO = dr["ANULACIÓN_CONVENIO_NEGATIVO"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["ANULACIÓN_CONVENIO_NEGATIVO"].ToString());
                        reportAnnul.Add(obj);
                    }
                }

                ELog.CloseConnection(dr);

                dataReport.reportVig = reportVig;
                dataReport.reportAnnul = reportAnnul;

                response.TotalRowNumber = reportVig.Count + reportAnnul.Count;
                response.GenericResponse = dataReport;

            }
            catch (Exception ex)
            {
                LogControl.save("ReportOperac", ex.ToString(), "3");
            }
            return response;
        }

        //REPORTE FACTURACION DESGRAVAMEN CON DEVOLUCION
        public ResponseReportATPVM ReportFacturacion(RequestReportFacturacion request)
        {
            ResponseReportATPVM response = new ResponseReportATPVM();
            string nameStore = "INSUDB.SP_REPORT_FACTURACION_DESGRAVAMEN_DEVOL";
            List<OracleParameter> oracleParameterList = new List<OracleParameter>();
            List<ReportFacturacion> dataReport = new List<ReportFacturacion>();
            try
            {
                if (request.dStart_Date.HasValue)
                    oracleParameterList.Add(new OracleParameter("P_DSTARTDATE", OracleDbType.Date, (object)request.dStart_Date, ParameterDirection.Input));
                else
                    oracleParameterList.Add(new OracleParameter("P_DSTARTDATE", OracleDbType.Date, (object)DBNull.Value, ParameterDirection.Input));

                if (request.dExpir_Dat.HasValue)
                    oracleParameterList.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, (object)request.dExpir_Dat, ParameterDirection.Input));
                else
                    oracleParameterList.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, (object)DBNull.Value, ParameterDirection.Input));

                string[] nameCursor = new string[1] { "C_POLICY" };
                oracleParameterList.Add(new OracleParameter(nameCursor[0], OracleDbType.RefCursor, ParameterDirection.Output));

                OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(nameStore, nameCursor, oracleParameterList);

                if (dr.HasRows)
                {
                    while (dr.Read())

                    {
                        ReportFacturacion obj = new ReportFacturacion();
                        obj.PRODUCTO = dr["PRODUCTO"] == DBNull.Value ? string.Empty : dr["PRODUCTO"].ToString().Trim();
                        obj.TIPO = dr["TIPO"] == DBNull.Value ? string.Empty : dr["TIPO"].ToString().Trim();
                        obj.MONEDA = dr["MONEDA"] == DBNull.Value ? string.Empty : dr["MONEDA"].ToString().Trim();
                        obj.CODIGO = dr["CODIGO"] == DBNull.Value ? string.Empty : dr["CODIGO"].ToString().Trim();
                        obj.CONTRATANTE = dr["CONTRATANTE"] == DBNull.Value ? string.Empty : dr["CONTRATANTE"].ToString().Trim();
                        obj.POLIZA = dr["POLIZA"] == DBNull.Value ? 0 : Convert.ToInt64(dr["POLIZA"].ToString());
                        obj.CERTIFICADO = dr["CERTIFICADO"] == DBNull.Value ? 0 : Convert.ToInt64(dr["CERTIFICADO"].ToString());
                        obj.FACTURA = dr["FACTURA"] == DBNull.Value ? string.Empty : dr["FACTURA"].ToString().Trim();
                        obj.EMISION = dr["EMISION"] == DBNull.Value ? string.Empty : dr["EMISION"].ToString();
                        obj.INICIOCOMPROB = dr["INICIOCOMPROB"] == DBNull.Value ? string.Empty : dr["INICIOCOMPROB"].ToString();
                        obj.FINCOMPROB = dr["FINCOMPROB"] == DBNull.Value ? string.Empty : dr["FINCOMPROB"].ToString();
                        obj.ESTADO = dr["ESTADO"] == DBNull.Value ? string.Empty : dr["ESTADO"].ToString().Trim();
                        obj.FECEST = dr["FECEST"] == DBNull.Value ? string.Empty : dr["FECEST"].ToString();
                        obj.GRACIA = dr["GRACIA"] == DBNull.Value ? 0 : Convert.ToInt32(dr["GRACIA"].ToString());
                        obj.NETO = dr["NETO"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["NETO"].ToString());
                        obj.DEREMI = dr["DEREMI"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["DEREMI"].ToString());
                        obj.IGV = dr["IGV"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["IGV"].ToString());
                        obj.TOTAL = dr["TOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["TOTAL"].ToString());
                        obj.RECONOCIMIENTO_DE_INGRESO_EN_MES_ANTERIOR = dr["RECONOCIMIENTO_DE_INGRESO_EN_MES_ANTERIOR"] == DBNull.Value ? string.Empty : dr["RECONOCIMIENTO_DE_INGRESO_EN_MES_ANTERIOR"].ToString();
                        dataReport.Add(obj);
                    }
                }
                ELog.CloseConnection(dr);
                response.TotalRowNumber = dataReport.Count;
                response.GenericResponse = dataReport;
            }
            catch (Exception ex)
            {
                LogControl.save("ReportFacturacion", ex.ToString(), "3");
            }
            return response;
        }


        //REPORTE CONVENIOS DESGRAVAMEN CON DEVOLUCION
        public ResponseReportATPVM ReportConvenios(RequestReportConvenios request)
        {
            ResponseReportATPVM response = new ResponseReportATPVM();
            string nameStore = "INSUDB.SP_REPORT_CONVENIOS_DESGRAVAMEN_DEVOL";
            List<OracleParameter> oracleParameterList = new List<OracleParameter>();
            List<ReportConvenios> dataReport = new List<ReportConvenios>();
            try
            {
                if (request.dStart_Date.HasValue)
                    oracleParameterList.Add(new OracleParameter("P_DSTARTDATE", OracleDbType.Date, (object)request.dStart_Date, ParameterDirection.Input));
                else
                    oracleParameterList.Add(new OracleParameter("P_DSTARTDATE", OracleDbType.Date, (object)DBNull.Value, ParameterDirection.Input));

                if (request.dExpir_Dat.HasValue)
                    oracleParameterList.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, (object)request.dExpir_Dat, ParameterDirection.Input));
                else
                    oracleParameterList.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, (object)DBNull.Value, ParameterDirection.Input));

                string[] nameCursor = new string[1] { "C_POLICY" };
                oracleParameterList.Add(new OracleParameter(nameCursor[0], OracleDbType.RefCursor, ParameterDirection.Output));

                OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(nameStore, nameCursor, oracleParameterList);

                if (dr.HasRows)
                {
                    while (dr.Read())

                    {
                        ReportConvenios obj = new ReportConvenios();
                        obj.PRODUCTO = dr["PRODUCTO"] == DBNull.Value ? string.Empty : dr["PRODUCTO"].ToString().Trim();
                        obj.RIESGO = dr["RIESGO"] == DBNull.Value ? string.Empty : dr["RIESGO"].ToString().Trim();
                        obj.RAMO = dr["RAMO"] == DBNull.Value ? 0 : Convert.ToInt32(dr["RAMO"].ToString());
                        obj.EMISIONCONVENIO = dr["EMISIONCONVENIO"] == DBNull.Value ? string.Empty : dr["EMISIONCONVENIO"].ToString();
                        obj.INICIOCONVENIO = dr["INICIOCONVENIO"] == DBNull.Value ? string.Empty : dr["INICIOCONVENIO"].ToString();
                        obj.FINCONVENIO = dr["FINCONVENIO"] == DBNull.Value ? string.Empty : dr["FINCONVENIO"].ToString();
                        obj.CERTIFICADO = dr["CERTIFICADO"] == DBNull.Value ? 0 : Convert.ToInt64(dr["CERTIFICADO"].ToString());
                        obj.POLIZA = dr["POLIZA"] == DBNull.Value ? 0 : Convert.ToInt64(dr["POLIZA"].ToString());
                        obj.TIPO = dr["TIPO"] == DBNull.Value ? string.Empty : dr["TIPO"].ToString().Trim();
                        obj.COMPROBANTE = dr["COMPROBANTE"] == DBNull.Value ? string.Empty : dr["COMPROBANTE"].ToString().Trim();
                        obj.EMISIONCOMP = dr["EMISIONCOMP"] == DBNull.Value ? string.Empty : dr["EMISIONCOMP"].ToString();
                        obj.GRACIA = dr["GRACIA"] == DBNull.Value ? 0 : Convert.ToInt32(dr["GRACIA"].ToString());
                        obj.FECVENCIMIENTO = dr["FECVENCIMIENTO"] == DBNull.Value ? string.Empty : dr["FECVENCIMIENTO"].ToString();
                        obj.FECPROVISION = dr["FECPROVISION"] == DBNull.Value ? string.Empty : dr["FECPROVISION"].ToString();
                        obj.COMPASOCIADO = dr["COMPASOCIADO"] == DBNull.Value ? string.Empty : dr["COMPASOCIADO"].ToString().Trim();
                        obj.DOCIDENT = dr["DOCIDENT"] == DBNull.Value ? string.Empty : dr["DOCIDENT"].ToString().Trim();
                        obj.CONTRATANTEFACT = dr["CONTRATANTEFACT"] == DBNull.Value ? string.Empty : dr["CONTRATANTEFACT"].ToString().Trim();
                        obj.MONEDA = dr["MONEDA"] == DBNull.Value ? string.Empty : dr["MONEDA"].ToString().Trim();
                        obj.NETO = dr["NETO"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["NETO"].ToString());
                        obj.DEREMI = dr["DEREMI"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["DEREMI"].ToString());
                        obj.IGV = dr["IGV"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["IGV"].ToString());
                        obj.TOTAL = dr["TOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["TOTAL"].ToString());
                        obj.CONTRATANTEPOLIZA = dr["CONTRATANTEPOLIZA"] == DBNull.Value ? string.Empty : dr["CONTRATANTEPOLIZA"].ToString().Trim();
                        obj.INTERMEDIARIO1 = dr["INTERMEDIARIO1"] == DBNull.Value ? string.Empty : dr["INTERMEDIARIO1"].ToString().Trim();
                        obj.COMISION1 = dr["COMISION1"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["COMISION1"].ToString());
                        obj.INTERMEDIARIO1_1 = dr["INTERMEDIARIO1_1"] == DBNull.Value ? string.Empty : dr["INTERMEDIARIO1_1"].ToString().Trim();
                        obj.COMISION2 = dr["COMISION2"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["COMISION2"].ToString());
                        obj.GASTOTECNICO = dr["GASTOTECNICO"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["GASTOTECNICO"].ToString());
                        obj.PAGOASISTENCIAS = dr["PAGOASISTENCIAS"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PAGOASISTENCIAS"].ToString());
                        obj.TIPOCONVENIO = dr["TIPOCONVENIO"] == DBNull.Value ? string.Empty : dr["TIPOCONVENIO"].ToString().Trim();
                        obj.RECIBO = dr["RECIBO"] == DBNull.Value ? 0 : Convert.ToInt64(dr["RECIBO"].ToString());
                        obj.COMENTARIOS = dr["COMENTARIOS"] == DBNull.Value ? string.Empty : dr["COMENTARIOS"].ToString().Trim();
                        dataReport.Add(obj);
                    }
                }
                ELog.CloseConnection(dr);
                response.TotalRowNumber = dataReport.Count;
                response.GenericResponse = dataReport;
            }
            catch (Exception ex)
            {
                LogControl.save("ReportConvenios", ex.ToString(), "3");
            }
            return response;
        }


        //REPORTE COMISIONES DESGRAVAMEN CON DEVOLUCION
        public ResponseReportATPVM ReportComisiones(RequestReportComisiones request)
        {
            ResponseReportATPVM response = new ResponseReportATPVM();
            string nameStore = "INSUDB.SP_REPORT_COMISIONES_DESGRAVAMEN_DEVOL";
            List<OracleParameter> oracleParameterList = new List<OracleParameter>();
            List<ReportComisiones> dataReport = new List<ReportComisiones>();
            try
            {
                if (request.dStart_Date.HasValue)
                    oracleParameterList.Add(new OracleParameter("P_DSTARTDATE", OracleDbType.Date, (object)request.dStart_Date, ParameterDirection.Input));
                else
                    oracleParameterList.Add(new OracleParameter("P_DSTARTDATE", OracleDbType.Date, (object)DBNull.Value, ParameterDirection.Input));

                if (request.dExpir_Dat.HasValue)
                    oracleParameterList.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, (object)request.dExpir_Dat, ParameterDirection.Input));
                else
                    oracleParameterList.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, (object)DBNull.Value, ParameterDirection.Input));

                string[] nameCursor = new string[1] { "C_POLICY" };
                oracleParameterList.Add(new OracleParameter(nameCursor[0], OracleDbType.RefCursor, ParameterDirection.Output));

                OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(nameStore, nameCursor, oracleParameterList);

                if (dr.HasRows)
                {
                    while (dr.Read())

                    {
                        ReportComisiones obj = new ReportComisiones();
                        obj.TIPO = dr["TIPO"] == DBNull.Value ? string.Empty : dr["TIPO"].ToString().Trim();
                        obj.CONTRATANTE = dr["CONTRATANTE"] == DBNull.Value ? string.Empty : dr["CONTRATANTE"].ToString().Trim();
                        obj.INTERMEDIARIO_CANAL = dr["INTERMEDIARIO_CANAL"] == DBNull.Value ? string.Empty : dr["INTERMEDIARIO_CANAL"].ToString().Trim();
                        obj.POLIZA = dr["POLIZA"] == DBNull.Value ? 0 : Convert.ToInt64(dr["POLIZA"].ToString());
                        obj.CODIGO_SBS = dr["CODIGO_SBS"] == DBNull.Value ? string.Empty : dr["CODIGO_SBS"].ToString().Trim();
                        obj.PRODUCTO_ASOCIADO = dr["PRODUCTO_ASOCIADO"] == DBNull.Value ? string.Empty : dr["PRODUCTO_ASOCIADO"].ToString().Trim();
                        obj.RAMO = dr["RAMO"] == DBNull.Value ? 0 : Convert.ToInt32(dr["RAMO"].ToString());
                        obj.MONEDA = dr["MONEDA"] == DBNull.Value ? string.Empty : dr["MONEDA"].ToString().Trim();
                        obj.IMPORTE_COMISION = dr["IMPORTE_COMISION"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["IMPORTE_COMISION"].ToString());
                        obj.IMPORTE_COMISION_EXTORNADO_NC = dr["IMPORTE_COMISION_EXTORNADO_NC"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["IMPORTE_COMISION_EXTORNADO_NC"].ToString());
                        dataReport.Add(obj);
                    }
                }
                ELog.CloseConnection(dr);
                response.TotalRowNumber = dataReport.Count;
                response.GenericResponse = dataReport;
            }
            catch (Exception ex)
            {
                LogControl.save("ReportComisiones", ex.ToString(), "3");
            }
            return response;
        }


        //REPORTE COMISIONES DESGRAVAMEN CON DEVOLUCION
        public ResponseReportATPVM ReportProviComisiones(RequestReportProviComisiones request)
        {
            ResponseReportATPVM response = new ResponseReportATPVM();
            string nameStore = "INSUDB.SP_REPORT_PROVISION_COMISIONES";
            List<OracleParameter> oracleParameterList = new List<OracleParameter>();
            List<ReportProviComisiones> dataReport = new List<ReportProviComisiones>();
            try
            {
                if (request.dStart_Date.HasValue)
                    oracleParameterList.Add(new OracleParameter("P_DSTARTDATE", OracleDbType.Date, (object)request.dStart_Date, ParameterDirection.Input));
                else
                    oracleParameterList.Add(new OracleParameter("P_DSTARTDATE", OracleDbType.Date, (object)DBNull.Value, ParameterDirection.Input));

                if (request.dExpir_Dat.HasValue)
                    oracleParameterList.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, (object)request.dExpir_Dat, ParameterDirection.Input));
                else
                    oracleParameterList.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, (object)DBNull.Value, ParameterDirection.Input));

                oracleParameterList.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int64, (object)request.nBranch, ParameterDirection.Input));
                string[] nameCursor = new string[1] { "C_TABLE" };
                oracleParameterList.Add(new OracleParameter(nameCursor[0], OracleDbType.RefCursor, ParameterDirection.Output));


                OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(nameStore, nameCursor, oracleParameterList);

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        ReportProviComisiones obj = new ReportProviComisiones();

                        obj.TIPO = dr["TIPO"] == DBNull.Value ? string.Empty : dr["TIPO"].ToString().Trim();
                        obj.nbranch = dr["nbranch"] == DBNull.Value ? 0 : Convert.ToInt32(dr["nbranch"].ToString());
                        obj.sbranch = dr["sbranch"] == DBNull.Value ? string.Empty : dr["sbranch"].ToString().Trim();
                        obj.nproduct = dr["nproduct"] == DBNull.Value ? 0 : Convert.ToInt32(dr["nproduct"].ToString());
                        obj.sproduct = dr["sproduct"] == DBNull.Value ? string.Empty : dr["sproduct"].ToString().Trim();
                        obj.npolicy = dr["npolicy"] == DBNull.Value ? 0 : Convert.ToInt64(dr["npolicy"].ToString());
                        obj.nreceipt = dr["nreceipt"] == DBNull.Value ? 0 : Convert.ToInt64(dr["nreceipt"].ToString());
                        obj.sstatus_rec = dr["sstatus_rec"] == DBNull.Value ? string.Empty : dr["sstatus_rec"].ToString().Trim();
                        obj.stype_comp = dr["stype_comp"] == DBNull.Value ? string.Empty : dr["stype_comp"].ToString().Trim();
                        obj.nbillnum = dr["nbillnum"] == DBNull.Value ? 0 : Convert.ToInt64(dr["nbillnum"].ToString());
                        obj.COMPROBANTE = dr["COMPROBANTE"] == DBNull.Value ? string.Empty : dr["COMPROBANTE"].ToString().Trim();
                        obj.sstatus_comp = dr["sstatus_comp"] == DBNull.Value ? string.Empty : dr["sstatus_comp"].ToString().Trim();
                        obj.moneda = dr["moneda"] == DBNull.Value ? string.Empty : dr["moneda"].ToString().Trim();
                        obj.contratante = dr["contratante"] == DBNull.Value ? string.Empty : dr["contratante"].ToString().Trim();
                        obj.dprov_interfaz = dr["dprov_interfaz"].Equals("") ? string.Empty : dr["dprov_interfaz"].ToString();
                        obj.ini_per = dr["ini_per"].Equals("") ? string.Empty : dr["ini_per"].ToString();
                        obj.fin_per = dr["fin_per"].Equals("") ? string.Empty : dr["fin_per"].ToString();
                        obj.dauto_comm = dr["dauto_comm"].Equals("") ? string.Empty : dr["dauto_comm"].ToString();
                        obj.sstate_com = dr["sstate_com"] == DBNull.Value ? string.Empty : dr["sstate_com"].ToString().Trim();
                        obj.ncomm_pago_inter = dr["ncomm_pago_inter"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["ncomm_pago_inter"].ToString());
                        obj.ncomm_pago_comer = dr["ncomm_pago_comer"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["ncomm_pago_comer"].ToString());
                        obj.ncomm_pago_bys = dr["ncomm_pago_bys"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["ncomm_pago_bys"].ToString());
                        obj.nprov_comm_int = dr["nprov_comm_int"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["nprov_comm_int"].ToString());
                        obj.nprov_comm_com = dr["nprov_comm_com"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["nprov_comm_com"].ToString());
                        obj.nprov_com_bys = dr["nprov_com_bys"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["nprov_com_bys"].ToString());
                        obj.suser_auto = dr["suser_auto"] == DBNull.Value ? string.Empty : dr["suser_auto"].ToString().Trim();

                        dataReport.Add(obj);
                    }
                }
                ELog.CloseConnection(dr);
                response.TotalRowNumber = dataReport.Count;
                response.GenericResponse = dataReport;
            }
            catch (Exception ex)
            {
                LogControl.save("ReportProviComisiones", ex.ToString(), "3");
            }
            return response;
        }



        //REPORTE CUENTASXCOBRAR DESGRAVAMEN CON DEVOLUCION
        public ResponseReportATPVM ReportCuentasxcobrar(RequestReportCuentasxcobrar request)
        {
            ResponseReportATPVM response = new ResponseReportATPVM();
            string nameStore = "INSUDB.SP_REPORT_CUENTASXCOBRAR_DESGRAVAMEN_DEVOL";
            List<OracleParameter> oracleParameterList = new List<OracleParameter>();
            List<ReportCuentasxcobrar> dataReport = new List<ReportCuentasxcobrar>();
            try
            {
                if (request.dStart_Date.HasValue)
                    oracleParameterList.Add(new OracleParameter("P_DSTARTDATE", OracleDbType.Date, (object)request.dStart_Date, ParameterDirection.Input));
                else
                    oracleParameterList.Add(new OracleParameter("P_DSTARTDATE", OracleDbType.Date, (object)DBNull.Value, ParameterDirection.Input));

                if (request.dExpir_Dat.HasValue)
                    oracleParameterList.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, (object)request.dExpir_Dat, ParameterDirection.Input));
                else
                    oracleParameterList.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, (object)DBNull.Value, ParameterDirection.Input));

                string[] nameCursor = new string[1] { "C_POLICY" };
                oracleParameterList.Add(new OracleParameter(nameCursor[0], OracleDbType.RefCursor, ParameterDirection.Output));

                OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(nameStore, nameCursor, oracleParameterList);

                if (dr.HasRows)
                {
                    while (dr.Read())

                    {
                        ReportCuentasxcobrar obj = new ReportCuentasxcobrar();
                        obj.PRODUCTO = dr["PRODUCTO"] == DBNull.Value ? string.Empty : dr["PRODUCTO"].ToString().Trim();
                        obj.TIPODOC = dr["TIPODOC"] == DBNull.Value ? string.Empty : dr["TIPODOC"].ToString().Trim();
                        obj.MONEDA = dr["MONEDA"] == DBNull.Value ? string.Empty : dr["MONEDA"].ToString().Trim();
                        obj.DOCUMENTO_IDENTIDAD = dr["DOCUMENTO_IDENTIDAD"] == DBNull.Value ? string.Empty : dr["DOCUMENTO_IDENTIDAD"].ToString().Trim();
                        obj.CONTRATANTE = dr["CONTRATANTE"] == DBNull.Value ? string.Empty : dr["CONTRATANTE"].ToString().Trim();
                        obj.NRO_POLIZA = dr["NRO_POLIZA"] == DBNull.Value ? 0 : Convert.ToInt64(dr["NRO_POLIZA"].ToString());
                        obj.CERTIFICADO = dr["CERTIFICADO"] == DBNull.Value ? 0 : Convert.ToInt64(dr["CERTIFICADO"].ToString());
                        obj.FECHA_INICIO_POLIZA = dr["FECHA_INICIO_POLIZA"] == DBNull.Value ? string.Empty : dr["FECHA_INICIO_POLIZA"].ToString().Trim();
                        obj.FECHA_FIN_POLIZA = dr["FECHA_FIN_POLIZA"] == DBNull.Value ? string.Empty : dr["FECHA_FIN_POLIZA"].ToString();
                        obj.FECHA_INICIO_COMPROBANTE = dr["FECHA_INICIO_COMPROBANTE"] == DBNull.Value ? string.Empty : dr["FECHA_INICIO_COMPROBANTE"].ToString();
                        obj.FECHA_FIN_COMPROBANTE = dr["FECHA_FIN_COMPROBANTE"] == DBNull.Value ? string.Empty : dr["FECHA_FIN_COMPROBANTE"].ToString();
                        obj.COMPROBANTE = dr["COMPROBANTE"] == DBNull.Value ? string.Empty : dr["COMPROBANTE"].ToString().Trim();
                        obj.FECHA_COMPROBANTE = dr["FECHA_COMPROBANTE"] == DBNull.Value ? string.Empty : dr["FECHA_COMPROBANTE"].ToString().Trim();
                        obj.FECHA_VENCIMIENTO = dr["FECHA_VENCIMIENTO"] == DBNull.Value ? string.Empty : dr["FECHA_VENCIMIENTO"].ToString().Trim();
                        obj.ESTADO = dr["ESTADO"] == DBNull.Value ? string.Empty : dr["ESTADO"].ToString().Trim();
                        obj.DIAS = dr["DIAS"] == DBNull.Value ? 0 : Convert.ToInt32(dr["DIAS"].ToString());
                        obj.FECEST = dr["FECEST"] == DBNull.Value ? string.Empty : dr["FECEST"].ToString();
                        obj.NETO = dr["NETO"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["NETO"].ToString());
                        obj.D_E = dr["D_E"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["D_E"].ToString());
                        obj.IGV = dr["IGV"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["IGV"].ToString());
                        obj.TOTAL = dr["TOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["TOTAL"].ToString());
                        dataReport.Add(obj);
                    }
                }
                ELog.CloseConnection(dr);
                response.TotalRowNumber = dataReport.Count;
                response.GenericResponse = dataReport;
            }
            catch (Exception ex)
            {
                LogControl.save("ReportCuentasxcobrar", ex.ToString(), "3");
            }
            return response;
        }




    }
    //FIN JF
}
