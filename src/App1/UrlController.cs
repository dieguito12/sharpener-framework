using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mvc;
using JWT;
using System.Collections.Specialized;
using App1.Models;
using MySql.Data.MySqlClient;

namespace App1
{
    public class UrlController : BaseController
    {

        public UrlController()
        {
            //
        }

        [HttpGet]
        [Authorize]
        public IActionResult Urls()
        {
            using (MySqlConnection connection = new MySqlConnection(HttpContext.ConfigurationManager.ApplicationDatabaseConnection))
            {
                connection.Open();
                using (var context = new Context(connection, false))
                {

                    string username = User.Username;
                    var user = context.Users
                                    .Where(b => b.Username == username)
                                    .FirstOrDefault();
                    var urls = user.Urls;
                    NameValueCollection response = new NameValueCollection();
                    string json = "[";
                    for(int i = 0; i < urls.Count; i++)
                    {
                        NameValueCollection value = new NameValueCollection();
                        value.Add("id", urls[i].Id.ToString());
                        value.Add("url", urls[i].Url);
                        value.Add("shortUrl", urls[i].Short);
                        value.Add("clicks", urls[i].Clicks.ToString());
                        value.Add("lastClick", urls[i].LastClick.ToString("yyyy-MM-dd HH:mm:ss"));
                        value.Add("created", urls[i].Created.ToString("yyyy-MM-dd HH:mm:ss"));
                        value.Add("location", urls[i].Location);
                        json += Mvc.Json.Jsonify(value);
                        if (urls.Count - i > 1)
                        {
                            json += ",";
                        }
                    }
                    json += "]";
                    HttpContext.Headers["Auth-Token"] = User.Token;
                    return Json(json, 200);
                }
            }
        }
    }
}
