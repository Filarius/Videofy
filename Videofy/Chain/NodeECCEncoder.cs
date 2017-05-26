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
        private NodeToken token;

        public NodeECCEncoder(IPipe Input, IPipe Output, NodeToken token):base(Input,Output)
        {
            this.token = token;
            ecc = new ReedSolomon();
        }

        public override void Start()
        {
            while(Input.IsOpen | (Input.Count > 0))
            {
                if(token.token)
                { break; }
                byte[] temp = Input.Take(250-100);
                temp = ecc.Encode(temp, 100);
                Output.Add(temp);
            }
            Output.Complete();
        }
    }
}
