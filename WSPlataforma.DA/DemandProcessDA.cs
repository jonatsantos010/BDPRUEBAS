using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using WSPlataforma.Entities.DemandProcessModel.BindingModel;
using WSPlataforma.Entities.DemandProcessModel.ViewModel;
using WSPlataforma.Entities.ViewModel;
using WSPlataforma.Util;
using WSPlataforma.Util.PrintPolicyUtility;

namespace WSPlataforma.DA
{
    public class DemandProcessDA : ConnectionBase
    {
        public ResponseVM SendBillsMassive()
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseVM response = new ResponseVM();
            string storedProcedureName = ProcedureName.pkg_ProcesoDemanda + ".INSUPD_SEND_BILLS_MASSIVE";

            try
            {
                this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return response;
        }

        public ResponseVM UpdateDemandState(DemandProcessBM demandProcessBM)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseVM response = new ResponseVM();
            string storedProcedureName = ProcedureName.pkg_ProcesoDemanda + ".INSUPD_BILLS_NBILLNEW";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_SBILLTYPE", OracleDbType.Varchar2, demandProcessBM.SBILLTYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NINSUR_AREA", OracleDbType.Int32, demandProcessBM.NINSUR_AREA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBILLNUM", OracleDbType.Int64, demandProcessBM.NBILLNUM, ParameterDirection.Input));
                //parameter.Add(new OracleParameter("P_NNEWBILL", OracleDbType.Int32, demandProcessBM.NNEWBILL, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, response.P_NCODE, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, response.P_SMESSAGE, ParameterDirection.Output);

                //P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;
                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);

                this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                response.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                response.P_SMESSAGE = P_SMESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                response.P_NCODE = 1;
                response.P_SMESSAGE = ex.Message;
                return response;
            }

            return response;
        }

        public SunatResponseVM ExecuteServiceSunat(SunatRequestBM sunatSendBM)
        {
            string baseUrlApi = ELog.obtainConfig("sunatBase");
            SunatResponseVM responsePrint = CallGenericService<SunatResponseVM, SunatRequestBM>.PostRequest(baseUrlApi, "/demanda", sunatSendBM);
            return responsePrint;
        }

        public List<DemandProcessVM> GetDemandProcessList(DemandProcessBM demandProcessBM)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<DemandProcessVM> DemandProcessList = new List<DemandProcessVM>();
            string storedProcedureName = ProcedureName.pkg_ProcesoDemanda + ".REA_LIST_DOCUMENTOS";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, demandProcessBM.NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, demandProcessBM.NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int64, demandProcessBM.NPOLICY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SBILLTYPE", OracleDbType.Varchar2, demandProcessBM.SBILLTYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NINSUR_AREA", OracleDbType.Int32, demandProcessBM.NINSUR_AREA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBILLNUM", OracleDbType.Int64, demandProcessBM.NBILLNUM, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DDESDE", OracleDbType.Date, demandProcessBM.DDESDE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DHASTA", OracleDbType.Date, demandProcessBM.DHASTA, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        DemandProcessVM item = new DemandProcessVM();
                        item.NBRANCH = odr["NBRANCH"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NBRANCH"].ToString());
                        item.SBRANCH = odr["SBRANCH"] == DBNull.Value ? string.Empty : odr["SBRANCH"].ToString();
                        item.NPRODUCT = odr["NPRODUCT"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NPRODUCT"].ToString());
                        item.SPRODUCT = odr["SPRODUCT"] == DBNull.Value ? string.Empty : odr["SPRODUCT"].ToString();
                        item.NPOLICY = odr["NPOLICY"] == DBNull.Value ? 0 : Convert.ToInt64(odr["NPOLICY"].ToString());
                        item.NBILLNUM = odr["NBILLNUM"] == DBNull.Value ? 0 : Convert.ToInt64(odr["NBILLNUM"].ToString());
                        item.NPREMIUM = odr["NPREMIUM"] == DBNull.Value ? 0 : Convert.ToDouble(odr["NPREMIUM"].ToString());
                        item.SBILLTYPE = odr["SBILLTYPE"] == DBNull.Value ? string.Empty : odr["SBILLTYPE"].ToString();
                        item.SBILLING = odr["SBILLING"] == DBNull.Value ? string.Empty : odr["SBILLING"].ToString();
                        item.NINSUR_AREA = odr["NINSUR_AREA"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NINSUR_AREA"].ToString());
                        item.DSTATDATE = odr["DSTATDATE"] == DBNull.Value ? string.Empty : odr["DSTATDATE"].ToString();
                        item.SSTATE = odr["SSTATE"] == DBNull.Value ? string.Empty : odr["SSTATE"].ToString();
                        DemandProcessList.Add(item);
                    }
                }
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return DemandProcessList;
        }
    }
}
