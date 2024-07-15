using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Cors;
using WSPlataforma.Core;
using WSPlataforma.Entities.PrintPolicyModel;
using WSPlataforma.Entities.PrintPolicyModel.ATP;
using WSPlataforma.Entities.PrintPolicyModel.BindingModel;
using WSPlataforma.Entities.PrintPolicyModel.ViewModel;

namespace WSPlataforma.Controllers
{
    [RoutePrefix("Api/PrintPolicyManager")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PrintPolicyController : ApiController
    {
        PrintPolicyCORE printPolicyCore = new PrintPolicyCORE();

        [Route("GetFormatLastOrder")]
        [HttpPost]
        public List<FormatOrderVM> GetFormatLastOrder(PrintGenerateBM printGenerateBM)
        {
            return printPolicyCore.GetFormatLastOrder(printGenerateBM.NNTRANSAC, printGenerateBM.NPRODUCT, printGenerateBM.NBRANCH);
        }
        [Route("UpdateDocumentProcessState")]
        [HttpPost]
        public ResponsePrintVM UpdateDocumentProcessState(PrintGenerateBM printGenerateBM)
        {
            return printPolicyCore.UpdateDocumentProcessState(printGenerateBM);
        }
        [Route("PrintPDF")]
        [HttpPost]
        public ResponsePrintVM PrintPDF(PrintGenerateBM printGenerateBM)
        {
            return printPolicyCore.PrintPDF(printGenerateBM);
        }
        /*[Route("PrintRequestFormat")]
        [HttpGet]
        public ResponsePrintVM PrintRequestFormat()
        {
            return printPolicyCore.PrintRequestFormat();
        }*/

        [Route("GetJobsPrint")]
        [HttpGet]
        public List<ResponsePrintJobsVM> GetJobsPrint()
        {
            return printPolicyCore.GetJobsPrint();
        }
        [Route("GetFormatsPrint")]
        [HttpPost]
        public List<ResponsePrintFormatsVM> GetFormatsPrint(PrintFormatsBM printFormatsBM)
        {
            return printPolicyCore.GetFormatsPrint(printFormatsBM);
        }

        [Route("GetProceduresPrint")]
        [HttpPost]
        public List<PrintProceduresVM> GetProceduresPrint(PrintProceduresBM printProceduresBM)
        {
            return printPolicyCore.GetProceduresPrint(printProceduresBM);
        }

        [Route("GetPathsPrint")]
        [HttpPost]
        public List<PrintPathsVM> GetPathsPrint(dynamic NCOD_CONDICIONADO)
        {
            return printPolicyCore.GetPathsPrint(NCOD_CONDICIONADO);
        }

        [Route("PrintATP")]
        [HttpPost]
        public ResponsePrintVM PrintATP(RequestATP request)
        {
            return this.printPolicyCore.PrintATP(request);
        }

        [Route("PrintSuspATP")]
        [HttpPost]
        public ResponsePrintVM PrintSuspATP(RequestSuspATP request)
        {
            return this.printPolicyCore.PrintSuspATP(request);
        }
    }
}
