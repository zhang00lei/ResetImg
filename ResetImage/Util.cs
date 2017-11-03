using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ResetImage
{
    public class Util
    {
        /// <summary>
        /// 获取取string中的两个字符串之间的一段字符串
        /// </summary>
        /// <param name="first"></param>
        /// <param name="last"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static string GetInfoByStrings(string first, string last, string info)
        {
            int startIndex = info.LastIndexOf(first);
            int lastIndex = info.LastIndexOf(last);
            string result = info.Substring(startIndex + first.Length, lastIndex - startIndex - first.Length);
            return result;
        }

        public static string GetFileNameByFilePath(string filePath)
        {
            string[] temp = filePath.Split('/');
            string fileName = temp[temp.Length - 1];
            return fileName.Replace(".png", string.Empty);
        }

        public static string GetDirPathByFilePath(string filePath)
        {
            string[] temp = filePath.Split('\\');
            string str = temp[temp.Length - 1];
            filePath = filePath.Replace("\\"+str,string.Empty);
            return filePath;
        }

        public static string[] GetValueBracket(string info)
        {
            List<string> result = new List<string>();
            Regex reg = new Regex(@"(?is)(?<=\()[^\)]+(?=\))");
            MatchCollection mc = reg.Matches(info);
            foreach (Match m in mc)
            {
                result.Add(m.Value);
            }
            return result.ToArray();
        }
    }
}
