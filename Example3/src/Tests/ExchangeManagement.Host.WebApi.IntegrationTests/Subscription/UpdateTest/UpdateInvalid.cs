using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace ExchangeManagement.Host.WebApi.IntegrationTests.Subscription.UpdateTest
{
    public class UpdateInvalid : BaseSubscriptionTest
    {
        public UpdateInvalid(WebServerFixture fixture)
            : base(fixture)
        {
        }

        [Fact(DisplayName = "Обновление подписки. Неудачная аутентификация. Должен вернуть статус код - Unauthorized", Skip = "Нет реализации авторизации")]
        public async Task UpdateSubscriptionNotAuthorize()
        {
            var result = await Fixture.ClientNotAuthorize.PutAsXmlAsync($"subscription/{1}", new Contract.Subscription());
            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [Theory(DisplayName = "Обновление подписки с невалидным полем - SubscriptionId.")]
        [InlineData(default(long), HttpStatusCode.BadRequest)]
        [InlineData(long.MaxValue, HttpStatusCode.NotFound)]
        public async Task UpdateInvalidSubscriptionFieldSubscriptionId(dynamic property, dynamic status)
        {
            await CreateSubscriptionTest(GetValidSubscriptionForCreateRequest(), HttpStatusCode.Created);
            var updateSubscription = GetValidSubscriptionForUpdateRequest();
            updateSubscription.SubscriptionId = property;
            await UpdateSubscriptionTest(updateSubscription, status);
        }
        
        [Theory(DisplayName = "Обновление подписки с невалидным полем - Url. Должен вернуть статус код BadRequest.")]
        [InlineData("")]
        public async Task UpdateInvalidSubscriptionFieldUrl(dynamic property)
        {
            var subscriptionId = (await CreateSubscriptionTest(GetValidSubscriptionForCreateRequest(), HttpStatusCode.Created)).SubscriptionId;
            var updateSubscription = GetValidSubscriptionForUpdateRequest();
            updateSubscription.SubscriptionId = subscriptionId;
            updateSubscription.Url = property;
            await UpdateSubscriptionTest(updateSubscription, HttpStatusCode.BadRequest);
        }

        [Theory(DisplayName = "Обновление подписки с невалидным полем - Query. Должен вернуть статус код BadRequest.")]
        [InlineData("")]
        public async Task UpdateInvalidSubscriptionFieldQuery(dynamic property)
        {
            var subscriptionId = (await CreateSubscriptionTest(GetValidSubscriptionForCreateRequest(), HttpStatusCode.Created)).SubscriptionId;
            var updateSubscription = GetValidSubscriptionForUpdateRequest();
            updateSubscription.SubscriptionId = subscriptionId;
            updateSubscription.Query = property;
            await UpdateSubscriptionTest(updateSubscription, HttpStatusCode.BadRequest);
        }

        [Theory(DisplayName = "Обновление подписки с невалидным полем - TokenEndpoint. Должен вернуть статус код BadRequest.")]
        [InlineData("")]
        public async Task UpdateInvalidSubscriptionFieldTokenEndpoint(dynamic property)
        {
            var subscriptionId = (await CreateSubscriptionTest(GetValidSubscriptionForCreateRequest(), HttpStatusCode.Created)).SubscriptionId;
            var updateSubscription = GetValidSubscriptionForUpdateRequest();
            updateSubscription.SubscriptionId = subscriptionId;
            updateSubscription.AuthorizationOptions.TokenEndpoint = property;
            await UpdateSubscriptionTest(updateSubscription, HttpStatusCode.BadRequest);
        }

        [Theory(DisplayName = "Обновление подписки с невалидным полем - ClientId. Должен вернуть статус код BadRequest.")]
        [InlineData("")]
        public async Task UpdateInvalidSubscriptionFieldClientId(dynamic property)
        {
            var subscriptionId = (await CreateSubscriptionTest(GetValidSubscriptionForCreateRequest(), HttpStatusCode.Created)).SubscriptionId;
            var updateSubscription = GetValidSubscriptionForUpdateRequest();
            updateSubscription.SubscriptionId = subscriptionId;
            updateSubscription.AuthorizationOptions.ClientId = property;
            await UpdateSubscriptionTest(updateSubscription, HttpStatusCode.BadRequest);
        }

        [Theory(DisplayName = "Обновление подписки с невалидным полем - ClientSecret. Должен вернуть статус код BadRequest.")]
        [InlineData("")]
        public async Task UpdateInvalidSubscriptionFieldClientSecret(dynamic property)
        {
            var subscriptionId = (await CreateSubscriptionTest(GetValidSubscriptionForCreateRequest(), HttpStatusCode.Created)).SubscriptionId;
            var updateSubscription = GetValidSubscriptionForUpdateRequest();
            updateSubscription.SubscriptionId = subscriptionId;
            updateSubscription.AuthorizationOptions.ClientSecret = property;
            await UpdateSubscriptionTest(updateSubscription, HttpStatusCode.BadRequest);
        }
    }
}
