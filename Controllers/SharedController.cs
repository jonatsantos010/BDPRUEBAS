using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WSPlataforma.Core;
using WSPlataforma.Entities.DemandProcessModel.ViewModel;
using WSPlataforma.Entities.ProductModel.ViewModel;
using WSPlataforma.Entities.SharedModel.BindingModel;
using WSPlataforma.Entities.SharedModel.ViewModel;
using System.Linq;
using WSPlataforma.Util;

namespace WSPlataforma.Controllers
{

    [RoutePrefix("Api/SharedManager")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SharedController : ApiController
    {
        SharedCORE sharedCore = new SharedCORE();
        //INI TEST PRINT
        PrintPolicyCORE printPolicyCore = new PrintPolicyCORE();
        //END TEST PRINT
        //INI MARC

        [Route("GetMailFE")]
        [HttpGet]
        public String GetMailFE()
        {
            return sharedCore.GetMailFE();
        }
        [Route("GetDocumentTypeSntList")]
        [HttpGet]
        public List<DocumentTypeSntVM> GetDocumentTypeSntList()
        {
            return sharedCore.GetDocumentTypeSntList();
        }

        [Route("GetCurrencyDescription")]
        [HttpGet]
        public String GetCurrencyDescription(decimal NCODIGINT)
        {
            return sharedCore.GetCurrencyDescription(NCODIGINT);
        }

        [Route("GetRechargeDescription")]
        [HttpGet]
        public String GetRechargeDescription(decimal NCODREC)
        {
            return sharedCore.GetRechargeDescription(NCODREC);
        }

        [Route("GetCoverTypeList")]
        [HttpGet]
        public List<CoverTypeVM> GetCoverTypeList()
        {
            return sharedCore.GetCoverTypeList();
        }

        [Route("GetRechargeTypeList")]
        [HttpGet]
        public List<RechargeTypeVM> GetRechargeTypeList()
        {
            return sharedCore.GetRechargeTypeList();
        }

        [Route("GetSinisterList")]
        [HttpGet]
        public List<SinisterVM> GetSinisterList()
        {
            return sharedCore.GetSinisterList();
        }

        [Route("GetGenericList")]
        [HttpGet]
        public List<GenericVM> GetGenericList()
        {
            return sharedCore.GetGenericList();
        }

        [Route("GetClasificationList")]
        [HttpGet]
        public List<ClasificationVM> GetClasificationList()
        {
            return sharedCore.GetClasificationList();
        }

        [Route("GetReInsuranceList")]
        [HttpGet]
        public List<ReInsuranceVM> GetReInsuranceList()
        {
            return sharedCore.GetReInsuranceList();
        }

        [Route("GetContabilityList")]
        [HttpGet]
        public List<ContabilityVM> GetContabilityList()
        {
            return sharedCore.GetContabilityList();
        }

        [Route("GetMortalityList")]
        [HttpGet]
        public List<MortalityVM> GetMortalityList()
        {
            return sharedCore.GetMortalityList();
        }

        [Route("GetStateList")]
        [HttpGet]
        public List<StateVM> GetStateList()
        {
            return sharedCore.GetStateList();
        }

        [Route("GetBranchList")]
        [HttpGet]
        public List<BranchVM> GetBranchList(string branch = null)
        {
            return sharedCore.GetBranchList(branch);
        }

        [Route("GetBranches")]
        [HttpGet]
        public List<BranchVM> GetBranches(string productId, string epsId)
        {
            return sharedCore.GetBranches(productId, epsId);
        }


        [Route("GetProductsListByBranch")]
        [HttpGet]
        public List<ProductVM> GetProductsListByBranch(string P_NBRANCH)
        {
            return sharedCore.GetProductsListByBranch(P_NBRANCH);
        }

        //FIN MARC

        [Route("GetPersonTypeList")]
        [HttpGet]
        public List<PersonTypeVM> GetPersonTypeList()
        {
            return sharedCore.GetPersonTypeList();
        }
        [Route("GetDocumentTypeList")]
        [HttpGet]
        public List<DocumentTypeVM> GetDocumentTypeList(string codProducto)
        {
            return sharedCore.GetDocumentTypeList(codProducto);
        }
        [Route("GetProfileEsp")]
        [HttpGet]
        public List<ProfileCampVM> GetProfileEsp()
        {
            return sharedCore.GetProfileEsp();
        }

        [Route("GetDistrictList")]
        [HttpGet]
        public List<DistrictVM> GetDistrictList(Int32 provinceId)
        {
            return sharedCore.GetDistrictList(provinceId);
        }
        [Route("GetProvinceList")]
        [HttpGet]
        public List<ProvinceVM> GetProvinceList(Int32 departmentId)
        {
            return sharedCore.GetProvinceList(departmentId);
        }
        [Route("GetDepartmentList")]
        [HttpGet]
        public List<DepartmentVM> GetDepartmentList()
        {
            return sharedCore.GetDepartmentList();
        }

        [Route("GetTechnicalActivityList")]
        [HttpGet]
        public List<TechnicalActivityVM> GetTechnicalActivityList(int codProducto = 2)
        {
            return sharedCore.GetTechnicalActivityList(codProducto);
        }

        [Route("GetEconomicActivityList")]
        [HttpGet]
        public List<EconomicActivityVM> GetEconomicActivityList(string technicalActivityId)
        {
            return sharedCore.GetEconomicActivityList(technicalActivityId);
        }
        [Route("GetEconomicActivityList")]
        [HttpGet]
        public List<EconomicActivityVM> GetEconomicActivityList()
        {
            return sharedCore.GetEconomicActivityList("");
        }

        [Route("GetOccupationsList")]
        [HttpGet]
        public List<TechnicalActivityVM> GetOccupationsList()
        {
            return sharedCore.GetOccupationsList();
        }

        //----------------------------------------------------
        [Route("GetActivityDesgravamenList")]
        [HttpGet]
        public List<TechnicalActivityVM> GetActivityDesgravamenList()
        {
            return sharedCore.GetActivityDesgravamenList();
        }
        //----------------------------------------------------

        [Route("GetTypeFactura")]
        [HttpGet]
        public List<TypeFacturaVM> GetTypeFactura(string codproducto, int perfil = 0)
        {
            return sharedCore.GetTypeFactura(codproducto, perfil);
        }

        [HttpPost]
        [Route("UploadFile")]
        public async Task<GenericResponseVM> UploadFile(String fileName, String rootName)
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                var response = new GenericResponseVM();
                response.StatusCode = 1;
                response.Message = "El requerimiento no contiene un contenido válido.";
                return response;
            }
            String filePath = "C:\\Laboratorio\\";  //Directorio raiz de proyecto

            switch (rootName)
            {
                case "agency":
                    filePath = filePath + "Agency\\";
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }
                    break;
                case "otro":

                    break;
                default:
                    var response = new GenericResponseVM();
                    response.StatusCode = 1;
                    response.Message = "EL directorio no es válido";
                    return response;
            }

            try
            {
                var provider = new MultipartMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);
                foreach (var file in provider.Contents)
                {
                    var dataStream = await file.ReadAsStreamAsync();
                    using (var fileStream = File.Create(filePath + fileName))
                    {
                        dataStream.Seek(0, SeekOrigin.Begin);
                        dataStream.CopyTo(fileStream);
                    }
                    var response = new GenericResponseVM();
                    response.StatusCode = 0;
                    response.Message = "Archivo subido exitosamente.";
                    response.GenericResponse = filePath + fileName;
                    return response;

                }
                var otherresponse = new GenericResponseVM();
                otherresponse.StatusCode = 1;
                otherresponse.Message = "Archivo no puedo ser subido.";
                return otherresponse;
            }
            catch (Exception e)
            {
                var otherresponse = new GenericResponseVM();
                otherresponse.StatusCode = 1;
                otherresponse.Message = e.Message;
                return otherresponse;
            }
        }

        [HttpGet]
        [Route("GetFilePaths")]
        public string[] GetFilePaths(string folderPath)
        {
            try
            {
                return Directory.GetFiles(folderPath, "*",
                                         SearchOption.TopDirectoryOnly);
            }
            catch (System.IO.DirectoryNotFoundException dnfEx)
            {
                return new string[] { dnfEx.Message };
            }
        }

        [HttpGet]
        [Route("DownloadFile")]
        public HttpResponseMessage DownloadFile(String filePath)
        {
            try
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                JsonMediaTypeFormatter jsonMediaTypeFormatter = new JsonMediaTypeFormatter();
                GenericPackageVM genericPackage = new GenericPackageVM();

                if (!File.Exists(filePath))
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }
                else
                {
                    genericPackage.StatusCode = 0;
                    Byte[] bytes = File.ReadAllBytes(filePath);
                    response.Content = new ByteArrayContent(bytes);
                    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                    return response;
                }
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

        [Route("GetNationalityList")]
        [HttpGet]
        public List<NationalityVM> GetNationalityList()
        {
            return sharedCore.GetNationalityList();
        }
        [Route("GetGenderList")]
        [HttpGet]
        public List<GenderVM> GetGenderList()
        {
            return sharedCore.GetGenderList();
        }
        [Route("GetCivilStatusList")]
        [HttpGet]
        public List<CivilStatusVM> GetCivilStatusList()
        {
            return sharedCore.GetCivilStatusList();
        }
        [Route("GetProfessionList")]
        [HttpGet]
        public List<ProfessionVM> GetProfessionList()
        {
            return sharedCore.GetProfessionList();
        }
        [Route("GetPhoneTypeList")]
        [HttpGet]
        public List<PhoneTypeVM> GetPhoneTypeList()
        {
            return sharedCore.GetPhoneTypeList();
        }
        [Route("GetCityCodeList")]
        [HttpGet]
        public List<CityCodeVM> GetCityCodeList()
        {
            return sharedCore.GetCityCodeList();
        }
        [Route("GetEmailTypeList")]
        [HttpGet]
        public List<EmailTypeVM> GetEmailTypeList()
        {
            return sharedCore.GetEmailTypeList();
        }
        [Route("GetDirectionTypeList")]
        [HttpGet]
        public List<DirectionTypeVM> GetDirectionTypeList()
        {
            return sharedCore.GetDirectionTypeList();
        }
        [Route("GetRoadTypeList")]
        [HttpGet]
        public List<RoadTypeVM> GetRoadTypeList()
        {
            return sharedCore.GetRoadTypeList();
        }
        [Route("GetInteriorTypeList")]
        [HttpGet]
        public List<InteriorTypeVM> GetInteriorTypeList()
        {
            return sharedCore.GetInteriorTypeList();
        }
        [Route("GetBlockTypeList")]
        [HttpGet]
        public List<BlockTypeVM> GetBlockTypeList()
        {
            return sharedCore.GetBlockTypeList();
        }
        [Route("GetCJHTTypeList")]
        [HttpGet]
        public List<CJHTTypeVM> GetCJHTTypeList()
        {
            return sharedCore.GetCJHTTypeList();
        }
        [Route("GetCountryList")]
        [HttpGet]
        public List<CountryVM> GetCountryList()
        {
            return sharedCore.GetCountryList();
        }
        [Route("GetContactTypeList")]
        [HttpPost]
        public List<ContactTypeVM> GetContactTypeList(ContactTypeBM data)
        {
            return sharedCore.GetContactTypeList(data);
        }
        [Route("GetCurrencyList")]
        [HttpGet]
        public List<CurrencyVM> GetCurrencyList(int nbranch = 0, int nproduct = 0)
        {
            return sharedCore.GetCurrencyList(nbranch, nproduct);
        }
        [Route("GetCiiuList")]
        [HttpGet]
        public List<CiiuVM> GetCiiuList()
        {
            return sharedCore.GetCiiuList();
        }
        [Route("ValPhone")]
        [HttpPost]
        public ResponseVM ValPhone(PhoneBM data)
        {
            return sharedCore.ValPhone(data);
        }
        [Route("ValCiiu")]
        [HttpPost]
        public ResponseVM ValCiiu(CiiuBM data)
        {
            return sharedCore.ValCiiu(data);
        }
        [Route("ValEmail")]
        [HttpPost]
        public ResponseVM ValEmail(EmailBM data)
        {
            return sharedCore.ValEmail(data);
        }
        [Route("ValStreet")]
        [HttpPost]
        public ResponseVM ValStreet(AddressBM data)
        {
            return sharedCore.ValStreet(data);
        }
        [Route("ValContact")]
        [HttpPost]
        public ResponseVM ValContact(ContactBM data)
        {
            return sharedCore.ValContact(data);
        }
        [Route("GetProducts")]
        [HttpGet]
        public List<ProductVM> GetProducts(string productId, string epsId = "", string branch = "")
        {
            return sharedCore.GetProducts(productId, epsId, branch);
        }
        [Route("GetListProductsForBranch")]
        [HttpGet]
        public List<ProductVM> GetListProductsForBranch(string branch = "")
        {
            return sharedCore.GetListProductsForBranch(branch);
        }

        [Route("GetVariables")]
        [HttpGet]
        public string GetVariables(string key)
        {
            string codKey = String.Empty;

            try
            {
                codKey = ELog.obtainConfig(key);
            }
            catch (Exception ex)
            {
                codKey = String.Empty;
                LogControl.save("GetVariables", ex.ToString(), "3");
            }

            return codKey;
        }

        [Route("getFormaPago")]
        [HttpPost]
        public async Task<PaymentVM> getFormaPago(PaymentModel model)
        {
            return await sharedCore.getFormaPago(model);
        }


        /*[Route("GetBranchList")]
        [HttpGet]
        public List<BranchVM> GetBranchList()
        {
            return sharedCore.GetBranchList();
        }*/

        [Route("GetUserProfile")]
        [HttpGet]
        public string GetUserProfile(int product, int usercode)
        {
            return sharedCore.GetUserProfile(usercode, product);
        }

        [Route("GetAlcance")]
        [HttpGet]
        public async Task<List<TypeFacturaVM>> GetAlcance(string nbranch)
        {
            return await sharedCore.getAlcance(nbranch);
        }

        [Route("GetDepartamentos")]
        [HttpGet]
        public async Task<List<DepartmentVM>> GetDepartamentos()
        {
            return await sharedCore.GetDepartamentos();
        }

        [Route("GetTipoViaje")]
        [HttpGet]
        public List<TypeFacturaVM> GetTipoViaje()
        {
            return sharedCore.getInfoTable(2);
        }

        [Route("GetHoras")]
        [HttpGet]
        public List<TypeFacturaVM> GetHoras()
        {
            var listHoras = new List<TypeFacturaVM>();

            for (int i = 1; i < 25; i++)
            {
                var item = new TypeFacturaVM()
                {
                    id = i,
                    descripcion = i.ToString()
                };
                listHoras.Add(item);
            }

            return listHoras;
        }

        [Route("GetTechnicalTariffList")]
        [HttpGet]
        public async Task<List<TechnicalActivityVM>> GetTechnicalActivityList(TechnicalActivityBM data)
        {
            var activityList = sharedCore.GetTechnicalActivityList(data.codProducto);
            var segmento = await new QuotationCORE().getSegmentGraph(data.codSegmento, data.codRamo.ToString());

            return await Task.FromResult(activityList);
        }

        [Route("GetBrokerDepartamento")]
        [HttpPost]
        public List<BrokDepVM> GetBrokerDepartamento(int nbranch, int nproduct)
        {
            return sharedCore.getBrokerDep(nbranch, nproduct);
        }

        //[Route("insertNCTemp")] //AVS - PRO - NC
        //[HttpPost]
        //public ErrorCode insertNCTemp()
        //{
        //    var data = JsonConvert.DeserializeObject<EntidadesNCTEMP>(HttpContext.Current.Request.Params.Get("objeto"));
        //    return sharedCore.insertNCTemp(data);
        //}

        [Route("GetNotaCreditoList")] //AVS - PRO - NC
        [HttpGet]
        public List<NotaCreditoVM> GetNotaCreditoList(string P_CONTRATANTE, string P_REC_NC, int P_TIPO, int P_TIPO_MONEDA, double P_PRIMA)
        {
            var total = sharedCore.GetNotaCreditoList(P_CONTRATANTE, P_REC_NC, P_TIPO, P_TIPO_MONEDA);
            /*double monto_pre = 0; // para sumar valor
            var total_pcp = total.FindAll(x => x.FORMA_PAGO == "PCP").OrderBy(X => X.DOCUMENTO);
            var premium = P_PRIMA;
            List<NotaCreditoVM> data_pcp = new List<NotaCreditoVM>();*/


           /* foreach (var x in total_pcp)

            {
                monto_pre = monto_pre + x.MONTO;
                data_pcp.Add(x);

                if (monto_pre > premium)
                {
                    break;
                }

            }*/


           /* foreach (var data in total)
            {
                if (data_pcp.Contains(data))
                {
                    data.MARCADO = 1;
                }
            }*/


            return total;
        }

        [Route("NCParcialVISA")] //AVS - PRO - NC
        [HttpPost]
        public ErrorCode NCParcialVISA()
        {
            var data = JsonConvert.DeserializeObject<EntidadParcialVisa>(HttpContext.Current.Request.Params.Get("NC"));
            return sharedCore.NCParcialVISA(data);
        }

        [Route("NCParcialPF")] //AVS - PRO - NC
        [HttpPost]
        public ErrorCode NCParcialPF()
        {
            var data = JsonConvert.DeserializeObject<EntidadParcialPF>(HttpContext.Current.Request.Params.Get("NCPF"));
            return sharedCore.NCParcialPF(data);
        }

    }
}