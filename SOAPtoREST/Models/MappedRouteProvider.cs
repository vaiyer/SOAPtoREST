using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Routing;

namespace SoapToRest.Models
{
    public class MappedRouteProvider : HttpRoute
    {
        private HttpRouteCollection mappedRoutes;

        public void SetMappedRoutes(List<IHttpRoute> mappedRoutes) {
            
            HttpRouteCollection newRoutes = new HttpRouteCollection();
          
            foreach (var route in mappedRoutes) {
                newRoutes.Add(route.RouteTemplate, route);
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