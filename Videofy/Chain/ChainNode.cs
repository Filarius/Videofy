using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.IO;
//using Videofy.Chain;

namespace Videofy.Chain
{
    class ChainNode
    {
        protected const int blockSize = 1024000;

        private IPipe _input, _output;
       // private int _chunkSize;
       // private Task _task;
       // private NodeFunc _func;

        public IPipe Input
        {            
            get
            {
                return _input;
            }

            set { _input = value; }
        }

        public IPipe Output
        {
            get
            {
                return _output;
            }

            set { _output = value; }
        }

        private MemoryStream buffer;

        public ChainNode(IPipe input, /*int size,*/ IPipe output/*, NodeFunc func*/)
        {
            _input = input;
            _output = output;
            buffer = new MemoryStream();
           // _chunkSize = size;
         //   _func = func;
        }

        public virtual void Start()
        {
            Console.WriteLine("ERRROR ERRROR");
            throw new Exception("This Method Should Not Run");
        }

        protected void BufferWrite(byte[] array)
        {
            buffer.Write(array, 0, array.Length);
        }

        protected void BufferFlush()
        {
            if (buffer.Position == 0) return;
            
            //buffer.Position = 0;

            Output.Add(buffer.ToArray());

            buffer.SetLength(0);
        }

        protected long BufferLength
        {
            get
            {
                return buffer.Length;
            }
            private set { }
        }

        


    }
}
