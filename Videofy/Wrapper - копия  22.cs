using System;
using System.Text;
using System.IO;
using System.Diagnostics;
using Videofy.Settings;
using System.Threading;

namespace Videofy.Tools

{
    class Wrapper
    {
        public Process _proc;
        private ProcessStartInfo _procInfo;
        public MemoryStream _readStream;
        public MemoryStream _errorStream;
        public MemoryStream _writeStream;
        public Boolean IgnoreInput;
        private Thread _tread, _twrite, _terror;
        private readonly Object rLocker, wLocker, eLocker;
        private bool _isRun;
        private int _charBlock = Config.BlockSize / sizeof(char);
        private int _byteBlock = Config.BlockSize;
        public Wrapper(string Path, string args)            
        {
            IgnoreInput = false;
            rLocker = new Object();
            eLocker = new Object();
            wLocker = new Object();
            _errorStream = new MemoryStream(_byteBlock * 2);
            _readStream = new MemoryStream(_byteBlock * 2);
            _writeStream = new MemoryStream(_byteBlock * 2);
            _procInfo = new ProcessStartInfo(Path, args);

            _procInfo.CreateNoWindow = true;

            _procInfo.UseShellExecute = false;

            _procInfo.RedirectStandardError = true;
            _procInfo.RedirectStandardInput = true;
            _procInfo.RedirectStandardOutput = true;



            _proc = new Process();


            _proc.StartInfo = _procInfo;
            _isRun = false;         
        }

        ~Wrapper()
        {
            Stop();
        }
        

        public void StarByte()
        {
            _proc.Start();
            _proc.PriorityClass = ProcessPriorityClass.BelowNormal;
            //  return;
            if (_isRun) return;
            _isRun = true;

            (_twrite = new Thread(
             () =>
             {
                 IntractBytes(_writeStream, _proc.StandardInput.BaseStream, wLocker);
             }

             )
            ).Start();

            (_tread = new Thread(
             () =>
             {
                 ExtractBytes(_proc.StandardOutput.BaseStream, _readStream, rLocker);
             }

             )
            ).Start();

            (_terror = new Thread(
            () =>
            {
                ExtractBytes(_proc.StandardError.BaseStream, _errorStream, eLocker);
            }

            )
           ).Start();
            //_thread.Start();
        }

        public void Stop()
        {
            (new Thread(() =>
            {
                if (_isRun)
                {
                    WriteFlush();

                    _proc.StandardInput.BaseStream.Close();
                    _proc.StandardError.BaseStream.Close();
                    _proc.StandardOutput.BaseStream.Close();
                   
                    try
                    {
                        if (!_proc.HasExited)
                            _proc.Close();
                    }
                    catch
                    { }
                                        
                    _isRun = false;

                    _tread.Join();
                    _twrite.Join();
                    _terror.Join();

                    _terror.Abort();
                    _tread.Abort();
                    _twrite.Abort();                    
                }
            })
            ).Start();            
        }

        public void Wait()
        {
            System.Threading.Thread.Sleep(1);
        }

        public void Wait(int ms)
        {
            System.Threading.Thread.Sleep(ms);
        }

        private void WaitFor(ref bool flag)
        {
            System.Threading.Thread.Sleep(1);
            while (flag)
            {
                System.Threading.Thread.Sleep(1);
            }
        }        

        private string BArrToString(byte[] data)
        {
            string reply = "";
            if (data != null)
            {
                reply = System.Text.Encoding.ASCII.GetString(data);
            }
            return reply;
        }

        private byte[] BArrFromString(String data)
        {
            return Encoding.ASCII.GetBytes(data);
        }
        
               
        private void ExtractBytes(Stream input, Stream output, Object locker)
        {
            while (_isRun)
            {
                lock (locker)
                {
                    if (output.Position < _byteBlock)
                    {
                        byte[] buf = new byte[_byteBlock];
                        int i = 0;
                        try
                        {
                            if (input.CanRead)
                            {
                                i = input.Read(buf, 0, _byteBlock);
                            }
                        }
                        catch
                        {
                            break;
                        }

                        if (i > 0)
                        {

                            output.Write(buf, 0, i);
                        }
                    }

                }
            }


        }

        private void IntractBytes(Stream input, Stream output, Object locker)
        {
            while (_isRun)
            {
                if (!IsRunning)
                {
                    break;
                }
                lock (locker)
                {
                    if ((input.Position > 0))
                    {
                        int pos = (int)input.Position;
                        if (pos == 0)
                        {
                            Wait();
                            continue;
                        }
                        byte[] buf = new byte[pos];
                        input.Position = 0;
                        int i = input.Read(buf, 0, pos);
                        input.Position = 0;                        
                        try
                        {
                            output.Write(buf, 0, pos);
                            output.Flush();
                        }
                        catch
                        {
                            break;
                        }
                    }
                }

            }


        }

        public void WriteString(String data)
        {
            throw new Exception("Not Implemented");
        }

        private byte[] GetStreamData(MemoryStream stream)
        {
            byte[] data;
            if (stream.Position > 0)
            {
                int pos = (int)stream.Position;
                data = new byte[pos];
                stream.Position = 0;
                int i = stream.Read(data, 0, pos);
                if (i < stream.Position)
                {
                    throw new Exception("Somewhy get stream readed not at full length");
                }
                stream.Position = 0;

                return data;
            }
            return null;

        }

        private void SetStreamData(MemoryStream stream, byte[] data)
        {
            int i = 0;
            while ((i + 1) < data.Length)
            {
                if (!IsRunning)
                {
                    break;
                }
                int len = (_byteBlock * 2) - (int)stream.Position - 1;
                if (len <= 0)
                {
                    continue;
                }
                int cnt = data.Length - i - 1;
                if (len > cnt)
                {
                    len = cnt;
                }
                len = len + 1;
                byte[] buf = new byte[len];
                Buffer.BlockCopy(data, i, buf, 0, len);
                i += len;
                lock (wLocker)
                {
                    stream.Write(buf, 0, buf.Length);
                }
            }
        }
        

        public byte[] Read()
        {
            lock (rLocker)
            {
                return GetStreamData(_readStream);
            }
        }

        public string ReadString()
        {
            if (Monitor.TryEnter(eLocker))
            {
                return BArrToString(Read());
            }
            else
            {
                return "";
            };
        }


        public byte[] Error()
        {
            lock (eLocker)
            {
                return GetStreamData(_errorStream);
            }
        }

        public string ErrorString()
        {
            if (Monitor.TryEnter(eLocker))
            {
                return BArrToString(Error());
            }
            else
            {
                return "";
            }
        }


        public void Write(byte[] data)
        {
            SetStreamData(_writeStream, data);
        }

        public void Write(String data)
        {
            Write(BArrFromString(data));
        }

        public void WriteFlush()
        {
            _proc.StandardInput.BaseStream.Flush();            
        }


        public Boolean IsRunning
        {
            get
            {
                try
                {
                    return (!_proc.HasExited);
                }
                catch
                {
                    return false;
                }
            }
        }
        
        private byte[] GetBytes(string str)
        {
            byte[] buff = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, buff, 0, buff.Length);
            return buff;
        }
        

        private void SendData(byte[] bytes)
        {
            _proc.StandardInput.BaseStream.Write(bytes, 0, bytes.Length);
        }
    }
}
