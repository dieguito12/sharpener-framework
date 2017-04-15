using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Mvc
{
    /// <summary>
    /// Representative class of a file comming in an http request.
    /// </summary>
    public class HttpFile
    {
        /// <summary>
        /// Constructor of the HttpFile class.
        /// </summary>
        /// <param name="contentLength">An int representing the content length of the file.</param>
        /// <param name="contentType">A string representing the content type of the file.</param>
        /// <param name="fileName">A string representing the file name.</param>
        /// <param name="inputStream">A Stream representing the raw information of the file.</param>
        public HttpFile(int contentLength, string contentType, string fileName, Stream inputStream)
        {
            if (fileName == null)
                throw new ArgumentNullException("fileName");
            if (inputStream == null)
                throw new ArgumentNullException("inputStream");

            ContentLength = contentLength;
            ContentType = contentType;
            FileName = fileName;
            InputStream = inputStream;
        }

        /// <summary>
        /// Gets the content length of the file.
        /// </summary>
        public int ContentLength { get; private set; }

        /// <summary>
        /// Gets the content type of the file.
        /// </summary>
        public string ContentType { get; private set; }

        /// <summary>
        /// Gets the file name.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Gets the data stream of the file.
        /// </summary>
        public Stream InputStream { get; private set; }
    }
}
