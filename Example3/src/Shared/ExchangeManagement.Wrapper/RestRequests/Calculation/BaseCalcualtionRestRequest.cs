using System.Net.Http;
using RestSharpClient.Contracts;

namespace ExchangeManagement.Wrapper.RestRequests.Subscription
{
    public abstract class BaseCalcualtionRestRequest : RestRequest
    {
        private const string BaseRoute = "calculation/";

        protected BaseCalcualtionRestRequest(HttpMethod method) : base(BaseRoute, method)
        {
        }

        protected BaseCalcualtionRestRequest(string resource, HttpMethod method) : base(BaseRoute + resource, method)
        {
        }
    }
}
