using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PHttp;
using System.Configuration;

namespace DiegoApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = ConfigurationManager.AppSettings.Get("ApplicationsDir");
            Startup.LoadApps(path);
            Console.ReadKey();
        }
    }
}
