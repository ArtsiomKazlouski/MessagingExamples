using System.Net.Http;
using RestSharpClient.Contracts;

namespace ExchangeManagement.Wrapper.RestRequests.Subscription
{
    public abstract class BaseSubscriptionRestRequest : RestRequest
    {
        private const string BaseRoute = "subscription/";

        protected BaseSubscriptionRestRequest(HttpMethod method) : base(BaseRoute, method)
        {
        }

        protected BaseSubscriptionRestRequest(string resource, HttpMethod method) : base(BaseRoute + resource, method)
        {
        }
    }
}
