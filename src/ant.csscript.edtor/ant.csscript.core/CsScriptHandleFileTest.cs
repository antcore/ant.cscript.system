﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Xml;
using System.Xml.Linq;
using System.Drawing;

using ant.csscript.core;
using ant.csscript.core.domain;

namespace CsScript
{ 
    public class CsScriptHandleFileTest : ICsScriptHandleFile
    {
        public void Run()
        {
            string txt = Txt_FileHandle.Instance.LoadFile(FileInfo.FilePathHandle);
            ResultData.FileText = txt;
            //ResultData.Data = new ScriptResults<SampleData>()
            //{
            //    Data = new List<SampleData>()
            //    {
            //        new SampleData
            //        {
            //              ItemName = ""1"",
            //              PramaName = ""2"",
            //              PramaValue = ""3"",
            //              SampleNo = ""4""
            //         }
            //    }
            //};
            //运行输出
            RunLog.AppendLine("******** 运行结束 ********");
        }
        public StringBuilder RunLog { get; set; }
        #region PROPS
        /// <summary>
        /// 需要处理文件信息
        /// </summary>
        public FileInfoWaitHandle FileInfo { get; set; } 
        public ScriptResults<SampleData> ResultData { get; set; }
        #endregion
        public void Init(FileInfoWaitHandle fileInfo)
        {
            FileInfo = fileInfo;
            ResultData = new ScriptResults<SampleData>();
            ResultData.Data = new List<SampleData>();
            RunLog = new StringBuilder();
        }
    }
}