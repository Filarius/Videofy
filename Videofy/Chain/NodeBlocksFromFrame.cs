using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Videofy.Chain.Types;
using Videofy.Main;

namespace Videofy.Chain
{
    class NodeBlocksFromFrame : ChainNode
    {
        private DFFrame Frame;
        private OptionsStruct opts;

        public NodeBlocksFromFrame(OptionsStruct opts, Pipe input, Pipe output) : base(input, output)
        {
            this.opts = opts;
        }

        public override void Start()
        {
            while ((Input.IsOpen) | (Input.Count > 0))
            {
                Frame = new DFFrame(opts);
                byte[] temp = Input.Take(Frame.Size);
                Frame.FromArray(temp);
                while (!Frame.IsFull)
                {
                    temp = Frame.GetBlockArray();
                    Output.Add(temp);

                }                
                Frame.Free();
            }
            Output.Complete();
        }
    }
}
