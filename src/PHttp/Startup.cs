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
        public static Application.IPHttpApplication LoadApp(string path)
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
                        return (Application.IPHttpApplication)Activator.CreateInstance(type);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Load all apps of the web server that implements the IPHttpApplication interface.
        /// </summary>
        /// <param name="path">Path of the dll file where all the apps are.</param>
        /// <returns>A list of the instances of all applications.</returns>
        public static Dictionary<string, Application.IPHttpApplication> LoadApps(string[] physicalPaths, string[] virtualPaths)
        {
            Dictionary<string, Application.IPHttpApplication> apps = new Dictionary<string, Application.IPHttpApplication>();
            for(int i = 0; i < physicalPaths.Length; i++)
            {
                DirectoryInfo di = new DirectoryInfo(physicalPaths[i]);
                FileInfo[] fis = di.GetFiles("*.dll");
                foreach (FileInfo fls in fis)
                {
                    var assembly = Assembly.LoadFrom(fls.FullName);
                    try
                    {
                        foreach (var type in assembly.GetTypes())
                        {
                            if (type != typeof(Application.IPHttpApplication) && typeof(Application.IPHttpApplication).IsAssignableFrom(type))
                            {
                                apps.Add(virtualPaths[i], (Application.IPHttpApplication)Activator.CreateInstance(type));
                            }
                        }
                    }
                    catch(Exception e)
                    {
                        continue;
                    }
                    
                }
            }
            return apps;
        }
    }
}
