using System;
using EHR.ServerEvent.Infrastructure;
using EHR.ServerEvent.Infrastructure.Contract;
using Newtonsoft.Json;

namespace EHR.ServerEvent.Publisher.Dequeue.Postgres
{
    public class Message
    {
        public long Id { get; set; }

        public string Body { get; set; }

        public ActionMetadata ActionMetadata => JsonConvert.DeserializeObject<ActionMetadata>(Body);

        public DateTime CreatedAt { get; set; }
    }
}