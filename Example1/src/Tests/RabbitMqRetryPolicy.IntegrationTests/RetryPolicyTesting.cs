using System;
using System.Collections.Generic;
using System.Threading;
using EasyNetQ;
using EasyNetQ.Topology;
using InfResourceManagement.Shared.Contracts.Types.InformationResource;
using NotifyRecipientsOfFinishedProductService;
using Xunit;

namespace RabbitMqRetryPolicy.IntegrationTests
{
    public class RetryPolicyTesting:IDisposable
    {
        private IAdvancedBus _bus;

        public RetryPolicyTesting()
        {
            _bus = RabbitHutch.CreateBus().Advanced;
        }
        
        [Fact(Skip = "For investigation")]
        public void RedeliveringInvestigation()
        {
            var e = _bus.ExchangeDeclare("RetryE", ExchangeType.Fanout);
            var q = _bus.QueueDeclare("RetryQ");

            var er = _bus.ExchangeDeclare("RetryER", ExchangeType.Fanout, delayed:true);

            var areDelivered = new AutoResetEvent(false);
            var areRedelivered = new AutoResetEvent(false);

            var delivered = false;
            var redelivered = false;

            var firstConsume = DateTime.Now;
            var secondConsume = DateTime.Now;

            _bus.Consume(q, registration => registration.Add<RedeliveribleInformationResource>((message, info) =>
            {
                if (message.Body.DeliveryCount == 0)
                {
                    delivered = true;
                    areDelivered.Set();
                    message.Body.DeliveryCount += 1;
                    firstConsume = DateTime.Now;
                    var properties = new MessageProperties()
                    {
                        Headers = new Dictionary<string, object>()
                        {
                            {"x-delay",5000 }
                        }
                    };
                    _bus.Publish(er,String.Empty, true,new Message<RedeliveribleInformationResource>(message.Body,properties));
                    return;
                }

                if (message.Body.DeliveryCount == 1)
                {
                    redelivered = true;
                    areRedelivered.Set();
                    secondConsume = DateTime.Now;
                }
            }));

            _bus.Bind(e, q, "*");
            _bus.Bind(er, q, "*");

            var payload = new RedeliveribleInformationResource(){Resource = new AggregateInformationResourceDetails(){ } };

            _bus.Publish(e,String.Empty, false,new Message<RedeliveribleInformationResource>(payload));
            areDelivered.WaitOne(TimeSpan.FromSeconds(2));
            Assert.True(delivered);


            areRedelivered.WaitOne(TimeSpan.FromSeconds(10));
            Assert.True(redelivered);

            Assert.True(secondConsume-firstConsume>TimeSpan.FromSeconds(5));
        }

        public void Dispose()
        {
            _bus?.Dispose();
        }
    }
}
