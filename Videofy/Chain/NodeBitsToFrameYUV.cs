using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Videofy.Main;
using Videofy.Chain.Types;

namespace Videofy.Chain
{

    

    class NodeBitsToFrameYUV : NodeBlockBase
    {
        private int[] snakeIteratorCore4;
        private int maxDC4;
        private OptionsStruct options;
        private int[] valueBounds4;
        private int Count4, Count8;
        private DFFrameYUV Frame;
        private NodeToken token;

        public NodeBitsToFrameYUV(OptionsStruct opt, IPipe input, IPipe output, NodeToken token)
            : base(opt, input, output, token)
        {
            SnakeIteratorCore4Init();
            options = opt;
            valueBounds4 = new int[2];
        }

        private void SnakeIteratorCore4Init()
        {
            snakeIteratorCore4 = new int[4 * 4] { 0,1,4,8,5,2,3,6,9,12,13,10,7,11,14,15 };
        }

        private void SnakeArray4Set(int[] array, int z, int value)
        {
            array
                [ 
                snakeIteratorCore4[z]
                ]
                = value;
        }

        private int SnakeArray4Get(int[] array, int z)
        {
            return
              array
                [
                snakeIteratorCore4[z]
                ];
        }

        private int DCT4BoundsFind()
        {
            int lastB = 0;
            int db = 1024;
            //double min, max;
            int min = 0, max = 255;
            int newmin = 255, newmax = 0;
            int newmin2 = 255, newmax2 = 0;
            Boolean itsOkay;
            byte[] data = new byte[16];
            //get middle value of DC
            byte[] bt = new byte[16];
            for (int i = 0; i < 16; i++)
                bt[i] = 128;
            int[] result = new int[16];
            
            DCT4x4(bt, ref result);
            maxDC4 = result[0];

            while (db > 0)
            {
                int nextB = lastB + db;
                int[] core = new int[16];
                core[0] = maxDC4;

                for (int i = 1; i < options.cellCount; i++)
                {
                    SnakeArray4Set(core, i, nextB);
                }

                
                newmin = 255;
                newmax = 0;
                IDCT4x4(core, ref data);
                for (int i = 0; i < 16; i++)
                {
                    if (newmax < data[i]) { newmax = data[i]; }
                    if (newmin > data[i]) { newmin = data[i]; }
                }



                //itsOkay = (min >= 0) & (max < 256);
                itsOkay = !((min == newmin) | (max == newmax));

                if (itsOkay)
                {
                    core[0] = maxDC4;

                    for (int i = 1; i < options.cellCount; i++)
                    {
                        SnakeArray4Set(core, i, -nextB);
                    }
                    
                    newmin2 = 255;
                    newmax2 = 0;
                    IDCT4x4(core, ref data);
                    for (int i = 0; i < 16; i++)
                    {
                        if (newmax2 < data[i]) { newmax2 = data[i]; }
                        if (newmin2 > data[i]) { newmin2 = data[i]; }
                    }
                    itsOkay = !((min == newmin2) | (max == newmax2));
                }

                if (itsOkay)
                {
                    lastB = nextB;
                    //  min = (newmin < newmin2) ? newmin : newmin2;
                    // max = (newmax > newmax2) ? newmax : newmax2;
                }
                else
                {
                    db = db / 2;
                }
            }
            if (lastB == 0) throw new ArgumentOutOfRangeException();
            return lastB;
        }

        private void Bounds4Init()
        {
            if (options.cellCount < 1) throw new ArgumentOutOfRangeException();
            if (options.cellCount > 15) throw new ArgumentOutOfRangeException();
                //for  DCT transcoder
                valueBounds4[0] = -DCT4BoundsFind();
                valueBounds4[1] = -valueBounds4[0];

        }

        private byte[] Block8Encode(int cnt)
        {
            int i;
            //dctarray[0, 0] = 1024;
            int[] dctarray = new int[64];
            dctarray[0] = maxDC;

            //for (i = 1; i < options.cellCount + 1; i++)
            for (i = 1; i < cnt + 1; i++)
            {
                byte[] temp = Input.Take(options.density);

                SnakeArraySet(dctarray, i, BitsToCell(temp));
            }
            //for (i = (options.cellCount) + 1; i < 64; i++)
            for (i = (cnt) + 1; i < 64; i++)
            {
                SnakeArraySet(dctarray, i, 0);
            }

            byte[] ar = new byte[64];

            IDCT8x8(dctarray, ref ar);

            return ar;
        }

        private byte[] Block4Encode(int cnt)
        {
            int i;
            //dctarray[0, 0] = 1024;
            int[] dctarray = new int[16];
            dctarray[0] = maxDC;

            //for (i = 1; i < options.cellCount + 1; i++)
            for (i = 1; i < cnt + 1; i++)
            {
                byte[] temp = Input.Take(options.density);

                SnakeArray4Set(dctarray, i, BitsToCell(temp));
            }
            //for (i = (options.cellCount) + 1; i < 64; i++)
            for (i = (cnt) + 1; i < 16; i++)
            {
                SnakeArraySet(dctarray, i, 0);
            }

            byte[] ar = new byte[16];

            IDCT4x4(dctarray, ref ar);

            return ar;
        }

        private byte[] Block8Decode(int cnt)
        {
            byte[] temp = Input.Take(64);

            int[] dctarray = new int[64];

            DCT8x8(temp, ref dctarray);

            byte[] result = new byte[cnt * options.density];

            int k = 0;

            for (int i = 1; i < cnt + 1; i++)
            {
                temp = BitsFromCell(SnakeArrayGet(dctarray, i));
                foreach (byte val in temp)
                {
                    result[k] = val;
                }
                //Output.Add((BitsFromCell(SnakeArrayGet(dctarray, i))));
            }
            return result;

        }

        private byte[] Block4Decode(int cnt)
        {
            byte[] temp = Input.Take(16);

            int[] dctarray = new int[16];

            DCT4x4(temp, ref dctarray);

            int cellCount = (cnt < 16) ? cnt : 15;

            byte[] result = new byte[cellCount * options.density];

            int k = 0;

            for (int i = 1; i < options.cellCount + 1; i++)
            {
                temp = BitsFromCell4(SnakeArray4Get(dctarray, i));
                foreach (byte val in temp)
                {
                    result[k] = val;
                }
                //Output.Add((BitsFromCell(SnakeArrayGet(dctarray, i))));
            }
            return result;

        }

        protected byte[] BitsFromCell4(int value)
        {
            //normalize
            float tmp;
            float max = (1 << options.density) - 1;
            tmp = value - valueBounds4[0];
            tmp = tmp / (valueBounds4[1] - valueBounds4[0]);
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

        protected int BitsToCell4(byte[] bits)
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
            tmp = tmp * (valueBounds4[1] - valueBounds4[0]) + valueBounds4[0]; // stretch to bounds            
            return (int)Math.Round(tmp);
        }


        public void StartToFrame()
        {
            
            int height = (int)(options.resolution);
            int width = height * 16 / 9;
            int BlocksCount = width * height / 64;
            int state = 0;
            int size;
            while ((Input.IsOpen) | (Input.Count > 0))
            {
                if (token.token)
                { break; }
                Frame = new DFFrameYUV(options);
                while (!Frame.IsFull)
                {
                    if (token.token)
                    { break; }
                    byte[] temp;
                    if (state==0)
                    {
                        temp = Block8Encode(options.cellCount);
                        Frame.SetBlockArray
                    }
                    else
                    {
                        temp = Block8Encode
                    }

                    byte[] temp = Input.Take(64);
                    Frame.SetBlockArray(temp);
                }
                Output.Add(Frame.ToArray());
                Frame.Free();
            }
            Output.Complete();
        }
    }
