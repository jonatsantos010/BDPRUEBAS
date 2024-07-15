/*************************************************************************************************/
/*  NOMBRE              :   ReporteRebillDA.cs                                                   */
/*  DESCRIPCION         :   Capa CORE - Reporte Refacturación                                    */
/*  AUTOR               :   MATERIAGRIS - Francisco Aquiño Ramirez                               */
/*  FECHA               :   08-11-2023                                                           */
/*  VERSION             :   1.0                                                                  */
/*************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.Util;
using WSPlataforma.Entities.ReporteNotaCreditoModel.ViewModel;
using WSPlataforma.Entities.ReporteNotaCreditoModel.BindingModel;
using WSPlataforma.Entities.RebillModel;
using Oracle.DataAccess.Client;
using System.Data;

namespace WSPlataforma.DA
{
    public class ReporteRebillDA: ConnectionBase
    {
        private readonly string Package = "PKG_PV_REBILL";

        public List<BranchVM> listarRamo()
        {
            var parameters = new List<OracleParameter>();
            var listaRamo = new List<BranchVM>();
            var procedure = string.Format("{0}.{1}", this.Package, "SPS_BRANCHES");
            var branch = new BranchVM();

            try
            {
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                if (reader.HasRows)
                {

                    while (reader.Read())
                    {
                        branch = new BranchVM();
                        branch.nBranch = reader["NBRANCH"] == DBNull.Value ? 0 : int.Parse(reader["NBRANCH"].ToString());
                        branch.sDescript = reader["SBRANCH_DESCRIPT"] == DBNull.Value ? string.Empty : reader["SBRANCH_DESCRIPT"].ToString();

                        listaRamo.Add(branch);
                    }
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarRamo", ex.ToString(), "3");
                throw;
            }

            return listaRamo;
        }

        public int permisoUser(NuserCodeFilter data)
        {
            var parameters = new List<OracleParameter>();
            var procedure = string.Format("{0}.{1}", this.Package, "SPS_PERMISO_USER");
            var PCode = 0;

            try
            {
                //INPUT
                parameters.Add(new OracleParameter("P_SUSER", OracleDbType.NVarchar2, data.nusercode, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.nBranch, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, data.nProduct, ParameterDirection.Input));

                var P_EST = new OracleParameter("P_NCODE", OracleDbType.Int32, null, ParameterDirection.Output);
                parameters.Add(P_EST);


                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters))
                {

                    PCode = Convert.ToInt32(P_EST.Value.ToString());


                }

                return PCode;

            }
            catch (Exception ex)
            {
                LogControl.save("permisoUser", ex.ToString(), "3");
                throw;
            }

        }

        public List<ProductVM> tipoDocumento()
        {
            var parameters = new List<OracleParameter>();
            var listaRamo = new List<ProductVM>();
            var procedure = string.Format("{0}.{1}", this.Package, "SPS_TIPDOCU");
            var branch = new ProductVM();

            try
            {
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                if (reader.HasRows)
                {

                    while (reader.Read())
                    {
                        branch = new ProductVM();
                        branch.nParameter = reader["NPARAMETER"] == DBNull.Value ? 0 : int.Parse(reader["NPARAMETER"].ToString());
                        branch.sDes_para = reader["SDES_PARA"] == DBNull.Value ? string.Empty : reader["SDES_PARA"].ToString();

                        listaRamo.Add(branch);
                    }
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarRamo", ex.ToString(), "3");
                throw;
            }

            return listaRamo;
        }
        public List<ProductVM> listarProducto(ProductoFilter data)
        {
            var sPackageName = string.Format("{0}.{1}", this.Package, "SPS_LISTPRODUCT");
            ProductVM entities = null;
            List<ProductVM> ListProductoPay = new List<ProductVM>();
            List<OracleParameter> parameters = new List<OracleParameter>();
            long _idRamo = data.nBranch;

            parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int16, data.nBranch, ParameterDirection.Input));
            parameters.Add(new OracleParameter("C_LISTA", OracleDbType.RefCursor, ParameterDirection.Output));

            using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameters))
            {
                while (dr.Read())
                {
                    entities = new ProductVM();
                    entities.nParameter = Convert.ToInt32(dr["NPARAMETER"]);
                    entities.sDes_para = Convert.ToString(dr["SDES_PARA"]);
                    ListProductoPay.Add(entities);
                }
            }
            return ListProductoPay;
        }

        public List<ListarRefactVM> listarRefact(ListarNotaCreditoFilter data)
        {
            var sPackageName = string.Format("{0}.{1}", this.Package, "SP_REPORT_REFACTURACION");
            List<OracleParameter> parameters = new List<OracleParameter>();

            ListarRefactVM entities = new ListarRefactVM();
            List<ListarRefactVM> listaReportesListado = new List<ListarRefactVM>();

            string endDateStr = data.endDate.ToString("dd/MM/yyyy");
            string initDateStr = data.startDate.ToString("dd/MM/yyyy");
            entities.lista = new List<ListarRefactVM.P_TABLE>();

            var nRamo = 0;

            if (data.nBranch == 999)
            {
                nRamo = data.nParameter;
            }
            else
            {
                nRamo = data.nBranch;
            }

            try
            {
                parameters.Add(new OracleParameter("P_BRANCH", OracleDbType.Int32, nRamo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_PRODUCT", OracleDbType.Int32, data.nProduct, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPOLICY", OracleDbType.Long, data.idPoliza, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NUMDOCU", OracleDbType.NVarchar2, data.idComprobante, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NTYPEDOCUMENT", OracleDbType.Int32, data.nTipDocu, ParameterDirection.Input));//TIPO DE DOCUMENTO
                parameters.Add(new OracleParameter("P_SCLIDOCUMENT", OracleDbType.NVarchar2, data.idCliDocument, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_INITDATE", OracleDbType.NVarchar2, initDateStr, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_ENDDATE", OracleDbType.NVarchar2, endDateStr, ParameterDirection.Input));

                var P_MENSAGE = new OracleParameter("P_MENSAGE", OracleDbType.NVarchar2, 40000, null, ParameterDirection.Output);
                var P_EST = new OracleParameter("P_EST", OracleDbType.Int32, null, ParameterDirection.Output);
                parameters.Add(P_MENSAGE);
                parameters.Add(P_EST);
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameters))
                {
                    while (dr.Read())
                    {
                        //entities = new ListarNotaCreditoVM();
                        ListarRefactVM.P_TABLE suggest = new ListarRefactVM.P_TABLE();

                        suggest.RAMO = dr["RAMO"] == DBNull.Value ? string.Empty : dr["RAMO"].ToString();
                        suggest.PRODUCTO = dr["PRODUCTO"] == DBNull.Value ? string.Empty : dr["PRODUCTO"].ToString();
                        suggest.NPOLICY = dr["POLIZA"] == DBNull.Value ? string.Empty : dr["POLIZA"].ToString();
                        suggest.COMPROBANTE_NUEVO = dr["COMPROBANTE_NUEVO"] == DBNull.Value ? string.Empty : dr["COMPROBANTE_NUEVO"].ToString();
                        suggest.FECHA_COMPROBANTE_NUEVO = dr["FECHA_COMPROBANTE_NUEVO"] == DBNull.Value ? string.Empty : dr["FECHA_COMPROBANTE_NUEVO"].ToString();
                        suggest.SCLIENAME = dr["CLIENTE"] == DBNull.Value ? string.Empty : dr["CLIENTE"].ToString();
                        suggest.COMPROBANTE_ORIGEN = dr["COMPROBANTE_ORIGEN"] == DBNull.Value ? string.Empty : dr["COMPROBANTE_ORIGEN"].ToString();
                        suggest.FECHA_COMPROBANTE_ORIGEN = dr["FECHA_COMPROBANTE_ORIGEN"] == DBNull.Value ? string.Empty : dr["FECHA_COMPROBANTE_ORIGEN"].ToString();
                        suggest.MONTO_COMPROBANTE = dr["MONTO_COMPROBANTE"] == DBNull.Value ? 0 : Decimal.Parse(dr["MONTO_COMPROBANTE"].ToString());
                        suggest.NOTA_CREDITO = dr["NOTA_CREDITO"] == DBNull.Value ? string.Empty : dr["NOTA_CREDITO"].ToString();
                        suggest.USUARIO = dr["USUARIO"] == DBNull.Value ? string.Empty : dr["USUARIO"].ToString();
                        suggest.FECHA_REFACTURACION = dr["FECHA_REFACTURACION"] == DBNull.Value ? string.Empty : dr["FECHA_REFACTURACION"].ToString();
                        suggest.MOTIVO = dr["MOTIVO"] == DBNull.Value ? string.Empty : dr["MOTIVO"].ToString();


                        //Montos a dos decimales
                        suggest.MONTO_COMPROBANTE = Convert.ToDecimal(String.Format("{0:0.00}", suggest.MONTO_COMPROBANTE));


                        //formato de fechas a dd/MM/yyyy
                        if (suggest.FECHA_COMPROBANTE_NUEVO != "")
                        {
                            DateTime fechaComprobanteNuevo = Convert.ToDateTime(suggest.FECHA_COMPROBANTE_NUEVO);
                            suggest.FECHA_COMPROBANTE_NUEVO = fechaComprobanteNuevo.ToString("dd/MM/yyyy");
                        }
                        if (suggest.FECHA_COMPROBANTE_ORIGEN != "")
                        {
                            DateTime fechaComprobanteOrigen = Convert.ToDateTime(suggest.FECHA_COMPROBANTE_ORIGEN);
                            suggest.FECHA_COMPROBANTE_ORIGEN = fechaComprobanteOrigen.ToString("dd/MM/yyyy");
                        }
                        if (suggest.FECHA_REFACTURACION != "")
                        {
                            DateTime fechaComprobanteRefact = Convert.ToDateTime(suggest.FECHA_REFACTURACION);
                            suggest.FECHA_REFACTURACION = fechaComprobanteRefact.ToString("dd/MM/yyyy");
                        }
                        entities.lista.Add(suggest);

                    }
                    entities.P_EST = Convert.ToInt32(P_EST.Value.ToString());
                    entities.P_MENSAGE = P_MENSAGE.Value.ToString();

                    listaReportesListado.Add(entities);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return listaReportesListado;
        }

    }
}
