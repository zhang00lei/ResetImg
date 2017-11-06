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

        private struct imgPos {
            public int Start_X;
            public int Start_Y;
            public int End_X;
            public int End_Y;
        }

        private int _getX(Bitmap bitmap,bool isStart)
        {
            int startIndex_X,endIndex_X,addCount;
            if (isStart)
            {
                startIndex_X = 0;
                endIndex_X = bitmap.Width - 1;
                addCount = 1;
            }
            else {
                startIndex_X = bitmap.Width - 1;
                endIndex_X = 0;
                addCount = -1;
            }
            for (int i = startIndex_X; i != endIndex_X; i += addCount)
            {
                for (int j = 0, height = bitmap.Height; j < height; j++)
                {
                    if (bitmap.GetPixel(i, j).ToArgb() != 0)
                    {
                        return i;
                    }
                }
            } 
            return 0;
        }
        private int _getY(Bitmap bitmap, bool isStart)
        {
            int startIndex_Y, endIndex_Y, addCount;
            if (isStart)
            {
                startIndex_Y = 0;
                endIndex_Y = bitmap.Height - 1;
                addCount = 1;
            }
            else
            {
                startIndex_Y = bitmap.Height - 1;
                endIndex_Y = 0;
                addCount = -1;
            }
            for (int i = startIndex_Y; i != endIndex_Y; i += addCount)
            {
                for (int j = 0, width = bitmap.Width; j < width; j++)
                {
                    if (bitmap.GetPixel(j, i).ToArgb() != 0)
                    {
                        return i;
                    }
                }
            }
            return 0;
        }

        private imgPos _getImgPos(Bitmap bitmap,bool isTrim)
        {
            imgPos pos = new imgPos();
            if (isTrim)
            {
                pos.Start_X = _getX(bitmap, true);
                pos.End_X = _getX(bitmap, false);
                pos.Start_Y = _getY(bitmap, true);
                pos.End_Y = _getY(bitmap, false); 
            } 
            return pos;
        }
        private bool _isTrim;
        //对应图片的绝对路径
        private Dictionary<string, string> imgPathInfoDic = new Dictionary<string, string>();
        private void _setImagePathByTpsFilePath(string tpsPath)
        {
            string str = File.ReadAllText(tpsPath);
            Regex regex = new Regex("<enum type=\"SpriteSettings::TrimMode\">None</enum>");
            _isTrim = !regex.IsMatch(str); 
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

        public void DrawImage(string configPath, string tpsPath, int type)
        {
            //默认tpssheet文件和对应的png文件放置于同一目录下
            string imgPath;
            if (type == 0)
            {
                _setImageInfoByTpsheetFilePath(configPath);
                imgPath = configPath.Replace(".tpsheet", ".png");
            }
            else
            {
                _setImageInfoByJsonFilePath(configPath);
                imgPath = configPath.Replace(".txt", ".png");
            }
            _setImagePathByTpsFilePath(tpsPath);

            Bitmap bmp = new Bitmap(_imgWidth, _imgHeight);
            foreach (KeyValuePair<string, imgInfo> kv in imgInfoDic)
            {
                string imgPathTemp = imgPathInfoDic[kv.Key];
                Bitmap imgBitmapTemp = new Bitmap(Bitmap.FromFile(imgPathTemp));
                imgPos pos = _getImgPos(imgBitmapTemp,_isTrim);
                for (int i = pos.Start_X; i < pos.End_X; i++)
                {
                    for (int j = pos.Start_Y; j < pos.End_Y; j++)
                    {
                        bmp.SetPixel(
                            (i-pos.Start_X) + kv.Value.x,
                            (j-pos.Start_Y) + kv.Value.y,
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
                if (infoArray[i].StartsWith("#") || infoArray[i] == ""||infoArray[i].StartsWith(":"))
                {
                    if (infoArray[i].StartsWith(":size="))
                    {
                        string strTemp = infoArray[i].Replace(":size=",string.Empty);
                        string[] tempArray = strTemp.Split('x');
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