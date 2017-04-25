using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Videofy.Main;

namespace Videofy.Chain.Helpers
{
    static class DataHeader
    {
        static readonly public int Length = 6;

        static public byte[] Generate(string path,OptionsStruct opt)
        {
            int size = 0;
            var fi = new FileInfo(path);
            long fileSize = fi.Length;
            string name = Path.GetFileName(path);
            size += sizeof(long);
            //opt.density
            size += 1;
            //opt.cellCount
            size += 1;
            byte[] result = new byte[size];
            int i = 0;
            while(i<4)
            {
                result[i] = (byte)(fileSize % 256);
                fileSize = fileSize / 256;
                i++;
            }
            result[i] = opt.density;
            i++;
            result[i] = opt.cellCount;
            return result;            
        }

        static public void Extract(byte[] header,ref OptionsStruct opt,ref long fileSize)
        {
            if (header.Length != Length) throw new ArgumentOutOfRangeException();
            long size = 0;
            int i = Length - 1;
            opt.cellCount = header[i];
            i--;
            opt.density = header[i];
            i--;
            while(i>0)
            {
                size += header[i];
                size = size * 256;
                i--;
            }
            fileSize = size;
        }



        

    }
}
