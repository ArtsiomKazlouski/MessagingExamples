using System;
using System.Configuration;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
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
        private HubConnection _signalRConnection;

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

            var signalRConnection = new HubConnection(settings.SignalREndpoint,useDefaultUrl:false);

            _signalRConnection = signalRConnection;

            var hubProxy = signalRConnection.CreateHubProxy("TaskFinishedNotificationHub");
            _hubProxy = hubProxy;
            
        }
        [Fact]
        public async Task RunDistributedCalculation()
        {
            var mre = new ManualResetEvent(false);
            
            _hubProxy.On<long>("CalculationCompleted", (id) =>
            {
                mre.Set();
            });

            await _signalRConnection.Start();

            

            var task = new TaskArguments()
            {
                A = 2,
                B = 1,
                IsReady = true,
            };
            var createdTask = _apiService.Calculate(task);

            mre.WaitOne(TimeSpan.FromSeconds(20));

            var result = _apiService.GetResult(createdTask.Id);

            Assert.Equal(task.A + task.B,result.Result);
        }
    }
}
