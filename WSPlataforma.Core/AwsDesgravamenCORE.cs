using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.DA;
using WSPlataforma.Entities.AwsDesgravamenModel.ViewModel;
using WSPlataforma.DA;
//using WSPlataforma.Entities.MaintenancePlanesAsistModel.ViewModel;

namespace WSPlataforma.Core
{
    public class AwsDesgravamenCORE
    {
        AwsDesgravamenDA awsDesgravamenDA = new AwsDesgravamenDA();

        public List<HorariosVM> getHorasList()
        {
            return awsDesgravamenDA.getHorasList();
        }
        public List<SimpleModelVM> GetBranch()
        {
            return awsDesgravamenDA.GetBranch();
        }

        public List<SimpleModelVM> GetDiaList()
        {
            return awsDesgravamenDA.GetDiaList();
        }

        public List<SimpleModelVM> GetHoraList()
        {
            return awsDesgravamenDA.GetHoraList();
        }

        public List<SimpleModelVM> getMinutoList()
        {
            return awsDesgravamenDA.getMinutoList();
        }

        public List<SimpleModelVM> GetProductsById(int nBranch)
        {
            return awsDesgravamenDA.GetProductsById(nBranch);
        }

        public SimpleModelVM nuevoHorario(HorariosVM data)
        {
            return awsDesgravamenDA.nuevoHorario(data);
        }

        public SimpleModelVM actualizarHorario(HorariosVM data)
        {
            return awsDesgravamenDA.actualizarHorario(data);
        }

        public SimpleModelVM getFacSuspendida(int NPOLICY)//INI <RQ2024-57 - 03/04/2024>
        {
            return awsDesgravamenDA.getFacSuspendida(NPOLICY);
        }

        public SimpleModelVM BorrarHorario(int NBRANCH, int NPRODUCT, int NDIA)
        {
            return awsDesgravamenDA.BorrarHorario(NBRANCH, NPRODUCT, NDIA);
        }

    }
}
