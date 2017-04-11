using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mvc
{
    public class HttpContext
    {
        string _physicalPath;

        private ConfigurationManager _configurationManager;

        private NameValueCollection _headers = new NameValueCollection();

        private string _session;

        public NameValueCollection Headers
        {
            get
            {
                return _headers;
            }
            set
            {
                _headers = value;
            }
        }

        public string Session
        {
            get
            {
                return _session;
            }
            set
            {
                _session = value;
            }
        }

        public ConfigurationManager ConfigurationManager
        {
            get
            {
                return _configurationManager;
            }
            set
            {
                _configurationManager = value;
            }
        }

        public string PhysicalPath
        {
            get
            {
                return _physicalPath;
            }
            set
            {
                _physicalPath = value;
            }
        }

    }
}
