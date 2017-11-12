using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace ExchangeManagement.Host.WebApi.IntegrationTests.Subscription.UpdateTest
{
    public class UpdateValid : BaseSubscriptionTest
    {
        public UpdateValid(WebServerFixture fixture)
            : base(fixture)
        {
        }

        private async Task Update(Contract.Subscription subscription, Contract.Subscription updateSubscription)
        {
            var createSubscription = await CreateSubscriptionTest(subscription, HttpStatusCode.Created);

            var subscriptionFromService = await GetSubscriptionTest(createSubscription.SubscriptionId, HttpStatusCode.OK);
            Assert.NotNull(subscriptionFromService);

            updateSubscription.UpdatedAt = subscriptionFromService.UpdatedAt;
            updateSubscription.SubscriptionId = subscriptionFromService.SubscriptionId;

            await UpdateSubscriptionTest(updateSubscription, HttpStatusCode.OK);

            var subscriptionUpdatedFromService = await GetSubscriptionTest(createSubscription.SubscriptionId, HttpStatusCode.OK);
            Assert.NotNull(subscriptionFromService);

            Assert.Equal(updateSubscription.Url, subscriptionUpdatedFromService.Url);
            Assert.Equal(updateSubscription.Query, subscriptionUpdatedFromService.Query);
            Assert.Equal(updateSubscription.AuthorizationOptions.TokenEndpoint, subscriptionUpdatedFromService.AuthorizationOptions.TokenEndpoint);
            Assert.Equal(updateSubscription.AuthorizationOptions.ClientId, subscriptionUpdatedFromService.AuthorizationOptions.ClientId);
            Assert.Equal(updateSubscription.AuthorizationOptions.ClientSecret, subscriptionUpdatedFromService.AuthorizationOptions.ClientSecret);
            Assert.Equal(updateSubscription.IsDownloadResourceFile, subscriptionUpdatedFromService.IsDownloadResourceFile);
            Assert.Equal(updateSubscription.IsActive, subscriptionUpdatedFromService.IsActive);
        }

        [Fact(DisplayName = "Обновление валидной подписки. Должен вернуть статус код Ok.")]
        public async Task UpdateValidSubscriptionExpectedStatusCodeOk()
        {
            await Update(GetValidSubscriptionForCreateRequest(), GetValidSubscriptionForUpdateRequest());
        }

        [Theory(DisplayName = "Обновление подписки с валидным полем - Url. Должен вернуть статус код Ok.")]
        [InlineData("test")]
        public async Task UpdateValidSubscriptionFieldUrl(dynamic property)
        {
            var updateSubscription = GetValidSubscriptionForUpdateRequest();
            updateSubscription.Url = property;
            await Update(GetValidSubscriptionForCreateRequest(), updateSubscription);
        }

        [Theory(DisplayName = "Обновление подписки с валидным полем - Query. Должен вернуть статус код Ok.")]
        [InlineData("test")]
        public async Task UpdateValidSubscriptionFieldQuery(dynamic property)
        {
            var updateSubscription = GetValidSubscriptionForUpdateRequest();
            updateSubscription.Query = property;
            await Update(GetValidSubscriptionForCreateRequest(), updateSubscription);
        }

        [Theory(DisplayName = "Обновление подписки с валидным полем - TokenEndpoint. Должен вернуть статус код Ok.")]
        [InlineData("test")]
        public async Task UpdateValidSubscriptionFieldTokenEndpoint(dynamic property)
        {
            var updateSubscription = GetValidSubscriptionForUpdateRequest();
            updateSubscription.AuthorizationOptions.TokenEndpoint = property;
            await Update(GetValidSubscriptionForCreateRequest(), updateSubscription);
        }

        [Theory(DisplayName = "Обновление подписки с валидным полем - ClientId. Должен вернуть статус код Ok.")]
        [InlineData("test")]
        public async Task UpdateValidSubscriptionFieldClientId(dynamic property)
        {
            var updateSubscription = GetValidSubscriptionForUpdateRequest();
            updateSubscription.AuthorizationOptions.ClientId = property;
            await Update(GetValidSubscriptionForCreateRequest(), updateSubscription);
        }

        [Theory(DisplayName = "Обновление подписки с валидным полем - ClientSecret. Должен вернуть статус код Ok.")]
        [InlineData("test")]
        public async Task UpdateValidSubscriptionFieldClientSecret(dynamic property)
        {
            var updateSubscription = GetValidSubscriptionForUpdateRequest();
            updateSubscription.AuthorizationOptions.ClientSecret = property;
            await Update(GetValidSubscriptionForCreateRequest(), updateSubscription);
        }
    }
}
