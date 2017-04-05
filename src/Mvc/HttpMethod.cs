using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mvc
{
    [AttributeUsage(AttributeTargets.Method)]
    public class HttpMethod : FilterAttribute
    {
        //
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HttpPost : HttpMethod
    {
        //
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HttpGet : HttpMethod
    {
        //
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HttpPut : HttpMethod
    {
        //
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HttpDelete : HttpMethod
    {
        //
    }
}
