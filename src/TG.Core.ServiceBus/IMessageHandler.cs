using System.Threading;
using System.Threading.Tasks;

namespace TG.Core.ServiceBus
{
    public interface IMessageHandler<in TMessage>
    {
        Task HandleMessage(TMessage message, CancellationToken cancellationToken);
    }
}