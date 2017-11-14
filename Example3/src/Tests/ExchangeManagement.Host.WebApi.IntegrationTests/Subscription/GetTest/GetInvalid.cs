using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace ExchangeManagement.Host.WebApi.IntegrationTests.Subscription.GetTest
{
    public class GetInvalid : BaseSubscriptionTest
    {
        public GetInvalid(WebServerFixture fixture) : base(fixture)
        {
        }

        [Fact(DisplayName = "Получение подписки. Неудачная аутентификация. Должен вернуть статус код - Unauthorized", Skip = "Нет реализации авторизации")]
        public async Task GetSubscriptionNotAuthorize()
        {
            var result = await Fixture.ClientNotAuthorize.GetAsync($"subscription/{1}");
            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [Fact(DisplayName = "Получение несуществующей подписки. Ожидаем статус NotFound")]
        public async Task GetSubscriptionNotExistExpectedNotFound()
        {
            var result = await CreateSubscriptionTest(GetValidSubscriptionForCreateRequest(), HttpStatusCode.Created);
            Assert.NotNull(result);
            await GetSubscriptionTest(long.MaxValue, HttpStatusCode.NotFound);
        }
    }
}
