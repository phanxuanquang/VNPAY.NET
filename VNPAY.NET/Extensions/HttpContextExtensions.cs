using Microsoft.AspNetCore.Http;

namespace VNPAY.Extensions
{
    public static class HttpContextExtensions
    {
        /// <summary>
        /// Lấy địa chỉ IP từ HttpContext của API Controller.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetIpAddress(this HttpContext context)
        {
            var remoteIpAddress = context.Connection.RemoteIpAddress ?? throw new InvalidOperationException("Không tìm thấy địa chỉ IP");

            if (remoteIpAddress.IsIPv4MappedToIPv6)
            {
                return remoteIpAddress.MapToIPv4().ToString();
            }

            return remoteIpAddress.ToString();
        }
    }
}
