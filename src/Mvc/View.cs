using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandlebarsDotNet;

namespace Mvc
{
    /// <summary>
    /// Representative class of a custom view.
    /// </summary>
    public class View : IActionResult
    {
        /// <summary>
        /// Data to be passed to the view template.
        /// </summary>
        object _data;

        /// <summary>
        /// A string representing the name of the view template.
        /// </summary>
        string _template;

        /// <summary>
        /// An int representing the status code.
        /// </summary>
        int _code;

        /// <summary>
        /// A string representing the physical path of the App.
        /// </summary>
        string _physicalPath;

        /// <summary>
        /// A Route object representing the information of the route
        /// </summary>
        /// <remarks>This is to now in which controller folder look the template</remarks>
        Route _route;

        /// <summary>
        /// Constructor of View class
        /// </summary>
        /// <param name="template">A string representing the name of the view template.</param>
        /// <param name="data">An object with the data to be passed to the view template.</param>
        /// <param name="code">An int representing the status code.</param>
        /// <param name="physicalPath">A string representing the physical path of the App.</param>
        /// <param name="route">A Route object representing the information of the route</param>
        public View(string template, object data, int code, string physicalPath, Route route)
        {
            _data = data;
            _template = template;
            _code = code;
            _route = route;
            _physicalPath = physicalPath;
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
            string controller = _route.Controller.Replace("Controller", "");
            byte[] page = File.ReadAllBytes(_physicalPath + "\\Views\\" + controller + "\\" + _template + ".hbs");
            var template = Handlebars.Compile(System.Text.Encoding.Default.GetString(page));
            return new MemoryStream(Encoding.ASCII.GetBytes(template(_data)));
        }

        /// <summary>
        /// Gets the content type of the response.
        /// </summary>
        /// <returns>A string representing the content type of the response</returns>
        public string ContentType()
        {
            return "text/html";
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
