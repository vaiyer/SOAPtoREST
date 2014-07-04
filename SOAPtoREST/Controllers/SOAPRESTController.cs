using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Hosting;
using System.Web.Http;

namespace SOAPtoREST.Controllers
{
    public class SOAPRESTController : ApiController
    {
        [HttpDelete, HttpHead, HttpOptions, HttpPatch, HttpPut, HttpPost, HttpGet]
        public HttpResponseMessage Handler(HttpRequestMessage request)
        {
            JsonSerializer serializer = new JsonSerializer();
            var file = HostingEnvironment.MapPath("~/map.json");
            JObject o = (JObject)serializer.Deserialize(new JsonTextReader(new StreamReader(file)));
            JArray a = (JArray)o.GetValue("mappings");

            var routeData = request.GetRouteData();

            var exampleBody = "";
            /*foreach (var ex in a)
            {
                if (routeData == o.GetValue(){
                    exampleBody
                }
            }*/

            foreach(var kvp in routeData.Values)
            {
                exampleBody = exampleBody.Replace("{" + kvp.Key + "}", kvp.Value as string);
            }

            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(exampleBody);

            return response;
        }
    }
}
