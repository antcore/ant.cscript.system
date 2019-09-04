using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ant.handle.files.scanning
{
    public class FileScanHandle
    {
        public void GetFiles(FileScanRule rule, ref List<string> FileScanResult)
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
                        //var newRule = DeepCopyByBin(rule);
                        var newRule = rule.DeepCopyByBin();

                        ++newRule.DirectoryDeepCurrent;
                        newRule.PathDirectory = directory;

                        // 递归查找
                        GetFiles(newRule, ref FileScanResult);
                    }
                }
                #endregion
            }
        }

    }
}
