using System;
using System.Net.Http;
using System.Net.Http.Headers;
using ExchangeManagement.Contract;
using InfResourceManagement.Shared.Contracts.Types.InformationResource;
using Newtonsoft.Json;
using Serilog;
using Thinktecture.IdentityModel.Client;

namespace NotifyRecipientsOfFinishedProductService
{
    public class ImportService : IImportService
    {
        private readonly Settings _settings;
        private readonly HttpClient _client;

        public ImportService(HttpClient client,Settings settings)
        {
            _settings = settings;
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public long Import(Subscription subscription, AggregateInformationResourceDetails product)
        {
            Log.Verbose($"Credential of subscription: {subscription.AuthorizationOptions.TokenEndpoint} {subscription.AuthorizationOptions.ClientId} {subscription.AuthorizationOptions.ClientSecret}");

            var auth2Client = new OAuth2Client(
                    new Uri(subscription.AuthorizationOptions.TokenEndpoint),
                    subscription.AuthorizationOptions.ClientId,
                    subscription.AuthorizationOptions.ClientSecret);
            var tokenResult = auth2Client.RequestClientCredentialsAsync(_settings.RequestedScopes).Result;
            Log.Verbose($"Token Result: {JsonConvert.SerializeObject(tokenResult.AccessToken)}");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{tokenResult.AccessToken}");

            var result = _client.PostAsXmlAsync(subscription.Url, product).Result;
            Log.Verbose("Responce Code: " + result.StatusCode);
            return result.Content.ReadAsAsync<InformationResource>().Result.Id;
        }
    }
}
