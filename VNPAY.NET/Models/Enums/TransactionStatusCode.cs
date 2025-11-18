namespace VNPAY.Models.Enums
{
    using System.ComponentModel;

    /// <summary>
    /// Mã phản hồi trạng thái giao dịch từ VNPAY</see>.
    /// </summary>
    public enum TransactionStatusCode : sbyte
    {
        /// <summary>
        /// Giao dịch thành công.
        /// </summary>
        [Description("Giao dịch thành công")]
        Code_00 = 0,

        /// <summary>
        /// Giao dịch chưa hoàn tất.
        /// </summary>
        [Description("Giao dịch chưa hoàn tất")]
        Code_01 = 1,

        /// <summary>
        /// Giao dịch bị lỗi.
        /// </summary>
        [Description("Giao dịch bị lỗi")]
        Code_02 = 2,

        /// <summary>
        /// Khách hàng đã bị trừ tiền tại ngân hàng nhưng giao dịch chưa thành công ở VNPAY
        /// </summary>
        [Description("Khách hàng đã bị trừ tiền tại ngân hàng nhưng giao dịch chưa thành công ở VNPAY")]
        Code_04 = 4,

        /// <summary>
        /// VNPAY đang xử lý giao dịch này (hoàn tiền)
        /// </summary>
        [Description("VNPAY đang xử lý giao dịch này (hoàn tiền)")]
        Code_05 = 5,

        /// <summary>
        /// VNPAY đã gửi yêu cầu hoàn tiền đến ngân hàng (hoàn tiền)
        /// </summary>
        [Description("VNPAY đã gửi yêu cầu hoàn tiền đến ngân hàng (hoàn tiền)")]
        Code_06 = 6,

        /// <summary>
        /// Giao dịch bị nghi ngờ gian lận
        /// </summary>
        [Description("Giao dịch bị nghi ngờ gian lận")]
        Code_07 = 7,

        /// <summary>
        /// Giao dịch hoàn trả bị từ chối
        /// </summary>
        [Description("Giao dịch hoàn trả bị từ chối")]
        Code_09 = 9
    }
}
