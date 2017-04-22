using OpenCvSharp.CPlusPlus;
using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Threading.Tasks.Dataflow;
using Videofy.Main;


namespace Videofy.Dataflow
{
    class ByteAssembler_
    {
        private OptionsStruct options;

        private byte[] ByteAssembler(byte[] data)
        {
            if (data == null) throw new ArgumentNullException();
            if ((data.Length % 8) != 0) throw new ArgumentException();
            byte cnt = 0;
            byte tmp = 0;
            var lst = new List<byte>();
            for (int i = 0; i < data.Length; i++)
            {
                tmp *= 2;
                tmp += data[i];
                cnt += 1;
                if (cnt == 8)
                {
                    lst.Add(tmp);
                    cnt = 0;
                    tmp = 0;
                }
            }
            return lst.ToArray();
        }

        private byte[] ByteDisassembler(byte[] data)
        {
            if (data == null) throw new ArgumentNullException();
            var lst = new List<byte>();
            for (int i = 0; i < data.Length; i++)
            {
                byte b = data[i];
                var tmp = new List<byte>();
                for (int j = 0; j < 8; j++)
                {
                    tmp.Add((byte)(b % 2));
                    b /= 2;
                }
                tmp.Reverse();
                lst.AddRange(tmp);
            }
            return lst.ToArray();
        }




        private int[,] _snakeIteratorCore = new int[64, 2];

        private void SnakeIteratorInit()
        {
            int dx, dy, x, y, k;
            x = y = k = 0;
            dx = dy = 0;
            while (k < 64)
            {
                x += dx;
                y += dy;
                if (k > 26)
                {
                    k = k;
                }
                _snakeIteratorCore[k, 0] = x;
                _snakeIteratorCore[k, 1] = y;
                if ((x == 0) & (y == 0))
                {
                    dx = 1;
                    dy = 0;
                }
                else if ((y == 0) & (x < 7) & (dy == -1))
                {
                    dx = 1;
                    dy = 0;
                }
                else if ((y == 0) & (x < 7) & (dx == 1) & (dy == 0))
                {
                    dx = -1;
                    dy = 1;
                }
                else if ((x == 7) & (dx == 1) & (dy == -1))
                {
                    dx = 0;
                    dy = 1;
                }
                else if ((x == 7) & (dx == 0) & (dy == 1))
                {
                    dx = -1;
                    dy = 1;
                }
                else if ((x == 7) & (dx == 1) & (dy == 0))
                {
                    dx = -1;
                    dy = 1;
                }
                else if ((x == 0) & (y < 7) & (dx == -1) & (dy == 1))
                {
                    dx = 0;
                    dy = 1;
                }
                else if ((x == 0) & (y < 7) & (dx == 0) & (dy == 1))
                {
                    dx = 1;
                    dy = -1;
                }
                else if ((x < 7) & (y == 7) & (dx == -1) & (dy == 1))
                {
                    dx = 1;
                    dy = 0;
                }
                else if ((x < 7) & (y == 7) & (dx == 1) & (dy == 0))
                {
                    dx = 1;
                    dy = -1;
                }
                k++;
            }
        }

        private void SnakeArraySet(float[,] array, int z, float value)
        {
            array
                [
                _snakeIteratorCore[z, 0]
                ,
                _snakeIteratorCore[z, 1]
                ]
                = value;
        }

        private float SnakeArrayGet(float[,] array, int z)
        {
            return
              array
                [
                _snakeIteratorCore[z, 0]
                ,
                _snakeIteratorCore[z, 1]
                ];
        }

        private int DCTBoundsFind()
        {
            Mat main;// = Mat.Zeros(new Size(8,8),MatType.CV_32F);
            //var mainInd = main.GetGenericIndexer<float>();
            int lastB = 0;
            int db = 256;
            double min, max;
            Boolean itsOkay;
            while (db > 0)
            {
                int nextB = lastB + db;
                float[,] core = new float[8, 8];
                core[0, 0] = 1024;
                for (int i = 1; i < options.cellCount; i++)
                {
                    SnakeArraySet(core, i, nextB);
                }
                Mat DCT = new Mat(8, 8, MatType.CV_32F, core);
                Mat mat = DCT.Idct();
                mat.MinMaxIdx(out min, out max);
                itsOkay = (min >= 0) & (max < 256);
                if (itsOkay)
                {
                    core[0, 0] = 1024;
                    for (int i = 1; i < options.cellCount; i++)
                    {
                        SnakeArraySet(core, i, -nextB);
                    }
                    DCT = new Mat(8, 8, MatType.CV_32F, core);
                    mat = DCT.Idct();
                    mat.MinMaxIdx(out min, out max);
                    itsOkay = (min >= 0) & (max < 256);
                }

                if (itsOkay)
                {
                    lastB = nextB;
                }
                else
                {
                    db = db / 2;
                }
            }
            if (lastB == 0) throw new ArgumentOutOfRangeException();
            return lastB;
        }

        private int[] _bounds;

        private void BoundsInit()
        {
            if (options.cellCount < 1) throw new ArgumentOutOfRangeException();
            if (options.cellCount > 64) throw new ArgumentOutOfRangeException();
            _bounds = new int[2];
            if (options.cellCount == 1)
            {
                //for plain transcoder                
                _bounds[0] = 0;
                _bounds[1] = 255;
            }
            else
            {
                //for  DCT transcoder
                _bounds[0] = -DCTBoundsFind();
                _bounds[1] = -_bounds[0];
            }
        }

        //covert bits into value within bounds
        private float BitsToCell(IList<byte> bits)
        {
            if (bits.Count != options.density) throw new ArgumentOutOfRangeException();
            float tmp = 0;
            float max = (1 << options.cellCount) - 1;
            foreach(byte bit in bits)
            {
                tmp = tmp * 2;
                tmp = tmp + bit;
            }
            tmp = tmp / max; //normalize
            tmp = tmp * (_bounds[1] - _bounds[0]) + _bounds[0]; // stretch to bounds            
            return tmp;
        }

        //convert bounded cell value into bits
        private byte[] BitsFromCell(float value)
        {
            //normalize
            float tmp;
            float max = (1 << options.density) - 1;
            tmp = value + _bounds[0];
            tmp = tmp / (_bounds[1] - _bounds[0]);
            if (tmp < 0) { tmp = 0; }
            else if (tmp > 1) { tmp = 1; }

            tmp = tmp * max; //stretch

            //fix value shift
            int result = (int)Math.Round(tmp);
            float error = (result - tmp) * 2; //normalized value shift
            ErrorShiftProcessor(error);

            byte[] array = new byte[options.density];
            for(int i = options.density - 1; i >= 0 ; i--)
            {
                array[i] = (byte)(result % 2);
                result = result / 2;
            }

            return array;
        }


        private float _maxError = 0;
        //handle information about value shift
        private void ErrorShiftProcessor(float errorShift)
        {
            float tmp = Math.Abs(errorShift);
            if (tmp > 1) throw new ArgumentOutOfRangeException();
            if(_maxError < errorShift)
            {
                _maxError = errorShift;
            }
        }
                    
        

        /*
        private Mat _DCTCoreFirst;
        private Mat _DCTCoreSnake;
        */


            

        

        public ByteAssembler_(OptionsStruct options)
        {
           // Func<byte>
        }
    }
}
