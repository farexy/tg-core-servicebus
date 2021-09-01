using System.Threading.Tasks;

namespace TG.Core.ServiceBus
{
    public interface IServiceBusProducer<in TMessage>
    {
        Task SendMessageAsync(TMessage message);
    }
}
