using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Videofy.Chain.Types;
using Videofy.Main;

namespace Videofy.Chain
{
    class NodeBlocksFromFrame : ChainNode
    {
        private DFFrame Frame;
        private OptionsStruct opts;
        private NodeToken token;

        public NodeBlocksFromFrame(OptionsStruct opts, IPipe input, IPipe output, NodeToken token) : base(input, output)
        {
            this.opts = opts;
            this.token = token;
        }

        public override void Start()
        {
            while ((Input.IsOpen) | (Input.Count > 0))
            {
                if (token.token)
                {
                    break;
                }
                Frame = new DFFrame(opts);
                byte[] temp = Input.Take(Frame.Size);
                Frame.FromArray(temp);
                while ((!Frame.IsFull))
                {
                    if (token.token)
                    {
                       break;
                    }
                    temp = Frame.GetBlockArray();
                    Output.Add(temp);
                }
                Frame.Free();
            }
            Output.Complete();
        }
    }
}
