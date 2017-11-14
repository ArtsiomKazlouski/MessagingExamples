using Xunit;

namespace ExchangeManagement.Host.WebApi.IntegrationTests
{
    [CollectionDefinition("WebServer")]
    public class WebServerCollection : ICollectionFixture<WebServerFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
