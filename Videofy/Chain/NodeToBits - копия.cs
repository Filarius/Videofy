using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Videofy.Chain
{
    class NodeToBits : ChainNode
    {

        public NodeToBits(Pipe input, Pipe output) : base(input, output)
        {

        }

        public override void Start()
        {
            while (true)
            {
                byte[] temp = Input.Take();
                if (temp == null)
                    break;

                var list = new List<byte>();
                for(int i = 0; i < temp.Length; i++)
                {
                    list.AddRange(
                             ByteToBits(
                                        temp[i]
                                       )
                            );
                }
                Output.Add(list.ToArray());
            }
        }

        private byte[] ByteToBits(byte data)
        {
            byte b = data;
            var tmp = new List<byte>();
            for (int j = 0; j < 8; j++)
            {
                tmp.Add((byte)(b % 2));
                b /= 2;
            }
            tmp.Reverse();
            return tmp.ToArray();
        }
    }
}
