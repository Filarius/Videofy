using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Videofy.Settings;
using Videofy;
using OpenCvSharp.CPlusPlus;
using OpenCvSharp;

namespace Videofy.Tools
{

    class Frame
    {
        private byte[] _frame;
        private int _pos;
        private int _size;
        private int _scale;


        public Frame() : this(false)
        {

        }

        public Frame(Boolean resized)
        {
            _scale = resized ? Config.Block : 1;
            _size = Config.BytesInFrame / (_scale * _scale);
            _frame = new byte[_size];
            Clear();
            _pos = 0;
        }

        public void Clear()
        {
            for (int i = 0; i < _size; i++)
            {
                _frame[i] = 0;
            }
            _pos = 0;
        }

        public int Write(byte data)
        {
            if (_pos == _size)
            {
                return 0;
            }
            else
            {
                _frame[_pos] = data;
                _pos++;
                return 1;
            }
        }

        public int Write(byte[] data)
        {
            int cnt = 0;
            int t;
            for (int i = 0; i < data.Length; i++)
            {
                t = Write(data[i]);
                if (t == 0)
                {
                    break;
                }
                cnt += t;
            }
            return cnt;
        }


        private byte FilterDown(Byte[,] data)
        {

            /*
            Mat mat = new Mat(8, 8, MatType.CV_8U, data);
            Mat mat2 = new Mat();
            mat.ConvertTo(mat2, MatType.CV_32F);
            mat = mat2.Dct(DctFlag2.None);//.ConvertTo(mat,MatType.CV_16S);
            */

            float s = 0;
            int border = 1;
            //  int deep = Config.BitLevel;
            // int x;

            List<Byte> list = new List<byte>();
            for (int i = 0 + border; i < data.GetLength(0) - border; i++)
            {
                for (int j = 0 + border; j < data.GetLength(1) - border; j++)
                {
                    s += data[i, j];
                    //    Checks.WriteSaveAdd(data[i,j]);

                    //s += data[i, j];
                    Config.gist[data[i, j]] += 1;

                    /*
                    double f = (data[i, j] * 1.0 * ((1 << deep) - 1) / 255);
                    int x = (int)Math.Round(f);
                    f = x;
                    f = f * 255 / ((1 << deep) - 1);
                    x = (int)Math.Round(f);

                    */
                    //list.Add(data[i, j]);

                    //   list.Add((byte)x);
                }
                //   Checks.WriteSaveLine();
            }

            /*
            Checks.WriteSaveLine();
            var indexer = mat.GetGenericIndexer<float>();
            for (int i = 0 + border; i < data.GetLength(0) - border; i++)
            {
                for (int j = 0 + border; j < data.GetLength(1) - border; j++)
                {

                    Checks.WriteSaveAdd(indexer[i, j]);
                }
                Checks.WriteSaveLine();
            }
            /*

            /*
            list.Sort();          
            for(int i = 0; i < 8; i++)
            {
                list.RemoveAt(list.Count - 1);
                list.RemoveAt(0);
            }
            foreach(byte b in list)
            {
                s += b;
            }
            s /= list.Count;
            */

            s /= (data.GetLength(0) - border * 2) * (data.GetLength(0) - border * 2);

            /*
            Checks.WriteSaveRead();
            Checks.WriteSaveAdd((byte)s);
            Checks.WriteSaveLine();
            Checks.WriteSaveLine();

            Checks.WriteSaveFlush();
            */

            return (byte)s;

        }

        private void ScaleYUV444(int factor)
        {
            if (factor < _scale) // scale=2^n, factor = 1, MAKE BIGGER
            {
                Byte[] tmp = new Byte[Config.BytesInFrame];
                int w = Config.FrameWidth / _scale;
                int h = Config.FrameHeigth / _scale;
                int ws = Config.FrameWidth;
                int hs = Config.FrameHeigth;
                for (int z = 0; z < 3; z++)//color plane
                {
                    for (int i = 0; i < h; i++)
                    {
                        for (int j = 0; j < w; j++)
                        {
                            byte val = _frame[z * w * h + (i) * w + (j)];
                            for (int x = 0; x < _scale; x++)
                            {
                                for (int y = 0; y < _scale; y++)
                                {
                                    tmp[z * ws * hs + (i * _scale + y) * ws + (j * _scale + x)] = val;
                                }
                            }
                        }
                    }
                }
                _frame = tmp;
                _pos = _frame.Length;
                _size = _frame.Length;
                _scale = 1;
            }
            else if (factor > _scale) //scale=1, factor=2^n, MAKE SMALLER
            {
                Byte[] tmp = new Byte[Config.BytesInFrame / (factor * factor)];
                int w = Config.FrameWidth;
                int h = Config.FrameHeigth;
                int ws = Config.FrameWidth / factor;
                int hs = Config.FrameHeigth / factor; ;

                for (int z = 0; z < 3; z++)//color plane
                {
                    for (int i = 0; i < h; i += factor)
                    {
                        for (int j = 0; j < w; j += factor)
                        {
                            /*
                            Byte[,] block = new Byte[factor, factor];
                            for (int x = 0; x < factor - 2; x++)
                            {
                                for (int y = 0; y < factor - 2; y++)
                                {
                                    block[x, y] = _frame[z * w * h + w * (i + y) + (j + x)];
                                }
                            }
                            tmp[z * ws * hs + (i / factor) * ws + (j / factor)] = FilterDown(block);
                            */

                            int s = 0;
                            int border = 1;
                            for (int x = border; x < factor - border; x++)
                            {
                                for (int y = border; y < factor - border; y++)
                                {
                                    s += _frame[z * w * h + w * (i + y) + (j + x)];
                                }
                            }
                            s /= (factor - border * 2) * (factor - border * 2);
                            tmp[z * ws * hs + (i / factor) * ws + (j / factor)] = (byte)s;
                        }
                    }
                }
                _frame = tmp;
                _pos = _frame.Length;
                _size = _frame.Length;
                _scale = factor;
            }
            else
            {
                //Do nothing
                int a = 1;
                a = 2;
            }
        }

        private void ScaleYUV420(int factor)
        {
            if (factor < _scale) // scale=2^n, factor = 1, MAKE BIGGER
            {
                Byte[] tmp = new Byte[Config.BytesInFrame];
                int w = Config.FrameWidth / _scale;
                int h = Config.FrameHeigth / _scale;
                int ws = Config.FrameWidth;
                int hs = Config.FrameHeigth;
                int prev = 0;
                int prevs = 0;
                int div = 1;
                for (int z = 0; z < 3; z++)//color plane
                {
                    switch (z)
                    {
                        case 0:
                            prev = 0;
                            prevs = 0;
                            div = 1;
                            break;
                        case 1:
                            prev = w * h;
                            prevs = ws * hs;
                            div = 2;
                            break;
                        case 2:
                            prev = w * h + w * h / 4;
                            prevs = ws * hs + ws * hs / 4;
                            div = 2;
                            break;
                    }

                    for (int i = 0; i < (h / div); i++)
                    {
                        for (int j = 0; j < (w / div); j++)
                        {
                            byte val = _frame[prev + (i) * (w / div) + (j)];


                            for (int x = 0; x < _scale; x++)
                            {
                                for (int y = 0; y < _scale; y++)
                                {
                                    //tmp[z * ws * hs + (i * _scale + y) * ws + (j * _scale + x)] = val;
                                    tmp[prevs + (i * _scale + y) * (ws / div) + (j * _scale + x)] = val;
                                }
                            }
                        }
                    }
                }
                _frame = tmp;
                _pos = _frame.Length;
                _size = _frame.Length;
                _scale = 1;
            }
            else if (factor > _scale) //scale=1, factor=2^n, MAKE SMALLER
            {
                Byte[] tmp = new Byte[Config.BytesInFrame / (factor * factor)];
                int w = Config.FrameWidth;
                int h = Config.FrameHeigth;
                int ws = Config.FrameWidth / factor;
                int hs = Config.FrameHeigth / factor; ;
                int prev = 0;
                int prevs = 0;
                int div = 1;

                
                
                 
                 
                Checks.cnt += 1;
                Console.WriteLine(Checks.cnt);
               // Checks.ReadSaveStart();

                Byte[] matarray = new Byte[w * h];
                Buffer.BlockCopy(_frame, 0, matarray, 0, w * h);
                //float[] farray = Array.ConvertAll<byte, float>(matarray, cn => (float)cn);
                
                
                Mat mat;
                mat = new Mat(1, h * w, MatType.CV_8U, matarray);

                //Mat mat2 = new Mat();
                Mat mat2;
                mat2 = mat.Reshape(1, h);
               // mat2.SaveImage("1.png");
                mat = mat2.Resize(new Size(ws, hs), 0, 0, OpenCvSharp.Interpolation.Area);
                mat2 = mat;
              //  mat2.SaveImage("2.png");
                //.ConvertTo(mat,MatType.CV_8U);
                
                var ind = mat2.GetGenericIndexer<byte>();
                
                prevs = 0;
                div = 1;
                for (int i = 0; i < hs; i++)
                {
                    for (int j = 0; j < ws; j++)
                    {
                        tmp[prevs + (i) * (ws / div) + (j)] = ind[i,j];
                        //Checks.ReadSaveAdd(ind[i,j]);

                        
                    }
                }


                


                matarray = new Byte[ws * hs];
                Buffer.BlockCopy(_frame, w * h, matarray, 0, ws * hs);
                mat = new Mat(1, hs * ws, MatType.CV_8U, matarray);
                mat2 = mat.Reshape(1, hs)
                   .Resize(new Size(ws / 2, hs / 2), 0, 0, OpenCvSharp.Interpolation.Area);
                mat = mat2;
                mat2 = mat;
                ind = mat2.GetGenericIndexer<byte>();
                
                prevs = ws * hs;
                div = 2;
                for (int i = 0; i < hs/2; i++)
                {
                    for (int j = 0; j < ws/2; j++)
                    {
                        tmp[prevs + (i) * (ws / div) + (j)] = ind[i, j];
                //        Checks.ReadSaveAdd(prevs + (i) * (ws / div) + (j));

                    }
                }

               // throw new Exception();


                matarray = new Byte[ws * hs];
                Buffer.BlockCopy(_frame, w * h + ws * hs, matarray, 0, ws * hs);
                mat = new Mat(1, hs * ws, MatType.CV_8U, matarray);
                mat2 = mat.Reshape(1, hs)
                   .Resize(new Size(ws / 2, hs / 2), 0, 0, OpenCvSharp.Interpolation.Area);
                mat = mat2;
                mat2 = mat;

                ind = mat2.GetGenericIndexer<byte>();

                prevs = ws * hs+ ws*hs/4;
                div = 2;
                for (int i = 0; i < hs/2; i++)
                {
                    for (int j = 0; j < ws/2; j++)
                    {
                        tmp[prevs + (i) * (ws / div) + (j)] = ind[i, j];
                    }
                }
                


                /*
                Checks.ReadSaveStart();
                
                for (int z = 0; z < 3; z++)//color plane
                {

                    switch (z)
                    {
                        case 0:
                            prev = 0;
                            prevs = 0;
                            div = 1;
                            break;
                        case 1:
                            prev = w * h;
                            prevs = ws * hs;
                            div = 2;
                            break;
                        case 2:
                            prev = w * h + w * h / 4;
                            prevs = ws * hs + ws * hs / 4;
                            div = 2;
                            break;
                    }

                    for (int i = 0; i < ((h / div)); i += factor)
                    {
                        for (int j = 0; j < ((w / div)); j += factor)
                        {


                            Byte[,] block = new Byte[factor, factor];
                            for (int x = 0; x < factor; x++)
                            {
                                for (int y = 0; y < factor; y++)
                                {
                                    block[x, y] = _frame[prev + (i + y) * (w / div) + (j + x)];
                    
                                }
                            }
                            tmp[prevs + (i / factor) * (ws / div) + (j / factor)] = FilterDown(block);
                            if(z==1)
                            Checks.ReadSaveAdd(prevs + (i / factor) * (ws / div) + (j / factor));
                        }
                    }
                    if (z == 1)
                        throw new Exception();
                }
                */
                
                _frame = tmp;
                _pos = _frame.Length;
                _size = _frame.Length;
                _scale = factor;
            }
            else
            {
                //Do nothing
                int a = 1;
                a = 2;
            }
        }

        public void Scale(int factor)
        {
            switch (Config.PixFmt)
            {
                case Main.PixelFormat.YUV444P: ScaleYUV444(factor); break;
                case Main.PixelFormat.YUV420P: ScaleYUV420(factor); break;
                    //   default: ScaleYUV444(factor); break;
            }

        }

        /*
        public Byte this[int x,int y]
        {
            get { return _frame[Config.FrameWidth_ * x + y]; }
            set { _frame[Config.FrameWidth_ * x + y] = value; }
        }
        */


        public byte[] GetBytes()
        {
            return _frame;
        }

        public bool IsFull
        {
            get { return (_pos == _size); }
        }



    }
}
