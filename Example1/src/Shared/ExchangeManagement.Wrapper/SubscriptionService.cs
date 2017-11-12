using System;
using System.Collections.Generic;
using ExchangeManagement.Contract;
using ExchangeManagement.Contract.ServiceContracts;
using ExchangeManagement.Wrapper.RestRequests.Subscription;
using RestSharpClient.Contracts;

namespace ExchangeManagement.Wrapper
{
    public class SubscriptionService : ISubscriptionService
    {

        private readonly IRestClient _client;

        public SubscriptionService(IRestClient client)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            _client = client;
        }

        public Subscription Create(Subscription request)
        {
            return _client.Request<Subscription>(new CreateSubscriptionRestRequest(request));
        }

        public void Delete(long subscriptionId)
        {
            _client.Request(new DeleteSubscriptionRestRequest(subscriptionId));
        }

        public Subscription Update(long subscriptionId, Subscription request)
        {
            return _client.Request<Subscription>(new UpdateSubscriptionRestRequest(subscriptionId, request));
        }

        public Subscription GetById(long subscriptionId)
        {
            return _client.Request<Subscription>(new GetByIdSubscriptionRestRequest(subscriptionId));
        }

        public IList<Subscription> List()
        {
            return _client.Request<IList<Subscription>>(new ListSubscriptionRestRequest());
        }
    }
}
