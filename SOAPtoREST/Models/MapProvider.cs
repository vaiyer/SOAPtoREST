using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace SOAPtoREST.Models
{
    public class MapProvider
    {
        public MapProvider()
        {
            this.Mappings = new List<Mapping>();
        }

        public MapProvider(string path)
        {
            var file = HostingEnvironment.MapPath(path);
            var fileJson = File.ReadAllText(file);
            SoapMap map = JsonConvert.DeserializeObject<SoapMap>(fileJson);
            this.Mappings = map.Mappings;
        }

        public void Save(string path)
        {
            var json = JsonConvert.SerializeObject(new SoapMap() { Mappings = this.Mappings });
            File.WriteAllText(HostingEnvironment.MapPath(path), json);
        }

        public List<Mapping> Mappings;

    }
}