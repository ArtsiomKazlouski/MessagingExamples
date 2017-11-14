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
            var infExchanger = _bus.ExchangeDeclare(ExchangerNames.Tasks, ExchangeType.Topic);

            _finishedProductsExchange = _bus.ExchangeDeclare(_settings.FinishedProductExchanger, ExchangeType.Fanout);

            var queue = _bus.QueueDeclare("CheckProductReadyQueue");

            _bus.Bind(infExchanger, queue, "*");

            _bus.Consume(queue, registration => registration
                .Add<TaskArguments>((message, info) =>{ ConsumeAndProcess(message.Body); })
            );
        }


       

        public void ConsumeAndProcess(TaskArguments taskArguments)
        {
            if (taskArguments == null)
            {
                throw new ArgumentNullException(nameof(taskArguments));
            }

            Handle(taskArguments);
        }

        private void Handle(TaskArguments taskArguments)
        {
            if (_finishedProductService.IsReady(taskArguments))
            {
                //if product ready publish it
                _bus.Publish(_finishedProductsExchange, String.Empty, false, new Message<TaskArguments>(taskArguments));
            }
        }


        public void Stop()
        {
            _bus.SafeDispose();
        }
    }
}