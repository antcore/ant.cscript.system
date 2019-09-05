using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ant.plugin.slot
{
    public interface IEvent
    {
        Guid Id { get; }
        DateTime Timestamp { get; }
    }
     

}
