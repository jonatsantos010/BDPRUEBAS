using System;
using System.Collections.Generic;
using WSPlataforma.DA;
using WSPlataforma.Entities.ReporteSucaveModel;
using WSPlataforma.Entities.ReporteSucaveModel.BindingModel;
using WSPlataforma.Entities.ReporteSucaveModel.ViewModel;
using WSPlataforma.DA.CSVgenerator;
using WSPlataforma.Entities.AccountStateModel.ViewModel;
using WSPlataforma.Entities.ViewModel;
using System.Threading.Tasks;

namespace WSPlataforma.Core
{
  
    public class ReporteSucaveCORE
    {
        ReporteSucaveDA ReporteSucaveDA = new ReporteSucaveDA();
      
        #region Reporte Sucave
        public List<ReporteSoatVM> GetBranch()
        {
            return this.ReporteSucaveDA.GetBranch();
        }
        public List<ReporteSoatVM> GetProduct(ProductoFilter data)
        {

            return this.ReporteSucaveDA.GetProduct(data);

        }
        public List<ReporteSoatBM> InsertReportStatus(ReporteSoatFilter responseControl)
        {
            var rpta = this.ReporteSucaveDA.InsertReportStatus(responseControl);

            var rpteSoat = this.ReporteSucaveDA.GetReportStatus(responseControl);
            return rpteSoat;

        }

        public List<ReporteSoatBM> GetReportStatus(ReporteSoatFilter data)
        {

            return this.ReporteSucaveDA.GetReportStatus(data);

        }


        #endregion

        #region Consulta Reporte Sucave
        public ResponseVM GenerateFact(int NIDHEADERPROC, int NIDDETAILPROC, int NUSERCODE)
        {
            return ReporteSucaveDA.GenerateFact(NIDHEADERPROC, NIDDETAILPROC, NUSERCODE);
        }

        public List<ProcessHeaderBM> GetProcessHeader(ReporteVisualizarFilter data)
        {
            return this.ReporteSucaveDA.GetProcessHeader(data);
        }
        public List<ProcessDetailBM> GetProcessDetail(int IdProcessHeader)
        {
            List<ProcessDetailBM> LstprocessDetailBMs = ReporteSucaveDA.GetProcessDetail(IdProcessHeader);
            foreach (ProcessDetailBM Detail in LstprocessDetailBMs)
            {
                Detail.PorcentAvance = ReporteSucaveDA.GetAvanceDetailProcess(Convert.ToInt32(Detail.IdDetailProcess), IdProcessHeader);
            }
            return LstprocessDetailBMs;
        }
        public List<ProcessLogBM> GetProcessLogError(int IdDetailProcess, string Opcion)
        {
            return ReporteSucaveDA.GetProcessLogError(IdDetailProcess, Opcion);

        }
        // public String GetAvanceDetailProcess(int IdDetailProcess)
        //{
        //    return ReporteSucaveDA.GetAvanceDetailProcess(IdDetailProcess);
        //}

        public List<PathConfigBM> getPathConfig(int path)
        {
            return ReporteSucaveDA.getPathConfig(path);
        }
        public Task<Contratante> GetDataExport(ConfigFileBM configFile)
        {
            //GenerateCSV generate = new GenerateCSV();
            //return generate.EXPORT_DATA_CSV(configFile.IdFileConfig, configFile.table_reference, configFile.IdHeaderProcess, ExportFile.Reproceso);
            //return ReporteSucaveDA.ListarDetalleInterfacesXLSX(configFile.IdFileConfig, configFile.table_reference, configFile.IdHeaderProcess, ExportFile.Reproceso);


            return new ReporteSucaveDA().GetDataExport(configFile.IdFileConfig, configFile.table_reference, configFile.IdHeaderProcess);
                    
               
        }
        public Task<Expuestos> GetDataExport2(ConfigFileBM configFile)
        {
            //  GenerateCSV generate = new GenerateCSV();
            //return generate.EXPORT_DATA_CSV(configFile.IdFileConfig, configFile.table_reference, configFile.IdHeaderProcess, ExportFile.Reproceso);
            //return ReporteSucaveDA.ListarDetalleInterfacesXLSX(configFile.IdFileConfig, configFile.table_reference, configFile.IdHeaderProcess, ExportFile.Reproceso);
             
            return new ReporteSucaveDA().GetDataExport2(configFile.IdFileConfig, configFile.table_reference, configFile.IdHeaderProcess);
             
        }
        //public Task<Expuestos> GetDataExportCorrect(ConfigFileBM configFile)
        //{
        //   // GenerateCSV generate = new GenerateCSV();
        //   // return generate.EXPORT_DATA_CSV(configFile.IdFileConfig, configFile.table_reference, configFile.IdHeaderProcess, ExportFile.Correct);
        //   return new ReporteSucaveDA().GetDataExportCorrect(configFile.IdFileConfig, configFile.table_reference, configFile.IdHeaderProcess);
        //}
        public GenericPackageVM GetDataExportCorrect(ConfigFileBM configFile)
        {
            GenerateCSV generate = new GenerateCSV();
            return generate.EXPORT_DATA_CSV(configFile.IdFileConfig, configFile.table_reference, configFile.IdHeaderProcess, ExportFile.Correct);
        }
        public List<PathConfigBM> GetConfigurationPath(int Identity)
        {
            return ReporteSucaveDA.GetConfigurationPath(Identity);
        }
        public List<EntityConfigBM> GetConfigurationEntity()
        {
            return ReporteSucaveDA.GetConfigurationEntity();
        }
        public GenericPackageVM InsertHeaderProc(ProcessHeaderBM processHeaderBM)
        {
            return ReporteSucaveDA.InsertHeaderProc(processHeaderBM);
        }
        public GenericPackageVM InsertDetailProc(ProcessDetailBM processHeaderBM)
        {
            return ReporteSucaveDA.InsertDetailProc(processHeaderBM);
        }
        public List<FileConfigBM> GetConfigurationFiles(int idPath)
        {
            return ReporteSucaveDA.GetConfigurationFiles(idPath);
        }
        public List<ProductLoadVM> GetProductsList(int idPath)
        {
            return ReporteSucaveDA.GetProductsList(idPath);
        }

        public GenericPackageVM GetDataExportTxt(ConfigFileBM configFile)
        {
            GenerateCSV generate = new GenerateCSV();
            return generate.EXPORT_DATA_TXT(configFile.IdFileConfig, configFile.table_reference, configFile.IdHeaderProcess, ExportFile.Txt);
        }

        #endregion
    }
}
