using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mvc
{
    /// <summary>
    /// Representative class of and http method attribute
    /// </summary>
    /// <remarks>Works as a method decorator</remarks>
    [AttributeUsage(AttributeTargets.Method)]
    public class HttpMethod : FilterAttribute
    {
        //
    }

    /// <summary>
    /// Representative class of and http POST method attribute
    /// </summary>
    /// <remarks>Works as a method decorator</remarks>
    [AttributeUsage(AttributeTargets.Method)]
    public class HttpPost : HttpMethod
    {
        //
    }

    /// <summary>
    /// Representative class of and http GET method attribute
    /// </summary>
    /// <remarks>Works as a method decorator</remarks>
    [AttributeUsage(AttributeTargets.Method)]
    public class HttpGet : HttpMethod
    {
        //
    }

    /// <summary>
    /// Representative class of and http PUT method attribute
    /// </summary>
    /// <remarks>Works as a method decorator</remarks>
    [AttributeUsage(AttributeTargets.Method)]
    public class HttpPut : HttpMethod
    {
        //
    }

    /// <summary>
    /// Representative class of and http DELETE method attribute
    /// </summary>
    /// <remarks>Works as a method decorator</remarks>
    [AttributeUsage(AttributeTargets.Method)]
    public class HttpDelete : HttpMethod
    {
        //
    }
}
