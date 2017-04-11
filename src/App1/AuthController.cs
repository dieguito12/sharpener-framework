using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mvc;
using System.Collections.Specialized;
using App1.Models;
using MySql.Data.MySqlClient;

namespace App1
{
    public class AuthController : BaseController
    {
        public AuthController()
        {
            //
        }

        [HttpPost]
        public IActionResult Login()
        {
            NameValueCollection response = new NameValueCollection();
            string json;
            using (MySqlConnection connection = new MySqlConnection(HttpContext.ConfigurationManager.ApplicationDatabaseConnection))
            {
                connection.Open();
                using (var context = new Context(connection, false))
                {
                    string username = HttpRequest.Parameters["username"];
                    var user = context.Users
                                        .Where(b => b.Username == username)
                                        .FirstOrDefault();
                    if (user == null)
                    {
                        response.Add("error", "Invalid Username");
                        json = Mvc.Json.Jsonify(response);
                        return Json(json, 400);
                    }
                    if (user.Password != HttpRequest.Parameters["password"])
                    {
                        response.Add("error", "Username and Password don't match");
                        json = Mvc.Json.Jsonify(response);
                        return Json(json, 400);
                    }
                    User = new User(HttpRequest.Parameters["username"], HttpRequest.Parameters["password"]);

                    response.Add("username", HttpRequest.Parameters["username"]);
                    HttpContext.Headers["Auth-Token"] = User.Token;
                    json = Mvc.Json.Jsonify(response);
                    return Json(json, 200);
                }
            }
            
        }

        [HttpPost]
        public IActionResult SessionLogin()
        {
            NameValueCollection response = new NameValueCollection();
            string json;
            using (MySqlConnection connection = new MySqlConnection(HttpContext.ConfigurationManager.ApplicationDatabaseConnection))
            {
                connection.Open();
                using (var context = new Context(connection, false))
                {
                    string username = HttpRequest.Parameters["username"];
                    var user = context.Users
                                        .Where(b => b.Username == username)
                                        .FirstOrDefault();
                    if (user == null)
                    {
                        response.Add("error", "Invalid Username");
                        json = Mvc.Json.Jsonify(response);
                        return Json(json, 400);
                    }
                    if (user.Password != HttpRequest.Parameters["password"])
                    {
                        response.Add("error", "Username and Password don't match");
                        json = Mvc.Json.Jsonify(response);
                        return Json(json, 400);
                    }
                    User = new User(HttpRequest.Parameters["username"], HttpRequest.Parameters["password"]);
                    string session = Mvc.Session.GenerateAuthSession(User, HttpContext.ConfigurationManager.ApplicationSecretKey);
                    HttpContext.Session = session;
                    response.Add("username", HttpRequest.Parameters["username"]);
                    json = Mvc.Json.Jsonify(response);
                    return Json(json, 200);
                }
            }

        }

        [HttpPost]
        public IActionResult Register()
        {
            string json;
            NameValueCollection response = new NameValueCollection();
            if (HttpRequest.Parameters["password"] != HttpRequest.Parameters["passwordConfirmation"])
            {
                response.Add("error", "The password and the password confirmation don't match");
                return Json(Mvc.Json.Jsonify(response), 400);
            }
            try
            {
                using (MySqlConnection connection = new MySqlConnection(HttpContext.ConfigurationManager.ApplicationDatabaseConnection))
                {
                    connection.Open();
                    using (var context = new Context(connection, false))
                    {
                        
                        string username = HttpRequest.Parameters["username"];
                        string password = HttpRequest.Parameters["password"];
                        var exist = context.Users
                                        .Where(b => b.Username == username)
                                        .FirstOrDefault();
                        if (exist != null)
                        {
                            response.Add("error", "This username has been already taken.");
                            json = Mvc.Json.Jsonify(response);
                            return Json(json, 400);
                        }
                        var user = new UserModel { Username = username, Password = password };
                        context.Users.Add(user);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                response.Add("error", "Internal Error: " + e.Message);
                json = Mvc.Json.Jsonify(response);
                return Json(json, 500);
            }
            User = new User(HttpRequest.Parameters["username"], HttpRequest.Parameters["password"]);

            response.Add("created", "ok");
            HttpContext.Headers["Auth-Token"] = User.Token;
            response.Add("username", HttpRequest.Parameters["username"]);
            json = Mvc.Json.Jsonify(response);

            return Json(json, 201);
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
    }
}
