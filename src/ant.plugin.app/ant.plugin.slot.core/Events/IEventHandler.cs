using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ant.plugin.slot
{

    public interface IEventHandler
    {
        Task<bool> HandleAsync(IEvent @event, CancellationToken cancellationToken = default);
        bool Handle(IEvent @event);

        bool CanHandle(IEvent @event);
    }

    public interface IEventHandler<in T> : IEventHandler
        where T : IEvent
    {
        Task<bool> HandleAsync(T @event, CancellationToken cancellationToken = default);
        bool Handle(T @event);
    }


}
