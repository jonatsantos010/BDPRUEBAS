using System;
using System.Collections.Generic;
using WSPlataforma.DA;
using WSPlataforma.Entities.LoadMassiveModel;
using WSPlataforma.DA.CSVgenerator;
using WSPlataforma.Entities.AccountStateModel.ViewModel;
using WSPlataforma.Entities.ViewModel;

namespace WSPlataforma.Core
{
    public class LoadMassiveCORE
    {
        LoadMassiveDA LoadMassiveDA = new LoadMassiveDA();
        public ResponseVM GenerateFact(int NIDHEADERPROC, int NIDDETAILPROC, int NUSERCODE)
        {
            return LoadMassiveDA.GenerateFact(NIDHEADERPROC, NIDDETAILPROC, NUSERCODE);
        }

        public List<ProcessHeaderBM> GetProcessHeader(double nbranch, string dateProcess, int IdProduct)
        {
            return LoadMassiveDA.GetProcessHeader(nbranch, dateProcess, IdProduct);
        }
        public List<ProcessDetailBM> GetProcessDetail(int IdProcessHeader)
        {
            List<ProcessDetailBM> LstprocessDetailBMs = LoadMassiveDA.GetProcessDetail(IdProcessHeader);
            foreach (ProcessDetailBM Detail in LstprocessDetailBMs)
            {
                Detail.PorcentAvance = LoadMassiveDA.GetAvanceDetailProcess(Convert.ToInt32(Detail.IdDetailProcess), IdProcessHeader);
            }
            return LstprocessDetailBMs;
        }
        public List<ProcessLogBM> GetProcessLogError(int IdDetailProcess, string Opcion)
        {
            return LoadMassiveDA.GetProcessLogError(IdDetailProcess, Opcion);

        }
        // public String GetAvanceDetailProcess(int IdDetailProcess)
        //{
        //    return LoadMassiveDA.GetAvanceDetailProcess(IdDetailProcess);
        //}

        public List<PathConfigBM> getPathConfig(int path)
        {
            return LoadMassiveDA.getPathConfig(path);
        }
        public GenericPackageVM GetDataExport(ConfigFileBM configFile)
        {
            GenerateCSV generate = new GenerateCSV();
            return generate.EXPORT_DATA_CSV(configFile.IdFileConfig, configFile.table_reference, configFile.IdHeaderProcess, ExportFile.Reproceso);
        }

        public GenericPackageVM GetDataExportCorrect(ConfigFileBM configFile)
        {
            GenerateCSV generate = new GenerateCSV();
            return generate.EXPORT_DATA_CSV(configFile.IdFileConfig, configFile.table_reference, configFile.IdHeaderProcess, ExportFile.Correct);
        }
        public List<PathConfigBM> GetConfigurationPath(int Identity)
        {
            return LoadMassiveDA.GetConfigurationPath(Identity);
        }
        public List<EntityConfigBM> GetConfigurationEntity()
        {
            return LoadMassiveDA.GetConfigurationEntity();
        }
        public GenericPackageVM InsertHeaderProc(ProcessHeaderBM processHeaderBM)
        {
            return LoadMassiveDA.InsertHeaderProc(processHeaderBM);
        }
        public GenericPackageVM InsertDetailProc(ProcessDetailBM processHeaderBM)
        {
            return LoadMassiveDA.InsertDetailProc(processHeaderBM);
        }
        public List<FileConfigBM> GetConfigurationFiles(int idPath)
        {
            return LoadMassiveDA.GetConfigurationFiles(idPath);
        }
        public List<ProductLoadVM> GetProductsList(int idPath)
        {
            return LoadMassiveDA.GetProductsList(idPath);
        }

    }
}
