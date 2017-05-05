using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Videofy.Chain.Helpers
{
    class WorkMonitor
    {
        public int TotalWork;
        public int CurrentWork;
        public byte Percent
        {
            get
            {
                if (TotalWork == 0)
                    return 0;
                return (byte)Math.Truncate(100.0 * CurrentWork / TotalWork);
            }
        }
        public WorkMonitor()
        {
            TotalWork = 0;
            CurrentWork = 0;
        }

        public void Add(int inc)
        {
            CurrentWork += inc;
        }
    }
}
