using System;
using System.Collections.Generic;
using Videofy.Settings;
using OpenCvSharp.CPlusPlus;
using System.Runtime.InteropServices;

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
            
            float s = 0;
            int border = 1;
           
            List<Byte> list = new List<byte>();
            for (int i = 0 + border; i < data.GetLength(0) - border; i++)
            {
                for (int j = 0 + border; j < data.GetLength(1) - border; j++)
                {
                    s += data[i, j];
                    
                }
                
            }         
            s /= (data.GetLength(0) - border * 2) * (data.GetLength(0) - border * 2);
           
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

                Mat mat, mat2;

                var rType = OpenCvSharp.Interpolation.Area;
                {
                    GCHandle hframe = GCHandle.Alloc(_frame, GCHandleType.Pinned);
                    IntPtr iframe = hframe.AddrOfPinnedObject();

                    mat = new Mat
                        (h, w, MatType.CV_8UC1, iframe);
                    mat2 = new Mat();
                    mat2 = mat.Resize(new Size(ws, hs), 0, 0, rType);
                    Marshal.Copy(mat2.Data, tmp, 0, ws * hs);

                    iframe = new IntPtr(iframe.ToInt64() + w * h);

                    mat = new Mat
                       (h / 2, w / 2, MatType.CV_8UC1, iframe);
                    mat2 = new Mat();
                    mat2 = mat.Resize(new Size(ws / 2, hs / 2), 0, 0, rType);
                    Marshal.Copy(mat2.Data, tmp, ws * hs, ws * hs / 4);

                    iframe = new IntPtr(iframe.ToInt64() + w * h / 4);

                    mat = new Mat
                       (h / 2, w / 2, MatType.CV_8UC1, iframe);
                    mat2 = new Mat();
                    mat2 = mat.Resize(new Size(ws / 2, hs / 2), 0, 0, rType);
                    Marshal.Copy(mat2.Data, tmp, ws * hs + ws * hs / 4, ws * hs / 4);

                    hframe.Free();
                }
                

                
                _frame = tmp;
                _pos = _frame.Length;
                _size = _frame.Length;
                _scale = factor;
            }            
        }

        public void Scale(int factor)
        {
            switch (Config.PixFmt)
            {
                case Main.PixelFormat.Gray: ScaleYUV444(factor); break;
                case Main.PixelFormat.YUV444P: ScaleYUV444(factor); break;
                case Main.PixelFormat.YUV420P: ScaleYUV420(factor); break;                   
            }

        }
        
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
