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
        

        public NodeWriter(string path, Pipe input) : base(input,null)
        {
            //if (!File.Exists(path)) throw new Exception("File does not exists: " + path);
            writer = new FileStream(path, FileMode.Create);
        }

        public override void Start()
        {
            while(true)
            {
                byte[] temp = Input.Take();
                if (temp == null)
                    break;
                writer.Write(temp, 0, temp.Length);
            }
            writer.Close();            
        }
    }
}
