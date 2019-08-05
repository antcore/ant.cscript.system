using System;
using System.Reflection;

namespace ant.csscript.handle.domain.core
{
    public class CsScirptCompilerInfo
    {
        public Guid CsScirptGuid { get; set; }
        public string CsScirptCode { get; set; }
        public Assembly CsAssembly { get; set; }
    }
}