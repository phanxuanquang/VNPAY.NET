﻿using System.Net;
using System.Text;

namespace VNPAY.NET.Utilities
{
    internal class PaymentHelper
    {
        private readonly SortedList<string, string> _requestData = new(new Comparer());
        private readonly SortedList<string, string> _responseData = new(new Comparer());

        internal void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _requestData.Add(key, value);
            }
        }

        internal void AddResponseData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _responseData.Add(key, value);
            }
        }

        internal string GetResponseValue(string key)
        {
            return _responseData.TryGetValue(key, out var value) ? value : string.Empty;
        }

        internal string GetPaymentUrl(string baseUrl, string hashSecret)
        {
            var queryBuilder = new StringBuilder();

            foreach (var (key, value) in _requestData.Where(kv => !string.IsNullOrEmpty(kv.Value)))
            {
                queryBuilder.Append($"{WebUtility.UrlEncode(key)}={WebUtility.UrlEncode(value)}&");
            }

            if (queryBuilder.Length > 0)
            {
                queryBuilder.Length--;
            }

            var signData = queryBuilder.ToString();

            var secureHash = Encoder.AsHmacSHA512(hashSecret, signData);

            return $"{baseUrl}?{signData}&vnp_SecureHash={WebUtility.UrlEncode(secureHash)}";
        }


        internal bool IsSignatureValid(string inputHash, string secretKey)
        {
            var rspRaw = GetResponseData();
            var checksum = Encoder.AsHmacSHA512(secretKey, rspRaw);
            return checksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }

        internal string GetResponseData()
        {
            _responseData.Remove("vnp_SecureHashType");
            _responseData.Remove("vnp_SecureHash");

            var validData = _responseData
                .Where(kv => !string.IsNullOrEmpty(kv.Value))
                .Select(kv => $"{WebUtility.UrlEncode(kv.Key)}={WebUtility.UrlEncode(kv.Value)}");

            return string.Join("&", validData);
        }

    }
}
