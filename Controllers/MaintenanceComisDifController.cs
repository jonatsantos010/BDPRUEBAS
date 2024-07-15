using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WSPlataforma.Core;
using WSPlataforma.Entities.MaintenanceComisDifModel.BindingModel;
using WSPlataforma.Entities.MaintenanceComisDifModel.ViewModel;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using System.Diagnostics;
using System.Text;
using WSPlataforma.Entities.CommissionModel.BindingModel;

namespace WSPlataforma.Controllers
{
    [RoutePrefix("Api/comisDifMaintenance")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class MaintenanceComiDifController : ApiController
    {
        MaintenanceComiDifCORE maintenanceCORE = new MaintenanceComiDifCORE();

        [Route("GetSAList")]
        [HttpGet]
        public IHttpActionResult GetSAList()
        {
            return Ok(maintenanceCORE.GetSAList());
        }

        [Route("GetBranchList")]
        [HttpGet]
        public IHttpActionResult GetTransactionsByProduct()
        {
            return Ok(maintenanceCORE.GetBranch());
        }

        [Route("GetModulosXPol")]
        [HttpGet]
        public IHttpActionResult GetModulosXPol(long poliza, int ramo, int producto)
        {
            return Ok(maintenanceCORE.GetModulosXPol(poliza, ramo, producto));
        }
        [Route("GetRolesXPol")]
        [HttpGet]
        public IHttpActionResult GetRolesXPol(long poliza, int ramo, int producto)
        {
            return Ok(maintenanceCORE.GetRolesXPol(poliza, ramo, producto));
        }
        [Route("GetProductsList")]
        [HttpGet]
        public IHttpActionResult GetProductsList(int nBranch)
        {
            return Ok(maintenanceCORE.GetProductsById(nBranch));
        }

        [Route("GetTypeComiDif")]
        [HttpGet]
        public IHttpActionResult GetTypeComiDif(int nFilter)
        {
            return Ok(maintenanceCORE.GetTypeComiDif(nFilter));
        }

        [Route("GetGroupByTypeComiDif")]
        [HttpGet]
        public IHttpActionResult GetGroupByTypeComiDif(int nFilter)
        {
            return Ok(maintenanceCORE.GetGroupByTypeComiDif(nFilter));
        }

        [Route("GetCampos")]
        [HttpGet]
        public IHttpActionResult GetCampos(int ntipo, int ngrupo)
        {
            return Ok(maintenanceCORE.GetCampos(ntipo, ngrupo));
        }

        [Route("setListaComisiones")]
        [HttpGet]
        public IHttpActionResult setListaComisiones(String data, int NUSERCODE, int grupo, int producto, int ramo, int tipo, long poliza)
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            List<ComisDifVM> objects = JsonConvert.DeserializeObject<List<ComisDifVM>>(data, settings);
            return Ok(maintenanceCORE.setListaComisiones(objects, ramo, producto, tipo, grupo, poliza, NUSERCODE));
        }
        [Route("ActualizaComision")]
        [HttpGet]
        public IHttpActionResult ActualizaComision(String data, int NUSERCODE, int grupo, int producto, int ramo, int tipo, long poliza)
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            List<ComisDifVM> objects = JsonConvert.DeserializeObject<List<ComisDifVM>>(data, settings);
            return Ok(maintenanceCORE.ActualizaComision(objects, ramo, producto, tipo, grupo, poliza, NUSERCODE));
        }


        [Route("Habilitar")]
        [HttpGet]
        public IHttpActionResult Habilitar(long NPOLIZA, int NOPCION)
        {
            return Ok(maintenanceCORE.Habilitar(NPOLIZA, NOPCION));
        }

        [Route("EliminaConfig")]
        [HttpPost]
        public IHttpActionResult EliminaConfig(FiltersBM data)
        {
            return Ok(maintenanceCORE.EliminaConfig(data.nBranch, data.nProduct, data.nPolicy,data.nOrden));
        }
        
        [Route("ValidaPoliza")]
        [HttpPost]
        public IHttpActionResult ValidaPoliza(FiltersBM data)
        {
            return Ok(maintenanceCORE.ValidaPoliza(data.nBranch, data.nProduct, data.nPolicy));
        }

        [Route("GetComisDifConfigsA")]
        [HttpPost]
        public IHttpActionResult GetComisDifConfigs(FiltersBM data)
        {
            RequestConfigs Configs = new RequestConfigs();
            Configs.POLIZAS = maintenanceCORE.GetComisDifConfigs(data.nBranch, data.nProduct, data.nPolicy);
            Configs.VALIDACIONES = maintenanceCORE.ValidaPoliza(data.nBranch, data.nProduct, data.nPolicy);
            return Ok(Configs);
        }
        /*
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
        */
    }
}
