using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Videofy.Chain
{
    class PipeJoiner : IPipe
    {
        private List<Pipe> list;
     //   private Pipe pipe;
        private CancellationToken token;
        private byte pipeIndex;

        public PipeJoiner(CancellationToken token,
           List<Pipe> pipeList)
        {
            if (pipeList.Count == 0)
                throw new ArgumentOutOfRangeException();
            this.list = new List<Pipe>(pipeList);
       //     this.pipe = pipe;
            this.token = token;
            this.pipeIndex = 0;
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
                return list[pipeIndex].Count;
            }
        }

        public bool IsOpen
        {
            get
            {
                Boolean result = false;
                foreach (var pp in list)
                {
                    if (pp.IsOpen)
                    {
                        result = true;
                        break;
                    }
                }
                return result;
            }
        }



        public void Complete()
        {
            foreach (Pipe pp in list)
            {
                pp.Complete();
            }
        }

        public byte[] Take()
        {
            byte[] result = null;
            while (true)
            {
                result = list[pipeIndex].Take();
                if (result == null)
                {
                    pipeIndex++;
                    if (pipeIndex == (list.Count))
                    {
                        pipeIndex--;
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            return result;
        }

        public byte[] Take(int size)
        {
            Pipe pp = list[pipeIndex];
            while (pipeIndex < (list.Count - 1))
            {
                ;
                if ((!pp.IsOpen) & (pp.Count == 0))
                {
                    pipeIndex++;
                    pp = list[pipeIndex];
                    continue;
                }
                else
                {
                    break;
                }
            }
            if (pipeIndex == list.Count)
            { pipeIndex--; }
            return pp.Take(size);
        }

        public void Add(byte[] value)
        {
            throw new NotImplementedException();
        }
    }
}
