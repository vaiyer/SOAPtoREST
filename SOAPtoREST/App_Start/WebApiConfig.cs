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
            var file = HostingEnvironment.MapPath("~/map.json");
            var fileJson = File.ReadAllText(file);
            var mappingsFile = JObject.Parse(fileJson);

            int routeNum = 1;
            foreach (dynamic mapping in mappingsFile.Value<JArray>("mappings"))            
            {
                string routeTemplate = (string)mapping.routeTemplate;
                config.Routes.MapHttpRoute(
                    name: "DynamicSoapToRestRoute" + routeNum,
                    routeTemplate: routeTemplate,
                    defaults: new
                    {
                        controller = "SOAPREST",
                        action = "Handler",
                        routeTemplate = routeTemplate
                    },
                    constraints: new {
                        //httpMethod = new HttpMethodConstraint(new HttpMethod((string)mapping.method))
                    });

                routeNum++;
            }
        }
    }
}
