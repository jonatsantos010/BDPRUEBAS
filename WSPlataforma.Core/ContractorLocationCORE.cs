using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WSPlataforma.DA;
using WSPlataforma.Entities.BindingModel;
using WSPlataforma.Entities.ContractorLocationModel.ViewModel;
//using WSPlataforma.Entities.PolicyModel.BindingModel;
using WSPlataforma.Entities.ViewModel;

namespace WSPlataforma.Core
{
    public class ContractorLocationCORE
    {
        ContractorLocationDA DataAccessQuery = new ContractorLocationDA();

        public ResponseVM GetContractorLocationList(string contractorId, Int32 limitPerPage, Int32 pageNumber)
        {
            return DataAccessQuery.GetContractorLocationList(contractorId, limitPerPage, pageNumber);
        }

        public ContractorLocationVM GetContractorLocation(Int32 contractorLocationId)
        {
            return DataAccessQuery.GetContractorLocation(contractorLocationId);
        }
        public ResponseVM GetContractorLocationContactList(Int32 contractorLocationId, Int32 limitPerPage, Int32 pageNumber)
        {
            return DataAccessQuery.GetContractorLocationContactList(contractorLocationId, limitPerPage, pageNumber);
        }
        public List<ContractorLocationTypeVM> GetContractorLocationTypeList()
        {
            return DataAccessQuery.GetContractorLocationTypeList();
        }
        public ContractorVM GetContractor(string contractorId)
        {
            return DataAccessQuery.GetContractor(contractorId);
        }
        public ContractorLocationContactVM GetContractorLocationContact(Int32 contractorLocationContactId)
        {
            return DataAccessQuery.GetContractorLocationContact(contractorLocationContactId);
        }
        public ResponseVM UpdateContractorLocation(ContractorLocationBM contractorLocation)
        {
            return DataAccessQuery.UpdateContractorLocation(contractorLocation);
        }
        public ResponseVM UpdateContractorLocationContact(ContractorLocationContactBM contact)
        {
            return DataAccessQuery.UpdateContractorLocationContact(contact);
        }
        public ResponseVM DeleteContact(Int32 contactId, Int32 userCode)
        {
            return DataAccessQuery.DeleteContact(contactId, userCode);
        }
        public ResponseVM GetSuggestedLocationType(string contractorId, Int32 userCode) // 1:error , 2: crear sucursal, 3: crear principal
        {
            return DataAccessQuery.GetSuggestedLocationType(contractorId, userCode);
        }
        public ResponseVM DeleteClientLocation(Int32 locationId, Int32 userCode)
        {
            return DataAccessQuery.DeleteClientLocation(locationId, userCode);
        }
        public ResponseVM HasActivePrincipalLocation(string contractorId, Int32 locationIdToIgnore, Int32 userCode)
        {
            return DataAccessQuery.HasActivePrincipalLocation(contractorId, locationIdToIgnore, userCode);
        }
    }
}
