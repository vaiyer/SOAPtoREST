using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SOAPtoREST.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http;

namespace SOAPtoREST.Controllers
{
    public class SoapRestController : ApiController
    {
        private MapProvider mapProvider;

        public SoapRestController()
	    {
            // TODO - This should really use constructor injection but couldn't get that working.
            this.mapProvider = (MapProvider)System.Web.Mvc.DependencyResolver.Current.GetService(typeof(MapProvider));
	    }

        [HttpDelete, HttpHead, HttpOptions, HttpPatch, HttpPut, HttpPost, HttpGet]
        public async Task<System.Net.Http.HttpResponseMessage> Handler(HttpRequestMessage request)
        {
            var routeData = request.GetRouteData();
            string routeTemplate = routeData.Route.RouteTemplate;

            // go thru all mappings, find the one with the route template that matches this request
            var mapping = this.mapProvider.Mappings.FirstOrDefault(m => m.RouteTemplate == routeTemplate);

            if (mapping == null)
            {
                throw new InvalidOperationException(string.Format(
                    "No matching routeTemplate found for '{0}' from '{1}' mapping(s)",
                    routeTemplate,
                    mapProvider.Mappings.Count));
            }

            var body = (string)mapping.SoapBody;
            foreach (var kvp in routeData.Values)
            {
                body = body.Replace("{" + kvp.Key + "}", kvp.Value as string);
            }

            var action = mapping.SoapAction;
            var url = mapping.SoapUrl;
            var contentType = mapping.ContentType;

            string oRequest = body;
            using (var client = new HttpClient())
            {
                var requestContent = new StringContent(oRequest)
                {
                    Headers = { ContentType = new MediaTypeHeaderValue(contentType) }
                };

                requestContent.Headers.TryAddWithoutValidation("SOAPAction", action);

                using (var response = await client.PostAsync(url, requestContent))
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    var outgoingResponse = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(responseContent)
                        {
                            Headers =
                            {
                                ContentType = new MediaTypeHeaderValue(contentType)
                            }
                        }
                    };

                    return outgoingResponse;
                }
            }
        }
    }
}
