using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace PhonePeWeb
{
    public class CreditCardPhonePeBL_Old
    {
        public string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
        private string SHA256Hash(string randomString)
        {
            var crypt = new SHA256Managed();
            string hash = String.Empty;
            byte[] crypto = crypt.ComputeHash(Encoding.ASCII.GetBytes(randomString));
            foreach (byte theByte in crypto)
            {
                hash += theByte.ToString("x2");
            }
            return hash;
        }
        public PhonePeRestResponseObject_Old ProcessPayLoad(string templateFileName, string baseUrl, string restAPIRootUri, string requestUri)
        {
            string payloadJson;
            StreamReader streamReader = new StreamReader(templateFileName);
            payloadJson = streamReader.ReadToEnd();
            streamReader.Close();
            string merchantId = "PGTESTPAYUAT91";
            var merchantTransactionId = Guid.NewGuid().ToString();
            payloadJson = payloadJson.Replace("<merchantTransactionId>", merchantTransactionId);
            string base64EncodedPayload = Base64Encode(payloadJson);
            var saltKey = "05992a0b-5254-4f37-86fb-e23bb79ea7e7";
            var saltIndex = 1;
            var stringToHash = base64EncodedPayload + "/pg/v1/pay" + saltKey;
            var sha256 = SHA256Hash(stringToHash);
            var finalXHeader = sha256 + "###" + saltIndex;
            var options = new RestClientOptions(baseUrl)
            {
                MaxTimeout = -1,
            };
            var restClient = new RestClient(options);
            //var request = new RestRequest("https://api-preprod.phonepe.com/apis/pg-sandbox/pg/v1/pay", Method.Post);
            var restRequest = new RestRequest(restAPIRootUri + requestUri, Method.Post);
            restRequest.AddHeader("Content-Type", "application/json");
            restRequest.AddHeader("X-VERIFY", finalXHeader);
            var body = "{" + "\n" +
                "\"request\":\"" +
                base64EncodedPayload
                + "\"\n" +
                "}";
            restRequest.AddStringBody(body, DataFormat.Json);
            RestResponse restResponse = restClient.Execute(restRequest);
            /*
            1. Redirect to the info / url
            2. Check Api Status - In a loop
            3. Create a new page in our project
            4. Pass this in the original payload
            5. We should come back here
            6. Check API status probably should be stopped
            */
            Console.WriteLine(restResponse.Content);
            if (restResponse.IsSuccessful)
            {
                // Get the content of the response as a string
                string responseContent = restResponse.Content;
                PhonePeRestResponseObject_Old responseObject = JsonConvert.DeserializeObject<PhonePeRestResponseObject_Old>(responseContent);
                return responseObject;
            }
            else
            {
                Console.WriteLine($"Request failed with status code: {restResponse.StatusCode}");
                Console.WriteLine($"Error message: {restResponse.ErrorMessage}");
                throw new Exception(restResponse.StatusCode + " " +restResponse.ErrorMessage);
            }
            //CheckApiStatus(restAPIRootUri, "pg/v1/status/", merchantId, merchantTransactionId, saltKey);
        }
        private void CheckApiStatus(string restAPIRootUri, string requestUri, string merchantId, string merchantTransactionId, string saltKey)
        {
            var saltIndex = 1;
            var stringToHash = restAPIRootUri + requestUri + merchantId + "/" + merchantTransactionId + "/status" + saltKey;
            var sha256 = SHA256Hash(stringToHash);
            var finalXHeader = sha256 + "###" + saltIndex;

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(restAPIRootUri);

            webRequest.Method = "GET";
            webRequest.ContentType = "application/json";
            webRequest.Headers.Add("x-verify", finalXHeader);

            string responseData = string.Empty;

            using (StreamReader responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream()))
            {
                responseData = responseReader.ReadToEnd();
                if (responseData.Length > 0)
                {
                    //PhonePeStatusResponseBody responseBody = JsonConvert.DeserializeObject<PhonePeStatusResponseBody>(responseData);
                    Console.WriteLine(responseData);
                    //Console.WriteLine(responseBody.message);
                }
            }
        }
    }
    public class PhonePeInstrumentResponse
    {
        public string Type { get; set; }
        public PhonePeRedirectInfo RedirectInfo { get; set; }
    }

    public class PhonePeRedirectInfo
    {
        public string Url { get; set; }
        public string Method { get; set; }
    }

    public class PhonePeData
    {
        public string MerchantId { get; set; }
        public string MerchantTransactionId { get; set; }
        public PhonePeInstrumentResponse InstrumentResponse { get; set; }
    }

    public class PhonePeRestResponseObject_Old
    {
        public bool Success { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
        public PhonePeData Data { get; set; }
    }

}
