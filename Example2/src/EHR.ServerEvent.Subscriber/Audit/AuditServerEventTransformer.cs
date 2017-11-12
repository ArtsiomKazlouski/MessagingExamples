using AutoMapper;
using EHR.ServerEvent.Infrastructure.Contract;
using EHR.ServerEvent.Subscriber.Contract;

namespace EHR.ServerEvent.Subscriber.Audit
{
    public class AuditServerEventTransformer: ITransformer<ServerEventMessage, AuditEventMessage>
    {
        private readonly IMapper _mapper;

        public AuditServerEventTransformer(IMapper mapper)
        {
            _mapper = mapper;
        }

        public AuditEventMessage Transform(ServerEventMessage src)
        {
            return _mapper.Map<AuditEventMessage>(src);
        }
    }
}
