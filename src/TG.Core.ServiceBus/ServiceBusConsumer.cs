using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TG.Core.ServiceBus.Options;

namespace TG.Core.ServiceBus
{
    public class ServiceBusConsumer<TMessage> : BackgroundService
    {
        private readonly IReceiverClient _receiverClient;
        private readonly IServiceScopeFactory _serviceProvider;
        private readonly ILogger<ServiceBusConsumer<TMessage>> _logger;
        private readonly SbTracingOptions _tracingOptions;

        public ServiceBusConsumer(IReceiverClient receiverClient, IServiceScopeFactory serviceScopeFactory,
            ILogger<ServiceBusConsumer<TMessage>> logger, SbTracingOptions tracingOptions)
        {
            _receiverClient = receiverClient;
            _serviceProvider = serviceScopeFactory;
            _logger = logger;
            _tracingOptions = tracingOptions;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _receiverClient.RegisterMessageHandler(HandleMessage, new MessageHandlerOptions(HandleReceivedException)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            });
            return Task.CompletedTask;
        }

        private async Task HandleMessage(Message message, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Received message of type {Type}, body: {Body}", typeof(TMessage).Name, message.Body);
            var body = JsonSerializer.Deserialize<TMessage>(message.Body);

            using (var scope = _serviceProvider.CreateScope())
            {
                SetTraceId(message.SessionId);
                var handler = scope.ServiceProvider.GetService<IMessageHandler<TMessage>>();
                if (handler is null)
                {
                    throw new ApplicationException($"Handler for message of type {typeof(TMessage).Name} is not registered");
                }

                await handler.HandleMessage(body, cancellationToken);
            }

            await _receiverClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        private Task HandleReceivedException(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;

            _logger.LogError(
                exceptionReceivedEventArgs.Exception,
                "Message handler encountered an exception. Endpoint: {endpoint}, entity path: {entityPath}, action: {action}",
                context.Endpoint, context.EntityPath, context.Action);
            return Task.CompletedTask;
        }

        private void SetTraceId(string? traceId)
        {
            if (traceId is null)
            {
                traceId = Guid.NewGuid().ToString();
                _logger.LogWarning(
                    "Incoming Service bus message doesn't have Session Id - generating a new one. Message type: {Name}. New traceID: {traceId}",
                    typeof(TMessage).Name, traceId);
            }

            _tracingOptions.SetTraceId?.Invoke(traceId);
        }
    }
}
