using System;

namespace ant.plugin.slot
{
    public class EventProcessedEventArgs : EventArgs
    {
        public EventProcessedEventArgs(IEvent @event)
        {
            this.Event = @event;
        }

        public IEvent Event { get; }

    }
}