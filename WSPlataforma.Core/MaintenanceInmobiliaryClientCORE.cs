using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.DA;
using WSPlataforma.Entities.MaintenanceInmoClientModel.ViewModel;
using WSPlataforma.Entities.MaintenanceInmoClientModel.BindingModel;




namespace WSPlataforma.Core
{
    public class MaintenanceInmobiliaryClientCORE
    {
        MaintenanceInmobiliaryClientDA maintenanceInmoDA = new MaintenanceInmobiliaryClientDA();

        public List<DocumentBM> GetTypeDocumentList()
        {
            return maintenanceInmoDA.GetTypeDocumentList();
        }

        public List<OptionBM> GetTypeOptionList()
        {
            return maintenanceInmoDA.GetTypeOptionList();
        }

        public List<OptionBM> GetListBuscarPor()
        {
            return maintenanceInmoDA.GetListBuscarPor();
        }

        public List<OptionBM> GetListTipFacturacion(ContratantesVM data)
        {
            return maintenanceInmoDA.GetListTipFacturacion(data);
        }

        public List<ContratantesBM> GetContratantesList(ContratantesVM data)
        {
            return maintenanceInmoDA.GetContratantesList(data);
        }

        public List<ConsultaClienteBM> GetConsultaClientesList(ConsultaClienteVM data)
        {
            return maintenanceInmoDA.GetConsultaClientesList(data);
        }

        // CIERRE
        public Task<DocumentsCierreVM> ListarDocumentosCierre()
        {
            return maintenanceInmoDA.ListarDocumentosCierre();
        }
        public Task<PorVencerCierreVM> ListarPorVencerCierre()
        {
            return maintenanceInmoDA.ListarPorVencerCierre();
        }
        public Task<EstadosBillsCierreVM> ListarEstadosBillsCierre()
        {
            return maintenanceInmoDA.ListarEstadosBillsCierre();
        }
        public Task<InmobiliairyCierreVM> GetInmobiliairyCierreReport(InmobiliairyCierreBM data)
        {
            return maintenanceInmoDA.GetInmobiliairyCierreReport(data);
        }
        public DataSet GetDataReportInmobiliariasControlSeguimiento(InmobiliairyCierreBM response)
        {
            return maintenanceInmoDA.GetDataReportInmobiliariasControlSeguimiento(response);
        }
        public Task<TipoServicioCierreVM> ListarTipoServicioCierre()
        {
            return maintenanceInmoDA.ListarTipoServicioCierre();
        }
        public Task<InmobiliairyReportCierreVM> GetInmobiliairyReporteCierre(InmobiliairyReportCierreBM data)
        {
            return maintenanceInmoDA.GetInmobiliairyReporteCierre(data);
        }
        public DataSet GetDataInmobiliairyReporteCierre(InmobiliairyReportCierreBM response)
        {
            return maintenanceInmoDA.GetDataInmobiliairyReporteCierre(response);
        }

        // DGC

        // GÉNERO
        public Task<GeneroVM> ListarGeneroInmobiliaria()
        {
            return maintenanceInmoDA.ListarGeneroInmobiliaria();
        }

        // NACIONALIDAD
        public Task<NacionalidadVM> ListarNacionalidadInmobiliaria()
        {
            return maintenanceInmoDA.ListarNacionalidadInmobiliaria();
        }

        // ESTADO CIVIL
        public Task<EstadoCivilVM> ListarEstadoCivilInmobiliaria()
        {
            return maintenanceInmoDA.ListarEstadoCivilInmobiliaria();
        }

        // TIPO DE DOCUMENTO
        public Task<TipoDocumentoVM> ListarTipoDocumentoInmobiliaria()
        {
            return maintenanceInmoDA.ListarTipoDocumentoInmobiliaria();
        }

        // DETALLE CLIENTE
        public Task<GetClientDetVM> GetClientDetInmobiliaria(GetClientDetBM data)
        {
            return maintenanceInmoDA.GetClientDetInmobiliaria(data);
        }

        // TIPO VIA
        public Task<List<TipoViaVM>> GetTipoVias()
        {
            return maintenanceInmoDA.GetTipoVias();
        }
        
        // DEPARTAMENTOS
        public Task<List<DepartamentoVM>> GetDepartamentos()
        {
            return maintenanceInmoDA.GetDepartamentos();
        }
        
        // PROVINCIAS
        public Task<List<ProvinciaVM>> GetProvincias(int P_NPROVINCE)
        {
            return maintenanceInmoDA.GetProvincias(P_NPROVINCE);
        }

        // DISTRITOS
        public Task<List<DistritoVM>> GetDistrito(int P_NLOCAL)
        {
            return maintenanceInmoDA.GetDistrito(P_NLOCAL);
        }

        //VALIDACION FACT
        public Task<ExistBillsVM> GetExistsBills(int P_NBRANCH, int P_NPRODUCT, int P_NPOLICY)
        {
            return maintenanceInmoDA.GetExistsBills(P_NBRANCH, P_NPRODUCT, P_NPOLICY);
        }

        //DETALLE DIRECCION
        public Task<AddressClient> GetDetalleDireccion(string P_SCLIENT)
        {
            return maintenanceInmoDA.GetDetalleDireccion(P_SCLIENT);
        }

        // INSERTAR CLIENTE
        public Task<InsClientVM> InsertarClienteInmobiliaria(InsClientBM data)
        {
            return maintenanceInmoDA.InsertarClienteInmobiliaria(data);
        }

        // ACTUALIZAR CLIENTE
        public Task<UpdClientVM> ActualizarClienteInmobiliaria(UpdClientBM data)
        {
            return maintenanceInmoDA.ActualizarClienteInmobiliaria(data);
        }

        // OBTENER PAGOS
        public Task<PayDateVM> PayDateInmob(PayDateBM data)
        {
            return maintenanceInmoDA.PayDateInmob(data);
        }
    }
}