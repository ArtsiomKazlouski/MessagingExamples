using System;
using System.Collections.Generic;
using System.Linq;
using EasyNetQ;
using EasyNetQ.Topology;
using ExchangeManagement.Contract;
using ExchangeManagement.Contract.ServiceContracts;
using InfResourceManagement.Shared.Contracts.Types.InformationResource;
using Serilog;

namespace NotifyRecipientsOfFinishedProductService
{
    public class Service
    {
        private readonly IAdvancedBus _bus;
        private readonly Settings _settings;
        private readonly ITimeDemoPictureService _timeDemoPictureService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IImportService _importService;
        private IExchange _retryExchanger;

        public Service(IAdvancedBus bus, Settings settings, ISubscriptionService subscriptionService, ITimeDemoPictureService timeDemoPictureService, IImportService importService)
        {
            _bus = bus;
            _settings = settings;
            _subscriptionService = subscriptionService;
            _timeDemoPictureService = timeDemoPictureService;
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
                .Add<AggregateInformationResourceDetails>((message, info) =>
                {
                    Process(message.Body);
                })
                .Add<RedeliveribleInformationResource>((message, info) =>
                {
                    ProcessWithRetry(message.Body.Resource,message.Body.Subscriptions,message.Body.DeliveryCount);
                }));
        }

        


        public void Process(AggregateInformationResourceDetails product)
        {
            Log.Verbose($"RecievedProduct with id={product.Resource.Id}");
            //get subscriptions
            var subscriptions = _subscriptionService.List();
            Log.Verbose($"Obtained subscriptions count: {subscriptions.Count}");
            //foreach subscriptions
            foreach (var subscriptionGroup in subscriptions.GroupBy(subscription => subscription.Url ))
            {
                Log.Verbose($"Check consumer uri:{subscriptionGroup.Key}");

                ProcessWithRetry(product,subscriptionGroup.ToList());
            }
        }

        public void ProcessWithRetry(AggregateInformationResourceDetails product, IList<Subscription> subscriptionGroup, int tryIndex = 0)
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
                        nextDelay = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                        break;
                    case 2:
                        nextDelay = (int)TimeSpan.FromMinutes(5).TotalMilliseconds;
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

            var redeliveribleInformationResource = new RedeliveribleInformationResource()
            {
                DeliveryCount = deliveryCount,
                Resource = product,
                Subscriptions = subscriptionGroup
            };

            _bus.Publish(_retryExchanger, String.Empty, true, new Message<RedeliveribleInformationResource>(redeliveribleInformationResource , properties));
        }

        public void ProcessSubscriptionGroup(AggregateInformationResourceDetails product, IList<Subscription> subscriptionGroup, int tryIndex = 0)
        {
            foreach (var subscription in subscriptionGroup.Where(s=>s.IsDownloadResourceFile))
            {
                if (!IsHaveResult(subscription, product.Resource.Id))
                {
                    continue;
                }

                //if has any result => send to consumer
                _importService.Import(subscription, product);
                Log.Information($"Notify by suscription id: {subscription.SubscriptionId}, uri {subscription.Url}");

                return;
            }

            foreach (var subscription in subscriptionGroup.Where(s=>!s.IsDownloadResourceFile))
            {
                if (!IsHaveResult(subscription, product.Resource.Id))
                {
                    continue;
                }

                var productCopy=new AggregateInformationResourceDetails
                {
                    Resource = product.Resource,
                    DemoPictures = product.DemoPictures,
                    Previews = product.Previews,
                    Version = product.Version
                };

                //if has any result => send to consumer
                _importService.Import(subscription, productCopy);
                Log.Information($"Notify by suscription id: {subscription.SubscriptionId}, uri {subscription.Url}");

                return;
            }
        }

        private bool IsHaveResult(Subscription subscription, long resouurceId)
        {
            //search by query in service
            var query = subscription.Query + "&informationResourceId=" + resouurceId;

            Log.Verbose($"Query for search: {query}");

            try
            {
                var searchResult = _timeDemoPictureService.Search(query);
                Log.Verbose($"Found demoPictures count: {searchResult.TotalCount}");

                if (searchResult.TotalCount == 0)
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Log.Warning($"problem occured when check subscription id {subscription.SubscriptionId}", e);
                return false;
            }

            return true;
        }

        public void Stop()
        {
            _bus.SafeDispose();
        }
    }
}
