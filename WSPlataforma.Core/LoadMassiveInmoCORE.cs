using System;
using System.Data;

using System.Collections.Generic;
using WSPlataforma.DA;
using WSPlataforma.Entities.LoadMassiveInmoModel;
using WSPlataforma.Entities.LoadMassiveModel;
using WSPlataforma.DA.CSVgenerator;
using WSPlataforma.Entities.AccountStateModel.ViewModel;
using WSPlataforma.Entities.LoadMassiveInmoModel.ViewModel;
using WSPlataforma.Entities.ViewModel;

namespace WSPlataforma.Core
{
    public class LoadMassiveInmoCORE
    {
        LoadMassiveInmoDA LoadMassiveInmoDA = new LoadMassiveInmoDA();

        public List<BranchVM> GetBranchList(string branch)
        {
            return LoadMassiveInmoDA.GetBranchList(branch);
        }

        public List<ProcessHeaderBM> GetProcessHeader(double nbranch, string dateProcess, int IdProduct)
        {
            return LoadMassiveInmoDA.GetProcessHeader(nbranch, dateProcess, IdProduct);
        }

        public List<ProcessDetailBM> GetProcessDetail(int IdProcessHeader)
        {
            List<ProcessDetailBM> LstprocessDetailBMs = LoadMassiveInmoDA.GetProcessDetail(IdProcessHeader);
            foreach (ProcessDetailBM Detail in LstprocessDetailBMs)
            {
                Detail.PorcentAvance = LoadMassiveInmoDA.GetAvanceDetailProcess(Convert.ToInt32(Detail.IdDetailProcess), IdProcessHeader);
            }
            return LstprocessDetailBMs;
        }
        public List<PathConfigBM> GetConfigurationPath(int Identity)
        {
            return LoadMassiveInmoDA.GetConfigurationPath(Identity);
        }
        public List<EntityConfigInmoBM> GetConfigurationEntity()
        {
            return LoadMassiveInmoDA.GetConfigurationEntity();
        }

        public List<FileConfigInmoBM> GetConfigurationFiles(int idPath)
        {
            return LoadMassiveInmoDA.GetConfigurationFiles(idPath);
        }
        public List<ProductLoadVM> GetProductsList(int idPath)
        {
            return LoadMassiveInmoDA.GetProductsList(idPath);
        }
        public TramaInmoVMResponse SaveUsingOracleBulkCopy(DataTable data, int codeTRX, string TablaTrama, int P_NIDHEADERPROC, int P_NIDDETAILPROC, int codUser)
        {
            return this.LoadMassiveInmoDA.SaveUsingOracleBulkCopy(data, codeTRX, TablaTrama, P_NIDHEADERPROC, P_NIDDETAILPROC, codUser);
        }
        public GenericPackageVM InsertHeaderProc(ProcessHeaderBM processHeaderBM)
        {
            return LoadMassiveInmoDA.InsertHeaderProc(processHeaderBM);
        }
        public GenericPackageVM InsertDetailProc(ProcessDetailBM processHeaderBM)
        {
            return LoadMassiveInmoDA.InsertDetailProc(processHeaderBM);
        }
        public List<PathConfigBM> getPathConfig(int path)
        {
            return LoadMassiveInmoDA.getPathConfig(path);
        }
        public GenericPackageVM GetDataExport(ConfigFileBM configFile)
        {
            GenerateCSV generate = new GenerateCSV();
            return generate.EXPORT_DATA_XLS(configFile.IdFileConfig, configFile.table_reference, configFile.IdHeaderProcess, ExportFile.Reproceso);
        }
        public GenericPackageVM GetDataExportCorrect(ConfigFileBM configFile)
        {
            GenerateCSV generate = new GenerateCSV();
            return generate.EXPORT_DATA_XLS(configFile.IdFileConfig, configFile.table_reference, configFile.IdHeaderProcess, ExportFile.Correct);
        }
        public List<ConfigFields> GetTramaFields(int P_NIDFILEHEADER, int P_NREQUIRED)
        {
            return LoadMassiveInmoDA.GetTramaFields(P_NIDFILEHEADER, P_NREQUIRED);
        }
        public List<ProcessLogBM> GetProcessLogError(int IdDetailProcess, string Opcion)
        {
            return LoadMassiveInmoDA.GetProcessLogError(IdDetailProcess, Opcion);

        }
    }
}
