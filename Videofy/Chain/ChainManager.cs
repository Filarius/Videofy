using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Videofy.Main;
using Videofy.Chain.Helpers;

namespace Videofy.Chain
{
    class ChainManager
    {
        private CancellationTokenSource _tokenSource;

        public ChainManager()
        {
            _tokenSource = new CancellationTokenSource();
        }

        public void EncodeFile(string path)
        {
            OptionsStruct opt = new OptionsStruct(0);
            opt.encodingPreset = EncodingPreset.medium;
            opt.cellCount = 1;
            opt.density = 4;
            opt.isEncodingCRF = false;
            opt.videoQuality = 20000000;
            opt.pxlFmtIn = PixelFormat.YUV420P;
            opt.pxlFmtOut = PixelFormat.YUV420P;
            opt.resolution = ResolutionsEnum.p720;

            OptionsStruct headopt = opt;
            headopt.density = 1;
            headopt.cellCount = 1;            

            List<ChainNode> nodes = new List<ChainNode>();
            Pipe pipein = new Pipe(_tokenSource.Token);
            ChainNode node;
            node = new NodeReader(path, pipein);
            nodes.Add(node);
            Pipe pipeout = pipein;

            pipein = new Pipe(_tokenSource.Token);
            node = new NodeToBits(pipeout, pipein);
            nodes.Add(node);
            pipeout = pipein;

            pipein = new Pipe(_tokenSource.Token);
            node = new NodeBitsToBlock(opt, pipeout, pipein);
            nodes.Add(node);
            Pipe pipebody = pipein;

            //HEADER START
            pipein = new Pipe(_tokenSource.Token);
            DataHeader.ToPipe(path, opt, pipein);
            pipeout = pipein;
            pipein = new Pipe(_tokenSource.Token);
            node = new NodeToBits(pipeout, pipein);
            nodes.Add(node);

            pipeout = pipein;
            pipein = new Pipe(_tokenSource.Token);
            node = new NodeBitsToBlock(headopt, pipeout, pipein);                        
            Pipe pipeheader = pipein;                     
            nodes.Add(node);
            //HEADER END

            var plist = new List<Pipe>();
            plist.Add(pipeheader);
            plist.Add(pipebody);
            IPipe pipejoiner = new PipeJoiner(_tokenSource.Token, plist);            

            pipein = new Pipe(_tokenSource.Token);
            node = new NodeBlocksToFrame(opt, pipejoiner, pipein);
            nodes.Add(node);
            pipeout = pipein;

            pipein = new Pipe(_tokenSource.Token);
            node = new NodeFrameToMP4("out.mp4", opt, pipeout);
            //node = new NodeDebugRawStorage(pipeout, null);
            nodes.Add(node);

            Task.Run(() =>
                (Parallel.ForEach(nodes, (n) => n.Start()))
             );

            int f = 0;
        }

        public void DecodeFile(string path)
        {
            int width = (new MP4Info("out.mp4")).GetWidth();

            OptionsStruct opt = new OptionsStruct(0);
            opt.cellCount = 1;
            opt.density = 1;
            opt.pxlFmtIn = PixelFormat.YUV420P;
            opt.pxlFmtOut = PixelFormat.YUV420P;
            opt.resolution = (ResolutionsEnum)width;

            OptionsStruct headopt = opt;
            opt.cellCount = 1;
            opt.density = 1;

            List<ChainNode> nodes = new List<ChainNode>();

            Pipe pipein = new Pipe(_tokenSource.Token);
            ChainNode node;
            /*
            node = new NodeDebugRawStorage(null,pipein);
            */
            node = new NodeFrameFromMP4("out.mp4", opt, pipein);
            nodes.Add(node);
            ChainNode n1 = node;
            Pipe pipeout = pipein;

            pipein = new Pipe(_tokenSource.Token);
            node = new NodeBlocksFromFrame(opt, pipeout, pipein);
            ChainNode n2 = node;
            nodes.Add(node);
            pipeout = pipein;

            // Task.Run(()=>Parallel.ForEach(nodes, (n) => n.Start()));
            Task.Run(() => { n1.Start(); });
            Task.Run(() => { n2.Start(); });

            nodes = new List<ChainNode>();
            //HEADER START
            string fileName = "";
            long fileSize = 0;
            DataHeader.FromPipe(ref opt, ref fileName, ref fileSize, pipeout);
            fileName = System.IO.Path.GetDirectoryName(path) +@"\"+ fileName;
            //HEADER END

            pipein = new Pipe(_tokenSource.Token);
            node = new NodeBitsFromBlock(opt, pipeout, pipein);
            nodes.Add(node);
            pipeout = pipein;
            
            pipein = new Pipe(_tokenSource.Token);
            node = new NodeFromBits(pipeout, pipein);
            nodes.Add(node);
            pipeout = pipein;
            
            pipein = new Pipe(_tokenSource.Token);
            node = new NodeWriter(fileName, fileSize, pipeout);
            nodes.Add(node);


            Task.Run(() =>
                Parallel.ForEach(nodes, (n) => n.Start())
            );

        }

    }
}
