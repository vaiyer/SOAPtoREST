using SoapToRest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SoapToRest.Controllers
{
    public class ManageMapController : ApiController
    {
        private MapProvider mapProvider;
        private MappedRoute mappedRoute;

        public ManageMapController()
        {
            this.mapProvider = (MapProvider) System.Web.Mvc.DependencyResolver.Current.GetService(typeof(MapProvider));
            this.mappedRoute = (MappedRoute)System.Web.Mvc.DependencyResolver.Current.GetService(typeof(MappedRoute));
        }

        // GET api/managemap
        public List<Mapping> Get()
        {
            return this.mapProvider.Mappings;
        }


        // PUT api/managemap/5
        public HttpResponseMessage Put([FromBody] List<Mapping> mappings)
        {   
            // This is a little hacky, but will do for now - ideally this should 
            // be organized in the DI container too. And use of global state is bad and inconsistent.
            // This code clears the old routes...

            // then we update the map
            this.mapProvider.Mappings = mappings;

            this.mappedRoute.SetMappedRoutes(mappings);

            // TODO when confident, save over original location :)
            this.mapProvider.Save("~/map1.json");

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
