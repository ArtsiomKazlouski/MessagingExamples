using EHR.FhirServer.Core;
using Hl7.Fhir.Model;
using MediatR;

namespace EHR.FhirServer.Interaction.Delete
{
    public class DeleteRequestHandler : IRequestHandler<DeleteRequest, Resource>
    {
        private readonly IFhirBase _fhirBase;
        private readonly IMediator _mediator;

        public DeleteRequestHandler(IFhirBase fhirBase, IMediator mediator)
        {
            _fhirBase = fhirBase;
            _mediator = mediator;
        }

        public Resource Handle(DeleteRequest message)
        {
            var result = _fhirBase.Delete(message.Type, message.Id);
            return result;
        }
    }
}