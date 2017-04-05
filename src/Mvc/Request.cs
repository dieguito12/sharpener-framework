using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Specialized;

namespace Mvc
{
    public class Request
    {
    
        public Request(string path, NameValueCollection parameters, Dictionary<string, HttpFile> files, NameValueCollection headers)
        {
            Parameters = parameters;
            Path = path;
            Files = files;
            QueryString = QueryString;
            Headers = headers;
        }

        public NameValueCollection Headers { get; private set; }

        public NameValueCollection QueryString { get; private set; }

        public NameValueCollection Parameters { get; private set; }

        public Dictionary<string, HttpFile> Files { get; private set; }

        public string Path
        {
            get;
            private set;
        }

        public bool HasParameter(string key)
        {
            if (Parameters[key] != null && Parameters[key] != "")
            {
                return true;
            }
            return false;
        }

        public string GetParameter(string key)
        {
            return Parameters[key];
        }

        public HttpFile GetFile(string key)
        {
            return Files[key];
        }

        public bool HasFile(string key)
        {
            return Files.ContainsKey(key);
        }

    }
}
