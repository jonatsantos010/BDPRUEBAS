using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.Entities.CoverModel.BindingModel;
using WSPlataforma.Entities.CoverModel.ViewModel;
using WSPlataforma.Entities.ViewModel;
using WSPlataforma.Util;

namespace WSPlataforma.DA
{
    public class CoverGenDA : ConnectionBase
    {
        public List<CoverGenVM> GetCoverGenByCode(CoverGenBM coverGenBM)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<CoverGenVM> CoverGenList = new List<CoverGenVM>();

            string storedProcedureName = ProcedureName.pkg_CargaMasiva + ".GET_COVER_GEN_BY_CODE";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NCOVERGEN", OracleDbType.Decimal, coverGenBM.NCOVERGEN, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        CoverGenVM item = new CoverGenVM();

                        item.SDESCRIPT = odr["SDESCRIPT"] == DBNull.Value ? "" : odr["SDESCRIPT"].ToString();
                        item.SSHORT_DES = odr["SSHORT_DES"] == DBNull.Value ? "" : odr["SSHORT_DES"].ToString();
                        item.SROUSURRE = odr["SROUSURRE"] == DBNull.Value ? "" : odr["SROUSURRE"].ToString();
                        item.SCONDSVS = odr["SCONDSVS"] == DBNull.Value ? "" : odr["SCONDSVS"].ToString();
                        item.NCURRENCY = odr["NCURRENCY"] == DBNull.Value ? 0 : decimal.Parse(odr["NCURRENCY"].ToString());
                        item.SINFORPROV = odr["SINFORPROV"] == DBNull.Value ? "" : odr["SINFORPROV"].ToString();
                        item.NBRANCH_LED = odr["NBRANCH_LED"] == DBNull.Value ? 0 : decimal.Parse(odr["NBRANCH_LED"].ToString());
                        item.NBRANCH_REI = odr["NBRANCH_REI"] == DBNull.Value ? 0 : decimal.Parse(odr["NBRANCH_REI"].ToString());
                        item.NBRANCH_EST = odr["NBRANCH_EST"] == DBNull.Value ? 0 : decimal.Parse(odr["NBRANCH_EST"].ToString());
                        item.NBRANCH_GEN = odr["NBRANCH_GEN"] == DBNull.Value ? 0 : decimal.Parse(odr["NBRANCH_GEN"].ToString());
                        item.NCLA_LI_TYP = odr["NCLA_LI_TYP"] == DBNull.Value ? 0 : decimal.Parse(odr["NCLA_LI_TYP"].ToString());
                        item.NCACALFIX = odr["NCACALFIX"] == DBNull.Value ? 0 : decimal.Parse(odr["NCACALFIX"].ToString());
                        item.SCAPIPREM = odr["SCAPIPREM"] == DBNull.Value ? "" : odr["SCAPIPREM"].ToString();
                        item.SPREMCAPI = odr["SPREMCAPI"] == DBNull.Value ? "" : odr["SPREMCAPI"].ToString();
                        item.NCOVER_IN = odr["NCOVER_IN"] == DBNull.Value ? 0 : decimal.Parse(odr["NCOVER_IN"].ToString());
                        item.NPREMIRAT = odr["NPREMIRAT"] == DBNull.Value ? 0 : decimal.Parse(odr["NPREMIRAT"].ToString());
                        item.SCLDEATHI = odr["SCLDEATHI"] == DBNull.Value ? "" : odr["SCLDEATHI"].ToString();
                        item.SCLACCIDI = odr["SCLACCIDI"] == DBNull.Value ? "" : odr["SCLACCIDI"].ToString();
                        item.SCLVEHACI = odr["SCLVEHACI"] == DBNull.Value ? "" : odr["SCLVEHACI"].ToString();
                        item.SCLSURVII = odr["SCLSURVII"] == DBNull.Value ? "" : odr["SCLSURVII"].ToString();
                        item.SCLINCAPI = odr["SCLINCAPI"] == DBNull.Value ? "" : odr["SCLINCAPI"].ToString();
                        item.SCLINVALI = odr["SCLINVALI"] == DBNull.Value ? "" : odr["SCLINVALI"].ToString();
                        item.SIDURAYEAR = odr["SIDURAYEAR"] == DBNull.Value ? "" : odr["SIDURAYEAR"].ToString();
                        item.SPDURAAGE = odr["SPDURAAGE"] == DBNull.Value ? "" : odr["SPDURAAGE"].ToString();
                        item.NAGEMAXI = odr["NAGEMAXI"] == DBNull.Value ? 0 : decimal.Parse(odr["NAGEMAXI"].ToString());
                        item.SRENEWALI = odr["SRENEWALI"] == DBNull.Value ? "" : odr["SRENEWALI"].ToString();
                        item.SREVINDEX = odr["SREVINDEX"] == DBNull.Value ? "" : odr["SREVINDEX"].ToString();
                        item.SROUCHACA = odr["SROUCHACA"] == DBNull.Value ? "" : odr["SROUCHACA"].ToString();
                        item.SROUCHAPR = odr["SROUCHAPR"] == DBNull.Value ? "" : odr["SROUCHAPR"].ToString();
                        item.NDURATIND = odr["NDURATIND"] == DBNull.Value ? 0 : decimal.Parse(odr["NDURATIND"].ToString());
                        item.NDURATPAY = odr["NDURATPAY"] == DBNull.Value ? 0 : decimal.Parse(odr["NDURATPAY"].ToString());
                        item.SRECHAPRI = odr["SRECHAPRI"] == DBNull.Value ? "" : odr["SRECHAPRI"].ToString();
                        item.SINSURINI = odr["SINSURINI"] == DBNull.Value ? "" : odr["SINSURINI"].ToString();
                        item.SROUPRCAL = odr["SROUPRCAL"] == DBNull.Value ? "" : odr["SROUPRCAL"].ToString();
                        item.SCACALFRI = odr["SCACALFRI"] == DBNull.Value ? "" : odr["SCACALFRI"].ToString();
                        item.SPDURYEAR = odr["SPDURYEAR"] == DBNull.Value ? "" : odr["SPDURYEAR"].ToString();
                        item.SPDUROPEI = odr["SPDUROPEI"] == DBNull.Value ? "" : odr["SPDUROPEI"].ToString();
                        item.SPDURAAGE = odr["SPDURAAGE"] == DBNull.Value ? "" : odr["SPDURAAGE"].ToString();
                        item.SROURESER = odr["SROURESER"] == DBNull.Value ? "" : odr["SROURESER"].ToString();
                        item.SCLILLNESS = odr["SCLILLNESS"] == DBNull.Value ? "" : odr["SCLILLNESS"].ToString();
                        item.SIDUROPEI = odr["SIDUROPEI"] == DBNull.Value ? "" : odr["SIDUROPEI"].ToString();
                        item.SCOVERUSE = odr["SCOVERUSE"] == DBNull.Value ? "" : odr["SCOVERUSE"].ToString();
                        CoverGenList.Add(item);
                    }
                }
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return CoverGenList;
        }

        public ResponseVM UpdateStateCoverGen(CoverGenBM coverGenBM)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseVM response = new ResponseVM();
            string storedProcedureName = ProcedureName.pkg_CargaMasiva + ".UPD_STATE_COVER_GEN";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_SSTATE", OracleDbType.Varchar2, coverGenBM.SSTATE, ParameterDirection.Input));
                //parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Decimal, coverGenBM.NBRANCH_GEN, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCOVERGEN", OracleDbType.Decimal, coverGenBM.NCOVERGEN, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, response.P_NCODE, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, response.P_SMESSAGE, ParameterDirection.Output);

                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;
                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);

                this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                response.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                response.P_SMESSAGE = P_SMESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return response;
        }

        public List<CoverGenVM> GetCoverGenList(CoverGenBM coverGenBM)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<CoverGenVM> CoverGenList = new List<CoverGenVM>();

            //string storedProcedureName = LoadMassivePackage + ".SPS_LIST_HEADERPROC";
            string storedProcedureName = ProcedureName.pkg_CargaMasiva + ".REA_COVER_GEN_LIST";
            try
            {
                //INPUT
                //parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Decimal, coverGenBM.NBRANCH_GEN, ParameterDirection.Input));//MARC
                parameter.Add(new OracleParameter("P_NSTATE", OracleDbType.Decimal, coverGenBM.NESTADO, ParameterDirection.Input));//MARC
                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        CoverGenVM item = new CoverGenVM();
                        //item.NBRANCH_GEN = odr["NBRANCH_GEN"] == DBNull.Value ? 0 : Convert.ToDecimal(odr["NBRANCH_GEN"].ToString());
                        //item.SBRANCH = odr["SBRANCH"] == DBNull.Value ? string.Empty : odr["SBRANCH"].ToString();
                        item.NCOVERGEN = odr["NCOVERGEN"] == DBNull.Value ? 0 : Convert.ToDecimal(odr["NCOVERGEN"].ToString());
                        item.SDESCRIPT = odr["SDESCRIPT"] == DBNull.Value ? string.Empty : odr["SDESCRIPT"].ToString();
                        item.SSTATE = odr["SSTATE"] == DBNull.Value ? string.Empty : odr["SSTATE"].ToString();
                        CoverGenList.Add(item);
                    }
                }
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return CoverGenList;
        }

        public ResponseVM InsertCoverGeneric(CoverGenBM coverGenBM)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseVM response = new ResponseVM();
            string storedProcedureName = ProcedureName.pkg_CargaMasiva + ".INS_COVER_GEN";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_SACCION", OracleDbType.Varchar2, coverGenBM.SACCION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCOVERGEN", OracleDbType.Decimal, coverGenBM.NCOVERGEN, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NAGEMAXI", OracleDbType.Decimal, coverGenBM.NAGEMAXI, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH_EST", OracleDbType.Decimal, coverGenBM.NBRANCH_EST, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH_LED", OracleDbType.Decimal, coverGenBM.NBRANCH_LED, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH_GEN", OracleDbType.Decimal, coverGenBM.NBRANCH_GEN, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH_REI", OracleDbType.Decimal, coverGenBM.NBRANCH_REI, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCACALFIX", OracleDbType.Decimal, coverGenBM.NCACALFIX, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCACALFRI", OracleDbType.Varchar2, coverGenBM.SCACALFRI, ParameterDirection.Input));

                parameter.Add(new OracleParameter("P_SCAPIPREM", OracleDbType.Varchar2, coverGenBM.SCAPIPREM, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCLACCIDI", OracleDbType.Varchar2, coverGenBM.SCLACCIDI, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCLDEATHI", OracleDbType.Varchar2, coverGenBM.SCLDEATHI, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCLINCAPI", OracleDbType.Varchar2, coverGenBM.SCLINCAPI, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCLINVALI", OracleDbType.Varchar2, coverGenBM.SCLINVALI, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCLSURVII", OracleDbType.Varchar2, coverGenBM.SCLSURVII, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCLVEHACI", OracleDbType.Varchar2, coverGenBM.SCLVEHACI, ParameterDirection.Input));

                parameter.Add(new OracleParameter("P_SCOVERUSE", OracleDbType.Varchar2, coverGenBM.SCOVERUSE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCURRENCY", OracleDbType.Decimal, coverGenBM.NCURRENCY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SDESCRIPT", OracleDbType.Varchar2, coverGenBM.SDESCRIPT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SIDURAAGE", OracleDbType.Varchar2, coverGenBM.SIDURAAGE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SIDURAYEAR", OracleDbType.Varchar2, coverGenBM.SIDURAYEAR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SIDUROPEI", OracleDbType.Varchar2, coverGenBM.SIDUROPEI, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SINSURINI", OracleDbType.Varchar2, coverGenBM.SINSURINI, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCOVER_IN", OracleDbType.Decimal, coverGenBM.NCOVER_IN, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPREMIRAT", OracleDbType.Decimal, coverGenBM.NPREMIRAT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SPDURAAGE", OracleDbType.Varchar2, coverGenBM.SPDURAAGE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NDURATIND", OracleDbType.Decimal, coverGenBM.NDURATIND, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NDURATPAY", OracleDbType.Decimal, coverGenBM.NDURATPAY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SREVINDEX", OracleDbType.Varchar2, coverGenBM.SREVINDEX, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SPDUROPEI", OracleDbType.Varchar2, coverGenBM.SPDUROPEI, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SPDURYEAR", OracleDbType.Varchar2, coverGenBM.SPDURYEAR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SPREMCAPI", OracleDbType.Varchar2, coverGenBM.SPREMCAPI, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SRECHAPRI", OracleDbType.Varchar2, coverGenBM.SRECHAPRI, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SRENEWALI", OracleDbType.Varchar2, coverGenBM.SRENEWALI, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SROUCHACA", OracleDbType.Varchar2, coverGenBM.SROUCHACA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SROUCHAPR", OracleDbType.Varchar2, coverGenBM.SROUCHAPR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SROUPRCAL", OracleDbType.Varchar2, coverGenBM.SROUPRCAL, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SROURESER", OracleDbType.Varchar2, coverGenBM.SROURESER, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SROUSURRE", OracleDbType.Varchar2, coverGenBM.SROUSURRE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SSHORT_DES", OracleDbType.Varchar2, coverGenBM.SSHORT_DES, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SSTATREGT", OracleDbType.Varchar2, coverGenBM.SSTATREGT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Decimal, coverGenBM.NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCLILLNESS", OracleDbType.Varchar2, coverGenBM.SCLILLNESS, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCONDSVS", OracleDbType.Varchar2, coverGenBM.SCONDSVS, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SINFORPROV", OracleDbType.Varchar2, coverGenBM.SINFORPROV, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SPROVIDER", OracleDbType.Varchar2, coverGenBM.SPROVIDER, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCLA_LI_TYP", OracleDbType.Decimal, coverGenBM.NCLA_LI_TYP, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, response.P_NCODE, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, response.P_SMESSAGE, ParameterDirection.Output);

                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;
                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);

                this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                response.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                response.P_SMESSAGE = P_SMESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return response;
        }
    }
}
