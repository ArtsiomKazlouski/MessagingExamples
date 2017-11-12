using System.Collections.Generic;

namespace EHR.ServerEvent.Subscriber.Audit
{
    public class AuditEventMessage
    {
        public string Registered { get; set; }
        
        public string ActionCode { get; set; }
        
        public string ActionScope { get; set; }
        
        public string Outcome { get; set; }
        
        public string OutcomeDesc { get; set; }
        
        public string Source { get; set; }
        
        public string ResourceId { get; set; }

        public string ResourceVersionId { get; set; }
        
        public string Hash { get; set; }
        
        public string AccessPoint { get; set; }
        
        public string ClientId { get; set; }
        
        public string UserId { get; set; }

        public IList<KeyValuePair<string, string>> Claims { get; set; }
        
        public string PatientId { get; set; }
        

        public string PatientVersionId { get; set; }
        
        public string EncounterId { get; set; }
        
        public string EncounterVersionId { get; set; }
    }
}
