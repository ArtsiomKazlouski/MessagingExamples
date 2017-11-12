using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace ExchangeManagement.Host.WebApi.IntegrationTests.Subscription.DeleteTest
{
    public class DeleteInvalid : BaseSubscriptionTest
    {
        public DeleteInvalid(WebServerFixture fixture)
            : base(fixture)
        {
        }

        [Fact(DisplayName = "Удаление подписки. Неудачная аутентификация. Должен вернуть статус код - Unauthorized", Skip = "Нет реализации авторизации")]
        public async Task DeleteSubscriptionNotAuthorize()
        {
            var result = await Fixture.ClientNotAuthorize.DeleteAsync($"subscription/{1}");
            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [Theory(DisplayName = "Удаление несуществующей подписки")]
        [InlineData(default(long), HttpStatusCode.BadRequest)]
        [InlineData(long.MaxValue, HttpStatusCode.NotFound)]
        public async Task DeleteSubscriptionNotExistExpectedNotFound(dynamic property, dynamic status)
        {
            var result = await CreateSubscriptionTest(GetValidSubscriptionForCreateRequest(), HttpStatusCode.Created);
            Assert.NotNull(result);
            await GetSubscriptionTest(result.SubscriptionId, HttpStatusCode.OK);
            await DeleteSubscriptionTest(property, status);
        }
    }
}
