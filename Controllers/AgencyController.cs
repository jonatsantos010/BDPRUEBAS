using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WSPlataforma.Core;
using WSPlataforma.Entities.AgencyModel.BindingModel;
using WSPlataforma.Entities.AgencyModel.ViewModel;

namespace WSPlataforma.Controllers
{
    [RoutePrefix("Api/AgencyManager")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AgencyController : ApiController
    {
        AgencyCORE AgencyCORE = new AgencyCORE();

        [Route("GetBrokerAgencyList")]
        [HttpPost]
        public GenericResponseVM GetBrokerAgencyList(AgencySearchBM dataToSearch)
        {
            return AgencyCORE.GetBrokerAgencyList(dataToSearch);
        }

        [Route("AddAgency")]
        [HttpPost]
        public GenericResponseVM AddAgency()
        {
            try
            {

                HttpPostedFile file = null;
                if (HttpContext.Current.Request.Files.Count > 0)
                {
                    file = HttpContext.Current.Request.Files.Get("agencyFile");

                }

                var agencyData = JsonConvert.DeserializeObject<AgencyBM>(HttpContext.Current.Request.Params.Get("agencyData"));
                GenericResponseVM resp = AgencyCORE.AddAgency(agencyData, file);

                return resp;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [Route("GetLastBrokerList")]
        [HttpGet]
        public GenericResponseVM GetLastBrokerList(string clientId, string nbranch, string nproduct, int limitPerpage, int pageNumber)
        {
            return AgencyCORE.GetLastBrokerList(clientId, nbranch, nproduct, limitPerpage, pageNumber);
        }
    }
}