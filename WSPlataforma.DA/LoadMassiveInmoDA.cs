using System.Linq;
using System;
using System.Collections.Generic;
using WSPlataforma.Util;
//using WSPlataforma.Entities.ViewModel;
using WSPlataforma.Entities.LoadMassiveModel;
using WSPlataforma.Entities.LoadMassiveInmoModel.ViewModel;
using WSPlataforma.Entities.LoadMassiveInmoModel;
using Oracle.DataAccess.Client;
using System.Data;
using System.Configuration;
//using WSPlataforma.Entities.AccountStateModel.ViewModel;
using WSPlataforma.Entities.QuotationModel.ViewModel;


namespace WSPlataforma.DA
{
    public class LoadMassiveInmoDA : ConnectionBase
    {
        public List<BranchVM> GetBranchList(string branch)
        {
            var sPackageName = ProcedureName.pkg_CargaMasiva + ".REA_LIST_BRANCH";
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<BranchVM> ListBranch = new List<BranchVM>();

            try
            {
                //OUTPUT
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureInmo(sPackageName, parameter))
                {
                    ListBranch = dr.ReadRowsList<BranchVM>();
                    ELog.CloseConnection(dr);
                }

                if (!String.IsNullOrEmpty(branch) && branch != "undefined")
                {
                    string[] ramoList = ELog.obtainConfig("ramoList" + branch).Split(';');
                    ListBranch = ListBranch.Where((x) => ramoList.Contains(x.NBRANCH.ToString())).ToList();
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetBranchList", ex.ToString(), "3");
            }

            return ListBranch;
        }
        //FIN MARC
        public List<ProcessHeaderBM> GetProcessHeader(double Nbranch, string DateProcess, int Idproduct)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<ProcessHeaderBM> ListProcess = new List<ProcessHeaderBM>();
            string storedProcedureName = ProcedureName.pkg_CargaMasivaTrama + ".SPS_LIST_HEADERPROC";
            try
            {
                //INPUT                
                parameter.Add(new OracleParameter("V_DATEPROCESS", OracleDbType.Date, DateProcess, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Double, Nbranch, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_IDPRODUCT", OracleDbType.Int32, Idproduct, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureInmo(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        ProcessHeaderBM item = new ProcessHeaderBM();
                        item.IdHeaderProcess = odr["NIDHEADERPROC"] == DBNull.Value ? string.Empty : odr["NIDHEADERPROC"].ToString();
                        item.IdIdentity = odr["NIDENTITY"] == DBNull.Value ? string.Empty : odr["NIDENTITY"].ToString();
                        item.IdProduct = odr["NPRODUCT"] == DBNull.Value ? string.Empty : odr["NPRODUCT"].ToString();
                        item.StatusProc = (Entities.LoadMassiveModel.Status)(Int32.Parse(odr["NSTATUSPROC"].ToString()));
                        item.DescriptionStatus = odr["SDESCRIPTION"] == DBNull.Value ? string.Empty : odr["SDESCRIPTION"].ToString();
                        item.UserCode = odr["NUSERCODE"] == DBNull.Value ? string.Empty : odr["NUSERCODE"].ToString();
                        item.DIniPorcess = odr["DREGINIPROC"] == DBNull.Value ? string.Empty : odr["DREGINIPROC"].ToString();
                        item.DFinProcess = odr["DREGFINPROC"] == DBNull.Value ? string.Empty : odr["DREGFINPROC"].ToString();
                        item.DescriptionIdentity = odr["SDESCRIPT"] == DBNull.Value ? string.Empty : odr["SDESCRIPT"].ToString();
                        item.DescriptionFile = odr["SFILENAME"] == DBNull.Value ? string.Empty : odr["SFILENAME"].ToString();
                        item.DescriptionBranch = odr["SBRANCH"] == DBNull.Value ? string.Empty : odr["SBRANCH"].ToString();
                        ListProcess.Add(item);
                    }
                }
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("GetProcessHeader", ex.ToString(), "3");
            }

            return ListProcess;
        }

        public List<ProcessDetailBM> GetProcessDetail(int IdHeaderProcess)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<ProcessDetailBM> ListProcessDetail = new List<ProcessDetailBM>();
            string storedProcedureName = ProcedureName.pkg_CargaMasivaTrama + ".SPS_LIST_DETAILPROC";
            try
            {
                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleINMOB"].ToString()))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = cn;

                        IDataReader odr = null;

                        cmd.CommandText = string.Format("{0}.{1}", ProcedureName.pkg_CargaMasivaTrama, "SPS_LIST_DETAILPROC");

                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new OracleParameter("P_NIDHEADERPROC", OracleDbType.Int32, IdHeaderProcess, ParameterDirection.Input));
                        cmd.Parameters.Add("C_TABLE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        cn.Open();
                        odr = cmd.ExecuteReader();

                        if (odr != null)
                        {
                            while (odr.Read())
                            {
                                ProcessDetailBM item = new ProcessDetailBM();
                                item.IdDetailProcess = odr["NIDDETAILPROC"] == DBNull.Value ? string.Empty : odr["NIDDETAILPROC"].ToString();
                                item.IdHeaderProcess = odr["NIDHEADERPROC"] == DBNull.Value ? string.Empty : odr["NIDHEADERPROC"].ToString();
                                item.IdStatusDetail = odr["NSTATUSPROC"] == DBNull.Value ? string.Empty : odr["NSTATUSPROC"].ToString();
                                item.IdFileConfig = odr["NIDFILECONFIG"] == DBNull.Value ? string.Empty : odr["NIDFILECONFIG"].ToString();
                                item.DescriptionStatus = odr["SDESCRIPTION"] == DBNull.Value ? string.Empty : odr["SDESCRIPTION"].ToString();
                                item.DIniDetail = odr["DREGINIDETAIL"] == DBNull.Value ? string.Empty : odr["DREGINIDETAIL"].ToString();
                                item.DFinDetail = odr["DREGFINDETAIL"] == DBNull.Value ? string.Empty : odr["DREGFINDETAIL"].ToString();
                                item.FileName = odr["SFILENAME"] == DBNull.Value ? string.Empty : odr["SFILENAME"].ToString();
                                item.table_reference = odr["STABLEREFERENCE"] == DBNull.Value ? string.Empty : odr["STABLEREFERENCE"].ToString();
                                item.FileName_Save = odr["SFILENAME_SAVE"] == DBNull.Value ? string.Empty : odr["SFILENAME_SAVE"].ToString();
                                ListProcessDetail.Add(item);
                            }
                        }
                        cn.Close();
                        cmd.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetProcessDetail", ex.ToString(), "3");
            }

            return ListProcessDetail;
        }

        public string GetAvanceDetailProcess(int IdDetailProcess, int IdHeaderProcess)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<ProcessLogBM> ListProcessLog = new List<ProcessLogBM>();
            string Porcent = "0";
            string storedProcedureName = ProcedureName.pkg_CargaMasivaTrama + ".SPS_GET_AVANCE_DETAIL";
            try
            {

                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleINMOB"].ToString()))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = cn;

                        //IDataReader reader = null;

                        cmd.CommandText = string.Format("{0}.{1}", ProcedureName.pkg_CargaMasivaTrama, "SPS_GET_AVANCE_DETAIL");

                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new OracleParameter("P_NIDHEADERPROC", OracleDbType.Int32, IdHeaderProcess, ParameterDirection.Input));
                        cmd.Parameters.Add(new OracleParameter("P_NIDDETAILPROC", OracleDbType.Int32, IdDetailProcess, ParameterDirection.Input));
                        OracleParameter P_TOTAL = new OracleParameter("P_TOTAL", OracleDbType.Int32, 1000, ParameterDirection.Output);
                        cmd.Parameters.Add(P_TOTAL);
                        cn.Open();

                        cmd.ExecuteNonQuery();

                        Porcent = P_TOTAL.Value.ToString();

                        cn.Close();
                        cmd.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetAvanceDetailProcess", ex.ToString(), "3");
            }
            return Porcent;
        }

        public List<PathConfigBM> GetConfigurationPath(int Identity)
        {
            List<PathConfigBM> ListpathConfig = new List<PathConfigBM>();

            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleINMOB"].ToString()))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = cn;

                    IDataReader reader = null;

                    cmd.CommandText = string.Format("{0}.{1}", ProcedureName.pkg_CargaMasivaTrama, "SPS_GET_CONFIGURATION_PATH");

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("P_NIDENTITY", OracleDbType.Int32).Value = Identity;
                    cmd.Parameters.Add("C_TABLE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    cn.Open();

                    reader = cmd.ExecuteReader();

                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            PathConfigBM pathConfig = new PathConfigBM();
                            pathConfig.IdPathConfig = Convert.ToInt16(reader["NIDPATHCONFIG"].ToString());
                            pathConfig.RouteOrigin = reader["SFILEPATHORI"].ToString();
                            pathConfig.RouteDestine = reader["SFILEPATHDES"].ToString();
                            pathConfig.Extension = reader["SEXTENSION"].ToString();
                            pathConfig.Pattern = reader["SPATTERN"].ToString();
                            pathConfig.IdEntity = Convert.ToInt16(reader["NIDENTITY"].ToString());
                            pathConfig.IdProduct = Convert.ToInt16(reader["NPRODUCT"].ToString());
                            pathConfig.Status = reader["NSTATUS"].ToString();
                            ListpathConfig.Add(pathConfig);
                        }
                    }
                    cn.Close();
                    cmd.Dispose();
                }
            }
            return ListpathConfig;
        }
        public List<EntityConfigInmoBM> GetConfigurationEntity()
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<EntityConfigInmoBM> ListEntityConfig = new List<EntityConfigInmoBM>();
            string storedProcedureName = ProcedureName.pkg_CargaMasivaTrama + ".SPS_GET_CONFIGURATION_ENTITY";
            try
            {
                //INPUT
                // parameter.Add(new OracleParameter("P_NIDENTITY", OracleDbType.Int32, Identity, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureInmo(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        EntityConfigInmoBM item = new EntityConfigInmoBM();
                        item.IdEntity = Convert.ToInt16(odr["NIDENTITY"].ToString());
                        item.Description = odr["SDESCRIPT"].ToString();
                        item.ShortName = odr["SSHORTNAME"].ToString();
                        item.Status = Convert.ToInt16(odr["NSTATUS"].ToString());
                        item.Branch = Convert.ToInt16(odr["NBRANCH"].ToString());
                        ListEntityConfig.Add(item);
                    }
                }
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                LogControl.save("GetConfigurationEntity", ex.ToString(), "3");
            }

            return ListEntityConfig;
        }

        public List<FileConfigInmoBM> GetConfigurationFiles(int idPath)
        {

            List<FileConfigInmoBM> ListFileConfig = new List<FileConfigInmoBM>();

            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleINMOB"].ToString()))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = cn;

                    IDataReader reader = null;

                    cmd.CommandText = string.Format("{0}.{1}", ProcedureName.pkg_CargaMasivaTrama, "SPS_GET_CONFIGURATION_FILE");
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("P_NIDPATHCONFIG", OracleDbType.Int32).Value = idPath;
                    cmd.Parameters.Add("C_TABLE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    cn.Open();

                    reader = cmd.ExecuteReader();

                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            FileConfigInmoBM fileConfig = new FileConfigInmoBM();
                            fileConfig.IdFileConfig = Convert.ToInt32(reader["NIDFILECONFIG"].ToString());
                            fileConfig.FileName = reader["SFILENAME"].ToString();
                            fileConfig.Pattern = reader["SPATTERN"].ToString();
                            fileConfig.FileRequired = reader["NFILEREQUIRED"].ToString();
                            fileConfig.FileRequiredLoad = reader["NLOADREQUIRED"].ToString();
                            fileConfig.Order = (!DBNull.Value.Equals(reader["NORDEN"])) ? Convert.ToInt16(reader["NORDEN"].ToString()) : 0;
                            fileConfig.IdPathConfig = Convert.ToInt16(reader["NIDPATHCONFIG"].ToString());
                            fileConfig.IdFileProc = (!DBNull.Value.Equals(reader["NIDFILEPROC"])) ? Convert.ToInt16(reader["NIDFILEPROC"]) : 0;
                            fileConfig.status = reader["NSTATUS"].ToString();
                            fileConfig.table_reference = (!DBNull.Value.Equals(reader["STABLEREFERENCE"])) ? (reader["STABLEREFERENCE"].ToString()) : string.Empty;
                            fileConfig.caracterFormat = reader["SFILEFORMAT"].ToString();
                            fileConfig.flg_optional = reader["SFIELDS"].ToString();
                            fileConfig.Extension = reader["SEXTENSION"].ToString();
                            fileConfig.IndiciadorService = (!DBNull.Value.Equals(reader["SINDSERVICE"])) ? reader["SINDSERVICE"].ToString() : "0";
                            ListFileConfig.Add(fileConfig);
                        }
                    }
                }
            }
            return ListFileConfig;
        }
        public List<ProductLoadVM> GetProductsList(int P_NBRANCH)
        {
            var sPackageName = string.Format("{0}.{1}", ProcedureName.pkg_CargaMasivaTrama, "SPS_LIST_PRODUCT");
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<ProductLoadVM> ListProductByVidaLey = new List<ProductLoadVM>();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, P_NBRANCH, ParameterDirection.Input));
                //OUTPUT
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureInmo(sPackageName, parameter))
                {
                    ListProductByVidaLey = dr.ReadRowsList<ProductLoadVM>();
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetProductsList", ex.ToString(), "3");
            }

            return ListProductByVidaLey;
        }

        public TramaInmoVMResponse SaveUsingOracleBulkCopy(DataTable dataTable, int codeTRX, string TablaTrama, int P_NIDHEADERPROC, int P_NIDDETAILPROC, int codUser)
        {
            string FechaHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            DateTime dr_fch_ini_pag; DateTime dr_fch_fin_pag;           
            string st_fch_ini_pag; string st_fch_fin_pag; string st_fch_emision;
            TramaInmoVMResponse Respuesta = new TramaInmoVMResponse();
            string campoERR = string.Empty;

            try
            {
                List<ConfigFields> lstFieldTrama = GetTramaFields(codeTRX, 1); // OBTIENE CONFIGURACION CAMPOS DE LA TRAMA 
                Int32 rows = 1;                
                DataTable dt = new DataTable();
                              
                switch (codeTRX)
                {
                    case 17: // contrato/emision

                        dt.Columns.Add("NIDHEADERPROC", typeof(int));
                        dt.Columns.Add("NIDTRAMAEMI", typeof(int));
                        dt.Columns.Add("CODIGO_INTERNO");
                        dt.Columns.Add("TIPO_DOC");
                        dt.Columns.Add("NRO_DOC");
                        dt.Columns.Add("RAZON_SOCIAL");
                        dt.Columns.Add("NOMBRES_TIT");
                        dt.Columns.Add("PRIMER_APE_TIT");
                        dt.Columns.Add("SEGUND_APE_TIT");
                        dt.Columns.Add("FECHA_NAC_TIT");
                        dt.Columns.Add("ESTADO_CIVIL");
                        dt.Columns.Add("SEXO");
                        dt.Columns.Add("EMAIL");
                        dt.Columns.Add("FECHA_INI_PAGO");
                        dt.Columns.Add("FECHA_FIN_PAGO");
                        dt.Columns.Add("MONEDA");
                        dt.Columns.Add("PRIMA_TOTAL");
                        dt.Columns.Add("FREC_PAGO");
                        dt.Columns.Add("TIP_EMISION");
                        dt.Columns.Add("CODUSER", typeof(int));
                        dt.Columns.Add("FECHA_CARGA");
                        dt.Columns.Add("NSTATUS", typeof(int));

                        while (rows < dataTable.Rows.Count)
                        {
                            DataRow dr = dt.NewRow();

                            dr_fch_ini_pag = Convert.ToDateTime(dataTable.Rows[rows][5].ToString().Trim());
                            dr_fch_fin_pag = Convert.ToDateTime(dataTable.Rows[rows][6].ToString().Trim());

                            st_fch_ini_pag = dr_fch_ini_pag.ToString("dd/MM/yyyy");
                            st_fch_fin_pag = dr_fch_fin_pag.ToString("dd/MM/yyyy");

                            dr["NIDHEADERPROC"]     = P_NIDHEADERPROC;
                            dr["NIDTRAMAEMI"]       = rows;
                            dr["CODIGO_INTERNO"]    = dataTable.Rows[rows][0].ToString().Trim() == "" ? null : dataTable.Rows[rows][0].ToString().Trim();
                            dr["TIPO_DOC"]          = dataTable.Rows[rows][1].ToString().Trim() == "" ? null : dataTable.Rows[rows][1].ToString().Trim();
                            dr["NRO_DOC"]           = dataTable.Rows[rows][2].ToString().Trim() == "" ? null : dataTable.Rows[rows][2].ToString().Trim();
                            dr["RAZON_SOCIAL"]      = dataTable.Rows[rows][3].ToString().Trim() == "" ? null : dataTable.Rows[rows][3].ToString().Trim();
                            dr["NOMBRES_TIT"]       = null;
                            dr["PRIMER_APE_TIT"]    = null;
                            dr["SEGUND_APE_TIT"]    = null;
                            dr["FECHA_NAC_TIT"]     = null;
                            dr["ESTADO_CIVIL"]      = null;
                            dr["SEXO"]              = null;
                            dr["EMAIL"]             = dataTable.Rows[rows][4].ToString().Trim() == "" ? null : dataTable.Rows[rows][4].ToString().Trim();
                            dr["FECHA_INI_PAGO"]    = st_fch_ini_pag;
                            dr["FECHA_FIN_PAGO"]    = st_fch_fin_pag;
                            dr["MONEDA"]            = dataTable.Rows[rows][7].ToString().Trim() == "" ? null : dataTable.Rows[rows][7].ToString().Trim();
                            dr["PRIMA_TOTAL"]       = dataTable.Rows[rows][8].ToString().Trim() == "" ? null : dataTable.Rows[rows][8].ToString().Trim();
                            dr["FREC_PAGO"]         = dataTable.Rows[rows][9].ToString().Trim() == "" ? null : dataTable.Rows[rows][9].ToString().Trim();
                            dr["TIP_EMISION"]       = dataTable.Rows[rows][10].ToString().Trim() == "" ? null : dataTable.Rows[rows][10].ToString().Trim();
                            dr["CODUSER"]           = codUser;
                            dr["FECHA_CARGA"]       = FechaHora;
                            dr["NSTATUS"]           = 0;

                            rows++;
                            dt.Rows.Add(dr);
                        }

                        break;
                    case 21: // facturacion

                        dt.Columns.Add("NIDHEADERPROC", typeof(int));
                        dt.Columns.Add("NIDTRAMAFACT", typeof(int));
                        dt.Columns.Add("CODIGO_INTERNO");
                        dt.Columns.Add("TIPO_SERVICIO");
                        dt.Columns.Add("GLOSA_COMPROB");
                        dt.Columns.Add("COMPROB_ORIGEN");
                        dt.Columns.Add("FECHA_EMISION");
                        dt.Columns.Add("MONEDA");
                        dt.Columns.Add("MONTO_MENSUAL");
                        dt.Columns.Add("MONTO_PRORRAT");
                        dt.Columns.Add("TIPO_COMPROB");
                        dt.Columns.Add("COMPROB_STOCK");
                        dt.Columns.Add("FECHA_PAGO_CP");
                        dt.Columns.Add("FECHA_CARGA");
                        dt.Columns.Add("CODUSER", typeof(int));
                        dt.Columns.Add("NSTATUS", typeof(int));

                        while (rows < dataTable.Rows.Count)
                        {                            
                            DataRow dr = dt.NewRow();
                            string fechh =  dataTable.Rows[rows][4].ToString().Trim().Substring(0,10);

                            dr_fch_ini_pag = Convert.ToDateTime(fechh); //dataTable.Rows[rows][5].ToString().Trim());
                            st_fch_ini_pag = dr_fch_ini_pag.ToString("dd/MM/yyyy");
                            


                            dr["NIDHEADERPROC"]  = P_NIDHEADERPROC;
                            dr["NIDTRAMAFACT"]   = rows;
                            dr["CODIGO_INTERNO"] = dataTable.Rows[rows][0].ToString().Trim() == null ? null : dataTable.Rows[rows][0].ToString().Trim();
                            dr["TIPO_SERVICIO"]  = dataTable.Rows[rows][1].ToString().Trim() == null ? null : dataTable.Rows[rows][1].ToString().Trim();
                            dr["GLOSA_COMPROB"]  = dataTable.Rows[rows][2].ToString().Trim() == null ? null : dataTable.Rows[rows][2].ToString().Trim();
                            dr["COMPROB_ORIGEN"] = dataTable.Rows[rows][3].ToString().Trim() == null ? null : dataTable.Rows[rows][3].ToString().Trim();
                            dr["FECHA_EMISION"]  = st_fch_ini_pag; //dataTable.Rows[rows][4].ToString().Trim() == null ? null : dataTable.Rows[rows][4].ToString().Trim();
                            dr["MONEDA"]         = dataTable.Rows[rows][5].ToString().Trim() == null ? null : dataTable.Rows[rows][5].ToString().Trim();
                            dr["MONTO_MENSUAL"]  = dataTable.Rows[rows][6].ToString().Trim() == null ? null : dataTable.Rows[rows][6].ToString().Trim();
                            dr["MONTO_PRORRAT"]  = dataTable.Rows[rows][7].ToString().Trim() == null ? null : dataTable.Rows[rows][7].ToString().Trim();
                            dr["TIPO_COMPROB"]   = null;
                            dr["COMPROB_STOCK"]  = null;
                            dr["FECHA_PAGO_CP"]  = null;
                            dr["FECHA_CARGA"]    = FechaHora;                            
                            dr["CODUSER"]        = codUser;                            
                            dr["NSTATUS"]        = 0;
                            rows++;
                            dt.Rows.Add(dr);
                        }
                        break;          
                }

                //INSERTA TRAMA DEL XLS A LA BD
                var resultPackage = new SalidaTramaBaseVM() { P_COD_ERR = "0" };
                var tblNameList = new string[] { TablaTrama };

                try
                {
                    foreach (var tblName in tblNameList)
                    {
                        if (resultPackage.P_COD_ERR == "0")
                        {
                            resultPackage = this.SaveUsingOracleBulkCopyInmo(tblName, dt);
                        }
                    }
                }
                catch (Exception ex)
                {
                    resultPackage.P_COD_ERR = "1";
                    resultPackage.P_MESSAGE = ex.ToString();
                    LogControl.save("SaveUsingOracleBulkCopy", ex.ToString(), "3");
                }

                #region PROCESO GENERAL DE CENTRALIZADOS PARA EMISION(CONTRATO) Y GENERACION DE RECIBOS Y COMPROBANTES

                if (resultPackage.P_COD_ERR == "0")
                {
                    try
                    {
                        var sPackageName = string.Format("{0}.{1}", ProcedureName.pkg_CargaMasivaTramaInmo, "SPS_SET_PROCESS_GENERAL"); // "PKG_CARGA_MASIVA_TRAMA_INMOBILIARIA", "SPS_SET_PROCESS_GRAL");
                        OracleDataReader dr = null;
                        List<OracleParameter> parameter = new List<OracleParameter>();

                        parameter.Add(new OracleParameter("P_NIDHEADERPROC", OracleDbType.Int32, P_NIDHEADERPROC, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NIDDETAILPROC", OracleDbType.Int32, P_NIDDETAILPROC, ParameterDirection.Input));

                        OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, 4000, null, ParameterDirection.Output);
                        OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, 4000, null, ParameterDirection.Output);
                        parameter.Add(P_NCODE);
                        parameter.Add(P_SMESSAGE);
                        var reader = (OracleDataReader)this.ExecuteByStoredProcedureInmo(sPackageName, parameter);

                        Respuesta.MESSAGE = P_SMESSAGE.Value.ToString();
                        Respuesta.NCODE = P_NCODE.Value.ToString();

                        parameter.Clear();

                        ELog.CloseConnection(dr);

                    }
                    catch (Exception ex)
                    {
                        LogControl.save("GetListReciDev", ex.ToString(), "3");
                    }
                }
                else
                {
                    Respuesta.MESSAGE = "ERROR EN LA CARGA DE LA TRAMA EXCEL";
                }
                #endregion

                
               


                return Respuesta;
            }
            catch (Exception ex)
            {
                //Respuesta.MESSAGE = "Campos no configurados en la trama: " + campoERR.Substring(2, campoERR.Length);
                Respuesta.MESSAGE = "El archivo excel contiene datos no validos.";
                Respuesta.NCODE = "1";
                return Respuesta;
            }
        }
        public Entities.AccountStateModel.ViewModel.GenericPackageVM InsertHeaderProc(ProcessHeaderBM entitie)
        {
            ProcessHeaderBM headerProcess = (ProcessHeaderBM)entitie;
            Entities.AccountStateModel.ViewModel.GenericPackageVM generic = new Entities.AccountStateModel.ViewModel.GenericPackageVM();
            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleINMOB"].ToString()))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = cn;

                    //IDataReader reader = null;

                    cmd.CommandText = string.Format("{0}.{1}", ProcedureName.pkg_CargaMasivaTrama, "SPS_INS_CAB_PROCESS");

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("P_NIDENTITY", OracleDbType.Int32).Value = headerProcess.IdIdentity;
                    cmd.Parameters.Add("P_NPRODUCT", OracleDbType.Int32).Value = headerProcess.IdProduct;
                    cmd.Parameters.Add("P_NSTATUSPROC", OracleDbType.Int32).Value = headerProcess.StatusProc;
                    cmd.Parameters.Add("P_NUSERCODE", OracleDbType.Int32).Value = headerProcess.UserCode;
                    var idProcess = new OracleParameter("P_IDHEADERPROC", OracleDbType.Int32, ParameterDirection.Output);

                    cmd.Parameters.Add(idProcess);
                    cn.Open();

                    cmd.ExecuteNonQuery();
                    generic.GenericResponse = Convert.ToInt32(idProcess.Value.ToString());
                    cmd.Dispose();
                }
            }
            return generic;
        }

        public Entities.AccountStateModel.ViewModel.GenericPackageVM InsertDetailProc(ProcessDetailBM entitie)
        {
            ProcessDetailBM DetailProcess = entitie;
            Entities.AccountStateModel.ViewModel.GenericPackageVM generic = new Entities.AccountStateModel.ViewModel.GenericPackageVM();
            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleINMOB"].ToString()))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = cn;

                    //IDataReader reader = null;

                    cmd.CommandText = string.Format("{0}.{1}", ProcedureName.pkg_CargaMasivaTrama, "SPS_INS_DETAIL_PROCESS");

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("P_NIDHEADERPROC", OracleDbType.Int32).Value = DetailProcess.IdHeaderProcess;
                    cmd.Parameters.Add("P_NSTATUSPROC", OracleDbType.Int32).Value = DetailProcess.Status;
                    cmd.Parameters.Add("P_NIDFILECONFIG", OracleDbType.Int32).Value = DetailProcess.IdFileConfig;
                    cmd.Parameters.Add("P_SFILENAME", OracleDbType.Varchar2).Value = DetailProcess.FileName;
                    var idDetailProcess = new OracleParameter("P_NIDDETAILPROC", OracleDbType.Int32, ParameterDirection.Output);
                    cmd.Parameters.Add(idDetailProcess);
                    cn.Open();

                    cmd.ExecuteNonQuery();
                    generic.GenericResponse = Convert.ToInt32(idDetailProcess.Value.ToString());
                    cmd.Dispose();
                }
            }
            return generic;
        }

        public List<PathConfigBM> getPathConfig(int Identity)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<PathConfigBM> ListPathConfig = new List<PathConfigBM>();
            string storedProcedureName = ProcedureName.pkg_CargaMasivaTrama + ".SPS_GET_CONFIGURATION_PATH";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NIDENTITY", OracleDbType.Int32, Identity, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureInmo(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        PathConfigBM item = new PathConfigBM();
                        item.IdPathConfig = Convert.ToInt16(odr["NIDPATHCONFIG"].ToString());
                        item.RouteOrigin = odr["SFILEPATHORI"].ToString();
                        item.RouteDestine = odr["SFILEPATHDES"].ToString();
                        item.Extension = odr["SEXTENSION"].ToString();
                        item.Pattern = odr["SPATTERN"].ToString();
                        item.IdEntity = Convert.ToInt16(odr["NIDENTITY"].ToString());
                        item.IdProduct = Convert.ToInt16(odr["NPRODUCT"].ToString());
                        item.Routereprocessing = odr["SFILEPATHREP"].ToString();
                        item.Status = odr["NSTATUS"].ToString();
                        ListPathConfig.Add(item);
                    }
                }
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                LogControl.save("getPathConfig", ex.ToString(), "3");
            }

            return ListPathConfig;
        }

        public DataTable GetDataExport(int IdDetailProcess, string TableReference, int IdHeaderProcess)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            DataTable DataReturn = new DataTable();
            string storedProcedureName = ProcedureName.pkg_CargaMasivaTrama + ".SPS_EXPORTAR_DATA_PROCESS";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NIDFILECONFIG", OracleDbType.Int32, IdDetailProcess, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TABLE_REFERENCE", OracleDbType.Varchar2, TableReference, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDHEADERPROC", OracleDbType.Int32, IdHeaderProcess, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                P_NCODE.Size = 100;
                P_MESSAGE.Size = 4000;

                parameter.Add(P_NCODE);
                parameter.Add(P_MESSAGE);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureInmo(storedProcedureName, parameter);

                if (odr != null)
                {
                    DataReturn.Load(odr);
                }
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                LogControl.save("GetDataExport", ex.ToString(), "3");
            }

            return DataReturn;
        }


        public DataTable GetDataExportCorrect(int IdDetailProcess, string TableReference, int IdHeaderProcess)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            DataTable DataReturn = new DataTable();
            string storedProcedureName = ProcedureName.pkg_CargaMasivaTrama + ".SPS_EXPORTAR_DATA_CORRECT";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NIDFILECONFIG", OracleDbType.Int32, IdDetailProcess, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDHEADERPROC", OracleDbType.Int32, IdHeaderProcess, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                P_NCODE.Size = 100;
                P_MESSAGE.Size = 4000;

                parameter.Add(P_NCODE);
                parameter.Add(P_MESSAGE);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureInmo(storedProcedureName, parameter);

                if (odr != null)
                {
                    DataReturn.Load(odr);
                }
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                LogControl.save("GetDataExportCorrect", ex.ToString(), "3");
            }

            return DataReturn;
        }

        public List<ConfigFields> GetTramaFields(int P_NIDFILEHEADER, int P_NREQUIRED)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<ConfigFields> ListPathConfig = new List<ConfigFields>();
            string storedProcedureName = ProcedureName.pkg_CargaMasivaTramaInmo + ".SPS_GET_TRAMA_FIELDS";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NIDFILEHEADER", OracleDbType.Int32, P_NIDFILEHEADER, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NREQUIRED", OracleDbType.Int32, P_NREQUIRED, ParameterDirection.Input));
                

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                parameter.Add(C_TABLE);
                parameter.Add(P_NCODE);
                parameter.Add(P_MESSAGE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureInmo(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        ConfigFields item = new ConfigFields();
                        item.SNAME = odr["SNAME"] == DBNull.Value ? string.Empty : odr["SNAME"].ToString();
                        item.SNAME_EQ = odr["SNAME_EQ"] == DBNull.Value ? string.Empty : odr["SNAME_EQ"].ToString();
                        ListPathConfig.Add(item);
                    }
                }
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("getPathConfig", ex.ToString(), "3");
            }
            return ListPathConfig;
        }

        public List<ProcessLogBM> GetProcessLogError(int IdDetailProcess, string Opcion)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<ProcessLogBM> ListProcessLog = new List<ProcessLogBM>();
            string storedProcedureName = ProcedureName.pkg_CargaMasivaTrama + ".SPS_LIST_LOGPROC";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NIDDETAILPROC", OracleDbType.Int32, IdDetailProcess, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TYPE_OBS", OracleDbType.Varchar2, Opcion, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureInmo(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        ProcessLogBM item = new ProcessLogBM();
                        item.IdLogProcess = odr["NIDLOGPROC"] == DBNull.Value ? string.Empty : odr["NIDLOGPROC"].ToString();
                        item.IdDetailProcess = odr["NIDDETAILPROC"] == DBNull.Value ? string.Empty : odr["NIDDETAILPROC"].ToString();
                        item.RecordFile = odr["NRECORDFILE"] == DBNull.Value ? string.Empty : odr["NRECORDFILE"].ToString();
                        item.NameColumn = odr["SNAME"] == DBNull.Value ? string.Empty : odr["SNAME"].ToString();
                        item.DescriptionError = odr["SDESCRIPT"] == DBNull.Value ? string.Empty : odr["SDESCRIPT"].ToString();
                        item.DateRegister = odr["DREGPROC"] == DBNull.Value ? string.Empty : odr["DREGPROC"].ToString();
                        item.IdFileHeader = odr["NIDFILEHEADER"] == DBNull.Value ? string.Empty : odr["NIDFILEHEADER"].ToString();
                        ListProcessLog.Add(item);
                    }
                }
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                LogControl.save("GetProcessLogError", ex.ToString(), "3");
            }

            return ListProcessLog;
        }

    }
}
