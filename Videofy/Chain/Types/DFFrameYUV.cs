using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Videofy.Main;

namespace Videofy.Chain.Types
{
    class DFFrameYUV
    {
        private List<DFFrameSubYUV> _frames;
        private int _lowBlockSize;
        public List<DFFrameSubYUV> SubFrames
        {
            get
            {
                return _frames;
            }
        }
        public int Size { get; private set; }

        public DFFrameYUV(OptionsStruct opt)
        {
            _frames = new List<DFFrameSubYUV>();
            int height = (int)opt.resolution;
            int width = height * 16 / 9;
            //always need one fullsize subframe;
           // DFFrameSub subFrame = new DFFrameSub(width, height);
            _frames.Add(new DFFrameSubYUV(width, height,8));

            switch (opt.pxlFmtIn)
            {
                case PixelFormat.RGB24:
                case PixelFormat.YUV444P:
                    //add 2 fullsize subframes
                    _frames.Add(new DFFrameSubYUV(width, height,8));
                    _frames.Add(new DFFrameSubYUV(width, height,8));
                    Size = Size * 3;
                    _lowBlockSize = 8;
                    break;
                case PixelFormat.YUV420P:
                    //add 2 halfsize subframes
                    _frames.Add(new DFFrameSubYUV(width / 2, height / 2, 4));
                    _frames.Add(new DFFrameSubYUV(width / 2, height / 2, 4));
                    Size = Size + Size / 2;
                    _lowBlockSize = 4;
                    break;
            }
            Size = 0;
            foreach(var sub in _frames)
            {
                Size += sub.Size;
            }
        }

        public Boolean IsFull
        {
            get
            {   
                foreach(var sub in _frames)
                {   
                    if(!sub.IsFull)
                    {
                        return false;
                    }
                }
                return true;
            }
        }       

        public void SetBlock(DFFrameBlock block)
        {
            byte cnt = 0;
            foreach(DFFrameSubYUV sub in _frames)
            {
                if(sub.IsFull)
                {
                    cnt += 1;
                    continue;                    
                }
                if ((cnt > 0) & (_lowBlockSize == 4))
                {
                    sub.SetBlock4(block);
                }
                else
                {
                    sub.SetBlock8(block);
                }
                return;
                
            }
            throw new IndexOutOfRangeException();
        }

        public void SetBlockArray(byte[] array)
        {
            DFFrameBlock block = new DFFrameBlock(array);
            SetBlock(block);
            block.Free();
        }

        public DFFrameBlock GetBlock()
        {
            byte cnt = 0;
            foreach (DFFrameSubYUV sub in _frames)
            {
                if (sub.IsFull)
                {
                    cnt += 1;
                    continue;
                }

                if ((cnt > 0) & (_lowBlockSize == 4))
                {
                    return sub.GetBlock4();
                }
                else
                {
                    return sub.GetBlock8();
                }

                
            }
            throw new IndexOutOfRangeException();
        }

        public byte[] GetBlockArray()
        {
            DFFrameBlock block = GetBlock();
            byte[] temp = block.ToArray();
            block.Free();
            return temp;
        }

        public void FromArray(byte[] array)
        {
            if (array.Length != Size) throw new ArgumentOutOfRangeException();
            int i = 0;
            foreach(var sub in _frames)
            {
                byte[] temp = new byte[sub.Size];
                Array.Copy(array, i, temp, 0, temp.Length);
                sub.FromArray(temp);
                i += temp.Length;
            }           
        }

        public byte[] ToArray()
        {
            if (!IsFull) throw new Exception("Frame is not full");
            byte[] temp = new byte[Size];
            int i = 0;
            foreach(var sub in _frames)
            {
                Array.Copy(sub.ToArray(), 0, temp, i, sub.Size);
                i += sub.Size;
            }
            return temp;
        }

        public void Free()
        {
            foreach(var sub in _frames)
            {
                sub.Free();
            }
            _frames.Clear();
        }

    }
}