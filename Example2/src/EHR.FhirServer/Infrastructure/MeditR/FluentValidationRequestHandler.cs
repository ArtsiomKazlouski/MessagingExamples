using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using MediatR;

namespace EHR.FhirServer.Infrastructure.MeditR
{
    public class FluentValidationRequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IRequestHandler<TRequest, TResponse> _innerHandler;
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public FluentValidationRequestHandler(IRequestHandler<TRequest, TResponse> innerHandler, IEnumerable<IValidator<TRequest>> validators)
        {
            _innerHandler = innerHandler;
            _validators = validators;
        }

        public TResponse Handle(TRequest message)
        {
            var context = new ValidationContext(message);

            var failures =
                _validators
                .Select(v => v.Validate(context))
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Any())
            {
                throw new ValidationException(failures);
            }

            return _innerHandler.Handle(message);
        }
    }
}