using ant.csscript.handle.domain.core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ant.csscript.run
{
    public class CsScriptRun
    {
        public CsScriptRun()
        {
            RunSwitch = true;

        }
        private string GetTestCodeFilePath
        {
            get
            {
                return string.Format(@"{0}InitCsScript.txt", AppDomain.CurrentDomain.BaseDirectory);
            }
        }

        private bool RunSwitch = false;

        public void Start()
        {
            while (RunSwitch)
            {
                if (StaticInfo.ScriptInfoCompilerResults != null)
                    Console.WriteLine("当前脚本数量  ：  " + StaticInfo.ScriptInfoCompilerResults.Count);

                //获取脚本信息
                Guid _CsScriptGuid = Guid.NewGuid();
                string _CsScriptCode = System.IO.File.ReadAllText(GetTestCodeFilePath, Encoding.Default);
                string _PathFile = GetTestCodeFilePath;
                if (_CsScriptCode.Length > 10)
                    RunScript(_CsScriptGuid, _CsScriptCode, _PathFile);
                

                System.Threading.Thread.Sleep(1000);
            }

        }
        public void Stop()
        {
            RunSwitch = false;
        }

        string FilePathHandle = string.Empty;
        string FilePathSource = string.Empty;

        Guid CsScriptGuid = Guid.Empty;
        string CsScriptCode = string.Empty;
        string CompilerMessage = string.Empty;
        string RunMessage = string.Empty;
        string RunLogMessage = string.Empty;
        CsScriptHandleFile HandleFileCsScript = null;
        public void RunScript(Guid CsScriptGuid, string CsScriptCode, string filePath)
        {
            //初始化 脚本 编译 及 运行 
            if (null == HandleFileCsScript)
                HandleFileCsScript = new CsScriptHandleFile();

            //编译脚本
            CompilerMessage = string.Empty;
            var resultCompiler = HandleFileCsScript.CompilerFromCsCode(CsScriptGuid, CsScriptCode, ref CompilerMessage);
            if (!resultCompiler)
            {
                Console.WriteLine(CompilerMessage);
                return;
            }
            Console.WriteLine(!string.IsNullOrEmpty(CompilerMessage) ? CompilerMessage : "已完成编译");
             
            //执行脚本
            FilePathHandle = FilePathSource = filePath;

            RunMessage = string.Empty;
            RunLogMessage = string.Empty;
            var result = HandleFileCsScript.RunFromCsCodeCompilerHandleFile(CsScriptGuid, FilePathHandle, FilePathSource, ref RunMessage,ref RunLogMessage);
            if (null == result)
            {
                Console.WriteLine(RunMessage);
                return;
            }
            var sbInfo = new StringBuilder();
            sbInfo.AppendLine(" --------------------------------------- ");
            sbInfo.AppendLine(" ********** 【脚本已成功运行】 ********** ");
            sbInfo.AppendLine("");
            if (!string.IsNullOrEmpty(result.FileText))
            {
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

            if (RunLogMessage.Length>0)
            {
                sbInfo.AppendLine(" --------------------------------------- ");
                sbInfo.AppendLine(" *********** 【脚本运行日志】 *********** ");
                sbInfo.AppendLine(" --------------------------------------- ");
                sbInfo.AppendLine(RunLogMessage);
            }

            Console.WriteLine(sbInfo.ToString());
        }


    }
}
