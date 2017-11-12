using System;
using System.Linq;
using EHR.FhirServer.Core;
using FluentValidation;
using FluentValidation.Results;
using Hl7.Fhir.Model;

namespace EHR.FhirServer.Infrastructure
{
    public abstract class FhirResoureTypeValidatorBase<TFhirTypeRequest> : AbstractValidator<TFhirTypeRequest>
        where TFhirTypeRequest : FhirTypeRequest
    {
        protected FhirResoureTypeValidatorBase(Conformance conformance)
        {
            
          

            Custom(p =>
            {
                if (p.Type != null)
                {
                    var supported = conformance.Rest.FirstOrDefault()?
                        .Resource
                        .FirstOrDefault(r => r.Type.Equals(p.Type, StringComparison.OrdinalIgnoreCase));

                    if (supported == null)
                    {
                        return new ValidationFailure(null, $"Неизвестный ресурс - '{ p.Type}'.");
                    }

                    return supported.Type.Equals(p.Type) == false
                        ? new ValidationFailure(null, $"Неверный регистр в названии ресурса, попробуйте '{ supported.Type}'.")
                        : null;
                }
                return null;
            });

          
        }
    }


    public abstract class FhirInteractionValidatorBase<TFhirTypeRequest> : FhirResoureTypeValidatorBase<TFhirTypeRequest>
        where TFhirTypeRequest : FhirInteractionRequest
    {
        protected FhirInteractionValidatorBase(Conformance conformance):base(conformance)
        {
            RuleFor(p => p.Type).NotEmpty();
            RuleFor(p => p.Interaction).NotNull();
           
            Custom(p =>
            {
                return conformance.Rest.FirstOrDefault()?
                    .Resource
                    .Where(r => r.Type.Equals(p.Type, StringComparison.OrdinalIgnoreCase))
                    .SelectMany(r => r.Interaction)
                    .Any(r => r.Code == p.Interaction) != null
                    ? null
                    : new ValidationFailure(null, $"Взаимодействие - '{p.Interaction}' не поддерживается.");
            });
        }
    }
}