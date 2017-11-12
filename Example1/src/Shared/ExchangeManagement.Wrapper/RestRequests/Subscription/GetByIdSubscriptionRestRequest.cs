using System.Net.Http;

namespace ExchangeManagement.Wrapper.RestRequests.Subscription
{
    public class GetByIdSubscriptionRestRequest : BaseSubscriptionRestRequest
    {
        public GetByIdSubscriptionRestRequest(long subscriptionId) : base(subscriptionId.ToString(), HttpMethod.Get)
        {
        }
    }
}
