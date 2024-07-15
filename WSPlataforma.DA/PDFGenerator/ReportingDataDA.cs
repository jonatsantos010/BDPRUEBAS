using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.Entities.ReportModel.BindingModel;
using WSPlataforma.Entities.Reports.BindingModel;
using WSPlataforma.Entities.Reports.BindingModel.ReportEntities;
using WSPlataforma.Entities.Reports.ViewModel;
using WSPlataforma.Util;

namespace WSPlataforma.DA
{
    public class ReportingDataDA : ConnectionBase
    {
        public Quotation Slip_Cotizacion(string quotationNumber)
        {
            var datosSplit = new Quotation();

            try
            {
                string storedProcedureName = ProcedureName.pkg_SendEmail + ".REA_DATOS_SLIP_CAB";
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Varchar2, quotationNumber, ParameterDirection.Input));

                OracleParameter table = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameters.Add(table);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameters);
                if (odr.HasRows)
                {
                    odr.Read();
                    datosSplit.ProductId = odr["PRODUCTO_ID"].ToString();
                    datosSplit.RamoId = odr["NBRANCH"].ToString();
                    datosSplit.ProductName = ELog.obtainConfig("productoKey" + datosSplit.RamoId + datosSplit.ProductId);
                    datosSplit.IsMining = odr["MINA"].ToString() == "1" ? true : false;
                    datosSplit.Contractor = odr["CONTRATANTE"].ToString();
                    datosSplit.EconomicActivity = odr["ACTIVIDAD"].ToString();
                    datosSplit.OperationDate = Convert.ToDateTime(odr["FECHA_OPERACION"].ToString());
                    datosSplit.InsuredQuantity = String.Format("{0:#,0}", odr["ASEGURADOS_TOTAL"]); //odr["ASEGURADOS_TOTAL"].ToString();
                    datosSplit.Payroll = "S/." + String.Format("{0:#,0.00}", Convert.ToDecimal(odr["PLANILLA_TOTAL"])); // Math.Round(Convert.ToDouble(odr["PLANILLA_MENSUAL"].ToString()), 2).ToString(); // FormatAmount(odr["PLANILLA_MENSUAL"].ToString());
                    datosSplit.RenovationMinimunPremium = String.Format("{0:#,0.00}", odr["PRIMA_MINIMA_RENOVACION"]); // FormatAmount(odr["PRIMA_MINIMA_RENOVACION"].ToString());
                    datosSplit.EndorsementMinimunPremium = String.Format("{0:#,0.00}", odr["PRIMA_MINIMA_ENDOSO"]);  // FormatAmount(odr["PRIMA_MINIMA_ENDOSO"].ToString());
                    datosSplit.Gloss = odr["GLOSA_SLIP"].ToString();
                    datosSplit.Company = odr["NCOMPANY_LNK"].ToString();
                    odr.Close();

                    datosSplit.RiskList = GetQuotationRisk(quotationNumber);
                }
                else
                {
                    datosSplit = null;
                }
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                datosSplit = null;
                LogControl.save("Slip_Cotizacion", ex.ToString(), "3");
            }

            return datosSplit;
        }
        private List<QuotationRisk> GetQuotationRisk(string quotationNumber)
        {
            List<QuotationRisk> riskDetailList = new List<QuotationRisk>();
            try
            {
                string storedProcedureName = ProcedureName.pkg_SendEmail + ".REA_DATOS_SLIP_DET";

                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Varchar2, quotationNumber, ParameterDirection.Input));

                OracleParameter table = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameters.Add(table);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameters);
                while (odr.Read())
                {
                    QuotationRisk item = new QuotationRisk();
                    item.ProductId = odr["PRODUCTO_ID"].ToString();
                    item.Risk = odr["RIESGO"].ToString();
                    item.Rate = String.Format("{0} %", odr["TASA"]);
                    item.Payroll = String.Format("{0:#,0.00}", odr["PLANILLA_MENSUAL"]); // Math.Round(Convert.ToDouble(odr["PLANILLA_MENSUAL"].ToString()), 2).ToString(); // FormatAmount(odr["PLANILLA_MENSUAL"].ToString());
                    item.Premium = String.Format("{0:#,0.00}", odr["PRIMA_NETA_MENSUAL"]);   //Math.Round(Convert.ToDouble(odr["PRIMA_NETA_MENSUAL"].ToString()), 2).ToString(); // FormatAmount(odr["PRIMA_NETA_MENSUAL"].ToString());
                    item.TotalNetPremium = String.Format("{0:#,0.00}", odr["PRIMA_COMERCIAL_NETA"]);  //FormatAmount(odr["PRIMA_COMERCIAL_NETA"].ToString());
                    item.TotalPremimumIGV = String.Format("{0:#,0.00}", odr["IGV_PRIMA_COMERCIAL"]);  //FormatAmount(odr["IGV_PRIMA_COMERCIAL"].ToString());
                    item.TotalGrossPremium = String.Format("{0:#,0.00}", odr["PRIMA_COMERCIAL_BRUTA"]);  //FormatAmount(odr["PRIMA_COMERCIAL_BRUTA"].ToString());

                    riskDetailList.Add(item);
                }
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("GetQuotationRisk", ex.ToString(), "3");
            }

            return riskDetailList;
        }
        public async Task<Constancia> Constancia(TransactionTrack transactionTrack)
        {
            OracleDataReader odr = null;
            try
            {
                string storedProcedureName = ProcedureName.pkg_DocumentosSCTR + ".REA_DATOS_CONSTANCIA";

                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Varchar2, transactionTrack.QuotationNumber, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NMOVEMENT", OracleDbType.Varchar2, transactionTrack.TransactionId, ParameterDirection.Input));

                OracleParameter table = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameters.Add(table);

                odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameters);
                if (odr.HasRows)
                {
                    odr.Read();
                    Constancia item = new Constancia();
                    item.ProofNumber = odr["NUMERO_CONSTANCIA"].ToString();
                    item.RmaSalud = "S/. " + String.Format("{0:#,0.00}", odr["TOPE_REMU_SAL"]);
                    item.RmaPension = "S/. " + String.Format("{0:#,0.00}", odr["TOPE_REMU_PEN"]);

                    item.HealthPolicy = new Policy();
                    item.PensionPolicy = new Policy();
                    item.TransactionType = transactionTrack.TransactionType;

                    item.Contractor = new Contractor();
                    item.Contractor.Name = odr["NOMBRE_CONTRATANTE"].ToString();

                    item.VigencyStartDate = Convert.ToDateTime(odr["INI_VIGENCIA"].ToString());
                    item.VigencyEndDate = Convert.ToDateTime(odr["FIN_VIGENCIA"].ToString());

                    item.OperationDate = Convert.ToDateTime(odr["FEC_OPERACION"].ToString());
                    item.Contractor.EconomicActiviy = odr["ACTIVIDAD_ECONOMICA"].ToString();
                    item.HealthPolicy.Number = odr["NRO_POLIZA_SAL"].ToString();
                    item.PensionPolicy.Number = odr["NRO_POLIZA_PEN"].ToString();

                    item.Contractor.Location = new Location();
                    item.Contractor.Location.Description = odr["SDESCRIPTION"].ToString();
                    item.Contractor.Location.District = odr["DISTRITO"].ToString();
                    item.Contractor.Location.Province = odr["PROVINCIA"].ToString();
                    item.Contractor.Location.Department = odr["DEPARTAMENTO"].ToString();

                    item.Protecta = new Protecta();
                    item.Protecta.IconPath = AppDomain.CurrentDomain.BaseDirectory + odr["LOGO_PRO"].ToString();
                    item.Protecta.SignaturePath = AppDomain.CurrentDomain.BaseDirectory + odr["FIRMA_PRO"].ToString();
                    item.Protecta.Name = odr["NOMBRE_PRO"].ToString();

                    item.Sanitas = new Sanitas();
                    item.Sanitas.IconPath = AppDomain.CurrentDomain.BaseDirectory + odr["LOGO_SAN"].ToString();
                    item.Sanitas.SignaturePath = AppDomain.CurrentDomain.BaseDirectory + odr["FIRMA_SAN"].ToString();
                    item.Sanitas.Name = "GRANDÍA S.A.- EPS";

                    item.IsMining = odr["ES_MINA"].ToString() == "1" ? true : false;

                    ELog.CloseConnection(odr);

                    if (item.HealthPolicy.Number.ToString().Trim() != "" && item.PensionPolicy.Number.ToString().Trim() != "")
                    {
                        item.Mode = "3";
                        item.InsuredList = GetInsuredList(transactionTrack, ELog.obtainConfig("saludKey"));
                    }
                    else if (item.HealthPolicy.Number.ToString().Trim() != "")
                    {
                        item.Mode = "1";
                        item.InsuredList = GetInsuredList(transactionTrack, ELog.obtainConfig("saludKey"));
                    }
                    else
                    {
                        item.Mode = "2";
                        item.InsuredList = GetInsuredList(transactionTrack, ELog.obtainConfig("pensionKey"));
                    }

                    return await Task.FromResult(item);
                }
                else return null;
            }
            catch (Exception ex)
            {
                ELog.CloseConnection(odr);
                LogControl.save("Constancia", ex.ToString(), "3");
                return null;
            }
        }

        public async Task<Contrato> Contrato(TransactionTrack transactionTrack)
        {
            OracleDataReader odr = null;
            try
            {
                string storedProcedureName = ProcedureName.pkg_DocumentosSCTR + ".SPS_DOC_CONTRATO_CAB";

                List<OracleParameter> parameters = new List<OracleParameter>();

                parameters.Add(new OracleParameter("P_SKEY", OracleDbType.Varchar2, transactionTrack.Key, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, ELog.obtainConfig("saludKey"), ParameterDirection.Input));

                OracleParameter table = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameters.Add(table);

                odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameters);
                if (odr.HasRows)
                {
                    odr.Read();
                    Contrato item = new Contrato();
                    item.TransactionType = transactionTrack.TransactionType;

                    item.Contractor = new Contractor();
                    item.Contractor.Name = odr["NOMBRE_CONTRATANTE"].ToString();
                    item.Contractor.DocumentType = odr["TIPDOC_CONTRATANTE"].ToString();
                    item.Contractor.DocumentNumber = odr["NUMDOC_CONTRATANTE"].ToString();

                    item.Contractor.Location = new Location();
                    item.Contractor.Location.EconomicActivity = odr["ACTECO_SEDE"].ToString();
                    item.Contractor.Location.Description = odr["DESC_SEDE"].ToString();
                    item.Contractor.Location.District = odr["DISTR_SEDE"].ToString();
                    item.Contractor.Location.Province = odr["PROV_SEDE"].ToString();
                    item.Contractor.Location.Department = odr["DEP_SEDE"].ToString();

                    item.Contractor.Address = odr["DIREC_CONTRATANTE"].ToString();
                    item.Contractor.District = odr["DISTR_CONTRATANTE"].ToString();
                    item.Contractor.Province = odr["PROV_CONTRATANTE"].ToString();
                    item.Contractor.Department = odr["DEP_CONTRATANTE"].ToString();

                    item.Sanitas = new Sanitas();
                    item.Sanitas.Name = odr["NOMBRE_SANITAS"].ToString();
                    item.Sanitas.IconPath = AppDomain.CurrentDomain.BaseDirectory + odr["ICONO_SANITAS"].ToString();
                    item.Sanitas.SignaturePath = AppDomain.CurrentDomain.BaseDirectory + odr["FIRMA_SANITAS"].ToString();

                    item.VigencyStartDate = Convert.ToDateTime(odr["INI_VIGENCIA"].ToString());
                    item.VigencyEndDate = Convert.ToDateTime(odr["FIN_VIGENCIA"].ToString());
                    item.OperationDate = Convert.ToDateTime(odr["FEC_OPERACION"].ToString());

                    item.Policy = new Policy();
                    item.Policy.Number = odr["NRO_POLIZA_SALUD"].ToString();

                    item.Policy.TotalNetPremium = String.Format("{0:#,0.00}", odr["PRIMA_NETA_SALUD"]); // FormatAmount(odr["PRIMA_NETA_SALUD"].ToString());
                    item.Policy.TotalPremiumIGV = String.Format("{0:#,0.00}", odr["IGV_PRIMA_SALUD"]); // FormatAmount(odr["IGV_PRIMA_SALUD"].ToString());
                    item.Policy.TotalGrossPremium = String.Format("{0:#,0.00}", odr["PRIMA_BRUTA_SALUD"]);  //FormatAmount(odr["PRIMA_BRUTA_SALUD"].ToString());

                    ELog.CloseConnection(odr);

                    item.InsuredList = GetInsuredList(transactionTrack, ELog.obtainConfig("saludKey"));
                    item.Policy.RiskDetailList = GetRiskDetailList(transactionTrack, ELog.obtainConfig("saludKey"));

                    return await Task.FromResult(item);
                }
                else return null;
            }
            catch (Exception ex)
            {
                ELog.CloseConnection(odr);
                LogControl.save("Contrato", ex.ToString(), "3");
                return null;
            }
        }
        public async Task<Solicitud_Pension> Solicitud_Pension(TransactionTrack transactionTrack)
        {
            OracleDataReader odr = null;
            try
            {
                string storedProcedureName = ProcedureName.pkg_DocumentosSCTR + ".SPS_DOC_SOLICITUD_PEN_CAB";

                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("P_SKEY", OracleDbType.Varchar2, transactionTrack.Key, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, ELog.obtainConfig("pensionKey"), ParameterDirection.Input));

                OracleParameter table = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameters.Add(table);
                odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameters);
                if (odr.HasRows)
                {
                    odr.Read();
                    Solicitud_Pension item = new Solicitud_Pension();
                    item.Policy = new Policy();
                    item.Policy.Number = odr["NRO_POLIZA_PEN"].ToString();
                    item.TextB = odr["TEXTO_BENMAX"].ToString();

                    item.Protecta = new Protecta();
                    item.Protecta.Name = odr["NOMBRE_PROTECTA"].ToString();
                    item.Protecta.RUC = odr["RUC_PROTECTA"].ToString();
                    item.Protecta.Address = odr["DIREC_PROTECTA"].ToString();
                    item.Protecta.Email = odr["EMAIL_PROTECTA"].ToString();

                    item.Contractor = new Contractor();
                    item.Contractor.Name = odr["NOMBRE_CONTRATANTE"].ToString();
                    item.Contractor.DocumentType = odr["TIPDOC_CONTRATANTE"].ToString();
                    item.Contractor.DocumentNumber = odr["NUMDOC_CONTRATANTE"].ToString();
                    item.Contractor.Address = odr["DIREC_CONTRATANTE"].ToString();
                    item.Contractor.Phone = odr["TELF_CONTRATANTE"].ToString();
                    item.Contractor.Email = odr["EMAIL_CONTRATANTE"].ToString();
                    item.Contractor.EconomicActiviy = odr["ACTECO_CONTRATANTE"].ToString();
                    item.Contractor.CIIU = odr["CIIU_CONTRATANTE"].ToString();

                    item.Broker = new Broker();
                    item.Broker.Name = odr["NOMBRE_BROKER"].ToString();
                    item.Broker.DocumentType = odr["TIPDOC_BROKER"].ToString();
                    item.Broker.DocumentNumber = odr["NUMDOC_BROKER"].ToString();
                    item.Broker.Address = odr["DIREC_BROKER"].ToString();
                    item.Broker.Phone = odr["TELF_BROKER"].ToString();
                    item.Broker.SBSCode = odr["SBS_BROKER"].ToString();
                    item.Broker.Email = odr["EMAIL_BROKER"].ToString();

                    item.WorkersTotalNumber = String.Format("{0:#,0}", Convert.ToInt64(odr["TOTAL_TRAB"]));
                    item.PayrollAmount = String.Format("{0:#,0.00}", odr["PLANILLA"]);  // FormatAmount(odr["PLANILLA"].ToString());

                    item.Policy.VigencyStartDate = Convert.ToDateTime(odr["INI_VIGENCIA"].ToString());
                    item.Policy.VigencyEndDate = Convert.ToDateTime(odr["FIN_VIGENCIA"].ToString());

                    item.Policy.TotalGrossPremium = String.Format("{0:#,0.00}", odr["PRIMA_BRUTA_PEN"]);  // FormatAmount(odr["PRIMA_BRUTA_PEN"].ToString());
                    item.PremiumPaymentMode = odr["MODO_PAGO_PRIMA"].ToString();
                    item.PremiumPaymentPlace = odr["LUGAR_PAGO_PRIMA"].ToString();

                    item.Contractor.Location = new Location();
                    item.Contractor.Location.EconomicActivity = odr["ACTECO_SEDE"].ToString();
                    item.Protecta.IconPath = AppDomain.CurrentDomain.BaseDirectory + odr["LOGO_PROTECTA"].ToString();
                    item.OperationDate = Convert.ToDateTime(odr["FEC_OPERACION"].ToString());

                    ELog.CloseConnection(odr);

                    item.InsuranceCoverages = GetInsuranceCoverageList();
                    item.InsuranceExclusions = GetInsuranceExclusionList();
                    item.InsuranceCoveragePlaces = GetInsuranceCoveragePlaceList();
                    item.Policy.RiskDetailList = GetRiskDetailList(transactionTrack, ELog.obtainConfig("pensionKey"));
                    item.Protecta.PhoneList = GetProtectaPhoneList();

                    return await Task.FromResult(item);
                }
                else
                {
                    ELog.CloseConnection(odr);
                    return null;
                }

            }
            catch (Exception ex)
            {
                ELog.CloseConnection(odr);
                LogControl.save("Solicitud_Pension", ex.ToString(), "3");
                return null;
            }
        }
        public async Task<Proforma> Proforma(TransactionTrack transactionTrack, string productId)
        {
            OracleDataReader odr = null;
            try
            {
                string storedProcedureName = ProcedureName.pkg_DocumentosSCTR + ".SPS_DOC_PROFORMA";

                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("P_SKEY", OracleDbType.Varchar2, transactionTrack.Key, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, int.Parse(productId), ParameterDirection.Input));

                OracleParameter table = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameters.Add(table);

                odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameters);
                if (odr.HasRows)
                {
                    odr.Read();
                    Proforma item = new Proforma();
                    item.OperationDate = Convert.ToDateTime(odr["FEC_OPERACION"].ToString());
                    item.Policy = new Policy();
                    item.Policy.ProductTypeId = productId;
                    item.Policy.Number = odr["NRO_POLIZA"].ToString();
                    item.ProformaCode = odr["PROFORMA_CODIGO"].ToString();
                    item.ProformaExpirationDate = Convert.ToDateTime(odr["PROFORMA_EXPIRACION"].ToString());
                    item.Policy.VigencyStartDate = Convert.ToDateTime(odr["INI_VIGENCIA"].ToString());
                    item.Policy.VigencyEndDate = Convert.ToDateTime(odr["FIN_VIGENCIA"].ToString());

                    item.Protecta = new Protecta();
                    item.Protecta.IconPath = AppDomain.CurrentDomain.BaseDirectory + odr["LOGO_ASEG"].ToString();
                    item.Protecta.Name = odr["NOMBRE_ASEG"].ToString();
                    item.Protecta.Address = odr["DIREC_ASEG"].ToString();
                    item.Protecta.Department = odr["DEP_ASEG"].ToString();
                    item.Protecta.Province = odr["PROV_ASEG"].ToString();
                    item.Protecta.District = odr["DISTR_ASEG"].ToString();
                    item.Protecta.RUC = odr["RUC_ASEG"].ToString();
                    item.Protecta.WEB = odr["WEB_ASEG"].ToString();

                    item.Contractor = new Contractor();
                    item.Contractor.Name = odr["NOMBRE_CONTRATANTE"].ToString();
                    item.Contractor.DocumentNumber = odr["NUMDOC_CONTRATANTE"].ToString();
                    item.Contractor.Address = odr["DIREC_CONTRATANTE"].ToString();
                    item.Contractor.Phone = odr["TELF_CONTRATANTE"].ToString();
                    item.Contractor.District = odr["DISTR_CONTRATANTE"].ToString();
                    item.Contractor.Province = odr["PROV_CONTRATANTE"].ToString();
                    item.Contractor.Department = odr["DEP_CONTRATANTE"].ToString();
                    item.Contractor.ZipCode = odr["POSTAL_CONTRATANTE"].ToString();
                    item.Policy.TotalNetPremium = String.Format("{0:#,0.00}", odr["PRIMA_NETA"]);
                    item.Policy.TotalPremiumIGV = String.Format("{0:#,0.00}", odr["IGV_PRIMA"]); //FormatAmount(odr["IGV_PRIMA"].ToString());
                    item.Policy.TotalGrossPremium = String.Format("{0:#,0.00}", odr["PRIMA_BRUTA"]); // FormatAmount(odr["PRIMA_BRUTA"].ToString());

                    ELog.CloseConnection(odr);

                    item.PaymentModeList = GetPaymentModeList();
                    return await Task.FromResult(item);
                }
                else
                {
                    ELog.CloseConnection(odr);
                    return null;
                }
            }
            catch (Exception ex)
            {
                ELog.CloseConnection(odr);
                LogControl.save("Proforma", ex.ToString(), "3");
                return null;
            }
        }
        public async Task<Condiciones_Particulares_Pension> Condiciones_Particulares_Pension(TransactionTrack transactionTrack)
        {
            OracleDataReader odr = null;
            try
            {
                string storedProcedureName = ProcedureName.pkg_DocumentosSCTR + ".SPS_DOC_COND_PARTIC_PENSION";

                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("P_SKEY", OracleDbType.Varchar2, transactionTrack.Key, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, ELog.obtainConfig("pensionKey").ToString(), ParameterDirection.Input));

                OracleParameter table = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameters.Add(table);

                odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameters);
                if (odr.HasRows)
                {
                    odr.Read();
                    Condiciones_Particulares_Pension item = new Condiciones_Particulares_Pension();
                    item.OperationDate = Convert.ToDateTime(odr["FEC_OPERACION"].ToString());
                    item.TextA = odr["TEXTO_ASEG"].ToString();
                    item.TextB = odr["TEXTO_BENMAX"].ToString();

                    item.Policy = new Policy();
                    item.Policy.Number = odr["NRO_POLIZA"].ToString();
                    item.Policy.VigencyStartDate = Convert.ToDateTime(odr["INI_VIGENCIA"].ToString());
                    item.Policy.VigencyEndDate = Convert.ToDateTime(odr["FIN_VIGENCIA"].ToString());

                    item.Protecta = new Protecta();
                    item.Protecta.IconPath = AppDomain.CurrentDomain.BaseDirectory + odr["LOGO_PROTECTA"].ToString();
                    item.Protecta.Name = odr["NOMBRE_PROTECTA"].ToString();
                    item.Protecta.Address = odr["DIREC_PROTECTA"].ToString();
                    item.Protecta.Department = odr["DEP_PROTECTA"].ToString();
                    item.Protecta.Province = odr["PROV_PROTECTA"].ToString();
                    item.Protecta.District = odr["DISTR_PROTECTA"].ToString();
                    item.Protecta.RUC = odr["RUC_PROTECTA"].ToString();
                    item.Protecta.Email = odr["EMAIL_PROTECTA"].ToString();

                    item.Contractor = new Contractor();
                    item.Contractor.Name = odr["NOMBRE_CONTRATANTE"].ToString();
                    item.Contractor.Address = odr["DIREC_CONTRATANTE"].ToString();
                    item.Contractor.Phone = odr["TELF_CONTRATANTE"].ToString();
                    item.Contractor.District = odr["DISTR_CONTRATANTE"].ToString();
                    item.Contractor.Province = odr["PROV_CONTRATANTE"].ToString();
                    item.Contractor.Department = odr["DEP_CONTRATANTE"].ToString();

                    item.Contractor.Location = new Location();
                    item.Contractor.Location.Description = odr["DESC_SEDE"].ToString();
                    item.Contractor.Location.EconomicActivity = odr["ACTECO_SEDE"].ToString();

                    item.PaymentMode = odr["MODO_PAGO_PRIMA"].ToString();
                    item.PaymentPlace = odr["LUGAR_PAGO_PRIMA"].ToString();

                    item.Broker = new Broker();
                    item.Broker.Name = odr["NOMBRE_BROKER"].ToString();
                    item.Broker.SBSCode = odr["SBS_BROKER"].ToString();
                    item.Broker.Commission = String.Format("{0:0.00}", odr["COMISION_BROKER"]);

                    item.Policy.TotalNetPremium = String.Format("{0:#,0.00}", odr["PRIMA_NETA"]);  //FormatAmount(odr["PRIMA_NETA"].ToString());
                    item.Policy.TotalPremiumIGV = String.Format("{0:#,0.00}", odr["IGV_PRIMA"]);  //FormatAmount(odr["IGV_PRIMA"].ToString());
                    item.Policy.TotalGrossPremium = String.Format("{0:#,0.00}", odr["PRIMA_BRUTA"]);  //FormatAmount(odr["PRIMA_BRUTA"].ToString());

                    ELog.CloseConnection(odr);

                    item.Policy.RiskDetailList = GetRiskDetailList(transactionTrack, ELog.obtainConfig("pensionKey"));
                    item.InsuranceCoverageList = GetInsuranceCoverageList();
                    item.Protecta.PhoneList = GetProtectaPhoneList();

                    return await Task.FromResult(item);
                }
                else
                {
                    ELog.CloseConnection(odr);
                    return null;
                }
            }
            catch (Exception ex)
            {
                ELog.CloseConnection(odr);
                LogControl.save("Condiciones_Particulares_Pension", ex.ToString(), "3");
                return null;
            }
        }
        public List<string> GetProtectaPhoneList()
        {
            OracleDataReader odr = null;
            try
            {
                string storedProcedureName = ProcedureName.pkg_DocumentosSCTR + ".REA_PROTECTA_TELF";

                List<OracleParameter> parameters = new List<OracleParameter>();
                List<string> phoneList = new List<string>();
                OracleParameter table = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameters.Add(table);

                odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameters);
                while (odr.Read())
                {
                    phoneList.Add(odr["NOMBRE"].ToString());
                }

                ELog.CloseConnection(odr);
                return phoneList;
            }
            catch (Exception ex)
            {
                ELog.CloseConnection(odr);
                LogControl.save("GetProtectaPhoneList", ex.ToString(), "3");
                return null;
            }
        }
        public List<string> GetInsuranceCoverageList()
        {
            OracleDataReader odr = null;
            try
            {
                string storedProcedureName = ProcedureName.pkg_DocumentosSCTR + ".REA_COBERTURA_SEG_PEN";

                List<OracleParameter> parameters = new List<OracleParameter>();
                List<string> insuranceCoverageList = new List<string>();
                OracleParameter table = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameters.Add(table);

                odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameters);
                while (odr.Read())
                {
                    insuranceCoverageList.Add(odr["NOMBRE"].ToString());
                }
                ELog.CloseConnection(odr);

                return insuranceCoverageList;
            }
            catch (Exception ex)
            {
                ELog.CloseConnection(odr);
                LogControl.save("GetInsuranceCoverageList", ex.ToString(), "3");
                return null;
            }
        }
        public List<string> GetInsuranceExclusionList()
        {
            OracleDataReader odr = null;
            try
            {
                string storedProcedureName = ProcedureName.pkg_DocumentosSCTR + ".REA_EXCLUSION_SEG_PEN";

                List<OracleParameter> parameters = new List<OracleParameter>();
                List<string> insuranceExclusionList = new List<string>();
                OracleParameter table = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameters.Add(table);

                odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameters);
                while (odr.Read())
                {
                    insuranceExclusionList.Add(odr["NOMBRE"].ToString());
                }
                ELog.CloseConnection(odr);

                return insuranceExclusionList;
            }
            catch (Exception ex)
            {
                ELog.CloseConnection(odr);
                LogControl.save("GetInsuranceExclusionList", ex.ToString(), "3");
                return null;
            }
        }
        public List<string> GetInsuranceCoveragePlaceList()
        {
            OracleDataReader odr = null;
            try
            {
                string storedProcedureName = ProcedureName.pkg_DocumentosSCTR + ".REA_LUG_SOL_COBERTURA";

                List<OracleParameter> parameters = new List<OracleParameter>();
                List<string> insuranceCoveragePlaceList = new List<string>();
                OracleParameter table = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameters.Add(table);

                odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameters);
                while (odr.Read())
                {
                    insuranceCoveragePlaceList.Add(odr["NOMBRE"].ToString());
                }
                ELog.CloseConnection(odr);
                return insuranceCoveragePlaceList;
            }
            catch (Exception ex)
            {
                ELog.CloseConnection(odr);
                LogControl.save("GetInsuranceCoveragePlaceList", ex.ToString(), "3");
                return null;
            }
        }
        public List<PaymentModeBM> GetPaymentModeList()
        {
            OracleDataReader odr = null;
            try
            {
                string storedProcedureName = ProcedureName.pkg_DocumentosSCTR + ".REA_METODO_PAGO";

                List<OracleParameter> parameters = new List<OracleParameter>();
                List<PaymentModeBM> paymentModeList = new List<PaymentModeBM>();

                OracleParameter table = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameters.Add(table);

                odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameters);
                while (odr.Read())
                {
                    PaymentModeBM item = new PaymentModeBM();
                    item.ShortName = odr["NOM_CORTO"].ToString();
                    item.LongName = odr["NOM_LARGO"].ToString();

                    paymentModeList.Add(item);
                }
                ELog.CloseConnection(odr);
                return paymentModeList;
            }
            catch (Exception ex)
            {
                ELog.CloseConnection(odr);
                LogControl.save("GetPaymentModeList", ex.ToString(), "3");
                return null;
            }
        }
        public List<InsuredBM> GetInsuredList(TransactionTrack transactionTrack, string productId)
        {
            OracleDataReader odr = null;
            try
            {
                string storedProcedureName = ProcedureName.pkg_DocumentosSCTR + ".REA_LIST_ASEGURADOS";

                List<OracleParameter> parameters = new List<OracleParameter>();
                List<InsuredBM> insuredList = new List<InsuredBM>();
                parameters.Add(new OracleParameter("P_SKEY", OracleDbType.Varchar2, transactionTrack.Key, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Varchar2, productId, ParameterDirection.Input));

                OracleParameter table = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameters.Add(table);

                odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameters);
                while (odr.Read())
                {
                    InsuredBM item = new InsuredBM();
                    item.FirstName = odr["NOMBRE"].ToString();
                    item.DocumentType = odr["TIPO_DOCUMENTO"].ToString();
                    item.PaternalLastName = odr["APE_PATERNO"].ToString().ToUpper();
                    item.MaternalLastName = odr["APE_MATERNO"].ToString().ToUpper();
                    item.DocumentNumber = odr["NRO_DOCUMENTO"].ToString().ToUpper();
                    item.Type = odr["NETEO"].ToString();
                    item.LocationName = odr["NOMBRE_SEDE"].ToString();

                    insuredList.Add(item);
                }
                ELog.CloseConnection(odr);

                return insuredList;
            }
            catch (Exception ex)
            {
                ELog.CloseConnection(odr);
                LogControl.save("GetInsuredList", ex.ToString(), "3");
                return null;
            }
        }
        public List<PolicyRiskDetailBM> GetRiskDetailList(TransactionTrack transactionTrack, string productId)
        {
            OracleDataReader odr = null;
            try
            {
                string storedProcedureName = ProcedureName.pkg_DocumentosSCTR + ".SPS_DOC_DETALLE_RIESGO";

                List<OracleParameter> parameters = new List<OracleParameter>();
                List<PolicyRiskDetailBM> riskDetailList = new List<PolicyRiskDetailBM>();
                parameters.Add(new OracleParameter("P_SKEY", OracleDbType.Varchar2, transactionTrack.Key, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Varchar2, productId, ParameterDirection.Input));

                OracleParameter table = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameters.Add(table);

                odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameters);
                while (odr.Read())
                {
                    PolicyRiskDetailBM item = new PolicyRiskDetailBM();
                    item.WorkersCount = odr["CANT_TRAB"].ToString();
                    item.Rate = String.Format("{0} %", odr["TASA"]);
                    item.RiskType = odr["NOMBRE_RIESGO"].ToString();
                    item.PayRoll = String.Format("{0:#,0.00}", odr["PRIMA"]);  // FormatAmount(odr["PRIMA"].ToString());

                    riskDetailList.Add(item);
                }
                ELog.CloseConnection(odr);

                return riskDetailList;
            }
            catch (Exception ex)
            {
                ELog.CloseConnection(odr);
                LogControl.save("GetRiskDetailList", ex.ToString(), "3");
                return null;
            }
        }

        public List<ReportDataPolicyVM> getReportData(TransactionTrack transactionTrack)
        {
            OracleDataReader odr = null;
            try
            {
                string storedProcedureName = ProcedureName.pkg_ReportesSCTR + ".REA_POLIZA_DOC";

                List<OracleParameter> parameters = new List<OracleParameter>();
                List<ReportDataPolicyVM> reportData = new List<ReportDataPolicyVM>();
                parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int64, transactionTrack.QuotationNumber, ParameterDirection.Input));

                OracleParameter table = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameters.Add(table);

                odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameters);
                while (odr.Read())
                {
                    ReportDataPolicyVM item = new ReportDataPolicyVM();
                    item.policy = odr["NPOLICY"].ToString();
                    item.planilla = odr["NSUMA"].ToString();

                    reportData.Add(item);
                }

                ELog.CloseConnection(odr);

                return reportData;
            }
            catch (Exception ex)
            {
                ELog.CloseConnection(odr);
                LogControl.save("getReportData", ex.ToString(), "3");
                return null;
            }

        }

        public List<UsersDataVM> getReportUsersData(TransactionTrack transactionTrack)
        {
            OracleDataReader odr = null;
            try
            {
                string storedProcedureName = ProcedureName.pkg_ReportesSCTR + ".REA_POLIZA_USERS";

                List<OracleParameter> parameters = new List<OracleParameter>();
                List<UsersDataVM> reportData = new List<UsersDataVM>();
                parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int64, transactionTrack.QuotationNumber, ParameterDirection.Input));

                OracleParameter table = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameters.Add(table);

                odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameters);
                while (odr.Read())
                {
                    UsersDataVM item = new UsersDataVM();
                    item.snombre = odr["SNOMBRE"].ToString();
                    item.sapePat = odr["SAPELLIDOP"].ToString();
                    item.sapeMat = odr["SAPELLIDOM"].ToString();
                    item.slegalName = odr["SLEGALNAME"].ToString();
                    item.sdocumento = odr["SDOCUMENTO"].ToString();
                    item.semail = odr["SEMAIL"].ToString();

                    reportData.Add(item);
                }
                ELog.CloseConnection(odr);

                return reportData;
            }
            catch (Exception ex)
            {
                ELog.CloseConnection(odr);
                LogControl.save("getReportUsersData", ex.ToString(), "3");
                return null;
            }

        }
    }
}
