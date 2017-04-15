using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JWT;
using System.Collections.Specialized;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Mvc
{
    /// <summary>
    /// Representative class of an authorization attribute for the controllers methods.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class AuthorizeAttribute : FilterAttribute
    {
        /// <summary>
        /// Authorize an incoming Auth Token and returns the corresponded user or null if is not valid.
        /// </summary>
        /// <param name="request">A Request instance with the incoming request information.</param>
        /// <param name="secret">A string representing the secret key to encode and decode the token.</param>
        /// <returns>Corresponded User of the token, or null if is not valid</returns>
        public User AuthorizeAuthToken(Request request, string secret)
        {
            string token = request.Headers["Auth-Token"];
            if (token == null || token == "")
            {
                return null;
            }
            var json = JsonWebToken.Decode(token, secret, false);
            NameValueCollection values = Json.Serialize(json);

            string newToken = Session.RefreshToken(10, secret, token);

            JObject jsonFile = JObject.Parse(File.ReadAllText(@"C:\Users\dieguito12\Code\sharpener-framework\src\Mvc\Tokens\tokens.json"));
            List<string> tokens = new List<string>();
            JArray authTokens = (JArray)jsonFile["tokens"];

            foreach (JValue authToken in authTokens)
            {
                tokens.Add(authToken.ToString());
            }

            if (tokens.Contains(newToken))
            {
                User user = new User(values["username"], values["password"]);
                user.Token = newToken;
                return user;
            }
            return null;
        }

        /// <summary>
        /// Authorize session and returns the corresponded user or null if is not valid.
        /// </summary>
        /// <param name="session">A string the presenting the session</param>
        /// <returns>Corresponded User of the token, or null if is not valid</returns>
        public User AuthorizeSession(string session)
        {
            return Session.AuthenticateSession(session);
        }
    }
}
