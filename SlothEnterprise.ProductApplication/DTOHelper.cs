﻿using SlothEnterprise.External;
using SlothEnterprise.ProductApplication.Applications;
using System;
using System.Collections.Generic;
using System.Text;

namespace SlothEnterprise.ProductApplication
{
    public static class DTOHelper
    {
        public static CompanyDataRequest GetCompanyDataRequest(ISellerCompanyData data)
        {
            return new CompanyDataRequest
            {
                CompanyFounded = data.Founded,
                CompanyNumber = data.Number,
                CompanyName = data.Name,
                DirectorName = data.DirectorName
            };
        }
        public static int GetResponse(IApplicationResult result)
        {
            return (result.Success) ? result.ApplicationId ?? Constants.FAILURE_RESPONSE : Constants.FAILURE_RESPONSE;
        }
    }
}
