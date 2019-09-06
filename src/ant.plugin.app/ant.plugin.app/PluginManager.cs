using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ant.plugin.app
{
    public class PluginManager
    {

        #region PluginManager Singleton
        private static PluginManager instance = null;
        private static readonly object lockObj = new object();
        PluginManager()
        {
        }
        public static PluginManager Instance
        {
            get
            {
                if (instance == null)
                    lock (lockObj)
                        if (instance == null)
                            instance = new PluginManager();
                return instance;
            }
        }
        #endregion

        #region Plugins Directory Path
        private string GetRootPath
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }
        /// <summary>
        /// 服务器下载 依赖 组件[第三方插件]
        /// </summary>
        private string GetPathDownloadDependenciesComponents
        {
            get
            {
                string dirPath = string.Format(@"{0}Dependencies\$Download\Components\", GetRootPath);
                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);
                return dirPath;
            }
        }
        /// <summary>
        /// 服务器下载 依赖 功能插件
        /// </summary>
        private string GetPathDownloadDependenciesPlugins
        {
            get
            {
                string dirPath = string.Format(@"{0}Dependencies\$Download\Plugins\", GetRootPath);
                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);
                return dirPath;
            }
        }
        /// <summary>
        /// 依赖 组件[第三方插件]
        /// </summary>
        private string GetPathDependenciesComponents
        {
            get
            {
                string dirPath = string.Format(@"{0}Dependencies\Components\", GetRootPath);
                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);
                return dirPath;
            }
        }
        /// <summary>
        /// 依赖 功能插件
        /// </summary>
        private string GetPathDependenciesPlugins
        {
            get
            {
                string dirPath = string.Format(@"{0}Dependencies\Plugins\", GetRootPath);
                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);
                return dirPath;
            }
        }
        #endregion

        public void Install()
        {
            var FilterFileName = "*.dll";
            string[] PluginFiles;
            #region 插件维护
            if (CheckUpdate())
            {
                Download();
            }
            /*
             * 1.下载的插件目录是否存在插件
             * 2.存在插件则拷贝更新至对应的插件目录
             */

            PluginFiles = Directory.GetFiles(GetPathDownloadDependenciesComponents, FilterFileName, SearchOption.TopDirectoryOnly);
            if (PluginFiles.Length > 0)
                foreach (var PluginPath in PluginFiles)
                    File.Copy(PluginPath, GetPathDependenciesComponents + Path.GetFileName(PluginPath), true);

            PluginFiles = Directory.GetFiles(GetPathDownloadDependenciesPlugins, FilterFileName, SearchOption.TopDirectoryOnly);
            if (PluginFiles.Length > 0)
                foreach (var PluginPath in PluginFiles)
                    File.Copy(PluginPath, GetPathDependenciesPlugins + Path.GetFileName(PluginPath), true);
            #endregion

            #region 插件加载
            //PluginFiles = Directory.GetFiles(GetPathDependenciesComponents, FilterFileName, SearchOption.TopDirectoryOnly);
            //if (PluginFiles.Length > 0)
            //{
            //    var _PluginFiles = PluginFiles.ToList().OrderByDescending(o => o);
            //    foreach (var PluginPath in _PluginFiles)
            //        AssemblyLoadPlugin(PluginPath);
            //}

            PluginFiles = Directory.GetFiles(GetPathDependenciesPlugins, FilterFileName, SearchOption.TopDirectoryOnly);
            if (PluginFiles.Length > 0)
            {
                var _PluginFiles = PluginFiles.ToList().OrderBy(o => o);
                foreach (var PluginPath in _PluginFiles)
                    AssemblyLoadPlugin(PluginPath);
            }
            #endregion
        }

        #region Plugins 更新
        /// <summary>
        /// 检查更新
        /// </summary>
        private bool CheckUpdate()
        {
            bool flagUpdate = false;

            //检查依赖组件

            //检查依赖插件 

            return flagUpdate;
        }

        /// <summary>
        /// 下载插件
        /// </summary>
        private void Download()
        {

        }
        #endregion

        private void AssemblyLoadPlugin(string PluginPath)
        {
            try
            {
                //加载程序集
                Assembly assembly = Assembly.LoadFile(PluginPath);
                var types = assembly.GetTypes();
            }
            catch (Exception ex)
            {
                throw;
            }


        }



    }
}
