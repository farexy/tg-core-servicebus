using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus.Management;

namespace TG.Core.ServiceBus.HealthChecks
{
    internal class AzureServiceBusQueueHealthCheck : AzureServiceBusHealthCheck
    {
        private readonly ManagementClient _client;
        private readonly string _queue;

        public AzureServiceBusQueueHealthCheck(ManagementClient client, string queue)
        {
            _client = client;
            _queue = queue;
        }

        protected override async Task ReachResource()
        {
            await _client.GetQueueRuntimeInfoAsync(_queue);
        }
    }
}
