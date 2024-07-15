using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.DA.PrintPolicyPDF;
using WSPlataforma.Entities.MapfreIntegrationModel.BindingModel;
using WSPlataforma.Entities.MapfreIntegrationModel.ViewModel;
using WSPlataforma.Entities.PrintPolicyModel.BindingModel;
using WSPlataforma.Entities.PrintPolicyModel.ViewModel;
using WSPlataforma.Entities.ReportModel.BindingModel;
using WSPlataforma.Entities.ReportModel.ViewModel;
using WSPlataforma.Entities.Reports.BindingModel;
using WSPlataforma.Entities.Reports.BindingModel.ErrorLog;
using WSPlataforma.Entities.Reports.BindingModel.ReportEntities;
using WSPlataforma.Entities.Reports.ViewModel;
using WSPlataforma.Util;

namespace WSPlataforma.DA
{
    public class PDFGeneratorDA : ConnectionBase
    {
        /// <summary>
        /// Directorio donde se guadarán los archivos
        /// </summary>
        private string rootPath;
        private HtmlStringGenerator htmlStringGenerator = new HtmlStringGenerator();
        private SharedMethods sharedMethods = new SharedMethods();
        private ReportingDataDA reportingData = new ReportingDataDA();
        private MapfreIntegrationDA mapfreIntegration = new MapfreIntegrationDA();

        public List<DataPolicyVM> GetPolizaData(string skey)
        {
            List<DataPolicyVM> response = new List<DataPolicyVM>();
            string ruta_des = String.Empty;

            List<OracleParameter> parameter = new List<OracleParameter>();
            string storedProcedureName = ProcedureName.pkg_SendEmail + ".REA_DATOS_POLIZA";
            try
            {
                parameter.Add(new OracleParameter("P_SKEY", OracleDbType.Varchar2, skey, ParameterDirection.Input));
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));



                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                while (odr.Read())
                {
                    DataPolicyVM obj = new DataPolicyVM();
                    obj.BranchId = odr["NBRANCH"] == DBNull.Value ? 0 : int.Parse(odr["NBRANCH"].ToString());
                    obj.ProductId = odr["NPRODUCT"] == DBNull.Value ? 0 : int.Parse(odr["NPRODUCT"].ToString());
                    obj.TransactTypeId = odr["NTYPE_TRANSAC"] == DBNull.Value ? 0 : int.Parse(odr["NTYPE_TRANSAC"].ToString());
                    response.Add(obj);
                }

                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("GetPolizaData", ex.ToString(), "3");
            }


            return response;
        }

        public async Task<GenericPackageVM> GenerateQuotationDocument(string quotationNumber)
        {
            var response = new GenericPackageVM();
            try
            {
                rootPath = String.Format(ELog.obtainConfig("pathPrincipal"), ELog.obtainConfig("pathCotizacion") + quotationNumber + "\\Split\\");
                response = await Slip_Cotizacion(quotationNumber);

                if (response.StatusCode == 0)
                {
                    response.FolderPath = rootPath;
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GenerateQuotationDocument", ex.ToString(), "3");
            }

            return response;
        }

        public GenericPackageVM GenerateQuotationVL(string quotationNumber)
        {
            GenericPackageVM result = new GenericPackageVM();
            string ruta_des = String.Empty;

            List<OracleParameter> parameter = new List<OracleParameter>();
            string storedProcedureName = ProcedureName.pkg_SendEmail + ".REA_RUTA_COT";
            try
            {
                //INPUT
                //parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, quotationNumber, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_SRUTA_DESTINO = new OracleParameter("P_SRUTA_DESTINO", OracleDbType.Varchar2, ruta_des, ParameterDirection.Output);

                P_SRUTA_DESTINO.Size = 4000;
                parameter.Add(P_SRUTA_DESTINO);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);
                ruta_des = P_SRUTA_DESTINO.Value.ToString();

                result.StatusCode = !String.IsNullOrEmpty(ruta_des) ? 0 : 1;
                result.FolderPath = ruta_des + quotationNumber;

                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                result.StatusCode = 3;
                LogControl.save("GenerateQuotationVL", ex.ToString(), "3");
            }

            return result;
        }

        public GenericPackageVM GenerateQuotationCovid(string quotationNumber, int condicionado)
        {
            GenericPackageVM result = new GenericPackageVM();
            List<PrintPathsVM> response = new List<PrintPathsVM>();
            string ruta_des = String.Empty;

            List<OracleParameter> parameter = new List<OracleParameter>();
            string storedProcedureName = ProcedureName.pkg_CargaMasivaPD + ".REA_PATHS";
            try
            {
                parameter.Add(new OracleParameter("P_NCOD_CONDICIONADO", OracleDbType.Double, condicionado, ParameterDirection.Input));
                parameter.Add(new OracleParameter("C_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, "C_CURSOR", parameter))
                {
                    response = dr.ReadRowsList<PrintPathsVM>();
                    ELog.CloseConnection(dr);
                }

                if (response.Count > 0)
                {
                    result.StatusCode = 0;
                    result.FolderPath = response[0].SRUTA_DESTINO + quotationNumber;
                }
                else
                {
                    result.StatusCode = 1;
                    result.FolderPath = response[0].SRUTA_DESTINO + quotationNumber;
                }
            }
            catch (Exception ex)
            {
                result.StatusCode = 3;
                LogControl.save("GenerateQuotationCovid", ex.ToString(), "3");
            }


            return result;
        }

        public async Task<TransactionTrack> GetTransactionData(string key)
        {
            var transactionTrack = new TransactionTrack();
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                string storedProcedureName = ProcedureName.pkg_DocumentosSCTR + ".REA_TRANS_DATO";

                parameters.Add(new OracleParameter("SKEY", OracleDbType.Varchar2, key, ParameterDirection.Input));

                OracleParameter quotationNumber = new OracleParameter("P_NID_COTIZACION", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                OracleParameter transactionId = new OracleParameter("P_NMOVEMENT", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                OracleParameter transactionType = new OracleParameter("P_NTIPO_TRANSAC", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                OracleParameter clientId = new OracleParameter("P_SCLIENT", OracleDbType.Char, 4000, null, ParameterDirection.Output);
                OracleParameter effectDate = new OracleParameter("P_DEFFECDATE", OracleDbType.Date, 4000, null, ParameterDirection.Output);
                OracleParameter processId = new OracleParameter("P_NID_PROC", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                OracleParameter operationStatus = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter companyId = new OracleParameter("P_NCOMPANY_LNK", OracleDbType.Int32, 4000, null, ParameterDirection.Output); // Mapfre JDD
                OracleParameter QuotationNumberEps = new OracleParameter("P_SCOTIZA_LNK", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output); // Mapfre JDD
                OracleParameter productId = new OracleParameter("P_SPRODUCT", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output); // Mapfre JDD
                OracleParameter branchId = new OracleParameter("P_SBRANCH", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output); // Mapfre JDD
                OracleParameter policyNro = new OracleParameter("P_SPOLICY", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output); // Mapfre JDD
                OracleParameter amountPay = new OracleParameter("P_SCAPITAL", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output); // Mapfre JDD
                OracleParameter ncot_mixta = new OracleParameter("P_MIXTA_SCTR", OracleDbType.Int32, 4000, null, ParameterDirection.Output); // AVS - INTERCONEXION SABSA

                parameters.Add(quotationNumber);
                parameters.Add(transactionId);
                parameters.Add(transactionType);
                parameters.Add(clientId);
                parameters.Add(effectDate);
                parameters.Add(processId);
                parameters.Add(operationStatus);
                parameters.Add(companyId); // JDD
                parameters.Add(QuotationNumberEps); // JDD
                parameters.Add(productId); // JDD
                parameters.Add(branchId);
                parameters.Add(policyNro); // JDD
                parameters.Add(amountPay); // JDD
                parameters.Add(ncot_mixta); // AVS - INTERCONEXION SABSA

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameters);
                transactionTrack.OperationStatus = int.Parse(operationStatus.Value.ToString());
                if (transactionTrack.OperationStatus == 0)
                {
                    transactionTrack.Key = key;
                    transactionTrack.QuotationNumber = quotationNumber.Value.ToString();
                    transactionTrack.TransactionId = transactionId.Value.ToString();
                    transactionTrack.TransactionType = transactionType.Value.ToString();
                    transactionTrack.ClientId = clientId.Value.ToString();
                    transactionTrack.ProcessId = processId.Value.ToString();
                    transactionTrack.EffectDate = (DateTime)(OracleDate)effectDate.Value;
                    transactionTrack.companyId = companyId.Value.ToString(); // JDD
                    transactionTrack.QuotationNumberEps = QuotationNumberEps.Value.ToString(); // JDD
                    transactionTrack.productId = productId.Value.ToString(); // 
                    transactionTrack.branchId = branchId.Value.ToString();
                    transactionTrack.policyNro = policyNro.Value.ToString(); // JDD
                    transactionTrack.amountPay = amountPay.Value.ToString(); // JDD
                    transactionTrack.ncot_mixta = int.Parse(ncot_mixta.Value.ToString()); // AVS - INTERCONEXION SABSA
                    rootPath = String.Format(ELog.obtainConfig("pathPrincipal"), ELog.obtainConfig("pathPoliza") + transactionTrack.QuotationNumber + "\\" + ELog.obtainConfig("movimiento") + "\\" + ELog.obtainConfig("movimiento" + transactionTrack.TransactionType) + "\\" + transactionTrack.TransactionId + "\\");
                    Directory.CreateDirectory(rootPath);
                }
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                transactionTrack.OperationStatus = 3;
                LogControl.save("GetTransactionData", ex.ToString(), "3");
            }

            return await Task.FromResult(transactionTrack);
        }

        /// <summary>
        /// Obtiene la lista de documentos que se deben generar según el tipo de transacción de la póliza
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<GenericPackageVM> GetDocumentTypeList(string quotationNumber, string transactionId)
        {
            GenericPackageVM resultPackage = new GenericPackageVM();
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                List<DocumentVM> list = new List<DocumentVM>();
                string storedProcedureName = ProcedureName.pkg_DocumentosSCTR + ".REA_DOC_TRANSACCION";

                parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Varchar2, quotationNumber, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NTIPO_TRANSAC", OracleDbType.Varchar2, transactionId, ParameterDirection.Input));

                OracleParameter statusCode = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter message = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                OracleParameter table = new OracleParameter("C_TABLE", OracleDbType.RefCursor, 4000, null, ParameterDirection.Output);
                parameters.Add(statusCode);
                parameters.Add(message);
                parameters.Add(table);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameters);
                resultPackage.StatusCode = int.Parse(statusCode.Value.ToString());
                if (resultPackage.StatusCode != 0) resultPackage.ErrorMessageList = sharedMethods.StringToList(message.Value.ToString());
                else
                {
                    while (odr.Read())
                    {
                        DocumentVM item = new DocumentVM();
                        item.Id = odr["NID_DOCS"].ToString();
                        item.Name = odr["DOC_DESCRIPTION"].ToString();
                        list.Add(item);
                    }
                    resultPackage.GenericResponse = list;
                }

                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("GetDocumentTypeList", ex.ToString(), "3");
            }

            return await Task.FromResult(resultPackage);
        }

        public async Task<GenericPackageVM> ManageGenerationAsync(string key)
        {
            var package = new GenericPackageVM();

            try
            {
                TransactionTrack transactionTrack = await GetTransactionData(key);
                if (transactionTrack.OperationStatus == 0)
                {
                    var processToGetDocumentTypeList = await GetDocumentTypeList(transactionTrack.QuotationNumber, transactionTrack.TransactionType);
                    package.ErrorMessageList = new List<string>();

                    if (processToGetDocumentTypeList.StatusCode == 0)
                    {
                        foreach (DocumentVM documentType in processToGetDocumentTypeList.GenericResponse)
                        {
                            switch (documentType.Id)
                            {
                                case "1":
                                    // Salud JDD
                                    if(transactionTrack.companyId == ELog.obtainConfig("grandiaKey")) //AVS - INTERCONEXION SABSA
                                    {
                                        package = await Contrato_Salud(package, transactionTrack);
                                    }
                                    else if (transactionTrack.companyId == ELog.obtainConfig("protectaKey"))
                                    {
                                        package = await Contrato_Salud(package, transactionTrack);
                                    }
                                    break;
                                case "2":
                                    // Salud JDD
                                    if (transactionTrack.companyId == ELog.obtainConfig("grandiaKey")) //AVS - INTERCONEXION SABSA
                                    {
                                        package = await Constancia(package, transactionTrack, ELog.obtainConfig("saludKey"), "A");
                                    }
                                    else if(transactionTrack.companyId == ELog.obtainConfig("protectaKey"))
                                    {
                                        package = await Constancia(package, transactionTrack, ELog.obtainConfig("saludKey"), "A");
                                    }
                                    break;
                                case "3":
                                    // Salud JDD
                                    if (transactionTrack.companyId == ELog.obtainConfig("grandiaKey")) //AVS - INTERCONEXION SABSA
                                    {
                                        package = await Proforma(package, transactionTrack, ELog.obtainConfig("saludKey"));
                                    }
                                    else if(transactionTrack.companyId == ELog.obtainConfig("protectaKey"))
                                    {
                                        package = await Proforma(package, transactionTrack, ELog.obtainConfig("saludKey"));
                                    }
                                    break;
                                case "4":
                                    package = await Solicitud_Pension(package, transactionTrack);
                                    break;
                                case "5":
                                    package = await Condiciones_Generales_Pension(package);
                                    break;
                                case "6":
                                    package = await Condiciones_Particulares_Pension(package, transactionTrack);
                                    break;
                                case "7":
                                    package = await Resumen_Informativo_Pension(package);
                                    break;
                                case "8":
                                    package = await Constancia(package, transactionTrack, ELog.obtainConfig("pensionKey"), "A");
                                    break;
                                case "9":
                                    package = await Proforma(package, transactionTrack, ELog.obtainConfig("pensionKey"));
                                    break;
                                case "10":
                                    package = await Constancia(package, transactionTrack, "0", "A");
                                    break;
                                case "12":
                                    package = await Constancia(package, transactionTrack, ELog.obtainConfig("saludKey"), "E");
                                    break;
                                case "13":
                                    package = await Constancia(package, transactionTrack, ELog.obtainConfig("pensionKey"), "E");
                                    break;
                                case "14":
                                    package = await Constancia(package, transactionTrack, "0", "E");
                                    break;
                                case "15":
                                    package = await Constancia(package, transactionTrack, ELog.obtainConfig("saludKey"), "I");
                                    break;
                                case "16":
                                    package = await Constancia(package, transactionTrack, ELog.obtainConfig("pensionKey"), "I");
                                    break;
                                case "17":
                                    package = await Constancia(package, transactionTrack, "0", "I");
                                    break;
                            }

                        }

                        // Mapfre JDD
                        if (transactionTrack.companyId == ELog.obtainConfig("mapfreKey") && transactionTrack.QuotationNumberEps != null && package.StatusCode == 0)
                        {
                            Directory.CreateDirectory(rootPath);
                            List<string> listDocumentos = ELog.obtainConfig(String.Format("transaccion{0}", transactionTrack.TransactionType)).Split(';').ToList();
                            DocumentosVM data = await getDataPolizaMapfre(transactionTrack);

                            foreach (var documento in listDocumentos)
                            {
                                if (package.StatusCode == 0)
                                {
                                    package = await gestionarDocumentosMapfre(package, transactionTrack, data, documento);
                                }
                            }
                        }


                        if (package.StatusCode == 0 && package.ErrorMessageList.Count == 0)
                        {
                            package.FolderPath = rootPath;

                            bool generate = false;
                            string[] epolicyEps = ELog.obtainConfig("epolicyEps" + transactionTrack.companyId).Split(';');
                            string[] productList = transactionTrack.productId.Split('/');

                            for (int i = 0; i < epolicyEps.Count(); i++)
                            {
                                epolicyEps[i] = epolicyEps[i].Trim();
                                for (int y = 0; y < productList.Count(); y++)
                                {
                                    productList[y] = productList[y].Trim();
                                    if (epolicyEps[i] == productList[y])
                                    {
                                        generate = true;
                                        break;
                                    }
                                    else
                                    {
                                        generate = false;
                                    }
                                }
                                if (generate) break;
                            }


                            if (generate)
                            {
                                var response = generarPolizaElectronica(transactionTrack, rootPath);
                            }

                        }
                        else
                        {
                            package.StatusCode = 3;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                package.StatusCode = 3;
                LogControl.save("ManageGenerationAsync", ex.ToString(), "3");
            }

            return package;
        }

        public async Task<ResponsePDFVM> generarPolizaElectronica(TransactionTrack data, string url)
        {
            var response = new ResponsePDFVM();

            if (!String.IsNullOrEmpty(url))
            {
                Stopwatch s = new Stopwatch();
                s.Start();
                while (s.Elapsed < TimeSpan.FromSeconds(Double.Parse("3"))) { }
                s.Stop();

                var listFiles = await UnirListaPdf(url);

                string[] policyList = data.policyNro.Split('/');
                string[] productList = data.productId.Split('/');
                string[] amountList = data.amountPay.Split('/');
                string[] epolicyEps = ELog.obtainConfig("epolicyEps" + data.companyId).Split(';');
                List<string[]> infolist = new List<string[]>();
                infolist.Add(policyList);
                infolist.Add(productList);
                infolist.Add(epolicyEps);
                infolist.Add(amountList);

                if (policyList.Count() > 0)
                {
                    var infoPolicy = new PrintGenerateBM();
                    infoPolicy.NBRANCH = 77;
                    infoPolicy.NPRODUCT = productList[0].Trim();
                    infoPolicy.NPOLICY = Convert.ToInt64(policyList[0].Trim());
                    infoPolicy.NMOVEMENT = data.TransactionId;
                    var dataUsers = new PrintPolicyDA().GetUsersPolicyList(infoPolicy);
                    response = sendDocumentsService(data, dataUsers, infolist);
                }
            }

            return response;
        }

        private ResponsePDFVM sendDocumentsService(TransactionTrack data, List<UserInfoVM> dataUsers, List<string[]> infolist)
        {
            ResponsePDFVM response = new ResponsePDFVM();
            try
            {
                WsGenerarPDFReference.WsGenerarPDFexternoClient client = new WsGenerarPDFReference.WsGenerarPDFexternoClient();
                List<ReportDocPDF> listDoc = new List<ReportDocPDF>();
                List<ReportUserPDF> listUser = new List<ReportUserPDF>();

                if (data != null && dataUsers != null)
                {
                    for (int i = 0; i < infolist[2].Count(); i++)
                    {
                        infolist[2][i] = infolist[2][i].Trim();
                        for (int y = 0; y < infolist[1].Count(); y++)
                        {
                            infolist[1][y] = infolist[1][y].Trim();
                            if (infolist[2][i] == infolist[1][y])
                            {
                                var item = new ReportDocPDF();
                                item.Nombre = ELog.obtainConfig("outPoliza");
                                item.Poliza = infolist[0][y];
                                item.SumaAsegurada = ELog.obtainConfig("amountActive") == "1" ? infolist[3][y] : "0";
                                item.Certificado = "1";
                                item.NombrePlantilla = ELog.obtainConfig("planName" + data.branchId + infolist[1][y]);
                                listDoc.Add(item);
                            }
                        }
                    }

                    foreach (var userItem in dataUsers)
                    {
                        if (!string.IsNullOrEmpty(userItem.sdocumento.Trim()) && !string.IsNullOrEmpty(userItem.semail.Trim()))
                        {
                            ReportUserPDF user = new ReportUserPDF();
                            user.Nombre = userItem.slegalName == "" ? userItem.snombre.Trim() : userItem.slegalName.Trim();
                            user.Apellido = (userItem.sapePat.Trim() + " " + userItem.sapeMat).Trim();
                            user.Correo = userItem.semail == "" ? "" : userItem.semail.Trim();
                            user.DNI = userItem.sdocumento.Trim();
                            user.Documentos = new List<Documento>();
                            var item = new Documento();
                            item.nombre = ELog.obtainConfig("outPoliza");
                            user.Documentos.Add(item);
                            listUser.Add(user);
                        }
                    }
                }

                string informacionDocumentos = JsonConvert.SerializeObject(listDoc);
                string informacionUsuarios = JsonConvert.SerializeObject(listUser);
                string rutaDirectorio = ELog.obtainConfig("pathImpresionSctr") + "" + ELog.obtainConfig("pathPoliza") + data.QuotationNumber + "\\" + ELog.obtainConfig("movimiento") + "\\" + ELog.obtainConfig("movimiento" + data.TransactionType) + "\\" + data.TransactionId + "\\";
                //string rutaDirectorio = @"\\PRTSRV33\Documentos WS externo";
                var res = client.CargaDocumentoExterno(informacionDocumentos, informacionUsuarios, rutaDirectorio);
                response = JsonConvert.DeserializeObject<ResponsePDFVM>(res, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                if (response.Estatus == "ERROR") response.Error = response.Error + " | Ruta: " + rutaDirectorio;
                if (response.Estatus == "OK") response.Estatus = response.Estatus + " | Ruta: " + rutaDirectorio;
                //ELog.save(this, listUser[0].Documentos[0].nombre);
                LogControl.save("sendDocumentsService", "InformacionDocumentos" + informacionDocumentos, "2");
                LogControl.save("sendDocumentsService", "informacionUsuarios" + informacionUsuarios, "2");
                LogControl.save("sendDocumentsService", "rutaDirectorio" + rutaDirectorio, "2");
                LogControl.save("sendDocumentsService", "error" + response.Error, "2");
                LogControl.save("sendDocumentsService", "poliza_electronica" + response.Estatus, "2");
            }
            catch (Exception ex)
            {
                LogControl.save("sendDocumentsService", ex.ToString(), "3");
            }

            return response;
        }

        public Task<List<string>> UnirListaPdf(string urlBase)
        {
            List<string> OutFilesList = new List<string>();

            string[] listPoliza = ELog.obtainConfig("orderPoliza").Split(';');
            string listFirma = ELog.obtainConfig("outPoliza") + ".pdf";

            List<string> InFilesPoliza = new List<string>();
            listPoliza.ToList().ForEach(file =>
            {
                InFilesPoliza.Add(Directory.GetFiles(urlBase, file).ToList().Count > 0 ? Directory.GetFiles(urlBase, file)[0] : "");
            });

            OutFileBM OutFiles = new OutFileBM();
            OutFiles.outFile = urlBase + listFirma;
            OutFiles.inFile = InFilesPoliza;

            try
            {
                byte[] mergedPdf = null;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (Document document = new Document())
                    {
                        using (PdfCopy copy = new PdfCopy(document, ms))
                        {
                            document.Open();

                            for (var y = 0; y < OutFiles.inFile.Count(); y++)
                            {
                                if (!String.IsNullOrEmpty(OutFiles.inFile[y]))
                                {
                                    PdfReader reader = new PdfReader(OutFiles.inFile[y]);
                                    // loop over the pages in that document
                                    int n = reader.NumberOfPages;
                                    for (int page = 0; page < n;)
                                    {
                                        copy.AddPage(copy.GetImportedPage(reader, ++page));
                                    }
                                    reader.Close();
                                }
                            }
                        }
                    }
                    mergedPdf = ms.ToArray();
                    System.IO.File.WriteAllBytes(OutFiles.outFile, mergedPdf);
                    OutFilesList.Add(OutFiles.outFile);
                }
            }
            catch (Exception ex)
            {
                LogControl.save("UnirListaPdf", ex.ToString(), "3");
            }

            return Task.FromResult(OutFilesList);
        }

        private async Task<GenericPackageVM> gestionarDocumentosMapfre(GenericPackageVM response, TransactionTrack transactionTrack, DocumentosVM data, string documento)
        {
            var fileName = documento;
            try
            {
                var itemDocumento = new CetifDocumGenVM();
                var item = new CetifDocumGenBM();

                if ("polizaDocumentoKey" == documento)
                {
                    item.codAgt = ELog.obtainConfig("codAgente");
                    item.codCia = ELog.obtainConfig("codCia");
                    item.numApli = data.nnumaplicacion;
                    item.numPoliza = data.numPoliza;
                    item.numSpto = data.nnumspto;
                    item.numSptoApli = data.nnumsptoapli;
                    item.tipoImpresion = ELog.obtainConfig("tipoImpresion");
                    item.urlService = String.Format(ELog.obtainConfig("mapfreCore"), ELog.obtainConfig(documento));
                    item.urlLogin = String.Format(ELog.obtainConfig("mapfreCore"), ELog.obtainConfig("login"));
                    itemDocumento = await mapfreIntegration.GesDocumentoMapfre(item);
                }
                else if ("reciboDocumentoKey" == documento)
                {
                    item.nroRecibo = data.nroRecibo;
                    item.urlService = String.Format(ELog.obtainConfig("mapfreCore"), ELog.obtainConfig(documento));
                    item.urlLogin = String.Format(ELog.obtainConfig("mapfreCore"), ELog.obtainConfig("login"));
                    itemDocumento = await mapfreIntegration.GesDocumentoMapfre(item);
                }
                else
                {
                    item.nroConstancia = data.nroConstancia;
                    item.numeroPoliza = data.numPoliza;
                    item.urlService = String.Format(ELog.obtainConfig("mapfreRrtt"), ELog.obtainConfig(documento)); ;
                    item.urlLogin = String.Format(ELog.obtainConfig("mapfreRrtt"), ELog.obtainConfig("login"));
                    itemDocumento = await mapfreIntegration.GesDocumentoMapfre(item);
                }

                if (itemDocumento != null)
                {
                    if (itemDocumento.codError == "0")
                    {
                        byte[] sPDFDecoded = Convert.FromBase64String(itemDocumento.documento);
                        string OutputPath = rootPath + fileName + ".pdf";
                        File.WriteAllBytes(OutputPath, sPDFDecoded);
                        response.StatusCode = 0;
                    }
                    else
                    {
                        response.StatusCode = 3;
                        response.ErrorMessageList.Add("Error en " + fileName + ". Mensaje: " + itemDocumento.descError);
                    }

                }
                else
                {
                    response.StatusCode = 3;
                    response.ErrorMessageList.Add("Error en " + fileName + ". Mensaje: Error en servicio Mapfre");
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 3;
                response.ErrorMessageList.Add("Error en " + fileName + ". Mensaje: " + ex.Message);
                LogControl.save("gestionarDocumentosMapfre", ex.ToString(), "3");
            }
            return response;
        }

        private async Task<DocumentosVM> getDataPolizaMapfre(TransactionTrack transactionTrack)
        {
            DocumentosVM response = new DocumentosVM();
            string storedProcedureName = ProcedureName.pkg_IntegraSCTR + ".SEL_CONSTANCIA_MPE"; // Modificar 
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                // INPUT
                parameters.Add(new OracleParameter("p_scotiza_lnk", OracleDbType.Varchar2, transactionTrack.QuotationNumberEps, ParameterDirection.Input));
                parameters.Add(new OracleParameter("p_stipmovimiento", OracleDbType.Varchar2, ELog.obtainConfig(String.Format("equiTransaccion{0}", transactionTrack.TransactionType)), ParameterDirection.Input));
                // OUTPUT
                OracleParameter numPoliza = new OracleParameter("p_spolicy_mpe", OracleDbType.Varchar2, 4000, response.numPoliza, ParameterDirection.Output);
                OracleParameter nroConstancia = new OracleParameter("p_sconstancia_mpe", OracleDbType.Varchar2, 4000, response.nroConstancia, ParameterDirection.Output);
                OracleParameter nroRecibo = new OracleParameter("p_snumrecibo_mpe", OracleDbType.Varchar2, 4000, response.nroRecibo, ParameterDirection.Output);
                OracleParameter nnumaplicacion = new OracleParameter("p_nnumaplicacion", OracleDbType.Int32, 4000, response.nnumaplicacion, ParameterDirection.Output);
                OracleParameter nnumspto = new OracleParameter("p_nnumspto", OracleDbType.Int32, 4000, response.nnumspto, ParameterDirection.Output);
                OracleParameter nnumsptoapli = new OracleParameter("p_nnumsptoapli", OracleDbType.Int32, 4000, response.nnumsptoapli, ParameterDirection.Output);
                OracleParameter mensaje = new OracleParameter("p_mensaje", OracleDbType.Varchar2, 4000, response.mensaje, ParameterDirection.Output);

                parameters.Add(numPoliza);
                parameters.Add(nroConstancia);
                parameters.Add(nroRecibo);
                parameters.Add(nnumaplicacion);
                parameters.Add(nnumspto);
                parameters.Add(nnumsptoapli);
                parameters.Add(mensaje);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameters);
                response.numPoliza = numPoliza.Value.ToString();
                response.nroConstancia = nroConstancia.Value.ToString();
                response.nroRecibo = nroRecibo.Value.ToString();
                response.nnumaplicacion = nnumaplicacion.Value.ToString();
                response.nnumspto = nnumspto.Value.ToString();
                response.nnumsptoapli = nnumsptoapli.Value.ToString();
                response.mensaje = mensaje.Value.ToString();

                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                response = null;
                LogControl.save("getDataPolizaMapfre", ex.ToString(), "3");
            }

            return await Task.FromResult(response);
        }

        private void processPDF(string htmlHeaderPath, string htmlFooterPath, string outputPath, string htmlBodyString)
        {
            if (!String.IsNullOrEmpty(htmlBodyString))
            {
                String PDFGenerador = ELog.obtainConfig("pathToExePdf");
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.UseShellExecute = false;
                psi.CreateNoWindow = true;
                psi.RedirectStandardInput = true;
                psi.RedirectStandardOutput = true;
                psi.RedirectStandardError = true;
                psi.FileName = PDFGenerador;

                String config = "--print-media-type ";
                config += "--margin-top 20mm --margin-bottom 10mm --margin-right 10mm --margin-left 10mm ";
                config += "--page-size A4 ";
                if (htmlHeaderPath != null) config += "--header-html " + htmlHeaderPath + " --header-spacing 10 -T 25mm";
                if (htmlFooterPath != null) config += " --footer-html " + htmlFooterPath + " ";
                config += " -q -n - ";
                psi.Arguments = config + outputPath;

                Directory.CreateDirectory(rootPath);
                Process p = Process.Start(psi);

                System.IO.StreamWriter stdin;
                stdin = p.StandardInput;
                stdin.AutoFlush = true;

                stdin.Write(htmlBodyString);
                stdin.Close();

                p.Close();
                p.Dispose();
            }
        }

        public async Task<GenericPackageVM> Slip_Cotizacion(string quotationNumber)
        {
            var response = new GenericPackageVM();
            string fileName = String.Empty;

            try
            {
                var datosSplit = reportingData.Slip_Cotizacion(quotationNumber);

                if (datosSplit != null)
                {
                    fileName = datosSplit.IsMining ? "slip-mina.pdf" : "slip-no-mina.pdf";
                    string OutputPath = rootPath + fileName;
                    string htmlHeaderPath = String.Empty;

                    if (datosSplit.Company == "1")
                    {
                        htmlHeaderPath = AppDomain.CurrentDomain.BaseDirectory + @"\Others\Headers\Both.html";
                    }
                    else
                    {
                        htmlHeaderPath = AppDomain.CurrentDomain.BaseDirectory + @"\Others\Headers\Both_mapfre.html";
                    }

                    processPDF(htmlHeaderPath, null, OutputPath, htmlStringGenerator.Slip_Cotizacion(datosSplit, AppDomain.CurrentDomain.BaseDirectory + @"\Others\Templates\slip_cotizacion.html"));
                    response.StatusCode = 0;
                }
                else
                {
                    response.StatusCode = 3;
                    response.ErrorMessageList.Add("Error en split. Mensaje: No genera datos en split");
                }

            }
            catch (Exception ex)
            {
                response.StatusCode = 3;
                response.ErrorMessageList.Add("Error en " + fileName + ". Mensaje: " + ex.Message);
                LogControl.save("Slip_Cotizacion", ex.ToString(), "3");
            }

            return await Task.FromResult(response);
        }
        public async Task<GenericPackageVM> Constancia(GenericPackageVM response, TransactionTrack transactionTrack, string productId, string IOMode)
        {
            string fileName = String.Empty;
            try
            {
                //Ruta a Guardar
                var OutputPath = String.Empty;
                string htmlHeaderPath = String.Empty;
                if (productId == ELog.obtainConfig("saludKey"))
                {
                    fileName = IOMode == "I" ? "constancia-inclusion-salud.pdf" : IOMode == "E" ? "constancia-exclusion-salud.pdf" : "constancia-salud.pdf";
                    OutputPath = rootPath + fileName;
                    htmlHeaderPath = AppDomain.CurrentDomain.BaseDirectory + @"\Others\Headers\Sanitas.html";
                }
                else if (productId == ELog.obtainConfig("pensionKey"))
                {
                    fileName = IOMode == "I" ? "constancia-inclusion-pension.pdf" : IOMode == "E" ? "constancia-exclusion-pension.pdf" : "constancia-pension.pdf";
                    OutputPath = rootPath + fileName;
                    htmlHeaderPath = AppDomain.CurrentDomain.BaseDirectory + @"\Others\Headers\Protecta.html";
                }
                else
                {
                    fileName = IOMode == "I" ? "constancia-inclusion-conjunta.pdf" : IOMode == "E" ? "constancia-exclusion-conjunta.pdf" : "constancia-conjunta.pdf";
                    OutputPath = rootPath + fileName;
                    htmlHeaderPath = AppDomain.CurrentDomain.BaseDirectory + @"\Others\Headers\Both.html";

                }
                Constancia data = await reportingData.Constancia(transactionTrack);
                processPDF(htmlHeaderPath, null, OutputPath, htmlStringGenerator.Constancia(data, productId, IOMode, AppDomain.CurrentDomain.BaseDirectory + @"\Others\Templates\constancia.html"));

                response.StatusCode = 0;
            }
            catch (Exception ex)
            {
                response.StatusCode = 1;
                response.ErrorMessageList.Add("Error en " + fileName + ". Mensaje: " + ex.Message);
                LogControl.save("Constancia", ex.ToString(), "3");
            }
            return response;
        }

        public async Task<GenericPackageVM> Contrato_Salud(GenericPackageVM response, TransactionTrack transactionTrack)
        {
            string fileName = "contrato-salud.pdf";
            try
            {
                Contrato data = await reportingData.Contrato(transactionTrack);

                string OutputPath = rootPath + fileName;

                processPDF(AppDomain.CurrentDomain.BaseDirectory + @"\Others\Headers\Sanitas.html", null, OutputPath, htmlStringGenerator.Contrato_Salud(data, AppDomain.CurrentDomain.BaseDirectory + @"\Others\Templates\contrato.html"));

                response.StatusCode = 0;
            }
            catch (Exception ex)
            {
                response.StatusCode = 3;
                LogControl.save("Contrato_Salud", ex.ToString(), "3");
            }

            return response;
        }


        public async Task<GenericPackageVM> Solicitud_Pension(GenericPackageVM response, TransactionTrack transactionTrack)
        {
            string fileName = "solicitud-pension.pdf";
            try
            {
                Solicitud_Pension data = await reportingData.Solicitud_Pension(transactionTrack);

                string OutputPath = rootPath + fileName;

                processPDF(AppDomain.CurrentDomain.BaseDirectory + @"\Others\Headers\Protecta.html", null, OutputPath, htmlStringGenerator.Solicitud_Pension(data, AppDomain.CurrentDomain.BaseDirectory + @"\Others\Templates\solicitud_pension.html"));
                response.StatusCode = 0;
            }
            catch (Exception ex)
            {
                response.StatusCode = 3;
                response.ErrorMessageList.Add("Error en " + fileName + ". Mensaje: " + ex.Message);
                LogControl.save("Solicitud_Pension", ex.ToString(), "3");
            }

            return response;

        }
        public async Task<GenericPackageVM> Proforma(GenericPackageVM response, TransactionTrack transactionTrack, string productId)
        {
            string fileName = (productId == ELog.obtainConfig("saludKey")) ? "proforma_salud.pdf" : "proforma_pension.pdf";
            try
            {
                Proforma data = await reportingData.Proforma(transactionTrack, productId);

                string OutputPath = rootPath + fileName;

                processPDF(null, null, OutputPath, htmlStringGenerator.Proforma(data, AppDomain.CurrentDomain.BaseDirectory + @"\Others\Templates\proforma.html"));
                response.StatusCode = 0;
            }
            catch (Exception ex)
            {
                response.StatusCode = 3;
                response.ErrorMessageList.Add("Error en " + fileName + ". Mensaje: " + ex.Message);
                LogControl.save("Proforma", ex.ToString(), "3");
            }

            return response;
        }
        public async Task<GenericPackageVM> Condiciones_Particulares_Pension(GenericPackageVM response, TransactionTrack transactionTrack)
        {
            string fileName = "condiciones-particulares-pension.pdf";
            try
            {
                Condiciones_Particulares_Pension data = await reportingData.Condiciones_Particulares_Pension(transactionTrack);

                string OutputPath = rootPath + fileName;

                processPDF(AppDomain.CurrentDomain.BaseDirectory + @"\Others\Headers\Protecta.html", AppDomain.CurrentDomain.BaseDirectory + @"\Others\Footers\condiciones_particulares_pension.html", OutputPath, htmlStringGenerator.Condiciones_Particulares_Pension(data, AppDomain.CurrentDomain.BaseDirectory + @"\Others\Templates\condiciones_particulares_pension.html"));

                response.StatusCode = 0;
            }
            catch (Exception ex)
            {
                response.StatusCode = 3;
                response.ErrorMessageList.Add("Error en " + fileName + ". Mensaje: " + ex.Message);
                LogControl.save("Condiciones_Particulares_Pension", ex.ToString(), "3");
            }

            return response;
        }

        private string GetFilePath(string name)
        {
            List<OracleParameter> parameters = new List<OracleParameter>();
            string storedProcedureName = ProcedureName.pkg_DocumentosSCTR + ".rea_documento_ruta";

            parameters.Add(new OracleParameter("P_NOMBRE", OracleDbType.Varchar2, name, ParameterDirection.Input));

            OracleParameter path = new OracleParameter("P_RUTA", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
            parameters.Add(path);
            ExecuteByStoredProcedureVT(storedProcedureName, parameters);
            return path.Value.ToString();
        }

        public async Task<GenericPackageVM> Condiciones_Generales_Pension(GenericPackageVM response)
        {
            string fileName = "condiciones-generales-pension.pdf";
            try
            {
                //archivo estático
                string sourceFilePath = AppDomain.CurrentDomain.BaseDirectory + GetFilePath("RUTA_DOC_CGP");
                //Ruta a Guardar
                string OutputPath = rootPath + fileName;
                File.Copy(sourceFilePath, OutputPath, true);
                response.StatusCode = 0;
            }
            catch (Exception ex)
            {
                response.StatusCode = 3;
                response.ErrorMessageList.Add("Error en " + fileName + ". Mensaje: " + ex.Message);
                LogControl.save("Condiciones_Generales_Pension", ex.ToString(), "3");
            }

            return await Task.FromResult(response);

        }
        public async Task<GenericPackageVM> Resumen_Informativo_Pension(GenericPackageVM response)
        {
            string fileName = "resumen-informativo-pension.pdf";
            try
            {
                //archivo estático
                string sourceFilePath = AppDomain.CurrentDomain.BaseDirectory + GetFilePath("RUTA_DOC_RIP");
                //Ruta a Guardar
                string OutputPath = rootPath + fileName;

                File.Copy(sourceFilePath, OutputPath, true);
                response.StatusCode = 0;
                //response.filePdf.OperationStatus = 0;
                //response.filePdf.FilePath = OutputPath;
                //return await Task.FromResult(new GeneratedFileVM { OperationStatus = 0, FilePath = OutputPath });
            }
            catch (Exception ex)
            {
                response.StatusCode = 3;
                response.ErrorMessageList.Add("Error en " + fileName + ". Mensaje: " + ex.Message);
                LogControl.save("Resumen_Informativo_Pension", ex.ToString(), "3");
            }

            return await Task.FromResult(response);
        }
    }
}
