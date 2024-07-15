
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
using WSPlataforma.Entities.MaintenancePlanesAsistModel.BindingModel;
using WSPlataforma.Entities.MaintenancePlanesAsistModel.ViewModel;
using WSPlataforma.Entities.PreliminaryModel.ViewModel;
using WSPlataforma.Entities.Reports.BindingModel.ReportEntities;
using WSPlataforma.Util;



namespace WSPlataforma.DA
{
    public class MaintenancePlanesAsistDA : ConnectionBase
    {

        public List<PlusListVM> GetServices()
        {
            var parameters = new List<OracleParameter>();
            var suggests = new List<PlusListVM>();
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_MaintenancePlanesAsist, "SP_LISTAR_SERVICE");

            try
            {
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {//NBRANCH, SDESCRIPT
                        var suggest = new PlusListVM();
                        suggest.Id = reader["NID_SERVICE"] == DBNull.Value ? 0 : int.Parse(reader["NID_SERVICE"].ToString());
                        suggest.Description = reader["SDESCRIPT"] == DBNull.Value ? string.Empty : reader["SDESCRIPT"].ToString();
                        suggest.Flag = reader["NFLAG_COVER"] == DBNull.Value ? 0 : int.Parse(reader["NFLAG_COVER"].ToString());

                        suggests.Add(suggest);
                    }
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("GetServices", ex.ToString(), "3");
                throw;
            }

            return suggests;
        }
        public List<ServicesVM> GetServicioXPlan(int nplan, int nusuario, int ramo, int producto, long poliza)
        {
            var parameters = new List<OracleParameter>();
            var suggests = new List<ServicesVM>();
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_MaintenancePlanesAsist, "SP_GET_SERVICES_BY_PLAN");

            try
            {
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, ramo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, producto, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int64, poliza, ParameterDirection.Input));

                parameters.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, nplan, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, nusuario, ParameterDirection.Input));
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                /*
                OracleParameter P_NPLAN = new OracleParameter("P_NPLAN", OracleDbType.Int32, 5);
                P_NPLAN.Direction = ParameterDirection.Output;
                parameters.Add(P_NPLAN);*/

                OracleParameter SUSER = new OracleParameter("P_SUSERCODE", OracleDbType.Varchar2, 100);
                SUSER.Direction = ParameterDirection.Output;
                parameters.Add(SUSER);


                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {//NBRANCH, SDESCRIPT
                        var suggest = new ServicesVM();
                        suggest.Id = reader["NMODULEC"] == DBNull.Value ? 0 : int.Parse(reader["NMODULEC"].ToString());
                        suggest.NombreServicio = reader["SDESC_SERVICE"] == DBNull.Value ? string.Empty : reader["SDESC_SERVICE"].ToString();
                        suggest.Descripcion = reader["SDESCRIPTION"] == DBNull.Value ? string.Empty : reader["SDESCRIPTION"].ToString();
                        suggest.Usuario = reader["SUSER"] == DBNull.Value ? string.Empty : reader["SUSER"].ToString(); //SUSER.Value.ToString();

                        suggests.Add(suggest);
                    }
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("GetServicioXPlan", ex.ToString(), "3");
                throw;
            }

            return suggests;
        }
        public List<SimpleModelVM> GetBranch()
        {
            var parameters = new List<OracleParameter>();
            var suggests = new List<SimpleModelVM>();
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_MaintenancePlanesAsist, "LIST_BRANCH");

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
        public SimpleModelVM GetPlanes(Int64 poliza, int ramo, int producto)
        {
            var parameters = new List<OracleParameter>();
            var suggests = new List<SimpleModelVM>();
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_MaintenancePlanesAsist, "SP_SIG_PLAN");

            try
            {
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, ramo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, producto, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int64, poliza, ParameterDirection.Input));
                OracleParameter P_NPLAN = new OracleParameter("P_NPLAN", OracleDbType.Int32, 5);
                P_NPLAN.Direction = ParameterDirection.Output;
                parameters.Add(P_NPLAN);

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                var suggest = new SimpleModelVM();
                if(P_NPLAN.Value != null){
                    suggest.Id = int.Parse(P_NPLAN.Value.ToString());
                }
                /*
                if (!(reader.Equals(null))) { 
                    reader.Close();
                }
                */
                return suggest;

            }
            catch (Exception ex)
            {
                LogControl.save("GetPlanes", ex.ToString(), "3");
                throw;
            }
        }
        public List<SimpleModelVM> GetProductsById(int nBranch)
        {
            var parameters = new List<OracleParameter>();
            var suggests = new List<SimpleModelVM>();
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_MaintenancePlanesAsist, "LIST_PRODUCT");

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

        public List<SimpleModelVM> GetCoverAdicional(int ramo, int producto)
        {
            var parameters = new List<OracleParameter>();
            var suggests = new List<SimpleModelVM>();
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_MaintenancePlanesAsist, "LIST_COVER_ADICIONAL");

            try
            {
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, ramo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, producto, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NMODULE", OracleDbType.Int32, null, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DFECHA", OracleDbType.Date, null, ParameterDirection.Input));
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {//NBRANCH, SDESCRIPT
                        var suggest = new SimpleModelVM();
                        suggest.Id = reader["NCOVER"] == DBNull.Value ? 0 : int.Parse(reader["NCOVER"].ToString());
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

        public List<SimpleModelVM> GetRoles()
        {
            var parameters = new List<OracleParameter>();
            var suggests = new List<SimpleModelVM>();
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_MaintenancePlanesAsist, "SP_LISTAR_ROLES");

            try
            {
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
                LogControl.save("GetRoles", ex.ToString(), "3");
                throw;
            }

            return suggests;
        }

        public List<SimpleModelVM> Habilitar(long NPOLIZA, int NORDEN, int NOPCION)
        {
            var parameters = new List<OracleParameter>();
            var suggests = new List<SimpleModelVM>();
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_MaintenancePlanesAsist, NOPCION==1? "HABILITA_POLIZA": "INHABILITA_POLIZA");

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
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_MaintenancePlanesAsist, "LIST_DIF_TYPE");

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
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_MaintenancePlanesAsist, "DIF_TYPE_GROUP");

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
        public List<PlanAsistVM> GetPlanesAsistencias(int nBranch, int nProduct, Int64 nPolicy, int ntipBusqueda, int nPlan)//INI <RQ2024-57 - 03/04/2024>
        {
            var parameters = new List<OracleParameter>();
            var suggestL = new List<PlanAsistVM>();
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_MaintenancePlanesAsist, "SP_LISTAR_PLAN");

            try
            {
                parameters.Add(new OracleParameter("NBRANCH", OracleDbType.Int32, nBranch, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NPRODUCT", OracleDbType.Int32, nProduct, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NPOLICY", OracleDbType.Int64, nPolicy, ParameterDirection.Input));

                parameters.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, nPlan, ParameterDirection.Input));//INI <RQ2024-57 - 03/04/2024>
                parameters.Add(new OracleParameter("P_NID_SERVICE", OracleDbType.Int32, null, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NROLE", OracleDbType.Int32, null, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NTIPBUSQUEDA", OracleDbType.Int32, ntipBusqueda, ParameterDirection.Input));

                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {//NBRANCH, SDESCRIPT
                        var suggest = new PlanAsistVM();
                        //tps.NBRANCH, T10.SSHORT_DES SBRANCH, tps.NPRODUCT, PM.SDESCRIPT SPRODUCT, NPOLICY, tps.NMODULEC,
                        //tps.NID_SERVICE, S.SDESC_SERVICE, NRATE_CLIENT, NRATE_COMPANY, NAMOUNT_CLIENT, NAMOUNT_COMPANY,
                        //tps.NROLE, T12.SSHORT_DES SROLE, tps.NSTATUS , NTYPE_VALUE_PLAN NTYPEPLAN, DECODE(NTYPE_VALUE_PLAN, 1, ' || '''PORCENTUAL''' || ', 2, ' || '''MONTO''' || ') STYPEPLAN ' || CHR(13);

                        suggest.NBRANCH = reader["NBRANCH"] == DBNull.Value ? 0 : int.Parse(reader["NBRANCH"].ToString());
                        suggest.SBRANCH = reader["SBRANCH"] == DBNull.Value ? string.Empty : reader["SBRANCH"].ToString();

                        suggest.NPRODUCT = reader["NPRODUCT"] == DBNull.Value ? 0 : int.Parse(reader["NPRODUCT"].ToString());
                        suggest.SPRODUCT = reader["SPRODUCT"] == DBNull.Value ? string.Empty : reader["SPRODUCT"].ToString();
                        suggest.NPOLICY = reader["NPOLICY"] == DBNull.Value ? 0 : Int64.Parse(reader["NPOLICY"].ToString());

                        suggest.NMODULO = reader["NMODULEC"] == DBNull.Value ? 0 : int.Parse(reader["NMODULEC"].ToString());
                        //suggest.SPLAN = reader["SDESC_PLAN"] == DBNull.Value ? string.Empty : reader["SDESC_PLAN"].ToString();
                        suggest.NSERVICE = reader["NID_SERVICE"] == DBNull.Value ? 0 : Int64.Parse(reader["NID_SERVICE"].ToString());

                        suggest.SSERVICE = reader["SDESC_SERVICE"] == DBNull.Value ? string.Empty : reader["SDESC_SERVICE"].ToString();

                        suggest.NTASACLIENTE = reader["NRATE_CLIENT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["NRATE_CLIENT"].ToString());
                        suggest.NTASACOMPANIA = reader["NRATE_COMPANY"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["NRATE_COMPANY"].ToString());

                        suggest.NAMOUNT_CLIENT = reader["NAMOUNT_CLIENT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["NAMOUNT_CLIENT"].ToString());
                        suggest.NAMOUNT_COMPANY = reader["NAMOUNT_COMPANY"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["NAMOUNT_COMPANY"].ToString());

                        suggest.NMINCREDITO = reader["NAMOUNT_CREDIT_INI"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["NAMOUNT_CREDIT_INI"].ToString());
                        suggest.NMAXCREDITO = reader["NAMOUNT_CREDIT_END"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["NAMOUNT_CREDIT_END"].ToString());

                        suggest.NROL = reader["NROLE"] == DBNull.Value ? 0 : int.Parse(reader["NROLE"].ToString());/**/
                        suggest.SROL = reader["SROLE"] == DBNull.Value ? string.Empty : reader["SROLE"].ToString();/**/

                        suggest.NSTATUS = reader["NSTATUS"] == DBNull.Value ? 0 : int.Parse(reader["NSTATUS"].ToString());/**/

                        suggest.NTYPEPLAN = reader["NTYPEPLAN"] == DBNull.Value ? 0 : int.Parse(reader["NTYPEPLAN"].ToString());/**/
                        suggest.STYPEPLAN = reader["STYPEPLAN"] == DBNull.Value ? string.Empty : reader["STYPEPLAN"].ToString();/**/

                        suggest.NTYPE_DIVISOR = reader["NTYPE_DIVISOR"] == DBNull.Value ? 0 : int.Parse(reader["NTYPE_DIVISOR"].ToString());


                        suggestL.Add(suggest);
                    }
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("GetPlanesAsistencias", ex.ToString(), "3");
                throw;
            }

            return suggestL;
        }

        public List<SimpleModelVM> GetCampos(int ntipo, int ngrupo)
        {
            var parameters = new List<OracleParameter>();
            var suggests = new List<SimpleModelVM>();
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_MaintenancePlanesAsist, "DIF_LIST_FIELDS_CONFI");

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

        public List<SimpleModelVM> SetListaPlanes(List<PlanAsistVM> data, int ramo, int producto, long poliza, long NUSERCODE)
        {
            var suggests = new List<SimpleModelVM>();
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_MaintenancePlanesAsist, "SP_INSERT_PLAN_SERVICE");
            int total = data.Count;
            var rtrn = new SimpleModelVM();

            try
            {
                for (int i = 0; i < total; i++)
                {
                    var parameters = new List<OracleParameter>();
                    parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, ramo, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, producto, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int64, poliza, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_NID_SERVICE", OracleDbType.Int32, data[i].NSERVICE, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, data[i].NMODULO, ParameterDirection.Input));

                    //parameters.Add(new OracleParameter("NIND_CAPITAL", OracleDbType.Int32, data[i].NSUMAASEG, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_NFLAG_PRINCIPAL", OracleDbType.Int32, DBNull.Value, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_NRATE_CLIENT", OracleDbType.Decimal, data[i].NTASACLIENTE, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_NRATE_COMPANY", OracleDbType.Decimal, data[i].NTASACOMPANIA, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_NAMOUNT_CLIENT", OracleDbType.Decimal, data[i].NAMOUNT_CLIENT, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_NAMOUNT_COMPANY", OracleDbType.Decimal, data[i].NAMOUNT_COMPANY, ParameterDirection.Input));

                    
                    //parameters.Add(new OracleParameter("DEFFECDATE", OracleDbType.Date, null, ParameterDirection.Input));
                    //parameters.Add(new OracleParameter("DNULLDATE", OracleDbType.Date, null, ParameterDirection.Input));

                    parameters.Add(new OracleParameter("P_NCOVER", OracleDbType.Int32, data[i].NCOVER, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_NTYPE_VALUE_PLAN", OracleDbType.Int32, data[i].NTYPEPLAN, ParameterDirection.Input));

                    parameters.Add(new OracleParameter("P_NROLE", OracleDbType.Int32, data[i].NROL, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int64, NUSERCODE, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_NAMOUNT_MIN_CRED", OracleDbType.Decimal, data[i].NMINCREDITO, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_NAMOUNT_MAX_CRED", OracleDbType.Decimal, data[i].NMAXCREDITO, ParameterDirection.Input));
                    //parameters.Add(new OracleParameter("P_NFLAG_COVER_CHANGE", OracleDbType.Int32, 1, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_NCOMMIT", OracleDbType.Int32, 1, ParameterDirection.Input));


                    OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 200);
                    P_SMESSAGE.Direction = ParameterDirection.Output;
                    parameters.Add(P_SMESSAGE);

                    OracleParameter P_NCODIGO = new OracleParameter("P_NCODIGO", OracleDbType.Int32, 5);
                    P_NCODIGO.Direction = ParameterDirection.Output;
                    parameters.Add(P_NCODIGO);

                    var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                    var suggest = new SimpleModelVM();
                    if (P_NCODIGO.Value != null)
                    {
                        suggest.Id = int.Parse(P_NCODIGO.Value.ToString());
                        suggest.Description = P_SMESSAGE.Value.ToString();
                    }
                    //suggests.Add(suggest);
                    /*
                    if (!(reader.Equals(null))) { 
                        reader.Close();
                    }
                    */

                    //reader.Close();
                }


            }
            catch (Exception ex)
            {
                LogControl.save("SetListaPlanes", ex.ToString(), "3");
                //throw;
                rtrn.Id = -10;
                rtrn.Description = "error";
                suggests.Add(rtrn);
                return suggests;
            }
            rtrn.Id = 0;
            rtrn.Description = "Proceso correcto";
            suggests.Add(rtrn);
            return suggests;
        }
        public List<SimpleModelVM> ActualizaPlan(List<PlanAsistVM> data, int ramo, int producto, long poliza, int NUSERCODE)
        {
            var suggests = new List<SimpleModelVM>();
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_MaintenancePlanesAsist, "SP_UPDATE_PLAN_SERVICE");
            int total = data.Count;
            var rtrn = new SimpleModelVM();

            try
            {
                for (int i = 0; i < total; i++)
                {
                    var parameters = new List<OracleParameter>();
                    parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, ramo, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, producto, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int64, poliza, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_NID_SERVICE", OracleDbType.Int32, data[i].NSERVICE, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, data[i].NMODULO, ParameterDirection.Input));

                    //parameters.Add(new OracleParameter("NIND_CAPITAL", OracleDbType.Int32, data[i].NSUMAASEG, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_NRATE_CLIENT", OracleDbType.Decimal, data[i].NTASACLIENTE, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_NRATE_COMPANY", OracleDbType.Decimal, data[i].NTASACOMPANIA, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_NAMOUNT_CLIENT", OracleDbType.Decimal, data[i].NAMOUNT_CLIENT, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_NAMOUNT_COMPANY", OracleDbType.Decimal, data[i].NAMOUNT_COMPANY, ParameterDirection.Input));

                    //parameters.Add(new OracleParameter("DEFFECDATE", OracleDbType.Date, null, ParameterDirection.Input));
                    //parameters.Add(new OracleParameter("DNULLDATE", OracleDbType.Date, null, ParameterDirection.Input));

                    parameters.Add(new OracleParameter("P_NCOVER", OracleDbType.Int32, data[i].NCOVER, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_NTYPE_VALUE_PLAN", OracleDbType.Int32, data[i].NTYPEPLAN, ParameterDirection.Input));

                    parameters.Add(new OracleParameter("P_NROLE", OracleDbType.Int64, data[i].NROL, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_NAMOUNT_MIN_CRED", OracleDbType.Decimal, data[i].NMINCREDITO, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_NAMOUNT_MAX_CRED", OracleDbType.Decimal, data[i].NMAXCREDITO, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_NFLAG_COVER_CHANGE", OracleDbType.Int32, 1, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_NCOMMIT", OracleDbType.Int32, 0, ParameterDirection.Input));
                    //parameters.Add(new OracleParameter("NUSERCODE", OracleDbType.Int64, NUSERCODE, ParameterDirection.Input));


                    OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2,100);
                    P_SMESSAGE.Direction = ParameterDirection.Output;
                    parameters.Add(P_SMESSAGE);

                    OracleParameter P_NCODIGO = new OracleParameter("P_NCODIGO", OracleDbType.Int32,5);
                    P_NCODIGO.Direction = ParameterDirection.Output;
                    parameters.Add(P_NCODIGO);

                    var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                    var suggest = new SimpleModelVM();
                    if (P_NCODIGO.Value != null)
                    {
                        suggest.Id = int.Parse(P_NCODIGO.Value.ToString());
                        suggest.Description = P_SMESSAGE.Value.ToString();
                    }
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
            rtrn.Description = "Proceso correcto";
            suggests.Add(rtrn);
            return suggests;
        }

        public List<SimpleModelVM> ValidaPoliza(int nBranch, Int64 nPolicy, int nProduct)
        {
            var parameters = new List<OracleParameter>();
            var suggestL = new List<SimpleModelVM>();
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_MaintenancePlanesAsist, "VALIDA_POLIZA");

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
                        suggest.Description = reader["NPOLICY"] == DBNull.Value ? "" : reader["NPOLICY"].ToString();
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

        public List<SimpleModelVM> GetTipBusquedaList() //INI <RQ2024-57 - 03/04/2024>
        {
            var parameters = new List<OracleParameter>();
            var suggests = new List<SimpleModelVM>();
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_MaintenancePlanesTasa, "LIST_TIPBUSQUEDA");

            try
            {
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {//NBRANCH, SDESCRIPT
                        var suggest = new SimpleModelVM();
                        suggest.Id = reader["NTIPO"] == DBNull.Value ? 0 : int.Parse(reader["NTIPO"].ToString());
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

        public List<SimpleModelVM> GetDivisorList() //INI <RQ2024-57 - 03/04/2024>
        {
            var parameters = new List<OracleParameter>();
            var suggests = new List<SimpleModelVM>();
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_MaintenancePlanesTasa, "SP_LISTAR_TIPO_DIVISOR");

            try
            {
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {//NBRANCH, SDESCRIPT
                        var suggest = new SimpleModelVM();
                        suggest.Id = reader["NTIPO"] == DBNull.Value ? 0 : int.Parse(reader["NTIPO"].ToString());
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

        public SimpleModelVM setListaTasa(PlanAsistVM data)
        {
            var suggests = new SimpleModelVM();
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_MaintenancePlanesTasa, "SP_INSERT_TASA");
            var rtrn = new SimpleModelVM();
            var suggest = new SimpleModelVM();

            try
            {

                    var parameters = new List<OracleParameter>();
                    parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.NBRANCH, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, data.NPRODUCT, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int64, data.NPOLICY, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, data.NMODULO, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_NROLE", OracleDbType.Int32, data.NROL, ParameterDirection.Input));

                    parameters.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Date, data.DINIVIG, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_DNULLDATE", OracleDbType.Date, data.DFINVIG, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_NAMOUNT_MIN_CRED", OracleDbType.Decimal, data.NMINCREDITO, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_NAMOUNT_MAX_CRED", OracleDbType.Decimal, data.NMAXCREDITO, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_NRATE_CLIENT", OracleDbType.Decimal, data.NTASACLIENTE, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_NRATE_COMPANY", OracleDbType.Decimal, data.NTASACOMPANIA, ParameterDirection.Input));

                    parameters.Add(new OracleParameter("P_NTYPE_DIVISOR", OracleDbType.Int32, data.NDIVISOR, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int64, data.NUSERCODE, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_NCOMMIT", OracleDbType.Int32, 1, ParameterDirection.Input));

                    OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 200);
                    P_SMESSAGE.Direction = ParameterDirection.Output;
                    parameters.Add(P_SMESSAGE);

                    OracleParameter P_NCODIGO = new OracleParameter("P_NCODIGO", OracleDbType.Int32, 5);
                    P_NCODIGO.Direction = ParameterDirection.Output;
                    parameters.Add(P_NCODIGO);

                    var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);
                    
                    if (P_NCODIGO.Value != null)
                    {
                        suggest.Id = int.Parse(P_NCODIGO.Value.ToString());
                        suggest.Description = P_SMESSAGE.Value.ToString();
                    }
                
            }
            catch (Exception ex)
            {
                LogControl.save("SetListaTasa", ex.ToString(), "3");
                //throw;
                rtrn.Id = -10;
                rtrn.Description = "error";
                return rtrn;
            }
            rtrn.Id = suggest.Id;
            rtrn.Description = suggest.Description;
            return rtrn;
        }

        public PlanAsistVM getEdiTasaPol(int nBranch, int nProduct, Int64 nPolicy, int nModulo, int nRol)//INI <RQ2024-57 - 03/04/2024>
        {
            var parameters = new List<OracleParameter>();
            PlanAsistVM suggestL = new PlanAsistVM();
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_MaintenancePlanesTasa, "SP_EDITASAPOL");

            try
            {
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, nBranch, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, nProduct, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int64, nPolicy, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, nModulo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NROLE", OracleDbType.Int32, nRol, ParameterDirection.Input));

                OracleParameter P_NCODIGO = new OracleParameter("P_NCODIGO", OracleDbType.Int32, 5);
                P_NCODIGO.Direction = ParameterDirection.Output;
                parameters.Add(P_NCODIGO);

                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 100);
                P_SMESSAGE.Direction = ParameterDirection.Output;
                parameters.Add(P_SMESSAGE);

                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                //PlanAsistVM suggestL = new PlanAsistVM();

                while (reader.Read())
                {

                    suggestL.NTASACLIENTE = reader["NRATE_CLIENT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["NRATE_CLIENT"].ToString());
                    suggestL.NTASACOMPANIA = reader["NRATE_COMPANY"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["NRATE_COMPANY"].ToString());
                    suggestL.NMINCREDITO = reader["NAMOUNT_CREDIT_INI"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["NAMOUNT_CREDIT_INI"].ToString());
                    suggestL.NMAXCREDITO = reader["NAMOUNT_CREDIT_END"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["NAMOUNT_CREDIT_END"].ToString());
                    suggestL.NMODULO = reader["NMODULEC"] == DBNull.Value ? 0 : int.Parse(reader["NMODULEC"].ToString());
                    suggestL.NROL = reader["NROLE"] == DBNull.Value ? 0 : int.Parse(reader["NROLE"].ToString());
                    suggestL.NTYPE_DIVISOR = reader["NTYPE_DIVISOR"] == DBNull.Value ? 0 : int.Parse(reader["NTYPE_DIVISOR"].ToString());
                    suggestL.DINIVIG = reader["DEFFECDATE"] == DBNull.Value ? Convert.ToDateTime("") : Convert.ToDateTime(reader["DEFFECDATE"].ToString());
                    suggestL.DFINVIG = reader["DCOMPDATE"] == DBNull.Value ? Convert.ToDateTime("") : Convert.ToDateTime(reader["DCOMPDATE"].ToString());

                }
                suggestL.Id = int.Parse(P_NCODIGO.Value.ToString());
                suggestL.Description = P_SMESSAGE.Value.ToString();

                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("GetPlanesAsistencias", ex.ToString(), "3");
                throw;
            }

            return suggestL;
        }

        public SimpleModelVM actualizarTasa(PlanAsistVM data)
        {
            var suggests = new SimpleModelVM();
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_MaintenancePlanesTasa, "SP_UPDATE_CONFIG_TASAS");
            var rtrn = new SimpleModelVM();
            var suggest = new SimpleModelVM();

            try
            {

                var parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.NBRANCH, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, data.NPRODUCT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int64, data.NPOLICY, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NMODULEC", OracleDbType.Int32, data.NMODULO, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NROLE", OracleDbType.Int32, data.NROL, ParameterDirection.Input));

                parameters.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Date, data.DINIVIG, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DNULLDATE", OracleDbType.Date, data.DFINVIG, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NAMOUNT_MIN_CRED", OracleDbType.Decimal, data.NMINCREDITO, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NAMOUNT_MAX_CRED", OracleDbType.Decimal, data.NMAXCREDITO, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NRATE_CLIENT", OracleDbType.Decimal, data.NTASACLIENTE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NRATE_COMPANY", OracleDbType.Decimal, data.NTASACOMPANIA, ParameterDirection.Input));

                parameters.Add(new OracleParameter("P_NTYPE_DIVISOR", OracleDbType.Int32, data.NDIVISOR, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int64, data.NUSERCODE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCOMMIT", OracleDbType.Int32, 1, ParameterDirection.Input));

                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 200);
                P_SMESSAGE.Direction = ParameterDirection.Output;
                parameters.Add(P_SMESSAGE);

                OracleParameter P_NCODIGO = new OracleParameter("P_NCODIGO", OracleDbType.Int32, 5);
                P_NCODIGO.Direction = ParameterDirection.Output;
                parameters.Add(P_NCODIGO);

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                if (P_NCODIGO.Value != null)
                {
                    suggest.Id = int.Parse(P_NCODIGO.Value.ToString());
                    suggest.Description = P_SMESSAGE.Value.ToString();
                }

            }
            catch (Exception ex)
            {
                LogControl.save("SetListaTasa", ex.ToString(), "3");
                //throw;
                rtrn.Id = -10;
                rtrn.Description = "error";
                return rtrn;
            }
            rtrn.Id = suggest.Id;
            rtrn.Description = suggest.Description;
            return rtrn;
        }
    }
}

