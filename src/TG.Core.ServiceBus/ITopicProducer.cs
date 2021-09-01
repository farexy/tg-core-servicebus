namespace TG.Core.ServiceBus
{
    public interface ITopicProducer<in TMessage> : IServiceBusProducer<TMessage>
    {
    }
}
