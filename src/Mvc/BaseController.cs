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
    /// <summary>
    /// Representative class of a base controller
    /// </summary>
    public class BaseController
    {
        /// <summary>
        /// Current logged in user.
        /// </summary>
        private User _user;

        /// <summary>
        /// A context information from the http server.
        /// </summary>
        public HttpContext HttpContext
        {
            get;
            set;
        }

        /// <summary>
        /// The incoming request from the server.
        /// </summary>
        public Request HttpRequest
        {
            get;
            set;
        }

        /// <summary>
        /// The current route that is being used.
        /// </summary>
        public Route Route
        {
            get;
            set;
        }

        /// <summary>
        /// A session dictionary to store volatile data.
        /// </summary>
        public Dictionary<string, object> Session
        {
            get;
            set;
        }

        /// <summary>
        /// Gets of sets the current user
        /// </summary>
        /// <remarks>When it is set it is validated if is using the jwt driver</remarks>
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

        /// <summary>
        /// Unsets the current user.
        /// </summary>
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

        /// <summary>
        /// Constructor of the BaseController class.
        /// </summary>
        public BaseController()
        {
            HttpContext = new HttpContext();
        }

        /// <summary>
        /// Sets the current user
        /// </summary>
        /// <param name="user">New User object</param>
        /// <remarks>Just internal user</remarks>
        internal void SetUser(User user)
        {
            _user = user;
        }

        /// <summary>
        /// Creates a new Json response
        /// </summary>
        /// <param name="json">A string representing the json to respond.</param>
        /// <param name="code">An int representing the status code.</param>
        /// <returns>A Json object with the response.</returns>
        public IActionResult Json(string json, int code)
        {
            return new Json(json, code);
        }

        /// <summary>
        /// Creates a new Content response
        /// </summary>
        /// <param name="text">A string of raw text of the response</param>
        /// <param name="code">An int representing the status code</param>
        /// <param name="redirect">A string representing the redirection url</param>
        /// <returns>A Content object with the response</returns>
        public IActionResult Content(string text, int code, string redirect)
        {
            return new Content(text, code, redirect);
        }

        /// <summary>
        /// Creates a new View response
        /// </summary>
        /// <param name="template">A string representing the name of the view template.</param>
        /// <param name="data">An object with the data to be passed to the view template.</param>
        /// <param name="code">An int representing the status code.</param>
        /// <returns>A View object with the response data.</returns>
        public IActionResult View(string template, object data, int code)
        {
            return new View(template, data, code, HttpContext.PhysicalPath, Route);
        }
    }
}
