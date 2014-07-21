using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Routing;

namespace SoapToRest.Models
{
    public class MappedRoute : HttpRoute
    {
        private HttpRouteCollection mappedRoutes;

        public void SetMappedRoutes(List<Mapping> mappings) 
        {
            HttpRouteCollection newRoutes = new HttpRouteCollection();


            int num = 1;
            foreach (var mapping in mappings)
            {
               
                string routeTemplate = mapping.RouteTemplate;
                    newRoutes.MapHttpRoute(
                    name: "RouteNumber" + num,
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
                    num++;
            }

            this.mappedRoutes = newRoutes;
        }

        public override IHttpRouteData GetRouteData(string virtualPathRoot, System.Net.Http.HttpRequestMessage request)
        {
            foreach (var subRoute in mappedRoutes) {
                var ret = subRoute.GetRouteData(virtualPathRoot, request);
                if (ret != null) return ret;
            }
            return null;
        }
    }
}