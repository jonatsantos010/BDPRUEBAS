using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WSPlataforma.Core;
using WSPlataforma.Entities.CommissionModel.BindingModel;

namespace WSPlataforma.Controllers
{
    [RoutePrefix("Api/Commission")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CommissionController : ApiController
    {

        [Route("ListBroker")]
        [HttpPost]
        public IHttpActionResult CommisionBroker(CommissionTransactionAllSearchBM search)
        {
            CommissionCORE commissionCORE = new CommissionCORE();
            var response = commissionCORE.GetCommisionBroker(search);
            return Ok(response);
        }


        [Route("ListIntermedary")]
        [HttpPost]
        public IHttpActionResult GetListIntermedary(CommissionTransactionAllSearchBM search)
        {
            CommissionCORE commissionCORE = new CommissionCORE();
            var response = commissionCORE.getListIntermedary(search);
            return Ok(response);
        }

        [Route("UpdCommission")]
        [HttpPost]
        public IHttpActionResult UpdCommissionBR(CommissionBR comission)
        {
            CommissionCORE commissionCORE = new CommissionCORE();
            var response = commissionCORE.UpdCommissionBR(comission);
            return Ok(response);
        }

        [Route("GetLastCommission")]
        [HttpPost]
        public IHttpActionResult GetLastCommission(CommissionTransactionAllSearchBM comission)
        {
            CommissionCORE commissionCORE = new CommissionCORE();
            var response = commissionCORE.GetLastCommission(comission);
            return Ok(response);
        }

        [Route("InsCommissionVDP")]
        [HttpPost]
        public IHttpActionResult InsCommissionVDP(CommissionBR comission)
        {
            CommissionCORE commissionCORE = new CommissionCORE();
            var response = commissionCORE.InsCommissionVDP(comission);
            return Ok(response);
        }

        [Route("UpdCommissionVDP")]
        [HttpPost]
        public IHttpActionResult UpdCommissionVDP(CommissionBR comission)
        {
            CommissionCORE commissionCORE = new CommissionCORE();
            var response = commissionCORE.UpdCommissionVDP(comission);
            return Ok(response);
        }

        [Route("ValPerfilVDP")]
        [HttpGet]
        public IHttpActionResult ValPerfilVDP(int nid_user)
        {
            CommissionCORE commissionCORE = new CommissionCORE();
            var response = commissionCORE.ValPerfilVDP(nid_user);
            return Ok(response);
        }

    }
}
