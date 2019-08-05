using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ant.csscript.handle.domain.core
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

        List<MetadataReference> references = new List<MetadataReference>();

        public CsScriptHandleFile(string _DependencyDllPath = "" )
        {
            DependencyDllPath = string.IsNullOrEmpty(_DependencyDllPath) ? GetPathPluginsCsScriptDependencyDll : _DependencyDllPath;
            //引入第三方dll
            if (!string.IsNullOrEmpty(DependencyDllPath) && System.IO.Directory.Exists(DependencyDllPath))
            {
                var directoryInfo = new System.IO.DirectoryInfo(DependencyDllPath);
                var files = directoryInfo.GetFiles("*.dll");
                foreach (var item in files)
                    references.Add(MetadataReference.CreateFromFile(item.FullName));
            }
            // 引用系统依赖
            references.AddRange(AppDomain.CurrentDomain.GetAssemblies().Select(x => MetadataReference.CreateFromFile(x.Location)));

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
                sbError.AppendLine(AppDomain.CurrentDomain.GetAssemblies().Length + "-----------------AppDomain.CurrentDomain.GetAssemblies()");

                // 引用系统依赖
                //var references = AppDomain.CurrentDomain.GetAssemblies().Select(x => {
                //    sbError.AppendLine(x.Location + "");
                //    return MetadataReference.CreateFromFile(x.Location);
                //});
                //List<MetadataReference> _references = new List<MetadataReference>();
                ////引入第三方dll
                //if (!string.IsNullOrEmpty(DependencyDllPath) && System.IO.Directory.Exists(DependencyDllPath))
                //{
                //    var directoryInfo = new System.IO.DirectoryInfo(DependencyDllPath);
                //    var files = directoryInfo.GetFiles("*.dll");
                //    foreach (var item in files)
                //        references.Add(MetadataReference.CreateFromFile(item.FullName));
                //}
                //_references.AddRange(references);

                SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(CsScriptCode);

                CSharpCompilation compilation = CSharpCompilation.Create(
                    null,
                    syntaxTrees: new[] { syntaxTree },
                    references: references,
                    options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

                using (var ms = new MemoryStream())
                {
                    EmitResult result = compilation.Emit(ms);

                    //编译脚本异常
                    if (!result.Success)
                    {
                        IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                            diagnostic.IsWarningAsError ||
                            diagnostic.Severity == DiagnosticSeverity.Error);

                        sbError.AppendLine("【编译】脚本异常: ");
                        foreach (Diagnostic diagnostic in failures)
                            sbError.AppendLine($"{diagnostic.Id}:${diagnostic.GetMessage()}");
                    }
                    else
                    {
                        ms.Seek(0, SeekOrigin.Begin);
                        Assembly assembly = Assembly.Load(ms.ToArray());

                        if (null == StaticInfo.ScriptInfoCompilerResults)
                            StaticInfo.ScriptInfoCompilerResults = new List<CsScirptCompilerInfo>();
                        var OldComplierResult = StaticInfo.ScriptInfoCompilerResults.FirstOrDefault(o => o.CsScirptGuid == CsScriptGuid);
                        if (null == OldComplierResult)
                            StaticInfo.ScriptInfoCompilerResults.Add(new CsScirptCompilerInfo
                            {
                                CsScirptGuid = CsScriptGuid,
                                CsScirptCode = CsScriptCode,
                                CsAssembly = assembly
                            });
                        else
                            OldComplierResult.CsAssembly = assembly;

                        CompilerFalg = true;
                    }
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
                var objAssembly = compilerResults.CsAssembly;
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
                        if (iCsScript.RunLog.Length > 0)
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