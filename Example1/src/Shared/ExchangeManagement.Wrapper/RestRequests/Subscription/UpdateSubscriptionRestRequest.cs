using System.Net.Http;

namespace ExchangeManagement.Wrapper.RestRequests.Subscription
{
    public class UpdateSubscriptionRestRequest : BaseSubscriptionRestRequest
    {
        public UpdateSubscriptionRestRequest(long subscriptionId, Contract.Subscription subscription)
            : base(subscriptionId.ToString(), HttpMethod.Put)
        {
            AddJsonBody(subscription);
        }
    }
}
