using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.Util;

namespace WSPlataforma.DA
{
    public class ServiceDA
    {
        public string ValidarClientes(string json)
        {
            string response = string.Empty;
            var url = ConfigurationManager.AppSettings["UrlValidarClientes"];
            byte[] data = UTF8Encoding.UTF8.GetBytes(json);

            try
            {
                HttpWebRequest request;
                request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "POST";
                request.ContentLength = data.Length;
                request.ContentType = "application/json; charset=utf-8";
                request.Proxy = new WebProxy() { UseDefaultCredentials = true };

                Stream postTorrente = request.GetRequestStream();
                postTorrente.Write(data, 0, data.Length);
                postTorrente.Close();


                HttpWebResponse respuesta = request.GetResponse() as HttpWebResponse;
                StreamReader read = new StreamReader(respuesta.GetResponseStream(), Encoding.UTF8);
                response = read.ReadToEnd();
            }
            catch (Exception ex)
            {
                //ELog.save(this, ex);
            }

            return response;
        }
    }
}
