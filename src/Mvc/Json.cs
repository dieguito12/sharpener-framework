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
    /// <summary>
    /// Representative class of a json response
    /// </summary>
    public class Json : IActionResult
    {
        /// <summary>
        /// A string representing the json to respond.
        /// </summary>
        string _json;

        /// <summary>
        /// An int representing the status code.
        /// </summary>
        int _code;

        /// <summary>
        /// An static method that turns a NameValueCollection into a json string.
        /// </summary>
        /// <param name="values">NameValueCollection with the json values</param>
        /// <returns>A string representing a json.</returns>
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

        /// <summary>
        /// An static method that turns a json string into a NameValueCollection.
        /// </summary>
        /// <param name="json">A string representing a json</param>
        /// <returns>A NameValueCollection with the json data.</returns>
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

        /// <summary>
        /// Constructor of the Json class.
        /// </summary>
        /// <param name="json">A string representing the json to respond.</param>
        /// <param name="code">An int representing the status code.</param>
        public Json(string json, int code)
        {
            _json = json;
            _code = code;
        }

        /// <summary>
        /// Gets the response of the controller in a stream.
        /// </summary>
        /// <returns>MemoryStream that represents the response of the controller.</returns>
        public MemoryStream Response()
        {
            byte[] response = Encoding.ASCII.GetBytes(_json);
            return new MemoryStream(response);
        }

        /// <summary>
        /// Gets the status code of the response.
        /// </summary>
        /// <returns>An int representing the status code</returns>
        public int Code()
        {
            return _code;
        }

        /// <summary>
        /// Gets the content type of the response.
        /// </summary>
        /// <returns>A string representing the content type of the response</returns>
        public string ContentType()
        {
            return "application/json";
        }

        /// <summary>
        /// Gets the url where the response is going to redirect (if there is any).
        /// </summary>
        /// <returns>A string representing the redirection url</returns>
        public string Redirection()
        {
            return "";
        }
    }
}
