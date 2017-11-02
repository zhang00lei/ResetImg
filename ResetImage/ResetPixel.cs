using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace ResetImage
{
    class ResetPixel
    {
        private struct imgInfo
        {
            public string ImgName;
            public int Width;
            public int Height;
            public int x;
            public int y;
        }   

        //对应图片的大小位置等相关信息
        private Dictionary<string, imgInfo> imgInfoDic = new Dictionary<string, imgInfo>();

        private void _setImageInfoByTpsheetFilePath(string tpsheetPath)
        {
            string[] infoArray = File.ReadAllLines(tpsheetPath);
            for (int i = 0, length = infoArray.Length; i < length; i++)
            {
                if (infoArray[i].StartsWith("#") || infoArray[i] == "")
                {
                    continue;
                }
                imgInfo info = new imgInfo();
                string[] temp = infoArray[i].Split(';');
                info.ImgName = temp[0];
                info.x = int.Parse(temp[1]);
                info.y = int.Parse(temp[2]);
                info.Width = int.Parse(temp[3]);
                info.Height = int.Parse(temp[4]);
                imgInfoDic.Add(temp[0], info);
            }
        }

        //对应图片的绝对路径
        private Dictionary<string, string> imgPathInfoDic = new Dictionary<string, string>();
        private void _setImagePathByTpsFilePath(string tpsPath)
        {
            string info = File.ReadAllText(tpsPath);
            info = info.Replace("\r", string.Empty).Replace("\n",string.Empty); ;
            info = info.Replace(" ", string.Empty);
            info = Util.GetInfoByStrings("<array><filename>", "</filename></array>", info);
            string[] fileArray = Regex.Split(info, "</filename><filename>");
            string fileDir = Util.GetDirPathByFilePath(tpsPath);

            for (int i = 0; i < fileArray.Length; i++)
            {
                string fileName = Util.GetFileNameByFilePath(fileArray[i]);
                string imgPath = fileArray[i].Replace("/", "\\");
                imgPath = Path.Combine(fileDir, imgPath);
                imgPathInfoDic.Add(fileName, imgPath);
            }
        }

        public void DrawImage(string tpsheetPath, string tpsPath)
        {
            _setImageInfoByTpsheetFilePath(tpsheetPath);
            _setImagePathByTpsFilePath(tpsPath);
            //默认tpssheet文件和对应的png文件放置于同一目录下
            string imgPath = tpsheetPath.Replace(".tpsheet", ".png");
            Image image = Image.FromFile(imgPath);

            Bitmap bmp = new Bitmap(image.Width, image.Height);
            foreach (KeyValuePair<string, imgInfo> kv in imgInfoDic)
            {
                string imgPathTemp = imgPathInfoDic[kv.Key];
                Bitmap imgBitmapTemp = new Bitmap(Bitmap.FromFile(imgPathTemp));
                for (int i = 0, width = kv.Value.Width; i < width; i++)
                {
                    for (int j = kv.Value.y, height = kv.Value.Height; j < height; j++)
                    {
                        bmp.SetPixel(
                            i + kv.Value.x,
                            bmp.Height-kv.Value.y-j,
                            imgBitmapTemp.GetPixel(i, j)
                            );
                    }
                }
                Console.WriteLine(kv.Key);
            }
            bmp.Save(imgPath.Replace(".png", "1.png"));
        }
    }
}
