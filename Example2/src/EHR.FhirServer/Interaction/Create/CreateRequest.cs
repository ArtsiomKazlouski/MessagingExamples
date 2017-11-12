using EHR.FhirServer.Core;
using EHR.FhirServer.Infrastructure;
using Hl7.Fhir.Model;
using MediatR;

namespace EHR.FhirServer.Interaction.Create
{
    public class CreateRequestValidator : FhirInteractionValidatorBase<CreateRequest>
    {
        public CreateRequestValidator(Conformance conformance) : base(conformance)
        {
        }
    }


    public class CreateRequest : FhirInteractionRequest
    {
        public Resource Resource { get; private set; }

        public CreateRequest(string type, Resource resource) : base(type, Conformance.TypeRestfulInteraction.Create)
        {
            Resource = resource;
        }
    }
}