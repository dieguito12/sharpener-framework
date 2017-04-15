using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mvc
{
    /// <summary>
    /// Representative class of a route.
    /// </summary>
    public class Route
    {
        /// <summary>
        /// The virtual path of this route.
        /// </summary>
        private string _path;

        /// <summary>
        /// The Http method (GET, POST, PUT) of this route.
        /// </summary>
        private string _method;

        /// <summary>
        /// The controller which this route points to.
        /// </summary>
        private string _controller;

        /// <summary>
        /// The controller action which this route points to.
        /// </summary>
        private string _action;


        /// <summary>
        /// Gets the virtual path of this route.
        /// </summary>
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

        /// <summary>
        /// Gets the http method of the route.
        /// </summary>
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

        /// <summary>
        /// Gets the controller name which this route points to.
        /// </summary>
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

        /// <summary>
        /// Gets the controller action name which this route points to.
        /// </summary>
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

        /// <summary>
        /// Constructor of the Route class.
        /// </summary>
        /// <param name="path">The virtual path of this route.</param>
        /// <param name="method">The Http method (GET, POST, PUT) of this route.</param>
        /// <param name="controller">The controller which this route points to.</param>
        /// <param name="action">The controller action which this route points to.</param>
        public Route(string path, string method, string controller, string action)
        {
            _path = path;
            _method = method;
            _controller = controller;
            _action = action;
        }

    }
}
