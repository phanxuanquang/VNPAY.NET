using System.ComponentModel;

namespace VNPAY.Models.Enums
{
    /// <summary>
    /// Mã phản hồi qua IPN và Callback URL
    /// </summary>
    public enum ResponseCode : sbyte
    {
        /// <summary>
        /// Giao dịch thành công
        /// </summary>
        [Description("Giao dịch thành công")]
        Code_00 = 0,

        /// <summary>
        /// Trừ tiền thành công. Giao dịch bị nghi ngờ (liên quan tới lừa đảo, giao dịch bất thường).
        /// </summary>
        [Description("Trừ tiền thành công. Giao dịch bị nghi ngờ (liên quan tới lừa đảo, giao dịch bất thường)")]
        Code_07 = 7,

        /// <summary>
        /// Thẻ hoặc tài khoản chưa đăng ký dịch vụ Internet Banking tại ngân hàng.
        /// </summary>
        [Description("Thẻ hoặc tài khoản chưa đăng ký dịch vụ Internet Banking tại ngân hàng")]
        Code_09 = 9,

        /// <summary>
        /// Xác thực thông tin thẻ hoặc tài khoản không đúng quá 3 lần
        /// </summary>
        [Description("Xác thực thông tin thẻ hoặc tài khoản không đúng quá 3 lần")]
        Code_10 = 10,

        /// <summary>
        /// Hết hạn chờ thanh toán.
        /// </summary>
        [Description("Hết hạn chờ thanh toán")]
        Code_11 = 11,

        /// <summary>
        /// Thẻ hoặc tài khoản ngân hàng của quý khách bị khóa.
        /// </summary>
        [Description("Thẻ hoặc tài khoản ngân hàng của quý khách bị khóa")]
        Code_12 = 12,

        /// <summary>
        /// Mã OTP không chính xác.
        /// </summary>
        [Description("Mã OTP không chính xác")]
        Code_13 = 13,

        /// <summary>
        /// Giao dịch bị hủy bởi người dùng.
        /// </summary>
        [Description("Giao dịch bị hủy bởi người dùng")]
        Code_24 = 24,

        /// <summary>
        /// Tài khoản của không đủ số dư để thực hiện giao dịch.
        /// </summary>
        [Description("Tài khoản của không đủ số dư để thực hiện giao dịch")]
        Code_51 = 51,

        /// <summary>
        /// Tài khoản đã vượt quá hạn mức giao dịch trong ngày.
        /// </summary>
        [Description("Tài khoản đã vượt quá hạn mức giao dịch trong ngày")]
        Code_65 = 65,

        /// <summary>
        /// Ngân hàng thanh toán đang bảo trì.
        /// </summary>
        [Description("Ngân hàng thanh toán đang bảo trì")]
        Code_75 = 75,

        /// <summary>
        /// Nhập sai mật khẩu thanh toán quá số lần quy định.
        /// </summary>
        [Description("Nhập sai mật khẩu thanh toán quá số lần quy định")]
        Code_79 = 79,

        /// <summary>
        /// Lỗi không xác định.
        /// </summary>
        [Description("Lỗi không xác định")]
        Code_99 = 99
    }

}
