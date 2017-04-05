using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Mvc
{
    public class HttpFile
    {
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

        public int ContentLength { get; private set; }

        public string ContentType { get; private set; }

        public string FileName { get; private set; }

        public Stream InputStream { get; private set; }
    }
}
