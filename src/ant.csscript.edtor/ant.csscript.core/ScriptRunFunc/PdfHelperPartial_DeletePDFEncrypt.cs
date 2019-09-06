using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.IO;

namespace daq.lib.Files
{
    public partial class PDFHelper
    {
        /// <summary>
        /// //有密码的pdf文件转存新文件（返回无密码的新文件地址）
        /// </summary>
        /// <returns></returns>
        public string DeletePDFEncrypt(string _pdfPath)
        {
            if (string.IsNullOrEmpty(_pdfPath))
            {
                throw new Exception("确少pdf文件路径.");
            }
            string dir = string.Format(@"{0}TEMP_PDF_TO_PDF", AppDomain.CurrentDomain.BaseDirectory);
            if (!System.IO.Directory.Exists(dir))
            {
                System.IO.Directory.CreateDirectory(dir);
            }
            string newFullName = string.Format(@"{0}\\{1}.pdf", dir, DateTime.Now.ToString("yyyyMMddHHmmssfff"));
            try
            {
                // 创建一个PdfReader对象
                PdfReader reader = new PdfReader(_pdfPath);
                PdfReader.unethicalreading = true;
                // 获得文档页数
                int n = reader.NumberOfPages;
                // 获得第一页的大小
                Rectangle pagesize = reader.GetPageSize(1);
                float width = pagesize.Width;
                float height = pagesize.Height;
                // 创建一个文档变量
                Document document = new Document(pagesize, 50, 50, 50, 50);
                // 创建该文档
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(newFullName, FileMode.Create));
                // 打开文档
                document.Open();
                // 添加内容
                PdfContentByte cb = writer.DirectContent;
                int i = 0;
                int p = 0;
                while (i < n)
                {
                    document.NewPage();
                    p++;
                    i++;
                    PdfImportedPage page1 = writer.GetImportedPage(reader, i);
                    cb.AddTemplate(page1, 1f, 0, 0, 1f, 0, 0);
                }
                // 关闭文档
                document.Close();
            }
            catch (Exception ex)
            {
                newFullName = string.Empty;
                throw new Exception(ex.Message);
            }
            return newFullName;
        }


    }
}