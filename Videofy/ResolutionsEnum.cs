/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
*/

namespace Videofy.Main
{
    enum ResolutionsEnum
    {
        p144 = 144,
        p240 = 240,
        p360 = 360,
        p480 = 480,
        p720 = 720,
        p1080 = 1080,
        p1440 = 1440,
        p2160 = 2160
    }

    static partial class ExtensionMethods
    {
        public static string ToName(this ResolutionsEnum res)
        {
            return ((int)res).ToString();            
        }

        public static ResolutionsEnum FromName(this ResolutionsEnum res,string resolution
        {
            return (ResolutionsEnum)int.Parse(resolution);
            /*
            switch (pxlFmtString)
            {
                case "GRAY": return PixelFormat.Gray;
                case "RGB24": return PixelFormat.RGB24;
                case "YUV420P":
                    return PixelFormat.YUV420P;
                case "YUV444P":
                    return PixelFormat.YUV444P;
            }
            */
            throw new System.ArgumentOutOfRangeException();
        }
    }

}
