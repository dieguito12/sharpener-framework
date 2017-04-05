using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.IO;

namespace PHttp
{
    public class HttpConfig
    {
        private static string _dirPath = "C:\\Users\\dieguito12\\Code\\sharpener-framework\\src\\PHttp\\config.json";
        public static int GetPort()
        {
            JObject json = JObject.Parse(File.ReadAllText(_dirPath));
            return (int)json.GetValue("port");
        }

        public static Dictionary<int, string> GetErrorPages()
        {
            JObject json = JObject.Parse(File.ReadAllText(_dirPath));
            Dictionary<int, string> errorPages = new Dictionary<int, string>();
            JArray pages = (JArray)json.GetValue("errorPages");
            foreach(JObject page in pages)
            {
                foreach(JProperty prop in page.Properties())
                {
                    errorPages.Add(int.Parse(prop.Name), page.Property(prop.Name).Value.ToString());
                }
            }

            return errorPages;
        }

        public static List<string> GetDefaultDocuments()
        {
            JObject json = JObject.Parse(File.ReadAllText(_dirPath));
            List<string> defaultDocuments = new List<string>();
            JArray documents = (JArray)json.GetValue("defaultDocuments");
            foreach (JToken document in documents)
            {
                defaultDocuments.Add(document.ToString());
            }

            return defaultDocuments;
        }

        public static List<HttpSite> GetSites()
        {
            JObject json = JObject.Parse(File.ReadAllText(_dirPath));
            List<HttpSite> httpSites = new List<HttpSite>();
            JArray sites = (JArray)json.GetValue("sites");
            foreach (JObject site in sites)
            {
                Dictionary<int, string> errorPages = new Dictionary<int, string>();
                JArray pages = (JArray)site["errorPages"];
                foreach (JObject page in pages)
                {
                    foreach (JProperty prop in page.Properties())
                    {
                        errorPages.Add(int.Parse(prop.Name), page.Property(prop.Name).Value.ToString());
                    }
                }

                List<string> defaultDocuments = new List<string>();
                JArray documents = (JArray)site["defaultDocument"];
                foreach (JToken document in documents)
                {
                    defaultDocuments.Add(document.ToString());
                }
                HttpSite httpSite = new HttpSite(
                    site["name"].ToString(),
                    site["virtualPath"].ToString(),
                    site["physicalPath"].ToString(),
                    bool.Parse(site["directoryBrowsing"].ToString()),
                    defaultDocuments,
                    errorPages
                );
                httpSites.Add(httpSite);
            }

            return httpSites;
        }

        public static HttpSite GetSiteByVirtualPath(string virtualPath)
        {
            JObject json = JObject.Parse(File.ReadAllText(_dirPath));
            List<HttpSite> httpSites = new List<HttpSite>();
            JArray sites = (JArray)json.GetValue("sites");
            foreach (JObject site in sites)
            {
                if(site["virtualPath"].ToString() == virtualPath)
                {
                    Dictionary<int, string> errorPages = new Dictionary<int, string>();
                    JArray pages = (JArray)site["errorPages"];
                    foreach (JObject page in pages)
                    {
                        foreach (JProperty prop in page.Properties())
                        {
                            errorPages.Add(int.Parse(prop.Name), page.Property(prop.Name).Value.ToString());
                        }
                    }

                    List<string> defaultDocuments = new List<string>();
                    JArray documents = (JArray)site["defaultDocument"];
                    foreach (JToken document in documents)
                    {
                        defaultDocuments.Add(document.ToString());
                    }
                    HttpSite httpSite = new HttpSite(
                        site["name"].ToString(),
                        site["virtualPath"].ToString(),
                        site["physicalPath"].ToString(),
                        bool.Parse(site["directoryBrowsing"].ToString()),
                        defaultDocuments,
                        errorPages
                    );
                    return httpSite;
                }
            }
            return null;
        }
    }
}
