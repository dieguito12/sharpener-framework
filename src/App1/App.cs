using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mvc;
using System.IO;
using System.Reflection;

namespace App1
{
    public class App : PHttpApplication
    {
        public App() : base()
        {
            RegisterRoutingMap("{controller}/{action}");
        }
    }
}
