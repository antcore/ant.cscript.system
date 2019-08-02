using ant.csscript.core;
using System.Collections.Generic;
using System.Text;

namespace ant.csscript.core.domain
{
    /// <summary>
    /// 处理文件数据
    /// </summary>
    public interface ICsScriptHandleFile
    {
        /// <summary>
        /// 初始化脚本参数
        /// </summary>
        void Init(FileInfoWaitHandle _fileInfo);
        /// <summary>
        /// 
        /// </summary>
        void Run();
        /// <summary>
        /// 
        /// </summary>
        StringBuilder RunLog { get; set; }
        /// <summary>
        /// 需要处理文件信息
        /// </summary>
        FileInfoWaitHandle FileInfo { get; set; } 
        /// <summary>
        /// 
        /// </summary>
        ScriptResults<SampleData> ResultData { get; set; }
    }
}