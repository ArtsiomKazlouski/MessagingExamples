using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EHR.FhirServer.Core;
using EHR.FhirServer.Interaction.Read;
using EHR.FhirServer.Operations;
using Hl7.Fhir.Model;
using MediatR;

namespace EHR.FhirServer.Interaction.Operation
{
   
    public class OperationRequestHandler : IRequestHandler<OperationRequest, Resource>
    {
        private readonly OperationEngine _operationEngine;

        public OperationRequestHandler(OperationEngine operationEngine)
        {
            _operationEngine = operationEngine;
        }
      

        public Resource Handle(OperationRequest message)
        {
            return _operationEngine.Execute(message.OperationContext);
        }
    }
}
