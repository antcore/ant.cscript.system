using ant.csscript.core.domain;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.CodeCompletion;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ant.csscript.editor.avalonedit
{
    public partial class Mainform : Form
    {
        public string GetRootPath
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }
        public string GetPathPluginsDynamicCodeDll
        {
            get
            {
                string dirPath = string.Format(@"{0}Plugins\DynamicCodeDll", GetRootPath);
                if (!System.IO.Directory.Exists(dirPath))
                    System.IO.Directory.CreateDirectory(dirPath);
                return dirPath;
            }
        }
        public string GetPathPluginsDynamicCodeDependencyDll
        {
            get
            {
                string dirPath = string.Format(@"{0}Plugins\DynamicCodeDllDependencyDll", GetRootPath);
                if (!System.IO.Directory.Exists(dirPath))
                    System.IO.Directory.CreateDirectory(dirPath);
                return dirPath;
            }
        }
        public string GetPathTempCsFile
        {
            get
            {
                string dirPath = string.Format(@"{0}Temp\CsFile", GetRootPath);
                if (!System.IO.Directory.Exists(dirPath))
                    System.IO.Directory.CreateDirectory(dirPath);
                return dirPath;
            }
        }

        public Mainform()
        {
            InitializeComponent();
        }

        #region CsEditer

        private CodeTextEditor editor;
        //private ICSharpCode.CodeCompletion.CSharpCompletion completion;
        #region 初始化编辑器

        private void InitCsEditer()
        {
            editor = new CodeTextEditor();
            editor.FontFamily = new System.Windows.Media.FontFamily("Console");
            editor.FontSize = 12;

            numericUpDown1.Value = 16;


            editor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("C#");

            //将editor作为elemetnHost的组件
            elementHost1.Child = editor;
            //在编辑器中加载 CodeEditorView.cs

            //智能提示
            //completion = new CSharpCompletion(new ScriptProvider());
            //editor.Completion = new CSharpCompletion(new ScriptProvider());
            editor.Completion = new CSharpCompletion(new ScriptProvider(), ScriptProviderAssembly);

            //editor 代码折叠
            InitFoldStrategy(editor);
            editor.TextChanged += Editor_TextChanged;

            lblHandleFile.Text = string.Empty;
        }
        // Editer 智能提示所需要的类库
        private IReadOnlyList<Assembly> ScriptProviderAssembly
        {
            get
            {
                return new List<Assembly>
                {
                    typeof(object).Assembly, // mscorlib
                    typeof(Uri).Assembly, // System.dll
                    typeof(System.Xml.XmlDocument).Assembly, // System.Xml.dll
                    typeof(System.Xml.Linq.XAttribute).Assembly, // System.Xml.dll
                    //typeof(Enumerable).Assembly, // System.Core.dll
                    typeof(System.Linq.Enumerable).Assembly, // System.Core.dll
                    typeof(System.Data.DataRow).Assembly, // System.Drawing.dll
                    typeof(System.Drawing.Image).Assembly, // System.Drawing.dll
                    typeof(ant.csscript.core.domain.ICsScriptHandleFile).Assembly, // System.Drawing.dll
                    //typeof(Form).Assembly, // System.Windows.Forms.dll
                };
            }
        }


        #endregion

        #region 代码折叠
        private void Editor_TextChanged(object sender, EventArgs e)
        {
            RefreshFoldStrategy(editor);
        }
        private void InitFoldStrategy(CodeTextEditor editor)
        {
            editor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.CSharp.CSharpIndentationStrategy(editor.Options);
            foldingStrategy = new BraceFoldingStrategy();
        }
        FoldingManager foldingManager = null;
        BraceFoldingStrategy foldingStrategy = null;
        void RefreshFoldStrategy(CodeTextEditor editor)
        {
            if (foldingStrategy != null)
            {
                if (foldingManager == null)
                    foldingManager = FoldingManager.Install(editor.TextArea);
                ((BraceFoldingStrategy)foldingStrategy).UpdateFoldings(foldingManager, editor.Document);
            }
            else
            {
                if (foldingManager != null)
                {
                    FoldingManager.Uninstall(foldingManager);
                    foldingManager = null;
                }
            }
        }
        #endregion


        #endregion

        private string editorScriptTempFilePath;
        private void Mainform_Load(object sender, EventArgs e)
        {
            richTextBoxMessage.Multiline = true;  //将Multiline属性设为true，实现显示多行
            richTextBoxMessage.ScrollBars = RichTextBoxScrollBars.Vertical;  //设置ScrollBars属性实现只显示垂直滚动条
            richTextBoxMessage.SelectionFont = new Font("楷体", 12, FontStyle.Bold);  //设置SelectionFont属性实现控件中的文本为楷体，大小为12，字样是粗体

            richTextBoxFileText.Multiline = true;  //将Multiline属性设为true，实现显示多行
            richTextBoxFileText.ScrollBars = RichTextBoxScrollBars.Vertical;  //设置ScrollBars属性实现只显示垂直滚动条
            richTextBoxFileText.SelectionFont = new Font("楷体", 12, FontStyle.Bold);  //设置SelectionFont属性实现控件中的文本为楷体，大小为12，字样是粗体


            splitContainer2.FixedPanel = FixedPanel.Panel2;

            InitCsEditer();

            editorScriptTempFilePath = InitCsCodePath(getCode());
            ////editor.OpenFile("../../Program.cs");
            //只有以打开文件的方式才能有智能提示
            editor.OpenFile(editorScriptTempFilePath);

            //editor.Load("../../CodeEditorView.cs"); 
        }
        private string InitCsCodePath(string csCode)
        {
            string csFilePath = string.Format(@"{0}\{1}.cs", GetPathTempCsFile, Guid.NewGuid());
            System.IO.File.WriteAllText(csFilePath, csCode, Encoding.Default);
            return csFilePath;
        }


        private string getCode()
        {
            string csCode = @"
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Xml;
using System.Xml.Linq;
using System.Drawing;

using ant.csscript.core.domain;

namespace CsScript
{ 
    public class CsScriptHandle[CLASS_NAME] : ICsScriptHandleFile
    {
        public void Run()
        {

            FileData = Txt_FileHandle.Instance.LoadFile(FileInfo.FilePathHandle);

            DataHandleResult = new ScriptResults<SampleData>()
            {
                Data = new List<SampleData>()
                {
                    new SampleData
                    {
                          ItemName = ""1"",
                          PramaName = ""2"",
                          PramaValue = ""3"",
                          SampleNo = ""4""
                     }
                }
            };


            //运行输出
            RunLog.AppendLine("" ********运行结束 ******** "");
        }
        public StringBuilder RunLog { get; set; }
        #region PROPS
        /// <summary>
        /// 需要处理文件信息
        /// </summary>
        public FileInfoWaitHandle FileInfo { get; set; }
        /// <summary>
        /// 文件内容信息
        /// </summary>
        public FileDataHandleResult FileData { get; set; }
        public ScriptResults<SampleData> DataHandleResult { get; set; }
        #endregion

        public void Init(FileInfoWaitHandle fileInfo)
        {
            FileInfo = fileInfo;
            DataHandleResult = new ScriptResults<SampleData>();
            DataHandleResult.Data = new List<SampleData>();
            RunLog = new StringBuilder();
        }
    }
}";
            return csCode.Replace("[CLASS_NAME]", Guid.NewGuid().ToString("N"));
        }
        private void showMessage(string message)
        {
            showMessage(message, Color.Black);
        }
        private void showMessage(string message, Color color)
        {
            richTextBoxMessage.Clear();
            richTextBoxMessage.Text = message;
            richTextBoxMessage.SelectionColor = color;    //设置SelectionColor属性实现控件中的文本颜色为红色
            richTextBoxMessage.ScrollToCaret();
        }



        CompilerResults compilerResults = null;
        /// <summary>
        /// 动态编译并执行代码
        /// </summary>
        /// <param name="OutPutDllPath">输出Dll的路径</param>
        /// <param name="DependencyDllPath">自定义依赖Dll路径</param>
        /// <param name="CsCode">代码</param>
        /// <returns>返回输出内容</returns>
        private CompilerResults CompilerFromCsCode(string OutPutDllPath, string DependencyDllPath, string CsCode)
        {
            CompilerResults result = null;
            try
            {

                CSharpCodeProvider complier = new CSharpCodeProvider();
                //设置编译参数
                CompilerParameters paras = new CompilerParameters();
                //引入第三方dll
                paras.ReferencedAssemblies.Add("System.dll");
                paras.ReferencedAssemblies.Add("System.Xml.dll");
                paras.ReferencedAssemblies.Add("System.Xml.Linq.dll");
                paras.ReferencedAssemblies.Add("System.linq.dll");
                paras.ReferencedAssemblies.Add("System.Data.dll");
                paras.ReferencedAssemblies.Add("System.Drawing.dll");

                if (!string.IsNullOrEmpty(DependencyDllPath) && System.IO.Directory.Exists(DependencyDllPath))
                {
                    var directoryInfo = new System.IO.DirectoryInfo(DependencyDllPath);
                    var files = directoryInfo.GetFiles("*.dll");
                    foreach (var item in files)
                        paras.ReferencedAssemblies.Add(item.FullName);
                }

                //是否内存中生成输出
                paras.GenerateInMemory = false;
                //是否生成可执行文件
                paras.GenerateExecutable = false;

                paras.OutputAssembly = OutPutDllPath;
                //paras.OutputAssembly = binpath + dllName + ".dll";
                //编译代码
                result = complier.CompileAssemblyFromSource(paras, CsCode);

                if (result.Errors.HasErrors)                            //如果有错误
                {
                    StringBuilder sbError = new StringBuilder();            //创建错误信息字符串
                    sbError.Append("编译有错误的表达式: ");                //添加错误文本
                    foreach (CompilerError err in result.Errors)            //遍历每一个出现的编译错误
                    {
                        sbError.AppendLine($"{err.ErrorText}");        //添加进错误文本，每个错误后换行
                    }
                    showMessage(sbError.ToString(), Color.Red);
                    //throw new Exception("编译错误: " + error.ToString());//抛出异常
                    //nError = "编译错误: " + error.ToString();
                    //return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return result;
        }
        private string filePathSelect = string.Empty;
        private void BtnSelectFile_Click(object sender, EventArgs e)
        {
            var fileDialog = new OpenFileDialog();
            fileDialog.InitialDirectory = "D:\\";    //打开对话框后的初始目录
            fileDialog.Filter = "所有文件|*.*";
            fileDialog.RestoreDirectory = false;    //若为false，则打开对话框后为上次的目录。若为true，则为初始目录
            if (fileDialog.ShowDialog() == DialogResult.OK)
                filePathSelect = System.IO.Path.GetFullPath(fileDialog.FileName);
            //
            if (!string.IsNullOrEmpty(filePathSelect))
                lblHandleFile.Text = filePathSelect;
        }
        private void BtnCompiler_Click(object sender, EventArgs e)
        {
            string OutPutDllName = string.Format(@"{0}.dll", Guid.NewGuid());
            string OutPutDllPath = string.Format(@"{0}\{1}", GetPathPluginsDynamicCodeDll, OutPutDllName);
            string DependencyDllPath = GetPathPluginsDynamicCodeDependencyDll;
            string CsCode = editor.Text;
            compilerResults = CompilerFromCsCode(OutPutDllPath, DependencyDllPath, CsCode);
        }

        private void BtnRun_Click(object sender, EventArgs e)
        {
            Assembly objAssembly = compilerResults.CompiledAssembly;
            //object objHelloWorld = objAssembly.CreateInstance("scirptcode.Test");
            //MethodInfo objMI = objHelloWorld.GetType().GetMethod("Run");
            //Console.WriteLine(objMI.Invoke(objHelloWorld, null));
            ICsScriptHandleFile iCsScript = null;
            Type[] types = objAssembly.GetTypes();
            foreach (var t in types)
                if (t.GetInterface("ICsScriptHandleFile") != null)
                    iCsScript = (ICsScriptHandleFile)Activator.CreateInstance(t);
            if (iCsScript != null)
            {
                try
                {
                    //初始化脚本参数
                    var fileInfo = new FileInfoWaitHandle()
                    {
                        FilePathHandle = filePathSelect,
                        FilePathSource = filePathSelect
                    };
                    iCsScript.Init(fileInfo);

                    iCsScript.Run();

                    showMessage(iCsScript.RunLog.ToString());

                    richTextBoxFileText.Clear();
                    richTextBoxFileText.Text = iCsScript.ResultData.FileText;
                    richTextBoxFileText.ScrollToCaret();

                    var strObj = iCsScript.ResultData.Data;
                    Console.WriteLine(strObj);
                    
                }
                catch (Exception ex)
                {
                    showMessage(ex.ToString(), Color.Red);
                }
            }
        }

        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            editor.FontSize = (int)numericUpDown1.Value;
        }



        //        private string getCode()
        //        {
        //            return @"
        //using System;
        //using System.Collections.Generic;
        //using System.Linq; 
        //using ant.csscript.core;

        //namespace scirptcode
        //{
        //    public class Test:ICsScriptBase
        //    {
        //        public ScriptResults<Sampledata> RunSample()
        //        {
        //            return new ScriptResults<Sampledata>()
        //            {
        //                Data = new List<Sampledata>()
        //                {
        //                     new Sampledata 
        //                    {
        //                          ItemName = ""1"",
        //                          PramaName = ""2"",
        //                          PramaValue = ""3"",
        //                          SampleNo = ""4""
        //                     }
        //                }
        //            };
        //        }
        //    }
        //}";
        //        }
    }
}
