using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Videofy.Chain
{
    class NodeWriter:ChainNode
    {
        private FileStream writer;
        private long size;

        public NodeWriter(string path,long size, IPipe input) : base(input,null)
        {
            //if (!File.Exists(path)) throw new Exception("File does not exists: " + path);
            this.size = size;
            writer = new FileStream(path, FileMode.Create);
        }

        public override void Start()
        {
            long pos = 0;
            while(true)
            {
                byte[] temp = Input.Take();
                if (temp == null)
                    break;
                if (pos + temp.Length < size)
                {
                    writer.Write(temp, 0, temp.Length);
                    pos += temp.Length;
                }
                else
                {
                    writer.Write(temp, 0, (int)(size - pos));
                    break;
                }
            }
            writer.Close();            
        }
    }
}
