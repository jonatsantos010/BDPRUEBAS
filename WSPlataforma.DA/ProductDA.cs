using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.Entities.ProductModel.BindingModel;
using WSPlataforma.Entities.ProductModel.ViewModel;
using WSPlataforma.Util;

namespace WSPlataforma.DA
{
    public class ProductDA : ConnectionBase
    {
        public List<ProductUserVM> ProductByUser(ProductByUserBM request)
        {
            //var sPackageName = ProcedureName.pkg_UserCanal + ".SP_SEL_PRODUCTS_BY_USER";
            //List<OracleParameter> parameter = new List<OracleParameter>();
            List<ProductUserVM> ListProductByUser = new List<ProductUserVM>();

            //try
            //{
            //    //INPUT
            //    parameter.Add(new OracleParameter("P_NIDUSER", OracleDbType.Varchar2, request.P_NIDUSER, ParameterDirection.Input));
            //    //OUTPUT
            //    parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

            //    using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedure(sPackageName, parameter))
            //    {
            //        ListProductByUser = dr.ReadRowsList<ProductUserVM>();
            //        ELog.CloseConnection(dr);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    ELog.save(this, ex.ToString());
            //}


            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracle"].ToString()))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        OracleDataReader dr;

                        cmd.Connection = cn;
                        cmd.CommandText = ProcedureName.pkg_UserCanal + ".SP_SEL_PRODUCTS_BY_USER";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new OracleParameter("P_NIDUSER", OracleDbType.Varchar2, request.P_NIDUSER, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;

                        cn.Open();
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        if (dr.HasRows)
                        {
                            ListProductByUser = dr.ReadRowsList<ProductUserVM>();
                        }

                        dr.Close();

                        //response.GenericResponse = listTransacction;

                    }
                    catch (Exception ex)
                    {
                        LogControl.save("ProductByUser", ex.ToString(), "3");
                    }
                    finally
                    {
                        if (cn.State == ConnectionState.Open) cn.Close();
                    }
                }
            }

            return ListProductByUser;
        }

        public List<EpsVM> EpsKuntur(ProductByUserBM request)
        {
            var sPackageName = ProcedureName.pkg_IntegraSCTR + ".SEL_COMPANY_LNK";
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<EpsVM> ListEps = new List<EpsVM>();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("p_cod_user", OracleDbType.Int32, request.P_NIDUSER, ParameterDirection.Input));

                //OUTPUT
                OracleParameter p_cursor = new OracleParameter("p_cursor", OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter p_mensaje = new OracleParameter("p_mensaje", OracleDbType.Varchar2, ParameterDirection.Output);
                p_mensaje.Size = 4000;
                parameter.Add(p_cursor);
                parameter.Add(p_mensaje);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter);

                if (p_mensaje.Value.ToString() == "null")
                {
                    while (odr.Read())
                    {
                        EpsVM eps = new EpsVM();
                        eps.NCODE = odr["NCOMPANY_LNK"].ToString().Trim();
                        eps.SNAME = odr["SSHORT_DES"].ToString().Trim();
                        eps.SIMAGE = odr["SIMAGE_LNK"].ToString().Trim();
                        eps.STYPE = odr["STYPE_LNK"].ToString().Trim();
                        ListEps.Add(eps);
                    }
                }
                else
                {
                    ListEps = null;
                }

                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("EpsKuntur", ex.ToString(), "3");
            }

            return ListEps;
        }
    }
}
