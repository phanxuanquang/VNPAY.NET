using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Net;
using System.Text;
using VNPAY.Extensions;
using VNPAY.Extensions.Options;
using VNPAY.Models;
using VNPAY.Models.Enums;
using VNPAY.Utilities;

namespace VNPAY
{
    public class Vnpay(IOptions<VnpayConfigurations> options, IHttpContextAccessor httpContextAccessor) : IVnpay
    {
        private readonly VnpayConfigurations _options = options.Value;
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

            var requestData = new SortedList<string, string>(new Comparer());
            
            // Thêm dữ liệu yêu cầu vào SortedList
            if (!string.IsNullOrEmpty(_options.Version))
                requestData.Add("vnp_Version", _options.Version);
            
            requestData.Add("vnp_Command", "pay");
            
            if (!string.IsNullOrEmpty(_options.TmnCode))
                requestData.Add("vnp_TmnCode", _options.TmnCode);
                
            requestData.Add("vnp_Amount", (request.MoneyInVnd * 100).ToString());
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
                
            if (!string.IsNullOrEmpty(_options.OrderType))
                requestData.Add("vnp_OrderType", _options.OrderType);
                
            if (!string.IsNullOrEmpty(_options.CallbackUrl))
                requestData.Add("vnp_ReturnUrl", _options.CallbackUrl);
                
            requestData.Add("vnp_TxnRef", request.PaymentId.ToString());

            return CreatePaymentUrl(requestData, _options.BaseUrl, _options.HashSecret);
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

            // Tạo SortedList với Comparer để xử lý dữ liệu phản hồi
            var sortedResponseData = new SortedList<string, string>(new Comparer());
            foreach (var (key, value) in responseData)
            {
                if (!key.Equals("vnp_SecureHash") && !string.IsNullOrEmpty(value))
                {
                    sortedResponseData.Add(key, value);
                }
            }

            var responseCode = (ResponseCode)sbyte.Parse(vnp_ResponseCode);
            var transactionStatusCode = (TransactionStatusCode)sbyte.Parse(vnp_TransactionStatus);

            return new PaymentResult
            {
                PaymentId = long.Parse(vnp_TxnRef),
                VnpayTransactionId = long.Parse(vnp_TransactionNo),
                IsSuccess = transactionStatusCode == TransactionStatusCode.Code_00 && responseCode == ResponseCode.Code_00 && IsSignatureCorrect(sortedResponseData, vnp_SecureHash, _options.HashSecret),
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

        #region Private Payment Helper Methods

        /// <summary>
        /// Tạo URL thanh toán từ dữ liệu yêu cầu
        /// </summary>
        /// <param name="requestData">Dữ liệu yêu cầu thanh toán</param>
        /// <param name="baseUrl">URL cơ sở của VNPAY</param>
        /// <param name="hashSecret">Khóa bí mật để tạo chữ ký</param>
        /// <returns>URL thanh toán hoàn chỉnh</returns>
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
            var secureHash = VNPAY.Utilities.Encoder.AsHmacSHA512(hashSecret, signData);

            return $"{baseUrl}?{signData}&vnp_SecureHash={WebUtility.UrlEncode(secureHash)}";
        }

        /// <summary>
        /// Kiểm tra tính chính xác của chữ ký phản hồi
        /// </summary>
        /// <param name="responseData">Dữ liệu phản hồi từ VNPAY</param>
        /// <param name="inputHash">Chữ ký đầu vào cần kiểm tra</param>
        /// <param name="secretKey">Khóa bí mật</param>
        /// <returns>True nếu chữ ký chính xác</returns>
        private bool IsSignatureCorrect(SortedList<string, string> responseData, string? inputHash, string secretKey)
        {
            if (string.IsNullOrEmpty(inputHash))
            {
                return false;
            }

            var rspRaw = GetResponseData(responseData);
            var checksum = VNPAY.Utilities.Encoder.AsHmacSHA512(secretKey, rspRaw);
            return checksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Tạo chuỗi dữ liệu phản hồi để kiểm tra chữ ký
        /// </summary>
        /// <param name="responseData">Dữ liệu phản hồi từ VNPAY</param>
        /// <returns>Chuỗi dữ liệu được mã hóa URL</returns>
        private string GetResponseData(SortedList<string, string> responseData)
        {
            // Tạo bản sao để không thay đổi dữ liệu gốc
            var filteredData = new SortedList<string, string>(new Comparer());
            
            foreach (var (key, value) in responseData)
            {
                if (!key.Equals("vnp_SecureHashType") && 
                    !key.Equals("vnp_SecureHash") && 
                    !string.IsNullOrEmpty(value))
                {
                    filteredData.Add(key, value);
                }
            }

            var validData = filteredData
                .Select(kv => $"{WebUtility.UrlEncode(kv.Key)}={WebUtility.UrlEncode(kv.Value)}");

            return string.Join("&", validData);
        }

        #endregion
    }
}
