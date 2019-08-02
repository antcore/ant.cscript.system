using ICSharpCode.CodeCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ant.csscript.editor.avalonedit
{
    /// <summary>
    /// This is a simple script provider that adds a few using statements to the C# scripts (.csx files)
    /// </summary>
    class ScriptProvider : ICSharpScriptProvider
    {
        // 需要智能提示的命名空间
        public string GetUsing()
        {
            //    return "" +
            //        "using System; " +
            //        "using System.Collections.Generic; " +
            //        "using System.Linq; " +
            //        "using System.Xml; " +
            //        "using ant.csscript.core; " +
            //        "using System.Text; ";

            return @"
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Xml;
using System.Xml.Linq;
using System.Drawing;

using ant.csscript.core;

";
        }


        public string GetVars()
        {
            return "int age = 25;";
        }

        public string GetNamespace() => null;
    }
}
