using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Topshelf;

namespace ant.csscript.run
{
    class Program
    {
        static void Main(string[] args)
        {

#if (DEBUG)
            {
                Console.WriteLine("开始调试脚本解析服务");
                new ControlService().Start();
                Console.ReadKey();
            }
#endif

            //Windows 服务
            HostFactory.Run(x =>
            {
                x.Service<ControlService>(s =>
                { 
                    s.ConstructUsing(name => new ControlService());
                    s.WhenStarted(cs => cs.Start());
                    s.WhenStopped(cs => cs.Stop());
                });

                x.RunAsLocalSystem();

                x.StartAutomatically();//自动运行

                x.SetServiceName("AntCsScriptRun");
                x.SetDisplayName("AntCsScriptRun");
                x.SetDescription("AntCsScriptRun 脚本解析服务 ");
            });
        }
    }
}
