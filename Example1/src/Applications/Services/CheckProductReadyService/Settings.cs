using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckProductReadyService
{
    public class Settings
    {
        public string MetainfoServiceEndpoint { get; } = ConfigurationManager.AppSettings["informationResourceManagementApiEndpoint"];
        public string TokenEndpoint { get; } = ConfigurationManager.AppSettings["identityServerTokenEndpoint"];
        public string Client { get; } = ConfigurationManager.AppSettings["identityServer:client"];
        public string Clientsecret { get; } = ConfigurationManager.AppSettings["identityServer:clientsecret"];
        public string RequestedScopes { get; } = ConfigurationManager.AppSettings["identityServer:requestedScopes"];
        public string FinishedProductExchanger { get; } = ConfigurationManager.AppSettings["finishedProductExchanger"];
        public string ServiceName { get; } = "CheckProductReadyService";
    }
}
