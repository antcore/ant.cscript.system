using System;
using System.Collections.Generic;

namespace ant.csscript.core
{
    /// <summary>
    /// 字符串 扩展方法
    /// </summary>
    public static partial class StringExtends
    {


        //public static string _查找字符串_全文搜索忽略换行符(this string _原始字符串, string _起始字符串, string _结束字符串_可空 = "")
        //{
        //    return Find(_原始字符串, _起始字符串, _结束字符串_可空);
        //}

        /// <summary>
        /// 查找指定字符之间的字符,如果没有指定结束符,则取开符之后的所有字符.LW
        /// </summary>
        /// <param name="content">原始字符串</param>
        /// <param name="startStr">起始字符串</param>
        /// <param name="endStr">结束字符串,可为空</param>
        /// <returns></returns>
        public static string FindStr(this string content, string startStr, string endStr = "")
        {
            return Find(content, startStr, endStr);
        }

        /// <summary>
        /// 跨行查找指定字符之间的字符
        /// 如果没有找到结束符号 则取开始符号之后的所有字符
        /// </summary>
        /// <param name="content">原始字符串</param>
        /// <param name="startStr">起始字符串</param>
        /// <param name="endStr">结束字符串</param>
        /// <returns></returns>
        public static string Find(this string content, string startStr, string endStr = "")
        {
            string result = string.Empty;
            int startIndex = content.IndexOf(startStr);
            if (startIndex >= 0)
            {
                if (string.IsNullOrEmpty(endStr))
                {
                    result = content.Substring(startIndex + startStr.Length);
                }
                else
                {
                    int endIndex = content.IndexOf(endStr, startIndex + startStr.Length);
                    if (endIndex >= 0)
                    {
                        result = content.Substring(startIndex + startStr.Length, endIndex - startIndex - startStr.Length);
                    }
                    else
                    {
                        result = content.Substring(startIndex + startStr.Length);
                    }
                }
            }

            return result;
        }

        public static string Replace(this string content, string startStr, string newStr, string endStr = "")
        {
            string result = string.Empty;
            int startIndex = content.IndexOf(startStr);
            if (startIndex >= 0)
            {
                if (string.IsNullOrEmpty(endStr))
                {
                    result = content.Remove(startIndex + startStr.Length);
                    result += newStr;
                }
                else
                {
                    int endIndex = content.IndexOf(endStr);
                    if (endIndex >= 0)
                    {
                        result = content.Remove(startIndex + startStr.Length, endIndex - startIndex - startStr.Length);
                        result.Insert(startIndex + startStr.Length, newStr);
                    }
                    else
                    {
                        result = content.Remove(startIndex + startStr.Length);
                        result += newStr;
                    }
                }
            }

            return result;
        }

        //public static List<string> _查找字符串_按行查找(this string _原始字符串, string _起始字符串, string _结束字符串_可空 = "")
        //{
        //    return _原始字符串.FindByLine(_起始字符串, _结束字符串_可空);
        //}

        /// <summary>
        /// 逐行查找指定字符之间的字符,如果没有指定结束符，则取开始符之后的所有字符. LW
        /// </summary>
        /// <param name="content">数据源</param>
        /// <param name="startStr">起始字符</param>
        /// <param name="endStr">结束字符</param>
        /// <returns></returns>
        public static List<string> FindStrByLine(this string content, string startStr, string endStr = "")
        {
            return FindByLine(content, startStr, endStr);
        }

        /// <summary>
        /// 逐行查找指定字符之间的字符
        /// 如果没有找到结束符号 则取开始符号之后的所有字符
        /// </summary>
        /// <param name="content">数据源</param>
        /// <param name="startStr">起始字符</param>
        /// <param name="endStr">结束字符</param>
        /// <returns></returns>
        public static List<string> FindByLine(this string content, string startStr, string endStr = "")
        {
            List<string> result = new List<string>(1);
            string[] rows = content.Replace("\r", "").Split('\n');
            foreach (string value in rows)
            {
                if (!value.Contains(startStr)) continue;
                int startIndex = value.IndexOf(startStr);

                if (string.IsNullOrEmpty(endStr))
                {
                    result.Add(value.Substring(startIndex + startStr.Length));
                    continue;
                }

                int endIndex = value.IndexOf(endStr, startIndex + startStr.Length);
                if (endIndex > startIndex)//结束位置正确
                {
                    result.Add(value.Substring(startIndex + startStr.Length, endIndex - startIndex - startStr.Length));
                }
                else
                {
                    result.Add(value.Substring(startIndex + startStr.Length));
                }

                break;
            }

            return result;
        }

        //public static string _查找字符串_按行查找_返回第一个匹配项(this string _原始字符串, string _起始字符串, string _结束字符串_可空 = "")
        //{
        //    return _原始字符串.FindByLineFirst(_起始字符串, _结束字符串_可空);
        //}


        /// <summary>
        /// 逐行查找指定字符之间的字符的第一个匹配的字符,如果没有指定结束符 则取开始符之后的所有字符,LW
        /// </summary>
        /// <param name="content">数据源</param>
        /// <param name="startStr">起始字符</param>
        /// <param name="endStr">结束字符</param>
        /// <returns></returns>
        public static string FindStrByLineFirst(this string content, string startStr, string endStr = "")
        {
            return FindByLineFirst(content, startStr, endStr);
        }

        /// <summary>
        /// 逐行查找指定字符之间的字符 第一个匹配的字符
        /// 如果没有找到结束符号 则取开始符号之后的所有字符
        /// </summary>
        /// <param name="content">数据源</param>
        /// <param name="startStr">起始字符</param>
        /// <param name="endStr">结束字符</param>
        /// <returns></returns>
        public static string FindByLineFirst(this string content, string startStr, string endStr = "")
        {
            var results = content.FindByLine(startStr, endStr);
            return results.Count > 0 ? results[0] : string.Empty;
        }

        //public static string _查找字符串_按行查找_返回最后一个匹配项(this string _原始字符串, string _起始字符串, string _结束字符串_可空 = "")
        //{
        //    return _原始字符串.FindByLineLast(_起始字符串, _结束字符串_可空);
        //}

        /// <summary>
        /// 逐行查找指定字符之间的最后一个匹配的字符 如果没有指定结束符 则取开始符之后的所有字符。LW
        /// </summary>
        /// <param name="content">数据源</param>
        /// <param name="startStr">起始字符</param>
        /// <param name="endStr">结束字符</param>
        /// <returns></returns>
        public static string FindStrByLineLast(this string content, string startStr, string endStr = "")
        {
            return FindByLineLast(content, startStr, endStr);
        }
        /// <summary>
        /// 逐行查找指定字符之间的字符 最后一个匹配的字符
        /// 如果没有找到结束符号 则取开始符号之后的所有字符
        /// </summary>
        /// <param name="content">数据源</param>
        /// <param name="startStr">起始字符</param>
        /// <param name="endStr">结束字符</param>
        /// <returns></returns>
        public static string FindByLineLast(this string content, string startStr, string endStr = "")
        {
            var results = content.FindByLine(startStr, endStr);
            return results.Count > 0 ? results[results.Count - 1] : string.Empty;
        }

        //public static string _查找字符串_全文搜索忽略换行符(this string _原始字符串, int _起始位置, int? _结束位置_可空 = null)
        //{
        //    return _原始字符串.Find(_起始位置, _结束位置_可空);
        //}

        /// <summary>
        /// 跨行查找指定位置之间的字符，如果没有指定结束位置 则取开始位置之后的所有字符。LW
        /// </summary>
        /// <param name="content">原始字符串</param>
        /// <param name="startStr">起始字符串</param>
        /// <param name="endStr">结束字符串</param>
        /// <returns></returns>
        public static string FindStr(this string content, int startIndex, int? endIndex = null)
        {
            return Find(content, startIndex, endIndex);
        }
        /// <summary>
        /// 跨行查找指定位置之间的字符
        /// 如果没有找到结束符号 则取开始符号之后的所有字符
        /// </summary>
        /// <param name="content">原始字符串</param>
        /// <param name="startStr">起始字符串</param>
        /// <param name="endStr">结束字符串</param>
        /// <returns></returns>
        public static string Find(this string content, int startIndex, int? endIndex = null)
        {
            string result = string.Empty;
            if (startIndex < 0) startIndex = 0;

            if (content.Length > startIndex)
            {
                if (endIndex != null)
                {
                    if (endIndex > startIndex && endIndex < content.Length)
                    {
                        result = content.Substring(startIndex, (int)endIndex - startIndex);
                    }
                    else
                    {
                        result = content.Substring(startIndex);
                    }
                }
                else
                {
                    result = content.Substring(startIndex);
                }
            }
            return result;
        }

        //public static string _查找字符串_按行查找(this string _原始字符串,int _行号, int _起始位置, int? _结束位置_可空 = null)
        //{
        //    return _原始字符串.FindByLine(_行号,_起始位置, _结束位置_可空);
        //}
        /// <summary>
        /// 逐行查找指定位置之间的字符，如果没有指定结束位置 则取开始位置之后的所有字符。LW
        /// </summary>
        /// <param name="content">原始字符串</param>
        /// <param name="startStr">起始字符串</param>
        /// <param name="endStr">结束字符串</param>
        /// <returns></returns>
        public static string FindStrByLine(this string content, int rowIndex, int startIndex, int? endIndex = null)
        {
            return FindByLine(content, rowIndex, startIndex, endIndex);
        }
        /// <summary>
        /// 逐行查找指定位置之间的字符
        /// 如果没有找到结束符号 则取开始符号之后的所有字符
        /// </summary>
        /// <param name="content">数据源</param>
        /// <param name="startStr">起始字符</param>
        /// <param name="endStr">结束字符</param>
        /// <returns></returns>
        public static string FindByLine(this string content, int rowIndex, int startIndex, int? endIndex = null)
        {
            string result = string.Empty;

            string[] rows = content.Replace("\r", "").Split('\n');
            if (rows.Length > rowIndex && rowIndex >= 0)
            {
                string value = rows[rowIndex];
                result = value.Find(startIndex, endIndex);
            }

            return result;
        }


    }
}
