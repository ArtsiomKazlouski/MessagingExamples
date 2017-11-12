using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;

namespace EHR.ServerEvent.Publisher.Mappers.PayloadReader
{
    public class PayloadXmlReader : PayloadReader
    {
        public PayloadXmlReader() : base(ContentType.XML_CONTENT_HEADERS)
        {
        }

        public override Resource Read(string base64String)
        {
            using (var memmoryStream = new MemoryStream(Convert.FromBase64String(base64String)))
            {
                using (var reader = new StreamReader(memmoryStream, Encoding.UTF8, true))
                {
                    return FhirParser.ParseResourceFromXml(reader.ReadToEnd());
                }
            }
        }
    }
}
