using System;
using System.Threading.Tasks;
using EHR.ServerEvent.Subscriber.Cds.Mediatr.Conditions.Delete.Command;
using EHR.ServerEvent.Subscriber.Cds.Mediatr.Conditions.Upsert;
using EHR.ServerEvent.Subscriber.CDS.Handlers;
using Hl7.Fhir.Model;
using MediatR;

namespace EHR.ServerEvent.Subscriber.Cds.Handlers
{
    public class ConditionHandlers : IConsumerHandler
    {
        private readonly IMediator _mediator;

        public ConditionHandlers(IMediator mediator)
        {
            _mediator = mediator;
        }

        public bool CanHandle(ResourceType respurceType, string actionCode)
        {
            return respurceType == ResourceType.Condition && actionCode.IsUpsertActionCode();
        }

       

        public async Task Handle(Resource resource)
        {
            var condition = resource as Condition;
            if (condition == null)
            {
                throw new ArgumentException("Resource is not \"Condition\" as expected");
            }
            await _mediator.Send(new UpsertConditionCommand(condition));
        }
    }

    public class DeleteConditionHandler : IConsumerHandler
    {
        private readonly IMediator _mediator;

        public DeleteConditionHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public bool CanHandle(ResourceType resourceType, string actionCode)
        {
            return resourceType == ResourceType.Condition && actionCode.IsDeleteActionCode();
        }

        public async Task Handle(Resource resource)
        {
            var condition = resource as Condition;
            if (condition == null)
            {
                throw new ArgumentException("Resource is not \"Condition\" as expected");
            }
            await _mediator.Send(new DeleteConditionCommand(condition.Id));
        }
    }
}