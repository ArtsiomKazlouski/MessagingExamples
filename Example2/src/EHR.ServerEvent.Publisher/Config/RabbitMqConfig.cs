namespace EHR.ServerEvent.Publisher.Config
{
    public class RabbitMqConfig
    {
        public RabbitMqConfig()
        {
            ExchangeName = "ehr.serverevent";
            RoutingKeyPrefix = "serverevent";
        }
        
        public string ExchangeName { get; set; }
        public string RoutingKeyPrefix {  get; set; }

        
        public string Username { get; set; }
        public string Password { get; set; }
        public string VirtualHost { get; set; }
        public int Port { get; set; }
        public string HostName { get; set; }



    }
}
