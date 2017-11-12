using System;
using System.Threading.Tasks;
using EHR.ServerEvent.Subscriber.CDS.Handlers;
using EHR.ServerEvent.Subscriber.CDS.Mediatr.Composition.Delete.Command;
using EHR.ServerEvent.Subscriber.CDS.Mediatr.Composition.Upsert;
using Hl7.Fhir.Model;
using MediatR;

namespace EHR.ServerEvent.Subscriber.Cds.Handlers
{
    public class CompositionHandlers : IConsumerHandler
    {
        private readonly IMediator _mediator;

        public CompositionHandlers(IMediator mediator)
        {
            _mediator = mediator;
        }

        public bool CanHandle(ResourceType respurceType, string actionCode)
        {
            return respurceType == ResourceType.Composition && actionCode.IsUpsertActionCode();
        }

       

        public async Task Handle(Resource resource)
        {
            var composition = resource as Composition;
            if (composition == null)
            {
                throw new ArgumentException("Resource is not \"Composition\" as expected");
            }
            await _mediator.Send(new UpsertCompositionCommand(composition));
        }
    }

    public class DeleteCompositionHandler : IConsumerHandler
    {
        private readonly IMediator _mediator;

        public DeleteCompositionHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public bool CanHandle(ResourceType resourceType, string actionCode)
        {
            return resourceType == ResourceType.Composition && actionCode.IsDeleteActionCode();
        }

        public async Task Handle(Resource resource)
        {
            var composition = resource as Composition;
            if (composition == null)
            {
                throw new ArgumentException("Resource is not \"Composition\" as expected");
            }
            await _mediator.Send(new DeleteCompositionCommand(composition.Id));
        }
    }
}