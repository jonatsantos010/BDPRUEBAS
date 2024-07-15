using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.DataAccess.Client;
using System.Data;
using WSPlataforma.Util;
using WSPlataforma.Entities.ReprocessModel.ViewModel;
using WSPlataforma.Entities.ReprocessModel.BindingModel;
using System.Diagnostics;

namespace WSPlataforma.DA
{
    public class ReprocessDA : ConnectionBase
    {
        private readonly string Package = "PKG_ONLINE_INTERFACE";

        public int InsertarProceso(ProcessFilter data)
        {
            var parameters = new List<OracleParameter>();
            ProcessVM entities = new ProcessVM();
            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_NEW_PROCESS_999");

            try
            {
                /* INI INSERTA NUEVO PROCESO*/
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int16, data.P_NBRANCH, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NNUMORI", OracleDbType.Int16, data.P_NNUMORI, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCODGRU", OracleDbType.Int16, data.P_NCODGRU, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_ID_JOB", OracleDbType.Long, data.P_ID_JOB, ParameterDirection.Input));
                OracleParameter P_NIDPROCESS = new OracleParameter("P_NIDPROCESS", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NIDPROCESS);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureOnlineDB(procedure, parameters);
                entities.P_NIDPROCESS = Convert.ToInt32(P_NIDPROCESS.Value.ToString());
                entities.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                entities.P_SMESSAGE = P_SMESSAGE.Value.ToString();

                return int.Parse(P_NIDPROCESS.Value.ToString());

            }
            catch (Exception ex)
            {
                LogControl.save("InsertarProceso", ex.ToString(), "3");
                throw;
            }
        }

        public int InsertarReprocesoCab(ReprocessFilter data)
        {
            var parameters = new List<OracleParameter>();
            ReprocessVM entities = new ReprocessVM();
            var procedure = string.Format("{0}.{1}", this.Package, "SPS_OI_NEW_REPROCESS");

            try
            {
                parameters.Add(new OracleParameter("P_NIDPROCESS_ORI", OracleDbType.Int16, data.P_NIDPROCESS_ORI, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NIDPROCESS", OracleDbType.Int16, data.P_NIDPROCESS, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NNUMORI", OracleDbType.Int16, data.P_NNUMORI, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCODGRU", OracleDbType.Int16, data.P_NCODGRU, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int16, data.P_NBRANCH, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NSTATUS", OracleDbType.Int16, data.P_NSTATUS, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SPROCESS_TYPE", OracleDbType.Varchar2, data.P_SPROCESS_TYPE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SIDUSUREG", OracleDbType.Varchar2, data.P_SIDUSUREG, ParameterDirection.Input));          // INI MMQ 14-03-2022 FIN
                OracleParameter P_NIDREPROCESS = new OracleParameter("P_NIDREPROCESS", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NIDREPROCESS);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureOnlineDB(procedure, parameters);
                entities.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                entities.P_SMESSAGE = P_SMESSAGE.Value.ToString();

                return int.Parse(P_NIDREPROCESS.Value.ToString());

            }
            catch (Exception ex)
            {
                LogControl.save("InsertarReprocesoCab", ex.ToString(), "3");
                throw;
            }
        }

        public Task<ReprocessDetailVM> InsertarReprocesoDet(ReprocessDetailFilter data)
        {
            var parameters = new List<OracleParameter>();
            ReprocessDetailVM entities = new ReprocessDetailVM();
            var procedure = string.Format("{0}.{1}", this.Package, "SPS_OI_NEW_REPROCESS_DET");

            try
            {
                parameters.Add(new OracleParameter("P_NIDREPROCESS", OracleDbType.Int32, data.P_NIDREPROCESS, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NIDPROCESS_ORI", OracleDbType.Int32, data.P_NIDPROCESS_ORI, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SPROCESS_TYPE", OracleDbType.Varchar2, data.P_SPROCESS_TYPE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NIDREC_ASI", OracleDbType.Long, data.P_NIDREC_ASI, ParameterDirection.Input));
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
                LogControl.save("InsertarReprocesoDet", ex.ToString(), "3");
                InsertLogerror(data.P_NIDREPROCESS, "SPS_OI_NEW_REPROCESS_DET Lin(114)", ex.Message);
                throw;
            }
            return Task.FromResult<ReprocessDetailVM>(entities);
        }

        public Task<ReprocessVM> InsertarReproceso(ReprocessFilter data)
        {
            var parameters = new List<OracleParameter>();
            ProcessVM entitiesPro = new ProcessVM();
            ReprocessVM entitiesRepCab = new ReprocessVM();
            ReprocessDetailVM entitiesRepDet = new ReprocessDetailVM();

            //Nuevo proceso
            ProcessFilter dataNew = new ProcessFilter();
            dataNew.P_NBRANCH = data.P_NBRANCH;
            dataNew.P_NNUMORI = data.P_NNUMORI;
            dataNew.P_NCODGRU = data.P_NCODGRU;
            dataNew.P_ID_JOB = data.P_JOB;

            int nroReproceso = 0;
            int NIDPROCESS = 0;

            try
            {
                //Cabecera Reproceso
                ReprocessFilter dataCab = new ReprocessFilter();
                dataCab.P_NBRANCH = data.P_NBRANCH;
                dataCab.P_NCODGRU = data.P_NCODGRU;
                dataCab.P_NIDPROCESS_ORI = data.P_NIDPROCESS_ORI;
                dataCab.P_NIDPROCESS = InsertarProceso(dataNew);
                dataCab.P_NNUMORI = data.P_NNUMORI;
                dataCab.P_NSTATUS = data.P_NSTATUS;
                dataCab.P_SPROCESS_TYPE = data.P_SPROCESS_TYPE;
                NIDPROCESS = dataCab.P_NIDPROCESS;

                //GENERA CABECERA REPROCESO
                try
                {
                    nroReproceso = InsertarReprocesoCab(dataCab);
                }
                catch (Exception ex)
                {
                    InsertLogerror(dataCab.P_NIDPROCESS, "InsertarReprocesoCab Lin(151)", ex.Message);
                }

                //GENERA DETALLE REPROCESO
                ReprocessDetailFilter dataDet = new ReprocessDetailFilter();
                int canReg = data.P_LIST.Count();
                for (int i = 0; i < canReg; i++)
                {
                    dataDet.P_NIDREPROCESS = nroReproceso;
                    dataDet.P_NIDPROCESS_ORI = data.P_NIDPROCESS_ORI;
                    dataDet.P_SPROCESS_TYPE = data.P_SPROCESS_TYPE;
                    dataDet.P_NIDREC_ASI = data.P_LIST[i];
                    try
                    {
                        InsertarReprocesoDet(dataDet);
                    }
                    catch (Exception ex)
                    {
                        InsertLogerror(dataCab.P_NIDPROCESS, "InsertarReprocesoDet Lin(173)", ex.Message);
                    }
                }

                if (NIDPROCESS != 0)
                {
                    //NUEVOS ARGUMENTOS PARA VALIDACION PRIMAS COMISIONES
                    string arguments = string.Format(@"{0}|{1}|{2}", nroReproceso, NIDPROCESS, 0);
                    using
                    (
                        var process = new Process
                        {
                            StartInfo = new ProcessStartInfo { FileName = ELog.obtainConfig("exePathRepro"), Arguments = arguments }
                        }
                    )
                    {
                        process.Start();
                    }

                    entitiesRepCab.P_NIDREPROCESS = NIDPROCESS;
                }

            }
            catch (Exception ex)
            {
                LogControl.save("InsertarReproceso", ex.ToString(), "3");
                InsertLogerror(NIDPROCESS, "InsertarReprocesoDet Lin(199)", ex.Message);
                throw;
            }
            return Task.FromResult<ReprocessVM>(entitiesRepCab);
        }

        public Task<ReprocessVM> InsertarReprocesoAsi(ReprocessFilter data)
        {
            var parameters = new List<OracleParameter>();
            ProcessVM entitiesPro = new ProcessVM();
            ReprocessVM entitiesRepCab = new ReprocessVM();
            ReprocessDetailVM entitiesRepDet = new ReprocessDetailVM();

            int NIDPROCESS = data.P_NIDPROCESS_ORI;

            try
            {
                //Cabecera Reproceso
                ReprocessFilter dataCab = new ReprocessFilter();
                dataCab.P_NBRANCH = data.P_NBRANCH;
                dataCab.P_NCODGRU = data.P_NCODGRU;
                dataCab.P_NIDPROCESS_ORI = data.P_NIDPROCESS_ORI;
                dataCab.P_NIDPROCESS = data.P_NIDPROCESS_ORI;
                dataCab.P_NNUMORI = data.P_NNUMORI;
                dataCab.P_NSTATUS = data.P_NSTATUS;
                dataCab.P_SPROCESS_TYPE = data.P_SPROCESS_TYPE;

                //GENERA CABECERA REPROCESO
                int nroReproceso = InsertarReprocesoCab(dataCab);

                //GENERA DETALLE REPROCESO
                ReprocessDetailFilter dataDet = new ReprocessDetailFilter();
                int canReg = data.P_LIST.Count();
                for (int i = 0; i < canReg; i++)
                {
                    dataDet.P_NIDREPROCESS = nroReproceso;
                    dataDet.P_NIDPROCESS_ORI = data.P_NIDPROCESS_ORI;
                    dataDet.P_SPROCESS_TYPE = data.P_SPROCESS_TYPE;
                    dataDet.P_NIDREC_ASI = data.P_LIST[i];
                    InsertarReprocesoDet(dataDet);
                }

                if (NIDPROCESS != 0)
                {
                    //NUEVOS ARGUMENTOS PARA VALIDACION PRIMAS COMISIONES
                    string arguments = string.Format(@"{0}|{1}|{2}", nroReproceso, NIDPROCESS, 0);
                    using
                    (
                        var process = new Process
                        {
                            StartInfo = new ProcessStartInfo { FileName = ELog.obtainConfig("exePathRepro"), Arguments = arguments }
                        }
                    )
                    {
                        process.Start();
                    }
                    entitiesRepCab.P_NIDREPROCESS = NIDPROCESS;
                }

            }
            catch (Exception ex)
            {
                LogControl.save("InsertarReprocesoAsi", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<ReprocessVM>(entitiesRepCab);
        }

        public Task<ReprocessVM> InsertarReprocesoPlanilla(ReprocessFilter data)
        {
            var parameters = new List<OracleParameter>();
            ProcessVM entitiesPro = new ProcessVM();
            ReprocessVM entitiesRepCab = new ReprocessVM();
            ReprocessDetailVM entitiesRepDet = new ReprocessDetailVM();

            int NIDPROCESS = data.P_NIDPROCESS_ORI;

            try
            {
                //Cabecera Reproceso
                ReprocessFilter dataCab = new ReprocessFilter();
                dataCab.P_NBRANCH = data.P_NBRANCH;
                dataCab.P_NCODGRU = data.P_NCODGRU;
                dataCab.P_NIDPROCESS_ORI = data.P_NIDPROCESS_ORI;
                dataCab.P_NIDPROCESS = data.P_NIDPROCESS_ORI;
                dataCab.P_NNUMORI = data.P_NNUMORI;
                dataCab.P_NSTATUS = data.P_NSTATUS;
                dataCab.P_SPROCESS_TYPE = data.P_SPROCESS_TYPE;

                //GENERA CABECERA REPROCESO
                int nroReproceso = InsertarReprocesoCab(dataCab);

                //GENERA DETALLE REPROCESO
                ReprocessDetailFilter dataDet = new ReprocessDetailFilter();
                dataDet.P_NIDREPROCESS = nroReproceso;
                dataDet.P_NIDPROCESS_ORI = data.P_NIDPROCESS_ORI;
                dataDet.P_SPROCESS_TYPE = data.P_SPROCESS_TYPE;
                dataDet.P_NIDREC_ASI = data.P_JOB;
                InsertarReprocesoDet(dataDet);

                if (NIDPROCESS != 0)
                {
                    //NUEVOS ARGUMENTOS PARA VALIDACION PRIMAS COMISIONES
                    string arguments = string.Format(@"{0}|{1}|{2}", nroReproceso, NIDPROCESS, 0);
                    using
                    (
                        var process = new Process
                        {
                            StartInfo = new ProcessStartInfo { FileName = ELog.obtainConfig("exePathRepro"), Arguments = arguments }
                        }
                    )
                    {
                        process.Start();
                    }
                    entitiesRepCab.P_NIDREPROCESS = NIDPROCESS;
                }

            }
            catch (Exception ex)
            {
                LogControl.save("InsertarReprocesoPlanilla", ex.ToString(), "3");
                throw;
            }
            return Task.FromResult<ReprocessVM>(entitiesRepCab);
        }

        public void InsertLogerror(int P_NIDPROCESS, string P_SPROCEDURE, string P_SDESCRIPT)
        {
            var procedure = string.Format("{0}.{1}", this.Package, "SPS_OI_INS_PROCESS_LOG");
            var parameters = new List<OracleParameter>();

            try
            {
                parameters.Add(new OracleParameter("P_NIDPROCESS", OracleDbType.Int32, P_NIDPROCESS, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NNUMORI", OracleDbType.Int32, 1, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCODGRU", OracleDbType.Varchar2, 12, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SPROCEDURE", OracleDbType.Long, P_SPROCEDURE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SDESCRIPT", OracleDbType.Long, P_SDESCRIPT, ParameterDirection.Input));
                //OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                //OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                //parameters.Add(P_NCODE);
                //parameters.Add(P_SMESSAGE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureOnlineDB(procedure, parameters);
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("InsertLogerror", ex.ToString(), "3");
                throw;
            }

        }

        /****************************************************************************************************************************/
        // REPROCESOS DE CONTROL BANCARIO
        /****************************************************************************************************************************/
        public Task<ReprocessVM> InsertarReprocesoAsi_CBCO(ReprocessFilter data)
        {
            var parameters = new List<OracleParameter>();
            ProcessVM entitiesPro = new ProcessVM();
            ReprocessVM entitiesRepCab = new ReprocessVM();
            ReprocessDetailVM entitiesRepDet = new ReprocessDetailVM();

            int NIDPROCESS = data.P_NIDPROCESS_ORI;

            try
            {
                //Cabecera Reproceso CB
                ReprocessFilter dataCab = new ReprocessFilter();
                dataCab.P_NBRANCH = data.P_NBRANCH;
                dataCab.P_NCODGRU = data.P_NCODGRU;
                dataCab.P_NIDPROCESS_ORI = data.P_NIDPROCESS_ORI;
                dataCab.P_NIDPROCESS = data.P_NIDPROCESS_ORI;
                dataCab.P_NNUMORI = data.P_NNUMORI;
                dataCab.P_NSTATUS = data.P_NSTATUS;
                dataCab.P_SPROCESS_TYPE = data.P_SPROCESS_TYPE;
                dataCab.P_SIDUSUREG = data.P_SIDUSUREG;

                //GENERA CABECERA REPROCESO CB
                int nroReproceso = InsertarReprocesoCab_CBCO(dataCab);

                //GENERA DETALLE REPROCESO CB
                ReprocessDetailFilter dataDet = new ReprocessDetailFilter();
                int canReg = data.P_LIST.Count();
                for (int i = 0; i < canReg; i++)
                {
                    dataDet.P_NIDREPROCESS = nroReproceso;
                    dataDet.P_NIDPROCESS_ORI = data.P_NIDPROCESS_ORI;
                    dataDet.P_SPROCESS_TYPE = data.P_SPROCESS_TYPE;
                    dataDet.P_NIDREC_ASI = data.P_LIST[i];
                    InsertarReprocesoDet_CBCO(dataDet);
                }

                if (NIDPROCESS != 0)
                {
                    //NUEVOS ARGUMENTOS PARA VALIDACION PRIMAS COMISIONES
                    string arguments = string.Format(@"{0}|{1}|{2}", nroReproceso, NIDPROCESS, 1);
                    using
                    (
                        var process = new Process
                        {
                            StartInfo = new ProcessStartInfo { FileName = ELog.obtainConfig("exePathRepro"), Arguments = arguments }
                        }
                    )
                    {
                        process.Start();
                    }
                    entitiesRepCab.P_NIDREPROCESS = NIDPROCESS;
                }

            }
            catch (Exception ex)
            {
                LogControl.save("InsertarReprocesoAsi_CBCO", ex.ToString(), "3");
                InsertLogerror(NIDPROCESS, "InsertarReprocesoAsi_CBCO Lin(382)", ex.Message);
                throw;
            }
            return Task.FromResult<ReprocessVM>(entitiesRepCab);
        }

        public int InsertarReprocesoCab_CBCO(ReprocessFilter data)
        {
            var parameters = new List<OracleParameter>();
            ReprocessVM entities = new ReprocessVM();
            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_ADD_CB_REPROCESS");

            try
            {
                parameters.Add(new OracleParameter("P_NIDPROCESS_ORI", OracleDbType.Int16, data.P_NIDPROCESS_ORI, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NIDPROCESS", OracleDbType.Int16, data.P_NIDPROCESS, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NNUMORI", OracleDbType.Int16, data.P_NNUMORI, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCODGRU", OracleDbType.Int16, data.P_NCODGRU, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int16, data.P_NBRANCH, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NSTATUS", OracleDbType.Int16, data.P_NSTATUS, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SPROCESS_TYPE", OracleDbType.Varchar2, data.P_SPROCESS_TYPE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SIDUSUREG", OracleDbType.Varchar2, data.P_SIDUSUREG, ParameterDirection.Input));
                OracleParameter P_NIDREPROCESS = new OracleParameter("P_NIDREPROCESS", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                parameters.Add(P_NIDREPROCESS);
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureOnlineDB(procedure, parameters);
                entities.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                entities.P_SMESSAGE = P_SMESSAGE.Value.ToString();

                return int.Parse(P_NIDREPROCESS.Value.ToString());

            }
            catch (Exception ex)
            {
                LogControl.save("InsertarReprocesoCab_CBCO", ex.ToString(), "3");
                throw;
            }
        }

        public Task<ReprocessDetailVM> InsertarReprocesoDet_CBCO(ReprocessDetailFilter data)
        {
            var parameters = new List<OracleParameter>();
            ReprocessDetailVM entities = new ReprocessDetailVM();
            var procedure = string.Format("{0}.{1}", this.Package, "USP_OI_ADD_CB_REPROCESS_DET");

            try
            {
                parameters.Add(new OracleParameter("P_NIDREPROCESS", OracleDbType.Int32, data.P_NIDREPROCESS, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NIDPROCESS_ORI", OracleDbType.Int32, data.P_NIDPROCESS_ORI, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SPROCESS_TYPE", OracleDbType.Varchar2, data.P_SPROCESS_TYPE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NIDREC_ASI", OracleDbType.Long, data.P_NIDREC_ASI, ParameterDirection.Input));
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
                LogControl.save("InsertarReprocesoDet_CBCO", ex.ToString(), "3");
                InsertLogerror(data.P_NIDREPROCESS, "SPS_OI_NEW_REPROCESS_DET Lin(114)", ex.Message);
                throw;
            }
            return Task.FromResult<ReprocessDetailVM>(entities);
        }
    }
}