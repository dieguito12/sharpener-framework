using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PHttp.Application
{
    /// <summary>
    /// Delegate for the PreApplicationStartMethod event.</summary>
    /// <returns>
    /// String value.</returns>
    public delegate void PreApplicationStartMethod(string virtualPath);

    /// <summary>
    /// Delegate for the ApplicationStartMethod event.</summary>
    /// <returns>
    /// String value.</returns>
    public delegate void ApplicationStartMethod(string physicalPat);

    /// <summary>
    /// Interface of a Http application hosted in our Web Server.
    /// </summary>
    public interface IPHttpApplication : ICloneable
    {
        event PreApplicationStartMethod PreApplicationStart;

        event ApplicationStartMethod ApplicationStart;

        void OnApplicationStart(string physicalPat);

        void OnPreApplicationStart(string virtualPath);

        object ExecuteControllerAction(Dictionary<string, object> actionToCall);

        void Init(string name, string virtualPath, string physicalPath);

        string Error(string path, string errorMessage);

        object GetConfigurationManager();

        NameValueCollection GetHeaders();

        string GetSession();
    }
}
