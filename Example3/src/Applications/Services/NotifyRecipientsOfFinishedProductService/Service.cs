using System;
using EasyNetQ;
using EasyNetQ.Topology;
using ExchangeManagement.Contract.Messages;
using Serilog;

namespace WorkerService
{
    public class Service
    {
        private readonly IAdvancedBus _bus;
        private readonly Settings _settings;
        
        private readonly IApiService _apiService;
        private IExchange _retryExchanger;

        public Service(IAdvancedBus bus, Settings settings, IApiService apiService)
        {
            _bus = bus;
            _settings = settings;
            _apiService = apiService;
        }

        public void Start()
        {
            var finishedProductExchanger = _bus.ExchangeDeclare(_settings.FinishedProductExchanger, ExchangeType.Fanout);

            var queue = _bus.QueueDeclare("ProductQueue");

            _bus.Bind(finishedProductExchanger, queue, "*");
            _bus.Bind(_retryExchanger, queue, "*");

            _bus.Consume(queue, registration => registration
                .Add<TaskArguments>((message, info) =>
                {
                    Process(message.Body);
                }));
        }

        


        public void Process(TaskArguments taskArguments)
        {
            Log.Verbose($"RecievedMessage with id={taskArguments.Id}");
            
            var result = new TaskCalculationResult()
            {
                Id = taskArguments.Id,
                Result = taskArguments.A + taskArguments.B
            };

            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(result.Result));

            _apiService.SendResults(result);
        }

       

        public void Stop()
        {
            _bus.SafeDispose();
        }
    }
}
