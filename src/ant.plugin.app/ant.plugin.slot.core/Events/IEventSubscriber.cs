﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ant.plugin.slot
{
    public interface IEventSubscriber : IDisposable
    {
        void Subscribe();
    }
 
}
