using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using ExchangeManagement.Contract.ServiceContracts;
using ExchangeManagement.Wrapper;
using Xunit;

namespace ExchangeManagement.Host.WebApi.IntegrationTests.Subscription.ListTest
{
    public class ListValid : BaseSubscriptionTest
    {
        public ListValid(WebServerFixture fixture) : base(fixture)
        {
            
        }

        [Fact(DisplayName = "Получение всех подписок. Ожидаем статус Ok")]
        public async Task ListSubscriptionsStatusCodeOk()
        {
            var subscriptionsFromServiceBeforeTest = await ListSubscriptionsTest(HttpStatusCode.OK);
            var subscription = GetValidSubscriptionForCreateRequest();
            var subscriptions = new List<Contract.Subscription>();
            for (var i = 0; i < 10; i++)
            {
                var result = await CreateSubscriptionTest(subscription, HttpStatusCode.Created);
                Assert.NotNull(result);
                subscriptions.Add(result);
            }
            
            var subscriptionsFromService = await ListSubscriptionsTest(HttpStatusCode.OK);
            var wrapperResult = SubscriptionService.List();
            Assert.NotNull(subscriptionsFromService);
            Assert.NotNull(wrapperResult);
            Assert.Equal(subscriptionsFromService.Count,wrapperResult.Count);
            Assert.Equal(subscriptions.Count, subscriptionsFromService.Count - subscriptionsFromServiceBeforeTest.Count);
        }
    }
}
