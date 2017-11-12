using AutoMapper;
using EHR.ServerEvent.Infrastructure.Contract;
using EHR.ServerEvent.Subscriber.Contract;

namespace EHR.ServerEvent.Subscriber.Cds
{
    public class CdsServerEventTransformer : ITransformer<ServerEventMessage, CdsEventMessage>
    {

        private readonly IMapper _mapper;

        public CdsServerEventTransformer(IMapper mapper)
        {
            _mapper = mapper;
        }

        public CdsEventMessage Transform(ServerEventMessage src)
        {
            return _mapper.Map<CdsEventMessage>(src);
        }
    }
}
