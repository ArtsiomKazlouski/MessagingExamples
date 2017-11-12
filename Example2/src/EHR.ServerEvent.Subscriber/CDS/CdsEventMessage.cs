using Hl7.Fhir.Model;

namespace EHR.ServerEvent.Subscriber.Cds
{
    public class CdsEventMessage
    {
        public string ActionCode { get; set; }
        public string ResourceType { get; set; }
        public Resource Resource { get; set; }
    }
}
