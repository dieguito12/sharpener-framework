using JWT;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mvc
{
    public class BaseController
    {
        private User _user;

        public HttpContext HttpContext
        {
            get;
            set;
        }

        public Request HttpRequest
        {
            get;
            set;
        }

        public Route Route
        {
            get;
            set;
        }

        public Dictionary<string, object> Session
        {
            get;
            set;
        }

        public User User
        {
            get
            {
                return _user;
            }
            set
            {
                _user = value;
                if (HttpContext.ConfigurationManager.ApplicationAuthenticationDriver == "jwt")
                {
                    var now = DateTime.Now.AddMinutes(30);
                    var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    var secondsSinceEpoch = Math.Round((now - unixEpoch).TotalSeconds);
                    var payload = new Dictionary<string, object>
                    {
                        { "username", value.Username },
                        { "password", value.Password },
                        { "exp", secondsSinceEpoch }
                    };
                    var token = JsonWebToken.Encode(payload, HttpContext.ConfigurationManager.ApplicationSecretKey, JwtHashAlgorithm.HS256);
                    _user.Token = token;
                    JObject json = JObject.Parse(File.ReadAllText(@"C:\Users\dieguito12\Code\sharpener-framework\src\Mvc\Tokens\tokens.json"));
                    dynamic jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject(json.ToString());
                    JArray allTokens = jsonObject.tokens;
                    JToken newToken = (JToken)token;
                    allTokens.Add(newToken);
                    jsonObject.tokens = allTokens;
                    var modifiedJsonString = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObject);
                    File.WriteAllText(@"C:\Users\dieguito12\Code\sharpener-framework\src\Mvc\Tokens\tokens.json", modifiedJsonString);
                }
            }
        }

        public void UnsetUser()
        {
            JObject json = JObject.Parse(File.ReadAllText(@"C:\Users\dieguito12\Code\sharpener-framework\src\Mvc\Tokens\tokens.json"));
            dynamic jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject(json.ToString());
            JArray allTokens = jsonObject.tokens;
            List<string> tokens = new List<string>();
            foreach (JValue authToken in allTokens)
            {
                tokens.Add(authToken.ToString());
            }
            if (tokens.Contains(User.Token))
            {
                tokens.Remove(User.Token);
            }
            allTokens = new JArray(tokens);
            jsonObject.tokens = allTokens;
            var modifiedJsonString = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObject);
            File.WriteAllText(@"C:\Users\dieguito12\Code\sharpener-framework\src\Mvc\Tokens\tokens.json", modifiedJsonString);
            _user = null;
        }

        public BaseController()
        {
            HttpContext = new HttpContext();
        }

        internal void SetUser(User user)
        {
            _user = user;
        }

        public IActionResult Json(string json, int code)
        {
            return new Json(json, code);
        }

        public IActionResult Content(string text, int code, string redirect)
        {
            return new Content(text, code, redirect);
        }

        public IActionResult View(string template, object data, int code)
        {
            return new View(template, data, code, HttpContext.PhysicalPath, Route);
        }
    }
}
