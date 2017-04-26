using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Videofy.Main;
using Videofy.Chain.Helpers;

namespace Videofy.Chain
{

    //send header data bits as simplest plain blocks    
    class NodeMetaHeaderToBlocks:ChainNode
    {
        private OptionsStruct opt;
        

        public OptionsStruct Options { get { return opt; } }
        public IPipe PipeOut { get { return Input; } }

        
        public NodeMetaHeaderToBlocks(string path,OptionsStruct opt, IPipe output):base(null,output)
        {
            this.opt = opt;              

        }

        public override void Start()
        {
            Pipe inp = new Pipe(PipeOut.Token);
            Pipe outp = new Pipe(PipeOut.Token);
            NodeToBits bitNode = new NodeToBits(inp, outp);
            NodeBitsToBlock blockNode = new NodeBitsToBlock(opt, outp, Output);
                       

        }

    }
}
