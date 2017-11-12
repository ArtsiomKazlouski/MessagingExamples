
using System;

namespace EHR.ServerEvent.Subscriber.Config
{
    public class RabbitConfig
    {

        public string Queue { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string VirtualHost { get; set; }
        public int Port { get; set; }
        public string HostName { get; set; }
        public TimeSpan RequeueOnErrorAfter { get; set; }
    }
}
