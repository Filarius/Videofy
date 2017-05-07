using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Videofy.Main;

namespace Videofy.Chain
{
    class NodeBitsFromBlock : NodeBlockBase
    {
        
        public NodeBitsFromBlock(OptionsStruct opt, IPipe input, IPipe output, NodeToken token) 
            : base(opt, input, output, token)
        {

        }

        public override void Start()
        {
            StartBitFromBlock();
        }
    }
}
