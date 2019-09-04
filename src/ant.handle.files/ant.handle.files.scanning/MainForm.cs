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


        private void FileWatch_EventGetFile(string fileFullPath)
        {
            

            //sbMessage.AppendLine(fileFullPath);

            //showMessage(sbMessage.ToString(), Color.Black);
        }

        private   void fileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            StringBuilder sbMessage = new StringBuilder();
            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Created:
                case WatcherChangeTypes.Changed:
                case WatcherChangeTypes.Renamed:
                default:
                    sbMessage.AppendLine(e.FullPath);
                    break;
            }
            showMessage(sbMessage.ToString(), Color.Black);
        }

        void ServiceScanFiles()
        {
            string rootPath = string.Empty;

            rootPath = @"C:\00Files\00testfiles";
            rootPath = @"C:\Users\mr\Desktop\新建文件夹";

            var fileNameFilter = "*.txt";

            //var fileWatch = new FileWatchHandle(rootPath);

            //fileWatch.EventGetFile += FileWatch_EventGetFile;
            //fileWatch.Start();

            var  s_DelayFileSystemWatcher = new DelayFileSystemWatcher(rootPath, fileNameFilter, fileSystemWatcher_Changed, 500);
            


            return;

              rootPath = string.Empty;

            rootPath = @"C:\00Files\00testfiles";
            rootPath = @"C:\Users\mr\Desktop\新建文件夹";


            // 过滤目录
            var filterDirectoryNames = "*";
            // 过滤文件夹
              fileNameFilter = "*.txt";
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

            var handle = new FileScanHandle();
            while (flaThreadServiceScanFiles)
            {
                //扫描周期内执行查找
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
                        var rule = new FileScanRule(filterDirectoryNames, fileNameFilter);
                        rule.PathDirectory = rootPath;
                        rule.DirectoryDeepCheck = DirectoryDeepCheck;
                        rule.DirectoryDeepCurrent = DirectoryDeepCurrent;
                        rule.CheckFileStartDateTime = CheckFileStartDateTime;
                        try
                        {
                            stopWatch.Start();
                            ResultFileScan.Clear();
                            handle.GetFiles(rule, ref ResultFileScan);
                            stopWatch.Stop();

                            sbMessage.AppendLine($"【扫描结果】：【扫描耗时】{stopWatch.ElapsedMilliseconds} (毫秒)");
                            sbMessage.AppendLine($"【扫描结果】：【发现文件】{ResultFileScan.Count} (个)");

                            CheckFileStartDateTime = DateTime.Now.AddMilliseconds(-stopWatch.ElapsedMilliseconds-50);
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

     
        private void ThreadAction(Action<object> action)
        {
            new Thread(new ParameterizedThreadStart(action)).Start();
        } 

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            flaThreadServiceScanFiles = false;
        }
    }
}
