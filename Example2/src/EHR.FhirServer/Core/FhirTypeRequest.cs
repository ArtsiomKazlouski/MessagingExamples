using Hl7.Fhir.Model;
using MediatR;

namespace EHR.FhirServer.Core
{

    public abstract class FhirTypeRequest : IRequest<Resource>
    {
        public string Type { get;  }
        
        protected FhirTypeRequest(string type)
        {
            Type = type;
          
        }


        protected FhirTypeRequest()
        {
            

        }
    }


    public abstract class FhirInteractionRequest : FhirTypeRequest
    {
         public Conformance.TypeRestfulInteraction Interaction { get;  }

        protected FhirInteractionRequest(string type, Conformance.TypeRestfulInteraction interaction):base(type)
        {
          
            Interaction = interaction;
        }
    }
}