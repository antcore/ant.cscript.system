using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ant.csscript.core.domain
{
    /// <summary>
    /// 
    /// </summary>
    public class CsScriptHandleFile
    {
        /// <summary>
        /// 脚本依赖 三方库 [三方库放入指定路径]
        /// </summary>
        private string DependencyDllPath { get; set; }
        /// <summary>
        /// 脚本依赖 系统库
        /// </summary>
        private List<string> DependencySystemDlls { get; set; }


        public string GetRootPath
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }
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

        public CsScriptHandleFile(string _DependencyDllPath = "", List<string> _DependencySystemDlls = null)
        {
            DependencyDllPath = string.IsNullOrEmpty(_DependencyDllPath) ? GetPathPluginsCsScriptDependencyDll : _DependencyDllPath;
            if (null == _DependencySystemDlls)
                DependencySystemDlls = new List<string>
                {
                    "System.dll",
                    "System.Xml.dll",
                    "System.Xml.Linq.dll",
                    "System.linq.dll",
                    "System.Data.dll",
                    "System.Drawing.dll"
                };
            else
                DependencySystemDlls = _DependencySystemDlls;
        }
        /// <summary>
        /// 动态编译并执行代码
        /// </summary>
        /// <param name="OutPutDllPath">输出Dll的路径</param>
        /// <param name="DependencyDllPath">自定义依赖Dll路径</param>
        /// <param name="CsCode">代码</param>
        /// <returns>返回输出内容</returns>
        public bool CompilerFromCsCode(Guid CsScriptGuid, string CsScriptCode, ref string CompilerMessage)
        {
            //判断是否 同一脚本 相同代码 防止重复编译
            if (null != StaticInfo.ScriptInfoCompilerResults && StaticInfo.ScriptInfoCompilerResults.Count > 0)
                if (StaticInfo.ScriptInfoCompilerResults.Any(o => o.CsScirptGuid == CsScriptGuid && o.CsScirptCode == CsScriptCode))
                {
                    CompilerMessage = "代码未更改，使用上一次编译结果";
                    return true;
                }

            bool CompilerFalg = false;
            StringBuilder sbError = new StringBuilder();
            try
            {
                CSharpCodeProvider complier = new CSharpCodeProvider();
                //设置编译参数
                CompilerParameters paras = new CompilerParameters();
                //引入第三方dll
                if (null != DependencySystemDlls)
                    foreach (var item in DependencySystemDlls)
                        paras.ReferencedAssemblies.Add(item);
                //paras.ReferencedAssemblies.Add("System.dll");
                //paras.ReferencedAssemblies.Add("System.Xml.dll");
                //paras.ReferencedAssemblies.Add("System.Xml.Linq.dll");
                //paras.ReferencedAssemblies.Add("System.linq.dll");
                //paras.ReferencedAssemblies.Add("System.Data.dll");
                //paras.ReferencedAssemblies.Add("System.Drawing.dll");
                //引入第三方dll
                if (!string.IsNullOrEmpty(DependencyDllPath) && System.IO.Directory.Exists(DependencyDllPath))
                {
                    var directoryInfo = new System.IO.DirectoryInfo(DependencyDllPath);
                    var files = directoryInfo.GetFiles("*.dll");
                    foreach (var item in files)
                        paras.ReferencedAssemblies.Add(item.FullName);
                }
                //是否内存中生成输出
                paras.GenerateInMemory = true;
                //是否生成可执行文件
                paras.GenerateExecutable = false;
                //string OutPutDllPath = "";
                //paras.OutputAssembly = OutPutDllPath;
                //paras.OutputAssembly = binpath + dllName + ".dll";
                //编译代码
                var result = complier.CompileAssemblyFromSource(paras, CsScriptCode);
                //编译脚本异常
                if (result.Errors.HasErrors)
                {
                    sbError.AppendLine("【编译】脚本异常: ");
                    foreach (CompilerError err in result.Errors)
                        sbError.AppendLine($"{err.ErrorText}");
                }
                else
                {
                    //if (null == StaticInfo.ScriptInfoCompilerResults)
                    //    StaticInfo.ScriptInfoCompilerResults = new Dictionary<Guid, CompilerResults>();
                    //if (StaticInfo.ScriptInfoCompilerResults.ContainsKey(CsScriptGuid))
                    //    StaticInfo.ScriptInfoCompilerResults[CsScriptGuid] = result;
                    //else
                    //    StaticInfo.ScriptInfoCompilerResults.Add(CsScriptGuid, result);

                    if (null == StaticInfo.ScriptInfoCompilerResults)
                        StaticInfo.ScriptInfoCompilerResults = new List<CsScirptCompilerInfo>();

                    var OldComplierResult = StaticInfo.ScriptInfoCompilerResults.FirstOrDefault(o => o.CsScirptGuid == CsScriptGuid);
                    if (null == OldComplierResult)
                        StaticInfo.ScriptInfoCompilerResults.Add(new CsScirptCompilerInfo
                        {
                            CsScirptGuid = CsScriptGuid,
                            CsScirptCode = CsScriptCode,
                            CompilerResults = result
                        });
                    else
                        OldComplierResult.CompilerResults = result;

                    CompilerFalg = true;
                }
            }
            catch (Exception ex)
            {
                sbError.AppendLine("【执行】编译异常：");
                sbError.AppendLine(ex.ToString());
            }
            if (sbError.Length > 0)
                CompilerMessage = sbError.ToString();
            return CompilerFalg;
        }

        public ScriptResults<SampleData> RunFromCsCodeCompilerHandleFile(Guid CsScriptGuid, string FilePathHandle, string FilePathSource, ref string RunMessage, ref string RunLog)
        {
            //if (!StaticInfo.ScriptInfoCompilerResults.ContainsKey(CsScriptGuid))
            //{
            //    RunMessage = "【1】:未找到脚本编译信息请先编译脚本";
            //    return null;
            //}
            //var compilerResults = StaticInfo.ScriptInfoCompilerResults[CsScriptGuid];
            //if (null == compilerResults)
            //{
            //    RunMessage = "【2】:未找到脚本编译信息请先编译脚本";
            //    return null;
            //}
            if (null == StaticInfo.ScriptInfoCompilerResults || StaticInfo.ScriptInfoCompilerResults.Count <= 0)
            {
                RunMessage = "【1】:未找到脚本编译信息请先编译脚本";
                return null;
            }
            var compilerResults = StaticInfo.ScriptInfoCompilerResults.FirstOrDefault(o => o.CsScirptGuid == CsScriptGuid);
            if (null == compilerResults)
            {
                RunMessage = "【2】:未找到脚本编译信息请先编译脚本";
                return null;
            }
            try
            {
                ICsScriptHandleFile iCsScript = null;
                //var objAssembly = compilerResults.CompiledAssembly;
                var objAssembly = compilerResults.CompilerResults.CompiledAssembly;
                Type[] types = objAssembly.GetTypes();
                foreach (var t in types)
                    if (t.GetInterface("ICsScriptHandleFile") != null)
                        iCsScript = (ICsScriptHandleFile)Activator.CreateInstance(t);
                if (iCsScript != null)
                {

                    var fileInfo = new FileInfoWaitHandle()
                    {
                        FilePathHandle = FilePathHandle,
                        FilePathSource = FilePathSource
                    };
                    iCsScript.Init(fileInfo);//初始化脚本参数

                    try
                    {
                        iCsScript.Run();

                        //用于调试
                        //输出脚本执行日志
                        if (iCsScript.RunLog.Length>0)
                            RunLog = iCsScript.RunLog.ToString();

                        RunMessage = string.Empty;
                        return iCsScript.ResultData;
                    }
                    catch (Exception ex)
                    {
                        var sbError = new StringBuilder();
                        sbError.AppendLine("【运行】脚本异常：");
                        sbError.AppendLine(ex.ToString());
                        RunMessage = sbError.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                var sbError = new StringBuilder();
                sbError.AppendLine("【执行】脚本异常：");
                sbError.AppendLine(ex.ToString());
                RunMessage = sbError.ToString();
            }
            return null;
        }

    }

}
