using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Videofy.Main;
using System.Threading;


namespace Videofy.Chain
{
    class NodeBitsToBlock:NodeBlockBase
    {

        public NodeBitsToBlock(OptionsStruct opt,IPipe input,IPipe output, NodeToken token)
            :base(opt,input,output, token)
        {

        }

        public override void Start()
        {
            StartBitsToBlock();
        }
    }
}
