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
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : FilterAttribute
    {
        
        public User AuthorizeAuthToken(Request request, string secret)
        {
            string token = request.Headers["Auth-Token"];
            if (token == null || token == "")
            {
                return null;
            }
            var json = JsonWebToken.Decode(token, secret, false);
            NameValueCollection values = Json.Serialize(json);
            var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var now = Math.Round((DateTime.Now - unixEpoch).TotalSeconds);
            if (values["exp"] != null && values["exp"] != "")
            {
                if (long.Parse(values["exp"]) < now)
                {
                    return null;
                }
            }

            JObject jsonFile = JObject.Parse(File.ReadAllText(@"C:\Users\dieguito12\Code\sharpener-framework\src\Mvc\Tokens\tokens.json"));
            List<string> tokens = new List<string>();
            JArray authTokens = (JArray)jsonFile["tokens"];
            foreach (JValue authToken in authTokens)
            {
                tokens.Add(authToken.ToString());
            }

            if (tokens.Contains(token))
            {
                User user = new User(values["username"], values["password"]);
                user.Token = token;
                return user;
            }
            return null;
        }

        public User AuthorizeSession(string session)
        {
            return Session.AuthenticateSession(session);
        }
    }
}
