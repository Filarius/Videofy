using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Videofy.Chain.Helpers;

namespace Videofy.Chain
{
    class NodeWriter:ChainNode
    {
        private FileStream writer;
        private long size;
        private WorkMonitor Monitor;
        private NodeToken token;

        public NodeWriter(string path,long size, IPipe input,WorkMonitor Monitor,NodeToken token) : base(input,null)
        {
            //if (!File.Exists(path)) throw new Exception("File does not exists: " + path);
            this.size = size;
            this.token = token;
            if(File.Exists(path))
            {
                string temp = Path.GetDirectoryName(path)
                    + @"\"
                    + Path.GetFileNameWithoutExtension(path)
                    + "({0})"
                    + Path.GetExtension(path);

                int i = 1;
                do
                {
                    path = String.Format(temp, i);
                    i++;
                } while (File.Exists(path));
            }
            writer = new FileStream(path, FileMode.Create);
            Monitor.TotalWork = (int)size;
            this.Monitor = Monitor;

        }

        public override void Start()
        {
            long pos = 0;
            while(true)
            {
                if(token.token)
                { break; }
                byte[] temp = Input.Take();
                if (temp == null)
                    break;
                if (pos + temp.Length < size)
                {
                    writer.Write(temp, 0, temp.Length);
                    pos += temp.Length;
                    Monitor.Add(temp.Length);
                }
                else
                {
                    writer.Write(temp, 0, (int)(size - pos));
                    Monitor.Add((int)(size - pos));
                    break;
                }
            }
            Monitor.Add(-1);
            writer.Close();
                        
        }
    }
}
