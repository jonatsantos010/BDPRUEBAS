using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.Util;
using WSPlataforma.Entities.ReporteProviComisionesModel.ViewModel;
using WSPlataforma.Entities.ReporteProviComisionesModel.BindingModel;
using WSPlataforma.Entities.ReportModel.BindingModel;

using WSPlataforma.Entities.PrintPolicyModel.ATP;
using WSPlataforma.Entities.ReportModel.ViewModel;

using Oracle.DataAccess.Client;
using System.Data;

namespace WSPlataforma.DA
{
    public class ReporteProviComisionesDA : ConnectionBase
    {
        //obtiene ramo
        public List<ReporteProviComisionesVM> GetBranch()
        {
            var parameters = new List<OracleParameter>();
            var suggests = new List<ReporteProviComisionesVM>();
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_ReportesBrokerSCTR, "SPS_GET_BRANCHES");

            try
            {
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var suggest = new ReporteProviComisionesVM();
                        suggest.Id = reader["NID"] == DBNull.Value ? 0 : int.Parse(reader["NID"].ToString());
                        suggest.Description = reader["SDESCRIPTION"] == DBNull.Value ? string.Empty : reader["SDESCRIPTION"].ToString();

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

        //inserta tabla status de reporte
        public int InsertReportStatus(ReporteProviComisionesFilter data)
        {
            try
            {
                ResponseControl generic = new ResponseControl(Response.Ok);

                var invoices = new List<ReporteProviComisionesBM>();
                var parameters = new List<OracleParameter>();
                var procedure = "SPS_INSERT_REPORT";
                int NSTATUS = 2; //insertar
                string fechaInicio = data.StartDate.ToString("dd/MM/yyyy");
                string fechaFin = data.EndDate.ToString("dd/MM/yyyy");

                parameters.Add(new OracleParameter("P_NIDREPORT", OracleDbType.Varchar2, data.IdReport, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NTYPE_REPORT", OracleDbType.Int32, data.NTYPE_REPORT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SUSERNAME", OracleDbType.Varchar2, data.UserName.ToUpper(), ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DINIREP", OracleDbType.Date, Convert.ToDateTime(fechaInicio), ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DFINREP", OracleDbType.Date, Convert.ToDateTime(fechaFin), ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.BranchId, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NSTATUSPROC", OracleDbType.Int32, NSTATUS, ParameterDirection.Input));

                //Ncode controla los errores
                var NCode = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);
                var SMessage = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                parameters.Add(NCode);
                parameters.Add(SMessage);
                NCode.Size = 4000;
                SMessage.Size = 4000;

                this.ExecuteByStoredProcedureVT(string.Format("{0}.{1}", ProcedureName.pkg_ReportesProviComisiones, procedure), parameters);

                var procedure2 = "SP_UPD_REPORT_PD";
                var parameters2 = new List<OracleParameter>();
                parameters2.Add(new OracleParameter("P_SID", OracleDbType.Varchar2, data.IdReport, ParameterDirection.Input));
                var InsertStatus = new OracleParameter("P_STATUS", OracleDbType.Int32, ParameterDirection.Output);
                parameters2.Add(InsertStatus);
                InsertStatus.Size = 10;
                this.ExecuteByStoredProcedureVT(string.Format("{0}.{1}", ProcedureName.pkg_ReportesProviComisiones, procedure2), parameters2);

                var PInsertStatus = Convert.ToInt32(InsertStatus.Value.ToString());
                var PCode = Convert.ToInt32(NCode.Value.ToString());
                var PMessage = (SMessage.Value.ToString());

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
        public List<ReporteProviComisionesBM> GetReportStatus(ReporteProviComisionesFilter data)
        {
            var validations = new List<ReporteProviComisionesBM>();

            try
            {
                var parameters = new List<OracleParameter>();
                var procedure = "SPS_GET_REPORT_STATUS";

                string fch_inicio = data.StartDate.ToString("dd/MM/yyyy");
                string fch_fin = data.EndDate.ToString("dd/MM/yyyy");

                parameters.Add(new OracleParameter("P_NIDREPORT", OracleDbType.Varchar2, data.IdReport, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.BranchId, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DINIREP", OracleDbType.Date, Convert.ToDateTime(fch_inicio), ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DFINREP", OracleDbType.Date, Convert.ToDateTime(fch_fin), ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NTYPE_REPORT", OracleDbType.Int32, data.NTYPE_REPORT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(string.Format("{0}.{1}", ProcedureName.pkg_ReportesProviComisiones, procedure), parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var validation = new ReporteProviComisionesBM();

                        validation.NIDREPORT = reader["SIDREPORT"] == DBNull.Value ? string.Empty : reader["SIDREPORT"].ToString();
                        validation.SUSERNAME = reader["SUSERNAME"] == DBNull.Value ? string.Empty : reader["SUSERNAME"].ToString();
                        validation.DINIREP = reader["DINIREP"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["DINIREP"].ToString());
                        validation.DFINREP = reader["DFINREP"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["DFINREP"].ToString());
                        validation.DPROCESO = reader["DFINPROC"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["DFINPROC"].ToString());
                        validation.NSTATUSPROC = reader["NSTATUSPROC"] == DBNull.Value ? 0 : int.Parse(reader["NSTATUSPROC"].ToString());
                        validation.NBRANCH = reader["NBRANCH"] == DBNull.Value ? 0 : int.Parse(reader["NBRANCH"].ToString());
                        validation.SBRANCH_NAME = reader["SBRANCH_NAME"] == DBNull.Value ? string.Empty : reader["SBRANCH_NAME"].ToString();

                        validations.Add(validation);

                    }
                }

                reader.Close();

            }
            catch (Exception ex)
            {
                LogControl.save("GetReportStatus", ex.ToString(), "3");
                throw;
            }
            return validations;
        }

        public ResponseReportATPVM ReportProviComisionesById(RequestReportProviComisionesById request)
        {

            ResponseReportATPVM response = new ResponseReportATPVM();
            var parameters = new List<OracleParameter>();
            var procedure = "SPS_GET_REPORT_PROVI_COMISIONES_XID";

            List<ReportProviComisiones> dataReport = new List<ReportProviComisiones>();

            parameters.Add(new OracleParameter("P_SID", OracleDbType.Varchar2, request.reportId, ParameterDirection.Input));
            parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

            var dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(string.Format("{0}.{1}", ProcedureName.pkg_ReportesProviComisiones, procedure), parameters);

            //    OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(nameStore, nameCursor, oracleParameterList);

            try
            {
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        ReportProviComisiones obj = new ReportProviComisiones();

                        obj.TIPO = dr["TIPO"] == DBNull.Value ? string.Empty : dr["TIPO"].ToString().Trim();
                        obj.nbranch = dr["nbranch"] == DBNull.Value ? 0 : Convert.ToInt32(dr["nbranch"].ToString());
                        obj.sbranch = dr["sbranch"] == DBNull.Value ? string.Empty : dr["sbranch"].ToString().Trim();
                        obj.nproduct = dr["nproduct"] == DBNull.Value ? 0 : Convert.ToInt32(dr["nproduct"].ToString());
                        obj.sproduct = dr["sproduct"] == DBNull.Value ? string.Empty : dr["sproduct"].ToString().Trim();
                        obj.npolicy = dr["npolicy"] == DBNull.Value ? 0 : Convert.ToInt64(dr["npolicy"].ToString());
                        obj.nreceipt = dr["nreceipt"] == DBNull.Value ? 0 : Convert.ToInt64(dr["nreceipt"].ToString());
                        obj.sstatus_rec = dr["sstatus_rec"] == DBNull.Value ? string.Empty : dr["sstatus_rec"].ToString().Trim();
                        obj.stype_comp = dr["stype_comp"] == DBNull.Value ? string.Empty : dr["stype_comp"].ToString().Trim();
                        obj.nbillnum = dr["nbillnum"] == DBNull.Value ? 0 : Convert.ToInt64(dr["nbillnum"].ToString());
                        obj.COMPROBANTE = dr["COMPROBANTE"] == DBNull.Value ? string.Empty : dr["COMPROBANTE"].ToString().Trim();
                        obj.sstatus_comp = dr["sstatus_comp"] == DBNull.Value ? string.Empty : dr["sstatus_comp"].ToString().Trim();
                        obj.moneda = dr["moneda"] == DBNull.Value ? string.Empty : dr["moneda"].ToString().Trim();
                        obj.contratante = dr["contratante"] == DBNull.Value ? string.Empty : dr["contratante"].ToString().Trim();
                        obj.dprov_interfaz = dr["dprov_interfaz"].Equals("") ? string.Empty : dr["dprov_interfaz"].ToString();
                        obj.ini_per = dr["ini_per"].Equals("") ? string.Empty : dr["ini_per"].ToString();
                        obj.fin_per = dr["fin_per"].Equals("") ? string.Empty : dr["fin_per"].ToString();
                        obj.dauto_comm = dr["dauto_comm"].Equals("") ? string.Empty : dr["dauto_comm"].ToString();
                        obj.sstate_com = dr["sstate_com"] == DBNull.Value ? string.Empty : dr["sstate_com"].ToString().Trim();
                        obj.ncomm_pago_inter = dr["ncomm_pago_inter"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["ncomm_pago_inter"].ToString());
                        obj.ncomm_pago_comer = dr["ncomm_pago_comer"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["ncomm_pago_comer"].ToString());
                        obj.ncomm_pago_bys = dr["ncomm_pago_bys"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["ncomm_pago_bys"].ToString());
                        obj.nprov_comm_int = dr["nprov_comm_int"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["nprov_comm_int"].ToString());
                        obj.nprov_comm_com = dr["nprov_comm_com"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["nprov_comm_com"].ToString());
                        obj.nprov_com_bys = dr["nprov_com_bys"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["nprov_com_bys"].ToString());
                        obj.suser_auto = dr["suser_auto"] == DBNull.Value ? string.Empty : dr["suser_auto"].ToString().Trim();
                        //obj.fec_proc = dr["DFINPROC"].Equals("") ? string.Empty : dr["DFINPROC"].ToString();

                        dataReport.Add(obj);
                    }
                }
                ELog.CloseConnection(dr);
                response.TotalRowNumber = dataReport.Count;
                response.GenericResponse = dataReport;
            }
            catch (Exception ex)
            {
                LogControl.save("ReportProviComisionesById", ex.ToString(), "3");
            }
            return response;
        }

    }
}
