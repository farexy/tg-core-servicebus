using System;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using TG.Core.ServiceBus.HealthChecks;

namespace TG.Core.ServiceBus.Extensions
{
    internal static class HealthChecksBuilderExtensions
    {
        private const string ServiceBusHealthCheckPrefix = "servicebus";
        private const string QueueHealthCheckPrefix = "queue";
        private const string TopicHealthCheckPrefix = "topic";
        private const string SubscriptionHealthCheckPrefix = "subscription";
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);

        public static IHealthChecksBuilder AddServiceBusQueueHealthCheck<TMessage>(this IHealthChecksBuilder builder)
        {
            string queue = typeof(TMessage).GetQueueEntityPath();
            string registrationName = BuildRegistrationName(QueueHealthCheckPrefix, queue);
            return builder.AddServiceBusResourceHealthCheck(registrationName,
                client => new AzureServiceBusQueueHealthCheck(client, queue));
        }

        public static IHealthChecksBuilder AddServiceBusTopicHealthCheck<TMessage>(this IHealthChecksBuilder builder)
        {
            string topic = typeof(TMessage).GetTopicEntityPath();
            string registrationName = BuildRegistrationName(TopicHealthCheckPrefix, topic);
            return builder.AddServiceBusResourceHealthCheck(registrationName,
                client => new AzureServiceBusTopicHealthCheck(client, topic));
        }

        public static IHealthChecksBuilder AddServiceBusSubscriptionHealthCheck<TMessage>(this IHealthChecksBuilder builder, string serviceName)
        {
            string subscription = typeof(TMessage).GetSubscriptionEntityPath(serviceName);
            string registrationName = BuildRegistrationName(SubscriptionHealthCheckPrefix, subscription);
            return builder.AddServiceBusResourceHealthCheck(registrationName,
                client => new AzureServiceBusSubscriptionHealthCheck(client, typeof(TMessage).GetTopicEntityPath(), subscription));
        }

        private static IHealthChecksBuilder AddServiceBusResourceHealthCheck(this IHealthChecksBuilder builder,
            string registrationName, Func<ManagementClient, AzureServiceBusHealthCheck> healthCheckProvider)
        {
            builder.Services.TryAddServiceBusManagementClient();
            return builder.Add(new HealthCheckRegistration(registrationName, provider =>
            {
                var client = provider.GetRequiredService<ManagementClient>();
                return healthCheckProvider.Invoke(client);
            }, null, null, Timeout));
        }

        private static string BuildRegistrationName(string resourceType, string resourcePath) =>
            $"{ServiceBusHealthCheckPrefix}-{resourceType}-{resourcePath}";
    }
}
