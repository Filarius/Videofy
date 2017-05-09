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
                + url
                  + " "
                + " best";
            return result;
        }

        private String GenerateArgsFFmpeg(OptionsStruct opt)
        {
            string result =
                "-y -i "
                + "-"
                  + " "
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
            // int i = 0;

            byte[] temp;
            while (true)
            {
                if(token.token)
                {
                    break;
                }

                while ((s=ffmpeg.ErrorString()) != "")
                {
                       Console.WriteLine(s);
                }
                //while ((ffmpeg.Read()) != null)
                {
                    //   Console.WriteLine(s);
                }
                while ((s=youtube.ErrorString()) != "")
                {
                    Console.WriteLine(s);
                }

                youtubeStillRun = youtube.IsRunning;
                temp = youtube.Read();
                if (temp != null)
                {
                    ffmpeg.Write(temp);
                }


                if (ffmpeg.IsRunning)
                {
                    temp = ffmpeg.Read();
                    if (temp == null)
                    {
                        continue;
                    }
                }
                else
                {
                    temp = ffmpeg.Read();
                    if (temp == null)
                        if (youtubeStillRun) //be sure there was no more data 
                                             //from youtube after last request                       
                        {
                            continue;
                        }
                        else
                        {
                            break;
                        }

                }

                Output.Add(temp);
            }
            //debug

            Output.Complete();
            ffmpeg.Terminate();
            youtube.Terminate();
        }


    }
}
