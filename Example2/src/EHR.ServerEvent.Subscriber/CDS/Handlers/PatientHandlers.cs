using System;
using System.Linq;
using System.Threading.Tasks;
using EHR.ServerEvent.Subscriber.Cds.Mediatr.Patients.Delete.Command;
using EHR.ServerEvent.Subscriber.Cds.Mediatr.Patients.Upsert;
using EHR.ServerEvent.Subscriber.CDS.Handlers;
using Hl7.Fhir.Model;
using MediatR;

namespace EHR.ServerEvent.Subscriber.Cds.Handlers
{
    public class UpsertPatientHandlers : IConsumerHandler
    {
        private readonly IMediator _mediator;

        public UpsertPatientHandlers(IMediator mediator)
        {
            _mediator = mediator;
        }

        public bool CanHandle(ResourceType respurceType, string actionCode)
        {
            return respurceType == ResourceType.Patient && actionCode.IsUpsertActionCode();
        }

        public async Task Handle(Resource resource)
        {
            var patient = resource as Patient;
            if (patient == null)
            {
                throw new ArgumentException("Resource is not \"Patient\" as expected");
            }
            await _mediator.Send(new UpsertPatientCommand(patient));
        }
    }

    public class DeletePatientHandler : IConsumerHandler
    {
        private readonly IMediator _mediator;

        public DeletePatientHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public bool CanHandle(ResourceType resourceType, string actionCode)
        {
            return resourceType == ResourceType.Patient && actionCode.IsDeleteActionCode();
        }

        public async Task Handle(Resource resource)
        {
            var patient = resource as Patient;
            if (patient == null)
            {
                throw new ArgumentException("Resource is not \"Patient\" as expected");
            }
            await _mediator.Send(new DeletePatientCommand(patient.Id));}
    }
}