using System;
using System.IO;
using WSPlataforma.Util;
using System.Web.Http;
using System.Web.Http.Cors;
using WSPlataforma.Core;
using WSPlataforma.Entities.RentasModel.BindingModel;
using WSPlataforma.Entities.ReportModel.BindingModel;
using System.Threading.Tasks;
using WSPlataforma.Entities.RentasModel.ViewModel;
using WSPlataforma.Entities.SharedModel.ViewModel;
using System.Net.Http;

namespace WSPlataforma.Controllers
{
    [RoutePrefix("Api/Rentas")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]

    public class RentasController : ApiController
    {
        RentasCORE CoreQuery = new RentasCORE();

        [Route("ListarAprobacionesRentasRes")]
        [HttpPost]
        public IHttpActionResult ListarAprobacionesRentasRes(RentasAprobacionesResBM data)
        {
            var result = this.CoreQuery.ListarAprobacionesRentasRes(data);
            return Ok(result);
        }
        [Route("ListarAprobacionesRentasDet")]
        [HttpPost]
        public IHttpActionResult ListarAprobacionesRentasDet(RentasAprobacionesDetBM data)
        {
            var result = this.CoreQuery.ListarAprobacionesRentasDet(data);
            return Ok(result);
        }
        [Route("AprobarPagos")]
        [HttpPost]
        public IHttpActionResult AprobarPagos(RentasAprobarPagosBM data)
        {
            var result = this.CoreQuery.AprobarPagos(data);
            return Ok(result);
        }
        [Route("GetDataReportRentasRes")]
        [HttpPost]
        public IHttpActionResult GetDataReportRentasRes(ReportRentasResBM data)
        {
            ResponseControl Rpt = new ResponseControl(Response.Ok);
            try
            {
                var response = this.CoreQuery.GetDataReportRentasRes(data);
                if (response.Tables[0].Rows.Count > 0 || response.Tables[1].Rows.Count > 0)
                {
                    string directory = ELog.obtainConfig("reportRentasRes");

                    if (Directory.Exists(directory) == false)
                        Directory.CreateDirectory(directory);

                    string file = "Reporte_Rentas_Resumen.xlsx";
                    string route = directory + "\\" + file;

                    if (File.Exists(route))
                    {
                        Rpt.Data = Convert.ToBase64String(File.ReadAllBytes(route));
                        File.Delete(route);
                    }
                    else
                    {
                        Rpt.Data = null;
                        Rpt.response = Response.Fail;
                    }
                }
                else
                {
                    Rpt.response = Response.Fail;
                    Rpt.Data = "No se obtuvo información en las fechas solicitadas para el reporte.";
                }
            }
            catch (Exception ex)
            {
                Rpt.response = Response.Fail;
                Rpt.Data = ex.Message;

            }
            return Ok(Rpt);
        }
        [Route("GetDataReportRentasDet")]
        [HttpPost]
        public IHttpActionResult GetDataReportRentasDet(ReportRentasDetBM data)
        {
            ResponseControl Rpt = new ResponseControl(Response.Ok);
            try
            {
                var response = this.CoreQuery.GetDataReportRentasDet(data);
                if (response.Tables[0].Rows.Count > 0 || response.Tables[1].Rows.Count > 0)
                {
                    string directory = ELog.obtainConfig("reportRentasDet");

                    if (Directory.Exists(directory) == false)
                        Directory.CreateDirectory(directory);

                    string file = "Reporte_Rentas_Detalle.xlsx";
                    string route = directory + "\\" + file;

                    if (File.Exists(route))
                    {
                        Rpt.Data = Convert.ToBase64String(File.ReadAllBytes(route));
                        File.Delete(route);
                    }
                    else
                    {
                        Rpt.Data = null;
                        Rpt.response = Response.Fail;
                    }
                }
                else
                {
                    Rpt.response = Response.Fail;
                    Rpt.Data = "No se obtuvo información en las fechas solicitadas para el reporte.";
                }
            }
            catch (Exception ex)
            {
                Rpt.response = Response.Fail;
                Rpt.Data = ex.Message;

            }
            return Ok(Rpt);
        }

        [Route("PD_REA_CLIENT_TICKETS")]
        [HttpPost]
        public IHttpActionResult PD_REA_CLIENT_TICKETS(RentasFilterTicketsBM data)
        {
            var result = this.CoreQuery.PD_REA_CLIENT_TICKETS(data);
            return Ok(result);
        }
        [Route("PD_REA_CLIENT_TICKET")]
        [HttpPost]
        public IHttpActionResult PD_REA_CLIENT_TICKET(RentasFilterTicketBM data)
        {
            var result = this.CoreQuery.PD_REA_CLIENT_TICKET(data);
            return Ok(result);
        }
        [Route("PD_GET_PRODUCTOS")]
        [HttpPost]
        public IHttpActionResult PD_GET_PRODUCTOS()
        {
            var result = this.CoreQuery.PD_GET_PRODUCTOS();
            return Ok(result);
        }
        [Route("PD_GET_MOTIVOS")]
        [HttpPost]
        public IHttpActionResult PD_GET_MOTIVOS()
        {
            var result = this.CoreQuery.PD_GET_MOTIVOS();
            return Ok(result);
        }
        [Route("PD_GET_SUBMOTIVOS")]
        [HttpPost]
        public IHttpActionResult PD_GET_SUBMOTIVOS(RentasSubMotivoscsBM data)
        {
            var result = this.CoreQuery.PD_GET_SUBMOTIVOS(data);
            return Ok(result);
        }
        [Route("PD_UPD_ASSIGNEXEC")]
        [HttpPost]
        public IHttpActionResult PD_UPD_ASSIGNEXEC(RentasASSIGNEXECBM data)
        {
            var result = this.CoreQuery.PD_UPD_ASSIGNEXEC(data);
            return Ok(result);
        }
        [Route("PD_REA_CLIENTS")]
        [HttpPost]
        public IHttpActionResult PD_REA_CLIENTS(RentasClientsBM data)
        {
            var result = this.CoreQuery.PD_REA_CLIENTS(data);
            return Ok(result);
        }
        [Route("PD_GET_ESTADOS")]
        [HttpPost]
        public IHttpActionResult PD_GET_ESTADOS()
        {
            var result = this.CoreQuery.PD_GET_ESTADOS();
            return Ok(result);
        }
        [Route("PD_GET_EJECUTIVOS")]
        [HttpPost]
        public IHttpActionResult PD_GET_EJECUTIVOS(RentaseEjecutivosBM data)
        {
            var result = this.CoreQuery.PD_GET_EJECUTIVOS(data);
            return Ok(result);
        }
        [Route("PD_GET_TYPE_DOCUMENT")]
        [HttpPost]
        public IHttpActionResult PD_GET_TYPE_DOCUMENT()
        {
            var result = this.CoreQuery.PD_GET_TYPE_DOCUMENT();
            return Ok(result);
        }
        [Route("PD_GET_TYPE_PERSON")]
        [HttpPost]
        public IHttpActionResult PD_GET_TYPE_PERSON()
        {
            var result = this.CoreQuery.PD_GET_TYPE_PERSON();
            return Ok(result);
        }
        [Route("PD_VALFORMATVALUES")]
        [HttpPost]
        public IHttpActionResult PD_VALFORMATVALUES(RentasValidaBM data)
        {
            var result = this.CoreQuery.PD_VALFORMATVALUES(data);
            return Ok(result);
        }
        [Route("PD_GET_FILTERDAY_START")]
        [HttpPost]
        public IHttpActionResult PD_GET_FILTERDAY_START()
        {
            var result = this.CoreQuery.PD_GET_FILTERDAY_START();
            return Ok(result);
        }
        [Route("PD_GET_NPRODUCTCANAL")]
        [HttpPost]
        public IHttpActionResult PD_GET_NPRODUCTCANAL()
        {
            var result = this.CoreQuery.PD_GET_NPRODUCTCANAL();
            return Ok(result);
        }
        [Route("PD_GET_NIDPROFILE")]
        [HttpPost]
        public IHttpActionResult PD_GET_NIDPROFILE(RentasNidProfileBM data)
        {
            var result = this.CoreQuery.PD_GET_NIDPROFILE(data);
            return Ok(result);
        }
        [Route("PD_REA_LIST_ACTIONS")]
        [HttpPost]
        public IHttpActionResult PD_REA_LIST_ACTIONS(RentasListActionsBM data)
        {
            var result = this.CoreQuery.PD_REA_LIST_ACTIONS(data);
            return Ok(result);
        }
        [Route("PD_REA_LIST_ACTIONS_TICKET")]
        [HttpPost]
        public IHttpActionResult PD_REA_LIST_ACTIONS_TICKET(RentasListActionsTikectBM data)
        {
            var result = this.CoreQuery.PD_REA_LIST_ACTIONS_TICKET(data);
            return Ok(result);
        }

        [Route("PD_UPD_STATUS_TICKET")]
        [HttpPost]
        public IHttpActionResult PD_UPD_STATUS_TICKET(RentasStatusTicketTikectBM data)
        {
            var result = this.CoreQuery.PD_UPD_STATUS_TICKET(data);
            return Ok(result);
        }

        [Route("GET_POLICY_DATA")]
        [HttpPost]
        public async Task<RentasGetPolicyDataResponseVM> GET_POLICY_DATA(RentasGetPolicyDataBM data)
        {
            var response = new RentasGetPolicyDataResponseVM();
            response = await this.CoreQuery.GET_POLICY_DATA(data);
            return response;
        }

        [Route("GET_CALCULATION_AMOUNT")]
        [HttpPost]
        public async Task<RentasGetCalculationAmountResponseVM> GET_CALCULATION_AMOUNT(RentasGetCalculationAmountBM data)
        {
            var response = new RentasGetCalculationAmountResponseVM();
            response = await this.CoreQuery.GET_CALCULATION_AMOUNT(data);
            return response;
        }
        [Route("GET_CALCULATION_AMOUNT_DUMMY")]
        [HttpPost]
        public async Task<RentasGetCalculationAmountResponseVM> GET_CALCULATION_AMOUNT_DUMMY(RentasGetCalculationAmountBM data)
        {
            var response = new RentasGetCalculationAmountResponseVM();
            response = await this.CoreQuery.GET_CALCULATION_AMOUNT_DUMMY(data);
            return response;
        }
        [Route("PD_UPD_AMOUNT_TICKET")]
        [HttpPost]
        public IHttpActionResult PD_UPD_AMOUNT_TICKET(RentasUpdAmountTicketBM data)
        {
            var result = this.CoreQuery.PD_UPD_AMOUNT_TICKET(data);
            return Ok(result);
        }
        [Route("PD_LIST_ADJ")]
        [HttpPost]
        public IHttpActionResult PD_LIST_ADJ(RentasListAdjBM data)
        {
            var result = this.CoreQuery.PD_LIST_ADJ(data);
            return Ok(result);
        }
        [Route("PD_UPD_ATTACHMENT")]
        [HttpPost]
        public IHttpActionResult PD_UPD_ATTACHMENT(RentasUpdAttachmentBM data)
        {
            var result = this.CoreQuery.PD_UPD_ATTACHMENT(data);
            return Ok(result);
        }
        [Route("PD_INS_DATA_EMAIL")]
        [HttpPost]
        public IHttpActionResult PD_INS_DATA_EMAIL(RentasInsDataEmailBM data)
        {
            var result = this.CoreQuery.PD_INS_DATA_EMAIL(data);
            return Ok(result);
        }


        public string PD_GET_ROUTE_FILE()
        {
            var result = this.CoreQuery.PD_GET_ROUTE_FILE();
            return result;
        }


        [HttpPost]
        [Route("UploadFile")]
        public async Task<GenericResponseVM> UploadFile(string fileName)
        {
            var routeFileResult = PD_GET_ROUTE_FILE();

            //var test = routeFileResult.C_TABLE[0].SROUTE;


            if (!Request.Content.IsMimeMultipartContent())
            {
                var response = new GenericResponseVM
                {
                    StatusCode = 1,
                    Message = "El requerimiento no contiene un contenido válido."
                };
                return response;
            }

            // Directorio raíz del proyecto
            string filePath = routeFileResult;

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            // Agregar "-ticket" al nombre del archivo
            //string fileNameWithSuffix = AddTicketSuffix(fileName, scode);

            try
            {
                var provider = new MultipartMemoryStreamProvider();

                await Request.Content.ReadAsMultipartAsync(provider);

                foreach (var file in provider.Contents)
                {
                    var dataStream = await file.ReadAsStreamAsync();
                    string fullPath = Path.Combine(filePath, fileName);

                    using (var fileStream = File.Create(fullPath))
                    {
                        dataStream.Seek(0, SeekOrigin.Begin);
                        dataStream.CopyTo(fileStream);
                    }

                    var response = new GenericResponseVM
                    {
                        StatusCode = 0,
                        Message = "Archivo subido exitosamente.",
                        GenericResponse = fullPath
                    };
                    return response;
                }

                var otherresponse = new GenericResponseVM
                {
                    StatusCode = 1,
                    Message = "Archivo no pudo ser subido."
                };
                return otherresponse;
            }
            catch (Exception e)
            {
                var otherresponse = new GenericResponseVM
                {
                    StatusCode = 1,
                    Message = e.Message
                };
                return otherresponse;
            }
        }

        //private string AddTicketSuffix(string fileName,string scode)
        //{
        //    // Separar el nombre del archivo y su extensión
        //    string fileWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        //    string extension = Path.GetExtension(fileName);

        //    // Concatenar el sufijo "-ticket" antes de la extensión
        //    return $"{fileWithoutExtension}-{scode}{extension}";
        //}

        [Route("PD_INS_TBL_TICK_ADJUNT")]
        [HttpPost]
        public IHttpActionResult PD_INS_TBL_TICK_ADJUNT(RentasInsTickAdjuntBM data)
        {
            var result = this.CoreQuery.PD_INS_TBL_TICK_ADJUNT(data);
            return Ok(result);
        }

        [Route("PD_DEL_TBL_TICK_ADJUNT")]
        [HttpPost]
        public IHttpActionResult PD_DEL_TBL_TICK_ADJUNT(RentasDelTickAdjuntBM data)
        {
            var result = this.CoreQuery.PD_DEL_TBL_TICK_ADJUNT(data);
            return Ok(result);
        }
        [Route("PD_UPD_TICKET_DESCRIPT")]
        [HttpPost]
        public IHttpActionResult PD_UPD_TICKET_DESCRIPT(RentasUpdTicketDescriptBM data)
        {
            var result = this.CoreQuery.PD_UPD_TICKET_DESCRIPT(data);
            return Ok(result);
        }

        [Route("PD_UPD_TICKET_NMOTIV")]
        [HttpPost]
        public IHttpActionResult PD_UPD_TICKET_NMOTIV(RentasUpdTicketNmotivBM data)
        {
            var result = this.CoreQuery.PD_UPD_TICKET_NMOTIV(data);
            return Ok(result);
        }

        [Route("PD_GET_USER_RESPONSIBLE")]
        [HttpPost]
        public IHttpActionResult PD_GET_USER_RESPONSIBLE(RentasGetUserResponsibleBM data)
        {
            var result = this.CoreQuery.PD_GET_USER_RESPONSIBLE(data);
            return Ok(result);
        }
        [Route("PD_GET_VALPOPUP")]
        [HttpPost]
        public IHttpActionResult PD_GET_VALPOPUP(RentasGetValpopupBM data)
        {
            var result = this.CoreQuery.PD_GET_VALPOPUP(data);
            return Ok(result);
        }
        [Route("PD_GET_TYPECOMMENT")]
        [HttpPost]
        public IHttpActionResult PD_GET_TYPECOMMENT()
        {
            var result = this.CoreQuery.PD_GET_TYPECOMMENT();
            return Ok(result);
        }
        [Route("PD_GET_DESTINATION")]
        [HttpPost]
        public IHttpActionResult PD_GET_DESTINATION()
        {
            var result = this.CoreQuery.PD_GET_DESTINATION();
            return Ok(result);
        }
        [Route("PD_GET_EMAIL_DESTINATION")]
        [HttpPost]
        public IHttpActionResult PD_GET_EMAIL_DESTINATION(RentasGetEmailDestinationBM data)
        {
            var result = this.CoreQuery.PD_GET_EMAIL_DESTINATION(data);
            return Ok(result);
        }
        [Route("PD_GET_EMAIL_USER")]
        [HttpPost]
        public IHttpActionResult PD_GET_EMAIL_USER(RentasGetEmailUserBM data)
        {
            var result = this.CoreQuery.PD_GET_EMAIL_USER(data);
            return Ok(result);
        }
        [Route("PD_GET_CONF_FILE")]
        [HttpPost]
        public IHttpActionResult PD_GET_CONF_FILE()
        {
            var result = this.CoreQuery.PD_GET_CONF_FILE();
            return Ok(result);
        }

        [Route("PD_GET_MESSAGE")]
        [HttpPost]
        public IHttpActionResult PD_GET_MESSAGE(RentasGetMessage data)
        {
            var result = this.CoreQuery.PD_GET_MESSAGE(data);
            return Ok(result);
        }
    }
}

