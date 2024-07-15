using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using System.Web.Http;
using System.Web.Http.Cors;
using WSPlataforma.Entities.Reports.ViewModel;
using System.Net;
using System.Web.Http;
using System.IO;
using System.Net.Http;
using WSPlataforma.Entities.Reports.BindingModel;
using WSPlataforma.Core;
using System.Configuration;
using System.Threading.Tasks;
using WSPlataforma.Util;
using WSPlataforma.Util.PrintPolicyUtility;

namespace WSPlataforma.Controllers
{
    [RoutePrefix("Api/PDFGenerator")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class GeneratorPDFController : ApiController
    {
        private PDFGeneratorCORE PDFGeneratorCORE = new PDFGeneratorCORE();
        private PrintPolicyCORE printPolicyCORE = new PrintPolicyCORE();

        [Route("ManageGeneration")]
        [HttpGet]
        public async Task<GenericPackageVM> ManageGeneration(string key, string product)
        {
            List<DataPolicyVM> data = PDFGeneratorCORE.GetPolizaData(key);
            GenericPackageVM response = new GenericPackageVM();

            if (data[0].BranchId == int.Parse(ELog.obtainConfig("sctrBranch")) &&
                data[0].ProductId == int.Parse(ELog.obtainConfig("pensionKey")) &&
                (data[0].TransactTypeId == (int)PrintEnum.TransacType.EMISION ||
                 data[0].TransactTypeId == (int)PrintEnum.TransacType.RENOVACION))
            {
                response.FolderPath = "";
                return (response);
            }
            else
            {
                return await PDFGeneratorCORE.ManageGeneration(key);
            }

        }

        [Route("GenerateQuotationDocument")]
        [HttpGet]
        public async Task<GenericPackageVM> GenerateQuotationDocument(string quotationNumber, string product, string branch)
        {
            GenericPackageVM response = new GenericPackageVM();

            if (branch == ELog.obtainConfig("vidaLeyBranch"))
            {
                response = PDFGeneratorCORE.GenerateQuotationVL(quotationNumber);
            }
            else if (branch == ELog.obtainConfig("vidaGrupoBranch") ||
                    branch == ELog.obtainConfig("accidentesBranch"))
            {
                int condicionado = Convert.ToInt32(ELog.obtainConfig("condicionado" + branch));
                response = PDFGeneratorCORE.GenerateQuotationCovid(quotationNumber, condicionado);
            }
            else if (product != ELog.obtainConfig("pensionKey") &&
                    branch == ELog.obtainConfig("sctrBranch"))
            {
                response = await PDFGeneratorCORE.GenerateQuotationDocument(quotationNumber);
            }
            else if (product == ELog.obtainConfig("pensionKey") &&
                    branch == ELog.obtainConfig("sctrBranch"))
            {
                response.FolderPath = ELog.obtainConfig("pathSlipSctr") + "\\" + quotationNumber + "\\";
            }

            return response;
        }
    }
}
