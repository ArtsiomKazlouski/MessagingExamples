using System.Threading.Tasks;
using EasyNetQ;
using EasyNetQ.Topology;
using ExchangeManagement.Contract.Messages;
using ExchangeManagement.Host.WebApi.TasksDatabase;
using MediatR;

namespace ExchangeManagement.Host.WebApi.Calculation
{
    public class RequestCalculationRequest:IAsyncRequest<TaskArguments>
    {
        public TaskArguments Arguments { get; set; }

        public RequestCalculationRequest(TaskArguments arguments)
        {
            Arguments = arguments;
        }
    }

    public class RequestCalculationHandler:IAsyncRequestHandler<RequestCalculationRequest,TaskArguments>
    {
        private readonly IAdvancedBus _advancedBus;
        private readonly ITaskRepository _repository;

        public RequestCalculationHandler(IAdvancedBus advancedBus, ITaskRepository repository)
        {
            _advancedBus = advancedBus;
            _repository = repository;
        }

        public async Task<TaskArguments> Handle(RequestCalculationRequest message)
        {
            var newTask = new TaskEntity()
            {
                A = message.Arguments.A,
                B = message.Arguments.B,
            };
            var createdTask = _repository.Create(newTask);

            message.Arguments.Id = createdTask.Id;
            
            var exchanger = _advancedBus.ExchangeDeclare(ExchangerNames.Tasks, ExchangeType.Topic);

            await _advancedBus.PublishAsync(exchanger, MessageTopics.Sum, false, new Message<TaskArguments>(message.Arguments));

            return message.Arguments;
        }
    }
}