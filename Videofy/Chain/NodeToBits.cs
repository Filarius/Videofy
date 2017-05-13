using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Videofy.Chain
{
    class NodeToBits : ChainNode
    {
        private NodeToken token;
        public NodeToBits(IPipe input, IPipe output,NodeToken token) : base(input, output)
        {
            this.token = token;
        }

        public override void Start()
        {

            //debug
            int cnt = 0;
            //debug
            while (true)
            {
                if(token.token)
                {
                    break;
                }
                byte[] temp = Input.Take();
                if (temp == null)
                    break;

                byte[] result = new byte[temp.Length * 8];
                for (int i = 0; i < temp.Length; i++)
                {
                    byte m = temp[i];
                    for (int j = 7; j >= 0; j--)
                    {
                        result[i * 8 + j] = (byte)(m % 2);
                        m = (byte)(m / 2);
                    }
                }

                Output.Add(result);
            }
            Output.Complete();

            
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
