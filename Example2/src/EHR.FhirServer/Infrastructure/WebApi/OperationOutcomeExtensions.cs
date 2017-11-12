using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;
using Hl7.Fhir.Model;

namespace EHR.FhirServer.Infrastructure.WebApi
{
    public static class OperationOutcomeExtensions
    {
        public static OperationOutcome GetOperationOutcome()
        {
            var operationOutcome = new OperationOutcome
            {
                Issue = new List<OperationOutcome.OperationOutcomeIssueComponent>()
            };
            return operationOutcome;
        }

        public static OperationOutcome AddFailures(this OperationOutcome operationOutcome, IEnumerable<ValidationFailure> failures)
        {
            operationOutcome.Issue.AddRange(failures.Select(f => new OperationOutcome.OperationOutcomeIssueComponent
            {
                Severity = OperationOutcome.IssueSeverity.Error,
                Details = f.ErrorMessage
            }));

            return operationOutcome;
        }

        public static OperationOutcome AddException(this OperationOutcome operationOutcome, Exception exception)
        {
            Exception e = exception;
            do
            {
                operationOutcome.AddError(e.Message);
                e = e.InnerException;
            }
            while (e != null);
            
            return operationOutcome;
        }

        public static OperationOutcome AddError(this OperationOutcome operationOutcome, string exception)
        {
            operationOutcome.Issue.Add(new OperationOutcome.OperationOutcomeIssueComponent
            {
                Severity = OperationOutcome.IssueSeverity.Error,
                Details = exception
            });
            return operationOutcome;
        }
    }
}