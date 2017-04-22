using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Videofy.Chain
{
    class NodeReader:ChainNode
    {
        private FileStream reader;
        

        public NodeReader(string path,Pipe output) : base(null,output/*,null*/)
        {
            if (!File.Exists(path)) throw new Exception("File does not exists: " + path);            
            reader = new FileStream(path,FileMode.Open);            
        }

        public override void Start()
        {
            //debug
            //int cnt = 0;
            //debug
            byte[] buf = new byte[blockSize];
            int i;
            while((i = reader.Read(buf,0,blockSize)) > 0)
            {
                //cnt += i;
                //Console.WriteLine("Nodereader " + cnt.ToString());
                byte[] temp = new byte[i];
                Array.Copy(buf, 0, temp, 0, i);
                Output.Add(temp);
                
            }
            Output.Complete();
        }

        
    }
}
