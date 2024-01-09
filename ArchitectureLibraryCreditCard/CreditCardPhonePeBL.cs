using ArchitectureLibraryCreditCardModels;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
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
            payloadJson = payloadJson.Replace("##@@MerchantCallBackUrl@@##", phonePePayLoad.MerchantCallBackUrl);
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
        public async Task<PhonePeStatusResult> CheckPhonePePaymentStatus(PhonePePaymentResponseModel responseObj, string restAPIRootUri, string requestUri, string saltKey)
        {
            DateTime startTime = DateTime.Now;

            // First status check at 20 seconds post transaction start
            Thread.Sleep(20000);

            while (true)
            {
                var paymentStatus = await CheckApiStatusAsync(restAPIRootUri, requestUri, responseObj.MerchantId, responseObj.TransactionId, saltKey);

                if (paymentStatus.Code == PhonePeResponseCode.PAYMENT_SUCCESS) return PhonePeStatusResult.Success;
                else if (paymentStatus.Code == PhonePeResponseCode.PAYMENT_PENDING)
                {
                    int elapsedTimeSeconds = (int)(DateTime.Now - startTime).TotalSeconds;

                    if (elapsedTimeSeconds < 30)
                        Thread.Sleep(3000); // Every 3 seconds for the next 30 seconds
                    else if (elapsedTimeSeconds < 90)
                        Thread.Sleep(6000); // Every 6 seconds once for the next 60 seconds
                    else if (elapsedTimeSeconds < 150)
                        Thread.Sleep(10000); // Every 10 seconds for the next 60 seconds
                    else if (elapsedTimeSeconds < 210)
                        Thread.Sleep(30000); // Every 30 seconds for the next 60 seconds
                    else if (elapsedTimeSeconds < 1200)
                        Thread.Sleep(60000); // Every 1 min until timeout (20 mins)
                    else
                    {
                        return PhonePeStatusResult.Timeout;
                    }
                }
                else return PhonePeStatusResult.Failure;
            }
        }
        public async Task<PhonePeCheckPaymentResponse> CheckApiStatusAsync(string restAPIRootUri, string requestUri, string merchantId, string merchantTransactionId, string saltKey)
        {
            var saltIndex = 1;
            var stringToHash = requestUri + "/" + merchantId + "/" + merchantTransactionId + saltKey;
            var sha256 = SHA256Hash(stringToHash);
            var finalXHeader = sha256 + "###" + saltIndex;

            string apiUrl = restAPIRootUri + requestUri + "/" + merchantId + "/" + merchantTransactionId;

            using (HttpClient client = new HttpClient())
            {
                // Create HttpRequestMessage
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, apiUrl);


                // Set other request headers
                //request.Headers.Add("Content-Type", "application/json");
                request.Headers.Add("X-VERIFY", finalXHeader);
                request.Headers.Add("X-MERCHANT-ID", merchantId);

                // Make the API call
                HttpResponseMessage response = await client.SendAsync(request);

                // Check the response
                if (response.IsSuccessStatusCode)
                {
                    // Handle successful response
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<PhonePeCheckPaymentResponse>(responseBody);
                }
                else
                {
                    // Handle error response
                    Console.WriteLine("Error: " + response.StatusCode);
                    throw new Exception(response.StatusCode.ToString());
                }
            }
        }
    }
}
