using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EHR.FhirServer.Core;
using EHR.FhirServer.Infrastructure;
using EHR.FhirServer.Interaction.Read;
using EHR.FhirServer.Operations;
using Hl7.Fhir.Model;
using MediatR;

namespace EHR.FhirServer.Interaction.Operation
{
    public class ReadRequestValidator : FhirResoureTypeValidatorBase<OperationRequest>
    {
        public ReadRequestValidator(Conformance conformance) : base(conformance)
        {
        }
    }


    public class OperationRequest : FhirTypeRequest
    {
        public OperationContext OperationContext { get; }

       
        public OperationRequest(OperationContext operationContext):base(operationContext.ResourceType)
        {
            OperationContext = operationContext;
        }
    }
}
