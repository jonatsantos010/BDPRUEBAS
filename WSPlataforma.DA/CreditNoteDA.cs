/*************************************************************************************************/
/*  NOMBRE              :   CreditNoteDA.CS                                                      */
/*  DESCRIPCION         :   Capa Controller                                                      */
/*  AUTOR               :   MATERIAGRIS - JOSUE CORONEL FLORES                                   */
/*  FECHA               :   18-10-2022                                                           */
/*  VERSION             :   1.0                                                                  */
/*************************************************************************************************/

using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.Entities.CreditNoteModel.ViewModel;
using WSPlataforma.Entities.QuotationModel.ViewModel;
using WSPlataforma.Util;

namespace WSPlataforma.DA
{
    public class NoteCreditRefundDA : ConnectionBase
    {
        #region Filters

        private readonly string Package = "PKG_SCTR_CREDITNOTE4";
        private readonly string PackageVal = "PKG_SCTR_CREDITNOTE_VAL";

        List<PremiumVM> ListPremium = new List<PremiumVM>();

        public List<BranchVM> GetBranches()
        {
            var parameters = new List<OracleParameter>();
            var branches = new List<BranchVM>();
            var procedure = string.Format("{0}.{1}", this.Package, "SPS_GET_BRANCHES");

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
            var sPackageName = string.Format("{0}.{1}", this.Package, "SPS_GET_LISTPRODUCT");

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
            var sPackageName = string.Format("{0}.{1}", this.Package, "SPS_GET_LISTPARAMETER");

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
            var procedure = string.Format("{0}.{1}", this.Package, "SPS_GET_TYPE_DOCUMENT");

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
            var procedure = string.Format("{0}.{1}", this.Package, "SPS_GET_TYPE_BILL");

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
            var sPackageName = string.Format("{0}.{1}", this.Package, "SPS_GET_LISTPREMIUM");
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
                parameter.Add(new OracleParameter("P_NBILLNUM", OracleDbType.Varchar2, data.SCOMPROBANTE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Varchar2, data.SFECHAINI, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Varchar2, data.SFECHAFIN, ParameterDirection.Input));
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
                    rec.VALIDADOR = false;

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
            var sPackageName = string.Format("{0}.{1}", this.Package, "SPS_GET_LISTRECIBO");
            int rows = 0;
            int V_NMCM = 0;
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
                    parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int64, data[rows].NPOLICY, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_NCERTIF", OracleDbType.Int64, data[rows].NCERTIF, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_NRECEIPT", OracleDbType.Int64, data[rows].NRECEIPT, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_DDESDE", OracleDbType.Date, data[rows].DEFFECDATE, ParameterDirection.Input));
                    parameter.Add(new OracleParameter("P_TIPO", OracleDbType.Int64, data[rows].NTIPO, ParameterDirection.Input));

                    if (rows == 0)
                    {
                        parameter.Add(new OracleParameter("P_NMCN", OracleDbType.Int64, data[rows].NMCN, ParameterDirection.Input));
                    }
                    else
                    {
                        parameter.Add(new OracleParameter("P_NMCN", OracleDbType.Int64, V_NMCM, ParameterDirection.Input));
                    };

                    //OUTPUT
                    OracleParameter P_SMESSAGE = new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, string.Empty, ParameterDirection.Output);
                    P_SMESSAGE.Size = 4000;
                    parameter.Add(P_SMESSAGE);
                    parameter.Add(new OracleParameter("C_OUT", OracleDbType.RefCursor, ParameterDirection.Output));
                    OracleParameter P_CHEK = new OracleParameter("P_CHEK", OracleDbType.Int64, string.Empty, ParameterDirection.Output);
                    P_CHEK.Size = 2;
                    parameter.Add(P_CHEK);
                    OracleParameter P_NMCM_OUT = new OracleParameter("P_NMCN_OUT", OracleDbType.Int32, string.Empty, ParameterDirection.Output);
                    P_NMCM_OUT.Size = 10;
                    parameter.Add(P_NMCM_OUT);
                    OracleParameter P_EST = new OracleParameter("P_EST", OracleDbType.Int64, string.Empty, ParameterDirection.Output);
                    P_EST.Size = 10;
                    parameter.Add(P_EST);
                    using (dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter))
                        if (dr.HasRows)
                        {
                            if (data[rows].NTIPO == 1)
                            {

                                while (dr.Read())
                                {
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
                                    rec.NTIPO = dr["TIPO"] == DBNull.Value ? 0 : Int64.Parse(dr["TIPO"].ToString());
                                    rec.NMCN = dr["NMCN"] == DBNull.Value ? 0 : Int64.Parse(dr["NMCN"].ToString());
                                    rec.NTIPO_DEVOANU = dr["NTIPO_DEVOANU"] == DBNull.Value ? string.Empty : dr["NTIPO_DEVOANU"].ToString();
                                    rec.NCOM_CAN = dr["NCOM_CAN"] == DBNull.Value ? 0 : decimal.Parse(dr["NCOM_CAN"].ToString());
                                    rec.NCOM_COR = dr["NCOM_COR"] == DBNull.Value ? 0 : decimal.Parse(dr["NCOM_COR"].ToString());
                                    rec.DEFFECDATE = dr["DEFFECDATE"] == DBNull.Value ? Convert.ToDateTime("") : Convert.ToDateTime(dr["DEFFECDATE"]);
                                    rec.NCOMMISSION = dr["NCOMMISSION"] == DBNull.Value ? 0 : decimal.Parse(dr["NCOMMISSION"].ToString());

                                    ListReciDev.Add(rec);
                                    
                                }
                            }

                        }
                    Respuesta.PremiumVM = ListReciDev;
                    Respuesta.MESSAGE = P_SMESSAGE.Value.ToString();
                    Respuesta.EST = P_EST.Value.ToString();
                    Respuesta.P_CHEK = P_CHEK.Value.ToString();
                    V_NMCM = Int32.Parse(P_NMCM_OUT.Value.ToString());

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

        public PremiumVMResponse GetListDetRecDev(PremiumVM data)
        {
            PremiumVMResponse Respuesta = new PremiumVMResponse();
            var parameter = new List<OracleParameter>();
            List<PremiumVM> ListDetRecDev = new List<PremiumVM>();
            OracleDataReader dr = null;
            var sPackageName = string.Format("{0}.{1}", this.Package, "SPS_GET_CONFDEV");

            string[] _List =
            {
                "A",
                "B",
                "C",
                "D",
                "E",
                "F",
                "G",
                "H",
                "102.2",
                "0",
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")
            };

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NMCN", OracleDbType.Int32, data.NMCN, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, data.NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCOMMIT ", OracleDbType.Int32, data.NCOMMIT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NFLAG_ANUL", OracleDbType.Int32, data.P_NFLAG_ANUL, ParameterDirection.Input));
                //OUTPUT
                OracleParameter P_SMESSAGE = new OracleParameter("P_MENSAGE", OracleDbType.Varchar2, string.Empty, ParameterDirection.Output);
                P_SMESSAGE.Size = 4000;
                parameter.Add(P_SMESSAGE);
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                OracleParameter P_EST = new OracleParameter("P_EST", OracleDbType.Int64, string.Empty, ParameterDirection.Output);
                P_EST.Size = 10;
                parameter.Add(P_EST);

                using (dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter))
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            var rec = new PremiumVM();
                            rec.SBILLNUM = dr["SBILLNUM"] == DBNull.Value ? "" : dr["SBILLNUM"].ToString();
                            rec.NBRANCH = dr["NBRANCH"] == DBNull.Value ? 0 : Int32.Parse(dr["NBRANCH"].ToString());
                            rec.NPRODUCT = dr["NPRODUCT"] == DBNull.Value ? 0 : Int32.Parse(dr["NPRODUCT"].ToString());
                            rec.NPOLICY = dr["NPOLICY"] == DBNull.Value ? 0 : Int64.Parse(dr["NPOLICY"].ToString());
                            rec.NCERTIF = dr["NCERTIF"] == DBNull.Value ? 0 : Int64.Parse(dr["NCERTIF"].ToString());
                            rec.DDESDE = dr["DDESDE"] == DBNull.Value ? Convert.ToDateTime("") : Convert.ToDateTime(dr["DDESDE"]);
                            rec.DEXPIRDAT = dr["DEXPIRDAT"] == DBNull.Value ? Convert.ToDateTime("") : Convert.ToDateTime(dr["DEXPIRDAT"]);
                            rec.NRECEIPT = dr["NRECEIPT"] == DBNull.Value ? 0 : Int64.Parse(dr["NRECEIPT"].ToString());
                            rec.NRECEIPT_NC = dr["NRECEIPT_NC"] == DBNull.Value ? 0 : Int64.Parse(dr["NRECEIPT_NC"].ToString());
                            rec.IMPDEVO = dr["IMPDEVO"] == DBNull.Value ? 0 : decimal.Parse(dr["IMPDEVO"].ToString());
                            rec.SMONEDA = dr["SMONEDA"] == DBNull.Value ? "" : dr["SMONEDA"].ToString();
                            rec.SPRODUCT = dr["SPRODUCT"] == DBNull.Value ? "" : dr["SPRODUCT"].ToString();
                            rec.SBRANCH = dr["SBRANCH"] == DBNull.Value ? "" : dr["SBRANCH"].ToString();
                            ListDetRecDev.Add(rec);
                        }

                    }

                Respuesta.PremiumVM = ListDetRecDev;
                Respuesta.MESSAGE = P_SMESSAGE.Value.ToString();
                Respuesta.EST = P_EST.Value.ToString();

                parameter.Clear();
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
            //OracleDataReader dr = null;
            var sPackageName = string.Format("{0}.{1}", this.Package, "SPS_GET_FILARESI");

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_SCERTYPE", OracleDbType.Varchar2, data.SCERTYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, data.NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, data.SCLIENT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int32, data.NPOLICY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCERTIF", OracleDbType.Int64, data.NCERTIF, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NRECEIPT", OracleDbType.Int64, data.NRECEIPT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DDESDE", OracleDbType.Date, data.DDESDE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TIPO", OracleDbType.Int64, data.NTIPO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMCN", OracleDbType.Int64, data.NMCN, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_SMESSAGE = new OracleParameter("P_MENSAGE", OracleDbType.Varchar2, string.Empty, ParameterDirection.Output);
                P_SMESSAGE.Size = 4000;
                parameter.Add(P_SMESSAGE);
                //parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                parameter.Add(new OracleParameter("C_OUT", OracleDbType.RefCursor, ParameterDirection.Output));
                parameter.Add(new OracleParameter("P_NMCN_OUT", OracleDbType.Int64, string.Empty, ParameterDirection.Output));
                OracleParameter P_EST = new OracleParameter("P_EST", OracleDbType.Int64, string.Empty, ParameterDirection.Output);
                P_EST.Size = 10;
                parameter.Add(P_EST);

                OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter);

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
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

                        rec.NTIPO = dr["TIPO"] == DBNull.Value ? 0 : Int64.Parse(dr["TIPO"].ToString());
                        rec.NMCN = dr["NMCN"] == DBNull.Value ? 0 : Int64.Parse(dr["NMCN"].ToString());
                        rec.NTIPO_DEVOANU = dr["NTIPO_DEVOANU"] == DBNull.Value ? string.Empty : dr["NTIPO_DEVOANU"].ToString();
                        rec.NCOM_CAN = dr["NCOM_CAN"] == DBNull.Value ? 0 : decimal.Parse(dr["NCOM_CAN"].ToString());
                        rec.NCOM_COR = dr["NCOM_COR"] == DBNull.Value ? 0 : decimal.Parse(dr["NCOM_COR"].ToString());
                        rec.DEFFECDATE = dr["DEFFECDATE"] == DBNull.Value ? Convert.ToDateTime("") : Convert.ToDateTime(dr["DEFFECDATE"]);
                        rec.NCOMMISSION = dr["NCOMMISSION"] == DBNull.Value ? 0 : decimal.Parse(dr["NCOMMISSION"].ToString());

                        filaResi.Add(rec);
                    }
                }

                dr.Close();

                /* using (dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter))

                    if (dr.HasRows)
                    {
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

                        rec.NTIPO = dr["TIPO"] == DBNull.Value ? 0 : Int64.Parse(dr["TIPO"].ToString());
                        rec.NMCN = dr["NMCN"] == DBNull.Value ? 0 : Int64.Parse(dr["NMCN"].ToString());
                        rec.NTIPO_DEVOANU = dr["NTIPO_DEVOANU"] == DBNull.Value ? string.Empty : dr["NTIPO_DEVOANU"].ToString();
                        rec.NCOM_CAN = dr["NCOM_CAN"] == DBNull.Value ? 0 : decimal.Parse(dr["NCOM_CAN"].ToString());
                        rec.NCOM_COR = dr["NCOM_COR"] == DBNull.Value ? 0 : decimal.Parse(dr["NCOM_COR"].ToString());
                        rec.DEFFECDATE = dr["DEFFECDATE"] == DBNull.Value ? Convert.ToDateTime("") : Convert.ToDateTime(dr["DEFFECDATE"]);
                        rec.NCOMMISSION = dr["NCOMMISSION"] == DBNull.Value ? 0 : decimal.Parse(dr["NCOMMISSION"].ToString());

                        filaResi.Add(rec);


                    }*/

                Respuesta.PremiumVM = filaResi;
                Respuesta.MESSAGE = P_SMESSAGE.Value.ToString();
                Respuesta.EST = P_EST.Value.ToString();

                parameter.Clear();

                ELog.CloseConnection(dr);

            }
            catch (Exception ex)
            {
                LogControl.save("GetFilaResi", ex.ToString(), "3");
            }

            return Respuesta;
        }

        public resPremiumVM valCargaMasiva(PremiumVM data)
        {
            var parameters = new List<OracleParameter>();
            resPremiumVM entities = new resPremiumVM();

            var procedure = string.Format("{0}.{1}", this.Package, "SPS_VAL_CARGA_MASIVA");
            try
            {
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.NVarchar2, data.NBRANCH, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.NVarchar2, data.NPRODUCT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPOLICY", OracleDbType.NVarchar2, data.NPOLICY, ParameterDirection.Input));
                OracleParameter P_NEST = new OracleParameter("P_NEST", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);

                parameters.Add(P_NEST);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                entities.p_nest = Convert.ToInt32(P_NEST.Value.ToString());
                entities.p_ncode = Convert.ToInt32(P_NCODE.Value.ToString());
                entities.p_smessage = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("ConsultarTipoAsiento", ex.ToString(), "3");
                throw;
            }
            return entities;
        }

        public PremiumVMResponse SaveUsingOracleBulkCopy(DataTable dt)
        {
            //var sPackageName = string.Format("{0}.{1}", this.Package, "SPS_LIST_MASIVA");--INI <Migrante - 022024  - 20/02/2024>
            var sPackageName = string.Format("{0}.{1}", this.PackageVal, "SPS_VAL_PRINCIPAL_CARGA"); //INI <Migrante - 022024  - 20/02/2024>

            //OracleDataReader dr = null;

            List<OracleParameter> parameter = new List<OracleParameter>();

            List<PremiumVM> ListReciDev = new List<PremiumVM>();

            PremiumVMResponse Respuesta = new PremiumVMResponse();
            Respuesta.SKEY = dt.Rows[0][7].ToString();
            var resultPackage = new SalidaTramaBaseVM() { P_COD_ERR = "0" };
            var tblNameList = new string[] { "TMP_CARGA_FACTURACION_AGRUPADA_TEMPORAL" };

            try
            {
                foreach (var tblName in tblNameList)
                {
                    if (resultPackage.P_COD_ERR == "0")
                    {
                        resultPackage = this.SaveUsingOracleBulkCopy(tblName, dt);
                    }
                }
            }
            catch (Exception ex)
            {
                resultPackage.P_COD_ERR = "1";
                resultPackage.P_MESSAGE = ex.ToString();
                LogControl.save("SaveUsingOracleBulkCopy", ex.ToString(), "3");
            }

         //OBTIENE LA GRILLA PARA LLENAOD DE DATOS DESPUES DEL VOLCADO
            if (resultPackage.P_COD_ERR == "0")
            {
                try
                {
                    //INPUT
                    parameter.Add(new OracleParameter("P_SKEY", OracleDbType.NVarchar2, Respuesta.SKEY, ParameterDirection.Input));

                    //OUTPUT  --INI <Migrante - 022024  - 20/02/2024>
                    OracleParameter P_NCANTNC = new OracleParameter("P_NCANTNC", OracleDbType.Int32, string.Empty, ParameterDirection.Output);
                    P_NCANTNC.Size = 10;
                    parameter.Add(P_NCANTNC);
                    //OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, string.Empty, ParameterDirection.Output);
                    // P_NCODE.Size = 10;
                    // parameter.Add(P_NCODE);
                    //OracleParameter P_SMESSAGE = new OracleParameter("P_SMENSAJE", OracleDbType.Varchar2, string.Empty, ParameterDirection.Output);
                    //P_SMESSAGE.Size = 4000;
                    // parameter.Add(P_SMESSAGE);
                    
                    //FIN < Migrante - 022024 - 20 / 02 / 2024 >

                    parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));


                    OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter);


                    while (dr.Read())
                    {
                        var rec = new PremiumVM();
                        rec.SBRANCH = dr["RAMO"] == DBNull.Value ? "" : dr["RAMO"].ToString();
                        rec.SPRODUCT = dr["PRODUCTO"] == DBNull.Value ? "" : dr["PRODUCTO"].ToString();
                        rec.NPOLICY = dr["NRO_POLIZA"] == DBNull.Value ? 0 : Int64.Parse(dr["NRO_POLIZA"].ToString());
                        rec.SCLIENT = dr["CLIENTE"] == DBNull.Value ? "" : dr["CLIENTE"].ToString();
                        rec.SCOMPROBANTE = dr["COMPROBANTE"] == DBNull.Value ? "" : dr["COMPROBANTE"].ToString();
                        rec.NRECEIPT = dr["NRO_RECIBO"] == DBNull.Value ? 0 : Int64.Parse(dr["NRO_RECIBO"].ToString());
                        rec.DDESDE = dr["DESDE"] == DBNull.Value ? Convert.ToDateTime("") : Convert.ToDateTime(dr["DESDE"]);
                        rec.DEXPIRDAT = dr["FIN"] == DBNull.Value ? Convert.ToDateTime("") : Convert.ToDateTime(dr["FIN"]);
                        rec.SPRIMA = dr["PRIMA_A_DEVOLVER"] == DBNull.Value ? "" : dr["PRIMA_A_DEVOLVER"].ToString();
                        rec.SMENERROR = dr["MENSAJE_ERROR"] == DBNull.Value ? "" : dr["MENSAJE_ERROR"].ToString();
                        ListReciDev.Add(rec);

                    }
                    

                    //Respuesta.MESSAGE = P_SMESSAGE.Value.ToString();
                    //Respuesta.NCODE = P_NCODE.Value.ToString();
                    Respuesta.NCANTNC = int.Parse(P_NCANTNC.Value.ToString());
                    Respuesta.PremiumVM = ListReciDev;

                    ELog.CloseConnection(dr);

                    parameter.Clear();

                    Respuesta.MESSAGE = evalError(Respuesta.SKEY).MESSAGE;
                    Respuesta.NCODE = evalError(Respuesta.SKEY).NCODE;

                }
                catch (Exception ex)
                {
                    LogControl.save("GetListReciDev", ex.ToString(), "3");
                }
            }
            else 
            {
                Respuesta.MESSAGE = "PROBLEMAS EN EL VOLCADO DEL ARCHIVO EXCEL";
            }

            return Respuesta;
        }

        public PremiumVMResponse evalError(string SKEY)
        {

            var sPackageNameRes = string.Format("{0}.{1}", this.PackageVal, "SPS_CONDICION_RESULTADO_VALIDACION_CARGA"); 


            List<OracleParameter> parameterRes = new List<OracleParameter>();

            List<PremiumVM> ListReciDev = new List<PremiumVM>();

            PremiumVMResponse Respuesta = new PremiumVMResponse();
          
                try
                {
                   
                    parameterRes.Add(new OracleParameter("P_SKEY", OracleDbType.Varchar2, SKEY, ParameterDirection.Input));
                    OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, string.Empty, ParameterDirection.Output);
                    P_NCODE.Size = 10;
                    parameterRes.Add(P_NCODE);
                    OracleParameter P_SMESSAGE = new OracleParameter("P_SDESC_MESSAGE", OracleDbType.Varchar2, string.Empty, ParameterDirection.Output);
                    P_SMESSAGE.Size = 4000;
                    parameterRes.Add(P_SMESSAGE);

                    OracleDataReader dr2 = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageNameRes, parameterRes);

                    Respuesta.MESSAGE = P_SMESSAGE.Value.ToString();
                    Respuesta.NCODE = P_NCODE.Value.ToString();

                    parameterRes.Clear();

                    ELog.CloseConnection(dr2);

                }
                catch (Exception ex)
                {
                    LogControl.save("GetListReciDev", ex.ToString(), "3");
                }
            

            return Respuesta;
        }

        public PremiumVMResponse genNcMasiva(PremiumVM data)
        {
            var parameters = new List<OracleParameter>();
            resPremiumVM entities = new resPremiumVM();
           //OracleDataReader dr = null;
            List<PremiumVM> ListReciDev = new List<PremiumVM>();
            PremiumVMResponse Respuesta = new PremiumVMResponse();

            var sPackageName = string.Format("{0}.{1}", this.Package, "SPS_GET_NC_MASIVA");
            try
            {
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.NBRANCH, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, data.NPRODUCT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int64, data.NPOLICY, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SKEY", OracleDbType.NVarchar2, data.SKEY, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int64, data.NUSERCODE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCOMMIT", OracleDbType.Int16, 2, ParameterDirection.Input));

                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                P_SMESSAGE.Size = 4000;
                parameters.Add(P_SMESSAGE);
                OracleParameter P_NCODE = new OracleParameter("P_NERROR", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                P_NCODE.Size = 10;
                parameters.Add(P_NCODE);
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                
                
                //using (dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameters))
                OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameters);

                while (dr.Read())
                        {
                            var rec = new PremiumVM();
                            rec.NPOLICY = dr["NPOLICY"] == DBNull.Value ? 0 : long.Parse(dr["NPOLICY"].ToString());
                            rec.SCOMPROBANTE = dr["COMPROBANTE"] == DBNull.Value ? "" : dr["COMPROBANTE"].ToString();
                            rec.SNC = dr["comprobante_NC"] == DBNull.Value ? "" : dr["comprobante_NC"].ToString();
                            rec.DDESDE = dr["DFECHA_INICIO_NC"] == DBNull.Value ? Convert.ToDateTime("") : Convert.ToDateTime(dr["DFECHA_INICIO_NC"]);
                            rec.SPRIMA = dr["NPRIMA_DEVOLVER"] == DBNull.Value ? "" : dr["NPRIMA_DEVOLVER"].ToString();
                            ListReciDev.Add(rec);

                        }

                Respuesta.NCODE = P_NCODE.Value.ToString();
                Respuesta.MESSAGE = P_SMESSAGE.Value.ToString();
                Respuesta.PremiumVM = ListReciDev;
                ELog.CloseConnection(dr);
            }
            catch (Exception ex)
            {
                LogControl.save("ConsultarTipoAsiento", ex.ToString(), "3");
                throw;
            }
            return Respuesta;
        }

        #endregion
    }
}
