using System;
using VNPAY.Models.Enums;

namespace VNPAY.Models.Exceptions
{
    public class VnpayException : Exception
    {
        public string Message { get; internal set; }
        public TransactionStatusCode TransactionStatusCode { get; internal set; }
        public PaymentResponseCode PaymentResponseCode { get; internal set; }
    }
}
