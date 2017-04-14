using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HandlebarsDotNet;
using PHttp.Application;
using System.Collections.Specialized;

namespace Mvc
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
    public class PHttpApplication : PHttp.Application.IPHttpApplication
    {
        public event PHttp.Application.PreApplicationStartMethod PreApplicationStart;

        public event PHttp.Application.ApplicationStartMethod ApplicationStart;

        /// <summary>
        /// Name of the application.
        /// </summary>
        private string _name;

        private string _physicalPath;

        private string _routingMapPattern;

        /// <summary>
        /// Virtual path of the application.
        /// </summary>
        private string _virtualPath;

        private List<Route> _routes;

        private ConfigurationManager _configurationManager;

        private string _configurationPath;

        private NameValueCollection _headers = new NameValueCollection();

        public NameValueCollection Headers
        {
            get
            {
                return _headers;
            }
            private set
            {
                _headers = value;
            }
        }

        public NameValueCollection GetHeaders()
        {
            return Headers;
        }

        public string ConfigurationPath
        {
            get
            {
                return _configurationPath;
            }
            protected set
            {
                _configurationPath = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the application.</summary>
        /// <value>
        /// String value.</value>
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
            }
        }

        public string PhysicalPath
        {
            get
            {
                return _physicalPath;
            }

            set
            {
                _physicalPath = value;
            }
        }

        /// <summary>
        /// Gets or sets the virtual path of the application.</summary>
        /// <value>
        /// String value.</value>
        public string VirtualPath
        {
            get
            {
                return _virtualPath;
            }

            set
            {
                _virtualPath = value;
            }
        }

        public PHttpApplication()
        {
            _routes = new List<Route>();
            _configurationPath = "app.json";
        }

        private void AddRoutes()
        {
            BaseController app = new BaseController();
            DirectoryInfo di = new DirectoryInfo(PhysicalPath);
            FileInfo[] fis = di.GetFiles("*.dll");
            foreach (FileInfo fls in fis)
            {
                try
                {
                    var assembly = Assembly.LoadFrom(fls.FullName);

                    foreach (var type in assembly.GetTypes())
                    {
                        if (type != typeof(Mvc.BaseController) && typeof(Mvc.BaseController).IsAssignableFrom(type))
                        {
                            string path = "/";
                            string controller = "";
                            app = (Mvc.BaseController)Activator.CreateInstance(type);
                            controller = app.GetType().Name;
                            path += controller.Replace("Controller", "").ToLower();
                            foreach (MethodInfo method in app.GetType().GetMethods())
                            {
                                string action = "/" + method.Name.ToLower();
                                object[] customAttributes = method.GetCustomAttributes(typeof(HttpMethod), true);
                                foreach (var attribute in customAttributes)
                                {
                                    string httpMethod = attribute.ToString().Replace("Mvc.Http", "").ToUpper();
                                    Route route = new Route(path + action, httpMethod, controller, method.Name);
                                    _routes.Add(route);
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    continue;
                }
            }
        }

        public void RegisterRoutingMap(string routingMap)
        {
            _routingMapPattern = routingMap;
        }

        public void Init(string name, string virtualPath, string physicalPath)
        {
            VirtualPath = virtualPath;
            Name = name;
            PhysicalPath = physicalPath;
            ConfigurationPath = physicalPath + "\\" + _configurationPath;
            PreApplicationStart += (path) =>
            {
                _configurationManager = new ConfigurationManager(path);
                AddRoutes();
            };
            var task = Task.Run(() => OnPreApplicationStart(ConfigurationPath));
            task.Wait();
        }

        public ConfigurationManager ConfigurationManager
        {
            get
            {
                return _configurationManager;
            }
            private set
            {
                ;
            }
        }

        public string Error(string path, string errorMessage)
        {
            byte[] page = File.ReadAllBytes(path);
            var template = Handlebars.Compile(System.Text.Encoding.Default.GetString(page));
            return template(new { message = errorMessage });
        }

        public void OnApplicationStart(string physicalPath)
        {
            var ev = ApplicationStart;
            if (ev != null)
            {
                ev(physicalPath);
            }
        }

        public void OnPreApplicationStart(string virtualPath)
        {
            var ev = PreApplicationStart;
            if (ev != null)
            {
                ev(virtualPath);
            }
        }

        public object GetConfigurationManager()
        {
            return ConfigurationManager;
        }

        public string GetSession()
        {
            return Session.GetCurrentSession();
        }


        public object ExecuteControllerAction(Dictionary<string, object> actionToCall)
        {
            bool methodNotAllowed = false;
            foreach(Route route in _routes)
            {
                string[] pathParts = actionToCall["Path"].ToString().Split('/');
                string path = "/" + pathParts[1] + "/" + pathParts[2];
                if (route.Path == path)
                {
                    if (route.Method != actionToCall["Method"].ToString())
                    {
                        methodNotAllowed = true;
                    }
                    else
                    {
                        methodNotAllowed = false;
                        BaseController app = new BaseController();
                        DirectoryInfo di = new DirectoryInfo(PhysicalPath);
                        FileInfo[] fis = di.GetFiles("*.dll");
                        foreach (FileInfo fls in fis)
                        {
                            try
                            {
                                var assembly = Assembly.LoadFrom(fls.FullName);

                                foreach (var type in assembly.GetTypes())
                                {
                                    if (type != typeof(Mvc.BaseController) && typeof(Mvc.BaseController).IsAssignableFrom(type))
                                    {
                                        app = (Mvc.BaseController)Activator.CreateInstance(type);
                                        app.HttpRequest = new Request(
                                                (string)actionToCall["Path"],
                                                (NameValueCollection)actionToCall["Parameters"],
                                                (Dictionary<string, HttpFile>)actionToCall["Files"],
                                                (NameValueCollection)actionToCall["Headers"],
                                                (NameValueCollection)actionToCall["QueryParams"]
                                        );
                                        app.Route = route;
                                        app.HttpContext.PhysicalPath = PhysicalPath;
                                        app.HttpContext.ConfigurationManager = ConfigurationManager;
                                        if (app.GetType().Name == route.Controller)
                                        {
                                            MethodInfo method = app.GetType().GetMethod(route.Action);
                                            AuthorizeAttribute attribute = (AuthorizeAttribute)method.GetCustomAttribute(typeof(AuthorizeAttribute));
                                            User user = null;
                                            if (attribute != null)
                                            {
                                                switch (ConfigurationManager.ApplicationAuthenticationDriver)
                                                {
                                                    case "session":
                                                        user = attribute.AuthorizeSession(Session.GetCurrentSession());
                                                        break;
                                                    case "jwt":
                                                        user = attribute.AuthorizeAuthToken(app.HttpRequest, ConfigurationManager.ApplicationSecretKey);
                                                        Headers["Auth-Token"] = user.Token;
                                                        break;
                                                    default:
                                                        throw new NotImplementedException("Not implemented for other authentication drivers");
                                                }
                                                if (user != null)
                                                {
                                                    app.SetUser(user);
                                                    IActionResult result = (IActionResult)method.Invoke(app, new object[] { });
                                                    for(int i = 0; i < app.HttpContext.Headers.Count; i++)
                                                    {
                                                        Headers.Set(
                                                            app.HttpContext.Headers.Keys[i],
                                                            app.HttpContext.Headers[i]
                                                        );
                                                    }
                                                    return result;
                                                }
                                                return (int)401;
                                            }
                                            else
                                            {
                                                IActionResult result = (IActionResult)method.Invoke(app, new object[] { });
                                                for (int i = 0; i < app.HttpContext.Headers.Count; i++)
                                                {
                                                    Headers.Set(
                                                        app.HttpContext.Headers.Keys[i],
                                                        app.HttpContext.Headers[i]
                                                    );
                                                }
                                                return result;
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.StackTrace);
                                continue;
                            }
                        }
                    }
                }
            }
            if (methodNotAllowed)
            {
                return (int)405;
            }
            return (int)404;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
