using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SOAPtoREST.Models
{
    public class Mapping
    {
        public string Name { get; set; }
        public string Method { get; set; }
        public string RouteTemplate { get; set; }
        public string SoapBody { get; set; }
        public string SoapAction { get; set; }
        public string SoapUrl { get; set; }
        public string ContentType { get; set; }
    }
}