using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mvc
{
    public class Route
    {
        private string _path;

        private string _method;

        private string _controller;

        private string _action;

        private Dictionary<string, object> _parameters;

        public Dictionary<string, object> Parameters
        {
            get
            {
                return _parameters;
            }
            private set
            {
                _parameters = value;
            }
        }

        public string Path
        {
            get
            {
                return _path;
            }
            private set
            {
                _path = value;
            }
        }

        public string Method
        {
            get
            {
                return _method;
            }
            private set
            {
                _method = value;
            }
        }

        public string Controller
        {
            get
            {
                return _controller;
            }
            private set
            {
                _controller = value;
            }
        }

        public string Action
        {
            get
            {
                return _action;
            }
            private set
            {
                _action = value;
            }
        }

        public Route(string path, string method, string controller, string action)
        {
            _path = path;
            _method = method;
            _controller = controller;
            _action = action;
        }

    }
}
