using Hl7.Fhir.Model;

namespace EHR.FhirServer.Operations
{
    public interface IOperationHandler
    {
        bool CanExecute(OperationContext context);
        Resource Execute(OperationContext context);
    }
}
