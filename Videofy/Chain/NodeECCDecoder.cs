using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Videofy.Chain.Helpers;

namespace Videofy.Chain
{
    class NodeECCDecoder:ChainNode
    {
        private ReedSolomon ecc;

        public NodeECCDecoder(IPipe Input, IPipe Output):base(Input,Output)
        {
            ecc = new ReedSolomon();
        }

        public override void Start()
        {
            while (Input.IsOpen | (Input.Count > 0))
            {
                byte[] temp = Input.Take(256);
                temp = ecc.Decode(temp, 100);
                Output.Add(temp);
            }
            Output.Complete();
        }
    }
}
