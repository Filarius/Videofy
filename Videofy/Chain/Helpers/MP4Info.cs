using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Videofy.Chain.Types;
using Videofy.Main;

namespace Videofy.Chain.Helpers
{
    class MP4Info
    {
        private DFffmpeg ffmpeg;
        

        public MP4Info(String path)
        {
           // this.opt = opt;
            //InitFrameSize();
            ffmpeg = new DFffmpeg(GenerateArgs(path));
        }

        public String GenerateArgs(String path)
        {
            String result = 
                " -i "
                + "\"" + path + "\"";
            return result;
        }

        public int GetWidth()
        {
            ffmpeg.StarByte();
            String s = "";
            String temp;
            while (true)
            {
                temp = ffmpeg.ErrorString();
                if(temp != "")
                {
                    s += temp;
                    continue;
                }
                else if(!ffmpeg.IsRunning)
                {
                    break;
                }
                ffmpeg.Wait(100);
            }
            ffmpeg.Stop();

            int l = s.IndexOf("Stream #");
            l = s.IndexOf("yuv", l);
            l = s.IndexOf("x", l) + 1;
            int r = s.IndexOf(",", l);
            s = s.Substring(l,r-l);
            return int.Parse(s);


        }
    }
}
