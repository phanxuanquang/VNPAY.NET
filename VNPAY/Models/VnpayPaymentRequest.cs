using System;
using VNPAY.Models.Enums;

namespace VNPAY.Models
{
    /// <summary>
    /// Yêu cầu thanh toán gửi đến cổng thanh toán VNPAY.
    /// </summary>
    public class VnpayPaymentRequest
    {
        /// <summary>
        /// Mã tham chiếu giao dịch (Transaction Reference). Đây là mã số duy nhất dùng để xác định giao dịch.  
        /// Lưu ý: Giá trị này bắt buộc và cần đảm bảo không bị trùng lặp giữa các giao dịch.
        /// </summary>
        public long PaymentId { get; protected set; } = DateTime.UtcNow.Ticks;

        /// <summary>
        /// Thông tin mô tả nội dung thanh toán, không dấu và không bao gồm các ký tự đặc biệt
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Số tiền phải nằm trong khoảng 5.000 (VND) đến 1.000.000.000 (VND).
        /// </summary>
        public double Money { get; set; }

        /// <summary>
        /// Mã phương thức thanh toán, mã loại ngân hàng hoặc ví điện tử thanh toán. Nếu mang giá trị <c>BankCode.ANY</c> thì chuyển hướng người dùng sang VNPAY chọn phương thức thanh toán.
        /// </summary>
        public BankCode BankCode { get; set; } = BankCode.ANY;

        /// <summary>
        /// Thời điểm khởi tạo giao dịch. Giá trị mặc định là ngày và giờ hiện tại tại thời điểm yêu cầu được khởi tạo.  
        /// </summary>
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VND
        /// </summary>
        internal Currency Currency { get; set; } = Currency.VND;

        /// <summary>
        /// Ngôn ngữ hiển thị trên giao diện thanh toán của VNPAY, mặc định là tiếng Việt.  
        /// </summary>
        public DisplayLanguage Language { get; set; } = DisplayLanguage.Vietnamese;
    }
}
