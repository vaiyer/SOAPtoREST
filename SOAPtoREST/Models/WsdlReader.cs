using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Web.Services.Description;
using System.Web.UI.WebControls;
using System.Xml.Schema;

namespace SoapToRest.Models
{
    public class WsdlReader
    {
        public List<Mapping> returnOps(string wsdl)
        {
            string soapUrl = null;
            List<Mapping> fullMap = new List<Mapping>();
            XmlTextReader reader = new XmlTextReader(wsdl);
            ServiceDescription backEnd = ServiceDescription.Read(reader);

            // HACK - at the moment this is all matching on name, this probably isn't adequate!
            Binding binding = null;

            foreach (Binding b in backEnd.Bindings)
            {
                foreach (ServiceDescriptionFormatExtension e in b.Extensions)
                {
                    if (e.GetType() == typeof(SoapBinding))
                    {
                        binding = b;
                        break;
                    }
                }
                if (binding != null)
                {
                    break;
                }
            }

            // Find Service URL
            foreach (Service s in backEnd.Services)
            {
                foreach (Port p in s.Ports)
                {
                    if (p.Binding.Name == binding.Name)
                    {
                        soapUrl = p.Extensions.OfType<SoapAddressBinding>().First().Location;
                    }
                }
            }

            // Find Operation Binding
            foreach (OperationBinding opb in binding.Operations)
            {
                Mapping map = new Mapping();
                map.Name = opb.Name;
                map.RouteTemplate = opb.Name;
                map.Method = "POST"; //default

                string[] methods = { "GET", "POST", "PUT", "PATCH", "DELETE" };
                foreach (var method in methods)
                {
                    if (map.Name.StartsWith(method, StringComparison.InvariantCultureIgnoreCase))
                    {
                        map.Method = method;
                    }
                }

                if (map.Method == "GET")
                {
                    string allParams = "";
                    var sequence = GetParameters(backEnd, opb.Name);

                    if (sequence.Parameters.Count > 0)
                    {
                        // HACK this will only work for the most simple of bodies...\
                        // HACK yes, using strings to manipulate XML is a bit daft but it's quick! :)
                        foreach (Parameter p in sequence.Parameters)
                        {
                            map.RouteTemplate = map.RouteTemplate + "/{" + p.Name + "}";
                            allParams = allParams + "<" + p.Name + ">{" + p.Name + "}</" + p.Name + ">";
                        }

                        allParams = String.Format("<{0} xmlns=\"{1}\">{2}</{0}>",
                            sequence.QualifiedName.Name,
                            sequence.QualifiedName.Namespace,
                            allParams);
                    }

                    map.SoapBody = "<?xml version=\"1.0\" encoding=\"utf-8\"?><soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"><soap:Body>" + allParams + "</soap:Body></soap:Envelope>";
                }

                map.SoapAction = opb.Extensions.OfType<SoapOperationBinding>().First().SoapAction;
                map.SoapUrl = soapUrl;
                map.ContentType = "text/xml";

                fullMap.Add(map);
            }

            return fullMap;
        }

        public SequenceParameters GetParameters(ServiceDescription serviceDescription, string messagePartName)
        {
            SequenceParameters result = new SequenceParameters();
            result.Parameters = new List<Parameter>();

            Types types = serviceDescription.Types;
            System.Xml.Schema.XmlSchema xmlSchema = types.Schemas[0];

            foreach (var element in xmlSchema.Items.OfType<XmlSchemaElement>())
            {
                if (element.Name == messagePartName)
                {
                    var complexType = element.SchemaType as XmlSchemaComplexType;
                    if (complexType != null)
                    {
                        var particle = complexType.Particle;
                        var sequence = particle as System.Xml.Schema.XmlSchemaSequence;
                        if (sequence != null)
                        {
                            foreach (XmlSchemaElement childElement in sequence.Items)
                            {
                                string parameterName = childElement.Name;
                                string parameterType = childElement.SchemaTypeName.Name;
                                Parameter param = new Parameter();
                                param.Name = parameterName;
                                param.Type = parameterType;
                                result.Parameters.Add(param);
                                result.QualifiedName = element.QualifiedName;
                            }
                        }
                    }
                }
            }
            return result;
        }

        public class SequenceParameters
        {
            public XmlQualifiedName QualifiedName { get; set; }

            public List<Parameter> Parameters { get; set; }
        }
    }
}