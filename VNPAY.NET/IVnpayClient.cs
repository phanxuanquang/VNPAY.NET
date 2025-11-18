using Microsoft.AspNetCore.Http;
using VNPAY.Models;
using VNPAY.Models.Enums;

namespace VNPAY
{
    /// <summary>
    /// Giao diện định nghĩa các phương thức cần thiết để tích hợp với hệ thống thanh toán VNPAY.
    /// Các phương thức trong giao diện này cung cấp chức năng tạo URL thanh toán và thực hiện giao dịch.
    /// </summary>
    public interface IVnpayClient
    {

        /// <summary>
        /// Tạo URL thanh toán cho giao dịch dựa trên các tham số trong yêu cầu thanh toán.
        /// </summary>
        /// <param name="request">Thông tin yêu cầu thanh toán, bao gồm các tham số như mã giao dịch, số tiền, mô tả,...</param>
        /// <param name="isTest">Chỉ định xem có phải là môi trường thử nghiệm hay không (mặc định là true).</param>
        /// <returns>URL thanh toán để chuyển hướng người dùng tới trang thanh toán của VNPAY.</returns>
        PaymentUrlDetail CreatePaymentUrl(VnpayPaymentRequest request);

        /// <summary>
        /// Tạo URL thanh toán cho giao dịch dựa trên các tham số truyền vào.
        /// </summary>
        /// <param name="money">Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Số tiền phải nằm trong khoảng 5.000 (VND) đến 1.000.000.000 (VND).</param>
        /// <param name="description">Thông tin mô tả nội dung thanh toán, không dấu và không bao gồm các ký tự đặc biệt</param>
        /// <param name="bankCode">Mã phương thức thanh toán, mã loại ngân hàng hoặc ví điện tử thanh toán. Nếu mang giá trị <c>BankCode.ANY</c> thì chuyển hướng người dùng sang VNPAY chọn phương thức thanh toán.</param>
        /// <returns></returns>
        PaymentUrlDetail CreatePaymentUrl(double money, string description, BankCode bankCode = BankCode.ANY);

        /// <summary>
        /// Thực hiện giao dịch thanh toán và trả về kết quả.
        /// Phương thức này được gọi khi nhận được thông tin callback từ VNPAY.
        /// </summary>
        /// <param name="collections">Thông tin các tham số trả về từ VNPAY qua callback.</param>
        /// <returns></returns>
        VnpayPaymentResult GetPaymentResult(IQueryCollection parameters);
    }
}
