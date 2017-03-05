using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

namespace PHttp
{
    /// <summary>
    /// Class that performs the initializing processes of the server
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Load all apps of the web server that implements the IPHttpApplication interface.
        /// </summary>
        /// <param name="path">Path of the dll file where all the apps are.</param>
        /// <returns>A list of the instances of all applications.</returns>
        public static List<Application.IPHttpApplication> LoadApps(string path)
        {
            List<Application.IPHttpApplication> apps = new List<Application.IPHttpApplication>();
            DirectoryInfo di = new DirectoryInfo(path);
            FileInfo[] fis = di.GetFiles("*.dll");
            foreach (FileInfo fls in fis)
            {
                var assembly = Assembly.LoadFrom(fls.FullName);

                foreach(var type in assembly.GetTypes())
                {
                    if (type != typeof(Application.IPHttpApplication) && typeof(Application.IPHttpApplication).IsAssignableFrom(type))
                    {
                        apps.Add((Application.IPHttpApplication)Activator.CreateInstance(type));
                    }
                }
            }

            foreach(var app in apps)
            {
                app.Start();
            }
            return apps;
        }
    }
}
