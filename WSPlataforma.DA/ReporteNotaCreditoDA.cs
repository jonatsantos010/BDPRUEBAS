/*************************************************************************************************/
/*  NOMBRE              :   ReporteNotaCreditoDA.CS                                              */
/*  DESCRIPCION         :   Capa DA - ReporteNotaCredito                                         */
/*  AUTOR               :   MATERIAGRIS - JOSUE CORONEL FLORES                                   */
/*  FECHA               :   08-11-2022                                                           */
/*  VERSION             :   1.0                                                                  */
/*************************************************************************************************/

using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.Entities.ReporteNotaCreditoModel.ViewModel;
using WSPlataforma.Entities.ReporteNotaCreditoModel.BindingModel;
using WSPlataforma.Util;

namespace WSPlataforma.DA
{
    public class ReporteNotaCreditoDA: ConnectionBase
    {

        private readonly string Package = "PKG_SCTR_COBRANZAS";

        List<PremiumVM> ListPremium = new List<PremiumVM>();

        #region Filters
        public List<BranchVM> ListarRamo()
        {
            var parameters = new List<OracleParameter>();
            var listaRamo = new List<BranchVM>();
            var procedure = string.Format("{0}.{1}", this.Package, "SPS_GET_BRANCHES");
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

        //public List<BranchVM> ListarRamo()
        //{
        //    var sPackageName = string.Format("{0}.{1}", this.Package, "LIST_RAMO");
        //    BranchVM entities = null;
        //    List<BranchVM> listaRamo = new List<BranchVM>();
        //    List<OracleParameter> parameters = new List<OracleParameter>();

        //    parameters.Add(new OracleParameter("C_LISTA", OracleDbType.RefCursor, ParameterDirection.Output));

        //    using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameters))
        //    {
        //        while (dr.Read())
        //        {
        //            entities = new BranchVM();
        //            entities.nBranch = Convert.ToInt32(dr["NBRANCH"]);
        //            entities.sDescript = Convert.ToString(dr["SDESCRIPT"]);
        //            listaRamo.Add(entities);
        //        }
        //    }
        //    return listaRamo;
        //}

        public List<ProductVM> GetProductsListByBranch(ProductoFilter data)
        {
            var parameter = new List<OracleParameter>();
            var ListProductByBranch = new List<ProductVM>();
            ProductVM entities = null;
            var sPackageName = string.Format("{0}.{1}", this.Package, "SPS_GET_LISTPRODUCT");

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.nBranch, ParameterDirection.Input));
                //OUTPUT
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                if(data.nBranch == 999)
                {
                    using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter))
                    {
                        while (dr.Read())
                        {
                            entities = new ProductVM();
                            entities.nParameter = Convert.ToInt32(dr["NPARAMETER"]);
                            entities.sDes_para = Convert.ToString(dr["SDES_PARA"]);
                            ListProductByBranch.Add(entities);
                        }
                        //ListProductByBranch = dr.ReadRowsList<ProductVM>();
                        //ELog.CloseConnection(dr);
                    }
                }
                else
                {
                    using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter))
                    {
                        while (dr.Read())
                        {
                            entities = new ProductVM();
                            entities.nProduct = Convert.ToInt32(dr["NPRODUCT"]);
                            entities.sDescript = Convert.ToString(dr["SDESCRIPT"]);
                            ListProductByBranch.Add(entities);
                        }
                        //ListProductByBranch = dr.ReadRowsList<ProductVM>();
                        //ELog.CloseConnection(dr);
                    }
                }

            }
            catch (Exception ex)
            {
                LogControl.save("GetProductsListByBranch", ex.ToString(), "3");
            }

            return ListProductByBranch;
        }


        public List<ProductVM> ListarProducto(ProductoFilter data)
        {
            var sPackageName = string.Format("{0}.{1}", this.Package, "LIST_PRODUCTO");
            ProductVM entities = null;
            List<ProductVM> ListProductoPay = new List<ProductVM>();
            List<OracleParameter> parameters = new List<OracleParameter>();
            long _idRamo = data.nBranch;

            parameters.Add(new OracleParameter("RAMO", OracleDbType.Int16, _idRamo, ParameterDirection.Input));
            parameters.Add(new OracleParameter("C_LISTA", OracleDbType.RefCursor, ParameterDirection.Output));

            using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameters))
            {
                while (dr.Read())
                {
                    entities = new ProductVM();
                    entities.nProduct = Convert.ToInt32(dr["NPRODUCT"]);
                    entities.sDescript = Convert.ToString(dr["SDESCRIPT"]);
                    ListProductoPay.Add(entities);
                }
            }
            return ListProductoPay;
        }
        public List<TipoConsultaVM> ListarTipoConsulta()
        {
            var sPackageName = string.Format("{0}.{1}", this.Package, "LIST_TIPO_CONSULTA");
            TipoConsultaVM entities = null;
            List<TipoConsultaVM> listaTipoConsulta = new List<TipoConsultaVM>();
            List<OracleParameter> parameters = new List<OracleParameter>();

            parameters.Add(new OracleParameter("C_LISTA", OracleDbType.RefCursor, ParameterDirection.Output));

            using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameters))
            {
                while (dr.Read())
                {
                    entities = new TipoConsultaVM();
                    entities.nTipoConsulta = Convert.ToInt32(dr["NVALUE"]);
                    entities.sDescript = Convert.ToString(dr["SDESCRIPT"]);
                    listaTipoConsulta.Add(entities);
                }
            }
            return listaTipoConsulta;
        }
        #endregion

        #region Listado
        public List<ListarNotaCreditoVM> ListarNotaCredito(ListarNotaCreditoFilter data)
        {
            var sPackageName = string.Format("{0}.{1}", "PKG_CONCI_DEV_REPORT", "LIST_MOV_NC2");
            List<OracleParameter> parameters = new List<OracleParameter>();

            //ListarNotaCreditoVM entities = null;
            ListarNotaCreditoVM entities = new ListarNotaCreditoVM();
            List<ListarNotaCreditoVM> listaReportesListado = new List<ListarNotaCreditoVM>();
            //DateTime dtToday = DateTime.Now;
            string endDateStr  = data.endDate.ToString("dd/MM/yyyy");
            string initDateStr = data.startDate.ToString("dd/MM/yyyy");
            string idEstadoCon = "";
            entities.lista = new List<ListarNotaCreditoVM.P_TABLE>();

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
                parameters.Add(new OracleParameter("P_SCLIDOCUMENT", OracleDbType.NVarchar2, data.idCliDocument, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_INITDATE", OracleDbType.NVarchar2, initDateStr, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_ENDDATE", OracleDbType.NVarchar2, endDateStr, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_IDTIPOCON", OracleDbType.Int32, data.idTipoConsulta, ParameterDirection.Input));

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
                        ListarNotaCreditoVM.P_TABLE suggest = new ListarNotaCreditoVM.P_TABLE();
                        suggest.COMPROBANTE = dr["COMPROBANTE"] == DBNull.Value ? string.Empty : dr["COMPROBANTE"].ToString();
                        suggest.FECHA_COMPROBANTE = dr["FECHA_COMPROBANTE"] == DBNull.Value ? string.Empty : dr["FECHA_COMPROBANTE"].ToString();
                        suggest.ESTADO_COMPROBANTE = dr["ESTADO_COMPROBANTE"] == DBNull.Value ? string.Empty : dr["ESTADO_COMPROBANTE"].ToString();
                        suggest.FECHA_ESTADO = dr["FECHA_ESTADO"] == DBNull.Value ? string.Empty : dr["FECHA_ESTADO"].ToString();
                        suggest.PRIMA_NETA = dr["PRIMA_NETA"] == DBNull.Value ? 0 : Decimal.Parse(dr["PRIMA_NETA"].ToString());
                        suggest.DERECHO_EMISION = dr["DERECHO_EMISION"] == DBNull.Value ? 0 : Decimal.Parse(dr["DERECHO_EMISION"].ToString());
                        suggest.IMPUESTO = dr["IMPUESTO"] == DBNull.Value ? 0 : Decimal.Parse(dr["IMPUESTO"].ToString());
                        suggest.PRIMA_TOTAL = dr["PRIMA_TOTAL"] == DBNull.Value ? 0 : Decimal.Parse(dr["PRIMA_TOTAL"].ToString());
                        suggest.INICIO_POLIZA = dr["INICIO_POLIZA"] == DBNull.Value ? string.Empty : dr["INICIO_POLIZA"].ToString();
                        suggest.FIN_POLIZA = dr["FIN_POLIZA"] == DBNull.Value ? string.Empty : dr["FIN_POLIZA"].ToString();
                        suggest.FACTURA_AFECTA = dr["FACTURA_AFECTA"] == DBNull.Value ? string.Empty : dr["FACTURA_AFECTA"].ToString();
                        suggest.EMISION_FAC_AFECTA = dr["EMISION_FAC_AFECTA"] == DBNull.Value ? string.Empty : dr["EMISION_FAC_AFECTA"].ToString();
                        suggest.MONTO_FAC_AFECTA = dr["MONTO_FAC_AFECTA"] == DBNull.Value ? 0 : Decimal.Parse(dr["MONTO_FAC_AFECTA"].ToString());
                        suggest.MODO_DE_USO = dr["MODO_DE_USO"] == DBNull.Value ? string.Empty : dr["MODO_DE_USO"].ToString();
                        suggest.MONTO_USADO = dr["MONTO_USADO"] == DBNull.Value ? 0 : Decimal.Parse(dr["MONTO_USADO"].ToString());
                        suggest.SALDO_NC = dr["SALDO_NC"] == DBNull.Value ? 0 : Decimal.Parse(dr["SALDO_NC"].ToString());
                        suggest.TRANSACCION = dr["TRANSACCION"] == DBNull.Value ? 0 : int.Parse(dr["TRANSACCION"].ToString());
                        //suggest.EMISION_FAC_AFECTA = dr["EMISIÓN_FAC_AFECTA"] == DBNull.Value ? string.Empty : dr["EMISIÓN_FAC_AFECTA"].ToString(); /* JE - Fecha de Emision - 18/11/2022 */
                        suggest.FECHA_APLICACION = dr["FECHA_APLICACION"] == DBNull.Value ? string.Empty : dr["FECHA_APLICACION"].ToString(); /* JE - Fecha de Aplicacion - 18/11/2022 */
                        suggest.FECHA_ANULACION = dr["FECHA_ANULACION"] == DBNull.Value ? string.Empty : dr["FECHA_ANULACION"].ToString(); /* JE - Fecha de Anulacion - 18/11/2022 */
                        idEstadoCon = dr["IDESTADO"] == DBNull.Value ? string.Empty : dr["IDESTADO"].ToString();

                        suggest.SCLINUMDOCU = dr["SCLINUMDOCU"] == DBNull.Value ? string.Empty : dr["SCLINUMDOCU"].ToString();
                        suggest.SCLIENAME = dr["SCLIENAME"] == DBNull.Value ? string.Empty : dr["SCLIENAME"].ToString();
                        suggest.NPOLICY = dr["NPOLICY"] == DBNull.Value ? string.Empty : dr["NPOLICY"].ToString();
                        suggest.FECH_EMI_POLI = dr["FECH_EMI_POLI"] == DBNull.Value ? string.Empty : dr["FECH_EMI_POLI"].ToString();
                        suggest.FACTURA_ORIGEN = dr["FACTURA_ORIGEN"] == DBNull.Value ? string.Empty : dr["FACTURA_ORIGEN"].ToString();

                        //Seteamos pendiente cuando TRANSACCION = 0
                        // if (suggest.TRANSACCION == 0)
                        //{
                        //     suggest.ESTADO_COMPROBANTE = "PENDIENTE";
                        // }

                        //Montos a dos decimales
                        suggest.PRIMA_NETA = Convert.ToDecimal(String.Format("{0:0.00}", suggest.PRIMA_NETA));
                        suggest.DERECHO_EMISION = Convert.ToDecimal(String.Format("{0:0.00}", suggest.DERECHO_EMISION));
                        suggest.IMPUESTO = Convert.ToDecimal(String.Format("{0:0.00}", suggest.IMPUESTO));
                        suggest.PRIMA_TOTAL = Convert.ToDecimal(String.Format("{0:0.00}", suggest.PRIMA_TOTAL));
                        suggest.MONTO_FAC_AFECTA = Convert.ToDecimal(String.Format("{0:0.00}", suggest.MONTO_FAC_AFECTA));
                        suggest.MONTO_USADO = Convert.ToDecimal(String.Format("{0:0.00}", suggest.MONTO_USADO));
                        suggest.SALDO_NC = Convert.ToDecimal(String.Format("{0:0.00}", suggest.SALDO_NC));

                        //importe total Prima Neta + Derecha Emisión + IGV
                        suggest.IMPORTTOTAL = suggest.PRIMA_NETA + suggest.DERECHO_EMISION + suggest.IMPUESTO;


                        //formato de fechas a dd/MM/yyyy
                        if (suggest.FECHA_COMPROBANTE != "")
                        {
                            DateTime fechaComprobante = Convert.ToDateTime(suggest.FECHA_COMPROBANTE);
                            suggest.FECHA_COMPROBANTE = fechaComprobante.ToString("dd/MM/yyyy");
                        }
                        if (suggest.FECHA_ESTADO != "")
                        {
                            DateTime fechaEstado = Convert.ToDateTime(suggest.FECHA_ESTADO);
                            suggest.FECHA_ESTADO = fechaEstado.ToString("dd/MM/yyyy");
                        }
                        if (suggest.INICIO_POLIZA != "")
                        {
                            DateTime inicioPoliza = Convert.ToDateTime(suggest.INICIO_POLIZA);
                            suggest.INICIO_POLIZA = inicioPoliza.ToString("dd/MM/yyyy");
                        }
                        if (suggest.FIN_POLIZA != "")
                        {
                            DateTime finPoliza = Convert.ToDateTime(suggest.FIN_POLIZA);
                            suggest.FIN_POLIZA = finPoliza.ToString("dd/MM/yyyy");
                        }
                        if (suggest.EMISION_FAC_AFECTA != "")
                        {
                            DateTime fechaFacAfecta = Convert.ToDateTime(suggest.EMISION_FAC_AFECTA);
                            suggest.EMISION_FAC_AFECTA = fechaFacAfecta.ToString("dd/MM/yyyy");
                        }
                        if (suggest.FECHA_APLICACION != "") { 
                            DateTime fechaAplicacion = Convert.ToDateTime(suggest.FECHA_APLICACION);
                            suggest.FECHA_APLICACION = fechaAplicacion.ToString("dd/MM/yyyy");
                        }
                        if (suggest.FECHA_ANULACION != "")
                        {
                            DateTime fechaAnulacion = Convert.ToDateTime(suggest.FECHA_ANULACION);
                            suggest.FECHA_ANULACION = fechaAnulacion.ToString("dd/MM/yyyy");
                        }
                        entities.lista.Add(suggest);
                        //suggest = new ListarNotaCreditoVM.P_TABLE();

                        //if (data.idTipoConsulta == "0")
                        //{
                        //    entities.lista.Add(suggest);
                        //}
                        //else
                        //{
                        //    if (data.idTipoConsulta == "1")
                        //    {
                        //        if (idEstadoCon == "3")
                        //        {
                        //            entities.lista.Add(suggest);
                        //        }
                        //    }
                        //    else
                        //    {
                        //        if (idEstadoCon != "3")
                        //        {
                        //            entities.lista.Add(suggest);
                        //        }
                        //    }
                        //}

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
        #endregion
    }
}
