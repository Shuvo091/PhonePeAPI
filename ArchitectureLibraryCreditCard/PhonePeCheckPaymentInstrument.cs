using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchitectureLibraryCreditCardModels
{
    public class PhonePeCheckPaymentInstrument
    {
        public string Type { get; set; }
        public string Utr { get; set; }
        public string CardType { get; set; }
        public string PgTransactionId { get; set; }
        public string BankTransactionId { get; set; }
        public string PgAuthorizationCode { get; set; }
        public string Arn { get; set; }
        public string BankId { get; set; }
        public string Brn { get; set; }
        public string PgServiceTransactionId { get; set; }
    }
}
