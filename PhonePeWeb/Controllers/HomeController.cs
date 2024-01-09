using ArchitectureLibraryCreditCardBusinessLayer;
using ArchitectureLibraryCreditCardModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

        [HttpGet]
        public ActionResult PhonePe()
        {
            //CreditCardPhonePeBL_Old creditCardPhonePeBL = new CreditCardPhonePeBL_Old();
            //string templateFileName, baseUrl, restAPIRootUri, requestUri;
            //baseUrl = "https://api-preprod.phonepe.com/";
            //restAPIRootUri = "https://api-preprod.phonepe.com/apis/pg-sandbox/";
            //requestUri = "pg/v1/pay";
            //templateFileName = Server.MapPath("~/") + "PayPagePayLoad.json";
            //creditCardPhonePeBL.ProcessPayLoad(templateFileName, baseUrl, restAPIRootUri, requestUri);
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
                MerchantCallBackUrl = "https://localhost:44386/Home/PhonePeReturn",
            };
            string templateFullFileName = Server.MapPath("~/") + "PhonePePayPagePayLoadTemplate.json";
            PhonePeRestResponseObject phonePeRestResponseObject = creditCardPhonePeBL.ProcessPhonePe(templateFullFileName, phonePePayLoad);
            // Response.Redirect(responseObj.Data.InstrumentResponse.RedirectInfo.Url);
            // return Json(responseObj);
            return Json(phonePeRestResponseObject);
        }

        [HttpPost]
        public ActionResult PhonePeReturn(FormCollection formCollection)
        {
            // form["FirstName"] 
            return View();
        }
    }
}
