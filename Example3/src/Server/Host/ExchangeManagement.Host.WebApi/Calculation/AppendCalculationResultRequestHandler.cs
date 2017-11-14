using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ExchangeManagement.Contract.Messages;
using ExchangeManagement.Host.WebApi.SignalR;
using ExchangeManagement.Host.WebApi.TasksDatabase;
using MediatR;
using Microsoft.AspNet.SignalR;

namespace ExchangeManagement.Host.WebApi.Calculation
{

    public class AppendCalculationResultRequest : IAsyncRequest
    {
        public TaskCalculationResult CalculationResult { get; set; }

        public AppendCalculationResultRequest(TaskCalculationResult calculationResult)
        {
            CalculationResult = calculationResult;
        }
    }

    public class AppendCalculationResultMessageProcessor:IMessageProcessor<AppendCalculationResultRequest,Unit>
    {

        private long _id;

        public Task OnBeforeRequestAsync(AppendCalculationResultRequest request)
        {
            _id = request.CalculationResult.Id;
            return Task.FromResult((object)null);
        }

        public void OnRequestHandled(AppendCalculationResultRequest request, Unit responce)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<TaskFinishedNotificationHub>();
            hubContext.Clients.All.CalculationCompleted(_id);
        }
    }

    public class AppendCalculationResultRequestHandler:AsyncRequestHandler<AppendCalculationResultRequest>
    {
        private readonly ITaskRepository _repository;

        public AppendCalculationResultRequestHandler(ITaskRepository repository)
        {
            _repository = repository;
        }
        protected override Task HandleCore(AppendCalculationResultRequest message)
        {
            var task = _repository.Get(message.CalculationResult.Id);

            task.Result = message.CalculationResult.Result;

            _repository.Update(task);

            return Task.FromResult((object)null);
        }
    }
}