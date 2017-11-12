using System;
using EasyNetQ;
using EasyNetQ.Topology;
using ExchangeManagement.Contract.Messages;

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

            _finishedProductsExchange = _bus.ExchangeDeclare(_settings.FinishedProductExchanger, ExchangeType.Fanout);

            var queue = _bus.QueueDeclare("CheckProductReadyQueue");

            _bus.Bind(infExchanger, queue, "*");
            _bus.Bind(demoExchanger, queue, "*");
            _bus.Bind(quickLookExchanger, queue, "*");
            

            _bus.Consume(queue, registration => registration
                .Add<MessageMetadata>((message, info) =>{ ConsumeAndProcess(message.Body); })
            );
        }


       

        public void ConsumeAndProcess(MessageMetadata message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            Handle(message);
        }

        private void Handle(MessageMetadata message)
        {
            if (_finishedProductService.IsReady(message))
            {
                //if product ready publish it
                _bus.Publish(_finishedProductsExchange, String.Empty, false, new Message<MessageMetadata>(message));
            }
        }


        public void Stop()
        {
            _bus.SafeDispose();
        }
    }
}