using Microsoft.AspNetCore.Mvc;
using VNPAY;
using VNPAY.Models;
using VNPAY.Models.Enums;
using VNPAY.Models.Exceptions;

namespace Backend_API_Testing.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VnpayController(IVnpayClient vnpayClient) : ControllerBase
    {
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
                return Ok(paymentUrlInfo.Url);
            }
            catch (Exception ex)
            {
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
                var paymentUrlInfor = _vnpayClient.CreatePaymentUrl(request);
                return Ok(paymentUrlInfor.Url);
            }
            catch (Exception ex)
            {
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

                // Thực hiện hành động nếu thanh toán thành công tại đây. Ví dụ: Cập nhật trạng thái đơn hàng trong cơ sở dữ liệu.
                return Ok();
            }
            catch (VnpayException ex)  // Bắt lỗi liên quan đến VNPAY
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}