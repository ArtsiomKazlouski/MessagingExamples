using System;
using System.IO;
using System.Linq;
using System.Text;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace EHR.FhirServer.Formatters.Output
{
    public class FhirJsonMediaTypeOutputFormatter : BaseFhirOutputFormatter
    {
        public FhirJsonMediaTypeOutputFormatter():base(ContentType.JSON_CONTENT_HEADERS.Select(MediaTypeHeaderValue.Parse))
        {
           
        }


        protected override void Serialize(Type type, Resource value, TextWriter writer, Encoding encoding)
        {
            JsonWriter jsonWriter = new JsonTextWriter(writer);
            FhirSerializer.SerializeResource(value, jsonWriter);
        }
    }
}