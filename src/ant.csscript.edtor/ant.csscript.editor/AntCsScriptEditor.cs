using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
using ICSharpCode.TextEditor.Document;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace ant.csscript.editor
{
    public partial class AntCsScriptEditor : UserControl
    {
        public CsScriptEditor mainForm;

        public AntCsScriptEditor()
        {
            InitializeComponent();
        }

        public void InitEditor()
        {
            if (pcRegistry == null)
                InitScriptCode(string.Empty);

            mainForm.pcRegistry = pcRegistry;
            mainForm.myProjectContent = myProjectContent;
            mainForm.parseInformation = parseInformation;
            mainForm.lastCompilationUnit = lastCompilationUnit;

            parserThread = new Thread(ParserThread);
            parserThread.IsBackground = true;
            parserThread.Start();
        }

        public ProjectContentRegistry pcRegistry;
        public DefaultProjectContent myProjectContent;
        public ParseInformation parseInformation = new ParseInformation();
        public ICompilationUnit lastCompilationUnit;

        private Thread parserThread;
        public const string DummyFileName = "edited.cs";
        private static readonly LanguageProperties CurrentLanguageProperties = LanguageProperties.CSharp;

        public List<string> ReferencedAssembliesSystemDefine = new List<string> {
            "System",
            "System.Data",
            "System.Data",
            "System.Drawing",
            "System.Xml",
        };

        public string ReferencedAssembliesMyDefinePath;

        private void ParserThread()
        {
            myProjectContent.AddReferencedContent(pcRegistry.Mscorlib);
             
            ParseStep();
            foreach (string assemblyName in ReferencedAssembliesSystemDefine)
            {
                string assemblyNameCopy = assemblyName; // copy for anonymous method
                IProjectContent referenceProjectContent = pcRegistry.GetProjectContentForReference(assemblyName, assemblyName);
                myProjectContent.AddReferencedContent(referenceProjectContent);
                if (referenceProjectContent is ReflectionProjectContent)
                {
                    (referenceProjectContent as ReflectionProjectContent).InitializeReferences();
                }
            }
            if (!string.IsNullOrEmpty(ReferencedAssembliesMyDefinePath) && Directory.Exists(ReferencedAssembliesMyDefinePath))
            {
                var files = Directory.GetFiles(ReferencedAssembliesMyDefinePath);
                foreach (var assemblyName in files)
                {
                    var referenceProjectContent = pcRegistry.GetProjectContentForReference(assemblyName, assemblyName);
                    myProjectContent.AddReferencedContent(referenceProjectContent);
                    if (referenceProjectContent is ReflectionProjectContent)
                        (referenceProjectContent as ReflectionProjectContent).InitializeReferences();
                }
            }
            while (!IsDisposed)
            {
                ParseStep();
                Thread.Sleep(2000);
            }
        }

        public string TextCode
        {
            get
            {
                return textEditorControl1.Text;
            }
        }

        private void ParseStep()
        {
            string code = null;
            try
            {
                Invoke(new MethodInvoker(delegate
                {
                    code = textEditorControl1.Text;
                }));
            }
            catch { return; }

            TextReader textReader = new StringReader(code);
            ICompilationUnit newCompilationUnit;
            SupportedLanguage supportedLanguage;

            supportedLanguage = SupportedLanguage.CSharp;
            using (ICSharpCode.NRefactory.IParser p = ParserFactory.CreateParser(supportedLanguage, textReader))
            {
                // we only need to parse types and method definitions, no method bodies
                // so speed up the parser and make it more resistent to syntax
                // errors in methods
                p.ParseMethodBodies = false;

                p.Parse();
                newCompilationUnit = ConvertCompilationUnit(p.CompilationUnit);
            }
            // Remove information from lastCompilationUnit and add information from newCompilationUnit.
            myProjectContent.UpdateCompilationUnit(lastCompilationUnit, newCompilationUnit, DummyFileName);
            lastCompilationUnit = newCompilationUnit;
            parseInformation.SetCompilationUnit(newCompilationUnit);
        }

        private ICompilationUnit ConvertCompilationUnit(CompilationUnit cu)
        {
            NRefactoryASTConvertVisitor converter;
            converter = new NRefactoryASTConvertVisitor(myProjectContent);
            cu.AcceptVisitor(converter, null);
            return converter.Cu;
        }

        public void formatCode()
        {
            ICSharpCode.TextEditor.Actions.IEditAction EditAction = new ICSharpCode.TextEditor.Actions.IndentSelection();
            EditAction.Execute(textEditorControl1.ActiveTextAreaControl.TextArea);
        }

        /// <summary>
        /// 按键检测
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AntCsScriptEditor_KeyDown(object sender, KeyEventArgs e)
        {
        }

        public void InitScriptCode(string ScriptCode)
        {
            textEditorControl1.Text = ScriptCode;

            textEditorControl1.SetHighlighting("C#");

            textEditorControl1.Document.FormattingStrategy = new CSharpBinding.FormattingStrategy.CSharpFormattingStrategy();//格式化策略
            textEditorControl1.IndentStyle = IndentStyle.Smart;

            textEditorControl1.ShowEOLMarkers = false;
            textEditorControl1.ShowInvalidLines = false;
            HostCallbackImplementation.Register(mainForm);
            CodeCompletionKeyHandler.Attach(mainForm, textEditorControl1);
            ToolTipProvider.Attach(mainForm, textEditorControl1);

            pcRegistry = new ProjectContentRegistry(); // Default .NET 2.0 registry

            // Persistence lets SharpDevelop.Dom create a cache file on disk so that
            // future starts are faster.
            // It also caches XML documentation files in an on-disk hash table, thus
            // reducing memory usage.
            pcRegistry.ActivatePersistence(Path.Combine(Path.GetTempPath(), "CSharpCodeCompletion"));

            myProjectContent = new DefaultProjectContent();
            myProjectContent.Language = CurrentLanguageProperties;
        }
    }
}