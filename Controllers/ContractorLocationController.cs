using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Cors;
using WSPlataforma.Core;
using WSPlataforma.Entities.BindingModel;
using WSPlataforma.Entities.ViewModel;

namespace WSPlataforma.Controllers
{
    [RoutePrefix("Api/ContractorLocationManager")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ContractorLocationController : ApiController
    {

        ContractorLocationCORE contractorLocationCore = new ContractorLocationCORE();

        [Route("GetContractorLocationList")]
        [HttpGet]
        public ResponseVM GetContractorLocationList(string contractorId, Int32 limitPerPage, Int32 pageNumber)
        {
            return contractorLocationCore.GetContractorLocationList(contractorId, limitPerPage, pageNumber);
        }
        [Route("GetContractorLocation")]
        [HttpGet]
        public ContractorLocationVM GetContractorLocation(Int32 contractorLocationId)
        {
            return contractorLocationCore.GetContractorLocation(contractorLocationId);
        }

        [Route("GetContractorLocationTypeList")]
        [HttpGet]
        public List<ContractorLocationTypeVM> GetContractorLocationTypeList()
        {
            return contractorLocationCore.GetContractorLocationTypeList();
        }
        [Route("GetContractorLocationContactList")]
        [HttpGet]
        public ResponseVM GetContractorLocationContactList(Int32 contractorLocationId, Int32 limitPerPage, Int32 pageNumber)
        {
            return contractorLocationCore.GetContractorLocationContactList(contractorLocationId, limitPerPage, pageNumber);
        }
        [Route("GetContractorLocationContact")]
        [HttpGet]
        public ContractorLocationContactVM GetContractorLocationContact(Int32 contractorLocationContactId)
        {
            return contractorLocationCore.GetContractorLocationContact(contractorLocationContactId);
        }
        [Route("GetContractor")]
        [HttpGet]
        public ContractorVM GetContractor(string contractorId)
        {
            return contractorLocationCore.GetContractor(contractorId);
        }
        [Route("UpdateContractorLocation")]
        [HttpPost]
        public ResponseVM UpdateContractorLocation(ContractorLocationBM contractorLocation)
        {
            return contractorLocationCore.UpdateContractorLocation(contractorLocation);
        }
        [Route("UpdateContractorLocationContact")]
        [HttpPost]
        public ResponseVM UpdateContractorLocationContact(ContractorLocationContactBM contact)
        {
            return contractorLocationCore.UpdateContractorLocationContact(contact);
        }
        [Route("DeleteContact")]
        [HttpGet]
        public ResponseVM DeleteContact(Int32 contactId, Int32 userCode)
        {
            return contractorLocationCore.DeleteContact(contactId, userCode);
        }
        [Route("GetSuggestedLocationType")]
        [HttpGet]
        public ResponseVM GetSuggestedLocationType(string contractorId, Int32 userCode) // 1:error , 2: crear sucursal, 3: crear principal
        {
            return contractorLocationCore.GetSuggestedLocationType(contractorId, userCode);
        }
        [Route("DeleteClientLocation")]
        [HttpGet]
        public ResponseVM DeleteClientLocation(Int32 locationId, Int32 userCode)
        {
            return contractorLocationCore.DeleteClientLocation(locationId, userCode);
        }
        [Route("HasActivePrincipalLocation")]
        [HttpGet]
        public ResponseVM HasActivePrincipalLocation(string contractorId, Int32 locationIdToIgnore, Int32 userCode)
        {
            return contractorLocationCore.HasActivePrincipalLocation(contractorId, locationIdToIgnore, userCode);
        }
    }
}