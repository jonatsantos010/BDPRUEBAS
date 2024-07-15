/*************************************************************************************************/
/*  NOMBRE              :   CreditNoteCore.CS                                                    */
/*  DESCRIPCION         :   Capa CORE                                                            */
/*  AUTOR               :   MATERIAGRIS - JOSUE CORONEL FLORES                                   */
/*  FECHA               :   18-10-2022                                                           */
/*  VERSION             :   1.0                                                                  */
/*************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.DA;
using WSPlataforma.Entities.CreditNoteModel.ViewModel;

namespace WSPlataforma.Core
{
    public class NoteCreditRefundCore
    {
       NoteCreditRefundDA DataAccessQuery = new NoteCreditRefundDA();
        #region Filters

        public List<BranchVM> GetBranches()
        {
            return this.DataAccessQuery.GetBranches();
        }
        public List<ProductVM> GetProductsListByBranch(ProductVM data)
        {
            return DataAccessQuery.GetProductsListByBranch(data);
        }
        public List<ProductVM> GetParameter(ProductVM data)
        {
            return DataAccessQuery.GetParameter(data);
        }
        public List<DocumentVM> GetDocumentTypeList()
        {
            return this.DataAccessQuery.GetDocumentTypeList();
        }
        public List<BillVM> GetBillTypeList()
        {
            return this.DataAccessQuery.GetBillTypeList();
        }

        public PremiumVMResponse GetListPremium(PremiumVM data)
        {
            return this.DataAccessQuery.GetListPremium(data);
        }

        public List<PremiumVM> pruebas(PremiumVM data)
        {
            return this.DataAccessQuery.pruebas(data);
        }
        
        public PremiumVMResponse GetListReciDev(PremiumVM[] data)
        {
            return this.DataAccessQuery.GetListReciDev(data);
        }

        public PremiumVMResponse GetListDetRecDev(PremiumVM data)
        {
            return this.DataAccessQuery.GetListDetRecDev(data);
        }

        public PremiumVMResponse GetFilaResi(PremiumVM data)
        {
            return this.DataAccessQuery.GetFilaResi(data);
        }

        // evalua la opcion de carga masiva //migrantes 12/09/2023
        public resPremiumVM valCargaMasiva(PremiumVM data)
        {
            return this.DataAccessQuery.valCargaMasiva(data);
        }

        // evalua la opcion de carga masiva //migrantes 12/09/2023
        public PremiumVMResponse SaveUsingOracleBulkCopy(DataTable data)
        {
            return this.DataAccessQuery.SaveUsingOracleBulkCopy(data);
        }

        // Genera la nc de la acrga masaiva //migrantes 12/09/2023
        public PremiumVMResponse genNcMasiva(PremiumVM data)
        {
            return this.DataAccessQuery.genNcMasiva(data);
        }

        #endregion

        #region Validations

        #endregion
    }

}
