using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Randomizer
{
    public class EmptyBlock: Block
    {
        public static new int TotalBlockLength = 0;
        public new byte[] blockbuffer = Array.Empty<byte>(); // VS suggestion
        public EmptyBlock() { }
    }
}
