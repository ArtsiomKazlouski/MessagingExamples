using EHR.FhirServer.Core;
using EHR.FhirServer.Infrastructure;
using EHR.FhirServer.Interaction.Update;
using Hl7.Fhir.Model;
using MediatR;

namespace EHR.FhirServer.Interaction.Vread
{

    public class VreadRequestValidator : FhirInteractionValidatorBase<VreadRequest>
    {
        public VreadRequestValidator(Conformance conformance) : base(conformance)
        {
        }
    }


    public class VreadRequest: FhirInteractionRequest
    {
       
        public string Id { get; private set; }
        public string Vid { get; private set; }

        public VreadRequest(string type, string id, string vid) : base(type, Conformance.TypeRestfulInteraction.Vread)
        {
          
            Id = id;
            Vid = vid;
        }
    }
}