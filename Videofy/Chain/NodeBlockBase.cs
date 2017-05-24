using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using Videofy.Main;
using Videofy.Chain.Types;
using System.Threading;

namespace Videofy.Chain
{
    class NodeBlockBase : ChainNode
    {
        private OptionsStruct options;
        private int[] valueBounds;
        private int[,] snakeIteratorCore;
        private DFFrameBlock block;
        private byte[] blockarray;
        //private float[,] dctarray;
        private int[] dctarray;
        private NodeToken token;

        public NodeBlockBase(OptionsStruct options, IPipe input, IPipe output, NodeToken token) : base(input, output)
        {
            this.options = options;
            this.token = token;
            valueBounds = new int[2];
            snakeIteratorCore = new int[64, 2];
            SnakeIteratorInit();
            BoundsInit();
            blockarray = new byte[64];
            //dctarray = new float[8, 8];
            dctarray = new int[64];

            MaxError = 0;
        }

        private void SnakeIteratorInit()
        {
            int dx, dy, x, y, k;
            x = y = k = 0;
            dx = dy = 0;
            while (k < 64)
            {
                x += dx;
                y += dy;
                snakeIteratorCore[k, 0] = x;
                snakeIteratorCore[k, 1] = y;
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

        /*
        private void SnakeArraySet(float[,] array, int z, float value)
        {
            array
                [
                snakeIteratorCore[z, 0]
                ,
                snakeIteratorCore[z, 1]
                ]
                = value;
        }
        */
        private void SnakeArraySet(int [] array, int z, int value)
        {
            array
                [
                8*snakeIteratorCore[z, 0]
                +
                snakeIteratorCore[z, 1]
                ]
                = value;
        }
        /*
        private float SnakeArrayGet(float[,] array, int z)
        {
            return
              array
                [
                snakeIteratorCore[z, 0]
                ,
                snakeIteratorCore[z, 1]
                ];
        }
        */

        private int SnakeArrayGet(int[] array, int z)
        {
            return
              array
                [
                8*snakeIteratorCore[z, 0]
                +
                snakeIteratorCore[z, 1]
                ];
        }


        private int DCTBoundsFind()
        {           
            int lastB = 0;
            int db = 256;
            double min, max;
            Boolean itsOkay;
            while (db > 0)
            {
                int nextB = lastB + db;
                //float[,] core = new float[8, 8];
                int[] core = new int[64];
                //core[0, 0] = 1024;
                core[0] = 1024;
                for (int i = 1; i < options.cellCount; i++)
                {
                    SnakeArraySet(core, i, nextB);
                }
                Mat DCT = new Mat(8, 8, MatType.CV_32F, core);
                Mat mat = DCT.Idct();
                mat.MinMaxIdx(out min, out max);
                DCT.Dispose();
                mat.Dispose();
                itsOkay = (min >= 0) & (max < 256);
                if (itsOkay)
                {
                    core[0] = 1024;
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

        private void BoundsInit()
        {
            if (options.cellCount < 1) throw new ArgumentOutOfRangeException();
            if (options.cellCount > 64) throw new ArgumentOutOfRangeException();
            if (options.cellCount == 1)
            {
                //for plain transcoder                
                valueBounds[0] = 0;
                valueBounds[1] = 255;
            }
            else
            {
                //for  DCT transcoder
                valueBounds[0] = -DCTBoundsFind();
                valueBounds[1] = -valueBounds[0];
            }
        }

        /*
        private float BitsToCell(byte[] bits)
        {
            if (bits.Length != options.density) throw new ArgumentOutOfRangeException();
            float tmp = 0;
            float max = (1 << options.density) - 1;
            foreach (byte bit in bits)
            {
                tmp = tmp * 2;
                tmp = tmp + bit;
            }
            tmp = tmp / max; //normalize
            tmp = tmp * (valueBounds[1] - valueBounds[0]) + valueBounds[0]; // stretch to bounds            
            return tmp;
        }
        */

        private int BitsToCell(byte[] bits)
        {
            if (bits.Length != options.density) throw new ArgumentOutOfRangeException();
            float tmp = 0;
            float max = (1 << options.density) - 1;
            foreach (byte bit in bits)
            {
                tmp = tmp * 2;
                tmp = tmp + bit;
            }
            tmp = tmp / max; //normalize
            tmp = tmp * (valueBounds[1] - valueBounds[0]) + valueBounds[0]; // stretch to bounds            
            return (int)Math.Round(tmp);
        }



        //private byte[] BitsFromCell(float value)
        private byte[] BitsFromCell(int value)
        {
            //normalize
            float tmp;
            float max = (1 << options.density) - 1;
            tmp = value - valueBounds[0];
            tmp = tmp / (valueBounds[1] - valueBounds[0]);
            if (tmp < 0) { tmp = 0; }
            else if (tmp > 1) { tmp = 1; }

            tmp = tmp * max; //stretch

            //fix value shift
            int result = (int)Math.Round(tmp);
            float error = (result - tmp) * 2; //normalized value shift
            ErrorShiftProcessor(error);

            byte[] array = new byte[options.density];
            for (int i = options.density - 1; i >= 0; i--)
            {
                array[i] = (byte)(result % 2);
                result = result / 2;
            }

            return array;
        }

        private float MaxError { get; set; }
        //handle information about value shift
        private void ErrorShiftProcessor(float errorShift)
        {
            float tmp = Math.Abs(errorShift);
            if (tmp > 1) throw new ArgumentOutOfRangeException();
            if (MaxError < errorShift)
            {
                MaxError = errorShift;
            }
        }


        private void PlainTransformBitsToBlock()
        {

            byte[] bits = Input.Take(options.density);
            //byte cell = (byte)Math.Round(BitsToCell(bits), MidpointRounding.AwayFromZero);
            byte cell = (byte)(BitsToCell(bits));
            byte[] block = new byte[64];
            for (int i = 0; i < 64; i++)
            {
                block[i] = cell;
            }
            Output.Add(block);
        }

        private void PlainTransformBitsFromBlock()
        {
            byte[] block = Input.Take(64);

            //float sum = 0;
            int sum = 0;
            for (int i = 0; i < 64; i++)
            {
                sum += block[i];
            }
            sum = sum / 64;
            byte[] bits = BitsFromCell(sum);
            // Debug.i += bits.Length;
            Output.Add(bits);
        }

        private void DCTTransformBitsToBlock()
        {
            int i;
            //dctarray[0, 0] = 1024;
            dctarray[0] = 1024;

            for (i = 1; i < options.cellCount + 1; i++)
            {
                byte[] temp = Input.Take(options.density);

                SnakeArraySet(dctarray, i, BitsToCell(temp));
            }
            for (i = (options.cellCount) + 1; i < 64; i++)
            {
                SnakeArraySet(dctarray, i, 0);
            }


            byte[] ar = new byte[64];

            /*
            byte[] ar = new byte[64];
            DFFrameBlock blok = new DFFrameBlock(ar);
            
            Mat mat = null;

            mat = block.Body.Idct();
            
            mat.ConvertTo(blok.Body, MatType.CV_8U);
            mat.Dispose();
            

            Output.Add(blok.ToArray());
            blok.Free();
            */
            
        }

        private void DCTTransformBitsFromBlock()
        {
            byte[] temp = Input.Take(64);
            
            DFFrameBlock blok = new DFFrameBlock(temp);
            Mat mat = new Mat();
            blok.Body.ConvertTo(mat, MatType.CV_32FC1);
            mat = mat.Dct();
            mat.CopyTo(block.Body);
            for (int i = 1; i < options.cellCount + 1; i++)
            {
                Output.Add(BitsFromCell(SnakeArrayGet(dctarray, i)));
            }
            mat.Dispose();
            blok.Free();
            
            /*
            var data = new float[8, 8];
            int k = 0;
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    data[x, y] = temp[k++];
                }
            }
            var dct = DCT2D(data);
            var result1d = new float[64];
            k = 0;                
            for (int i = 1; i < options.cellCount + 1; i++)
            {
                Output.Add(BitsFromCell(SnakeArrayGet(dct, i)));
            }
            */

        }

        protected void StartBitsToBlock()
        {
            //DEBUG
            int cnt =1;
            if (options.density == 1)
            {
                int a = 89;
            }
            //DEBUG
            if (options.cellCount == 1)
            {
                while ((Input.Count > 0) | (Input.IsOpen)) // pipe have data or not closed
                {
                    //DEBUG
                    if (cnt > 1878540)
                        if (cnt % 1 == 0)
                            //      Console.WriteLine(cnt);
                            cnt += 1;
                    //DEBUG
                    PlainTransformBitsToBlock();
                }
            }
            else
            {

                block = new DFFrameBlock(dctarray);
                while ((Input.Count > 0) | (Input.IsOpen)) // pipe have data or not closed
                {
                    DCTTransformBitsToBlock();
                }
                block.Free();
            }
            Output.Complete();
        }

        protected void StartBitFromBlock()
        {
            //DEBUG
            int i = 0;
            //DEBUG
            if (options.cellCount == 1)
            {
                while ((Input.Count > 0) | (Input.IsOpen)) // pipe have data or not closed
                {
                    if (token.token)
                    {
                        break;
                    }

                    PlainTransformBitsFromBlock();
                }
            }
            else
            {
                block = new DFFrameBlock(dctarray);
                while ((Input.Count > 0) | (Input.IsOpen)) // pipe have data or not closed
                {
                    if (token.token)
                    {
                        break;
                    }
                    DCTTransformBitsFromBlock();
                }
                block.Free();
            }
            Output.Complete();
            //Console.WriteLine(Debug.i.ToString());
        }

        public float[,] DCT2D(float[,] input)
        {
            float[,] coeffs = new float[8, 8];
            for (int u = 0; u < 8; u++)
            {
                for (int v = 0; v < 8; v++)
                {
                    double sum = 0d;
                    for (int x = 0; x < 8; x++)
                    {
                        for (int y = 0; y < 8; y++)
                        {
                            double a = input[x, y];
                            sum += BasisFunction(a, u, v, x, y);
                        }
                    }
                    coeffs[u, v] = (float)(sum * beta * alpha(u) * alpha(v));
                }
            }
            return coeffs;
        }

        public float[,] IDCT2D(float[,] coeffs)
        {
            float[,] output = new float[8, 8];
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    double sum = 0d;

                    for (int u = 0; u < 8; u++)
                    {
                        for (int v = 0; v < 8; v++)
                        {
                            double a = coeffs[u, v];
                            sum += BasisFunction(a, u, v, x, y) * alpha(u) * alpha(v);
                        }
                    }
                    output[x, y] = (float)(sum * beta);
                }
            }
            return output;
        }

        public double BasisFunction(double a, double u, double v, double x, double y)
        {
            double b = Math.Cos(((2d * x + 1d) * u * Math.PI) / (2 * 8));
            double c = Math.Cos(((2d * y + 1d) * v * Math.PI) / (2 * 8));
            return a * b * c;
        }

        private double alpha(int u)
        {
            if (u == 0)
                return 1 / Math.Sqrt(2);
            return 1;
        }

        private double beta
        {
            get { return (1d / 4); }
        }


        private void pixel_sub_wxh(
            ref Int32[] diff,
            int isize,
            byte[] pix1,
            byte[] pix2
            )
        {
            int ipix1 = 16;
            int ipix2 = 32;
            int d1 = 0;
            int d2 = 0;
            for (int y = 0; y < isize; y++)
            {
                for (int x = 0; x < isize; x++)
                    diff[x + y * isize] = pix1[x+d1] - pix2[x+d2];
                d1 += ipix1;
                d2 += ipix2;
            }
        }

        private  byte ClipByte(int a)
        {
            return (a < 0) ? (byte)0 : ((a>255) ? (byte)255 : (byte)a);
        }

        private void DCT8x8(byte[] data,ref int[] result)
        {
            int i;
            int[] tmp = new int[64];
            int[] pTmp = tmp;
            int a0, a1, a2, a3;
            int p0, p1, p2, p3, p4, p5, p6, p7;
            int b0, b1, b2, b3, b4, b5, b6, b7;
            // Horizontal
            for (i = 0; i < 8; i++)
            {
                p0 = data[i * 8 + 0];
                p1 = data[i * 8 + 1];
                p2 = data[i * 8 + 2];
                p3 = data[i * 8 + 3];
                p4 = data[i * 8 + 4];
                p5 = data[i * 8 + 5];
                p6 = data[i * 8 + 6];
                p7 = data[i * 8 + 7];

                a0 = p0 + p7;
                a1 = p1 + p6;
                a2 = p2 + p5;
                a3 = p3 + p4;

                b0 = a0 + a3;
                b1 = a1 + a2;
                b2 = a0 - a3;
                b3 = a1 - a2;

                a0 = p0 - p7;
                a1 = p1 - p6;
                a2 = p2 - p5;
                a3 = p3 - p4;

                b4 = a1 + a2 + ((a0 >> 1) + a0);
                b5 = a0 - a3 - ((a2 >> 1) + a2);
                b6 = a0 + a3 - ((a1 >> 1) + a1);
                b7 = a1 - a2 + ((a3 >> 1) + a3);

                tmp[i * 8 + 0] = b0 + b1;
                tmp[i * 8 + 1] = b4 + (b7 >> 2);
                tmp[i * 8 + 2] = b2 + (b3 >> 1);
                tmp[i * 8 + 3] = b5 + (b6 >> 2);
                tmp[i * 8 + 4] = b0 - b1;
                tmp[i * 8 + 5] = b6 - (b5 >> 2);
                tmp[i * 8 + 6] = (b2 >> 1) - b3;
                tmp[i * 8 + 7] = (b4 >> 2) - b7;
            }

            // Vertical 
            for (i = 0; i < 8; i++)
            {

                p0 = tmp[0 * 8 + i];
                p1 = tmp[1 * 8 + i];
                p2 = tmp[2 * 8 + i];
                p3 = tmp[3 * 8 + i];
                p4 = tmp[4 * 8 + i];
                p5 = tmp[5 * 8 + i];
                p6 = tmp[6 * 8 + i];
                p7 = tmp[7 * 8 + i];

                a0 = p0 + p7;
                a1 = p1 + p6;
                a2 = p2 + p5;
                a3 = p3 + p4;

                b0 = a0 + a3;
                b1 = a1 + a2;
                b2 = a0 - a3;
                b3 = a1 - a2;

                a0 = p0 - p7;
                a1 = p1 - p6;
                a2 = p2 - p5;
                a3 = p3 - p4;

                b4 = a1 + a2 + ((a0 >> 1) + a0);
                b5 = a0 - a3 - ((a2 >> 1) + a2);
                b6 = a0 + a3 - ((a1 >> 1) + a1);
                b7 = a1 - a2 + ((a3 >> 1) + a3);

                
                result[0*8 + i] = b0 + b1;
                result[1*8 + i] = b4 + (b7 >> 2);
                result[2*8 + i] = b2 + (b3 >> 1);
                result[3*8 + i] = b5 + (b6 >> 2);
                result[4*8 + i] = b0 - b1;
                result[5*8 + i]= b6 - (b5 >> 2);
                result[6*8 + i] = (b2 >> 1) - b3;
                result[7*8 + i] = (b4 >> 2) - b7;
            }

        }

        private void IDCT(int[] data, ref byte[] result)
        {
            int[] tmp = new int[64];
            int a0, a1, a2, a3;
            int p0, p1, p2, p3, p4, p5, p6, p7;
            int b0, b1, b2, b3, b4, b5, b6, b7;
            int i;
            // Horizontal  
            for (i = 0; i < 8; i++)
            {                
                p0 = data[i * 8 + 0];
                p1 = data[i * 8 +1];
                p2 = data[i * 8 +2];
                p3 = data[i * 8 +3];
                p4 = data[i * 8 +4];
                p5 = data[i * 8 +5];
                p6 = data[i * 8 +6];
                p7 = data[i * 8 +7];

                a0 = p0 + p4;
                a1 = p0 - p4;
                a2 = p6 - (p2 >> 1);
                a3 = p2 + (p6 >> 1);

                b0 = a0 + a3;
                b2 = a1 - a2;
                b4 = a1 + a2;
                b6 = a0 - a3;

                a0 = -p3 + p5 - p7 - (p7 >> 1);
                a1 = p1 + p7 - p3 - (p3 >> 1);
                a2 = -p1 + p7 + p5 + (p5 >> 1);
                a3 = p3 + p5 + p1 + (p1 >> 1);


                b1 = a0 + (a3 >> 2);
                b3 = a1 + (a2 >> 2);
                b5 = a2 - (a1 >> 2);
                b7 = a3 - (a0 >> 2);

                

                tmp[i * 8 + 0] = b0 + b7;
                tmp[i * 8 + 1] = b2 - b5;
                tmp[i * 8 + 2] = b4 + b3;
                tmp[i * 8 + 3] = b6 + b1;
                tmp[i * 8 + 4] = b6 - b1;
                tmp[i * 8 + 5] = b4 - b3;
                tmp[i * 8 + 6] = b2 + b5;
                tmp[i * 8 + 7] = b0 - b7;
            }

            for (i = 0; i < 8; i++)
            {                
                p0 = tmp[0*8 + i];
                p1 = tmp[1*8 + i];
                p2 = tmp[2*8 + i];
                p3 = tmp[3*8 + i];
                p4 = tmp[4*8 + i];
                p5 = tmp[5*8 + i];
                p6 = tmp[6*8 + i];
                p7 = tmp[7*8 + i];

                a0 = p0 + p4;
                a1 = p0 - p4;
                a2 = p6 - (p2 >> 1);
                a3 = p2 + (p6 >> 1);

                b0 = a0 + a3;
                b2 = a1 - a2;
                b4 = a1 + a2;
                b6 = a0 - a3;

                a0 = -p3 + p5 - p7 - (p7 >> 1);
                a1 = p1 + p7 - p3 - (p3 >> 1);
                a2 = -p1 + p7 + p5 + (p5 >> 1);
                a3 = p3 + p5 + p1 + (p1 >> 1);


                b1 = a0 + (a3 >> 2);
                b7 = a3 - (a0 >> 2);
                b3 = a1 + (a2 >> 2);
                b5 = a2 - (a1 >> 2);


                result[0*8 + i] = ClipByte(b0 + b7);
                result[1*8 + i] = ClipByte(b2 - b5);
                result[2*8 + i] = ClipByte(b4 + b3);
                result[3*8 + i] = ClipByte(b6 + b1);
                result[4*8 + i] = ClipByte(b6 - b1);
                result[5*8 + i] = ClipByte(b4 - b3);
                result[6*8 + i] = ClipByte(b2 + b5);
                result[7*8 + i] = ClipByte(b0 - b7);
            }
        }
        
    }
}
