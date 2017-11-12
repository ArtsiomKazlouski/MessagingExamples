using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EHR.FhirServer.Exceptions;
using Hl7.Fhir.Model;

namespace EHR.FhirServer.Operations
{
    public class OperationEngine
    {
        private readonly IEnumerable<IOperationHandler> _operationHandlers;

        public OperationEngine(IEnumerable<IOperationHandler> handlers)
        {
            _operationHandlers = handlers;
        }

        public Resource Execute(OperationContext context)
        {
            var operationHandler = _operationHandlers.SingleOrDefault(t => t.CanExecute(context));
            if (operationHandler == null)
                throw new FhirHttpResponseException(HttpStatusCode.NotFound, "Запрашиваемая операция не найдена");

            return operationHandler.Execute(context);
        }
    }
}
