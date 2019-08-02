using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;

namespace ant.csscript.core.domain
{

    public class CsScirptCompilerInfo
    {
        public Guid CsScirptGuid { get; set; }
        public string CsScirptCode { get; set; }
        public CompilerResults CompilerResults { get; set; }
    }

    public class StaticInfo
    {
        //public static Dictionary<Guid, CompilerResults> ScriptInfoCompilerResults { get; set; } 
        public static List<CsScirptCompilerInfo> ScriptInfoCompilerResults { get; set; }
    }
}