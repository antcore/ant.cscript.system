using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using Microsoft.CSharp;

using ant.csscript.core.domain;
using ant.csscript.editor;


namespace ant.csscript.editor.handlefiles
{
    public partial class MainForm : CsScriptEditor
    {
        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 设置脚本编辑器 代码
        /// </summary>
        /// <param name="ScriptCode"></param>
        public void SetScirptCode(string ScriptCode = "")
        {
            //初始化脚本编辑及运行类
            if (null == HandleFileCsScript)
                HandleFileCsScript = new CsScriptHandleFile(GetPathPluginsCsScriptDependencyDll, DependencySystemDlls);
            if (string.IsNullOrEmpty(ScriptCode))
                ScriptCode = GetInitCode;
            antCsScriptEditor.InitScriptCode(ScriptCode);
        }
        #region 脚本编辑器相关
        public string GetRootPath
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }
        //脚本编译 和 编辑器 智能感知依赖
        public string GetPathPluginsCsScriptDependencyDll
        {
            get
            {
                string dirPath = string.Format(@"{0}Plugins\CsScriptDependencyDll", GetRootPath);
                if (!System.IO.Directory.Exists(dirPath))
                    System.IO.Directory.CreateDirectory(dirPath);
                return dirPath;
            }
        }
        //脚本编译 依赖
        private List<string> DependencySystemDlls = new List<string>
                {
                    "System.dll",
                    "System.Xml.dll",
                    "System.Xml.Linq.dll",
                    "System.linq.dll",
                    "System.Data.dll",
                    "System.Drawing.dll"
                };
        //编辑器 智能感知依赖
        private List<string> ReferencedAssembliesSystemDefine = new List<string>
                {
                    "System",
                    //"System.Linq",
                    "System.Data",
                    "System.Xml",
                    //"System.Xml.Linq",
                    "System.Drawing",
                };
        void InitAntCsScriptEditor()
        {
            antCsScriptEditor.mainForm = this;
            //编辑器 智能感知
            antCsScriptEditor.ReferencedAssembliesSystemDefine = ReferencedAssembliesSystemDefine;
            //编辑器 智能感知
            antCsScriptEditor.ReferencedAssembliesMyDefinePath = GetPathPluginsCsScriptDependencyDll;
            antCsScriptEditor.InitEditor();
        }

        private string GetInitCode
        {
            get
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

using ant.csscript.core;
using ant.csscript.core.domain;

namespace CsScript
{ 
    public class CsScriptHandleFileTest : ICsScriptHandleFile
    {
        public void Run()
        {
            string txt = Txt_FileHandle.Instance.LoadFile(FileInfo.FilePathHandle);
            ResultData.FileText = txt;
            //ResultData.Data = new ScriptResults<SampleData>()
            //{
            //    Data = new List<SampleData>()
            //    {
            //        new SampleData
            //        {
            //              ItemName = ""1"",
            //              PramaName = ""2"",
            //              PramaValue = ""3"",
            //              SampleNo = ""4""
            //         }
            //    }
            //};
            //运行输出
            RunLog.AppendLine("" * *******运行结束 * *******"");
        }
        public StringBuilder RunLog { get; set; }
        #region PROPS
        /// <summary>
        /// 需要处理文件信息
        /// </summary>
        public FileInfoWaitHandle FileInfo { get; set; } 
        public ScriptResults<SampleData> ResultData { get; set; }
        #endregion
        public void Init(FileInfoWaitHandle fileInfo)
        {
            FileInfo = fileInfo;
            ResultData = new ScriptResults<SampleData>();
            ResultData.Data = new List<SampleData>();
            RunLog = new StringBuilder();
        }
    }
}";
                return csCode.Replace("[CLASS_NAME]", Guid.NewGuid().ToString("N"));
            }
        }

        #endregion
        
        private void MainForm_Load(object sender, EventArgs e)
        {
            splitContainer2.FixedPanel = FixedPanel.Panel2;

            InitAntCsScriptEditor();

            SetScirptCode();
        }
        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == (Keys.I | Keys.Control))
                antCsScriptEditor.formatCode();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            antCsScriptEditor.formatCode();
        }
        private void showMessage(string message)
        {
            showMessage(message, Color.Black);
        }
        private void showMessage(string message, Color color)
        {
            richTextBoxMessage.Clear();
            richTextBoxMessage.SelectionColor = color;
            richTextBoxMessage.AppendText(message); 
            richTextBoxMessage.ScrollToCaret();
        }

        Guid CsScriptGuid = Guid.Empty;
        string CsScriptCode = string.Empty;
        string CompilerMessage = string.Empty;
        string RunMessage = string.Empty;
        string RunLogMessage = string.Empty;
        CsScriptHandleFile HandleFileCsScript = null;
        /// <summary>
        /// 编译脚本
        /// </summary>
        private void BtnCompiler_Click(object sender, EventArgs e)
        {
            CompilerMessage = string.Empty;
            CsScriptCode = antCsScriptEditor.TextCode;
            var result = HandleFileCsScript.CompilerFromCsCode(CsScriptGuid, CsScriptCode, ref CompilerMessage);
            if (!result)
            {
                showMessage(CompilerMessage, Color.Red);
                return;
            }
            showMessage(!string.IsNullOrEmpty(CompilerMessage) ? CompilerMessage : "已完成编译", Color.Black);
        }
        /// <summary>
        /// 运行脚本
        /// </summary>
        private void BtnRun_Click(object sender, EventArgs e)
        {
            string FilePathHandle = string.Empty;
            string FilePathSource = string.Empty;

            FilePathHandle = FilePathSource = filePathSelect;

            RunMessage = string.Empty;
            RunLogMessage = string.Empty;
            var result = HandleFileCsScript.RunFromCsCodeCompilerHandleFile(CsScriptGuid, FilePathHandle, FilePathSource, ref RunMessage, ref RunLogMessage);
            if (null == result)
            {
                showMessage(RunMessage, Color.Red);
                return;
            }
            var sbInfo = new StringBuilder();
            sbInfo.AppendLine(" --------------------------------------- ");
            sbInfo.AppendLine(" ********** 【脚本已成功运行】 ********** ");
            sbInfo.AppendLine("");

            richTextBoxFileText.Clear();
            if (!string.IsNullOrEmpty(result.FileText))
            {
                richTextBoxFileText.Text = result.FileText;
                sbInfo.AppendLine($" 【文件】：存在文本数据 ( {result.FileText.Length} ) ");
            }
            if (null != result.FileAreaData && result.FileAreaData.Count > 0)
            {
                sbInfo.AppendLine($" 【文件】：表格数据 ( {result.FileAreaData.Count} ) ");
            }

            if (null != result.Data && result.Data.Count > 0)
                sbInfo.AppendLine($" 【文件】：解析结果 ( {result.Data.Count} ) ");
            else
                sbInfo.AppendLine($" 【文件】：[**** 解析无数据 ****] ");

            sbInfo.AppendLine("");
            sbInfo.AppendLine(" ********** 【脚本已成功运行】 ********** ");
            sbInfo.AppendLine(" --------------------------------------- ");

            if (RunLogMessage.Length > 0)
            {
                sbInfo.AppendLine(" --------------------------------------- ");
                sbInfo.AppendLine(" *********** 【脚本运行日志】 *********** ");
                sbInfo.AppendLine(" --------------------------------------- ");
                sbInfo.AppendLine(RunLogMessage);
            }

            showMessage(sbInfo.ToString(), Color.Black);
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

            if (!string.IsNullOrEmpty(filePathSelect))
                lblHandleFile.Text = filePathSelect;
        }
        //private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        //{
        //    editor.FontSize = (int)numericUpDown1.Value;
        //}

    }
}
