using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.DA;
using WSPlataforma.Entities.MaintenanceModel.ViewModel;
using WSPlataforma.Entities.MaintenanceModel.BindingModel;
using WSPlataforma.Entities.CommissionModel.BindingModel;

namespace WSPlataforma.Core
{
    public class MaintenanceCORE
    {
        MaintenanceDA maintenanceDA = new MaintenanceDA();

        public List<TransactionVM> GetTranscactionsByProduct(int nBranch, int nProduct)
        {
            return maintenanceDA.GetTranscactionsByProduct(nBranch, nProduct);
        }

        public List<ParameterVM> GetParametersByTransaction(int nBranch, int nProduct, int nTypeTransac, int nPerfil, int nParam, string nGob)
        {
            return maintenanceDA.GetParametersByTransaction(nBranch, nProduct, nTypeTransac, nPerfil, nParam, nGob);
        }

        public List<ParameterVM> GetComboParametersByTransaction(int nBranch, int nProduct, int nTypeTransac, int nPerfil)
        {
            return maintenanceDA.GetComboParametersByTransaction(nBranch, nProduct, nTypeTransac, nPerfil);
        }

        public GenericResponse InsertRma(RmaBM data)
        {
            return maintenanceDA.InsertRma(data);
        }

        public List<RmaVM> GetRma(RmaVM rma)
        {
            return maintenanceDA.GetRma(rma);
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
        }

        public List<PorcentajeRMV> GetPorcentajeRmv(PorcentajeRMV rma)
        {
            return maintenanceDA.GetPorcentajeRmv(rma);
        }

        public List<TypeRiskVM> GetTypeRiskVM(TypeRiskVM riesgo)
        {
            return maintenanceDA.GetTypeRiskVM(riesgo);
        }

        public GenericResponse UpdatePorcentajeRMV(PorcentajeRMV riesgo)
        {
            return maintenanceDA.UpdatePorcentajeRMV(riesgo);
        }

        public List<PorcentajeRMV> GetHistorialPorcentajeRMV(PorcentajeRMV riesgo)
        {
            return maintenanceDA.GetHistorialPorcentajeRMV(riesgo);
        }
    }
}
