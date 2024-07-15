using Oracle.DataAccess.Client;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.Entities.TransactModel;
using WSPlataforma.Util;

namespace WSPlataforma.DA
{
    public class TransactDA : ConnectionBase
    {
        private SharedMethods sharedMethods = new SharedMethods();
        public TransactResponse InsertTransact(RequestTransact request, DbConnection connection = null, DbTransaction trx = null)
        {
            var sPackageName = ProcedureName.pkg_Tramites + ".INS_TRAMITE_GENERATE";

            List<OracleParameter> parameter = new List<OracleParameter>();
            TransactResponse result = new TransactResponse();

            if (request.P_NBRANCH.ToString() == ELog.obtainConfig("vidaIndividualBranch"))
            {
                request.P_SDEVOLPRI = 0;
                request.P_NTIP_RENOV = 0;
                request.P_NPAYFREQ = 0;
                request.P_NTYPENDOSO = 0;
                request.P_NIDPLAN = 0;
                request.P_NINTERMED = 0;

            }

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, request.P_NID_COTIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DFEC_REG", OracleDbType.Date, Convert.ToDateTime(request.P_DFEC_REG), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, request.P_NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, request.P_NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STRANSAC", OracleDbType.Varchar2, request.P_STRANSAC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int64, request.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SMAIL_EJECCOM", OracleDbType.Varchar2, request.P_SMAIL_EJECCOM, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SRUTA", OracleDbType.Varchar2, request.P_SRUTA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOMMENT", OracleDbType.Varchar2, request.P_SCOMMENT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SDEVOLPRI", OracleDbType.Varchar2, request.P_SDEVOLPRI, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTIP_RENOV", OracleDbType.Int32, request.P_NTIP_RENOV, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPAYFREQ", OracleDbType.Int32, request.P_NPAYFREQ, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPENDOSO", OracleDbType.Int32, request.P_NTYPENDOSO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDPLAN", OracleDbType.Int32, request.P_NIDPLAN, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NINTERMED", OracleDbType.Int32, request.P_NINTERMED, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_NID_TRAMITE = new OracleParameter("P_NID_TRAMITE", OracleDbType.Int32, result.P_NID_TRAMITE, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, result.P_MESSAGE, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, result.P_COD_ERR, ParameterDirection.Output);

                P_NID_TRAMITE.Size = 9000;
                P_MESSAGE.Size = 9000;
                P_COD_ERR.Size = 9000;

                parameter.Add(P_NID_TRAMITE);
                parameter.Add(P_MESSAGE);
                parameter.Add(P_COD_ERR);

                parameter.Add(new OracleParameter("P_SLETTER_AGENCY", OracleDbType.Varchar2, request.P_SLETTER_AGENCY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, request.P_SCLIENT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SPOL_MATRIZ", OracleDbType.Varchar2, request.P_SPOL_MATRIZ, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SPOL_ESTADO", OracleDbType.Int32, request.P_SPOL_ESTADO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_APROB_CLI", OracleDbType.Int32, request.P_APROB_CLI, ParameterDirection.Input));

                // mejoras CIIU VL 
                parameter.Add(new OracleParameter("P_SCOD_ACTIVITY_TEC", OracleDbType.Varchar2, request.P_SCOD_ACTIVITY_TEC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOD_CIUU", OracleDbType.Varchar2, request.P_SCOD_CIUU, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NFLAG_UPD", OracleDbType.Int32, request.P_NFLAG_UPD, ParameterDirection.Input));

                if (connection == null)
                {
                    this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                }
                else
                {
                    this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, connection, trx);
                }

                result.P_NID_TRAMITE = Convert.ToInt32(P_NID_TRAMITE.Value.ToString());
                result.P_MESSAGE = P_MESSAGE.Value.ToString();
                result.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                if (result.P_NID_TRAMITE == 0)
                {
                    result.P_COD_ERR = 1;
                    if (result.P_MESSAGE == "")
                    {
                        result.P_MESSAGE = "Error en el servidor. Error al generar trámite.";
                    }
                }
            }
            catch (Exception ex)
            {
                result.P_NID_TRAMITE = 0;
                result.P_MESSAGE = "Error en el servidor. " + ex.Message;
                result.P_COD_ERR = 1;
            }
            return result;
        }

        public List<UserTransact> GetUsersTransact(RequestTransact request)
        {
            List<UserTransact> response = new List<UserTransact>();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.pkg_Tramites + ".READ_USERS";
            OracleDataReader dr = null;
            try
            {
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, request.P_NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, request.P_NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, request.P_NUSERCODE, ParameterDirection.Input));

                string[] arrayCursor = { "C_TABLE" };
                OracleParameter C_TABLE = new OracleParameter(arrayCursor[0], OracleDbType.RefCursor, ParameterDirection.Output);

                parameter.Add(C_TABLE);

                dr = (OracleDataReader)this.ExecuteByStoredProcedureVTCursores(storeprocedure, arrayCursor, parameter);

                foreach (var obj in dr.ReadRowsList<UserTransact>())
                {
                    response.Add(obj);
                }
                ELog.CloseConnection(dr);
            }
            catch (Exception ex)
            {
                response = new List<UserTransact>();
                ELog.CloseConnection(dr);
            }
            return response;
        }

        public TransactResponse AsignarTransact(RequestTransact request)
        {
            var sPackageName = ProcedureName.pkg_Tramites + ".INS_TRAMITE_ASIGNAR";

            List<OracleParameter> parameter = new List<OracleParameter>();
            TransactResponse result = new TransactResponse();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_TRAMITE", OracleDbType.Int32, request.P_NID_TRAMITE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, request.P_NID_COTIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DFEC_REG", OracleDbType.Date, DateTime.Today, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE_ASSIGNOR", OracleDbType.Int32, request.P_NUSERCODE_ASSIGNOR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, request.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NSTATUS_TRA", OracleDbType.Int32, request.P_NSTATUS_TRA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SRUTA", OracleDbType.Varchar2, request.P_SRUTA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOMMENT", OracleDbType.Varchar2, request.P_SCOMMENT, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, result.P_MESSAGE, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, result.P_COD_ERR, ParameterDirection.Output);

                P_MESSAGE.Size = 9000;
                P_COD_ERR.Size = 9000;

                parameter.Add(P_MESSAGE);
                parameter.Add(P_COD_ERR);

                this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                result.P_MESSAGE = P_MESSAGE.Value.ToString();
                result.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
            }
            catch (Exception ex)
            {
                //result.P_MESSAGE = "Error en el servidor. " + ex.Message;
                result.P_MESSAGE = "Error en el servidor.";
                result.P_COD_ERR = 1;
                LogControl.save("AsignarTransact", ex.ToString(), "3");
            }
            return result;
        }
        public List<TransactModel> GetHistTransact(RequestTransact request)
        {
            List<TransactModel> response = new List<TransactModel>();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.pkg_Tramites + ".REA_TRAMITE_HIS";
            OracleDataReader dr = null;
            try
            {
                parameter.Add(new OracleParameter("P_NID_TRAMITE", OracleDbType.Int32, request.P_NID_TRAMITE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, request.P_NID_COTIZACION, ParameterDirection.Input));
                //parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, request.P_NPRODUCT, ParameterDirection.Input));
                //parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, request.P_NBRANCH, ParameterDirection.Input));

                string[] arrayCursor = { "C_TABLE" };
                OracleParameter C_TABLE = new OracleParameter(arrayCursor[0], OracleDbType.RefCursor, ParameterDirection.Output);

                parameter.Add(C_TABLE);

                dr = (OracleDataReader)this.ExecuteByStoredProcedureVTCursores(storeprocedure, arrayCursor, parameter);

                foreach (var obj in dr.ReadRowsList<TransactModel>())
                {
                    response.Add(obj);
                }

                foreach (TransactModel item in response)
                {
                    try
                    {
                        if (item.SRUTA != null)
                        {
                            if (!String.IsNullOrEmpty(item.SRUTA.ToString()))
                            {
                                item.PATHS_FILES = sharedMethods.GetFilePathList(String.Format(ELog.obtainConfig("pathPrincipal"), item.SRUTA.ToString()));
                            }
                        }
                    }
                    catch (Exception) { }
                }

                ELog.CloseConnection(dr);
            }
            catch (Exception ex)
            {
                response = new List<TransactModel>();
                ELog.CloseConnection(dr);
            }
            return response;
        }

        public TransactResponse InsertDerivarTransact(RequestTransact request)
        {
            var sPackageName = ProcedureName.pkg_Tramites + ".INS_TRAMITE_DERIVAR";

            List<OracleParameter> parameter = new List<OracleParameter>();
            TransactResponse result = new TransactResponse();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_TRAMITE", OracleDbType.Int32, request.P_NID_TRAMITE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, request.P_NID_COTIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DFEC_REG", OracleDbType.Date, DateTime.Today, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE_ASSIGNOR", OracleDbType.Int32, request.P_NUSERCODE_ASSIGNOR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE_ASSIGNED", OracleDbType.Int32, request.P_NUSERCODE_ASSIGNED, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, request.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NSTATUS_TRA", OracleDbType.Int32, request.P_NSTATUS_TRA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SRUTA", OracleDbType.Varchar2, request.P_SRUTA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOMMENT", OracleDbType.Varchar2, request.P_SCOMMENT, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, result.P_MESSAGE, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, result.P_COD_ERR, ParameterDirection.Output);

                P_MESSAGE.Size = 9000;
                P_COD_ERR.Size = 9000;

                parameter.Add(P_MESSAGE);
                parameter.Add(P_COD_ERR);

                parameter.Add(new OracleParameter("P_FLAG_EJECCOM", OracleDbType.Varchar2, request.P_FLAG_EJECCOM, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDMOTIVO_RECHAZO", OracleDbType.Int32, request.P_NIDMOTIVO_RECHAZO, ParameterDirection.Input));
                this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                result.P_MESSAGE = P_MESSAGE.Value.ToString();
                result.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
            }
            catch (Exception ex)
            {
                result.P_MESSAGE = "Error en el servidor. " + ex.Message;
                result.P_COD_ERR = 1;
            }
            return result;
        }
        public List<StatusTransact> GetStatusListTransact()
        {
            List<StatusTransact> response = new List<StatusTransact>();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.pkg_Tramites + ".READ_TRAMITE_ESTADO";
            OracleDataReader dr = null;
            try
            {
                string[] arrayCursor = { "C_TABLE" };
                OracleParameter C_TABLE = new OracleParameter(arrayCursor[0], OracleDbType.RefCursor, ParameterDirection.Output);

                parameter.Add(C_TABLE);

                dr = (OracleDataReader)this.ExecuteByStoredProcedureVTCursores(storeprocedure, arrayCursor, parameter);

                foreach (var obj in dr.ReadRowsList<StatusTransact>())
                {
                    response.Add(obj);
                }
                ELog.CloseConnection(dr);
            }
            catch (Exception ex)
            {
                response = new List<StatusTransact>();
                ELog.CloseConnection(dr);
            }
            return response;
        }

        public GenericResponseTransact GetTransactList(TransactSearchRequest dataToSearch)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            GenericResponseTransact resultPackage = new GenericResponseTransact();
            List<TransactVM> list = new List<TransactVM>();

            string storedProcedureName = ProcedureName.pkg_Tramites + ".REA_TRAMITE";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_TYPE_SEARCH", OracleDbType.Int32, dataToSearch.TypeSearch, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_TRAMITE", OracleDbType.Varchar2, dataToSearch.TransactNumber, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Varchar2, dataToSearch.QuotationNumber, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Varchar2, dataToSearch.PolicyNumber, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Varchar2, dataToSearch.ProductType, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FECHA_DESDE", OracleDbType.Date, dataToSearch.StartDate, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FECHA_HASTA", OracleDbType.Date, dataToSearch.EndDate, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NSTATUS_TRA", OracleDbType.Varchar2, dataToSearch.Status, ParameterDirection.Input));

                parameter.Add(new OracleParameter("P_TIPO_BUS_CONT", OracleDbType.Varchar2, dataToSearch.ContractorSearchMode, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TIPO_DOC_CONT", OracleDbType.Varchar2, dataToSearch.ContractorDocumentType, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUM_DOC_CONT", OracleDbType.Varchar2, dataToSearch.ContractorDocumentNumber, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_RAZON_SOCIAL_CONT", OracleDbType.Varchar2, dataToSearch.ContractorLegalName, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_APE_PAT_CONT", OracleDbType.Varchar2, dataToSearch.ContractorPaternalLastName, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_APE_MAT_CONT", OracleDbType.Varchar2, dataToSearch.ContractorMaternalLastName, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NOMBRES_CONT", OracleDbType.Varchar2, dataToSearch.ContractorFirstName, ParameterDirection.Input));

                parameter.Add(new OracleParameter("P_TIPO_BUS_BR", OracleDbType.Varchar2, dataToSearch.BrokerSearchMode, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TIPO_DOC_BR", OracleDbType.Varchar2, dataToSearch.BrokerDocumentType, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUM_DOC_BR", OracleDbType.Varchar2, dataToSearch.BrokerDocumentNumber, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_RAZON_SOCIAL_BR", OracleDbType.Varchar2, dataToSearch.BrokerLegalName, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_APE_PAT_BR", OracleDbType.Varchar2, dataToSearch.BrokerPaternalLastName, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_APE_MAT_BR", OracleDbType.Varchar2, dataToSearch.BrokerMaternalLastName, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NOMBRES_BR", OracleDbType.Varchar2, dataToSearch.BrokerFirstName, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Varchar2, dataToSearch.User, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLIMITPERPAGE", OracleDbType.Int32, dataToSearch.LimitPerPage, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPAGENUM", OracleDbType.Int32, dataToSearch.PageNumber, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCOMPANY_LNK", OracleDbType.Int32, dataToSearch.CompanyLNK, ParameterDirection.Input)); // JDD
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, dataToSearch.Nbranch, ParameterDirection.Input)); // JDD
                parameter.Add(new OracleParameter("P_SISCLIENT_GBD", OracleDbType.Varchar2, dataToSearch.TypeClient, ParameterDirection.Input));
                OracleParameter table = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(table);
                OracleParameter totalRowNumber = new OracleParameter("P_NTOTALROWS", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameter.Add(totalRowNumber);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                try
                {
                    while (odr.Read())
                    {
                        TransactVM item = new TransactVM();
                        item.QuotationNumber = odr["NUM_COTIZACION"].ToString();
                        item.TransactNumber = Convert.ToInt32(odr["NUM_TRAMITE"].ToString());
                        item.PolicyNumber = odr["POLIZA"].ToString();
                        item.ProductName = odr["NOMBRE_PRODUCT"].ToString();
                        item.ProductId = odr["ID_PRODUCTO"].ToString();
                        item.ContractorFullName = odr["NOMBRE_CONTRATANTE"].ToString();
                        item.BrokerFullName = odr["CLIENT_BROKER"].ToString();
                        item.MinimalPremium = odr["PRIMA_MINIMA"].ToString();
                        item.WorkersCount = odr["NOM_TOTAL_TRAB"].ToString();
                        item.Payroll = odr["PLANILLA"].ToString();
                        item.Rate = odr["TASA"].ToString();
                        item.Bounty = odr["COMISION"].ToString();
                        item.Status = odr["DES_ESTADO_TRAMITE"].ToString();
                        item.Mode = odr["MODO"].ToString();
                        item.ApprovalDate = odr["FECHA_APROBACION"] != null && odr["FECHA_APROBACION"].ToString().Trim() != "" ? Convert.ToDateTime(odr["FECHA_APROBACION"].ToString()) : (DateTime?)null;
                        item.TypeTransac = odr["STRANSAC"] == DBNull.Value ? "" : odr["STRANSAC"].ToString();
                        item.UserCodeAssigned = odr["NUSERCODE_ASSIGNED"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NUSERCODE_ASSIGNED"].ToString());
                        item.UserAssigned = odr["USUARIO_ASSIGNED"] == DBNull.Value ? "" : Convert.ToString(odr["USUARIO_ASSIGNED"].ToString());
                        if (dataToSearch.TypeSearch == 2)
                        {
                            item.DerivateDate = odr["FECHA_DERIVACION"] != null && odr["FECHA_DERIVACION"].ToString().Trim() != "" ? Convert.ToDateTime(odr["FECHA_DERIVACION"].ToString()) : (DateTime?)null;
                        }
                        item.CreateDate = odr["FECHA_CREACION"] != null && odr["FECHA_CREACION"].ToString().Trim() != "" ? Convert.ToDateTime(odr["FECHA_CREACION"].ToString()) : (DateTime?)null;
                        item.typeClient = odr["TYPE_ACCOUNT"] == DBNull.Value ? "" : Convert.ToString(odr["TYPE_ACCOUNT"].ToString());
                        item.Tray = odr["BANDEJA"] == DBNull.Value ? "" : Convert.ToString(odr["BANDEJA"].ToString());
                        if (dataToSearch.TypeSearch == 1)
                        {
                            item.MailActual = odr["SMAIL_EJECCOM"] == DBNull.Value ? "" : Convert.ToString(odr["SMAIL_EJECCOM"].ToString());
                            item.ModeMail = odr["MODO_MAIL"] == DBNull.Value ? "" : Convert.ToString(odr["MODO_MAIL"].ToString());
                            item.ModeAssig = odr["MODO_ASSIG"] == DBNull.Value ? "" : Convert.ToString(odr["MODO_ASSIG"].ToString());
                        }
                        item.NCLIENT_SEG = odr["NCLIENT_SEG"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NCLIENT_SEG"]);
                        item.SCLIENT_SEG = odr["SCLIENT_SEG"] == DBNull.Value ? string.Empty : odr["SCLIENT_SEG"].ToString();
                        item.NTIEMPO_TOTAL_SLA = odr["NTIEMPO_TOTAL_SLA"] == DBNull.Value ? "" : odr["NTIEMPO_TOTAL_SLA"].ToString();
                        item.SDESCRIPT_SEG = odr["SDESCRIPT_SEG"] == DBNull.Value ? string.Empty : odr["SDESCRIPT_SEG"].ToString();
                        item.NFLAG_EXTERNO = odr["NFLAG_EXTERNO"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NFLAG_EXTERNO"]);
                        item.SPOL_ESTADO = odr["SPOL_ESTADO"] == DBNull.Value ? 0 : Convert.ToInt32(odr["SPOL_ESTADO"]);
                        item.SPOL_MATRIZ = odr["SPOL_MATRIZ"] == DBNull.Value ? 0 : Convert.ToInt32(odr["SPOL_MATRIZ"]);
                        item.APROB_CLI = odr["APROB_CLI"] == DBNull.Value ? 0 : Convert.ToInt32(odr["APROB_CLI"]);
                        item.SMAIL_EJECCOM = odr["SMAIL_EJECCOM"] == DBNull.Value ? "" : Convert.ToString(odr["SMAIL_EJECCOM"].ToString());


                        list.Add(item);
                    }
                    resultPackage.StatusCode = 0;
                    resultPackage.TotalRowNumber = Int32.Parse(totalRowNumber.Value.ToString());
                    resultPackage.GenericResponse = list;
                    ELog.CloseConnection(odr);
                }
                catch (Exception ex)
                {
                    ELog.CloseConnection(odr);
                }
                return resultPackage;
            }
            catch (Exception ex)
            {
                LogControl.save("GetTransactList", ex.ToString(), "3");
                return resultPackage;
            }
        }

        public string GetExcelTransactList(TransactSearchRequest data)
        {
            GenericResponseTransact response = this.GetTransactList(data);
            List<TransactVM> lista = (List<TransactVM>)response.GenericResponse;
            string templatePath = string.Empty;
            if (lista[0].NFLAG_EXTERNO == 1)
            {
                templatePath = "D:/doc_templates/reportes/dev/Template_consulta_tramites-ext.xlsx";
            }
            else
            {
                templatePath = "D:/doc_templates/reportes/dev/Template_consulta_tramites.xlsx";
            }
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
                    int i = 6;
                    int letra = 65;

                    sl.SetCellValue("B1", data.StartDate.ToString("dd/MM/yyyy"));
                    sl.SetCellValue("B2", data.EndDate.ToString("dd/MM/yyyy"));

                    foreach (TransactVM item in lista)
                    {
                        string fecha = "";
                        try
                        {
                            fecha = item.ApprovalDate == null ? "" : Convert.ToDateTime(item.ApprovalDate).ToString("dd/MM/yyyy hh:mm");
                        }
                        catch (Exception)
                        {
                            fecha = "";
                        }
                        int c = 0;
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.ProductName);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.TransactNumber);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.QuotationNumber);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.PolicyNumber);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.ContractorFullName);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.typeClient);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.BrokerFullName);
                        if (item.NFLAG_EXTERNO == 0)
                        {
                            sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.SDESCRIPT_SEG);
                            sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.NTIEMPO_TOTAL_SLA);
                        }
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.TypeTransac == "Broker" ? "Modificar Broker" : item.TypeTransac);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.Status);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.Tray);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, item.UserAssigned);
                        sl.SetCellValue(Char.ToString((char)(letra + c++)) + i, fecha);
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
                plantilla = "";
                throw ex;
            }
            return plantilla;

        }

        public TransactModel GetInfoTransact(RequestTransact request)
        {
            TransactModel response = new TransactModel();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.pkg_Tramites + ".REA_TRAMITE_CAB";
            OracleDataReader dr = null;
            try
            {
                parameter.Add(new OracleParameter("P_NID_TRAMITE", OracleDbType.Int32, request.P_NID_TRAMITE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, request.P_NID_COTIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, request.P_NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, request.P_NBRANCH, ParameterDirection.Input));

                string[] arrayCursor = { "C_TABLE" };
                OracleParameter C_TABLE = new OracleParameter(arrayCursor[0], OracleDbType.RefCursor, ParameterDirection.Output);

                parameter.Add(C_TABLE);

                dr = (OracleDataReader)this.ExecuteByStoredProcedureVTCursores(storeprocedure, arrayCursor, parameter);

                response = dr.ReadRowsList<TransactModel>()[0];
                ELog.CloseConnection(dr);
                try
                {
                    if (response.SLETTER_AGENCY != null)
                    {
                        if (!String.IsNullOrEmpty(response.SLETTER_AGENCY.ToString()))
                        {
                            response.PATHS_FILES_LETTER = sharedMethods.GetFilePathList(String.Format(ELog.obtainConfig("pathPrincipal"), response.SLETTER_AGENCY.ToString()));
                        }
                    }
                }
                catch (Exception) { }
            }
            catch (Exception ex)
            {
                response = new TransactModel();
                ELog.CloseConnection(dr);
            }
            return response;
        }

        public TransactResponse InsertHistTransact(RequestTransact request)
        {
            var sPackageName = ProcedureName.pkg_Tramites + ".INS_TRAMITES_HIS";

            List<OracleParameter> parameter = new List<OracleParameter>();
            TransactResponse result = new TransactResponse();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_TRAMITE", OracleDbType.Int32, request.P_NID_TRAMITE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, request.P_NID_COTIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DFEC_REG", OracleDbType.Date, DateTime.Today, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPERFIL", OracleDbType.Int32, request.P_NPERFIL, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, request.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE_ASSIGNOR", OracleDbType.Int32, request.P_NUSERCODE_ASSIGNOR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NSTATUS_TRA", OracleDbType.Int32, request.P_NSTATUS_TRA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NFLAG_END", OracleDbType.Int32, request.P_NFLAG_END, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NFLAG_EMAIL", OracleDbType.Int32, request.P_NFLAG_EMAIL, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SRUTA", OracleDbType.Varchar2, request.P_SRUTA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOMMENT", OracleDbType.Varchar2, request.P_SCOMMENT, ParameterDirection.Input));

                this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                result.P_MESSAGE = "";
                result.P_COD_ERR = 0;
            }
            catch (Exception ex)
            {
                result.P_MESSAGE = "Error en el servidor. " + ex.Message;
                result.P_COD_ERR = 1;
            }
            return result;
        }

        public TransactResponse UpdateTransact(RequestTransact request, DbConnection connection = null, DbTransaction trx = null)
        {
            var sPackageName = ProcedureName.pkg_Tramites + ".UPD_TRAMITE_DERIVAR";

            List<OracleParameter> parameter = new List<OracleParameter>();
            TransactResponse result = new TransactResponse();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_TRAMITE", OracleDbType.Int32, request.P_NID_TRAMITE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, request.P_NID_COTIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DFEC_REG", OracleDbType.Date, Convert.ToDateTime(request.P_DFEC_REG), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, request.P_NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, request.P_NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STRANSAC", OracleDbType.Varchar2, request.P_STRANSAC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int64, request.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SMAIL_EJECCOM", OracleDbType.Varchar2, request.P_SMAIL_EJECCOM, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SRUTA", OracleDbType.Varchar2, request.P_SRUTA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOMMENT", OracleDbType.Varchar2, request.P_SCOMMENT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SDEVOLPRI", OracleDbType.Varchar2, request.P_SDEVOLPRI, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTIP_RENOV", OracleDbType.Int32, request.P_NTIP_RENOV, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPAYFREQ", OracleDbType.Int32, request.P_NPAYFREQ, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPENDOSO", OracleDbType.Int32, request.P_NTYPENDOSO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDPLAN", OracleDbType.Int32, request.P_NIDPLAN, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NINTERMED", OracleDbType.Int32, request.P_NINTERMED, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, result.P_MESSAGE, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, result.P_COD_ERR, ParameterDirection.Output);

                P_MESSAGE.Size = 9000;
                P_COD_ERR.Size = 9000;

                parameter.Add(P_MESSAGE);
                parameter.Add(P_COD_ERR);

                parameter.Add(new OracleParameter("P_SLETTER_AGENCY", OracleDbType.Varchar2, request.P_SLETTER_AGENCY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, request.P_SCLIENT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SPOL_MATRIZ", OracleDbType.Varchar2, request.P_SPOL_MATRIZ, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SPOL_ESTADO", OracleDbType.Int32, request.P_SPOL_ESTADO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_APROB_CLI", OracleDbType.Int32, request.P_APROB_CLI, ParameterDirection.Input));

                // mejoras CIIU VL 
                parameter.Add(new OracleParameter("P_SCOD_ACTIVITY_TEC", OracleDbType.Varchar2, request.P_SCOD_ACTIVITY_TEC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOD_CIUU", OracleDbType.Varchar2, request.P_SCOD_CIUU, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NFLAG_UPD", OracleDbType.Int32, request.P_NFLAG_UPD, ParameterDirection.Input));

                if (connection == null)
                {
                    this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                }
                else
                {
                    this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, connection, trx);
                }

                result.P_MESSAGE = P_MESSAGE.Value.ToString();
                result.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
            }
            catch (Exception ex)
            {
                result.P_NID_TRAMITE = 0;
                result.P_MESSAGE = "Error en el servidor. " + ex.Message;
                result.P_COD_ERR = 1;
            }
            return result;
        }

        public TransactModel GetInfoLastTransact(RequestTransact request)
        {
            TransactModel response = new TransactModel();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.pkg_Tramites + ".GET_TRAMITE_ANTERIOR";
            OracleDataReader dr = null;
            try
            {
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, request.P_NID_COTIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, request.P_NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, request.P_NBRANCH, ParameterDirection.Input));

                string[] arrayCursor = { "C_TABLE" };
                OracleParameter C_TABLE = new OracleParameter(arrayCursor[0], OracleDbType.RefCursor, ParameterDirection.Output);

                parameter.Add(C_TABLE);

                parameter.Add(new OracleParameter("P_STRANSAC", OracleDbType.Varchar2, request.P_STRANSAC, ParameterDirection.Input));

                dr = (OracleDataReader)this.ExecuteByStoredProcedureVTCursores(storeprocedure, arrayCursor, parameter);

                if (dr.HasRows) response = dr.ReadRowsList<TransactModel>()[0];

                ELog.CloseConnection(dr);
            }
            catch (Exception ex)
            {
                response = new TransactModel();
                ELog.CloseConnection(dr);
            }
            return response;
        }
        public TransactResponse UpdateBroker(RequestTransact request)
        {
            var sPackageName = ProcedureName.pkg_Tramites + ".UPD_TRAMITE_BROKER";

            List<OracleParameter> parameter = new List<OracleParameter>();
            TransactResponse result = new TransactResponse();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_TRAMITE", OracleDbType.Int32, request.P_NID_TRAMITE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, request.P_NID_COTIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, request.P_SCLIENT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, request.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Date, Convert.ToDateTime(request.P_DFEC_REG), ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, result.P_MESSAGE, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, result.P_COD_ERR, ParameterDirection.Output);

                P_MESSAGE.Size = 9000;
                P_COD_ERR.Size = 9000;

                parameter.Add(P_MESSAGE);
                parameter.Add(P_COD_ERR);

                this.ExecuteByStoredProcedureVT(sPackageName, parameter);

                result.P_MESSAGE = P_MESSAGE.Value.ToString();
                result.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());

            }
            catch (Exception ex)
            {
                result.P_MESSAGE = "Error en el servidor. " + ex.Message;
                result.P_COD_ERR = 1;
            }
            return result;
        }

        public TransactResponse ValidateAccess(RequestTransact request)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            TransactResponse response = new TransactResponse();
            string storedProcedureName = ProcedureName.pkg_Tramites + ".LOGIN_RUC_TRAMITE";

            try
            {
                parameter.Add(new OracleParameter("P_SLINK", OracleDbType.Varchar2, request.token, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCLINUMDOCU", OracleDbType.Char, request.documento, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, response.P_MESSAGE, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, response.P_COD_ERR, ParameterDirection.Output);
                OracleParameter P_NID_COTIZACION = new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, response.P_NID_COTIZACION, ParameterDirection.Output);
                OracleParameter P_NUSERCODE = new OracleParameter("P_NUSERCODE", OracleDbType.Int32, response.P_NUSERCODE, ParameterDirection.Output);
                OracleParameter P_NPRODUCT = new OracleParameter("P_NPRODUCT", OracleDbType.Int32, response.P_NPRODUCT, ParameterDirection.Output);
                OracleParameter P_NBRANCH = new OracleParameter("P_NBRANCH", OracleDbType.Int32, response.P_NBRANCH, ParameterDirection.Output);
                OracleParameter P_NID_TRAMITE = new OracleParameter("P_NID_TRAMITE", OracleDbType.Int32, response.P_NID_TRAMITE, ParameterDirection.Output);
                OracleParameter P_FLAG_RECHAZO = new OracleParameter("P_FLAG_RECHAZO", OracleDbType.Int32, response.P_FLAG_RECHAZO, ParameterDirection.Output);
                P_MESSAGE.Size = 4000;

                parameter.Add(P_MESSAGE);
                parameter.Add(P_COD_ERR);
                parameter.Add(P_NID_COTIZACION);
                parameter.Add(P_NUSERCODE);
                parameter.Add(P_NPRODUCT);
                parameter.Add(P_NBRANCH);
                parameter.Add(P_NID_TRAMITE);
                parameter.Add(P_FLAG_RECHAZO);

                this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                response.P_MESSAGE = P_MESSAGE.Value.ToString();
                response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                response.P_NID_COTIZACION = Convert.ToInt32(P_NID_COTIZACION.Value.ToString());
                response.P_NUSERCODE = Convert.ToInt32(P_NUSERCODE.Value.ToString());
                response.P_NPRODUCT = Convert.ToInt32(P_NPRODUCT.Value.ToString());
                response.P_NBRANCH = Convert.ToInt32(P_NBRANCH.Value.ToString());
                response.P_NID_TRAMITE = Convert.ToInt32(P_NID_TRAMITE.Value.ToString());
                response.P_FLAG_RECHAZO = Convert.ToInt32(P_FLAG_RECHAZO.Value.ToString());

                if (response.P_NID_COTIZACION == 0)
                {
                    response.P_COD_ERR = 1;
                    response.P_MESSAGE = "Hubo un error al validar el acceso.";
                }
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = "Hubo un error al validar el acceso.";
                throw ex;
            }
            return response;
        }

        public TransactResponse ValidateAccessDes(RequestTransact request)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            TransactResponse response = new TransactResponse();
            string storedProcedureName = ProcedureName.pkg_Tramites + ".LOGIN_RUC_TRAMITE_DESGRAVAMEN";

            try
            {
                parameter.Add(new OracleParameter("P_SLINK", OracleDbType.Varchar2, request.token, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCLINUMDOCU", OracleDbType.Char, request.documento, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, response.P_MESSAGE, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, response.P_COD_ERR, ParameterDirection.Output);
                OracleParameter P_NID_COTIZACION = new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, response.P_NID_COTIZACION, ParameterDirection.Output);
                OracleParameter P_NUSERCODE = new OracleParameter("P_NUSERCODE", OracleDbType.Int32, response.P_NUSERCODE, ParameterDirection.Output);
                OracleParameter P_NPRODUCT = new OracleParameter("P_NPRODUCT", OracleDbType.Int32, response.P_NPRODUCT, ParameterDirection.Output);
                OracleParameter P_NBRANCH = new OracleParameter("P_NBRANCH", OracleDbType.Int32, response.P_NBRANCH, ParameterDirection.Output);

                P_MESSAGE.Size = 4000;

                parameter.Add(P_MESSAGE);
                parameter.Add(P_COD_ERR);
                parameter.Add(P_NID_COTIZACION);
                parameter.Add(P_NUSERCODE);
                parameter.Add(P_NPRODUCT);
                parameter.Add(P_NBRANCH);

                this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                response.P_MESSAGE = P_MESSAGE.Value.ToString();
                response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                response.P_NID_COTIZACION = Convert.ToInt32(P_NID_COTIZACION.Value.ToString());
                response.P_NUSERCODE = Convert.ToInt32(P_NUSERCODE.Value.ToString());
                response.P_NPRODUCT = Convert.ToInt32(P_NPRODUCT.Value.ToString());
                response.P_NBRANCH = Convert.ToInt32(P_NBRANCH.Value.ToString());
                if (response.P_NID_COTIZACION == 0)
                {
                    response.P_COD_ERR = 1;
                    response.P_MESSAGE = "Hubo un error al validar el acceso.";
                }
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = "Hubo un error al validar el acceso.";
                throw ex;
            }
            return response;
        }

        public GenericResponseTransact GetVigenciaAnterior(RequestTransact request)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            GenericResponseTransact resultPackage = new GenericResponseTransact();
            DatesRenovacionTransact response = new DatesRenovacionTransact();
            string storedProcedureName = "INSUDB.PKG_PD_VALIDACION_GEN" + ".SP_GET_DATOS_VIGENCIA";

            try
            {
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, request.P_NID_COTIZACION, ParameterDirection.Input));

                //OUTPUT
                OracleParameter table = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(table);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);
                try
                {
                    while (odr.Read())
                    {
                        response.DSTARTDATE = odr["DSTARTDATE"] != null && odr["DSTARTDATE"].ToString().Trim() != "" ? Convert.ToDateTime(odr["DSTARTDATE"].ToString()) : (DateTime?)null;
                        response.DEXPIRDAT = odr["DEXPIRDAT"] != null && odr["DEXPIRDAT"].ToString().Trim() != "" ? Convert.ToDateTime(odr["DEXPIRDAT"].ToString()) : (DateTime?)null;
                        response.DSTARTDATE_ASE = odr["DSTARTDATE_ASE"] != null && odr["DSTARTDATE_ASE"].ToString().Trim() != "" ? Convert.ToDateTime(odr["DSTARTDATE_ASE"].ToString()) : (DateTime?)null;
                        response.DEXPIRDAT_ASE = odr["DEXPIRDAT_ASE"] != null && odr["DEXPIRDAT_ASE"].ToString().Trim() != "" ? Convert.ToDateTime(odr["DEXPIRDAT_ASE"].ToString()) : (DateTime?)null;
                    }
                    resultPackage.StatusCode = 0;
                    resultPackage.GenericResponse = response;
                    ELog.CloseConnection(odr);
                }
                catch (Exception)
                {
                    ELog.CloseConnection(odr);
                }
                return resultPackage;
            }
            catch (Exception ex)
            {
                LogControl.save("GetVigenciaAnterior", ex.ToString(), "3");
                return resultPackage;
            }
        }

        public TransactResponse FinalizarTramite(RequestTransact request)
        {
            var sPackageName = ProcedureName.pkg_Tramites + ".INS_TRAMITE_FINALIZAR";

            List<OracleParameter> parameter = new List<OracleParameter>();
            TransactResponse result = new TransactResponse();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, request.P_NID_COTIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DFEC_REG", OracleDbType.Date, DateTime.Today, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, request.P_NUSERCODE, ParameterDirection.Input));

                this.ExecuteByStoredProcedureVT(sPackageName, parameter);

            }
            catch (Exception ex)
            {
                //result.P_MESSAGE = "Error en el servidor. " + ex.Message;
                result.P_MESSAGE = "Error en el servidor.";
                result.P_COD_ERR = 1;
                LogControl.save("FinalizarTramite", ex.ToString(), "3");
            }
            return result;
        }

        public TransactResponse AnularTramite(RequestTransact request)
        {
            var sPackageName = ProcedureName.pkg_Tramites + ".INS_TRAMITE_BANDEJA_FINALIZAR";

            List<OracleParameter> parameter = new List<OracleParameter>();
            TransactResponse result = new TransactResponse();
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_TRAMITE", OracleDbType.Int32, request.P_NID_TRAMITE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, request.P_NID_COTIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, request.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NSTATUS_TRA", OracleDbType.Int32, request.P_NSTATUS_TRA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SRUTA", OracleDbType.Varchar2, request.P_SRUTA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOMMENT", OracleDbType.Varchar2, request.P_SCOMMENT, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, result.P_MESSAGE, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, result.P_COD_ERR, ParameterDirection.Output);

                P_MESSAGE.Size = 9000;
                P_COD_ERR.Size = 9000;

                parameter.Add(P_MESSAGE);
                parameter.Add(P_COD_ERR);

                this.ExecuteByStoredProcedureVT(sPackageName, parameter);

                result.P_MESSAGE = P_MESSAGE.Value.ToString();
                result.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());

            }
            catch (Exception ex)
            {
                result.P_MESSAGE = "Error en el servidor. " + ex.Message;
                result.P_COD_ERR = 1;
            }
            return result;
        }
        public bool UpdateDevolvPri(int cotizacion, int devolvPri)
        {
            var sPackageName = ProcedureName.pkg_Tramites + ".UPD_DEVOLVER_PRIMA";

            List<OracleParameter> parameter = new List<OracleParameter>();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, cotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SDEVOLPRI", OracleDbType.Int32, devolvPri, ParameterDirection.Input));

                this.ExecuteByStoredProcedureVT(sPackageName, parameter);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public TransactResponse PerfilTramiteOpe(RequestTransact request)
        {
            var sPackageName = ProcedureName.pkg_Tramites + ".FN_NFLAG_PERFIL";

            List<OracleParameter> parameter = new List<OracleParameter>();
            TransactResponse result = new TransactResponse();
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, request.P_NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, request.P_NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPERFIL", OracleDbType.Int32, request.P_NPERFIL, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_NFLAG_PERFIL = new OracleParameter("P_NFLAG_PERFIL", OracleDbType.Int32, result.P_NFLAG_PERFIL, ParameterDirection.Output);

                P_NFLAG_PERFIL.Size = 9000;

                parameter.Add(P_NFLAG_PERFIL);

                this.ExecuteByStoredProcedureVT(sPackageName, parameter);

                result.P_NFLAG_PERFIL = Convert.ToInt32(P_NFLAG_PERFIL.Value.ToString());

            }
            catch (Exception ex)
            {
                result.P_MESSAGE = "Error en el servidor. " + ex.Message;
                result.P_COD_ERR = 1;
            }
            return result;
        }

        public TransactResponse PerfilComercialEx(RequestTransact request)
        {
            var sPackageName = ProcedureName.pkg_Tramites + ".FN_ACTIVE_PROFILE";

            List<OracleParameter> parameter = new List<OracleParameter>();
            TransactResponse result = new TransactResponse();
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, request.P_NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, request.P_NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, request.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPERFIL", OracleDbType.Int32, request.P_NPERFIL, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_FLAG_ACTIVE = new OracleParameter("P_FLAG_ACTIVE", OracleDbType.Int32, result.P_FLAG_ACTIVE, ParameterDirection.Output);

                P_FLAG_ACTIVE.Size = 9000;

                parameter.Add(P_FLAG_ACTIVE);

                this.ExecuteByStoredProcedureVT(sPackageName, parameter);

                result.P_FLAG_ACTIVE = Convert.ToInt32(P_FLAG_ACTIVE.Value.ToString());

            }
            catch (Exception ex)
            {
                result.P_MESSAGE = "Error en el servidor. " + ex.Message;
                result.P_COD_ERR = 1;
            }
            return result;
        }

        public TransactResponse UpdateMail(RequestTransact request)
        {
            var sPackageName = ProcedureName.pkg_Tramites + ".UPD_TRAMITE_MAILSEND";

            List<OracleParameter> parameter = new List<OracleParameter>();
            TransactResponse result = new TransactResponse();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_TRAMITE", OracleDbType.Int32, request.P_NID_TRAMITE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, request.P_NID_COTIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SMAIL_EJECCOM", OracleDbType.Varchar2, request.P_SMAIL_SEND, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Varchar2, request.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOMMENT", OracleDbType.Varchar2, request.P_SCOMMENT, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, result.P_MESSAGE, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, result.P_COD_ERR, ParameterDirection.Output);

                P_MESSAGE.Size = 9000;
                P_COD_ERR.Size = 9000;

                parameter.Add(P_MESSAGE);
                parameter.Add(P_COD_ERR);

                this.ExecuteByStoredProcedureVT(sPackageName, parameter);

                result.P_MESSAGE = P_MESSAGE.Value.ToString();
                result.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());

            }
            catch (Exception ex)
            {
                //result.P_MESSAGE = "Error en el servidor. " + ex.Message;
                result.P_MESSAGE = "Error en el servidor. ";
                result.P_COD_ERR = 1;
                LogControl.save("UpdateMail", ex.ToString(), "3");
            }
            return result;
        }

        public TransactResponse InsReingresarTransact(RequestTransact request, DbConnection connection = null, DbTransaction trx = null)
        {
            var sPackageName = ProcedureName.pkg_Tramites + ".INS_TRAMITE_REINGRESO";

            List<OracleParameter> parameter = new List<OracleParameter>();
            TransactResponse result = new TransactResponse();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, request.P_NID_COTIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_TRAMITE_PRE", OracleDbType.Int32, request.P_NID_TRAMITE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DFEC_TRANSAC", OracleDbType.Date, Convert.ToDateTime(request.P_DFEC_REG), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STRANSAC", OracleDbType.Varchar2, request.P_STRANSAC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int64, request.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SMAIL_EJECCOM", OracleDbType.Varchar2, request.P_SMAIL_EJECCOM, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SRUTA", OracleDbType.Varchar2, request.P_SRUTA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOMMENT", OracleDbType.Varchar2, request.P_SCOMMENT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SDEVOLPRI", OracleDbType.Varchar2, request.P_SDEVOLPRI, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTIP_RENOV", OracleDbType.Int32, request.P_NTIP_RENOV, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPAYFREQ", OracleDbType.Int32, request.P_NPAYFREQ, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPENDOSO", OracleDbType.Int32, request.P_NTYPENDOSO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDPLAN", OracleDbType.Int32, request.P_NIDPLAN, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NINTERMED", OracleDbType.Int32, request.P_NINTERMED, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLETTER_AGENCY", OracleDbType.Varchar2, request.P_SLETTER_AGENCY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, request.P_SCLIENT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SPOL_MATRIZ", OracleDbType.Varchar2, request.P_SPOL_MATRIZ, ParameterDirection.Input));

                //parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, request.P_NPRODUCT, ParameterDirection.Input));
                //parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, request.P_NBRANCH, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_NID_TRAMITE = new OracleParameter("P_NID_TRAMITE", OracleDbType.Int32, result.P_COD_ERR, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, result.P_MESSAGE, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, result.P_COD_ERR, ParameterDirection.Output);

                P_NID_TRAMITE.Size = 9000;
                P_MESSAGE.Size = 9000;
                P_COD_ERR.Size = 9000;

                parameter.Add(P_NID_TRAMITE);
                parameter.Add(P_MESSAGE);
                parameter.Add(P_COD_ERR);
                parameter.Add(new OracleParameter("P_SPOL_ESTADO", OracleDbType.Int32, request.P_SPOL_ESTADO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_APROB_CLI", OracleDbType.Int32, request.P_APROB_CLI, ParameterDirection.Input));
                // mejoras CIIU VL INI
                parameter.Add(new OracleParameter("P_SCOD_ACTIVITY_TEC", OracleDbType.Varchar2, request.P_SCOD_ACTIVITY_TEC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOD_CIUU", OracleDbType.Varchar2, request.P_SCOD_CIUU, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NFLAG_UPD", OracleDbType.Int32, request.P_NFLAG_UPD, ParameterDirection.Input));
                // mejoras CIIU VL FIN
                if (connection == null)
                {
                    this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                }
                else
                {
                    this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, connection, trx);
                }

                result.P_NID_TRAMITE = Convert.ToInt32(P_NID_TRAMITE.Value.ToString());
                result.P_MESSAGE = P_MESSAGE.Value.ToString();
                result.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());

            }
            catch (Exception ex)
            {
                result.P_MESSAGE = "Error en el servidor. " + ex.Message;
                result.P_COD_ERR = 1;
            }
            return result;
        }

        //R.P.

        public List<BrokerObl> GetBrokerObl(int nbranch)
        {
            List<BrokerObl> response = new List<BrokerObl>();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.pkg_Tramites + ".REA_BROKER_OBL";
            try
            {
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, nbranch, ParameterDirection.Input));

                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storeprocedure, parameter))
                {
                    response = dr.ReadRowsList<BrokerObl>();
                    ELog.CloseConnection(dr);
                }
            }
            catch (Exception ex)
            {
                response = new List<BrokerObl>();
            }
            return response;
        }

        public TransactResponse InsertDepBrkTramite(DepBroker databrkdep)
        {
            TransactResponse response = new TransactResponse();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.pkg_Tramites + ".INS_TRAMITE_BROKER_DEP";

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_TRAMITE", OracleDbType.Int32, databrkdep.P_NID_TRAMITE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, databrkdep.P_NID_COTIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCLIENT_COMER", OracleDbType.Varchar2, databrkdep.P_SCLIENT_COMER, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLOCAL", OracleDbType.Int32, databrkdep.P_NLOCAL, ParameterDirection.Input));


                //parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, request.P_NPRODUCT, ParameterDirection.Input));
                //parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, request.P_NBRANCH, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, response.P_MESSAGE, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, response.P_COD_ERR, ParameterDirection.Output);

                P_MESSAGE.Size = 9000;
                P_COD_ERR.Size = 9000;

                parameter.Add(P_MESSAGE);
                parameter.Add(P_COD_ERR);

                this.ExecuteByStoredProcedureVT(storeprocedure, parameter);

                response.P_MESSAGE = P_MESSAGE.Value.ToString();
                response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());

            }
            catch (Exception ex)
            {
                response.P_MESSAGE = "Error en el servidor. " + ex.Message;
                response.P_COD_ERR = 1;
            }

            return response;
        }
        //R.P.



        public List<MotivoRechazoVM> GetMotivoRechazoTransact()
        {
            List<MotivoRechazoVM> lista = new List<MotivoRechazoVM>();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storedProcedureName = ProcedureName.pkg_Tramites + ".REA_MOTIVO_RECHZAZO";
            try
            {
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, lista, ParameterDirection.Output);
                parameter.Add(C_TABLE);
                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);
                while (odr.Read())
                {
                    MotivoRechazoVM motivoRechazo = new MotivoRechazoVM();
                    motivoRechazo.COD_MOTIVO = Convert.ToInt32(odr["COD_MOTIVO"]);
                    motivoRechazo.DES_MOTIVO = odr["DES_MOTIVO"].ToString();
                    lista.Add(motivoRechazo);
                }
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("GetMotivoRechazoTransact", ex.ToString() + "error al obtener motivos de rechazo", "3");
            }
            return lista;

        }

        public TransactResponse ValidarRechazoEjecutivo(ValidateEjecutivoTransac request)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            TransactResponse response = new TransactResponse();
            string storedProcedureName = ProcedureName.pkg_Tramites + ".VALIDATE_EJECUTIVO_TRAMITE";

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_TRAMITE", OracleDbType.Int32, request.P_NID_TRAMITE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, request.P_NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, request.P_NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTIPO", OracleDbType.Int32, ELog.obtainConfig("rechazoEjecutivo"), ParameterDirection.Input));
                // OUTPUT

                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, response.P_MESSAGE, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, response.P_COD_ERR, ParameterDirection.Output);
                OracleParameter P_FLAG_RECHAZO = new OracleParameter("P_FLAG_RECHAZO", OracleDbType.Int32, response.P_FLAG_RECHAZO, ParameterDirection.Output);
                P_MESSAGE.Size = 4000;

                parameter.Add(P_MESSAGE);
                parameter.Add(P_COD_ERR);
                parameter.Add(P_FLAG_RECHAZO);

                this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);
                response.P_MESSAGE = P_MESSAGE.Value.ToString();
                response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                response.P_FLAG_RECHAZO = Convert.ToInt32(P_FLAG_RECHAZO.Value.ToString());

                if (response.P_FLAG_RECHAZO == 1)
                {
                    response.P_COD_ERR = 1;
                    response.P_MESSAGE = "Hubo un error al validar el status del trámite.";
                }
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = "Hubo un error al validar el status del trámite.";
                LogControl.save("ValidarRechazoEjecutivo", ex.ToString(), "3");
            }
            return response;
        }





    }
}
