using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ant.plugin.slot
{
    interface IPluginFunc
    {
        void Init();
        void Start();
        void Stop();
        void Destroy();

    }
}
