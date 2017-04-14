using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mvc
{
    public interface IActionResult
    {
        MemoryStream Response();

        string ContentType();

        int Code();

        string Redirection();

    }
}
