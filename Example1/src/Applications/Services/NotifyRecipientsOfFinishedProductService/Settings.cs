using System.Configuration;

namespace NotifyRecipientsOfFinishedProductService
{
    public class Settings
    {
        public string MetainfoServiceEndpoint { get; } = ConfigurationManager.AppSettings["informationResourceManagementApiEndpoint"];
        public string SubscriptionServiceEndpoint { get; } = ConfigurationManager.AppSettings["subscriptionManagementApiEndpoint"];
        public string TokenEndpoint { get; } = ConfigurationManager.AppSettings["identityServerTokenEndpoint"];
        public string Client { get; } = ConfigurationManager.AppSettings["identityServer:client"];
        public string Clientsecret { get; } = ConfigurationManager.AppSettings["identityServer:clientsecret"];
        public string RequestedScopes { get; } = ConfigurationManager.AppSettings["identityServer:requestedScopes"];
        public string FinishedProductExchanger { get; } = ConfigurationManager.AppSettings["finishedProductExchanger"];
        public string FinishedProductQueue { get; } = ConfigurationManager.AppSettings["finishedProductQueue"];
        public string ServiceName { get; } = "NotifyRecipientsOfFinishedProductService";
    }
}
