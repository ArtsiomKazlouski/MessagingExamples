using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hl7.Fhir.Model;

namespace EHR.FhirServer.Operations
{
    public class OperationContext
    {

        public string OperationName { get; }

        public Parameters OperationParameters { get; }

        public string ResourceType { get; }
        public string ResourceId { get; }
        public string ResourceVersion { get; }


        public OperationContext(string operationName, Parameters operationParameters)
        {
            OperationName = operationName;
            OperationParameters = operationParameters;
        }

        public OperationContext(string operationName, Parameters operationParameters, string resourceType) : this(operationName, operationParameters)
        {
            ResourceType = resourceType;
        }

        public OperationContext(
            string operationName,
            Parameters operationParameters,
            string resourceType,
            string resourceId
        ) : this(operationName, operationParameters, resourceType)
        {
            ResourceId = resourceId;
        }

        public OperationContext(
            string operationName,
            Parameters operationParameters,
            string resourceType,
            string resourceId,
            string resourceVersion
        ) : this(operationName, operationParameters, resourceType, resourceId)
        {
            ResourceVersion = resourceVersion;
        }

    }
}
