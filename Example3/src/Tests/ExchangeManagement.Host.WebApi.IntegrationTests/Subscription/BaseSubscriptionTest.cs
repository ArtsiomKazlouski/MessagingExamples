using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ExchangeManagement.Contract;
using ExchangeManagement.Contract.ServiceContracts;
using ExchangeManagement.Wrapper;
using Xunit;

namespace ExchangeManagement.Host.WebApi.IntegrationTests.Subscription
{
    [Collection("WebServer")]
    public class BaseSubscriptionTest
    {
        protected readonly WebServerFixture Fixture;

        protected ISubscriptionService SubscriptionService;

        public BaseSubscriptionTest(WebServerFixture fixture)
        {
            Fixture = fixture;
            SubscriptionService = new SubscriptionService(fixture.RestClientNotAuthorize);
        }
        
        public Contract.Subscription GetValidSubscriptionForCreateRequest()
        {
            return new Contract.Subscription()
            {
                Url = "test",
                Query = "test",
                AuthorizationOptions = new AuthorizationOptions()
                {
                    TokenEndpoint = "test",
                    ClientId = "test",
                    ClientSecret = "test"
                },
                IsDownloadResourceFile = true,
                IsActive = true
            };
        }

        public Contract.Subscription GetValidSubscriptionForUpdateRequest()
        {
            return new Contract.Subscription()
            {
                Url = "test2",
                Query = "test2",
                AuthorizationOptions = new AuthorizationOptions()
                {
                    TokenEndpoint = "test2",
                    ClientId = "test2",
                    ClientSecret = "test2"
                },
                IsDownloadResourceFile = false,
                IsActive = false
            };
        }

        public async Task<Contract.Subscription> CreateSubscriptionTest(Contract.Subscription subscription, HttpStatusCode expectedStatusCode)
        {
            var result = await Fixture.AuthorizeClient.PostAsXmlAsync("subscription", subscription);
            Assert.Equal(expectedStatusCode, result.StatusCode);

            if (result.IsSuccessStatusCode)
            {
                return await result.Content.ReadAsAsync<Contract.Subscription>();
            }
            return null;
        }

        public async Task UpdateSubscriptionTest(Contract.Subscription subscription, HttpStatusCode expectedStatusCode)
        {
            var result = await Fixture.AuthorizeClient.PutAsXmlAsync($"subscription/{subscription.SubscriptionId}", subscription);
            Assert.Equal(expectedStatusCode, result.StatusCode);
        }

        public async Task<Contract.Subscription> GetSubscriptionTest(long subscriptionId, HttpStatusCode expectedStatusCode)
        {
            var result = await Fixture.AuthorizeClient.GetAsync($"subscription/{subscriptionId}");
            Assert.Equal(expectedStatusCode, result.StatusCode);

            if (result.IsSuccessStatusCode)
            {
                return await result.Content.ReadAsAsync<Contract.Subscription>();
            }
            return null;
        }
        
        public async Task DeleteSubscriptionTest(long subscriptionId, HttpStatusCode expectedStatusCode)
        {
            var result = await Fixture.AuthorizeClient.DeleteAsync($"subscription/{subscriptionId}");
            Assert.Equal(expectedStatusCode, result.StatusCode);
        }

        public async Task<List<Contract.Subscription>> ListSubscriptionsTest(HttpStatusCode expectedStatusCode)
        {
            var result = await Fixture.AuthorizeClient.GetAsync("subscription");
             
            Assert.Equal(expectedStatusCode, result.StatusCode);

            if (result.IsSuccessStatusCode)
            {
                return await result.Content.ReadAsAsync<List<Contract.Subscription>>();
            }
            return null;
        }
    }
}
