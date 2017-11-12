using System;
using System.Linq;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using Microsoft.Net.Http.Headers;

namespace EHR.FhirServer.Formatters.Input
{
    public class FhirJsonMediaTypeInputFormatter : BaseFhirInputFormatter
    {
        public FhirJsonMediaTypeInputFormatter():base(
            ContentType.JSON_CONTENT_HEADERS.Select(MediaTypeHeaderValue.Parse))
        {
            
          
        }


        protected override Resource Deserialize(Type type, string content)
        {
            return FhirParser.ParseResourceFromJson(content);
        }

       
        

      


    }
}