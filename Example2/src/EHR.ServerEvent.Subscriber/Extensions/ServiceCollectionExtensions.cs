using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EHR.ServerEvent.Subscriber.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddTypeWithInterfaces(this IServiceCollection services, Type t)
        {
            services.AddTransient(t);
            var ii = t.GetInterfaces();
            foreach (var i in ii)
            {
                services.TryAddTransient(i, t);
            }
        }
    }
}
