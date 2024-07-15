using Newtonsoft.Json;
using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.Entities.MapfreIntegrationModel;
using WSPlataforma.Entities.MapfreIntegrationModel.BindingModel;
using WSPlataforma.Entities.MapfreIntegrationModel.ViewModel;
using WSPlataforma.Util;

namespace WSPlataforma.DA
{
    public class MapfreIntegrationDA : ConnectionBase
    {
        WebServiceUtil WebServiceUtil = new WebServiceUtil();
        public async Task<UpdateRequestVM> UpdateRequest(UpdateRequestBM data)
        {
            var response = new UpdateRequestVM();
            response.codError = ELog.obtainConfig("codigo_OK");
            var nroItem = 0;

            try
            {
                if (data.riesgoList != null && data.riesgoList.Count() > 0)
                {
                    foreach (var item in data.riesgoList)
                    {
                        if (response.codError == ELog.obtainConfig("codigo_OK"))
                        {
                            response = actualizarCotizacion(data, item, nroItem);
                        }

                        nroItem++;
                    }

                }
                else
                {
                    if (data.codEstado == "3")
                    {
                        response = actualizarCotizacion(data, null, 1);
                    }
                    else
                    {
                        response.codError = ELog.obtainConfig("codigo_NO");
                        response.mensaje = "No ha enviado la lista de riesgo a actualizad";
                    }
                }
            }
            catch (Exception ex)
            {
                LogControl.save("UpdateRequest", ex.ToString(), "3");
            }

            return await Task.FromResult(response);
        }

        private UpdateRequestVM actualizarCotizacion(UpdateRequestBM data, RiesgoBM item, int nroItem)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            var response = new UpdateRequestVM();
            response.codError = ELog.obtainConfig("codigo_OK");
            var storeName = ProcedureName.pkg_IntegraSCTR + ".UPD_COTIZA_MPE";
            try
            {
                parameter.Add(new OracleParameter("p_scotiza_lnk", OracleDbType.Varchar2, data.nroCotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_sstatregt", OracleDbType.Int32, data.codEstado, ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_nmotivo", OracleDbType.Int32, data.codMotivo, ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_scomment", OracleDbType.Varchar2, data.comentario, ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_nproduct", OracleDbType.Int32, item != null ? Convert.ToInt32(item.codProducto) : 0, ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_nmodulec", OracleDbType.Int32, item != null ? Convert.ToInt32(item.codRiesgo) : 0, ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_ntasa_autor", OracleDbType.Decimal, item != null ? Convert.ToDecimal(item.tasaAut) : 0, ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_npremium_men_aut", OracleDbType.Decimal, item != null ? Convert.ToDecimal(item.primaMensualAut) : 0, ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_npremium_min_au", OracleDbType.Decimal, item != null ? Convert.ToDecimal(item.primaMinimaAut) : 0, ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_nsum_premiumn", OracleDbType.Decimal, data.sumPrimaNetaAut != null ? Convert.ToDecimal(data.sumPrimaNetaAut) : 0, ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_nsum_igv", OracleDbType.Decimal, data.sumPrimaNetaAut != null ? Convert.ToDecimal(data.sumIgvAut) : 0, ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_nsum_premium", OracleDbType.Decimal, data.sumPrimaNetaAut != null ? Convert.ToDecimal(data.sumPrimaAut) : 0, ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_nusercode", OracleDbType.Int32, data.codUsuario, ParameterDirection.Input));
                //OUTPUT
                OracleParameter p_result = new OracleParameter("p_result", OracleDbType.Varchar2, response.codError, ParameterDirection.Output);
                OracleParameter p_mensaje = new OracleParameter("p_mensaje", OracleDbType.Varchar2, response.mensaje, ParameterDirection.Output);

                p_result.Size = 4000;
                p_mensaje.Size = 4000;
                parameter.Add(p_result);
                parameter.Add(p_mensaje);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storeName, parameter);

                ELog.CloseConnection(odr);
                response.codError = p_mensaje.Value.ToString() == "null" ? ELog.obtainConfig("codigo_OK") : ELog.obtainConfig("codigo_NO");
                response.mensaje = p_mensaje.Value.ToString() == "null" ? "Se realizó la operación correctamente" : p_mensaje.Value.ToString();

            }
            catch (Exception ex)
            {
                response.codError = ELog.obtainConfig("codigo_NO");
                response.mensaje = "Hubo un error al procesar el registro N° " + nroItem;
                LogControl.save("actualizarCotizacion", ex.ToString(), "3");
            }

            return response;
        }

        public async Task<CancelPolicyVM> CancelPolicy(CancelPolicyBM data)
        {

            List<OracleParameter> parameter = new List<OracleParameter>();
            var response = new CancelPolicyVM();
            var storeName = ProcedureName.pkg_IntegraSCTR + ".NUL_POLICY_MPE";

            try
            {
                parameter.Add(new OracleParameter("p_nproduct", OracleDbType.Int32, ELog.obtainConfig("saludKey"), ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_spolicy_mpe", OracleDbType.Varchar2, data.nroPolizaEnlace, ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_smotivoanula", OracleDbType.Varchar2, data.motivoAnulacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_nusercode", OracleDbType.Varchar2, data.codigoUsuario, ParameterDirection.Input));

                //OUTPUT
                OracleParameter p_result = new OracleParameter("p_result", OracleDbType.Varchar2, response.codError, ParameterDirection.Output);
                OracleParameter p_mensaje = new OracleParameter("p_mensaje", OracleDbType.Varchar2, response.mensaje, ParameterDirection.Output);

                p_result.Size = 4000;
                p_mensaje.Size = 4000;
                parameter.Add(p_result);
                parameter.Add(p_mensaje);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storeName, parameter);
                ELog.CloseConnection(odr);

                response.codError = p_result.Value == DBNull.Value ? ELog.obtainConfig("codigo_NO") : p_result.Value.ToString();
                response.mensaje = p_mensaje.Value.ToString() == "null" ? "Se realizó la operación correctamente" : p_mensaje.Value.ToString();
            }
            catch (Exception ex)
            {
                LogControl.save("CancelPolicy", ex.ToString(), "3");
            }

            return await Task.FromResult(response);
        }

        public async Task<DeclararMpVM> DeclararPolizaMapfre(DeclararMpBM data)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            var response = new DeclararMpVM();
            var storeName = ProcedureName.pkg_IntegraSCTR + ".INS_CONSTANCIA_MPE";

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("p_nproduct", OracleDbType.Int32, data.nproduct, ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_spolicy_mpe", OracleDbType.Varchar2, data.spolicy_mpe, ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_sconstancia_mpe", OracleDbType.Varchar2, data.sconstancia_mpe, ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_stipmovimiento", OracleDbType.Varchar2, ELog.obtainConfig(String.Format("equiTransaccion{0}", data.stipmovimiento)), ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_snumrecibo_mpe", OracleDbType.Varchar2, data.snumrecibo_mpe, ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_nimprecibo_mpe", OracleDbType.Decimal, data.nimprecibo_mpe, ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_sefecrecibo_mpe", OracleDbType.Varchar2, data.sefecrecibo_mpe, ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_svctorecibo_mpe", OracleDbType.Varchar2, data.svctorecibo_mpe, ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_ncuotarec_mpe", OracleDbType.Varchar2, data.ncuotarec_mpe, ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_nusercode", OracleDbType.Int32, data.nusercode, ParameterDirection.Input));

                //OUTPUT
                OracleParameter p_result = new OracleParameter("p_result", OracleDbType.Varchar2, response.codError, ParameterDirection.Output);
                OracleParameter p_mensaje = new OracleParameter("p_mensaje", OracleDbType.Varchar2, response.mensaje, ParameterDirection.Output);

                p_result.Size = 4000;
                p_mensaje.Size = 4000;
                parameter.Add(p_result);
                parameter.Add(p_mensaje);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storeName, parameter);
                ELog.CloseConnection(odr);

                response.codError = p_result.Value.ToString();
                response.mensaje = p_mensaje.Value == DBNull.Value ? string.Empty : p_mensaje.Value.ToString();
            }
            catch (Exception ex)
            {
                LogControl.save("DeclararPolizaMapfre", ex.ToString(), "3");
            }

            return await Task.FromResult(response);
        }

        public async Task<PolizaMpVM> InsertaPolizaMapfre(PolizaMpBM data)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            var response = new PolizaMpVM();
            var storeName = ProcedureName.pkg_IntegraSCTR + ".INS_POLICY_MPE";

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("p_nid_cotizacion", OracleDbType.Int64, data.nid_cotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_nproduct", OracleDbType.Int32, data.nproduct, ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_ncompany_lnk", OracleDbType.Int32, data.ncompany_lnk, ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_scotiza_lnk", OracleDbType.Varchar2, data.scotiza_lnk, ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_ncodcia_mpe", OracleDbType.Int32, data.ncodcia_mpe, ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_spolicy_mpe", OracleDbType.Varchar2, data.spolicy_mpe, ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_nspto_mpe", OracleDbType.Int32, data.nspto_mpe, ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_napli_mpe", OracleDbType.Varchar2, data.napli_mpe, ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_nsptoapli_mpe", OracleDbType.Int32, data.nsptoapli_mpe, ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_sefecspto_mpe", OracleDbType.Varchar2, data.sefecspto_mpe, ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_svctospto_mpe", OracleDbType.Varchar2, data.svctospto_mpe, ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_ncodagt_mpe", OracleDbType.Int32, data.ncodagt_mpe, ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_nfraccpago_mpe", OracleDbType.Int32, data.nfraccpago_mpe, ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_sfraccpago_mpe", OracleDbType.Varchar2, data.sfraccpago_mpe, ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_nfracuotas_mpe", OracleDbType.Int32, data.nfracuotas_mpe, ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_ncodmon_mpe", OracleDbType.Int32, data.ncodmon_mpe, ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_snommon_mpe", OracleDbType.Varchar2, data.snommon_mpe, ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_nriesgo_mpe", OracleDbType.Int32, data.nriesgo_mpe, ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_scategoria_mpe", OracleDbType.Varchar2, data.scategoria_mpe, ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_ntasa_mpe", OracleDbType.Decimal, data.ntasa_mpe, ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_sconstancia_mpe", OracleDbType.Decimal, data.sconstancia_mpe, ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_snumrecibo_mpe", OracleDbType.Varchar2, data.snumrecibo_mpe, ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_nimprecibo_mpe", OracleDbType.Decimal, data.nimprecibo_mpe, ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_sefecrecibo_mpe", OracleDbType.Varchar2, data.sefecrecibo_mpe, ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_svctorecibo_mpe", OracleDbType.Varchar2, data.svctorecibo_mpe, ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_nusercode", OracleDbType.Int32, data.nusercode, ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_smesadlntdo_mpe", OracleDbType.Char, data.smesadlntdo_mpe, ParameterDirection.Input));

                //OUTPUT
                OracleParameter p_result = new OracleParameter("p_result", OracleDbType.Varchar2, response.codError, ParameterDirection.Output);
                OracleParameter p_mensaje = new OracleParameter("p_mensaje", OracleDbType.Varchar2, response.mensaje, ParameterDirection.Output);

                p_result.Size = 4000;
                p_mensaje.Size = 4000;
                parameter.Add(p_result);
                parameter.Add(p_mensaje);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storeName, parameter);
                ELog.CloseConnection(odr);

                response.codError = p_result.Value.ToString();
                response.mensaje = p_mensaje.Value == DBNull.Value ? string.Empty : p_mensaje.Value.ToString();
            }
            catch (Exception ex)
            {
                response.codError = ELog.obtainConfig("codigo_NO");
                response.mensaje = ex.Message;
                LogControl.save("InsertaPolizaMapfre", ex.ToString(), "3");
            }

            return await Task.FromResult(response);
        }

        public async Task<EquivalenteVM> EquivalenciaMapfre(EquivalenteBM data)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            var response = new EquivalenteVM();
            var storeName = ProcedureName.pkg_IntegraSCTR + "." + ELog.obtainConfig(data.keyStore);

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("p_snomtbl", OracleDbType.Varchar2, ELog.obtainConfig(data.keyTable), ParameterDirection.Input));
                parameter.Add(new OracleParameter("p_scodprt", OracleDbType.Varchar2, data.codProtecta, ParameterDirection.Input));

                //OUTPUT
                OracleParameter p_result = new OracleParameter("p_result", OracleDbType.Varchar2, response.codMapfre, ParameterDirection.Output);
                OracleParameter p_mensaje = new OracleParameter("p_mensaje", OracleDbType.Varchar2, response.mensaje, ParameterDirection.Output);

                p_result.Size = 4000;
                p_mensaje.Size = 4000;
                parameter.Add(p_result);
                parameter.Add(p_mensaje);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storeName, parameter);
                ELog.CloseConnection(odr);

                response.codMapfre = p_result.Value.ToString();
                response.codError = response.codMapfre == ELog.obtainConfig("codigo_NO_EPS") ? ELog.obtainConfig("codigo_NO") : ELog.obtainConfig("codigo_OK");
                response.mensaje = p_mensaje.Value == DBNull.Value ? string.Empty : p_mensaje.Value.ToString();
            }
            catch (Exception ex)
            {
                LogControl.save("EquivalenciaMapfre", ex.ToString(), "3");
            }

            return await Task.FromResult(response);
        }

        public async Task<CetifDocumGenVM> GesDocumentoMapfre(CetifDocumGenBM data)
        {
            var response = new CetifDocumGenVM();

            try
            {
                string urlLogin = data.urlLogin;
                var jsonToken = await WebServiceUtil.invocarServicio(String.Empty, urlLogin, null, ELog.obtainConfig("usuarioWS"), ELog.obtainConfig("passwordWS"));
                var responseToken = JsonConvert.DeserializeObject<TokenVM>(jsonToken, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                if (!String.IsNullOrEmpty(responseToken.token))
                {
                    string urlServicio = data.urlService;
                    data = limpiarData(data);
                    var json = await WebServiceUtil.invocarServicio(JsonConvert.SerializeObject(data, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), urlServicio, responseToken.token);
                    //var json = await WebServiceUtil.invocarServicio(JsonConvert.SerializeObject(data), urlServicio, null);
                    response = JsonConvert.DeserializeObject<CetifDocumGenVM>(json, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    if (response.codError == null)
                    {
                        response.codError = ELog.obtainConfig("codigo_NO");
                        response.descError = ELog.obtainConfig("errorMapfre");
                    }
                }
                else
                {
                    response.codError = ELog.obtainConfig("codigo_NO");
                    response.descError = ELog.obtainConfig("sinToken");
                }

            }
            catch (Exception ex)
            {
                LogControl.save("GesDocumentoMapfre", ex.ToString(), "3");
            }

            return await Task.FromResult(response);
        }

        public async Task<DeclararVM> DeclararMapfre(DeclararBM data)
        {
            var response = new DeclararVM();

            try
            {
                string urlLogin = String.Format(ELog.obtainConfig("mapfreRrtt"), ELog.obtainConfig("login"));
                var jsonToken = await WebServiceUtil.invocarServicio(String.Empty, urlLogin, null, ELog.obtainConfig("usuarioWS"), ELog.obtainConfig("passwordWS"));
                var responseToken = JsonConvert.DeserializeObject<TokenVM>(jsonToken, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                if (!String.IsNullOrEmpty(responseToken.token))
                {
                    string urlServicio = String.Format(ELog.obtainConfig("mapfreRrtt"), ELog.obtainConfig(data.cabecera.keyService));
                    data = limpiarData(data);
                    var json = await WebServiceUtil.invocarServicio(JsonConvert.SerializeObject(data, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), urlServicio, responseToken.token);
                    //var json = await WebServiceUtil.invocarServicio(JsonConvert.SerializeObject(data), urlServicio, null);
                    response = JsonConvert.DeserializeObject<DeclararVM>(json, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    if (response.codError == null)
                    {
                        response.codError = ELog.obtainConfig("codigo_NO");
                        response.descError = ELog.obtainConfig("errorMapfre");
                    }
                }
                else
                {
                    response.codError = ELog.obtainConfig("codigo_NO");
                    response.descError = ELog.obtainConfig("sinToken");
                }


            }
            catch (Exception ex)
            {
                LogControl.save("DeclararMapfre", ex.ToString(), "3");
                response.codError = ELog.obtainConfig("codigo_NO");
                response.descError = ELog.obtainConfig("sinToken");
            }

            return await Task.FromResult(response);
        }

        public async Task<EmitirVM> EmitirMapfre(EmitirBM data)
        {
            var response = new EmitirVM();

            try
            {
                string urlLogin = String.Format(ELog.obtainConfig("mapfreRrtt"), ELog.obtainConfig("login"));
                var jsonToken = await WebServiceUtil.invocarServicio(String.Empty, urlLogin, null, ELog.obtainConfig("usuarioWS"), ELog.obtainConfig("passwordWS"));
                var responseToken = JsonConvert.DeserializeObject<TokenVM>(jsonToken, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                if (!String.IsNullOrEmpty(responseToken.token))
                {
                    string urlServicio = String.Format(ELog.obtainConfig("mapfreRrtt"), ELog.obtainConfig(data.cabecera.keyService));
                    data = limpiarData(data);
                    var json = await WebServiceUtil.invocarServicio(JsonConvert.SerializeObject(data, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), urlServicio, responseToken.token);
                    response = JsonConvert.DeserializeObject<EmitirVM>(json, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    if (response.codError == null)
                    {
                        response.codError = ELog.obtainConfig("codigo_NO");
                        response.descError = ELog.obtainConfig("errorMapfre");
                    }
                }
                else
                {
                    response.codError = ELog.obtainConfig("codigo_NO");
                    response.descError = ELog.obtainConfig("sinToken");
                }
            }
            catch (Exception ex)
            {
                LogControl.save("EmitirMapfre", ex.ToString(), "3");
                response.codError = ELog.obtainConfig("codigo_NO");
                response.descError = ELog.obtainConfig("sinToken");
            }

            return await Task.FromResult(response);
        }

        public async Task<ValidarAseguradosVM> ValAseguradoMapfre(ValidarAseguradosBM data)
        {
            var response = new ValidarAseguradosVM();

            try
            {
                string urlLogin = String.Format(ELog.obtainConfig("mapfreRrtt"), ELog.obtainConfig("login"));
                var jsonToken = await WebServiceUtil.invocarServicio(String.Empty, urlLogin, null, ELog.obtainConfig("usuarioWS"), ELog.obtainConfig("passwordWS"));
                var responseToken = JsonConvert.DeserializeObject<TokenVM>(jsonToken, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                if (!String.IsNullOrEmpty(responseToken.token))
                {
                    string urlServicio = String.Format(ELog.obtainConfig("mapfreRrtt"), ELog.obtainConfig(data.cabecera.keyService));
                    data = limpiarData(data);
                    var json = await WebServiceUtil.invocarServicio(JsonConvert.SerializeObject(data, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), urlServicio, responseToken.token);
                    //var json = await WebServiceUtil.invocarServicio(JsonConvert.SerializeObject(data), urlServicio, null);
                    response = JsonConvert.DeserializeObject<ValidarAseguradosVM>(json, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    if (response.codError == null)
                    {
                        response.codError = ELog.obtainConfig("codigo_NO");
                        response.descError = ELog.obtainConfig("errorMapfre");
                    }
                }
                else
                {
                    response.codError = ELog.obtainConfig("codigo_NO");
                    response.descError = ELog.obtainConfig("sinToken");
                }
            }
            catch (Exception ex)
            {
                LogControl.save("ValAseguradoMapfre", ex.ToString(), "3");
                response.codError = ELog.obtainConfig("codigo_NO");
                response.descError = ELog.obtainConfig("errorMapfre");
            }

            return await Task.FromResult(response);
        }

        public async Task<CotizarVM> GesCotizacionMapfre(CotizarBM data)
        {
            var response = new CotizarVM();

            try
            {
                string urlLogin = String.Format(ELog.obtainConfig("mapfreRrtt"), ELog.obtainConfig("login"));
                var jsonToken = await WebServiceUtil.invocarServicio(String.Empty, urlLogin, null, ELog.obtainConfig("usuarioWS"), ELog.obtainConfig("passwordWS"));
                var responseToken = JsonConvert.DeserializeObject<TokenVM>(jsonToken, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                if (!String.IsNullOrEmpty(responseToken.token))
                {
                    string urlServicio = String.Format(ELog.obtainConfig("mapfreRrtt"), ELog.obtainConfig(data.cabecera.keyService));
                    data = limpiarData(data);
                    var json = await WebServiceUtil.invocarServicio(JsonConvert.SerializeObject(data, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), urlServicio, responseToken.token);
                    //var json = await WebServiceUtil.invocarServicio(JsonConvert.SerializeObject(data), urlServicio, null);
                    response = JsonConvert.DeserializeObject<CotizarVM>(json, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                    if (response.codError == null)
                    {
                        response.codError = ELog.obtainConfig("codigo_NO");
                        response.descError = ELog.obtainConfig("errorMapfre");
                    }
                }
                else
                {
                    response.codError = ELog.obtainConfig("codigo_NO");
                    response.descError = ELog.obtainConfig("sinToken");
                }

            }
            catch (Exception ex)
            {
                response.codError = ELog.obtainConfig("codigo_NO");
                response.descError = ELog.obtainConfig("sinToken");
                LogControl.save("GesCotizacionMapfre", ex.ToString(), "3");
            }

            return await Task.FromResult(response);
        }

        private CotizarBM limpiarData(CotizarBM data)
        {
            data.cabecera.tariff = null;
            data.cabecera.keyService = null;
            return data;
        }

        private DeclararBM limpiarData(DeclararBM data)
        {
            data.cabecera.tariff = null;
            data.cabecera.keyService = null;
            return data;
        }

        private ValidarAseguradosBM limpiarData(ValidarAseguradosBM data)
        {
            data.cabecera.tariff = null;
            data.cabecera.keyService = null;
            return data;
        }

        private CetifDocumGenBM limpiarData(CetifDocumGenBM data)
        {
            data.urlService = null;
            data.urlLogin = null;
            data.nroConstancia = data.nroConstancia == "null" ? null : data.nroConstancia;
            return data;
        }
        private EmitirBM limpiarData(EmitirBM data)
        {
            data.cabecera.tariff = null;
            data.cabecera.keyService = null;
            return data;
        }
    }
}
