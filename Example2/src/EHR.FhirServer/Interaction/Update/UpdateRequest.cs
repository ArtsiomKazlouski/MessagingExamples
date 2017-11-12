using EHR.FhirServer.Core;
using EHR.FhirServer.Infrastructure;
using EHR.FhirServer.Interaction.Search;
using Hl7.Fhir.Model;
using MediatR;

namespace EHR.FhirServer.Interaction.Update
{

    public class UpdateRequestValidator : FhirInteractionValidatorBase<UpdateRequest>
    {
        public UpdateRequestValidator(Conformance conformance) : base(conformance)
        {
        }
    }

    public class UpdateRequest: FhirInteractionRequest
    {
        public string Id { get; private set; }
        public Resource Resource { get; private set; }

        public UpdateRequest(string type, string id, Resource resource) : base(type, Conformance.TypeRestfulInteraction.Update)
        {
            Id = id;
            Resource = resource;
        }
    }
}