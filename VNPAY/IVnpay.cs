using Microsoft.AspNetCore.Http;
using VNPAY.Models;

namespace VNPAY
{
    /// <summary>
    /// Giao diện định nghĩa các phương thức cần thiết để tích hợp với hệ thống thanh toán VNPAY.
    /// Các phương thức trong giao diện này cung cấp chức năng tạo URL thanh toán và thực hiện giao dịch.
    /// </summary>
    public interface IVnpay
    {

        /// <summary>
        /// Tạo URL thanh toán cho giao dịch dựa trên các tham số trong yêu cầu thanh toán.
        /// </summary>
        /// <param name="request">Thông tin yêu cầu thanh toán, bao gồm các tham số như mã giao dịch, số tiền, mô tả,...</param>
        /// <param name="isTest">Chỉ định xem có phải là môi trường thử nghiệm hay không (mặc định là true).</param>
        /// <returns>URL thanh toán để chuyển hướng người dùng tới trang thanh toán của VNPAY.</returns>
        string GetPaymentUrl(VnpayPaymentRequest request);

        /// <summary>
        /// Thực hiện giao dịch thanh toán và trả về kết quả.
        /// Phương thức này được gọi khi nhận được thông tin callback từ VNPAY.
        /// </summary>
        /// <param name="collections">Thông tin các tham số trả về từ VNPAY qua callback.</param>
        /// <returns></returns>
        VnpayPaymentResult GetPaymentResult(IQueryCollection parameters);
    }
}
