using FluentValidation;

namespace ExchangeManagement.Host.WebApi.Subscription
{
    public class SubscriptionValidator : AbstractValidator<Contract.Subscription>
    {
        public SubscriptionValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            
            RuleFor(p => p.Url).Must(y => !string.IsNullOrWhiteSpace(y)).WithMessage("{PropertyName} не может быть пусто");
            RuleFor(p => p.Query).Must(y => !string.IsNullOrWhiteSpace(y)).WithMessage("{PropertyName} не может быть пусто");
            RuleFor(p => p.AuthorizationOptions).SetValidator(new AuthorizationOptionsValidator());
        }
    }

    public class AuthorizationOptionsValidator : AbstractValidator<Contract.AuthorizationOptions>
    {
        public AuthorizationOptionsValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(p => p.TokenEndpoint).Must(y => !string.IsNullOrWhiteSpace(y)).WithMessage("{PropertyName} не может быть пусто");
            RuleFor(p => p.ClientId).Must(y => !string.IsNullOrWhiteSpace(y)).WithMessage("{PropertyName} не может быть пусто");
            RuleFor(p => p.ClientSecret).Must(y => !string.IsNullOrWhiteSpace(y)).WithMessage("{PropertyName} не может быть пусто");
        }
    }
}