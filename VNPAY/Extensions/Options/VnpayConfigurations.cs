using System;

namespace VNPAY.Extensions.Options
{
    /// <summary>
    /// Configuration options for VNPAY integration
    /// </summary>
    public class VnpayConfigurations
    {
        /// <summary>
        /// Mã cửa hàng của bạn trên VNPAY.
        /// </summary>
        public string TmnCode { get; set; }

        /// <summary>
        /// Mật khẩu bảo mật dùng để mã hóa và xác thực giao dịch.
        /// </summary>
        public string HashSecret { get; set; }

        /// <summary>
        /// URL của trang web thanh toán.
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// URL mà VNPAY sẽ gọi lại sau khi giao dịch hoàn tất.
        /// </summary>
        public string CallbackUrl { get; set; }

        /// <summary>
        /// Phiên bản của API mà bạn đang sử dụng.
        /// </summary>
        public string Version { get; set; } = "2.1.0";

        /// <summary>
        /// Loại đơn hàng.
        /// </summary>
        public string OrderType { get; set; } = "other";

        internal void EnsureValid()
        {
            if (string.IsNullOrEmpty(TmnCode))
            {
                throw new ArgumentException("TmnCode is required.");
            }

            if (string.IsNullOrEmpty(HashSecret))
            {
                throw new ArgumentException("HashSecret is required.");
            }

            if (string.IsNullOrEmpty(BaseUrl))
            {
                throw new ArgumentException("BaseUrl is required.");
            }

            if (string.IsNullOrEmpty(CallbackUrl))
            {
                throw new ArgumentException("CallbackUrl is required.");
            }
        }
    }
}