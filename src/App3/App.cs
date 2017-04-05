using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mvc;

namespace App3
{
    public class App : PHttpApplication
    {
        public App() : base()
        {
            RegisterRoutingMap("{controller}/{action}/{id}");
        }
    }
}
