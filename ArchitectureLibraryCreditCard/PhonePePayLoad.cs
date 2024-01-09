using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchitectureLibraryCreditCardModels
{
    public class PhonePePayLoad
    {
        public string MerchantId { set; get; }
        public string MerchantTransactionId { set; get; }
        public string MerchantUserId { set; get; }
        public string CreditCardAmount { set; get; }
        public string MerchantRedirectUrl { set; get; }
        public string MerchantRedirectMode { set; get; }
        public string MerchantCallBackUrl { set; get; }
        public string CustomerMobileNumber { set; get; }
        public string PaymentInstrumentType { set; get; }
        public string SaltKey { set; get; }
        public string SaltIndex { set; get; }
        public string BaseUrl { set; get; }
        public string RestAPIRootUri { set; get; }
        public string RequestUri { set; get; }
        public string CheckStatusRestAPIRootUri { set; get; }
        public string CheckStatusRequestUri { set; get; }
    }
}
