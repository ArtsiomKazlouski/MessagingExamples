using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace ExchangeManagement.Host.WebApi.IntegrationTests.Subscription.DeleteTest
{
    public class DeleteValid : BaseSubscriptionTest
    {
        public DeleteValid(WebServerFixture fixture)
            : base(fixture)
        {
        }

        [Fact(DisplayName = "Удаление подписки. Ожидаем статус NoContent")]
        public async Task DeleteSubscriptionStatusCodeOk()
        {
            var result = await CreateSubscriptionTest(GetValidSubscriptionForCreateRequest(), HttpStatusCode.Created);
            Assert.NotNull(result);
            var subscriptionFromService = await GetSubscriptionTest(result.SubscriptionId, HttpStatusCode.OK);
            await DeleteSubscriptionTest(subscriptionFromService.SubscriptionId, HttpStatusCode.NoContent);
            await GetSubscriptionTest(result.SubscriptionId, HttpStatusCode.NotFound);
        }
    }
}
