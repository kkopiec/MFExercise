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
            // companyData is used throughOut most of service calls 
            var companyData = DTOHelper.GetCompanyDataRequest(application.CompanyData);

            // this is only call that returns stright int response from the service 
            if (application.Product is SelectiveInvoiceDiscount sid)
            {
                return _selectInvoiceService.SubmitApplicationFor(application.CompanyData.Number.ToString(), sid.InvoiceAmount, sid.AdvancePercentage);
            }
            
            if (application.Product is ConfidentialInvoiceDiscount cid)
            {
                var result = _confidentialInvoiceWebService.SubmitApplicationFor(
                      companyData
                    , cid.TotalLedgerNetworth, cid.AdvancePercentage, cid.VatRate);

                return DTOHelper.GetResponse(result);
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
                return DTOHelper.GetResponse(result);
            }

            // If there is any IProduct implementation which has not been yet implemented
            throw new InvalidOperationException(Constants.ERR_01);
        }
    }
}
