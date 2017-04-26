using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Videofy.Chain
{
    interface IPipe
    {
        CancellationToken Token { get; }
        int Count { get; }
        void Complete();
        Boolean IsOpen { get; }
        byte[] Take();
        byte[] Take(int size);
        void Add(byte[] value);
    }
}
