using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace ExchangeManagement.Host.WebApi.IntegrationTests.Subscription.CreateTest
{
    public class CreateValid : BaseSubscriptionTest
    {
        public CreateValid(WebServerFixture fixture)
               : base(fixture)
        {
        }

        private async Task<long> Create(Contract.Subscription subscription)
        {
            var subscriptionId = (await CreateSubscriptionTest(subscription, HttpStatusCode.Created)).SubscriptionId;

            var subscriptionFromService = await GetSubscriptionTest(subscriptionId, HttpStatusCode.OK);

            Assert.NotNull(subscriptionFromService);
            Assert.NotNull(subscriptionFromService.SubscriptionId);

            Assert.Equal(subscription.Url, subscriptionFromService.Url);
            Assert.Equal(subscription.Query, subscriptionFromService.Query);
            
            Assert.Equal(subscription.AuthorizationOptions.TokenEndpoint, subscriptionFromService.AuthorizationOptions.TokenEndpoint);
            Assert.Equal(subscription.AuthorizationOptions.ClientId, subscriptionFromService.AuthorizationOptions.ClientId);
            Assert.Equal(subscription.AuthorizationOptions.ClientSecret, subscriptionFromService.AuthorizationOptions.ClientSecret);

            Assert.Equal(subscription.IsDownloadResourceFile, subscriptionFromService.IsDownloadResourceFile);
            Assert.Equal(subscription.IsActive, subscriptionFromService.IsActive);
            
            return subscriptionId;
        }

        [Fact(DisplayName = "Создание валидной подписки. Должен вернуть статус код Oк.")]
        public async Task CreateValidSubscriptionExpectedStatusCodeOk()
        {
            await Create(GetValidSubscriptionForCreateRequest());
        }

        [Fact(DisplayName = "Создание валидной подписки. Должен вернуть URL по которому можно получить подписку.")]
        public async Task CreateSubscriptionExpectedUrl()
        {
            var result = await Fixture.AuthorizeClient.PostAsXmlAsync("subscription", GetValidSubscriptionForCreateRequest());
            Assert.Equal(HttpStatusCode.Created, result.StatusCode);

            var resultGet = await Fixture.AuthorizeClient.GetAsync(result.Headers.Location);
            Assert.Equal(HttpStatusCode.OK, resultGet.StatusCode);
        }

        [Fact(DisplayName = "Создание валидной подписки. Должен вернуть только что созданную подписку.")]
        public async Task CreateSubscriptionExpectedSubscription()
        {
            var result = await Fixture.AuthorizeClient.PostAsXmlAsync("subscription", GetValidSubscriptionForCreateRequest());
            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
            var resultCreateEntity = await result.Content.ReadAsAsync<Contract.Subscription>();

            var resultGet = await Fixture.AuthorizeClient.GetAsync(result.Headers.Location);
            Assert.Equal(HttpStatusCode.OK, resultGet.StatusCode);
            var resultGetEntity = await resultGet.Content.ReadAsAsync<Contract.Subscription>();

            Assert.Equal(resultCreateEntity.SubscriptionId, resultGetEntity.SubscriptionId);
        }

        [Theory(DisplayName = "Создание подписки с валидным полем - SubscriptionId. Должен вернуть статус код OK.")]
        [InlineData(default(long))]
        public async Task CreateValidSubscriptionFieldSubscriptionId(dynamic property)
        {
            var subscription = GetValidSubscriptionForCreateRequest();
            subscription.SubscriptionId = property;
            await Create(subscription);
        }

        [Theory(DisplayName = "Создание подписки с валидным полем - Url. Должен вернуть статус код OK.")]
        [InlineData("test")]
        public async Task CreateValidSubscriptionFieldUrl(dynamic property)
        {
            var subscription = GetValidSubscriptionForCreateRequest();
            subscription.Url = property;
            await Create(subscription);
        }

        [Theory(DisplayName = "Создание демоснимка с валидным полем - Query. Должен вернуть статус код Ok.")]
        [InlineData("test")]
        public async Task CreateValidSubscriptionFieldQuery(dynamic property)
        {
            var subscription = GetValidSubscriptionForCreateRequest();
            subscription.Query = property;
            await Create(subscription);
        }

        [Theory(DisplayName = "Создание демоснимка с валидным полем - TokenEndpoint. Должен вернуть статус код Ok.")]
        [InlineData("test")]
        public async Task CreateValidSubscriptionFieldTokenEndpoint(dynamic property)
        {
            var subscription = GetValidSubscriptionForCreateRequest();
            subscription.AuthorizationOptions.TokenEndpoint = property;
            await Create(subscription);
        }

        [Theory(DisplayName = "Создание демоснимка с валидным полем - ClientId. Должен вернуть статус код Ok.")]
        [InlineData("test")]
        public async Task CreateValidSubscriptionFieldClientId(dynamic property)
        {
            var subscription = GetValidSubscriptionForCreateRequest();
            subscription.AuthorizationOptions.ClientId = property;
            await Create(subscription);
        }

        [Theory(DisplayName = "Создание демоснимка с валидным полем - ClientSecret. Должен вернуть статус код Ok.")]
        [InlineData("test")]
        public async Task CreateValidSubscriptionFieldClientSecret(dynamic property)
        {
            var subscription = GetValidSubscriptionForCreateRequest();
            subscription.AuthorizationOptions.ClientSecret = property;
            await Create(subscription);
        }
    }
}
