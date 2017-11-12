using System.Collections.Generic;
using ProtoBuf;

namespace EHR.ServerEvent.Infrastructure.Contract
{
    [ProtoContract]
    public class ServerEventMessage
    {
        /// <summary>
        /// ISO 8601 
        /// </summary>
        [ProtoMember(1, IsRequired = true)]
        public string Registered { get; set; }
        
        [ProtoMember(2, IsRequired = true)]
        public string ActionCode { get; set; }

        [ProtoMember(3, IsRequired = false)]
        public string ActionScope { get; set; }
        
        [ProtoMember(4, IsRequired = true)]
        public string Outcome { get; set; }

        [ProtoMember(5, IsRequired = false)]
        public string OutcomeDesc { get; set; }
        
        [ProtoMember(6, IsRequired = true)]
        public string Source { get; set; }

        [ProtoMember(7, IsRequired = true)]
        public string ResourceType { get; set; }

        [ProtoMember(8, IsRequired = true)]
        public string ResourceId { get; set; }

        [ProtoMember(9, IsRequired = true)]
        public string ResourceVersionId { get; set; }
        
        [ProtoMember(10, IsRequired = true)]
        public string Hash { get; set; }

        [ProtoMember(11, IsRequired = false)]
        public string AccessPoint { get; set; }

        [ProtoMember(12, IsRequired = true)]
        public string ClientId { get; set; }

        [ProtoMember(13, IsRequired = false)]
        public string UserId { get; set; }

        [ProtoMember(14, IsRequired = false)]
        public IList<KeyValuePair<string, string>> Claims { get; set; }

        [ProtoMember(15, IsRequired = true)]
        public string PatientId { get; set; }
        [ProtoMember(16, IsRequired = true)]
        public string PatientVersionId { get; set; }

        [ProtoMember(17, IsRequired = false)]
        public string EncounterId { get; set; }
        [ProtoMember(18, IsRequired = false)]
        public string EncounterVersionId { get; set; }

        /// <summary>
        /// Serialized resourse
        /// </summary>
        [ProtoMember(19, IsRequired = false)]
        public string Resource { get; set; }
    }
    
}
