using System;
using Videofy.Tools;
using Videofy.Settings;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace Videofy.Main
{
    class Converter : IDisposable
    {
        public float[,] rect = new float[8, 8];

        public Wrapper _wrapIn;
        public Wrapper _downloader;
        public ConverterState state;
        private Biter _biter;
        private Byter _byter;
        //private Frame _frame;
        private FrameManager _frameManager;
        private String _filePath;
        private FileStream _file;
        //private String _URL;
        private const int EncodedHeaderLength = 3624;
        private Boolean _encode;
        private Boolean _workDone;
        private Boolean _isWorking;
        private Boolean _download;

        //private BinaryWriter bw;
        public WorkMeter workMeter;

        public void Init()
        {
            state = new ConverterState();
            workMeter = new WorkMeter();
            _biter = new Biter();
            _byter = new Byter();
            _workDone = false;
            _isWorking = false;
        }
        public Converter(String file, Boolean encode)
        {
            if (encode)
            {
                _wrapIn = new Wrapper(Config.FFMpegPath, Config.GetFFmpegEncCRF(25, file + ".Videofy.mp4"));

            }
            else
            {
                _wrapIn = new Wrapper(Config.FFMpegPath, Config.FFMpegDec.Replace("%INPUT%", file));
            }
            _wrapIn.IgnoreInput = false;
            _filePath = file;
            _encode = encode;
            _download = false;
            Init();
        }

        public Converter(String file, String URL)
        {
            _wrapIn = new Wrapper(Config.FFMpegPath, Config.FFMpegDecDL);
            _wrapIn.IgnoreInput = true;
            _downloader = new Wrapper(Config.YoutubeDLPath, Config.YoutubeDLGet.Replace("%INPUT%", URL));
            _downloader.IgnoreInput = true;
            //EXPECT WRAPPERS RE-PLACED WHILE UNPACKING
            _filePath = file;
            _encode = false;
            _download = true;
            Init();
        }

        ~Converter()
        {
            _wrapIn.Stop();
        }

        private void WriteHeader(FrameManager frameManager)

        {

            var header = new FileHeader();
            FileInfo file = (new FileInfo(_filePath));

            header.Length = file.Length;
            header.BBP = 1;
            header.FileName = Path.GetFileName(_filePath);

            BinaryFormatter bf = new BinaryFormatter();

            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, header);
            int len = (int)ms.Position;

            ms.Position = 0;
            byte[] buf = new byte[len];
            ms.Position = 0;
            ms.Read(buf, 0, len);

            _biter.Push(buf);
            buf = _biter.GetBytes();

            if (Config.BytesInFrame < len)
            {
                throw new Exception("File name too long");
            }
            _frameManager.Write(buf);
        }



        public void Pack()
        {
            new Thread(() =>
            {
                state.Set(ConverterStateEnum.Working);
                workMeter.Reset();
                _isWorking = true;
                _workDone = false;

                _frameManager = new FrameManager(Config.Block, 1);

                _wrapIn.StarByte();

                var rnd = new Random();
                byte[] buff = new byte[Config.BytesInFrame];

                WriteHeader(_frameManager);
                workMeter.workTotal = (new FileInfo(_filePath)).Length;
                _file = new FileStream(_filePath, FileMode.Open);

                Byte[] buf = new Byte[Config.BlockSize];
                int i;
                int k;

                Byte[] arr;

                while ((i = _file.Read(buf, 0, buf.Length)) > 0)
                {
                    workMeter.Add(i);
                    if (i < buf.Length)
                    {
                        arr = new Byte[i];
                        Buffer.BlockCopy(buf, 0, arr, 0, i);
                    }
                    else
                    {
                        arr = buf;
                    }
                    k = _biter.Push(arr);
                    if (k == 0)
                    {
                        state.Set(ConverterStateEnum.Error);
                        throw new Exception("Biter pushing goes wrong");
                    }
                    arr = _biter.GetBytes();

                    _frameManager.Write(arr);

                    while (_frameManager.IsDataReady)
                    {
                        _wrapIn.Write(_frameManager.ReadFrame());
                    }
                                       
                }
                if (_frameManager.IsDataHere)
                {
                    _wrapIn.Write(_frameManager.ReadFrame());
                }
                _file.Flush();
                _file.Close();
                
                _wrapIn.Stop();
                _isWorking = false;
                _workDone = true;
                if (state.Get() != ConverterStateEnum.Error)
                {
                    state.Set(ConverterStateEnum.Done);
                }

            }
            ).Start();
        }

        public FileHeader ReadHeader(Frame frame, MemoryStream ms)
        {
            byte[] buf;
            buf = frame.GetBytes();

            _byter.Push(buf);
            buf = _byter.GetBytes();
            BinaryFormatter bf = new BinaryFormatter();

            FileHeader header = (FileHeader)bf.Deserialize(ms);

            return header;
        }

        public void Unpack()
        {

            workMeter.Reset();
            (new Thread(() =>
            {
                state.Set(ConverterStateEnum.Working);
                _isWorking = true;
                _workDone = false;
                _wrapIn.StarByte();
                if (_download)
                {
                    _downloader.StarByte();
                }

                Byte[] buf = null;
               // Byte[] tmp = null;
                FileHeader fh = null;
                FileStream file = null;
                MemoryStream ms = new MemoryStream(Config.BlockSize * 2);
                _frameManager = new FrameManager(1, Config.Block);
                //int j;

                int bytesRead = 0;
                int bytesWrite = 0;
                if (_download)
                {
                    (new Thread(() =>
                       {
                           while (true)
                           {
                               var dbuf = _downloader.Read();
                               if (dbuf != null)
                               {
                                   _wrapIn.Write(dbuf);
                               }
                               else
                               {
                                   if (
                                       (!_downloader.IsRunning) ||
                                       (state.Get() != ConverterStateEnum.Working)
                                      )

                                   {                                      
                                       break;
                                   }
                               }
                           }
                           _downloader.Stop();
                       }
                     )
                     ).Start();
                }

                while (true)
                {
                    buf = _wrapIn.Read();

                    if ((buf == null))
                    {
                        if (!(_wrapIn.IsRunning))
                        {
                            break;
                        }
                        _wrapIn.Wait();
                        continue;
                    }
                                      
                    if (bytesRead >= 0)
                    {
                        bytesRead += buf.Length;
                    }

                    _frameManager.Write(buf);


                    if (!_frameManager.IsDataReady)
                    {
                        continue;
                    }

                    _byter.Push(_frameManager.ReadFrame());
                    buf = _byter.GetBytes();

                    if (bytesRead != -1)
                    {
                        ms.Write(buf, 0, buf.Length);
                        if (bytesRead >= Config.BytesInFrame)
                        {
                            ms.Position = 0;
                            try
                            {
                                fh = (FileHeader)(new BinaryFormatter()).Deserialize(ms);
                            }
                            catch (Exception)
                            {
                                state.Set(ConverterStateEnum.Error);
                                MessageBox.Show("File header corrupted or wrong file!");
                                break;
                            }

                            bytesWrite = (int)fh.Length;

                            workMeter.workTotal = fh.Length;

                            file = new FileStream(Path.GetDirectoryName(_filePath) + @"\Videofy " + fh.FileName, FileMode.Create);

                            buf = new Byte[ms.Length - ms.Position];
                            ms.Read(buf, 0, buf.Length);
                            bytesRead = -1;
                        }
                    }
                    
                    if (bytesWrite > buf.Length)
                    {
                        file.Write(buf, 0, buf.Length);
                        file.Flush();
                        bytesWrite -= buf.Length;
                        workMeter.Add(buf.Length);
                    }
                    else
                    {
                        file.Write(buf, 0, bytesWrite);
                        file.Flush();
                        workMeter.Add(bytesWrite);
                        break;
                    }                    
                }
     
                if (file != null)
                {
                    file.Close();
                }
                _wrapIn.Stop();
                KillAll();
               
                if (state.Get() != ConverterStateEnum.Error)
                {
                    state.Set(ConverterStateEnum.Done);
                }

                _workDone = true;
                _isWorking = false;

            }
            )
            ).Start();

        }

        public float error
        {
            get { return _byter.error; }
        }

        public Boolean workDone
        {
            get { return _workDone; }
        }

        public void workReset()
        {
            if (!_isWorking)
            {
                _workDone = false;
            }
        }

        public void KillAll()
        {
            try
            {
                _downloader._proc.Kill();
            }
            catch
            {
            }

            try
            {
                _wrapIn._proc.Kill();
            }
            catch
            {
            }

        }

        public void Dispose()
        {
            KillAll();
        }


    }
}
