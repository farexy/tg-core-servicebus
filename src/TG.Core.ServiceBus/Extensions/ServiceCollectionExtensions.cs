using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Azure.ServiceBus.Primitives;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using TG.Core.ServiceBus.Builders;
using TG.Core.ServiceBus.Options;

namespace TG.Core.ServiceBus.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static ServiceBusConfigurationBuilder AddServiceBus(this IServiceCollection services, string serviceName)
        {
            return new ServiceBusConfigurationBuilder(services, serviceName);
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
