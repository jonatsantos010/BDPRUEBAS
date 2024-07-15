using System.Web.Http;
using System.Web.Http.Cors;
using WSPlataforma.Core;
using WSPlataforma.Entities.HistoryCreditServiceModel.BindingModel;
using WSPlataforma.Entities.HistoryCreditServiceModel.ViewModel;
using WSPlataforma.Util;
using WSPlataforma.Util.PrintPolicyUtility;
using WSPlataforma.WebServices;

namespace WSPlataforma.Controllers
{
    [RoutePrefix("Api/ClientManager")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ClientController : ApiController
    {
        [Route("GetCreditHistory")]
        [HttpPost]
        public CreditHistoryVM GetCreditHistory(ConsultBM request)
        {
            var response = new CreditHistoryVM();

            try
            {
                var consult = new CreditHistoryInvoke().consultar(request);
                consult.sclient = request.sclient;
                consult.usercode = request.usercode;

                response = new QuotationCORE().GetConfigCreditHistory(consult);
                response.score = consult.score;
            }
            catch (System.Exception ex)
            {
                response.nflag = -2;
                response.serror = ex.Message;
            }

            return response;
        }

        [Route("InvokeServiceExperia")]
        [HttpPost]
        public CreditHistoryVM InvokeServiceExperia(ConsultBM request)
        {
            var response = new CreditHistoryVM();

            try
            {
                // Validar si es necesario ir a Experian
                bool sendExperian = new SharedCORE().ValExperiaScore(request);

                if (sendExperian)
                {
                    request.papellido = !string.IsNullOrEmpty(request.papellido) ? request.papellido.Replace(" ", "\\") : request.papellido;

                    // Se consume servicio SOAP de Experian
                    PrintGenerateUtil.executeWSExperia(request);
                }

                double score = new SharedCORE().GetExperiaScore(request.sclient);
                /*AVS - INTERCONEXION SABSA*/
                var consult = new ConsultVM()
                {
                    sclient = request.sclient,
                    usercode = request.usercode,
                    score = score.ToString(),
                    nbranch = request.nbranch
                };

                response = new QuotationCORE().GetConfigCreditHistory(consult);
                response.score = consult.score;
            }
            catch (System.Exception ex)
            {
                //response.nflag = -2;
                response.nflag = 0; // ENDOSO TECNICA JTV 17012023 - RETORNAR SIEMPRE RIESGO ALTO
                response.serror = ex.Message;
            }

            return response;
        }
    }
}