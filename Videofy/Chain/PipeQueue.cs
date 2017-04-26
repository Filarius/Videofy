using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Videofy.Chain
{
    class PipeQueue:IPipe
    {
        private List<Pipe> list;
        private Pipe pipe;
        private Boolean toSingle;
        private CancellationToken token;

        public PipeQueue(CancellationToken token,
           List<Pipe> pipeList,
           Pipe pipe,
           Boolean toSingle)
        {
            if (pipeList.Count == 0)
                throw new ArgumentOutOfRangeException();
            this.list = pipeList;
            this.pipe = pipe;
            this.toSingle = toSingle;
            this.token = token;
        }


        public CancellationToken Token
        {
            get
            {
                return token;
            }
        }

        public int Count
        {
            get
            {
                int cnt = 0;
                foreach(Pipe pp in list)
                {
                    cnt += pp.Count;
                    if (cnt > 0)
                        break;
                }
                return cnt;
            }
        }

        public bool IsOpen
        {
            get
            {
                throw new NotImplementedException();
            }
        }

       

        public void Complete()
        {
            throw new NotImplementedException();
        }

        public byte[] Take()
        {
            throw new NotImplementedException();
        }

        public byte[] Take(int size)
        {
            throw new NotImplementedException();
        }

        public void Add(byte[] value)
        {
            throw new NotImplementedException();
        }
    }
}
