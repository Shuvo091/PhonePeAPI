using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchitectureLibraryCreditCardModels
{
    public class PhonePePaymentResponseModel
    {
        public string Code { get; set; }
        public string MerchantId { get; set; }
        public string TransactionId { get; set; }
        public int Amount { get; set; }
        public string ProviderReferenceId { get; set; }

        public string Param1 { get; set; }
        public string Param2 { get; set; }
        public string Param3 { get; set; }
        // Add properties for other parameters as needed
        public string Checksum { get; set; }
    }
}
