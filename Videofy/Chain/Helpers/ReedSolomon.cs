using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing.Common.ReedSolomon;

namespace Videofy.Chain.Helpers
{
    class ReedSolomon
    {
        public byte[] Encode(byte depth,byte[] data, int parity)
        {
            GenericGF field = GenericGF.DATA_MATRIX_FIELD_256;
            ReedSolomonEncoder encoder = new ReedSolomonEncoder(field);


        }
    }
}
