using ArchitectureLibraryCreditCardModels;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ArchitectureLibraryCreditCardBusinessLayer
{
    public class CreditCardPhonePeBL
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
        public PhonePeRestResponseObject ProcessPhonePe(string templateFullFileName, PhonePePayLoad phonePePayLoad) 
        {
            string payloadJson;
            StreamReader streamReader = new StreamReader(templateFullFileName);
            payloadJson = streamReader.ReadToEnd();
            streamReader.Close();
            payloadJson = payloadJson.Replace("##@@MerchantId@@##", phonePePayLoad.MerchantId);     
            payloadJson = payloadJson.Replace("##@@MerchantTransactionId@@##", phonePePayLoad.MerchantTransactionId);
            payloadJson = payloadJson.Replace("##@@MerchantUserId@@##", phonePePayLoad.MerchantUserId);
            payloadJson = payloadJson.Replace("##@@CreditCardAmount@@##", phonePePayLoad.CreditCardAmount);
            payloadJson = payloadJson.Replace("##@@MerchantRedirectUrl@@##", phonePePayLoad.MerchantRedirectUrl);
            payloadJson = payloadJson.Replace("##@@MerchantRedirectMode@@##", phonePePayLoad.MerchantRedirectMode);
            payloadJson = payloadJson.Replace("##@@MerchantCallBackUrl@@##", phonePePayLoad.MerchantCallBackUrlId);
            payloadJson = payloadJson.Replace("##@@CustomerMobileNumber@@##", phonePePayLoad.CustomerMobileNumber);
            string base64EncodedPayload = Base64Encode(payloadJson);
            var stringToHash = base64EncodedPayload + phonePePayLoad.RequestUri + phonePePayLoad.SaltKey;
            var sha256 = SHA256Hash(stringToHash);
            var finalXHeader = sha256 + "###" + phonePePayLoad.SaltIndex;
            var options = new RestClientOptions(phonePePayLoad.BaseUrl)
            {
                MaxTimeout = -1,
            };
            var restClient = new RestClient(options);
            //var request = new RestRequest("https://api-preprod.phonepe.com/apis/pg-sandbox/pg/v1/pay", Method.Post);
            var restRequest = new RestRequest(phonePePayLoad.RestAPIRootUri + phonePePayLoad.RequestUri, Method.Post);
            restRequest.AddHeader("Content-Type", "application/json");
            restRequest.AddHeader("X-VERIFY", finalXHeader);
            var body = "{" + "\n" +
                "\"request\":\"" +
                base64EncodedPayload
                + "\"\n" +
                "}";
            restRequest.AddStringBody(body, DataFormat.Json);
            RestResponse restResponse = restClient.Execute(restRequest);
            Console.WriteLine(restResponse.Content);
            if (restResponse.IsSuccessful)
            {
                // Get the content of the response as a string
                string responseContent = restResponse.Content;
                PhonePeRestResponseObject phonePeRestResponseObject = JsonConvert.DeserializeObject<PhonePeRestResponseObject>(responseContent);
                return phonePeRestResponseObject;
            }
            else
            {
                Console.WriteLine($"Request failed with status code: {restResponse.StatusCode}");
                Console.WriteLine($"Error message: {restResponse.ErrorMessage}");
                throw new Exception(restResponse.StatusCode + " " + restResponse.ErrorMessage);
            }
        }

        public void CheckApiStatus(string restAPIRootUri, string requestUri, string merchantId, string merchantTransactionId, string saltKey)
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
}
