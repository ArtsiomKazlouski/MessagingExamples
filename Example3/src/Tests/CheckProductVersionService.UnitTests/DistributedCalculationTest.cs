using System;
using System.Configuration;
using System.Net.Http;
using System.Threading;
using ExchangeManagement.Contract.Messages;
using ExchangeManagement.Wrapper;
using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;
using RestSharpClient;
using RestSharpClient.Contracts;
using Xunit;

namespace Calculation.Test
{

    public class Settings
    {
        public string ApiEndpoint { get; } = ConfigurationManager.AppSettings["ApiEndpoint"];
        public string SignalREndpoint { get; } = ConfigurationManager.AppSettings["SignalREndpoint"];
    }

    public class DistributedCalculationTest
    {
        private ApiService _apiService;
        private IHubProxy _hubProxy;

        public DistributedCalculationTest()
        {
            var  settings = new Settings();

            var client = new HttpMessageHandlerClient(
                () => new HttpClientHandler(),
                settings.ApiEndpoint,
                null,
                new DelegatedDeserializer(JsonConvert.DeserializeObject),
                new DelegatedSerializer(JsonConvert.SerializeObject),
                string.Empty);

            var apiService = new ApiService(client);
            _apiService = apiService;

            var signalRConnection = new HubConnection(settings.SignalREndpoint);

            var hubProxy = signalRConnection.CreateHubProxy("TaskFinishedNotificationHub");
            _hubProxy = hubProxy;
            
        }
        [Fact]
        public void RunDistributedCalculation()
        {
            var mre = new ManualResetEvent(false);
            
            _hubProxy.On<long>("CalculationCompleted", (id) => mre.Set());

            var task = new TaskArguments()
            {
                A = 5,
                B = 8
            };
            var createdTask = _apiService.Calculate(task);

            mre.WaitOne(TimeSpan.FromSeconds(20));

            _apiService.GetResult(createdTask.Id);
        }
    }
}
