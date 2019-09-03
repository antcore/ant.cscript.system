using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ant.handle.files.scanning
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            ServiceScanFiles();
        }

        void ServiceScanFiles()
        {
            string rootPath = string.Empty;

            rootPath = @"C:\00Files\00testfiles";

            StringBuilder sbMessage = new StringBuilder();
            List<string> files = new List<string>();
            while (true)
            {
                if (System.IO.Directory.Exists(rootPath))
                {
                    var fileNameFilter = "*.txt";
                    // 检查文件开始时间
                    DateTime CheckFileStartDateTime = DateTime.Parse("2019-09-03");
                    // 检查文件夹深度
                    int DirectoryDeepCheck = 3;
                    int DirectoryDeepCurrent = 1;

                    var rule = new FileScanningRule("*",fileNameFilter);
                    rule.PathDirectory = rootPath;
                    rule.DirectoryDeepCheck = DirectoryDeepCheck;
                    rule.DirectoryDeepCurrent = DirectoryDeepCurrent;



                    GetFiles(rule);

                    //GetFiles(rootPath, CheckFileStartDateTime, CheckDirectoryDeep, CurrentDirectoryDeep, ref files);
                }
                sbMessage.AppendLine(@" 文件夹不存在 ");

                System.Threading.Thread.Sleep(1000 * 5);
            }
        }
        private void GetFiles(FileScanningRule rule)
        {
            var dirInfo = new System.IO.DirectoryInfo(rule.PathDirectory);
            //文件在规定的无条件搜索深度范围内 不检查文件夹的创建时间和写入时间
            //文件夹上次写入时间大于上次检查时间  认为文件夹下的文件有更新
            //文件夹创建时间大于置顶的检查时间  认为文件夹需要检查
            if (rule.DirectoryDeepCurrent <= rule.DirectoryDeepCheck || rule.CheckFileStartDateTime < dirInfo.CreationTime)
            {
                List<string> listFiles = new List<string>();
                //查询指定规则的文件
                string[] files;
                foreach (var FilterFileName in rule.FilterFileNames)
                {
                    files = Directory.GetFiles(rule.PathDirectory, FilterFileName, System.IO.SearchOption.TopDirectoryOnly);
                    if (null != files && files.Length > 0)
                        listFiles.AddRange(files);
                }
                //排除指定规则的文件
                if (listFiles.Count > 0 && rule.FilterFileNamesExclude.Count > 0)
                {
                    var listFilesExclude = new List<string>();
                    foreach (var FilterFileName in rule.FilterFileNamesExclude)
                    {
                        files = Directory.GetFiles(rule.PathDirectory, FilterFileName, System.IO.SearchOption.TopDirectoryOnly);
                        if (null != files && files.Length > 0)
                            listFilesExclude.AddRange(files);
                    }
                    if (listFilesExclude.Count > 0)
                    {
                        //交集算出需要排出的文件
                        var items = listFiles.Intersect(listFilesExclude).ToList();
                        //差集算出目标文件
                        listFiles = listFiles.Except(items).ToList();
                    }
                }
                if (listFiles.Count > 0)
                {
                    FileInfo info;
                    foreach (var filePath in listFiles)
                    {
                        if (filePath.StartsWith("~$") || filePath.StartsWith("$"))
                            continue;
                        if (File.Exists(filePath))
                        {
                            info = new System.IO.FileInfo(filePath);
                            //检索的起始时间 < 更新时间 创建时间 
                            if (info.CreationTime < rule.CheckFileStartDateTime && info.LastWriteTime < rule.CheckFileStartDateTime)
                                continue;

                            // 延迟采集
                            // ？

                            //归档状态为 A 则采集  N 则不采集
                            if (info.Attributes == FileAttributes.Archive)
                            {
                                if (rule.ResultFileScan.Contains(filePath))
                                {
                                    rule.ResultFileScan.Add(filePath);
                                    //标记文件已经被采集
                                    info.Attributes = FileAttributes.Normal;
                                }
                            }
                        }
                    }
                }

                string[] directories;
                string[] directoriesExclude = new string[0];
                if (searchDeep <= _searchRule.directorySearchDeep)
                {
                    directories = Directory.GetDirectories(rule.PathDirectory, _searchRule.directoryFilter, System.IO.SearchOption.TopDirectoryOnly);
                    //需要排除的文件夹
                    if (!string.IsNullOrWhiteSpace(_searchRule.directoryExcludeFilter))
                        directoriesExclude = Directory.GetDirectories(path, _searchRule.directoryExcludeFilter, System.IO.SearchOption.TopDirectoryOnly);

                    var items = directories.Intersect(directoriesExclude);//交集算出需要排出的文件
                    directories = directories.Except(items).ToArray();//差集算出目标文件  需要提交的文件
                }
                else
                {
                    //搜索深度已经大于目标深度，则不再继续搜索
                    return;
                }

                searchDeep++;//搜索深度增加

                foreach (var dir in directories)
                {
                    getFiles(dir, pathCheckTimeLimit, searchDeep, ref files);
                }



            }
        }


        public class FileScanningRule
        {
            public FileScanningRule(string filterDirectoryNames, string fileNameFilter,  string filterDirectoryNamesExclude = "", string fileNameFilterExclude = "")
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

                ResultFileScan = new List<string>();
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

            public List<string> ResultFileScan { get; set; }
        }



        private void getNewFiles(string path)
        {
            _Files.Clear();
            DateTime beginTime = _searchRule.firstSearchTime;

            getFiles(path, beginTime, 1, ref _Files);

            //检测到的文件变更放入到文件队列
            while (_FilesHaschangedList.Count > 0)
            {
                if (!_Files.Contains(_FilesHaschangedList[0]))
                    _Files.Add(_FilesHaschangedList[0]);
                _FilesHaschangedList.RemoveAt(0);
            }

            //移除非正常文件
            for (int i = 0; i < _Files.Count; i++)
            {
                if (_Files[i].StartsWith("~$") || _Files[i].StartsWith("$"))
                {
                    _Files.RemoveAt(i);
                    i--;
                }
            }

            if (_Files.Count > 0)
            {
                string msg = "发现文件:" + _Files.Count;
                if (!daq.tools.Sys.WindowsInfo.IsCnWindows())
                    msg = "find files:" + _Files.Count;
                //appendOnReciveMessage(TraceEventType.Information, "find files:" + _Files.Count);
                appendOnReciveMessage(TraceEventType.Information, msg);
                appendOnFindNewFile(_Files.ToArray());
            }
        }

        //List<string> _tempFiles = new List<string>(1000);
        /// <summary>
        /// 获取文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="files"></param>
        //private void GetFiles(string PathDirectory, DateTime CheckFileStartDateTime, int CheckDirectoryDeep, int CurrentDirectoryDeep, ref List<string> files)
        //{
        //    var dirInfo = new System.IO.DirectoryInfo(PathDirectory);
        //    //文件在规定的无条件搜索深度范围内 不检查文件夹的创建时间和写入时间
        //    //文件夹上次写入时间大于上次检查时间  认为文件夹下的文件有更新
        //    //文件夹创建时间大于置顶的检查时间  认为文件夹需要检查
        //    if (CurrentDirectoryDeep <= CheckDirectoryDeep || CheckFileStartDateTime < dirInfo.CreationTime)
        //    {
        //        //应包含的文件
        //        string[] filters = _searchRule.fileNameFilter.Split(',');
        //        string[] fs = new string[0];

        //        _tempFiles.Clear();
        //        foreach (var value in filters)
        //        {
        //            fs = System.IO.Directory.GetFiles(PathDirectory, _searchRule.fileNameFilter, System.IO.SearchOption.TopDirectoryOnly);
        //            _tempFiles.AddRange(fs);
        //        }
        //        fs = _tempFiles.Distinct().ToArray();
        //        //string[] fs = System.IO.Directory.GetFiles(path, _searchRule.fileNameFilter, System.IO.SearchOption.TopDirectoryOnly);

        //        //应排除的文件
        //        string[] fsExclude = new string[0];
        //        if (!string.IsNullOrWhiteSpace(_searchRule.fileNameExcludeFilter))
        //        {
        //            filters = _searchRule.fileNameExcludeFilter.Split(',');

        //            _tempFiles.Clear();
        //            foreach (var value in filters)
        //            {
        //                fsExclude = System.IO.Directory.GetFiles(path, value, System.IO.SearchOption.TopDirectoryOnly);

        //                _tempFiles.AddRange(fsExclude);
        //            }
        //            fsExclude = _tempFiles.Distinct().ToArray();
        //            // fsExclude = System.IO.Directory.GetFiles(path, _searchRule.fileNameExcludeFilter, System.IO.SearchOption.TopDirectoryOnly);
        //        }

        //        if (fs.Length > 0)
        //        {
        //            var items = fs.Intersect(fsExclude).ToList();//交集算出需要排出的文件
        //            var distFiles = fs.Except(items).ToList();//差集算出目标文件  需要提交的文件

        //            foreach (var file in distFiles)
        //            {
        //                var info = new System.IO.FileInfo(file);
        //                //如果更新时间和创建时间都小于 检索的起始时间 则忽略
        //                if (info.CreationTime < pathCheckTimeLimit && info.LastWriteTime < pathCheckTimeLimit) continue;


        //                //判断归档条件  创建 后多少秒钟 才开始采集
        //                if (_searchRule.doArchiveEventType.ToLower() == "create")
        //                {
        //                    if (info.CreationTime.AddSeconds(_searchRule.timeNumOfDoArchiveAfterEvent) > DateTime.Now) continue;
        //                }
        //                else if (_searchRule.doArchiveEventType.ToLower() == "update")//更新后多少秒钟才开始采集
        //                {
        //                    if (info.LastWriteTime.AddSeconds(_searchRule.timeNumOfDoArchiveAfterEvent) > DateTime.Now) continue;
        //                }

        //                //归档状态为 A 则采集  N 则不采集
        //                if (info.Attributes == FileAttributes.Archive)
        //                {
        //                    if (!files.Contains(file))
        //                    {
        //                        files.Add(file);
        //                        info.Attributes = FileAttributes.Normal;//标记文件已经被采集
        //                    }
        //                }


        //                //删除满足条件的文件
        //                //归档后n秒删除
        //                //创建后n秒删除
        //                if (!string.IsNullOrWhiteSpace(_searchRule.doDeleteEventType))
        //                {
        //                    if (_searchRule.doDeleteEventType.ToLower() == "after archived" && info.Attributes == FileAttributes.Normal)
        //                    {
        //                        if (info.CreationTime.AddSeconds(_searchRule.timeNumOfDoDeleteAfterEvent) < DateTime.Now)
        //                        {
        //                            //删除文件
        //                            deleteFile(file);
        //                        }
        //                    }
        //                    else if (_searchRule.doDeleteEventType.ToLower() == "after created")
        //                    {
        //                        if (info.LastWriteTime.AddSeconds(_searchRule.timeNumOfDoDeleteAfterEvent) < DateTime.Now)
        //                        {
        //                            //删除文件
        //                            deleteFile(file);
        //                        }
        //                    }
        //                    else if (_searchRule.doDeleteEventType.ToLower() == "none") { }
        //                }
        //            }
        //        }

        //        string[] directories = new string[0];
        //        string[] directoriesExclude = new string[0];
        //        if (searchDeep <= _searchRule.directorySearchDeep)
        //        {
        //            directories = System.IO.Directory.GetDirectories(path, _searchRule.directoryFilter, System.IO.SearchOption.TopDirectoryOnly);
        //            //需要排除的文件夹
        //            if (!string.IsNullOrWhiteSpace(_searchRule.directoryExcludeFilter))
        //                directoriesExclude = System.IO.Directory.GetDirectories(path, _searchRule.directoryExcludeFilter, System.IO.SearchOption.TopDirectoryOnly);

        //            var items = directories.Intersect(directoriesExclude);//交集算出需要排出的文件
        //            directories = directories.Except(items).ToArray();//差集算出目标文件  需要提交的文件
        //        }
        //        else
        //        {
        //            //搜索深度已经大于目标深度，则不再继续搜索
        //            return;
        //        }

        //        searchDeep++;//搜索深度增加

        //        foreach (var dir in directories)
        //        {
        //            getFiles(dir, pathCheckTimeLimit, searchDeep, ref files);
        //        }

        //    }
        //}



        private void ThreadAction(Action<object> action)
        {
            new Thread(new ParameterizedThreadStart(action)).Start();
        }


    }
}
