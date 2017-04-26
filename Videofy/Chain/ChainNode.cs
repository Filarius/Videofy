using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
//using Videofy.Chain;

namespace Videofy.Chain
{
    class ChainNode
    {
        protected const int blockSize = 1024000;

        private IPipe _input, _output;
        private int _chunkSize;
        private Task _task;
        private NodeFunc _func;

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

        public ChainNode(IPipe input, /*int size,*/ IPipe output/*, NodeFunc func*/)
        {
            _input = input;
            _output = output;
           // _chunkSize = size;
         //   _func = func;
        }

        public virtual void Start()
        {
            Console.WriteLine("ERRROR ERRROR");
            throw new Exception("This Method Should Not Run");
        }
        


    }
}
