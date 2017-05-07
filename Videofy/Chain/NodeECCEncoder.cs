using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Videofy.Chain.Helpers;

namespace Videofy.Chain
{
    class NodeECCEncoder : ChainNode
    {
        private ReedSolomon ecc;

        public NodeECCEncoder(IPipe Input, IPipe Output):base(Input,Output)
        {
            ecc = new ReedSolomon();
        }

        public override void Start()
        {
            while(Input.IsOpen | (Input.Count > 0))
            {
                byte[] temp = Input.Take(255 - 40);
                temp = ecc.Encode(temp, 40);
                Output.Add(temp);
            }
            Output.Complete();
        }
    }
}
