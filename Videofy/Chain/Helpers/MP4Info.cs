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
      //  private DFffmpeg ffmpeg;
        //private string path;
        

        public MP4Info()
        {
           // this.path = path;
           // this.opt = opt;
            //InitFrameSize();
            
        }

        public String GenerateArgs(String path)
        {
            ;
            String result = 
                " -i "
                + "\"" + path + "\"";
            return result;
        }

        public int GetWidthFromPath(String path)
        {
            DFffmpeg ffmpeg = new DFffmpeg(GenerateArgs(path));
            ffmpeg.StarByte();
            String s = "";
            String temp;
            while (true)
            {
                ffmpeg.Wait(100);
                temp = ffmpeg.ErrorString();
                if(temp != "")
                {
                    s += temp;
                    continue;
                }
                else if((s.Length>0) & (!ffmpeg.IsRunning))
                {
                    break;
                }
                
            }
            ffmpeg.Terminate();

            int l = s.IndexOf("Stream #");
            l = s.IndexOf("yuv", l);
            l = s.IndexOf("x", l) + 1;
            int r = s.IndexOf(",", l);
            s = s.Substring(l,r-l);
            r = s.IndexOf(" ");
            if (r > 0)
            {
                s = s.Substring(0, r);
            }
            return int.Parse(s);
        }

        public int GetWidthFromUrl(String url)
        {
            DFYoutube youtube = new DFYoutube(url);
            youtube.StarByte();

            String s = "";
            String temp;
            while (true)
            {
                youtube.Wait(100);
                temp = youtube.ReadString();               
                if (temp != "")
                {
                    s += temp;
                    continue;
                }
                else
                {
                    temp = youtube.ErrorString();
                    if (temp != "")
                    {
                        s += temp;
                        continue;
                    }
                    else if ((s.Length > 0) & (!youtube.IsRunning))
                    {
                        break;
                    }
                }                
                

            }
            youtube.Terminate();

            int p = s.IndexOf("(best)");
            if (p <= 0) throw new Exception("WIDTH SEARCH ERROR");
            int r, l;
            r = 0;
            while(((l=s.IndexOf(",",r+1))!=-1)&(l<p))
            {
                r = l;
            }
            l = r + 2;
            r = s.IndexOf(" ", l);
            s = s.Substring(l, r - l-1);
            
            return int.Parse(s);
        }
        
    }
}
