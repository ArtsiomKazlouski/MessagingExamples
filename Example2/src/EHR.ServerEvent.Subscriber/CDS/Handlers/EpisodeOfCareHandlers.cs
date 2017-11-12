using System;
using System.Linq;
using System.Threading.Tasks;
using EHR.ServerEvent.Subscriber.Cds.Mediatr.EocCondition.Upsert;
using EHR.ServerEvent.Subscriber.Cds.Mediatr.EoC.Delete.Command;
using EHR.ServerEvent.Subscriber.Cds.Mediatr.EoC.Upsert.Commands;
using EHR.ServerEvent.Subscriber.CDS.Handlers;
using Hl7.Fhir.Model;
using MediatR;

namespace EHR.ServerEvent.Subscriber.Cds.Handlers
{
    public class UpsertEpisodeOfCareHandler : IConsumerHandler
    {
        private const string CondTypeSystem = "http://fhir.org/fhir/StructureDefinition/by-condition-type";
        private readonly IMediator _mediator;

        public UpsertEpisodeOfCareHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public bool CanHandle(ResourceType resourceType, 
            string actionCode)
        {
            return resourceType == ResourceType.EpisodeOfCare && actionCode.IsUpsertActionCode(); ;
        }

        public async Task Handle(Resource resource)
        {
            var episodeOfCare = resource as EpisodeOfCare;
            if (episodeOfCare == null)
            {
                throw new ArgumentException("Resource type is not \"Episode of Care\" as expected");
            }
            await _mediator.Send(new UpsertEpisodeOfCareCommand(episodeOfCare, GetIdFromRef(episodeOfCare.Patient, ResourceType.Patient.ToString())));
            foreach (var condition in episodeOfCare.Condition.Where(c => c.GetExtension(CondTypeSystem)?.Value != null))
            {
                await
                    _mediator.Send(new UpsertEocConditionCommand(episodeOfCare, GetIdFromRef(condition, ResourceType.Condition.ToString())));
            }
        }
        
        private static string GetIdFromRef(ResourceReference refer, string resourceName)
        {
            return string.IsNullOrEmpty(refer.ElementId)
                ? refer.Reference.Replace($"{resourceName}/", string.Empty)
                : refer.ElementId;
        }
    }

    public class DeleteEpisodeOfCareHandler : IConsumerHandler
    {
        private readonly IMediator _mediator;

        public DeleteEpisodeOfCareHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public bool CanHandle(ResourceType resourceType, string actionCode)
        {
            return resourceType == ResourceType.EpisodeOfCare && actionCode.IsDeleteActionCode();
        }

        public async Task Handle(Resource resource)
        {
            var episodeOfCare = resource as EpisodeOfCare;
            if (episodeOfCare == null)
            {
                throw new ArgumentException("Resource is not Episode of Care as expected");
            }
            await _mediator.Send(new DeleteEpisodeOfCareCommand(episodeOfCare.Id));
        }
    }
}