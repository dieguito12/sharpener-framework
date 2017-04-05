using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Mvc
{
    public class ConfigurationManager
    {
        public ConfigurationManager(string appConfigPath)
        {
            LoadConfiguration(appConfigPath);
        }

        private void LoadConfiguration(string appConfigPath)
        {
            JObject json = JObject.Parse(File.ReadAllText(appConfigPath));
            Dictionary<int, string> errorPages = new Dictionary<int, string>();
            JArray pages = (JArray)json["errorPages"];
            foreach (JObject page in pages)
            {
                foreach (JProperty prop in page.Properties())
                {
                    errorPages.Add(int.Parse(prop.Name), page.Property(prop.Name).Value.ToString());
                }
            }
            ApplicationName = (string)json.GetValue("name");
            ApplicationErrorPages = errorPages;
            ApplicaitonDefaultDocument = (string)json.GetValue("defaultDocument");
            ApplicationDatabaseConnection = (string)json.GetValue("databaseConnection");
            ApplicationAuthenticationDriver = (string)json.GetValue("authenticationDriver");
            ApplicationSecretKey = (string)json.GetValue("secret");
        }

        public string ApplicationName
        {
            get;
            private set;
        }

        public string ApplicationSecretKey
        {
            get;
            private set;
        }

        public Dictionary<int, string> ApplicationErrorPages
        {
            get;
            private set;
        }

        public string ApplicationAuthenticationDriver
        {
            get;
            private set;
        }

        public string ApplicationDatabaseConnection
        {
            get;
            private set;
        }

        public string ApplicaitonDefaultDocument
        {
            get;
            private set;
        }
    }
}
