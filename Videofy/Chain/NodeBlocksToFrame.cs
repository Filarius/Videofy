using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Videofy.Main;
using Videofy.Chain.Types;

namespace Videofy.Chain
{
    class NodeBlocksToFrame : ChainNode
    {
        private DFFrame Frame;
        private OptionsStruct opts;

        public NodeBlocksToFrame(OptionsStruct opts, Pipe input, Pipe output) : base(input, output)
        {
            this.opts = opts;            
        }

        public override void Start()
        {
            while ((Input.IsOpen) | (Input.Count > 0))
            {
                Frame = new DFFrame(opts);
                while (!Frame.IsFull)
                {
                    byte[] temp = Input.Take(64);
                    Frame.SetBlockArray(temp);
                }
                Output.Add(Frame.ToArray());
                Frame.Free();
            }
            Output.Complete();
        }
    }
}
