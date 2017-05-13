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
        NodeToken token;

        public NodeBlocksToFrame(OptionsStruct opts, IPipe input, IPipe output,NodeToken token) : base(input, output)
        {
            this.opts = opts;
            this.token = token;
        }

        public override void Start()
        {
            while ((Input.IsOpen) | (Input.Count > 0))
            {
                if (token.token)
                { break; }
                Frame = new DFFrame(opts);
                while (!Frame.IsFull)
                {
                    if (token.token)
                    { break; }
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
