using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Videofy.Main;

namespace Videofy.Chain.Types
{
    class DFFrameSub
    {
        private byte[] _frame; // array pinned in memory, permanent OpenCV Mat data storage.
        private GCHandle _handle;// handle of pinned _frame
        private IntPtr ptr; //address of _frame in memory
        private Mat _mat;
        private int _blockPos;
        private int _size, _height, _width;
        public int Size { get { return _size; } }


        public DFFrameSub(int width, int height)
        {
           
            _frame = new byte[height * width];
            _handle = GCHandle.Alloc(_frame, GCHandleType.Pinned);
            ptr = _handle.AddrOfPinnedObject();
            _mat = new Mat(height, width, MatType.CV_8U, ptr);
            _blockPos = 0;
            _size = height * width;
            _height = height;
            _width = width;
        }

        public void Free()
        {
            _mat.Dispose();
            _handle.Free();
        }

        public Boolean IsFull
        {
            get
            {
                return _blockPos >= _size;
            }
        }

        private void BlockPointerInc()
        {
            _blockPos += 8;
            if ((_blockPos % _width) == 0)
            {
                //blocks are 8x8, at end of line move 7 lines forward
                _blockPos += _width * 7;
            }
        }

        public void SetBlock(DFFrameBlock block)
        {
            if (IsFull) throw new ArgumentOutOfRangeException();
            int x = _blockPos % _width;
            int y = _blockPos / _width;
            Mat ROI = _mat.SubMat(new Rect(x, y, 8, 8));
            block.Body.CopyTo(ROI);
            BlockPointerInc();
        }

        public DFFrameBlock GetBlock()
        {
            if (IsFull) throw new ArgumentOutOfRangeException();
            int x = _blockPos % _width;
            int y = _blockPos / _width;
            Mat ROI = _mat.SubMat(new Rect(x, y, 8, 8));
            DFFrameBlock block = new DFFrameBlock();
            ROI.CopyTo(block.Body);
            BlockPointerInc();
            return block;
        }

        public void FromArray(byte[] array)
        {
            if (array.Length != _frame.Length) throw new ArgumentOutOfRangeException();
            Array.Copy(array, _frame, array.Length);
            _mat = new Mat(_height, _width, MatType.CV_8U, ptr);
        }

        public byte[] ToArray()
        {
            byte[] array = new byte[_frame.Length];
            Array.Copy(_frame, array, _frame.Length);
            return array;
        }

    }
}
