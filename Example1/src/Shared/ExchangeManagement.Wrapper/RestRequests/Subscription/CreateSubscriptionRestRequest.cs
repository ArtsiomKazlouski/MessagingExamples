using System.Net.Http;

namespace ExchangeManagement.Wrapper.RestRequests.Subscription
{
    public class CreateSubscriptionRestRequest : BaseSubscriptionRestRequest
    {
        public CreateSubscriptionRestRequest(Contract.Subscription subscription) : base(HttpMethod.Post)
        {
            AddJsonBody(subscription);
        }
    }
}
