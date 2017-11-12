using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace ExchangeManagement.Host.WebApi.IntegrationTests.Subscription.CreateTest
{
    public class CreateInvalid : BaseSubscriptionTest
    {
        public CreateInvalid(WebServerFixture fixture)
               : base(fixture)
        {
        }
        
        private async Task Create(Contract.Subscription subscription, HttpStatusCode expectedStatusCode)
        {
            var subscriptionFromService = await CreateSubscriptionTest(subscription, expectedStatusCode);
            Assert.Null(subscriptionFromService);
        }

        [Fact(DisplayName = "Создание подписки. Неудачная аутентификация. Должен вернуть статус код - Unauthorized", Skip = "Нет реализации авторизации")]
        public async Task CreateSubscriptionNotAuthorize()
        {
            var result = await Fixture.ClientNotAuthorize.PostAsXmlAsync("subscription", new Contract.Subscription());
            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [Theory(DisplayName = "Создание подписки с невалидным полем - SubscriptionId. Должен вернуть статус код BadRequest.")]
        [InlineData(-1)]
        [InlineData(1)]
        public async Task CreateInvalidSubscriptionFieldSubscriptionId(dynamic property)
        {
            var subscription = GetValidSubscriptionForCreateRequest();
            subscription.SubscriptionId = property;
            await Create(subscription, HttpStatusCode.BadRequest);
        }

        [Theory(DisplayName = "Создание подписки с невалидным полем - Url. Должен вернуть статус код BadRequest.")]
        [InlineData("")]
        public async Task CreateInvalidSubscriptionFieldUrl(dynamic property)
        {
            var subscription = GetValidSubscriptionForCreateRequest();
            subscription.Url = property;
            await Create(subscription, HttpStatusCode.BadRequest);
        }

        [Theory(DisplayName = "Создание подписки с невалидным полем - Query. Должен вернуть статус код BadRequest.")]
        [InlineData("")]
        public async Task CreateInvalidSubscriptionFieldQuery(dynamic property)
        {
            var subscription = GetValidSubscriptionForCreateRequest();
            subscription.Query = property;
            await Create(subscription, HttpStatusCode.BadRequest);
        }

        [Theory(DisplayName = "Создание подписки с невалидным полем - TokenEndpoint. Должен вернуть статус код BadRequest.")]
        [InlineData("")]
        public async Task CreateInvalidSubscriptionFieldTokenEndpoint(dynamic property)
        {
            var subscription = GetValidSubscriptionForCreateRequest();
            subscription.AuthorizationOptions.TokenEndpoint = property;
            await Create(subscription, HttpStatusCode.BadRequest);
        }

        [Theory(DisplayName = "Создание подписки с невалидным полем - ClientId. Должен вернуть статус код BadRequest.")]
        [InlineData("")]
        public async Task CreateInvalidSubscriptionFieldClientId(dynamic property)
        {
            var subscription = GetValidSubscriptionForCreateRequest();
            subscription.AuthorizationOptions.ClientId = property;
            await Create(subscription, HttpStatusCode.BadRequest);
        }

        [Theory(DisplayName = "Создание подписки с невалидным полем - ClientSecret. Должен вернуть статус код BadRequest.")]
        [InlineData("")]
        public async Task CreateInvalidSubscriptionFieldClientSecret(dynamic property)
        {
            var subscription = GetValidSubscriptionForCreateRequest();
            subscription.AuthorizationOptions.ClientSecret = property;
            await Create(subscription, HttpStatusCode.BadRequest);
        }
    }
}
