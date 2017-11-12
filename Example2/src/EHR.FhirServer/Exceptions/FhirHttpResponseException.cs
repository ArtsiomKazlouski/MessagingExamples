using System;
using System.Collections.Generic;
using System.Net;
using EHR.FhirServer.Extensions;
using Hl7.Fhir.Model;

namespace EHR.FhirServer.Exceptions
{
    public class FhirHttpResponseException : Exception
    {
        public OperationOutcome OperationOutcome { get; private set; }
        public HttpStatusCode StatusCode { get; private set; }

        public FhirHttpResponseException(HttpStatusCode statusCode,OperationOutcome operationOutcome)
        {
            OperationOutcome = operationOutcome;
            StatusCode = statusCode;
        }
        public FhirHttpResponseException(OperationOutcome operationOutcome)
            :this(operationOutcome.GetHttpStatusCode(), operationOutcome)
        { }
        public FhirHttpResponseException(string display, string details):this(HttpStatusCode.InternalServerError, display, details) { }

        public FhirHttpResponseException(HttpStatusCode code, string display, string details)
            : this(code, new OperationOutcome
            {
                Issue = new List<OperationOutcome.OperationOutcomeIssueComponent>
                {
                    new OperationOutcome.OperationOutcomeIssueComponent
                    {
                        Severity = OperationOutcome.IssueSeverity.Error,
                        Details = details,
                        Code = new CodeableConcept
                        {
                            Coding = new List<Coding>
                            {
                                new Coding("http://hl7.org/fhir/http-code", code.ToString())
                                {
                                    Display = display
                                },
                            }
                        }
                    }
                }
            })
        {
            
        }


        public FhirHttpResponseException(HttpStatusCode code, string details)
            : this(code,null, details)
        {

        }
    }
}