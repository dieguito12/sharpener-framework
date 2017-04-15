using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Specialized;

namespace Mvc
{
    /// <summary>
    /// Representative class of a parsed request
    /// </summary>
    public class Request
    {
        /// <summary>
        /// Constructor of the Request class.
        /// </summary>
        /// <param name="path">A string representing the path of the url.</param>
        /// <param name="parameters">A NameValueCollectoin representing the body parameters of the request.</param>
        /// <param name="files">A Dictionary representing the files in the request.</param>
        /// <param name="headers">A NameValueCollection representing the headers of the request</param>
        /// <param name="queryParams">A NameValudCollection representing the query string of the url.</param>
        public Request(string path, NameValueCollection parameters, Dictionary<string, HttpFile> files, NameValueCollection headers, NameValueCollection queryParams)
        {
            Parameters = parameters;
            Path = path;
            Files = files;
            QueryParams = queryParams;
            Headers = headers;
        }

        /// <summary>
        /// Gets the headers of the request.
        /// </summary>
        public NameValueCollection Headers { get; private set; }
        
        /// <summary>
        /// Gets the query string parameters of the request.
        /// </summary>
        public NameValueCollection QueryParams { get; private set; }

        /// <summary>
        /// Gets the body parameters of the request.
        /// </summary>
        public NameValueCollection Parameters { get; private set; }

        /// <summary>
        /// Gets the dictionary of files
        /// </summary>
        public Dictionary<string, HttpFile> Files { get; private set; }

        /// <summary>
        /// Gets the path of the url
        /// </summary>
        public string Path
        {
            get;
            private set;
        }
    }
}
