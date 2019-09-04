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
        private readonly Timer m_Timer;
        private readonly Int32 m_TimerInterval;
        private readonly FileSystemWatcher m_FileSystemWatcher;
        private readonly FileSystemEventHandler m_FileSystemEventHandler;
        private readonly Dictionary<String, FileSystemEventArgs> m_ChangedFiles = new Dictionary<string, FileSystemEventArgs>();

        public DelayFileSystemWatcher(string path, string filter, FileSystemEventHandler watchHandler, int timerInterval)
        {
            m_Timer = new Timer(OnTimer, null, Timeout.Infinite, Timeout.Infinite);
            m_FileSystemWatcher = new FileSystemWatcher(path, filter);
            m_FileSystemWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.CreationTime;
            m_FileSystemWatcher.Created += fileSystemWatcher_Changed;
            m_FileSystemWatcher.Changed += fileSystemWatcher_Changed;
            m_FileSystemWatcher.Deleted += fileSystemWatcher_Changed;
            m_FileSystemWatcher.Renamed += fileSystemWatcher_Changed;

            //子目录
            m_FileSystemWatcher.IncludeSubdirectories = true;

            m_FileSystemWatcher.EnableRaisingEvents = true;
            m_FileSystemEventHandler = watchHandler;
            m_TimerInterval = timerInterval;
        }

        public void fileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            lock (m_ChangedFiles)
            {
                if (!m_ChangedFiles.ContainsKey(e.FullPath))
                {
                    m_ChangedFiles.Add(e.FullPath, e);
                }
            }
            m_Timer.Change(m_TimerInterval, Timeout.Infinite);
        }

        private void OnTimer(object state)
        {
            var tempChangedFiles = new Dictionary<string, FileSystemEventArgs>();

            lock (m_ChangedFiles)
            {
                foreach (KeyValuePair<string, FileSystemEventArgs> changedFile in m_ChangedFiles)
                {
                    tempChangedFiles.Add(changedFile.Key, changedFile.Value);
                }
                m_ChangedFiles.Clear();
            }

            foreach (KeyValuePair<string, FileSystemEventArgs> changedFile in tempChangedFiles)
            {
                m_FileSystemEventHandler(this, changedFile.Value);
            }
        }
    }
}
