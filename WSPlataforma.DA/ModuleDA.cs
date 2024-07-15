using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using WSPlataforma.Util;
using WSPlataforma.Entities.ViewModel;
using WSPlataforma.Entities.ModuleModel.BindingModel;
using WSPlataforma.Entities.ModuleModel.ViewModel;

namespace WSPlataforma.DA
{
    public class ModuleDA : ConnectionBase
    {
        public ResponseVM UpdateStateModule(ModuleBM moduleBM)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseVM response = new ResponseVM();
            string storedProcedureName = ProcedureName.pkg_CargaMasiva + ".UPD_STATE_MODULE";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_SSTATE", OracleDbType.Varchar2, moduleBM.SSTATE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Decimal, moduleBM.NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Decimal, moduleBM.NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Decimal, moduleBM.NMODULEC, ParameterDirection.Input));

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

        public List<ModuleVM> GetModuleList(ModuleBM moduleBM)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<ModuleVM> ModuleList = new List<ModuleVM>();
            //string storedProcedureName = LoadMassivePackage + ".SPS_LIST_HEADERPROC";
            string storedProcedureName = ProcedureName.pkg_CargaMasiva + ".REA_MODULE_LIST";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_DATEPROCESS", OracleDbType.Date, moduleBM.DEFFECDATE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Decimal, moduleBM.NBRANCH, ParameterDirection.Input));//MARC
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Decimal, moduleBM.NPRODUCT, ParameterDirection.Input));//MARC
                parameter.Add(new OracleParameter("P_NSTATE", OracleDbType.Decimal, moduleBM.NESTADO, ParameterDirection.Input));//MARC
                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        ModuleVM item = new ModuleVM();
                        item.NBRANCH = odr["NBRANCH"] == DBNull.Value ? 0 : Convert.ToDecimal(odr["NBRANCH"].ToString());
                        item.SBRANCH = odr["SBRANCH"] == DBNull.Value ? string.Empty : odr["SBRANCH"].ToString();
                        item.NPRODUCT = odr["NPRODUCT"] == DBNull.Value ? 0 : Convert.ToDecimal(odr["NPRODUCT"].ToString());
                        item.SPRODUCT = odr["SPRODUCT"] == DBNull.Value ? string.Empty : odr["SPRODUCT"].ToString();
                        item.NMODULEC = odr["NMODULEC"] == DBNull.Value ? 0 : Convert.ToDecimal(odr["NMODULEC"].ToString());
                        item.SDESCRIPT = odr["SDESCRIPT"] == DBNull.Value ? string.Empty : odr["SDESCRIPT"].ToString();
                        item.SSTATE = odr["SSTATE"] == DBNull.Value ? string.Empty : odr["SSTATE"].ToString();
                        item.DEFFECDATE = odr["DEFFECDATE"] == DBNull.Value ? string.Empty : odr["DEFFECDATE"].ToString();
                        ModuleList.Add(item);
                    }
                }
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ModuleList;
        }

        public List<ModuleVM> GetModuleByCode(ModuleBM moduleBM)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<ModuleVM> ModuleList = new List<ModuleVM>();

            string storedProcedureName = ProcedureName.pkg_CargaMasiva + ".GET_MODULE_BY_CODE";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Decimal, moduleBM.NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Decimal, moduleBM.NPRODUCT, ParameterDirection.Input));//MARC
                parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Decimal, moduleBM.NMODULEC, ParameterDirection.Input));//MARC
                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        ModuleVM item = new ModuleVM();
                        item.NMODULEC = odr["NMODULEC"] == DBNull.Value ? 0 : decimal.Parse(odr["NMODULEC"].ToString());
                        item.SDESCRIPT = odr["SDESCRIPT"] == DBNull.Value ? string.Empty : odr["SDESCRIPT"].ToString();
                        item.DEFFECDATE = odr["DEFFECDATE"] == DBNull.Value ? string.Empty : odr["DEFFECDATE"].ToString();
                        item.SSHORT_DES = odr["SSHORT_DES"] == DBNull.Value ? string.Empty : odr["SSHORT_DES"].ToString();
                        item.SCONDSVS = odr["SCONDSVS"] == DBNull.Value ? string.Empty : odr["SCONDSVS"].ToString();
                        item.NCHPRELEV = odr["NCHPRELEV"] == DBNull.Value ? 0 : decimal.Parse(odr["NCHPRELEV"].ToString());
                        item.SREQUIRE = odr["SREQUIRE"] == DBNull.Value ? string.Empty : odr["SREQUIRE"].ToString();
                        item.SDEFAULTI = odr["SDEFAULTI"] == DBNull.Value ? string.Empty : odr["SDEFAULTI"].ToString();
                        item.SVIGEN = odr["SVIGEN"] == DBNull.Value ? string.Empty : odr["SVIGEN"].ToString();
                        item.SCHANALLO = odr["SCHANALLO"] == DBNull.Value ? string.Empty : odr["SCHANALLO"].ToString();
                        item.SCHANGETYP = odr["SCHANGETYP"] == DBNull.Value ? string.Empty : odr["SCHANGETYP"].ToString();
                        item.NRATEPREADD = odr["NRATEPREADD"] == DBNull.Value ? 0 : decimal.Parse(odr["NRATEPREADD"].ToString());
                        item.NRATEPRESUB = odr["NRATEPRESUB"] == DBNull.Value ? 0 : decimal.Parse(odr["NRATEPRESUB"].ToString());
                        item.STYP_RAT = odr["STYP_RAT"] == DBNull.Value ? string.Empty : odr["STYP_RAT"].ToString();
                        item.NPREMIRAT = odr["NPREMIRAT"] == DBNull.Value ? 0 : decimal.Parse(odr["NPREMIRAT"].ToString());
                        ModuleList.Add(item);
                    }
                }
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ModuleList;
        }

        public ResponseVM UpdateModule(ModuleBM moduleBM)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseVM response = new ResponseVM();
            string storedProcedureName = ProcedureName.pkg_CargaMasiva + ".INS_MODULE_CAB";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_SACCION", OracleDbType.Varchar2, moduleBM.SACCION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Decimal, moduleBM.NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Decimal, moduleBM.NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Decimal, moduleBM.NMODULEC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Date, DateTime.Parse(moduleBM.DEFFECDATE).ToShortDateString(), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SDESCRIPT", OracleDbType.Char, moduleBM.SDESCRIPT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SSHORT_DES", OracleDbType.Char, moduleBM.SSHORT_DES, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCONDSVS", OracleDbType.Char, moduleBM.SCONDSVS, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SREQUIRE", OracleDbType.Char, moduleBM.SREQUIRE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SDEFAULTI", OracleDbType.Char, moduleBM.SDEFAULTI, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCHANALLO", OracleDbType.Char, moduleBM.SCHANALLO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STYP_RAT", OracleDbType.Char, moduleBM.STYP_RAT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPREMIRAT", OracleDbType.Decimal, moduleBM.NPREMIRAT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NRATEPREADD", OracleDbType.Decimal, moduleBM.NRATEPREADD, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NRATEPRESUB", OracleDbType.Decimal, moduleBM.NRATEPRESUB, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SVIGEN", OracleDbType.Char, moduleBM.SVIGEN, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCHPRELEV", OracleDbType.Decimal, moduleBM.NCHPRELEV, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCHANGETYP", OracleDbType.Char, moduleBM.SCHANGETYP, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Decimal, moduleBM.NUSERCODE, ParameterDirection.Input));
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

        public ModuleVM GetModuleCode(ModuleBM moduleBM)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            ModuleVM response = new ModuleVM();
            string storedProcedureName = ProcedureName.pkg_CargaMasiva + ".GET_COD_MODULE";

            try
            {
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Decimal, moduleBM.NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Decimal, moduleBM.NPRODUCT, ParameterDirection.Input));//MARC

                //OUTPUT
                OracleParameter P_NCOD_MODULE = new OracleParameter("P_NCOD_MODULE", OracleDbType.Int32, response.NMODULEC, ParameterDirection.Output);

                parameter.Add(P_NCOD_MODULE);

                this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                response.NMODULEC = Convert.ToInt32(P_NCOD_MODULE.Value.ToString());

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return response;
        }
    }
}
