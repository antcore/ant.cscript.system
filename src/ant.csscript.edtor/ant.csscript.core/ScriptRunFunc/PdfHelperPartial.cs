using Aspose.Pdf;
using Aspose.Pdf.Devices;
using Aspose.Pdf.Text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace daq.lib.Files
{
    public partial class PDFHelper
    {
        static object lockObj = new object();

        /// <summary>
        /// 获取pdf转换excel 后的到的excel 文件路基
        /// </summary>
        /// <returns></returns>
        public string GetExchangeExcelFilePath(string _pdfPath)
        {
            if (string.IsNullOrEmpty(_pdfPath))
            {
                throw new Exception("没有文件加载");
            }
            string dir = string.Format(@"{0}TEMP_PDF_TO_EXCEL", AppDomain.CurrentDomain.BaseDirectory);
            if (!System.IO.Directory.Exists(dir))
            {
                System.IO.Directory.CreateDirectory(dir);
            }
            string filePath = string.Empty;
            lock (lockObj)
            {
                filePath = string.Format(@"{0}\\{1}.xls", dir, DateTime.Now.ToString("yyyyMMddHHmmssfff"));
                Aspose.Pdf.Document doc = new Aspose.Pdf.Document(_pdfPath);
                doc.Save(filePath, Aspose.Pdf.SaveFormat.Excel);

                doc.Dispose();
            }
            return filePath;
        }

        public string GetExchangeWordFilePath(string _pdfPath)
        {
            if (string.IsNullOrEmpty(_pdfPath))
            {
                throw new Exception("没有文件加载");
            }
            string dir = string.Format(@"{0}TEMP_PDF_TO_WORD", AppDomain.CurrentDomain.BaseDirectory);
            if (!System.IO.Directory.Exists(dir))
            {
                System.IO.Directory.CreateDirectory(dir);
            }
            string filePath = string.Empty;
            lock (lockObj)
            {
                filePath = string.Format(@"{0}\\{1}.doc", dir, DateTime.Now.ToString("yyyyMMddHHmmssfff"));
                Aspose.Pdf.Document doc = new Aspose.Pdf.Document(_pdfPath);
                doc.Save(filePath, Aspose.Pdf.SaveFormat.Doc);
                doc.Dispose();
            }
            return filePath;
        }

        public void RemoveExchangeExcelFileByPath(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        public int GetPageCount(string _pdfPath)
        {
            return new Document(_pdfPath).Pages.Count;
        }

        /// <summary>
        /// 查找PDF文件中的所有文本信息
        /// </summary>
        /// <param name="_pdfPath"></param>
        /// <returns></returns>
        public string GetTextFromAllPage(string _pdfPath)
        {
            if (string.IsNullOrEmpty(_pdfPath))
            {
                throw new Exception("没有文件加载");
            }

            //string pdffilename = _pdfPath;
            //PdfReader pdfReader = new PdfReader(pdffilename);
            //int numberOfPages = pdfReader.NumberOfPages;
            //StringBuilder text = new StringBuilder();
            //for (int i = 1; i <= numberOfPages; ++i)
            //{
            //    text.Append(iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(pdfReader, i));
            //}
            //pdfReader.Close();
            //return text.ToString();

            //open document
            Document pdfDocument = new Document(_pdfPath);
            //create TextAbsorber object to extract text
            TextAbsorber textAbsorber = new TextAbsorber();
            //accept the absorber for all the pages
            pdfDocument.Pages.Accept(textAbsorber);
            //get the extracted text
            return textAbsorber.Text;
        }

        /// <summary>
        /// 查找PDF文件中的所有文本信息
        /// </summary>
        /// <param name="_pdfPath"></param>
        /// <returns></returns>
        public string GetAllPageText(string _pdfPath)
        {
            string text = string.Empty;
            PdfReader reader = new PdfReader(_pdfPath);
            for (int i = 1; i <= reader.NumberOfPages; i++)
            {
                text += GetTextFromPage(reader, i);
            }
            return text;
        }
        string GetTextFromPage(PdfReader reader, int pageNum)
        {
            ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
            return PdfTextExtractor.GetTextFromPage(reader, pageNum, strategy);
        }

        /// <summary>
        /// 查找PDF文件中的所有文本信息
        /// </summary>
        /// <param name="_pdfPath"></param>
        /// <returns></returns>
        public string GetTextByPage(string _pdfPath, int PageIndex)
        {
            if (PageIndex <= 0)
            {
                throw new Exception("页码必须大于等于1");
            }
            if (string.IsNullOrEmpty(_pdfPath))
            {
                throw new Exception("没有文件加载");
            }
            //open document
            Document pdfDocument = new Document(_pdfPath);
            //create TextAbsorber object to extract text
            TextAbsorber textAbsorber = new TextAbsorber();
            //accept the absorber for all the pages
            //pdfDocument.Pages.Accept(textAbsorber);
            pdfDocument.Pages[PageIndex].Accept(textAbsorber);
            //get the extracted text
            return textAbsorber.Text;
        }

        public ExchangeImageInfo CutImageOfPdf(string pdfPath, int pageNum, System.Drawing.Rectangle cutArea, int dpi = 300)
        {
            if (string.IsNullOrEmpty(pdfPath))
            {
                throw new Exception("文件路径不正确或为空");
            }
            if (pageNum < 1)
            {
                throw new Exception("pageNum 应从1开始计算");
            }

            Document pdfDocument = new Document(pdfPath);

            if (pageNum > pdfDocument.Pages.Count)
            {
                throw new Exception("pageNum 超出页码数");
            }

            using (var imageStream = new MemoryStream())
            {
                //create JPEG device with specified attributes
                //Width, Height, Resolution, Quality
                //Quality [0-100], 100 is Maximum
                //create Resolution object
                Resolution resolution = new Resolution(dpi);

                //JpegDevice jpegDevice = new JpegDevice(500, 700, resolution, 100);
                JpegDevice jpegDevice = new JpegDevice(resolution, 100);

                //convert a particular page and save the image to stream
                jpegDevice.Process(pdfDocument.Pages[pageNum], imageStream);

                var img = System.Drawing.Bitmap.FromStream(imageStream);

                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(cutArea.Width, cutArea.Height);

                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);

                g.DrawImage(img, new System.Drawing.Rectangle(0, 0, cutArea.Width, cutArea.Height), cutArea, System.Drawing.GraphicsUnit.Pixel);

                g.Dispose();

                var imageInfo = new ExchangeImageInfo();
                imageInfo.BitmapToBase64String(bitmap);

                //close stream
                imageStream.Close();

                return imageInfo;
            }
        }

        /// <summary>
        /// 必须指定 页码号和截图页面名称  且页码号与截图名称索引一致
        /// </summary>
        /// <param name="_pdfPath"></param>
        /// <param name="PagesIndex"> 页码 s</param>
        /// <returns></returns>
        public List<ExchangeImageInfo> ConvertPagesToImages(string _pdfPath, List<int> PagesIndex = null)
        {
            var list = new List<ExchangeImageInfo>();
            ExchangeImageInfo imageInfo;
            if (string.IsNullOrEmpty(_pdfPath))
            {
                throw new Exception("没有文件加载 _pdfPath");
            }
            //if (null == PagesIndex || PagesIndex.Count <= 0)
            //{
            //    throw new Exception("必须指明需要采集的页码 PagesIndex");
            //}

            Document pdfDocument = new Document(_pdfPath);
            bool flag = true;

            for (int pageCount = 1; pageCount <= pdfDocument.Pages.Count; pageCount++)
            {
                flag = false;
                //获取指定页面的截图
                if (null != PagesIndex && PagesIndex.Count > 0)
                {
                    foreach (var item in PagesIndex)
                    {
                        if (pageCount == item)
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                else
                {
                    flag = true;
                }

                if (flag)
                {
                    using (var imageStream = new MemoryStream())
                    {
                        //create JPEG device with specified attributes
                        //Width, Height, Resolution, Quality
                        //Quality [0-100], 100 is Maximum
                        //create Resolution object
                        //Resolution resolution = new Resolution(300);
                        Resolution resolution = new Resolution(80);

                        //JpegDevice jpegDevice = new JpegDevice(500, 700, resolution, 100);
                        JpegDevice jpegDevice = new JpegDevice(resolution, 100);

                        //convert a particular page and save the image to stream
                        jpegDevice.Process(pdfDocument.Pages[pageCount], imageStream);

                        imageInfo = new ExchangeImageInfo();

                        imageInfo.MemoryStreamToBase64String(imageStream);

                        list.Add(imageInfo);
                        //close stream
                        imageStream.Close();
                    }
                }
            }
            return list;
        }

    }
}