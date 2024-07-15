using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.DA;
using WSPlataforma.Entities.PolicyModel.BindingModel;

namespace WSPlataforma.Core
{
    public class ServiceCORE
    {
        ServiceDA ServiceDA = new ServiceDA();
        public String ValidarClientes(List<InsuredValBM> data)
        {
            try
            {
                string json = JsonConvert.SerializeObject(data);
                return ServiceDA.ValidarClientes(json);
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }
    }
}
