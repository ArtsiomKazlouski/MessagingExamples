
using System.Runtime.Serialization;

namespace EHR.Cds.Models
{
    [DataContract]
    public class Link
    {
        public Link(string label, string url = "")
        {
            Label = label;
            Url = url;
        }

        [DataMember(Name = "label")]
        public string Label { get; internal set; }

        [DataMember(Name = "url")]
        public string Url { get; internal set; }
    }
}
