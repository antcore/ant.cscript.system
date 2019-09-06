using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace daq.lib.Files
{
    public class ExchangeImageInfo
    {

        private string _imageName;
        /// <summary>
        /// 图片名称
        /// </summary>
        public string ImageName
        {
            get
            {
                return _imageName;
            }
            set
            {
                _imageName = value.Replace(@"/", "_").Replace(@"\", "_");//.Replace(" ","");
            }
        }
        /// <summary>
        /// 图片的Base64 
        /// </summary>
        public string ImageBase64Str { get; set; }

        public void BitmapToBase64String(Bitmap bmp)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);

                //ImageBase64Str = GZip.ToBase64String(GZip.Compress(arr));  //Serialize.GZip.Compress(Convert.ToBase64String(arr));

                ms.Close();
                
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void MemoryStreamToBase64String(MemoryStream ms)
        {
            try
            {
                //Bitmap bmp = new Bitmap();
                //bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);

                //ImageBase64Str = GZip.ToBase64String(GZip.Compress(arr));//Serialize.GZip.Compress(Convert.ToBase64String(arr));

                ms.Close(); 
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 从base64获取图片
        /// </summary>
        /// <param name="base64String"></param>
        /// <returns></returns>
        public static void ToImageFromBase64String(string base64String,string imgPath)
        {
            //byte[] arr2 = GZip.ToBytesFromBase64(base64String);
            ////base64还原成 数组 
            //byte[] imgBytes = GZip.Decompress(arr2);

            //using (System.IO.MemoryStream ms2 = new System.IO.MemoryStream(imgBytes))
            //{
            //    System.Drawing.Bitmap bmp2 = new System.Drawing.Bitmap(ms2);
            //    bmp2.Save(imgPath, System.Drawing.Imaging.ImageFormat.Jpeg);
            //    //return bmp2;
            //}
        }
    }
}
