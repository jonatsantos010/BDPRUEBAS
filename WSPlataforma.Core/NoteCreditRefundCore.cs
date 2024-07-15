/*************************************************************************************************/
/*  NOMBRE              :   NoteCreditCore.CS                                               */
/*  DESCRIPCION         :   Capa CORE                                                            */
/*  AUTOR               :   MATERIAGRIS - PEDRO ANTICONA VALLE                                   */
/*  FECHA               :   23-12-2021                                                           */
/*  VERSION             :   1.0 - Generación de NC - PD                                          */
/*************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.DA;
using WSPlataforma.Entities.NoteCreditRefundModel.ViewModel;

namespace WSPlataforma.Core
{
    public class NoteCreditRefundCore2
    {
       NoteCreditRefundDA2 DataAccessQuery = new NoteCreditRefundDA2();
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

        public PremiumVMResponse GetListDetRecDev(PremiumVM[] data)
        {
            return this.DataAccessQuery.GetListDetRecDev(data);
        }

        public PremiumVMResponse GetFilaResi(PremiumVM data)
        {
            return this.DataAccessQuery.GetFilaResi(data);
        }


        #endregion

        #region Validations

        #endregion
    }

}
