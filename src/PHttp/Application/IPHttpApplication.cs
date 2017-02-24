using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PHttp.Application
{
    public delegate string PreApplicationStartMethod();

    public delegate string ApplicationStartMethod();

    public interface IPHttpApplication
    {
        string Name { get; set; }

        event PreApplicationStartMethod PreApplicationStart;

        event ApplicationStartMethod ApplicationStart;

        void Start();

        void ExecuteAction();
    }
}
