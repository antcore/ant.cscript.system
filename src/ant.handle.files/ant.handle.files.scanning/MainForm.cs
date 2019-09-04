using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
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
            ThreadAction((o) =>
            {
                ServiceScanFiles();
            });
        }

        private void showMessage(string message, Color color)
        {
            if (!flaThreadServiceScanFiles)
                return;
            Invoke(new MethodInvoker(delegate
            {
                if (richTextBoxMessage.TextLength > 5000)
                    richTextBoxMessage.Clear();

                richTextBoxMessage.SelectionColor = color;
                richTextBoxMessage.AppendText(message);
                richTextBoxMessage.ScrollToCaret();
            }));
        }


        bool flaThreadServiceScanFiles = true;

        void ServiceScanFiles()
        {
            string rootPath = string.Empty;

            rootPath = @"C:\00Files\00testfiles";
            rootPath = @"C:\Users\mr\Desktop\新建文件夹";


            // 过滤目录
            var filterDirectoryNames = "*";
            // 过滤文件夹
            var fileNameFilter = "*.txt";
            // 检查文件开始时间
            var CheckFileStartDateTime = DateTime.Parse("2019-09-03");
            // 检查文件夹深度
            var DirectoryDeepCheck = 3;
            var DirectoryDeepCurrent = 1;
            //扫描间隔
            int ScanIntervalSeconds = 5;

            fileNameFilter = "*.txt,*.xlsx,*.xls";

            Stopwatch stopWatch = new Stopwatch();
            StringBuilder sbMessage = new StringBuilder();
            var ResultFileScan = new List<string>();

            DateTime runStart = DateTime.Parse("2019-09-04");
            while (flaThreadServiceScanFiles)
            {
                if (runStart.AddSeconds(ScanIntervalSeconds) <= DateTime.Now)
                {
                    #region 执行文件查找
                    sbMessage.Clear();
                    stopWatch.Restart();

                    sbMessage.AppendLine(@"-------------------- 文件扫描开始 --------------------");
                    sbMessage.AppendLine($"【扫描条件：目录】    {rootPath}");
                    sbMessage.AppendLine($"【扫描条件：目录深度】{DirectoryDeepCheck}");
                    sbMessage.AppendLine($"【扫描条件：目录过滤】{filterDirectoryNames}");
                    sbMessage.AppendLine($"【扫描条件：文件过滤】{fileNameFilter}");
                    sbMessage.AppendLine($"【扫描条件：起始时间】{CheckFileStartDateTime}");
                    sbMessage.AppendLine();
                    sbMessage.AppendLine($"【扫描结果】：【执行时间】{DateTime.Now}");
                    if (Directory.Exists(rootPath))
                    {
                        var rule = new FileScanningRule(filterDirectoryNames, fileNameFilter);
                        rule.PathDirectory = rootPath;
                        rule.DirectoryDeepCheck = DirectoryDeepCheck;
                        rule.DirectoryDeepCurrent = DirectoryDeepCurrent;
                        rule.CheckFileStartDateTime = CheckFileStartDateTime;
                        try
                        {
                            stopWatch.Start();
                            ResultFileScan.Clear();
                            GetFiles(rule, ref ResultFileScan);
                            stopWatch.Stop();

                            sbMessage.AppendLine($"【扫描结果】：【扫描耗时】{stopWatch.ElapsedMilliseconds} (毫秒)");
                            sbMessage.AppendLine($"【扫描结果】：【发现文件】{ResultFileScan.Count} (个)");

                            CheckFileStartDateTime = DateTime.Now.AddMilliseconds(-stopWatch.ElapsedMilliseconds);
                            //记录文件
                            if (ResultFileScan.Count > 0)
                            {
                                sbMessage.AppendLine();
                                sbMessage.AppendLine($"    ************  目标文件 START ************ ");
                                foreach (var item in ResultFileScan)
                                {
                                    sbMessage.AppendLine($" File:   {item}");
                                }
                                sbMessage.AppendLine($"    ************  目标文件 END ************ ");
                                sbMessage.AppendLine();
                            }
                        }
                        catch (Exception ex)
                        {
                            sbMessage.AppendLine($"【扫描文件异常】{ex.ToString()}");
                        }
                    }
                    else
                        sbMessage.AppendLine($" 【扫描结果】：文件夹不存在 ({rootPath})");

                    sbMessage.AppendLine(@"-------------------- 文件扫描结束 --------------------");
                    sbMessage.AppendLine();

                    showMessage(sbMessage.ToString(), Color.Black);
                    #endregion

                    runStart = DateTime.Now;
                }
                Thread.Sleep(1000);
            }
        }
        public void GetFiles(FileScanningRule rule, ref List<string> FileScanResult)
        {
            var dirInfo = new DirectoryInfo(rule.PathDirectory);
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
                    files = Directory.GetFiles(rule.PathDirectory, FilterFileName, SearchOption.TopDirectoryOnly);
                    if (null != files && files.Length > 0)
                        listFiles.AddRange(files);
                }
                //排除指定规则的文件
                if (listFiles.Count > 0 && rule.FilterFileNamesExclude.Count > 0)
                {
                    var listFilesExclude = new List<string>();
                    foreach (var FilterFileName in rule.FilterFileNamesExclude)
                    {
                        files = Directory.GetFiles(rule.PathDirectory, FilterFileName, SearchOption.TopDirectoryOnly);
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
                        //特殊文件处理
                        if (filePath.StartsWith("~$") || filePath.StartsWith("$"))
                            continue;

                        if (File.Exists(filePath))
                        {
                            info = new FileInfo(filePath);
                            //检索的起始时间 < 更新时间 创建时间 
                            if (info.CreationTime < rule.CheckFileStartDateTime && info.LastWriteTime < rule.CheckFileStartDateTime)
                                continue;

                            // 延迟采集
                            // ？

                            //归档状态为 A 则采集  N 则不采集
                            if (info.Attributes == FileAttributes.Archive)
                            {
                                FileScanResult.Add(filePath);
                                //标记文件已经被采集
                                info.Attributes = FileAttributes.Normal;
                            }
                        }
                    }
                }

                #region 下级目录文件查找
                var listDirectory = new List<string>();
                if (rule.DirectoryDeepCurrent < rule.DirectoryDeepCheck)
                {
                    string[] directories;
                    foreach (var FilterDirectoryName in rule.FilterDirectoryNames)
                    {
                        directories = Directory.GetDirectories(rule.PathDirectory, FilterDirectoryName, SearchOption.TopDirectoryOnly);
                        if (null != directories && directories.Length > 0)
                            listDirectory.AddRange(directories);
                    }
                    if (listDirectory.Count > 0 && rule.FilterDirectoryNamesExclude.Count > 0)
                    {
                        var listDirectoryExclude = new List<string>();
                        string[] directoriesExclude;
                        foreach (var FilterDirectoryName in rule.FilterDirectoryNamesExclude)
                        {
                            directoriesExclude = Directory.GetDirectories(rule.PathDirectory, FilterDirectoryName, SearchOption.TopDirectoryOnly);
                            if (null != directoriesExclude && directoriesExclude.Length > 0)
                                listDirectoryExclude.AddRange(directoriesExclude);
                        }
                        if (listDirectoryExclude.Count > 0)
                        {
                            //交集算出需要排出的文件夹
                            var items = listDirectory.Intersect(listDirectoryExclude).ToList();
                            //差集算出目标文件夹
                            listDirectory = listDirectory.Except(items).ToList();
                        }
                    }
                }
                if (listDirectory.Count > 0)
                {
                    // 存在下一级目录 继续寻找下级目录中的文件
                    foreach (var directory in listDirectory)
                    {
                        var newRule = DeepCopyByBin(rule);

                        ++newRule.DirectoryDeepCurrent;
                        newRule.PathDirectory = directory;

                        // 递归查找
                        GetFiles(newRule, ref FileScanResult);
                    }
                }
                #endregion
            }
        }

        [Serializable]
        public class FileScanningRule
        {
            public FileScanningRule(string filterDirectoryNames, string fileNameFilter, string filterDirectoryNamesExclude = "", string fileNameFilterExclude = "")
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



        private void ThreadAction(Action<object> action)
        {
            new Thread(new ParameterizedThreadStart(action)).Start();
        }
        /// <summary>
        /// 深拷贝，通过序列化与反序列化实现
        /// </summary>
        /// <typeparam name="T">深拷贝的类类型</typeparam>
        /// <param name="obj">深拷贝的类对象</param>
        /// <returns></returns>
        public T DeepCopyByBin<T>(T obj)
        {
            object retval;
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                //序列化成流
                bf.Serialize(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                //反序列化成对象
                retval = bf.Deserialize(ms);
                ms.Close();
            }
            return (T)retval;
        }
        /// <summary>
        /// 深拷贝，通过反射实现
        /// </summary>
        /// <typeparam name="T">深拷贝的类类型</typeparam>
        /// <param name="obj">深拷贝的类对象</param>
        /// <returns></returns>
        public T DeepCopyByReflect<T>(T obj)
        {
            //如果是字符串或值类型则直接返回
            if (obj is string || obj.GetType().IsValueType) return obj;

            object retval = Activator.CreateInstance(obj.GetType());
            FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            foreach (FieldInfo field in fields)
            {
                try { field.SetValue(retval, DeepCopyByReflect(field.GetValue(obj))); }
                catch { }
            }
            return (T)retval;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            flaThreadServiceScanFiles = false;
        }
    }
}
