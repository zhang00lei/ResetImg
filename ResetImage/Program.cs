using System;
using System.Collections.Generic;
using System.Drawing;
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
            string tpsPath, configPath;
            if (args.Length != 0)
            {
                tpsPath = args[0];
                configPath = args[1];
            }
            else
            { 
                tpsPath = @"F:\UnityGame_UGUI\ArtResource\Atlas\AtlasLoad.tps";
                configPath = @"F:\UnityGame_UGUI\Assets\Atlas\AtlasLoad\AtlasLoad.tpsheet";
            }
            if (configPath.EndsWith(".txt"))
            {
                //for NGUI
                reset.DrawImage(configPath, tpsPath, 1);
            }
            else
            {
                //for UGUI
                reset.DrawImage(configPath, tpsPath, 0);
            } 
        }
    }
}
