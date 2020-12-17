using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using razorPay.Models;
using Razorpay.Api;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace razorPay.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public string orderId;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            Dictionary<string, object> input = new Dictionary<string, object>();
            input.Add("amount", 100); // this amount should be same as transaction amount
            input.Add("currency", "USD");
            input.Add("receipt", "12121");
            input.Add("payment_capture", 1);

            string key = "rzp_test_EdbyY6ssXBY0KD";
            string secret = "x3iIXErqBtpuzSSzqopAgBL1";
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            RazorpayClient client = new RazorpayClient(key, secret);

            Razorpay.Api.Order order = client.Order.Create(input);
            ViewBag.orderId = order["id"].ToString();

            return View();
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

            string key = "rzp_test_EdbyY6ssXBY0KD";
            string secret = "x3iIXErqBtpuzSSzqopAgBL1";

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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
