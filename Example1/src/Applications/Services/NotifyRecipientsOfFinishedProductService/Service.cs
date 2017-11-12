using System;
using System.Collections.Generic;
using System.Linq;
using EasyNetQ;
using EasyNetQ.Topology;
using ExchangeManagement.Contract;
using ExchangeManagement.Contract.Messages;
using ExchangeManagement.Contract.ServiceContracts;
using Serilog;

namespace NotifyRecipientsOfFinishedProductService
{
    public class Service
    {
        private readonly IAdvancedBus _bus;
        private readonly Settings _settings;
        private readonly ICheckSubscriptionService _checkSubscriptionService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IImportService _importService;
        private IExchange _retryExchanger;

        public Service(IAdvancedBus bus, Settings settings, ISubscriptionService subscriptionService, ICheckSubscriptionService checkSubscriptionService, IImportService importService)
        {
            _bus = bus;
            _settings = settings;
            _subscriptionService = subscriptionService;
            _checkSubscriptionService = checkSubscriptionService;
            _importService = importService;
        }

        public void Start()
        {
            var finishedProductExchanger = _bus.ExchangeDeclare(_settings.FinishedProductExchanger, ExchangeType.Fanout);
            _retryExchanger = _bus.ExchangeDeclare("FinifshedProductDelayRetryForNotify", ExchangeType.Fanout, delayed: true);

            var queue = _bus.QueueDeclare("ProductQueue");

            _bus.Bind(finishedProductExchanger, queue, "*");
            _bus.Bind(_retryExchanger, queue, "*");

            _bus.Consume(queue, registration => registration
                .Add<MessageMetadata>((message, info) =>
                {
                    Process(message.Body);
                })
                .Add<RedeliveribleMessage>((message, info) =>
                {
                    ProcessWithRetry(message.Body.Message,message.Body.Subscriptions,message.Body.DeliveryCount);
                }));
        }

        


        public void Process(MessageMetadata message)
        {
            Log.Verbose($"RecievedProduct with id={message.Id}");
            //get subscriptions
            var subscriptions = _subscriptionService.List();
            Log.Verbose($"Obtained subscriptions count: {subscriptions.Count}");
            //foreach subscriptions
            foreach (var subscriptionGroup in subscriptions.GroupBy(subscription => subscription.Url ))
            {
                Log.Verbose($"Check consumer uri:{subscriptionGroup.Key}");

                ProcessWithRetry(message,subscriptionGroup.ToList());
            }
        }

        public void ProcessWithRetry(MessageMetadata product, IList<Subscription> subscriptionGroup, int tryIndex = 0)
        {
            var deliveryCount = tryIndex+1;
            int nextDelay;
            try
            {
                ProcessSubscriptionGroup(product, subscriptionGroup);
                return;
            }
            catch (Exception e)
            {
                Log.Warning(e, $"Error occured when retry count: {deliveryCount}");
                switch (deliveryCount)
                {
                    case 1:
                        nextDelay = (int)TimeSpan.FromSeconds(5).TotalMilliseconds;
                        break;
                    case 2:
                        nextDelay = (int)TimeSpan.FromSeconds(15).TotalMilliseconds;
                        break;
                    default:
                        Log.Error(e, $"Exceeded Retry Count: {deliveryCount}");
                        return;
                }
            }

            var properties = new MessageProperties()
            {
                Headers = new Dictionary<string, object>()
                {
                    {"x-delay", nextDelay }
                }
            };

            var redeliveribleInformationResource = new RedeliveribleMessage()
            {
                DeliveryCount = deliveryCount,
                Message = product,
                Subscriptions = subscriptionGroup
            };

            _bus.Publish(_retryExchanger, String.Empty, true, new Message<RedeliveribleMessage>(redeliveribleInformationResource , properties));
        }

        public void ProcessSubscriptionGroup(MessageMetadata product, IList<Subscription> subscriptionGroup, int tryIndex = 0)
        {
            foreach (var subscription in subscriptionGroup)
            {
                if (!IsHaveResult(subscription, product))
                {
                    continue;
                }

                //if has any result => send to consumer
                _importService.Import(subscription, product);
                Log.Information($"Notify by suscription id: {subscription.SubscriptionId}, uri {subscription.Url}");

                return;
            }

            
        }

        private bool IsHaveResult(Subscription subscription, MessageMetadata message)
        {
            //search by query in service
            var query = subscription.Query;// + "&informationResourceId=" + message.Id;

            Log.Verbose($"Query for search: {query}");

            try
            {
                var checkResult = _checkSubscriptionService.Check(message, query);
                Log.Verbose($"Check result: {checkResult}");

                return checkResult;
            }
            catch (Exception e)
            {
                Log.Warning($"problem occured when check subscription id {subscription.SubscriptionId}", e);
                return false;
            }
        }

        public void Stop()
        {
            _bus.SafeDispose();
        }
    }
}
