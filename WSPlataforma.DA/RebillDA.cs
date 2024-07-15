/*************************************************************************************************/
/*  NOMBRE              :   Refacturacaión.CS                                            */
/*  DESCRIPCION         :   Capa CORE - Reporte Nota de Credito                                  */
/*  AUTOR               :   MATERIAGRIS - FRANCISCO AQUIÑO                                  */
/*  FECHA               :   08-11-2023                                                           */
/*  VERSION             :   1.0                                                                  */
/*************************************************************************************************/

using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.Entities.ReporteNotaCreditoModel.ViewModel;
using WSPlataforma.Entities.ReporteNotaCreditoModel.BindingModel;
using WSPlataforma.Entities.RebillModel;
using WSPlataforma.Util;
using System.Net.Mail;
using WSPlataforma.Entities.RebillModel.ViewModel;
using System.Net;
using System.Net.Mime;

namespace WSPlataforma.DA
{
    public class RebillDA: ConnectionBase
    {
        private readonly string Package = "PKG_PV_REBILL";


        public List<BranchVM> ListarRamo()
        {

           // EnviarCorreoRefact("F001-1542",2685);

            var parameters = new List<OracleParameter>();
            var listaRamo = new List<BranchVM>();
            var procedure = string.Format("{0}.{1}", this.Package, "SPS_GET_BRANCHES");
            var branch = new BranchVM();

            try
            {
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                if (reader.HasRows)
                {

                    while (reader.Read())
                    {
                        branch = new BranchVM();
                        branch.nBranch = reader["NBRANCH"] == DBNull.Value ? 0 : int.Parse(reader["NBRANCH"].ToString());
                        branch.sDescript = reader["SBRANCH_DESCRIPT"] == DBNull.Value ? string.Empty : reader["SBRANCH_DESCRIPT"].ToString();

                        listaRamo.Add(branch);
                    }
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarRamo", ex.ToString(), "3");
                throw;
            }

            return listaRamo;
        }

        public List<PerfilVM> ListarPerfil()
        {
            var parameters = new List<OracleParameter>();
            var listaPerfil = new List<PerfilVM>();
            var procedure = string.Format("{0}.{1}", this.Package, "SPS_GET_PERFILES");
            var branch = new PerfilVM();

            try
            {
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                if (reader.HasRows)
                {

                    while (reader.Read())
                    {
                        branch = new PerfilVM();
                        branch.nPerfil = reader["NPERFIL"] == DBNull.Value ? 0 : int.Parse(reader["NPERFIL"].ToString());
                        branch.sDescript = reader["SDESCRIPT"] == DBNull.Value ? string.Empty : reader["SDESCRIPT"].ToString();

                        listaPerfil.Add(branch);
                    }
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarRamo", ex.ToString(), "3");
                throw;
            }

            return listaPerfil;
        }


        public List<ProductVM> ListarProducto(ProductoFilter data)
        {
            var sPackageName = string.Format("{0}.{1}", this.Package, "LIST_PRODUCTO");
            ProductVM entities = null;
            List<ProductVM> ListProductoPay = new List<ProductVM>();
            List<OracleParameter> parameters = new List<OracleParameter>();
            long _idRamo = data.nBranch;

            parameters.Add(new OracleParameter("RAMO", OracleDbType.Int16, _idRamo, ParameterDirection.Input));
            parameters.Add(new OracleParameter("C_LISTA", OracleDbType.RefCursor, ParameterDirection.Output));

            using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameters))
            {
                while (dr.Read())
                {
                    entities = new ProductVM();
                    entities.nProduct = Convert.ToInt32(dr["NPRODUCT"]);
                    entities.sDescript = Convert.ToString(dr["SDESCRIPT"]);
                    ListProductoPay.Add(entities);
                }
            }
            return ListProductoPay;
        }

        public int EnviarCorreoRefact(string comprobante, int NUSERCODE)
        {
            int respuesta;
            var sPackageName = string.Format("{0}.{1}", this.Package, "CONFCORREO");
            EmailVM entities = null;
            List<OracleParameter> parameters = new List<OracleParameter>();
            List<OracleParameter> parameters2 = new List<OracleParameter>();
            OracleDataReader dr = null;
            string fron = "";
            string clave = "";
            string host = "";
            string port = "";
            string to = "";
            int toc = 0;
            string cc = "";
            int ccc = 0;
            string subject = "Refacturación Protecta Security";
            var mm = new MailMessage();

            //obtiene las configuraciones para el envio de correo
            try
            {
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                P_NCODE.Size = 2;
                P_SMESSAGE.Size = 40000;
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);

                parameters.Add(new OracleParameter("C_LISTA", OracleDbType.RefCursor, ParameterDirection.Output));

                using (dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameters))

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            entities = new EmailVM();
                            entities.CABMAIL = Convert.ToString(dr["CABMAIL"]);
                            entities.DETMAIL = Convert.ToString(dr["DETMAIL"]);

                            switch (entities.CABMAIL)
                            {
                                case "FROM":
                                    fron += entities.DETMAIL;
                                    break;
                                case "CLAVE":
                                    clave = entities.DETMAIL;
                                    break;
                                case "HOST":
                                    host = entities.DETMAIL;
                                    break;
                                case "PORT":
                                    port = entities.DETMAIL;
                                    break;
                            }

                        }
                    }
                dr.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ConsultarTipoAsiento", ex.ToString(), "3");
                throw;
            }

            //obtiene los destintarios para el envio de correo
            var sPackageName2 = string.Format("{0}.{1}", this.Package, "DESTCORREO");
            try
            {

                parameters2.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int16, NUSERCODE, ParameterDirection.Input));
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                P_NCODE.Size = 2;
                P_SMESSAGE.Size = 40000;
                parameters2.Add(P_NCODE);
                parameters2.Add(P_SMESSAGE);

                parameters2.Add(new OracleParameter("C_LISTA", OracleDbType.RefCursor, ParameterDirection.Output));

                using (dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName2, parameters2))

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            entities = new EmailVM();
                            entities.CABMAIL = Convert.ToString(dr["CABMAIL"]);
                            entities.DETMAIL = Convert.ToString(dr["DETMAIL"]);

                            switch (entities.CABMAIL)
                            {
                                case "PARA":
                                    mm.To.Add(new MailAddress(entities.DETMAIL));
                                    break;
                                case "CC":
                                    mm.CC.Add(new MailAddress(entities.DETMAIL));
                                    break;
                            }

                        }
                    }
                dr.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ConsultarTipoAsiento", ex.ToString(), "3");
                throw;
            }

            MailAddress from = new MailAddress(fron, "PROTECTA SECURITY");
            // MailAddress from = new MailAddress("operaciones_sctr@protectasecurity.pe", "PROTECTA SECURITY");
            mm.From = from;
            
            
            mm.Body = prepareBodyRefact(comprobante);
            mm.Subject = subject;
            mm.IsBodyHtml = true;
            AlternateView av = AlternateView.CreateAlternateViewFromString(mm.Body, System.Text.Encoding.UTF8, MediaTypeNames.Text.Html);
            mm.AlternateViews.Add(av);

            try
            {
                using (SmtpClient smtp = new SmtpClient(host, Convert.ToInt32(port)))
                //using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(fron, clave);
                    smtp.EnableSsl = true;
                    smtp.Send(mm);
                }

                respuesta = 1;

            }
            catch (Exception ex)
            {
                throw ex;
                respuesta = 0;
            }


            return respuesta;
        }
        private string prepareBodyRefact(string comprobante)
        {
            string body = "";
            var sPackageName = string.Format("{0}.{1}", this.Package, "DETCORREO");
            CorreoVM entities = null;
            List<OracleParameter> parameters = new List<OracleParameter>();
            OracleDataReader dr = null;


            try
            {
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                P_NCODE.Size = 2;
                P_SMESSAGE.Size = 40000;
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);

                parameters.Add(new OracleParameter("C_LISTA", OracleDbType.RefCursor, ParameterDirection.Output));

                using (dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameters))

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            entities = new CorreoVM();
                            entities.CABMAIL = Convert.ToString(dr["CABMAIL"]);
                            entities.DETMAIL = Convert.ToString(dr["DETMAIL"]);

                            switch (entities.CABMAIL)
                            {
                                case "CABECERA":
                                    body += entities.DETMAIL;
                                    break;
                                case "CUERPO":
                                    body += entities.DETMAIL.Replace("@COMPROBANTE", comprobante); //adjunta el nombre del comprobante
                                    break;
                                case "PIE":
                                    body += entities.DETMAIL;
                                    break;
                            }



                        }
                    }
            }
            catch (Exception ex)
            {
                LogControl.save("ConsultarTipoAsiento", ex.ToString(), "3");
                throw;
            }
            
            return body;
        }

        public ListPerPerfil permisoPerfil(NuserCodeFilter data)
        {
            ListPerPerfil entities = new ListPerPerfil();
            var parameters = new List<OracleParameter>();
            var procedure = string.Format("{0}.{1}", this.Package, "SPS_PERMISO_PERFIL");

            try
            {
                //INPUT
                // parameters.Add(new OracleParameter("P_SUSER", OracleDbType.Int32, data.nusercode, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SUSER", OracleDbType.Varchar2, data.nusercode, ParameterDirection.Input));

                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 20, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                P_NCODE.Size = 20;
                P_SMESSAGE.Size = 4000;
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);


                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters))
                {

                    entities.P_EST = Convert.ToInt32(P_NCODE.Value.ToString());
                    entities.P_MENSAGE = P_SMESSAGE.Value.ToString();

                }

                return entities;

            }
            catch (Exception ex)
            {
                LogControl.save("permisoPerfil", ex.ToString(), "3");
                throw;
            }

        }

        public List<ListPerPerfil> listPerPerfil(PerfilFilter data)
        {
            var sPackageName = string.Format("{0}.{1}", this.Package, "SPS_LISTPERPERFIL");
            ListPerPerfil.P_TABLE suggest = null;
            ListPerPerfil entities = new ListPerPerfil();
            List<ListPerPerfil> listPerPerfil = new List<ListPerPerfil>();
            entities.lista = new List<ListPerPerfil.P_TABLE>();
            List<OracleParameter> parameters = new List<OracleParameter>();
            long _idRamo = data.nBranch;

            parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int16, data.nBranch, ParameterDirection.Input));
            parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int16, data.nProduct, ParameterDirection.Input));
            parameters.Add(new OracleParameter("P_NPERFIL", OracleDbType.Int16, data.nPerfil, ParameterDirection.Input));

            OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 20, null, ParameterDirection.Output);
            OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
            P_NCODE.Size = 20;
            P_SMESSAGE.Size = 40000;
            parameters.Add(P_NCODE);
            parameters.Add(P_SMESSAGE);
            parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

            using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameters))
            {
                while (dr.Read())
                {
                    suggest = new ListPerPerfil.P_TABLE();
                    suggest.estado = Convert.ToString(dr["SESTADO"]);
                    suggest.fecha_modf = Convert.ToString(dr["DSTATDATE"]);
                    suggest.comentario = Convert.ToString(dr["SCOMENTARIO"]);
                    suggest.usuario = Convert.ToString(dr["sUSER"]);
                    suggest.perfil = Convert.ToString(dr["SPERFIL"]);
                    suggest.diasref = Convert.ToString(dr["NDIAS_REF"]);
                    suggest.checkmod = Convert.ToInt32(dr["NCHKPERMISO"]);

                    if (suggest.fecha_modf != "")
                    {
                        DateTime fechaComprobanteNuevo = Convert.ToDateTime(suggest.fecha_modf);
                        suggest.fecha_modf = fechaComprobanteNuevo.ToString("dd/MM/yyyy");
                    }

                    entities.lista.Add(suggest);
                }
                entities.P_EST = Convert.ToInt32(P_NCODE.Value.ToString());
                entities.P_MENSAGE = P_SMESSAGE.Value.ToString();
                listPerPerfil.Add(entities);
            }
            return listPerPerfil;
        }

        public List<ListPerPerfil> guardar(PerfilFilter data)
        {
            var sPackageName = string.Format("{0}.{1}", this.Package, "SPS_GUARDAR");
            ListPerPerfil.P_TABLE suggest = null;
            ListPerPerfil entities = new ListPerPerfil();
            List<ListPerPerfil> listPerPerfil = new List<ListPerPerfil>();
            entities.lista = new List<ListPerPerfil.P_TABLE>();
            List<OracleParameter> parameters = new List<OracleParameter>();
            long _idRamo = data.nBranch;

            parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int16, data.nBranch, ParameterDirection.Input));
            parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int16, data.nProduct, ParameterDirection.Input));
            parameters.Add(new OracleParameter("P_NPERFIL", OracleDbType.Int16, data.nPerfil, ParameterDirection.Input));
            parameters.Add(new OracleParameter("P_NDIAS_REF", OracleDbType.Int16, data.nDias_Ref, ParameterDirection.Input));
            parameters.Add(new OracleParameter("P_NCHKPERMISO", OracleDbType.Int16, data.nChkPermiso, ParameterDirection.Input));
            parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int16, data.nusercode, ParameterDirection.Input));
            parameters.Add(new OracleParameter("P_SCOMENTARIO", OracleDbType.NVarchar2, data.sComentario, ParameterDirection.Input));

            OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 20, null, ParameterDirection.Output);
            OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
            P_NCODE.Size = 20;
            P_SMESSAGE.Size = 40000;
            parameters.Add(P_NCODE);
            parameters.Add(P_SMESSAGE);
            parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

            using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameters))
            {
                while (dr.Read())
                {
                    suggest = new ListPerPerfil.P_TABLE();
                    suggest.estado = Convert.ToString(dr["SESTADO"]);
                    suggest.fecha_modf = Convert.ToString(dr["DSTATDATE"]);
                    suggest.comentario = Convert.ToString(dr["SCOMENTARIO"]);
                    suggest.usuario = Convert.ToString(dr["sUSER"]);
                    suggest.perfil = Convert.ToString(dr["SPERFIL"]);
                    suggest.diasref = Convert.ToString(dr["NDIAS_REF"]);
                    suggest.checkmod = Convert.ToInt32(dr["NCHKPERMISO"]);

                    if (suggest.fecha_modf != "")
                    {
                        DateTime fechaComprobanteNuevo = Convert.ToDateTime(suggest.fecha_modf);
                        suggest.fecha_modf = fechaComprobanteNuevo.ToString("dd/MM/yyyy");
                    }

                    entities.lista.Add(suggest);
                }
                entities.P_EST = Convert.ToInt32(P_NCODE.Value.ToString());
                entities.P_MENSAGE = P_SMESSAGE.Value.ToString();
                listPerPerfil.Add(entities);
            }
            return listPerPerfil;
        }
    }
   
}
