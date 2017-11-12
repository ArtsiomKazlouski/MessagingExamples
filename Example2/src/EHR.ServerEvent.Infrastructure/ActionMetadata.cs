using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace EHR.ServerEvent.Infrastructure
{
    public class ActionMetadata
    {
        public DateTime ExecutionDateTime { get; set; }

        public ActionRequest ActionRequest { get; set; }

        public ActionResponce ActionResponce { get; set; }

    }

    public class ActionRequest
    {
        public string HttpMethod { get; set; }

        public string Query { get; set; }

        public string URI { get; set; }

        public IEnumerable<KeyValuePair<string,string>> Header { get; set; }

        public string Payload { get; set; }
        public RouteValueDictionary RouteData { get; set; }
    }

    public class ActionResponce
    {
        public string Payload { get; set; }

        public IEnumerable<KeyValuePair<string, string>> Header { get; set; }

        public int StatusCode { get; set; }
    }
}
