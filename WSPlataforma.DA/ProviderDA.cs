using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.Entities.ProviderModel.BindingModel;
using WSPlataforma.Entities.ProviderModel.ViewModel;
using WSPlataforma.Util;

namespace WSPlataforma.DA
{
    public class ProviderDA : ConnectionBase
    {
        public ResponseProvider getProviderList(ProviderBM request)
        {
            var sPackageName = ProcedureName.pkg_BDUClienteOtros + ".SP_LIST_PROVIDER";
            List<OracleParameter> parameter = new List<OracleParameter>();
            var response = new ResponseProvider();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_TYPE_SEARCH", OracleDbType.Int32, request.tipo_busqueda, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPCLIENTDOC", OracleDbType.Int32, request.tipo_documento, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCLINUMDOCU", OracleDbType.Varchar2, request.documento, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLEGALNAME ", OracleDbType.Varchar2, request.nombre_legal, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH ", OracleDbType.Int32, request.ramo, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SSTATREGT ", OracleDbType.Varchar2, request.estado, ParameterDirection.Input));
                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        var item = new Provider();

                        item.tipo_documento = odr["NTYPCLIENTDOC"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NTYPCLIENTDOC"].ToString());
                        item.documento = odr["SCLINUMDOCU"] == DBNull.Value ? string.Empty : odr["SCLINUMDOCU"].ToString();
                        item.nombre_legal = odr["SLEGALNAME"] == DBNull.Value ? string.Empty : odr["SLEGALNAME"].ToString();
                        item.cod_proveedor = odr["SCLIENT_PROVIDER"] == DBNull.Value ? string.Empty : odr["SCLIENT_PROVIDER"].ToString();
                        item.cod_tabla = odr["NID_TABLE"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NID_TABLE"].ToString());
                        item.cod_detalle = odr["NID_DETAIL"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NID_DETAIL"].ToString());
                        item.descripcionTp = odr["SDESCRIPT_TP"] == DBNull.Value ? string.Empty : odr["SDESCRIPT_TP"].ToString();
                        item.cod_voucher = odr["SBILLTYPE"] == DBNull.Value ? 0 : Convert.ToInt32(odr["SBILLTYPE"].ToString());
                        item.descripcionCp = odr["SDESCRIPT_CP"] == DBNull.Value ? string.Empty : odr["SDESCRIPT_CP"].ToString();
                        response.providerList.Add(item);
                    }
                }

                odr.Close();
                response.cod_error = response.providerList.Count > 0 ? 0 : 1;
                response.smessage = response.providerList.Count > 0 ? "Se ejecuto correctamente" : "No existen proveedores";
            }
            catch (Exception ex)
            {
                response.cod_error = 1;
                response.smessage = ex.ToString();
            }

            return response;
        }

        public ResponseProvider deltProvider(ProviderDelBM request)
        {
            var response = new ResponseProvider();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.pkg_BDUClienteOtros + ".SP_DEL_ALL_PROVIDER";

            try
            {   //INPUT
                parameter.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, request.cod_cliente, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, request.ramo, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, request.cod_usuario, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SSTATREGT", OracleDbType.Varchar2, request.estado, ParameterDirection.Input));


                //OUTPUT
                OracleParameter cod_error = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter smensage = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                cod_error.Size = 9000;
                smensage.Size = 9000;

                parameter.Add(cod_error);
                parameter.Add(smensage);

                this.ExecuteByStoredProcedureVT(storeprocedure, parameter);

                response.cod_error = cod_error.Value.ToString() == "null" ? 0 : Convert.ToInt32(cod_error.Value.ToString());
                response.smessage = smensage.Value.ToString();
            }
            catch (Exception ex)
            {
                response.cod_error = 1;
                response.smessage = ex.ToString();
                LogControl.save("deltProvider", ex.ToString(), "3");
            }

            return response;
        }


        public ResponseProvider updProvider(ProviderUpdBM request)
        {
            var response = new ResponseProvider();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.pkg_BDUClienteOtros + ".SP_UPD_PROVIDER";

            try
            {   //INPUT
                parameter.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, request.cod_cliente, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_TABLE", OracleDbType.Int32, request.cod_tabla, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_DETAIL", OracleDbType.Int32, request.cod_detalle, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SBILLTYPE", OracleDbType.Varchar2, request.cod_comprobante, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, request.cod_usuario, ParameterDirection.Input));



                //OUTPUT
                OracleParameter cod_error = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter smensage = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                cod_error.Size = 9000;
                smensage.Size = 9000;

                parameter.Add(cod_error);
                parameter.Add(smensage);

                this.ExecuteByStoredProcedureVT(storeprocedure, parameter);

                response.cod_error = cod_error.Value.ToString() == "null" ? 0 : Convert.ToInt32(cod_error.Value.ToString());
                response.smessage = smensage.Value.ToString();
            }
            catch (Exception ex)
            {
                response.cod_error = 1;
                response.smessage = ex.ToString();
                LogControl.save("updProvider", ex.ToString(), "3");
            }

            return response;
        }
        public ResponseProvider insProvider(ProviderInsBM request)
        {
            var response = new ResponseProvider();
            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.pkg_BDUClienteOtros + ".SP_INS_ALL_PROVIDER";

            try
            {   //INPUT
                parameter.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, request.cod_cliente, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, request.ramo, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_TABLE", OracleDbType.Int32, request.cod_tabla, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_DETAIL", OracleDbType.Int32, request.cod_detalle, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SBILLTYPE", OracleDbType.Varchar2, request.cod_comprobante, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, request.cod_usuario, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SSTATREGT", OracleDbType.Varchar2, request.estado, ParameterDirection.Input));


                //OUTPUT
                OracleParameter cod_error = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter smensage = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                cod_error.Size = 9000;
                smensage.Size = 9000;

                parameter.Add(cod_error);
                parameter.Add(smensage);

                this.ExecuteByStoredProcedureVT(storeprocedure, parameter);

                response.cod_error = cod_error.Value.ToString() == "null" ? 0 : Convert.ToInt32(cod_error.Value.ToString());
                response.smessage = smensage.Value.ToString();
            }
            catch (Exception ex)
            {
                response.cod_error = 1;
                response.smessage = ex.ToString();
                LogControl.save("insProvider", ex.ToString(), "3");
            }

            return response;
        }

        public List<TypeProvider> getTypeProviderList(int cod_tabla)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<TypeProvider> list = new List<TypeProvider>();

            var sPackageName = ProcedureName.pkg_BDUClienteOtros + ".SP_LIST_TYPE_PROVIDER";

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_TABLE", OracleDbType.Int32, cod_tabla, ParameterDirection.Input));
                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter);

                while (odr.Read())
                {

                    var item = new TypeProvider();

                    item.cod_tabla = odr["NID_TABLE"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NID_TABLE"].ToString());
                    item.detalle = odr["NID_DETAIL"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NID_DETAIL"].ToString());
                    item.descripcion = odr["SDESCRIPT"] == DBNull.Value ? string.Empty : odr["SDESCRIPT"].ToString();

                    list.Add(item);
                }
                odr.Close();

            }
            catch (Exception ex)
            {
                LogControl.save("getTypeProviderList", ex.ToString(), "3");
            }
            return list;
        }

        public List<TypeVoucher> getTypeVoucherList(int cod_voucher)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<TypeVoucher> list = new List<TypeVoucher>();

            var sPackageName = ProcedureName.pkg_BDUClienteOtros + ".SP_LIST_TYPE_VOUCHER";

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_SBILLTYPE", OracleDbType.Int32, cod_voucher, ParameterDirection.Input));
                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter);

                while (odr.Read())
                {

                    var item = new TypeVoucher();

                    item.cod_voucher = odr["SBILLTYPE"] == DBNull.Value ? 0 : Convert.ToInt32(odr["SBILLTYPE"].ToString());
                    item.descripcion = odr["SDESCRIPT"] == DBNull.Value ? string.Empty : odr["SDESCRIPT"].ToString();

                    list.Add(item);
                }
                odr.Close();

            }
            catch (Exception ex)
            {
                LogControl.save("getTypeVoucherList", ex.ToString(), "3");
            }
            return list;
        }

        public ResponseProvider getProviderListByQuotation(ProviderQuotationBM request)
        {
            var sPackageName = ProcedureName.pkg_Cotizacion + ".SP_LIST_PROVIDER";
            List<OracleParameter> parameter = new List<OracleParameter>();
            var response = new ResponseProvider();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, request.num_cotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH ", OracleDbType.Int32, request.ramo, ParameterDirection.Input));
                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        var item = new Provider();

                        item.tipo_documento = odr["NTYPCLIENTDOC"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NTYPCLIENTDOC"].ToString());
                        item.documento = odr["SCLINUMDOCU"] == DBNull.Value ? string.Empty : odr["SCLINUMDOCU"].ToString();
                        item.nombre_legal = odr["SLEGALNAME"] == DBNull.Value ? string.Empty : odr["SLEGALNAME"].ToString();
                        item.cod_proveedor = odr["SCLIENT_PROVIDER"] == DBNull.Value ? string.Empty : odr["SCLIENT_PROVIDER"].ToString();
                        item.cod_tabla = odr["NID_TABLE"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NID_TABLE"].ToString());
                        item.cod_detalle = odr["NID_DETAIL"] == DBNull.Value ? 0 : Convert.ToInt32(odr["NID_DETAIL"].ToString());
                        item.descripcionTp = odr["SDESCRIPT_TP"] == DBNull.Value ? string.Empty : odr["SDESCRIPT_TP"].ToString();
                        item.cod_voucher = odr["SBILLTYPE"] == DBNull.Value ? 0 : Convert.ToInt32(odr["SBILLTYPE"].ToString());
                        item.descripcionCp = odr["SDESCRIPT_CP"] == DBNull.Value ? string.Empty : odr["SDESCRIPT_CP"].ToString();
                        response.providerList.Add(item);
                    }
                }

                odr.Close();
                response.cod_error = response.providerList.Count > 0 ? 0 : 1;
                response.smessage = response.providerList.Count > 0 ? "Se ejecuto correctamente" : "No existen proveedores";
            }
            catch (Exception ex)
            {
                response.cod_error = 1;
                response.smessage = ex.ToString();
            }

            return response;
        }

    }
}
