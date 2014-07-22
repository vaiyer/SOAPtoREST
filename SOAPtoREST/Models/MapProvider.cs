using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace SoapToRest.Models
{
    public class MapProvider
    {
        private string path;

        public MapProvider()
        {
            this.Mappings = new List<Mapping>();
        }

        public MapProvider(string path)
        {
            this.path = HostingEnvironment.MapPath(path);
            var file = this.path;
            var fileJson = File.ReadAllText(file);
            SoapMap map = JsonConvert.DeserializeObject<SoapMap>(fileJson);
            this.Mappings = map.Mappings;
        }

        public void Save(string path)
        {
            var json = JsonConvert.SerializeObject(new SoapMap() { Mappings = this.Mappings });;
            File.WriteAllText(this.path, json);
        }

        public List<Mapping> Mappings;

        public void ApplyChanges()
        {
            this.Save(this.path);
            // HACK Touch web.config to restart the app domain
            File.SetLastWriteTimeUtc(HostingEnvironment.MapPath(@"~/web.config"), DateTime.UtcNow);
        }
    }
}