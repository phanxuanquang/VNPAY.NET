using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace VNPAY.Extensions
{
    internal static class StringExtension
    {
        internal static string AsHmacSHA512(this string data, string key)
        {
            if (string.IsNullOrEmpty(data))
            {
                throw new ArgumentNullException(nameof(data));
            }

            var keyBytes = Encoding.UTF8.GetBytes(key);
            var inputBytes = Encoding.UTF8.GetBytes(data);

            using var hmac = new HMACSHA512(keyBytes);
            return BitConverter.ToString(hmac.ComputeHash(inputBytes)).Replace("-", string.Empty);
        }
    }
}
