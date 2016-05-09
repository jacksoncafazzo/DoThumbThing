using System;
using System.Configuration;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using RestSharp;
using RestSharp.Authenticators;
using Twilio;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using InstaSharp;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using InstaSharp.Models.Responses;

namespace DoThumbThing.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            byte[] oAuthResponse;
            var lilBool = HttpContext.Session.TryGetValue("InstaSharp.AuthInfo", out oAuthResponse);
            if (!lilBool)
            {
                return RedirectToAction("InstagramLogin");
            } else
            {
                var token = Encoding.ASCII.GetString(oAuthResponse);
                var users = new InstaSharp.Endpoints.Users(config, new OAuthResponse(token));
            }
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
            var sid = "AC8c4f011ae0d7ca6a008f8ab7a2319ebb";
            var pass = "16053c0481c1f57c090ba8ca5f95a84d";

            var client = new RestClient("https://api.twilio.com/2010-04-01");
            var request = new RestRequest("Accounts/" + sid + "/Messages.json", Method.GET);
            client.Authenticator = new HttpBasicAuthenticator(sid, pass);

            var response = client.Execute(request);
            JObject jsonResponse = (JObject)JsonConvert.DeserializeObject(response.Content);
            ViewBag.messages = jsonResponse["messages"];//JsonConvert.DeserializeObject<List<Message>>(jsonResponse["messages"].ToString());
           
            return View("Message");
        }

        public IActionResult Zillo()
        {
            var zid = "X1-ZWz1f9dgwvdq8b_44w9p";
            var client = new RestClient("http://www.zillo.com/webservice/");
            var request = new RestRequest("GetSearchResults.htm?zws-id=" + zid + "&address=400 sw 6th avenue" + "&citystatezip=Portland%2C+OR", Method.GET);
            var response = client.Execute(request);


            return Content(response.Content);
        }
         
        InstagramConfig config = new InstagramConfig(ConfigurationManager.AppSettings["clientid"], ConfigurationManager.AppSettings["clientsecret"], ConfigurationManager.AppSettings["uri"]);

        public IActionResult InstagramLogin()
        {
            var scopes = new List<OAuth.Scope>();
            scopes.Add(InstaSharp.OAuth.Scope.Likes);
            scopes.Add(InstaSharp.OAuth.Scope.Comments);
            var link = InstaSharp.OAuth.AuthLink(config.OAuthUri + "authorize", config.ClientId, config.RedirectUri, scopes, InstaSharp.OAuth.ResponseType.Code);
            return Redirect(link);
        }

        public async Task<IActionResult> OAuth(string code)
        {
            //add this code to the auth object
            var auth = new OAuth(config);

            //now call back to instgram and include the code we got along with client secret
            var oauthResponse = await auth.RequestToken(code);

            // both the client secret and token are considered 
            HttpContext.Session.Set("InstaSharp.AuthInfo", Encoding.ASCII.GetBytes(oauthResponse.AccessToken));

            return View("Index");

        }
    }
}
