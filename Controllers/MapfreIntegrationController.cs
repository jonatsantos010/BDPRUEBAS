using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WSPlataforma.Core;
using WSPlataforma.Entities.MapfreIntegrationModel;
using WSPlataforma.Entities.MapfreIntegrationModel.BindingModel;
using WSPlataforma.Entities.MapfreIntegrationModel.ViewModel;
using WSPlataforma.Util;

namespace WSPlataforma.Controllers
{
    [RoutePrefix("Api/MapfreIntegration")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class MapfreIntegrationController : ApiController
    {
        MapfreIntegrationCORE mapfreCore = new MapfreIntegrationCORE();

        [Route("UpdateRequest")]
        [HttpPost]
        public async Task<UpdateRequestVM> UpdateRequest(UpdateRequestBM data)
        {
            return await mapfreCore.UpdateRequest(data);
        }

        [Route("CancelPolicy")]
        [HttpPost]
        public async Task<CancelPolicyVM> CancelPolicy(CancelPolicyBM data)
        {
            return await mapfreCore.CancelPolicy(data);
        }

        // Cotizar, Validar Cliente, Ajustar Tasa y Consultar
        [Route("GesCotizacionMapfre")]
        [HttpPost]
        public async Task<CotizarVM> GesCotizacionMapfre(CotizarBM data)
        {
            // cabecera
            if (data.cabecera != null)
            {
                data.cabecera.codigoAplicacion = ELog.obtainConfig("codAplicacion");
                data.cabecera.codigoUsuario = ELog.obtainConfig("codUsuarioMP");
            }

            // poliza
            if (data.poliza != null)
            {
                data.poliza.codAgt = Convert.ToInt32(ELog.obtainConfig("codAgente"));
            }

            // producto
            if (data.producto != null)
            {
                data.producto.codCia = Convert.ToInt32(ELog.obtainConfig("codCia"));
                data.producto.codRamo = Convert.ToInt32(ELog.obtainConfig("codRamo"));
                data.producto.mcaEmiteSalud = ELog.obtainConfig("mcaEmiteSalud");
            }

            // cliente
            if (data.cliente != null)
            {
                data.listaValidacion = ELog.obtainConfig("validarList").Split(';').ToList();
            }

            // riesgos
            if (data.riesgoSCTR != null)
            {
                data.riesgoSCTR[0].numRiesgoSalud = 1;
                data.riesgoSCTR[0].nomCategoriaSalud = ELog.obtainConfig("nomCategoriaMP");
            }

            var response = new CotizarVM();
            response = await mapfreCore.GesCotizacionMapfre(data);
            return response;
        }

        // Validar Asegurados
        [Route("ValAseguradosMapfre")]
        [HttpPost]
        public async Task<ValidarAseguradosVM> ValAseguradoMapfre(ValidarAseguradosBM data)
        {
            // cabecera
            if (data.cabecera != null)
            {
                data.cabecera.codigoAplicacion = ELog.obtainConfig("codAplicacion");
                data.cabecera.codigoUsuario = ELog.obtainConfig("codUsuarioMP");
            }

            var response = new ValidarAseguradosVM();
            response = await mapfreCore.ValAseguradoMapfre(data);
            return response;
        }

        // Declarar
        [Route("DeclararMapfre")]
        [HttpPost]
        public async Task<DeclararVM> DeclararMapfre(DeclararBM data)
        {
            data.tipoMovimiento = ELog.obtainConfig(String.Format(ELog.obtainConfig("equiTransaccionKey"), data.tipoMovimiento));

            // cabecera
            if (data.cabecera != null)
            {
                data.cabecera.codigoAplicacion = ELog.obtainConfig("codAplicacion");
                data.cabecera.codigoUsuario = ELog.obtainConfig("codUsuarioMP");
            }

            // Riesgo SCTR
            if (data.riesgoSCTR != null)
            {
                data.riesgoSCTR[0].numRiesgoSalud = 1;
                data.riesgoSCTR[0].nomCategoriaSalud = ELog.obtainConfig("nomCategoriaMP");
            }

            if (data.producto != null)
            {
                data.producto.codCia = Convert.ToInt32(ELog.obtainConfig("codCia"));
                data.producto.codRamo = Convert.ToInt32(ELog.obtainConfig("codRamo"));
                data.producto.mcaEmiteSalud = ELog.obtainConfig("mcaEmiteSalud");
            }

            var response = new DeclararVM();
            response = await mapfreCore.DeclararMapfre(data);
            return response;
        }

        // Emitir
        [Route("EmitirMapfre")]
        [HttpPost]
        public async Task<EmitirVM> EmitirMapfre(EmitirBM data)
        {
            // cabecera
            if (data.cabecera != null)
            {
                data.cabecera.codigoAplicacion = ELog.obtainConfig("codAplicacion");
                data.cabecera.codigoUsuario = ELog.obtainConfig("codUsuarioMP");
            }

            var response = new EmitirVM();
            response = await mapfreCore.EmitirMapfre(data);
            return response;
        }

        // Documentos
        [Route("GesDocumentoMapfre")]
        [HttpPost]
        public async Task<CetifDocumGenVM> GesDocumentoMapfre(CetifDocumGenBM data)
        {
            var response = new CetifDocumGenVM();
            response = await mapfreCore.GesDocumentoMapfre(data);
            return response;
        }

        // Equivalentes
        [Route("EquivalenciaMapfre")]
        [HttpPost]
        public async Task<EquivalenteVM> EquivalenciaMapfre(EquivalenteBM data)
        {
            var response = new EquivalenteVM();

            if (!String.IsNullOrEmpty(data.codProtecta) && data.codProtecta != "null")
            {
                response = await mapfreCore.EquivalenciaMapfre(data);
            }

            return response;
        }

        public async Task<string> EquivalenciaMapfreWS(string codProtecta, string keyTable, string keyStore)
        {
            var request = new EquivalenteBM();
            request.codProtecta = codProtecta;
            request.keyTable = keyTable;
            request.keyStore = keyStore;

            var response = await EquivalenciaMapfre(request);

            return response.codMapfre;
        }

        // Emitir Mapfre en Protecta
        [Route("InsertaPolizaMapfre")]
        [HttpPost]
        public async Task<PolizaMpVM> InsertaPolizaMapfre(PolizaMpBM data)
        {
            var response = new PolizaMpVM();
            response = await mapfreCore.InsertaPolizaMapfre(data);
            return response;
        }

        // Declarar Mapfre en Protecta
        [Route("DeclararPolizaMapfre")]
        [HttpPost]
        public async Task<DeclararMpVM> DeclararPolizaMapfre(DeclararMpBM data)
        {
            var response = new DeclararMpVM();
            response = await mapfreCore.DeclararPolizaMapfre(data);
            return response;
        }
    }
}
