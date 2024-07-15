using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.DA;
using WSPlataforma.Entities.ScheduleModel.BindingModel;
using WSPlataforma.Entities.ScheduleModel.ViewModel;

namespace WSPlataforma.Core
{
    public class ScheduleCORE
    {
        ScheduleDA DataAccessQuery = new ScheduleDA();

        // LISTAR HORARIOS
        public Task<ScheduleVM> ListarHorarios()
        {
            return this.DataAccessQuery.ListarHorarios();
        }

        // MODIFICAR HORARIOS
        public Task<ResponseVM> ModificarHorarios(ScheduleFilter data)
        {
            return this.DataAccessQuery.ModificarHorarios(data);
        }

        // REGISTRAR HORARIOS
        public Task<ResponseVM> RegistrarHorarios(ScheduleFilter data)
        {
            return this.DataAccessQuery.RegistrarHorarios(data);
        }

        // INICIAR SERVICIO
        public string IniciarServicio(ServicioFilter data)
        {
            return this.DataAccessQuery.IniciarServicio(data);
        }

        // DETENER SERVICIO
        public string DetenerServicio(ServicioFilter data)
        {
            return this.DataAccessQuery.DetenerServicio(data);
        }

        // ESTADO SERVICIO
        public string EstadoServicio(ServicioFilter data)
        {
            return this.DataAccessQuery.EstadoServicio(data);
        }

        // LISTAR HORARIOS
        public Task<ServiceVM> ListarServicios()
        {
            return this.DataAccessQuery.ListarServicios();
        }
    }
}