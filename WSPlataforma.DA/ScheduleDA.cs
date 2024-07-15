using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Oracle.DataAccess.Client;
using WSPlataforma.Util;
using WSPlataforma.Entities.ScheduleModel.ViewModel;
using WSPlataforma.Entities.ScheduleModel.BindingModel;
using System.ServiceProcess;
using System.Threading;

namespace WSPlataforma.DA
{
    public class ScheduleDA : ConnectionBase
    {
        private readonly string Package = "PKG_OI_HORARIO";

        // LISTAR HORARIOS
        public Task<ScheduleVM> ListarHorarios()
        {
            var parameters = new List<OracleParameter>();
            ScheduleVM entities = new ScheduleVM();
            entities.lista = new List<ScheduleVM.P_TABLE>();
            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_GET_LISTHORARIO");
            try
            {
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureOnlineDB(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ScheduleVM.P_TABLE suggest = new ScheduleVM.P_TABLE();
                        suggest.NNUNTAREA = reader["NNUNTAREA"] == DBNull.Value ? 0 : int.Parse(reader["NNUNTAREA"].ToString());
                        suggest.SPROCEDI = reader["SPROCEDI"] == DBNull.Value ? string.Empty : reader["SPROCEDI"].ToString();
                        suggest.SHORAEJE = reader["SHORAEJE"] == DBNull.Value ? string.Empty : reader["SHORAEJE"].ToString();
                        suggest.NCODGRU = reader["NCODGRU"] == DBNull.Value ? 0 : int.Parse(reader["NCODGRU"].ToString());
                        suggest.NBRANCH = reader["NBRANCH"] == DBNull.Value ? 0 : int.Parse(reader["NBRANCH"].ToString());
                        suggest.SLUN = reader["SLUN"] == DBNull.Value ? string.Empty : reader["SLUN"].ToString();
                        suggest.SMAR = reader["SMAR"] == DBNull.Value ? string.Empty : reader["SMAR"].ToString();
                        suggest.SMIE = reader["SMIE"] == DBNull.Value ? string.Empty : reader["SMIE"].ToString();
                        suggest.SJUE = reader["SJUE"] == DBNull.Value ? string.Empty : reader["SJUE"].ToString();
                        suggest.SVIE = reader["SVIE"] == DBNull.Value ? string.Empty : reader["SVIE"].ToString();
                        suggest.SSAB = reader["SSAB"] == DBNull.Value ? string.Empty : reader["SSAB"].ToString();
                        suggest.SDOM = reader["SDOM"] == DBNull.Value ? string.Empty : reader["SDOM"].ToString();
                        entities.lista.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarHorarios", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<ScheduleVM>(entities);
        }

        // MODIFICAR HORARIOS
        public Task<ResponseVM> ModificarHorarios(ScheduleFilter data)
        {
            var parameters = new List<OracleParameter>();
            ResponseVM entities = new ResponseVM();
            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_POST_ACTHORARIO");
            try
            {
                parameters.Add(new OracleParameter("P_NNUNTAREA", OracleDbType.Int16, data.NNUNTAREA, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SHORAEJE", OracleDbType.Varchar2, data.SHORAEJE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCODGRU", OracleDbType.Int16, data.NCODGRU, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int16, data.NBRANCH, ParameterDirection.Input));

                parameters.Add(new OracleParameter("P_SLUN", OracleDbType.Varchar2, data.SLUN, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SMAR", OracleDbType.Varchar2, data.SMAR, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SMIE", OracleDbType.Varchar2, data.SMIE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SJUE", OracleDbType.Varchar2, data.SJUE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SVIE", OracleDbType.Varchar2, data.SVIE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SSAB", OracleDbType.Varchar2, data.SSAB, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SDOM", OracleDbType.Varchar2, data.SDOM, ParameterDirection.Input));

                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureOnlineDB(procedure, parameters);
                entities.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                entities.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                LogControl.save("ModificarHorarios", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<ResponseVM>(entities);
        }

        // REGISTRAR HORARIOS
        public Task<ResponseVM> RegistrarHorarios(ScheduleFilter data)
        {
            var parameters = new List<OracleParameter>();
            ResponseVM entities = new ResponseVM();
            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_NEW_ACTHORARIO");
            try
            {
                parameters.Add(new OracleParameter("P_NNUNTAREA", OracleDbType.Int16, data.NNUNTAREA, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SHORAEJE", OracleDbType.Varchar2, data.SHORAEJE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCODGRU", OracleDbType.Int16, data.NCODGRU, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int16, data.NBRANCH, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPROCEDI", OracleDbType.Int16, data.P_NPROCEDI, ParameterDirection.Input));

                parameters.Add(new OracleParameter("P_SLUN", OracleDbType.Varchar2, data.SLUN, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SMAR", OracleDbType.Varchar2, data.SMAR, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SMIE", OracleDbType.Varchar2, data.SMIE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SJUE", OracleDbType.Varchar2, data.SJUE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SVIE", OracleDbType.Varchar2, data.SVIE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SSAB", OracleDbType.Varchar2, data.SSAB, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SDOM", OracleDbType.Varchar2, data.SDOM, ParameterDirection.Input));

                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureOnlineDB(procedure, parameters);
                entities.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                entities.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                LogControl.save("RegistrarHorarios", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<ResponseVM>(entities);
        }

        // INICIAR SERVICIO
        public string IniciarServicio(ServicioFilter data)
        {
            ServiceController servicio = new ServiceController();
            servicio.ServiceName = data.SSERVICIO;

            if (servicio.Status == ServiceControllerStatus.Stopped)
            {
                try
                {
                    servicio.Start();
                    while (servicio.Status == ServiceControllerStatus.Stopped)
                    {
                        Thread.Sleep(1000);
                        servicio.Refresh();
                    }
                    servicio.Close();
                    return "Se inició el servicio";
                }
                catch (Exception ex)
                {
                    LogControl.save("IniciarServicio", ex.ToString(), "3");
                    throw;
                }
            }
            else
            {
                return "El servicio ya se encuentra activo";
            }
        }

        // DETENER SERVICIO
        public string DetenerServicio(ServicioFilter data)
        {
            ServiceController servicio = new ServiceController();
            servicio.ServiceName = data.SSERVICIO;

            if (servicio.Status == ServiceControllerStatus.Running)
            {
                try
                {
                    servicio.Stop();
                    servicio.WaitForStatus(ServiceControllerStatus.Stopped);
                    servicio.Close();
                    return "Se detuvo el servicio";
                }
                catch (Exception ex)
                {
                    LogControl.save("DetenerServicio", ex.ToString(), "3");
                    throw;
                }
            }
            else
            {
                return "El servicio ya se encuentra inactivo";
            }
        }

        // ESTADO SERVICIO
        public string EstadoServicio(ServicioFilter data)
        {
            ServiceController servicio = new ServiceController();
            servicio.ServiceName = data.SSERVICIO;

            if (servicio.Status == ServiceControllerStatus.Running)
            {
                return "El servicio se encuentra activo";
            }
            else
            {
                return "El servicio se encuentra inactivo";
            }
        }

        // LISTAR SERVICIOS
        public Task<ServiceVM> ListarServicios()
        {
            var parameters = new List<OracleParameter>();
            ServiceVM entities = new ServiceVM();
            entities.lista = new List<ServiceVM.P_TABLE>();
            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_GET_LISTSERVICIO");
            try
            {
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                var reader = (OracleDataReader)this.ExecuteByStoredProcedureOnlineDB(procedure, parameters);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ServiceVM.P_TABLE suggest = new ServiceVM.P_TABLE();
                        suggest.SDESCRIPT3 = reader["SDESCRIPT3"] == DBNull.Value ? string.Empty : reader["SDESCRIPT3"].ToString();
                        entities.lista.Add(suggest);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("ListarServicios", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<ServiceVM>(entities);
        }
    }
}