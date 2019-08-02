using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ant.csscript.core.domain
{
    public class FileInfoWaitHandle
    {
        /// <summary>
        /// 文件路径 等待处理
        /// </summary>
        public string FilePathHandle { get; set; }
        /// <summary>
        /// 文件路径 文件实际路径
        /// </summary>
        public string FilePathSource { get; set; }

    }
}
