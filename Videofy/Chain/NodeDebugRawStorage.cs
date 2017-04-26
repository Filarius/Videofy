using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Videofy.Chain
{
    class NodeDebugRawStorage:ChainNode
    {
        private FileStream file;

        public NodeDebugRawStorage(IPipe input,IPipe output):base(input,output)
        {
            if(input!= null)
            {
                file = new FileStream("DebugStorage.txt", FileMode.Create);
            }
            else if (output!=null)
            {
                file = new FileStream("DebugStorage.txt", FileMode.Open);
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        public override void Start()
        {
            if (Input != null)
            {
                while(true)
                {
                    byte[] temp = Input.Take();
                    if (temp == null)
                        break;
                    file.Write(temp, 0, temp.Length);
                }
                file.Close();                
            }
            else if (Output!= null)
            {
                byte[] temp = new byte[blockSize];
                while(true)
                {
                    int i = file.Read(temp, 0, blockSize);
                    if (i == 0)
                        break;
                    byte[] buf = new byte[i];
                    Array.Copy(temp, buf, i);
                    Output.Add(buf);
                }
                Output.Complete();
                file.Close();
            }
            Console.WriteLine("DONE");
            //Output.Complete();
        }
       
        
    }
}
