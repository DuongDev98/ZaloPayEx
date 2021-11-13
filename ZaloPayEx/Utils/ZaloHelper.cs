using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ZaloPayEx
{
    //Zalo helper
    public enum ZaloPayHMAC
    {
        HMACMD5,
        HMACSHA1,
        HMACSHA256,
        HMACSHA512
    }
    public class ZaloPayHmacHelper
    {
        public static string Compute(ZaloPayHMAC algorithm = ZaloPayHMAC.HMACSHA256, string key = "", string message = "")
        {
            byte[] keyByte = System.Text.Encoding.UTF8.GetBytes(key);
            byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(message);
            byte[] hashMessage = null;

            switch (algorithm)
            {
                case ZaloPayHMAC.HMACMD5:
                    hashMessage = new HMACMD5(keyByte).ComputeHash(messageBytes);
                    break;
                case ZaloPayHMAC.HMACSHA1:
                    hashMessage = new HMACSHA1(keyByte).ComputeHash(messageBytes);
                    break;
                case ZaloPayHMAC.HMACSHA256:
                    hashMessage = new HMACSHA256(keyByte).ComputeHash(messageBytes);
                    break;
                case ZaloPayHMAC.HMACSHA512:
                    hashMessage = new HMACSHA512(keyByte).ComputeHash(messageBytes);
                    break;
                default:
                    hashMessage = new HMACSHA256(keyByte).ComputeHash(messageBytes);
                    break;
            }
            return BitConverter.ToString(hashMessage).Replace("-", "").ToLower();
        }
    }

    public class ZaloPayHttpHelper
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public static async Task<T> PostAsync<T>(string uri, HttpContent content)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var response = await httpClient.PostAsync(uri, content);
            var responseString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseString);
        }

        public static Task<Dictionary<string, object>> PostAsync(string uri, HttpContent content)
        {
            return PostAsync<Dictionary<string, object>>(uri, content);
        }

        public static Task<T> PostFormAsync<T>(string uri, Dictionary<string, string> data)
        {
            return PostAsync<T>(uri, new FormUrlEncodedContent(data));
        }

        public static Task<Dictionary<string, object>> PostFormAsync(string uri, Dictionary<string, string> data)
        {
            return PostFormAsync<Dictionary<string, object>>(uri, data);
        }

        public static async Task<T> GetJson<T>(string uri)
        {
            var response = await httpClient.GetAsync(uri);
            var responseString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseString);
        }

        public static Task<Dictionary<string, object>> GetJson(string uri)
        {
            return GetJson<Dictionary<string, object>>(uri);
        }
    }
    public class ZaloPayUtils
    {
        public static long GetTimeStamp(DateTime date)
        {
            return (long)(date.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
        }

        public static long GetTimeStamp()
        {
            return GetTimeStamp(DateTime.Now);
        }
    }
}
