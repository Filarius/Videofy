using System;
using System.Collections.Generic;
//using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;

namespace Videofy.Chain
{
    class Pipe : IPipe
    {
        private const int maxCount = 5;
        private const int minSize = 10240*4;
        private BlockingCollection<byte[]> data;
        //private Queue<byte> queue;
        private MemoryStream buffIn, buffOut;
        private int inPos, outPos;
        //private byte[] buff = null;
        //private int pos;
        private CancellationToken token;

        public CancellationToken Token
        {
            get
            { return token; }
            set
            { token = value; }
        }

        public Pipe(CancellationToken token)
        {
            this.token = token;
            data = new BlockingCollection<byte[]>(new ConcurrentQueue<byte[]>(), maxCount);
            //queue = new Queue<byte>(102400);
            buffIn = new MemoryStream(102400);
            buffOut = new MemoryStream(102400);
            inPos = outPos = 0;
            //pos = 0;
            //buff = new byte[0];                        
        }

        public Boolean QueryIsFull { get { return data.Count == maxCount; } private set { } }

        public Boolean QueryIsEmpty { get { return data.Count == 0; } private set { } }

        public int BufferLength { get { return (int)(buffIn.Length + buffOut.Length); } private set { } }
        public int Count { get { return data.Count + (int)(buffIn.Length + buffOut.Length); } private set { } }

        public void Complete()
        {
            if (data.IsCompleted) return;
            if (buffIn.Length != 0)
            {
                data.Add(buffIn.ToArray());
                buffIn.SetLength(0);
            }
            data.CompleteAdding();
        }

        public Boolean IsOpen
        {
            get
            {
                return
                    !data.IsCompleted;
            }
        }

        //take data of any available size
        public byte[] Take()
        {
            byte[] temp = null;

            /*
            if (queue.Count > 0)
            {
                temp = queue.ToArray();
                queue.Clear();
            }
            else
            {
                try
                {
                    temp = data.Take(token);
                }
                catch (InvalidOperationException e)
                {
                    if (!((token.IsCancellationRequested) | (data.IsCompleted)))
                    {
                        throw e;
                    }
                }
            }
            */
            int i = 0;
            do
            {
                try
                {
                    byte[] d = data.Take();
                    buffOut.Write(d, 0, d.Length);
                    i++;
                }
                catch (InvalidOperationException e)
                {
                    if (!((token.IsCancellationRequested) | (data.IsCompleted)))
                    {
                        throw e;
                    }
                }

            } while ((i < maxCount) & (data.Count != 0));
                      

            if (buffOut.Length > 0)
            {
                temp = new byte[buffOut.Length - outPos];
                buffOut.Position = outPos;
                buffOut.Read(temp, 0, (int)buffOut.Length - outPos);
                buffOut.SetLength(0);
                outPos = 0;
            }

            return temp;
        }

        // take only data of expected size
        public byte[] Take(int size)
        {


            while ((buffOut.Length - outPos) < size)
            {
                if (data.IsCompleted) // no data in buffer pipe, append zeros to tail
                {
                    byte[] t = new byte[size - buffOut.Length + outPos];
                    Array.Clear(t, 0, t.Length);
                    buffOut.Write(t, 0, t.Length);
                }
                else
                {
                    try
                    {
                        byte[] t = data.Take(token);
                        buffOut.Write(t, 0, t.Length);
                    }
                    catch (InvalidOperationException e)
                    {
                        if (!((token.IsCancellationRequested) | (data.IsCompleted)))
                        {
                            throw e;
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        data.CompleteAdding();
                        while (data.Count > 0)
                        {
                            data.Take();
                        }

                    }
                }
            }

            byte[] result = new byte[size];
            buffOut.Position = outPos;
            buffOut.Read(result, 0, size);
            outPos += size;
            if ((buffOut.Length - outPos) < size)
            {
                byte[] t = new byte[buffOut.Length - buffOut.Position];
                buffOut.Read(t, 0, t.Length);
                buffOut.SetLength(0);
                buffOut.Write(t, 0, t.Length);
                outPos = 0;
            }
            else
            {
                buffOut.Position = buffOut.Length;
            }

            return result;

            /*
            
            byte[] temp = null;
             
            while (queue.Count < size)
            {
                if (data.IsCompleted & (data.Count == 0)) // no data in buffer pipe, append zeros to tail
                {
                    int cnt = queue.Count;
                    for (int i = cnt; i < size; i++)
                    {
                        queue.Enqueue(0);
                    }
                }
                else
                {
                    try
                    {
                        temp = data.Take(token);
                    }
                    catch (InvalidOperationException e)
                    {
                        if (!((token.IsCancellationRequested) | (data.IsCompleted)))
                        {
                            throw e;
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        data.CompleteAdding();
                        while (data.Count > 0)
                        {
                            data.Take();
                        }

                    }
                }

                if (temp != null)
                {
                    for (int i = 0; i < temp.Length; i++)
                    {
                        queue.Enqueue(temp[i]);
                    }
                }
            }

            if (queue.Count >= size)
            {
                temp = new byte[size];
                for (int i = 0; i < size; i++)
                {
                    temp[i] = queue.Dequeue();
                }

                return temp;
            }
            else
            {
                throw new Exception("THIS LINE MUST NOT BE EXECUTED");
            }
            */
        }



        public void Add(byte[] value)
        {
            if (value == null) throw new ArgumentNullException();
            if (value.Length == 0) throw new ArgumentException();

            if (token.IsCancellationRequested | data.IsCompleted)
            {
                return;
            }

            buffIn.Write(value, 0, value.Length);

            if (buffIn.Length >= minSize)
            {
                try
                {
                    byte[] t = buffIn.ToArray();
                    data.Add(t, token);

                }
                catch (OperationCanceledException)
                {
                    data.CompleteAdding();
                }
                buffIn.SetLength(0);
            }

        }
    }
}
