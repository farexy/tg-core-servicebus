using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus.Management;

namespace TG.Core.ServiceBus.HealthChecks
{
    internal class AzureServiceBusTopicHealthCheck : AzureServiceBusHealthCheck
    {
        private readonly ManagementClient _client;
        private readonly string _topic;

        public AzureServiceBusTopicHealthCheck(ManagementClient client, string topic)
        {
            _client = client;
            _topic = topic;
        }

        protected override async Task ReachResource()
        {
            await _client.GetTopicRuntimeInfoAsync(_topic);
        }
    }
}
