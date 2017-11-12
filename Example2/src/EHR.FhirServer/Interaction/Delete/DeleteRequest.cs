using EHR.FhirServer.Core;
using EHR.FhirServer.Infrastructure;
using Hl7.Fhir.Model;
using MediatR;

namespace EHR.FhirServer.Interaction.Delete
{
    public class DeleteRequestValidator : FhirInteractionValidatorBase<DeleteRequest>
    {
        public DeleteRequestValidator(Conformance conformance) : base(conformance)
        {
        }
    }

    public class DeleteRequest: FhirInteractionRequest
    {
      
        public string Id { get; private set; }

        public DeleteRequest(string type, string id) : base(type, Conformance.TypeRestfulInteraction.Delete)
        {
           
            Id = id;
        }
    }
}