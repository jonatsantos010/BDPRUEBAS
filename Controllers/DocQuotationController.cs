using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Cors;
using WSPlataforma.Core;
using WSPlataforma.Entities.DocQuotationModel.BindingModel;
using WSPlataforma.Entities.DocQuotationModel.ViewModel;
using WSPlataforma.Entities.PrintPolicyModel;

namespace WSPlataforma.Controllers
{
    [RoutePrefix("Api/DocQuotationManager")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class DocQuotationController : ApiController
    {
        DocQuotationCORE docQuotationCore = new DocQuotationCORE();

        [Route("QuotationPDF")]
        [HttpPost]
        public ResponsePrintVM QuotationPDF(GenerateQuotationBM generateQuotationBM)
        {
            return docQuotationCore.QuotationPDF(generateQuotationBM);
        }
        [Route("GetStateProcess")]
        [HttpGet]
        public string GetStateProcess(long NID_COTIZACION)
        {
            return docQuotationCore.GetStateProcess(NID_COTIZACION);
        }
        [Route("GetJobsQuotation")]
        [HttpGet]
        public List<JobQuotationVM> GetJobsQuotation()
        {
            return docQuotationCore.GetJobsQuotation();
        }
        [Route("GetModulesQuotation")]
        [HttpPost]
        public List<ModuleQuotationVM> GetModulesQuotation(FormatQuotationBM formatQuotationBM)
        {
            return docQuotationCore.GetModulesQuotation(formatQuotationBM);
        }
        [Route("GetFormatsQuotation")]
        [HttpPost]
        public List<FormatQuotationVM> GetFormatsQuotation(FormatQuotationBM formatQuotationBM)
        {
            return docQuotationCore.GetFormatsQuotation(formatQuotationBM);
        }
        [Route("GetProceduresQuotationPrint")]
        [HttpPost]
        public List<ProcedureQuotationVM> GetProceduresQuotationPrint(ProcedureQuotationBM procedureQuotationBM)
        {
            return docQuotationCore.GetProceduresQuotationPrint(procedureQuotationBM);
        }

    }
}
