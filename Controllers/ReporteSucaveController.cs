using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Cors;
using WSPlataforma.Core;
using WSPlataforma.Entities.PolicyModel.BindingModel;
using System.Web.Http;
using System.Configuration;
//using WSPlataforma.Entities.LoadMassiveModel;
using WSPlataforma.Entities.ReporteSucaveModel;
using WSPlataforma.Entities.ReporteSucaveModel.BindingModel;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;
using WSPlataforma.Util;
using WSPlataforma.Entities.ViewModel;
using ExcelDataReader;
using System.Text;
using WSPlataforma.Entities.QuotationModel.ViewModel;
using System.Data;
using WSPlataforma.Entities.QuotationModel.BindingModel;
using WSPlataforma.Entities.ReportModel.BindingModel;
using System.Web.UI.WebControls;

namespace WSPlataforma.Controllers
{
    [RoutePrefix("Api/ReporteSucave")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ReporteSucaveController : ApiController
    {
        #region Reporte Sucave
        ReporteSucaveCORE reporteSucaveCORE = new ReporteSucaveCORE();
        [Route("GetBranch")]
        [HttpGet]
        public IHttpActionResult GetBranches()
        {
            var result = this.reporteSucaveCORE.GetBranch();
            return Ok(result);
        }

        //Servicio para obtener productos
        [Route("GetProduct")]
        [HttpPost]
        public IHttpActionResult GetProduct(ProductoFilter data)

        {
            var result = this.reporteSucaveCORE.GetProduct(data);
            return Ok(result);
        }

        [Route("ProcessReport")]
        [HttpPost]
        public IHttpActionResult InsertReportStatus(ReporteSoatFilter data)
        {
            string codigoProceso = string.Empty;
            codigoProceso = (data.UserName).ToUpper() + DateTime.Now.ToString("yyyyMMddhhmmssff");
            data.IdReport = codigoProceso.Trim();

            var result = this.reporteSucaveCORE.InsertReportStatus(data);

            long rpta = result.LongCount();

            ReportProcess rpoProc = new ReportProcess();

            rpoProc.P_NCODE = int.Parse(rpta.ToString());
            rpoProc.P_SMESSAGE = "Error";

            //inicio proceso para generar el reporte preliminar detalle producción SOAT
            try
            {
                if (rpoProc.P_NCODE > 0)
                {
                    //NUEVOS ARGUMENTOS PARA VALIDACION PRIMAS COMISIONES
                    string fecInicio = data.StartDate.ToString("dd/MM/yyyy");
                    string fecFin = data.EndDate.ToString("dd/MM/yyyy");

                    string arguments = string.Format(@"{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}", codigoProceso,
                        data.BranchId, fecInicio, fecFin, 0, 0, 0, 0, 0, 104);
                    using (var process = new Process
                    {
                        StartInfo =
                    new ProcessStartInfo { FileName = ELog.obtainConfig("exePath"), Arguments = arguments }
                    })
                    {
                        process.Start();
                    }

                }
                else
                {
                    return Ok(rpoProc.P_SMESSAGE);
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            if (rpoProc.P_NCODE > 0)
            {
                return Ok(result);
            }
            else
            {
                return Ok(rpoProc.P_SMESSAGE);
            }
        }

        //Servicio para obtener status de reportes
        [Route("GetStatusReport")]
        [HttpPost]
        public IHttpActionResult GetStatusReport(ReporteSoatFilter data)

        {
            var result = this.reporteSucaveCORE.GetReportStatus(data);
            return Ok(result);
        }

        //Servicio para Obtener Archivo(Excel)
        [Route("getFileReporteSoat")]
        [HttpPost]
        public IHttpActionResult GetFilePremiumReport(ReporteSoatFilter data)
        {
            ResponseControl Rpt = new ResponseControl(Response.Ok);
            try
            {
                var soatNotas = "-N";
                var soatPrimas = "-P";

                if (data.IdReport != null || data.IdReport != "")
                {
                    //Construimos la ruta para obtener el archivo de cabecera
                    string directory = ELog.obtainConfig("reportPathRPTSoat");
                    string directoryReport = data.IdReport + "\\";
                    string file = data.IdReport.ToUpper() + soatNotas + ".xlsx";
                    string route = directory + "\\" + directoryReport + file;
                    if (File.Exists(route))
                    {
                        //Convertimos el archivo en Base64
                        Rpt.Data = Convert.ToBase64String(File.ReadAllBytes(route));
                    }

                    //Construimos la ruta para obtener el archivo de detalle
                    string directory2 = ELog.obtainConfig("reportPathRPTSoat");
                    string directoryReport2 = data.IdReport + "\\";
                    string file2 = data.IdReport.ToUpper() + soatPrimas + ".xlsx";
                    string route2 = directory2 + "\\" + directoryReport2 + file2;
                    if (File.Exists(route2))
                    {
                        //Convertimos el archivo en Base64
                        Rpt.Data2 = Convert.ToBase64String(File.ReadAllBytes(route2));
                    }
                }
                else
                {
                    Rpt.Data = null;
                    Rpt.Data2 = null;
                    Rpt.response = Response.Fail;
                }
            }
            catch (Exception ex)
            {
                Rpt.response = Response.Fail;
                Rpt.Data = ex.Message;
                Rpt.Data2 = ex.Message;

            }
            // var response = GetFilePath(id);
            return Ok(Rpt);
        }

        #endregion

        #region Consulta Reporte Sucave
        [Route("GenerateFact")]
        [HttpGet]
        public ResponseVM GenerateFact(int NIDHEADERPROC, int NIDDETAILPROC, int NUSERCODE)
        {
            ReporteSucaveCORE reporteSucaveCORE = new ReporteSucaveCORE();
            return reporteSucaveCORE.GenerateFact(NIDHEADERPROC, NIDDETAILPROC, NUSERCODE);
        }
        [Route("ProcessHeaderList")]
        [HttpPost]
        public IHttpActionResult ProcessHeaderList(ReporteVisualizarFilter data)
        {
            var result = this.reporteSucaveCORE.GetProcessHeader(data);
            return Ok(result);
        }
        [Route("ProcessDetailList")]
        [HttpGet]
        public IHttpActionResult ProcessDetailList(int IdProcessHeader)
        {
            ReporteSucaveCORE reporteSucaveCORE = new ReporteSucaveCORE();
            var response = reporteSucaveCORE.GetProcessDetail(IdProcessHeader);
            return Ok(response);
        }
        [Route("GetProcessLogError")]
        [HttpGet]
        public IHttpActionResult GetProcessLogError(int IdProcessDetail, string Opcion)
        {
            ReporteSucaveCORE reporteSucaveCORE = new ReporteSucaveCORE();
            var response = reporteSucaveCORE.GetProcessLogError(IdProcessDetail, Opcion);
            return Ok(response);
        }

        [Route("GetDataExport")]
        [HttpPost]
        public IHttpActionResult GetDataExport(ConfigFileBM configFile)
        {
            ReporteSucaveCORE reporteSucaveCORE = new ReporteSucaveCORE();
            
            if (configFile.IdFileConfig == 1) {
               var    response1 = reporteSucaveCORE.GetDataExport(configFile);
                return Ok(response1);
            }
            else {
              var  response2 = reporteSucaveCORE.GetDataExport2(configFile);
                return Ok(response2);
            }
           
          
        }

        [Route("GetDataExportCorrect")]
        [HttpPost]
        public IHttpActionResult GetDataExportCorrect(ConfigFileBM configFile)
        {
            ReporteSucaveCORE reporteSucaveCORE = new ReporteSucaveCORE();
            var response = reporteSucaveCORE.GetDataExportCorrect(configFile);
            return Ok(response);
        }
        [Route("GetConfigurationPath")]
        [HttpGet]
        public IHttpActionResult GetConfigurationPath(int Identity)
        {
            ReporteSucaveCORE reporteSucaveCORE = new ReporteSucaveCORE();
            var response = reporteSucaveCORE.GetConfigurationPath(Identity);
            return Ok(response);
        }
        [Route("GetConfigurationEntity")]
        [HttpGet]
        public IHttpActionResult GetConfigurationEntity()
        {
            ReporteSucaveCORE reporteSucaveCORE = new ReporteSucaveCORE();
            var response = reporteSucaveCORE.GetConfigurationEntity();
            return Ok(response);
        }

        [Route("GetConfigurationFiles")]
        [HttpGet]
        public IHttpActionResult GetConfigurationFiles(int IdPath)
        {
            ReporteSucaveCORE reporteSucaveCORE = new ReporteSucaveCORE();
            var response = reporteSucaveCORE.GetConfigurationFiles(IdPath);
            return Ok(response);
        }
        [Route("InsertHeaderProc")]
        [HttpPost]
        public IHttpActionResult InsertHeaderProc(ProcessHeaderBM HeaderProcess)
        {
            ReporteSucaveCORE reporteSucaveCORE = new ReporteSucaveCORE();
            var response = reporteSucaveCORE.InsertHeaderProc(HeaderProcess);
            return Ok(response);
        }
        [Route("GetProductsList")]
        [HttpGet]
        public IHttpActionResult GetProductsList(int IdBranch)
        {
            ReporteSucaveCORE reporteSucaveCORE = new ReporteSucaveCORE();
            var response = reporteSucaveCORE.GetProductsList(IdBranch);
            return Ok(response);
        }

        [Route("GenerateFacturacion")]
        [HttpGet]
        public IHttpActionResult GenerateFacturacion(int IdIdentity, int IdHeaderProcess)
        {
            ReporteSucaveCORE reporteSucaveCORE = new ReporteSucaveCORE();

            var IdDetailProcess = reporteSucaveCORE.InsertDetailProc(new ProcessDetailBM() { IdHeaderProcess = IdHeaderProcess.ToString(), IdFileConfig = "21", Status = Status.Registrate });

            var GetDataExport = this.GetDataExport(new ConfigFileBM { IdFileConfig = 21, IdHeaderProcess = IdHeaderProcess });

            // var response = reporteSucaveCORE.GetProductsList(IdBranch);
            return GetDataExport;
        }

        [Route("UploadFileProcess")]
        [HttpPost]
        public IHttpActionResult UploadFileProcess()
        {
            string codUser = HttpContext.Current.Request.Params.Get("UserCode").ToString();
            string idHeaderProcess = HttpContext.Current.Request.Params.Get("idHeaderProcess").ToString();
            string idDetailProcess = HttpContext.Current.Request.Params.Get("idDetailProcess").ToString();
            string idFileConfig = HttpContext.Current.Request.Params.Get("idFileConfig").ToString();
            string idIdentity = HttpContext.Current.Request.Params.Get("idIdentity").ToString();
            List<HttpPostedFile> adjuntoList = null;
            if (HttpContext.Current.Request.Files.Count > 0)
            {
                adjuntoList = new List<HttpPostedFile>(HttpContext.Current.Request.Files.GetMultiple("dataFile"));
            }
            ReporteSucaveCORE reporteSucaveCORE = new ReporteSucaveCORE();
            List<PathConfigBM> pathConfigBMs = reporteSucaveCORE.getPathConfig(Int32.Parse(idIdentity));

            var path = pathConfigBMs[0].Routereprocessing.ToString();

            string Folder = string.Format("Process-{0}", idHeaderProcess);


            CreateDirectory(ConcatRoute(path, Folder));

            if (adjuntoList != null)
            {
                foreach (var item in adjuntoList)
                {
                    item.SaveAs(ConcatRoute(path, Folder) + @"\\" + item.FileName);
                }
            }

            string arguments = string.Format(@"{0}|{1}|{2}|{3}|{4}\|{5}", pathConfigBMs[0].IdPathConfig, idHeaderProcess, idDetailProcess, idFileConfig, ConcatRoute(path, Folder), codUser);

            using (var process = new Process { StartInfo = new ProcessStartInfo { FileName = ELog.obtainConfig("pathMasivo"), Arguments = arguments } })
            {
                process.Start();
            }

            var response = "Correct";

            return Ok(response);
        }


        [Route("ProcessFiles")]
        [HttpPost]
        public IHttpActionResult ProcessFiles()
        {
            try
            {
                string codUser = HttpContext.Current.Request.Params.Get("UserCode").ToString();
                string IdEntity = HttpContext.Current.Request.Params.Get("IdEntity").ToString();
                string IdPath = HttpContext.Current.Request.Params.Get("IdPath").ToString();
                string IdProduct = HttpContext.Current.Request.Params.Get("IdProduct").ToString();

                var ListFileProcess = JsonConvert.DeserializeObject<List<FileConfigBM>>(HttpContext.Current.Request.Params.Get("ListFileProcess").ToString(), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                List<HttpPostedFile> adjuntoList = null;
                if (HttpContext.Current.Request.Files.Count > 0)
                {
                    adjuntoList = new List<HttpPostedFile>(HttpContext.Current.Request.Files.GetMultiple("dataFile"));
                }

                ProcessHeaderBM processHeaderBM = new ProcessHeaderBM
                {
                    IdIdentity = IdEntity,
                    StatusProc = Status.Registrate,
                    UserCode = codUser,
                    IdProduct = IdProduct
                };
                ReporteSucaveCORE reporteSucaveCORE = new ReporteSucaveCORE();

                var idHeaderProcess = reporteSucaveCORE.InsertHeaderProc(processHeaderBM);
                if ((Int32)idHeaderProcess.GenericResponse > 0)
                {
                    foreach (FileConfigBM DetailFile in ListFileProcess)
                    {
                        reporteSucaveCORE.InsertDetailProc(new ProcessDetailBM() { IdHeaderProcess = idHeaderProcess.GenericResponse.ToString(), IdFileConfig = DetailFile.IdFileConfig.ToString(), Status = Status.Registrate });
                    }
                }
                List<PathConfigBM> pathConfigBMs = reporteSucaveCORE.getPathConfig(Convert.ToInt16(IdEntity));

                var path = pathConfigBMs[0];

                CreateDirectory(path.RouteDestine);

                string Folder = string.Format("Process-{0}", idHeaderProcess.GenericResponse);

                CreateDirectory(ConcatRoute(path.RouteDestine, Folder));

                if (adjuntoList != null)
                {
                    foreach (var item in adjuntoList)
                    {
                        item.SaveAs(ConcatRoute(path.RouteDestine, Folder) + @"\\" + item.FileName);
                    }
                }

                string arguments = string.Format(@"{0}|{1}|{2}|{3}|{4}\|{5}", IdPath, idHeaderProcess.GenericResponse, 0, 0, ConcatRoute(path.RouteDestine, Folder), codUser);

                using (var process = new Process { StartInfo = new ProcessStartInfo { FileName = ELog.obtainConfig("pathMasivo"), Arguments = arguments } })
                {
                    process.Start();
                }

                var response = idHeaderProcess.GenericResponse;

                return Ok(response);

            }
            catch (Exception ex)
            {
                // Util.ELog.save(this,ex);
            }
            return null;
        }



        #region Utils

        public void CreateDirectory(string Folder)
        {

            if (!Directory.Exists(Folder))
            {
                Directory.CreateDirectory(Folder);
            }
        }

        private string ConcatRoute(string Folder, string File)
        {
            return string.Format(@"{0}\\{1}", Folder, File);
        }

        #endregion



        void IntegrateWithQuotation(List<HttpPostedFile> files, string codUser)
        {
            var asegurados = new List<LoadMasiveAsegurados>();
            var excluidos = new List<LoadMasiveExclusion>();
            var polizas = new List<LoadMasivePoliza>();

            DataTable dtAsegurados = new DataTable();

            foreach (var file in files)
            {
                var stream = file.InputStream;
                using (StreamReader sReader = new StreamReader(stream))
                {
                    var allText = sReader.ReadToEnd();
                    List<string> strList = allText.Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();
                    strList.RemoveAt(0);

                    if (file.FileName.Contains("Poliza"))
                    {
                        polizas = GetPolizasFromCSV(strList);
                    }
                    if (file.FileName.Contains("Asegurados"))
                    {
                        asegurados = GetAseguradosFromCSV(strList);
                    }
                    //if (file.FileName.Contains("Exclusion"))
                    //{
                    //    excluidos = GetExcluidosFromCSV(strList);
                    //}
                }
            }

            var polizasFromTramaPoliza = polizas.Select(m => m.POLIZA).Distinct().ToList();
            var polizasFromTramaAsegurados = asegurados.Select(m => m.Poliza).Distinct().ToList();

            var diferencias = polizasFromTramaPoliza.Where(m => !polizasFromTramaAsegurados.Contains(m)).ToList();
            if (diferencias.Any())
            {
                //Alguna de las polizas de la trama de polizas no se encuentra en la trama de asegurados.
                return;
            }

            var isCoorect = asegurados.Any(m => string.IsNullOrWhiteSpace(m.TipoTrabajador));
            if (isCoorect == false)
            {
                //En la trama de asegurados el campo del [tipo de trabajador] es obligatorio.
                return;
            }

            this.CreateQuotationOfTramaAsegurados(asegurados, polizas, files, codUser);



        }

        List<LoadMasivePoliza> GetPolizasFromCSV(List<string> allLines)
        {
            var polizas = new List<LoadMasivePoliza>();
            allLines.ForEach(m =>
            {
                polizas.Add(new LoadMasivePoliza(m.Split(';')));
            });
            return polizas;
        }

        List<LoadMasiveAsegurados> GetAseguradosFromCSV(List<string> allLines)
        {
            var asegurados = new List<LoadMasiveAsegurados>();
            allLines.ForEach(m =>
            {
                var entity = new LoadMasiveAsegurados(m.Split(';'));
                asegurados.Add(entity);
            });

            return asegurados;
        }

        List<LoadMasiveExclusion> GetExcluidosFromCSV(List<string> allLines)
        {
            var exclusions = new List<LoadMasiveExclusion>();
            allLines.ForEach(m =>
            {
                exclusions.Add(new LoadMasiveExclusion(m.Split(';')));
            });
            return exclusions;
        }

        void CreateQuotationOfTramaAsegurados(List<LoadMasiveAsegurados> tramaAsegurados, List<LoadMasivePoliza> tramaPolizas, List<HttpPostedFile> files, string codigoUsuario)
        {

            var quotationCORE = new QuotationCORE();
            var polizasTramaAsegurados = tramaAsegurados.Select(s => s.Poliza).Distinct().ToList();
            foreach (var policy in polizasTramaAsegurados)
            {
                var polizasNuevasEnProceso = tramaPolizas.Where(m => m.POLIZA == policy).ToList();

                var aseguradosPorPoliza = tramaAsegurados.Where(m => m.Poliza == policy).ToList();
                var tiposTransaccion = aseguradosPorPoliza.Select(s => s.Transaccion).Distinct().OrderBy(o => o).ToList();
                foreach (var tipoTrx in tiposTransaccion)
                {
                    var aseguradosPorPolizaPorTipoTrx = aseguradosPorPoliza.Where(m => m.Transaccion == tipoTrx).ToList();
                    var firstRow = aseguradosPorPolizaPorTipoTrx.FirstOrDefault();

                    //1. Insertar trama - Esto es para todos los procesos (Emision, Renovación, Inclusión, Exclusión)
                    var index = 0;
                    var codProceso = (codigoUsuario + new QuotationController().GenerarCodigo() + DateTime.Now.ToString("yyyyMMddHHmmss")).PadLeft(30, '0');
                    var dtAsegurados = new DataTable();
                    dtAsegurados = this.SetColumnsAsegurados();

                    aseguradosPorPolizaPorTipoTrx.ForEach(entity =>
                    {
                        index++;
                        dtAsegurados.Rows.Add(this.SetDataRowAsegurados(dtAsegurados, entity, index, codProceso));
                    });

                    var responseInsertTrama = quotationCORE.SaveUsingOracleBulkCopy(dtAsegurados);



                    //2. Obtener datos de cotización si ya exise la poliza.
                    QuotationCabBM cotizacion = new QuotationCabBM();
                    if (polizasNuevasEnProceso != null && polizasNuevasEnProceso.Any())
                    {
                        var newPolicy = polizasNuevasEnProceso.FirstOrDefault();
                        cotizacion.P_SCLIENT = newPolicy.NUM_DOC;
                        cotizacion.P_NCURRENCY = int.Parse(newPolicy.MONEDA);
                        cotizacion.P_NBRANCH = int.Parse(newPolicy.RAMO);
                        cotizacion.P_DSTARTDATE = this.GetDate(newPolicy.INICIO_VIGENCIA);
                        cotizacion.P_DEXPIRDAT = this.GetDate(newPolicy.FIN_VIGENCIA);
                        cotizacion.P_NIDCLIENTLOCATION = 1;
                        cotizacion.P_SCOMMENT = string.Empty;
                        cotizacion.P_SRUTA = @"Cotizacion\\";
                        cotizacion.P_NUSERCODE = int.Parse(codigoUsuario);
                        cotizacion.P_NACT_MINA = 0;
                        cotizacion.P_NTIP_RENOV = int.Parse(newPolicy.RENOVACION);
                        cotizacion.P_NPAYFREQ = int.Parse(newPolicy.FRECUENCIA_PAGO);
                        cotizacion.P_SCOD_ACTIVITY_TEC = newPolicy.TIPO_NEGOCIO;
                        cotizacion.P_SCOD_CIUU = null;
                        cotizacion.P_NTIP_NCOMISSION = int.Parse(newPolicy.TIPO_COMISION);
                        cotizacion.P_NPRODUCT = int.Parse(newPolicy.PRODUCTO);
                        cotizacion.P_NCOMISION_SAL_PR = 0;
                        cotizacion.P_DSTARTDATE_ASE = this.GetDate(firstRow.InicioVigencia);
                        cotizacion.P_DEXPIRDAT_ASE = this.GetDate(firstRow.FinVigencia);
                        cotizacion.P_NEPS = 0;
                        cotizacion.P_QUOTATIONNUMBER_EPS = string.Empty;
                        cotizacion.planId = 0; //=>crear funcion para obtener el plan

                        cotizacion.QuotationDet = new List<QuotationDetBM>();
                        cotizacion.QuotationCom = new List<QuotizacionComBM>();

                        var modulos = aseguradosPorPolizaPorTipoTrx.Select(m => m.TipoTrabajador).Distinct().ToList();
                        modulos.ForEach(item =>
                        {
                            var planillaPorModulo = aseguradosPorPolizaPorTipoTrx.Where(m => m.TipoTrabajador == item);
                            cotizacion.QuotationDet.Add(new QuotationDetBM
                            {
                                P_NBRANCH = int.Parse(newPolicy.RAMO),
                                P_NPRODUCT = int.Parse(newPolicy.PRODUCTO),
                                P_NMODULEC = item == "E" ? "EMPLEADO" : "OBRERO",
                                P_NTOTAL_TRABAJADORES = planillaPorModulo.Count(),
                                P_NMONTO_PLANILLA = planillaPorModulo.Sum(s => decimal.Parse(s.Salario)),
                                P_NTASA_CALCULADA = 0,
                                P_NTASA_PROP = 0, //--OK
                                P_NPREMIUM_MENSUAL = planillaPorModulo.Sum(s => decimal.Parse(s.Prima)), //-- SUMAR CAMPO PRIMA DE LA TRAMA DE ASEGURADOS
                                P_NPREMIUM_MIN = 0,//-- TIENE Q SALIR DEL TARIFARIO POR EL MOMENTO 0
                                P_NPREMIUM_MIN_PR = 0, //OK 0
                                P_NPREMIUM_END = 0, //--END = ENDOSO => 0
                                P_NSUM_PREMIUMN = planillaPorModulo.Sum(s => decimal.Parse(s.Prima)), //--SUMAR CAMPO PRIMA DE LA TRAMA DE ASEGURADOS
                                P_NSUM_IGV = this.GetImporteIGV(planillaPorModulo.Sum(s => decimal.Parse(s.Prima)), Convert.ToInt32(newPolicy.FRECUENCIA_PAGO)), //-- REVISAR EL CALCULO EN EL FRONT
                                P_NSUM_PREMIUM = planillaPorModulo.Sum(s => decimal.Parse(s.Prima)) + this.GetImporteIGV(planillaPorModulo.Sum(s => decimal.Parse(s.Prima)), Convert.ToInt32(newPolicy.FRECUENCIA_PAGO)), //-- P_NSUM_PREMIUMN + P_NSUM_IGV
                                //P_NUSERCODE = Convert.ToInt32(codigoUsuario),
                                P_NRATE = 0,
                                P_NDISCOUNT = 0,
                                P_NACTIVITYVARIATION = 0,
                                P_FLAG = 0
                            });
                        });

                        cotizacion.QuotationCom.Add(new QuotizacionComBM
                        {
                            P_NIDTYPECHANNEL = Convert.ToInt32(newPolicy.TIPO_INTERMEDIARIO_INT), //--TIPO_INTERMEDIARIO_INT
                            P_NINTERMED = Convert.ToInt32(newPolicy.NUM_DOC_INT),
                            P_SCLIENT_COMER = "", //--OBTENER DEL FRONT SACAR EL CODIGO DE INTERMEDIARIO.
                            P_NCOMISION_SAL = 0,
                            P_NCOMISION_SAL_PR = 0,
                            P_NCOMISION_PEN = 0,
                            P_NCOMISION_PEN_PR = 0,
                            P_NPRINCIPAL = 0,
                            P_NSHARE = null
                        });
                    }
                    else
                    {
                        cotizacion = GetQuotationCab(quotationCORE, firstRow, responseInsertTrama);
                    }

                    if (responseInsertTrama.P_COD_ERR == "0")
                    {
                        continue;
                    }

                    if (tipoTrx == "E") //1.Emisión
                    {
                        var responseInsert = quotationCORE.InsertQuotation(cotizacion, files, null);
                    }
                    else if (tipoTrx == "I") //2.Inclusión
                    {

                    }
                    else if (tipoTrx == "R") //3.Renovación
                    {

                    }
                }
            }
        }

        QuotationCabBM GetQuotationCab(QuotationCORE quotationCORE, LoadMasiveAsegurados firstRow, SalidaTramaBaseVM responseInsertTrama)
        {
            var datosCotizacion = quotationCORE.GetQuotationByPolicy(Convert.ToInt32(firstRow.Poliza));

            var cotizacion = new QuotationCabBM();
            cotizacion.CodigoProceso = responseInsertTrama.NIDPROC;
            cotizacion.P_NBRANCH = Convert.ToInt32(firstRow.Ramo);
            cotizacion.P_DSTARTDATE = this.GetDate(firstRow.InicioVigencia);
            cotizacion.P_DEXPIRDAT = this.GetDate(firstRow.FinVigencia);
            return cotizacion;
        }
        DataTable SetColumnsAsegurados()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("NID_COTIZACION");
            dt.Columns.Add("NID_PROC");
            dt.Columns.Add("SFIRSTNAME");
            dt.Columns.Add("SLASTNAME");
            dt.Columns.Add("SLASTNAME2");
            dt.Columns.Add("NMODULEC");
            dt.Columns.Add("NNATIONALITY");
            dt.Columns.Add("NIDDOC_TYPE");
            dt.Columns.Add("SIDDOC");
            dt.Columns.Add("DBIRTHDAT");
            dt.Columns.Add("NREMUNERACION");
            dt.Columns.Add("NIDCLIENTLOCATION");
            dt.Columns.Add("NCOD_NETEO");
            dt.Columns.Add("NUSERCODE");
            dt.Columns.Add("SSTATREGT");
            dt.Columns.Add("SCOMMENT");
            dt.Columns.Add("NID_REG");
            dt.Columns.Add("SSEXCLIEN");
            dt.Columns.Add("NACTION", typeof(int));
            dt.Columns.Add("SROLE");
            dt.Columns.Add("SCIVILSTA");
            dt.Columns.Add("SSTREET");
            dt.Columns.Add("SPROVINCE");
            dt.Columns.Add("SLOCAL");
            dt.Columns.Add("SMUNICIPALITY");
            dt.Columns.Add("SE_MAIL");
            dt.Columns.Add("SPHONE_TYPE");
            dt.Columns.Add("SPHONE");

            return dt;
        }

        DataRow SetDataRowAsegurados(DataTable dt, LoadMasiveAsegurados entity, int index, string codProceso)
        {
            DataRow dr = dt.NewRow();
            dr["NID_COTIZACION"] = null;
            dr["NID_PROC"] = codProceso;
            dr["SFIRSTNAME"] = entity.Nombres;
            dr["SLASTNAME"] = entity.ApePaterno;
            dr["SLASTNAME2"] = entity.ApeMaterno;
            dr["NMODULEC"] = entity.TipoTrabajador == "E" ? "Empleado" : "Obrero";
            dr["NNATIONALITY"] = entity.Nacionalidad;
            dr["NIDDOC_TYPE"] = entity.TipoDocumento == "1" ? "DNI" : "";
            dr["SIDDOC"] = entity.NumeroDocumento;
            //var fechav = dataTable.Rows[rows][7].ToString().Trim().Split(' ').ToList();
            //var fnacimientov = fechav.Count() == 1 ? fechav[0] : Convert.ToDateTime(dataTable.Rows[rows][7].ToString()).ToString("dd/MM/yyyy");
            //string fecha = IsDate(dataTable.Rows[rows][7].ToString().Trim()) ? fnacimientov : dataTable.Rows[rows][7].ToString().Trim();
            dr["DBIRTHDAT"] = this.GetDate(entity.FechaNacimiento); //entity.FechaNacimiento.Substring(6, 2) + "/" + entity.FechaNacimiento.Substring(4, 2) + "/" + entity.FechaNacimiento.Substring(0, 4);
            dr["NREMUNERACION"] = entity.Salario;
            dr["SE_MAIL"] = entity.Correo;
            dr["SPHONE_TYPE"] = entity.TipoTelefono == "" ? null : "Celular";
            dr["SPHONE"] = entity.Telefono;
            dr["NIDCLIENTLOCATION"] = string.Empty;
            dr["NCOD_NETEO"] = string.Empty;
            dr["NUSERCODE"] = string.Empty;
            dr["SSTATREGT"] = "1";
            dr["SCOMMENT"] = null;
            dr["NID_REG"] = index.ToString();
            dr["SSEXCLIEN"] = entity.Sexo == "1" ? "M" : "F";
            dr["NACTION"] = null;
            //rows++;
            //count++;
            //dt.Rows.Add(dr);


            return dr;
        }

        string GetDate(string val)
        {
            var dateFormat = val.Substring(6, 2) + "/" + val.Substring(4, 2) + "/" + val.Substring(0, 4);
            return dateFormat;
        }

        decimal GetImporteIGV(decimal sumaPrima, int frecuenciaPago)
        {
            decimal importeMensual = 0;
            if (frecuenciaPago == 1)
            {
                importeMensual = sumaPrima / 12;
            }
            else if (frecuenciaPago == 2)
            {
                importeMensual = sumaPrima / 6;
            }
            else if (frecuenciaPago == 3)
            {
                importeMensual = sumaPrima / 3;
            }
            else if (frecuenciaPago == 4)
            {
                importeMensual = sumaPrima / 2;
            }
            else if (frecuenciaPago == 5)
            {
                importeMensual = sumaPrima;
            }
            else
            {
                importeMensual = sumaPrima;
            }

            return (decimal)(importeMensual * 0.18m);
        }

        [Route("GetDataExportTxt")]
        [HttpPost]
        public IHttpActionResult GetDataExportTxt(ConfigFileBM configFile)
        {
            ReporteSucaveCORE reporteSucaveCORE = new ReporteSucaveCORE();
            var response1 = reporteSucaveCORE.GetDataExportTxt(configFile);
            return Ok(response1);
        }

        #endregion
    }
}
