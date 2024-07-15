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
    public class CoverDA : ConnectionBase
    {
        public CoverVM GetCoverEspCorrelative(CoverBM coverBM)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            CoverVM response = new CoverVM();
            string storedProcedureName = ProcedureName.pkg_CargaMasiva + ".GET_COD_COVER_CORRELATIVE";
            try
            {
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Decimal, coverBM.NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Decimal, coverBM.NPRODUCT, ParameterDirection.Input));//MARC
                parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Decimal, coverBM.NMODULEC, ParameterDirection.Input));//MARC
                //OUTPUT

                OracleParameter P_NCOVER = new OracleParameter("P_NCOVER", OracleDbType.Int32, response.NCOVER, ParameterDirection.Output);

                parameter.Add(P_NCOVER);

                this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                response.NCOVER = Convert.ToDecimal(P_NCOVER.Value.ToString());

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return response;
        }

        public List<CoverRateVM> GetCoverRateByCode(CoverRateBM coverRateBM)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<CoverRateVM> CoverRateList = new List<CoverRateVM>();

            string storedProcedureName = ProcedureName.pkg_CargaMasiva + ".GET_RATE_BY_CODE";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Decimal, coverRateBM.NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Decimal, coverRateBM.NPRODUCT, ParameterDirection.Input));//MARC
                parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Decimal, coverRateBM.NMODULEC, ParameterDirection.Input));//MARC
                parameter.Add(new OracleParameter("P_NCOVER", OracleDbType.Decimal, coverRateBM.NCOVER, ParameterDirection.Input));//MARC
                parameter.Add(new OracleParameter("P_NCODREC", OracleDbType.Decimal, coverRateBM.NCODREC, ParameterDirection.Input));//MARC
                parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Decimal, coverRateBM.NPOLICY, ParameterDirection.Input));//MARC
                parameter.Add(new OracleParameter("P_SBYPOLICY", OracleDbType.Varchar2, coverRateBM.SBYPOLICY, ParameterDirection.Input));//MARC
                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        CoverRateVM item = new CoverRateVM();

                        item.NBRANCH = odr["NBRANCH"] == DBNull.Value ? 0 : double.Parse(odr["NBRANCH"].ToString());
                        item.NPRODUCT = odr["NPRODUCT"] == DBNull.Value ? 0 : double.Parse(odr["NPRODUCT"].ToString());
                        item.NMODULEC = odr["NMODULEC"] == DBNull.Value ? 0 : double.Parse(odr["NMODULEC"].ToString());
                        item.NCOVER = odr["NCOVER"] == DBNull.Value ? 0 : double.Parse(odr["NCOVER"].ToString());
                        item.NCODREC = odr["NCODREC"] == DBNull.Value ? 0 : double.Parse(odr["NCODREC"].ToString());
                        item.NMONTHI = odr["NMONTHI"] == DBNull.Value ? 0 : double.Parse(odr["NMONTHI"].ToString());
                        item.NMONTHE = odr["NMONTHE"] == DBNull.Value ? 0 : double.Parse(odr["NMONTHE"].ToString());
                        item.NPERCENT = odr["NPERCENT"] == DBNull.Value ? 0 : double.Parse(odr["NPERCENT"].ToString());
                        item.NAMOUNT = odr["NAMOUNT"] == DBNull.Value ? 0 : double.Parse(odr["NAMOUNT"].ToString());
                        item.NCURRENCY = odr["NCURRENCY"] == DBNull.Value ? 0 : double.Parse(odr["NCURRENCY"].ToString());
                        item.NPOLICY = odr["NPOLICY"] == DBNull.Value ? 0 : double.Parse(odr["NPOLICY"].ToString());

                        CoverRateList.Add(item);
                    }
                }
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return CoverRateList;
        }

        public List<CoverRateVM> GetCoverRateList(CoverRateBM coverRateBM)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<CoverRateVM> CoverRateList = new List<CoverRateVM>();
            //string storedProcedureName = LoadMassivePackage + ".SPS_LIST_HEADERPROC";
            string storedProcedureName = ProcedureName.pkg_CargaMasiva + ".REA_RATE_COVER_ESP_LIST";

            try
            {
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Decimal, coverRateBM.NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Decimal, coverRateBM.NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Decimal, coverRateBM.NMODULEC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCOVER", OracleDbType.Decimal, coverRateBM.NCOVER, ParameterDirection.Input));
                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        CoverRateVM item = new CoverRateVM();
                        item.NBRANCH = odr["NBRANCH"] == DBNull.Value ? 0.0 : Double.Parse(odr["NBRANCH"].ToString());
                        item.NPRODUCT = odr["NPRODUCT"] == DBNull.Value ? 0.0 : Double.Parse(odr["NPRODUCT"].ToString());
                        item.NMODULEC = odr["NMODULEC"] == DBNull.Value ? 0.0 : Double.Parse(odr["NMODULEC"].ToString()); ;
                        item.SRECHARGETYPE = odr["SRECHARGETYPE"] == DBNull.Value ? string.Empty : odr["SRECHARGETYPE"].ToString();
                        item.NMONTHI = odr["NMONTHI"] == DBNull.Value ? 0.0 : Double.Parse(odr["NMONTHI"].ToString());
                        item.NMONTHE = odr["NMONTHE"] == DBNull.Value ? 0.0 : Double.Parse(odr["NMONTHE"].ToString());
                        item.NPERCENT = odr["NPERCENT"] == DBNull.Value ? 0.0 : Double.Parse(odr["NPERCENT"].ToString());
                        item.NAMOUNT = odr["NAMOUNT"] == DBNull.Value ? 0.0 : Double.Parse(odr["NAMOUNT"].ToString());
                        item.NCURRENCY = odr["NCURRENCY"] == DBNull.Value ? 0.0 : Double.Parse(odr["NCURRENCY"].ToString());
                        item.SCURRENCY = odr["SCURRENCY"] == DBNull.Value ? string.Empty : odr["SCURRENCY"].ToString();
                        item.NPOLICY = odr["NPOLICY"] == DBNull.Value ? 0.0 : Double.Parse(odr["NPOLICY"].ToString());
                        item.NCODREC = odr["NCODREC"] == DBNull.Value ? 0.0 : Double.Parse(odr["NCODREC"].ToString());
                        item.NROLE = odr["NROLE"] == DBNull.Value ? 0.0 : Double.Parse(odr["NROLE"].ToString());
                        item.NCOVER = odr["NCOVER"] == DBNull.Value ? 0.0 : Double.Parse(odr["NCOVER"].ToString());
                        item.SORIGEN = odr["SORIGEN"] == DBNull.Value ? string.Empty : odr["SORIGEN"].ToString();
                        item.SBYPOLICY = odr["SBYPOLICY"] == DBNull.Value ? string.Empty : odr["SBYPOLICY"].ToString();
                        item.DEFFECDATE = odr["DEFFECDATE"] == DBNull.Value ? "" : String.Format("{0:dd/MM/yyyy}", odr["DEFFECDATE"]);
                        CoverRateList.Add(item);
                    }
                }
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return CoverRateList;
        }

        public ResponseVM InsertRateDetail(CoverRateBM coverRateBM)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseVM response = new ResponseVM();
            string storedProcedureName = ProcedureName.pkg_CargaMasiva + ".INSUPD_TASA";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_SACCION", OracleDbType.Varchar2, coverRateBM.SACCION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Decimal, coverRateBM.NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Decimal, coverRateBM.NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Decimal, coverRateBM.NMODULEC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCODREC", OracleDbType.Decimal, coverRateBM.NCODREC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCOVER", OracleDbType.Decimal, coverRateBM.NCOVER, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NROLE", OracleDbType.Decimal, coverRateBM.NROLE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMONTHI", OracleDbType.Decimal, coverRateBM.NMONTHI, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Varchar2, coverRateBM.DEFFECDATE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMONTHE", OracleDbType.Decimal, coverRateBM.NMONTHE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPERCENT", OracleDbType.Decimal, coverRateBM.NPERCENT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCURRENCY", OracleDbType.Decimal, coverRateBM.NCURRENCY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NAMOUNT", OracleDbType.Decimal, coverRateBM.NAMOUNT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Decimal, coverRateBM.NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DNULLDATE", OracleDbType.Date, coverRateBM.DNULLDATE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Decimal, coverRateBM.NPOLICY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SBYPOLICY", OracleDbType.Char, coverRateBM.SBYPOLICY, ParameterDirection.Input));
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

        public List<CoverEspVM> GetCoverEspByCoverGen(CoverEspBM coverEspBM)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<CoverEspVM> CoverEspList = new List<CoverEspVM>();

            string storedProcedureName = ProcedureName.pkg_CargaMasiva + ".GET_COVER_ESP_BY_COVERGEN";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Decimal, coverEspBM.NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Decimal, coverEspBM.NPRODUCT, ParameterDirection.Input));//MARC
                parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Decimal, coverEspBM.NMODULEC, ParameterDirection.Input));//MARC
                parameter.Add(new OracleParameter("P_NCOVERGEN", OracleDbType.Decimal, coverEspBM.NCOVERGEN, ParameterDirection.Input));//MARC

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        CoverEspVM item = new CoverEspVM();

                        item.NBRANCH = odr["NBRANCH"] == DBNull.Value ? 0 : decimal.Parse(odr["NBRANCH"].ToString());
                        item.NPRODUCT = odr["NPRODUCT"] == DBNull.Value ? 0 : decimal.Parse(odr["NPRODUCT"].ToString());
                        item.NMODULEC = odr["NMODULEC"] == DBNull.Value ? 0 : decimal.Parse(odr["NMODULEC"].ToString());
                        item.NCOVERGEN = odr["NCOVERGEN"] == DBNull.Value ? 0 : decimal.Parse(odr["NCOVERGEN"].ToString());

                        item.SADDSUINI = odr["SADDSUINI"] == DBNull.Value ? string.Empty : odr["SADDSUINI"].ToString();
                        item.SADDREINI = odr["SADDREINI"] == DBNull.Value ? string.Empty : odr["SADDREINI"].ToString();
                        item.SADDTAXIN = odr["SADDTAXIN"] == DBNull.Value ? string.Empty : odr["SADDTAXIN"].ToString();
                        item.SMORTACOF = odr["SMORTACOF"] == DBNull.Value ? string.Empty : odr["SMORTACOF"].ToString();
                        item.SMORTACOM = odr["SMORTACOM"] == DBNull.Value ? string.Empty : odr["SMORTACOM"].ToString();
                        item.NINTEREST = odr["NINTEREST"] == DBNull.Value ? 0 : decimal.Parse(odr["NINTEREST"].ToString());

                        item.SCOVERUSE = odr["SCOVERUSE"] == DBNull.Value ? string.Empty : odr["SCOVERUSE"].ToString();
                        item.NCURRENCY = odr["NCURRENCY"] == DBNull.Value ? 0 : decimal.Parse(odr["NCURRENCY"].ToString());
                        item.SROURESER = odr["SROURESER"] == DBNull.Value ? string.Empty : odr["SROURESER"].ToString();
                        item.SCONTROL = odr["SCONTROL"] == DBNull.Value ? string.Empty : odr["SCONTROL"].ToString();
                        item.DNULLDATE = odr["DNULLDATE"] == DBNull.Value ? "" : odr["DNULLDATE"].ToString();

                        item.SEDITABLE = odr["SEDITABLE"] == DBNull.Value ? string.Empty : odr["SEDITABLE"].ToString();
                        item.NCOVER = odr["NCOVER"] == DBNull.Value ? 0 : decimal.Parse(odr["NCOVER"].ToString());
                        //item.SDESCRIPT_CAPITAL = odr["SDESCRIPT_CAPITAL"] == DBNull.Value ? string.Empty : odr["SDESCRIPT_CAPITAL"].ToString();
                        CoverEspList.Add(item);
                    }
                }
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return CoverEspList;
        }

        public List<CoverEspVM> GetCoverEspByCover(CoverEspBM coverEspBM)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<CoverEspVM> CoverEspList = new List<CoverEspVM>();

            string storedProcedureName = ProcedureName.pkg_CargaMasiva + ".GET_COVER_ESP_BY_COVER";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Decimal, coverEspBM.NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Decimal, coverEspBM.NPRODUCT, ParameterDirection.Input));//MARC
                parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Decimal, coverEspBM.NMODULEC, ParameterDirection.Input));//MARC
                parameter.Add(new OracleParameter("P_NCOVERGEN", OracleDbType.Decimal, coverEspBM.NCOVERGEN, ParameterDirection.Input));//MARC
                parameter.Add(new OracleParameter("P_NCOVER", OracleDbType.Decimal, coverEspBM.NCOVER, ParameterDirection.Input));//MARC
                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        CoverEspVM item = new CoverEspVM();

                        item.NBRANCH = odr["NBRANCH"] == DBNull.Value ? 0 : decimal.Parse(odr["NBRANCH"].ToString());
                        item.NPRODUCT = odr["NPRODUCT"] == DBNull.Value ? 0 : decimal.Parse(odr["NPRODUCT"].ToString());
                        item.NMODULEC = odr["NMODULEC"] == DBNull.Value ? 0 : decimal.Parse(odr["NMODULEC"].ToString());
                        item.NCOVERGEN = odr["NCOVERGEN"] == DBNull.Value ? 0 : decimal.Parse(odr["NCOVERGEN"].ToString());

                        item.SADDSUINI = odr["SADDSUINI"] == DBNull.Value ? string.Empty : odr["SADDSUINI"].ToString();
                        item.SADDREINI = odr["SADDREINI"] == DBNull.Value ? string.Empty : odr["SADDREINI"].ToString();
                        item.SADDTAXIN = odr["SADDTAXIN"] == DBNull.Value ? string.Empty : odr["SADDTAXIN"].ToString();
                        item.SMORTACOF = odr["SMORTACOF"] == DBNull.Value ? string.Empty : odr["SMORTACOF"].ToString();
                        item.SMORTACOM = odr["SMORTACOM"] == DBNull.Value ? string.Empty : odr["SMORTACOM"].ToString();
                        item.NINTEREST = odr["NINTEREST"] == DBNull.Value ? 0 : decimal.Parse(odr["NINTEREST"].ToString());

                        item.SCOVERUSE = odr["SCOVERUSE"] == DBNull.Value ? string.Empty : odr["SCOVERUSE"].ToString();
                        item.NCURRENCY = odr["NCURRENCY"] == DBNull.Value ? 0 : decimal.Parse(odr["NCURRENCY"].ToString());
                        item.SROURESER = odr["SROURESER"] == DBNull.Value ? string.Empty : odr["SROURESER"].ToString();
                        item.SCONTROL = odr["SCONTROL"] == DBNull.Value ? string.Empty : odr["SCONTROL"].ToString();
                        item.DNULLDATE = odr["DNULLDATE"] == DBNull.Value ? "" : odr["DNULLDATE"].ToString();

                        item.SEDITABLE = odr["SEDITABLE"] == DBNull.Value ? string.Empty : odr["SEDITABLE"].ToString();
                        item.NCOVER = odr["NCOVER"] == DBNull.Value ? 0 : decimal.Parse(odr["NCOVER"].ToString());
                        item.NCAPMINIM = odr["NCAPMINIM"] == DBNull.Value ? 0 : decimal.Parse(odr["NCAPMINIM"].ToString());
                        item.NCAPMAXIM = odr["NCAPMAXIM"] == DBNull.Value ? 0 : decimal.Parse(odr["NCAPMAXIM"].ToString());
                        item.NCACALFIX = odr["NCACALFIX"] == DBNull.Value ? 0 : decimal.Parse(odr["NCACALFIX"].ToString());
                        item.NCACALMUL = odr["NCACALMUL"] == DBNull.Value ? 0 : decimal.Parse(odr["NCACALMUL"].ToString());
                        item.NCAPBASPE = odr["NCAPBASPE"] == DBNull.Value ? 0 : decimal.Parse(odr["NCAPBASPE"].ToString());
                        item.SDESCRIPT_CAPITAL = odr["SDESCRIPT_CAPITAL"] == DBNull.Value ? string.Empty : odr["SDESCRIPT_CAPITAL"].ToString();
                        CoverEspList.Add(item);
                    }
                }
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return CoverEspList;
        }

        public CoverVM GetCoverEspCode(CoverBM coverBM)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            CoverVM response = new CoverVM();
            string storedProcedureName = ProcedureName.pkg_CargaMasiva + ".GET_COD_COVER_ESP";
            try
            {
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Decimal, coverBM.NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Decimal, coverBM.NPRODUCT, ParameterDirection.Input));//MARC
                parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Decimal, coverBM.NMODULEC, ParameterDirection.Input));//MARC
                parameter.Add(new OracleParameter("P_NCOVERGEN", OracleDbType.Decimal, coverBM.NCOVERGEN, ParameterDirection.Input));//MARC
                //OUTPUT

                OracleParameter P_NCOVER = new OracleParameter("P_NCOVER", OracleDbType.Int32, response.NCOVER, ParameterDirection.Output);

                parameter.Add(P_NCOVER);

                this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                response.NCOVER = Convert.ToDecimal(P_NCOVER.Value.ToString());

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return response;
        }

        public ResponseVM InsertModuleDetail(CoverBM coverBM)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseVM response = new ResponseVM();
            string storedProcedureName = ProcedureName.pkg_CargaMasiva + ".INS_MODULE_DET";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_SACCION", OracleDbType.Varchar2, coverBM.SACCION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Decimal, coverBM.NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Decimal, coverBM.NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Decimal, coverBM.NMODULEC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SADDSUINI", OracleDbType.Varchar2, coverBM.SADDSUINI, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SADDREINI", OracleDbType.Varchar2, coverBM.SADDREINI, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SADDTAXIN", OracleDbType.Varchar2, coverBM.SADDTAXIN, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCOVERGEN", OracleDbType.Decimal, coverBM.NCOVERGEN, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SMORTACOF", OracleDbType.Varchar2, coverBM.SMORTACOF, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SMORTACOM", OracleDbType.Varchar2, coverBM.SMORTACOM, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NINTEREST", OracleDbType.Decimal, coverBM.NINTEREST, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOVERUSE", OracleDbType.Varchar2, coverBM.SCOVERUSE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCURRENCY", OracleDbType.Decimal, coverBM.NCURRENCY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SROURESER", OracleDbType.Varchar2, coverBM.SROURESER, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCONTROL", OracleDbType.Varchar2, coverBM.SCONTROL, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Decimal, coverBM.NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCOVER", OracleDbType.Decimal, coverBM.NCOVER, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DNULLDATE", OracleDbType.Date, coverBM.DNULLDATE == null || coverBM.DNULLDATE == "" ? null : DateTime.Parse(coverBM.DNULLDATE).ToShortDateString(), ParameterDirection.Input));

                parameter.Add(new OracleParameter("P_NCAPMINIM", OracleDbType.Decimal, coverBM.NCAPMINIM, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCAPMAXIM", OracleDbType.Decimal, coverBM.NCAPMAXIM, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCACALFIX", OracleDbType.Decimal, coverBM.NCACALFIX, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCAPBASPE", OracleDbType.Decimal, coverBM.NCACALMUL, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCAPMAXIM", OracleDbType.Decimal, coverBM.NCAPBASPE, ParameterDirection.Input));

                parameter.Add(new OracleParameter("P_SCHANGETYP", OracleDbType.Varchar2, coverBM.SCHANGETYP, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPREMIRAT", OracleDbType.Decimal, coverBM.NPREMIRAT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Date, DateTime.Parse(coverBM.DEFFECDATE).ToShortDateString(), ParameterDirection.Input));
                parameter.Add(new OracleParameter("SDESCRIPT_CAPITAL", OracleDbType.Varchar2, coverBM.SDESCRIPT_CAPITAL, ParameterDirection.Input));

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

        public List<CoverVM> GetCoverList(CoverBM coverBM)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<CoverVM> CoverList = new List<CoverVM>();
            //string storedProcedureName = LoadMassivePackage + ".SPS_LIST_HEADERPROC";
            string storedProcedureName = ProcedureName.pkg_CargaMasiva + ".REA_COVER_LIST";

            try
            {
                //parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Decimal, coverBM.NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Decimal, coverBM.NPRODUCT, ParameterDirection.Input));
                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        CoverVM item = new CoverVM();
                        item.NCOVERGEN = odr["NCOVERGEN"] == DBNull.Value ? 0 : Convert.ToDecimal(odr["NCOVERGEN"].ToString());
                        item.SDESCRIPT = odr["SDESCRIPT"] == DBNull.Value ? string.Empty : odr["SDESCRIPT"].ToString();
                        item.SSTATE = odr["SSTATE"] == DBNull.Value ? string.Empty : odr["SSTATE"].ToString();
                        CoverList.Add(item);
                    }
                }
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return CoverList;
        }
    }
}
