using VNPAY.Models.Enums;

namespace VNPAY.Models
{
    /// <summary>
    /// Thông tin phản hồi thanh toán
    /// </summary>
    public class PaymentResponse
    {
        /// <summary>
        /// Mã phản hồi từ hệ thống do VNPAY định nghĩa. 
        /// </summary>
        public ResponseCode Code { get; set; }

        /// <summary>
        /// Mô tả chi tiết về mã phản hồi, cung cấp thông tin bổ sung về trạng thái giao dịch.
        /// </summary>
        public string Description { get; set; }
    }
}
