using System.Net.Http;
using ExchangeManagement.Contract.Messages;

namespace ExchangeManagement.Wrapper.RestRequests.Subscription
{
    public class CreateCalcualtionRestRequest : BaseCalcualtionRestRequest
    {
        public CreateCalcualtionRestRequest(TaskArguments task)
            : base( HttpMethod.Post)
        {
            AddJsonBody(task);
        }
    }
}
