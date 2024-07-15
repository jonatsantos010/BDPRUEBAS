using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WSPlataforma.Entities.AgencyModel.BindingModel;
using WSPlataforma.Entities.AgencyModel.ViewModel;
using WSPlataforma.Util;

namespace WSPlataforma.DA
{
    public class AgencyDA : ConnectionBase
    {
        private SharedMethods sharedMethods = new SharedMethods();

        public GenericResponseVM GetBrokerAgencyList(AgencySearchBM data)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            GenericResponseVM resultPackage = new GenericResponseVM();
            List<BrokerAgencyVM> list = new List<BrokerAgencyVM>();
            string storedProcedureName = ProcedureName.pkg_BrokerAgency + ".rea_agenciamiento";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NIDTYPECHANNEL", OracleDbType.Varchar2, data.ChannelTypeId, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_CODCHANNEL", OracleDbType.Varchar2, data.BrokerId, ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_startdate", OracleDbType.Date, data.StartDate, ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_enddate", OracleDbType.Date, data.EndDate, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLIMITPERPAGE", OracleDbType.Int32, data.LimitPerpage, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPAGENUM", OracleDbType.Int32, data.PageNumber, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, list, ParameterDirection.Output);
                OracleParameter NTOTALROWS = new OracleParameter("P_NTOTALROWS", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);

                parameter.Add(C_TABLE);
                parameter.Add(NTOTALROWS);
                parameter.Add(P_MESSAGE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                while (odr.Read())
                {
                    BrokerAgencyVM item = new BrokerAgencyVM();
                    item.BrokerDocumentTypeId = odr["TYPE_DOC_BR"].ToString();
                    item.BrokerDocumentTypeName = odr["DES_DOC_BR"].ToString();
                    item.BrokerDocumentNumber = odr["SIDDOC_BR"].ToString();
                    item.BrokerFullName = odr["SCLIENAME_BR"].ToString();

                    item.BrokerId = odr["COD_CANAL"].ToString();
                    item.ChannelTypeId = odr["NTYPECHANNEL"].ToString();

                    item.ContractorDocumentTypeId = odr["TYPE_DOC_CO"].ToString();
                    item.ContractorDocumentTypeName = odr["DES_DOC_CO"].ToString();
                    item.ContractorDocumentNumber = odr["SIDDOC_CO"].ToString();
                    item.ContractorFullName = odr["SCLIENAME_CO"].ToString();
                    item.ProductName = odr["DES_PRODUCT"].ToString();
                    item.ProductId = odr["NPRODUCT"].ToString();

                    if (odr["DSTARTRELATION"].ToString() != null && odr["DSTARTRELATION"].ToString().Trim() != "") item.AgencyStartDate = Convert.ToDateTime(odr["DSTARTRELATION"].ToString());
                    else item.AgencyStartDate = null;

                    if (odr["DENDRELATION"].ToString() != null && odr["DENDRELATION"].ToString().Trim() != "") item.AgencyEndDate = Convert.ToDateTime(odr["DENDRELATION"].ToString());
                    else item.AgencyEndDate = null;
                    item.FilePath = odr["SRUTA"] == null || odr["SRUTA"].ToString().Trim() == "" ? "" : String.Format(ELog.obtainConfig("pathPrincipal"), odr["SRUTA"].ToString().Trim());

                    list.Add(item);

                }

                ELog.CloseConnection(odr);
                resultPackage.TotalRowNumber = Convert.ToInt32(NTOTALROWS.Value.ToString());
                resultPackage.GenericResponse = list;
            }
            catch (Exception ex)
            {
                LogControl.save("GetBrokerAgencyList", ex.ToString(), "3");
            }

            return resultPackage;
        }
        public GenericResponseVM GetLastBrokerList(String clientId, string nbranch, string nproduct, int limitPerpage, int pageNumber) //obtener la lista de ultimo broker por cada producto
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            GenericResponseVM resultPackage = new GenericResponseVM();
            List<ContractorBrokerVM> list = new List<ContractorBrokerVM>();
            string storedProcedureName = ProcedureName.pkg_BrokerAgency + ".REA_LAST_BR";
            try
            {
                parameter.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, clientId, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, nbranch, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, nproduct, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLIMITPERPAGE", OracleDbType.Int32, limitPerpage, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPAGENUM", OracleDbType.Int32, pageNumber, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, list, ParameterDirection.Output);
                OracleParameter NTOTALROWS = new OracleParameter("P_NTOTALROWS", OracleDbType.Int32, resultPackage.TotalRowNumber, ParameterDirection.Output);

                NTOTALROWS.Size = 4000;

                parameter.Add(C_TABLE);
                parameter.Add(NTOTALROWS);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                while (odr.Read())
                {
                    ContractorBrokerVM item = new ContractorBrokerVM();
                    item.BrokerDocumentTypeName = odr["DES_DOC_BR"].ToString();
                    item.BrokerDocumentNumber = odr["SIDDOC_BR"].ToString();
                    item.BrokerFullName = odr["NOMBRE"].ToString();
                    item.ProductName = odr["DES_PRODUCT"].ToString();
                    item.ProductId = odr["NPRODUCT"].ToString();

                    if (odr["AGEN_INI"].ToString() != null && odr["AGEN_INI"].ToString().Trim() != "") item.StartDate = Convert.ToDateTime(odr["AGEN_INI"].ToString());
                    else item.StartDate = null;

                    if (odr["AGEN_FIN"].ToString() != null && odr["AGEN_FIN"].ToString().Trim() != "") item.EndDate = Convert.ToDateTime(odr["AGEN_FIN"].ToString());
                    else item.EndDate = null;

                    list.Add(item);

                }

                ELog.CloseConnection(odr);
                resultPackage.TotalRowNumber = Convert.ToInt32(NTOTALROWS.Value.ToString());
                resultPackage.GenericResponse = list;
            }
            catch (Exception ex)
            {
                LogControl.save("GetLastBrokerList", ex.ToString(), "3");
            }

            return resultPackage;
        }

        public GenericResponseVM AddAgency(AgencyBM agencyData, HttpPostedFile file)
        {
            GenericResponseVM resultPackage = new GenericResponseVM();
            string storedProcedureName = ProcedureName.pkg_BrokerAgency + ".INS_CANAL";
            string folderPath = ELog.obtainConfig("pathAgenciamiento");
            var connectionString = ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString();
            using (var connection = new OracleConnection(connectionString))
            {
                connection.Open();
                var transaction = connection.BeginTransaction();
                try
                {

                    DbCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.Connection = connection;
                    command.CommandText = storedProcedureName;
                    command.Transaction = transaction;

                    command.Parameters.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, agencyData.ContractorId, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("P_NTYPECHANNEL", OracleDbType.Varchar2, agencyData.ChannelTypeId, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("P_NCANAL", OracleDbType.Varchar2, agencyData.BrokerId, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("P_BRANCH", OracleDbType.Varchar2, "77", ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, agencyData.ProductId, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("P_DSTARTRELATION", OracleDbType.Date, agencyData.AgencyDate.Date, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("P_DENDRELATION", OracleDbType.Date, null, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, agencyData.User, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("P_SINTERMEDR", OracleDbType.Varchar2, null, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("P_NDOCUMENTACION", OracleDbType.Varchar2, null, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("P_DFECDOCUMENT", OracleDbType.Date, null, ParameterDirection.Input));

                    if (agencyData.FileName.Trim() == "") command.Parameters.Add(new OracleParameter("P_SRUTA", OracleDbType.Varchar2, null, ParameterDirection.Input));
                    else command.Parameters.Add(new OracleParameter("P_SRUTA", OracleDbType.Varchar2, folderPath + agencyData.FileName, ParameterDirection.Input));

                    command.Parameters.Add(new OracleParameter("P_CNOMBRA", OracleDbType.Int32, 0, ParameterDirection.Input));

                    command.Parameters.Add(new OracleParameter("P_STATUS", OracleDbType.Int32, 4000, null, ParameterDirection.Output));
                    command.Parameters.Add(new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output));

                    command.ExecuteNonQuery();
                    resultPackage.StatusCode = Convert.ToInt32(command.Parameters["P_STATUS"].Value.ToString());
                    resultPackage.Message = command.Parameters["P_MESSAGE"].Value.ToString();

                    if (resultPackage.StatusCode == 0) //si el registro ha sido exitoso
                    {
                        if (agencyData.FileName != null && agencyData.FileName.Trim() != "")
                        {

                            Directory.CreateDirectory(String.Format(ELog.obtainConfig("pathPrincipal"), folderPath));
                            file.SaveAs(String.Format(ELog.obtainConfig("pathPrincipal"), folderPath + agencyData.FileName));
                            transaction.Commit();
                            connection.Close();
                            connection.Dispose();
                            return resultPackage;
                        }
                        else
                        {
                            transaction.Commit();
                            connection.Close();
                            connection.Dispose();
                            return resultPackage;
                        }
                    }
                    else
                    {
                        transaction.Rollback();
                        connection.Close();
                        connection.Dispose();
                        return resultPackage;
                    }

                }
                catch (Exception ex)
                {
                    LogControl.save("AddAgency", ex.ToString(), "3");
                    transaction.Rollback();
                    connection.Close();
                    connection.Dispose();
                    throw;
                }

            }
        }

    }
}
