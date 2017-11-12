using System;
using EasyNetQ;
using EasyNetQ.Topology;
using InfResourceManagement.Shared.Contracts.Messages;
using InfResourceManagement.Shared.Contracts.ServiceContracts;
using InfResourceManagement.Shared.Contracts.Types.InformationResource;

namespace CheckProductReadyService
{
    public class CheckProductReadyService
    {
        private readonly IAdvancedBus _bus;
        private readonly IFinishedProductService _finishedProductService;
        private readonly Settings _settings;
        private IExchange _finishedProductsExchange;

        public CheckProductReadyService(IAdvancedBus bus, IFinishedProductService finishedProductService, Settings settings)
        {
            _bus = bus;
            _finishedProductService = finishedProductService;
            _settings = settings;
        }

        public void Start()
        {
            var infExchanger = _bus.ExchangeDeclare(ExchangerNames.InformationResource, ExchangeType.Topic);
            var demoExchanger = _bus.ExchangeDeclare(ExchangerNames.DemoPicture, ExchangeType.Topic);
            var quickLookExchanger = _bus.ExchangeDeclare(ExchangerNames.QuickLook, ExchangeType.Topic);
            var resourceFileExchanger =
                _bus.ExchangeDeclare(ResourceFileManagment.Contracts.Messages.ExchangerNames.ResourceFile,
                    ExchangeType.Topic);

            _finishedProductsExchange = _bus.ExchangeDeclare(_settings.FinishedProductExchanger, ExchangeType.Fanout);

            var queue = _bus.QueueDeclare("CheckProductReadyQueue");

            _bus.Bind(infExchanger, queue, "*");
            _bus.Bind(demoExchanger, queue, "*");
            _bus.Bind(quickLookExchanger, queue, "*");
            _bus.Bind(resourceFileExchanger, queue, "*");

            _bus.Consume(queue, registration => registration
                .Add<InformationResourceMessage>((message, info) =>{ ConsumeAndProcess(message.Body); })
                .Add<DemopictureMessage>((message, info) => { ConsumeAndProcess(message.Body); })
                .Add<QuickLookMessage>((message, info) => { ConsumeAndProcess(message.Body); })
                .Add<ResourceFileManagment.Contracts.Messages.ResourceFileMessage>((message,info)=>{ConsumeAndProcess(message.Body);})
            );
        }


        public void ConsumeAndProcess(InformationResourceMessage resourceMessage)
        {
            if (resourceMessage == null)
            {
                throw new ArgumentNullException(nameof(resourceMessage));
            }

            Handle(resourceMessage.InformationResourceId);
        }

        public void ConsumeAndProcess(ResourceFileManagment.Contracts.Messages.ResourceFileMessage resourceFileMessage)
        {
            if (resourceFileMessage == null)
            {
                throw new ArgumentNullException(nameof(resourceFileMessage));
            }

            Handle(resourceFileMessage.ResourceFileId);
            
        }

        private void Handle(long informationResourceId)
        {
            var product = _finishedProductService.Get(informationResourceId);
            //if product ready publish it
            _bus.Publish(_finishedProductsExchange, String.Empty, false, new Message<AggregateInformationResourceDetails>(product));
        }


        public void Stop()
        {
            _bus.SafeDispose();
        }
    }

   
}