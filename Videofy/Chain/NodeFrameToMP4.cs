using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Videofy.Chain.Types;
using Videofy.Main;
namespace Videofy.Chain
{
    
    class NodeFrameToMP4:ChainNode
    {
        private  DFffmpeg ffmpeg;
        private OptionsStruct opt;
        private int frameSize;
        NodeToken token;

        public NodeFrameToMP4(String path,OptionsStruct opt, IPipe Input,NodeToken token):base(Input,null)
        {
            this.opt = opt;
            this.token = token;
            ffmpeg = new DFffmpeg(GenerateArgs(path, opt));
            InitFrameSize();
        }

        private String GenerateArgs(String path, OptionsStruct opt)
        {
            string result =
                "-y -f rawvideo -vcodec rawvideo "
                + "-s "
                    + ((int)opt.resolution * 16 / 9).ToString()
                    + "x"
                    + ((int)opt.resolution).ToString()
                    + " "
                + "-pix_fmt "
                    + opt.pxlFmtIn.ToName()
                    + " "
                + "-r 24 " //fps
                + "-i - "
                + "-pix_fmt "
                    + opt.pxlFmtOut.ToName()
                    + " "
                + "-c:v libx264 ";
            if (opt.isEncodingCRF)
            {
                result += "-crf ";
            }                 
            else
            {
                result += "-b:v ";
            }    
            result += 
                opt.videoQuality.ToString() 
                   + " "
                + "-preset "
                    + opt.encodingPreset.ToString()
                    + " "
                + "-an "
                + "\"" 
                    + path
                    +"\"";
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
            int pos = 0;
            ffmpeg.StarByte();
            while(Input.IsOpen|(Input.Count>0))
            {
                if(token.token)
                { break; }
                byte[] temp = Input.Take(frameSize);
                pos += temp.Length;
                ffmpeg.Write(temp);
                
                String s;
                while ((s = ffmpeg.ErrorString()) != "")
                {
                    Console.Write(s);
                }
                while ((s = ffmpeg.ReadString()) != "")
                {
                   Console.Write(s);
                }                
            }
            /*
            Console.WriteLine("MP4 TO MP4 TO MP4 TO MP4 TO");
            Console.WriteLine(pos.ToString());
            Console.WriteLine(pos.ToString());
            Console.WriteLine(pos.ToString());
            Console.WriteLine(pos.ToString());
            */
            ffmpeg.Hatiko();
        }


    }
}
