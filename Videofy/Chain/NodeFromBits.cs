using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Videofy.Chain
{
    class NodeFromBits : ChainNode
    {
        private NodeToken token;

        public NodeFromBits(IPipe input, IPipe output, NodeToken token) : base(input, output)
        {
            this.token = token;
        }

        public override void Start()
        {
            while ((Input.Count > 0) | (Input.IsOpen))
            {
                if (token.token)
                {
                    break;
                }
                
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
            Output.Complete();
            // Console.WriteLine(Debug.i.ToString());
        }
    }
}
