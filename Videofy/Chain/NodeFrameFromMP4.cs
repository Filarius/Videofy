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
        private NodeToken token;
        public NodeFrameFromMP4(String path,OptionsStruct opt, IPipe Output,NodeToken token):base(null,Output)
        {
            this.opt = opt;
            this.token = token;
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
