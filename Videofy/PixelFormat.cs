//using 

namespace Videofy.Main
{
    enum PixelFormat
    {
        Gray,
        RGB24,
        YUV420P,
        YUV444P
    }

    static class PixelFormater
    {
        static public string ToString(PixelFormat pixFmt)
        {
            
            switch (pixFmt)
            {
                case PixelFormat.Gray: return "gray";
                case PixelFormat.RGB24: return "rgb24";
                case PixelFormat.YUV420P:
                    return "yuv420p";
                case PixelFormat.YUV444P:
                    return "yuv444p";
            }
            return "wrong_pxl_fmt";
        }
    }


    //EXTENTIONS
    static partial class ExtensionMethods

    {
        public static string ToFriendlyName(this PixelFormat pxlFmt)
        {
            switch (pxlFmt)
            {
                case PixelFormat.Gray: return "GRAY";
                case PixelFormat.RGB24: return "RGB24";
                case PixelFormat.YUV420P:
                    return "YUV420P";
                case PixelFormat.YUV444P:
                    return "YUV444P";
            }
            throw new System.ArgumentOutOfRangeException();            
        }

        public static PixelFormat FromFriendlyName(this PixelFormat pxlFmt, string pxlFmtString)
        {
            switch (pxlFmtString)
            {
                case "GRAY": return PixelFormat.Gray;
                case "RGB24": return PixelFormat.RGB24;
                case "YUV420P": 
                    return PixelFormat.YUV420P;
                case "YUV444P":
                    return PixelFormat.YUV444P;
            }
            throw new System.ArgumentOutOfRangeException();
        }
    }
}
