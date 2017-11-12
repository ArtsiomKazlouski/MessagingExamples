using EHR.ServerEvent.Infrastructure.Contract;
using EHR.ServerEvent.Subscriber.Cds;
using Hl7.Fhir.Serialization;

namespace EHR.ServerEvent.Subscriber.CDS.Mapping
{
    public class CdsEventMappingProfile : AutoMapper.Profile
    {
        public CdsEventMappingProfile()
        {
            CreateMap<ServerEventMessage, CdsEventMessage>()
                .ForMember(dst => dst.Resource, opt => opt.MapFrom(src =>
                    string.IsNullOrWhiteSpace(src.Resource)
                        ? null
                        : FhirParser.ParseResourceFromJson(src.Resource)))
                ;
        }
    }
}
