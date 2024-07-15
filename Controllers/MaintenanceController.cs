using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WSPlataforma.Core;
using WSPlataforma.Entities.MaintenanceModel.BindingModel;
using WSPlataforma.Entities.MaintenanceModel.ViewModel;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using System.Diagnostics;
using System.Text;
using WSPlataforma.Entities.CommissionModel.BindingModel;

namespace WSPlataforma.Controllers
{
    [RoutePrefix("Api/Maintenance")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class MaintenanceController : ApiController
    {
        MaintenanceCORE maintenanceCORE = new MaintenanceCORE();

        [Route("transactionsByProduct")]
        [HttpPost]
        public IHttpActionResult GetTransactionsByProduct(FiltersBM data)
        {
            return Ok(maintenanceCORE.GetTranscactionsByProduct(data.nBranch, data.nProduct));
        }

        [Route("parametersByTransaction")]
        [HttpPost]
        public IHttpActionResult GetParametersByTransaction(FiltersBM data)
        {
            return Ok(maintenanceCORE.GetParametersByTransaction(data.nBranch, data.nProduct, data.nTypeTransac, data.nPerfil, data.nParam, data.nGob));
        }

        [Route("comboParametersByTransaction")]
        [HttpPost]
        public IHttpActionResult GetComboParametersByTransaction(FiltersBM data)
        {
            return Ok(maintenanceCORE.GetComboParametersByTransaction(data.nBranch, data.nProduct, data.nTypeTransac, data.nPerfil));
        }

        [Route("insertRma")]
        [HttpPost]
        public IHttpActionResult InsertRma(RmaBM data)
        {
            return Ok(maintenanceCORE.InsertRma(data));
        }

        [Route("getRma")]
        [HttpPost]
        public IHttpActionResult GetRma(RmaVM rma)
        {
            return Ok(maintenanceCORE.GetRma(rma));
        }

        [Route("getActivityList")]
        [HttpPost]
        public IHttpActionResult GetActivityList(ActivityBM data)
        {
            return Ok(maintenanceCORE.GetActivityList(data));
        }

        [Route("GetActivityListMovement")]
        [HttpPost]
        public IHttpActionResult GetActivityListMovement(ActivityBM data)
        {
            return Ok(maintenanceCORE.GetActivityListMovement(data));
        }
        [Route("insertActivities")]
        [HttpPost]
        public GenericResponse InsertActivities(ActivityParamsBM data)
        {
            return maintenanceCORE.InsertActivities(data);
        }

        [Route("getCombActivities")]
        [HttpPost]
        public IHttpActionResult GetCombActivities(ActivityParamsBM data)
        {
            return Ok(maintenanceCORE.GetCombActivities(data));
        }

        [Route("insertCombActivDet")]
        [HttpPost]
        public IHttpActionResult InsertCombActivityDet(ActivityParamsBM data)
        {
            maintenanceCORE.InsertCombActivityDet(data);
            return Ok();
        }

        [Route("insertRetroactivity")]
        [HttpPost]
        public IHttpActionResult InsertRetroactivity(RetroactivityBM data)
        {
            return Ok(maintenanceCORE.InsertRetroactivity(data));
        }

        [Route("getProfileProduct")]
        [HttpPost]
        public IHttpActionResult getProfileProduct(FiltersBM data)
        {
            return Ok(maintenanceCORE.getProfileProduct(data));
        }

        [Route("getProfiles")]
        [HttpPost]
        public IHttpActionResult GetProfiles(FiltersBM data)
        {
            return Ok(maintenanceCORE.GetProfiles(data));
        }

        [Route("getConfigDays")]
        [HttpPost]
        public IHttpActionResult GetConfigDays()
        {
            return Ok(maintenanceCORE.GetConfigDays());
        }

        [Route("getRetroactivityList")]
        [HttpPost]
        public IHttpActionResult GetRetroactivityList(FiltersBM data)
        {
            return Ok(maintenanceCORE.GetRetroactivityList(data));
        }

        [Route("calcFinVigencia")]
        [HttpGet]
        public string CalcFinVigencia(string inicioVigencia)
        {
            return maintenanceCORE.CalcFinVigencia(inicioVigencia);
        }

        [Route("GetRetroactivityMessage")]
        [HttpPost]
        public IHttpActionResult GetRetroactivityMessage(FiltersBM data)
        {
            return Ok(maintenanceCORE.GetRetroactivityMessage(data));
        }

        [Route("GetUserAccess")]
        [HttpPost]
        public IHttpActionResult GetUserAccess(FiltersBM data)
        {
            return Ok(maintenanceCORE.getUserAccess(data));
        }


        [Route("getRmvPorcentaje")]
        [HttpPost]
        public IHttpActionResult GetRmvPorcentaje(PorcentajeRMV rmv)
        {
            return Ok(maintenanceCORE.GetPorcentajeRmv(rmv));
        }

        [Route("getTiposRiesgo")]
        [HttpPost]
        public IHttpActionResult GetTiposRiesgo(TypeRiskVM riesgo)
        {
            return Ok(maintenanceCORE.GetTypeRiskVM(riesgo));
        }

        [Route("updatePorcentajeRMV")]
        [HttpPost]
        public IHttpActionResult UpdatePorcentajeRMV(PorcentajeRMV riesgo)
        {
            return Ok(maintenanceCORE.UpdatePorcentajeRMV(riesgo));
        }

        [Route("getHistorialPorcentajeRMV")]
        [HttpPost]
        public IHttpActionResult GetHistorialPorcentajeRMV(PorcentajeRMV riesgo)
        {
            return Ok(maintenanceCORE.GetHistorialPorcentajeRMV(riesgo));
        }
    }
}
