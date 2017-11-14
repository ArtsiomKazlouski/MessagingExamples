using System;
using System.Net.Http;
using System.Net.Http.Headers;
using ExchangeManagement.Contract;
using ExchangeManagement.Contract.Messages;
using Newtonsoft.Json;
using Serilog;
using Thinktecture.IdentityModel.Client;

namespace WorkerService
{
    public class ApiService : IApiService
    {
        private readonly Settings _settings;
        private readonly HttpClient _client;

        public ApiService(HttpClient client,Settings settings)
        {
            _settings = settings;
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public void SendResults(TaskCalculationResult result)
        {
            _client.PostAsJsonAsync("", result);
        }
    }
}
