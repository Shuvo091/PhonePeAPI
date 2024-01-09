using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchitectureLibraryCreditCardBusinessLayer
{
    public static class PhonePeResponseCode
    {
        // Constants for SUCCESS status
        public const string PAYMENT_SUCCESS = "PAYMENT_SUCCESS";

        // Constants for FAILURE status
        public const string BAD_REQUEST = "BAD_REQUEST";
        public const string AUTHORIZATION_FAILED = "AUTHORIZATION_FAILED";
        public const string INTERNAL_SERVER_ERROR = "INTERNAL_SERVER_ERROR";
        public const string TRANSACTION_NOT_FOUND = "TRANSACTION_NOT_FOUND";
        public const string PAYMENT_ERROR = "PAYMENT_ERROR";
        public const string PAYMENT_PENDING = "PAYMENT_PENDING";
        public const string PAYMENT_DECLINED = "PAYMENT_DECLINED";
        public const string TIMED_OUT = "TIMED_OUT";
    }
}
