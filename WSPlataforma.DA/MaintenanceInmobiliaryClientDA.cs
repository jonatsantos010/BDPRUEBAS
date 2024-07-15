using System.Linq;
using System;
using System.Collections.Generic;
using WSPlataforma.Util;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.Entities.MaintenanceInmoClientModel.ViewModel;
using WSPlataforma.Entities.MaintenanceInmoClientModel.BindingModel;
using WSPlataforma.Entities.ModuleReportsModel.BindingModel;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using System.Data;
using System.Configuration;

namespace WSPlataforma.DA
{
    public class MaintenanceInmobiliaryClientDA : ConnectionBase
    {
        public List<DocumentBM> GetTypeDocumentList()
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<DocumentBM> ListDocument = new List<DocumentBM>();
            string storedProcedureName = ProcedureName.pkg_inmobiliaryMaintenance + ".SPS_GET_LIST_DOCUMENTS";
          
            try
            {
                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("P_CTABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                parameter.Add(C_TABLE);
                parameter.Add(P_NCODE);
                parameter.Add(P_MESSAGE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureInmo(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        DocumentBM item = new DocumentBM();
                        item.NTIPDOC = odr["NTIPDOC"] == DBNull.Value ? 0 : int.Parse(odr["NTIPDOC"].ToString());
                        item.STIPDOC_DES = odr["STIPDOC_DES"] == DBNull.Value ? string.Empty : odr["STIPDOC_DES"].ToString();
                        ListDocument.Add(item);
                    }
                }
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("getPathConfig", ex.ToString(), "3");
            }
            return ListDocument;

        }

        public List<OptionBM> GetTypeOptionList()
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<OptionBM> ListOptions = new List<OptionBM>();
            string storedProcedureName = ProcedureName.pkg_inmobiliaryMaintenance + ".SPS_GET_LIST_OPCIONES";
            try
            {
                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("P_CTABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                parameter.Add(C_TABLE);
                parameter.Add(P_NCODE);
                parameter.Add(P_MESSAGE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureInmo(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        OptionBM item = new OptionBM();
                        item.NOPCION = odr["NOPCION"] == DBNull.Value ? 0 : int.Parse(odr["NOPCION"].ToString());
                        item.SOPCION_DES = odr["SOPCION_DES"] == DBNull.Value ? string.Empty : odr["SOPCION_DES"].ToString();
                        ListOptions.Add(item); 
                    }
                }
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("GetProcessDetail", ex.ToString(), "3");
            }

            return ListOptions;
        }

        //LISTA OPCIONES BUSCAR POR
        public List<OptionBM> GetListBuscarPor()
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<OptionBM> ListOptions = new List<OptionBM>();
            string storedProcedureName = ProcedureName.pkg_inmobiliaryMaintenance + ".SPS_GET_LIST_OPCIONES_BUSQ";
            try
            {
                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("P_CTABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                parameter.Add(C_TABLE);
                parameter.Add(P_NCODE);
                parameter.Add(P_MESSAGE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureInmo(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        OptionBM item = new OptionBM();
                        item.NOPCION = odr["NOPCION"] == DBNull.Value ? 0 : int.Parse(odr["NOPCION"].ToString());
                        item.SOPCION_DES = odr["SOPCION_DES"] == DBNull.Value ? string.Empty : odr["SOPCION_DES"].ToString();
                        ListOptions.Add(item);
                    }
                }
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("GetProcessDetail", ex.ToString(), "3");
            }

            return ListOptions;
        }

        //LISTA TIPOS FACTURACION
        public List<OptionBM> GetListTipFacturacion(ContratantesVM data)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<OptionBM> ListOptions = new List<OptionBM>();
            string storedProcedureName = ProcedureName.pkg_inmobiliaryMaintenance + ".SPS_GET_LIST_TIP_FACT";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NVALOR", OracleDbType.Varchar2, data.NVALOR, ParameterDirection.Input));
                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("P_CTABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                parameter.Add(C_TABLE);
                parameter.Add(P_NCODE);
                parameter.Add(P_MESSAGE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureInmo(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        OptionBM item = new OptionBM();
                        item.NOPCION = odr["NOPCION"] == DBNull.Value ? 0 : int.Parse(odr["NOPCION"].ToString());
                        item.SOPCION_DES = odr["SOPCION_DES"] == DBNull.Value ? string.Empty : odr["SOPCION_DES"].ToString();
                        item.STIP_EMISION = odr["STIP_EMISION"] == DBNull.Value ? string.Empty : odr["STIP_EMISION"].ToString();
                        ListOptions.Add(item);
                    }
                }
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("GetProcessDetail", ex.ToString(), "3");
            }

            return ListOptions;
        }

        //LISTA CONTRATANTES (CLIENTES)
        public List<ContratantesBM> GetContratantesList(ContratantesVM data)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<ContratantesBM> ListProcess = new List<ContratantesBM>();
            string storedProcedureName = ProcedureName.pkg_inmobiliaryMaintenance + ".SPS_GET_LIST_CLIENTES";
            try
            {
                
                //INPUT                
                parameter.Add(new OracleParameter("P_NTYPCLIENTDOC", OracleDbType.Int32, data.P_NTYPCLIENTDOC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCLINUMDOCU", OracleDbType.Varchar2, data.P_SCLINUMDOCU, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOD_INTERNO", OracleDbType.Varchar2, data.P_SCOD_INTERNO, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("P_CTABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                parameter.Add(C_TABLE);
                parameter.Add(P_NCODE);
                parameter.Add(P_MESSAGE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureInmo(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        ContratantesBM item = new ContratantesBM();

                        item.SCLIENT            = odr["SCLIENT"]           == DBNull.Value ? string.Empty : odr["SCLIENT"].ToString();
                        item.NIDTRXEMISION      = odr["NIDTRXEMISION"]     == DBNull.Value ? string.Empty : odr["NIDTRXEMISION"].ToString();
                        item.SCOD_INTERNO       = odr["SCOD_INTERNO"]      == DBNull.Value ? string.Empty : odr["SCOD_INTERNO"].ToString();
                        item.NTYPCLIENTDOC      = odr["NTYPCLIENTDOC"]     == DBNull.Value ? 0 : int.Parse (odr["NTYPCLIENTDOC"].ToString());
                        item.SDOCUMENT_DESC     = odr["SDOCUMENT_DESC"]    == DBNull.Value ? string.Empty : odr["SDOCUMENT_DESC"].ToString();
                        item.SCLINUMDOCU        = odr["SCLINUMDOCU"]       == DBNull.Value ? string.Empty : odr["SCLINUMDOCU"].ToString();
                        item.SCLIENAME          = odr["SCLIENAME"]         == DBNull.Value ? string.Empty : odr["SCLIENAME"].ToString();
                        item.SNOM_DIRECCION     = odr["SNOM_DIRECCION"]    == DBNull.Value ? string.Empty : odr["SNOM_DIRECCION"].ToString();                        
                        item.DEFFECDATE         = odr["DEFFECDATE"]        == DBNull.Value ? string.Empty : Convert.ToDateTime(odr["DEFFECDATE"]).ToString("dd/MM/yyyy");
                        item.DEXPIRDAT          = odr["DEXPIRDAT"]         == DBNull.Value ? string.Empty : Convert.ToDateTime(odr["DEXPIRDAT"]).ToString("dd/MM/yyyy");
                        item.NCURRENCY          = odr["NCURRENCY"]         == DBNull.Value ? 0 : int.Parse (odr["NCURRENCY"].ToString());                        
                        item.SCURRENCY_DESC     = odr["SCURRENCY_DESC"]    == DBNull.Value ? string.Empty : odr["SCURRENCY_DESC"].ToString();
                        item.NPREMIUM           = odr["NPREMIUM"]          == DBNull.Value ? 0 : double.Parse(odr["NPREMIUM"].ToString());
                        item.STIP_EMISION       = odr["STIP_EMISION"]      == DBNull.Value ? string.Empty : odr["STIP_EMISION"].ToString();
                        item.NVALOR             = odr["NVALOR"]            == DBNull.Value ? string.Empty : odr["NVALOR"].ToString();
                        item.STIP_EMISION_DESC  = odr["STIP_EMISION_DESC"] == DBNull.Value ? string.Empty : odr["STIP_EMISION_DESC"].ToString();
                        item.NBRANCH            = odr["NBRANCH"]           == DBNull.Value ? string.Empty : odr["NBRANCH"].ToString();
                        item.NPRODUCT           = odr["NPRODUCT"]          == DBNull.Value ? string.Empty : odr["NPRODUCT"].ToString();
                        item.NPOLICY            = odr["NPOLICY"]           == DBNull.Value ? string.Empty : odr["NPOLICY"].ToString();
                        item.SGLOSA             = odr["SGLOSA"]            == DBNull.Value ? string.Empty : odr["SGLOSA"].ToString();
                        item.DFEC_EMISION       = odr["DFEC_EMISION"]      == DBNull.Value ? string.Empty : Convert.ToDateTime(odr["DFEC_EMISION"]).ToString("dd/MM/yyyy");


                        ListProcess.Add(item);
                    }
                }
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("GetProcessHeader ", ex.ToString() , "3");
            }

            return ListProcess;
        }

        //CONSULTA CLIENTE POR: 
        public List<ConsultaClienteBM> GetConsultaClientesList(ConsultaClienteVM data)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<ConsultaClienteBM> ListProcess = new List<ConsultaClienteBM>();
            string storedProcedureName = ProcedureName.pkg_inmobiliaryMaintenance + ".SPS_GET_LIST_CLIENT";
            try
            {
                //INPUT                
                parameter.Add(new OracleParameter("P_NTYPCLIENTDOC", OracleDbType.Int32, data.NOPCION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCLINUMDOCU", OracleDbType.Varchar2, data.SVALOR, ParameterDirection.Input));                

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("P_CTABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                parameter.Add(C_TABLE);
                parameter.Add(P_NCODE);
                parameter.Add(P_MESSAGE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureInmo(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        ConsultaClienteBM item = new ConsultaClienteBM();

                        item.SCLIENT = odr["SCLIENT"] == DBNull.Value ? string.Empty : odr["SCLIENT"].ToString().Trim();
                        item.NTYPCLIENTDOC = odr["NTYPCLIENTDOC"] == DBNull.Value ? 0 : int.Parse(odr["NTYPCLIENTDOC"].ToString().Trim());
                        item.SDESCRIPT = odr["SDESCRIPT"] == DBNull.Value ? string.Empty : odr["SDESCRIPT"].ToString().Trim();
                        item.SCLINUMDOCU = odr["SCLINUMDOCU"] == DBNull.Value ? string.Empty : odr["SCLINUMDOCU"].ToString().Trim();
                        item.SCLIENAME = odr["SCLIENAME"] == DBNull.Value ? string.Empty : odr["SCLIENAME"].ToString().Trim();

                        ListProcess.Add(item);
                    }
                }
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("GetProcessHeader", ex.ToString(), "3");
            }

            return ListProcess;
        }

        // CIERRE
        public Task<DocumentsCierreVM> ListarDocumentosCierre()
        {
            var parameters = new List<OracleParameter>();
            DocumentsCierreVM entities = new DocumentsCierreVM();
            entities.P_LIST = new List<DocumentsCierreVM.DocumentsCierre>();
            string procedure = ProcedureName.pkg_inmobiliaryCierre + ".SPS_GET_LIST_DOCUMENTS";
            try
            {
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        DocumentsCierreVM.DocumentsCierre suggest = new DocumentsCierreVM.DocumentsCierre();
                        suggest.NTIPDOC = reader["NTIPDOC"] == DBNull.Value ? 0 : int.Parse(reader["NTIPDOC"].ToString());
                        suggest.STIPDOC_DES = reader["STIPDOC_DES"] == DBNull.Value ? string.Empty : reader["STIPDOC_DES"].ToString();
                        entities.P_LIST.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarDocumentosCierre", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<DocumentsCierreVM>(entities);
        }
        public Task<PorVencerCierreVM> ListarPorVencerCierre()
        {
            var parameters = new List<OracleParameter>();
            PorVencerCierreVM entities = new PorVencerCierreVM();
            entities.P_LIST = new List<PorVencerCierreVM.PorVencerCierre>();
            string procedure = ProcedureName.pkg_inmobiliaryCierre + ".SPS_GET_LIST_POR_VENCER";
            try
            {
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        PorVencerCierreVM.PorVencerCierre suggest = new PorVencerCierreVM.PorVencerCierre();
                        suggest.NOPCION = reader["NOPCION"] == DBNull.Value ? 0 : int.Parse(reader["NOPCION"].ToString());
                        suggest.SOPCION_DES = reader["SOPCION_DES"] == DBNull.Value ? string.Empty : reader["SOPCION_DES"].ToString();
                        entities.P_LIST.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarPorVencerCierre", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<PorVencerCierreVM>(entities);
        }
        public Task<EstadosBillsCierreVM> ListarEstadosBillsCierre()
        {
            var parameters = new List<OracleParameter>();
            EstadosBillsCierreVM entities = new EstadosBillsCierreVM();
            entities.P_LIST = new List<EstadosBillsCierreVM.EstadosBillsCierre>();
            string procedure = ProcedureName.pkg_inmobiliaryCierre + ".SPS_GET_LIST_STATE_BILLS";
            try
            {
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        EstadosBillsCierreVM.EstadosBillsCierre suggest = new EstadosBillsCierreVM.EstadosBillsCierre();
                        suggest.NBILLSTAT = reader["NBILLSTAT"] == DBNull.Value ? 0 : int.Parse(reader["NBILLSTAT"].ToString());
                        suggest.SDESCRIPT = reader["SDESCRIPT"] == DBNull.Value ? string.Empty : reader["SDESCRIPT"].ToString();
                        entities.P_LIST.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarEstadosBillsCierre", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<EstadosBillsCierreVM>(entities);
        }

        public Task<InmobiliairyCierreVM> GetInmobiliairyCierreReport(InmobiliairyCierreBM data)
        {
            var parameters = new List<OracleParameter>();
            InmobiliairyCierreVM entities = new InmobiliairyCierreVM();
            entities.P_LIST = new List<InmobiliairyCierreVM.InmobiliairyCierre>();

            string ini = data.P_STARTDATE == "" || data.P_STARTDATE == null ? "" : Convert.ToDateTime(data.P_STARTDATE).ToString("dd/MM/yyyy");
            string fin = data.P_ENDDATE == "" || data.P_ENDDATE == null ? "" : Convert.ToDateTime(data.P_ENDDATE).ToString("dd/MM/yyyy");

            string procedure = ProcedureName.pkg_inmobiliaryCierre + ".SPS_REPORT_TABLERO_CONTROL";
            try
            {
                parameters.Add(new OracleParameter("P_NTYPCLIENTDOC", OracleDbType.NVarchar2, data.P_NTYPCLIENTDOC, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SCLINUMDOCU", OracleDbType.NVarchar2, data.P_SCLINUMDOCU, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SCOD_INTERNO", OracleDbType.NVarchar2, data.P_SCOD_INTERNO, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NBILLSTAT", OracleDbType.Int32, data.P_NBILLSTAT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_STARTDATE", OracleDbType.NVarchar2, ini, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_ENDDATE", OracleDbType.NVarchar2, fin, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPOR_VENCER", OracleDbType.Int32, data.P_NPOR_VENCER, ParameterDirection.Input));
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        InmobiliairyCierreVM.InmobiliairyCierre suggest = new InmobiliairyCierreVM.InmobiliairyCierre();
                        suggest.CODIGO_INMUEBLE = reader["CODIGO_INMUEBLE"] == DBNull.Value ? string.Empty : reader["CODIGO_INMUEBLE"].ToString();
                        suggest.TIPO_SERVICIO = reader["TIPO_SERVICIO"] == DBNull.Value ? string.Empty : reader["TIPO_SERVICIO"].ToString();
                        suggest.TIPO_DOCUMENTO = reader["TIPO_DOCUMENTO"] == DBNull.Value ? string.Empty : reader["TIPO_DOCUMENTO"].ToString();
                        suggest.NRO_DOCUMENTO = reader["NRO_DOCUMENTO"] == DBNull.Value ? string.Empty : reader["NRO_DOCUMENTO"].ToString();

                        suggest.RAZONSOCIAL_NOMBRE = reader["RAZONSOCIAL_NOMBRE"] == DBNull.Value ? string.Empty : reader["RAZONSOCIAL_NOMBRE"].ToString();
                        suggest.FCH_INI = reader["FCH_INI"] == DBNull.Value ? string.Empty : Convert.ToDateTime(reader["FCH_INI"]).ToString("dd/MM/yyyy");
                        suggest.FCH_FIN = reader["FCH_FIN"] == DBNull.Value ? string.Empty : Convert.ToDateTime(reader["FCH_FIN"]).ToString("dd/MM/yyyy");
                        suggest.NRO_COMPROBANTE = reader["NRO_COMPROBANTE"] == DBNull.Value ? string.Empty : reader["NRO_COMPROBANTE"].ToString();

                        suggest.FCH_COMPROBANTE = reader["FCH_COMPROBANTE"] == DBNull.Value ? string.Empty : reader["FCH_COMPROBANTE"].ToString();
                        suggest.MONTO = reader["MONTO"] == DBNull.Value ? 0 : decimal.Parse(reader["MONTO"].ToString());
                        suggest.SPOR_VENCER = reader["SPOR_VENCER"] == DBNull.Value ? string.Empty : reader["SPOR_VENCER"].ToString();

                        suggest.ESTADO_COMPROBANTE = reader["ESTADO_COMPROBANTE"] == DBNull.Value ? string.Empty : reader["ESTADO_COMPROBANTE"].ToString();
                        suggest.ESTADO_COMPROBANTE_COLOR = reader["ESTADO_COMPROBANTE_COLOR"] == DBNull.Value ? string.Empty : reader["ESTADO_COMPROBANTE_COLOR"].ToString();
                        suggest.ESTADO_CONTRATO = reader["ESTADO_CONTRATO"] == DBNull.Value ? string.Empty : reader["ESTADO_CONTRATO"].ToString();
                        suggest.ESTADO_CONTRATO_COLOR = reader["ESTADO_CONTRATO_COLOR"] == DBNull.Value ? string.Empty : reader["ESTADO_CONTRATO_COLOR"].ToString();

                        entities.P_LIST.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("GetInmobiliairyCierreReport", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<InmobiliairyCierreVM>(entities);
        }
        public DataSet GetDataReportInmobiliariasControlSeguimiento(InmobiliairyCierreBM data)
        {
            DataSet dataReport = new DataSet();
            DataTable dataCab = new DataTable();

            int Pcode;
            string Pmessage;

            string ini = data.P_STARTDATE == "" || data.P_STARTDATE == null ? "" : Convert.ToDateTime(data.P_STARTDATE).ToString("dd/MM/yyyy");
            string fin = data.P_ENDDATE == "" || data.P_ENDDATE == null ? "" : Convert.ToDateTime(data.P_ENDDATE).ToString("dd/MM/yyyy");

            try
            {
                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleINMOB"].ToString()))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = cn;
                        cmd.CommandText = string.Format("{0}.{1}", ProcedureName.pkg_inmobiliaryCierre, "SPS_REPORT_TABLERO_CONTROL_XLSX");
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("P_NTYPCLIENTDOC", OracleDbType.Int32).Value = data.P_NTYPCLIENTDOC;
                        cmd.Parameters.Add("P_SCLINUMDOCU", OracleDbType.NVarchar2).Value = data.P_SCLINUMDOCU;
                        cmd.Parameters.Add("P_SCOD_INTERNO", OracleDbType.NVarchar2).Value = data.P_SCOD_INTERNO;
                        cmd.Parameters.Add("P_NBILLSTAT", OracleDbType.Int32).Value = data.P_NBILLSTAT;
                        cmd.Parameters.Add("P_STARTDATE", OracleDbType.NVarchar2).Value = ini;
                        cmd.Parameters.Add("P_ENDDATE", OracleDbType.NVarchar2).Value = fin;
                        cmd.Parameters.Add("P_NPOR_VENCER", OracleDbType.Int32).Value = data.P_NPOR_VENCER;

                        cmd.Parameters.Add("C_TABLE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                        var P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);
                        var P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                        P_NCODE.Size = 4000;
                        P_SMESSAGE.Size = 4000;
                        cmd.Parameters.Add(P_NCODE);
                        cmd.Parameters.Add(P_SMESSAGE);

                        cn.Open();
                        cmd.ExecuteNonQuery();

                        Pcode = Convert.ToInt32(P_NCODE.Value.ToString());
                        Pmessage = P_SMESSAGE.Value.ToString();

                        if (Pcode == 1) { throw new Exception(Pmessage); }

                        // CURSOR DE CABECERA
                        OracleRefCursor cursorCab = (OracleRefCursor)cmd.Parameters["C_TABLE"].Value;
                        dataCab.Load(cursorCab.GetDataReader());
                        dataCab.TableName = "REPORTE";
                        dataReport.Tables.Add(dataCab);
                        var cantCab = dataCab.Rows.Count;

                        ModuleReportsFilter dataRpteXLS = new ModuleReportsFilter();
                        ModuleReportsDA moduleDA = new ModuleReportsDA();

                        // OBTIENE CONFIGURACIÓN DE REPORTE
                        dataRpteXLS.P_NNUMORI = 0; // INMOBILIARIAS
                        dataRpteXLS.P_NREPORT = 10; // INMOBILIARIAS SEGUIMIENTO Y CONTROL
                        dataRpteXLS.P_NTIPO = 1; // RESUMEN

                        DataTable configRES = moduleDA.GetDataFields(dataRpteXLS);
                        List<ConfigFields> configurationCAB = moduleDA.GetFieldsConfiguration(configRES, dataRpteXLS);

                        string pathReport = ELog.obtainConfig("reportInmobControlSeg");
                        string fileName = "Reporte_Seguimiento_Control_Inmobiliarias.xlsx";

                        moduleDA.ExportToExcelCD(dataReport, configurationCAB, configurationCAB, pathReport, fileName);
                        cn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetDataReportInmobiliariasControlSeguimiento", ex.ToString(), "3");
                throw;
            }
            return dataReport;
        }
        public Task<TipoServicioCierreVM> ListarTipoServicioCierre()
        {
            var parameters = new List<OracleParameter>();
            TipoServicioCierreVM entities = new TipoServicioCierreVM();
            entities.P_LIST = new List<TipoServicioCierreVM.TipoServicioCierre>();
            string procedure = ProcedureName.pkg_inmobiliaryMaintenance + ".SPS_GET_LIST_TYPE_SERVICE";
            try
            {
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        TipoServicioCierreVM.TipoServicioCierre suggest = new TipoServicioCierreVM.TipoServicioCierre();
                        suggest.NPRODUCT = reader["NPRODUCT"] == DBNull.Value ? 0 : int.Parse(reader["NPRODUCT"].ToString());
                        suggest.SDESCRIPT = reader["SDESCRIPT"] == DBNull.Value ? string.Empty : reader["SDESCRIPT"].ToString();
                        entities.P_LIST.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarTipoServicioCierre", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<TipoServicioCierreVM>(entities);
        }
        public Task<InmobiliairyReportCierreVM> GetInmobiliairyReporteCierre(InmobiliairyReportCierreBM data)
        {
            var parameters = new List<OracleParameter>();
            InmobiliairyReportCierreVM entities = new InmobiliairyReportCierreVM();
            entities.P_LIST = new List<InmobiliairyReportCierreVM.InmobiliairyReportCierre>();

            string ini = data.P_STARTDATE == "" || data.P_STARTDATE == null ? "" : Convert.ToDateTime(data.P_STARTDATE).ToString("dd/MM/yyyy");
            string fin = data.P_ENDDATE == "" || data.P_ENDDATE == null ? "" : Convert.ToDateTime(data.P_ENDDATE).ToString("dd/MM/yyyy");

            string procedure = ProcedureName.pkg_inmobiliaryMaintenance + ".SPS_GET_REPORT_CLOSING";
            try
            {
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.NVarchar2, data.P_NPRODUCT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_STARTDATE", OracleDbType.NVarchar2, ini, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_ENDDATE", OracleDbType.NVarchar2, fin, ParameterDirection.Input));
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        InmobiliairyReportCierreVM.InmobiliairyReportCierre suggest = new InmobiliairyReportCierreVM.InmobiliairyReportCierre();
                        suggest.SCOD_INTERNO = reader["SCOD_INTERNO"] == DBNull.Value ? string.Empty : reader["SCOD_INTERNO"].ToString();
                        suggest.SDOCUMENT_DESC = reader["SDOCUMENT_DESC"] == DBNull.Value ? string.Empty : reader["SDOCUMENT_DESC"].ToString();
                        suggest.NRODOCUMENTO = reader["NRODOCUMENTO"] == DBNull.Value ? string.Empty : reader["NRODOCUMENTO"].ToString();
                        suggest.SCLIENAME = reader["SCLIENAME"] == DBNull.Value ? string.Empty : reader["SCLIENAME"].ToString();
                        suggest.FCH_INI = reader["FCH_INI"] == DBNull.Value ? string.Empty : Convert.ToDateTime(reader["FCH_INI"]).ToString("dd/MM/yyyy");

                        suggest.FCH_FIN = reader["FCH_FIN"] == DBNull.Value ? string.Empty : Convert.ToDateTime(reader["FCH_FIN"]).ToString("dd/MM/yyyy");
                        suggest.STIP_EMISION_DESC = reader["STIP_EMISION_DESC"] == DBNull.Value ? string.Empty : reader["STIP_EMISION_DESC"].ToString();
                        suggest.DBILLDATE = reader["DBILLDATE"] == DBNull.Value ? string.Empty : Convert.ToDateTime(reader["DBILLDATE"]).ToString("dd/MM/yyyy");
                        suggest.TIPO_COMPROBANTE = reader["TIPO_COMPROBANTE"] == DBNull.Value ? string.Empty : reader["TIPO_COMPROBANTE"].ToString();
                        suggest.NRO_COMPROBANTE = reader["NRO_COMPROBANTE"] == DBNull.Value ? string.Empty : reader["NRO_COMPROBANTE"].ToString();

                        suggest.SGLOSA_COMPROB = reader["SGLOSA_COMPROB"] == DBNull.Value ? string.Empty : reader["SGLOSA_COMPROB"].ToString();
                        suggest.SCURRENCY_DESC = reader["SCURRENCY_DESC"] == DBNull.Value ? string.Empty : reader["SCURRENCY_DESC"].ToString();
                        suggest.NPREMIUM = reader["NPREMIUM"] == DBNull.Value ? 0 : decimal.Parse(reader["NPREMIUM"].ToString());
                        suggest.ESTADO_COMPROBANTE = reader["ESTADO_COMPROBANTE"] == DBNull.Value ? string.Empty : reader["ESTADO_COMPROBANTE"].ToString();
                        suggest.NPRODUCT = reader["NPRODUCT"] == DBNull.Value ? 0 : int.Parse(reader["NPRODUCT"].ToString());

                        entities.P_LIST.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("GetInmobiliairyReporteCierre", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<InmobiliairyReportCierreVM>(entities);
        }
        public DataSet GetDataInmobiliairyReporteCierre(InmobiliairyReportCierreBM data)
        {
            DataSet dataReport = new DataSet();
            DataTable dataCab = new DataTable();

            int Pcode;
            string Pmessage;

            string ini = data.P_STARTDATE == "" || data.P_STARTDATE == null ? "" : Convert.ToDateTime(data.P_STARTDATE).ToString("dd/MM/yyyy");
            string fin = data.P_ENDDATE == "" || data.P_ENDDATE == null ? "" : Convert.ToDateTime(data.P_ENDDATE).ToString("dd/MM/yyyy");

            try
            {
                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleINMOB"].ToString()))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = cn;
                        cmd.CommandText = string.Format("{0}.{1}", ProcedureName.pkg_inmobiliaryMaintenance, "SPS_GET_REPORT_CLOSING_XLSX");
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("P_NPRODUCT", OracleDbType.Int32).Value = data.P_NPRODUCT;
                        cmd.Parameters.Add("P_STARTDATE", OracleDbType.NVarchar2).Value = ini;
                        cmd.Parameters.Add("P_ENDDATE", OracleDbType.NVarchar2).Value = fin;
                        cmd.Parameters.Add("C_TABLE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                        var P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);
                        var P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                        P_NCODE.Size = 4000;
                        P_SMESSAGE.Size = 4000;
                        cmd.Parameters.Add(P_NCODE);
                        cmd.Parameters.Add(P_SMESSAGE);

                        cn.Open();
                        cmd.ExecuteNonQuery();

                        Pcode = Convert.ToInt32(P_NCODE.Value.ToString());
                        Pmessage = P_SMESSAGE.Value.ToString();

                        if (Pcode == 1) { throw new Exception(Pmessage); }

                        // CURSOR DE CABECERA
                        OracleRefCursor cursorCab = (OracleRefCursor)cmd.Parameters["C_TABLE"].Value;
                        dataCab.Load(cursorCab.GetDataReader());
                        dataCab.TableName = "REPORTE";
                        dataReport.Tables.Add(dataCab);
                        var cantCab = dataCab.Rows.Count;

                        ModuleReportsFilter dataRpteXLS = new ModuleReportsFilter();
                        ModuleReportsDA moduleDA = new ModuleReportsDA();

                        // OBTIENE CONFIGURACIÓN DE REPORTE
                        dataRpteXLS.P_NNUMORI = 0; // INMOBILIARIAS
                        dataRpteXLS.P_NREPORT = 11; // INMOBILIARIAS CIERRE
                        dataRpteXLS.P_NTIPO = 1; // RESUMEN

                        DataTable configRES = moduleDA.GetDataFields(dataRpteXLS);
                        List<ConfigFields> configurationCAB = moduleDA.GetFieldsConfiguration(configRES, dataRpteXLS);

                        string pathReport = ELog.obtainConfig("reportInmobCierre");
                        string fileName = "Reporte_Cierre_Inmobiliarias.xlsx";

                        moduleDA.ExportToExcelCD(dataReport, configurationCAB, configurationCAB, pathReport, fileName);
                        cn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetDataInmobiliairyReporteCierre", ex.ToString(), "3");
                throw;
            }
            return dataReport;
        }

        // DGC

        // GÉNERO
        public Task<GeneroVM> ListarGeneroInmobiliaria()
        {
            var parameters = new List<OracleParameter>();
            GeneroVM entities = new GeneroVM();
            entities.P_LIST = new List<GeneroVM.Genero>();
            string procedure = ProcedureName.pkg_inmobiliaryMaintenance + ".SPS_GET_LIST_SEX";
            try
            {
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        GeneroVM.Genero suggest = new GeneroVM.Genero();
                        suggest.SSEXCLIEN = reader["SSEXCLIEN"] == DBNull.Value ? 0 : int.Parse(reader["SSEXCLIEN"].ToString());
                        suggest.SDESCRIPT = reader["SDESCRIPT"] == DBNull.Value ? string.Empty : reader["SDESCRIPT"].ToString();
                        entities.P_LIST.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarGeneroInmobiliaria", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<GeneroVM>(entities);
        }

        // NACIONALIDAD
        public Task<NacionalidadVM> ListarNacionalidadInmobiliaria()
        {
            var parameters = new List<OracleParameter>();
            NacionalidadVM entities = new NacionalidadVM();
            entities.P_LIST = new List<NacionalidadVM.Nacionalidad>();
            string procedure = ProcedureName.pkg_inmobiliaryMaintenance + ".SPS_GET_LIST_NATIONALITY";
            try
            {
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        NacionalidadVM.Nacionalidad suggest = new NacionalidadVM.Nacionalidad();
                        suggest.NNATIONALITY = reader["NNATIONALITY"] == DBNull.Value ? 0 : int.Parse(reader["NNATIONALITY"].ToString());
                        suggest.SDESCRIPT = reader["SDESCRIPT"] == DBNull.Value ? string.Empty : reader["SDESCRIPT"].ToString();
                        entities.P_LIST.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarNacionalidadInmobiliaria", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<NacionalidadVM>(entities);
        }

        // ESTADO CIVIL
        public Task<EstadoCivilVM> ListarEstadoCivilInmobiliaria()
        {
            var parameters = new List<OracleParameter>();
            EstadoCivilVM entities = new EstadoCivilVM();
            entities.P_LIST = new List<EstadoCivilVM.EstadoCivil>();
            string procedure = ProcedureName.pkg_inmobiliaryMaintenance + ".SPS_GET_LIST_CIVIL";
            try
            {
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        EstadoCivilVM.EstadoCivil suggest = new EstadoCivilVM.EstadoCivil();
                        suggest.NCIVILSTA = reader["NCIVILSTA"] == DBNull.Value ? 0 : int.Parse(reader["NCIVILSTA"].ToString());
                        suggest.SDESCRIPT = reader["SDESCRIPT"] == DBNull.Value ? string.Empty : reader["SDESCRIPT"].ToString();
                        entities.P_LIST.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarEstadoCivilInmobiliaria", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<EstadoCivilVM>(entities);
        }

        // TIPO DE DOCUMENTO
        public Task<TipoDocumentoVM> ListarTipoDocumentoInmobiliaria()
        {
            var parameters = new List<OracleParameter>();
            TipoDocumentoVM entities = new TipoDocumentoVM();
            entities.P_LIST = new List<TipoDocumentoVM.TipoDocumento>();
            string procedure = ProcedureName.pkg_inmobiliaryMaintenance + ".SPS_GET_LIST_DOCUMENTS";
            try
            {
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        TipoDocumentoVM.TipoDocumento suggest = new TipoDocumentoVM.TipoDocumento();
                        suggest.NTIPDOC = reader["NTIPDOC"] == DBNull.Value ? 0 : int.Parse(reader["NTIPDOC"].ToString());
                        suggest.STIPDOC_DES = reader["STIPDOC_DES"] == DBNull.Value ? string.Empty : reader["STIPDOC_DES"].ToString();
                        entities.P_LIST.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarTipoDocumentoInmobiliaria", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<TipoDocumentoVM>(entities);
        }

        // DETALLE CLIENTE
        public Task<GetClientDetVM> GetClientDetInmobiliaria(GetClientDetBM data)
        {
            var parameters = new List<OracleParameter>();
            GetClientDetVM entities = new GetClientDetVM();
            entities.P_LIST = new List<GetClientDetVM.ClientDet>();
            string procedure = ProcedureName.pkg_inmobiliaryMaintenance + ".SPS_GET_LIST_CLIENT_DET";
            try
            {
                parameters.Add(new OracleParameter("P_SCLIENT", OracleDbType.NVarchar2, data.P_SCLIENT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        GetClientDetVM.ClientDet suggest = new GetClientDetVM.ClientDet();
                        suggest.NTYPCLIENTDOC = reader["NTYPCLIENTDOC"] == DBNull.Value ? 0 : int.Parse(reader["NTYPCLIENTDOC"].ToString());
                        suggest.SCLINUMDOCU = reader["SCLINUMDOCU"] == DBNull.Value ? string.Empty : reader["SCLINUMDOCU"].ToString();
                        suggest.SCLIENAME = reader["SCLIENAME"] == DBNull.Value ? string.Empty : reader["SCLIENAME"].ToString();
                        suggest.NAME = reader["NAME"] == DBNull.Value ? string.Empty : reader["NAME"].ToString();
                        suggest.APELLIDO_P = reader["APELLIDO_P"] == DBNull.Value ? string.Empty : reader["APELLIDO_P"].ToString();
                        
                        suggest.APELLIDO_M = reader["APELLIDO_M"] == DBNull.Value ? string.Empty : reader["APELLIDO_M"].ToString();
                        suggest.DBIRTHDAT = reader["DBIRTHDAT"] == DBNull.Value ? string.Empty : Convert.ToDateTime(reader["DBIRTHDAT"]).ToString("yyyy-MM-dd");
                        suggest.SSEXCLIEN = reader["SSEXCLIEN"] == DBNull.Value ? 0 : int.Parse(reader["SSEXCLIEN"].ToString());
                        suggest.NCIVILSTA = reader["NCIVILSTA"] == DBNull.Value ? 0 : int.Parse(reader["NCIVILSTA"].ToString());
                        suggest.NNATIONALITY = reader["NNATIONALITY"] == DBNull.Value ? 0 : int.Parse(reader["NNATIONALITY"].ToString());

                        suggest.SSTRET = reader["SSTRET"] == DBNull.Value ? string.Empty : reader["SSTRET"].ToString();
                        suggest.SPHONE = reader["SPHONE"] == DBNull.Value ? string.Empty : reader["SPHONE"].ToString();
                        suggest.SMAIL = reader["SMAIL"] == DBNull.Value ? string.Empty : reader["SMAIL"].ToString();
                        suggest.SCLIENT = reader["SCLIENT"] == DBNull.Value ? string.Empty : reader["SCLIENT"].ToString();
                        suggest.SNAME_CONTACT = reader["SNAME_CONTACT"] == DBNull.Value ? string.Empty : reader["SNAME_CONTACT"].ToString();
                        suggest.SMAIL_CONTACT = reader["SMAIL_CONTACT"] == DBNull.Value ? string.Empty : reader["SMAIL_CONTACT"].ToString();
                        suggest.SPHONE_CONTACT = reader["SPHONE_CONTACT"] == DBNull.Value ? string.Empty : reader["SPHONE_CONTACT"].ToString();

                        entities.P_LIST.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("GetClientDetInmobiliaria", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<GetClientDetVM>(entities);
        }

        // INSERTAR CLIENTE
        public Task<InsClientVM> InsertarClienteInmobiliaria(InsClientBM data)
        {
            var parameters = new List<OracleParameter>();
            InsClientVM entities = new InsClientVM();
            string procedure = ProcedureName.pkg_inmobiliaryMaintenance + ".SPS_INSCRE_CLIENT_DET";

            try
            {
                parameters.Add(new OracleParameter("P_NOPCION", OracleDbType.Int64, data.P_NOPCION, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NIDTRXEMISION", OracleDbType.Int64, data.P_NIDTRXEMISION, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SCOD_INTERNO", OracleDbType.NVarchar2, data.P_SCOD_INTERNO, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SCLIENT_OLD", OracleDbType.NVarchar2, data.P_SCLIENT_OLD, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NTYPCLIENTDOC", OracleDbType.Int64, data.P_NTYPCLIENTDOC, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SCLINUMDOCU", OracleDbType.NVarchar2, data.P_SCLINUMDOCU, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SCLIENAME", OracleDbType.NVarchar2, data.P_SCLIENAME, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SFIRSTNAME", OracleDbType.NVarchar2, data.P_SFIRSTNAME, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SLASTNAME", OracleDbType.NVarchar2, data.P_SLASTNAME, ParameterDirection.Input));

                parameters.Add(new OracleParameter("P_SLASTNAME2", OracleDbType.NVarchar2, data.P_SLASTNAME2, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DBIRTHDAT", OracleDbType.Date, data.P_DBIRTHDAT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SSEXCLIEN", OracleDbType.NVarchar2, data.P_SSEXCLIEN, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCIVILSTA", OracleDbType.Int64, data.P_NCIVILSTA, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NNATIONALITY", OracleDbType.Int64, data.P_NNATIONALITY, ParameterDirection.Input));

                parameters.Add(new OracleParameter("P_NPROVINCE", OracleDbType.Int64, data.P_NPROVINCE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NLOCAL", OracleDbType.Int64, data.P_NLOCAL, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NMUNICIPALITY", OracleDbType.Int64, data.P_NMUNICIPALITY, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SMANZANA", OracleDbType.NVarchar2, data.P_SMANZANA, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SLOTE", OracleDbType.NVarchar2, data.P_SLOTE, ParameterDirection.Input));

                parameters.Add(new OracleParameter("P_SREFERENCIA", OracleDbType.NVarchar2, data.P_SREFERENCIA, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_STI_DIRE", OracleDbType.NVarchar2, data.P_STI_DIRE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SNUM_DIRECCION", OracleDbType.NVarchar2, data.P_SNUM_DIRECCION, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SNUM_INTERIOR", OracleDbType.NVarchar2, data.P_SNUM_INTERIOR, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SNOM_DIRECCION", OracleDbType.NVarchar2, data.P_SNOM_DIRECCION, ParameterDirection.Input));

                parameters.Add(new OracleParameter("P_SPHONE", OracleDbType.NVarchar2, data.P_SPHONE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SE_MAIL", OracleDbType.NVarchar2, data.P_SE_MAIL, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SNAME_CONTACT", OracleDbType.NVarchar2, data.P_SNAME_CONTACT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SMAIL_CONTACT", OracleDbType.NVarchar2, data.P_SMAIL_CONTACT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SPHONE_CONTACT", OracleDbType.NVarchar2, data.P_SPHONE_CONTACT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int64, data.P_NUSERCODE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCOMMIT", OracleDbType.Int64, data.P_NCOMMIT, ParameterDirection.Input));

                OracleParameter P_SCLIENT = new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_SCLIENT);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureInmo(procedure, parameters);
                entities.P_SCLIENT = P_SCLIENT.Value.ToString();
                entities.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                entities.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("InsertarClienteInmobiliaria", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<InsClientVM>(entities);
        }


        // TIPO VIA
        public Task<List<TipoViaVM>> GetTipoVias()
        {
            var parameters = new List<OracleParameter>();
            var response = new List<TipoViaVM>();
            string procedure = ProcedureName.pkg_inmobiliaryMaintenance + ".SPS_GET_LIST_TIP_DIRECCION";
            try
            {
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        TipoViaVM item = new TipoViaVM();
                        item.STI_DIRE = reader["STI_DIRE"] == DBNull.Value ? string.Empty : reader["STI_DIRE"].ToString();
                        item.SDESCRIPT = reader["SDESCRIPT"] == DBNull.Value ? string.Empty : reader["SDESCRIPT"].ToString();
                        response.Add(item);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarTipoDocumentoInmobiliaria", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<List<TipoViaVM>>(response);
        }

        //DEPARTAMENTOS
        public Task<List<DepartamentoVM>> GetDepartamentos()
        {
            var parameters = new List<OracleParameter>();
            var response = new List<DepartamentoVM>();
            string procedure = ProcedureName.pkg_inmobiliaryMaintenance + ".SPS_GET_LIST_DEPARTAMENT";
            try
            {
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        DepartamentoVM item = new DepartamentoVM();
                        item.NPROVINCE = reader["NPROVINCE"] == DBNull.Value ? 0 : int.Parse(reader["NPROVINCE"].ToString());
                        item.SDESCRIPT = reader["SDESCRIPT"] == DBNull.Value ? string.Empty : reader["SDESCRIPT"].ToString();
                        response.Add(item);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarTipoDocumentoInmobiliaria", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<List<DepartamentoVM>>(response);
        }

        //PROVINCIAS
        public Task<List<ProvinciaVM>> GetProvincias(int P_NPROVINCE)
        {
            var parameters = new List<OracleParameter>();
            var response = new List<ProvinciaVM>();
            string procedure = ProcedureName.pkg_inmobiliaryMaintenance + ".SPS_GET_LIST_PROVINCE";
            try
            {
                parameters.Add(new OracleParameter("P_NPROVINCE", OracleDbType.Int32, P_NPROVINCE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ProvinciaVM item = new ProvinciaVM();
                        item.NLOCAL = reader["NLOCAL"] == DBNull.Value ? 0 : int.Parse(reader["NLOCAL"].ToString());
                        item.SDESCRIPT = reader["SDESCRIPT"] == DBNull.Value ? string.Empty : reader["SDESCRIPT"].ToString();
                        response.Add(item);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarTipoDocumentoInmobiliaria", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<List<ProvinciaVM>>(response);
        }

        //DISTRITO
        public Task<List<DistritoVM>> GetDistrito(int P_NLOCAL)
        {
            var parameters = new List<OracleParameter>();
            var response = new List<DistritoVM>();
            string procedure = ProcedureName.pkg_inmobiliaryMaintenance + ".SPS_GET_LIST_MUNICIPALITY";
            try
            {
                parameters.Add(new OracleParameter("P_NLOCAL", OracleDbType.Int32, P_NLOCAL, ParameterDirection.Input));
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                var reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        DistritoVM item = new DistritoVM();
                        item.NMUNICIPALITY = reader["NMUNICIPALITY"] == DBNull.Value ? 0 : int.Parse(reader["NMUNICIPALITY"].ToString());
                        item.SDESCRIPT = reader["SDESCRIPT"] == DBNull.Value ? string.Empty : reader["SDESCRIPT"].ToString();
                        response.Add(item);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarTipoDocumentoInmobiliaria", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<List<DistritoVM>>(response);
        }

        // VALIDACION FACTURACION
        public Task<ExistBillsVM> GetExistsBills(int P_NBRANCH, int P_NPRODUCT, int P_NPOLICY)
        {
            var parameter = new List<OracleParameter>();
            var response = new ExistBillsVM();
            string procedure = ProcedureName.pkg_inmobiliaryMaintenance + ".SPS_GET_EXIST_BILLS";
            try
            {
                //INPUT                
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int64, P_NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int64, P_NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int64, P_NPOLICY, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_NVALOR = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                parameter.Add(P_NVALOR);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureInmo(procedure, parameter);
                response.P_NVALOR = Convert.ToInt32(P_NVALOR.Value.ToString());
                ELog.CloseConnection(odr); 

            }
            catch (Exception ex)
            {
                LogControl.save("ListarTipoDocumentoInmobiliaria", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<ExistBillsVM>(response);
        }

        //Detalle Direccion
        public Task<AddressClient> GetDetalleDireccion(string P_SCLIENT)
        {
            var parameter = new List<OracleParameter>();
            var response = new AddressClient(); 
            string procedure = ProcedureName.pkg_inmobiliaryMaintenance + ".SPS_GET_ADDRESS_DET";
            try
            {
                //parameters.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, P_SCLIENT, ParameterDirection.Input));
                //parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));
                //OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                //OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                //parameters.Add(P_NCODE);
                //parameters.Add(P_SMESSAGE);
                //var reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo(procedure, parameters);



                //INPUT                
                parameter.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, P_SCLIENT, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("P_CTABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                parameter.Add(C_TABLE);
                parameter.Add(P_NCODE);
                parameter.Add(P_MESSAGE);
                OracleDataReader reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo(procedure, parameter);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        response.NDEPARTMENT = reader["NDEPARTMENT"] == DBNull.Value ? 0 : int.Parse(reader["NDEPARTMENT"].ToString());
                        response.NPROVINCE = reader["NPROVINCE"] == DBNull.Value ? 0 : int.Parse(reader["NPROVINCE"].ToString());
                        response.NDISTRITO = reader["NDISTRITO"] == DBNull.Value ? 0 : int.Parse(reader["NDISTRITO"].ToString());

                        response.SCLIENT = reader["SCLIENT"] == DBNull.Value ? string.Empty : reader["SCLIENT"].ToString();
                        response.SKEYADDRESS = reader["SKEYADDRESS"] == DBNull.Value ? string.Empty : reader["SKEYADDRESS"].ToString();
                        response.STI_DIRE = reader["STI_DIRE"] == DBNull.Value ? string.Empty : reader["STI_DIRE"].ToString();
                        response.SNOM_DIRECCION = reader["SNOM_DIRECCION"] == DBNull.Value ? string.Empty : reader["SNOM_DIRECCION"].ToString();
                        response.SNUM_DIRECCION = reader["SNUM_DIRECCION"] == DBNull.Value ? string.Empty : reader["SNUM_DIRECCION"].ToString();
                        response.SMANZANA = reader["SMANZANA"] == DBNull.Value ? string.Empty : reader["SMANZANA"].ToString();
                        response.SLOTE = reader["SLOTE"] == DBNull.Value ? string.Empty : reader["SLOTE"].ToString();
                        response.SREFERENCIA = reader["SREFERENCIA"] == DBNull.Value ? string.Empty : reader["SREFERENCIA"].ToString();
                        response.SNUM_INTERIOR = reader["SNUM_INTERIOR"] == DBNull.Value ? string.Empty : reader["SNUM_INTERIOR"].ToString();
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarTipoDocumentoInmobiliaria", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<AddressClient>(response);
        }


        // ACTUALIZAR CLIENTE
        public Task<UpdClientVM> ActualizarClienteInmobiliaria(UpdClientBM data)
        {
            var parameters = new List<OracleParameter>();
            UpdClientVM entities = new UpdClientVM();
            string procedure = ProcedureName.pkg_inmobiliaryMaintenance + ".SPS_INSUPD_MANT_CLIENTE";

            var fIni = Convert.ToDateTime(data.P_DSTARTDATE);
            var fFin = Convert.ToDateTime(data.P_DEXPIRDAT);

            try
            {
                parameters.Add(new OracleParameter("P_NOPCION", OracleDbType.Int64, data.P_NOPCION, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NIDTRXEMISION", OracleDbType.Int64, data.P_NIDTRXEMISION, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SCOD_INTERNO", OracleDbType.NVarchar2, data.P_SCOD_INTERNO, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int64, data.P_NUSERCODE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SCLIENT_OLD", OracleDbType.NVarchar2, data.P_SCLIENT_OLD, ParameterDirection.Input));

                parameters.Add(new OracleParameter("P_NTYPCLIENTDOC", OracleDbType.Int64, data.P_NTYPCLIENTDOC, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SCLINUMDOCU", OracleDbType.NVarchar2, data.P_SCLINUMDOCU, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NTYPCLIENTDOC_NEW", OracleDbType.Int64, data.P_NTYPCLIENTDOC_NEW, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SCLINUMDOCU_NEW", OracleDbType.NVarchar2, data.P_SCLINUMDOCU_NEW, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SCLIENAME_NEW", OracleDbType.NVarchar2, data.P_SCLIENAME_NEW, ParameterDirection.Input));

                parameters.Add(new OracleParameter("P_SFIRSTNAME_NEW", OracleDbType.NVarchar2, data.P_SFIRSTNAME_NEW, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SLASTNAME_NEW", OracleDbType.NVarchar2, data.P_SLASTNAME_NEW, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SLASTNAME2_NEW", OracleDbType.NVarchar2, data.P_SLASTNAME2_NEW, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SCLIENT_NEW", OracleDbType.NVarchar2, data.P_SCLIENT_NEW, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SNAME_CONTACT", OracleDbType.NVarchar2, data.P_SNAME_CONTACT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SMAIL_CONTACT", OracleDbType.NVarchar2, data.P_SMAIL_CONTACT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SPHONE_CONTACT", OracleDbType.NVarchar2, data.P_SPHONE_CONTACT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPROVINCE", OracleDbType.Int64, data.P_NPROVINCE, ParameterDirection.Input));

                parameters.Add(new OracleParameter("P_NLOCAL", OracleDbType.Int64, data.P_NLOCAL, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NMUNICIPALITY", OracleDbType.Int64, data.P_NMUNICIPALITY, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SNOM_DIRECCION", OracleDbType.NVarchar2, data.P_SNOM_DIRECCION, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SMANZANA", OracleDbType.NVarchar2, data.P_SMANZANA, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SLOTE", OracleDbType.NVarchar2, data.P_SLOTE, ParameterDirection.Input));

                parameters.Add(new OracleParameter("P_SREFERENCIA", OracleDbType.NVarchar2, data.P_SREFERENCIA, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_STI_DIRE", OracleDbType.NVarchar2, data.P_STI_DIRE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SNUM_DIRECCION", OracleDbType.NVarchar2, data.P_SNUM_DIRECCION, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SNUM_INTERIOR", OracleDbType.NVarchar2, data.P_SNUM_INTERIOR, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NTYPE_DATE", OracleDbType.Int64, data.P_NTYPE_DATE, ParameterDirection.Input));

                parameters.Add(new OracleParameter("P_DSTARTDATE", OracleDbType.Date, fIni , ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DEXPIRDAT", OracleDbType.Date, fFin, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SGLOSA_COMPROB", OracleDbType.NVarchar2, data.P_SGLOSA_COMPROB, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPREMIUM", OracleDbType.Int64, data.P_NPREMIUM, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_STIP_EMISION", OracleDbType.NVarchar2, data.P_STIP_EMISION, ParameterDirection.Input));

                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureInmo(procedure, parameters);
                entities.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                entities.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("ActualizarClienteInmobiliaria", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<UpdClientVM>(entities);
        }

        // OBTENER PAGOS
        public Task<PayDateVM> PayDateInmob(PayDateBM data)
        {
            var parameters = new List<OracleParameter>();
            PayDateVM entities = new PayDateVM();
            string procedure = ProcedureName.pkg_inmobiliaryMaintenance + ".SPS_GET_VALIDATE_DATE";
            try
            {
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int16, data.P_NBRANCH, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int16, data.P_NPRODUCT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPOLICY", OracleDbType.NVarchar2, data.P_NPOLICY, ParameterDirection.Input));
                OracleParameter P_VALOR = new OracleParameter("P_VALOR", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                parameters.Add(P_VALOR);
                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureInmo(procedure, parameters);
                entities.P_VALOR = Convert.ToInt32(P_VALOR.Value.ToString());
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("AgregarInterfaz", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<PayDateVM>(entities);
        }
    }
}