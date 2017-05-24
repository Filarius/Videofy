using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Videofy.Chain.Types
{
    class DFFrameBlock
    {
        private GCHandle handle;
        private IntPtr ptr;
        private byte[] block;
        private int[] blockint;
        private float[,] blockf;

        public Mat Body { get; set; }

        public DFFrameBlock()
        {
            block = new byte[64];
            handle = GCHandle.Alloc(block, GCHandleType.Pinned);
            ptr = handle.AddrOfPinnedObject();
            Body = new Mat(8, 8, MatType.CV_8U, ptr);
            //Body = new Mat(8, 8, MatType.CV_8U, 0);
        }               

        public DFFrameBlock(float[,] array)
        {
            if (array.Length != 64) throw new ArgumentOutOfRangeException();
            blockf = array;
            handle = GCHandle.Alloc(blockf, GCHandleType.Pinned);
            ptr = handle.AddrOfPinnedObject();
            Body = new Mat(8, 8, MatType.CV_32FC1, ptr);
        }

        
        public DFFrameBlock(byte[] array)
        {
            if (array.Length != 64) throw new ArgumentOutOfRangeException();
            block = array;
            handle = GCHandle.Alloc(block, GCHandleType.Pinned);
            ptr = handle.AddrOfPinnedObject();
            Body = new Mat(8, 8, MatType.CV_8U, ptr);
        }

        public DFFrameBlock(int[] array)
        {
            if (array.Length != 64) throw new ArgumentOutOfRangeException();
            blockint = array;
            handle = GCHandle.Alloc(block, GCHandleType.Pinned);
            ptr = handle.AddrOfPinnedObject();
            Body = new Mat(8, 8, MatType.CV_32SC1, ptr);
        }


        public byte[] ToArray()
        {
            return block;
        }

        public float[,] ToArrayF()
        {
            return blockf;
        }

        public void Free()
        {
            Body.Dispose();
            handle.Free();            
        }
    }
}
