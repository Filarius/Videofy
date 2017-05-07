using System;
using System.Collections.Generic;
using System.IO;
using Videofy.Settings;

namespace Videofy.Tools
{
    class Biter
    {
        //private byte _bppRatio;
        private MemoryStream _bits;
        private MemoryStream _data;
        private int _BLOCK_SIZE = Config.BlockSize;
        private List<Byte> _bitlist;
        private List<Byte> _bytelist;

        public Biter() : this(1)
        {

        }
        public Biter(byte bppRatio)
        {
            _bits = new MemoryStream(_BLOCK_SIZE * 8);
            _data = new MemoryStream(_BLOCK_SIZE);
            _bitlist = new List<byte>();

            _bytelist = new List<byte>();

            /*
            _bppRatio = 1;

            if (bppRatio < 1)
            {
                _bppRatio = 1;
            }
            else
            {
                _bppRatio = 1;
            }
            */
        }

        private int Encode(bool force)
        {
            return EncodeN(force,Config.BitLevel);
        }

        
        private int EncodeN(bool force, int deep)
        {
            
            if (deep == 0) { return Encode0(force); }
            if (deep == 1) { return Encode1(force); }

            byte[] buff = new byte[_BLOCK_SIZE];
            int cnt = 0;
            int i;
            _bits.Position = 0;
            int list_size = deep;
            while ((i = _bits.Read(buff, 0, _BLOCK_SIZE)) > 0)
            {
                for(var j = 0; j < i; j++)
                {                    
                    _bitlist.Add(buff[j]);
                    if(_bitlist.Count >= list_size)
                    {
                        byte bufout = 0;
                        for(int k = 0; k < list_size; k++)
                        {
                            bufout *= 2;
                            bufout += _bitlist[0];                            
                            _bitlist.RemoveAt(0);
                            cnt += 1;
                        }
                        double f = bufout * 1.0 * 255 / ((1 << list_size) - 1);
                        bufout = (byte)Math.Round(f);
                        _data.WriteByte(bufout);                      
                    }
                }
            }
            _bits.Position = 0;
            return cnt;
        }


        private int Encode1(bool force)
        {
            byte[] buff = new byte[_BLOCK_SIZE];
            int cnt = 0;
            int i;
            _bits.Position = 0;
            while ((i = _bits.Read(buff, 0, _BLOCK_SIZE)) > 0)
            {
                byte[] bufout = new byte[i];
                for (int j = 0; j < i; j++)
                {
                    bufout[j] = (byte)(255 * buff[j]);
                }
                _data.Write(bufout, 0, bufout.Length);
                cnt += i;
            }
            _bits.Position = 0;
            return cnt;
        }

        private int Encode0(bool force)
        {
            byte[] buff = new byte[_BLOCK_SIZE];
            int cnt = 0;
            int i;
            _bits.Position = 0;
            while ((i = _bits.Read(buff, 0, _BLOCK_SIZE)) > 0)
            {
                byte[] bufout = new byte[i * 3];
                for (int j = 0; j < i; j++)
                {
                    byte tmp = (byte)(255 * buff[j]);
                    bufout[3 * j] = tmp;
                    bufout[3 * j + 1] = tmp;
                    bufout[3 * j + 2] = tmp;
                }
                _data.Write(bufout, 0, bufout.Length);
                cnt += i;
            }
            _bits.Position = 0;
            return cnt;
        }


        public int Push(byte data)
        {
            byte[] buff = new byte[8];
            byte temp = data;
            for (int i = 0; i < 8; i++)
            {
                buff[i] = (byte)((temp & 1));
                temp /= 2;
            }
            _bits.Write(buff, 0, 8);
            return Encode(false);
        }

        public int Push(byte[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                byte temp = data[i];
                byte[] buff = new byte[8];
                for (int j = 0; j < 8; j++)
                {
                    buff[j] = (byte)((temp & 1));
                    temp /= 2;
                }
                _bits.Write(buff, 0, 8);
            }
            return Encode(false);
        }



        public void Nail()

        {
            throw new Exception("Not implemented");
        }



        public Byte[] GetBytes()
        {
            int pos = (int)_data.Position;
            if (_data.Position == 0)
            {
                return null;
            }
            else
            {
                Byte[] data = new Byte[pos];
                _data.Position = 0;
                _data.Read(data, 0, pos);
                _data.Position = 0;
                return data;
            }
        }

        public MemoryStream GetStream()
        {
            return _data;
        }


    }
}
