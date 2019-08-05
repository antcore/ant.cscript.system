using System;

namespace ant.csscript.run.core
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
        }
    }
}
