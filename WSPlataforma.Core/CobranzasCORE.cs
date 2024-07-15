using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSPlataforma.DA;
using WSPlataforma.Entities.CobranzasModel;
using WSPlataforma.Entities.PolicyModel.ViewModel;

namespace WSPlataforma.Core
{
    public class CobranzasCORE
    {
        CobranzasDA cobranzasDA = new CobranzasDA();

        public List<TypeRiskBM> GetTypeRisk()
        {
            return cobranzasDA.getTypeRisk();
        }
        public List<TypeRestricBM> GetTypeRestric()
        {
            return cobranzasDA.getTypeRestric();
        }
        public List<EntityPolicyCreditBM> getCreditPoliciesList(ParamsCreditPoliciesBM param)
        {
            return cobranzasDA.getCreditPoliciesList(param);
        }
        public ResponseVM InsertCreditPolicies(EntityPolicyCreditBM entitie)
        {
            return cobranzasDA.InsertCreditPolicies(entitie);
        }
        public List<EntityPaymentClientStatusBM> getStatusClientList(ParamsPaymentClientStatus param)
        {
            return cobranzasDA.getStatusClientList(param);
        }

        public List<EntityPaymentClientInfoBM> getClientInfoList(ParamsPaymentClientInfo param)
        {
            return cobranzasDA.getClientInfoList(param);
        }
        public List<EntityPrivilegiosClientBM> getClientPrivilegiosList(ParamsPaymentClientStatus param)
        {
            return cobranzasDA.getClientPrivilegiosList(param);
        }

        public ResponseVM InsertClientRestric(ParamsPaymentClientStatus entitie)
        {
            return cobranzasDA.InsertClientRestric(entitie);
        }

        public EntityValidateLock ValidateLock(ParamsValidateLock param)
        {
            return cobranzasDA.ValidateLock(param);
        }

        public EntityValidateDebt ValidateDebt(ParamsValidateDebt param)
        {
            if (param.profileCode != 134)
            {
                return cobranzasDA.ValidateDebt(param);
            }
            else
            {
                return cobranzasDA.ValidateDebtExterno(param);
            }
        }

        public ResponseAccountStatusVM generateAccountStatus(ParamsGenerateAccountStatus param)
        {
            return cobranzasDA.generateAccountStatus(param);
        }
        public ResponseAccountStatusVM GetTramaMovimiento(ParamsMovimientoVL param)
        {
            ExcelGenerate exceDA = new ExcelGenerate();

            return exceDA.GetTramaMovimiento(param);
        }

        public ResponseAccountStatusVM GetTramaMovimientoSCTR(ParamsMovimientoVL param)
        {
            ExcelGenerate exceDA = new ExcelGenerate();

            return exceDA.GetTramaMovimientoSCTR(param);
        }
        public ResponseAccountStatusVM GeneratesSlipCCE(ParamsValidateDebt param)
        {
            ResponseAccountStatusVM response = new ResponseAccountStatusVM();
            EntityValidateDebt validate = cobranzasDA.ValidateDebtCot(param);

            if (validate.lockMark != 0)
            {
                response = cobranzasDA.generateAccountStatus(new ParamsGenerateAccountStatus
                {
                    branchCode = validate.idbranch,
                    clientCode = validate.idclient,
                    documentCode = 27,
                    productCode = validate.idproduct
                });

                response.indEECC = 1;
            }
            return response;
        }
        public EntityValidateDebt ValidateDebtPolicy(ParamsValidateDebt param)
        {
            return cobranzasDA.ValidateDebtPolicy(param);
        }

        public List<EntityReceiptBill> getReceiptBill(ParamsBill param)
        {
            return cobranzasDA.getReceiptBill(param);
        }

        public ResponseVM InsertRebill(ParamsRebill entitie)
        {
            return cobranzasDA.InsertRebill(entitie);
        }

        public ResponseAccountStatusVM GetTramaEndoso(ParamsMovimientoVL param)
        {
            ExcelGenerate exceDA = new ExcelGenerate();

            return exceDA.GetTramaEndoso(param);
        }

        public ResponseAccountStatusVM GetTramaCover(ParamsMovimientoVL param)
        {
            ExcelGenerate exceDA = new ExcelGenerate();

            return exceDA.GetTramaCover(param);
        }
    }
}
