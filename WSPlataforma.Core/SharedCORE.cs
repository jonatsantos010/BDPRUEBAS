using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WSPlataforma.DA;
using WSPlataforma.Entities.DemandProcessModel.ViewModel;
using WSPlataforma.Entities.HistoryCreditServiceModel.BindingModel;
using VPolicy = WSPlataforma.Entities.PolicyModel.ViewModel;
using WSPlataforma.Entities.SharedModel.BindingModel;
using WSPlataforma.Entities.SharedModel.ViewModel;

namespace WSPlataforma.Core
{

    public class SharedCORE
    {
        SharedDA DataAccessQuery = new SharedDA();

        //INI MARC
        public double GetExperiaScore(string sclient)
        {
            return DataAccessQuery.GetExperiaScore(sclient);
        }
        public string GetMailFE()
        {
            return DataAccessQuery.GetMailFE();
        }
        public List<DocumentTypeSntVM> GetDocumentTypeSntList()
        {
            return DataAccessQuery.GetDocumentTypeSntList();
        }
        public string GetCurrencyDescription(decimal NCODIGINT)
        {
            return DataAccessQuery.GetCurrencyDescription(NCODIGINT);
        }

        public string GetRechargeDescription(decimal NCODREC)
        {
            return DataAccessQuery.GetRechargeDescription(NCODREC);
        }

        public List<CoverTypeVM> GetCoverTypeList()
        {
            return DataAccessQuery.GetCoverTypeList();
        }

        public List<RechargeTypeVM> GetRechargeTypeList()
        {
            return DataAccessQuery.GetRechargeTypeList();
        }

        public List<SinisterVM> GetSinisterList()
        {
            return DataAccessQuery.GetSinisterList();
        }

        public List<GenericVM> GetGenericList()
        {
            return DataAccessQuery.GetGenericList();
        }

        public bool ValExperiaScore(ConsultBM request)
        {
            return DataAccessQuery.ValExperiaScore(request);
        }

        public List<ClasificationVM> GetClasificationList()
        {
            return DataAccessQuery.GetClasificationList();
        }

        public List<ReInsuranceVM> GetReInsuranceList()
        {
            return DataAccessQuery.GetReInsuranceList();
        }

        public List<ContabilityVM> GetContabilityList()
        {
            return DataAccessQuery.GetContabilityList();
        }

        public List<MortalityVM> GetMortalityList()
        {
            return DataAccessQuery.GetMortalityList();
        }

        public List<StateVM> GetStateList()
        {
            return DataAccessQuery.GetStateList();
        }

        public List<BranchVM> GetBranchList(string branch)
        {
            return DataAccessQuery.GetBranchList(branch);
        }

        public List<BranchVM> GetBranches(string productId, string epsId)
        {
            return DataAccessQuery.GetBranches(productId, epsId);
        }

        public List<ProductVM> GetProductsListByBranch(string P_NBRANCH)
        {
            return DataAccessQuery.GetProductsListByBranch(P_NBRANCH);
        }
        //FIN MARC

        public List<PersonTypeVM> GetPersonTypeList()
        {
            return DataAccessQuery.GetPersonTypeList();
        }
        public List<DocumentTypeVM> GetDocumentTypeList(string codProducto)
        {
            return DataAccessQuery.GetDocumentTypeList(codProducto);
        }

        public List<ProfileCampVM> GetProfileEsp()
        {
            return DataAccessQuery.GetProfileEsp();
        }

        public List<DistrictVM> GetDistrictList(Int32 provinceId)
        {
            return DataAccessQuery.GetDistrictList(provinceId);
        }
        public List<ProvinceVM> GetProvinceList(Int32 departmentId)
        {
            return DataAccessQuery.GetProvinceList(departmentId);
        }
        public List<DepartmentVM> GetDepartmentList()
        {
            return DataAccessQuery.GetDepartmentList();
        }
        public List<EconomicActivityVM> GetEconomicActivityList(string technicalActivityId)
        {
            return DataAccessQuery.GetEconomicActivityList(technicalActivityId);
        }
        public List<NationalityVM> GetNationalityList()
        {
            return DataAccessQuery.GetNationalityList();
        }
        public List<GenderVM> GetGenderList()
        {
            return DataAccessQuery.GetGenderList();
        }
        public List<CivilStatusVM> GetCivilStatusList()
        {
            return DataAccessQuery.GetCivilStatusList();
        }
        public List<ProfessionVM> GetProfessionList()
        {
            return DataAccessQuery.GetProfessionList();
        }
        public List<PhoneTypeVM> GetPhoneTypeList()
        {
            return DataAccessQuery.GetPhoneTypeList();
        }
        public List<CityCodeVM> GetCityCodeList()
        {
            return DataAccessQuery.GetCityCodeList();
        }
        public List<EmailTypeVM> GetEmailTypeList()
        {
            return DataAccessQuery.GetEmailTypeList();
        }
        public List<DirectionTypeVM> GetDirectionTypeList()
        {
            return DataAccessQuery.GetDirectionTypeList();
        }
        public List<RoadTypeVM> GetRoadTypeList()
        {
            return DataAccessQuery.GetRoadTypeList();
        }
        public List<InteriorTypeVM> GetInteriorTypeList()
        {
            return DataAccessQuery.GetInteriorTypeList();
        }
        public List<BlockTypeVM> GetBlockTypeList()
        {
            return DataAccessQuery.GetBlockTypeList();
        }
        public List<CJHTTypeVM> GetCJHTTypeList()
        {
            return DataAccessQuery.GetCJHTTypeList();
        }
        public List<CountryVM> GetCountryList()
        {
            return DataAccessQuery.GetCountryList();
        }
        public List<ContactTypeVM> GetContactTypeList(ContactTypeBM data)
        {
            return DataAccessQuery.GetContactTypeList(data);
        }
        public List<CurrencyVM> GetCurrencyList(int nbranch, int nproduct)
        {
            return DataAccessQuery.GetCurrencyList(nbranch, nproduct);
        }
        public List<CiiuVM> GetCiiuList()
        {
            return DataAccessQuery.GetCiiuList();
        }
        public ResponseVM ValPhone(PhoneBM data)
        {
            return DataAccessQuery.ValPhone(data);
        }
        public ResponseVM ValCiiu(CiiuBM data)
        {
            return DataAccessQuery.ValCiiu(data);
        }
        public ResponseVM ValEmail(EmailBM data)
        {
            return DataAccessQuery.ValEmail(data);
        }
        public ResponseVM ValStreet(AddressBM data)
        {
            return DataAccessQuery.ValStreet(data);
        }
        public ResponseVM ValContact(ContactBM data)
        {
            return DataAccessQuery.ValContact(data);
        }
        public List<ProductVM> GetProducts(string productId, string epsId, string branch)
        {
            return DataAccessQuery.GetProducts(productId, epsId, branch);
        }

        public List<ProductVM> GetListProductsForBranch(string branch)
        {
            return DataAccessQuery.GetListProductsForBranch(branch);
        }

        public List<TechnicalActivityVM> GetTechnicalActivityList(int codProducto)
        {
            return DataAccessQuery.GetTechnicalActivityList(codProducto);
        }

        public List<TechnicalActivityVM> GetOccupationsList()
        {
            return DataAccessQuery.GetOccupationsList();
        }

        //---------------------------------------------------------
        public List<TechnicalActivityVM> GetActivityDesgravamenList()
        {
            return DataAccessQuery.GetActivityDesgravamenList();
        }
        //---------------------------------------------------------

        public async Task<PaymentVM> getFormaPago(PaymentModel model)
        {
            return await DataAccessQuery.getFormaPago(model);
        }

        public List<TypeFacturaVM> GetTypeFactura(string codproducto, int perfil)
        {
            return DataAccessQuery.GetTypeFactura(codproducto, perfil);
        }
        //public List<BranchVM> GetBranchList()
        //{
        //    return DataAccessQuery.GetBranchList();
        //}

        public string GetUserProfile(int usercode, int product)
        {
            return DataAccessQuery.GetUserProfile(usercode, product);
        }

        public List<TypeFacturaVM> getInfoTable(int idTable)
        {
            return DataAccessQuery.getInfoTable(idTable);
        }

        public async Task<List<TypeFacturaVM>> getAlcance(string nbranch)
        {
            var response = new List<TypeFacturaVM>();

            var responseGraph = await new GraphqlDA().getAlcance(nbranch);

            if (responseGraph.codError == 0)
            {
                foreach (var scope in responseGraph.data.coverageScopes)
                {
                    var item = new TypeFacturaVM();
                    item.id = Convert.ToInt32(scope.id);
                    item.descripcion = scope.description;
                    response.Add(item);
                }
            }

            return response;
        }

        public async Task<List<DepartmentVM>> GetDepartamentos()
        {
            var response = new List<DepartmentVM>();

            var responseGraph = await new GraphqlDA().GetDepartamentos();

            if (responseGraph.codError == 0)
            {
                foreach (var departament in responseGraph.data.departments.items)
                {
                    var item = new DepartmentVM();
                    item.Id = Convert.ToInt32(departament.id);
                    item.Name = departament.description;
                    response.Add(item);
                }
            }

            return response;
        }

        public List<BrokDepVM> getBrokerDep(int nbranch, int nproduct)
        {
            return DataAccessQuery.getBrokerDep(nbranch, nproduct);
        }

        public VPolicy.SalidadPolizaEmit insertNCTemp(List<NotaCreditoModel> data)
        {
            return DataAccessQuery.insertNCTemp(data);
        }

        public List<NotaCreditoVM> GetNotaCreditoList(string P_CONTRATANTE, string P_REC_NC, int P_TIPO, int P_TIPO_MONEDA)
        {
            return DataAccessQuery.GetNotaCreditoList(P_CONTRATANTE, P_REC_NC, P_TIPO, P_TIPO_MONEDA);
        }

        public ErrorCode NCParcialVISA(EntidadParcialVisa data)
        {
            return DataAccessQuery.NCParcialVISA(data);
        }

        public ErrorCode NCParcialPF(EntidadParcialPF data)
        {
            return DataAccessQuery.NCParcialPF(data);
        }

    }
}