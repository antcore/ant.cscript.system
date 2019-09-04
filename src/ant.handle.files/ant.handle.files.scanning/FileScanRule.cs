using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ant.handle.files.scanning
{
    [Serializable]
    public class FileScanRule
    {
        public FileScanRule(string filterDirectoryNames, string fileNameFilter, string filterDirectoryNamesExclude = "", string fileNameFilterExclude = "")
        {
            if (string.IsNullOrEmpty(filterDirectoryNames))
                FilterDirectoryNames = filterDirectoryNames.Split(',').ToList();
            else
                FilterDirectoryNames = new List<string> { "*" };

            if (!string.IsNullOrEmpty(fileNameFilter))
                FilterFileNames = fileNameFilter.Split(',').ToList();
            else
                FilterFileNames = new List<string> { "*.*" };

            if (string.IsNullOrEmpty(filterDirectoryNamesExclude))
                FilterDirectoryNamesExclude = filterDirectoryNamesExclude.Split(',').ToList();
            else
                FilterDirectoryNamesExclude = new List<string>();

            if (!string.IsNullOrEmpty(fileNameFilterExclude))
                FilterFileNamesExclude = fileNameFilterExclude.Split(',').ToList();
            else
                FilterFileNamesExclude = new List<string>();


            CheckFileStartDateTime = DateTime.Now;

            DirectoryDeepCheck = 3;
            DirectoryDeepCurrent = 1;
        }
        /// <summary>
        /// 【扫描文件】文件夹目录
        /// </summary>
        public string PathDirectory { get; set; }
        /// <summary>
        /// 【扫描文件】检查文件开始时间 只找时间比此大的
        /// </summary>
        public DateTime CheckFileStartDateTime { get; set; }
        /// <summary>
        /// 【扫描文件】文件夹最深深度 文件夹层数
        /// </summary>
        public int DirectoryDeepCheck { get; set; }
        /// <summary>
        /// 【扫描文件】当前扫描文件夹 层数 初始 1
        /// </summary>
        public int DirectoryDeepCurrent { get; set; }

        public List<string> FilterDirectoryNames { get; set; }
        public List<string> FilterDirectoryNamesExclude { get; set; }
        public List<string> FilterFileNames { get; set; }
        public List<string> FilterFileNamesExclude { get; set; }
    }
}
