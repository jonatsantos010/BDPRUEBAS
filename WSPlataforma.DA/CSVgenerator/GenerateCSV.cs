using CsvHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.Entities.AccountStateModel.ViewModel;
using WSPlataforma.Util;
using SpreadsheetLight;

namespace WSPlataforma.DA.CSVgenerator
{
    public enum ExportFile
    {
        Correct = 0,
        Reproceso = 1,
        Txt = 2
    }

    public class GenerateCSV
    {
        public GenericPackageVM EXPORT_DATA_CSV(int IdDetailProcess, string StrTable_Reference, int IdHeaderProcess, ExportFile exportFile)
        {
            GenericPackageVM genericPackageVM = new GenericPackageVM();
            byte[] bytes = null;
            DataTable Data = new DataTable();

            try
            {
                switch (exportFile)
                {
                    case ExportFile.Reproceso:
                        Data = new LoadMassiveDA().GetDataExport(IdDetailProcess, StrTable_Reference, IdHeaderProcess);
                        break;
                    case ExportFile.Correct:
                        Data = new LoadMassiveDA().GetDataExportCorrect(IdDetailProcess, StrTable_Reference, IdHeaderProcess);
                        break;                    
                }

                if (Data.Rows.Count > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))

                        using (var csvWriter = new CsvWriter(streamWriter, new CsvHelper.Configuration.Configuration() { Delimiter = ";" }))
                        {
                            //Write Column values
                            foreach (DataColumn column in Data.Columns)
                            {
                                csvWriter.WriteField(column.ColumnName);
                            }
                            csvWriter.NextRecord();

                            // Write row values
                            foreach (DataRow row in Data.Rows)
                            {
                                for (var i = 0; i < Data.Columns.Count; i++)
                                {
                                    csvWriter.WriteField(row[i]);
                                }
                                csvWriter.NextRecord();
                            }
                        }

                        bytes = memoryStream.ToArray();

                    }
                    genericPackageVM.GenericResponse = Convert.ToBase64String(bytes);
                }
            }
            catch (Exception ex)
            {
                //LogControl.save("ExportDataCSV", ex.ToString(), "3");
            }

            return genericPackageVM;
        }

        public GenericPackageVM EXPORT_DATA_TXT(int IdDetailProcess, string StrTable_Reference, int IdHeaderProcess, ExportFile exportFile)
        {
            GenericPackageVM genericPackageVM = new GenericPackageVM();
            byte[] bytes = null;
            DataTable Data = new DataTable();

            try
            {
                Data = new ReporteSucaveDA().GetDataExportTxt(IdDetailProcess, StrTable_Reference, IdHeaderProcess);
                if(Data.Rows.Count > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        using(var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))                                                
                        //Write row values
                        foreach(DataRow row in Data.Rows)
                        {
                            for(var i = 0; i < Data.Columns.Count; i++)
                            {
                                streamWriter.WriteLine(row[i]);
                            }
                        }
                        bytes = memoryStream.ToArray();                       
                    }
                    genericPackageVM.GenericResponse = Convert.ToBase64String(bytes);
                }
            }
            catch (Exception ex)
            {
                ELog.save(this, ex);
            }
            return genericPackageVM;
        }
        // INI MMQ 11-12-2023
        public GenericPackageVM EXPORT_DATA_XLS(int IdDetailProcess, string StrTable_Reference, int IdHeaderProcess, ExportFile exportFile)
        {
            GenericPackageVM genericPackageVM = new GenericPackageVM();
            byte[] bytes = null;
            DataTable Data = new DataTable();

            var sl = new SLDocument();
            var Style = sl.CreateStyle();
            int RowInitial = 2;
            int ColInitial = 1;

            try
            {
                switch (exportFile)
                {
                    case ExportFile.Reproceso:
                        Data = new LoadMassiveInmoDA().GetDataExport(IdDetailProcess, StrTable_Reference, IdHeaderProcess);
                        break;
                    case ExportFile.Correct:
                        Data = new LoadMassiveInmoDA().GetDataExportCorrect(IdDetailProcess, StrTable_Reference, IdHeaderProcess);
                        break;
                }

                if (Data.Rows.Count > 0)
                {
                    
                    var fileTrama = new LoadMassiveInmoDA().GetConfigurationFiles(2);
                    var sheetName = fileTrama.Where(column => column.IdFileConfig == IdDetailProcess).FirstOrDefault();
                    Data.TableName = sheetName.FileName;

                    sl.AddWorksheet(Data.TableName);

                    //INSERTA TITULO
                    foreach (DataColumn Col in Data.Columns)
                    {
                        sl.SetCellValue(1, ColInitial, Col.ColumnName);
                        ColInitial++;
                    }

                    int rowTbl = 0;
                    //INSERTA CUERPO
                    foreach (DataRow row in Data.Rows)
                    {
                        for (int i = 1; i <= Data.Columns.Count; i++)
                        {
                            sl.SetCellValue(RowInitial, i, Data.Rows[rowTbl][i - 1].ToString());
                        }
                        RowInitial++;
                        rowTbl++;
                    }
                
                     sl.DeleteWorksheet(SLDocument.DefaultFirstSheetName);
                     sl.AutoFitColumn(1, ColInitial);
                     sl.SelectWorksheet(Data.TableName);

                     string fileName = Data.TableName.ToString() + "_ERROR.xlsx";
                     var pathReport = new LoadMassiveInmoDA().getPathConfig(2);

                     string rutaCarga = String.Format(@"{0}\{1}", pathReport[0].RouteDestine, fileName);

                     if (!Directory.Exists(pathReport[0].RouteDestine))
                         Directory.CreateDirectory(pathReport[0].RouteDestine);
                     sl.SaveAs(rutaCarga);

                     genericPackageVM.GenericResponse = Convert.ToBase64String(File.ReadAllBytes(rutaCarga));
                }
            }
            catch (Exception ex)
            {
                //LogControl.save("ExportDataCSV", ex.ToString(), "3");
            }

            return genericPackageVM;
        }
        // FIN MMQ 11-12-2023
    }

}
