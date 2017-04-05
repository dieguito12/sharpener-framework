using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mvc
{
    public class HttpContext
    {
        string _physicalPath;

        private ConfigurationManager _configurationManager;

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
