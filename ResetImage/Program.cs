using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ResetImage
{
    class Program
    {
        static void Main(string[] args)
        {
            ResetPixel reset = new ResetPixel();
            string tpsPath, tpsheetPath;
            if (args.Length != 0)
            {
                tpsPath = args[0];
                tpsheetPath = args[1];
            }
            else
            { 
                tpsPath = @"E:\project\美术资源\第四版美术资源\Atlas\AtlasCommon.tps"; 
                tpsheetPath = @"E:\project\Branch\ClientWorkBranch\NewUI_V4\Assets\Resources\Res\Atlas_V4\AtlasCommon\AtlasCommon.txt";
            }
            //0:for UGUI
            //1:for NGUI
            reset.DrawImage(tpsheetPath, tpsPath, 1);
        }
    }
}
