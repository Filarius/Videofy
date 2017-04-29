using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Videofy.Chain.Types;
using Videofy.Main;
namespace Videofy.Chain
{
    
    class NodeFrameFromMP4:ChainNode
    {
        private  DFffmpeg ffmpeg;
        private OptionsStruct opt;
        private int frameSize;
        public NodeFrameFromMP4(String path,OptionsStruct opt, IPipe Output):base(null,Output)
        {
            this.opt = opt;
            InitFrameSize();
            ffmpeg = new DFffmpeg(GenerateArgs(path, opt));            
        }

        private String GenerateArgs(String path, OptionsStruct opt)
        {
            string result =
                "-y -i "
                + "\"" + path + "\""
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
            byte[] temp;
            while (true)
            {
                while ((ffmpeg.ErrorString()) != "")
                {
                    // Console.WriteLine(s);
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
                Output.Add(temp);
            }
            Output.Complete();
            ffmpeg.Terminate();
        }


    }
}
