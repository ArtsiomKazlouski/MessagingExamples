using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EHR.Cds.Models
{
    [DataContract]
    public class Service
    {
        public Service(string id, string hook, string name)
        {
            Id = id;
            Hook = hook;
            Name = name;
            Prefetch = new Dictionary<string, string>();
        }

        
        [DataMember(Name = "id")]
        public string Id { get; set; }
        
        [DataMember(Name = "hook")]
        public string Hook { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }
        
        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "prefetch")]
        public IDictionary<string, string> Prefetch { get; set; }
    }
}
