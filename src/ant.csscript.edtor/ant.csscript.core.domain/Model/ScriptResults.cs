using System.Collections.Generic;

namespace ant.csscript.core.domain
{
    public class ScriptResults<T> where T : class
    {
        /// <summary>
        /// 文本数据
        /// </summary>
        public string FileText { get; set; }
        /// <summary>
        /// 表格数据
        /// </summary>
        public List<AreaData> FileAreaData { get; set; }
        /// <summary>
        /// 格式化数据
        /// </summary>
        public List<T> Data { get; set; }
    }
}