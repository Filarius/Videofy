using System;

namespace Videofy.Tools
{
    [Serializable]
    class FileHeader
    {
        public UInt16 ClassLength;
        public Int32 BBP;        
        public Int64 Length;
        public String FileName;
        
    }
}
