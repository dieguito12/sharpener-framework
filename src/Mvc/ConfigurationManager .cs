using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Mvc
{
    /// <summary>
    /// Representative class of an app configuration manager.
    /// </summary>
    public class ConfigurationManager
    {
        /// <summary>
        /// Constructor of the ConfigurationManager class
        /// </summary>
        /// <param name="appConfigPath">A string representing the path of the configuration file</param>
        public ConfigurationManager(string appConfigPath)
        {
            LoadConfiguration(appConfigPath);
        }

        /// <summary>
        /// Loads the information of the configuration file
        /// </summary>
        /// <param name="appConfigPath">A string representing the path of the configuration file</param>
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

        /// <summary>
        /// Gets the application name.
        /// </summary>
        public string ApplicationName
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the application secret key.
        /// </summary>
        public string ApplicationSecretKey
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the error pages dictionary.
        /// </summary>
        public Dictionary<int, string> ApplicationErrorPages
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the application authentication driver.
        /// </summary>
        public string ApplicationAuthenticationDriver
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the application database connection string.
        /// </summary>
        public string ApplicationDatabaseConnection
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the application default document name.
        /// </summary>
        public string ApplicaitonDefaultDocument
        {
            get;
            private set;
        }
    }
}
