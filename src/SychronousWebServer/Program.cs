using System;
using System.Threading;
using SharpenerClasses;

namespace SynchronousWebServer
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Web Server Running... Press ^C to Stop...");
                Thread th = new Thread(new ThreadStart(WebServer.StartListen));
                th.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine("An Exception Occurred while Listening :"
                                   + e.ToString());
            }
        }
    }
}
