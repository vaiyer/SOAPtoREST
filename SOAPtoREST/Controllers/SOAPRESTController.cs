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
        private MapProvider _mapProvider;

        public SoapRestController (MapProvider mapProvider)
	    {
            _mapProvider = mapProvider;
	    }

        [HttpDelete, HttpHead, HttpOptions, HttpPatch, HttpPut, HttpPost, HttpGet]
        public async Task<IHttpActionResult> Handler(string routeTemplate, HttpRequestMessage request)
        {
            var file = HostingEnvironment.MapPath("~/map.json");
            var fileJson = File.ReadAllText(file);
            var mappingsFile = JObject.Parse(fileJson);

            // go thru all mappings, find the one with the route template that matches this request
            var mapping = mappingsFile.Value<JArray>("mappings")
                                      .FirstOrDefault((dynamic m) => m.routeTemplate == routeTemplate);

            var routeData = request.GetRouteData();

            var body = (string)mapping.body;
            foreach (var kvp in routeData.Values)
            {
                body = body.Replace("{" + kvp.Key + "}", kvp.Value as string);
            }

            var action = (string)mapping.soapAction;
            var url = (string)mapping.soapUrl;
            var contentType = (string)mapping.contentType;

            //var response = new HttpResponseMessage(HttpStatusCode.OK);
            //response.Content = new StringContent(body);

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

                    return this.ResponseMessage(outgoingResponse);
                }
            }
        }
    }
}
