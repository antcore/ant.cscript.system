using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using iTextSharp.text.pdf.parser;
using iTextSharp.text.pdf;
using System.Drawing;

namespace daq.lib.Files
{

    public class pdftest
    {

        public void test获取文字坐标()
        {
            PdfReader readerTemp = new PdfReader(@"D:\_Number position.pdf");

            LocationTextExtractionStrategyEx pz = new LocationTextExtractionStrategyEx();

            iTextSharp.text.pdf.parser.PdfReaderContentParser p = new iTextSharp.text.pdf.parser.PdfReaderContentParser(readerTemp);
            p.ProcessContent<LocationTextExtractionStrategyEx>(1, pz);

            Console.WriteLine(pz.GetResultantText());//文字坐标信息等
            Console.ReadLine();
        }
    }

    public partial class PDFHelper_old
    {
        private static PDFHelper_old instance = null;

        public static PDFHelper_old Instance
        {
            get
            {
                if (instance == null)
                    instance = new PDFHelper_old();
                return PDFHelper_old.instance;
            }
        }

        private string fileName = "";

        public string FileName
        {
            get
            {
                if (string.IsNullOrEmpty(fileName))
                    throw new Exception("请指定文件信息");
                return fileName;
            }
            set { fileName = value; }
        }

        private int numOfPage = 0;

        public int NumOfPage
        {
            get { return numOfPage; }
            set { numOfPage = value; }
        }

        iTextSharp.text.pdf.PdfReader pdfReader = null;

        private Dictionary<int, List<LocationTextExtractionStrategyEx.TextChunk>> pdfCharRegionInfo = new Dictionary<int, List<LocationTextExtractionStrategyEx.TextChunk>>();

        /// <summary>
        /// 检索字符详细信息
        /// </summary>
        public void GetContentRegionInfo()
        {
            //加载文件
            pdfReader = new PdfReader(FileName);
            pdfCharRegionInfo.Clear();

            //PdfReader reader = new PdfReader(ofd.FileName);

            try
            {
                PdfReaderContentParser parser = new PdfReaderContentParser(pdfReader);

                Aspose.Pdf.Document doc = new Aspose.Pdf.Document(FileName);

                string reg = "[\\s\\S]*.";

                Aspose.Pdf.Text.TextFragmentAbsorber textFragmentAbsorber = new Aspose.Pdf.Text.TextFragmentAbsorber(reg);

                Aspose.Pdf.Text.TextOptions.TextSearchOptions textSearchOptions = new Aspose.Pdf.Text.TextOptions.TextSearchOptions(true);
                textFragmentAbsorber.TextSearchOptions = textSearchOptions;

                doc.Pages.Accept(textFragmentAbsorber);

                Aspose.Pdf.Text.TextFragmentCollection textFragmentCollection = textFragmentAbsorber.TextFragments;
                foreach (Aspose.Pdf.Text.TextFragment textFragment in textFragmentCollection)
                {
                    foreach (Aspose.Pdf.Text.TextSegment textSegment in textFragment.Segments)
                    {
                        var rec = textSegment.Rectangle;
                    }
                    //update text and other properties
                    //textFragment.Text = "TEXT";
                    //textFragment.TextState.Font = Aspose.Pdf.Text.FontRepository.FindFont("Arial");
                    //textFragment.TextState.FontSize = 22;
                    //textFragment.TextState.ForegroundColor = Aspose.Pdf.Color.FromRgb(System.Drawing.Color.Blue);
                    //textFragment.TextState.BackgroundColor = Aspose.Pdf.Color.FromRgb(System.Drawing.Color.Green);
                }
                //doc.Save();


                //var en = doc.Pages[1].Contents.GetEnumerator();
                //while (en.MoveNext())
                //{
                //    var value = en.Current as Aspose.Pdf.Operator;

                //    var tt = value.GetType();
                //}

                //// get particular form field from document
                //Aspose.Pdf.InteractiveFeatures.Forms.Field field = doc.Form["textField"] as Aspose.Pdf.InteractiveFeatures.Forms.Field;
                //// create font object
                //Aspose.Pdf.Text.Font font = Aspose.Pdf.Text.FontRepository.FindFont("Arial");
                //// set the font information for form field
                //field.DefaultAppearance = new Aspose.Pdf.InteractiveFeatures.DefaultAppearance(font, 10, System.Drawing.Color.Black);

                //ITextExtractionStrategy strategy;
                //strategy = parser.ProcessContent<SimpleTextExtractionStrategy>(1, new SimpleTextExtractionStrategy());
                ////将文本内容赋值给一个富文本框
                //string sss = strategy.GetResultantText();

            }
            catch (Exception)
            {
            }


            iTextSharp.text.pdf.parser.PdfReaderContentParser p = new iTextSharp.text.pdf.parser.PdfReaderContentParser(pdfReader);
            NumOfPage = pdfReader.NumberOfPages;
            for (int i = 1; i <= NumOfPage; i++)
            {
                //检索字符坐标信息
                LocationTextExtractionStrategyEx pz = new LocationTextExtractionStrategyEx();

                p.ProcessContent<LocationTextExtractionStrategyEx>(i, pz);

                pdfCharRegionInfo.Add(i, pz.LocationResult);
            }

            //关闭文件
            pdfReader.Close();
        }

        /// <summary>
        /// 获取页面文字坐标信息
        /// </summary>
        /// <param name="pageNum"></param>
        /// <returns></returns>
        public List<LocationTextExtractionStrategyEx.TextChunk> GetPageCharsPositionInfo(int pageNum)
        {
            if (pdfCharRegionInfo.ContainsKey(pageNum))
                return pdfCharRegionInfo[pageNum];
            else
                return null;
        }

        /// <summary>
        /// 获取搜索到的结果
        /// </summary>
        /// <returns></returns>
        public SearchResult GetSearchResult()
        {
            //PDFSearchResult_iTextSharp result = new PDFSearchResult_iTextSharp();
            return search_result;
        }

        public bool FindAreaFromDoubleString(int pageNum, string leftTopStr, int leftTopOffsetX, int leftTopOffsetY, string rightBottomStr, int rightBottomOffsetX, int rightBottomOffsetY, out List<RectangleF> recs)
        {
            recs = new List<RectangleF>();

            if (string.IsNullOrWhiteSpace(leftTopStr))
                return false;

            search_queue.Clear();
            search_region_queue.Clear();

            currentPageSize = pdfReader.GetPageSize(pageNum);
            var items = pdfCharRegionInfo[pageNum];
            bool result = findText(leftTopStr, items, 0, false);
            bool result2 = false;

            if (result)
            {
                //记录第一次查找到的字符
                var res = GetSearchResult();
                var rec = res.GetArea();


                result2 = findText(rightBottomStr, items, 0, false);
                if (!result2)
                {
                    rec.Width = currentPageSize.Width - rec.X;
                    rec.Height = currentPageSize.Height - rec.Y;
                }
                else
                {
                    res = GetSearchResult();
                    var rec2 = res.GetArea();

                }
            }

            return false;
        }


        public bool FindFirst(string text, int beginPage, bool up, int endPage = -1)
        {
            int _numOfPage = numOfPage;
            if (endPage > numOfPage)
                endPage = numOfPage;
            if (endPage >= beginPage && endPage > 0)
                _numOfPage = endPage;

            for (int i = beginPage; i <= _numOfPage; i++)
            {
                search_queue.Clear();
                search_region_queue.Clear();
                searchPageIndex = beginPage;
                currentPageSize = pdfReader.GetPageSize(i);
                var items = pdfCharRegionInfo[i];

                var result = findText(text, items, 0, up);
                if (result)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 查找上一字符串
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool FindPrevious(string text, int? nSearchBeginIndexInPage = null, int? beginPage = null, int? endPage = null)
        {
            if (beginPage == null)
                beginPage = searchPageIndex;
            if (endPage == null)
                endPage = numOfPage;

            if (nSearchBeginIndexInPage == null)
                nSearchBeginIndexInPage = searchBeginIndexInPage;

            for (int i = beginPage.Value; i <= endPage.Value; i++)
            {
                search_queue.Clear();
                search_region_queue.Clear();
                searchPageIndex = beginPage.Value;
                currentPageSize = pdfReader.GetPageSize(i);
                var items = pdfCharRegionInfo[i];
                var result = findText(text, items, nSearchBeginIndexInPage.Value, true);
                if (result)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 查找下个字符串
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool FindNext(string text, int? nSearchBeginIndexInPage = null, int? beginPage = null, int? endPage = null)
        {
            if (beginPage == null)
                beginPage = searchPageIndex;
            if (endPage == null)
                endPage = numOfPage;

            if (nSearchBeginIndexInPage == null)
                nSearchBeginIndexInPage = searchBeginIndexInPage;

            //var beginPage = searchPageIndex;
            for (int i = beginPage.Value; i <= endPage.Value; i++)
            {
                search_queue.Clear();
                search_region_queue.Clear();
                searchPageIndex = beginPage.Value;
                currentPageSize = pdfReader.GetPageSize(i);
                var items = pdfCharRegionInfo[i];
                var result = findText(text, items, nSearchBeginIndexInPage.Value, false);
                if (result)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 按指定区域查找文字
        /// </summary>
        /// <param name="pageNum"></param>
        /// <param name="area"></param>
        /// <returns></returns>
        public string FindInArea(int pageNum, RectangleF area)
        {
            string str = string.Empty;

            if (pdfCharRegionInfo.ContainsKey(pageNum))
            {
                Region reg = new Region(area);
                var items = pdfCharRegionInfo[pageNum];

                currentPageSize = pdfReader.GetPageSize(pageNum);

                StringBuilder tempMsg = new StringBuilder();

                for (int i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    var rec = item.GetArea();

                    rec.Y = currentPageSize.Height - rec.Y;

                    tempMsg.AppendLine(string.Format("{0}->{1}", item.Text, rec.ToString()));

                    var bl = reg.IsVisible(rec);
                    if (bl) str += item.Text;
                }
                string ssss = tempMsg.ToString();
            }

            return str;
        }

        SearchResult search_result = null;
        Queue<string> search_queue = new Queue<string>();
        Queue<LocationTextExtractionStrategyEx.TextChunk> search_region_queue = new Queue<LocationTextExtractionStrategyEx.TextChunk>();

        List<string> search_list = new List<string>();
        List<LocationTextExtractionStrategyEx.TextChunk> search_region_list = new List<LocationTextExtractionStrategyEx.TextChunk>();

        int searchBeginIndexInPage = 0;//字符搜索起始地址
        /// <summary>
        /// 字符搜索起始地址
        /// </summary>
        public int SearchBeginIndexInPage
        {
            get { return searchBeginIndexInPage; }
            set { searchBeginIndexInPage = value; }
        }
        int searchPageIndex = 1;

        iTextSharp.text.Rectangle currentPageSize = null;


        private bool findText(string text, List<LocationTextExtractionStrategyEx.TextChunk> charRegionInfo, int index, bool up)
        {
            if (index < 0) return false;
            if (index >= charRegionInfo.Count) return false;

            string tempStr = string.Empty;
            search_list.Clear();
            search_region_list.Clear();


            for (; up ? (index >= 0) : (index < charRegionInfo.Count); index += up ? (-1) : (1))
            {
                string currentChar = charRegionInfo[index].Text;


                search_list.Insert(up ? 0 : (search_list.Count == 0 ? 0 : (search_list.Count)), currentChar);
                search_region_list.Insert(up ? 0 : (search_region_list.Count == 0 ? 0 : (search_region_list.Count)), charRegionInfo[index]);

                searchBeginIndexInPage = index;

                //当前字符是否已经满足要求
                if (currentChar.Equals(text) || currentChar.Contains(text))//包含或者等于
                {
                    //精准匹配
                    //去掉最早的一个元素 是否还匹配                    
                    search_result = new SearchResult(new List<LocationTextExtractionStrategyEx.TextChunk>() { charRegionInfo[index] }, currentPageSize);
                    search_result.PageIndex = searchPageIndex;
                    return true;
                }



                tempStr = string.Empty;
                for (int i = 0; i <= search_list.Count - 1; i++)
                {
                    tempStr += search_list[i];

                    if (tempStr.Length < text.Length) continue;

                    if (tempStr.Equals(text) || tempStr.Contains(text))//包含或者等于
                    {
                        //if (i - 1 >= 0)
                        //{
                        //    search_list.RemoveRange(0, i - 1);
                        //    search_region_list.RemoveRange(0, i - 1);
                        //}

                        //精准匹配
                        //去掉最早的一个元素 是否还匹配                         

                        search_result = new SearchResult(search_region_list, currentPageSize);
                        search_result.PageIndex = searchPageIndex;
                        return true;
                    }
                }

                if (search_region_list.Count > text.Length)
                {
                    search_list.RemoveRange(0, search_region_list.Count - text.Length);
                    search_region_list.RemoveRange(0, search_region_list.Count - text.Length);
                }

            }

            return false;

            //search_list.Clear();
            //search_region_list.Clear();

            //for (int i = 0; i < charRegionInfo.Count; i++)
            //{
            //    string currentChar = charRegionInfo[i].Text;

            //    search_list.Insert(up ? 0 : (search_list.Count == 0 ? 0 : (search_list.Count - 1)), currentChar);
            //    search_region_list.Insert(up ? 0 : (search_region_list.Count == 0 ? 0 : (search_region_list.Count - 1)), charRegionInfo[index]);
            //}



            //if (up)
            //{
            //    for (int i = search_region_list.Count - 1; i >= 0; i--)
            //    {
            //        tempStr += search_region_list[i];

            //        if (tempStr.Length < text.Length) continue;

            //        if (tempStr.ToLower().Equals(text.ToLower()) || tempStr.ToLower().Contains(text.ToLower()))//包含或者等于
            //        {
            //            if (i - 1 >= 0)
            //            {
            //                search_list.RemoveRange(0, i - 1);
            //                search_region_list.RemoveRange(0, i - 1);
            //            }

            //            search_result = new SearchResult(search_region_list, currentPageSize);
            //            search_result.PageIndex = searchPageIndex;

            //            searchBeginIndexInPage = i;
            //            return true;
            //        }
            //    }
            //}
            //else
            //{
            //    for (int i = search_list.Count - 1; i >= 0; i--)
            //    {
            //        tempStr += search_list[i];

            //        if (tempStr.Length < text.Length) continue;

            //        if (tempStr.ToLower().Equals(text.ToLower()) || tempStr.ToLower().Contains(text.ToLower()))//包含或者等于
            //        {
            //            if (i - 1 >= 0)
            //            {
            //                search_list.RemoveRange(0, i - 1);
            //                search_region_list.RemoveRange(0, i - 1);
            //            }

            //            search_result = new SearchResult(search_region_list, currentPageSize);
            //            search_result.PageIndex = searchPageIndex;

            //            searchBeginIndexInPage = i;
            //            return true;
            //        }
            //    }
            //}

            //return false;
            //index = up ? --index : ++index;
            //searchBeginIndexInPage = index;
            //return findText(text, charRegionInfo, index, up);
        }

        /// <summary>
        /// 查找字符
        /// </summary>
        /// <param name="text"></param>
        /// <param name="charRegionInfo"></param>
        /// <param name="index"></param>
        /// <param name="up"></param>
        /// <returns></returns>
        private bool findTextB(string text, List<LocationTextExtractionStrategyEx.TextChunk> charRegionInfo, int index, bool up)
        {
            if (index < 0) return false;
            if (index >= charRegionInfo.Count) return false;

            string currentChar = charRegionInfo[index].Text;


            search_list.Insert(up ? 0 : (search_list.Count == 0 ? 0 : (search_list.Count - 1)), currentChar);
            search_region_list.Insert(up ? 0 : (search_region_list.Count == 0 ? 0 : (search_region_list.Count - 1)), charRegionInfo[index]);

            string tempStr = string.Empty;
            for (int i = search_list.Count - 1; i >= 0; i--)
            {
                tempStr += search_list[i];

                if (tempStr.Length < text.Length) continue;

                if (tempStr.ToLower().Equals(text.ToLower()) || tempStr.ToLower().Contains(text.ToLower()))//包含或者等于
                {
                    if (i - 1 >= 0)
                    {
                        search_list.RemoveRange(0, i - 1);
                        search_region_list.RemoveRange(0, i - 1);
                    }

                    search_result = new SearchResult(search_region_list, currentPageSize);
                    search_result.PageIndex = searchPageIndex;
                    return true;
                }
            }

            index = up ? --index : ++index;
            searchBeginIndexInPage = index;
            return findText(text, charRegionInfo, index, up);



            //search_queue.Enqueue(charRegionInfo[index].Text);
            //search_region_queue.Enqueue(charRegionInfo[index]);

            //if (search_queue.Count > text.Length)
            //    search_queue.Dequeue();
            //if (search_region_queue.Count > text.Length)
            //    search_region_queue.Dequeue();

            ////验证所搜到的字符是否为目标字符
            //string currentStr = string.Empty;
            //if(up)
            //{               

            //    var items= new List<string>();
            //    items.AddRange(search_queue.ToArray());
            //    items.Reverse();
            //    currentStr = string.Join("",items.ToArray());
            //}
            //else
            //{
            //    currentStr = string.Join("", search_queue.ToArray());
            //}


            //if(currentStr.ToLower().Equals(text.ToLower())||currentStr.Contains(text))
            //{
            //    //搜索到到的结果信息
            //    List<LocationTextExtractionStrategyEx.TextChunk> items = new List<LocationTextExtractionStrategyEx.TextChunk>();
            //    if(up)
            //    {
            //        while (search_region_queue.Count>0)
            //        items.Insert(0,search_region_queue.Dequeue());
            //    }
            //    else
            //    {
            //        while (search_region_queue.Count > 0)
            //            items.Add(search_region_queue.Dequeue());
            //    }

            //    search_result = new SearchResult(items,currentPageSize);
            //    search_result.PageIndex = searchPageIndex;
            //    return true;
            //}
            //else
            //{
            //    index = up ? --index : ++index;
            //    searchBeginIndexInPage = index;
            //    return findText(text,charRegionInfo,index,up);
            //}
        }
    }

    public class SearchResult
    {
        List<LocationTextExtractionStrategyEx.TextChunk> items = new List<LocationTextExtractionStrategyEx.TextChunk>();
        iTextSharp.text.Rectangle pageSize = null;
        public SearchResult(List<LocationTextExtractionStrategyEx.TextChunk> items, iTextSharp.text.Rectangle pageSize)
        {
            this.items = items;
            this.pageSize = pageSize;
        }

        /// <summary>
        /// 当前搜索的字符
        /// </summary>
        public string Text
        {
            get
            {
                string txt = string.Empty;
                items.ForEach(item => txt += item.Text);
                return txt;
            }
        }
        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageIndex { get; set; }

        public List<Rectangle> PosInfo
        {
            get
            {
                List<Rectangle> recs = new List<Rectangle>();
                foreach (var item in items)
                {
                    Rectangle rec = new Rectangle();

                    var vv = item.AscentLine.GetStartPoint().ToString();
                    string[] point = vv.ToString().Split(',');
                    int x = (int)float.Parse(point[0]);
                    int y = (int)(pageSize.Height - float.Parse(point[1]));
                    rec.X = x;
                    rec.Y = y;

                    var nn = item.DecentLine.GetEndPoint().ToString();
                    string[] point2 = nn.ToString().Split(',');
                    int x2 = (int)float.Parse(point2[0]);
                    int y2 = (int)(pageSize.Height - float.Parse(point2[1]));

                    rec.Width = x2 - x;
                    rec.Height = y2 - y;

                    recs.Add(rec);
                }
                return recs;
            }
        }

        /// <summary>
        /// 获取当前搜索结果的区域
        /// </summary>
        /// <returns></returns>
        public RectangleF GetArea()
        {
            RectangleF area = new RectangleF();
            bool isFirst = true;
            foreach (var item in items)
            {
                Rectangle rec = new Rectangle();

                var vv = item.AscentLine.GetStartPoint().ToString();
                string[] point = vv.ToString().Split(',');
                int x = (int)float.Parse(point[0]);
                int y = (int)(pageSize.Height - float.Parse(point[1]));
                rec.X = x;
                rec.Y = y;

                var nn = item.DecentLine.GetEndPoint().ToString();
                string[] point2 = nn.ToString().Split(',');
                int x2 = (int)float.Parse(point2[0]);
                int y2 = (int)(pageSize.Height - float.Parse(point2[1]));

                rec.Width = x2 - x;
                rec.Height = y2 - y;

                if (isFirst)
                {
                    isFirst = false;
                    area.X = rec.X;
                    area.Y = rec.Y;
                    area.Width = rec.Width;
                    area.Height = rec.Height;
                }
                else
                {
                    if (rec.X < area.X) area.X = rec.X;
                    if (rec.Y < area.Y) area.Y = rec.Y;
                    if (rec.Right > area.Right) area.Width = rec.Right - area.X;
                    if (rec.Bottom > area.Bottom) area.Height = rec.Bottom - area.Y;
                }
            }

            return area;
        }
    }


    public class PDFSearchResult_iTextSharp
    {
        private int _pageNumber;
        private Rectangle _position;

        public PDFSearchResult_iTextSharp(int page, int left, int top, int right, int bottom)
        {
            this._position = new Rectangle(left, top, right - left, bottom - top);
            this._pageNumber = page;
        }

        public int Page
        {
            get
            {
                return this._pageNumber;
            }
        }

        public Rectangle Position
        {
            get
            {
                return this._position;
            }
        }
    }



    /// <summary>
    /// Taken from http://www.java-frameworks.com/java/itext/com/itextpdf/text/pdf/parser/LocationTextExtractionStrategy.java.html
    /// </summary>
    public class LocationTextExtractionStrategyEx : LocationTextExtractionStrategy
    {
        private List<TextChunk> m_locationResult = new List<TextChunk>();
        private List<TextInfo> m_TextLocationInfo = new List<TextInfo>();
        public List<TextChunk> LocationResult
        {
            get { return m_locationResult; }
        }
        public List<TextInfo> TextLocationInfo
        {
            get { return m_TextLocationInfo; }
        }

        /// <summary>
        /// Creates a new LocationTextExtracationStrategyEx
        /// </summary>
        public LocationTextExtractionStrategyEx()
        {
        }

        /// <summary>
        /// Returns the result so far
        /// </summary>
        /// <returns>a String with the resulting text</returns>
        public override String GetResultantText()
        {
            m_locationResult.Sort();

            StringBuilder sb = new StringBuilder();
            TextChunk lastChunk = null;
            TextInfo lastTextInfo = null;
            foreach (TextChunk chunk in m_locationResult)
            {
                if (lastChunk == null)
                {
                    sb.Append(chunk.Text);
                    lastTextInfo = new TextInfo(chunk);
                    m_TextLocationInfo.Add(lastTextInfo);
                }
                else
                {
                    if (chunk.sameLine(lastChunk))
                    {
                        float dist = chunk.distanceFromEndOf(lastChunk);

                        if (dist < -chunk.CharSpaceWidth)
                        {
                            sb.Append(' ');
                            lastTextInfo.addSpace();
                        }
                        //append a space if the trailing char of the prev string wasn't a space && the 1st char of the current string isn't a space
                        else if (dist > chunk.CharSpaceWidth / 2.0f && chunk.Text[0] != ' ' && lastChunk.Text[lastChunk.Text.Length - 1] != ' ')
                        {
                            sb.Append(' ');
                            lastTextInfo.addSpace();
                        }
                        sb.Append(chunk.Text);
                        lastTextInfo.appendText(chunk);
                    }
                    else
                    {
                        sb.Append('\n');
                        sb.Append(chunk.Text);
                        lastTextInfo = new TextInfo(chunk);
                        m_TextLocationInfo.Add(lastTextInfo);
                    }
                }
                lastChunk = chunk;
            }
            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="renderInfo"></param>
        public override void RenderText(TextRenderInfo renderInfo)
        {
            LineSegment segment = renderInfo.GetBaseline();
            TextChunk location = new TextChunk(renderInfo.GetText(), segment.GetStartPoint(), segment.GetEndPoint(), renderInfo.GetSingleSpaceWidth(), renderInfo.GetAscentLine(), renderInfo.GetDescentLine());
            m_locationResult.Add(location);
        }
        /// <summary>
        /// 
        /// </summary>
        public class TextChunk : IComparable, ICloneable
        {
            string m_text;
            Vector m_startLocation;
            Vector m_endLocation;
            Vector m_orientationVector;
            int m_orientationMagnitude;
            int m_distPerpendicular;
            float m_distParallelStart;
            float m_distParallelEnd;
            float m_charSpaceWidth;

            public LineSegment AscentLine;
            public LineSegment DecentLine;

            public object Clone()
            {
                TextChunk copy = new TextChunk(m_text, m_startLocation, m_endLocation, m_charSpaceWidth, AscentLine, DecentLine);
                return copy;
            }

            public string Text
            {
                get { return m_text; }
                set { m_text = value; }
            }
            public float CharSpaceWidth
            {
                get { return m_charSpaceWidth; }
                set { m_charSpaceWidth = value; }
            }
            public Vector StartLocation
            {
                get { return m_startLocation; }
                set { m_startLocation = value; }
            }
            public Vector EndLocation
            {
                get { return m_endLocation; }
                set { m_endLocation = value; }
            }

            /// <summary>
            /// 获取当前文字区域
            /// </summary>
            /// <returns></returns>
            public RectangleF GetArea()
            {
                RectangleF rec = new RectangleF();
                var vv = AscentLine.GetStartPoint().ToString();
                string[] point = vv.ToString().Split(',');
                int x = (int)float.Parse(point[0]);
                //int y = (int)(pageSize.Height - float.Parse(point[1]));
                int y = (int)(float.Parse(point[1]));
                rec.X = x;
                rec.Y = y;

                var nn = DecentLine.GetEndPoint().ToString();
                string[] point2 = nn.ToString().Split(',');
                int x2 = (int)float.Parse(point2[0]);
                //int y2 = (int)(pageSize.Height - float.Parse(point2[1]));
                int y2 = (int)(float.Parse(point2[1]));

                rec.Width = x2 - x;
                rec.Height = System.Math.Abs(y2 - y);

                return rec;
            }

            /// <summary>
            /// Represents a chunk of text, it's orientation, and location relative to the orientation vector
            /// </summary>
            /// <param name="txt"></param>
            /// <param name="startLoc"></param>
            /// <param name="endLoc"></param>
            /// <param name="charSpaceWidth"></param>
            public TextChunk(string txt, Vector startLoc, Vector endLoc, float charSpaceWidth, LineSegment ascentLine, LineSegment decentLine)
            {
                m_text = txt;
                m_startLocation = startLoc;
                m_endLocation = endLoc;
                m_charSpaceWidth = charSpaceWidth;
                AscentLine = ascentLine;
                DecentLine = decentLine;

                m_orientationVector = m_endLocation.Subtract(m_startLocation).Normalize();
                m_orientationMagnitude = (int)(Math.Atan2(m_orientationVector[Vector.I2], m_orientationVector[Vector.I1]) * 1000);

                // see http://mathworld.wolfram.com/Point-LineDistance2-Dimensional.html
                // the two vectors we are crossing are in the same plane, so the result will be purely
                // in the z-axis (out of plane) direction, so we just take the I3 component of the result
                Vector origin = new Vector(0, 0, 1);
                m_distPerpendicular = (int)(m_startLocation.Subtract(origin)).Cross(m_orientationVector)[Vector.I3];

                m_distParallelStart = m_orientationVector.Dot(m_startLocation);
                m_distParallelEnd = m_orientationVector.Dot(m_endLocation);
            }

            /// <summary>
            /// true if this location is on the the same line as the other text chunk
            /// </summary>
            /// <param name="textChunkToCompare">the location to compare to</param>
            /// <returns>true if this location is on the the same line as the other</returns>
            public bool sameLine(TextChunk textChunkToCompare)
            {
                if (m_orientationMagnitude != textChunkToCompare.m_orientationMagnitude) return false;
                if (m_distPerpendicular != textChunkToCompare.m_distPerpendicular) return false;
                return true;
            }

            /// <summary>
            /// Computes the distance between the end of 'other' and the beginning of this chunk
            /// in the direction of this chunk's orientation vector.  Note that it's a bad idea
            /// to call this for chunks that aren't on the same line and orientation, but we don't
            /// explicitly check for that condition for performance reasons.
            /// </summary>
            /// <param name="other"></param>
            /// <returns>the number of spaces between the end of 'other' and the beginning of this chunk</returns>
            public float distanceFromEndOf(TextChunk other)
            {
                float distance = m_distParallelStart - other.m_distParallelEnd;
                return distance;
            }

            /// <summary>
            /// Compares based on orientation, perpendicular distance, then parallel distance
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public int CompareTo(object obj)
            {
                if (obj == null) throw new ArgumentException("Object is now a TextChunk");

                TextChunk rhs = obj as TextChunk;
                if (rhs != null)
                {
                    if (this == rhs) return 0;

                    int rslt;
                    rslt = m_orientationMagnitude - rhs.m_orientationMagnitude;
                    if (rslt != 0) return rslt;

                    rslt = m_distPerpendicular - rhs.m_distPerpendicular;
                    if (rslt != 0) return rslt;

                    // note: it's never safe to check floating point numbers for equality, and if two chunks
                    // are truly right on top of each other, which one comes first or second just doesn't matter
                    // so we arbitrarily choose this way.
                    rslt = m_distParallelStart < rhs.m_distParallelStart ? -1 : 1;

                    return rslt;
                }
                else
                {
                    throw new ArgumentException("Object is now a TextChunk");
                }
            }
        }

        public class TextInfo
        {
            public Vector TopLeft;
            public Vector BottomRight;
            private string m_Text;

            public string Text
            {
                get { return m_Text; }
            }

            /// <summary>
            /// Create a TextInfo.
            /// </summary>
            /// <param name="initialTextChunk"></param>
            public TextInfo(TextChunk initialTextChunk)
            {
                TopLeft = initialTextChunk.AscentLine.GetStartPoint();
                BottomRight = initialTextChunk.DecentLine.GetEndPoint();
                m_Text = initialTextChunk.Text;
            }

            /// <summary>
            /// Add more text to this TextInfo.
            /// </summary>
            /// <param name="additionalTextChunk"></param>
            public void appendText(TextChunk additionalTextChunk)
            {
                BottomRight = additionalTextChunk.DecentLine.GetEndPoint();
                m_Text += additionalTextChunk.Text;
            }

            /// <summary>
            /// Add a space to the TextInfo.  This will leave the endpoint out of sync with the text.
            /// The assumtion is that you will add more text after the space which will correct the endpoint.
            /// </summary>
            public void addSpace()
            {
                m_Text += ' ';
            }


        }
    }
}