namespace VNPAY.NET.Extensions.Options
{
    /// <summary>
    /// Configuration options for VNPAY integration
    /// </summary>
    public class VnpayOptions
    {
        /// <summary>
        /// Mã cửa hàng của bạn trên VNPAY.
        /// </summary>
        public required string TmnCode { get; set; }

        /// <summary>
        /// Mật khẩu bảo mật dùng để mã hóa và xác thực giao dịch.
        /// </summary>
        public required string HashSecret { get; set; }

        /// <summary>
        /// URL của trang web thanh toán.
        /// </summary>
        public required string BaseUrl { get; set; }

        /// <summary>
        /// URL mà VNPAY sẽ gọi lại sau khi giao dịch hoàn tất.
        /// </summary>
        public required string CallbackUrl { get; set; }

        /// <summary>
        /// Phiên bản của API mà bạn đang sử dụng.
        /// </summary>
        public string Version { get; set; } = "2.1.0";

        /// <summary>
        /// Loại đơn hàng.
        /// </summary>
        public string OrderType { get; set; } = "other";
    }
}