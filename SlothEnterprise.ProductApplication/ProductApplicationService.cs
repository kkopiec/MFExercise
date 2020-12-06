using System;
using SlothEnterprise.External;
using SlothEnterprise.External.V1;
using SlothEnterprise.ProductApplication.Applications;
using SlothEnterprise.ProductApplication.Products;

namespace SlothEnterprise.ProductApplication
{
    public class ProductApplicationService : IProductApplicationService
    {
        private readonly ISelectInvoiceService _selectInvoiceService;
        private readonly IConfidentialInvoiceService _confidentialInvoiceWebService;
        private readonly IBusinessLoansService _businessLoansService;

        public ProductApplicationService(ISelectInvoiceService selectInvoiceService, IConfidentialInvoiceService confidentialInvoiceWebService, IBusinessLoansService businessLoansService)
        {
            _selectInvoiceService = selectInvoiceService;
            _confidentialInvoiceWebService = confidentialInvoiceWebService;
            _businessLoansService = businessLoansService;
        }

        public int SubmitApplicationFor(ISellerApplication application)
        {
            var companyData = DTOHelper.GetCompanyDataRequest(application.CompanyData);
            if (application.Product is SelectiveInvoiceDiscount sid)
            {
                return _selectInvoiceService.SubmitApplicationFor(application.CompanyData.Number.ToString(), sid.InvoiceAmount, sid.AdvancePercentage);
            }
            
            if (application.Product is ConfidentialInvoiceDiscount cid)
            {
                var result = _confidentialInvoiceWebService.SubmitApplicationFor(
                      companyData
                    , cid.TotalLedgerNetworth, cid.AdvancePercentage, cid.VatRate);

                return (result.Success) ? result.ApplicationId ?? Constants.FAILURE_RESPONSE : Constants.FAILURE_RESPONSE;
            }

            if (application.Product is BusinessLoans loans)
            {
                var result = _businessLoansService.SubmitApplicationFor(
                    companyData                    
                    , new LoansRequest
                    {
                        InterestRatePerAnnum = loans.InterestRatePerAnnum,
                        LoanAmount = loans.LoanAmount
                    });
                return (result.Success) ? result.ApplicationId ?? Constants.FAILURE_RESPONSE : Constants.FAILURE_RESPONSE;
            }

            throw new InvalidOperationException(Constants.ERR_01);
        }
    }
}
