using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mvc
{
    public class Content : IActionResult
    {
        private int _code;

        private string _text;

        private string _redirect;

        public Content(string text, int code, string redirect)
        {
            _text = text;
            _code = code;
            _redirect = redirect;
        }
        public int Code()
        {
            return _code;
        }

        public MemoryStream Response()
        {
            return null;
        }

        public string ContentType()
        {
            return "text/plain";
        }

        public string Redirection()
        {
            return _redirect;
        }
    }
}
