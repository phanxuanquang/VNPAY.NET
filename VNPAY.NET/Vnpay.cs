using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Globalization;
using VNPAY.NET.Enums;
using VNPAY.NET.Extensions;
using VNPAY.NET.Extensions.Options;
using VNPAY.NET.Models;
using VNPAY.NET.Utilities;

namespace VNPAY.NET
{
    public class Vnpay(IOptions<VnpayOptions> options, IHttpContextAccessor httpContextAccessor) : IVnpay
    {
        private readonly VnpayOptions _options = options.Value;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        /// <summary>
        /// Tạo URL thanh toán 
        /// </summary>
        /// <param name="request">Thông tin cần có để tạo yêu cầu</param>
        /// <returns></returns>
        public string GetPaymentUrl(PaymentRequest request)
        {
            if (request.MoneyInVnd < 5000 || request.MoneyInVnd > 1000000000)
            {
                throw new ArgumentException("Số tiền thanh toán phải nằm trong khoảng 5.000 (VND) đến 1.000.000.000 (VND).");
            }

            if (string.IsNullOrEmpty(request.Description))
            {
                throw new ArgumentException("Không được để trống mô tả giao dịch.");
            }

            var ipAddress = GetCurrentUserIpAddress();

            var helper = new PaymentHelper();
            helper.AddRequestData("vnp_Version", _options.Version);
            helper.AddRequestData("vnp_Command", "pay");
            helper.AddRequestData("vnp_TmnCode", _options.TmnCode);
            helper.AddRequestData("vnp_Amount", (request.MoneyInVnd * 100).ToString());
            helper.AddRequestData("vnp_CreateDate", request.CreatedTime.ToString("yyyyMMddHHmmss"));
            helper.AddRequestData("vnp_CurrCode", request.Currency.ToString().ToUpper());
            helper.AddRequestData("vnp_IpAddr", ipAddress);
            helper.AddRequestData("vnp_Locale", request.Language.GetDescription());
            helper.AddRequestData("vnp_BankCode", request.BankCode == BankCode.ANY ? string.Empty : request.BankCode.ToString());
            helper.AddRequestData("vnp_OrderInfo", request.Description.Trim());
            helper.AddRequestData("vnp_OrderType", _options.OrderType);
            helper.AddRequestData("vnp_ReturnUrl", _options.CallbackUrl);
            helper.AddRequestData("vnp_TxnRef", request.PaymentId.ToString());

            return helper.GetPaymentUrl(_options.BaseUrl, _options.HashSecret);
        }

        /// <summary>
        /// Lấy kết quả thanh toán sau khi thực hiện giao dịch.
        /// </summary>
        /// <param name="parameters">Các tham số trong chuỗi truy vấn của <c>CallbackUrl</c></param>
        /// <returns></returns>
        public PaymentResult GetPaymentResult(IQueryCollection parameters)
        {
            var responseData = parameters
                .Where(kv => !string.IsNullOrEmpty(kv.Key) && kv.Key.StartsWith("vnp_"))
                .ToDictionary(kv => kv.Key, kv => kv.Value.ToString());

            var vnp_BankCode = responseData.GetValueOrDefault("vnp_BankCode");
            var vnp_BankTranNo = responseData.GetValueOrDefault("vnp_BankTranNo");
            var vnp_CardType = responseData.GetValueOrDefault("vnp_CardType");
            var vnp_PayDate = responseData.GetValueOrDefault("vnp_PayDate");
            var vnp_OrderInfo = responseData.GetValueOrDefault("vnp_OrderInfo");
            var vnp_TransactionNo = responseData.GetValueOrDefault("vnp_TransactionNo");
            var vnp_ResponseCode = responseData.GetValueOrDefault("vnp_ResponseCode");
            var vnp_TransactionStatus = responseData.GetValueOrDefault("vnp_TransactionStatus");
            var vnp_TxnRef = responseData.GetValueOrDefault("vnp_TxnRef");
            var vnp_SecureHash = responseData.GetValueOrDefault("vnp_SecureHash");

            if (string.IsNullOrEmpty(vnp_BankCode)
                || string.IsNullOrEmpty(vnp_OrderInfo)
                || string.IsNullOrEmpty(vnp_TransactionNo)
                || string.IsNullOrEmpty(vnp_ResponseCode)
                || string.IsNullOrEmpty(vnp_TransactionStatus)
                || string.IsNullOrEmpty(vnp_TxnRef)
                || string.IsNullOrEmpty(vnp_SecureHash))
            {
                throw new ArgumentException("Không đủ dữ liệu để xác thực giao dịch");
            }

            var helper = new PaymentHelper();
            foreach (var (key, value) in responseData)
            {
                if (!key.Equals("vnp_SecureHash"))
                {
                    helper.AddResponseData(key, value);
                }
            }

            var responseCode = (ResponseCode)sbyte.Parse(vnp_ResponseCode);
            var transactionStatusCode = (TransactionStatusCode)sbyte.Parse(vnp_TransactionStatus);

            return new PaymentResult
            {
                PaymentId = long.Parse(vnp_TxnRef),
                VnpayTransactionId = long.Parse(vnp_TransactionNo),
                IsSuccess = transactionStatusCode == TransactionStatusCode.Code_00 && responseCode == ResponseCode.Code_00 && helper.IsSignatureCorrect(vnp_SecureHash, _options.HashSecret),
                Description = vnp_OrderInfo,
                PaymentMethod = string.IsNullOrEmpty(vnp_CardType)
                    ? "Không xác định"
                    : vnp_CardType,
                Timestamp = string.IsNullOrEmpty(vnp_PayDate)
                    ? DateTime.Now
                    : DateTime.ParseExact(vnp_PayDate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                TransactionStatus = new TransactionStatus
                {
                    Code = transactionStatusCode,
                    Description = transactionStatusCode.GetDescription()
                },
                PaymentResponse = new PaymentResponse
                {
                    Code = responseCode,
                    Description = responseCode.GetDescription()
                },
                BankingInfor = new BankingInfor
                {
                    BankCode = vnp_BankCode,
                    BankTransactionId = string.IsNullOrEmpty(vnp_BankTranNo)
                        ? "Không xác định"
                        : vnp_BankTranNo,
                }
            };
        }

        /// <summary>
        /// Lấy địa chỉ IP từ HttpContext hiện tại
        /// </summary>
        /// <returns>Địa chỉ IP của client</returns>
        private string GetCurrentUserIpAddress()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            return httpContext == null ? throw new InvalidOperationException("HttpContext không khả dụng") : httpContext.GetIpAddress();
        }
    }
}
