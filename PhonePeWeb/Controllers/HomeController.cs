using ArchitectureLibraryCreditCardBusinessLayer;
using ArchitectureLibraryCreditCardModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PhonePeWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult PhonePeTimeOut()
        {
            ViewBag.Message = "Payment has timed Out.";

            return View();
        }

        [HttpGet]
        public ActionResult PhonePe()
        {
            return View();
        }


        [HttpPost]
        public ActionResult PhonePe(string paymentAmount)
        {
            CreditCardPhonePeBL creditCardPhonePeBL = new CreditCardPhonePeBL();
            PhonePePayLoad phonePePayLoad = new PhonePePayLoad
            {
                BaseUrl = "https://api-preprod.phonepe.com/",
                RestAPIRootUri = "https://api-preprod.phonepe.com/apis/pg-sandbox",
                RequestUri = "/pg/v1/pay",
                MerchantId = "PGTESTPAYUAT91",
                MerchantTransactionId = Guid.NewGuid().ToString(),//"b0e9f024-c617-49ce-bb57-70b28fa9ae84",
                MerchantUserId = "MUID123",
                MerchantRedirectMode = "POST",
                CreditCardAmount = paymentAmount,
                //MerchantRedirectUrl = "https://webhook.site/c7c8645a-ddd4-4aa0-89f0-9997b4accbaf",
                MerchantRedirectUrl = "https://localhost:44386/Home/PhonePeReturn",
                CustomerMobileNumber = "9999999999",
                PaymentInstrumentType = "PAY_PAGE",
                SaltKey = "05992a0b-5254-4f37-86fb-e23bb79ea7e7",
                SaltIndex = "1",
                CheckStatusRestAPIRootUri = "",
                CheckStatusRequestUri = "",
                MerchantCallBackUrl = "https://localhost:44386/Home/PhonePeReturnCallBack",
            };
            string templateFullFileName = Server.MapPath("~/") + "PhonePePayPagePayLoadTemplate.json";
            PhonePeRestResponseObject phonePeRestResponseObject = creditCardPhonePeBL.ProcessPhonePe(templateFullFileName, phonePePayLoad);
            return Json(phonePeRestResponseObject);
        }


        public delegate Task<PhonePeStatusResult> CheckPaymentStatusDelegate(PhonePePaymentResponseModel responseObj, string restAPIRootUri, string requestUri, string saltKey);

        [HttpPost]
        public async Task<ActionResult> PhonePeReturn(PhonePePaymentResponseModel responseObj)
        {
            if (false) // Check (from database or cache) if server to server has been recieved from PhonePe server.
            {
                // Decide if transaction is success or failure
            }
            else // This assumes server to server response hasn't been recieved. So, transaction status has to be checked.
            {
                CreditCardPhonePeBL creditCardPhonePeBL = new CreditCardPhonePeBL();
                var paymentStatus = await creditCardPhonePeBL.CheckApiStatusAsync("https://api-preprod.phonepe.com/apis/pg-sandbox", "/pg/v1/status", responseObj.MerchantId, responseObj.TransactionId, "05992a0b-5254-4f37-86fb-e23bb79ea7e7");
                if (paymentStatus.Code == PhonePeResponseCode.PAYMENT_PENDING)
                {
                    Task.Run(async () =>
                    {
                        var result = await creditCardPhonePeBL.CheckPhonePePaymentStatus(responseObj, "https://api-preprod.phonepe.com/apis/pg-sandbox", "/pg/v1/status", "05992a0b-5254-4f37-86fb-e23bb79ea7e7");

                        // You can handle the result of the background task if needed
                        Console.WriteLine($"Background Task Result: {result}");
                    });

                }
                return View(paymentStatus);
            }
        }

        [HttpPost]
        public void PhonePeReturnCallBack(PhonePeServerToServerResponseModel responseObj)
        {
            // Decode the response and save the result to database for future uses.
            // A response should come from PhonePe server after transaction is success or failure
        }
    }
}
