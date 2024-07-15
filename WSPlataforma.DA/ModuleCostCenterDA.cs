using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.DataAccess.Client;
using System.Data;
using WSPlataforma.Util;
using WSPlataforma.Entities.ModuleCostCenterModel.ViewModel;
using WSPlataforma.Entities.ModuleCostCenterModel.BindingModel;


namespace WSPlataforma.DA
{
    public class ModuleCostCenterDA : ConnectionBase
    {

        private readonly string Package = "PKG_SCTR_CENTCOSTOS";


        // LISTA RAMOS
        public Task<ListarRamosVM> ListarRamos()
        {
            var parameters = new List<OracleParameter>();
            ListarRamosVM entities = new ListarRamosVM();
            entities.combos = new List<ListarRamosVM.P_TABLE>();
            var procedure = string.Format("{0}.{1}", this.Package, "SPS_GET_BRANCHES");
            try
            {
                parameters.Add(new OracleParameter("P_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ListarRamosVM.P_TABLE suggest = new ListarRamosVM.P_TABLE();
                        suggest.NBRANCH = reader["NBRANCH"] == DBNull.Value ? 0 : int.Parse(reader["NBRANCH"].ToString());
                        suggest.SDESCRIPT = reader["SDESCRIPT"] == DBNull.Value ? string.Empty : reader["SDESCRIPT"].ToString();
                        entities.combos.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarRamos", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<ListarRamosVM>(entities);
        }


        // LISTA RAMOS TODOS
        public Task<ListarRamosVM> listarRamosTodos()
        {
            var parameters = new List<OracleParameter>();
            ListarRamosVM entities = new ListarRamosVM();
            entities.combos = new List<ListarRamosVM.P_TABLE>();
            var procedure = string.Format("{0}.{1}", this.Package, "SPS_GET_BRANCHESALL");
            try
            {
                parameters.Add(new OracleParameter("P_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ListarRamosVM.P_TABLE suggest = new ListarRamosVM.P_TABLE();
                        suggest.NBRANCH = reader["NBRANCH"] == DBNull.Value ? 0 : int.Parse(reader["NBRANCH"].ToString());
                        suggest.SDESCRIPT = reader["SDESCRIPT"] == DBNull.Value ? string.Empty : reader["SDESCRIPT"].ToString();
                        entities.combos.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("listarRamosTodos", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<ListarRamosVM>(entities);
        }

        // LISTA PRODUCTOS
        public Task<ListarProductosVM> ListarProductos(ListarProductosFilter data)
        {
            var parameters = new List<OracleParameter>();

            ListarProductosVM entities = new ListarProductosVM();
            entities.combos = new List<ListarProductosVM.P_TABLE>();

            var procedure = string.Format("{0}.{1}", this.Package, "SPS_POST_PRODUCTS");

            try
            {
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int16, data.NBRANCH, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ListarProductosVM.P_TABLE suggest = new ListarProductosVM.P_TABLE();
                        suggest.NPRODUCT = reader["NPRODUCT"] == DBNull.Value ? 0 : int.Parse(reader["NPRODUCT"].ToString());
                        suggest.SDESCRIPT = reader["SDESCRIPT"] == DBNull.Value ? string.Empty : reader["SDESCRIPT"].ToString();

                        entities.combos.Add(suggest);
                    }
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarProductos", ex.ToString(), "3");
                throw;
            }

            return Task.FromResult<ListarProductosVM>(entities);
        }

        // LISTA TODOS PRODUCTOS
        public Task<ListarProductosVM> ListarProductosTodos(ListarProductosFilter data)
        {
            var parameters = new List<OracleParameter>();

            ListarProductosVM entities = new ListarProductosVM();
            entities.combos = new List<ListarProductosVM.P_TABLE>();

            var procedure = string.Format("{0}.{1}", this.Package, "SPS_POST_PRODUCTSALL");

            try
            {
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int16, data.NBRANCH, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ListarProductosVM.P_TABLE suggest = new ListarProductosVM.P_TABLE();
                        suggest.NPRODUCT = reader["NPRODUCT"] == DBNull.Value ? 0 : int.Parse(reader["NPRODUCT"].ToString());
                        suggest.SDESCRIPT = reader["SDESCRIPT"] == DBNull.Value ? string.Empty : reader["SDESCRIPT"].ToString();

                        entities.combos.Add(suggest);
                    }
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarProductosTodos", ex.ToString(), "3");
                throw;
            }

            return Task.FromResult<ListarProductosVM>(entities);
        }

        //Reportes
        public Task<ListarCentroCostosVM> ListarCentroCostos(ListarCentroCostosFilter data)
        {
            var parameters = new List<OracleParameter>();
            ListarCentroCostosVM entities = new ListarCentroCostosVM();
            entities.lista = new List<ListarCentroCostosVM.P_TABLE>();

            try
            {
                parameters.Add(new OracleParameter("pFlagAll", OracleDbType.Varchar2, data.CFLAGALL, ParameterDirection.Input));
                parameters.Add(new OracleParameter("pFechaIni", OracleDbType.Varchar2, data.DFECINI, ParameterDirection.Input));
                parameters.Add(new OracleParameter("pFechaFin", OracleDbType.Varchar2, data.DFECFIN, ParameterDirection.Input));
                parameters.Add(new OracleParameter("pRamo", OracleDbType.Int16, data.NBRANCH, ParameterDirection.Input));
                parameters.Add(new OracleParameter("pProducto", OracleDbType.Int16, data.NPRODUCT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("pCentro", OracleDbType.Varchar2, data.CODCC, ParameterDirection.Input));

                parameters.Add(new OracleParameter("cCursor", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT("Buscar_Centro_Costos", parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ListarCentroCostosVM.P_TABLE suggest = new ListarCentroCostosVM.P_TABLE();
                        if (data.TIPOLIST == 1)
                        {
                            suggest.NIDCENCOS = reader["NID"] == DBNull.Value ? 0 : int.Parse(reader["NID"].ToString());
                            suggest.SRIESGO = reader["RIESGO"] == DBNull.Value ? string.Empty : reader["RIESGO"].ToString();
                            suggest.SPRODUCT = reader["PRODUCTO"] == DBNull.Value ? string.Empty : reader["PRODUCTO"].ToString();
                            suggest.SCENCOS = reader["CENTRO"] == DBNull.Value ? string.Empty : reader["CENTRO"].ToString();
                            suggest.DFECREG = reader["DCOMPDATE"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["DCOMPDATE"].ToString());
                            suggest.SUSER = reader["USUARIO"] == DBNull.Value ? string.Empty : reader["USUARIO"].ToString();
                        }
                        else
                        {
                            suggest.SRIESGO = reader["RIE"] == DBNull.Value ? string.Empty : reader["RIE"].ToString();
                            suggest.SPRODUCT = reader["PRO"] == DBNull.Value ? string.Empty : reader["PRO"].ToString();
                            suggest.SCENCOS = reader["CENTRO"] == DBNull.Value ? string.Empty : reader["CENTRO"].ToString();
                            suggest.DFECREG = reader["DCOMPDATE"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["DCOMPDATE"].ToString());
                            suggest.SUSER = reader["USUARIO"] == DBNull.Value ? string.Empty : reader["USUARIO"].ToString();
                        }
                        entities.lista.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarCentroCostos", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<ListarCentroCostosVM>(entities);

        }

        // VALIDAR CENTRO DE COSTOS
        public Task<RespuestaVM> ValidarCentroCostos(AgregarCentroCostoFilter data)
        {
            var parameters = new List<OracleParameter>();

            RespuestaVM entities = new RespuestaVM();

            var procedure = string.Format("{0}.{1}", this.Package, "SPS_POST_POLCONCC");

            try
            {
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int16, data.NBRANCH, ParameterDirection.Input)); // NUMERO DE ORIGEN
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int16, data.NPRODUCT, ParameterDirection.Input)); // NUMERO DE INTERFACE
                parameters.Add(new OracleParameter("P_SCLINUMDOCU", OracleDbType.Varchar2, data.SCLINUMDOCU, ParameterDirection.Input)); // NUMERO DE ASIENTO
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                OracleParameter P_EST = new OracleParameter("P_EST", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                parameters.Add(P_SMESSAGE);
                parameters.Add(P_EST);
                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);
                entities.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                entities.P_EST = Convert.ToInt32(P_EST.Value.ToString());
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("ValidarCentroCostos", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<RespuestaVM>(entities);
        }


        // BUSCAR AGREGAR CENTRO DE COSTOS
        public Task<RespuestaVM> BuscarAgregarCentroCostos(AgregarCentroCostoFilter data)
        {
            var parameters = new List<OracleParameter>();

            RespuestaVM entities = new RespuestaVM();

            var procedure = string.Format("{0}.{1}", this.Package, "SPS_POST_SINCC");

            try
            {
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int16, data.NBRANCH, ParameterDirection.Input)); // NUMERO DE RAMO
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int16, data.NPRODUCT, ParameterDirection.Input)); // NUMERO DE PRODUCTO
                parameters.Add(new OracleParameter("P_SCLINUMDOCU", OracleDbType.Varchar2, data.SCLINUMDOCU, ParameterDirection.Input)); // DOCUMENTO DEL CLIENTE
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                OracleParameter P_EST = new OracleParameter("P_EST", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_CONTRATA = new OracleParameter("P_CONTRATA", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                OracleParameter P_CODCC = new OracleParameter("P_CODCC", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_SMESSAGE);
                parameters.Add(P_EST);
                parameters.Add(P_CONTRATA);
                parameters.Add(P_CODCC);
                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);
                entities.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                entities.P_EST = Convert.ToInt32(P_EST.Value.ToString());
                entities.P_CONTRATA = P_CONTRATA.Value.ToString();
                entities.P_CODCC = P_CODCC.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("BuscarAgregarCentroCostos", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<RespuestaVM>(entities);
        }

        // AGREGAR CENTRO DE COSTOS
        public Task<RespuestaVM> AgregarCentroCostos(AgregarCentroCostoFilter data)
        {
            var parameters = new List<OracleParameter>();

            RespuestaVM entities = new RespuestaVM();

            var procedure = string.Format("{0}.{1}", this.Package, "SPS_POST_REGCC");

            try
            {
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int16, data.NBRANCH, ParameterDirection.Input)); // NUMERO DE RAMO
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int16, data.NPRODUCT, ParameterDirection.Input)); // NUMERO DE PRODUCTO
                parameters.Add(new OracleParameter("P_CC", OracleDbType.Varchar2, data.CODCC, ParameterDirection.Input)); // CODIGO CENTRO DE COSTOS
                parameters.Add(new OracleParameter("P_DESCRI", OracleDbType.Varchar2, data.SDESCENCOS, ParameterDirection.Input)); // DESCRIPCION CENTRO DE COSTOS

                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                OracleParameter P_EST = new OracleParameter("P_EST", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_PKNID = new OracleParameter("P_PKNID", OracleDbType.Int32, 4000, null, ParameterDirection.Output);

                parameters.Add(P_SMESSAGE);
                parameters.Add(P_EST);
                parameters.Add(P_PKNID);
                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);
                entities.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                entities.P_EST = Convert.ToInt32(P_EST.Value.ToString());
                entities.P_PKNID = Convert.ToInt32(P_PKNID.Value.ToString());
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("AgregarCentroCostos", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<RespuestaVM>(entities);
        }

        // LISTAR POLIZA CENTRO DE COSTOS
        public Task<AsignarCentroCostoVM> ListarPoliza(AsignarCentroCostoFilter data)
        {
            var parameters = new List<OracleParameter>();
            AsignarCentroCostoVM entities = new AsignarCentroCostoVM();
            entities.lista = new List<AsignarCentroCostoVM.P_TABLE>();

            try
            {
                parameters.Add(new OracleParameter("pFlag", OracleDbType.Varchar2, data.CFLAGALL, ParameterDirection.Input));
                parameters.Add(new OracleParameter("pFechaIni", OracleDbType.Varchar2, data.DFECINI, ParameterDirection.Input));
                parameters.Add(new OracleParameter("pFechaFin", OracleDbType.Varchar2, data.DFECFIN, ParameterDirection.Input));
                parameters.Add(new OracleParameter("pRamo", OracleDbType.Int16, data.NBRANCH, ParameterDirection.Input));
                parameters.Add(new OracleParameter("pProducto", OracleDbType.Int16, data.NPRODUCT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("pPoliza", OracleDbType.Int16, data.NPOLICY, ParameterDirection.Input));

                parameters.Add(new OracleParameter("cCursor", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT("listar_poliza_asignacion", parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        AsignarCentroCostoVM.P_TABLE suggest = new AsignarCentroCostoVM.P_TABLE();
                        suggest.SBRANCH = reader["RAMO"] == DBNull.Value ? string.Empty : reader["RAMO"].ToString();
                        suggest.SPRODUCT = reader["PRODUCTO"] == DBNull.Value ? string.Empty : reader["PRODUCTO"].ToString();
                        suggest.NPOLICY = reader["NPOLICY"] == DBNull.Value ? 0 : int.Parse(reader["NPOLICY"].ToString());
                        suggest.FECHA = reader["FECHA"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["FECHA"].ToString());
                        suggest.INDEX = 0;
                        entities.lista.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarPoliza", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<AsignarCentroCostoVM>(entities);

        }

        // REGISTRAR POLIZA AL CENTRO DE COSTOS
        public Task<RespuestaVM> RegistrarPoliza(AsignarCentroCostoFilter data)
        {
            var parameters = new List<OracleParameter>();

            RespuestaVM entities = new RespuestaVM();

            var procedure = string.Format("{0}.{1}", this.Package, "SPS_POST_ASIGCC");

            try
            {
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int16, data.NBRANCH, ParameterDirection.Input)); // NUMERO DE RAMO
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int16, data.NPRODUCT, ParameterDirection.Input)); // NUMERO DE PRODUCTO
                parameters.Add(new OracleParameter("P_PKCC", OracleDbType.Varchar2, data.NIDCENCOS, ParameterDirection.Input)); // CODIGO CENTRO DE COSTOS
                parameters.Add(new OracleParameter("P_POLIZA", OracleDbType.Int16, data.NPOLICY, ParameterDirection.Input)); // NUMERO DE POLIZA

                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                OracleParameter P_EST = new OracleParameter("P_EST", OracleDbType.Int32, 4000, null, ParameterDirection.Output);

                parameters.Add(P_SMESSAGE);
                parameters.Add(P_EST);
                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);
                entities.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                entities.P_EST = Convert.ToInt32(P_EST.Value.ToString());
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("RegistrarPoliza", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<RespuestaVM>(entities);
        }

        // REGISTRAR ASIGNACION POLIZA CON CENTRO DE COSTOS
        public Task<RespuestaVM> RegistrarAsignacion(AsignarCentroCostoFilter data)
        {
            var parameters = new List<OracleParameter>();

            RespuestaVM entities = new RespuestaVM();

            var procedure = string.Format("{0}.{1}", this.Package, "SPS_POST_ASIGCC");

            try
            {
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int16, data.NBRANCH, ParameterDirection.Input)); // NUMERO DE RAMO
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int16, data.NPRODUCT, ParameterDirection.Input)); // NUMERO DE PRODUCTO
                parameters.Add(new OracleParameter("P_PKCC", OracleDbType.Varchar2, data.NIDCENCOS, ParameterDirection.Input)); // CODIGO CENTRO DE COSTOS
                parameters.Add(new OracleParameter("P_POLIZA", OracleDbType.Varchar2, data.NPOLICY, ParameterDirection.Input)); // NUMERO DE POLIZA

                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                OracleParameter P_EST = new OracleParameter("P_EST", OracleDbType.Int32, 4000, null, ParameterDirection.Output);

                parameters.Add(P_SMESSAGE);
                parameters.Add(P_EST);
                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);
                entities.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                entities.P_EST = Convert.ToInt32(P_EST.Value.ToString());
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("RegistrarAsignacion", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<RespuestaVM>(entities);
        }
    }
}
