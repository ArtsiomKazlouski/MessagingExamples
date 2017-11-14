using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ExchangeManagement.Contract.Messages;
using ExchangeManagement.Host.WebApi.TasksDatabase;
using MediatR;

namespace ExchangeManagement.Host.WebApi.Calculation
{
    public class GetCalculationResultRequest:IAsyncRequest<TaskCalculationResult>
    {
        public long TaskId { get; set; }

        public GetCalculationResultRequest(long taskId)
        {
            TaskId = taskId;
        }
    }

    public class GetCalculationResultRequestHandler:IAsyncRequestHandler<GetCalculationResultRequest,TaskCalculationResult>
    {
        private readonly ITaskRepository _taskRepository;

        public GetCalculationResultRequestHandler(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<TaskCalculationResult> Handle(GetCalculationResultRequest message)
        {
            var task = _taskRepository.Get(message.TaskId);

            return await Task.FromResult(new TaskCalculationResult(){Id = task.Id, Result = task.Result.Value});
        }
    }
}