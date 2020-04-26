using System.Threading.Tasks;

namespace Framework.Infrastructure.Interfaces.Core.Event
{
    public interface IEventBus
    {
        Task Publish<T>(T @event) where T : IEvent;
    }
}
