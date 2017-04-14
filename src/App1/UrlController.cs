using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mvc;
using JWT;
using System.Collections.Specialized;
using App1.Models;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using MaxMind.GeoIP2;
using System.Diagnostics;

namespace App1
{
    public class UrlController : BaseController
    {

        public UrlController()
        {
            //
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetUrl()
        {
            using (MySqlConnection connection = new MySqlConnection(HttpContext.ConfigurationManager.ApplicationDatabaseConnection))
            {
                connection.Open();
                using (var context = new Context(connection, false))
                {
                    var id = int.Parse(HttpRequest.QueryParams["id"]);
                    var url = context.Urls
                                    .Where(b => b.Id == id)
                                    .Where(b => b.Deleted == false)
                                    .FirstOrDefault();
                    if (url == null)
                    {
                        return Json("{\"error\":\"Url not found.\"}", 404);
                    }
                    var referers = url.Referers;
                    var platforms = url.Platforms;
                    var agents = url.Agents;
                    var locations = url.Locations;

                    NameValueCollection response = new NameValueCollection();
                    response.Add("id", url.Id.ToString());
                    response.Add("url", url.Url);
                    response.Add("shortUrl", url.Short);
                    response.Add("clicks", url.Clicks.ToString());
                    response.Add("lastClick", url.LastClick.ToString("yyyy-MM-dd HH:mm:ss"));
                    response.Add("created", url.Created.ToString("yyyy-MM-dd HH:mm:ss"));

                    string json = Mvc.Json.Jsonify(response);
                    json = json.Remove(json.Length - 1);
                    string referersJson = "[";
                    if (referers != null)
                    {
                        for (int i = 0; i < referers.Count; i++)
                        {
                            NameValueCollection value = new NameValueCollection();
                            value.Add("id", referers[i].Id.ToString());
                            value.Add("referer", referers[i].Referer);
                            referersJson += Mvc.Json.Jsonify(value);
                            if (referers.Count - i > 1)
                            {
                                referersJson += ",";
                            }
                        }
                    }
                    referersJson += "]";
                    json += ",\"referers\":" + referersJson;

                    string locationsJson = "[";
                    if (locations != null)
                    {
                        for (int i = 0; i < locations.Count; i++)
                        {
                            NameValueCollection value = new NameValueCollection();
                            value.Add("id", locations[i].Id.ToString());
                            value.Add("location", locations[i].Location);
                            locationsJson += Mvc.Json.Jsonify(value);
                            if (locations.Count - i > 1)
                            {
                                locationsJson += ",";
                            }
                        }
                    }
                    locationsJson += "]";
                    json += ",\"locations\":" + locationsJson;

                    string agentsJson = "[";
                    if (agents != null)
                    {
                        for (int i = 0; i < agents.Count; i++)
                        {
                            NameValueCollection value = new NameValueCollection();
                            value.Add("id", agents[i].Id.ToString());
                            value.Add("agent", agents[i].Agent);
                            agentsJson += Mvc.Json.Jsonify(value);
                            if (agents.Count - i > 1)
                            {
                                agentsJson += ",";
                            }
                        }
                    }
                    agentsJson += "]";
                    json += ",\"agents\":" + agentsJson;
                    string platformsJson = "[";
                    if (platforms != null)
                    {
                        for (int i = 0; i < platforms.Count; i++)
                        {
                            NameValueCollection value = new NameValueCollection();
                            value.Add("id", platforms[i].Id.ToString());
                            value.Add("referer", platforms[i].Platform);
                            platformsJson += Mvc.Json.Jsonify(value);
                            if (platforms.Count - i > 1)
                            {
                                platformsJson += ",";
                            }
                        }
                    }
                    platformsJson += "]}";
                    json += ",\"platforms\":" + platformsJson;
                    return Json(json, 200);
                }
            }
        }

        [HttpGet]
        [Authorize]
        public IActionResult Urls()
        {
            using (MySqlConnection connection = new MySqlConnection(HttpContext.ConfigurationManager.ApplicationDatabaseConnection))
            {
                connection.Open();
                using (var context = new Context(connection, false))
                {
                    string username = User.Username;
                    var user = context.Users
                                    .Where(b => b.Username == username)
                                    .FirstOrDefault();
                    var urls = user.Urls;
                    
                    NameValueCollection response = new NameValueCollection();
                    string json = "[";
                    for(int i = 0; i < urls.Count; i++)
                    {
                        if (urls[i].Deleted == false)
                        {
                            NameValueCollection value = new NameValueCollection();

                            value.Add("id", urls[i].Id.ToString());
                            value.Add("url", urls[i].Url);
                            value.Add("shortUrl", urls[i].Short);
                            value.Add("clicks", urls[i].Clicks.ToString());
                            value.Add("lastClick", urls[i].LastClick.ToString("yyyy-MM-dd HH:mm:ss"));
                            value.Add("created", urls[i].Created.ToString("yyyy-MM-dd HH:mm:ss"));
                            json += Mvc.Json.Jsonify(value);
                            if (urls.Count - i > 1)
                            {
                                json += ",";
                            }
                        }
                    }
                    json += "]";
                    return Json(json, 200);
                }
            }
        }

        [HttpGet]
        public IActionResult Click()
        {
            using (MySqlConnection connection = new MySqlConnection(HttpContext.ConfigurationManager.ApplicationDatabaseConnection))
            {
                connection.Open();
                using (var context = new Context(connection, false))
                {
                    var agent = HttpRequest.QueryParams["HTTP_USER_AGENT"];
                    if (agent.Contains("Firefox/") && !agent.Contains("Seamonkey/"))
                    {
                        agent = "Firefox";
                    }
                    else if (agent.Contains("Seamonkey/"))
                    {
                        agent = "Seamonkey";
                    }
                    else if (agent.Contains("Chrome/") && !agent.Contains("Chromium/") && !agent.Contains("Edge/"))
                    {
                        agent = "Chrome";
                    }
                    else if (agent.Contains("Edge/"))
                    {
                        agent = "Edge";
                    }
                    else if (agent.Contains("Chromium/"))
                    {
                        agent = "Chromium";
                    }
                    else if (agent.Contains("Safari/") && !agent.Contains("Chromium/") && !agent.Contains("Chrome/"))
                    {
                        agent = "Safari";
                    }
                    else if (agent.Contains("OPR/") || agent.Contains("Opera/"))
                    {
                        agent = "Opera";
                    }
                    else if (agent.Contains("MSIE") || agent.Contains("Trident/"))
                    {
                        agent = "Internet Explorer";
                    }
                    var referer = HttpRequest.QueryParams["Referer"];
                    var location = HttpRequest.QueryParams["REMOTE_ADDR"];
                    using (var reader = new DatabaseReader("GeoLite2-Country.mmdb"))
                    {
                        try
                        {
                            location = reader.Country(location).Country.Name;
                        }
                        catch (Exception e)
                        {
                            location = "Unknown";
                        }
                    }
                    var platform = Regex.Match(HttpRequest.QueryParams["HTTP_USER_AGENT"], @"\(([^)]*)\)").Groups[1].Value;
                    platform = platform.Split(';')[0];
                    var shortUrl = HttpRequest.QueryParams["Host"] + HttpRequest.QueryParams["URL"];
                    var url = context.Urls
                                    .Where(b => b.Short == shortUrl)
                                    .FirstOrDefault();
                    if (url == null)
                    {
                        return Json("{\"error\":\"Url not found.\"}", 404);
                    }
                    if (url.Agents == null)
                    {
                        url.Agents = new List<AgentModel>();
                    }
                    url.Agents.Add(new AgentModel { UrlId = url.Id, Agent = agent });
                    
                    if (url.Referers == null)
                    {
                        url.Referers = new List<RefererModel>();
                    }
                    if (referer != null)
                    {
                        url.Referers.Add(new RefererModel { UrlId = url.Id, Referer = referer});
                    }

                    if (url.Locations == null)
                    {
                        url.Locations = new List<LocationModel>();
                    }
                    url.Locations.Add(new LocationModel { UrlId = url.Id, Location = location });

                    if (url.Platforms == null)
                    {
                        url.Platforms = new List<PlatformModel>();
                    }
                    url.Platforms.Add(new PlatformModel { UrlId = url.Id, Platform = platform});

                    url.Clicks++;
                    url.LastClick = DateTime.Now;
                    context.SaveChanges();
                    return Content("", 301, url.Url);
                }
            }
        }

        [HttpPost]
        public IActionResult Delete()
        {
            using (MySqlConnection connection = new MySqlConnection(HttpContext.ConfigurationManager.ApplicationDatabaseConnection))
            {
                connection.Open();
                using (var context = new Context(connection, false))
                {
                    var id = int.Parse(HttpRequest.Parameters["id"]);
                    var url = context.Urls
                                    .Where(b => b.Id == id)
                                    .FirstOrDefault();
                    if (url == null)
                    {
                        return Json("{\"error\":\"Url not found.\"}", 404);
                    }
                    url.Deleted = true;
                    context.SaveChanges();
                    NameValueCollection response = new NameValueCollection();
                    response.Add("deleted", "Ok");
                    return Json(Mvc.Json.Jsonify(response), 200);
                }
            }
        }

        [HttpPost]
        public IActionResult Short()
        {
            using (MySqlConnection connection = new MySqlConnection(HttpContext.ConfigurationManager.ApplicationDatabaseConnection))
            {
                connection.Open();
                using (var context = new Context(connection, false))
                {
                    AuthorizeAttribute authorize = new AuthorizeAttribute();
                    var user = authorize.AuthorizeAuthToken(HttpRequest, HttpContext.ConfigurationManager.ApplicationSecretKey);
                    if (user != null)
                    {
                        User = user;
                        HttpContext.Headers["Auth-Token"] = User.Token;
                    }
                    
                    string shortUrl = Path.GetRandomFileName();
                    shortUrl = shortUrl.Replace(".", "");
                    shortUrl = HttpRequest.QueryParams["Host"] + "/app1/url/click/" + shortUrl;
                    int userId = 0;
                    if (User != null)
                    {
                        userId = context.Users
                                    .Where(b => b.Username == User.Username)
                                    .FirstOrDefault().Id;
                    }

                    var url = new UrlModel
                    {
                        Url = HttpRequest.Parameters["url"],
                        Short = shortUrl,
                        Created = DateTime.Now,
                        Deleted = false,
                        Clicks = 0,
                        UserId = userId
                    };
                    var pictureName = DateTime.Now.Subtract(DateTime.MinValue).TotalSeconds.ToString();
                    pictureName = pictureName.Replace(".", "");

                    // Linux
                    /*ProcessStartInfo startInfo = new ProcessStartInfo() { FileName = "/bin/bash", Arguments = "phantomjs screenshot.js " + HttpRequest.Parameters["url"] + " " + pictureName + ".jpg", };
                    Process proc = new Process() { StartInfo = startInfo, };
                    proc.Start();*/

                    //Windows
                    Process p = new Process();
                    ProcessStartInfo psi = new ProcessStartInfo();
                    psi.FileName = "CMD.EXE";
                    psi.Arguments = "/C C:\\phantomjs-2.1.1-windows\\bin\\phantomjs.exe C:\\Users\\dieguito12\\Code\\sharpener-framework\\src\\App1\\screenshot.js \"" + HttpRequest.Parameters["url"] + "\" C:\\Users\\dieguito12\\Code\\sharpener-framework\\src\\App1\\screenshots\\" + pictureName + ".jpg";
                    psi.CreateNoWindow = true;
                    psi.UseShellExecute = false;
                    p.StartInfo = psi;
                    p.Start();
                    p.WaitForExit();

                    context.Urls.Add(url);
                    context.SaveChanges();
                    NameValueCollection response = new NameValueCollection();
                    response.Add("urlId", url.Id.ToString());
                    response.Add("url", url.Url);
                    response.Add("shorUrl", url.Short);
                    response.Add("screenshot", HttpRequest.QueryParams["Host"] + "/app1/" + "screenshots/" + pictureName + ".jpg");
                    return Json(Mvc.Json.Jsonify(response), 201);
                }
            }
        }
    }
}
