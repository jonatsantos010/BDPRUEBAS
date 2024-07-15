using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace WSPlataforma.Controllers
{
    [RoutePrefix("api/tecnica")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class TecnicaController : ApiController
    {
        public IHttpActionResult SendNotification(SendNotificationRequest request)
        {
            return Ok(true);
        }

        public IHttpActionResult SendResponseNotification(object data)
        {
            return Ok(new
            {
                numeroCotizacion = "11111",
                fechaAprobacion = DateTime.Now,
                usuarioTecnicoAprobador = "usuario-aprobador",
                estadoCotizacion = "Aprobado",
                comentario = "se aprueba cotización",
                primaNeta = 14.2,
                primasPorCategoria = new List<object> {
                    new { categoria = "obrero", primaNeta = 9.2 },
                    new { categoria = "empleado", primaNeta = 5.2 },
                    //obrero alto riesgo*
                    //si se quiere manejar una categoria más se tienes q hacer varios cambios.
                },
                tasasPorCategoria = new List<object> {
                    new { categoria = "obrero", tasaNeta = 9.2 },
                    new { categoria = "empleado", tasaNeta = 5.2 },
                }
            });
        }
    }


    public class SendNotificationRequest
    {
        public int NroCotizacion { get; set; }
        public EntidadItem Broker { get; set; }
        public EntidadItem Cliente { get; set; }
        public string CodActividad { get; set; }
        public string UsuarioCreacion { get; set; }
        public DateTime InicioVigencia { get; set; }
        public DateTime FinVigencia { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public DateTime FechaHoraEnvioBandejaGerenciaTecnica { get; set; }
        public string TipoCotizacion { get; set; }
        public string EstadoCotizacion { get; set; }
        public decimal TasaComercialPropuesta { get; set; }
        public string Comentario { get; set; }
        public List<PlanillaItem> PlanillaTrabajadores { get; set; }
    }

    public class EntidadItem
    {
        public string TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public string NombreRazonSocial { get; set; }
    }
    public class PlanillaItem
    {

    }
}


public class SendNotificationTecnicaRequest
{
    //public int codTipoTarifa { get; set; }
    //public int cantidadTrabajadores { get; set; }
    //public decimal MontoPlanilla { get; set; }
    //public int Frecuencia { get; set; }
    //public int CantidadObreros { get; set; }
    //public int CantidadObrerosAR { get; set; }
    //public int CantidadTrabajadoresMayor65Menor69 { get; set; }
    //public int CantidadTrabajadoresMenorIgual65 { get; set; }
    //public int NroCotizacion { get; set; }
    public Header header { get; set; }
    public Detail detail { get; set; }
}

public class GetUrlUploadFilesRequest
{
    public string number { get; set; }
    public string comment { get; set; }
    public string extension { get; set; }
    public string quotationType { get; set; }
    public string name { get; set; }
}


public class Header
{
    public List<ProposedComercialRate> proposedComercialRate { get; set; }
    public List<PreviousCategoryList> previousCategoryList { get; set; } = new List<PreviousCategoryList>();
    public string brokerCode { get; set; }
    public string brokerName { get; set; }
    public string contractorCode { get; set; }
    public string contractorName { get; set; }
    public string economicActivityCode { get; set; }
    public string economicActivityName { get; set; }
    public string associatedEconomyActivityCode { get; set; }
    public string associatedEconomyActivityName { get; set; }
    public string user { get; set; }
    public string quotationRequestDate { get; set; }
    public string quotationSendedDate { get; set; }
    public string initialValidity { get; set; }
    public string endValidity { get; set; }
    public string number { get; set; }
    public string quotationType { get; set; }
    public string quotationRequestComment { get; set; }
    public string plan { get; set; }
    public string processType { get; set; }
    public string maxDate { get; set; }
    public double maxRem { get; set; }
    public int maxAge { get; set; }
    public decimal payrollAmount { get; set; }
    public int monthFrequency { get; set; }
    public decimal minPremium { get; set; }
    public string policyNumber { get; set; }
    public decimal endorsementBonus { get; set; }
    public string endorsementType { get; set; }
    public string endorsementDate { get; set; }
    public string procedure { get; set; }
    public string segment { get; set; }
    public string sla { get; set; }
    public int rateType { get; set; } //Tipo de tasa VL
    public double commission { get; set; }
    public double endorsementMinBonus { get; set; }
    public int previousPaymentFrecuency { get; set; }
}
public class ProposedComercialRate
{
    public string description { get; set; }
    public double rate { get; set; }
    public string rango_edad { get; set; } // GCAA 27/10/2023    
    public int categoryId { get; set; }// GCAA 20052024
    public int option { get; set; }// GCAA 20052024
}
public class WorkerType
{
    public string code_core { get; set; }
    public string description { get; set; }
    public string rango_edad { get; set; } // GCAA 27/10/2023
}

public class Payroll
{
    public int id { get; set; }
    public string name { get; set; }
    public string surname1 { get; set; }
    public string surname2 { get; set; }
    public string documentType { get; set; }
    public string documentNumber { get; set; }
    public string birthDate { get; set; }
    public string gender { get; set; }
    public decimal salary { get; set; }
    public WorkerType workerType { get; set; }
    public string status { get; set; }
    public string countryOfBirth { get; set; }
}

public class Detail
{
    public List<Payroll> payroll { get; set; }
    //public string riskCoverageRates { get; set; }
}

public class SendNotificationTecnicaResponse
{
    public string Message { get; set; }
}

public class GetUrlUploadFilesResponse
{
    public int code { get; set; }
    public string data { get; set; }
}

public class PreviousCategoryList
{
    public double extraRem { get; set; }
    public double extraRate { get; set; }
    public double topRem { get; set; }
    public decimal extraBonus { get; set; }
    public double topRate { get; set; }
    public decimal topBonus { get; set; }
    public string category { get; set; }
}