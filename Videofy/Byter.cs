using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Videofy.Settings;

namespace Videofy.Tools
{
    class Byter
    {
        private byte _bppRatio;
        private MemoryStream _bytes;
        private MemoryStream _data;
        private int _BLOCK_SIZE = Config.BlockSize;
        private List<byte> _bits;
        private List<byte> mem;
        private float _error;
        public float error
        {
            get { return _error; }
        }


        public Byter() : this(1)
        {

        }

        public Byter(byte bppRation)
        {
            _bytes = new MemoryStream(Config.BlockSize);
            _data = new MemoryStream(Config.BlockSize);
            _bppRatio = 1;
            _bits = new List<byte>();
            _error = 0;
            mem = new List<byte>();
        }

        private byte clip(int x)
        {
            if (x < 0) return 0;
            if (x > 0) return 255;
            return (byte)x;
        }
        public void Decode(bool force)
        {
            DecodeN(force, Config.BitLevel);
        }

        private void DecodeN(bool force, int deep)
        {

            if (_bytes.Position == 0)
            {
                return;
            }

            if (deep == 0) { Decode0(force); return; }
            if (deep == 1) { Decode1(force); return; }

            float t;
            int list_len = deep;

            byte[] buff = new byte[_bytes.Position];
            int i;
            _bytes.Position = 0;
            while ((i = _bytes.Read(buff, 0, buff.Length)) > 0)
            {
                for (int j = 0; j < i; j++)
                {
                    float f;
                    mem.Add(buff[j]);
                    if (mem.Count == 1)
                    {
                        f = mem[0];
                        mem.Clear();
                    }
                    else
                    {                        
                        continue;
                    }                   
                  
                    f = (float)(f * 1.0 * ((1 << deep) - 1) / 255);
                    byte tmp = (byte)Math.Round(f);

                    if (_error < Math.Abs(tmp - f) * 2 )
                    {
                        _error = Math.Abs(tmp - f) * 2;
                    }

                    List<Byte> list = new List<byte>();
                    for (int k = 0; k < list_len; k++)
                    {
                        byte bit = (byte)(tmp % 2);
                        tmp /= 2;
                        list.Add(bit);
                    }
                    list.Reverse();
                    _bits.AddRange(list);
                    while (_bits.Count >= 8)
                    {
                        byte byt = 0;
                        for (int a = 7; a >= 0; a--)
                        {
                            byt *= 2;
                            byt += _bits[a];
                            _bits.RemoveAt(a);
                        }
                        _data.WriteByte(byt);
                    }

                }
            }
            _bytes = new MemoryStream(Config.BlockSize);
        }

        private void Decode1(bool force)
        {
            if (_bytes.Position == 0)
            {
                return;
            }

            float t;

            byte[] buff = new byte[_bytes.Position];
            int i;
            _bytes.Position = 0;
            while ((i = _bytes.Read(buff, 0, buff.Length)) > 0)
            {
                for (int j = 0; j < i; j++)
                {
                    byte bit;
                    if (buff[j] < 128) { bit = 0; }
                    else { bit = 1; }
                    t = buff[j];
                    t /= 255;



                    if (_error < (Math.Abs(bit - t) * 2))
                    {
                        _error = Math.Abs(bit - t) * 2;
                    }
                    byte bt = (byte)(255 * bit);

                    _bits.Add(bit);
                    if (_bits.Count == 8)
                    {
                        byte byt = 0;
                        for (int a = 7; a >= 0; a--)
                        {
                            byt *= 2;
                            byt += _bits[a];
                        }

                        _bits.Clear();
                        _data.WriteByte(byt);

                    }
                }
            }
            _bytes = new MemoryStream(Config.BlockSize);
        }

        private void Decode0(bool force)
        {
            float t;
            byte[] buff = new byte[3];
            int i;

            if (_bytes.Position < 3)
            {
                return;
            }
            _bytes.Position = 0;
            while ((i = _bytes.Read(buff, 0, 3)) == 3)
            {
                t = buff[0] + buff[1] + buff[2];
                t /= 3;
                t /= 255;

                byte bit = (byte)Math.Round(t);
                if (_error < (Math.Abs(bit - t) * 2))
                {
                    _error = Math.Abs(bit - t) * 2;
                }
                _bits.Add(bit);
                if (_bits.Count == 8)
                {
                    byte byt = 0;
                    for (int a = 7; a >= 0; a--)
                    {
                        byt *= 2;
                        byt += _bits[a];
                    }

                    _bits.Clear();
                    _data.WriteByte(byt);
                }
            }

            if (i < 3)
            {
                if (_bytes.Length == _bytes.Position)
                {
                    _bytes = new MemoryStream(Config.BlockSize);
                    _bytes.Write(buff, 0, i);
                    return;
                }
                Console.Write("Bytes Count Read Error");
                throw new Exception("Bytes Count Read Error");
            }
        }


        public void Push(byte data)
        {
            _bytes.WriteByte(data);
            Decode(false);
        }

        public void Push(byte[] data)
        {
            // for (int i = 0; i < data.Length; i++)
            // {
            //     _bytes.Write(data, 0, data.Length);
            // }
            _bytes.Write(data, 0, data.Length);
            Decode(false);
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
                //_data.Position();
                return data;
            }
        }

        public MemoryStream GetStream()
        {
            return _data;
        }


    }
}
