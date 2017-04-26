﻿using System;
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
            opt.cellCount = 1;
            opt.density = 1;
            opt.pxlFmtIn = PixelFormat.YUV420P;
            opt.pxlFmtOut = PixelFormat.YUV420P;
            opt.resolution = ResolutionsEnum.p720;

            OptionsStruct headopt = opt;
            headopt.density = 1;
            headopt.cellCount = 1;




            Pipe pipein;
            Pipe pipeout;
            ChainNode node;
            List<ChainNode> solidChain = new List<ChainNode>();
            pipeout = new Pipe(_tokenSource.Token);
            node = new NodeFrameToMP4("out.mp4", opt, pipeout);
            pipein = pipeout;
            solidChain.Insert(0, node);

            pipeout = new Pipe(_tokenSource.Token);
            node = new NodeBlocksToFrame(opt, pipeout, pipein);
            pipein = pipeout;
            solidChain.Insert(0, node);

            Parallel.ForEach(solidChain, (n) => n.Start());

            Pipe pipeintemp;
            Pipe pipeouttemp;
            List<ChainNode> headerChain = new List<ChainNode>();


            DataHeader.ToPipe(path, headopt, pipein);

            /*
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
            pipeout = pipein;
            
            pipein = new Pipe(_tokenSource.Token);
            node = new NodeBlocksToFrame(opt, pipeout, pipein);
            nodes.Add(node);
            pipeout = pipein;
            
            pipein = new Pipe(_tokenSource.Token);
            node = new NodeFrameToMP4("out.mp4", opt, pipeout);
            //node = new NodeDebugRawStorage(pipeout, null);
            nodes.Add(node);

            Parallel.ForEach(nodes, (n) => n.Start());
            */
            /*
            List<Action> actions = new List<Action>();
            ChainNode node;
            Pipe pipe = new Pipe(_tokenSource.Token);

            //node = new NodeReader(path, pipe);
            //actions.Add(()=>node.Start());
            actions.Add(() => (new NodeReader(path, pipe)).Start());

            //node = new NodeDebugRawStorage(pipe,null);
            //actions.Add(() => node.Start());              
            actions.Add(() => (new NodeDebugRawStorage(pipe, null)).Start());

            Parallel.ForEach(actions, (a) => a.Invoke());
            */


        }

        public void DecodeFile(string path)
        {


            int width = (new MP4Info("out.mp4")).GetWidth();


            OptionsStruct opt = new OptionsStruct();
            opt.cellCount = 1;
            opt.density = 1;
            opt.pxlFmtIn = PixelFormat.YUV420P;
            opt.pxlFmtOut = PixelFormat.YUV420P;
            opt.resolution = (ResolutionsEnum)width;

            List<ChainNode> nodes = new List<ChainNode>();

            Pipe pipein = new Pipe(_tokenSource.Token);
            ChainNode node;
            /*
            node = new NodeDebugRawStorage(null,pipein);
            */
            node = new NodeFrameFromMP4("out.mp4", opt, pipein);
            nodes.Add(node);
            Pipe pipeout = pipein;

            pipein = new Pipe(_tokenSource.Token);
            node = new NodeBlocksFromFrame(opt, pipeout, pipein);
            nodes.Add(node);
            pipeout = pipein;

            pipein = new Pipe(_tokenSource.Token);
            node = new NodeBitsFromBlock(opt, pipeout, pipein);
            nodes.Add(node);
            pipeout = pipein;
            
            pipein = new Pipe(_tokenSource.Token);
            node = new NodeFromBits(pipeout, pipein);
            nodes.Add(node);
            pipeout = pipein;


            
            pipein = new Pipe(_tokenSource.Token);
            node = new NodeWriter(path, pipeout);
            nodes.Add(node);      
            
                  

            Parallel.ForEach(nodes, (n) => n.Start());
            /*
            List<Action> actions = new List<Action>();
            ChainNode node;
            Pipe pipe = new Pipe(_tokenSource.Token);

            node = new NodeDebugRawStorage(null, pipe);
            actions.Add(() => node.Start());
            

            node = new NodeWriter(path, pipe);
            actions.Add(() => node.Start());


            Parallel.Invoke(actions.ToArray());  
            */

        }

    }
}