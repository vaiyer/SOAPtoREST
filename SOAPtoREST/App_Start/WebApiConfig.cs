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
                string met = (string)mapping.method;
                HttpMethod method = null;
                if(met.Equals("GET")){
                    method = HttpMethod.Get;
                }
                else if(met.Equals("DELETE")){
                    method = HttpMethod.Delete;
                }
                else if (met.Equals("HEAD"))
                {
                    method = HttpMethod.Head;
                }
                else if (met.Equals("OPTIONS"))
                {
                    method = HttpMethod.Options;
                }
                else if (met.Equals("POST"))
                {
                    method = HttpMethod.Post;
                }
                else if (met.Equals("PUT"))
                {
                    method = HttpMethod.Put;
                }
                else if (met.Equals("TRACE"))
                {
                    method = HttpMethod.Trace;
                }
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
                        httpMethod = new HttpMethodConstraint(method)
                    });

                routeNum++;
            }
        }
    }
}
