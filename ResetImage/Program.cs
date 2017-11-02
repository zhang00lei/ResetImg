using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResetImage
{
    class Program
    {
        static void Main(string[] args)
        {
            ResetPixel reset = new ResetPixel();
            //reset.SetImagePathByFilePath(@"F:\UnityGame_UGUI\ArtResource\Atlas\AtlasLoad.tps");
            //reset.SetImageInfoByFilePath(@"F:\UnityGame_UGUI\Assets\Atlas\AtlasLoad\AtlasLoad.tpsheet");
            string tpsPath = @"F:\UnityGame_UGUI\ArtResource\Atlas\AtlasLoad.tps";
            string tpsheetPath = @"F:\UnityGame_UGUI\Assets\Atlas\AtlasLoad\AtlasLoad.tpsheet";
            reset.DrawImage(tpsheetPath,tpsPath); 
        }
    }
}
