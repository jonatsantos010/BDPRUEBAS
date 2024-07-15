using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.Entities.CommissionModel.BindingModel;
using WSPlataforma.Util;

namespace WSPlataforma.DA
{
    public class CommissionDA : ConnectionBase
    {
        public List<BrokerCommissionBM> GetCommisionBroker(CommissionTransactionAllSearchBM search)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<BrokerCommissionBM> ListBroker = new List<BrokerCommissionBM>();
            string storedProcedureName = ProcedureName.pkg_Comision + ".REA_LIST_BROKER";
            try
            {
                //INPUT

                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, search.NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, search.NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int64, search.NPOLICY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, search.SIDDOC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDDOC_TYPE", OracleDbType.Int32, search.NTYPE_DOC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCLIENAME", OracleDbType.Varchar2, search.SLEGALNAME, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SINTER_ID", OracleDbType.Varchar2, search.SCODSBS, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Varchar2, search.DEFECT, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        BrokerCommissionBM item = new BrokerCommissionBM();
                        item.SCERTYPE = odr["SCERTYPE"] == DBNull.Value ? string.Empty : odr["SCERTYPE"].ToString();
                        item.NBRANCH = odr["NBRANCH"] == DBNull.Value ? string.Empty : odr["NBRANCH"].ToString();
                        item.NPRODUCT = odr["NPRODUCT"] == DBNull.Value ? string.Empty : odr["NPRODUCT"].ToString();
                        item.COD_SBS = odr["COD_SBS"] == DBNull.Value ? string.Empty : odr["COD_SBS"].ToString();
                        item.TIPO_INTERMEDIARIO = odr["TIPO_INTERMEDIARIO"] == DBNull.Value ? string.Empty : odr["TIPO_INTERMEDIARIO"].ToString();
                        item.RAZON_INTERMEDIARIO = odr["RAZON_INTERMEDIARIO"] == DBNull.Value ? string.Empty : odr["RAZON_INTERMEDIARIO"].ToString();
                        item.POLIZA = odr["POLIZA"] == DBNull.Value ? string.Empty : odr["POLIZA"].ToString();
                        item.FECHA_EFECTO = odr["FECHA_EFECTO"] == DBNull.Value ? string.Empty : odr["FECHA_EFECTO"].ToString();
                        item.POR_COMISION = odr["POR_COMISION"] == DBNull.Value ? string.Empty : odr["POR_COMISION"].ToString();
                        item.POR_PARTICIPACION = odr["POR_PARTICIPACION"] == DBNull.Value ? string.Empty : odr["POR_PARTICIPACION"].ToString();
                        item.NINTERMED = odr["INTERMEDIARIO"] == DBNull.Value ? string.Empty : odr["INTERMEDIARIO"].ToString();
                        item.DES_RAMO = odr["DES_RAMO"] == DBNull.Value ? string.Empty : odr["DES_RAMO"].ToString();
                        item.DES_PRODUCT = odr["DES_PRODUCT"] == DBNull.Value ? string.Empty : odr["DES_PRODUCT"].ToString();
                        item.FECHA_EFECTO = (item.FECHA_EFECTO.Trim() != String.Empty ? item.FECHA_EFECTO.Substring(0, 10) : item.FECHA_EFECTO);
                        ListBroker.Add(item);
                    }
                }
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("GetCommisionBroker", ex.ToString(), "3");
            }

            return ListBroker;
        }




        public List<IntermedaryBM> getListIntermedary(CommissionTransactionAllSearchBM search)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<IntermedaryBM> ListIntermedary = new List<IntermedaryBM>();
            string storedProcedureName = ProcedureName.pkg_Comision + ".REA_BR_PRODUCT";
            try
            {
                //INPUT

                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, search.NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, search.NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int64, search.NPOLICY, ParameterDirection.Input));


                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        IntermedaryBM item = new IntermedaryBM();
                        item.intermedary = odr["INTERMEDIARIO"] == DBNull.Value ? string.Empty : odr["INTERMEDIARIO"].ToString();
                        item.nintermed = odr["NINTERMED"] == DBNull.Value ? 0 : Int32.Parse(odr["NINTERMED"].ToString());
                        item.tipoIntermediario = odr["TIPO_INTERMEDIARIO"] == DBNull.Value ? string.Empty : odr["TIPO_INTERMEDIARIO"].ToString();
                        ListIntermedary.Add(item);
                    }
                }
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("getListIntermedary", ex.ToString(), "3");
            }

            return ListIntermedary;
        }


        public List<CommissionBR> GetLastCommission(CommissionTransactionAllSearchBM search)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<CommissionBR> last_commission = new List<CommissionBR>();
            string storedProcedureName = ProcedureName.pkg_Comision_VDP + ".SP_GET_LAST_COMMISSION_VDP";
            try
            {

                //OUTPUT
                OracleParameter C_CURSOR = new OracleParameter("C_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_CURSOR);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        CommissionBR item = new CommissionBR();

                        item.NHIS_CONFIGURATION = odr["NHIS_CONFIGURATION"] == DBNull.Value ? 0 : Int32.Parse(odr["NHIS_CONFIGURATION"].ToString());
                        item.NINI_MONTH = odr["NINI_MONTH"] == DBNull.Value ? 0 : Int32.Parse(odr["NINI_MONTH"].ToString());
                        item.NCANTMONTHS = odr["NCANTMONTHS"] == DBNull.Value ? 0 : Int32.Parse(odr["NCANTMONTHS"].ToString());

                        // Enviando este formato de fecha para poder controlarlo en el Front
                        item.DINIDATE = odr["DINIDATE"] == DBNull.Value ? string.Empty : odr["DINIDATE"].ToString();
                        item.DINIDATE = item.DINIDATE == string.Empty ? "" : Convert.ToDateTime(item.DINIDATE).ToString("MM/dd/yyyy");

                        item.DESC_USER_REGISTER = odr["DESC_USER_REGISTER"] == DBNull.Value ? string.Empty : odr["DESC_USER_REGISTER"].ToString();

                        item.DEFFECDATE = odr["DEFFECDATE"] == DBNull.Value ? string.Empty : odr["DEFFECDATE"].ToString();
                        item.DEFFECDATE = item.DEFFECDATE == string.Empty ? "" : Convert.ToDateTime(item.DEFFECDATE).ToString("MM/dd/yyyy");


                        item.DESC_USER_ACTION = odr["DESC_USER_ACTION"] == DBNull.Value ? string.Empty : odr["DESC_USER_ACTION"].ToString();

                        item.DEFFECDATE_ACTION = odr["DEFFECDATE_ACTION"] == DBNull.Value ? string.Empty : odr["DEFFECDATE_ACTION"].ToString();
                        item.DEFFECDATE_ACTION = item.DEFFECDATE_ACTION == string.Empty ? "" : Convert.ToDateTime(item.DEFFECDATE_ACTION).ToString("MM/dd/yyyy");


                        item.NSTATE = odr["NSTATE"] == DBNull.Value ? 0 : Int32.Parse(odr["NSTATE"].ToString());
                        item.CONFIG_EDITABLE = odr["CONFIG_EDITABLE"] == DBNull.Value ? 0 : Int32.Parse(odr["CONFIG_EDITABLE"].ToString());


                        if (item.NSTATE == 0)
                        {
                            item.DESC_STATE = "PENDIENTE";
                        }

                        else if (item.NSTATE == 1)
                        {

                            item.DESC_STATE = "APROBADO";
                        }
                        else
                        {
                            item.DESC_STATE = "RECHAZADO";
                        }

                        last_commission.Add(item);
                    }
                }
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("GetLastCommission", ex.ToString(), "3");
            }

            return last_commission;
        }

        public GenericResponse InsCommissionVDP(CommissionBR commission)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            GenericResponse response = new GenericResponse();
            string storedProcedureName = ProcedureName.pkg_Comision_VDP + ".SP_INS_HIS_COMMISSION_VDP";
            try
            {
                string format = "MM/dd/yyyy";

                //INPUT
                parameter.Add(new OracleParameter("P_NINI_MONTH", OracleDbType.Int32, commission.NINI_MONTH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCANTMONTHS", OracleDbType.Int32, commission.NCANTMONTHS, ParameterDirection.Input));

                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Date, DateTime.ParseExact(commission.DEFFECDATE, format, CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"), ParameterDirection.Input));

                parameter.Add(new OracleParameter("P_NIDUSER_REGISTER", OracleDbType.Int64, commission.NIDUSER_REGISTER, ParameterDirection.Input));

                //OUTPUT
                var P_NERROR = new OracleParameter("P_NERROR", OracleDbType.Int16, 100, ParameterDirection.Output);
                var P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 5000, ParameterDirection.Output);
                P_SMESSAGE.Size = 4000;
                parameter.Add(P_NERROR);
                parameter.Add(P_SMESSAGE);

                this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                response.code = Int32.Parse(P_NERROR.Value.ToString());
                response.message = P_SMESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                response.code = 1;
                response.message = ex.Message;
                LogControl.save("InsCommissionVDP", ex.ToString(), "3");
            }

            return response;
        }

        public GenericResponse UpdCommissionVDP(CommissionBR commission)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            GenericResponse response = new GenericResponse();
            string storedProcedureName = ProcedureName.pkg_Comision_VDP + ".SP_UPD_HIS_COMMISSION_VDP";
            try
            {

                // P_NSTATE El valor del estado que se tiene actualmente;
                // RECIBE FECHA CON MES DIA AÑO
                string format = "MM/dd/yyyy";

                var tipo = commission.DEFFECDATE.GetType();

                //INPUT
                parameter.Add(new OracleParameter("P_NINI_MONTH", OracleDbType.Int32, commission.NINI_MONTH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCANTMONTHS", OracleDbType.Int32, commission.NCANTMONTHS, ParameterDirection.Input));

                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Date, DateTime.ParseExact(commission.DEFFECDATE, format, CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"), ParameterDirection.Input));
                //parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Date, DateTime.Parse(commission.DEFFECDATE).ToString("dd/MM/yyyy"), ParameterDirection.Input));

                parameter.Add(new OracleParameter("P_NIDUSER_REGISTER", OracleDbType.Int64, commission.NIDUSER_REGISTER, ParameterDirection.Input));
                // Id de la configuracion para poder actualizar si es Comercial
                parameter.Add(new OracleParameter("P_NHIS_CONFIGURATION", OracleDbType.Int64, commission.NHIS_CONFIGURATION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NSTATE", OracleDbType.Int64, commission.NSTATE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NSTATE_ACTION", OracleDbType.Int64, commission.NSTATE_ACTION, ParameterDirection.Input));

                //TIpo de usuario para saber si es comercial  o tecnica, si es comercial solo me va dejar actualizar la ultima Solicitud Pendiente
                parameter.Add(new OracleParameter("P_TYPE_USU", OracleDbType.Int64, commission.NTYPE_USU, ParameterDirection.Input));


                //OUTPUT
                var P_NERROR = new OracleParameter("P_NERROR", OracleDbType.Int16, 100, ParameterDirection.Output);
                var P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 5000, ParameterDirection.Output);
                P_SMESSAGE.Size = 4000;
                parameter.Add(P_NERROR);
                parameter.Add(P_SMESSAGE);

                this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                response.code = Int32.Parse(P_NERROR.Value.ToString());
                response.message = P_SMESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                response.code = 1;
                response.message = ex.Message;
                LogControl.save("InsCommissionVDP", ex.ToString(), "3");
            }

            return response;
        }

        public ValPerfilVDP ValPerfilVDP(int nid_user)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            ValPerfilVDP response = new ValPerfilVDP();
            string storedProcedureName = "INSUDB.SP_VAL_PERFIL";
            response.IS_TECNICA = false;
            response.IS_COMERCIAL = false;
            try
            {

                //INPUT
                parameter.Add(new OracleParameter("P_NIDUSER", OracleDbType.Int32, nid_user, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        ValPerfilVDP item = new ValPerfilVDP();

                        item.NIDUSER = odr["NIDUSER"] == DBNull.Value ? 0 : Int32.Parse(odr["NIDUSER"].ToString());
                        item.NIDPROFILE = odr["NIDPROFILE"] == DBNull.Value ? 0 : Int32.Parse(odr["NIDPROFILE"].ToString());
                        item.NIDPRODUCT = odr["NIDPRODUCT"] == DBNull.Value ? 0 : Int32.Parse(odr["NIDPRODUCT"].ToString());

                        if (item.NIDPROFILE == 7)
                        {
                            response.IS_COMERCIAL = true;
                        }

                        if (item.NIDPROFILE == 137)
                        {
                            response.IS_TECNICA = true;
                        }

                    }

                }
               
            }
            catch (Exception ex)
            {
                response.IS_TECNICA = false;
                response.IS_COMERCIAL = false;
                LogControl.save("ValPerfilVDP", ex.ToString(), "3");
            }

            return response;
        }

        public GenericResponse UpdCommissionBR(CommissionBR commission)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            GenericResponse response = new GenericResponse();
            string storedProcedureName = ProcedureName.pkg_Comision + ".INSUPD_COMISION_BR";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_SCERTYPE", OracleDbType.Varchar2, commission.P_SCERTYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, commission.P_NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, commission.P_NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int64, commission.P_NPOLICY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Varchar2, commission.P_DEFFECDATE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NAMOUNT", OracleDbType.Decimal, commission.P_NAMOUNT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NINTERMED", OracleDbType.Int64, commission.P_NINTERMED, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, commission.P_NUSERCODE, ParameterDirection.Input));

                var Code = new OracleParameter("P_NCODE", OracleDbType.Int16, 100, ParameterDirection.Output);
                var Message = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 5000, ParameterDirection.Output);
                Message.Size = 4000;
                parameter.Add(Code);
                parameter.Add(Message);

                this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                response.code = Int32.Parse(Code.Value.ToString());
                response.message = Message.Value.ToString();

            }
            catch (Exception ex)
            {
                response.code = 1;
                response.message = ex.Message;
                LogControl.save("UpdCommissionBR", ex.ToString(), "3");
            }

            return response;
        }
    }
}
