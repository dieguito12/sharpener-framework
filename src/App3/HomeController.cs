using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mvc;

namespace App3
{
    public class HomeController : BaseController
    {
        public HomeController()
        {
            //
        }
        public IActionResult Index()
        {
            return View(
                "welcome",
                new
                {
                    message = "This is the index method",
                    name = "Sharpener Framework"
                },
                200
            );
        }

        public IActionResult Test()
        {
            var test = @"{""name"": ""this is a test""}";
            return Json(test, 200);
        }

        public IActionResult OtherTest()
        {
            var test = @"{""name"": ""this is another test""}";
            return Json(test, 200);
        }

        public IActionResult Home()
        {
            var test = @"{""name"": ""this is a home""}";
            return Json(test, 200);
        }
    }
}
