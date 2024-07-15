using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Cors;
using WSPlataforma.Core;
using System.Web.Http;
using System.Configuration;
using WSPlataforma.Entities.LoadMassiveModel;
using WSPlataforma.Entities.CobranzasModel;

namespace WSPlataforma.Controllers
{
    [RoutePrefix("Api/Cobranzas")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CobranzasController : ApiController
    {
        [Route("InsertCreditPolicies")]
        [HttpPost]
        public IHttpActionResult InsertCreditPolicies(EntityPolicyCreditBM entitie)
        {
            CobranzasCORE CobranzasCORE = new CobranzasCORE();
            var response = CobranzasCORE.InsertCreditPolicies(entitie);
            return Ok(response);
        }

        [Route("GetTypeRisk")]
        [HttpGet]
        public IHttpActionResult GetTypeRisk()
        {
            CobranzasCORE CobranzasCORE = new CobranzasCORE();
            var response = CobranzasCORE.GetTypeRisk();
            return Ok(response);
        }

        [Route("GetTypeRestric")]
        [HttpGet]
        public IHttpActionResult GetTypeRestric()
        {
            CobranzasCORE CobranzasCORE = new CobranzasCORE();
            var response = CobranzasCORE.GetTypeRestric();
            return Ok(response);
        }

        [Route("GetCreditPoliciesList")]
        [HttpPost]
        public IHttpActionResult GetCreditPoliciesList(ParamsCreditPoliciesBM param)
        {
            CobranzasCORE CobranzasCORE = new CobranzasCORE();
            var response = CobranzasCORE.getCreditPoliciesList(param);
            return Ok(response);
        }
        [Route("getStatusClientList")]
        [HttpPost]
        public IHttpActionResult GetStatusClientList(ParamsPaymentClientStatus param)
        {
            CobranzasCORE CobranzasCORE = new CobranzasCORE();
            var response = CobranzasCORE.getStatusClientList(param);
            return Ok(response);
        }
        [Route("getClientInfoList")]
        [HttpPost]
        public IHttpActionResult GetClientInfoList(ParamsPaymentClientInfo param)
        {
            CobranzasCORE CobranzasCORE = new CobranzasCORE();
            var response = CobranzasCORE.getClientInfoList(param);
            return Ok(response);
        }

        [Route("GetClientPrivilegiosList")]
        [HttpPost]
        public IHttpActionResult GetClientPrivilegiosList(ParamsPaymentClientStatus param)
        {
            CobranzasCORE CobranzasCORE = new CobranzasCORE();
            var response = CobranzasCORE.getClientPrivilegiosList(param);
            return Ok(response);
        }
        [Route("InsertClientRestric")]
        [HttpPost]
        public IHttpActionResult InsertClientRestric(ParamsPaymentClientStatus entitie)
        {
            CobranzasCORE CobranzasCORE = new CobranzasCORE();
            var response = CobranzasCORE.InsertClientRestric(entitie);
            return Ok(response);
        }

        [Route("ValidateLock")]
        [HttpPost]
        public IHttpActionResult ValidateLock(ParamsValidateLock param)
        {
            CobranzasCORE CobranzasCORE = new CobranzasCORE();
            var response = CobranzasCORE.ValidateLock(param);
            return Ok(response);
        }

        [Route("ValidateDebt")]
        [HttpPost]
        public IHttpActionResult ValidateDebt(ParamsValidateDebt param)
        {
            CobranzasCORE CobranzasCORE = new CobranzasCORE();
            var response = CobranzasCORE.ValidateDebt(param);
            return Ok(response);
        }

        [Route("GenerateAccountStatus")]
        [HttpPost]
        public IHttpActionResult generateAccountStatus(ParamsGenerateAccountStatus param)
        {
            CobranzasCORE CobranzasCORE = new CobranzasCORE();
            var response = CobranzasCORE.generateAccountStatus(param);
            return Ok(response);
        }
        [Route("GeneratesSlipCCE")]
        [HttpPost]
        public IHttpActionResult GeneratesSlipCCE(ParamsValidateDebt param)
        {
            CobranzasCORE CobranzasCORE = new CobranzasCORE();
            var response = CobranzasCORE.GeneratesSlipCCE(param);
            return Ok(response);
        }
        [Route("GetTramaMovimiento")]
        [HttpPost]
        public IHttpActionResult GetTramaMovimiento(ParamsMovimientoVL param)
        {
            CobranzasCORE CobranzasCORE = new CobranzasCORE();
            var response = CobranzasCORE.GetTramaMovimiento(param);
            return Ok(response);
        }
        [Route("GetTramaMovimientoSCTR")]
        [HttpPost]
        public IHttpActionResult GetTramaMovimientoSCTR(ParamsMovimientoVL param)
        {
            CobranzasCORE CobranzasCORE = new CobranzasCORE();
            var response = CobranzasCORE.GetTramaMovimientoSCTR(param);
            return Ok(response);
        }
        [Route("ValidateDebtPolicy")]
        [HttpPost]
        public IHttpActionResult ValidateDebtPolicy(ParamsValidateDebt param)
        {
            CobranzasCORE CobranzasCORE = new CobranzasCORE();
            var response = CobranzasCORE.ValidateDebtPolicy(param);
            return Ok(response);
        }
        [Route("GetReceiptBill")]
        [HttpPost]
        public IHttpActionResult GetBill(ParamsBill param)
        {
            CobranzasCORE CobranzasCORE = new CobranzasCORE();
            var response = CobranzasCORE.getReceiptBill(param);
            return Ok(response);
        }
        [Route("InsertRebill")]
        [HttpPost]
        public IHttpActionResult InsertRebill(ParamsRebill entitie)
        {
            CobranzasCORE CobranzasCORE = new CobranzasCORE();
            var response = CobranzasCORE.InsertRebill(entitie);
            return Ok(response);
        }

        [Route("GetTramaEndoso")]
        [HttpPost]
        public IHttpActionResult GetTramaEndoso(ParamsMovimientoVL param)
        {
            CobranzasCORE CobranzasCORE = new CobranzasCORE();
            var response = CobranzasCORE.GetTramaEndoso(param);
            return Ok(response);
        }

        [Route("GetCoverDetail")]
        [HttpPost]
        public IHttpActionResult GetCoverDetail(ParamsMovimientoVL param)
        {
            CobranzasCORE CobranzasCORE = new CobranzasCORE();
            var response = CobranzasCORE.GetTramaCover(param);
            return Ok(response);
        }
    }
}