using SOAPtoREST.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Web.Mvc;

namespace SOAPtoREST
{
    public class SoapToRestApiConfig
    {
        private static HttpConfiguration _config;

        public static void Register(HttpConfiguration config)
        {
            _config = config;
            
            MapProvider mp = DependencyResolver.Current.GetService<MapProvider>();

            foreach (var mapping in mp.Mappings)
            {
                string routeTemplate = mapping.RouteTemplate;
                config.Routes.MapHttpRoute(
                    // TODO - remove the GUID
                    name: mapping.RouteTemplate + Guid.NewGuid().ToString(),
                    routeTemplate: routeTemplate,
                    defaults: new
                    {
                        controller = "SoapRest",
                        action = "Handler"
                    },
                    constraints: new
                    {
                        httpMethod = new HttpMethodConstraint(new HttpMethod(mapping.Method))
                    });
            }
        }

        public static void Register()
        {
            if (_config == null)
            {
                throw new InvalidOperationException("You must first Register(HttpConfiguration) before calling Register()");
            }

            Register(_config);
        }

        public static void Deregister()
        {
            if (_config == null)
            {
                throw new InvalidOperationException("You must first Register(HttpConfiguration) before calling Deregister");
            }

            MapProvider mp = DependencyResolver.Current.GetService<MapProvider>();

            foreach (var mapping in mp.Mappings)
            {
                _config.Routes.Remove(mapping.RouteTemplate);
            }
        }

    }
}