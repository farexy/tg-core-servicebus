using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus.Management;

namespace TG.Core.ServiceBus.HealthChecks
{
    internal class AzureServiceBusSubscriptionHealthCheck : AzureServiceBusHealthCheck
    {
        private readonly ManagementClient _client;
        private readonly string _topic;
        private readonly string _subscription;

        public AzureServiceBusSubscriptionHealthCheck(ManagementClient client, string topic, string subscription)
        {
            _client = client;
            _topic = topic;
            _subscription = subscription;
        }

        protected override async Task ReachResource()
        {
            await _client.GetSubscriptionRuntimeInfoAsync(_topic, _subscription);
        }
    }
}
