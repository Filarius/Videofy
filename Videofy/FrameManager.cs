using System;
using System.Collections.Generic;

namespace Videofy.Tools
{
    class FrameManager
    {
        private List<Frame> _frames;
        private int _sin;
        private int _sout;

        public FrameManager()
        {
            _frames = new List<Frame>();
            _sin =
            _sout = 1;
        }

        public FrameManager(int scale_in, int scale_out)
        {
            _sin = scale_in;
            _sout = scale_out;
            _frames = new List<Frame>();
        }

        public Boolean HaveReady()
        {
            Boolean result = (_frames.Count > 0);
            if (!result) return result;
            result = _frames[0].IsFull;
            return result;
        }

        private void AddFrame()
        {
            if (_sin > 1)
            {
                _frames.Add(new Frame(true));
            }
            else
            {
                _frames.Add(new Frame(false));
            }
        }

        public void Write(Byte[] data)
        {
            Byte[] tmp;
            Byte[] arr = data;
            int done = 0;
            if (_frames.Count == 0)
            {
                AddFrame();
            }
            int i = 0;
            while (done < data.Length)
            {
                tmp = arr;
                arr = new Byte[tmp.Length - i];
                Array.Copy(tmp, i, arr, 0, tmp.Length - i);
                int last = _frames.Count - 1;
                i = _frames[last].Write(arr);
                done += i;
                if (i < arr.Length)
                {
                    AddFrame();
                    continue;
                }
            }
        }

        public Boolean IsDataHere
        {
            get
            {
                return (_frames.Count > 0);
            }
        }

        public Boolean IsDataReady
        {
            get
            {
                if (_frames.Count > 0)
                {
                    return _frames[0].IsFull;
                }
                else
                {
                    return false;
                }

            }
        }

        public Byte[] ReadFrame()
        {
            if (_frames.Count == 0)
            {
                return null;
            }
            if (_sin != _sout)
            {
                _frames[0].Scale(_sout);
                Byte[] arr = _frames[0].GetBytes();
                _frames.RemoveAt(0);
                return arr;
            }
            return _frames[0].GetBytes();
        }
    }
}
