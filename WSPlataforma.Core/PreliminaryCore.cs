using System;
using System.Linq;
using System.Collections.Generic;
using WSPlataforma.DA;
using WSPlataforma.Entities.PreliminaryModel.BindingModel;
using WSPlataforma.Entities.PreliminaryModel.ViewModel;
using WSPlataforma.Util.Preliminary;
using WSPlataforma.Entities.ReportModel.BindingModel;

namespace WSPlataforma.Core
{
    public class PreliminaryCore
    {
        PreliminaryDA DataAccessQuery = new PreliminaryDA();

        #region Filters

        public List<SuggestVM> GetBranches()
        {
            return this.DataAccessQuery.GetBranches();
        }

        public List<SuggestVMDate> GetBranchPeriod(PreliminaryFilter data)
        {
            return this.DataAccessQuery.GetBranchPeriod(data);
        }
        public List<SuggestVM> GetTypeProcess()
        {
            return this.DataAccessQuery.GetTypeProcess();
        }

        public List<SuggestVM> GetReports()
        {
            return this.DataAccessQuery.GetReports();
        }

        #endregion

        #region Validations

        public List<PreliminaryValidationVM> GetPreliminaryValidation(PreliminaryFilter data)
        {
            var validationHandler = new ValidationDirector();
            return validationHandler.StartValidation(data);
        }

        //INI MMQ 14-04-2021 REQ-CSTR-002
        public ResponseControl InsertProcessPremiumReport(ReportProcess responseControl)
        {
            return this.DataAccessQuery.InsertProcessPremiumReport(responseControl);
        }

        public List<SuggestVMMonitor> GetMonitorProcess(PreliminaryFilter data)
        {
            return this.DataAccessQuery.GetMonitorProcess(data);
        }


        //FIN MMQ 14-04-2021 REQ-CSTR-002
        #endregion

    }

    public class ValidationDirector
    {
        public List<PreliminaryValidationVM> StartValidation(PreliminaryFilter data)
        {
            var commisionValidation = new CommissionHandler()
                                                .SetReportType(PreliminarySettings.ReportTypeEnum.Comisiones.GetHashCode());
            var collectionValidation = new CollectionHandler()
                                                .SetHandler(commisionValidation)
                                                .SetReportType(PreliminarySettings.ReportTypeEnum.Cobranzas.GetHashCode());
            var premiumValidation = new PremiumHandler()
                                                .SetHandler(collectionValidation)
                                                .SetReportType(PreliminarySettings.ReportTypeEnum.Primas.GetHashCode());

            return premiumValidation.GetHandlerValidation(data);
        }
    }

    public abstract class Handler
    {
        protected int ReportType;
        protected Handler NextHandler;
        protected PreliminaryDA DataAccessQuery = new PreliminaryDA();

        public Handler SetHandler(Handler nextHandler)
        {
            this.NextHandler = nextHandler;
            return this;
        }

        public Handler SetReportType(int reportType)
        {
            this.ReportType = reportType;
            return this;
        }

        public List<PreliminaryValidationVM> GetHandlerValidation(PreliminaryFilter data)
        {
            var preliminaries = new List<PreliminaryValidationVM>();

            if (this.ReportType == data.ReportId)
            {
                //VALIDACION PRIMAS Y COMISIONES
                var preliminary = this.GetPreliminaryValidation(data);
                preliminaries.AddRange(preliminary);
            }
            else
            {
                if (data.ReportId == 2) //VALIDACION COBRANZAS
                {
                    var preliminary = this.GetPreliminaryValidation(data);
                    preliminaries.AddRange(preliminary);
                }

                if (data.ReportId == 0) //VALIDACION PRIMAS, COMISIONES Y COBRANZAS (TODO)
                {
                    data.ReportId = 1;
                    var preliminary = this.GetPreliminaryValidation(data);

                    data.ReportId = 2;
                    var cobranzas = this.GetPreliminaryValidation(data);

                    data.ReportId = 0;

                    preliminaries.AddRange(preliminary);
                    preliminaries.AddRange(cobranzas);

                }
            }

            return preliminaries;
        }
        //FIN MMQ 14-04-2021 REQ-SCTR-002

        protected abstract List<PreliminaryValidationVM> GetPreliminaryValidation(PreliminaryFilter data);

    }

    public class PremiumHandler : Handler
    {
        protected override List<PreliminaryValidationVM> GetPreliminaryValidation(PreliminaryFilter data)
        {
            try
            {
                //GENERA PROCESO VALIDACION PRELIMIAR/DEFINITIVA     
                var validationId = this.DataAccessQuery.GetValidateHeadId(data);
                //OBTIENE CABECERA GENERADA
                var validation = this.DataAccessQuery.GetValidationById(validationId);
                return validation;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }

    public class CollectionHandler : Handler
    {
        protected override List<PreliminaryValidationVM> GetPreliminaryValidation(PreliminaryFilter data)
        {
            return new List<PreliminaryValidationVM>();
        }
    }

    public class CommissionHandler : Handler
    {
        protected override List<PreliminaryValidationVM> GetPreliminaryValidation(PreliminaryFilter data)
        {
            return new List<PreliminaryValidationVM>();
        }
    }
}
