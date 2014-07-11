using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SOAPtoREST.Models;
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
            MapProvider mp = new MapProvider("~/map.json");

            int routeNum = 1;
            foreach (var mapping in mp.Mappings)            
            {
                string routeTemplate = mapping.RouteTemplate;
                config.Routes.MapHttpRoute(
                    name: "DynamicSoapToRestRoute" + routeNum,
                    routeTemplate: routeTemplate,
                    defaults: new
                    {
                        controller = "SoapRest",
                        action = "Handler",
                        routeTemplate = routeTemplate
                    },
                    constraints: new {
                        httpMethod = new HttpMethodConstraint(new HttpMethod(mapping.Method))
                    });

                routeNum++;
            }
        }
    }
}
