/*************************************************************************************************/
/*  NOMBRE              :   CreditNoteController.CS                                              */
/*  DESCRIPCION         :   Capa Controller                                                      */
/*  AUTOR               :   MATERIAGRIS - JOSUE CORONEL FLORES                                   */
/*  FECHA               :   18-10-2022                                                           */
/*  VERSION             :   1.0                                                                  */
/*************************************************************************************************/

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WSPlataforma.Entities.PreliminaryModel.BindingModel;
using WSPlataforma.Entities.ReportModel.BindingModel;
using WSPlataforma.Core;
using WSPlataforma.Entities.CreditNoteModel.ViewModel;
using System.Web.Http.Cors;
using WSPlataforma.Util;
using WSPlataforma.Core;
using System.Web;
using ExcelDataReader;
using System.Data;
using System.Text.RegularExpressions;

namespace WSPlataforma.Controllers
{
    [RoutePrefix("Api/NoteCredit")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class NoteCreditRefundController : ApiController
    {
        NoteCreditRefundCore NoteCreditRefundCore = new NoteCreditRefundCore();

        #region Filters

        [Route("Branches")]
        [HttpGet]
        public IHttpActionResult GetBranches()
        {
            var result = this.NoteCreditRefundCore.GetBranches();
            return Ok(result);
        }

        [Route("GetProductsListByBranch")]
        [HttpPost]
        public IHttpActionResult GetProductsListByBranch(ProductVM data)
        {
           
            var response = this.NoteCreditRefundCore.GetProductsListByBranch(data);

            return Ok(response);
            
        }

        [Route("GetParameter")]
        [HttpPost]
        public IHttpActionResult GetParameter(ProductVM data)
        {

            var response = this.NoteCreditRefundCore.GetParameter(data);

            return Ok(response);

        }

        [Route("GetDocumentTypeList")]
        [HttpGet]
        public IHttpActionResult GetDocumentTypeList()
        {
            var result = this.NoteCreditRefundCore.GetDocumentTypeList();
            return Ok(result);
        }

        [Route("GetBillTypeList")]
        [HttpGet]
        public IHttpActionResult GetBillTypeList()
        {
            var result = this.NoteCreditRefundCore.GetBillTypeList();
            return Ok(result);
        }
        #endregion
        //
        [Route("pruebas")]
        [HttpPost]
        public IHttpActionResult pruebas(PremiumVM data)
        {
            var result = this.NoteCreditRefundCore.pruebas(data);
            return Ok(result);
        }


        [Route("GetListPremium")]
        [HttpPost]
        public IHttpActionResult GetListPremium(PremiumVM data)
        {
            var result = this.NoteCreditRefundCore.GetListPremium(data);
            return Ok(result);
        }

        [Route("GetListReciDev")]
        [HttpPost]
        public IHttpActionResult GetListReciDev(PremiumVM[] data)
        {
            var result = this.NoteCreditRefundCore.GetListReciDev(data);
            return Ok(result);
        }

        [Route("GetListDetRecDev")]
        [HttpPost]
        public IHttpActionResult GetListDetRecDev(PremiumVM data)
        {
            var result = this.NoteCreditRefundCore.GetListDetRecDev(data);
            return Ok(result);
        }

        [Route("GetFilaResi")]
        [HttpPost]
        public IHttpActionResult GetFilaResi(PremiumVM data)
        {
            var result = this.NoteCreditRefundCore.GetFilaResi(data);
            return Ok(result);
        }

        [Route("valCargaMasiva")]
        [HttpPost]
        public IHttpActionResult valCargaMasiva(PremiumVM data)
        {
            var result = this.NoteCreditRefundCore.valCargaMasiva(data);
            return Ok(result);
        }
        [Route("insertarTrama")]
        [HttpPost]
        public IHttpActionResult insertarTrama()
        {
            var response = new PremiumVMResponse();
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

                    response = tramaMigrante(result.Tables[0]);

                }

            }
            catch (Exception ex)
            {

                LogControl.save("validarTramaVL", ex.ToString(), "3");
            }

            return Ok(response);
        }

        private PremiumVMResponse tramaMigrante(DataTable dataTable) 
        {
            var response = new PremiumVMResponse();
            Random rnd = new Random();
            DateTime thisDay = DateTime.Today;
            string time = DateTime.Now.ToString("hhmmss");

            try 
            {
                Regex regex = new Regex("\\s+");
                Int32 rows = 1;
                string skey = "m"+ rnd.Next(1000, 999999)+ "a"+thisDay.ToString("yyyyMMdd")+ time;

                DataTable dt = new DataTable();
                dt.Columns.Add("NID_PROCESS");
                dt.Columns.Add("DFECHA_INICIO_NC");
                dt.Columns.Add("NID_TIPO_DOCUMENTO");
                dt.Columns.Add("STIPO_DOCUMENTO");
                dt.Columns.Add("SID_DOCUMENTO");
                dt.Columns.Add("SCOMPROBANTE");
                dt.Columns.Add("NPRIMA_DEVOLVER");
                dt.Columns.Add("SKEY");

                while (rows < dataTable.Rows.Count) 
                {
                    DataRow dr = dt.NewRow();
                    dr["NID_PROCESS"] = rows;
                    dr["DFECHA_INICIO_NC"] = dataTable.Rows[rows][0].ToString().Trim() == "" ? "" : dataTable.Rows[rows][0].ToString().Trim();
                    dr["NID_TIPO_DOCUMENTO"] = dataTable.Rows[rows][1].ToString().Trim() == "" ? "" : dataTable.Rows[rows][1].ToString().Trim();
                    dr["STIPO_DOCUMENTO"] = null;
                    dr["SID_DOCUMENTO"] = dataTable.Rows[rows][2].ToString().Trim() == "" ? "" : dataTable.Rows[rows][2].ToString().Trim();
                    dr["SCOMPROBANTE"] = dataTable.Rows[rows][3].ToString().Trim() == "" ? "" : dataTable.Rows[rows][3].ToString().Trim();
                    dr["NPRIMA_DEVOLVER"] = dataTable.Rows[rows][4].ToString().Trim() == "" ? "" : dataTable.Rows[rows][4].ToString().Trim();
                    dr["SKEY"] = skey;
                    rows++;
                    dt.Rows.Add(dr);
                }

                if (rows >1) 
                {
                    response = this.NoteCreditRefundCore.SaveUsingOracleBulkCopy(dt);
                }

            }
            catch (Exception ex)
            {

                LogControl.save("tramaMigrante", ex.ToString(), "1");
            }

            return (response);
        }

        // Genera la nc de la acrga masaiva //migrantes 12/09/2023
        [Route("genNcMasiva")]
        [HttpPost]
        public IHttpActionResult genNcMasiva(PremiumVM data)
        {
            var result = this.NoteCreditRefundCore.genNcMasiva(data);
            return Ok(result);
        }


    }
}