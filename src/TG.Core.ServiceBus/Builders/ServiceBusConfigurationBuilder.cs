using System;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Primitives;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TG.Core.ServiceBus.Extensions;
using TG.Core.ServiceBus.Options;

namespace TG.Core.ServiceBus.Builders
{
    public class ServiceBusConfigurationBuilder
    {
        private readonly IServiceCollection _services;

        public ServiceBusConfigurationBuilder(IServiceCollection services)
        {
            _services = services;
        }
        
        private static readonly ManagedIdentityTokenProvider ManagedIdentityTokenProvider = new();
        /// <summary>
        /// Registers IQueueProducer&lt;TMessage&gt; and all required services for producing messages, and the HealthCheck. 
        /// </summary>
        public ServiceBusConfigurationBuilder AddQueueProducer<TMessage>()
        {
            _services.AddHealthChecks().AddServiceBusQueueHealthCheck<TMessage>();

            _services.AddSingleton<IQueueProducer<TMessage>, ServiceBusProducer<TMessage>>(provider =>
                new ServiceBusProducer<TMessage>(BuildQueueClient<TMessage>(provider), provider.GetRequiredService<IOptions<SbTracingOptions>>().Value));
            return this;
        }

        /// <summary>
        /// Registers BackgroundService and all required _services for consuming messages, and the HealthCheck.<para></para>
        /// Use IMessageHandler&lt;TMessage&gt; for processing incoming messages in your service.
        /// </summary>
        public ServiceBusConfigurationBuilder AddQueueConsumer<TMessage, THandler>()
            where THandler : class, IMessageHandler<TMessage>
        {
            _services.AddHostedService(provider =>
                new ServiceBusConsumer<TMessage>(BuildQueueClient<TMessage>(provider), provider.GetRequiredService<IServiceScopeFactory>(),
                    provider.GetRequiredService<ILogger<ServiceBusConsumer<TMessage>>>(), provider.GetRequiredService<IOptions<SbTracingOptions>>().Value));

            _services.AddHealthChecks().AddServiceBusQueueHealthCheck<TMessage>();

            _services.AddScoped<IMessageHandler<TMessage>, THandler>();
            return this;
        }

        /// <summary>
        /// Registers ITopicProducer&lt;TMessage&gt; and all required _services for producing messages, and the HealthCheck. 
        /// </summary>
        public ServiceBusConfigurationBuilder AddTopicProducer<TMessage>()
        {
            _services.AddHealthChecks().AddServiceBusTopicHealthCheck<TMessage>();

            _services.AddSingleton<ITopicProducer<TMessage>, ServiceBusProducer<TMessage>>(provider =>
                new ServiceBusProducer<TMessage>(BuildTopicClient<TMessage>(provider), provider.GetRequiredService<IOptions<SbTracingOptions>>().Value));
            return this;
        }

        /// <summary>
        /// Registers BackgroundService and all required _services for consuming messages, and the HealthCheck.<para></para>
        /// Use IMessageHandler&lt;TMessage&gt; for processing incoming messages in your service.
        /// </summary>
        public ServiceBusConfigurationBuilder AddSubscriptionConsumer<TMessage, THandler>(string subscriberName)
            where THandler : class, IMessageHandler<TMessage>
        {
            _services.AddHostedService(provider =>
                new ServiceBusConsumer<TMessage>(BuildSubscriptionClient<TMessage>(provider, subscriberName), provider.GetRequiredService<IServiceScopeFactory>(),
                    provider.GetRequiredService<ILogger<ServiceBusConsumer<TMessage>>>(), provider.GetRequiredService<IOptions<SbTracingOptions>>().Value));

            _services.AddHealthChecks().AddServiceBusSubscriptionHealthCheck<TMessage>(subscriberName);

            _services.AddScoped<IMessageHandler<TMessage>, THandler>();
            return this;
        }

        public ServiceBusConfigurationBuilder ConfigureTracing(Func<string?> getTraceIdSetup, Action<string> setTraceIdSetup)
        {
            _services.Configure<SbTracingOptions>(opt =>
            {
                opt.GetTraceId = getTraceIdSetup;
                opt.SetTraceId = setTraceIdSetup;
            });
            return this;
        }
        
        private static IQueueClient BuildQueueClient<TMessage>(IServiceProvider provider) =>
            new QueueClient(provider.GetRequiredService<IOptions<ServiceBusOptions>>().Value.Endpoint,
                typeof(TMessage).GetQueueEntityPath(), ManagedIdentityTokenProvider);

        private static ITopicClient BuildTopicClient<TMessage>(IServiceProvider provider) =>
            new TopicClient(provider.GetRequiredService<IOptions<ServiceBusOptions>>().Value.Endpoint,
                typeof(TMessage).GetTopicEntityPath(), ManagedIdentityTokenProvider);

        private static ISubscriptionClient BuildSubscriptionClient<TMessage>(IServiceProvider provider, string serviceName) =>
            new SubscriptionClient(provider.GetRequiredService<IOptions<ServiceBusOptions>>().Value.Endpoint,
                typeof(TMessage).GetTopicEntityPath(),
                typeof(TMessage).GetSubscriptionEntityPath(serviceName),
                ManagedIdentityTokenProvider);
    }
}