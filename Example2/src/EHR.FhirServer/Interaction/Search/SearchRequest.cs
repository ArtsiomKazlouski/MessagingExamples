using System.Collections.Generic;
using EHR.FhirServer.Core;
using EHR.FhirServer.Infrastructure;
using Hl7.Fhir.Model;
using MediatR;

namespace EHR.FhirServer.Interaction.Search
{
    public class SearchRequestValidator : FhirInteractionValidatorBase<SearchRequest>
    {
        public SearchRequestValidator(Conformance conformance) : base(conformance)
        {
        }
    }


    public class SearchRequest  : FhirInteractionRequest
    {
        
        public IEnumerable<KeyValuePair<string, string>> Query { get; private set; }

        public SearchRequest(string type, IEnumerable<KeyValuePair<string,string>> query) : base(type, Conformance.TypeRestfulInteraction.Read)
        {
            Query = query;
        }
    }
}