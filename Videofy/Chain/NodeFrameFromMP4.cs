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
        public NodeFrameFromMP4(String path,OptionsStruct opt, Pipe Input):base(Input,null)
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
            while(Input.IsOpen|(Input.Count>0))
            {
                byte[] temp = Input.Take(frameSize);
                ffmpeg.Write(temp);
                String s;
                while ((s = ffmpeg.ErrorString()) != "")
                {
                   // Console.WriteLine(s);
                }
                while ((s = ffmpeg.ReadString()) != "")
                {
                  //  Console.WriteLine(s);
                }                
            }
            ffmpeg.Stop();
        }


    }
}
