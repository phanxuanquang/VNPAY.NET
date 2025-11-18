using System.Collections.Generic;

namespace VNPAY.Models
{
    /// <summary>
    /// Thông tin chi tiết về URL thanh toán được tạo ra để chuyển hướng người dùng tới trang thanh toán của VNPAY.
    /// </summary>
    public class PaymentUrlDetail
    {
        /// <summary>
        /// Mã tham chiếu giao dịch (Transaction Reference). Đây là mã số duy nhất dùng để xác định giao dịch.
        /// </summary>
        public long PaymentId { get; internal set; }

        /// <summary>
        /// URL thanh toán để chuyển hướng người dùng tới trang thanh toán của VNPAY.
        /// </summary>
        public string Url { get; internal set; }

        /// <summary>
        /// Các tham số được sử dụng để tạo URL thanh toán.
        /// </summary>
        public SortedList<string, string> Parameters { get; internal set; }
    }
}
