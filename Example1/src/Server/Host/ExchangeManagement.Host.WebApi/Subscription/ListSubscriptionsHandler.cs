using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using ExchangeManagement.Contract;
using MediatR;

namespace ExchangeManagement.Host.WebApi.Subscription
{
    public class ListSubscriptionsRequest : IAsyncRequest<IEnumerable<Contract.Subscription>>
    {
        public ListSubscriptionsRequest()
        {
        }
    }

    public class ListSubscriptionsHandler : IAsyncRequestHandler<ListSubscriptionsRequest, IEnumerable<Contract.Subscription>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ListSubscriptionsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Contract.Subscription>> Handle(ListSubscriptionsRequest message)
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
                        FROM subscription sub;";
            var subscriptionList = (await _unitOfWork.QueryAsync(async (c, t) =>
                    await c.QueryAsync(sql: sql, transaction: t, map: mappFunction, splitOn: "SubscriptionId, TokenEndpoint"))).ToList();

            return subscriptionList;
        }
    }
}