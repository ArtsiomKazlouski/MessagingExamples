using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EHR.FhirServer.Exceptions;
using Hl7.Fhir.Model;

namespace EHR.FhirServer.Operations
{
    public abstract class OperationHandlerBase:IOperationHandler
    {
        protected abstract string OperationName { get; }

        protected abstract bool ExecuteOnSpecificInstance { get;  }

        protected abstract IEnumerable<ResourceType> SupportedTypes { get; }

        public abstract Resource Execute(OperationContext context);

        public bool CanExecute(OperationContext context)
        {
            //check operation Name
            if (OperationName.Equals(context.OperationName) == false)
                return false;

            //accept specific resource
            if (ExecuteOnSpecificInstance && string.IsNullOrWhiteSpace(context.ResourceId) == false)
                return true;

            //accept specific type
            if ( SupportedTypes.Any(t => t.ToString().Equals(context.ResourceType)) && string.IsNullOrEmpty(context.ResourceType) == false)
                return true;

            //accept global
            if (SupportedTypes.Any(t => t.Equals(context.ResourceType))==false && context.ResourceType == null)
                return true;

            return false;
        }

       
    }

    public static class OperationExtentions
    {
        public static T GetResult<T>(this Resource resource) where T : Resource
        {
            var outcome = resource as OperationOutcome;
            if (outcome != null)
            {
                throw new FhirHttpResponseException(outcome);
            }

            return resource as T;

        }
    }
}
