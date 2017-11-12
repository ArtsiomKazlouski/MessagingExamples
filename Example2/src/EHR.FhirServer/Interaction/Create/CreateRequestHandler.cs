using EHR.FhirServer.Core;
using Hl7.Fhir.Model;
using MediatR;

namespace EHR.FhirServer.Interaction.Create
{
    public class CreateRequestHandler : IRequestHandler<CreateRequest, Resource>
    {
        private readonly IFhirBase _fhirBase;
        private readonly IMediator _mediator;

        public CreateRequestHandler(IFhirBase fhirBase, IMediator mediator)
        {
            _fhirBase = fhirBase;
            _mediator = mediator;
        }

        public Resource Handle(CreateRequest message)
        {
            var result = _fhirBase.Create(message.Resource);

        
            return result;
        }
    }
}