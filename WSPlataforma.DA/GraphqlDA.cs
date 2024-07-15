using Google.Apis.Json;
using GraphQL;
using GraphQL.Client.Http;
using Newtonsoft.Json;
using Oracle.DataAccess.Client;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WSPlataforma.Entities.Graphql;
using WSPlataforma.Entities.QuotationModel.ViewModel;
using WSPlataforma.Entities.TechnicalTariffModel.BindingModel;
using WSPlataforma.Entities.TechnicalTariffModel.ViewModel;
using WSPlataforma.Util;
using static WSPlataforma.Entities.Graphql.CotizadorGraph;
using Rule = WSPlataforma.Entities.Graphql.Rule;

namespace WSPlataforma.DA
{
    public class GraphqlDA : ConnectionBase
    {
        public async Task<ResponseGraph> SendDataQuotationGraphql(string nroQuotation, string trxCode, int polizaMatriz, string rutaFiles)
        {
            var response = new ResponseGraph() { codError = 0 };

            try
            {
                // Trae data a enviar al servicio del cotizador
                var quotationRequest = await ReadInfoQuotationGEN(nroQuotation);

                // Se setea la url del servicio
                var URI_SendNotification = ELog.obtainConfig("urlGraphqlCotizador" + quotationRequest.nbranch);

                new QuotationDA().InsertLog(Convert.ToInt64(quotationRequest.dataCotizacion.quotationNumber), "01 - Genera data para enviar a Devmente", URI_SendNotification,
                             JsonConvert.SerializeObject(quotationRequest), null);

                #region Asegurado: Creación y envio de archivo json
                if (quotationRequest.aseguradosList.Count > 0)
                {
                    quotationRequest.url = await generarAseguradosJson(quotationRequest.dataCotizacion.requestId, quotationRequest.aseguradosList);
                }

                // Guarda log en tbl
                new QuotationDA().InsertLog(Convert.ToInt64(quotationRequest.dataCotizacion.quotationNumber), "02 - Se prepara info de asegurados", URI_SendNotification,
                     JsonConvert.SerializeObject(quotationRequest.aseguradosList), null);

                if (!string.IsNullOrEmpty(quotationRequest.url))
                {
                    // Se crea objeto Graphql
                    var responseReference = await adjuntarAsegurados(URI_SendNotification, "Cotizador", quotationRequest.dataCotizacion.quotationNumber, quotationRequest.url, quotationRequest.nbranch.ToString());

                    if (responseReference.data.newReference != null)
                    {
                        quotationRequest.dataCotizacion.workers = responseReference.data.newReference.id;
                        var responseFiles = await new WebServiceUtil().invocarServicioAseguradoFile(responseReference.data.newReference.url, quotationRequest.dataCotizacion.quotationNumber, quotationRequest.url);

                        // Guarda log en tbl
                        new QuotationDA().InsertLog(Convert.ToInt64(quotationRequest.dataCotizacion.quotationNumber), "04 - Se envió info de asegurados a Devmente", URI_SendNotification,
                             JsonConvert.SerializeObject(quotationRequest.dataCotizacion), JsonConvert.SerializeObject(responseFiles));
                    }
                    else
                    {
                        response.codError = 1;
                        response.message = "Hubo un error al consultar el tarifario. Favor de comunicarse con sistemas.";
                    }
                }
                #endregion

                #region Envío de archivos adjuntos
                if (HttpContext.Current.Request.Files != null && HttpContext.Current.Request.Files.Count > 0)
                {
                    int index = 0;
                    foreach (var item in HttpContext.Current.Request.Files)
                    {
                        var arch = HttpContext.Current.Request.Files[item.ToString()];
                        // Se crea objeto Graphql
                        var queryReference = new GraphQLRequest
                        {
                            Query = @"mutation ($contentType: String) {
                                   newReference(contentType: $contentType){
                                        id,
                                        url
                                    }
                                }",
                            Variables = new { contentType = arch.ContentType }
                        };

                        // Objeto Graphql se pasa a json 
                        var requestReference = JsonConvert.SerializeObject(queryReference);

                        // Se invoca a servicio Graphql
                        var jsonReference = await new WebServiceUtil().invocarServicioGraphql(requestReference, URI_SendNotification, "Cotizador", quotationRequest.nbranch.ToString());
                        var responseReference2 = JsonConvert.DeserializeObject<ResponseGraph>(jsonReference, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                        // Guarda log en tbl
                        new QuotationDA().InsertLog(Convert.ToInt64(quotationRequest.dataCotizacion.quotationNumber), "05 - Se envió adjuntos para generar url Graphql", URI_SendNotification,
                            JsonConvert.SerializeObject(requestReference), JsonConvert.SerializeObject(responseReference2));

                        if (responseReference2.data.newReference != null)
                        {
                            quotationRequest.dataCotizacion.files.Add(responseReference2.data.newReference.id);
                            quotationRequest.dataCotizacion.filesNames.Add(new FileName
                            {
                                id = responseReference2.data.newReference.id,
                                name = arch.FileName
                            });

                            var responseFiles = await new WebServiceUtil().invocarServicioWithFiles(null, responseReference2.data.newReference.url, arch, quotationRequest.dataCotizacion.quotationNumber, rutaFiles);

                            // Guarda log en tbl
                            new QuotationDA().InsertLog(Convert.ToInt64(quotationRequest.dataCotizacion.quotationNumber), "06 - Se envía adjunto Graphql a devmente", URI_SendNotification,
                                JsonConvert.SerializeObject(responseReference2.data.newReference), JsonConvert.SerializeObject(responseFiles));
                        }
                        else
                        {
                            response.codError = 1;
                            response.message = "Hubo un error en el envió de la cotización a técnica. Favor de comunicarse con sistemas.";
                        }

                        index++;
                    }
                }
                #endregion

                if (response.codError == 0)
                {
                    // Se crea objeto Graphql
                    var queryNotification = new GraphQLRequest
                    {
                        Query = @"mutation createQuotationRequest($quotation: QuotationRequestInput){
                            createQuotationRequest(quotation: $quotation)
                        }",
                        Variables = new { quotation = quotationRequest.dataCotizacion }
                    };

                    // Objeto Graphql se pasa a json 
                    var request = JsonConvert.SerializeObject(queryNotification, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                    // Se invoca a servicio Graphql
                    var json = await new WebServiceUtil().invocarServicioGraphql(request, URI_SendNotification, "Cotizador", quotationRequest.nbranch.ToString());
                    response = JsonConvert.DeserializeObject<ResponseGraph>(json, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                    new QuotationDA().InsertLog(Convert.ToInt64(quotationRequest.dataCotizacion.quotationNumber), "07 - Se envía la cotización al cotizador", URI_SendNotification,
                       request, JsonConvert.SerializeObject(response));

                    if (response.data != null)
                    {
                        if (response.data.createQuotationRequest == null && response.errors == null)
                        {
                            response.codError = 1;
                            response.message = "Hubo un error en el envió de la cotización a técnica. Favor de comunicarse con sistemas.";
                        }
                        else
                        {
                            if (response.errors != null)
                            {
                                response.codError = 1;

                                foreach (var item in response.errors)
                                {
                                    response.message = response.message + " " + item.message;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (response.errors != null)
                        {
                            response.codError = 1;

                            foreach (var item in response.errors)
                            {
                                response.message = response.message + " " + item.message;
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                response.codError = 1;
                response.message = ex.ToString();
                LogControl.save("SendDataQuotationGraphql", ex.ToString(), "3");
            }

            return await Task.FromResult(response);
        }

        public GraphQLRequest generateQuerySearchPremiumApVg(QuotationTariffGraphql quotationRequest)
        {
            var response = new GraphQLRequest()
            {
                Query = @"query SearchPremium($quotation: QuotationInput) {
                                      searchPremium(quotation: $quotation) {
                                        quotationNumber
                                        quotationCode
                                        quotationId
                                        responseDate
                                        channel
                                    netRate
                                    riskRate
                                    riskPremium
                                    commercialPremium
                                    isMin
                                    totalValueWithIgv
                                    totalPremium
                                    unitCommercialPremium
                                    igvUnitValue
                                    unitCommercialPremiumWithIgv
                                    anualCommercialPremium
                                    studentCoverages
                                        module {
                                          code
                                          description
                                        }
                                        coverages {
                                          id
                                          code
                                          description
                                          capital
                                          rate
                                          value
                                        }
                                        assistences {
                                          id
                                      description
                                          code
                                          rate
                                          value
                                        }
                                        benefits {
                                          id
                                          code
                                          description
                                          rate
                                          value
                                        }
                                        readjustmentFactors {
                                          id
                                          code
                                          description
                                          rate
                                          value
                                        }
                                        surcharges {
                                          id
                                          code
                                          description
                                          rate
                                          value
                                        }
                                    additionalServices {
                                      id
                                      description
                                      value
                                      hours
                                      amount
                                    }
                                      }
                                    }",
                Variables = new { quotation = quotationRequest }
            };
            return response;
        }

        public GraphQLRequest generateQuerySearchPremiumDes(QuotationTariffGraphqlDes quotationRequest)
        {
            var response = new GraphQLRequest()
            {
                Query = @"query searchPremium(
                          $quotation: QuotationInput
                        ) {
                          searchPremium(quotation: $quotation)
                          {
                            quotationNumber
                            quotationCode
                            quotationId
                            responseDate
                            riskRate
                            commercialRate
                            riskPremium
                            commercialPremium
                            isMin
                            module{
                              code
                              description
                            }
                            coverages{
                              id
                              code
                              description
                              capital
                              rate
                              value
                            }
                            assistances{
                              id
                              code
                              description
                              rate
                              value
                            }
                            benefits{
                              id
                              code
                              description
                            }
                            readjustmentFactors{
                              id
                              code
                              description
                              rate
                            }
                            surcharges{
                              id
                              code
                              description
                              rate
                              value
                            }
                            faults {code name description}
                          }
                        }",
                Variables = new { quotation = quotationRequest }
            };
            return response;
        }

        public async Task<ResponseGraph> SendDataTarifarioGraphql(validaTramaVM dataCotizacion)
        {
            var response = new ResponseGraph() { codError = 0 };

            try
            {
                // Inicializar
                var quotationRequest = new QuotationTariffGraphql();
                var quotationRequestDes = new QuotationTariffGraphqlDes();

                // Se setea la url del servicio
                var URI_SendNotification = ELog.obtainConfig("urlGraphqlTarifario" + dataCotizacion.datosPoliza.branch);

                // Trae data a enviar al servicio del cotizador
                if (new string[] { ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch") }.Contains(dataCotizacion.datosPoliza.branch.ToString()))
                {
                    quotationRequest = ReadInfoTarfarioPD(dataCotizacion);
                    LogControl.save(dataCotizacion.codProceso, "SendDataTarifarioGraphql Ini: " + JsonConvert.SerializeObject(quotationRequest, Formatting.None), "2", dataCotizacion.codAplicacion);
                }

                if (new string[] { ELog.obtainConfig("vidaIndividualBranch") }.Contains(dataCotizacion.datosPoliza.branch.ToString()))
                {
                    quotationRequestDes = ReadInfoTarfarioDes(dataCotizacion);
                }

                // Se sube archivo
                if (!string.IsNullOrEmpty(dataCotizacion.ruta))
                {
                    // Genera url para saber donde adjuntar archivo
                    var responseReference = await adjuntarAsegurados(URI_SendNotification, "Tarifario", quotationRequest.quotationNumber, dataCotizacion.ruta, dataCotizacion.datosPoliza.branch.ToString());

                    LogControl.save(dataCotizacion.codProceso, "adjuntarAsegurados Ini: " + JsonConvert.SerializeObject(responseReference, Formatting.None), "2", dataCotizacion.codAplicacion);

                    if (responseReference.data.newReference != null)
                    {
                        quotationRequest.workers = responseReference.data.newReference.id;
                        var responseFiles = await new WebServiceUtil().invocarServicioAseguradoFile(responseReference.data.newReference.url, quotationRequest.quotationNumber, dataCotizacion.ruta);

                        LogControl.save(dataCotizacion.codProceso, "adjuntarAsegurados Fin: " + JsonConvert.SerializeObject(responseFiles, Formatting.None), "2", dataCotizacion.codAplicacion);

                        // Guarda log en tbl
                        new QuotationDA().InsertLog(Convert.ToInt64(quotationRequest.quotationNumber), "Envío a Devmente (Files - pre signed)",
                            responseReference.data.newReference.url,
                            null, responseFiles);
                    }
                    else
                    {
                        response.codError = 1;
                        response.message = "Hubo un error al consultar el tarifario. Favor de comunicarse con sistemas.";
                    }
                }

                //CREAR Y SUBIR AQUI JSON
                if (new string[] { ELog.obtainConfig("vidaIndividualBranch") }.Contains(dataCotizacion.datosPoliza.branch.ToString()))
                {
                    var pais = "";
                    if (dataCotizacion.datosContratante.nacionalidad == "1")
                    {
                        pais = "PERU";
                    }
                    else
                    {
                        pais = "OTRO";
                    }

                    var datosTrabajador = new WorkerType { code_core = "1", description = "EMPLEADO" };

                    List<insuredGraph> datosAsegurado = new List<insuredGraph>()
                    {
                        new insuredGraph
                        {
                            name = dataCotizacion.datosContratante.nombre,
                            surname1 = dataCotizacion.datosContratante.apePaterno,
                            surname2 = dataCotizacion.datosContratante.apeMaterno,
                            documentType = dataCotizacion.datosContratante.codDocumento,
                            documentNumber = dataCotizacion.datosContratante.documento,
                            birthDate = Convert.ToDateTime(dataCotizacion.datosContratante.fechaNacimiento).ToString("yyyy-MM-ddThh:mm:ssZ"),
                            gender = dataCotizacion.datosContratante.sexo,
                            workerType = datosTrabajador,
                            salary = 1500,
                            status = "ACTIVE",
                            countryOfBirth = pais,
                        }
                    };

                    string path = null;
                    path = String.Format(ELog.obtainConfig("pathPrincipalDPS"), quotationRequestDes.requestId) + "\\";

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    System.IO.File.WriteAllText(path + "asegurados.json", JsonConvert.SerializeObject(datosAsegurado));

                    var responseReference = await adjuntarAseguradosDes(URI_SendNotification, "Cotizador", path, dataCotizacion.datosPoliza.branch.ToString());

                    if (responseReference.data.newReference != null)
                    {
                        quotationRequestDes.detail = responseReference.data.newReference.id;
                        var responseFiles = await new WebServiceUtil().invocarServicioAseguradoFile(responseReference.data.newReference.url, quotationRequestDes.requestId, path);
                        /*
                        // Guarda log en tbl
                        new QuotationDA().InsertLog(Convert.ToInt64(quotationRequest.dataCotizacion.quotationNumber), "Envío a Devmente (Files - pre signed)",
                            responseReference.data.newReference.url,
                            null, responseFiles);
                        */
                    }
                    else
                    {
                        response.codError = 1;
                        response.message = "Hubo un error al consultar el tarifario. Favor de comunicarse con sistemas.";
                    }
                }

                if (response.codError == 0)
                {
                    // Se crea objeto Graphql
                    var queryNotification = new GraphQLRequest();

                    if (new string[] { ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch") }.Contains(dataCotizacion.datosPoliza.branch.ToString()))
                    {
                        queryNotification = generateQuerySearchPremiumApVg(quotationRequest);
                    }
                    if (new string[] { ELog.obtainConfig("vidaIndividualBranch") }.Contains(dataCotizacion.datosPoliza.branch.ToString()))
                    {
                        queryNotification = generateQuerySearchPremiumDes(quotationRequestDes);
                    }

                    List<string> Excluir = new List<string>();

                    //Excluir.Add("insuredQuantity");
                    //Excluir.Add("hours");
                    //Excluir.Add("coverages");           //REVISAR COMO INGRESAR COVERAGE
                    // You can convert it back to an array if you would like to
                    string[] ArrayExcluir = Excluir.ToArray();

                    // Objeto Graphql se pasa a json 
                    var request = JsonConvert.SerializeObject(queryNotification, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        ContractResolver = new DynamicContractResolver(ArrayExcluir)
                    });

                    // Se invoca a servicio Graphql
                    LogControl.save(dataCotizacion.codProceso, "Tarifario Ini: " + JsonConvert.SerializeObject(request, Formatting.None), "2", dataCotizacion.codAplicacion);
                    var json = await new WebServiceUtil().invocarServicioGraphql(request, URI_SendNotification, "Tarifario", dataCotizacion.datosPoliza.branch.ToString());
                    response = JsonConvert.DeserializeObject<ResponseGraph>(json, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });

                    LogControl.save(dataCotizacion.codProceso, "Tarifario Fin: " + JsonConvert.SerializeObject(response, Formatting.None), "2", dataCotizacion.codAplicacion);

                    new QuotationDA().InsertLog(Convert.ToInt64(quotationRequest.quotationNumber), "Query searchPremium", URI_SendNotification,
                       request.ToString(), response.data + " | " + response.errors + " | " + dataCotizacion.codProceso);

                    if (response.data == null || (response.data.searchPremium == null && response.errors == null))
                    {
                        response.codError = 1;
                        response.message = "Hubo un error al consultar el tarifario. Favor de comunicarse con sistemas.";
                    }
                    else
                    {
                        if (response.errors != null)
                        {
                            response.codError = 1;

                            foreach (var item in response.errors)
                            {
                                response.message = response.message + " " + item.message;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.codError = 1;
                //response.message = ex.ToString();
                response.message = "Hubo un error al consultar el tarifario. Favor de comunicarse con sistemas";
                LogControl.save("SendDataTarifarioGraphql", ex.ToString(), "3");
            }

            return await Task.FromResult(response);
        }

        public async Task<ResponseGraph> getAlcance(string nbranch)
        {
            var response = new ResponseGraph() { codError = 0 };

            try
            {
                // Se setea la url del servicio
                var URI_SendNotification = ELog.obtainConfig("urlGraphqlTarifario" + nbranch);

                // Se crea objeto Graphql
                var queryNotification = new GraphQLRequest
                {
                    Query = @"query{
                                  coverageScopes{
                                    id,
                                    description
                                  }
                                }",
                };

                // Objeto Graphql se pasa a json 
                var request = JsonConvert.SerializeObject(queryNotification);

                // Se invoca a servicio Graphql
                var json = await new WebServiceUtil().invocarServicioGraphql(request, URI_SendNotification, "Tarifario", nbranch);
                response = JsonConvert.DeserializeObject<ResponseGraph>(json, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                if (response.data == null || (response.data.coverageScopes == null && response.errors == null))
                {
                    response.codError = 1;
                    response.message = "Hubo un error al consultar el servicio.";
                }
                else
                {
                    if (response.errors != null)
                    {
                        response.codError = 1;

                        foreach (var item in response.errors)
                        {
                            response.message = response.message + " " + item.message;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.codError = 1;
                response.message = ex.ToString();
                LogControl.save("getAlcance", ex.ToString(), "3");
            }

            return await Task.FromResult(response);
        }

        public async Task<ResponseGraph> GetDepartamentos()
        {
            var response = new ResponseGraph() { codError = 0 };

            try
            {
                // Se setea la url del servicio
                var URI_SendNotification = ELog.obtainConfig("urlGraphqlCore");

                // Se crea objeto Graphql
                var queryNotification = new GraphQLRequest
                {
                    Query = @"{
                                departments {
                                    items {
                                        id
                                        description
                                    }
                                }
                            }",
                };

                // Objeto Graphql se pasa a json 
                var request = JsonConvert.SerializeObject(queryNotification);

                // Se invoca a servicio Graphql
                var json = await new WebServiceUtil().invocarServicioGraphql(request, URI_SendNotification, "Core");
                response = JsonConvert.DeserializeObject<ResponseGraph>(json, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                if (response.data == null || (response.data.departments == null && response.errors == null))
                {
                    response.codError = 1;
                    response.message = "Hubo un error al consultar el servicio.";
                }
                else
                {
                    if (response.errors != null)
                    {
                        response.codError = 1;

                        foreach (var item in response.errors)
                        {
                            response.message = response.message + " " + item.message;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.codError = 1;
                response.message = ex.ToString();
                LogControl.save("GetDepartamentos", ex.ToString(), "3");
            }

            return await Task.FromResult(response);
        }

        public QuotationTariffGraphql ReadInfoTarfarioPD(validaTramaVM dataCotizacion)
        {
            var response = new QuotationTariffGraphql();
            var trxCode = dataCotizacion.datosPoliza.trxCode;

            dataCotizacion.PolizaMatriz = validarAforo(dataCotizacion.codRamo, dataCotizacion.datosPoliza.codTipoPerfil, dataCotizacion.datosPoliza.trxCode) ? false : dataCotizacion.PolizaMatriz;

            try
            {
                response.id = dataCotizacion.datosPoliza.idTariff;
                response.version = Convert.ToInt32(dataCotizacion.datosPoliza.versionTariff);
                response.quotationNumber = string.IsNullOrEmpty(dataCotizacion.nroCotizacion.ToString()) ? "0" : dataCotizacion.nroCotizacion.ToString();
                response.requestDate = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss.fffZ");
                response.requestId = dataCotizacion.codProceso;
                response.currency = dataCotizacion.datosPoliza.codMon;
                response.processType = "QUOTATION";
                response.type = dataCotizacion.PolizaMatriz && (trxCode == "EM" || trxCode == "E" || trxCode == "EMISION") ? "MATRIX_EMISSION" : dataCotizacion.PolizaMatriz && (trxCode == "RE" || trxCode == "R") ? "MATRIX_RENOVATION" : getQuotationType(dataCotizacion.datosPoliza.trxCode);
                response.queryChannel = "DIGITAL_PLATFORM";
                response.contractor = new Contractor()
                {
                    code = dataCotizacion.datosContratante.codContratante,
                    description = dataCotizacion.datosContratante.desContratante
                };
                response.channel = dataCotizacion.codCanal;
                response.policyInitDate = Convert.ToDateTime(dataCotizacion.datosPoliza.InicioVigPoliza).ToString("yyyy-MM-ddThh:mm:ss.fffZ");
                response.policyEndDate = Convert.ToDateTime(dataCotizacion.datosPoliza.FinVigPoliza).ToString("yyyy-MM-ddThh:mm:ss.fffZ");
                response.paymentFrecuency = cantidadMeses(Convert.ToInt32(dataCotizacion.datosPoliza.codTipoFrecuenciaPago), Convert.ToInt32(dataCotizacion.datosPoliza.branch));
                response.billingType = dataCotizacion.datosPoliza.codTipoFacturacion;
                response.policyType = dataCotizacion.datosPoliza.codTipoNegocio;
                response.colocationType = dataCotizacion.datosPoliza.codTipoModalidad;
                response.profile = getProfileRepeat(dataCotizacion.datosPoliza.branch.ToString(), dataCotizacion.datosPoliza.codTipoPerfil);
                response.insuredQuantity = Convert.ToInt32(dataCotizacion.CantidadTrabajadores);
                response.activity = dataCotizacion.datosPoliza.CodActividadRealizar;
                response.location = dataCotizacion.datosPoliza.codUbigeo == null ? string.Empty : dataCotizacion.datosPoliza.codUbigeo.ToString();
                response.module = dataCotizacion.datosPoliza.codTipoPlan;
                response.hours = dataCotizacion.datosPoliza.temporalidad;
                response.scope = dataCotizacion.datosPoliza.codAlcance.ToString(); //codigo de alcance
                response.isMine = Convert.ToBoolean(dataCotizacion.mina);
                response.igvRate = new QuotationDA().GetIGV(dataCotizacion);
                response.workers = string.Empty;
                response.pensionAmount = Generic.valRentaEstudiantil(dataCotizacion.datosPoliza.branch.ToString(), dataCotizacion.datosPoliza.codTipoProducto, dataCotizacion.datosPoliza.codTipoPerfil) ? dataCotizacion.montoPlanilla : 0;
                response.coverages = dataCotizacion.lcoberturas != null && dataCotizacion.lcoberturas.Count > 0 ? coverageList(dataCotizacion.lcoberturas) : new List<CoverageTariff>();
                response.assistances = dataCotizacion.lasistencias != null && dataCotizacion.lasistencias.Count > 0 ? dataCotizacion.lasistencias.Select(c => c.codAsistencia).ToList() : new List<string>();
                response.benefits = dataCotizacion.lbeneficios != null && dataCotizacion.lbeneficios.Count > 0 ? dataCotizacion.lbeneficios.Select(c => c.codBeneficio).ToList() : new List<string>();
                response.surcharges = dataCotizacion.lrecargos != null && dataCotizacion.lrecargos.Count > 0 ? dataCotizacion.lrecargos.Select(c => c.codRecargo).ToList() : new List<string>();
                response.additionalServices = dataCotizacion.lservAdicionales != null && dataCotizacion.lservAdicionales.Count > 0 ? additionalServiceList(dataCotizacion.lservAdicionales) : new List<AdditionalServiceTariff>();
                response.files = new List<string>();
            }
            catch (Exception ex)
            {
                LogControl.save("ReadInfoTarfarioPD", ex.ToString(), "3");
            }

            return response;
        }

        public QuotationTariffGraphqlDes ReadInfoTarfarioDes(validaTramaVM dataCotizacion)
        {

            var response = new QuotationTariffGraphqlDes()
            {
                //quotationNumber = string.IsNullOrEmpty(dataCotizacion.nroCotizacion.ToString()) ? "0" : dataCotizacion.nroCotizacion.ToString(),
                //requestDate = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss.fffZ"),
                //requestId = dataCotizacion.codProceso,
                currency = dataCotizacion.datosPoliza.codMon,
                processType = "QUOTATION",
                type = getQuotationType(dataCotizacion.datosPoliza.trxCode),
                queryChannel = "DIGITAL_PLATFORM",
                contractor = new Contractor()
                {
                    code = dataCotizacion.datosContratante.codContratante,
                    description = dataCotizacion.datosContratante.desContratante
                },
                answers = dpsList(dataCotizacion.datosDPS),
                channel = dataCotizacion.codCanal,
                policyInitDate = Convert.ToDateTime(dataCotizacion.datosPoliza.InicioVigPoliza).ToString("yyyy-MM-ddThh:mm:ssZ"),
                policyEndDate = Convert.ToDateTime(dataCotizacion.datosPoliza.FinVigPoliza).ToString("yyyy-MM-ddThh:mm:ssZ"),
                paymentFrequency = cantidadMeses(Convert.ToInt32(dataCotizacion.datosPoliza.codTipoFrecuenciaPago), Convert.ToInt32(dataCotizacion.codRamo)),
                //paymentFrecuency = cantidadMeses(Convert.ToInt32(dataCotizacion.datosPoliza.codTipoFrecuenciaPago)),
                billingType = dataCotizacion.datosPoliza.codTipoFacturacion,    //Por Poliza
                policyType = dataCotizacion.datosPoliza.codTipoNegocio,         //Individual
                renewalType = dataCotizacion.datosPoliza.renewalType,           //No Renovable
                creditType = dataCotizacion.datosPoliza.creditType,             //Producto
                //profile = getProfileRepeat(dataCotizacion.datosPoliza.branch.ToString(), dataCotizacion.datosPoliza.codTipoPerfil),
                //insuredQuantity = Convert.ToInt32(dataCotizacion.CantidadTrabajadores),
                //activity = dataCotizacion.datosPoliza.CodActividadRealizar,
                capital = dataCotizacion.datosPoliza.capitalCredito.ToString(),
                //occupation = dataCotizacion.datosPoliza.occupation,               //V.C.F.  V.2
                occupation = dataCotizacion.datosPoliza.activity,
                location = dataCotizacion.datosPoliza.codUbigeo.ToString(),
                module = dataCotizacion.datosPoliza.codTipoPlan,
                renewalFrequency = dataCotizacion.datosPoliza.renewalFrequency,
                //economicActivity = dataCotizacion.datosPoliza.economicActivity,   //V.C.F.  V.2
                economicActivity = dataCotizacion.datosPoliza.activity,
                //hours = dataCotizacion.datosPoliza.temporalidad,
                ///scope = dataCotizacion.datosPoliza.codAlcance.ToString(), //codigo de alcance
                detail = string.Empty,
                coverages = dataCotizacion.lcoberturas != null && dataCotizacion.lcoberturas.Count > 0 ? coverageList(dataCotizacion.lcoberturas) : new List<CoverageTariff>(), //CoverageTariff! Agregar si permite
                //coverages = dataCotizacion.lcoberturas != null && dataCotizacion.lcoberturas.Count > 0 ? coverageListDes(dataCotizacion.lcoberturas) : new List<String>(),
                assistances = dataCotizacion.lasistencias != null && dataCotizacion.lasistencias.Count > 0 ? dataCotizacion.lasistencias.Select(c => c.codAsistencia).ToList() : new List<string>(),
                benefits = dataCotizacion.lbeneficios != null && dataCotizacion.lbeneficios.Count > 0 ? dataCotizacion.lbeneficios.Select(c => c.codBeneficio).ToList() : new List<string>(),
                surcharges = dataCotizacion.lrecargos != null && dataCotizacion.lrecargos.Count > 0 ? dataCotizacion.lrecargos.Select(c => c.codRecargo).ToList() : new List<string>(),
                //files = new List<string>(),
            };

            return response;
        }

        public string getProfileRepeat(string nbranch, string codPerfil)
        {
            string perfilEquvalente = "0";

            if (nbranch == ELog.obtainConfig("accidentesBranch"))
            {
                var repeatProfile = new string[] { "7", "8" };
                if (repeatProfile.Contains(codPerfil))
                {
                    if (codPerfil == "7")
                    {
                        perfilEquvalente = "3";
                    }

                    if (codPerfil == "8")
                    {
                        perfilEquvalente = "4";
                    }
                }
                else
                {
                    perfilEquvalente = codPerfil;
                }
            }
            else
            {
                var repeatProfile = new string[] { "7", "8", "9", "10" };
                if (repeatProfile.Contains(codPerfil))
                {
                    if (codPerfil == "7")
                    {
                        perfilEquvalente = "3";
                    }

                    if (codPerfil == "8")
                    {
                        perfilEquvalente = "7";
                    }

                    if (codPerfil == "9" || codPerfil == "10")
                    {
                        perfilEquvalente = "8";
                    }
                }
                else
                {
                    perfilEquvalente = codPerfil;
                }
            }

            return perfilEquvalente;
        }

        public List<string> coverageListDes(List<Entities.QuotationModel.BindingModel.coberturaPropuesta> lcoberturas)
        {
            var list = new List<string>();

            foreach (var item in lcoberturas)
            {
                list.Add(item.codCobertura);
            }

            return list;
        }

        public List<AdditionalServiceTariff> additionalServiceList(List<servicioAdicionalPropuesto> lservicios)
        {
            var list = new List<AdditionalServiceTariff>();

            foreach (var item in lservicios)
            {
                var service = new AdditionalServiceTariff()
                {
                    code = item.codServAdicionales,
                    hours = Convert.ToInt32(item.amount)
                };

                list.Add(service);
            }

            return list;
        }

        public List<CoverageTariff> coverageList(List<Entities.QuotationModel.BindingModel.coberturaPropuesta> lcoberturas)
        {
            var list = new List<CoverageTariff>();

            foreach (var item in lcoberturas)
            {
                var cover = new CoverageTariff()
                {
                    id = item.codCobertura,
                    capital = item.sumaPropuesta
                };

                list.Add(cover);
            }

            return list;
        }

        public List<AnswerInput> dpsList(List<Entities.QuotationModel.BindingModel.DatosDPS> dpsDatos)
        {
            var list = new List<AnswerInput>();

            foreach (var item in dpsDatos)
            {
                var dps = new AnswerInput()
                {
                    question = item.question,
                    type = item.type,
                    value = item.value,
                    detail = (item.detail == null) || (item.detail.type == "" && item.detail.value == "") || ((item.detail.type == "NUMBER" || item.detail.type == "TEXT") && (item.detail.value == "")) ? null : new Entities.Graphql.AnswerDetailInput() { type = item.detail.type, value = item.detail.value },
                    items = item.items != null ? new List<AnswerInput>() : null
                };

                list.Add(dps);
            }

            return list;
        }


        public string getQuotationType(string trxCode)
        {
            string quotationType = "EMISSION";

            switch (trxCode)
            {
                case "IN":
                case "I":
                case "INCLUSION":
                    quotationType = "INCLUSION";
                    break;
                case "RE":
                case "R":
                    quotationType = "RENOVATION";
                    break;
                case "EMISION":
                case "E":
                case "EM":
                    quotationType = "EMISSION";
                    break;
                case "RECOTIZACION":
                    quotationType = "RECOTIZACION";
                    break;
                case "DE":
                case "DECLARACION":
                    quotationType = "DECLARATION";
                    break;
                default:
                    break;
            }

            return quotationType;
        }

        public async Task<genericTechnicalGraph> ReadInfoQuotationGEN(string nroCotizacion)
        {
            var response = new genericTechnicalGraph();

            List<OracleParameter> parameter = new List<OracleParameter>();
            string storeprocedure = ProcedureName.sp_LeerData_PD;

            OracleDataReader dr = null;
            try
            {
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, nroCotizacion, ParameterDirection.Input));
                string[] arrayCursor = { "C_TABLE_COT", "C_TABLE_PLL_ACT", "C_TABLE_COVER", "C_TABLE_ASSISTANCE", "C_TABLE_BENEFIT", "C_TABLE_SURCHARGE", "C_TABLE_ADDITIONAL" };
                OracleParameter C_CAB = new OracleParameter(arrayCursor[0], OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter C_PLANILLA = new OracleParameter(arrayCursor[1], OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter C_TABLE_COVER = new OracleParameter(arrayCursor[2], OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter C_TABLE_ASSISTANCE = new OracleParameter(arrayCursor[3], OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter C_TABLE_BENEFIT = new OracleParameter(arrayCursor[4], OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter C_TABLE_SURCHARGE = new OracleParameter(arrayCursor[5], OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter C_TABLE_ADDITIONAL = new OracleParameter(arrayCursor[6], OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_CAB);
                parameter.Add(C_PLANILLA);
                parameter.Add(C_TABLE_COVER);
                parameter.Add(C_TABLE_ASSISTANCE);
                parameter.Add(C_TABLE_BENEFIT);
                parameter.Add(C_TABLE_SURCHARGE);
                parameter.Add(C_TABLE_ADDITIONAL);

                dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storeprocedure, arrayCursor, parameter);

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        response.flagMatriz = dr["NPOLIZA_MATRIZ"] == DBNull.Value ? 0 : Convert.ToInt32(dr["NPOLIZA_MATRIZ"].ToString());
                        response.nbranch = dr["RAMO"] == DBNull.Value ? 0 : Convert.ToInt32(dr["RAMO"].ToString());
                        response.stransac = dr["STRANSAC"] == DBNull.Value ? "0" : dr["STRANSAC"].ToString();
                        response.dataCotizacion.quotationNumber = dr["NUM_COTIZACION"] == DBNull.Value ? "0" : dr["NUM_COTIZACION"].ToString();
                        response.dataCotizacion.requestDate = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss.fffZ");
                        response.dataCotizacion.requestId = dr["NID_PROC"] == DBNull.Value ? string.Empty : dr["NID_PROC"].ToString(); //
                        response.dataCotizacion.currency = dr["COD_CURRENCY"] == DBNull.Value ? string.Empty : dr["COD_CURRENCY"].ToString();//
                        response.dataCotizacion.processType = dr["DES_PROC_TYPE"] == DBNull.Value ? string.Empty : dr["DES_PROC_TYPE"].ToString();
                        response.dataCotizacion.type = dr["DES_TIPO_COT"] == DBNull.Value ? "" : dr["DES_TIPO_COT"].ToString();
                        response.dataCotizacion.queryChannel = "DIGITAL_PLATFORM";
                        response.dataCotizacion.contractor = new Contractor()
                        {
                            code = dr["COD_CONTRATANTE"] == DBNull.Value ? "0" : dr["COD_CONTRATANTE"].ToString(),
                            description = dr["DES_CONTRATANTE"] == DBNull.Value ? string.Empty : dr["DES_CONTRATANTE"].ToString()
                        };
                        response.dataCotizacion.channel = dr["COD_BROKER"] == DBNull.Value ? string.Empty : dr["COD_BROKER"].ToString(); //
                        response.dataCotizacion.policyInitDate = dr["INI_VIG"] == DBNull.Value ? string.Empty : Convert.ToDateTime(dr["INI_VIG"]).ToString("yyyy-MM-ddThh:mm:ss.fffZ");//
                        response.dataCotizacion.policyEndDate = dr["FIN_VIG"] == DBNull.Value ? string.Empty : Convert.ToDateTime(dr["FIN_VIG"]).ToString("yyyy-MM-ddThh:mm:ss.fffZ");//
                        response.dataCotizacion.paymentFrecuency = dr["COD_FRECUENCIA"] == DBNull.Value ? 0 : Convert.ToInt32(dr["COD_FRECUENCIA"].ToString());//
                        response.dataCotizacion.paymentFrecuency = cantidadMeses(response.dataCotizacion.paymentFrecuency, response.nbranch);
                        response.dataCotizacion.billingType = dr["COD_FACT"] == DBNull.Value ? string.Empty : dr["COD_FACT"].ToString();//
                        response.dataCotizacion.policyType = dr["COD_POL_TYPE"] == DBNull.Value ? string.Empty : dr["COD_POL_TYPE"].ToString(); //
                        response.dataCotizacion.colocationType = dr["COD_COLOCATION"] == DBNull.Value ? string.Empty : dr["COD_COLOCATION"].ToString(); //
                        response.dataCotizacion.profile = dr["COD_PROFILE"] == DBNull.Value ? string.Empty : dr["COD_PROFILE"].ToString();//
                        response.dataCotizacion.profile = getProfileRepeat(response.nbranch.ToString(), response.dataCotizacion.profile);
                        response.dataCotizacion.insuredQuantity = dr["NUM_TRABAJADORES"] == DBNull.Value ? 0 : Convert.ToInt32(dr["NUM_TRABAJADORES"].ToString());//
                        response.dataCotizacion.activity = dr["COD_ACT_TEC"] == DBNull.Value ? string.Empty : dr["COD_ACT_TEC"].ToString(); //
                        response.dataCotizacion.location = dr["COD_LOCATION"] == DBNull.Value ? string.Empty : dr["COD_LOCATION"].ToString();  //
                        response.dataCotizacion.module = dr["COD_PLAN"] == DBNull.Value ? string.Empty : dr["COD_PLAN"].ToString(); //
                        response.dataCotizacion.hours = dr["HOURS"] == DBNull.Value ? 0 : Convert.ToInt32(dr["HOURS"].ToString()); //
                        response.dataCotizacion.scope = dr["COD_ALCANCE"] == DBNull.Value ? string.Empty : dr["COD_ALCANCE"].ToString(); //
                        response.dataCotizacion.workers = ""; //
                        response.dataCotizacion.userCode = dr["COD_USUARIO"] == DBNull.Value ? string.Empty : dr["COD_USUARIO"].ToString();
                        response.dataCotizacion.userName = dr["DES_USUARIO"] == DBNull.Value ? string.Empty : dr["DES_USUARIO"].ToString();
                        response.dataCotizacion.retroactivityDate = dr["DAY_ALLOWED"] == DBNull.Value ? string.Empty : Convert.ToDateTime(dr["DAY_ALLOWED"]).ToString("yyyy-MM-ddThh:mm:ss.fffZ");
                        response.dataCotizacion.comments = dr["COMENTARIO"] == DBNull.Value ? string.Empty : dr["COMENTARIO"].ToString();
                        response.dataCotizacion.isMine = dr["MINA"] == DBNull.Value ? false : dr["MINA"].ToString() == "1" ? true : false;
                        response.dataCotizacion.igvRate = new QuotationDA().GetIGVGraph(response, dr["PRODUCTO"].ToString(), dr["COD_PROFILE"].ToString());
                        response.dataCotizacion.proposedCommercialRate = new ProposedCommision()
                        {
                            currency = dr["DES_CURRENCY"] == DBNull.Value ? string.Empty : dr["DES_CURRENCY"].ToString(),
                            value = 0
                        };

                        response.dataCotizacion.proposedCommission = new ProposedCommision()
                        {
                            currency = dr["DES_CURRENCY"] == DBNull.Value ? string.Empty : dr["DES_CURRENCY"].ToString(),
                            value = dr["COMISION_PRO"] == DBNull.Value ? 0 : Convert.ToDouble(dr["COMISION_PRO"].ToString())
                        };

                        response.dataCotizacion.tariffId = dr["ID_TARIFARIO"] == DBNull.Value ? string.Empty : dr["ID_TARIFARIO"].ToString();
                        response.dataCotizacion.tariffVersion = dr["VERSION_TARIFARIO"] == DBNull.Value ? 0 : Convert.ToInt32(dr["VERSION_TARIFARIO"].ToString());
                        response.dataCotizacion.detailId = dr["ID_DETAIL"] == DBNull.Value ? string.Empty : dr["ID_DETAIL"].ToString();
                        response.dataCotizacion.pensionAmount = dr["NMONTO_PENSION"] == DBNull.Value ? 0 : Convert.ToInt32(dr["NMONTO_PENSION"].ToString()); //AVS - RENTAS
                        response.dataCotizacion.dataFrame = dr["VALIDA_TRAMA"] == DBNull.Value ? true : dr["VALIDA_TRAMA"].ToString() == "1" ? true : false;
                        response.dataCotizacion.quotationType = dr["TIPO_COTIZACION"] == DBNull.Value ? string.Empty : dr["TIPO_COTIZACION"].ToString();
                        response.dataCotizacion.requestIdLastRenovation = dr["NID_PROC_LAST"] == DBNull.Value ? string.Empty : dr["NID_PROC_LAST"].ToString();
                        response.dataCotizacion.version = null;

                        response.dataCotizacion.coverages = new List<CoverageTariff>();
                        response.dataCotizacion.assistances = new List<string>();
                        response.dataCotizacion.benefits = new List<string>();
                        response.dataCotizacion.surcharges = new List<string>();
                        response.dataCotizacion.additionalServices = new List<AdditionalServiceTariff>();
                        response.dataCotizacion.files = new List<string>();
                    }

                    dr.NextResult();

                    var aseguradoList = new QuotationDA().GetTramaAsegurado(response.dataCotizacion.requestId);

                    while (dr.Read())
                    {
                        if (aseguradoList.Count > 0 && !validarAforo(response.nbranch.ToString(), response.dataCotizacion.profile, response.stransac)) //AVS - RENTAS
                        {
                            var det = new insuredGraph();
                            det.role = dr["ROLE"] == DBNull.Value ? "" : Convert.ToString(dr["ROLE"].ToString());
                            det.name = dr["NAME"] == DBNull.Value ? "" : Convert.ToString(dr["NAME"].ToString());
                            det.surname1 = dr["SURNAME1"] == DBNull.Value ? "" : Convert.ToString(dr["SURNAME1"].ToString());
                            det.surname2 = dr["SURNAME2"] == DBNull.Value ? "" : Convert.ToString(dr["SURNAME2"].ToString());
                            det.documentType = dr["DOCUMENTTYPE"] == DBNull.Value ? "" : Convert.ToString(dr["DOCUMENTTYPE"].ToString());
                            det.documentNumber = dr["DOCUMENTNUMBER"] == DBNull.Value ? "" : Convert.ToString(dr["DOCUMENTNUMBER"].ToString());
                            det.birthDate = dr["BIRTHDATE"] == DBNull.Value ? new DateTime().ToString("yyyy-MM-ddThh:mm:ss.fffZ") : Convert.ToDateTime(dr["BIRTHDATE"].ToString()).ToString("yyyy-MM-ddThh:mm:ss.fffZ");
                            det.gender = dr["GENDER"] == DBNull.Value ? "" : Convert.ToString(dr["GENDER"].ToString());
                            det.salary = dr["SALARY"] == DBNull.Value ? 0 : Convert.ToDouble(dr["SALARY"].ToString());
                            det.workerType = new WorkerType()
                            {
                                code_core = dr["CODE_CORE"] == DBNull.Value ? "0" : Convert.ToString(dr["CODE_CORE"].ToString()),
                                description = dr["DESCRIPTION"] == DBNull.Value ? "" : Convert.ToString(dr["DESCRIPTION"].ToString())

                            };
                            det.status = dr["STATUS"] == DBNull.Value ? "" : Convert.ToString(dr["STATUS"].ToString());
                            det.countryOfBirth = dr["COUNTRYOFBITH"] == DBNull.Value ? "Peru" : Convert.ToString(dr["COUNTRYOFBITH"].ToString());
                            det.documentType2 = dr["DOCUMENTTYPE2"] == DBNull.Value ? "" : Convert.ToString(dr["DOCUMENTTYPE2"].ToString());
                            det.documentNumber2 = dr["DOCUMENTNUMBER2"] == DBNull.Value ? "" : Convert.ToString(dr["DOCUMENTNUMBER2"].ToString());
                            det.relation = dr["RELATION"] == DBNull.Value ? "" : Convert.ToString(dr["RELATION"].ToString());
                            det.entryGrade = dr["ENTRYGRADE"] == DBNull.Value ? "" : Convert.ToString(dr["ENTRYGRADE"].ToString());

                            response.aseguradosList.Add(det);
                        }
                        else
                        {
                            aseguradoList = new List<insuredGraph>();
                        }
                    }

                    dr.NextResult();

                    while (dr.Read())
                    {
                        var item = new CoverageTariff()
                        {
                            id = dr["NCOVERGEN"].ToString(),
                            capital = Convert.ToDouble(dr["NCAPITAL"].ToString())
                        };
                        response.dataCotizacion.coverages.Add(item);
                    }

                    dr.NextResult();

                    while (dr.Read())
                    {
                        if (dr["NCOD_ASSISTANCE"] != DBNull.Value)
                        {
                            response.dataCotizacion.assistances.Add(dr["NCOD_ASSISTANCE"].ToString());
                        }
                    }

                    dr.NextResult();

                    while (dr.Read())
                    {
                        if (dr["NCOD_BENEFIT"] != DBNull.Value)
                        {
                            response.dataCotizacion.benefits.Add(dr["NCOD_BENEFIT"].ToString());
                        }
                    }

                    dr.NextResult();

                    while (dr.Read())
                    {
                        if (dr["NCOD_SURCHARGE"] != DBNull.Value)
                        {
                            response.dataCotizacion.surcharges.Add(dr["NCOD_SURCHARGE"].ToString());
                        }
                    }

                    dr.NextResult();

                    while (dr.Read())
                    {
                        var item = new AdditionalServiceTariff()
                        {
                            code = dr["NCOD_ADDITIONAL_SERVICES"].ToString(),
                            hours = Convert.ToInt32(dr["NHOUR"].ToString())
                        };
                        response.dataCotizacion.additionalServices.Add(item);
                    }

                    response.dataCotizacion.type = validateQuotationPM(response, aseguradoList, 0);
                    response.dataCotizacion.processType = validateQuotationPM(response, aseguradoList, 1);
                    response.dataCotizacion.insuredQuantity = aseguradoList.Count > 0 ? aseguradoList.Count : response.dataCotizacion.insuredQuantity; //AVS - RENTAS
                }
            }
            catch (Exception ex)
            {
                LogControl.save("ReadInfoQuotationGEN", ex.ToString(), "3");
            }
            finally
            {
                if (dr != null)
                    dr.Close();
            }

            return await Task.FromResult(response);
        }

        public string validateQuotationPM(genericTechnicalGraph data, List<insuredGraph> aseguradoList, int type)
        {
            string value = string.Empty;

            // response.flagMatriz == 0 && aseguradoList.Count > 0 ? response.stransac == "" : response.dataCotizacion.type;
            if (type == 0)
            {
                value = data.dataCotizacion.type;

                // if (data.flagMatriz == 1 && aseguradoList.Count == 0)
                if (!validarAforo(data.nbranch.ToString(), data.dataCotizacion.profile, data.stransac) && aseguradoList.Count == 0)
                {
                    if (data.stransac == "EM")
                    {
                        value = "MATRIX_EMISSION";

                    }

                    if (data.stransac == "RE")
                    {
                        value = "MATRIX_RENOVATION";
                    }
                }
            }

            if (type == 1)
            {
                value = data.dataCotizacion.processType;

                // if (data.flagMatriz == 1 && aseguradoList.Count == 0)
                if (!validarAforo(data.nbranch.ToString(), data.dataCotizacion.profile, data.stransac) && aseguradoList.Count == 0)
                {
                    value = "MP_" + value;
                }

            }

            return value;
        }

        public bool validarAforo(string ramo, string perfil, string stransac)
        {
            var flag = false;
            if (stransac == "EM")
            {
                var perfilAforo = ELog.obtainConfig("aforo" + ramo);
                flag = perfilAforo == perfil ? true : false;
            }

            return flag;
        }

        public async Task<ResponseGraph> getSegmentsGraph(string nbranch)
        {
            var response = new ResponseGraph() { codError = 0 };

            try
            {
                // Se setea la url del servicio
                var URI_SendNotification = ELog.obtainConfig("urlGraphqlTarifario" + nbranch);

                // Se crea objeto Graphql
                var query = new GraphQLRequest
                {
                    Query = @"query ListarSegmentosPD($Limit: Int,$NextToken: String) {
                                  entities(type: ""Segment"", nextToken: $NextToken, limit: $Limit) {
                                    items {
                                      ...SegmentHeader
                                    }
                                    nextToken
                                  }
                            }

                            fragment SegmentHeader on Segment {
                                _id
                                policyType {
                                    id
                                    description
                                }
                                collocationType {
                                    id
                                      description
                                }
                                profile {
                                    id
                                    description
                                }
                                billingType {
                                    id
                                    description
                                }
                            }",
                    Variables = new { Limit = 99999999 }
                };

                // Objeto Graphql se pasa a json 
                var request = JsonConvert.SerializeObject(query);

                // Se invoca a servicio Graphql
                var json = await new WebServiceUtil().invocarServicioGraphql(request, URI_SendNotification, "Tarifario", nbranch);
                response = JsonConvert.DeserializeObject<ResponseGraph>(json, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                if (response.data == null || (response.data.entities == null && response.errors == null))
                {
                    response.codError = 1;
                    response.message = "Hubo un error al consultar el servicio.";
                }
                else
                {
                    if (response.errors != null)
                    {
                        response.codError = 1;

                        foreach (var item in response.errors)
                        {
                            response.message = response.message + " " + item.message;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.codError = 1;
                response.message = ex.ToString();
                LogControl.save("getSegmentsGraph", ex.ToString(), "3");
            }

            return await Task.FromResult(response);
        }

        public async Task<ResponseGraph> getSegmentGraph(string segmentId, string nbranch)
        {
            var response = new ResponseGraph() { codError = 0 };

            var segment = new SegmentGraph()
            {
                id = segmentId
            };

            try
            {
                // Se setea la url del servicio
                var URI_SendNotification = ELog.obtainConfig("urlGraphqlTarifario" + nbranch);

                // Se crea objeto Graphql
                var query = new GraphQLRequest
                {
                    Query = @"query ObtenerSegmentoPD($id: ID!) {
                              entity(type: ""Segment"", id: $id) {
                                ...FullSegment
                                            }
                            }

                            fragment FullSegment on Segment {
                                        _id
                                modules {
                                    ...FullModule
                                }
                                rules {
                                    ...FullRule
                                }
                            }

                            fragment FullCoverage on Coverage {
                                id
                                code
                                status
                                description
                                limit
                                hours
                                required
                                capitalCovered
                                capital {
                                    min
                                    max
                                    base
                                }
                                stayAge {
                                    min
                                    max
                                    base
                                }
                                entryAge {
                                    min
                                    max
                                  base
                                }
                            }

                            fragment FullBenefit on Benefit {
                                id
                                code
                                description
                                coverages {
                                    id
                                    code
                                    description
                                }
                            }

                            fragment FullAssistance on Assistance {
                                id
                                code
                                description
                                provider {
                                    code
                                    description
                                }
                                document
                            }

                            fragment FullModule on SegmentModule {
                                id
                                description
                                benefits {
                                    ...FullBenefit
                                }
                                coverages {
                                    ...FullCoverage
                                }
                                assistances {
                                    ...FullAssistance
                                }
                            }

                            fragment FullRule on Rule {
                                id
                                status
                                definition {
                                    _id
                                    code
                                    order
                                    type
                                    description
                                }
                                ... on ConditionalValueRule {
                                    condition
                                    value
                                }
                                ... on NumericRangeRule {
                                    range {
                                        min
                                        max
                                        type
                                    }
                                }
                                ... on ConditionalMultipleValueRule {
                                    condition
                                    values
                                }
                                ... on LogicRule {
                                    active
                                }
                                ... on MultipleSelectionRule {
                                    values
                                    inclusion
                                }
                        } ",
                    Variables = new { id = segmentId }
                };

                // Objeto Graphql se pasa a json 
                var request = JsonConvert.SerializeObject(query);

                // Se invoca a servicio Graphql
                var json = await new WebServiceUtil().invocarServicioGraphql(request, URI_SendNotification, "Tarifario", nbranch);
                response = JsonConvert.DeserializeObject<ResponseGraph>(json, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                if (response.data == null || (response.data.entity == null && response.errors == null))
                {
                    response.codError = 1;
                    response.message = "Hubo un error al consultar el servicio.";
                }
                else
                {
                    if (response.errors != null)
                    {
                        response.codError = 1;

                        foreach (var item in response.errors)
                        {
                            response.message = response.message + " " + item.message;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.codError = 1;
                response.message = ex.ToString();
                LogControl.save("getSegmentGraph", ex.ToString(), "3");
            }

            return await Task.FromResult(response);
        }

        public async Task<ResponseGraph> getTariffGraph(TariffGraph data)
        {
            var response = new ResponseGraph() { codError = 0 };

            try
            {
                // Se setea la url del servicio
                var URI_SendNotification = ELog.obtainConfig("urlGraphqlTarifario" + data.nbranch);

                var query = new GraphQLRequest();

                if (new string[] { ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch") }.Contains(data.nbranch))
                {
                    query = generateTariffQueryGraphApVg(data);
                }

                // Objeto Graphql se pasa a json 
                var request = JsonConvert.SerializeObject(query);

                // Se invoca a servicio Graphql
                var json = await new WebServiceUtil().invocarServicioGraphql(request, URI_SendNotification, "Tarifario", data.nbranch);
                response = JsonConvert.DeserializeObject<ResponseGraph>(json, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                if (response.data == null || (response.data.tariffQuery == null && response.errors == null))
                {
                    response.codError = 1;
                    response.message = "Hubo un error al consultar el servicio.";
                }
                else
                {
                    if (response.errors != null)
                    {
                        response.codError = 1;

                        foreach (var item in response.errors)
                        {
                            response.message = response.message + " " + item.message;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.codError = 1;
                response.message = ex.ToString();
                LogControl.save("getMiniTariffGraph", ex.ToString(), "3");
            }

            return await Task.FromResult(response);
        }

        public async Task<ResponseGraph> getMiniTariffGraph(TariffGraph data, validaTramaVM objValida)
        {
            var response = new ResponseGraph() { codError = 0 };

            if (objValida != null)
            {
                LogControl.save(objValida.codProceso, "getMiniTariffGraph Ini: " + JsonConvert.SerializeObject(data, Formatting.None), "2", objValida.codAplicacion);
            }

            try
            {
                // Se setea la url del servicio
                var URI_SendNotification = ELog.obtainConfig("urlGraphqlTarifario" + data.nbranch);

                var query = new GraphQLRequest();

                if (new string[] { ELog.obtainConfig("accidentesBranch") }.Contains(data.nbranch))
                {
                    query = generateQueryGraphAp(data);
                }
                else if (new string[] { ELog.obtainConfig("vidaGrupoBranch") }.Contains(data.nbranch))
                {
                    query = generateQueryGraphVg(data);
                }
                else if (new string[] { ELog.obtainConfig("vidaIndividualBranch") }.Contains(data.nbranch))
                {
                    query = generateQueryGraphDes(data);
                }

                // Objeto Graphql se pasa a json 
                var request = JsonConvert.SerializeObject(query);

                if (objValida != null)
                {
                    LogControl.save(objValida.codProceso, "getMiniTariffGraph request: " + request, "2", objValida.codAplicacion);
                }

                // Se invoca a servicio Graphql
                var json = await new WebServiceUtil().invocarServicioGraphql(request, URI_SendNotification, "Tarifario", data.nbranch);
                response = JsonConvert.DeserializeObject<ResponseGraph>(json, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                if (response.data == null || (response.data.miniTariffMatrix == null && response.errors == null))
                {
                    response.codError = 1;
                    response.message = "Hubo un error al consultar el servicio.";
                }
                else
                {
                    if (response.errors != null)
                    {
                        response.codError = 1;

                        foreach (var item in response.errors)
                        {
                            response.message = response.message + " " + item.message;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.codError = 1;
                response.message = ex.ToString();
                LogControl.save("getMiniTariffGraph", ex.ToString(), "3");
            }

            if (objValida != null)
            {
                LogControl.save(objValida.codProceso, "getMiniTariffGraph Fin: " + JsonConvert.SerializeObject(response, Formatting.None), "2", objValida.codAplicacion);
            }

            return await Task.FromResult(response);
        }

        public async Task<ResponseGraphDes> getMiniTariffGraphDes(TariffGraph data)
        {
            var response = new ResponseGraphDes() { codError = 0 };

            try
            {
                // Se setea la url del servicio
                var URI_SendNotification = ELog.obtainConfig("urlGraphqlTarifario" + data.nbranch);

                var query = new GraphQLRequest();

                if (new string[] { ELog.obtainConfig("accidentesBranch") }.Contains(data.nbranch))
                {
                    query = generateQueryGraphAp(data);
                }
                else if (new string[] { ELog.obtainConfig("vidaGrupoBranch") }.Contains(data.nbranch))
                {
                    query = generateQueryGraphVg(data);
                }
                else if (new string[] { ELog.obtainConfig("vidaIndividualBranch") }.Contains(data.nbranch))
                {
                    query = generateQueryGraphDes(data);
                }

                // Objeto Graphql se pasa a json 
                var request = JsonConvert.SerializeObject(query);

                // Se invoca a servicio Graphql
                var json = await new WebServiceUtil().invocarServicioGraphql(request, URI_SendNotification, "Tarifario", data.nbranch);
                response = JsonConvert.DeserializeObject<ResponseGraphDes>(json, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                if (response.data == null || (response.data.miniTariffMatrix == null && response.errors == null))
                {
                    response.codError = 1;
                    response.message = "Hubo un error al consultar el servicio.";
                }
                else
                {
                    if (response.errors != null)
                    {
                        response.codError = 1;

                        foreach (var item in response.errors)
                        {
                            response.message = response.message + " " + item.message;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.codError = 1;
                response.message = ex.ToString();
                LogControl.save("getMiniTariffGraphDes", ex.ToString(), "3");
            }

            return await Task.FromResult(response);
        }

        public GraphQLRequest generateTariffQueryGraphApVg(TariffGraph data)
        {
            // Se crea objeto Graphql
            var response = new GraphQLRequest()
            {
                Query = @"query TariffQuery($channel: ID!, $currency: ID!, $policyType: ID!, $collocationType: ID!, $profile: ID!, $billingType: ID!) {
                          tariffQuery(channel: $channel, currency: $currency, policyType: $policyType, collocationType: $collocationType, profile: $profile, billingType: $billingType) {
                            id
                            version
                            description
                            dataFrame
                            quotationType
                          }
                        }",
                Variables = new
                {
                    channel = data.channel,
                    currency = data.currency,
                    policyType = data.policyType,
                    collocationType = data.collocationType,
                    profile = data.profile,
                    billingType = data.billingType
                }
            };

            return response;
        }

        public GraphQLRequest generateQueryGraphVg(TariffGraph data)
        {
            // Se crea objeto Graphql
            var response = new GraphQLRequest()
            {
                Query = @"query miniTariffMatrix(
                        $id: ID!
                        $version: Int!
                                  $channel: ID!
                                  $currency: ID!
                                  $policyType: ID!
                                  $collocationType: ID!
                                  $profile: ID!
                        $billingType: ID! ) 
                        {
                                  miniTariffMatrix(
                            id: $id
                                    channel: $channel
                                    currency: $currency
                                    policyType: $policyType
                                    collocationType: $collocationType
                                    profile: $profile
                                    billingType: $billingType
                            version: $version ) 
                            {
                                    id
                                    version
                                    description
                                    startDate
                                    endDate
                                    currency {
                                      id
                                      code
                                      symbol
                                      description
                                    }
                                    type
                                    sector
                                    segment {
                                      ...FullSegment
                                    }
                                quotationType
                                dataFrame
                                  }
                                }

                                fragment FullSegment on TariffMatrixSegment {
                                  modules {
                                    ...FullModule
                                  }
                                  rules {
                                    ...FullRule
                                  }
                                  surcharges {
                                    ...FullSurcharges
                                  }
                                  exclusionLinks {
                                    ...FullExclusion
                                  }
                                }

                                fragment FullCoverage on Coverage {
                                  id
                                  code
                                  status
                                  description
                                  limit
                                  hours
                                  required
                                  capitalCovered
                                  capital {
                                    min
                                    max
                                    base
                                type
                                  }
                                  stayAge {
                                    min
                                    max
                                    base
                                  }
                                  entryAge {
                                    min
                                    max
                                    base
                                  }
                                  items {
                                    id
                                    description
                                    capitalCovered
                                  }
                                  lackPeriod
                                  deductible
                                  copayment
                                  maxAccumulation
                                  comment
                                }

                                fragment FullBenefit on Benefit {
                                  id
                                  code
                                  description
                                  value {
                                    type
                                    amount
                                  }
                                  capitalCovered
                                  coverages {
                                    id
                                    code
                                    description
                                  }
                                  status
                            exclusiveStudentRent
                            studentRentBenefit
                                }

                                fragment FullAssistance on Assistance {
                                  id
                                  code
                                  description
                                  value {
                                    type
                                    amount
                                  }
                                  provider {
                                    code
                                    description
                                  }
                                  document
                            status
                            igv
                                }

                                fragment FullAdditionalServices on AdditionalService {
                                    id
                                    description
                                    value
                                    state
                                    igv
                                }

                                fragment FullModule on SegmentModule {
                                  id
                                  description
                                  benefits {
                                    ...FullBenefit
                                  }
                                  coverages {
                                    ...FullCoverage
                                  }
                                  assistances {
                                    ...FullAssistance
                                  }
                            additionalServices {
                            ...FullAdditionalServices
                            }
                            planType
                                }

                                fragment FullRule on Rule {
                                  id
                                  status
                                  definition {
                                    _id
                                    code
                                    order
                                    type
                                    description
                                  }
                                  ... on ConditionalValueRule {
                                    condition
                                    value
                                  }
                                  ... on NumericRangeRule {
                                    range {
                                      min
                                      max
                                      type
                                    }
                                  }
                                  ... on ConditionalMultipleValueRule {
                                    condition
                                    values
                                  }
                                  ... on LogicRule {
                                    active
                                  }
                                  ... on MultipleSelectionRule {
                                    values
                                    inclusion
                                  }
                                }

                                fragment FullSurcharges on Surcharge {
                                  id
                                  code
                                  description
                                  optional
                                  value {
                                    type
                                    amount
                                  }
                                  status
                            igv
                                }

                                fragment FullExclusion on ExclusionLink {
                                  status
                                  exclusion {
                                    _id
                                    code
                                    description
                                    type
                                  }
                                }",
                Variables = new
                {
                    id = data.idTariff,
                    version = data.versionTariff,
                    channel = data.channel,
                    currency = data.currency,
                    policyType = data.policyType,
                    collocationType = data.collocationType,
                    profile = data.profile,
                    billingType = data.billingType
                }
            };

            return response;
        }

        public GraphQLRequest generateQueryGraphAp(TariffGraph data)
        {
            // Se crea objeto Graphql
            var response = new GraphQLRequest()
            {
                Query = @"query miniTariffMatrix(
                                $id: ID!
                                $version: Int!
                                $channel: ID!
                                $currency: ID!
                                $policyType: ID!
                                $collocationType: ID!
                                $profile: ID!
                                $billingType: ID! ) 
                                {
                                    miniTariffMatrix(
                                    id: $id
                                    channel: $channel
                                    currency: $currency
                                    policyType: $policyType
                                    collocationType: $collocationType
                                    profile: $profile
                                    billingType: $billingType
                                    version: $version ) 
                                    {
                                        id
                                        version
                                        description
                                        startDate
                                        endDate
                                        currency {
                                            id
                                            code
                                            symbol
                                            description
                                        }
                                        type
                                        sector
                                        segment {
                                            ...FullSegment
                                        }
                                        quotationType
                                        dataFrame
                                    }
                                }

                                fragment FullSegment on TariffMatrixSegment {
                                    modules {
                                    ...FullModule
                                    }
                                    rules {
                                    ...FullRule
                                    }
                                    surcharges {
                                    ...FullSurcharges
                                    }
                                    exclusionLinks {
                                    ...FullExclusion
                                    }
                                }

                                fragment FullCoverage on Coverage {
                                    id
                                    code
                                    status
                                    description
                                    limit
                                    hours
                                    required
                                    capitalCovered
                                    capital {
                                        min
                                        max
                                        base
                                        type
                                    }
                                    stayAge {
                                        min
                                        max
                                        base
                                    }
                                    entryAge {
                                        min
                                        max
                                        base
                                    }
                                    items {
                                        id
                                        description
                                        capitalCovered
                                    }
                                    lackPeriod
                                    deductible
                                    copayment
                                    maxAccumulation
                                    comment
                                }

                                fragment FullBenefit on Benefit {
                                    id
                                    code
                                    description
                                    value {
                                        type
                                        amount
                                    }
                                    capitalCovered
                                    coverages {
                                        id
                                        code
                                        description
                                    }
                                    status
                                }

                                fragment FullAssistance on Assistance {
                                    id
                                    code
                                    description
                                    value {
                                        type
                                        amount
                                    }
                                    provider {
                                        code
                                        description
                                    }
                                    document
                                    status
                                }

                                fragment FullAdditionalServices on AdditionalService {
                                    id
                                    description
                                    value
                                    state
                                    igv
                                }

                                fragment FullModule on SegmentModule {
                                    id
                                    description
                                    benefits {
                                    ...FullBenefit
                                    }
                                    coverages {
                                    ...FullCoverage
                                    }
                                    assistances {
                                    ...FullAssistance
                                    }
                                    additionalServices {
                                    ...FullAdditionalServices
                                    }
                                    planType
                                }

                                fragment FullRule on Rule {
                                    id
                                    status
                                    definition {
                                        _id
                                        code
                                        order
                                        type
                                        description
                                    }
                                    ... on ConditionalValueRule {
                                        condition
                                        value
                                    }
                                    ... on NumericRangeRule {
                                        range {
                                            min
                                            max
                                            type
                                        }
                                    }
                                    ... on ConditionalMultipleValueRule {
                                        condition
                                        values
                                    }
                                    ... on LogicRule {
                                        active
                                    }
                                    ... on MultipleSelectionRule {
                                        values
                                        inclusion
                                    }
                                }

                                fragment FullSurcharges on Surcharge {
                                    id
                                    code
                                    description
                                    optional
                                    value {
                                        type
                                        amount
                                    }
                                    status
                                }

                                fragment FullExclusion on ExclusionLink {
                                    status
                                    exclusion {
                                        _id
                                        code
                                        description
                                        type
                                    }
                            }",
                Variables = new
                {
                    id = data.idTariff,
                    version = data.versionTariff,
                    channel = data.channel,
                    currency = data.currency,
                    policyType = data.policyType,
                    collocationType = data.collocationType,
                    profile = data.profile,
                    billingType = data.billingType
                }
            };

            return response;
        }

        public GraphQLRequest generateQueryGraphDes(TariffGraph data)
        {
            // Se crea objeto Graphql
            var response = new GraphQLRequest()
            {
                Query = @"query(
                                  $channel: ID!
                                  $currency: ID!
                                  $policyType: ID!
                                  $renewalType: ID!
                                  $creditType: ID!
                                  $billingType: ID!
                                ) {
                                  miniTariffMatrix(
                                    channel: $channel
                                    currency: $currency
                                    policyType: $policyType
                                    renewalType: $renewalType
                                    creditType: $creditType
                                    billingType: $billingType
                                  ) {
                                    id
                                    version
                                    description
                                    startDate
                                    endDate
                                    type
                                    sector
                                    segment{
                                      ...FullSegment
                                    }
                                  }
                                }

                                fragment FullSegment on Segment {
                                  policyType{
                                    id
                                    description
                                  }
                                  renewalType{
                                    id
                                    description
                                  }
                                  modules {
                                    ...FullModule
                                  }
                                  rules {
                                    ...FullRule
                                  }
                                  exclusionLinks{
                                    ...FullExclusion
                                  }
                                    surcharges {
                                        id
                                        code
                                        description
                                        status
                                        optional
                                        value {
                                            type
                                            amount
                                        }
                                    } 
                                }

                                fragment FullCoverage on Coverage {
                                  id
                                  code
                                  type
                                  status
                                  description
                                  limit
                                  hours
                                  capitalCovered
                                  optional
                                  capital {
                                    min
                                    max
                                    base
                                  }
                                  stayAge {
                                    min
                                    max
                                    base
                                  }
                                  entryAge {
                                    min
                                    max
                                    base
                                  }
                                }

                                fragment FullBenefit on Benefit {
                                  id
                                  code
                                  optional
                                  status
                                  description
                                  capitalCovered
                                  value{
                                    type
                                    amount
                                  }
                                }

                                fragment FullAssistance on Assistance {
                                  id
                                  code
                                  optional
                                  status
                                  description
                                  value{
                                    type
                                    amount
                                  }
                                  provider {
                                    code
                                    description
                                  }
                                  document
                                }

                                fragment FullModule on Module {
                                  id
                                  description
                                  benefits {
                                    ...FullBenefit
                                  }
                                  coverages {
                                    ...FullCoverage
                                  }
                                  assistances {
                                    ...FullAssistance
                                  }
                                }

                                fragment FullRule on Rule {
                                  id
                                  status
                                  definition {
                                    _id
                                    code
                                    order
                                    type
                                    description
                                  }
                                  content{
          logicCondition{
            active
          }
          numericInterval{
            interval{
              min
              max
              type
            }
          }
          intervalTable{
            row{
              min
              max
              type
            }
            column{
              min
              max
              type
            }
          }
          conditionalValue{
            condition
            value
          }
          conditionalOptions{
            condition
            options
          }
        }  
                                }
                fragment FullExclusion on ExclusionLink{
                                  status
                                  exclusion{
                                    _id
                                    code
                                    description
                                    type
                                  }
                                }   
",
                Variables = new
                {
                    channel = data.channel,
                    currency = data.currency,
                    policyType = data.policyType,
                    renewalType = data.renewalType,
                    creditType = data.creditType,
                    billingType = data.billingType
                }
            };

            return response;
        }

        public async Task<ResponseGraph> adjuntarAsegurados(string URI_SendNotification, string typeUrl, string quotationNumber, string path, string branch)
        {
            //var contentType = MimeMapping.GetMimeMapping(dataCotizacion.ruta + "asegurados.json");

            //foreach (var item in HttpContext.Current.Request.Files)
            //{
            // Se crea objeto Graphql
            var queryReference = new GraphQLRequest()
            {
                Query = @"mutation ($contentType: String) {
                            newReference(contentType: $contentType){
                                id,
                                url
                            }
                        }",
                Variables = new { contentType = MimeMapping.GetMimeMapping(path + "asegurados.json") }
            };

            // Objeto Graphql se pasa a json 
            var requestReference = JsonConvert.SerializeObject(queryReference);

            // Se invoca a servicio Graphql
            var jsonReference = await new WebServiceUtil().invocarServicioGraphql(requestReference, URI_SendNotification, typeUrl, branch);
            var responseReference = JsonConvert.DeserializeObject<ResponseGraph>(jsonReference, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            // Guarda log en archivo txt
            LogControl.save("adjuntarAsegurados",
                "Mutación newReference: " + URI_SendNotification
                + Environment.NewLine
                + "Response:"
                + Environment.NewLine
                + responseReference.data.newReference, "1");

            // Guarda log en tbl
            new QuotationDA().InsertLog(Convert.ToInt64(quotationNumber), "03 - Se prepara info de asegurados", URI_SendNotification,
                JsonConvert.SerializeObject(queryReference),
                JsonConvert.SerializeObject(responseReference.data));

            return await Task.FromResult(responseReference);
        }

        public async Task<ResponseGraph> adjuntarAseguradosDes(string URI_SendNotification, string typeUrl, string path, string branch)
        {
            //var contentType = MimeMapping.GetMimeMapping(dataCotizacion.ruta + "asegurados.json");

            //foreach (var item in HttpContext.Current.Request.Files)
            //{
            // Se crea objeto Graphql
            var queryReference = new GraphQLRequest()
            {
                Query = @"mutation ($contentType: String) {
                            newReference(contentType: $contentType){
                                id,
                                url
                            }
                        }",
                Variables = new { contentType = MimeMapping.GetMimeMapping(path + "asegurados.json") }
            };

            // Objeto Graphql se pasa a json 
            var requestReference = JsonConvert.SerializeObject(queryReference);

            // Se invoca a servicio Graphql
            var jsonReference = await new WebServiceUtil().invocarServicioGraphql(requestReference, URI_SendNotification, typeUrl, branch);
            var responseReference = JsonConvert.DeserializeObject<ResponseGraph>(jsonReference, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            // Guarda log en archivo txt
            LogControl.save("adjuntarAseguradosDes",
                "Mutación newReference: " + URI_SendNotification
                + Environment.NewLine
                + "Response:"
                + Environment.NewLine
                + responseReference.data.newReference, "1");
            /*
            // Guarda log en tbl
            new QuotationDA().InsertLog(Convert.ToInt64(quotationNumber), "Mutación newReference: ", URI_SendNotification,
                JsonConvert.SerializeObject(jsonReference, Formatting.None),
                JsonConvert.SerializeObject(responseReference.data, Formatting.None));
            */
            return await Task.FromResult(responseReference);
        }

        private async Task<string> generarAseguradosJson(string codProceso, List<insuredGraph> aseguradosList)
        {
            string path = null;

            if (aseguradosList.Count() > 0)
            {
                path = String.Format(ELog.obtainConfig("pathPrincipal"), codProceso.PadLeft(31, 'A')) + "\\";

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                System.IO.File.WriteAllText(path + "asegurados.json", JsonConvert.SerializeObject(aseguradosList));
            }

            return await Task.FromResult(path);
        }

        public async Task<bool> valMiniTariffGraph(TariffGraph data)
        {
            bool validate = false;

            if (new string[] { ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch") }.Contains(data.nbranch))
            {
                if (!string.IsNullOrEmpty(data.channel) &&
                !string.IsNullOrEmpty(data.policyType) &&
                !string.IsNullOrEmpty(data.profile) &&
                !string.IsNullOrEmpty(data.collocationType) &&
                !string.IsNullOrEmpty(data.billingType) &&
                !string.IsNullOrEmpty(data.currency)
                )
                {
                    validate = true;
                }
            }
            else if (new string[] { ELog.obtainConfig("vidaIndividualBranch") }.Contains(data.nbranch))
            {
                if (!string.IsNullOrEmpty(data.channel) &&
                !string.IsNullOrEmpty(data.policyType) &&
                !string.IsNullOrEmpty(data.renewalType) &&
                !string.IsNullOrEmpty(data.creditType) &&
                !string.IsNullOrEmpty(data.billingType) &&
                !string.IsNullOrEmpty(data.currency)
                )
                {
                    validate = true;
                }
            }


            return await Task.FromResult(validate);
        }

        public int cantidadMeses(int? codRenov, int nbranch = 0)
        {
            var meses = 0;
            string[][] arr;

            if (nbranch == 71)
            {
                arr = ELog.obtainConfig("cantidadMesesVCF").Split(';').Select(x => x.Split(',')).ToArray();
            }
            else
            {
                arr = ELog.obtainConfig("cantidadMeses").Split(';').Select(x => x.Split(',')).ToArray();
            }

            foreach (var item in arr)
            {
                if (item[0] == codRenov.ToString())
                {
                    meses = Convert.ToInt32(item[1]);
                }

            }

            return meses;
        }

        public int cantidadMesesDes(int? codRenov)
        {
            var meses = 0;
            var arr = ELog.obtainConfig("cantidadMesesDes").Split(';').Select(x => x.Split(',')).ToArray();
            foreach (var item in arr)
            {
                if (item[1] == codRenov.ToString())
                {
                    meses = Convert.ToInt32(item[0]);
                }

            }

            return meses;
        }

        public string codPerfil(string meses)
        {
            var codPerfil = "0";
            var arr = ELog.obtainConfig("codRenovacion").Split(';').Select(x => x.Split(',')).ToArray();
            foreach (var item in arr)
            {
                if (item[0] == meses)
                {
                    codPerfil = item[1];
                }
            }

            return codPerfil;
        }

        public async Task<RulesLogic> getRulesLogic(List<Rule> data)
        {
            var response = new RulesLogic();

            var rulesLogic = data.Where(x => x.definition.type == "GENERAL_LOGIC").ToList();

            if (rulesLogic.Count > 0)
            {
                response.flagComision = data.SingleOrDefault(x => x.definition.code == "1") != null ? data.SingleOrDefault(x => x.definition.code == "1").status == "ACTIVE" ? data.SingleOrDefault(x => x.definition.code == "1").active : false : false;
                response.flagCobertura = data.SingleOrDefault(x => x.definition.code == "2") != null ? data.SingleOrDefault(x => x.definition.code == "2").status == "ACTIVE" ? data.SingleOrDefault(x => x.definition.code == "2").active : false : false;
                response.flagAsistencia = true; // data.SingleOrDefault(x => x.id == "3") != null ? data.SingleOrDefault(x => x.id == "3").active : false;
                response.flagBeneficio = true; // data.SingleOrDefault(x => x.id == "4") != null ? data.SingleOrDefault(x => x.id == "4").active : false;
                response.flagSiniestralidad = data.SingleOrDefault(x => x.definition.code == "8") != null ? data.SingleOrDefault(x => x.definition.code == "8").status == "ACTIVE" ? data.SingleOrDefault(x => x.definition.code == "8").active : false : false;
                response.flagAlcance = data.SingleOrDefault(x => x.definition.code == "26") != null ? data.SingleOrDefault(x => x.definition.code == "26").status == "ACTIVE" ? data.SingleOrDefault(x => x.definition.code == "26").active : false : false;
                response.flagTemporalidad = data.SingleOrDefault(x => x.definition.code == "27") != null ? data.SingleOrDefault(x => x.definition.code == "27").status == "ACTIVE" ? data.SingleOrDefault(x => x.definition.code == "27").active : false : false;
                response.flagTrama = true; // data.SingleOrDefault(x => x.definition.code == "27") != null ? data.SingleOrDefault(x => x.definition.code == "27").status == "ACTIVE" ? data.SingleOrDefault(x => x.definition.code == "27").active : false : false;
                response.flagTasa = true; // data.SingleOrDefault(x => x.definition.code == "27") != null ? data.SingleOrDefault(x => x.definition.code == "27").status == "ACTIVE" ? data.SingleOrDefault(x => x.definition.code == "27").active : false : false;
                response.flagPrima = false; // data.SingleOrDefault(x => x.definition.code == "27") != null ? data.SingleOrDefault(x => x.definition.code == "27").status == "ACTIVE" ? data.SingleOrDefault(x => x.definition.code == "27").active : false : false;
            }

            return await Task.FromResult(response);

        }

        public async Task<RulesLogic> getRulesLogicDes(List<Rule> data)
        {
            var response = new RulesLogic();

            var rulesLogic = data.Where(x => x.definition.type == "GENERAL_CONDITIONS").ToList();

            if (rulesLogic.Count > 0)
            {
                response.flagComision = data.SingleOrDefault(x => x.id == "1") != null ? data.SingleOrDefault(x => x.id == "1").active : false;
                response.flagCobertura = data.SingleOrDefault(x => x.id == "2") != null ? data.SingleOrDefault(x => x.id == "2").content.logicCondition.active : false;
                response.flagAsistencia = data.SingleOrDefault(x => x.id == "3") != null ? data.SingleOrDefault(x => x.id == "3").content.logicCondition.active : false;
                response.flagBeneficio = data.SingleOrDefault(x => x.id == "4") != null ? data.SingleOrDefault(x => x.id == "4").content.logicCondition.active : false;
                response.flagSiniestralidad = data.SingleOrDefault(x => x.id == "8") != null ? data.SingleOrDefault(x => x.id == "8").content.logicCondition.active : false;
            }

            return await Task.FromResult(response);

        }

        public async Task<ResponseGraph> getReferenceURL(string nbranch, string codUrl, string origen = null)
        {
            var response = new ResponseGraph() { codError = 0 };

            try
            {
                // Se setea la url del servicio
                var URI_SendNotification = ELog.obtainConfig("urlGraphql" + origen + nbranch);

                LogControl.save("getReferenceURL", "URL: " + URI_SendNotification, "2");

                // Se crea objeto Graphql
                var queryNotification = new GraphQLRequest
                {
                    Query = @"query($id: ID!) {
                      referenceURL(id: $id)
                    }",

                    Variables = new
                    {
                        id = codUrl,
                    }
                };

                // Objeto Graphql se pasa a json 
                var request = JsonConvert.SerializeObject(queryNotification);

                LogControl.save("getReferenceURL", "Request Graphql: " + request, "2");

                // Se invoca a servicio Graphql
                var json = await new WebServiceUtil().invocarServicioGraphql(request, URI_SendNotification, origen, nbranch);
                LogControl.save("getReferenceURL", "Response: " + json, "2");
                response = JsonConvert.DeserializeObject<ResponseGraph>(json, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                if (response.data == null || (response.data.referenceURL == null && response.errors == null))
                {
                    response.codError = 1;
                    response.message = "Hubo un error al consultar el servicio.";
                }
                else
                {
                    if (response.errors != null)
                    {
                        response.codError = 1;

                        foreach (var item in response.errors)
                        {
                            response.message = response.message + " " + item.message;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.codError = 1;
                response.message = ex.ToString();
                LogControl.save("getReferenceURL", ex.ToString(), "3");
            }

            return await Task.FromResult(response);
        }

        public async Task<ResponseGraph> confirmQuote(ResponseUpdateVM data)
        {
            var response = new ResponseGraph() { codError = 0 };

            // Se setea la url del servicio
            var URI_SendNotification = ELog.obtainConfig("urlGraphqlCotizador" + data.branch);
            LogControl.save("confirmQuote", URI_SendNotification, "1");
            //ELogControl.save(this, "GraphqlDA", JsonConvert.SerializeObject(quotationRequest, Formatting.None));


            // Trae data a enviar al servicio del cotizador
            var dataQuote = new confirmQuote()
            {
                status = data.stateRequest,
                quoteId = data.id,
                errors = data.errors
            };

            var queryNotification = new GraphQLRequest
            {
                Query = @"mutation ($confirmation: ConfirmQuoteInput){
                            confirmQuote(confirmation: $confirmation)
                        }",
                Variables = new { confirmation = dataQuote }
            };

            // Objeto Graphql se pasa a json 
            var request = JsonConvert.SerializeObject(queryNotification, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            LogControl.save("confirmQuote", request, "2");

            // Se invoca a servicio Graphql
            var json = await new WebServiceUtil().invocarServicioGraphql(request, URI_SendNotification, "Cotizador", data.branch);
            LogControl.save("confirmQuote", JsonConvert.SerializeObject(json, Formatting.None), "2");

            var responseServ = JsonConvert.DeserializeObject<ResponseGraph>(json, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            new QuotationDA().InsertLog(Convert.ToInt64(data.nroCotizacion), "06 - Se llama a ConfirmQuoteInput", URI_SendNotification, request,
                                    responseServ.errors != null ? json : responseServ.data.confirmQuote);

            if (responseServ.data == null && responseServ.errors == null)
            {
                response.codError = 1;
                response.message = "Hubo un error en el enviar la cotización a técnica. Favor de comunicarse con sistemas.";
            }
            else
            {
                if (responseServ.errors != null)
                {
                    response.codError = 1;

                    foreach (var item in responseServ.errors)
                    {
                        response.message = response.message + " " + item.message;
                    }
                }
            }

            return response;
        }
    }

}