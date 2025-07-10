using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace AdnTestingSystem.Services.Helpers
{
    public class zVnPayLibrary
    {
        private readonly Dictionary<string, string> requestData = new();

        public void AddRequestData(string key, string value)
        {
            requestData[key] = value;
        }

        public string CreateRequestUrl(string baseUrl, string hashSecret)
        {
            var sortedData = requestData.OrderBy(x => x.Key);
            var signData = new StringBuilder();
            var query = new StringBuilder();

            foreach (var item in sortedData)
            {
                if (query.Length > 0)
                {
                    query.Append("&");
                }
                query.Append($"{item.Key}={item.Value}");
            }

            var secureHash = HmacSHA512(hashSecret, query.ToString());

            var paymentUrl = $"{baseUrl}?vnp_SecureHash={secureHash}";

            Console.WriteLine("==== VNPay Debug ====");
            //Console.WriteLine("signData: " + signData.ToString());
            Console.WriteLine("secureHash: " + secureHash);
            Console.WriteLine("Request URL: " + paymentUrl);
            Console.WriteLine("=====================");

            return paymentUrl;
        }

        private string HmacSHA512(string key, string input)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var inputBytes = Encoding.UTF8.GetBytes(input);

            using var hmac = new HMACSHA512(keyBytes);
            var hashBytes = hmac.ComputeHash(inputBytes);

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }

}
