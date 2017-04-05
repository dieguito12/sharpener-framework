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

        public Content(string text, int code)
        {
            _text = text;
            _code = code;
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
    }
}
