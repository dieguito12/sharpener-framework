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
                var myObjects = (from t in assembly.GetTypes()
                                 where t.GetInterfaces().Contains(typeof(Application.IPHttpApplication))
                                 select (Application.IPHttpApplication)Activator.CreateInstance(t)).ToList();

                apps = apps.Concat(myObjects).ToList();
            }

            foreach(var app in apps)
            {
                app.Start();
            }
            return apps;
        }
    }
}
