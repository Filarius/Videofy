using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Videofy.Chain.Types;
using Videofy.Main;
namespace Videofy.Chain
{
    
    class NodeFrameFromYoutube:ChainNode
    {
        private  DFffmpeg ffmpeg;
        private DFYoutube youtube;
        private OptionsStruct opt;
        private int frameSize;
        public NodeFrameFromYoutube(String path,OptionsStruct opt, IPipe Output):base(null,Output)
        {
            this.opt = opt;
            InitFrameSize();
            ffmpeg = new DFffmpeg(GenerateArgsFFmpeg(path, opt));
        }

        private String GenerateArgsFFmpeg(String path, OptionsStruct opt)
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
            String s;
            //debug
           // int i = 0;

            byte[] temp;
            while (true)
            {
                while ((s=ffmpeg.ErrorString()) != "")
                {
                  //   Console.WriteLine(s);
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
                    {
                        break;
                    }
                }
                //debug
             //   i += temp.Length;
                
                Output.Add(temp);
            }
            //debug
          
            Output.Complete();
            ffmpeg.Terminate();
        }


    }
}
