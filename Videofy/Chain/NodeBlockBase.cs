﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp.CPlusPlus;
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
        private float[,] dctarray;
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
            dctarray = new float[8, 8];

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
                DCT.Dispose();
                mat.Dispose();
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

        private byte[] BitsFromCell(float value)
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
            byte cell = (byte)Math.Round(BitsToCell(bits), MidpointRounding.AwayFromZero);
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

            float sum = 0;
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
            dctarray[0, 0] = 1024;

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
            DFFrameBlock blok = new DFFrameBlock(ar);
            Mat mat = block.Body.Idct();
            mat.ConvertTo(blok.Body, MatType.CV_8U);
            mat.Dispose();
            Output.Add(blok.ToArray());
            blok.Free();
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
        }

        protected void StartBitsToBlock()
        {
            //DEBUG
            int cnt = 0;
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
                    if(token.token)
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



    }
}
