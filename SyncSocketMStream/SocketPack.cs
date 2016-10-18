using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SyncSocket
{
    [DataContract(Name = "SocketPack", Namespace = "SyncSocket")]
    public class SocketPack
    {
        [DataMember(Name = "DataType", Order = 0)]
        public string DataType { get; set; }

        [DataMember(Name = "FileName", Order = 1)]
        public string FileName { get; set; }

        [DataMember(Name = "DataBody", Order = 2)]
        public byte[] DataBody { get; set; }

        [DataMember(Name = "Packs", Order = 3)]
        public List<SocketPack> SubPacks;
    }
}
