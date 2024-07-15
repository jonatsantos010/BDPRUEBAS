using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.Entities.CoverModel.BindingModel;
using WSPlataforma.Entities.CoverModel.ViewModel;

using WSPlataforma.Util;

namespace WSPlataforma.DA
{
    public class LifeCoverDA : ConnectionBase
    {
        public List<CoverEspVM> GetCoverList(CoverEspBM coverBM)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<CoverEspVM> LifeCoverList = new List<CoverEspVM>();

            string storedProcedureName = ProcedureName.pkg_CargaMasiva + ".REA_LIFE_COVER_LIST";

            try
            {
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Decimal, coverBM.NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Decimal, coverBM.NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMODULEC", OracleDbType.Decimal, coverBM.NMODULEC, ParameterDirection.Input));
                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        CoverEspVM item = new CoverEspVM();
                        item.NCOVERGEN = odr["NCOVERGEN"] == DBNull.Value ? 0 : Convert.ToDecimal(odr["NCOVERGEN"].ToString());
                        item.NCOVER = odr["NCOVER"] == DBNull.Value ? 0 : Convert.ToDecimal(odr["NCOVER"].ToString());
                        item.NBRANCH = odr["NBRANCH"] == DBNull.Value ? 0 : Convert.ToDecimal(odr["NBRANCH"].ToString());
                        item.NPRODUCT = odr["NPRODUCT"] == DBNull.Value ? 0 : Convert.ToDecimal(odr["NPRODUCT"].ToString());
                        item.NMODULEC = odr["NMODULEC"] == DBNull.Value ? 0 : Convert.ToDecimal(odr["NMODULEC"].ToString());
                        item.DNULLDATE = odr["DNULLDATE"] == DBNull.Value ? "" : odr["DNULLDATE"].ToString();
                        item.SEDITABLE = odr["SEDITABLE"] == DBNull.Value ? string.Empty : odr["SEDITABLE"].ToString();

                        item.SADDSUINI = odr["SADDSUINI"] == DBNull.Value ? string.Empty : odr["SADDSUINI"].ToString();
                        item.SADDREINI = odr["SADDREINI"] == DBNull.Value ? string.Empty : odr["SADDREINI"].ToString();
                        item.SADDTAXIN = odr["SADDTAXIN"] == DBNull.Value ? string.Empty : odr["SADDTAXIN"].ToString();
                        item.SMORTACOF = odr["SMORTACOF"] == DBNull.Value ? string.Empty : odr["SMORTACOF"].ToString();
                        item.SMORTACOF = odr["SMORTACOM"] == DBNull.Value ? string.Empty : odr["SMORTACOM"].ToString();
                        item.NINTEREST = odr["NINTEREST"] == DBNull.Value ? 0 : Convert.ToDecimal(odr["NINTEREST"].ToString());
                        item.SCOVERUSE = odr["SCOVERUSE"] == DBNull.Value ? string.Empty : odr["SCOVERUSE"].ToString();
                        item.NCURRENCY = odr["NCURRENCY"] == DBNull.Value ? 0 : Convert.ToDecimal(odr["NCURRENCY"].ToString());
                        item.SROURESER = odr["SROURESER"] == DBNull.Value ? string.Empty : odr["SROURESER"].ToString();
                        item.SCONTROL = odr["SCONTROL"] == DBNull.Value ? string.Empty : odr["SCONTROL"].ToString();
                        item.DEFFECDATE = odr["DEFFECDATE"] == DBNull.Value ? string.Empty : String.Format("{0:dd/MM/yyyy}", odr["DEFFECDATE"]);
                        item.SORIGEN = odr["SORIGEN"] == DBNull.Value ? string.Empty : odr["SORIGEN"].ToString();

                        item.NCAPMINIM = odr["NCAPMINIM"] == DBNull.Value ? 0 : Convert.ToDecimal(odr["NCAPMINIM"].ToString());
                        item.NCAPMAXIM = odr["NCAPMAXIM"] == DBNull.Value ? 0 : Convert.ToDecimal(odr["NCAPMAXIM"].ToString());
                        item.NCACALFIX = odr["NCACALFIX"] == DBNull.Value ? 0 : Convert.ToDecimal(odr["NCACALFIX"].ToString());
                        item.NCAPBASPE = odr["NCAPBASPE"] == DBNull.Value ? 0 : Convert.ToDecimal(odr["NCAPBASPE"].ToString());
                        item.NCACALMUL = odr["NCACALMUL"] == DBNull.Value ? 0 : Convert.ToDecimal(odr["NCACALMUL"].ToString());
                        item.SDESCRIPT_CAPITAL = odr["SDESCRIPT_CAPITAL"] == DBNull.Value ? string.Empty : odr["SDESCRIPT_CAPITAL"].ToString();
                        LifeCoverList.Add(item);
                    }
                }
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return LifeCoverList;
        }
    }
}
