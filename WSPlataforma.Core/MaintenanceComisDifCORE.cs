using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.DA;
using WSPlataforma.Entities.MaintenanceComisDifModel.ViewModel;
using WSPlataforma.Entities.MaintenanceComisDifModel.BindingModel;
using WSPlataforma.Entities.CommissionModel.BindingModel;

namespace WSPlataforma.Core
{
    public class MaintenanceComiDifCORE
    {
        MaintenanceComisDifDA maintenanceComisDifDA = new MaintenanceComisDifDA();
        
        public List<SimpleModelVM> GetSAList()
        {
            return maintenanceComisDifDA.GetSAList();
        }
        public List<SimpleModelVM> GetBranch()
        {
            return maintenanceComisDifDA.GetBranch();
        }

        public List<SimpleModelVM> GetProductsById(int nBranch)
        {
            return maintenanceComisDifDA.GetProductsById(nBranch);
        }
        public List<SimpleModelVM> GetTypeComiDif(int nFilter)
        {
            return maintenanceComisDifDA.GetTypeComiDif(nFilter);
        }

        public List<SimpleModelVM> GetGroupByTypeComiDif(int nFilter)
        {
            return maintenanceComisDifDA.GetGroupByTypeComiDif(nFilter);
        }
        public List<SimpleModelVM> GetRolesXPol(long poliza, int ramo, int producto)
        {
            return maintenanceComisDifDA.GetRolesXPol(poliza, ramo, producto);
        }
        public List<SimpleModelVM> GetModulosXPol(long poliza, int ramo, int producto)
        {
            return maintenanceComisDifDA.GetModulosXPol(poliza, ramo, producto);
        }
        public List<SimpleModelVM> Habilitar(long NPOLIZA, int NOPCION)
        {
            return maintenanceComisDifDA.Habilitar(NPOLIZA, NOPCION);
        }

        public List<SimpleModelVM> GetCampos(int ntipo, int ngrupo)
        {
            return maintenanceComisDifDA.GetCampos(ntipo, ngrupo);
        }
        public List<SimpleModelVM> setListaComisiones(List<ComisDifVM> data, int ramo, int producto, int tipo, int grupo, long poliza, int NUSERCODE)
        {
            return maintenanceComisDifDA.SetListaComisiones(data, ramo, producto, tipo, grupo, poliza, NUSERCODE);
        }
        public List<SimpleModelVM> ActualizaComision(List<ComisDifVM> data, int ramo, int producto, int tipo, int grupo, long poliza, int NUSERCODE)
        {
            return maintenanceComisDifDA.ActualizaComision(data, ramo, producto, tipo, grupo, poliza, NUSERCODE);
        }

        public SimpleModelVM EliminaConfig(int nBranch, int nProduct, Int64 nPolicy, int nOrden)
        {
            return maintenanceComisDifDA.EliminaConfig(nBranch, nProduct, nPolicy, nOrden);
        }
        public List<SimpleModelVM> ValidaPoliza(int nBranch, int nProduct, Int64 nPolicy)
        {
            return maintenanceComisDifDA.ValidaPoliza(nBranch, nProduct, nPolicy);
        }
        public List<ComisDifVM> GetComisDifConfigs(Int32 nBranch, Int32 nProduct, Int64 nPolicy)
        {
            return maintenanceComisDifDA.GetComisDifConfigs(nBranch, nProduct, nPolicy);
        }

        /*
        public GenericResponse InsertRma(RmaBM data)
        {
            return maintenanceComisDifDA.InsertRma(data);
        }
        public List<RmaVM> GetComisDifConfigs(RmaVM rma)
        {
            return maintenanceDA.GetComisDifConfigs(rma);
        }

        public List<ActivityVM> GetActivityList(ActivityBM data)
        {
            return maintenanceDA.GetActivityList(data);
        }

        public List<ActivityVM> GetActivityListMovement(ActivityBM data)
        {
            return maintenanceDA.GetActivityListMovement(data);
        }

        public GenericResponse InsertActivities(ActivityParamsBM data)
        {
            return maintenanceDA.InsertActivities(data);
        }

        public List<ActivityParamsBM> GetCombActivities(ActivityParamsBM data)
        {
            return maintenanceDA.GetCombActivities(data);
        }

        public void InsertCombActivityDet(ActivityParamsBM data)
        {
            maintenanceDA.InsertCombActivityDet(data);
        }

        public GenericResponse InsertRetroactivity(RetroactivityBM data)
        {
            return maintenanceDA.InsertRetroactivity(data);
        }

        public List<RetroactivityVM> GetProfiles(FiltersBM data)
        {
            return maintenanceDA.GetProfiles(data);
        }

        public string getProfileProduct(FiltersBM data)
        {
            return maintenanceDA.getProfileProduct(data);
        }

        public List<RetroactivityVM> GetConfigDays()
        {
            return maintenanceDA.GetConfigDays();
        }

        public List<RetroactivityVM> GetRetroactivityList(FiltersBM data)
        {
            return maintenanceDA.GetRetroactivityList(data);
        }

        public string CalcFinVigencia(string inicioVigencia)
        {
            return maintenanceDA.CalcFinVigencia(inicioVigencia);
        }

        public List<RetroactivityMessageVM> GetRetroactivityMessage(FiltersBM data)
        {
            return maintenanceDA.GetRetroactivityMessage(data);
        }

        public GenericResponse getUserAccess(FiltersBM data)
        {
            return maintenanceDA.getUserAccess(data);
        }*/
    }
}
