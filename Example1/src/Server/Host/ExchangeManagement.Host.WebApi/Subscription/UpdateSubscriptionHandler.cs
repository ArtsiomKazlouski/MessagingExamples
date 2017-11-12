using System.Linq;
using System.Threading.Tasks;
using Dapper;
using ExchangeManagement.Host.WebApi.Exceptions;
using FluentValidation;
using MediatR;

namespace ExchangeManagement.Host.WebApi.Subscription
{
    public class UpdateSubscriptionRequest : IAsyncRequest
    {
        public UpdateSubscriptionRequest(Contract.Subscription subscription)
        {
            Subscription = subscription;
        }

        /// <summary>
        /// Подписка
        /// </summary>
        public Contract.Subscription Subscription { get; set; }
    }

    public class UpdateSubscriptionRequestValidator : AbstractValidator<UpdateSubscriptionRequest>
    {
        public UpdateSubscriptionRequestValidator()
        {
            RuleFor(p => p.Subscription.SubscriptionId).Must(y => (y > 0)).WithMessage("Идентификатор должен быть больше 0");
            RuleFor(p => p.Subscription).SetValidator(new SubscriptionValidator());
        }
    }

    public class UpdateSubscriptionHandler : AsyncRequestHandler<UpdateSubscriptionRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateSubscriptionHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        protected override async Task HandleCore(UpdateSubscriptionRequest message)
        {
            var alreadyExistsSql = "select cast(case when EXISTS (select 1 from subscription t where t.id = @sequenceId) THEN 1 ELSE 0 END as bit);";
            var alreadyExists = (await _unitOfWork.QueryAsync(async (c, t) => await c.QueryAsync<bool>(sql: alreadyExistsSql, param: new
            {
                sequenceId = message.Subscription.SubscriptionId
            }, transaction: t))).Single();

            if (alreadyExists == false)
            {
                throw new EntityNotFoundException($"Подписка с идентификатором [{message.Subscription.SubscriptionId}] не найдена.");
            }

            var sql = @"UPDATE sub
                        SET [updated_at] = DEFAULT
                        , [url] = @url
                        , [query] = @query
                        , [token_endpoint] = @tokenEndpoint
                        , [client_id] = @clientId
                        , [client_secret] = @clientSecret
                        , [download_resource_file]=@isDownloadResourceFile
                        , [active]=@isActive
                        FROM subscription sub
                        WHERE sub.[id] = @id AND datediff(second, sub.[updated_at],  @updatedAt)=0;";
            object param = new
            {
                id = message.Subscription.SubscriptionId,
                url = message.Subscription.Url,
                query = message.Subscription.Query,
                tokenEndpoint = message.Subscription.AuthorizationOptions.TokenEndpoint,
                clientId = message.Subscription.AuthorizationOptions.ClientId,
                clientSecret = message.Subscription.AuthorizationOptions.ClientSecret,
                updatedAt = message.Subscription.UpdatedAt,
                isDownloadResourceFile = message.Subscription.IsDownloadResourceFile,
                isActive = message.Subscription.IsActive
            };

            var updated = await _unitOfWork.QueryAsync(async (c, t) => await c.ExecuteAsync(sql: sql, param: param, transaction: t));
            if (updated != 1)
            {
                throw new EntityUpdateConcurrencyException("");
            }
        }
    }
}