using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PHttp.Application;

namespace Mvc
{
    public class Application : IPHttpApplication
    {
        private string name;

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public event PreApplicationStartMethod PreApplicationStart;
        public event ApplicationStartMethod ApplicationStart;

        public void Start()
        {
            Console.WriteLine("Start Method");
        }

        public void ExecuteAction()
        {
            Console.WriteLine("ExecuteAction Method");
        }

        public void Test()
        {
            Console.WriteLine("This is a Test Method");
        }
    }
}
