using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using VNPAY.Extensions;
using VNPAY.Extensions.Options;
using VNPAY.Helpers;
using VNPAY.Models;
using VNPAY.Models.Enums;

namespace VNPAY
{
    public class Vnpay : IVnpay
    {
        private readonly VnpayConfigurations _configs;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Vnpay(IOptions<VnpayConfigurations> configs, IHttpContextAccessor httpContextAccessor)
        {
            configs.Value.EnsureValid();

            _configs = configs.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetPaymentUrl(VnpayPaymentRequest request)
        {
            if (request.Money < 5 * 1000 || request.Money > 1 * 1000 * 1000 * 1000)
            {
                throw new ArgumentException("Số tiền thanh toán phải nằm trong khoảng 5.000 (VND) đến 1.000.000.000 (VND).");
            }

            if (string.IsNullOrWhiteSpace(request.Description))
            {
                throw new ArgumentException("Không được để trống mô tả giao dịch.");
            }

            var ipAddress = GetCurrentUserIpAddress();

            var requestData = new SortedList<string, string>(new Comparer());

            if (!string.IsNullOrEmpty(_configs.Version))
                requestData.Add("vnp_Version", _configs.Version);

            requestData.Add("vnp_Command", "pay");

            if (!string.IsNullOrEmpty(_configs.TmnCode))
                requestData.Add("vnp_TmnCode", _configs.TmnCode);

            requestData.Add("vnp_Amount", (request.Money * 100).ToString());
            requestData.Add("vnp_CreateDate", request.CreatedTime.ToString("yyyyMMddHHmmss"));
            requestData.Add("vnp_CurrCode", request.Currency.ToString().ToUpper());

            if (!string.IsNullOrEmpty(ipAddress))
                requestData.Add("vnp_IpAddr", ipAddress);

            requestData.Add("vnp_Locale", request.Language.GetDescription());

            var bankCode = request.BankCode == BankCode.ANY ? string.Empty : request.BankCode.ToString();
            if (!string.IsNullOrEmpty(bankCode))
                requestData.Add("vnp_BankCode", bankCode);

            if (!string.IsNullOrEmpty(request.Description.Trim()))
                requestData.Add("vnp_OrderInfo", request.Description.Trim());

            if (!string.IsNullOrEmpty(_configs.OrderType))
                requestData.Add("vnp_OrderType", _configs.OrderType);

            if (!string.IsNullOrEmpty(_configs.CallbackUrl))
                requestData.Add("vnp_ReturnUrl", _configs.CallbackUrl);

            requestData.Add("vnp_TxnRef", request.PaymentId.ToString());

            return CreatePaymentUrl(requestData, _configs.BaseUrl, _configs.HashSecret);
        }

        public VnpayPaymentResult GetPaymentResult(IQueryCollection parameters)
        {
            var responseData = parameters
                .Where(kv => !string.IsNullOrEmpty(kv.Key) && kv.Key.StartsWith("vnp_"))
                .ToDictionary(kv => kv.Key, kv => kv.Value.ToString());

            var bankCode = responseData.GetValueOrDefault("vnp_BankCode");
            var bankTranNo = responseData.GetValueOrDefault("vnp_BankTranNo");
            var cardType = responseData.GetValueOrDefault("vnp_CardType");
            var payDate = responseData.GetValueOrDefault("vnp_PayDate");
            var orderInfo = responseData.GetValueOrDefault("vnp_OrderInfo");
            var transactionNo = responseData.GetValueOrDefault("vnp_TransactionNo");
            var responseCode = responseData.GetValueOrDefault("vnp_ResponseCode");
            var transactionStatus = responseData.GetValueOrDefault("vnp_TransactionStatus");
            var txnRef = responseData.GetValueOrDefault("vnp_TxnRef");
            var secureHash = responseData.GetValueOrDefault("vnp_SecureHash");

            if (string.IsNullOrEmpty(bankCode)
                || string.IsNullOrEmpty(orderInfo)
                || string.IsNullOrEmpty(transactionNo)
                || string.IsNullOrEmpty(responseCode)
                || string.IsNullOrEmpty(transactionStatus)
                || string.IsNullOrEmpty(txnRef)
                || string.IsNullOrEmpty(secureHash))
            {
                throw new ArgumentException("Không đủ dữ liệu để xác thực giao dịch");
            }

            var sortedResponseData = new SortedList<string, string>(new Comparer());
            foreach (var (key, value) in responseData)
            {
                if (!key.Equals("vnp_SecureHash") && !string.IsNullOrEmpty(value))
                {
                    sortedResponseData.Add(key, value);
                }
            }

            var responseCodeValue = (ResponseCode)sbyte.Parse(responseCode);
            var transactionStatusCode = (TransactionStatusCode)sbyte.Parse(transactionStatus);

            return new VnpayPaymentResult
            {
                PaymentId = long.Parse(txnRef),
                VnpayTransactionId = long.Parse(transactionNo),
                IsSuccess = transactionStatusCode == TransactionStatusCode.Code_00 
                    && responseCodeValue == ResponseCode.Code_00 
                    && IsSignatureCorrect(sortedResponseData, secureHash, _configs.HashSecret),
                Description = orderInfo,
                PaymentMethod = string.IsNullOrEmpty(cardType)
                    ? "Không xác định"
                    : cardType,
                Timestamp = string.IsNullOrEmpty(payDate)
                    ? DateTime.UtcNow
                    : DateTime.ParseExact(payDate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                TransactionStatus = new TransactionStatus
                {
                    Code = transactionStatusCode,
                    Description = transactionStatusCode.GetDescription()
                },
                PaymentResponse = new PaymentResponse
                {
                    Code = responseCodeValue,
                    Description = responseCodeValue.GetDescription()
                },
                BankingInfor = new BankingInfor
                {
                    BankCode = bankCode,
                    BankTransactionId = string.IsNullOrEmpty(bankTranNo)
                        ? "Không xác định"
                        : bankTranNo,
                }
            };
        }

        private string GetCurrentUserIpAddress()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            return httpContext == null 
                ? throw new InvalidOperationException("HttpContext không khả dụng") 
                : httpContext.GetIpAddress();
        }

        #region Private Payment Helper Methods
        private string CreatePaymentUrl(SortedList<string, string> requestData, string baseUrl, string hashSecret)
        {
            var queryBuilder = new StringBuilder();

            foreach (var (key, value) in requestData.Where(kv => !string.IsNullOrEmpty(kv.Value)))
            {
                queryBuilder.Append($"{WebUtility.UrlEncode(key)}={WebUtility.UrlEncode(value)}&");
            }

            if (queryBuilder.Length > 0)
            {
                queryBuilder.Length--;
            }

            var signData = queryBuilder.ToString();
            var secureHash = signData.AsHmacSHA512(hashSecret);

            return $"{baseUrl}?{signData}&vnp_SecureHash={WebUtility.UrlEncode(secureHash)}";
        }

        private bool IsSignatureCorrect(SortedList<string, string> responseData, string? inputHash, string secretKey)
        {
            if (string.IsNullOrEmpty(inputHash))
            {
                return false;
            }

            var rspRaw = GetResponseData(responseData);
            var checksum = rspRaw.AsHmacSHA512(secretKey);
            return checksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }

        private string GetResponseData(SortedList<string, string> responseData)
        {
            var filteredData = new SortedList<string, string>(new Comparer());

            foreach (var (key, value) in responseData)
            {
                if (!key.Equals("vnp_SecureHashType") && !key.Equals("vnp_SecureHash") && !string.IsNullOrEmpty(value))
                {
                    filteredData.Add(key, value);
                }
            }

            var validData = filteredData.Select(kv => $"{WebUtility.UrlEncode(kv.Key)}={WebUtility.UrlEncode(kv.Value)}");

            return string.Join("&", validData);
        }

        #endregion
    }
}
