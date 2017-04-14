using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using PHttp;
using System.Configuration;
using System.Collections.Specialized;

namespace DiegoApp
{
    class Program
    {
        private static string rootPath = @"C:\Users\dieguito12\Code\sharpener-framework\src";

        static void Main(string[] args)
        {
            using (var server = new HttpServer(HttpConfig.GetPort()))
            {
                
                List<HttpSite> sites = HttpConfig.GetSites();

                string[] appsPaths = new string[sites.Count];
                string[] virtualPaths = new string[sites.Count];

                for (int i = 0; i < sites.Count; i++)
                {
                    appsPaths[i] = sites[i].PhysicalPath;
                    virtualPaths[i] = sites[i].VirtualPath;
                }

                Dictionary<string, PHttp.Application.IPHttpApplication> apps = Startup.LoadApps(appsPaths, virtualPaths);

                server.StateChanged += (s, e) =>
                {
                    //
                };

                // New requests are signaled through the RequestReceived
                // event.
                server.RequestReceived += (s, e) =>
                {
                    // The response must be written to e.Response.OutputStream.
                    // When writing text, a StreamWriter can be used.
                    try
                    {
                        string[] pathElements = e.Request.Path.Split('/');
                        if (pathElements[1] == "")
                        {
                            byte[] data;
                            string indexPath = @"C:\Users\dieguito12\Code\sharpener-framework\src\PHttp\Resources\" + HttpConfig.GetDefaultDocuments()[0];
                            data = File.ReadAllBytes(indexPath);
                            e.Response.StatusCode = 200;
                            e.Response.ContentType = "text/html";
                            MemoryStream stream = new MemoryStream(data);
                            e.Response.OutputStream = new HttpOutputStream(stream);
                        }
                        else
                        {
                            Dictionary<string, object> actionToCall = new Dictionary<string, object>();
                            string path = e.Request.Path.Replace("/" + pathElements[1], "");
                            if (path == "")
                            {
                                path = "/";
                            }
                            actionToCall.Add("Path", path);
                            actionToCall.Add("Method", e.Request.HttpMethod);
                            actionToCall.Add("Parameters", e.Request.Form);
                            actionToCall.Add("QueryParams", e.Request.Params);
                            actionToCall.Add("Headers", e.Request.Headers);
                            Dictionary<string, Mvc.HttpFile> files = new Dictionary<string, Mvc.HttpFile>();
                            for(int i = 0; i < e.Request.Files.Count; i++)
                            {
                                HttpPostedFile file = e.Request.Files.Get(i);
                                Mvc.HttpFile mvcFile = new Mvc.HttpFile(file.ContentLength, file.ContentType, file.FileName, file.InputStream);
                                files.Add(e.Request.Files.GetKey(i), mvcFile);
                            }
                            actionToCall.Add("Files", files);
                            HttpSite site = null;
                            foreach(HttpSite mySite in sites)
                            {
                                if(mySite.VirtualPath == "/" + pathElements[1])
                                {
                                    site = mySite;
                                    break;
                                }
                            }
                            if (site == null || e.Request.Path.Contains('.'))
                            {
                                byte[] data;
                                data = File.ReadAllBytes(rootPath + e.Request.Path.Replace("/", "\\"));
                                e.Response.StatusCode = 200;
                                string file = pathElements[pathElements.Length - 1];
                                string extension = file.Split('.')[file.Split('.').Length - 1];
                                e.Response.ContentType = HttpMimeTypeMap.GetMimeType(extension);
                                MemoryStream stream = new MemoryStream(data);
                                e.Response.OutputStream = new HttpOutputStream(stream);
                            }
                            else
                            {
                                
                                PHttp.Application.IPHttpApplication app = null;
                                if (apps.ContainsKey(site.VirtualPath))
                                {
                                    app = (PHttp.Application.IPHttpApplication)apps[site.VirtualPath].Clone();
                                }
                                if (app == null)
                                {
                                    byte[] data;
                                    data = File.ReadAllBytes(HttpConfig.GetErrorPages()[502]);
                                    e.Response.StatusCode = 502;
                                    e.Response.ContentType = "text/html";
                                    MemoryStream stream = new MemoryStream(data);
                                    e.Response.OutputStream = new HttpOutputStream(stream);
                                }
                                else
                                {
                                    app.Init(site.Name, site.VirtualPath, site.PhysicalPath);
                                    Mvc.Session.DeleteExpiredSessions(((Mvc.ConfigurationManager)app.GetConfigurationManager()).ApplicationSecretKey);
                                    string currentSession = e.Request.Cookies["sharpener-session"].Value;
                                    if (currentSession != null && currentSession != "" && Mvc.Session.SessionExists(currentSession))
                                    {
                                        Mvc.Session.SetSession(currentSession);
                                    }
                                    if (pathElements[2] == "" || pathElements[2] == "/")
                                    {
                                        byte[] data;
                                        string indexPath = site.PhysicalPath + "\\" + ((Mvc.ConfigurationManager)app.GetConfigurationManager()).ApplicaitonDefaultDocument;
                                        data = File.ReadAllBytes(indexPath);
                                        e.Response.StatusCode = 200;
                                        e.Response.ContentType = "text/html";
                                        MemoryStream stream = new MemoryStream(data);
                                        e.Response.OutputStream = new HttpOutputStream(stream);
                                    }
                                    else
                                    {
                                        object result = app.ExecuteControllerAction(actionToCall);                                     
                                        e.Response.Headers = app.GetHeaders();
                                        e.Response.Cookies.Add(new HttpCookie("sharpener-session", app.GetSession()));
                                        MemoryStream stream = new MemoryStream();
                                        if (result.GetType() == typeof(int))
                                        {
                                            byte[] data;
                                            if (site.ErrorPages.ContainsKey((int)result))
                                            {
                                                data = Encoding.ASCII.GetBytes(app.Error(site.ErrorPages[(int)result], "Handlebars Error"));
                                            }
                                            else
                                            {
                                                data = File.ReadAllBytes(HttpConfig.GetErrorPages()[(int)result]);
                                            }
                                            e.Response.StatusCode = (int)result;
                                            e.Response.ContentType = "text/html";
                                            stream = new MemoryStream(data);
                                        }
                                        else
                                        {
                                            if (((Mvc.IActionResult)result).Code() == 301 || ((Mvc.IActionResult)result).Code() == 302)
                                            {
                                                e.Response.RedirectPermanent(((Mvc.IActionResult)result).Redirection());
                                            }
                                            else
                                            {
                                                e.Response.ContentType = ((Mvc.IActionResult)result).ContentType();
                                                e.Response.StatusCode = ((Mvc.IActionResult)result).Code();
                                                stream = ((Mvc.IActionResult)result).Response();
                                            }
                                        }
                                        e.Response.OutputStream = new HttpOutputStream(stream);
                                    }
                                }
                            }
                        }
                    }
                    catch (FileNotFoundException exception)
                    {
                        byte[] data;
                        data = File.ReadAllBytes(HttpConfig.GetErrorPages()[404]);
                        e.Response.StatusCode = 404;
                        e.Response.ContentType = "text/html";
                        MemoryStream stream = new MemoryStream(data);
                        e.Response.OutputStream = new HttpOutputStream(stream);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        byte[] data;
                        data = File.ReadAllBytes(HttpConfig.GetErrorPages()[500]);
                        e.Response.StatusCode = 500;
                        e.Response.ContentType = "text/html";
                        MemoryStream stream = new MemoryStream(data);
                        e.Response.OutputStream = new HttpOutputStream(stream);
                    }
                };


                server.Start();
                while(true) {; }
                //Console.WriteLine("Press any key to continue...");
                //Console.ReadKey();

            }
        }
    }
}