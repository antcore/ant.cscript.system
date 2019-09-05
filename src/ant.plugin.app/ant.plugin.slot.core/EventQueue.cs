using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ant.plugin.slot
{
    internal sealed class EventQueue
    {
        public event EventHandler<EventProcessedEventArgs> EventPushed;

        public EventQueue() { }

        public void Push(IEvent @event)
        {
            OnMessagePushed(new EventProcessedEventArgs(@event));
        }

        private void OnMessagePushed(EventProcessedEventArgs e) => this.EventPushed?.Invoke(this, e);
    }
}
