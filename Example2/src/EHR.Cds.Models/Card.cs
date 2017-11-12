using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EHR.Cds.Models
{
    [DataContract]
    public class Card
    {
        public Card(string summary, Indicator indicator, Link source)
        {
            Summary = summary;
            Indicator = indicator;
            Detail = "";
            Source = source;
            Links = new List<Link>();
        }
        [DataMember(Name = "summary")]
        public string Summary { get; internal set; }
        
        [DataMember(Name = "indicator")]
        public Indicator Indicator { get; internal set; }

        [DataMember(Name = "detail")]
        public string Detail { get; internal set; }

        [DataMember(Name = "source")]
        public Link Source { get; internal set; }

        [DataMember(Name = "links")]
        public IList<Link> Links { get; internal set; }

    }
}
