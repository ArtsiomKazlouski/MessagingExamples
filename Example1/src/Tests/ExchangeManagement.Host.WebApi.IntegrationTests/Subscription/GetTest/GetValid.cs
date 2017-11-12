using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace ExchangeManagement.Host.WebApi.IntegrationTests.Subscription.GetTest
{
    public class GetValid : BaseSubscriptionTest
    {
        public GetValid(WebServerFixture fixture) : base(fixture)
        {
        }

        [Fact(DisplayName = "Получение подписки.  Ожидаем статус Ok")]
        public async Task GetSubscriptionStatusCodeOk()
        {
            var subscription = GetValidSubscriptionForCreateRequest();
            var result = await CreateSubscriptionTest(subscription, HttpStatusCode.Created);
            Assert.NotNull(result);
            var subscriptionFromService = await GetSubscriptionTest(result.SubscriptionId, HttpStatusCode.OK);
            Assert.NotNull(subscriptionFromService);
        }

        [Fact(DisplayName = "Получение подписки. Ожидаем статус Ok и проверяем что подписка, которую записывали, ту и получили")]
        public async Task GetSubscriptionStatusCheckFields()
        {
            var subscription = GetValidSubscriptionForCreateRequest();
            var result = await CreateSubscriptionTest(subscription, HttpStatusCode.Created);
            Assert.NotNull(result);
            var subscriptionFromService = await GetSubscriptionTest(result.SubscriptionId, HttpStatusCode.OK);
            Assert.NotNull(subscriptionFromService);

            Assert.Equal(subscription.Url, subscriptionFromService.Url);
            Assert.Equal(subscription.Query, subscriptionFromService.Query);
            Assert.Equal(subscription.AuthorizationOptions.TokenEndpoint, subscriptionFromService.AuthorizationOptions.TokenEndpoint);
            Assert.Equal(subscription.AuthorizationOptions.ClientId, subscriptionFromService.AuthorizationOptions.ClientId);
            Assert.Equal(subscription.AuthorizationOptions.ClientSecret, subscriptionFromService.AuthorizationOptions.ClientSecret);

            Assert.Equal(subscription.IsDownloadResourceFile, subscriptionFromService.IsDownloadResourceFile);
            Assert.Equal(subscription.IsActive, subscriptionFromService.IsActive);
        }
    }
}
