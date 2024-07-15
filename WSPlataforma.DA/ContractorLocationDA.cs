using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WSPlataforma.Entities.BindingModel;
using WSPlataforma.Entities.ContractorLocationModel.ViewModel;
using WSPlataforma.Entities.PolicyModel.BindingModel;
using WSPlataforma.Entities.ViewModel;
using WSPlataforma.Util;

namespace WSPlataforma.DA
{
    public class ContractorLocationDA : ConnectionBase
    {
        public ResponseVM GetContractorLocationList(string contractorId, Int32 limitPerPage, Int32 pageNumber)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseVM resultPackage = new ResponseVM();
            List<ContractorLocationForTableVM> lista = new List<ContractorLocationForTableVM>();
            string storedProcedureName = ProcedureName.pkg_SedesCliente + ".SEL_CLIENTLOCATIONLIST";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_SCLIENT", OracleDbType.Char, contractorId, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLIMITPERPAGE", OracleDbType.Int32, limitPerPage, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPAGENUM", OracleDbType.Varchar2, pageNumber, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, resultPackage.GENERICLIST, ParameterDirection.Output);
                OracleParameter NTOTALROWS = new OracleParameter("NTOTALROWS", OracleDbType.Int32, resultPackage.P_SMESSAGE, ParameterDirection.Output);

                NTOTALROWS.Size = 4000;

                parameter.Add(C_TABLE);
                parameter.Add(NTOTALROWS);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                while (odr.Read())
                {
                    ContractorLocationForTableVM item = new ContractorLocationForTableVM();
                    item.Id = Convert.ToInt32(odr["NIDCLIENTLOCATION"]);
                    item.Type = odr["CLIENTLOCATIONTTYPE"].ToString();
                    item.Description = odr["SDESCRIPTION"].ToString();
                    item.IdActivity = odr["NCOD_CIUU"].ToString();
                    item.IdTechnical = odr["NTECHNICALACTIVITY"].ToString();
                    item.Activity = odr["SDESC_ACTIVIDAD"].ToString();
                    item.EconomicActivity = odr["SDESC_CIUU"].ToString();
                    item.Delimiter = odr["SDELIMITER"].ToString();
                    item.Departament = odr["NPROVINCE"].ToString();
                    item.Province = odr["NLOCAL"].ToString();
                    item.Municipality = odr["NMUNICIPALITY"].ToString();

                    item.DistrictName = odr["NOMBRE_DISTRITO"].ToString();
                    item.ProvinceName = odr["NOMBRE_PROVINCIA"].ToString();
                    item.DepartmentName = odr["NOMBRE_DEPARTAMENTO"].ToString();
                    item.Address = odr["DIRECCION"].ToString();

                    if (odr["SSTATE"].ToString().Trim() == "1") item.State = "Activo";
                    else if (odr["SSTATE"].ToString().Trim() == "2") item.State = "Inactivo";

                    lista.Add(item);
                }

                ELog.CloseConnection(odr);

                resultPackage.GENERICLIST = lista;
                resultPackage.P_NTOTALROWS = Convert.ToInt32(NTOTALROWS.Value.ToString());
            }
            catch (Exception ex)
            {
                LogControl.save("GetContractorLocationList", ex.ToString(), "3");
            }

            return resultPackage;
        }

        public ContractorLocationVM GetContractorLocation(Int32 contractorLocationId)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            ContractorLocationVM contractorLocation = null;
            string storedProcedureName = ProcedureName.pkg_SedesCliente + ".SEL_CLIENTLOCATION";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NIDCLIENTLOCATION", OracleDbType.Int32, contractorLocationId, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, contractorLocation, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                if (odr.HasRows)
                {
                    odr.Read();
                    contractorLocation = new ContractorLocationVM();

                    contractorLocation.Id = Convert.ToInt32(odr["NIDCLIENTLOCATION"]);
                    contractorLocation.ContractorId = odr["SCLIENT"].ToString();
                    contractorLocation.Activity = odr["SDESC_ACTIVIDAD"].ToString();
                    contractorLocation.EconomicActivity = odr["SDESC_CIUU"].ToString();
                    contractorLocation.TypeId = odr["SLOCATIONTYPE"].ToString();
                    contractorLocation.Description = odr["SDESCRIPTION"].ToString();
                    contractorLocation.DepartmentId = odr["NDEPARTMENT"].ToString();
                    contractorLocation.ProvinceId = odr["NPROVINCE"].ToString();
                    contractorLocation.DistrictId = odr["NDISTRICT"].ToString();
                    contractorLocation.Address = odr["SSTREET"].ToString();
                    contractorLocation.State = odr["SSTATE"].ToString();
                    contractorLocation.EconomicActivityId = odr["SECONOMICACTIVITY"].ToString();
                    contractorLocation.TechnicalActivityId = odr["TECHNICALACTIVITY"].ToString();
                }
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                LogControl.save("GetContractorLocation", ex.ToString(), "3");
            }

            return contractorLocation;
        }

        public ResponseVM GetContractorLocationContactList(Int32 contractorLocationId, Int32 limitPerPage, Int32 pageNumber)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseVM resultPackage = new ResponseVM();
            List<ContractorLocationContactForTableVM> lista = new List<ContractorLocationContactForTableVM>();
            string storedProcedureName = ProcedureName.pkg_SedesCliente + ".SEL_CLIENTLOCATIONCONTACTLIST";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("NIDCLIENTLOCATIONCONTACT", OracleDbType.Int32, contractorLocationId, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NLIMITPERPAGE", OracleDbType.Int32, limitPerPage, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPAGENUM", OracleDbType.Varchar2, pageNumber, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, resultPackage.GENERICLIST, ParameterDirection.Output);
                OracleParameter NTOTALROWS = new OracleParameter("NTOTALROWS", OracleDbType.Int32, resultPackage.P_NTOTALROWS, ParameterDirection.Output);

                NTOTALROWS.Size = 4000;
                parameter.Add(C_TABLE);
                parameter.Add(NTOTALROWS);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                while (odr.Read())
                {
                    ContractorLocationContactForTableVM clientLocationContact = new ContractorLocationContactForTableVM();
                    clientLocationContact.Id = Convert.ToInt32(odr["NIDCLIENTLOCATIONCONTACT"]);
                    clientLocationContact.FullName = odr["SFULLNAME"].ToString();
                    if (odr["SSTATE"].ToString().Trim() == "1") clientLocationContact.State = "Activo";
                    else if (odr["SSTATE"].ToString().Trim() == "2") clientLocationContact.State = "No Activo";
                    clientLocationContact.Email = odr["SEMAIL"].ToString();
                    clientLocationContact.PhoneNumber = odr["SPHONE"].ToString();
                    lista.Add(clientLocationContact);
                }

                ELog.CloseConnection(odr);

                resultPackage.P_NTOTALROWS = Convert.ToInt32(NTOTALROWS.Value.ToString());
                resultPackage.GENERICLIST = lista;
            }
            catch (Exception ex)
            {
                LogControl.save("GetContractorLocationContactList", ex.ToString(), "3");
            }

            return resultPackage;
        }
        public ContractorLocationContactVM GetContractorLocationContact(Int32 contractorLocationContactId)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            ContractorLocationContactVM contact = null;
            string storedProcedureName = ProcedureName.pkg_SedesCliente + ".SEL_CLIENTLOCATIONCONTACT";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NIDCLIENTLOCATIONCONTACT", OracleDbType.Int32, contractorLocationContactId, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, contact, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                odr.Read();
                contact = new ContractorLocationContactVM();

                contact.Id = Convert.ToInt32(odr["NIDCLIENTLOCATIONCONTACT"]);
                contact.FullName = odr["SFULLNAME"].ToString();
                contact.PhoneNumber = odr["SPHONE"].ToString();
                contact.Email = odr["SEMAIL"].ToString();
                contact.StateId = odr["SSTATE"].ToString();
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                LogControl.save("GetContractorLocationContact", ex.ToString(), "3");
            }

            return contact;
        }
        public List<ContractorLocationTypeVM> GetContractorLocationTypeList()
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            List<ContractorLocationTypeVM> lista = new List<ContractorLocationTypeVM>();
            string storedProcedureName = ProcedureName.pkg_SedesCliente + ".SEL_CLIENTLOCATIONTYPELIST";
            try
            {
                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, lista, ParameterDirection.Output);

                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                while (odr.Read())
                {
                    ContractorLocationTypeVM clientLocationType = new ContractorLocationTypeVM();
                    clientLocationType.Id = odr["sidclientlocationtype"].ToString();
                    clientLocationType.Name = odr["sname"].ToString();
                    lista.Add(clientLocationType);
                }

                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                LogControl.save("GetContractorLocationTypeList", ex.ToString(), "3");
            }

            return lista;
        }
        public ContractorVM GetContractor(string contractorId)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            ContractorVM contractor = null;
            string storedProcedureName = ProcedureName.pkg_SedesCliente + ".SEL_CONTRACTOR_BYCODE";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_SCLIENT", OracleDbType.Char, contractorId, ParameterDirection.Input));

                //OUTPUT
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, contractor, ParameterDirection.Output);
                parameter.Add(C_TABLE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                odr.Read();
                contractor = new ContractorVM();

                contractor.Id = odr["SCLIENT"].ToString();
                contractor.PersonalName = odr["SPERSONNAME"].ToString();
                contractor.LegalName = odr["SLEGALNAME"].ToString();
                contractor.Address = odr["SSTREET"].ToString();
                contractor.District = odr["SDISTRICT"].ToString();
                contractor.Province = odr["SPROVINCE"].ToString();
                contractor.Department = odr["SDEPARTAMENT"].ToString();
                contractor.Email = odr["SEMAIL"].ToString();
                contractor.PhoneNumber = odr["SPHONE"].ToString();
                ELog.CloseConnection(odr);

            }
            catch (Exception ex)
            {
                LogControl.save("GetContractor", ex.ToString(), "3");
            }

            return contractor;
        }

        public ResponseVM UpdateContractorLocation(ContractorLocationBM contractorLocation)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseVM response = new ResponseVM();
            string storedProcedureName = ProcedureName.pkg_SedesCliente + ".INS_CLIENTLOCATION";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_ACTION", OracleDbType.Char, contractorLocation.Action, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDCLIENTLOCATION", OracleDbType.Int32, contractorLocation.Id, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SIDCLIENTLOCATIONTYPE", OracleDbType.Char, contractorLocation.TypeId, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SDESCRIPTION", OracleDbType.Char, contractorLocation.Description, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCOD_CIUU", OracleDbType.Char, contractorLocation.EconomicActivityId, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SSTATREGT", OracleDbType.Char, contractorLocation.StateId, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SCLIENT", OracleDbType.Char, contractorLocation.ContractorId, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NDEPARTMENT", OracleDbType.Int32, contractorLocation.DepartmentId, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NPROVINCE", OracleDbType.Int32, contractorLocation.ProvinceId, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NDISTRICT", OracleDbType.Int32, contractorLocation.DistrictId, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SSTREET", OracleDbType.Char, contractorLocation.Address, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, contractorLocation.UserCode, ParameterDirection.Input));
                //OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, response.P_NCODE, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, response.P_SMESSAGE, ParameterDirection.Output);
                OracleParameter P_SIDCLGENERATED = new OracleParameter("P_SIDCLGENERATED", OracleDbType.Char, response.P_RESULT, ParameterDirection.Output);
                P_SIDCLGENERATED.Size = 25;
                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;
                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);
                parameter.Add(P_SIDCLGENERATED);

                this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                response.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                response.P_SMESSAGE = P_SMESSAGE.Value.ToString();
                response.P_RESULT = P_SIDCLGENERATED.Value.ToString().Trim();
            }
            catch (Exception ex)
            {
                LogControl.save("UpdateContractorLocation", ex.ToString(), "3");
            }

            return response;
        }

        public ResponseVM UpdateContractorLocationContact(ContractorLocationContactBM contact)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseVM response = new ResponseVM();
            string storedProcedureName = ProcedureName.pkg_SedesCliente + ".INS_CLIENTLOCATIONCONTACT";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_ACTION", OracleDbType.Char, contact.Action, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDCLIENTLOCATIONCONTACT", OracleDbType.Int32, contact.Id, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SFULLNAME", OracleDbType.Char, contact.FullName, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SPHONENUMBER", OracleDbType.Char, contact.PhoneNumber, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SEMAIL", OracleDbType.Char, contact.Email, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDCLIENTLOCATION", OracleDbType.Int32, contact.ContractorLocationId, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_SSTATE", OracleDbType.Char, contact.State, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, contact.UserCode, ParameterDirection.Input));


                //OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, response.P_NCODE, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, response.P_SMESSAGE, ParameterDirection.Output);
                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;
                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);

                this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                response.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                response.P_SMESSAGE = P_SMESSAGE.Value.ToString().Trim();

            }
            catch (Exception ex)
            {
                LogControl.save("UpdateContractorLocationContact", ex.ToString(), "3");
            }

            return response;
        }

        public ResponseVM DeleteContact(Int32 contactId, Int32 userCode)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseVM response = new ResponseVM();
            string storedProcedureName = ProcedureName.pkg_SedesCliente + ".ELIM_CLIENTLOCATIONCONTACT";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NIDCLIENTLOCATIONCONTACT", OracleDbType.Int32, contactId, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, userCode, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, response.P_NCODE, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, response.P_SMESSAGE, ParameterDirection.Output);
                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;
                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);

                this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                response.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                response.P_SMESSAGE = P_SMESSAGE.Value.ToString().Trim();

            }
            catch (Exception ex)
            {
                LogControl.save("DeleteContact", ex.ToString(), "3");
            }

            return response;
        }
        public ResponseVM GetSuggestedLocationType(string contractorId, Int32 userCode) // 1:error , 2: crear sucursal, 3: crear principal
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseVM resultPackage = new ResponseVM();
            string storedProcedureName = ProcedureName.pkg_SedesCliente + ".SEL_SUGGESTEDLOCATIONTYPE";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_SCLIENT", OracleDbType.Char, contractorId, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, userCode, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, resultPackage.GENERICLIST, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Char, resultPackage.P_SMESSAGE, ParameterDirection.Output);

                P_SMESSAGE.Size = 4000;
                P_NCODE.Size = 4000;
                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);

                this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                resultPackage.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                resultPackage.P_SMESSAGE = P_SMESSAGE.Value.ToString().Trim();
            }
            catch (Exception ex)
            {
                LogControl.save("GetSuggestedLocationType", ex.ToString(), "3");
            }

            return resultPackage;
        }
        public ResponseVM HasActivePrincipalLocation(string contractorId, Int32 locationIdToIgnore, Int32 userCode)
        { // 1:error , 2: si, 3: no
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseVM resultPackage = new ResponseVM();
            string storedProcedureName = ProcedureName.pkg_SedesCliente + ".HASACTIVEPRINCIPALLOCATION";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_SCLIENT", OracleDbType.Char, contractorId, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NIDCLIENTLOCATION", OracleDbType.Int32, locationIdToIgnore, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, userCode, ParameterDirection.Input));


                //OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, resultPackage.GENERICLIST, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Char, resultPackage.P_SMESSAGE, ParameterDirection.Output);

                P_SMESSAGE.Size = 4000;
                P_NCODE.Size = 4000;
                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);

                this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                resultPackage.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                resultPackage.P_SMESSAGE = P_SMESSAGE.Value.ToString().Trim();
            }
            catch (Exception ex)
            {
                LogControl.save("HasActivePrincipalLocation", ex.ToString(), "3");
            }

            return resultPackage;
        }
        public ResponseVM DeleteClientLocation(Int32 locationId, Int32 userCode)
        {
            List<OracleParameter> parameter = new List<OracleParameter>();
            ResponseVM resultPackage = new ResponseVM();
            string storedProcedureName = ProcedureName.pkg_SedesCliente + ".DEL_CLIENTLOCATION";
            try
            {
                //INPUT
                parameter.Add(new OracleParameter("P_NIDCLIENTLOCATION", OracleDbType.Int32, locationId, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, userCode, ParameterDirection.Input));

                //OUTPUT
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, resultPackage.GENERICLIST, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Char, resultPackage.P_SMESSAGE, ParameterDirection.Output);

                P_SMESSAGE.Size = 4000;
                P_NCODE.Size = 4000;
                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);

                this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                resultPackage.P_NCODE = Convert.ToInt32(P_NCODE.Value.ToString());
                resultPackage.P_SMESSAGE = P_SMESSAGE.Value.ToString().Trim();
            }
            catch (Exception ex)
            {
                LogControl.save("DeleteClientLocation", ex.ToString(), "3");
            }

            return resultPackage;
        }
    }
}
