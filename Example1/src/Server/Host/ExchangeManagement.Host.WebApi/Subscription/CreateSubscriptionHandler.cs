using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FluentValidation;
using MediatR;

namespace ExchangeManagement.Host.WebApi.Subscription
{
    public class CreateSubscriptionRequest : IAsyncRequest<long>
    {
        public CreateSubscriptionRequest(Contract.Subscription subscription)
        {
            Subscription = subscription;
        }

        /// <summary>
        /// Подписка
        /// </summary>
        public Contract.Subscription Subscription { get; set; }
    }

    public class CreateSubscriptionRequestValidator : AbstractValidator<CreateSubscriptionRequest>
    {
        public CreateSubscriptionRequestValidator()
        {
            RuleFor(p => p.Subscription.SubscriptionId).Must(y => y == default(long)).WithMessage("При создании подписки идентификатор указывать нельзя");
            RuleFor(p => p.Subscription).SetValidator(new SubscriptionValidator());
        }
    }

    public class CreateSubscriptionHandler : IAsyncRequestHandler<CreateSubscriptionRequest, long>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateSubscriptionHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<long> Handle(CreateSubscriptionRequest message)
        {
            string sql = @"INSERT INTO 
	                            subscription([url], 
				                            [query],
				                            [token_endpoint], 
				                            [client_id], 
				                            [client_secret],
                                            [download_resource_file],
                                            [active])
                                OUTPUT inserted.id 
	                            VALUES(@url,
                                    @query,
                                    @tokenEndpoint,
                                    @clientId,
                                    @clientSecret,
                                    @isDownloadRresourceFile,
                                    @active)";
            object param = new
            {
                url = message.Subscription.Url,
                query = message.Subscription.Query,
                tokenEndpoint = message.Subscription.AuthorizationOptions.TokenEndpoint,
                clientId = message.Subscription.AuthorizationOptions.ClientId,
                clientSecret = message.Subscription.AuthorizationOptions.ClientSecret,
                isDownloadRresourceFile = message.Subscription.IsDownloadResourceFile,
                active = message.Subscription.IsActive
            };

            var createdId = (await _unitOfWork.QueryAsync(async (c, t) => await c.QueryAsync<long>(sql: sql, param: param, transaction: t))).Single();
            
            return createdId;
        }
    }
}