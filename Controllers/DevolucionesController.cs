/*************************************************************************************************/
/*  NOMBRE              :   NoteCreditRefundController.CS                                        */
/*  DESCRIPCION         :   Capa API COntroller, recibe la llamada desde el front-end.           */
/*  AUTOR               :   MATERIAGRIS - JORGE LUIS BEDON GONZALES                              */
/*  FECHA               :   02/06/2022                                                           */
/*  VERSION             :   1.0 - Devoluciones                                                   */
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
using System.Net.Mail;
using System.Net.Mime;
using WSPlataforma.Entities.DevolucionesModel.ViewModel;
using System.Web.Http.Cors;
using WSPlataforma.Util;
using WSPlataforma.Core;

namespace WSPlataforma.Controllers
{
    [RoutePrefix("Api/Devoluciones")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class DevolucionesController : ApiController
    {
       
        DevolucionesCore NoteCreditRefundCore = new DevolucionesCore();

        #region Filters

        [Route("ListarBancosCajas")]
        [HttpGet]
        public IHttpActionResult ListarBancosCajas()
        {
            var result = this.NoteCreditRefundCore.ListarBancosCajas();
            return Ok(result);
        }

        [Route("ListarMotivoDev")]
        [HttpGet]
        public IHttpActionResult ListarMotivoDev()
        {
            var result = this.NoteCreditRefundCore.ListarMotivoDev();
            return Ok(result);
        }
        [Route("ListarTipoDevoluciones")]
        [HttpGet]
        public IHttpActionResult ListarTipoDevoluciones()
        {
            var result = this.NoteCreditRefundCore.ListarTipoDevoluciones();
            return Ok(result);
        }
        [Route("ListarTipoDoc")]
        [HttpGet]
        public IHttpActionResult ListarTipoDoc()
        {
            var result = this.NoteCreditRefundCore.ListarTipoDoc();
            return Ok(result);
        }
        [Route("ListarCombosDevoluciones")]
        [HttpGet]
        public IHttpActionResult ListarCombosDevoluciones()
        {
            var result = this.NoteCreditRefundCore.ListarCombosDevoluciones();
            return Ok(result);
        }



        [Route("VizualizarDevoluciones")]
        [HttpPost]
        public IHttpActionResult VizualizarDevoluciones(VisualizarVM data)
        {

            var response = this.NoteCreditRefundCore.VizualizarDevoluciones(data);

            return Ok(response);

        }

        [Route("EnviarExactus")]
        [HttpPost]
        public IHttpActionResult EnviarExactus(EnviarExactusVM data)
        {

            var response = this.NoteCreditRefundCore.EnviarExactus(data);

            return Ok(response);

        }
        [Route("SeleccionarBanco")]
        [HttpPost]
        public IHttpActionResult SeleccionarBanco(BancoProtecta data)
        {

            var response = this.NoteCreditRefundCore.SeleccionarBanco(data);

            return Ok(response);

        }
        //--------------Reversion----------------//C:\@MateriaGris\devolucionesPD\protecta.kuntur.back\WSPlataforma\Model\

        [Route("ListarMetodosBusqueda")]
        [HttpGet]
        public IHttpActionResult ListarMetodosBusqueda()
        {
            var result = this.NoteCreditRefundCore.ListarMetodosBusqueda();
            return Ok(result);
        }

        [Route("ListarBusquedaCombo")]
        [HttpPost]
        public IHttpActionResult ListarBusquedaCombo(ReversionVM data)
        {

            var response = this.NoteCreditRefundCore.ListarBusquedaCombo(data);

            return Ok(response);

        }

        [Route("EnviarCorreoReversion")]
        [HttpPost]
        public IHttpActionResult EnviarCorreoReversion(CorreoEnviarReversionVM data)
        {
            string Respuesta;

            //CORREO A CLIENTE 
            //CORREO A COBRANZAS

            if (EnviarCorreoReversionCli(data) == "Todo Salio Correctamente")
            {
                if (EnviarCorreoReversionCobranzas(data) == "Todo Salio Correctamente")
                {
                    this.NoteCreditRefundCore.EnviarCorreoReversion(data);
                    Respuesta = "Se ingreso Correctamente";
                }
                else
                {
                    Respuesta = "Fallo el Proceso";
                }
            }
            else
            {
                Respuesta = "Fallo el Proceso";
            }
            return Ok(Respuesta);
        }
        public string EnviarCorreoReversionCli(CorreoEnviarReversionVM data)
        {
            string respuesta;
            string subject = "Devolución Protecta Security";
            var mm = new MailMessage();
            MailAddress from = new MailAddress("operaciones_sctr@protectasecurity.pe", "PROTECTA SECURITY");
            mm.From = from;
            mm.To.Add(new MailAddress(data.p_correo_cli));
            data.P_tipo_correo = "CLIENTE";
            mm.Body = prepareBodyReversion(data);
            mm.Subject = subject;
            mm.IsBodyHtml = true;
            AlternateView av = AlternateView.CreateAlternateViewFromString(mm.Body, System.Text.Encoding.UTF8, MediaTypeNames.Text.Html);
            mm.AlternateViews.Add(av);

            try
            {
                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential("onlineinterface@protectasecurity.pe", "Ay<3%762AA");
                    smtp.EnableSsl = true;
                    smtp.Send(mm);
                }

                respuesta = "Todo Salio Correctamente";

            }
            catch (Exception ex)
            {
                throw ex;
                respuesta = "Ocurrio un error al enviar el Correo" + ex;
            }


            return respuesta;
        }
        public string EnviarCorreoReversionCobranzas(CorreoEnviarReversionVM data)
        {
            string respuesta;
            string subject = "Devolución Protecta Security";
            var mm = new MailMessage();
            MailAddress from = new MailAddress("operaciones_sctr@protectasecurity.pe", "PROTECTA SECURITY");
            mm.From = from;
            mm.To.Add(new MailAddress("onlineinterface@protectasecurity.pe"));
            data.P_tipo_correo = "COBRANZAS";
            mm.Body = prepareBodyReversion(data);
            mm.Subject = subject;
            mm.IsBodyHtml = true;
            AlternateView av = AlternateView.CreateAlternateViewFromString(mm.Body, System.Text.Encoding.UTF8, MediaTypeNames.Text.Html);
            mm.AlternateViews.Add(av);

            try
            {
                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential("onlineinterface@protectasecurity.pe", "Ay<3%762AA");
                    smtp.EnableSsl = true;
                    smtp.Send(mm);
                }

                respuesta = "Todo Salio Correctamente";

            }
            catch (Exception ex)
            {
                throw ex;
                respuesta = "Ocurrio un error al enviar el Correo" + ex;
            }


            return respuesta;
        }
        private string prepareBodyReversion(CorreoEnviarReversionVM data)
        {
            string body = "";
            body += $"<center style='width: 100%; table-layout: fixed; background-color: #ffffff; padding-bottom: 40px;font-family: Verdana;'>";
            body += $"<div style='max-width: 600px; background-color: #ffffff; margin: 3px;'><table style='margin: 0 auto; width: 100%; max-width: 600px;'><tr>";
            body += $"<td style='width:75%'><span style='color:#6A2E92;text-align:left'> Protegemos lo que</span><span style ='color:#ed6e00;text-align:left'> más valoras </span></td>";
            body += $"</tr></table> <div style='background-color: #FF6E00; color : rgb(43, 13, 97) !important; height: 45px;'>";
            body += $"<h2 style='text-align: left; justify-content: left; justify-items: left; padding-left:20px; padding-top: 8px;'> Devolución Protecta Security</h2>";
            body += $"</div><div style='margin: 0 auto; width: 100%; max-width: 550px;' >";
            if (data.P_tipo_correo == "CLIENTE")
            {
                body += $"<h3 style='text-align: left; justify-content: left; justify-items: left; padding-left:5px!important; padding-right: 5px!important; font-family: Verdana; color: gray;'>Estimado Cliente : {data.c_nombrecliente} </h3>";
            }
            else
            {
                body += $"<h3 style='text-align: left; justify-content: left; justify-items: left; padding-left:5px!important; padding-right: 5px!important; font-family: Verdana; color: gray;'>Área de Cobranzas de Protecta Security </h3>";
            }
            //TODO SALIO BIEN
            if (data.P_tipo_correo == "CLIENTE")
            {
                body += $"<p style='text-align: left; justify-content: left; justify-items: left; padding-left: 20px; font-family: Verdana; color: gray;'> Te informamos que : {data.p_sdetalle_correo_rcliente}</p>  ";
            }
            else
            {
                body += $"<p style='text-align: left; justify-content: left; justify-items: left; padding-left: 20px; font-family: Verdana; color: gray;'> Te informamos que el Cliente {data.c_nombrecliente} : {data.p_sdetalle_correo_rcobranza}</p>  ";
            }
            body += $"<p style='text-align: left; justify-content: left; justify-items: left; padding-left: 20px; font-family: Verdana; color: gray;'> Muchas Gracias.</p>";
            body += $"<h3 style='text-align: left; justify-content: left; justify-items: left; padding-left: 5px!important; padding-right: 5px!important; font-family: Verdana; color: gray;'>Equipo de Tesoreria.</h3></div>";
            body += $"<div style='background-color: #FF6E00; color : rgb(43, 13, 97);height: 10%;'> <div style='padding-top: 10px;padding-bottom: 10px;'>";
            body += $"<section style='font-size: 14px; text-align: center; padding-left: 20px; padding-right: 20px; padding-top: 9px;'>Este es un email automático, si tienes cualquier tipo de duda ponte en contacto con nosotros a través de nuestro servicio de atención al cliente al e-mail <b> canalcorporativo@protectasecurity.pe </b>, por favor no respondas a este mensaje.</section>";
            body += $"</div></div><div style='height: 100px;' ><div style='padding-top: 15px; color :#FF6E00;'><span> Encuentranos en:</span><table><tr>";
            body += $"<td><a href='https://protectasecurity.pe/' target ='_blank'><img src='https://i.ibb.co/kMmSk7D/web.jpg' alt ='web' border='0'></a ></td >";
            body += $"<td><a href='https://www.youtube.com/channel/UCLDUNm7ULAC4jbie_7S8beQ' target='_blank'><img src='https://i.ibb.co/Wy4p9t7/youtube.jpg' alt='youtube' border='0'></a></td>";
            body += $"<td><a href='https://www.facebook.com/ProtectaSecurity/' target='_blank'><img src='https://i.ibb.co/gmssGnb/facebook.jpg' alt='facebook' border='0'></a></td>";
            body += $"<td><a href='https://www.linkedin.com/company/359787/admin/' target='_blank'><img src='https://i.ibb.co/dKLspzX/likendin.jpg' alt='likendin' border='0'></a></td>";
            body += $"<td><a href='https://www.instagram.com/somosprotectores/' target='_blank'><img src='https://i.ibb.co/vqFNRPw/instagran.jpg' alt='instagran' border='0'></a></td>";
            body += $"</tr></table></div></div>";
            body += $"<table style='width:100%; height:10px; border-spacing:0px! important;'><tr><td style='background-color: #FF6E00; width: 50%;'></td><td style='background-color:rgb(43, 13, 97); width: 50%;'></td></tr></table></div></div></center>";
            return body;
        }

        //---------------CORREOS-----------------------//
        [Route("BusquedaFechasCorreo")]
        [HttpPost]
        public IHttpActionResult BusquedaFechasCorreo(CorreoVM data)
        {

            var response = this.NoteCreditRefundCore.BusquedaFechasCorreo(data);

            return Ok(response);

        }
        [Route("BusquedaHoraFechasCorreo")]
        [HttpPost]
        public IHttpActionResult BusquedaHoraFechasCorreo(CorreoVM data)
        {

            var response = this.NoteCreditRefundCore.BusquedaHoraFechasCorreo(data);

            return Ok(response);

        }
        [Route("EnviarCorreo")]
        [HttpPost]
        public IHttpActionResult EnviarCorreo(CorreoEnviarVM data)
        {
            string Respuesta;

            if (EnviarCorreoCliente(data)=="Todo Salio Correctamente")
            {
                 this.NoteCreditRefundCore.EnviarCorreo(data);
                 Respuesta = "Se ingreso Correctamente";
            }
            else
            {
                 Respuesta = "Fallo el Proceso";
            }

            return Ok(Respuesta);

        }
        

        public string EnviarCorreoCliente(CorreoEnviarVM data)
        {
            string respuesta;
            string subject = "Devolución Protecta Security";
            var mm = new MailMessage();
            MailAddress from = new MailAddress("operaciones_sctr@protectasecurity.pe", "PROTECTA SECURITY");
            mm.From = from;
            mm.To.Add(new MailAddress(data.C_CORREO));

            mm.Body = prepareBody(data);
            mm.Subject = subject;
            mm.IsBodyHtml = true;
            AlternateView av = AlternateView.CreateAlternateViewFromString(mm.Body, System.Text.Encoding.UTF8, MediaTypeNames.Text.Html);
            mm.AlternateViews.Add(av);

            try
            {
                using (SmtpClient smtp = new SmtpClient("smtp-relay.gmail.com", 25))
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential("onlineinterface@protectasecurity.pe", "Ay<3%762AA");
                    smtp.EnableSsl = true;
                    smtp.Send(mm);
                }

                respuesta = "Todo Salio Correctamente";
               
            }
            catch (Exception ex)
            {
                throw ex;
                respuesta = "Ocurrio un error al enviar el Correo"+ex;
            }


            return respuesta;
        }

        private string prepareBody(CorreoEnviarVM data)
        {
      
            string body = "";
            body += $"<center style='width: 100%; table-layout: fixed; background-color: #ffffff; padding-bottom: 40px;font-family: Verdana;'>";
            body += $"<div style='max-width: 600px; background-color: #ffffff; margin: 3px;'><table style='margin: 0 auto; width: 100%; max-width: 600px;'><tr>";
            body += $"<td style='width:75%'><span style='color:#6A2E92;text-align:left'> Protegemos lo que</span><span style ='color:#ed6e00;text-align:left'> más valoras </span></td>";
            body += $"<td style='width:25%'><img src='https://plataformadigital.protectasecurity.pe/ecommerce/assets/logos/logo-PS.svg' width='145.25px' height ='32.75px' alt='logo' border='0' ></td>";
            body += $"</tr></table> <div style='background-color: #FF6E00; color : rgb(43, 13, 97) !important; height: 45px;'>";
            body += $"<h2 style='text-align: left; justify-content: left; justify-items: left; padding-left:20px; padding-top: 8px;'> Devolución Protecta Security</h2>";
            body += $"</div><div style='margin: 0 auto; width: 100%; max-width: 550px;' >";
            body += $"<h3 style='text-align: left; justify-content: left; justify-items: left; padding-left:5px!important; padding-right: 5px!important; font-family: Verdana; color: gray;'>Estimado Cliente : {data.C_NOMBRECLIENTE} </h3>";
            if (data.P_STRANSFERENCIA=="1")
            {
                //TODO SALIO BIEN
                body += $"<p style='text-align: left; justify-content: left; justify-items: left; padding-left: 20px; font-family: Verdana; color: gray;'> Te informamos que hemos procedido con exito el envio de la devolución, a su N° de cuenta : {data.C_NUMEROCUENTA}</p>  ";
            }
            else
            {   //OCURRIO ALGUN ERROR
                body += $"<p style='text-align: left; justify-content: left; justify-items: left; padding-left: 20px; font-family: Verdana; color: gray;'> Te informamos que se rechazo la devolución a su N° de cuenta : {data.C_NUMEROCUENTA},  debido a la(s) siguiente(s) observacion(es) : </p> ";
                body += $"<p style='text-align: left; justify-content: left; justify-items: left; padding-left: 20px; font-family: Verdana; color: gray;'>{data.P_SDETALLE_CORREO}</p> ";
            }
            body += $"<p style='text-align: left; justify-content: left; justify-items: left; padding-left: 20px; font-family: Verdana; color: gray;'> Muchas Gracias.</p>";
            body += $"<h3 style='text-align: left; justify-content: left; justify-items: left; padding-left: 5px!important; padding-right: 5px!important; font-family: Verdana; color: gray;'>Equipo de Tesoreria.</h3></div>";
            body += $"<div style='background-color: #FF6E00; color : rgb(43, 13, 97);height: 10%;'> <div style='padding-top: 10px;padding-bottom: 10px;'>";
            body += $"<section style='font-size: 14px; text-align: center; padding-left: 20px; padding-right: 20px; padding-top: 9px;'>Este es un email automático, si tienes cualquier tipo de duda ponte en contacto con nosotros a través de nuestro servicio de atención al cliente al e-mail <b> canalcorporativo@protectasecurity.pe </b>, por favor no respondas a este mensaje.</section>";
            body += $"</div></div><div style='height: 100px;' ><div style='padding-top: 15px; color :#FF6E00;'><span> Encuentranos en:</span><table><tr>";
            body += $"<td><a href='https://protectasecurity.pe/' target ='_blank'><img src='https://i.ibb.co/kMmSk7D/web.jpg' alt ='web' border='0'></a ></td >";
            body += $"<td><a href='https://www.youtube.com/channel/UCLDUNm7ULAC4jbie_7S8beQ' target='_blank'><img src='https://i.ibb.co/Wy4p9t7/youtube.jpg' alt='youtube' border='0'></a></td>";
            body += $"<td><a href='https://www.facebook.com/ProtectaSecurity/' target='_blank'><img src='https://i.ibb.co/gmssGnb/facebook.jpg' alt='facebook' border='0'></a></td>";
            body += $"<td><a href='https://www.linkedin.com/company/359787/admin/' target='_blank'><img src='https://i.ibb.co/dKLspzX/likendin.jpg' alt='likendin' border='0'></a></td>";
            body += $"<td><a href='https://www.instagram.com/somosprotectores/' target='_blank'><img src='https://i.ibb.co/vqFNRPw/instagran.jpg' alt='instagran' border='0'></a></td>";
            body += $"</tr></table></div></div>";
            body += $"<table style='width:100%; height:10px; border-spacing:0px! important;'><tr><td style='background-color: #FF6E00; width: 50%;'></td><td style='background-color:rgb(43, 13, 97); width: 50%;'></td></tr></table></div></div></center>";


            return body;
        }



        #endregion
    }
}