using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;

namespace Videofy.Chain
{
    class Pipe
    {
        private BlockingCollection<byte[]> data;
        private byte[] buff = null;
        private int pos;
        private CancellationToken token;

        public Pipe(CancellationToken token)
        {
            this.token = token;
            data = new BlockingCollection<byte[]>(new ConcurrentQueue<byte[]>(), 5);
            pos = 0;
            buff = new byte[0];                        
        }

        public void Complete()
        {
            data.CompleteAdding();
        }

        public Boolean IsOpen { get { return !data.IsCompleted; } }

        //take data of any available size
        public byte[] Take()
        {
            byte[] temp = null;

            if (buff.Length > 0)
            {
                temp = buff;
                buff = new byte[0];
            }
            else
            {
                try
                {
                    temp = data.Take(token);
                }
                catch (InvalidOperationException e)
                {
                    if (!((token.IsCancellationRequested)|(data.IsCompleted)))
                    {
                        throw e;
                    }
                }
            }
            return temp;
        }

        // take only data of expected size
        public byte[] Take(int size)
        {
            byte[] temp = null; 

            while (buff.Length < size)
            {
                try
                {
                    temp = data.Take(token);
                }
                catch (InvalidOperationException e)
                {
                    if (!((token.IsCancellationRequested) | (data.IsCompleted)) )
                    {
                        throw e;
                    }
                    // pipe is finalized, append zeros to tail 
                    

                    int cnt = buff.Length;
                    Array.Resize<byte>(ref buff, size);
                    for(int i = cnt; i < size; i++)
                    {
                        buff[i] = 0;
                    }
                    temp = null;           
                }

                if(temp!=null)
                {
                    int cnt = buff.Length;
                    Array.Resize<byte>(ref buff, cnt + temp.Length);
                    Array.Copy(temp, 0, buff, cnt, temp.Length);
                }
            }

            if (buff.Length == size)
            {
                temp = buff;
                buff = new byte[0];
                return temp;
            }
            else //len > size
            {
                byte[] result = new byte[size];
                Array.Copy(buff, 0, result, 0, size);
                temp = new byte[buff.Length - size];
                Array.Copy(buff, size, temp, 0, buff.Length - size);                
                buff = temp;
                return result;
            }
        }



        public void Add(byte[] value)
        {
            if (value == null) throw new ArgumentNullException();
            if (value.Length == 0) throw new ArgumentException();
            data.Add(value,token);
        }
    }
}
