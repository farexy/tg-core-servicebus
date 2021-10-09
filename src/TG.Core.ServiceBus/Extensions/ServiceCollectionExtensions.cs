using System;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Azure.ServiceBus.Primitives;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using TG.Core.ServiceBus.Builders;
using TG.Core.ServiceBus.Options;

namespace TG.Core.ServiceBus.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static ServiceBusConfigurationBuilder AddServiceBus(this IServiceCollection services, IConfiguration configuration)
        {
            return AddServiceBus(services, opt => opt.Endpoint = configuration.GetConnectionString("ServiceBus"));
        }

        public static ServiceBusConfigurationBuilder AddServiceBus(this IServiceCollection services, Action<ServiceBusOptions> setup)
        {
            services.Configure(setup);
            return new ServiceBusConfigurationBuilder(services);
        }
        
        internal static void TryAddServiceBusManagementClient(this IServiceCollection services)
        {
            services.TryAddSingleton(provider =>
            {
                var options = provider.GetRequiredService<IOptions<ServiceBusOptions>>();
                return new ManagementClient(options.Value.Endpoint, new ManagedIdentityTokenProvider());
            });
        }
    }
}
