using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mvc
{
    /// <summary>
    /// Representative interface of the result of a Controller
    /// </summary>
    public interface IActionResult
    {
        /// <summary>
        /// Gets the response of the controller in a stream.
        /// </summary>
        /// <returns>MemoryStream that represents the response of the controller.</returns>
        MemoryStream Response();

        /// <summary>
        /// Gets the content type of the response.
        /// </summary>
        /// <returns>A string representing the content type of the response</returns>
        string ContentType();

        /// <summary>
        /// Gets the status code of the response.
        /// </summary>
        /// <returns>An int representing the status code</returns>
        int Code();

        /// <summary>
        /// Gets the url where the response is going to redirect (if there is any).
        /// </summary>
        /// <returns>A string representing the redirection url</returns>
        string Redirection();

    }
}
