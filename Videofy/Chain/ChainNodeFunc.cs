using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Videofy.Chain
{

    delegate void NodeFunc(byte[] valueIn, int size, byte[] valueOut);

}
