using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Videofy
{
    static class Checks
    {
        static public int cnt = 0;
        static private StreamWriter fread;
        static public void ReadSaveStart()
        {
            fread = new StreamWriter("readsave.txt", true);
        }
        static public void ReadSaveAdd(int b)
        {
            fread.WriteLine(b);
        }
        static public void ReadSaveEnd()
        {
            fread.Flush();
            fread.Close();
           
        }

        static private StreamWriter fwrite;
        static private StreamReader f;
        static public void WriteSaveStart()
        {
            fwrite = new StreamWriter("writesave.txt", true);
            f = new StreamReader("readsave.txt");
        }
        static public void WriteSaveAdd(int b)
        {
            fwrite.Write(b);
            fwrite.Write(" ");
        }
        static public void WriteSaveAdd(float b)
        {
            fwrite.Write(b.ToString("0"));
            fwrite.Write("    ");
        }

        static public void WriteSaveRead()
        {
            byte b;
            b = Convert.ToByte(f.ReadLine());
            fwrite.Write(b);
            fwrite.WriteLine();
        }

        static public void WriteSaveLine()
        {            
            fwrite.WriteLine();
        }
        static public void WriteSaveEnd()
        {
            fwrite.Flush();
            fwrite.Close();
            f.Close();
        }
        static public void WriteSaveFlush()
        {
            fwrite.Flush();
        }
    }
}
