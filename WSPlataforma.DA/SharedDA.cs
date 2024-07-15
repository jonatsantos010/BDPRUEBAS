using Newtonsoft.Json;
using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using WSPlataforma.Entities.DemandProcessModel.ViewModel;
using WSPlataforma.Entities.HistoryCreditServiceModel.BindingModel;
using VPolicy = WSPlataforma.Entities.PolicyModel.ViewModel;
using WSPlataforma.Entities.SharedModel.BindingModel;
using WSPlataforma.Entities.SharedModel.ViewModel;
using WSPlataforma.Util;

namespace WSPlataforma.DA
{
    public class SharedDA : ConnectionBase
    {
        WebServiceUtil WebServiceUtil = new WebServiceUtil();

        //INI MARC
        public double GetExperiaScore(string sclient)
        {
            var sPackageName = ProcedureName.pkg_Cotizacion + ".REA_SCORE_EXPERIA";
            double result = 0;
            List<OracleParameter> parameters = new List<OracleParameter>();
            try
            {
                parameters.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, sclient, ParameterDirection.Input));
                OracleParameter P_NSCORE = new OracleParameter("P_NSCORE", OracleDbType.Decimal, result, ParameterDirection.Output);

                P_NSCORE.Size = 100;

                parameters.Add(P_NSCORE);
                this.ExecuteByStoredProcedureVT(sPackageName, parameters);
                result = double.Parse(P_NSCORE.Value.ToString());

            }
            catch (Exception ex)
            {
                LogControl.save("GetExperiaScore", ex.ToString(), "3");
            }
            return result;
        }

        public List<DocumentTypeSntVM> GetDocumentTypeSntList()
        {
            var sPackageName = ProcedureName.pkg_ProcesoDemanda + ".REA_DOCUMENTO";
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<DocumentTypeSntVM> documentTypeSntList = new List<DocumentTypeSntVM>();

            try
            {
                //OUTPUT
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter))
                {
                    documentTypeSntList = dr.ReadRowsList<DocumentTypeSntVM>();
                    ELog.CloseConnection(dr);
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetDocumentTypeSntList", ex.ToString(), "3");
            }

            return documentTypeSntList;
        }

        public bool ValExperiaScore(ConsultBM request)
        {
            bool response = false;

            var sPackageName = ProcedureName.pkg_ClientSGC + ".VAL_INS_DATA_EXPERIA";
            List<OracleParameter> parameters = new List<OracleParameter>();
            try
            {
                parameters.Add(new OracleParameter("P_SCLIENT", OracleDbType.Varchar2, request.sclient, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NIDSYSTEM", OracleDbType.Int32, Convert.ToInt32(ELog.obtainConfig("systemCode")), ParameterDirection.Input));
                OracleParameter P_NFLAG = new OracleParameter("P_NFLAG", OracleDbType.Int32, response, ParameterDirection.Output);

                P_NFLAG.Size = 100;

                parameters.Add(P_NFLAG);
                this.ExecuteByStoredProcedureVT(sPackageName, parameters);
                response = Convert.ToInt32(P_NFLAG.Value.ToString()) == 1 ? true : false;

            }
            catch (Exception ex)
            {
                response = false;
                LogControl.save("ValExperiaScore", ex.ToString(), "3");
            }

            return response;
        }

        public string GetMailFE()
        {
            var sPackageName = ProcedureName.pkg_ProcesoDemanda + ".REA_MAIL";
            string result = "";
            List<OracleParameter> parameters = new List<OracleParameter>();
            try
            {
                OracleParameter P_SDESCRIPT = new OracleParameter("P_SMAIL", OracleDbType.Varchar2, result, ParameterDirection.Output);

                P_SDESCRIPT.Size = 100;

                parameters.Add(P_SDESCRIPT);
                this.ExecuteByStoredProcedureVT(sPackageName, parameters);
                result = P_SDESCRIPT.Value.ToString().Trim();

            }
            catch (Exception ex)
            {
                result = null;
                LogControl.save("GetMailFE", ex.ToString(), "3");
            }
            return result;
        }

        public string GetCurrencyDescription(decimal NCODIGINT)
        {
            var sPackageName = ProcedureName.pkg_CargaMasiva + ".REA_CURRENCY_DESCRIPT_BY_CODE";
            string result = "";
            List<OracleParameter> parameters = new List<OracleParameter>();
            try
            {
                parameters.Add(new OracleParameter("P_NCODIGINT", OracleDbType.Decimal, NCODIGINT, ParameterDirection.Input));

                OracleParameter P_SDESCRIPT = new OracleParameter("P_SDESCRIPT", OracleDbType.Varchar2, result, ParameterDirection.Output);

                P_SDESCRIPT.Size = 200;
                parameters.Add(P_SDESCRIPT);
                this.ExecuteByStoredProcedureVT(sPackageName, parameters);
                result = P_SDESCRIPT.Value.ToString().Trim();

            }
            catch (Exception ex)
            {
                result = null;
                LogControl.save("GetCurrencyDescription", ex.ToString(), "3");
            }
            return result;
        }

        public string GetRechargeDescription(decimal NCODREC)
        {
            var sPackageName = ProcedureName.pkg_CargaMasiva + ".REA_RECHARGE_DESCRIPT_BY_CODE";
            string result = "";
            List<OracleParameter> parameters = new List<OracleParameter>();
            try
            {
                parameters.Add(new OracleParameter("P_NCODREC", OracleDbType.Decimal, NCODREC, ParameterDirection.Input));

                OracleParameter P_SDESCRIPT = new OracleParameter("P_SDESCRIPT", OracleDbType.Varchar2, result, ParameterDirection.Output);

                P_SDESCRIPT.Size = 200;
                parameters.Add(P_SDESCRIPT);
                this.ExecuteByStoredProcedureVT(sPackageName, parameters);
                result = P_SDESCRIPT.Value.ToString().Trim();

            }
            catch (Exception ex)
            {
                result = null;
                LogControl.save("GetRechargeDescription", ex.ToString(), "3");
            }
            return result;
        }

        public List<CoverTypeVM> GetCoverTypeList()
        {
            var sPackageName = ProcedureName.pkg_CargaMasiva + ".REA_LIST_COVER_TYPE";
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<CoverTypeVM> coverTypeList = new List<CoverTypeVM>();

            try
            {
                //OUTPUT
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter))
                {
                    coverTypeList = dr.ReadRowsList<CoverTypeVM>();
                    ELog.CloseConnection(dr);
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetCoverTypeList", ex.ToString(), "3");
            }

            return coverTypeList;
        }

        public List<RechargeTypeVM> GetRechargeTypeList()
        {
            var sPackageName = ProcedureName.pkg_CargaMasiva + ".REA_LIST_RECHARGE";
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<RechargeTypeVM> rechargeTypeList = new List<RechargeTypeVM>();

            try
            {
                //OUTPUT
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter))
                {
                    rechargeTypeList = dr.ReadRowsList<RechargeTypeVM>();
                    ELog.CloseConnection(dr);
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetRechargeTypeList", ex.ToString(), "3");
            }

            return rechargeTypeList;
        }

        public List<SinisterVM> GetSinisterList()
        {
            List<SinisterVM> sinisterList = new List<SinisterVM>();

            try
            {
                List<OracleParameter> parameter = new List<OracleParameter>();
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(ProcedureName.pkg_CargaMasiva + ".REA_LIST_SINISTER", parameter))
                {
                    sinisterList = dr.ReadRowsList<SinisterVM>();
                    ELog.CloseConnection(dr);
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetSinisterList", ex.ToString(), "3");
            }

            return sinisterList;
        }

        public List<GenericVM> GetGenericList()
        {
            List<GenericVM> genericList = new List<GenericVM>();

            try
            {
                List<OracleParameter> parameter = new List<OracleParameter>();
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(ProcedureName.pkg_CargaMasiva + ".REA_LIST_GENERIC", parameter))
                {
                    genericList = dr.ReadRowsList<GenericVM>();
                    ELog.CloseConnection(dr);
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetGenericList", ex.ToString(), "3");
            }

            return genericList;
        }

        public List<TypeFacturaVM> getInfoTable(int idTable)
        {
            var spExecute = ProcedureName.sp_InfoTablaGenerica;
            var response = new List<TypeFacturaVM>();

            try
            {
                var parameter = new List<OracleParameter>();
                parameter.Add(new OracleParameter("P_NID_TABLE", OracleDbType.Int32, idTable, ParameterDirection.Input));

                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);
                OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(spExecute, parameter);

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        var item = new TypeFacturaVM();
                        item.id = Convert.ToInt32(dr["NID_DETAIL"].ToString());
                        item.descripcion = dr["SDESCRIPT"].ToString();
                        response.Add(item);
                    }
                }

                dr.Close();
            }
            catch (Exception ex)
            {
                LogControl.save("getInfoTable", ex.ToString(), "3");
            }

            return response;
        }

        public List<ClasificationVM> GetClasificationList()
        {
            List<ClasificationVM> reInsuranceList = new List<ClasificationVM>();

            try
            {
                List<OracleParameter> parameter = new List<OracleParameter>();
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(ProcedureName.pkg_CargaMasiva + ".REA_LIST_CLASIFICATION", parameter))
                {
                    reInsuranceList = dr.ReadRowsList<ClasificationVM>();
                    ELog.CloseConnection(dr);
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetClasificationList", ex.ToString(), "3");
            }

            return reInsuranceList;
        }

        public List<ReInsuranceVM> GetReInsuranceList()
        {
            List<ReInsuranceVM> reInsuranceList = new List<ReInsuranceVM>();

            try
            {
                List<OracleParameter> parameter = new List<OracleParameter>();
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(ProcedureName.pkg_CargaMasiva + ".REA_LIST_REINSURANCE", parameter))
                {
                    reInsuranceList = dr.ReadRowsList<ReInsuranceVM>();
                    ELog.CloseConnection(dr);
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetReInsuranceList", ex.ToString(), "3");
            }

            return reInsuranceList;
        }

        public List<ContabilityVM> GetContabilityList()
        {
            List<ContabilityVM> contabilityList = new List<ContabilityVM>();

            try
            {
                List<OracleParameter> parameter = new List<OracleParameter>();
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(ProcedureName.pkg_CargaMasiva + ".REA_LIST_CONTABILITY", parameter))
                {
                    contabilityList = dr.ReadRowsList<ContabilityVM>();
                    ELog.CloseConnection(dr);
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetContabilityList", ex.ToString(), "3");
            }

            return contabilityList;
        }

        public List<MortalityVM> GetMortalityList()
        {
            var sPackageName = ProcedureName.pkg_CargaMasiva + ".REA_LIST_MORTALITY";
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<MortalityVM> mortalityList = new List<MortalityVM>();

            try
            {
                //OUTPUT
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter))
                {
                    mortalityList = dr.ReadRowsList<MortalityVM>();
                    ELog.CloseConnection(dr);
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetMortalityList", ex.ToString(), "3");
            }

            return mortalityList;
        }

        public List<StateVM> GetStateList()
        {
            var sPackageName = ProcedureName.pkg_CargaMasiva + ".REA_LIST_STATE";
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<StateVM> stateList = new List<StateVM>();

            try
            {
                //OUTPUT
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter))
                {
                    stateList = dr.ReadRowsList<StateVM>();
                    ELog.CloseConnection(dr);
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetStateList", ex.ToString(), "3");
            }

            return stateList;
        }

        public List<ProductVM> GetProductsListByBranch(string P_NBRANCH)
        {
            var sPackageName = ProcedureName.pkg_CargaMasiva + ".REA_LIST_PRODUCT";
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<ProductVM> ListProductByVidaLey = new List<ProductVM>();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Decimal, P_NBRANCH, ParameterDirection.Input));
                //OUTPUT
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter))
                {
                    ListProductByVidaLey = dr.ReadRowsList<ProductVM>();
                    ELog.CloseConnection(dr);
                }

                #region Filtro de productos
                string[] ramoList = ELog.obtainConfig("productoList" + P_NBRANCH).Split(';');
                ListProductByVidaLey = ListProductByVidaLey.Where((x) => ramoList.Contains(x.COD_PRODUCT.ToString())).ToList();
                #endregion

            }
            catch (Exception ex)
            {
                LogControl.save("GetProductsListByBranch", ex.ToString(), "3");
            }

            return ListProductByVidaLey;
        }

        public List<BranchVM> GetBranchList(string branch)
        {
            var sPackageName = ProcedureName.pkg_CargaMasiva + ".REA_LIST_BRANCH";
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<BranchVM> ListBranch = new List<BranchVM>();

            try
            {
                //OUTPUT
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter))
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

        public List<BranchVM> GetBranches(string productId, string epsId)
        {
            var sPackageName = ProcedureName.pkg_CargaMasiva + ".REA_LIST_BRANCH";
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<BranchVM> ListBranch = new List<BranchVM>();
            string[] branchArray = ELog.obtainConfig("ramoList" + productId + epsId).Split(';');

            try
            {
                //OUTPUT
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter))
                {
                    ListBranch = dr.ReadRowsList<BranchVM>();
                    ELog.CloseConnection(dr);
                }

                if (!String.IsNullOrEmpty(productId))
                {
                    ListBranch = ListBranch.Where((x) => branchArray.Contains(x.NBRANCH.ToString())).ToList();
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetBranches", ex.ToString(), "3");
            }

            return ListBranch;
        }

        public List<PersonTypeVM> GetPersonTypeList()
        {

            List<OracleParameter> parameter = new List<OracleParameter>();
            List<PersonTypeVM> response = new List<PersonTypeVM>();
            string storedProcedureName = ProcedureName.pkg_BDUCliente + ".SP_SEL_TYPE_PERSON";
            try
            {
                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, response, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                while (odr.Read())
                {
                    PersonTypeVM item = new PersonTypeVM();
                    item.Id = Convert.ToInt32(odr["NPERSON_TYP"]);
                    item.Name = odr["SDESCRIPT"].ToString().Substring(0, 2);

                    response.Add(item);
                }
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("GetPersonTypeList", ex.ToString(), "3");
            }

            return response;

        }
        public List<DocumentTypeVM> GetDocumentTypeList(string codProducto)
        {

            List<OracleParameter> parameter = new List<OracleParameter>();
            List<DocumentTypeVM> response = new List<DocumentTypeVM>();
            string storedProcedureName = ProcedureName.pkg_BDUCliente + ".SP_SEL_TYPE_DOCUMENT";
            try
            {
                //OUTPUT
                List<DocumentTypeVM> responsePreview = new List<DocumentTypeVM>();
                string[] listTypeDocument = ELog.obtainConfig("documentAuth" + codProducto).Split(';');
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, response, ParameterDirection.Output);
                parameter.Add(C_TABLE);


                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                while (odr.Read())
                {
                    DocumentTypeVM item = new DocumentTypeVM();
                    item.Id = Convert.ToInt32(odr["NIDDOC_TYPE"]);
                    item.Name = odr["SDESCRIPT"].ToString();
                    responsePreview.Add(item);
                }
                ELog.CloseConnection(odr);


                listTypeDocument.ToList().ForEach(type =>
                {
                    foreach (var item in responsePreview)
                    {
                        if (item.Id == Convert.ToInt32(type))
                        {
                            if (item.Id == Convert.ToInt32(type))
                            {
                                response.Add(item);
                            }
                        }
                    }
                });

            }
            catch (Exception ex)
            {
                LogControl.save("GetDocumentTypeList", ex.ToString(), "3");
            }

            return response;
        }

        public List<ProfileCampVM> GetProfileEsp()
        {

            List<OracleParameter> parameter = new List<OracleParameter>();
            List<ProfileCampVM> response = new List<ProfileCampVM>();
            string storedProcedureName = ProcedureName.pkg_BDUCliente + ".SP_EXIST_USER_ESP";
            try
            {
                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, response, ParameterDirection.Output);
                parameter.Add(C_TABLE);


                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                while (odr.Read())
                {
                    ProfileCampVM item = new ProfileCampVM();
                    item.Profile = Convert.ToInt32(odr["NIDPROFILE"]);
                    item.NIDUSER = Convert.ToInt32(odr["NIDUSER"]);
                    item.SCLIENT = Convert.ToString(odr["SCLIENT"]);
                    item.RUC = Convert.ToString(odr["RUC"]);
                    response.Add(item);
                }

                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                LogControl.save("GetProfileEsp", ex.ToString(), "3");
            }

            return response;
        }

        public List<DistrictVM> GetDistrictList(Int32 provinceId)
        {

            List<OracleParameter> parameter = new List<OracleParameter>();
            List<DistrictVM> response = new List<DistrictVM>();
            string storedProcedureName = ProcedureName.pkg_RegistroRapido + ".SPS_SEL_MUNICIPALITY";
            try
            {
                //input
                parameter.Add(new OracleParameter("P_NLOCAL", OracleDbType.Int32, provinceId, ParameterDirection.Input));
                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, response, ParameterDirection.Output);
                parameter.Add(C_TABLE);


                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                while (odr.Read())
                {
                    DistrictVM item = new DistrictVM();
                    item.Id = Convert.ToInt32(odr["NMUNICIPALITY"]);
                    item.Name = odr["SDESCRIPT"].ToString();

                    response.Add(item);
                }
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                LogControl.save("GetDistrictList", ex.ToString(), "3");
            }

            return response;
        }

        public List<ProvinceVM> GetProvinceList(Int32 departmentId)
        {

            List<OracleParameter> parameter = new List<OracleParameter>();
            List<ProvinceVM> response = new List<ProvinceVM>();
            string storedProcedureName = ProcedureName.pkg_RegistroRapido + ".SPS_SEL_TAB_LOCAT";
            try
            {
                //input
                parameter.Add(new OracleParameter("P_NPROVINCE", OracleDbType.Int32, departmentId, ParameterDirection.Input));
                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, response, ParameterDirection.Output);
                parameter.Add(C_TABLE);


                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                while (odr.Read())
                {
                    ProvinceVM item = new ProvinceVM();
                    item.Id = Convert.ToInt32(odr["NLOCAL"]);
                    item.Name = odr["SDESCRIPT"].ToString();

                    response.Add(item);
                }
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                LogControl.save("GetProvinceList", ex.ToString(), "3");
            }

            return response;
        }

        public List<ProductVM> GetProducts(string productId, string epsId, string branch)
        {
            List<ProductVM> productList = new List<ProductVM>();
            string[] prodList = ELog.obtainConfig("productoList" + productId + epsId).Split(';');

            //try
            //{
            //    List<OracleParameter> parameter = new List<OracleParameter>();
            //    parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

            //    OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(ProcedureName.pkg_Cotizacion + ".REA_PRODUCT_SCTR", parameter);

            //    while (odr.Read())
            //    {
            //        ProductVM item = new ProductVM();
            //        item.COD_PRODUCT = Convert.ToInt64(odr["COD_PRODUCT"]);
            //        item.NBRANCH = Convert.ToInt64(odr["NBRANCH"]);
            //        item.DES_PRODUCT = odr["DES_PRODUCT"].ToString();
            //        item.TIP_PRODUCT = odr["TIP_PRODUCT"].ToString();

            //        productList.Add(item);
            //    }
            //    ELog.CloseConnection(odr);

            //    if (!String.IsNullOrEmpty(productId))
            //    {
            //        productList = productList.Where((x) => prodList.Contains(x.COD_PRODUCT.ToString()) &&
            //                                                                 x.NBRANCH == Convert.ToInt64(branch)).ToList();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    ELog.save(this, ex.ToString());
            //}

            using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ToString()))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        OracleDataReader odr;

                        cmd.Connection = cn;
                        cmd.CommandText = ProcedureName.pkg_Cotizacion + ".REA_PRODUCT_SCTR";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;

                        cn.Open();
                        odr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        if (odr.HasRows)
                        {
                            while (odr.Read())
                            {
                                var item = new ProductVM();
                                item.COD_PRODUCT = Convert.ToInt64(odr["COD_PRODUCT"]);
                                item.NBRANCH = Convert.ToInt64(odr["NBRANCH"]);
                                item.DES_PRODUCT = odr["DES_PRODUCT"].ToString();
                                item.TIP_PRODUCT = odr["TIP_PRODUCT"].ToString();

                                productList.Add(item);
                            }
                        }

                        odr.Close();

                        //response.GenericResponse = listTransacction;
                        if (!String.IsNullOrEmpty(productId))
                        {
                            productList = productList.Where((x) => prodList.Contains(x.COD_PRODUCT.ToString()) &&
                                                                                     x.NBRANCH == Convert.ToInt64(branch)).ToList();
                        }

                    }
                    catch (Exception ex)
                    {
                        productList = null;
                        LogControl.save("GetProducts", ex.ToString(), "3");
                    }
                    finally
                    {
                        if (cn.State == ConnectionState.Open) cn.Close();
                    }
                }
            }

            return productList;
        }

        public List<ProductVM> GetListProductsForBranch(string branch)
        {
            List<ProductVM> productList = new List<ProductVM>();

            try
            {
                List<OracleParameter> parameter = new List<OracleParameter>();
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, branch, ParameterDirection.Input));
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(ProcedureName.pkg_Cotizacion + ".REA_PRODUCT_X_BRANCH", parameter);

                while (odr.Read())
                {
                    ProductVM item = new ProductVM();
                    item.COD_PRODUCT = Convert.ToInt64(odr["COD_PRODUCT"]);
                    item.NBRANCH = Convert.ToInt64(odr["NBRANCH"]);
                    item.DES_PRODUCT = odr["DES_PRODUCT"].ToString();
                    item.TIP_PRODUCT = odr["TIP_PRODUCT"].ToString();

                    productList.Add(item);
                }
                ELog.CloseConnection(odr);

                //if (!String.IsNullOrEmpty(productId))
                //{
                // productList = productList.Where((x) => prodList.Contains(x.COD_PRODUCT.ToString()) && x.NBRANCH == Convert.ToInt64(branch)).ToList();
                //} 
            }
            catch (Exception ex)
            {
                LogControl.save("GetListProductsForBranch", ex.ToString(), "3");
            }

            return productList;
        }

        public List<DepartmentVM> GetDepartmentList()
        {

            List<OracleParameter> parameter = new List<OracleParameter>();
            List<DepartmentVM> response = new List<DepartmentVM>();
            string storedProcedureName = ProcedureName.pkg_RegistroRapido + ".SPS_SEL_PROVINCE";
            try
            {
                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, response, ParameterDirection.Output);
                parameter.Add(C_TABLE);


                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                while (odr.Read())
                {
                    DepartmentVM item = new DepartmentVM();
                    item.Id = Convert.ToInt32(odr["NPROVINCE"]);
                    item.Name = odr["SDESCRIPT"].ToString();

                    response.Add(item);
                }
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                LogControl.save("GetDepartmentList", ex.ToString(), "3");
            }


            return response;

        }
        public List<EconomicActivityVM> GetEconomicActivityList(string technicalActivityId)
        {

            List<OracleParameter> parameter = new List<OracleParameter>();
            List<EconomicActivityVM> response = new List<EconomicActivityVM>();
            string storedProcedureName = ProcedureName.pkg_SedesCliente + ".SEL_ECONOMICACTIVITYLIST";
            try
            {
                parameter.Add(new OracleParameter("P_SCOD_ACTIVITY_TEC", OracleDbType.Varchar2, technicalActivityId, ParameterDirection.Input));
                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, response, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                while (odr.Read())
                {
                    EconomicActivityVM item = new EconomicActivityVM();
                    item.Id = odr["SCOD_CIUU"].ToString();
                    item.Name = odr["SDESC_CIUU"].ToString();
                    item.Delimiter = odr["SDELIMITER"].ToString();

                    response.Add(item);
                }
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                LogControl.save("GetEconomicActivityList", ex.ToString(), "3");
            }

            return response;
        }

        public List<TechnicalActivityVM> GetTechnicalActivityList(int codProducto)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<TechnicalActivityVM> response = new List<TechnicalActivityVM>();
            string[] listActivity = ELog.obtainConfig("activityExc").Split(';');
            string storedProcedureName = ProcedureName.pkg_SedesCliente + ".SEL_TECNICALACTIVITYLIST";
            try
            {
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, response, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                while (odr.Read())
                {
                    TechnicalActivityVM item = new TechnicalActivityVM();
                    item.Id = odr["SCOD_ACTIVITY_TEC"].ToString();
                    item.Id = odr["SCOD_ACTIVITY_TEC"].ToString().Trim();
                    item.Name = odr["SDESCRIPT"].ToString();

                    response.Add(item);
                }
                ELog.CloseConnection(odr);

                if (codProducto == 3)
                {
                    response = response.Where((n) => !(listActivity.Contains(n.Id.ToString()))).ToList();
                }

            }
            catch (Exception ex)
            {
                LogControl.save("GetTechnicalActivityList", ex.ToString(), "3");
            }

            return response;
        }

        public List<TechnicalActivityVM> GetOccupationsList()
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<TechnicalActivityVM> response = new List<TechnicalActivityVM>();
            string storedProcedureName = ProcedureName.pkg_SedesCliente + ".SEL_OCCUPATIONSLIST";
            try
            {
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, response, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                while (odr.Read())
                {
                    TechnicalActivityVM item = new TechnicalActivityVM();
                    item.Id = odr["NTITLE"].ToString();
                    item.Id = odr["NTITLE"].ToString().Trim();
                    item.Name = odr["SDESCRIPT"].ToString();

                    response.Add(item);
                }
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                LogControl.save("GetOccupationsList", ex.ToString(), "3");
            }

            return response;
        }

        //--------------------------------------------NUEVO

        public List<TechnicalActivityVM> GetActivityDesgravamenList()
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<TechnicalActivityVM> response = new List<TechnicalActivityVM>();
            string storedProcedureName = ProcedureName.pkg_SedesCliente + ".SEL_ACTIVITYDESGRALIST";
            try
            {
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, response, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                while (odr.Read())
                {
                    TechnicalActivityVM item = new TechnicalActivityVM();
                    item.Id = odr["NACTIVITY"].ToString();
                    item.Id = odr["NACTIVITY"].ToString().Trim();
                    item.Name = odr["SDESCRIPT"].ToString();

                    response.Add(item);
                }
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                LogControl.save("GetActivityDesgravamenList", ex.ToString(), "3");
            }

            return response;
        }

        //-------------------------------------------

        public List<NationalityVM> GetNationalityList()
        {
            List<NationalityVM> nationalityList = new List<NationalityVM>();

            try
            {
                List<OracleParameter> parameter = new List<OracleParameter>();
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(ProcedureName.pkg_BDUCliente + ".SP_SEL_NACIONALIDAD_CLIENTE", parameter))
                {
                    nationalityList = dr.ReadRowsList<NationalityVM>();
                    ELog.CloseConnection(dr);
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetNationalityList", ex.ToString(), "3");
            }

            return nationalityList;
        }

        public List<GenderVM> GetGenderList()
        {
            List<GenderVM> genderList = new List<GenderVM>();

            try
            {
                List<OracleParameter> parameter = new List<OracleParameter>();
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(ProcedureName.pkg_BDUCliente + ".SP_SEL_SEXO_CLIENTE", parameter))
                {
                    genderList = dr.ReadRowsList<GenderVM>();
                    ELog.CloseConnection(dr);
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetGenderList", ex.ToString(), "3");
            }

            return genderList;
        }

        public List<CivilStatusVM> GetCivilStatusList()
        {
            List<CivilStatusVM> civilStatusList = new List<CivilStatusVM>();

            try
            {
                List<OracleParameter> parameter = new List<OracleParameter>();
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(ProcedureName.pkg_BDUCliente + ".SP_SEL_ESTADO_CIVIL_CLIENTE", parameter))
                {
                    civilStatusList = dr.ReadRowsList<CivilStatusVM>();
                    ELog.CloseConnection(dr);
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetCivilStatusList", ex.ToString(), "3");
            }

            return civilStatusList;
        }

        public List<ProfessionVM> GetProfessionList()
        {
            List<ProfessionVM> professionList = new List<ProfessionVM>();

            try
            {
                List<OracleParameter> parameter = new List<OracleParameter>();
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(ProcedureName.pkg_BDUCliente + ".SP_SEL_PROFESION_CLIENTE", parameter))
                {
                    professionList = dr.ReadRowsList<ProfessionVM>();
                    ELog.CloseConnection(dr);
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetProfessionList", ex.ToString(), "3");
            }

            return professionList;
        }

        public List<PhoneTypeVM> GetPhoneTypeList()
        {
            List<PhoneTypeVM> phoneTypeList = new List<PhoneTypeVM>();

            try
            {
                List<OracleParameter> parameter = new List<OracleParameter>();
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(ProcedureName.pkg_BDUCliente + ".SP_SEL_TIPO_TELEFONO", parameter))
                {
                    phoneTypeList = dr.ReadRowsList<PhoneTypeVM>();
                    ELog.CloseConnection(dr);
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetPhoneTypeList", ex.ToString(), "3");
            }

            return phoneTypeList;
        }

        public List<CityCodeVM> GetCityCodeList()
        {
            List<CityCodeVM> cityCodeList = new List<CityCodeVM>();

            try
            {
                List<OracleParameter> parameter = new List<OracleParameter>();
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(ProcedureName.pkg_BDUCliente + ".SP_SEL_CODIGO_AREA", parameter))
                {
                    cityCodeList = dr.ReadRowsList<CityCodeVM>();
                    ELog.CloseConnection(dr);
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetCityCodeList", ex.ToString(), "3");
            }

            return cityCodeList;
        }
        public List<EmailTypeVM> GetEmailTypeList()
        {
            List<EmailTypeVM> emailTypeList = new List<EmailTypeVM>();

            try
            {
                List<OracleParameter> parameter = new List<OracleParameter>();
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(ProcedureName.pkg_BDUCliente + ".SP_SEL_TIPO_CORREO", parameter))
                {
                    emailTypeList = dr.ReadRowsList<EmailTypeVM>();
                    ELog.CloseConnection(dr);
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetEmailTypeList", ex.ToString(), "3");
            }

            return emailTypeList;
        }
        public List<DirectionTypeVM> GetDirectionTypeList()
        {
            List<DirectionTypeVM> directionTypeList = new List<DirectionTypeVM>();

            try
            {
                List<OracleParameter> parameter = new List<OracleParameter>();
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(ProcedureName.pkg_BDUCliente + ".SP_SEL_TIPO_DIRECCION", parameter))
                {
                    directionTypeList = dr.ReadRowsList<DirectionTypeVM>();
                    ELog.CloseConnection(dr);
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetDirectionTypeList", ex.ToString(), "3");
            }

            return directionTypeList;
        }
        public List<RoadTypeVM> GetRoadTypeList()
        {
            List<RoadTypeVM> roadTypeList = new List<RoadTypeVM>();

            try
            {
                List<OracleParameter> parameter = new List<OracleParameter>();
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(ProcedureName.pkg_BDUCliente + ".SP_SEL_PREFIJO_TIP_DIR", parameter))
                {
                    roadTypeList = dr.ReadRowsList<RoadTypeVM>();
                    ELog.CloseConnection(dr);
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetRoadTypeList", ex.ToString(), "3");
            }

            return roadTypeList;
        }

        public List<InteriorTypeVM> GetInteriorTypeList()
        {
            List<InteriorTypeVM> interiorTypeList = new List<InteriorTypeVM>();

            try
            {
                List<OracleParameter> parameter = new List<OracleParameter>();
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(ProcedureName.pkg_BDUCliente + ".SP_SEL_PREFIJO_TIP_INT", parameter))
                {
                    interiorTypeList = dr.ReadRowsList<InteriorTypeVM>();
                    ELog.CloseConnection(dr);
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetInteriorTypeList", ex.ToString(), "3");
            }

            return interiorTypeList;
        }

        public List<BlockTypeVM> GetBlockTypeList()
        {
            List<BlockTypeVM> blockTypeList = new List<BlockTypeVM>();

            try
            {
                List<OracleParameter> parameter = new List<OracleParameter>();
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(ProcedureName.pkg_BDUCliente + ".SP_SEL_PREFIJO_TIP_BLOCK", parameter))
                {
                    blockTypeList = dr.ReadRowsList<BlockTypeVM>();
                    ELog.CloseConnection(dr);
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetBlockTypeList", ex.ToString(), "3");
            }

            return blockTypeList;
        }

        public List<CJHTTypeVM> GetCJHTTypeList()
        {
            List<CJHTTypeVM> CJHTTypeList = new List<CJHTTypeVM>();

            try
            {
                List<OracleParameter> parameter = new List<OracleParameter>();
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(ProcedureName.pkg_BDUCliente + ".SP_SEL_PREFIJO_TIP_CJHT", parameter))
                {
                    CJHTTypeList = dr.ReadRowsList<CJHTTypeVM>();
                    ELog.CloseConnection(dr);
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetCJHTTypeList", ex.ToString(), "3");
            }

            return CJHTTypeList;
        }

        public List<CountryVM> GetCountryList()
        {
            List<CountryVM> countryList = new List<CountryVM>();

            try
            {
                List<OracleParameter> parameter = new List<OracleParameter>();
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(ProcedureName.pkg_BDUCliente + ".SP_SEL_PAIS", parameter))
                {
                    countryList = dr.ReadRowsList<CountryVM>();
                    ELog.CloseConnection(dr);
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetCountryList", ex.ToString(), "3");
            }

            return countryList;
        }

        public List<ContactTypeVM> GetContactTypeList(ContactTypeBM data)
        {
            List<ContactTypeVM> contactTypeList = new List<ContactTypeVM>();

            try
            {
                List<OracleParameter> parameter = new List<OracleParameter>();
                parameter.Add(new OracleParameter("P_NIDDOC_TYPE", OracleDbType.Int64, data.P_NIDDOC_TYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, data.P_SIDDOC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(ProcedureName.pkg_BDUCliente + ".SP_SEL_TIPO_CONTACTO", parameter))
                {
                    contactTypeList = dr.ReadRowsList<ContactTypeVM>();
                    ELog.CloseConnection(dr);
                }

                if (data.P_SORIGEN == "COT")
                {
                    if (data.P_NIDDOC_TYPE == 1 && data.P_SIDDOC.Substring(0, 2) == "20")
                    {
                        contactTypeList = contactTypeList.Where(x => x.NCODIGO == 8).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetContactTypeList", ex.ToString(), "3");
            }

            return contactTypeList;
        }

        public List<TypeFacturaVM> GetTypeFactura(string codProducto, int perfil)
        {
            List<TypeFacturaVM> types = new List<TypeFacturaVM>();
            List<TypeFacturaVM> response = new List<TypeFacturaVM>();
            try
            {
                List<OracleParameter> parameter = new List<OracleParameter>();
                string[] listTypeFactura = ELog.obtainConfig("typefactura" + codProducto).Split(';');
                string[] lisProductsAP = ELog.obtainConfig("CodProductAP").Split(';');
                int perfilIndividual = Convert.ToInt32(ELog.obtainConfig("ProdIndividual"));
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(ProcedureName.pkg_Cotizacion + ".OBT_TYPEFACTURA", parameter);
                while (odr.Read())
                {
                    TypeFacturaVM item = new TypeFacturaVM();
                    item.id = Convert.ToInt32(odr["SCOLINVOT"]);
                    item.descripcion = odr["SSHORT_DES"].ToString();
                    types.Add(item);
                }
                odr.Close();

                listTypeFactura.ToList().ForEach(type =>
                {
                    foreach (var item in types)
                    {
                        if ((lisProductsAP.Contains(codProducto)) && perfil == perfilIndividual && Convert.ToInt32(type) == Convert.ToInt32(listTypeFactura[1]))
                        {
                            continue;
                        }
                        else
                        {

                            if (item.id == Convert.ToInt32(type))
                            {
                                if (item.id == Convert.ToInt32(type))
                                {
                                    response.Add(item);
                                }
                            }
                        }
                    }
                });

            }
            catch (Exception ex)
            {
                LogControl.save("GetTypeFactura", ex.ToString(), "3");
            }
            return response;
        }

        public List<CurrencyVM> GetCurrencyList(int nbranch, int nproduct)
        {
            List<CurrencyVM> currencyList = new List<CurrencyVM>();

            try
            {
                List<OracleParameter> parameter = new List<OracleParameter>();
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, nbranch, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, nproduct, ParameterDirection.Input));
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(ProcedureName.pkg_Cotizacion + ".REA_CURRENCY", parameter))
                {
                    currencyList = dr.ReadRowsList<CurrencyVM>();
                    ELog.CloseConnection(dr);
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetCurrencyList", ex.ToString(), "3");
            }

            return currencyList;
        }

        public List<CiiuVM> GetCiiuList()
        {
            List<CiiuVM> ciiuList = new List<CiiuVM>();

            try
            {
                List<OracleParameter> parameter = new List<OracleParameter>();
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(ProcedureName.pkg_BDUCliente + ".SP_SEL_CIIU", parameter))
                {
                    ciiuList = dr.ReadRowsList<CiiuVM>();
                    ELog.CloseConnection(dr);
                }
            }
            catch (Exception ex)
            {
                LogControl.save("GetCiiuList", ex.ToString(), "3");
            }

            return ciiuList;
        }

        public ResponseVM ValPhone(PhoneBM data)
        {
            var sPackageName = ProcedureName.pkg_BDUCliente + ".SP_VAL_TELEFONO";
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseVM result = new ResponseVM();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_STI_ACCION", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDDOC_TYPE", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NKEYPHONES", OracleDbType.Varchar2, data.P_NKEYPHONES, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBESTTIMETOCALL", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NAREA_CODE", OracleDbType.Varchar2, data.P_NAREA_CODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SPHONE", OracleDbType.Varchar2, data.P_SPHONE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NORDER", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NEXTENS1", OracleDbType.Varchar2, data.P_NEXTENS1, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NEXTENS2", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPHONE_TYPE", OracleDbType.Varchar2, data.P_NPHONE_TYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Varchar2, data.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCOUNTRY_CODE", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SASSOCADDR", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SKEYADDRESS", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SORIGEN", OracleDbType.Varchar2, data.P_SORIGEN, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, result.C_TABLE, ParameterDirection.Output);
                OracleParameter P_CAMPO = new OracleParameter("P_CAMPO", OracleDbType.Varchar2, result.P_CAMPO, ParameterDirection.Output);
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int64, result.P_NCODE, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, result.P_SMESSAGE, ParameterDirection.Output);

                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;
                P_CAMPO.Size = 4000;

                parameter.Add(C_TABLE);
                parameter.Add(P_CAMPO);
                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);

                this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                result.P_CAMPO = P_CAMPO.Value.ToString();
                result.P_NCODE = Convert.ToInt64(P_NCODE.Value.ToString());
                result.P_SMESSAGE = P_SMESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                result.P_NCODE = 1;
                result.P_SMESSAGE = ex.ToString();
                result.P_CAMPO = string.Empty;
                LogControl.save("ValPhone", ex.ToString(), "3");
            }

            return result;
        }

        public ResponseVM ValCiiu(CiiuBM data)
        {
            var sPackageName = ProcedureName.pkg_BDUCliente + ".SP_VAL_CIIU";
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseVM result = new ResponseVM();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_STI_ACCION", OracleDbType.Varchar2, data.P_TipOper, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDDOC_TYPE", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCIIU", OracleDbType.Varchar2, data.P_SCIIU, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Varchar2, data.P_DEFFECDATE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Varchar2, data.P_NUSERCODE, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int64, result.P_NCODE, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, result.P_SMESSAGE, ParameterDirection.Output);

                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;

                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);

                this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                result.P_NCODE = Convert.ToInt64(P_NCODE.Value.ToString());
                result.P_SMESSAGE = P_SMESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                result.P_NCODE = 1;
                result.P_SMESSAGE = ex.ToString();
                result.P_CAMPO = string.Empty;
                LogControl.save("ValCiiu", ex.ToString(), "3");
            }

            return result;
        }

        public ResponseVM ValEmail(EmailBM data)
        {
            var sPackageName = ProcedureName.pkg_BDUCliente + ".SP_VAL_CORREO";
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseVM result = new ResponseVM();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_STI_ACCION", OracleDbType.Varchar2, data.P_TipOper, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SRECTYPE", OracleDbType.Varchar2, data.P_SRECTYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Varchar2, data.P_DEFFECDATE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDDOC_TYPE", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SE_MAIL", OracleDbType.Varchar2, data.P_SE_MAIL, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Varchar2, data.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SKEYADDRESS", OracleDbType.Varchar2, data.P_SKEYADDRESS, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SORIGEN", OracleDbType.Varchar2, data.P_SORIGEN, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, result.C_TABLE, ParameterDirection.Output);
                OracleParameter P_CAMPO = new OracleParameter("P_CAMPO", OracleDbType.Varchar2, result.P_CAMPO, ParameterDirection.Output);
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int64, result.P_NCODE, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, result.P_SMESSAGE, ParameterDirection.Output);

                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;
                P_CAMPO.Size = 4000;

                parameter.Add(C_TABLE);
                parameter.Add(P_CAMPO);
                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);

                this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                result.P_CAMPO = P_CAMPO.Value.ToString();
                result.P_NCODE = Convert.ToInt64(P_NCODE.Value.ToString());
                result.P_SMESSAGE = P_SMESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                result.P_NCODE = 1;
                result.P_SMESSAGE = ex.ToString();
                result.P_CAMPO = string.Empty;
                LogControl.save("ValEmail", ex.ToString(), "3");
            }

            return result;
        }

        public ResponseVM ValStreet(AddressBM data)
        {
            var sPackageName = ProcedureName.pkg_BDUCliente + ".SP_VAL_DIRECCION";
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseVM result = new ResponseVM();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_STI_ACCION", OracleDbType.Varchar2, data.P_TipOper, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NRECOWNER", OracleDbType.Varchar2, "2", ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SRECTYPE", OracleDbType.Varchar2, data.P_SRECTYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Varchar2, data.P_DEFFECDATE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SSTREET", OracleDbType.Varchar2, data.P_SSTREET, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SZONE", OracleDbType.Varchar2, data.P_SZONE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDDOC_TYPE", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCERTYPE", OracleDbType.Varchar2, data.P_SCERTYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SE_MAIL", OracleDbType.Varchar2, data.P_SE_MAIL, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLAT_SECOND", OracleDbType.Varchar2, data.P_NLAT_SECOND, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLON_SECOND", OracleDbType.Varchar2, data.P_NLON_SECOND, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLAT_COORD", OracleDbType.Varchar2, data.P_NLAT_COORD, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLON_COORD", OracleDbType.Varchar2, data.P_NLON_COORD, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCONTRAT", OracleDbType.Varchar2, data.P_NCONTRAT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLAT_CARDIN", OracleDbType.Varchar2, data.P_NLAT_CARDIN, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLAT_MINUTE", OracleDbType.Varchar2, data.P_NLAT_MINUTE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLON_CARDIN", OracleDbType.Varchar2, data.P_NLON_CARDIN, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLON_MINUTE", OracleDbType.Varchar2, data.P_NLON_MINUTE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCERTIF", OracleDbType.Varchar2, data.P_NCERTIF, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCLAIM", OracleDbType.Varchar2, data.P_NCLAIM, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPOLICY", OracleDbType.Varchar2, data.P_NPOLICY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NZIP_CODE", OracleDbType.Varchar2, data.P_NZIP_CODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLAT_GRADE", OracleDbType.Varchar2, data.P_NLAT_GRADE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCOUNTRY", OracleDbType.Varchar2, data.P_NCOUNTRY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLON_GRADE", OracleDbType.Varchar2, data.P_NLON_GRADE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Varchar2, data.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBK_AGENCY", OracleDbType.Varchar2, data.P_NBK_AGENCY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBANK_CODE", OracleDbType.Varchar2, data.P_NBANK_CODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Varchar2, data.P_NBRANCH, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NOFFICE", OracleDbType.Varchar2, data.P_NOFFICE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPROVINCE", OracleDbType.Varchar2, data.P_NPROVINCE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Varchar2, data.P_NPRODUCT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLOCAL", OracleDbType.Varchar2, data.P_NLOCAL, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SBUILD", OracleDbType.Varchar2, data.P_SBUILD, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NMUNICIPALITY", OracleDbType.Varchar2, data.P_NMUNICIPALITY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NFLOOR", OracleDbType.Varchar2, data.P_NFLOOR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SDEPARTMENT", OracleDbType.Varchar2, data.P_SDEPARTMENT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SPOPULATION", OracleDbType.Varchar2, data.P_SPOPULATION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SINFOR", OracleDbType.Varchar2, data.P_SINFOR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SPOBOX", OracleDbType.Varchar2, data.P_SPOBOX, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SDESCADD", OracleDbType.Varchar2, data.P_SDESCADD, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCOD_AGREE", OracleDbType.Varchar2, data.P_NCOD_AGREE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SREFERENCE", OracleDbType.Varchar2, data.P_SREFERENCE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STI_DIRE", OracleDbType.Varchar2, data.P_STI_DIRE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SNOM_DIRECCION", OracleDbType.Varchar2, data.P_SNOM_DIRECCION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SNUM_DIRECCION", OracleDbType.Varchar2, data.P_SNUM_DIRECCION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STI_BLOCKCHALET", OracleDbType.Varchar2, data.P_STI_BLOCKCHALET, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SBLOCKCHALET", OracleDbType.Varchar2, data.P_SBLOCKCHALET, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STI_INTERIOR", OracleDbType.Varchar2, data.P_STI_INTERIOR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SNUM_INTERIOR", OracleDbType.Varchar2, data.P_SNUM_INTERIOR, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STI_CJHT", OracleDbType.Varchar2, data.P_STI_CJHT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SNOM_CJHT", OracleDbType.Varchar2, data.P_SNOM_CJHT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SETAPA", OracleDbType.Varchar2, data.P_SETAPA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SMANZANA", OracleDbType.Varchar2, data.P_SMANZANA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SLOTE", OracleDbType.Varchar2, data.P_SLOTE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SREFERENCIA", OracleDbType.Varchar2, data.P_SREFERENCIA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOD_DEP_UBI_DOM", OracleDbType.Varchar2, data.P_SCOD_DEP_UBI_DOM, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOD_PRO_UBI_DOM", OracleDbType.Varchar2, data.P_SCOD_PRO_UBI_DOM, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOD_DIS_UBI_DOM", OracleDbType.Varchar2, data.P_SCOD_DIS_UBI_DOM, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SDES_DEP_DOM", OracleDbType.Varchar2, data.P_SDES_DEP_DOM, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SDES_PRO_DOM", OracleDbType.Varchar2, data.P_SDES_PRO_DOM, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SDES_DIS_DOM", OracleDbType.Varchar2, data.P_SDES_DIS_DOM, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SISRENIEC_IND", OracleDbType.Varchar2, "0", ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SKEYADDRESS", OracleDbType.Varchar2, data.P_SKEYADDRESS, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SORIGEN", OracleDbType.Varchar2, data.P_SORIGEN, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, result.C_TABLE, ParameterDirection.Output);
                OracleParameter P_CAMPO = new OracleParameter("P_CAMPO", OracleDbType.Varchar2, result.P_CAMPO, ParameterDirection.Output);
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int64, result.P_NCODE, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, result.P_SMESSAGE, ParameterDirection.Output);

                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;
                P_CAMPO.Size = 4000;

                parameter.Add(C_TABLE);
                parameter.Add(P_CAMPO);
                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);

                this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                result.P_CAMPO = P_CAMPO.Value.ToString();
                result.P_NCODE = Convert.ToInt64(P_NCODE.Value.ToString());
                result.P_SMESSAGE = P_SMESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                result.P_NCODE = 1;
                result.P_SMESSAGE = ex.ToString();
                result.P_CAMPO = string.Empty;
                LogControl.save("ValStreet", ex.ToString(), "3");
            }

            return result;
        }

        public ResponseVM ValContact(ContactBM data)
        {
            var sPackageName = ProcedureName.pkg_BDUCliente + ".SP_VAL_CONTACTO";
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseVM result = new ResponseVM();

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_STI_ACCION", OracleDbType.Varchar2, data.P_TipOper, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NRECOWNER", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDDOC_TYPE_CL", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SIDDOC_CL", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDCONT", OracleDbType.Varchar2, data.P_NIDCONT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTIPCONT", OracleDbType.Varchar2, data.P_NTIPCONT == "0" ? "99" : data.P_NTIPCONT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DEFFECDATE", OracleDbType.Varchar2, data.P_DEFFECDATE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDDOC_TYPE", OracleDbType.Varchar2, data.P_NIDDOC_TYPE == "0" ? null : data.P_NIDDOC_TYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, data.P_SIDDOC == "" ? null : data.P_SIDDOC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SNOMBRES", OracleDbType.Varchar2, data.P_SNOMBRES, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SAPEPAT", OracleDbType.Varchar2, data.P_SAPEPAT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SAPEMAT", OracleDbType.Varchar2, data.P_SAPEMAT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SE_MAIL", OracleDbType.Varchar2, data.P_SE_MAIL, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SPHONE", OracleDbType.Varchar2, data.P_SPHONE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SNAMEAREA", OracleDbType.Varchar2, data.P_SNAMEAREA, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SNAMEPOSITION", OracleDbType.Varchar2, data.P_SNAMEPOSITION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Varchar2, data.P_NUSERCODE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SORIGEN", OracleDbType.Varchar2, data.P_SORIGEN, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SPHONE1", OracleDbType.Varchar2, null, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NEXTENS", OracleDbType.Varchar2, null, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int64, result.P_NCODE, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, result.P_SMESSAGE, ParameterDirection.Output);

                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;

                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);

                this.ExecuteByStoredProcedureVT(sPackageName, parameter);
                result.P_NCODE = Convert.ToInt64(P_NCODE.Value.ToString());
                result.P_SMESSAGE = P_SMESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                result.P_NCODE = 1;
                result.P_SMESSAGE = ex.ToString();
                LogControl.save("ValContact", ex.ToString(), "3");
            }

            return result;
        }

        public async Task<PaymentVM> getFormaPago(PaymentModel model)
        {
            var wsFormaPago = new WsFormaPago();
            var reqToken = new getTokenBM();
            var response = new PaymentVM();
            var arrayPE = new List<int>();
            var arrayPE1 = new List<int>();
            string url = String.Format(ELog.obtainConfig("pagosBase"), ELog.obtainConfig("pagosCanal"));
            string data = model.Canal != null ? model.Canal + "/2" : model.codigoCanal + "/2";
            string urltoken = model.Canal != null ? String.Format(ELog.obtainConfig("pagosBase"), ELog.obtainConfig("visaToken")) : String.Format(ELog.obtainConfig("pagosBaseNiubiz"), ELog.obtainConfig("niubizToken"));
            string urlPE = String.Format(ELog.obtainConfig("pagosBase"), ELog.obtainConfig("pagoEfectivo"));
            string urlPE1 = String.Format(ELog.obtainConfig("pagosBase"), ELog.obtainConfig("pagoEfectivo1"));
            string urlKushki = String.Format(ELog.obtainConfig("pagosBaseKushki"), ELog.obtainConfig("pagoKushki"));

            var lNotaCredito = new string[] { ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch"), ELog.obtainConfig("vidaLeyBranch") };


            try
            {
                model.CodigoComercio = getCodigoComercio(model); // ELog.obtainConfig("comercio" + model.Ramo);
                #region Comentado por problemas en prod
                /*var json = await WebServiceUtil.GetServicio(url, data, model.token);

                if (!String.IsNullOrEmpty(json))
                {
                    wsFormaPago = JsonConvert.DeserializeObject<WsFormaPago>(json, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });*/
                #endregion
                wsFormaPago = null;
                wsFormaPago = getTypeChannel(model.Canal != null ? model.Canal : model.codigoCanal)[0];

                if (wsFormaPago != null)
                {
                    var jsonPE = await WebServiceUtil.invocarServicio(JsonConvert.SerializeObject("{}", new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), urlPE, model.token, null, null);
                    var jsonPE1 = await WebServiceUtil.invocarServicio(JsonConvert.SerializeObject("{}", new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), urlPE1, model.token, null, null);
                    var jsonKushki = await WebServiceUtil.invocarServicioKushki(JsonConvert.SerializeObject("{}", new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), urlKushki, model.token, null, null);
                    arrayPE = JsonConvert.DeserializeObject<List<int>>(jsonPE, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    arrayPE1 = JsonConvert.DeserializeObject<List<int>>(jsonPE1, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    
                    if (wsFormaPago.bvisa == 1)
                    {
                        response.flagvisa = Convert.ToInt32(model.Ramo) == 77 ? false : true;
                    }
                    else
                    {
                        response.flagvisa = false;
                    }

                    if (wsFormaPago.bpagoefectivo == 1)
                    {
                        if(Convert.ToInt32(model.Ramo) == 77)
                        {
                            if(jsonKushki == "success")
                            {
                                response.flagpagoefectivo = true;
                            }
                        }
                        else
                        {
                            if (arrayPE != null)
                            {
                                if (arrayPE.Contains(Convert.ToInt32(model.Ramo)))
                                {
                                    response.flagpagoefectivo = true;
                                }
                                else
                                {
                                    response.flagpagoefectivo = false;
                                }
                            }
                        }
                    }
                    else
                    {
                        response.flagpagoefectivo = false;
                    }

                    /*Logica de pago Nota decredito - AVS*/
                    if (lNotaCredito.Contains(model.Ramo))
                    {
                        if (wsFormaPago.ncredito == 1)
                        {
                            response.flagcredito = true;

                        }
                        else
                        {
                            response.flagcredito = false;
                        }

                        if (wsFormaPago.bpagoefectivocredito == 1)
                        {
                            if (arrayPE1.Contains(Convert.ToInt32(model.Ramo)))
                            {
                                response.flagpagoefectivocredito = true;
                            }
                            else
                            {
                                response.flagpagoefectivocredito = false;
                            }
                        }
                        else
                        {
                            response.flagpagoefectivocredito = false;
                        }
                    }
                    else
                    {
                        response.flagcredito = false;
                        response.flagpagoefectivocredito = false;
                    }
                }
                else
                {
                    response.flagvisa = false;
                    response.flagpagoefectivo = false;
                    //response.flagcredDirect = false;
                    response.flagpagoefectivocredito = false;
                    response.flagcredito = false;
                }


                if (model.tipoTransac == "0")
                {
                    response.flagOmitir = true;
                }

                if (model.emisionDirecta == "S")
                {
                    response.flagdirecto = true;
                    //response.flagcredDirect = true;
                    response.flagvoucher = false;
                }
                else
                {
                    if (model.emisionDirecta == "V")
                    {
                        response.flagdirecto = false;
                        response.flagvoucher = true;
                        //response.flagcredDirect = true;

                        if (model.clienteDeuda == "1")  // 0: sin deuda | 1: bloqueado | 2: privilegio
                        {
                            response.flagvisa = false;
                            //response.flagcredDirect = false;
                            response.flagpagoefectivo = false;
                            response.flagpagoefectivocredito = false;
                            response.flagcredito = false;
                        }
                    }
                    else
                    {
                        response.flagdirecto = false;
                        //response.flagcredDirect = true;
                        response.flagvoucher = false;
                    }
                }

                if (!string.IsNullOrEmpty(model.CodigoComercio))
                {
                    string obj = JsonConvert.SerializeObject(model, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    string datenvio = "";
                    if (model.Canal != null)
                    {
                        byte[] objBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(obj);

                        var database64 = Convert.ToBase64String(objBytes);
                        reqToken.Data = database64;
                        datenvio = JsonConvert.SerializeObject(reqToken, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    }
                    else
                    {
                        //reqToken.Data = obj;
                        datenvio = obj;
                    }

                    var json2 = await WebServiceUtil.invocarServicio(datenvio, urltoken, model.token, null, null);   //TOKEN PARA ENVIO A VISA PARA BOTON
                    if (!String.IsNullOrEmpty(json2))
                    {
                        try
                        {
                            if (model.Canal != null)
                            {
                                response.ObjVisa = JsonConvert.DeserializeObject<TokenVM>(json2, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                                response.ObjVisa.action = ELog.obtainConfig("action" + model.Ramo);
                                response.ObjVisa.timeoutUrl = ELog.obtainConfig("timeoutUrl" + model.Ramo + model.tipoTransac);
                                response.ObjVisa.merchantid = model.CodigoComercio;
                                response.ObjVisa.flow = ELog.obtainConfig("flow" + model.Ramo);
                                response.ObjVisa.quotationNumber = model.nroCotizacion;
                            }
                            else
                            {
                                response.objNiubiz = JsonConvert.DeserializeObject<TokenNiubizVM>(json2, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                                response.objNiubiz.action = ELog.obtainConfig("action" + model.idRamo);
                                response.objNiubiz.timeoutUrl = ELog.obtainConfig("timeoutUrl" + model.idRamo + model.tipoTransac);
                                response.objNiubiz.merchantid = model.CodigoComercio;
                                response.objNiubiz.flow = ELog.obtainConfig("flow" + model.idRamo);
                                response.objNiubiz.quotationNumber = model.nroCotizacion;
                            }


                        }
                        catch (Exception ex)
                        {
                            response.ObjVisa = null;
                            response.flagvisa = false;
                        }
                    }
                    else
                    {
                        response.flagvisa = false;
                    }
                }
                else
                {
                    response.ObjVisa = null;
                    response.flagvisa = false;
                }

                response.flagdirecto = getPagoDirecto(Convert.ToInt32(model.nroCotizacion)) == false ? response.flagdirecto : true; //AVS - RENTAS
                response.cod_error = 0;
                response.mensaje = "Exitoso";
                response.urlContinuar = ELog.obtainConfig("urlContinuar" + (model.Ramo != null ? model.Ramo : model.idRamo));
                response.urlVolver = ELog.obtainConfig("urlVolver" + (model.Ramo != null ? model.Ramo : model.idRamo) + model.tipoTransac);

            }
            catch (Exception ex)
            {
                response.cod_error = 1;
                response.mensaje = ex.ToString();
            }

            return await Task.FromResult(response);
        }

        public List<WsFormaPago> getTypeChannel(string canal)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<WsFormaPago> response = new List<WsFormaPago>();
            string storedProcedureName = ProcedureName.pkg_CodeChannel + ".SPS_GET_PAYTYPECHANNEL";
            try
            {
                parameter.Add(new OracleParameter("P_CODE", OracleDbType.Int32, canal, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SETTINGS", OracleDbType.Varchar2, "2", ParameterDirection.Input));
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, response, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                while (odr.Read())
                {
                    WsFormaPago item = new WsFormaPago();
                    item.scodchannel = odr["SCODCHANNEL"] == DBNull.Value ? 0 : Convert.ToInt32(odr["SCODCHANNEL"]);
                    item.bvisa = odr["BVISA"] == DBNull.Value ? 0 : Convert.ToInt32(odr["BVISA"]);
                    item.bpagoefectivo = odr["BPAGOEFECTIVO"] == DBNull.Value ? 0 : Convert.ToInt32(odr["BPAGOEFECTIVO"]);
                    item.bvoucher = odr["BVOUCHER"] == DBNull.Value ? 0 : Convert.ToInt32(odr["BVOUCHER"]);
                    item.bdelivery = Convert.ToInt32(odr["BDELIVERY"]);
                    item.bfactura = Convert.ToInt32(odr["BFACTURA"]);
                    item.btiporeporte = Convert.ToInt32(odr["BTIPOREPORTE"]);
                    item.nsoatgratis = Convert.ToInt32(odr["NSOATGRATIS"]);
                    item.navisomailfact = Convert.ToInt32(odr["NAVISOMAILFACT"]);
                    item.ncredito = Convert.ToInt32(odr["NCREDITO"]);
                    item.bpagoefectivocredito = odr["BPAGOEFECTIVO_CRED"] == DBNull.Value ? 0 : Convert.ToInt32(odr["BPAGOEFECTIVO_CRED"]);

                    response.Add(item);
                }
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("getTypeChannel", ex.ToString(), "3");
            }

            return response;
        }

        public bool getPagoDirecto(int nidCotizacion)
        {
            bool flag_directo = false;
            List<OracleParameter> parameters = new List<OracleParameter>();
            string procedure = ProcedureName.sp_pagoDirectoAPVG;

            try
            {
                parameters.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int64, nidCotizacion, ParameterDirection.Input));

                OracleParameter P_FLAG_DIRECTO = new OracleParameter("P_FLAG_DIRECTO", OracleDbType.Varchar2, null, ParameterDirection.Output);
                P_FLAG_DIRECTO.Size = 20;
                parameters.Add(P_FLAG_DIRECTO);
                this.ExecuteByStoredProcedureVT(procedure, parameters);
                flag_directo = Convert.ToBoolean(P_FLAG_DIRECTO.Value.ToString());
            }
            catch (Exception)
            {
                flag_directo = false;
            }
            return flag_directo;
        }


        /*
        public List<BranchVM> GetBranchList()
        {
            var sPackageName = ProcedureName.pkg_CargaMasiva + ".REA_LIST_BRANCH";
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<BranchVM> ListBranch = new List<BranchVM>();

            try
            {
                //OUTPUT
                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(sPackageName, parameter))
                {
                    ListBranch = dr.ReadRowsList<BranchVM>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ListBranch;
        }
        */

        public string GetUserProfile(int usercode, int product)
        {
            string perfil = "";
            List<OracleParameter> parameters = new List<OracleParameter>();
            string procedure = ProcedureName.sp_PerfilUsuario;

            try
            {
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int64, product, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int64, usercode, ParameterDirection.Input));

                OracleParameter P_NPERFIL = new OracleParameter("P_NPERFIL", OracleDbType.Int32, null, ParameterDirection.Output);
                P_NPERFIL.Size = 20;
                parameters.Add(P_NPERFIL);
                this.ExecuteByStoredProcedureVT(procedure, parameters);
                perfil = P_NPERFIL.Value.ToString();
            }
            catch (Exception)
            {
                perfil = "";
            }
            return perfil;
        }

        public string getCodigoComercio(PaymentModel data)
        {
            var genericList = new string[] { ELog.obtainConfig("accidentesBranch"), ELog.obtainConfig("vidaGrupoBranch") };

            List<OracleParameter> parameter = new List<OracleParameter>();
            var response = string.Empty;
            var comerList = new List<ComercioVM>();
            var numRamo = data.Ramo != null ? data.Ramo : data.idRamo;
            var numproducto = genericList.Contains(data.Ramo) ? "0" : data.Producto != null ? data.Producto : data.idProducto;      //Cambio Pago Visa - R.P.

            string storedProcedureName = ProcedureName.pkg_Ecommerce + ".SPS_OBTENER_CODIGO_COMERCIO_GENERICO"; //Cambio Pago Visa - R.P.
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, numRamo, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int32, numproducto, ParameterDirection.Input));    //Cambio Pago Visa - R.P.

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        var item = new ComercioVM();

                        item.CodigoComercio = odr["CodigoComercio"] == DBNull.Value ? string.Empty : odr["CodigoComercio"].ToString();
                        item.IdMoneda = odr["IdMoneda"] == DBNull.Value ? string.Empty : odr["IdMoneda"].ToString();
                        item.TipoCanal = odr["TipoCanal"] == DBNull.Value ? string.Empty : odr["TipoCanal"].ToString();
                        item.IdCategoria = odr["IdCategoria"] == DBNull.Value ? string.Empty : odr["IdCategoria"].ToString();
                        comerList.Add(item);
                    }
                }

                odr.Close();

                response = comerList.Count > 0 ? comerList.FirstOrDefault(x => x.IdMoneda == (data.codMoneda != null ? data.codMoneda : data.idMoneda) && x.TipoCanal.ToLower() == ELog.obtainConfig("Pagoweb" + numRamo) && x.IdCategoria == "1")?.CodigoComercio : null;
                //response = comerList.Count > 0 ? comerList.FirstOrDefault(x => x.IdMoneda == data.codMoneda && x.TipoCanal.ToLower() == "web" && x.IdCategoria == "1")?.CodigoComercio : null;
            }
            catch (Exception ex)
            {
                response = null;
            }

            return response;
        }

        //R.P.

        public List<BrokDepVM> getBrokerDep(int nbranch, int nproduct)
        {
            List<BrokDepVM> depList = new List<BrokDepVM>();
            List<OracleParameter> parameter = new List<OracleParameter>();

            try
            {
                parameter.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int64, nbranch, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Int64, nproduct, ParameterDirection.Input));

                parameter.Add(new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)this.ExecuteByStoredProcedureVT(ProcedureName.pkg_Poliza + ".SEL_DEP_BRK", parameter))
                {
                    depList = dr.ReadRowsList<BrokDepVM>();
                    ELog.CloseConnection(dr);
                }
            }
            catch (Exception ex)
            {
                LogControl.save("getBrokerDep", ex.ToString(), "3");
            }

            return depList;
        }


        //AVS PRY NC 24012023
        public List<NotaCreditoVM> GetNotaCreditoList(string P_CONTRATANTE, string P_REC_NC, int P_TIPO, int P_TIPO_MONEDA)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<NotaCreditoVM> response = new List<NotaCreditoVM>();
            CultureInfo provider = CultureInfo.InvariantCulture;
            string storedProcedureName = ProcedureName.pkg_paymentNC + ".LIST_NC_DOCCLIENTES";
            try
            {
                parameter.Add(new OracleParameter("P_CONTRATANTE", OracleDbType.Varchar2, P_CONTRATANTE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_REC_NC", OracleDbType.Varchar2, P_REC_NC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TIPO", OracleDbType.Int64, P_TIPO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_TIPO_MONEDA", OracleDbType.Int32, P_TIPO_MONEDA, ParameterDirection.Input));
                //OUTPUT
                OracleParameter C_LISTA = new OracleParameter("C_LISTA", OracleDbType.RefCursor, response, ParameterDirection.Output);
                parameter.Add(C_LISTA);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                while (odr.Read())
                {
                    NotaCreditoVM item = new NotaCreditoVM();
                    item.NC = odr["NC"] == DBNull.Value ? string.Empty : odr["NC"].ToString();
                    item.DOCUMENTO = Convert.ToInt64(odr["DOCUMENTO"]);
                    item.DOCUMENTO_NC = Convert.ToInt64(odr["DOCUMENTO_NC"]);
                    item.CONTRATANTE = Convert.ToString(odr["CONTRATANTE"]);
                    item.INI_VIGENCIA = Convert.ToDateTime(odr["INI_VIGENCIA"]);
                    item.FIN_VIGENCIA = Convert.ToDateTime(odr["FIN_VIGENCIA"]);
                    item.MONEDA = Convert.ToString(odr["MONEDA"]);
                    item.MONTO = Convert.ToDouble(odr["MONTO"]);
                    item.NRECEIPT = Convert.ToInt64(odr["NRECEIPT"]);
                    item.DISSUEDAT = Convert.ToDateTime(odr["DISSUEDAT"]);
                    item.SEL_AUTOMATICA = Convert.ToInt64(odr["SEL_AUTOMATICA"]);
                    item.TIPO = Convert.ToInt64(odr["TIPO"]);
                    item.FORMA_PAGO = Convert.ToString(odr["FORMA_PAGO"]);
                    item.MARCADO = Convert.ToInt64(odr["MARCADO"]);
                    item.DES_PROD = Convert.ToString(odr["DES_PROD"]);
                    item.DES_RAMO = Convert.ToString(odr["DES_RAMO"]);

                    response.Add(item);
                }
                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("GetNotaCreditoList", ex.ToString(), "3");
            }
            return response;
        }

        //AVS PRY NC 24012023
        public ErrorCode NCParcialVISA(EntidadParcialVisa data)
        {
            var errorCode = new ErrorCode();
            DbConnection DataConnection = ConnectionGet(enuTypeDataBase.OracleVTime);
            DbTransaction trx = null;

            try
            {
                DataConnection.Open();
                trx = DataConnection.BeginTransaction();

                if (errorCode.P_COD_ERR == 0)
                {

                    foreach (var item in data.ListainsertNCVISA)
                    {
                        if (errorCode.P_COD_ERR == 0)
                        {
                            errorCode = NCParcialVISA_VL(item, DataConnection, trx);
                        }

                    }

                    trx.Commit();
                    trx.Dispose();
                    DataConnection.Close();
                }
                else
                {
                    if (trx.Connection != null)
                    {
                        trx.Rollback();
                        trx.Dispose();
                    }
                    DataConnection.Close();
                    DataConnection.Dispose();
                }

            }
            catch (Exception ex)
            {
                errorCode.P_COD_ERR = 1;
                errorCode.P_MESSAGE = ex.ToString();
                if (trx.Connection != null) trx.Rollback();
                if (trx.Connection != null) trx.Dispose();
                DataConnection.Close();
                DataConnection.Dispose();
                LogControl.save("NCParcialVISA", ex.ToString(), "3");
            }

            return errorCode;
        }

        //AVS PRY NC 24012023
        public ErrorCode NCParcialVISA_VL(itemNCVisa data, DbConnection connection, DbTransaction trx)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            var errorCode = new ErrorCode();
            var sPackageName = ProcedureName.pkg_paymentNC + ".NC_PARCIALVISA";

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NIDPAYMENT", OracleDbType.Int32, data.P_NIDPAYMENT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCURRENCY", OracleDbType.Varchar2, data.P_NCURRENCY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDPAYTYPE", OracleDbType.Int32, data.P_NIDPAYTYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SOPERATION_NUMBER", OracleDbType.Int64, data.P_SOPERATION_NUMBER, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NAMOUNT", OracleDbType.Double, data.P_NAMOUNT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int64, data.P_NID_COTIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_CODIGOCANAL", OracleDbType.Int64, data.P_CODIGOCANAL, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDUSER", OracleDbType.Int64, data.P_NIDUSER, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_ESTADO", OracleDbType.Int64, data.P_ESTADO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Int64, data.P_NTYPE_TRANSAC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STRANSAC", OracleDbType.Varchar2, data.P_STRANSAC, ParameterDirection.Input));


                //OUTPUT
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, 9000, errorCode.P_COD_ERR, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, 900, errorCode.P_MESSAGE, ParameterDirection.Output);

                parameter.Add(P_COD_ERR);
                parameter.Add(P_MESSAGE);

                this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, connection, trx);

                errorCode.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                errorCode.P_MESSAGE = P_MESSAGE.Value.ToString();

            }
            catch (Exception ex)
            {
                errorCode.P_COD_ERR = 1;
                errorCode.P_MESSAGE = ex.ToString();
                LogControl.save("NCParcialVISA_VL", ex.ToString(), "3");
            }

            return errorCode;
        }

        public AutoNidPay AutItemNidPay(DbConnection connection, DbTransaction trx)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            var nidpay = new AutoNidPay();
            var sPackageName = ProcedureName.pkg_paymentNC + ".AUTO_NIDPAY";

            try
            {
                //OUTPUT
                OracleParameter P_NIDPAYROLL = new OracleParameter("P_NIDPAYROLL", OracleDbType.Int32, 9000, nidpay.P_NIDPAYROLL, ParameterDirection.Output);
                OracleParameter P_NIDPAYROLLDETAIL = new OracleParameter("P_NIDPAYROLLDETAIL", OracleDbType.Varchar2, 900, nidpay.P_NIDPAYROLLDETAIL, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, 9000, nidpay.P_COD_ERR, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, 900, nidpay.P_MESSAGE, ParameterDirection.Output);

                parameter.Add(P_NIDPAYROLL);
                parameter.Add(P_NIDPAYROLLDETAIL);
                parameter.Add(P_COD_ERR);
                parameter.Add(P_MESSAGE);

                this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, connection, trx);

                nidpay.P_NIDPAYROLL = Convert.ToInt32(P_NIDPAYROLL.Value.ToString());
                nidpay.P_NIDPAYROLLDETAIL = Convert.ToInt32(P_NIDPAYROLLDETAIL.Value.ToString());
                nidpay.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                nidpay.P_MESSAGE = P_MESSAGE.Value.ToString();

            }
            catch (Exception ex)
            {
                nidpay.P_COD_ERR = 1;
                nidpay.P_MESSAGE = ex.ToString();
                LogControl.save("AutItemNidPay", ex.ToString(), "3");
            }
            return nidpay;
        }

        //AVS PRY NC 24012023
        public VPolicy.SalidadPolizaEmit insertNCTemp(List<NotaCreditoModel> data)
        {
            var response = new VPolicy.SalidadPolizaEmit();
            DbConnection DataConnection = ConnectionGet(enuTypeDataBase.OracleVTime);
            DbTransaction trx = null;

            try
            {
                DataConnection.Open();
                trx = DataConnection.BeginTransaction();
                var autonid = AutItemNidPay(DataConnection, trx);

                foreach (var item in data)
                {
                    response = insertNCTEMP(item, DataConnection, trx, autonid);

                    if (response.P_COD_ERR == 1)
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = ex.ToString();
                LogControl.save("insertNCTemp", ex.ToString(), "3");
            }
            finally
            {
                if (response.P_COD_ERR == 0)
                {
                    trx.Commit();
                }
                else
                {
                    if (trx.Connection != null) trx.Rollback();
                }

                if (trx.Connection != null) trx.Dispose();
                ELog.CloseConnection(DataConnection);
            }

            return response;
        }

        //AVS PRY NC 24012023
        public VPolicy.SalidadPolizaEmit insertNCTEMP(NotaCreditoModel data, DbConnection connection, DbTransaction trx, AutoNidPay nidpay)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            var response = new VPolicy.SalidadPolizaEmit();
            var sPackageName = ProcedureName.pkg_paymentNC + ".SP_INS_PAGONC_TEMP";

            try
            {
                parameter.Add(new OracleParameter("P_NIDPAYROLLDETAIL", OracleDbType.Int64, nidpay.P_NIDPAYROLLDETAIL, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDPAYROLL", OracleDbType.Int64, nidpay.P_NIDPAYROLL, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int64, data.ncotizacion, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NRECEIPT_NC", OracleDbType.Int64, data.documento_nc, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_MONEDA", OracleDbType.Int64, data.moneda, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NAMOUNT_NC", OracleDbType.Double, data.monto, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCODCHANNEL", OracleDbType.Int64, data.canal, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDPAYTYPE", OracleDbType.Int64, data.codTipoPago, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDUSER", OracleDbType.Int64, data.codUsuario, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_ESTADO", OracleDbType.Int64, data.estado, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPETRANSAC", OracleDbType.Int64, data.codTransac, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_DESTYPETRANSAC", OracleDbType.Varchar2, data.desTransac, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, 9000, response.P_COD_ERR, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, 900, response.P_MESSAGE, ParameterDirection.Output);

                parameter.Add(P_COD_ERR);
                parameter.Add(P_MESSAGE);

                this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, connection, trx);

                response.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                response.P_MESSAGE = P_MESSAGE.Value.ToString();
            }
            catch (Exception ex)
            {
                response.P_COD_ERR = 1;
                response.P_MESSAGE = ex.ToString();
                LogControl.save("insertNCTEMP", ex.ToString(), "3");
            }

            return response;
        }

        //AVS PRY NC 24012023
        public ErrorCode NCParcialPF(EntidadParcialPF data)
        {
            var errorCode = new ErrorCode();
            DbConnection DataConnection = ConnectionGet(enuTypeDataBase.OracleVTime);
            DbTransaction trx = null;

            try
            {
                DataConnection.Open();
                trx = DataConnection.BeginTransaction();

                if (errorCode.P_COD_ERR == 0)
                {

                    foreach (var item in data.ListainsertNCPF)
                    {
                        if (errorCode.P_COD_ERR == 0)
                        {
                            errorCode = NCParcialPF_VL(item, DataConnection, trx);
                        }

                    }

                    trx.Commit();
                    trx.Dispose();
                    DataConnection.Close();
                }
                else
                {
                    if (trx.Connection != null)
                    {
                        trx.Rollback();
                        trx.Dispose();
                    }
                    DataConnection.Close();
                    DataConnection.Dispose();
                }

            }
            catch (Exception ex)
            {
                errorCode.P_COD_ERR = 1;
                errorCode.P_MESSAGE = ex.ToString();
                if (trx.Connection != null) trx.Rollback();
                if (trx.Connection != null) trx.Dispose();
                DataConnection.Close();
                DataConnection.Dispose();
                LogControl.save("NCParcialPF", ex.ToString(), "3");
            }

            return errorCode;
        }

        //AVS PRY NC 24012023
        public ErrorCode NCParcialPF_VL(itemNCPF data, DbConnection connection, DbTransaction trx)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            var errorCode = new ErrorCode();
            var sPackageName = ProcedureName.pkg_paymentNC + ".PC_PARCIALPAGOEFECTIVO";

            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NIDPAYMENT", OracleDbType.Int32, data.P_NIDPAYMENT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NCURRENCY", OracleDbType.Varchar2, data.P_NCURRENCY, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDPAYTYPE", OracleDbType.Int32, data.P_NIDPAYTYPE, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SOPERATION_NUMBER", OracleDbType.Int64, data.P_SOPERATION_NUMBER, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NAMOUNT", OracleDbType.Double, data.P_NAMOUNT, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int64, data.P_NID_COTIZACION, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_CODIGOCANAL", OracleDbType.Int64, data.P_CODIGOCANAL, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDUSER", OracleDbType.Int64, data.P_NIDUSER, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_ESTADO", OracleDbType.Int64, data.P_ESTADO, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NTYPE_TRANSAC", OracleDbType.Int64, data.P_NTYPE_TRANSAC, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_STRANSAC", OracleDbType.Varchar2, data.P_STRANSAC, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, 9000, errorCode.P_COD_ERR, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, 900, errorCode.P_MESSAGE, ParameterDirection.Output);

                parameter.Add(P_COD_ERR);
                parameter.Add(P_MESSAGE);

                this.ExecuteByStoredProcedureVT_TRX(sPackageName, parameter, connection, trx);

                errorCode.P_COD_ERR = Convert.ToInt32(P_COD_ERR.Value.ToString());
                errorCode.P_MESSAGE = P_MESSAGE.Value.ToString();

            }
            catch (Exception ex)
            {
                errorCode.P_COD_ERR = 1;
                errorCode.P_MESSAGE = ex.ToString();
                LogControl.save("NCParcialPF_VL", ex.ToString(), "3");
            }

            return errorCode;
        }

    }
}