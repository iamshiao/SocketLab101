using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SyncSocket
{
    [Serializable]
    public class SocketPack
    {
        public string DataType { get; set; }

        public string FileName { get; set; }

        public byte[] DataBody { get; set; }

        public List<SocketPack> SubPacks;
    }
}
