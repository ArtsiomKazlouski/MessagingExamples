using System.Net.Http;

namespace ExchangeManagement.Wrapper.RestRequests.Subscription
{
    public class DeleteSubscriptionRestRequest : BaseSubscriptionRestRequest
    {
        public DeleteSubscriptionRestRequest(long subscriptionId) : base(subscriptionId.ToString(), HttpMethod.Delete)
        {
        }
    }
}
