using System.ComponentModel;

namespace VNPAY.Models.Enums
{
    /// <summary>
    /// Đơn vị tiền tệ sử dụng cho giao dịch
    /// </summary>
    internal enum Currency
    {
        /// <summary>
        /// Việt Nam đồng
        /// </summary>
        [Description("Việt Nam đồng")]
        VND,

        //[Description("Đô la Mỹ")]
        //USD,
    }
}
