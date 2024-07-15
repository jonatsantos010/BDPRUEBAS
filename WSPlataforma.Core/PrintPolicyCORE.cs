using System.Collections.Generic;
using WSPlataforma.DA.PrintPolicyPDF;
using WSPlataforma.Entities.PrintPolicyModel;
using WSPlataforma.Entities.PrintPolicyModel.ATP;
using WSPlataforma.Entities.PrintPolicyModel.BindingModel;
using WSPlataforma.Entities.PrintPolicyModel.ViewModel;

namespace WSPlataforma.Core
{
    public class PrintPolicyCORE
    {
        PrintPolicyDA DataAccessQuery = new PrintPolicyDA();
        public List<FormatOrderVM> GetFormatLastOrder(dynamic NTRANSAC, dynamic NPRODUCT, dynamic NBRANCH)
        {
            return DataAccessQuery.GetFormatLastOrder(NTRANSAC, NPRODUCT, NBRANCH);
        }

        public ResponsePrintVM UpdateDocumentProcessState(PrintGenerateBM printGenerateBM)
        {
            return DataAccessQuery.UpdateDocumentProcessState(printGenerateBM);
        }

        public ResponsePrintVM PrintPDF(PrintGenerateBM printGenerateBM)
        {
            return DataAccessQuery.PrintPDF(printGenerateBM);
        }

        public List<ResponsePrintJobsVM> GetJobsPrint()
        {
            return DataAccessQuery.GetJobsPrint();
        }

        public List<ResponsePrintFormatsVM> GetFormatsPrint(PrintFormatsBM printFormatsBM)
        {
            return DataAccessQuery.GetFormatsPrint(printFormatsBM);
        }

        public List<PrintProceduresVM> GetProceduresPrint(PrintProceduresBM printProceduresBM)
        {
            return DataAccessQuery.GetProceduresPrint(printProceduresBM);
        }

        public List<PrintPathsVM> GetPathsPrint(dynamic NCOD_CONDICIONADO)
        {
            return DataAccessQuery.GetProceduresPrint(NCOD_CONDICIONADO);
        }
        public ResponsePrintVM PrintATP(RequestATP request)
        {
            return this.DataAccessQuery.PrintATP(request);
        }
        public ResponsePrintVM PrintSuspATP(RequestSuspATP request)
        {
            return this.DataAccessQuery.PrintSuspATP(request);
        }
    }
}
