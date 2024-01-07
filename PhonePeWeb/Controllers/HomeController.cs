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
            //CreditCardPhonePeBL creditCardPhonePeBL = new CreditCardPhonePeBL();
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
                RestAPIRootUri = "https://api-preprod.phonepe.com/apis/pg-sandbox/",
                RequestUri = "pg/v1/pay",
                MerchantId = "PGTESTPAYUAT91",
                MerchantTransactionId = Guid.NewGuid().ToString(),
                MerchantUserId = "MUID123",
                MerchantRedirectMode = "POST",
                CreditCardAmount = "10000",
                MerchantRedirectUrl = "https://webhook.site/c7c8645a-ddd4-4aa0-89f0-9997b4accbaf",
                CustomerMobileNumber = "123456789",
                PaymentInstrumentType = "PAY_PAGE",
                SaltKey = "05992a0b-5254-4f37-86fb-e23bb79ea7e7",
                SaltIndex = "1",
                CheckStatusRestAPIRootUri = "",
                CheckStatusRequestUri = ""
            };
            string templateFullFileName = Server.MapPath("~/") + "PhonePePayPagePayLoadTemplate.json";
            PhonePeRestResponseObject responseObj = creditCardPhonePeBL.ProcessPhonePe(templateFullFileName, phonePePayLoad);
            // Response.Redirect(responseObj.Data.InstrumentResponse.RedirectInfo.Url);
            // return Json(responseObj);
            return Json(responseObj);
        }

        [HttpPost]
        public ActionResult PhonePeReturn(FormCollection formCollection)
        {
            // form["FirstName"] 
            return View();
        }
    }
}
