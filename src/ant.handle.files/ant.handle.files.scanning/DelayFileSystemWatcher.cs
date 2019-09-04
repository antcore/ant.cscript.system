using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace ant.handle.files.scanning
{
    public class DelayFileSystemWatcher
    {
        private readonly string WatchPathDirectory;
        private readonly FileSystemWatcher fileWatcher;
        private readonly Timer timerHandle;
        private readonly Int32 timerHandleInterval;

        private readonly Dictionary<string, FileSystemEventArgs> changedFiles ;

        private readonly FileSystemEventHandler fileSystemEventHandler;
        public DelayFileSystemWatcher(string WatchDirectory, string filter, FileSystemEventHandler watchHandler, int timerInterval = 250)
        {
            if (!Directory.Exists(WatchDirectory))
                return ;
            WatchPathDirectory = WatchDirectory;

            fileWatcher = new FileSystemWatcher();
            fileWatcher.Path = WatchDirectory;
            fileWatcher.Filter = filter;

            //附带监控子目录
            fileWatcher.IncludeSubdirectories = true;
            //监控事件类型
            fileWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.CreationTime;

            timerHandle = new Timer(OnTimer, null, Timeout.Infinite, Timeout.Infinite);
            changedFiles = new Dictionary<string, FileSystemEventArgs>();
            
            fileSystemEventHandler = watchHandler;
            timerHandleInterval = timerInterval;
        }

        public bool Start()
        {
            if (!Directory.Exists(WatchPathDirectory))
                return false;

            fileWatcher.Created += FileWatcher_Created;
            fileWatcher.Created += FileWatcher_Created;
            fileWatcher.Changed += FileWatcher_Created;
            //fileWatcher.Deleted += FileWatcher_Created;
            fileWatcher.Renamed += FileWatcher_Created;

            fileWatcher.EnableRaisingEvents = true;

            return true;
        }

        public void Stop()
        {
            fileWatcher.Created -= FileWatcher_Created;
            fileWatcher.Changed -= FileWatcher_Created;
            //fileWatcher.Deleted -= FileWatcher_Created;
            fileWatcher.Renamed -= FileWatcher_Created;

            fileWatcher.EnableRaisingEvents = false;
        }


        public void FileWatcher_Created(object sender, FileSystemEventArgs e)
        {
            lock (changedFiles)
                if (!changedFiles.ContainsKey(e.FullPath))
                    changedFiles.Add(e.FullPath, e);
            timerHandle.Change(timerHandleInterval, Timeout.Infinite);
        }

        private void OnTimer(object state)
        {
            var _changedFiles = new Dictionary<string, FileSystemEventArgs>();

            lock (changedFiles)
            {
                foreach (var changedFile in changedFiles)
                    _changedFiles.Add(changedFile.Key, changedFile.Value);
                changedFiles.Clear();
            }

            foreach (var changedFile in _changedFiles)
                fileSystemEventHandler(this, changedFile.Value);

            _changedFiles.Clear();
        }
    }
}
