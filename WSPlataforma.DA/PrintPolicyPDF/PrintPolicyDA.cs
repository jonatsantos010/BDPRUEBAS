using Newtonsoft.Json;
using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using WSPlataforma.Entities.PrintPolicyModel;
using WSPlataforma.Entities.PrintPolicyModel.ATP;
using WSPlataforma.Entities.PrintPolicyModel.BindingModel;
using WSPlataforma.Entities.PrintPolicyModel.ViewModel;
using WSPlataforma.Entities.ReportModel.BindingModel;
using WSPlataforma.Entities.ReportModel.ViewModel;
using WSPlataforma.Util;
using WSPlataforma.Util.PrintPolicyUtility;
using WSPlataforma.Entities.VDPModel.BindingModel;

namespace WSPlataforma.DA.PrintPolicyPDF
{
    public class PrintPolicyDA : ConnectionBase
    {
        public ResponsePrintVM PrintPDF(PrintGenerateBM printGenerateBM)
        {
            ResponsePrintVM response = new ResponsePrintVM();
            try
            {
                List<PrintPathsVM> printPathsVM = GetPathsPrint(printGenerateBM.NCOD_CONDICIONADO, Convert.ToInt32(printGenerateBM.NIDHEADERPROC));

                printGenerateBM.NSTATE = Convert.ToInt32(PrintEnum.State.EN_PROCESOCP);
                printGenerateBM.NOPC = 1;
                response = UpdateDocumentProcessState(printGenerateBM);

                if (new int[] { (int)PrintEnum.Condicionado.SOLICITUD, (int)PrintEnum.Condicionado.SOLICITUD_LEY,
                                (int)PrintEnum.Condicionado.SOLICITUD_COMPLETO, (int)PrintEnum.Condicionado.SOLICITUD_ESPECIAL,
                                (int)PrintEnum.Condicionado.COVID_SOLICITUD, (int)PrintEnum.Condicionado.AP_SOLICITUD,
                                (int)PrintEnum.Condicionado.SOLICITUD_SCTR, (int) PrintEnum.Condicionado.SOLICITUD_SCTR_SIN_SUELDO,
                                (int)PrintEnum.Condicionado.SOLICITUD_AP, (int)PrintEnum.Condicionado.SOLICITUD_VILP }.Contains((int)printGenerateBM.NCOD_CONDICIONADO))
                {
                    response = new GenerateDocumentsDA().RequestPDF(printGenerateBM, printPathsVM[0]);
                }
                else if (new int[] { (int)PrintEnum.Condicionado.CONDICIONES_PARTICULARES, (int)PrintEnum.Condicionado.COVID_CON_PARTICULAR,
                                     (int)PrintEnum.Condicionado.AP_CON_PARTICULAR, (int)PrintEnum.Condicionado.CONDICIONES_PARTICULARES_SCTR,
                                     (int)PrintEnum.Condicionado.CONDICIONES_PARTICULARES_AP, (int)PrintEnum.Condicionado.CONDICIONES_PARTICULARES_VG,
                                     (int)PrintEnum.Condicionado.CONDICIONES_PARTICULARES_VILP}.Contains((int)printGenerateBM.NCOD_CONDICIONADO))
                {
                    response = new GenerateDocumentsDA().ParticularConditionsPDF(printGenerateBM, printPathsVM[0]);
                }
                //else if (printGenerateBM.NCOD_CONDICIONADO == (int)PrintEnum.Condicionado.CONDICIONES_PARTICULARES_AP ||
                //         printGenerateBM.NCOD_CONDICIONADO == (int)PrintEnum.Condicionado.CONDICIONES_PARTICULARES_VG)
                //{
                //    List<FormatsDetailVM> formatsDetailVMs = GetFormatDetail(
                //                                              new PrintFormatsDetailBM
                //                                              {
                //                                                  NBRANCH = printGenerateBM.NBRANCH,
                //                                                  NPRODUCT = printGenerateBM.NPRODUCT,
                //                                                  NPOLICY = printGenerateBM.NPOLICY,
                //                                                  NCOD_CONDICIONADO = printGenerateBM.NCOD_CONDICIONADO
                //                                              }
                //                                           );
                //response = new GenerateDocumentsDA().CondicionesCertificatePDF(printGenerateBM, printPathsVM[0], formatsDetailVMs);
                //}
                else if (new int[] { (int)PrintEnum.Condicionado.CONDICIONES_GENERALES, (int)PrintEnum.Condicionado.COVID_CON_GENERAL,
                                     (int)PrintEnum.Condicionado.AP_CON_GENERAL, (int)PrintEnum.Condicionado.CONDICIONES_GENERALES_SCTR,
                                     (int)PrintEnum.Condicionado.CONDICIONES_GENERALES_AP, (int)PrintEnum.Condicionado.CONDICIONES_GENERALES_VG,
                                     (int)PrintEnum.Condicionado.CONDICIONES_GENERALES_VILP }.Contains((int)printGenerateBM.NCOD_CONDICIONADO))
                {
                    response = new GenerateDocumentsDA().GeneralConditionsPDF(printGenerateBM, printPathsVM[0]);
                }
                else if (new int[] { (int)PrintEnum.Condicionado.CLAUSULAS_ADICIONALES, (int)PrintEnum.Condicionado.CLAUSULAS_ADICIONALES_BASICO,
                                     (int)PrintEnum.Condicionado.CLAUSULAS_ADICIONALES_COMPLETO , (int)PrintEnum.Condicionado.CLAUSULAS_ADICIONALES_ESPECIAL,
                                     (int)PrintEnum.Condicionado.COVID_CON_ESPECIAL, (int)PrintEnum.Condicionado.CONDICIONES_ESPECIALES_AP,
                                     (int)PrintEnum.Condicionado.CONDICIONES_ESPECIALES_VG/*, (int)PrintEnum.Condicionado.CONDICIONES_GENERALES_VILP*/}.Contains((int)printGenerateBM.NCOD_CONDICIONADO))
                {
                    List<FormatsDetailVM> formatsDetailVMs = GetFormatDetail(
                                                                new PrintFormatsDetailBM
                                                                {
                                                                    NBRANCH = printGenerateBM.NBRANCH,
                                                                    NPRODUCT = printGenerateBM.NPRODUCT,
                                                                    NPOLICY = printGenerateBM.NPOLICY,
                                                                    NCOD_CONDICIONADO = printGenerateBM.NCOD_CONDICIONADO
                                                                }
                                                             );
                    //ELog.save(this, "ANTES DE COBERTURAS");
                    response = new GenerateDocumentsDA().DetailDocumentsPDF(printGenerateBM, printPathsVM[0], formatsDetailVMs);
                }
                else if (new int[] { (int)PrintEnum.Condicionado.POLIZA_ELECTRONICA }.Contains((int)printGenerateBM.NCOD_CONDICIONADO))
                {
                    response = new GenerateDocumentsDA().ElectronicPolicyPDF(printGenerateBM, printPathsVM[0]);
                }
                else if (new int[] { (int)PrintEnum.Condicionado.RESUMEN, (int)PrintEnum.Condicionado.RESUMEN_BASICO,
                                     (int)PrintEnum.Condicionado.RESUMEN_COMPLETO, (int)PrintEnum.Condicionado.RESUMEN_ESPECIAL,
                                     (int)PrintEnum.Condicionado.COVID_RESUMEN, (int)PrintEnum.Condicionado.AP_RESUMEN,
                                     (int)PrintEnum.Condicionado.RESUMEN_SCTR, (int)PrintEnum.Condicionado.RESUMEN_AP,
                                     (int)PrintEnum.Condicionado.RESUMEN_VG, (int)PrintEnum.Condicionado.RESUMEN_VILP }.Contains((int)printGenerateBM.NCOD_CONDICIONADO))
                {
                    List<FormatsDetailVM> formatsDetailVMs = GetFormatDetail(
                                                              new PrintFormatsDetailBM
                                                              {
                                                                  NBRANCH = printGenerateBM.NBRANCH,
                                                                  NPRODUCT = printGenerateBM.NPRODUCT,
                                                                  NPOLICY = printGenerateBM.NPOLICY,
                                                                  NCOD_CONDICIONADO = printGenerateBM.NCOD_CONDICIONADO
                                                              }
                                                           );
                    response = new GenerateDocumentsDA().SummaryPDF(printGenerateBM, printPathsVM[0], formatsDetailVMs);
                }
                else if (new int[] { (int)PrintEnum.Condicionado.CONSTANCIA_ASEGURADO, (int)PrintEnum.Condicionado.CONSTANCIA_ASEGURADO_AP,
                                     (int)PrintEnum.Condicionado.CONSTANCIA_ASEGURADO_VG}.Contains((int)printGenerateBM.NCOD_CONDICIONADO))
                {
                    response = new GenerateDocumentsDA().InsuredProofPDF(printGenerateBM, printPathsVM[0]);
                }
                else if (new int[] { (int)PrintEnum.Condicionado.CONSTANCIA_PROVISIONAL_VL, (int)PrintEnum.Condicionado.CONSTANCIA_PROVISIONAL_SCTR }.Contains((int)printGenerateBM.NCOD_CONDICIONADO))
                {
                    var polizaEstado = new GenerateDocumentsDA().ReaPolicyState(printGenerateBM);
                    response = polizaEstado == 1 ? new GenerateDocumentsDA().provisionalRecordPDF(printGenerateBM, printPathsVM[0]) : new ResponsePrintVM() { NCODE = 0 };
                }
                else if (new int[] { (int)PrintEnum.Condicionado.CERTIFICADO, (int)PrintEnum.Condicionado.CERTIFICADO_BASICO,
                                     (int)PrintEnum.Condicionado.CERTIFICADO_COMPLETO, (int)PrintEnum.Condicionado.CERTIFICADO_ESPECIAL,
                                     (int)PrintEnum.Condicionado.COVID_CERTIFICADO, (int)PrintEnum.Condicionado.AP_CERTIFICADO,
                                     (int)PrintEnum.Condicionado.CERTIFICADO_SCTR, (int)PrintEnum.Condicionado.CERTIFICADO_AP,
                                     (int)PrintEnum.Condicionado.CERTIFICADO_VG, (int)PrintEnum.Condicionado.SOLICITUD_CERTIFICADO_AP,
                                     (int)PrintEnum.Condicionado.SOLICITUD_VG }.Contains((int)printGenerateBM.NCOD_CONDICIONADO))
                {
                    List<FormatsDetailVM> formatsDetailVMs = GetFormatDetail(
                                                              new PrintFormatsDetailBM
                                                              {
                                                                  NBRANCH = printGenerateBM.NBRANCH,
                                                                  NPRODUCT = printGenerateBM.NPRODUCT,
                                                                  NPOLICY = printGenerateBM.NPOLICY,
                                                                  NCOD_CONDICIONADO = printGenerateBM.NCOD_CONDICIONADO
                                                              }
                                                           );
                    response = new GenerateDocumentsDA().CertificatePDF(printGenerateBM, printPathsVM[0], formatsDetailVMs);
                }
                else if (new int[] { (int)PrintEnum.Condicionado.ENDORSEMENT, (int)PrintEnum.Condicionado.CONSTANCIA_INCLUSION_AP,
                                     (int)PrintEnum.Condicionado.CONSTANCIA_INCLUSION_VIAJES_AP, (int)PrintEnum.Condicionado.CONSTANCIA_DECLARACION_AP,
                                     (int)PrintEnum.Condicionado.CONSTANCIA_MODIFICACION_AP, (int)PrintEnum.Condicionado.CONSTANCIA_EXCLUSION_AP,
                                     (int)PrintEnum.Condicionado.CONSTANCIA_ANULACION_AP, (int)PrintEnum.Condicionado.CONSTANCIA_EXCLUSION_VG,
                                     (int)PrintEnum.Condicionado.CONSTANCIA_INCLUSION_VG, (int)PrintEnum.Condicionado.CONSTANCIA_DECLARACION_VG,
                                     (int)PrintEnum.Condicionado.CONSTANCIA_ANULACION_VG, (int)PrintEnum.Condicionado.CONSTANCIA_MODIFICACION_VG }.Contains((int)printGenerateBM.NCOD_CONDICIONADO))
                {
                    response = new GenerateDocumentsDA().EndorsementPDF(printGenerateBM, printPathsVM[0]);
                }
                else if (new int[] { (int)PrintEnum.Condicionado.COVID_INDEMNIZACION }.Contains((int)printGenerateBM.NCOD_CONDICIONADO))
                {
                    response = new GenerateDocumentsDA().GeneratePDF(printGenerateBM, printPathsVM[0]);
                }
                else if (new int[] { (int)PrintEnum.Condicionado.EXCLUSION_VL }.Contains((int)printGenerateBM.NCOD_CONDICIONADO))
                {
                    response = new GenerateDocumentsDA().ProofExcludePDF(printGenerateBM, printPathsVM[0]);
                }
                else if (new int[] { (int)PrintEnum.Condicionado.COBERTURA_EXCEDENTE }.Contains((int)printGenerateBM.NCOD_CONDICIONADO))
                {
                    response = new GenerateDocumentsDA().CoverExcPDF(printGenerateBM, printPathsVM[0]);
                }
                else if (new int[] { (int)PrintEnum.Condicionado.ENDOSO }.Contains((int)printGenerateBM.NCOD_CONDICIONADO))
                {
                    response = new GenerateDocumentsDA().EndosoPDF(printGenerateBM, printPathsVM[0]);
                }
                else if (new int[] { (int)PrintEnum.Condicionado.ENDOSO_POLIZA }.Contains((int)printGenerateBM.NCOD_CONDICIONADO))
                {
                    response = new GenerateDocumentsDA().EndorsementPolicyPDF(printGenerateBM, printPathsVM[0]);
                }
                else if (new int[] { (int)PrintEnum.Condicionado.ENDOSO_MODIFIC_REM }.Contains((int)printGenerateBM.NCOD_CONDICIONADO))
                {
                    response = new GenerateDocumentsDA().EndorsementModifPDF(printGenerateBM, printPathsVM[0]);
                }
                //else if (new int[] { (int)PrintEnum.Condicionado.CONSTANCIA_ANULACION_AP }.Contains((int)printGenerateBM.NCOD_CONDICIONADO)) //ACCPER 
                //{
                //    response = new GenerateDocumentsDA().CancellationPDF(printGenerateBM, printPathsVM[0]);
                //}
                else if (new int[] { (int)PrintEnum.Condicionado.ANEXO_ASISTENCIA, (int)PrintEnum.Condicionado.ANEXO_CLAUSULA }.Contains((int)printGenerateBM.NCOD_CONDICIONADO))
                {
                    response = new GenerateDocumentsDA().AnexosPDF(printGenerateBM, printPathsVM[0]);
                }

                //ELog.save(this, "ESTADO FINALIZADO" + response.NCODE);
                if (response.NCODE == 0)
                {
                    /*PrintPolicyService printPolicyService = new PrintPolicyService();

                    List<FormatOrderVM> formatOrderVM = GetFormatLastOrder(printGenerateBM.NNTRANSAC);
                    if (formatOrderVM[0].NORDER == printGenerateBM.NORDER)
                    {
                        printPolicyService.joinDocuments(response.PATH_PDF, formatOrderVM[0].SNAME_ATTACHED_DOC);

                        if (response.NCODE == 0)
                        {
                            BaseDataQuotationVM baseDataQuotation = GetCotizacionByProc((int) printGenerateBM.NIDHEADERPROC);
                            ResponsePDFVM responsePE = sendDocumentsService(printGenerateBM, baseDataQuotation, GetUsersPolicyList(baseDataQuotation), formatOrderVM[0].SNAME_ATTACHED_DOC);

                            if (responsePE != null)
                            {
                                if (responsePE.Estatus == "ERROR")
                                {
                                    printGenerateBM.NSTATE = Convert.ToInt32(PrintEnum.State.ERROR);
                                    printGenerateBM.SMESSAGE_IMP = responsePE.Error;
                                    UpdateDocumentProcessState(printGenerateBM);
                                }
                                else
                                {
                                    printGenerateBM.NSTATE = Convert.ToInt32(PrintEnum.State.CORRECTO);
                                    printGenerateBM.NOPC = 2;
                                    response = UpdateDocumentProcessState(printGenerateBM);
                                }
                            }
                        }
                    }*/

                    //ELog.save(this, "INCIA POLIZA ELECTRONICA");
                    List<FormatOrderVM> formatOrderVM = GetFormatLastOrder(printGenerateBM.NNTRANSAC, printGenerateBM.NPRODUCT, printGenerateBM.NBRANCH);
                    BaseDataQuotationVM baseDataQuotation = new PrintPolicyDA().GetCotizacionByProc((int)printGenerateBM.NIDHEADERPROC);
                    /*ResponsePDFVM responsePE = new PrintPolicyDA().sendDocumentsService(printGenerateBM, baseDataQuotation, new PrintPolicyDA().GetUsersPolicyList(baseDataQuotation), formatOrderVM[0].SNAME_ATTACHED_DOC);
                    //ELog.save(this, "ESTADO FINALIZADO" + responsePE);

                    if (responsePE != null)
                    {
                        //ELog.save(this, "ESTADO POLIZA ELECTRONICA" + responsePE.Estatus);
                        if (responsePE.Estatus == "ERROR")
                        {
                            printGenerateBM.NOPC = 2;
                            printGenerateBM.NSTATE = Convert.ToInt32(PrintEnum.State.ERROR);
                            printGenerateBM.SMESSAGE_IMP = "WS Poliza electronica: " + responsePE.Error;
                            //ELog.save(this, "LOG WS PL" + printGenerateBM.SMESSAGE_IMP);

                            //ELog.save(this, "LOG WS PL" + printGenerateBM.SMESSAGE_IMP);
                            //ELog.save(this, "PARAMETROS" + printGenerateBM.NIDHEADERPROC);
                            //ELog.save(this, "PARAMETROS" + printGenerateBM.NIDDETAILPROC);
                            //ELog.save(this, "PARAMETROS" + printGenerateBM.NBRANCH);
                            //ELog.save(this, "PARAMETROS" + printGenerateBM.NPRODUCT);
                            //ELog.save(this, "PARAMETROS" + printGenerateBM.NPOLICY);
                            //ELog.save(this, "PARAMETROS" + printGenerateBM.NSTATE);
                            //ELog.save(this, "PARAMETROS" + printGenerateBM.NOPC);
                            //ELog.save(this, "PARAMETROS" + printGenerateBM.SMESSAGE_IMP);
                            //ELog.save(this, "PARAMETROS P_NMOVEMENT" + printGenerateBM.NMOVEMENT);
                            
                            response = new PrintPolicyDA().UpdateDocumentProcessState(printGenerateBM);
                            //ELog.save(this, "LOG WS PL" + response.NCODE);
                        }
                        else
                        {
                            printGenerateBM.NOPC = 2;
                            printGenerateBM.NSTATE = Convert.ToInt32(PrintEnum.State.CORRECTO);
                            response = new PrintPolicyDA().UpdateDocumentProcessState(printGenerateBM);
                        }
                    }
                    else
                    {
                        //ELog.save(this, "responsePE nulo");
                    }
                    */

                    printGenerateBM.NOPC = 2;
                    printGenerateBM.NSTATE = Convert.ToInt32(PrintEnum.State.CORRECTO);
                    UpdateDocumentProcessState(printGenerateBM);
                }

                if (response.NCODE == 1)
                {
                    printGenerateBM.NSTATE = Convert.ToInt32(PrintEnum.State.ERROR);
                    printGenerateBM.SMESSAGE_IMP = response.SMESSAGE;
                    UpdateDocumentProcessState(printGenerateBM);
                }

            }
            catch (Exception ex)
            {
                printGenerateBM.NOPC = 2;
                printGenerateBM.NSTATE = Convert.ToInt32(PrintEnum.State.ERROR);
                printGenerateBM.SMESSAGE_IMP = " - CONDICIONADO: " + printGenerateBM.SCONDICIONADO + " - HEADERPROC: " + printGenerateBM.NIDHEADERPROC + " - POLICY: " + printGenerateBM.NPOLICY + " - ERROR" + ex.Message;
                response.SMESSAGE = " - CONDICIONADO: " + printGenerateBM.SCONDICIONADO + " - HEADERPROC: " + printGenerateBM.NIDHEADERPROC + " - POLICY: " + printGenerateBM.NPOLICY + " - ERROR" + ex.Message;
                UpdateDocumentProcessState(printGenerateBM);
                response.NCODE = 1;
                ELog.saveDocumentos(this, ex.ToString());
            }

            return response;
        }

        public ResponsePDFVM sendDocumentsService(PrintGenerateBM printGenerateBM, BaseDataQuotationVM baseDataQuotationVM, List<UserInfoVM> userInfoList, string nameAttachDoc)
        {
            ResponsePDFVM response = new ResponsePDFVM();
            try
            {
                WsGenerarPDFReference.WsGenerarPDFexternoClient client = new WsGenerarPDFReference.WsGenerarPDFexternoClient();

                List<ReportDocPDF> listDoc = new List<ReportDocPDF>();
                List<ReportUserPDF> listUser = new List<ReportUserPDF>();
                if (printGenerateBM != null)
                {
                    string nomPlan = GetTemplateName(printGenerateBM);
                    try { InsertLog(printGenerateBM.NPOLICY, "Consulta nombre Template (Poliza electronica)", "PrintPolicyDa/sendDocumentsService", JsonConvert.SerializeObject(printGenerateBM), nomPlan); } catch { }
                    nomPlan = string.IsNullOrEmpty(nomPlan) ? ELog.obtainConfig("nombrePlanillaPL" + printGenerateBM.NBRANCH + printGenerateBM.NPRODUCT) : nomPlan;
                    ReportDocPDF report = new ReportDocPDF();
                    report.Nombre = nameAttachDoc.Substring(0, nameAttachDoc.IndexOf("."));
                    //report.Nombre = "7_Constancia_asegurados";
                    report.Poliza = printGenerateBM.NPOLICY.ToString();
                    report.SumaAsegurada = "0";
                    //report.SumaAsegurada = baseDataQuotationVM.NAMOUNT_PLANILLA.ToString();
                    report.Certificado = "1";
                    report.NombrePlantilla = nomPlan;
                    listDoc.Add(report);

                    foreach (UserInfoVM userItem in userInfoList)
                    {
                        if (!string.IsNullOrEmpty(userItem.sdocumento.Trim()) && !string.IsNullOrEmpty(userItem.semail.Trim()))
                        {
                            ReportUserPDF user = new ReportUserPDF();
                            user.Nombre = userItem.slegalName == "" ? userItem.snombre : userItem.slegalName;
                            user.Apellido = (userItem.sapePat + " " + userItem.sapeMat).Trim();
                            user.Correo = userItem.semail == "" ? "" : userItem.semail;
                            user.DNI = userItem.sdocumento;
                            user.Documentos = new List<Documento>();
                            var item = new Documento();
                            //ELog.save(this, "Info  Lista Usuario" + userItem.semail);
                            //ELog.save(this, "nombre adjunto" + nameAttachDoc.Substring(0, nameAttachDoc.IndexOf(".")));
                            item.nombre = nameAttachDoc.Substring(0, nameAttachDoc.IndexOf("."));
                            //item.nombre = "7_Constancia_asegurados";
                            user.Documentos.Add(item);
                            listUser.Add(user);
                        }
                    }
                }

                string informacionDocumentos = JsonConvert.SerializeObject(listDoc);
                string informacionUsuarios = JsonConvert.SerializeObject(listUser);
                string rutaDirectorio = "";
                if (printGenerateBM.NTRANSAC != "E")
                {
                    rutaDirectorio = ELog.obtainConfig("pathImpresion" + printGenerateBM.NBRANCH) + "" + printGenerateBM.NPOLICY + "\\" + printGenerateBM.NIDHEADERPROC + "\\" + printGenerateBM.STRANSAC + "\\" + printGenerateBM.DSTARTDATE + "\\";
                }
                else
                {
                    rutaDirectorio = ELog.obtainConfig("pathImpresion" + printGenerateBM.NBRANCH) + "" + printGenerateBM.NPOLICY + "\\" + printGenerateBM.NIDHEADERPROC + "\\" + printGenerateBM.STRANSAC + "\\";
                }
                //string rutaDirectorio = @"\\PRTSRV33\Documentos WS externo";
                LogControl.save("sendDocumentsService", "rutaDirectorio: " + rutaDirectorio, "2");
                LogControl.save("sendDocumentsService", "informacionUsuarios: " + informacionUsuarios, "2");
                LogControl.save("sendDocumentsService", "informacionDocumentos: " + informacionDocumentos, "2");

                var res = client.CargaDocumentoExterno(informacionDocumentos, informacionUsuarios, rutaDirectorio);
                try { InsertLog(printGenerateBM.NPOLICY, "Envio de documentos (Poliza electronica)", "PrintPolicyDa/CargaDocumentoExterno/WsGenerarPDFexternoClient/CargaDocumentoExterno", "rutaDirectorio: " + rutaDirectorio + Environment.NewLine + "informacionUsuarios: " + informacionUsuarios + Environment.NewLine + "informacionDocumentos: " + informacionDocumentos, JsonConvert.SerializeObject(res)); } catch { }
                response = JsonConvert.DeserializeObject<ResponsePDFVM>(res, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                if (response.Estatus == "ERROR") response.Error = response.Error + " | Ruta: " + rutaDirectorio;
                if (response.Estatus == "OK") response.Estatus = response.Estatus + " | Ruta: " + rutaDirectorio;
                //ELog.save(this, listUser[0].Documentos[0].nombre);

                LogControl.save("sendDocumentsService", "InformacionDocumentos: " + informacionDocumentos, "2");
                LogControl.save("sendDocumentsService", "informacionUsuarios: " + informacionUsuarios, "2");
                LogControl.save("sendDocumentsService", "rutaDirectorio: " + rutaDirectorio, "2");
                LogControl.save("sendDocumentsService", "error: " + response.Error, "2");
                LogControl.save("sendDocumentsService", "poliza_electronica: " + response.Estatus, "2");
            }
            catch (Exception ex)
            {
                LogControl.save("sendDocumentsService", ex.ToString(), "3");
                //ELog.save(this, ex.Message.ToString());
                //throw ex;
                try { InsertLog(printGenerateBM.NPOLICY, "Ex. (Poliza electronica)", "PrintPolicyDa/CargaDocumentoExterno", "", ex.ToString()); } catch { }
            }

            return response;
        }

        public BaseDataQuotationVM GetCotizacionByProc(int NIDHEADERPROC)
        {
            var sPackageJobs = ProcedureName.pkg_CargaMasivaPD + ".GET_COTIZACION_BY_HPROC";
            BaseDataQuotationVM response = new BaseDataQuotationVM();
            List<OracleParameter> parameters = new List<OracleParameter>();
            try
            {
                parameters.Add(new OracleParameter("P_NIDHEADERPROC", OracleDbType.Int32, NIDHEADERPROC, ParameterDirection.Input));

                OracleParameter P_NID_COT = new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, response.NID_COTIZACION, ParameterDirection.Output);
                OracleParameter P_MONTO_PLANILLA = new OracleParameter("P_MONTO_PLANILLA", OracleDbType.Double, response.NAMOUNT_PLANILLA, ParameterDirection.Output);

                P_NID_COT.Size = 200;
                P_MONTO_PLANILLA.Size = 200;

                parameters.Add(P_NID_COT);
                parameters.Add(P_MONTO_PLANILLA);
                this.ExecuteByStoredProcedureVT(sPackageJobs, parameters);
                response.NID_COTIZACION = int.Parse(P_NID_COT.Value.ToString());
                response.NAMOUNT_PLANILLA = double.Parse(P_MONTO_PLANILLA.Value.ToString());
            }
            catch (Exception ex)
            {
                LogControl.save("GetCotizacionByProc", ex.ToString(), "3");
                //throw ex;
            }

            return response;
        }

        public List<ResponsePrintJobsVM> GetJobsPrint()
        {
            var sPackageJobs = ProcedureName.pkg_CargaMasivaPD + ".REA_JOBPRINT";

            List<OracleParameter> parameter = new List<OracleParameter>();
            List<ResponsePrintJobsVM> response = new List<ResponsePrintJobsVM>();

            try
            {
                parameter.Add(new OracleParameter("C_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageJobs, "C_CURSOR", parameter))
                {
                    response = dr.ReadRowsList<ResponsePrintJobsVM>();
                    ELog.CloseConnection(dr);
                }
            }
            catch (Exception ex)
            {
                response = null;
                LogControl.save("GetJobsPrint", ex.ToString(), "3");
                //ELog.save(this, ex);
                throw ex;
            }

            return response;
        }

        public List<ResponsePrintFormatsVM> GetFormatsPrint(PrintFormatsBM printFormatsBM)
        {
            var sPackageJobs = ProcedureName.pkg_CargaMasivaPD + ".REA_FORMATOS";

            List<OracleParameter> parameter = new List<OracleParameter>();
            List<ResponsePrintFormatsVM> response = new List<ResponsePrintFormatsVM>();

            try
            {
                parameter.Add(new OracleParameter("P_NIDHEADERPROC", OracleDbType.Double, printFormatsBM.NIDHEADERPROC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDDETAILPROC", OracleDbType.Double, printFormatsBM.NIDDETAILPROC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDFILECONFIG", OracleDbType.Double, printFormatsBM.NIDFILECONFIG, ParameterDirection.Input));
                parameter.Add(new OracleParameter("C_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));
                /*OracleParameter C_CURSOR = new OracleParameter("C_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_CURSOR);*/

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageJobs, "C_CURSOR", parameter))
                {
                    response = dr.ReadRowsList<ResponsePrintFormatsVM>();
                    ELog.CloseConnection(dr);
                }
            }
            catch (Exception ex)
            {
                //ELog.save(this, ex);
                throw ex;
            }

            return response;
        }

        public List<PrintProceduresVM> GetProceduresPrint(PrintProceduresBM printProceduresBM)
        {
            var sPackageJobs = ProcedureName.pkg_CargaMasivaPD + ".REA_PROCEDURES";

            List<OracleParameter> parameter = new List<OracleParameter>();
            List<PrintProceduresVM> response = new List<PrintProceduresVM>();

            try
            {
                parameter.Add(new OracleParameter("P_NIDFILECONFIG", OracleDbType.Double, printProceduresBM.NIDFILECONFIG, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STRANSAC", OracleDbType.Varchar2, printProceduresBM.STRANSAC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCOD_CONDICIONADO", OracleDbType.Double, printProceduresBM.NCOD_CONDICIONADO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("C_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));
                /*OracleParameter C_CURSOR = new OracleParameter("C_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_CURSOR);*/

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageJobs, "C_CURSOR", parameter))
                {
                    response = dr.ReadRowsList<PrintProceduresVM>();
                    ELog.CloseConnection(dr);
                }
            }
            catch (Exception ex)
            {
                //ELog.save(this, ex);
                throw ex;
            }

            return response;
        }

        public List<PrintPathsVM> GetPathsPrint(dynamic NCOD_CONDICIONADO, int nidheaderproc = 0)
        {
            var sPackageJobs = ProcedureName.pkg_CargaMasivaPD + ".REA_PATHS";

            List<OracleParameter> parameter = new List<OracleParameter>();
            List<PrintPathsVM> response = new List<PrintPathsVM>();

            try
            {
                parameter.Add(new OracleParameter("P_NCOD_CONDICIONADO", OracleDbType.Double, NCOD_CONDICIONADO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("C_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));
                parameter.Add(new OracleParameter("P_NIDHEADERPROC", OracleDbType.Double, nidheaderproc, ParameterDirection.Input));
                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageJobs, "C_CURSOR", parameter))
                {
                    response = dr.ReadRowsList<PrintPathsVM>();
                    ELog.CloseConnection(dr);
                }
            }
            catch (Exception ex)
            {
                //ELog.save(this, ex);
                throw ex;
            }

            return response;
        }

        public List<FormatOrderVM> GetFormatLastOrder(dynamic NTRANSAC, dynamic NPRODUCT, dynamic NBRANCH, dynamic STRANSAC = null)
        {
            var sPackageJobs = ProcedureName.pkg_CargaMasivaPD + ".REA_FORMAT_LAST_ORDER";

            List<OracleParameter> parameter = new List<OracleParameter>();
            List<FormatOrderVM> response = new List<FormatOrderVM>();

            // Accidentes Personales - Config
            NPRODUCT = GetProductSend(NTRANSAC, NPRODUCT, NBRANCH);

            try
            {
                parameter.Add(new OracleParameter("P_NTRANSAC", OracleDbType.Double, NTRANSAC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Double, NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Double, NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STRANSAC", OracleDbType.Varchar2, STRANSAC == null ? string.Empty : STRANSAC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("C_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageJobs, "C_CURSOR", parameter))
                {
                    response = dr.ReadRowsList<FormatOrderVM>();
                    ELog.CloseConnection(dr);
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetFormatLastOrder", ex.ToString(), "3");
                throw ex;
            }

            return response;
        }

        private dynamic GetProductSend(dynamic NTRANSAC, dynamic NPRODUCT, dynamic NBRANCH)
        {
            dynamic product = NPRODUCT;
            if (NBRANCH == Convert.ToInt64(ELog.obtainConfig("accidentesBranch")))
            {
                //if (NTRANSAC == 11 && (NPRODUCT == 4 || NPRODUCT == 8)) // Solo viaje
                //{
                //    product = NPRODUCT;
                //}
                //else
                //{
                product = 0;
                //}
            }

            if (NBRANCH == Convert.ToInt64(ELog.obtainConfig("vidaGrupoBranch")) || NBRANCH == Convert.ToInt64(ELog.obtainConfig("vidaIndividualBranch")))
            {
                product = 0;
            }

            return product;
        }

        public List<FormatsDetailVM> GetFormatDetail(PrintFormatsDetailBM printFormatsDetailBM)
        {
            var sPackageJobs = ProcedureName.pkg_CargaMasivaPD + ".REA_FORMATOS_DETAIL";

            List<OracleParameter> parameter = new List<OracleParameter>();
            List<FormatsDetailVM> response = new List<FormatsDetailVM>();

            try
            {
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Double, printFormatsDetailBM.NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Double, printFormatsDetailBM.NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int64, printFormatsDetailBM.NPOLICY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCOD_CONDICIONADO", OracleDbType.Double, printFormatsDetailBM.NCOD_CONDICIONADO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("C_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageJobs, "C_CURSOR", parameter))
                {
                    response = dr.ReadRowsList<FormatsDetailVM>();
                    ELog.CloseConnection(dr);
                }
            }
            catch (Exception ex)
            {
                //ELog.save(this, ex);
                throw ex;
            }

            return response;
        }

        public List<UserInfoVM> GetUsersPolicyList(BaseDataQuotationVM baseDataQuotation)
        {
            var sPackageJobs = ProcedureName.pkg_CargaMasiva + ".REA_POLIZA_USERS";
            OracleDataReader dr = null;
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<UserInfoVM> response = new List<UserInfoVM>();

            try
            {
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, baseDataQuotation.NID_COTIZACION, ParameterDirection.Input));

                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageJobs, parameter);
                while (dr.Read())
                {
                    UserInfoVM item = new UserInfoVM();
                    item.snombre = dr["SNOMBRE"].ToString();
                    item.sapePat = dr["SAPELLIDOP"].ToString();
                    item.sapeMat = dr["SAPELLIDOM"].ToString();
                    item.slegalName = dr["SLEGALNAME"].ToString();
                    item.sdocumento = dr["SDOCUMENTO"].ToString();
                    item.semail = dr["SEMAIL"].ToString();

                    response.Add(item);
                }

                ELog.CloseConnection(dr);
            }
            catch (Exception ex)
            {
                //ELog.save(this, ex);
                throw ex;
            }

            return response;
        }

        public List<UserInfoVM> GetUsersPolicyList(PrintGenerateBM printGenerate)
        {
            var sPackageJobs = ProcedureName.pkg_CargaMasiva + ".REA_POLIZA_USERS";
            OracleDataReader dr = null;
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<UserInfoVM> response = new List<UserInfoVM>();

            try
            {
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, printGenerate.NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, printGenerate.NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int64, printGenerate.NPOLICY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMOVEMENT", OracleDbType.Int32, printGenerate.NMOVEMENT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageJobs, parameter);
                while (dr.Read())
                {
                    UserInfoVM item = new UserInfoVM();
                    item.snombre = dr["SNOMBRE"].ToString();
                    item.sapePat = dr["SAPELLIDOP"].ToString();
                    item.sapeMat = dr["SAPELLIDOM"].ToString();
                    item.slegalName = dr["SLEGALNAME"].ToString();
                    item.sdocumento = dr["SDOCUMENTO"].ToString();
                    item.semail = dr["SEMAIL"].ToString();

                    response.Add(item);
                }

                ELog.CloseConnection(dr);
            }
            catch (Exception ex)
            {
                LogControl.save("GetUsersPolicyList", ex.ToString(), "3");
                throw ex;
            }

            return response;
        }

        public ResponsePrintVM UpdateDocumentProcessState(PrintGenerateBM printGenerateBM)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponsePrintVM response = new ResponsePrintVM();
            string storedProcedureName = ProcedureName.pkg_CargaMasivaPD + ".INSUPD_ENV_PRINT";
            try
            {
                parameter.Add(new OracleParameter("P_NIDHEADERPROC", OracleDbType.Double, printGenerateBM.NIDHEADERPROC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDDETAILPROC", OracleDbType.Double, printGenerateBM.NIDDETAILPROC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Double, printGenerateBM.NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Double, printGenerateBM.NPRODUCT, ParameterDirection.Input));
                //parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int32, printGenerateBM.NPOLICY, ParameterDirection.Input));
                //CAMBIO POLICY
                parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int64, printGenerateBM.NPOLICY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NSTATE", OracleDbType.Int32, printGenerateBM.NSTATE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NOPC", OracleDbType.Int32, printGenerateBM.NOPC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SMESSAGE_IMP", OracleDbType.Varchar2, printGenerateBM.SMESSAGE_IMP, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMOVEMENT", OracleDbType.Double, printGenerateBM.NMOVEMENT, ParameterDirection.Input));

                /*OracleParameter C_CURSOR = new OracleParameter("C_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_CURSOR);*/

                this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);
            }
            catch (Exception ex)
            {
                LogControl.save("UpdateDocumentProcessState", ex.ToString(), "3");
                throw ex;
            }

            return response;
        }
        public string GetTemplateName(PrintGenerateBM printGenerateBM)
        {
            var sPackageJobs = ProcedureName.pkg_CargaMasivaPD + ".REA_ELEC_POL_TEMPLATE";
            BaseDataQuotationVM response = new BaseDataQuotationVM();
            List<OracleParameter> parameters = new List<OracleParameter>();
            try
            {
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, printGenerateBM.NBRANCH, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, printGenerateBM.NPRODUCT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NIDHEADERPROC", OracleDbType.Int32, printGenerateBM.NIDHEADERPROC, ParameterDirection.Input));

                OracleParameter P_STEMPLATE = new OracleParameter("P_STEMPLATE", OracleDbType.Varchar2, null, ParameterDirection.Output);

                P_STEMPLATE.Size = 200;

                parameters.Add(P_STEMPLATE);
                this.ExecuteByStoredProcedureVT(sPackageJobs, parameters);
                return P_STEMPLATE.Value.ToString() == "null" ? null : P_STEMPLATE.Value.ToString();
            }
            catch (Exception ex)
            {
                LogControl.save("GetTemplateName", ex.ToString(), "3");
                return "";
            }
        }
        public void InsertLog(long poliza, string proceso, string url, string parametros, string resultados)
        {
            QuotationDA quotationDA = new QuotationDA();
            quotationDA.InsertLog(poliza, proceso, url, parametros, resultados);
        }
        /******/

        #region Generacion de documentos ATP

        private bool GenerarDocumento(RequestATP request, DocumentoATP doc)
        {
            try
            {
                string rutaTmpImg = doc.rutaTmp.ToString();
                MasterDataATP masterData = new MasterDataATP();
                switch (doc.id)
                {
                    case 1:
                        masterData = this.CargarDataATP_01(request);
                        break;
                    case 2:
                        masterData = this.CargarDataATP_02(request);
                        break;
                    case 3:
                        masterData = this.CargarDataATP_03(request);
                        break;
                    case 4:
                        masterData = this.CargarDataATP_04(request, doc);
                        break;
                    case 5:
                        masterData = this.CargarDataATP_05(request);
                        break;
                    case 7:
                        masterData = this.CargarDataATP_07(request);
                        break;
                    case 8:
                        masterData = this.CargarDataATP_08(request);
                        break;
                    case 9:
                        masterData = this.CargarDataATP_09(request);
                        break;
                }
                doc.rutaTmp = doc.rutaTmp + request.policy + "_" + doc.id.ToString() + "_" + doc.nombre;
                doc.rutaDestino = doc.rutaDestino + request.policy;
                bool result = new PrintPolicyService().CreateDocument(masterData, doc, rutaTmpImg);
                return result;
            }
            catch (Exception ex)
            {
                //return false;
                LogControl.save("GenerarDocumento", ex.ToString(), "3");
                throw ex;
            }
        }

        public void InsertLogATP(long? policy, string sClient, string print_res, int code, int sent, string message)
        {
            string nameStore = ProcedureName.pkg_ATP + ".SP_ATP_INS_PRINT_LOG";
            List<OracleParameter> oracleParameterList = new List<OracleParameter>();
            try
            {
                oracleParameterList.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int64, policy.HasValue ? (object)policy.Value : DBNull.Value, ParameterDirection.Input));
                oracleParameterList.Add(new OracleParameter("P_SPRINT_RES", OracleDbType.Varchar2, (object)print_res, ParameterDirection.Input));
                oracleParameterList.Add(new OracleParameter("P_NCODE", OracleDbType.Int64, (object)code, ParameterDirection.Input));
                oracleParameterList.Add(new OracleParameter("P_SENT", OracleDbType.Int64, (object)sent, ParameterDirection.Input));
                oracleParameterList.Add(new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, (object)message, ParameterDirection.Input));
                oracleParameterList.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, string.IsNullOrEmpty(sClient) ? DBNull.Value : (object)sClient, ParameterDirection.Input));
                this.ExecuteByStoredProcedureVT(nameStore, oracleParameterList);
            }
            catch (Exception ex)
            {
                LogControl.save("InsertLogATP", ex.ToString(), "3");
            }
        }

        public void InsertLogTrxErrorVDP(ErrorTrxBM errorTrx)
        {
            string nameStore = ProcedureName.pkg_ErrorVDP + ".INSERT_TRX_ERROR";
            List<OracleParameter> oracleParameterList = new List<OracleParameter>();
            try
            {
                oracleParameterList.Add(new OracleParameter("NORIGIN", OracleDbType.Int32, errorTrx.nOrigin, ParameterDirection.Input));
                oracleParameterList.Add(new OracleParameter("SCERTYPE", OracleDbType.Varchar2, errorTrx.sCertype, ParameterDirection.Input));
                oracleParameterList.Add(new OracleParameter("NBRANCH", OracleDbType.Int32, errorTrx.nBranch, ParameterDirection.Input));
                oracleParameterList.Add(new OracleParameter("NPRODUCT", OracleDbType.Int32, errorTrx.nProduct, ParameterDirection.Input));
                oracleParameterList.Add(new OracleParameter("SCLIENT", OracleDbType.Varchar2, string.IsNullOrEmpty(errorTrx.sClient) ? DBNull.Value : (object)errorTrx.sClient, ParameterDirection.Input));
                oracleParameterList.Add(new OracleParameter("NPOLICY", OracleDbType.Int64, errorTrx.nPolicy, ParameterDirection.Input));
                oracleParameterList.Add(new OracleParameter("SIDPROCESS", OracleDbType.Varchar2, string.IsNullOrEmpty(errorTrx.sIdProcess) ? DBNull.Value : (object)errorTrx.sIdProcess, ParameterDirection.Input));
                oracleParameterList.Add(new OracleParameter("SMESSAGE", OracleDbType.Varchar2, (object)errorTrx.sMessage, ParameterDirection.Input));
                oracleParameterList.Add(new OracleParameter("NSENDMAIL", OracleDbType.Int32, errorTrx.nSendMail.HasValue ? (object)errorTrx.nSendMail.Value : DBNull.Value, ParameterDirection.Input));
                oracleParameterList.Add(new OracleParameter("NUSERCODE", OracleDbType.Int32, errorTrx.nUserCode, ParameterDirection.Input));
                this.ExecuteByStoredProcedureVT(nameStore, oracleParameterList);
            }
            catch (Exception ex)
            {
                LogControl.save("InsertLogTrxErrorVDP", ex.ToString(), "3");
            }
        }

        public ResponsePrintVM sendPEATP(string rutaDestino, string policy, RequestATP request)
        {
            ResponsePrintVM responsePrintVm = new ResponsePrintVM();
            try
            {
                List<InfoPE_ATP> infoList = CargarDataPolizaElectronica(request);

                List<ReportDocPDF> listDoc = new List<ReportDocPDF>()
                {
                    new ReportDocPDF()
                    {
                        Nombre = infoList[0].DOCUMENTOS.ToString(),
                        Poliza = request.policy,
                        Certificado = "0",
                        SumaAsegurada = "0",
                        NombrePlantilla = infoList[0].NOMBRE_PLANTILLA.ToString(),
                        Ramo = request.branch,
                        Producto = request.product,
                        Prima = infoList[0].PRIMA.ToString(),
                        DocContratante = infoList[0].DNI.ToString()
                    }
                };
                List<ReportUserPDF> listUser = new List<ReportUserPDF>();

                foreach (var item in infoList)
                {
                    ReportUserPDF user = new ReportUserPDF();
                    user.Nombre = item.NOMBRES.ToString().Trim();
                    user.Apellido = item.APELLIDOS.ToString().Trim();
                    user.Correo = item.CORREO.ToString().Trim();
                    user.DNI = item.DNI.ToString().Trim();
                    user.Documentos = new List<Documento>();
                    var doc = new Documento();
                    doc.nombre = infoList[0].DOCUMENTOS.ToString();
                    user.Documentos.Add(doc);
                    listUser.Add(user);
                }
                ResponsePDFVM responsePdfvm = new ResponsePDFVM();
                WsGenerarPDFReference.WsGenerarPDFexternoClient pdFexternoClient = new WsGenerarPDFReference.WsGenerarPDFexternoClient();
                string informacionDocumentos = JsonConvert.SerializeObject(listDoc);
                string informacionUsuarios = JsonConvert.SerializeObject(listUser);
                string rutaDirectorio = rutaDestino;
                ELogSendWS.save("Input", string.Format("{0} || {1} || {2}", informacionDocumentos, informacionUsuarios, rutaDirectorio));
                responsePdfvm = JsonConvert.DeserializeObject<ResponsePDFVM>(pdFexternoClient.CargaDocumentoExterno(informacionDocumentos, informacionUsuarios, rutaDirectorio), new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
                if (responsePdfvm.Estatus == "ERROR")
                {
                    responsePrintVm.SMESSAGE = responsePdfvm.Error;
                    responsePrintVm.NCODE = 2;
                    ErrorTrxBM errorTrx = new ErrorTrxBM();
                    errorTrx.nOrigin = 1;
                    errorTrx.sCertype = "2";
                    errorTrx.nBranch = Convert.ToInt32(request.branch);
                    errorTrx.nProduct = Convert.ToInt32(request.product);
                    errorTrx.nPolicy = Convert.ToInt64(request.policy);
                    errorTrx.nSendMail = 2;
                    errorTrx.sMessage = responsePdfvm.Error;
                    errorTrx.nUserCode = 99999;
                    InsertLogTrxErrorVDP(errorTrx);
                }
                ELogSendWS.save("Output", string.Format("{0} | {1}", request.policy, JsonConvert.SerializeObject(responsePdfvm)));
            }
            catch (Exception ex)
            {
                responsePrintVm.SMESSAGE = "send - " + ex.Message;
                responsePrintVm.NCODE = 1;
                ELogSendWS.save("Error", string.Format("{0} | {1}", request.policy, ex.Message));
                ErrorTrxBM errorTrx = new ErrorTrxBM(ex);
                errorTrx.nOrigin = 1;
                errorTrx.sCertype = "2";
                errorTrx.nBranch = Convert.ToInt32(request.branch);
                errorTrx.nProduct = Convert.ToInt32(request.product);
                errorTrx.nPolicy = Convert.ToInt64(request.policy);
                InsertLogTrxErrorVDP(errorTrx);
            }
            return responsePrintVm;
        }
        public ResponsePrintVM sendPEATPasegurado(string rutaDestino, string policy, RequestATP request)
        {
            ResponsePrintVM responsePrintVm = new ResponsePrintVM();
            try
            {
                List<InfoPE_ATP> infoList = CargarDataPolizaElectronicaAsegurado(request);

                List<ReportDocPDF> listDoc = new List<ReportDocPDF>()
                {
                    new ReportDocPDF()
                    {
                        Nombre = infoList[0].DOCUMENTOS.ToString(),
                        Poliza = request.policy,
                        Certificado = "0",
                        SumaAsegurada = "0",
                        NombrePlantilla = infoList[0].NOMBRE_PLANTILLA.ToString(),
                        Ramo = request.branch,
                        Producto = request.product,
                        Prima = infoList[0].PRIMA.ToString(),
                        DocContratante = infoList[0].DNI.ToString()
                    }
                };
                List<ReportUserPDF> listUser = new List<ReportUserPDF>();

                foreach (var item in infoList)
                {
                    ReportUserPDF user = new ReportUserPDF();
                    user.Nombre = item.NOMBRES.ToString().Trim();
                    user.Apellido = item.APELLIDOS.ToString().Trim();
                    user.Correo = item.CORREO.ToString().Trim();
                    user.DNI = item.DNI.ToString().Trim();
                    user.Documentos = new List<Documento>();
                    var doc = new Documento();
                    doc.nombre = infoList[0].DOCUMENTOS.ToString();
                    user.Documentos.Add(doc);
                    listUser.Add(user);
                }

                ResponsePDFVM responsePdfvm = new ResponsePDFVM();
                WsGenerarPDFReference.WsGenerarPDFexternoClient pdFexternoClient = new WsGenerarPDFReference.WsGenerarPDFexternoClient();
                string informacionDocumentos = JsonConvert.SerializeObject(listDoc);
                string informacionUsuarios = JsonConvert.SerializeObject(listUser);
                string rutaDirectorio = rutaDestino;
                ELogSendWS.save("Input", string.Format("{0} || {1} || {2}", informacionDocumentos, informacionUsuarios, rutaDirectorio));
                responsePdfvm = JsonConvert.DeserializeObject<ResponsePDFVM>(pdFexternoClient.CargaDocumentoExterno(informacionDocumentos, informacionUsuarios, rutaDirectorio), new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
                if (responsePdfvm.Estatus == "ERROR")
                {
                    responsePrintVm.SMESSAGE = responsePdfvm.Error;
                    responsePrintVm.NCODE = 2;
                    ErrorTrxBM errorTrx = new ErrorTrxBM();
                    errorTrx.nOrigin = 1;
                    errorTrx.sCertype = "2";
                    errorTrx.nBranch = Convert.ToInt32(request.branch);
                    errorTrx.nProduct = Convert.ToInt32(request.product);
                    errorTrx.nPolicy = Convert.ToInt64(request.policy);
                    errorTrx.nSendMail = 2;
                    errorTrx.sMessage = responsePdfvm.Error;
                    errorTrx.nUserCode = 99999;
                    InsertLogTrxErrorVDP(errorTrx);
                }
                ELogSendWS.save("Output", string.Format("{0} | {1}", request.policy, JsonConvert.SerializeObject(responsePdfvm)));
            }
            catch (Exception ex)
            {
                responsePrintVm.SMESSAGE = "send - " + ex.Message;
                responsePrintVm.NCODE = 1;
                ELogSendWS.save("Error", string.Format("{0} | {1}", request.policy, ex.Message));
                ErrorTrxBM errorTrx = new ErrorTrxBM(ex);
                errorTrx.nOrigin = 1;
                errorTrx.sCertype = "2";
                errorTrx.nBranch = Convert.ToInt32(request.branch);
                errorTrx.nProduct = Convert.ToInt32(request.product);
                errorTrx.nPolicy = Convert.ToInt64(request.policy);
                InsertLogTrxErrorVDP(errorTrx);
            }
            return responsePrintVm;
        }

        public ResponsePrintVM PrintATP(RequestATP request)
        {
            ResponsePrintVM response = new ResponsePrintVM();
            PrintPolicyService printPolicyService = new PrintPolicyService();
            int num = 0;
            int sent = 0;
            int valCorreo = 2;
            try
            {
                List<DocumentoATP> listaDoc = this.CargarDataGeneral(request).listaDoc;
                List<FlagAsegurado> listAseg = this.FlagAsegurado(request).listaFlag;

                if (!Directory.Exists(listaDoc[0].rutaTmp))
                    Directory.CreateDirectory(listaDoc[0].rutaTmp);

                if (!Directory.Exists(listaDoc[0].rutaDestino))
                    Directory.CreateDirectory(listaDoc[0].rutaDestino);


                //Eliminar archivos cuadro poliza

                if (Directory.Exists(listaDoc[0].rutaDestino))
                {
                    DirectoryInfo di = Directory.CreateDirectory(listaDoc[0].rutaDestino + "\\" + request.policy);
                    FileInfo[] files = di.GetFiles();
                    foreach (FileInfo file in files)
                    {
                        file.Delete();
                    }
                }

                string print_res = "";

                foreach (DocumentoATP doc in listaDoc)
                {
                    bool flag = this.GenerarDocumento(request, doc);
                    print_res += flag ? "1" : "0";
                }
                response = printPolicyService.JoinDocumentATP(listaDoc[0].rutaDestino + "\\", listaDoc[0].nombreCD);

                if (response.NCODE == 0)
                {
                    num = 1;
                    if (request.send == "S")
                    {
                        response = sendPEATP(listaDoc[0].rutaDestino + "\\", listaDoc[0].nombreCD, request);
                        // VALIDACIÓN PARA ENVIAR CORREO SI ASEGURADO SON DISTINTOS --> 1 = IGUAL || 2 = DIFERENTES

                        if (listAseg[0].FLAG_ASE == valCorreo)
                        {
                            response = sendPEATPasegurado(listaDoc[0].rutaDestino + "\\", listaDoc[0].nombreCD, request);
                        }
                        sent = response.NCODE == 0 ? 1 : 0;
                    }
                }
                response.SRESULTADO_PRINT = print_res;
                response.NGEN_CD = num;
                response.RESULT_SEND = sent;
                response.PATH_GEN = listaDoc[0].rutaDestino;
                try
                {
                    this.InsertLogATP(Convert.ToInt64(request.policy), string.Empty, print_res, response.NCODE, sent, JsonConvert.SerializeObject(response));
                }
                catch (Exception ex)
                {
                    LogControl.save("PrintATP", ex.ToString(), "3");
                }
            }
            catch (Exception ex)
            {
                LogControl.save("PrintATP", ex.ToString(), "3");
                this.InsertLogATP(Convert.ToInt64(request.policy), string.Empty, string.Empty, 2, 0, ex.Message);
                response.NCODE = ex.HResult;
                response.SMESSAGE = ex.Message;
                response.SRESULTADO_PRINT = string.Empty;
                return response;
            }
            return response;
        }

        private GeneralDataATP CargarDataGeneral(RequestATP request)
        {
            string nameStore = ProcedureName.pkg_ATP + ".SP_ATP_REA_DATA_GENERAL";
            OracleDataReader dataConnection = (OracleDataReader)null;
            List<OracleParameter> oracleParameterList = new List<OracleParameter>();
            GeneralDataATP generalDataAtp = new GeneralDataATP();
            List<DocumentoATP> documentoAtpList = new List<DocumentoATP>();
            try
            {
                oracleParameterList.Add(new OracleParameter("P_NPOLICY", OracleDbType.Varchar2, (object)request.policy, ParameterDirection.Input));
                string[] nameCursor = new string[1]
                {
                  "C_DOCUMENTOS"
                };
                oracleParameterList.Add(new OracleParameter(nameCursor[0], OracleDbType.RefCursor, ParameterDirection.Output));
                dataConnection = (OracleDataReader)this.ExecuteByStoredProcedureVT(nameStore, nameCursor, oracleParameterList);

                while (dataConnection.Read())
                    documentoAtpList.Add(new DocumentoATP()
                    {
                        id = Convert.ToInt32(dataConnection["id"].ToString()),
                        estado = Convert.ToInt32(dataConnection["estado"].ToString()),
                        nOrden = Convert.ToInt32(dataConnection["nOrden"].ToString()),
                        nombre = dataConnection["nombre"].ToString(),
                        nombreFinal = dataConnection["nombreFinal"].ToString(),
                        rutaTemplate = dataConnection["rutaTemplate"].ToString(),
                        rutaTmp = dataConnection["rutaTmp"].ToString(),
                        rutaDestino = dataConnection["rutaDestino"].ToString(),
                        copia = Convert.ToInt32(dataConnection["copia"].ToString()),
                        nombreCD = dataConnection["nombreCD"].ToString()
                    });

                generalDataAtp.listaDoc = documentoAtpList;
                ELog.CloseConnection(dataConnection);
            }
            catch (Exception ex)
            {
                ELog.CloseConnection(dataConnection);
                generalDataAtp = (GeneralDataATP)null;
            }
            return generalDataAtp;
        }
        private GeneralDataATP FlagAsegurado(RequestATP request)
        {
            string nameStore = ProcedureName.pkg_ATP + ".SP_ATP_REA_DATA_FLAG";
            OracleDataReader dataConnection = (OracleDataReader)null;
            List<OracleParameter> oracleParameterList = new List<OracleParameter>();
            GeneralDataATP generalDataAtp = new GeneralDataATP();
            List<FlagAsegurado> documentoAtpListAse = new List<FlagAsegurado>();
            try
            {
                oracleParameterList.Add(new OracleParameter("P_NPOLICY", OracleDbType.Varchar2, (object)request.policy, ParameterDirection.Input));
                string[] nameCursor = new string[1]
                {
                  "C_FLAGASEGURADO"
                };
                oracleParameterList.Add(new OracleParameter(nameCursor[0], OracleDbType.RefCursor, ParameterDirection.Output));
                dataConnection = (OracleDataReader)this.ExecuteByStoredProcedureVT(nameStore, nameCursor, oracleParameterList);

                while (dataConnection.Read())
                    documentoAtpListAse.Add(new FlagAsegurado()
                    {
                        FLAG_ASE = Convert.ToInt32(dataConnection["FLAG_ASE"].ToString())
                    });

                generalDataAtp.listaFlag = documentoAtpListAse;



                ELog.CloseConnection(dataConnection);
            }
            catch (Exception ex)
            {
                ELog.CloseConnection(dataConnection);
                generalDataAtp = (GeneralDataATP)null;
            }
            return generalDataAtp;

        }
        private MasterDataATP CargarDataATP_01(RequestATP request)
        {
            string nameStore = ProcedureName.pkg_ATP + ".SP_ATP_REA_DATA_COND_GEN";
            OracleDataReader oracleDataReader = (OracleDataReader)null;
            List<OracleParameter> oracleParameterList = new List<OracleParameter>();
            MasterDataATP masterDataAtp = new MasterDataATP();
            List<ComercializadorATP> comercializadorAtpList = new List<ComercializadorATP>();
            try
            {
                oracleParameterList.Add(new OracleParameter("P_NPOLICY", OracleDbType.Varchar2, (object)request.policy, ParameterDirection.Input));
                oracleParameterList.Add(new OracleParameter("P_NBRANCH", OracleDbType.Varchar2, (object)request.branch, ParameterDirection.Input));
                oracleParameterList.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Varchar2, (object)request.product, ParameterDirection.Input));
                string[] nameCursor = new string[1] { "C_COMER" };
                oracleParameterList.Add(new OracleParameter(nameCursor[0], OracleDbType.RefCursor, ParameterDirection.Output));

                oracleDataReader = (OracleDataReader)this.ExecuteByStoredProcedureVT(nameStore, nameCursor, oracleParameterList);

                foreach (ComercializadorATP readRows in oracleDataReader.ReadRowsList<ComercializadorATP>())
                    comercializadorAtpList.Add(readRows);

                masterDataAtp.COMER = comercializadorAtpList;

                ELog.CloseConnection(oracleDataReader);
            }
            catch (Exception ex)
            {
                ELog.CloseConnection(oracleDataReader);
                LogControl.save("CargarDataATP_01", ex.ToString(), "3");
                masterDataAtp = (MasterDataATP)null;
                throw ex;
            }
            return masterDataAtp;
        }

        private MasterDataATP CargarDataATP_02(RequestATP request)
        {
            string nameStore = ProcedureName.pkg_ATP + ".SP_ATP_REA_DATA_COND_PAR";
            OracleDataReader oracleDataReader = (OracleDataReader)null;
            List<OracleParameter> oracleParameterList = new List<OracleParameter>();
            MasterDataATP masterDataAtp = new MasterDataATP();
            List<ContratanteATP> contratanteAtpList = new List<ContratanteATP>();
            List<AseguradoATP> aseguradoAtpList = new List<AseguradoATP>();
            List<BeneficiarioATP> beneficiarioAtpList = new List<BeneficiarioATP>();
            List<BeneficiarioCepATP> beneficiarioCepAtpList = new List<BeneficiarioCepATP>();
            List<CoverATP> coverAtpList = new List<CoverATP>();
            List<CoverMainATP> coverMainAtpList = new List<CoverMainATP>();
            List<RescateATP> rescateAtpList = new List<RescateATP>();
            List<ReportPagosAseguradoATP> PagosAseuradoList = new List<ReportPagosAseguradoATP>();
            try
            {
                oracleParameterList.Add(new OracleParameter("P_NPOLICY", OracleDbType.Varchar2, (object)request.policy, ParameterDirection.Input));
                oracleParameterList.Add(new OracleParameter("P_NBRANCH", OracleDbType.Varchar2, (object)request.branch, ParameterDirection.Input));
                oracleParameterList.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Varchar2, (object)request.product, ParameterDirection.Input));
                string[] nameCursor = new string[11]
                {
                  "C_GENERAL",
                  "C_CONT",
                  "C_ASEG",
                  "C_BENE",
                  "C_BENECEP",
                  "C_COVER",
                  "C_COVERMAIN",
                  "C_RESC",
                  "C_CURSOR1",
                  "C_FIRMA_CT",
                  "C_FIRMA_AS",

                };
                oracleParameterList.Add(new OracleParameter(nameCursor[0], OracleDbType.RefCursor, ParameterDirection.Output));
                oracleParameterList.Add(new OracleParameter(nameCursor[1], OracleDbType.RefCursor, ParameterDirection.Output));
                oracleParameterList.Add(new OracleParameter(nameCursor[2], OracleDbType.RefCursor, ParameterDirection.Output));
                oracleParameterList.Add(new OracleParameter(nameCursor[3], OracleDbType.RefCursor, ParameterDirection.Output));
                oracleParameterList.Add(new OracleParameter(nameCursor[4], OracleDbType.RefCursor, ParameterDirection.Output));
                oracleParameterList.Add(new OracleParameter(nameCursor[5], OracleDbType.RefCursor, ParameterDirection.Output));
                oracleParameterList.Add(new OracleParameter(nameCursor[6], OracleDbType.RefCursor, ParameterDirection.Output));
                oracleParameterList.Add(new OracleParameter(nameCursor[7], OracleDbType.RefCursor, ParameterDirection.Output));
                oracleParameterList.Add(new OracleParameter(nameCursor[8], OracleDbType.RefCursor, ParameterDirection.Output));
                oracleParameterList.Add(new OracleParameter(nameCursor[9], OracleDbType.RefCursor, ParameterDirection.Output));
                oracleParameterList.Add(new OracleParameter(nameCursor[10], OracleDbType.RefCursor, ParameterDirection.Output));
                oracleDataReader = (OracleDataReader)this.ExecuteByStoredProcedureVTCursores(nameStore, nameCursor, oracleParameterList);

                masterDataAtp = oracleDataReader.ReadRowsList<MasterDataATP>()[0];

                oracleDataReader.NextResult();
                foreach (ContratanteATP readRows in oracleDataReader.ReadRowsList<ContratanteATP>())
                    contratanteAtpList.Add(readRows);

                oracleDataReader.NextResult();
                foreach (AseguradoATP readRows in oracleDataReader.ReadRowsList<AseguradoATP>())
                    aseguradoAtpList.Add(readRows);

                oracleDataReader.NextResult();
                foreach (BeneficiarioATP readRows in oracleDataReader.ReadRowsList<BeneficiarioATP>())
                    beneficiarioAtpList.Add(readRows);

                oracleDataReader.NextResult();
                foreach (BeneficiarioCepATP readRows in oracleDataReader.ReadRowsList<BeneficiarioCepATP>())
                    beneficiarioCepAtpList.Add(readRows);

                oracleDataReader.NextResult();
                foreach (CoverATP readRows in oracleDataReader.ReadRowsList<CoverATP>())
                    coverAtpList.Add(readRows);

                oracleDataReader.NextResult();
                foreach (CoverMainATP readRows in oracleDataReader.ReadRowsList<CoverMainATP>())
                    coverMainAtpList.Add(readRows);

                oracleDataReader.NextResult();
                foreach (RescateATP readRows in oracleDataReader.ReadRowsList<RescateATP>())
                    rescateAtpList.Add(readRows);
                //EMPEZAR
                oracleDataReader.NextResult();
                foreach (ReportPagosAseguradoATP readRows in oracleDataReader.ReadRowsList<ReportPagosAseguradoATP>())
                    PagosAseuradoList.Add(readRows);

                oracleDataReader.NextResult();
                masterDataAtp.FIRMA = oracleDataReader.ReadRowsList<FirmaATP>()[0]; // SE AGREGOO FIRMA ASEGURADO
                oracleDataReader.NextResult();
                masterDataAtp.FIRMACO = oracleDataReader.ReadRowsList<FirmaAseATP>()[0]; // SE AGREGOO FIRMA CONTRATANTE
                masterDataAtp.CONT = contratanteAtpList;
                masterDataAtp.ASEG = aseguradoAtpList;
                masterDataAtp.BENE = beneficiarioAtpList;
                masterDataAtp.BENECEP = beneficiarioCepAtpList;
                masterDataAtp.COVER = coverAtpList;
                masterDataAtp.COVERMAIN = coverMainAtpList;
                masterDataAtp.RESC = rescateAtpList;
                masterDataAtp.PG = PagosAseuradoList;
                try
                {
                    masterDataAtp.RESC_IMP_EXP = rescateAtpList[0].IMP_EXP;
                }
                catch (Exception)
                {

                }
                ELog.CloseConnection(oracleDataReader);
            }
            catch (Exception ex)
            {
                ELog.CloseConnection(oracleDataReader);
                LogControl.save("CargarDataATP_02", ex.ToString(), "3");
                masterDataAtp = (MasterDataATP)null;
                throw ex;
            }
            return masterDataAtp;
        }

        private MasterDataATP CargarDataATP_03(RequestATP request)
        {
            string nameStore = ProcedureName.pkg_ATP + ".SP_ATP_REA_DATA_RESUMEN";
            OracleDataReader oracleDataReader = (OracleDataReader)null;
            List<OracleParameter> oracleParameterList = new List<OracleParameter>();
            MasterDataATP masterDataAtp = new MasterDataATP();
            List<ComercializadorATP> comercializadorAtpList = new List<ComercializadorATP>();
            try
            {
                oracleParameterList.Add(new OracleParameter("P_NPOLICY", OracleDbType.Varchar2, (object)request.policy, ParameterDirection.Input));
                oracleParameterList.Add(new OracleParameter("P_NBRANCH", OracleDbType.Varchar2, (object)request.branch, ParameterDirection.Input));
                oracleParameterList.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Varchar2, (object)request.product, ParameterDirection.Input));
                string[] nameCursor = new string[2]
                {
                  "C_GENERAL",
                  "C_COMER"
                };
                oracleParameterList.Add(new OracleParameter(nameCursor[0], OracleDbType.RefCursor, ParameterDirection.Output));
                oracleParameterList.Add(new OracleParameter(nameCursor[1], OracleDbType.RefCursor, ParameterDirection.Output));
                oracleDataReader = (OracleDataReader)this.ExecuteByStoredProcedureVTCursores(nameStore, nameCursor, oracleParameterList);
                masterDataAtp = oracleDataReader.ReadRowsList<MasterDataATP>()[0];

                oracleDataReader.NextResult();
                foreach (ComercializadorATP readRows in oracleDataReader.ReadRowsList<ComercializadorATP>())
                    comercializadorAtpList.Add(readRows);

                masterDataAtp.COMER = comercializadorAtpList;

                ELog.CloseConnection(oracleDataReader);
            }
            catch (Exception ex)
            {
                ELog.CloseConnection(oracleDataReader);
                LogControl.save("CargarDataATP_03", ex.ToString(), "3");
                masterDataAtp = (MasterDataATP)null;
                throw ex;
            }
            return masterDataAtp;
        }

        private MasterDataATP CargarDataATP_04(RequestATP request, DocumentoATP doc)
        {
            string nameStore = ProcedureName.pkg_ATP + ".SP_ATP_REA_DATA_SOLICITUD";
            OracleDataReader oracleDataReader = (OracleDataReader)null;
            List<OracleParameter> oracleParameterList = new List<OracleParameter>();
            MasterDataATP masterDataAtp = new MasterDataATP();
            List<ContratanteATP> contratanteAtpList = new List<ContratanteATP>();
            List<AseguradoATP> aseguradoAtpList = new List<AseguradoATP>();
            List<BeneficiarioATP> beneficiarioAtpList = new List<BeneficiarioATP>();
            List<BeneficiarioCepATP> beneficiarioCepAtpList = new List<BeneficiarioCepATP>();
            List<CoverATP> coverAtpList = new List<CoverATP>();
            List<CoverMainATP> coverMainAtpList = new List<CoverMainATP>();
            try
            {
                oracleParameterList.Add(new OracleParameter("P_NPOLICY", OracleDbType.Varchar2, (object)request.policy, ParameterDirection.Input));
                oracleParameterList.Add(new OracleParameter("P_NBRANCH", OracleDbType.Varchar2, (object)request.branch, ParameterDirection.Input));
                oracleParameterList.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Varchar2, (object)request.product, ParameterDirection.Input));
                string[] nameCursor = new string[9]
                {
                  "C_GENERAL",
                  "C_CONT",
                  "C_ASEG",
                  "C_BENE",
                  "C_BENECEP",
                  "C_COVER",
                  "C_COVERMAIN",
                  "C_FIRMA",
                  "C_FIRMA_AS",
                };
                oracleParameterList.Add(new OracleParameter(nameCursor[0], OracleDbType.RefCursor, ParameterDirection.Output));
                oracleParameterList.Add(new OracleParameter(nameCursor[1], OracleDbType.RefCursor, ParameterDirection.Output));
                oracleParameterList.Add(new OracleParameter(nameCursor[2], OracleDbType.RefCursor, ParameterDirection.Output));
                oracleParameterList.Add(new OracleParameter(nameCursor[3], OracleDbType.RefCursor, ParameterDirection.Output));
                oracleParameterList.Add(new OracleParameter(nameCursor[4], OracleDbType.RefCursor, ParameterDirection.Output));
                oracleParameterList.Add(new OracleParameter(nameCursor[5], OracleDbType.RefCursor, ParameterDirection.Output));
                oracleParameterList.Add(new OracleParameter(nameCursor[6], OracleDbType.RefCursor, ParameterDirection.Output));
                oracleParameterList.Add(new OracleParameter(nameCursor[7], OracleDbType.RefCursor, ParameterDirection.Output));
                oracleParameterList.Add(new OracleParameter(nameCursor[8], OracleDbType.RefCursor, ParameterDirection.Output));
                oracleDataReader = (OracleDataReader)this.ExecuteByStoredProcedureVTCursores(nameStore, nameCursor, oracleParameterList);
                masterDataAtp = oracleDataReader.ReadRowsList<MasterDataATP>()[0];

                oracleDataReader.NextResult();
                foreach (ContratanteATP readRows in oracleDataReader.ReadRowsList<ContratanteATP>())
                    contratanteAtpList.Add(readRows);

                oracleDataReader.NextResult();
                foreach (AseguradoATP readRows in oracleDataReader.ReadRowsList<AseguradoATP>())
                    aseguradoAtpList.Add(readRows);

                oracleDataReader.NextResult();
                foreach (BeneficiarioATP readRows in oracleDataReader.ReadRowsList<BeneficiarioATP>())
                    beneficiarioAtpList.Add(readRows);

                oracleDataReader.NextResult();
                foreach (BeneficiarioCepATP readRows in oracleDataReader.ReadRowsList<BeneficiarioCepATP>())
                    beneficiarioCepAtpList.Add(readRows);

                oracleDataReader.NextResult();
                foreach (CoverATP readRows in oracleDataReader.ReadRowsList<CoverATP>())
                    coverAtpList.Add(readRows);

                oracleDataReader.NextResult();
                foreach (CoverMainATP readRows in oracleDataReader.ReadRowsList<CoverMainATP>())
                    coverMainAtpList.Add(readRows);

                oracleDataReader.NextResult();
                masterDataAtp.FIRMA = oracleDataReader.ReadRowsList<FirmaATP>()[0];
                oracleDataReader.NextResult();
                masterDataAtp.FIRMACO = oracleDataReader.ReadRowsList<FirmaAseATP>()[0];
                masterDataAtp.CONT = contratanteAtpList;
                masterDataAtp.ASEG = aseguradoAtpList;
                masterDataAtp.BENE = beneficiarioAtpList;
                masterDataAtp.BENECEP = beneficiarioCepAtpList;
                masterDataAtp.COVER = coverAtpList;
                masterDataAtp.COVERMAIN = coverMainAtpList;

                ELog.CloseConnection(oracleDataReader);
            }
            catch (Exception ex)
            {
                ELog.CloseConnection(oracleDataReader);
                LogControl.save("CargarDataATP_04", ex.ToString(), "3");
                masterDataAtp = (MasterDataATP)null;
                throw ex;
            }
            return masterDataAtp;
        }

        private MasterDataATP CargarDataATP_05(RequestATP request)
        {
            string nameStore = ProcedureName.pkg_ATP + ".SP_ATP_REA_DATA_CLAU_POL";
            OracleDataReader oracleDataReader = (OracleDataReader)null;
            List<OracleParameter> oracleParameterList = new List<OracleParameter>();
            MasterDataATP masterDataAtp = new MasterDataATP();
            List<ComercializadorATP> comercializadorAtpList = new List<ComercializadorATP>();
            try
            {
                oracleParameterList.Add(new OracleParameter("P_NPOLICY", OracleDbType.Varchar2, (object)request.policy, ParameterDirection.Input));
                oracleParameterList.Add(new OracleParameter("P_NBRANCH", OracleDbType.Varchar2, (object)request.branch, ParameterDirection.Input));
                oracleParameterList.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Varchar2, (object)request.product, ParameterDirection.Input));
                string[] nameCursor = new string[1] { "C_TABLE" };
                oracleParameterList.Add(new OracleParameter(nameCursor[0], OracleDbType.RefCursor, ParameterDirection.Output));

                oracleDataReader = (OracleDataReader)this.ExecuteByStoredProcedureVT(nameStore, nameCursor, oracleParameterList);

                masterDataAtp = oracleDataReader.ReadRowsList<MasterDataATP>()[0];

                ELog.CloseConnection(oracleDataReader);
            }
            catch (Exception ex)
            {
                ELog.CloseConnection(oracleDataReader);
                LogControl.save("CargarDataATP_05", ex.ToString(), "3");
                masterDataAtp = (MasterDataATP)null;
                throw ex;
            }
            return masterDataAtp;
        }

        private MasterDataATP CargarDataATP_07(RequestATP request)
        {
            string nameStore = ProcedureName.pkg_ATP + ".SP_ATP_REA_DATA_COBER_ADI_FALL";
            OracleDataReader oracleDataReader = (OracleDataReader)null;
            List<OracleParameter> oracleParameterList = new List<OracleParameter>();
            MasterDataATP masterDataAtp = new MasterDataATP();
            List<ComercializadorATP> comercializadorAtpList = new List<ComercializadorATP>();
            try
            {
                oracleParameterList.Add(new OracleParameter("P_NPOLICY", OracleDbType.Varchar2, (object)request.policy, ParameterDirection.Input));
                oracleParameterList.Add(new OracleParameter("P_NBRANCH", OracleDbType.Varchar2, (object)request.branch, ParameterDirection.Input));
                oracleParameterList.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Varchar2, (object)request.product, ParameterDirection.Input));
                string[] nameCursor = new string[1] { "C_TABLE" };
                oracleParameterList.Add(new OracleParameter(nameCursor[0], OracleDbType.RefCursor, ParameterDirection.Output));

                oracleDataReader = (OracleDataReader)this.ExecuteByStoredProcedureVT(nameStore, nameCursor, oracleParameterList);

                masterDataAtp = oracleDataReader.ReadRowsList<MasterDataATP>()[0];

                ELog.CloseConnection(oracleDataReader);
            }
            catch (Exception ex)
            {
                ELog.CloseConnection(oracleDataReader);
                LogControl.save("CargarDataATP_07", ex.ToString(), "3");
                masterDataAtp = (MasterDataATP)null;
                throw ex;
            }
            return masterDataAtp;
        }
        private MasterDataATP CargarDataATP_08(RequestATP request)
        {
            string nameStore = ProcedureName.pkg_ATP + ".SP_ATP_REA_DATA_DPS";
            OracleDataReader oracleDataReader = (OracleDataReader)null;
            List<OracleParameter> oracleParameterList = new List<OracleParameter>();
            MasterDataATP masterDataAtp = new MasterDataATP();
            List<ComercializadorATP> comercializadorAtpList = new List<ComercializadorATP>();
            try
            {
                oracleParameterList.Add(new OracleParameter("P_NPOLICY", OracleDbType.Varchar2, (object)request.policy, ParameterDirection.Input));
                oracleParameterList.Add(new OracleParameter("P_NBRANCH", OracleDbType.Varchar2, (object)request.branch, ParameterDirection.Input));
                oracleParameterList.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Varchar2, (object)request.product, ParameterDirection.Input));
                string[] nameCursor = new string[2] { "C_TABLE", "C_FIRMA" };

                oracleParameterList.Add(new OracleParameter(nameCursor[0], OracleDbType.RefCursor, ParameterDirection.Output));

                oracleParameterList.Add(new OracleParameter(nameCursor[1], OracleDbType.RefCursor, ParameterDirection.Output));

                oracleDataReader = (OracleDataReader)this.ExecuteByStoredProcedureVT(nameStore, nameCursor, oracleParameterList);
                masterDataAtp = oracleDataReader.ReadRowsList<MasterDataATP>()[0];

                oracleDataReader.NextResult();
                masterDataAtp.FIRMA = oracleDataReader.ReadRowsList<FirmaATP>()[0];

                ELog.CloseConnection(oracleDataReader);
            }
            catch (Exception ex)
            {
                ELog.CloseConnection(oracleDataReader);
                LogControl.save("CargarDataATP_08", ex.ToString(), "3");
                masterDataAtp = (MasterDataATP)null;
                throw ex;
            }
            return masterDataAtp;
        }
        private MasterDataATP CargarDataATP_09(RequestATP request)
        {
            string nameStore = ProcedureName.pkg_ATP + ".SP_ATP_REA_DATA_AFFIDAVIT";
            OracleDataReader oracleDataReader = (OracleDataReader)null;
            List<OracleParameter> oracleParameterList = new List<OracleParameter>();
            MasterDataATP masterDataAtp = new MasterDataATP();
            List<ContratanteATP> contratanteAtpList = new List<ContratanteATP>();
            try
            {
                oracleParameterList.Add(new OracleParameter("P_NPOLICY", OracleDbType.Varchar2, (object)request.policy, ParameterDirection.Input));
                oracleParameterList.Add(new OracleParameter("P_NBRANCH", OracleDbType.Varchar2, (object)request.branch, ParameterDirection.Input));
                oracleParameterList.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Varchar2, (object)request.product, ParameterDirection.Input));


                string[] nameCursor = new string[3]
                {
                  "C_BODY",
                  "C_CONT",
                  "C_FIRMA"
                };
                oracleParameterList.Add(new OracleParameter(nameCursor[0], OracleDbType.RefCursor, ParameterDirection.Output));
                oracleParameterList.Add(new OracleParameter(nameCursor[1], OracleDbType.RefCursor, ParameterDirection.Output));
                oracleParameterList.Add(new OracleParameter(nameCursor[2], OracleDbType.RefCursor, ParameterDirection.Output));


                oracleDataReader = (OracleDataReader)this.ExecuteByStoredProcedureVT(nameStore, nameCursor, oracleParameterList);
                masterDataAtp = oracleDataReader.ReadRowsList<MasterDataATP>()[0];
                oracleDataReader.NextResult();
                foreach (ContratanteATP readRows in oracleDataReader.ReadRowsList<ContratanteATP>())
                    contratanteAtpList.Add(readRows);

                oracleDataReader.NextResult();
                masterDataAtp.FIRMA = oracleDataReader.ReadRowsList<FirmaATP>()[0]; // SE AGREGOO FIRMA ASEGURADO

                ELog.CloseConnection(oracleDataReader);
            }
            catch (Exception ex)
            {
                ELog.CloseConnection(oracleDataReader);
                masterDataAtp = (MasterDataATP)null;
                throw ex;
            }
            return masterDataAtp;
        }
        private List<InfoPE_ATP> CargarDataPolizaElectronica(RequestATP request)
        {
            string nameStore = ProcedureName.pkg_ATP + ".SP_ATP_REA_DATA_USUARIOS";
            OracleDataReader oracleDataReader = (OracleDataReader)null;
            List<OracleParameter> oracleParameterList = new List<OracleParameter>();
            List<InfoPE_ATP> infoPE = new List<InfoPE_ATP>();
            try
            {
                oracleParameterList.Add(new OracleParameter("P_NPOLICY", OracleDbType.Varchar2, (object)request.policy, ParameterDirection.Input));
                oracleParameterList.Add(new OracleParameter("P_NBRANCH", OracleDbType.Varchar2, (object)request.branch, ParameterDirection.Input));
                oracleParameterList.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Varchar2, (object)request.product, ParameterDirection.Input));
                string[] nameCursor = new string[1] { "C_TABLA" };
                oracleParameterList.Add(new OracleParameter(nameCursor[0], OracleDbType.RefCursor, ParameterDirection.Output));

                oracleDataReader = (OracleDataReader)this.ExecuteByStoredProcedureVT(nameStore, nameCursor, oracleParameterList);

                foreach (InfoPE_ATP readRows in oracleDataReader.ReadRowsList<InfoPE_ATP>())
                    infoPE.Add(readRows);

                ELog.CloseConnection(oracleDataReader);
            }
            catch (Exception ex)
            {
                ELog.CloseConnection(oracleDataReader);
                LogControl.save("CargarDataPolizaElectronica", ex.ToString(), "3");
                infoPE = (List<InfoPE_ATP>)null;
            }
            return infoPE;
        }
        private List<InfoPE_ATP> CargarDataPolizaElectronicaAsegurado(RequestATP request)
        {
            string nameStore = ProcedureName.pkg_ATP + ".SP_ATP_REA_DATA_USUARIOS_ASE";
            OracleDataReader oracleDataReader = (OracleDataReader)null;
            List<OracleParameter> oracleParameterList = new List<OracleParameter>();
            List<InfoPE_ATP> infoPE = new List<InfoPE_ATP>();
            try
            {
                oracleParameterList.Add(new OracleParameter("P_NPOLICY", OracleDbType.Varchar2, (object)request.policy, ParameterDirection.Input));
                oracleParameterList.Add(new OracleParameter("P_NBRANCH", OracleDbType.Varchar2, (object)request.branch, ParameterDirection.Input));
                oracleParameterList.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Varchar2, (object)request.product, ParameterDirection.Input));
                string[] nameCursor = new string[1] { "C_TABLA" };
                oracleParameterList.Add(new OracleParameter(nameCursor[0], OracleDbType.RefCursor, ParameterDirection.Output));

                oracleDataReader = (OracleDataReader)this.ExecuteByStoredProcedureVT(nameStore, nameCursor, oracleParameterList);

                foreach (InfoPE_ATP readRows in oracleDataReader.ReadRowsList<InfoPE_ATP>())
                    infoPE.Add(readRows);

                ELog.CloseConnection(oracleDataReader);
            }
            catch (Exception ex)
            {
                ELog.CloseConnection(oracleDataReader);
                LogControl.save("CargarDataPolizaElectronicaAsegurado", ex.ToString(), "3");
                infoPE = (List<InfoPE_ATP>)null;
            }
            return infoPE;
        }
        #endregion

        #region Suspension de Poliza

        private PolizaSuspension CargarDataSuspension(RequestSuspATP request)
        {
            string nameStore = ProcedureName.pkg_ATP + ".SP_ATP_REA_DATA_SUSPENSION";
            List<OracleParameter> oracleParameterList = new List<OracleParameter>();
            PolizaSuspension dataSuspension = new PolizaSuspension();
            List<PolizaDetalleSuspension> detalleSuspension = new List<PolizaDetalleSuspension>();
            try
            {
                oracleParameterList.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, (object)request.sClient, ParameterDirection.Input));
                string[] nameCursor = new string[2] { "C_CLIENT", "C_POLICY" };
                oracleParameterList.Add(new OracleParameter(nameCursor[0], OracleDbType.RefCursor, ParameterDirection.Output));
                oracleParameterList.Add(new OracleParameter(nameCursor[1], OracleDbType.RefCursor, ParameterDirection.Output));

                OracleDataReader oracleDataReader = (OracleDataReader)this.ExecuteByStoredProcedureVT(nameStore, nameCursor, oracleParameterList);

                dataSuspension = oracleDataReader.ReadRowsList<PolizaSuspension>()[0];
                oracleDataReader.NextResult();

                foreach (PolizaDetalleSuspension readRows in oracleDataReader.ReadRowsList<PolizaDetalleSuspension>())
                    detalleSuspension.Add(readRows);

                dataSuspension.DETAIL = detalleSuspension;
                ELog.CloseConnection(oracleDataReader);
            }
            catch (Exception ex)
            {
                LogControl.save("CargarDataSuspension", ex.ToString(), "3");
                dataSuspension = (PolizaSuspension)null;
            }
            return dataSuspension;
        }

        private string GetCard(RequestSuspATP request)
        {
            string sCard = string.Empty;

            string nameStore = "INSUDB.INSPOLICY_CARDSUSP";
            List<OracleParameter> oracleParameterList = new List<OracleParameter>();
            try
            {
                oracleParameterList.Add(new OracleParameter("SCERTYPE", OracleDbType.Varchar2, (object)request.sCertype, ParameterDirection.Input));
                oracleParameterList.Add(new OracleParameter("NBRANCH", OracleDbType.Int32, (object)request.nBranch, ParameterDirection.Input));
                oracleParameterList.Add(new OracleParameter("NPRODUCT", OracleDbType.Int32, (object)request.nProduct, ParameterDirection.Input));
                oracleParameterList.Add(new OracleParameter("SCLIENT", OracleDbType.Varchar2, (object)request.sClient, ParameterDirection.Input));
                oracleParameterList.Add(new OracleParameter("NYEAR", OracleDbType.Int32, (object)request.nYear, ParameterDirection.Input));
                oracleParameterList.Add(new OracleParameter("NCARD", OracleDbType.Int32, (object)request.nCard, ParameterDirection.Input));

                //OracleParameter pCard = new OracleParameter("NCARD", OracleDbType.Int32, ParameterDirection.Output);
                //oracleParameterList.Add(pCard);

                this.ExecuteByStoredProcedureVT(nameStore, oracleParameterList);

                sCard = request.nCard.ToString();
                sCard = string.Format("{0}/{1}", sCard.PadLeft(5, '0'), request.nYear.ToString());
            }
            catch (Exception ex)
            {
                LogControl.save("GetCard", ex.ToString(), "3");
                throw ex;
            }
            return sCard;
        }

        public ResponsePrintVM PrintSuspATP(RequestSuspATP request)
        {
            ResponsePrintVM response = new ResponsePrintVM();
            PrintPolicyService printPolicyService = new PrintPolicyService();

            DocumentoATP doc = new DocumentoATP();
            string sDocName = ELog.obtainConfig("docSuspension");
            doc.nombre = sDocName;
            doc.nombreFinal = sDocName;
            doc.rutaTemplate = string.Format(@"{0}\{1}.docx", ELog.obtainConfig("rutaTemplateSusp"), sDocName);
            doc.rutaTmp = ELog.obtainConfig("rutaTmpSusp");
            doc.rutaDestino = string.Format(@"{0}{1}\", ELog.obtainConfig("rutaDestinoSusp"), DateTime.Today.ToString("yyyyMMdd"));
            doc.nombreCD = sDocName;

            int num = 0;
            int sent = 0;
            try
            {
                if (!Directory.Exists(doc.rutaTmp))
                    Directory.CreateDirectory(doc.rutaTmp);

                if (!Directory.Exists(doc.rutaDestino))
                    Directory.CreateDirectory(doc.rutaDestino);

                string print_res = "";
                bool flag = this.GenerarDocumentoSuspension(request, doc);
                print_res += flag ? "1" : "0";

                response.NCODE = 0;
                if (response.NCODE == 0)
                {
                    num = 1;
                    /*
                    if (request.send == "S")
                    {
                        response = sendPEATP(listaDoc[0].rutaDestino + "\\", listaDoc[0].nombreCD, request);
                        sent = response.NCODE == 0 ? 1 : 0;
                    }
                    */
                }
                response.SRESULTADO_PRINT = print_res;
                response.NGEN_CD = num;
                response.RESULT_SEND = sent;
                response.PATH_GEN = doc.rutaDestino + doc.nombreCD;

                try
                {
                    this.InsertLogATP(null, request.sClient, print_res, response.NCODE, sent, JsonConvert.SerializeObject(response));
                }
                catch (Exception ex)
                {
                    LogControl.save("PrintSuspATP", ex.ToString(), "3");
                }

            }
            catch (Exception ex)
            {
                LogControl.save("PrintSuspATP", ex.ToString(), "3");
                this.InsertLogATP(null, request.sClient, string.Empty, 2, 0, ex.Message);
                throw ex;
            }
            return response;
        }

        private bool GenerarDocumentoSuspension(RequestSuspATP request, DocumentoATP doc)
        {
            try
            {
                string rutaTmpImg = doc.rutaTmp.ToString();
                PolizaSuspension dataSuspension = new PolizaSuspension();
                dataSuspension = this.CargarDataSuspension(request);
                if (string.IsNullOrEmpty(dataSuspension.SCLINUMDOCU))
                {
                    throw new Exception("El cliente no tiene número de documento para generar la carta");
                }
                dataSuspension.SCARD = this.GetCard(request);
                doc.nombre = string.Format("{0}.docx", dataSuspension.SCLINUMDOCU);
                doc.nombreFinal = string.Format("{0}.pdf", dataSuspension.SCLINUMDOCU);
                doc.nombreCD = doc.nombreFinal;
                doc.rutaTmp = string.Format("{0}{1}", doc.rutaTmp, doc.nombre);
                //doc.rutaDestino = doc.rutaDestino + request.nPolicy;
                return new PrintPolicyService().CreateDocumentSusp(dataSuspension, doc);
            }
            catch (Exception ex)
            {
                LogControl.save("GenerarDocumentoSuspension", ex.ToString(), "3");
                return false;
            }
        }
        #endregion

        public bool GetTipoPolizaByProc(int NIDHEADERPROC)
        {
            var sPackageJobs = ProcedureName.pkg_CargaMasivaPD + ".GET_TYPE_POLICY_BY_HPROC";
            var P_NTYPE_POLICY = 0;
            var response = true;
            List<OracleParameter> parameters = new List<OracleParameter>();
            try
            {
                parameters.Add(new OracleParameter("P_NIDHEADERPROC", OracleDbType.Int32, NIDHEADERPROC, ParameterDirection.Input));
                OracleParameter NTYPE_POLICY = new OracleParameter("P_NTYPE_POLICY", OracleDbType.Int32, P_NTYPE_POLICY, ParameterDirection.Output);

                NTYPE_POLICY.Size = 200;

                parameters.Add(NTYPE_POLICY);
                this.ExecuteByStoredProcedureVT(sPackageJobs, parameters);
                P_NTYPE_POLICY = int.Parse(NTYPE_POLICY.Value.ToString());

                response = P_NTYPE_POLICY == 1 ? false : true;
            }
            catch (Exception ex)
            {
                LogControl.save("GetTipoPolizaByProc", ex.ToString(), "3");
                response = true;
            }

            return response;
        }
    }
}