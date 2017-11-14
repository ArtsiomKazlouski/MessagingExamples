using System.Net.Http;
using ExchangeManagement.Contract.Messages;

namespace ExchangeManagement.Wrapper.RestRequests.Subscription
{
    public class SendResultsRestRequest : BaseCalcualtionRestRequest
    {
        public SendResultsRestRequest(TaskCalculationResult result) : base("result", HttpMethod.Post)
        {
            AddJsonBody(result);
        }
    }
}
