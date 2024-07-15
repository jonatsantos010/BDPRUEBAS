using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Cors;
using System.Web.Http;
using WSPlataforma.Core;
using Newtonsoft.Json;
using WSPlataforma.Entities.AwsDesgravamenModel.ViewModel;
//using WSPlataforma.Entities.MaintenancePlanesAsistModel.ViewModel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView;
using WSPlataforma.Entities.Reports.BindingModel.ReportEntities;

namespace WSPlataforma.Controllers
{
    [RoutePrefix("Api/AwsDesgravamen")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AwsDesgravamenController: ApiController
    {
        AwsDesgravamenCORE awsDesgravamenCORE = new AwsDesgravamenCORE();

        [Route("getHorasList")]
        [HttpGet]
        public IHttpActionResult getHorasList()
        {
            return Ok(awsDesgravamenCORE.getHorasList());
        }

        [Route("GetBranchList")]
        [HttpGet]
        public IHttpActionResult GetTransactionsByProduct()
        {
            return Ok(awsDesgravamenCORE.GetBranch());
        }

        [Route("GetDiaList")]
        [HttpGet]
        public IHttpActionResult GetDiaList()
        {
            return Ok(awsDesgravamenCORE.GetDiaList());
        }

        [Route("GetHoraList")]
        [HttpGet]
        public IHttpActionResult GetHoraList()
        {
            return Ok(awsDesgravamenCORE.GetHoraList());
        }

        [Route("getMinutoList")]
        [HttpGet]
        public IHttpActionResult getMinutoList()
        {
            return Ok(awsDesgravamenCORE.getMinutoList());
        }

        [Route("GetProductsList")]
        [HttpGet]
        public IHttpActionResult GetProductsList(int nBranch)
        {
            return Ok(awsDesgravamenCORE.GetProductsById(nBranch));
        }

        [Route("nuevoHorario")]//INI <RQ2024-57 - 03/04/2024>
        [HttpPost]
        public IHttpActionResult nuevoHorario(HorariosVM data)
        {
            return Ok(awsDesgravamenCORE.nuevoHorario(data));
        }

        [Route("actualizarHorario")]//INI <RQ2024-57 - 03/04/2024>
        [HttpPost]
        public IHttpActionResult actualizarHorario(HorariosVM data)
        {
            return Ok(awsDesgravamenCORE.actualizarHorario(data));
        }

        [Route("getFacSuspendida")]//INI <RQ2024-57 - 03/04/2024>
        [HttpGet]
        public IHttpActionResult getFacSuspendida(int NPOLICY)
        {
            return Ok(awsDesgravamenCORE.getFacSuspendida(NPOLICY));
        }

        [Route("BorrarHorario")]//INI <RQ2024-57 - 03/04/2024>
        [HttpGet]
        public IHttpActionResult BorrarHorario(int NBRANCH, int NPRODUCT, int NDIA)
        {
            return Ok(awsDesgravamenCORE.BorrarHorario(NBRANCH, NPRODUCT, NDIA));
        }

    }
}