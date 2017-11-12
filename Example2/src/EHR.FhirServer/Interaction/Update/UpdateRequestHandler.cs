using EHR.FhirServer.Core;
using Hl7.Fhir.Model;
using MediatR;

namespace EHR.FhirServer.Interaction.Update
{
    public class UpdateRequestHandler : IRequestHandler<UpdateRequest, Resource>
    {
        private readonly IFhirBase _fhirBase;
        private readonly IMediator _mediator;

        public UpdateRequestHandler(IFhirBase fhirBase, IMediator mediator)
        {
            _fhirBase = fhirBase;
            _mediator = mediator;
        }

        public Resource Handle(UpdateRequest message)
        {
            var result = _fhirBase.Update(message.Resource);

          
            return result;
        }
    }
}