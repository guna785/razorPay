using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using razorPay.Models;
using Razorpay.Api;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace razorPay.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public string key,secret,currency, payment_capture;
       
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            var configBuilder = new ConfigurationBuilder();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            configBuilder.AddJsonFile(path, false);
            var root = configBuilder.Build();
            var conStrConfig = root.GetSection("RazorPay:key");
            var conDB = root.GetSection("RazorPay:secret");
            var cur = root.GetSection("RazorPay:currency");
            var pcapture = root.GetSection("RazorPay:payment_capture");
            key = conStrConfig.Value;
            secret = conDB.Value;
            currency = cur.Value;
            payment_capture = pcapture.Value;
        }

        public IActionResult Index()
        {
            ViewBag.key = key;
            ViewBag.secret = secret;

            return View();
        }
        [HttpPost]
        public IActionResult getOrderID([FromBody] Models.Payment pay)
        {            
            Dictionary<string, object> input = new Dictionary<string, object>();
            input.Add("amount", pay.amt); // this amount should be same as transaction amount
            input.Add("currency", currency);
            input.Add("receipt", GetRandomNumber());
            input.Add("payment_capture", payment_capture);

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            RazorpayClient client = new RazorpayClient(key, secret);

            Razorpay.Api.Order order = client.Order.Create(input);
            var orderId = order["id"].ToString();
            return  Ok(new { orderID = orderId });
        }
        [HttpPost]
        public IActionResult PaymentPost()
        {
            string name = HttpContext.Request.Form["name"];
            string email = HttpContext.Request.Form["email"];
            string contact = HttpContext.Request.Form["phone"];
            string amt = HttpContext.Request.Form["amount"];
            ViewBag.amt = amt;
            ViewBag.name = name;
            ViewBag.email = email;
            ViewBag.contact = contact;
            return View();
        }
        [HttpPost]
        public IActionResult ChechIn()
        {
            string paymentId = HttpContext.Request.Form["razorpay_payment_id"];
            Dictionary<string, object> input = new Dictionary<string, object>();
            input.Add("amount", 100); // this amount should be same as transaction amount

            string key = "rzp_live_mvhUwNIUpJavYy";
            string secret = "RlUy7d10QDYvTBLjoeI9Qku4";

            RazorpayClient client = new RazorpayClient(key, secret);

            Dictionary<string, string> attributes = new Dictionary<string, string>();

            attributes.Add("razorpay_payment_id", paymentId);
            attributes.Add("razorpay_order_id", Request.Form["razorpay_order_id"]);
            attributes.Add("razorpay_signature", Request.Form["razorpay_signature"]);

            Utils.verifyPaymentSignature(attributes);

            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
        [NonAction]
        public string GetRandomNumber()
        {
            using (RNGCryptoServiceProvider rg = new RNGCryptoServiceProvider())
            {
                byte[] rno = new byte[6];
                rg.GetBytes(rno);
                int randomvalue = BitConverter.ToInt32(rno, 0);
                return randomvalue.ToString();
            }
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
