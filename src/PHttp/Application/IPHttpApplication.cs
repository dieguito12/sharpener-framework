using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PHttp.Application
{
    /// <summary>
    /// Delegate for the PreApplicationStartMethod event.</summary>
    /// <returns>
    /// String value.</returns>
    public delegate string PreApplicationStartMethod();

    /// <summary>
    /// Delegate for the ApplicationStartMethod event.</summary>
    /// <returns>
    /// String value.</returns>
    public delegate string ApplicationStartMethod();

    /// <summary>
    /// Interface of a Http application hosted in our Web Server.
    /// </summary>
    public interface IPHttpApplication
    {
        string Name { get; set; }

        event PreApplicationStartMethod PreApplicationStart;

        event ApplicationStartMethod ApplicationStart;

        void Start();

        void ExecuteAction();
    }
}
