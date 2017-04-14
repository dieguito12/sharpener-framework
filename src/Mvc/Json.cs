using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Mvc
{
    public class Json : IActionResult
    {
        string _json;

        int _code;

        public static string Jsonify(NameValueCollection values)
        {
            string json = "{";
            for (int i = 0; i < values.Count; i++)
            {
                json += "\"" + values.GetKey(i) + "\":";
                json += "\"" + values[i] + "\"";
                if (values.Count - i > 1)
                {
                    json += ",";
                }
            }
            json += "}";
            return json;
        }

        public static NameValueCollection Serialize(string json)
        {
            JObject values = (JObject)JsonConvert.DeserializeObject(json);
            NameValueCollection nameValues = new NameValueCollection();
            foreach(KeyValuePair<string, JToken> token in values)
            {
                nameValues.Add(token.Key, (string)token.Value);
            }
            return nameValues;
        }

        public Json(string json, int code)
        {
            _json = json;
            _code = code;
        }

        public MemoryStream Response()
        {
            byte[] response = Encoding.ASCII.GetBytes(_json);
            return new MemoryStream(response);
        }

        public int Code()
        {
            return _code;
        }

        public string ContentType()
        {
            return "application/json";
        }

        public string Redirection()
        {
            return "";
        }
    }
}
