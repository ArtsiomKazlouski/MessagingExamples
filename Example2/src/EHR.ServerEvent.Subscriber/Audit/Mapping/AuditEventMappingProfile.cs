using EHR.ServerEvent.Infrastructure.Contract;

namespace EHR.ServerEvent.Subscriber.Audit.Mapping
{
    public class AuditEventMappingProfile:AutoMapper.Profile
    {
        public AuditEventMappingProfile()
        {
            CreateMap<ServerEventMessage, AuditEventMessage>();
        }
    }
}
