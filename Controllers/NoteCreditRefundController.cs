/*************************************************************************************************/
/*  NOMBRE              :   NoteCreditRefundController.CS                                        */
/*  DESCRIPCION         :   Capa API COntroller, recibe la llamada desde el front-end.           */
/*  AUTOR               :   MATERIAGRIS - PEDRO ANTICONA VALLE                                    */
/*  FECHA               :   22-12-2021                                                           */
/*  VERSION             :   1.0 - Generación de NC - PD                                          */
/*************************************************************************************************/
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WSPlataforma.Entities.PreliminaryModel.BindingModel;
using WSPlataforma.Entities.ReportModel.BindingModel;
using WSPlataforma.Core;
using WSPlataforma.Entities.NoteCreditRefundModel.ViewModel;
using System.Web.Http.Cors;
using WSPlataforma.Util;
using WSPlataforma.Core;

namespace WSPlataforma.Controllers
{
    [RoutePrefix("Api/NoteCreditRefund")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class NoteCreditRefundController2 : ApiController
    {
        NoteCreditRefundCore2 NoteCreditRefundCore = new NoteCreditRefundCore2();

        #region Filters

        [Route("Branches")]
        [HttpGet]
        public IHttpActionResult GetBranches()
        {
            var result = this.NoteCreditRefundCore.GetBranches();
            return Ok(result);
        }

        [Route("GetProductsListByBranch")]
        [HttpPost]
        public IHttpActionResult GetProductsListByBranch(ProductVM data)
        {
           
            var response = this.NoteCreditRefundCore.GetProductsListByBranch(data);

            return Ok(response);
            
        }

        [Route("GetParameter")]
        [HttpPost]
        public IHttpActionResult GetParameter(ProductVM data)
        {

            var response = this.NoteCreditRefundCore.GetParameter(data);

            return Ok(response);

        }

        [Route("GetDocumentTypeList")]
        [HttpGet]
        public IHttpActionResult GetDocumentTypeList()
        {
            var result = this.NoteCreditRefundCore.GetDocumentTypeList();
            return Ok(result);
        }

        [Route("GetBillTypeList")]
        [HttpGet]
        public IHttpActionResult GetBillTypeList()
        {
            var result = this.NoteCreditRefundCore.GetBillTypeList();
            return Ok(result);
        }
        #endregion

        [Route("pruebas")]
        [HttpPost]
        public IHttpActionResult pruebas(PremiumVM data)
        {
            var result = this.NoteCreditRefundCore.pruebas(data);
            return Ok(result);
        }


        [Route("GetListPremium")]
        [HttpPost]
        public IHttpActionResult GetListPremium(PremiumVM data)
        {
            var result = this.NoteCreditRefundCore.GetListPremium(data);
            return Ok(result);
        }

        [Route("GetListReciDev")]
        [HttpPost]
        public IHttpActionResult GetListReciDev(PremiumVM [] data)
        {
            var result = this.NoteCreditRefundCore.GetListReciDev(data);
            return Ok(result);
        }

        [Route("GetListDetRecDev")]
        [HttpPost]
        public IHttpActionResult GetListDetRecDev(PremiumVM[] data)
        {
            var result = this.NoteCreditRefundCore.GetListDetRecDev(data);
            return Ok(result);
        }

        [Route("GetFilaResi")]
        [HttpPost]
        public IHttpActionResult GetFilaResi(PremiumVM data)
        {
            var result = this.NoteCreditRefundCore.GetFilaResi(data);
            return Ok(result);
        }



    }
}