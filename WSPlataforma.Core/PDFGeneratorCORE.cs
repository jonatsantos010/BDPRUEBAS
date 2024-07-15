using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.DA;
using WSPlataforma.Entities.Reports.BindingModel;
using WSPlataforma.Entities.Reports.ViewModel;

namespace WSPlataforma.Core
{
    public class PDFGeneratorCORE
    {
        private PDFGeneratorDA pDFGeneratorDA = new PDFGeneratorDA();

        public List<DataPolicyVM> GetPolizaData(string skey)
        {
            return pDFGeneratorDA.GetPolizaData(skey);
        }

        public async Task<GenericPackageVM> ManageGeneration(string key)
        {
            return await pDFGeneratorDA.ManageGenerationAsync(key);
        }
        public async Task<GenericPackageVM> GenerateQuotationDocument(string quotationNumber)
        {
            return await pDFGeneratorDA.GenerateQuotationDocument(quotationNumber);
        }

        public GenericPackageVM GenerateQuotationVL(string quotationNumber)
        {
            return pDFGeneratorDA.GenerateQuotationVL(quotationNumber);
        }

        public GenericPackageVM GenerateQuotationCovid(string quotationNumber, int condicionado)
        {
            return pDFGeneratorDA.GenerateQuotationCovid(quotationNumber, condicionado);
        }

        public async Task<TransactionTrack> GetTransactionData(string key)
        {
            return await pDFGeneratorDA.GetTransactionData(key);
        }

    }
}
