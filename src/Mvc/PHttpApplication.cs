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
        /// <summary>
        /// Event that is triggered before the application starts
        /// </summary>
        public event PHttp.Application.PreApplicationStartMethod PreApplicationStart;

        /// <summary>
        /// Event that is triggered when the application starts
        /// </summary>
        public event PHttp.Application.ApplicationStartMethod ApplicationStart;

        /// <summary>
        /// Name of the application.
        /// </summary>
        private string _name;

        /// <summary>
        /// Physical path of the application dll.
        /// </summary>
        private string _physicalPath;

        /// <summary>
        /// String of the routing pattern
        /// </summary>
        private string _routingMapPattern;

        /// <summary>
        /// Virtual path of the application.
        /// </summary>
        private string _virtualPath;

        /// <summary>
        /// The list of all available routes of the app.
        /// </summary>
        private List<Route> _routes;

        /// <summary>
        /// Configuration manager of the app.
        /// </summary>
        private ConfigurationManager _configurationManager;

        /// <summary>
        /// Path of the application configuration file.
        /// </summary>
        private string _configurationPath;

        /// <summary>
        /// Headers of the incoming request.
        /// </summary>
        private NameValueCollection _headers = new NameValueCollection();

        /// <summary>
        /// Gets the headers of the app response to the web server
        /// </summary>
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

        /// <summary>
        /// Gets the headers of the app response to the web server
        /// </summary>
        /// <returns>NameValueCollection with the headers</returns>
        public NameValueCollection GetHeaders()
        {
            return Headers;
        }

        /// <summary>
        /// Gets the path of the application configuration file.
        /// </summary>
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
        /// Gets the name of the application.</summary>
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

        /// <summary>
        /// Gets the physical path of the application dll.
        /// </summary>
        public string PhysicalPath
        {
            get
            {
                return _physicalPath;
            }
            internal set
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

        /// <summary>
        /// Constructor of the PHttpApplication class.
        /// </summary>
        public PHttpApplication()
        {
            _routes = new List<Route>();
            _configurationPath = "app.json";
        }

        /// <summary>
        /// Creates the list of available routes of the application.
        /// </summary>
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

        /// <summary>
        /// Registers the routing map pattern
        /// </summary>
        /// <param name="routingMap">A string with the routing map pattern</param>
        /// <remarks>Format: {controller}/{method}/{id (optional)}</remarks>
        public void RegisterRoutingMap(string routingMap)
        {
            _routingMapPattern = routingMap;
        }

        /// <summary>
        /// Initializes the application.
        /// </summary>
        /// <param name="name">Name of the application</param>
        /// <param name="virtualPath">Virtual path of the application</param>
        /// <param name="physicalPath">Physical path of the application</param>
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

        /// <summary>
        /// Gets the configuration manager of the application
        /// </summary>
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

        /// <summary>
        /// Returns an error page given the path of the template and the error message
        /// </summary>
        /// <param name="path">Path of the view template</param>
        /// <param name="errorMessage">String with the error message</param>
        /// <returns>A compiled html view in a string</returns>
        public string Error(string path, string errorMessage)
        {
            byte[] page = File.ReadAllBytes(path);
            var template = Handlebars.Compile(System.Text.Encoding.Default.GetString(page));
            return template(new { message = errorMessage });
        }

        /// <summary>
        /// Listener of the application start event
        /// </summary>
        /// <param name="physicalPath">Physical path of the application</param>
        public void OnApplicationStart(string physicalPath)
        {
            var ev = ApplicationStart;
            if (ev != null)
            {
                ev(physicalPath);
            }
        }

        /// <summary>
        /// Listener of the pre application start event.
        /// </summary>
        /// <param name="virtualPath">Virtual path of the application</param>
        public void OnPreApplicationStart(string virtualPath)
        {
            var ev = PreApplicationStart;
            if (ev != null)
            {
                ev(virtualPath);
            }
        }

        /// <summary>
        /// Gets the configuration manager as a generic object
        /// </summary>
        /// <returns>A generic object with the configuration manager information</returns>
        public object GetConfigurationManager()
        {
            return ConfigurationManager;
        }

        /// <summary>
        /// Gets the current session of the application.
        /// </summary>
        /// <returns>String with the current session.</returns>
        public string GetSession()
        {
            return Session.GetCurrentSession();
        }

        /// <summary>
        /// Executes an action of a controller
        /// </summary>
        /// <param name="actionToCall">Dictionary of all the information the application needs to execute a controller
        /// (request, path, parameters, headers, etc.)</param>
        /// <returns>A generic object that varies between an ActionResult (if the request was succesfull) or an int
        /// of an error status code (if not)</returns>
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
                                                        if (user != null)
                                                        {
                                                            Headers["Auth-Token"] = user.Token;
                                                        }
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

        /// <summary>
        /// Clones this object
        /// </summary>
        /// <returns>A PHttpApplication cloned object.</returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
