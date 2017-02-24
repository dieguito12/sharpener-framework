using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

namespace PHttp
{
    public class Startup
    {

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
