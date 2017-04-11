using JWT;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Mvc
{
    public class Session
    {
        private static string _session;

        private static string _sessionDirectory = @"C:\Users\dieguito12\Code\sharpener-framework\src\Mvc\Sessions\";

        public static bool SessionExists(string session)
        {
            return File.Exists(_sessionDirectory + session);
        }

        public static string RefreshToken(int days, string secret, string incomingToken)
        {
            JObject json = JObject.Parse(File.ReadAllText(@"C:\Users\dieguito12\Code\sharpener-framework\src\Mvc\Tokens\tokens.json"));
            dynamic jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject(json.ToString());
            JArray allTokens = jsonObject.tokens;
            JArray aux = new JArray();
            string newToken = null;
            foreach (JValue authToken in allTokens)
            {
                if (authToken.ToString() == incomingToken)
                {
                    var token = JsonWebToken.Decode(authToken.ToString(), secret, false);
                    NameValueCollection values = Json.Serialize(token);
                    var now = DateTime.Now.AddDays(days);
                    var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    var secondsSinceEpoch = Math.Round((now - unixEpoch).TotalSeconds);
                    var payload = new Dictionary<string, object>
                    {
                        { "username", values["username"] },
                        { "password", values["password"] },
                        { "exp", secondsSinceEpoch }
                    };
                    newToken = JsonWebToken.Encode(payload, secret, JwtHashAlgorithm.HS256);
                }
                if (newToken != null)
                {
                    aux.Add((JToken)newToken);
                }
                else
                {
                    aux.Add(authToken);
                }
            }

            jsonObject.tokens = aux;
            var modifiedJsonString = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObject);
            File.WriteAllText(@"C:\Users\dieguito12\Code\sharpener-framework\src\Mvc\Tokens\tokens.json", modifiedJsonString);
            return newToken;
        }

        public static void DeleteExpiredSessions(string secret)
        {
            var sessions = Directory.GetFiles(_sessionDirectory);
            foreach(string session in sessions)
            {
                System.IO.StreamReader file = new System.IO.StreamReader(session);
                string token = file.ReadLine();
                if (token != null)
                {
                    var json = JsonWebToken.Decode(token, secret, false);
                    NameValueCollection values = Json.Serialize(json);
                    var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    var now = Math.Round((DateTime.Now - unixEpoch).TotalSeconds);
                    if (long.Parse(values["exp"]) < now)
                    {
                        file.Dispose();
                        File.Delete(session);
                    }
                }
                else
                {
                    file.Dispose();
                    File.Delete(session);
                }
                file.Dispose();
            }
            
        }

        public static string GetCurrentSession()
        {
            return _session;
        }


        public static User AuthenticateSession(string secret)
        {
            System.IO.StreamReader file = new System.IO.StreamReader(_sessionDirectory + "\\" + _session);
            string session = file.ReadLine();
            var json = JsonWebToken.Decode(session, secret, false);
            NameValueCollection values = Json.Serialize(json);
            var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var now = Math.Round((DateTime.Now - unixEpoch).TotalSeconds);
            if (long.Parse(values["exp"]) < now)
            {
                file.Dispose();
                File.Delete(_session);
                return null;
            }
            else
            {
                file.Dispose();
                RefreshSession(30, secret);
            }
            return new User(values["username"], values["password"]);
        }

        public static void SetSession(string session)
        {
            _session = session;
        }

        public static void RefreshSession(int minutes, string secret)
        {
            System.IO.StreamReader file = new System.IO.StreamReader(_sessionDirectory + "\\" + _session);
            string session = file.ReadLine();
            var json = JsonWebToken.Decode(session, secret, false);
            NameValueCollection values = Json.Serialize(json);
            var now = DateTime.Now.AddMinutes(minutes);
            var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var secondsSinceEpoch = Math.Round((now - unixEpoch).TotalSeconds);

            var payload = new Dictionary<string, object>
            {
                { "username", values["username"] },
                { "password", values["password"] },
                { "exp", secondsSinceEpoch }
            };
            var token = JsonWebToken.Encode(payload, secret, JwtHashAlgorithm.HS256);

            string path = _sessionDirectory + "\\" + _session;

            file.Dispose();
            File.WriteAllText(path, token);
        }
        
        public static string GenerateAuthSession(User user, string secret)
        {
            var now = DateTime.Now.AddMinutes(30);
            var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var secondsSinceEpoch = Math.Round((now - unixEpoch).TotalSeconds);
            var payload = new Dictionary<string, object>
            {
                { "username", user.Username },
                { "password", user.Password },
                { "exp", secondsSinceEpoch }
            };
            Guid g = Guid.NewGuid();
            _session = g.ToString();
            _session = _session.Replace("+", "");
            _session = _session.Replace("-", "");
            _session = _session.Replace("\\", "");
            _session = _session.Replace("/", "");
            _session = _session.Replace(".", "");
            var token = JsonWebToken.Encode(payload, secret, JwtHashAlgorithm.HS256);
            string path = _sessionDirectory + "\\" + _session;
            File.WriteAllText(path, token);
            return _session;
        }
    }
}
