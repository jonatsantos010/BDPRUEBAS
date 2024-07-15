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
using WSPlataforma.Entities.MaintenancePlanesAsistModel.BindingModel;
using WSPlataforma.Entities.MaintenancePlanesAsistModel.ViewModel;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using System.Diagnostics;
using System.Text;
using WSPlataforma.Entities.CommissionModel.BindingModel;

namespace WSPlataforma.Controllers
{
    [RoutePrefix("Api/PlanesAsistMaintenance")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class MaintenancePlanesAsistController : ApiController
    {
        MaintenancePlanesAsistCORE maintenanceCORE = new MaintenancePlanesAsistCORE();


        [Route("Habilitar")]
        [HttpGet]
        public IHttpActionResult Habilitar(long NPOLIZA, int NORDEN, int NOPCION)
        {
            return Ok(maintenanceCORE.Habilitar(NPOLIZA, NORDEN, NOPCION));
        }
        [Route("GetServices")]
        [HttpGet]
        public IHttpActionResult GetServices()
        {
            return Ok(maintenanceCORE.GetServices());
        }

        [Route("GetBranchList")]
        [HttpGet]
        public IHttpActionResult GetTransactionsByProduct()
        {
            return Ok(maintenanceCORE.GetBranch());
        }

        [Route("GetPlan")]
        [HttpGet]
        public IHttpActionResult GetPlanes(Int64 poliza, int ramo, int producto)
        {
            return Ok(maintenanceCORE.GetPlanes(poliza, ramo, producto));
        }

        [Route("GetCoverAdicional")]
        [HttpGet]
        public IHttpActionResult GetCoverAdicional(int ramo, int producto)
        {
            return Ok(maintenanceCORE.GetCoverAdicional(ramo, producto));
        }
        [Route("GetRoles")]
        [HttpGet]
        public IHttpActionResult GetRoles()
        {
            return Ok(maintenanceCORE.GetRoles());
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

        [Route("setListaPlanes")]
        [HttpGet]
        public IHttpActionResult SetListaPlanes(String data, int NUSERCODE, int producto, int ramo, long poliza)
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            List<PlanAsistVM> objects = JsonConvert.DeserializeObject<List<PlanAsistVM>>(data, settings);
            return Ok(maintenanceCORE.SetListaPlanes(objects, ramo, producto, poliza, NUSERCODE));
        }
        [Route("ActualizaPlan")]
        [HttpGet]
        public IHttpActionResult ActualizaPlan(String data, int NUSERCODE, int producto, int ramo, long poliza)
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            List<PlanAsistVM> objects = JsonConvert.DeserializeObject<List<PlanAsistVM>>(data, settings);
            return Ok(maintenanceCORE.ActualizaPlan(objects, ramo, producto, poliza, NUSERCODE));
        }

        [Route("GetPlanesAsistenciasA")]
        [HttpPost]
        public IHttpActionResult GetPlanesAsistencias(FiltersBM data)
        {
            return Ok(maintenanceCORE.GetPlanesAsistencias(data.nBranch, data.nProduct, data.nPolicy, data.ntipBusqueda, data.nPlan)); //INI <RQ2024-57 - 03/04/2024>
        }

        [Route("GetServicioXPlan")]
        [HttpPost]
        public IHttpActionResult GetServicioXPlan(FiltersBM data)
        {
            return Ok(maintenanceCORE.GetServicioXPlan(data.nPlan, data.nUsercode, data.nBranch, data.nProduct, data.nPolicy));
        }

        [Route("ValidaPoliza")]
        [HttpPost]
        public IHttpActionResult ValidaPoliza(FiltersBM data)
        {
            return Ok(maintenanceCORE.ValidaPoliza(data.nBranch, data.nPolicy, data.nProduct));
        }

        [Route("GetTipBusquedaList")]//INI <RQ2024-57 - 03/04/2024>
        [HttpGet]
        public IHttpActionResult GetTipBusquedaList()
        {
            return Ok(maintenanceCORE.GetTipBusquedaList());
        }

        [Route("GetDivisorList")]//INI <RQ2024-57 - 03/04/2024>
        [HttpGet]
        public IHttpActionResult GetDivisorList()
        {
            return Ok(maintenanceCORE.GetDivisorList());
        }

        [Route("setListaTasa")]//INI <RQ2024-57 - 03/04/2024>
        [HttpPost]
        public IHttpActionResult setListaTasa(PlanAsistVM data)
        {
          //  var settings = new JsonSerializerSettings
            //{
          //      NullValueHandling = NullValueHandling.Ignore,
           //    MissingMemberHandling = MissingMemberHandling.Ignore
            //};
            //PlanAsistVM objects = JsonConvert.DeserializeObject<PlanAsistVM>(data, settings);
            return Ok(maintenanceCORE.setListaTasa(data));
        }

        [Route("getEdiTasaPol")]
        [HttpPost]
        public IHttpActionResult getEdiTasaPol(FiltersBM data)
        {
            return Ok(maintenanceCORE.getEdiTasaPol(data.nBranch, data.nProduct, data.nPolicy, data.nModulo, data.nRol));
        }

        [Route("actualizarTasa")]//INI <RQ2024-57 - 03/04/2024>
        [HttpPost]
        public IHttpActionResult actualizarTasa(PlanAsistVM data)
        {
            return Ok(maintenanceCORE.actualizarTasa(data));
        }
    }
}
