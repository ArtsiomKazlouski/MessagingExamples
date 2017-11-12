using EHR.ServerEvent.Subscriber.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EHR.ServerEvent.Subscriber.Audit.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection UseAuditElastic(this IServiceCollection services, IConfigurationRoot configuration)
        {
            services.Configure<ElasticSettings>(configuration.GetSection("ElasticSettings"));
            return services;
        }
    }
}
