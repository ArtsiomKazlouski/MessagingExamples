using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using ExchangeManagement.Contract;
using ExchangeManagement.Host.WebApi.Exceptions;
using FluentValidation;
using MediatR;

namespace ExchangeManagement.Host.WebApi.Subscription
{
    public class GetByIdSubscriptionRequest : IAsyncRequest<Contract.Subscription>
    {
        public GetByIdSubscriptionRequest(long subscriptionId)
        {
            SubscriptionId = subscriptionId;
        }
        
        public long SubscriptionId { get; set; }
    }

    public class GetByIdSubscriptionRequestValidator : AbstractValidator<GetByIdSubscriptionRequest>
    {
        public GetByIdSubscriptionRequestValidator()
        {
            RuleFor(p => p.SubscriptionId).Must(y => (y > 0)).WithMessage("Идентификатор должен быть больше 0");
        }
    }

    public class GetByIdSubscriptionHandler : IAsyncRequestHandler<GetByIdSubscriptionRequest, Contract.Subscription>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetByIdSubscriptionHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Contract.Subscription> Handle(GetByIdSubscriptionRequest message)
        {
            Func
                <Contract.Subscription, AuthorizationOptions,
                    Contract.Subscription> mappFunction =
                        (sub, aut) =>
                        {
                            Contract.Subscription result = sub;
                            result.AuthorizationOptions = new AuthorizationOptions() { TokenEndpoint = aut.TokenEndpoint, ClientId = aut.ClientId, ClientSecret = aut.ClientSecret };
                            return result;
                        };

            var sql = @"SELECT [id] AS SubscriptionId
                            ,[url] AS Url
                            ,[query] AS Query
                            ,[download_resource_file] AS IsDownloadResourceFile
                            ,[active] AS IsActive
                            ,[created_at] AS CreatedAt
                            ,[updated_at] AS UpdatedAt
                            ,[token_endpoint] AS TokenEndpoint
                            ,[client_id] AS ClientId
                            ,[client_secret] AS ClientSecret                            
                            
                        FROM subscription sub
                        WHERE sub.[id] = @id;";
            var subscription = (await _unitOfWork.QueryAsync(async (c, t) => await c.QueryAsync(sql: sql, param: new
            {
                id = message.SubscriptionId
            }, transaction: t, map: mappFunction, splitOn: "SubscriptionId, TokenEndpoint"))).SingleOrDefault();

            if (subscription == null)
            {
                throw new EntityNotFoundException(
                    $"Подписка с идентификатором [{message.SubscriptionId}] не существует");
            }
            return subscription;
        }
    }
}