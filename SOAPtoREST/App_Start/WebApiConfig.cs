using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Routing;

namespace SOAPtoREST
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            
            JsonSerializer serializer = new JsonSerializer();
            var file = HostingEnvironment.MapPath("~/map.json");
            JObject o = (JObject)serializer.Deserialize(new JsonTextReader(new StreamReader(file)));
            JArray a = (JArray)o.GetValue("mappings");

            // Web API routes
            /*config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }*/

            config.Routes.MapHttpRoute(
              name: "Default",
              routeTemplate: "api/{controller}/{op}",
              defaults: new
              {
                  controller = "SOAPREST",
                  action = "Handler",
              },
              constraints: new
              {
                  httpMethod = new HttpMethodConstraint(HttpMethod.Post)
              }

            );
        }
    }
}
