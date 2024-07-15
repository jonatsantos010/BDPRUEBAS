using System;
using System.Collections.Generic;
using WSPlataforma.Util;
using WSPlataforma.Entities.ViewModel;
using WSPlataforma.Entities.ReporteSucaveModel;
using WSPlataforma.Entities.ReporteSucaveModel.ViewModel;
using WSPlataforma.Entities.ReporteSucaveModel.BindingModel;
using Oracle.DataAccess.Client;
using System.Data;
using System.Configuration;
using WSPlataforma.Entities.AccountStateModel.ViewModel;
using WSPlataforma.Entities.ReportModel.BindingModel;
using WSPlataforma.DA;
using System.Threading.Tasks;
namespace WSPlataforma.DA
{

    public class ReporteSucaveDA : ConnectionBase
    {

        #region Reporte Sucave
        //obtiene ramo
        public List<ReporteSoatVM> GetBranch()
        {
            var parameters = new List<OracleParameter>();
            var suggests = new List<ReporteSoatVM>();
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_ReporteSucaveSCTR, "REA_LIST_BRANCH");

            try
            {
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var suggest = new ReporteSoatVM();
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

        //obtiene producto
        public List<ReporteSoatVM> GetProduct(ProductoFilter data)
        {
            var validations = new List<ReporteSoatVM>();

            try
            {
                var parameters = new List<OracleParameter>();
                var procedure = "REA_LIST_PRODUCT";

                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.branch, ParameterDirection.Input));
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(string.Format("{0}.{1}", ProcedureName.pkg_ReporteSucaveSCTR, procedure), parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var validation = new ReporteSoatVM();
                        validation.Id = reader["COD_PRODUCT"] == DBNull.Value ? 0 : int.Parse(reader["COD_PRODUCT"].ToString());
                        validation.Description = reader["DES_PRODUCT"] == DBNull.Value ? string.Empty : reader["DES_PRODUCT"].ToString();

                        validations.Add(validation);

                    }
                }

                reader.Close();

            }
            catch (Exception ex)
            {
                LogControl.save("GetProduct", ex.ToString(), "3");
                throw;
            }
            return validations;
        }

        //inserta tabla status de reporte
        public int InsertReportStatus(ReporteSoatFilter data)
        {
            try
            {
                ResponseControl generic = new ResponseControl(Response.Ok);

                var invoices = new List<ReporteSoatBM>();
                var parameters = new List<OracleParameter>();
                var procedure = "SPS_GENERAR_REPORTES_SUCAVE_SCTR";
                //string fechaInicio = data.StartDate.ToString("dd/MM/yyyy");
                //string fechaFin = data.EndDate.ToString("dd/MM/yyyy");

                //parameters.Add(new OracleParameter("P_IDPROCESS", OracleDbType.Varchar2, data.IdProcess, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.BranchId, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, data.ProductId, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, data.NUserCode, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_USER_REG", OracleDbType.Varchar2, data.UserName.ToUpper(), ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_FEINI", OracleDbType.Varchar2, data.FechaIni, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_FEFIN", OracleDbType.Varchar2, data.FechaFin, ParameterDirection.Input));

                //Ncode controla los errores
                var IdHeaderProcess = new OracleParameter("P_NIDHEADERPROC", OracleDbType.Int32, ParameterDirection.Output);
                var NCode = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);
                var SMessage = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                parameters.Add(IdHeaderProcess);
                parameters.Add(NCode);
                parameters.Add(SMessage);
                IdHeaderProcess.Size = 4000;
                NCode.Size = 4000;
                SMessage.Size = 4000;

                this.ExecuteByStoredProcedureVT(string.Format("{0}.{1}", ProcedureName.pkg_ReporteSucaveSCTR, procedure), parameters);

                var PIdHeaderProcess = Convert.ToInt32(IdHeaderProcess.Value.ToString());
                var PCode = Convert.ToInt32(NCode.Value.ToString());
                var PMessage = (SMessage.Value.ToString());
                data.HeaderId = PIdHeaderProcess;
                data.Mensaje = PMessage;

                if (PCode == 1)
                {
                    generic.Message = SMessage.Value.ToString();
                    generic.response = Response.Fail;
                }

                return PCode;
            }
            catch (Exception ex)
            {
                LogControl.save("InsertReportStatus", ex.ToString(), "3");
                throw;
            }
        }

        //obtiene lista del status de reportes
        public List<ReporteSoatBM> GetReportStatus(ReporteSoatFilter data)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<ReporteSoatBM> ListProcess = new List<ReporteSoatBM>();
            string storedProcedureName = ProcedureName.pkg_ReporteSucaveSCTR + ".SPS_LIST_CREATED_REPORTS_SUCAVE";

            //string fechaInicio = data.StartDate.ToString("dd/MM/yyyy");
            //string fechaFin = data.EndDate.ToString("dd/MM/yyyy");

            try
            {
                //INPUT                
                parameter.Add(new OracleParameter("P_NIDHEADERPROC", OracleDbType.Int32, data.HeaderId, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FEINI", OracleDbType.NVarchar2, data.FechaIni, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FEFIN", OracleDbType.NVarchar2, data.FechaFin, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.BranchId, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_IDPRODUCT", OracleDbType.Int32, data.ProductId, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        ReporteSoatBM item = new ReporteSoatBM();
                        item.IdHeaderProcess = odr["NIDHEADERPROC"] == DBNull.Value ? string.Empty : odr["NIDHEADERPROC"].ToString();
                        item.IdIdentity = odr["NIDENTITY"] == DBNull.Value ? string.Empty : odr["NIDENTITY"].ToString();
                        item.IdProduct = odr["NPRODUCT"] == DBNull.Value ? string.Empty : odr["NPRODUCT"].ToString();
                        item.DescriptionProduct = odr["DES_PRODUCT"] == DBNull.Value ? string.Empty : odr["DES_PRODUCT"].ToString();
                        item.StatusProc = (Status)(Int32.Parse(odr["NSTATUSPROC"].ToString()));
                        item.DescriptionStatus = odr["SDESCRIPTION"] == DBNull.Value ? string.Empty : odr["SDESCRIPTION"].ToString();
                        item.UserCode = odr["NUSERCODE"] == DBNull.Value ? string.Empty : odr["NUSERCODE"].ToString();
                        item.DIniProcess = odr["DREGINIPROD"] == DBNull.Value ? string.Empty : odr["DREGINIPROD"].ToString();
                        item.DFinProcess = odr["DREGFINPROD"] == DBNull.Value ? string.Empty : odr["DREGFINPROD"].ToString();
                        item.DescriptionIdentity = odr["SDESCRIPT"] == DBNull.Value ? string.Empty : odr["SDESCRIPT"].ToString();
                        item.DescriptionFile = odr["SFILENAME"] == DBNull.Value ? string.Empty : odr["SFILENAME"].ToString();
                        item.DescriptionBranch = odr["SBRANCH"] == DBNull.Value ? string.Empty : odr["SBRANCH"].ToString();
                        item.SDescript = odr["SDESCRIPT"] == DBNull.Value ? string.Empty : odr["SDESCRIPT"].ToString();
                        item.mensajeAlerta = data.Mensaje;

                        ListProcess.Add(item);
                    }
                }
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                LogControl.save("GetReportStatus", ex.ToString(), "3");
            }


            return ListProcess;
        }


        #endregion

        #region Consulta Reporte Sucave

        public ResponseVM GenerateFact(int NIDHEADERPROC, int NIDDETAILPROC, int NUSERCODE)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseVM response = new ResponseVM();
            string storedProcedureName = ProcedureName.sp_CreaFactura;
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NIDHEADERPROC", OracleDbType.Int32, NIDHEADERPROC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDDETAILPROC", OracleDbType.Int32, NIDDETAILPROC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, NUSERCODE, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, response.P_NCODE, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, response.P_SMESSAGE, ParameterDirection.Output);

                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;
                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);

                this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                response.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                response.P_SMESSAGE = P_SMESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                LogControl.save("GenerateFact", ex.ToString(), "3");
            }


            return response;
        }

        public List<ProcessHeaderBM> GetProcessHeader(ReporteVisualizarFilter data)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<ProcessHeaderBM> ListProcess = new List<ProcessHeaderBM>();
            string storedProcedureName = ProcedureName.pkg_ReporteSucaveSCTR + ".SPS_LIST_HEADER_REPORTS_SUCAVE";

            string fechaInicio = data.StartDate.ToString("dd/MM/yyyy");
            string fechaFin = data.EndDate.ToString("dd/MM/yyyy");

            try
            {
                //INPUT                
                //parameter.Add(new OracleParameter("P_FEINI", OracleDbType.Date, Convert.ToDateTime(fechaInicio), ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FEINI", OracleDbType.NVarchar2, fechaInicio, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_FEFIN", OracleDbType.NVarchar2, fechaFin, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.branch, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_IDPRODUCT", OracleDbType.Int32, data.product, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        ProcessHeaderBM item = new ProcessHeaderBM();
                        item.IdHeaderProcess = odr["NIDHEADERPROC"] == DBNull.Value ? string.Empty : odr["NIDHEADERPROC"].ToString();
                        item.IdIdentity = odr["NIDENTITY"] == DBNull.Value ? string.Empty : odr["NIDENTITY"].ToString();
                        item.IdProduct = odr["NPRODUCT"] == DBNull.Value ? string.Empty : odr["NPRODUCT"].ToString();
                        item.DescriptionProduct = odr["DES_PRODUCT"] == DBNull.Value ? string.Empty : odr["DES_PRODUCT"].ToString();
                        item.StatusProc = (Status)(Int32.Parse(odr["NSTATUSPROC"].ToString()));
                        item.DescriptionStatus = odr["SDESCRIPTION"] == DBNull.Value ? string.Empty : odr["SDESCRIPTION"].ToString();
                        item.UserCode = odr["NUSERCODE"] == DBNull.Value ? string.Empty : odr["NUSERCODE"].ToString();
                        item.DIniProcess = odr["DREGINIPROD"] == DBNull.Value ? string.Empty : odr["DREGINIPROD"].ToString();
                        item.DFinProcess = odr["DREGFINPROD"] == DBNull.Value ? string.Empty : odr["DREGFINPROD"].ToString();
                        item.DescriptionIdentity = odr["SDESCRIPT"] == DBNull.Value ? string.Empty : odr["SDESCRIPT"].ToString();
                        item.DescriptionFile = odr["SFILENAME"] == DBNull.Value ? string.Empty : odr["SFILENAME"].ToString();
                        item.DescriptionBranch = odr["SBRANCH"] == DBNull.Value ? string.Empty : odr["SBRANCH"].ToString();
                        item.SDescript = odr["SDESCRIPT"] == DBNull.Value ? string.Empty : odr["SDESCRIPT"].ToString();
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


        public List<ProcessDetailBM> GetProcessDetail(int IdHeaderProcess)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<ProcessDetailBM> ListProcessDetail = new List<ProcessDetailBM>();
            string storedProcedureName = ProcedureName.pkg_CargaMasivaTrama + ".SPS_LIST_DETAILPROC";
            try
            {
                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = cn;

                        IDataReader odr = null;

                        cmd.CommandText = string.Format("{0}.{1}", ProcedureName.pkg_ReporteSucaveSCTR, "SPS_LIST_DETAIL_REPORTS_SUCAVE");

                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new OracleParameter("P_NIDHEADERPROC", OracleDbType.Int32, IdHeaderProcess, ParameterDirection.Input));
                        cmd.Parameters.Add("C_TABLE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        cn.Open();
                        odr = cmd.ExecuteReader();

                        if (odr != null)
                        {
                            while (odr.Read())
                            {
                                ProcessDetailBM item = new ProcessDetailBM();
                                item.IdDetailProcess = odr["NIDDETAILPROC"] == DBNull.Value ? string.Empty : odr["NIDDETAILPROC"].ToString();
                                item.IdHeaderProcess = odr["NIDHEADERPROC"] == DBNull.Value ? string.Empty : odr["NIDHEADERPROC"].ToString();
                                item.IdStatusDetail = odr["NSTATUSPROC"] == DBNull.Value ? string.Empty : odr["NSTATUSPROC"].ToString();
                                item.IdFileConfig = odr["NIDFILEHEADER"] == DBNull.Value ? string.Empty : odr["NIDFILEHEADER"].ToString();
                                item.DescriptionStatus = odr["SDESCRIPTION"] == DBNull.Value ? string.Empty : odr["SDESCRIPTION"].ToString();
                                item.DIniDetail = odr["DREGINIDETAIL"] == DBNull.Value ? string.Empty : odr["DREGINIDETAIL"].ToString();
                                item.DFinDetail = odr["DREGFINDETAIL"] == DBNull.Value ? string.Empty : odr["DREGFINDETAIL"].ToString();
                                item.FileName = odr["SFILENAME"] == DBNull.Value ? string.Empty : odr["SFILENAME"].ToString();
                                item.table_reference = odr["STABLENAME"] == DBNull.Value ? string.Empty : odr["STABLENAME"].ToString();
                                ListProcessDetail.Add(item);
                            }
                        }
                        cn.Close();
                        cmd.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetProcessDetail", ex.ToString(), "3");
            }


            return ListProcessDetail;
        }

        public List<ProcessLogBM> GetProcessLogError(int IdDetailProcess, string Opcion)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<ProcessLogBM> ListProcessLog = new List<ProcessLogBM>();
            string storedProcedureName = ProcedureName.pkg_ReporteSucaveSCTR + ".SPS_LIST_LOG_REPORTS_SUCAVE";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NIDDETAILPROC", OracleDbType.Int32, IdDetailProcess, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TYPE_OBS", OracleDbType.Varchar2, Opcion, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        ProcessLogBM item = new ProcessLogBM();
                        item.IdLogProcess = odr["NIDLOGPROC"] == DBNull.Value ? string.Empty : odr["NIDLOGPROC"].ToString();
                        item.IdDetailProcess = odr["NIDDETAILPROC"] == DBNull.Value ? string.Empty : odr["NIDDETAILPROC"].ToString();
                        item.RecordFile = odr["NRECORDFILE"] == DBNull.Value ? string.Empty : odr["NRECORDFILE"].ToString();
                        item.NameColumn = odr["SNAME"] == DBNull.Value ? string.Empty : odr["SNAME"].ToString();
                        item.DescriptionError = odr["SDESCRIPT"] == DBNull.Value ? string.Empty : odr["SDESCRIPT"].ToString();
                        item.DateRegister = odr["DREGPROC"] == DBNull.Value ? string.Empty : odr["DREGPROC"].ToString();
                        item.IdFileHeader = odr["NIDFILEHEADER"] == DBNull.Value ? string.Empty : odr["NIDFILEHEADER"].ToString();
                        ListProcessLog.Add(item);
                    }
                }
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                LogControl.save("GetProcessLogError", ex.ToString(), "3");
            }


            return ListProcessLog;
        }

        public string GetAvanceDetailProcess(int IdDetailProcess, int IdHeaderProcess)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<ProcessLogBM> ListProcessLog = new List<ProcessLogBM>();
            string Porcent = "0";
            string storedProcedureName = ProcedureName.pkg_CargaMasivaTrama + ".SPS_GET_AVANCE_DETAIL";
            try
            {

                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = cn;

                        //IDataReader reader = null;

                        cmd.CommandText = string.Format("{0}.{1}", ProcedureName.pkg_ReporteSucaveSCTR, "SPS_GET_AVANCE_REPORTE_SUCAVE_DETAIL");

                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new OracleParameter("P_NIDHEADERPROC", OracleDbType.Int32, IdHeaderProcess, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NIDDETAILPROC", OracleDbType.Int32, IdDetailProcess, ParameterDirection.Input));
                        OracleParameter P_TOTAL = new OracleParameter("P_TOTAL", OracleDbType.Int32, 1000, ParameterDirection.Output);
                        cmd.Parameters.Add(P_TOTAL);
                        cn.Open();

                        cmd.ExecuteNonQuery();

                        Porcent = P_TOTAL.Value.ToString();

                        cn.Close();
                        cmd.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetAvanceDetailProcess", ex.ToString(), "3");
            }
            return Porcent;
        }

        public Task<Contratante> GetDataExport(int IdDetailProcess, string TableReference, int IdHeaderProcess)
        {

            List<OracleParameter> parameter = new List<OracleParameter>();
            Contratante entities = new Contratante();
            entities.lista = new List<Contratante.P_TABLE>();
            string storedProcedureName = ProcedureName.pkg_ReporteSucaveSCTR + ".SPS_EXPORTAR_DATA_SUCAVE";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NIDFILEHEADER", OracleDbType.Int32, IdDetailProcess, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TABLE_REFERENCE", OracleDbType.Varchar2, TableReference, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDHEADERPROC", OracleDbType.Int32, IdHeaderProcess, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                P_NCODE.Size = 100;
                P_MESSAGE.Size = 4000;

                parameter.Add(P_NCODE);
                parameter.Add(P_MESSAGE);
                parameter.Add(C_TABLE);

                OracleDataReader reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Contratante.P_TABLE suggest = new Contratante.P_TABLE();
                        suggest.FILA = reader["FILA"] == DBNull.Value ? string.Empty : reader["FILA"].ToString();
                        suggest.TIPODOC = reader["TIPODOC"] == DBNull.Value ? string.Empty : reader["TIPODOC"].ToString();
                        suggest.RUC = reader["RUC"] == DBNull.Value ? string.Empty : reader["RUC"].ToString();
                        suggest.RAZONSOCIAL = reader["RAZONSOCIAL"] == DBNull.Value ? string.Empty : reader["RAZONSOCIAL"].ToString();
                        suggest.CODACTIVIDADECONOMICA = reader["CODACTIVIDADECONOMICA"] == DBNull.Value ? string.Empty : reader["CODACTIVIDADECONOMICA"].ToString();
                        suggest.DOMICILIO = reader["DOMICILIO"] == DBNull.Value ? string.Empty : reader["DOMICILIO"].ToString();
                        suggest.POLIZA = reader["POLIZA"] == DBNull.Value ? string.Empty : reader["POLIZA"].ToString();
                        suggest.CANTIDAD_POLIZA = reader["CANTIDAD_POLIZA"] == DBNull.Value ? string.Empty : reader["CANTIDAD_POLIZA"].ToString();
                        suggest.DESCRIPT_ERROR = reader["DESCRIPT_ERROR"] == DBNull.Value ? string.Empty : reader["DESCRIPT_ERROR"].ToString();

                        entities.lista.Add(suggest);
                    }
                }
                reader.Close();

            }
            catch (Exception ex)
            {
                LogControl.save("GetDataExport", ex.ToString(), "3");
            }
            return Task.FromResult<Contratante>(entities);


        }


        public Task<Expuestos> GetDataExport2(int IdDetailProcess, string TableReference, int IdHeaderProcess)
        {


            List<OracleParameter> parameter = new List<OracleParameter>();
            Expuestos entities = new Expuestos();
            entities.lista = new List<Expuestos.P_TABLE>();
            string storedProcedureName = ProcedureName.pkg_ReporteSucaveSCTR + ".SPS_EXPORTAR_DATA_SUCAVE";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NIDFILEHEADER", OracleDbType.Int32, IdDetailProcess, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TABLE_REFERENCE", OracleDbType.Varchar2, TableReference, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDHEADERPROC", OracleDbType.Int32, IdHeaderProcess, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                P_NCODE.Size = 100;
                P_MESSAGE.Size = 4000;

                parameter.Add(P_NCODE);
                parameter.Add(P_MESSAGE);
                parameter.Add(C_TABLE);

                OracleDataReader reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Expuestos.P_TABLE suggest = new Expuestos.P_TABLE();
                        suggest.FILA = reader["FILA"] == DBNull.Value ? string.Empty : reader["FILA"].ToString();
                        suggest.POLIZA = reader["POLIZA"] == DBNull.Value ? string.Empty : reader["POLIZA"].ToString();
                        suggest.INICIO_VIGENCIA = reader["INICIO_VIGENCIA"] == DBNull.Value ? string.Empty : reader["INICIO_VIGENCIA"].ToString();
                        suggest.FIN_VIGENCIA = reader["FIN_VIGENCIA"] == DBNull.Value ? string.Empty : reader["FIN_VIGENCIA"].ToString();
                        suggest.MONEDA = reader["MONEDA"] == DBNull.Value ? string.Empty : reader["MONEDA"].ToString();
                        suggest.CERTIFICADO = reader["CERTIFICADO"] == DBNull.Value ? string.Empty : reader["CERTIFICADO"].ToString();
                        suggest.INICIO_COBERTURA = reader["INICIO_COBERTURA"] == DBNull.Value ? string.Empty : reader["INICIO_COBERTURA"].ToString();
                        suggest.FIN_COBERTURA = reader["FIN_COBERTURA"] == DBNull.Value ? string.Empty : reader["FIN_COBERTURA"].ToString();
                        suggest.NOMBRE = reader["NOMBRE"] == DBNull.Value ? string.Empty : reader["NOMBRE"].ToString();
                        suggest.NOMBRE2 = reader["NOMBRE2"] == DBNull.Value ? string.Empty : reader["NOMBRE2"].ToString();
                        suggest.APELLIDO_PATERNO = reader["APELLIDO_PATERNO"] == DBNull.Value ? string.Empty : reader["APELLIDO_PATERNO"].ToString();
                        suggest.APELLIDO_MATERNO = reader["APELLIDO_MATERNO"] == DBNull.Value ? string.Empty : reader["APELLIDO_MATERNO"].ToString();
                        suggest.TIPODOC = reader["TIPODOC"] == DBNull.Value ? string.Empty : reader["TIPODOC"].ToString();
                        suggest.DOCUMENTO = reader["DOCUMENTO"] == DBNull.Value ? string.Empty : reader["DOCUMENTO"].ToString();
                        suggest.SEXO = reader["SEXO"] == DBNull.Value ? string.Empty : reader["SEXO"].ToString();
                        suggest.FECHA_NACIMIENTO = reader["FECHA_NACIMIENTO"] == DBNull.Value ? string.Empty : reader["FECHA_NACIMIENTO"].ToString();
                        suggest.OBLIGATORIO_FACULTATIVO = reader["OBLIGATORIO_FACULTATIVO"] == DBNull.Value ? string.Empty : reader["OBLIGATORIO_FACULTATIVO"].ToString();
                        suggest.CODACTIVIDADECONOMICA = reader["CODACTIVIDADECONOMICA"] == DBNull.Value ? string.Empty : reader["CODACTIVIDADECONOMICA"].ToString();
                        suggest.DESCRIPT_ERROR = reader["DESCRIPT_ERROR"] == DBNull.Value ? string.Empty : reader["DESCRIPT_ERROR"].ToString();
                        entities.lista.Add(suggest);
                    }
                }
                reader.Close();

            }
            catch (Exception ex)
            {
                LogControl.save("GetDataExport2", ex.ToString(), "3");
            }
            return Task.FromResult<Expuestos>(entities);



        }

        public DataTable GetDataExportCorrect(int IdDetailProcess, string TableReference, int IdHeaderProcess)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            DataTable DataReturn = new DataTable();
            string storedProcedureName = ProcedureName.pkg_CargaMasivaTrama + ".SPS_EXPORTAR_DATA_CORRECT";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NIDFILECONFIG", OracleDbType.Int32, IdDetailProcess, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDHEADERPROC", OracleDbType.Int32, IdHeaderProcess, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                P_NCODE.Size = 100;
                P_MESSAGE.Size = 4000;

                parameter.Add(P_NCODE);
                parameter.Add(P_MESSAGE);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                if (odr != null)
                {
                    DataReturn.Load(odr);
                }
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                LogControl.save("GetDataExportCorrect", ex.ToString(), "3");
            }
            return DataReturn;
        }
        public List<EntityConfigBM> GetConfigurationEntity()
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<EntityConfigBM> ListEntityConfig = new List<EntityConfigBM>();
            string storedProcedureName = ProcedureName.pkg_CargaMasivaTrama + ".SPS_GET_CONFIGURATION_ENTITY";
            try
            {
                //INPUT
                // parameter.Add(new OracleParameter("P_NIDENTITY", OracleDbType.Int32, Identity, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        EntityConfigBM item = new EntityConfigBM();
                        item.IdEntity = Convert.ToInt16(odr["NIDENTITY"].ToString());
                        item.Description = odr["SDESCRIPT"].ToString();
                        item.ShortName = odr["SSHORTNAME"].ToString();
                        item.Status = Convert.ToInt16(odr["NSTATUS"].ToString());
                        item.Branch = Convert.ToInt16(odr["NBRANCH"].ToString());
                        ListEntityConfig.Add(item);
                    }
                }
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                LogControl.save("GetConfigurationEntity", ex.ToString(), "3");
            }


            return ListEntityConfig;
        }

        public List<PathConfigBM> getPathConfig(int Identity)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<PathConfigBM> ListPathConfig = new List<PathConfigBM>();
            string storedProcedureName = ProcedureName.pkg_CargaMasivaTrama + ".SPS_GET_CONFIGURATION_PATH";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NIDENTITY", OracleDbType.Int32, Identity, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        PathConfigBM item = new PathConfigBM();
                        item.IdPathConfig = Convert.ToInt16(odr["NIDPATHCONFIG"].ToString());
                        item.RouteOrigin = odr["SFILEPATHORI"].ToString();
                        item.RouteDestine = odr["SFILEPATHDES"].ToString();
                        item.Extension = odr["SEXTENSION"].ToString();
                        item.Pattern = odr["SPATTERN"].ToString();
                        item.IdEntity = Convert.ToInt16(odr["NIDENTITY"].ToString());
                        item.IdProduct = Convert.ToInt16(odr["NPRODUCT"].ToString());
                        item.Routereprocessing = odr["SFILEPATHREP"].ToString();
                        item.Status = odr["NSTATUS"].ToString();
                        ListPathConfig.Add(item);
                    }
                }
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                LogControl.save("getPathConfig", ex.ToString(), "3");
            }


            return ListPathConfig;
        }

        public List<PathConfigBM> GetConfigurationPath(int Identity)
        {
            List<PathConfigBM> ListpathConfig = new List<PathConfigBM>();

            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = cn;

                    IDataReader reader = null;

                    cmd.CommandText = string.Format("{0}.{1}", ProcedureName.pkg_CargaMasivaTrama, "SPS_GET_CONFIGURATION_PATH");

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("P_NIDENTITY", OracleDbType.Int32).Value = Identity;
                    cmd.Parameters.Add("C_TABLE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    cn.Open();

                    reader = cmd.ExecuteReader();

                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            PathConfigBM pathConfig = new PathConfigBM();
                            pathConfig.IdPathConfig = Convert.ToInt16(reader["NIDPATHCONFIG"].ToString());
                            pathConfig.RouteOrigin = reader["SFILEPATHORI"].ToString();
                            pathConfig.RouteDestine = reader["SFILEPATHDES"].ToString();
                            pathConfig.Extension = reader["SEXTENSION"].ToString();
                            pathConfig.Pattern = reader["SPATTERN"].ToString();
                            pathConfig.IdEntity = Convert.ToInt16(reader["NIDENTITY"].ToString());
                            pathConfig.IdProduct = Convert.ToInt16(reader["NPRODUCT"].ToString());
                            pathConfig.Status = reader["NSTATUS"].ToString();
                            ListpathConfig.Add(pathConfig);
                        }
                    }
                    cn.Close();
                    cmd.Dispose();
                }
            }
            return ListpathConfig;
        }

        public GenericPackageVM InsertHeaderProc(ProcessHeaderBM entitie)
        {
            ProcessHeaderBM headerProcess = (ProcessHeaderBM)entitie;
            GenericPackageVM generic = new GenericPackageVM();
            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = cn;

                    //IDataReader reader = null;

                    cmd.CommandText = string.Format("{0}.{1}", ProcedureName.pkg_CargaMasivaTrama, "SPS_INS_CAB_PROCESS");

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("P_NIDENTITY", OracleDbType.Int32).Value = headerProcess.IdIdentity;
                    cmd.Parameters.Add("P_NPRODUCT", OracleDbType.Int32).Value = headerProcess.IdProduct;
                    cmd.Parameters.Add("P_NSTATUSPROC", OracleDbType.Int32).Value = headerProcess.StatusProc;
                    cmd.Parameters.Add("P_NUSERCODE", OracleDbType.Int32).Value = headerProcess.UserCode;
                    var idProcess = new OracleParameter("P_IDHEADERPROC", OracleDbType.Int32, ParameterDirection.Output);

                    cmd.Parameters.Add(idProcess);
                    cn.Open();

                    cmd.ExecuteNonQuery();
                    generic.GenericResponse = Convert.ToInt32(idProcess.Value.ToString());
                    cmd.Dispose();
                }
            }
            return generic;
        }

        public GenericPackageVM InsertDetailProc(ProcessDetailBM entitie)
        {
            ProcessDetailBM DetailProcess = entitie;
            GenericPackageVM generic = new GenericPackageVM();
            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = cn;

                    //IDataReader reader = null;

                    cmd.CommandText = string.Format("{0}.{1}", ProcedureName.pkg_CargaMasivaTrama, "SPS_INS_DETAIL_PROCESS");

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("P_NIDHEADERPROC", OracleDbType.Int32).Value = DetailProcess.IdHeaderProcess;
                    cmd.Parameters.Add("P_NSTATUSPROC", OracleDbType.Int32).Value = DetailProcess.Status;
                    cmd.Parameters.Add("P_NIDFILECONFIG", OracleDbType.Int32).Value = DetailProcess.IdFileConfig;
                    var idDetailProcess = new OracleParameter("P_NIDDETAILPROC", OracleDbType.Int32, ParameterDirection.Output);
                    cmd.Parameters.Add(idDetailProcess);
                    cn.Open();

                    cmd.ExecuteNonQuery();
                    generic.GenericResponse = Convert.ToInt32(idDetailProcess.Value.ToString());
                    cmd.Dispose();
                }
            }
            return generic;
        }

        public List<FileConfigBM> GetConfigurationFiles(int idPath)
        {

            List<FileConfigBM> ListFileConfig = new List<FileConfigBM>();

            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = cn;

                    IDataReader reader = null;

                    cmd.CommandText = string.Format("{0}.{1}", ProcedureName.pkg_CargaMasivaTrama, "SPS_GET_CONFIGURATION_FILE");
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("P_NIDPATHCONFIG", OracleDbType.Int32).Value = idPath;
                    cmd.Parameters.Add("C_TABLE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    cn.Open();

                    reader = cmd.ExecuteReader();

                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            FileConfigBM fileConfig = new FileConfigBM();
                            fileConfig.IdFileConfig = Convert.ToInt32(reader["NIDFILECONFIG"].ToString());
                            fileConfig.FileName = reader["SFILENAME"].ToString();
                            fileConfig.Pattern = reader["SPATTERN"].ToString();
                            fileConfig.FileRequired = reader["NFILEREQUIRED"].ToString();
                            fileConfig.FileRequiredLoad = reader["NLOADREQUIRED"].ToString();
                            fileConfig.Order = (!DBNull.Value.Equals(reader["NORDEN"])) ? Convert.ToInt16(reader["NORDEN"].ToString()) : 0;
                            fileConfig.IdPathConfig = Convert.ToInt16(reader["NIDPATHCONFIG"].ToString());
                            fileConfig.IdFileProc = (!DBNull.Value.Equals(reader["NIDFILEPROC"])) ? Convert.ToInt16(reader["NIDFILEPROC"]) : 0;
                            fileConfig.status = reader["NSTATUS"].ToString();
                            fileConfig.table_reference = (!DBNull.Value.Equals(reader["STABLEREFERENCE"])) ? (reader["STABLEREFERENCE"].ToString()) : string.Empty;
                            fileConfig.caracterFormat = reader["SFILEFORMAT"].ToString();
                            fileConfig.flg_optional = reader["SFIELDS"].ToString();
                            fileConfig.Extension = reader["SEXTENSION"].ToString();
                            fileConfig.IndiciadorService = (!DBNull.Value.Equals(reader["SINDSERVICE"])) ? reader["SINDSERVICE"].ToString() : "0";
                            ListFileConfig.Add(fileConfig);
                        }
                    }
                }
            }
            return ListFileConfig;
        }

        public List<ProductLoadVM> GetProductsList(int P_NBRANCH)
        {
            var sPackageName = string.Format("{0}.{1}", ProcedureName.pkg_CargaMasivaTrama, "SPS_LIST_PRODUCT");
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<ProductLoadVM> ListProductByVidaLey = new List<ProductLoadVM>();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, P_NBRANCH, ParameterDirection.Input));
                //OUTPUT
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter))
                {
                    ListProductByVidaLey = dr.ReadRowsList<ProductLoadVM>();
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetProductsList", ex.ToString(), "3");
            }

            return ListProductByVidaLey;
        }

        public DataTable GetDataExportTxt(int IdDetailProcess, string TableReference, int IdHeaderProcess)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            DataTable DataReturn = new DataTable();            
            string storeProcedureName = ProcedureName.pkg_ReporteSucaveSCTR + ".SPS_EXPORTAR_DATA_SUCAVE_TXT";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NIDFILEHEADER", OracleDbType.Int32, IdDetailProcess, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TABLE_REFERENCE", OracleDbType.Varchar2, TableReference, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDHEADERPROC", OracleDbType.Int32, IdHeaderProcess, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                P_NCODE.Size = 100;
                P_MESSAGE.Size = 4000;

                parameter.Add(P_NCODE);
                parameter.Add(P_MESSAGE);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storeProcedureName, parameter);

                if (odr != null)
                {
                    DataReturn.Load(odr);
                }
                ELog.CloseConnection(odr);

            } catch(Exception ex)
            {
                ELog.save(this, ex);
            }
            return DataReturn;

        }

        #endregion

    }
}
