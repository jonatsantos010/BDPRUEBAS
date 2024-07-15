using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WSPlataforma.DA;
using WSPlataforma.Entities.AgencyModel.BindingModel;
using WSPlataforma.Entities.AgencyModel.ViewModel;

namespace WSPlataforma.Core
{
    public class AgencyCORE
    {
        AgencyDA DataAccessQuery = new AgencyDA();
        public GenericResponseVM GetBrokerAgencyList(AgencySearchBM dataToSearch)
        {
            return DataAccessQuery.GetBrokerAgencyList(dataToSearch);
        }

        public GenericResponseVM AddAgency(AgencyBM agencyData, HttpPostedFile file)
        {
            return DataAccessQuery.AddAgency(agencyData, file);
        }

        public GenericResponseVM GetLastBrokerList(String clientId, string nbranch, string nproduct, int limitPerpage, int pageNumber)
        {
            return DataAccessQuery.GetLastBrokerList(clientId, nbranch, nproduct, limitPerpage, pageNumber);
        }
    }
}
