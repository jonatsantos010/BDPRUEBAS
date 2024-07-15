using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.Entities.AccountStateModel.ViewModel;
using WSPlataforma.Entities.CobranzasModel;
using WSPlataforma.Entities.PolicyModel.ViewModel;
using WSPlataforma.Util;
using WSPlataforma.Entities.PrintPolicyModel;
using WSPlataforma.Entities.PrintPolicyModel.ViewModel;
using WSPlataforma.DA.PrintPolicyPDF;
using WSPlataforma.Util.PrintPolicyUtility;
using WSPlataforma.DA;

namespace WSPlataforma.DA
{
    public class CobranzasDA : ConnectionBase
    {
        public List<TypeRiskBM> getTypeRisk()
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<TypeRiskBM> listRisk = new List<TypeRiskBM>();
            string storedProcedureName = ProcedureName.pkg_PoliticasCredito + ".REA_TYPE_RISK";
            try
            {
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        TypeRiskBM item = new TypeRiskBM();
                        item.idTypeRish = Convert.ToInt16(odr["NTYPE_RISK"].ToString());
                        item.description = odr["SDESCRIPT"].ToString();
                        listRisk.Add(item);
                    }
                }

                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("getTypeRisk", ex.ToString(), "3");
            }
            return listRisk;
        }

        public ResponseVM InsertCreditPolicies(EntityPolicyCreditBM entitie)
        {
            ResponseVM response = new ResponseVM();
            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = cn;

                    try
                    {

                        cmd.CommandText = string.Format("{0}.{1}", ProcedureName.pkg_PoliticasCredito, "INSUPD_CREDIT_POLICIES");

                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("P_NID_CRED", OracleDbType.Int32).Value = entitie.idPolicyCredit;
                        cmd.Parameters.Add("P_NTYPE_RISK", OracleDbType.Int32).Value = entitie.tipoRiesgo;
                        cmd.Parameters.Add("P_NBRANCH", OracleDbType.Int32).Value = entitie.idRamo;
                        cmd.Parameters.Add("P_NPRODUCT", OracleDbType.Int32).Value = entitie.idProduct;
                        cmd.Parameters.Add("P_SDESCRIPT", OracleDbType.Varchar2).Value = entitie.description;
                        cmd.Parameters.Add("P_NAMOUNT_MIN", OracleDbType.Decimal).Value = entitie.montoMinimo;
                        cmd.Parameters.Add("P_NAMOUNT_MAX", OracleDbType.Decimal).Value = entitie.montoMaximo;
                        cmd.Parameters.Add("P_NCREDIT_DAYS", OracleDbType.Int32).Value = entitie.diasCredito;
                        cmd.Parameters.Add("P_DSTARDATE", OracleDbType.Varchar2).Value = entitie.FechaEfecto;
                        cmd.Parameters.Add("P_DNULLDATE", OracleDbType.Varchar2).Value = entitie.fechaAnulacion;

                        cmd.Parameters.Add("P_NUSERCODE", OracleDbType.Int32).Value = entitie.usercode;

                        //OUTPUT
                        OracleParameter P_NCODE = new OracleParameter("P_COD_ERR", OracleDbType.Int32, string.Empty, ParameterDirection.Output);
                        OracleParameter P_SMESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, string.Empty, ParameterDirection.Output);

                        P_NCODE.Size = 4000;
                        P_SMESSAGE.Size = 4000;
                        cmd.Parameters.Add(P_SMESSAGE);
                        cmd.Parameters.Add(P_NCODE);

                        cn.Open();
                        cmd.ExecuteNonQuery();
                        response.P_NCODE = P_NCODE.Value.ToString();
                        response.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                        cn.Close();
                        cmd.Dispose();

                    }
                    catch (Exception ex)
                    {
                        response.P_NCODE = "1";
                        response.P_SMESSAGE = ex.ToString();
                        LogControl.save("InsertCreditPolicies", ex.ToString(), "3");
                    }
                    finally
                    {
                        if (cn.State == ConnectionState.Open) cn.Close();
                    }
                }
            }
            return response;
        }

        public List<EntityPolicyCreditBM> getCreditPoliciesList(ParamsCreditPoliciesBM param)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<EntityPolicyCreditBM> listEntity = new List<EntityPolicyCreditBM>();
            string storedProcedureName = ProcedureName.pkg_PoliticasCredito + ".REA_CREDIT_POLICIES";
            try
            {
                //INPUT

                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, param.idRamo, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, param.idProducto, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DSTARDATE", OracleDbType.Varchar2, param.fechaEfecto, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        EntityPolicyCreditBM item = new EntityPolicyCreditBM();
                        item.idPolicyCredit = odr["ITEM"] == DBNull.Value ? 0 : Int32.Parse(odr["ITEM"].ToString());
                        item.idRamo = odr["COD_RAMO"] == DBNull.Value ? 0 : Int32.Parse(odr["COD_RAMO"].ToString());
                        item.idProduct = odr["COD_PRODUCTO"] == DBNull.Value ? 0 : Int32.Parse(odr["COD_PRODUCTO"].ToString());
                        item.desRamo = odr["RAMO"] == DBNull.Value ? string.Empty : odr["RAMO"].ToString();
                        item.desProduct = odr["PRODUCTO"] == DBNull.Value ? string.Empty : odr["PRODUCTO"].ToString();
                        item.description = odr["DESCRIPCION"] == DBNull.Value ? string.Empty : odr["DESCRIPCION"].ToString();
                        item.montoMinimo = odr["MONTO_MINIMO"] == DBNull.Value ? 0 : Decimal.Parse(odr["MONTO_MINIMO"].ToString());
                        item.montoMaximo = odr["MONTO_MAXIMO"] == DBNull.Value ? 0 : Decimal.Parse(odr["MONTO_MAXIMO"].ToString());
                        item.diasCredito = odr["DIAS_CRED"] == DBNull.Value ? 0 : Int32.Parse(odr["DIAS_CRED"].ToString());
                        item.FechaEfecto = odr["FECHA_EFECTO"] == DBNull.Value ? string.Empty : odr["FECHA_EFECTO"].ToString();
                        item.usercode = odr["ID_USUARIO"] == DBNull.Value ? string.Empty : odr["ID_USUARIO"].ToString();
                        item.desUsuario = odr["USUARIO"] == DBNull.Value ? string.Empty : odr["USUARIO"].ToString();
                        item.tipoRiesgo = odr["ID_TIPO_RIESGO"] == DBNull.Value ? 0 : Int32.Parse(odr["ID_TIPO_RIESGO"].ToString());
                        item.desRiesgo = odr["TIPO_RIESGO"] == DBNull.Value ? string.Empty : odr["TIPO_RIESGO"].ToString();
                        item.fechaAnulacion = odr["FECHA_ANULACION"] == DBNull.Value ? string.Empty : odr["FECHA_ANULACION"].ToString();
                        listEntity.Add(item);
                    }
                }
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                LogControl.save("getCreditPoliciesList", ex.ToString(), "3");
            }

            return listEntity;
        }


        public List<EntityPaymentClientStatusBM> getStatusClientList(ParamsPaymentClientStatus param)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<EntityPaymentClientStatusBM> listEntity = new List<EntityPaymentClientStatusBM>();
            string storedProcedureName = ProcedureName.pkg_EstadoCobroCliente + ".REA_CLIENT_RESTRIC";
            try
            {
                //INPUT

                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, param.idRamo, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, param.idProducto, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDDOC_TYPE", OracleDbType.Int32, param.idTipoDocumento, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, param.nroDocumento, ParameterDirection.Input));


                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        EntityPaymentClientStatusBM item = new EntityPaymentClientStatusBM();
                        item.idRamo = odr["ID_RAMO"] == DBNull.Value ? 0 : Int32.Parse(odr["ID_RAMO"].ToString());
                        item.desRamo = odr["RAMO"] == DBNull.Value ? string.Empty : odr["RAMO"].ToString();
                        item.idProducto = odr["ID_PRODUCT"] == DBNull.Value ? 0 : Int32.Parse(odr["ID_PRODUCT"].ToString());
                        item.desProducto = odr["PRODUCTO"] == DBNull.Value ? string.Empty : odr["PRODUCTO"].ToString();
                        item.idTipoDocumento = odr["ID_TIPO_DOCUMENTO"] == DBNull.Value ? 0 : Int32.Parse(odr["ID_TIPO_DOCUMENTO"].ToString());
                        item.desTipoDocumento = odr["TIPO_DOCUMENTO"] == DBNull.Value ? string.Empty : odr["TIPO_DOCUMENTO"].ToString();
                        item.documento = odr["DOCUMENTO"] == DBNull.Value ? string.Empty : odr["DOCUMENTO"].ToString();
                        item.descripcion = odr["DESCRIPCION"] == DBNull.Value ? string.Empty : odr["DESCRIPCION"].ToString();
                        item.observacion = odr["OBSERVACION"] == DBNull.Value ? string.Empty : odr["OBSERVACION"].ToString();
                        item.idRestriccion = odr["ID_TIPO_RESTRICCION"] == DBNull.Value ? 0 : Int32.Parse(odr["ID_TIPO_RESTRICCION"].ToString());
                        item.desRestriccion = odr["RESTRICCION"] == DBNull.Value ? string.Empty : odr["RESTRICCION"].ToString();
                        item.idusuario = odr["ID_USUARIO"] == DBNull.Value ? 0 : Int32.Parse(odr["ID_USUARIO"].ToString());
                        item.desUsuario = odr["USUARIO"] == DBNull.Value ? string.Empty : odr["USUARIO"].ToString();
                        item.FInitPago = odr["DSTARTPAYMENT"] == DBNull.Value ? string.Empty : odr["DSTARTPAYMENT"].ToString();
                        item.FVigenPago = odr["DENDPAYMENT"] == DBNull.Value ? string.Empty : odr["DENDPAYMENT"].ToString();
                        listEntity.Add(item);
                    }
                }
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("getStatusClientList", ex.ToString(), "3");
            }

            return listEntity;
        }
        public List<EntityPaymentClientInfoBM> getClientInfoList(ParamsPaymentClientInfo param)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<EntityPaymentClientInfoBM> listEntity = new List<EntityPaymentClientInfoBM>();
            string storedProcedureName = ProcedureName.pkg_EstadoCobroCliente + ".REA_CLIENT_INFO";
            try
            {
                //INPUT

                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, param.idRamo, ParameterDirection.Input));

                parameter.Add(new OracleParameter("P_NIDDOC_TYPE", OracleDbType.Int32, param.idTipoDocumento, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, param.documento, ParameterDirection.Input));


                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        EntityPaymentClientInfoBM item = new EntityPaymentClientInfoBM();
                        item.documento = odr["SIDDOC"] == DBNull.Value ? string.Empty : odr["SIDDOC"].ToString();
                        item.idTipoDocumento = odr["NIDDOC_TYPE"] == DBNull.Value ? 0 : Int32.Parse(odr["NIDDOC_TYPE"].ToString());
                        item.razonSocial = odr["SCLIENAME"] == DBNull.Value ? string.Empty : odr["SCLIENAME"].ToString();
                        listEntity.Add(item);
                    }
                }
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("getClientInfoList", ex.ToString(), "3");
            }

            return listEntity;
        }
        public List<EntityPrivilegiosClientBM> getClientPrivilegiosList(ParamsPaymentClientStatus param)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<EntityPrivilegiosClientBM> listEntity = new List<EntityPrivilegiosClientBM>();
            string storedProcedureName = ProcedureName.pkg_EstadoCobroCliente + ".REA_CLIENT_PRIVILEGES";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NIDDOC_TYPE", OracleDbType.Int32, param.idTipoDocumento, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, param.nroDocumento, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Varchar2, param.idRamo, ParameterDirection.Input));


                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        EntityPrivilegiosClientBM item = new EntityPrivilegiosClientBM();
                        item.tipoDetRestric = odr["NTYPE_DET_RESTRIC"] == DBNull.Value ? string.Empty : odr["NTYPE_DET_RESTRIC"].ToString();
                        item.creditDias = odr["NCREDIT_DAYS"] == DBNull.Value ? string.Empty : odr["NCREDIT_DAYS"].ToString();
                        item.indNoLimit = odr["NFLAG_NO_LIMIT"] == DBNull.Value ? string.Empty : odr["NFLAG_NO_LIMIT"].ToString();
                        listEntity.Add(item);
                    }
                }
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                LogControl.save("getClientPrivilegiosList", ex.ToString(), "3");
            }

            return listEntity;
        }


        public List<TypeRestricBM> getTypeRestric()
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<TypeRestricBM> listRestric = new List<TypeRestricBM>();
            string storedProcedureName = ProcedureName.pkg_EstadoCobroCliente + ".REA_TYPE_RESTRIC";
            try
            {
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        TypeRestricBM item = new TypeRestricBM();
                        item.idTypeRestric = Convert.ToInt16(odr["ID_RESTRIC"].ToString());
                        item.description = odr["DES_RESTRIC"].ToString();
                        listRestric.Add(item);
                    }
                }
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                LogControl.save("getTypeRestric", ex.ToString(), "3");
            }

            return listRestric;
        }

        public ResponseVM InsertClientRestric(ParamsPaymentClientStatus entitie)
        {
            ResponseVM response = new ResponseVM();
            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = cn;

                    try
                    {

                        cmd.CommandText = string.Format("{0}.{1}", ProcedureName.pkg_EstadoCobroCliente, "INSUPD_CLIENT_RESTRIC");

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("P_NBRANCH", OracleDbType.Int32).Value = entitie.idRamo;
                        cmd.Parameters.Add("P_NPRODUCT", OracleDbType.Int32).Value = entitie.idProducto;
                        cmd.Parameters.Add("P_NIDDOC_TYPE", OracleDbType.Int32).Value = entitie.idTipoDocumento;
                        cmd.Parameters.Add("P_SIDDOC", OracleDbType.Varchar2).Value = entitie.nroDocumento;
                        cmd.Parameters.Add("P_SDESCRIPT", OracleDbType.Varchar2).Value = entitie.descripcion;
                        cmd.Parameters.Add("P_SOBSERVACION", OracleDbType.Varchar2).Value = entitie.observacion;
                        cmd.Parameters.Add("P_NTYPE_RESTRIC", OracleDbType.Int32).Value = entitie.idRestric;
                        cmd.Parameters.Add("P_NUSERCODE", OracleDbType.Int32).Value = entitie.usercode;
                        cmd.Parameters.Add("P_SSTATREGT", OracleDbType.Varchar2).Value = entitie.indEstado;
                        cmd.Parameters.Add("P_NCREDIT_DAYS_EM", OracleDbType.Varchar2).Value = entitie.daysCreditEmi;
                        cmd.Parameters.Add("P_NCREDIT_DAYS_IN", OracleDbType.Varchar2).Value = entitie.daysCreditInc;
                        cmd.Parameters.Add("P_NCREDIT_DAYS_RE", OracleDbType.Varchar2).Value = entitie.daysCreditRen;
                        cmd.Parameters.Add("P_DSTARTPAYMENT", OracleDbType.Date).Value = entitie.FDateInitPago == null ? (DateTime?)null : Convert.ToDateTime(entitie.FDateInitPago);
                        cmd.Parameters.Add("P_DENDPAYMENT", OracleDbType.Date).Value = entitie.FDateVigenPago == null ? (DateTime?)null : Convert.ToDateTime(entitie.FDateVigenPago);
                        //OUTPUT
                        OracleParameter P_NCODE = new OracleParameter("P_COD_ERR", OracleDbType.Int32, string.Empty, ParameterDirection.Output);
                        OracleParameter P_SMESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, string.Empty, ParameterDirection.Output);

                        P_NCODE.Size = 4000;
                        P_SMESSAGE.Size = 4000;
                        cmd.Parameters.Add(P_SMESSAGE);
                        cmd.Parameters.Add(P_NCODE);

                        cn.Open();

                        cmd.ExecuteNonQuery();
                        response.P_NCODE = P_NCODE.Value.ToString();
                        response.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                        cn.Close();
                        cmd.Dispose();

                    }
                    catch (Exception ex)
                    {
                        LogControl.save("InsertClientRestric", ex.ToString(), "3");
                    }
                    finally
                    {
                        if (cn.State == ConnectionState.Open) cn.Close();
                    }
                }
            }
            return response;
        }

        public EntityValidateLock ValidateLock(ParamsValidateLock param)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            EntityValidateLock entity = new EntityValidateLock();
            string storedProcedureName = ProcedureName.pkg_EstadoCiente + ".VALIDAR_BLOQUEO";
            try
            {
                //INPUT

                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, param.branchCode, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, param.productCode, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDDOC_TYPE", OracleDbType.Int32, param.documentType, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, param.documentNumber, ParameterDirection.Input));
                //OUTPUT
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, ParameterDirection.Output);

                P_MESSAGE.Size = 9000;
                P_COD_ERR.Size = 9000;

                parameter.Add(P_MESSAGE);
                parameter.Add(P_COD_ERR);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                entity.observation = P_MESSAGE.Value.ToString();
                entity.lockMark = Convert.ToInt32(P_COD_ERR.Value.ToString());
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                LogControl.save("ValidateLock", ex.ToString(), "3");
            }
            return entity;
        }

        public EntityValidateDebt ValidateDebt(ParamsValidateDebt param)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            EntityValidateDebt entity = new EntityValidateDebt();
            string storedProcedureName = ProcedureName.pkg_EstadoCiente + ".VALIDAR_DEUDA";
            try
            {
                //INPUT

                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, param.branchCode, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, param.productCode, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, param.clientCode, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTRANSAC", OracleDbType.Int32, param.transactionCode, ParameterDirection.Input));
                //OUTPUT
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, ParameterDirection.Output);

                P_MESSAGE.Size = 9000;
                P_COD_ERR.Size = 9000;

                parameter.Add(P_MESSAGE);
                parameter.Add(P_COD_ERR);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                entity.observation = P_MESSAGE.Value.ToString();
                entity.lockMark = Convert.ToInt32(P_COD_ERR.Value.ToString());

                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                LogControl.save("ValidateDebt", ex.ToString(), "3");
            }
            return entity;
        }

        public ResponseAccountStatusVM generateAccountStatus(ParamsGenerateAccountStatus param)
        {
            ResponseAccountStatusVM response = new ResponseAccountStatusVM();
            GenericView genericView = new GenericView();
            List<PrintPathsVM> path = new PrintPolicyDA().GetPathsPrint(param.documentCode);
            genericView.baseAccountStatusVM = new BaseGenerateAccountStatusVM();
            try
            {
                List<OracleParameter> parameter = new List<OracleParameter>
                {
                    new OracleParameter("P_NBRANCH", OracleDbType.Int32, param.branchCode, ParameterDirection.Input),
                    new OracleParameter("P_NPRODUCT", OracleDbType.Int32, param.productCode, ParameterDirection.Input),
                    new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, param.clientCode, ParameterDirection.Input),
                    new OracleParameter("C_CLIENT", OracleDbType.RefCursor, ParameterDirection.Output),
                    new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output)
                };

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(ProcedureName.pkg_EstadoCiente + ".REA_ESTADO_CUENTA", parameter))
                {
                    genericView.baseAccountStatusVM = dr.ReadRowsList<BaseGenerateAccountStatusVM>()[0];

                    dr.NextResult();

                    foreach (var item in dr.ReadRowsList<GenerateAccountStatusVM>())
                    {
                        genericView.baseAccountStatusVM.clientList.Add(item);
                    }
                    ELog.CloseConnection(dr);
                }

                PrintPolicyService documentService = new PrintPolicyService();
                response = documentService.generateDocument(param, path[0], genericView);
            }
            catch (Exception ex)
            {
                response.responseCode = 1;
                response.message = ex.ToString();
                LogControl.save("generateAccountStatus", ex.ToString(), "3");
            }

            return response;
        }

        public EntityValidateDebt ValidateDebtCot(ParamsValidateDebt param)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            EntityValidateDebt entity = new EntityValidateDebt();
            string storedProcedureName = ProcedureName.pkg_EstadoCiente + ".VALIDAR_DEUDA_COT";
            try
            {
                //INPUT

                parameter.Add(new OracleParameter("P_COTIZACION", OracleDbType.Int32, param.nidCotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTRANSAC", OracleDbType.Int32, param.transactionCode, ParameterDirection.Input));
                //OUTPUT
                OracleParameter P_SCLIENT = new OracleParameter("R_SCLIENT", OracleDbType.Varchar2, ParameterDirection.Output);
                OracleParameter P_NPRODUCT = new OracleParameter("R_NPRODUCT", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_NBRANCH = new OracleParameter("R_NBRANCH", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, ParameterDirection.Output);
                P_SCLIENT.Size = 9000;
                P_NPRODUCT.Size = 9000;
                P_NBRANCH.Size = 9000;
                P_MESSAGE.Size = 9000;
                P_COD_ERR.Size = 9000;
                parameter.Add(P_SCLIENT);
                parameter.Add(P_NPRODUCT);
                parameter.Add(P_NBRANCH);
                parameter.Add(P_MESSAGE);
                parameter.Add(P_COD_ERR);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                entity.observation = P_MESSAGE.Value.ToString();
                entity.lockMark = Convert.ToInt32(P_COD_ERR.Value.ToString());
                entity.idclient = P_SCLIENT.Value.ToString().Trim();
                entity.idbranch = Convert.ToInt32(P_NBRANCH.Value.ToString());
                entity.idproduct = Convert.ToInt32(P_NPRODUCT.Value.ToString());

                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("ValidateDebtCot", ex.ToString(), "3");
            }

            return entity;
        }

        public EntityValidateDebt ValidateDebtPolicy(ParamsValidateDebt param)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            EntityValidateDebt entity = new EntityValidateDebt();
            string storedProcedureName = ProcedureName.pkg_EstadoCiente + ".REA_DEUDA_PENDIENTE";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, param.branchCode, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, param.productCode, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int64, param.nidPolicy, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, param.clientCode, ParameterDirection.Input));
                //OUTPUT
                OracleParameter P_NAMOUNT = new OracleParameter("P_NAMOUNT", OracleDbType.Double, ParameterDirection.Output);

                P_NAMOUNT.Size = 9000;

                parameter.Add(P_NAMOUNT);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                entity.amount = String.Format("{0:#,0.00}", Convert.ToDecimal(P_NAMOUNT.Value.ToString().Replace(',', '.')));

                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("ValidateDebtPolicy", ex.ToString(), "3");
            }

            return entity;
        }

        public EntityValidateDebt ValidateDebtExterno(ParamsValidateDebt param)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            EntityValidateDebt entity = new EntityValidateDebt();
            string storedProcedureName = ProcedureName.pkg_EstadoCiente + ".VALIDAR_DEUDA_EXTERNO";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, param.branchCode, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, param.productCode, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, param.clientCode, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NINTERMED", OracleDbType.Int64, param.nintermed, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTRANSAC", OracleDbType.Int32, param.transactionCode, ParameterDirection.Input));
                //OUTPUT
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_COD_EXT = new OracleParameter("P_COD_EXT", OracleDbType.Int32, ParameterDirection.Output);

                P_MESSAGE.Size = 9000;
                P_COD_ERR.Size = 9000;
                P_COD_EXT.Size = 9000;

                parameter.Add(P_MESSAGE);
                parameter.Add(P_COD_ERR);
                parameter.Add(P_COD_EXT);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                entity.observation = P_MESSAGE.Value.ToString();
                entity.lockMark = Convert.ToInt32(P_COD_ERR.Value.ToString());
                entity.external = Convert.ToInt32(P_COD_EXT.Value.ToString());

                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("ValidateDebtExterno", ex.ToString(), "3");
            }

            return entity;
        }

        public List<EntityReceiptBill> getReceiptBill(ParamsBill param)//obtiene los datos del comprobante
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<EntityReceiptBill> listEntity = new List<EntityReceiptBill>();
            string storedProcedureName = ProcedureName.pkg_RebillPackage + ".SP_SEL_BILL_RECEIPT";
            try
            {
                //INPUT
                int sunatDias = Convert.ToInt32(ELog.obtainConfig("sunatDias"));

                parameter.Add(new OracleParameter("P_NUMBILL", OracleDbType.Varchar2, param.Bill, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NSUNATDIAS", OracleDbType.Int32, sunatDias, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, param.nusercode , ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                if (odr != null)
                {
                    if (odr.HasRows)
                    {
                        while (odr.Read())
                        {
                            EntityReceiptBill item = new EntityReceiptBill();
                            item.dateBill = odr["DATEBILL"] == DBNull.Value ? string.Empty : Convert.ToDateTime(odr["DATEBILL"]).ToString("dd/MM/yyyy");
                            item.idBranch = odr["IDBRANCH"] == DBNull.Value ? string.Empty : odr["IDBRANCH"].ToString().Trim();
                            item.desBranch = odr["DESBRANCH"] == DBNull.Value ? string.Empty : odr["DESBRANCH"].ToString().Trim();
                            item.idPolicy = odr["IDPOLICY"] == DBNull.Value ? string.Empty : odr["IDPOLICY"].ToString().Trim();
                            item.idCertif = odr["IDCERTIF"] == DBNull.Value ? string.Empty : odr["IDCERTIF"].ToString().Trim();
                            item.dateBegin = odr["DATEBEGIN"] == DBNull.Value ? string.Empty : Convert.ToDateTime(odr["DATEBEGIN"]).ToString("dd/MM/yyyy");
                            item.dateEnd = odr["DATEEND"] == DBNull.Value ? string.Empty : Convert.ToDateTime(odr["DATEEND"]).ToString("dd/MM/yyyy");
                            item.numReceipt = odr["NUMRECEIPT"] == DBNull.Value ? string.Empty : odr["NUMRECEIPT"].ToString().Trim();
                            item.idStatusReceipt = odr["IDSTATUSRECEIPT"] == DBNull.Value ? string.Empty : odr["IDSTATUSRECEIPT"].ToString().Trim();
                            item.desStatusReceipt = odr["DESSTATUSRECEIPT"] == DBNull.Value ? string.Empty : odr["DESSTATUSRECEIPT"].ToString().Trim();
                            item.idIntermed = odr["IDINTERMED"] == DBNull.Value ? string.Empty : odr["IDINTERMED"].ToString().Trim();
                            item.desIntermed = odr["DESINTERMED"] == DBNull.Value ? string.Empty : odr["DESINTERMED"].ToString().Trim();
                            item.idClient = odr["IDCLIENT"] == DBNull.Value ? string.Empty : odr["IDCLIENT"].ToString().Trim();
                            item.nameclient = odr["NAMECLIENT"] == DBNull.Value ? string.Empty : odr["NAMECLIENT"].ToString().Trim();
                            item.numDocClient = odr["NUMDOCCLIENT"] == DBNull.Value ? string.Empty : odr["NUMDOCCLIENT"].ToString().Trim();
                            item.idTypeDocClient = odr["IDTYPEDOCCLIENT"] == DBNull.Value ? string.Empty : odr["IDTYPEDOCCLIENT"].ToString().Trim();
                            item.desTypeDocClient = odr["DESTYPEDOCCLIENT"] == DBNull.Value ? string.Empty : odr["DESTYPEDOCCLIENT"].ToString().Trim();
                            item.addressClient = odr["ADDRESSCLIENT"] == DBNull.Value ? string.Empty : odr["ADDRESSCLIENT"].ToString().Trim();
                            item.sKeyAddressClient = odr["SKEYADDRESS"] == DBNull.Value ? string.Empty : odr["SKEYADDRESS"].ToString().Trim();
                            item.sRecType = odr["SRECTYPE"] == DBNull.Value ? string.Empty : odr["SRECTYPE"].ToString().Trim();
                            item.amouCom = odr["AMOUCOM"] == DBNull.Value ? string.Empty : String.Format("{0:#,0.00}", odr["AMOUCOM"]);
                            item.amouPremiumT = odr["AMOUPREMIUMT"] == DBNull.Value ? string.Empty : String.Format("{0:#,0.00}", odr["AMOUPREMIUMT"]);
                            item.amouPremiumN = odr["AMOUPREMIUMN"] == DBNull.Value ? string.Empty : String.Format("{0:#,0.00}", odr["AMOUPREMIUMN"]);
                            item.amouCapital = odr["AMOUCAPITAL"] == DBNull.Value ? string.Empty : String.Format("{0:#,0.00}", odr["AMOUCAPITAL"]);
                            item.idCurrency = odr["IDCURRENCY"] == DBNull.Value ? string.Empty : odr["IDCURRENCY"].ToString().Trim();
                            item.desCurrency = odr["DESCURRENCY"] == DBNull.Value ? string.Empty : odr["DESCURRENCY"].ToString().Trim();
                            item.billAccoun = odr["BILLACCOUN"] == DBNull.Value ? string.Empty : odr["BILLACCOUN"].ToString().Trim();
                            item.newBill = odr["NEWBILL"] == DBNull.Value ? string.Empty : odr["NEWBILL"].ToString().Trim();

                            item.sBillType = odr["SBILLTYPE"] == DBNull.Value ? string.Empty : odr["SBILLTYPE"].ToString().Trim();
                            item.nInsur_Area = odr["NINSUR_AREA"] == DBNull.Value ? string.Empty : odr["NINSUR_AREA"].ToString().Trim();
                            item.nBillNum = odr["NBILLNUM"] == DBNull.Value ? string.Empty : odr["NBILLNUM"].ToString().Trim();
                            item.stRentas = odr["SRENTAS"] == DBNull.Value ? string.Empty : odr["SRENTAS"].ToString().Trim();
                            item.cantReceiptCob = odr["CANTRECEIPTCOB"] == DBNull.Value ? string.Empty : odr["CANTRECEIPTCOB"].ToString().Trim();

                            item.nBillStat = odr["NBILLSTAT"] == DBNull.Value ? string.Empty : odr["NBILLSTAT"].ToString().Trim();
                            item.desStateFac = odr["DESSTATEFAC"] == DBNull.Value ? string.Empty : odr["DESSTATEFAC"].ToString().Trim();
                            item.factSunat = odr["FACTSUNAT"] == DBNull.Value ? string.Empty : odr["FACTSUNAT"].ToString().Trim();
                            item.intProd = odr["INTPROD"] == DBNull.Value ? string.Empty : odr["INTPROD"].ToString().Trim();
                            item.intCobr = odr["INTCOBR"] == DBNull.Value ? string.Empty : odr["INTCOBR"].ToString().Trim();
                            item.igvBill = odr["IGVBILL"] == DBNull.Value ? string.Empty : String.Format("{0:#,0.00}", odr["IGVBILL"]);
                            item.actionRefac = odr["ACTIONREFAC"] == DBNull.Value ? string.Empty : odr["ACTIONREFAC"].ToString().Trim();
                            item.nProduct = odr["NPRODUCT"] == DBNull.Value ? string.Empty : odr["NPRODUCT"].ToString().Trim();
                            item.desProduct = odr["DESPRODUCT"] == DBNull.Value ? string.Empty : odr["DESPRODUCT"].ToString().Trim();

                            item.nCode = odr["NCODE"] == DBNull.Value ? 0 : Int32.Parse(odr["NCODE"].ToString());
                            item.sMessage = odr["SMESSAGE"] == DBNull.Value ? string.Empty : odr["SMESSAGE"].ToString().Trim();

                            listEntity.Add(item);
                        }
                    }
                }
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("getReceiptBill", ex.ToString(), "3");
            }
            return listEntity;
        }

        public ResponseVM InsertRebill(ParamsRebill entitie)//genera la refacturación
        {
            var cor = 0;
            string compro = "";
            ResponseVM response = new ResponseVM();
            RebillDA correo = new RebillDA();

            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = cn;

                    try
                    {
                        int sunatDias = Convert.ToInt32(ELog.obtainConfig("sunatDias"));

                        cmd.CommandText = string.Format("{0}.{1}", ProcedureName.pkg_RebillPackage, "SP_INS_REBILL");

                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("P_SBILLTYPE", OracleDbType.Varchar2).Value = entitie.SBILLTYPE.Trim();
                        cmd.Parameters.Add("P_NINSUR_AREA", OracleDbType.Int32).Value = Convert.ToInt32(entitie.NINSUR_AREA);
                        cmd.Parameters.Add("P_NBILLNUM", OracleDbType.Int64).Value = Convert.ToInt64(entitie.NBILLNUM);
                        cmd.Parameters.Add("P_SCLIENT", OracleDbType.Varchar2).Value = entitie.SCLIENT.Trim();
                        cmd.Parameters.Add("P_SCLIENT_NEW", OracleDbType.Varchar2).Value = entitie.SCLIENT_NEW.Trim();
                        cmd.Parameters.Add("P_NTYPCLIENTDOC", OracleDbType.Int32).Value = Convert.ToInt32(entitie.NTYPCLIENTDOC);
                        cmd.Parameters.Add("P_SCLINUMDOCU", OracleDbType.Varchar2).Value = entitie.SCLINUMDOCU.Trim();
                        cmd.Parameters.Add("P_BILLACCOUN", OracleDbType.Varchar2).Value = entitie.BILLACCOUN.Trim();
                        cmd.Parameters.Add("P_NUSERCODE", OracleDbType.Int32).Value = Convert.ToInt32(entitie.NUSERCODE);
                        cmd.Parameters.Add("P_NSUNATDIAS", OracleDbType.Int32).Value = sunatDias;
                        cmd.Parameters.Add("P_NTIPREF", OracleDbType.Int32).Value = entitie.NTIPREF;

                        //OUTPUT
                        
                        OracleParameter P_SMESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, string.Empty, ParameterDirection.Output);
                        OracleParameter P_NCODE = new OracleParameter("P_COD_ERR", OracleDbType.Int32, string.Empty, ParameterDirection.Output);
                        OracleParameter P_SCOMPROBANTE = new OracleParameter("P_SCOMPROBANTE", OracleDbType.Varchar2, string.Empty, ParameterDirection.Output);

                        P_NCODE.Size = 4000;
                        P_SMESSAGE.Size = 4000;
                        P_SCOMPROBANTE.Size = 4000;
                        cmd.Parameters.Add(P_SMESSAGE);
                        cmd.Parameters.Add(P_NCODE);
                        cmd.Parameters.Add(P_SCOMPROBANTE);

                        cn.Open();

                        cmd.ExecuteNonQuery();
                        response.P_NCODE = P_NCODE.Value.ToString();
                        response.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                        compro = P_SCOMPROBANTE.Value.ToString();
                        cn.Close();
                        cmd.Dispose();

                        if (response.P_NCODE == "5" || response.P_NCODE == "15")//evalua el envio de correo
                        {
                            cor = correo.EnviarCorreoRefact(compro, Convert.ToInt32(entitie.NUSERCODE));
                            if (cor == 0)
                            {//problemas con el envio del correo
                                response.P_NCODE = "1";//activa el mensaje de error en el front
                                response.P_SMESSAGE = P_SMESSAGE + ", problemas con el envio de correo";
                            }
                            else {
                                if (response.P_NCODE == "15")
                                {
                                    response.P_NCODE = "10";//activa el mensaje de correcto en periodo de suspención
                                }
                                else {
                                    response.P_NCODE = "0";//continua el flujo
                                }
                                
                            }
                            
                        }
                    }
                    catch (Exception ex)
                    {
                        LogControl.save("InsertRebill", ex.ToString(), "3");
                    }
                    finally
                    {
                        if (cn.State == ConnectionState.Open) cn.Close();
                    }
                }
            }
            return response;
        }

    }
}
