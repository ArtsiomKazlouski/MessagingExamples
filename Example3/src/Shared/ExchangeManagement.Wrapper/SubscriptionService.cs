using System;
using System.Collections.Generic;
using ExchangeManagement.Contract;
using ExchangeManagement.Contract.Messages;
using ExchangeManagement.Wrapper.RestRequests.Calculation;
using ExchangeManagement.Wrapper.RestRequests.Subscription;
using RestSharpClient.Contracts;

namespace ExchangeManagement.Wrapper
{
    public class ApiService : IApiService
    {

        private readonly IRestClient _client;

        public ApiService(IRestClient client)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            _client = client;
        }

        public void SendResults(TaskCalculationResult result)
        {
            _client.Request(new SendResultsRestRequest(result));
        }
        
        public TaskArguments Calculate(TaskArguments result)
        {
            return _client.Request<TaskArguments>(new CreateCalcualtionRestRequest(result));
        }

        public TaskCalculationResult GetResult(long taskId)
        {
            return _client.Request<TaskCalculationResult>(new GetCalculationResultRestRequest(taskId));
        }
    }
}
