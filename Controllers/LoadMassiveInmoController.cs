using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Cors;
using WSPlataforma.Core;
using WSPlataforma.Entities.PolicyModel.BindingModel;
using System.Web.Http;
using System.Configuration;
using WSPlataforma.Entities.LoadMassiveInmoModel;
using WSPlataforma.Entities.LoadMassiveModel;
using WSPlataforma.Entities.LoadMassiveInmoModel.ViewModel;
using WSPlataforma.Entities.ReportModel.BindingModel;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;
using WSPlataforma.Util;
using WSPlataforma.Entities.ViewModel;
using ExcelDataReader;
using System.Text;
using System.Data;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

namespace WSPlataforma.Controllers
{
    [RoutePrefix("Api/LoadMassiveIM")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class LoadMassiveInmoController : ApiController
    {
        [Route("GetBranchList")]
        [HttpGet]
        public List<BranchVM> GetBranchList(string branch = null)
        {
            LoadMassiveInmoCORE loadMassiveCORE = new LoadMassiveInmoCORE();
            return loadMassiveCORE.GetBranchList(branch);
        }
        [Route("ProcessHeaderList")]
        [HttpPost]
        public IHttpActionResult ProcessHeaderList(DisplayProcessBM_Inmo request)
        {
            LoadMassiveInmoCORE loadMassiveCORE = new LoadMassiveInmoCORE();
            var response = loadMassiveCORE.GetProcessHeader(request.P_NBRANCH, request.P_DEFFECDATE, request.P_NPRODUCT);
            return Ok(response);
        }

        [Route("ProcessDetailList")]
        [HttpGet]
        public IHttpActionResult ProcessDetailList(int IdProcessHeader)
        {
            LoadMassiveInmoCORE LoadMassiveIMCORE = new LoadMassiveInmoCORE();
            var response = LoadMassiveIMCORE.GetProcessDetail(IdProcessHeader);
            return Ok(response);
        }

        [Route("GetConfigurationPath")]
        [HttpGet]
        public IHttpActionResult GetConfigurationPath(int Identity)
        {
            LoadMassiveInmoCORE loadMassiveCORE = new LoadMassiveInmoCORE();
            var response = loadMassiveCORE.GetConfigurationPath(Identity);
            return Ok(response);
        }
        [Route("GetConfigurationEntity")]
        [HttpGet]
        public IHttpActionResult GetConfigurationEntityInmo()
        {
            LoadMassiveInmoCORE loadMassiveInmoCORE = new LoadMassiveInmoCORE();
            var response = loadMassiveInmoCORE.GetConfigurationEntity();
            return Ok(response);
        }
        [Route("GetConfigurationFiles")]
        [HttpGet]
        public IHttpActionResult GetConfigurationFilesInmo(int IdPath)
        {
            LoadMassiveInmoCORE loadMassiveInmoCORE = new LoadMassiveInmoCORE();
            var response = loadMassiveInmoCORE.GetConfigurationFiles(IdPath);
            return Ok(response);
        }

        [Route("GetProductsList")]
        [HttpGet]
        public IHttpActionResult GetProductsList(int IdBranch)
        {
            LoadMassiveInmoCORE LoadMassiveIMCORE = new LoadMassiveInmoCORE();
            var response = LoadMassiveIMCORE.GetProductsList(IdBranch);
            return Ok(response);
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
            int codeTRX =0; string TablaTrama =string.Empty;

            List<HttpPostedFile> adjuntoList = null;
            if (HttpContext.Current.Request.Files.Count > 0)
            {
                adjuntoList = new List<HttpPostedFile>(HttpContext.Current.Request.Files.GetMultiple("dataFile"));
            }
            LoadMassiveInmoCORE loadMassiveCORE = new LoadMassiveInmoCORE();
            List<PathConfigBM> pathConfigBMs = loadMassiveCORE.getPathConfig(Int32.Parse(idIdentity));

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

            //string arguments = string.Format(@"{0}|{1}|{2}|{3}|{4}\|{5}", pathConfigBMs[0].IdPathConfig, idHeaderProcess, idDetailProcess, idFileConfig, ConcatRoute(path, Folder), codUser);

            //using (var process = new Process { StartInfo = new ProcessStartInfo { FileName = ELog.obtainConfig("pathMasivo"), Arguments = arguments } })
            //{
            //    process.Start();
            //}


            // var rpta = insertarTrama(int.Parse(idHeaderProcess),null,  int.Parse(codUser));
            /***********************************/
            codeTRX = int.Parse(idFileConfig);

            if (codeTRX == 17) { TablaTrama = "TBL_IM_TRX_TRAMA_EMISION"; }
            if (codeTRX == 21) { TablaTrama = "TBL_IM_TRX_TRAMA_FACTURACION"; }
                        
            var rpta = insertarTrama(int.Parse(idHeaderProcess),int.Parse(idDetailProcess), codeTRX, TablaTrama, int.Parse(codUser));


            var response = "Correct";

            return Ok(response);
        }

        [Route("ProcessFiles")]
        [HttpPost]
        public IHttpActionResult ProcessFiles()
        {            
            LoadMassiveInmoCORE loadMassiveCORE = new LoadMassiveInmoCORE();
            TramaInmoVMResponse Respuesta = new TramaInmoVMResponse();
            DataTable dTable;
            DataSet dsExcel;
            string campoERR = string.Empty;
            string FileName = string.Empty;
            try
            {                
                string codUser = HttpContext.Current.Request.Params.Get("UserCode").ToString();
                string IdEntity = HttpContext.Current.Request.Params.Get("IdEntity").ToString();
                string IdPath = HttpContext.Current.Request.Params.Get("IdPath").ToString();
                string IdProduct = HttpContext.Current.Request.Params.Get("IdProduct").ToString();
                int cantXLS = 0; int codeTRX = 0;  string NameTableTrama = string.Empty;
                

                var ListFileProcess = JsonConvert.DeserializeObject<List<FileConfigBM>>(HttpContext.Current.Request.Params.Get("ListFileProcess").ToString(), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });                

                List<HttpPostedFile> adjuntoList = null;
                if (HttpContext.Current.Request.Files.Count > 0)
                {
                    adjuntoList = new List<HttpPostedFile>(HttpContext.Current.Request.Files.GetMultiple("dataFile"));
                }

                ProcessHeaderBM processHeaderBM = new ProcessHeaderBM
                {
                    IdIdentity = IdEntity,
                    StatusProc = Entities.LoadMassiveModel.Status.Registrate,
                    UserCode = codUser,
                    IdProduct = IdProduct
                };

                NameTableTrama = ListFileProcess[0].table_reference;
                codeTRX = ListFileProcess[0].IdFileConfig;
                FileName = adjuntoList[0].FileName.ToString();

                List<ConfigFields> lstFieldTrama = loadMassiveCORE.GetTramaFields(codeTRX, 1); // OBTIENE CONFIGURACION TRAMA REQUERIDA

                /**********OBTIENE EXCEL************/

                dsExcel = ObtenerExcel();
                dTable = dsExcel.Tables[0];
                
                cantXLS = dTable.Columns.Count;
            
                /***********************************/

                if (lstFieldTrama.Count <= cantXLS) // compara catidad de campos de trama vs configuracion
                {

                    // COMNPARA LOS CAMPOS DE LA TRAMA EXCEL Vs CAMPOS CONFIGURADOS
                    for (int i = 0; i <= cantXLS - 1; i++)
                    {
                        var columnTramaXLS = dTable.Rows[0][i].ToString().Trim();

                        if (lstFieldTrama[i].SNAME_EQ != columnTramaXLS.ToString())
                        {
                            campoERR = campoERR + ", " + columnTramaXLS.ToString();
                        }
                    }

                    if (campoERR == string.Empty)
                    {

                        var idHeaderProcess = loadMassiveCORE.InsertHeaderProc(processHeaderBM);
                        if ((Int32)idHeaderProcess.GenericResponse > 0)
                        {
                            foreach (FileConfigBM DetailFile in ListFileProcess)
                            {
                                var idDetailProcess = loadMassiveCORE.InsertDetailProc(new ProcessDetailBM() { IdHeaderProcess = idHeaderProcess.GenericResponse.ToString(), IdFileConfig = DetailFile.IdFileConfig.ToString(), Status = Entities.LoadMassiveModel.Status.Registrate, FileName = FileName });

                                //CARGA TRAMA XLS INMOBILIARIA
                                var rpta = loadMassiveCORE.SaveUsingOracleBulkCopy(dTable, codeTRX, NameTableTrama, (Int32)idHeaderProcess.GenericResponse, (Int32)idDetailProcess.GenericResponse, int.Parse(codUser));
                            }
                        }

                        List<PathConfigBM> pathConfigBMs = loadMassiveCORE.getPathConfig(Convert.ToInt16(IdEntity));

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

                        var response = idHeaderProcess.GenericResponse;


                        return Ok(response);
                    }
                    else // CUANDO LOS CAMPOS DE LA TRAMA NO ESTAN CONFIGURADOS
                    {
                        Respuesta.MESSAGE = "Campos no configurados en la trama: " + campoERR.Substring(2,(campoERR.Length -2 )) + ". Modifique el nombre de los campos o seleccione otro archivo.";
                        Respuesta.NCODE = "1";
                        return Ok(Respuesta);
                    }
                }
                else
                {
                    Respuesta.NCODE = "1";
                    Respuesta.MESSAGE = "El archivo seleccionado no tiene el formato correcto. Seleccione otro archivo";
                    return Ok(Respuesta);
                }

            }
            catch (Exception ex)
            {                
                Respuesta.NCODE = "1";
                Respuesta.MESSAGE = "El archivo seleccionado no tiene el formato correcto. Seleccione otro archivo";
                return Ok(Respuesta);
            }

        }

        private IHttpActionResult insertarTrama(int P_NIDHEADERPROC,int P_NIDDETAILPROC, int codeTRX, string TablaTrama, int codUser)
        {
            LoadMassiveInmoCORE loadMassiveInmoCORE = new LoadMassiveInmoCORE();

            var response = new TramaInmoVMResponse();

            try
            {
                HttpPostedFile upload = HttpContext.Current.Request.Files["dataFile"];
                Stream stream = upload.InputStream;
                IExcelDataReader reader = null;

                if (upload.FileName.EndsWith(".xls"))
                {
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                else if (upload.FileName.EndsWith(".xlsx") || upload.FileName.EndsWith(".xlsm"))
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }

                if (reader != null)
                {
                    DataSet result = reader.AsDataSet();
                    reader.Close();

                    response = loadMassiveInmoCORE.SaveUsingOracleBulkCopy(result.Tables[0], codeTRX, TablaTrama, P_NIDHEADERPROC, P_NIDDETAILPROC, codUser);

                    if (response.NCODE == "1")
                    {
                    }
                }

            }
            catch (Exception ex)
            {

                LogControl.save("validarTramaVL", ex.ToString(), "3");
            }

            return Ok(response);
        }

        [Route("GetDataExport")]
        [HttpPost]
        public IHttpActionResult GetDataExport(ConfigFileBM configFile)
        {
            LoadMassiveInmoCORE loadMassiveCORE = new LoadMassiveInmoCORE();
            var response = loadMassiveCORE.GetDataExport(configFile);
            return Ok(response);
        }

        [Route("GetDataExportCorrect")]
        [HttpPost]
        public IHttpActionResult GetDataExportCorrect(ConfigFileBM configFile)
        {
            LoadMassiveInmoCORE loadMassiveCORE = new LoadMassiveInmoCORE();
            var response = loadMassiveCORE.GetDataExportCorrect(configFile);
            return Ok(response);
        }

        [Route("GetProcessLogError")]
        [HttpGet]
        public IHttpActionResult GetProcessLogError(int IdProcessDetail, string Opcion)
        {
            LoadMassiveInmoCORE loadMassiveCORE = new LoadMassiveInmoCORE();
            var response = loadMassiveCORE.GetProcessLogError(IdProcessDetail, Opcion);
            return Ok(response);
        }

        public DataSet ObtenerExcel()
        {
            LoadMassiveInmoCORE loadMassiveInmoCORE = new LoadMassiveInmoCORE();
            DataSet result = new DataSet();
            var response = new TramaInmoVMResponse();

            try
            {
                HttpPostedFile upload = HttpContext.Current.Request.Files["dataFile"];
                Stream stream = upload.InputStream;
                IExcelDataReader reader = null;

                if (upload.FileName.EndsWith(".xls"))
                {
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                else if (upload.FileName.EndsWith(".xlsx") || upload.FileName.EndsWith(".xlsm"))
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }

                if (reader != null)
                {
                    result = reader.AsDataSet();
                    reader.Close();
                }

            }
            catch (Exception ex)
            {

                LogControl.save("Error obtener trama Inmobiliaria", ex.ToString(), "3");
            }

            return result;
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

    }
}