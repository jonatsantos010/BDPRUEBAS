using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.Entities.AccountStateModel.BindingModel;
using WSPlataforma.Entities.AccountStateModel.ViewModel;
using WSPlataforma.Util;

namespace WSPlataforma.DA
{
    public class AccountStateDA : ConnectionBase
    {
        private SharedMethods sharedMethods = new SharedMethods();

        public List<QualificationVM> GetQualificationTypeList()
        {
            string storedProcedure = ProcedureName.pkg_EstadoCuenta + ".REA_TABLE801";
            List<OracleParameter> parameters = new List<OracleParameter>();
            List<QualificationVM> list = new List<QualificationVM>();

            try
            {
                OracleParameter table = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameters.Add(table);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedure, parameters);
                while (odr.Read())
                {
                    QualificationVM item = new QualificationVM();
                    item.Id = odr["NCAL_CLIENT"].ToString();
                    item.Name = odr["SDESCRIPT"].ToString();

                    list.Add(item);
                }

                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                LogControl.save("GetQualificationTypeList", ex.ToString(), "3");
            }

            return list;
        }

        public GenericPackageVM GetCreditEvaluationList(CreditEvaluationSearchBM filter)
        {
            string storedProcedure = ProcedureName.pkg_EstadoCuenta + ".REA_CAL_CLIENT_HIS";
            GenericPackageVM resultPackage = new GenericPackageVM();
            List<OracleParameter> parameters = new List<OracleParameter>();
            List<CreditEvaluationVM> list = new List<CreditEvaluationVM>();

            try
            {
                parameters.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, filter.ClientId, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_STIPO_CAL", OracleDbType.Varchar2, filter.Qualification, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_FEC_INI", OracleDbType.Varchar2, filter.StartDate.Value.Date.ToString("dd/MM/yyyy"), ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_FEC_FIN", OracleDbType.Varchar2, filter.EndDate.Value.Date.ToString("dd/MM/yyyy"), ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NLIMITPERPAGE", OracleDbType.Int32, filter.LimitPerPage, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPAGENUM", OracleDbType.Int32, filter.PageNumber, ParameterDirection.Input));

                OracleParameter table = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter totalRowNumber = new OracleParameter("P_NTOTALROWS", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                parameters.Add(table);
                parameters.Add(totalRowNumber);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedure, parameters);

                while (odr.Read())
                {
                    CreditEvaluationVM item = new CreditEvaluationVM();
                    item.Qualification = odr["DESC_EVAL"].ToString();
                    if (odr["FEC_OPERACION"].ToString() != null && odr["FEC_OPERACION"].ToString().Trim() != "") item.Date = Convert.ToDateTime(odr["FEC_OPERACION"].ToString());
                    else item.Date = null;
                    item.User = odr["USUARIO"].ToString();

                    list.Add(item);
                }

                ELog.CloseConnection(odr);
                resultPackage.TotalRowNumber = Convert.ToInt32(totalRowNumber.Value.ToString());
                resultPackage.GenericResponse = list;
                resultPackage.StatusCode = 0;
            }
            catch (Exception ex)
            {
                LogControl.save("GetCreditEvaluationList", ex.ToString(), "3");
            }

            return resultPackage;
        }

        public GenericPackageVM EvaluateClient(CreditEvaluationBM data)
        {
            string storedProcedure = ProcedureName.pkg_EstadoCuenta + ".INS_CAL_CLIENT";
            GenericPackageVM resultPackage = new GenericPackageVM();
            List<OracleParameter> parameters = new List<OracleParameter>();

            try
            {
                parameters.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, data.ClientId, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCAL_CLIENT", OracleDbType.Varchar2, data.Qualification, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Varchar2, data.User, ParameterDirection.Input));

                OracleParameter message = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                OracleParameter statusCode = new OracleParameter("P_COD_ERR", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                parameters.Add(message);
                parameters.Add(statusCode);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedure, parameters);

                resultPackage.StatusCode = Convert.ToInt32(statusCode.Value.ToString());
                resultPackage.ErrorMessageList = sharedMethods.StringToList(message.Value.ToString());
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("EvaluateClient", ex.ToString(), "3");
            }

            return resultPackage;
        }
        public GenericPackageVM EnableClientMovement(ClientEnablementBM data)
        {
            string storedProcedure = ProcedureName.pkg_EstadoCuenta + ".SPS_LIBERAR_CLIENTE";
            GenericPackageVM resultPackage = new GenericPackageVM();
            List<OracleParameter> parameters = new List<OracleParameter>();

            try
            {
                parameters.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, data.ClientId, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Varchar2, data.User, ParameterDirection.Input));

                OracleParameter statusCode = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter message = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(statusCode);
                parameters.Add(message);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedure, parameters);


                resultPackage.StatusCode = Convert.ToInt32(statusCode.Value.ToString());
                if (resultPackage.StatusCode != 0)
                {
                    resultPackage.ErrorMessageList = sharedMethods.StringToList(message.Value.ToString());
                }
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("EnableClientMovement", ex.ToString(), "3");
            }

            return resultPackage;
        }

        public ContractorStateVM GetContractorState(string clientId)
        {
            string storedProcedure = ProcedureName.pkg_EstadoCuenta + ".REA_CAL_CLIENT";
            List<OracleParameter> parameters = new List<OracleParameter>();

            try
            {
                parameters.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, clientId, ParameterDirection.Input));

                OracleParameter table = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameters.Add(table);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedure, parameters);

                if (odr.HasRows)
                {
                    odr.Read();
                    ContractorStateVM item = new ContractorStateVM();
                    item.CreditEnablementId = odr["COD_CREDIT"].ToString();
                    item.CreditEnablementName = odr["DESC_CREDIT"].ToString();
                    item.MovementEnablementId = odr["COD_EXEC_MOV"].ToString();
                    item.MovementEnablementName = odr["DESC_EXEC"].ToString();
                    item.LastCreditEvaluationId = odr["COD_EVAL"].ToString();
                    item.LastCreditEvaluationName = odr["DESC_EVAL"].ToString();
                    item.LatePaymentDays = Int32.Parse(odr["DIAS_MORA"].ToString());
                    ELog.CloseConnection(odr);
                    return item;
                }

                else return null;
            }
            catch (Exception ex)
            {
                LogControl.save("GetContractorState", ex.ToString(), "3");
                return null;
            }

        }

    }
}
