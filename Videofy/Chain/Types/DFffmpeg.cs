using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Videofy.Main;

namespace Videofy.Chain.Types
{
    class DFffmpeg
    {
        public Process _proc;
        private ProcessStartInfo _procInfo;

        private BlockingCollection<byte[]> _readQueue, _writeQueue, _errorQueue;
        private CancellationTokenSource _cancelToken;

        public Boolean IgnoreInput;
        private Thread _tread, _twrite, _terror;
        private Object rLocker, wLocker, eLocker;
        private bool _isRun;
        private int _charBlock = 1024000 / sizeof(char);
        private int _byteBlock = 1024000;

        //public DFffmpeg(string Path,string args)
        public DFffmpeg(string args)
        {
            IgnoreInput = false;

            rLocker = new Object();
            eLocker = new Object();
            wLocker = new Object();

            _cancelToken = new CancellationTokenSource();

            _readQueue = new BlockingCollection<byte[]>(2);
            _writeQueue = new BlockingCollection<byte[]>(2);
            _errorQueue = new BlockingCollection<byte[]>(2);

            _procInfo = new ProcessStartInfo("Utils/ffmpeg.exe", args);
            _procInfo.CreateNoWindow = true;
            _procInfo.UseShellExecute = false;
            _procInfo.RedirectStandardError = true;
            _procInfo.RedirectStandardInput = true;
            _procInfo.RedirectStandardOutput = true;

            _proc = new Process();

            _proc.StartInfo = _procInfo;

            _isRun = false;
        }

        /*
        public DFffmpeg(string Path, OptionsStruct opt)
        {

        }
        */

        ~DFffmpeg()
        {
            Stop();
        }


        public void StarByte()
        {
            if (_isRun) return;


            _isRun = true;
            _proc.Start();
            try
            {
                _proc.PriorityClass = ProcessPriorityClass.BelowNormal;
            }
            catch 
            {
            }
            

            // seems best low CPU solution to check if process if alive 
            // and wrapper need to do cleanup
            (new Thread(
                () =>
                {
                    while (true)
                    {
                        try
                        {
                            if (_proc.HasExited) // HasExited is expensive operation 
                            {
                                _isRun = false;
                                break;
                            }
                        }
                        catch
                        {
                            _isRun = false;
                            break;
                        };
                        Wait(1000);
                    }
                }
                )
            ).Start();


            _twrite = new Thread(
             () =>
             {
                 IntractBytes(_writeQueue, _proc.StandardInput.BaseStream);
             }

             );
            _twrite.Start();


            _tread = new Thread(
             () =>
             {
                 ExtractBytes(_proc.StandardOutput.BaseStream, _readQueue);
             }
            );
            _tread.Start();


            _terror = new Thread(
                () =>
                {
                    ExtractBytes(_proc.StandardError.BaseStream, _errorQueue);
                }
            );
            _terror.Start();
        }

        public void Stop()
        {
            (new Thread(() =>
            {
                if (_isRun)
                {
                    _isRun = false;
                }

                try { _proc.StandardInput.BaseStream.Close(); } catch { }
                try { _proc.StandardError.BaseStream.Close(); } catch { }
                try { _proc.StandardOutput.BaseStream.Close(); } catch { }

                Wait(1000);

                _cancelToken.Cancel();

                try { _tread.Abort(); } catch { }
                try { _terror.Abort(); } catch { }
                try { _twrite.Abort(); } catch { }

                try
                {
                    if (!_proc.HasExited)
                    {
                        Console.WriteLine("Killing " + _proc.ProcessName);
                        _proc.Kill();
                        _proc.Close();
                    }
                }
                catch
                { }

                _readQueue.Dispose();
                _errorQueue.Dispose();
                _writeQueue.Dispose();

                    /*
                    _readStream.Close();
                    _errorStream.Close();
                    _writeStream.Close();
                    */

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

        /*
        private void WaitFor(ref bool flag)
        {
            System.Threading.Thread.Sleep(1);
            while (flag)
            {
                System.Threading.Thread.Sleep(1);
            }
        }
        */

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


        //private void ExtractBytes(Stream input, Stream output, Object locker)
        private void ExtractBytes(Stream input, BlockingCollection<byte[]> outputQueue)
        {
            while (IsRunning)
            {
                try
                {
                    // var temp = new MemoryStream();
                    byte[] buf = new byte[_byteBlock];
                    int i = input.Read(buf, 0, _byteBlock);
                    if (i == 0)
                        continue;
                    if (i < _byteBlock)
                    {
                        Array.Resize<byte>(ref buf, i);
                    }
                    outputQueue.Add(buf);
                }
                catch (Exception e)
                {
                    Console.Write("Extract " + e.ToString());
                    break;
                }
            }

            /*
                            if (i == 0)
                                if (output.Position < _byteBlock)
                            {
                                byte[] buf = new byte[_byteBlock];
                                int i = 0;
                                try
                                {
                                    i = input.Read(buf, 0, _byteBlock);
                                }
                                catch
                                {
                                    break;
                                }

                                if (i > 0)
                                {
                                    lock (locker)
                                    {
                                        output.Write(buf, 0, i);
                                    }
                                }
                            }
            */

        }

        //private void IntractBytes(Stream input, Stream output, Object locker)
        private void IntractBytes(BlockingCollection<byte[]> inputQueue, Stream output)
        {
            while (IsRunning)
            {
                try
                {
                    byte[] buf = inputQueue.Take(_cancelToken.Token);
                    output.Write(buf, 0, buf.Length);
                  //  output.Flush();
                }
                catch (Exception e)
                {
                    Console.Write(e.ToString());
                    break;
                }
            }

            /*
            if (input.Position == 0)
            {
                Wait(100);
                continue;
            }

            int pos;

            lock (locker)
            {
                pos = (int)input.Position;
                buf = new byte[pos];
                input.Position = 0;
                int i = input.Read(buf, 0, pos);
                input.Position = 0;
            }*/


        }

        public void WriteString(String data)
        {
            throw new Exception("Not Implemented");
        }

        //private byte[] GetStreamData(MemoryStream stream)
        private byte[] GetArray(BlockingCollection<byte[]> queue)
        {
            try
            {
                byte[] temp = null;
                if(queue.TryTake(out temp))
                {
                    return temp;
                }
                else
                {
                    return null;
                }

                //return queue.TryTake(_cancelToken.Token);
            }
            catch (Exception e)
            {
                Console.WriteLine("ARRAY ERROR " + e.ToString());
                return null;
            }

            /*
                if (stream.Position > 0)
                {
                    int pos = (int)stream.Position;
                    byte[] data = new byte[pos];
                    stream.Position = 0;
                    int i = stream.Read(data, 0, pos);

                    if (i < stream.Position)
                    {
                        throw new Exception("Somewhy stream readed not at full length");
                    }

                    stream.Position = 0;

                    return data;
                }
                return null;
                */
        }

        //private void SetStreamData(MemoryStream stream, byte[] data)
        private void SetArray(BlockingCollection<byte[]> queue, byte[] data)
        {
            try
            {
                queue.Add(data, _cancelToken.Token);
            }
            catch (Exception e)
            {
                Console.WriteLine("Set Array " + e.ToString());
            }
            /*
            int i = 0;
            while ((i + 1) < data.Length)
            {
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
            */
        }


        public byte[] Read()
        {
            return GetArray(_readQueue);
            /*
            lock (rLocker)
            {
                return GetStreamData(_readStream);
            }
            */
        }

        public string ReadString()
        {
            return BArrToString(Read());
            /*
            if (Monitor.TryEnter(rLocker))
            {

            }            
            else
            {
                return "";
            };
            */
        }


        public byte[] Error()
        {
            return GetArray(_errorQueue);
            /*
            lock (eLocker)
            {
                return GetStreamData(_errorStream);
            }
            */
        }

        public string ErrorString()
        {
            try
            {
                return BArrToString(Error());
            }
            catch
            {
                return "";
            }

            /*
            if (Monitor.TryEnter(eLocker, 1000))
            {
                try
                {
                    return BArrToString(Error());
                }
                catch
                {
                    return "";
                }
                finally
                {
                    Monitor.Exit(eLocker);
                }
            }
            else
            {
                return "";
            }
            */
        }


        public void Write(byte[] data)
        {
            SetArray(_writeQueue, data);
            //SetStreamData(_writeStream, data);
        }

        public void Write(String data)
        {
            Write(BArrFromString(data));
        }

        /*
        public void WriteFlush()
        {
            _proc.StandardInput.BaseStream.Flush();
        }
        */

        public Boolean IsRunning
        {
            get
            {
                try
                {
                    //return (!_proc.HasExited); // operataion is too heavy !!
                    return _isRun;
                }
                catch
                {
                    return false;
                }
            }
        }
        /*
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
        */
    }
}




