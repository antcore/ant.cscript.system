using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace ant.csscript.editor
{
    public partial class CsScriptEditor : Form
    {
        public ProjectContentRegistry pcRegistry;
        public DefaultProjectContent myProjectContent;
        public ParseInformation parseInformation  ;
        public ICompilationUnit lastCompilationUnit;
        public Thread parserThread;

        public const string DummyFileName = "edited.cs";

        private static readonly LanguageProperties CurrentLanguageProperties = LanguageProperties.CSharp;

        public CsScriptEditor()
        {
            InitializeComponent();
        }

        public void InitCsScriptEditor()
        {
            
             
        }
    }
}