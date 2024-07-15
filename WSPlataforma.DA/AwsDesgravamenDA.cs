using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.Util;
using WSPlataforma.Entities.AwsDesgravamenModel.ViewModel;

namespace WSPlataforma.DA
{
    public class AwsDesgravamenDA : ConnectionBase
    {
        public List<HorariosVM> getHorasList()
        {
            var parameters = new List<OracleParameter>();
            var suggests = new List<HorariosVM>();
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_HORARIO_MIGRACION_NUBE, "LIST_HORAS");

            DateTime fechaInicio;
            DateTime fechaFin;

            try
            {
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var suggest = new HorariosVM();
                        suggest.NBRANCH = reader["NBRANCH"] == DBNull.Value ? 0 : Int32.Parse(reader["NBRANCH"].ToString());
                        suggest.SBRANCH = reader["SBRANCH"] == DBNull.Value ? "" : reader["SBRANCH"].ToString();
                        suggest.NPRODUCT = reader["NPRODUCT"] == DBNull.Value ? 0 : Int32.Parse(reader["NPRODUCT"].ToString());
                        suggest.SPRODUCT = reader["SPRODUCT"] == DBNull.Value ? "" : reader["SPRODUCT"].ToString();
                        suggest.NDIA = reader["NDIA"] == DBNull.Value ? 0 : Int32.Parse(reader["NDIA"].ToString());
                        suggest.SDIA = reader["SDIA"] == DBNull.Value ? "" : reader["SDIA"].ToString();
                        suggest.DHORA_INICIO = reader["DHORA_INICIO"] == DBNull.Value ? Convert.ToDateTime("") : Convert.ToDateTime(reader["DHORA_INICIO"]);
                        suggest.DHORA_FIN = reader["DHORA_FIN"] == DBNull.Value ? Convert.ToDateTime("") : Convert.ToDateTime(reader["DHORA_FIN"]);
                        suggest.HoraSiguienteDia = reader["NFLAG_DIA_SIG"] == DBNull.Value ? 0 : Int32.Parse(reader["NFLAG_DIA_SIG"].ToString());

                        suggest.horaIniList = suggest.DHORA_INICIO.ToString("HH:mm:ss").Substring(0,2);
                        suggest.minutoIniList = suggest.DHORA_INICIO.ToString("HH:mm:ss").Substring(3, 2);

                        suggest.horaFinList = suggest.DHORA_FIN.ToString("HH:mm:ss").Substring(0, 2);
                        suggest.minutoFinList = suggest.DHORA_FIN.ToString("HH:mm:ss").Substring(3, 2);
                        suggests.Add(suggest);
                    }
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("GetBranch", ex.ToString(), "3");
                throw;
            }

            return suggests;
        }
        public List<SimpleModelVM> GetBranch()
        {
            var parameters = new List<OracleParameter>();
            var suggests = new List<SimpleModelVM>();
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_HORARIO_MIGRACION_NUBE, "LIST_BRANCH");

            try
            {
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {//NBRANCH, SDESCRIPT
                        var suggest = new SimpleModelVM();
                        suggest.Id = reader["NBRANCH"] == DBNull.Value ? 0 : int.Parse(reader["NBRANCH"].ToString());
                        suggest.Description = reader["SDESCRIPT"] == DBNull.Value ? string.Empty : reader["SDESCRIPT"].ToString();

                        suggests.Add(suggest);
                    }
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("GetBranch", ex.ToString(), "3");
                throw;
            }

            return suggests;
        }

        public List<SimpleModelVM> GetDiaList()
        {
            var parameters = new List<OracleParameter>();
            var suggests = new List<SimpleModelVM>();
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_HORARIO_MIGRACION_NUBE, "LIST_DIA");

            try
            {
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {//NBRANCH, SDESCRIPT
                        var suggest = new SimpleModelVM();
                        suggest.Id = reader["NDIA"] == DBNull.Value ? 0 : int.Parse(reader["NDIA"].ToString());
                        suggest.Description = reader["SDESCRIPT"] == DBNull.Value ? string.Empty : reader["SDESCRIPT"].ToString();

                        suggests.Add(suggest);
                    }
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("GetBranch", ex.ToString(), "3");
                throw;
            }

            return suggests;
        }

        public List<SimpleModelVM> GetHoraList()
        {
            var parameters = new List<OracleParameter>();
            var suggests = new List<SimpleModelVM>();
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_HORARIO_MIGRACION_NUBE, "LIST_HORA");

            try
            {
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {//NBRANCH, SDESCRIPT
                        var suggest = new SimpleModelVM();
                        suggest.Id = reader["NHORA"] == DBNull.Value ? 0 : int.Parse(reader["NHORA"].ToString());
                        suggest.Description = reader["SDESCRIPT"] == DBNull.Value ? string.Empty : reader["SDESCRIPT"].ToString();

                        suggests.Add(suggest);
                    }
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("GetBranch", ex.ToString(), "3");
                throw;
            }

            return suggests;
        }

        public List<SimpleModelVM> getMinutoList()
        {
            var parameters = new List<OracleParameter>();
            var suggests = new List<SimpleModelVM>();
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_HORARIO_MIGRACION_NUBE, "LIST_MINUTOS");

            try
            {
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {//NBRANCH, SDESCRIPT
                        var suggest = new SimpleModelVM();
                        suggest.Id = reader["NMINUTO"] == DBNull.Value ? 0 : int.Parse(reader["NMINUTO"].ToString());
                        suggest.Description = reader["SDESCRIPT"] == DBNull.Value ? string.Empty : reader["SDESCRIPT"].ToString();

                        suggests.Add(suggest);
                    }
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("GetBranch", ex.ToString(), "3");
                throw;
            }

            return suggests;
        }

        public List<SimpleModelVM> GetProductsById(int nBranch)
        {
            var parameters = new List<OracleParameter>();
            var suggests = new List<SimpleModelVM>();
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_HORARIO_MIGRACION_NUBE, "LIST_PRODUCT");

            try
            {
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int64, nBranch, ParameterDirection.Input));
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {//NBRANCH, SDESCRIPT
                        var suggest = new SimpleModelVM();
                        suggest.Id = reader["NPRODUCT"] == DBNull.Value ? 0 : int.Parse(reader["NPRODUCT"].ToString());
                        suggest.Description = reader["SDESCRIPT"] == DBNull.Value ? string.Empty : reader["SDESCRIPT"].ToString();

                        suggests.Add(suggest);
                    }
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("GetProductsById", ex.ToString(), "3");
                throw;
            }

            return suggests;
        }

        public SimpleModelVM nuevoHorario(HorariosVM data)
        {
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_HORARIO_MIGRACION_NUBE, "SP_INSERT_HORARIO");
            var rtrn = new SimpleModelVM();

            DateTime fechaBase = new DateTime(1, 1, 1);
            DateTime fechaHoraInicio = fechaBase.Add(TimeSpan.Parse(string.Concat(data.horaIniList, ":", data.minutoIniList)));
            DateTime fechaHoraFin = fechaBase.Add(TimeSpan.Parse(string.Concat(data.horaFinList, ":", data.minutoFinList)));

            try
            {

                var parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.NBRANCH, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, data.NPRODUCT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NDIA", OracleDbType.Int64, data.NDIA, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DHORA_INICIO", OracleDbType.Date, fechaHoraInicio, ParameterDirection.Input));

                parameters.Add(new OracleParameter("P_DHORA_FIN", OracleDbType.Date, fechaHoraFin, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NFLAG_DIA_SIG", OracleDbType.Int32, data.HoraSiguienteDia, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int64, data.NUSERCODE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCOMMIT", OracleDbType.Int32, 1, ParameterDirection.Input));


                var SMessage = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                var NCode = new OracleParameter("P_NCODIGO", OracleDbType.Int32, ParameterDirection.Output);

                parameters.Add(SMessage);
                parameters.Add(NCode);
                SMessage.Size = 4000;
                NCode.Size = 400;


                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                rtrn.Id = int.Parse(NCode.Value.ToString());
                rtrn.Description = SMessage.Value.ToString();
                return rtrn;


            }
            catch (Exception ex)
            {
                LogControl.save("SetListaPlanes", ex.ToString(), "3");
                //throw;
                rtrn.Id = 1;
                rtrn.Description = "error: "+ex;
                return rtrn;
            }
           
        }

        public SimpleModelVM actualizarHorario(HorariosVM data)
        {
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_HORARIO_MIGRACION_NUBE, "SP_UPDATE_HORARIO");
            var rtrn = new SimpleModelVM();

            DateTime fechaBase = new DateTime(1, 1, 1);
            DateTime fechaHoraInicio = fechaBase.Add(TimeSpan.Parse(string.Concat(data.horaIniList, ":", data.minutoIniList)));
            DateTime fechaHoraFin = fechaBase.Add(TimeSpan.Parse(string.Concat(data.horaFinList, ":", data.minutoFinList)));

            try
            {

                var parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, data.NBRANCH, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, data.NPRODUCT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NDIA", OracleDbType.Int64, data.NDIA, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DHORA_INICIO", OracleDbType.Date, fechaHoraInicio, ParameterDirection.Input));

                parameters.Add(new OracleParameter("P_DHORA_FIN", OracleDbType.Date, fechaHoraFin, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NFLAG_DIA_SIG", OracleDbType.Int32, data.HoraSiguienteDia, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int64, data.NUSERCODE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCOMMIT", OracleDbType.Int32, 1, ParameterDirection.Input));


                var SMessage = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                var NCode = new OracleParameter("P_NCODIGO", OracleDbType.Int32, ParameterDirection.Output);

                parameters.Add(SMessage);
                parameters.Add(NCode);
                SMessage.Size = 4000;
                NCode.Size = 400;


                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                rtrn.Id = int.Parse(NCode.Value.ToString());
                rtrn.Description = SMessage.Value.ToString();
                return rtrn;


            }
            catch (Exception ex)
            {
                LogControl.save("SetListaPlanes", ex.ToString(), "3");
                //throw;
                rtrn.Id = 1;
                rtrn.Description = "error: " + ex;
                return rtrn;
            }

        }
        public SimpleModelVM getFacSuspendida(int NPOLICY)//INI <RQ2024-57 - 03/04/2024>
        {
            var suggests = new SimpleModelVM();
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_HORARIO_MIGRACION_NUBE, "SP_FAC_SUSPENDIDA");

            try
            {

                var parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("P_NPOLICY", OracleDbType.Int32, NPOLICY, ParameterDirection.Input));

                var SMessage = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                var NCode = new OracleParameter("P_NCODIGO", OracleDbType.Int32, ParameterDirection.Output);

                parameters.Add(SMessage);
                parameters.Add(NCode);
                SMessage.Size = 4000;
                NCode.Size = 400;


                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                suggests.Id = int.Parse(NCode.Value.ToString());
                suggests.Description = SMessage.Value.ToString();
                return suggests;


            }
            catch (Exception ex)
            {
                LogControl.save("SetListaPlanes", ex.ToString(), "3");
                //throw;
                suggests.Id = 1;
                suggests.Description = "error: " + ex;
                return suggests;
            }

        }

        public SimpleModelVM BorrarHorario(int NBRANCH, int NPRODUCT, int NDIA)
        {
            var procedure = string.Format("{0}.{1}", ProcedureName.pkg_HORARIO_MIGRACION_NUBE, "SP_DELETE_HORARIO");
            var rtrn = new SimpleModelVM();

            try
            {

                var parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, NBRANCH, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, NPRODUCT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NDIA", OracleDbType.Int32, NDIA, ParameterDirection.Input));

                var SMessage = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                var NCode = new OracleParameter("P_NCODIGO", OracleDbType.Int32, ParameterDirection.Output);

                parameters.Add(SMessage);
                parameters.Add(NCode);
                SMessage.Size = 4000;
                NCode.Size = 400;


                var reader = (OracleDataReader)this.ExecuteByStoredProcedureVT(procedure, parameters);

                rtrn.Id = int.Parse(NCode.Value.ToString());
                rtrn.Description = SMessage.Value.ToString();
                return rtrn;

            }
            catch (Exception ex)
            {
                LogControl.save("SetListaPlanes", ex.ToString(), "3");
                //throw;
                rtrn.Id = 1;
                rtrn.Description = "error: " + ex;
                return rtrn;
            }

        }
    }
}
