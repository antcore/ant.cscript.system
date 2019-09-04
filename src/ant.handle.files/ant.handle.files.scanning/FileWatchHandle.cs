using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ant.handle.files.scanning
{
    // https://www.cnblogs.com/zanxiaofeng/archive/2011/01/08/1930583.html

    public class FileWatchHandle
    {

        public delegate void handelGetFile(string fileFullPath);

        public event handelGetFile EventGetFile;


        private string WathtPathDirectory;
        private FileSystemWatcher watcher;
        public FileWatchHandle(string PathDirectory)
        {
            WathtPathDirectory = PathDirectory;
            //创建一个新的FileSystemWatcher并设置其属性
            watcher = new FileSystemWatcher();
        }



        public void Start()
        {
            watcher.Path = WathtPathDirectory;
            /*监视LastAcceSS和LastWrite时间的更改以及文件或目录的重命名*/
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            //只监视文本文件
            watcher.Filter = "*.txt";
            //添加事件句柄
            //当由FileSystemWatcher所指定的路径中的文件或目录的
            //大小、系统属性、最后写时间、最后访问时间或安全权限
            //发生更改时，更改事件就会发生
            //watcher.Changed += new FileSystemEventHandler(OnChanged);
            ////由FileSystemWatcher所指定的路径中文件或目录被创建时，创建事件就会发生
            //watcher.Created += new FileSystemEventHandler(OnChanged);
            ////当由FileSystemWatcher所指定的路径中文件或目录被删除时，删除事件就会发生
            //watcher.Deleted += new FileSystemEventHandler(OnChanged);
            ////当由FileSystemWatcher所指定的路径中文件或目录被重命名时，重命名事件就会发生
            //watcher.Renamed += new RenamedEventHandler(OnRenamed);

            watcher.IncludeSubdirectories = true;

            watcher.Changed += OnChanged;
            //由FileSystemWatcher所指定的路径中文件或目录被创建时，创建事件就会发生
            watcher.Created += OnChanged;
            //当由FileSystemWatcher所指定的路径中文件或目录被删除时，删除事件就会发生
            //watcher.Deleted += OnChanged;
            //当由FileSystemWatcher所指定的路径中文件或目录被重命名时，重命名事件就会发生
            watcher.Renamed += OnRenamed;
            //开始监视
            watcher.EnableRaisingEvents = true;
        }
        public void Stop()
        {
            watcher.Changed -= OnChanged;
            //由FileSystemWatcher所指定的路径中文件或目录被创建时，创建事件就会发生
            watcher.Created -= OnChanged;
            //当由FileSystemWatcher所指定的路径中文件或目录被删除时，删除事件就会发生
            //watcher.Deleted += OnChanged;
            //当由FileSystemWatcher所指定的路径中文件或目录被重命名时，重命名事件就会发生
            watcher.Renamed -= OnRenamed;
            //开始监视
            watcher.EnableRaisingEvents = false;
        }


        private Boolean flagIsFilst = true;

        //定义事件处理程序
        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            
                watcher.EnableRaisingEvents = false;

            watcher.EnableRaisingEvents = true;




            if (e.ChangeType == WatcherChangeTypes.Created)
                EventGetFile("1 " + " " + e.ChangeType.ToString() + " " + e.FullPath);
            if (e.ChangeType == WatcherChangeTypes.Changed)
                EventGetFile("1 " + " " + e.ChangeType.ToString() + " " + e.FullPath);
            if (e.ChangeType == WatcherChangeTypes.Renamed)
                EventGetFile("1 " + " " + e.ChangeType.ToString() + " " + e.FullPath);

           

       
        }
        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Renamed)
                EventGetFile( "2 " + " "+ e.ChangeType .ToString()+ " " + e.FullPath);
            //指定当文件被重命名时发生的动作
            Console.WriteLine("Fi]e:{0} renamed to{1}", e.OldFullPath, e.FullPath);
        }



    }
}
