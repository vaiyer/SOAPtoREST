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
        
        public List<Mapping> returnOps (string wsdl, string soapUrl)
        {
            List<Mapping> fullMap = new List<Mapping>();
            XmlTextReader reader = new XmlTextReader(wsdl);
            ServiceDescription backEnd = ServiceDescription.Read(reader);

            Binding binding = null;

            foreach (Binding b in backEnd.Bindings)
            {
                foreach (ServiceDescriptionFormatExtension e in b.Extensions)
                {
                    if (e.GetType == typeof(Soap12Binding))
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


            foreach(PortType pt in backEnd.PortTypes)
            {
                foreach (Operation op in pt.Operations)
                {
                    foreach (OperationMessage msg in op.Messages)
                    {
                        Mapping nextMap = new Mapping();
                        nextMap.Name = op.ToString();
                        nextMap.Method = "POST";
                        if (op.ToString().Contains("get") || op.ToString().Contains("Get") || op.ToString().Contains("GET"))
                        {
                            nextMap.Method = "GET";
                        }
                        else if (op.ToString().Contains("post"))
                        {
                            nextMap.Method = "POST";
                        }
                        else if (op.ToString().Contains("put"))
                        {
                            nextMap.Method = "PUT";
                        }
                        else if (op.ToString().Contains("patch"))
                        {
                            nextMap.Method = "PATCH";
                        }
                        else if (op.ToString().Contains("delete"))
                        {
                            nextMap.Method = "DELETE";
                        }
                        nextMap.RouteTemplate = "" + op.Name;
                        string allParams = "";
                        List<Parameter> parameters = this.getParameters(backEnd, msg.Operation.Name);
                        foreach(Parameter p in parameters){
                            nextMap.RouteTemplate = nextMap.RouteTemplate + "/{" + p.Name + "}";
                            allParams = allParams + "<" + p.Name + ">{" + p.Name + "}</" + p.Name + ">";
                        }
                        nextMap.SoapBody = "<?xml version=\"1.0\" encoding=\"utf-8\"?><soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"><soap:Body>" + allParams + "</soap:Body></soap:Envelope>";
                        nextMap.SoapAction = "http://ws.cdyne.com/WeatherWS/" + nextMap.RouteTemplate;
                        nextMap.SoapUrl = soapUrl;
                        nextMap.ContentType = "text/xml";
                        /*
                        if (msg is OperationInput)
                        {
                            OperationInput oi = msg as OperationInput;
                        }
                        else if (msg is OperationOutput)
                        {
                             
                        }
                        else
                        {
                            
                        }
                         * */
                        fullMap.Add(nextMap);
                    }
                }
            }
            return fullMap;
        }

        public List<Parameter> getParameters(ServiceDescription serviceDescription, string messagePartName)
        {
            List<Parameter> parameters = new List<Parameter>();

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
                                parameters.Add(param);
                            }
                        }
                    }
                }
            }
            return parameters;
        }
    }
}