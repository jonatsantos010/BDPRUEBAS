using System.Collections.Generic;
using WSPlataforma.DA.PrintPolicyPDF.Quotation;
using WSPlataforma.Entities.DocQuotationModel.BindingModel;
using WSPlataforma.Entities.DocQuotationModel.ViewModel;
using WSPlataforma.Entities.PrintPolicyModel;

namespace WSPlataforma.Core
{
    public class DocQuotationCORE
    {
        DocQuotationDA DataAccessQuery = new DocQuotationDA();

        public ResponsePrintVM QuotationPDF(GenerateQuotationBM generateQuotationBM)
        {
            return DataAccessQuery.QuotationPDF(generateQuotationBM);
        }
        public ResponsePrintVM UpdateStateQuotation(GenerateQuotationBM generateQuotationBM)
        {
            return DataAccessQuery.UpdateStateQuotation(generateQuotationBM);
        }
        public string GetStateProcess(long NID_COTIZACION)
        {
            return DataAccessQuery.GetStateProcess(NID_COTIZACION);
        }
        public List<JobQuotationVM> GetJobsQuotation()
        {
            return DataAccessQuery.GetJobsQuotation();
        }
        public List<ModuleQuotationVM> GetModulesQuotation(FormatQuotationBM formatQuotationBM)
        {
            return DataAccessQuery.GetModulesQuotation(formatQuotationBM);
        }
        public List<FormatQuotationVM> GetFormatsQuotation(FormatQuotationBM formatQuotationBM)
        {
            return DataAccessQuery.GetFormatsQuotation(formatQuotationBM);
        }
        public List<ProcedureQuotationVM> GetProceduresQuotationPrint(ProcedureQuotationBM procedureQuotationBM)
        {
            return DataAccessQuery.GetProceduresQuotationPrint(procedureQuotationBM);
        }
    }
}
