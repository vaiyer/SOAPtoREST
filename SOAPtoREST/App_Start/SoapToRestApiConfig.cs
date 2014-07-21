using SoapToRest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Web.Mvc;

namespace SoapToRest
{
    public class SoapToRestApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Dependencies are incorrectly ordered here. Why is this class orchestrating 
            // between mp and mrp?
            MapProvider mp = DependencyResolver.Current.GetService<MapProvider>();
            MappedRoute mrp = DependencyResolver.Current.GetService<MappedRoute>();

            mrp.SetMappedRoutes(mp.Mappings);

            config.Routes.Add("MappedRouteProviderRoute", mrp);
        }

    }
}