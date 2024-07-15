using System.Collections.Generic;
using WSPlataforma.DA;
using WSPlataforma.Entities.ModuleModel.BindingModel;
using WSPlataforma.Entities.ModuleModel.ViewModel;
using WSPlataforma.Entities.ViewModel;

namespace WSPlataforma.Core
{
    public class ModuleCORE
    {
        ModuleDA DataAccessQuery = new ModuleDA();

        public ResponseVM UpdateStateModule(ModuleBM moduleBM)
        {
            return DataAccessQuery.UpdateStateModule(moduleBM);
        }

        public List<ModuleVM> GetModuleList(ModuleBM moduleBM)
        {
            return DataAccessQuery.GetModuleList(moduleBM);
        }

        public List<ModuleVM> GetModuleByCode(ModuleBM moduleBM)
        {
            return DataAccessQuery.GetModuleByCode(moduleBM);
        }

        public ResponseVM UpdateModule(ModuleBM moduleBM)
        {
            return DataAccessQuery.UpdateModule(moduleBM);
        }

        public ModuleVM GetModuleCode(ModuleBM moduleBM)
        {
            return DataAccessQuery.GetModuleCode(moduleBM);
        }
    }
}
