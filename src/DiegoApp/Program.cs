using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using PHttp;
using MimeTypes;
using System.Configuration;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var server = new HttpServer(8084))
            {
                // New requests are signaled through the RequestReceived
                // event.

                server.StateChanged += (s, e) =>
                {
                    //
                };

                server.RequestReceived += (s, e) =>
                {
                    // The response must be written to e.Response.OutputStream.
                    // When writing text, a StreamWriter can be used.
                    string dirPath = ConfigurationManager.AppSettings.Get("ResourcesDir");
                    string filePath = dirPath + e.Request.Path;
                    if (e.Request.Path.Equals("/"))
                    {
                        filePath = dirPath + '/' + @"index.html";
                    }
                    string extension = Path.GetExtension(filePath);
                    string mime = MimeTypeMap.GetMimeType(extension);
                    e.Response.ContentType = mime;

                    byte[] data;
                    if (!File.Exists(filePath))
                    {
                        data = File.ReadAllBytes(dirPath + '/' + "404.html");
                        e.Response.ContentType = "text/html";
                    }
                    else
                    {
                        data = File.ReadAllBytes(filePath);
                    }

                    MemoryStream stream = new MemoryStream(data);
                    e.Response.OutputStream = new HttpOutputStream(stream);
                };

                // Start the server on a random port. Use server.EndPoint
                // to specify a specific port, e.g.:
                //
                //     server.EndPoint = new IPEndPoint(IPAddress.Loopback, 80);
                //

                server.Start();

                // Start the default web browser.

                Process.Start(String.Format("http://{0}/", server.EndPoint));

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();

                // When the HttpServer is disposed, all opened connections
                // are automatically closed.
            }
        }
    }
}