using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using RestSharp;
using RestSharp.Authenticators;
using Twilio;

namespace DoThumbThing.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Send()
        {
            var number = "+1"+Request.Form["number"].ToString();
            var body = Request.Form["message"].ToString();
            ViewBag.message = body;
            ViewBag.number = number;
              var sid = "AC8c4f011ae0d7ca6a008f8ab7a2319ebb";
//            var sid = "AC90096414604b4263def706710f248e29";
            var pass = "16053c0481c1f57c090ba8ca5f95a84d";
//            var pass = "652166b2eb6850575e01317128fb9e3d";

            string from = "+15034867949";
//            var twilio = new TwilioRestClient(sid, pass);
//            var message = twilio.SendMessage(from, "+1" + number, body);
            var client = new RestClient("https://api.twilio.com/2010-04-01/");
            var request = new RestRequest("Accounts/AC8c4f011ae0d7ca6a008f8ab7a2319ebb/Messages.json", Method.POST);

            request.AddParameter("To", number);
            request.AddParameter("From", from);
            request.AddParameter("Body", body);

            client.Authenticator = new HttpBasicAuthenticator(sid, pass);
            var response = client.Execute(request);
            ViewBag.response = response;

            return Message();
        }

        public IActionResult Message()
        {
            return View("Message");
        }
    }
}
