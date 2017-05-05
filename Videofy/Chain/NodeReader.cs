using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Videofy.Chain.Helpers;

namespace Videofy.Chain
{
    class NodeReader:ChainNode
    {
        private FileStream reader;
        private WorkMonitor Monitor;

        public NodeReader(string path,IPipe output,WorkMonitor Monitor) : base(null,output/*,null*/)
        {
            if (!File.Exists(path)) throw new Exception("File does not exists: " + path);            
            reader = new FileStream(path,FileMode.Open);            
            var fi = new FileInfo(path);
            Monitor.TotalWork = (int)fi.Length;
            this.Monitor = Monitor;
        }

        public override void Start()
        {
            //byte[] buf = new byte[blockSize];
            byte[] buf = new byte[10240];
            int i;
            while((i = reader.Read(buf,0,10240)) > 0)
            {
                //cnt += i;
                //Console.WriteLine("Nodereader " + cnt.ToString());
                byte[] temp = new byte[i];
                Array.Copy(buf, 0, temp, 0, i);
                Output.Add(temp);
                Monitor.Add(i);                
            }
            Monitor.Add(-1);
            reader.Close();
            Output.Complete();
        }

        
    }
}
