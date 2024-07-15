using Newtonsoft.Json;
using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WSPlataforma.Entities.CommissionModel.BindingModel;
using WSPlataforma.Entities.MaintenanceModel.BindingModel;
using WSPlataforma.Entities.MaintenanceModel.ViewModel;
using WSPlataforma.Util;



namespace WSPlataforma.DA
{
    public class MaintenanceDA : ConnectionBase
    {
        public List<TransactionVM> GetTranscactionsByProduct(int nBranch, int nProduct)
        {
            List<OracleParameter> parameters = new List<OracleParameter>();
            List<TransactionVM> lista = new List<TransactionVM>();
            string procedure = ProcedureName.pkg_ConfigParametros + ".REA_TYPE_TRANSAC";

            try
            {
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int64, nBranch, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int64, nProduct, ParameterDirection.Input));
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);
                while (odr.Read())
                {
                    TransactionVM item = new TransactionVM();
                    item.NTYPE_TRANSAC = odr["NTYPE_TRANSAC"];
                    item.SDESCRIPT = odr["SDESCRIPT"];
                    lista.Add(item);
                }
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lista;
        }

        public List<ParameterVM> GetComboParametersByTransaction(int nBranch, int nProduct, int nTypeTransac, int nPerfil)
        {
            List<OracleParameter> parameters = new List<OracleParameter>();
            List<ParameterVM> lista = new List<ParameterVM>();
            string procedure = ProcedureName.pkg_ConfigParametros + ".REA_COMBO_PARAMETERS";

            try
            {
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int64, nBranch, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int64, nProduct, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Int64, nTypeTransac, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPERFIL", OracleDbType.Int64, nPerfil, ParameterDirection.Input));
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);
                while (odr.Read())
                {
                    ParameterVM item = new ParameterVM();
                    item.NPARAM = odr["NPARAM"];
                    item.SDESCRIPT = odr["SDESCRIPT"];
                    lista.Add(item);
                }
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lista;

        }

        public List<ParameterVM> GetParametersByTransaction(int nBranch, int nProduct, int nTypeTransac, int nPerfil, int nParam, string nGob)
        {
            List<OracleParameter> parameters = new List<OracleParameter>();
            List<ParameterVM> lista = new List<ParameterVM>();
            string procedure = ProcedureName.pkg_ConfigParametros + ".REA_PARAMETERS";

            var branchRetroactivity = new string[] { ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch"), ELog.obtainConfig("vidaLeyBranch") };

            try
            {
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int64, nBranch, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int64, nProduct, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Int64, nTypeTransac, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPERFIL", OracleDbType.Int64, nPerfil, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPARAM", OracleDbType.Int64, nParam, ParameterDirection.Input));
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);
                while (odr.Read())
                {
                    ParameterVM item = new ParameterVM();
                    item.NPARAM = odr["NPARAM"];
                    //item.SDESCRIPT = odr["SDESCRIPT"];
                    //item.SDESCRIPT_BRANCH = odr["SDESCRIPT_BRANCH"];
                    //item.SDESCRIPT_PRODUCT = odr["SDESCRIPT_PRODUCT"];
                    //item.SDESCRIPT_TRANSAC = odr["SDESCRIPT_TRANSAC"];
                    //item.SDESCRIPT_PERFIL = odr["SDESCRIPT_PERFIL"];
                    //item.NPERFIL = odr["NPERFIL"];
                    //item.NTYPE_TRANSAC = Convert.ToInt32(odr["NTYPE_TRANSAC"]);
                    //lista.Add(item);


                    if (item.NPARAM == 3 && branchRetroactivity.Contains(nBranch.ToString())) //RETROACTIVIDAD
                    {
                        if (nGob == "0")
                        {
                            for (int i = 0; i < 2; i++) // es 2 porque exiten 2 tipos de cuentas -> 1: Cliente Gobierno // 2: Cliente Privado
                            {
                                var itemTipoCuenta = new ParameterVM();
                                itemTipoCuenta.SDESCRIPT = odr["SDESCRIPT"];
                                itemTipoCuenta.SDESCRIPT_BRANCH = odr["SDESCRIPT_BRANCH"];
                                itemTipoCuenta.SDESCRIPT_PRODUCT = odr["SDESCRIPT_PRODUCT"];
                                itemTipoCuenta.SDESCRIPT_TRANSAC = odr["SDESCRIPT_TRANSAC"];
                                itemTipoCuenta.SDESCRIPT_PERFIL = odr["SDESCRIPT_PERFIL"];
                                itemTipoCuenta.NPERFIL = odr["NPERFIL"];
                                itemTipoCuenta.NTYPE_TRANSAC = Convert.ToInt32(odr["NTYPE_TRANSAC"]);
                                itemTipoCuenta.P_SISCLIENT_GBD = (i + 1).ToString();
                                itemTipoCuenta.NPARAM = odr["NPARAM"];
                                lista.Add(itemTipoCuenta);
                            }
                        }
                        else
                        {
                            item.SDESCRIPT = odr["SDESCRIPT"];
                            item.SDESCRIPT_BRANCH = odr["SDESCRIPT_BRANCH"];
                            item.SDESCRIPT_PRODUCT = odr["SDESCRIPT_PRODUCT"];
                            item.SDESCRIPT_TRANSAC = odr["SDESCRIPT_TRANSAC"];
                            item.SDESCRIPT_PERFIL = odr["SDESCRIPT_PERFIL"];
                            item.NPERFIL = odr["NPERFIL"];
                            item.NTYPE_TRANSAC = Convert.ToInt32(odr["NTYPE_TRANSAC"]);
                            item.P_SISCLIENT_GBD = nGob;
                            lista.Add(item);
                        }
                    }
                    else
                    {
                        item.SDESCRIPT = odr["SDESCRIPT"];
                        item.SDESCRIPT_BRANCH = odr["SDESCRIPT_BRANCH"];
                        item.SDESCRIPT_PRODUCT = odr["SDESCRIPT_PRODUCT"];
                        item.SDESCRIPT_TRANSAC = odr["SDESCRIPT_TRANSAC"];
                        item.SDESCRIPT_PERFIL = odr["SDESCRIPT_PERFIL"];
                        item.NPERFIL = odr["NPERFIL"];
                        item.NTYPE_TRANSAC = Convert.ToInt32(odr["NTYPE_TRANSAC"]);
                        item.P_SISCLIENT_GBD = "0";
                        lista.Add(item);
                    }
                }
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lista;
        }

        public GenericResponse InsertRma(RmaBM data)
        {
            List<OracleParameter> parameters = new List<OracleParameter>();
            GenericResponse response = new GenericResponse();
            string procedure = ProcedureName.pkg_ConfigParametros + ".INS_RMA";
            DbConnection DataConnection = ConnectionGet(enuTypeDataBase.OracleVTime);
            DbTransaction trx = null;
            try
            {
                DataConnection.Open();
                trx = DataConnection.BeginTransaction();
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Decimal, data.NBRANCH, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Decimal, data.NPRODUCT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPERFIL", OracleDbType.Decimal, data.NPERFIL, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Varchar2, data.NTYPE_TRANSAC, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_RMA", OracleDbType.Decimal, data.RMA, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_INICIO_VIG", OracleDbType.Date, Convert.ToDateTime(data.INICIO_VIGENCIA).ToString("dd/MM/yyyy"), ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_FIN_VIG", OracleDbType.Date, Convert.ToDateTime(data.FIN_VIGENCIA).ToString("dd/MM/yyyy"), ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_COMMENT", OracleDbType.Varchar2, data.COMENTARIO, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Varchar2, data.NUSERCODE, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, response.message, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Varchar2, response.code, ParameterDirection.Output);

                P_MESSAGE.Size = 9000;
                P_COD_ERR.Size = 9000;

                parameters.Add(P_MESSAGE);
                parameters.Add(P_COD_ERR);

                this.ExecuteByStoredProcedureVT_TRX(procedure, parameters, DataConnection, trx);
                response.message = P_MESSAGE.Value.ToString();
                response.code = Convert.ToInt32(P_COD_ERR.Value.ToString());
            }
            catch (Exception ex)
            {
                if (trx != null)
                {
                    trx.Rollback();
                    trx.Dispose();
                }
                response.message = ex.Message;
                response.code = 2;
            }
            finally
            {
                if (response.code == 0)
                {
                    trx.Commit();
                }
                else
                {
                    if (trx.Connection != null) trx.Rollback();
                }
                DataConnection.Close();
                trx.Dispose();
            }
            return response;
        }

        public List<RmaVM> GetRma(RmaVM rma)
        {
            List<OracleParameter> parameters = new List<OracleParameter>();
            List<RmaVM> lista = new List<RmaVM>();
            string procedure = ProcedureName.pkg_ConfigParametros + ".REA_RMA";

            try
            {
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int64, rma.NBRANCH, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int64, rma.NPRODUCT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPERFIL", OracleDbType.Int64, rma.NPERFIL, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Int64, rma.NTYPE_TRANSAC, ParameterDirection.Input));
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);
                while (odr.Read())
                {
                    RmaVM item = new RmaVM();
                    item.NUMERO = odr["NUMERO"];
                    item.ESTADO = odr["ESTADO"];
                    item.FECHA_MODIF = odr["FECHA_MODIF"].ToString();
                    item.RMA = odr["RMA"];
                    item.INICIO_VIG = odr["INICIO_VIG"].ToString();
                    item.FIN_VIG = odr["FIN_VIG"].ToString();
                    item.COMENTARIOS = odr["COMENTARIOS"];
                    item.USUARIO = odr["USUARIO"];
                    item.PERFIL = odr["PERFIL"];
                    lista.Add(item);
                }
                odr.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lista;
        }


        public List<PorcentajeRMV> GetPorcentajeRmv(PorcentajeRMV rma)
        {
            List<OracleParameter> parameters = new List<OracleParameter>();
            List<PorcentajeRMV> lista = new List<PorcentajeRMV>();
            string procedure = ProcedureName.pkg_ConfigParametros + ".REA_PORCENTAJE_RMV";

            try
            {
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int64, rma.NBRANCH, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Int64, rma.NTYPE_TRANSAC, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int64, rma.NMODULEC, ParameterDirection.Input));
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);
                while (odr.Read())
                {
                    PorcentajeRMV item = new PorcentajeRMV();
                    item.PORCENTAJE = Convert.ToDecimal(odr["MIN"]);
                    item.RAMO = odr["RAMO"].ToString();
                    item.TIPO_TRABAJADOR = odr["TIPO_TRAB"].ToString();
                    item.TIPO_TRANSACCION = odr["TIPO_TRANSACC"].ToString();
                    item.RMV = odr["RMV"].ToString();
                    item.NBRANCH = Convert.ToInt32(odr["NBRANCH"]);
                    item.NTYPE_TRANSAC = Convert.ToInt32(odr["NTYPE_TRANSAC"]);
                    item.NMODULEC = Convert.ToInt32(odr["NMODULEC"]);
                    item.PORCENTAJE_RMV = odr["PORCENTAJE_RMV"].ToString();
                    item.VALOR_PORCENTAJE = Convert.ToDecimal(odr["VALOR_PORCENTAJE"].ToString());
                    lista.Add(item);
                }
                odr.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lista;
        }

        public List<TypeRiskVM> GetTypeRiskVM(TypeRiskVM riesgo)
        {
            List<OracleParameter> parameters = new List<OracleParameter>();
            List<TypeRiskVM> lista = new List<TypeRiskVM>();
            string procedure = ProcedureName.pkg_ConfigParametros + ".REA_NMODULEC";

            try
            {
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int64, riesgo.NBRANCH, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int64, riesgo.NPRODUCT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);
                while (odr.Read())
                {
                    TypeRiskVM item = new TypeRiskVM();
                    item.NMODULEC = Convert.ToInt32(odr["NMODULEC"]);
                    item.SDESCRIPT = odr["SDESCRIPT"].ToString();
                    lista.Add(item);
                }
                odr.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lista;
        }

        public GenericResponse UpdatePorcentajeRMV(PorcentajeRMV riesgo)
        {
            List<OracleParameter> parameters = new List<OracleParameter>();
            GenericResponse response = new GenericResponse();
            string procedure = ProcedureName.pkg_ConfigParametros + ".UPD_PORCENTAJE_RMV";
            DbConnection DataConnection = ConnectionGet(enuTypeDataBase.OracleVTime);
            DbTransaction trx = null;
            try
            {
                DataConnection.Open();
                trx = DataConnection.BeginTransaction();
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int64, riesgo.NBRANCH, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Int64, riesgo.NTYPE_TRANSAC, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int64, riesgo.NMODULEC, ParameterDirection.Input));
                parameters.Add(new OracleParameter("PORCENTAJE", OracleDbType.Decimal, riesgo.PORCENTAJE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NUSER_CODE", OracleDbType.Int64, riesgo.NUSER_CODE, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, response.message, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Varchar2, response.code, ParameterDirection.Output);

                P_MESSAGE.Size = 9000;
                P_COD_ERR.Size = 9000;

                parameters.Add(P_MESSAGE);
                parameters.Add(P_COD_ERR);

                this.ExecuteByStoredProcedureVT_TRX(procedure, parameters, DataConnection, trx);
                response.message = P_MESSAGE.Value.ToString();
                response.code = Convert.ToInt32(P_COD_ERR.Value.ToString());
            }
            catch (Exception ex)
            {
                if (trx != null)
                {
                    trx.Rollback();
                    trx.Dispose();
                }
                response.message = ex.Message;
                response.code = 2;
            }
            finally
            {
                if (response.code == 0)
                {
                    trx.Commit();
                }
                else
                {
                    if (trx.Connection != null) trx.Rollback();
                }
                DataConnection.Close();
                trx.Dispose();
            }
            return response;
        }


        public List<PorcentajeRMV> GetHistorialPorcentajeRMV(PorcentajeRMV riesgo)
        {
            List<OracleParameter> parameters = new List<OracleParameter>();
            List<PorcentajeRMV> lista = new List<PorcentajeRMV>();
            string procedure = ProcedureName.pkg_ConfigParametros + ".REA_HISTORIAL_RMV";

            try
            {
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int64, riesgo.NBRANCH, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Int64, riesgo.NTYPE_TRANSAC, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int64, riesgo.NMODULEC, ParameterDirection.Input));
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);
                while (odr.Read())
                {
                    PorcentajeRMV item = new PorcentajeRMV();
                    item.INICIO_VIG = odr["DEFFECDATE"].ToString();
                    item.FIN_VIG = odr["DNULLDATE"] == null ? "" : odr["DNULLDATE"].ToString();
                    item.ESTADO = odr["ESTADO"].ToString();
                    item.PORCENTAJE = Convert.ToDecimal(odr["MIN"].ToString());
                    item.USUARIO = odr["USUARIO"].ToString();
                    lista.Add(item);
                }
                odr.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lista;
        }

        public List<ActivityVM> GetActivityList(ActivityBM data)
        {
            List<OracleParameter> parameters = new List<OracleParameter>();
            List<ActivityVM> lista = new List<ActivityVM>();
            string procedure = ProcedureName.pkg_ConfigParametros + ".REA_ACTIVITIES";

            try
            {
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);
                while (odr.Read())
                {
                    ActivityVM item = new ActivityVM();
                    item.SCOD_ACTIVITY_TEC = odr["SCOD_ACTIVITY_TEC"];
                    item.SDESCRIPT = odr["SDESCRIPT"];
                    item.CHK = false;
                    lista.Add(item);
                }
                odr.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lista;
        }

        public List<ActivityVM> GetActivityListMovement(ActivityBM data)
        {
            List<OracleParameter> parameters = new List<OracleParameter>();
            List<ActivityVM> lista = new List<ActivityVM>();
            string procedure = ProcedureName.pkg_ConfigParametros + ".REA_ACTIVITIES_MOVEMENT";

            try
            {
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int64, data.NBRANCH, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int64, data.NPRODUCT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPERFIL", OracleDbType.Int64, data.NPERFIL, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Int64, data.NTYPE_TRANSAC, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NMOVEMENT", OracleDbType.Int64, data.NMOVEMENT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);
                while (odr.Read())
                {
                    ActivityVM item = new ActivityVM();
                    item.SCOD_ACTIVITY_TEC = odr["SCOD_ACTIVITY_TEC"];
                    item.CHK = true;
                    item.TYPE = Convert.ToInt32(odr["NACT_MINA"]);
                    lista.Add(item);
                }
                odr.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lista;
        }


        public GenericResponse InsertActivities(ActivityParamsBM data)
        {
            List<OracleParameter> parameters = new List<OracleParameter>();
            GenericResponse response = new GenericResponse();
            string procedure = ProcedureName.pkg_ConfigParametros + ".INS_ACTIVITIES";
            DbConnection DataConnection = ConnectionGet(enuTypeDataBase.OracleVTime);
            DbTransaction trx = null;
            try
            {
                DataConnection.Open();
                trx = DataConnection.BeginTransaction();

                string P_JLIST = JsonConvert.SerializeObject(data.LIST_ACTIVITY);

                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int64, data.NBRANCH, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int64, data.NPRODUCT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_MINING", OracleDbType.Int64, data.MINING, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_COD_ACTIV", OracleDbType.Varchar2, data.SCOD_ACTIVITY, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_INICIO_VIGENCIA", OracleDbType.Date, Convert.ToDateTime(data.INICIO_VIGENCIA), ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_COMENTARIO", OracleDbType.Varchar2, data.COMENTARIO, ParameterDirection.Input));
                //
                parameters.Add(new OracleParameter("P_NUSER_CODE", OracleDbType.Varchar2, data.USUARIO, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPERFIL", OracleDbType.Varchar2, data.NPERFIL, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Varchar2, data.NTYPE_TRANSAC, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_JLIST", OracleDbType.Clob, P_JLIST, ParameterDirection.Input));
                //OUTPUT
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, response.message, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Varchar2, response.code, ParameterDirection.Output);

                P_MESSAGE.Size = 9000;
                P_COD_ERR.Size = 9000;

                parameters.Add(P_MESSAGE);
                parameters.Add(P_COD_ERR);

                this.ExecuteByStoredProcedureVT_TRX(procedure, parameters, DataConnection, trx);
                response.message = P_MESSAGE.Value.ToString();
                response.code = Convert.ToInt32(P_COD_ERR.Value.ToString());
            }
            catch (Exception ex)
            {
                if (trx != null) trx.Rollback();
                response.message = ex.Message;
                response.code = 2;
            }
            finally
            {
                if (response.code == 0)
                {
                    trx.Commit();
                }
                else
                {
                    if (trx.Connection != null) trx.Rollback();
                }
                DataConnection.Close();
                trx.Dispose();
            }
            return response;
        }

        public List<ActivityParamsBM> GetCombActivities(ActivityParamsBM data)
        {
            List<ActivityParamsBM> lista = new List<ActivityParamsBM>();

            List<OracleParameter> parameters = new List<OracleParameter>();
            string procedure = ProcedureName.pkg_ConfigParametros + ".REA_COMB_ACTIV";

            try
            {
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int64, data.NBRANCH, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int64, data.NPRODUCT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPERFIL", OracleDbType.Int64, data.NPERFIL, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Int64, data.NTYPE_TRANSAC, ParameterDirection.Input));
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);
                while (odr.Read())
                {
                    ActivityParamsBM item = new ActivityParamsBM();
                    item.NMOVEMENT = odr["NMOVEMENT"];
                    item.ESTADO = odr["ESTADO"];
                    item.FECHA_MODIF = Convert.ToDateTime(odr["FECHA_MODIF"]).ToString("dd/MM/yyyy hh:mm");
                    item.DEFFECDATE = odr["DEFFECDATE"];
                    item.SCOMMENT = odr["SCOMMENT"];
                    item.USUARIO = odr["USUARIO"];
                    item.PERFIL = odr["PERFIL"];
                    item.DFIN_VIGENCIA = odr["DFIN_VIGENCIA"];
                    lista.Add(item);
                }
                odr.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lista;
        }

        public void InsertCombActivityDet(ActivityParamsBM data)
        {
            List<OracleParameter> parameters = new List<OracleParameter>();
            string procedure = ProcedureName.pkg_ConfigParametros + ".INS_ACTIVITY_DET";

            try
            {
                parameters.Add(new OracleParameter("P_COD_ACTIV", OracleDbType.Int64, data.SCOD_ACTIVITY, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_MINING", OracleDbType.Int64, data.MINING, ParameterDirection.Input));

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public GenericResponse InsertRetroactivity(RetroactivityBM data)
        {
            List<OracleParameter> parameters = new List<OracleParameter>();
            GenericResponse response = new GenericResponse();
            string procedure = ProcedureName.pkg_ConfigParametros + ".INS_RETROACTIVITY";
            DbConnection DataConnection = ConnectionGet(enuTypeDataBase.OracleVTime);
            DbTransaction trx = null;
            try
            {
                DataConnection.Open();
                trx = DataConnection.BeginTransaction();

                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int64, data.NBRANCH, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int64, data.NPRODUCT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Int64, data.NTYPE_TRANSAC, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPERFIL", OracleDbType.Int64, data.NPERFIL, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NRETROACTIVIDAD", OracleDbType.Int64, data.NRETROACTIVIDAD, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NRETRO_DIAS_RETROACTIVOS", OracleDbType.Int64, data.NRETRO_DIAS_RETROACTIVOS, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NRETRO_DIAS_POSTERIORES", OracleDbType.Int64, data.NRETRO_DIAS_POSTERIORES, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SCOMMENT", OracleDbType.Varchar2, data.SCOMMENT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NVIGENCIA_COTIZACION", OracleDbType.Int64, data.NVIGENCIA_COTIZACION, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NVIG_DIAS", OracleDbType.Int64, data.NVIG_DIAS, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NUSER_CODE", OracleDbType.Int64, data.NUSER_CODE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DINI_VIGENCIA", OracleDbType.Date, data.INICIO_VIGENCIA, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SISCLIENT_GBD", OracleDbType.Int32, Convert.ToInt32(data.P_SISCLIENT_GBD), ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, response.message, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Varchar2, response.code, ParameterDirection.Output);

                P_MESSAGE.Size = 9000;
                P_COD_ERR.Size = 9000;

                parameters.Add(P_MESSAGE);
                parameters.Add(P_COD_ERR);

                this.ExecuteByStoredProcedureVT_TRX(procedure, parameters, DataConnection, trx);
                response.message = P_MESSAGE.Value.ToString();
                response.code = Convert.ToInt32(P_COD_ERR.Value.ToString());
            }
            catch (Exception ex)
            {
                if (trx != null)
                {
                    trx.Rollback();
                    trx.Dispose();
                }
                response.message = ex.Message;
                response.code = 2;
            }
            finally
            {
                if (response.code == 0)
                {
                    trx.Commit();
                }
                else
                {
                    if (trx.Connection != null) trx.Rollback();
                }
                DataConnection.Close();
                trx.Dispose();
            }
            return response;
        }

        public List<RetroactivityVM> GetProfiles(FiltersBM data)
        {
            List<RetroactivityVM> lista = new List<RetroactivityVM>();
            List<OracleParameter> parameters = new List<OracleParameter>();
            string procedure = ProcedureName.pkg_ConfigParametros + ".REA_PROFILES";

            try
            {
                parameters.Add(new OracleParameter("P_NPERFIL", OracleDbType.Int64, data.nPerfil, ParameterDirection.Input));
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);
                while (odr.Read())
                {
                    RetroactivityVM item = new RetroactivityVM();
                    item.NPERFIL = odr["NIDPROFILE"];
                    item.SDESCRIPT = odr["SNAME"];
                    lista.Add(item);
                }
                odr.Close();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lista;

        }

        public string getProfileProduct(FiltersBM data)
        {
            string profile = string.Empty;
            List<OracleParameter> parameters = new List<OracleParameter>();
            string procedure = ProcedureName.pkg_ConfigParametros + ".REA_PERFIL_PRODUCTO";

            try
            {
                parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, data.nUsercode, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT_CANAL", OracleDbType.Int32, data.nProduct, ParameterDirection.Input));

                OracleParameter nProfile = new OracleParameter("P_NPERFIL", OracleDbType.Int32, ParameterDirection.Output);
                nProfile.Size = 20;
                parameters.Add(nProfile);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);
                profile = nProfile.Value.ToString();
            }
            catch (Exception ex)
            {
                profile = "0";
            }
            return profile;
        }

        public List<RetroactivityVM> GetConfigDays()
        {
            List<RetroactivityVM> lista = new List<RetroactivityVM>();
            List<OracleParameter> parameters = new List<OracleParameter>();
            string procedure = ProcedureName.pkg_ConfigParametros + ".REA_CONFIG_DAYS";

            try
            {
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);
                while (odr.Read())
                {
                    RetroactivityVM item = new RetroactivityVM();
                    item.NCONFIG_DIAS = odr["NCONFIG_DIAS"];
                    item.SDESCRIPT = odr["SDESCRIPT"];
                    lista.Add(item);
                }
                odr.Close();

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return lista;
        }

        public List<RetroactivityVM> GetRetroactivityList(FiltersBM data)
        {
            List<RetroactivityVM> lista = new List<RetroactivityVM>();
            List<OracleParameter> parameters = new List<OracleParameter>();
            string procedure = ProcedureName.pkg_ConfigParametros + ".REA_RETROACTIVITY";

            try
            {
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int64, data.nBranch, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int64, data.nProduct, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPERFIL", OracleDbType.Int64, data.nPerfil, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Int64, data.nTypeTransac, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SISCLIENT_GBD", OracleDbType.Int32, Convert.ToInt32(data.nGob), ParameterDirection.Input)); //ED 2509

                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);
                while (odr.Read())
                {
                    RetroactivityVM item = new RetroactivityVM();
                    item.NUMERO = odr["NUMERO"];
                    item.ESTADO = odr["ESTADO"];
                    item.FECHA_MODIF = odr["FECHA_MODIF"];
                    item.SCOMMENT = odr["SCOMMENT"];
                    item.USUARIO = odr["USUARIO"];
                    item.PERFIL = odr["PERFIL"];
                    item.NRETROACTIVIDAD = Convert.ToInt32(odr["NRETROACTIVIDAD"]);
                    item.NRETRO_DIAS_RETROACTIVOS = Convert.ToInt32(odr["NRETRO_DIAS_RETROACTIVOS"]);
                    item.NRETRO_DIAS_POSTERIORES = Convert.ToInt32(odr["NRETRO_DIAS_POSTERIORES"]);
                    item.NVIGENCIA_COTIZACION = Convert.ToInt32(odr["NVIGENCIA_COTIZACION"]);
                    item.NVIG_DIAS = Convert.ToInt32(odr["NVIG_DIAS"]);
                    item.DINI_VIGENCIA = Convert.ToDateTime(odr["DINI_VIGENCIA"]).ToString("dd/MM/yyyy");
                    item.DFIN_VIGENCIA = Convert.ToString(odr["DFIN_VIGENCIA"]);
                    lista.Add(item);
                }
                odr.Close();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lista;


        }

        public string CalcFinVigencia(string inicioVigencia)
        {
            string finVigencia = null;
            List<OracleParameter> parameters = new List<OracleParameter>();
            string procedure = ProcedureName.pkg_ConfigParametros + ".CALC_FIN_VIGENCIA";

            try
            {
                parameters.Add(new OracleParameter("P_INICIO_VIGENCIA", OracleDbType.Date, Convert.ToDateTime(inicioVigencia), ParameterDirection.Input));
                var FIN_VIGENCIA = new OracleParameter("FIN_VIGENCIA", OracleDbType.Varchar2, ParameterDirection.Output);
                FIN_VIGENCIA.Size = 20;
                parameters.Add(FIN_VIGENCIA);
                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);
                finVigencia = FIN_VIGENCIA.Value.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return finVigencia;
        }

        public List<RetroactivityMessageVM> GetRetroactivityMessage(FiltersBM data)
        {
            List<RetroactivityMessageVM> lista = new List<RetroactivityMessageVM>();
            List<OracleParameter> parameters = new List<OracleParameter>();
            string procedure = ProcedureName.pkg_ConfigParametros + ".REA_RETROACTIVITY_MESSAGE";

            try
            {
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int64, data.nBranch, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int64, data.nProduct, ParameterDirection.Input));
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);
                while (odr.Read())
                {
                    RetroactivityMessageVM item = new RetroactivityMessageVM();
                    item.NBRANCH = Convert.ToInt32(odr["NBRANCH"]);
                    item.NPRODUCT = Convert.ToInt32(odr["NPRODUCT"]);
                    item.type = Convert.ToInt32(odr["NTYPE"]);
                    item.transaction = Convert.ToInt32(odr["NTYPE_TRANSAC"]);
                    item.retroactivity = Convert.ToInt32(odr["NTYPE_RETROACTIVITY"]);
                    item.message = Convert.ToString(odr["SMESSAGE"]);
                    lista.Add(item);
                }
                odr.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lista;
        }

        public GenericResponse getUserAccess(FiltersBM data)
        {
            List<OracleParameter> parameters = new List<OracleParameter>();
            GenericResponse response = new GenericResponse();
            string procedure = ProcedureName.pkg_ConfigParametros + ".REA_USERS_ACCESO";
            DbConnection DataConnection = ConnectionGet(enuTypeDataBase.OracleVTime);
            try
            {
                DataConnection.Open();

                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int64, data.nBranch, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int64, data.nProduct, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int64, data.nUsercode, ParameterDirection.Input));

                //OUTPUT                
                OracleParameter P_NACCESO = new OracleParameter("P_NACCESO", OracleDbType.Varchar2, response.code, ParameterDirection.Output);

                P_NACCESO.Size = 9000;

                parameters.Add(P_NACCESO);

                this.ExecuteByStoredProcedureVT_TRX(procedure, parameters, DataConnection);
                response.code = Convert.ToInt32(P_NACCESO.Value.ToString());
            }
            catch (Exception ex)
            {
                response.message = ex.Message;
                response.code = 2;
            }
            finally
            {
                DataConnection.Close();
            }
            return response;
        }
    }
}
