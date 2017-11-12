using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Newtonsoft.Json;

namespace EHR.Cds.Host
{
    public class ResourceCustomJsonConverter : JsonConverter
    {
        public override bool CanRead { get; } = true;
        public override bool CanWrite { get; } = true;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            FhirSerializer.SerializeResource(value as Resource, writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            var resource = FhirParser.ParseResource(reader);
            return resource;
        }

        public override bool CanConvert(Type objectType)
        {
            if (typeof(Resource).IsAssignableFrom(objectType))
            {
                return true;
            }
            return false;
        }
    }
}

