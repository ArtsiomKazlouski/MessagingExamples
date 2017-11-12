using System;
using System.Threading.Tasks;
using EHR.ServerEvent.Infrastructure.Contract;
using EHR.ServerEvent.Subscriber.Config;
using EHR.ServerEvent.Subscriber.Contract;
using Microsoft.Extensions.Options;
using Nest;

namespace EHR.ServerEvent.Subscriber.Audit
{
    public class AuditElasticWriter:IWriter<AuditEventMessage>
    {
        private readonly IElasticClient _client;

        public AuditElasticWriter(IOptions<ElasticSettings> conf)
        {
            if (conf == null)
            {
                throw new ArgumentNullException(nameof(conf));
            }

            var node = new Uri(conf.Value.Address);
            var settings = new ConnectionSettings(node);
            settings.MapDefaultTypeIndices(dictionary => dictionary.Add(typeof(ServerEventMessage), conf.Value.IdxName));
            _client = new ElasticClient(settings);
        }

        public async Task WriteAsync(AuditEventMessage eventMessage)
        {
            await _client.IndexAsync(eventMessage, descriptor => descriptor.Index<ServerEventMessage>());
        }

        
    }
}