using System;
using System.Text;

namespace PHttp
{
    /// <summary>
    /// Representative of the utilities of the server.
    /// </summary>
    public class HttpServerUtility
    {
        /// <summary>
        /// Constructor of the class.
        /// </summary>
        internal HttpServerUtility()
        {

        }

        /// <summary>
        /// Gets the name of the machine where the server is running.
        /// </summary>
        public string MachineName
        {
            get { return Environment.MachineName; }
        }

        /// <summary>
        /// Encode the html given.
        /// </summary>
        /// <param name="value">Html template.</param>
        /// <returns>Html template encoded</returns>
        public string HtmlEncode(string value)
        {
            return HttpUtil.HtmlEncode(value);
        }

        /// <summary>
        /// Decode an encoded html template.
        /// </summary>
        /// <param name="value">Encoded html template.</param>
        /// <returns>Html template</returns>
        public string HtmlDecode(string value)
        {
            return HttpUtil.HtmlDecode(value);
        }

        /// <summary>
        /// Encode an URL
        /// </summary>
        /// <param name="text">URL to encode</param>
        /// <returns>URL encoded</returns>
        public string UrlEncode(string text)
        {
            return Uri.EscapeDataString(text);
        }

        /// <summary>
        /// Decode an URL
        /// </summary>
        /// <param name="text">Encoded URL</param>
        /// <returns>URL decoded</returns>
        public string UrlDecode(string text)
        {
            return UrlDecode(text, Encoding.UTF8);
        }

        /// <summary>
        /// Decode an URL
        /// </summary>
        /// <param name="text">Encoded URL</param>
        /// <param name="encoding">Encoding type</param>
        /// <returns>URL decoded</returns>
        public string UrlDecode(string text, Encoding encoding)
        {
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            return HttpUtil.UriDecode(text, encoding);
        }
    }
}