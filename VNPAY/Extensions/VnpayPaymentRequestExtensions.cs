using System;
using VNPAY.Models;
using VNPAY.Models.Enums;

namespace VNPAY.Extensions
{
    public static class VnpayPaymentRequestExtensions
    {
        public static VnpayPaymentRequest Create(double money, string description, BankCode bankCode = BankCode.ANY)
        {
            if (money < 5 * 1000 || money > 1 * 1000 * 1000 * 1000)
            {
                throw new ArgumentException("Số tiền thanh toán phải nằm trong khoảng 5.000 (VND) đến 1.000.000.000 (VND).", nameof(money));
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("Không được để trống mô tả giao dịch.", nameof(description));
            }

            return new VnpayPaymentRequest
            {
                Money = money,
                Description = description.Trim(),
                BankCode = bankCode
            };
        }
    }
}
