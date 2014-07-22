using SoapToRest.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SoapToRest.Controllers
{
    public class ManageMapController : ApiController
    {
        private MapProvider mapProvider;
        private WsdlReader wsdlRead = new WsdlReader();

        public ManageMapController()
        {
            this.mapProvider = (MapProvider)System.Web.Mvc.DependencyResolver.Current.GetService(typeof(MapProvider));
        }

        // GET api/managemap
        public List<Mapping> Get()
        {
            return this.mapProvider.Mappings;
        }


        public void Post([FromBody] WsdlUrl data)
        {
            this.mapProvider.Mappings =  this.wsdlRead.returnOps(data.Url, "http://wsf.cdyne.com/WeatherWS/Weather.asmx");
            this.mapProvider.ApplyChanges();
        }

        // PUT api/managemap/5
        public HttpResponseMessage Put([FromBody] List<Mapping> mappings)
        {   
            // This is a little hacky, but will do for now - ideally this should 
            // be organized in the DI container too. And use of global state is bad and inconsistent.
            // This code clears the old routes...

            // then we update the map
            this.mapProvider.Mappings = mappings;
            this.mapProvider.ApplyChanges();

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}