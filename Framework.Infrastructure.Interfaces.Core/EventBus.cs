using System.Threading.Tasks;
using Framework.Infrastructure.Interfaces.Core.Event;
using Framework.Infrastructure.Ioc.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Framework.Infrastructure.Interfaces.Core
{
    public class EventBus:IEventBus
    {
        public async Task Publish<T>(T @event) where T : IEvent
        {
            var eventHandler = ServiceLocator.Instance.GetService<IEventHandler<T>>();
            await eventHandler.Handle(@event);
        }
    }
}
