using System;
using System.Collections.Generic;
using System.IO;
//using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Videofy.Main;

namespace Videofy.Chain.Helpers
{
    static class DataHeader
    {
        static private int headLength = 12;

        static public void ToPipe(string path, OptionsStruct opt, Pipe pipeOut)
        {
            int size = 0;
            var fi = new FileInfo(path);
            long fileSize = fi.Length;            
            size += sizeof(long); //8
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
            while(i<8)
            {
                result[i] = (byte)(fileSize % 256);
                fileSize = fileSize / 256;
                i++;
            }
            result[i] = opt.density;
            i++;
            result[i] = opt.cellCount;
            i++;
            int temp = nameSize;
            result[i] = (byte)(temp % 256);
            temp /= 256;
            i++;
            result[i] = (byte)(temp % 256);
            i++;
            for(int j = 0; j < nameSize; j++)
            {
                result[i + j] = nameArray[j];
            }
            pipeOut.Add(result);
            pipeOut.Complete();
        }


        static public void FromPipe(ref OptionsStruct opt, 
                                    ref String fileName,
                                    ref long fileSize,
                                    Pipe pipeBlocks)
        {
            Queue<byte> queue = new Queue<byte>(headLength * 8);


            int k = 0;
            int todo = headLength;
            byte[] header = new byte[headLength];
            int nameSize = 0;
            //do decoding for params
            //density=1
            //cellcount=1
            while(k<todo)    
            {
                // get 64 bytes = block
                byte[] block = pipeBlocks.Take(64);
                
                // average value of block
                int sum = 0;
                for(byte q=0;q<64;q++)
                {
                    sum += block[q];
                }
                double tmp = (sum / 64.0);

                //covert cell value to byte sequence                               
                tmp /= 255;
                byte result = (byte)Math.Round(tmp);
                queue.Enqueue(result);

                if(queue.Count==8)
                {
                    byte b = 0;
                    for(int q = 0; q < 8; q++)
                    {
                        b *= 2;
                        b += queue.Dequeue();
                    }
                    header[k] = b;
                    k++;

                    if (k == headLength)
                    {
                        fileSize = 0;
                        byte x;
                        for (x = 7;x>=0;x--)
                        {
                            fileSize *= 256;
                            fileSize += header[x];
                        }
                        x = 8;
                        opt.density = header[x];
                        x++;
                        opt.cellCount = header[x];
                        x++;
                        nameSize = header[x+1]*256+header[x];

                        todo += nameSize;
                    }                    
                }


                
            }







            /*
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

            */
        }




        

    }
}
