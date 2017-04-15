using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mvc
{
    /// <summary>
    /// Representative class of the context information of the http server.
    /// </summary>
    public class HttpContext
    {
        /// <summary>
        /// A string representing the physical path where the dll of the app is.
        /// </summary>
        string _physicalPath;

        /// <summary>
        /// A ConfigurationManager object representing the loaded configuration of the app.
        /// </summary>
        private ConfigurationManager _configurationManager;

        /// <summary>
        /// A NameValueCollection representing the headers of the response.
        /// </summary>
        private NameValueCollection _headers = new NameValueCollection();

        /// <summary>
        /// The current session string.
        /// </summary>
        private string _session;

        /// <summary>
        /// Gets and sets the headers of the response.
        /// </summary>
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

        /// <summary>
        /// Gets and sets the session of the app.
        /// </summary>
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

        /// <summary>
        /// Gets a configuration manager.
        /// </summary>
        public ConfigurationManager ConfigurationManager
        {
            get
            {
                return _configurationManager;
            }
            internal set
            {
                _configurationManager = value;
            }
        }

        /// <summary>
        /// Gets and the physycal path of the app.
        /// </summary>
        public string PhysicalPath
        {
            get
            {
                return _physicalPath;
            }
            internal set
            {
                _physicalPath = value;
            }
        }

    }
}
