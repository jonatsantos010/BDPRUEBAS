/*************************************************************************************************/
/*  NOMBRE              :   NoteCreditRefundDA.CS                                               */
/*  DESCRIPCION         :   Capa DataAccess                                                           */
/*  AUTOR               :   MATERIAGRIS - PEDRO ANTICONA VALLE                                     */
/*  FECHA               :   22-12-2021                                                           */
/*  VERSION             :   1.0 - Generación de NC - PD                 */
/*************************************************************************************************/
using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.Entities.NoteCreditRefundModel.ViewModel;
using WSPlataforma.Util;

namespace WSPlataforma.DA
{
    public class NoteCreditRefundDA2 : ConnectionBase
    {
        #region Filters

        List<PremiumVM> ListPremium = new List<PremiumVM>();

        public List<BranchVM> GetBranches()
        {
            var parameters = new List<OracleParameter>();
            var branches = new List<BranchVM>();
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_NoteCreditSCTR, "SPS_GET_BRANCHES");

            try
            {
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var branch = new BranchVM();
                        branch.NBRANCH = reader["NBRANCH"] == DBNull.Value ? 0 : int.Parse(reader["NBRANCH"].ToString());
                        branch.SBRANCH_DES = reader["SBRANCH_DESCRIPT"] == DBNull.Value ? string.Empty : reader["SBRANCH_DESCRIPT"].ToString();

                        branches.Add(branch);
                    }
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("GetBranches", ex.ToString(), "3");
                throw;
            }

            return branches;
        }
        public List<ProductVM> GetProductsListByBranch(ProductVM data)
        {
            var parameter = new List<OracleParameter>();
            var ListProductByBranch = new List<ProductVM>();
            var sPackageName = string.Format("{0}.{1}", ProcedureName.pkg_NoteCreditSCTR, "SPS_GET_LISTPRODUCT");

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.NBRANCH, ParameterDirection.Input));
                //OUTPUT
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter))
                {
                    ListProductByBranch = dr.ReadRowsList<ProductVM>();
                    ELog.CloseConnection(dr);
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetProductsListByBranch", ex.ToString(), "3");
            }

            return ListProductByBranch;
        }

        public List<ProductVM> GetParameter(ProductVM data)
        {
            var parameter = new List<OracleParameter>();
            var ListProductByBranch = new List<ProductVM>();
            var sPackageName = string.Format("{0}.{1}", ProcedureName.pkg_NoteCreditSCTR, "SPS_GET_LISTPARAMETER");

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NPARAMETER", OracleDbType.Int32, data.NPARAMETER, ParameterDirection.Input));
                //OUTPUT
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter))
                {
                    ListProductByBranch = dr.ReadRowsList<ProductVM>();
                    ELog.CloseConnection(dr);
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetParameter", ex.ToString(), "3");
            }

            return ListProductByBranch;
        }
        public List<DocumentVM> GetDocumentTypeList()
        {

            var parameter = new List<OracleParameter>();
            var documents = new List<DocumentVM>();
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_NoteCreditSCTR, "SPS_GET_TYPE_DOCUMENT");

            try
            {
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameter);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var document = new DocumentVM();
                        document.NIDDOC_TYPE = reader["NIDDOC_TYPE"] == DBNull.Value ? 0 : int.Parse(reader["NIDDOC_TYPE"].ToString());
                        document.SDOC_DES = reader["SDESCRIPT"] == DBNull.Value ? string.Empty : reader["SDESCRIPT"].ToString();

                        documents.Add(document);
                    }
                }

                reader.Close();

            }
            catch (Exception ex)
            {
                LogControl.save("GetDocumentTypeList", ex.ToString(), "3");
            }

            return documents;
        }

        public List<BillVM> GetBillTypeList()
        {
            var parameter = new List<OracleParameter>();
            var bills = new List<BillVM>();
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_NoteCreditSCTR, "SPS_GET_TYPE_BILL");

            try
            {
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameter);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var bill = new BillVM();
                        bill.SBILLTYPE = reader["SBILLTYPE"] == DBNull.Value ? string.Empty : reader["SBILLTYPE"].ToString();
                        bill.SDES_BILL = reader["SDESCRIPT"] == DBNull.Value ? string.Empty : reader["SDESCRIPT"].ToString();

                        bills.Add(bill);
                    }
                }

                reader.Close();

            }
            catch (Exception ex)
            {
                LogControl.save("GetBillTypeList", ex.ToString(), "3");
            }

            return bills;
        }



        public List<PremiumVM> pruebas(PremiumVM data)
        {

            PremiumVM productos = new PremiumVM();

            try
            {

                Console.WriteLine("valores", ListPremium);
                productos = data;
                ListPremium.Add(productos);
                Console.WriteLine("valores", ListPremium);

            }
            catch (Exception ex)
            {
                LogControl.save("pruebas", ex.ToString(), "3");
            }

            return ListPremium;
        }

        public PremiumVMResponse GetListPremium(PremiumVM data)
        {
            var sPackageName = string.Format("{0}.{1}", ProcedureName.pkg_NoteCreditSCTR, "SPS_GET_LISTPREMIUM");
            //var sPackageName = "SPS_GET_LISTPREMIUM";
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<PremiumVM> ListPremium = new List<PremiumVM>();
            PremiumVMResponse Respuesta = new PremiumVMResponse();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, data.NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int32, data.NPOLICY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCERTIF", OracleDbType.Int32, data.NCERTIF, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCLINUMDOCU", OracleDbType.Varchar2, data.SCLINUMDOCU, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SBILLTYPE", OracleDbType.Varchar2, data.idTipCompro, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBILLNUM", OracleDbType.Int32, data.COMPROBANTE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Date, data.DEFFECDATE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, data.DEXPIRDAT, ParameterDirection.Input));
                //OUTPUT
                OracleParameter P_SMESSAGE = new OracleParameter("P_MENSAGE", OracleDbType.Varchar2, string.Empty, ParameterDirection.Output);
                P_SMESSAGE.Size = 4000;
                parameter.Add(P_SMESSAGE);
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));



                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter);

                //if (reader.HasRows)
                //{
                while (reader.Read())
                {
                    var rec = new PremiumVM();
                    rec.SCERTYPE = reader["SCERTYPE"] == DBNull.Value ? "" : reader["SCERTYPE"].ToString();
                    rec.NCERTIF = reader["NCERTIF"] == DBNull.Value ? 0 : Int64.Parse(reader["NCERTIF"].ToString());
                    rec.NBRANCH = reader["NBRANCH"] == DBNull.Value ? 0 : Int32.Parse(reader["NBRANCH"].ToString());
                    rec.NPRODUCT = reader["NPRODUCT"] == DBNull.Value ? 0 : Int32.Parse(reader["NPRODUCT"].ToString());
                    rec.SCLIENT = reader["SCLIENT"] == DBNull.Value ? "" : reader["SCLIENT"].ToString();
                    rec.SDES_BRANCH = reader["SDES_BRANCH"] == DBNull.Value ? "" : reader["SDES_BRANCH"].ToString();
                    rec.SDES_PRODMAST = reader["SDES_PRODMAST"] == DBNull.Value ? "" : reader["SDES_PRODMAST"].ToString();
                    rec.NPOLICY = reader["NPOLICY"] == DBNull.Value ? 0 : Int64.Parse(reader["NPOLICY"].ToString());
                    rec.SCLINUMDOCU = reader["SCLINUMDOCU"] == DBNull.Value ? "" : reader["SCLINUMDOCU"].ToString();
                    rec.SCLIENAME = reader["SCLIENAME"] == DBNull.Value ? "" : reader["SCLIENAME"].ToString();
                    rec.SCOMPROBANTE = reader["COMPROBANTE"] == DBNull.Value ? "" : reader["COMPROBANTE"].ToString();
                    rec.SDES_COMPRO = reader["SDES_COMPRO"] == DBNull.Value ? "" : reader["SDES_COMPRO"].ToString();
                    rec.NRECEIPT = reader["NRECEIPT"] == DBNull.Value ? 0 : Int64.Parse(reader["NRECEIPT"].ToString());
                    rec.DEFFECDATE = reader["DEFFECDATE"] == DBNull.Value ? Convert.ToDateTime("") : Convert.ToDateTime(reader["DEFFECDATE"]);
                    rec.DEXPIRDAT = reader["DEXPIRDAT"] == DBNull.Value ? Convert.ToDateTime("") : Convert.ToDateTime(reader["DEXPIRDAT"]);


                    ListPremium.Add(rec);
                }

                Respuesta.PremiumVM = ListPremium;

                Respuesta.MESSAGE = P_SMESSAGE.Value.ToString();
                //}


                reader.Close();

                /*
                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter))
                {
                    ListPremium = dr.ReadRowsList<PremiumVM>();
                    ELog.CloseConnection(dr);
                }*/
            }
            catch (Exception ex)
            {
                LogControl.save("GetListPremium", ex.ToString(), "3");
            }

            return Respuesta;
        }

        public PremiumVMResponse GetListReciDev(PremiumVM[] data)
        {
            var sPackageName = string.Format("{0}.{1}", ProcedureName.pkg_NoteCreditSCTR, "SPS_GET_LISTRECIBO");
            int rows = 0;
            OracleDataReader dr = null;
            List<OracleParameter> parameter = new List<OracleParameter>();

            List<PremiumVM> ListReciDev = new List<PremiumVM>();
            PremiumVMResponse Respuesta = new PremiumVMResponse();

            try
            {

                while (rows < data.Length)
                {
                    //INPUT
                    parameter.Add(new OracleParameter("P_SCERTYPE", OracleDbType.Varchar2, data[rows].SCERTYPE, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data[rows].NBRANCH, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, data[rows].NPRODUCT, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, data[rows].SCLIENT, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int32, data[rows].NPOLICY, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_NCERTIF", OracleDbType.Int64, data[rows].NCERTIF, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_NRECEIPT", OracleDbType.Int64, data[rows].NRECEIPT, ParameterDirection.Input));
                    /*parameter.Add(new OracleParameter("DEFFECDATE", OracleDbType.Date, data[rows].DEFFECDATE, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("DEXPIRDAT", OracleDbType.Date, data[rows].DEXPIRDAT, ParameterDirection.Input));*/

                    //OUTPUT
                    OracleParameter P_SMESSAGE = new OracleParameter("P_MENSAGE", OracleDbType.Varchar2, string.Empty, ParameterDirection.Output);
                    P_SMESSAGE.Size = 4000;
                    parameter.Add(P_SMESSAGE);
                    parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                    using (dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter))

                        if (dr.HasRows)
                        {
                            // while (dr.Read())
                            //{
                            var rec = new PremiumVM();
                            rec.SCERTYPE = dr["SCERTYPE"] == DBNull.Value ? "" : dr["SCERTYPE"].ToString();
                            rec.NBRANCH = dr["NBRANCH"] == DBNull.Value ? 0 : Int32.Parse(dr["NBRANCH"].ToString());
                            rec.NPRODUCT = dr["NPRODUCT"] == DBNull.Value ? 0 : Int32.Parse(dr["NPRODUCT"].ToString());
                            rec.SCLIENT = dr["SCLIENT"] == DBNull.Value ? "" : dr["SCLIENT"].ToString();
                            rec.NPOLICY = dr["NPOLICY"] == DBNull.Value ? 0 : Int64.Parse(dr["NPOLICY"].ToString());
                            rec.NRECEIPT = dr["NRECEIPT"] == DBNull.Value ? 0 : Int64.Parse(dr["NRECEIPT"].ToString());
                            rec.NCERTIF = dr["NCERTIF"] == DBNull.Value ? 0 : Int64.Parse(dr["NCERTIF"].ToString());
                            rec.DDESDE = dr["DDESDE"] == DBNull.Value ? Convert.ToDateTime("") : Convert.ToDateTime(dr["DDESDE"]);
                            rec.DEXPIRDAT = dr["DEXPIRDAT"] == DBNull.Value ? Convert.ToDateTime("") : Convert.ToDateTime(dr["DEXPIRDAT"]);
                            rec.NAMOUNT = dr["NAMOUNT"] == DBNull.Value ? 0 : decimal.Parse(dr["NAMOUNT"].ToString());
                            rec.PORDEVO = dr["PORDEVO"] == DBNull.Value ? 0 : decimal.Parse(dr["PORDEVO"].ToString());
                            rec.IMPDEVO = dr["IMPDEVO"] == DBNull.Value ? 0 : decimal.Parse(dr["IMPDEVO"].ToString());

                            ListReciDev.Add(rec);


                            // }
                        }
                    Respuesta.PremiumVM = ListReciDev;

                    Respuesta.MESSAGE = P_SMESSAGE.Value.ToString();



                    rows++;
                    parameter.Clear();

                    if (data.Length == rows)
                    {
                        return Respuesta;
                    }

                }
                Respuesta.PremiumVM = ListReciDev;

                Respuesta.MESSAGE = "NO HAY DATOS SELECCIONADOS";
                ELog.CloseConnection(dr);

            }
            catch (Exception ex)
            {
                LogControl.save("GetListReciDev", ex.ToString(), "3");
            }

            return Respuesta;
        }

        //ENVIA UNO A UNO LOS PARAMETROS PARA REALIZAR LA NC
        public PremiumVMResponse GetListDetRecDev(PremiumVM[] data)
        {
            var sPackageName = string.Format("{0}.{1}", ProcedureName.pkg_NoteCreditSCTR, "SPS_GET_CONFDEV");
            int rows = 0;
            OracleDataReader dr = null;
            List<OracleParameter> parameter = new List<OracleParameter>();

            List<PremiumVM> ListDetRecDev = new List<PremiumVM>();
            PremiumVMResponse Respuesta = new PremiumVMResponse();
            Int64 NRECEIPTGRU = 0;

            try
            {

                while (rows < data.Length)
                {
                    //INPUT
                    parameter.Add(new OracleParameter("P_SCERTYPE", OracleDbType.Varchar2, data[rows].SCERTYPE, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data[rows].NBRANCH, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, data[rows].NPRODUCT, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, data[rows].SCLIENT, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int32, data[rows].NPOLICY, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_NRECEIPT", OracleDbType.Int64, data[rows].NRECEIPT, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_NCERTIF", OracleDbType.Int64, data[rows].NCERTIF, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_DDESDE", OracleDbType.Date, data[rows].DDESDE, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, data[rows].DEXPIRDAT, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_NAMOUNT", OracleDbType.Int32, data[rows].NAMOUNT, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_PORDEVO", OracleDbType.Int32, data[rows].PORDEVO, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_IMPDEVO", OracleDbType.Int32, data[rows].IMPDEVO, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_TOTAL", OracleDbType.Int32, data[rows].TOTAL, ParameterDirection.Input));

                    //RECIBO PARA MULTIPOLIZA
                    parameter.Add(new OracleParameter("P_NRECEIPTGRU", OracleDbType.Int64, NRECEIPTGRU, ParameterDirection.Input));

                    //OUTPUT
                    OracleParameter P_SMESSAGE = new OracleParameter("P_MENSAGE", OracleDbType.Varchar2, string.Empty, ParameterDirection.Output);
                    P_SMESSAGE.Size = 4000;
                    parameter.Add(P_SMESSAGE);
                    parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                    using (dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter))
                        if (dr.HasRows)
                        {
                            var rec = new PremiumVM();
                            rec.SCERTYPE = dr["SCERTYPE"] == DBNull.Value ? "" : dr["SCERTYPE"].ToString();
                            rec.NBRANCH = dr["NBRANCH"] == DBNull.Value ? 0 : Int32.Parse(dr["NBRANCH"].ToString());
                            rec.SDES_BRANCH = dr["SDES_BRANCH"] == DBNull.Value ? "" : dr["SDES_BRANCH"].ToString();
                            rec.NPRODUCT = dr["NPRODUCT"] == DBNull.Value ? 0 : Int32.Parse(dr["NPRODUCT"].ToString());
                            rec.SCLIENT = dr["SCLIENT"] == DBNull.Value ? "" : dr["SCLIENT"].ToString();
                            rec.SDES_PRODMAST = dr["SDES_PRODMAST"] == DBNull.Value ? "" : dr["SDES_PRODMAST"].ToString();
                            rec.NPOLICY = dr["NPOLICY"] == DBNull.Value ? 0 : Int64.Parse(dr["NPOLICY"].ToString());
                            rec.NRECEIPT = dr["NRECEIPT"] == DBNull.Value ? 0 : Int64.Parse(dr["NRECEIPT"].ToString());
                            rec.NRECEIPTDEV = dr["NRECEIPTDEV"] == DBNull.Value ? 0 : Int64.Parse(dr["NRECEIPTDEV"].ToString());
                            rec.NCERTIF = dr["NCERTIF"] == DBNull.Value ? 0 : Int64.Parse(dr["NCERTIF"].ToString());
                            rec.DDESDE = dr["DDESDE"] == DBNull.Value ? Convert.ToDateTime("") : Convert.ToDateTime(dr["DDESDE"]);
                            rec.DEXPIRDAT = dr["DEXPIRDAT"] == DBNull.Value ? Convert.ToDateTime("") : Convert.ToDateTime(dr["DEXPIRDAT"]);
                            rec.IMPDEVO = dr["IMPDEVO"] == DBNull.Value ? 0 : decimal.Parse(dr["IMPDEVO"].ToString());
                            rec.SDES_MONEDA = dr["SDES_MONEDA"] == DBNull.Value ? "" : dr["SDES_MONEDA"].ToString();

                            ListDetRecDev.Add(rec);
                            NRECEIPTGRU = rec.NRECEIPTDEV;


                        }

                    Respuesta.PremiumVM = ListDetRecDev;
                    Respuesta.MESSAGE = P_SMESSAGE.Value.ToString();


                    rows++;
                    parameter.Clear();
                }
                ELog.CloseConnection(dr);

            }
            catch (Exception ex)
            {
                LogControl.save("GetListDetRecDev", ex.ToString(), "3");
            }

            return Respuesta;
        }
        //ENVIA LOS PARAMETROS PARA RECALCULAR LOS MONTOS
        public PremiumVMResponse GetFilaResi(PremiumVM data)
        {
            var parameter = new List<OracleParameter>();
            var filaResi = new List<PremiumVM>();
            PremiumVMResponse Respuesta = new PremiumVMResponse();
            OracleDataReader dr = null;
            var sPackageName = string.Format("{0}.{1}", ProcedureName.pkg_NoteCreditSCTR, "SPS_GET_FILARESI");

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_SCERTYPE", OracleDbType.Varchar2, data.SCERTYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, data.NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int32, data.NPOLICY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCERTIF", OracleDbType.Int32, data.NCERTIF, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, data.SCLIENT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NRECEIPT", OracleDbType.Int64, data.NRECEIPT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DDESDE", OracleDbType.Date, data.DDESDE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, data.DEXPIRDAT, ParameterDirection.Input));


                //OUTPUT
                OracleParameter P_SMESSAGE = new OracleParameter("P_MENSAGE", OracleDbType.Varchar2, string.Empty, ParameterDirection.Output);
                P_SMESSAGE.Size = 4000;
                parameter.Add(P_SMESSAGE);
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                using (dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter))

                    if (dr.HasRows)
                    {
                        // while (dr.Read())
                        //{
                        var rec = new PremiumVM();
                        rec.SCERTYPE = dr["SCERTYPE"] == DBNull.Value ? "" : dr["SCERTYPE"].ToString();
                        rec.NBRANCH = dr["NBRANCH"] == DBNull.Value ? 0 : Int32.Parse(dr["NBRANCH"].ToString());
                        rec.NPRODUCT = dr["NPRODUCT"] == DBNull.Value ? 0 : Int32.Parse(dr["NPRODUCT"].ToString());
                        rec.SCLIENT = dr["SCLIENT"] == DBNull.Value ? "" : dr["SCLIENT"].ToString();
                        rec.NPOLICY = dr["NPOLICY"] == DBNull.Value ? 0 : Int64.Parse(dr["NPOLICY"].ToString());
                        rec.NRECEIPT = dr["NRECEIPT"] == DBNull.Value ? 0 : Int64.Parse(dr["NRECEIPT"].ToString());
                        rec.NCERTIF = dr["NCERTIF"] == DBNull.Value ? 0 : Int64.Parse(dr["NCERTIF"].ToString());
                        rec.DDESDE = dr["DDESDE"] == DBNull.Value ? Convert.ToDateTime("") : Convert.ToDateTime(dr["DDESDE"]);
                        rec.DEXPIRDAT = dr["DEXPIRDAT"] == DBNull.Value ? Convert.ToDateTime("") : Convert.ToDateTime(dr["DEXPIRDAT"]);
                        rec.NAMOUNT = dr["NAMOUNT"] == DBNull.Value ? 0 : decimal.Parse(dr["NAMOUNT"].ToString());
                        rec.PORDEVO = dr["PORDEVO"] == DBNull.Value ? 0 : decimal.Parse(dr["PORDEVO"].ToString());
                        rec.IMPDEVO = dr["IMPDEVO"] == DBNull.Value ? 0 : decimal.Parse(dr["IMPDEVO"].ToString());

                        filaResi.Add(rec);

                        Respuesta.PremiumVM = filaResi;

                        Respuesta.MESSAGE = P_SMESSAGE.Value.ToString();
                        // }
                    }

                parameter.Clear();

                ELog.CloseConnection(dr);

            }
            catch (Exception ex)
            {
                LogControl.save("GetFilaResi", ex.ToString(), "3");
            }

            return Respuesta;
        }
        #endregion
    }
}
