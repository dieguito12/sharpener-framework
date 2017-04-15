using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mvc
{
    /// <summary>
    /// Representative class of a raw content (or redirection) response of a controller
    /// </summary>
    public class Content : IActionResult
    {
        /// <summary>
        /// An int representing the status code
        /// </summary>
        private int _code;

        /// <summary>
        /// A string of raw text of the response
        /// </summary>
        private string _text;

        /// <summary>
        /// A string representing the redirection url
        /// </summary>
        private string _redirect;

        /// <summary>
        /// Constructor of the Content class
        /// </summary>
        /// <param name="text">A string of raw text of the response</param>
        /// <param name="code">An int representing the status code</param>
        /// <param name="redirect">A string representing the redirection url</param>
        public Content(string text, int code, string redirect)
        {
            _text = text;
            _code = code;
            _redirect = redirect;
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
        /// Gets the response of the controller in a stream.
        /// </summary>
        /// <returns>MemoryStream that represents the response of the controller.</returns>
        public MemoryStream Response()
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(_text);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        /// <summary>
        /// Gets the content type of the response.
        /// </summary>
        /// <returns>A string representing the content type of the response</returns>
        public string ContentType()
        {
            return "text/plain";
        }

        /// <summary>
        /// Gets the url where the response is going to redirect (if there is any).
        /// </summary>
        /// <returns>A string representing the redirection url</returns>
        public string Redirection()
        {
            return _redirect;
        }
    }
}
