﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ant.plugin.app
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            PluginManager.Instance.Install();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
