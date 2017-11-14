using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace ExchangeManagement.Host.WebApi.IntegrationTests.Subscription.ListTest
{
    public class ListInvalid : BaseSubscriptionTest
    {
        public ListInvalid(WebServerFixture fixture) : base(fixture)
        {
        }

        [Fact(DisplayName = "Получение всех подписок. Неудачная аутентификация. Должен вернуть статус код - Unauthorized", Skip = "Нет реализации авторизации")]
        public async Task ListSubscriptionsNotAuthorize()
        {
            var result = await Fixture.ClientNotAuthorize.GetAsync($"subscription");
            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        }
    }
}
