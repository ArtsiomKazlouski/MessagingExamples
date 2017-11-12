using EHR.FhirServer.Core;
using EHR.FhirServer.Infrastructure;
using Hl7.Fhir.Model;
using MediatR;

namespace EHR.FhirServer.Interaction.Read
{
    public class ReadRequestValidator : FhirInteractionValidatorBase<ReadRequest>
    {
        public ReadRequestValidator(Conformance conformance) : base(conformance)
        {
        }
    }


    public class ReadRequest : FhirInteractionRequest
    {
      
        public string Id { get;  private set; }

        public ReadRequest(string type, string id):base(type, Conformance.TypeRestfulInteraction.Read)
        {
          
            Id = id;
        }
    }


}