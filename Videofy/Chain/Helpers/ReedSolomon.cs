using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing.Common.ReedSolomon;

namespace Videofy.Chain.Helpers
{
    class  ReedSolomon
    {
        private GenericGF field;
        private ReedSolomonEncoder encoder;
        private ReedSolomonDecoder decoder;

        public ReedSolomon()
        {           
            field = GenericGF.DATA_MATRIX_FIELD_256;
            encoder = new ReedSolomonEncoder(field);
            decoder = new ReedSolomonDecoder(field);
        }

        public byte[] Encode(byte[] data, int parity)
        {            
            int[] intar = new int[data.Length + parity];
            for(int i=0;i<data.Length;i++)
            {
                intar[i] = data[i];
            }
            encoder.encode(intar, parity);
            var result = new byte[intar.Length];
            for (int i = 0; i < intar.Length; i++)
            {
                result[i] = (byte)intar[i];
            }
            return result;
        }

        public byte[] Decode(byte[] data,int parity)
        {
            int[] intar = new int[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                intar[i] = data[i];
            }
            decoder.decode(intar, parity);

            var result = new byte[data.Length - parity];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = (byte)intar[i];
            }
            return result;
        }

    }
}
