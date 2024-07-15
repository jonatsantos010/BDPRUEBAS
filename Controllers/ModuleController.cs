using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WSPlataforma.Core;
using WSPlataforma.Entities.ModuleModel.BindingModel;
using WSPlataforma.Entities.ViewModel;
using System.Web.Http;
using System.Web.Http.Cors;
using WSPlataforma.Entities.ModuleModel.ViewModel;

namespace WSPlataforma.Controllers
{
    [RoutePrefix("Api/ModuleManager")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ModuleController : ApiController
    {
        ModuleCORE moduleCore = new ModuleCORE();

        [Route("UpdateStateModule")]
        [HttpPost]
        public ResponseVM UpdateStateModule(ModuleBM moduleBM)
        {
            return moduleCore.UpdateStateModule(moduleBM);
        }

        [Route("GetModuleList")]
        [HttpPost]
        public List<ModuleVM> GetModuleList(ModuleBM moduleBM)
        {
            return moduleCore.GetModuleList(moduleBM);
        }

        [Route("GetModuleByCode")]
        [HttpPost]
        public List<ModuleVM> GetModuleByCode(ModuleBM moduleBM)
        {
            return moduleCore.GetModuleByCode(moduleBM);
        }

        [Route("UpdateModule")]
        [HttpPost]
        public ResponseVM UpdateModule(ModuleBM moduleBM)
        {
            return moduleCore.UpdateModule(moduleBM);
        }

        [Route("GetModuleCode")]
        [HttpPost]
        public ModuleVM GetModuleCode(ModuleBM moduleBM)
        {
            return moduleCore.GetModuleCode(moduleBM);
        }
    }
}