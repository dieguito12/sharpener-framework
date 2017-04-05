using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mvc;
using JWT;
using System.Collections.Specialized;

namespace App1
{
    public class HomeController : BaseController
    {
        public HomeController()
        {
            //
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(
                "welcome", 
                new {
                    message = "This is the index method",
                    name = "Sharpener Framework"
                }, 
                200
            );
        }

        [HttpPost]
        public IActionResult Auth()
        {
            User = new User(HttpRequest.Parameters["username"], HttpRequest.Parameters["password"]);

            NameValueCollection response = new NameValueCollection();
            response.Add("token", User.Token);
            response.Add("username", HttpRequest.Parameters["username"]);
            string json = Mvc.Json.Jsonify(response);

            return Json(json, 200);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Logout()
        {
            UnsetUser();
            NameValueCollection response = new NameValueCollection();
            response.Add("loggedOut", "Ok");
            string json = Mvc.Json.Jsonify(response);
            return Json(json, 200);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Test()
        {
            var test = @"{""name"": ""this is a test""}";
            return Json(test, 200);
        }

        [HttpPost]
        [Authorize]
        public IActionResult OtherTest()
        {
            var test = HttpRequest.Parameters;
            string json = Mvc.Json.Jsonify(test);
            return Json(json, 200);
        }

        [HttpGet]
        public IActionResult Home()
        {
            var test = @"{""name"": ""this is a home""}";
            return Json(test, 200);
        }
    }
}
