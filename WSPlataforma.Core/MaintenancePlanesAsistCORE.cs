using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.DA;
using WSPlataforma.Entities.MaintenancePlanesAsistModel.BindingModel;
using WSPlataforma.Entities.MaintenancePlanesAsistModel.ViewModel;
using WSPlataforma.Entities.CommissionModel.BindingModel;

namespace WSPlataforma.Core
{
    public class MaintenancePlanesAsistCORE
    {
        MaintenancePlanesAsistDA MaintenancePlanesAsistDA = new MaintenancePlanesAsistDA();

        public List<PlusListVM> GetServices()
        {
            return MaintenancePlanesAsistDA.GetServices();
        }
        public List<SimpleModelVM> GetBranch()
        {
            return MaintenancePlanesAsistDA.GetBranch();
        }
        public SimpleModelVM GetPlanes(Int64 poliza, int ramo, int producto)
        {
            return MaintenancePlanesAsistDA.GetPlanes(poliza, ramo, producto);
        }

        public List<SimpleModelVM> GetProductsById(int nBranch)
        {
            return MaintenancePlanesAsistDA.GetProductsById(nBranch);
        }
        public List<SimpleModelVM> GetTypeComiDif(int nFilter)
        {
            return MaintenancePlanesAsistDA.GetTypeComiDif(nFilter);
        }

        public List<SimpleModelVM> GetGroupByTypeComiDif(int nFilter)
        {
            return MaintenancePlanesAsistDA.GetGroupByTypeComiDif(nFilter);
        }
        public List<SimpleModelVM> GetRoles()
        {
            return MaintenancePlanesAsistDA.GetRoles();
        }
        public List<SimpleModelVM> GetCoverAdicional(int ramo, int producto)
        {
            return MaintenancePlanesAsistDA.GetCoverAdicional(ramo, producto);
        }
        public List<SimpleModelVM> Habilitar(long NPOLIZA, int NORDEN, int NOPCION)
        {
            return MaintenancePlanesAsistDA.Habilitar(NPOLIZA, NORDEN, NOPCION);
        }

        public List<SimpleModelVM> GetCampos(int ntipo, int ngrupo)
        {
            return MaintenancePlanesAsistDA.GetCampos(ntipo, ngrupo);
        }
        public List<SimpleModelVM> SetListaPlanes(List<PlanAsistVM> data, int ramo, int producto, long poliza, int NUSERCODE)
        {
            return MaintenancePlanesAsistDA.SetListaPlanes(data, ramo, producto, poliza, NUSERCODE);
        }
        public List<SimpleModelVM> ActualizaPlan(List<PlanAsistVM> data, int ramo, int producto, long poliza, int NUSERCODE)
        {
            return MaintenancePlanesAsistDA.ActualizaPlan(data, ramo, producto, poliza, NUSERCODE);
        }

        public List<PlanAsistVM> GetPlanesAsistencias(int nBranch, int nProduct, Int64 nPolicy, int ntipBusqueda, int nPlan)//INI <RQ2024-57 - 03/04/2024>
        {
            return MaintenancePlanesAsistDA.GetPlanesAsistencias(nBranch, nProduct, nPolicy, ntipBusqueda, nPlan);
        }

        public List<ServicesVM> GetServicioXPlan(int nPlan, int nUsercode, int ramo, int producto, long poliza)
        {
            return MaintenancePlanesAsistDA.GetServicioXPlan(nPlan, nUsercode, ramo, producto, poliza);
        }
        public List<SimpleModelVM> ValidaPoliza(int nBranch, Int64 nPolicy, int nProduct)
        {
            return MaintenancePlanesAsistDA.ValidaPoliza(nBranch, nPolicy, nProduct);
        }
        public List<SimpleModelVM> GetTipBusquedaList()//INI <RQ2024-57 - 03/04/2024>
        {
            return MaintenancePlanesAsistDA.GetTipBusquedaList();
        }
        public List<SimpleModelVM> GetDivisorList()//INI <RQ2024-57 - 03/04/2024>
        {
            return MaintenancePlanesAsistDA.GetDivisorList();
        }

        public SimpleModelVM setListaTasa(PlanAsistVM data)//INI <RQ2024-57 - 03/04/2024>
        {
            return MaintenancePlanesAsistDA.setListaTasa(data);
        }

        public PlanAsistVM getEdiTasaPol(int nBranch, int nProduct, Int64 nPolicy, int nModulo, int nRol)//INI <RQ2024-57 - 03/04/2024>
        {
            return MaintenancePlanesAsistDA.getEdiTasaPol(nBranch, nProduct, nPolicy, nModulo, nRol);
        }

        public SimpleModelVM actualizarTasa(PlanAsistVM data)//INI <RQ2024-57 - 03/04/2024>
        {
            return MaintenancePlanesAsistDA.actualizarTasa(data);
        }

    }
}
