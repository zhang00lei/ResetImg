using Newtonsoft.Json.Linq;
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

        //对应图片的绝对路径
        private Dictionary<string, string> imgPathInfoDic = new Dictionary<string, string>();
        private void _setImagePathByTpsFilePath(string tpsPath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(tpsPath);
            XmlNodeList xnl = doc.ChildNodes[1].ChildNodes[0].ChildNodes;
            List<XmlNode> arrayNodeList = new List<XmlNode>();
            foreach (XmlNode xn in xnl)
            {
                if (xn.Name == "array")
                {
                    arrayNodeList.Add(xn);
                }
            }
            xnl = arrayNodeList[1].ChildNodes;
            List<string> pathList = new List<string>();
            foreach (XmlNode xn in xnl)
            {
                pathList.Add(xn.InnerText);
            }
            string fileDir = Util.GetDirPathByFilePath(tpsPath);

            for (int i = 0; i < pathList.Count; i++)
            {
                string fileName = Util.GetFileNameByFilePath(pathList[i]);
                string imgPath = pathList[i].Replace("/", "\\");
                imgPath = Path.Combine(fileDir, imgPath);
                imgPathInfoDic.Add(fileName, imgPath);
            }
        }

        public void DrawImage(string tpsheetPath, string tpsPath, int type)
        {
            //默认tpssheet文件和对应的png文件放置于同一目录下
            string imgPath;
            if (type == 0)
            {
                _setImageInfoByTpsheetFilePath(tpsheetPath);
                imgPath = tpsheetPath.Replace(".tpsheet", ".png");
            }
            else
            {
                _setImageInfoByJsonFilePath(tpsheetPath);
                imgPath = tpsheetPath.Replace(".txt", ".png");
            }
            _setImagePathByTpsFilePath(tpsPath);

            Bitmap bmp = new Bitmap(_imgWidth, _imgHeight);
            foreach (KeyValuePair<string, imgInfo> kv in imgInfoDic)
            {
                string imgPathTemp = imgPathInfoDic[kv.Key];
                Bitmap imgBitmapTemp = new Bitmap(Bitmap.FromFile(imgPathTemp));
                for (int i = 0, width = kv.Value.Width; i < width; i++)
                {
                    for (int j = 0, height = kv.Value.Height; j < height; j++)
                    {
                        bmp.SetPixel(
                            i + kv.Value.x,
                            j + kv.Value.y,
                            imgBitmapTemp.GetPixel(i, j)
                            );
                    }
                }
            }
            bmp.Save(imgPath, ImageFormat.Png);
        }

        #region 通过TexturePacker生成的json文件或者tpsheet文件获取对应图片信息

        //生成图片的大小
        private int _imgWidth;
        private int _imgHeight;
        //对应图片的大小位置等相关信息
        private Dictionary<string, imgInfo> imgInfoDic = new Dictionary<string, imgInfo>();
        private void _setImageInfoByJsonFilePath(string tpsheetPath)
        {
            string jsonConfig = File.ReadAllText(tpsheetPath);
            JToken jo = JObject.Parse(jsonConfig);
            JToken arr = jo["frames"];
            foreach (JProperty baseJToken in arr)
            {
                JToken temp = baseJToken.First;
                imgInfo info = new imgInfo();
                info.Width = int.Parse(temp["frame"]["w"].Value<string>());
                info.Height = int.Parse(temp["frame"]["h"].Value<string>());
                info.x = int.Parse(temp["frame"]["x"].Value<string>());
                info.y = int.Parse(temp["frame"]["y"].Value<string>());
                imgInfoDic.Add(baseJToken.Name.Replace(".png",string.Empty),info);
            }

            JToken imgInfo = jo["meta"];
            _imgWidth = int.Parse( imgInfo["size"]["w"].Value<string>());
            _imgHeight = int.Parse(imgInfo["size"]["h"].Value<string>());
        }

        private void _setImageInfoByTpsheetFilePath(string tpsheetPath)
        {
            string[] infoArray = File.ReadAllLines(tpsheetPath);
            for (int i = 0, length = infoArray.Length; i < length; i++)
            {
                if (infoArray[i].StartsWith("#") || infoArray[i] == "")
                {
                    string repTemp = "# Sprite sheet: ";
                    if (infoArray[i].StartsWith(repTemp))
                    {
                        string imgInfo = Util.GetValueBracket(infoArray[i])[0];
                        string[] tempArray = imgInfo.Split('x');
                        _imgWidth = int.Parse(tempArray[0].Trim());
                        _imgHeight = int.Parse(tempArray[1].Trim());
                    }
                    continue;
                }
                imgInfo info = new imgInfo();
                string[] temp = infoArray[i].Split(';');
                info.ImgName = temp[0];
                info.Width = int.Parse(temp[3]);
                info.Height = int.Parse(temp[4]);
                info.x = int.Parse(temp[1]);
                info.y = _imgHeight - (int.Parse(temp[2]) + info.Height);
                imgInfoDic.Add(temp[0], info);
            }
        }
    }
#endregion
}