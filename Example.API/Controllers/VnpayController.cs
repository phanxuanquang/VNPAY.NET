using Microsoft.AspNetCore.Mvc;
using VNPAY;
using VNPAY.Models;
using VNPAY.Models.Enums;
using VNPAY.Models.Exceptions;

namespace Example.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VnpayController(ILogger<VnpayController> logger, IVnpayClient vnpayClient) : ControllerBase
    {
        private readonly ILogger<VnpayController> _logger = logger;
        private readonly IVnpayClient _vnpayClient = vnpayClient;

        /// <summary>
        /// Tạo URL thanh toán từ yêu cầu thanh toán
        /// </summary>
        /// <param name="money">Số tiền phải thanh toán</param>
        /// <param name="description">Mô tả giao dịch</param>
        /// <param name="bankCode">Mã phương thức thanh toán</param>
        /// <returns></returns>
        [HttpPost("CreatePaymentUrl")]
        public IActionResult CreatePaymentUrl(double money, string description, BankCode bankCode = BankCode.ANY)
        {
            try
            {
                var paymentUrlInfo = _vnpayClient.CreatePaymentUrl(money, description, bankCode);

                _logger.LogInformation("Payment ID: {PaymentId}", paymentUrlInfo.PaymentId);
                _logger.LogInformation("Payment URL created: {Url}", paymentUrlInfo.Url);
                _logger.LogInformation("Payment Parameters: {Parameters}", string.Join(", ", paymentUrlInfo.Parameters.Select(kv => $"{kv.Key}={kv.Value}")));

                return Ok(paymentUrlInfo.Url);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment URL");
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Tạo URL thanh toán từ yêu cầu thanh toán
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("CreatePaymentUrlFromPaymentRequest")]
        public IActionResult CreatePaymentUrl([FromBody] VnpayPaymentRequest request)
        {
            try
            {
                var paymentUrlInfo = _vnpayClient.CreatePaymentUrl(request);

                _logger.LogInformation("Payment ID: {PaymentId}", paymentUrlInfo.PaymentId);
                _logger.LogInformation("Payment URL created: {Url}", paymentUrlInfo.Url);
                _logger.LogInformation("Payment Parameters: {Parameters}", string.Join(", ", paymentUrlInfo.Parameters.Select(kv => $"{kv.Key}={kv.Value}")));

                return Ok(paymentUrlInfo.Url);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment URL");
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Thực hiện hành động sau khi thanh toán. URL này cần được khai báo với VNPAY để API này hoạt đồng (ví dụ: http://localhost:1234/api/Vnpay/IpnAction)
        /// </summary>
        /// <returns></returns>
        [HttpGet("ProceedAfterPayment")]
        public IActionResult ProceedAfterPayment()
        {
            try
            {
                var paymentResult = _vnpayClient.GetPaymentResult(this.Request);

                // Ghi log thông tin thanh toán để tiện theo dõi nếu cần
                _logger.LogInformation("Payment ID: {PaymentId}", paymentResult.PaymentId);
                _logger.LogInformation("VNPAY Transaction ID: {VnpayTransactionId}", paymentResult.VnpayTransactionId);
                _logger.LogInformation("Timestamp: {Timestamp}", paymentResult.Timestamp);
                _logger.LogInformation("Card Type: {CardType}", paymentResult.CardType);
                _logger.LogInformation("Banking Info: {BankingInfo}", paymentResult.BankingInfor != null ? $"{paymentResult.BankingInfor.BankCode} - {paymentResult.BankingInfor.BankTransactionId}" : "N/A");

                // Thực hiện hành động nếu thanh toán thành công tại đây. Ví dụ: Cập nhật trạng thái đơn hàng trong cơ sở dữ liệu.
                return Ok();
            }
            catch (VnpayException ex)  // Bắt lỗi liên quan đến VNPAY
            {
                _logger.LogError(ex, "VNPAY Error: {Message}, TransactionStatusCode: {TransactionStatusCode}, PaymentResponseCode: {PaymentResponseCode}", ex.Message, ex.TransactionStatusCode, ex.PaymentResponseCode);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing payment");
                return BadRequest(ex.Message);
            }
        }
    }
}
