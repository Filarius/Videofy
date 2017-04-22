using System;
using Videofy.Main;

namespace Videofy.Settings
{

    static class Config
    {
        //public static float[,] rect = new float[8, 8];
        //public static float[] gist = new float[256];

        public static PixelFormat PixFmt = PixelFormat.YUV420P;
        public static PixelFormat PixFmtOut = PixelFormat.YUV420P;
        public static int BitLevel = 1;

        public static int FrameHeigth = 720;//90
        public static int FrameWidth
        {
            get { return (FrameHeigth * 16) / 9; }
        }
        // public static int FrameWidth = 1920;//160
        //  public static int FrameHeigth = 1080;
        public static int Block = 8;

        public static string FFMpegPath = /* @"D:\FILES\Code\Videofy\Videofy\bin\Debug\" + */ @"Utils\ffmpeg.exe";
        //public static string YoutubeDLPath = @"Utils\youtube-dl.exe";
        public static string YoutubeDLPath = @"Utils\livestreamer\livestreamer.exe ";


        private static string TempFFMpegEnc = "-y " +
            " -f rawvideo -vcodec rawvideo -s " +
          //  "%WIDTHSMALL%x%HEIGHTSMALL%"
          "%WIDTH%x%HEIGHT%" +
          " -pix_fmt " + // " gray "+
          " %PIXFORMAT% " +
          " -r 24 -i - " +
          //"-vf scale=%WIDTH%:%HEIGHT%:flags=neighbor" +
          " -pix_fmt %PIXFORMATOUT% -c:v libx264 " +/* "-g 1"+  */" -%ENC_MODE% %CRF%   -preset faster -an \"%OUTPUT%\"";

        private static string FFMpegEnc
        {
            get
            {
                return TempFFMpegEnc.Replace("%WIDTHSMALL%", (FrameWidth / Block).ToString())
                                    .Replace("%HEIGHTSMALL%", (FrameHeigth / Block).ToString())
                                    .Replace("%WIDTH%", FrameWidth.ToString())
                                    .Replace("%HEIGHT%", FrameHeigth.ToString())
                                    .Replace("%PIXFORMAT%", PixelFormater.ToString(PixFmt))
                                    .Replace("%PIXFORMATOUT%", PixelFormater.ToString(PixFmtOut));

            }
        }

        public static string GetFFmpegEncCRF(int crf, string path)
        {
            if (crf < 1) { crf = 1; }
            else if (crf > 60) { crf = 60; }
            return FFMpegEnc.Replace("%ENC_MODE%", "crf")
                            .Replace("%CRF%", crf.ToString())
                            .Replace("%OUTPUT%", path);
        }


        private static string TempFFMpegDec =
            "-y " +
            //   +   " -report "     + 
            " -i \"%INPUT%\"  -f image2pipe " +
            "  -pix_fmt " + " %PIXFORMAT% " +
            "-vcodec rawvideo " +
            // " -vf scale=%WIDTHSMALL%x%HEIGHTSMALL%:flags=area "+
            " -";
        public static string FFMpegDec
        {
            get
            {
                return TempFFMpegDec.Replace("%WIDTHSMALL%", (FrameWidth / Block).ToString())
                                    .Replace("%HEIGHTSMALL%", (FrameHeigth / Block).ToString())
                                    .Replace("%PIXFORMAT%", PixelFormater.ToString(PixFmt))
                                    //.Replace("%PIXFORMATOUT%", PixelFormater.ToString(PixFmtOut))
                                    ;
            }
        }



        private static string TempFFMpegDecDL = "-y " +
            // " -report +"   
            " -i - -f image2pipe  -pix_fmt %PIXFORMAT% -vcodec rawvideo " +
            //" -vf scale=%WIDTHSMALL%x%HEIGHTSMALL%:flags=area "+
            " -";
        public static string FFMpegDecDL
        {
            get
            {
                return TempFFMpegDecDL.Replace("%WIDTHSMALL%", (FrameWidth / Block).ToString())
                                    .Replace("%HEIGHTSMALL%", (FrameHeigth / Block).ToString())
                                    .Replace("%PIXFORMAT%", PixelFormater.ToString(PixFmt));
            }
        }

        public static string YoutubeDLGet = " -O %INPUT%  best  ";


        public static int BytesInFrame
        {
            get
            {
                int s = FrameWidth * FrameHeigth;
                switch (PixFmt)
                {
                    case PixelFormat.Gray:
                    case PixelFormat.RGB24:
                    case PixelFormat.YUV444P:
                        s *= 3;
                        return s;
                    case PixelFormat.YUV420P:
                        s = s + (s / 4) + (s / 4);
                        return s;
                }
                throw new Exception("Unexpected pixel format");
            }
        }
        public static int BlockSize = 502400;

        //   " output_%03d.jpg";


        static Config()
        {
            //     PixelsInFrame = FrameWidth * FrameHeigth;
            //    BytesInFrame = PixelsInFrame * 3;
            //    FFMpegEnc = String.Format(FFMpegEnc, FrameWidth, FrameHeigth) + "\"{0}\"";
            //  FFMpegEnc.Replace()
        }
    }

}
