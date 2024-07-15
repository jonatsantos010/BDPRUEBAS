
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
//using WSPlataforma.Entities.CommissionModel.BindingModel;
using WSPlataforma.Entities.MaintenanceComisDifModel.BindingModel;
using WSPlataforma.Entities.MaintenanceComisDifModel.ViewModel;
using WSPlataforma.Util;



namespace WSPlataforma.DA
{
    public class MaintenanceComisDifDA : ConnectionBase
    {
        
        public List<SimpleModelVM> GetSAList()
        {
            var parameters = new List<OracleParameter>();
            var suggests = new List<SimpleModelVM>();
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_MaintenanceComissDif, "LIST_SA");

            try
            {
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var suggest = new SimpleModelVM();
                        suggest.Id = reader["NCALCAPITAL"] == DBNull.Value ? 0 : int.Parse(reader["NCALCAPITAL"].ToString());
                        suggest.Description = reader["SDESCRIPT"] == DBNull.Value ? string.Empty : reader["SDESCRIPT"].ToString();

                        suggests.Add(suggest);
                    }
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("GetSAList", ex.ToString(), "3");
                throw;
            }

            return suggests;
        }
        public List<SimpleModelVM> GetBranch()
        {
            var parameters = new List<OracleParameter>();
            var suggests = new List<SimpleModelVM>();
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_MaintenanceComissDif, "LIST_BRANCH");

            try
            {
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {//NBRANCH, SDESCRIPT
                        var suggest = new SimpleModelVM();
                        suggest.Id = reader["NBRANCH"] == DBNull.Value ? 0 : int.Parse(reader["NBRANCH"].ToString());
                        suggest.Description = reader["SDESCRIPT"] == DBNull.Value ? string.Empty : reader["SDESCRIPT"].ToString();

                        suggests.Add(suggest);
                    }
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("GetBranch", ex.ToString(), "3");
                throw;
            }

            return suggests;
        }
        public List<SimpleModelVM> GetProductsById(int nBranch)
        {
            var parameters = new List<OracleParameter>();
            var suggests = new List<SimpleModelVM>();
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_MaintenanceComissDif, "LIST_PRODUCT");

            try
            {
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int64, nBranch, ParameterDirection.Input));
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {//NBRANCH, SDESCRIPT
                        var suggest = new SimpleModelVM();
                        suggest.Id = reader["NPRODUCT"] == DBNull.Value ? 0 : int.Parse(reader["NPRODUCT"].ToString());
                        suggest.Description = reader["SDESCRIPT"] == DBNull.Value ? string.Empty : reader["SDESCRIPT"].ToString();

                        suggests.Add(suggest);
                    }
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("GetProductsById", ex.ToString(), "3");
                throw;
            }

            return suggests;
        }

        public List<SimpleModelVM> GetModulosXPol(long poliza, int ramo, int producto)
        {
            var parameters = new List<OracleParameter>();
            var suggests = new List<SimpleModelVM>();
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_MaintenanceComissDif, "LIST_MODULES");

            try
            {
                parameters.Add(new OracleParameter("P_NPOLICY", OracleDbType.Long, poliza, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, ramo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, producto, ParameterDirection.Input));
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {//NBRANCH, SDESCRIPT
                        var suggest = new SimpleModelVM();
                        suggest.Id = reader["NMODULEC"] == DBNull.Value ? 0 : int.Parse(reader["NMODULEC"].ToString());
                        suggest.Description = reader["SDESCRIPT"] == DBNull.Value ? string.Empty : reader["SDESCRIPT"].ToString();

                        suggests.Add(suggest);
                    }
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("GetModulosXPol", ex.ToString(), "3");
                throw;
            }

            return suggests;
        }

        public List<SimpleModelVM> GetRolesXPol(long poliza, int ramo, int producto)
        {
            var parameters = new List<OracleParameter>();
            var suggests = new List<SimpleModelVM>();
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_MaintenanceComissDif, "LIST_ROLES");

            try
            {
                parameters.Add(new OracleParameter("P_NPOLICY", OracleDbType.Long, poliza, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, ramo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, producto, ParameterDirection.Input));
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {//NBRANCH, SDESCRIPT
                        var suggest = new SimpleModelVM();
                        suggest.Id = reader["NROLE"] == DBNull.Value ? 0 : int.Parse(reader["NROLE"].ToString());
                        suggest.Description = reader["SDESCRIPT"] == DBNull.Value ? string.Empty : reader["SDESCRIPT"].ToString();

                        suggests.Add(suggest);
                    }
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("GetRolesXPol", ex.ToString(), "3");
                throw;
            }

            return suggests;
        }

        public List<SimpleModelVM> Habilitar(long NPOLIZA, int NOPCION)
        {
            var parameters = new List<OracleParameter>();
            var suggests = new List<SimpleModelVM>();
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_MaintenanceComissDif, NOPCION==1? "HABILITA_POLIZA": "INHABILITA_POLIZA");

            try
            {
                parameters.Add(new OracleParameter("P_NPOLICY", OracleDbType.Long, NPOLIZA, ParameterDirection.Input));
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {//NBRANCH, SDESCRIPT
                        var suggest = new SimpleModelVM();
                        suggest.Id = reader["NESTADO"] == DBNull.Value ? 0 : int.Parse(reader["NESTADO"].ToString());
                        suggest.Description = reader["SDESCRIPT"] == DBNull.Value ? string.Empty : reader["SDESCRIPT"].ToString();

                        suggests.Add(suggest);
                    }
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("Habilitar", ex.ToString(), "3");
                throw;
            }

            return suggests;
        }
        public List<SimpleModelVM> GetTypeComiDif(int nFilter)
        {
            var parameters = new List<OracleParameter>();
            var suggests = new List<SimpleModelVM>();
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_MaintenanceComissDif, "LIST_DIF_TYPE");

            try
            {
                if (nFilter == 0)
                {
                    parameters.Add(new OracleParameter("NDIF_TYPE", OracleDbType.Int32, null, ParameterDirection.Input));
                }
                else
                {
                    parameters.Add(new OracleParameter("NDIF_TYPE", OracleDbType.Int32, nFilter, ParameterDirection.Input));
                }
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {//NBRANCH, SDESCRIPT
                        var suggest = new SimpleModelVM();
                        suggest.Id = reader["NDIF_TYPE"] == DBNull.Value ? 0 : int.Parse(reader["NDIF_TYPE"].ToString());
                        suggest.Description = reader["SDESCRIPT"] == DBNull.Value ? string.Empty : reader["SDESCRIPT"].ToString();

                        suggests.Add(suggest);
                    }
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("GetTypeComiDif", ex.ToString(), "3");
                throw;
            }

            return suggests;
        }

        public List<SimpleModelVM> GetGroupByTypeComiDif(int nFilter)
        {
            var parameters = new List<OracleParameter>();
            var suggests = new List<SimpleModelVM>();
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_MaintenanceComissDif, "DIF_TYPE_GROUP");

            try
            {
                parameters.Add(new OracleParameter("NDIF_TYPE", OracleDbType.Int64, nFilter, ParameterDirection.Input));
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {//NBRANCH, SDESCRIPT
                        var suggest = new SimpleModelVM();
                        suggest.Id = reader["NFLAG_GRUPO"] == DBNull.Value ? 0 : int.Parse(reader["NFLAG_GRUPO"].ToString());
                        suggest.Description = reader["SDESCRIPT"] == DBNull.Value ? string.Empty : reader["SDESCRIPT"].ToString();

                        suggests.Add(suggest);
                    }
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("GetGroupByTypeComiDif", ex.ToString(), "3");
                throw;
            }

            return suggests;
        }
        
        public SimpleModelVM EliminaConfig(int nBranch, int nProduct, Int64 nPolicy, int nOrden)
        {
            var parameters = new List<OracleParameter>();
            var suggestL = new List<SimpleModelVM>();
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_MaintenanceComissDif, "DEL_CONFIG_X_POL_N_NORDEN");
            int cod = 0;
            string message="";
            try
            {
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, nBranch, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, nProduct, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int64, nPolicy, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NORDEN", OracleDbType.Int32, nOrden, ParameterDirection.Input));


                OracleParameter P_EST = new OracleParameter("P_EST", OracleDbType.Int32, 5);
                P_EST.Direction = ParameterDirection.Output;
                parameters.Add(P_EST);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, 100);
                P_MESSAGE.Direction = ParameterDirection.Output;
                parameters.Add(P_MESSAGE);

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                var suggest = new SimpleModelVM();

                suggest.Id = int.Parse(P_EST.Value.ToString());
                suggest.Description = P_MESSAGE.Value.ToString();
                /*
                if (!(reader.Equals(null))) { 
                    reader.Close();
                }
                */
                return suggest;


            }
            catch (Exception ex)
            {
                LogControl.save("ValidaPoliza", ex.ToString(), "3");
                throw;
            }
        }
        public List<SimpleModelVM> ValidaPoliza(int nBranch, int nProduct, Int64 nPolicy)
        {
            var parameters = new List<OracleParameter>();
            var suggestL = new List<SimpleModelVM>();
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_MaintenanceComissDif, "VALIDA_POLIZA");

            try
            {
                parameters.Add(new OracleParameter("NBRANCH", OracleDbType.Int32, nBranch, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NPRODUCT", OracleDbType.Int32, nProduct, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NPOLICY", OracleDbType.Int64, nPolicy, ParameterDirection.Input));
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {//NBRANCH, SDESCRIPT
                        var suggest = new SimpleModelVM();

                        suggest.Id = reader["NPOLICY"] == DBNull.Value ? 0 : int.Parse(reader["NPOLICY"].ToString());
                        suggest.Description = reader["NPOLICY"] == DBNull.Value ? "" : reader["DESCRIPCION"].ToString();
                        suggestL.Add(suggest);
                    }
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ValidaPoliza", ex.ToString(), "3");
                throw;
            }
            return suggestL;
        }
        public List<ComisDifVM> GetComisDifConfigs(int nBranch, int nProduct, Int64 nPolicy)
        {
            var parameters = new List<OracleParameter>();
            var suggestL = new List<ComisDifVM>();
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_MaintenanceComissDif, "LIST_COMMISION_DIF");

            try
            {
                parameters.Add(new OracleParameter("NBRANCH", OracleDbType.Int32, nBranch, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NPRODUCT", OracleDbType.Int32, nProduct, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NPOLICY", OracleDbType.Int64, nPolicy, ParameterDirection.Input));
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {//NBRANCH, SDESCRIPT
                        var suggest = new ComisDifVM();

                        suggest.NBRANCH = reader["NBRANCH"] == DBNull.Value ? 0 : int.Parse(reader["NBRANCH"].ToString());
                        suggest.NPRODUCT = reader["NPRODUCT"] == DBNull.Value ? 0 : int.Parse(reader["NPRODUCT"].ToString());
                        suggest.NPOLICY = reader["NPOLICY"] == DBNull.Value ? 0 : Int64.Parse(reader["NPOLICY"].ToString());
                        suggest.NTIPO = reader["NDIF_TYPE"] == DBNull.Value ? 0 : int.Parse(reader["NDIF_TYPE"].ToString());
                        suggest.STIPO = reader["STIPO"] == DBNull.Value ? string.Empty : reader["STIPO"].ToString();
                        suggest.NGRUPO = reader["NFLAG_GRUPO"] == DBNull.Value ? 0 : int.Parse(reader["NFLAG_GRUPO"].ToString());
                        suggest.SGRUPO = reader["SGRUPO"] == DBNull.Value ? string.Empty : reader["SGRUPO"].ToString();
                        suggest.DNULLDATE = reader["DNULLDATE"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["DNULLDATE"].ToString());

                        suggest.SESTADO = reader["ESTADO"] == DBNull.Value ? string.Empty : reader["ESTADO"].ToString();
                        suggest.NMODULO = reader["NMODULEC"] == DBNull.Value ? 0 : int.Parse(reader["NMODULEC"].ToString());
                        suggest.DINIVIG = reader["FECHA_INI_VIG"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["FECHA_INI_VIG"].ToString());
                        suggest.DFINVIG = reader["FECHA_FIN_VIG"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["FECHA_FIN_VIG"].ToString());
                        /*suggest.NINIVIG = reader["NINIVIG"] == DBNull.Value ? 0 : int.Parse(reader["NINIVIG"].ToString());
                        suggest.NFINVIG = reader["NFINVIG"] == DBNull.Value ? 0 : int.Parse(reader["NFINVIG"].ToString());*/
                        suggest.NTASACLIENTE = reader["TASA_NETA_CLI"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TASA_NETA_CLI"].ToString());
                        suggest.NTASACOMPANIA = reader["NRATE_COMPANY"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["NRATE_COMPANY"].ToString());

                        //suggest.NANIOFIN = reader["NYEAR_CONTR_FIN"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["NYEAR_CONTR_FIN"].ToString());

                        suggest.NPORCENTAJECANAL = reader["PORC_CANAL"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["PORC_CANAL"].ToString());
                        suggest.NPORCENTAJEBROKER = reader["PORC_BROKER"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["PORC_BROKER"].ToString());
                        suggest.NMONTOCANAL = reader["MONTO_CANAL"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["MONTO_CANAL"].ToString());
                        suggest.NMONTOBROKER = reader["MONTO_BROKER"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["MONTO_BROKER"].ToString());
                        suggest.NMINCREDITO = reader["NAMOUNT_CREDIT_INI"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["NAMOUNT_CREDIT_INI"].ToString());
                        suggest.NMAXCREDITO = reader["NAMOUNT_CREDIT_END"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["NAMOUNT_CREDIT_END"].ToString());
                        suggest.NINIVIG = reader["NYEAR_CONTR_INI"] == DBNull.Value ? 0 : int.Parse(reader["NYEAR_CONTR_INI"].ToString());
                        suggest.NFINVIG = reader["NYEAR_CONTR_FIN"] == DBNull.Value ? 0 : int.Parse(reader["NYEAR_CONTR_FIN"].ToString());
                        suggest.NSUMAASEG = reader["NIND_CAPITAL"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["NIND_CAPITAL"].ToString());
                        suggest.NEDADMIN = reader["NAGE_MIN"] == DBNull.Value ? 0 : int.Parse(reader["NAGE_MIN"].ToString());
                        suggest.NEDADMAX = reader["NAGE_MAX"] == DBNull.Value ? 0 : int.Parse(reader["NAGE_MAX"].ToString());
                        suggest.NORDEN = reader["NORDEN"] == DBNull.Value ? 0 : int.Parse(reader["NORDEN"].ToString());/**/
                        suggest.NROL = reader["NROLE"] == DBNull.Value ? 0 : int.Parse(reader["NROLE"].ToString());/**/

                        suggestL.Add(suggest);
                    }
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("GetComisDifConfigs", ex.ToString(), "3");
                throw;
            }

            return suggestL;
        }

        public List<SimpleModelVM> GetCampos(int ntipo, int ngrupo)
        {
            var parameters = new List<OracleParameter>();
            var suggests = new List<SimpleModelVM>();
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_MaintenanceComissDif, "DIF_LIST_FIELDS_CONFI");

            try
            {
                parameters.Add(new OracleParameter("NDIF_TYPE", OracleDbType.Int32, ntipo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NFLAG_GRUPO", OracleDbType.Int32, ngrupo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {//NBRANCH, SDESCRIPT
                        var suggest = new SimpleModelVM();
                        suggest.Id = reader["NDIF_TYPE"] == DBNull.Value ? 0 : int.Parse(reader["NDIF_TYPE"].ToString());
                        suggest.Description = reader["SCAMPO"] == DBNull.Value ? string.Empty : reader["SCAMPO"].ToString();

                        suggests.Add(suggest);
                    }
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("GetCampos", ex.ToString(), "3");
                throw;
            }

            return suggests;
        }

        public List<SimpleModelVM> SetListaComisiones(List<ComisDifVM> data, int ramo, int producto, int tipo, int grupo, long poliza, int NUSERCODE)
        {
            var suggests = new List<SimpleModelVM>();
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_MaintenanceComissDif, "ADD_COMMI_DIFF");
            int total = data.Count;
            var rtrn = new SimpleModelVM();

            try
            {
                for (int i = 0; i < total; i++)
                {
                    var parameters = new List<OracleParameter>();
                    parameters.Add(new OracleParameter("NDIF_TYPE", OracleDbType.Int32, tipo, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("NFLAG_GRUPO", OracleDbType.Int32, grupo, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("NBRANCH", OracleDbType.Int32, ramo, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("NPRODUCT", OracleDbType.Int32, producto, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("NPOLICY", OracleDbType.Int64, poliza, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("NMODULEC", OracleDbType.Int32, data[i].NMODULO, ParameterDirection.Input));
                    /*if (data[i].DINIVIG.Value.Year == 1)
                    {
                        parameters.Add(new OracleParameter("DEFFECDATE", OracleDbType.Date, DBNull.Value, ParameterDirection.Input));
                    }
                    else
                    {*/
                        parameters.Add(new OracleParameter("DEFFECDATE", OracleDbType.Date, data[i].DINIVIG, ParameterDirection.Input));
                    /*}*/
                    /*if (data[i].DFINVIG.Value.Year == 1)
                    {
                        parameters.Add(new OracleParameter("DNULLDATE", OracleDbType.Date, DBNull.Value, ParameterDirection.Input));
                    }
                    else
                    {*/
                        parameters.Add(new OracleParameter("DNULLDATE", OracleDbType.Date, data[i].DFINVIG, ParameterDirection.Input));
                    /*}*/

                    parameters.Add(new OracleParameter("NIND_CAPITAL", OracleDbType.Int32, data[i].NSUMAASEG, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("NYEAR_CONTR_INI", OracleDbType.Int32, data[i].NINIVIG/*NANIOINI*/, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("NYEAR_CONTR_FIN", OracleDbType.Int32, data[i].NFINVIG, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("NRATE_CLIENT", OracleDbType.Decimal, data[i].NTASACLIENTE, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("NRATE_COMPANY", OracleDbType.Decimal, data[i].NTASACOMPANIA, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("NAMOUNT_CREDIT_INI", OracleDbType.Decimal, data[i].NMINCREDITO, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("NAMOUNT_CREDIT_END", OracleDbType.Decimal, data[i].NMAXCREDITO, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("NRATE_GROSS_UP", OracleDbType.Decimal, data[i].NPORCENTAJECANAL, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("NCOMMI_RATE", OracleDbType.Decimal, data[i].NPORCENTAJEBROKER, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("NROLE", OracleDbType.Int64, data[i].NROL, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("NAGE_MIN", OracleDbType.Int64, data[i].NAGE_MIN, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("NAGE_MAX", OracleDbType.Int64, data[i].NAGE_MAX, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("NPREMIUM_GROSS_UP", OracleDbType.Decimal, data[i].NMONTOCANAL, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("NPREMIUM_COMMI_RATE", OracleDbType.Decimal, data[i].NMONTOBROKER, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("NUSERCODE", OracleDbType.Int32, NUSERCODE, ParameterDirection.Input));

                    parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                    var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {//NBRANCH, SDESCRIPT
                            int Id = reader["ID"] == DBNull.Value ? 0 : int.Parse(reader["ID"].ToString());
                            string Description = reader["SDESCRIPT"] == DBNull.Value ? string.Empty : reader["SDESCRIPT"].ToString();
                            if (Id != 0)
                            {
                                rtrn.Id = Id;
                                rtrn.Description = Description;
                                suggests.Add(rtrn);
                                return suggests;
                            }
                        }
                    }
                    reader.Close();
                }


            }
            catch (Exception ex)
            {
                LogControl.save("SetListaComisiones", ex.ToString(), "3");
                //throw;
                rtrn.Id = -10;
                rtrn.Description = "error";
                suggests.Add(rtrn);
                return suggests;
            }
            rtrn.Id = 0;
            rtrn.Description = "ok";
            suggests.Add(rtrn);
            return suggests;
        }
        public List<SimpleModelVM> ActualizaComision(List<ComisDifVM> data, int ramo, int producto, int tipo, int grupo, long poliza, int NUSERCODE)
        {
            var suggests = new List<SimpleModelVM>();
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_MaintenanceComissDif, "UPD_COMMI_DIFF");
            int total = data.Count;
            var rtrn = new SimpleModelVM();

            try
            {
                for (int i = 0; i < total; i++)
                {
                    var parameters = new List<OracleParameter>();
                    parameters.Add(new OracleParameter("NDIF_TYPE", OracleDbType.Int32, tipo, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("NFLAG_GRUPO", OracleDbType.Int32, grupo, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("NBRANCH", OracleDbType.Int32, ramo, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("NPRODUCT", OracleDbType.Int32, producto, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("NPOLICY", OracleDbType.Int64, poliza, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("NMODULEC", OracleDbType.Int32, data[i].NMODULO, ParameterDirection.Input));

                    parameters.Add(new OracleParameter("DEFFECDATE", OracleDbType.Date, data[i].DINIVIG, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("DNULLDATE", OracleDbType.Date, data[i].DFINVIG, ParameterDirection.Input));

                    parameters.Add(new OracleParameter("NIND_CAPITAL", OracleDbType.Int32, data[i].NSUMAASEG, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("NYEAR_CONTR_INI", OracleDbType.Int32, data[i].NINIVIG/*NANIOINI*/, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("NYEAR_CONTR_FIN", OracleDbType.Int32, data[i].NFINVIG, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("NRATE_CLIENT", OracleDbType.Decimal, data[i].NTASACLIENTE, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("NRATE_COMPANY", OracleDbType.Decimal, data[i].NTASACOMPANIA, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("NAMOUNT_CREDIT_INI", OracleDbType.Decimal, data[i].NMINCREDITO, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("NAMOUNT_CREDIT_END", OracleDbType.Decimal, data[i].NMAXCREDITO, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("NRATE_GROSS_UP", OracleDbType.Decimal, data[i].NPORCENTAJECANAL, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("NCOMMI_RATE", OracleDbType.Decimal, data[i].NPORCENTAJEBROKER, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("NROLE", OracleDbType.Int64, data[i].NROL, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("NAGE_MIN", OracleDbType.Int64, data[i].NAGE_MIN, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("NAGE_MAX", OracleDbType.Int64, data[i].NAGE_MAX, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("NPREMIUM_GROSS_UP", OracleDbType.Decimal, data[i].NMONTOCANAL, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("NPREMIUM_COMMI_RATE", OracleDbType.Decimal, data[i].NMONTOBROKER, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("NORDEN", OracleDbType.Int32, data[i].NORDEN, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("NUSERCODE", OracleDbType.Int32, NUSERCODE, ParameterDirection.Input));

                    parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                    var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {//NBRANCH, SDESCRIPT
                            int Id = reader["ID"] == DBNull.Value ? 0 : int.Parse(reader["ID"].ToString());
                            string Description = reader["SDESCRIPT"] == DBNull.Value ? string.Empty : reader["SDESCRIPT"].ToString();
                            if (Id != 0)
                            {
                                rtrn.Id = Id;
                                rtrn.Description = Description;
                                suggests.Add(rtrn);
                                return suggests;
                            }
                        }
                    }
                    reader.Close();
                }


            }
            catch (Exception ex)
            {
                LogControl.save("ActualizaComision", ex.ToString(), "3");
                //throw;
                rtrn.Id = -10;
                rtrn.Description = "error";
                suggests.Add(rtrn);
                return suggests;
            }
            rtrn.Id = 0;
            rtrn.Description = "ok";
            suggests.Add(rtrn);
            return suggests;
        }
    }
}

