using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchitectureLibraryCreditCardModels
{
    public class PhonePeCheckPaymentData
    {
        public string MerchantId { get; set; }
        public string MerchantTransactionId { get; set; }
        public string TransactionId { get; set; }
        public int Amount { get; set; }
        public string State { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseCodeDescription { get; set; }
        public PhonePeCheckPaymentInstrument PaymentInstrument { get; set; }
    }
}
