using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using Microsoft.Net.Http.Headers;

namespace EHR.FhirServer.Formatters.Output
{
    public class FhirXmlMediaTypeOutputFormatter : BaseFhirOutputFormatter
    {
        public FhirXmlMediaTypeOutputFormatter() : base(ContentType.XML_CONTENT_HEADERS.Select(MediaTypeHeaderValue.Parse))
        {
           
        }


        protected override void Serialize(Type type, Resource value, TextWriter writer, Encoding encoding)
        {
            using (var xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings {OmitXmlDeclaration = true, Encoding = encoding }))
            {
                FhirSerializer.SerializeResource(value, xmlWriter);
            }
        }
    }
}