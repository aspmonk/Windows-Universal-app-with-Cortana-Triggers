using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;

namespace MR_Robot.AzureService
{
    public class MessageService
    {

        public static void SendSBMessage(string message)
        {
            try
            {
                string baseUri = "https://mrrobot.servicebus.windows.net";
                using (System.Net.Http.HttpClient client = new System.Net.Http.HttpClient())
                {
                    client.BaseAddress = new Uri(baseUri);
                    client.DefaultRequestHeaders.Accept.Clear();

                    string token = SASTokenHelper();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("SharedAccessSignature", token);

                    string json = JsonConvert.SerializeObject(message);
                    HttpContent content = new StringContent(json, Encoding.UTF8);
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    content.Headers.Add("led", message);
                    string path = "/lighttopic/messages";

                    var response = client.PostAsync(path, content).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        // Do something
                        // Debug.WriteLine("Success!");
                    }
                    else
                    {
                        // Debug.WriteLine("Failure!" + response);
                    }

                }
            }
            catch (Exception ex)
            {
                // Debug.WriteLine("ERORR!" + ex.ToString());
            }
        }

        private static string SASTokenHelper()
        {
            // Endpoint = sb://mrrobot.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=MpcCuDqNeq3kcr3AnElZNMTwBxXVqqHWrsuRGHeB9KI=
            string keyName = "RootManageSharedAccessKey";
            string key = "MpcCuDqNeq3kcr3AnElZNMTwBxXVqqHWrsuRGHeB9KI=";
            string uri = "mrrobot.servicebus.windows.net";

            int expiry = (int)DateTime.UtcNow.AddMinutes(20).Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            string stringToSign = WebUtility.UrlEncode(uri) + "\n" + expiry.ToString();
            string signature = HmacSha256(key, stringToSign);
            string token = String.Format("sr={0}&sig={1}&se={2}&skn={3}", WebUtility.UrlEncode(uri), WebUtility.UrlEncode(signature), expiry, keyName);

            return token;
        }

        // Because Windows.Security.Cryptography.Core.MacAlgorithmNames.HmacSha256 doesn't
        // exist in WP8.1 context we need to do another implementation
        public static string HmacSha256(string key, string value)
        {
            var keyStrm = CryptographicBuffer.ConvertStringToBinary(key, BinaryStringEncoding.Utf8);
            var valueStrm = CryptographicBuffer.ConvertStringToBinary(value, BinaryStringEncoding.Utf8);

            var objMacProv = MacAlgorithmProvider.OpenAlgorithm(MacAlgorithmNames.HmacSha256);
            var hash = objMacProv.CreateHash(keyStrm);
            hash.Append(valueStrm);

            return CryptographicBuffer.EncodeToBase64String(hash.GetValueAndReset());
        }


    }
}
