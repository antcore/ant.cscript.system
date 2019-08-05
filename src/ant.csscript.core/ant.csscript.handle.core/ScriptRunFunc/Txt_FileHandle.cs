using ant.csscript.handle.domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ant.csscript.handle.core
{
    /// <summary>
    /// 文本文件读取
    /// </summary>
    public class Txt_FileHandle
    {
        private static Txt_FileHandle instance = null;

        public static Txt_FileHandle Instance
        {
            get
            {
                if (null == instance)
                    instance = new Txt_FileHandle();
                return instance;
            }
        }

        public string LoadFile(string filePath)
        {
            return LoadFile(filePath, Encoding.Default);
        }

        public string LoadFile(string filePath, Encoding encoding)
        {
            return System.IO.File.ReadAllText(filePath, encoding);
        }

    }
}
