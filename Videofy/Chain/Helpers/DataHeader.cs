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
        static private int headLength = 8;

        static public void ToPipe(string path, OptionsStruct opt, Pipe pipeOut)
        {
            int size = 0;
            var fi = new FileInfo(path);
            long fileSize = fi.Length;            
            size += sizeof(long); //4
            //opt.density
            size += 1;
            //opt.cellCount
            size += 1;
            //fileName length
            string name = Path.GetFileName(path);
            byte[] nameArray = System.Text.UTF8Encoding.UTF8.GetBytes(name);
            int nameSize = nameArray.Length;
            if (nameSize >= 256 * 256)
                throw new ArgumentOutOfRangeException("File name length");
            size += 2;
            size += nameSize;
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
            i++;
            result[i] = (byte)(nameSize % 256);
            nameSize /= 256;
            i++;
            result[i] = (byte)(nameSize % 256);
            i++;
            for(int j = 0; j < nameSize; j++)
            {
                result[i + j] = nameArray[j];
            }
            pipeOut.Add(result);
        }

        static public void FromPipe(ref OptionsStruct opt, 
                                    ref String fileName,
                                    ref int fileSize,
                                    Pipe pipeIn)
        {
            byte[] head = pipeIn.Take(headLength);
            fileSize = 0;
            int i = 3;
            while (i >= 0)
            {
                fileSize = fileSize * 256;
                fileSize += head[i];                
                i--;
            }
            i = 4;
            opt.density = head[i];
            i++;
            opt.cellCount = head[i];
            i++;
            int nameSize = head[i]*256 + head[i+1];

            byte[] nameArray = pipeIn.Take(nameSize);

            fileName = System.Text.UTF8Encoding.UTF8.GetString(nameArray);
        }



        

    }
}
