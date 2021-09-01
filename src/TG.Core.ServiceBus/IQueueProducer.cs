namespace TG.Core.ServiceBus
{
    public interface IQueueProducer<in TMessage> : IServiceBusProducer<TMessage>
    {
    }
}
