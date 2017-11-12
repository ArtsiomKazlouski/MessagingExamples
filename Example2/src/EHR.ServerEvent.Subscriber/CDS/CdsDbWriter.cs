using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EHR.ServerEvent.Subscriber.Cds.Handlers;
using EHR.ServerEvent.Subscriber.Contract;

namespace EHR.ServerEvent.Subscriber.Cds
{
    public class CdsDbWriter : IWriter<CdsEventMessage>
    {
        private readonly IEnumerable<IConsumerHandler> _handlers;

        public CdsDbWriter(IEnumerable<IConsumerHandler> handlers)
        {
            _handlers = handlers;
        }

        public async Task WriteAsync(CdsEventMessage eventMessage)
        {
            var resource = eventMessage.Resource;

            var handler = _handlers.Where(h => h.CanHandle(resource.ResourceType, eventMessage.ActionCode));
            foreach (var consumerHandler in handler)
            {
                await consumerHandler.Handle(resource);
            }
        }

        
    }
}