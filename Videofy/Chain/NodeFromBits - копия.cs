using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Videofy.Chain
{
    class NodeFromBits : ChainNode
    {
        public NodeFromBits(Pipe input, Pipe output) : base(input, output)
        {

        }

        public override void Start()
        {
            while (true)
            {
                byte[] temp = Input.Take(8);
                if (temp == null)
                    break;

                byte cnt = 0;
                byte tmp = 0;
                var lst = new List<byte>();
                for (int i = 0; i < temp.Length; i++)
                {
                    tmp *= 2;
                    tmp += temp[i];
                    cnt += 1;
                    if (cnt == 8)
                    {
                        lst.Add(tmp);
                        cnt = 0;
                        tmp = 0;
                    }
                }
                Output.Add(lst.ToArray());
            }
        }
    }
}
