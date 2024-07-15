using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.DA;
using WSPlataforma.Entities.TransactModel;

namespace WSPlataforma.Core
{
    public class TransactCORE
    {
        TransactDA transactDA = new TransactDA();
        public TransactResponse InsertTransact(RequestTransact request)
        {
            return transactDA.InsertTransact(request);
        }
        public List<UserTransact> GetUsersTransact(RequestTransact request)
        {
            return transactDA.GetUsersTransact(request);
        }
        public TransactResponse AsignarTransact(RequestTransact request)
        {
            return transactDA.AsignarTransact(request);
        }
        public List<TransactModel> GetHistTransact(RequestTransact request)
        {
            return transactDA.GetHistTransact(request);
        }
        public TransactResponse InsertDerivarTransact(RequestTransact request)
        {
            return transactDA.InsertDerivarTransact(request);
        }
        public List<StatusTransact> GetStatusListTransact()
        {
            return transactDA.GetStatusListTransact();
        }
        public GenericResponseTransact GetTransactList(TransactSearchRequest dataToSearch)
        {
            return transactDA.GetTransactList(dataToSearch);
        }
        public string GetExcelTransactList(TransactSearchRequest dataToSearch)
        {
            return transactDA.GetExcelTransactList(dataToSearch);
        }
        public TransactModel GetInfoTransact(RequestTransact request)
        {
            return transactDA.GetInfoTransact(request);
        }
        public TransactResponse InsertHistTransact(RequestTransact request)
        {
            return transactDA.InsertHistTransact(request);
        }
        public TransactResponse UpdateTransact(RequestTransact request)
        {
            return transactDA.UpdateTransact(request);
        }
        public TransactModel GetInfoLastTransact(RequestTransact request)
        {
            return transactDA.GetInfoLastTransact(request);
        }
        public TransactResponse UpdateBroker(RequestTransact request)
        {
            return transactDA.UpdateBroker(request);
        }
        public TransactResponse ValidateAccess(RequestTransact request)
        {
            return transactDA.ValidateAccess(request);
        }
        public TransactResponse ValidateAccessDes(RequestTransact request)
        {
            return transactDA.ValidateAccessDes(request);
        }
        public GenericResponseTransact GetVigenciaAnterior(RequestTransact request)
        {
            return transactDA.GetVigenciaAnterior(request);
        }
        public TransactResponse FinalizarTramite(RequestTransact request)
        {
            return transactDA.FinalizarTramite(request);
        }
        public TransactResponse AnularTramite(RequestTransact request)
        {
            return transactDA.AnularTramite(request);
        }
        public bool UpdateDevolvPri(int cotizacion, int devolvPri)
        {
            return transactDA.UpdateDevolvPri(cotizacion, devolvPri);
        }
        public TransactResponse PerfilTramiteOpe(RequestTransact request)
        {
            return transactDA.PerfilTramiteOpe(request);
        }
        public TransactResponse PerfilComercialEx(RequestTransact request)
        {
            return transactDA.PerfilComercialEx(request);
        }

        public TransactResponse UpdateMail(RequestTransact request)
        {
            return transactDA.UpdateMail(request);
        }

        public TransactResponse InsReingresarTransact(RequestTransact request)
        {
            return transactDA.InsReingresarTransact(request);
        }
        //R.P.
        public List<BrokerObl> GetBrokerObl(int nbranch)
        {
            return transactDA.GetBrokerObl(nbranch);
        }

        public TransactResponse InsertDepBrkTramite(DepBroker databrkdep)
        {
            return transactDA.InsertDepBrkTramite(databrkdep);
        }
        //R.P.

        public List<MotivoRechazoVM> GetMotivoRechazoTransact()
        {
            return transactDA.GetMotivoRechazoTransact();
        }

       public TransactResponse ValidarRechazoEjecutivo(ValidateEjecutivoTransac request)
        {
            return transactDA.ValidarRechazoEjecutivo(request);
        }
    }
}
