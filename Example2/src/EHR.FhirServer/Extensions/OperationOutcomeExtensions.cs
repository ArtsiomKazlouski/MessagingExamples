using System;
using System.Linq;
using System.Net;
using Hl7.Fhir.Model;

namespace EHR.FhirServer.Extensions
{
    public static class OperationOutcomeExtensions
    {
        public static HttpStatusCode GetHttpStatusCode(this OperationOutcome operationOutcome)
        {
            HttpStatusCode defaultStatusCode = HttpStatusCode.InternalServerError;

            return operationOutcome
                .Issue
                .SelectMany(i => i.Code.Coding)
                .Where(c => c.System == "http://hl7.org/fhir/http-code")
                .Any(coding => Enum.TryParse(coding.Code, out defaultStatusCode))
                ? defaultStatusCode
                : defaultStatusCode;
        }
    }
}