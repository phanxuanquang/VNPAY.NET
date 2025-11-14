using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Sockets;

namespace VNPAY.NET.Utilities
{
    public class NetworkHelper
    {
        /// <summary>
        /// Lấy địa chỉ IP từ HttpContext của API Controller.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetIpAddress(HttpContext context)
        {
            var remoteIpAddress = context.Connection.RemoteIpAddress;

            if (remoteIpAddress != null)
            {
                if (remoteIpAddress.IsIPv4MappedToIPv6)
                {
                    return remoteIpAddress.MapToIPv4().ToString();
                }

                return remoteIpAddress.ToString();
            }

            throw new InvalidOperationException("Không tìm thấy địa chỉ IP");
        }
    }
}
