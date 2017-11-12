using System.Threading.Tasks;
using Hl7.Fhir.Model;

namespace EHR.ServerEvent.Subscriber.Cds.Handlers
{
    public interface IConsumerHandler
    {
        bool CanHandle(ResourceType resourceType, string actionCode);
        Task Handle(Resource resource);
    }
}