using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using TG.Core.ServiceBus.Options;

namespace TG.Core.ServiceBus
{
    public class ServiceBusProducer<TMessage> : IQueueProducer<TMessage>, ITopicProducer<TMessage>
    {
        private readonly ISenderClient _senderClient;
        private readonly SbTracingOptions _tracingOptions;

        public ServiceBusProducer(ISenderClient senderClient, SbTracingOptions tracingOptions)
        {
            _senderClient = senderClient;
            _tracingOptions = tracingOptions;
        }

        public Task SendMessageAsync(TMessage message)
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(message);
            var serviceBusMessage = new Message(bytes)
            {
                ContentType = "application/json",
                SessionId = _tracingOptions.GetTraceId?.Invoke()
            };
            return _senderClient.SendAsync(serviceBusMessage);
        }
    }
}
