using ExchangeManagement.Contract;
using ExchangeManagement.Contract.Messages;

namespace NotifyRecipientsOfFinishedProductService.UnitTests
{
    public class ServiceTests
    {
        public Subscription GetSubscription(string url)
        {
            return new Subscription()
            {
                Url = url,
                Query = "test",
                AuthorizationOptions = new AuthorizationOptions()
                {
                    TokenEndpoint = "test",
                    ClientId = "test",
                    ClientSecret = "test"
                },
                IsDownloadResourceFile = true
            };
        }
        

        public TaskArguments GetAggregateInformationResource(long infTestId, long demoTestId)
        {
            return new TaskArguments()
            {
                Id = 5,
                A = 3,
                B = 5
            };
        }
    }
}
