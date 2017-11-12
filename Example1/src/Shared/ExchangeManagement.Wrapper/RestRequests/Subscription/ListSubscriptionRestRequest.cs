using System.Net.Http;

namespace ExchangeManagement.Wrapper.RestRequests.Subscription
{
    public class ListSubscriptionRestRequest : BaseSubscriptionRestRequest
    {
        public ListSubscriptionRestRequest() : base(HttpMethod.Get)
        {
        }
    }
}
