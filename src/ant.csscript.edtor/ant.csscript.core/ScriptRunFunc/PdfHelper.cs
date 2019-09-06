using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using iTextSharp.text.pdf.parser;
using iTextSharp.text.pdf;
using System.Drawing;

namespace daq.lib.Files
{
    public partial class PDFHelper
    {
        private static PDFHelper instance = null;

        public static PDFHelper Instance
        {
            get
            {
                if (instance == null)
                    instance = new PDFHelper();
                return PDFHelper.instance;
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

        List<Aspose.Pdf.Text.TextFragment> _SearchResults = new List<Aspose.Pdf.Text.TextFragment>();

        //查找文本
        /// <summary>
        /// 查找文本
        /// </summary>
        /// <param name="text">待搜索文本</param>
        /// <param name="beginPage">起始页</param>
        /// <param name="endPage">结束页</param>
        /// <param name="nMatchIndex">匹配到的第几个结果</param>
        /// <returns></returns>
        public bool FindText(string text, int? beginPage = null, int? endPage = null)
        {
            if (beginPage == null)
                beginPage = 1;
            if (endPage == null)
                endPage = numOfPage;

            using (Aspose.Pdf.Document doc = new Aspose.Pdf.Document(FileName))
            {
                string reg = text;
                //reg = "[\\s\\S]*.";//查找所有字符

                Aspose.Pdf.Text.TextFragmentAbsorber textFragmentAbsorber = new Aspose.Pdf.Text.TextFragmentAbsorber(reg);

                Aspose.Pdf.Text.TextOptions.TextSearchOptions textSearchOptions = new Aspose.Pdf.Text.TextOptions.TextSearchOptions(true);
                textFragmentAbsorber.TextSearchOptions = textSearchOptions;

                _SearchResults.Clear();
                for (int index = beginPage.Value; index <= endPage.Value; index++)
                {
                    doc.Pages[index].Accept(textFragmentAbsorber);

                    Aspose.Pdf.Text.TextFragmentCollection textFragmentCollection = textFragmentAbsorber.TextFragments;
                    foreach (Aspose.Pdf.Text.TextFragment textFragment in textFragmentCollection)
                    {
                        _SearchResults.Add(textFragment);
                        //foreach (Aspose.Pdf.Text.TextSegment textSegment in textFragment.Segments)
                        //{
                        //    var rec = textSegment.Rectangle;
                        //}
                    }
                }
                return _SearchResults.Count > 0;
            }
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <returns></returns>
        public List<Aspose.Pdf.Text.TextFragment> GetSearchResult()
        {
            return _SearchResults;
        }

        /// <summary>
        /// 清楚缓存
        /// </summary>
        public void ClearBuffer()
        {
            dicTextSeg.Clear();
            pageInfo = null;
        }


        Dictionary<int, Aspose.Pdf.ImagePlacementCollection> dicImg = new Dictionary<int, Aspose.Pdf.ImagePlacementCollection>();
        /// <summary>
        /// 获取所有图片
        /// </summary>
        /// <param name="pageNum"></param>
        /// <returns></returns>
        public void FindAllImage(int pageNum, Func<Aspose.Pdf.ImagePlacementCollection, bool> fuc)
        {
            var items = new List<Aspose.Pdf.ImagePlacement>();
            using (Aspose.Pdf.Document doc = new Aspose.Pdf.Document(FileName))
            {
                var imgAbsorber = new Aspose.Pdf.ImagePlacementAbsorber();
                doc.Pages[pageNum].Accept(imgAbsorber);
                var imgs = imgAbsorber.ImagePlacements;

                var op = new Aspose.Pdf.OperatorSelector(new Aspose.Pdf.Operator.ClosePathFillStroke());
                doc.Pages[1].Contents.Accept(op);

                //System.IO.MemoryStream ms = new System.IO.MemoryStream();
                //imgs[1].Save(ms);
                //ms.Close();
                fuc(imgs);
            }
        }

        Dictionary<int, List<Aspose.Pdf.Text.TextSegment>> dicTextSeg = new Dictionary<int, List<Aspose.Pdf.Text.TextSegment>>();
        /// <summary>
        /// 查找文本段落
        /// </summary>
        /// <param name="pageNum"></param>
        /// <returns></returns>
        public List<Aspose.Pdf.Text.TextSegment> FindAllTextSeg(int pageNum)
        {
            if (dicTextSeg.ContainsKey(pageNum)) return dicTextSeg[pageNum];

            var items = new List<Aspose.Pdf.Text.TextSegment>();

            dicTextSeg.Add(pageNum, items);

            using (Aspose.Pdf.Document doc = new Aspose.Pdf.Document(FileName))
            {
                string reg = string.Empty;
                reg = "[\\s\\S]*.";//查找所有字符

                Aspose.Pdf.Text.TextFragmentAbsorber textFragmentAbsorber = new Aspose.Pdf.Text.TextFragmentAbsorber(reg);

                Aspose.Pdf.Text.TextOptions.TextSearchOptions textSearchOptions = new Aspose.Pdf.Text.TextOptions.TextSearchOptions(true);
                textFragmentAbsorber.TextSearchOptions = textSearchOptions;

                doc.Pages[pageNum].Accept(textFragmentAbsorber);

                Aspose.Pdf.Text.TextFragmentCollection textFragmentCollection = textFragmentAbsorber.TextFragments;
                foreach (Aspose.Pdf.Text.TextFragment textFragment in textFragmentCollection)
                {
                    // _SearchResults.Add(textFragment);
                    foreach (Aspose.Pdf.Text.TextSegment textSegment in textFragment.Segments)
                    {
                        items.Add(textSegment);
                    }
                }

                //反序
                // items.Reverse();

                return items;
            }
        }

        Aspose.Pdf.PageInfo pageInfo = null;

        /// <summary>
        /// 获取页面信息
        /// </summary>
        /// <returns></returns>
        public Aspose.Pdf.PageInfo GetPageSize()
        {
            if (pageInfo == null)
            {
                using (Aspose.Pdf.Document doc = new Aspose.Pdf.Document(FileName))
                {
                    pageInfo = doc.PageInfo;

                    //FileName = @"E:\Mr'King\开发二部\项目实施\内蒙食药\PDF文件\PDF文件\点图.pdf";
                    //using (Aspose.Pdf.Document doc2 = new Aspose.Pdf.Document(FileName))
                    //{
                    //    pageInfo = doc2.PageInfo;
                    //}
                }
            }
            return pageInfo;
        }

        /// <summary>
        /// 按区域查找文本
        /// </summary>
        /// <param name="pageNum"></param>
        /// <param name="area"></param>
        /// <returns></returns>
        public string FindInArea(int pageNum, RectangleF area, double pageH = -1)
        {
            string retStr = string.Empty;
            var items = FindAllTextSeg(pageNum);

            var pageSize = GetPageSize();

            //string txt = string.Empty;
            //foreach (var item in items)
            //{
            //    txt += string.Format("{0} -> {1} \r\n", item.Text,new{ x = item.Rectangle.LLX,y= /*pageInfo.Height -*/ item.Rectangle.LLY,w=item.Rectangle.Width, h=item.Rectangle.Height});
            //}

            // Console.WriteLine(txt);

            if (items != null || items.Count > 0)
            {
                Region reg = new Region(area);

                float _refixY = 5;// 10;
                items.ForEach(m =>
                {
                    //坐标原点在左下方
                    RectangleF rec = new RectangleF((float)m.Rectangle.LLX, (float)(pageSize.Height - m.Rectangle.LLY - _refixY), (float)m.Rectangle.Width, (float)m.Rectangle.Height);
                    if (pageH > 0)
                    {
                        rec.Y = (float)(pageH - m.Rectangle.LLY - _refixY);
                    }
                    if (reg.IsVisible(rec))
                    {
                        retStr += m.Text;
                    }
                });
            }

            return retStr;
        }


        /// <summary>
        /// 按区域查找文本
        /// </summary>
        /// <param name="pageNum"></param>
        /// <param name="area"></param>
        /// <returns></returns>
        public string FindInAreaV2(int pageNum, RectangleF area)
        {
            using (var pdfReader = new PdfReader(FileName))
            {
                PdfReaderContentParser parser = new PdfReaderContentParser(pdfReader);
                Aspose.Pdf.Document doc = new Aspose.Pdf.Document(FileName);

                Aspose.Pdf.Text.TextAbsorber textAbsorber = new Aspose.Pdf.Text.TextAbsorber();
                textAbsorber.TextSearchOptions.LimitToPageBounds = true;
                textAbsorber.TextSearchOptions.Rectangle = new Aspose.Pdf.Rectangle(area.X, area.Y, area.Right, area.Bottom);

                doc.Pages[pageNum].Accept(textAbsorber);

                var sss = textAbsorber.Text;

                Aspose.Pdf.Text.TextFragmentAbsorber textFragmentAbsorber = new Aspose.Pdf.Text.TextFragmentAbsorber();
                Aspose.Pdf.Rectangle rect = new Aspose.Pdf.Rectangle(area.X, area.Y, area.Right, area.Bottom);
                Aspose.Pdf.Text.TextOptions.TextSearchOptions textSearchOptions = new Aspose.Pdf.Text.TextOptions.TextSearchOptions(rect);
                textSearchOptions.LimitToPageBounds = true;
                textFragmentAbsorber.TextSearchOptions = textSearchOptions;

                //_SearchResults.Clear();

                doc.Pages[pageNum].Accept(textFragmentAbsorber);

                Aspose.Pdf.Text.TextFragmentCollection textFragmentCollection = textFragmentAbsorber.TextFragments;

                string txt = string.Empty;
                foreach (Aspose.Pdf.Text.TextFragment textFragment in textFragmentCollection)
                {
                    txt += textFragment.Text;
                    //_SearchResults.Add(textFragment);
                }
                return txt;
            }

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

            #region drop code 

            //PdfReader reader = new PdfReader(ofd.FileName);

            //try
            //{
            //    PdfReaderContentParser parser = new PdfReaderContentParser(pdfReader);

            //    Aspose.Pdf.Document doc = new Aspose.Pdf.Document(FileName);

            //    string reg = "[\\s\\S]*.";

            //    Aspose.Pdf.Text.TextFragmentAbsorber textFragmentAbsorber = new Aspose.Pdf.Text.TextFragmentAbsorber(reg);

            //    Aspose.Pdf.Text.TextOptions.TextSearchOptions textSearchOptions = new Aspose.Pdf.Text.TextOptions.TextSearchOptions(true);
            //    textFragmentAbsorber.TextSearchOptions = textSearchOptions;

            //    doc.Pages.Accept(textFragmentAbsorber);

            //    Aspose.Pdf.Text.TextFragmentCollection textFragmentCollection = textFragmentAbsorber.TextFragments;
            //    foreach (Aspose.Pdf.Text.TextFragment textFragment in textFragmentCollection)
            //    {
            //        foreach (Aspose.Pdf.Text.TextSegment textSegment in textFragment.Segments)
            //        {
            //            var rec = textSegment.Rectangle;
            //        }
            //        //update text and other properties
            //        //textFragment.Text = "TEXT";
            //        //textFragment.TextState.Font = Aspose.Pdf.Text.FontRepository.FindFont("Arial");
            //        //textFragment.TextState.FontSize = 22;
            //        //textFragment.TextState.ForegroundColor = Aspose.Pdf.Color.FromRgb(System.Drawing.Color.Blue);
            //        //textFragment.TextState.BackgroundColor = Aspose.Pdf.Color.FromRgb(System.Drawing.Color.Green);
            //    }
            //    //doc.Save();


            //    //var en = doc.Pages[1].Contents.GetEnumerator();
            //    //while (en.MoveNext())
            //    //{
            //    //    var value = en.Current as Aspose.Pdf.Operator;

            //    //    var tt = value.GetType();
            //    //}

            //    //// get particular form field from document
            //    //Aspose.Pdf.InteractiveFeatures.Forms.Field field = doc.Form["textField"] as Aspose.Pdf.InteractiveFeatures.Forms.Field;
            //    //// create font object
            //    //Aspose.Pdf.Text.Font font = Aspose.Pdf.Text.FontRepository.FindFont("Arial");
            //    //// set the font information for form field
            //    //field.DefaultAppearance = new Aspose.Pdf.InteractiveFeatures.DefaultAppearance(font, 10, System.Drawing.Color.Black);

            //    //ITextExtractionStrategy strategy;
            //    //strategy = parser.ProcessContent<SimpleTextExtractionStrategy>(1, new SimpleTextExtractionStrategy());
            //    ////将文本内容赋值给一个富文本框
            //    //string sss = strategy.GetResultantText();

            //}
            //catch (Exception ex)
            //{
            //}
            #endregion

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
        /// 获取页面文本信息
        /// </summary>
        /// <param name="pageNum"></param>
        /// <returns></returns>
        public List<LocationTextExtractionStrategyEx.TextChunk> FindAllTextSeg_iTextSharp(int pageNum)
        {
            if (pdfCharRegionInfo.ContainsKey(pageNum)) return pdfCharRegionInfo[pageNum];

            List<LocationTextExtractionStrategyEx.TextChunk> items = null;
            pdfReader = new PdfReader(FileName);

            if (pdfReader.NumberOfPages < pageNum) return null;

            iTextSharp.text.pdf.parser.PdfReaderContentParser p = new iTextSharp.text.pdf.parser.PdfReaderContentParser(pdfReader);
            //检索字符坐标信息
            LocationTextExtractionStrategyEx pz = new LocationTextExtractionStrategyEx();
            p.ProcessContent<LocationTextExtractionStrategyEx>(pageNum, pz);
            //items = pz.LocationResult;
            items = (from m in pz.LocationResult orderby m.GetArea().Y descending, m.GetArea().X ascending select m).ToList();
            pdfCharRegionInfo.Add(pageNum, items);

            return items;
        }

        /// <summary>
        /// 读取所有文本内容
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public  string GetAllText_iTextSharp( String filename)
        {
                PdfReader reader = new PdfReader(filename); //读取pdf所使用的输出流 
                int PageNum = reader.NumberOfPages;//获得页数 

                String content = "";  //存放读取出的文档内容 
                for (int i = 1; i <= PageNum; i++)
                {
                    content += PdfTextExtractor.GetTextFromPage(reader, i);//读取第i页的文档内容 
                }
            return content;
        }


    public string FindInArea_iTextSharp(int pageNum, RectangleF area)
    {
        string retStr = string.Empty;
        var items = FindAllTextSeg_iTextSharp(pageNum);

        string txt = string.Empty;
        foreach (var item in items)
        {
            txt += string.Format("{0} -> {1} \r\n", item.Text, item.GetArea());
        }

        //Console.WriteLine(txt);

        if (items != null || items.Count > 0)
        {
            Region reg = new Region(area);
            var pageSize = GetPageSize();
            items.ForEach(m =>
            {
                    //float _refixY = 10;
                    var rec = m.GetArea();
                rec.Y = (float)pageInfo.Height - rec.Y;
                rec.Y = rec.Y = (float)pageSize.Height - rec.Y;
                if (reg.IsVisible(rec))
                {
                    retStr += m.Text;
                }
            });
        }

        return retStr;
    }

    #region ocr  图像识别


    #endregion


    #region 文件转换
    /// <summary>
    /// 获取pdf文件的svg
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public static List<string> GetSvgFromPdf(string file)
    {
        Aspose.Pdf.Document doc = new Aspose.Pdf.Document(file);
        Aspose.Pdf.SvgSaveOptions saveOptions = new Aspose.Pdf.SvgSaveOptions();
        // Do not compress SVG image to Zip archive
        saveOptions.CompressOutputToZipArchive = false;

        // Output file name
        string key = Guid.NewGuid().ToString("n");
        string path = AppDomain.CurrentDomain.BaseDirectory + "\\temp\\";
        if (!System.IO.Directory.Exists(path))
            System.IO.Directory.CreateDirectory(path);
        string outFileName = path + key + ".svg";
        doc.Save(outFileName, saveOptions);
        List<string> pages = new List<string>();
        for (int index = 0; index < doc.Pages.Count; index++)
        {
            string svgFile = path + key + ".svg";
            if (index > 0)
            {
                svgFile = path + key + "_" + (index + 1) + ".svg";
            }
            pages.Add(System.IO.File.ReadAllText(svgFile));
        }
        return pages;
    }
    #endregion

    ///// <summary>
    ///// 获取页面文字坐标信息
    ///// </summary>
    ///// <param name="pageNum"></param>
    ///// <returns></returns>
    //public List<LocationTextExtractionStrategyEx.TextChunk> GetPageCharsPositionInfo(int pageNum)
    //{
    //    if (pdfCharRegionInfo.ContainsKey(pageNum))
    //        return pdfCharRegionInfo[pageNum];
    //    else
    //        return null;
    //}

    /////// <summary>
    /////// 获取搜索到的结果
    /////// </summary>
    /////// <returns></returns>
    ////public SearchResult GetSearchResult()
    ////{            
    ////    return search_result;
    ////}

    //public bool FindAreaFromDoubleString(int pageNum, string leftTopStr, int leftTopOffsetX, int leftTopOffsetY, string rightBottomStr, int rightBottomOffsetX, int rightBottomOffsetY, out List<RectangleF> recs)
    //{
    //    recs = new List<RectangleF>();

    //    if (string.IsNullOrWhiteSpace(leftTopStr))
    //        return false;

    //    search_queue.Clear();
    //    search_region_queue.Clear();

    //    currentPageSize = pdfReader.GetPageSize(pageNum);
    //    var items = pdfCharRegionInfo[pageNum];
    //    bool result = findText(leftTopStr, items, 0, false);
    //    bool result2 = false;

    //    if (result)
    //    {
    //        //记录第一次查找到的字符
    //        var res = GetSearchResult();
    //        var rec = res.GetArea();


    //        result2 = findText(rightBottomStr, items, 0, false);
    //        if (!result2)
    //        {
    //            rec.Width = currentPageSize.Width - rec.X;
    //            rec.Height = currentPageSize.Height - rec.Y;
    //        }
    //        else
    //        {
    //            res = GetSearchResult();
    //            var rec2 = res.GetArea();

    //        }
    //    }

    //    return false;
    //}


    //public bool FindFirst(string text, int beginPage, bool up, int endPage = -1)
    //{
    //    int _numOfPage = numOfPage;
    //    if (endPage > numOfPage)
    //        endPage = numOfPage;
    //    if (endPage >= beginPage && endPage > 0)
    //        _numOfPage = endPage;

    //    for (int i = beginPage; i <= _numOfPage; i++)
    //    {
    //        search_queue.Clear();
    //        search_region_queue.Clear();
    //        searchPageIndex = beginPage;
    //        currentPageSize = pdfReader.GetPageSize(i);
    //        var items = pdfCharRegionInfo[i];

    //        var result = findText(text, items, 0, up);
    //        if (result)
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}

    ///// <summary>
    ///// 查找上一字符串
    ///// </summary>
    ///// <param name="text"></param>
    ///// <returns></returns>
    //public bool FindPrevious(string text, int? nSearchBeginIndexInPage = null, int? beginPage = null, int? endPage = null)
    //{
    //    if (beginPage == null)
    //        beginPage = searchPageIndex;
    //    if (endPage == null)
    //        endPage = numOfPage;

    //    if (nSearchBeginIndexInPage == null)
    //        nSearchBeginIndexInPage = searchBeginIndexInPage;

    //    for (int i = beginPage.Value; i <= endPage.Value; i++)
    //    {
    //        search_queue.Clear();
    //        search_region_queue.Clear();
    //        searchPageIndex = beginPage.Value;
    //        currentPageSize = pdfReader.GetPageSize(i);
    //        var items = pdfCharRegionInfo[i];
    //        var result = findText(text, items, nSearchBeginIndexInPage.Value, true);
    //        if (result)
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}

    ///// <summary>
    ///// 查找下个字符串
    ///// </summary>
    ///// <param name="text"></param>
    ///// <returns></returns>
    //public bool FindNext(string text, int? nSearchBeginIndexInPage = null, int? beginPage = null, int? endPage = null)
    //{
    //    if (beginPage == null)
    //        beginPage = searchPageIndex;
    //    if (endPage == null)
    //        endPage = numOfPage;

    //    if (nSearchBeginIndexInPage == null)
    //        nSearchBeginIndexInPage = searchBeginIndexInPage;

    //    //var beginPage = searchPageIndex;
    //    for (int i = beginPage.Value; i <= endPage.Value; i++)
    //    {
    //        search_queue.Clear();
    //        search_region_queue.Clear();
    //        searchPageIndex = beginPage.Value;
    //        currentPageSize = pdfReader.GetPageSize(i);
    //        var items = pdfCharRegionInfo[i];
    //        var result = findText(text, items, nSearchBeginIndexInPage.Value, false);
    //        if (result)
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}

    ///// <summary>
    ///// 按指定区域查找文字
    ///// </summary>
    ///// <param name="pageNum"></param>
    ///// <param name="area"></param>
    ///// <returns></returns>
    //public string FindInArea(int pageNum, RectangleF area)
    //{
    //    string str = string.Empty;

    //    if (pdfCharRegionInfo.ContainsKey(pageNum))
    //    {
    //        Region reg = new Region(area);
    //        var items = pdfCharRegionInfo[pageNum];

    //        currentPageSize = pdfReader.GetPageSize(pageNum);

    //        StringBuilder tempMsg = new StringBuilder();

    //        for (int i = 0; i < items.Count; i++)
    //        {
    //            var item = items[i];
    //            var rec = item.GetArea();

    //            rec.Y = currentPageSize.Height - rec.Y;

    //            tempMsg.AppendLine(string.Format("{0}->{1}", item.Text, rec.ToString()));

    //            var bl = reg.IsVisible(rec);
    //            if (bl) str += item.Text;
    //        }
    //        string ssss = tempMsg.ToString();
    //    }

    //    return str;
    //}

    //SearchResult search_result = null;
    //Queue<string> search_queue = new Queue<string>();
    //Queue<LocationTextExtractionStrategyEx.TextChunk> search_region_queue = new Queue<LocationTextExtractionStrategyEx.TextChunk>();

    //List<string> search_list = new List<string>();
    //List<LocationTextExtractionStrategyEx.TextChunk> search_region_list = new List<LocationTextExtractionStrategyEx.TextChunk>();

    //int searchBeginIndexInPage = 0;//字符搜索起始地址
    ///// <summary>
    ///// 字符搜索起始地址
    ///// </summary>
    //public int SearchBeginIndexInPage
    //{
    //    get { return searchBeginIndexInPage; }
    //    set { searchBeginIndexInPage = value; }
    //}
    //int searchPageIndex = 1;

    //iTextSharp.text.Rectangle currentPageSize = null;


    //private bool findText(string text, List<LocationTextExtractionStrategyEx.TextChunk> charRegionInfo, int index, bool up)
    //{
    //    if (index < 0) return false;
    //    if (index >= charRegionInfo.Count) return false;

    //    string tempStr = string.Empty;
    //    search_list.Clear();
    //    search_region_list.Clear();


    //    for (; up ? (index >= 0) : (index < charRegionInfo.Count); index += up ? (-1) : (1))
    //    {
    //        string currentChar = charRegionInfo[index].Text;


    //        search_list.Insert(up ? 0 : (search_list.Count == 0 ? 0 : (search_list.Count)), currentChar);
    //        search_region_list.Insert(up ? 0 : (search_region_list.Count == 0 ? 0 : (search_region_list.Count)), charRegionInfo[index]);

    //        searchBeginIndexInPage = index;

    //        //当前字符是否已经满足要求
    //        if (currentChar.Equals(text) || currentChar.Contains(text))//包含或者等于
    //        {
    //            //精准匹配
    //            //去掉最早的一个元素 是否还匹配                    
    //            search_result = new SearchResult(new List<LocationTextExtractionStrategyEx.TextChunk>() { charRegionInfo[index] }, currentPageSize);
    //            search_result.PageIndex = searchPageIndex;
    //            return true;
    //        }



    //        tempStr = string.Empty;
    //        for (int i = 0; i <= search_list.Count - 1; i++)
    //        {
    //            tempStr += search_list[i];

    //            if (tempStr.Length < text.Length) continue;

    //            if (tempStr.Equals(text) || tempStr.Contains(text))//包含或者等于
    //            {
    //                //if (i - 1 >= 0)
    //                //{
    //                //    search_list.RemoveRange(0, i - 1);
    //                //    search_region_list.RemoveRange(0, i - 1);
    //                //}

    //                //精准匹配
    //                //去掉最早的一个元素 是否还匹配                         

    //                search_result = new SearchResult(search_region_list, currentPageSize);
    //                search_result.PageIndex = searchPageIndex;
    //                return true;
    //            }
    //        }

    //        if (search_region_list.Count > text.Length)
    //        {
    //            search_list.RemoveRange(0, search_region_list.Count - text.Length);
    //            search_region_list.RemoveRange(0, search_region_list.Count - text.Length);
    //        }

    //    }

    //    return false;

    //    //search_list.Clear();
    //    //search_region_list.Clear();

    //    //for (int i = 0; i < charRegionInfo.Count; i++)
    //    //{
    //    //    string currentChar = charRegionInfo[i].Text;

    //    //    search_list.Insert(up ? 0 : (search_list.Count == 0 ? 0 : (search_list.Count - 1)), currentChar);
    //    //    search_region_list.Insert(up ? 0 : (search_region_list.Count == 0 ? 0 : (search_region_list.Count - 1)), charRegionInfo[index]);
    //    //}



    //    //if (up)
    //    //{
    //    //    for (int i = search_region_list.Count - 1; i >= 0; i--)
    //    //    {
    //    //        tempStr += search_region_list[i];

    //    //        if (tempStr.Length < text.Length) continue;

    //    //        if (tempStr.ToLower().Equals(text.ToLower()) || tempStr.ToLower().Contains(text.ToLower()))//包含或者等于
    //    //        {
    //    //            if (i - 1 >= 0)
    //    //            {
    //    //                search_list.RemoveRange(0, i - 1);
    //    //                search_region_list.RemoveRange(0, i - 1);
    //    //            }

    //    //            search_result = new SearchResult(search_region_list, currentPageSize);
    //    //            search_result.PageIndex = searchPageIndex;

    //    //            searchBeginIndexInPage = i;
    //    //            return true;
    //    //        }
    //    //    }
    //    //}
    //    //else
    //    //{
    //    //    for (int i = search_list.Count - 1; i >= 0; i--)
    //    //    {
    //    //        tempStr += search_list[i];

    //    //        if (tempStr.Length < text.Length) continue;

    //    //        if (tempStr.ToLower().Equals(text.ToLower()) || tempStr.ToLower().Contains(text.ToLower()))//包含或者等于
    //    //        {
    //    //            if (i - 1 >= 0)
    //    //            {
    //    //                search_list.RemoveRange(0, i - 1);
    //    //                search_region_list.RemoveRange(0, i - 1);
    //    //            }

    //    //            search_result = new SearchResult(search_region_list, currentPageSize);
    //    //            search_result.PageIndex = searchPageIndex;

    //    //            searchBeginIndexInPage = i;
    //    //            return true;
    //    //        }
    //    //    }
    //    //}

    //    //return false;
    //    //index = up ? --index : ++index;
    //    //searchBeginIndexInPage = index;
    //    //return findText(text, charRegionInfo, index, up);
    //}

    ///// <summary>
    ///// 查找字符
    ///// </summary>
    ///// <param name="text"></param>
    ///// <param name="charRegionInfo"></param>
    ///// <param name="index"></param>
    ///// <param name="up"></param>
    ///// <returns></returns>
    //private bool findTextB(string text, List<LocationTextExtractionStrategyEx.TextChunk> charRegionInfo, int index, bool up)
    //{
    //    if (index < 0) return false;
    //    if (index >= charRegionInfo.Count) return false;

    //    string currentChar = charRegionInfo[index].Text;


    //    search_list.Insert(up ? 0 : (search_list.Count == 0 ? 0 : (search_list.Count - 1)), currentChar);
    //    search_region_list.Insert(up ? 0 : (search_region_list.Count == 0 ? 0 : (search_region_list.Count - 1)), charRegionInfo[index]);

    //    string tempStr = string.Empty;
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
    //            return true;
    //        }
    //    }

    //    index = up ? --index : ++index;
    //    searchBeginIndexInPage = index;
    //    return findText(text, charRegionInfo, index, up);



    //    //search_queue.Enqueue(charRegionInfo[index].Text);
    //    //search_region_queue.Enqueue(charRegionInfo[index]);

    //    //if (search_queue.Count > text.Length)
    //    //    search_queue.Dequeue();
    //    //if (search_region_queue.Count > text.Length)
    //    //    search_region_queue.Dequeue();

    //    ////验证所搜到的字符是否为目标字符
    //    //string currentStr = string.Empty;
    //    //if(up)
    //    //{               

    //    //    var items= new List<string>();
    //    //    items.AddRange(search_queue.ToArray());
    //    //    items.Reverse();
    //    //    currentStr = string.Join("",items.ToArray());
    //    //}
    //    //else
    //    //{
    //    //    currentStr = string.Join("", search_queue.ToArray());
    //    //}


    //    //if(currentStr.ToLower().Equals(text.ToLower())||currentStr.Contains(text))
    //    //{
    //    //    //搜索到到的结果信息
    //    //    List<LocationTextExtractionStrategyEx.TextChunk> items = new List<LocationTextExtractionStrategyEx.TextChunk>();
    //    //    if(up)
    //    //    {
    //    //        while (search_region_queue.Count>0)
    //    //        items.Insert(0,search_region_queue.Dequeue());
    //    //    }
    //    //    else
    //    //    {
    //    //        while (search_region_queue.Count > 0)
    //    //            items.Add(search_region_queue.Dequeue());
    //    //    }

    //    //    search_result = new SearchResult(items,currentPageSize);
    //    //    search_result.PageIndex = searchPageIndex;
    //    //    return true;
    //    //}
    //    //else
    //    //{
    //    //    index = up ? --index : ++index;
    //    //    searchBeginIndexInPage = index;
    //    //    return findText(text,charRegionInfo,index,up);
    //    //}
    //}
}

}