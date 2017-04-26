using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Videofy.Chain
{
    class NodeFromBits : ChainNode
    {
        public NodeFromBits(IPipe input, IPipe output) : base(input, output)
        {

        }

        public override void Start()
        {
            //DEBUG
            int q = 0;
            //DEBUG
            while ((Input.Count > 0) | (Input.IsOpen))
            {
                byte[] temp = Input.Take(8);
                if (temp == null)
                    break;

                //DEBUG
                int r = Debug.i;
                if (q > 939200)
                    if (q % 100 == 0)
                    {
             //           Console.WriteLine(q);
                    }
                q++;
                //DEBUG

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
