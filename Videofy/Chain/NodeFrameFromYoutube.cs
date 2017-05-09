using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Videofy.Chain.Types;
using Videofy.Main;
namespace Videofy.Chain
{

    class NodeFrameFromYoutube : ChainNode
    {
        private DFffmpeg ffmpeg;
        private DFYoutube youtube;
        private OptionsStruct opt;
        private int frameSize;
        private NodeToken token;

        public NodeFrameFromYoutube(String url, OptionsStruct opt, IPipe Output, NodeToken token)
            : base(null, Output)
        {
            this.opt = opt;
            this.token = token;
            InitFrameSize();
            ffmpeg = new DFffmpeg(GenerateArgsFFmpeg(opt));
            youtube = new DFYoutube(GenerateArgsYoutube(url));
        }

        private String GenerateArgsYoutube(String url)
        {
            string result = " -O "
                //+ " --player-external-http-port 12345 "
                + url
                  + " "
                + " best"
                ;
            return result;
        }

        private String GenerateArgsFFmpeg(OptionsStruct opt)
        {
            string result =
                "-y -i "
                + "- "
                //    + "http://127.0.0.1:12345 "
                + "-f image2pipe "
                + "-pix_fmt "
                  + opt.pxlFmtIn.ToName()
                  + " "
                + "-vcodec rawvideo "
                + "-";
            return result;
        }

        private void InitFrameSize()
        {
            DFFrame frame = new DFFrame(opt);
            frameSize = frame.Size;
            frame.Free();
        }

        public override void Start()
        {
            ffmpeg.StarByte();
            youtube.StarByte();
            Boolean youtubeStillRun;
            String s;
            //debug
            int i = 0;

            byte[] temp;
            bool youtubeClosed = false;
            while (true)
            {
                if (token.token)
                {
                    break;
                }

                while ((s = ffmpeg.ErrorString()) != "")
                {
                    Console.WriteLine(s);
                }
                while ((s = youtube.ErrorString()) != "")
                {
                    Console.WriteLine(s);
                }                               

                while (ffmpeg.ReadCount > 0)
                {
                    temp = ffmpeg.Read();
                    if (temp != null)
                    {
                        Output.Add(temp);
                    }
                }

                while ((youtube.ReadCount > 0) & (ffmpeg.WriteCount < 2))
                {
                    temp = youtube.Read();
                    if (temp != null)
                    {
                        ffmpeg.Write(temp);
                    }
                //    i += temp.Length;
                //    Console.WriteLine(i);
                }

                if ((!youtubeClosed)
                    &(!youtube.IsRunning)
                    &(youtube.ReadCount==0)
                    &ffmpeg.WriteCount==0
                    &ffmpeg.ReadCount==0)
                {
                    ffmpeg.Hatiko();
                    youtubeClosed = true;
                }

                if (youtubeClosed & (!ffmpeg.IsRunning)&(ffmpeg.ReadCount==0))
                {
                    break;
                }
            }
            //debug

            Output.Complete();
            
            youtube.Terminate();
        }


    }
}
