using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandlebarsDotNet;

namespace Mvc
{
    public class View : IActionResult
    {
        object _data;

        string _path;

        int _code;

        string _physicalPath;

        Route _route;

        public View(string path, object data, int code, string physicalPath, Route route)
        {
            _data = data;
            _path = path;
            _code = code;
            _route = route;
            _physicalPath = physicalPath;
        }

        public int Code()
        {
            return _code;
        }

        public MemoryStream Response()
        {
            string controller = _route.Controller.Replace("Controller", "");
            byte[] page = File.ReadAllBytes(_physicalPath + "\\Views\\" + controller + "\\" + _path + ".hbs");
            var template = Handlebars.Compile(System.Text.Encoding.Default.GetString(page));
            return new MemoryStream(Encoding.ASCII.GetBytes(template(_data)));
        }

        public string ContentType()
        {
            return "text/html";
        }

    }
}
