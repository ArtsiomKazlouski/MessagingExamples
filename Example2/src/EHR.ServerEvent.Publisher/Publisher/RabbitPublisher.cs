using System.IO;
using System.Threading.Tasks;
using EHR.ServerEvent.Infrastructure.Contract;
using EHR.ServerEvent.Infrastructure.Extensions;
using EHR.ServerEvent.Publisher.Config;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace EHR.ServerEvent.Publisher.Publisher
{
    public class RabbitPublisher:IPublisher
    {
        
        private readonly RabbitMqConfig _config;
        private readonly RabbitMQ.Client.IModel _channel;

        public RabbitPublisher(IConnection connection, IOptions<RabbitMqConfig> configOptions)
        {
            _config = configOptions.Value;
            _channel = connection.CreateModel();
        }

        public async Task SendServerEventAsync(ServerEventMessage msg)
        {
            await Task.Run(() =>
            {
                using (var ms = new MemoryStream())
                {
                    ProtoBuf.Serializer.Serialize(ms, msg);
                    _channel.BasicPublish(_config.ExchangeName, msg.GetRoutingGey(_config.RoutingKeyPrefix), true, null,
                        ms.ToArray());
                }
            });
        }

      
    }
}
